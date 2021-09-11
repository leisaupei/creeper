using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common.Models
{
	public class ExceptsGlobal
	{
		public string[] Tables { get; set; } = new string[0];
		public string[] Views { get; set; } = new string[0];
		public string[] Schemas { get; set; } = new string[0];
		public string[] Composites { get; set; } = new string[0];
	}
}
