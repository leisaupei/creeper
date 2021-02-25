﻿using System;

namespace Creeper.Attributes
{

	/// <summary>
	/// 主键特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public class CreeperPrimaryKeyAttribute : Attribute
	{
		public CreeperPrimaryKeyAttribute() { }
	}
}