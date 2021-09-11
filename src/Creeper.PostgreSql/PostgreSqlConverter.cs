using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql.Extensions;
using Creeper.SqlBuilder;
using Newtonsoft.Json.Linq;
using Npgsql;
using Npgsql.LegacyPostgis;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Creeper.PostgreSql
{
	internal class PostgreSqlConverter : CreeperConverter
	{
		public override DataBaseKind DataBaseKind { get; } = DataBaseKind.PostgreSql;

		/// <summary>
		/// 使用DEFAULT关键字可使使用默认规则
		/// </summary>
		protected override string IdentityKeyDefault { get; } = "DEFAULT";

		protected override object ConvertDbData(object value, Type convertType)
		{
			return convertType switch
			{
				var t when t == typeof(PostgisGeometry) => (PostgisGeometry)value,
				// jsonb json 类型
				var t when PostgreSqlTypeMappingExtensions.JTypes.Contains(t) => JToken.Parse(value?.ToString() ?? "{}"),
				var t when t == typeof(NpgsqlTsQuery) => NpgsqlTsQuery.Parse(value.ToString()),
				var t when t == typeof(BitArray) && value is bool b => new BitArray(1, b),
				_ => base.ConvertDbData(value, convertType),
			};
		}

		protected override string ConvertSqlToString(ISqlBuilder sqlBuilder)
		{
			var sql = sqlBuilder.CommandText;

			foreach (var p in sqlBuilder.Params)
			{
				var value = GetParamValue(p.Value);
				var key = string.Concat("@", p.ParameterName);
				if (value == null)
					sql = base.GetNullSql(sql, key);

				else if (ParamPattern.IsMatch(value) && p.DbType == DbType.String)
					sql = sql.Replace(key, value);

				else if (value.Contains("array"))
					sql = sql.Replace(key, value);

				else
					sql = sql.Replace(key, $"'{value}'");
			}
			return sql.Replace("\r", " ").Replace("\n", " ");
		}

		public static string GetParamValue(object value)
		{
			Type type = value.GetType();
			if (type.IsArray || typeof(IList).IsAssignableFrom(type))
			{
				var values = (IList)value;
				var stringArray = new object[values.Count];
				for (int i = 0; i < values.Count; i++) stringArray[i] = $"'{values[i]?.ToString() ?? ""}'";
				return $"array[{string.Join(",", stringArray)}]";
			}
			return value?.ToString();
		}

		public override DbConnection GetDbConnection(string connectionString)
			=> new NpgsqlConnection(connectionString);

		protected override DbParameter GetDbParameter(string name, object value)
			=> new NpgsqlParameter(name, value);

		protected override string GetInsertRangeSql<TModel>(string table, IDictionary<string, string>[] inserts, bool returning)
		{
			var sql = $"INSERT INTO {table} ({string.Join(",", inserts[0].Keys)})"
				+ $" VALUES" + string.Join(",", inserts.Select(i => $"({string.Join(",", i.Values)})"))
				+ GetReturning<TModel>(returning);
			return sql;
		}

		private string GetReturning<TModel>(bool returning) where TModel : ICreeperModel
		{
			return returning ? $"RETURNING {GetRetColumns<TModel>()}" : null;
		}

		/// <summary>
		/// 获取增补sql语句, 此处非主键唯一键(UniqueKey)不会并列为匹配条件, 所以包含唯一键的表需要保证数据库不包含此行. 
		/// </summary>
		/// <param name="table">表</param>
		/// <param name="upsertSets">需要设置的值, 此处key包含字段标识符</param>
		/// <param name="returning">是否返回受影响结果</param>
		/// <returns></returns>
		protected override string GetUpsertSql<TModel>(string table, IDictionary<string, string> upsertSets, bool returning)
		{
			var quoPks = GetPks<TModel>(withQuote: true);
			return @$"INSERT INTO {table} ({string.Join(",", upsertSets.Keys)}) VALUES({string.Join(",", upsertSets.Values)})
ON CONFLICT({string.Join(",", quoPks)}) DO UPDATE
SET {string.Join(",", upsertSets.Keys.Except(quoPks).Select(a => $"{a} = EXCLUDED.{a}"))}
{GetReturning<TModel>(returning)}";
		}

		protected override string GetUpdateSql<TModel>(string table, string alias, List<string> setList, List<string> whereList, bool returning)
		{
			return $"UPDATE {table} AS {alias} SET {string.Join(",", setList)} WHERE {string.Join(" AND ", whereList)} {GetReturning<TModel>(returning)}";
		}

		protected override string GetInsertSql<TModel>(string table, IDictionary<string, string> inserts, List<string> wheres, bool returning)
		{
			if (wheres.Count > 0)
			{
				var identityPk = GetIdenKeysWithQuote<TModel>();
				foreach (var item in identityPk) inserts.Remove(item);
			}

			var sql = $"INSERT INTO {table} ({string.Join(",", inserts.Keys)})";
			if (wheres.Count == 0)
				sql += $" VALUES({string.Join(",", inserts.Values)})";
			else
				sql += $" SELECT {string.Join(",", inserts.Values)} WHERE {string.Join(" AND ", wheres)}";

			return string.Concat(sql, " ", GetReturning<TModel>(returning));
		}

		protected override string ExplainOrderBy(string field, bool asc, bool isNullsLast) => string.Concat(field, " ", asc ? "ASC" : "DESC", isNullsLast ? " NULLS LAST" : "");

		protected override string ExplainLike(bool isIngoreCase, bool isNot) => string.Concat("{0} ", isNot ? "NOT " : null, isIngoreCase ? "ILIKE" : "LIKE", " {1}");
		protected override bool MergeArray { get; } = true;
		protected override string ExplainAny(string field, bool isNot, IEnumerable<string> parameters, bool memberIsArray)
		{
			var oper = isNot ? "<>" : "=";
			var method = isNot ? "ALL" : "ANY";
			if (memberIsArray)
				return string.Concat(parameters.ElementAt(0), oper, method, "(", field, ")");
			return string.Concat(field, oper, method, "(", parameters.ElementAt(0), ")");
		}
	}
}
