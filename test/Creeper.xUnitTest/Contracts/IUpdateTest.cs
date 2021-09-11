using System.ComponentModel;
using Xunit;

namespace Creeper.xUnitTest.Contracts
{
	public interface IUpdateTest
	{
		void ReturnAffrows();
		void UpdateReturning();
		void UpdateModelSinglePk();
		void UpdateModels();
		void UpdateModelCompositePk();

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