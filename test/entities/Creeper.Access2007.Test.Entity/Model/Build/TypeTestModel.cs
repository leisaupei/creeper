using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Access2007.Test.Entity.Model
{
	/// <summary>
	/// 类型映射测试
	/// </summary>
	[CreeperTable("[TypeTest]")]
	public partial class TypeTestModel : ICreeperModel
	{
		/// <summary>
		/// 主键id
		/// </summary>
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		public string ShortTextType { get; set; }

		public int LongType { get; set; }

		public decimal? CurrencyType { get; set; }

		public DateTime? DateTimeType { get; set; }

		public bool BooleanType { get; set; }

		public string FormatTextType { get; set; }

		public string LongTextType { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update)]
		public string AttachmentType { get; set; }

		public string HttpType { get; set; }

		public float? FloatType { get; set; }

		public byte[] OLEObjectType { get; set; }

		public double? DoubleType { get; set; }

		public Guid? GuidType { get; set; }

		public decimal? NumericType { get; set; }

		public byte? ByteType { get; set; }

		public short? IntegerType { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update)]
		public int? CalcNumberType { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update)]
		public bool CalcBoolType { get; set; }

		public DateTime? TimeType { get; set; }
	}
}
