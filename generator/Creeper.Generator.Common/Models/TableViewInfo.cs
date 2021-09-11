namespace Creeper.Generator.Common.Models
{
	/// <summary>
	/// 
	/// </summary>
	public class TableViewInfo
	{
		/// <summary>
		/// 表名
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 是否视图
		/// </summary>
		public bool IsView { get; set; }
		/// <summary>
		/// 表格注释
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// 模式名称
		/// </summary>
		public string SchemaName { get; set; }
		public override string ToString()
		{
			return SchemaName + Name;
		}
	}

}
