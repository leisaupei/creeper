create SCHEMA if not exists "class";

create extension if not exists hstore;

DO
$$
BEGIN
  IF NOT EXISTS (SELECT 1 FROM pg_type typ INNER JOIN pg_namespace nsp ON nsp.oid = typ.typnamespace
		WHERE nsp.nspname = 'public'	AND typ.typname = 'et_data_state') 
	THEN
    CREATE TYPE "public"."et_data_state" AS ENUM ('正常','删除');
  END IF;
END;
$$
LANGUAGE plpgsql;

DO
$$
BEGIN
  IF NOT EXISTS (SELECT *FROM pg_type typ INNER JOIN pg_namespace nsp ON nsp.oid = typ.typnamespace
		WHERE nsp.nspname = 'public' AND typ.typname = 'info') 
	THEN
    CREATE TYPE  "public"."info" AS (
			"id" uuid,
			"name" varchar COLLATE "pg_catalog"."default"
		);
  END IF;
END;
$$
LANGUAGE plpgsql;

CREATE TABLE if not exists  "classmate" (
"teacher_id" uuid NOT NULL,
"student_id" uuid NOT NULL,
"grade_id" uuid NOT NULL,
"create_time" timestamp(6),
CONSTRAINT "classmate_pkey" PRIMARY KEY ("teacher_id", "grade_id", "student_id") 
)
WITHOUT OIDS;

CREATE TABLE if not exists "people" (
"id" uuid NOT NULL,
"age" int4 NOT NULL,
"name" varchar(255) COLLATE "default" NOT NULL,
"sex" bool,
"create_time" timestamp(6) NOT NULL,
"address" varchar(255) COLLATE "default",
"address_detail" jsonb NOT NULL DEFAULT '{}'::jsonb,
"state" "public"."et_data_state" NOT NULL DEFAULT '正常'::et_data_state,
CONSTRAINT "people_pkey" PRIMARY KEY ("id") 
)
WITHOUT OIDS;
COMMENT ON COLUMN "people"."age" IS '年龄';
COMMENT ON COLUMN "people"."name" IS '姓名';
COMMENT ON COLUMN "people"."sex" IS '性别';
COMMENT ON COLUMN "people"."address" IS '家庭住址';
COMMENT ON COLUMN "people"."address_detail" IS '详细住址';

CREATE TABLE if not exists "student" (
"stu_no" varchar(32) COLLATE "default" NOT NULL,
"grade_id" uuid NOT NULL,
"people_id" uuid NOT NULL,
"create_time" timestamp(6) NOT NULL,
"id" uuid NOT NULL,
CONSTRAINT "student_pkey" PRIMARY KEY ("id") ,
CONSTRAINT "unique_stu_no" UNIQUE ("stu_no"),
CONSTRAINT "unique_people_id" UNIQUE ("people_id")
)
WITHOUT OIDS;
CREATE INDEX if not exists "fki_fk_grade" ON "student" USING btree ("grade_id" "pg_catalog"."uuid_ops" ASC NULLS LAST);
COMMENT ON COLUMN "student"."stu_no" IS '学号';

CREATE TABLE if not exists  "teacher" (
"teacher_no" varchar(32) COLLATE "default" NOT NULL,
"people_id" uuid NOT NULL,
"create_time" timestamp(6) NOT NULL,
"id" uuid NOT NULL,
CONSTRAINT "student_copy1_pkey" PRIMARY KEY ("id") ,
CONSTRAINT "teacher_no_key" UNIQUE ("teacher_no"),
CONSTRAINT "people_id_key" UNIQUE ("people_id")
)
WITHOUT OIDS;
COMMENT ON COLUMN "teacher"."teacher_no" IS '学号';

CREATE SEQUENCE if not exists "public"."type_test_serial2_type_seq"  
 INCREMENT 1  
 MINVALUE 1  
 MAXVALUE 9223372036854775807  
 START 1  
 CACHE 1;  
CREATE TABLE if not exists "type_test" (
"id" uuid NOT NULL,
"bit_type" bit(1),
"bool_type" bool,
"box_type" box,
"bytea_type" bytea,
"char_type" char(1) COLLATE "default",
"cidr_type" cidr,
"circle_type" circle,
"date_type" date,
"decimal_type" numeric,
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
"serial2_type" int2 NOT NULL DEFAULT nextval('public.type_test_serial2_type_seq'::regclass),
"serial4_type" int4 NOT NULL DEFAULT nextval('public.type_test_serial2_type_seq'::regclass),
"serial8_type" int8 NOT NULL DEFAULT nextval('public.type_test_serial2_type_seq'::regclass),
"text_type" text COLLATE "default",
"time_type" time(6),
"timestamp_type" timestamp(6),
"timestamptz_type" timestamptz(6),
"timetz_type" timetz(6),
"tsquery_type" tsquery,
"tsvector_type" tsvector,
"varbit_type" varbit,
"varchar_type" varchar COLLATE "default",
"xml_type" xml,
"hstore_type" "public"."hstore",
"enum_type" "public"."et_data_state",
"composite_type" "public"."info",
"bit_length_type" bit(8),
"array_type" int4[],
"uuid_array_type" uuid[],
"varchar_array_type" varchar(255)[] COLLATE "pg_catalog"."default",
CONSTRAINT "type_test_pkey" PRIMARY KEY ("id") 
)
WITHOUT OIDS;

