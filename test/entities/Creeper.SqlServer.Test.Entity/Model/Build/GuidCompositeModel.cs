using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.SqlServer.Test.Entity.Model
{
	[CreeperTable("[dbo].[GuidComposite]")]
	public partial class GuidCompositeModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public Guid Uid { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public Guid Gid { get; set; }

		public string Name { get; set; }
	}
}
