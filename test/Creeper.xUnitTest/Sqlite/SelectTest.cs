using System;
using System.Data.Common;
using System.Data.SQLite;
using Xunit;
using Creeper.Driver;
using Creeper.SqlBuilder;
using Creeper.Sqlite.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using System.Collections;
using Creeper.xUnitTest.Extensions;
using System.Linq;
using System.ComponentModel;
using Creeper.Generic;

namespace Creeper.xUnitTest.Sqlite
{
	public class SelectTest : BaseTest, ISelectTest
	{

		[Fact]
		public void Avg()
		{
			var result1 = Context.Select<ProductModel>().Avg(a => a.Price);
			var result2 = Context.Select<ProductModel>().Avg(a => a.Price ?? 0, 0);
		}

		[Fact]
		public void Count()
		{
			var result1 = Context.Select<ProductModel>().Count();
			Assert.True(result1 >= 0);
		}

		[Fact]
		public void CountDistinct()
		{
			var count = Context.Select<ProductModel>().CountDistinct(a => a.Name);
			Assert.True(count >= 0);
		}

		[Fact]
		public void Distinct()
		{
			//var names = Context.Select<ProductModel>().Distinst().ToList(a => a.Name);
			//Assert.True(names.Count >= 0);
		}

		[Fact, Description("SQLite不支持EXCEPT ALL语法")]
		public void Except()
		{
			var result1 = Context.Select<ProductModel>().Where(a => a.Price > 10).Except<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
			var result3 = Context.Select<ProductModel>().Where(a => a.Price > 10).Except("select *from \"product\" where \"price\" > 20").ToList();
			var result5 = Context.Select<ProductModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Price > 10).Except<ProductModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Price > 20)).ToList<(int, string)>();
			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result2 = Context.Select<ProductModel>().Where(a => a.Price > 10).ExceptAll<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
				var result4 = Context.Select<ProductModel>().Where(a => a.Price > 10).ExceptAll("select *from \"product\" where \"price\" > 20").ToList();
			});
			Assert.IsType<System.Data.SQLite.SQLiteException>(exception.InnerException);
		}

		[Fact]
		public void FirstOrDefault()
		{
			var result1 = Context.Select<ProductModel>().FirstOrDefault();
			var result2 = Context.Select<ProductModel>().FirstOrDefault<(int, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<ProductModel>().FirstOrDefault<(int, string)>("a.\"id\",a.\"name\"");
			var result4 = Context.Select<ProductModel>().FirstOrDefault(a => a.Id);
			var result5 = Context.Select<ProductModel>(a => a.Id == -1).FirstOrDefault<int?>(a => a.Id);
			var result6 = Context.Select<ProductModel>().FirstOrDefault<Hashtable>("a.\"id\",a.\"name\"");
			Assert.Null(result5);
		}

		[Fact]
		public void Frist()
		{
			Assert.Throws<CreeperFirstNotFoundException>(() =>
			{
				var result1 = Context.Select<ProductModel>(a => a.Id == -1).First();
			});
		}

		[Fact]
		public void GroupBy()
		{
			var result = Context.Select<ProductModel>().GroupBy(a => a.Category_id).ToList<(string, long)>("a.\"category_id\",COUNT(1)");
		}

		[Fact]
		public void Having()
		{
			var result = Context.Select<ProductModel>().GroupBy(a => a.Category_id).Having("COUNT(1) >= 10").ToList(a => a.Category_id);
		}

		[Fact, Description("SQLite不支持INTERSECT ALL语法")]
		public void Intersect()
		{
			var result1 = Context.Select<ProductModel>().Where(a => a.Price > 10).Intersect<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
			var result3 = Context.Select<ProductModel>().Where(a => a.Price > 10).Intersect("select *from \"product\" where \"price\" > 20").ToList();
			var result5 = Context.Select<ProductModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Price > 10).Intersect<ProductModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Price > 20)).ToList<(int, string)>();

			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result2 = Context.Select<ProductModel>().Where(a => a.Price > 10).IntersectAll<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
				var result4 = Context.Select<ProductModel>().Where(a => a.Price > 10).IntersectAll("select *from \"product\" where \"price\" > 20").ToList();
			});
			Assert.IsType<System.Data.SQLite.SQLiteException>(exception.InnerException);
		}

		[Fact,]
		public void Join()
		{
			var result1 = Context.Select<ProductModel>().InnerJoin<CategoryModel>((a, b) => a.Category_id == b.Id).FirstOrDefault();
			var result2 = Context.Select<ProductModel>().LeftOuterJoin<CategoryModel>((a, b) => a.Category_id == b.Id).FirstOrDefault();
			var result3 = Context.Select<ProductModel>().Join<CategoryModel>(JoinType.CROSS_JOIN, (a, b) => a.Category_id == b.Id).FirstOrDefault();

			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result4 = Context.Select<ProductModel>().LeftJoin<CategoryModel>((a, b) => a.Category_id == b.Id).FirstOrDefault();
				var result5 = Context.Select<ProductModel>().RightJoin<CategoryModel>((a, b) => a.Category_id == b.Id).FirstOrDefault();
				var result6 = Context.Select<ProductModel>().RightOuterJoin<CategoryModel>((a, b) => a.Category_id == b.Id).FirstOrDefault();
			});
			Assert.IsType<SQLiteException>(exception.InnerException);
		}
		[Fact]
		public void Limit()
		{
			var result1 = Context.Select<ProductModel>().Take(10).ToList();
			var result2 = Context.Select<ProductModel>().OrderByDescending(a => a.Price).Take(10).ToList();
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

		[Theory]
		[InlineData(1, 5)]
		[InlineData(2, 5)]
		public void Page(int index, int size)
		{

			var result1 = Context.Select<ProductModel>().Page(index, size).ToList();
			var result2 = Context.Select<ProductModel>().OrderByDescending(a => a.Price).Page(index, size).ToList();
		}

		[Fact]
		public void Scalar()
		{
			var result1 = Context.Select<ProductModel>(a => a.Id == 1).ToScalar("\"id\"");
			var result2 = Context.Select<ProductModel>(a => a.Id == 1).ToScalar<int>("\"id\"");
		}

		[Fact]
		public void SelectByDbCache()
		{
			var result = Context.Select<ProductModel>(a => a.Id == 1).ByCache(TimeSpan.FromMinutes(1)).FirstOrDefault();
		}

		[Fact, Description("SQLite分页必须传入页码, 不允许单独跳过前n条数据(OFFSET必须在LIMIT后面使用)")]
		public void Skip()
		{
			var exception = Assert.Throws<CreeperException>(() =>
			{
				var result1 = Context.Select<ProductModel>().Skip(10).ToString();
				var result2 = Context.Select<ProductModel>().OrderByDescending(a => a.Price).Skip(10).ToString();
			});
		}

		[Fact]
		public void Sum()
		{
			var result1 = Context.Select<ProductModel>().Min(a => a.Price);
			var result2 = Context.Select<ProductModel>().Min(a => a.Price ?? 0, 0);
		}

		[Fact]
		public void ToList()
		{
			var result1 = Context.Select<ProductModel>().ToList();
			var result2 = Context.Select<ProductModel>().ToList<(int, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<ProductModel>().ToList<(int, string)>("a.[Id],a.[Name]");
			var result4 = Context.Select<ProductModel>().ToList(a => a.Id);
			var result5 = Context.Select<ProductModel>(a => a.Id == -1).ToList<int?>(a => a.Id);
			var result6 = Context.Select<ProductModel>().ToList<Hashtable>("a.[Id],a.[Name]");
			Assert.Empty(result5);
		}

		[Fact]
		public void ToPipe()
		{
			object[] obj = Context.ExecuteReaderPipe(new ISqlBuilder[] {
				Context.Select<ProductModel>().ToListPipe(a => a.Id),
				Context.Select<ProductModel>().FirstOrDefaultPipe<(int, string)>(a => new { a.Id, a.Name }),
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
			var result = Context.Select<ProductModel>().InnerJoinUnion<CategoryModel>((a, b) => a.Category_id == b.Id).FirstOrDefaultUnion<CategoryModel>();
		}

		[Fact]
		public void Union()
		{
			var result1 = Context.Select<ProductModel>().Where(a => a.Price > 10).Union<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
			var result2 = Context.Select<ProductModel>().Where(a => a.Price > 10).UnionAll<ProductModel>(binder => binder.Where(a => a.Price > 20)).ToList();
			var result3 = Context.Select<ProductModel>().Where(a => a.Price > 10).Union("select *from \"product\" where \"price\" > 20").ToList();
			var result4 = Context.Select<ProductModel>().Where(a => a.Price > 10).UnionAll("select *from \"product\" where \"price\" > 20").ToList();
			var result5 = Context.Select<ProductModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Price > 10).Union<ProductModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Price > 20)).ToList<(int, string)>();
		}

		[Fact(Skip = "SQLite暂不支持数组")]
		public void WhereArrayEqual()
		{
		}

		[Fact]
		public void WhereCoalesce()
		{
			var result = Context.Select<ProductModel>().Where(a => (a.Category_id ?? 0) == 0).FirstOrDefault();
			if (result != null)
				Assert.True(result.Category_id == null || result.Category_id == 0);
		}

		[Fact]
		public void WhereCollectionContains()
		{
			var ids = new long[] { 1, 2, 3 };
			var testAny = Context.Select<ProductModel>(a => ids.Contains(a.Id)).ToString();
			var result = Context.Select<ProductModel>(a => ids.Contains(a.Id)).FirstOrDefault();
			if (result is not null)
				Assert.Contains(result.Id, ids);
		}

		[Fact(Skip = "SQLite暂不支持数组")]
		public void WhereDbArrayFieldIndexParameter()
		{
		}

		[Fact(Skip = "SQLite暂不支持数组")]
		public void WhereDbArrayFieldLength()
		{
		}

		[Fact]
		public void WhereDbFieldToString()
		{
			var result = Context.Select<ProductModel>().Where(a => a.Category_id.ToString() == "1").FirstOrDefault();
		}

		[Fact]
		public void WhereExists()
		{
			var result1 = Context.Select<ProductModel>().WhereExists<ProductModel>(binder => binder.Field(b => b.Id)).FirstOrDefault();
			var result2 = Context.Select<ProductModel>().WhereExists<ProductModel>(binder => binder.Field(b => b.Id).Where("b.\"id\" = a.\"category_id\"")).FirstOrDefault();
		}

		[Fact]
		public void WhereIn()
		{
			var result = Context.Select<ProductModel>()
				   .WhereIn<CategoryModel>(a => a.Category_id, binder => binder.Field(b => b.Id)).FirstOrDefault();
		}

		[Fact]
		public void WhereOperationExpression()
		{
			var result = Context.Select<ProductModel>().Where(a => a.Price + 10 < 50).FirstOrDefault();
		}

		[Fact]
		public void WherePk()
		{
			var product = new ProductModel()
			{
				Id = 1,
				Name = "测试"
			};
			var result = Context.Select<ProductModel>().Where(product).FirstOrDefault();
		}

		[Fact]
		public void WhereStringLike()
		{
			var result1 = Context.Select<ProductModel>().Where(a => a.Name.Contains("Apple")).FirstOrDefault();
			var result2 = Context.Select<ProductModel>().Where(a => a.Name.Contains("Apple", StringComparison.Ordinal)).FirstOrDefault();
			var result3 = Context.Select<ProductModel>().Where(a => a.Name.StartsWith("Apple")).FirstOrDefault();
			var result4 = Context.Select<ProductModel>().Where(a => a.Name.StartsWith("Apple", StringComparison.Ordinal)).FirstOrDefault();
			var result5 = Context.Select<ProductModel>().Where(a => a.Name.EndsWith("Apple")).FirstOrDefault();
			var result6 = Context.Select<ProductModel>().Where(a => a.Name.EndsWith("Apple", StringComparison.Ordinal)).FirstOrDefault();


			var result7 = Context.Select<ProductModel>().Where(a => "Apple".Contains(a.Name)).FirstOrDefault();
			var result8 = Context.Select<ProductModel>().Where(a => "Apple".Contains(a.Name, StringComparison.Ordinal)).FirstOrDefault();
			var result9 = Context.Select<ProductModel>().Where(a => "Apple".StartsWith(a.Name)).FirstOrDefault();
			var result10 = Context.Select<ProductModel>().Where(a => "Apple".StartsWith(a.Name, StringComparison.Ordinal)).FirstOrDefault();
			var result11 = Context.Select<ProductModel>().Where(a => "Apple".EndsWith(a.Name)).FirstOrDefault();
			var result12 = Context.Select<ProductModel>().Where(a => "Apple".EndsWith(a.Name, StringComparison.Ordinal)).FirstOrDefault();


			var result13 = Context.Select<ProductModel>().Where(a => !"Apple".Contains(a.Name)).FirstOrDefault();
			var result14 = Context.Select<ProductModel>().Where(a => !a.Name.Contains("Apple")).FirstOrDefault();
		}
	}
}
