using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	[CreeperTable("`test_ext`")]
	public partial class TestExtModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public int Id { get; set; }

		public string Bio { get; set; }
	}
}
