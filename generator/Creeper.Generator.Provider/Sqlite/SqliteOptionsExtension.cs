using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generator.Provider.Sqlite;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class SqliteOptionsExtension
	{
		public static CreeperGeneratorOptions UseSqliteRules(this CreeperGeneratorOptions options, Action<GenerateRules> action)
		{
			options.AddExtension(new SqliteGeneratorExtension(action));
			return options;
		}

	}

}
