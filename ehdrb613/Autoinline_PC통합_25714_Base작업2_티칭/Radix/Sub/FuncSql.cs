using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;


namespace Radix
{
    /** @brief SQL 처리 함수 선언 */
    class FuncSql
    {

        /** 
         * @brief 클래스 내 로컬 디버그 문자열 출력
         * @param str 출력할 문자열
         */
        private static void debug(string str)
        {
            //Util.Debug("FuncSql : " + str);
        }


        /** 
         * @brief 데이터베이스 버전 관리.
         *      최종 소스에 맞춰 데이터베이스 자동 변경
         *      프로젝트마다 따로 선언을 해야 할 필요 있음
         */
        public static void UpdateDatabase() // 데이터베이스 버전 관리.최종 소스에 맞춰 데이터베이스 자동 변경
        {
            if (!GlobalVar.UseMsSQL)
            {
                // MsSQL 미사용시는 동작 안 함
                return;
            }

            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                //debug("database connection error");
                return;
            }

            try
            {
                switch (GlobalVar.SWName)
                {
                    #region 모듈형 오토인라인
                    case "AutoInline":
                        #region Version table 확인
                        string sql = "if not exists (select * from information_schema.tables where table_name='DatabaseVersion') " +
                                        "begin " +
                                            "CREATE TABLE[dbo].[DatabaseVersion]( " +
                                                "[Date][nvarchar](10) NOT NULL, " +
                                                "[Time] [nvarchar](10) NOT NULL, " +
                                                "[Version] [smallint] NOT NULL " +
                                            ") " +
                                        "end";
                        if (!GlobalVar.Sql.Execute(sql))
                        {
                            //debug("database connection error");
                            return;
                        }

                        sql = "select isnull(max([Version]), 0) from [DatabaseVersion]";
                        string[,] rs = GlobalVar.Sql.Read(sql);
                        if (rs == null)
                        {
                            //debug("database version check error");
                            return;
                        }
                        try
                        {
                            //if (rs.Read())
                            if (rs.GetLength(0) > 0)
                            {
                                GlobalVar.MsSql_Version = int.Parse(rs[0, 0].ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            //debug("database version check error");
                            debug(ex.ToString());
                            debug(ex.StackTrace);
                        }
                        //rs.Close();
                        #endregion


                        #region Version 1 초기 테이블
                        if (GlobalVar.MsSql_Version < 1)
                        {
                            #region SystemError
                            sql = "if not exists (select * from information_schema.tables where table_name='SystemError') " +
                                            "begin " +
                                                "CREATE TABLE[dbo].[SystemError]( " +
                                                    "[Date][nvarchar](10) NOT NULL, " +
                                                    "[Time] [nvarchar](10) NOT NULL, " +
                                                    "[Part] [nvarchar](20) NOT NULL, " +
                                                    "[ErrorCode] [smallint] NOT NULL, " +
                                                    "[ErrorName] [nvarchar](50) NOT NULL " +
                                                ") " +
                                            "end";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("SystemError table error");
                                return;
                            }
                            #endregion

                            #region PinLog
                            sql = "if not exists (select * from information_schema.tables where table_name='PinLog') " +
                                            "begin " +
                                                "CREATE TABLE[dbo].[PinLog]( " +
                                                    "[Date][nvarchar](10) NOT NULL, " +
                                                    "[Time] [nvarchar](10) NOT NULL, " +
                                                    "[Part] [smallint] NOT NULL, " +
                                                    "[TestCount] [int] NOT NULL, " +
                                                    "[Using] [bit] NOT NULL " +
                                                ") " +
                                            "end";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("PinLog table error");
                                return;
                            }
                            #endregion

                            #region CommLog
                            sql = "if not exists (select * from information_schema.tables where table_name='CommLog') " +
                                            "begin " +
                                                "CREATE TABLE[dbo].[CommLog]( " +
                                                    "[Date][nvarchar](10) NOT NULL, " +
                                                    "[Time] [nvarchar](10) NOT NULL, " +
                                                    "[Site] [smallint] NOT NULL, " +
                                                    "[Array] [smallint] NOT NULL, " +
                                                    "[Content] [nvarchar](20) NOT NULL, " +
                                                    "[Result] [nvarchar](20) NOT NULL " +
                                                ") " +
                                            "end";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("CommLog table error");
                                return;
                            }
                            #endregion

                            #region DefectLog
                            sql = "if not exists (select * from information_schema.tables where table_name='DefectLog') " +
                                            "begin " +
                                                "CREATE TABLE[dbo].[DefectLog]( " +
                                                    "[Date][nvarchar](10) NOT NULL, " +
                                                    "[Time] [nvarchar](10) NOT NULL, " +
                                                    "[Site] [smallint] NOT NULL, " +
                                                    "[Array] [smallint] NOT NULL, " +
                                                    "[DefectCode] [smallint] NOT NULL, " +
                                                    "[DefectName] [nvarchar](50) NOT NULL " +
                                                ") " +
                                            "end";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("CommLog table error");
                                return;
                            }
                            #endregion

                            #region 버전 정보 저장
                            sql = "insert into [DatabaseVersion] " +
                                            "values (CONVERT(CHAR(8), getdate(), 112), " +
                                                " CONVERT(CHAR(8), getdate(), 108), " +
                                                ++GlobalVar.MsSql_Version + ")";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("DatabaseVersion table error");
                                return;
                            }
                            #endregion
                        }
                        #endregion

