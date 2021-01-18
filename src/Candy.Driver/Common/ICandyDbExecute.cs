using Candy.Driver.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Candy.Driver.Common
{
	public interface ICandyDbExecute : IDisposable
	{
		ICandyDbConnectionOptions ConnectionOptions { get; }
		/// <summary>
		/// 开启事务
		/// </summary>
		ICandyDbExecute BeginTransaction();

		/// <summary>
		/// 开启事务
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<ICandyDbExecute> BeginTransactionAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 提交事务
		/// </summary>
		void CommitTransaction();

		/// <summary>
		/// 提交事务
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 读取数据返回
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		void ExecuteDataReader(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null);

		/// <summary>
		/// 读取数据返回
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask ExecuteDataReaderAsync(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取多行记录用实体类列表接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		List<T> ExecuteDataReaderList<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null);

		/// <summary>
		/// 获取多行记录用实体类列表接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<T>> ExecuteDataReaderListAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取一行记录用实体类接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		T ExecuteDataReaderModel<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null);

		/// <summary>
		/// 获取一行记录用实体类接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<T> ExecuteDataReaderModelAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 执行多个sql语句
		/// </summary>
		/// <param name="builders"></param>
		/// <param name="cmdType">command type</param>
		/// <returns></returns>
		object[] ExecuteDataReaderPipe(IEnumerable<ISqlBuilder> builders, CommandType cmdType = CommandType.Text);

		/// <summary>
		/// 执行多个sql语句
		/// </summary>
		/// <param name="builders"></param>
		/// <param name="cmdType">command type</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<object[]> ExecuteDataReaderPipeAsync(IEnumerable<ISqlBuilder> builders, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取command text修改行数
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null);

		/// <summary>
		/// 获取command text修改行数
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取单个返回值
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null);

		/// <summary>
		/// 获取单个返回值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		T ExecuteScalar<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null);

		/// <summary>
		/// 获取单个返回值
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<object> ExecuteScalarAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取单个返回值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<T> ExecuteScalarAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 回滚事务
		/// </summary>
		void RollBackTransaction();

		/// <summary>
		/// 回滚事务
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask RollBackTransactionAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 事务, 需要手动 commit提交事务/rollback回滚事务
		/// </summary>
		/// <param name="action"></param>
		void Transaction(Action<ICandyDbExecute> action);

		/// <summary>
		/// 事务, 需要手动commit提交事务/rollback回滚事务
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		ValueTask TransactionAsync(Action<ICandyDbExecute> action, CancellationToken cancellationToken = default);
	}
}
