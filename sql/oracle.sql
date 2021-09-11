/*
 Navicat Premium Data Transfer

 Source Server         : OracleCreeperTest
 Source Server Type    : Oracle
 Source Server Version : 190000
 Source Schema         : CREEPER

 Target Server Type    : Oracle
 Target Server Version : 190000
 File Encoding         : 65001

 Date: 02/09/2021 17:15:37
*/


-- ----------------------------
-- Table structure for IdenColumnTest
-- ----------------------------
DROP TABLE "CREEPER"."IdenColumnTest";
CREATE TABLE "CREEPER"."IdenColumnTest" (
  "Id" NUMBER(11) VISIBLE DEFAULT "CREEPER"."ISEQ$$_73131".nextval NOT NULL ,
  "Name" VARCHAR2(50 BYTE) VISIBLE NOT NULL 
)
TABLESPACE "creepertest"
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;

-- ----------------------------
-- Table structure for IdenPkTest
-- ----------------------------
DROP TABLE "CREEPER"."IdenPkTest";
CREATE TABLE "CREEPER"."IdenPkTest" (
  "Id" NUMBER VISIBLE NOT NULL ,
  "Name" VARCHAR2(255 BYTE) VISIBLE DEFAULT 'Sam'  
)
TABLESPACE "creepertest"
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;

-- ----------------------------
-- Table structure for IdenUidCompositePkTest
-- ----------------------------
DROP TABLE "CREEPER"."IdenUidCompositePkTest";
CREATE TABLE "CREEPER"."IdenUidCompositePkTest" (
  "Id" NUMBER VISIBLE DEFAULT "CREEPER"."SEQ_IDEN_PK_DEFAULT_TEST".nextval  NOT NULL ,
  "Name" VARCHAR2(255 BYTE) VISIBLE ,
  "Uid" VARCHAR2(20 BYTE) VISIBLE NOT NULL 
)
TABLESPACE "creepertest"
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;

-- ----------------------------
-- Table structure for LongRawTypeTest
-- ----------------------------
DROP TABLE "CREEPER"."LongRawTypeTest";
CREATE TABLE "CREEPER"."LongRawTypeTest" (
  "Id" NUMBER VISIBLE NOT NULL ,
  "LongRawType" LONG RAW VISIBLE 
)
TABLESPACE "creepertest"
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;

-- ----------------------------
-- Table structure for LongVarcharTypeTest
-- ----------------------------
DROP TABLE "CREEPER"."LongVarcharTypeTest";
CREATE TABLE "CREEPER"."LongVarcharTypeTest" (
  "Id" NUMBER VISIBLE NOT NULL ,
  "LongVarcharType" LONG VISIBLE 
)
TABLESPACE "creepertest"
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;

