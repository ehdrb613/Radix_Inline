using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;

/*****************************************

Domino Laser Mark 통신 프로그램 - JHRYU 2023/09/11

주 : 레이저 마킹전 도미노 QuickStep 2 프로그램에서 
     MsgStore1/QR1 인쇄 프로젝트와 MsgStore1/QR2 인쇄 프로젝트가 있어야 하고 
     QR1 은 일반 종이박스에 대응 QR2는 섬유질 박스에 대응 하는 교체용으로 설정해 놓을것

IP : 192.168.58.2
PORT : 20000

*****************************************/
namespace Radix
{
    public class DominoMark
    {
        private Thread _thread = null;
        private bool _bThreadStateRun = true;

        private static TcpClient socketCommand = new TcpClient();
        private static NetworkStream streamCommand = default(NetworkStream);

        public bool connected = false;  // 소켓이 접속 되어 있을때 true
        bool IsReceivedMarkOK = false;  // 마킹 성공 수신여부

        private string ip = "192.168.100.130"; //"192.168.58.2"; // 192.168.100.130 으로 바꾸는걸 검토할것
        //private string ip = "127.0.0.1";
        private int port = 20000;

        public string Result = null;
        public int LaserState = 0;      // 1: READY 2: MARKING 3: MARKING DONE 4: NOT READY 26: PC COMMAND RECEIVED
        public ulong LastStatusTime = GlobalVar.TickCount64;      // 마지막 수신 시간 저장용

        object lock_obj = new object();



        string cmd_setting = "RESETSYSTEM\r\nMARK START\r\nSETMSG 1 1\r\nSETMSG 2 0\r\nSETMSG 3 1\r\nSETMSG 4 1\r\nSETMSG 26 1";

        string cmd_load1 = "LOADPROJECT store:MsgStore1/QR1\r\nMARK START\r\n";
        string cmd_load2 = "LOADPROJECT store:MsgStore1/QR2\r\nMARK START\r\n";


        public DominoMark()
        {

        }

        private void debug(string str)
        {
            //Util.Debug(str);
            //FuncBoxPacking.LogView(str);
        }

        public void Connect()
        {
            try
            {
                if ( GlobalVar.Simulation ) 
                {
                    ip = "127.0.0.1";
                }

                // 이전 접속 해제
                socketCommand.Close();
                socketCommand = new TcpClient();

                socketCommand.ReceiveTimeout = 0;
                socketCommand.SendTimeout = 5000;


                // 접속 시도
                socketCommand.Connect(ip, port);

                connected = socketCommand.Connected;
                if (!connected)
                {
                    debug("Domino Mark not connected");
                    return;
                }
                streamCommand = socketCommand.GetStream();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("1서버가 실행중이 아닙니다.", "연결실패!");
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                return;
            }
            //GlobalVar.LaserStatus = enumLaserStatus.Idle;
        }

