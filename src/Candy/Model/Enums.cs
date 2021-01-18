using System;
using System.ComponentModel;

namespace Candy.Model
{
	public enum UnionEnum
	{
		INNER_JOIN = 1, LEFT_JOIN, RIGHT_JOIN, LEFT_OUTER_JOIN, RIGHT_OUTER_JOIN
	}
	public enum PipeReturnType
	{
		One = 1, List, Rows
	}
	internal enum ExpressionExcutionType
	{
		None = 0, Single, Union, Condition, SingleForNoAlias
	}

	public enum DatabaseType
	{
		Postgres = 1,
		[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Future")]
		Mysql = 2,
	}
	/// <summary>
	/// 参照的分割字段类型
	/// </summary>
	public enum SplitType
	{
		/// <summary>
		/// 时间类型 从1970-1-1 开始按照N年分割
		/// </summary>
		DateTimeEveryYears = 2,
		/// <summary>
		/// 时间类型 从1970-1-1 开始按照N个月分割
		/// </summary>
		DateTimeEveryMonths = 3,
		/// <summary>
		/// 时间类型 从1970-1-1 开始按照N月分割
		/// </summary>
		DateTimeEveryDays = 4,
		/// <summary>
		/// int类型 每个int一个表
		/// </summary>
		IntEveryValue = 11,
		/// <summary>
		/// int类型 多个int类型一个表
		/// </summary>
		IntEveryValues = 12,
		/// <summary>
		/// 枚举 每个枚举类型一个表
		/// </summary>
		EnumEveryValue = 21,
		/// <summary>
		/// 枚举 多个枚举类型一个表
		/// </summary>
		EnumEveryValues = 22,
		/// <summary>
		/// Guid 按照首字母分表
		/// </summary>
		UuidEveryFirstLetter = 31
	}

}
