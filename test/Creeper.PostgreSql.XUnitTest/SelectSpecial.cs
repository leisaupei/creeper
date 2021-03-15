using Creeper.Extensions;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using Creeper.PostgreSql.XUnitTest.Entity.Options;
using Npgsql;
using System;
using System.Linq;
using Xunit;

namespace Creeper.PostgreSql.XUnitTest
{
	public class SelectSpecial : BaseTest
	{
		[Fact]
		public void ReturnJTokenInTuple()
		{
			//var info = _dbContext.Select<PeopleModel>() .Where(a => a.Id == StuPeopleId1).FirstOrDefault<(Guid id, string name, JToken address_detail, EDataState state)>("id,name,address_detail,state");
			_dbContext.GetExecute<DbMain>().ExecuteNonQuery("update class.grade set create_time = now() + @years", System.Data.CommandType.Text,
				new[] { new Npgsql.NpgsqlParameter<TimeSpan>("years", TimeSpan.FromDays(365)) });

		}
		[Fact]
		public void ReturnStringOrValueType()
		{
			var result = _dbContext.GetExecute<DbMain>().ExecuteScalar<Guid?>("select id from people where id <> @id", cmdParams: new[] { new NpgsqlParameter("id", null) });
			var result1 = _dbContext.Select<PeopleModel>().Where(a => new string[] { "xxx", null }.Contains(a.Name)).ToList();
		}
	}
}
