using Creeper.Driver;
using Creeper.Sqlite.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Creeper.xUnitTest.Sqlite
{
	public class DeleteTest : BaseTest, IDeleteTest
	{
		[Fact]
		[Description("根据条件删除")]
		public void DeleteCondition()
		{
			var id = Context.Select<IdenTestModel>().Max(a => a.Id);
			var affrows = Context.Delete<IdenTestModel>(a => a.Id == id);
			Assert.Equal(1, affrows);
		}

		[Fact]
		[Description("删除单个对象, 根据主键获取删除条件")]
		public void DeleteOne()
		{
			var info = Context.Select<IdenTestModel>().OrderByDescending(a => a.Id).FirstOrDefault();
			var affrows = Context.Delete(info);
			Assert.Equal(1, affrows);
		}

		[Fact]
		[Description("删除多个对象, 根据主键获取删除条件")]
		public void DeleteRange()
		{
			var info = Context.Select<IdenTestModel>().OrderByDescending(a => a.Id).Take(2).ToList();
			var affrows = Context.DeleteRange(info);
			Assert.Equal(2, affrows);
		}
	}
}
