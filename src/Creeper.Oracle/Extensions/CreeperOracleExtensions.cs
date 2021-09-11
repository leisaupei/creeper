using System;
using Creeper.Driver;
using Creeper.Oracle.Extensions;
using Creeper.Oracle;
using Creeper.Generic;
using System.Reflection;
using Creeper;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperOracleExtensions
	{
		/// <summary>
		/// 添加oracle数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddOracleContext<TContext>(this CreeperOptions option, Action<CreeperOracleContextOptions> action) where TContext : class, ICreeperContext
		{
			if (action is null)
			{
				throw new ArgumentNullException(nameof(action));
			}
			var oracleOptions = new CreeperOracleContextOptions();
			action(oracleOptions);
			//Oracle字段名区分大小写, 此步骤不能省略
			if (oracleOptions.ColumnNameStyle == ColumnNameStyle.None)
				throw new CreeperException("请使用SetColumnNameStyle(ColumnNameStyle)描述数据库字段命名规范");

			var contextConfigure = ConfigureHelper.Copy<CreeperOracleContextOptions, CreeperContextOptions>(action);

			option.AddContext<TContext>(contextConfigure);
			option.AddConverter<TContext, OracleConverter>(new OracleConverter() { ColumnNameStyle = oracleOptions.ColumnNameStyle });
			option.AddExtension(new CreeperOracleOptionsExtension());
		}

		/// <summary>
		/// Oracle字段区分大小写，表字段命名规范需要统一，需要配置数据库字段的命名规范，
		/// </summary>
		/// <param name="option"></param>
		/// <param name="columnNameStyle"></param>
		public static void SetColumnNameStyle(this CreeperOracleContextOptions option, ColumnNameStyle columnNameStyle)
		{
			option.ColumnNameStyle = columnNameStyle;
		}
	}
}
