using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Xunit.Abstractions;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.xUnitTest.Extensions;
using Creeper.PostgreSql.Test.Entity.Options;

namespace Creeper.xUnitTest.PostgreSql
{
	public class BaseTest
	{
		public const string TestMainConnectionString = "host=192.168.1.15;port=5432;username=postgres;password=123456;database=postgres;maximum pool size=100;pooling=true;Timeout=10;CommandTimeout=10;";

		public const string TestSecondaryConnectionString = "host=192.168.1.15;port=5432;username=postgres;password=123456;database=postgres;maximum pool size=100;pooling=true;Timeout=10;CommandTimeout=20;";
		public static readonly Guid StuPeopleId1 = Guid.Parse("da58b577-414f-4875-a890-f11881ce6341");

		public static readonly Guid StuPeopleId2 = Guid.Parse("5ef5a598-e4a1-47b3-919e-4cc1fdd97757");
		public static readonly Guid GradeId = Guid.Parse("81d58ab2-4fc6-425a-bc51-d1d73bf9f4b1");

		public static readonly string StuNo1 = "1333333";
		public static readonly string StuNo2 = "1333334";

		protected ITestOutputHelper Output { get; }

		protected static ICreeperContext Context => _context.Value;

		static readonly Lazy<ICreeperContext> _context = new(() =>
		{
			var services = new ServiceCollection();
			services.AddCreeper(options =>
			{
				options.AddPostgreSqlContext<PostgreSqlContext>(t =>
				{
					t.UseCache<HashtableTestDbCache>();
					t.UseStrategy(DataBaseTypeStrategy.OnlyMain);
					t.UseConnectionString(TestMainConnectionString);
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
