using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	/// <summary>
	/// 测试用表
	/// </summary>
	[CreeperTable("`people`")]
	public partial class PeopleModel : ICreeperModel
	{
		/// <summary>
		/// 主键id
		/// 唯一键
		/// </summary>
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 年龄
		/// </summary>
		public int? Age { get; set; }

		public int? Stu_no { get; set; }
	}
}
