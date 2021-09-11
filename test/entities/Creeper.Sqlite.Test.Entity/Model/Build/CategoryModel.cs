using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Sqlite.Test.Entity.Model
{
	[CreeperTable("\"category\"")]
	public partial class CategoryModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public long Id { get; set; }

		public string Name { get; set; }
	}
}
