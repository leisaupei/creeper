using Creeper.Driver;
using Creeper.MySql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using Creeper.xUnitTest.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Creeper.xUnitTest.MySql
{
	public class InsertTest : BaseTest, IInsertTest
	{
		[Fact]
		[Description("插入时使用Where条件判断")]
		public void InsertWithWhere()
		{
			var ucolumn = Guid.NewGuid().ToString();
			var info = new IdPkUcolumnModel
			{
				Name = "Tam",
				U_column = ucolumn
			};
			var result = Context.Insert<IdPkUcolumnModel>().Set(info)
				.WhereExists<IdPkUcolumnModel>(binder => binder.Where(a => a.Name == "Tam"))
				.ToAffrowsResult();

			Assert.True(result.AffectedRows > 0);
			Assert.Equal(ucolumn, result.Value.U_column);
		}

		[Fact(Skip = "与功能无关")]
		[Description("自增单主键外加唯一列")]
		public void SinglePkAndUniqueField()
		{
			var ucolumn = Guid.NewGuid().ToString();
			var info = new IdPkUcolumnModel
			{
				Name = "Tam",
				U_column = ucolumn
			};
			var result = Context.InsertResult(info);

			Assert.True(result.AffectedRows > 0);
			Assert.Equal(ucolumn, result.Value.U_column);
		}

		[Fact]
		[Description("自增主键")]
		public void IdentityPk()
		{
			var info = new IidPkModel { Name = "Tam" };
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.True(result.Value.Id > 0);
		}

		[Fact]
		[Description("Guid作为主键(使用char(36)生成Guid类型)")]
		public void UidPk()
		{
			//如果是Guid类型, 可忽略Uid = Guid.NewGuid(), 自动生成
			var info = new UuidPkModel { Name = "Tam" };
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.True(result.Value.Uid != Guid.Empty);
		}

		[Fact]
		[Description("批量插入, 使用单条语句")]
		public void InsertRangeSingle()
		{
			//如果是Guid类型, 可忽略Uid = Guid.NewGuid(), 自动生成
			var list = new List<UuidPkModel>();
			list.Add(new UuidPkModel { Name = "Tam" });
			list.Add(new UuidPkModel { Name = "Tam" });
			list.Add(new UuidPkModel { Name = "Tam" });
			list.Add(new UuidPkModel { Name = "Tam" });
			list.Add(new UuidPkModel { Name = "Tam" });
			var affrows = Context.InsertRange(list);
			Assert.Equal(list.Count, affrows);
		}

		[Fact]
		[Description("批量插入, 使用多条语句")]
		public void InsertRangeMultiple()
		{
			var list = new List<UuidPkModel>();
			list.Add(new UuidPkModel { Name = "Tam" });
			list.Add(new UuidPkModel { Name = "Tam" });
			list.Add(new UuidPkModel { Name = "Tam" });
			list.Add(new UuidPkModel { Name = "Tam" });
			list.Add(new UuidPkModel { Name = "Tam" });
			var affrows = Context.InsertRange(list, false);
			Assert.Equal(list.Count, affrows);
		}

		[Fact(Skip = "UidPk已实现")]
		public void InsertReturning()
		{
		}

		[Fact]
		public void DoubleUniqueCompositePk()
		{
			var info = new CompositeUidPkModel
			{
				Id = SnowflakeId.Default().NextIdBase16(),
				Name = "Tam",
				U_id = SnowflakeId.Default().NextIdBase16()
			};
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.Equal(info.Id, result.Value.Id);
			Assert.Equal(info.U_id, result.Value.U_id);

		}

		[Fact]
		public void UniqueAndIdentityCompositePk()
		{
			var info = new CompositeUidIidPkModel
			{
				Name = "Tam",
				U_id = SnowflakeId.Default().NextIdBase16()
			};
			var result = Context.InsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.True(result.Value.Id > 0);
			Assert.Equal(info.U_id, result.Value.U_id);
		}
	}
}
