using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.IO; // MemoryStream
using System.Threading;
using System.Windows.Forms;

namespace Radix
{
    public class Print_Zebra_ZT610
    {
        private SerialPort Printer = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
        

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
        public Print_Zebra_ZT610()
        {
            Printer.PortName = "COM1";
            Printer.BaudRate = 9600;
            Printer.DataBits = 8;
            Printer.Parity = Parity.None;
            Printer.Handshake = Handshake.None;
            Printer.StopBits = StopBits.One;
            Printer.ReadTimeout = 1000;
            Printer.WriteTimeout = 1000;
            Printer.DataReceived += Printer_DataReceived;
        
        }

        public bool Print_Connect()
        {
            if (!Printer.IsOpen)
            {
                try
                {
                    debug("connect");
                    // 연결
                    Printer.Open();
                    return Printer.IsOpen;
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
            Printer.Close();            
        }

        public void Printer_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String msg = Printer.ReadExisting();
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

            debug("InConveyorScanner receive : " + msg);           
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

        public void SendTrigger(string data)
        {
            SendMessage(Printer, data);
            
        }

        public void DataMix()
        {
            if (GlobalVar.Printer.Print_Connect())
            {
                string BarCode = string.Empty;

                BarCode = "^XA";                                                            // 시작
                BarCode = BarCode + "\r\n^LH10,50";                                         // 기준점

                BarCode = BarCode + @"\r\n^FO340,20^BQN,2,6^FH\^FDLA," + GlobalVar.Label_Barcode + "^FS";    // 2차원바코드
                BarCode = BarCode + "\r\n^FO340,160^A0N,20,20^FD" + GlobalVar.Label_Barcode + "^FS";     // 바코드 내용 표시

                BarCode = BarCode + "\r\n^FO00,010^A0N,30,30^FD" + GlobalVar.Label_String01 + "^FS";   // 첫번째 줄
                BarCode = BarCode + "\r\n^FO00,050^A0N,30,30^FD" + GlobalVar.Label_String02 + "^FS";        // 두번째 줄

                //BarCode = BarCode + "\r\n^FO0,90^GFA,00128,00128,00004,:Z64:eJxjYKA6+ADBjD8QGCaGCQDcjgas:A8B3";         //●
                BarCode = BarCode + "\r\n^FO00,090^A0N,25,25^FD" + GlobalVar.Label_String03 + "^FS";        // 세번째 줄
                BarCode = BarCode + "\r\n^FO00,120^A0N,25,25^FD" + GlobalVar.Label_String04 + "^FS";        // 네번째 줄
                BarCode = BarCode + "\r\n^FO00,150^A0N,25,25^FD" + GlobalVar.Label_String05 + "^FS";        // 다섯번째 줄
                BarCode = BarCode + "\r\n^FO00,180^A0N,25,25^FD" + GlobalVar.Label_String06 + "^FS";        // 여섯번째 줄
                BarCode = BarCode + "\r\n^FO00,210^A0N,25,25^FD" + GlobalVar.Label_String07 + "^FS";        // 일곱번째 줄
                BarCode = BarCode + "\r\n^FO00,240^A0N,25,25^FD" + GlobalVar.Label_String08 + "^FS";        // 여덟번째 줄
                BarCode = BarCode + "\r\n^FO00,270^A0N,25,25^FD" + GlobalVar.Label_String09 + "^FS";        // 아홉번째 줄
                BarCode = BarCode + "\r\n^FO00,300^A0N,25,25^FD" + GlobalVar.Label_String10 + "^FS";        // 열번째 줄

                BarCode = BarCode + "\r\n^FO00,330^A0N,25,25^FD" + GlobalVar.Label_String11 + "^FS";        // 열한번째 줄
                BarCode = BarCode + "\r\n^FO00,360^A0N,25,25^FD" + GlobalVar.Label_String12 + "^FS";        // 열두번째 줄               

                // CS 마크
                //BarCode = BarCode + "\r\n^FO350,250^GFA,02560,02560,00020,:Z64:eJzt1L1txiAQBmAQBaVH8CiMhkdjFEZwSYFMbN1reLEhSRlFH42lx3Acf6fUp33XdJKvIXNipjQyRcyRrWK6kPmSr89Cdv7PMrSZhRWyVcywOTHL5sUWtiLmyDTMkxlYIbtip4ctYpptFbNsbmCUXqT0ooQ4OOVNQiRK+UDYOtTgv5fut+0SooWzGIONvS1I2DrFldcmtrMhbJ32zPWABbL8MidmKb3TEkyR7XNryzjXGSWlkWW2MLfuOIKk/qNtv7Ly52263tG+PCzOrdvnfWbdWdZ7MLLuHuSXtfu3kd33NAwsVpvc8aAeb2HyZl5va/IGD1idWONNe54Yb9/xJL6Vq7oS1JLu4GDdgYzqEOqVGdS1roaZQa3TMK6TCsb19OqQ1KPuonp19XkV6+q4xQ6y3bvq+NhhS38VEvo30zhGT6ZswmTq3dzAPu3/ty8BCASQ:8FB8^PQ1";

                BarCode = BarCode + "\r\n^XZ";

                GlobalVar.Printer.SendTrigger(BarCode);

            }
            else
            {
                MessageBox.Show("연결 되지 않음");
            }

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

