using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	[CreeperTable("\"Product\"")]
	public partial class ProductModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public string Id { get; set; }

		/// <summary>
		/// 价格
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime { get; set; }

		/// <summary>
		/// 类别
		/// </summary>
		public string CategoryId { get; set; }

		/// <summary>
		/// 商品名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 库存
		/// </summary>
		public int Stock { get; set; }
	}
}
