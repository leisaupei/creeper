using Creeper.Driver;
using System.Collections.Generic;

namespace Creeper.Generator.Common.Options
{
	/// <summary>
	/// 传入生成配置
	/// </summary>
	public class CreeperGenerateOption
	{
		/// <summary>
		/// 项目名称
		/// </summary>
		public string ProjectName { get; set; }
		/// <summary>
		/// 输出路径
		/// </summary>
		public string OutputPath { get; set; }
		/// <summary>
		/// 是否生成.sln文件
		/// </summary>
		public bool Sln { get; set; } = true;
		/// <summary>
		/// 字符串连接
		/// </summary>
		public List<CreeperGenerateConnection> Builders { get; set; } = new List<CreeperGenerateConnection>();
	}
	public class CreeperGenerateConnection
	{
		public CreeperGenerateConnection(string name, ICreeperConnection connection)
		{
			Name = name;
			Connection = connection;
		}

		public string Name { get; }
		public ICreeperConnection Connection { get; }
		public ICreeperExecute DbExecute => new CreeperExecute(Connection);
	}
}
