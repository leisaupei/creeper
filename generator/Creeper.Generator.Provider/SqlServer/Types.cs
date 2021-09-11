using Creeper.Generator.Common.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Creeper.Generator.Provider.SqlServer
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
		public static string ConvertSqlServerDataTypeToCSharpType(string dataType)
		{
			string cSharpType;
			switch (dataType)
			{
				case "bigint": cSharpType = "long"; break;

				case "tinyint":
					cSharpType = "byte"; break;

				case "timestamp":
				case "image":
				case "varbinary":
				case "binary":
					cSharpType = "byte[]"; break;

				case "bit":
					cSharpType = "bool";
					break;

				case "smalldatetime":
				case "date":
				case "datetime":
				case "datetime2": cSharpType = "DateTime"; break;
				case "datetimeoffset": cSharpType = "DateTimeOffset"; break;

				case "time": cSharpType = "TimeSpan"; break;


				case "money":
				case "smallmoney":
				case "numeric":
				case "decimal": cSharpType = "decimal"; break;

				case "real": cSharpType = "float"; break;
				case "float": cSharpType = "double"; break;

				case "smallint": cSharpType = "short"; break;
				case "int": cSharpType = "int"; break;

				case "xml":
				case "char":
				case "text":
				case "varchar":
				case "nvarchar":
				case "nchar":
				case "ntext":
					cSharpType = "string"; break;

				case "uniqueidentifier":
					cSharpType = "Guid"; break;

				case "sql_variant":
				case "hierarchyid":
				case "geography":
				case "geometry":
					cSharpType = "object"; break;

				default:
					return "object";
			}
			return cSharpType;
		}


		public static string[] NotSupportDataType = new[] { "geography", "hierarchyid", "geometry" };
	}
}
