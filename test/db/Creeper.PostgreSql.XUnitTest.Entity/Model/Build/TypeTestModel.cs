/* ##########################################################
 * #   此文件由 https://github.com/leisaupei/creeper 生成    #
 * ##########################################################
 */
using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.NetworkInformation;
using NpgsqlTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Creeper.Generic;
using Creeper.PostgreSql.XUnitTest.Entity.Options;

namespace Creeper.PostgreSql.XUnitTest.Entity.Model
{

	[DbTable(@"""public"".""type_test""", typeof(DbMain))]
	public partial class TypeTestModel : ICreeperDbModel
	{
		#region Properties
		[PrimaryKey] public Guid Id { get; set; }
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
		public short Serial2_type { get; set; }
		public int Serial4_type { get; set; }
		public long Serial8_type { get; set; }
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
		public EDataState? Enum_type { get; set; }
		public Info Composite_type { get; set; }
		public BitArray Bit_length_type { get; set; }
		public int[] Array_type { get; set; }
		public Guid[] Uuid_array_type { get; set; }
		public string[] Varchar_array_type { get; set; }
		#endregion

	}
}
