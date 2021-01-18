using Candy.Driver.Common;
using Candy.Driver.Model;
using Candy.Driver.PostgreSql;
using Npgsql;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Candy.Driver.Model
{
	public class CandyPostgreSqlDbConnectionOptions : ICandyDbConnectionOptions
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="connectionString">数据库连接</param>
		/// <param name="dbName">数据库别名</param>
		/// <param name="options">配置</param>
		public CandyPostgreSqlDbConnectionOptions(string connectionString, string dbName, DbConnectionOptions options)
		{
			ConnectionString = connectionString;
			DbName = dbName;
			Options = options;
			TypeConverter = new PostgreSqlTypeConverter();
		}

		public string DbName { get; }

		/// <summary>
		/// 数据库连接
		/// </summary>
		public string ConnectionString { get; }

		/// <summary>
		/// 数据库类型
		/// </summary>
		public DatabaseType Type { get; } = DatabaseType.Postgres;

		/// <summary>
		/// 针对不同类型的数据库需要响应的配置
		/// </summary>
		public DbConnectionOptions Options { get; }

		public ICandyTypeConverter TypeConverter { get; }

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

		public DbParameter GetDbParameter(string name, object value)
		{
			return new NpgsqlParameter(name, value);
		}

		async Task<DbConnection> GetConnectionAsync(bool async, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
				return await Task.FromCanceled<DbConnection>(cancellationToken);

			DbConnection connection = new NpgsqlConnection(ConnectionString);

			if (connection == null)
				throw new ArgumentNullException(nameof(connection));

			if (async)
				await connection.OpenAsync(cancellationToken);
			else
				connection.Open();

			SetDatabaseOption(connection);
			return connection;
		}

		void SetDatabaseOption(DbConnection connection)
		{
			if (Type == DatabaseType.Postgres)
				Options?.MapAction?.Invoke((NpgsqlConnection)connection);
		}
	}
}
