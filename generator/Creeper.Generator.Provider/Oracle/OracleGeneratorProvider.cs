using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Extensions;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generic;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.Generator.Provider.Oracle
{
	internal class OracleGeneratorProvider : CreeperGeneratorProvider
	{
		public OracleGeneratorProvider() { }

		private OracleGeneratorProvider(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) : base(generateRules, options, connection) { }

		public override DataBaseKind DataBaseKind => DataBaseKind.Oracle;

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
					case "source": connectionString += $"data source={sp[1]};"; break;
					case "user": connectionString += $"user id={sp[1]};"; break;
					case "pwd": connectionString += $"password={sp[1]};"; break;
					case "name": dbName = sp[1].ToUpperPascal(); break;
				}
			}
			connectionString += $"Pooling=true;Max Pool Size=10;";
			dbName = string.IsNullOrWhiteSpace(dbName) ? DataBaseKind.ToString() : dbName;
			var connection = new CreeperGenerateConnection(dbName, new CreeperGeneratorConnection(connectionString, new OracleGeneratorConverter()));
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
		private static readonly Regex _identityTriggerReg = new Regex(@"(?<=(select)\s+\S+\.nextval[ \f\n\r\t\v]+into\s*:new\.""*)[a-zA-Z]+(?=""*\s+from\s+dual)", RegexOptions.IgnoreCase);
		public override List<TableInfo> GetAllColumns()
		{
			var tableColumnsSql = @"
SELECT 
    a.TABLE_NAME		as tablename,
    b.COMMENTS			as tabledescription,
    a.COLUMN_NAME		as name,
    a.DATA_TYPE			as datatype,
    a.DATA_LENGTH		as length,
    a.NULLABLE			as isnullable,
    c.comments			as columndescription,
    a.DATA_DEFAULT		as defaultvalue,
    a.DATA_PRECISION	as precision,
    a.DATA_SCALE		as scale,
	a.IDENTITY_COLUMN	as isidentity,
	(case when d.COLUMN_NAME is not null then 1 else 0 end) as isprimary
FROM USER_TAB_COLS a
inner join user_tab_comments b on a.TABLE_NAME = b.table_name
inner join user_col_comments c on c.TABLE_NAME = a.TABLE_NAME and c.COLUMN_NAME = a.COLUMN_NAME
left join (
	select cu.TABLE_NAME,cu.COLUMN_NAME 
	from user_cons_columns cu, user_constraints au 
	where cu.constraint_name = au.constraint_name and au.constraint_type = 'P'
) d on a.TABLE_NAME = d.TABLE_NAME and a.COLUMN_NAME = d.COLUMN_NAME
where b.TABLE_TYPE in ('TABLE','VIEW')
order by a.COLUMN_ID
";
			var tableColumns = new List<TableInfo>();
			GetLongResult(dr => SetTableColumnValues(dr, tableColumns), tableColumnsSql);

			var identityTriggers = new Dictionary<string, string>();
			var triggerSql = $"select TABLE_NAME,TRIGGER_BODY from user_triggers where TRIGGER_TYPE = 'BEFORE EACH ROW' and TRIGGERING_EVENT = 'INSERT' and STATUS = 'ENABLED' and TABLE_NAME in ({string.Join(", ", tableColumns.Select(a => $"'{a.Table.Name}'"))})";
			GetLongResult(dr =>
			{
				var tableName = CheckString(dr["TABLE_NAME"]);
				var triggerBody = CheckString(dr["TRIGGER_BODY"]);
				if (!triggerBody.Contains(".nextval")) return;
				var matches = _identityTriggerReg.Matches(triggerBody);
				if (matches.Count == 0) return;
				identityTriggers.Add(tableName, matches[0].Value);

			}, triggerSql);

			foreach (var tableColumn in tableColumns)
			{
				foreach (var column in tableColumn.Columns)
				{
					if (!column.IsIdentity)
						column.IsIdentity = column.DefaultValue?.TrimEnd().EndsWith("\".nextval") ?? false;

					if (!column.IsIdentity)
						column.IsIdentity = identityTriggers.TryGetValue(tableColumn.Table.Name, out string c) && c == column.Name;

					if (column.IsIdentity)
						column.CsharpTypeName = "long";
					else
						column.CsharpTypeName = Types.ConvertOracleDataTypeToCSharpType(column.DataType, column.Length, column.Precision, column.Scale);

					if (column.IsNullable && !_notAddQues.Contains(column.CsharpTypeName))
						column.CsharpTypeName += "?";

					if (Types.OnlySelectDataTypes.Contains(column.DataType.ToLower()))
					{
						GenerateRules.FieldIgnore.Insert = SetFileIgnore(GenerateRules.FieldIgnore.Insert, $"{tableColumn.Table.Name}.{column.Name}");
						GenerateRules.FieldIgnore.Update = SetFileIgnore(GenerateRules.FieldIgnore.Update, $"{tableColumn.Table.Name}.{column.Name}");
					}

					if (Types.NotSupportDataTypes.Contains(column.DataType.ToLower()))
					{
						GenerateRules.FieldIgnore.Returning = SetFileIgnore(GenerateRules.FieldIgnore.Returning, $"{tableColumn.Table.Name}.{column.Name}");
					}
				}
			}
			return tableColumns;
		}

		private void GetLongResult(Action<OracleDataReader> action, string sql)
		{
			using OracleConnection connection = (OracleConnection)GenerateConnection.Connection.GetConnection();
			var cmd = connection.CreateCommand();
			cmd.InitialLONGFetchSize = -1;
			cmd.CommandText = sql;
			var reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				action.Invoke(reader);
			}
		}

		public override string GetAttributeTableName(TableViewInfo tableView) => $"\\\"{tableView.Name}\\\"";

		public override ICreeperGeneratorProvider CreateInstance(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection) => new OracleGeneratorProvider(generateRules, options, connection);
	}
}
