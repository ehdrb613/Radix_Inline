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
using System.IO.Ports;
using System.IO;

/***********************************************************************************************

Honeywell HF800HD QR리더 이더넷 통신 프로그램 - JHRYU 2024/01/29

이더넷 IP/PORT : DataMax 설정 프로그램에서 변경가능 Default : 192.168.1.110:55256

************************************************************************************************/
namespace Radix
{
    public class HoneywellQR_Ethernet
    {
        private Thread _thread = null;
        private bool _bThreadStateRun = true;

        private TcpClient socketCommand = new TcpClient();
        private NetworkStream streamCommand = default(NetworkStream);

        private string ip = "";     //"192.168.100.110";  // 생성자에서 재설정 할것 ( 110,111,112 사용예정 )
        //private string ip = "127.0.0.1";
        private int port = 55256;               // Heneywell Scanner Ethernet Default Port

        public string[] Results = new string[] { "", "", "", "", "", "", "", "", "", "" };      // LOT 정보 파싱된 결과
        public string ResultOrg = "";                                                           // LOT 정보 파싱전 원본 결과

        //object lock_obj = new object();

        public ulong result_recvtime = 0;     // 최근 result 수신시간
        public bool connected = false;      // 접속 되어 있을때 true
        public bool IsRecivedQR = false;    // QR 수신 여부
        public bool IsReadWaiting = false;     // QR 수신 대기 중인가

        private bool IsRequestQR = false;   // 스케너 시작
        private bool IsRequestOff = false;   // 스케너 끄기

        int recvTimeout = 1500;
        int sendTimeout = 500;

        byte[] cmd_trg = { 0x16, (byte)'T', 0x0D };
        byte[] cmd_stop = { 0x16, (byte)'U', 0x0D };

        public HoneywellQR_Ethernet(string ip)
        {
            this.ip = ip;
            //this.port = port;
        }


        public int GetPort()
        {
            return port;
        }

        private void debug(string str)
        {
            //Util.Debug(str);


            Func.StatusPrint(str);
        }
  
  
  

        public void SetTimeOut( int recvTime, int sendTime)
        {
            this.recvTimeout = recvTime;
            this.sendTimeout = sendTime;

            if (socketCommand.Connected)
            {
                socketCommand.ReceiveTimeout = this.recvTimeout;
                socketCommand.SendTimeout = this.sendTimeout;
            }
        }

        //public bool SendCmd(string cmd)
        //{
        //    if (!connected)
        //    {
        //        return false;
        //    }

