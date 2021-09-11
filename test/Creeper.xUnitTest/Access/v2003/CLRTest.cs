using System;
using System.Data.Common;
using System.Data.SQLite;
using Xunit;
using Creeper.Driver;
using Creeper.Access2007.Test.Entity.Model;
using System.Data.OleDb;
using System.Diagnostics;

namespace Creeper.xUnitTest.Access.v2003
{
	public class CLRTest : BaseTest
	{
		const int Pid = 1;

		[Fact]
		public void InsertModel()
		{
			var info = Context.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault();
			Context.Insert(new TypeTestModel
			{
				LongType = 123,
				IntegerType = 900,
				BooleanType = false,
				ByteType = 20,
				CurrencyType = 123.211M,
				//Access时间不支持有毫秒
				DateTimeType = DateTime.Now,
				DoubleType = 20.32,
				FloatType = 231.123F,
				FormatTextType = "中国人",
				GuidType = Guid.NewGuid(),
				HttpType = "https://github.com/leisaupei/",
				LongTextType = "中国人",
				NumericType = 202.12M,
				//AttachmentType = "asc",
				ShortTextType = "中国人",
				OLEObjectType = new byte[] { 123, 41, 12 },
				TimeType = DateTime.Now,

			});
		}


		/// <summary>
		/// Access中长整型接收类型是int
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(int.MaxValue)]
		public void Integer(int value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.LongType);
			Assert.IsType<int>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.LongType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// Access中整型接收类型是short
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(short.MaxValue)]
		public void Short(short value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.IntegerType);
			Assert.IsType<short>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.IntegerType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData("测试")]
		public void LongText(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.LongTextType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.LongTextType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData(12345.21)]
		public void Double(double value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.DoubleType);
			Assert.IsType<double>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.DoubleType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData(new byte[] { 123, 23 })]
		public void OLEObject(byte[] value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.OLEObjectType);
			Assert.IsType<byte[]>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.OLEObjectType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData(true)]
		public void Boolean(bool value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.BooleanType);
			Assert.IsType<bool>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.BooleanType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData("36797719-f97d-40fa-9af6-69addb53eb96")]
		public void GuidTest(Guid value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.GuidType);
			Assert.IsType<Guid>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.GuidType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData("中国")]
		public void ShortText(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.ShortTextType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.ShortTextType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData(123.211)]
		public void Currency(decimal value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.CurrencyType);
			Assert.IsType<decimal>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.CurrencyType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// Access时间不支持有毫秒
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("2020-9-12 22:12:11.11223123")]
		public void Datetime(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.DateTimeType);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.DateTimeType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData("中国人")]
		public void FormatText(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.FormatTextType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.FormatTextType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// 不支持此类型插入/更新
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData("abc.jpg")]
		public void Attachment(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.AttachmentType);
			Assert.IsType<string>(obj);
			var exception = Assert.ThrowsAny<Creeper.CreeperException>(() =>
			{
				var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.AttachmentType, value).ToAffrows();
				Assert.Equal(1, result);
			});
			Assert.IsType<System.Data.OleDb.OleDbException>(exception.InnerException);
		}

		[Theory]
		[InlineData("https://github.com/leisaupei/")]
		public void Http(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.HttpType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.HttpType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData(99.22)]
		public void Float(float value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.FloatType);
			Assert.IsType<float>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.FloatType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData(99.22)]
		public void Numeric(decimal value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.NumericType);
			Assert.IsType<decimal>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.NumericType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData(99)]
		public void Byte(byte value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.ByteType);
			Assert.IsType<byte>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.ByteType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory]
		[InlineData("2020-9-12 22:29:11")]
		public void Time(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Pid).FirstOrDefault<object>(a => a.TimeType);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Pid).Set(a => a.TimeType, value).ToAffrows();
			Assert.Equal(1, result);
		}
	}
}
