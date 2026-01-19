using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Radix
{
    /**
     * @brief 로그 처리 함수 선언
     */
    class FuncLog
    {
        /*
         * FuncLog.cs : 로그 처리 함수 선언
         */

        /**
         * @brief 생산량 누적값 저장
         *      샌딩 투배출기 프로그램 종료시에도 일일 누적 생산량 카운트하기 위해 저장
         * @param i 카운트 증가시킬 투입수
         * @param o 카운트 증가시킬 배출수
         */
        public static void WriteCount(int i, int o) // WriteCount(입력,출력) 생산량 누적값 저장
        {
            // 로그는 날짜별로 텍스트 파일로 저장함
            string countPath = GlobalVar.FaPath;
            if (!Directory.Exists(countPath))
            {
                Directory.CreateDirectory(countPath);
            }
            countPath += "\\" + GlobalVar.SWName;
            if (!Directory.Exists(countPath))
            {
                Directory.CreateDirectory(countPath);
            }
            countPath += "\\Count";
            if (!Directory.Exists(countPath))
            {
                Directory.CreateDirectory(countPath);
            }
            countPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".cnt";
            if (!File.Exists(countPath))
            {
                FileStream fs = File.Create(countPath);
                fs.Close();
            }
            if (i > 0)
            {
                int inCount = int.Parse(FuncIni.ReadIniFile("count", "SandingInput", countPath, "0"));
                FuncIni.WriteIniFile("count", "SandingInput", (inCount + i).ToString(), countPath);
            }
            if (o > 0)
            {
                int outCount = int.Parse(FuncIni.ReadIniFile("count", "SandingOutput", countPath, "0"));
                FuncIni.WriteIniFile("count", "SandingOutput", (outCount + o).ToString(), countPath);
            }
        }

        /**
         * @brief 로그 저장 큐에 로그 추가
         *      실제 로그 저장은 LogThread에서 실행함
         * @param text 로그 문자열
         */
        public static void WriteLog(string text) // WriteLog(문자열) 로그 저장
        {
            try
            {
                // 로그는 날짜별로 텍스트 파일로 저장함
                //string logPath = GlobalVar.FaPath;
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + GlobalVar.SWName;
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + GlobalVar.LogPath;
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                //if (!File.Exists(logPath))
                //{
                //    FileStream fs = File.Create(logPath);
                //    fs.Close();
                //}

                //메뉴얼 창에서는 인터락 관련 로그를 여기에 추가 by DG
                if(FuncInline.TabMain == FuncInline.enumTabMain.Manual)
                {
                    FuncInline.Interlock_View = text;
                }
                if(FuncInline.TabMain == FuncInline.enumTabMain.Auto)
                {
                    FuncInline.LogView(text);
                }
            

                string logText = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text;

                //FuncFile.WriteFile(logPath, logText);
                GlobalVar.LogQueue.Enqueue(logText);
            }
            catch
            { }
        }
        //검사 값 정보를 저장하기 위해
        public static void WriteValueLog(string text) // WriteLog(문자열) 로그 저장
        {
            try
            {

                string logText = "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + text;

                //FuncFile.WriteFile(logPath, logText);
                GlobalVar.ValueLogQueue.Enqueue(logText);
            }
            catch
            { }
        }
        //서보 이동 값 정보를 저장하기 위해
        public static void WriteServoLog(string text) // WriteLog(문자열) 로그 저장
        {
            try
            {

                string logText = "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + text;

                //FuncFile.WriteFile(logPath, logText);
                GlobalVar.ServoLogQueue.Enqueue(logText);
            }
            catch
            { }
        }

      

        /**
         * @brief 테스트 로그 저장 큐에 로그 추가
         *      오토인라인 SMD 테스트 관련
         * @param text 로그 문자열
         */
        public static void WriteLog_Tester(string text) // WriteLog(문자열) 로그 저장
        {
            try
            {
                // 로그는 날짜별로 텍스트 파일로 저장함
                //string logPath = GlobalVar.FaPath;
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + GlobalVar.SWName;
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + GlobalVar.TesterLogPath;
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                //if (!File.Exists(logPath))
                //{
                //    FileStream fs = File.Create(logPath);
                //    fs.Close();
                //}
                string logText = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text;

                //FuncFile.WriteFile(logPath, logText);
                GlobalVar.TesterLogQueue.Enqueue(logText);

            }
            catch
            { }
        }

        /**
         * @brief 디버그 로그 저장 큐에 로그 추가
         *      디버그용 데이터를 로그 파일로 남김
         * @param text 로그 문자열
         */
        public static void WriteLog_Debug(string text) // WriteLog(문자열) 로그 저장
        {
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
                logPath += "\\" + GlobalVar.DebugLogPath;
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
                string logText = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text;

                FuncFile.WriteFile(logPath, logText);

            }
            catch
            { }
        }

        /**
         * @brief 스캐너 로그 저장 큐에 로그 추가
         *      실제 로그 저장은 LogThread에서 실행함
         * @param text 로그 문자열
         */
        public static void WriteLog_Scan(string text) // WriteLog(문자열) 로그 저장
        {
            try
            {
                // 로그는 날짜별로 텍스트 파일로 저장함
                //string logPath = GlobalVar.FaPath;
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + GlobalVar.SWName;
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + GlobalVar.ScanLogPath;
                //if (!Directory.Exists(logPath))
                //{
                //    Directory.CreateDirectory(logPath);
                //}
                //logPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                //if (!File.Exists(logPath))
                //{
                //    FileStream fs = File.Create(logPath);
                //    fs.Close();
                //}
                string logText = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text;
                GlobalVar.ScanLogQueue.Enqueue(logText);
                //FuncFile.WriteFile(logPath, logText);

            }
            catch
            { }
        }

        /**
         * @brief 데이터베이스 로그 저장 큐에 로그 추가
         *      실제 로그 저장은 LogThread에서 실행함
         * @param text 로그 문자열
         */
        public static void WriteLog_SQL(string str) // MsSQL 로그 저장. 데이터 베이스 처리 에러 등 로깅
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


        


        /**
         * @brief 폴더 내 지정 기간 지난 파일 삭제. 
         * @param path 검색 경로
         * @param period 삭제 기준 일수
         * @param ext 확장자 구분 안 할 때는 ext = "*"
         * JHRYU BUGFIX 2023/11/09
         */
        public static void DeleteLog(string path, int period, string ext) // 폴더 내 지정 기간 지난 파일 삭제. 확장자 구분 안 할 때는 ext = "*"
        {
            //삭제할 경로에 파일들이 존재한다면 
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
            {
                FileInfo[] files = info.GetFiles();

                //생성된지 1주일 된 파일 지우기 위한 날짜 지정
                string date = DateTime.Today.AddDays(-period).ToString("yyyy-MM-dd");

                foreach (FileInfo file in files)
                {
                    try
                    {
                        //파일의 마지막 쓰여진 시간과 date 날짜와 비교                        
                        if (date.CompareTo(file.LastWriteTime.ToString("yyyy-MM-dd")) > 0)
                        {
                            //만약 마지막으로 쓰여진 시간이 1주일 지난 파일이라면
                            //확장자가 .cnt인 파일들 지워라
                            if (Path.GetExtension(file.Name.ToUpper()) == ("." + ext.ToUpper()) || ext == "*")
                            {
                                File.Delete(info + "\\" + file.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.ToString();
                    }
                }
            }
        }


        //HJ 수정 200318 로그파일 기간 지정 삭제(Util.cs)
        /**
         * @brief 내정된 경로 지정 기간 지난 파일 삭제. 
         *      프로젝트에 따라서 종류 변경할 것
         */
        public static void DeleteLogs()
        {
            try
            {
                //삭제할 파일들이 있는 경로 설정
                string countPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.CountPath + "\\";
                string logPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.LogPath + "\\";
                string secsPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.SECSPath + "\\";

                DeleteLog(countPath, GlobalVar.LogFileDeleteDay, "cnt");
                DeleteLog(logPath, GlobalVar.LogFileDeleteDay, "log");
                DeleteLog(secsPath, GlobalVar.LogFileDeleteDay, "log");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
