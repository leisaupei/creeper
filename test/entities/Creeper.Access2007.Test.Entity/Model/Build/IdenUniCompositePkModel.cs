using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Access2007.Test.Entity.Model
{
	[CreeperTable("[IdenUniCompositePk]")]
	public partial class IdenUniCompositePkModel : ICreeperModel
	{
		/// <summary>
		/// 自增主键
		/// </summary>
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 唯一编号主键
		/// </summary>
		[CreeperColumn(IsPrimary = true)]
		public string NextId { get; set; }

		public string Name { get; set; }

		public short? Age { get; set; }
	}
}
