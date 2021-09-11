using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Access2007.Test.Entity.Model
{
	[CreeperTable("[Product]")]
	public partial class ProductModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 价格
		/// </summary>
		public float? Price { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime? CreateTime { get; set; }

		/// <summary>
		/// 分类id
		/// </summary>
		public int? CategoryId { get; set; }

		/// <summary>
		/// 商品名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 库存
		/// </summary>
		public int? Stock { get; set; }
	}
}
