using System;

namespace Creeper.Attributes
{
	/// <summary>
	/// 排除字段
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public class CreeperIgnoreAttribute : Attribute
	{
		public CreeperIgnoreAttribute() { }
	}
}
