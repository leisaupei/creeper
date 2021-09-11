using Creeper.Driver;
using Creeper.SqlServer.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using Creeper.xUnitTest.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Creeper.xUnitTest.SqlServer
{
	public class UpdateTest : BaseTest, IUpdateTest
	{
		private const int Id = 1;

		[Fact]
		[Description("更新返回受影响行数")]
		public void ReturnAffrows()
		{
			var affrows = Context.Update<StudentModel>().Where(a => a.Id == Id).Set(a => a.Name, "测试").ToAffrows();
			affrows = Context.Update<StudentModel>(a => a.Id == Id).Set(a => a.Name, "测试").ToAffrows();
			Assert.True(affrows >= 0);
		}

		[Fact]
		[Description("更新条件传入实体类, 则使用主键作为条件")]
		public void UpdateModelSinglePk()
		{
			var info = Context.Select<StudentModel>(a => a.Id == 1).FirstOrDefault();
			if (info != null)
			{
				var affrows = Context.Update(info).Set(a => a.Name, "测试").ToAffrows();
				Assert.True(affrows >= 0);
			}
		}

		[Fact]
		[Description("更新返回受影响行")]
		public void UpdateReturning()
		{
			var info = Context.Select<StudentModel>(a => a.Id == 1).FirstOrDefault();
			if (info != null)
			{
				var result = Context.Update(info).Set(a => a.Name, "小明").ToAffrowsResult();
				Assert.True(result.AffectedRows >= 0);
				Assert.Equal("小明", result.Value.Name);
			}
		}

		[Fact]
		public void UpdateModels()
		{
			var infos = Context.Select<StudentModel>().OrderByDescending(a => a.Id).Take(2).ToList();

			var affrows = Context.UpdateRange(infos).Set(a => a.Name, "测试").ToAffrows();
			Assert.True(affrows >= 0);
		}

		[Fact]
		public void UpdateModelCompositePk()
		{
			var infos = Context.Select<GuidCompositeModel>().OrderByDescending(a => a.Uid).Take(2).ToList();
			var affrows = Context.UpdateRange(infos).Set(a => a.Name, "测试").ToAffrows();
			Assert.True(affrows >= 0);
		}

		[Fact]
		public void SetEnumToInt()
		{
			var affrows2 = Context.Update<TypeTestModel>(a => a.Id == 11).Set(a => a.IntType, (Enum)null).ToAffrows();
			var affrows1 = Context.Update<TypeTestModel>(a => a.Id == 11).Set(a => a.IntType, TestEnum.正常).ToAffrows();
			Assert.Equal(1, affrows1);
			Assert.Equal(1, affrows2);
		}

		[Fact]
		public void Inc()
		{
			var affrows1 = Context.Update<TypeTestModel>(a => a.Id == 11).Inc(a => a.IntType, 20, 0).ToAffrows();
			//sqlserver datetime类型不能与time类型运算
			//var affrows2 = Context.Update<TypeTestModel>(a => a.Id == 11).Inc(a => a.DateTimeType, TimeSpan.FromHours(1), DateTime.Parse("2021-9-1")).ToAffrows();
			//var affrows3 = Context.Update<TypeTestModel>(a => a.Id == 11).Inc(a => a.TimeType, TimeSpan.FromHours(1), TimeSpan.Zero).ToAffrows();

			Assert.Equal(1, affrows1);
			//Assert.Equal(1, affrows2);
			//Assert.Equal(1, affrows3);
		}
	}
}
