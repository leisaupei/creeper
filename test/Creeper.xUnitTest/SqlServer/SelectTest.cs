using Creeper.Driver;
using Creeper.SqlBuilder;
using Creeper.SqlServer.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using Creeper.xUnitTest.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace Creeper.xUnitTest.SqlServer
{
	public class SelectTest : BaseTest, ISelectTest
	{
		[Theory]
		[InlineData(1, 5)]
		[InlineData(2, 5)]
		public void Page(int index, int size)
		{
			var result1 = Context.Select<StudentModel>().Page(index, size).ToList();
			var result2 = Context.Select<StudentModel>().OrderByDescending(a => a.CreateTime).Page(index, size).ToList();
		}

		[Fact]
		public void Limit()
		{
			var result1 = Context.Select<StudentModel>().Take(10).ToList();
			var result2 = Context.Select<StudentModel>().OrderByDescending(a => a.CreateTime).Take(10).ToList();
		}

		[Fact]
		public void Skip()
		{
			var result1 = Context.Select<StudentModel>().Skip(10).ToList();
			var result2 = Context.Select<StudentModel>().OrderByDescending(a => a.CreateTime).Skip(10).ToList();
		}

		[Fact]
		public void Avg()
		{
			var result1 = Context.Select<StudentModel>().Avg(a => a.StuNo);
			var result2 = Context.Select<TypeTestModel>().Avg(a => a.DecimalType ?? 0, 0);
		}

		[Fact]
		public void Count()
		{
			var result1 = Context.Select<StudentModel>().Count();
			Assert.True(result1 >= 0);
		}

		[Fact]
		public void FirstOrDefault()
		{
			var result1 = Context.Select<StudentModel>().FirstOrDefault();
			var result2 = Context.Select<StudentModel>().FirstOrDefault<(int, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<StudentModel>().FirstOrDefault<(int, string)>("a.[Id],a.[Name]");
			var result4 = Context.Select<StudentModel>().FirstOrDefault(a => a.Id);
			var result5 = Context.Select<StudentModel>(a => a.Id == -1).FirstOrDefault<int?>(a => a.Id);
			var result6 = Context.Select<StudentModel>().FirstOrDefault<Hashtable>("a.[Id],a.[Name]");
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
			var result = Context.Select<StudentModel>().GroupBy(a => a.Name).ToList<(string, long)>("a.[Name],COUNT(1)");
		}

		[Fact]
		public void Having()
		{
			var result = Context.Select<StudentModel>().GroupBy(a => a.Name).Having("COUNT(1) >= 10").ToList(a => a.Name);
		}

		[Fact]
		public void Max()
		{
			var result1 = Context.Select<StudentModel>().Max(a => a.StuNo);
			var result2 = Context.Select<TypeTestModel>().Max(a => a.DecimalType ?? 0, 0);
		}

		[Fact]
		public void Min()
		{
			var result1 = Context.Select<StudentModel>().Min(a => a.StuNo);
			var result2 = Context.Select<TypeTestModel>().Min(a => a.DecimalType ?? 0, 0);
		}

		[Fact]
		public void OrderBy()
		{
			var result1 = Context.Select<StudentModel>().OrderByDescending(a => a.Name).FirstOrDefault();
			var result2 = Context.Select<StudentModel>().OrderByDescendingNullsLast(a => a.Name).FirstOrDefault();
			var result3 = Context.Select<StudentModel>().OrderBy(a => a.Name).FirstOrDefault();
			var result4 = Context.Select<StudentModel>().OrderByNullsLast(a => a.Name).FirstOrDefault();
		}

		[Fact]
		public void Scalar()
		{
			var result1 = Context.Select<StudentModel>(a => a.Id == 1).ToScalar("[Id]");
			var result2 = Context.Select<StudentModel>(a => a.Id == 1).ToScalar<int>("[Id]");
		}

		[Fact]
		public void SelectByDbCache()
		{
			var result = Context.Select<StudentModel>(a => a.Id == 1).ByCache(TimeSpan.FromMinutes(1)).FirstOrDefault();
		}

		[Fact]
		public void Sum()
		{
			var result1 = Context.Select<StudentModel>().Sum(a => a.StuNo);
			var result2 = Context.Select<TypeTestModel>().Sum(a => a.DecimalType ?? 0, 0);
		}

		[Fact]
		public void ToList()
		{
			var result1 = Context.Select<StudentModel>().ToList();
			var result2 = Context.Select<StudentModel>().ToList<(int, string)>(a => new { a.Id, a.Name });
			var result3 = Context.Select<StudentModel>().ToList<(int, string)>("a.[Id],a.[Name]");
			var result4 = Context.Select<StudentModel>().ToList(a => a.Id);
			var result5 = Context.Select<StudentModel>(a => a.Id == -1).ToList<int?>(a => a.Id);
			var result6 = Context.Select<StudentModel>().ToList<Hashtable>("a.[Id],a.[Name]");
			Assert.Empty(result5);
		}

		[Fact]
		public void ToPipe()
		{
			object[] obj = Context.ExecuteReaderPipe(new ISqlBuilder[] {
				Context.Select<StudentModel>().ToListPipe(a => a.Id),
				Context.Select<StudentModel>().FirstOrDefaultPipe<(int, string)>(a => new { a.Id, a.Name }),
				Context.Select<StudentModel>().FirstOrDefaultPipe(),
				Context.Select<StudentModel>().Take(2).ToListPipe()

			});
			var info = obj[0].ToObjectArray().OfType<Guid>();
			var info1 = ((int, string))obj[1];
			var info2 = (StudentModel)obj[2];
			var info3 = obj[3].ToObjectArray().OfType<StudentModel>().ToList();
		}

		[Fact]
		public void ToUnion()
		{
			var result = Context.Select<StudentModel>().InnerJoinUnion<TeacherModel>((a, b) => a.TeacherId == b.Id).FirstOrDefaultUnion<TeacherModel>();
		}

		[Fact]
		public void Join()
		{
			var result1 = Context.Select<StudentModel>().InnerJoin<TeacherModel>((a, b) => a.TeacherId == b.Id).FirstOrDefault();
			var result2 = Context.Select<StudentModel>().LeftJoin<TeacherModel>((a, b) => a.TeacherId == b.Id).FirstOrDefault();
			var result3 = Context.Select<StudentModel>().RightJoin<TeacherModel>((a, b) => a.TeacherId == b.Id).FirstOrDefault();
			var result4 = Context.Select<StudentModel>().LeftOuterJoin<TeacherModel>((a, b) => a.TeacherId == b.Id).FirstOrDefault();
			var result5 = Context.Select<StudentModel>().RightOuterJoin<TeacherModel>((a, b) => a.TeacherId == b.Id).FirstOrDefault();
		}

		[Fact(Skip = "SqlServer暂不支持数组")]
		public void WhereArrayEqual()
		{
		}

		[Fact]
		public void WhereCoalesce()
		{
			var result = Context.Select<StudentModel>().Where(a => (a.TeacherId ?? 0) == 0).FirstOrDefault();
			if (result != null)
				Assert.True(result.TeacherId == null || result.TeacherId == 0);
		}

		[Fact]
		public void WhereCollectionContains()
		{
			var ids = new[] { 1, 2, 3 };
			var testAny = Context.Select<StudentModel>(a => ids.Contains(a.Id)).ToString();
			var result = Context.Select<StudentModel>(a => ids.Contains(a.Id)).FirstOrDefault();
			if (result is not null)
				Assert.Contains(result.Id, ids);
		}

		[Fact]
		public void WhereExists()
		{
			var result1 = Context.Select<StudentModel>().WhereExists<TeacherModel>(binder => binder.Field(b => b.Id)).FirstOrDefault();
			var result2 = Context.Select<StudentModel>().WhereExists<TeacherModel>(binder => binder.Field(b => b.Id).Where("b.[Id] = a.[TeacherId]")).FirstOrDefault();
		}

		[Fact(Skip = "SqlServer暂不支持数组")]
		public void WhereDbArrayFieldLength()
		{
		}

		[Fact(Skip = "SqlServer暂不支持数组")]
		public void WhereDbArrayFieldIndexParameter()
		{
		}

		[Fact]
		public void WhereIn()
		{
			var result = Context.Select<StudentModel>()
				.WhereIn<TeacherModel>(a => a.TeacherId, binder => binder.Field(b => b.Id)).FirstOrDefault();
		}

		[Fact]
		public void WhereOperationExpression()
		{
			var result = Context.Select<StudentModel>()
				.Where(a => a.CreateTime + (int)TimeSpan.FromHours(1).TotalSeconds < DateTimeOffset.Now.ToUnixTimeMilliseconds()).FirstOrDefault();
		}

		[Fact]
		public void WherePk()
		{
			var stu = new StudentModel()
			{
				Id = 1,
				Name = "Sam",
				TeacherId = 1,
				StuNo = 10000,
				CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
			};
			var result = Context.Select<StudentModel>().Where(stu).FirstOrDefault();
		}

		[Fact]
		public void WhereStringLike()
		{
			var result1 = Context.Select<StudentModel>().Where(a => a.Name.Contains("Sam")).FirstOrDefault();
			var result2 = Context.Select<StudentModel>().Where(a => a.Name.Contains("Sam", StringComparison.Ordinal)).FirstOrDefault();
			var result3 = Context.Select<StudentModel>().Where(a => a.Name.StartsWith("Sam")).FirstOrDefault();
			var result4 = Context.Select<StudentModel>().Where(a => a.Name.StartsWith("Sam", StringComparison.Ordinal)).FirstOrDefault();
			var result5 = Context.Select<StudentModel>().Where(a => a.Name.EndsWith("Sam")).FirstOrDefault();
			var result6 = Context.Select<StudentModel>().Where(a => a.Name.EndsWith("Sam", StringComparison.Ordinal)).FirstOrDefault();


			var result7 = Context.Select<StudentModel>().Where(a => "Sam".Contains(a.Name)).FirstOrDefault();
			var result8 = Context.Select<StudentModel>().Where(a => "Sam".Contains(a.Name, StringComparison.Ordinal)).FirstOrDefault();
			var result9 = Context.Select<StudentModel>().Where(a => "Sam".StartsWith(a.Name)).FirstOrDefault();
			var result10 = Context.Select<StudentModel>().Where(a => "Sam".StartsWith(a.Name, StringComparison.Ordinal)).FirstOrDefault();
			var result11 = Context.Select<StudentModel>().Where(a => "Sam".EndsWith(a.Name)).FirstOrDefault();
			var result12 = Context.Select<StudentModel>().Where(a => "Sam".EndsWith(a.Name, StringComparison.Ordinal)).FirstOrDefault();


			var result13 = Context.Select<StudentModel>().Where(a => !"Sam".Contains(a.Name)).FirstOrDefault();
			var result14 = Context.Select<StudentModel>().Where(a => !a.Name.Contains("Sam")).FirstOrDefault();

		}

		[Fact]
		public void WhereDbFieldToString()
		{
			var result = Context.Select<StudentModel>().Where(a => a.StuNo.ToString() == "10000").FirstOrDefault();
		}

		[Fact]
		public void Union()
		{
			var result1 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).Union<StudentModel>(binder => binder.Where(a => a.StuNo > 20)).ToList();
			var result2 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).UnionAll<StudentModel>(binder => binder.Where(a => a.StuNo > 20)).ToList();
			var result3 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).Union("select *from [Student] where [StuNo] > 20").ToList();
			var result4 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).UnionAll("select *from [Student] where [StuNo] > 20").ToList();
			var result5 = Context.Select<StudentModel>().Field(a => new { a.Id, a.Name }).Where(a => a.StuNo > 10).Union<StudentModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.StuNo > 20)).ToList<(int, string)>();
		}


		[Fact, Description("SqlServer不支持EXCEPT ALL语法")]
		public void Except()
		{
			var result1 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).Except<StudentModel>(binder => binder.Where(a => a.StuNo > 20)).ToList();
			var result3 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).Except("select *from [Student] where [StuNo] > 20").ToList();
			var result5 = Context.Select<StudentModel>().Field(a => new { a.Id, a.Name }).Where(a => a.StuNo > 10).Except<StudentModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.StuNo > 20)).ToList<(int, string)>();
			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result2 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).ExceptAll<StudentModel>(binder => binder.Where(a => a.StuNo > 20)).ToList();
				var result4 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).ExceptAll("select *from [Student] where [StuNo] > 20").ToList();
			});
			Assert.IsType<SqlException>(exception.InnerException);
		}

		[Fact, Description("SqlServer不支持INTERSECT ALL语法")]
		public void Intersect()
		{
			var result1 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).Intersect<StudentModel>(binder => binder.Where(a => a.StuNo > 20)).ToList();
			var result3 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).Intersect("select *from [Student] where [StuNo] > 20").ToList();
			var result5 = Context.Select<StudentModel>().Field(a => new { a.Id, a.Name }).Where(a => a.StuNo > 10).Intersect<StudentModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.StuNo > 20)).ToList<(int, string)>();

			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var result2 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).IntersectAll<StudentModel>(binder => binder.Where(a => a.StuNo > 20)).ToList();
				var result4 = Context.Select<StudentModel>().Where(a => a.StuNo > 10).IntersectAll("select *from [Student] where [StuNo] > 20").ToList();
			});
			Assert.IsType<SqlException>(exception.InnerException);
		}
		[Fact]
		public void CountDistinct()
		{
			var count = Context.Select<IdenPkModel>().CountDistinct(a => a.Name);
			Assert.True(count > 0);
		}

		[Fact]
		public void Distinct()
		{
			//var names1 = Context.Select<IdenPkModel>().Distinst().Take(10).ToList(a => a.Name);
			//var names2 = Context.Select<IdenPkModel>().Distinst().OrderByDescending(a => a.Name).Page(2, 10).ToList(a => a.Name);
		}
	}
}
