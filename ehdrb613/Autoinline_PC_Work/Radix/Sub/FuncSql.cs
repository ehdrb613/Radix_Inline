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
        public static void UpdateDatabase()
        {

            if (GlobalVar.Sql == null ||
                !GlobalVar.Sql.connected)
            {
                //debug("database connection error");
                return;
            }

            try
            {

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
                    //return;
                }

                sql = "select isnull(max([Version]), 0) from [DatabaseVersion]";
                string[,] rs = GlobalVar.Sql.Read(sql);
                if (rs == null)
                {
                    //debug("database version check error");
                    //return;
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
                    //debug(ex.ToString());
                    //debug(ex.StackTrace);
                }
                //rs.Close();
                #endregion


                #region Version 1 초기 테이블
                if (GlobalVar.MsSql_Version < 1)
                {
                    sql = "CREATE TABLE [dbo].[ArrayPin]( " +
                            "[arrayNo][smallint] NOT NULL, " +
                            "[pinNo] [smallint] NOT NULL " +
                            ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, ArrayPin");
                        //return;
                    }

                    sql = "CREATE TABLE[dbo].[CommLog]( " +
                            "[Date] [nvarchar](10) NOT NULL, " +
                            "[Time] [nvarchar](10) NOT NULL, " +
                            "[Site] [smallint] NOT NULL, " +
                            "[Array] [smallint] NOT NULL, " +
                            "[Type] [nvarchar](10) NOT NULL, " +
                            "[Content] [nvarchar](50) NOT NULL, " +
                            "[Result] [nvarchar](20) NOT NULL " +
                            ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, CommLog");
                        //return;
                    }

                    sql = "CREATE TABLE[dbo].[DefectLog]( " +
                            "[Date] [nvarchar](10) NOT NULL, " +
                            "[Time] [nvarchar](10) NOT NULL, " +
                            "[Site] [smallint] NOT NULL, " +
                            "[Array] [smallint] NOT NULL, " +
                            "[DefectCode] [smallint] NOT NULL, " +
                            "[DefectName] [nvarchar](50) NOT NULL " +
                            ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, DefectLog");
                        //return;
                    }

                    sql = "CREATE TABLE[dbo].[ModuleLog]( " +
                            "[Date] [nvarchar](10) NOT NULL, " +
                            "[Time] [nvarchar](10) NOT NULL, " +
                            "[Site] [smallint] NOT NULL, " +
                            "[ModuleID] [nvarchar](20) NOT NULL, " +
                            "[TestCount] [int] NOT NULL, " +
                            "[Using] [bit] NOT NULL " +
                            ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, ModuleLog");
                        //return;
                    }

                    sql = "CREATE TABLE[dbo].[PinLog]( " +
                            "[Date] [nvarchar](10) NOT NULL, " +
                            "[Time] [nvarchar](10) NOT NULL, " +
                            "[Site] [smallint] NOT NULL, " +
                            "[Side] [nvarchar](10) NOT NULL, " +
                            "[PinNo] [tinyint] NOT NULL, " +
                            "[PinID] [nvarchar](20) NOT NULL, " +
                            "[ArrayNo] [tinyint] NOT NULL, " +
                            "[TestCount] [int] NOT NULL, " +
                            "[NGCount] [int] NOT NULL, " +
                            "[Using] [bit] NOT NULL " +
                            ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, PinLog");
                        //return;
                    }

                    sql = "CREATE TABLE[dbo].[SystemError]( " +
                            "[Date] [nvarchar](10) NOT NULL, " +
                            "[Time] [nvarchar](10) NOT NULL, " +
                            "[Part] [nvarchar](20) NOT NULL, " +
                            "[ErrorCode] [smallint] NOT NULL, " +
                            "[ErrorName] [nvarchar](50) NOT NULL, " +
                            "[Description] [nvarchar](200) NULL, " +
                            "[Clear] [bit] NOT NULL " +
                            ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, SystemError");
                        //return;
                    }

                    sql = "CREATE TABLE[dbo].[TestErrorCode]( " +
                            "[ErrorCode] [nvarchar](3) NOT NULL, " +
                            "[ErrorName] [nvarchar](50) NOT NULL, " +
                            "[RetestMethod] [nvarchar](10) NULL, " +
                            "[Retest] [bit] NULL " +
                            ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, SystemError");
                        //return;
                    }

                    sql = "CREATE TABLE[dbo].[TestResult]( " +
                            "[Date] [nvarchar](10) NOT NULL, " +
                            "[Time] [nvarchar](10) NOT NULL, " +
                            "[Site] [smallint] NOT NULL, " +
                            "[Array] [smallint] NOT NULL, " +
                            "[Barcode] [nvarchar](20) NOT NULL, " +
                            "[Type] [nvarchar](10) NOT NULL, " +
                            "[Command_Send] [bit] NOT NULL, " +
                            "[Command_Receive] [bit] NOT NULL, " +
                            "[Command_OK] [bit] NOT NULL, " +
                            "[Test_Finish] [bit] NOT NULL, " +
                            "[Test_Pass] [bit] NOT NULL, " +
                            "[Test_Cancel] [bit] NOT NULL, " +
                            "[User_Timeout] [bit] NOT NULL, " +
                            "[Finish] [bit] NOT NULL, " +
                            "[NG] [bit] NOT NULL, " +
                            "[DefectCode] [nvarchar](3) NOT NULL, " +
                            "[TestTime] [decimal](7, 2) NOT NULL " +
                            ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, SystemError");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_Site]  DEFAULT((0)) FOR[Site]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_Array]  DEFAULT((0)) FOR[Array]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_Command_Send]  DEFAULT((0)) FOR[Command_Send]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_Command_Receive]  DEFAULT((0)) FOR[Command_Receive]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_Command_OK]  DEFAULT((0)) FOR[Command_OK]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_Test_Finish]  DEFAULT((0)) FOR[Test_Finish]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_Test_Pass]  DEFAULT((0)) FOR[Test_Pass]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_Test_Cancel]  DEFAULT((0)) FOR[Test_Cancel]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_User_Cancel]  DEFAULT((0)) FOR[User_Timeout]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_Fininsh]  DEFAULT((0)) FOR[Finish]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_NG]  DEFAULT((0)) FOR[NG]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "ALTER TABLE[dbo].[TestResult] ADD CONSTRAINT[DF_TestResult_TestTime]  DEFAULT((0)) FOR[TestTime]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 1, TestResult");
                        //return;
                    }

                    sql = "create view [dbo].[All_Site_Array] as select distinct[Site],[Array] from[TestResult] ";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 2, All_SIte_Array");
                        //return;
                    }


                    sql = "CREATE TABLE [dbo].[PCBCount]( " +
                                "[Date][nvarchar](8) NOT NULL, " +
                                "[Shift] [nvarchar](1) NOT NULL, " +
                                "[Input] [smallint] NOT NULL, " +
                                "[Pass] [smallint] NOT NULL, " +
                                "[NG] [smallint] NOT NULL " +
                            ") ON[PRIMARY]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 3, PCBCount");
                        //return;
                    }

                    sql = "ALTER TABLE [dbo].[PCBCount] ADD  CONSTRAINT [DF_PCBCount_Shift]  DEFAULT (N'A') FOR [Shift]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 3, PCBCount");
                        //return;
                    }

                    sql = "ALTER TABLE [dbo].[PCBCount] ADD  CONSTRAINT [DF_PCBCount_Input]  DEFAULT ((0)) FOR [Input]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 3, PCBCount");
                        //return;
                    }

                    sql = "ALTER TABLE [dbo].[PCBCount] ADD  CONSTRAINT [DF_PCBCount_Pass]  DEFAULT ((0)) FOR [Pass]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 3, PCBCount");
                        //return;
                    }

                    sql = "ALTER TABLE [dbo].[PCBCount] ADD  CONSTRAINT [DF_PCBCount_NG]  DEFAULT ((0)) FOR [NG]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 3, PCBCount");
                        //return;
                    }

                    #region 버전 정보 저장
                    sql = "insert into [DatabaseVersion] " +
                                                "values (CONVERT(CHAR(8), getdate(), 112), " +
                                                    " CONVERT(CHAR(8), getdate(), 108), " +
                                                    ++GlobalVar.MsSql_Version + ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("DatabaseVersion table error");
                        //return;
                    }
                    #endregion

                    sql = "CREATE TABLE [dbo].[BlockHistory]( " +
                            "[Date][nvarchar](8) NOT NULL, " +
                            "[Time] [nvarchar](8) NOT NULL, " +
                            "[Site] [smallint] NOT NULL, " +
                            "[Use] [bit] NOT NULL, " +
                            "[Content] [nvarchar](50) NULL " +
                            ") ON[PRIMARY]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 4, BlockHistory");
                        //return;
                    }



                    sql = "Alter TABLE [dbo].[PCBCount] " +
                            "Add [PBA] [bit]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 5, PCBCount");
                        //return;
                    }

                    sql = "Update [dbo].[PCBCount] " +
                            "set PBA = '0'";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 5, PCBCount");
                        //return;
                    }



                    sql = "alter table [dbo].[PCBCount] drop constraint [DF_PCBCount_Input]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 6, PCBCount");
                        //return;
                    }

                    sql = "Alter TABLE [dbo].[PCBCount] Alter COLUMN " +
                            " [Input] [int] not null";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 6, PCBCount");
                        //return;
                    }

                    sql = "alter table [dbo].[PCBCount] add CONSTRAINT [DF_PCBCount_Input]  DEFAULT ('0') FOR [Input]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 6, PCBCount");
                        //return;
                    }



                    sql = "alter table [dbo].[PCBCount] drop constraint [DF_PCBCount_Pass]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 6, PCBCount");
                        //return;
                    }

                    sql = "Alter TABLE [dbo].[PCBCount] Alter COLUMN " +
                            " [Pass] [int] not null";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 6, PCBCount");
                        //return;
                    }

                    sql = "alter table [dbo].[PCBCount] add CONSTRAINT [DF_PCBCount_Pass]  DEFAULT ('0') FOR [Pass]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 6, PCBCount");
                        //return;
                    }



                    sql = "alter table [dbo].[PCBCount] drop constraint [DF_PCBCount_NG]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 6, PCBCount");
                        //return;
                    }

                    sql = "Alter TABLE [dbo].[PCBCount] Alter COLUMN " +
                            " [NG] [int] not null";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 6, PCBCount");
                        //return;
                    }

                    sql = "alter table [dbo].[PCBCount] add CONSTRAINT [DF_PCBCount_NG]  DEFAULT ('0') FOR [NG]";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 6, PCBCount");
                        //return;
                    }




                    sql = "Alter TABLE [dbo].[BlockHistory] " +
                            "Add [Comment] [nvarchar](200)";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("SystemError table error");
                        FuncLog.WriteLog("DataBase Init Error : Version 7, BlockHistory");
                        //return;
                    }

                    #region 버전 정보 저장
                    sql = "insert into [DatabaseVersion] " +
                                                "values (CONVERT(CHAR(8), getdate(), 112), " +
                                                    " CONVERT(CHAR(8), getdate(), 108), " +
                                                    ++GlobalVar.MsSql_Version + ")";
                    if (!GlobalVar.Sql.Execute(sql))
                    {
                        //debug("DatabaseVersion table error");
                        //return;
                    }
                    #endregion
                }
                #endregion

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }

        public static void WriteSqlLog(string str)
        {
            if (!GlobalVar.SqlLog)
            {
                return;
            }

            try
            {
                // 로그는 날짜별로 텍스트 파일로 저장함
                string logPath = GlobalVar.FaPath;
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                logPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                logPath += "\\" + GlobalVar.SqlLogPath;
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                logPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                if (!File.Exists(logPath))
                {
                    FileStream fs = File.Create(logPath);
                    fs.Close();
                }
                string logText = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + str;

                FuncFile.WriteFile(logPath, logText);

            }
            catch
            { }
        }
    }
}
