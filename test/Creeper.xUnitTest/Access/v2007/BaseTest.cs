using Creeper.Access2007.Test.Entity.Options;
using Creeper.Driver;
using Creeper.Sqlite.Test.Entity.Options;
using Creeper.xUnitTest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Creeper.xUnitTest.Access.v2007
{
	public class BaseTest
	{
		public const string MainConnectionString = "data source=../../../../../sql/access.accdb;Jet OLEDB:Database Password=123456;Provider=Microsoft.ACE.OLEDB.12.0;";

		protected ITestOutputHelper Output { get; }

		protected static ICreeperContext Context => _context.Value;

		static readonly Lazy<ICreeperContext> _context = new(() =>
		{
			var services = new ServiceCollection();
			services.AddCreeper(options =>
			{
				options.AddAccessContext<AccessContext>(a =>
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
