using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	[CreeperTable("`composite_uid_iid_pk`")]
	public partial class CompositeUidIidPkModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public string U_id { get; set; }

		public string Name { get; set; }
	}
}
