DROP PROCEDURE IF EXISTS sp_RenameTraindetailTable;
CREATE PROCEDURE sp_RenameTraindetailTable(in vTableName_back VARCHAR(50))
BEGIN 
DECLARE vTrainDetail_ID VARCHAR(36);#在备份表中存储的延迟数据ID
#DECLARE vTableName_back VARCHAR(200);#备份表名

#step1
/*
#WEB需要遍历各表，不需要old表，暂保留此代码，留作后用，2017-08-08，edit by Susie
IF Not EXISTS(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA='sartas' and TABLE_NAME='traindetail_old')THEN
SET vTableName_back='traindetail_old';
END IF;*/

SET @sqlstr1 = CONCAT('RENAME TABLE traindetail TO ',vTableName_back);
PREPARE stmt1 FROM @sqlstr1 ;
EXECUTE stmt1 ;
DEALLOCATE PREPARE stmt1;
SELECT @sqlstr1;

#step2
CREATE TABLE IF NOT EXISTS `traindetail` (
  `TrainDetail_ID` char(36) NOT NULL,
  `Train_ID` char(36) DEFAULT NULL,
  `TrainDetail_OrderNo` int(11) DEFAULT NULL,
  `TrainDetail_No` varchar(50) DEFAULT NULL,
  `TrainDetail_Remark` varchar(50) DEFAULT NULL,
  `TrainDetail_IF` int(11) DEFAULT '0',
  `vehicletype` varchar(50) DEFAULT NULL,
  `startstation` varchar(50) DEFAULT NULL,
  `endstation` varchar(50) DEFAULT NULL,
  `GoodsName` varchar(50) DEFAULT NULL,
  `Ziz` decimal(10,2) DEFAULT NULL,
  `Zaiz` decimal(10,2) DEFAULT NULL,
  `HC` decimal(10,2) DEFAULT NULL,
  `smoke_result` varchar(50) DEFAULT NULL,
  `DealTime` datetime DEFAULT NULL,
  `AlarmLevel` varchar(2) DEFAULT NULL,
  `AlarmTime` datetime DEFAULT NULL,
  PRIMARY KEY (`TrainDetail_ID`),
  KEY `Train_ID` (`Train_ID`),
  KEY `TrainDetail_No` (`TrainDetail_No`),
  KEY `idx_TrainDetail_If` (`TrainDetail_IF`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=gb2312;


#step3
SET @sqlstr2=CONCAT('INSERT INTO traindetail SELECT * FROM ', vTableName_back ,
' where Train_ID IN (SELECT Train_ID from train where Train_ComeDate>=  ','\'',CONCAT(CURDATE(),' 00:00:00'),'\')');
PREPARE stmt2 FROM @sqlstr2 ;
EXECUTE stmt2 ;
DEALLOCATE PREPARE stmt2;
SELECT  @sqlstr2;

#step4
SET @sqlstr3=CONCAT('DELETE FROM ',vTableName_back, ' where Train_ID IN (SELECT Train_ID from train where Train_ComeDate>= ', '\'',CONCAT(CURDATE(),' 00:00:00'),'\')');
PREPARE stmt3 FROM @sqlstr3 ;
EXECUTE stmt3 ;
DEALLOCATE PREPARE stmt3;
SELECT @sqlstr3;
END;

