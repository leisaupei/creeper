using System;
using Creeper.Generic;
namespace Creeper.Annotations
{
	/// <summary>
	/// 数据库表特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class CreeperTableAttribute : Attribute
	{
		/// <summary>
		/// 表名
		/// </summary>
		public string TableName { get; }
		public CreeperTableAttribute(string tableName)
		{
			TableName = tableName;
		}
	}
}
