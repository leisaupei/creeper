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

	[CreeperDbTable(@"""test"".""uuid_iden_pk""", typeof(DbMain))]
	public partial class TestUuidIdenPkModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public Guid Id { get; set; }

		/// <summary>
		/// 名字
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 年龄
		/// </summary>
		public int? Age { get; set; }

		[CreeperDbColumn(Primary = true, Identity = true)]
		public int Id_sec { get; set; }
		#endregion
	}
}
