using Creeper.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public interface ICreeperDbExecute : ICreeperDbComponent, IDisposable
	{
		ICreeperDbConnectionOption ConnectionOptions { get; }

		/// <summary>
		/// 提交事务
		/// </summary>
		void CommitTransaction();

		/// <summary>
		/// 提交事务
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask CommitTransactionAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 回滚事务
		/// </summary>
		void RollBackTransaction();

		/// <summary>
		/// 回滚事务
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask RollBackTransactionAsync(CancellationToken cancellationToken = default);
	}
}
