using Creeper.Generic;

namespace Creeper.Generator.Common.Contracts
{
	public interface ICreeperGeneratorProviderFactory
	{
		/// <summary>
		/// 按照数据库种类获取构造器
		/// </summary>
		/// <value></value>
		ICreeperGeneratorProvider this[DataBaseKind kind] { get; }
	}
}