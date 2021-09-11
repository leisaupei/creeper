using Creeper.Driver;
using Creeper.Sqlite.Test.Entity.Options;
using Creeper.xUnitTest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Creeper.xUnitTest.Sqlite
{
	public class BaseTest
	{

		public const string MainConnectionString = "data source=../../../../../sql/sqlitedemo.db";

		protected ITestOutputHelper Output { get; }
		protected static ICreeperContext Context => _context.Value;

		static readonly Lazy<ICreeperContext> _context = new(() =>
		{
			var services = new ServiceCollection();
			services.AddCreeper(options =>
			{
				options.AddSqliteContext<SqliteContext>(a =>
				{
					a.UseCache<HashtableTestDbCache>();
					a.UseStrategy(Generic.DataBaseTypeStrategy.OnlyMain);
					a.UseConnectionString(MainConnectionString);
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
