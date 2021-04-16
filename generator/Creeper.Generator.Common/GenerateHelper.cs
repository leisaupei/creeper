using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common
{
	public class GenerateHelper
	{
		public static string ExceptConvert(string column, string[] excepts)
		{
			var exceptPattern = new List<string>();
			var exceptEqual = new List<string>();
			Array.ForEach(excepts, e =>
			{
				if (e.Contains('%'))
					exceptPattern.Add($"lower({column}) NOT LIKE '{e.ToLower()}'");
				else if (e.Contains('*'))
					exceptPattern.Add($"lower({column}) NOT LIKE '{e.Replace('*', '%').ToLower()}'");
				else
					exceptEqual.Add($"'{e.ToLower()}'");
			});
			var wheres = new List<string>();
			if (exceptEqual.Count > 0)
				wheres.Add($"lower({column}) NOT IN ({string.Join(", ", exceptEqual)})");
			if (exceptPattern.Count > 0)
				wheres.Add(string.Join(" AND ", exceptPattern));

			if (wheres.Count > 0)
				return $"({string.Join(" AND ", wheres)})";
			else
				return " 1=1 ";
		}
	}
}
