using Creeper.Driver;
using Creeper.Generic;
using System;

namespace Creeper.Sqlite.Test.Entity.Options
{
	public class SqliteContext : CreeperContextBase
	{
		public SqliteContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
	}

}
