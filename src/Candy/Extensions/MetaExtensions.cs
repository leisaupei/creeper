using Candy.Common;
using Candy.DbHelper;
using Candy.Model;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Candy.Extensions
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
