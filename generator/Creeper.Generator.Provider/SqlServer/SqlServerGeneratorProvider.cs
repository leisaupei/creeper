using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Extensions;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.Generator.Provider.SqlServer
{
	internal class SqlServerGeneratorProvider : CreeperGeneratorProvider
	{
		public SqlServerGeneratorProvider() { }

		private SqlServerGeneratorProvider(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) : base(generateRules, options, connection) { }

		public override DataBaseKind DataBaseKind => DataBaseKind.SqlServer;

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
					//server=.;database=demo;uid=sa;pwd=123456;Max Pool Size=512;
					case "source": connectionString += $"server={sp[1]};"; break;
					case "user": connectionString += $"uid={sp[1]};"; break;
					case "pwd": connectionString += $"pwd={sp[1]};"; break;
					case "db": connectionString += $"database={sp[1]};"; break;
					case "name": dbName = sp[1].ToUpperPascal(); break;
				}
			}
			connectionString += $"Max Pool Size=512;";
			dbName = string.IsNullOrWhiteSpace(dbName) ? DataBaseKind.ToString() : dbName;
			var connection = new CreeperGenerateConnection(dbName, new CreeperGeneratorConnection(connectionString, new SqlServerGeneratorConverter()));
			return connection;
		}

		private static readonly string[] _notAddQues = {
			"string",
			"JToken",
			"byte[]",
			"object",
			"IPAddress",
			"Dictionary<string, string>",
			"System.Data.SqlTypes.SqlXml"
		};
		public override List<TableInfo> GetAllColumns()
		{
			var sql = @"

SELECT *FROM (
	SELECT DISTINCT
		[TableName]			= d.[name],
		[TableDescription]	= CASE WHEN a.[colorder] = 1 AND d.[xtype] <> 'V' THEN ISNULL(f.[value],'') ELSE '' END,
		[SchemaName]		= c.[name],
		[IsView]			= CASE WHEN d.[xtype] = 'V' THEN 1 ELSE 0 END,
		[Name]				= a.[name],
		[IsIdentity]		= CASE WHEN COLUMNPROPERTY(a.[id],a.[name],'IsIdentity') = 1 THEN 1 ELSE 0 END,
		[IsPrimary]			= uni.[is_primary_key],
		[DataType]			= b.[name],
		[Length]			= COLUMNPROPERTY(a.[id],a.[name],'PRECISION'),
		--小数位数			= ISNULL(COLUMNPROPERTY(a.id,a.name,'Scale'),0),
		[IsNullable]		= CASE WHEN a.[isnullable] = 1 THEN 1 ELSE 0 END,
		[DefaultValue]		= ISNULL(e.[text],''),
		[ColumnDescription] = ISNULL(g.[value],NULL),
		[IsUnique]			= CASE WHEN uni.[is_unique] = 1 and uni.[is_primary_key] != 1 THEN 1 ELSE 0 END,
		a.[id], a.[colorder]
	FROM syscolumns a
	LEFT JOIN systypes b ON a.[xusertype] = b.[xusertype]
	INNER JOIN sysobjects d ON a.[id] = d.[id] AND d.[xtype] IN ('U','V') AND  d.[name] <> 'dtproperties'
	INNER JOIN sys.schemas c ON d.uid = c.schema_id
	LEFT JOIN syscomments e ON a.[cdefault] = e.[id]
	LEFT JOIN sys.extended_properties g ON a.[id] = g.[major_id] AND a.[colid] = g.[minor_id]
	LEFT JOIN sys.extended_properties f ON d.[id] = f.[major_id] AND f.[minor_id] = 0
	LEFT JOIN (
		SELECT
			tab.[name] AS [TableName],
			col.[name] AS [ColumnName],
			idx.[is_primary_key],
			idx.[is_unique]
		FROM sys.indexes idx 
		INNER JOIN sys.index_columns idxCol ON idx.[object_id] = idxCol.[object_id] AND idx.[index_id] = idxCol.[index_id]
		INNER JOIN sys.tables tab ON idx.[object_id] = tab.[object_id]
		INNER JOIN sys.columns col ON idx.[object_id] = col.[object_id] AND idxCol.[column_id] = col.[column_id]
		WHERE idx.[is_unique] = 1 OR idx.[is_primary_key] != 1
	) uni ON uni.[TableName] = d.[name] AND  a.[name] = uni.[ColumnName]
	WHERE f.[name] IS NULL OR f.[name] <> 'microsoft_database_tools_support'
) info
order by info.[id], info.[colorder]
";
			var tableColumns = GetAllColumns(sql);
			foreach (var tableColumn in tableColumns)
			{
				foreach (var column in tableColumn.Columns)
				{
					column.CsharpTypeName = Types.ConvertSqlServerDataTypeToCSharpType(column.DataType);

					if (column.IsNullable && !_notAddQues.Contains(column.CsharpTypeName))
						column.CsharpTypeName += "?";

					if (Types.NotSupportDataType.Contains(column.DataType.ToLower()))
						column.IsNotSupported = true;
					if (column.DataType == "timestamp")
					{
						GenerateRules.FieldIgnore.Update = SetFileIgnore(GenerateRules.FieldIgnore.Update, $"{tableColumn.Table.SchemaName}{tableColumn.Table.Name}.{column.Name}");
					}
				}
			}
			return tableColumns;
		}

		public override string GetAttributeTableName(TableViewInfo tableView) => $"[{tableView.SchemaName}].[{tableView.Name}]";

		public override ICreeperGeneratorProvider CreateInstance(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) => new SqlServerGeneratorProvider(generateRules, options, connection);
	}
}
