/* ##########################################################
 * #   此文件由 https://github.com/leisaupei/creeper 生成    #
 * ##########################################################
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
