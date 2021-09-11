using Xunit;
using System.Data.Common;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using Creeper.xUnitTest.Contracts;
using System;
using Creeper.Driver;
using Creeper.Access2007.Test.Entity.Model;

namespace Creeper.xUnitTest.Access.v2007
{
	public class DeleteTest : BaseTest, IDeleteTest
	{
		[Fact]
		public void DeleteCondition()
		{
			var info = Context.Select<IdenPkTestModel>().OrderBy($"rnd(-Timer()*id)").FirstOrDefault();
			var affrows = Context.Delete<IdenPkTestModel>().Where(a => a.Id == info.Id).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void DeleteOne()
		{
			var info = Context.Select<IdenPkTestModel>().OrderBy($"rnd(-Timer()*id)").FirstOrDefault();
			var affrows = Context.Delete(info);
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void DeleteRange()
		{
			var info = Context.Select<IdenPkTestModel>().OrderBy($"rnd(-Timer()*id)").Take(2).ToList();
			var affrows = Context.DeleteRange(info);
			Assert.Equal(2, affrows);
		}

	}
}
