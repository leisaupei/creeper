using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.PostgreSql.Test.Entity.Model
{
	[CreeperTable("\"creeper\".\"classmate\"")]
	public partial class CreeperClassmateModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public Guid Teacher_id { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public Guid Student_id { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public Guid Grade_id { get; set; }

		public DateTime? Create_time { get; set; }
	}
}
