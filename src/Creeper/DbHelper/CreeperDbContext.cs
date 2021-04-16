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
using System.Collections.ObjectModel;

namespace Creeper.DbHelper
{
	/// <summary>
	/// 
	/// </summary>
	public class CreeperDbContext : ICreeperDbContext
	{
		#region Private
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

			#region Init Options
			var executeOptions = new Dictionary<string, List<ICreeperDbConnectionOption>>();
			var dbTypeConverts = new Dictionary<DataBaseKind, ICreeperDbTypeConverter>();
			var dbTypeConvertsName = new Dictionary<string, ICreeperDbTypeConverter>();

			foreach (var convert in options.CreeperDbTypeConverters)
			{
				if (!dbTypeConverts.ContainsKey(convert.DataBaseKind))
					dbTypeConverts[convert.DataBaseKind] = convert;
			}

			foreach (var option in options.DbOptions)
			{
				if (option.Main == null)
					throw new ArgumentNullException(nameof(option.Main), $"Main Connectionstring is null");

				executeOptions[option.Main.DbName] = new List<ICreeperDbConnectionOption> { option.Main };

				dbTypeConvertsName[option.Main.DbName] = dbTypeConverts[option.Main.DataBaseKind];

				if (option.Secondary == null) continue;

				foreach (var item in option.Secondary)
				{
					if (!executeOptions.ContainsKey(item.DbName))
						executeOptions[item.DbName] = new List<ICreeperDbConnectionOption> { item };
					else
						executeOptions[item.DbName].Add(item);
				}
			}

			TypeHelper.DbTypeConvertsName = dbTypeConvertsName;
			TypeHelper.DbTypeConverts = dbTypeConverts;
			TypeHelper.ExecuteOptions = executeOptions;
			#endregion
		}

		#region GetExecuteOption
		/// <summary>
		/// 获取连接配置
		/// </summary>
		/// <param name="name">数据库类型</param>
		/// <exception cref="ArgumentNullException">没有找到对应名称实例</exception>
		/// <returns>对应实例</returns>
		public static ICreeperDbConnectionOption GetExecuteOption(string name)
		{
			if (TypeHelper.ExecuteOptions.ContainsKey(name))
			{
				var execute = TypeHelper.ExecuteOptions[name];

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
		public object[] ExecuteDataReaderPipe(IEnumerable<ISqlBuilder> builders)
			=> GetExecute(_defaultDbOptionName).ExecuteDataReaderPipe(builders);

		public Task<object[]> ExecuteDataReaderPipeAsync(IEnumerable<ISqlBuilder> builders, CancellationToken cancellationToken = default)
			=> GetExecute(_defaultDbOptionName).ExecuteDataReaderPipeAsync(builders, cancellationToken);
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
