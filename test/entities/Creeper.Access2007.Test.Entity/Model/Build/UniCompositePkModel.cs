﻿using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Access2007.Test.Entity.Model
{
	[CreeperTable("[UniCompositePk]")]
	public partial class UniCompositePkModel : ICreeperModel
	{
		/// <summary>
		/// Guid主键
		/// </summary>
		[CreeperColumn(IsPrimary = true)]
		public Guid Id { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public string NextId { get; set; }

		public string Name { get; set; }

		public short? Age { get; set; }
	}
}
