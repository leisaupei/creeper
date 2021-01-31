using System;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql;
using Creeper.PostgreSql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperOptionsExtensions
	{
		public static CreeperOptions AddPostgreSql<TDbMainName, TDbSecondaryName>(this CreeperOptions options, BasePostgreSqlDbOption<TDbMainName, TDbSecondaryName> postgreSqlDbOption)
			where TDbMainName : struct, ICreeperDbName
			where TDbSecondaryName : struct, ICreeperDbName
		{
			options.AddPostgreSqlOptions();
			options.AddOption(postgreSqlDbOption);
			return options;
		}
		public static CreeperOptions AddPostgreSqlOptions(this CreeperOptions options)
		{
			options.TryAddDbTypeConvert<PostgreSqlTypeConverter>();
			options.RegisterExtension(new PostgreSqlCreeperOptionsExtensions());
			return options;
		}
	}
}
