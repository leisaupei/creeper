using Creeper.Driver;
using Creeper.Generic;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Creeper.Oracle
{
	internal class OracleConverter : CreeperConverter
	{
		public override DataBaseKind DataBaseKind { get; } = DataBaseKind.Oracle;

		internal ColumnNameStyle ColumnNameStyle { get; set; } = ColumnNameStyle.None;

		protected override bool IsColumnNameCaseInsensitive { get; } = false; //字段名是否大小写不敏感

		protected override string DbParameterPrefix { get; } = ":"; //参数化名称前缀

		protected override string IdentityKeyDefault { get; } = null; //自增键插入时不能用显式赋值

		protected override ushort MaximumPrecision { get; } = 25; //返回值浮点型最大精度

		protected override string CastStringDataType(string field) => string.Concat("CAST(", field, " AS VARCHAR2(2000)", ")");

		protected override string ExceptKeyName { get; } = "MINUS"; // Oracle EXCEPT语法关键字

		protected override void PrepareDbCommand(DbCommand cmd)
		{
			var oracleCommand = (OracleCommand)cmd;
			oracleCommand.BindByName = true; // 按照参数化顺序改为按照参数化名称
			base.PrepareDbCommand(cmd);
		}

		protected override DbParameter GetDbParameter(string name, object value) => new OracleParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString) => new OracleConnection(connectionString);

		protected override void Initialization(string connectionString)
		{
			SetServerVersion(connectionString);
			base.Initialization(connectionString);
		}

		protected override bool TrySetSpecialDbParameter(out string format, ref object value)
		{
			if (value is bool b)
				value = Convert.ToInt16(b);
			if (value is Guid g)
				value = g.ToString();
			return base.TrySetSpecialDbParameter(out format, ref value);
		}

		protected override string GetInsertSql<TModel>(string table, IDictionary<string, string> inserts, List<string> wheres, bool returning)
		{
			if (returning) throw new CreeperNotSupportedException(DataBaseKind.ToString() + "数据库Insert语句暂不支持RETURNING");
			var sql = $"INSERT INTO {table} ({string.Join(", ", inserts.Keys)})";
			if (wheres.Count == 0)
				sql += $" VALUES({string.Join(", ", inserts.Values)})";
			else
				sql += $" SELECT {string.Join(", ", inserts.Values)} FROM DUAL WHERE {string.Join(" AND ", wheres)}";
			return sql;
		}

		protected override string GetInsertRangeSql<TModel>(string table, IDictionary<string, string>[] inserts, bool returning)
		{
			if (returning) throw new CreeperNotSupportedException(DataBaseKind.ToString() + "数据库InsertRange语句暂不支持RETURNING");
			var sb = new StringBuilder("INSERT ALL").AppendLine();
			foreach (var item in inserts)
			{
				sb.Append("INTO ").Append(table).Append("(").AppendJoin(',', item.Keys)
					.Append(") VALUES(").AppendJoin(',', item.Values).AppendLine(")");
			}
			sb.Append("SELECT 1 FROM DUAL");
			return sb.ToString();
		}

		protected override string GetUpdateSql<TModel>(string table, string alias, List<string> setList, List<string> whereList, bool returning)
		{
			if (returning) throw new CreeperNotSupportedException(DataBaseKind.ToString() + "数据库Update语句暂不支持RETURNING");
			return $"UPDATE {table} {alias} SET {string.Join(",", setList)} WHERE {string.Join(" AND ", whereList)}";
		}

		protected override string GetDeleteSql(string table, string alias, List<string> wheres)
		{
			return $"DELETE FROM {table} {alias} WHERE {string.Join(" AND ", wheres)}";
		}

		protected override string GetSelectSql(string columns, string table, string alias, IEnumerable<string> wheres, string groupBy, string having, string orderBy, int? limit, int? offset, string union, string except, string intersect, string join, string afterTableStr)
		{
			var sqlText = new StringBuilder($"SELECT {columns} FROM {table} {alias}").AppendLine();
			if (!string.IsNullOrWhiteSpace(afterTableStr)) sqlText.AppendLine(afterTableStr);
			if (!string.IsNullOrEmpty(join)) sqlText.AppendLine(join);
			if (wheres?.Count() > 0) sqlText.Append("WHERE ").AppendJoin(" AND ", wheres).AppendLine();
			if (!string.IsNullOrEmpty(union)) sqlText.AppendLine(union);
			if (!string.IsNullOrEmpty(except)) sqlText.AppendLine(except);
			if (!string.IsNullOrEmpty(intersect)) sqlText.AppendLine(intersect);
			if (!string.IsNullOrEmpty(groupBy))
			{
				sqlText.Append("GROUP BY ").AppendLine(groupBy);
				if (!string.IsNullOrEmpty(having)) sqlText.Append("HAVING ").AppendLine(having);
			}
			if (!string.IsNullOrEmpty(orderBy)) sqlText.Append("ORDER BY ").AppendLine(orderBy);
			if (offset.HasValue) sqlText.Append("OFFSET ").Append(offset.Value).Append(" ROWS").AppendLine();
			if (limit.HasValue) sqlText.Append("FETCH NEXT ").Append(limit.Value).Append(" ROWS ONLY").AppendLine();
			return sqlText.ToString().TrimEnd();
		}
		protected override string GetUpsertSql<TModel>(string table, IDictionary<string, string> upsertSets, bool returning)
		{
			/*
			 MERGE INTO "UniPkTest" USING DUAL ON ( "Id"='13e19e83997a1001')
	WHEN MATCHED THEN UPDATE SET "Name"='Faker'
	WHEN NOT MATCHED THEN INSERT ("Id","Name") 
	VALUES ( '13e19e83997a1001','Faker')
			 */
			var keys = upsertSets.Keys;
			var quoPks = GetPks<TModel>(withQuote: true);
			var quoIdenKeys = GetIdenKeysWithQuote<TModel>();
			var insertKeys = keys.Except(quoIdenKeys); // 排除自增键, 默认情况oracle自增键只能隐式赋值
			var sb = new StringBuilder("MERGE INTO ").Append(table).Append(" USING DUAL ON (").AppendJoin(" AND ", quoPks.Select(a => $"{a}={upsertSets[a] ?? "0"}")).AppendLine(")")
				.Append("WHEN MATCHED THEN UPDATE SET ").AppendJoin(",", keys.Except(quoPks).Select(a => $"{a}={upsertSets[a]}")).AppendLine()
				.Append("WHEN NOT MATCHED THEN INSERT (").AppendJoin(",", insertKeys).Append(")VALUES(").AppendJoin(",", insertKeys.Select(a => upsertSets[a])).Append(")");
			return sb.ToString();
		}

		protected override string WithQuote(string field)
		{
			switch (ColumnNameStyle)
			{
				case ColumnNameStyle.ToLower:
					field = field.ToLower();
					break;
				case ColumnNameStyle.Camel:
					field = $"{field[0..1].ToLower()}{field[1..]}";
					break;
			}
			return string.Concat('"', field, '"');
		}

		protected override string ExplainOrderBy(string field, bool asc, bool isNullsLast)
		{
			if (asc) return isNullsLast ? $"{field} ASC" : $"{field} ASC NULLS FIRST";
			else return isNullsLast ? $"{field} DESC NULLS LAST" : $"{field} DESC";
		}
	}
}
