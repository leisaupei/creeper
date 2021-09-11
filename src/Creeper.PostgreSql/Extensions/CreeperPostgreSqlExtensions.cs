using System;
using Creeper.Driver;
using Creeper.PostgreSql;
using Creeper.PostgreSql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperPostgreSqlExtensions
	{
		/// <summary>
		/// 添加PostgreSql数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddPostgreSqlContext<TContext>(this CreeperOptions option, Action<CreeperContextOptions> action) where TContext : class, ICreeperContext
		{
			if (action is null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			option.AddContext<TContext>(action);
			option.AddConverter<TContext, PostgreSqlConverter>();
			option.AddExtension(new CreeperPostgreSqlOptionsExtensions());
		}
	}
}
