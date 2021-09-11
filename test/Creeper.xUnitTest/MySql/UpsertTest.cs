using Creeper.Driver;
using Creeper.MySql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using Creeper.xUnitTest.Extensions;
using MySql.Data.MySqlClient;
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
	public class UpsertTest : BaseTest, IUpsertTest
	{
		[Fact, Description("单主键且包含唯一键的表, 此处非主键唯一键(UniqueKey)会成为匹配条件, 会存在唯一列冲突情况, 建议只留一个")]
		public void SinglePkAndUniqueField()
		{
			var uColumn = Guid.NewGuid().ToString();
			var info = new IdPkUcolumnModel
			{
				Name = "Tam",
				U_column = uColumn,
			};
			IdPkUcolumnModel result = Context.UpsertResult(info);
			Assert.True(result?.Id >= 0);

			if (result?.Id > 0)
				Assert.Equal(uColumn, result.U_column);
		}

		[Fact, Description("只有int类型自增主键")]
		public void IdentityPk()
		{
			var info = new IidPkModel
			{
				Id = 37,
				Name = "Xam",
			};
			var result = Context.UpsertResult(info);
			Assert.True(result.AffectedRows > 0);
			if (result.AffectedRows == 2)
				Assert.True(info.Id == result.Value.Id);

			var info1 = new IidPkModel
			{
				Name = "Tam",
			};
			var result1 = Context.UpsertResult(info1);
			Assert.Equal(1, result1.AffectedRows);
			Assert.Equal(info1.Name, result1.Value.Name);
		}

		[Fact]
		[Description("只有字符串类型唯一id")]
		public void UidPk()
		{
			var info = new UidPkModel
			{
				Id = "1",
				Name = "Tam"
			};
			var result = Context.UpsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.True(!string.IsNullOrEmpty(result.Value?.Id));
		}

		[Fact, Description("字符串类型与int自增类型复合主键")]
		public void UniqueAndIdentityCompositePk()
		{
			var info = new CompositeUidIidPkModel
			{
				U_id = SnowflakeId.Default().NextIdBase16(),
				Name = "Tam"
			};
			var result = Context.UpsertResult(info);
			Assert.Equal(1, result.AffectedRows);
			Assert.True(!string.IsNullOrEmpty(result?.Value.U_id) && result.Value?.Id >= 0);
		}

		[Fact, Description("字符串类型复合主键")]
		public void DoubleUniqueCompositePk()
		{
			var info = new CompositeUidPkModel
			{
				Id = Guid.NewGuid().ToString(),
				U_id = Guid.NewGuid().ToString(),
				Name = "Tam"
			};
			CompositeUidPkModel result = Context.UpsertResult(info);
			Assert.True(!string.IsNullOrEmpty(result?.U_id) && !string.IsNullOrEmpty(result?.Id));
		}
	}
}
