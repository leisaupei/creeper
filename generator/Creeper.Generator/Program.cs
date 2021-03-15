/* ##########################################################
 * #        .net standard 2.1 + data base Code Maker        #
 * #                author by leisaupei                     #
 * #          https://github.com/leisaupei/creeper          #
 * ##########################################################
 */
using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generic;
using Creeper.PostgreSql;
using Creeper.PostgreSql.Generator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
namespace Creeper.Generator
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			Console.OutputEncoding = Encoding.GetEncoding("UTF-8");
			Console.InputEncoding = Encoding.GetEncoding("UTF-8");

			Console.WriteLine(@"
##########################################################
#        .net standard 2.1 + data base Code Maker        #
#                author by leisaupei                     #
#           https://github.com/leisaupei/creeper         #
##########################################################
> Parameters description:
	-o output path
	-p project name
	-s create .sln file, *optional(t/f) default: f.
	--b build options, arguments must be at the end
		host	host
		port	port
		user	username
		pwd		password
		db		database name
		type	database enum type name, 'main' if only one. *optional
		dbtype	postgres/mysql at presents
> Single Example: -o d:\workspace\test -p SimpleTest -s t --b host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;name=main;dbtype=postgresql

> Multiple Example: -o d:\workspace\test -p SimpleTest -s t --b host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;name=main;dbtype=postgresql host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;name=main;dbtype=postgresql
");
			IConfiguration cfg = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, false).Build();

			IServiceCollection services = new ServiceCollection();
			services.AddSingleton(cfg);
			//postgresql 
			services.Configure<PostgresExcepts>(cfg.GetSection("GenerateRules:PostgresqlRules:Excepts"));
			services.AddSingleton<ICreeperGeneratorProvider, PostgerSqlGeneratorProvider>();
			services.TryAddSingleton<CreeperGeneratorProviderFactory>();

			services.TryAddSingleton<ICreeperGenerator, CreeperGenerator>();

			services.AddCreeperDbContext(option =>
			{
				option.AddPostgreSqlOptions();
			});
			var serviceProvider = services.BuildServiceProvider();

			var generatorFactory = serviceProvider.GetService<CreeperGeneratorProviderFactory>();
			var creeperGenerator = serviceProvider.GetService<ICreeperGenerator>();
			var creeperDbContext = serviceProvider.GetService<ICreeperDbContext>();
			if (args?.Length > 0)
			{
				CreeperGenerateBuilder model = new CreeperGenerateBuilder();
				for (int i = 0; i < args.Length; i += 2)
				{
					//host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;type=name;dbtype=postgresql
					var finish = false;
					switch (args[i].ToLower())
					{
						case "-o": model.OutputPath = args[i + 1]; break;
						case "-p": model.ProjectName = args[i + 1]; break;
						case "-s": model.Sln = args[i + 1].ToLower() == "t"; break;
						case "--b":
							var builds = args.ToList().GetRange(i + 1, args.Length - i - 1);
							foreach (var build in builds)
							{
								var ps = build.Split(';');
								var types = ps.FirstOrDefault(a => a.Contains("type="));
								if (types == null)
									throw new ArgumentException("choose one of ", string.Join(",", Enum.GetNames(typeof(DataBaseKind))));
								var kindStr = types.Split('=')[1];

								var kind = Enum.TryParse<DataBaseKind>(kindStr, ignoreCase: true, out var value)
									? value : throw new ArgumentException("choose one of ", string.Join(",", Enum.GetNames(typeof(DataBaseKind))));

								model.Connections.Add(generatorFactory[kind].GetDbConnectionOptionFromString(build));
							}
							finish = true;
							break;
						default:
							break;
					}
					if (finish) break;
				}
				creeperGenerator.Gen(model);
				Console.WriteLine("successful...");
			}
			Console.ReadKey();
		}
	}
}
