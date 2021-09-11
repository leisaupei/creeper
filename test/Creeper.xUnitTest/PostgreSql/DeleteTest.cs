using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions.Ordering;
using System.Data.Common;
using Npgsql;
using Creeper.SqlBuilder;
using System.Reflection;
using Creeper.Utils;
using Creeper.Driver;
using Creeper.PostgreSql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;

namespace Creeper.xUnitTest.PostgreSql
{
	public class DeleteTest : BaseTest, IDeleteTest
	{
		[Fact]
		public void DeleteCondition()
		{
			var info = Context.Select<CreeperIdenPkModel>().Take(1).FirstOrDefault();
			var affrows = Context.Delete<CreeperIdenPkModel>().Where(a => a.Id == info.Id).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void DeleteOne()
		{
			var info = Context.Select<CreeperIdenPkModel>().Take(1).FirstOrDefault();
			var affrows = Context.Delete(info);
			Assert.Equal(1, affrows);
		}
		[Fact]
		public void DeleteRange()
		{
			var infos = Context.Select<CreeperIdenPkModel>().Take(2).ToList();
			var affrows = Context.DeleteRange(infos);
			Assert.Equal(2, affrows);
		}
	}
}
