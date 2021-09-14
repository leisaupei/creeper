using Creeper.Generator.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Creeper.Generator.Provider.Oracle
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
		public static string ConvertOracleDataTypeToCSharpType(string dataType, int length, int precision, int scale)
		{
			string cSharpType;
			switch (dataType.ToLower())
			{
				case "bfile":
				case "blob":
				case "long raw":
				case "raw":
					cSharpType = "byte[]"; break;

				case "char":
					cSharpType = length switch
					{
						var l when l == 36 => "Guid",
						_ => "string",
					};
					break;
				case "clob":
				case "long":
				case "nchar":
				case "nclob":
				case "nvarchar2":
				case "varchar2":
				case "rowid":
				case "urowid":
					cSharpType = "string"; break;

				case "timestamp":
				case "date":
				case "timestamp(6)":
				case "timestamp(6) with local time zone":
				case "timestamp(6) with time zone":
					cSharpType = "DateTime"; break;


				case "interval year(2) to month": cSharpType = "int"; break;
				case "interval day(2) to second(6)": cSharpType = "TimeSpan"; break;
				case "float":
					cSharpType = precision switch
					{
						var p when p <= 63 => "float",
						var p when p <= 126 => "double",
						_ => "double",
					};
					break;

				case "number":
					cSharpType = precision switch
					{
						_ when scale > 0 => "decimal",
						var p when p == -1 => "int",
						var p when p == 1 => "bool",
						var p when p <= 4 => "short",
						var p when p <= 9 => "int",
						var p when p <= 19 => "long",
						_ => "long",
					};
					break;

				case "binary_double": cSharpType = "double"; break;
				case "binary_float": cSharpType = "float"; break;
				default:
					return "object";
			}
			return cSharpType;
		}
		public static string[] OnlySelectDataTypes = new[] { "interval year(2) to month", "bfile" };
		public static string[] NotSupportDataTypes = new[] { "long raw", "long" };

	}
}
