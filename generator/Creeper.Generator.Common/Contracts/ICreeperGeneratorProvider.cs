using Creeper.Driver;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generic;

namespace Creeper.Generator.Common.Contracts
{
	public interface ICreeperGeneratorProvider
	{
		/// <summary>
		/// 数据库种类
		/// </summary>
		/// <value></value>
		DataBaseKind DataBaseKind { get; }

		/// <summary>
		/// 数据库表实体类生成器
		/// </summary>
		/// <param name="options">生成配置</param>
		/// <param name="dbOption">数据库配置</param>
		/// <param name="folder">是否使用分类文件夹</param>
		void Generate();

		/// <summary>
		/// 通过命令行传入字符串获取数据库连接信息
		/// </summary>
		/// <param name="build"></param>
		/// <returns></returns>
		CreeperGenerateConnection GetDbConnectionOptionFromString(string build);

		/// <summary>
		/// 创建派生实例
		/// </summary>
		/// <param name="generateRules"></param>
		/// <param name="options"></param>
		/// <param name="connection"></param>
		/// <returns></returns>
		ICreeperGeneratorProvider CreateInstance(GenerateRules generateRules, CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection);
	}
}
