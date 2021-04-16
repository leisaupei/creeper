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

	[CreeperDbTable(@"""test"".""iden_nopk""", typeof(DbMain))]
	public partial class TestIdenNopkModel : ICreeperDbModel
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

		[CreeperDbColumn(Identity = true)]
		public int Name_no { get; set; }
		#endregion
	}
}
