using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Concurrent;
namespace Radix
{

    /** 
     * @brief 로그 저장 큐에서 하나씩 저장하는 쓰레드
     *      바로 저장시 로그 저장이 많아지면 동시 저장시 로그 누락이 발생하므로 쓰레드를 별도로 작동시켜 하나씩 순서대로 저장한다.
     *      MSSQL경우도 동시에 저장시도하면 문제될 수 있어 한번에 한 쿼리만 실행하도록 한다.
     *      처리할 로그의 종류는 GlobalVar에 ConcurrentQueue로 정의하고 
     *      FuncLog에 로그 추가함수를 작성하여 해당 큐에 Enqueue하도록 한다.
     *      로그 저장 기간 경과한 파일을 삭제하는 기능도 있음
     */
    class LogThread
    {
        /*
         * LogThread.cs : 로그 저장 큐에서 하나씩 저장하는 쓰레드
         *      바로 저장시 로그 저장이 많아지면 동시 저장시 로그 누락이 발생하므로 쓰레드를 별도로 작동시켜 하나씩 순서대로 저장한다.
         *      MSSQL경우도 동시에 저장시도하면 문제될 수 있어 한번에 한 쿼리만 실행하도록 한다.
         */

        /**
         * @brief 클래스 내부에서 Call 하는 로컬 Debug
         * @param str 디버그 처리할 문자열
         * @return void
         */
        private void debug(string str) // 클래스 내부에서 Call 하는 로컬 Debug
        {
            Util.Debug("AlertThread : " + str);
        }

