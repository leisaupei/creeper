using Candy.Common;
using Npgsql;
using System;
using System.Data;
using System.Linq;

namespace Candy.PostgreSql
{
	/// <summary>
	/// db配置
	/// </summary>
	/// <typeparam name="TDbMaterName">主库名称</typeparam>
	/// <typeparam name="TDbSecondaryName">从库名称</typeparam>
	public class BasePostgreSqlDbOption<TDbMaterName, TDbSecondaryName> : ICandyDbOption
		where TDbMaterName : struct, ICandyDbName
		where TDbSecondaryName : struct, ICandyDbName
	{
		private readonly string _mainConnectionString;
		private readonly string[] _secondaryConnectionStrings;
		/// <summary>
		/// 数据库连接配置
		/// </summary>
		public DbConnectionOptions Options { get; private set; } = new DbConnectionOptions();

		public BasePostgreSqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings)
		{
			_mainConnectionString = mainConnectionString;
			_secondaryConnectionStrings = secondaryConnectionStrings;

		}

		/// <summary>
		/// 主库对象
		/// </summary>
		ICandyDbConnectionOptions ICandyDbOption.Main =>
			new CandyPostgreSqlDbConnectionOptions(_mainConnectionString, typeof(TDbMaterName).Name, Options);

		/// <summary>
		/// 从库数组对象
		/// </summary>
		ICandyDbConnectionOptions[] ICandyDbOption.Secondary
			=> _secondaryConnectionStrings?.Select(f => new CandyPostgreSqlDbConnectionOptions(f, typeof(TDbSecondaryName).Name, Options)).ToArray()
			?? new CandyPostgreSqlDbConnectionOptions[0];
	}
	public class DbConnectionOptions
	{
		/// <summary>
		/// Postgres SQL CLR映射
		/// </summary>
		public Action<NpgsqlConnection> MapAction { get; set; }
	}
}
