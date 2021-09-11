using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	[CreeperTable("\"v_test\"")]
	public partial class VTestModel : ICreeperModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Returning)]
		public string LongType { get; set; }
	}
}
