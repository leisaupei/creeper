using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Extensions
{
	public static class CreeperDbTypeConverterExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string WithQuotationMarks(this ICreeperDbTypeConverter converter, string value)
		{
			var mark = converter.QuotationMarks ? '"' : '\0';
			return string.Concat(mark, value, mark);
		}
	}
}
