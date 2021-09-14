using Creeper.Driver;
using Creeper.Extensions;
using Creeper.PostgreSql.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using System;
using System.ComponentModel;
using Xunit;
namespace Creeper.xUnitTest.PostgreSql
{
	public class UpdateTest : BaseTest, IUpdateTest
	{
		[Fact]
		public void Inc()
		{
			var affrows = Context.Update<CreeperTypeTestModel>(a => a.Id == Guid.Empty).Inc(a => a.Int2_type, 12, defaultValue: 0).ToAffrows();
			Assert.Equal(1, affrows);
		}


		[Fact, Description("从数组中移除元素")]
		public void Remove()
		{
			var affrows = Context.Update<CreeperTypeTestModel>(a => a.Id == Guid.Empty).Remove(a => a.Array_type, 1).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact, Description("添加元素到数组")]
		public void Append()
		{
			var affrows = Context.Update<CreeperTypeTestModel>(a => a.Id == Guid.Empty).Append(a => a.Array_type, 1, 1, 2, 3).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void ReturnAffrows()
		{
			var affrows = Context.Update<CreeperTypeTestModel>(a => a.Id == Guid.Empty).Set(a => a.Varchar_type, "中国").ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void SetEnumToInt()
		{
			var affrows = Context.Update<CreeperTypeTestModel>().Set(a => a.Int2_type, CreeperDataState.正常).Where(a => a.Id == Guid.Empty).ToAffrows();
			Assert.Equal(1, affrows);
			affrows = Context.Update<CreeperTypeTestModel>().Set(a => a.Int4_type, CreeperDataState.正常).Where(a => a.Id == Guid.Empty).ToAffrows();
			Assert.Equal(1, affrows);
		}
		[Fact]
		public void UpdateModelCompositePk()
		{
			var info = Context.Select<CreeperUidCompositePkModel>().FirstOrDefault();
			var affrows = Context.Update(info).Set(a => a.Name, "Sam").ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UpdateModels()
		{
			var infos = Context.Select<CreeperUidCompositePkModel>().Take(2).ToList();
			var affrows = Context.UpdateRange(infos).Set(a => a.Name, "Sam").ToAffrows();
			Assert.Equal(2, affrows);
		}

		[Fact]
		public void UpdateModelSinglePk()
		{
			var info = Context.Select<CreeperGradeModel>().FirstOrDefault();
			var affrows = Context.Update(info).Set(a => a.Name, "信息工程").ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UpdateReturning()
		{
			var value = "信息工程";
			var info = Context.Select<CreeperGradeModel>().FirstOrDefault();
			var result = Context.Update(info).Set(a => a.Name, value).ToAffrowsResult();
			Assert.Equal(1, result.AffectedRows);
			Assert.Equal(value, result.Value.Name);
		}

		[Fact]
		public void UpdateSave()
		{
			var info = Context.Select<CreeperIdenPkModel>().Where(a => a.Name != "Trick").FirstOrDefault();
			info.Name = "Trick";
			var affrows = Context.UpdateSave(info);
			Assert.Equal(1, affrows);
		}
	}
}
