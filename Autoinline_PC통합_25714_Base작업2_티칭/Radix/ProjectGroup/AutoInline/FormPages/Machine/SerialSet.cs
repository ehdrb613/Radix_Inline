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
                Util.SetComboIndex(ref cmbPortTest1, "COM" + FuncInline.PortTest[0]);
                Util.SetComboIndex(ref cmbBaudTest1, FuncInline.BaudTest[0].ToString());
                Util.SetComboIndex(ref cmbParityTest1, FuncInline.ParityTest[0].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest1, FuncInline.StopBitsTest[0].ToString());

                Util.SetComboIndex(ref cmbPortTest2, "COM" + FuncInline.PortTest[1]);
                Util.SetComboIndex(ref cmbBaudTest2, FuncInline.BaudTest[1].ToString());
                Util.SetComboIndex(ref cmbParityTest2, FuncInline.ParityTest[1].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest2, FuncInline.StopBitsTest[1].ToString());

                Util.SetComboIndex(ref cmbPortTest3, "COM" + FuncInline.PortTest[2]);
                Util.SetComboIndex(ref cmbBaudTest3, FuncInline.BaudTest[2].ToString());
                Util.SetComboIndex(ref cmbParityTest3, FuncInline.ParityTest[2].ToString());
                Util.SetComboIndex(ref cmbStopBitsTest3, FuncInline.StopBitsTest[2].ToString());


                Util.SetComboIndex(ref cmbPortIn_NgShuttle, "COM" + FuncInline.PortPMC[0].ToString());
                Util.SetComboIndex(ref cmbBaudIn_NgShuttle, FuncInline.BaudPMC[0].ToString());
                Util.SetComboIndex(ref cmbParityIn_NgShuttle, FuncInline.ParityPMC[0].ToString());
                Util.SetComboIndex(ref cmbStopBitsIn_NgShuttle, FuncInline.StopBitsPMC[0].ToString());

                Util.SetComboIndex(ref cmbPortOutShuttle_Passline1, "COM" + FuncInline.PortPMC[1].ToString());
                Util.SetComboIndex(ref cmbBaudOutShuttle_Passline1, FuncInline.BaudPMC[1].ToString());
                Util.SetComboIndex(ref cmbParityOutShuttle_Passline1, FuncInline.ParityPMC[1].ToString());
                Util.SetComboIndex(ref cmbStopBitsOutShuttle_Passline1, FuncInline.StopBitsPMC[1].ToString());

                Util.SetComboIndex(ref cmbPortPassline2_Passline3, "COM" + FuncInline.PortPMC[2].ToString());
                Util.SetComboIndex(ref cmbBaudPassline2_Passline3, FuncInline.BaudPMC[2].ToString());
                Util.SetComboIndex(ref cmbParityPassline2_Passline3, FuncInline.ParityPMC[2].ToString());
                Util.SetComboIndex(ref cmbStopBitsPassline2_Passline3, FuncInline.StopBitsPMC[2].ToString());

                Util.SetComboIndex(ref cmbPortLift1_Lift2, "COM" + FuncInline.PortPMC[3].ToString());
                Util.SetComboIndex(ref cmbBaudLift1_Lift2, FuncInline.BaudPMC[3].ToString());
                Util.SetComboIndex(ref cmbParityLift1_Lift2, FuncInline.ParityPMC[3].ToString());
                Util.SetComboIndex(ref cmbStopBitsLift1_Lift2, FuncInline.StopBitsPMC[3].ToString());

                Util.SetComboIndex(ref cmbPortIn_OutInverter, "COM" + FuncInline.PortPMC[4].ToString());
                Util.SetComboIndex(ref cmbBaudIn_OutInverter, FuncInline.BaudPMC[4].ToString());
                Util.SetComboIndex(ref cmbParityIn_OutInverter, FuncInline.ParityPMC[4].ToString());
                Util.SetComboIndex(ref cmbStopBitsIn_OutInverter, FuncInline.StopBitsPMC[4].ToString());

                Util.SetComboIndex(ref cmbPortJigBuffer_LinkConveyor, "COM" + FuncInline.PortPMC[5].ToString());
                Util.SetComboIndex(ref cmbBaudJigBuffer_LinkConveyor, FuncInline.BaudPMC[5].ToString());
                Util.SetComboIndex(ref cmbParityJigBuffer_LinkConveyor, FuncInline.ParityPMC[5].ToString());
                Util.SetComboIndex(ref cmbStopBitsJigBuffer_LinkConveyor, FuncInline.StopBitsPMC[5].ToString());

                Util.SetComboIndex(ref cmbPortIn_NgPickup, "COM" + FuncInline.PortPMC[6].ToString());
                Util.SetComboIndex(ref cmbBaudIn_NgPickup, FuncInline.BaudPMC[6].ToString());
                Util.SetComboIndex(ref cmbParityIn_NgPickup, FuncInline.ParityPMC[6].ToString());
                Util.SetComboIndex(ref cmbStopBitsIn_NgPickup, FuncInline.StopBitsPMC[6].ToString());

                Util.SetComboIndex(ref cmbPortOutPickup, "COM" + FuncInline.PortPMC[7].ToString());
                Util.SetComboIndex(ref cmbBaudOutPickup, FuncInline.BaudPMC[7].ToString());
                Util.SetComboIndex(ref cmbParityOutPickup, FuncInline.ParityPMC[7].ToString());
                Util.SetComboIndex(ref cmbStopBitsOutPickup, FuncInline.StopBitsPMC[7].ToString());


                Util.SetComboIndex(ref cmbPortTemperature, "COM" + FuncInline.PortTemperature.ToString());
                Util.SetComboIndex(ref cmbBaudTemperature, FuncInline.BaudTemperature.ToString());
                Util.SetComboIndex(ref cmbParityTemperature, FuncInline.ParityTemperature.ToString());
                Util.SetComboIndex(ref cmbStopBitsTemperature, FuncInline.StopBitsTemperature.ToString());

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

                FuncInline.PortPMC[0] = int.Parse(cmbPortIn_NgShuttle.Text.Replace("COM", ""));
                int.TryParse(cmbBaudIn_NgShuttle.Text, out FuncInline.BaudPMC[0]);
                FuncInline.ParityPMC[0] = (Parity)cmbParityIn_NgShuttle.SelectedIndex;
                FuncInline.StopBitsPMC[0] = (StopBits)cmbStopBitsIn_NgShuttle.SelectedIndex;

                FuncInline.PortPMC[1] = int.Parse(cmbPortOutShuttle_Passline1.Text.Replace("COM", ""));
                int.TryParse(cmbBaudOutShuttle_Passline1.Text, out FuncInline.BaudPMC[1]);
                FuncInline.ParityPMC[1] = (Parity)cmbParityOutShuttle_Passline1.SelectedIndex;
                FuncInline.StopBitsPMC[1] = (StopBits)cmbStopBitsOutShuttle_Passline1.SelectedIndex;


                FuncInline.PortTemperature = int.Parse(cmbPortPassline2_Passline3.Text.Replace("COM", ""));
                int.TryParse(cmbBaudPassline2_Passline3.Text, out FuncInline.BaudTemperature);
                FuncInline.ParityTemperature = (Parity)cmbParityPassline2_Passline3.SelectedIndex;
                FuncInline.StopBitsTemperature = (StopBits)cmbStopBitsPassline2_Passline3.SelectedIndex;

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

                cmbPortIn_NgShuttle.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortIn_NgShuttle.Items.AddRange(ports);
                    cmbPortIn_NgShuttle.SelectedIndex = 0;
                }

                cmbPortOutShuttle_Passline1.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortOutShuttle_Passline1.Items.AddRange(ports);
                    cmbPortOutShuttle_Passline1.SelectedIndex = 0;
                }
                cmbPortPassline2_Passline3.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortPassline2_Passline3.Items.AddRange(ports);
                    cmbPortPassline2_Passline3.SelectedIndex = 0;
                }
                cmbPortLift1_Lift2.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortLift1_Lift2.Items.AddRange(ports);
                    cmbPortLift1_Lift2.SelectedIndex = 0;
                }
                cmbPortIn_OutInverter.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortIn_OutInverter.Items.AddRange(ports);
                    cmbPortIn_OutInverter.SelectedIndex = 0;
                }
                cmbPortJigBuffer_LinkConveyor.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortJigBuffer_LinkConveyor.Items.AddRange(ports);
                    cmbPortJigBuffer_LinkConveyor.SelectedIndex = 0;
                }
                cmbPortIn_NgPickup.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortIn_NgPickup.Items.AddRange(ports);
                    cmbPortIn_NgPickup.SelectedIndex = 0;
                }
                cmbPortOutPickup.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortOutPickup.Items.AddRange(ports);
                    cmbPortOutPickup.SelectedIndex = 0;
                }
                cmbPortTemperature.Items.Clear();
                if (ports.Length > 0)
                {
                    cmbPortTemperature.Items.AddRange(ports);
                    cmbPortTemperature.SelectedIndex = 0;
                }
                #endregion

                #region baud rate
                cmbBaudTest1.Items.Clear();
                cmbBaudTest2.Items.Clear();
                cmbBaudIn_NgShuttle.Items.Clear();
                cmbBaudOutShuttle_Passline1.Items.Clear();
                cmbBaudPassline2_Passline3.Items.Clear();
                cmbBaudLift1_Lift2.Items.Clear();
                cmbBaudIn_OutInverter.Items.Clear();
                cmbBaudJigBuffer_LinkConveyor.Items.Clear();
                cmbBaudIn_NgPickup.Items.Clear();
                cmbBaudOutPickup.Items.Clear();
                cmbBaudTemperature.Items.Clear();
                Array arrBaud = Enum.GetValues(typeof(EnumBaudRate));
                for (int j = 0; j < arrBaud.Length; j++)
                {
                    string baudStr = arrBaud.GetValue(j).ToString().Replace("Baud", "");

                    cmbBaudTest1.Items.Add(baudStr);
                    cmbBaudTest1.SelectedIndex = 0;
                    cmbBaudTest2.Items.Add(baudStr);
                    cmbBaudTest2.SelectedIndex = 0;
                    cmbBaudIn_NgShuttle.Items.Add(baudStr);
                    cmbBaudIn_NgShuttle.SelectedIndex = 0;
                    cmbBaudOutShuttle_Passline1.Items.Add(baudStr);
                    cmbBaudOutShuttle_Passline1.SelectedIndex = 0;
                    cmbBaudPassline2_Passline3.Items.Add(baudStr);
                    cmbBaudPassline2_Passline3.SelectedIndex = 0;
                    cmbBaudLift1_Lift2.Items.Add(baudStr);
                    cmbBaudLift1_Lift2.SelectedIndex = 0;
                    cmbBaudIn_OutInverter.Items.Add(baudStr);
                    cmbBaudIn_OutInverter.SelectedIndex = 0;
                    cmbBaudJigBuffer_LinkConveyor.Items.Add(baudStr);
                    cmbBaudJigBuffer_LinkConveyor.SelectedIndex = 0;
                    cmbBaudIn_NgPickup.Items.Add(baudStr);
                    cmbBaudIn_NgPickup.SelectedIndex = 0;
                    cmbBaudOutPickup.Items.Add(baudStr);
                    cmbBaudOutPickup.SelectedIndex = 0;
                    cmbBaudTemperature.Items.Add(baudStr);
                    cmbBaudTemperature.SelectedIndex = 0;
                }
                #endregion

                #region parity
                cmbParityTest1.Items.Clear();
                cmbParityTest2.Items.Clear();
                cmbParityIn_NgShuttle.Items.Clear();
                cmbParityOutShuttle_Passline1.Items.Clear();
                cmbParityPassline2_Passline3.Items.Clear();
                cmbParityLift1_Lift2.Items.Clear();
                cmbParityIn_OutInverter.Items.Clear();
                cmbParityJigBuffer_LinkConveyor.Items.Clear();
                cmbParityIn_NgPickup.Items.Clear();
                cmbParityOutPickup.Items.Clear();
                cmbParityTemperature.Items.Clear();
                Array arrParity = Enum.GetValues(typeof(Parity));
                for (int j = 0; j < arrParity.Length; j++)
                {
                    object parityObj = arrParity.GetValue(j);

                    cmbParityTest1.Items.Add(parityObj);
                    cmbParityTest1.SelectedIndex = 0;
                    cmbParityTest2.Items.Add(parityObj);
                    cmbParityTest2.SelectedIndex = 0;
                    cmbParityIn_NgShuttle.Items.Add(parityObj);
                    cmbParityIn_NgShuttle.SelectedIndex = 0;
                    cmbParityOutShuttle_Passline1.Items.Add(parityObj);
                    cmbParityOutShuttle_Passline1.SelectedIndex = 0;
                    cmbParityPassline2_Passline3.Items.Add(parityObj);
                    cmbParityPassline2_Passline3.SelectedIndex = 0;
                    cmbParityLift1_Lift2.Items.Add(parityObj);
                    cmbParityLift1_Lift2.SelectedIndex = 0;
                    cmbParityIn_OutInverter.Items.Add(parityObj);
                    cmbParityIn_OutInverter.SelectedIndex = 0;
                    cmbParityJigBuffer_LinkConveyor.Items.Add(parityObj);
                    cmbParityJigBuffer_LinkConveyor.SelectedIndex = 0;
                    cmbParityIn_NgPickup.Items.Add(parityObj);
                    cmbParityIn_NgPickup.SelectedIndex = 0;
                    cmbParityOutPickup.Items.Add(parityObj);
                    cmbParityOutPickup.SelectedIndex = 0;
                    cmbParityTemperature.Items.Add(parityObj);
                    cmbParityTemperature.SelectedIndex = 0;
                }
                #endregion

                #region Stop bit
                cmbStopBitsTest1.Items.Clear();
                cmbStopBitsTest2.Items.Clear();
                cmbStopBitsIn_NgShuttle.Items.Clear();
                cmbStopBitsOutShuttle_Passline1.Items.Clear();
                cmbStopBitsPassline2_Passline3.Items.Clear();
                cmbStopBitsLift1_Lift2.Items.Clear();
                cmbStopBitsIn_OutInverter.Items.Clear();
                cmbStopBitsJigBuffer_LinkConveyor.Items.Clear();
                cmbStopBitsIn_NgPickup.Items.Clear();
                cmbStopBitsOutPickup.Items.Clear();
                cmbStopBitsTemperature.Items.Clear();
                Array arrStopBits = Enum.GetValues(typeof(StopBits));
                for (int j = 0; j < arrStopBits.Length; j++)
                {
                    object stopObj = arrStopBits.GetValue(j);

                    cmbStopBitsTest1.Items.Add(stopObj);
                    cmbStopBitsTest2.Items.Add(stopObj);
                    cmbStopBitsIn_NgShuttle.Items.Add(stopObj);
                    cmbStopBitsOutShuttle_Passline1.Items.Add(stopObj);
                    cmbStopBitsPassline2_Passline3.Items.Add(stopObj);
                    cmbStopBitsLift1_Lift2.Items.Add(stopObj);
                    cmbStopBitsIn_OutInverter.Items.Add(stopObj);
                    cmbStopBitsJigBuffer_LinkConveyor.Items.Add(stopObj);
                    cmbStopBitsIn_NgPickup.Items.Add(stopObj);
                    cmbStopBitsOutPickup.Items.Add(stopObj);
                    cmbStopBitsTemperature.Items.Add(stopObj);
                    if (j > 0)
                    {
                        cmbStopBitsTest1.SelectedIndex = 1;
                        cmbStopBitsTest2.SelectedIndex = 1;
                        cmbStopBitsIn_NgShuttle.SelectedIndex = 1;
                        cmbStopBitsOutShuttle_Passline1.SelectedIndex = 1;
                        cmbStopBitsPassline2_Passline3.SelectedIndex = 1;
                        cmbStopBitsLift1_Lift2.SelectedIndex = 1;
                        cmbStopBitsIn_OutInverter.SelectedIndex = 1;
                        cmbStopBitsJigBuffer_LinkConveyor.SelectedIndex = 1;
                        cmbStopBitsIn_NgPickup.SelectedIndex = 1;
                        cmbStopBitsOutPickup.SelectedIndex = 1;
                        cmbStopBitsTemperature.SelectedIndex = 1;
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
