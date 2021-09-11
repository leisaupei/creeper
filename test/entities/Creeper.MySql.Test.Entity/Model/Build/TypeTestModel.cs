using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.MySql.Test.Entity.Model
{
	[CreeperTable("`type_test`")]
	public partial class TypeTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true, IsIdentity = true)]
		public int Id { get; set; }

		public long? Bigint_t { get; set; }

		public byte[] Binary_t { get; set; }

		public byte? Bit_t { get; set; }

		public byte[] Blob_t { get; set; }

		public string Char_t { get; set; }

		public DateTime? Date_t { get; set; }

		public DateTime? Datetime_t { get; set; }

		public decimal? Decimal_t { get; set; }

		public double? Double_t { get; set; }

		public TypeTestEnumT? Enum_t { get; set; }

		public float? Float_t { get; set; }

		public Creeper.MySql.Types.MySqlGeometry Geometry_t { get; set; }

		public Creeper.MySql.Types.MySqlGeometryCollection Geometrycollection_t { get; set; }

		public int? Integer_t { get; set; }

		public string Json_t { get; set; }

		public Creeper.MySql.Types.MySqlLineString Linestring_t { get; set; }

		public decimal? Numeric_t { get; set; }

		public Creeper.MySql.Types.MySqlPoint Point_t { get; set; }

		public Creeper.MySql.Types.MySqlPolygon Polygon_t { get; set; }

		public double? Real_t { get; set; }

		public string Set_t { get; set; }

		public short? Smallint_t { get; set; }

		public string Text_t { get; set; }

		public TimeSpan? Time_t { get; set; }

		public DateTime? Timestamp_t { get; set; }

		public byte[] Tinyblob_t { get; set; }

		public sbyte? Tinyint_t { get; set; }

		public string Tinytext_t { get; set; }

		public byte[] Varbinary_t { get; set; }

		public string Varchar_t { get; set; }

		public short? Year_t { get; set; }

		public Creeper.MySql.Types.MySqlMultiLineString Multilinestring_t { get; set; }

		public Creeper.MySql.Types.MySqlMultiPolygon Multipolygon_t { get; set; }

		public Creeper.MySql.Types.MySqlMultiPoint Mulitpoint_t { get; set; }
	}
}
