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

	[CreeperDbTable(@"""public"".""teacher""", typeof(DbMain))]
	public partial class TeacherModel : ICreeperDbModel
	{
		#region Properties
		/// <summary>
		/// 学号
		/// </summary>
		public string Teacher_no { get; set; }

		public Guid People_id { get; set; }

		public DateTime Create_time { get; set; }

		[CreeperDbColumn(Primary = true)]
		public Guid Id { get; set; }
		#endregion
	}
}
