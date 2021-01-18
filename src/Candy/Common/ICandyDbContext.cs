using System;
using System.Threading;
using System.Threading.Tasks;

namespace Candy.Common
{
	public interface ICandyDbContext
	{
		/// <summary>
		/// 是否主库优先
		/// </summary>
		bool SecondaryFirst { get; }

		/// <summary>
		/// 获取数据库执行对象
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <returns></returns>
		ICandyDbExecute GetExecute<TDbName>() where TDbName : struct, ICandyDbName;

		/// <summary>
		/// 获取数据库执行对象
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		ICandyDbExecute GetExecute(string name);

		/// <summary>
		/// 事务
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <param name="action"></param>
		void Transaction<TDbName>(Action<ICandyDbExecute> action) where TDbName : struct, ICandyDbName;

		/// <summary>
		/// 事务
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask TransactionAsync<TDbName>(Action<ICandyDbExecute> action, CancellationToken cancellationToken = default) where TDbName : struct, ICandyDbName;
	}
}
