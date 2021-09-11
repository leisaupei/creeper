using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.SqlServer.Test.Entity.Model
{
	[CreeperTable("[dbo].[Student]")]
	public partial class StudentModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 创建时间戳
		/// </summary>
		public long CreateTime { get; set; }

		/// <summary>
		/// 学号
		/// </summary>
		public int StuNo { get; set; }

		public int? TeacherId { get; set; }
	}
}
