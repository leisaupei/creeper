using Creeper.Generic;
using System;

namespace Creeper.Attributes
{
	/// <summary>
	/// 主键特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public class CreeperDbColumnAttribute : Attribute
	{
		/// <summary>
		/// 主键
		/// </summary>
		public bool Primary { get; set; } = false;
		/// <summary>
		/// 忽略
		/// </summary>
		public IgnoreWhen Ignore { get; set; } = IgnoreWhen.None;
		/// <summary>
		/// 自增字段
		/// </summary>
		public bool Identity { get; set; } = false;
		public CreeperDbColumnAttribute() { }
	}
	/// <summary>
	/// 数据库字段忽略策略
	/// </summary>
	public enum IgnoreWhen
	{
		/// <summary>
		/// 输入时忽略, Insert
		/// </summary>
		Input = 1,
		/// <summary>
		/// 查询输出时忽略
		/// </summary>
		Output = 2,
		/// <summary>
		/// 都忽略
		/// </summary>
		Both = 3,
		/// <summary>
		/// 不忽略
		/// </summary>
		None = 4
	}
}
