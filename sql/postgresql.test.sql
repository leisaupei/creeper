/*
 Navicat Premium Data Transfer

 Source Server         : 192.168.1.15
 Source Server Type    : PostgreSQL
 Source Server Version : 130002
 Source Host           : 192.168.1.15:5432
 Source Catalog        : postgres
 Source Schema         : creeper

 Target Server Type    : PostgreSQL
 Target Server Version : 130002
 File Encoding         : 65001

 Date: 02/09/2021 17:52:41
*/
create SCHEMA if not exists "creeper";

create extension if not exists hstore;

-- ----------------------------
-- Type structure for info
-- ----------------------------
DO
$$
BEGIN
  IF NOT EXISTS (SELECT *FROM pg_type typ INNER JOIN pg_namespace nsp ON nsp.oid = typ.typnamespace
		WHERE nsp.nspname = 'creeper' AND typ.typname = 'info') 
	THEN
    CREATE TYPE  "creeper"."info" AS (
			"id" uuid,
			"name" varchar COLLATE "pg_catalog"."default"
		);
  END IF;
END;
$$
LANGUAGE plpgsql;
-- ----------------------------
-- Type structure for data_state
-- ----------------------------
DO
$$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_type typ INNER JOIN pg_namespace nsp ON nsp.oid = typ.typnamespace
		WHERE nsp.nspname = 'creeper'	AND typ.typname = 'data_state') 
	THEN
    CREATE TYPE "creeper"."data_state" AS ENUM ('正常','删除');
  END IF;
END;
$$
LANGUAGE plpgsql;

-- ----------------------------
-- Sequence structure for iden_nopk_name_no_seq
-- ----------------------------
CREATE SEQUENCE IF NOT EXISTS "creeper"."iden_nopk_name_no_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for iden_pk_id_seq
-- ----------------------------
CREATE SEQUENCE IF NOT EXISTS "creeper"."iden_pk_id_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for type_test_serial2_type_seq
-- ----------------------------
CREATE SEQUENCE IF NOT EXISTS "creeper"."type_test_serial2_type_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 32767
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for type_test_serial4_type_seq
-- ----------------------------
CREATE SEQUENCE IF NOT EXISTS "creeper"."type_test_serial4_type_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for type_test_serial8_type_seq
-- ----------------------------
CREATE SEQUENCE IF NOT EXISTS "creeper"."type_test_serial8_type_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for uuid_iden_pk_id_sec_seq
-- ----------------------------
CREATE SEQUENCE IF NOT EXISTS "creeper"."uuid_iden_pk_id_sec_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Table structure for classmate
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."classmate" (
  "teacher_id" uuid NOT NULL,
  "student_id" uuid NOT NULL,
  "grade_id" uuid NOT NULL,
  "create_time" timestamp(6)
)
;

-- ----------------------------
-- Table structure for grade
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."grade" (
  "id" uuid NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "create_time" timestamp(6) NOT NULL
)
;
COMMENT ON COLUMN "creeper"."grade"."name" IS '班级名称';
COMMENT ON TABLE "creeper"."grade" IS '班级';

-- ----------------------------
-- Table structure for iden_nopk
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."iden_nopk" (
  "id" uuid NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default",
  "age" int4,
  "name_no" int4 NOT NULL DEFAULT nextval('"creeper".iden_nopk_name_no_seq'::regclass)
)
;
COMMENT ON COLUMN "creeper"."iden_nopk"."name" IS '名字';
COMMENT ON COLUMN "creeper"."iden_nopk"."age" IS '年龄';

-- ----------------------------
-- Table structure for iden_pk
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."iden_pk" (
  "name" varchar(255) COLLATE "pg_catalog"."default",
  "age" int4,
  "id" int4 NOT NULL DEFAULT nextval('"creeper".iden_pk_id_seq'::regclass)
)
;
COMMENT ON COLUMN "creeper"."iden_pk"."name" IS '名字';
COMMENT ON COLUMN "creeper"."iden_pk"."age" IS '年龄';

