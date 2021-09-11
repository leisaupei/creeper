using Creeper.Driver;
using Creeper.Generic;
using System;

namespace Creeper.SqlServer.Test.Entity.Options
{
	public class SqlServerContext : CreeperContextBase
	{
		public SqlServerContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
	}

}
