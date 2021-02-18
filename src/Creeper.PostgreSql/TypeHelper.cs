using System.Data;
using Creeper.DBHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.PostgreSql
{
    internal class TypeHelper
    {
        private static readonly Regex _paramPattern = new Regex(@"(^(\-|\+)?\d+(\.\d+)?$)|(^SELECT\s.+\sFROM\s)|(true)|(false)", RegexOptions.IgnoreCase);
        public static string SqlToString(string sql, List<DbParameter> nps)
        {
            foreach (var p in nps)
            {
                var value = GetParamValue(p.Value);
                var key = string.Concat("@", p.ParameterName);
                if (value == null)
                    sql = SqlHelper.GetNullSql(sql, key);

                else if (_paramPattern.IsMatch(value) && p.DbType == DbType.String)
                    sql = sql.Replace(key, value);

                else if (value.Contains("array"))
                    sql = sql.Replace(key, value);

                else
                    sql = sql.Replace(key, $"'{value}'");
            }
            return sql.Replace("\r", " ").Replace("\n", " ");
        }
        public static string SqlToJsonString(string sql, List<DbParameter> nps)
        {
            var jobj = new Hashtable();
            jobj["cmdText"] = sql;
            var ht = new Hashtable();
            foreach (var p in nps)
                ht[p.ParameterName] = p.Value;

            jobj["parameters"] = ht;
            return JsonConvert.SerializeObject(jobj);
        }
        public static string GetParamValue(object value)
        {
            Type type = value.GetType();
            if (type.IsArray)
            {
                var arrStr = (value as object[]).Select(a => $"'{a?.ToString() ?? ""}'");
                return $"array[{string.Join(",", arrStr)}]";
            }
            return value?.ToString();
        }
    }
}