        public bool SendCmd(string cmd)
        {
            if (!connected)
            {
                return false;
            }

            lock(lock_obj)
            {
                try
                {
                    Result = "";

                    if (cmd.Length > 2)
                    {
                        // 송신시 CRLF 가 없으면 추가 해준다.
                        if (cmd.Substring(cmd.Length - 2) != "\r\n")
                        {
                            cmd += "\r\n";
                        }
                    }

                    byte[] SendBytes = Util.StringToByteArray(cmd);
                    byte[] RecvBytes = new Byte[1024];

                    streamCommand.Write(SendBytes, 0, SendBytes.Length);
                    streamCommand.Flush();
                    //// recv return msg (버림)
                    //int nRecvSize = streamCommand.Read(RecvBytes, 0, RecvBytes.Length);
                    //Array.Resize(ref RecvBytes, nRecvSize);

                    //if (nRecvSize > 0)
                    //{
                    //    string str = Util.ByteArrayToString(RecvBytes);
                    //    debug("Recv : " + str);
                    //}

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return false;
        }


        bool Init() // 접속 및 초기 동작부터 펌핑까지
        {
            ulong startTime = GlobalVar.TickCount64;
            while (!connected)
            {
                LaserState = 0;
                if (GlobalVar.TickCount64 - startTime > 10000)
                {
                    // connect failed and time out 10 sec
                    debug("Laser Mark Init() Connect failed!");
                    return false;
                }
                Thread.Sleep(100);
                Connect();

                if ( connected )
                {
                    FuncInline.LogView("Laser Mark connected");
                    LastStatusTime = GlobalVar.TickCount64;

                    // 접속 직후 명령 송신시 응답 누락 되는 현상 방지용 (시뮬레이터는 때때로 누락됨)
                    Thread.Sleep(200);
                    // SELECT MSG MODEL
                    if (GlobalVar.G_LaserMarkModel == 1)
                    {
                        SendCmd(cmd_load1);
                        FuncInline.LogView("Set Laser Mark Model 1");
                    }
                    else
                    {
                        SendCmd(cmd_load2);
                        FuncInline.LogView("Set Laser Mark Model 2");
                    }
                    Thread.Sleep(200);
                    // 기본 설정값 송신
                    SendCmd(cmd_setting);
                }
            }
            return true;
        }
        

        public bool InitThread()
        {
            _thread = new Thread(new ThreadStart(ThreadEntry));

            _thread.IsBackground = true;


            _thread.Start();
            return true;
        }


        public bool EndThread()
        {
            if (_thread != null)
            {
                _bThreadStateRun = false;
                if (_thread.Join(100) == false) _thread.Abort();
            }

            return true;
        }


        void ThreadEntry()
        {

            while (_bThreadStateRun)
            {
                try
                {
                    if (!Init())
                    {
                        debug("Laser Mark Connect Failed!");
                        continue;   // retry
                    }

                    int BUFFERSIZE = socketCommand.ReceiveBufferSize;
                    byte[] buffer = new byte[BUFFERSIZE];
                    int rbytes = streamCommand.Read(buffer, 0, buffer.Length);
                    if ( rbytes <= 0 )
                    {
                        connected = false;
                        debug("Laser Mark trying to reconnect...");
                        Thread.Sleep(2000);
                        continue;
                    }

                    LastStatusTime = GlobalVar.TickCount64;
                    string str = Encoding.ASCII.GetString(buffer, 0, rbytes);

                    if (buffer[rbytes - 1] == 10)   // 수신 문자열 끝이 0x0A로 끝나면
                    {
                        //str = str.Replace("\n", "").Replace("\r", "").Replace("OK", "");
                        Result = str.Replace("\r\n", "");

                        if (str.IndexOf("MSG 2") > -1 && str.Length == 5 )
                        {
                            LaserState = 2;
                            debug("RECV: " + str);
                        }
                        if (str.IndexOf("MSG 4") > -1)
                        {
                            LaserState = 4;
                            debug("RECV: " + str);
                        }
                        if (str.IndexOf("MSG 26") > -1)
                        {
                            LaserState = 26;
                            debug("RECV: " + str);
                        }
                        if (str.IndexOf("MSG 1") > -1)
                        {
                            LaserState = 1;
                            debug("RECV: " + str);
                        }
                        if (str.IndexOf("MSG 3") > -1)
                        {
                            LaserState = 3;
                            debug("RECV: " + str);
                            IsReceivedMarkOK = true;    // 마킹 성공 수신
                        }
                        //debug("RECV: " + str);
                    }


                    Thread.Sleep(100);
                }
                catch(IOException)
                {
                    // 아마도 소켓 오류
                    FuncLog.WriteLog("Socket exception catched");
                    Thread.Sleep(1000);
                    connected = false;
                }
                catch (ThreadAbortException)
                {
                    FuncLog.WriteLog("Domino Thread End Exception catched");
                }
                catch( Exception ex )
                {
                    FuncLog.WriteLog("Domino Exception : " + ex.ToString());
                    Thread.Sleep(1000);
                    connected = false;
                    continue;
                }
            }

            FuncLog.WriteLog("Domino socket Thread End");
        }


        // timeoutMS 동안 통신 결과를 기다린다. 촬영직후 사용
        // 시그널을 받았으면 True를 리턴 타임아웃이면 False 리턴
        public bool WaitForJobResult(int timeoutMS)
        {
            return AsyncDelay(timeoutMS);
        }

        private bool AsyncDelay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                if (IsReceivedMarkOK) return true;
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
                Thread.Sleep(10);
            }

            return false;
        }


        public bool ChangeMsgModel(int num)
        {
            SendCmd("BEGINTRANS");
            if (num == 1)
            {
                SendCmd(cmd_load1);
            }
            else
            {
                SendCmd(cmd_load2);
            }
            SendCmd("MARK START");
            SendCmd("EXECTRANS");
            WaitForJobResult(1000);
            if (Result == "MSG 26")
            {
                FuncInline.LogView("LABEL PRINT MODEL CHANGE OK");
                return true;
            }
            return false;
        }


        public bool QRPrint(string msg)
        {
            LaserState = 4;
            IsReceivedMarkOK = false;
            if (connected)
            {

                var qr = msg;

                SendCmd("BEGINTRANS");
                SendCmd("MARK START");
                SendCmd($"SETTEXT \"QR1\" \"{qr}\"");
                SendCmd("EXECTRANS");
                WaitForJobResult(500);
                SendCmd("TRIGGER");
                WaitForJobResult(1500);

                if (Result == "MSG 3")
                {
                    //FuncBoxPacking.LogView("DOMINO MARK OK");
                    return true;
                }

                //if (Result == "MSG 26")
                //{
                //    SendCmd("TRIGGER");

                //    WaitForJobResult(2000);
                //    if (Result == "MSG 3")
                //    {
                //        //FuncBoxPacking.LogView("DOMINO MARK OK");
                //        return true;
                //    }
                //}
                FuncInline.LogView("DOMINO LASER MARK NG");
            }
            return false;
        }


        public void SocketClose()
        {
            if (streamCommand != null)
            {
                streamCommand.Close();
            }
            connected = false;
            LaserState = 0;
        }

        // 상태진단 결과 체크 
        // 10초이내 주기적으로 한번씩 호출해주면 접속 상태를 체크할수 있다.)
        public int UpdateHostStatus()
        {
            // 나중을 위해
            // 레이저 마킹기는 기능 구현이 애매함..
            //
            // 왜냐면 명령을 대화식으로 보내고 특정 응답을 기다리는 구조에서 통신체크 때문에 특정 명령문자가 보내지는 상황에서는
            // 원하지 않는 응답이 섞여오게 되고 이것은 수신처리시 잘못된 응답으로 판단될 요지가 있기 때문에
            // 확실히 레이저 마킹기능을 사용하지 않는 시점에서 통신 테스트를 보내는 방식이 필요하다.

            if (!connected) return -1;

            return 0;
        }

    }
}
