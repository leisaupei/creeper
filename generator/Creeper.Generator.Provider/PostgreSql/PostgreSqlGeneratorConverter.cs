using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql.Extensions;
using Newtonsoft.Json.Linq;
using Npgsql;
using Npgsql.LegacyPostgis;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Data.Common;
using System.Linq;

namespace Creeper.PostgreSql
{
	internal class PostgreSqlGeneratorConverter : CreeperConverter
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.PostgreSql;

		/// <summary>
		/// 使用DEFAULT关键字可使使用默认规则
		/// </summary>
		protected override string IdentityKeyDefault => "DEFAULT";

		protected override object ConvertDbData(object value, Type convertType)
		{
			return convertType switch
			{
				var t when t == typeof(PostgisGeometry) => (PostgisGeometry)value,
				// jsonb json 类型
				var t when PostgreSqlTypeMappingExtensions.JTypes.Contains(t) => JToken.Parse(value?.ToString() ?? "{}"),
				var t when t == typeof(NpgsqlTsQuery) => NpgsqlTsQuery.Parse(value.ToString()),
				var t when t == typeof(BitArray) && value is bool b => new BitArray(1, b),
				_ => base.ConvertDbData(value, convertType),
			};
		}

		public override DbConnection GetDbConnection(string connectionString)
			=> new NpgsqlConnection(connectionString);

		protected override DbParameter GetDbParameter(string name, object value)
			=> new NpgsqlParameter(name, value);
	}
}