-- ----------------------------
-- Table structure for TypeTest
-- ----------------------------
DROP TABLE "CREEPER"."TypeTest";
CREATE TABLE "CREEPER"."TypeTest" (
  "Id" NUMBER VISIBLE NOT NULL ,
  "Varchar2Type" VARCHAR2(255 BYTE) VISIBLE ,
  "BFileType" BFILE VISIBLE ,
  "BinaryDoubleType" BINARY_DOUBLE VISIBLE ,
  "BinaryFloatType" BINARY_FLOAT VISIBLE ,
  "BlobType" BLOB VISIBLE ,
  "CharType" CHAR(255 BYTE) VISIBLE ,
  "CharVaryingType" VARCHAR2(255 BYTE) VISIBLE ,
  "CharacterType" CHAR(255 BYTE) VISIBLE ,
  "CharacterVaryingType" VARCHAR2(255 BYTE) VISIBLE ,
  "ClobType" CLOB VISIBLE ,
  "DateType" DATE VISIBLE ,
  "DecimalType" NUMBER(10,4) VISIBLE ,
  "DoublePrecisionType" FLOAT(126) VISIBLE ,
  "FloatType" FLOAT(10) VISIBLE ,
  "IntType" NUMBER VISIBLE ,
  "IntegerType" NUMBER VISIBLE ,
  "IntervalDayToSecondType" INTERVAL DAY(2) TO SECOND(6) VISIBLE ,
  "IntervalYearToMonthType" INTERVAL YEAR(2) TO MONTH VISIBLE ,
  "LongType" LONG VISIBLE ,
  "NationalCharType" NCHAR(255) VISIBLE ,
  "NationalCharVaryingType" NVARCHAR2(255) VISIBLE ,
  "NationalCharacterType" NCHAR(255) VISIBLE ,
  "NationalCharacterVaryingType" NVARCHAR2(255) VISIBLE ,
  "NcharType" NCHAR(255) VISIBLE ,
  "NcharVaryingType" NVARCHAR2(255) VISIBLE ,
  "NclobType" NCLOB VISIBLE ,
  "NumberType" NUMBER(10,2) VISIBLE ,
  "NumericType" NUMBER(10,4) VISIBLE ,
  "RawType" RAW(255) VISIBLE ,
  "RealType" FLOAT(63) VISIBLE ,
  "RowIdType" ROWID VISIBLE ,
  "SmallIntType" NUMBER VISIBLE ,
  "TimestampType" TIMESTAMP(6) VISIBLE ,
  "TimestampWithLocalTimeZoneType" TIMESTAMP(6) WITH LOCAL TIME ZONE VISIBLE ,
  "TimestampWithTimeZoneType" TIMESTAMP(6) WITH TIME ZONE VISIBLE ,
  "URowIdType" UROWID(255) VISIBLE ,
  "VarcharType" VARCHAR2(255 BYTE) VISIBLE ,
  "GuidType" CHAR(36 BYTE) VISIBLE ,
  "NumberLongType" NUMBER(21) VISIBLE ,
  "NumberBoolType" NUMBER(1) VISIBLE ,
  "NumberIntType" NUMBER(9) VISIBLE 
)
TABLESPACE "creepertest"
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;
COMMENT ON COLUMN "CREEPER"."TypeTest"."Id" IS '主键';
COMMENT ON TABLE "CREEPER"."TypeTest" IS '测试CLR';

-- ----------------------------
-- Table structure for UidCompositePkTest
-- ----------------------------
DROP TABLE "CREEPER"."UidCompositePkTest";
CREATE TABLE "CREEPER"."UidCompositePkTest" (
  "Id" VARCHAR2(20 BYTE) VISIBLE NOT NULL ,
  "Id2" VARCHAR2(20 BYTE) VISIBLE NOT NULL ,
  "Name" VARCHAR2(255 BYTE) VISIBLE 
)
TABLESPACE "creepertest"
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;

-- ----------------------------
-- Table structure for UniKeyTest
-- ----------------------------
DROP TABLE "CREEPER"."UniKeyTest";
CREATE TABLE "CREEPER"."UniKeyTest" (
  "Id" CHAR(36 BYTE) VISIBLE NOT NULL ,
  "IdxUniqueKeyTest" VARCHAR2(255 BYTE) VISIBLE NOT NULL ,
  "UniqueKeyTest" VARCHAR2(255 BYTE) VISIBLE NOT NULL 
)
TABLESPACE "creepertest"
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;
COMMENT ON COLUMN "CREEPER"."UniKeyTest"."Id" IS 'Guid';
COMMENT ON COLUMN "CREEPER"."UniKeyTest"."IdxUniqueKeyTest" IS '索引唯一键';
COMMENT ON COLUMN "CREEPER"."UniKeyTest"."UniqueKeyTest" IS '普通唯一键';

-- ----------------------------
-- Table structure for UniPkTest
-- ----------------------------
DROP TABLE "CREEPER"."UniPkTest";
CREATE TABLE "CREEPER"."UniPkTest" (
  "Id" VARCHAR2(20 BYTE) VISIBLE NOT NULL ,
  "Name" VARCHAR2(255 BYTE) VISIBLE 
)
TABLESPACE "creepertest"
LOGGING
NOCOMPRESS
PCTFREE 10
INITRANS 1
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
)
PARALLEL 1
NOCACHE
DISABLE ROW MOVEMENT
;

-- ----------------------------
-- View structure for v_test
-- ----------------------------
CREATE OR REPLACE VIEW "CREEPER"."v_test" AS select "id","name","LongType"from "IdenPkTest" where "id" = 0;

