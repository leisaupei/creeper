using Creeper.SqlBuilder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Creeper.Driver;
using Creeper.Generic;

namespace Creeper.DbHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class CreeperDbContext : ICreeperDbContext
    {
		#region Private
        /// <summary>
        /// 静态数据库类型转换器
        /// </summary>
        private static readonly Dictionary<DataBaseKind, ICreeperDbTypeConverter> _dbTypeConverts = new();

        /// <summary>
        /// 实例键值对
        /// </summary>
        private static readonly Dictionary<string, List<ICreeperDbConnectionOption>> _executeOptions = new();

        /// <summary>
        /// 默认数据库名称
        /// </summary>
        private readonly string _defaultDbOptionName;

		#endregion

		/// <summary>
		/// 从库后缀
		/// </summary>
		public static readonly string SecondarySuffix = DataBaseType.Secondary.ToString();

        /// <summary>
        /// 数据库策略
        /// </summary>
        public static DataBaseTypeStrategy DbTypeStrategy { get; private set; }

        /// <summary>
        /// 数据库缓存
        /// </summary>
        public static ICreeperDbCache DbCache { get; private set; }

        /// <summary>
        /// 初始化一主多从数据库连接, 从库后缀默认: DataBaseType.Secondary.ToString()
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="ArgumentOutOfRangeException">options长度为0</exception>
        public CreeperDbContext(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetService<IOptions<CreeperOptions>>().Value;
            var dbCache = serviceProvider.GetService<ICreeperDbCache>();
            if (dbCache != null) DbCache = dbCache;

            DbTypeStrategy = options.DbTypeStrategy;

            _defaultDbOptionName = options.DefaultDbOptionName?.Name ?? "DbMain";

            //if (!CreeperOptions.DbOptions?.Any() ?? true)
            //	throw new ArgumentOutOfRangeException(nameof(CreeperOptions.DbOptions));

            foreach (var option in options.DbOptions)
            {
                if (option.Main == null)
                    throw new ArgumentNullException(nameof(option.Main), $"Connection string model is null");

                _executeOptions[option.Main.DbName] = new List<ICreeperDbConnectionOption> { option.Main };

                if (option.Secondary == null) continue;

                foreach (var item in option.Secondary)
                {
                    if (!_executeOptions.ContainsKey(item.DbName))
                        _executeOptions[item.DbName] = new List<ICreeperDbConnectionOption> { item };
                    else
                        _executeOptions[item.DbName].Add(item);
                }
            }

            foreach (var convert in options.CreeperDbTypeConverters)
            {
                if (!_dbTypeConverts.ContainsKey(convert.DataBaseKind))
                    _dbTypeConverts[convert.DataBaseKind] = convert;
            }
        }

        /// <summary>
        /// 通过数据库类型获取转换器
        /// </summary>
        /// <param name="dataBaseKind"></param>
        /// <returns></returns>
        public static ICreeperDbTypeConverter GetConvert(DataBaseKind dataBaseKind)
            => _dbTypeConverts.TryGetValue(dataBaseKind, out var convert)
            ? convert : throw new ArgumentException("没有添加响应的数据库类型转换器");

        #region GetExecute
        /// <summary>
        ///  获取连接实例
        /// </summary>
        /// <typeparam name="TDbName"></typeparam>
        /// <returns></returns>
        public ICreeperDbExecute GetExecute<TDbName>() where TDbName : struct, ICreeperDbName
            => new CreeperDbExecute(GetExecuteOption(typeof(TDbName)));

        /// <summary>
        ///  获取连接实例
        /// </summary>
		/// <param name="type"></param>
        /// <returns></returns>
        public ICreeperDbExecute GetExecute(Type type)
            => new CreeperDbExecute(GetExecuteOption(type.Name));

        /// <summary>
        /// 获取连接实例
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ICreeperDbExecute GetExecute(string name)
            => new CreeperDbExecute(GetExecuteOption(name));
        #endregion

        #region GetExecuteOption
        /// <summary>
        /// 获取连接配置
        /// </summary>
        /// <param name="name">数据库类型</param>
        /// <exception cref="ArgumentNullException">没有找到对应名称实例</exception>
        /// <returns>对应实例</returns>
        public static ICreeperDbConnectionOption GetExecuteOption(string name)
        {
            if (_executeOptions.ContainsKey(name))
            {
                var execute = _executeOptions[name];

                switch (execute.Count)
                {
                    case 0:
                        if (DbTypeStrategy == DataBaseTypeStrategy.SecondaryFirst && name.EndsWith(SecondarySuffix))
                            throw new ArgumentNullException("connectionstring", $"not exist {name} ICreeperDbConnectionOption");
                        break;
                    case 1:
                        return execute[0];
                    default:
                        return execute[Math.Abs(Guid.NewGuid().GetHashCode() % execute.Count)];
                }
            }

            if (DbTypeStrategy == DataBaseTypeStrategy.SecondaryFirstOfMainIfEmpty && name.EndsWith(SecondarySuffix))
                return GetExecuteOption(name.Replace(SecondarySuffix, string.Empty));

            // 从没有从库连接会查主库->如果没有连接会报错
            throw new ArgumentNullException("connectionstring", $"not exist {name} ICreeperDbConnectionOption");
        }

        /// <summary>
        /// 获取连接配置
        /// </summary>
        /// <exception cref="ArgumentNullException">没有找到对应名称实例</exception>
        /// <returns>对应实例</returns>
        public static ICreeperDbConnectionOption GetExecuteOption(Type type)
            => GetExecuteOption(type.Name);

        /// <summary>
        /// 获取连接配置
        /// </summary>
        /// <exception cref="ArgumentNullException">没有找到对应名称实例</exception>
        /// <returns>对应实例</returns>
        public static ICreeperDbConnectionOption GetExecuteOption<TDbName>() where TDbName : struct, ICreeperDbName
            => GetExecuteOption(typeof(TDbName));
        #endregion

        #region Transaction
        public ICreeperDbExecute BeginTransaction()
            => GetExecute(_defaultDbOptionName).BeginTransaction();

        public ValueTask<ICreeperDbExecute> BeginTransactionAsync(CancellationToken cancellationToken = default)
            => GetExecute(_defaultDbOptionName).BeginTransactionAsync(cancellationToken);

        public void Transaction(Action<ICreeperDbExecute> action)
            => GetExecute(_defaultDbOptionName).Transaction(action);

        public ValueTask TransactionAsync(Action<ICreeperDbExecute> action, CancellationToken cancellationToken = default)
            => GetExecute(_defaultDbOptionName).TransactionAsync(action, cancellationToken);

        /// <summary>
        /// 事务
        /// </summary>
        /// <typeparam name="TDbName"></typeparam>
        /// <param name="action"></param>
        public void Transaction<TDbName>(Action<ICreeperDbExecute> action) where TDbName : struct, ICreeperDbName
            => GetExecute<TDbName>().Transaction(action);

        /// <summary>
        /// 事务
        /// </summary>
        /// <typeparam name="TDbName"></typeparam>
        /// <param name="action"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask TransactionAsync<TDbName>(Action<ICreeperDbExecute> action, CancellationToken cancellationToken = default) where TDbName : struct, ICreeperDbName
            => GetExecute<TDbName>().TransactionAsync(action, cancellationToken);
        #endregion

        #region ExcuteDataReader
        public void ExecuteDataReader(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
            => GetExecute(_defaultDbOptionName).ExecuteDataReader(action, cmdText, cmdType, cmdParams);

        public ValueTask ExecuteDataReaderAsync(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
            => GetExecute(_defaultDbOptionName).ExecuteDataReaderAsync(action, cmdText, cmdType, cmdParams, cancellationToken);

        public List<T> ExecuteDataReaderList<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
            => GetExecute(_defaultDbOptionName).ExecuteDataReaderList<T>(cmdText, cmdType, cmdParams);

        public Task<List<T>> ExecuteDataReaderListAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
            => GetExecute(_defaultDbOptionName).ExecuteDataReaderListAsync<T>(cmdText, cmdType, cmdParams, cancellationToken);

        public T ExecuteDataReaderModel<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
            => GetExecute(_defaultDbOptionName).ExecuteDataReaderModel<T>(cmdText, cmdType, cmdParams);

        public Task<T> ExecuteDataReaderModelAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
            => GetExecute(_defaultDbOptionName).ExecuteDataReaderModelAsync<T>(cmdText, cmdType, cmdParams, cancellationToken);
        #endregion

        #region ExecuteDataReaderPipe
        public object[] ExecuteDataReaderPipe(IEnumerable<ISqlBuilder> builders, CommandType cmdType = CommandType.Text)
            => GetExecute(_defaultDbOptionName).ExecuteDataReaderPipe(builders, cmdType);

        public Task<object[]> ExecuteDataReaderPipeAsync(IEnumerable<ISqlBuilder> builders, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
            => GetExecute(_defaultDbOptionName).ExecuteDataReaderPipeAsync(builders, cmdType, cancellationToken);
        #endregion

        #region ExecuteNonQuery
        public int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
            => GetExecute(_defaultDbOptionName).ExecuteNonQuery(cmdText, cmdType, cmdParams);

        public ValueTask<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
            => GetExecute(_defaultDbOptionName).ExecuteNonQueryAsync(cmdText, cmdType, cmdParams, cancellationToken);
        #endregion

        #region ExecuteScalar
        public object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
            => GetExecute(_defaultDbOptionName).ExecuteScalar(cmdText, cmdType, cmdParams);

        public T ExecuteScalar<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
            => GetExecute(_defaultDbOptionName).ExecuteScalar<T>(cmdText, cmdType, cmdParams);

        public ValueTask<object> ExecuteScalarAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
            => GetExecute(_defaultDbOptionName).ExecuteScalarAsync(cmdText, cmdType, cmdParams, cancellationToken);

        public ValueTask<T> ExecuteScalarAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
            => GetExecute(_defaultDbOptionName).ExecuteScalarAsync<T>(cmdText, cmdType, cmdParams, cancellationToken);
        #endregion
    }
}
