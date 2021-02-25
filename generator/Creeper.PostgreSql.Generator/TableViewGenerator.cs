using Creeper.Driver;
using Creeper.Generator.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace Creeper.PostgreSql.Generator
{
	/// <summary>
	/// 
	/// </summary>
	public class TableViewGenerator
	{
		/// <summary>
		/// 项目名称
		/// </summary>
		private string _projectName;

		/// <summary>
		/// 模型路径
		/// </summary>
		private string _modelPath;

		/// <summary>
		/// schema 名称
		/// </summary>
		private string _schemaName;

		/// <summary>
		/// 表/视图
		/// </summary>
		private TableViewModel _table;

		private readonly ICreeperDbExecute _dbExecute;
		private readonly bool _folder;

		/// <summary>
		/// 是不是空间表
		/// </summary>
		private bool _isGeometryTable = false;

		/// <summary>
		/// 是否视图
		/// </summary>
		private bool _isView = false;

		/// <summary>
		/// 字段列表
		/// </summary>
		private List<TableFieldModel> _fieldList = new List<TableFieldModel>();

		/// <summary>
		/// 主键
		/// </summary>
		private List<PrimarykeyInfo> _pkList = new List<PrimarykeyInfo>();

		/// <summary>
		/// 生成项目多库
		/// </summary>
		private string _dataBaseTypeName;

		/// <summary>
		/// 命名空间后缀
		/// </summary>
		private string NamespaceSuffix => _folder ? "." + _dataBaseTypeName : "";

		/// <summary>
		/// 多库枚举 *需要在目标项目添加枚举以及创建该库实例
		/// </summary>
		private string DbNameAttribute => $", typeof(Db" + _dataBaseTypeName + ")";

		/// <summary>
		/// Model名称
		/// </summary>
		private string ModelClassName => DalClassName + CreeperGenerator.ModelSuffix;

		/// <summary>
		/// DAL名称
		/// </summary>
		private string DalClassName => Types.DeletePublic(_schemaName, _table.Name, isView: _isView);

		/// <summary>
		/// 表名
		/// </summary>
		private string TableName => Types.DeletePublic(_schemaName, _table.Name, true, _isView).ToLowerPascal();

		public ICreeperDbExecute DbExecute => _dbExecute;

		private static readonly string[] _notAddQues = { "string", "JToken", "byte[]", "object", "IPAddress", "Dictionary<string, string>", "NpgsqlTsQuery", "NpgsqlTsVector", "BitArray", "PhysicalAddress", "XmlDocument", "PostgisGeometry" };

		/// <summary>
		/// 构建函数
		/// </summary>
		/// <param name="projectName"></param>
		/// <param name="modelPath"></param>
		/// <param name="dalPath"></param>
		/// <param name="schemaName"></param>
		/// <param name="table"></param>
		/// <param name="type"></param>
		public TableViewGenerator(ICreeperDbExecute dbExecute, bool folder)
		{
			_dbExecute = dbExecute;
			_folder = folder;
		}

		public void Generate(string projectName, string modelPath, string schemaName, TableViewModel table, string type)
		{
			_dataBaseTypeName = type.ToUpperPascal();
			_projectName = projectName;
			_modelPath = modelPath;
			_schemaName = schemaName;
			_table = table;
			Console.WriteLine($"Generating {_schemaName}.{_table.Name}...");
			GetFieldList();
			if (table.Type == "table")
			{
				GetPrimaryKey();
			}
			if (table.Type == "view")
				_isView = true;
		}

		/// <summary>
		/// 获取字段
		/// </summary>
		public void GetFieldList()
		{
			var sql = $@"
SELECT a.oid, 
	c.attnum as num, 
	c.attname AS field,
	c.attnotnull AS isnotnull, 
	d.description AS comment, 
	e.typcategory, 
	(f.is_identity = 'YES') as isidentity, 
	format_type(c.atttypid,c.atttypmod) AS type_comment, 
	c.attndims as dimensions,  
	(CASE WHEN f.character_maximum_length IS NULL THEN c.attlen ELSE f.character_maximum_length END) AS length,  
	(CASE WHEN e.typelem = 0 THEN e.typname WHEN e.typcategory = 'G' THEN format_type (c.atttypid, c.atttypmod) ELSE e2.typname END ) AS dbtype,  
	(CASE WHEN e.typelem = 0 THEN e.typtype ELSE e2.typtype END) AS datatype, ns.nspname, COALESCE(pc.contype = 'u',false) as isunique ,
	f.column_default
FROM pg_class a  
INNER JOIN pg_namespace b ON a.relnamespace = b.oid  
INNER JOIN pg_attribute c ON attrelid = a.oid  
LEFT OUTER JOIN pg_description d ON c.attrelid = d.objoid AND c.attnum = d.objsubid AND c.attnum > 0  
INNER JOIN pg_type e ON e.oid = c.atttypid  
LEFT JOIN pg_type e2 ON e2.oid = e.typelem  
INNER JOIN information_schema.COLUMNS f ON f.table_schema = b.nspname AND f.TABLE_NAME = a.relname AND COLUMN_NAME = c.attname  
LEFT JOIN pg_namespace ns ON ns.oid = e.typnamespace and ns.nspname <> 'pg_catalog'  
LEFT JOIN pg_constraint pc ON pc.conrelid = a.oid and pc.conkey[1] = c.attnum and pc.contype = 'u'  
WHERE (b.nspname='{_schemaName}' and a.relname='{_table.Name}')  
";
			_fieldList = DbExecute.ExecuteDataReaderList<TableFieldModel>(sql);

			foreach (var f in _fieldList)
			{
				f.IsArray = f.Dimensions > 0;
				f.DbType = f.DbType.StartsWith("_", StringComparison.Ordinal) ? f.DbType.Remove(0, 1) : f.DbType;
				f.PgDbType = Types.ConvertDbTypeToNpgsqlDbType(f.DataType, f.DbType, f.IsArray);
				f.PgDbTypeString = Types.ConvertDbTypeToNpgsqlDbTypeString(f.DbType, f.IsArray);
				f.IsEnum = f.DataType == "e";
				string _type = Types.ConvertPgDbTypeToCSharpType(f.DataType, f.DbType);
				if (f.DbType == "xml")
					MappingOptions.XmlTypeName.Add(_dataBaseTypeName);
				if (f.DbType == "geometry")
				{
					_isGeometryTable = true;
					MappingOptions.GeometryTableTypeName.Add(_dataBaseTypeName);
				}

				if (f.IsEnum)
					_type = Types.DeletePublic(f.Nspname, _type);
				f.CSharpType = _type;

				if (f.DataType == "c")
					f.RelType = Types.DeletePublic(f.Nspname, _type);
				else
				{
					string _notnull = "";
					if (!_notAddQues.Contains(_type) && !f.IsArray)
					{
						_notnull = f.IsNotNull ? "" : "?";
					}
					string _array = f.IsArray ? "[".PadRight(Math.Max(0, f.Dimensions), ',') + "]" : "";
					f.RelType = $"{_type}{_notnull}{_array}";
				}
				if (f.IsUnique)
				{
					if (f.Column_default?.StartsWith("nextval(") == true)
						f.IsIdentity = true;
				}
			}
		}

		/// <summary>
		/// 获取主键
		/// </summary>
		private void GetPrimaryKey()
		{
			var sqlPk = $@"
SELECT b.attname AS field,format_type (b.atttypid, b.atttypmod) AS typename 
FROM pg_index a  
INNER JOIN pg_attribute b ON b.attrelid = a.indrelid AND b.attnum = ANY (a.indkey)  
WHERE a.indrelid = '{_schemaName}.{_table.Name}'::regclass AND a.indisprimary
";
			_pkList = DbExecute.ExecuteDataReaderList<PrimarykeyInfo>(sqlPk);

			List<string> d_key = new List<string>();
			for (var i = 0; i < _pkList.Count; i++)
			{
				TableFieldModel fs = _fieldList.FirstOrDefault(f => f.Field == _pkList[i].Field);
				d_key.Add(fs.RelType + " " + fs.Field);
				if (fs.Column_default?.StartsWith("nextval(") == true)
					fs.IsIdentity = true;

			}
		}

		/// <summary>
		/// 生成Model.cs文件
		/// </summary>
		public void ModelGenerator()
		{
			string _filename = Path.Combine(_modelPath, ModelClassName + ".cs");

			using StreamWriter writer = new StreamWriter(File.Create(_filename), Encoding.UTF8);
			CreeperGenerator.WriteAuthorHeader.Invoke(writer);
			writer.Write(@"using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.NetworkInformation;
using NpgsqlTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Creeper.Attributes;
using {3}.{0}.Options;
{5}
namespace {3}.{0}.{1}{2}
{{
{4}
	[CreeperDbTable(@""""""{6}"""".""""{9}""""""{8})]
	public partial class {7} : ICreeperDbModel
	{{
		#region Properties
",
CreeperGenerator.DbStandardSuffix,
CreeperGenerator.Namespace,
NamespaceSuffix,
_projectName,
WriteComment(_table.Description, 1),
_isGeometryTable ? "using Npgsql.LegacyPostgis;" + Environment.NewLine : "",
_schemaName,
ModelClassName,
DbNameAttribute,
_table.Name);

			foreach (var item in _fieldList)
			{
				var pkAttr = string.Empty;
				if (_pkList.Any(a => a.Field == item.Field))
					pkAttr = "[CrepperPrimaryKey] ";
				if (Types.NotCreateModelFieldDbType(item.DbType, item.Typcategory))
				{
					WriteComment(item.Comment, 2);
					writer.WriteLine($"{WriteComment(item.Comment, 2)}\t\t{pkAttr}public {item.RelType} {item.FieldUpCase} {{ get; set; }}");
				}

				if (item.DbType == "geometry")
				{
					writer.WriteLine($"{WriteComment(item.Comment, 2)}\t\t{pkAttr}public {item.RelType} {item.FieldUpCase} {{ get; set; }}");
				}
			}
			writer.WriteLine("\t\t#endregion");
			writer.WriteLine();
			writer.WriteLine("\t}");
			writer.WriteLine("}");

			writer.Flush();
		}

		#region Private Method
		/// <summary>
		/// 写评论
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="comment"></param>
		private static StringBuilder WriteComment(string comment, int tab)
		{
			var sb = new StringBuilder();
			var tabStr = string.Empty;
			for (int i = 0; i < tab; i++)
				tabStr += "\t";
			if (!string.IsNullOrEmpty(comment))
			{
				if (comment.Contains("\n"))
				{
					comment = comment.Replace("\r\n", string.Concat("\n", tabStr, "/// "));
				}
				sb.AppendLine(tabStr + "/// <summary>");
				sb.AppendLine(tabStr + $"/// {comment}");
				sb.AppendLine(tabStr + "/// </summary>");
			}
			return sb;
		}

		#endregion
	}
}