using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.PostgreSql.Test.Entity.Model
{
	[CreeperTable("\"creeper\".\"iden_pk\"")]
	public partial class CreeperIdenPkModel : ICreeperModel
	{
		/// <summary>
		/// 名字
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 年龄
		/// </summary>
		public int? Age { get; set; }

		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }
	}
}
