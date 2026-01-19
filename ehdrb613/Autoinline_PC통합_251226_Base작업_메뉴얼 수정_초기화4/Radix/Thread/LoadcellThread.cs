using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가


namespace Radix
{
    class LoadcellThread
    {
        /*
         * LoadcellThread.cs : 로드셀 통신 수행 쓰레드
         */

        private void debug(string str) // 클래스 내 로컬 디버그
        {
            Util.Debug(str);
        }

        #region 로컬 변수
        private SerialPort Port1;
        private System.Threading.Timer timerSerial; // Thread Timer
        private bool timerDoing = false;

        byte stx = 0x02;
        byte etx = 0x03;
        byte R = 0x52;

        public string BufferLoad = "";
        #endregion

        public void Run()
        {
            Port1 = new SerialPort();
            Port1.PortName = GlobalVar.LoadcellPort;
            Port1.BaudRate = GlobalVar.LoadcellBaud;
            while (!GlobalVar.GlobalStop)
            {
                if (Port1 == null ||
                    !Port1.IsOpen)
                {
                    try
                    {
                        if (!Port1.IsOpen)
                        {
                            Port1.Open();
                        }
                    }
                    catch (Exception ex)
                    {
                        debug(ex.ToString());
                        debug(ex.StackTrace);
                    }
                }

                if (Port1 == null ||
                    !Port1.IsOpen)
                {
                    
                    Thread.Sleep(GlobalVar.ThreadSleep);
                    GlobalVar.LoadcellOpened = false;
                    continue;
                }
                GlobalVar.LoadcellOpened = true;

                String msg = Port1.ReadExisting();
                String a = Port1.ReadLine();

                if(a==null)
                {
                    char _sStx, _sEtx;
                    _sStx = Convert.ToChar(0x01);
                    _sEtx = Convert.ToChar(0x52);
                    Port1.Write(_sStx + "" +_sEtx);
                    //Port1.Write(_sStx + _sEtx);                    
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
                        //debug("no stx");
                        timerDoing = false;
                        return;
                    }
                    else
                    {
                        //debug("stx : " + sindex.ToString() + stx);
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
                        //debug("no etx");
                        timerDoing = false;
                        return;
                    }
                    else
                    {
                        //debug("etx : " + eindex.ToString());
                    }
                    if (eindex - sindex != 24)
                    {
                        //debug("length not 24");
                        timerDoing = false;
                        return;
                    }
                    else
                    {
                        //debug("length : " + (eindex - sindex).ToString());
                    }
                    msg = msg.Substring(sindex + 10, 8);
                }
                catch
                {
                    timerDoing = false;
                    return;
                }

                //debug("loadcell receive : " + msg);
                if (msg != null && msg.Length > 0)
                {
                    try
                    {
                        GlobalVar.LoadcellData = double.Parse(msg.Replace("+", ""));
                    }
                    catch (Exception ex)
                    {
                        debug(ex.ToString() + " : msg");
                        debug(ex.StackTrace);
                    }
                }

                Thread.Sleep(GlobalVar.ThreadSleep);
            }

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


    }
}
