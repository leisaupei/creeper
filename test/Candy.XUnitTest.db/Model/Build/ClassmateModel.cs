using System;
using Candy.Common;
using Candy.Model;

namespace Candy.XUnitTest.Model
{
    [DbTable("classmate", typeof(Options.DbMain))]
	public partial class ClassmateModel : ICandyDbModel
	{
		#region Properties
		[PrimaryKey] public Guid Teacher_id { get; set; }
		[PrimaryKey] public Guid Student_id { get; set; }
		[PrimaryKey] public Guid Grade_id { get; set; }
		public DateTime? Create_time { get; set; }
		#endregion

	}
}
