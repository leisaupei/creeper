using Creeper.Attributes;
using Creeper.Driver;
using Creeper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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

		static IReadOnlyDictionary<string, TypeFieldsInfo> _typeFields;

		const string SystemLoadSuffix = ".SystemLoad";
		static readonly object _lock = new object();

		/// <summary>
		/// 根据实体类获取所有字段数组, 有双引号
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string[] GetFieldsMark(Type type)
		{
			InitStaticTypesFields(type);
			return _typeFields[string.Concat(type.FullName, SystemLoadSuffix)].Fields.Select(a => $"\"{a}\"").ToArray();
		}
		/// <summary>
		/// 根据实体类获取所有主键
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string[] GetPkFields(Type type)
		{
			InitStaticTypesFields(type);
			return _typeFields[string.Concat(type.FullName, SystemLoadSuffix)].PkFields;
		}
		/// <summary>
		/// 根据实体类获取所有主键
		/// </summary>
		/// <returns></returns>
		public static string[] GetPkFields<T>() => GetPkFields(typeof(T));

		/// <summary>
		/// 根据实体类获取所有字段数组, 有双引号
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static string[] GetFieldsMark<T>() where T : ICreeperDbModel
		{
			return GetFieldsMark(typeof(T));
		}

		/// <summary>
		/// 根据实体类获取所有字段数组, 不包含双引号
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string[] GetFields(Type type)
		{
			InitStaticTypesFields(type);
			return _typeFields[string.Concat(type.FullName, SystemLoadSuffix)].Fields;
		}

		/// <summary>
		/// 根据实体类获取所有字段数组, 不包含双引号
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		static string[] GetFields<T>() where T : ICreeperDbModel
		{
			return GetFields(typeof(T));
		}

		/// <summary>
		/// 根据类型初始化, 实体类map
		/// </summary>
		/// <param name="t"></param>
		static void InitStaticTypesFields(Type t)
		{
			lock (_lock)
			{
				if (_typeFields != null) return;
				if (!t.GetInterfaces().Contains(typeof(ICreeperDbModel))) return;
				var types = t.Assembly.GetTypes().Where(f => f.Namespace?.Contains(".Model") == true
					&& f.GetCustomAttribute<CreeperDbTableAttribute>() != null
					&& t.GetInterfaces().Contains(typeof(ICreeperDbModel)));
				var dict = new Dictionary<string, TypeFieldsInfo>();
				foreach (var type in types)
				{
					var key = string.Concat(type.FullName, SystemLoadSuffix);
					var fieldInfo = GetTypeFields(type);
					dict[key] = fieldInfo;
				}
				_typeFields = dict;
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
		static TypeFieldsInfo GetTypeFields(Type type)
		{
			var fields = new List<string>();
			var pkFields = new List<string>();
			GetAllFields(p =>
			{
				fields.Add(p.Name.ToLower());

				var column = p.GetCustomAttribute<CreeperDbColumnAttribute>();
				if (column != null && column.Primary)
					pkFields.Add(p.Name.ToLower());
			}, type);
			var fieldInfo = new TypeFieldsInfo
			{
				Fields = fields.ToArray(),
				PkFields = pkFields.ToArray(),
			};

			return fieldInfo;
		}

		/// <summary>
		/// 获取Mapping特性
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static CreeperDbTableAttribute GetDbTable<T>() where T : ICreeperDbModel
		{
			return GetDbTable(typeof(T));
		}

		/// <summary>
		/// 获取Mapping特性
		/// </summary>
		/// <returns></returns>
		public static CreeperDbTableAttribute GetDbTable(Type type) => type.GetCustomAttribute<CreeperDbTableAttribute>() ?? throw new ArgumentNullException(nameof(CreeperDbTableAttribute));

		/// <summary>
		/// 获取当前类字段的字符串, 包含双引号
		/// </summary>
		/// <param name="type"></param>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string GetFieldsAlias(string alias, Type type)
		{
			InitStaticTypesFields(type);
			var fs = _typeFields[string.Concat(type.FullName, SystemLoadSuffix)].Fields;
			var sb = new StringBuilder();
			for (int i = 0; i < fs.Length; i++)
			{
				sb.Append($"{alias}.\"{fs[i]}\"");
				if (i != fs.Length - 1)
					sb.Append(",");
			}
			return sb.ToString();
		}

		/// <summary>
		/// 获取当前类字段的字符串, 包含双引号
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		public static string GetFieldsAlias<T>(string alias) where T : ICreeperDbModel
		{
			return GetFieldsAlias(alias, typeof(T));
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
			IEnumerable<PropertyInfo> properties = GetProperties(type);
			foreach (var p in properties)
				action?.Invoke(p);
		}

		private static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			return type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p =>
			{
				var column = p.GetCustomAttribute<CreeperDbColumnAttribute>();
				if (column == null) return true;
				if (column.Ignore != IgnoreWhen.None)
					return false;
				return true;
			});
		}

		internal class TypeFieldsInfo
		{
			public string[] Fields { get; set; } = new string[0];
			public string[] PkFields { get; set; } = new string[0];

		}
	}
}