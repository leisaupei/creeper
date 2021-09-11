using Xunit;
using System.Data.Common;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using Creeper.xUnitTest.Contracts;
using System;
using Creeper.Driver;
using Creeper.Access2007.Test.Entity.Model;
using System.ComponentModel;

namespace Creeper.xUnitTest.Access.v2003
{
	public class SelectTest : BaseTest
	{
		[Theory]
		[InlineData(1, 10)]
		[InlineData(2, 10)]
		[Description("Access分页时必须传入排序字段, 第一页可不传, 也就是说存在传统意义的offset时会判断排序字段是否存在")]
		public void Page(int index, int size)
		{
			Assert.Throws<CreeperException>(() =>
				{
					var result1 = Context.Select<UniPkTestModel>().Page(index, size).ToList();
				});
			var result2 = Context.Select<UniPkTestModel>().OrderByDescending(a => a.Id).Page(index, size).ToList();
		}

		[Fact]
		public void Limit()
		{
			var result1 = Context.Select<UniPkTestModel>().Take(10).ToList();
			var result2 = Context.Select<UniPkTestModel>().OrderByDescending(a => a.Id).Take(10).ToList();
		}

		[Fact(Skip = "鉴于Access分页方式比较局限, 分页必须传入页码, 不允许单独跳过前n条数据")]
		public void Skip()
		{
			Assert.Throws<CreeperException>(() =>
			{
				var result2 = Context.Select<UniPkTestModel>().OrderByDescending(a => a.Id).Skip(10).ToList();
			});
		}
	}
}
