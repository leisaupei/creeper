using System;
using System.Data.Common;

namespace Candy
{
	public class CandySqlExecuteException : DbException
	{
		public CandySqlExecuteException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}
