using Candy.Driver.Common;
using Candy.Driver.DbHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Candy.Driver.Extensions
{
	internal static class Extensions
	{
		/// <summary>
		/// 判断数组为空
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> value) => !value?.Any() ?? true;

		/// <summary>
		/// 成员表达式改成数据库字段 a.Xxx-> a."xxx"
		/// </summary>
		/// <param name="mb"></param>
		/// <returns></returns>
		public static string ToDatebaseField(this MemberExpression mb)
		{
			return string.Concat(mb.ToString().ToLower().Replace(".", ".\""), "\"");
		}

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
		public static bool IsTuple(this Type tupleType) => tupleType.Namespace == "System" && tupleType.Name.StartsWith("ValueTuple`", StringComparison.Ordinal);

		/// <summary>
		/// 当类型是Nullable&lt;T&gt;,则返回T,否则返回传入类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetOriginalType(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(type) : type;



		public static bool IsNullOrDBNull(this object obj) => obj is DBNull || obj == null;

	}
}
