using Creeper.Driver;
using Creeper.Generic;
using System.Data.Common;
using System.Data.OleDb;

namespace Creeper.Generator.Provider.Access
{
	internal class AccessGeneratorConverter : CreeperConverter
	{

		public override DataBaseKind DataBaseKind => DataBaseKind.Access;

		protected override DbParameter GetDbParameter(string name, object value)
			=> new OleDbParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		=> new OleDbConnection(connectionString);

	}
}
