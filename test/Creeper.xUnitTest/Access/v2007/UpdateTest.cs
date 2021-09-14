using Xunit;
using System.Data.Common;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using Creeper.xUnitTest.Contracts;
using System;
using Creeper.Extensions;
using Creeper.Access2007.Test.Entity.Model;
using Creeper.Driver;
using Creeper.xUnitTest.Extensions;

namespace Creeper.xUnitTest.Access.v2007
{
	public class UpdateTest : BaseTest, IUpdateTest
	{
		[Fact]
		public void ReturnAffrows()
		{
			var info = Context.Select<UniPkTestModel>().OrderByDescending(a => a.Id).Take(1).FirstOrDefault();
			var affrows = Context.Update<UniPkTestModel>(a => a.Id == info.Id).Set(a => a.Name, "Sue").ToAffrows();
			//Assert.Equal(1, affrows);
		}
		[Fact]
		public void UpdateModelSinglePk()
		{
			var info = Context.Select<UniPkTestModel>().Take(1).FirstOrDefault();
			var affrows = Context.Update(info).Set(a => a.Name, "Sue").ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UpdateModelCompositePk()
		{
			var info = Context.Select<UniCompositePkModel>().Take(1).FirstOrDefault();
			var affrows = Context.Update(info).Set(a => a.Name, "Sue").ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UpdateModels()
		{
			var infos = Context.Select<UniPkTestModel>().Take(2).ToList();
			var affrows = Context.UpdateRange(infos).Set(a => a.Name, "Sue").ToAffrows();
			Assert.Equal(2, affrows);
		}

		[Fact(Skip = "Access数据库暂不支持RETURNING语法")]
		public void UpdateReturning()
		{
			var info = Context.Select<UniCompositePkModel>().Take(1).FirstOrDefault();
			var result = Context.Update(info).Set(a => a.Name, "Sue").ToAffrowsResult();
			Assert.Equal(1, result.AffectedRows);
			Assert.Equal("Sue", result.Value.Name);
		}

		[Fact]
		public void SetEnumToInt()
		{
			var info = Context.Select<TypeTestModel>().Take(1).FirstOrDefault();
			var affrows = Context.Update(info).Set(a => a.LongType, TestEnum.正常).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void Inc()
		{
			var info = Context.Select<TypeTestModel>().Take(1).FirstOrDefault();
			var affrows = Context.Update(info).Inc(a => a.LongType, 200).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void UpdateSave()
		{
			var info = Context.Select<ProductModel>().FirstOrDefault();
			info.Stock += 20;
			var affrows = Context.UpdateSave(info);
			Assert.Equal(1, affrows);
		}
	}
}
