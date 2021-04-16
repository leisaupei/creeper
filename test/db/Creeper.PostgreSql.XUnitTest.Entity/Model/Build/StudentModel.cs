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

	[CreeperDbTable(@"""public"".""student""", typeof(DbMain))]
	public partial class StudentModel : ICreeperDbModel
	{
		#region Properties
		/// <summary>
		/// 学号
		/// </summary>
		public string Stu_no { get; set; }

		public Guid Grade_id { get; set; }

		public Guid People_id { get; set; }

		public DateTime Create_time { get; set; }

		[CreeperDbColumn(Primary = true)]
		public Guid Id { get; set; }
		#endregion
	}
}
