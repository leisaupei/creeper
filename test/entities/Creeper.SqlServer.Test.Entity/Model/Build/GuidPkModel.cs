using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.SqlServer.Test.Entity.Model
{
	[CreeperTable("[dbo].[GuidPk]")]
	public partial class GuidPkModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public Guid Id { get; set; }

		public string Name { get; set; }
	}
}
