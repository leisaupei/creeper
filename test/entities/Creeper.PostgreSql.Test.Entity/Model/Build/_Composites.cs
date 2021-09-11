using System;
using Newtonsoft.Json;

namespace Creeper.PostgreSql.Test.Entity.Model
{
	public partial struct CreeperInfo
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}
