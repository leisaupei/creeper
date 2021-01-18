using System;

namespace Candy.Model
{
	/// <summary>
	/// 数据库表特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class DbTableAttribute : Attribute
	{
		private readonly Type _dbName;

		/// <summary>
		/// 表名
		/// </summary>
		public string TableName { get; }
		public string DbName => _dbName.Name;

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="tableName">表名</param>
		/// <param name="dbName"></param>
		public DbTableAttribute(string tableName, Type dbName)
		{
			TableName = tableName;
			_dbName = dbName;
		}
	}

	/// <summary>
	/// 主键特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public class PrimaryKeyAttribute : Attribute
	{
		public PrimaryKeyAttribute() { }
	}
}
