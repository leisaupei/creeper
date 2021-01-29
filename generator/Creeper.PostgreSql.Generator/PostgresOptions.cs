using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.PostgreSql.Generator
{
	public class PostgresExcepts
	{
		public PostgresExceptsGlobal Global { get; set; } = new PostgresExceptsGlobal();
		public Dictionary<string, PostgresExceptsGlobal> Customs { get; set; } = new Dictionary<string, PostgresExceptsGlobal>();
	}
	public class PostgresExceptsGlobal
	{
		public string[] Schemas { get; set; } = new string[0];
		public string[] Tables { get; set; } = new string[0];
		public string[] Views { get; set; } = new string[0];
		public string[] Composites { get; set; } = new string[0];
	}
}
