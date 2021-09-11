using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	[CreeperTable("\"UniKeyTest\"")]
	public partial class UniKeyTestModel : ICreeperModel
	{
		/// <summary>
		/// Guid
		/// </summary>
		[CreeperColumn(IsPrimary = true)]
		public Guid Id { get; set; }

		/// <summary>
		/// 索引唯一键
		/// </summary>
		public string IdxUniqueKeyTest { get; set; }

		/// <summary>
		/// 普通唯一键
		/// </summary>
		public string UniqueKeyTest { get; set; }
	}
}
