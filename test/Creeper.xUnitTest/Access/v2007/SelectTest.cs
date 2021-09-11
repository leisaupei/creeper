using Xunit;
using System.Data.Common;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using Creeper.xUnitTest.Contracts;
using System;
using Creeper.Driver;
using Creeper.Access2007.Test.Entity.Model;
using System.ComponentModel;
using System.Collections;
using Creeper.SqlBuilder;
using Creeper.xUnitTest.Extensions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Creeper.xUnitTest.Access.v2007
{
	public class SelectTest : BaseTest, ISelectTest
	{
		[Theory]
		[InlineData(1, 10)]
		[InlineData(2, 10)]
		[Description("Access分页时必须传入排序字段, 第一页可不传, 也就是说存在传统意义的offset时会判断排序字段是否存在")]
		public void Page(int index, int size)
		{
			//	var result1 = Context.Select<UniPkTestModel>().Page(index, size).ToList();
			var result2 = Context.Select<UniPkTestModel>().OrderByDescending(a => a.Id).Page(index, size).ToList();
		}

		[Fact]
		public void Limit()
		{
			var result1 = Context.Select<UniPkTestModel>().Take(10).ToList();
			var result2 = Context.Select<UniPkTestModel>().OrderByDescending(a => a.Id).Take(10).ToList();
		}

		[Fact(Skip = "鉴于Access分页方式比较局限, 分页必须传入页码, 不允许单独跳过前n条数据")]
		public void Skip()
		{
			Assert.Throws<CreeperException>(() =>
			{
				var result2 = Context.Select<UniPkTestModel>().OrderByDescending(a => a.Id).Skip(10).ToList();
			});
		}

		[Fact]
		public void Avg()
		{
			//using var conn = new OleDbConnection(MainConnectionString);
			//conn.Open();
			//var cmd = conn.CreateCommand();
			//cmd.CommandText = "SELECT  TOP 1 IIf(AVG(a.[price]) Is Null,0,AVG(a.[price])) FROM [Product] AS a";
			//cmd.CommandType = System.Data.CommandType.Text;
			//var result = cmd.ExecuteScalar();
			//var dou = Convert.ToSingle(result);
			//var type = result.GetType();
			var result1 = Context.Select<ProductModel>().Avg(a => a.Price);
			var result2 = Context.Select<ProductModel>().Avg(a => a.Price ?? 0, 0);
		}

		[Fact]
		public void Count()
		{
			var result1 = Context.Select<ProductModel>().Count();
		}

		[Fact]
		public void Except()
		{
			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result1 = Context.Select<ProductModel>().Where(a => a.Price > 10).Except<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
				var result2 = Context.Select<ProductModel>().Where(a => a.Price > 10).ExceptAll<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
				var result3 = Context.Select<ProductModel>().Where(a => a.Price > 10).Except("select *from [Product] where [Price] > 20").ToList();
				var result4 = Context.Select<ProductModel>().Where(a => a.Price > 10).ExceptAll("select *from [Product] where [Price] > 20").ToList();
				var result5 = Context.Select<ProductModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Price > 10).Except<ProductModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Price > 20)).ToList<(int, string)>();
			});
#pragma warning disable CA1416 // 验证平台兼容性
			Assert.IsType<OleDbException>(exception.InnerException);
#pragma warning restore CA1416 // 验证平台兼容性
		}

		[Fact]
		public void FirstOrDefault()
		{
			var result1 = Context.Select<ProductModel>().FirstOrDefault();
			var result2 = Context.Select<ProductModel>().FirstOrDefault<(int, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<ProductModel>().FirstOrDefault<(int, string)>("a.[Id],a.[Name]");
			var result4 = Context.Select<ProductModel>().FirstOrDefault(a => a.Id);
			var result5 = Context.Select<ProductModel>(a => a.Id == 0).FirstOrDefault<int?>(a => a.Id);
			var result6 = Context.Select<ProductModel>().FirstOrDefault<Hashtable>("a.[Id],a.[Name]");
			Assert.Null(result5);
		}

		[Fact]
		public void Frist()
		{
			Assert.Throws<CreeperFirstNotFoundException>(() =>
			{
				var result1 = Context.Select<ProductModel>(a => a.Id == 0).First();
			});
		}

		[Fact]
		public void GroupBy()
		{
			var result = Context.Select<ProductModel>().GroupBy(a => a.CategoryId).ToList<(int, long)>("a.[CategoryId],COUNT(1)");
		}

		[Fact]
		public void Having()
		{
			var result = Context.Select<ProductModel>().GroupBy(a => a.CategoryId).Having("COUNT(1) >= 1").ToList(a => a.CategoryId);
		}

		[Fact]
		public void Intersect()
		{
			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result1 = Context.Select<ProductModel>().Where(a => a.Price > 10).Intersect<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
				var result2 = Context.Select<ProductModel>().Where(a => a.Price > 10).IntersectAll<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
				var result3 = Context.Select<ProductModel>().Where(a => a.Price > 10).Intersect("select *from [Product] where [Price] > 20").ToList();
				var result4 = Context.Select<ProductModel>().Where(a => a.Price > 10).IntersectAll("select *from [Product] where [Price] > 20").ToList();
				var result5 = Context.Select<ProductModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Price > 10)
					.Intersect<ProductModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Price > 20)).ToList<(int, string)>();
			});
