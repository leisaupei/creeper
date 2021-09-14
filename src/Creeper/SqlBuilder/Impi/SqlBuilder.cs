using Creeper.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.Extensions;

namespace Creeper.SqlBuilder.Impi
{
	internal abstract class SqlBuilder<TBuilder, TModel> : ISqlBuilder<TBuilder>
		where TBuilder : class, ISqlBuilder
		where TModel : class, ICreeperModel, new()
	{
		#region Identity
		/// <summary>
		/// 是否使用缓存
		/// </summary>
		private DbCacheType _cacheType = DbCacheType.None;

		/// <summary>
		/// 缓存过期时间
		/// </summary>
		private int? _dbCacheExpireTime;
		private DataBaseType _dataBaseType = DataBaseType.Default;
		private readonly ICreeperContext _context;
		private readonly ICreeperExecute _execute;

		/// <summary>
		/// 类型转换
		/// </summary>
		private TBuilder This => this as TBuilder;

		/// <summary>
		/// 主表
		/// </summary>
		protected string MainTable { get; }

		/// <summary>
		/// 
		/// </summary>
		protected CreeperConverter DbConverter => DbExecute.ConnectionOptions.Converter;

		/// <summary>
		/// 主表别名, 默认为: "a"
		/// </summary>
		protected string MainAlias { get; set; } = "a";

		/// <summary>
		/// 是否返回默认值, 默认: false
		/// </summary>
		public bool IsReturnDefault { get; set; } = false;

		/// <summary>
		/// 返回实例类型
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// 参数列表
		/// </summary>
		public List<DbParameter> Params { get; } = new List<DbParameter>();

		/// <summary>
		/// 返回类型
		/// </summary>
		public PipeReturnType ReturnType { get; set; }

		/// <summary>
		/// 查询字段
		/// </summary>
		public string Fields { get; set; }
		#endregion

		#region Constructor
		protected SqlBuilder(ICreeperContext context) : this() => _context = context;

		protected SqlBuilder(ICreeperExecute execute) : this() => _execute = execute;

		private SqlBuilder()
		{
			var table = EntityUtils.GetDbTable<TModel>();
			if (string.IsNullOrEmpty(MainTable)) MainTable = table.TableName;
		}
		#endregion

		/// <summary>
		/// 选择主库还是从库, Default预设策略
		/// </summary>
		/// <returns></returns>
		public TBuilder By(DataBaseType dataBaseType) { _dataBaseType = dataBaseType; return This; }

		/// <summary>
		/// 使用数据库缓存, 仅支持FirstOrDefault,ToScalar方法
		/// </summary>
		/// <returns></returns>
		public TBuilder ByCache(TimeSpan? expireTime)
		{
			_ = _context.Cache ?? throw new CreeperDbCacheNotFoundException();
			_cacheType = DbCacheType.Default;
			_dbCacheExpireTime = expireTime?.Seconds;
			return This;
		}

		/// <summary>
		/// 使用数据库缓存, 仅支持FirstOrDefault,ToScalar方法
		/// </summary>
		/// <returns></returns>
		public TBuilder ByCache(int? expireTime = null)
		{
			_ = _context.Cache ?? throw new CreeperDbCacheNotFoundException();
			_cacheType = DbCacheType.Default;
			_dbCacheExpireTime = expireTime;
			return This;
		}

		/// <summary>
		/// 使用主键缓存
		/// </summary>
		/// <returns></returns>
		[Obsolete]
		public TBuilder ByPkCache(TimeSpan? expireTime = null)
		{
			_ = _context.Cache ?? throw new CreeperDbCacheNotFoundException();
			_cacheType = DbCacheType.PkCache;
			_dbCacheExpireTime = expireTime?.Seconds;
			return This;
		}

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public TBuilder AddParameter(string parameterName, object value)
			=> AddParameter(DbConverter.GetDbParameter(parameterName, value));

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="value"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public TBuilder AddParameter(out string parameterName, object value)
			=> AddParameter(parameterName = ParameterUtils.Index, value);

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="ps"></param>
		/// <returns></returns>
		public TBuilder AddParameter(DbParameter ps) { Params.Add(ps); return This; }

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="ps"></param>
		/// <returns></returns>
		public TBuilder AddParameters(IEnumerable<DbParameter> ps) { Params.AddRange(ps); return This; }

		/// <summary>
		/// 设置返回类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected TBuilder SetReturnType(PipeReturnType type) { ReturnType = type; return This; }

		/// <summary>
		/// 名称为create_time/createtime(忽略大小写)时, 如果没有赋值则赋值DateTime.Now。兼容long类型13位时间戳
		/// </summary>
		/// <param name="column"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected object SetDefaultCreateDateTime(string column, object value)
		{
			if (column.ToLower() == DbConverter.WithQuote("create_time") ||
				column.ToLower() == DbConverter.WithQuote("createtime"))
				value = CommonUtils.SetDefaultDateTime(value);

			return value;
		}

		#region ToScalar

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected object ToScalar() => GetCacheResult(() => DbExecute.ExecuteScalar(CommandText, Params.ToArray(), CommandType.Text));

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected ValueTask<object> ToScalarAsync(CancellationToken cancellationToken)
			=> GetCacheResultAsync(() => DbExecute.ExecuteScalarAsync(CommandText, Params.ToArray(), CommandType.Text, cancellationToken));

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected TKey ToScalar<TKey>() => GetCacheResult(() => DbExecute.ExecuteScalar<TKey>(CommandText, Params.ToArray(), CommandType.Text));

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected ValueTask<TKey> ToScalarAsync<TKey>(CancellationToken cancellationToken)
			=> GetCacheResultAsync(() => DbExecute.ExecuteScalarAsync<TKey>(CommandText, Params.ToArray(), CommandType.Text, cancellationToken));
		#endregion

		#region ToList
		/// <summary>
		/// 返回list 
		/// </summary>
		/// <typeparam name="TResult">model type</typeparam>
		/// <returns></returns>
		protected List<TResult> ToList<TResult>()
			=> DbExecute.ExecuteReaderList<TResult>(CommandText, Params.ToArray(), CommandType.Text);

		/// <summary>
		/// 返回list 
		/// </summary>
		/// <typeparam name="TResult">model type</typeparam>
		/// <returns></returns>
		protected Task<List<TResult>> ToListAsync<TResult>(CancellationToken cancellationToken)
			=> DbExecute.ExecuteReaderListAsync<TResult>(CommandText, Params.ToArray(), CommandType.Text, cancellationToken);
		#endregion

		#region FirstOrDefault
		/// <summary>
		/// 返回第一行
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		protected TResult FirstOrDefault<TResult>()
			=> GetCacheResult(() => DbExecute.ExecuteReaderFirst<TResult>(CommandText, Params.ToArray(), CommandType.Text));

		/// <summary>
		/// 返回第一行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		protected Task<TResult> FirstOrDefaultAsync<TResult>(CancellationToken cancellationToken)
			=> GetCacheResultAsync(() => DbExecute.ExecuteReaderFirstAsync<TResult>(CommandText, Params.ToArray(), CommandType.Text, cancellationToken));

		/// <summary>
		/// 返回第一行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TResult"></typeparam>
		/// <exception cref="CreeperFirstNotFoundException">没有查询到数据时抛出此异常</exception>
		/// <returns></returns>
		protected async Task<TResult> FirstAsync<TResult>(CancellationToken cancellationToken)
		{
			var result = await GetCacheResultAsync(() => DbExecute.ExecuteReaderFirstAsync<TResult>(CommandText, Params.ToArray(), CommandType.Text, cancellationToken));
			if (result == null) throw new CreeperFirstNotFoundException();
			return result;
		}
		#endregion

		#region AffrowsResult.ToList
		/// <summary>
		/// 返回list 
		/// </summary>
		/// <typeparam name="TResult">model type</typeparam>
		/// <returns></returns>
		protected AffrowsResult<List<TResult>> ToListAffrowsResult<TResult>()
			=> DbExecute.ExecuteReaderListAffrows<TResult>(CommandText, Params.ToArray(), CommandType.Text);

		/// <summary>
		/// 返回list 
		/// </summary>
		/// <typeparam name="TResult">model type</typeparam>
		/// <returns></returns>
		protected Task<AffrowsResult<List<TResult>>> ToListAffrowsResultAsync<TResult>(CancellationToken cancellationToken)
			=> DbExecute.ExecuteReaderListAffrowsAsync<TResult>(CommandText, Params.ToArray(), CommandType.Text, cancellationToken);
		#endregion

		#region AffrowsResult.FirstOrDefault
		/// <summary>
		/// 返回第一行
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		protected AffrowsResult<TResult> ToAffrowsResult<TResult>()
			=> DbExecute.ExecuteReaderFirstAffrows<TResult>(CommandText, Params.ToArray(), CommandType.Text);

		/// <summary>
		/// 返回第一行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		protected Task<AffrowsResult<TResult>> ToAffrowsResultAsync<TResult>(CancellationToken cancellationToken)
			=> DbExecute.ExecuteReaderFirstAffrowsAsync<TResult>(CommandText, Params.ToArray(), CommandType.Text, cancellationToken);
		#endregion

		#region ToAffrows
		/// <summary>
		/// 返回行数
		/// </summary>
		/// <returns></returns>
		protected int ToAffrows()
		{
			SetReturnType(PipeReturnType.Affrows);
			return DbExecute.ExecuteNonQuery(CommandText, Params.ToArray(), CommandType.Text);
		}
		/// <summary>
		/// 返回行数
		/// </summary>
		/// <returns></returns>
		protected ValueTask<int> ToAffrowsAsync(CancellationToken cancellationToken)
		{
			SetReturnType(PipeReturnType.Affrows);
			return DbExecute.ExecuteNonQueryAsync(CommandText, Params.ToArray(), CommandType.Text, cancellationToken);
		}
		#endregion

		/// <summary>
		/// 输出管道元素
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		protected TBuilder Pipe<TResult>(PipeReturnType returnType)
		{
			Type = typeof(TResult);
			return SetReturnType(returnType);
		}

		/// <summary>
		/// 绑定select builder语句
		/// </summary>
		/// <param name="selectBuilderBinder">select语句的builder</param>
		/// <typeparam name="TTarget">binder的主表对象</typeparam>
		/// <returns></returns>
		protected ISelectBuilder<TTarget> BindSelectBuilder<TTarget>(Action<ISelectBuilder<TTarget>> selectBuilderBinder) where TTarget : class, ICreeperModel, new()
		{
			if (selectBuilderBinder is null)
			{
				throw new ArgumentNullException(nameof(selectBuilderBinder));
			}

			var selectBuilder = new SelectBuilder<TTarget>(DbExecute);
			selectBuilderBinder(selectBuilder);
			return selectBuilder;
		}

		#region Override
		/// <summary>
		/// Override ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(null);
		}
		/// <summary>
		/// 输出sql语句
		/// </summary>
		public string CommandText => GetCommandText();

