using Creeper.Driver;
using Creeper.Sqlite.Test.Entity.Model;
using Creeper.xUnitTest.Contracts;
using Creeper.xUnitTest.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Creeper.xUnitTest.Sqlite
{
	public class UpdateTest : BaseTest, IUpdateTest
	{
		private static long Id => Context.Select<IdenTestModel>().Max(a => a.Id);
		[Fact]
		[Description("更新返回受影响行数")]
		public void ReturnAffrows()
		{
			var affrows = Context.Update<IdenTestModel>().Where(a => a.Id == Id).Set(a => a.Name, "测试").ToAffrows();
			affrows = Context.Update<IdenTestModel>(a => a.Id == Id).Set(a => a.Name, "测试").ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		[Description("更新条件传入实体类, 则使用主键作为条件")]
		public void UpdateModelSinglePk()
		{
			var info = Context.Select<IdenTestModel>(a => a.Id == Id).FirstOrDefault();
			if (info != null)
			{
				var affrows = Context.Update(info).Set(a => a.Name, "测试").ToAffrows();
				Assert.Equal(1, affrows);
			}
		}

		[Fact]
		[Description("更新条件传入实体类, 则使用主键作为条件, 测试复合主键逻辑")]
		public void UpdateModelCompositePk()
		{
			var info = Context.Select<UniCompositeModel>().FirstOrDefault();
			if (info != null)
			{
				var affrows = Context.Update(info).Set(a => a.Name, "测试").ToAffrows();
				Assert.Equal(1, affrows);
			}
		}

		[Fact]
		public void UpdateModels()
		{
			var infos = Context.Select<IdenTestModel>().OrderByDescending(a => a.Id).Take(2).ToList();

			var affrows = Context.UpdateRange(infos).Set(a => a.Name, "测试").ToAffrows();
			Assert.Equal(2, affrows);
		}

		[Fact]
		[Description("SQLite暂不支持更新返回行")]
		public void UpdateReturning()
		{
			//var affrows = 0;
			//long id = 0;
			//string name = null;
			//using (var conn = new SQLiteConnection(MainConnectionString))
			//{
			//	conn.Open();
			//	var cmd = conn.CreateCommand();
			//	cmd.CommandText = @"UPDATE ""test"" AS a SET ""name"" = 'Sam' WHERE (a.""id"" = 1) RETURNING id,name";
			//	var reader = cmd.ExecuteReader();
			//	while (reader.Read())
			//	{
			//		id = Convert.ToInt64(reader["id"]);
			//		name = reader["name"].ToString();
			//	}
			//	affrows = reader.RecordsAffected;
			//}


			//var resul1 = Context.ExecuteReaderFirstAffrows<IdenTestModel>(@"
			//INSERT INTO iden_test(id,name) VALUES (NULL, 'johnny') returning ""id"",""name"";
			//");
			//var resul11 = Context.ExecuteReaderFirstAffrows<IdenTestModel>(@"
			//	UPDATE ""iden_test"" AS a SET ""name"" = 'Sammy' WHERE (a.""id"" =10000)  RETURNING ""id"",""name""
			//");
			////			var result = Context.Update<IdenTestModel>(a => a.Id == 1000).Set(a => a.Name, "小明").ToAffrowsResult();
			//var info = Context.Select<IdenTestModel>(a => a.Id == Id).FirstOrDefault();
			//if (info != null)
			//{
			//var result = Context.Update<IdenTestModel>(a => a.Id == Id).Set(a => a.Name, "小明").ToAffrowsResult();
			//Assert.True(result.AffectedRows >= 0);
			//Assert.Equal("小明", result.Value.Name);
			//}

			Assert.Throws<CreeperNotSupportedException>(() =>
			{
				var result2 = Context.Update<IdenTestModel>(a => a.Id == Id).Set(a => a.Name, "小明").ToAffrowsResult();
				Assert.True(result2.AffectedRows >= 0);
				Assert.Equal("小明", result2.Value.Name);
			});
		}

		[Fact]
		public void SetEnumToInt()
		{
			var info = Context.Select<ProductModel>().OrderBy(a => a.Stock).FirstOrDefault();
			var affrows = Context.Update(info).Set(a => a.Stock, TestEnum.正常).ToAffrows();
			Assert.Equal(1, affrows);
		}

		[Fact]
		public void Inc()
		{
			var info = Context.Select<ProductModel>().OrderBy(a => a.Stock).FirstOrDefault();
			var affrows = Context.Update(info).Inc(a => a.Stock, 20, 0).ToAffrows();
			Assert.Equal(1, affrows);
		}
	}
}
