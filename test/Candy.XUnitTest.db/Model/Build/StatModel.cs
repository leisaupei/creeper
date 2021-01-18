using Candy.Common;
using Candy.Model;

namespace Candy.XUnitTest.Model
{
    [DbTable("stat", typeof(Options.DbMain))]
	public partial class StatModel : ICandyDbModel
	{
		#region Properties
		public int Times { get; set; }
		/// <summary>
		/// 单位毫秒
		/// </summary>
		public decimal Haoshi { get; set; }
		/// <summary>
		/// 自增id
		/// </summary>
		[PrimaryKey] public int Id { get; set; }
		#endregion

	}
}
