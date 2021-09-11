using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Sqlite.Test.Entity.Model
{
	[CreeperTable("\"test\"")]
	public partial class TestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public long Id { get; set; }

		public int? Age { get; set; }
	}
}
