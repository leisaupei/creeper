using System;
using Creeper.Driver;
using Creeper.SqlServer;
using Creeper.SqlServer.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperSqlServerExtensions
	{
		/// <summary>
		/// 添加SqlServer数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddSqlServerContext<TContext>(this CreeperOptions option, Action<CreeperContextOptions> action) where TContext : class, ICreeperContext
		{
			if (action is null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			option.AddContext<TContext>(action);
			option.AddConverter<TContext, SqlServerConverter>();
			option.AddExtension(new CreeperSqlServerOptionsExtension());
		}

	}
}
