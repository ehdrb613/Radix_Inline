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

/***********************************************************************************************

Honeywell 3310g QR리더 시리얼 통신 프로그램 - JHRYU 2023/09/18

PORT : PC 설정에 따름 (장치관리 -> 포트 -> 33xx Area-Imaging Scanner -> COM포트설정 할것)
시리얼 설정 : USB 장치를 꼽고 드라이버를 설치한후 시리얼포트를 활성화하는 QR 코드를 읽으면 
             가상 시리얼 통신용 포트가 생성됨 시리얼 설정은 115200 bps N-8-1 로 할것

************************************************************************************************/
namespace Radix
{
    public class HoneywellQR
    {
        private Thread _thread = null;
        private bool _bThreadStateRun = true;
        SimpleSerial _serial = new SimpleSerial();
        //bool _bIsOpenSerial = false;

        //private AutoResetEvent eventWaitQR = new AutoResetEvent(false);

        public string[] Results = new string[] { "", "", "", "", "", "", "", "", "", "" };      // LOT 정보 파싱된 결과
        public string ResultOrg = "";                                                           // LOT 정보 파싱전 원본 결과

        public ulong result_recvtime = 0;     // 최근 result 수신시간
        public bool connected = false;      // 접속 되어 있을때 true
        public bool IsRecivedQR = false;    // QR 수신 여부
        public bool IsReadWaiting = false;     // QR 수신 대기 중인가

        private bool IsRequestQR = false;   // 스케너 시작
        private bool IsRequestOff = false;   // 스케너 끄기
        private int Port = 0;
        private int ComSpeed = 115200;

        int recvTimeout = 5000;
        int sendTimeout = 50;

        public HoneywellQR( int port )
        {
            Port = port;
        }

        public int GetPort()
        {
            return Port;
        }

        private void debug(string str)
        {
            //Util.Debug(str);


            Func.StatusPrint(str);
        }


        bool InitPort()
        {
            _serial.ClosePort();

            //_bIsOpenSerial = false;
            connected = false;

            if (!_bThreadStateRun) return false;

            if (_serial.OpenPort("COM"+Port, ComSpeed, Parity.None, 8, StopBits.One))
            {
                SetTimeOut(recvTimeout, sendTimeout);

                //_bIsOpenSerial = true;
                connected = true;
            }

            if( Port == 0 )
            {
                _bThreadStateRun = false;
            }

            return true;
        }

        public void SetTimeOut( int recvTime, int sendTime)
        {
            try
            {
                // set serial timeout ms
                if (_serial != null)
                {
                    this.recvTimeout = recvTime;
                    this.sendTimeout = sendTime;
                    _serial.SetCommunicationTimeout(recvTimeout, sendTimeout);
                }
            }
            catch (Exception)
            { }
        }

        public bool SendCmd(string cmd)
        {
            if (!connected)
            {
                return false;
            }

            try
            {
                byte[] SendBytes = Util.StringToByteArray(cmd);
                byte[] RecvBytes = new Byte[1024];


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return false;
        }

        public bool SendCmd(byte [] cmd)
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
                byte[] RecvBytes = new Byte[1024];
                _serial.WriteBytes(cmd, cmd.Length);
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
                if (Port==0)
                {
                    EndThread();
                    return false;
                }

                if (GlobalVar.TickCount64 - startTime > 10000)
                {
                    // connect failed and time out
                    debug($"HoneywellQR Init() failed! Port({Port})");
                    return false;
                }
                Thread.Sleep(100);
                InitPort();

                if ( connected )
                {
                    // TRIGGER OFF
                    byte[] cmd_stop = { 0x16, (byte)'U', 0x0D };
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
            _serial.ClosePort();
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
                //if (Port != 1)
                //{
                //    Thread.Sleep(100);
                //    continue;
                //}

                try
                {
                    if (!Init())
                    {
                        debug("QR Reader Connect Failed!");
                        continue;   // retry
                    }
                    connected = _serial.IsOpen();

                    // 통신 시작
                    if ( IsRequestQR )
                    {
                        IsRequestQR = false;
                        IsRecivedQR = false;
                        byte[] cmd_trg = { 0x16, (byte)'T', 0x0D };
                        Results = new string[] { "", "", "", "", "", "", "", "", "", "" };
                        ResultOrg = "";

                        if ( SendCmd(cmd_trg) )
                        {
                            IsReadWaiting = true;
                            // clear read buffer
                            Array.Clear(bReadBuff, 0, bReadBuff.Length);
                            // try read
                            if (_serial.ReadBytes(bReadBuff, READ_SIZE-1))
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
                        byte[] cmd_stop = { 0x16, (byte)'U', 0x0D };

                        if (SendCmd(cmd_stop))
                        {
                            IsReadWaiting = false;
                        }
                    }

                    // Final Sleep
                    Thread.Sleep(GlobalVar.ThreadSleep);
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
                    if ( IsRecivedQR ) return true;
                    return false;
                }
                //System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
                Thread.Sleep(10);
            }

            return false;
        }


        // isOn : true 일때 바코드 스켄 시작, false 일때 바코드 스켄 강제 종료
        public void RequestQR(bool isOn)
        {
            _serial.DisCardInBuffer();

            SetTimeOut(1500, 500);   // 스케너 타임아웃 수신 1.5초 송신 0.5초

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
    }
}