        /** 
         * @brief 로그 저장 큐에서 하나씩 저장하는 쓰레드 메인 함수
         *      바로 저장시 로그 저장이 많아지면 동시 저장시 로그 누락이 발생하므로 쓰레드를 별도로 작동시켜 하나씩 순서대로 저장한다.
         *      MSSQL경우도 동시에 저장시도하면 문제될 수 있어 한번에 한 쿼리만 실행하도록 한다.
         *      처리할 로그의 종류는 GlobalVar에 ConcurrentQueue로 정의하고 
         *      FuncLog에 로그 추가함수를 작성하여 해당 큐에 Enqueue하도록 한다.
         *      로그 저장 기간 경과한 파일을 삭제하는 기능도 있음
         */
        public void Run()
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            while (GlobalVar.GlobalStop == false) // 프로그램 종료 전까지
            {
                try
                {
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


                    #region 시스템 로그
                    string SystemLogPath = logPath + "\\" + GlobalVar.LogPath;
                    if (!Directory.Exists(SystemLogPath))
                    {
                        Directory.CreateDirectory(SystemLogPath);
                    }


                    SystemLogPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    //if (!File.Exists(SystemLogPath))
                    //{
                    //    FileStream fs = File.Create(SystemLogPath);
                    //    fs.Close();
                    //}

                    string systemLogText = "";
                    if (GlobalVar.LogQueue.TryDequeue(out systemLogText))
                    {
                        FuncFile.WriteFile(SystemLogPath, systemLogText);
                    }
                    #endregion

                    #region 검사 로그
                    string ValueLogPath = logPath + "\\" + GlobalVar.ValueLogPath;
                    if (!Directory.Exists(ValueLogPath))
                    {
                        Directory.CreateDirectory(ValueLogPath);
                    }


                    ValueLogPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    //if (!File.Exists(SystemLogPath))
                    //{
                    //    FileStream fs = File.Create(SystemLogPath);
                    //    fs.Close();
                    //}

                    string ValueLogText = "";
                    if (GlobalVar.ValueLogQueue.TryDequeue(out ValueLogText))
                    {
                        FuncFile.WriteFile(ValueLogPath, ValueLogText);
                    }
                    #endregion

                    #region 서보 동작 로그
                    string ServoLogPath = logPath + "\\" + GlobalVar.ServoLogPath;
                    if (!Directory.Exists(ServoLogPath))
                    {
                        Directory.CreateDirectory(ServoLogPath);
                    }


                    ServoLogPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    //if (!File.Exists(SystemLogPath))
                    //{
                    //    FileStream fs = File.Create(SystemLogPath);
                    //    fs.Close();
                    //}

                    string ServoLogText = "";
                    if (GlobalVar.ServoLogQueue.TryDequeue(out ServoLogText))
                    {
                        FuncFile.WriteFile(ServoLogPath, ServoLogText);
                    }
                    #endregion

                    #region 카운트 로그 저장
                    //string countLogPath = Path.Combine(logPath, "CntLog", DateTime.Now.ToString("yyMMdd"));
                    //if (!Directory.Exists(countLogPath))
                    //{
                    //    Directory.CreateDirectory(countLogPath);
                    //}

                    //string countLogText = "";

                    //GlobalVar.LogData logData;

                    //// 큐에서 LogData 구조체 객체 꺼내기
                    //if (GlobalVar.CntQueue.TryDequeue(out logData))
                    //{
                    //    // 꺼낸 데이터 사용
                    //    string logText = logData.LogText;
                    //    string model = logData.Model;
                    //    string countLogFilePath = Path.Combine(countLogPath, model + ".log");
                    //    FuncFile.WriteNewFile(countLogFilePath, logText);
                    //}
                    //else
                    //{
                    //    Console.WriteLine("큐가 비어 있습니다.");
                    //}

                    //string countLogFilePath = Path.Combine(countLogPath, FuncAmplePacking.countLogModel + ".log");
                    //if (GlobalVar.CntQueue.TryDequeue(out countLogText))
                    //{

                    //    FuncFile.WriteNewFile(countLogFilePath, countLogText);
                    //}
                    #endregion

                    #region Scan 로그
                    string ScanLogPath = logPath + "\\" + GlobalVar.ScanLogPath;
                    if (!Directory.Exists(ScanLogPath))
                    {
                        Directory.CreateDirectory(ScanLogPath);
                    }


                    ScanLogPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    //if (!File.Exists(ScanLogPath))
                    //{
                    //    FileStream fs = File.Create(ScanLogPath);
                    //    fs.Close();
                    //}

                    string ScanLogText = "";
                    if (GlobalVar.ScanLogQueue.TryDequeue(out ScanLogText))
                    {
                        FuncFile.WriteFile(ScanLogPath, ScanLogText);
                    }
                    #endregion

                    #region Tester 로그
                    string TesterLogPath = logPath + "\\" + GlobalVar.TesterLogPath;
                    if (!Directory.Exists(TesterLogPath))
                    {
                        Directory.CreateDirectory(TesterLogPath);
                    }


                    TesterLogPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    //if (!File.Exists(TesterLogPath))
                    //{
                    //    FileStream fs = File.Create(TesterLogPath);
                    //    fs.Close();
                    //}

                    string TesterLogText = "";
                    if (GlobalVar.TesterLogQueue.TryDequeue(out TesterLogText))
                    {
                        FuncFile.WriteFile(TesterLogPath, TesterLogText);
                    }
                    #endregion

                    #region Debug 로그
                    string DebugLogPath = logPath + "\\" + GlobalVar.DebugLogPath;
                    if (!Directory.Exists(DebugLogPath))
                    {
                        Directory.CreateDirectory(DebugLogPath);
                    }


                    DebugLogPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    //if (!File.Exists(TesterLogPath))
                    //{
                    //    FileStream fs = File.Create(TesterLogPath);
                    //    fs.Close();
                    //}

                    string DebugLogText = "";
                    if (GlobalVar.DebugLogQueue.TryDequeue(out DebugLogText))
                    {
                        FuncFile.WriteFile(DebugLogPath, DebugLogText);
                    }
                    #endregion

                 

                    #region SQL execute - ADO가 동시 처리 못해서 
                    string sql = "";
                    if (GlobalVar.SQLExecuteQueue.TryDequeue(out sql))
                    {
                        GlobalVar.Sql.ExecuteQuery(sql);
                    }
                    #endregion

                    #region 날짜 변경시 지난 로그 삭제
                    if (date != DateTime.Now.ToString("yyyyMMdd"))
                    {
                        FuncLog.DeleteLogs();
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                    debug(ex.StackTrace);
                }

                date = DateTime.Now.ToString("yyyyMMdd");

                Thread.Sleep(GlobalVar.ThreadSleep);
            }
        }
    }
}