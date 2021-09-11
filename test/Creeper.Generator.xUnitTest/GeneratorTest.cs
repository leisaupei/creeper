using System;
using Xunit;

namespace Creeper.Generator.xUnitTest
{
	public class GeneratorTest
	{
		[Fact]
		public void SqlServer()
		{
			Generator.Gen(@"-o D:\workspacenon\creeper\test\entities -p Creeper.SqlServer.Test -s f --b source=.;user=sa;pwd=123456;db=demo;type=sqlserver");
		}
		[Fact]
		public void MySql()
		{
			Generator.Gen(@"-o d:\workspace\creeper\test\entities -p Creeper.MySql.Test -s f --b source=192.168.1.15:3306;user=root;pwd=123456;db=demo;type=mysql");
		}

		[Fact]
		public void PostgreSql()
		{
			Generator.Gen(@"-o d:\workspace\creeper\test\entities -p Creeper.PostgreSql.Test -s f --b source=192.168.1.15:5432;user=postgres;pwd=123456;db=postgres;type=postgresql");
		}

		[Fact]
		public void PostgreSqlLocalhost()
		{
			Generator.Gen(@"-o d:\workspacenon\creeper\test\entities -p Creeper.PostgreSql.Test -s f --b source=localhost:5432;user=postgres;pwd=123456;db=postgres;type=postgresql");
		}
		[Fact]
		public void Sqlite()
		{
			Generator.Gen(@"-o d:\workspace\creeper\test\entities -p Creeper.Sqlite.Test -s f --b source=../../../../../sql/sqlitedemo.db;type=sqlite");
		}

		[Fact]
		public void Access2003()
		{
			Generator.Gen(@"-o d:\workspace\creeper\test\entities -p Creeper.Access2003.Test -s f --b source=../../../../../sql/access2003.mdb;type=access;pwd=123456;provider=Microsoft.Jet.OleDb.4.0");
		}

		[Fact]
		public void Access2007()
		{
			Generator.Gen(@"-o d:\workspace\creeper\test\entities -p Creeper.Access2007.Test -s f --b source=../../../../../sql/access.accdb;type=access;pwd=123456;provider=Microsoft.ACE.OLEDB.12.0");
		}

		[Fact]
		public void Oracle()
		{
			Generator.Gen(@"-o d:\workspace\creeper\test\entities -p Creeper.Oracle.Test -s f --b source=//192.168.1.15:1521/ORCLPDB1;type=oracle;user=CREEPER;pwd=123456");
		}
	}
}
