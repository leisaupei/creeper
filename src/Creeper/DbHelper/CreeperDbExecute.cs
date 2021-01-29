using Creeper;
using Creeper.SqlBuilder;
using Creeper.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using Creeper.Driver;
using Creeper.Generic;

namespace Creeper.DbHelper
{
	public class CreeperDbExecute : ICreeperDbExecute
	{
		public ICreeperDbConnectionOption ConnectionOptions { get; }

		/// <summary>
		/// 事务池
		/// </summary>
		protected DbTransaction _trans;

		/// <summary>
		/// constructer
		/// </summary>
		/// <param name="conn"></param>
		public CreeperDbExecute(ICreeperDbConnectionOption connectionOptions)
		{
			if (string.IsNullOrEmpty(connectionOptions.ConnectionString))
				throw new ArgumentNullException(nameof(connectionOptions.ConnectionString));
			ConnectionOptions = connectionOptions;
		}

		#region ExecuteScalar
		/// <summary>
		/// 返回一行数据
		/// </summary>
		public object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
			=> ExecuteScalarAsync(false, cmdText, cmdType, cmdParams, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 返回一行数据
		/// </summary>
		public ValueTask<object> ExecuteScalarAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? new ValueTask<object>(Task.FromCanceled<object>(cancellationToken))
			: ExecuteScalarAsync(true, cmdText, cmdType, cmdParams, cancellationToken);

		/// <summary>
		/// 返回一行数据
		/// </summary>
		public T ExecuteScalar<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
			=> ExecuteScalarAsync<T>(false, cmdText, cmdType, cmdParams, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 返回一行数据
		/// </summary>
		public ValueTask<T> ExecuteScalarAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? new ValueTask<T>(Task.FromCanceled<T>(cancellationToken))
			: ExecuteScalarAsync<T>(true, cmdText, cmdType, cmdParams, cancellationToken);

		private async ValueTask<object> ExecuteScalarAsync(bool async, string cmdText, CommandType cmdType, DbParameter[] cmdParams, CancellationToken cancellationToken)
		{
			DbCommand cmd = null;
			object ret = null;
			try
			{
				cmd = await PrepareCommandAsync(async, cmdText, cmdType, cmdParams, cancellationToken);
				ret = async ? await cmd.ExecuteScalarAsync(cancellationToken) : cmd.ExecuteScalar();
			}
			catch (Exception ex)
			{
				ThrowException(cmd, ex);
			}
			finally
			{
				await CloseCommandAsync(async, cmd);
			}
			return ret;
		}

		private async ValueTask<T> ExecuteScalarAsync<T>(bool async, string cmdText, CommandType cmdType, DbParameter[] cmdParams, CancellationToken cancellationToken)
		{
			var value = async
				? await ExecuteScalarAsync(cmdText, cmdType, cmdParams, cancellationToken)
				: ExecuteScalar(cmdText, cmdType, cmdParams);
			return value == null ? default : (T)Convert.ChangeType(value, typeof(T).GetOriginalType());
		}

		#endregion

		#region ExecuteNonQuery
		/// <summary>
		/// 执行sql语句
		/// </summary>
		public int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
			=> ExecuteNonQueryAsync(false, cmdText, cmdType, cmdParams, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 执行sql语句
		/// </summary>
		public ValueTask<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? new ValueTask<int>(Task.FromCanceled<int>(cancellationToken))
			: ExecuteNonQueryAsync(true, cmdText, cmdType, cmdParams, cancellationToken);

		private async ValueTask<int> ExecuteNonQueryAsync(bool async, string cmdText, CommandType cmdType, DbParameter[] cmdParams, CancellationToken cancellationToken)
		{
			int affrows = 0;
			DbCommand cmd = null;
			try
			{
				cmd = await PrepareCommandAsync(async, cmdText, cmdType, cmdParams, cancellationToken);
				affrows = async ? await cmd.ExecuteNonQueryAsync(cancellationToken) : cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				ThrowException(cmd, ex);
			}
			finally
			{
				await CloseCommandAsync(async, cmd);
			}
			return affrows;
		}
		#endregion

		#region ExecuteDataReaderBase
		/// <summary>
		/// 读取数据库reader
		/// </summary>
		public void ExecuteDataReader(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
			=> ExecuteDataReaderBaseAsync(dr =>
			{
				while (dr.Read())
					action?.Invoke(dr);
			}, cmdText, cmdType, cmdParams, false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 读取数据库reader
		/// </summary>
		public ValueTask ExecuteDataReaderAsync(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? new ValueTask(Task.FromCanceled(cancellationToken))
			: ExecuteDataReaderBaseAsync(async dr =>
			{
				while (await dr.ReadAsync(cancellationToken))
					action?.Invoke(dr);

			}, cmdText, cmdType, cmdParams, true, cancellationToken);

		private async ValueTask ExecuteDataReaderBaseAsync(Action<DbDataReader> action, string cmdText, CommandType cmdType, DbParameter[] cmdParams, bool async, CancellationToken cancellationToken)
		{
			DbCommand cmd = null;
			try
			{
				cmd = await PrepareCommandAsync(async, cmdText, cmdType, cmdParams, cancellationToken);
				if (async)
				{
					await using DbDataReader dr = await cmd.ExecuteReaderAsync(cancellationToken);
					action?.Invoke(dr);
				}
				else
				{
					using DbDataReader dr = cmd.ExecuteReader();
					action?.Invoke(dr);
				}
			}
			catch (Exception ex)
			{
				ThrowException(cmd, ex);
			}
			finally
			{
				await CloseCommandAsync(async, cmd);
			}
		}
		#endregion

		#region ExecuteDataReaderModel
		public Task<T> ExecuteDataReaderModelAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
			=> ExecuteDataReaderModelAsync<T>(true, cmdText, cmdType, cmdParams, cancellationToken);

		public T ExecuteDataReaderModel<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
			=> ExecuteDataReaderModelAsync<T>(false, cmdText, cmdType, cmdParams, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		private async Task<T> ExecuteDataReaderModelAsync<T>(bool async, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
		{
			var list = await ExecuteDataReaderListAsync<T>(async, cmdText, cmdType, cmdParams, cancellationToken);
			return list.Count > 0 ? list[0] : default;
		}
		#endregion

		#region ExecuteDataReaderList
		public Task<List<T>> ExecuteDataReaderListAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
		=> ExecuteDataReaderListAsync<T>(true, cmdText, cmdType, cmdParams, cancellationToken);

		public List<T> ExecuteDataReaderList<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
			=> ExecuteDataReaderListAsync<T>(false, cmdText, cmdType, cmdParams, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		private async Task<List<T>> ExecuteDataReaderListAsync<T>(bool async, string cmdText, CommandType cmdType, DbParameter[] cmdParams, CancellationToken cancellationToken)
		{
			var list = new List<T>();
			if (async)
				await ExecuteDataReaderAsync(dr =>
				{
					list.Add(CreeperDbContext.GetConvert(ConnectionOptions.DataBaseKind).ConvertDataReader<T>(dr));
				}, cmdText, cmdType, cmdParams, cancellationToken);
			else
				ExecuteDataReader(dr =>
				{
					list.Add(CreeperDbContext.GetConvert(ConnectionOptions.DataBaseKind).ConvertDataReader<T>(dr));
				}, cmdText, cmdType, cmdParams);
			return list;
		}

		#endregion

		#region ExecuteDataReaderPipe
		public Task<object[]> ExecuteDataReaderPipeAsync(IEnumerable<ISqlBuilder> builders, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
		=> ExecuteDataReaderPipeAsync(true, builders, cmdType, cancellationToken);

		public object[] ExecuteDataReaderPipe(IEnumerable<ISqlBuilder> builders, CommandType cmdType = CommandType.Text)
			=> ExecuteDataReaderPipeAsync(false, builders, cmdType, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		private async Task<object[]> ExecuteDataReaderPipeAsync(bool async, IEnumerable<ISqlBuilder> builders, CommandType cmdType, CancellationToken cancellationToken)
		{
			if (!builders?.Any() ?? true)
				throw new ArgumentNullException(nameof(builders));

			if (builders.Any(a => a.ReturnType == PipeReturnType.Rows)
				&& builders.Any(a => a.ReturnType != PipeReturnType.Rows))
				throw new NotSupportedException("暂不支持同时返回结果和修改行数");

			object[] results = new object[builders.Count()];
			var paras = new List<DbParameter>();
			var cmdText = new StringBuilder();
			foreach (var item in builders)
			{
				paras.AddRange(item.Params);
				cmdText.Append(item.CommandText).AppendLine(";");
			}
			if (async)
				await ExecuteDataReaderBaseAsync(async dr =>
				{
					for (int i = 0; i < results.Length; i++)
					{
						var item = builders.ElementAt(i);
						List<object> list = new List<object>();
						while (await dr.ReadAsync(cancellationToken))
							list.Add(CreeperDbContext.GetConvert(ConnectionOptions.DataBaseKind).ConvertDataReader(dr, item.Type));

						results[i] = GetResult(dr, item, list);

						await dr.NextResultAsync();
					}
				}, cmdText.ToString(), cmdType, paras.ToArray(), true, cancellationToken);
			else
				ExecuteDataReaderBaseAsync(dr =>
				{
					for (int i = 0; i < results.Length; i++)
					{
						var item = builders.ElementAt(i);
						List<object> list = new List<object>();
						while (dr.Read())
							list.Add(CreeperDbContext.GetConvert(ConnectionOptions.DataBaseKind).ConvertDataReader(dr, item.Type));

						results[i] = GetResult(dr, item, list);

						dr.NextResult();
					}
				}, cmdText.ToString(), cmdType, paras.ToArray(), false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

			static object GetResult(DbDataReader dr, ISqlBuilder item, List<object> list)
			{
				return item.ReturnType switch
				{
					PipeReturnType.List =>
					   list.ToArray(),
					PipeReturnType.One =>
					   list.Count > 0 ? list[0] : item.Type.IsTuple() ? Activator.CreateInstance(item.Type) : default, // 返回默认值
					PipeReturnType.Rows =>
						dr.RecordsAffected,
					_ => throw new ArgumentException("ReturnType is wrong", nameof(item.ReturnType)),
				};
			}

			return results;
		}
		#endregion

		#region Transaction
		/// <summary>
		/// 开启事务
		/// </summary>
		public ICreeperDbExecute BeginTransaction()
			=> BeginTransactionAsync(false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 开启事务
		/// </summary>
		public ValueTask<ICreeperDbExecute> BeginTransactionAsync(CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested ? new ValueTask<ICreeperDbExecute>(Task.FromCanceled<ICreeperDbExecute>(cancellationToken)) : BeginTransactionAsync(true, cancellationToken);

		/// <summary>
		/// 确认事务
		/// </summary>
		public void CommitTransaction()
			=> CommitTransactionAsync(false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 确认事务
		/// </summary>
		public ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested ? new ValueTask(Task.FromCanceled(cancellationToken)) : CommitTransactionAsync(true, cancellationToken);

		private async ValueTask CommitTransactionAsync(bool async, CancellationToken cancellationToken)
		{
			using (_trans)
			{
				using var conn = _trans?.Connection;
				if (async)
					await _trans.CommitAsync(cancellationToken);
				else
					_trans.Commit();
			}
		}

		/// <summary>
		/// 回滚事务
		/// </summary>
		public void RollBackTransaction()
			=> RollBackTransactionAsync(false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 回滚事务
		/// </summary>
		public ValueTask RollBackTransactionAsync(CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested ? new ValueTask(Task.FromCanceled(cancellationToken)) : RollBackTransactionAsync(true, cancellationToken);

		private async ValueTask RollBackTransactionAsync(bool async, CancellationToken cancellationToken)
		{
			using (_trans)
			{
				using var conn = _trans?.Connection;
				if (async)
					await _trans.RollbackAsync(cancellationToken);
				else
					_trans.Rollback();
			}
		}

		/// <summary>
		/// 事务
		/// </summary>
		/// <param name="action"></param>
		public void Transaction(Action<ICreeperDbExecute> action)
			=> TransactionAsync(false, action, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 事务
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask TransactionAsync(Action<ICreeperDbExecute> action, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested ? new ValueTask(Task.FromCanceled(cancellationToken)) : TransactionAsync(true, action, cancellationToken);

		private async ValueTask<ICreeperDbExecute> BeginTransactionAsync(bool async, CancellationToken cancellationToken)
		{
			if (_trans != null)
				throw new Exception("exists a transaction already");

			var conn = async ? await ConnectionOptions.GetConnectionAsync(cancellationToken) : ConnectionOptions.GetConnection();
			_trans = async ? await conn.BeginTransactionAsync(cancellationToken) : conn.BeginTransaction();
			return this;
		}

		private async ValueTask TransactionAsync(bool async, Action<ICreeperDbExecute> action, CancellationToken cancellationToken)
		{
			try
			{
				if (async) await BeginTransactionAsync(cancellationToken);
				else BeginTransaction();
				action(this);
				if (async) await CommitTransactionAsync(cancellationToken);
				else CommitTransaction();
			}
			catch (Exception ex)
			{
				if (async) await RollBackTransactionAsync(cancellationToken);
				else RollBackTransaction();
				throw ex;
			}
		}

		#endregion

		#region Prepare and Quit
		public void Dispose()
		{
			_trans?.Dispose();
			GC.SuppressFinalize(this);
		}

		private async ValueTask<DbCommand> PrepareCommandAsync(bool async, string cmdText, CommandType cmdType, DbParameter[] cmdParams, CancellationToken cancellationToken)
		{

			if (string.IsNullOrEmpty(cmdText))
				throw new ArgumentNullException(nameof(cmdText));

			DbCommand cmd;
			using (cancellationToken.Register(cmd => ((DbCommand)cmd!).Cancel(), this))
			{
				if (_trans == null)
				{
					var conn = async ? await ConnectionOptions.GetConnectionAsync(cancellationToken) : ConnectionOptions.GetConnection();
					cmd = conn.CreateCommand();
				}
				else
				{
					cmd = _trans.Connection.CreateCommand();
					cmd.Transaction = _trans;
				}
				cmd.CommandText = cmdText;
				cmd.CommandType = cmdType;
				if (cmdParams?.Any() != true) return cmd;

				foreach (var p in cmdParams)
				{
					if (p == null) continue;
					if ((p.Direction == ParameterDirection.Input || p.Direction == ParameterDirection.InputOutput) && p.Value == null)
						p.Value = DBNull.Value;
					cmd.Parameters.Add(p);
				}
			}
			return cmd;
		}

		/// <summary>
		/// 抛出异常
		/// </summary>
		private void ThrowException(DbCommand cmd, Exception ex)
		{
			var ht = new Hashtable();
			if (cmd?.Parameters != null)
				foreach (DbParameter item in cmd?.Parameters)
					ht[item.ParameterName] = item.Value;

			string msg = string.Format("{1}数据库执行出错：===== {0}", JsonConvert.SerializeObject(new
			{
				ConnectionString = cmd?.Connection?.ConnectionString,
				CmdText = cmd?.CommandText,
				Parameters = ht
			}), ConnectionOptions.DbName);

			throw new CreeperSqlExecuteException(msg, ex);
		}

		private async ValueTask CloseConnectionAsync(bool async, DbConnection connection)
		{
			if (connection != null && connection.State != ConnectionState.Closed)
			{
				if (async)
					await connection.DisposeAsync();
				else
					connection.Dispose();
			}
		}

		private async ValueTask CloseCommandAsync(bool async, DbCommand cmd)
		{
			if (cmd == null)
				return;
			if (cmd.Parameters != null)
				cmd.Parameters.Clear();

			if (_trans == null)
				await CloseConnectionAsync(async, cmd.Connection);

			if (async)
				await cmd.DisposeAsync();
			else
				cmd.Dispose();
		}
		#endregion

	}
}
