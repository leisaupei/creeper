using Creeper.Utils;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.SqlBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Creeper.SqlBuilder.ExpressionAnalysis;

namespace Creeper.Driver
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class CreeperConverter
	{
		protected static readonly Regex ParamPattern = new Regex(@"(^(\-|\+)?\d+(\.\d+)?$)|(^SELECT\s.+\sFROM\s)|(true)|(false)", RegexOptions.IgnoreCase);

		protected CreeperConverter()
		{
			EntityUtils = new EntityUtils(this);
		}
		/// <summary>
		/// 处理表与字段的实体
		/// </summary>
		internal EntityUtils EntityUtils { get; }

		/// <summary>
		/// 数据库版本号
		/// </summary>
		protected internal Version ServerVersion { get; private set; }

		/// <summary>
		/// 数据库种类
		/// </summary>
		public abstract DataBaseKind DataBaseKind { get; }

		/// <summary>
		/// 获取dbparameter
		/// </summary>
		/// <param name="name">键</param>
		/// <param name="value">值</param>
		/// <returns></returns>
		protected internal abstract DbParameter GetDbParameter(string name, object value);

		/// <summary>
		/// 获取dbconnection
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		public abstract DbConnection GetDbConnection(string connectionString);

		/// <summary>
		/// 数据库列名是否大小写不敏感
		/// </summary>
		protected internal virtual bool IsColumnNameCaseInsensitive { get; } = true;

		/// <summary>
		/// 数据库强转字符串类型
		/// </summary>
		protected internal virtual string CastStringDataType(string field) => string.Concat("CAST(", field, " AS VARCHAR", ")");

		/// <summary>
		/// 使用数据库自增规则时的默认值, 为null时忽略此自增字段
		/// </summary>
		protected internal virtual string IdentityKeyDefault { get; } = "0";

		/// <summary>
		/// 字符串与字符串之间的连接符
		/// </summary>
		protected internal virtual string StringConnectWord { get; } = "||";

		/// <summary>
		/// sql语句参数化前缀
		/// </summary>
		protected internal virtual string DbParameterPrefix { get; } = "@";

		/// <summary>
		/// 最大小数精度, 0代表由数据库返回定义
		/// </summary>
		protected internal virtual ushort MaximumPrecision { get; } = 0;

		/// <summary>
		/// EXCEPT语法关键字
		/// </summary>
		protected internal virtual string ExceptKeyName { get; } = "EXCEPT";

		/// <summary>
		/// COALESCE语法
		/// </summary>
		/// <param name="field"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		protected internal virtual string CallCoalesce(string field, string parameterName, List<DbParameter> ps) => string.Concat("COALESCE(", field, ",", parameterName, ")");

		/// <summary>
		/// 初始化Context的时候执行
		/// </summary>
		/// <param name="connectionString"></param>
		protected internal virtual void Initialization(string connectionString) { }

		/// <summary>
		/// 转化为字符串sql语句
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		protected internal virtual string ConvertSqlToString(ISqlBuilder sqlBuilder)
		{
			var sql = sqlBuilder.CommandText;

			foreach (var p in sqlBuilder.Params)
			{
				var value = p.Value?.ToString();
				var key = GetSqlDbParameterName(p.ParameterName);
				if (value == null) sql = GetNullSql(sql, key);
				else if (ParamPattern.IsMatch(value) && p.DbType == DbType.String) sql = sql.Replace(key, value);
				else sql = sql.Replace(key, $"'{value}'");
			}
			return sql.Replace("\r", " ").Replace("\n", " ");
		}

		/// <summary>
		/// 通过object类型判断设置特别的数据库参数, 如MySqlGeometry类型
		/// </summary>
		/// <param name="format">包含'{0}'的字符串格式</param>
		/// <param name="value">传入值, 传出转换后的值</param>
		/// <returns>是否通过设置</returns>
		protected internal virtual bool TrySetSpecialDbParameter(out string format, ref object value)
		{
			format = null;
			return false;
		}

		/// <summary>
		/// 自定义输出, 
		/// </summary>
		/// <param name="type">输出类型</param>
		/// <param name="format">包含'{0}'的字符串格式</param>
		/// <returns></returns>
		protected internal virtual bool TryGetSpecialReturnFormat(Type type, out string format)
		{
			format = null;
			return false;
		}

		/// <summary>
		/// 数据库列名使用符号包裹
		/// </summary>
		/// <param name="field">字段名称/表名称/模式名称</param>
		/// <returns></returns>
		protected internal virtual string WithQuote(string field) => string.Concat('"', field, '"');

		/// <summary>
		/// 获取delete sql语句
		/// </summary>
		/// <param name="table">表名</param>
		/// <param name="alias">表的别名</param>
		/// <param name="wheres">删除条件</param>
		/// <returns></returns>
		protected internal virtual string GetDeleteSql(string table, string alias, List<string> wheres)
		{
			return $"DELETE FROM {table} AS {alias} WHERE {string.Join(" AND ", wheres)}";
		}

		/// <summary>
		/// 获取增补sql语句, 此处非主键唯一键(UniqueKey)不会并列为匹配条件, 所以包含唯一键的表需要保证数据库不包含此行. 
		/// </summary>
		/// <param name="table">表名</param>
		/// <param name="upsertSets">需要设置的值</param>
		/// <param name="returning">是否返回upsert结果</param>
		/// <returns></returns>
		protected internal virtual string GetUpsertSql<TModel>(string table, IDictionary<string, string> upsertSets, bool returning) where TModel : class, ICreeperModel, new()
			=> throw new CreeperNotSupportedException();

		/// <summary>
		/// 获取更新sql语句
		/// </summary>
		/// <param name="table">表名</param>
		/// <param name="alias">表名别称</param>
		/// <param name="setList">设置项的集合</param>
		/// <param name="whereList">更新条件</param>
		/// <param name="returning">是否返回更新结果</param>
		/// <returns></returns>
		protected internal virtual string GetUpdateSql<TModel>(string table, string alias, List<string> setList, List<string> whereList, bool returning) where TModel : class, ICreeperModel, new()
		{
			if (returning) throw new CreeperNotSupportedException(DataBaseKind.ToString() + "数据库Update语句暂不支持RETURNING");
			return $"UPDATE {table} AS {alias} SET {string.Join(",", setList)} WHERE {string.Join(" AND ", whereList)}";
		}

		/// <summary>
		/// 获取插入sql语句
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="table">表名</param>
		/// <param name="inserts">插入的键值对</param>
		/// <param name="returning">是否返回插入结果</param>
		/// <returns></returns>
		protected internal virtual string GetInsertRangeSql<TModel>(string table, IDictionary<string, string>[] inserts, bool returning) where TModel : class, ICreeperModel, new()
		{
			if (returning) throw new CreeperNotSupportedException(DataBaseKind.ToString() + "数据库InsertRange语句暂不支持RETURNING");
			var sql = $"INSERT INTO {table} ({string.Join(", ", inserts[0].Keys)})";

			sql += $" VALUES" + string.Join(",", inserts.Select(i => $"({string.Join(", ", i.Values)})"));

			return sql;
		}

		/// <summary>
		/// 获取插入sql语句
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="table">表名</param>
		/// <param name="inserts">插入的键值对</param>
		/// <param name="wheres">插入条件</param>
		/// <param name="returning">是否返回插入结果</param>
		/// <returns></returns>
		protected internal virtual string GetInsertSql<TModel>(string table, IDictionary<string, string> inserts, List<string> wheres, bool returning) where TModel : class, ICreeperModel, new()
		{
			if (returning) throw new CreeperNotSupportedException(DataBaseKind.ToString() + "数据库Insert语句暂不支持RETURNING");
			var sql = $"INSERT INTO {table} ({string.Join(", ", inserts.Keys)})";
			if (wheres.Count == 0)
				sql += $" VALUES({string.Join(", ", inserts.Values)})";
			else
				sql += $" SELECT {string.Join(", ", inserts.Values)} WHERE {string.Join(" AND ", wheres)}";

			return sql;
		}

		/// <summary>
		/// 获取查询语句
		/// </summary>
		/// <param name="columns">查询的列</param>
		/// <param name="table">表名</param>
		/// <param name="alias">主表的别名 例如: [table] AS [alias]</param>
		/// <param name="wheres">查询条件 例如: WHERE [expressions]</param>
		/// <param name="groupBy">分组查询 例如: GROPY BY [columns]</param>
		/// <param name="having">统计函数过滤 例如: GROPY BY [columns] HAVING [expressions]</param>
		/// <param name="orderBy">排序 例如: ORDER BY [columns] [DESC/ASC*option]</param>
		/// <param name="limit">前x条数据 例如: LIMIT [int]</param>
		/// <param name="offset">偏移量 例如 OFFSET [int]</param>
		/// <param name="union">合并结果集的视图 例如: UNION [ALL*option] [view]</param>
		/// <param name="except">排除结果集的视图 例如: EXCEPT [view]</param>
		/// <param name="intersect">相交结果集的视图 例如: INTERSECT [view]</param>
		/// <param name="join">关联查询表与条件 例如: [method] join [table] [alias] on [expressions]</param>
		/// <param name="afterTableStr">某些种类数据库需要在主表后面使用特殊函数的字符串 例如: Postgresql的随机抽样函数(tablesample system)</param>
		/// <returns></returns>
		protected internal virtual string GetSelectSql(string columns, string table, string alias, IEnumerable<string> wheres, string groupBy, string having, string orderBy, int? limit, int? offset, string union, string except, string intersect, string join, string afterTableStr)
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
			if (limit.HasValue) sqlText.Append("LIMIT ").Append(limit.Value).AppendLine();
			if (offset.HasValue) sqlText.Append("OFFSET ").Append(offset.Value).AppendLine();
			return sqlText.ToString();
		}

		/// <summary>
		/// 排序参数化实例
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="ps"></param>
		protected internal virtual void OrderDbParameters(string sql, ref DbParameter[] ps) { }

		/// <summary>
		/// 准备命令
		/// </summary>
		/// <param name="cmd"></param>
		protected internal virtual void PrepareDbCommand(DbCommand cmd) { }

		/// <summary>
		/// 解析ORDER BY条件, 如果没有重写此方法, NullsLast不生效
		/// </summary>
		/// <param name="field">字段名称</param>
		/// <param name="asc">是否升序</param>
		/// <param name="isNullsLast">是否null在行尾</param>
		/// <returns></returns>
		protected internal virtual string ExplainOrderBy(string field, bool asc, bool isNullsLast) => string.Concat(field, " ", asc ? "ASC" : "DESC");

		/// <summary>
		/// LIKE语法
		/// </summary>
		/// <param name="isIngoreCase">是否忽略大小写</param>
		/// <param name="isNot">时候包含非运算符</param>
		/// <returns>Format语句</returns>
		protected internal virtual string ExplainLike(bool isIngoreCase, bool isNot) => string.Concat("{0}", isNot ? " NOT" : null, " LIKE {1}");

		/// <summary>
		/// 是否参数化数组, false: 把数组元素参数化, 而不是数组本身
		/// </summary>
		protected internal virtual bool MergeArray { get; } = false;

		/// <summary>
		/// 解析Any语法
		/// </summary>
		/// <param name="field"></param>
		/// <param name="isNot"></param>
		/// <param name="parameters"></param>
		/// <param name="memberIsArray"></param>
		/// <returns></returns>
		protected internal virtual string ExplainAny(string field, bool isNot, IEnumerable<string> parameters, bool memberIsArray)
		{
			return parameters.Count() == 1
				? string.Concat(field, isNot ? "<>" : "=", parameters.ElementAt(0))
				: string.Concat(field, isNot ? " NOT" : null, " IN(", string.Join(",", parameters), ")");
		}

		/// <summary>
		/// 获取sql语句中的参数化名称，因为各数据库前缀不一样
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected internal string GetSqlDbParameterName(string name) => DbParameterPrefix + name;

		/// <summary>
		/// 设置数据库版本
		/// </summary>
		/// <param name="connectionString"></param>
		protected internal void SetServerVersion(string connectionString)
		{
			using var connection = GetDbConnection(connectionString);
			connection.Open();
			var version = connection.ServerVersion;
			if (connection.ServerVersion.Count(a => a == '.') > 3)
			{
				var arr = connection.ServerVersion.Split('.');
				version = string.Concat(arr[0], '.', arr[1], '.', arr[2], '.', arr[3]);
			}
			ServerVersion = Version.Parse(version);
		}

		/// <summary>
		/// 是否大小写敏感翻译器
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		protected internal string CaseInsensitiveTranslator(string field) => IsColumnNameCaseInsensitive ? field.ToLower() : field;

		/// <summary>
		/// 转化null值 例如: Where("name = {0}", null)=>where name is null
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		protected internal string GetNullSql(string sql, string key)
		{
			var equalsReg = new Regex(@"=\s*" + key);
			var notEqualsReg = new Regex(@"(!=|<>)\s*" + key);
			if (notEqualsReg.IsMatch(sql))
				return notEqualsReg.Replace(sql, " IS NOT NULL");
			else if (equalsReg.IsMatch(sql))
				return equalsReg.Replace(sql, " IS NULL");
			else
				return sql;
		}

		/// <summary>
		/// 获取Nullable类型的泛型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected Type GetOriginalType(Type type) => type.GetOriginalType();

		/// <summary>
		/// 获取表的所有主键
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="withQuote"></param>
		/// <returns></returns>
		protected string[] GetPks<T>(bool withQuote = false) => EntityUtils.GetPks<T>(withQuote);

		/// <summary>
		/// 获取表的所有自增主键
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		protected string[] GetIdenKeysWithQuote<T>() => EntityUtils.GetIdenKeysWithQuote<T>();

		/// <summary>
		/// 获取所有返回的列
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="alias"></param>
		/// <returns></returns>
		protected string GetRetColumns<T>(string alias = null) where T : ICreeperModel => EntityUtils.GetFieldsAlias<T>(alias);

		#region Convert.Data

		/// <summary>
		/// 遍历元组类型
		/// </summary>
		/// <param name="objType"></param>
		/// <param name="dr"></param>
		/// <param name="columnIndex"></param>
		/// <returns></returns>
		private object GetValueTuple(Type objType, IDataReader dr, ref int columnIndex)
		{
			if (objType.IsTuple())
			{
				FieldInfo[] fs = objType.GetFields();
				Type[] types = new Type[fs.Length];
				object[] parameters = new object[fs.Length];
				for (int i = 0; i < fs.Length; i++)
				{
					types[i] = fs[i].FieldType;
					parameters[i] = GetValueTuple(types[i], dr, ref columnIndex);
				}
				ConstructorInfo info = objType.GetConstructor(types);
				return info.Invoke(parameters);
			}
			// 当元组里面含有实体类
			if (objType.IsClass && !objType.IsSealed && !objType.IsAbstract)
			{
				if (!typeof(ICreeperModel).IsAssignableFrom(objType))
					throw new CreeperNotSupportedException("only the generate models.");

				var model = Activator.CreateInstance(objType);
				var isSet = false; // 这个实体类是否有赋值 没赋值直接返回 default

				var fs = EntityUtils.GetPropertiesName(objType);
				for (int i = 0; i < fs.Length; i++)
				{
					++columnIndex;
					if (!dr[columnIndex].IsNullOrDBNull())
					{
						isSet = true;
						SetPropertyValue(objType, dr[columnIndex], model, fs[i]);
					}
				}
				return isSet ? model : default;
			}
			else
			{
				++columnIndex;
				return ConvertData(dr[columnIndex], objType);
			}
		}

		/// <summary>
		/// 反射设置实体类字段值
		/// </summary>
		/// <param name="objType">获取属性的类的类型</param>
		/// <param name="value">字段的值</param>
		/// <param name="model">需要设置的类</param>
		/// <param name="fs">字段名称</param>
		private void SetPropertyValue(Type objType, object value, object model, string fs)
		{
			var p = objType.GetProperty(fs, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (p != null) p.SetValue(model, ConvertData(value, p.PropertyType));
		}

		/// <summary>
		/// 把数据库返回数据转化为C#基本类型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		protected internal T ConvertData<T>(object value) => (T)ConvertData(value, typeof(T));

		/// <summary>
		/// 把数据库返回数据转化为C#基本类型
		/// </summary>
		/// <param name="value"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		protected internal object ConvertData(object value, Type convertType)
		{
			if (value.IsNullOrDBNull())
				return null;
			if (convertType == typeof(object) || value.GetType() == convertType)
				return value;
			return ConvertDbData(value, convertType);
		}

		/// <summary>
		/// 数据转化为C#基本类型, 因不同数据库有不同的类型转换, 所以可派生重写
		/// </summary>
		/// <param name="value"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		protected internal virtual object ConvertDbData(object value, Type convertType)
		{
			convertType = convertType.GetOriginalType();
			var converter = TypeDescriptor.GetConverter(convertType);
			return converter.CanConvertFrom(value.GetType()) ? converter.ConvertFrom(value) : Convert.ChangeType(value, convertType);
		}

		/// <summary>
		/// 遍历数据库查询结果并转换类型
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		protected internal object ConvertDataReader(IDataReader reader, Type convertType)
		{
			bool isValueOrString = convertType == typeof(string) || convertType.IsValueType || convertType == typeof(object);
			object model;
			if (convertType.IsTuple())
			{
				int columnIndex = -1;
				model = GetValueTuple(convertType, reader, ref columnIndex);
			}
			else if (isValueOrString || convertType.IsEnum)
			{
				model = ConvertData(reader[0], convertType);
			}
			else
			{
				model = Activator.CreateInstance(convertType);
				bool isDictionary = typeof(IDictionary).IsAssignableFrom(convertType); //判断是否字典类型

				for (int i = 0; i < reader.FieldCount; i++)
				{
					if (isDictionary)
						convertType.GetMethod("Add").Invoke(model, new[] { reader.GetName(i), reader[i].IsNullOrDBNull() ? null : reader[i] });
					else
						SetPropertyValue(convertType, reader[i], model, reader.GetName(i));
				}
			}
			return model;
		}

		/// <summary>
		/// 遍历数据库查询结果并转换类型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objReader"></param>
		/// <returns></returns>
		protected internal T ConvertDataReader<T>(IDataReader objReader) => (T)ConvertDataReader(objReader, typeof(T));

		#endregion
	}
}