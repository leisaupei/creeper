using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Creeper.Generator
{
	public class Generator
	{
		public static void Gen(string generateString)
		{
			Gen(generateString.Split(' '));
		}

		public static void Gen(string[] commandArgs)
		{
			var serviceProvider = BuildServiceProvider();
			var generatorFactory = serviceProvider.GetService<ICreeperGeneratorProviderFactory>();
			var option = new CreeperGenerateOption();
			for (int i = 0; i < commandArgs.Length; i += 2)
			{
				//host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;type=name;type=postgresql
				var finish = false;
				switch (commandArgs[i].ToLower())
				{
					case "-o": option.OutputPath = commandArgs[i + 1]; break;
					case "-p": option.ProjectName = commandArgs[i + 1]; break;
					case "-s": option.Sln = commandArgs[i + 1].ToLower() == "t"; break;
					case "--b":
						var builds = commandArgs[(i + 1)..];
						foreach (var build in builds)
						{
							var ps = build.Split(';');
							var type = ps.FirstOrDefault(a => a.StartsWith("type="));

							if (type == null || !Enum.TryParse<DataBaseKind>(type.Split('=')[1], true, out var kind))
								throw new ArgumentException("choose one of ", string.Join(",", Enum.GetNames<DataBaseKind>()));

							option.Builders.Add(generatorFactory[kind].GetDbConnectionOptionFromString(build));
						}
						finish = true;
						break;
				}
				if (finish) break;
			}

			if (option.Builders.Count > 1 && option.Builders.Any(a => string.IsNullOrWhiteSpace(a.Name)))
				throw new ArgumentNullException(nameof(CreeperGenerateConnection.Name), "如果需要生成多个数据库, 请声明'name'参数");

			if (option.Builders.GroupBy(a => a.Name.ToLower()).Any(a => a.Count() > 1))
				throw new ArgumentException($"包含{nameof(CreeperGenerateConnection.Name)}相同的参数");

			var creeperGenerator = serviceProvider.GetService<ICreeperGenerator>();
			creeperGenerator.Generate(option);
		}

		private static ServiceProvider BuildServiceProvider()
		{
			IConfiguration cfg = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, false)
				.Build();

			IServiceCollection services = new ServiceCollection();

			services.AddSingleton(cfg);

			//postgresql 
			var postgreSqlRules = cfg.GetSection("GenerateRules:PostgreSqlRules").Get<GenerateRules>();
			//mysql
			var mySqlRules = cfg.GetSection("GenerateRules:MySqlRules").Get<GenerateRules>();

			services.AddCreeperGenerator(option =>
			{
				option.UseMySqlRules(o => Copy(mySqlRules, o));
				option.UsePostgreSqlRules(o => Copy(postgreSqlRules, o));
				option.UseSqlServerRules(o => { });
				option.UseSqliteRules(o => { });
				option.UseAccessRules(o => { });
				option.UseOracleRules(o => { });

			});
			var serviceProvider = services.BuildServiceProvider();
			return serviceProvider;
		}

		private static TTo Copy<TFrom, TTo>(TFrom fromValue) where TTo : new()
		{
			var toValue = Activator.CreateInstance<TTo>();
			Copy(fromValue, toValue);
			return toValue;
		}
		private static void Copy<TFrom, TTo>(TFrom fromValue, TTo toValue) where TTo : new()
		{
			if (fromValue is null)
			{
				throw new ArgumentNullException(nameof(fromValue));
			}

			var toProperties = typeof(TTo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var fromType = typeof(TFrom);

			foreach (var toProperty in toProperties)
			{
				var fromProperty = fromType.GetProperty(toProperty.Name, BindingFlags.Public | BindingFlags.Instance);
				if (fromProperty.PropertyType == toProperty.PropertyType)
					fromProperty.SetValue(toValue, fromProperty.GetValue(fromValue));
			}
		}
		private static Action<TTo> Copy<TFrom, TTo>(Action<TFrom> from) where TTo : new()
		{
			if (from is null)
			{
				throw new ArgumentNullException(nameof(from));
			}

			var fromValue = Activator.CreateInstance<TFrom>();
			from.Invoke(fromValue);

			return new Action<TTo>(toValue => Copy(fromValue, toValue));
		}
	}
}
