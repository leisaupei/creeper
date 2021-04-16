using System;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql;
using Creeper.PostgreSql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperOptionsExtensions
	{
		/// <summary>
		/// 添加PostgreSql数据库配置
		/// </summary>
		/// <typeparam name="TDbMainName">主库名称泛型</typeparam>
		/// <typeparam name="TDbSecondaryName">从库名称泛型</typeparam>
		/// <param name="options"></param>
		/// <param name="postgreSqlDbOption">postgresql连接配置, db层目录下/Options/PostgreSqlDbOptions.cs</param>
		/// <returns></returns>
		public static CreeperOptions AddPostgreSql<TDbMainName, TDbSecondaryName>(this CreeperOptions options, BasePostgreSqlDbOption<TDbMainName, TDbSecondaryName> postgreSqlDbOption)
			where TDbMainName : struct, ICreeperDbName
			where TDbSecondaryName : struct, ICreeperDbName
		{
			options.AddPostgreSqlOptions();
			options.AddOption(postgreSqlDbOption);
			return options;
		}
		/// <summary>
		/// PostgreSql选项配置
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static CreeperOptions AddPostgreSqlOptions(this CreeperOptions options)
		{
			options.TryAddDbTypeConvert<PostgreSqlTypeConverter>();
			options.RegisterExtension(new PostgreSqlCreeperDbOptionsExtensions());
			return options;
		}
	}
}
