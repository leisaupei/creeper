using Creeper.Extensions;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using System;
using Xunit;
using Xunit.Extensions.Ordering;
namespace Creeper.PostgreSql.XUnitTest
{
	[Order(4)]
	public class Update : BaseTest
	{
		[Fact]
		public void SetEnumToInt()
		{
			//var info = TypeTest.GetItem(Guid.Empty);
			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Enum_type, EDataState.正常).Where(a => (a.Int4_type > Math.Abs(-3) && a.Int4_type == 2) || (a.Int4_type == 2)).ToString();
		}

	}
}
