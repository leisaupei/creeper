using Creeper.Driver;
using Creeper.Oracle.Test.Entity.Options;
using Creeper.Sqlite.Test.Entity.Options;
using Creeper.xUnitTest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Creeper.Generic;

namespace Creeper.xUnitTest.Oracle
{
	public class BaseTest
	{

		public const string MainConnectionString = "user id=CREEPER;password=123456;data source=//192.168.1.15:1521/ORCLPDB1;Pooling=true;Max Pool Size=10";

		protected ITestOutputHelper Output { get; }
		protected static ICreeperContext Context => _context.Value;

		static readonly Lazy<ICreeperContext> _context = new(() =>
		{
			var services = new ServiceCollection();
			services.AddCreeper(options =>
			{
				options.AddOracleContext<OracleContext>(a =>
				{
					a.UseCache<HashtableTestDbCache>();
					a.UseStrategy(DataBaseTypeStrategy.OnlyMain);
					a.UseConnectionString(MainConnectionString);
					a.SetColumnNameStyle(ColumnNameStyle.Pascal);
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
