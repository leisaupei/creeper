using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.SqlServer.Test.Entity.Model
{
	[CreeperTable("[dbo].[IdenPkUniCol]")]
	public partial class IdenPkUniColModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		public string UniqueColumn { get; set; }

		public string Name { get; set; }
	}
}
