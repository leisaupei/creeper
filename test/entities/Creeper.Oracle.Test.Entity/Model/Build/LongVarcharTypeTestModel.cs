using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	[CreeperTable("\"LongVarcharTypeTest\"")]
	public partial class LongVarcharTypeTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public int Id { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Returning)]
		public string LongVarcharType { get; set; }
	}
}
