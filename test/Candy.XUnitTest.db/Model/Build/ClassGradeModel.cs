using System;
using Candy.Driver.Model;
using Candy.Driver.Common;

namespace Candy.XUnitTest.Model
{
    /// <summary>
    /// 班的年纪
    /// 比如是 一年级,二年级
    /// </summary>
    [DbTable("class.grade", typeof(Options.DbMain))]
	public partial class ClassGradeModel : ICandyDbModel
	{
		#region Properties
		[PrimaryKey] public Guid Id { get; set; }
		/// <summary>
		/// 班级名称
		/// </summary>
		public string Name { get; set; }
		public DateTime Create_time { get; set; }
		#endregion

	}
}
