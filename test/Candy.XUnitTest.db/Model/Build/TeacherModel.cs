using System;
using Candy.Driver.Common;
using Candy.Driver.Model;

namespace Candy.XUnitTest.Model
{
    [DbTable("teacher", typeof(Options.DbMain))]
	public partial class TeacherModel : ICandyDbModel
	{
		#region Properties
		/// <summary>
		/// 学号
		/// </summary>
		public string Teacher_no { get; set; }
		public Guid People_id { get; set; }
		public DateTime Create_time { get; set; }
		[PrimaryKey] public Guid Id { get; set; }
		#endregion

	}
}
