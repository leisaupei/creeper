using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generic;
using Creeper.SqlServer;
using Creeper.SqlServer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Creeper.Generator.Provider.SqlServer
{
	public class SqlServerGeneratorExtension : ICreeperOptionsExtension
	{
		private readonly Action<GenerateRules> _generatorRulesConfigure;

		public SqlServerGeneratorExtension(Action<GenerateRules> generatorRulesConfigure)
		{
			_generatorRulesConfigure = generatorRulesConfigure;
		}

		public void AddServices(IServiceCollection services)
		{
			services.Configure(DataBaseKind.SqlServer.ToString(), _generatorRulesConfigure);
			services.AddSingleton<ICreeperGeneratorProvider, SqlServerGeneratorProvider>(serviceProvider => new SqlServerGeneratorProvider());
		}

	}

}
