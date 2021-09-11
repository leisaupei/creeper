using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common.Models
{
	public class GenerateRules
	{
		public Excepts Excepts { get; set; } = new Excepts();
		public FieldIgnore FieldIgnore { get; set; } = new FieldIgnore();
	}
	public class Excepts
	{
		public ExceptsGlobal Global { get; set; } = new ExceptsGlobal();
		public Dictionary<string, ExceptsGlobal> Customs { get; set; } = new Dictionary<string, ExceptsGlobal>();
	}
}
