using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Creeper.SqlBuilder
{
	public interface IWhereBuilder<TBuilder, TModel> : ISqlBuilder<TBuilder>
		where TBuilder : class, ISqlBuilder
		where TModel : class, ICreeperModel, new()
	{
		/// <summary>
		/// 是否有where条件存在
		/// </summary>
		bool HasWhere { get; }

		/// <summary>
		/// where语句
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		TBuilder Where(Expression<Func<TModel, bool>> predicate);

		/// <summary>
		/// 字符串where语句
		/// </summary>
		/// <param name="where"></param>
		/// <returns></returns>
		TBuilder Where(string where);

		/// <summary>
		/// where format 写法
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		TBuilder Where(string filter, params object[] values);

		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		TBuilder Where<TSource>(Expression<Func<TModel, TSource, bool>> predicate) where TSource : ICreeperModel, new();

		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		TBuilder Where<TSource>(Expression<Func<TSource, bool>> predicate) where TSource : ICreeperModel, new();

		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource1">关联查询出现过的表</typeparam>
		/// <typeparam name="TSource2">关联查询出现过的表</typeparam>
		/// <typeparam name="TSource3">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		TBuilder Where<TSource1, TSource2, TSource3>(Expression<Func<TSource1, TSource2, TSource3, bool>> predicate)
			where TSource1 : ICreeperModel, new()
			where TSource2 : ICreeperModel, new()
			where TSource3 : ICreeperModel, new();

		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource1">关联查询出现过的表</typeparam>
		/// <typeparam name="TSource2">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		TBuilder Where<TSource1, TSource2>(Expression<Func<TModel, TSource1, TSource2, bool>> predicate)
			where TSource1 : ICreeperModel, new()
			where TSource2 : ICreeperModel, new();

		/// <summary>
		/// 子模型where
		/// </summary>
		/// <typeparam name="TSource1">关联查询出现过的表</typeparam>
		/// <typeparam name="TSource2">关联查询出现过的表</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		TBuilder Where<TSource1, TSource2>(Expression<Func<TSource1, TSource2, bool>> predicate)
			where TSource1 : ICreeperModel, new()
			where TSource2 : ICreeperModel, new();

		/// <summary>
		/// 主键查找
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		TBuilder Where(TModel model);

		/// <summary>
		/// 主键查找
		/// </summary>
		/// <param name="models"></param>
		/// <returns></returns>
		TBuilder Where(IEnumerable<TModel> models);

		/// <summary>
		/// any 方法, optional字段
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		TBuilder WhereAny<TKey>(Expression<Func<TModel, TKey?>> selector, IEnumerable<TKey> values) where TKey : struct;

		/// <summary>
		/// any
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		TBuilder WhereAny<TKey>(Expression<Func<TModel, TKey>> selector, IEnumerable<TKey> values);

		/// <summary>
		/// any方法
		/// </summary>
		/// <typeparam name="TKey">key类型</typeparam>
		/// <param name="key">字段名片</param>
		/// <param name="values">值</param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		TBuilder WhereAny<TKey>(string key, IEnumerable<TKey> values);

		/// <summary>
		/// any 方法, optional字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		TBuilder WhereAny<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, IEnumerable<TKey> values)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// any方法
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		TBuilder WhereAny<TSource, TKey>(Expression<Func<TSource, TKey>> selector, IEnumerable<TKey> values) where TSource : ICreeperModel, new();

		/// <summary>
		/// 如果values是空或长度为0，直接返回default(无论or/and什么条件)
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		TBuilder WhereAnyOrDefault<TKey>(Expression<Func<TModel, TKey>> selector, IEnumerable<TKey> values);

		/// <summary>
		/// any方法 如果values是空或长度为0，直接返回空数据(无论or/and什么条件)
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		TBuilder WhereAnyOrDefault<TSource, TKey>(Expression<Func<TSource, TKey>> selector, IEnumerable<TKey> values) where TSource : ICreeperModel, new();

		/// <summary>
		/// where exists 
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		TBuilder WhereExists<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// where exists 
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		TBuilder WhereExists(Action<ISelectBuilder<TModel>> selectBuilderBinder);

		/// <summary>
		/// where in
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		TBuilder WhereIn<TTarget>(Expression<Func<TModel, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// where in
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <typeparam name="TSource">语句中的其他表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		TBuilder WhereIn<TSource, TTarget>(Expression<Func<TSource, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TSource : ICreeperModel, new() where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// not equals any 方法
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		TBuilder WhereNotAny<TKey>(string key, IEnumerable<TKey> values);

		/// <summary>
		/// not equals any 方法, optional字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		TBuilder WhereNotAny<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, IEnumerable<TKey> values)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// not equals any 方法
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="values"></param>
		/// <exception cref="ArgumentNullException">values is null or length is zero</exception>
		/// <returns></returns>
		TBuilder WhereNotAny<TSource, TKey>(Expression<Func<TSource, TKey>> selector, IEnumerable<TKey> values) where TSource : ICreeperModel, new();

		/// <summary>
		/// where not exists 
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		TBuilder WhereNotExists<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// where not exists 
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		TBuilder WhereNotExists(Action<ISelectBuilder<TModel>> selectBuilderBinder);

		/// <summary>
		/// where not in
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <exception cref="ArgumentNullException">binder is null</exception>
		/// <returns></returns>
		TBuilder WhereNotIn<TTarget>(Expression<Func<TModel, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// where not in
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <typeparam name="TSource">语句中的其他表对象</typeparam>
		/// <exception cref="ArgumentNullException">sql is null</exception>
		/// <returns></returns>
		TBuilder WhereNotIn<TSource, TTarget>(Expression<Func<TSource, dynamic>> selector, Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TSource : ICreeperModel, new() where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// where表达式结束Or，Start与End之前的表达式使用OR连接，而不是原来的AND
		/// </summary>
		/// <returns></returns>
		TBuilder WhereOrEnd();

		/// <summary>
		/// where表达式开始Or，Start与End之前的表达式使用OR连接，而不是原来的AND
		/// </summary>
		/// <returns></returns>
		TBuilder WhereOrStart();
	}
}