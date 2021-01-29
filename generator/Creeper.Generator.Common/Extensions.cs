using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Creeper.Generator.Common
{
	public static class Extensions
	{

		/// <summary>
		///  将首字母转大写
		/// </summary>
		public static string ToUpperPascal(this string s) => string.IsNullOrEmpty(s) ? s : $"{ s.Substring(0, 1).ToUpper()}{s.Substring(1)}";

		/// <summary>
		///  将首字母转小写
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ToLowerPascal(this string s) => string.IsNullOrEmpty(s) ? s : $"{ s.Substring(0, 1).ToLower()}{s.Substring(1)}";

		/// <summary>
		/// 在数组中插入分隔符
		/// </summary>
		/// <param name="arr"></param>
		/// <param name="s">分隔符</param>
		/// <returns></returns>
		public static string Join<T>(this IEnumerable<T> arr, string s) => arr.IsNullOrEmpty() ? null : string.Join(s, arr);

		/// <summary>
		/// 判断数组不为空
		/// </summary>
		public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> value) => value != null && value.Count() != 0;


		/// <summary>
		/// 判断数组为空
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> value) => value == null || value.Count() == 0;
	}
}
