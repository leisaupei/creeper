using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	[CreeperTable("\"LongRawTypeTest\"")]
	public partial class LongRawTypeTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public int Id { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Returning)]
		public byte[] LongRawType { get; set; }
	}
}
