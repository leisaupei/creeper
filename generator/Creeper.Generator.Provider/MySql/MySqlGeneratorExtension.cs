using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Provider.MySql
{
	public class MySqlGeneratorExtension : ICreeperOptionsExtension
	{
		private readonly Action<GenerateRules> _generatorRulesConfigure;

		public MySqlGeneratorExtension(Action<GenerateRules> generatorRulesConfigure)
		{
			_generatorRulesConfigure = generatorRulesConfigure;
		}

		public void AddServices(IServiceCollection services)
		{
			services.Configure(DataBaseKind.MySql.ToString(), _generatorRulesConfigure);
			services.AddSingleton<ICreeperGeneratorProvider, MySqlGeneratorProvider>(a => new MySqlGeneratorProvider());
		}

	}

}
