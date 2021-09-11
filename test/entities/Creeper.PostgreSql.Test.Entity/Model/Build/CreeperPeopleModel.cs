using Newtonsoft.Json.Linq;
using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.PostgreSql.Test.Entity.Model
{
	[CreeperTable("\"creeper\".\"people\"")]
	public partial class CreeperPeopleModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public Guid Id { get; set; }

		/// <summary>
		/// 年龄
		/// </summary>
		public int Age { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 性别
		/// </summary>
		public bool? Sex { get; set; }

		public DateTime Create_time { get; set; }

		/// <summary>
		/// 家庭住址
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// 详细住址
		/// </summary>
		public JToken Address_detail { get; set; }

		public CreeperDataState? State { get; set; }
	}
}
