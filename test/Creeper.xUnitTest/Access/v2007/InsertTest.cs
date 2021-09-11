using Xunit;
using System.Data.Common;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using Creeper.xUnitTest.Contracts;
using Creeper.Driver;
using Creeper.Access2007.Test.Entity;
using Creeper.Access2007.Test.Entity.Model;
using System.ComponentModel;
using System;
using Creeper.xUnitTest.Extensions;
using System.Collections.Generic;

namespace Creeper.xUnitTest.Access.v2007
{
	public class InsertTest : BaseTest, IInsertTest
	{
		[Fact(Skip = "xxxxx")]
		public void Init()
		{
			var categories = new List<CategoryModel>();
			categories.Add(new CategoryModel { Name = "数码" });
			categories.Add(new CategoryModel { Name = "母婴" });
			categories.Add(new CategoryModel { Name = "家具" });
			categories.Add(new CategoryModel { Name = "日用" });
			categories.Add(new CategoryModel { Name = "厨具" });
			foreach (var item in categories)
			{
				var affrows = Context.Insert(item);
				Assert.Equal(1, affrows);
			}

			var products = new List<ProductModel>();
			products.Add(new ProductModel { Price = 4999.5F, Name = "Apple", CategoryId = categories[0].Id, CreateTime = DateTime.Now, Stock = 99 });
			products.Add(new ProductModel { Price = 300.55F, Name = "奶粉", CategoryId = categories[1].Id, CreateTime = DateTime.Now, Stock = 99 });
			products.Add(new ProductModel { Price = 999.55F, Name = "办公桌", CategoryId = categories[2].Id, CreateTime = DateTime.Now, Stock = 99 });
			products.Add(new ProductModel { Price = 50.55F, Name = "洗头水", CategoryId = categories[3].Id, CreateTime = DateTime.Now, Stock = 99 });
			products.Add(new ProductModel { Price = 50.55F, Name = "碱液", CategoryId = categories[3].Id, CreateTime = DateTime.Now, Stock = 99 });
			products.Add(new ProductModel { Price = 100.55F, Name = "电饭锅", CategoryId = categories[4].Id, CreateTime = DateTime.Now, Stock = 99 });
			foreach (var item in products)
			{
				var affrows = Context.Insert(item);
				Assert.Equal(1, affrows);
			}
		}
		[Fact]
		public void IdentityPk()
		{
			var model = new IdenPkTestModel
			{
				Name = "Sam"
			};
			for (int i = 0; i < 10; i++)
			{
				var result = Context.Insert(model);
				Assert.Equal(1, result);
			}
		}
		[Fact]
		public void UniqueAndIdentityCompositePk()
		{
			var affrows = Context.Insert(new IdenUniCompositePkModel
			{
				Age = 10,
				Name = "Cam",
				NextId = SnowflakeId.Default().NextIdBase16(),
			});
			Assert.Equal(1, affrows);

		}

		[Fact(Skip = "Access单次请求只允许一个T-SQL语句")]
		public void InsertRangeMultiple()
		{
			var list = new List<UniPkTestModel>();
			list.Add(new UniPkTestModel { Name = "Tam", Age = 3 });
			list.Add(new UniPkTestModel { Name = "Tam", Age = 3 });
			list.Add(new UniPkTestModel { Name = "Tam", Age = 3 });
			list.Add(new UniPkTestModel { Name = "Tam", Age = 3 });
			list.Add(new UniPkTestModel { Name = "Tam", Age = 3 });
			var affrows = Context.InsertRange(list, false);
			Assert.Equal(list.Count, affrows);
		}

		[Fact(Skip = "Access不支持INSERT INTO [TABLE] VALUES(VALUE1),(VALUE2)...语法")]
		public void InsertRangeSingle()
		{
		}

		[Fact(Skip = "Access暂不支持使用RETURNING")]
		public void InsertReturning()
		{
			UniCompositePkModel info = new UniCompositePkModel
			{
				Age = 10,
				Name = "Cam",
				NextId = SnowflakeId.Default().NextIdBase16(),
				Id = Guid.NewGuid(),
			};
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.Equal(info.Id, result.Value.Id);
		}

		[Fact(Skip = "Access Insert方法暂不支持使用Where条件")]
		public void InsertWithWhere()
		{
			var info = new UniCompositePkModel
			{
				Age = 10,
				Name = "Cam",
				NextId = SnowflakeId.Default().NextIdBase16(),
				Id = Guid.NewGuid(),
			};
			var affrows = Context.Insert<UniCompositePkModel>().Set(info).WhereNotExists(binder => binder.Where(a => a.Id == info.Id)).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void DoubleUniqueCompositePk()
		{
			var affrows = Context.Insert(new UniCompositePkModel
			{
				Age = 10,
				Name = "Cam",
				NextId = SnowflakeId.Default().NextIdBase16(),
				Id = Guid.NewGuid(),
			});
			Assert.Equal(1, affrows);
		}

		[Fact]
		[Description("Access的随机自动编号也是自动生成的, 所以无需赋值")]
		public void UidPk()
		{
			var model = new UniPkTestModel
			{
				Age = 20,
				Name = "TEST"
			};
			var result = Context.Insert(model);
			Assert.Equal(1, result);
			//Assert.NotEqual(Guid.Empty, result.Value.ID);
		}
	}
}
