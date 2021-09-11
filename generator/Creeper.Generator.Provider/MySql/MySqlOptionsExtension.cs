using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generator.Provider.MySql;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class MySqlOptionsExtension
	{
		public static CreeperGeneratorOptions UseMySqlRules(this CreeperGeneratorOptions options, Action<GenerateRules> action)
		{
			options.AddExtension(new MySqlGeneratorExtension(action));
			return options;
		}

	}
}
