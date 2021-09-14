
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.PostgreSql.Test.Entity.Model;
using Creeper.SqlBuilder;
using Creeper.xUnitTest.Contracts;
using Creeper.xUnitTest.Extensions;
using Creeper.xUnitTest.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Creeper.xUnitTest.PostgreSql
{
	public class SelectTest : BaseTest, ISelectTest
	{
		public SelectTest(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public void FirstOrDefault()
		{
			var peopleId = Context.Select<CreeperStudentModel>(a => a.People_id == StuPeopleId1).FirstOrDefault<Guid>("people_id");
			var info = Context.Select<CreeperPeopleModel>(a => a.Id == StuPeopleId1).FirstOrDefault<ToOneTTestModel>("name,id");

			var emptyNullablePeopleId = Context.Select<CreeperStudentModel>(a => a.People_id == Guid.Empty).FirstOrDefault<Guid?>("people_id");
			var emptyPeopleId = Context.Select<CreeperStudentModel>(a => a.People_id == Guid.Empty).FirstOrDefault<Guid>("people_id");

			Assert.IsType<ToOneTTestModel>(info);
			Assert.Equal(StuPeopleId1, info.Id);
			Assert.Equal(StuPeopleId1, peopleId);
			Assert.Null(emptyNullablePeopleId);
			Assert.Equal(Guid.Empty, emptyPeopleId);

			//tuple
			var foundInfo = Context.Select<CreeperPeopleModel>().Where(a => a.Id == StuPeopleId1)
				.FirstOrDefault<(Guid id, string name)>(a => new { a.Id, a.Name });
			// if not found
			var notFoundInfo = Context.Select<CreeperPeopleModel>().Where(a => a.Id == Guid.Empty).FirstOrDefault<(Guid id, string name)>("id,name");
			var notFoundNullableInfo = Context.Select<CreeperPeopleModel>().Where(a => a.Id == Guid.NewGuid()).FirstOrDefault<(Guid? id, string name)>("id,name");
			Assert.Equal(StuPeopleId1, foundInfo.id);
			Assert.Equal(Guid.Empty, notFoundInfo.id);
			Assert.Null(notFoundNullableInfo.id);

			// dictonary
			var result1 = Context.Select<CreeperPeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<Dictionary<string, object>>();
			// option
			var result2 = Context.Select<CreeperPeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<Hashtable>("name,id");
			Assert.Equal(StuPeopleId1, Guid.Parse(result1["id"].ToString()));
			Assert.Equal(StuPeopleId1, Guid.Parse(result2["id"].ToString()));
		}

		[Fact]
		public void Frist()
		{
			Assert.Throws<CreeperFirstNotFoundException>(() =>
			{
				var info = Context.Select<CreeperStudentModel>().Where(a => a.People_id == Guid.NewGuid()).First();
			});

		}

		[Fact]
		public void SelectByDbCache()
		{
			Stopwatch sw = Stopwatch.StartNew();
			var info = Context.Select<CreeperStudentModel>().Where(a => a.Stu_no == StuNo1).ByCache(TimeSpan.FromMinutes(2)).FirstOrDefault();

			var a = sw.ElapsedMilliseconds.ToString();
			info = Context.Select<CreeperStudentModel>().Where(a => a.Stu_no == StuNo1).ByCache().FirstOrDefault();
			sw.Stop();
			var b = sw.ElapsedMilliseconds.ToString();
			//var key = _context.Select<StudentModel>().Where(a => a.Stu_no == StuNo1).ByCache().ToScalar(a => a.Id);
			//Assert.Equal(StuNo1, info.Stu_no);
		}

		[Fact]
		public void ToList()
		{
			var info = Context.Select<CreeperPeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList();

			Assert.Contains(info, f => f.Id == StuPeopleId1);
			Assert.Contains(info, f => f.Id == StuPeopleId2);


			var info1 = Context.Select<CreeperPeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList<(Guid, string)>("id,name");

			Assert.Contains(info1, f => f.Item1 == StuPeopleId1);
			Assert.Contains(info1, f => f.Item1 == StuPeopleId2);

			// all
			var info2 = Context.Select<CreeperPeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList<Dictionary<string, object>>();
			// option
			var info3 = Context.Select<CreeperPeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList<Dictionary<string, object>>("name,id");

			Assert.Contains(info2, f => f["id"].ToString() == StuPeopleId1.ToString());
			Assert.Contains(info2, f => f["id"].ToString() == StuPeopleId2.ToString());
			Assert.Contains(info3, f => f["id"].ToString() == StuPeopleId1.ToString());
			Assert.Contains(info3, f => f["id"].ToString() == StuPeopleId2.ToString());
		}

		[Fact]
		public void ToPipe()
		{
			object[] obj = Context.ExecuteReaderPipe(new ISqlBuilder[] {
				Context.Select<CreeperPeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToListPipe(),
				Context.Select<CreeperPeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefaultPipe<(Guid, string)>("id,name"),
				Context.Select<CreeperPeopleModel>().Where(a =>a.Id==StuPeopleId1).ToListPipe<(Guid, string)>("id,name"),
				Context.Select<CreeperPeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToListPipe<Dictionary<string, object>>("name,id"),
				Context.Select<CreeperPeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefaultPipe<ToOneTTestModel>("name,id"),
				Context.Select<CreeperStudentModel>().Where(a => a.People_id == StuPeopleId1).FirstOrDefaultPipe<Guid>("people_id"),
				Context.Select<CreeperStudentModel>().LeftJoinUnion<CreeperPeopleModel>((a,b) => a.People_id == b.Id).Where(a => a.People_id == StuPeopleId2).FirstOrDefaultUnionPipe<CreeperPeopleModel>(),
				 });
			var info = obj[0].ToObjectArray().OfType<CreeperPeopleModel>();
			var info1 = ((Guid, string))obj[1];
			var info2 = obj[2].ToObjectArray().OfType<(Guid, string)>();
			var info3 = obj[3].ToObjectArray().OfType<Dictionary<string, object>>();
			var info4 = (ToOneTTestModel)obj[4];
			var info5 = (Guid)obj[5];
			var info7 = obj[6];
			Assert.Contains(info, f => f.Id == StuPeopleId1);
			Assert.Equal(StuPeopleId1, info1.Item1);
			Assert.Contains(info2, f => f.Item1 == StuPeopleId1);
			Assert.Contains(info3, f => f["id"].ToString() == StuPeopleId1.ToString());
			Assert.Equal(StuPeopleId1, info4.Id);
			Assert.Equal(StuPeopleId1, info5);
			//Assert.Null(info6);
		}

		[Fact]
		public void Count()
		{
			var count = Context.Select<CreeperPeopleModel>().Count();
		}

		[Fact]
		public void Max()
		{
			var maxAge = Context.Select<CreeperPeopleModel>().Max(a => a.Age);

			maxAge = Context.Select<CreeperStudentModel>().InnerJoin<CreeperPeopleModel>((a, b) => a.People_id == b.Id).Max<CreeperPeopleModel, int>(b => b.Age);
			Assert.True(maxAge >= 0);
		}

		[Fact]
		public void Min()
		{
			var minAge = Context.Select<CreeperPeopleModel>().Min(a => a.Age);
			Assert.True(minAge >= 0);
		}

		[Fact]
		public void Avg()
		{
			var avgAge = Context.Select<CreeperPeopleModel>().Avg<decimal>(a => a.Age);
			Assert.True(avgAge >= 0);
		}

		[Fact]
		public void Scalar()
		{
			var id = Context.Select<CreeperPeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<Guid>(a => a.Id);
			Assert.Equal(StuPeopleId1, id);
		}

		[Fact]
		public void OrderBy()
		{
			var infos = Context.Select<CreeperPeopleModel>().InnerJoin<CreeperStudentModel>((a, b) => a.Id == b.People_id)
				.OrderByDescending(a => a.Age).ToList();
		}

		[Fact]
		public void GroupBy()
		{
			var result = Context.Select<CreeperStudentModel>().GroupBy(a => a.Grade_id).ToList(a => a.Grade_id);
		}

		[Theory]
		[InlineData(1, 10)]
		[InlineData(2, 10)]
		public void Page(int index, int size)
		{
			var result = Context.Select<CreeperStudentModel>().Page(index, size).ToList();
		}

		[Fact]
		public void Limit()
		{
			var result = Context.Select<CreeperStudentModel>().Take(10).ToList();
		}

		[Fact]
		public void Skip()
		{
			var result = Context.Select<CreeperStudentModel>().Skip(10).ToList();
		}

		[Fact, Description("using with group by expression")]
		public void Having()
		{
			var result = Context.Select<CreeperStudentModel>().GroupBy(a => a.Grade_id).Having("COUNT(1) >= 10").ToList(a => a.Grade_id);
		}

		[Fact, Description("")]
		public void ToUnion()
		{
			Stopwatch stop = new Stopwatch();
			stop.Start();
			var union0 = Context.Select<CreeperStudentModel>().InnerJoinUnion<CreeperPeopleModel>((a, b) => a.People_id == b.Id && a.Stu_no == StuNo1).FirstOrDefaultUnion<CreeperPeopleModel>();

			var a = stop.ElapsedMilliseconds;
			var union1 = Context.Select<CreeperStudentModel>().InnerJoinUnion<CreeperPeopleModel>((a, b) => a.People_id == b.Id && a.Stu_no == StuNo1).FirstOrDefaultUnion<CreeperPeopleModel>();
			var b = stop.ElapsedMilliseconds;
			var union2 = Context.Select<CreeperStudentModel>().InnerJoinUnion<CreeperPeopleModel>((a, b) => a.People_id == b.Id && a.Stu_no == StuNo1).FirstOrDefaultUnion<CreeperPeopleModel>();
			var c = stop.ElapsedMilliseconds;
			stop.Stop();

			Output.WriteLine(string.Concat(a, ",", b, ",", c));
		}

		[Fact]
		public void Sum()
		{
			var avgAge = Context.Select<CreeperPeopleModel>().Sum<long>(a => a.Age);
			Assert.True(avgAge >= 0);
		}

		[Fact]
		public void Join()
		{
			var union = Context.Select<CreeperStudentModel>()
					.InnerJoin<CreeperClassmateModel>((a, b) => a.Id == b.Student_id)
					.InnerJoin<CreeperClassmateModel, CreeperTeacherModel>((b, c) => b.Teacher_id == c.Id)
					.InnerJoin<CreeperPeopleModel>((a, d) => a.People_id == d.Id && d.Id == StuPeopleId1)
					.FirstOrDefault();

			Assert.Null(union);
		}

		[Fact]
		public void WherePk()
		{
			var people = new CreeperPeopleModel { Id = StuPeopleId1, Address = "xxx", Age = 1, Create_time = DateTime.Now, Name = "lsp", Sex = true, State = CreeperDataState.正常, Address_detail = new JObject() };
			var peoples = new CreeperPeopleModel { Id = StuPeopleId2, Address = "xxx", Age = 1, Create_time = DateTime.Now, Name = "lsp", Sex = true, State = CreeperDataState.正常, Address_detail = new JObject() };
			var result = Context.Select<CreeperPeopleModel>().Where(people).FirstOrDefault();
			Assert.Equal(people.Id, result.Id);
		}

		[Fact]
		public void WhereExists()
		{
			var info = Context.Select<CreeperStudentModel>()
				.WhereExists<CreeperPeopleModel>(binder => binder.Field(b => b.Id).Where(b => b.Id == StuPeopleId1))
				.ByCache(TimeSpan.FromSeconds(60))
				.FirstOrDefault();
			info = Context.Select<CreeperStudentModel>()
				.WhereExists<CreeperPeopleModel>(binder => binder.Field(b => b.Id).Where(b => b.Id == StuPeopleId1))
				.FirstOrDefault();
		}

		[Fact]
		public void WhereOperationExpression()
		{
			//var info1 = Context.Select<CreeperPeopleModel>().Where(a => "s" + a.Name == "sss").FirstOrDefault();
			var info = Context.Select<CreeperPeopleModel>().Where(a => DateTime.Today - a.Create_time > TimeSpan.FromDays(2)).FirstOrDefault();
			Assert.NotNull(info);
		}

		[Fact]
		public void WhereDbArrayFieldIndexParameter()
		{
			var arr = new[] { 1 };
			var info = Context.Select<CreeperTypeTestModel>().Where(a => a.Array_type[1] == arr[0]).FirstOrDefault();
			Assert.NotNull(info);
		}

		[Fact]
		public void WhereDbArrayFieldLength()
		{
			var info = Context.Select<CreeperTypeTestModel>().Where(a => a.Array_type.Length == 2).FirstOrDefault();
			Assert.NotNull(info);
		}

		[Fact]
		public void WhereCollectionContains()
		{
			var xx = new CreeperPeopleModel();
			xx.Id = Guid.Empty;
			var b = new CreeperPeopleModel();
			b.Id = Guid.Empty;
			CreeperTypeTestModel info = null;

			info = Context.Select<CreeperTypeTestModel>().Where(a => new[] { xx.Id, b.Id }.Select(a => a).Contains(a.Id)).FirstOrDefault();
			info = Context.Select<CreeperTypeTestModel>().Where(a => new[] { xx.Id, b.Id }.Contains(a.Id)).FirstOrDefault();
			Assert.NotNull(info);

			info = Context.Select<CreeperTypeTestModel>().Where(a => new[] { 1, 3, 4 }.Contains(a.Int4_type.Value)).FirstOrDefault();
			Assert.NotNull(info);

			//a.int_type <> all(array[2,3])
			info = Context.Select<CreeperTypeTestModel>().Where(a => !a.Array_type.Contains(3)).FirstOrDefault();
			Assert.NotNull(info);

			//3 = any(a.array_type)
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Array_type.Contains(3)).FirstOrDefault();
			Assert.NotNull(info);

			//var ints = new int[] { 2, 3 }.Select(f => f).ToList();
			//a.int_type = any(array[2,3])
			info = Context.Select<CreeperTypeTestModel>().Where(a => new int[] { 2, 3 }.Select(f => f).Contains(a.Int4_type.Value)).FirstOrDefault();
			Assert.NotNull(info);

			info = Context.Select<CreeperTypeTestModel>().Where(a => new[] { (int)CreeperDataState.删除, (int)CreeperDataState.正常 }.Contains(a.Int4_type.Value)).FirstOrDefault();

			Assert.NotNull(info);
		}
		[Fact]
		public void WhereStringLike()
		{
			CreeperTypeTestModel info = null;


			//'xxx' like a.Varchar_type || '%'
			info = Context.Select<CreeperTypeTestModel>().Where(a => "xxxxxxxxxxxx".StartsWith(a.Varchar_type)).FirstOrDefault();
			Assert.NotNull(info);
			//a.varchar_type like '%xxx%'
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.Contains("xxx")).FirstOrDefault();
			Assert.NotNull(info);
			//a.varchar_type like 'xxx%'
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.StartsWith("xxx")).FirstOrDefault();
			Assert.NotNull(info);
			//a.varchar_type like '%xxx'
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.EndsWith("xxx")).FirstOrDefault();
			Assert.NotNull(info);

			//a.varchar_type ilike '%xxx%'
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.Contains("xxx", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
			Assert.NotNull(info);
			//a.varchar_type ilike 'xxx%'
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.StartsWith("xxx", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
			Assert.NotNull(info);
			//a.varchar_type ilike '%xxx'
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.EndsWith("xxx", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
			Assert.NotNull(info);
		}

		[Fact]
		public void WhereDbFieldToString()
		{
			CreeperTypeTestModel info = null;
			//a.varchar_type::text = 'xxxx'
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.ToString() == "xxxx").FirstOrDefault();
			Assert.NotNull(info);
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_type.ToString() == "xxxx".ToString()).FirstOrDefault();
			Assert.NotNull(info);

		}

		[Fact]
		public void WhereArrayEqual()
		{
			CreeperTypeTestModel info = null;
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Array_type == new[] { 0, 1 }).FirstOrDefault();
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Uuid_array_type == new[] { Guid.Empty }).FirstOrDefault();
			info = Context.Select<CreeperTypeTestModel>().Where(a => new[] { "广东" } == a.Varchar_array_type).FirstOrDefault();
			info = Context.Select<CreeperTypeTestModel>().Where(a => a.Varchar_array_type == new[] { "广东" }).FirstOrDefault();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereCoalesce()
		{
			var info = Context.Select<CreeperTypeTestModel>().Where(a => (a.Int4_type ?? 3) == 3).FirstOrDefault();
			Assert.NotNull(info);

		}

		[Fact]
		public void WhereIn()
		{
			var info = Context.Select<CreeperStudentModel>()
					.WhereIn<CreeperStudentModel>(a => a.People_id, binder => binder.Field(a => a.People_id).Where(a => a.People_id == StuPeopleId1))
					.FirstOrDefault();

			Assert.Equal(info.People_id, StuPeopleId1);
		}

		[Fact]
		public void Union()
		{
			var result1 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).Union<CreeperPeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
			var result2 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).UnionAll<CreeperPeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
			var result3 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).Union("select *from \"creeper\".\"people\" where \"age\" > 20").ToList();
			var result4 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).UnionAll("select *from \"creeper\".\"people\" where \"age\" > 20").ToList();
			var result5 = Context.Select<CreeperPeopleModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Age > 10).Union<CreeperPeopleModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Age > 20)).ToList<(Guid, string)>();
		}

		[Fact]
		public void Except()
		{
			var result1 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).Union<CreeperPeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
			var result2 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).UnionAll<CreeperPeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
			var result3 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).Union("select *from \"creeper\".\"people\" where \"age\" > 20").ToList();
			var result4 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).UnionAll("select *from \"creeper\".\"people\" where \"age\" > 20").ToList();
			var result5 = Context.Select<CreeperPeopleModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Age > 10).Union<CreeperPeopleModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Age > 20)).ToList<(Guid, string)>();
		}

		[Fact]
		public void Intersect()
		{
			var result1 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).Union<CreeperPeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
			var result2 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).UnionAll<CreeperPeopleModel>(binder => binder.Where(a => a.Age > 20)).ToList();
			var result3 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).Union("select *from \"creeper\".\"people\" where \"age\" > 20").ToList();
			var result4 = Context.Select<CreeperPeopleModel>().Where(a => a.Age > 10).UnionAll("select *from \"creeper\".\"people\" where \"age\" > 20").ToList();
			var result5 = Context.Select<CreeperPeopleModel>().Field(a => new { a.Id, a.Name }).Where(a => a.Age > 10).Union<CreeperPeopleModel>(binder => binder.Field(a => new { a.Id, a.Name }).Where(a => a.Age > 20)).ToList<(Guid, string)>();
		}

		[Fact]
		public void CountDistinct()
		{
			var count = Context.Select<CreeperIdenPkModel>().CountDistinct(a => a.Name);
			Assert.True(count > 0);
		}

		[Fact]
		public void Distinct()
		{
			//var names = Context.Select<CreeperIdenPkModel>().Distinst().ToList(a => a.Name);
			//Assert.True(names.Count > 0);
		}
	}
}
