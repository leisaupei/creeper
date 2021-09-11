using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generator.Provider.PostgreSql;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class PostgreSqlOptionsExtension
	{

		public static CreeperGeneratorOptions UsePostgreSqlRules(this CreeperGeneratorOptions options, Action<GenerateRules> action)
		{
			options.AddExtension(new PostgreSqlGeneratorExtension(action));
			return options;
		}
	}

}
