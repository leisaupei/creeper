using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common.Models
{
	public class CompositeType
	{
		/// <summary>
		/// 模式名称
		/// </summary>
		public string SchemaName { get; set; }

		/// <summary>
		/// 数据库复合类型名称
		/// </summary>
		public string DbCompositeName { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string CsharpClassName { get; set; }
		/// <summary>
		/// 类型描述
		/// </summary>
		public string Description { get; set; }

		public List<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();
		public override string ToString()
		{
			return SchemaName + "." + DbCompositeName;
		}
	}
}
