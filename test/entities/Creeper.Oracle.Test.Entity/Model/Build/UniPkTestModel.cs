using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	[CreeperTable("\"UniPkTest\"")]
	public partial class UniPkTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public string Id { get; set; }

		public string Name { get; set; }
	}
}
