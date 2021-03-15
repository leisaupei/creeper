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

	[CreeperDbTable(@"""public"".""people""", typeof(DbMain))]
	public partial class PeopleModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public Guid Id { get; set; }

		/// <summary>
		/// 年龄
		/// </summary>
		public int Age { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 性别
		/// </summary>
		public bool? Sex { get; set; }

		public DateTime Create_time { get; set; }

		/// <summary>
		/// 家庭住址
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// 详细住址
		/// </summary>
		public JToken Address_detail { get; set; }

		public EDataState State { get; set; }
		#endregion
	}
}
