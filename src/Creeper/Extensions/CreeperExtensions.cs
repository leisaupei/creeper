using Creeper.Utils;
using Creeper.Driver;
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
		public static IServiceCollection AddCreeper(this IServiceCollection services, Action<CreeperOptions> optionsAction)
		{
			var options = new CreeperOptions();
			optionsAction(options);

			//添加DbConverterFactory
			services.TryAddSingleton(options.ConverterFactory);

			foreach (var serviceExtension in options.Extensions)
				serviceExtension.AddServices(services);


			return services;
		}
	}
}
