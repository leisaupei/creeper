using Candy.Driver.Common;
using Candy.Driver.DbHelper;
using Candy.Driver.Model;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Candy.Driver.Extensions
{
	public static class CandyExtensions
	{
		public static IServiceCollection AddCandyDbContext(this IServiceCollection services, Action<CandyOptions> options)
		{
			services.AddOptions();
			services.Configure(options);
			services.AddSingleton<ICandyDbContext, DbContext>();
			return services;
		}

	}
}
