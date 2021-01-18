using System;
using Candy.Driver.Common;
using Candy.Driver.Model;

namespace Candy.XUnitTest.Model
{
    [DbTable("student", typeof(Options.DbMain))]
	public partial class StudentModel : ICandyDbModel
	{
		#region Properties
		/// <summary>
		/// 学号
		/// </summary>
		public string Stu_no { get; set; }
		public Guid Grade_id { get; set; }
		public Guid People_id { get; set; }
		public DateTime Create_time { get; set; }
		[PrimaryKey] public Guid Id { get; set; }
		#endregion

	}
}
