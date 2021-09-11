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
using Creeper.Generic;
using Creeper.SqlBuilder;

namespace Creeper.Driver
{
	public class CreeperExecute : ICreeperExecute
	{
		public ICreeperConnection ConnectionOptions { get; }

		public bool InTransaction => _trans != null;
		/// <summary>
		/// 事务
		/// </summary>
		private DbTransaction _trans;

		/// <summary>
		/// 是否正在使用管道模式
		/// </summary>
		private bool _piping = false;

		/// <summary>
		/// 当前数据库连接
		/// </summary>
		private DbConnection _connection;

		private readonly CreeperConverter _converter;

		public CreeperExecute(ICreeperConnection connectionOptions)
		{
			ConnectionOptions = connectionOptions;
			_converter = ConnectionOptions.Converter;
		}

		#region ExecuteScalar
		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		public object ExecuteScalar(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> ExecuteScalarAsync(false, cmdText, cmdParams, cmdType, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		public ValueTask<object> ExecuteScalarAsync(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? new ValueTask<object>(Task.FromCanceled<object>(cancellationToken))
			: ExecuteScalarAsync(true, cmdText, cmdParams, cmdType, cancellationToken);

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		public T ExecuteScalar<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> ExecuteScalarAsync<T>(false, cmdText, cmdParams, cmdType, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		public ValueTask<T> ExecuteScalarAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? new ValueTask<T>(Task.FromCanceled<T>(cancellationToken))
			: ExecuteScalarAsync<T>(true, cmdText, cmdParams, cmdType, cancellationToken);

		private async ValueTask<object> ExecuteScalarAsync(bool async, string cmdText, DbParameter[] cmdParams, CommandType cmdType, CancellationToken cancellationToken)
		{
			DbCommand cmd = null;
			object ret = null;
			try
			{
				cmd = await PrepareCommandAsync(async, cmdText, cmdParams, cmdType, cancellationToken);
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

		private async ValueTask<T> ExecuteScalarAsync<T>(bool async, string cmdText, DbParameter[] cmdParams, CommandType cmdType, CancellationToken cancellationToken)
		{
			var value = async
				? await ExecuteScalarAsync(cmdText, cmdParams, cmdType, cancellationToken)
				: ExecuteScalar(cmdText, cmdParams, cmdType);
			return value == null ? default : _converter.ConvertData<T>(value);
		}

		#endregion

		#region ExecuteNonQuery
		/// <summary>
		/// 执行sql语句
		/// </summary>
		public int ExecuteNonQuery(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> ExecuteNonQueryAsync(false, cmdText, cmdParams, cmdType, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 执行sql语句
		/// </summary>
		public ValueTask<int> ExecuteNonQueryAsync(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? new ValueTask<int>(Task.FromCanceled<int>(cancellationToken))
			: ExecuteNonQueryAsync(true, cmdText, cmdParams, cmdType, cancellationToken);

		private async ValueTask<int> ExecuteNonQueryAsync(bool async, string cmdText, DbParameter[] cmdParams, CommandType cmdType, CancellationToken cancellationToken)
		{
			int affrows = 0;
			DbCommand cmd = null;
			try
			{
				cmd = await PrepareCommandAsync(async, cmdText, cmdParams, cmdType, cancellationToken);
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

		#region ExecuteReader.Base
		/// <summary>
		/// 读取数据库reader
		/// </summary>
		public void ExecuteReader(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> ExecuteReaderAsync(dr =>
			{
				while (dr.Read()) action?.Invoke(dr);
				return Task.CompletedTask;
			}, cmdText, cmdParams, cmdType, false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 读取数据库reader
		/// </summary>
		public ValueTask ExecuteReaderAsync(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? new ValueTask(Task.FromCanceled(cancellationToken))
			: ExecuteReaderAsync(async dr =>
			{
				while (await dr.ReadAsync(cancellationToken)) action?.Invoke(dr);
			}, cmdText, cmdParams, cmdType, true, cancellationToken);

		/// <summary>
		/// 遍历reader结果
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cmdText"></param>
		/// <param name="cmdType"></param>
		/// <param name="cmdParams"></param>
		/// <param name="async"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async ValueTask ExecuteReaderAsync(ActionAsync<DbDataReader> action, string cmdText, DbParameter[] cmdParams, CommandType cmdType, bool async, CancellationToken cancellationToken)
		{
			await ExecuteReaderAffrowsAsync(action, cmdText, cmdParams, cmdType, async, cancellationToken);
		}
		#endregion

		#region ExecuteReader.Affrows

		/// <summary>
		/// 读取数据库reader
		/// </summary>
		public int ExecuteReaderAffrows(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> ExecuteReaderAffrowsAsync(dr =>
			{
				while (dr.Read()) action?.Invoke(dr);
				return Task.CompletedTask;
			}, cmdText, cmdParams, cmdType, false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 读取数据库reader
		/// </summary>
		public ValueTask<int> ExecuteReaderAffrowsAsync(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? new ValueTask<int>(Task.FromCanceled<int>(cancellationToken))
			: ExecuteReaderAffrowsAsync(async dr =>
			{
				while (await dr.ReadAsync(cancellationToken)) action?.Invoke(dr);
			}, cmdText, cmdParams, cmdType, true, cancellationToken);

		/// <summary>
		/// 返回修改行数并遍历reader结果
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cmdText"></param>
		/// <param name="cmdType"></param>
		/// <param name="cmdParams"></param>
		/// <param name="async"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async ValueTask<int> ExecuteReaderAffrowsAsync(ActionAsync<DbDataReader> action, string cmdText, DbParameter[] cmdParams, CommandType cmdType, bool async, CancellationToken cancellationToken)
		{
			if (action is null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			DbCommand cmd = null;
			var affrows = 0;
			try
			{
				cmd = await PrepareCommandAsync(async, cmdText, cmdParams, cmdType, cancellationToken);
				if (async)
				{
					var dr = await cmd.ExecuteReaderAsync(cancellationToken);
					await action.Invoke(dr);
					affrows = dr.RecordsAffected;
					await dr.DisposeAsync();
				}
				else
				{
					var dr = cmd.ExecuteReader();
					action.Invoke(dr).ConfigureAwait(false).GetAwaiter().GetResult();
					affrows = dr.RecordsAffected;
					dr.Dispose();
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

			return affrows;
		}
		#endregion

		#region ExecuteReader.First
		public Task<T> ExecuteReaderFirstAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? Task.FromCanceled<T>(cancellationToken)
			: ExecuteReaderFirstAsync<T>(true, cmdText, cmdParams, cmdType, cancellationToken);

		public T ExecuteReaderFirst<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> ExecuteReaderFirstAsync<T>(false, cmdText, cmdParams, cmdType, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		private async Task<T> ExecuteReaderFirstAsync<T>(bool async, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
		{
			var list = await ExecuteReaderListAsync<T>(async, cmdText, cmdParams, cmdType, cancellationToken);
			return list.Count > 0 ? list[0] : default;
		}
		#endregion

		#region ExecuteReader.First.Affrows
		public Task<AffrowsResult<T>> ExecuteReaderFirstAffrowsAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? Task.FromCanceled<AffrowsResult<T>>(cancellationToken)
			: ExecuteReaderFirstAffrowsAsync<T>(true, cmdText, cmdParams, cmdType, cancellationToken);

		public AffrowsResult<T> ExecuteReaderFirstAffrows<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> ExecuteReaderFirstAffrowsAsync<T>(false, cmdText, cmdParams, cmdType, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		private async Task<AffrowsResult<T>> ExecuteReaderFirstAffrowsAsync<T>(bool async, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
		{
			var result = await ExecuteReaderListAffrowsAsync<T>(async, cmdText, cmdParams, cmdType, cancellationToken);
			return new AffrowsResult<T>(result.AffectedRows, result.Value.Count > 0 ? result.Value[0] : default);
		}
		#endregion

		#region ExecuteReader.List.Affrows
		public Task<AffrowsResult<List<T>>> ExecuteReaderListAffrowsAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? Task.FromCanceled<AffrowsResult<List<T>>>(cancellationToken)
			: ExecuteReaderListAffrowsAsync<T>(true, cmdText, cmdParams, cmdType, cancellationToken);

		public AffrowsResult<List<T>> ExecuteReaderListAffrows<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> ExecuteReaderListAffrowsAsync<T>(false, cmdText, cmdParams, cmdType, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		private async Task<AffrowsResult<List<T>>> ExecuteReaderListAffrowsAsync<T>(bool async, string cmdText, DbParameter[] cmdParams, CommandType cmdType, CancellationToken cancellationToken)
		{
			var list = new List<T>();
			var affrows = async
				? await ExecuteReaderAffrowsAsync(dr => list.Add(_converter.ConvertDataReader<T>(dr)), cmdText, cmdParams, cmdType, cancellationToken)
				: ExecuteReaderAffrows(dr => list.Add(_converter.ConvertDataReader<T>(dr)), cmdText, cmdParams, cmdType);
			//return new AffrowsResult<List<T>>(affrows, list);
			return new AffrowsResult<List<T>>(affrows, affrows > 0 ? list : new List<T>());
		}
		#endregion

		#region ExecuteReader.List
		public Task<List<T>> ExecuteReaderListAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? Task.FromCanceled<List<T>>(cancellationToken)
			: ExecuteReaderListAsync<T>(true, cmdText, cmdParams, cmdType, cancellationToken);

		public List<T> ExecuteReaderList<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text)
			=> ExecuteReaderListAsync<T>(false, cmdText, cmdParams, cmdType, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		private async Task<List<T>> ExecuteReaderListAsync<T>(bool async, string cmdText, DbParameter[] cmdParams, CommandType cmdType, CancellationToken cancellationToken)
		{
			var list = new List<T>();
			if (async)
				await ExecuteReaderAsync(dr => list.Add(_converter.ConvertDataReader<T>(dr)), cmdText, cmdParams, cmdType, cancellationToken);
			else
				ExecuteReader(dr => list.Add(_converter.ConvertDataReader<T>(dr)), cmdText, cmdParams, cmdType);
			return list;
		}

		#endregion

		#region ExecuteReader.Pipe
		public void ExecutePipe(Action<ICreeperExecute> action)
		{
			_piping = true;
			using (this)
				action.Invoke(this);
		}

		public async ValueTask ExecutePipeAsync(ActionAsync<ICreeperExecute> action)
		{
			_piping = true;
			await using (this)
				await action.Invoke(this);
		}

		public Task<object[]> ExecutePipeAsync(IEnumerable<ISqlBuilder> builders, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested
			? Task.FromCanceled<object[]>(cancellationToken)
			: ExecutePipeAsync(true, builders, cancellationToken);

		public object[] ExecutePipe(IEnumerable<ISqlBuilder> builders)
			=> ExecutePipeAsync(false, builders, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		private async Task<object[]> ExecutePipeAsync(bool async, IEnumerable<ISqlBuilder> builders, CancellationToken cancellationToken)
		{
			if (!builders?.Any() ?? true)
				throw new ArgumentNullException(nameof(builders));

			if (builders.Any(a => a.ReturnType == PipeReturnType.Affrows)
				&& builders.Any(a => a.ReturnType != PipeReturnType.Affrows))
				throw new CreeperNotSupportedException("暂不支持同时返回结果和修改行数");

			object[] results = new object[builders.Count()];
			var paras = new List<DbParameter>();
			var cmdText = new StringBuilder();
			foreach (var item in builders)
			{
				paras.AddRange(item.Params);
				cmdText.Append(item.CommandText).AppendLine(";");
			}
			//cmdText = cmdText.Insert(0, "BEGIN" + Environment.NewLine).Append("END;");
			var affrows = await ExecuteReaderAffrowsAsync(dr => SetResult(builders, dr, results, async, cancellationToken),
				cmdText.ToString(), paras.ToArray(), CommandType.Text, async, cancellationToken);

			if (!builders.Any(a => a.ReturnType != PipeReturnType.Affrows))
				results = new object[1] { affrows };
			return results;
		}

		async Task SetResult(IEnumerable<ISqlBuilder> builders, DbDataReader dr, object[] results, bool async, CancellationToken cancellationToken)
		{
			for (int i = 0; i < results.Length; i++)
			{
				var item = builders.ElementAt(i);
				var list = new List<object>();
				while (async ? await dr.ReadAsync(cancellationToken) : dr.Read())
					list.Add(_converter.ConvertDataReader(dr, item.Type));

				results[i] = GetResult(dr, item, list);

				if (async)
					await dr.NextResultAsync(cancellationToken);
				else
					dr.NextResult();
			}
		}

		static object GetResult(DbDataReader dr, ISqlBuilder item, List<object> list)
		{
			return item.ReturnType switch
			{
				PipeReturnType.List =>
					list.ToArray(),
				PipeReturnType.First =>
					list.Count > 0 ? list[0] : item.Type.IsTuple() ? Activator.CreateInstance(item.Type) : default, // 返回默认值
				PipeReturnType.Affrows =>
					dr.RecordsAffected,
				_ => throw new ArgumentException("ReturnType is wrong", nameof(item.ReturnType)),
			};
		}
		#endregion

		#region Transaction
		#region Begin
		/// <summary>
		/// 开启事务
		/// </summary>
		public ICreeperExecute BeginTransaction()
			=> BeginTransactionAsync(false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 开启事务
		/// </summary>
		public ValueTask<ICreeperExecute> BeginTransactionAsync(CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested ? new ValueTask<ICreeperExecute>(Task.FromCanceled<ICreeperExecute>(cancellationToken)) : BeginTransactionAsync(true, cancellationToken);

		private async ValueTask<ICreeperExecute> BeginTransactionAsync(bool async, CancellationToken cancellationToken)
		{
			if (_trans != null)
				throw new Exception("exists a transaction already");
			await GetConnectionAsync(async, cancellationToken);
			_trans = async ? await _connection.BeginTransactionAsync(cancellationToken) : _connection.BeginTransaction();
			return this;
		}
		#endregion

		#region Commit
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
			if (async)
				await _trans.CommitAsync(cancellationToken);
			else
				_trans.Commit();

			await CloseConnectionAsync(async);
			await CloseTransactionAsync(async);
		}
		#endregion

		#region Rollback
		/// <summary>
		/// 回滚事务
		/// </summary>
		public void RollbackTransaction()
			=> RollbackTransactionAsync(false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 回滚事务
		/// </summary>
		public ValueTask RollbackTransactionAsync(CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested ? new ValueTask(Task.FromCanceled(cancellationToken)) : RollbackTransactionAsync(true, cancellationToken);

		private async ValueTask RollbackTransactionAsync(bool async, CancellationToken cancellationToken)
		{
			if (async)
				await _trans.RollbackAsync(cancellationToken);
			else
				_trans.Rollback();

			await CloseConnectionAsync(async);
			await CloseTransactionAsync(async);
		}
		#endregion

		#region Extension
		/// <summary>
		/// 事务
		/// </summary>
		/// <param name="action"></param>
		public void Transaction(Action<ICreeperExecute> action)
			=> TransactionAsync(false, action, null, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 事务
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask TransactionAsync(Action<ICreeperExecute> action, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested ? new ValueTask(Task.FromCanceled(cancellationToken)) : TransactionAsync(true, action, null, cancellationToken);

		/// <summary>
		/// 事务
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public ValueTask TransactionAsync(ActionAsync<ICreeperExecute> action, CancellationToken cancellationToken = default)
			=> cancellationToken.IsCancellationRequested ? new ValueTask(Task.FromCanceled(cancellationToken)) : TransactionAsync(true, null, action, cancellationToken);

		private async ValueTask TransactionAsync(bool async, Action<ICreeperExecute> action, ActionAsync<ICreeperExecute> actionAsync, CancellationToken cancellationToken)
		{
			try
			{
				if (async) await BeginTransactionAsync(cancellationToken);
				else BeginTransaction();
				if (action != null) action.Invoke(this);
				if (actionAsync != null) await actionAsync.Invoke(this);
				if (async) await CommitTransactionAsync(cancellationToken);
				else CommitTransaction();
			}
			catch (Exception ex)
			{
				if (async) await RollbackTransactionAsync(cancellationToken);
				else RollbackTransaction();
				throw ex;
			}
		}
		#endregion

		#endregion

		#region Prepare and Quit
		public void Dispose() => DisposeAsync(false).ConfigureAwait(false).GetAwaiter().GetResult();

		public ValueTask DisposeAsync() => DisposeAsync(true);

		~CreeperExecute() => Dispose();

		private async ValueTask DisposeAsync(bool async)
		{
			_piping = false;
			await CloseTransactionAsync(async);
			await CloseConnectionAsync(async);
			GC.SuppressFinalize(this);
		}

		private async ValueTask<DbCommand> PrepareCommandAsync(bool async, string cmdText, DbParameter[] cmdParams, CommandType cmdType, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(cmdText))
				throw new ArgumentNullException(nameof(cmdText));
			DbCommand cmd;
			using (cancellationToken.Register(cmd => ((DbCommand)cmd!).Cancel(), this))
			{
				if (_trans == null)
				{
					await GetConnectionAsync(async, cancellationToken);
					cmd = _connection.CreateCommand();
				}
				else
				{
					cmd = _trans.Connection.CreateCommand();
					cmd.Transaction = _trans;
				}
				_converter.PrepareDbCommand(cmd);

				cmd.CommandText = cmdText;
				cmd.CommandType = cmdType;
				if (!cmdParams?.Any() ?? true) return cmd;

				_converter.OrderDbParameters(cmdText, ref cmdParams);

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
			if (cmd == null)
				throw new CreeperSqlExecuteException(ex.ToString(), ex);

			var exception = new CreeperSqlExecuteException("数据库执行出错", ex);
			exception.Data["ConnectionString"] = cmd.Connection?.ConnectionString;
			exception.Data["CommandText"] = cmd.CommandText;

			var ps = new Hashtable();
			if (cmd.Parameters != null)
				foreach (DbParameter item in cmd.Parameters)
					ps[item.ParameterName] = item.Value;
			exception.Data["Parameters"] = ps;

			throw exception;
		}

		private async ValueTask GetConnectionAsync(bool async, CancellationToken cancellationToken = default)
		{
			if (_piping && _connection != null) return;

			if (_connection != null)
				await CloseConnectionAsync(async);

			_connection = async ? await ConnectionOptions.GetConnectionAsync(cancellationToken) : ConnectionOptions.GetConnection();
			ConnectionOptions.DbConnectionOptions?.Invoke(_connection);
		}

		private async ValueTask CloseTransactionAsync(bool async)
		{
			if (_trans == null) return;

			if (async) await _trans.DisposeAsync();
			else _trans.Dispose();

			_trans = null;
		}

		private async Task CloseConnectionAsync(bool async)
		{
			if (_connection == null) return;
			if (_piping) return;

			if (async) await ConnectionOptions.CloseAsync(_connection);
			else ConnectionOptions.Close(_connection);

			_connection = null;
		}

		private async ValueTask CloseCommandAsync(bool async, DbCommand cmd)
		{
			if (cmd == null) return;

			if (async) await cmd.DisposeAsync();
			else cmd.Dispose();

			if (_trans == null && !_piping)
				await CloseConnectionAsync(async);
		}

		#endregion

	}
}