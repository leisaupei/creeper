using Creeper.Driver;
using Npgsql;
using System;
using System.Data;
using System.Linq;

namespace Creeper.PostgreSql
{
	/// <summary>
	/// db配置
	/// </summary>
	/// <typeparam name="TDbMaterName">主库名称</typeparam>
	/// <typeparam name="TDbSecondaryName">从库名称</typeparam>
	public abstract class BasePostgreSqlDbOption<TDbMaterName, TDbSecondaryName> : ICreeperDbOption
        where TDbMaterName : struct, ICreeperDbName
        where TDbSecondaryName : struct, ICreeperDbName
    {
        private readonly string _mainConnectionString;
        private readonly string[] _secondaryConnectionStrings;

        /// <summary>
        /// 数据库连接配置
        /// </summary>
        public abstract DbConnectionOptions Options { get; }

        public BasePostgreSqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings)
        {
            _mainConnectionString = mainConnectionString;
            _secondaryConnectionStrings = secondaryConnectionStrings;

        }

        /// <summary>
        /// 主库对象
        /// </summary>
        ICreeperDbConnectionOption ICreeperDbOption.Main =>
            new PostgreSqlDbConnectionOptions(_mainConnectionString, typeof(TDbMaterName).Name, Options);

        /// <summary>
        /// 从库数组对象
        /// </summary>
        ICreeperDbConnectionOption[] ICreeperDbOption.Secondary
            => _secondaryConnectionStrings?.Select(f => new PostgreSqlDbConnectionOptions(f, typeof(TDbSecondaryName).Name, Options)).ToArray()
            ?? new PostgreSqlDbConnectionOptions[0];
    }
    public class DbConnectionOptions
    {
        /// <summary>
        /// Postgres SQL CLR映射
        /// </summary>
        public Action<NpgsqlConnection> MapAction { get; set; }
    }
}
