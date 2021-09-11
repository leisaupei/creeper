﻿using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	[CreeperTable("\"IdenPkTest\"")]
	public partial class IdenPkTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public long Id { get; set; }

		public string Name { get; set; }
	}
}
