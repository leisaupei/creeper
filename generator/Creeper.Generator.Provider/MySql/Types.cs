using Creeper.Generator.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.Generator.Provider.MySql
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
		/// <param name="length"></param>
		/// <returns></returns>
		public static string ConvertMySqlDataTypeToCSharpType(string dataType, int length)
		{
			string cSharpType;
			switch (dataType)
			{
				//确定
				case "bigint": cSharpType = "long"; break;

				case "tinyint":
					cSharpType = length switch
					{
						var l when l == 1 => "bool",
						_ => "sbyte",
					};
					break;

				case "int":
				case "mediumint":
				case "integer": cSharpType = "int"; break;

				case "year":
				case "smallint": cSharpType = "short"; break;
				case "time": cSharpType = "TimeSpan"; break;

				case "timestamp":
				case "date":
				case "datetime": cSharpType = "DateTime"; break;

				case "numeric":
				case "decimal": cSharpType = "decimal"; break;

				case "float": cSharpType = "float"; break;

				case "real":
				case "double": cSharpType = "double"; break;

				case "bit":
					cSharpType = length switch
					{
						var l when l == 1 => "bool",
						var l when l <= 8 => "byte",
						var l when l <= 16 => "ushort",
						var l when l <= 32 => "uint",
						var l when l > 32 => "ulong",
						_ => "ulong",
					};
					break;

				case "tinyblob":
				case "longblob":
				case "mediumblob":
				case "blob":
				case "binary":
				case "varbinary":
					cSharpType = "byte[]"; break;

				case "char":
					cSharpType = length switch
					{
						var l when l == 36 => "Guid",
						_ => "string",
					};
					break;
				case "tinytext":
				case "mediumtext":
				case "longtext":
				case "text":
				case "json":
				case "set":
				case "varchar": cSharpType = "string"; break;

				case "point": cSharpType = "Creeper.MySql.Types.MySqlPoint"; break;
				case "multipoint": cSharpType = "Creeper.MySql.Types.MySqlMultiPoint"; break;

				case "polygon": cSharpType = "Creeper.MySql.Types.MySqlPolygon"; break;
				case "multipolygon": cSharpType = "Creeper.MySql.Types.MySqlMultiPolygon"; break;

				case "linestring": cSharpType = "Creeper.MySql.Types.MySqlLineString"; break;
				case "multilinestring": cSharpType = "Creeper.MySql.Types.MySqlMultiLineString"; break;

				case "geometry": cSharpType = "Creeper.MySql.Types.MySqlGeometry"; break;
				case "geometrycollection": cSharpType = "Creeper.MySql.Types.MySqlGeometryCollection"; break;

				case "enum":
					return dataType;
				default:
					return "object";
			}
			return cSharpType;
		}

	}
}