-- ----------------------------
-- Sequence structure for ISEQ$$_73131
-- ----------------------------
DROP SEQUENCE "CREEPER"."ISEQ$$_73131";
CREATE SEQUENCE "CREEPER"."ISEQ$$_73131" MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 CACHE 20;

-- ----------------------------
-- Sequence structure for ISEQ$$_73134
-- ----------------------------
DROP SEQUENCE "CREEPER"."ISEQ$$_73134";
CREATE SEQUENCE "CREEPER"."ISEQ$$_73134" MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 CACHE 20;

-- ----------------------------
-- Sequence structure for ISEQ$$_73233
-- ----------------------------
DROP SEQUENCE "CREEPER"."ISEQ$$_73233";
CREATE SEQUENCE "CREEPER"."ISEQ$$_73233" MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 CACHE 20;

-- ----------------------------
-- Sequence structure for SEQ_IDEN_PK_DEFAULT_TEST
-- ----------------------------
DROP SEQUENCE "CREEPER"."SEQ_IDEN_PK_DEFAULT_TEST";
CREATE SEQUENCE "CREEPER"."SEQ_IDEN_PK_DEFAULT_TEST" MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 NOCACHE;

-- ----------------------------
-- Sequence structure for SEQ_IDEN_PK_TEST
-- ----------------------------
DROP SEQUENCE "CREEPER"."SEQ_IDEN_PK_TEST";
CREATE SEQUENCE "CREEPER"."SEQ_IDEN_PK_TEST" MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 NOCACHE;

-- ----------------------------
-- Sequence structure for SEQ_TYPE_TEST
-- ----------------------------
DROP SEQUENCE "CREEPER"."SEQ_TYPE_TEST";
CREATE SEQUENCE "CREEPER"."SEQ_TYPE_TEST" MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 NOCACHE;

-- ----------------------------
-- Primary Key structure for table IdenColumnTest
-- ----------------------------
ALTER TABLE "CREEPER"."IdenColumnTest" ADD CONSTRAINT "SYS_C007612" PRIMARY KEY ("Id");

