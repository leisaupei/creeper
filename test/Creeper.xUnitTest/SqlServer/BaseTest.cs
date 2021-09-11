using Creeper.Driver;
using Creeper.Generic;
using Creeper.SqlServer.Test.Entity.Options;
using Creeper.xUnitTest.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Creeper.xUnitTest.SqlServer
{
	public class BaseTest
	{

		public const string TestMainConnectionString = "server=.;database=demo;uid=sa;pwd=123456;Max Pool Size=512;";

		public const string TestSecondaryConnectionString = "server=.;database=demo;uid=sa;pwd=123456;Max Pool Size=512;";

		protected ITestOutputHelper Output { get; }
		protected static ICreeperContext Context => _context.Value;

		static readonly Lazy<ICreeperContext> _context = new(() =>
		{
			var services = new ServiceCollection();
			services.AddCreeper(options =>
			{
				options.AddSqlServerContext<SqlServerContext>(a =>
				{
					a.UseCache<HashtableTestDbCache>();
					a.UseStrategy(DataBaseTypeStrategy.OnlyMain);
					a.UseConnectionString(TestMainConnectionString);
					a.UseSecondaryConnectionString(TestSecondaryConnectionString);
				});
			});
			var serviceProvider = services.BuildServiceProvider();
			return serviceProvider.GetService<ICreeperContext>();
		});
		public BaseTest()
		{
			
		}

		public BaseTest(ITestOutputHelper output) : this()
		{
			Output = output;
		}
	}

}
