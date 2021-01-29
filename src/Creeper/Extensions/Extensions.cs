using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Creeper.Extensions
{
	internal static class Extensions
	{
		/// <summary>
		/// 判断数组为空
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> value) => !value?.Any() ?? true;

		/// <summary>
		///  将首字母转小写
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ToLowerPascal(this string s) => string.IsNullOrEmpty(s) ? s : $"{s.Substring(0, 1).ToLower()}{s[1..]}";

		/// <summary>
		/// 类型是否元组
		/// </summary>
		/// <param name="tupleType"></param>
		/// <returns></returns>
		public static bool IsTuple(this Type tupleType) => typeof(ITuple).IsAssignableFrom(tupleType);

		/// <summary>
		/// 当类型是Nullable&lt;T&gt;,则返回T, 否则返回传入类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetOriginalType(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(type) : type;

		/// <summary>
		/// 判断object值是不是空值或DBNull
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNullOrDBNull(this object obj) => obj is DBNull || obj == null;

		/// <summary>
		/// 获取表达式是否可用的集合类型
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static bool IsImplementation<T>(this Type type)
		{
			return typeof(T).IsAssignableFrom(type);
		}

	}
}
