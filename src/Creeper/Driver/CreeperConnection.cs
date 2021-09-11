using Creeper.Utils;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Creeper.Driver
{
	internal class CreeperConnection : ICreeperConnection
	{
		private int _currentSize;
		private readonly int _maxPoolSize;
		private readonly ConcurrentQueue<DbConnection> _pool;
		private readonly SemaphoreSlim _paramLock;
		private readonly object _cLock;
		public CreeperConnection(string connectionString, CreeperConverter converter, int maxPoolSize) : this(connectionString, converter)
		{
			_maxPoolSize = maxPoolSize;
			if (_maxPoolSize <= 0) return;

			_pool = new ConcurrentQueue<DbConnection>();
			_paramLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
			_cLock = new object();
			_currentSize = 0;
		}
		public CreeperConnection(string connectionString, CreeperConverter converter)
		{
			ConnectionString = connectionString;
			Converter = converter;
		}

		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		internal string ConnectionString { get; }

		/// <summary>
		/// 数据库连接配置
		/// </summary>
		public Action<DbConnection> DbConnectionOptions { get; set; }

		/// <summary>
		/// 数据库转换器
		/// </summary>
		public CreeperConverter Converter { get; }

		/// <summary>
		/// 创建连接
		/// </summary>
		/// <returns></returns>
		public DbConnection GetConnection() => GetConnectionAsync(false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 创建连接
		/// </summary>
		/// <returns></returns>
		public Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken)
			=> cancellationToken.IsCancellationRequested ? Task.FromCanceled<DbConnection>(cancellationToken) : GetConnectionAsync(true, cancellationToken);

		async Task<DbConnection> GetConnectionAsync(bool async, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
				return await Task.FromCanceled<DbConnection>(cancellationToken);

			DbConnection connection = null;
			if (_maxPoolSize == 0)
				connection = Converter.GetDbConnection(ConnectionString);
			else
			{
				lock (_cLock)
				{
					while (_currentSize > _maxPoolSize)
					{
						Thread.SpinWait(1);
					}

					if (_pool.TryDequeue(out connection))
					{
						Interlocked.Decrement(ref _currentSize);
					}
				}
				if (connection == null) connection = Converter.GetDbConnection(ConnectionString);
			}
			if (connection == null) await GetConnectionAsync(async, cancellationToken);

			if (async) await connection.OpenAsync(cancellationToken);
			else connection.Open();

			return connection;

		}

		private async Task CloseAsync(bool async, DbConnection connection)
		{
			if (_maxPoolSize != 0)
			{
				if (connection.State != System.Data.ConnectionState.Closed)
				{
					connection.Close();
				}
				try
				{
					_paramLock.Wait();
					if (Interlocked.Increment(ref _currentSize) <= _maxPoolSize)
					{
						_pool.Enqueue(connection);
						return;
					}
					else
						Interlocked.Decrement(ref _currentSize);
				}
				finally
				{
					_paramLock.Release();
				}
			}
			if (async) await connection.DisposeAsync();
			else connection.Dispose();
		}

		public void Close(DbConnection connection) => CloseAsync(false, connection).ConfigureAwait(false).GetAwaiter().GetResult();

		public Task CloseAsync(DbConnection connection) => CloseAsync(true, connection);

		public void Dispose() => DisposeAsync(false).ConfigureAwait(false).GetAwaiter().GetResult();

		private async ValueTask DisposeAsync(bool async)
		{
			if (_maxPoolSize == 0) return;
			while (_pool.TryDequeue(out var connection))
			{
				if (async) await connection.DisposeAsync();
				else connection.Dispose();
			}
			_currentSize = 0;
		}

		public ValueTask DisposeAsync() => DisposeAsync(true);
	}
}
