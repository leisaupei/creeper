using Creeper.Driver;
using System;
using Creeper.Annotations;
using Creeper.Generic;

namespace Creeper.SqlServer.Test.Entity.Model
{
    /// <summary>
    /// CLR测试表
    /// </summary>
    [CreeperTable("[dbo].[TypeTest]")]
    public partial class TypeTestModel : ICreeperModel
    {
        public long? BigintType { get; set; }

        [CreeperColumn(IsPrimary = true, IsIdentity = true)]
        public int Id { get; set; }

        public string NvarcharType { get; set; }

        public bool? BitType { get; set; }

        public byte[] BinaryType { get; set; }

        public string CharType { get; set; }

        public DateTime? DateType { get; set; }

        public DateTime? DateTimeType { get; set; }

        public DateTime? DateTime2Type { get; set; }

        public DateTimeOffset? DateTimeOffsetType { get; set; }

        /// <summary>
        /// 说明测试
        /// </summary>
        public decimal? DecimalType { get; set; }

        public double? FloatType { get; set; }

        [CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update | IgnoreWhen.Returning)]
        public object GeographyType { get; set; }

        [CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update | IgnoreWhen.Returning)]
        public object GeometryType { get; set; }

        [CreeperColumn(IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Update | IgnoreWhen.Returning)]
        public object HierarchyidType { get; set; }

        public byte[] ImageType { get; set; }

        public int? IntType { get; set; }

        public decimal? MoneyType { get; set; }

        public string NcharType { get; set; }

        public string NtextType { get; set; }

        public decimal? NumericType { get; set; }

        public float? RealType { get; set; }

        public DateTime? SmallDateTimeType { get; set; }

        public short? SmallIntType { get; set; }

        public decimal? SmallMoneyType { get; set; }

        public object SqlVariantType { get; set; }

        public string TextType { get; set; }

        public TimeSpan? TimeType { get; set; }

        [CreeperColumn(IgnoreFlags = IgnoreWhen.Update)]
        public byte[] TimestampType { get; set; }

        public byte? TinyIntType { get; set; }

        public Guid? UniqueIdentifierType { get; set; }

        public byte[] VarbinaryType { get; set; }

        public string VarcharType { get; set; }

        public string XmlType { get; set; }
    }
}
