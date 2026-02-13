using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.IO; // MemoryStream
using System.Threading;

namespace Radix
{
    public class LoadUnload
    {
        /*
         * LoadUnload.cs : 일진 PCB 로더/언로더 제어
         */

        private SerialPort SerialLoader = new SerialPort("COM5", 9600, Parity.None, 8, StopBits.One);
        private SerialPort SerialUnloader = new SerialPort("COM6", 9600, Parity.None, 8, StopBits.One);

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
        string BufferUnloader = "";

        private void debug(string str)
        {
            //Util.Debug(str);
        }
        public LoadUnload()
        {
            SerialLoader.PortName = "COM5";
            SerialLoader.BaudRate = 9600;
            SerialLoader.DataBits = 8;
            SerialLoader.Parity = Parity.None;
            SerialLoader.Handshake = Handshake.None;
            SerialLoader.StopBits = StopBits.One;
            SerialLoader.ReadTimeout = 1000;
            SerialLoader.WriteTimeout = 1000;
            SerialLoader.DataReceived += SerialLoader_DataReceived;

            SerialUnloader.PortName = "COM6";
            SerialUnloader.BaudRate = 9600;
            SerialUnloader.DataBits = 8;
            SerialUnloader.Parity = Parity.None;
            SerialUnloader.Handshake = Handshake.None;
            SerialUnloader.StopBits = StopBits.One;
            SerialUnloader.ReadTimeout = 1000;
            SerialUnloader.WriteTimeout = 1000;
            SerialUnloader.DataReceived += SerialUnloader_DataReceived;
        }

