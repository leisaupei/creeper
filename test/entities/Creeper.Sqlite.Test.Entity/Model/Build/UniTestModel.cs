using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Sqlite.Test.Entity.Model
{
	[CreeperTable("\"uni_test\"")]
	public partial class UniTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public long Id { get; set; }

		public string Name { get; set; }
	}
}
