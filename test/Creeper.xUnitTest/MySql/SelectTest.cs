using Creeper.Driver;
using Creeper.SqlBuilder;
using MySql.Data.MySqlClient;
using Xunit;
using Creeper.MySql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using System.Collections;
using System;
using Creeper.xUnitTest.Extensions;
using System.Linq;
using System.ComponentModel;
using Creeper.Generic;

namespace Creeper.xUnitTest.MySql
{
	public class SelectTest : BaseTest, ISelectTest
	{
		[Fact]
		public void Avg()
		{
			var result1 = Context.Select<PeopleModel>().Avg(a => a.Age);
			var result2 = Context.Select<PeopleModel>().Avg(a => a.Age ?? 0, 0);
		}

		[Fact]
		public void Count()
		{
			var result1 = Context.Select<StudentModel>().Count();
			Assert.True(result1 >= 0);
		}

		[Fact]
		public void CountDistinct()
		{
			var count = Context.Select<PeopleModel>().CountDistinct(a => a.Name);
			Assert.True(count > 0);
		}

		[Fact]
		public void Distinct()
		{
			//var names = Context.Select<PeopleModel>().Distinst().ToList(a => a.Name);
			//Assert.True(names.Count > 0);
		}

		[Fact, Description("MySqle不支持EXCEPT语法")]
		public void Except()
		{
			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result1 = Context.Select<PeopleModel>().Where(a => a.Age > 10).Except<PeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
				var result2 = Context.Select<PeopleModel>().Where(a => a.Age > 10).ExceptAll<PeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
				var result3 = Context.Select<PeopleModel>().Where(a => a.Age > 10).Except("select *from `people` where `age` > 20").ToList();
				var result4 = Context.Select<PeopleModel>().Where(a => a.Age > 10).ExceptAll("select *from `people` where `age` > 20").ToList();
				var result5 = Context.Select<PeopleModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Age > 10).Except<PeopleModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Age > 20)).ToList<(int, string)>();
			});
			Assert.IsType<MySqlException>(exception.InnerException);

		}

		[Fact]
		public void FirstOrDefault()
		{
			var result1 = Context.Select<PeopleModel>().FirstOrDefault();
			var result2 = Context.Select<PeopleModel>().FirstOrDefault<(int, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<PeopleModel>().FirstOrDefault<(int, string)>("a.`id`,a.`name`");
			var result4 = Context.Select<PeopleModel>().FirstOrDefault(a => a.Id);
			var result5 = Context.Select<PeopleModel>(a => a.Id == -1).FirstOrDefault<int?>(a => a.Id);
			var result6 = Context.Select<PeopleModel>().FirstOrDefault<Hashtable>("a.`id`,a.`name`");
			Assert.Null(result5);
		}

		[Fact]
		public void Frist()
		{
			Assert.Throws<CreeperFirstNotFoundException>(() =>
			{
				var result1 = Context.Select<StudentModel>(a => a.Id == -1).First();
			});
		}

		[Fact]
		public void GroupBy()
		{
			var result = Context.Select<PeopleModel>().GroupBy(a => a.Age).ToList<(int, long)>("a.`age`,COUNT(1)");
		}

		[Fact]
		public void Having()
		{
			var result = Context.Select<PeopleModel>().GroupBy(a => a.Age).Having("COUNT(1) >= 10").ToList(a => a.Age);
		}

		[Fact, Description("MySqle不支持INTERSECT语法")]
		public void Intersect()
		{
			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result1 = Context.Select<PeopleModel>().Where(a => a.Age > 10).Intersect<PeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
				var result2 = Context.Select<PeopleModel>().Where(a => a.Age > 10).IntersectAll<PeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
				var result3 = Context.Select<PeopleModel>().Where(a => a.Age > 10).Intersect("select *from `people` where `age` > 20").ToList();
				var result4 = Context.Select<PeopleModel>().Where(a => a.Age > 10).IntersectAll("select *from `people` where `age` > 20").ToList();
				var result5 = Context.Select<PeopleModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Age > 10).Intersect<PeopleModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Age > 20)).ToList<(int, string)>();
			});
			Assert.IsType<MySqlException>(exception.InnerException);

		}

		[Fact]
		public void Join()
		{
			var result1 = Context.Select<StudentModel>().InnerJoin<PeopleModel>((a, b) => a.People_id == b.Id).FirstOrDefault();
			var result2 = Context.Select<StudentModel>().LeftJoin<PeopleModel>((a, b) => a.People_id == b.Id).FirstOrDefault();
			var result3 = Context.Select<StudentModel>().RightJoin<PeopleModel>((a, b) => a.People_id == b.Id).FirstOrDefault();
			var result4 = Context.Select<StudentModel>().LeftOuterJoin<PeopleModel>((a, b) => a.People_id == b.Id).FirstOrDefault();
			var result5 = Context.Select<StudentModel>().RightOuterJoin<PeopleModel>((a, b) => a.People_id == b.Id).FirstOrDefault();
		}

		[Fact]
		public void Limit()
		{
			var result1 = Context.Select<PeopleModel>().Take(10).ToList();
			var result2 = Context.Select<PeopleModel>().OrderByDescending(a => a.Age).Take(10).ToList();
		}

		[Fact]
		public void Max()
		{
			var result1 = Context.Select<PeopleModel>().Max(a => a.Age);
			var result2 = Context.Select<PeopleModel>().Max(a => a.Age ?? 0, 0);
		}

		[Fact]
		public void Min()
		{
			var result1 = Context.Select<PeopleModel>().Min(a => a.Age);
			var result2 = Context.Select<PeopleModel>().Min(a => a.Age ?? 0, 0);
		}

		[Fact]
		public void OrderBy()
		{
			var result1 = Context.Select<PeopleModel>().OrderByDescending(a => a.Name).FirstOrDefault();
			var result2 = Context.Select<PeopleModel>().OrderByDescendingNullsLast(a => a.Name).FirstOrDefault();
			var result3 = Context.Select<PeopleModel>().OrderBy(a => a.Name).FirstOrDefault();
			var result4 = Context.Select<PeopleModel>().OrderByNullsLast(a => a.Name).FirstOrDefault();
		}

		[Theory]
		[InlineData(1, 5)]
		[InlineData(2, 5)]
		public void Page(int index, int size)
		{
			var result1 = Context.Select<PeopleModel>().Page(index, size).ToList();
			var result2 = Context.Select<PeopleModel>().OrderByDescending(a => a.Name).Page(index, size).ToList();
		}

		[Fact]
		public void Scalar()
		{
			var result1 = Context.Select<StudentModel>(a => a.Id == 1).ToScalar("`id`");
			var result2 = Context.Select<StudentModel>(a => a.Id == 1).ToScalar<int>("`id`");
		}

		[Fact]
		public void SelectByDbCache()
		{
			var result = Context.Select<StudentModel>(a => a.Id == 1).ByCache(TimeSpan.FromMinutes(1)).FirstOrDefault();
		}

		[Fact, Description("MySql分页必须传入页码, 不允许单独跳过前n条数据(OFFSET必须在LIMIT后面使用)")]
		public void Skip()
		{
			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result1 = Context.Select<PeopleModel>().Skip(10).ToList();
				var result2 = Context.Select<PeopleModel>().OrderByDescending(a => a.Name).Skip(10).ToList();
			});
			Assert.IsType<MySqlException>(exception.InnerException);
		}

		[Fact]
		public void Sum()
		{
			var result1 = Context.Select<PeopleModel>().Sum(a => a.Age);
			var result2 = Context.Select<PeopleModel>().Sum(a => a.Age ?? 0, 0);
		}

		[Fact]
		public void ToList()
		{
			var result1 = Context.Select<PeopleModel>().ToList();
			var result2 = Context.Select<PeopleModel>().ToList<(int, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<PeopleModel>().ToList<(int, string)>("a.`id`,a.`name`");
			var result4 = Context.Select<PeopleModel>().ToList(a => a.Id);
			var result5 = Context.Select<PeopleModel>(a => a.Id == -1).ToList<int?>(a => a.Id);
			var result6 = Context.Select<PeopleModel>().ToList<Hashtable>("a.`id`,a.`name`");
			Assert.Empty(result5);
		}

		[Fact]
		public void ToPipe()
		{
			object[] obj = Context.ExecuteReaderPipe(new ISqlBuilder[] {
				Context.Select<PeopleModel>().ToListPipe(a => a.Id),
				Context.Select<PeopleModel>().FirstOrDefaultPipe<(int, string)>(a => new { a.Id, a.Name }),
				Context.Select<PeopleModel>().FirstOrDefaultPipe(),
				Context.Select<PeopleModel>().Take(2).ToListPipe()

			});
			var info = obj[0].ToObjectArray().OfType<Guid>();
			var info1 = ((int, string))obj[1];
			var info2 = (PeopleModel)obj[2];
			var info3 = obj[3].ToObjectArray().OfType<PeopleModel>().ToList();
		}

		[Fact]
		public void ToUnion()
		{
			var (stu, people) = Context.Select<StudentModel>().InnerJoinUnion<PeopleModel>((a, b) => a.People_id == b.Id).FirstOrDefaultUnion<PeopleModel>();
			if (stu != null && people != null)
				Assert.Equal(stu.People_id, people.Id);
		}

		[Fact]
		public void Union()
		{
			var result1 = Context.Select<PeopleModel>().Where(a => a.Age > 10).Union<PeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
			var result2 = Context.Select<PeopleModel>().Where(a => a.Age > 10).UnionAll<PeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
			var result3 = Context.Select<PeopleModel>().Where(a => a.Age > 10).Union("select *from `people` where `age` > 20").ToList();
			var result4 = Context.Select<PeopleModel>().Where(a => a.Age > 10).UnionAll("select *from `people` where `age` > 20").ToList();
			var result5 = Context.Select<PeopleModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Age > 10).Union<PeopleModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Age > 20)).ToList<(int, string)>();
		}

		[Fact(Skip = "MySql暂不支持数组")]
		public void WhereArrayEqual()
		{
		}

		[Fact]
		public void WhereCoalesce()
		{
			var result = Context.Select<PeopleModel>().Where(a => (a.Age ?? 0) == 0).FirstOrDefault();
			if (result != null)
				Assert.True(result.Age == null || result.Age == 0);
		}

		[Fact]
		public void WhereCollectionContains()
		{
			var ids = new long[] { 1, 2, 3 };
			var testAny = Context.Select<PeopleModel>(a => ids.Contains(a.Id)).ToString();
			var result = Context.Select<PeopleModel>(a => ids.Contains(a.Id)).FirstOrDefault();
			if (result is not null)
				Assert.Contains(result.Id, ids);
		}

		[Fact(Skip = "MySql暂不支持数组")]
		public void WhereDbArrayFieldIndexParameter()
		{
		}

		[Fact(Skip = "MySql暂不支持数组")]
		public void WhereDbArrayFieldLength()
		{
		}

		[Fact]
		public void WhereDbFieldToString()
		{
			var result = Context.Select<PeopleModel>().Where(a => a.Id.ToString() == "1").FirstOrDefault();
		}

		[Fact]
		public void WhereExists()
		{
			var result1 = Context.Select<PeopleModel>().WhereExists<StudentModel>(binder => binder.Field(b => b.Id)).FirstOrDefault();
			var result2 = Context.Select<PeopleModel>().WhereExists<StudentModel>(binder => binder.Field(b => b.Id).Where("b.`people_id` = a.`id`")).FirstOrDefault();
		}

		[Fact]
		public void WhereIn()
		{
			var result = Context.Select<PeopleModel>().WhereIn<StudentModel>(a => a.Id, binder => binder.Field(b => b.People_id)).FirstOrDefault();
		}

		[Fact]
		public void WhereOperationExpression()
		{
			var result = Context.Select<PeopleModel>().Where(a => a.Age + 5 < 20).FirstOrDefault();
		}

		[Fact]
		public void WherePk()
		{
			var people = new PeopleModel()
			{
				Id = 1,
				Name = "测试"
			};
			var result = Context.Select<PeopleModel>().Where(people).FirstOrDefault();
		}

		[Fact]
		public void WhereStringLike()
		{
			var result1 = Context.Select<PeopleModel>().Where(a => a.Name.Contains("小明")).FirstOrDefault();
			var result2 = Context.Select<PeopleModel>().Where(a => a.Name.Contains("小明", StringComparison.Ordinal)).FirstOrDefault();
			var result3 = Context.Select<PeopleModel>().Where(a => a.Name.StartsWith("小明")).FirstOrDefault();
			var result4 = Context.Select<PeopleModel>().Where(a => a.Name.StartsWith("小明", StringComparison.Ordinal)).FirstOrDefault();
			var result5 = Context.Select<PeopleModel>().Where(a => a.Name.EndsWith("小明")).FirstOrDefault();
			var result6 = Context.Select<PeopleModel>().Where(a => a.Name.EndsWith("小明", StringComparison.Ordinal)).FirstOrDefault();


			var result7 = Context.Select<PeopleModel>().Where(a => "小明".Contains(a.Name)).FirstOrDefault();
			var result8 = Context.Select<PeopleModel>().Where(a => "小明".Contains(a.Name, StringComparison.Ordinal)).FirstOrDefault();
			var result9 = Context.Select<PeopleModel>().Where(a => "小明".StartsWith(a.Name)).FirstOrDefault();
			var result10 = Context.Select<PeopleModel>().Where(a => "小明".StartsWith(a.Name, StringComparison.Ordinal)).FirstOrDefault();
			var result11 = Context.Select<PeopleModel>().Where(a => "小明".EndsWith(a.Name)).FirstOrDefault();
			var result12 = Context.Select<PeopleModel>().Where(a => "小明".EndsWith(a.Name, StringComparison.Ordinal)).FirstOrDefault();


			var result13 = Context.Select<PeopleModel>().Where(a => !"小明".Contains(a.Name)).FirstOrDefault();
			var result14 = Context.Select<PeopleModel>().Where(a => !a.Name.Contains("小明")).FirstOrDefault();
		}
	}
}
