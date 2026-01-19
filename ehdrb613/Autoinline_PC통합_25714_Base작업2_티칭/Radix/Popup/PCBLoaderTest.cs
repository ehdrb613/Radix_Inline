using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.IO; // MemoryStream


namespace Radix
{
    public partial class PCBLoaderTest : Form
    {        
        public PCBLoaderTest()
        {
            InitializeComponent();
        }
        //private PMCSerial loder = new PMCSerial(1, EnumBaudRate.Baud9600, 0, Parity.None, StopBits.None);

        private void PCBLoaderTest_Load(object sender, EventArgs e)
        {
            // 시리얼포트 목록 갱신
            cmbPort.DataSource = SerialPort.GetPortNames();

            // 기타 셋팅 목록 기본값 선택
            cmbBaud.SelectedIndex = 0;
            cmbData.SelectedIndex = 0;
            cmbParity.SelectedIndex = 0;
            cmbHandshake.SelectedIndex = 0;
        }

        private SerialPort _Port1;
        private SerialPort _Port2;
        private SerialPort Port1
        {
            get
            {
                if(_Port1 ==null)
                {                    
                    _Port1 = new SerialPort();
                    _Port1.PortName = "COM5";
                    _Port1.BaudRate = 19200;
                    _Port1.DataBits = 8;
                    _Port1.Parity = Parity.None;
                    _Port1.Handshake = Handshake.None;
                    _Port1.StopBits = StopBits.One;
                    _Port1.DataReceived += Port1_DataReceived;
                }
                return _Port1;
            }            
        }
        private SerialPort Port2
        {
            get
            {
                if (_Port2 == null)
                {
                    _Port2 = new SerialPort();
                    _Port2.PortName = "COM6";
                    _Port2.BaudRate = 19200;
                    _Port2.DataBits = 8;
                    _Port2.Parity = Parity.None;
                    _Port2.Handshake = Handshake.None;
                    _Port2.StopBits = StopBits.One;
                    _Port2.DataReceived += Port2_DataReceived;
                }
                return _Port2;
            }
        }
        private Boolean IsOpen1
        {
            get { return Port1.IsOpen; }
            set
            {
                if (value)
                {
                    Strings = "Port1 연결 됨";                    
                }
                else
                {
                    Strings = "Port1 연결 해제됨";                    
                }
            }
        }
        private Boolean IsOpen2
        {
            get { return Port2.IsOpen; }
            set
            {
                if (value)
                {
                    Strings = "Port2 연결 됨";
                }
                else
                {
                    Strings = "Port2 연결 해제됨";
                }
            }
        }

        /* 로그 제어 */
        private StringBuilder _Strings;
        
        /// 로그 객체        
        private String Strings
        {
            set
            {
                if (_Strings == null)
                    _Strings = new StringBuilder(1024);
                // 로그 길이가 1024자가 되면 이전 로그 삭제
                if (_Strings.Length >= (1024 - value.Length))
                    _Strings.Clear();
                // 로그 추가 및 화면 표시
                _Strings.AppendLine(value);
                textBox_Loder_Communication.Text = _Strings.ToString();
            }
        }





        public bool Serial_Connect()
        {
            if (!Port1.IsOpen)
            {
                // 현재 시리얼이 연결된 상태가 아니면 연결.
                //Port1.PortName = cmbPort.SelectedItem.ToString();
                //Port1.BaudRate = Convert.ToInt32(cmbBaud.SelectedItem);
                //Port1.DataBits = Convert.ToInt32(cmbData.SelectedItem);
                //Port1.Parity = (Parity)cmbParity.SelectedIndex;
                //Port1.Handshake = (Handshake)cmbHandshake.SelectedIndex;

                try
                {
                    // 연결
                    Port1.Open();
                    return false;
                }
                catch (Exception ex) { Strings = String.Format("[ERR] {0}", ex.Message); }
                return false;
            }
            else
            {
                // 현재 시리얼이 연결 상태이면 연결 해제
                Port1.Close();
                return true;
            }
            // 상태 변경
            IsOpen1 = Port1.IsOpen;
            /////////////////////////////////////////////////////////////////////////////////////////////////
            //if (!Port2.IsOpen)
            //{
            //    // 현재 시리얼이 연결된 상태가 아니면 연결.
            //    //Port2.PortName = cmbPort.SelectedItem.ToString();
            //    //Port2.BaudRate = Convert.ToInt32(cmbBaud.SelectedItem);
            //    //Port2.DataBits = Convert.ToInt32(cmbData.SelectedItem);
            //    //Port2.Parity = (Parity)cmbParity.SelectedIndex;
            //    //Port2.Handshake = (Handshake)cmbHandshake.SelectedIndex;

            //    try
            //    {
            //        // 연결
            //        Port2.Open();
            //        return false;
            //    }
            //    catch (Exception ex) { Strings = String.Format("[ERR] {0}", ex.Message); }
            //    return false;
            //}
            //else
            //{
            //    // 현재 시리얼이 연결 상태이면 연결 해제
            //    Port2.Close();
            //    return true;
            //}
            //// 상태 변경
            //IsOpen2 = Port2.IsOpen;
        }


        

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            Serial_Connect();
        }

