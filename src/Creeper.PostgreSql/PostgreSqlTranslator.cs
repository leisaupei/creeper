using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.PostgreSql
{
	public class PostgreSqlTranslator
	{
		private static NpgsqlNameTranslator _instance = null;

		public static NpgsqlNameTranslator Instance
		{
			get
			{
				if (_instance != null) return _instance;
				_instance = new NpgsqlNameTranslator();
				return _instance;
			}
		}
		public class NpgsqlNameTranslator : INpgsqlNameTranslator
		{
			public string TranslateMemberName(string clrName) => clrName;
			public string TranslateTypeName(string clrName) => clrName;
		}
	}
}
