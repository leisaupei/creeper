using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	[CreeperTable("`student`")]
	public partial class StudentModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		public int People_id { get; set; }

		public int? Stu_no { get; set; }

		public string Class_name { get; set; }

		public string Teacher_name { get; set; }
	}
}
