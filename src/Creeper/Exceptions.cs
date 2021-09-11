using Creeper.Generic;
using System;
using System.Data.Common;

namespace Creeper
{
	public class CreeperException : Exception
	{
		public CreeperException() : base() { }
		public CreeperException(string message) : base(message) { }

		public CreeperException(string message, Exception innerException) : base(message, innerException) { }
	}
	public class CreeperNotSupportedException : CreeperException
	{
		public CreeperNotSupportedException() : base() { }
		public CreeperNotSupportedException(string message) : base(message) { }
	}
	public class CreeperFirstNotFoundException : CreeperException
	{
		public CreeperFirstNotFoundException() : base("没有找到First记录") { }
	}
	internal class CreeperNoPrimaryKeyException<T> : CreeperException
	{
		public CreeperNoPrimaryKeyException() : base(typeof(T).Name + "没有主键标识") { }
	}
	internal class CreeperConverterNotFoundException : CreeperException
	{
		public CreeperConverterNotFoundException(string message) : base(message) { }
		public CreeperConverterNotFoundException(DataBaseKind dbKind) : base($"没有添加相应的数据库种类的转换器, 使用Add{dbKind}Option()注入相应数据库配置") { }
	}
	internal class CreeperDbCacheNotFoundException : CreeperException
	{
		public CreeperDbCacheNotFoundException() : base("DbCache为空, 使用UseDbCache<T>方法注入数据库缓存示例") { }
	}
	internal class CreeperDbTableAttributeNotFoundException : CreeperException
	{
		public CreeperDbTableAttributeNotFoundException(string dbModelName) : base(dbModelName + "没有找到CreeperDbTableAttribute特性") { }
	}
	internal class CreeperNotDbModelDeriverException : CreeperException
	{
		public CreeperNotDbModelDeriverException(string dbModelName) : base(dbModelName + "不是ICreeperDbModel派生类") { }
	}
	internal class CreeperDbExecuteNotFoundException : CreeperException
	{
		public CreeperDbExecuteNotFoundException() : base("DbExecute为空") { }
	}
	internal class CreeperSqlExecuteException : CreeperException
	{
		public CreeperSqlExecuteException(string message, Exception innerException) : base(message, innerException) { }
	}
	internal class CreeperDbConnectionOptionNotFoundException : CreeperException
	{
		public CreeperDbConnectionOptionNotFoundException(DataBaseType dataBaseType, DataBaseTypeStrategy dataBaseTypeStrategy) : base($"没找到相应的数据库配置, 策略: {dataBaseTypeStrategy}, 类型: {dataBaseType}") { }
	}
}
