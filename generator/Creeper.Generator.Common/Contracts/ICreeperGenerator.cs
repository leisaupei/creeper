using Creeper.Generator.Common.Options;

namespace Creeper.Generator.Common.Contracts
{
	public interface ICreeperGenerator
	{
		/// <summary>
		/// 运行数据库生成逻辑
		/// </summary>
		/// <param name="option"></param>
		void Generate(CreeperGenerateOption option);
	}
}
