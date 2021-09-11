using Creeper.Generic;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public interface ICreeperConnection : IDisposable, IAsyncDisposable
	{
		/// <summary>
		/// 数据库特性转换器
		/// </summary>
		CreeperConverter Converter { get; }

		/// <summary>
		/// 获取dbconnection
		/// </summary>
		/// <returns></returns>
		DbConnection GetConnection();

		/// <summary>
		/// 数据库连接配置(在<see cref="DbConnection.Open()"/>之后执行)
		/// </summary>
		Action<DbConnection> DbConnectionOptions { get; set; }

		/// <summary>
		/// 获取dbconnection
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken);

		/// <summary>
		/// 关闭连接
		/// </summary>
		/// <param name="connection"></param>
		void Close(DbConnection connection);

		/// <summary>
		/// 关闭连接
		/// </summary>
		/// <param name="connection"></param>
		Task CloseAsync(DbConnection connection);
	}
}
