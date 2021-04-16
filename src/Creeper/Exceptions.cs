using System;
using System.Data.Common;

namespace Creeper
{
	public class CreeperException : Exception
	{
		public CreeperException(string message) : base(message)
		{
		}

		public CreeperException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
	public class NoPrimaryKeyException<T> : CreeperException
	{
		public NoPrimaryKeyException() : base(typeof(T).Name + "没有主键标识")
		{
		}
	}
	public class CreeperSqlExecuteException : DbException
	{
		public CreeperSqlExecuteException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
}
