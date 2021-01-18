using Newtonsoft.Json;

namespace Candy.Driver.Common
{
	/// <summary>
	/// 数据库表模型
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public interface ICandyDbModel
	{
	}
}
