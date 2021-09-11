namespace Creeper.xUnitTest.Contracts
{
	public interface IUpsertTest
	{
		void UidPk();
		void IdentityPk();

		/// <summary>
		/// 单主键包含一个唯一键, 此种情况mysql会有问题, 唯一键会成为upsert的条件之一, 其余数据库会抛出唯一键重复异常
		/// </summary>
		void SinglePkAndUniqueField();
		void UniqueAndIdentityCompositePk();
		void DoubleUniqueCompositePk();
	}
}