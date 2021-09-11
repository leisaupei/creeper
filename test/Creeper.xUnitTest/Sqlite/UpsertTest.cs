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
		[Description("�����������Ұ���Ψһ���ı�")]
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

			//�ٴβ���Υ��Ψһ��Լ��
			// 3.24.0+�汾���õ���ON CONFLICT�﷨, ������Ψһ��(UniqueKey)���Ტ��Ϊƥ������, ���԰���Ψһ���ı���Ҫ��֤���ݿⲻ��������.
			// 3.24.0-�汾���õ���INSERT OR REPLACE�﷨, ������Ψһ��(UniqueKey)���Ϊƥ������, �����Ψһ�г�ͻ���, ����ֻ��һ��. ��˴˴����ᱨ��;
			var exception = Assert.ThrowsAny<CreeperNotSupportedException>(() =>
			{
				var result2 = Context.UpsertResult(info);
			});
		}

		[Fact]
		[Description("ֻ��������������")]
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
		[Description("ֻ���ַ�����/Guid��Ψһid")]
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
		[Description("Ψһ����������, sqlite���ܴ����������������ĸ���������")]
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

		[Fact(Skip = "sqlite���ܴ����������������ĸ���������")]
		public void UniqueAndIdentityCompositePk()
		{

		}

	}
}
