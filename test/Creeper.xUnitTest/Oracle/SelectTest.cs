using System;
using Xunit;
using Creeper.xUnitTest.Contracts;
using Creeper.Driver;
using Creeper.Oracle.Test.Entity.Model;
using System.Collections;
using Creeper.SqlBuilder;
using Creeper.xUnitTest.Extensions;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using System.ComponentModel;

namespace Creeper.xUnitTest.Oracle
{
	public class SelectTest : BaseTest, ISelectTest
	{
		[Fact]
		public void Avg()
		{
			var result1 = Context.Select<ProductModel>().Avg(a => a.Price);
			var result2 = Context.Select<TypeTestModel>().Avg(a => a.DecimalType ?? 0, 0);
		}

		[Fact]
		public void Count()
		{
			var count = Context.Select<ProductModel>().Count();
			Assert.True(count > 0);
		}

		[Fact]
		public void CountDistinct()
		{
			var count = Context.Select<IdenPkTestModel>().CountDistinct(a => a.Name);
			Assert.True(count > 0);
		}
		[Fact]
		public void Distinct()
		{
			//var names = Context.Select<IdenPkTestModel>().Distinst().ToList(a => a.Name);
			//Assert.True(names.Count > 0);
		}

		[Fact, Description("Oracle不支持EXCEPT ALL语法")]
		public void Except()
		{
			var result1 = Context.Select<ProductModel>().Where(a => a.Price > 10).Except<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
			var result3 = Context.Select<ProductModel>().Where(a => a.Price > 10).Except("select *from \"Product\" where \"Price\" > 20").ToList();
			var result5 = Context.Select<ProductModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Price > 10).Except<ProductModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Price > 20)).ToList<(string, string)>();

			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result2 = Context.Select<ProductModel>().Where(a => a.Price > 10).ExceptAll<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
				var result4 = Context.Select<ProductModel>().Where(a => a.Price > 10).ExceptAll("select *from \"Product\" where \"Price\" > 20").ToList();
			});
			Assert.IsType<OracleException>(exception.InnerException);
		}

		[Fact]
		public void FirstOrDefault()
		{
			var result1 = Context.Select<ProductModel>().FirstOrDefault();
			var result2 = Context.Select<ProductModel>().FirstOrDefault<(string, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<ProductModel>().FirstOrDefault<(string, string)>("a.\"Id\",a.\"Name\"");
			var result4 = Context.Select<ProductModel>().FirstOrDefault(a => a.Id);
			var result5 = Context.Select<ProductModel>(a => a.Id == "0").FirstOrDefault(a => a.Id);
			var result6 = Context.Select<ProductModel>().FirstOrDefault<Hashtable>("a.\"Id\",a.\"Name\"");
			Assert.Null(result5);
		}

		[Fact]
		public void Frist()
		{
			Assert.Throws<CreeperFirstNotFoundException>(() =>
			{
				var result1 = Context.Select<ProductModel>(a => a.Id == "0").First();
			});
		}

		[Fact]
		public void GroupBy()
		{
			var result = Context.Select<ProductModel>().GroupBy(a => a.CategoryId).ToList<(string, long)>("a.\"CategoryId\",COUNT(1)");
		}

		[Fact]
		public void Having()
		{
			var result = Context.Select<ProductModel>().GroupBy(a => a.CategoryId).Having("COUNT(1) >= 1").ToList(a => a.CategoryId);
		}

		[Fact, Description("Oracle不支持INTERSECT ALL语法")]
		public void Intersect()
		{
			var result1 = Context.Select<ProductModel>().Where(a => a.Price > 10).Intersect<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
			var result3 = Context.Select<ProductModel>().Where(a => a.Price > 10).Intersect("select *from \"Product\" where \"Price\" > 20").ToList();
			var result5 = Context.Select<ProductModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Price > 10)
				.Intersect<ProductModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Price > 20)).ToList<(string, string)>();

			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result4 = Context.Select<ProductModel>().Where(a => a.Price > 10).IntersectAll("select *from \"Product\" where \"Price\" > 20").ToList();
				var result2 = Context.Select<ProductModel>().Where(a => a.Price > 10).IntersectAll<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
			});
			Assert.IsType<OracleException>(exception.InnerException);
		}

		[Fact]
		public void Join()
		{
			var result1 = Context.Select<ProductModel>().InnerJoin<CategoryModel>((a, b) => a.CategoryId == b.Id).FirstOrDefault();
			var result2 = Context.Select<ProductModel>().LeftJoin<CategoryModel>((a, b) => a.CategoryId == b.Id).FirstOrDefault();
			var result3 = Context.Select<ProductModel>().RightJoin<CategoryModel>((a, b) => a.CategoryId == b.Id).FirstOrDefault();
			var result4 = Context.Select<ProductModel>().LeftOuterJoin<CategoryModel>((a, b) => a.CategoryId == b.Id).FirstOrDefault();
			var result5 = Context.Select<ProductModel>().RightOuterJoin<CategoryModel>((a, b) => a.CategoryId == b.Id).FirstOrDefault();
		}

		[Fact]
		public void Limit()
		{
			var result1 = Context.Select<ProductModel>().Take(10).ToList();
			var result2 = Context.Select<ProductModel>().OrderByDescending(a => a.CreateTime).Take(10).ToList();
		}

		[Fact]
		public void Max()
		{
			var result1 = Context.Select<ProductModel>().Max(a => a.Price);
			var result2 = Context.Select<TypeTestModel>().Max(a => a.DecimalType ?? 0, 0);
		}

		[Fact]
		public void Min()
		{
			var result1 = Context.Select<ProductModel>().Min(a => a.Price);
			var result2 = Context.Select<TypeTestModel>().Min(a => a.DecimalType ?? 0, 0);
		}

		[Fact]
		public void OrderBy()
		{
			var result1 = Context.Select<ProductModel>().OrderByDescending(a => a.Name).FirstOrDefault();
			var result2 = Context.Select<ProductModel>().OrderByDescendingNullsLast(a => a.Name).FirstOrDefault();
			var result3 = Context.Select<ProductModel>().OrderBy(a => a.Name).FirstOrDefault();
			var result4 = Context.Select<ProductModel>().OrderByNullsLast(a => a.Name).FirstOrDefault();
		}

		[Theory]
		[InlineData(1, 5)]
		[InlineData(2, 5)]
		public void Page(int index, int size)
		{
			var result1 = Context.Select<ProductModel>().Page(index, size).ToList();
			var result2 = Context.Select<ProductModel>().OrderByDescending(a => a.CreateTime).Page(index, size).ToList();
		}

		[Fact]
		public void Scalar()
		{
			var result1 = Context.Select<ProductModel>(a => a.Id == "13eacc8bce6e6001").ToScalar("\"Id\"");
			var result2 = Context.Select<ProductModel>(a => a.Id == "13eacc8bce6e6001").ToScalar<string>("\"Id\"");
		}

		[Fact]
		public void SelectByDbCache()
		{
			var result = Context.Select<ProductModel>(a => a.Id == "13eacc8bce6e6001").ByCache(TimeSpan.FromMinutes(1)).FirstOrDefault();
		}

		[Fact]
		public void Skip()
		{
			var result1 = Context.Select<ProductModel>().Skip(10).ToList();
			var result2 = Context.Select<ProductModel>().OrderByDescending(a => a.CreateTime).Skip(10).ToList();
		}

		[Fact]
		public void Sum()
		{
			var result1 = Context.Select<ProductModel>().Sum(a => a.Price);
			var result2 = Context.Select<TypeTestModel>().Sum(a => a.DecimalType ?? 0, 0);
		}

		[Fact]
		public void ToList()
		{
			var result1 = Context.Select<ProductModel>().ToList();
			var result2 = Context.Select<ProductModel>().ToList<(string, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<ProductModel>().ToList<(string, string)>("a.\"Id\",a.\"Name\"");
			var result4 = Context.Select<ProductModel>().ToList(a => a.Id);
			var result5 = Context.Select<ProductModel>(a => a.Id == "0").ToList(a => a.Id);
			var result6 = Context.Select<ProductModel>().ToList<Hashtable>("a.\"Id\",a.\"Name\"");
			Assert.Empty(result5);
		}

		[Fact(Skip = "Oracle不支持SELECT...;SELECT...;多个语句同时请求的管道模式")]
		public void ToPipe()
		{
			object[] obj = Context.ExecuteReaderPipe(new ISqlBuilder[] {
				Context.Select<ProductModel>().ToListPipe(a => a.Id),
				Context.Select<ProductModel>().FirstOrDefaultPipe<(string, string)>(a => new { a.Id, a.Name }),
				Context.Select<ProductModel>().FirstOrDefaultPipe(),
				Context.Select<ProductModel>().Take(2).ToListPipe()

			});
			var info = obj[0].ToObjectArray().OfType<string>();
			var info1 = ((int, string))obj[1];
			var info2 = (ProductModel)obj[2];
			var info3 = obj[3].ToObjectArray().OfType<ProductModel>().ToList();
		}

		[Fact]
		public void ToUnion()
		{
			var result = Context.Select<ProductModel>().InnerJoinUnion<CategoryModel>((a, b) => a.CategoryId == b.Id).FirstOrDefaultUnion<CategoryModel>();
		}

		[Fact]
		public void Union()
		{
			var result1 = Context.Select<ProductModel>().Where(a => a.Price > 10).Union<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
			var result2 = Context.Select<ProductModel>().Where(a => a.Price > 10).UnionAll<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
			var result3 = Context.Select<ProductModel>().Where(a => a.Price > 10).Union("select *from \"Product\" where \"Price\" > 20").ToList();
			var result4 = Context.Select<ProductModel>().Where(a => a.Price > 10).UnionAll("select *from \"Product\" where \"Price\" > 20").ToList();
			var result5 = Context.Select<ProductModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Price > 10)
				.Union<ProductModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Price > 20)).ToList<(string, string)>();
		}

		[Fact(Skip = "Oracle暂不支持数组")]
		public void WhereArrayEqual()
		{
		}

		[Fact]
		public void WhereCoalesce()
		{
			var result = Context.Select<UniPkTestModel>().Where(a => (a.Name ?? "Sam") == "Sam").FirstOrDefault();
			if (result != null)
				Assert.True(result.Name == null || result.Name == "Sam");
		}

		[Fact]
		public void WhereCollectionContains()
		{
			var ids = new[] { "13eacc8bce6e6000", "13eacc8bce6e6001" };
			var testAny = Context.Select<ProductModel>(a => ids.Contains(a.Id)).ToString();
			var result = Context.Select<ProductModel>(a => ids.Contains(a.Id)).FirstOrDefault();
			if (result is not null)
				Assert.Contains(result.Id, ids);
		}

		[Fact(Skip = "Oracle暂不支持数组")]
		public void WhereDbArrayFieldIndexParameter()
		{
		}

		[Fact(Skip = "Oracle暂不支持数组")]
		public void WhereDbArrayFieldLength()
		{
		}

		[Fact, Description("Oracle强制转换只支持长度2000, CAST([column] AS VARCHAR2(2000))")]
		public void WhereDbFieldToString()
		{
			var result = Context.Select<ProductModel>().Where(a => a.Price.ToString() == "300.55").FirstOrDefault();
		}

		[Fact]
		public void WhereExists()
		{
			var result1 = Context.Select<ProductModel>().WhereExists<CategoryModel>(binder => binder.Field(b => b.Id)).FirstOrDefault();
			var result2 = Context.Select<ProductModel>().WhereExists<CategoryModel>(binder => binder.Field(b => b.Id).Where("b.\"Id\" = a.\"CategoryId\"")).FirstOrDefault();
		}

		[Fact]
		public void WhereIn()
		{
			var result = Context.Select<ProductModel>()
				.WhereIn<CategoryModel>(a => a.CategoryId, binder => binder.Field(b => b.Id)).FirstOrDefault();
		}

		[Fact]
		public void WhereOperationExpression()
		{
			var result = Context.Select<ProductModel>().Where(a => a.Stock + 25 < 200).FirstOrDefault();
		}

		[Fact]
		public void WherePk()
		{
			var category = new CategoryModel { Id = "13eacc8bb82e6000", Name = "数码" };
			var result = Context.Select<CategoryModel>().Where(category).FirstOrDefault();
		}

		[Fact]
		public void WhereStringLike()
		{
			var result1 = Context.Select<UniPkTestModel>().Where(a => a.Name.Contains("Sam")).FirstOrDefault();
			var result2 = Context.Select<UniPkTestModel>().Where(a => a.Name.Contains("Sam", StringComparison.Ordinal)).FirstOrDefault();
			var result3 = Context.Select<UniPkTestModel>().Where(a => a.Name.StartsWith("Sam")).FirstOrDefault();
			var result4 = Context.Select<UniPkTestModel>().Where(a => a.Name.StartsWith("Sam", StringComparison.Ordinal)).FirstOrDefault();
			var result5 = Context.Select<UniPkTestModel>().Where(a => a.Name.EndsWith("Sam")).FirstOrDefault();
			var result6 = Context.Select<UniPkTestModel>().Where(a => a.Name.EndsWith("Sam", StringComparison.Ordinal)).FirstOrDefault();


			var result7 = Context.Select<UniPkTestModel>().Where(a => "Sam".Contains(a.Name)).FirstOrDefault();
			var result8 = Context.Select<UniPkTestModel>().Where(a => "Sam".Contains(a.Name, StringComparison.Ordinal)).FirstOrDefault();
			var result9 = Context.Select<UniPkTestModel>().Where(a => "Sam".StartsWith(a.Name)).FirstOrDefault();
			var result10 = Context.Select<UniPkTestModel>().Where(a => "Sam".StartsWith(a.Name, StringComparison.Ordinal)).FirstOrDefault();
			var result11 = Context.Select<UniPkTestModel>().Where(a => "Sam".EndsWith(a.Name)).FirstOrDefault();
			var result12 = Context.Select<UniPkTestModel>().Where(a => "Sam".EndsWith(a.Name, StringComparison.Ordinal)).FirstOrDefault();


			var result13 = Context.Select<UniPkTestModel>().Where(a => !"Sam".Contains(a.Name)).FirstOrDefault();
			var result14 = Context.Select<UniPkTestModel>().Where(a => !a.Name.Contains("Sam")).FirstOrDefault();
		}
	}
}
