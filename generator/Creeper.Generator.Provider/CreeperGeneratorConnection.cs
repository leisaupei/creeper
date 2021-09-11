using Creeper.Utils;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	internal class CreeperGeneratorConnection : ICreeperConnection
	{
		public CreeperGeneratorConnection(string connectionString, CreeperConverter dbConverter)
		{
			ConnectionString = connectionString;
			Converter = dbConverter;
		}
		public string ConnectionString { get; }

		public Action<DbConnection> DbConnectionOptions { get; set; }

		public CreeperConverter Converter { get; }

		public void Close(DbConnection connection)
		{
			connection.Dispose();
		}

		public Task CloseAsync(DbConnection connection)
		{
			return connection.DisposeAsync().AsTask();
		}

		public void Dispose()
		{
		}

		public ValueTask DisposeAsync()
		{
			return new ValueTask(Task.CompletedTask);
		}

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

			DbConnection connection = Converter.GetDbConnection(ConnectionString);

			if (connection == null)
				throw new ArgumentNullException(nameof(connection));

			if (async)
				await connection.OpenAsync(cancellationToken);
			else
				connection.Open();

			return connection;
		}
	}
}
