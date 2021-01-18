using Candy.Driver.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Candy.Driver.Common
{
	public interface ICandyDbConnectionOptions
	{
		/// <summary>
		/// 数据库名称
		/// </summary>
		string DbName { get; }

		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		string ConnectionString { get; }

		/// <summary>
		/// 数据库类型
		/// </summary>
		DatabaseType Type { get; }

		/// <summary>
		/// 获取dbconnection
		/// </summary>
		/// <returns></returns>
		DbConnection GetConnection();

		/// <summary>
		/// 获取dbconnection
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken);

		/// <summary>
		/// 获取dbparameter
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		DbParameter GetDbParameter(string name, object value);

		/// <summary>
		/// 获取类型装换起
		/// </summary>
		ICandyTypeConverter TypeConverter { get; }
	}
}
