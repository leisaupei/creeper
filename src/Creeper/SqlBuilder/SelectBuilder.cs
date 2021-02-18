using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// select 语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class SelectBuilder<TModel> : WhereBuilder<SelectBuilder<TModel>, TModel>
		where TModel : class, ICreeperDbModel, new()
	{

		private readonly List<UnionModel> _unions = new List<UnionModel>();
		#region Identity
		private string _groupBy;
		private string _orderBy;
		private int? _limit;
		private int? _offset;
		private string _having;
		private string _union;
		private string _tablesampleSystem;
		private string _distinctOn;
		private string _except;
		#endregion

		#region Constructor
		internal SelectBuilder(ICreeperDbContext dbContext) : base(dbContext) { }

		internal SelectBuilder(ICreeperDbExecute dbExecute) : base(dbExecute) { }
		#endregion

		#region Field
		/// <summary>
		/// 设置单个字段 常用于IN系列与EXISTS系列 会采用key selector别名为表别名
		/// </summary>
		/// <param name="selector"></param>
		/// <returns>ISqlBuilder</returns>
		public SelectBuilder<TModel> Field(Expression<Func<TModel, object>> selector)
		{
			var key = GetSelector(selector);
			Fields = key;
			MainAlias = key.Split('.')[0];
			return this;
		}

		/// <summary>
		/// 设置单个字段 常用于IN系列与EXISTS系列
		/// </summary>
		/// <returns>ISqlBuilder</returns>
		public SelectBuilder<TModel> Field(string field)
		{
			if (field.Contains('.'))
			{
				var arr = field.Split('.');
				Fields = arr[1];
				MainAlias = arr[0];
			}
			else
				Fields = field;
			return this;
		}
		#endregion

		#region KeyWord
		/// <summary>
		/// sql语句group by
		/// </summary>
		/// <param name="s"></param>
		/// <example>GroupBy("xxx,xxx")</example>
		/// <returns></returns>
		public SelectBuilder<TModel> GroupBy(string s)
		{
			if (!string.IsNullOrEmpty(_groupBy))
				_groupBy += ", ";
			_groupBy += s;
			return this;
		}

		/// <summary>
		/// sql语句order by
		/// </summary>
		/// <param name="s"></param>
		/// <example>OrderBy("xxx desc,xxx asc")</example>
		/// <returns></returns>
		public SelectBuilder<TModel> OrderBy(string s)
		{
			if (!string.IsNullOrEmpty(_orderBy))
				_orderBy += ", ";
			_orderBy += s;
			return this;
		}

		/// <summary>
		/// having
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> Having(string s)
		{
			_having = s;
			return this;
		}

		/// <summary>
		/// limit
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> Limit(int i)
		{
			_limit = i;
			return this;
		}

		/// <summary>
		/// 等于数据库offset
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> Skip(int i)
		{
			_offset = i;
			return this;
		}

		/// <summary>
		/// 连接一个sql语句
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> Union(string view)
		{
			_union = $"({view})";
			return this;
		}

		/// <summary>
		/// 连接selectBuilder
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> Union(ISqlBuilder sqlBuilder)
		{
			_union = $"({sqlBuilder.CommandText})";
			return AddParameters(sqlBuilder.Params);
		}

		/// <summary>
		/// 排除一个sql语句
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> Except(string view)
		{
			_except = $"({view})";
			return this;
		}

		/// <summary>
		/// 排除selectBuilder
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> Except(ISqlBuilder sqlBuilder)
		{
			_except = $"({sqlBuilder.CommandText})";
			return AddParameters(sqlBuilder.Params);
		}

		/// <summary>
		/// 分页
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> Page(int pageIndex, int pageSize)
		{
			Limit(pageSize); Skip(Math.Max(0, pageIndex - 1) * pageSize);
			return this;
		}

		/// <summary>
		/// 随机抽样
		/// </summary>
		/// <param name="percent">seed</param>
		/// <returns></returns>
		public SelectBuilder<TModel> TableSampleSystem(double percent)
		{
			_tablesampleSystem = $" tablesample system({percent}) ";
			return this;
		}

		/// <summary>
		/// 去除重复, 建议与order by连用
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public SelectBuilder<TModel> DistinctOn(Expression<Func<TModel, object>> selector)
		{
			_distinctOn = GetSelector(selector);
			return this;
		}

		/// <summary>
		/// group by
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public SelectBuilder<TModel> GroupBy(Expression<Func<TModel, object>> selector)
			=> GroupBy<TModel>(selector);

		/// <summary>
		/// group by
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> GroupBy<TSource>(Expression<Func<TSource, object>> selector) where TSource : ICreeperDbModel, new()
			=> GroupBy(GetSelector(selector));

		/// <summary>
		/// order by asc
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <param name="isNullsLast">use nulls last</param>
		/// <returns></returns>
		public SelectBuilder<TModel> OrderBy(Expression<Func<TModel, object>> selector, bool isNullsLast = false)
			=> OrderBy<TModel>(selector, isNullsLast);

		/// <summary>
		/// order by desc
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <param name="isNullsLast">is nulls last</param>
		/// <returns></returns>
		public SelectBuilder<TModel> OrderByDescending(Expression<Func<TModel, object>> selector, bool isNullsLast = false)
			=> OrderByDescending<TModel>(selector, isNullsLast);

		/// <summary>
		/// order by asc
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="isNullsLast">is nulls last</param>
		/// <returns></returns>
		public SelectBuilder<TModel> OrderBy<TSource>(Expression<Func<TSource, object>> selector, bool isNullsLast = false) where TSource : ICreeperDbModel, new()
			=> OrderBy(string.Concat(GetSelector(selector), isNullsLast ? " NULLS LAST" : ""));

		/// <summary>
		/// order by desc
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="isNullsLast">is nulls last</param>
		/// <returns></returns>
		public SelectBuilder<TModel> OrderByDescending<TSource>(Expression<Func<TSource, object>> selector, bool isNullsLast = false) where TSource : ICreeperDbModel, new()
			=> OrderBy(string.Concat(GetSelector(selector), " desc", isNullsLast ? " NULLS LAST" : ""));
		#endregion

		#region ToList
		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fields"></param>
		/// <returns></returns>
		public List<T> ToList<T>(string fields = null)
		{
			if (!string.IsNullOrEmpty(fields)) Fields = fields;
			if (IsReturnDefault) return new List<T>();
			return base.ToList<T>();
		}

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public List<TKey> ToList<TKey>(Expression<Func<TModel, TKey>> selector) => ToList<TModel, TKey>(selector);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public List<TKey> ToList<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperDbModel, new()
			=> ToList<TKey>(GetSelector(selector));

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <returns></returns>
		public List<TModel> ToList() => ToList<TModel>();
		#endregion

		#region ToOne
		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fields"></param>
		/// <returns></returns>
		public T ToOne<T>(string fields = null)
		{
			Limit(1);
			if (!string.IsNullOrEmpty(fields)) Fields = fields;
			return base.ToOne<T>();
		}

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <returns></returns>
		public TModel ToOne()
			=> ToOne<TModel>();

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TKey ToOne<TKey>(Expression<Func<TModel, TKey>> selector)
			=> ToOne<TModel, TKey>(selector);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TKey ToOne<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperDbModel, new()
			=> ToScalar(selector);

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="fields"></param>
		/// <returns></returns>
		public TKey ToScalar<TKey>(string fields)
		{
			Limit(1);
			Fields = fields;
			return ToScalar<TKey>();
		}

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TKey ToScalar<TKey>(Expression<Func<TModel, TKey>> selector)
			=> ToScalar<TModel, TKey>(selector);

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TKey ToScalar<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperDbModel, new()
			=> ToScalar<TKey>(GetSelector(selector));
		#endregion

		#region Single Method
		/// <summary>
		/// 返回行数
		/// </summary>
		/// <returns></returns>
		public long Count() => ToScalar<long>("COUNT(1)");

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Max<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperDbModel, new()
			=> ScalarTransfer(selector, "MAX", defaultValue);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Min<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperDbModel, new()
			=> ScalarTransfer(selector, "MIN", defaultValue);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Sum<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperDbModel, new()
			=> ScalarTransfer(selector, "SUM", defaultValue);

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Avg<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperDbModel, new()
			=> ScalarTransfer(selector, "AVG", defaultValue);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Max<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default)
			=> Max<TModel, TKey>(selector, defaultValue);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Min<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default)
			=> Min<TModel, TKey>(selector, defaultValue);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Sum<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default)
			=> Sum<TModel, TKey>(selector, defaultValue);

		/// <summary>
		/// 去平均值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Avg<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default)
			=> Avg<TModel, TKey>(selector, defaultValue);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Max<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default) where TSource : ICreeperDbModel, new() where TKey : struct
			=> ScalarTransfer(selector, "MAX", defaultValue);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Min<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default) where TSource : ICreeperDbModel, new() where TKey : struct
			=> ScalarTransfer(selector, "MIN", defaultValue);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Sum<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default) where TSource : ICreeperDbModel, new() where TKey : struct
			=> ScalarTransfer(selector, "SUM", defaultValue);

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Avg<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default) where TSource : ICreeperDbModel, new() where TKey : struct
			=> ScalarTransfer(selector, "AVG", defaultValue);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Max<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default) where TKey : struct
			=> Max<TModel, TKey>(selector, defaultValue);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Min<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default) where TKey : struct
			=> Min<TModel, TKey>(selector, defaultValue);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Sum<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default) where TKey : struct
			=> Sum<TModel, TKey>(selector, defaultValue);

		/// <summary>
		/// 去平均值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Avg<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default) where TKey : struct
			=> Avg<TModel, TKey>(selector, defaultValue);
		#endregion

		#region Async

		#region ToList
		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fields"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<List<T>> ToListAsync<T>(string fields = null, CancellationToken cancellationToken = default)
		{
			if (!string.IsNullOrEmpty(fields)) Fields = fields;
			if (IsReturnDefault) return Task.FromResult(new List<T>());
			return base.ToListAsync<T>(cancellationToken);
		}

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<List<TKey>> ToListAsync<TKey>(Expression<Func<TModel, TKey>> selector, CancellationToken cancellationToken = default)
			=> ToListAsync<TModel, TKey>(selector, cancellationToken);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<List<TKey>> ToListAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperDbModel, new()
			=> ToListAsync<TKey>(GetSelector(selector), cancellationToken);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <returns></returns>
		public Task<List<TModel>> ToListAsync(CancellationToken cancellationToken = default)
			=> ToListAsync<TModel>(cancellationToken);
		#endregion

		#region ToOne
		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fields"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<T> ToOneAsync<T>(string fields = null, CancellationToken cancellationToken = default)
		{
			Limit(1);
			if (!string.IsNullOrEmpty(fields)) Fields = fields;
			return base.ToOneAsync<T>(cancellationToken);
		}

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<TModel> ToOneAsync(CancellationToken cancellationToken = default)
			=> ToOneAsync<TModel>(cancellationToken);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> ToOneAsync<TKey>(Expression<Func<TModel, TKey>> selector, CancellationToken cancellationToken = default)
			=> ToOneAsync<TModel, TKey>(selector, cancellationToken);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> ToOneAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperDbModel, new()
			=> ToScalarAsync(selector, cancellationToken);

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="fields"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> ToScalarAsync<TKey>(string fields, CancellationToken cancellationToken = default)
		{
			Limit(1);
			Fields = fields;
			return base.ToScalarAsync<TKey>(cancellationToken);
		}

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> ToScalarAsync<TKey>(Expression<Func<TModel, TKey>> selector, CancellationToken cancellationToken = default)
			=> ToScalarAsync<TModel, TKey>(selector, cancellationToken);

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> ToScalarAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperDbModel, new()
			=> ToScalarAsync<TKey>(GetSelector(selector), cancellationToken);
		#endregion

		#region ToOneUnion
		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		public Task<(TModel, T1)> ToOneUnionAsync<T1>(CancellationToken cancellationToken = default) where T1 : ICreeperDbModel, new()
			=> ToOneAsync<(TModel, T1)>(cancellationToken);

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		public Task<(TModel, T1, T2)> ToOneUnionAsync<T1, T2>(CancellationToken cancellationToken = default) where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new()
			=> ToOneAsync<(TModel, T1, T2)>(cancellationToken);

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		public Task<(TModel, T1, T2, T3)> ToOneUnionAsync<T1, T2, T3>(CancellationToken cancellationToken = default) where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new() where T3 : ICreeperDbModel, new()
			=> ToOneAsync<(TModel, T1, T2, T3)>(cancellationToken);
		#endregion

		#region ToListUnion
		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		public Task<List<(TModel, T1)>> ToListUnionAsync<T1>(CancellationToken cancellationToken = default) where T1 : ICreeperDbModel, new()
			=> ToListAsync<(TModel, T1)>(cancellationToken);

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		public Task<List<(TModel, T1, T2)>> ToListUnionAsync<T1, T2>(CancellationToken cancellationToken = default) where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new()
			=> ToListAsync<(TModel, T1, T2)>(cancellationToken);

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		public Task<List<(TModel, T1, T2, T3)>> ToListUnionAsync<T1, T2, T3>(CancellationToken cancellationToken = default) where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new() where T3 : ICreeperDbModel, new()
			=> ToListAsync<(TModel, T1, T2, T3)>(cancellationToken);
		#endregion

		#region SingleMethod

		/// <summary>
		/// 返回行数
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<long> CountAsync(CancellationToken cancellationToken = default) => ToScalarAsync<long>("COUNT(1)", cancellationToken);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> MaxAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperDbModel, new()
			=> ScalarTransferAsync(selector, "MAX", defaultValue, cancellationToken);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> MinAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperDbModel, new()
			=> ScalarTransferAsync(selector, "MIN", defaultValue, cancellationToken);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> SumAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperDbModel, new()
			=> ScalarTransferAsync(selector, "SUM", defaultValue, cancellationToken);

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> AvgAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperDbModel, new()
			=> ScalarTransferAsync(selector, "AVG", defaultValue, cancellationToken);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> MaxAsync<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			=> MaxAsync<TModel, TKey>(selector, defaultValue, cancellationToken);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> MinAsync<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			=> MinAsync<TModel, TKey>(selector, defaultValue, cancellationToken);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> SumAsync<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			=> SumAsync<TModel, TKey>(selector, defaultValue, cancellationToken);

		/// <summary>
		/// 去平均值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> AvgAsync<TKey>(Expression<Func<TModel, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			=> AvgAsync<TModel, TKey>(selector, defaultValue, cancellationToken);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> MaxAsync<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			where TSource : ICreeperDbModel, new() where TKey : struct
			=> ScalarTransferAsync(selector, "MAX", defaultValue, cancellationToken);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> MinAsync<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			where TSource : ICreeperDbModel, new() where TKey : struct
			=> ScalarTransferAsync(selector, "MIN", defaultValue, cancellationToken);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> SumAsync<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			where TSource : ICreeperDbModel, new() where TKey : struct
			=> ScalarTransferAsync(selector, "SUM", defaultValue, cancellationToken);

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> AvgAsync<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default)
			where TSource : ICreeperDbModel, new() where TKey : struct
			=> ScalarTransferAsync(selector, "AVG", defaultValue, cancellationToken);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> MaxAsync<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TKey : struct
			=> MaxAsync<TModel, TKey>(selector, defaultValue, cancellationToken);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> MinAsync<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TKey : struct
			=> MinAsync<TModel, TKey>(selector, defaultValue, cancellationToken);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> SumAsync<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TKey : struct
			=> SumAsync<TModel, TKey>(selector, defaultValue, cancellationToken);

		/// <summary>
		/// 去平均值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> AvgAsync<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TKey : struct
			=> AvgAsync<TModel, TKey>(selector, defaultValue, cancellationToken);
		#endregion

		#endregion

		#region Pipe
		#region ToOne

		/// <summary>
		/// 返回一行(管道)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fields">返回字段, 可选</param>
		/// <returns></returns>
		public SelectBuilder<TModel> ToOnePipe<T>(string fields = null)
		{
			Limit(1);
			if (!string.IsNullOrEmpty(fields)) Fields = fields;
			return base.ToPipe<T>(PipeReturnType.One);
		}

		/// <summary>
		/// 返回一行(管道)
		/// </summary>
		/// <param name="fields">返回字段, 可选</param>
		/// <returns></returns>
		public SelectBuilder<TModel> ToOnePipe(string fields = null)
			=> ToOnePipe<TModel>(fields);

		/// <summary>
		/// 返回联表实体(管道)
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		public SelectBuilder<TModel> ToOneUnionPipe<T1>() where T1 : ICreeperDbModel, new()
			=> ToOnePipe<(TModel, T1)>();

		/// <summary>
		/// 返回联表实体(管道)
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		public SelectBuilder<TModel> ToOneUnionPipe<T1, T2>() where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new()
			=> ToOnePipe<(TModel, T1, T2)>();

		/// <summary>
		/// 返回联表实体(管道)
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		public SelectBuilder<TModel> ToOneUnionPipe<T1, T2, T3>() where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new() where T3 : ICreeperDbModel, new()
			=> ToOnePipe<(TModel, T1, T2, T3)>();
		#endregion

		#region ToList
		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="T">model type</typeparam>
		/// <param name="fields">指定输出字段</param>
		/// <returns></returns>
		public SelectBuilder<TModel> ToListPipe<T>(string fields = null)
		{
			if (!string.IsNullOrEmpty(fields)) Fields = fields;
			return base.ToPipe<T>(PipeReturnType.List);
		}

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public SelectBuilder<TModel> ToListPipe(string fields = null) => ToListPipe<TModel>(fields);

		/// <summary>
		/// 返回联表实体列表(管道)
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		public SelectBuilder<TModel> ToListUnionPipe<T1>() where T1 : ICreeperDbModel, new()
			=> ToListPipe<(TModel, T1)>();

		/// <summary>
		/// 返回联表实体列表(管道)
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		public SelectBuilder<TModel> ToListUnionPipe<T1, T2>() where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new()
			=> ToListPipe<(TModel, T1, T2)>();

		/// <summary>
		/// 返回联表实体列表(管道)
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		public SelectBuilder<TModel> ToListUnionPipe<T1, T2, T3>() where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new() where T3 : ICreeperDbModel, new()
			=> ToListPipe<(TModel, T1, T2, T3)>();

		#endregion
		#endregion

		#region Union
		/// <summary>
		/// inner join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> InnerJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate, bool isReturn = false) where TTarget : ICreeperDbModel, new()
			=> InnerJoin<TModel, TTarget>(predicate, isReturn);

		/// <summary>
		/// left join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> LeftJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate, bool isReturn = false) where TTarget : ICreeperDbModel, new()
				  => LeftJoin<TModel, TTarget>(predicate, isReturn);

		/// <summary>
		/// right join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> RightJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate, bool isReturn = false) where TTarget : ICreeperDbModel, new()
				  => RightJoin<TModel, TTarget>(predicate, isReturn);

		/// <summary>
		/// left outer join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> LeftOuterJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate, bool isReturn = false) where TTarget : ICreeperDbModel, new()
				  => LeftJoin<TModel, TTarget>(predicate, isReturn);

		/// <summary>
		/// right outer join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> RightOuterJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate, bool isReturn = false) where TTarget : ICreeperDbModel, new()
				  => RightJoin<TModel, TTarget>(predicate, isReturn);

		/// <summary>
		/// inner join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> InnerJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate, bool isReturn = false) where TSource : ICreeperDbModel, new() where TTarget : ICreeperDbModel, new()
			=> Join(predicate, UnionEnum.INNER_JOIN, isReturn);

		/// <summary>
		/// left join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> LeftJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate, bool isReturn = false) where TSource : ICreeperDbModel, new() where TTarget : ICreeperDbModel, new()
			=> Join(predicate, UnionEnum.LEFT_JOIN, isReturn);

		/// <summary>
		/// right join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> RightJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate, bool isReturn = false) where TSource : ICreeperDbModel, new() where TTarget : ICreeperDbModel, new()
			=> Join(predicate, UnionEnum.RIGHT_JOIN, isReturn);

		/// <summary>
		/// left outer join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> LeftOuterJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate, bool isReturn = false) where TSource : ICreeperDbModel, new() where TTarget : ICreeperDbModel, new()
			=> Join(predicate, UnionEnum.LEFT_OUTER_JOIN, isReturn);

		/// <summary>
		/// right outer join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> RightOuterJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate, bool isReturn = false) where TSource : ICreeperDbModel, new() where TTarget : ICreeperDbModel, new()
			=> Join(predicate, UnionEnum.RIGHT_OUTER_JOIN, isReturn);

		/// <summary>
		/// join base method
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <param name="unionType">union type</param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		private SelectBuilder<TModel> Join<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate, UnionEnum unionType, bool isReturn = false) where TTarget : ICreeperDbModel, new() where TSource : ICreeperDbModel, new()
		{
			var expression = GetExpression(predicate);
			var unionAlias = MainAlias;
			foreach (var p in expression.Alias)
			{
				if (p != MainAlias && !_unions.Any(a => a.AliasName == p))
				{
					unionAlias = p;
					break;
				}
			}

			var info = new UnionModel(unionAlias, EntityHelper.GetDbTable<TTarget>().TableName, expression.CmdText, unionType, isReturn);
			if (info.IsReturn)
				info.Fields = EntityHelper.GetModelTypeFieldsString<TTarget>(unionAlias);
			_unions.Add(info);
			return AddParameters(expression.Parameters);
		}

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="unionType">union type</param>
		/// <param name="alias">table alias name</param>
		/// <param name="on">on expression</param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		public SelectBuilder<TModel> Join<TTarget>(UnionEnum unionType, string alias, string on, bool isReturn = false) where TTarget : ICreeperDbModel, new()
		{
			var info = new UnionModel(alias, EntityHelper.GetDbTable<TTarget>().TableName, on, unionType, isReturn);
			if (info.IsReturn)
				info.Fields = EntityHelper.GetModelTypeFieldsString<TTarget>(alias);
			_unions.Add(info);

			return this;
		}
		#endregion

		#region ToUnion
		#region ToOne
		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		public (TModel, T1) ToOneUnion<T1>() where T1 : ICreeperDbModel, new()
			=> ToOne<(TModel, T1)>();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		public (TModel, T1, T2) ToOneUnion<T1, T2>() where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new()
			=> ToOne<(TModel, T1, T2)>();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		public (TModel, T1, T2, T3) ToOneUnion<T1, T2, T3>() where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new() where T3 : ICreeperDbModel, new()
			=> ToOne<(TModel, T1, T2, T3)>();
		#endregion

		#region ToList
		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		public List<(TModel, T1)> ToListUnion<T1>() where T1 : ICreeperDbModel, new()
			=> ToList<(TModel, T1)>();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		public List<(TModel, T1, T2)> ToListUnion<T1, T2>() where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new()
			=> ToList<(TModel, T1, T2)>();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		public List<(TModel, T1, T2, T3)> ToListUnion<T1, T2, T3>() where T1 : ICreeperDbModel, new() where T2 : ICreeperDbModel, new() where T3 : ICreeperDbModel, new()
			=> ToList<(TModel, T1, T2, T3)>();
		#endregion
		#endregion

		#region Override
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
			=> base.ToString();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public new string ToString(string field)
			=> base.ToString(field);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string GetCommandText()
		{
			if (string.IsNullOrEmpty(Fields))
				Fields = EntityHelper.GetModelTypeFieldsString<TModel>(MainAlias);
			var field = new StringBuilder();
			var union = new StringBuilder();
			if (!string.IsNullOrEmpty(_distinctOn))
				field.AppendLine(string.Concat("DISTINCT ON (", _distinctOn, ")"));
			field.Append(Fields);
			foreach (var item in _unions)
			{
				union.AppendLine(string.Format("{0} {1} {2} ON {3}", item.UnionTypeString, item.Table, item.AliasName, item.Expression));
				if (item.IsReturn) field.Append(", ").Append(item.Fields);
			}
			StringBuilder sqlText = new StringBuilder($"SELECT {field} FROM {MainTable} {MainAlias} {_tablesampleSystem} {union}");

			// other
			if (WhereList?.Count() > 0)
				sqlText.AppendLine("WHERE " + string.Join(" AND ", WhereList));

			if (!string.IsNullOrEmpty(_groupBy))
				sqlText.AppendLine(string.Concat("GROUP BY ", _groupBy));

			if (!string.IsNullOrEmpty(_groupBy) && !string.IsNullOrEmpty(_having))
				sqlText.AppendLine(string.Concat("HAVING ", _having));

			if (!string.IsNullOrEmpty(_orderBy))
				sqlText.AppendLine(string.Concat("ORDER BY ", _orderBy));

			if (_limit.HasValue)
				sqlText.AppendLine(string.Concat("LIMIT ", _limit));

			if (_offset.HasValue)
				sqlText.AppendLine(string.Concat("OFFSET ", _offset));

			if (!string.IsNullOrEmpty(_union))
				sqlText.AppendLine(string.Concat("UNION ", _union));

			if (!string.IsNullOrEmpty(_except))
				sqlText.AppendLine(string.Concat("EXCEPT ", _except));
			return sqlText.ToString().TrimEnd();
		}
		#endregion

		#region Protected Method
		private TKey ScalarTransfer<TKey>(Expression selector, string method, TKey defaultValue)
		{
			var visit = GetExpression(selector);
			AddParameters(visit.Parameters);
			AddParameter(out string pName, defaultValue);
			return ToScalar<TKey>($"COALESCE({method}({visit.CmdText}),@{pName})");
		}

		private ValueTask<TKey> ScalarTransferAsync<TKey>(Expression selector, string method, TKey defaultValue, CancellationToken cancellationToken)
		{
			var visit = GetExpression(selector);
			AddParameters(visit.Parameters);
			AddParameter(out string pName, defaultValue);
			return ToScalarAsync<TKey>($"COALESCE({method}({visit.CmdText}),@{pName})", cancellationToken);
		}
		#endregion
	}
}
