using Candy.Common;
using Candy.DbHelper;
using Candy.Extensions;
using Candy.PostgreSql.Extensions;
using Candy.SqlBuilder;
using Newtonsoft.Json.Linq;
using Npgsql.LegacyPostgis;
using NpgsqlTypes;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace Candy.PostgreSql
{
	public class PostgreSqlTypeConverter : CandyTypeConvertBase
	{
		public override T Convert<T>(object value)
		{
			return (T)Convert(value, typeof(T));
		}

		public override object Convert(object value, Type convertType)
		{
			if (value is null) return value;
			try
			{
				switch (convertType)
				{
					case var t when t == typeof(PostgisGeometry):
						return (PostgisGeometry)value;
					// jsonb json 类型
					case var t when PostgreSqlTypeMappingExtensions.JTypes.Contains(t):
						return JToken.Parse(value?.ToString() ?? "{}");

					case var t when t == typeof(NpgsqlTsQuery):
						return NpgsqlTsQuery.Parse(value.ToString());

					case var t when t == typeof(BitArray) && value is bool b:
						return new BitArray(1, b);

					default:
						var converter = TypeDescriptor.GetConverter(convertType);
						return converter.CanConvertFrom(value.GetType()) ? converter.ConvertFrom(value) : System.Convert.ChangeType(value, convertType);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public override string ConvertSqlToString(ISqlBuilder sqlBuilder)
		{
			return TypeHelper.SqlToString(sqlBuilder.CommandText, sqlBuilder.Params);
		}
	}
}
