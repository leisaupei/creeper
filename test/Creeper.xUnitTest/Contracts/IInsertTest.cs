using System.ComponentModel;
using Xunit;

namespace Creeper.xUnitTest.Contracts
{
	public interface IInsertTest
	{
		/// <summary>
		/// 随机唯一键(测试普通逻辑)
		/// </summary>
		void UidPk();

		/// <summary>
		/// 自增主键(测试自增逻辑)
		/// </summary>
		void IdentityPk();

		/// <summary>
		/// 批量插入, 使用多条语句
		/// </summary>
		void InsertRangeMultiple();

		/// <summary>
		/// 批量插入, 使用单条语句
		/// </summary>
		void InsertRangeSingle();

		/// <summary>
		/// 按条件插入(使用where语句)
		/// </summary>
		void InsertWithWhere();

		/// <summary>
		/// 插入并返回受影响行
		/// </summary>
		void InsertReturning();

		/// <summary>
		/// 唯一复合主键
		/// </summary>
		void DoubleUniqueCompositePk();

		/// <summary>
		/// 自增+唯一复合主键
		/// </summary>
		void UniqueAndIdentityCompositePk();
	}
}