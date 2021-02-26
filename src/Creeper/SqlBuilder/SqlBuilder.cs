using Creeper.DbHelper;
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

namespace Creeper.SqlBuilder
{
    public abstract class SqlBuilder<TBuilder, TModel> : ISqlBuilder
        where TBuilder : class, ISqlBuilder
        where TModel : class, ICreeperDbModel, new()
    {
        #region Identity
        protected ICreeperDbExecute DbExecute { get; private set; }
        private readonly ICreeperDbContext _dbContext;
        /// <summary>
        /// 类型转换
        /// </summary>
        private TBuilder This => this as TBuilder;

        /// <summary>
        /// 主表
        /// </summary>
        protected string MainTable { get; set; }

        /// <summary>
        /// 主表别名, 默认为: "a"
        /// </summary>
        protected string MainAlias { get; set; } = "a";

        /// <summary>
        /// where条件列表
        /// </summary>
        protected List<string> WhereList { get; } = new List<string>();

        /// <summary>
        /// 设置默认数据库
        /// </summary>
        protected string DbName { get; set; }

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

        /// <summary>
        /// 是否使用缓存
        /// </summary>
        internal DbCacheType UseCacheType { get; private set; } = DbCacheType.None;

        /// <summary>
        /// where条件数量
        /// </summary>
        public int WhereCount => WhereList.Count;
        #endregion

        #region Constructor
        protected SqlBuilder(ICreeperDbContext dbContext)
        {
            var table = EntityHelper.GetDbTable<TModel>();

            if (string.IsNullOrEmpty(MainTable))
                MainTable = table.TableName;

            DbName = table.DbName;

            if (CreeperDbContext.DbTypeStrategy != DataBaseTypeStrategy.OnlyMain)
                DbName += CreeperDbContext.SecondarySuffix;

            DbExecute = dbContext.GetExecute(DbName);
            _dbContext = dbContext;
        }
        protected SqlBuilder(ICreeperDbExecute dbExecute)
        {
            if (string.IsNullOrEmpty(MainTable))
                MainTable = EntityHelper.GetDbTable<TModel>().TableName;
            DbExecute = dbExecute;
        }
        #endregion

        /// <summary>
        /// 查询指定数据库
        /// </summary>
        /// <typeparam name="TDbName">数据库名称</typeparam>
        /// <returns></returns>
        public TBuilder By<TDbName>() where TDbName : struct, ICreeperDbName
        {
            DbExecute.Dispose();
            DbExecute = _dbContext.GetExecute<TDbName>();
            return This;
        }

        /// <summary>
        /// 选择主库还是从库
        /// </summary>
        /// <returns></returns>
        public TBuilder By(DataBaseType dataBaseType)
        {
            DbExecute.Dispose();
            DbExecute = _dbContext.GetExecute(dataBaseType.ChangeDataBaseKind(DbName));
            return This;
        }

        /// <summary>
        /// 使用数据库缓存, 仅支持ToOne,ToScalar方法
        /// </summary>
        /// <returns></returns>
        public TBuilder ByCache()
        {
            if (CreeperDbContext.DbCache == null)
                throw new ArgumentNullException("Not found the implemention type of ICreeperDbExecute");
            UseCacheType = DbCacheType.Default;
            return This;
        }

        /// <summary>
        /// 使用数据库主键缓存, 仅支持ToOne,ToScalar方法
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        private TBuilder ByPkCache()
        {
            if (CreeperDbContext.DbCache == null)
                throw new ArgumentNullException("Not found the implemention type of ICreeperDbExecute");
            UseCacheType = DbCacheType.PkCache;
            return This;
        }

        /// <summary>
        /// 穿透缓存
        /// </summary>
        /// <returns></returns>
        private TBuilder ThroughCache()
        {
            return This;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TBuilder AddParameter(string parameterName, object value)
            => AddParameter(DbExecute.ConnectionOptions.GetDbParameter(parameterName, value));

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public TBuilder AddParameter(out string parameterName, object value)
        {
            parameterName = EntityHelper.ParamsIndex;
            return AddParameter(parameterName, value);
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public TBuilder AddParameter(DbParameter ps)
        {
            Params.Add(ps);
            return This;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public TBuilder AddParameters(IEnumerable<DbParameter> ps)
        {
            Params.AddRange(ps);
            return This;
        }

        #region ToScalar

        /// <summary>
        /// 返回第一个元素
        /// </summary>
        /// <returns></returns>
        protected object ToScalar() => GetCacheResult(Scalar);

        /// <summary>
        /// 返回第一个元素
        /// </summary>
        /// <returns></returns>
        protected ValueTask<object> ToScalarAsync(CancellationToken cancellationToken)
            => new(GetCacheResultAsync(() => ScalarAsync(cancellationToken).AsTask()));

        /// <summary>
        /// 返回第一个元素
        /// </summary>
        /// <returns></returns>
        protected TKey ToScalar<TKey>() => GetCacheResult(Scalar<TKey>);

        /// <summary>
        /// 返回第一个元素
        /// </summary>
        /// <returns></returns>
        protected ValueTask<TKey> ToScalarAsync<TKey>(CancellationToken cancellationToken)
            => new(GetCacheResultAsync(() => ScalarAsync<TKey>(cancellationToken).AsTask()));

        /// <summary>
        /// 返回第一个元素
        /// </summary>
        /// <returns></returns>
        private object Scalar()
            => DbExecute.ExecuteScalar(CommandText, CommandType.Text, Params.ToArray());

        /// <summary>
        /// 返回第一个元素
        /// </summary>
        /// <returns></returns>
        private ValueTask<object> ScalarAsync(CancellationToken cancellationToken)
            => DbExecute.ExecuteScalarAsync(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

        /// <summary>
        /// 返回第一个元素
        /// </summary>
        /// <returns></returns>
        private TKey Scalar<TKey>()
            => DbExecute.ExecuteScalar<TKey>(CommandText, CommandType.Text, Params.ToArray());

        /// <summary>
        /// 返回第一个元素
        /// </summary>
        /// <returns></returns>
        private ValueTask<TKey> ScalarAsync<TKey>(CancellationToken cancellationToken)
            => DbExecute.ExecuteScalarAsync<TKey>(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

        #endregion

        #region ToList
        /// <summary>
        /// 返回list 
        /// </summary>
        /// <typeparam name="T">model type</typeparam>
        /// <returns></returns>
        protected List<T> ToList<T>()
            => DbExecute.ExecuteDataReaderList<T>(CommandText, CommandType.Text, Params.ToArray());

        /// <summary>
        /// 返回list 
        /// </summary>
        /// <typeparam name="T">model type</typeparam>
        /// <returns></returns>
        protected Task<List<T>> ToListAsync<T>(CancellationToken cancellationToken)
            => DbExecute.ExecuteDataReaderListAsync<T>(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

        #endregion

        #region ToOne
        /// <summary>
        /// 返回第一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T ToOne<T>()
                => GetCacheResult(One<T>);

        /// <summary>
        /// 返回第一行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T One<T>()
            => DbExecute.ExecuteDataReaderModel<T>(CommandText, CommandType.Text, Params.ToArray());

        /// <summary>
        /// 返回第一行
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected Task<T> ToOneAsync<T>(CancellationToken cancellationToken)
            => GetCacheResultAsync(() => OneAsync<T>(cancellationToken));


        /// <summary>
        /// 返回第一行
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private Task<T> OneAsync<T>(CancellationToken cancellationToken)
            => DbExecute.ExecuteDataReaderModelAsync<T>(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

        #endregion

        #region ToRows
        /// <summary>
        /// 返回行数
        /// </summary>
        /// <returns></returns>
        protected int ToRows()
            => DbExecute.ExecuteNonQuery(CommandText, CommandType.Text, Params.ToArray());

        /// <summary>
        /// 返回行数
        /// </summary>
        /// <returns></returns>
        protected ValueTask<int> ToRowsAsync(CancellationToken cancellationToken)
            => DbExecute.ExecuteNonQueryAsync(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);

        #endregion

        /// <summary>
        /// 输出管道元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected TBuilder ToPipe<T>(PipeReturnType returnType)
        {
            Type = typeof(T);
            ReturnType = returnType;
            return This;
        }

        /// <summary>
        /// Override ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(null);

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
            return CreeperDbContext.GetConvert(DbExecute.ConnectionOptions.DataBaseKind).ConvertSqlToString(this);
        }

        /// <summary>
        /// 设置sql语句
        /// </summary>
        /// <returns></returns>
        public abstract string GetCommandText();

        #region Implicit
        public static implicit operator string(SqlBuilder<TBuilder, TModel> builder) => builder.ToString();
        #endregion

        private static readonly string _cachePrefix = "creeper_cache_";
        private T GetCacheResult<T>(Func<T> fn)
        {
            if (UseCacheType == DbCacheType.None) return fn.Invoke();
            var key = string.Concat(_cachePrefix, ToString().GetMD5String());
            if (CreeperDbContext.DbCache.Exists(key))
            {
                var value = (T)CreeperDbContext.DbCache.Get(key, typeof(T));
                return value;
            }
            var ret = fn.Invoke();
            CreeperDbContext.DbCache.Set(key, ret);
            return ret;
        }
        private async Task<T> GetCacheResultAsync<T>(Func<Task<T>> fn)
        {
            if (UseCacheType == DbCacheType.None) return await fn.Invoke();
            var key = string.Concat(_cachePrefix, ToString().GetMD5String());
            if (await CreeperDbContext.DbCache.ExistsAsync(key))
                return (T)await CreeperDbContext.DbCache.GetAsync(key, typeof(T));
            var ret = await fn.Invoke();
            await CreeperDbContext.DbCache.SetAsync(key, ret);
            return ret;
        }
    }
}
