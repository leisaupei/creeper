using Creeper.Utils;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder.Impi
{
	/// <summary>
	/// select 语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	internal sealed class SelectBuilder<TModel> : WhereBuilder<ISelectBuilder<TModel>, TModel>, ISelectBuilder<TModel> where TModel : class, ICreeperModel, new()
	{
		#region Identity
		private bool _distinct;
		private string _groupBy;
		private string _orderBy;
		private int? _limit;
		private int? _offset;
		private string _having;
		private string _union;
		private string _tablesampleSystem;
		private string _except;
		private string _intersect;
		private readonly List<JoinInfo> _unions = new List<JoinInfo>();
		#endregion

		#region Constructor
		internal SelectBuilder() : base(context: null) { }

		internal SelectBuilder(ICreeperContext context) : base(context) { }

		internal SelectBuilder(ICreeperExecute execute) : base(execute) { }
		#endregion

		#region Field
		/// <summary>
		/// 设置单个字段 常用于IN系列与EXISTS系列 会采用key selector别名为表别名
		/// </summary>
		/// <param name="selector"></param>
		/// <returns>ISqlBuilder</returns>
		public ISelectBuilder<TModel> Field(Expression<Func<TModel, dynamic>> selector)
		{
			var key = Translator.GetSelector(selector);
			Fields = key;
			MainAlias = key.Split('.')[0];
			return this;
		}

		/// <summary>
		/// 设置单个字段 常用于IN系列与EXISTS系列
		/// </summary>
		/// <returns>ISqlBuilder</returns>
		public ISelectBuilder<TModel> Field(string field)
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
		public ISelectBuilder<TModel> GroupBy(string s)
		{
			if (!string.IsNullOrEmpty(_groupBy)) _groupBy += ',';
			_groupBy += s;
			return this;
		}

		/// <summary>
		/// 等同于数据库offset
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Having(string s)
		{
			_having = s;
			return this;
		}

		/// <summary>
		/// 等同于数据库offset
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Take(int i)
		{
			_limit = i;
			return this;
		}

		/// <summary>
		/// 等同于数据库offset
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Skip(int i)
		{
			_offset = i;
			return this;
		}

		/// <summary>
		/// 分页
		/// </summary>
		/// <param name="pageIndex"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Page(int pageIndex, int pageSize) => Take(pageSize).Skip(Math.Max(0, pageIndex - 1) * pageSize);

		/// <summary>
		/// 随机抽样(仅支持postgresql)
		/// </summary>
		/// <param name="percent">采样的分数，表示为一个0到100之间的百分数</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> TableSampleSystem(double percent)
		{
			_tablesampleSystem = $" tablesample system({percent}) ";
			return this;
		}

		/// <summary>
		/// group by
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> GroupBy(Expression<Func<TModel, dynamic>> selector)
			=> GroupBy<TModel>(selector);

		/// <summary>
		/// group by
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> GroupBy<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new()
			=> GroupBy(Translator.GetSelector(selector));

		#endregion

		#region UNION/EXCEPT/INTERSECT
		/// <summary>
		/// 合并一个sql语句, 相同的行只会返回一行
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Union(string view)
		{
			if (!string.IsNullOrEmpty(_union)) _union += Environment.NewLine;
			_union = $"UNION {view}";
			return this;
		}

		/// <summary>
		/// 合并另一个ISelectBuilder, 相同的行只会返回一行
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> Union<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			return Union(selectBuilder.CommandText).AddParameters(selectBuilder.Params);
		}

		/// <summary>
		/// 合并一个sql语句, 相同的行会全部返回
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> UnionAll(string view)
		{
			if (!string.IsNullOrEmpty(_union)) _union += Environment.NewLine;
			_union += $"UNION ALL {view}";
			return this;
		}

		/// <summary>
		/// 合并另一个ISelectBuilder, 相同的行会全部返回
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> UnionAll<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			return UnionAll(selectBuilder.CommandText).AddParameters(selectBuilder.Params);
		}

		/// <summary>
		/// 排除一个sql语句, 相同的行只会返回一行
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Except(string view)
		{
			if (!string.IsNullOrEmpty(_except)) _except += Environment.NewLine;
			_except += string.Concat(DbConverter.ExceptKeyName, " ", view);
			return this;
		}

		/// <summary>
		/// 排除另一个ISelectBuilder, 相同的行只会返回一行
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> Except<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			return Except(selectBuilder.CommandText).AddParameters(selectBuilder.Params);
		}

		/// <summary>
		/// 排除一个sql语句, 相同的行会全部返回
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> ExceptAll(string view)
		{
			if (!string.IsNullOrEmpty(_except)) _except += Environment.NewLine;
			_except += string.Concat(DbConverter.ExceptKeyName, " ALL ", view);
			return this;
		}

		/// <summary>
		/// 排除另一个ISelectBuilder, 相同的行会全部返回
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> ExceptAll<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			return ExceptAll(selectBuilder.CommandText).AddParameters(selectBuilder.Params);
		}

		/// <summary>
		/// 与一个sql语句的交集, 相同的行只会返回一行
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Intersect(string view)
		{
			if (!string.IsNullOrEmpty(_intersect)) _intersect += Environment.NewLine;
			_intersect += $"INTERSECT {view}";
			return this;
		}

		/// <summary>
		/// 与另一个ISelectBuilder的交集, 相同的行只会返回一行
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> Intersect<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			return Intersect(selectBuilder.CommandText).AddParameters(selectBuilder.Params);
		}

		/// <summary>
		/// 与一个sql语句的交集, 相同的行会全部返回
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> IntersectAll(string view)
		{
			if (!string.IsNullOrEmpty(_intersect)) _intersect += Environment.NewLine;
			_intersect += $"INTERSECT ALL {view}";
			return this;
		}

		/// <summary>
		/// 与另一个ISelectBuilder的交集, 相同的行会全部返回
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> IntersectAll<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			var selectBuilder = BindSelectBuilder(selectBuilderBinder);
			return IntersectAll(selectBuilder.CommandText).AddParameters(selectBuilder.Params);
		}
		#endregion

		#region OrderBy
		/// <summary>
		/// sql语句order by
		/// </summary>
		/// <param name="s"></param>
		/// <example>OrderBy("xxx desc,xxx asc")</example>
		/// <returns></returns>
		public ISelectBuilder<TModel> OrderBy(string s)
		{
			if (!string.IsNullOrEmpty(_orderBy)) _orderBy += ',';
			_orderBy += s;
			return this;
		}

		/// <summary>
		/// order by asc
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> OrderBy(Expression<Func<TModel, dynamic>> selector)
			=> OrderBy<TModel>(selector);

		/// <summary>
		/// order by desc
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> OrderByDescending(Expression<Func<TModel, dynamic>> selector)
			=> OrderByDescending<TModel>(selector);

		/// <summary>
		/// order by asc
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> OrderBy<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new()
			=> OrderBy(DbConverter.ExplainOrderBy(Translator.GetSelector(selector), true, false));

		/// <summary>
		/// order by desc
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> OrderByDescending<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new()
			=> OrderBy(DbConverter.ExplainOrderBy(Translator.GetSelector(selector), false, false));

		/// <summary>
		/// order by asc
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> OrderByNullsLast(Expression<Func<TModel, dynamic>> selector)
			=> OrderByNullsLast<TModel>(selector);

		/// <summary>
		/// order by desc
		/// </summary>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> OrderByDescendingNullsLast(Expression<Func<TModel, dynamic>> selector)
			=> OrderByDescendingNullsLast<TModel>(selector);

		/// <summary>
		/// order by asc
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> OrderByNullsLast<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new()
			=> OrderBy(DbConverter.ExplainOrderBy(Translator.GetSelector(selector), true, true));

		/// <summary>
		/// order by desc
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <param name="selector">key selector</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> OrderByDescendingNullsLast<TSource>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new()
			=> OrderBy(DbConverter.ExplainOrderBy(Translator.GetSelector(selector), false, true));

		#endregion

		#region ToList
		/// <summary>
		/// 返回列表
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public List<TResult> ToList<TResult>(string fields = null)
		{
			if (IsReturnDefault) return new List<TResult>();
			SetFields(fields);
			return base.ToList<TResult>();
		}

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <returns></returns>
		public List<TModel> ToList() => ToList<TModel>();

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
		public List<TKey> ToList<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperModel, new()
			=> SetFields(Translator.GetSelectorSpecial(selector)).ToList<TKey>();

		/// <summary>
		/// 返回列表, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public List<TResult> ToList<TSource, TResult>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new()
			=> SetFields(Translator.GetSelectorSpecial(selector)).ToList<TResult>();

		/// <summary>
		/// 返回列表, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public List<TResult> ToList<TResult>(Expression<Func<TModel, dynamic>> selector)
			=> SetFields(Translator.GetSelectorSpecial(selector)).ToList<TModel, TResult>(selector);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="fields"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<List<TResult>> ToListAsync<TResult>(string fields = null, CancellationToken cancellationToken = default)
			=> IsReturnDefault ? Task.FromResult(new List<TResult>()) : SetFields(fields).ToListAsync<TResult>(cancellationToken);

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
		public Task<List<TKey>> ToListAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new()
			=> SetFields(Translator.GetSelectorSpecial(selector)).ToListAsync<TKey>(cancellationToken);

		/// <summary>
		/// 返回列表, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<List<TResult>> ToListAsync<TResult>(Expression<Func<TModel, dynamic>> selector, CancellationToken cancellationToken = default)
			=> ToListAsync<TModel, TResult>(selector, cancellationToken);

		/// <summary>
		/// 返回列表, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<List<TResult>> ToListAsync<TSource, TResult>(Expression<Func<TSource, dynamic>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new()
			=> SetFields(Translator.GetSelectorSpecial(selector)).ToListAsync<TResult>(cancellationToken);

		/// <summary>
		/// 返回列表
		/// </summary>
		/// <returns></returns>
		public Task<List<TModel>> ToListAsync(CancellationToken cancellationToken = default)
			=> ToListAsync<TModel>(cancellationToken);
		#endregion

		#region FirstOrDefault
		public TModel First() => FirstOrDefault() ?? throw new CreeperFirstNotFoundException();

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<TModel> FirstAsync(CancellationToken cancellationToken = default) => FirstAsync<TModel>(cancellationToken);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public TResult FirstOrDefault<TResult>(string fields = null)
		{
			if (IsReturnDefault) return default;
			SetFieldsTake(fields);
			return base.FirstOrDefault<TResult>();
		}

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <returns></returns>
		public TModel FirstOrDefault()
			=> FirstOrDefault<TModel>();

		/// <summary>
		/// 返回一行, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TResult FirstOrDefault<TResult>(Expression<Func<TModel, dynamic>> selector)
		 => SetFields(Translator.GetSelectorSpecial(selector)).FirstOrDefault<TModel, TResult>(selector);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TKey FirstOrDefault<TKey>(Expression<Func<TModel, TKey>> selector)
			=> FirstOrDefault<TModel, TKey>(selector);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TKey FirstOrDefault<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperModel, new()
			=> SetFieldsTake(Translator.GetSelectorSpecial(selector)).ToScalar<TKey>();

		/// <summary>
		/// 返回一行, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public TResult FirstOrDefault<TSource, TResult>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new()
			=> SetFields(Translator.GetSelectorSpecial(selector)).FirstOrDefault<TResult>();

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="fields"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<TResult> FirstOrDefaultAsync<TResult>(string fields = null, CancellationToken cancellationToken = default)
			=> SetFieldsTake(fields).FirstOrDefaultAsync<TResult>(cancellationToken);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<TModel> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
			=> FirstOrDefaultAsync<TModel>(cancellationToken);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> FirstOrDefaultAsync<TKey>(Expression<Func<TModel, TKey>> selector, CancellationToken cancellationToken = default)
			=> FirstOrDefaultAsync<TModel, TKey>(selector, cancellationToken);

		/// <summary>
		/// 返回一行
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> FirstOrDefaultAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new()
			=> SetFieldsTake(Translator.GetSelectorSpecial(selector)).ToScalarAsync<TKey>(cancellationToken);

		/// <summary>
		/// 返回一行, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TModel, dynamic>> selector, CancellationToken cancellationToken = default)
			=> FirstOrDefaultAsync<TModel, TResult>(selector, cancellationToken);

		/// <summary>
		/// 返回一行, 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<TResult> FirstOrDefaultAsync<TSource, TResult>(Expression<Func<TSource, dynamic>> selector, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new()
			=> SetFieldsTake(Translator.GetSelectorSpecial(selector)).FirstOrDefaultAsync<TResult>(cancellationToken);
		#endregion

		#region ToScalar
		public object ToScalar(string field = null) { SetFieldsTake(field); return base.ToScalar(); }

		public ValueTask<object> ToScalarAsync(string field = null, CancellationToken cancellationToken = default) => SetFieldsTake(field).ToScalarAsync(cancellationToken);

		public TKey ToScalar<TKey>(string field = null) { SetFieldsTake(field); return base.ToScalar<TKey>(); }

		public ValueTask<TKey> ToScalarAsync<TKey>(string field = null, CancellationToken cancellationToken = default) => SetFieldsTake(field).ToScalarAsync<TKey>(cancellationToken);
		#endregion

		#region Aggregate
		/// <summary>
		/// 返回行数
		/// </summary>
		/// <returns></returns>
		public long Count() => SetFields("COUNT(1)").ToScalar<long>();

		/// <summary>
		/// 返回行数
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<long> CountAsync(CancellationToken cancellationToken = default)
			=> SetFields("COUNT(1)").ToScalarAsync<long>(cancellationToken);

		public long CountDistinct(Expression<Func<TModel, dynamic>> selector)
			=> CountDistinct<TModel>(selector);

		public ValueTask<long> CountDistinctAsync(Expression<Func<TModel, dynamic>> selector, CancellationToken cancellationToken = default)
			=> CountDistinctAsync<TModel>(selector, cancellationToken);

		public long CountDistinct<TSource>(Expression<Func<TSource, dynamic>> selector)
			=> SetFields($"COUNT(DISTINCT {Translator.GetExpression(selector).CmdText})").ToScalar<long>();

		public ValueTask<long> CountDistinctAsync<TSource>(Expression<Func<TModel, dynamic>> selector, CancellationToken cancellationToken = default)
			=> SetFields($"COUNT(DISTINCT {Translator.GetExpression(selector).CmdText})").ToScalarAsync<long>(cancellationToken);

		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Max<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new()
			=> ScalarTransfer(selector, "MAX", defaultValue);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Min<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new()
			=> ScalarTransfer(selector, "MIN", defaultValue);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Sum<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new()
			=> ScalarTransfer(selector, "SUM", defaultValue);

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Avg<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new()
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
		public TKey Max<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new() where TKey : struct
			=> ScalarTransfer(selector, "MAX", defaultValue);

		/// <summary>
		/// 取最小值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Min<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new() where TKey : struct
			=> ScalarTransfer(selector, "MIN", defaultValue);

		/// <summary>
		/// 取总和
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Sum<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new() where TKey : struct
			=> ScalarTransfer(selector, "SUM", defaultValue);

		/// <summary>
		/// 取平均值
		/// </summary>
		/// <typeparam name="TSource">model type</typeparam>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public TKey Avg<TSource, TKey>(Expression<Func<TSource, TKey?>> selector, TKey defaultValue = default) where TSource : ICreeperModel, new() where TKey : struct
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


		/// <summary>
		/// 取最大值
		/// </summary>
		/// <typeparam name="TSource">model类型</typeparam>
		/// <typeparam name="TKey">返回值类型</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> MaxAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new()
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
		public ValueTask<TKey> MinAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new()
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
		public ValueTask<TKey> SumAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new()
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
		public ValueTask<TKey> AvgAsync<TSource, TKey>(Expression<Func<TSource, TKey>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TSource : ICreeperModel, new()
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
			where TSource : ICreeperModel, new() where TKey : struct
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
			where TSource : ICreeperModel, new() where TKey : struct
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
			where TSource : ICreeperModel, new() where TKey : struct
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
			where TSource : ICreeperModel, new() where TKey : struct
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
		/// 取平均值
		/// </summary>
		/// <typeparam name="TKey">return value type</typeparam>
		/// <param name="selector">key selector</param>
		/// <param name="defaultValue"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<TKey> AvgAsync<TKey>(Expression<Func<TModel, TKey?>> selector, TKey defaultValue = default, CancellationToken cancellationToken = default) where TKey : struct
			=> AvgAsync<TModel, TKey>(selector, defaultValue, cancellationToken);
		#endregion

		#region FirstOrDefault.Pipe

		/// <summary>
		/// 返回一行(管道)
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="fields">返回字段, 可选</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> FirstOrDefaultPipe<TResult>(string fields = null)
			=> SetFieldsTake(fields).Pipe<TResult>(PipeReturnType.First);

		/// <summary>
		/// 返回一行(管道)
		/// </summary>
		/// <returns></returns>
		public ISelectBuilder<TModel> FirstOrDefaultPipe()
			=> FirstOrDefaultPipe<TModel>();

		/// <summary>
		/// 返回列表(管道), 匿名类型作为查询字段 
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> FirstOrDefaultPipe<TResult>(Expression<Func<TModel, dynamic>> selector)
		 => SetFields(Translator.GetSelectorSpecial(selector)).FirstOrDefaultPipe<TModel, TResult>(selector);

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> FirstOrDefaultPipe<TKey>(Expression<Func<TModel, TKey>> selector)
			=> FirstOrDefaultPipe<TModel, TKey>(selector);

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> FirstOrDefaultPipe<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperModel, new()
			=> SetFieldsTake(Translator.GetSelectorSpecial(selector)).FirstOrDefaultPipe<TKey>();

		/// <summary>
		/// 返回列表(管道), 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> FirstOrDefaultPipe<TSource, TResult>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new()
			=> SetFields(Translator.GetSelectorSpecial(selector)).FirstOrDefaultPipe<TResult>();

		/// <summary>
		/// 返回联表实体(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> FirstOrDefaultUnionPipe<TResult1>() where TResult1 : ICreeperModel, new()
			=> FirstOrDefaultPipe<(TModel, TResult1)>();

		/// <summary>
		/// 返回联表实体(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> FirstOrDefaultUnionPipe<TResult1, TResult2>() where TResult1 : ICreeperModel, new() where TResult2 : ICreeperModel, new()
			=> FirstOrDefaultPipe<(TModel, TResult1, TResult2)>();

		/// <summary>
		/// 返回联表实体(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <typeparam name="TResult3"></typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> FirstOrDefaultUnionPipe<TResult1, TResult2, TResult3>() where TResult1 : ICreeperModel, new() where TResult2 : ICreeperModel, new() where TResult3 : ICreeperModel, new()
			=> FirstOrDefaultPipe<(TModel, TResult1, TResult2, TResult3)>();
		#endregion

		#region ToList.Pipe
		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TResult">model type</typeparam>
		/// <param name="fields">指定输出字段</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> ToListPipe<TResult>(string fields = null)
			=> SetFields(fields).Pipe<TResult>(PipeReturnType.List);

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <returns></returns>
		public ISelectBuilder<TModel> ToListPipe()
			=> ToListPipe<TModel>();

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> ToListPipe<TKey>(Expression<Func<TModel, TKey>> selector)
			=> ToListPipe<TModel, TKey>(selector);

		/// <summary>
		/// 返回列表(管道)
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> ToListPipe<TSource, TKey>(Expression<Func<TSource, TKey>> selector) where TSource : ICreeperModel, new()
			=> SetFields(Translator.GetSelectorSpecial(selector)).ToListPipe<TKey>();

		/// <summary>
		/// 返回列表(管道), 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> ToListPipe<TSource, TResult>(Expression<Func<TSource, dynamic>> selector) where TSource : ICreeperModel, new()
			=> SetFields(Translator.GetSelectorSpecial(selector)).ToListPipe<TResult>();

		/// <summary>
		/// 返回列表(管道), 匿名类型作为查询字段
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> ToListPipe<TResult>(Expression<Func<TModel, dynamic>> selector)
		 => SetFields(Translator.GetSelectorSpecial(selector)).ToListPipe<TModel, TResult>(selector);

		/// <summary>
		/// 返回联表实体列表(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> ToListUnionPipe<TResult1>() where TResult1 : ICreeperModel, new()
			=> ToListPipe<(TModel, TResult1)>();

		/// <summary>
		/// 返回联表实体列表(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> ToListUnionPipe<TResult1, TResult2>() where TResult1 : ICreeperModel, new() where TResult2 : ICreeperModel, new()
			=> ToListPipe<(TModel, TResult1, TResult2)>();

		/// <summary>
		/// 返回联表实体列表(管道)
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <typeparam name="TResult3"></typeparam>
		/// <returns></returns>
		public ISelectBuilder<TModel> ToListUnionPipe<TResult1, TResult2, TResult3>() where TResult1 : ICreeperModel, new() where TResult2 : ICreeperModel, new() where TResult3 : ICreeperModel, new()
			=> ToListPipe<(TModel, TResult1, TResult2, TResult3)>();

		#endregion

		#region Join
		/// <summary>
		/// inner join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> InnerJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> InnerJoin<TModel, TTarget>(predicate);

		/// <summary>
		/// left join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> LeftJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> LeftJoin<TModel, TTarget>(predicate);

		/// <summary>
		/// right join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> RightJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> RightJoin<TModel, TTarget>(predicate);

		/// <summary>
		/// left outer join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> LeftOuterJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> LeftOuterJoin<TModel, TTarget>(predicate);

		/// <summary>
		/// right outer join
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> RightOuterJoin<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> RightOuterJoin<TModel, TTarget>(predicate);

		/// <summary>
		/// inner join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> InnerJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.INNER_JOIN, predicate, false);

		/// <summary>
		/// left join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> LeftJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.LEFT_JOIN, predicate, false);

		/// <summary>
		/// right join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> RightJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.RIGHT_JOIN, predicate, false);

		/// <summary>
		/// left outer join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> LeftOuterJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.LEFT_OUTER_JOIN, predicate, false);

		/// <summary>
		/// right outer join
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> RightOuterJoin<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.RIGHT_OUTER_JOIN, predicate, false);

		/// <summary>
		/// inner join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> InnerJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> InnerJoinUnion<TModel, TTarget>(predicate);

		/// <summary>
		/// left join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> LeftJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> LeftJoinUnion<TModel, TTarget>(predicate);

		/// <summary>
		/// right join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> RightJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> RightJoinUnion<TModel, TTarget>(predicate);

		/// <summary>
		/// left outer join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> LeftOuterJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> LeftOuterJoinUnion<TModel, TTarget>(predicate);

		/// <summary>
		/// right outer join, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> RightOuterJoinUnion<TTarget>(Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> RightOuterJoinUnion<TModel, TTarget>(predicate);

		/// <summary>
		/// inner join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> InnerJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.INNER_JOIN, predicate, true);

		/// <summary>
		/// left join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> LeftJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.LEFT_JOIN, predicate, true);

		/// <summary>
		/// right join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> RightJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.RIGHT_JOIN, predicate, true);

		/// <summary>
		/// left outer join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> LeftOuterJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.LEFT_OUTER_JOIN, predicate, true);

		/// <summary>
		/// right outer join, 关联返回
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="predicate"></param>
		/// <returns></returns>
		public ISelectBuilder<TModel> RightOuterJoinUnion<TSource, TTarget>(Expression<Func<TSource, TTarget, bool>> predicate) where TSource : ICreeperModel, new() where TTarget : ICreeperModel, new()
			=> Join(JoinType.RIGHT_OUTER_JOIN, predicate, true);

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <param name="alias">table alias name</param>
		/// <param name="on">on expression</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Join<TTarget>(JoinType joinType, string alias, string on) where TTarget : ICreeperModel, new()
		 => Join<TTarget>(joinType, alias, on, false);

		/// <summary>
		/// join base method, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <param name="alias">table alias name</param>
		/// <param name="on">on expression</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> JoinUnion<TTarget>(JoinType joinType, string alias, string on) where TTarget : ICreeperModel, new()
			=> Join<TTarget>(joinType, alias, on, true);

		/// <summary>
		/// join base method
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Join<TTarget>(JoinType joinType, Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> Join<TModel, TTarget>(joinType, predicate);

		/// <summary>
		/// join base method, 关联返回
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> JoinUnion<TTarget>(JoinType joinType, Expression<Func<TModel, TTarget, bool>> predicate) where TTarget : ICreeperModel, new()
			=> JoinUnion<TModel, TTarget>(joinType, predicate);

		/// <summary>
		/// join base method
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <typeparam name="TScource">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> Join<TScource, TTarget>(JoinType joinType, Expression<Func<TScource, TTarget, bool>> predicate)
			where TScource : ICreeperModel, new()
			where TTarget : ICreeperModel, new()
			=> Join(joinType, predicate, false);

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <typeparam name="TScource">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <returns></returns>
		public ISelectBuilder<TModel> JoinUnion<TScource, TTarget>(JoinType joinType, Expression<Func<TScource, TTarget, bool>> predicate)
			where TScource : ICreeperModel, new()
			where TTarget : ICreeperModel, new()
			=> Join(joinType, predicate, true);

		/// <summary>
		/// join base method with string
		/// </summary>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <param name="alias">table alias name</param>
		/// <param name="on">on expression</param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		private ISelectBuilder<TModel> Join<TTarget>(JoinType joinType, string alias, string on, bool isReturn) where TTarget : ICreeperModel, new()
		{
			var info = new JoinInfo(alias, EntityUtils.GetDbTable<TTarget>().TableName, on, joinType.ToString().Replace("_", " "), isReturn);
			if (info.IsReturn)
				info.Fields = DbConverter.EntityUtils.GetFieldsAlias<TTarget>(alias);
			_unions.Add(info);

			return this;
		}

		/// <summary>
		/// join base method
		/// </summary>
		/// <typeparam name="TSource">table model type</typeparam>
		/// <typeparam name="TTarget">table model type</typeparam>
		/// <param name="joinType">join type</param>
		/// <param name="predicate"></param>
		/// <param name="isReturn">is add return fields</param>
		/// <returns></returns>
		private ISelectBuilder<TModel> Join<TSource, TTarget>(JoinType joinType, Expression<Func<TSource, TTarget, bool>> predicate, bool isReturn) where TTarget : ICreeperModel, new() where TSource : ICreeperModel, new()
		{
			var expression = Translator.GetExpression(predicate);
			var unionAlias = MainAlias;
			foreach (var p in expression.Alias)
			{
				if (p != MainAlias && !_unions.Any(a => a.AliasName == p))
				{
					unionAlias = p;
					break;
				}
			}
			return Join<TTarget>(joinType, unionAlias, expression.CmdText, isReturn).AddParameters(expression.Parameters);
		}
		#endregion

		#region FirstOrDefault.Union
		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <returns></returns>
		public (TModel, TResult1) FirstOrDefaultUnion<TResult1>() where TResult1 : ICreeperModel, new()
			=> FirstOrDefault<(TModel, TResult1)>();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <returns></returns>
		public (TModel, TResult1, TResult2) FirstOrDefaultUnion<TResult1, TResult2>() where TResult1 : ICreeperModel, new() where TResult2 : ICreeperModel, new()
			=> FirstOrDefault<(TModel, TResult1, TResult2)>();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <typeparam name="TResult3"></typeparam>
		/// <returns></returns>
		public (TModel, TResult1, TResult2, TResult3) FirstOrDefaultUnion<TResult1, TResult2, TResult3>() where TResult1 : ICreeperModel, new() where TResult2 : ICreeperModel, new() where TResult3 : ICreeperModel, new()
			=> FirstOrDefault<(TModel, TResult1, TResult2, TResult3)>();

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		public Task<(TModel, T1)> UnionFirstOrDefaultAsync<T1>(CancellationToken cancellationToken = default) where T1 : ICreeperModel, new()
			=> FirstOrDefaultAsync<(TModel, T1)>(cancellationToken);

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		public Task<(TModel, T1, T2)> UnionFirstOrDefaultAsync<T1, T2>(CancellationToken cancellationToken = default) where T1 : ICreeperModel, new() where T2 : ICreeperModel, new()
			=> FirstOrDefaultAsync<(TModel, T1, T2)>(cancellationToken);

		/// <summary>
		/// 返回联表实体
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		public Task<(TModel, T1, T2, T3)> UnionFirstOrDefaultAsync<T1, T2, T3>(CancellationToken cancellationToken = default) where T1 : ICreeperModel, new() where T2 : ICreeperModel, new() where T3 : ICreeperModel, new()
			=> FirstOrDefaultAsync<(TModel, T1, T2, T3)>(cancellationToken);
		#endregion

		#region ToList.Union
		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <returns></returns>
		public List<(TModel, TResult1)> ToListUnion<TResult1>() where TResult1 : ICreeperModel, new()
			=> ToList<(TModel, TResult1)>();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <returns></returns>
		public List<(TModel, TResult1, TResult2)> ToListUnion<TResult1, TResult2>() where TResult1 : ICreeperModel, new() where TResult2 : ICreeperModel, new()
			=> ToList<(TModel, TResult1, TResult2)>();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="TResult1"></typeparam>
		/// <typeparam name="TResult2"></typeparam>
		/// <typeparam name="TResult3"></typeparam>
		/// <returns></returns>
		public List<(TModel, TResult1, TResult2, TResult3)> ToListUnion<TResult1, TResult2, TResult3>() where TResult1 : ICreeperModel, new() where TResult2 : ICreeperModel, new() where TResult3 : ICreeperModel, new()
			=> ToList<(TModel, TResult1, TResult2, TResult3)>();

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <returns></returns>
		public Task<List<(TModel, T1)>> UnionToListAsync<T1>(CancellationToken cancellationToken = default) where T1 : ICreeperModel, new()
			=> ToListAsync<(TModel, T1)>(cancellationToken);

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <returns></returns>
		public Task<List<(TModel, T1, T2)>> UnionToListAsync<T1, T2>(CancellationToken cancellationToken = default) where T1 : ICreeperModel, new() where T2 : ICreeperModel, new()
			=> ToListAsync<(TModel, T1, T2)>(cancellationToken);

		/// <summary>
		/// 返回联表实体列表
		/// </summary>
		/// <typeparam name="T1"></typeparam>
		/// <typeparam name="T2"></typeparam>
		/// <typeparam name="T3"></typeparam>
		/// <returns></returns>
		public Task<List<(TModel, T1, T2, T3)>> UnionToListAsync<T1, T2, T3>(CancellationToken cancellationToken = default) where T1 : ICreeperModel, new() where T2 : ICreeperModel, new() where T3 : ICreeperModel, new()
			=> ToListAsync<(TModel, T1, T2, T3)>(cancellationToken);
		#endregion

		#region Override
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetCommandText()
		{
			if (_limit < 0 || _offset < 0)
				throw new ArgumentNullException("Take/Skip/Page不能小于0");

			var fields = !string.IsNullOrEmpty(Fields) ? Fields : DbConverter.EntityUtils.GetFieldsAlias<TModel>(MainAlias);
			var join = new StringBuilder();

			foreach (var union in _unions)
			{
				join.AppendLine(union.ToString());
				if (union.IsReturn) fields = string.Concat(fields, ", ", union.Fields);
			}
			return DbConverter.GetSelectSql(fields, MainTable, MainAlias, WhereList, _groupBy, _having, _orderBy, _limit, _offset, _union, _except, _intersect, join.ToString(), _tablesampleSystem);
		}
		#endregion

		#region Private
		private TKey ScalarTransfer<TKey>(Expression selector, string method, TKey defaultValue)
			=> ScalarTransferAsync(false, selector, method, defaultValue, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		private ValueTask<TKey> ScalarTransferAsync<TKey>(Expression selector, string method, TKey defaultValue, CancellationToken cancellationToken)
			=> ScalarTransferAsync(true, selector, method, defaultValue, cancellationToken);

		private async ValueTask<TKey> ScalarTransferAsync<TKey>(bool async, Expression selector, string method, TKey defaultValue, CancellationToken cancellationToken)
		{
			if (IsReturnDefault) return default;
			var visit = Translator.GetExpression(selector);
			AddParameters(visit.Parameters);
			AddParameter(out string index, defaultValue);

			var field = DbConverter.CallCoalesce($"{method}({visit.CmdText})", DbConverter.GetSqlDbParameterName(index), Params);
			if (DbConverter.MaximumPrecision != 0)
				field = string.Concat("ROUND(", field, ",", DbConverter.MaximumPrecision, ")");
			SetFields(field);
			return async ? await ToScalarAsync<TKey>(cancellationToken) : ToScalar<TKey>();
		}

		private SelectBuilder<TModel> SetFieldsTake(string fields) { SetFields(fields).Take(1); return this; }

		private SelectBuilder<TModel> SetFields(string fields)
		{
			if (!string.IsNullOrEmpty(fields)) Fields = fields;
			return this;
		}

		#endregion
	}
}
