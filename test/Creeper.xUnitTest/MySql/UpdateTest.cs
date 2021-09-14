using Creeper.Driver;
using System.ComponentModel;
using Xunit;
using Creeper.MySql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using Creeper.xUnitTest.Extensions;

namespace Creeper.xUnitTest.MySql
{
	public class UpdateTest : BaseTest, IUpdateTest
	{
		[Fact]
		[Description("更新返回受影响行数")]
		public void ReturnAffrows()
		{
			var affrows = Context.Update<StudentModel>().Where(a => a.Id == 1).Set(a => a.Class_name, "测试").ToAffrows();
			affrows = Context.Update<StudentModel>(a => a.Id == 1).Set(a => a.Class_name, "测试").ToAffrows();
			Assert.True(affrows >= 0);
		}

		[Fact]
		[Description("更新条件传入实体类, 则使用主键作为条件")]
		public void UpdateModelSinglePk()
		{
			var info = Context.Select<StudentModel>(a => a.Id == 1).FirstOrDefault();
			if (info != null)
			{
				var affrows = Context.Update(info).Set(a => a.Class_name, "测试").ToAffrows();
				Assert.True(affrows >= 0);
			}
		}

		[Fact]
		public void UpdateModelCompositePk()
		{
			var infos = Context.Select<CompositeUidPkModel>().OrderByDescending(a => a.Id).Take(2).ToList();
			var affrows = Context.UpdateRange(infos).Set(a => a.Name, "测试").ToAffrows();
			Assert.True(affrows >= 0);
		}

		[Fact]
		[Description("批量更新条件传入多个实体类, 则使用主键作为条件")]
		public void UpdateModels()
		{
			var infos = Context.Select<StudentModel>().OrderByDescending(a => a.Id).Take(2).ToList();
			var affrows = Context.UpdateRange(infos).Set(a => a.Class_name, "测试").ToAffrows();
			Assert.True(affrows >= 0);
		}

		[Fact]
		[Description("更新包含自增主键表返回修改行")]
		public void UpdateReturning()
		{
			var result = Context.Update<IidPkModel>(a => a.Id == 1).Set(a => a.Name, "测试").ToAffrowsResult();
			Assert.True(result.AffectedRows >= 0);
			Assert.True(result.Value.Name == "测试");
		}

		[Fact]
		public void SetEnumToInt()
		{
			var info = Context.Select<TypeTestModel>().FirstOrDefault();
			var result = Context.Update(info).Set(a => a.Integer_t, TestEnum.正常).ToAffrowsResult();

			Assert.Equal(1, result.AffectedRows);
			Assert.Equal((int)TestEnum.正常, result.Value.Integer_t);
		}

		[Fact]
		public void Inc()
		{
			var info = Context.Select<PeopleModel>().FirstOrDefault();
			var result = Context.Update(info).Inc(a => a.Age, 10, 0).ToAffrowsResult();
			Assert.Equal(1, result.AffectedRows);
			Assert.Equal((info.Age ?? 0) + 10, result.Value.Age);
		}

		[Fact]
		public void UpdateSave()
		{
			var info = Context.Select<PeopleModel>().Where(a => a.Name != "Trick").FirstOrDefault();
			info.Name = "Trick";
			var affrows = Context.UpdateSave(info);
			Assert.Equal(1, affrows);
		}
	}
}
