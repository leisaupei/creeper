using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.SqlServer.Test.Entity.Model
{
	/// <summary>
	/// 测试唯一键
	/// </summary>
	[CreeperTable("[dbo].[UniTest]")]
	public partial class UniTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		public string UniqueColumn { get; set; }

		public string IdxUniqueColumn { get; set; }
	}
}
