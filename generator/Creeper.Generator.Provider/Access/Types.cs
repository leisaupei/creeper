using Creeper.Generator.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Creeper.Generator.Provider.Access
{
	/// <summary>
	/// 
	/// </summary>
	public static class Types
	{
		/// <summary>
		/// 数据库类型转化成C#类型String
		/// </summary>
		/// <param name="dataType"></param>
		/// <returns></returns>
		public static string ConvertAccessDataTypeToCSharpType(string dataType)
		{
			string cSharpType;
			switch (dataType)
			{
				case "2": cSharpType = "short"; break;
				case "3": cSharpType = "int"; break;
				case "4": cSharpType = "float"; break;
				case "5": cSharpType = "double"; break;
				case "6": cSharpType = "decimal"; break;
				case "7": cSharpType = "DateTime"; break;
				case "11": cSharpType = "bool"; break;
				case "17": cSharpType = "byte"; break;
				case "72": cSharpType = "Guid"; break;
				case "130": cSharpType = "string"; break;
				case "131": cSharpType = "decimal"; break;
				case "128": cSharpType = "byte[]"; break;

				default:
					return "object";
			}
			return cSharpType;
		}

	}
}
