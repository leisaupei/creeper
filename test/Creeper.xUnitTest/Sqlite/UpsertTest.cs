using System;
using System.Data.Common;
using System.Data.SQLite;
using Xunit;
using Creeper.Driver;
using Creeper.SqlBuilder;
using Creeper.Sqlite.Test.Entity.Model;
using System.ComponentModel;
using Creeper.xUnitTest.Extensions;
using Creeper.xUnitTest.Contracts;

namespace Creeper.xUnitTest.Sqlite
{
	public class UpsertTest : BaseTest, IUpsertTest
	{
		[Fact]
		[Description("单自增主键且包含唯一键的表")]
		public void SinglePkAndUniqueField()
		{
			var info = new IdenpkUniTestModel()
			{
				Name = "Tam",
				Idx_uni_column = SnowflakeId.Default().NextId().ToString(),
				Uni_column = SnowflakeId.Default().NextId().ToString(),
			};

			//var affrows = Context.Upsert(info);
			//Assert.Equal(1, affrows);

			var result = Context.Upsert(info);

			//Assert.Equal(-1, result.AffectedRows);
			//Assert.True(result.Value.Id > 0);
			//Assert.Equal(info.Idx_uni_column, result.Value.Idx_uni_column);
			//Assert.Equal(info.Uni_column, result.Value.Uni_column);

			//再次插入违反唯一键约束
			// 3.24.0+版本是用的是ON CONFLICT语法, 非主键唯一键(UniqueKey)不会并列为匹配条件, 所以包含唯一键的表需要保证数据库不包含此行.
			// 3.24.0-版本是用的是INSERT OR REPLACE语法, 非主键唯一键(UniqueKey)会成为匹配条件, 会存在唯一列冲突情况, 建议只留一个. 因此此处不会报错;
			var exception = Assert.ThrowsAny<CreeperNotSupportedException>(() =>
			{
				var result2 = Context.UpsertResult(info);
			});
		}

		[Fact]
		[Description("只有整型自增主键")]
		public void IdentityPk()
		{
			var info = new IdenTestModel
			{
				Id = 37,
				Name = "Sam",
			};
			var result = Context.Upsert(info);

			//Assert.Equal(-1, result.AffectedRows);
			//Assert.Equal(info.Id, result.Value.Id);

			var info1 = new IdenTestModel
			{
				Name = "Tam",
			};
			var result1 = Context.Upsert(info1);

			//Assert.Equal(-1, result.AffectedRows);
			//Assert.Equal(info1.Name, result1.Value.Name);
		}

		[Fact]
		[Description("只有字符串类/Guid型唯一id")]
		public void UidPk()
		{
			var info = new UniTestModel
			{
				Id = SnowflakeId.Default().NextId(),
				Name = "Tam"
			};
			var result = Context.Upsert(info);

			//Assert.Equal(-1, result.AffectedRows);
			//Assert.Equal(info.Id, result.Value.Id);
		}

		[Fact]
		[Description("唯一键复合主键, sqlite不能创建存在自增主键的复合主键表")]
		public void DoubleUniqueCompositePk()
		{
			var info = new UniCompositeModel
			{
				Uid1 = SnowflakeId.Default().NextId(),
				Uid2 = SnowflakeId.Default().NextId(),
				Name = "Tam"
			};
			var result = Context.Upsert(info);

			//Assert.Equal(-1, result.AffectedRows);
			//Assert.Equal(info.Uid1, result.Value.Uid1);
			//Assert.Equal(info.Uid2, result.Value.Uid2);
		}

		[Fact(Skip = "sqlite不能创建存在自增主键的复合主键表")]
		public void UniqueAndIdentityCompositePk()
		{

		}

	}
}
