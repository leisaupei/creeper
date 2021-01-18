using System;
using Candy.Driver.Common;
using Candy.Driver.Model;

namespace Candy.XUnitTest.Model
{
    /// <summary>
    /// 班级视图
    /// </summary>
    [DbTable("class.grade_view", typeof(Options.DbMain))]
	public partial class ClassGradeViewViewModel : ICandyDbModel
	{
		#region Properties
		public Guid? Id { get; set; }
		public string Name { get; set; }
		public DateTime? Create_time { get; set; }
		#endregion

	}
}