        //    try
        //    {
        //        byte[] SendBytes = Util.StringToByteArray(cmd);
        //        byte[] RecvBytes = new Byte[1024];

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        Console.WriteLine(ex.StackTrace);
        //    }
        //    return false;
        //}
        
        
        public void Connect()
        {
            try
            {
                //if (GlobalVar.Simulation)
                //{
                //    ip = "127.0.0.1";
                //}

                // 이전 접속 해제
                socketCommand.Close();
                socketCommand = new TcpClient();

                socketCommand.ReceiveTimeout = this.recvTimeout;
                socketCommand.SendTimeout = this.sendTimeout;


                // 접속 시도
                socketCommand.Connect(ip, port);

                connected = socketCommand.Connected;
                if (!connected)
                {
                    debug("Honeywell HF700 not connected: "+ip);
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


        private bool SendCmd(byte[] cmd)
        {
            // reset timeout time
            SetTimeOut(recvTimeout, sendTimeout);

            if (!connected)
            {
                return false;
            }

            try
            {
                byte[] SendBytes = cmd;
                //byte[] RecvBytes = new Byte[1024];

                streamCommand.Write(SendBytes, 0, SendBytes.Length);
                streamCommand.Flush();
                return true;
            }
            catch (Exception ex)
            {
                connected = false;
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }




        bool Init() // 접속 및 초기 동작부터
        {
            ulong startTime = GlobalVar.TickCount64;

            while (!connected)
            {
                if (GlobalVar.TickCount64 - startTime > 5000)
                {
                    // connect failed and time out
                    debug("HoneywellQR EtherNet Init() failed!");
                    return false;
                }
                Thread.Sleep(100);
                Connect();

                if ( connected )
                {
                    // TRIGGER OFF
                    debug("Scanner H700 connected : " + ip);
                    SendCmd(cmd_stop);
                    Thread.Sleep(100);
                }
            }
            return true;
        }
        

        public bool InitThread()
        {
            _thread = new Thread(new ThreadStart(ThreadEntry));

            _thread.IsBackground = true;
            connected = false;
            //_bIsOpenSerial = false;
            
            _thread.Start();
            return true;
        }


        public bool EndThread()
        {
            connected = false;

            streamCommand.Close();
            if (_thread != null)
            {
                _bThreadStateRun = false;
                if (_thread.Join(100) == false) _thread.Abort();
            }

            return true;
        }


        void ThreadEntry()
        {
            const int WRITE_SIZE = 3;
            const int READ_SIZE = 512;
            byte[] bWriteBuff = new byte[READ_SIZE];
            byte[] bReadBuff = new byte[READ_SIZE];
            
            while (_bThreadStateRun)
            {

                try
                {
                    if (!Init())
                    {
                        debug("QR Reader Connect Failed!");
                        continue;   // retry
                    }
                    

                    // 통신 시작
                    if ( IsRequestQR )
                    {
                        IsRequestQR = false;
                        IsRecivedQR = false;
                        Results = new string[] { "", "", "", "", "", "", "", "", "", "" };
                        ResultOrg = "";

                        if ( SendCmd(cmd_trg) )
                        {
                            IsReadWaiting = true;
                            // clear read buffer
                            Array.Clear(bReadBuff, 0, bReadBuff.Length);
                            // try read

                            int rbytes = streamCommand.Read(bReadBuff, 0, READ_SIZE - 1);
                            if (rbytes > 0)
                            {
                                // 수신됨
                                //string strRecv = BitConverter.ToString(bReadBuff);
                                string[] splitStr1 = { "\r", "\n", "\0" };
                                string[] splitStr2 = { "(",")","\r", "\n", "\0" };
                                string recv = Encoding.ASCII.GetString(bReadBuff);

                                recv = recv.Split(splitStr1, StringSplitOptions.RemoveEmptyEntries)[0]; // "\r\n" 이후 문자는 삭제
                                recv = recv.Trim();
                                ResultOrg = String.Copy(recv);

                                Results = recv.Split(splitStr2, StringSplitOptions.RemoveEmptyEntries);// "(",")" 문자로 항목 분리하여 저장 
                                result_recvtime = GlobalVar.TickCount64;
                                IsRecivedQR = true;
                            }
                            else
                            {
                                // 수신 되지 않음
                                // 스케너 스켄 종료
                                IsRequestOff = true;
                            }
                            //// 수신 되거나 말거나 대기 종료
                            //eventWaitQR.Set();
                            IsReadWaiting = false;
                        }
                    }

                    if (IsRequestOff)
                    {
                        IsRequestOff = false;
                        if (SendCmd(cmd_stop))
                        {
                            IsReadWaiting = false;
                        }
                    }

                    // Final Sleep
                    Thread.Sleep(GlobalVar.ThreadSleep);
                }
                catch (IOException ex)
                {
                    var socketExept = ex.InnerException as SocketException;
                    if (socketExept != null && socketExept.ErrorCode == 10060)
                    {
                        // 수신 데이타 없음
                        //
                        IsReadWaiting = false;
                        IsRequestOff = true;
                    }
                    else
                    {
                        connected = false;
                        FuncLog.WriteLog("HoneywellQR Socket Exception catched");
                        Thread.Sleep(1000);
                    }
                }
                catch (ThreadAbortException)
                {
                    FuncLog.WriteLog("HoneywellQR Thread End Exception catched");
                }
                catch( Exception ex )
                {
                    //debug("HoneywellQR Exception");
                    FuncLog.WriteLog("HoneywellQR Exception");
                }
            }
            debug("HoneywellQR Thread End");
        }


        // timeoutMS 동안 통신 결과를 기다린다. 촬영직후 사용
        // 시그널을 받았으면 True를 리턴 타임아웃이면 False 리턴
        public bool WaitForJobResult()
        {
            return AsyncDelay(2000);
        }


        /**
        * @brief UI를 멈추지 않고 일정시간 DELAY한다.
        *        
        */
        private bool AsyncDelay(int MS)
        {
            //eventWaitQR.Reset();
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                if (!IsReadWaiting)
                {
                    if (IsRecivedQR) return true;
                    return false;
                }
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
                Thread.Sleep(10);
            }

            return false;
        }


        // isOn : true 일때 바코드 스켄 시작, false 일때 바코드 스켄 강제 종료
        public void RequestQR(bool isOn)
        {
            // 이미수신된 데이터가 있다면 버린다.
            if (connected)
            {
                var buffer = new byte[1024];
                while (streamCommand.DataAvailable)
                {
                    streamCommand.Read(buffer, 0, buffer.Length);
                }
            }

            //
            if (isOn)
            {
                IsReadWaiting = true;
                IsRequestOff = false;
                IsRecivedQR = false;
                IsRequestQR = true;
            }
            else
            {
                IsRequestQR = false;
                IsRecivedQR = false;
                IsRequestOff = true;
            }
        }


        public void TrggerOff()
        {
            // 스커너가 시간을 초과하면 스케너를 끌때 사용
            RequestQR(false);
        }
        
        public void SocketClose()
        {
            if (streamCommand != null)
            {
                streamCommand.Close();
            }
            connected = false;
        }
    }
}
