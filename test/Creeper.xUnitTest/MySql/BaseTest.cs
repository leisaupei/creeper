using Creeper.Driver;
using Creeper.MySql.Test.Entity.Options;
using Creeper.xUnitTest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Creeper.xUnitTest.MySql
{
	public class BaseTest
	{

		public const string TestMainConnectionString = "server=192.168.1.15;userid=root;pwd=123456;port=3306;database=demo;sslmode=none;";

		public const string TestSecondaryConnectionString = "server=192.168.1.15;userid=root;pwd=123456;port=3306;database=demo;sslmode=none;";

		protected ITestOutputHelper Output { get; }

		protected static ICreeperContext Context => _context.Value;

		static readonly Lazy<ICreeperContext> _context = new(() =>
		{
			var services = new ServiceCollection(); services.AddCreeper(options =>
			{
				options.AddMySqlContext<MySqlContext>(a =>
				{
					a.UseCache<HashtableTestDbCache>();
					a.UseConnectionString(TestMainConnectionString);
					a.UseSecondaryConnectionString(TestSecondaryConnectionString);
					a.UseMySqlGeometry();
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
