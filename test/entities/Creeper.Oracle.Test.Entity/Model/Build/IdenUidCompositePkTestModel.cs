using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	[CreeperTable("\"IdenUidCompositePkTest\"")]
	public partial class IdenUidCompositePkTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public long Id { get; set; }

		public string Name { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public string Uid { get; set; }
	}
}
