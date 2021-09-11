using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.Oracle.Test.Entity.Model
{
	/// <summary>
	/// 测试CLR
	/// </summary>
	[CreeperTable("\"TypeTest\"")]
	public partial class TypeTestModel : ICreeperModel
	{
		/// <summary>
		/// 主键
		/// </summary>
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public long Id { get; set; }

		public string Varchar2Type { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update)]
		public byte[] BFileType { get; set; }

		public double? BinaryDoubleType { get; set; }

		public float? BinaryFloatType { get; set; }

		public byte[] BlobType { get; set; }

		public string CharType { get; set; }

		public string CharVaryingType { get; set; }

		public string CharacterType { get; set; }

		public string CharacterVaryingType { get; set; }

		public string ClobType { get; set; }

		public DateTime? DateType { get; set; }

		public decimal? DecimalType { get; set; }

		public double? DoublePrecisionType { get; set; }

		public float? FloatType { get; set; }

		public int? IntType { get; set; }

		public int? IntegerType { get; set; }

		public TimeSpan? IntervalDayToSecondType { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update)]
		public int? IntervalYearToMonthType { get; set; }

		[CreeperColumn(IgnoreFlags = IgnoreWhen.Returning)]
		public string LongType { get; set; }

		public string NationalCharType { get; set; }

		public string NationalCharVaryingType { get; set; }

		public string NationalCharacterType { get; set; }

		public string NationalCharacterVaryingType { get; set; }

		public string NcharType { get; set; }

		public string NcharVaryingType { get; set; }

		public string NclobType { get; set; }

		public decimal? NumberType { get; set; }

		public decimal? NumericType { get; set; }

		public byte[] RawType { get; set; }

		public float? RealType { get; set; }

		public string RowIdType { get; set; }

		public int? SmallIntType { get; set; }

		public DateTime? TimestampType { get; set; }

		public DateTime? TimestampWithLocalTimeZoneType { get; set; }

		public DateTime? TimestampWithTimeZoneType { get; set; }

		public string URowIdType { get; set; }

		public string VarcharType { get; set; }

		public Guid? GuidType { get; set; }

		public long? NumberLongType { get; set; }

		public bool? NumberBoolType { get; set; }

		public int? NumberIntType { get; set; }
	}
}
