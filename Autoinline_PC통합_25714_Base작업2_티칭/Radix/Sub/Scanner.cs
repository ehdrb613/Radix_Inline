using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.IO; // MemoryStream
using System.Threading;

namespace Radix
{
    public class Scanner
    {
        /*
         * Scanner.cs : 핸디스캐너 제어
         */

        private SerialPort InConveyorScanner = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        

        public bool AutoLoader = true; // 로더 자동동작중
        public bool AutoUnloader = true; // 언로더 자동동작중

        public double WidthLoader = 1000; // 로더에 실제 적용 완료확인 된 폭
        public double WidthUnloader = 1000; // 언로더에 실제 적용 완료확인된 폭

        public bool WidthValueLoader = false; // 로더측에서 폭값 리턴 받음
        public bool WidthValueUnloader = false; // 언로더측에서 폭값 리턴 받음
        public bool WidthStartLoader = false; // 로더측에서 시작 신호 받음
        public bool WidthStartUnloader = false; // 언로더측에서 시작 신호 받음

        public bool WidthStartedLoader = false; // 로더측으로 폭조절 시작
        public bool WidthStartedUnloader = false; // 언로더측으로 폭조절 시작

        public int ErrorLoader = 0;
        public int ErrorUnloader = 0;

        string BufferLoader = "";        

        private void debug(string str)
        {
            //Util.Debug(str);
        }
        public Scanner()
        {
            InConveyorScanner.PortName = "COM1";
            InConveyorScanner.BaudRate = 115200;
            InConveyorScanner.DataBits = 8;
            InConveyorScanner.Parity = Parity.None;
            InConveyorScanner.Handshake = Handshake.None;
            InConveyorScanner.StopBits = StopBits.One;
            InConveyorScanner.ReadTimeout = 1000;
            InConveyorScanner.WriteTimeout = 1000;
            InConveyorScanner.DataReceived += InConveyorScanner_DataReceived;
       
        }

        public bool InConveyorScanner_Connect()
        {
            if (!InConveyorScanner.IsOpen)
            {
                try
                {
                    debug("connect");
                    // 연결
                    InConveyorScanner.Open();
                    return InConveyorScanner.IsOpen;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return false;
                }
            }
            else
            {
                //debug("loader connected");
            }
            return true;
        }

  

        public void Disconnect()
        {
            debug("disconnect");
            InConveyorScanner.Close();       
        }

        public void InConveyorScanner_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            GlobalVar.Load_Data_Received = true;

            String msg = InConveyorScanner.ReadExisting();
            Console.WriteLine(msg);
            if (msg.Contains(Convert.ToChar(0x02))) // StX
            {
                BufferLoader = msg;
            }
            else
            {
                BufferLoader += msg;
            }
            //if (!msg.Contains(Convert.ToChar(0x03))) // ETX
            //{
            //    return;
            //}

            try
            {
                msg = BufferLoader.Substring(0, 13);
            }
            catch
            {
                return;
            }

            debug("InConveyorScanner receive : " + msg);

            GlobalVar.Load_Scanner = msg;
            msg = "";
            BufferLoader = "";

            //msg 데이터 가공

            //if (msg != null && msg.Length > 0)
            //{
            //    if (msg.Length >= 8 &&
            //        msg.Substring(0, 5) == "ERROR")
            //    {
            //        debug("loader error : ");
            //        try
            //        {
            //            ErrorLoader = int.Parse(msg.Substring(7));
            //        }
            //        catch (Exception ex)
            //        {
            //            debug("loader error : " + ErrorLoader);
            //        }
            //    }
            //    if (msg.Contains("MANUAL"))
            //    {
            //        debug("loader manual");
            //        AutoLoader = false;
            //    }
            //    if (msg.Contains("AUTO"))
            //    {
            //        debug("loader auto");
            //        AutoLoader = true;
            //    }

            //    if (msg.Contains("WIDTH_START"))
            //    {
            //        debug("loader width started");
            //        WidthStartLoader = true;
            //    }
            //    else if (msg.Contains("WIDTH_END"))
            //    {
            //        debug("loader width end");
            //        // 세번째 칼럼이 숫자면 WIDTH 값 저장
            //        try
            //        {
            //            double width = int.Parse(msg.Substring(10)) / 100;
            //            if (width > 0)
            //            {
            //                WidthLoader = width;
            //            }
            //            debug("loader width end");
            //        }
            //        catch (Exception ex)
            //        {
            //            debug("width : " + msg.Substring(10));
            //            // 타입이 숫자형이 아니면 무시
            //        }
            //        WidthStartLoader = false;
            //        WidthStartedLoader = false;
            //    }
            //    else if (msg.Contains("WIDTH_ABORT"))
            //    {
            //        debug("loader width abort");
            //        WidthStartedLoader = false;
            //        WidthStartLoader = false;
            //    }
            //    else if (msg.Contains("WIDTH"))
            //    {
            //        debug("loader width change");
            //        // 두번째 칼럼이 숫자면 WIDTH_START 전송
            //        try
            //        {
            //            double width = int.Parse(msg.Substring(6)) / 100;
            //            if (width > 0)
            //            {
            //                debug("loader width start send");
            //                WidthValueLoader = true;
            //                //WidthStartedLoader = true;
            //                //SendMessage(SerialLoader, "WIDTH_START");
            //                /*
            //                int startTime = GlobalVar.TickCount64;
            //                while (GlobalVar.TickCount64 - startTime < 20000 &&
            //                    !WidthStartLoader)
            //                {
            //                    debug("loader width start send");
            //                    WidthStartedLoader = true;
            //                    SendMessage(SerialLoader, "WIDTH_START");
            //                    Thread.Sleep(1000);
            //                }
            //                //*/
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            debug("width ?? " + msg.Substring(6));
            //            // 타입이 숫자형이 아니면 무시
            //        }
            //    }
            //    else
            //    {
            //        debug("loader unknown");
            //    }
            //}
        }

       
        public void SendMessage(SerialPort port, string text)
        {
            debug(port.PortName.ToString() + " send : " + text);
            char _sStx, _sEtx;
            _sStx = Convert.ToChar(0x02);
            _sEtx = Convert.ToChar(0x03);

            if (string.IsNullOrEmpty(text)) return;

            port.DiscardOutBuffer();
            port.DiscardInBuffer();
            //port.Write(_sStx + text + _sEtx);
            port.Write(text);

        }

        public void SendTrigger(bool Inconveyor)
        {
            //Vuquest 3320g (아진엑스텍)
            //SendMessage(InConveyorScanner, "T");
            //LDBR-560(자동화 라우터)
            SendMessage(InConveyorScanner, "L");            
        }


        public void ClearStatus()
        {
            WidthStartLoader = false;
            WidthStartUnloader = false;
            WidthStartedLoader = false;
            WidthStartedUnloader = false;
            ErrorLoader = 0;
            ErrorUnloader = 0;
        }

    }
}

