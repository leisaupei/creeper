
using Creeper.Extensions;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using Creeper.PostgreSql.XUnitTest.Entity.Options;
using Creeper.SqlBuilder;
using Meta.xUnitTest.Extensions;
using Meta.xUnitTest.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(2)]
	public class SelectNormal : BaseTest
	{
		public SelectNormal(ITestOutputHelper output) : base(output)
		{
		}

		[Fact, Order(1)]
		public void FirstOrDefault()
		{
			var info = _dbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId1).By(Generic.DataBaseType.Secondary).FirstOrDefault();

			Assert.Equal(StuPeopleId1, info.People_id);
		}

		[Fact, Order(4)]
		public void Frist()
		{
			var peopleId = _dbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId1).FirstOrDefault<Guid>("people_id");
			var info = _dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<ToOneTTestModel>("name,id");

			var emptyNullablePeopleId = _dbContext.Select<StudentModel>().Where(a => a.People_id == Guid.Empty).FirstOrDefault<Guid?>("people_id");
			var emptyPeopleId = _dbContext.Select<StudentModel>().Where(a => a.People_id == Guid.Empty).FirstOrDefault<Guid>("people_id");

			Assert.IsType<ToOneTTestModel>(info);
			Assert.Equal(StuPeopleId1, info.Id);
			Assert.Equal(StuPeopleId1, peopleId);
			Assert.Null(emptyNullablePeopleId);
			Assert.Equal(Guid.Empty, emptyPeopleId);
		}

		[Fact, Order(5)]
		public void FristTuple()
		{
			var info = _dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<(Guid id, string name)>("id,name");
			// if not found
			var notFoundInfo = _dbContext.Select<PeopleModel>().Where(a => a.Id == Guid.Empty).FirstOrDefault<(Guid id, string name)>("id,name");
			var notFoundNullableInfo = _dbContext.Select<PeopleModel>().Where(a => a.Id == Guid.Empty).FirstOrDefault<(Guid? id, string name)>("id,name");


			Assert.Equal(StuPeopleId1, info.id);
			Assert.Equal(Guid.Empty, notFoundInfo.id);
			Assert.Null(notFoundNullableInfo.id);
		}

		[Fact, Order(6)]
		public void SelectByDbCache()
		{
			Stopwatch sw = Stopwatch.StartNew();
			var info = _dbContext.Select<StudentModel>().Where(a => a.Stu_no == StuNo1).ByCache().FirstOrDefault();

			var a = sw.ElapsedMilliseconds.ToString();
			info = _dbContext.Select<StudentModel>().Where(a => a.Stu_no == StuNo1).ByCache().FirstOrDefault();
			sw.Stop();
			var b = sw.ElapsedMilliseconds.ToString();
			//var key = _dbContext.Select<StudentModel>().Where(a => a.Stu_no == StuNo1).ByCache().ToScalar(a => a.Id);
			//Assert.Equal(StuNo1, info.Stu_no);
		}

		[Fact, Order(7)]
		public void GetItemsByUniqueKey()
		{
			var info = _dbContext.Select<StudentModel>().Where(a => new[] { StuNo1, StuNo2 }.Contains(a.Stu_no)).ToList();

			Assert.Contains(info, f => f.Stu_no == StuNo1);
			Assert.Contains(info, f => f.Stu_no == StuNo2);
		}

		[Fact, Order(8)]
		public void ToOneDictonary()
		{
			// all
			var info = _dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<Dictionary<string, object>>();
			// option
			var info1 = _dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<Hashtable>("name,id");
			Assert.Equal(StuPeopleId1, Guid.Parse(info["id"].ToString()));
			Assert.Equal(StuPeopleId1, Guid.Parse(info1["id"].ToString()));
		}

		[Fact, Order(9)]
		public void ToList()
		{
			var info = _dbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList();

			Assert.Contains(info, f => f.Id == StuPeopleId1);
			Assert.Contains(info, f => f.Id == StuPeopleId2);
		}

		[Fact, Order(10)]
		public void ToListTuple()
		{
			var info = _dbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList<(Guid, string)>("id,name");

			Assert.Contains(info, f => f.Item1 == StuPeopleId1);
			Assert.Contains(info, f => f.Item1 == StuPeopleId2);
		}

		[Fact, Order(11)]
		public void ToListDictonary()
		{
			// all
			var info = _dbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList<Dictionary<string, object>>();
			// option
			var info1 = _dbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList<Dictionary<string, object>>("name,id");

			Assert.Contains(info, f => f["id"].ToString() == StuPeopleId1.ToString());
			Assert.Contains(info, f => f["id"].ToString() == StuPeopleId2.ToString());
			Assert.Contains(info1, f => f["id"].ToString() == StuPeopleId1.ToString());
			Assert.Contains(info1, f => f["id"].ToString() == StuPeopleId2.ToString());
		}

		[Fact, Order(12)]
		public void ToPipe()
		{
			object[] obj = _dbContext.GetExecute<DbSecondary>().ExecuteDataReaderPipe(new ISqlBuilder[] {
				_dbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).PipeToList(),
				_dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).PipeFirstOrDefault<(Guid, string)>("id,name"),
				_dbContext.Select<PeopleModel>().Where(a =>a.Id==StuPeopleId1).PipeToList<(Guid, string)>("id,name"),
				_dbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).PipeToList<Dictionary<string, object>>("name,id"),
				_dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).PipeFirstOrDefault<ToOneTTestModel>("name,id"),
				_dbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId1).PipeFirstOrDefault<Guid>("people_id"),
				_dbContext.Select<StudentModel>().UnionLeftJoin<PeopleModel>((a,b) => a.People_id == b.Id).Where(a => a.People_id == StuPeopleId2).PipeUnionFirstOrDefault<PeopleModel>(),
				 });
			var info = obj[0].ToObjectArray().OfType<PeopleModel>();
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
		[Fact, Order(13)]
		public void Count()
		{
			var count = _dbContext.Select<PeopleModel>().Count();
		}
		[Fact, Order(14)]
		public void Max()
		{
			var maxAge = _dbContext.Select<PeopleModel>().Max(a => a.Age);

			maxAge = _dbContext.Select<StudentModel>().InnerJoin<PeopleModel>((a, b) => a.People_id == b.Id).Max<PeopleModel, int>(b => b.Age);
			Assert.True(maxAge >= 0);
		}
		[Fact, Order(15)]
		public void Min()
		{
			var minAge = _dbContext.Select<PeopleModel>().Min(a => a.Age);
			Assert.True(minAge >= 0);
		}
		[Fact, Order(16), Description("the type of T must be same as the column's type")]
		public async void Avg()
		{
			var avgAge1 = await _dbContext.Select<PeopleModel>().FirstOrDefaultAsync<Guid>(a => a.Id);
			var avgAge = _dbContext.Select<PeopleModel>().Avg<decimal>(a => a.Age);
			Assert.True(avgAge >= 0);
		}

		[Fact, Order(17), Description("same usage as FirstOrDefault<T>(), but T is ValueType")]
		public void Scalar()
		{
			var id = _dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<Guid>(a => a.Id);
			Assert.Equal(StuPeopleId1, id);
		}
		[Fact, Order(18)]
		public void OrderBy()
		{
			var infos = _dbContext.Select<PeopleModel>().InnerJoin<StudentModel>((a, b) => a.Id == b.People_id)
				.OrderByDescending(a => a.Age).ToList();
		}
		[Fact, Order(19)]
		public void GroupBy()
		{

		}
		[Fact, Order(20)]
		public void Page()
		{

		}
		[Fact, Order(21)]
		public void Limit()
		{

		}
		[Fact, Order(22)]
		public void Skip()
		{

		}
		[Fact, Order(23), Description("using with group by expression")]
		public void Having()
		{

		}
		[Fact, Order(24), Description("")]
		public void Union()
		{
			Stopwatch stop = new Stopwatch();
			stop.Start();
			var union0 = _dbContext.Select<StudentModel>().UnionInnerJoin<PeopleModel>((a, b) => a.People_id == b.Id && a.Stu_no == StuNo1).UnionFirstOrDefault<PeopleModel>();

			var a = stop.ElapsedMilliseconds;
			var union1 = _dbContext.Select<StudentModel>().UnionInnerJoin<PeopleModel>((a, b) => a.People_id == b.Id && a.Stu_no == StuNo1).UnionFirstOrDefault<PeopleModel>();
			var b = stop.ElapsedMilliseconds;
			var union2 = _dbContext.Select<StudentModel>().UnionInnerJoin<PeopleModel>((a, b) => a.People_id == b.Id && a.Stu_no == StuNo1).UnionFirstOrDefault<PeopleModel>();
			var c = stop.ElapsedMilliseconds;
			stop.Stop();

			_output.WriteLine(string.Concat(a, ",", b, ",", c));
		}

		[Fact, Order(16), Description("the type of T must be same as the column's type")]
		public void Sum()
		{
			var avgAge = _dbContext.Select<PeopleModel>().Sum<long>(a => a.Age);
			Assert.True(avgAge >= 0);
		}
	}
}
