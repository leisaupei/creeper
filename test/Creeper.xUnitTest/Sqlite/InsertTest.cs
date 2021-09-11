using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Creeper.Sqlite.Test.Entity.Model;
using Creeper.Driver;
using Creeper.xUnitTest.Extensions;
using Creeper.xUnitTest.Contracts;

namespace Creeper.xUnitTest.Sqlite
{
	public class InsertTest : BaseTest, IInsertTest
	{
		[Fact]
		[Description("批量插入, 使用单条语句")]
		public void InsertRangeSingle()
		{
			var list = new List<IdenTestModel>();
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			var affrows = Context.InsertRange(list);
			Assert.Equal(list.Count, affrows);
		}

		[Fact]
		[Description("批量插入, 使用多条语句")]
		public void InsertRangeMultiple()
		{
			var list = new List<IdenTestModel>();
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			list.Add(new IdenTestModel { Name = "中国" });
			var affrows = Context.InsertRange(list, singleCommand: false);
			Assert.Equal(list.Count, affrows);
		}

		[Fact]
		[Description("插入使用Where语句")]
		public void InsertWithWhere()
		{
			var info = new IdenTestModel { Name = "中国" };
			var result = Context.Insert<IdenTestModel>().Set(info).WhereExists<IdenTestModel>(binder => binder.Where(a => a.Id == 1)).ToAffrowsResult();
			Assert.Equal(1, result.AffectedRows);
		}

		[Fact]
		[Description("插入返回插入行, 自增主键测试")]
		public void IdentityPk()
		{
			var info = new IdenTestModel { Name = "中国" };
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.Equal(info.Name, result.Value.Name);
		}

		[Fact]
		[Description("插入返回插入行, 随机唯一主键测试")]
		public void UidPk()
		{
			var id = SnowflakeId.Default().NextId();
			var info = new UniTestModel { Id = id, Name = "中国" };
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.Equal(info.Name, result.Value.Name);
			Assert.Equal(id, result.Value.Id);
		}

		[Fact(Skip = "UidPk已实现")]
		public void InsertReturning()
		{

		}

		[Fact]
		public void DoubleUniqueCompositePk()
		{
			var info = new UniCompositeModel
			{
				Uid1 = SnowflakeId.Default().NextId(),
				Uid2 = SnowflakeId.Default().NextId(),
				Name = "Tam"
			};
			var result = Context.InsertResult(info);

			Assert.Equal(1, result.AffectedRows);
			Assert.Equal(info.Uid1, result.Value.Uid1);
			Assert.Equal(info.Uid2, result.Value.Uid2);
		}

		[Fact(Skip = "sqlite不能创建存在自增主键的复合主键表")]
		public void UniqueAndIdentityCompositePk()
		{
		}
	}
}
