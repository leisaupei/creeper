using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Creeper.DBHelper
{
	public class SqlHelper
	{
		public static string GetNullSql(string sql, string key)
		{
			var equalsReg = new Regex(@"=\s*" + key);
			var notEqualsReg = new Regex(@"(!=|<>)\s*" + key);
			if (notEqualsReg.IsMatch(sql))
				return notEqualsReg.Replace(sql, " IS NOT NULL");
			else if (equalsReg.IsMatch(sql))
				return equalsReg.Replace(sql, " IS NULL");
			else
				return sql;
		}
	}
}
