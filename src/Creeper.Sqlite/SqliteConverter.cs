using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Creeper.Sqlite
{

	internal class SqliteConverter : CreeperConverter
	{
		public override DataBaseKind DataBaseKind { get; } = DataBaseKind.Sqlite;

		protected override string IdentityKeyDefault { get; } = "NULL";

		protected override DbParameter GetDbParameter(string name, object value)
			=> new SQLiteParameter(name, value);
		/// <summary>
		/// 此版本后支持postgresql Returning语法
		/// </summary>
		private readonly static Version _returningVersion = Version.Parse("3.35.0");

		/// <summary>
		/// 此版本后支持postgresql upsert语法
		/// </summary>
		private readonly static Version _upsertVersion = Version.Parse("3.24.0");

		private bool IsUseUpsert => ServerVersion > _upsertVersion;

		private bool IsUseReturning => ServerVersion > _returningVersion;

		protected override void Initialization(string connectionString)
		{
			SetServerVersion(connectionString);
			base.Initialization(connectionString);
		}

		protected override object ConvertDbData(object value, Type convertType)
		{
			return convertType switch
			{
				var t when t == typeof(TimeSpan) => ((DateTime)value).TimeOfDay,
				var t when t == typeof(Guid) => Guid.Parse(value.ToString()),
				_ => base.ConvertDbData(value, convertType),
			};

		}
		protected override bool TrySetSpecialDbParameter(out string format, ref object value)
		{
			if (value is Guid g)
				value = g.ToString();
			return base.TrySetSpecialDbParameter(out format, ref value);
		}

		private string GetReturning<TModel>(bool returning) where TModel : ICreeperModel
		{
			if (returning)
				throw new CreeperNotSupportedException(DataBaseKind.ToString() + $"数据库Upsert/Update暂不支持RETURNING(Insert支持), 使用{_returningVersion}+版本的RETURNING受影响行数为-1故不支持, 若已更新至不为-1版本或有其他方式可判断, 可联系作者提供建议");
			return returning ? $"RETURNING {GetRetColumns<TModel>().Replace("\"", "")}" : null;
		}

		public override DbConnection GetDbConnection(string connectionString)
			=> new SQLiteConnection(connectionString);

		protected override string GetSelectSql(string columns, string table, string alias, IEnumerable<string> wheres, string groupBy, string having, string orderBy, int? limit, int? offset, string union, string except, string intersect, string join, string afterTableStr)
		{
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
			if (!string.IsNullOrEmpty(orderBy)) sqlText.Append("ORDER BY ").AppendLine(orderBy);

			if (!limit.HasValue && offset.HasValue)
			{
				throw new CreeperException("SQLite分页必须传入页码, 不允许单独跳过前n条数据(OFFSET必须在LIMIT后面使用)");
			}

			if (limit.HasValue)
			{
				sqlText.Append("LIMIT ").Append(limit.Value).AppendLine();
				if (offset.HasValue) sqlText.Append("OFFSET ").Append(offset.Value).AppendLine();
			}
			return sqlText.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="table"></param>
		/// <param name="inserts"></param>
		/// <param name="wheres"></param>
		/// <param name="returning"></param>
		/// <returns></returns>
		protected override string GetInsertSql<TModel>(string table, IDictionary<string, string> inserts, List<string> wheres, bool returning)
		{
			var sql = $"INSERT INTO {table} ({string.Join(", ", inserts.Keys)})";
			if (wheres.Count == 0)
				sql += $" VALUES({string.Join(", ", inserts.Values)})";
			else
				sql += $" SELECT {string.Join(", ", inserts.Values)} WHERE {string.Join(" AND ", wheres)}";

			if (!returning) return sql;

			var columns = GetRetColumns<TModel>();
			var idpks = GetIdenKeysWithQuote<TModel>();
			if (idpks.Length == 1)
				sql += $"; SELECT {columns} FROM {table} WHERE {idpks[0]} = last_insert_rowid() OR {idpks[0]} = {inserts[idpks[0]]}";
			else
				sql += $"; SELECT {columns} FROM {table} WHERE {string.Join(" AND ", GetPks<TModel>(withQuote: true).Select(a => $"{a} = {inserts[a]}"))}";

			return sql;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// 3.24.0+版本是用的是ON CONFLICT语法, 非主键唯一键(UniqueKey)不会并列为匹配条件, 所以包含唯一键的表需要保证数据库不包含此行.<br/>
		/// 3.24.0-版本是用的是INSERT OR REPLACE语法, 非主键唯一键(UniqueKey)会成为匹配条件, 会存在唯一列冲突情况, 建议只留一个.
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="table"></param>
		/// <param name="upsertSets"></param>
		/// <param name="returning"></param>
		/// <returns></returns>
		protected override string GetUpsertSql<TModel>(string table, IDictionary<string, string> upsertSets, bool returning)
		{
			if (IsUseUpsert)
			{
				var quoPks = GetPks<TModel>(withQuote: true);
				return @$"INSERT INTO {table} ({string.Join(", ", upsertSets.Keys)}) VALUES({string.Join(", ", upsertSets.Values)})
ON CONFLICT({string.Join(", ", quoPks)}) DO UPDATE
SET {string.Join(", ", upsertSets.Keys.Except(quoPks).Select(a => $"{a} = EXCLUDED.{a}"))}
{GetReturning<TModel>(returning)}";
			}
			else
			{
				return $"INSERT OR REPLACE INTO {table} ({string.Join(", ", upsertSets.Keys)}) VALUES ({string.Join(", ", upsertSets.Values)})"
				+ GetReturning<TModel>(returning);
			}

		}

		protected override string GetUpdateSql<TModel>(string table, string alias, List<string> setList, List<string> whereList, bool returning)
		{
			return $"UPDATE {table} AS {alias} SET {string.Join(",", setList)} WHERE {string.Join(" AND ", whereList)} {GetReturning<TModel>(returning)}";
		}
		protected override string ExplainOrderBy(string field, bool asc, bool isNullsLast)
		{
			if (asc) return isNullsLast ? $"CASE WHEN {field} IS NULL THEN 1 ELSE 0 END, {field} ASC" : $"{field} ASC";
			else return isNullsLast ? $"{field} DESC" : $"CASE WHEN {field} IS NULL THEN 0 ELSE 1 END, {field} DESC";
		}
	}

}
