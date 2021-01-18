using Newtonsoft.Json;
using Npgsql.LegacyPostgis;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Candy.PostgreSql
{
	/// <summary>
	/// ip地址
	/// </summary>
	public class IPConverter : JsonConverter<IPAddress>
	{
		public override void WriteJson(JsonWriter writer, IPAddress value, JsonSerializer serializer)
		{
			writer.WriteValue(value?.ToString());
		}

		public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var s = (string)reader.Value;
			if (string.IsNullOrEmpty(s))
				return null;
			return IPAddress.Parse(s);
		}
	}
	/// <summary>
	/// NpgsqlTsVector
	/// </summary>
	public class NpgsqlTsVectorConverter : JsonConverter<NpgsqlTsVector>
	{
		public override NpgsqlTsVector ReadJson(JsonReader reader, Type objectType, [AllowNull] NpgsqlTsVector existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return NpgsqlTsVector.Parse(reader.Value.ToString());
		}

		public override void WriteJson(JsonWriter writer, [AllowNull] NpgsqlTsVector value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}
	}
	/// <summary>
	/// NpgsqlTsQuery
	/// </summary>
	public class NpgsqlTsQueryConverter : JsonConverter<NpgsqlTsQuery>
	{
		public override NpgsqlTsQuery ReadJson(JsonReader reader, Type objectType, [AllowNull] NpgsqlTsQuery existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			return NpgsqlTsQuery.Parse(reader.Value.ToString());
		}

		public override void WriteJson(JsonWriter writer, [AllowNull] NpgsqlTsQuery value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}
	}
	/// <summary>
	/// NpgsqlPolygon/NpgsqlPath Converter
	/// </summary>
	public class NpgsqlPointListConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(NpgsqlPolygon) || objectType == typeof(NpgsqlPolygon?) || objectType == typeof(NpgsqlPath) || objectType == typeof(NpgsqlPath?);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;
			IList<NpgsqlPoint> points = new List<NpgsqlPoint>();
			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndArray)
					break;
				var point = new NpgsqlPoint();
				while (reader.Read())
				{
					if (reader.TokenType == JsonToken.EndObject)
						break;
					if (reader.TokenType != JsonToken.PropertyName)
						continue;

					if (reader.Value.ToString() == "x")
						if (reader.Read())
							point.X = Convert.ToDouble(reader.Value);

					if (reader.Value.ToString() == "y")
						if (reader.Read())
							point.Y = Convert.ToDouble(reader.Value);
				}
				points.Add(point);
			}
			if (objectType.IsGenericType && objectType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				objectType = new NullableConverter(objectType).UnderlyingType;
			return Activator.CreateInstance(objectType, points);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null) writer.WriteNull();
			var polygon = (IList<NpgsqlPoint>)value;
			writer.WriteStartArray();
			foreach (var item in polygon)
			{
				writer.WriteStartObject();
				writer.WritePropertyName("x");
				writer.WriteValue(item.X);
				writer.WritePropertyName("y");
				writer.WriteValue(item.Y);
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
		}
	}
	/// <summary>
	/// BitArray
	/// </summary>
	public class BitArrayConverter : JsonConverter<BitArray>
	{
		public override BitArray ReadJson(JsonReader reader, Type objectType, [AllowNull] BitArray existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.Value == null)
				return new BitArray(0);
			return new BitArray(reader.Value.ToString().Select(f => f == '1').ToArray());
		}

		public override void WriteJson(JsonWriter writer, [AllowNull] BitArray value, JsonSerializer serializer)
		{
			char[] c = new char[value.Length];
			for (int i = 0; i < value.Length; i++)
				c[i] = value[i] ? '1' : '0';
			writer.WriteValue(new string(c));
		}

	}
	/// <summary>
	/// PhysicalAddress
	/// </summary>
	public class PhysicalAddressConverter : JsonConverter<PhysicalAddress>
	{
		public override PhysicalAddress ReadJson(JsonReader reader, Type objectType, [AllowNull] PhysicalAddress existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var s = (string)reader.Value;
			if (string.IsNullOrEmpty(s))
				return null;
			return PhysicalAddress.Parse(s);
		}

		public override void WriteJson(JsonWriter writer, [AllowNull] PhysicalAddress value, JsonSerializer serializer)
		{
			writer.WriteValue(value?.ToString());
		}
	}
	/// <summary>
	/// PostgisGeometry
	/// </summary>
	public class PostgisGeometryConverter : JsonConverter<PostgisGeometry>
	{
		public override PostgisGeometry ReadJson(JsonReader reader, Type objectType, PostgisGeometry existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;
			double x = 0;
			double y = 0;
			uint srid = 0;
			while (reader.Read())
			{
				if (reader.TokenType == JsonToken.EndObject)
					break;
				if (reader.TokenType != JsonToken.PropertyName)
					continue;

				if (reader.Value.ToString() == "x")
					if (reader.Read())
						x = Convert.ToDouble(reader.Value);
				if (reader.Value.ToString() == "y")
					if (reader.Read())
						y = Convert.ToDouble(reader.Value);
				if (reader.Value.ToString() == "srid")
					if (reader.Read())
						srid = Convert.ToUInt32(reader.Value);
			}
			return new PostgisPoint(x, y) { SRID = srid };
		}

		public override void WriteJson(JsonWriter writer, PostgisGeometry value, JsonSerializer serializer)
		{
			switch (value)
			{
				case PostgisPoint point:
					writer.WriteStartObject();
					writer.WritePropertyName("x");
					writer.WriteValue(point.X);
					writer.WritePropertyName("y");
					writer.WriteValue(point.Y);
					writer.WritePropertyName("srid");
					writer.WriteValue(point.SRID);
					writer.WriteEndObject();
					break;
				default:
					throw new NotSupportedException("Property of type geometry only supported PostgisPoint derived class.");
			}
		}
	}
}
