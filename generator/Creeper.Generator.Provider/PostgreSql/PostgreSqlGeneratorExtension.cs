using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generic;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Creeper.Generator.Provider.PostgreSql
{
	public class PostgreSqlGeneratorExtension : ICreeperOptionsExtension
	{
		private readonly Action<GenerateRules> _generatorRulesConfigure;

		public PostgreSqlGeneratorExtension(Action<GenerateRules> generatorRulesConfigure)
		{
			_generatorRulesConfigure = generatorRulesConfigure;
		}

		public void AddServices(IServiceCollection services)
		{

			services.Configure(DataBaseKind.PostgreSql.ToString(), _generatorRulesConfigure);
			services.AddSingleton<ICreeperGeneratorProvider, PostgreSqlGeneratorProvider>(serviceProvider => new PostgreSqlGeneratorProvider());
		}

	}

}
