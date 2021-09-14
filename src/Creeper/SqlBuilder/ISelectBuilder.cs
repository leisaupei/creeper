using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	public interface ISelectBuilder<TModel> : IWhereBuilder<ISelectBuilder<TModel>, TModel> where TModel : class, ICreeperModel, new()
	{
		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Avg<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default) where TKey : struct;

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Avg<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default);

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Avg<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Avg<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> AvgAsync<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TKey : struct;

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> AvgAsync<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> AvgAsync<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> AvgAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回行数
		/// </summary>
		/// <returns></returns>
		long Count();

		/// <summary>
		/// 返回行数
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<long> CountAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回去重字段行数
		/// </summary>
		/// <returns></returns>
		long CountDistinct(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// 返回去重字段行数
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<long> CountDistinctAsync(Expression<Func<TModel, dynamic>> selector, CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回去重字段行数
		/// </summary>
		/// <returns></returns>
		long CountDistinct<TSource>(Expression<Func<TSource, dynamic>> selector);

		/// <summary>
		/// 返回去重字段行数
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<long> CountDistinctAsync<TSource>(Expression<Func<TModel, dynamic>> selector, CancellationToken cancellationToken = default);

		/// <summary>
		/// 排除另一个ISelectBuilder, 相同的行只会返回一行
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns>差(减法)</returns>
		ISelectBuilder<TModel> Except<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// 排除一个sql语句/视图, 相同的行只会返回一行
		/// </summary>
		/// <param name="view"></param>
		/// <returns>差(减法)</returns>
		ISelectBuilder<TModel> Except(string view);

		/// <summary>
		/// 排除另一个ISelectBuilder, 相同的行会全部返回
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns>差(减法)</returns>
		ISelectBuilder<TModel> ExceptAll<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// 排除一个sql语句/视图, 相同的行会全部返回
		/// </summary>
		/// <param name="view"></param>
		/// <returns>差(减法)</returns>
		ISelectBuilder<TModel> ExceptAll(string view);

		/// <summary>
		/// 设置单个字段 常用于IN系列与EXISTS系列 会采用key selector别名为表别名
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> Field(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// 设置单个字段 常用于IN系列与EXISTS系列
		/// </summary>
		/// <returns></returns>
		ISelectBuilder<TModel> Field(string field);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <exception cref="CreeperFirstNotFoundException">没有查询到数据时抛出此异常</exception>
		/// <returns></returns>
		TModel First();

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <exception cref="CreeperFirstNotFoundException">没有查询到数据时抛出此异常</exception>
		/// <returns></returns>
		Task<TModel> FirstAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <returns></returns>
		TModel FirstOrDefault();

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		TKey FirstOrDefault<TKey>(Expression<Func<TModel, TKey>> selector);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		TResult FirstOrDefault<TResult>(string fields = null);

		/// <summary>
		/// 返回一行, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		TResult FirstOrDefault<TResult>(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		TKey FirstOrDefault<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回一行, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		TResult FirstOrDefault<TSource, TResult>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<TModel> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> FirstOrDefaultAsync<TKey>(Expression<Func<TModel, TKey>> selector, CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="fields"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<TResult> FirstOrDefaultAsync<TResult>(string fields = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回一行, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TModel, dynamic>> selector, CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> FirstOrDefaultAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回一行, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<TResult> FirstOrDefaultAsync<TSource, TResult>(Expression<Func<TSource, dynamic>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回一行(管道)
		/// </summary>
		/// <returns></returns>
		ISelectBuilder<TModel> FirstOrDefaultPipe();

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> FirstOrDefaultPipe<TKey>(Expression<Func<TModel, TKey>> selector);

		/// <summary>
		/// 返回一行(管道)
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="fields">返回字段, 可选</param>
		/// <returns></returns>
		ISelectBuilder<TModel> FirstOrDefaultPipe<TResult>(string fields = null);

		/// <summary>
		/// 返回列表(管道), 匿名类型作为查询字段 
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> FirstOrDefaultPipe<TResult>(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> FirstOrDefaultPipe<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回列表(管道), 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> FirstOrDefaultPipe<TSource, TResult>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <typeparam name="TResult3"></typeparam>
		/// <returns></returns>
		(TModel, TResult1, TResult2, TResult3) FirstOrDefaultUnion<TResult1, TResult2, TResult3>()
			where TResult1 : ICreeperModel, new()
			where TResult2 : ICreeperModel, new()
			where TResult3 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <returns></returns>
		(TModel, TResult1, TResult2) FirstOrDefaultUnion<TResult1, TResult2>()
			where TResult1 : ICreeperModel, new()
			where TResult2 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <returns></returns>
		(TModel, TResult1) FirstOrDefaultUnion<TResult1>() where TResult1 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <typeparam name="TResult3"></typeparam>
		/// <returns></returns>
		ISelectBuilder<TModel> FirstOrDefaultUnionPipe<TResult1, TResult2, TResult3>()
			where TResult1 : ICreeperModel, new()
			where TResult2 : ICreeperModel, new()
			where TResult3 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <returns></returns>
		ISelectBuilder<TModel> FirstOrDefaultUnionPipe<TResult1, TResult2>()
			where TResult1 : ICreeperModel, new()
			where TResult2 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <returns></returns>
		ISelectBuilder<TModel> FirstOrDefaultUnionPipe<TResult1>() where TResult1 : ICreeperModel, new();

		/// <summary>
		/// group by
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		ISelectBuilder<TModel> GroupBy(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// sql语句group by
		/// </summary>
		/// <param name="s"></param>
		/// <example>GroupBy("xxx,xxx")</example>
		/// <returns></returns>
		ISelectBuilder<TModel> GroupBy(string s);

		/// <summary>
		/// group by
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> GroupBy<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 聚合条件
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> Having(string s);

		/// <summary>
		/// inner join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> InnerJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// inner join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> InnerJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// inner join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> InnerJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// inner join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> InnerJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// 相交另一个ISelectBuilder, 相同的行只会返回一行
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns>交集</returns>
		ISelectBuilder<TModel> Intersect<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// 相交一个sql语句/视图, 相同的行只会返回一行
		/// </summary>
		/// <param name="view"></param>
		/// <returns>交集</returns>
		ISelectBuilder<TModel> Intersect(string view);

		/// <summary>
		/// 相交另一个ISelectBuilder, 相同的行会全部返回
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns>交集</returns>
		ISelectBuilder<TModel> IntersectAll<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// 相交一个sql语句/视图, 相同的行会全部返回
		/// </summary>
		/// <param name="view"></param>
		/// <returns>交集</returns>
		ISelectBuilder<TModel> IntersectAll(string view);

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <returns></returns>
		ISelectBuilder<TModel> Join<TTarget>(JoinType joinType, Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <returns></returns>
		ISelectBuilder<TModel> JoinUnion<TTarget>(JoinType joinType, Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <typeparam name="TScource">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <returns></returns>
		ISelectBuilder<TModel> Join<TScource, TTarget>(JoinType joinType, Expression<Func<TScource, TTarget, bool>> predicate) where TScource : ICreeperModel, new() where TTarget : ICreeperModel, new();

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <typeparam name="TScource">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <returns></returns>
		ISelectBuilder<TModel> JoinUnion<TScource, TTarget>(JoinType joinType, Expression<Func<TScource, TTarget, bool>> predicate) where TScource : ICreeperModel, new() where TTarget : ICreeperModel, new();

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <param name="alias">table alias name</param>
		/// <param name="on">on expression</param>
		/// <returns></returns>
		ISelectBuilder<TModel> Join<TTarget>(JoinType joinType, string alias, string on) where TTarget : ICreeperModel, new();

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <param name="alias">table alias name</param>
		/// <param name="on">on expression</param>
		/// <returns></returns>
		ISelectBuilder<TModel> JoinUnion<TTarget>(JoinType joinType, string alias, string on) where TTarget : ICreeperModel, new();

		/// <summary>
		/// left join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> LeftJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// left join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> LeftJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// left join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> LeftJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// left join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> LeftJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// left outer join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		ISelectBuilder<TModel> LeftOuterJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// left outer join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> LeftOuterJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// left outer join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> LeftOuterJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// left outer join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> LeftOuterJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Max<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default) where TKey : struct;

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Max<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Max<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Max<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> MaxAsync<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TKey : struct;

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> MaxAsync<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> MaxAsync<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> MaxAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Min<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default) where TKey : struct;

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Min<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Min<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Min<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> MinAsync<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TKey : struct;

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> MinAsync<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> MinAsync<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> MinAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// order by asc
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		ISelectBuilder<TModel> OrderBy(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// sql语句order by
		/// </summary>
		/// <param name="s"></param>
		/// <example>OrderBy("xxx desc,xxx asc")</example>
		/// <returns></returns>
		ISelectBuilder<TModel> OrderBy(string s);

		/// <summary>
		/// order by asc
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		ISelectBuilder<TModel> OrderBy<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// order by desc
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		ISelectBuilder<TModel> OrderByDescending(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// order by desc
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		ISelectBuilder<TModel> OrderByDescending<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// order by desc nulls last
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		ISelectBuilder<TModel> OrderByDescendingNullsLast(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// order by desc nulls last
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		ISelectBuilder<TModel> OrderByDescendingNullsLast<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// order by asc nulls last
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		ISelectBuilder<TModel> OrderByNullsLast(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// order by asc nulls last
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		ISelectBuilder<TModel> OrderByNullsLast<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 分页
		/// </summary>
		/// <param name="pageIndex">页码</param>
		/// <param name="pageSize">分页大小</param>
		/// <returns></returns>
		ISelectBuilder<TModel> Page(int pageIndex, int pageSize);

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <returns></returns>
		ISelectBuilder<TModel> ToListPipe();

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> ToListPipe<TKey>(Expression<Func<TModel, TKey>> selector);

		/// <summary>
		/// right join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> RightJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// right join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> RightJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// right join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> RightJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// right join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> RightJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// right outer join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> RightOuterJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// right outer join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> RightOuterJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// right outer join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> RightOuterJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate)
			where TSource : ICreeperModel, new()
			where TTarget : ICreeperModel, new();

		/// <summary>
		/// right outer join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> RightOuterJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new();

		/// <summary>
		/// 等同于数据库offset
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> Skip(int i);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Sum<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default) where TKey : struct;

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Sum<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		TKey Sum<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>	
		TKey Sum<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> SumAsync<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TKey : struct;

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> SumAsync<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> SumAsync<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			where TSource : ICreeperModel, new()
			where TKey : struct;

		/// <summary>
		/// 取总和
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <returns></returns>
		ValueTask<TKey> SumAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 随机抽样(仅支持postgresql), 因为效率较高, 所以单独支持了
		/// </summary>
		/// <param name="percent">采样的分数，表示为一个0到100之间的百分数</param>
		/// <returns></returns>
		ISelectBuilder<TModel> TableSampleSystem(double percent);

		/// <summary>
		/// 等同于数据库limit/top
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> Take(int i);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <returns></returns>
		List<TModel> ToList();

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		List<TKey> ToList<TKey>(Expression<Func<TModel, TKey>> selector);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		List<TResult> ToList<TResult>(string fields = null);

		/// <summary>
		/// 返回列表, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		List<TResult> ToList<TResult>(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		List<TKey> ToList<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回列表, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		List<TResult> ToList<TSource, TResult>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <returns></returns>
		Task<List<TModel>> ToListAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<TKey>> ToListAsync<TKey>(Expression<Func<TModel, TKey>> selector, CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="fields"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<TResult>> ToListAsync<TResult>(string fields = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回列表, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<TResult>> ToListAsync<TResult>(Expression<Func<TModel, dynamic>> selector, CancellationToken cancellationToken = default);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<TKey>> ToListAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回列表, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<TResult>> ToListAsync<TSource, TResult>(Expression<Func<TSource, dynamic>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TResult">model type</typeparam>
		/// <param name="fields">指定输出字段</param>
		/// <returns></returns>
		ISelectBuilder<TModel> ToListPipe<TResult>(string fields = null);

		/// <summary>
		/// 返回列表(管道), 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> ToListPipe<TResult>(Expression<Func<TModel, dynamic>> selector);

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> ToListPipe<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回列表(管道), 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> ToListPipe<TSource, TResult>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <typeparam name="TResult3"></typeparam>
		/// <returns></returns>
		List<(TModel, TResult1, TResult2, TResult3)> ToListUnion<TResult1, TResult2, TResult3>()
			where TResult1 : ICreeperModel, new()
			where TResult2 : ICreeperModel, new()
			where TResult3 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <returns></returns>
		List<(TModel, TResult1, TResult2)> ToListUnion<TResult1, TResult2>()
			where TResult1 : ICreeperModel, new()
			where TResult2 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <returns></returns>
		List<(TModel, TResult1)> ToListUnion<TResult1>() where TResult1 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体列表(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <typeparam name="TResult3"></typeparam>
		/// <returns></returns>
		ISelectBuilder<TModel> ToListUnionPipe<TResult1, TResult2, TResult3>()
			where TResult1 : ICreeperModel, new()
			where TResult2 : ICreeperModel, new()
			where TResult3 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体列表(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <returns></returns>
		ISelectBuilder<TModel> ToListUnionPipe<TResult1, TResult2>()
			where TResult1 : ICreeperModel, new()
			where TResult2 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体列表(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <returns></returns>
		ISelectBuilder<TModel> ToListUnionPipe<TResult1>() where TResult1 : ICreeperModel, new();

		/// <summary>
		/// Execute scalar，表达式入参请使用FirstOrDefault方法
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		object ToScalar(string field = null);

		/// <summary>
		/// Execute scalar，表达式入参请使用FirstOrDefault方法
		/// </summary>
		/// <param name="field"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<object> ToScalarAsync(string field = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Execute scalar，表达式入参请使用FirstOrDefault方法
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="field"></param>
		/// <returns></returns>
		TKey ToScalar<TKey>(string field = null);

		/// <summary>
		/// Execute scalar，表达式入参请使用FirstOrDefault方法
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="field"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<TKey> ToScalarAsync<TKey>(string field = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 合并另一个ISelectBuilder, 相同的行只会返回一行
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		ISelectBuilder<TModel> Union<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// 合并一个sql语句/视图, 相同的行只会返回一行
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> Union(string view);

		/// <summary>
		/// 合并另一个ISelectBuilder, 相同的行会全部返回
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		ISelectBuilder<TModel> UnionAll<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new();

		/// <summary>
		/// 合并一个sql语句/视图, 相同的行会全部返回
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		ISelectBuilder<TModel> UnionAll(string view);

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		Task<(TModel, T1, T2, T3)> UnionFirstOrDefaultAsync<T1, T2, T3>(CancellationToken cancellationToken = default)
			where T1 : ICreeperModel, new()
			where T2 : ICreeperModel, new()
			where T3 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		Task<(TModel, T1, T2)> UnionFirstOrDefaultAsync<T1, T2>(CancellationToken cancellationToken = default)
			where T1 : ICreeperModel, new()
			where T2 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		Task<(TModel, T1)> UnionFirstOrDefaultAsync<T1>(CancellationToken cancellationToken = default) where T1 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		Task<List<(TModel, T1, T2, T3)>> UnionToListAsync<T1, T2, T3>(CancellationToken cancellationToken = default)
			where T1 : ICreeperModel, new()
			where T2 : ICreeperModel, new()
			where T3 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		Task<List<(TModel, T1, T2)>> UnionToListAsync<T1, T2>(CancellationToken cancellationToken = default)
			where T1 : ICreeperModel, new()
			where T2 : ICreeperModel, new();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		Task<List<(TModel, T1)>> UnionToListAsync<T1>(CancellationToken cancellationToken = default) where T1 : ICreeperModel, new();

	}
}