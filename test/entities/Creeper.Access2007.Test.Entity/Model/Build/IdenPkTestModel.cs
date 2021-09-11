using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Access2007.Test.Entity.Model
{
	[CreeperTable("[IdenPkTest]")]
	public partial class IdenPkTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
