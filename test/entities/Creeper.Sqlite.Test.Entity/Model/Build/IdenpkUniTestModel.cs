using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Sqlite.Test.Entity.Model
{
	[CreeperTable("\"idenpk_uni_test\"")]
	public partial class IdenpkUniTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public long Id { get; set; }

		public string Uni_column { get; set; }

		public string Idx_uni_column { get; set; }

		public string Name { get; set; }
	}
}
