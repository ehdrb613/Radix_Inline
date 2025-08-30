using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Radix
{
    /**
     * @brief 로이체 ACR 스마트 비전 처리 함수 선언
     *      레이저 마킹기 사용
     *      Base Framework 작업하면서 대다수 주석처리된 듯함
     */
    public static class FuncLeuzeVision
    {
        /*
         * FuncLeuzeVision.cs : 로이체 ACR 스마트 비전 처리 함수 선언
         */

        #region 로컬 변수
        /**
         * @brief 트리거 통신 포트 연결 여부
         */
        private static bool connectLeuzeTrigger = false;
        /**
         * @brief 결과 데이터 통신 포트 연결 여부
         */
        private static bool connectLeuzeData = false;
        #endregion

        /**
         * @brief  로컬 디버그
         * @param str 디버그 처리할 문자열
         */
        private static void debug(string str) // 로컬 디버그
        {
            Util.Debug("FuncLeuzeVision : " + str);
        }

        #region 로이체 비전 관련

        /**
         * @brief 비전 통신 연결
         *      통신시 연결 동시 체크하므로 별도 연결은 하지 않는다.
         */
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
                        //FuncWin.TopMessageBox("접속 실 패");
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
                    //FuncWin.TopMessageBox("1서버가 실행중이 아닙니다.", "연결실패!");
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
                        //FuncWin.TopMessageBox("접속 실 패");
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
                    //FuncWin.TopMessageBox("2서버가 실행중이 아닙니다.", "연결실패!");
                    return false;
                }
            }
            */
            return true;

        }

        /**
         * @brief 비전 체크 트리거 전송
         *      함수 통일하면서 사용하지 않는다.
         */
        public static void SendVisionTrigger() // 비전 체크 트리거 전송
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes("TRG\r\n");
                //GlobalVar.streamLeuzeTrigger.Write(buffer, 0, buffer.Length);
                //GlobalVar.streamLeuzeTrigger.Flush();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        /**
         * @brief Job 변경 지령 전송
         *      함수 통일하면서 사용하지 않는다.
         */
        public static void SendVisionJob(enumVisionCmd cmd) // Job 변경 지령 전송
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes("CJB00" + ((int)cmd).ToString() + "\r\n");
                //GlobalVar.streamLeuzeTrigger.Write(buffer, 0, buffer.Length);
                //GlobalVar.streamLeuzeTrigger.Flush();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        /**
         * @brief 직교로봇을 이동하면서 비전 잡을 전송하고 데이터를 수신한다.
         * @param speed 직교로봇 이동 속도
         * @param pos 직교로봇 이동할 좌표
         * @param cmd 비전 체크할 job 종류
         * @param text 비교할 텍스트
         * @return string 검색된 문자열
         */
        public static string GetVisionMoveResult(double speed, structPosition pos, enumVisionCmd cmd, string text) // 모션 이동하면서 비전 체크
        {
            SendVisionJob(cmd);

            bool turn = false;

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

        /**
         * @brief 비전 잡을 전송하고 데이터를 수신한다.
         * @param cmd 비전 체크할 job 종류
         * @return string 검색된 문자열
         */
        public static string GetVisionResult(enumVisionCmd cmd) // 비전 체크 실행
        {
            SendVisionJob(cmd);
            string message = "";
            bool visionChecked = false;

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
                    debug("vision msg : " + message);
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
                            debug(ex.ToString());
                            debug(ex.StackTrace);
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

        /**
         * @brief 비전 체크값 수신
         * @return bool 수신 여부
         */
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
                        //FuncWin.TopMessageBox("접속 실 패");
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
                    //FuncWin.TopMessageBox("2서버가 실행중이 아닙니다.", "연결실패!");
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
                debug(message); //F,548248,298357,
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
    }
}
