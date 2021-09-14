using System;
using System.Reflection;
using Creeper.Driver;
using Creeper.MySql;
using Creeper.MySql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperMySqlExtensions
	{
		/// <summary>
		/// 添加MySql数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <remarks>
		/// 1. 使用Mysql数据库连接字符串时, 默认添加Allow User Variables=True参数, 以使用Update Returning
		/// </remarks>
		/// <returns></returns>
		public static void AddMySqlContext<TContext>(this CreeperOptions option, Action<CreeperMySqlContextOptions> action) where TContext : class, ICreeperContext
		{
			if (action is null)
			{
				throw new ArgumentNullException(nameof(action));
			}
			var mysqlOptions = new CreeperMySqlContextOptions();
			action(mysqlOptions);

			var contextConfigure = ConfigureHelper.Copy<CreeperMySqlContextOptions, CreeperContextOptions>(action);
			option.AddContext<TContext>(contextConfigure);
			option.AddConverter<TContext, MySqlConverter>(new MySqlConverter { UseGeometryType = mysqlOptions.UseGeometryType });
			option.AddExtension(new CreeperMySqlOptionsExtension());
		}

		/// <summary>
		/// 因Mysql空间数据为自定义类型, 所以放置控制开关, 默认是false
		/// </summary>
		/// <param name="option"></param>
		public static void UseGeometry(this CreeperMySqlContextOptions option)
		{
			option.UseGeometryType = true;
		}

	}
}
