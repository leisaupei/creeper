using System;
using System.Data.Common;
using System.Data.SQLite;
using Xunit;
using Creeper.Driver;
using Creeper.Sqlite.Test.Entity.Model;
using System.ComponentModel;

namespace Creeper.xUnitTest.Sqlite
{
	public class CLRTest : BaseTest
	{
		const long Pid = 1;

		[Fact]
		public void InsertModel()
		{
			Context.Insert(new ProductModel
			{
				Img = new byte[] { 12, 21, 1 },
				Price = 12.32,
				Name = "李修皮",
				Stock = 123,
				Id = new Random().Next(10000),
				Category_id = 1
			});
			Context.Insert(new TypeTestModel
			{
				Age = 20,
				Bool_type = false,
				Date_time = DateTime.Now,
				Float_type = 21.3F,
				Numeric_type = 99.12M,
				Time_type = TimeSpan.FromSeconds(3402),
				Guid_type = Guid.NewGuid(),
				Decimal_type = 22.32M
			});
		}

		[Theory]
		[InlineData(9223372036234775807)]
		public void Integer(long p)
		{
			var affrows = Context.Update<ProductModel>().Set(a => a.Stat, p).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<ProductModel>(a => a.Id == Pid).FirstOrDefault(a => a.Stat);
			Assert.Equal(1, affrows);
			Assert.Equal(p, result);
		}

		[Theory]
		[InlineData("测试")]
		public void Text(string c)
		{
			var affrows = Context.Update<ProductModel>().Set(a => a.Name, c).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<ProductModel>(a => a.Id == Pid).FirstOrDefault(a => a.Name);
			Assert.Equal(1, affrows);
			Assert.Equal(c, result);
		}

		[Theory]
		[InlineData(12345.21)]
		public void Real(double d)
		{
			var affrows = Context.Update<ProductModel>().Set(a => a.Price, d).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<ProductModel>(a => a.Id == Pid).FirstOrDefault(a => a.Price);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(new byte[] { 123, 23 })]
		public void Blob(byte[] b)
		{
			var affrows = Context.Update<ProductModel>().Set(a => a.Img, b).Where(a => a.Id == Pid).ToAffrows();
			var result = Context.Select<ProductModel>(a => a.Id == Pid).FirstOrDefault(a => a.Img);
			Assert.Equal(1, affrows);
			Assert.Equal(b.Length, result.Length);
		}

		[Theory]
		[InlineData("2021-9-12 21:22:22.09992")]
		public void Datetime(DateTime value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == 1).Set(a => a.Date_time, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<object>(a => a.Date_time);
			Assert.IsType<DateTime>(obj);
		}

		[Theory]
		[InlineData("21:22:22.09992")]
		[Description("从SQLite取出TimeSpan类型数据，数据库会将之转化为DateTime类型，若使用TimeSpan获取则会取DateTime.TimeOfDay数据返回")]
		public void Time(string v)
		{
			var value = TimeSpan.Parse(v);
			var result = Context.Update<TypeTestModel>(a => a.Id == 1).Set(a => a.Time_type, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<object>(a => a.Time_type);
			Assert.IsType<DateTime>(obj);
			var obj1 = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<TimeSpan>(a => a.Time_type);
			Assert.IsType<TimeSpan>(obj1);
		}

		[Theory]
		[InlineData(22.11)]
		public void Numeric(decimal value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == 1).Set(a => a.Numeric_type, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<object>(a => a.Numeric_type);
			Assert.IsType<decimal>(obj);
		}

		[Theory]
		[InlineData(22.11f)]
		[Description("从SQLite取出float类型数据，数据库会将之转化为double类型，若使用float获取则会自动转换为float")]
		public void Float(float value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == 1).Set(a => a.Float_type, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<object>(a => a.Float_type);
			Assert.IsType<double>(obj);
			var obj1 = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<float>(a => a.Float_type);
			Assert.IsType<float>(obj1);
		}

		[Theory]
		[InlineData(false)]
		public void Bool(bool value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == 1).Set(a => a.Bool_type, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<object>(a => a.Bool_type);
			Assert.IsType<bool>(obj);
		}

		[Theory]
		[InlineData("71b89bbb-b5ce-4542-8239-ac6b1171a862")]
		[Description("char(36)类型数据，框架将之转化为string类型存入数据库，使用Guid获取则会自动转换为Guid")]
		public void GuidType(Guid value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == 1).Set(a => a.Guid_type, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<object>(a => a.Guid_type);
			Assert.IsType<string>(obj);
			var obj1 = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<Guid>(a => a.Guid_type);
			Assert.IsType<Guid>(obj1);
		}
		/// <summary>
		/// decimal(10,2)
		/// </summary>
		/// <param name="value"></param>
		[Theory]
		[InlineData(22.11111)]
		public void Decimal(decimal value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == 1).Set(a => a.Decimal_type, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == 1).FirstOrDefault<object>(a => a.Decimal_type);
			Assert.IsType<decimal>(obj);
		}
	}
}
