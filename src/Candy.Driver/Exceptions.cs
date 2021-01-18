using System;
using System.Data.Common;

namespace Candy.Driver
{
	public class CandySqlExecuteException : DbException
	{
		public CandySqlExecuteException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}
