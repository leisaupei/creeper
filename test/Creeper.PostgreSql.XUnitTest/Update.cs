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
			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Int2_type, EDataState.正常).Where(a => a.Id == Guid.Empty).ToAffectedRows();
			affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Int4_type, EDataState.正常).Where(a => a.Id == Guid.Empty).ToAffectedRows();
		}

	}
}
