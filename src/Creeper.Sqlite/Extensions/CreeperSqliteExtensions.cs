using System;
using Creeper.Driver;
using Creeper.Sqlite;
using Creeper.Sqlite.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperSqliteExtensions
	{
		/// <summary>
		/// 添加sqlite数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddSqliteContext<TContext>(this CreeperOptions option, Action<CreeperContextOptions> action) where TContext : class, ICreeperContext
		{
			if (action is null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			option.AddContext<TContext>(action);
			option.AddConverter<TContext, SqliteConverter>();
			option.AddExtension(new CreeperSqliteOptionsExtension());
		}
	}
}
