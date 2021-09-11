using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Creeper.Generator.Provider.Oracle
{
	public class OracleGeneratorExtension : ICreeperOptionsExtension
	{
		private readonly Action<GenerateRules> _generatorRulesConfigure;

		public OracleGeneratorExtension(Action<GenerateRules> generatorRulesConfigure)
		{
			_generatorRulesConfigure = generatorRulesConfigure;
		}

		public void AddServices(IServiceCollection services)
		{
			services.Configure(DataBaseKind.Oracle.ToString(), _generatorRulesConfigure);
			services.AddSingleton<ICreeperGeneratorProvider, OracleGeneratorProvider>(serviceProvider => new OracleGeneratorProvider());
		}

	}

}
