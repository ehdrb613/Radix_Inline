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

        private SerialPort scanner = new SerialPort();
        


        string BufferLoader = "";        

        private void debug(string str)
        {
            //Util.Debug(str);
        }
        public Scanner()
        {
            
       
        }
        public void ComSet(string port, int baud, int databit, Parity parity, StopBits stopbit) // 통신설정값 저장
        {
            scanner.PortName = port;
            scanner.BaudRate = baud;
            scanner.DataBits = databit;
            scanner.Parity = parity;
            scanner.Handshake = Handshake.None;
            scanner.StopBits = stopbit;
            scanner.ReadTimeout = 1000;
            scanner.WriteTimeout = 1000;
            scanner.DataReceived += scanner_DataReceived;
        }
        public bool Connect() // serial 포트 연결
        {
            if (!scanner.IsOpen)
            {
                try
                {
                    //debug("SMD connect");
                    // 연결
                    if (GlobalVar.Simulation)
                    {
                        return true;
                    }
                    else
                    {
                        scanner.Open();
                        //FuncLog.WriteLog_Tester(SMD.PortName + " CONNECT " + SMD.IsOpen.ToString());
                        return scanner.IsOpen;
                    }
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
    

  

        public void Disconnect()
        {
            debug("disconnect");
            scanner.Close();       
        }

        public void scanner_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            FuncInline.Load_Data_Received = true;

            String msg = scanner.ReadExisting();
            FuncLog.WriteLog(msg);
           
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
                msg.Trim();

                if(msg.Length >= 13)
                {
                    msg = BufferLoader.Substring(0, 13);
                }
                
            }
            catch
            {
                return;
            }

            //debug("InConveyorScanner receive : " + msg);
            FuncLog.WriteLog($"receive: {msg}");
            FuncInline.Load_Scanner = msg;
            msg = "";
            BufferLoader = "";

         
        }

       
        public void SendMessage(SerialPort port, string text)
        {
            // debug(port.PortName.ToString() + " send : " + text);
            char _sStx, _sEtx;
            _sStx = Convert.ToChar(0x02);
            _sEtx = Convert.ToChar(0x03);

            if (string.IsNullOrEmpty(text)) return;

            port.DiscardOutBuffer();
            port.DiscardInBuffer();
            //port.Write(_sStx + text + _sEtx);
            port.Write(text);

        }

        public void SendTrigger()
        {
            //Vuquest 3320g (아진엑스텍)
            //SendMessage(scanner, "T");
            //LDBR-560(자동화 라우터)
            SendMessage(scanner, "L");            
        }


        public void ClearStatus()
        {
            
        }

    }
}

