using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Sqlite.Test.Entity.Model
{
	[CreeperTable("\"product\"")]
	public partial class ProductModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public long Id { get; set; }

		public string Name { get; set; }

		public double? Price { get; set; }

		public byte[] Img { get; set; }

		public int? Stock { get; set; }

		public int? Category_id { get; set; }

		public long? Stat { get; set; }
	}
}
