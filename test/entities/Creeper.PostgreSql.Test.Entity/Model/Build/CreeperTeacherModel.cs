using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.PostgreSql.Test.Entity.Model
{
	[CreeperTable("\"creeper\".\"teacher\"")]
	public partial class CreeperTeacherModel : ICreeperModel
	{
		/// <summary>
		/// 学号
		/// </summary>
		public string Teacher_no { get; set; }

		public Guid People_id { get; set; }

		public DateTime Create_time { get; set; }

		[CreeperColumn(IsPrimary = true)]
		public Guid Id { get; set; }
	}
}
