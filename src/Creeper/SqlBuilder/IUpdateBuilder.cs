using Creeper.Driver;
using Creeper.Generic;
using Creeper.SqlBuilder.Impi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	public interface IUpdateBuilder<TModel> : IWhereBuilder<IUpdateBuilder<TModel>, TModel>, IGetAffrows, IGetAffrowsResult<TModel>, IGetAffrowsResults<TModel> where TModel : class, ICreeperModel, new()
	{
		/// <summary>
		/// 是否有更新赋值
		/// </summary>
		bool HasSet { get; }

		/// <summary>
		/// 数组连接一个数组
		/// </summary>
		/// <typeparam name="TKey">数组类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">数组</param>
		/// <returns></returns>
		IUpdateBuilder<TModel> Append<TKey>(Expression<Func<TModel, TKey[]>> selector, params TKey[] value) where TKey : struct;

		/// <summary>
		/// 自增, 可空类型留默认值
		/// </summary>
		/// <typeparam name="TKey">COALESCE默认值类型</typeparam>
		/// <typeparam name="TTarget">增加值的类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">增量</param>
		/// <param name="defaultValue">COALESCE默认值, 如果null, 则取default(TKey)</param>
		/// <exception cref="ArgumentNullException">增量为空</exception>
		/// <returns></returns>
		IUpdateBuilder<TModel> Inc<TKey, TTarget>(Expression<Func<TModel, TKey?>> selector, TTarget value, TKey? defaultValue) where TKey : struct;

		/// <summary>
		/// 自增, 不可空类型不留默认值
		/// </summary>
		/// <typeparam name="TTarget">增加值的类型</typeparam>
		/// <typeparam name="TKey">原类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">增量</param>
		/// <exception cref="ArgumentNullException">增量为空</exception>
		/// <returns></returns>
		IUpdateBuilder<TModel> Inc<TKey, TTarget>(Expression<Func<TModel, TKey>> selector, TTarget value);

		/// <summary>
		/// 数组移除某元素
		/// </summary>
		/// <typeparam name="TKey">数组的类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="value">元素</param>
		/// <returns></returns>
		IUpdateBuilder<TModel> Remove<TKey>(Expression<Func<TModel, TKey[]>> selector, TKey value) where TKey : struct;

		/// <summary>
		/// 设置整型等于一个枚举, 可空字段
		/// </summary>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">value</param>
		/// <returns></returns>
		IUpdateBuilder<TModel> Set(Expression<Func<TModel, int?>> selector, [DisallowNull] Enum value);

		/// <summary>
		/// 设置整型等于一个枚举
		/// </summary>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">enum value, disallow null</param>
		/// <exception cref="ArgumentNullException">if value is null</exception>
		/// <returns></returns>
		IUpdateBuilder<TModel> Set(Expression<Func<TModel, int>> selector, [DisallowNull] Enum value);

		/// <summary>
		/// 设置字段等于SQL
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		IUpdateBuilder<TModel> Set<TTarget>(Expression<Func<TModel, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// 设置一个字段值
		/// </summary>
		/// <typeparam name="TKey">字段类型</typeparam>
		/// <param name="selector">字段key selector</param>
		/// <param name="value">value</param>
		/// <returns></returns>
		IUpdateBuilder<TModel> Set<TKey>(Expression<Func<TModel, TKey>> selector, TKey value);

		/// <summary>
		/// 根据实体类主键, 更新数据
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		IUpdateBuilder<TModel> Set(TModel model);

		/// <summary>
		/// SET语句
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		IUpdateBuilder<TModel> Set(string exp);
	}
}