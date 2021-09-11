using Xunit;
using Creeper.xUnitTest.Contracts;

namespace Creeper.xUnitTest.Access.v2003
{
	public class UpsertTest : BaseTest, IUpsertTest
	{
		[Fact(Skip = "Access不支持Upsert语法")]
		public void DoubleUniqueCompositePk()
		{
		}
		[Fact(Skip = "Access不支持Upsert语法")]
		public void UidPk()
		{
		}
		[Fact(Skip = "Access不支持Upsert语法")]
		public void IdentityPk()
		{
		}
		[Fact(Skip = "Access不支持Upsert语法")]
		public void SinglePkAndUniqueField()
		{
		}
		[Fact(Skip = "Access不支持Upsert语法")]
		public void UniqueAndIdentityCompositePk()
		{
		}
	}
}
