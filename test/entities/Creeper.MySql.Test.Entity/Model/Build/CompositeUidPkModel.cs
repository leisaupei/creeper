using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	[CreeperTable("`composite_uid_pk`")]
	public partial class CompositeUidPkModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public string Id { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public string U_id { get; set; }

		public string Name { get; set; }
	}
}
