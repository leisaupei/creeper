using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;

namespace Creeper.xUnitTest.Contracts
{
	public interface IOtherTest
	{
		/// <summary>
		/// 管道模式
		/// </summary>
		void ExecutePipe();

		/// <summary>
		/// 事务
		/// </summary>
		Task TransationAsync();
	}
}