using Creeper.Driver;
using Creeper.Generic;
using Creeper.MySql.Types;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Creeper.MySql
{
	internal class MySqlConverter : CreeperConverter
	{
		static readonly Type[] _geometryTypes = new[] {
			typeof(MySqlGeometry),
			typeof(MySqlGeometryCollection),
			typeof(MySqlLineString),
			typeof(MySqlMultiLineString),
			typeof(MySqlMultiPoint),
			typeof(MySqlMultiPolygon),
			typeof(MySqlPoint),
			typeof(MySqlPolygon)
		};

		internal bool UseGeometryType { get; set; }

		public override DataBaseKind DataBaseKind { get; } = DataBaseKind.MySql;

		protected override string CastStringDataType(string field) => string.Concat("CAST(", field, " AS CHAR", ")");

		protected override string WithQuote(string field) => string.Concat('`', field, '`');

		protected override object ConvertDbData(object value, Type convertType)
		{
			if (UseGeometryType && _geometryTypes.Contains(convertType))
				return MySqlGeometry.Parse(value.ToString());

			return base.ConvertDbData(value, convertType);
		}

		protected override DbParameter GetDbParameter(string name, object value)
			=> new MySqlParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		{
			var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString) { AllowUserVariables = true };

			var connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
			return connection;
		}

		protected override bool TrySetSpecialDbParameter(out string format, ref object value)
		{
			if (UseGeometryType && _geometryTypes.Contains(value.GetType()))
			{
				format = "ST_GeomFromText({0})";
				value = value.ToString();
				return true;
			}
			return base.TrySetSpecialDbParameter(out format, ref value);
		}

		protected override bool TryGetSpecialReturnFormat(Type type, out string format)
		{
			if (UseGeometryType && _geometryTypes.Contains(type))
			{
				format = "ST_AsText({0})";
				return true;
			}

			return base.TryGetSpecialReturnFormat(type, out format);
		}

		/// <summary>
		/// 获取增补sql语句, 使用ON DUPLICATE KEY语法, 此处非主键唯一键(UniqueKey)会成为匹配条件, 会存在唯一列冲突情况, 建议只留一个
		/// </summary>
		/// <param name="table">表</param>
		/// <param name="upsertSets">需要设置的值, 此处key包含字段标识符</param>
		/// <param name="returning">是否返回受影响结果</param>
		/// <returns></returns>
		protected override string GetUpsertSql<TModel>(string table, IDictionary<string, string> upsertSets, bool returning)
		{
			var quoPks = GetPks<TModel>(withQuote: true);
			var sql = @$"INSERT INTO {table}({string.Join(", ", upsertSets.Keys)}) VALUES({string.Join(", ", upsertSets.Values)}) 
ON DUPLICATE KEY UPDATE {string.Join(", ", upsertSets.Keys.Except(quoPks).Select(a => $"{a} = {upsertSets[a]}"))}";
			if (returning)
			{
				var columns = GetRetColumns<TModel>();
				var idpks = GetIdenKeysWithQuote<TModel>();
				if (idpks.Length == 1)
					sql += $"; SELECT {columns} FROM {table} WHERE {idpks[0]} = LAST_INSERT_ID() OR {idpks[0]} = {upsertSets[idpks[0]]}";
				else
					sql += $"; SELECT {columns} FROM {table} WHERE {string.Join(" AND ", quoPks.Select(a => $"{a} = {upsertSets[a]}"))}";
			}
			return sql;
		}

		protected override string GetUpdateSql<TModel>(string table, string alias, List<string> setList, List<string> whereList, bool returning)
		{
			string ret1 = null, ret2 = null, ret3 = null;
			if (returning)
			{
				var pks = GetPks<TModel>();
				for (int i = 0; i < pks.Length; i++)
				{
					ret1 += string.Format("@_{0} := ''", pks[i]);
					ret2 += string.Format("{1} = (SELECT @_{0} := {1})", pks[i], WithQuote(pks[i]));
					ret3 += string.Format("{1} = @_{0}", pks[i], WithQuote(pks[i]));
					if (i != pks.Length - 1)
					{
						ret1 += ", ";
						ret2 += ", ";
						ret3 += " AND ";
					}
				}
				ret1 = $"SET {ret1};";
				ret2 = $", {ret2}";
				ret3 = $"; SELECT {GetRetColumns<TModel>()} FROM {table} WHERE {ret3}";
			}
			return $"{ret1}UPDATE {table} AS {alias} SET {string.Join(",", setList)}{ret2} WHERE {string.Join(" AND ", whereList)} {ret3}";
		}

		protected override string GetInsertSql<TModel>(string table, IDictionary<string, string> inserts, List<string> wheres, bool returning)
		{
			var sql = $"INSERT INTO {table} ({string.Join(", ", inserts.Keys)})";
			if (wheres.Count == 0)
				sql += $" VALUES({string.Join(", ", inserts.Values)})";
			else
				sql += $" SELECT {string.Join(", ", inserts.Values)} FROM DUAL WHERE {string.Join(" AND ", wheres)}";

			if (returning)
			{
				var columns = GetRetColumns<TModel>();
				var idpks = GetIdenKeysWithQuote<TModel>();
				if (idpks.Length == 1)
				{
					sql += $"; SELECT {columns} FROM {table} WHERE {idpks[0]} = @@IDENTITY";
				}
				else
				{
					var pks = GetPks<TModel>(withQuote: true);
					var where = string.Join(" AND ", pks.Select(a => $"{a} = {inserts[a]}"));
					sql += $"; SELECT {columns} FROM {table} WHERE {where}";
				}
			}
			return sql;
		}

		protected override string GetDeleteSql(string table, string alias, List<string> wheres)
		{
			return $"DELETE {alias} FROM {table} AS {alias} WHERE {string.Join(" AND ", wheres)}";
		}

		protected override string ExplainLike(bool isIngoreCase, bool isNot)
		{
			return string.Concat("{0}", isNot ? "NOT" : null, " LIKE", isIngoreCase ? " BINARY" : "", " {1}");
		}

		protected override string ExplainOrderBy(string field, bool asc, bool isNullsLast)
		{
			return isNullsLast ? string.Concat(asc ? "-" : null, field, " DESC") : string.Concat(asc ? null : "-", field, " ASC");
		}
	}
}
