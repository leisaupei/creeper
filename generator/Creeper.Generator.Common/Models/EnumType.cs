using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common.Models
{
	public class EnumType
	{
		/// <summary>
		/// 数据库的枚举名称
		/// </summary>
		public string DbEnumName { get; set; }

		/// <summary>
		/// 模式名称
		/// </summary>
		public string SchemaName { get; set; }

		/// <summary>
		/// 枚举注释
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// C#枚举名称
		/// </summary>
		public string CsharpTypeName { get; set; }

		/// <summary>
		/// 枚举元素
		/// </summary>
		public string[] Elements { get; set; }
	}
}
