using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// 查询语句
	/// </summary>
	public interface ISqlBuilder
	{
		/// <summary>
		/// sql参数列表
		/// </summary>
		List<DbParameter> Params { get; }

		/// <summary>
		/// 参数化sql语句
		/// </summary>
		string CommandText { get; }

		/// <summary>
		/// 返回实例类型
		/// </summary>
		Type Type { get; }

		/// <summary>
		/// 是否列表
		/// </summary>
		PipeReturnType ReturnType { get; }

		/// <summary>
		/// 是否直接返回默认值
		/// </summary>
		bool IsReturnDefault { get; }

		/// <summary>
		/// 查询字段
		/// </summary>
		string Fields { get; set; }
	}
}
