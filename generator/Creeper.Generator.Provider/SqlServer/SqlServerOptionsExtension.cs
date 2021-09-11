using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generator.Provider.SqlServer;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class SqlServerOptionsExtension
	{
		public static CreeperGeneratorOptions UseSqlServerRules(this CreeperGeneratorOptions options, Action<GenerateRules> action)
		{
			options.AddExtension(new SqlServerGeneratorExtension(action));
			return options;
		}

	}

}
