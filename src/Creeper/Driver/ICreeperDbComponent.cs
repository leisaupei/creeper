using Creeper.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public interface ICreeperDbComponent
	{
		/// <summary>
		/// 开启事务
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ICreeperDbExecute BeginTransaction();

		/// <summary>
		/// 开启事务
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<ICreeperDbExecute> BeginTransactionAsync(CancellationToken cancellationToken = default);

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
		/// 事务, 需要手动 commit提交事务/rollback回滚事务
		/// </summary>
		/// <param name="action"></param>
		void Transaction(Action<ICreeperDbExecute> action);

		/// <summary>
		/// 事务, 需要手动commit提交事务/rollback回滚事务
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		ValueTask TransactionAsync(Action<ICreeperDbExecute> action, CancellationToken cancellationToken = default);
	}
}
