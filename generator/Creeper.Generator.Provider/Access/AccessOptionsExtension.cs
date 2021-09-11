using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generator.Provider.Access;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AccessOptionsExtension
	{
		public static CreeperGeneratorOptions UseAccessRules(this CreeperGeneratorOptions options, Action<GenerateRules> action)
		{
			options.AddExtension(new AccessGeneratorExtension(action));
			return options;
		}

	}

}
