using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using System.Data.Common;
using Npgsql;
using Creeper.SqlBuilder;
using System.Reflection;
using Creeper.Extensions;
using Creeper.Driver;
using Creeper.PostgreSql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;

namespace Creeper.xUnitTest.PostgreSql
{
	public class UpsertTest : BaseTest, IUpsertTest
	{
		[Theory, Description("Guid主键, 包含一个自增字段")]
		[InlineData("00000000-0000-0000-0000-000000000000")]
		[InlineData("7707d5e2-0ed0-4f80-931a-1d766028df08")]
		public void GuidPkWithIdentityField(Guid id)
		{
			var affrows = Context.Upsert(new CreeperIdenNopkModel
			{
				Id = id,
				Age = 20,
				Name = "小云"
			});
			Assert.True(affrows > 0);
		}

		[Fact]
		public void UidPk()
		{
			GuidPk(Guid.Empty);
			GuidPk(Guid.Parse("7707d5e2-0ed0-4f80-931a-1d766028df08"));
		}

		private void GuidPk(Guid id)
		{
			var affrows = Context.Upsert(new CreeperUuidPkModel
			{
				Id = id,
				Age = 20,
				Name = "小明"
			});
			Assert.True(affrows > 0);
		}

		[Fact]
		public void IdentityPk()
		{
			TestIdentityPk(default);
			TestIdentityPk(2);
		}

		private void TestIdentityPk(int id)
		{
			var affrows = Context.Upsert(new CreeperIdenPkModel
			{
				Id = id,
				Age = 20,
				Name = "小云"
			});
			Assert.True(affrows > 0);
		}

		[Fact]
		public void SinglePkAndUniqueField()
		{
			var exception = Assert.ThrowsAny<CreeperException>(() =>
			{
				var info = new CreeperStudentModel
				{
					Id = Guid.NewGuid(),
					Stu_no = "1333333",
					Create_time = DateTime.Now,
					Grade_id = Guid.Parse("81d58ab2-4fc6-425a-bc51-d1d73bf9f4b1"),
					People_id = Guid.Parse("00000000-0000-0000-0000-000000000000")
				};
				var result = Context.Upsert(info);
			});
			Assert.IsType<PostgresException>(exception.InnerException);
		}

		[Fact]
		public void UniqueAndIdentityCompositePk()
		{
			GuidIdentityPk(Guid.Parse("00000000-0000-0000-0000-000000000000"), 0);
			GuidIdentityPk(Guid.Parse("7707d5e2-0ed0-4f80-931a-1d766028df08"), 1);
			GuidIdentityPk(Guid.Parse("00000000-0000-0000-0000-000000000000"), 2);
		}

		private void GuidIdentityPk(Guid id, int idenId)
		{
			var affrows = Context.Upsert(new CreeperUuidIdenPkModel
			{
				Id = id,
				Id_sec = idenId,
				Age = 20,
				Name = "小明"
			});
			Assert.True(affrows > 0);
		}

		[Fact]
		public void DoubleUniqueCompositePk()
		{
			var info = new CreeperUidCompositePkModel
			{
				Uid1 = Guid.NewGuid(),
				Uid2 = Guid.NewGuid(),
				Name = "Tam"
			};
			var result = Context.UpsertResult(info);
			Assert.True(result.AffectedRows >= 0);
			Assert.Equal(info.Uid1, result.Value?.Uid1);
			Assert.Equal(info.Uid2, result.Value?.Uid2);
		}
	}
}
