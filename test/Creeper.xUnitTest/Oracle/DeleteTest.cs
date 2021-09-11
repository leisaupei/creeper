using Creeper.Driver;
using Creeper.Oracle.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.Generic;
using Xunit;

namespace Creeper.xUnitTest.Oracle
{
	public class DeleteTest : BaseTest, IDeleteTest
	{
		private IdenPkTestModel Info => Context.Select<IdenPkTestModel>().FirstOrDefault();
		[Fact]
		public void DeleteCondition()
		{
			var affrows = Context.Delete<IdenPkTestModel>(a => a.Id == Info.Id);
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void DeleteOne()
		{
			var affrows = Context.Delete(Info);
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void DeleteRange()
		{
			var infos = Context.Select<IdenPkTestModel>().Take(2).ToList();
			var affrows = Context.DeleteRange(infos);
			Assert.Equal(2, affrows);
		}
	}
}
