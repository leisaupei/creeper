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
			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Int2_type, EtDataState.正常).Where(a => a.Id == Guid.Empty).ToAffectedRows();
			affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Int4_type, EtDataState.正常).Where(a => a.Id == Guid.Empty).ToAffectedRows();
		}
		[Fact]
		public void UpdateFromModel()
		{
			var model = new ClassGradeModel
			{
				Create_time = DateTime.Now,
				Id = Guid.Parse("81d58ab2-4fc6-425a-bc51-d1d73bf9f4b1"),
				Name = "软件技术",
			};
			var affrows = _dbContext.Update(model);
		}
	}
}
