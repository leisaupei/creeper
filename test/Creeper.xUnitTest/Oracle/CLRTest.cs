using Creeper.Driver;
using Creeper.Oracle.Test.Entity.Model;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using Xunit;

namespace Creeper.xUnitTest.Oracle
{
	public class CLRTest : BaseTest
	{
		private const int Id = 1;
		[Fact]
		public void Insert()
		{
			var info = Context.Insert(new TypeTestModel
			{
				CharType = "中国",
				DateType = DateTime.Today,
				NumericType = 12.3M,
				RealType = 12.3F,
				SmallIntType = 123,
				VarcharType = "中国",
				DecimalType = 12.3M,
				IntType = 123,
				NcharType = "中国",
				BFileType = new byte[] { 12, 23, 41 },
				BinaryDoubleType = 123.141D,
				BinaryFloatType = 443.12F,
				BlobType = new byte[] { 1, 13, 4 },
				CharacterType = "中国",
				CharacterVaryingType = "中国",
				CharVaryingType = "中国",
				ClobType = "中国",
				DoublePrecisionType = 123.141D,
				FloatType = 123.141F,
				GuidType = System.Guid.NewGuid(),
				IntegerType = 10,
				IntervalDayToSecondType = TimeSpan.FromMinutes(2222),
				IntervalYearToMonthType = 1,
				LongType = "中国",
				NationalCharacterType = "中国",
				NationalCharacterVaryingType = "中国",
				NationalCharType = "中国",
				NationalCharVaryingType = "中国",
				NcharVaryingType = "中国",
				NclobType = "中国",
				NumberBoolType = false,
				NumberIntType = 10,
				NumberLongType = 1000,
				NumberType = 99.11M,
				RawType = new byte[] { 2, 1, 34, },
				RowIdType = "AAAR2WAAPAAAAC/AAR",
				TimestampType = DateTime.Now,
				TimestampWithLocalTimeZoneType = DateTime.Now,
				TimestampWithTimeZoneType = DateTime.Now,
				URowIdType = "AAAR2WAAPAAAAC/AAR",
				Varchar2Type = "中国",
			});
		}

