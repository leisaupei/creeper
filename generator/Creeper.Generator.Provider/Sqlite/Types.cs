using Creeper.Generator.Common.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Creeper.Generator.Provider.Sqlite
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
		public static string ConvertSqliteDataTypeToCSharpType(string dataType)
		{
			int length = 0;
			int scale = 0;
			if (dataType.Contains("("))
			{
				var strs = dataType.Split('(');
				dataType = strs[0];
				var ls = strs[1].Trim(')');
				if (ls.Contains(','))
				{
					length = int.Parse(ls.Split(',')[0]);
					scale = int.Parse(ls.Split(',')[1]);
				}
				else
					length = int.Parse(ls);
			}
			string cSharpType = "object";
			switch (dataType.ToLower().Trim())
			{
				case "int":
				case "integer":
					if (length == 0 || (length > 2 && length <= 4)) cSharpType = "int";
					if (length == 1) cSharpType = "bool";
					if (length > 4 && length <= 8) cSharpType = "long";
					if (length == 2) cSharpType = "short";
					break;
				case "bool":
				case "boolean":
					cSharpType = "bool";
					break;

				case "char":
					cSharpType = length switch
					{
						var l when l == 36 => "Guid",
						_ => "string",
					};
					break;

				case "tinyint":
				case "smallint":
				case "mediumint":
					cSharpType = "short";
					break;
				case "int2":
					cSharpType = "short"; break;
				case "int4":
					cSharpType = "int"; break;

				case "bigint":
				case "int8":
					cSharpType = "long"; break;

				case "unsigned big int":
					cSharpType = "ulong"; break;

				case "character":
				case "varchar":
				case "varying character":
				case "nchar":
				case "native character":
				case "national varying character":
				case "nvarchar":
				case "text":
				case "clob":
					cSharpType = "string";
					break;
				case "time":
					cSharpType = "TimeSpan";
					break;
				case "datetime":
					cSharpType = "DateTime"; break;
				case "blob":
				case "binary":
				case "varbinary":
					cSharpType = "byte[]"; break;
				case "decimal":
				case "numeric":
					cSharpType = "decimal";
					break;

				case "float":
					cSharpType = "float";
					break;
				case "money":
				case "real":
				case "double":
				case "double precision":
					cSharpType = "double"; break;
			}
			return cSharpType;
		}

	}
}
