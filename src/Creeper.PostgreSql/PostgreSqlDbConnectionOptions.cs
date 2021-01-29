using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql;
using Npgsql;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.PostgreSql
{
	public class PostgreSqlDbConnectionOptions : ICreeperDbConnectionOption
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionString">数据库连接</param>
        /// <param name="dbName">数据库别名</param>
        /// <param name="options">配置</param>
        public PostgreSqlDbConnectionOptions(string connectionString, string dbName, DbConnectionOptions options)
        {
            ConnectionString = connectionString;
            DbName = dbName;
            Options = options;
        }

        public string DbName { get; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseKind DataBaseKind { get; } = DataBaseKind.PostgreSql;

        /// <summary>
        /// 针对不同类型的数据库需要响应的配置
        /// </summary>
        public DbConnectionOptions Options { get; }

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

		public DbParameter GetDbParameter(string name, object value) => new NpgsqlParameter(name, value);

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
            Options?.MapAction?.Invoke((NpgsqlConnection)connection);
        }
    }
}
