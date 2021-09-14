using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Sqlite.Test.Entity.Model
{
	[CreeperTable("\"type_test\"")]
	public partial class TypeTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public long Id { get; set; }

		public int? Age { get; set; }

		public bool? Bool_type { get; set; }

		public DateTime? Date_time { get; set; }

		public float? Float_type { get; set; }

		public decimal? Numeric_type { get; set; }

		public TimeSpan? Time_type { get; set; }

		public decimal? Decimal_type { get; set; }

		public Guid? Guid_type { get; set; }
	}
}
