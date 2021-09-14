using Creeper.Driver;
using Creeper.SqlServer.Test.Entity.Model;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Types;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using Xunit;

namespace Creeper.xUnitTest.SqlServer
{
	public class CLRTest : BaseTest
	{
		private const int Id = 11;
		[Fact]
		public void Insert()
		{
			var info = Context.Insert(new TypeTestModel
			{
				BigintType = 10,
				BinaryType = new byte[] { 1, 234 },
				BitType = false,
				CharType = "中国",
				DateTimeType = DateTime.Now,
				DateTime2Type = DateTime.Now,
				DateType = DateTime.Today,
				FloatType = 234.2D,
				NumericType = 12.3M,
				RealType = 12.3F,
				SmallIntType = 123,
				TextType = "中国",
				TimeType = DateTime.Now.TimeOfDay,
				TinyIntType = 10,
				VarbinaryType = new byte[] { 1, 234 },
				VarcharType = "中国",
				DateTimeOffsetType = DateTimeOffset.Now,
				DecimalType = 12.3M,
				ImageType = new byte[] { 1, 234 },
				IntType = 123,
				MoneyType = 12.3M,
				NcharType = "中国",
				NtextType = "中国",
				NvarcharType = "中国",
				SmallDateTimeType = DateTime.Now,
				SmallMoneyType = 12.3M,
				UniqueIdentifierType = Guid.NewGuid(),
				XmlType = "<name>中国</name>",
				TimestampType = BitConverter.GetBytes(DateTimeOffset.Now.ToUnixTimeMilliseconds()),
				SqlVariantType = 1,
			});
		}