        public bool Loader_Connect()
        {
            /*
            SerialLoader = new SerialPort();
            SerialLoader.PortName = "COM5";
            SerialLoader.BaudRate = 19200;
            SerialLoader.DataBits = 8;
            SerialLoader.Parity = Parity.None;
            SerialLoader.Handshake = Handshake.None;
            SerialLoader.StopBits = StopBits.One;
            SerialLoader.ReadTimeout = 500;
            SerialLoader.WriteTimeout = 500;
            SerialLoader.DataReceived += SerialLoader_DataReceived;
       //*/

            if (!SerialLoader.IsOpen)
            {
                try
                {
                    debug("loader connect");
                    // 연결
                    SerialLoader.Open();
                   return SerialLoader.IsOpen;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                //debug("loader connected");


            }
            return true;
        }

        public bool Unloader_Connect()
        {
            /*
            SerialUnloader = new SerialPort();
            SerialUnloader.PortName = "COM6";
            SerialUnloader.BaudRate = 19200;
            SerialUnloader.DataBits = 8;
            SerialUnloader.Parity = Parity.None;
            SerialUnloader.Handshake = Handshake.None;
            SerialUnloader.StopBits = StopBits.One;
            SerialUnloader.ReadTimeout = 500;
            SerialUnloader.WriteTimeout = 500;
            SerialUnloader.DataReceived += SerialUnloader_DataReceived;
            //*/

            if (!SerialUnloader.IsOpen)
            {
                try
                {
                    debug("unloader connect");
                    // 연결
                    SerialUnloader.Open();
                    return SerialUnloader.IsOpen;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                //debug("unloader connected");
            }
            return true;
        }

        public void Disconnect()
        {
            debug("disconnect");
            SerialLoader.Close();
            SerialUnloader.Close();
        }

        public void SerialLoader_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String msg = SerialLoader.ReadExisting();
            if (msg.Contains(Convert.ToChar(0x02))) // StX
            {
                BufferLoader = msg;
            }
            else
            {
                BufferLoader += msg;
            }
            if (!msg.Contains(Convert.ToChar(0x03))) // ETX
            {
                return;
            }

            try
            {
                msg = BufferLoader.Substring(1, BufferLoader.Length - 2);
            }
            catch
            {
                return;
            }

            debug("loader receive : " + msg);
            if (msg != null && msg.Length > 0)
            {
                if (msg.Length >= 8 &&
                    msg.Substring(0, 5) == "ERROR")
                {
                    debug("loader error : ");
                    try
                    {
                        ErrorLoader = int.Parse(msg.Substring(7));
                    }
                    catch (Exception ex)
                    {
                        debug("loader error : " + ErrorLoader);
                    }
                }
                if (msg.Contains("MANUAL"))
                {
                    debug("loader manual");
                    AutoLoader = false;
                }
                if (msg.Contains("AUTO"))
                {
                    debug("loader auto");
                    AutoLoader = true;
                }

                if (msg.Contains("WIDTH_START"))
                {
                    debug("loader width started");
                    WidthStartLoader = true;
                }
                else if (msg.Contains("WIDTH_END"))
                {
                    debug("loader width end");
                    // 세번째 칼럼이 숫자면 WIDTH 값 저장
                    try
                    {
                        double width = int.Parse(msg.Substring(10)) / 100;
                        if (width > 0)
                        {
                            WidthLoader = width;
                        }
                        debug("loader width end");
                    }
                    catch (Exception ex)
                    {
                        debug("width : " + msg.Substring(10));
                        // 타입이 숫자형이 아니면 무시
                    }
                    WidthStartLoader = false;
                    WidthStartedLoader = false;
                }
                else if (msg.Contains("WIDTH_ABORT"))
                {
                    debug("loader width abort");
                    WidthStartedLoader = false;
                    WidthStartLoader = false;
                }
                else if (msg.Contains("WIDTH"))
                {
                    debug("loader width change");
                    // 두번째 칼럼이 숫자면 WIDTH_START 전송
                    try
                    {
                        double width = int.Parse(msg.Substring(6)) / 100;
                        if (width > 0)
                        {
                            debug("loader width start send");
                            WidthValueLoader = true;
                            //WidthStartedLoader = true;
                            //SendMessage(SerialLoader, "WIDTH_START");
                            /*
                            int startTime = GlobalVar.TickCount64;
                            while (GlobalVar.TickCount64 - startTime < 20000 &&
                                !WidthStartLoader)
                            {
                                debug("loader width start send");
                                WidthStartedLoader = true;
                                SendMessage(SerialLoader, "WIDTH_START");
                                Thread.Sleep(1000);
                            }
                            //*/
                        }
                    }
                    catch (Exception ex)
                    {
                        debug("width ?? " + msg.Substring(6));
                        // 타입이 숫자형이 아니면 무시
                    }
                }
                else
                {
                    debug("loader unknown");
                }
            }
        }

        public void SerialUnloader_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String msg = SerialUnloader.ReadExisting();
            if (msg.Contains(Convert.ToChar(0x02))) // StX
            {
                BufferUnloader = msg;
            }
            else
            {
                BufferUnloader += msg;
            }
            if (!msg.Contains(Convert.ToChar(0x03))) // ETX
            {
                return;
            }
            msg = BufferUnloader.Substring(0, BufferUnloader.Length - 1);
            debug("unloader receive : " + msg);
            if (msg != null && msg.Length > 0)
            {
                if (msg.Length >= 9 &&
                    msg.Substring(1, 5) == "ERROR")
                {
                    debug("unloader error checked");
                    try
                    {
                        ErrorUnloader = int.Parse(msg.Substring(7));
                    }
                    catch (Exception ex)
                    {
                        debug("Unloader error : " + ErrorUnloader);
                    }
                }
                if (msg.Contains("MANUAL"))
                {
                    debug("unloader manual");
                    AutoUnloader = false;
                }
                if (msg.Contains("AUTO"))
                {
                    debug("unloader auto");
                    AutoUnloader = true;
                }
                if (msg.Contains("WIDTH_START"))
                {
                    debug("unloader width started");
                    WidthStartUnloader = true;
                }
                else if (msg.Contains("WIDTH_END"))
                {
                    debug("unloader width end");
                    // 세번째 칼럼이 숫자면 WIDTH 값 저장
                    try
                    {
                        double width = int.Parse(msg.Substring(11)) / 100;
                        if (width > 0)
                        {
                            WidthUnloader = width;
                        }
                    }
                    catch (Exception ex)
                    {
                        debug("width : " + msg.Substring(11));
                        // 타입이 숫자형이 아니면 무시
                    }
                    WidthStartUnloader = false;
                    WidthStartedUnloader = false;
                }
                else if (msg.Contains("WIDTH_ABORT"))
                {
                    debug("unloader width aborted");
                    WidthStartUnloader = false;
                    WidthStartedUnloader = false;
                }
                else if (msg.Contains("WIDTH"))
                {
                    debug("unloader width change");
                    // 두번째 칼럼이 숫자면 WIDTH_START 전송
                    try
                    {
                        double width = int.Parse(msg.Substring(7)) / 100;
                        if (width > 0)
                        {
                            debug("unloader width start send");
                            WidthValueUnloader = true;
                            //WidthStartedUnloader = true;
                            //SendMessage(SerialUnloader, "WIDTH_START");
                            /*
                            int startTime = GlobalVar.TickCount64;
                            while (GlobalVar.TickCount64 - startTime < 20000 &&
                                !WidthStartUnloader)
                            {
                                debug("unloader width start send");
                                WidthStartedUnloader = true;
                                SendMessage(SerialUnloader, "WIDTH_START");
                                Thread.Sleep(1000);
                            }
                            //*/
                        }
                    }
                    catch (Exception ex)
                    {
                        // 타입이 숫자형이 아니면 무시
                    }
                }
                else
                {
                    debug("unloader unknown");
                }
            }
        }

