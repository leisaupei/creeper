using Creeper.Driver;
using Creeper.Generic;
using System.Data.Common;
using System.Data.SQLite;

namespace Creeper.Generator.Provider.Sqlite
{
	internal class SqliteGeneratorConverter : CreeperConverter
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.Sqlite;

		protected override DbParameter GetDbParameter(string name, object value)
			=> new SQLiteParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		=> new SQLiteConnection(connectionString);
	}

}
