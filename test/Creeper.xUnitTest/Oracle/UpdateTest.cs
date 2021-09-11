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
	public class UpdateTest : BaseTest, IUpdateTest
	{
		private long Id => Context.Select<IdenPkTestModel>().Max(a => a.Id);

		[Fact]
		public void Inc()
		{
			var info = Context.Select<ProductModel>().FirstOrDefault();
			var affrows = Context.Update(info).Inc(a => a.Stock, 20).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void ReturnAffrows()
		{
			var affrows = Context.Update<IdenPkTestModel>(a => a.Id == Id).Set(a => a.Name, "Sue").ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void SetEnumToInt()
		{
			var affrows = Context.Update<TypeTestModel>().Set(a => a.IntType, TestEnum.正常).Where(a => a.Id == 1).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UpdateModelCompositePk()
		{
			var info = Context.Select<IdenUidCompositePkTestModel>().FirstOrDefault();
			var affrows = Context.Update(info).Set(a => a.Name, "Sue").ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UpdateModels()
		{
			var info = Context.Select<IdenPkTestModel>().Take(2).ToList();
			var affrows = Context.UpdateRange(info).Set(a => a.Name, "Sue").ToAffrows();
			Assert.Equal(2, affrows);
		}

		[Fact]
		public void UpdateModelSinglePk()
		{
			var info = Context.Select<IdenPkTestModel>().FirstOrDefault();
			var affrows = Context.Update(info).Set(a => a.Name, "Sue").ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact(Skip = "Oracle暂不支持UPDATE RETURNING操作")]
		public void UpdateReturning()
		{
		}
	}
}
