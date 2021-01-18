using Candy.Model;

namespace Candy.Common
{
	public interface ICandyDbOption
	{
		/// <summary>
		/// 主库
		/// </summary>
		public ICandyDbConnectionOptions Main { get; }

		/// <summary>
		/// 从库
		/// </summary>
		public ICandyDbConnectionOptions[] Secondary { get; }
	}
}