		/// <summary>
		/// 调试或输出用
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public string ToString(string field)
		{
			if (!string.IsNullOrEmpty(field)) Fields = field;
			return DbConverter.ConvertSqlToString(this);
		}

		/// <summary>
		/// 设置sql语句
		/// </summary>
		/// <returns></returns>
		protected abstract string GetCommandText();
		#endregion

		#region Implicit
		//public static implicit operator string(SqlBuilder<TBuilder, TModel> builder) => builder.ToString();
		#endregion

		#region private
		private static readonly string _cachePrefix = "creeper_cache_";
		private TResult GetCacheResult<TResult>(Func<TResult> fn)
		{
			if (_cacheType == DbCacheType.None) return fn.Invoke();
			var key = string.Concat(_cachePrefix, ToString().GetMD5String());
			if (_context.Cache.Exists(key)) return (TResult)_context.Cache.Get(key, typeof(TResult));
			var ret = fn.Invoke();
			_context.Cache.Set(key, ret, _dbCacheExpireTime ?? _context.Cache.ExpireSeconds);
			return ret;
		}
		private async Task<TResult> GetCacheResultAsync<TResult>(Func<Task<TResult>> fn)
		{
			if (_cacheType == DbCacheType.None) return await fn.Invoke();
			var key = string.Concat(_cachePrefix, ToString().GetMD5String());
			if (await _context.Cache.ExistsAsync(key)) return (TResult)await _context.Cache.GetAsync(key, typeof(TResult));
			var ret = await fn.Invoke();
			await _context.Cache.SetAsync(key, ret, _dbCacheExpireTime ?? _context.Cache.ExpireSeconds);
			return ret;
		}
		private async ValueTask<TResult> GetCacheResultAsync<TResult>(Func<ValueTask<TResult>> fn) => await GetCacheResultAsync(() => fn.Invoke().AsTask());

		private ICreeperExecute DbExecute
			=> _execute ?? _context.Get(_dataBaseType) ?? throw new CreeperDbExecuteNotFoundException();
		#endregion

	}
}
