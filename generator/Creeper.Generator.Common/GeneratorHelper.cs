using Creeper.Generator.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Creeper.Generator.Common
{
	public class GeneratorHelper
	{
		/// <summary>
		/// 排除字符串转换器, 匹配字符串: '*','%'
		/// </summary>
		/// <param name="column"></param>
		/// <param name="excepts"></param>
		/// <returns></returns>
		public static string ExceptConvert(string column, string[] excepts)
		{
			var exceptPattern = new List<string>();
			var exceptEqual = new List<string>();
			Array.ForEach(excepts, e =>
			{
				if (e.Contains('*'))
					exceptPattern.Add($"lower({column}) NOT LIKE '{e.Replace('*', '%').ToLower()}'");
				else
					exceptEqual.Add($"'{e.ToLower()}'");
			});
			var wheres = new List<string>();
			if (exceptEqual.Count > 0)
				wheres.Add($"lower({column}) NOT IN ({string.Join(", ", exceptEqual)})");
			if (exceptPattern.Count > 0)
				wheres.Add(string.Join(" AND ", exceptPattern));

			if (wheres.Count > 0)
				return $"({string.Join(" AND ", wheres)})";
			else
				return " 1=1 ";
		}
		public static string ConvertColumnNameToPropertyName(string columnName)
		{
			if (AllUpperReg.IsMatch(columnName))
			{
				return columnName.ToUpperPascalOtherLower();
			}
			return columnName.ToUpperPascal();

		}
		/// <summary>
		/// 去除下划线并首字母大写
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ExceptUnderlineToUpper(string str)
		{
			if (!str.Contains('_'))
				return str.ToUpperPascal();
			else
			{
				var strArr = str.Split('_');
				str = string.Empty;
				foreach (var item in strArr)
					str = string.Concat(str, item.ToUpperPascalOtherLower());
				return str;
			}
		}
		public static readonly Regex AllUpperReg = new Regex("^[A-Z_0-9]+$");

		/// <summary>
		/// 写注释, 自带换行
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="summary"></param>
		public static StringBuilder WriteSummary(string summary, int tab)
		{
			var sb = new StringBuilder();
			if (string.IsNullOrWhiteSpace(summary)) return sb;
			var tabStr = string.Empty;
			for (int i = 0; i < tab; i++)
				tabStr += "\t";
			if (summary.Contains("\n"))
			{
				summary = summary.Replace("\r\n", string.Concat(Environment.NewLine, tabStr, "/// "));
			}
			sb.AppendLine(tabStr + "/// <summary>");
			sb.AppendLine(tabStr + $"/// {summary}");
			sb.AppendLine(tabStr + "/// </summary>");
			return sb;
		}
	}
}
