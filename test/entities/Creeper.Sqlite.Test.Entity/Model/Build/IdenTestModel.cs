using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Sqlite.Test.Entity.Model
{
	[CreeperTable("\"iden_test\"")]
	public partial class IdenTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public long Id { get; set; }

		public string Name { get; set; }
	}
}
