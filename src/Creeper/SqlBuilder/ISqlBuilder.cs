using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// 
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

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TBuilder"></typeparam>
	public interface ISqlBuilder<TBuilder> : ISqlBuilder
		where TBuilder : class, ISqlBuilder
	{
		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		TBuilder AddParameter(DbParameter p);

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		TBuilder AddParameter(string parameterName, object value);

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="ps"></param>
		/// <returns></returns>
		TBuilder AddParameters(IEnumerable<DbParameter> ps);

		/// <summary>
		/// 选择主/从库, Default预设策略
		/// </summary>
		/// <returns></returns>
		TBuilder By(DataBaseType dataBaseType);

		/// <summary>
		/// 使用数据库缓存, 仅支持FirstOrDefault/ToScalar方法, 注意: 此处条件必须是重写了ToString或者其他基础类型,
		/// </summary>
		/// <returns></returns>
		TBuilder ByCache(TimeSpan? expireTime);

		/// <summary>
		/// 使用数据库缓存, 仅支持FirstOrDefault/ToScalar方法, 注意: 此处条件必须是重写了ToString或者其他基础类型,
		/// </summary>
		/// <returns></returns>
		TBuilder ByCache(int? expireSeconds = null);

	}
}
