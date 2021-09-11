using Creeper.Driver;
using Creeper.Extensions;
using Creeper.MySql.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;
using TestSpace = MySql.Data.Types;
using Creeper.MySql.Test.Entity.Model;

namespace Creeper.xUnitTest.MySql
{
	public class CLRTest : BaseTest
	{
		public const int Pid = 1;

		[Fact]
		public void Init()
		{
			var info = Context.Insert(new TypeTestModel
			{
				Bigint_t = 10,
				Binary_t = new byte[] { 1, 234 },
				Bit_t = 12,
				Blob_t = new byte[] { 1, 234 },
				Char_t = "中国",
				Datetime_t = DateTime.Now,
				Date_t = DateTime.Today,
				Decimal_t = 213.3M,
				Double_t = 23.41,
				Enum_t = TypeTestEnumT.正常,
				Float_t = 234.2F,
				Geometrycollection_t = new MySqlGeometryCollection(new Creeper.MySql.Types.MySqlGeometry[] {
					new MySqlPoint(6, 2),
					new MySqlPolygon(new[] {
						new[] { new MySqlPoint(1, 2), new MySqlPoint(2, 2), new MySqlPoint(2, 1), new MySqlPoint(1, 2) },
						new[] { new MySqlPoint(1, 2), new MySqlPoint(3, 3), new MySqlPoint(2, 1), new MySqlPoint(1, 2) }
					}),
					new MySqlPoint(23, 2),
				}),
				Geometry_t = new MySqlPoint(23, 2),
				Integer_t = 2,
				Json_t = @"{""key"":""value""}",
				Linestring_t = new MySqlLineString(new[] { new MySqlPoint(1, 2), new MySqlPoint(1, 3) }),
				Numeric_t = 12.3M,
				Point_t = new MySqlPoint(23, 2),
				Polygon_t = new MySqlPolygon(new[] {
					new[] { new MySqlPoint(1, 2), new MySqlPoint(2, 2), new MySqlPoint(2, 1), new MySqlPoint(1, 2) },
					new[] { new MySqlPoint(1, 2), new MySqlPoint(3, 3), new MySqlPoint(2, 1), new MySqlPoint(1, 2) }
				}),
				Real_t = 12.3,
				Set_t = "1,2",
				Smallint_t = 123,
				Text_t = "中国",
				Timestamp_t = DateTime.Now,
				Time_t = DateTime.Now.TimeOfDay,
				Tinyblob_t = new byte[] { 1, 234 },
				Tinyint_t = 10,
				Tinytext_t = "中国",
				Varbinary_t = new byte[] { 1, 234 },
				Mulitpoint_t = new MySqlMultiPoint(new[] { new MySqlPoint(1, 2), new MySqlPoint(1, 2) }),
				Multilinestring_t = new MySqlMultiLineString(new[] {
					new MySqlLineString(new[] { new MySqlPoint(1, 2), new MySqlPoint(1, 3) }),
					new MySqlLineString(new[] { new MySqlPoint(1, 2), new MySqlPoint(1, 3) })
				}),
				Multipolygon_t = new MySqlMultiPolygon(new[]
				{
					new MySqlPolygon(new[] {
						new[] { new MySqlPoint(1, 2), new MySqlPoint(2, 2), new MySqlPoint(2, 1), new MySqlPoint(1, 2) },
						new[] { new MySqlPoint(1, 2), new MySqlPoint(3, 3), new MySqlPoint(2, 1), new MySqlPoint(1, 2) }
					}),
					new MySqlPolygon(new[] {
						new[] { new MySqlPoint(1, 2), new MySqlPoint(2, 2), new MySqlPoint(2, 1), new MySqlPoint(1, 2) },
						new[] { new MySqlPoint(1, 2), new MySqlPoint(3, 3), new MySqlPoint(2, 1), new MySqlPoint(1, 2) }
					})
				}),
				Varchar_t = "中国",
				Year_t = 2021
			});
		}
		[Theory]
		[InlineData(new byte[] { 0, 23, 1 })]
		public void Binary(byte[] bs)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Binary_t, bs).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Binary_t);
			Assert.Equal(1, affrows);
			Assert.True(bs[0] == result[0]);
		}

		[Theory]
		[InlineData(new byte[] { 0, 23, 1 })]
		public void Varbinary(byte[] bs)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Varbinary_t, bs).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Varbinary_t);
			Assert.Equal(1, affrows);
			Assert.True(bs[0] == result[0] && result.Length == bs.Length);
		}

		[Theory]
		[InlineData(9223372036234775807)]
		public void Bigint(long p)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Bigint_t, p).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Bigint_t);
			Assert.Equal(1, affrows);
			Assert.Equal(p, result);
		}

		[Theory]
		[InlineData(200)]
		public void Bit(byte b)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Bit_t, b).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Bit_t);
			Assert.Equal(1, affrows);
			Assert.Equal(b, result);
		}

		[Theory]
		[InlineData(new byte[] { 0, 23, 1 })]
		public void Blob(byte[] bs)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Blob_t, bs).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Blob_t);
			Assert.Equal(1, affrows);
			Assert.True(bs[0] == result[0] && bs.Length == result.Length);
		}

		[Theory]
		[InlineData(new byte[] { 0, 23, 1 })]
		public void TinyBlob(byte[] bs)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Tinyblob_t, bs).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Tinyblob_t);
			Assert.Equal(1, affrows);
			Assert.True(bs[0] == result[0] && bs.Length == result.Length);
		}

		[Theory]
		[InlineData("测试char")]
		public void Char(string str)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Char_t, str).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Char_t);
			Assert.Equal(1, affrows);
			Assert.Equal(str, result);
		}

		[Theory]
		[InlineData("2021-1-1 20:01:23")]
		public void Date(DateTime dt)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Date_t, dt).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Date_t);
			Assert.Equal(1, affrows);
			Assert.Equal(dt.Date, result?.Date);
			Assert.True(result?.Hour == 0 && result?.Minute == 0 && result?.Second == 0);

		}

		[Theory]
		[InlineData("2021-1-1 20:01:23")]
		public void Datetime(DateTime dt)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Datetime_t, dt).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Datetime_t);
			Assert.Equal(1, affrows);
			Assert.Equal(dt, result);
		}
		[Theory]
		[InlineData(23.34)]
		public void Decimal(decimal d)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Decimal_t, d).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Decimal_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(23.34)]
		public void Double(double d)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Double_t, d).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Double_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(TypeTestEnumT.已删除)]
		public void Enum(TypeTestEnumT d)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Enum_t, TypeTestEnumT.已删除).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Enum_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(12.33)]
		public void Float(float f)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Float_t, f).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Float_t);
			Assert.Equal(1, affrows);
			Assert.Equal(f, result);
		}

		[Theory]
		[InlineData(12)]
		public void Integer(int i)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Integer_t, i).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Integer_t);
			Assert.Equal(1, affrows);
			Assert.Equal(i, result);
		}

		[Theory]
		[InlineData(12)]
		public void TinyInt(sbyte i)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Tinyint_t, i).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Tinyint_t);
			Assert.Equal(1, affrows);
			Assert.Equal(i, result);
		}

		[Theory]
		[InlineData(@"{""s"":""sss""}")]
		public void Json(string s)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Json_t, s).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Json_t);
			Assert.Equal(1, affrows);
			Assert.Equal(JToken.Parse(s), JToken.Parse(result));
		}

		[Theory]
		[InlineData(12.33)]
		public void Real(double d)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Real_t, d).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Real_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(0x10)]
		public void Smallint(short d)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Smallint_t, d).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Smallint_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(12.33)]
		public void Numeric(decimal d)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Numeric_t, d).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Numeric_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData("abcd")]
		public void Text(string s)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Text_t, s).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Text_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Theory]
		[InlineData("abcd")]
		public void TinyText(string s)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Tinytext_t, s).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Tinytext_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Theory]
		[InlineData("abcd")]
		public void Varchar(string s)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Varchar_t, s).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault(a => a.Varchar_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Theory]
		[InlineData(2021)]
		public void Year(short s)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Year_t, s).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Year_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Fact]
		public void Time()
		{
			var s = TimeSpan.FromSeconds(20);
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Time_t, s).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Time_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Theory]
		[InlineData("2021-1-1 20:01:23")]
		public void Timestamp(DateTime dt)
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Timestamp_t, dt).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Timestamp_t);
			Assert.Equal(1, affrows);
			Assert.Equal(dt, result);
		}

		[Fact]
		public void Set()
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Set_t, "2,3").Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Set_t);
			Assert.Equal(1, affrows);
		}

		#region Geometry
		[Fact]
		public void Linestring()
		{
			var lineString = new MySqlLineString(new[] { new MySqlPoint(1, 2), new MySqlPoint(1, 3) });
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Linestring_t, lineString).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Linestring_t);
			Assert.Equal(1, affrows);
			Assert.Equal(lineString, result);
		}

		[Fact]
		public void Polygon()
		{
			var polygon = new MySqlPolygon(new[] {
				new[] { new MySqlPoint(1, 2), new MySqlPoint(2, 2), new MySqlPoint(2, 1), new MySqlPoint(1, 2) },
				new[] { new MySqlPoint(1, 2), new MySqlPoint(3, 3), new MySqlPoint(2, 1), new MySqlPoint(1, 2) }
			});
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Polygon_t, polygon).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Polygon_t);
			Assert.Equal(1, affrows);
			Assert.Equal(polygon, result);
		}

		[Fact]
		public void Geometry()
		{
			Creeper.MySql.Types.MySqlGeometry geometry = new MySqlPoint(2, 1);
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Geometry_t, geometry).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Geometry_t);
			Assert.Equal(1, affrows);
			Assert.IsType<MySqlPoint>(result);
		}

		[Fact]
		public void Point()
		{
			Creeper.MySql.Types.MySqlPoint point = new MySqlPoint(6, 2);
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Point_t, point).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Point_t);
			Assert.Equal(1, affrows);
			Assert.Equal(point, result);
		}

		[Fact]
		public void MultiPoint()
		{
			Creeper.MySql.Types.MySqlPoint point = new MySqlPoint(6, 2);
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Point_t, point).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Point_t);
			Assert.Equal(1, affrows);
			Assert.Equal(point, result);
		}

		[Fact]
		public void MultiPolygon()
		{
			Creeper.MySql.Types.MySqlPoint point = new MySqlPoint(6, 2);
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Point_t, point).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Point_t);
			Assert.Equal(1, affrows);
			Assert.Equal(point, result);
		}

		[Fact]
		public void MultiLineString()
		{
			Creeper.MySql.Types.MySqlPoint point = new MySqlPoint(6, 2);
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Point_t, point).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Point_t);
			Assert.Equal(1, affrows);
			Assert.Equal(point, result);
		}

		[Fact]
		public void GeometryCollection()
		{
			Creeper.MySql.Types.MySqlGeometryCollection geometries = new MySqlGeometryCollection(new Creeper.MySql.Types.MySqlGeometry[] {
				new MySqlPoint(6, 2),
				new MySqlPolygon(new[] {
					new[] { new MySqlPoint(1, 2), new MySqlPoint(2, 2), new MySqlPoint(2, 1), new MySqlPoint(1, 2) },
					new[] { new MySqlPoint(1, 2), new MySqlPoint(3, 3), new MySqlPoint(2, 1), new MySqlPoint(1, 2) }
				})
			});
			var affrows = Context.Update<TypeTestModel>().Set(a => a.Geometrycollection_t, geometries).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Geometrycollection_t);
			Assert.Equal(1, affrows);
			Assert.Equal(geometries, result);

		}
		#endregion

	}
}
