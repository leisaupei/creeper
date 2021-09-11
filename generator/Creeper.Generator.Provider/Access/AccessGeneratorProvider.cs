using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Extensions;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generic;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.Generator.Provider.Access
{
	internal class AccessGeneratorProvider : CreeperGeneratorProvider
	{
		public AccessGeneratorProvider() { }

		private AccessGeneratorProvider(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) : base(generateRules, options, connection) { }

		public override DataBaseKind DataBaseKind => DataBaseKind.Access;

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
					case "source": connectionString += $"Data Source={sp[1]};"; break;
					case "user": connectionString += $"User ID={sp[1]};"; break;
					case "pwd": connectionString += $"Jet OLEDB:Database Password={sp[1]};"; break;
					case "db": connectionString += $"database={sp[1]};"; break;
					case "provider": connectionString += $"Provider={sp[1]};"; break;
					case "name": dbName = sp[1].ToUpperPascal(); break;
				}
			}
			dbName = string.IsNullOrWhiteSpace(dbName) ? DataBaseKind.ToString() : dbName;
			var connection = new CreeperGenerateConnection(dbName, new CreeperGeneratorConnection(connectionString, new AccessGeneratorConverter()));
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
		private static readonly Regex _archivesReg = new Regex(@"^\S+_[0-9A-Z]{32}$");
		public override List<TableInfo> GetAllColumns()
		{
			var tableColumns = new List<TableInfo>();//重点语句，查询时获取键和
			using (var connection = (OleDbConnection)GenerateConnection.Connection.GetConnection())
			{
				using DataTable tableData = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
				using DataTable indexData = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Indexes, null);

				var unis = new List<string>();
				var archs = new List<string>();
				foreach (DataRow indexRow in indexData.Rows)
				{
					if (CheckBoolean(indexRow["UNIQUE"]) ?? false)
					{
						unis.Add($"{indexRow["TABLE_NAME"]}.{indexRow["COLUMN_NAME"]}");

						if (_archivesReg.IsMatch(CheckString(indexRow["INDEX_NAME"])))
							archs.Add($"{indexRow["TABLE_NAME"]}.{indexRow["COLUMN_NAME"]}");
					}
				}
				indexData.Dispose();

				foreach (DataRow tableRow in tableData.Rows)
				{
					if (CheckString(tableRow["TABLE_NAME"]).StartsWith("~")) continue;

					if (CheckString(tableRow["TABLE_TYPE"]) != "TABLE" && CheckString(tableRow["TABLE_TYPE"]) != "VIEW") continue;

					var table = new TableViewInfo
					{
						Name = CheckString(tableRow["TABLE_NAME"]),
						SchemaName = CheckString(tableRow["TABLE_SCHEMA"]),
						Description = CheckString(tableRow["DESCRIPTION"]),
						IsView = CheckString(tableRow["TABLE_TYPE"]) == "VIEW",
					};
					var tableColumn = new TableInfo { Table = table };

					DataTable columnData = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new[] { null, null, table.Name, null });
					using DataView dv = columnData.DefaultView;
					dv.Sort = "ORDINAL_POSITION asc";
					columnData = dv.ToTable();

					foreach (DataRow columnRow in columnData.Rows)
					{
						var column = new ColumnInfo
						{
							Name = CheckString(columnRow["COLUMN_NAME"]),
							DataType = CheckString(columnRow["DATA_TYPE"]),
							IsNullable = CheckBoolean(columnRow["IS_NULLABLE"]) ?? false,
							Description = CheckString(columnRow["DESCRIPTION"]),
							DefaultValue = CheckString(columnRow["COLUMN_DEFAULT"]),
						};
						var tableColumnName = string.Concat(table.Name, '.', column.Name);
						column.IsUnique = unis.Contains(tableColumnName);
						column.CsharpTypeName = Types.ConvertAccessDataTypeToCSharpType(column.DataType);

						if (column.CsharpTypeName == "string" && archs.Contains(tableColumnName))
						{
							GenerateRules.FieldIgnore.Insert = SetFileIgnore(GenerateRules.FieldIgnore.Insert, $"{tableColumn.Table.Name}.{column.Name}");
							GenerateRules.FieldIgnore.Update = SetFileIgnore(GenerateRules.FieldIgnore.Update, $"{tableColumn.Table.Name}.{column.Name}");
						}
						if (column.IsNullable && !_notAddQues.Contains(column.CsharpTypeName))
							column.CsharpTypeName += "?";

						tableColumn.Columns.Add(column);
					}
					columnData.Dispose();

					using OleDbCommand cmd = connection.CreateCommand();
					cmd.CommandText = "select top 1 * from " + table.Name;
					using OleDbDataReader dr = cmd.ExecuteReader(CommandBehavior.KeyInfo);
					var keyInfoData = dr.GetSchemaTable();

					foreach (DataRow keyInfoRow in keyInfoData.Rows)
					{
						var column = tableColumn.Columns.FirstOrDefault(a => a.Name == CheckString(keyInfoRow["ColumnName"]));
						if (column == null) continue;
						column.IsReadOnly = CheckBoolean(keyInfoRow["IsReadOnly"]) ?? false;
						column.IsIdentity = CheckBoolean(keyInfoRow["IsAutoIncrement"]) ?? false;
						column.IsPrimary = CheckBoolean(keyInfoRow["IsKey"]) ?? false;
					}

					keyInfoData.Dispose();
					tableColumns.Add(tableColumn);
				}
			}

			return tableColumns;
		}

		public override string GetAttributeTableName(TableViewInfo tableView)
			=> string.Concat(string.IsNullOrWhiteSpace(tableView.SchemaName) ? null : $"[{tableView.SchemaName}].", $"[{tableView.Name}]");

		public override ICreeperGeneratorProvider CreateInstance(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) => new AccessGeneratorProvider(generateRules, options, connection);
	}
}
