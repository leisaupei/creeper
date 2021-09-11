using Creeper.Generic;
using Creeper.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public interface ICreeperContext
	{
		/// <summary>
		/// 数据库缓存实例
		/// </summary>
		ICreeperCache Cache { get; }

		/// <summary>
		/// 开启事务
		/// </summary>
		/// <returns></returns>
		ICreeperExecute BeginTransaction();

		/// <summary>
		/// 开启事务
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<ICreeperExecute> BeginTransactionAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 读取数据返回
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		void ExecuteReader(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 读取数据返回
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask ExecuteReaderAsync(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取多行记录, 用实体类列表接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		List<T> ExecuteReaderList<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 获取多行记录, 用实体类列表接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<T>> ExecuteReaderListAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取一行记录用实体类接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		T ExecuteReaderFirst<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 获取一行记录用实体类接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<T> ExecuteReaderFirstAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 执行多个sql语句
		/// </summary>
		/// <param name="builders"></param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		object[] ExecuteReaderPipe(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 执行多个sql语句
		/// </summary>
		/// <param name="builders"></param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<object[]> ExecuteReaderPipeAsync(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取command text修改行数
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		int ExecuteNonQuery(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text);

		/// <summary>
		/// 获取command text修改行数
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		ValueTask<int> ExecuteNonQueryAsync(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		object ExecuteScalar(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		T ExecuteScalar<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<object> ExecuteScalarAsync(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取结果集游标为(0,0)的数据
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<T> ExecuteScalarAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取主/从数据库请求示例
		/// </summary>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		ICreeperExecute Get(DataBaseType dataBaseType);

		/// <summary>
		/// 事务, 自动提交事务, 当action抛出异常时回滚事务
		/// </summary>
		/// <param name="action"></param>
		void Transaction(Action<ICreeperExecute> action);

		/// <summary>
		/// 事务, 自动提交事务, 当action抛出异常时回滚事务
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		ValueTask TransactionAsync(Action<ICreeperExecute> action, CancellationToken cancellationToken = default);

		/// <summary>
		/// 事务, 自动提交事务, 当action抛出异常时回滚事务
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		ValueTask TransactionAsync(ActionAsync<ICreeperExecute> action, CancellationToken cancellationToken = default);

		/// <summary>
		/// 查询结果并返回受影响行数
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="cancellationToken"></param>
		Task<AffrowsResult<T>> ExecuteReaderFirstAffrowsAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default);

		/// <summary>
		/// 查询结果并返回受影响行数
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		AffrowsResult<T> ExecuteReaderFirstAffrows<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text);

		/// <summary>
		/// 查询结果并返回受影响行数
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<AffrowsResult<List<T>>> ExecuteReaderListAffrowsAsync<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default);

		/// <summary>
		/// 查询结果并返回受影响行数
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		AffrowsResult<List<T>> ExecuteReaderListAffrows<T>(string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text);

		/// <summary>
		/// 查询结果并返回受影响行数
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		/// <returns></returns>
		int ExecuteReaderAffrows(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text);

		/// <summary>
		/// 查询结果并返回受影响行数
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		ValueTask<int> ExecuteReaderAffrowsAsync(Action<DbDataReader> action, string cmdText, DbParameter[] cmdParams = null, CommandType cmdType = CommandType.Text, CancellationToken cancellationToken = default);

		/// <summary>
		/// 管道模式
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		ValueTask ExecutePipeAsync(ActionAsync<ICreeperExecute> action, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 管道模式
		/// </summary>
		/// <param name="action"></param>
		void ExecutePipe(Action<ICreeperExecute> action, DataBaseType dataBaseType = DataBaseType.Default);
	}
}
