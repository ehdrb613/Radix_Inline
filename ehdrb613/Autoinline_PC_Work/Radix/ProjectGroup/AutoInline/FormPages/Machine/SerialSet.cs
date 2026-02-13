using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.Threading;

namespace Radix.Popup.Machine
{
    public partial class SerialSet : Form
    {
        private bool valueChanged = false;
        private FuncInline.enumTabMain beforeMain = FuncInline.enumTabMain.Auto;
        private FuncInline.enumTabMachine beforeMachine = FuncInline.enumTabMachine.Machine;

        public SerialSet()
        {
            //debug("init");
            InitializeComponent();
            readSerialDefault();
        }

        private void debug(string str)
        {
            Util.Debug("frmSerial : " + str);
        }

        public void LoadAllValue()
        {
            try
            {
                Util.SetComboIndex(ref cmbPortTest1, "COM" + FuncInline.PortTest[0].ToString());
                Util.SetComboIndex(ref cmbBaudTest1, FuncInline.BaudTest[0].ToString());
                Util.SetComboIndex(ref cmbParityTest1, FuncInline.ParityTest[0].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest1, FuncInline.StopBitsTest[0].ToString());

                Util.SetComboIndex(ref cmbPortTest2, "COM" + FuncInline.PortTest[1].ToString());
                Util.SetComboIndex(ref cmbBaudTest2, FuncInline.BaudTest[1].ToString());
                Util.SetComboIndex(ref cmbParityTest2, FuncInline.ParityTest[1].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest2, FuncInline.StopBitsTest[1].ToString());

                Util.SetComboIndex(ref cmbPortTest3, "COM" + FuncInline.PortTest[2].ToString());
                Util.SetComboIndex(ref cmbBaudTest3, FuncInline.BaudTest[2].ToString());
                Util.SetComboIndex(ref cmbParityTest3, FuncInline.ParityTest[2].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest3, FuncInline.StopBitsTest[2].ToString());


                Util.SetComboIndex(ref cmbPortTest4, "COM" + FuncInline.PortTest[3].ToString());
                Util.SetComboIndex(ref cmbBaudTest4, FuncInline.BaudTest[3].ToString());
                Util.SetComboIndex(ref cmbParityTest4, FuncInline.ParityTest[3].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest4, FuncInline.StopBitsTest[3].ToString());

                Util.SetComboIndex(ref cmbPortTest5, "COM" + FuncInline.PortTest[4].ToString());
                Util.SetComboIndex(ref cmbBaudTest5, FuncInline.BaudTest[4].ToString());
                Util.SetComboIndex(ref cmbParityTest5, FuncInline.ParityTest[4].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest5, FuncInline.StopBitsTest[4].ToString());

                Util.SetComboIndex(ref cmbPortTest6, "COM" + FuncInline.PortTest[5].ToString());
                Util.SetComboIndex(ref cmbBaudTest6, FuncInline.BaudTest[5].ToString());
                Util.SetComboIndex(ref cmbParityTest6, FuncInline.ParityTest[5].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest6, FuncInline.StopBitsTest[5].ToString());

                Util.SetComboIndex(ref cmbPortTest7, "COM" + FuncInline.PortTest[6].ToString());
                Util.SetComboIndex(ref cmbBaudTest7, FuncInline.BaudTest[6].ToString());
                Util.SetComboIndex(ref cmbParityTest7, FuncInline.ParityTest[6].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest7, FuncInline.StopBitsTest[6].ToString());

                Util.SetComboIndex(ref cmbPortTest8, "COM" + FuncInline.PortTest[7].ToString());
                Util.SetComboIndex(ref cmbBaudTest8, FuncInline.BaudTest[7].ToString());
                Util.SetComboIndex(ref cmbParityTest8, FuncInline.ParityTest[7].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest8, FuncInline.StopBitsTest[7].ToString());

                Util.SetComboIndex(ref cmbPortTest9, "COM" + FuncInline.PortTest[8].ToString());
                Util.SetComboIndex(ref cmbBaudTest9, FuncInline.BaudTest[8].ToString());
                Util.SetComboIndex(ref cmbParityTest9, FuncInline.ParityTest[8].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest9, FuncInline.StopBitsTest[8].ToString());

               

    

                Util.SetComboIndex(ref cmbPortScanner, "COM" + FuncInline.PortScanner.ToString());
                Util.SetComboIndex(ref cmbBaudScanner, FuncInline.BaudScanner.ToString());
                Util.SetComboIndex(ref cmbParityScanner, FuncInline.ParityScanner.ToString());
                Util.SetComboIndex(ref cmbStopBitsScanner, FuncInline.StopBitsScanner.ToString());

                valueChanged = false;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        public void ApplyAllValue()
        {
            try
            {
                FuncInline.PortTest[0] = int.Parse(cmbPortTest1.Text.Replace("COM", ""));
                int.TryParse(cmbBaudTest1.Text, out FuncInline.BaudTest[0]);
                FuncInline.ParityTest[0] = (Parity)cmbParityTest1.SelectedIndex;
                FuncInline.StopBitsTest[0] = (StopBits)cmbStopBitsTest1.SelectedIndex;

                FuncInline.PortTest[1] = int.Parse(cmbPortTest2.Text.Replace("COM", ""));
                int.TryParse(cmbBaudTest2.Text, out FuncInline.BaudTest[1]);
                FuncInline.ParityTest[1] = (Parity)cmbParityTest2.SelectedIndex;
                FuncInline.StopBitsTest[1] = (StopBits)cmbStopBitsTest2.SelectedIndex;

                FuncInline.PortTest[2] = int.Parse(cmbPortTest3.Text.Replace("COM", ""));
                int.TryParse(cmbBaudTest3.Text, out FuncInline.BaudTest[2]);
                FuncInline.ParityTest[2] = (Parity)cmbParityTest3.SelectedIndex;
                FuncInline.StopBitsTest[2] = (StopBits)cmbStopBitsTest3.SelectedIndex;

                FuncInline.PortTest[3] = int.Parse(cmbPortTest4.Text.Replace("COM", ""));
                int.TryParse(cmbBaudTest4.Text, out FuncInline.BaudTest[3]);
                FuncInline.ParityTest[3] = (Parity)cmbParityTest4.SelectedIndex;
                FuncInline.StopBitsTest[3] = (StopBits)cmbStopBitsTest4.SelectedIndex;

                FuncInline.PortTest[4] = int.Parse(cmbPortTest5.Text.Replace("COM", ""));
                int.TryParse(cmbBaudTest5.Text, out FuncInline.BaudTest[4]);
                FuncInline.ParityTest[4] = (Parity)cmbParityTest5.SelectedIndex;
                FuncInline.StopBitsTest[4] = (StopBits)cmbStopBitsTest5.SelectedIndex;

                FuncInline.PortTest[5] = int.Parse(cmbPortTest6.Text.Replace("COM", ""));
                int.TryParse(cmbBaudTest6.Text, out FuncInline.BaudTest[5]);
                FuncInline.ParityTest[5] = (Parity)cmbParityTest6.SelectedIndex;
                FuncInline.StopBitsTest[5] = (StopBits)cmbStopBitsTest6.SelectedIndex;

                FuncInline.PortTest[6] = int.Parse(cmbPortTest7.Text.Replace("COM", ""));
                int.TryParse(cmbBaudTest7.Text, out FuncInline.BaudTest[6]);
                FuncInline.ParityTest[6] = (Parity)cmbParityTest7.SelectedIndex;
                FuncInline.StopBitsTest[6] = (StopBits)cmbStopBitsTest7.SelectedIndex;

                FuncInline.PortTest[7] = int.Parse(cmbPortTest8.Text.Replace("COM", ""));
                int.TryParse(cmbBaudTest8.Text, out FuncInline.BaudTest[7]);
                FuncInline.ParityTest[7] = (Parity)cmbParityTest8.SelectedIndex;
                FuncInline.StopBitsTest[7] = (StopBits)cmbStopBitsTest8.SelectedIndex;

                FuncInline.PortTest[8] = int.Parse(cmbPortTest9.Text.Replace("COM", ""));
                int.TryParse(cmbBaudTest9.Text, out FuncInline.BaudTest[8]);
                FuncInline.ParityTest[8] = (Parity)cmbParityTest9.SelectedIndex;
                FuncInline.StopBitsTest[8] = (StopBits)cmbStopBitsTest9.SelectedIndex;

                FuncInline.PortScanner = int.Parse(cmbPortScanner.Text.Replace("COM", ""));
                int.TryParse(cmbBaudScanner.Text, out FuncInline.BaudScanner);
                FuncInline.ParityScanner = (Parity)cmbParityScanner.SelectedIndex;
                FuncInline.StopBitsScanner = (StopBits)cmbStopBitsScanner.SelectedIndex;

                valueChanged = false;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void readSerialDefault()
        {
            try
            {
                #region Port
                string[] ports = SerialPort.GetPortNames();

                cmbPortTest1.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTest1.Items.AddRange(ports);
                    cmbPortTest1.SelectedIndex = 0;
                }
                cmbPortTest2.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTest2.Items.AddRange(ports);
                    cmbPortTest2.SelectedIndex = 0;
                }
                cmbPortTest3.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTest3.Items.AddRange(ports);
                    cmbPortTest3.SelectedIndex = 0;
                }

                cmbPortTest4.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTest4.Items.AddRange(ports);
                    cmbPortTest4.SelectedIndex = 0;
                }

                cmbPortTest5.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTest5.Items.AddRange(ports);
                    cmbPortTest5.SelectedIndex = 0;
                }
                cmbPortTest6.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTest6.Items.AddRange(ports);
                    cmbPortTest6.SelectedIndex = 0;
                }
                cmbPortTest7.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTest7.Items.AddRange(ports);
                    cmbPortTest7.SelectedIndex = 0;
                }
                cmbPortTest8.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTest8.Items.AddRange(ports);
                    cmbPortTest8.SelectedIndex = 0;
                }
                cmbPortTest9.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTest9.Items.AddRange(ports);
                    cmbPortTest9.SelectedIndex = 0;
                }
             
                cmbPortScanner.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortScanner.Items.AddRange(ports);
                    cmbPortScanner.SelectedIndex = 0;
                }
                #endregion

                #region baud rate
                cmbBaudTest1.Items.Clear();
                cmbBaudTest2.Items.Clear();
                cmbBaudTest4.Items.Clear();
                cmbBaudTest5.Items.Clear();
                cmbBaudTest6.Items.Clear();
                cmbBaudTest7.Items.Clear();
                cmbBaudTest8.Items.Clear();
                cmbBaudTest9.Items.Clear();
              
                cmbBaudScanner.Items.Clear();
                Array arrBaud = Enum.GetValues(typeof(EnumBaudRate));
                for (int j = 0; j < arrBaud.Length; j++)
                {
                    string baudStr = arrBaud.GetValue(j).ToString().Replace("Baud", "");

                    cmbBaudTest1.Items.Add(baudStr);
                    cmbBaudTest1.SelectedIndex = 0;
                    cmbBaudTest2.Items.Add(baudStr);
                    cmbBaudTest2.SelectedIndex = 0;
                    cmbBaudTest4.Items.Add(baudStr);
                    cmbBaudTest4.SelectedIndex = 0;
                    cmbBaudTest5.Items.Add(baudStr);
                    cmbBaudTest5.SelectedIndex = 0;
                    cmbBaudTest6.Items.Add(baudStr);
                    cmbBaudTest6.SelectedIndex = 0;
                    cmbBaudTest7.Items.Add(baudStr);
                    cmbBaudTest7.SelectedIndex = 0;
                    cmbBaudTest8.Items.Add(baudStr);
                    cmbBaudTest8.SelectedIndex = 0;
                    cmbBaudTest9.Items.Add(baudStr);
                    cmbBaudTest9.SelectedIndex = 0;
                    cmbBaudScanner.Items.Add(baudStr);
                    cmbBaudScanner.SelectedIndex = 0;
                }
                #endregion

                #region parity
                cmbParityTest1.Items.Clear();
                cmbParityTest2.Items.Clear();
                cmbParityTest4.Items.Clear();
                cmbParityTest5.Items.Clear();
                cmbParityTest6.Items.Clear();
                cmbParityTest7.Items.Clear();
                cmbParityTest8.Items.Clear();
                cmbParityTest9.Items.Clear();
              
                cmbParityScanner.Items.Clear();
                Array arrParity = Enum.GetValues(typeof(Parity));
                for (int j = 0; j < arrParity.Length; j++)
                {
                    object parityObj = arrParity.GetValue(j);

                    cmbParityTest1.Items.Add(parityObj);
                    cmbParityTest1.SelectedIndex = 0;
                    cmbParityTest2.Items.Add(parityObj);
                    cmbParityTest2.SelectedIndex = 0;
                    cmbParityTest4.Items.Add(parityObj);
                    cmbParityTest4.SelectedIndex = 0;
                    cmbParityTest5.Items.Add(parityObj);
                    cmbParityTest5.SelectedIndex = 0;
                    cmbParityTest6.Items.Add(parityObj);
                    cmbParityTest6.SelectedIndex = 0;
                    cmbParityTest7.Items.Add(parityObj);
                    cmbParityTest7.SelectedIndex = 0;
                    cmbParityTest8.Items.Add(parityObj);
                    cmbParityTest8.SelectedIndex = 0;
                    cmbParityTest9.Items.Add(parityObj);
                    cmbParityTest9.SelectedIndex = 0;
                  
                    cmbParityScanner.Items.Add(parityObj);
                    cmbParityScanner.SelectedIndex = 0;
                }
                #endregion

                #region Stop bit
                cmbStopBitsTest1.Items.Clear();
                cmbStopBitsTest2.Items.Clear();
                cmbStopBitsTest4.Items.Clear();
                cmbStopBitsTest5.Items.Clear();
                cmbStopBitsTest6.Items.Clear();
                cmbStopBitsTest7.Items.Clear();
                cmbStopBitsTest8.Items.Clear();
                cmbStopBitsTest9.Items.Clear();
              
                cmbStopBitsScanner.Items.Clear();
                Array arrStopBits = Enum.GetValues(typeof(StopBits));
                for (int j = 0; j < arrStopBits.Length; j++)
                {
                    object stopObj = arrStopBits.GetValue(j);

                    cmbStopBitsTest1.Items.Add(stopObj);
                    cmbStopBitsTest2.Items.Add(stopObj);
                    cmbStopBitsTest4.Items.Add(stopObj);
                    cmbStopBitsTest5.Items.Add(stopObj);
                    cmbStopBitsTest6.Items.Add(stopObj);
                    cmbStopBitsTest7.Items.Add(stopObj);
                    cmbStopBitsTest8.Items.Add(stopObj);
                    cmbStopBitsTest9.Items.Add(stopObj);
                 
                    cmbStopBitsScanner.Items.Add(stopObj);
                    if (j > 0)
                    {
                        cmbStopBitsTest1.SelectedIndex = 1;
                        cmbStopBitsTest2.SelectedIndex = 1;
                        cmbStopBitsTest4.SelectedIndex = 1;
                        cmbStopBitsTest5.SelectedIndex = 1;
                        cmbStopBitsTest6.SelectedIndex = 1;
                        cmbStopBitsTest7.SelectedIndex = 1;
                        cmbStopBitsTest8.SelectedIndex = 1;
                        cmbStopBitsTest9.SelectedIndex = 1;
                   
                        cmbStopBitsScanner.SelectedIndex = 1;
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void SerialSet_Load(object sender, EventArgs e)
        {
            //debug("load");
            /*
            try
            {
                #region Port
                string[] ports = SerialPort.GetPortNames();
                for (int i = 1; i <= FuncInline.ComSMDCount; i++)
                {
                    ((ComboBox)Controls.Find("cmbPortTest" + i, true)[0]).Items.Clear();
                    ((ComboBox)Controls.Find("cmbPortTest" + i, true)[0]).Items.AddRange(ports);
                    ((ComboBox)Controls.Find("cmbPortTest" + i, true)[0]).SelectedIndex = 0;
                }
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                {
                    ((ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.Clear();
                    ((ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.AddRange(ports);
                    ((ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).SelectedIndex = 0;
                }
                #endregion

                #region baud rate
                for (int i = 1; i <= FuncInline.ComSMDCount; i++)
                {
                    ((ComboBox)Controls.Find("cmbBaudTest" + i, true)[0]).Items.Clear();
                }
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                {
                    ((ComboBox)Controls.Find("cmbBaud" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.Clear();
                }
                Array arrBaud = Enum.GetValues(typeof(EnumBaudRate));
                for (int j = 0; j < arrBaud.Length; j++)
                {
                    for (int i = 1; i <= FuncInline.ComSMDCount; i++)
                    {
                        ((ComboBox)Controls.Find("cmbBaudTest" + i, true)[0]).Items.Add(arrBaud.GetValue(j).ToString().Replace("Baud", ""));
                        ((ComboBox)Controls.Find("cmbBaudTest" + i, true)[0]).SelectedIndex = 0;
                    }
                    for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                    {
                        ((ComboBox)Controls.Find("cmbBaud" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.Add(arrBaud.GetValue(j).ToString().Replace("Baud", ""));
                        ((ComboBox)Controls.Find("cmbBaud" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).SelectedIndex = 0;
                    }
                }
                #endregion
                
                #region parity
                for (int i = 1; i <= FuncInline.ComSMDCount; i++)
                {
                    ((ComboBox)Controls.Find("cmbParityTest" + i, true)[0]).Items.Clear();
                }
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                {
                    ((ComboBox)Controls.Find("cmbParity" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.Clear();
                }

                Array arrParity = Enum.GetValues(typeof(Parity));
                for (int j = 0; j < arrParity.Length; j++)
                {
                    for (int i = 1; i <= FuncInline.ComSMDCount; i++)
                    {
                        ((ComboBox)Controls.Find("cmbParityTest" + i, true)[0]).Items.Add(arrParity.GetValue(j));
                        ((ComboBox)Controls.Find("cmbParityTest" + i, true)[0]).SelectedIndex = 0;
                    }
                    for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                    {
                        ((ComboBox)Controls.Find("cmbParity" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.Add(arrParity.GetValue(j));
                        ((ComboBox)Controls.Find("cmbParity" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).SelectedIndex = 0;
                    }
                }
                #endregion

                #region Stop bit
                for (int i = 1; i <= FuncInline.ComSMDCount; i++)
                {
                    ((ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0]).Items.Clear();
                }
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                {
                    ((ComboBox)Controls.Find("cmbStopBits" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.Clear();
                }
                Array arrStopBits = Enum.GetValues(typeof(StopBits));
                for (int j = 0; j < arrStopBits.Length; j++)
                {
                    for (int i = 1; i <= FuncInline.ComSMDCount; i++)
                    {
                        ((ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0]).Items.Add(arrStopBits.GetValue(j));
                        if (j > 0)
                        {
                            ((ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0]).SelectedIndex = 1;
                        }
                    }
                    for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                    {
                        ((ComboBox)Controls.Find("cmbStopBits" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.Add(arrStopBits.GetValue(j));
                        if (j > 0)
                        {
                            ((ComboBox)Controls.Find("cmbStopBits" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).SelectedIndex = 1;
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            //*/
        }

        private void SerialSet_Shown(object sender, EventArgs e)
        {
            //debug("shown");
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            valueChanged = true;
        }

        private void timerChange_Tick(object sender, EventArgs e)
        {
            //timerChange.Enabled = false;

            #region 창 떠날 때 저장 확인
            if (valueChanged &&
                beforeMain == FuncInline.enumTabMain.Machine &&
                beforeMachine == FuncInline.enumTabMachine.SerialSet &&
                (FuncInline.TabMain != FuncInline.enumTabMain.Machine ||
                        FuncInline.TabMachine != FuncInline.enumTabMachine.SerialSet))
            {
                FuncInline.TabMain = FuncInline.enumTabMain.Machine;
                FuncInline.TabMachine = FuncInline.enumTabMachine.SerialSet;

                valueChanged = false;
                if (FuncWin.MessageBoxOK("Serial Port Setting changed. Save?"))
                {
                    ApplyAllValue();
                    Func.SavePortIni();
                }
            }
            beforeMain = FuncInline.TabMain;
            beforeMachine = FuncInline.TabMachine;
            #endregion

            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    timerChange.Enabled = true;
            //}
        }
    }
}
