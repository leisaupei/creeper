using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.SqlServer.Test.Entity.Model
{
	[CreeperTable("[dbo].[IdenGuidComposite]")]
	public partial class IdenGuidCompositeModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Iid { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public Guid Uid { get; set; }

		public string Name { get; set; }
	}
}
