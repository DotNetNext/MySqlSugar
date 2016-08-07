/*
Navicat MySQL Data Transfer

Source Server         : SqlSugarTest
Source Server Version : 50540
Source Host           : localhost:3306
Source Database       : sqlsugartest

Target Server Type    : MYSQL
Target Server Version : 50540
File Encoding         : 65001

Date: 2016-08-07 19:52:09
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `school`
-- ----------------------------
DROP TABLE IF EXISTS `school`;
CREATE TABLE `school` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(500) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=3 DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of school
-- ----------------------------
INSERT INTO `school` VALUES ('1', '蓝翔2');
INSERT INTO `school` VALUES ('2', '蓝翔2');

-- ----------------------------
-- Table structure for `student`
-- ----------------------------
DROP TABLE IF EXISTS `student`;
CREATE TABLE `student` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(500) DEFAULT NULL,
  `sch_id` int(11) DEFAULT NULL,
  `sex` varchar(500) DEFAULT NULL,
  `isOk` bit(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=15 DEFAULT CHARSET=gbk;

-- ----------------------------
-- Records of student
-- ----------------------------
INSERT INTO `student` VALUES ('1', '张三', '1', '男', '');
INSERT INTO `student` VALUES ('2', '李四', '2', '女', '');
INSERT INTO `student` VALUES ('3', '哈哈', '1', '女', '');
INSERT INTO `student` VALUES ('4', '1', '1', '1', '');
INSERT INTO `student` VALUES ('5', '2', '1', '1', '');
INSERT INTO `student` VALUES ('6', '3', '1', '1', '');
INSERT INTO `student` VALUES ('7', '7', '1', '1', null);
INSERT INTO `student` VALUES ('8', '8', '1', '1', null);
INSERT INTO `student` VALUES ('9', '9', '1', '1', '');
INSERT INTO `student` VALUES ('11', 'aa', '1', '1', '');
INSERT INTO `student` VALUES ('12', '张1904701816', '0', null, '');
INSERT INTO `student` VALUES ('13', '张946902134', '0', null, '');
INSERT INTO `student` VALUES ('14', '张946902134', '0', null, '');

-- ----------------------------
-- Table structure for `testupdatecolumns`
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
