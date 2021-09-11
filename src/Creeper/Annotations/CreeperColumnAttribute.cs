using Creeper.Generic;
using System;

namespace Creeper.Annotations
{
	/// <summary>
	/// 列特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public class CreeperColumnAttribute : Attribute
	{
		/// <summary>
		/// 主键
		/// </summary>
		public bool IsPrimary { get; set; } = false;

		/// <summary>
		/// 字段忽略, Flags
		/// </summary>
		public IgnoreWhen IgnoreFlags { get; set; } = IgnoreWhen.None;

		/// <summary>
		/// 自增字段
		/// </summary>
		public bool IsIdentity { get; set; } = false;

		/// <summary>
		/// 唯一键
		/// </summary>
		public bool IsUnique { get; set; }

		public CreeperColumnAttribute() { }
	}

	/// <summary>
	/// 数据库字段忽略策略
	/// </summary>
	[Flags]
	public enum IgnoreWhen
	{

		/// <summary>
		/// 不忽略
		/// </summary>
		None = 0x0,

		/// <summary>
		/// Insert对象时
		/// </summary>
		Insert = 0x1,

		/// <summary>
		/// 数据库查询返回数据时, 包括Select/Insert/Update/Upsert的Returning
		/// </summary>
		Returning = 0x2,

		/// <summary>
		/// Update对象时
		/// </summary>
		Update = 0x3,
	}
}