                        #region Version 2 
                        if (GlobalVar.MsSql_Version < 2)
                        {
                            #region SystemError
                            sql = "alter table [SystemError] " +
                                  "alter column [Description] nvarchar(200)";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("SystemError table error");
                                return;
                            }
                            #endregion

                            #region TestErrorCode
                            sql = "alter table [TestErrorCode] add [RetestMethod] nvarchar(10)";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("SystemError table error");
                                return;
                            }
                            sql = "alter table [TestErrorCode] add [Retest] bit";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("SystemError table error");
                                return;
                            }
                            #endregion

                            #region 버전 정보 저장
                            sql = "insert into [DatabaseVersion] " +
                                            "values (CONVERT(CHAR(8), getdate(), 112), " +
                                                " CONVERT(CHAR(8), getdate(), 108), " +
                                                ++GlobalVar.MsSql_Version + ")";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("DatabaseVersion table error");
                                return;
                            }
                            #endregion
                        }
                        #endregion

                        #region Version 3
                        if (GlobalVar.MsSql_Version < 3)
                        {
                            #region All_Site_Array
                            sql = "create view All_Site_Array as " +
                                    "select distinct[Site],[Array] " +
                                    "from[TestResult] ";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("All_Site_Array view error");
                                return;
                            }
                            #endregion
                            #region 버전 정보 저장
                            sql = "insert into [DatabaseVersion] " +
                                            "values (CONVERT(CHAR(8), getdate(), 112), " +
                                                " CONVERT(CHAR(8), getdate(), 108), " +
                                                ++GlobalVar.MsSql_Version + ")";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("DatabaseVersion table error");
                                return;
                            }
                            #endregion
                        }
                        #endregion

                        #region Version 4
                        if (GlobalVar.MsSql_Version < 4)
                        {
                            #region All_Site_Array
                            sql = "insert into [TestErrorCode] " +
                                  "values (994, " +
                                         " 'XOut', " +
                                         " '', " +
                                         " '0')";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("All_Site_Array view error");
                                return;
                            }
                            sql = "insert into [TestErrorCode] " +
                                  "values (995, " +
                                         " 'BadMark', " +
                                         " '', " +
                                         " '0')";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("All_Site_Array view error");
                                return;
                            }
                            #endregion
                            #region 버전 정보 저장
                            sql = "insert into [DatabaseVersion] " +
                                            "values (CONVERT(CHAR(8), getdate(), 112), " +
                                                " CONVERT(CHAR(8), getdate(), 108), " +
                                                ++GlobalVar.MsSql_Version + ")";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("DatabaseVersion table error");
                                return;
                            }
                            #endregion
                        }
                        #endregion

                        #region Version 5 - TestErrorCode 업데이트
                        if (GlobalVar.MsSql_Version < 5)
                        {
                            #region TestErrorCode
                            sql = "delete from TestErrorCode";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("All_Site_Array view error");
                                return;
                            }

                            string[] data =
                            {
                        "insert into TestErrorCode values ('00','MES_ERROR','F','1')",
                        "insert into TestErrorCode values ('01','BUILD','F','1')",
                        "insert into TestErrorCode values ('02', 'KNOX', 'F', '1')",
                        "insert into TestErrorCode values ('03', 'FT_UPLOAD', 'F', '1')",
                        "insert into TestErrorCode values ('04', 'MIC', 'F', '1')",
                        "insert into TestErrorCode values ('05', 'CODEC', 'F', '1')",
                        "insert into TestErrorCode values ('06', 'LOWBAT', 'F', '1')",
                        "insert into TestErrorCode values ('07', 'SLEEP', 'F', '1')",
                        "insert into TestErrorCode values ('08', 'WAKEUP', 'F', '1')",
                        "insert into TestErrorCode values ('09', 'IDC', 'F', '1')",
                        "insert into TestErrorCode values ('10', 'BT', 'F', '1')",
                        "insert into TestErrorCode values ('11', 'BT_SC', 'F', '1')",
                        "insert into TestErrorCode values ('12', 'WIFIFW', 'F', '1')",
                        "insert into TestErrorCode values ('13', '2GTX', 'F', '1')",
                        "insert into TestErrorCode values ('14', 'OIS', 'F', '1')",
                        "insert into TestErrorCode values ('15', 'FLICKER', 'F', '1')",
                        "insert into TestErrorCode values ('16', 'RTC', 'F', '1')",
                        "insert into TestErrorCode values ('17', 'KEYSH', 'F', '1')",
                        "insert into TestErrorCode values ('18', 'AUDIOAMP', 'F', '1')",
                        "insert into TestErrorCode values ('19', 'MOTOR', 'F', '1')",
                        "insert into TestErrorCode values ('20', 'STATICNV', 'F', '1')",
                        "insert into TestErrorCode values ('21', 'HALL IC', 'F', '1')",
                        "insert into TestErrorCode values ('22', 'POWER_S', 'F', '1')",
                        "insert into TestErrorCode values ('23', 'MFC', 'F', '1')",
                        "insert into TestErrorCode values ('24', 'TSPVER', 'F', '1')",
                        "insert into TestErrorCode values ('25', 'SWVER', 'F', '1')",
                        "insert into TestErrorCode values ('26', 'MIPI', 'F', '1')",
                        "insert into TestErrorCode values ('27', 'SECURE', 'F', '1')",
                        "insert into TestErrorCode values ('28', 'WIFION', 'F', '1')",
                        "insert into TestErrorCode values ('29', 'TEMP', 'F', '1')",
                        "insert into TestErrorCode values ('30', 'FLASH', 'F', '1')",
                        "insert into TestErrorCode values ('31', 'UV_SEN', 'F', '1')",
                        "insert into TestErrorCode values ('32', 'T_DMB', 'F', '1')",
                        "insert into TestErrorCode values ('33', 'LIGHT', 'F', '1')",
                        "insert into TestErrorCode values ('34', 'RFBACK', 'F', '1')",
                        "insert into TestErrorCode values ('35', 'ZWAVE', 'F', '1')",
                        "insert into TestErrorCode values ('36', 'ZIGBEE', 'F', '1')",
                        "insert into TestErrorCode values ('37', 'LEAKAGE', 'F', '1')",
                        "insert into TestErrorCode values ('38', 'LTE_TX', 'F', '1')",
                        "insert into TestErrorCode values ('39', 'GRIP_SEN', 'F', '1')",
                        "insert into TestErrorCode values ('40', 'FUSING', 'F', '1')",
                        "insert into TestErrorCode values ('41', 'MST', 'F', '1')",
                        "insert into TestErrorCode values ('42', 'SPEAKER', 'F', '1')",
                        "insert into TestErrorCode values ('43', 'DPSWIC', 'F', '1')",
                        "insert into TestErrorCode values ('44', 'MAX_TX', 'F', '1')",
                        "insert into TestErrorCode values ('45', 'SIMTRAY', 'F', '1')",
                        "insert into TestErrorCode values ('46', 'BATDIS', 'F', '1')",
                        "insert into TestErrorCode values ('47', 'RFIC_PLL', 'F', '1')",
                        "insert into TestErrorCode values ('48', 'SD_CARD', 'F', '1')",
                        "insert into TestErrorCode values ('49', 'M_SIZE', 'F', '1')",
                        "insert into TestErrorCode values ('50', 'DGS', 'F', '1')",
                        "insert into TestErrorCode values ('51', 'COMPAN', 'F', '1')",
                        "insert into TestErrorCode values ('52', '5GRED', 'F', '1')",
                        "insert into TestErrorCode values ('53', 'NAD', 'F', '1')",
                        "insert into TestErrorCode values ('54', 'M_CRC', 'F', '1')",
                        "insert into TestErrorCode values ('55', 'PDASW', 'F', '1')",
                        "insert into TestErrorCode values ('56', 'SPEED', 'F', '1')",
                        "insert into TestErrorCode values ('57', 'GYRO', 'F', '1')",
                        "insert into TestErrorCode values ('58', 'GM', 'F', '1')",
                        "insert into TestErrorCode values ('59', 'BARO', 'F', '1')",
                        "insert into TestErrorCode values ('60', 'EPEN', 'F', '1')",
                        "insert into TestErrorCode values ('61', 'IFPMIC', 'F', '1')",
                        "insert into TestErrorCode values ('62', 'HW_ID', 'F', '1')",
                        "insert into TestErrorCode values ('63', 'LTE_TX', 'F', '1')",
                        "insert into TestErrorCode values ('64', 'BCMSG', 'F', '1')",
                        "insert into TestErrorCode values ('65', 'WIFIAP', 'F', '1')",
                        "insert into TestErrorCode values ('66', 'CORE', 'F', '1')",
                        "insert into TestErrorCode values ('67', 'SHUB', 'F', '1')",
                        "insert into TestErrorCode values ('68', 'BCODE', 'F', '1')",
                        "insert into TestErrorCode values ('69', 'R_SIZE', 'F', '1')",
                        "insert into TestErrorCode values ('70', 'GPSON', 'F', '1')",
                        "insert into TestErrorCode values ('71', 'G_CNO', 'F', '1')",
                        "insert into TestErrorCode values ('72', 'FM_ON', 'F', '1')",
                        "insert into TestErrorCode values ('73', 'HDMI', 'F', '1')",
                        "insert into TestErrorCode values ('74', 'OTG', 'F', '1')",
                        "insert into TestErrorCode values ('75', 'APTEMP', 'F', '1')",
                        "insert into TestErrorCode values ('76', 'F/T USTOP', 'F', '0')",
                        "insert into TestErrorCode values ('77', 'GEST_S', 'F', '1')",
                        "insert into TestErrorCode values ('78', 'PROX_S', 'F', '1')",
                        "insert into TestErrorCode values ('79', 'HRM', 'F', '1')",
                        "insert into TestErrorCode values ('80', 'NFC', 'F', '1')",
                        "insert into TestErrorCode values ('81', 'Q_CHA', 'F', '1')",
                        "insert into TestErrorCode values ('82', 'QDAF', 'F', '1')",
                        "insert into TestErrorCode values ('83', 'EARJACK', 'F', '1')",
                        "insert into TestErrorCode values ('84', 'EM_LOW', 'A', '1')",
                        "insert into TestErrorCode values ('85', 'ERM_UNKNOWN', 'A', '1')",
                        "insert into TestErrorCode values ('86', 'ERM_AP', 'A', '1')",
                        "insert into TestErrorCode values ('87', 'TCP_IP', 'A', '1')",
                        "insert into TestErrorCode values ('88', 'EM_DL', 'A', '1')",
                        "insert into TestErrorCode values ('89', 'ERM DRAM', 'A', '1')",
                        "insert into TestErrorCode values ('90', 'EM_HIGH', 'A', '1')",
                        "insert into TestErrorCode values ('91', 'NAD', 'A', '1')",
                        "insert into TestErrorCode values ('92', 'NM_USB', 'A', '1')",
                        "insert into TestErrorCode values ('93', 'NM_DL', 'A', '1')",
                        "insert into TestErrorCode values ('94', 'ERM SET', 'A', '1')",
                        "insert into TestErrorCode values ('95', 'DL_UPLOAD', 'A', '1')",
                        "insert into TestErrorCode values ('96', 'ERM PMIC', 'A', '1')",
                        "insert into TestErrorCode values ('97', 'ATCMD', 'A', '1')",
                        "insert into TestErrorCode values ('98', 'ERM UFS', 'A', '1')",
                        "insert into TestErrorCode values ('99', 'SHORT', 'A', '1')",
                        "insert into TestErrorCode values ('100', 'DL_USTOP', '', '0')",
                        "insert into TestErrorCode values ('994', 'XOut', '', '0')",
                        "insert into TestErrorCode values ('995', 'BadMark', '', '0')",
                        "insert into TestErrorCode values ('996', 'COMMAND_NG', '', '0')",
                        "insert into TestErrorCode values ('997', 'TEST_CANCEL', '', '0')",
                        "insert into TestErrorCode values ('998', 'USER_CANCEL', '', '0')",
                        "insert into TestErrorCode values ('999', 'TEST_TIMEOUT', '', '0')"
                    };
                            for (int i = 0; i < data.Length; i++)
                                if (!GlobalVar.Sql.Execute(data[i]))
                                {
                                    //debug("TestErrorCode insert error : " + data[i]);
                                    return;
                                }
                            #endregion
                            #region 버전 정보 저장
                            sql = "insert into [DatabaseVersion] " +
                                            "values (CONVERT(CHAR(8), getdate(), 112), " +
                                                " CONVERT(CHAR(8), getdate(), 108), " +
                                                ++GlobalVar.MsSql_Version + ")";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("DatabaseVersion table error");
                                return;
                            }
                            #endregion
                        }
                        #endregion

                        #region Version 6
                        if (GlobalVar.MsSql_Version < 6)
                        {
                            #region All_Site_Array
                            sql = "insert into TestErrorCode values ('993', 'CN_MISMATCH', '', '0')";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("All_Site_Array view error");
                                return;
                            }

                            #endregion
                            #region 버전 정보 저장
                            sql = "insert into [DatabaseVersion] " +
                                            "values (CONVERT(CHAR(8), getdate(), 112), " +
                                                " CONVERT(CHAR(8), getdate(), 108), " +
                                                ++GlobalVar.MsSql_Version + ")";
                            if (!GlobalVar.Sql.Execute(sql))
                            {
                                //debug("DatabaseVersion table error");
                                return;
                            }
                            #endregion
                        }
                        #endregion
                        break;
                    #endregion 모듈형 오토인라인
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

        }

    }
}
