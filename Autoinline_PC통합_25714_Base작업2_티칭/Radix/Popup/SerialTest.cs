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

    public partial class SerialTest : Form
    {
        private SerialPort Port1;
        private System.Threading.Timer timerSerial; // Thread Timer
        private bool timerDoing = false;

        byte stx = 0x02;
        byte etx = 0x03;

        public string BufferLoad = "";

        public SerialTest()
        {
            InitializeComponent();

            #region 수신용 쓰레드 타이머 시작
            TimerCallback CallBackSerial = new TimerCallback(TimerSerial);
            timerSerial = new System.Threading.Timer(CallBackSerial, false, 0, 100);
            #endregion
        }

        private void TimerSerial(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            try
            {
                if (timerDoing)
                {
                    return;
                }
                timerDoing = true;

                this.Invoke(new EventHandler(delegate
                {
                    if (Port1 == null ||
                    !Port1.IsOpen)
                    {
                        tbReceiveText.Text = "not open";
                        //debug("not open");
                        timerDoing = false;
                        return;
                    }
                    String msg = Port1.ReadExisting();
                    if (msg != "")
                    {
                        debug("loadcell msg : " + msg);
                        debug("hex : " + BitConverter.ToString(System.Text.Encoding.ASCII.GetBytes(msg)));
                    }

                    try
                    {
                        byte[] arr_msg = Util.StringToByteArray(msg);

                        int sindex = -1; // msg.IndexOf(stx.ToString());
                        for (int i = 0; i < arr_msg.Length; i++)
                        {
                            if (arr_msg[i] == stx)
                            {
                                sindex = i;
                                break;
                            }
                        }
                        if (sindex < 0)
                        {
                            debug("no stx");
                            timerDoing = false;
                            return;
                        }
                        else
                        {
                            debug("stx : " + sindex.ToString() + stx);
                        }
                        int eindex = -1; // msg.IndexOf(etx.ToString(), sindex);
                        for (int i = sindex; i < arr_msg.Length; i++)
                        {
                            if (arr_msg[i] == etx)
                            {
                                eindex = i;
                                break;
                            }
                        }

                        if (eindex < 0)
                        {
                            debug("no etx");
                            timerDoing = false;
                            return;
                        }
                        else
                        {
                            debug("etx : " + eindex.ToString());
                        }
                        if (eindex - sindex != 24)
                        {
                            debug("length not 24");
                            timerDoing = false;
                            return;
                        }
                        else
                        {
                            debug("length : " + (eindex - sindex).ToString());
                        }
                        msg = msg.Substring(sindex + 10, 8);
                    }
                    catch
                    {
                        timerDoing = false;
                        return;
                    }

                    debug("loadcell receive : " + msg);
                    if (msg != null && msg.Length > 0)
                    {
                        try
                        {
                            tbProbe1.Text = double.Parse(msg.Replace("+", "")).ToString();
                        }
                        catch (Exception ex)
                        {
                            debug(ex.ToString() + " : msg");
                            debug(ex.StackTrace);
                        }
                    }

                    tbReceiveText.Text = msg + "\n";
                }));

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
                //debug(ex.StackTrace);
            }
            timerDoing = false;
            //try
            //{
            //    if (timerDoing)
            //    {
            //        //return;
            //    }
            //    timerDoing = true;

            //    this.Invoke(new EventHandler(delegate
            //    {
            //        if (Port1 == null)// ||
            //            //!Port1.IsOpen)
            //        {
            //            tbReceiveText.Text = "not open";
            //            timerDoing = false;
            //            return;
            //        }
            //        String msg = Port1.ReadExisting();
            //        debug("loadcell msg : " + msg);
            //        if (msg.Contains(Convert.ToChar(0x02)) && // StX
            //            msg.Contains(Convert.ToChar(0x01))) // ID
            //        {
            //            BufferLoad = msg;
            //        }
            //        else
            //        {
            //            BufferLoad += msg;
            //        }
            //        if (!msg.Contains(Convert.ToChar(0x03))) // ETX
            //        {
            //            return;
            //        }

            //        try
            //        {
            //            msg = BufferLoad.Substring(2, BufferLoad.Length - 3);
            //        }
            //        catch
            //        {
            //            return;
            //        }

            //        debug("loadcell receive : " + msg);
            //        if (msg != null && msg.Length > 0)
            //        {
            //            try
            //            {
            //                tbProbe1.Text = msg;
            //            }
            //            catch (Exception ex)
            //            {
            //                debug(ex.ToString() + " : msg");
            //                debug(ex.StackTrace);
            //            }
            //        }

            //        tbReceiveText.Text = msg + "\n";
            //    }));

            //}
            //catch (Exception ex)
            //{
            //    debug(ex.ToString());
            //    debug(ex.StackTrace);
            //    //debug(ex.StackTrace);
            //}
            //timerDoing = false;

        }

        private void debug(string str)
        {
            Util.Debug(str);
        }

        private void SerialTest_Load(object sender, EventArgs e)
        {
            try
            {
                cmbPortName.Items.AddRange(SerialPort.GetPortNames());
                cmbBaud.SelectedIndex = 3;
                cmbData.SelectedIndex = 0;
                cmbParity.SelectedIndex = 0;
                cmbHandshake.SelectedIndex = 0;
                cmbStopBit.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            if (cmbPortName.Items.Count > 0)
            {
                cmbPortName.SelectedIndex = 0;  // 컴포트 정보가 없을 경우 컴포트의 0번째를 사용
            }
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            Console.WriteLine("1");
            if (Port1 == null)
            {
                Console.WriteLine("2");
                try
                {
                    Port1 = new SerialPort();
                    Port1.PortName = cmbPortName.SelectedItem.ToString();
                    Port1.BaudRate = Convert.ToInt32(cmbBaud.SelectedItem);
                    //Port1.DataBits = Convert.ToInt32(cmbData.SelectedItem);
                    //Port1.Parity = (Parity)cmbParity.SelectedIndex;
                    //Port1.Handshake = (Handshake)cmbHandshake.SelectedIndex;
                    //Port1.StopBits = (StopBits)cmbStopBit.SelectedIndex;


                    Console.WriteLine("3");
                    if (!Port1.IsOpen)
                    {
                        Console.WriteLine("4");

                        // 연결
                        Console.WriteLine("5");
                        Port1.Open();
                    }
                    Console.WriteLine("6");
                    cbConnected.Checked = Port1.IsOpen;
                    Console.WriteLine("7");
                }
                catch (Exception ex)
                {
                    debug(ex.ToString() + " - " + cmbStopBit.SelectedIndex.ToString());
                    debug(ex.StackTrace);
                }

                //Port1.DataReceived += Port1_DataReceived;
            }
        }

        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            //if (Port1 != null
            //    && Port1.IsOpen)
            //{

                try
                {
                    // 현재 시리얼이 연결 상태이면 연결 해제
                    Port1.Close();
                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                    debug(ex.StackTrace);
                }
                cbConnected.Checked = false;
            //}
            Port1 = null;

        }
        private void Port1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.Invoke(new EventHandler(delegate
            {
                if (!Port1.IsOpen)
                {
                    tbReceiveText.Text = "not open";
                    return;
                }
                String msg = Port1.ReadExisting();
                try
                {
                    int stxIndex = msg.IndexOf(stx.ToString());
                    if (stxIndex < 0)
                    {
                        tbReceiveText.Text = msg + "\n" + "no stx";
                        return;
                    }
                    if (msg.Length - stxIndex < 24)
                    {
                        tbReceiveText.Text = msg + "\n" + "short";
                        return;
                    }
                    double val1 = double.Parse(msg.Substring(stxIndex + 1, 6)) * 0.1 * 2 / 3;
                    double val2 = double.Parse(msg.Substring(stxIndex + 7, 6)) * 0.1 * 2 / 3;
                    double val3 = double.Parse(msg.Substring(stxIndex + 13, 6)) * 0.1 * 2 / 3;
                    double val4 = double.Parse(msg.Substring(stxIndex + 19, 6)) * 0.1 * 2 / 3;


                    //-16384-16384-16384-16384

                    tbReceiveText.Text = msg + "\n" + stxIndex;
                    tbProbe1.Text = val1.ToString();
                }
                catch (Exception ex)
                {
                    debug(msg);
                    debug(ex.ToString());
                    debug(ex.StackTrace);
                }
            }));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] array = new byte[2];
            array[0] = byte.Parse(tbSendText1.Text);
            array[1] = byte.Parse(tbSendText2.Text);
            string sendText = Util.ByteArrayToString(array);
            Port1.WriteLine(sendText);
        }

        private void SerialTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                timerSerial.Dispose();
                if (Port1 != null)
                {
                    if (Port1.IsOpen)
                    {
                        Port1.Close();
                    }
                    Port1 = null;
                }
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (timerDoing)
                {
                    return;
                }
                timerDoing = true;

                if (Port1 == null ||
                    !Port1.IsOpen)
                {
                    tbReceiveText.Text = "not open";
                    //debug("not open");
                    timerDoing = false;
                    return;
                }
                String msg = Port1.ReadExisting();
                if(msg !="")
                {
                    debug("loadcell msg : " + msg);
                    debug("hex : " + BitConverter.ToString(System.Text.Encoding.ASCII.GetBytes(msg)));
                }
                
                /*
                if (msg.Contains(Convert.ToChar(0x02)) && // StX
                    msg.Contains(Convert.ToChar(0x01))) // ID
                {
                    BufferLoad = msg;
                    debug("stx + id");
                }
                else
                {
                    BufferLoad += msg;
                    debug("not stx + id");
                }
                if (!msg.Contains(Convert.ToChar(0x03))) // ETX
                {
                    debug("no etx");
                    timerDoing = false;
                    return;
                }
                //*/
                try
                {
                    byte[] arr_msg = Util.StringToByteArray(msg);

                    int sindex = -1; // msg.IndexOf(stx.ToString());
                    for (int i = 0; i < arr_msg.Length; i++)
                    {
                        if (arr_msg[i] == stx)
                        {
                            sindex = i;
                            break;
                        }
                    }
                    if (sindex < 0)
                    {
                        debug("no stx");
                        timerDoing = false;
                        return;
                    }
                    else
                    {
                        debug("stx : " + sindex.ToString() + stx);
                    }
                    int eindex = -1; // msg.IndexOf(etx.ToString(), sindex);
                    for (int i = sindex; i < arr_msg.Length; i++)
                    {
                        if (arr_msg[i] == etx)
                        {
                            eindex = i;
                            break;
                        }
                    }

                    if (eindex < 0)
                    {
                        debug("no etx");
                        timerDoing = false;
                        return;
                    }
                    else
                    {
                        debug("etx : " + eindex.ToString());
                    }
                    if (eindex - sindex != 24)
                    {
                        debug("length not 24");
                        timerDoing = false;
                        return;
                    }
                    else
                    {
                        debug("length : " + (eindex-sindex).ToString());
                    }
                    msg = msg.Substring(sindex + 10, 8);
                }
                catch
                {
                    timerDoing = false;
                    return;
                }

                debug("loadcell receive : " + msg);
                if (msg != null && msg.Length > 0)
                {
                    try
                    {
                        tbProbe1.Text = double.Parse(msg.Replace("+", "")).ToString();
                    }
                    catch (Exception ex)
                    {
                        debug(ex.ToString() + " : msg");
                        debug(ex.StackTrace);
                    }
                }

                tbReceiveText.Text = msg + "\n";

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
                //debug(ex.StackTrace);
            }
            timerDoing = false;

        }

        private void SerialTest_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            timerSerial.Dispose();
        }
    }
}
