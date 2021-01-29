using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.SqlBuilder;

namespace Creeper.SqlExpression.XUnitTest
{
	/// <summary>
	/// 测试的实体类, 用户
	/// </summary>
	public class User
	{

		public int Id { set; get; }
		public string Name { set; get; }
		public int? Age { get; set; }

	}

	/// <summary>
	/// 测试的实体类, 分组
	/// </summary>
	public class Group : ICreeperDbModel
	{

		public int Id { set; get; }
		public string Name { get; set; }
		public static SelectBuilder<Group> Select => new CreeperDbExecute(CreeperDbContext.GetExecuteOption("dbmain")).Select<Group>();


	}
	/// <summary>
	/// 测试的实体类, 分组的成员
	/// </summary>
	public class GroupUsers
	{

		public int GroupId { set; get; }
		public string UserId { get; set; }

	}
}
