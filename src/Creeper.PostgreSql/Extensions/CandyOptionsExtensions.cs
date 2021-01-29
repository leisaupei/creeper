using System;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql;
using Creeper.PostgreSql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperOptionsExtensions
	{
		public static CreeperOptions AddPostgreSql(this CreeperOptions options, ICreeperDbOption CreeperDbOption)
		{
			options.AddPostgreSqlOptions();
			options.AddOption(CreeperDbOption);
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
