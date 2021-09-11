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
	public class UpsertTest : BaseTest, IUpsertTest
	{
		[Fact]
		[Description("单自增主键且包含唯一键的表")]
		public void SinglePkAndUniqueField()
		{
			var uColumn = Guid.NewGuid().ToString();
			var info = new IdenPkUniColModel()
			{
				Name = "Tam",
				UniqueColumn = uColumn,
			};
			var result = Context.UpsertResult(info);

			Assert.True(result.AffectedRows > 0);
			Assert.True(result?.Value.Id > 0);
			Assert.Equal(uColumn, result.Value.UniqueColumn);

			//再次插入违反唯一键约束
			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				IdenPkUniColModel result2 = Context.UpsertResult(info);
			});
			Assert.IsType<Microsoft.Data.SqlClient.SqlException>(exception.InnerException);
		}

		[Fact]
		[Description("只有int类型自增主键")]
		public void IdentityPk()
		{
			var info = new IdenPkModel
			{
				Name = "Tam",
			};
			var result = Context.UpsertResult(info);
			Assert.True(result.AffectedRows > 0);
			Assert.True(result.Value?.Id > 0);
		}

		[Fact]
		[Description("只有字符串类/Guid型唯一id")]
		public void UidPk()
		{
			var info = new GuidPkModel
			{
				Id = Guid.NewGuid(),
				Name = "Tam"
			};
			var result = Context.UpsertResult(info);
			Assert.True(result.AffectedRows > 0);
			Assert.Equal(info.Id, result.Value?.Id);
		}

		[Fact]
		[Description("字符串/Guid类型与int自增类型复合主键")]
		public void UniqueAndIdentityCompositePk()
		{
			var info = new IdenGuidCompositeModel
			{
				Uid = Guid.NewGuid(),
				Name = "Tam"
			};
			var result = Context.UpsertResult(info);
			Assert.True(result.AffectedRows > 0);
			Assert.Equal(info.Uid, result.Value?.Uid);
			Assert.True(result.Value?.Iid > 0);
		}

		[Fact]
		[Description("字符串/Guid类型复合主键")]
		public void DoubleUniqueCompositePk()
		{
			var info = new GuidCompositeModel
			{
				Gid = Guid.NewGuid(),
				Uid = Guid.NewGuid(),
				Name = "Tam"
			};
			var result = Context.UpsertResult(info);
			Assert.True(result.AffectedRows >= 0);
			Assert.Equal(info.Uid, result.Value?.Uid);
		}
	}
}