-- ----------------------------
-- Checks structure for table IdenColumnTest
-- ----------------------------
ALTER TABLE "CREEPER"."IdenColumnTest" ADD CONSTRAINT "SYS_C007610" CHECK ("Id" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;
ALTER TABLE "CREEPER"."IdenColumnTest" ADD CONSTRAINT "SYS_C007611" CHECK ("Name" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Primary Key structure for table IdenPkTest
-- ----------------------------
ALTER TABLE "CREEPER"."IdenPkTest" ADD CONSTRAINT "SYS_C007606" PRIMARY KEY ("Id");

-- ----------------------------
-- Checks structure for table IdenPkTest
-- ----------------------------
ALTER TABLE "CREEPER"."IdenPkTest" ADD CONSTRAINT "SYS_C007605" CHECK ("Id" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Triggers structure for table IdenPkTest
-- ----------------------------
CREATE TRIGGER "CREEPER"."TRG_IDEN_PK_TEST_PRIMARY" BEFORE INSERT ON "CREEPER"."IdenPkTest" REFERENCING OLD AS "OLD" NEW AS "NEW" FOR EACH ROW 
begin
SELECT SEQ_IDEN_PK_TEST.nextval
    INTO :NEW."Id"
    FROM DUAL; 
end;
/

-- ----------------------------
-- Primary Key structure for table IdenUidCompositePkTest
-- ----------------------------
ALTER TABLE "CREEPER"."IdenUidCompositePkTest" ADD CONSTRAINT "SYS_C007689" PRIMARY KEY ("Id", "Uid");

-- ----------------------------
-- Checks structure for table IdenUidCompositePkTest
-- ----------------------------
ALTER TABLE "CREEPER"."IdenUidCompositePkTest" ADD CONSTRAINT "SYS_C007616" CHECK ("Id" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;
ALTER TABLE "CREEPER"."IdenUidCompositePkTest" ADD CONSTRAINT "SYS_C007688" CHECK ("Uid" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Primary Key structure for table LongRawTypeTest
-- ----------------------------
ALTER TABLE "CREEPER"."LongRawTypeTest" ADD CONSTRAINT "SYS_C007628" PRIMARY KEY ("Id");

-- ----------------------------
-- Primary Key structure for table LongVarcharTypeTest
-- ----------------------------
ALTER TABLE "CREEPER"."LongVarcharTypeTest" ADD CONSTRAINT "SYS_C007634" PRIMARY KEY ("Id");

-- ----------------------------
-- Checks structure for table LongVarcharTypeTest
-- ----------------------------
ALTER TABLE "CREEPER"."LongVarcharTypeTest" ADD CONSTRAINT "SYS_C007633" CHECK ("Id" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Primary Key structure for table TypeTest
-- ----------------------------
ALTER TABLE "CREEPER"."TypeTest" ADD CONSTRAINT "SYS_C007603" PRIMARY KEY ("Id");

-- ----------------------------
-- Checks structure for table TypeTest
-- ----------------------------
ALTER TABLE "CREEPER"."TypeTest" ADD CONSTRAINT "SYS_C007602" CHECK ("Id" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Triggers structure for table TypeTest
-- ----------------------------
CREATE TRIGGER "CREEPER"."TRG_TYPE_TEST_PRIMARY" BEFORE INSERT ON "CREEPER"."TypeTest" REFERENCING OLD AS "OLD" NEW AS "NEW" FOR EACH ROW 
BEGIN
  SELECT SEQ_TYPE_TEST.nextval
    INTO :NEW."Id"
    FROM DUAL;
END;
/

-- ----------------------------
-- Primary Key structure for table UidCompositePkTest
-- ----------------------------
ALTER TABLE "CREEPER"."UidCompositePkTest" ADD CONSTRAINT "SYS_C007687" PRIMARY KEY ("Id", "Id2");

-- ----------------------------
-- Checks structure for table UidCompositePkTest
-- ----------------------------
ALTER TABLE "CREEPER"."UidCompositePkTest" ADD CONSTRAINT "SYS_C007685" CHECK ("Id" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;
ALTER TABLE "CREEPER"."UidCompositePkTest" ADD CONSTRAINT "SYS_C007686" CHECK ("Id2" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Primary Key structure for table UniKeyTest
-- ----------------------------
ALTER TABLE "CREEPER"."UniKeyTest" ADD CONSTRAINT "SYS_C007621" PRIMARY KEY ("Id");

-- ----------------------------
-- Uniques structure for table UniKeyTest
-- ----------------------------
ALTER TABLE "CREEPER"."UniKeyTest" ADD CONSTRAINT "uni_key_test" UNIQUE ("UniqueKeyTest") NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Checks structure for table UniKeyTest
-- ----------------------------
ALTER TABLE "CREEPER"."UniKeyTest" ADD CONSTRAINT "SYS_C007618" CHECK ("Id" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;
ALTER TABLE "CREEPER"."UniKeyTest" ADD CONSTRAINT "SYS_C007619" CHECK ("IdxUniqueKeyTest" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;
ALTER TABLE "CREEPER"."UniKeyTest" ADD CONSTRAINT "SYS_C007620" CHECK ("UniqueKeyTest" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;

-- ----------------------------
-- Indexes structure for table UniKeyTest
-- ----------------------------
CREATE UNIQUE INDEX "CREEPER"."idx_uni_key"
  ON "CREEPER"."UniKeyTest" (""IdxUniqueKeyTest"" DESC)
  LOGGING
  TABLESPACE "creepertest"
  VISIBLE
PCTFREE 10
INITRANS 2
STORAGE (
  INITIAL 65536 
  NEXT 1048576 
  MINEXTENTS 1
  MAXEXTENTS 2147483645
  BUFFER_POOL DEFAULT
  FLASH_CACHE DEFAULT
)
   USABLE;

-- ----------------------------
-- Primary Key structure for table UniPkTest
-- ----------------------------
ALTER TABLE "CREEPER"."UniPkTest" ADD CONSTRAINT "SYS_C007682" PRIMARY KEY ("Id");

-- ----------------------------
-- Checks structure for table UniPkTest
-- ----------------------------
ALTER TABLE "CREEPER"."UniPkTest" ADD CONSTRAINT "SYS_C007681" CHECK ("Id" IS NOT NULL) NOT DEFERRABLE INITIALLY IMMEDIATE NORELY VALIDATE;