		/// <summary>
		/// <see cref="SqlDbType.BigInt"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(long.MaxValue)]
		public void Bitint(long value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.BigintType);
			Assert.IsType<long>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.BigintType, value).ToAffrows();
			Assert.Equal(1, result);
			//Assert.Equal(1, result.AffectedRows);
			//Assert.Equal(l, result.Value.BigintType);
		}

		/// <summary>
		/// <see cref="SqlDbType.NVarChar"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("中国")]
		public void Nvarchar(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NvarcharType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NvarcharType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Bit"/>
		/// </summary>
		/// <param name="valueS"></param>
		[Theory]
		[InlineData(false)]
		public void Bit(bool value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.BitType);
			Assert.IsType<bool>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.BitType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Binary"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(new byte[] { 1, 242, 213 })]
		public void Binary(byte[] value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.BinaryType);
			Assert.IsType<byte[]>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.BinaryType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Char"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("中国")]
		public void Char(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.CharType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.CharType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Date"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("2021-1-1")]
		public void DateType(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.DateType);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.DateType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.DateTime"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("2021-1-1 20:00:00.23475")]//实际结果是: 2021-01-01 20:00:00.233
		public void DateTimeType(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.DateTimeType);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.DateTimeType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.DateTime2"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("2021-1-1 20:00:00.6623566")]//实际结果是: 2021-01-01 20:00:00.6633333
		public void DateTime2(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.DateTime2Type);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.DateTime2Type, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.DateTimeOffset"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("2021-1-1 20:00:00.6683425+08:00")]//实际结果是: 2021-01-01 20:00:00.6683425 +08:00
		public void DateTimeOffsetType(DateTimeOffset value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.DateTimeOffsetType);
			Assert.IsType<DateTimeOffset>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.DateTimeOffsetType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Decimal"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(231.231)]
		public void Decimal(decimal value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.DecimalType);
			Assert.IsType<decimal>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.DecimalType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Float"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(231.231D)]
		public void Float(double value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.FloatType);
			Assert.IsType<double>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.FloatType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		[Fact]
		[Description("暂不支持此类型, 或以下面方式获取")]
		public void Geography()
		{
			SqlGeography geography = SqlGeography.Null;
			Context.ExecuteReader(dr =>
			{
				var sqlDr = (SqlDataReader)dr;
				geography = SqlGeography.Deserialize(sqlDr.GetSqlBytes(0));
			}, "select geographytype from typetest where id = 11");

			////Assert.IsType<object>(obj);
			//var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.GeographyType, value).ToAffrows();
			//Assert.Equal(1, result);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		[Fact]
		[Description("暂不支持此类型, 或以下面方式获取")]
		public void Geometry()
		{
			SqlGeometry geometry = SqlGeometry.Null;
			Context.ExecuteReader(dr =>
			{
				var sqlDr = (SqlDataReader)dr;
				geometry = SqlGeometry.Deserialize(sqlDr.GetSqlBytes(0));
			}, "select geometrytype from typetest where id = 11");

			//var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.GeographyType);
			//Assert.IsType<object>(obj);
			//var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.GeographyType, value).ToAffrows();
			//Assert.Equal(1, result);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		[Fact]
		[Description("暂不支持此类型")]
		public void Hierarchyid()
		{
			//var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.HierarchyidType);
			//Assert.IsType<object>(obj);
			//var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.HierarchyidType, value).ToAffrows();
			//Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Image"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(new byte[] { 1, 242, 213 })]
		public void Image(byte[] value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.ImageType);
			Assert.IsType<byte[]>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.ImageType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Int"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(99)]
		public void Int(int value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.IntType);
			Assert.IsType<int>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.IntType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Money"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(245.243)]
		public void Money(decimal value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.MoneyType);
			Assert.IsType<decimal>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.MoneyType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.NChar"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("中国")]
		public void Nchar(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NcharType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NcharType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.NText"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("中国")]
		public void Ntext(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NtextType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NtextType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(35.03577)] //实际结果: 35.04
		public void Numeric(decimal value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NumericType);
			Assert.IsType<decimal>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NumericType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Real"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(35.03577f)] //实际结果: 35.04
		public void Real(float value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.RealType);
			Assert.IsType<float>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.RealType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.SmallDateTime"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("2021-1-1 20:03:29.6623566")]//实际结果是: 2021-01-01 20:03:00
		public void SmallDateTime(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.SmallDateTimeType);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.SmallDateTimeType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.SmallInt"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(short.MaxValue)]
		public void SmallInt(short value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.SmallIntType);
			Assert.IsType<short>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.SmallIntType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.SmallMoney"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(123.125)]
		public void SmallMoney(decimal value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.SmallMoneyType);
			Assert.IsType<decimal>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.SmallMoneyType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		[Fact]
		public void SqlVariant()
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.SqlVariantType);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.SqlVariantType, "中国").ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Text"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("中国")]
		public void Text(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.TextType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.TextType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Time"/>
		/// </summary>
		/// <param name="value"></param>
		[Fact]
		public void Time()
		{
			TimeSpan value = TimeSpan.Parse("20:24:53.2343155");
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.TimeType);
			Assert.IsType<TimeSpan>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.TimeType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Timestamp"/>
		/// </summary>
		/// <param name="value"></param>
		[Fact]
		public void Timestamp()
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.TimestampType);
			Assert.IsType<byte[]>(obj);
			//不能更新时间戳列。
			Assert.ThrowsAny<Creeper.CreeperException>(() =>
			{
				var result = Context.Update<TypeTestModel>(a => a.Id == Id)
				.Set(a => a.TimestampType, BitConverter.GetBytes(DateTimeOffset.Now.ToUnixTimeMilliseconds()))
				.ToAffrows();
			});
		}

		/// <summary>
		/// <see cref="SqlDbType.TinyInt"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(90)]
		public void TinyInt(byte value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.TinyIntType);
			Assert.IsType<byte>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.TinyIntType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.UniqueIdentifier"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("efa09cb0-aa7a-4ba8-b412-6aee707daa91")]
		public void UniqueIdentifier(Guid value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.UniqueIdentifierType);
			Assert.IsType<Guid>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.UniqueIdentifierType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.VarBinary"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(new byte[] { 1, 242, 213 })]
		public void VarBinary(byte[] value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.VarbinaryType);
			Assert.IsType<byte[]>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.VarbinaryType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.VarChar"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("中国")]
		public void Varchar(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.VarcharType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.VarcharType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// <see cref="SqlDbType.Xml"/>
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("<testxml>Title</testxml>")]
		public void Xml(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.XmlType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.XmlType, value).ToAffrows();
			Assert.Equal(1, result);
		}
	}
}
