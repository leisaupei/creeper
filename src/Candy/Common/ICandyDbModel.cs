using Newtonsoft.Json;

namespace Candy.Common
{
	/// <summary>
	/// 数据库表模型
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public interface ICandyDbModel
	{
	}
}
