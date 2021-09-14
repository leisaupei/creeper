using Creeper.Driver;
using Creeper.Extensions;
using Creeper.PostgreSql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;

namespace Creeper.xUnitTest.PostgreSql
{
	public class InsertTest : BaseTest, IInsertTest
	{
		[Fact]
		public void InsertReturning()
		{
			var info = new CreeperPeopleModel
			{
				Address = "xxx",
				Id = Guid.NewGuid(),
				Age = 10,
				Create_time = DateTime.Now,
				Name = "leisaupei",
				Sex = true,
				State = CreeperDataState.正常,
				Address_detail = new JObject
				{
					["province"] = "广东",
					["city"] = "广州"
				},
			};
			var result = Context.InsertResult(info);

			Assert.Equal(1, result.AffectedRows);
			Assert.Equal(info.Id, result.Value.Id);

		}

		[Fact]
		public void ModelInsertReturnModifyRows()
		{
			var info = Context.Select<CreeperPeopleModel>().Where(a => a.Id == StuPeopleId2).FirstOrDefault();
			if (info != null) return;

			var row = Context.Insert(new CreeperPeopleModel
			{
				Address = "xxx",
				Id = StuPeopleId2,
				Age = 10,
				Create_time = DateTime.Now,
				Name = "nickname",
				Sex = true,
				State = CreeperDataState.正常,
				Address_detail = new JObject
				{
					["province"] = "广东",
					["city"] = "广州"
				},
			});
			Assert.Equal(1, row);
		}

		[Fact]
		public void InsertCustomizedDictonary()
		{
			var info = Context.Select<CreeperGradeModel>().Where(a => a.Id == GradeId).FirstOrDefault();
			if (info != null) return;

			var result = Context.Insert<CreeperGradeModel>().Set(a => a.Id, GradeId)
				.Set(f => f.Name, "移动互联网")
				.Set(a => a.Create_time, DateTime.Now)
				.ToAffrowsResult(); //return modify rows out model

			Assert.NotNull(result.Value);
			Assert.Equal(1, result.AffectedRows);
		}

		[Fact]
		public void InsertCustomized()
		{
			var info = Context.Select<CreeperStudentModel>().Where(a => a.People_id == StuPeopleId1).FirstOrDefault();
			if (info == null)
			{
				var result = Context.Insert<CreeperStudentModel>().Set(f => f.Id, Guid.NewGuid())
								.Set(a => a.People_id, StuPeopleId1)
								.Set(a => a.Stu_no, StuNo1)
								.Set(a => a.Grade_id, GradeId)
								.Set(a => a.Create_time, DateTime.Now)
								.ToAffrowsResult();
				Assert.Equal(1, result.AffectedRows);
				Assert.NotNull(info);
			}

			var info1 = Context.Select<CreeperStudentModel>().Where(a => a.People_id == StuPeopleId2).FirstOrDefault();
			if (info1 == null)
			{
				var result = Context.Insert<CreeperStudentModel>().Set(a => a.Id, Guid.NewGuid())
								.Set(a => a.People_id, StuPeopleId2)
								.Set(a => a.Stu_no, StuNo2)
								.Set(a => a.Grade_id, GradeId)
								.Set(a => a.Create_time, DateTime.Now)
								.ToAffrowsResult();
				Assert.NotNull(result.Value);
				Assert.Equal(1, result.AffectedRows);
			}
		}

		[Fact]
		public void InsertMultiple()
		{
			var info = Context.Insert<CreeperPeopleModel>().Set(new CreeperPeopleModel
			{
				Address = "xxx",
				Id = Guid.NewGuid(),
				Age = 10,
				Create_time = DateTime.Now,
				Name = "nickname",
				Sex = true,
				State = CreeperDataState.正常,
				Address_detail = new JObject
				{
					["province"] = "广东",
					["city"] = "广州"
				}
			}).WhereNotExists<CreeperPeopleModel>(binder => binder.Where(a => a.Name == "小明")).ToAffrows();
			var arr = new[] {
				new CreeperPeopleModel
				{
					Address = "xxx",
					Id = Guid.NewGuid(),
					Age = 10,
					Create_time = DateTime.Now,
					Name = "nickname",
					Sex = true,
					State = CreeperDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				},
				new CreeperPeopleModel
				{
					Address = "xxx",
					Id = Guid.NewGuid(),
					Age = 10,
					Create_time = DateTime.Now,
					Name = "nickname",
					Sex = true,
					State = CreeperDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				}
			};
			var rows = Context.InsertRange(arr);

			Assert.NotEqual(0, rows);
		}

		[Fact, Description("自增主键")]
		public void IdentityPk()
		{
			var affrows = Context.Insert(new CreeperIdenPkModel
			{
				Age = 20,
				Name = "小云"
			});
			Assert.Equal(1, affrows);
		}

		[Fact]
		[Description("使用单句的sql批量插入")]
		public void InsertRangeSingle()
		{
			var list = new List<CreeperIdenPkModel>();
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			var affrows = Context.InsertRange(list);
			Assert.Equal(list.Count, affrows);
			Context.InsertRange<CreeperIdenPkModel>().Set(list).ToListAffrowsResult();
			Assert.Throws<ArgumentNullException>(() => Context.InsertRange<CreeperIdenPkModel>(null));
			Assert.Throws<ArgumentNullException>(() => Context.InsertRange(Array.Empty<CreeperIdenPkModel>()));
		}

		[Fact]
		[Description("Insert语句包含Where条件")]
		public void InsertWithWhere()
		{
			var info = new CreeperIdenPkModel { Age = 20, Name = "中国" };
			var affrows = Context.Insert<CreeperIdenPkModel>().Set(info)
				.WhereExists<CreeperIdenPkModel>(binder => binder.Where(a => a.Id == 2)).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UidPk()
		{
			var affrows = Context.Insert(new CreeperUuidPkModel
			{
				Age = 20,
				Name = "中国",
			});
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void InsertRangeMultiple()
		{
			var list = new List<CreeperIdenPkModel>();
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			list.Add(new CreeperIdenPkModel { Age = 20, Name = "中国" });
			var affrows = Context.InsertRange(list, false);
			Assert.Equal(list.Count, affrows);
			Context.InsertRange<CreeperIdenPkModel>().Set(list).ToListAffrowsResult();
			Assert.Throws<ArgumentNullException>(() => Context.InsertRange<CreeperIdenPkModel>(null));
			Assert.Throws<ArgumentNullException>(() => Context.InsertRange(Array.Empty<CreeperIdenPkModel>()));
		}

		[Fact]
		public void DoubleUniqueCompositePk()
		{
			var affrows = Context.Insert(new CreeperUidCompositePkModel
			{
				Name = "中国",
			});
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UniqueAndIdentityCompositePk()
		{
			var affrows = Context.Insert(new CreeperUuidIdenPkModel
			{
				Name = "中国",
				Age = 20,
			});
			Assert.Equal(1, affrows);
		}
	}
}
