using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// INSERT语句实例。需要遵循自增整型主键必须大于0的规范
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public interface IInsertBuilder<TModel> : IWhereBuilder<IInsertBuilder<TModel>, TModel>, IGetAffrows, IGetAffrowsResult<TModel> where TModel : class, ICreeperModel, new()

	{
		/// <summary>
		/// 是否有更新赋值
		/// </summary>
		bool HasSet { get; }

		/// <summary>
		/// 设置一个结果 调用Field方法定义
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		IInsertBuilder<TModel> Set<TTarget>(Expression<Func<TModel, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// 根据实体类插入
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		IInsertBuilder<TModel> Set(TModel model);

		/// <summary>
		/// 设置语句 可空重载
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IInsertBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey?>> selector, TKey? value) where TKey : struct;

		/// <summary>
		/// 设置语句 不可空参数
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IInsertBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey>> selector, TKey value);
	}
}