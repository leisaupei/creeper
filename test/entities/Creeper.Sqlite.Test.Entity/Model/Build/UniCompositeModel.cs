using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Sqlite.Test.Entity.Model
{
	[CreeperTable("\"uni_composite\"")]
	public partial class UniCompositeModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public long Uid1 { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public long Uid2 { get; set; }

		public string Name { get; set; }
	}
}
