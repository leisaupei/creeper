using System.Collections;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using NpgsqlTypes;
using System.Xml;
using System.Collections.Generic;
using Npgsql.LegacyPostgis;
using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.PostgreSql.Test.Entity.Model
{
	[CreeperTable("\"creeper\".\"type_test\"")]
	public partial class CreeperTypeTestModel : ICreeperModel
	{
		[CreeperColumn(IsPrimary = true)]
		public Guid Id { get; set; }

		public BitArray Bit_type { get; set; }

		public bool? Bool_type { get; set; }

		public NpgsqlBox? Box_type { get; set; }

		public byte[] Bytea_type { get; set; }

		public string Char_type { get; set; }

		public (IPAddress, int)? Cidr_type { get; set; }

		public NpgsqlCircle? Circle_type { get; set; }

		public DateTime? Date_type { get; set; }

		public decimal? Decimal_type { get; set; }

		public float? Float4_type { get; set; }

		public double? Float8_type { get; set; }

		public IPAddress Inet_type { get; set; }

		public short? Int2_type { get; set; }

		public int? Int4_type { get; set; }

		public long? Int8_type { get; set; }

		public TimeSpan? Interval_type { get; set; }

		public JToken Json_type { get; set; }

		public JToken Jsonb_type { get; set; }

		public NpgsqlLine? Line_type { get; set; }

		public NpgsqlLSeg? Lseg_type { get; set; }

		public PhysicalAddress Macaddr_type { get; set; }

		public decimal? Money_type { get; set; }

		public NpgsqlPath? Path_type { get; set; }

		public NpgsqlPoint? Point_type { get; set; }

		public NpgsqlPolygon? Polygon_type { get; set; }

		public string Text_type { get; set; }

		public TimeSpan? Time_type { get; set; }

		public DateTime? Timestamp_type { get; set; }

		public DateTime? Timestamptz_type { get; set; }

		public DateTimeOffset? Timetz_type { get; set; }

		public NpgsqlTsQuery Tsquery_type { get; set; }

		public NpgsqlTsVector Tsvector_type { get; set; }

		public BitArray Varbit_type { get; set; }

		public string Varchar_type { get; set; }

		public XmlDocument Xml_type { get; set; }

		public Dictionary<string, string> Hstore_type { get; set; }

		public BitArray Bit_length_type { get; set; }

		public int[] Array_type { get; set; }

		public Guid[] Uuid_array_type { get; set; }

		public string[] Varchar_array_type { get; set; }

		public PostgisGeometry Geometry_type { get; set; }

		public CreeperDataState? Enum_type { get; set; }

		public CreeperInfo? Composite_type { get; set; }

		[CreeperColumn(IsIdentity = true)]
		public short Serial2_type { get; set; }

		[CreeperColumn(IsIdentity = true)]
		public int Serial4_type { get; set; }

		[CreeperColumn(IsIdentity = true)]
		public long Serial8_type { get; set; }

		public BitArray Bits_type { get; set; }
	}
}
