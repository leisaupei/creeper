using Creeper.Generator.Common.Extensions;
using System;
using System.Text;

namespace Creeper.Generator.Common.Models
{
	/// <summary>
	/// 列信息
	/// </summary>
	public class ColumnInfo
	{

		/// <summary>
		/// 字段名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 字段名称 首字母大写
		/// </summary>
		public string NameUpCase => GeneratorHelper.ConvertColumnNameToPropertyName(Name);

		/// <summary>
		/// 字段数据库长度
		/// </summary> 
		public int Length { get; set; }
		/// <summary>
		/// 标识
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 数据类型
		/// </summary>
		public string DataType { get; set; }
		/// <summary>
		/// 是否可空
		/// </summary>
		public bool IsNullable { get; set; } = false;
		/// <summary>
		/// 是否主键
		/// </summary>
		public bool IsPrimary { get; set; } = false;
		/// <summary>
		/// C#类型
		/// </summary>
		public string CsharpTypeName { get; set; }
		/// <summary>
		/// 是否自增
		/// </summary>
		public bool IsIdentity { get; set; } = false;
		/// <summary>
		/// 是否唯一键
		/// </summary>
		public bool IsUnique { get; set; } = false;

		/// <summary>
		/// 默认值
		/// </summary>
		public string DefaultValue { get; set; }

		/// <summary>
		/// 维度
		/// </summary>
		public int Dimensions { get; set; }

		/// <summary>
		/// 是否数组
		/// </summary>
		public bool IsArray { get; set; } = false;

		/// <summary>
		/// 是否枚举
		/// </summary>
		public bool IsEnum { get; set; } = false;

		/// <summary>
		/// 是否复合类型
		/// </summary>
		public bool IsComposite { get; set; } = false;

		/// <summary>
		/// 是否空间数据类型
		/// </summary>
		public bool IsGeometry { get; set; } = false;

		/// <summary>
		/// 类型分类
		/// </summary>
		public string Typcategory { get; set; }

		/// <summary>
		/// 类型的模式名称, 自定义类型的模式名称
		/// </summary>
		public string SchemaName { get; set; }

		/// <summary>
		/// 是否只读字段
		/// </summary>
		public bool IsReadOnly { get; set; } = false;

		/// <summary>
		/// 大小(一般用于数字类型)
		/// </summary>
		public int Precision { get; set; }

		/// <summary>
		/// 比例(一般用于数字类型)
		/// </summary>
		public int Scale { get; set; }

		/// <summary>
		/// 暂不支持类型
		/// </summary>
		public bool IsNotSupported { get; set; } = false;
	}

}
