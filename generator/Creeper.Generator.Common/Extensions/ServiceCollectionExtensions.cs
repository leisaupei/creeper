using Creeper.Generator.Common;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCreeperGenerator(this IServiceCollection services, Action<CreeperGeneratorOptions> action)
		{
			services.TryAddSingleton<ICreeperGeneratorProviderFactory, CreeperGeneratorProviderFactory>();

			services.TryAddSingleton<ICreeperGenerator, CreeperGenerator>();

			var options = new CreeperGeneratorOptions();
			action(options);

			foreach (var extension in options.Extensions)
			{
				extension.AddServices(services);
			}

			return services;
		}
	}
}
