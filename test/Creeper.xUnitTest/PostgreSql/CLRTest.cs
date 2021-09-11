using Creeper.Driver;
using Creeper.Extensions;
using Creeper.PostgreSql.Test.Entity.Model;
using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Xml;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Creeper.xUnitTest.PostgreSql
{
	public class CLRTest : BaseTest
	{
		private const string Name = "lsp";
		[Fact]
		public void Insert()
		{
			var dt = new DateTime(2019, 12, 14, 18, 34, 45, 756, DateTimeKind.Local);
			var info = Context.InsertResult(new CreeperTypeTestModel
			{
				Array_type = new[] { 0, 1 },
				Bit_type = new BitArray(1, false),
				Bool_type = false,
				Box_type = new NpgsqlTypes.NpgsqlBox(1D, 1D, 0D, 0D),
				Bytea_type = Encoding.UTF8.GetBytes(Name),
				Char_type = "1",
				Cidr_type = (IPAddress.Parse("127.0.0.1"), 32),
				Circle_type = new NpgsqlCircle(0D, 0D, 1D),
				Composite_type = new CreeperInfo { Id = Guid.Empty, Name = Name },
				Date_type = DateTime.Now.Date,
				Id = Guid.NewGuid(),
				Decimal_type = 1.1M,
				Enum_type = CreeperDataState.正常,
				Float4_type = 1.1f,
				Float8_type = 1.1,
				Hstore_type = new Dictionary<string, string> { { "name", Name } },
				Inet_type = IPAddress.Parse("127.0.0.1"),
				Int2_type = null,
				Int4_type = null,
				Int8_type = null,
				Interval_type = TimeSpan.FromDays(1),
				Jsonb_type = new JObject
				{
					["name"] = Name
				},
				Json_type = new JObject
				{
					["name"] = Name
				},
				Line_type = new NpgsqlLine(0D, 1D, 2D),
				Lseg_type = new NpgsqlLSeg(new NpgsqlPoint(0, 0), new NpgsqlPoint(1, 1)),
				Macaddr_type = System.Net.NetworkInformation.PhysicalAddress.Parse("44-45-53-54-00-00"),
				Money_type = 1.1M,
				Path_type = new NpgsqlPath(new NpgsqlPoint(0, 0), new NpgsqlPoint(1, 1)),
				Point_type = new NpgsqlPoint(0, 0),
				Polygon_type = new NpgsqlPolygon(new NpgsqlPoint(0, 0), new NpgsqlPoint(1, 1), new NpgsqlPoint(0, 2)),
				Text_type = Name,
				Timestamptz_type = dt,
				Timestamp_type = dt,
				Timetz_type = dt,
				Time_type = DateTime.Now - DateTime.Today,
				Tsquery_type = NpgsqlTsQuery.Parse(Name),
				Tsvector_type = NpgsqlTsVector.Parse(Name),
				Varbit_type = new System.Collections.BitArray(Encoding.UTF8.GetBytes(Name)),
				Varchar_type = Name,
				Xml_type = new XmlDocument(),// $"<summary>{_name}</summary>",
				Bit_length_type = new BitArray(new byte[] { 0 }),
				Uuid_array_type = new[] { Guid.Empty },
				Varchar_array_type = new[] { "广东" }
			});
		}

		[Fact]
		public void Bit1()
		{
			var obj = Context.Select<CreeperTypeTestModel>(a => a.Id == Guid.Empty).FirstOrDefault<object>(a => a.Bit_type);
			Assert.IsType<bool>(obj);
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Bit_type, new BitArray(1, false))
				.ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Bits()
		{
			var obj = Context.Select<CreeperTypeTestModel>(a => a.Id == Guid.Empty).FirstOrDefault<object>(a => a.Bits_type);
			Assert.IsType<BitArray>(obj);
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty)
				.Set(a => a.Bits_type, new BitArray(new[] { false, true, false, true }))
				.ToAffrows();
			Assert.True(affrows > 0);
		}
		[Fact]
		public void BitLength()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Bit_length_type, new BitArray(new byte[] { 0 })).ToAffrows();
			Assert.True(affrows > 0);
		}
		[Fact]
		public void Bool()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Bool_type, false).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Box()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Box_type, new NpgsqlBox(1D, 1D, 0D, 0D)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Bytea()
		{
			var obj = Context.Select<CreeperTypeTestModel>(a => a.Id == Guid.Empty).FirstOrDefault<object>(a => a.Bytea_type);
			Assert.IsType<byte[]>(obj);
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Bytea_type, Encoding.UTF8.GetBytes(Name)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Char()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Char_type, "l").ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Cidr()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Cidr_type, (IPAddress.Parse("127.0.0.1"), 32)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Circle()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Circle_type, new NpgsqlCircle(0D, 0D, 1D)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Composite()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Composite_type, new CreeperInfo { Id = Guid.Empty, Name = Name }).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Date()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Date_type, TimeSpan.FromDays(3)).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Date_type, DateTime.Now).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Uuid()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Id, Guid.Empty).ToAffrows();
			Assert.True(affrows > 0);
		}
		[Fact]
		public void Decimal()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Decimal_type, 1.2M).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Decimal_type, 1.2M).ToAffrows();
			var info = Context.Select<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Sum(a => a.Decimal_type.Value);
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Enum()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Enum_type, CreeperDataState.删除).ToAffrows();
			var test = Context.Select<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).FirstOrDefault(a => a.Enum_type);
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Float4()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Float4_type, 1.2f).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Float4_type, 1.2f).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Float8()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Float8_type, 1.3).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Float8_type, 1.3).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Hstore()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Hstore_type, new Dictionary<string, string> { { "name", Name } }).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Inet()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Inet_type, IPAddress.Parse("127.0.0.1")).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Int2()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Int2_type, 12).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Int2_type, (int?)12).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Int4()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(f => f.Int4_type, 1).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Int4_type, 23).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Int8()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Int8_type, 34, 0).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Int8_type, 34).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Interval()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Interval_type, TimeSpan.FromSeconds(22)).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Interval_type, TimeSpan.FromSeconds(22)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Jsonb()
		{
			var info = Context.Select<CreeperTypeTestModel>(a => a.Id == Guid.Empty).FirstOrDefault(a => a.Jsonb_type);
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Jsonb_type, new JObject { { "name", Name } }).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Json()
		{
			var info = Context.Select<CreeperTypeTestModel>(a => a.Id == Guid.Empty).FirstOrDefault(a => a.Json_type);
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Json_type, new JObject { { "name", Name } }).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Line()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Line_type, new NpgsqlLine(0D, 1D, 2D)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Lseg()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Lseg_type, new NpgsqlLSeg(new NpgsqlPoint(0, 0), new NpgsqlPoint(1, 1))).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Macaddr()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Macaddr_type, System.Net.NetworkInformation.PhysicalAddress.Parse("44-45-53-54-00-00")).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Money()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Money_type, 12.3M).ToAffrows();

			// not supported of increment of postgres dbtype is money 
			Assert.ThrowsAny<CreeperException>(() =>
			{
				Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Money_type, 12.3M).ToAffrows();
			});
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Path()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Path_type, new NpgsqlPath(new NpgsqlPoint(0, 0), new NpgsqlPoint(1, 1))).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Point()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Point_type, new NpgsqlPoint(0, 0)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Polygon()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty)
				.Set(a => a.Polygon_type, new NpgsqlPolygon(new NpgsqlPoint(0, 0), new NpgsqlPoint(1, 1), new NpgsqlPoint(0, 2)))
				.ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Serial2()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Serial2_type, 12).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Serial2_type, 12).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Serial4()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Serial4_type, 23).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Serial4_type, 23).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Serial8()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Serial8_type, 33).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Serial8_type, 33).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Text()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Text_type, Name).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Timestamptz()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Timestamptz_type, TimeSpan.FromDays(17)).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Timestamptz_type, DateTime.Now).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Timestamp()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(f => f.Timestamp_type, TimeSpan.FromSeconds(10), DateTime.Now).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Timestamp_type, DateTime.Now).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Timetz()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Timetz_type, TimeSpan.FromSeconds(10)).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Timetz_type, DateTime.Now).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Time()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Inc(a => a.Time_type, TimeSpan.FromSeconds(22)).ToAffrows();
			affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Time_type, TimeSpan.FromSeconds(22)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Tsquery()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Tsquery_type, NpgsqlTsQuery.Parse(Name)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Tsvector()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Tsvector_type, NpgsqlTsVector.Parse(Name)).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Varbit()
		{
			var obj = Context.Select<CreeperTypeTestModel>(a => a.Id == Guid.Empty).FirstOrDefault<object>(a => a.Varbit_type);
			Assert.IsType<BitArray>(obj);
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Varbit_type, new System.Collections.BitArray(Encoding.UTF8.GetBytes(Name))).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Varchar()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Varchar_type, Name).ToAffrows();
			Assert.True(affrows > 0);
		}

		[Fact]
		public void Xml()
		{
			var xml = new XmlDocument();
			xml.LoadXml("<type>xxx</type>");
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Xml_type, xml).ToAffrows();
			var info = Context.Select<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).FirstOrDefault();
			Assert.True(affrows > 0);

		}
		[Fact]
		public void Array()
		{
			//var affrows = _context.Update<TypeTestModel>().Where(a=>a.Id ==Guid.Empty).Set(a => a.Array_type, null).ToAffrows();
			var affrows = Context.Update<CreeperTypeTestModel>().Where(a => a.Id == Guid.Empty).Set(a => a.Array_type[0], 1).ToAffrows();
			Assert.True(affrows > 0);

		}
	}
}
