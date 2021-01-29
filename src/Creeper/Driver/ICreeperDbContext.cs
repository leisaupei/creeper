using System;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public interface ICreeperDbContext : ICreeperDbComponent
	{
		/// <summary>
		/// 获取数据库执行对象
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <returns></returns>
		ICreeperDbExecute GetExecute<TDbName>() where TDbName : struct, ICreeperDbName;

		/// <summary>
		/// 获取数据库执行对象
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		ICreeperDbExecute GetExecute(string name);

		/// <summary>
		/// 事务
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <param name="action"></param>
		void Transaction<TDbName>(Action<ICreeperDbExecute> action) where TDbName : struct, ICreeperDbName;

		/// <summary>
		/// 事务
		/// </summary>
		/// <typeparam name="TDbName"></typeparam>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask TransactionAsync<TDbName>(Action<ICreeperDbExecute> action, CancellationToken cancellationToken = default) where TDbName : struct, ICreeperDbName;

	}
}
