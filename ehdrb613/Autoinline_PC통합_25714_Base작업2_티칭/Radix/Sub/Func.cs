using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

//HJ 추가 내용
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections.Concurrent; // ConcurrentQueue

using System.Data.SqlClient;

namespace Radix
{
    public enum enumVisionCmd
    {
        Viewer = 1,
        Align = 2,
        Code2D = 3
    }

    public static class Func
    {
        /* Func.cs
           프로그램 내 주요 로직과 관련된 부분들에 대한 기본 함수들을 선언
       //*/

        private static void debug(string str) // //debug(문자열) Func.cs 내부에서만 콜하는 로컬 디버그
        {
            Util.Debug("Func : " + str);
        }

        public static void DebugTick(string str)
        {
            //Util.Debug(str + "tick : " + (Environment.TickCount - GlobalVar.tick));
            GlobalVar.tick = GlobalVar.TickCount64;
        }

        public static void StatusPrint(string str)
        {
            FuncInline.LogView(str);
            //debug(str);
        }
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
                int inCount = int.Parse(FuncFile.ReadIniFile("count", "SandingInput", countPath, "0"));
                FuncFile.WriteIniFile("count", "SandingInput", (inCount + i).ToString(), countPath);
            }
            if (o > 0)
            {
                int outCount = int.Parse(FuncFile.ReadIniFile("count", "SandingOutput", countPath, "0"));
                FuncFile.WriteIniFile("count", "SandingOutput", (outCount + o).ToString(), countPath);
            }
        }

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
                string logText = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + text;

                //FuncFile.WriteFile(logPath, logText);
                GlobalVar.LogQueue.Enqueue(logText);
            }
            catch
            { }
        }

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
                string logText = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + text;

                //FuncFile.WriteFile(logPath, logText);
                GlobalVar.TesterLogQueue.Enqueue(logText);

                //debug(logText);
            }
            catch
            { }
        }

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
                string logText = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + text;

                FuncFile.WriteFile(logPath, logText);

            }
            catch
            { }
        }

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
                string logText = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + text;
                GlobalVar.ScanLogQueue.Enqueue(logText);
                //FuncFile.WriteFile(logPath, logText);

            }
            catch
            { }
        }

        public static void WriteLog_SECS(string text) // WriteLog(문자열) 로그 저장
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
                logPath += "\\" + GlobalVar.SECSPath;
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
                string logText = "[" + DateTime.Now.ToString("HH:mm:ss.fff") + "] " + text;

                FuncFile.WriteFile(logPath, logText);
            }
            catch
            { }
        }

        public static double CalcWidthPos(int axis, double pos) // 일반 폭조절 모터 자체의 거리 계산값을 기구상의 실제 거리로 환산 - 위치값 확인할 때
        {
            switch (axis)
            {
                case 0: // In Shuttle
                    return FuncInline.DefaultPCBWidth + pos;
                case 1: // Out Shuttle
                    return FuncInline.DefaultPCBWidth + pos;
                case 2: // NG 컨베어
                    return FuncInline.DefaultPCBWidth + pos;
                case 3: // Rack1
                    return FuncInline.DefaultPCBWidth + pos;
                case 4: // Rack2
                    return FuncInline.DefaultPCBWidth + pos;
                default:
                    return pos;
            }
        }

        public static double ReCalcWidthPos(int axis, double pos) // 일반 폭조절 실제 거리를 기구상의 모터 자체의 거리 계산값으로 환산 - 지령 날릴 때
        {
            switch (axis)
            {
                case 0:
                    return pos - FuncInline.DefaultPCBWidth;
                case 1:
                    return pos - FuncInline.DefaultPCBWidth;
                case 2:
                    return pos - FuncInline.DefaultPCBWidth;
                case 3:
                    return pos - FuncInline.DefaultPCBWidth;
                case 4:
                    return pos - FuncInline.DefaultPCBWidth;
                default:
                    return pos;
            }
        }

        public static enumAxis GetPMCAxis(FuncInline.enumPMCAxis axis) // PMC Motion 컨트롤러에 연결된 축 이름으로 x/y 확인
        {
            int mod = (int)axis % 2;
            if (mod == 0)
            {
                return enumAxis.X;
            }
            return enumAxis.Y;
        }

     

        #region 파나소닉 레이저 각인기 관련
        public static string MakeMarkCode()
        {
            // 현재 시/분 값이 이전 각인된 것과 다르면 시리얼 초기화
            int curYear = int.Parse(DateTime.Now.ToString("yyyy"));
            int curMonth = int.Parse(DateTime.Now.ToString("MM"));
            int curDate = int.Parse(DateTime.Now.ToString("dd"));
            int curMin = int.Parse(DateTime.Now.ToString("mm"));
            int curHour = int.Parse(DateTime.Now.ToString("HH"));
            //int serial = 1;

            string str = "";
            if (CodeSearch(str))
            {
                return MakeMarkCode();
            }

            return str;
        }

        public static bool LaserMark(structPosition pos, bool turn, bool is2D, string text)
        {
            /*
            GlobalVar.LaserStatus = FuncInline.enumLaserStatus.Marking;

            int startTime = Environment.TickCount;
            if (pos.x != 0 ||
                pos.y != 0 ||
                pos.z != 0 ||
                pos.a != 0)
            {
                //debug("move");
                if (!Func.MoveStage(GlobalVar.MoveSpeed, pos.x, pos.y, turn, GlobalVar.SlowAcc))
                {
                    //debug("move fail");
                    if (is2D)
                    {
                        GlobalVar.MarkingText = text + " fail";
                    }
                    GlobalVar.LaserStatus = FuncInline.enumLaserStatus.Errored;
                    return false;
                }
            }
            else
            {
                //debug("no move");
            }

            if (!GlobalVar.Laser.Init())
            {
                GlobalVar.LaserStatus = FuncInline.enumLaserStatus.Errored;
                if (is2D)
                {
                    GlobalVar.MarkingText = text + " fail";
                }
                //debug("Laser marker init fail");
                return false;
            }

            GlobalVar.Laser.SetShutter(false);
            GlobalVar.Laser.SetFile(1);
            GlobalVar.Laser.GetStatus();
            //debug("2D " + is2D.ToString());
            //debug("text : " + text);
            GlobalVar.Laser.SetGlobalText(1, text);
            GlobalVar.Laser.GetStatus();
            GlobalVar.Laser.SetGlobalText(2, text);
            GlobalVar.Laser.GetStatus();
            GlobalVar.Laser.SetMarkObject(false, 1, is2D); // 2D 각인
            GlobalVar.Laser.GetStatus();
            GlobalVar.Laser.SetMarkObject(false, 2, !is2D); // 텍스트 미각인
            GlobalVar.Laser.GetStatus();
            startTime = Environment.TickCount;
            while (Environment.TickCount - startTime < 3000 &&
                    !GlobalVar.Laser.GetStatus().ShutterOpen)
            {
                GlobalVar.Laser.SetShutter(true);
                Thread.Sleep(100);
            }
            GlobalVar.Laser.AlarmReset();
            GlobalVar.Laser.GetStatus();
            GlobalVar.Laser.SetTrigger();
            startTime = Environment.TickCount;
            structMarkerStatus stat = GlobalVar.Laser.GetStatus();
            while (stat.Marking)
            {
                if (Environment.TickCount - startTime > 3000 ||
                    stat.Errored)
                {
                    //debug("marking wait fail");
                    GlobalVar.LaserStatus = FuncInline.enumLaserStatus.Errored;
                    if (is2D)
                    {
                        GlobalVar.MarkingText = text + " fail";
                    }
                    return false;
                }
                Thread.Sleep(100);
                stat = GlobalVar.Laser.GetStatus();
            }
            if (stat.Errored)
            {
                //debug("stat.Errored");
                GlobalVar.LaserStatus = FuncInline.enumLaserStatus.Errored;
                if (is2D)
                {
                    GlobalVar.MarkingText = text + " fail";
                }
                return false;
            }
            if (is2D)
            {
                GlobalVar.MarkingText = text + " OK";
            }
            //*/
            return true;
        }
        public static bool LaserMarkCont(structPosition pos, bool turn, bool is2D, string text)
        {
            //debugTick("LaserMarkCont " + pos.x + "," + pos.y + "," + pos.z + " " + is2D + " " + text);
            Func.WriteLog("LaserMarkCont " + pos.x + "," + pos.y + "," + pos.z + " " + is2D + " " + text);
            int startTime = Environment.TickCount;
            if (pos.x != 0 ||
                pos.y != 0 ||
                pos.z != 0 ||
                pos.a != 0)
            {
                //debugTick("move");

            }
            else
            {
                //debugTick("no move");
            }



            return true;
        }
        #endregion

        #region 로이체 비전 관련
        //private static bool connectLeuzeTrigger = false;
        private static bool connectLeuzeData = false;

        public static bool VisitionConnectLeuze() // VisionConnect() 비전체크
        {
            /*
            socketLeuzeTrigger.ReceiveTimeout = 500;
            socketLeuzeData.ReceiveTimeout = 500;
            if (connectLeuzeTrigger == false)
            {
                try
                {
                    socketLeuzeTrigger.Connect(GlobalVar.Vision_IP, GlobalVar.Vision_Trigger_Port);
                    if (!socketLeuzeTrigger.Connected)
                    {
                        //MessageBox.Show("접속 실 패");
                        return false;
                    }
                    else
                    {
                        connectLeuzeTrigger = true;
                    }
                    streamLeuzeTrigger = socketLeuzeTrigger.GetStream();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    //MessageBox.Show("1서버가 실행중이 아닙니다.", "연결실패!");
                    return false;
                }

            }

            if (connectLeuzeData == false)
            {
                try
                {
                    socketLeuzeData.Connect(GlobalVar.Vision_IP, GlobalVar.Vision_Receive_Port);

                    if (!socketLeuzeData.Connected)
                    {
                        //MessageBox.Show("접속 실 패");
                        return false;
                    }
                    else
                    {
                        connectLeuzeData = true;
                    }
                    streamLeuzeData = socketLeuzeData.GetStream();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    //MessageBox.Show("2서버가 실행중이 아닙니다.", "연결실패!");
                    return false;
                }
            }
            */
            return true;

        }

        public static void SendVisionTrigger()
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes("TRG\r\n");
                //GlobalVar.streamLeuzeTrigger.Write(buffer, 0, buffer.Length);
                //GlobalVar.streamLeuzeTrigger.Flush();
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SendVisionJob(enumVisionCmd cmd)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes("CJB00" + ((int)cmd).ToString() + "\r\n");
                //GlobalVar.streamLeuzeTrigger.Write(buffer, 0, buffer.Length);
                //GlobalVar.streamLeuzeTrigger.Flush();
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static string GetVisionMoveResult(double speed, structPosition pos, enumVisionCmd cmd, string text)
        {
            SendVisionJob(cmd);

            //bool turn = false;

            /*
            if (pos.a > 90)
            {
                turn = true;
                GlobalVar.WorkOutputPos.a = pos.a;
            }
            else
            {
                GlobalVar.WorkInputPos.a = pos.a;
            }


            if (cmd == enumVisionCmd.Align) // 얼라인 체크일 경우에는 이동 완료 후 검사
            {
                if (!Func.MoveStage(speed, (double)pos.x, (double)pos.y, turn, GlobalVar.MidAcc))
                {
                    return "";
                }
            }
            else if (cmd == enumVisionCmd.Code2D) // 2D 검사는 이동하면서 체크(코드 일치), 정지후 검사(코드 있으면)
            {
                if (!Func.MoveStageVision(speed, (double)pos.x, (double)pos.y, turn, GlobalVar.MidAcc, text))
                {
                    return "";
                }
            }
            //*/
            return GetVisionResult(cmd);
        }

        public static string GetVisionResult(enumVisionCmd cmd)
        {
            SendVisionJob(cmd);
            string message = "";
            //bool visionChecked = false;

            /*
            if (cmd != enumVisionCmd.Viewer)
            {
                for (int i = 0; i < 10; i++) // 10번 재시도
                {
                    SendVisionTrigger();
                    try
                    {
                        int BUFFERSIZE = GlobalVar.socketLeuzeData.ReceiveBufferSize;
                        byte[] buffer = new byte[BUFFERSIZE];
                        int bytes = GlobalVar.streamLeuzeData.Read(buffer, 0, buffer.Length);

                        message = Encoding.ASCII.GetString(buffer, 0, bytes);
                    }
                    catch
                    {
                        return "";
                    }
                    if (message.Length == 0)
                    {
                        SendVisionJob(cmd);
                        Thread.Sleep(100);
                        continue;
                    }
                    //debug("vision msg : " + message);
                    string[] cols = message.Split(',');
                    if (cmd == enumVisionCmd.Code2D &&
                        cols.Length > 0 &&
                        cols.Length < 5)
                    {
                        // 2D
                        GlobalVar.VisionResultAlign = new structPosition();
                        if (cols[0] == "P")
                        {
                            GlobalVar.VisionResult2D = cols[1];
                            visionChecked = true;
                        }
                        else
                        {
                            GlobalVar.VisionResult2D = "";
                        }
                    }
                    if (cmd == enumVisionCmd.Align &&
                            cols.Length > 5 &&
                            cols.Length < 9)
                    {
                        // Align P,314478,381784,F,313887,382000,
                        GlobalVar.VisionResult2D = "";
                        structPosition pos = new structPosition(999, 999, 999, 999);
                        try
                        {
                            // 카메라 장착 방향 때문에 X/Y가 서로 바뀜
                            if (cols[0] == "P")
                            {
                                pos.y = (double.Parse(cols[1]) / 1000 - GlobalVar.VisionResolution.x / 2) * GlobalVar.VisionArea.x / GlobalVar.VisionResolution.x / 2 * 1000 / 1046;
                                pos.x = (double.Parse(cols[2]) / 1000 - GlobalVar.VisionResolution.y / 2) * GlobalVar.VisionArea.y / GlobalVar.VisionResolution.y / 2;
                                visionChecked = true;
                            }
                            else if (cols[3] == "P")
                            {
                                pos.y = (double.Parse(cols[4]) / 1000 - GlobalVar.VisionResolution.x / 2) * GlobalVar.VisionArea.x / GlobalVar.VisionResolution.x / 2 * 1000 / 1046; // 왜 2배 가까이 나오는지는 모르겠음
                                pos.x = (double.Parse(cols[5]) / 1000 - GlobalVar.VisionResolution.y / 2) * GlobalVar.VisionArea.y / GlobalVar.VisionResolution.y / 2; // 왜 2배 가까이 나오는지는 모르겠음
                                visionChecked = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            //debug(ex.ToString());
                            //debug(ex.StackTrace);
                        }
                        GlobalVar.VisionResultAlign = pos;
                        if (visionChecked)
                        {
                            break;
                        }
                    }
                    //SendVisionJob(cmd);
                    //Thread.Sleep(200);
                    if (visionChecked)
                    {
                        break;
                    }
                }
            }
    //*/
            /*
            if (!visionChecked)
            {
                GlobalVar.VisionResultAlign = new structPosition();
                GlobalVar.VisionResult2D = "";
                return "";
            }
            //*/
            if (message.Length == 0)
            {
                //GlobalVar.VisionResultAlign = new structPosition();
                //GlobalVar.VisionResult2D = "";
            }
            return message;
        }

        private static bool VisionRecieveLeuze() // Recieve() 비전 체크값 수신
        {
            byte[] buffer = Encoding.ASCII.GetBytes("TRG\r\n");
            //GlobalVar.streamLeuzeTrigger.Write(buffer, 0, buffer.Length);
            //GlobalVar.streamLeuzeTrigger.Flush();

            for (int i = 0; i < 3; i++)
            {
                if (VisionRecieveLeuze())
                {
                    return true;
                }
                Thread.Sleep(100);
            }

            string message = string.Empty;

            if (connectLeuzeData == false)
            {
                try
                {
                    /*
                    GlobalVar.socketLeuzeData.Connect(GlobalVar.Vision_IP, GlobalVar.Vision_Receive_Port);

                    if (!GlobalVar.socketLeuzeData.Connected)
                    {
                        //MessageBox.Show("접속 실 패");
                        return false;
                    }
                    else
                    {
                        connectLeuzeData = true;
                    }
                    GlobalVar.streamLeuzeData = GlobalVar.socketLeuzeData.GetStream();
                    //*/
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    //MessageBox.Show("2서버가 실행중이 아닙니다.", "연결실패!");
                    return false;
                }
            }

            Thread.Sleep(100);
            //int timecheck = 0;
            //while (true)
            //{
            try
            {
                /*
                int BUFFERSIZE = GlobalVar.socketLeuzeData.ReceiveBufferSize;
                buffer = new byte[BUFFERSIZE];
                int bytes = GlobalVar.streamLeuzeData.Read(buffer, 0, buffer.Length);

                message = Encoding.ASCII.GetString(buffer, 0, bytes);
                //debug(message); //F,548248,298357,
                if (message != null &&
                    message.Length >= 15)
                {
                    try
                    {
                        GlobalVar.VisionResult = message;
                        GlobalVar.VisionPosition.x = (double.Parse(message.Substring(2, 6)) / 1000 - GlobalVar.VisionCenter.x) / 5.5;
                        GlobalVar.VisionPosition.y = (GlobalVar.VisionCenter.y - double.Parse(message.Substring(9, 6)) / 1000) / 5.5;
                        return true;
                    }
                    catch
                    { }
                }
        //*/
                /*
                Thread.Sleep(1000);
                timecheck = timecheck + 1;
                if (timecheck >= 5)
                {
                    break;
                }
                //*/
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            //}
            return false;
        }
        #endregion


        #region 이외 동작 관련
        public static bool CheckPCBWidth(double speed, double width) // 컨베어 폭조절
        {


            bool inputWidth = false;
            bool workWidth = false;

            /*
            if (!GlobalVar.WorkWidth.IsCon())
            {
                //debug("PMC Open Fail");
                return false;
            }
            //*/

            //bool mDir = true;
            /*
            if (Func.CalcAxisPos(4, FuncMotion.PulseToMM((long)GlobalVar.AxisStatus[4].Position, GlobalVar.GearRatio[4], GlobalVar.RevMM[4], GlobalVar.RevPulse[4])) - width > 0)
            {
                // 벌려야 하는 상황
                mDir = false;
            }
            //*/

            /*
            GlobalVar.WorkWidth.SetDrvSpd(enumAxis.X, 1);
            GlobalVar.WorkWidth.SetStrSpd(enumAxis.X, 30);
            GlobalVar.WorkWidth.SetCurDrvSpd(enumAxis.X, 300, 300);
            GlobalVar.WorkWidth.SetAccSpdRate(enumAxis.X, 3000);
            GlobalVar.WorkWidth.SetDecSpdRate(enumAxis.X, 3000);
            GlobalVar.WorkWidth.SetJrkSpd(enumAxis.X, 30000);
            //*/

            ulong startTime = GlobalVar.TickCount64;
            while (!GlobalVar.GlobalStop)
            {
                if (GlobalVar.TickCount64 - startTime > (ulong)GlobalVar.NormalTimeout * 1000)
                {
                    //GlobalVar.WorkWidth.SlowStop(enumAxis.X);
                    //FuncMotion.MoveStop(4);
                    return false;
                }

                #region 진입 컨베어
                /*
                if (Math.Abs(Func.CalcAxisPos(4, FuncMotion.PulseToMM((long)GlobalVar.AxisStatus[4].Position, GlobalVar.GearRatio[4], GlobalVar.RevMM[4], GlobalVar.RevPulse[4])) - width) < 1)
                {
                    inputWidth = true;
                }
                else if (GlobalVar.AxisStatus[4].Errored ||
                            GlobalVar.AxisStatus[4].ErrorStop)
                {
                    FuncMotion.ServoReset(4);
                }
                else if (!GlobalVar.AxisStatus[4].PowerOn)
                {
                    FuncMotion.ServoOn(4, true);
                }
                else if (GlobalVar.AxisStatus[4].StandStill)
                {
                    if (!mDir ||
                        workWidth)
                    {
                        FuncMotion.MoveAbsolute(4,
                                            FuncMotion.MMToPulse(Func.CalcAxisPos(4, width), GlobalVar.GearRatio[4], GlobalVar.RevMM[4], GlobalVar.RevPulse[4]),
                                            FuncMotion.MMToPulse(speed, GlobalVar.GearRatio[4], GlobalVar.RevMM[4], GlobalVar.RevPulse[4]));
                    }
                }
                //*/
                #endregion
                if (inputWidth && workWidth)
                {
                    return true;
                }

                Thread.Sleep(GlobalVar.ThreadSleep);
            }
            return false;
        }

        public static bool InputPCB() // 공급 컨베어에 PCB 로드
        {
            int startTime = Environment.TickCount;

            return true;
        }

        public static bool InputWork() // 작업 컨베어에 PCB 로드
        {
            int startTime = Environment.TickCount;

            #region 안전위치 이동
            /*
            if (Func.CalcAxisPos(0, FuncMotion.PulseToMM(GlobalVar.AxisStatus[0].Position, GlobalVar.GearRatio[0], GlobalVar.RevMM[0], GlobalVar.RevPulse[0])) > 200)
            {
                if (!Func.MoveStage(500, 100, GlobalVar.WorkInputPos.y, false)) // 워크지그 정위치
                {
                    return false;
                    // PCB 공급 에러처리
                }
            }
            */
            #endregion

            return true;
        }

        public static bool OutputWork() // 작업 컨베어의 PCB 배출
        {
            int startTime = Environment.TickCount;

            #region 안전위치로 이동
            /*
            if (Func.CalcAxisPos(0, FuncMotion.PulseToMM(GlobalVar.AxisStatus[0].Position, GlobalVar.GearRatio[0], GlobalVar.RevMM[0], GlobalVar.RevPulse[0])) > 200)
            {
                if (!Func.MoveStage(500, 200, GlobalVar.WorkOutputPos.y, true))
                {
                    //debug("Safe Point Fail");
                    return false;
                }
            }
            //*/

            //HJ 수정 200630 충돌 때문에 배출 하는 부분 Y축 먼저 움직임
            /*
            int chTime = Environment.TickCount;
            while (Func.CalcAxisPos(1, FuncMotion.PulseToMM((long)GlobalVar.AxisStatus[1].Position, GlobalVar.GearRatio[1], GlobalVar.RevMM[1], GlobalVar.RevPulse[1])) > 100)
            {
                if(Environment.TickCount - chTime > 5000)
                {
                    break;
                }

                if(GlobalVar.AxisStatus[1].Velocity ==0)
                {
                    FuncMotion.MoveAbsolute(1,
                    FuncMotion.MMToPulse(Func.CalcAxisPos(1, 100), GlobalVar.GearRatio[1], GlobalVar.RevMM[1], GlobalVar.RevPulse[1]),
                    FuncMotion.MMToPulse(GlobalVar.MoveSpeed, GlobalVar.GearRatio[1], GlobalVar.RevMM[1], GlobalVar.RevPulse[1]),
                    FuncMotion.MMToPulse(GlobalVar.MoveSpeed, GlobalVar.GearRatio[1], GlobalVar.RevMM[1], GlobalVar.RevPulse[1]) * GlobalVar.MidAcc,
                    FuncMotion.MMToPulse(GlobalVar.MoveSpeed, GlobalVar.GearRatio[1], GlobalVar.RevMM[1], GlobalVar.RevPulse[1]) * GlobalVar.MidAcc,
                    FuncMotion.MMToPulse(GlobalVar.MoveSpeed, GlobalVar.GearRatio[1], GlobalVar.RevMM[1], GlobalVar.RevPulse[1]) * GlobalVar.MidAcc * GlobalVar.MidAcc,
                    false,
                    0);
                }
                if (Func.CalcAxisPos(1, FuncMotion.PulseToMM((long)GlobalVar.AxisStatus[1].Position, GlobalVar.GearRatio[1], GlobalVar.RevMM[1], GlobalVar.RevPulse[1])) <= 100)
                {
                    break;
                }
            }
    //*/
            ////////////////////////////////////////////////////////////////////////////////////
            #endregion

            return true;
        }



        #endregion

        public static void GetModelList()
        {
            String modelPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ModelPath;
            int count = 0;
            if (Directory.Exists(modelPath))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(modelPath);
                foreach (System.IO.FileInfo File in di.GetFiles())
                {
                    if (File.Extension.ToLower().CompareTo(".ini") == 0 &&
                        !File.FullName.ToLower().Contains("_customer"))
                    {
                        try
                        {
                            String FileNameOnly = File.Name.Substring(0, File.Name.Length - 4);
                            GlobalVar.ModelNames[count] = FileNameOnly;
                            count++;
                        }
                        catch { }
                    }
                }
            }
            GlobalVar.ModelCount = count;
        }

        public static void ChangeLoaderUnloaderWidth(bool loader, double width)
        {
            int startTime = Environment.TickCount;
            if (loader)
            {
                //GlobalVar.LoaderUnloader.SendWidth(loader, width);
                /*
                while (!GlobalVar.LoaderUnloader.WidthStartLoader &&
                    !GlobalVar.LoaderUnloader.WidthStartedLoader &&
                    Environment.TickCount - startTime < 10000)
                {
                    GlobalVar.LoaderUnloader.SendWidth(loader, width);
                    Thread.Sleep(1000);
                }
                //*/
            }
            else
            {
                //GlobalVar.LoaderUnloader.SendWidth(loader, width);
                /*
                while (!GlobalVar.LoaderUnloader.WidthStartUnloader &&
                    !GlobalVar.LoaderUnloader.WidthStartedUnloader &&
                    Environment.TickCount - startTime < 10000)
                {
                    GlobalVar.LoaderUnloader.SendWidth(loader, width);
                    Thread.Sleep(1000);
                }
                //*/
            }
        }

        public static void UpProductCount(bool array, int index, string side, string code, string ok)
        {
            //debug("UpProductCount : " + array.ToString() + "," + index + "," + side + "," + code + "," + ok);
            try
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
                countPath += "\\" + GlobalVar.CountPath;
                if (!Directory.Exists(countPath))
                {
                    Directory.CreateDirectory(countPath);
                }
                countPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".cnt";
                bool created = false;
                if (!File.Exists(countPath))
                {
                    created = true;
                    FileStream fs = File.Create(countPath);
                    fs.Close();
                }
                int pcbCount = 0;
                if (!created) // 
                {
                    string strCount = "";
                    for (int i = 0; i < 100 && strCount == ""; i++) // 혹시 PCB 카운트 읽을 때 에러 있으면 로그가 완전 초기화되기 때문에 반복체크한다.
                    {
                        string trCount = FuncFile.ReadIniFile("count", "PCBCount", countPath, "");
                        //debug("strCount : " + strCount);
                        try
                        {
                            pcbCount = int.Parse(strCount);
                        }
                        catch { }
                        if (pcbCount > 0)
                        {
                            break;
                        }
                    }
                }
                if (!array) // PCB
                {
                    // 일달 PCB 갯수부터
                    FuncFile.WriteIniFile("count", "PCBCount", (pcbCount + 1).ToString(), countPath);
                }
                else // array
                {
                    int arrayCount = int.Parse(FuncFile.ReadIniFile(pcbCount.ToString(), "ArrayCount", countPath, "0"));
                    if (ok.Length == 0)
                    {
                        FuncFile.WriteIniFile(pcbCount.ToString(), "ArrayCount", Math.Max(arrayCount, index).ToString(), countPath);
                    }
                    FuncFile.WriteIniFile(pcbCount.ToString(), index.ToString() + "_" + side, code + "_" + ok, countPath);
                }
            }
            catch { }

            if (array &&
                side == "FRONT") // 20200320 빠른 검색을 위해 검색용 파일을 생성
            {
                WriteSearch(code);
            }
        }

        public static void WriteSearch(string code) // 코드 검색용 로그 저장. 자동시에는 전면 마킹시에만 콜...
        {
            if (code == null ||
                code.Length != 13)
            {
                return;
            }
            try
            {
                #region 검색용 파일 생성
                string searchPath = GlobalVar.FaPath;
                if (!Directory.Exists(searchPath))
                {
                    Directory.CreateDirectory(searchPath);
                }
                searchPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(searchPath))
                {
                    Directory.CreateDirectory(searchPath);
                }
                searchPath += "\\" + GlobalVar.SearchPath;
                if (!Directory.Exists(searchPath))
                {
                    Directory.CreateDirectory(searchPath);
                }
                string year = "";
                string month = "";
                string date = "";
                if (!GetCodeDate(code, ref year, ref month, ref date))
                {
                    return;
                }
                searchPath += "\\" + year;
                if (!Directory.Exists(searchPath))
                {
                    Directory.CreateDirectory(searchPath);
                }
                searchPath += "\\" + month;
                if (!Directory.Exists(searchPath))
                {
                    Directory.CreateDirectory(searchPath);
                }
                searchPath += "\\" + date;
                if (!Directory.Exists(searchPath))
                {
                    Directory.CreateDirectory(searchPath);
                }
                searchPath += "\\" + code;
                if (!File.Exists(searchPath))
                {
                    FileStream fs = File.Create(searchPath);
                    fs.Close();
                }
                #endregion
            }
            catch { }

        }

        public static bool CodeSearch(string code) // 코드 검색
        {
            if (code == null ||
                code.Length != 13)
            {
                return false;
            }
            try
            {

                string year = "";
                string month = "";
                string date = "";
                if (!GetCodeDate(code, ref year, ref month, ref date))
                {
                    return false;
                }
                string searchPath = GlobalVar.FaPath +
                                    "\\" + GlobalVar.SWName +
                                    "\\" + GlobalVar.SearchPath +
                                    "\\" + year +
                                    "\\" + month +
                                    "\\" + date +
                                    "\\" + code;
                return File.Exists(searchPath);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return false;
        }

        public static bool GetCodeDate(string code, ref string year, ref string month, ref string date) // 코드에서 연, 월, 일 값을 계산
        {
            if (code == null ||
                code.Length != 13)
            {
                return false;
            }

            try
            {
                /*
                string str = GlobalVar.CoCode.ToString(); // 법인코드 1자리
                str += curYear.ToString().Substring(3, 1); // 연도 1자리
                str += Util.IntToHexString(curMonth, 1); // 월 1자리
                str += curDate.ToString("D2"); // 일 2자리
                str += Util.IntToHexString(GlobalVar.MachineNumber, 1); // 호기 1자리
                str += curHour.ToString("D2"); // 시 2자리
                str += curMin.ToString("D2"); // 분 2자리
                str += GlobalVar.CustomerCode; // 거래선 1자리
                str += GlobalVar.MarkCodeSerial.ToString("D2"); // 시리얼 2자리
                //*/
                year = DateTime.Now.ToString("yyyy").Substring(0, 3) + code.Substring(1, 1);
                month = code.Substring(2, 1);
                date = code.Substring(3, 2);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
                return false;
            }

            return true;
        }

        public static void MoveOutPos()
        {
            //Func.MoveStage(GlobalVar.MoveSpeed, 100, GlobalVar.WorkOutputPos.y, true);
        }

        public static void Remark()
        {
            bool[] marked = new bool[100];

        }


        #region AGV 관련 PIO 통신 함수
        /*
        #region AGV 관련 PIO 통신 함수
        //PIO L_REQ Sequnce : AGV에서 설비로 물건 전달 (Load)
        //PIO R_REQ Sequnce : 설비에서 AGV로 물건 전달 (Unload)
        #region AGV Stage Check
        public static void AGVCheckStage(int AGVnumber)
        {
            #region 첫번째 AGV
            if (AGVnumber == 0)
            {
                DIO.WriteDOData(FuncInline.enumDONames.Y03_7_PIO_0_E_STOP + AGVnumber, true);

                #region AGV -> 설비 (Unload)
                if (DIO.GetDIData(FuncInline.enumDINames.X03_0_PIO_0_valid + AGVnumber) &&
                    !DIO.GetDIData(FuncInline.enumDINames.X03_1_PIO_0_CS_0 + AGVnumber) &&
                    DIO.GetDIData(FuncInline.enumDINames.X03_2_PIO_0_CS_1 + AGVnumber) &&
                    !DIO.GetDIData(FuncInline.enumDINames.X03_3_PIO_0_CS_2 + AGVnumber))
                {
                    GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Docking;
                    #region Job Code Receive
                    //AGV가 도킹 되고 Job 코드를 받았을 때 비트를 변수 변경이 필요하다.
                    if (GlobalVar.AGVJobCodeReceive_0 &&//메인(다른 쓰레드)에서 리시브 변수 변경
                        !GlobalVar.AGVReady_0)
                    {
                        GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Load;
                        AGVJobCodeReceive_U(AGVnumber);
                    }
                    else
                    {
                        AGVJobCodeReceive_S(AGVnumber);
                    }
                    #endregion

                    #region Ready Receive
                    //제품을 받을 준비가 되었을 때 레디 비트를 살려줘야 한다.
                    if (GlobalVar.AGVReady_0)//메인(다른 쓰레드)에서 레디 변수 변경
                    {
                        //TR 받았을 때
                        if (DIO.GetDIData(FuncInline.enumDINames.X03_4_PIO_0_tr_req + AGVnumber) &&
                            !DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkReady;
                            AGVChechWorkReady_On(AGVnumber);
                            if (DIO.GetDIData(FuncInline.enumDINames.X03_5_PIO_0_busy + AGVnumber))
                            {
                                GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Working;
                                GlobalVar.AGVJobCodeReceive_0 = false;
                            }
                        }
                        //컴플리트 받았을 때
                        else if (DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkFinish;
                            AGVChechWorkReady_Off(AGVnumber);
                            GlobalVar.AGVReady_0 = false;

                            GlobalVar.AGV_0_Working = false;
                        }
                    }
                    #endregion
                }
                #endregion
                #region 설비 -> AGV
                else if (DIO.GetDIData(FuncInline.enumDINames.X03_0_PIO_0_valid + AGVnumber) &&
                    DIO.GetDIData(FuncInline.enumDINames.X03_1_PIO_0_CS_0 + AGVnumber) &&
                    DIO.GetDIData(FuncInline.enumDINames.X03_2_PIO_0_CS_1 + AGVnumber) &&
                    !DIO.GetDIData(FuncInline.enumDINames.X03_3_PIO_0_CS_2 + AGVnumber))
                {
                    GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Docking;
                    //임시
                    GlobalVar.AGVJobCodeReceive_0 = true;
                    #region Job Code Receive
                    //AGV가 도킹 되고 Job 코드를 받았을 때 비트를 변수 변경이 필요하다.
                    if (GlobalVar.AGVJobCodeReceive_0 &&//메인(다른 쓰레드)에서 리시브 변수 변경
                        !GlobalVar.AGVReady_0)
                    {
                        GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.UnLoad;
                        AGVJobCodeReceive_L(AGVnumber);                        
                    }
                    else
                    {
                        AGVJobCodeReceive_S(AGVnumber);
                    }
                    #endregion

                    #region Ready Receive
                    //제품을 받을 준비가 되었을 때 레디 비트를 살려줘야 한다.
                    if (GlobalVar.AGVReady_0)//메인(다른 쓰레드)에서 레디 변수 변경
                    {
                        //TR 받았을 때
                        if (DIO.GetDIData(FuncInline.enumDINames.X03_4_PIO_0_tr_req + AGVnumber) &&
                            !DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkReady;
                            AGVChechWorkReady_On(AGVnumber);
                            if (DIO.GetDIData(FuncInline.enumDINames.X03_5_PIO_0_busy + AGVnumber))
                            {
                                GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Working;
                                GlobalVar.AGVJobCodeReceive_0 = false;
                            }
                        }
                        //컴플리트 받았을 때
                        else if (DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkFinish;
                            AGVChechWorkReady_Off(AGVnumber);
                            GlobalVar.AGVReady_0 = false;

                            GlobalVar.AGV_0_Working = false;
                        }
                    }
                    #endregion
                }
                #endregion
                #region  충전 시작
                #endregion
                #region  충전 종료
                #endregion
                #region 글로벌 변수 초기화
                else
                {
                    GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.None;
                    GlobalVar.AGVJobCodeReceive_0 = false;
                    GlobalVar.AGVReady_0 = false;
                }
                #endregion
            }
            #endregion

            #region 두번째 AGV              확인 해야 됨.
            if (AGVnumber == -8)
            {
                DIO.WriteDOData(FuncInline.enumDONames.Y03_7_PIO_0_E_STOP + AGVnumber, true);

                #region AGV -> 설비 (Unload)
                if (DIO.GetDIData(FuncInline.enumDINames.X03_0_PIO_0_valid + AGVnumber) &&
                    !DIO.GetDIData(FuncInline.enumDINames.X03_1_PIO_0_CS_0 + AGVnumber) &&
                    DIO.GetDIData(FuncInline.enumDINames.X03_2_PIO_0_CS_1 + AGVnumber) &&
                    !DIO.GetDIData(FuncInline.enumDINames.X03_3_PIO_0_CS_2 + AGVnumber))
                {
                    GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Docking;
                    #region Job Code Receive
                    //AGV가 도킹 되고 Job 코드를 받았을 때 비트를 변수 변경이 필요하다.
                    if (GlobalVar.AGVJobCodeReceive_0 &&//메인(다른 쓰레드)에서 리시브 변수 변경
                        !GlobalVar.AGVReady_0)
                    {
                        GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Load;
                        AGVJobCodeReceive_U(AGVnumber);
                    }
                    else
                    {
                        AGVJobCodeReceive_S(AGVnumber);
                    }
                    #endregion

                    #region Ready Receive
                    //제품을 받을 준비가 되었을 때 레디 비트를 살려줘야 한다.
                    if (GlobalVar.AGVReady_0)//메인(다른 쓰레드)에서 레디 변수 변경
                    {
                        //TR 받았을 때
                        if (DIO.GetDIData(FuncInline.enumDINames.X03_4_PIO_0_tr_req + AGVnumber) &&
                            !DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkReady;
                            AGVChechWorkReady_On(AGVnumber);
                            if (DIO.GetDIData(FuncInline.enumDINames.X03_5_PIO_0_busy + AGVnumber))
                            {
                                GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Working;
                                GlobalVar.AGVJobCodeReceive_0 = false;
                            }
                        }
                        //컴플리트 받았을 때
                        else if (DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkFinish;
                            AGVChechWorkReady_Off(AGVnumber);
                            GlobalVar.AGVReady_0 = false;
                        }
                    }
                    #endregion
                }
                #endregion
                #region 설비 -> AGV
                else if (DIO.GetDIData(FuncInline.enumDINames.X03_0_PIO_0_valid + AGVnumber) &&
                    DIO.GetDIData(FuncInline.enumDINames.X03_1_PIO_0_CS_0 + AGVnumber) &&
                    DIO.GetDIData(FuncInline.enumDINames.X03_2_PIO_0_CS_1 + AGVnumber) &&
                    !DIO.GetDIData(FuncInline.enumDINames.X03_3_PIO_0_CS_2 + AGVnumber))
                {
                    GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Docking;
                    //임시
                    GlobalVar.AGVJobCodeReceive_0 = true;
                    #region Job Code Receive
                    //AGV가 도킹 되고 Job 코드를 받았을 때 비트를 변수 변경이 필요하다.
                    if (GlobalVar.AGVJobCodeReceive_0 &&//메인(다른 쓰레드)에서 리시브 변수 변경
                        !GlobalVar.AGVReady_0)
                    {
                        GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.UnLoad;
                        AGVJobCodeReceive_L(AGVnumber);
                        //임시
                        GlobalVar.AGVReady_0 = true;
                    }
                    else
                    {
                        AGVJobCodeReceive_S(AGVnumber);
                    }
                    #endregion

                    #region Ready Receive
                    //제품을 받을 준비가 되었을 때 레디 비트를 살려줘야 한다.
                    if (GlobalVar.AGVReady_0)//메인(다른 쓰레드)에서 레디 변수 변경
                    {
                        //TR 받았을 때
                        if (DIO.GetDIData(FuncInline.enumDINames.X03_4_PIO_0_tr_req + AGVnumber) &&
                            !DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkReady;
                            AGVChechWorkReady_On(AGVnumber);
                            if (DIO.GetDIData(FuncInline.enumDINames.X03_5_PIO_0_busy + AGVnumber))
                            {
                                GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Working;
                                GlobalVar.AGVJobCodeReceive_0 = false;
                            }
                        }
                        //컴플리트 받았을 때
                        else if (DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkFinish;
                            AGVChechWorkReady_Off(AGVnumber);
                            GlobalVar.AGVReady_0 = false;
                        }
                    }
                    #endregion
                }
                #endregion
                #region  충전 시작
                #endregion
                #region  충전 종료
                #endregion
                #region 글로벌 변수 초기화
                else
                {
                    GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.None;
                    GlobalVar.AGVJobCodeReceive_0 = false;
                    GlobalVar.AGVReady_0 = false;
                }
                #endregion
            }
            #endregion

            #region 세번째 AGV              확인 해야 됨.
            if (AGVnumber == -16)
            {
            }
            #endregion
        }
        public static void AGVJobCodeReceive_U(int AGVnumber)
        {
            DIO.WriteDOData(FuncInline.enumDONames.Y03_0_PIO_0_L_REQ + AGVnumber, false);
            DIO.WriteDOData(FuncInline.enumDONames.Y03_1_PIO_0_U_REQ + AGVnumber, true);
        }
        public static void AGVJobCodeReceive_L(int AGVnumber)
        {
            DIO.WriteDOData(FuncInline.enumDONames.Y03_0_PIO_0_L_REQ + AGVnumber, true);
            DIO.WriteDOData(FuncInline.enumDONames.Y03_1_PIO_0_U_REQ + AGVnumber, false);
        }
        public static void AGVJobCodeReceive_S(int AGVnumber)
        {
            DIO.WriteDOData(FuncInline.enumDONames.Y03_0_PIO_0_L_REQ + AGVnumber, false);
            DIO.WriteDOData(FuncInline.enumDONames.Y03_1_PIO_0_U_REQ + AGVnumber, false);
        }
        #endregion

        #region AGV 작업 개시 요청    
        public static void AGVChechWorkReady_On(int AGVnumber)
        {
            DIO.WriteDOData(FuncInline.enumDONames.Y03_3_PIO_0_ready + AGVnumber, true);
        }
        public static void AGVChechWorkReady_Off(int AGVnumber)
        {
            DIO.WriteDOData(FuncInline.enumDONames.Y03_3_PIO_0_ready + AGVnumber, false);
        }
        #endregion

        #region AGV 작업 완료 확인
        public static void AGVChechWorkfinish(int AGVnumber)
        {
            if (DIO.GetDIData(FuncInline.enumDINames.X03_5_PIO_0_busy + AGVnumber) &&
                !DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber) &&
                DIO.GetDIData(FuncInline.enumDINames.X03_7_PIO_0_cont + AGVnumber))
            {
                GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkFinish;
            }
            else if (DIO.GetDIData(FuncInline.enumDINames.X03_5_PIO_0_busy + AGVnumber) &&
                DIO.GetDIData(FuncInline.enumDINames.X03_6_PIO_0_compt + AGVnumber) &&
                DIO.GetDIData(FuncInline.enumDINames.X03_7_PIO_0_cont + AGVnumber))
            {
                GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkContinue;
            }
        }
        #endregion

        #endregion
        */
        #endregion

        public static string decodeSecsName(byte s, byte f) // YJ추가. SECS GEM 통신시 stream과 function에 따른 이름을 리턴. 유효성 검사에도 이용.
        {
            string name = "Unknown"; // 모든 stream + function 0

            if (s == 1)
            {
                switch (f)
                {
                    case 1:
                        name = "AreYouThrere";
                        break;
                    case 2:
                        name = "OnLineData";
                        break;
                    case 3:
                        name = "SelectedEquipmentStatusRequest";
                        break;
                    case 4:
                        name = "SelectedEquipmentStatusData";
                        break;
                    //case 11:
                    //    name = "StatusVariableNameListRequest";
                    //    break;
                    //case 12:
                    //    name = "StatusVariableNameListReply";
                    //    break;
                    case 13:
                        name = "EstablishCommunicationRequest";
                        break;
                    case 14:
                        name = "EstablishCommunicationRequestAck";
                        break;
                    //case 15:
                    //    name = "ReqeustOffline";
                    //    break;
                    //case 16:
                    //    name = "OfflineAck";
                    //    break;
                    //case 17:
                    //    name = "RequestOnline";
                    //    break;
                    //case 18:
                    //    name = "OnlineAck";
                    //    break;
                    case 0:
                        name = "AbortTransaction";
                        break;
                    default:
                        name = "UnknownFunction";
                        break;
                }
            }
            else if (s == 2)
            {
                switch (f)
                {
                    //case 13:
                    //    name = "EquipmentConstantRequest";
                    //    break;
                    //case 14:
                    //    name = "EquipmentConstantData";
                    //    break;
                    //case 15:
                    //    name = "NewEquipmentConstantSend";
                    //    break;
                    //case 16:
                    //    name = "NewEquipmentConstantAck";
                    //    break;
                    //case 17:
                    //    name = "DateTimeRequest";
                    //    break;
                    //case 18:
                    //    name = "DateTimeData";
                    //    break;
                    //case 23:
                    //    name = "TraceInitializeSend";
                    //    break;
                    //case 24:
                    //    name = "TraceInitializeAck";
                    //    break;
                    //case 29:
                    //    name = "EquipmentConstantNameListRequest";
                    //    break;
                    //case 30:
                    //    name = "EquipmentConstantNameList";
                    //    break;
                    case 31:
                        name = "DateTimeRequest";
                        break;
                    case 32:
                        name = "DateTimeAck";
                        break;
                    case 33:
                        name = "DefineReport";
                        break;
                    case 34:
                        name = "DefineReportAck";
                        break;
                    case 35:
                        name = "LinkEventReport";
                        break;
                    case 36:
                        name = "LinkEventReportAck";
                        break;
                    case 37:
                        name = "EnableDisableEventReport";
                        break;
                    case 38:
                        name = "EnableDisableEventReportAck";
                        break;
                    //case 39:
                    //    name = "MultiBlockInquire";
                    //    break;
                    //case 40:
                    //    name = "MultiBlockGrant";
                    //    break;
                    case 41:
                        name = "HostCommandSend";
                        break;
                    case 42:
                        name = "HostCommandAck";
                        break;
                    //case 43:
                    //    name = "ResetSpoolingStreamsFunctions";
                    //    break;
                    //case 44:
                    //    name = "ResetSpoolingAcknowledge";
                    //    break;
                    //case 49:
                    //    name = "EnhancedRemoteCommand";
                    //    break;
                    //case 50:
                    //    name = "EnhancedRemoteCommandAck";
                    //    break;
                    case 0:
                        name = "AbortTransaction";
                        break;
                    default:
                        name = "UnknownFunction";
                        break;
                }
            }
            //else if (s == 3)
            //{
            //    switch (f)
            //    {
            //        case 17:
            //            name = "CarrierActionRequest";
            //            break;
            //        case 18:
            //            name = "CarrierActionAcknowledge";
            //            break;
            //        //case 19:
            //        //    name = "";
            //        //    break;
            //        //case 20:
            //        //    name = "";
            //        //    break;
            //        case 25:
            //            name = "PortAction";
            //            break;
            //        case 26:
            //            name = "PortActionAcknowledge";
            //            break;
            //        case 27:
            //            name = "ChangeAccess";
            //            break;
            //        case 28:
            //            name = "ChangeAccessAcknowledge";
            //            break;
            //        case 0:
            //            name = "AbortTransaction";
            //            break;
            //    }
            //}
            else if (s == 5)
            {
                switch (f)
                {
                    case 1:
                        name = "AlarmReportSend";
                        break;
                    case 2:
                        name = "AlarmReportAck";
                        break;
                    case 3:
                        name = "EnableDisableAlarmSend";
                        break;
                    case 4:
                        name = "EnableDisableAlarmAck";
                        break;
                    //case 5:
                    //    name = "ListAlarmRequest";
                    //    break;
                    //case 6:
                    //    name = "ListAlarmData";
                    //    break;
                    //case 7:
                    //    name = "ListEnabledAlarmRequest";
                    //    break;
                    //case 8:
                    //    name = "ListEnabledAlarmData";
                    //    break;
                    case 0:
                        name = "AbortTransaction";
                        break;
                    default:
                        name = "UnknownFunction";
                        break;
                }
            }
            else if (s == 6)
            {
                switch (f)
                {
                    //case 1:
                    //    name = "TraceDataSend";
                    //    break;
                    //case 2:
                    //    name = "TraceDataAck";
                    //    break;
                    //case 5:
                    //    name = "";
                    //    break;
                    //case 6:
                    //    name = "";
                    //    break;
                    case 11:
                        name = "EventReportSend";
                        break;
                    case 12:
                        name = "EventReportSendAck";
                        break;
                    //case 15:
                    //    name = "EventReportRequest";
                    //    break;
                    //case 16:
                    //    name = "EventReportData";
                    //    break;
                    //case 19:
                    //    name = "IndividualEventReportRequest";
                    //    break;
                    //case 20:
                    //    name = "IndividualEventReportData";
                    //    break;
                    //case 23:
                    //    name = "RequestSpoolData";
                    //    break;
                    //case 24:
                    //    name = "RequestSpoolDataAck";
                    //    break;
                    case 0:
                        name = "AbortTransaction";
                        break;
                    default:
                        name = "UnknownFunction";
                        break;
                }
            }
            else if (s == 7)
            {
                switch (f)
                {
                    //case 1:
                    //    name = "ProcessProgramLoadInquire";
                    //    break;
                    //case 2:
                    //    name = "ProcessProgramLoadGrant";
                    //    break;
                    case 3:
                        name = "ProcessProgramSend";
                        break;
                    case 4:
                        name = "ProcessProgramAck";
                        break;
                    //case 5:
                    //    name = "ProcessProgramRequest";
                    //    break;
                    //case 6:
                    //    name = "ProcessProgramData";
                    //    break;
                    //case 17:
                    //    name = "DeleteProcessProgramSend";
                    //    break;
                    //case 18:
                    //    name = "DeleteProcessProgramSendAck";
                    //    break;
                    case 19:
                        name = "CurrentEPPDRequestReport";
                        break;
                    case 20:
                        name = "CurrentEPPDData";
                        break;
                    //case 23:
                    //    name = "FormattedProcessProgramSend";
                    //    break;
                    //case 24:
                    //    name = "FormattedProcessProgramAck";
                    //    break;
                    //case 25:
                    //    name = "FormattedProcessProgramRequest";
                    //    break;
                    //case 26:
                    //    name = "FormattedProcessProgramData";
                    //    break;
                    //case 27:
                    //    name = "ProcessProgramVerificationSend";
                    //    break;
                    //case 28:
                    //    name = "ProcessProgramVerificationAck";
                    //    break;
                    //case 29:
                    //    name = "ProcessProgramVerificationInquire";
                    //    break;
                    //case 30:
                    //    name = "ProcessProgramVerificationGrant";
                    //    break;
                    case 0:
                        name = "AbortTransaction";
                        break;
                    default:
                        name = "UnknownFunction";
                        break;
                }
            }
            else if (s == 9)
            {
                switch (f)
                {
                    case 1:
                        name = "UnrecognizedDeviceID";
                        break;
                    case 3:
                        name = "UnrecognizedStreamType";
                        break;
                    case 5:
                        name = "UnrecognizedFunctionType";
                        break;
                    case 7:
                        name = "IllegalData";
                        break;
                    case 9:
                        name = "TransactionTimerTimeout";
                        break;
                    case 11:
                        name = "DataTooLong";
                        break;
                    case 13:
                        name = "ConversationTimeout";
                        break;
                    case 0:
                        name = "AbortTransaction";
                        break;
                    default:
                        name = "UnknownFunction";
                        break;
                }
            }
            else if (s == 10)
            {
                switch (f)
                {
                    //case 1:
                    //    name = "TerminalRequest";
                    //    break;
                    //case 2:
                    //    name = "TerminalRequestAck";
                    //    break;
                    case 3:
                        name = "TerminalDisplaySingle";
                        break;
                    case 4:
                        name = "TerminalDisplaySingleAck";
                        break;
                    //case 5:
                    //    name = "TerminalDisplayMulti";
                    //    break;
                    //case 6:
                    //    name = "TerminalDisplayMultiAcknowledge";
                    //    break;
                    //case 7:
                    //    name = "MultiBlockNotAllowed";
                    //    break;
                    case 0:
                        name = "AbortTransaction";
                        break;
                    default:
                        name = "UnknownFunction";
                        break;
                }
            }
            //if (s == 14)
            //{
            //    switch (f)
            //    {
            //        case 1:
            //            name = "GetAttributeRequest";
            //            break;
            //        case 2:
            //            name = "GetAttributeData";
            //            break;
            //        case 3:
            //            name = "SetAttributeRequest";
            //            break;
            //        case 4:
            //            name = "SetAttributeData";
            //            break;
            //        case 7:
            //            name = "GetAttributeName";
            //            break;
            //        case 8:
            //            name = "GetAttributeNameData";
            //            break;
            //        case 9:
            //            name = "ControlJobCreate";
            //            break;
            //        case 10:
            //            name = "ControlJobCreateAcknowledge";
            //            break;
            //        case 11:
            //            name = "ControlJobDeleteRequest";
            //            break;
            //        case 12:
            //            name = "ControlJobDeleteAcknowledge";
            //            break;
            //        case 0:
            //            name = "AbortTransaction";
            //            break;
            //    }
            //}
            //if (s == 16)
            //{
            //    switch (f)
            //    {
            //        case 5:
            //            name = "ProcessJobCommand";
            //            break;
            //        case 6:
            //            name = "ProcessJobCommandAcknowledge";
            //            break;
            //        case 11:
            //            name = "ProcessJobCreateEnh";
            //            break;
            //        case 12:
            //            name = "ProcessJobCreateEnhAcknowledge";
            //            break;
            //        case 15:
            //            name = "ProcessJobMultiCreate";
            //            break;
            //        case 16:
            //            name = "ProcessJobMultiCreateAcknowledge";
            //            break;
            //        case 17:
            //            name = "ProcessJobDequeue";
            //            break;
            //        case 18:
            //            name = "ProcessJobDequeueCreate";
            //            break;
            //        case 19:
            //            name = "PRGetAllJobs";
            //            break;
            //        case 20:
            //            name = "PRGetAllJobsSend";
            //            break;
            //        case 21:
            //            name = "PRGetSpace";
            //            break;
            //        case 22:
            //            name = "PRGetAllJobsSend";
            //            break;
            //        //case 25:
            //        //    name = "";
            //        //    break;
            //        //case 26:
            //        //    name = "";
            //        //    break;
            //        case 27:
            //            name = "ControlJobCommandRequest";
            //            break;
            //        case 28:
            //            name = "ControlJobCommandAcknowledge";
            //            break;
            //        case 0:
            //            name = "AbortTransaction";
            //            break;
            //    }
            //}
            else
            {
                name = "UnknownStream";
            }

            Console.WriteLine("S" + s + "F" + f + " : " + name);

            return name;
        }

        public static bool checkImageDirectory() // 이미지 폴더를 초기 생성
        {
            bool noDirectory = false;
            string defaultPath = GlobalVar.FaPath;
            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
                noDirectory = true;
            }
            defaultPath += "\\" + GlobalVar.SWName;
            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
                noDirectory = true;
            }
            defaultPath += "\\" + GlobalVar.ArrayImagePath;
            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
                noDirectory = true;
            }
            return !noDirectory;
        }



        #region INI 관련
        public static void LoadAllIni() // 모든 설정 읽기
        {
            LoadMachinenIni();
            LoadPinIni();
            LoadPortIni();
            //LoadRobotIni();
            LoadModelIni();
            LoadTeachingIni();
            //LoadPositionOffsetIni();
            //LoadWidthOffsetIni();
            LoadTestSiteIni();
            LoadTowerIni();
            LoadSiteOrder();
        }

        public static void SaveAllIni() // 모든 설정 쓰기
        {
            SaveMachineIni();
            SavePinIni();
            SavePortIni();
            //SaveRobotIni();
            SaveModelIni();
            SaveTeachingIni();
            //SavePositionOffsetIni();
            //SaveWidthOffsetIni();
            SaveTestSiteIni();
            SaveTowerIni();
            SaveSiteOrder();
        }

        public static void LoadSimulationIni() // 장비의 모든 설정 읽기
        {
            try
            {
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\Simulation.ini";
                string Section = GlobalVar.IniSection;

                GlobalVar.Simulation = FuncFile.ReadIniFile(Section, "Simulation", IniPath, "False") == "True";
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }



        public static void LoadMachinenIni() // 장비의 모든 설정 읽기
        {
            try
            {
                GlobalVar.ManagePasswd = FuncFile.ReadIniFile("manage", "pwd", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\manage.ini", "1234");

                #region general
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini";
                string Section = GlobalVar.IniSection;

                GlobalVar.AppName = FuncFile.ReadIniFile(Section, "appName", IniPath, "Module Auto Inline Test System");
                GlobalVar.CoCode = FuncFile.ReadIniFile(Section, "CoCode", IniPath, "9");
                GlobalVar.MachineNumber = int.Parse(FuncFile.ReadIniFile(Section, "MachineNumber", IniPath, "1"));
                GlobalVar.MachineLeftRight = (enumMachineLeftRight)int.Parse(FuncFile.ReadIniFile(Section, "MachineLeftRight", IniPath, "0"));
                GlobalVar.MachineFrontRear = (enumMachineFrontRear)int.Parse(FuncFile.ReadIniFile(Section, "MachineFrontRear", IniPath, "0"));
                GlobalVar.MachineLinear = (enumMachineLinear)int.Parse(FuncFile.ReadIniFile(Section, "MachineLinear", IniPath, "0"));
                GlobalVar.LogFileDeleteDay = int.Parse(FuncFile.ReadIniFile(Section, "LogFileDeleteDay", IniPath, "30"));


                FuncInline.LiftHomeSpeed = double.Parse(FuncFile.ReadIniFile(Section, "LiftHomeSpeed", IniPath, "100"));

                FuncInline.PinLifeDate = int.Parse(FuncFile.ReadIniFile(Section, "PinLifeDate", IniPath, "1000"));
                FuncInline.PinLifeCount = int.Parse(FuncFile.ReadIniFile(Section, "PinLifeCount", IniPath, "1000"));
                FuncInline.CheckPinLife = FuncFile.ReadIniFile(Section, "CheckPinLife", IniPath, "False") == "True";
                FuncInline.LiftSpeed = int.Parse(FuncFile.ReadIniFile(Section, "LiftSpeed", IniPath, "100"));
                FuncInline.LiftAccDec = int.Parse(FuncFile.ReadIniFile(Section, "LiftAccDec", IniPath, "1"));
                FuncInline.ConveyorTimeout = int.Parse(FuncFile.ReadIniFile(Section, "ConveyorTimeout", IniPath, "10"));
                FuncInline.ScanTimeout = int.Parse(FuncFile.ReadIniFile(Section, "ScanTimeout", IniPath, "10"));
                FuncInline.TestCommandTimeout = int.Parse(FuncFile.ReadIniFile(Section, "TestCommandTimeout", IniPath, "10"));
                FuncInline.TestCommandRetry = int.Parse(FuncFile.ReadIniFile(Section, "TestCommandRetry", IniPath, "10"));
                FuncInline.TestPassTime = int.Parse(FuncFile.ReadIniFile(Section, "TestPassTime", IniPath, "300"));
                //NgAlarm = FuncFile.ReadIniFile(Section, "NgAlarm", IniPath, "False") == "True";
                FuncInline.NgAlarmTime = int.Parse(FuncFile.ReadIniFile(Section, "NgAlarmTime", IniPath, "5"));
                //NGReEnterCount = int.Parse(FuncFile.ReadIniFile(Section, "NGReEnterCount", IniPath, "2"));
                FuncInline.BlockNGArray = FuncFile.ReadIniFile(Section, "BlockNGArray", IniPath, "False") == "True";
                FuncInline.BlockNGCount = int.Parse(FuncFile.ReadIniFile(Section, "BlockNGCount", IniPath, "3"));
                FuncInline.BlockDefectArray = FuncFile.ReadIniFile(Section, "BlockDefectArray", IniPath, "False") == "True";
                FuncInline.DefectBlockMinIn = int.Parse(FuncFile.ReadIniFile(Section, "DefectBlockMinIn", IniPath, "30"));
                FuncInline.DefectBlockMinNG = int.Parse(FuncFile.ReadIniFile(Section, "DefectBlockMinNG", IniPath, "30"));
                FuncInline.ScanTwice = FuncFile.ReadIniFile(Section, "ScanTwice", IniPath, "False") == "True";
                FuncInline.CheckCNDuplication = FuncFile.ReadIniFile(Section, "CheckCNDuplication", IniPath, "False") == "True";
                FuncInline.CheckCNDupeCount = int.Parse(FuncFile.ReadIniFile(Section, "CheckCNDupeCount", IniPath, "1"));
                FuncInline.CheckCNCross = FuncFile.ReadIniFile(Section, "CheckCNCross", IniPath, "False") == "True";

                FuncInline.SelfRetest = FuncFile.ReadIniFile(Section, "SelfRetest", IniPath, "False") == "True";
                FuncInline.OtherRetest = FuncFile.ReadIniFile(Section, "OtherRetest", IniPath, "False") == "True";
                FuncInline.FailWhenNoEmpty = FuncFile.ReadIniFile(Section, "FailWhenNoEmpty", IniPath, "False") == "True";
                FuncInline.SelfRetestCount = int.Parse(FuncFile.ReadIniFile(Section, "SelfRetestCount", IniPath, "3"));
                FuncInline.OtherRetestCount = int.Parse(FuncFile.ReadIniFile(Section, "OtherRetestCount", IniPath, "3"));
                FuncInline.NGToUnloading = FuncFile.ReadIniFile(Section, "NGToUnloading", IniPath, "False") == "True";
                FuncInline.PassToNG = FuncFile.ReadIniFile(Section, "PassToNG", IniPath, "False") == "True";
                FuncInline.PinDownAndClamp = FuncFile.ReadIniFile(Section, "PinDownAndClamp", IniPath, "False") == "True";
                FuncInline.TestWithSiteUnclamp = FuncFile.ReadIniFile(Section, "TestWithSiteUnclamp", IniPath, "False") == "True";
                FuncInline.ScanInsertCheck = FuncFile.ReadIniFile(Section, "ScanInsertCheck", IniPath, "False") == "True";


                FuncInline.CoolingByTime = FuncFile.ReadIniFile(Section, "CoolingByTime", IniPath, "False") == "True";
                FuncInline.CoolingByTemperature = FuncFile.ReadIniFile(Section, "CoolingByTemperature", IniPath, "False") == "True";
                FuncInline.CoolingTime = int.Parse(FuncFile.ReadIniFile(Section, "CoolingTime", IniPath, "10"));
                FuncInline.CoolingTemperature = int.Parse(FuncFile.ReadIniFile(Section, "CoolingTemperature", IniPath, "30"));
                FuncInline.CoolingMaxTime = int.Parse(FuncFile.ReadIniFile(Section, "CoolingMaxTime", IniPath, "30"));

                FuncInline.ShiftAHour = int.Parse(FuncFile.ReadIniFile(Section, "ShiftAHour", IniPath, "9"));
                FuncInline.ShiftAMin = int.Parse(FuncFile.ReadIniFile(Section, "ShiftAMin", IniPath, "0"));
                FuncInline.ShiftBHour = int.Parse(FuncFile.ReadIniFile(Section, "ShiftBHour", IniPath, "15"));
                FuncInline.ShiftBMin = int.Parse(FuncFile.ReadIniFile(Section, "ShiftBMin", IniPath, "0"));
                FuncInline.ShiftCHour = int.Parse(FuncFile.ReadIniFile(Section, "ShiftCHour", IniPath, "23"));
                FuncInline.ShiftCMin = int.Parse(FuncFile.ReadIniFile(Section, "ShiftCMin", IniPath, "0"));

                FuncInline.DefaultPCBWidth = double.Parse(FuncFile.ReadIniFile(Section, "DefaultPCBWidth", IniPath, "190"));
                FuncInline.UseShiftC = FuncFile.ReadIniFile(Section, "UseShiftC", IniPath, "True") == "True";

                //JHRYU
                FuncInline.SystemLogPath = FuncFile.ReadIniFile(Section, "SystemLogPath", IniPath, GlobalVar.FaPath);
                FuncInline.SystemLogTime = int.Parse(FuncFile.ReadIniFile(Section, "SystemLogTime", IniPath, "30"));

                FuncInline.SiteClampDelay = double.Parse(FuncFile.ReadIniFile(Section, "SiteClampDelay", IniPath, "1"));

                FuncInline.UseSMDReady = FuncFile.ReadIniFile(Section, "SMDReady", IniPath, "False") == "True";
                //FuncInline.UseSiteClampSensor = FuncFile.ReadIniFile(Section, "UseSiteClampSensor", IniPath, "False") == "True";
                FuncInline.WidthClampOffset = double.Parse(FuncFile.ReadIniFile(Section, "WidthClampOffset", IniPath, "0"));
                FuncInline.PassModeBuffer = FuncFile.ReadIniFile(Section, "PassModeBuffer", IniPath, "False") == "True";
                FuncInline.LeaveOneSite = FuncFile.ReadIniFile(Section, "LeaveOneSite", IniPath, "False") == "True";

                FuncInline.ScanSize.x = int.Parse(FuncFile.ReadIniFile(Section, "ScanStartX", IniPath, "1000"));
                FuncInline.ScanSize.y = int.Parse(FuncFile.ReadIniFile(Section, "ScanStartY", IniPath, "0"));
                FuncInline.ScanSize.z = int.Parse(FuncFile.ReadIniFile(Section, "ScanEndX", IniPath, "6000"));
                FuncInline.ScanSize.a = int.Parse(FuncFile.ReadIniFile(Section, "ScanEndY", IniPath, "5120"));
                #endregion
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void LoadTestSiteIni() // 장비의 모든 설정 읽기
        {
            try
            {
                GlobalVar.ManagePasswd = FuncFile.ReadIniFile("manage", "pwd", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\manage.ini", "1234");

                #region Test Site
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini";
                string Section = GlobalVar.IniSection;

                FuncInline.PinLifeDate = int.Parse(FuncFile.ReadIniFile(Section, "PinLifeDate", IniPath, "1000"));
                FuncInline.PinLifeCount = int.Parse(FuncFile.ReadIniFile(Section, "PinLifeCount", IniPath, "1000"));
                FuncInline.CheckPinLife = FuncFile.ReadIniFile(Section, "CheckPinLife", IniPath, "False") == "True";

                for (int i = 0; i < FuncInline.UseSite.Length; i++)
                {
                    FuncInline.UseSite[i] = FuncFile.ReadIniFile(Section, "UseSite_" + (i + 1), IniPath, "False") == "True";
                }
                for (int i = 0; i < FuncInline.ReTestSite.Length; i++)
                {
                    FuncInline.ReTestSite[i] = FuncFile.ReadIniFile(Section, "ReTestSite_" + (i + 1), IniPath, "False") == "True";
                }
                FuncInline.ReTestOnly = FuncFile.ReadIniFile(Section, "ReTestOnly", IniPath, "False") == "True";
                string testSite1 = FuncFile.ReadIniFile(Section, "BuyerChangeSite1", IniPath, "Site1");
                FuncInline.BuyerChangeSite[0] = FuncInline.enumTeachingPos.Site1_F_DT1 + (int.Parse(testSite1.Replace("Site","")) - 1);
                string testSite2 = FuncFile.ReadIniFile(Section, "BuyerChangeSite2", IniPath, "Site8");
                FuncInline.BuyerChangeSite[1] = FuncInline.enumTeachingPos.Site1_F_DT1 + (int.Parse(testSite2.Replace("Site", "")) - 1);
                string testSite3 = FuncFile.ReadIniFile(Section, "BuyerChangeSite3", IniPath, "Site15");
                FuncInline.BuyerChangeSite[2] = FuncInline.enumTeachingPos.Site1_F_DT1 + (int.Parse(testSite3.Replace("Site", "")) - 1);
                #endregion
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }

        public static void LoadTowerIni() // 장비의 모든 설정 읽기
        {
            try
            {
                string Section = GlobalVar.IniSection;
                GlobalVar.ManagePasswd = FuncFile.ReadIniFile("manage", "pwd", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\manage.ini", "1234");

                #region Tower Lamp
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\Tower.ini";

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            GlobalVar.TowerAction[i, j, k] = FuncFile.ReadIniFile(i.ToString(), j.ToString() + "_" + k.ToString(), IniPath, "False") == "True";
                        }
                    }
                    GlobalVar.TowerTime[i] = ulong.Parse(FuncFile.ReadIniFile(i.ToString(), "time", IniPath, "0"));
                }
                #endregion

                GlobalVar.InStopTime = int.Parse(FuncFile.ReadIniFile(Section, "InStopTime", IniPath, "300"));
                GlobalVar.OutStopTime = int.Parse(FuncFile.ReadIniFile(Section, "OutStopTime", IniPath, "300"));
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SaveMachineIni() // 장비의 모든 설정 쓰기
        {
            try
            {
                string Section = GlobalVar.IniSection;
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\setting.ini";


                #region general            

                FuncFile.WriteIniFile(Section, "appName", GlobalVar.AppName, IniPath);
                FuncFile.WriteIniFile(Section, "coCode", GlobalVar.CoCode, IniPath);
                FuncFile.WriteIniFile(Section, "MachineNumber", GlobalVar.MachineNumber.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "MachineLeftRight", ((int)GlobalVar.MachineLeftRight).ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "MachineFrontRear", ((int)GlobalVar.MachineFrontRear).ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "MachineLinear", ((int)GlobalVar.MachineLinear).ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "LogFileDeleteDay", GlobalVar.LogFileDeleteDay.ToString(), IniPath);


                FuncFile.WriteIniFile(Section, "DefaultModel", GlobalVar.ModelName, IniPath);

                FuncFile.WriteIniFile(Section, "LiftHomeSpeed", FuncInline.LiftHomeSpeed.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "WidthHomSpeed", GlobalVar.WidthHomSpeed.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "WidthSpeed", GlobalVar.WidthSpeed.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "WidthAccDec", GlobalVar.WidthAccDec.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "LiftSpeed", FuncInline.LiftSpeed.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "LiftAccDec", FuncInline.LiftAccDec.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "ConveyorTimeout", FuncInline.ConveyorTimeout.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ScanTimeout", FuncInline.ScanTimeout.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "TestCommandTimeout", FuncInline.TestCommandTimeout.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "TestCommandRetry", FuncInline.TestCommandRetry.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "TestPassTime", FuncInline.TestPassTime.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "NgAlarm", FuncInline.NgAlarm.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "NgAlarmTime", FuncInline.NgAlarmTime.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "NGReEnterCount", FuncInline.NGReEnterCount.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "BlockNGArray", FuncInline.BlockNGArray.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "BlockNGCount", FuncInline.BlockNGCount.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "BlockDefectArray", FuncInline.BlockDefectArray.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "BlockNGCount", FuncInline.BlockNGCount.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "DefectBlockMinIn", FuncInline.DefectBlockMinIn.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "DefectBlockMinNG", FuncInline.DefectBlockMinNG.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CheckCNDuplication", FuncInline.CheckCNDuplication.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CheckCNDupeCount", FuncInline.CheckCNDupeCount.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CheckCNCross", FuncInline.CheckCNCross.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "SelfRetest", FuncInline.SelfRetest.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "OtherRetest", FuncInline.OtherRetest.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "FailWhenNoEmpty", FuncInline.FailWhenNoEmpty.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "SelfRetestCount", FuncInline.SelfRetestCount.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "OtherRetestCount", FuncInline.OtherRetestCount.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "NGToUnloading", FuncInline.NGToUnloading.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PassToNG", FuncInline.PassToNG.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PinDownAndClamp", FuncInline.PinDownAndClamp.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "TestWithSiteUnclamp", FuncInline.TestWithSiteUnclamp.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "PinLifeDate", FuncInline.PinLifeDate.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PinLifeCount", FuncInline.PinLifeCount.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CheckPinLife", FuncInline.CheckPinLife.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "CoolingByTime", FuncInline.CoolingByTime.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CoolingByTemperature", FuncInline.CoolingByTemperature.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CoolingTime", FuncInline.CoolingTime.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CoolingTemperature", FuncInline.CoolingTemperature.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CoolingMaxTime", FuncInline.CoolingMaxTime.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "ShiftAHour", FuncInline.ShiftAHour.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ShiftAMin", FuncInline.ShiftAMin.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ShiftBHour", FuncInline.ShiftBHour.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ShiftBMin", FuncInline.ShiftBMin.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ShiftCHour", FuncInline.ShiftCHour.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ShiftCMin", FuncInline.ShiftCMin.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "DefaultPCBWidth", FuncInline.DefaultPCBWidth.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "UseShiftC", FuncInline.UseShiftC.ToString(), IniPath);

                // JHRYU
                FuncFile.WriteIniFile(Section, "SystemLogPath", FuncInline.SystemLogPath, IniPath);
                FuncFile.WriteIniFile(Section, "SystemLogTime", FuncInline.SystemLogTime.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "SiteClampDelay", FuncInline.SiteClampDelay.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "SMDReady", FuncInline.UseSMDReady.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "UseSiteClampSensor", FuncInline.UseSiteClampSensor.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "WidthClampOffset", FuncInline.WidthClampOffset.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PassModeBuffer", FuncInline.PassModeBuffer.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "LeaveOneSite", FuncInline.LeaveOneSite.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "ScanStartX", FuncInline.ScanSize.x.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ScanStartY", FuncInline.ScanSize.y.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ScanEndX", FuncInline.ScanSize.z.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ScanEndY", FuncInline.ScanSize.a.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ScanInsertCheck", FuncInline.ScanInsertCheck.ToString(), IniPath);
                #endregion

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }

        public static void SaveTestSiteIni() // 장비의 모든 설정 쓰기
        {
            try
            {
                string Section = GlobalVar.IniSection;
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\setting.ini";


                #region Test Site

                FuncFile.WriteIniFile(Section, "PinLifeDate", FuncInline.PinLifeDate.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PinLifeCount", FuncInline.PinLifeCount.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CheckPinLife", FuncInline.CheckPinLife.ToString(), IniPath);

                for (int i = 0; i < FuncInline.UseSite.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "UseSite_" + (i + 1), FuncInline.UseSite[i].ToString(), IniPath);
                }
                for (int i = 0; i < FuncInline.ReTestSite.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "ReTestSite_" + (i + 1), FuncInline.ReTestSite[i].ToString(), IniPath);
                }
                FuncFile.WriteIniFile(Section, "ReTestOnly", FuncInline.ReTestOnly.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "BuyerChangeSite1", FuncInline.BuyerChangeSite[0].ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "BuyerChangeSite2", FuncInline.BuyerChangeSite[1].ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "BuyerChangeSite3", FuncInline.BuyerChangeSite[2].ToString(), IniPath);
                #endregion

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }

        public static void SaveTowerIni() // 장비의 모든 설정 쓰기
        {
            try
            {
                string Section = GlobalVar.IniSection;
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\setting.ini";


                #region Tower Lamp
                IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\Tower.ini";

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            FuncFile.WriteIniFile(i.ToString(), j.ToString() + "_" + k.ToString(), GlobalVar.TowerAction[i, j, k].ToString(), IniPath);
                        }
                    }
                    FuncFile.WriteIniFile(i.ToString(), "time", GlobalVar.TowerTime[i].ToString(), IniPath);
                }
                #endregion

                FuncFile.WriteIniFile(Section, "InStopTime", GlobalVar.InStopTime.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "OutStopTime", GlobalVar.OutStopTime.ToString(), IniPath);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

        }

        public static void SaveSiteOrder() // 사이트 투입 순서 저장
        {
            try
            {
                string Section = GlobalVar.IniSection;
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\order.ini";

                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "SiteOrder" + i, FuncInline.SiteOrder[i].ToString(), IniPath);
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        /*
        public static void LoadPositionOffsetIni()
        {
            string Section = GlobalVar.IniSection;
            for (int i = 0; i < GlobalVar.PositionOffset.Length; i++)
            {
                GlobalVar.PositionOffset[i].x = double.Parse(FuncFile.ReadIniFile(Section, "PositionOffset.x_" + (i + 1).ToString(), GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "0"));
                GlobalVar.PositionOffset[i].y = double.Parse(FuncFile.ReadIniFile(Section, "PositionOffset.y_" + (i + 1).ToString(), GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "0"));
                GlobalVar.PositionOffset[i].z = double.Parse(FuncFile.ReadIniFile(Section, "PositionOffset.z_" + (i + 1).ToString(), GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "0"));
            }
        }
        //*/

        /*
        public static void LoadWidthOffsetIni()
        {
            string Section = GlobalVar.IniSection;
            for (int i = 0; i < FuncInline.WidthOffset.Length; i++)
            {
                FuncInline.WidthOffset[i] = double.Parse(FuncFile.ReadIniFile(Section, "WidthOffset_" + (i + 1).ToString(), GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "0"));
            }
        }
        //*/

        public static void LoadRobotIni()
        {
            string Section = GlobalVar.IniSection;
            //GlobalVar.SiteOffset.x = double.Parse(FuncFile.ReadIniFile(Section, "SiteOffset.x", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "100"));
            //GlobalVar.SiteOffset.z = double.Parse(FuncFile.ReadIniFile(Section, "SiteOffset.z", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "100"));
            //RobotInputOutputOffsetX = double.Parse(FuncFile.ReadIniFile(Section, "RobotInputOutputOffset", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "100"));
            //RobotGripShift = double.Parse(FuncFile.ReadIniFile(Section, "RobotGripShift", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "100"));
            //RobotWidthOffset = double.Parse(FuncFile.ReadIniFile(Section, "RobotWidthOffset", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "100"));
        }



        public static void LoadPinIni()
        {
            try
            {
                string Section = GlobalVar.IniSection;
                FuncInline.PinLifeDate = int.Parse(FuncFile.ReadIniFile(Section, "PinLifeDate", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "1000"));
                FuncInline.PinLifeCount = int.Parse(FuncFile.ReadIniFile(Section, "PinLifeCount", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "1000"));
                FuncInline.CheckPinLife = FuncFile.ReadIniFile(Section, "CheckPinLife", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "False") == "True";

                FuncInline.DefectLimit = double.Parse(FuncFile.ReadIniFile(Section, "DefectLimit", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "10"));
                FuncInline.PinLogTime = int.Parse(FuncFile.ReadIniFile(Section, "PinLogTime", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "30"));
                //PinLogDirectory = FuncFile.ReadIniFile(Section, "PinLogDirectory", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "");
                FuncInline.PinLogDirectory = FuncFile.ReadIniFile(Section, "PinLogDirectory", GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "D:\\SamsungMobile\\GlobalSIA\\Log");

                for (int i = 1; i <= FuncInline.MaxArrayCount; i++)
                {
                    FuncInline.PinArray[i - 1] = int.Parse(FuncFile.ReadIniFile(Section, "PinArray_" + i, GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini", "1"));
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SavePinIni()
        {
            try
            {
                string Section = GlobalVar.IniSection;
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\setting.ini";


                FuncFile.WriteIniFile(Section, "PinLifeDate", FuncInline.PinLifeDate.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PinLifeCount", FuncInline.PinLifeCount.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CheckPinLife", FuncInline.CheckPinLife.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "DefectLimit", FuncInline.DefectLimit.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PinLogTime", FuncInline.PinLogTime.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PinLogDirectory", FuncInline.PinLogDirectory.ToString(), IniPath);

                for (int i = 1; i <= FuncInline.MaxArrayCount; i++)
                {
                    FuncFile.WriteIniFile(Section, "PinArray_" + i, FuncInline.PinArray[i - 1].ToString(), IniPath);
                }

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void LoadTeachingPositionIni()
        {
            try
            {
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\Lift.ini";
                string Section = GlobalVar.IniSection;

                for (int j = 0; j < FuncInline.LiftPos.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.LiftPos.GetLength(1); i++)
                    {
                        FuncInline.LiftPos[j, i] = double.Parse(FuncFile.ReadIniFile(Section, "LiftPos" + j + "_" + i, IniPath, "0"));
                    }
                }
                for (int j = 0; j < FuncInline.ShuttlePos.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.ShuttlePos.GetLength(1); i++)
                    {
                        
                        FuncInline.ShuttlePos[j, i] = double.Parse(FuncFile.ReadIniFile(Section, "ShuttlePos" + j + "_" + i, IniPath, "0"));

                    }
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void LoadTeachingWidthIni()
        {
            try
            {
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\Width.ini";
                string Section = GlobalVar.IniSection;

                for (int i = 0; i < FuncInline.WidthOffset.Length; i++)
                {
                    FuncInline.WidthOffset[i] = double.Parse(FuncFile.ReadIniFile(Section, "TeachingWidth" + i, IniPath, "0"));
                    FuncInline.TeachingWidth[i] = FuncInline.PCBWidth + FuncInline.WidthOffset[i]; // apply건 load건 티칭계산이 다시 되었으면 티칭좌표에 적용
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

      

        public static void LoadTeachingIni() // 티칭의 모든 설정 읽기
        {
            LoadTeachingPositionIni();
            LoadTeachingWidthIni();
           
        }

        public static void LoadSiteOrder() // 사이트 투입 순서 읽기
        {
            try
            {
                string Section = GlobalVar.IniSection;
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\order.ini";

                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    FuncInline.SiteOrder[i] = FuncInline.enumTeachingPos.Site1_F_DT1 + int.Parse(FuncFile.ReadIniFile(Section, "SiteOrder" + i, IniPath, "Site1").Replace("Site","")) -1;
                }

                // 투입순서 중 좌측 첫 인덱스
                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    if (FuncInline.SiteOrder[i] >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                        FuncInline.SiteOrder[i] < FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.SitePerRack)
                    {
                        FuncInline.SiteInputIndex[0] = i;
                        break;
                    }
                }

                // 투입순서 중 가운데 첫 인덱스
                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    if (FuncInline.SiteOrder[i] >= FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.SitePerRack &&
                        FuncInline.SiteOrder[i] < FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.SitePerRack * 2)
                    {
                        FuncInline.SiteInputIndex[1] = i;
                        break;
                    }
                }

                // 투입순서 중 오른쪽 첫 인덱스
                for (int i = 0; i < FuncInline.SiteOrder.Length; i++)
                {
                    if (FuncInline.SiteOrder[i] >= FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.SitePerRack * 2)
                    {
                        FuncInline.SiteInputIndex[2] = i;
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SaveTeachingPositionIni()
        {
            try
            {
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\Lift.ini";

                string Section = GlobalVar.IniSection;

                for (int j = 0; j < FuncInline.LiftPos.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.LiftPos.GetLength(1); i++)
                    {
                        FuncFile.WriteIniFile(Section, "LiftPos" + j + "_" + i, FuncInline.LiftPos[j, i].ToString(), IniPath);
                    }
                }
                for (int j = 0; j < FuncInline.ShuttlePos.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.ShuttlePos.GetLength(1); i++)
                    {
                        FuncFile.WriteIniFile(Section, "ShuttlePos" + j + "_" + i, FuncInline.ShuttlePos[j, i].ToString(), IniPath);
                    }
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SaveTeachingWidthIni()
        {
            try
            {
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\Width.ini";

                string Section = GlobalVar.IniSection;


                for (int i = 0; i < FuncInline.WidthOffset.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "TeachingWidth" + i, FuncInline.WidthOffset[i].ToString(), IniPath);

                    //if ((FuncInline.enumTeachingPos)i != FuncInline.enumTeachingPos.LoadiNgShuttle)
                    //{
                    // 티칭좌표에는 저장이 안 되어 있어 티칭좌표 수정
                    FuncInline.TeachingWidth[i] = FuncInline.WidthOffset[i] + FuncInline.PCBWidth;
                    //}
                }

                // 티칭좌표에는 저장이 안 되어 있어 티칭좌표 수정
                //TeachingPos[(int)FuncInline.enumTeachingPos.LoadiNgShuttle].x = GlobalVar.PositionOffset[(int)FuncInline.enumTeachingPos.LoadiNgShuttle].x - FuncInline.PCBLength;
                //TeachingPos[(int)FuncInline.enumTeachingPos.LoadiNgShuttle].y = GlobalVar.PositionOffset[(int)FuncInline.enumTeachingPos.LoadiNgShuttle].y;
                //TeachingPos[(int)FuncInline.enumTeachingPos.LoadiNgShuttle].z = GlobalVar.PositionOffset[(int)FuncInline.enumTeachingPos.LoadiNgShuttle].z;

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SaveTeachingETCIni()
        {
            try
            {
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\ETC.ini";

                string Section = GlobalVar.IniSection;


                for (int i = 0; i < FuncInline.EtcPos.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "Teaching" + i, FuncInline.EtcPos[i].ToString(), IniPath);
                }
                FuncFile.WriteIniFile(Section, "JigBufferPitch", FuncInline.JigBufferPitch.ToString(), IniPath);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SaveTeachingIni() // 티칭의 모든 설정 쓰기
        {
            SaveTeachingPositionIni();
            SaveTeachingWidthIni();
            /*
            string IniPath = GlobalVar.FaPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.SWName;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.ModelPath;
            if (!Directory.Exists(GlobalVar.IniPath))
            {
                Directory.CreateDirectory(IniPath);
            }
            IniPath += "\\" + GlobalVar.ModelName + ".ini";

            string Section = GlobalVar.IniSection;


            for (int i = 0; i < FuncInline.TeachingWidth.Length; i++)
            {
                FuncFile.WriteIniFile(Section, "TeachingWidth" + i, FuncInline.TeachingWidth[i].ToString(), IniPath);
            }
            for (int i = 0; i < FuncInline.TeachingPos.Length; i++)
            {
                FuncFile.WriteIniFile(Section, "TeachingPosX" + i, FuncInline.TeachingPos[i].x.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "TeachingPosY" + i, FuncInline.TeachingPos[i].y.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "TeachingPosZ" + i, FuncInline.TeachingPos[i].z.ToString(), IniPath);
            }
            //*/
        }

        public static double LoadModelWidth(string modelName) // 모델의 폭 설정 읽기
        {
            try
            {
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ModelPath + "\\" + modelName + ".ini";
                string Section = GlobalVar.IniSection;

                return double.Parse(FuncFile.ReadIniFile(Section, "PCBWidth", IniPath, "70"));
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return 70;
        }

        public static void LoadModelIni() // 모델의 모든 설정 읽기
        {
            try
            {
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ModelPath + "\\" + GlobalVar.ModelName + ".ini";
                string Section = GlobalVar.IniSection;

                FuncInline.ArrayCount = int.Parse(FuncFile.ReadIniFile(Section, "ArrayCount", IniPath, "12"));

                //RobotTimeout = int.Parse(FuncFile.ReadIniFile(Section, "RobotTimeout", IniPath, "30"));
                //ConveyorTimeout = int.Parse(FuncFile.ReadIniFile(Section, "ConveyorTimeout", IniPath, "10"));
                //ScanTimeout = int.Parse(FuncFile.ReadIniFile(Section, "ScanTimeout", IniPath, "30"));
                //TestPassTime = int.Parse(FuncFile.ReadIniFile(Section, "TestPassTime", IniPath, "300"));
                //TestTimeout = int.Parse(FuncFile.ReadIniFile(Section, "TestTimeout", IniPath, "300"));
                //OutputPassedOnly = FuncFile.ReadIniFile(Section, "OutputPassedOnly", IniPath, "False") == "True";

                //NGRetest = FuncFile.ReadIniFile(Section, "NGRetest", IniPath, "False") == "True";
                //NGReEnterCount = int.Parse(FuncFile.ReadIniFile(Section, "NGReEnterCount", IniPath, "3"));

                FuncInline.ArrayImage = FuncFile.ReadIniFile(Section, "ArrayImage", IniPath, "");
                FuncInline.PinUseFront = FuncFile.ReadIniFile(Section, "PinUseFront", IniPath, "False") == "True";
                FuncInline.PinUseRear = FuncFile.ReadIniFile(Section, "PinUseRear", IniPath, "False") == "True";
                FuncInline.PCBWidth = double.Parse(FuncFile.ReadIniFile(Section, "PCBWidth", IniPath, "70"));
                FuncInline.PCBLength = double.Parse(FuncFile.ReadIniFile(Section, "PCBLength", IniPath, "100"));
                FuncInline.TestTimeout = int.Parse(FuncFile.ReadIniFile(Section, "TestTimeout", IniPath, "2000"));
                FuncInline.TestType = (FuncInline.enumTestType)int.Parse(FuncFile.ReadIniFile(Section, "TestType", IniPath, "2"));

                for (int i = 0; i < FuncInline.ArrayUse.Length; i++)
                {
                    FuncInline.ArrayUse[i] = FuncFile.ReadIniFile(Section, "ArrayUse_" + i, IniPath, "False") == "True";
                }
                for (int i = 0; i < FuncInline.XOut.Length; i++)
                {
                    FuncInline.XOut[i] = FuncFile.ReadIniFile(Section, "XOut_" + i, IniPath, "False") == "True";
                }
                FuncInline.XoutBySelection = FuncFile.ReadIniFile(Section, "XoutBySelection", IniPath, "False") == "True";
                FuncInline.XoutByVision = FuncFile.ReadIniFile(Section, "XoutByVision", IniPath, "False") == "True";
                FuncInline.UseBadMark = FuncFile.ReadIniFile(Section, "UseBadMark", IniPath, "False") == "True";
                //BadMarkWhenExist = FuncFile.ReadIniFile(Section, "BadMarkWhenExist", IniPath, "False") == "True";
                FuncInline.XoutToNG = FuncFile.ReadIniFile(Section, "XoutToNG", IniPath, "False") == "True";
                FuncInline.BadMarkToNG = FuncFile.ReadIniFile(Section, "BadMarkToNG", IniPath, "False") == "True";
                FuncInline.CarrierSeparation = FuncFile.ReadIniFile(Section, "CarrierSeparation", IniPath, "False") == "True";
                FuncInline.PCBInverting = FuncFile.ReadIniFile(Section, "PCBInverting", IniPath, "False") == "True";
                FuncInline.UseJigStop = FuncFile.ReadIniFile(Section, "UseJigStop", IniPath, "False") == "True";
                FuncInline.ScanAfterInvert = FuncFile.ReadIniFile(Section, "ScanAfterInvert", IniPath, "False") == "True";

                SaveDefaultModel(GlobalVar.ModelName);

                //string imagePath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath + "\\" + GlobalVar.ModelName + ".jpg";
                //FuncInline.ArrayBitmap = FuncScreen.LoadBitmap(imagePath);
                FuncInline.LoadArrayImage();

                for (int j = 0; j < FuncInline.PickupVaccum.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.PickupVaccum.GetLength(1); i++)
                    {
                        FuncInline.PickupVaccum[j, i] = FuncFile.ReadIniFile(Section, "PickupVaccum" + j.ToString() + i.ToString(), IniPath, "False") == "True";
                    }
                }

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SaveModelIni() // 모델의 모든 설정 쓰기
        {
            try
            {
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.ModelPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.ModelName + ".ini";

                string Section = GlobalVar.IniSection;

                FuncFile.WriteIniFile(Section, "ArrayCount", FuncInline.ArrayCount.ToString(), IniPath);

                //FuncFile.WriteIniFile(Section, "RobotTimeout", FuncInline.RobotTimeout.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "ConveyorTimeout", FuncInline.ConveyorTimeout.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "ScanTimeout", FuncInline.ScanTimeout.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "TestTimeout", FuncInline.TestTimeout.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "TestPassTime", FuncInline.TestPassTime.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "NGRetest", FuncInline.NGRetest.ToString(), IniPath);

                //FuncFile.WriteIniFile(Section, "OutputPassedOnly", FuncInline.OutputPassedOnly.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "NGRetest", FuncInline.NGRetest.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "NGReEnterCount", FuncInline.NGReEnterCount.ToString(), IniPath);

                FuncFile.WriteIniFile(Section, "ArrayImage", FuncInline.ArrayImage, IniPath);
                FuncFile.WriteIniFile(Section, "PinUseFront", FuncInline.PinUseFront.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PinUseRear", FuncInline.PinUseRear.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PCBWidth", FuncInline.PCBWidth.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PCBLength", FuncInline.PCBLength.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "TestTimeout", FuncInline.TestTimeout.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "TestType", ((int)FuncInline.TestType).ToString(), IniPath);

                for (int i = 0; i < FuncInline.ArrayUse.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "ArrayUse_" + i, FuncInline.ArrayUse[i].ToString(), IniPath);
                }
                for (int i = 0; i < FuncInline.XOut.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "XOut_" + i, FuncInline.XOut[i].ToString(), IniPath);
                }
                FuncFile.WriteIniFile(Section, "XoutBySelection", FuncInline.XoutBySelection.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "XoutByVision", FuncInline.XoutByVision.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "UseBadMark", FuncInline.UseBadMark.ToString(), IniPath);
                //FuncFile.WriteIniFile(Section, "BadMarkWhenExist", FuncInline.BadMarkWhenExist.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "XoutToNG", FuncInline.XoutToNG.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "BadMarkToNG", FuncInline.BadMarkToNG.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "CarrierSeparation", FuncInline.CarrierSeparation.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "PCBInverting", FuncInline.PCBInverting.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "UseJigStop", FuncInline.UseJigStop.ToString(), IniPath);
                FuncFile.WriteIniFile(Section, "ScanAfterInvert", FuncInline.ScanAfterInvert.ToString(), IniPath);

                for (int j = 0; j < FuncInline.PickupVaccum.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.PickupVaccum.GetLength(1); j++)
                    {
                        FuncFile.WriteIniFile(Section, "PickupVaccum" + j.ToString() + i.ToString(), FuncInline.PickupVaccum[j, i].ToString(), IniPath);
                    }
                }


                SaveDefaultModel(GlobalVar.ModelName);

                //string imagePath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.ArrayImagePath + "\\" + GlobalVar.ModelName + ".bmp";
                //FuncInline.ArrayBitmap = FuncScreen.LoadBitmap(imagePath);
                FuncInline.LoadArrayImage();
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SaveDefaultModel(string modelName)
        {
            try
            {
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\setting.ini";

                string Section = GlobalVar.IniSection;

                FuncFile.WriteIniFile(Section, "DefaultModel", modelName, IniPath);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void LoadPortIni() // 포트의 모든 설정 읽기
        {
            try
            {
                string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\port.ini";
                string Section = GlobalVar.IniSection;

                for (int i = 0; i < FuncInline.PortTest.Length; i++)
                {
                    FuncInline.PortTest[i] = int.Parse(FuncFile.ReadIniFile(Section, "PortTest" + i, IniPath, "COM1").Replace("COM", ""));
                }
                for (int i = 0; i < FuncInline.PortPMC.Length; i++)
                {
                    FuncInline.PortPMC[i] = int.Parse(FuncFile.ReadIniFile(Section, "PortPMC" + ((FuncInline.enumPMCMotion)i).ToString(), IniPath, "COM1").Replace("COM", ""));
                }
                FuncInline.PortTemperature = int.Parse(FuncFile.ReadIniFile(Section, "PortTemperature", IniPath, "COM1").Replace("COM", ""));

                for (int i = 0; i < FuncInline.BaudTest.Length; i++)
                {
                    FuncInline.BaudTest[i] = int.Parse(FuncFile.ReadIniFile(Section, "BaudTest" + i, IniPath, "9600"));
                }
                for (int i = 0; i < FuncInline.BaudPMC.Length; i++)
                {
                    FuncInline.BaudPMC[i] = int.Parse(FuncFile.ReadIniFile(Section, "BaudPMC" + ((FuncInline.enumPMCMotion)i).ToString(), IniPath, "9600"));
                }
                FuncInline.BaudTemperature = int.Parse(FuncFile.ReadIniFile(Section, "BaudTemperature", IniPath, "9600"));

                for (int i = 0; i < FuncInline.ParityTest.Length; i++)
                {
                    FuncInline.ParityTest[i] = Util.ParseParity(FuncFile.ReadIniFile(Section, "ParityTest" + i, IniPath, "None"));
                }
                for (int i = 0; i < FuncInline.ParityPMC.Length; i++)
                {
                    FuncInline.ParityPMC[i] = Util.ParseParity(FuncFile.ReadIniFile(Section, "ParityPMC" + ((FuncInline.enumPMCMotion)i).ToString(), IniPath, "None"));
                }
                FuncInline.ParityTemperature = Util.ParseParity(FuncFile.ReadIniFile(Section, "ParityTemperature", IniPath, "None"));

                for (int i = 0; i < FuncInline.StopBitsTest.Length; i++)
                {
                    FuncInline.StopBitsTest[i] = Util.ParseStopBits(FuncFile.ReadIniFile(Section, "StopBitsTest" + i, IniPath, "One"));
                }
                for (int i = 0; i < FuncInline.StopBitsPMC.Length; i++)
                {
                    FuncInline.StopBitsPMC[i] = Util.ParseStopBits(FuncFile.ReadIniFile(Section, "StopBitsPMC" + ((FuncInline.enumPMCMotion)i).ToString(), IniPath, "One"));
                }
                FuncInline.StopBitsTemperature = Util.ParseStopBits(FuncFile.ReadIniFile(Section, "StopBitsTemperature", IniPath, "One"));
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public static void SavePortIni() // 포트의 모든 설정 쓰기
        {
            try
            {
                string IniPath = GlobalVar.FaPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.SWName;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\" + GlobalVar.IniPath;
                if (!Directory.Exists(GlobalVar.IniPath))
                {
                    Directory.CreateDirectory(IniPath);
                }
                IniPath += "\\port.ini";

                string Section = GlobalVar.IniSection;

                for (int i = 0; i < FuncInline.PortTest.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "PortTest" + i, FuncInline.PortTest[i].ToString(), IniPath);
                }
                for (int i = 0; i < FuncInline.PortPMC.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "PortPMC" + ((FuncInline.enumPMCMotion)i).ToString(), FuncInline.PortPMC[i].ToString(), IniPath);
                }
                FuncFile.WriteIniFile(Section, "PortTemperature", FuncInline.PortTemperature.ToString(), IniPath);

                for (int i = 0; i < FuncInline.BaudTest.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "BaudTest" + i, FuncInline.BaudTest[i].ToString(), IniPath);
                }
                for (int i = 0; i < FuncInline.BaudPMC.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "BaudPMC" + ((FuncInline.enumPMCMotion)i).ToString(), FuncInline.BaudPMC[i].ToString(), IniPath);
                }
                FuncFile.WriteIniFile(Section, "BaudTemperature", FuncInline.BaudTemperature.ToString(), IniPath);

                for (int i = 0; i < FuncInline.ParityTest.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "ParityTest" + i, FuncInline.ParityTest[i].ToString(), IniPath);
                }
                for (int i = 0; i < FuncInline.ParityPMC.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "ParityPMC" + ((FuncInline.enumPMCMotion)i).ToString(), FuncInline.ParityPMC[i].ToString(), IniPath);
                }
                FuncFile.WriteIniFile(Section, "ParityTemperature", FuncInline.ParityTemperature.ToString(), IniPath);

                for (int i = 0; i < FuncInline.StopBitsTest.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "StopBitsTest" + i, FuncInline.StopBitsTest[i].ToString(), IniPath);
                }
                for (int i = 0; i < FuncInline.StopBitsPMC.Length; i++)
                {
                    FuncFile.WriteIniFile(Section, "StopBitsPMC" + ((FuncInline.enumPMCMotion)i).ToString(), FuncInline.StopBitsPMC[i].ToString(), IniPath);
                }
                FuncFile.WriteIniFile(Section, "StopBitsTemperature", FuncInline.StopBitsTemperature.ToString(), IniPath);
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }
        #endregion

        public static bool ServoErrored(int axis)
        {
            return ServoErrored((FuncInline.enumServoAxis)axis);
        }

        public static bool ServoErrored(FuncInline.enumServoAxis axis)
        {
            if (GlobalVar.Simulation)
            {
                return false;
            }
            return GlobalVar.AxisStatus[(int)axis].Disabled ||
                        GlobalVar.AxisStatus[(int)axis].Errored ||
                        GlobalVar.AxisStatus[(int)axis].ErrorID > 0 ||
                        //GlobalVar.AxisStatus[(int)axis].LimitSwitchNeg ||
                        //GlobalVar.AxisStatus[(int)axis].LimitSwitchPos ||
                        !GlobalVar.AxisStatus[(int)axis].PowerOn;
        }

        public static structPosition GetRobotPos()
        {
            structPosition pos = new structPosition();

            /*
            pos.x = FuncMotion.GetRealPosition((int)FuncInline.enumServoAxis.RobotX);
            pos.y = FuncMotion.GetRealPosition((int)FuncInline.enumServoAxis.RobotY);
            pos.z = FuncMotion.GetRealPosition((int)FuncInline.enumServoAxis.RobotZ2);
            //*/

            return pos;
        }



        public static void ResetServoError(FuncInline.enumServoAxis axis)
        {
            if (!GlobalVar.GlobalStop &&
                GlobalVar.AxisStatus[(int)axis].Disabled ||
                        GlobalVar.AxisStatus[(int)axis].Errored ||
                        GlobalVar.AxisStatus[(int)axis].ErrorID > 0)
            {
                FuncMotion.ServoReset((uint)axis);
            }
            if (!GlobalVar.GlobalStop &&
                !GlobalVar.AxisStatus[(int)axis].PowerOn)
            {
                FuncMotion.ServoOn((uint)axis, true);
            }
        }




        public static bool StopRobot()
        {
          
            FuncMotion.MoveStop((int)FuncInline.enumServoAxis.SV02_Lift1);
            FuncMotion.MoveStop((int)FuncInline.enumServoAxis.SV04_Lift2);
     
            return false;
        }

        public static bool CheckRobotStop()
        {
            /*
            return GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.RobotX].StandStill &&
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.RobotY].StandStill &&
                (GlobalVar.Axis_Sync || GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.RobotZ1].StandStill) &&
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.RobotZ2].StandStill;
                //*/
            return false;
        }

        /*
        public static bool MoveRobot(FuncInline.enumTeachingPos pos, double speed, enumRobotJig jig)
        {
            return MoveRobot(pos, speed, jig, false);
        }
        public static bool MoveRobot(FuncInline.enumTeachingPos pos, double speed, enumRobotJig jig, bool offset)
        {
            return MoveRobot(TeachingPos[(int)pos].x,
                                FuncInline.TeachingPos[(int)pos].y,
                                FuncInline.TeachingPos[(int)pos].z + (offset ? FuncInline.RobotOffsetZ : 0),
                                speed,
                                jig);
        }
        //*/

        public static bool CheckRobotMoveEnable(double xPos, double yPos, double zPos) // 현재 위치에서 이동 가능한 좌표인가?
        {
            /*
            double robotX = FuncMotion.GetRealPosition((int)FuncInline.enumServoAxis.RobotX);
            double robotY = FuncMotion.GetRealPosition((int)FuncInline.enumServoAxis.RobotX);
            double robotZ = FuncMotion.GetRealPosition((int)FuncInline.enumServoAxis.RobotZ2);
            //*/
            return true; // 걸리는 조건 없으면 이동 가능
        }

        public static bool CheckRobotMoveEnable(structPosition pos) // 현재 위치에서 이동 가능한 좌표인가?
        {
            return CheckRobotMoveEnable(pos.x, pos.y, pos.z);
        }

        public static bool PinClampMoveRobot(double xPos, double yPos, double zPos) // 로봇 회피구간 이동
        {
            if (CheckRobotMoveEnable(xPos, yPos, zPos))
            {
                return true;
            }

            return false;
        }

        public static bool PinClampMoveRobot(structPosition pos) // 로봇 회피구간 이동
        {
            return PinClampMoveRobot(pos.x, pos.y, pos.z);
        }

        #region 앰플포장 전역변수들.. 소장님이 여기에 만들어주었음

        public static SerialLVDT SerialLVDT1 = new SerialLVDT("UNIT1");   //하부시
        public static SerialLVDT SerialLVDT2 = new SerialLVDT("UNIT2");   //상부시
        public static SerialLVDT SerialLVDT3 = new SerialLVDT("UNIT3");   //갭높이

        public static string LVDTSensor1_Left_PV = "";
        public static string LVDTSensor1_Right_PV = "";

        public static string LVDTSensor2_Left_PV = "";
        public static string LVDTSensor2_Right_PV = "";

        public static string LVDTSensor3_Left_PV = "";
        public static string LVDTSensor3_Right_PV = "";

        //사용안하는듯
        public static bool LVDTSensor1_Connect_Fail;
        public static bool LVDTSensor2_Connect_Fail;
        public static bool LVDTSensor3_Connect_Fail;


        public static Domino_Inkjet Domino_InkjetP = new Domino_Inkjet();

        public static PanasonicVision sizeVision = new PanasonicVision("192.168.1.6", 8604, 8601); // 혼입감지(Size검사) 비전
        public static PanasonicVision printVision = new PanasonicVision("192.168.1.5", 8604, 8601); // 투입 및 인쇄 검사 비전

        //사용안하는듯
        public static string sizeVision_LeftW = "";
        public static string sizeVision_LeftH = "";
        public static string sizeVision_RightW = "";
        public static string sizeVision_RightLeftH = "";

        #endregion


    }
}
