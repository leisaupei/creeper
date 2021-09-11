using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.PostgreSql.Test.Entity.Model
{
	[CreeperTable("\"creeper\".\"uid_composite_pk\"")]
	public partial class CreeperUidCompositePkModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public Guid Uid1 { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public Guid Uid2 { get; set; }

		public string Name { get; set; }
	}
}
