using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.PostgreSql.Test.Entity.Model
{
	/// <summary>
	/// 班级
	/// </summary>
	[CreeperTable("\"creeper\".\"grade\"")]
	public partial class CreeperGradeModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public Guid Id { get; set; }

		/// <summary>
		/// 班级名称
		/// </summary>
		public string Name { get; set; }

		public DateTime Create_time { get; set; }
	}
}
