using Creeper.Attributes;
using Creeper.Driver;
using Creeper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Creeper.DbHelper
{
	/// <summary>
	/// 数据库表特性帮助类
	/// </summary>
	internal static class EntityHelper
	{
		/// <summary>
		/// 参数计数器
		/// </summary>
		static int _paramsCount = 0;

		/// <summary>
		/// 参数后缀
		/// </summary>
		public static string ParamsIndex
		{
			get
			{
				if (_paramsCount == int.MaxValue)
					_paramsCount = 0;
				return "p" + _paramsCount++.ToString().PadLeft(6, '0');
			}
		}


		static Dictionary<string, string[]> _typeFieldsDict;
		static Dictionary<string, string[]> _typeFieldsDictNoSymbol;
		static Dictionary<string, string[]> _typePrimaryKey;

		const string SystemLoadSuffix = ".SystemLoad";

		/// <summary>
		/// 根据实体类获取所有字段数组, 有双引号
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string[] GetFieldsFromStaticType(Type type)
		{
			InitStaticTypesFields(type);
			return _typeFieldsDict[string.Concat(type.FullName, SystemLoadSuffix)];
		}

		/// <summary>
		/// 根据实体类获取所有字段数组, 有双引号
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static string[] GetFieldsFromStaticType<T>() where T : ICreeperDbModel
		{
			return GetFieldsFromStaticType(typeof(T));
		}

		/// <summary>
		/// 根据实体类获取所有字段数组, 不包含双引号
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string[] GetFieldsFromStaticTypeNoSymbol(Type type)
		{
			InitStaticTypesFields(type);
			return _typeFieldsDictNoSymbol[string.Concat(type.FullName, SystemLoadSuffix)];
		}

		/// <summary>
		/// 根据实体类获取所有字段数组, 不包含双引号
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static string[] GetFieldsFromStaticTypeNoSymbol<T>() where T : ICreeperDbModel
		{
			return GetFieldsFromStaticTypeNoSymbol(typeof(T));
		}

		/// <summary>
		/// 根据类型初始化, 实体类map
		/// </summary>
		/// <param name="t"></param>
		static void InitStaticTypesFields(Type t)
		{
			if (_typeFieldsDict != null) return;
			if (!t.GetInterfaces().Any(f => f == typeof(ICreeperDbModel))) return;
			_typeFieldsDict = new Dictionary<string, string[]>();
			_typeFieldsDictNoSymbol = new Dictionary<string, string[]>();
			_typePrimaryKey = new Dictionary<string, string[]>();
			var types = t.Assembly.GetTypes().Where(f => !string.IsNullOrEmpty(f.Namespace) && f.Namespace.Contains(".Model") && f.GetCustomAttribute<DbTableAttribute>() != null);
			foreach (var type in types)
			{
				var key = string.Concat(type.FullName, SystemLoadSuffix);
				var fieldInfo = GetAllFields("", type);
				if (!_typeFieldsDict.ContainsKey(key))
					_typeFieldsDict[key] = fieldInfo.SymbolFields.ToArray();

				if (!_typeFieldsDictNoSymbol.ContainsKey(key))
					_typeFieldsDictNoSymbol[key] = fieldInfo.NoSymbolFields.ToArray();

				if (!_typePrimaryKey.ContainsKey(key))
					_typePrimaryKey[key] = fieldInfo.PkFields.ToArray();
			}
		}

		static void InitStaticTypesFields<T>() where T : ICreeperDbModel
		{
			InitStaticTypesFields(typeof(T));
		}

		/// <summary>
		/// 获取当前所有字段列表
		/// </summary>
		/// <param name="type"></param>
		/// <param name="alias"></param>
		/// <returns>(包含双引号,用于SQL语句,不包含双引号,用于反射)</returns>
		static TypeFieldsInfo GetAllFields(string alias, Type type)
		{
			var fieldInfo = new TypeFieldsInfo();
			alias = !string.IsNullOrEmpty(alias) ? alias + "." : "";
			GetAllFields(p =>
			{
				fieldInfo.SymbolFields.Add(alias + '"' + p.Name.ToLower() + '"');
				fieldInfo.NoSymbolFields.Add(alias + p.Name.ToLower());

				if (p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
					fieldInfo.PkFields.Add(p.Name.ToLower());
			}, type);
			return fieldInfo;
		}

		/// <summary>
		/// 获取Mapping特性
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static DbTableAttribute GetDbTable<T>() where T : ICreeperDbModel
		{
			return GetDbTable(typeof(T));
		}

		/// <summary>
		/// 获取Mapping特性
		/// </summary>
		/// <returns></returns>
		public static DbTableAttribute GetDbTable(Type type) => type.GetCustomAttribute<DbTableAttribute>() ?? throw new ArgumentNullException(nameof(DbTableAttribute));

		/// <summary>
		/// 获取当前类字段的字符串, 包含双引号
		/// </summary>
		/// <param name="type"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string GetModelTypeFieldsString(string alias, Type type)
		{
			InitStaticTypesFields(type);
			return string.Join(", ", _typeFieldsDict[string.Concat(type.FullName, SystemLoadSuffix)].Select(f => $"{alias}.{f}"));
		}

		/// <summary>
		/// 获取当前类字段的字符串, 包含双引号
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string GetModelTypeFieldsString<T>(string alias) where T : ICreeperDbModel
		{
			return GetModelTypeFieldsString(alias, typeof(T));
		}
		/// <summary>
		/// 获取当前类字段的字符串, 不包含双引号
		/// </summary>
		/// <param name="type"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string GetModelTypeFieldsStringNoSymbol(string alias, Type type)
		{
			InitStaticTypesFields(type);
			return string.Join(", ", _typeFieldsDictNoSymbol[string.Concat(type.FullName, SystemLoadSuffix)].Select(f => $"{alias}.{f}"));
		}

		/// <summary>
		/// 获取当前类字段的字符串, 不包含双引号
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string GetModelTypeFieldsStringNoSymbol<T>(string alias) where T : ICreeperDbModel
		{
			return GetModelTypeFieldsString(alias, typeof(T));
		}

		/// <summary>
		/// 遍历所有字段
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="action"></param>
		internal static void GetAllFields<T>(Action<PropertyInfo> action) where T : ICreeperDbModel
		{
			GetAllFields(action, typeof(T));
		}

		/// <summary>
		/// 遍历所有字段
		/// </summary>
		/// <param name="action"></param>
		/// <param name="type"></param>
		static void GetAllFields(Action<PropertyInfo> action, Type type)
		{
			var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			Array.ForEach(properties, action);
		}
		internal class TypeFieldsInfo
		{
			public List<string> SymbolFields { get; set; } = new List<string>();
			public List<string> NoSymbolFields { get; set; } = new List<string>();
			public List<string> PkFields { get; set; } = new List<string>();
		}
	}
}