using Creeper.Annotations;
using Creeper.Driver;
using Creeper.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Creeper.Utils
{
	/// <summary>
	/// 数据库表特性帮助类
	/// </summary>
	internal class EntityUtils
	{
		private static readonly IDictionary<string, TypeFieldsInfo> _typeFields = new Dictionary<string, TypeFieldsInfo>();

		private const string SystemLoadSuffix = ".SystemLoad";
		private static readonly object _lock = new object();
		private readonly CreeperConverter _converter;

		internal EntityUtils(CreeperConverter converter)
		{
			_converter = converter;
		}

		private static string GetKey(Type type) => string.Concat(type.FullName, SystemLoadSuffix);

		/// <summary>
		/// 根据实体类获取所有主键, 包含符号(此方法用于检索where条件中的主键)
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string[] GetIdenKeysWithQuote(Type type) => GetTypeFieldsInfo(type).IdenKeysWithQuote;

		/// <summary>
		/// 根据实体类获取所有主键, 包含符号(此方法用于检索where条件中的主键)
		/// </summary>
		/// <returns></returns>
		public string[] GetIdenKeysWithQuote<T>() => GetIdenKeysWithQuote(typeof(T));


		/// <summary>
		/// 根据实体类获取所有主键
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string[] GetPks(Type type, bool withQuote = false) => withQuote
			? GetTypeFieldsInfo(type).PksWithQuote
			: GetTypeFieldsInfo(type).Pks;

		/// <summary>
		/// 根据实体类获取所有主键
		/// </summary>
		/// <returns></returns>
		public string[] GetPks<T>(bool withQuote = false) => GetPks(typeof(T), withQuote);

		/// <summary>
		/// 根据实体类获取所有字段数组, 不包含数据库字段符号
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public string[] GetReturningFields<T>() => GetReturningFields(typeof(T));

		/// <summary>
		/// 根据实体类获取所有字段数组, 不包含数据库字段符号
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string[] GetReturningFields(Type type) => GetTypeFieldsInfo(type).Fields;

		/// <summary>
		/// 获取对象属性名称
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public string[] GetPropertiesName(Type type) => GetTypeFieldsInfo(type).PropertiesName;

		/// <summary>
		/// 根据类型初始化, 实体类map
		/// </summary>
		/// <param name="type"></param>
		private TypeFieldsInfo GetTypeFieldsInfo(Type type)
		{
			var key = GetKey(type);
			if (!_typeFields.TryGetValue(key, out var value))
				lock (_lock)
				{
					if (!type.GetInterfaces().Contains(typeof(ICreeperModel)))
						throw new CreeperNotDbModelDeriverException(type.FullName);

					value = GetTypeFields(type);
					_typeFields[key] = value;
				}
			return value;
		}

		/// <summary>
		/// 获取当前所有字段列表
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private TypeFieldsInfo GetTypeFields(Type type)
		{
			var fields = new List<string>();
			var pks = new List<string>();
			var pkWithQuote = new List<string>();
			var idenKeysWithQuote = new List<string>();
			var propertiesName = new List<string>();
			PropertiesEnumerator(p =>
			{
				var name = _converter.CaseInsensitiveTranslator(p.Name);
				var column = p.GetCustomAttribute<CreeperColumnAttribute>();
				if (column != null)
				{
					if (column.IsPrimary)
					{
						pkWithQuote.Add(_converter.WithQuote(name));
						pks.Add(name);
					}
					if (column.IsIdentity)
						idenKeysWithQuote.Add(_converter.WithQuote(name));
				}

				var returningName = "{0}" + _converter.WithQuote(name);
				if (_converter.TryGetSpecialReturnFormat(p.PropertyType, out var format))
					returningName = string.Format(format, returningName);

				if (column == null || (column.IgnoreFlags & IgnoreWhen.Returning) == 0)
				{
					fields.Add(returningName);
					propertiesName.Add(name);
				}
			}, type);
			var fieldInfo = new TypeFieldsInfo
			{
				PropertiesName = propertiesName.ToArray(),
				Fields = fields.ToArray(),
				Pks = pks.ToArray(),
				IdenKeysWithQuote = idenKeysWithQuote.ToArray(),
				PksWithQuote = pkWithQuote.ToArray(),
			};

			return fieldInfo;
		}

		/// <summary>
		/// 获取Mapping特性
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static CreeperTableAttribute GetDbTable<T>() where T : ICreeperModel => GetDbTable(typeof(T));


		/// <summary>
		/// 获取Mapping特性
		/// </summary>
		/// <returns></returns>
		public static CreeperTableAttribute GetDbTable(Type type)
			=> type.GetCustomAttribute<CreeperTableAttribute>() ?? throw new ArgumentNullException(nameof(CreeperTableAttribute));

		/// <summary>
		/// 获取当前类字段的字符串, 包含字段符号
		/// </summary>
		/// <param name="type"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public string GetFieldsAlias(Type type, string alias = null)
		{
			var fs = GetTypeFieldsInfo(type).Fields;
			var sb = new StringBuilder();
			alias = !string.IsNullOrEmpty(alias) ? alias + '.' : null;

			for (int i = 0; i < fs.Length; i++) sb.AppendFormat(fs[i], alias).Append(',');
			return sb.ToString().TrimEnd(',');
		}

		/// <summary>
		/// 获取当前类字段的字符串, 包含字段符号
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public string GetFieldsAlias<T>(string alias = null) where T : ICreeperModel => GetFieldsAlias(typeof(T), alias);

		/// <summary>
		/// 遍历所有字段
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		internal static void PropertiesEnumerator<T>(Action<PropertyInfo> action) where T : ICreeperModel => PropertiesEnumerator(action, typeof(T));

		/// <summary>
		/// 遍历所有字段
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		static void PropertiesEnumerator(Action<PropertyInfo> action, Type type)
		{
			IEnumerable<PropertyInfo> properties = GetProperties(type);
			foreach (var p in properties)
				action?.Invoke(p);
		}

		internal static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p =>
			{
				var column = p.GetCustomAttribute<CreeperColumnAttribute>();
				if (column == null) return true;
				//如果字段含有忽略返回自增标识
				if ((column.IgnoreFlags & IgnoreWhen.Returning) != 0)
					return false;
				return true;
			});
		}

		internal class TypeFieldsInfo
		{
			/// <summary>
			/// 用于按顺序从对象中获取属性
			/// </summary>
			public string[] PropertiesName { get; set; }
			/// <summary>
			/// 用于按顺序输出列, 没有符号
			/// </summary>
			public string[] Fields { get; set; } = new string[0];
			/// <summary>
			/// 所有主键字段, 没有符号
			/// </summary>
			public string[] Pks { get; set; } = new string[0];
			/// <summary>
			/// 所有主键字段, 没有符号
			/// </summary>
			public string[] PksWithQuote { get; set; } = new string[0];
			/// <summary>
			/// 所有自增主键字段, 有符号
			/// </summary>
			public string[] IdenKeysWithQuote { get; set; } = new string[0];

		}
	}
}