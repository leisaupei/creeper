using Creeper.Driver;
using Creeper.Generic;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Creeper.Generator.Provider.MySql
{
	internal class MySqlGeneratorConverter : CreeperConverter
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.MySql;

		protected override DbParameter GetDbParameter(string name, object value)
			=> new MySqlParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		{
			var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
			connectionStringBuilder.AllowUserVariables = true;

			var connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
			return connection;
		}
	}
}
