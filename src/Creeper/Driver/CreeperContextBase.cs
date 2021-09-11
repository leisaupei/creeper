using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.SqlBuilder;

namespace Creeper.Driver
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class CreeperContextBase : ICreeperContext
	{
		private readonly DataBaseTypeStrategy _strategy;

		/// <summary>
		/// 数据库连接配置
		/// </summary>
		protected virtual Action<DbConnection> ConnectionOptions { get; }

		/// <summary>
		/// 数据库缓存
		/// </summary>
		public ICreeperCache Cache { get; }

		/// <summary>
		/// 数据库配置
		/// </summary>
		public ICreeperConnectionOption ConnectionOption { get; }

		protected CreeperContextBase(IServiceProvider serviceProvider)
		{
			var contextName = this.GetType().FullName;
			var creeperOptions = serviceProvider.GetService<IOptionsMonitor<CreeperContextOptions>>().Get(contextName);
			var converter = serviceProvider.GetService<CreeperConverterFactory>().Get(contextName);

			converter.Initialization(creeperOptions.Main);
			_strategy = creeperOptions.Strategy;

			if (creeperOptions.CacheType != null)
				Cache = (ICreeperCache)serviceProvider.GetService(creeperOptions.CacheType);

			var main = Build(creeperOptions.Main);
			var secondary = creeperOptions.Secondary?.Select(a => Build(a)).ToArray() ?? new CreeperConnection[0];
			ConnectionOption = new CreeperConnectionOption(main, secondary);

			CreeperConnection Build(string connectionString) => new CreeperConnection(connectionString, converter) { DbConnectionOptions = ConnectionOptions + creeperOptions.ConnectionOptions };
		}

		#region GetExecute
		/// <summary>
		///  获取连接实例
		/// </summary>
		/// <returns></returns>
		public ICreeperExecute Get(DataBaseType dataBaseType)
			=> new CreeperExecute(GetOption(dataBaseType));

		/// <summary>
		/// 获取连接配置
		/// </summary>
		/// <exception cref="CreeperDbConnectionOptionNotFoundException">没有找到对应名称实例</exception>
		/// <returns>对应实例</returns>
		private ICreeperConnection GetOption(DataBaseType dataBaseType) => dataBaseType switch
		{
			DataBaseType.Default => _strategy switch
			{
				DataBaseTypeStrategy.MainIfSecondaryEmpty => ConnectionOption.Secondary.Any() ? GetOption(DataBaseType.Secondary) : GetOption(DataBaseType.Main),
				DataBaseTypeStrategy.OnlyMain => GetOption(DataBaseType.Main),
				DataBaseTypeStrategy.OnlySecondary => GetOption(DataBaseType.Secondary),
				_ => null
			},
			DataBaseType.Main => ConnectionOption.Main,
			DataBaseType.Secondary => ConnectionOption.Secondary.Any() ? ConnectionOption.Secondary[ConnectionOption.Secondary.Length == 1 ? 0 : Math.Abs(Guid.NewGuid().GetHashCode() % ConnectionOption.Secondary.Length)] : null,
			_ => null,
		} ?? throw new CreeperDbConnectionOptionNotFoundException(dataBaseType, _strategy);
		#endregion

		#region Transaction
		public ICreeperExecute BeginTransaction()
			=> Get(DataBaseType.Main).BeginTransaction();

		public ValueTask<ICreeperExecute> BeginTransactionAsync(CancellationToken cancellationToken = default)
			=> Get(DataBaseType.Main).BeginTransactionAsync(cancellationToken);

		public void Transaction(Action<ICreeperExecute> action)
			=> Get(DataBaseType.Main).Transaction(action);

		public ValueTask TransactionAsync(Action<ICreeperExecute> action, CancellationToken cancellationToken = default)
			=> Get(DataBaseType.Main).TransactionAsync(action, cancellationToken);

		public ValueTask TransactionAsync(ActionAsync<ICreeperExecute> action, CancellationToken cancellationToken = default)
			=> Get(DataBaseType.Main).TransactionAsync(action, cancellationToken);
		#endregion

		#region ExcuteReader.Base
		public void ExecuteReader(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default)
			=> Get(dataBaseType).ExecuteReader(action, cmdText, cmdParams, cmdType);

		public ValueTask ExecuteReaderAsync(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> Get(dataBaseType).ExecuteReaderAsync(action, cmdText, cmdParams, cmdType, cancellationToken);
		#endregion

		#region ExcuteReader.List
		public List<T> ExecuteReaderList<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default)
			=> Get(dataBaseType).ExecuteReaderList<T>(cmdText, cmdParams, cmdType);

		public Task<List<T>> ExecuteReaderListAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> Get(dataBaseType).ExecuteReaderListAsync<T>(cmdText, cmdParams, cmdType, cancellationToken);
		#endregion

		#region ExcuteReader.First
		public T ExecuteReaderFirst<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default)
			=> Get(dataBaseType).ExecuteReaderFirst<T>(cmdText, cmdParams, cmdType);

		public Task<T> ExecuteReaderFirstAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> Get(dataBaseType).ExecuteReaderFirstAsync<T>(cmdText, cmdParams, cmdType, cancellationToken);
		#endregion

		#region ExcuteReader.First.Affrows
		public Task<AffrowsResult<T>> ExecuteReaderFirstAffrowsAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> Get(DataBaseType.Main).ExecuteReaderFirstAffrowsAsync<T>(cmdText, cmdParams, cmdType, cancellationToken);

		public AffrowsResult<T> ExecuteReaderFirstAffrows<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> Get(DataBaseType.Main).ExecuteReaderFirstAffrows<T>(cmdText, cmdParams, cmdType);
		#endregion

		#region ExcuteReader.List.Affrows
		public Task<AffrowsResult<List<T>>> ExecuteReaderListAffrowsAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> Get(DataBaseType.Main).ExecuteReaderListAffrowsAsync<T>(cmdText, cmdParams, cmdType, cancellationToken);

		public AffrowsResult<List<T>> ExecuteReaderListAffrows<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> Get(DataBaseType.Main).ExecuteReaderListAffrows<T>(cmdText, cmdParams, cmdType);
		#endregion

		#region ExcuteReader.Affrows
		public int ExecuteReaderAffrows(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> Get(DataBaseType.Main).ExecuteReaderAffrows(action, cmdText, cmdParams, cmdType);

		public ValueTask<int> ExecuteReaderAffrowsAsync(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> Get(DataBaseType.Main).ExecuteReaderAffrowsAsync(action, cmdText, cmdParams, cmdType, cancellationToken);
		#endregion

		#region ExcuteReader.Pipe
		public object[] ExecuteReaderPipe(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default)
			=> Get(dataBaseType).ExecutePipe(builders);

		public Task<object[]> ExecuteReaderPipeAsync(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> Get(dataBaseType).ExecutePipeAsync(builders, cancellationToken);

		public ValueTask ExecutePipeAsync(ActionAsync<ICreeperExecute> action, DataBaseType dataBaseType = DataBaseType.Default)
			=> Get(dataBaseType).ExecutePipeAsync(action);

		public void ExecutePipe(Action<ICreeperExecute> action, DataBaseType dataBaseType = DataBaseType.Default)
			=> Get(dataBaseType).ExecutePipe(action);
		#endregion

		#region ExecuteNonQuery
		public int ExecuteNonQuery(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> Get(DataBaseType.Main).ExecuteNonQuery(cmdText, cmdParams, cmdType);

		public ValueTask<int> ExecuteNonQueryAsync(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> Get(DataBaseType.Main).ExecuteNonQueryAsync(cmdText, cmdParams, cmdType, cancellationToken);
		#endregion

		#region ExecuteScalar
		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		/// <param name="cmdText"></param>
		/// <param name="cmdParams"></param>
		/// <param name="cmdType"></param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		public object ExecuteScalar(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default)
			=> Get(dataBaseType).ExecuteScalar(cmdText, cmdParams, cmdType);

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText"></param>
		/// <param name="cmdParams"></param>
		/// <param name="cmdType"></param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		public T ExecuteScalar<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default)
			=> Get(dataBaseType).ExecuteScalar<T>(cmdText, cmdParams, cmdType);

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		/// <param name="cmdText"></param>
		/// <param name="cmdParams"></param>
		/// <param name="cmdType"></param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<object> ExecuteScalarAsync(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> Get(dataBaseType).ExecuteScalarAsync(cmdText, cmdParams, cmdType, cancellationToken);

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText"></param>
		/// <param name="cmdParams"></param>
		/// <param name="cmdType"></param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask<T> ExecuteScalarAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> Get(dataBaseType).ExecuteScalarAsync<T>(cmdText, cmdParams, cmdType, cancellationToken);

		#endregion
	}
}
