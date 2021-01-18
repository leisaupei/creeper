using Newtonsoft.Json.Linq;
using Npgsql;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Candy.Driver.PostgreSql.Extensions
{
    /// <summary>
    /// 数据库类型拓展
    /// </summary>
    public static class PostgreSqlTypeMappingExtensions
    {

        public static readonly Type[] JTypes = new[] { typeof(JToken), typeof(JObject), typeof(JArray) };
        /// <summary>
        /// 使用自定义xml映射, 使用XmlDocument接收
        /// </summary>
        /// <param name="map"></param>
        public static void UseCustomXml(this INpgsqlTypeMapper map)
        {
            map.RemoveMapping("xml");
            map.AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = "xml",
                NpgsqlDbType = NpgsqlDbType.Xml,
                ClrTypes = new[] { typeof(XmlDocument) },
                TypeHandlerFactory = new XmlHandlerFactory()
            }.Build());
        }
        public static void UseJsonNetForJtype(this INpgsqlTypeMapper mapper)
        {
            mapper.UseJsonNet(JTypes);
        }
    }

    #region Custom PostgresType,Factory,Handler
    internal class XmlHandlerFactory : NpgsqlTypeHandlerFactory<XmlDocument>
    {
        private static PostgresTypeModel _xmlType = null;
        internal static readonly string _sql = @"

SELECT ns.nspname, a.typname, a.oid, a.typbasetype, a.typnotnull,
CASE WHEN pg_proc.proname='array_recv' THEN 'a' ELSE a.typtype END AS typtype,
CASE
  WHEN pg_proc.proname='array_recv' THEN a.typelem
  ELSE 0
END AS typelem,
CASE
  WHEN pg_proc.proname IN ('array_recv','oidvectorrecv') THEN 3    /* Arrays before */
  WHEN a.typtype='r' THEN 2                                        /* Ranges before */
  WHEN a.typtype='d' THEN 1                                        /* Domains before */
  ELSE 0                                                           /* Base types first */
END AS ord
FROM pg_type AS a
JOIN pg_namespace AS ns ON (ns.oid = a.typnamespace)
JOIN pg_proc ON pg_proc.oid = a.typreceive
LEFT OUTER JOIN pg_class AS cls ON (cls.oid = a.typrelid)
LEFT OUTER JOIN pg_type AS b ON (b.oid = a.typelem)
LEFT OUTER JOIN pg_class AS elemcls ON (elemcls.oid = b.typrelid)
WHERE
  a.typname = 'xml' and a.typtype = 'b'
ORDER BY typname;
";

        private PostgresTypeModel GetXmlTypeModel(NpgsqlConnection conn)
        {
            var info = new PostgresTypeModel();
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            using var cmd = new NpgsqlCommand(_sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                info.Namespace = reader.GetString(0);
                info.InternalName = reader.GetString(1);
                info.Oid = Convert.ToUInt32(reader.GetValue(2));
                break;
            }
            return info;
        }
        public override NpgsqlTypeHandler<XmlDocument> Create(PostgresType pgType, NpgsqlConnection conn)
        {
            if (_xmlType == null)
                _xmlType = GetXmlTypeModel(conn);

            return new XmlHandler(new PostgresXmlType(_xmlType.Namespace, _xmlType.InternalName, _xmlType.Oid));
        }
        private class PostgresTypeModel
        {
            public string Namespace { get; set; }
            public string InternalName { get; set; }
            public uint Oid { get; set; }
        }
    }
    internal class PostgresXmlType : PostgresBaseType
    {
        protected internal PostgresXmlType(string ns, string internalName, uint oid) : base(ns, internalName, oid) { }
    }
    internal class XmlHandler : NpgsqlTypeHandler<XmlDocument>, INpgsqlTypeHandler<XmlDocument>
    {
        public XmlHandler(PostgresType postgresType) : base(postgresType) { }

        public override ValueTask<XmlDocument> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            if (len == 0)
                return new ValueTask<XmlDocument>(result: null);
            var xmlStr = buf.ReadString(len);
            var xml = new XmlDocument();
            if (string.IsNullOrEmpty(xmlStr))
                xml.LoadXml(xmlStr);
            return new ValueTask<XmlDocument>(xml);
        }

        public override int ValidateAndGetLength(XmlDocument value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            return value.InnerXml.Length;
        }
        public override Task Write(XmlDocument value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (value == null)
                return buf.WriteString(null, 0, async);
            var xmlStr = value.InnerXml;
            var charLen = parameter == null || parameter.Size <= 0 || parameter.Size >= xmlStr.Length ? xmlStr.Length : parameter.Size;
            return buf.WriteString(xmlStr, charLen, async);
        }
    }
    #endregion

}
