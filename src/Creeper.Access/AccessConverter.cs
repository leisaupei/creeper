using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Creeper.Access
{
	internal class AccessConverter : CreeperConverter
	{

		public override DataBaseKind DataBaseKind { get; } = DataBaseKind.Access;

		protected override string IdentityKeyDefault { get; } = null;

		protected override string WithQuote(string field) => string.Concat('[', field, ']');

		protected override string CastStringDataType(string field) => string.Concat("CStr(", field, ")");

		protected override string StringConnectWord { get; } = "&";

		protected override string CallCoalesce(string field, string parameterName, List<DbParameter> ps)
		{
			var signField = field;
			if (ps != null)
			{
				var matches = _parameterRegex.Matches(field);

				if (matches.Count == 1)
				{
					var matchParameterName = matches[0].Value[1..];
					var tempParameterName = matchParameterName + "1";
					var p = ps.FirstOrDefault(a => a.ParameterName == matchParameterName);
					if (p != null)
					{
						ps.Add(GetDbParameter(tempParameterName, p.Value));
						signField = signField.Replace(matchParameterName, tempParameterName);
					}
				}
			}
			return string.Concat("IIf(", field, " Is Null,", parameterName, ",", signField, ")");
		}

		protected override DbParameter GetDbParameter(string name, object value) => new OleDbParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString) => new OleDbConnection(connectionString);

		protected override string GetSelectSql(string columns, string table, string alias, IEnumerable<string> wheres, string groupBy, string having, string orderBy, int? limit, int? offset, string union, string except, string intersect, string join, string afterTableStr)
		{
			var isSkip = (offset ?? 0) > 0;
			StringBuilder sqlText;
			var top = limit.HasValue ? $" TOP {limit} " : null;

			if (!isSkip)
			{
				if (limit > 0) columns = string.Concat(top, columns);
				sqlText = new StringBuilder($"SELECT {columns} FROM {table} AS {alias}").AppendLine();
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
				return sqlText.ToString().TrimEnd();
			}


			if (string.IsNullOrEmpty(orderBy))
				throw new CreeperException("Access分页必须传入排序字段");
			if (offset > 0 && (limit ?? 0) <= 0)
				throw new CreeperException("Access分页必须传入页码, 不允许单独跳过前n条数据");

			sqlText = new StringBuilder($"SELECT * FROM (SELECT {top} * FROM (SELECT TOP {limit ?? 0 + offset ?? 0} {columns} FROM {table} AS {alias}").AppendLine();
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

			sqlText.Append("ORDER BY ").AppendLine(orderBy);

			var orderByMatches = _orderByRegex.Matches(orderBy);
			var orderByList = new List<string>();
			for (int i = 0; i < orderByMatches.Count; i++)
			{
				var str = orderByMatches[i].Value.TrimStart(',').Trim();
				if (str.Count(a => a == ',') > 2)
					throw new ArgumentException("Access排序字段不能忽略ASC/DESC后缀");
				if (str.Contains(" asc", StringComparison.OrdinalIgnoreCase))
					str = str.Replace(" asc", " DESC", StringComparison.OrdinalIgnoreCase);
				else if (str.Contains(" desc", StringComparison.OrdinalIgnoreCase))
					str = str.Replace(" desc", " ASC", StringComparison.OrdinalIgnoreCase);
				orderByList.Add(str);
			}

			//var orderByMatches = _orderByRegex.Matches(orderBy);
			//var orderByList = new List<string>();
			//for (int i = 0; i < orderByMatches.Count; i++)
			//{

			//}
			//var orderBys = orderBy.Split(',');
			//for (int i = 0; i < orderBys.Length; i++)
			//{
			//	if (orderBys[i].Contains(" asc", StringComparison.OrdinalIgnoreCase))
			//		orderBys[i] = orderBys[i].Replace(" asc", " DESC", StringComparison.OrdinalIgnoreCase);
			//	else if (orderBys[i].Contains(" desc", StringComparison.OrdinalIgnoreCase))
			//		orderBys[i] = orderBys[i].Replace(" desc", " ASC", StringComparison.OrdinalIgnoreCase);
			//	else throw new ArgumentException("Access排序字段不能忽略ASC/DESC后缀");
			//}
			sqlText.Append(") ").Append("ORDER BY ").AppendJoin(',', orderByList).AppendLine()
				.Append(") ").Append("ORDER BY ").AppendLine(orderBy);

			return sqlText.ToString().TrimEnd();

		}
		private static readonly Regex _orderByRegex = new Regex(@"(.*?\s+)(asc|desc)\s*", RegexOptions.IgnoreCase);
		protected override bool TrySetSpecialDbParameter(out string format, ref object value)
		{
			//Access时间类型需要精确到秒
			if (value is DateTime dt && dt.Millisecond != 0)
				value = new DateTime(dt.Ticks - (dt.Ticks % TimeSpan.TicksPerSecond), dt.Kind);
			return base.TrySetSpecialDbParameter(out format, ref value);
		}

		protected override string GetUpdateSql<TModel>(string table, string alias, List<string> setList, List<string> whereList, bool returning)
		{
			//var output = returning ? $" output {GetRetColumns<TModel>("inserted")}" : null;
			//var sql = $"UPDATE {alias} SET {string.Join(",", setList)}{output} FROM {table} {alias} WHERE {string.Join(" AND ", whereList)}";
			//return sql;
			return base.GetUpdateSql<TModel>(table, alias, setList, whereList, returning);
		}

		protected override string GetInsertSql<TModel>(string table, IDictionary<string, string> inserts, List<string> wheres, bool returning)
		{
			if (wheres.Count > 0)
				throw new NotSupportedException(DataBaseKind.ToString() + "数据库Insert语句暂不支持Where条件");
			return base.GetInsertSql<TModel>(table, inserts, wheres, returning);
		}

		private static readonly Regex _parameterRegex = new Regex(@"@p[0-9]{6,7}");

		protected override string ExplainOrderBy(string field, bool asc, bool isNullsLast)
		{
			if (asc) return isNullsLast ? $"IIf({field} IS NULL, 1, 0) ASC, {field} ASC" : $"{field} ASC";
			else return isNullsLast ? $"{field} DESC" : $"IIf({field} IS NULL, 0, 1) ASC, {field} DESC";
		}

		protected override void OrderDbParameters(string sql, ref DbParameter[] ps)
		{
			if (ps.Length < 2) return;
			var temp = new DbParameter[ps.Length];
			var matches = _parameterRegex.Matches(sql);
			if (matches.Count != ps.Length) throw new ArgumentException("参数化数量不匹配");
			for (int i = 0; i < matches.Count; i++)
				foreach (var p in ps)
					if (matches[i].Value == GetSqlDbParameterName(p.ParameterName))
						temp[i] = p;
			ps = temp;
		}
	}

}