#pragma warning disable CA1416 // 验证平台兼容性
			Assert.IsType<OleDbException>(exception.InnerException);
#pragma warning restore CA1416 // 验证平台兼容性
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
		public void Max()
		{
			var result1 = Context.Select<ProductModel>().Max(a => a.Price);
			var result2 = Context.Select<ProductModel>().Max(a => a.Price ?? 0, 0);
		}

		[Fact]
		public void Min()
		{
			var result1 = Context.Select<ProductModel>().Min(a => a.Price);
			var result2 = Context.Select<ProductModel>().Min(a => a.Price ?? 0, 0);
		}

		[Fact]
		public void OrderBy()
		{
			var result1 = Context.Select<ProductModel>().OrderByDescending(a => a.Name).FirstOrDefault();
			var result2 = Context.Select<ProductModel>().OrderByDescendingNullsLast(a => a.Name).FirstOrDefault();
			var result3 = Context.Select<ProductModel>().OrderBy(a => a.Name).FirstOrDefault();
			var result4 = Context.Select<ProductModel>().OrderByNullsLast(a => a.Name).FirstOrDefault();
		}

		[Fact]
		public void Scalar()
		{
			var result1 = Context.Select<ProductModel>(a => a.Id == 1).ToScalar("[Id]");
			var result2 = Context.Select<ProductModel>(a => a.Id == 1).ToScalar<int>("[Id]");
		}

		[Fact]
		public void SelectByDbCache()
		{
			var result = Context.Select<ProductModel>(a => a.Id == 1).ByCache(TimeSpan.FromMinutes(1)).FirstOrDefault();
		}

		[Fact]
		public void Sum()
		{
			var result1 = Context.Select<ProductModel>().Sum(a => a.Price);
			var result2 = Context.Select<ProductModel>().Sum(a => a.Price ?? 0, 0);
		}

		[Fact]
		public void ToList()
		{
			var result1 = Context.Select<ProductModel>().ToList();
			var result2 = Context.Select<ProductModel>().ToList<(string, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<ProductModel>().ToList<(string, string)>("a.[Id],a.[Name]");
			var result4 = Context.Select<ProductModel>().ToList(a => a.Id);
			var result5 = Context.Select<ProductModel>(a => a.Id == 0).ToList(a => a.Id);
			var result6 = Context.Select<ProductModel>().ToList<Hashtable>("a.[Id],a.[Name]");
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
			var info = obj[0].ToObjectArray().OfType<int>();
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
			var result3 = Context.Select<ProductModel>().Where(a => a.Price > 10).Union("select *from [Product] where [Price] > 20").ToList();
			var result4 = Context.Select<ProductModel>().Where(a => a.Price > 10).UnionAll("select *from [Product] where [Price] > 20").ToList();
			var result5 = Context.Select<ProductModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Price > 10)
				.Union<ProductModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Price > 20)).ToList<(int, string)>();
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
			var ids = new[] { 1, 2, 3 };
			var testAny = Context.Select<ProductModel>(a => ids.Contains(a.Id)).ToString();
			var result = Context.Select<ProductModel>(a => ids.Contains(a.Id)).FirstOrDefault();
			if (result is not null)
				Assert.Contains(result.Id, ids);
		}

		[Fact(Skip = "Access暂不支持数组")]
		public void WhereDbArrayFieldIndexParameter()
		{
		}

		[Fact(Skip = "Access暂不支持数组")]
		public void WhereDbArrayFieldLength()
		{
		}

		[Fact, Description("Access强制转换只支持长度2000, CAST([column] AS VARCHAR2(2000))")]
		public void WhereDbFieldToString()
		{
			var result = Context.Select<ProductModel>().Where(a => a.Price.ToString() == "300.55").FirstOrDefault();
		}

		[Fact]
		public void WhereExists()
		{
			var result1 = Context.Select<ProductModel>().WhereExists<CategoryModel>(binder => binder.Field(b => b.Id)).FirstOrDefault();
			var result2 = Context.Select<ProductModel>().WhereExists<CategoryModel>(binder => binder.Field(b => b.Id).Where("b.[Id] = a.[CategoryId]")).FirstOrDefault();
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
			var category = new CategoryModel { Id = 1, Name = "数码" };
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
