/*
 Navicat Premium Data Transfer

 Source Server         : mysql
 Source Server Type    : MySQL
 Source Server Version : 50732
 Source Schema         : demo

 Target Server Type    : MySQL
 Target Server Version : 50732
 File Encoding         : 65001

 Date: 02/09/2021 17:20:09
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for composite_uid_iid_pk
-- ----------------------------
DROP TABLE IF EXISTS `composite_uid_iid_pk`;
CREATE TABLE `composite_uid_iid_pk`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `u_id` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`id`, `u_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 32 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for composite_uid_pk
-- ----------------------------
DROP TABLE IF EXISTS `composite_uid_pk`;
CREATE TABLE `composite_uid_pk`  (
  `id` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `u_id` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`id`, `u_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for id_pk_ucolumn
-- ----------------------------
DROP TABLE IF EXISTS `id_pk_ucolumn`;
CREATE TABLE `id_pk_ucolumn`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `u_column` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '唯一约束列',
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_unique_column`(`u_column`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 65 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for iid_pk
-- ----------------------------
DROP TABLE IF EXISTS `iid_pk`;
CREATE TABLE `iid_pk`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 51 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for people
-- ----------------------------
DROP TABLE IF EXISTS `people`;
CREATE TABLE `people`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '主键id\r\n唯一键',
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '姓名',
  `age` int(11) NULL DEFAULT NULL COMMENT '年龄',
  `stu_no` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `idx_stu_no`(`stu_no`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 66 CHARACTER SET = utf8 COLLATE = utf8_general_ci COMMENT = '测试用表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for student
-- ----------------------------
DROP TABLE IF EXISTS `student`;
CREATE TABLE `student`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `people_id` int(11) NOT NULL,
  `stu_no` int(11) NULL DEFAULT NULL,
  `class_name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `teacher_name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `fk_people`(`people_id`) USING BTREE,
  CONSTRAINT `fk_people` FOREIGN KEY (`people_id`) REFERENCES `people` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 11 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for test_ext
-- ----------------------------
DROP TABLE IF EXISTS `test_ext`;
CREATE TABLE `test_ext`  (
  `id` int(11) NOT NULL,
  `bio` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  CONSTRAINT `fk_test_id` FOREIGN KEY (`id`) REFERENCES `people` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for type_test
-- ----------------------------
DROP TABLE IF EXISTS `type_test`;
CREATE TABLE `type_test`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bigint_t` bigint(255) NULL DEFAULT NULL,
  `binary_t` binary(255) NULL DEFAULT NULL,
  `bit_t` bit(8) NULL DEFAULT NULL,
  `blob_t` blob NULL,
  `char_t` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `date_t` date NULL DEFAULT NULL,
  `datetime_t` datetime(3) NULL DEFAULT NULL,
  `decimal_t` decimal(10, 2) NULL DEFAULT NULL,
  `double_t` double(10, 2) NULL DEFAULT NULL,
  `enum_t` enum('已删除','正常') CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `float_t` float(10, 2) NULL DEFAULT NULL,
  `geometry_t` geometry NULL,
  `geometrycollection_t` geometrycollection NULL,
  `integer_t` int(11) NULL DEFAULT NULL,
  `json_t` json NULL,
  `linestring_t` linestring NULL,
  `numeric_t` decimal(10, 2) NULL DEFAULT NULL,
  `point_t` point NULL,
  `polygon_t` polygon NULL,
  `real_t` double(10, 2) NULL DEFAULT NULL,
  `set_t` set('1','2','3') CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '',
  `smallint_t` smallint(255) NULL DEFAULT NULL,
  `text_t` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `time_t` time(3) NULL DEFAULT NULL,
  `timestamp_t` timestamp(3) NULL DEFAULT NULL,
  `tinyblob_t` tinyblob NULL,
  `tinyint_t` tinyint(4) NULL DEFAULT NULL,
  `tinytext_t` tinytext CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `varbinary_t` varbinary(255) NULL DEFAULT NULL,
  `varchar_t` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `year_t` year NULL DEFAULT NULL,
  `multilinestring_t` multilinestring NULL,
  `multipolygon_t` multipolygon NULL,
  `mulitpoint_t` multipoint NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1497194872 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for uid_pk
-- ----------------------------
DROP TABLE IF EXISTS `uid_pk`;
CREATE TABLE `uid_pk`  (
  `id` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for uuid_pk
-- ----------------------------
DROP TABLE IF EXISTS `uuid_pk`;
CREATE TABLE `uuid_pk`  (
  `uid` char(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `name` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`uid`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- View structure for test_view
-- ----------------------------
DROP VIEW IF EXISTS `test_view`;
CREATE ALGORITHM = UNDEFINED SQL SECURITY DEFINER VIEW `test_view` AS select `people`.`id` AS `id`,`people`.`name` AS `name`,`people`.`age` AS `age`,`people`.`stu_no` AS `stu_no` from `people` where (`people`.`name` = '111');

SET FOREIGN_KEY_CHECKS = 1;