-- ----------------------------
-- Table structure for people
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."people" (
  "id" uuid NOT NULL,
  "age" int4 NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "sex" bool,
  "create_time" timestamp(6) NOT NULL,
  "address" varchar(255) COLLATE "pg_catalog"."default",
  "address_detail" jsonb NOT NULL DEFAULT '{}'::jsonb,
  "state" "creeper"."data_state" DEFAULT '正常'::creeper.data_state
)
;
COMMENT ON COLUMN "creeper"."people"."age" IS '年龄';
COMMENT ON COLUMN "creeper"."people"."name" IS '姓名';
COMMENT ON COLUMN "creeper"."people"."sex" IS '性别';
COMMENT ON COLUMN "creeper"."people"."address" IS '家庭住址';
COMMENT ON COLUMN "creeper"."people"."address_detail" IS '详细住址';

-- ----------------------------
-- Table structure for student
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."student" (
  "stu_no" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "grade_id" uuid NOT NULL,
  "people_id" uuid NOT NULL,
  "create_time" timestamp(6) NOT NULL,
  "id" uuid NOT NULL
)
;
COMMENT ON COLUMN "creeper"."student"."stu_no" IS '学号';

-- ----------------------------
-- Table structure for teacher
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."teacher" (
  "teacher_no" varchar(32) COLLATE "pg_catalog"."default" NOT NULL,
  "people_id" uuid NOT NULL,
  "create_time" timestamp(6) NOT NULL,
  "id" uuid NOT NULL
)
;
COMMENT ON COLUMN "creeper"."teacher"."teacher_no" IS '学号';

-- ----------------------------
-- Table structure for type_test
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."type_test" (
  "id" uuid NOT NULL,
  "bit_type" bit(1),
  "bool_type" bool,
  "box_type" box,
  "bytea_type" bytea,
  "char_type" char(1) COLLATE "pg_catalog"."default",
  "cidr_type" cidr,
  "circle_type" circle,
  "date_type" date,
  "decimal_type" numeric(4),
  "float4_type" float4,
  "float8_type" float8,
  "inet_type" inet,
  "int2_type" int2,
  "int4_type" int4,
  "int8_type" int8,
  "interval_type" interval(6),
  "json_type" json,
  "jsonb_type" jsonb,
  "line_type" line,
  "lseg_type" lseg,
  "macaddr_type" macaddr,
  "money_type" money,
  "path_type" path,
  "point_type" point,
  "polygon_type" polygon,
  "text_type" text COLLATE "pg_catalog"."default",
  "time_type" time(6),
  "timestamp_type" timestamp(6),
  "timestamptz_type" timestamptz(6),
  "timetz_type" timetz(6),
  "tsquery_type" tsquery,
  "tsvector_type" tsvector,
  "varbit_type" varbit,
  "varchar_type" varchar COLLATE "pg_catalog"."default",
  "xml_type" xml,
  "hstore_type" "public"."hstore",
  "bit_length_type" bit(8),
  "array_type" int4[],
  "uuid_array_type" uuid[],
  "varchar_array_type" varchar(255)[] COLLATE "pg_catalog"."default",
  "enum_type" "creeper"."data_state" DEFAULT '正常'::creeper.data_state,
  "composite_type" "creeper"."info",
  "serial2_type" int2 NOT NULL DEFAULT nextval('"creeper".type_test_serial2_type_seq'::regclass),
  "serial4_type" int4 NOT NULL DEFAULT nextval('"creeper".type_test_serial4_type_seq'::regclass),
  "serial8_type" int8 NOT NULL DEFAULT nextval('"creeper".type_test_serial8_type_seq'::regclass)
)
;

-- ----------------------------
-- Table structure for uuid_iden_pk
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."uuid_iden_pk" (
  "id" uuid NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default",
  "age" int4,
  "id_sec" int4 NOT NULL DEFAULT nextval('"creeper".uuid_iden_pk_id_sec_seq'::regclass)
)
;
COMMENT ON COLUMN "creeper"."uuid_iden_pk"."name" IS '名字';
COMMENT ON COLUMN "creeper"."uuid_iden_pk"."age" IS '年龄';

