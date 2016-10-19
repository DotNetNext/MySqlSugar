/*
Navicat MySQL Data Transfer

Source Server         : mysql
Source Server Version : 50547
Source Host           : localhost:3306
Source Database       : sqlsugartest

Target Server Type    : MYSQL
Target Server Version : 50547
File Encoding         : 65001

Date: 2016-10-20 04:24:18
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for area
-- ----------------------------
DROP TABLE IF EXISTS `area`;
CREATE TABLE `area` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM AUTO_INCREMENT=5 DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of area
-- ----------------------------
INSERT INTO `area` VALUES ('1', '上海');
INSERT INTO `area` VALUES ('2', '北京');
INSERT INTO `area` VALUES ('3', '南通');
INSERT INTO `area` VALUES ('4', '日本');

-- ----------------------------
-- Table structure for inserttest
-- ----------------------------
DROP TABLE IF EXISTS `inserttest`;
CREATE TABLE `inserttest` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `v1` varchar(255) DEFAULT NULL,
  `v2` varchar(255) DEFAULT NULL,
  `v3` varchar(255) DEFAULT NULL,
  `int1` int(255) DEFAULT NULL,
  `d1` datetime DEFAULT NULL,
  `txt` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of inserttest
-- ----------------------------

-- ----------------------------
-- Table structure for school
-- ----------------------------
DROP TABLE IF EXISTS `school`;
CREATE TABLE `school` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(500) NOT NULL,
  `AreaId` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=7 DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of school
-- ----------------------------
INSERT INTO `school` VALUES ('1', '北大青鸟', '1');
INSERT INTO `school` VALUES ('2', 'IT清华', '2');
INSERT INTO `school` VALUES ('3', '全智', '3');
INSERT INTO `school` VALUES ('4', '黑马', '2');
INSERT INTO `school` VALUES ('5', '幼儿园', '3');
INSERT INTO `school` VALUES ('6', '蓝翔', '1');

-- ----------------------------
-- Table structure for student
-- ----------------------------
DROP TABLE IF EXISTS `student`;
CREATE TABLE `student` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(500) DEFAULT NULL,
  `sch_id` int(11) DEFAULT NULL COMMENT '学校编号',
  `sex` varchar(500) DEFAULT NULL,
  `isOk` bit(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=9 DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of student
-- ----------------------------
INSERT INTO `student` VALUES ('1', '小杰', '1', 'boy', '');
INSERT INTO `student` VALUES ('2', '小明', '2', 'grid', '\0');
INSERT INTO `student` VALUES ('3', '张三', '3', 'boy', '');
INSERT INTO `student` VALUES ('4', '李四', '2', 'grid', '\0');
INSERT INTO `student` VALUES ('5', '王五', '3', 'boy', '');
INSERT INTO `student` VALUES ('6', '小姐', '1', 'grid', '\0');
INSERT INTO `student` VALUES ('7', '小捷', '3', 'grid', '');
INSERT INTO `student` VALUES ('8', '小J', '1', 'grid', '');

-- ----------------------------
-- Table structure for subject
-- ----------------------------
DROP TABLE IF EXISTS `subject`;
CREATE TABLE `subject` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `studentId` int(11) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=7 DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of subject
-- ----------------------------
INSERT INTO `subject` VALUES ('1', '1', '语文');
INSERT INTO `subject` VALUES ('2', '2', '数学');
INSERT INTO `subject` VALUES ('3', '4', '.NET');
INSERT INTO `subject` VALUES ('4', '5', 'JAVA');
INSERT INTO `subject` VALUES ('5', '6', 'IOS');
INSERT INTO `subject` VALUES ('6', '7', 'PHP');

-- ----------------------------
-- Table structure for test
-- ----------------------------
DROP TABLE IF EXISTS `test`;
CREATE TABLE `test` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`Id`)
) ENGINE=MyISAM DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of test
-- ----------------------------

-- ----------------------------
-- Table structure for testupdatecolumns
-- ----------------------------
DROP TABLE IF EXISTS `testupdatecolumns`;
CREATE TABLE `testupdatecolumns` (
  `VGUID` varchar(255) NOT NULL DEFAULT '',
  `Name` varchar(255) DEFAULT NULL,
  `Name2` varchar(255) DEFAULT NULL,
  `IdentityField` int(11) NOT NULL,
  `CreateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`VGUID`)
) ENGINE=MyISAM DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of testupdatecolumns
-- ----------------------------
INSERT INTO `testupdatecolumns` VALUES ('6DF396EB-D1C8-48A5-8BE1-D58D685646A7', 'xx', 'xx2', '0', '2015-01-01 00:00:00');

-- ----------------------------
-- View structure for v_name
-- ----------------------------
DROP VIEW IF EXISTS `v_name`;
CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost`  VIEW `v_name` AS select 1 as id ;
