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

namespace Creeper.Generator.Provider.MySql
{
	internal class MySqlGeneratorProvider : CreeperGeneratorProvider
	{

		public MySqlGeneratorProvider() { }

		private MySqlGeneratorProvider(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) : base(generateRules, options, connection)
		{
		}

		public override DataBaseKind DataBaseKind => DataBaseKind.MySql;

		public override CreeperGenerateConnection GetDbConnectionOptionFromString(string conn)
		{
			var strings = conn.Split(';');
			var connectionString = string.Empty;
			string dbName = null;
			foreach (var item in strings)
			{
				var sp = item.Split('=');
				var left = sp[0];
				var right = sp[1];

				switch (left.ToLower())
				{
					//server=192.168.1.15;userid=root;pwd=123456;port=3306;database=demo;sslmode=none;
					case "source":
						if (!sp[1].Contains(':')) throw new ArgumentException("source未包含port名称, 如: 127.0.0.1:3306");
						var source = sp[1].Split(':');
						connectionString += $"server={source[0]};";
						connectionString += $"port={source[1]};";
						break;
					case "user": connectionString += $"userid={right};"; break;
					case "pwd": connectionString += $"pwd={right};"; break;
					case "db": connectionString += $"database={right};"; break;
					case "name": dbName = right.ToUpperPascal(); break;
				}
			}
			connectionString += $"sslmode=none;";
			dbName = string.IsNullOrWhiteSpace(dbName) ? DataBaseKind.ToString() : dbName;
			var connection = new CreeperGenerateConnection(dbName, new CreeperGeneratorConnection(connectionString, new MySqlGeneratorConverter()));
			return connection;
		}
		public override string GetAttributeTableName(TableViewInfo tableView) => $"`{tableView.Name}`";

		private static readonly string[] _notAddQues = {
			"string",
			"JToken",
			"byte[]",
			"object",
			"IPAddress",
			"Dictionary<string, string>",
			"Creeper.MySql.Types.MySqlGeometry",
			"Creeper.MySql.Types.MySqlGeometryCollection",
			"Creeper.MySql.Types.MySqlLineString",
			"Creeper.MySql.Types.MySqlMultiLineString",
			"Creeper.MySql.Types.MySqlMultiPoint",
			"Creeper.MySql.Types.MySqlMultiPolygon",
			"Creeper.MySql.Types.MySqlPoint",
			"Creeper.MySql.Types.MySqlPolygon"
		};
		public override List<TableInfo> GetAllColumns()
		{
			using var connection = GenerateConnection.Connection.GetConnection();
			var db = connection.Database;
			var sql = @$"
SELECT 
	(CASE b.`TABLE_TYPE` WHEN 'BASE TABLE' THEN 0 ELSE 1 END) AS `IsView`, 
	(CASE WHEN b.`TABLE_COMMENT` = 'VIEW' AND b.`TABLE_TYPE` = 'VIEW' THEN '' ELSE b.`TABLE_COMMENT` END) AS `TableDescription`,
  b.`TABLE_NAME` AS `TableName` ,
	`COLUMN_COMMENT` as `ColumnDescription`,
	`COLUMN_NAME` AS `Name`,
	`IS_NULLABLE` = 'YES' AS `IsNullable`,
	`DATA_TYPE` AS `DataType`,
	`CHARACTER_MAXIMUM_LENGTH` AS `Length`,
	`COLUMN_KEY` = 'PRI' AS `IsPrimary`,
	`EXTRA` = 'auto_increment' AS `IsIdentity`,
	`COLUMN_KEY` = 'UNI' AS `IsUnique`
FROM `INFORMATION_SCHEMA`.`COLUMNS` a 
INNER JOIN `INFORMATION_SCHEMA`.`TABLES` b ON b.`TABLE_NAME` = a.TABLE_NAME AND a.`TABLE_SCHEMA` = b.`TABLE_SCHEMA`
WHERE b.`TABLE_SCHEMA`='{db}'  ORDER BY b.`TABLE_NAME`, `ORDINAL_POSITION`;
";
			var tableColumns = GetAllColumns(sql);
			foreach (var tableColumn in tableColumns)
			{
				foreach (var column in tableColumn.Columns)
				{
					column.CsharpTypeName = Types.ConvertMySqlDataTypeToCSharpType(column.DataType, column.Length);
					if (column.DataType == "enum")
						column.CsharpTypeName = GetTableNameForCamel(tableColumn.Table) + GeneratorHelper.ExceptUnderlineToUpper(column.Name);

					if (column.IsNullable && !_notAddQues.Contains(column.CsharpTypeName))
						column.CsharpTypeName += "?";
				}
			}
			return tableColumns;
		}
		public override List<EnumType> GetEnumTypes()
		{
			var enumTypes = new List<EnumType>();
			using var connection = GenerateConnection.Connection.GetConnection();
			var db = connection.Database;
			var sql = $@"
SELECT 
	`TABLE_NAME` AS `tablename`,
	`COLUMN_NAME` AS `name`,
	`COLUMN_TYPE` AS `columntype`
FROM `INFORMATION_SCHEMA`.`COLUMNS` 
WHERE `TABLE_SCHEMA`='{db}' and `DATA_TYPE` = 'enum' ORDER BY `ORDINAL_POSITION`
		";
			var list = GenerateConnection.DbExecute.ExecuteReaderList<(string tableName, string columnType, string name)>(sql);
			if (list.Count == 0)
				return enumTypes;

			foreach (var (tableName, name, columnType) in list)
			{

				if (!columnType.StartsWith("enum(", StringComparison.OrdinalIgnoreCase))
					continue;
				var elementString = columnType.Replace("enum(", "").Replace(")", "");

				var elist = new List<string>();
				foreach (var e in elementString.Split(','))
				{
					var element = e.Trim('\'');
					if (StartWithNumberRegex.IsMatch(element))
						throw new CreeperNotSupportedException("暂不支持起始为数字的枚举成员: " + tableName + '.' + name);
					elist.Add(element);
				}

				if (elist.Count == 0)
					continue;

				var enumType = new EnumType
				{
					CsharpTypeName = GeneratorHelper.ExceptUnderlineToUpper(tableName) + GeneratorHelper.ExceptUnderlineToUpper(name),
					Elements = elist.ToArray()
				};
				enumTypes.Add(enumType);
			}
			return enumTypes;
		}

		public override ICreeperGeneratorProvider CreateInstance(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection)
		{
			return new MySqlGeneratorProvider(generateRules, options, connection);
		}
	}
}