		/// <summary>
		/// NUMBER(>-,0)类型使用int类型接收, DataReader会返回decimal类型
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData(long.MaxValue)]
		public void NumberLong(long value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NumberLongType);
			Assert.IsType<decimal>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NumberLongType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory, InlineData(12134.211F)]
		public void BinaryFloat(float value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.BinaryFloatType);
			Assert.IsType<float>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.BinaryFloatType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory, InlineData(123.141D)]
		public void BinaryDouble(double value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.BinaryDoubleType);
			Assert.IsType<double>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.BinaryDoubleType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// BFILE类型框架只支持返回byte[]查询结果
		/// </summary>
		/// <param name="value"></param>
		[Fact]
		public void BFile()
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.BFileType);
			Assert.IsType<byte[]>(obj);

			//ADO示例
			var result = Context.ExecuteNonQuery("Update \"TypeTest\" a set \"BFileType\" = BFILENAME('BFILE_TEST_DIR', 'bfiletest.txt') where a.\"Id\" = 1");
			Assert.Equal(1, result);
		}

		/// <summary>
		/// Oracle数据库使用Number(1)类型表示bool值, DataReader返回类型为Int16
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData(false)]
		public void NumberBool(bool value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NumberBoolType);
			Assert.IsType<short>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NumberBoolType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory, InlineData(new byte[] { 1, 242, 213 })]
		public void Blob(byte[] value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.BlobType);
			Assert.IsType<byte[]>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.BlobType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// CHAR
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData("中国")]
		public void Character(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.CharacterType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.CharacterType);
			Assert.IsType<string>(obj);
		}

		/// <summary>
		/// CHAR
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData("中国")]
		public void Char(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.CharType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.CharType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory, InlineData("2021-1-1 20:00:00.123456")]//实际结果是: 2021-01-01 20:00:00
		public void Date(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.DateType);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.DateType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory, InlineData("2021-1-1 20:00:00.23475")]//实际结果是: 2021-01-01 20:00:00.233
		public void Timestamp(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.TimestampType);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.TimestampType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory, InlineData("2021-1-1 20:00:00.6623566")]//实际结果是: 2021-01-01 20:00:00.6633333
		public void TimestampWithLocalTimeZone(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.TimestampWithLocalTimeZoneType);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.TimestampWithLocalTimeZoneType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory, InlineData("2021-1-1 20:00:00.6683425+07:00")]//实际结果是: 2021-01-01 20:00:00.6683425 +08:00
		public void TimestampWithTimeZone(DateTime value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.TimestampWithTimeZoneType);
			Assert.IsType<DateTime>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.TimestampWithTimeZoneType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// NUMBER类型DataReader返回类型为double(设置了scale都会返回double)
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData(231.231)]
		public void Decimal(decimal value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.DecimalType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.DecimalType);
			Assert.IsType<double>(obj);
		}

		/// <summary>
		/// FLOAT类型长度少于63都会使用float接收, 但DataReader会返回double
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData(231.231F)]
		public void Float(float value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.FloatType);
			Assert.IsType<double>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.FloatType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		[Theory, InlineData(new byte[] { 1, 242, 213 })]
		public void Raw(byte[] value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.RawType);
			Assert.IsType<byte[]>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.RawType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// NUMBER(0,0)类型默认使用int类型接收, DataReader会返回decimal类型
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData(int.MaxValue)]
		public void Int(int value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.IntType);
			Assert.IsType<decimal>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.IntType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// NUMBER(0,0)类型默认使用int类型接收, DataReader会返回decimal类型
		/// </summary>
		/// <param name="value"></param>

		[Theory, InlineData(int.MaxValue)]
		public void Integer(int value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.IntegerType);
			Assert.IsType<decimal>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.IntegerType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// NUMBER类型DataReader返回类型为double(设置了scale都会返回double)
		/// </summary>
		/// <param name="value"></param>

		[Theory, InlineData(245.246)]
		public void Number(decimal value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NumberType);
			Assert.IsType<double>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NumberType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// NUMBER类型DataReader返回类型为double(设置了scale都会返回double)
		/// </summary>
		/// <param name="value"></param>

		[Theory, InlineData(245.246)]
		public void Numeric(decimal value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NumericType);
			Assert.IsType<double>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NumericType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// NCHAR
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData("中国")]
		public void Nchar(string value)
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NcharType);
			Assert.IsType<string>(obj);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NcharType, value).ToAffrows();
			Assert.Equal(1, result);
		}

		/// <summary>
		/// NCHAR
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData("中国")]
		public void NationalCharacter(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NationalCharacterType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NationalCharacterType);
			Assert.IsType<string>(obj);
		}

		/// <summary>
		/// NCHAR
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData("中国")]
		public void NationalChar(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NationalCharType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NationalCharType);
			Assert.IsType<string>(obj);
		}

		/// <summary>
		/// NVARCHAR2
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData("中国")]
		public void NationalCharacterVarying(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NationalCharacterVaryingType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NationalCharacterVaryingType);
			Assert.IsType<string>(obj);
		}

		/// <summary>
		/// NVARCHAR2
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData("中国")]
		public void NationalCharVarying(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NationalCharVaryingType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NationalCharVaryingType);
			Assert.IsType<string>(obj);
		}

		/// <summary>
		/// NVARCHAR2
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData("中国")]
		public void NcharVarying(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NcharVaryingType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NcharVaryingType);
			Assert.IsType<string>(obj);
		}


		[Theory, InlineData("中国")]
		public void Clob(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.ClobType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.ClobType);
			Assert.IsType<string>(obj);
		}


		[Theory, InlineData("中国")]
		public void Nclob(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NclobType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NclobType);
			Assert.IsType<string>(obj);
		}

		/// <summary>
		/// FLOAT类型长度少于63都会使用float接收, 但DataReader会返回double
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData(35.03577f)]
		public void Real(float value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.RealType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.RealType);
			Assert.IsType<decimal>(obj);
		}

		/// <summary>
		/// FLOAT类型长度少于63都会使用float接收, 但DataReader会返回double
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData(35.03577D)]
		public void DoublePrecision(double value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.DoublePrecisionType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.DoublePrecisionType);
			Assert.IsType<decimal>(obj);
		}

		/// <summary>
		///  NUMBER(0,0)类型默认使用int类型接收, DataReader会返回decimal类型
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData(int.MaxValue)]
		public void SmallInt(int value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.SmallIntType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.SmallIntType);
			Assert.IsType<decimal>(obj);
		}

		/// <summary>
		/// CHAR(36)默认解析为Guid类型
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData("81d58ab2-4fc6-425a-bc51-d1d73bf9f4b1")]
		public void Guid(Guid value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.GuidType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.GuidType);
			Assert.IsType<string>(obj);
		}


		[Theory, InlineData("中国")]
		public void Varchar2(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.Varchar2Type, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.Varchar2Type);
			Assert.IsType<string>(obj);
		}

		[Theory, InlineData("中国")]
		public void Varchar(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.VarcharType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.VarcharType);
			Assert.IsType<string>(obj);
		}

		[Theory, InlineData("中国")]
		public void CharVarying(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.CharVaryingType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.CharVaryingType);
			Assert.IsType<string>(obj);
		}

		[Theory, InlineData("中国")]
		public void CharacterVarying(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.CharacterVaryingType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.CharacterVaryingType);
			Assert.IsType<string>(obj);
		}

		[Fact]
		public void IntervalDayToSecond()
		{
			var value = new TimeSpan(1, 20, 23, 23, 222);
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.IntervalDayToSecondType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.IntervalDayToSecondType);
			Assert.IsType<TimeSpan>(obj);
		}

		/// <summary>
		/// 返回值为多少个月, 此类型仅支持查询, 可用其他int类型代替
		/// </summary>
		/// <param name="value"></param>
		[Fact]
		public void IntervalYearToMonth()
		{
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.IntervalYearToMonthType);
			Assert.IsType<long>(obj);

			//ADO示例
			var result = Context.ExecuteNonQuery("Update \"TypeTest\" a set \"IntervalYearToMonthType\" = '20-10' where a.\"Id\" = 1");
			Assert.Equal(1, result);
		}

		/// <summary>
		/// 暂不支持数据库中LONG类型数据
		/// </summary>
		[Theory, InlineData("中国")]
		public void Long(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.LongType, value).ToAffrows();
			Assert.Equal(1, result);

			using OracleConnection conn = (OracleConnection)Context.Get(Generic.DataBaseType.Main).ConnectionOptions.GetConnection();
			using OracleCommand cmd = conn.CreateCommand();

			cmd.InitialLONGFetchSize = -1;//如果需要读取LONG类型数据，此处需要赋值为-1，否则会返回空字符串
			cmd.CommandText = "select \"LongType\" from \"TypeTest\" where \"Id\" = 1";
			using OracleDataReader dr = cmd.ExecuteReader();
			while (dr.Read())
			{
				string s1 = dr.GetString(0);
			}
		}

		/// <summary>
		/// 暂不支持数据库中LONG类型数据
		/// </summary>
		[Theory, InlineData("中国")]
		public void LongVarchar(string value)
		{
			var result = Context.Update<LongVarcharTypeTestModel>(a => a.Id == Id).Set(a => a.LongVarcharType, value).ToAffrows();
			Assert.Equal(1, result);

			using OracleConnection conn = (OracleConnection)Context.Get(Generic.DataBaseType.Main).ConnectionOptions.GetConnection();
			using OracleCommand cmd = conn.CreateCommand();

			cmd.InitialLONGFetchSize = -1;//如果需要读取LONG VARCHAR类型数据，此处需要赋值为-1，否则会返回空字符串
			cmd.CommandText = "select \"LongVarcharType\" from \"LongVarcharTypeTest\" where \"Id\" = 1";
			using OracleDataReader dr = cmd.ExecuteReader();
			while (dr.Read())
			{
				string s1 = dr.GetString(0);
			}
		}
		/// <summary>
		/// 暂不支持数据库中LONG类型数据
		/// </summary>
		[Theory, InlineData(new byte[] { 12, 67, 123 })]
		public void LongRaw(byte[] value)
		{
			var result = Context.Update<LongRawTypeTestModel>(a => a.Id == Id).Set(a => a.LongRawType, value).ToAffrows();
			Assert.Equal(1, result);

			using OracleConnection conn = (OracleConnection)Context.Get(Generic.DataBaseType.Main).ConnectionOptions.GetConnection();
			using OracleCommand cmd = conn.CreateCommand();

			cmd.InitialLONGFetchSize = -1;//如果需要读取LONG RAW类型数据，此处需要赋值为-1，否则会返回空字符串
			cmd.CommandText = "select \"LongRawType\" from \"LongRawTypeTest\" where \"Id\" = 1";
			using OracleDataReader dr = cmd.ExecuteReader();
			while (dr.Read())
			{
				byte[] s1 = (byte[])dr[0];
			}
		}
		/// <summary>
		/// NUMBER(9)类型默认使用int类型接收, DataReader会返回decimal类型
		/// </summary>
		/// <param name="value"></param>
		[Theory, InlineData(200000000)]
		public void NumberInt(int value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.NumberIntType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.NumberIntType);
			Assert.IsType<int>(obj);
		}

		[Theory, InlineData("AAAR2WAAPAAAAC/AAR")]
		public void RowId(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.RowIdType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.RowIdType);
			Assert.IsType<string>(obj);
		}

		[Theory, InlineData("AAAR2WAAPAAAAC/AAA")]
		public void URowId(string value)
		{
			var result = Context.Update<TypeTestModel>(a => a.Id == Id).Set(a => a.URowIdType, value).ToAffrows();
			Assert.Equal(1, result);
			var obj = Context.Select<TypeTestModel>(a => a.Id == Id).FirstOrDefault<object>(a => a.URowIdType);
			Assert.IsType<string>(obj);
		}
	}
}
