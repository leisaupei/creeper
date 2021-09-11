using Creeper.Driver;
using Creeper.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Data.OleDb;

namespace Creeper.Generator.Provider.Oracle
{
	internal class OracleGeneratorConverter : CreeperConverter
	{

		public override DataBaseKind DataBaseKind => DataBaseKind.Oracle;

		protected override DbParameter GetDbParameter(string name, object value)
			=> new OracleParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		=> new OracleConnection(connectionString);

	}
}
