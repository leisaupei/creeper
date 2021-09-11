using System;
using System.ComponentModel;

namespace Creeper.Generic
{
	/// <summary>
	/// 联表方式
	/// </summary>
	public enum JoinType
	{
		INNER_JOIN,
		/// <summary>
		/// SQLite不支持此种方式关联查询
		/// </summary>
		LEFT_JOIN,

		/// <summary>
		/// SQLite不支持此种方式关联查询
		/// </summary>
		RIGHT_JOIN,

		LEFT_OUTER_JOIN,

		/// <summary>
		/// SQLite不支持此种方式关联查询
		/// </summary>
		RIGHT_OUTER_JOIN,

		/// <summary>
		/// 仅SQLite支持此关联方式
		/// </summary>
		CROSS_JOIN,
	}

	/// <summary>
	/// 管道返回值类型
	/// </summary>
	public enum PipeReturnType
	{
		/// <summary>
		/// 返回第一项
		/// </summary>
		First = 1,
		/// <summary>
		/// 列表
		/// </summary>
		List,
		/// <summary>
		/// 受影响行数
		/// </summary>
		Affrows,
		/// <summary>
		/// 受影响行数与结果
		/// </summary>
		AffrowsResult,
	}

	/// <summary>
	/// 数据库类型
	/// </summary>
	public enum DataBaseType
	{
		/// <summary>
		/// 根据<see cref="DataBaseTypeStrategy"/>主从策略选择
		/// </summary>
		Default = 0,
		/// <summary>
		/// 主库
		/// </summary>
		Main = 1,
		/// <summary>
		/// 从库
		/// </summary>
		Secondary
	}

	/// <summary>
	/// 缓存策略, 仅支持FirstOrDefault与ToScalar方法
	/// </summary>
	internal enum DbCacheType
	{
		/// <summary>
		/// 不使用数据库缓存
		/// </summary>
		None = 0,

		/// <summary>
		/// 默认缓存策略
		/// </summary>
		Default = 1,

		/// <summary>
		/// 主键缓存策略, 暂时不支持
		/// </summary>
		PkCache = 2,
	}

	/// <summary>
	/// 数据库主从使用策略, 主要针对Select语句
	/// </summary>
	public enum DataBaseTypeStrategy
	{
		/// <summary>
		/// 只使用从库, 没有从库会报错
		/// </summary>
		OnlySecondary = 1,
		/// <summary>
		/// 从库优先, 如果从库是Empty自动使用主库
		/// </summary>
		MainIfSecondaryEmpty = 2,
		/// <summary>
		/// 只使用主库
		/// </summary>
		OnlyMain = 3,
	}

	/// <summary>
	/// 数据库种类
	/// </summary>
	public enum DataBaseKind
	{
		SqlServer = 1,
		Access = 2,
		MySql = 3,
		Oracle = 4,
		PostgreSql = 5,
		Sqlite = 6,
	}

	/// <summary>
	/// 数据库的字段名称规范，数据库字段中的'_'始终会输出到属性名称中
	/// </summary>
	public enum ColumnNameStyle
	{
		/// <summary>
		/// 默认是全部转小写, 忽略大小写时可选
		/// </summary>
		None = 0,

		/// <summary>
		/// 转为小写，数据库字段规范是以下情况时选此项，如: username, user_name
		/// </summary>
		ToLower = 1,

		/// <summary>
		/// 转为大写，数据库字段规范是以下情况时选此项，如: USERNAME, USER_NAME
		/// </summary>
		ToUpper = 2,

		/// <summary>
		/// 转为驼峰，数据库字段规范是以下情况时选此项，如: userName, user_Name
		/// </summary>
		Camel = 3,

		/// <summary>
		///	转为帕斯卡，数据库字段规范是以下情况时选此项，如: UserName, User_Name
		/// </summary>
		Pascal = 4,
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
