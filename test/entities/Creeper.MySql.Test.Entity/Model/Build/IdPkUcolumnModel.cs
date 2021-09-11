using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	[CreeperTable("`id_pk_ucolumn`")]
	public partial class IdPkUcolumnModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 唯一约束列
		/// </summary>
		public string U_column { get; set; }

		public string Name { get; set; }
	}
}
