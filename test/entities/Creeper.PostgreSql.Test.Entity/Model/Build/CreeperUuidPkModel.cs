using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.PostgreSql.Test.Entity.Model
{
	[CreeperTable("\"creeper\".\"uuid_pk\"")]
	public partial class CreeperUuidPkModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public Guid Id { get; set; }

		/// <summary>
		/// 名字
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 年龄
		/// </summary>
		public int? Age { get; set; }
	}
}
