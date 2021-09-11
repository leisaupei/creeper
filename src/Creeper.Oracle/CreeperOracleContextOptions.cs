using Creeper.Driver;
using Creeper.Generic;

namespace Creeper.Driver
{
	public class CreeperOracleContextOptions : CreeperContextOptions
	{
		internal ColumnNameStyle ColumnNameStyle { get; set; } = ColumnNameStyle.None;
	}
}
