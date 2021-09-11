using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	[CreeperTable("`uuid_pk`")]
	public partial class UuidPkModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public Guid Uid { get; set; }

		public string Name { get; set; }
	}
}
