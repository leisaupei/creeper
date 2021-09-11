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

namespace Creeper.Generator.Provider.Sqlite
{
	internal class SqliteGeneratorProvider : CreeperGeneratorProvider
	{
		public SqliteGeneratorProvider() { }

		private SqliteGeneratorProvider(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) : base(generateRules, options, connection) { }

		public override DataBaseKind DataBaseKind => DataBaseKind.Sqlite;

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
					//data source=../../../../../sql/sqlitedemo.db
					case "source": connectionString += $"data source={right};"; break;
					case "user": connectionString += $"uid={right};"; break;
					case "pwd": connectionString += $"pwd={right};"; break;
					case "db": connectionString += $"database={right};"; break;
					case "name": dbName = right.ToUpperPascal(); break;
				}
			}
			connectionString += $"Max Pool Size=512;";
			dbName = string.IsNullOrWhiteSpace(dbName) ? DataBaseKind.ToString() : dbName;
			var connection = new CreeperGenerateConnection(dbName, new CreeperGeneratorConnection(connectionString, new SqliteGeneratorConverter()));
			return connection;
		}

		private static readonly string[] _notAddQues = {
			"string",
			"JToken",
			"byte[]",
			"object",
			"IPAddress",
			"Dictionary<string, string>"
		};

		public override List<TableInfo> GetAllColumns()
		{
			var result = new List<TableInfo>();
			var tables = GenerateConnection.DbExecute.ExecuteReaderList<(string Type, string Name, string sql)>
				("select type, name, sql from sqlite_master  where name not like '/_%' escape '/' and name not in ('sqlite_sequence') and type in ('view','table')");
			foreach (var t in tables)
			{
				var uniKey = new List<string>();
				var idenkey = string.Empty;
				var indexStrings = GenerateConnection.DbExecute.ExecuteReaderList<string>($"select sql from sqlite_master where tbl_name = '{t.Name}' and type = 'index' and sql is not null");

				foreach (var item in indexStrings)
					if (item.Contains("UNIQUE"))
						uniKey.Add(item.ToLower().Split('(')[1].TrimEnd(')').Split('"')[1]);

				var s = t.sql.ToLower().Replace(Environment.NewLine, "");
				s = s[(s.IndexOf('(') + 1)..(s.Length - 1)];
				var keySentens = s.Split(',');
				foreach (var item in keySentens)
				{
					if (string.IsNullOrEmpty(item)) continue;
					if (item.Contains("autoincrement"))
						idenkey = item.Split("\"")[1];
					if (item.Contains("constraint") && item.Contains("unique"))
						uniKey.Add(item.Split('(')[1].TrimEnd(')').Split('"')[1]);
				}

				var tableInfo = new TableInfo() { Table = new TableViewInfo { Name = t.Name, IsView = t.Type == "view" } };

				var tableColumns = GenerateConnection.DbExecute.ExecuteReaderList<(int id, string name, string type, bool notNull, string defaultValue, bool pk)>($"PRAGMA table_info({tableInfo.Table.Name})");

				foreach (var tableColumn in tableColumns)
				{
					var column = new ColumnInfo
					{
						Name = tableColumn.name,
						DataType = tableColumn.type.ToLower(),
						DefaultValue = tableColumn.defaultValue,
						IsNullable = !tableColumn.notNull,
						IsPrimary = tableColumn.pk,
						IsUnique = uniKey.Contains(tableColumn.name),
					};
					column.IsIdentity = column.Name == idenkey;
					column.CsharpTypeName = Types.ConvertSqliteDataTypeToCSharpType(column.DataType);

					if (column.IsPrimary && column.CsharpTypeName == "int") column.CsharpTypeName = "long";

					if (column.IsNullable && !_notAddQues.Contains(column.CsharpTypeName))
						column.CsharpTypeName += "?";

					tableInfo.Columns.Add(column);
				}
				result.Add(tableInfo);
			}
			return result;
		}

		public override string GetAttributeTableName(TableViewInfo tableView) => $"\\\"{tableView.Name}\\\"";

		public override ICreeperGeneratorProvider CreateInstance(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) => new SqliteGeneratorProvider(generateRules, options, connection);
	}
}
