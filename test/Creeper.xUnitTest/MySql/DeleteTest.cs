using Creeper.Driver;
using Creeper.MySql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using System.ComponentModel;
using Xunit;

namespace Creeper.xUnitTest.MySql
{
	public class DeleteTest : BaseTest, IDeleteTest
	{
		[Fact]
		[Description("删除单行")]
		public void DeleteOne()
		{
			var info = Context.Select<PeopleModel>(a => a.Id == -1).FirstOrDefault();
			if (info != null)
			{
				var affrows = Context.Delete(info);
				Assert.True(affrows >= 0);
			}
		}

		[Fact]
		[Description("删除多行")]
		public void DeleteRange()
		{
			var list = Context.Select<PeopleModel>(a => a.Id == -1).ToList();
			if (list.Count > 0)
			{
				var affrows = Context.DeleteRange(list);
				Assert.True(affrows >= 0);
			}
		}

		[Fact]
		[Description("根据条件筛选删除")]
		public void DeleteCondition()
		{
			var affrows = Context.Delete<PeopleModel>(a => a.Id == -1);
			affrows = Context.Delete<PeopleModel>().Where(a => a.Id == -1).ToAffrows();
			Assert.True(affrows >= 0);
		}
	}
}
