using Creeper.Extensions;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(1)]
	public class Insert : BaseTest
	{
		[Fact, Order(1), Description("name of create_time can be ignored if use 'Create_time = Datetime.Now' in ModelInsert")]
		public void ModelInsertReturnModel()
		{
			var info = _dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).ToOne();
			if (info == null)
			{
				info = _dbContext.Insert(new PeopleModel
				{
					Address = "xxx",
					Id = StuPeopleId1,
					Age = 10,
					Create_time = DateTime.Now, // you can ignore if use Datetime.Now;
					Name = "leisaupei",
					Sex = true,
					State = EDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				});

				Assert.NotNull(info);
			}
			info = _dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId2).ToOne();
			if (info == null)
			{
				// else you can
				info = _dbContext.Insert(new PeopleModel
				{
					Address = "xxx",
					Id = StuPeopleId2,
					Age = 10,
					Create_time = DateTime.Now,
					Name = "leisaupei",
					Sex = true,
					State = EDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				});
				Assert.NotNull(info);
			}
		}
		[Fact, Order(2)]
		public void ModelInsertReturnModifyRows()
		{
			var info = _dbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId2).ToOne();
			if (info != null) return;

			var row = _dbContext.Commit(new PeopleModel
			{
				Address = "xxx",
				Id = StuPeopleId2,
				Age = 10,
				Create_time = DateTime.Now,
				Name = "nickname",
				Sex = true,
				State = EDataState.正常,
				Address_detail = new JObject
				{
					["province"] = "广东",
					["city"] = "广州"
				},
			});
			//else you can
			//row = Teacher.Commit(new TeacherModel
			//{
			//	Address = "xxx",
			//	Id = StuPeopleId2,
			//	Age = 10,
			//	Create_time = DateTime.Now,
			//	Name = "nickname",
			//	Sex = true

			//});
			Assert.Equal(1, row);
		}
		[Fact, Order(3)]
		public void InsertCustomizedDictonary()
		{
			var info = _dbContext.Select<ClassGradeModel>().Where(a => a.Id == GradeId).ToOne();
			if (info != null) return;

			var affrows = _dbContext.Insert<ClassGradeModel>().Set(a => a.Id, GradeId)
				.Set(f => f.Name, "移动互联网")
				.Set(a => a.Create_time, DateTime.Now)
				.ToRows(out info); //return modify rows out model

			Assert.NotNull(info);
			Assert.Equal(1, affrows);
		}
		[Fact, Order(4)]
		public void InsertCustomized()
		{
			var info = _dbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId1).ToOne();
			if (info != null) return;
			var affrows = _dbContext.Insert<StudentModel>().Set(f => f.Id, Guid.NewGuid())
							.Set(a => a.People_id, StuPeopleId1)
							.Set(a => a.Stu_no, StuNo1)
							.Set(a => a.Grade_id, GradeId)
							.Set(a => a.Create_time, DateTime.Now)
							.ToRows(out info);
			Assert.NotNull(info);
			Assert.Equal(1, affrows);

			var info1 = _dbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId1).ToOne();
			if (info1 != null) return;
			var affrows1 = _dbContext.Insert<StudentModel>().Set(a => a.Id, Guid.NewGuid())
							.Set(a => a.People_id, StuPeopleId2)
							.Set(a => a.Stu_no, StuNo2)
							.Set(a => a.Grade_id, GradeId)
							.Set(a => a.Create_time, DateTime.Now)
							.ToRows(out info1);
			Assert.NotNull(info1);
			Assert.Equal(1, affrows1);
		}
		[Fact, Order(4)]
		public void InsertMultiple()
		{
			var arr = new[] {
				new PeopleModel
				{
					Address = "xxx",
					Id = Guid.NewGuid(),
					Age = 10,
					Create_time = DateTime.Now,
					Name = "nickname",
					Sex = true,
					State = EDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				},
				new PeopleModel
				{
					Address = "xxx",
					Id = Guid.NewGuid(),
					Age = 10,
					Create_time = DateTime.Now,
					Name = "nickname",
					Sex = true,
					State = EDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				}
			};
			var rows = _dbContext.CommitMany(arr);

			Assert.NotEqual(0, rows);
		}
	}
}
