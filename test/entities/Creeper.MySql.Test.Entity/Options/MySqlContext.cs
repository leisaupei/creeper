using Creeper.Driver;
using System;

namespace Creeper.MySql.Test.Entity.Options
{
	public class MySqlContext : CreeperContextBase
	{
		public MySqlContext(IServiceProvider serviceProvider) : base(serviceProvider) { }
	}

}
