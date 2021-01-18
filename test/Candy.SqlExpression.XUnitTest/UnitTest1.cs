using Xunit;
using System.Collections;
using System.Linq;

namespace Candy.SqlExpression.XUnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var test = new TestModel() { Age = 1 };
            var name = "null";
            string sql7 = SqlSugor.GetWhereByLambda<Users>(a => a.Age.Value != test.Age.Value && a.Id > 2, DataBaseType.PostgreSql);
            string sql5 = SqlSugor.GetWhereByLambda<Users>(a => a.Name != null && a.Id > 2, DataBaseType.PostgreSql);
            string sql6 = SqlSugor.GetWhereByLambda<Users>(a => a.Name != name && a.Id > 2, DataBaseType.PostgreSql);

            string sql1 = SqlSugor.GetWhereByLambda<Users>(x => x.Name.StartsWith("test") && 2 > x.Id, DataBaseType.PostgreSql);
            string sql2 = SqlSugor.GetWhereByLambda<Users>(x => x.Name.EndsWith("test") && (x.Id > 4 || x.Id == 3), DataBaseType.PostgreSql);
            string sql3 = SqlSugor.GetWhereByLambda<Users>(x => x.Name.Contains("test") && (x.Id > 4 && x.Id <= 8), DataBaseType.PostgreSql);
            string sql4 = SqlSugor.GetWhereByLambda<Users>(x => x.Name == "FengCode" && (x.Id >= 1), DataBaseType.PostgreSql);
        }
        public class TestModel
        {
            public int? Age { get; set; }
        }
    }
}
