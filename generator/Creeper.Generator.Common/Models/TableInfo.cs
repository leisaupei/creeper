using System.Collections.Generic;

namespace Creeper.Generator.Common.Models
{
	public class TableInfo
	{
		public TableViewInfo Table { get; set; }
		public List<ColumnInfo> Columns { get; } = new List<ColumnInfo>();
	}

}
