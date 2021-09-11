using Creeper.Driver;
using Creeper.Oracle.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using Creeper.xUnitTest.Extensions;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;
using Xunit;

namespace Creeper.xUnitTest.Oracle
{
	public class UpsertTest : BaseTest, IUpsertTest
	{
		[Fact]
		public void DoubleUniqueCompositePk()
		{
			var result = Context.Upsert(new UidCompositePkTestModel
			{
				Id = SnowflakeId.Default().NextIdBase16(),
				Name = "sam",
				Id2 = SnowflakeId.Default().NextIdBase16(),
			});
			Assert.Equal(1, result);
		}

		[Fact]
		public void UidPk()
		{
			var affrows = Context.Upsert(new UniPkTestModel
			{
				Id = SnowflakeId.Default().NextIdBase16(),
				Name = "Xam",
			});
			Assert.Equal(1, affrows);
		}
		[Fact]
		public void IdentityPk()
		{
			//此表格类型是使用触发器触发序列
			var affrows = Context.Upsert(new IdenPkTestModel
			{
				Name = "Xam",
			});
			Assert.Equal(1, affrows);

			//此表使用generated by default as identity(序列+Default)
			affrows = Context.Upsert(new IdenColumnTestModel
			{
				Name = "Xam",
			});
			Assert.Equal(1, affrows);
		}
		[Fact]
		public void SinglePkAndUniqueField()
		{
			var affrows = Context.Upsert(new UniKeyTestModel
			{
				IdxUniqueKeyTest = SnowflakeId.Default().NextIdBase16(),
				UniqueKeyTest = SnowflakeId.Default().NextIdBase16(),
			});
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UniqueAndIdentityCompositePk()
		{
			var result = Context.Upsert(new IdenUidCompositePkTestModel
			{
				Uid = SnowflakeId.Default().NextIdBase16(),
				Name = "xxx"
			});
			Assert.Equal(1, result);
		}
	}
}
