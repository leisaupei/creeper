using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Access2007.Test.Entity.Model
{
	[CreeperTable("[Category]")]
	public partial class CategoryModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 分类名称
		/// </summary>
		public string Name { get; set; }
	}
}
