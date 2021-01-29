using Xunit;
using System.Collections;
using System.Linq;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Newtonsoft.Json;
using Creeper.Generic;

namespace Creeper.SqlExpression.XUnitTest
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			var test = new TestModel() { Age = 1 };
			var name = "null";

			var t = JsonConvert.SerializeObject(test);
			string sql11 = Group.Select.Where(a => a.Id == 1).ToString();

			string sql10 = SqlGenerator.GetWhereByLambdaUnion<Group, GroupUsers>((a, b) => a.Id == b.GroupId && a.Id == 2, DataBaseKind.PostgreSql);
			string sql9 = SqlGenerator.GetWhereByLambdaSelectorWithParameters<User, int>(a => a.Age ?? 3, DataBaseKind.PostgreSql);
			string sql8 = SqlGenerator.GetWhereByLambdaSelectorWithParameters<User, int>(a => a.Age.Value, DataBaseKind.PostgreSql);
			string sql7 = SqlGenerator.GetWhereByLambda<User>(a => a.Age.Value != test.Age.Value && a.Id > 2, DataBaseKind.PostgreSql);
			string sql5 = SqlGenerator.GetWhereByLambda<User>(a => a.Name != null && a.Id > 2, DataBaseKind.PostgreSql);
			string sql6 = SqlGenerator.GetWhereByLambda<User>(a => a.Name != name && a.Id > 2, DataBaseKind.PostgreSql);

			string sql1 = SqlGenerator.GetWhereByLambda<User>(x => x.Name.StartsWith("test") && 2 > x.Id, DataBaseKind.PostgreSql);
			string sql2 = SqlGenerator.GetWhereByLambda<User>(x => x.Name.EndsWith("test") && (x.Id > 4 || x.Id == 3), DataBaseKind.PostgreSql);
			string sql3 = SqlGenerator.GetWhereByLambda<User>(x => x.Name.Contains("test") && (x.Id > 4 && x.Id <= 8), DataBaseKind.PostgreSql);
			string sql4 = SqlGenerator.GetWhereByLambda<User>(x => x.Name == "FengCode" && (x.Id >= 1), DataBaseKind.PostgreSql);
		}
		public class TestModel
		{
			public int? Age { get; set; }
			public static int GetSet => 1;
		}
	}
}
