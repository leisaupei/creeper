/* ################################################################################
 * # 此文件由生成器创建或覆盖。see: https://github.com/leisaupei/creeper
 * ################################################################################
 */
using System;
using Newtonsoft.Json;

namespace Creeper.PostgreSql.XUnitTest.Entity.Model
{
	[JsonObject(MemberSerialization.OptIn)]
	public partial struct Info
	{
		[JsonProperty] public Guid? Id { get; set; }
		[JsonProperty] public string Name { get; set; }
	}
}
