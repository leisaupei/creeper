using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generic;
using Creeper.Sqlite;
using Creeper.Sqlite.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Creeper.Generator.Provider.Sqlite
{
	public class SqliteGeneratorExtension : ICreeperOptionsExtension
	{
		private readonly Action<GenerateRules> _generatorRulesConfigure;

		public SqliteGeneratorExtension(Action<GenerateRules> generatorRulesConfigure)
		{
			_generatorRulesConfigure = generatorRulesConfigure;
		}

		public void AddServices(IServiceCollection services)
		{
			services.Configure(DataBaseKind.Sqlite.ToString(), _generatorRulesConfigure);
			services.AddSingleton<ICreeperGeneratorProvider, SqliteGeneratorProvider>(serviceProvider => new SqliteGeneratorProvider());
		}

	}

}
