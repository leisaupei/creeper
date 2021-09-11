using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;

namespace Creeper.xUnitTest.PostgreSql.Model
{
	public class LowercaseContractResolver : DefaultContractResolver
	{
		protected override string ResolvePropertyName(string propertyName)
		{
			return propertyName.ToLower();
		}
	}

	/// <summary>
	/// 转化为时间戳
	/// </summary>
	public class NewtonJsonDateTimeConverter : DateTimeConverterBase
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null || reader.TokenType == JsonToken.Null)
				return null;

			return reader.TokenType switch
			{
				JsonToken.Integer or JsonToken.Float => DateTimeOffset.FromUnixTimeMilliseconds((long)reader.Value).LocalDateTime,
				JsonToken.Date or JsonToken.String => Convert.ToDateTime(reader.Value),
				_ => reader.Value,
			};
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			switch (value)
			{
				case DateTime dateTime:
					writer.WriteValue(new DateTimeOffset(dateTime).ToUnixTimeMilliseconds());
					break;
				case DateTimeOffset dateTimeOffset:
					writer.WriteValue(dateTimeOffset.ToUnixTimeMilliseconds());
					break;
				default:
					writer.WriteValue(Convert.ToInt64(value));
					break;
			}

		}
	}
}
