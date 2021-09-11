using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generator.Provider.Oracle;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class OracleOptionsExtension
	{
		public static CreeperGeneratorOptions UseOracleRules(this CreeperGeneratorOptions options, Action<GenerateRules> action)
		{
			options.AddExtension(new OracleGeneratorExtension(action));
			return options;
		}

	}

}
