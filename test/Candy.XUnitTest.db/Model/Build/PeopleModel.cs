using System;
using Candy.Common;
using Candy.Model;
using Newtonsoft.Json.Linq;

namespace Candy.XUnitTest.Model
{
    [DbTable("people", typeof(Options.DbMain))]
	public partial class PeopleModel : ICandyDbModel
	{
		#region Properties
		[PrimaryKey] public Guid Id { get; set; }
		/// <summary>
		/// 年龄
		/// </summary>
		public int Age { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 性别
		/// </summary>
		public bool? Sex { get; set; }
		public DateTime Create_time { get; set; }
		/// <summary>
		/// 家庭住址
		/// </summary>
		public string Address { get; set; }
		/// <summary>
		/// 详细住址
		/// </summary>
		public JToken Address_detail { get; set; }
		public EDataState State { get; set; }
		#endregion

	}
}
