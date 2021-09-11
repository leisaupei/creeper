using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Extensions;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generic;
using Creeper.PostgreSql;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Creeper.Generator.Provider.PostgreSql
{
	internal class PostgreSqlGeneratorProvider : CreeperGeneratorProvider
	{
		private bool _isGeometryConnection = false; //是否包含空间数据类型的数据库
		private bool _isXmlConnection = false; //是否包含xml类型的数据库
		private bool _isJsonConnection = false; //是否包含json/jsonb类型的数据库
		private readonly List<EnumType> _enums = new List<EnumType>();
		private readonly List<CompositeType> _composites = new List<CompositeType>();

		/// <summary>
		/// 是否需要DbConnection委托
		/// </summary>
		private bool HasNoDbConnectionOptionsAction => !_isJsonConnection && !_isXmlConnection && !_isGeometryConnection && _enums.Count == 0 && _composites.Count == 0;

		public PostgreSqlGeneratorProvider() { }

		private PostgreSqlGeneratorProvider(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) : base(generateRules, options, connection) { }

		public override DataBaseKind DataBaseKind => DataBaseKind.PostgreSql;

		public override CreeperGenerateConnection GetDbConnectionOptionFromString(string conn)
		{
			var strings = conn.Split(';');
			var connectionString = string.Empty;
			string dbName = null;
			foreach (var item in strings)
			{
				var sp = item.Split('=');

				switch (sp[0].ToLower())
				{
					case "source":
						if (!sp[1].Contains(':')) throw new ArgumentException("source未包含port名称, 如: 127.0.0.1:5432");
						var s = sp[1].Split(':');
						connectionString += $"host={s[0]};";
						connectionString += $"port={s[1]};";
						break;
					case "user": connectionString += $"username={sp[1]};"; break;
					case "pwd": connectionString += $"password={sp[1]};"; break;
					case "db": connectionString += $"database={sp[1]};"; break;
					case "name": dbName = sp[1].ToUpperPascal(); break;
				}
			}
			connectionString += $"maximum pool size=32;pooling=true;CommandTimeout=300";
			dbName = string.IsNullOrWhiteSpace(dbName) ? DataBaseKind.ToString() : dbName;
			var connection = new CreeperGenerateConnection(dbName, new CreeperGeneratorConnection(connectionString, new PostgreSqlGeneratorConverter()));
			return connection;
		}

		public override List<CompositeType> GetCompositeTypes()
		{
			var sql = $@"
SELECT 
	ns.nspname AS schemaname, 
	a.typname AS name,
	c.attname AS columnname, 
	d.typname AS datatype,
	c.attndims AS dimensions,
	d.typtype AS typcategory, 
	CAST(obj_description(a.oid,'pg_type') AS VARCHAR) AS description
FROM pg_type a 
INNER JOIN pg_class b on b.reltype = a.oid and b.relkind = 'c'
INNER JOIN pg_attribute c on c.attrelid = b.oid and c.attnum > 0
INNER JOIN pg_type d on d.oid = c.atttypid
INNER JOIN pg_namespace ns on ns.oid = a.typnamespace
LEFT JOIN pg_namespace ns2 on ns2.oid = d.typnamespace
WHERE {GeneratorHelper.ExceptConvert("ns.nspname || '.' || a.typname", GenerateRules.Excepts.Global.Composites)}
";
			GenerateConnection.DbExecute.ExecuteReader(dr =>
			{
				var composite = new CompositeType();
				var column = new ColumnInfo();
				composite.SchemaName = CheckString(dr["schemaname"]);
				composite.DbCompositeName = CheckString(dr["name"]);
				composite.Description = CheckString(dr["description"]);

				column.Name = CheckString(dr["columnname"]);
				column.DataType = CheckString(dr["datatype"]);
				column.Dimensions = CheckInt(dr["dimensions"]) ?? 0;
				column.Typcategory = CheckString(dr["typcategory"]);

				var item = _composites.FirstOrDefault(a => a.ToString() == composite.ToString());
				if (item == null)
				{
					composite.CsharpClassName = Types.DeletePublic(composite.SchemaName, composite.DbCompositeName);
					composite.Columns.Add(column);
					_composites.Add(composite);
				}
				else
				{
					item.Columns.Add(column);
				}
			}, sql);

			foreach (var item in _composites)
				foreach (var column in item.Columns)
					ChangeColumns(column);
			return _composites;
		}

		public override List<EnumType> GetEnumTypes()
		{
			var sql = @"
SELECT a.typname, b.nspname, CAST(obj_description(a.oid,'pg_type') AS VARCHAR) AS description, es.enums
FROM pg_type a  
INNER JOIN pg_namespace b ON a.typnamespace = b.oid 
inner join (
	select enumtypid, array_agg(enumlabel) enums from (
		SELECT * FROM pg_enum ORDER BY enumtypid, enumsortorder asc
	) ord
	group by enumtypid
	order by enumtypid
) es on es.enumtypid = a.oid
WHERE a.typtype = 'e'
ORDER BY es.enumtypid asc
";
			var list = GenerateConnection.DbExecute.ExecuteReaderList<(string name, string schemaName, string description, string[] enums)>(sql);

			foreach (var (name, schemaName, description, enums) in list)
			{
				_enums.Add(new EnumType
				{
					SchemaName = schemaName,
					DbEnumName = name,
					Description = description,
					Elements = enums,
					CsharpTypeName = Types.DeletePublic(schemaName, name)
				});
			}
			return _enums;
		}

		private static readonly string[] _notAddQues = { "string", "JToken", "byte[]", "object", "IPAddress", "Dictionary<string, string>", "NpgsqlTsQuery", "NpgsqlTsVector", "BitArray", "PhysicalAddress", "XmlDocument", "PostgisGeometry" };

		public override List<TableInfo> GetAllColumns()
		{
			var schemaExceptCond = GeneratorHelper.ExceptConvert("SCHEMA_NAME", GenerateRules.Excepts.Global.Schemas);
			var tableExceptCond = GeneratorHelper.ExceptConvert("schemaname ||'.'||tablename", GenerateRules.Excepts.Global.Tables);
			var viewExceptCond = GeneratorHelper.ExceptConvert("schemaname ||'.'||viewname", GenerateRules.Excepts.Global.Views);
			var sql = @$"
SELECT 
	tv.*,
	c.attname AS name,
	c.attnotnull = false AS isnullable, 
	d.description AS columndescription, 
	lower(e.typcategory) typcategory, 
	(f.is_identity = 'YES') AS isidentity, 
	c.attndims AS dimensions,  
	(CASE WHEN f.character_maximum_length IS NULL THEN c.attlen ELSE f.character_maximum_length END) AS length,  
	(CASE WHEN e.typelem = 0 THEN e.typname WHEN e.typcategory = 'G' THEN format_type (c.atttypid, c.atttypmod) ELSE e2.typname END) AS datatype,  
	ns.nspname AS datatypeschemaname, 
	COALESCE(pc.contype = 'u',false) as isunique,
	f.column_default AS defaultvalue,
	COALESCE(idx.indisprimary, false) as isprimary
FROM pg_class a  
INNER JOIN pg_namespace b ON a.relnamespace = b.oid  
INNER JOIN pg_attribute c ON attrelid = a.oid  
LEFT OUTER JOIN pg_description d ON c.attrelid = d.objoid AND c.attnum = d.objsubid AND c.attnum > 0  
INNER JOIN pg_type e ON e.oid = c.atttypid  
LEFT JOIN pg_type e2 ON e2.oid = e.typelem  
INNER JOIN information_schema.COLUMNS f ON f.table_schema = b.nspname AND f.TABLE_NAME = a.relname AND COLUMN_NAME = c.attname  
LEFT JOIN pg_namespace ns ON ns.oid = e.typnamespace and ns.nspname <> 'pg_catalog'  
LEFT JOIN pg_constraint pc ON pc.conrelid = a.oid and pc.conkey[1] = c.attnum and pc.contype = 'u'
LEFT JOIN pg_index idx ON c.attrelid = idx.indrelid AND c.attnum = ANY (idx.indkey) AND idx.indisprimary
INNER JOIN(
	SELECT schemaname, tablename, isview, tabledescription FROM (
		SELECT schemaname, tablename, false AS isview, CAST(obj_description(b.oid,'pg_class') AS VARCHAR) AS tabledescription
		FROM pg_tables a  
		LEFT JOIN pg_class b on a.tablename = b.relname AND b.relkind in ('r','p') 
		INNER JOIN pg_namespace c on c.oid = b.relnamespace AND c.nspname = a.schemaname
		WHERE {tableExceptCond}
		UNION (
			SELECT schemaname, viewname AS tablename,true AS isview, CAST(obj_description(b.oid,'pg_class') AS VARCHAR) AS TableDescription
			FROM pg_views a  
			LEFT JOIN pg_class b on a.viewname = b.relname AND b.relkind = 'v' 
			INNER JOIN pg_namespace c on c.oid = b.relnamespace AND c.nspname = a.schemaname
			WHERE {viewExceptCond}
		)  
	) tv
	WHERE schemaname in (
		SELECT SCHEMA_NAME AS schemaname FROM information_schema.schemata a WHERE {schemaExceptCond}
	)  
) tv on b.nspname = tv.schemaname and a.relname = tv.tablename
ORDER BY tv.schemaname, tv.tablename, c.attnum ASC;
";
			var tableColumns = GetAllColumns(sql);
			foreach (var tableColumn in tableColumns)
				foreach (var column in tableColumn.Columns)
					ChangeColumns(column);
			return tableColumns;
		}

		private void ChangeColumns(ColumnInfo column)
		{
			column.IsEnum = column.Typcategory == "e";
			column.IsComposite = column.Typcategory == "c";
			column.IsArray = column.Dimensions > 0;
			column.DataType = column.DataType.TrimStart('_');
			column.IsIdentity = column.DefaultValue?.StartsWith("nextval(") ?? false;
			column.IsGeometry = column.DataType == "geometry";

			if (!column.IsEnum && !column.IsComposite)
				column.CsharpTypeName = Types.ConvertPgDbTypeToCSharpType(column.DataType);

			if (column.CsharpTypeName == "JToken")
				_isJsonConnection = true;
			if (column.DataType == "xml")
				_isXmlConnection = true;
			if (column.IsGeometry)
				_isGeometryConnection = true;

			if (column.IsEnum || column.IsComposite)
				column.CsharpTypeName = Types.DeletePublic(column.SchemaName, column.DataType.ToUpperPascal());

			if (column.IsArray)
			{
				column.CsharpTypeName += "[".PadRight(Math.Max(0, column.Dimensions), ',') + "]";
				return;
			}
			if (column.IsNullable && !_notAddQues.Contains(column.CsharpTypeName))
				column.CsharpTypeName += "?";
		}

		public override List<string> GetUsingNamespace(List<ColumnInfo> columns)
		{
			var list = new HashSet<string>();
			foreach (var column in columns)
			{
				switch (column.CsharpTypeName)
				{
					case "PhysicalAddress":
						list.Add("using System.Net.NetworkInformation;");
						break;
					case "XmlDocument":
						list.Add("using System.Xml;");
						break;
					case "NpgsqlTsQuery":
					case "NpgsqlBox":
					case "NpgsqlCircle":
					case "NpgsqlTsVector":
					case "NpgsqlLine":
					case "NpgsqlLSeg":
					case "NpgsqlPath":
					case "NpgsqlPoint":
					case "NpgsqlPolygon":
						list.Add("using NpgsqlTypes;");
						break;
					case "BitArray":
						list.Add("using System.Collections;");
						break;
					case "PostgisGeometry":
						list.Add("using Npgsql.LegacyPostgis;");
						break;
					case "IPAddress":
						list.Add("using System.Net;");
						break;
					case "JToken":
						list.Add("using Newtonsoft.Json.Linq;");
						break;
					case "Dictionary":
						list.Add("using System.Collections.Generic;");
						break;
				}
			}
			return list.ToList();
		}

		public override List<string> GetDbConnectionOptionsUsingNamespace()
		{
			var hs = new HashSet<string>();
			if (!HasNoDbConnectionOptionsAction)
			{
				hs.Add("using System.Data.Common;");
				hs.Add("using Npgsql;");
			}
			if (_isJsonConnection || _isXmlConnection)
				hs.Add("using Creeper.PostgreSql.Extensions;");
			if (_isGeometryConnection)
				hs.Add("using Npgsql;");
			return hs.ToList();
		}
		public override string GetDbConnectionOptionsActionString()
		{
			if (HasNoDbConnectionOptionsAction)
				return null;

			var sb = new StringBuilder();
			sb.AppendLine("\t\tprotected override Action<DbConnection> ConnectionOptions => connection =>");
			sb.AppendLine("\t\t{");
			sb.AppendLine("\t\t\tvar c = (NpgsqlConnection)connection;");

			if (_isJsonConnection)
				sb.AppendLine("\t\t\tc.TypeMapper.UseNewtonsoftJson();");
			if (_isXmlConnection)
				sb.AppendLine("\t\t\tc.TypeMapper.UseSystemXmlDocument();");
			if (_isGeometryConnection)
				sb.AppendLine("\t\t\tc.TypeMapper.UseLegacyPostgis();");

			foreach (var item in _enums)
				sb.AppendLine($"\t\t\tc.TypeMapper.MapEnum<{Options.GetMappingNamespaceName(GenerateConnection.Name)}.{item.CsharpTypeName}>(\"{item.SchemaName}.{item.DbEnumName}\", PostgreSqlTranslator.Instance);");

			foreach (var item in _composites)
			{
				sb.AppendLine($"\t\t\tc.TypeMapper.MapComposite<{Options.GetMappingNamespaceName(GenerateConnection.Name)}.{item.CsharpClassName}>(\"{item.SchemaName}.{item.DbCompositeName}\");");
			}
			sb.Append("\t\t};");
			return sb.ToString();
		}

		public override string GetTableNameForCamel(TableViewInfo tableView)
		{
			if (tableView.SchemaName.ToLower() == "public")
				return base.GetTableNameForCamel(tableView);
			return GeneratorHelper.ExceptUnderlineToUpper(tableView.SchemaName.ToUpperPascal()) +
				GeneratorHelper.ExceptUnderlineToUpper(tableView.Name.ToUpperPascal());
		}

		public override string GetAttributeTableName(TableViewInfo tableView) => $"\\\"{tableView.SchemaName}\\\".\\\"{tableView.Name}\\\"";

		public override ICreeperGeneratorProvider CreateInstance(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) => new PostgreSqlGeneratorProvider(generateRules, options, connection);
	}
}
