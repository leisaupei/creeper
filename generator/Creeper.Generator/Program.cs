/* ##########################################################
 * #        .net standard 2.1 + data base Code Maker        #
 * #                author by leisaupei                     #
 * #          https://github.com/leisaupei/creeper          #
 * ##########################################################
 */
using System;
using System.Linq;
namespace Creeper.Generator
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine(@"
##########################################################
#        .net standard 2.1 + data base Code Maker        #
#                author by leisaupei                     #
#           https://github.com/leisaupei/creeper         #
##########################################################
> Parameters description:
	-o	output path
	-p	project name
	-s	create .sln file, *optional(t/f) default: f.
	--b	build options array,  arguments must be at the end
		source		database host/address/path
		user		database user id
		pwd			database password
		db			database name
		provider	access required, otherwise ignore
		name		database alias name, value of '@type' if empty or null. *optional
		type		kind of database, postgresql/mysql/sqlserver/access/sqlite/oracle at presents

> Single Example: -o d:\workspace\test -p SimpleTest -s t --b source=localhost:5432;user=postgres;pwd=123456;db=postgres;type=postgresql

> Multiple Example: -o d:\workspace\test -p SimpleTest -s t --b source=localhost:5432;user=postgres;pwd=123456;db=postgres;name=database1;type=postgresql source=localhost:5432;user=postgres;pwd=123456;db=postgres;name=database2;type=postgresql
");

			if (!args?.Any() ?? true)
			{
				string typeIn = null;
				while (string.IsNullOrEmpty(typeIn))
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Please type in parameters.");
					Console.ResetColor();
					typeIn = Console.ReadLine();
				}
				args = typeIn.Split(' ');
			}
			//-o D:\workspace\creeper\test\db -p Creeper.PostgreSql.XUnitTest -s f --b source=localhost:5432;user=postgres;pwd=123456;db=postgres;name=main;type=postgresql
			Generator.Gen(args);
			Console.WriteLine("successful...");
		}


	}
}
