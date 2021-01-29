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

namespace Meta.xUnitTest.Model
{
	public class LowercaseContractResolver : DefaultContractResolver
	{
		protected override string ResolvePropertyName(string propertyName)
		{
			return propertyName.ToLower();
		}
	}

	public class BooleanConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(bool) || objectType == typeof(bool?);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null)
				return null;

			return ConvertToBoolean(reader.Value);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
				writer.WriteNull();
			else
				writer.WriteValue(ConvertToBoolean(value));
		}
		private bool ConvertToBoolean(object val)
		{
			try
			{
				return Convert.ToBoolean(val);
			}
			catch { }
			return false;
		}
	}
	public class DateTimeConverter : DateTimeConverterBase
	{

		// 本地时区 1970.1.1格林威治时间
		public static DateTime Greenwich_Mean_Time = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null) return null;

			if (CanConvert(objectType))
				return ToDateTime(reader.Value);
			else
				return reader.Value;
		}
		//转化成Format形式
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null) writer.WriteNull();
			if (value is DateTime dt)
				writer.WriteValue(ToTimestamp(dt));
			else
				writer.WriteValue(value);
		}
		public static long ToTimestamp(DateTime dt)
		{
			return (dt.ToUniversalTime().Ticks - Greenwich_Mean_Time.Ticks) / 10000;
		}
		public static DateTime ToDateTime(object val)
		{
			DateTime dt = Greenwich_Mean_Time;
			try
			{
				return val switch
				{
					null => dt,
					long v => new DateTime(Greenwich_Mean_Time.Ticks + Convert.ToInt64(val) * 10000).ToLocalTime(),
					_ => Convert.ToDateTime(val),
				};
			}
			catch { }
			return dt;
		}

	}
}
