using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Access2007.Test.Entity.Model
{
	/// <summary>
	/// 视图测试 视图仅支持Select操作
	/// </summary>
	[CreeperTable("[VTest]")]
	public partial class VTestViewModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		public DateTime? DateTimeType { get; set; }

		public decimal? CurrencyType { get; set; }
	}
}
