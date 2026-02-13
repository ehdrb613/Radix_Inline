using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.IO; // MemoryStream
using System.Threading;

namespace Radix
{
    public class LoadCell
    {
        private SerialPort SerialLoadCell = new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One);

        public double Load = 0; // 실제 측정된 값
        public string BufferLoad = "";

        private void debug(string str)
        {
            Util.Debug(str);
        }
        public LoadCell()
        {
            SerialLoadCell.PortName = "COM2";
            SerialLoadCell.BaudRate = 9600;
            SerialLoadCell.DataBits = 8;
            SerialLoadCell.Parity = Parity.None;
            SerialLoadCell.Handshake = Handshake.None;
            SerialLoadCell.StopBits = StopBits.One;
            SerialLoadCell.ReadTimeout = 1000;
            SerialLoadCell.WriteTimeout = 1000;
            SerialLoadCell.DataReceived += SerialLoadCell_DataReceived;
        }

        public bool Connect()
        {
            if (!SerialLoadCell.IsOpen)
            {
                try
                {
                    debug("loadcell connect");
                    // 연결
                    SerialLoadCell.Open();
                    debug("connect : " + SerialLoadCell.IsOpen.ToString());
                    return SerialLoadCell.IsOpen;
                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                    debug(ex.StackTrace);
                    return false;
                }
            }
            else
            {
                debug("loadcell already connected");
            }
            return true;
        }

        public void Disconnect()
        {
            debug("disconnect");
            SerialLoadCell.Close();
        }

        public void SerialLoadCell_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String msg = SerialLoadCell.ReadExisting();
            String msg1,msg2 ;

            debug("loadcell msg : " + msg);
            if (msg.Contains(Convert.ToChar(0x02))) //&& // StX
                                                    //msg.Contains(Convert.ToChar(0x01))) // ID
            {
                BufferLoad = msg;
            }
            else
            {
                BufferLoad += msg;
            }
            if (!msg.Contains(Convert.ToChar(0x03))) // ETX
            {
                return;
            }            
            if (BufferLoad.Length < 24) // 데이터 길이가 작다면 좀더 모아라
            {                
                return;
            }
            GlobalVar.LoadcellDataLength = BufferLoad.Length;

            try
            {
                //GlobalVar.LoadcellDataLength = BufferLoad.Length;
                if (BufferLoad.Length > 2000)
                {
                    BufferLoad = "";
                    msg = "";
                    return;
                }

                //msg = BufferLoad.Substring(BufferLoad.Length - 50, BufferLoad.Length);
                msg1 = BufferLoad.Substring(BufferLoad.LastIndexOf("+") + 1);
                msg2 = msg1.Substring(0, 7);
            }
            catch (Exception ex)
            {
                debug(ex.ToString() + " : msg");
                debug(ex.StackTrace);
                return;
            }

            debug("loadcell receive : " + msg + "  " + BufferLoad.Length);
            if (msg != null && msg.Length > 0)
            {
                try
                {
                    GlobalVar.LoadcellData = double.Parse(msg2);
                    BufferLoad = "";
                    msg = "";
                    //Disconnect();
                    //Load = double.Parse(msg);
                }
                catch (Exception ex)
                {
                    debug(ex.ToString() + " : msg");
                    debug(ex.StackTrace);
                }
            }
        }

        public void SendTrigger()
        {
            try
            {
                char _sStx = Convert.ToChar(0x02);
                char _sR = Convert.ToChar(0x52);

                SerialLoadCell.DiscardOutBuffer();
                SerialLoadCell.DiscardInBuffer();
                SerialLoadCell.Write("" + _sStx + _sR);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

    }

}
