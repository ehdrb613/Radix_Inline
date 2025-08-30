//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
//using System.IO; // MemoryStream
//using System.Threading;
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
using System.Threading;
using System.Threading.Tasks;

namespace Radix
{
    public class SerialLVDT
    {
        /*
         * Panasonic LVDT 위치센서 1
         */

        #region 접속정보
        //private static int rsPort;
        //private static EnumBaudRate rsBaudRate;
        //private static int rsDataLength;
        //private static Parity rsParity;
        //private static StopBits rsStopBits;

        private int rsPort;
        private EnumBaudRate rsBaudRate;
        private int rsDataLength;
        private Parity rsParity;
        private StopBits rsStopBits;
        //        private static SerialPort serialPort = new SerialPort();    // 시리얼포트 선언
        //private static SerialPort serialPort = new SerialPort();    // 시리얼포트 선언
        private SerialPort serialPort = new SerialPort();    // 시리얼포트 선언
        #endregion

        #region 상태정보        
        public double pv = 0;
        public double sv = 0;
        public bool SendOK = false;
        public bool Write = false;
        public bool NS10LT = false;
        #endregion
        string BufferLoader = "";

        public string UNIT = "";

        public SerialLVDT(string str)
        {
            UNIT = str;
        }

        public void debug(string str)
        {
            Util.Debug("SerialLVDT : " + str);
        }

        public void SetCommunication(int Port, EnumBaudRate BaudRate, int DataLength, Parity Parity, StopBits StopBits)
        {
            rsPort = Port;
            rsBaudRate = BaudRate;
            rsDataLength = DataLength;
            rsParity = Parity;
            rsStopBits = StopBits;
        }

        public bool Connect()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    return true;
                }
                debug("connect");
                //debug("port : COM" + rsPort.ToString());
                //debug("baud rate : " + (int)rsBaudRate);
                //debug("parity : " + (int)rsParity);
                //debug("data length : " + rsDataLength);
                //debug("stop bits : " + (int)rsStopBits);

                if (UNIT == "UNIT1")   // 하부시
                {
                    SetCommunication(5, EnumBaudRate.Baud9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
                }
                else if (UNIT == "UNIT2")  // 상부시
                {
                    SetCommunication(6, EnumBaudRate.Baud9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
                }
                else if (UNIT == "UNIT3")  // 갭높이
                {
                    SetCommunication(3, EnumBaudRate.Baud9600, 8, System.IO.Ports.Parity.None, System.IO.Ports.StopBits.One);
                }

                serialPort = new SerialPort("COM" + rsPort.ToString(), (int)rsBaudRate, rsParity, rsDataLength, rsStopBits);
                serialPort.PortName = "COM" + rsPort.ToString();
                serialPort.BaudRate = (int)rsBaudRate;
                serialPort.Parity = rsParity;
                serialPort.DataBits = rsDataLength;
                serialPort.StopBits = rsStopBits;
                serialPort.ReadTimeout = 500;
                serialPort.WriteTimeout = 500;
                //                serialPort.DataReceived += ReceiveData;

                serialPort.Open();
                serialPort.DataReceived += ReceiveData;
                return serialPort.IsOpen;
            }
            catch (Exception e)
            {

                FuncLog.WriteLog(e.ToString());
                FuncLog.WriteLog(e.StackTrace);
                //Console.WriteLine(e.ToString());
                //Console.WriteLine(e.StackTrace);
            }
            return false;
        }


        public void Disconnect()
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
            }
        }

        public bool Send(string sendText)
        {
            try
            {
                serialPort.WriteLine(sendText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        public bool Connected()
        {
            return serialPort.IsOpen;
        }

        public bool SendTrigger()
        {
            if (UNIT == "UNIT1")   // 하부시
            {

                if (!Func.SerialLVDT1.Connect())
                {
                    return false;
                }
                Func.LVDTSensor1_Left_PV = "";
                Func.LVDTSensor1_Right_PV = "";
                Func.SerialLVDT1.Send("M0\r\n");  // 송신 전문
            }
            else if (UNIT == "UNIT2")  // 상부시
            {
                if (!Func.SerialLVDT2.Connect())
                {
                    return false;
                }
                Func.LVDTSensor2_Left_PV = "";
                Func.LVDTSensor2_Right_PV = "";
                Func.SerialLVDT2.Send("M0\r\n");  // 송신 전문
                return true;
            }
            else if (UNIT == "UNIT3")   // 갭높이
            {
                if (!Func.SerialLVDT3.Connect())
                {
                    return false;
                }
                Func.LVDTSensor3_Left_PV = "";
                Func.LVDTSensor3_Right_PV = "";
                Func.SerialLVDT3.Send("M0\r\n");  // 송신 전문
                return true;
            }

            return true;
        }

        private void ReceiveData(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(50);
            String msg = serialPort.ReadExisting();
            try
            {

                //M0,-999.999,-999.999
                //tbReceiveText.Text = msg;
                string[] ComUnit = msg.Split(',');  // 문자열 파싱

                //tbReceiveText.Clear();
                int i = 0;

                foreach (string Unit in ComUnit)
                {
                    if (Unit.Length > 0)//공백제거
                    {
                        if (Unit == "M0")
                        {
                            continue;
                        }
                        else
                        {
                            i++;
                            if (i == 1)
                            {

                                if (UNIT == "UNIT1")
                                {
                                    Func.LVDTSensor1_Left_PV = Unit.ToString();
                                }
                                else if (UNIT == "UNIT2")
                                {
                                    Func.LVDTSensor2_Left_PV = Unit.ToString();
                                }
                                else if (UNIT == "UNIT3")
                                {
                                    Func.LVDTSensor3_Left_PV = Unit.ToString();
                                }


                            }
                            else if (i == 2)
                            {
                                if (UNIT == "UNIT1")
                                {
                                    Func.LVDTSensor1_Right_PV = Unit.ToString();
                                }
                                else if (UNIT == "UNIT2")
                                {
                                    Func.LVDTSensor2_Right_PV = Unit.ToString();
                                }
                                else if (UNIT == "UNIT3")
                                {
                                    Func.LVDTSensor3_Right_PV = Unit.ToString();
                                }



                            }

                        }
                    }
                }
                // 수신완료후 연결을 끊는다
                Disconnect();
                //tbReceiveText.Text = convmsg;
            }
            catch (Exception ex)
            {
                debug(msg);
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }
    }
}