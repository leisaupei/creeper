using System;
using System.Data.Common;

namespace Creeper
{
	public class CreeperSqlExecuteException : DbException
	{
		public CreeperSqlExecuteException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}