        private void btn_Disconnect_Click(object sender, EventArgs e)
        {            
            Port1.Close();
            cbConnected.Checked = false;
        }











        
        private void btn_SendMessage_Click(object sender, EventArgs e)
        {
            string Message = textBox1.Text ;
            char _sStx,_sEtx;
            _sStx = Convert.ToChar(0x02);
            _sEtx = Convert.ToChar(0x03);            

            if (string.IsNullOrEmpty(Message)) return;                     

            Port1.Write(_sStx + Message + _sEtx);
            
            textBox_Loder_Communication.Text = Message;
        }

        private void btn_Unload_SendMessage_Click(object sender, EventArgs e)
        {
            string Message = textBox1.Text;
            char _sStx, _sEtx;
            _sStx = Convert.ToChar(0x02);
            _sEtx = Convert.ToChar(0x03);

            if (string.IsNullOrEmpty(Message)) return;

            Port2.Write(_sStx + Message + _sEtx);

            textBox_Loder_Communication.Text = Message;

        }



        public void Loder_Send_Message(string Message)
        {
            char _sStx, _sEtx;
            _sStx = Convert.ToChar(0x02);
            _sEtx = Convert.ToChar(0x03);

            if (string.IsNullOrEmpty(Message)) return;

            Port1.Write(_sStx + Message + _sEtx);
        }

        public void Unloder_Send_Message(string Message)
        {
            char _sStx, _sEtx;
            _sStx = Convert.ToChar(0x02);
            _sEtx = Convert.ToChar(0x03);

            if (string.IsNullOrEmpty(Message)) return;

            Port2.Write(_sStx + Message + _sEtx);
        }




        private void Port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String msg = Port1.ReadExisting();

            this.Invoke(new EventHandler(delegate
            {
                Strings = String.Format("[PORT1_RECV] {0}", msg);
            }));
        }

        private void Port2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            String msg = Port2.ReadExisting();

            this.Invoke(new EventHandler(delegate
            {
                Strings = String.Format("[PORT2_RECV] {0}", msg);
            }));
        }




































        public string Receive()
        {
            byte[] buffer = new byte[1024];
            try
            {
                if (Port1.Read(buffer, 0, buffer.Length) == 0)
                {
                    return "";
                }
                return buffer.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
            }

            return "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Serial_Connect1();

        }

        public bool Serial_Connect1()
        {
            if (!Port2.IsOpen)
            {
                // 현재 시리얼이 연결된 상태가 아니면 연결.
                //Port2.PortName = cmbPort.SelectedItem.ToString();
                //Port2.BaudRate = Convert.ToInt32(cmbBaud.SelectedItem);
                //Port2.DataBits = Convert.ToInt32(cmbData.SelectedItem);
                //Port2.Parity = (Parity)cmbParity.SelectedIndex;
                //Port2.Handshake = (Handshake)cmbHandshake.SelectedIndex;

                try
                {
                    // 연결
                    Port2.Open();
                    return false;
                }
                catch (Exception ex) { Strings = String.Format("[ERR] {0}", ex.Message); }
                return false;
            }
            else
            {
                // 현재 시리얼이 연결 상태이면 연결 해제
                Port2.Close();
                return true;
            }
            // 상태 변경
            IsOpen2 = Port2.IsOpen;
        }
    }

}
