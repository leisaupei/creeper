using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	[CreeperTable("`test_view`")]
	public partial class TestViewViewModel : ICreeperModel
	{
		/// <summary>
		/// 主键id
		/// 唯一键
		/// </summary>
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
