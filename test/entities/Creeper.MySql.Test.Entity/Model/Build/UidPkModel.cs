using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	[CreeperTable("`uid_pk`")]
	public partial class UidPkModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public string Id { get; set; }

		public string Name { get; set; }
	}
}
