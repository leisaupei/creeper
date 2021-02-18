/* ################################################################################
 * # 此文件由生成器创建或覆盖。see: https://github.com/leisaupei/creeper
 * ################################################################################
 */
using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.NetworkInformation;
using NpgsqlTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Creeper.Attributes;
using Creeper.PostgreSql.XUnitTest.Entity.Options;

namespace Creeper.PostgreSql.XUnitTest.Entity.Model
{

	[DbTable(@"""public"".""classmate""", typeof(DbMain))]
	public partial class ClassmateModel : ICreeperDbModel
	{
		#region Properties
		[PrimaryKey] public Guid Teacher_id { get; set; }
		[PrimaryKey] public Guid Student_id { get; set; }
		[PrimaryKey] public Guid Grade_id { get; set; }
		public DateTime? Create_time { get; set; }
		#endregion

	}
}
