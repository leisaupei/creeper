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

namespace Creeper.Generator.Provider.Access
{
	public class AccessGeneratorExtension : ICreeperOptionsExtension
	{
		private readonly Action<GenerateRules> _generatorRulesConfigure;

		public AccessGeneratorExtension(Action<GenerateRules> generatorRulesConfigure)
		{
			_generatorRulesConfigure = generatorRulesConfigure;
		}

		public void AddServices(IServiceCollection services)
		{
			services.Configure(DataBaseKind.Access.ToString(), _generatorRulesConfigure);
			services.AddSingleton<ICreeperGeneratorProvider, AccessGeneratorProvider>(serviceProvider => new AccessGeneratorProvider());
		}

	}

}
