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

namespace Creeper.xUnitTest.Access.v2003
{
	public class BaseTest
	{

		public const string MainConnectionString = "Provider=Microsoft.Jet.OleDb.4.0;data source=../../../../../sql/access2003.mdb;Jet OLEDB:Database Password=123456";

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
		public class AccessContext : CreeperContextBase
		{
			public AccessContext(IServiceProvider serviceProvider) : base(serviceProvider) { }

		}
	}

}
