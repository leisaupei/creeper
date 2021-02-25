using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperExtensions
	{
		/// <summary>
		/// 注册db context服务注册
		/// </summary>
		/// <param name="services"></param>
		/// <param name="optionsAction"></param>
		/// <returns></returns>
		public static IServiceCollection AddCreeperDbContext(this IServiceCollection services, Action<CreeperOptions> optionsAction)
		{
			var options = new CreeperOptions();
			optionsAction(options);
			foreach (var serviceExtension in options.Extensions)
				serviceExtension.AddServices(services);

			if (options.DbCacheType != null)
				services.TryAddSingleton(typeof(ICreeperDbCache), options.DbCacheType);

			services.Configure(optionsAction);
			services.TryAddSingleton<ICreeperDbContext, CreeperDbContext>();
			return services;
		}

	}
}
