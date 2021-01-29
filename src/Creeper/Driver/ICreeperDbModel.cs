using Newtonsoft.Json;

namespace Creeper.Driver
{
	/// <summary>
	/// 数据库表模型
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public interface ICreeperDbModel
	{
	}
}