        private void SendMessage(SerialPort port, string text)
        {
            debug(port.PortName.ToString() + " send : " + text);
            char _sStx, _sEtx;
            _sStx = Convert.ToChar(0x02);
            _sEtx = Convert.ToChar(0x03);

            if (string.IsNullOrEmpty(text)) return;

            port.DiscardOutBuffer();
            port.DiscardInBuffer();
            port.Write(_sStx + text + _sEtx);
        }

        public bool SendWidth(bool loader, double width)
        {
            ulong startTime = GlobalVar.TickCount64;
            if (loader)
            {
                //debug("loader width : " + width.ToString());
                WidthStartedLoader = true;
                // WIDTH_0000 리턴 올 때까지 폭지령
                while (!WidthValueLoader)
                {
                    if (GlobalVar.TickCount64 - startTime > (ulong)GlobalVar.NormalTimeout * 1000)
                    {
                        return false;
                    }
                    SendMessage(SerialLoader, "WIDTH_" + ((int)(width * 100)).ToString());
                    Thread.Sleep(GlobalVar.ThreadSleep);
                }
                // WIDTH_START 올 때까지 WIDTH_START 지령
                startTime = GlobalVar.TickCount64;
                while (!WidthStartLoader)
                {
                    if (GlobalVar.TickCount64 - startTime > (ulong)GlobalVar.NormalTimeout * 1000)
                    {
                        return false;
                    }
                    SendMessage(SerialLoader, "WIDTH_START");
                    Thread.Sleep(GlobalVar.ThreadSleep);
                }
                // WIDTH_END_0000 또는 WIDTH_ABORT_0000 올 때까지 대기
                startTime = GlobalVar.TickCount64;
                while (WidthStartLoader ||
                        WidthStartedLoader)
                {
                    if (GlobalVar.TickCount64 - startTime > (ulong)GlobalVar.NormalTimeout * 1000)
                    {
                        return false;
                    }
                    SendMessage(SerialLoader, "WIDTH_START");
                    Thread.Sleep(GlobalVar.ThreadSleep);
                }
            }
            else
            {
                //debug("unloader width : " + width.ToString());
                WidthStartedUnloader = true;
                // WIDTH_0000 리턴 올 때까지 폭지령
                while (!WidthValueUnloader)
                {
                    if (GlobalVar.TickCount64 - startTime > (ulong)GlobalVar.NormalTimeout * 1000)
                    {
                        return false;
                    }
                    SendMessage(SerialUnloader, "WIDTH_" + ((int)(width * 100)).ToString());
                    Thread.Sleep(GlobalVar.ThreadSleep);
                }
                // WIDTH_START 올 때까지 WIDTH_START 지령
                startTime = GlobalVar.TickCount64;
                while (!WidthStartUnloader)
                {
                    if (GlobalVar.TickCount64 - startTime > (ulong)GlobalVar.NormalTimeout * 1000)
                    {
                        return false;
                    }
                    SendMessage(SerialUnloader, "WIDTH_START");
                    Thread.Sleep(GlobalVar.ThreadSleep);
                }
                // WIDTH_END_0000 또는 WIDTH_ABORT_0000 올 때까지 대기
                startTime = GlobalVar.TickCount64;
                while (WidthStartUnloader ||
                        WidthStartedUnloader)
                {
                    if (GlobalVar.TickCount64 - startTime > (ulong)GlobalVar.NormalTimeout * 1000)
                    {
                        return false;
                    }
                    SendMessage(SerialLoader, "WIDTH_START");
                    Thread.Sleep(GlobalVar.ThreadSleep);
                }
            }
            return true;
        }

        public void SendCondition(bool loader)
        {
            /* 20200218 컨디션 요청 삭제
            if (loader &&
                !WidthStartLoader &&
                !WidthStartedLoader)
            {
                SendMessage(SerialLoader, "CONDITION");
                //SendMessage(SerialLoader, "WIDTH_START");
            }
            else if (!WidthStartUnloader &&
                !WidthStartedUnloader)
            {
                SendMessage(SerialUnloader, "CONDITION");
                //SendMessage(SerialUnloader, "WIDTH_START");
            }
            //*/
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
