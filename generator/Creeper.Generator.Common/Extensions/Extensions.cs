using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Creeper.Generator.Common.Extensions
{
	public static class Extensions
	{

		/// <summary>
		///  将首字母转大写
		/// </summary>
		public static string ToUpperPascal(this string s) => string.IsNullOrEmpty(s) ? s : $"{s[0..1].ToUpper()}{s[1..]}";

		/// <summary>
		///  将首字母转大写其余小写
		/// </summary>
		public static string ToUpperPascalOtherLower(this string s) => string.IsNullOrEmpty(s) ? s : $"{s[0..1].ToUpper()}{s[1..].ToLower()}";

		/// <summary>
		///  将首字母转小写
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ToLowerPascal(this string s) => string.IsNullOrEmpty(s) ? s : $"{s[0..1].ToLower()}{s[1..]}";

		/// <summary>
		/// 判断数组不为空
		/// </summary>
		public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> value) => value != null && value.Count() != 0;

	}
}