CREATE TABLE if not exists "class"."grade" (
"id" uuid NOT NULL,
"name" varchar(255) COLLATE "default" NOT NULL,
"create_time" timestamp(6) NOT NULL,
CONSTRAINT "grade_pkey" PRIMARY KEY ("id") 
)
WITHOUT OIDS;
COMMENT ON COLUMN "class"."grade"."name" IS '班级名称';

-- ----------------------------
-- Sequence structure for iden_nopk_name_no_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "test"."iden_nopk_name_no_seq";
CREATE SEQUENCE "test"."iden_nopk_name_no_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for iden_pk_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "test"."iden_pk_id_seq";
CREATE SEQUENCE "test"."iden_pk_id_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for uuid_iden_pk_id_sec_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "test"."uuid_iden_pk_id_sec_seq";
CREATE SEQUENCE "test"."uuid_iden_pk_id_sec_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 9223372036854775807
START 1
CACHE 1;

-- ----------------------------
-- Table structure for iden_nopk
-- ----------------------------
DROP TABLE IF EXISTS "test"."iden_nopk";
CREATE TABLE "test"."iden_nopk" (
  "id" uuid NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default",
  "age" int4,
  "name_no" int4 NOT NULL DEFAULT nextval('"test".iden_nopk_name_no_seq'::regclass)
)
;
COMMENT ON COLUMN "test"."iden_nopk"."name" IS '名字';
COMMENT ON COLUMN "test"."iden_nopk"."age" IS '年龄';

-- ----------------------------
-- Table structure for iden_pk
-- ----------------------------
DROP TABLE IF EXISTS "test"."iden_pk";
CREATE TABLE "test"."iden_pk" (
  "id" int4 NOT NULL DEFAULT nextval('"test".iden_pk_id_seq'::regclass),
  "name" varchar(255) COLLATE "pg_catalog"."default",
  "age" int4
)
;
COMMENT ON COLUMN "test"."iden_pk"."name" IS '名字';
COMMENT ON COLUMN "test"."iden_pk"."age" IS '年龄';

-- ----------------------------
-- Table structure for uuid_iden_pk
-- ----------------------------
DROP TABLE IF EXISTS "test"."uuid_iden_pk";
CREATE TABLE "test"."uuid_iden_pk" (
  "id" uuid NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default",
  "age" int4,
  "id_sec" int4 NOT NULL DEFAULT nextval('"test".uuid_iden_pk_id_sec_seq'::regclass)
)
;
COMMENT ON COLUMN "test"."uuid_iden_pk"."name" IS '名字';
COMMENT ON COLUMN "test"."uuid_iden_pk"."age" IS '年龄';

-- ----------------------------
-- Table structure for uuid_pk
-- ----------------------------
DROP TABLE IF EXISTS "test"."uuid_pk";
CREATE TABLE "test"."uuid_pk" (
  "id" uuid NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default",
  "age" int4
)
;
COMMENT ON COLUMN "test"."uuid_pk"."name" IS '名字';
COMMENT ON COLUMN "test"."uuid_pk"."age" IS '年龄';

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "test"."iden_nopk_name_no_seq"
OWNED BY "test"."iden_nopk"."name_no";
SELECT setval('"test"."iden_nopk_name_no_seq"', 3, true);
ALTER SEQUENCE "test"."iden_pk_id_seq"
OWNED BY "test"."iden_pk"."id";
SELECT setval('"test"."iden_pk_id_seq"', 7, true);
ALTER SEQUENCE "test"."uuid_iden_pk_id_sec_seq"
OWNED BY "test"."uuid_iden_pk"."id_sec";
SELECT setval('"test"."uuid_iden_pk_id_sec_seq"', 7, true);

-- ----------------------------
-- Primary Key structure for table iden_nopk
-- ----------------------------
ALTER TABLE "test"."iden_nopk" ADD CONSTRAINT "iden_nopk_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table iden_pk
-- ----------------------------
ALTER TABLE "test"."iden_pk" ADD CONSTRAINT "iden_pk_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table uuid_iden_pk
-- ----------------------------
ALTER TABLE "test"."uuid_iden_pk" ADD CONSTRAINT "uuid_iden_pk_pkey" PRIMARY KEY ("id", "id_sec");

-- ----------------------------
-- Primary Key structure for table uuid_pk
-- ----------------------------
ALTER TABLE "test"."uuid_pk" ADD CONSTRAINT "uuid_pk_pkey" PRIMARY KEY ("id");
