using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	[CreeperTable("\"UidCompositePkTest\"")]
	public partial class UidCompositePkTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public string Id { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public string Id2 { get; set; }

		public string Name { get; set; }
	}
}
