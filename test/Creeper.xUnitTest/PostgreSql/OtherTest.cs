using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Creeper.xUnitTest.PostgreSql
{
	public class OtherTest : BaseTest, IOtherTest
	{
		[Fact]
		public void ExecutePipe()
		{
			var total = 0L;
			Context.ExecutePipe(execute =>
			{
				total = execute.Select<CreeperTypeTestModel>().Sum(a => a.Int8_type, 0);
				execute.Transaction(tranExecute =>
				{
					total = tranExecute.Select<CreeperTypeTestModel>().Sum(a => a.Int8_type, 0);
					var affrows = tranExecute.Update<CreeperTypeTestModel>().Set(a => a.Int8_type, 0).Where(a => a.Id == Guid.Empty).ToAffrows();
				});

				total = execute.Select<CreeperTypeTestModel>().Sum(a => a.Int8_type, 0);
			}, DataBaseType.Main);
		}

		[Fact]
		public async Task TransationAsync()
		{
			await Context.TransactionAsync(tranExecute =>
			{
				var total = Context.Select<CreeperTypeTestModel>().Sum(a => a.Int8_type, 0);
				var affrows = Context.Update<CreeperTypeTestModel>().Set(a => a.Int8_type, 0).Where(a => a.Id == Guid.Empty).ToAffrows();
			});
			Context.Transaction(tranExecute =>
			{
				var total = Context.Select<CreeperTypeTestModel>().Sum(a => a.Int8_type, 0);
				var affrows = Context.Update<CreeperTypeTestModel>().Set(a => a.Int8_type, 0).Where(a => a.Id == Guid.Empty).ToAffrows();
			});
		}
	}
}
