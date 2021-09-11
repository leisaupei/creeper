using Creeper.Driver;
using Creeper.Generic;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace Creeper.Generator.Provider.SqlServer
{
	internal class SqlServerGeneratorConverter : CreeperConverter
	{

		public override DataBaseKind DataBaseKind => DataBaseKind.SqlServer;

		protected override DbParameter GetDbParameter(string name, object value)
			=> new SqlParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
			=> new SqlConnection(connectionString);
	}
}
