using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.SqlServer.Test.Entity.Model
{
	[CreeperTable("[dbo].[IdenPk]")]
	public partial class IdenPkModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
