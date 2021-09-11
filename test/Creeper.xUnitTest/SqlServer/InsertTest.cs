using Creeper.Driver;
using Creeper.SqlServer.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Creeper.xUnitTest.SqlServer
{
	public class InsertTest : BaseTest, IInsertTest
	{
		[Fact]
		[Description("测试自增主键")]
		public void IdentityPk()
		{
			var info = new IdenPkModel { Name = "中国人" };
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.True(result.Value.Id > 0);
		}

		[Fact]
		[Description("自增+唯一复合主键")]
		public void UniqueAndIdentityCompositePk()
		{
			var info = new IdenGuidCompositeModel { Name = "中国人" };
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.True(result.Value.Iid > 0);
			Assert.True(result.Value.Uid != Guid.Empty);
		}

		[Fact]
		[Description("批量插入, 使用多条语句")]
		public void InsertRangeMultiple()
		{

			var list = new List<StudentModel>();
			var count = (int)Context.Select<StudentModel>().Count();

			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });

			var affrows = Context.InsertRange(list, false);
			Assert.Equal(list.Count, affrows);
			Assert.Equal(count, (int)Context.Select<StudentModel>().Count());
		}

		[Fact]
		[Description("批量插入, 使用单条语句")]
		public void InsertRangeSingle()
		{
			var list = new List<StudentModel>();
			var count = (int)Context.Select<StudentModel>().Count();

			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });
			list.Add(new StudentModel() { CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(), Name = "中国人", StuNo = DateTime.Now.Year * 1000 + count++ });

			var affrows = Context.InsertRange(list);
			Assert.Equal(list.Count, affrows);
			Assert.Equal(count, (int)Context.Select<StudentModel>().Count());

		}

		[Fact]
		[Description("插入并返回受影响行")]
		public void InsertReturning()
		{
			var stu = new StudentModel()
			{
				CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
				Name = "张三",
				StuNo = (int)Context.Select<StudentModel>().Count() + 1
			};
			var result = Context.InsertResult(stu);
			Assert.Equal(1, result.AffectedRows);
			Assert.Equal(stu.Name, result.Value.Name);
		}

		[Fact]
		[Description("插入语句使用Where条件")]
		public void InsertWithWhere()
		{
			var stu = new StudentModel()
			{
				CreateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
				Name = "张三",
				StuNo = DateTime.Now.Year * 1000 + ((int)Context.Select<StudentModel>().Count() + 1)
			};
			var result = Context.Insert<StudentModel>().Set(stu).WhereExists<StudentModel>(binder => binder.Where(a => a.Id == 1)).ToAffrows();
			Assert.True(result >= 0);
		}

		[Fact]
		[Description("唯一类型复合主键")]
		public void DoubleUniqueCompositePk()
		{
			var info = new GuidCompositeModel { Name = "中国人" };
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.True(result.Value.Gid != Guid.Empty);
			Assert.True(result.Value.Uid != Guid.Empty);
		}

		[Fact]
		[Description("测试随机主键")]
		public void UidPk()
		{
			var info = new GuidPkModel { Name = "中国人" };
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.True(result.Value.Id != Guid.Empty);
		}
	}
}