-- ----------------------------
-- Table structure for uuid_pk
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."uuid_pk" (
  "id" uuid NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default",
  "age" int4
)
;
COMMENT ON COLUMN "creeper"."uuid_pk"."name" IS '名字';
COMMENT ON COLUMN "creeper"."uuid_pk"."age" IS '年龄';

-- ----------------------------
-- Table structure for uid_composite_pk
-- ----------------------------
CREATE TABLE IF NOT EXISTS "creeper"."uid_composite_pk" (
  "uid1" uuid NOT NULL,
  "uid2" uuid NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "creeper"."iden_nopk_name_no_seq"
OWNED BY "creeper"."iden_nopk"."name_no";
SELECT setval('"creeper"."iden_nopk_name_no_seq"', 7, true);
ALTER SEQUENCE "creeper"."iden_pk_id_seq"
OWNED BY "creeper"."iden_pk"."id";
SELECT setval('"creeper"."iden_pk_id_seq"', 53, true);
ALTER SEQUENCE "creeper"."type_test_serial2_type_seq"
OWNED BY "creeper"."type_test"."serial2_type";
SELECT setval('"creeper"."type_test_serial2_type_seq"', 13, true);
ALTER SEQUENCE "creeper"."type_test_serial4_type_seq"
OWNED BY "creeper"."type_test"."serial4_type";
SELECT setval('"creeper"."type_test_serial4_type_seq"', 13, true);
ALTER SEQUENCE "creeper"."type_test_serial8_type_seq"
OWNED BY "creeper"."type_test"."serial8_type";
SELECT setval('"creeper"."type_test_serial8_type_seq"', 13, true);
ALTER SEQUENCE "creeper"."uuid_iden_pk_id_sec_seq"
OWNED BY "creeper"."uuid_iden_pk"."id_sec";
SELECT setval('"creeper"."uuid_iden_pk_id_sec_seq"', 28, true);

-- ----------------------------
-- Primary Key structure for table classmate
-- ----------------------------
ALTER TABLE "creeper"."classmate" ADD CONSTRAINT "classmate_pkey" PRIMARY KEY ("teacher_id", "student_id", "grade_id");

-- ----------------------------
-- Primary Key structure for table grade
-- ----------------------------
ALTER TABLE "creeper"."grade" ADD CONSTRAINT "grade_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table iden_nopk
-- ----------------------------
ALTER TABLE "creeper"."iden_nopk" ADD CONSTRAINT "iden_nopk_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table people
-- ----------------------------
ALTER TABLE "creeper"."people" ADD CONSTRAINT "people_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Indexes structure for table student
-- ----------------------------
CREATE INDEX "fki_fk_grade" ON "creeper"."student" USING btree (
  "grade_id" "pg_catalog"."uuid_ops" ASC NULLS LAST
);

-- ----------------------------
-- Uniques structure for table student
-- ----------------------------
ALTER TABLE "creeper"."student" ADD CONSTRAINT "student_stu_no_key" UNIQUE ("stu_no");
ALTER TABLE "creeper"."student" ADD CONSTRAINT "student_people_id_key" UNIQUE ("people_id");

-- ----------------------------
-- Primary Key structure for table student
-- ----------------------------
ALTER TABLE "creeper"."student" ADD CONSTRAINT "student_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table teacher
-- ----------------------------
ALTER TABLE "creeper"."teacher" ADD CONSTRAINT "teacher_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table type_test
-- ----------------------------
ALTER TABLE "creeper"."type_test" ADD CONSTRAINT "type_test_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table uuid_pk
-- ----------------------------
ALTER TABLE "creeper"."uuid_pk" ADD CONSTRAINT "uuid_pk_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table uuid_iden_pk
-- ----------------------------
ALTER TABLE "creeper"."uuid_iden_pk" ADD CONSTRAINT "uuid_iden_pk_pkey" PRIMARY KEY ("id", "id_sec");

-- ----------------------------
-- Primary Key structure for table iden_pk
-- ----------------------------
ALTER TABLE "creeper"."iden_pk" ADD CONSTRAINT "iden_pk_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table uid_composite_pk
-- ----------------------------
ALTER TABLE "creeper"."uid_composite_pk" ADD CONSTRAINT "uid_composite_pk_pkey" PRIMARY KEY ("uid1", "uid2");
