using System.ComponentModel;
using Xunit;

namespace Creeper.xUnitTest.Contracts
{
	public interface IUpdateTest
	{
		/// <summary>
		/// 更新返回受影响行数
		/// </summary>
		void ReturnAffrows();

		/// <summary>
		/// 更新并返回更新后的行
		/// </summary>
		void UpdateReturning();

		/// <summary>
		/// 对象主键作为条件更新, 单主键
		/// </summary>
		void UpdateModelSinglePk();

		/// <summary>
		/// 对象主键作为条件更新, 批量更新
		/// </summary>
		void UpdateModels();

		/// <summary>
		/// 对象主键作为条件更新, 复合主键
		/// </summary>
		void UpdateModelCompositePk();

		/// <summary>
		/// 覆盖数据库数据
		/// </summary>
		void UpdateSave();

		/// <summary>
		/// 设置一个枚举类型为整型
		/// </summary>
		void SetEnumToInt();

		/// <summary>
		/// 设置一个枚举类型为整型
		/// </summary>
		void Inc();
	}
}