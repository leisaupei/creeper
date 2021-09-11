using Creeper.Driver;
using Creeper.Generic;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Creeper.SqlServer
{
	internal class SqlServerConverter : CreeperConverter
	{
		/// <summary>
		/// 替换字段别名的正则表达式
		/// </summary>
		private static readonly Regex _aliasRegex = new Regex(@"[a-zA-Z]+\s*\.");

		public override DataBaseKind DataBaseKind { get; } = DataBaseKind.SqlServer;

		protected override string StringConnectWord { get; } = "+";

		protected override string WithQuote(string field) => string.Concat('[', field, ']');

		protected override string IdentityKeyDefault { get; } = null;

		/// <summary>
		/// codename	RTM
		/// 2019	 	15.0.2000.5
		/// 2017	 	14.0.1000.169
		/// 2016	 	13.00.1601.5
		/// 2014	 	12.00.2000.8
		/// 2012		11.00.2100.60
		/// 2008 R2 	10.50.1600,10.50.1600.1
		/// 2008		10.00.1600.22
		/// 2005		9.00.1399.06
		/// 2000		8.00.194
		/// </summary>
		private bool IsUseFetchRows => ServerVersion.Major > 10; //sqlserver 2012+

		protected override DbParameter GetDbParameter(string name, object value) => new SqlParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString) => new SqlConnection(connectionString);

		protected override object ConvertDbData(object value, Type convertType)
		{
			return base.ConvertDbData(value, convertType);
		}

		protected override void Initialization(string connectionString)
		{
			SetServerVersion(connectionString);
			base.Initialization(connectionString);
		}

		protected override string GetSelectSql(string columns, string table, string alias, IEnumerable<string> wheres, string groupBy, string having, string orderBy, int? limit, int? offset, string union, string except, string intersect, string join, string afterTableStr)
		{
			return IsUseFetchRows
				? SelectRowsFetchNext(columns, table, alias, wheres, groupBy, having, orderBy, limit, offset, union, except, intersect, join, afterTableStr)
				: SelectRowNumber(columns, table, alias, wheres, groupBy, having, orderBy, limit, offset, union, except, intersect, join, afterTableStr);
		}

		/// <summary>
		/// sqlserver版本2012+ 的分页查询
		/// </summary>
		/// <returns></returns>
		private static string SelectRowsFetchNext(string columns, string table, string alias, IEnumerable<string> wheres, string groupBy, string having, string orderBy, int? limit, int? offset, string union, string except, string intersect, string join, string afterTableStr)
		{
			/*
			有offset
				select * from table order by id asc offset ((@pageIndex-1)*@pageSize) rows fetch next @pageSize rows only;
			无offset
				select top (@pageSize) * from table order by id asc
			*/

			var isSkip = (offset ?? 0) > 0;
			if (!isSkip && limit.HasValue) columns = string.Concat($"TOP ({limit}) ", columns);
			var sqlText = new StringBuilder($"SELECT {columns} FROM {table} AS {alias}").AppendLine();
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
			if (isSkip && string.IsNullOrEmpty(orderBy)) orderBy = "GetDate()";
			if (!string.IsNullOrEmpty(orderBy)) sqlText.Append("ORDER BY ").AppendLine(orderBy);
			if (isSkip) sqlText.Append("OFFSET ").Append(offset.Value).Append(" ROWS").AppendLine();
			if (isSkip && limit.HasValue) sqlText.Append("FETCH NEXT ").Append(limit.Value).Append(" ROWS ONLY").AppendLine();

			return sqlText.ToString().TrimEnd();
		}

		/// <summary>
		/// sqlserver版本小于2012 的分页查询
		/// </summary>
		/// <returns></returns>
		private static string SelectRowNumber(string columns, string table, string alias, IEnumerable<string> wheres, string groupBy, string having, string orderBy, int? limit, int? offset, string union, string except, string intersect, string join, string afterTableStr)
		{
			/*
			第一页: 
				SELECT TOP (10) * FROM [dbo].[Table] a
			非第一页:
				SELECT TOP (10) * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY [DateTime] ASC) AS [_RowIndex] FROM [dbo].[Table] a) T 
				WHERE [_RowIndex] > 10 ORDER BY [_RowIndex] ASC
			 */
			var isSkip = (offset ?? 0) > 0;
			var top = limit.HasValue ? $" TOP ({limit})" : null;
			var innerColumn = columns;

			if (isSkip) innerColumn = string.Concat(columns, $", ROW_NUMBER() OVER(ORDER BY {(string.IsNullOrEmpty(orderBy) ? "GetDate()" : orderBy)}) AS [_RowIndex]");
			else if (limit > 0) innerColumn = string.Concat(top, columns);
			var sqlText = new StringBuilder($"SELECT {innerColumn} FROM {table} AS {alias}").AppendLine();
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
			if (isSkip)
			{
				columns = _aliasRegex.Replace(columns, "");
				sqlText.Insert(0, $"SELECT{top} {columns} FROM (" + Environment.NewLine);
				sqlText.Append($") _RowTable WHERE [_RowIndex] > {offset} ORDER BY [_RowIndex] ASC");
			}
			else if (!string.IsNullOrEmpty(orderBy)) sqlText.Append("ORDER BY ").AppendLine(orderBy);

			return sqlText.ToString().TrimEnd();
		}

		protected override string GetUpdateSql<TModel>(string table, string alias, List<string> setList, List<string> whereList, bool returning)
		{
			var output = returning ? $" output {GetRetColumns<TModel>("inserted")}" : null;
			var sql = $"UPDATE {alias} SET {string.Join(",", setList)}{output} FROM {table} {alias} WHERE {string.Join(" AND ", whereList)}";
			return sql;
		}

		protected override string GetInsertSql<TModel>(string table, IDictionary<string, string> inserts, List<string> wheres, bool returning)
		{
			if (wheres.Count > 0)
			{
				var identityPk = GetIdenKeysWithQuote<TModel>();
				foreach (var item in identityPk) inserts.Remove(item);
			}

			var output = returning ? $" output {GetRetColumns<TModel>("inserted")}" : null;
			var sql = $"INSERT INTO {table} ({string.Join(",", inserts.Keys)}){output}";
			if (wheres.Count == 0)
				sql += $" VALUES({string.Join(",", inserts.Values)})";
			else
				sql += $" SELECT {string.Join(",", inserts.Values)} WHERE {string.Join(" AND ", wheres)}";

			return sql;
		}

		/// <summary>
		/// 生成增补sql语句, 使用MERGE语法, 此处非主键唯一键(UniqueKey)不会并列为匹配条件, 所以包含唯一键的表需要保证数据库不包含此行. 
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="table"></param>
		/// <param name="upsertSets"></param>
		/// <param name="returning"></param>
		/// <returns></returns>
		protected override string GetUpsertSql<TModel>(string table, IDictionary<string, string> upsertSets, bool returning)
		{
			/*
MERGE INTO [Student] as T
USING (SELECT '99' AS [Id], '小红' AS [Name], 1628909070000 AS [CreateTime], 2021900 AS [StuNo]) AS S
ON T.[Id] = S.[Id]
WHEN MATCHED THEN
	UPDATE SET T.[Name] = S.[Name], T.CreateTime = S.[CreateTime], T.[StuNo] = S.[StuNo] 
WHEN NOT MATCHED THEN
	INSERT([Name],[CreateTime],[StuNo]) VALUES(S.[Name], S.[CreateTime], S.[StuNo])
output inserted.[id],inserted.[name],inserted.[createtime],inserted.[stuno];
			 * */
			var keys = upsertSets.Keys;
			var quoPks = GetPks<TModel>(withQuote: true);
			var quoIdenKeys = GetIdenKeysWithQuote<TModel>();
			var output = returning ? $"output {GetRetColumns<TModel>("inserted")}" : null;

			var insertKeys = keys.Except(quoIdenKeys); // 排除自增键, 默认情况sqlserver自增键只能隐式赋值
			var sql = $@"MERGE INTO {table} AS T
USING (SELECT {string.Join(",", upsertSets.Select(a => $"{a.Value ?? "0"} AS {a.Key}"))}) AS S
ON {string.Join(" AND ", quoPks.Select(a => $"T.{a} = S.{a}"))}
WHEN MATCHED THEN UPDATE SET {string.Join(",", keys.Except(quoPks).Select(a => $"T.{a} = S.{a}"))}
WHEN NOT MATCHED THEN INSERT({string.Join(",", insertKeys)}) VALUES({string.Join(",", insertKeys.Select(a => $"S.{a}"))})
{output};
";
			return sql;
		}

		protected override string GetDeleteSql(string table, string alias, List<string> wheres)
		{
			return $"DELETE {alias} FROM {table} AS {alias} WHERE {string.Join(" AND ", wheres)}";
		}

		protected override string ExplainOrderBy(string field, bool asc, bool isNullsLast)
		{
			if (asc) return isNullsLast ? $"CASE WHEN {field} IS NULL THEN 1 ELSE 0 END, {field} ASC" : $"{field} ASC";
			else return isNullsLast ? $"{field} DESC" : $"CASE WHEN {field} IS NULL THEN 0 ELSE 1 END, {field} DESC";
		}

		protected override string ExplainLike(bool isIngoreCase, bool isNot)
		{
			return string.Concat("{0}", isNot ? "NOT" : null, " LIKE {1}", isIngoreCase ? "" : " COLLATE CHINESE_PRC_CS_AI");
		}
	}
}
