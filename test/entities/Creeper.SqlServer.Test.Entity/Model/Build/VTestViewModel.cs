using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.SqlServer.Test.Entity.Model
{
	/// <summary>
	/// 视图仅支持Select操作
	/// </summary>
	[CreeperTable("[dbo].[VTest]")]
	public partial class VTestViewModel : ICreeperModel
	{
		[CreeperColumn(IsIdentity = true)]
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
