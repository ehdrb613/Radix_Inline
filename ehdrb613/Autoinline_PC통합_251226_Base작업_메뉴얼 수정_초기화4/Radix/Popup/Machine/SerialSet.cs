using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가

namespace Radix.Popup.Machine
{
    public partial class SerialSet : UserControl
    {
        public SerialSet()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            Util.Debug(str);
        }

        public void LoadAllValue()
        {
            try
            {
                //for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
                //{
                //    ComboBox portTest = (ComboBox)Controls.Find("cmbPortTest" + i, true)[0];
                //    Util.SetComboIndex(ref portTest, "COM" + GlobalVar.AutoInline_PortTest[i - 1]);

                //    ComboBox baudTest = (ComboBox)Controls.Find("cmbBaudTest" + i, true)[0];
                //    Util.SetComboIndex(ref baudTest, GlobalVar.AutoInline_BaudTest[i - 1].ToString());

                //    ComboBox parityTest = (ComboBox)Controls.Find("cmbParityTest" + i, true)[0];
                //    Util.SetComboIndex(ref parityTest, GlobalVar.AutoInline_ParityTest[i - 1].ToString());

                //    ComboBox stopTest = (ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0];
                //    Util.SetComboIndex(ref stopTest, GlobalVar.AutoInline_StopBitsTest[i - 1].ToString());
                //}
                //for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                //{
                //    debug("cmbPort" + ((enumPMCMotion)i).ToString());
                //    ComboBox portSite = (ComboBox)Controls.Find("cmbPort" + ((enumPMCMotion)i).ToString(), true)[0];
                //    Util.SetComboIndex(ref portSite, "COM" + GlobalVar.AutoInline_PortPMC[i].ToString());

                //    ComboBox baudSite = (ComboBox)Controls.Find("cmbBaud" + ((enumPMCMotion)i).ToString(), true)[0];
                //    Util.SetComboIndex(ref baudSite, GlobalVar.AutoInline_BaudPMC[i].ToString());

                //    ComboBox paritySite = (ComboBox)Controls.Find("cmbParity" + ((enumPMCMotion)i).ToString(), true)[0];
                //    Util.SetComboIndex(ref paritySite, GlobalVar.AutoInline_ParityPMC[i].ToString());

                //    ComboBox stopSite = (ComboBox)Controls.Find("cmbStopBits" + ((enumPMCMotion)i).ToString(), true)[0];
                //    Util.SetComboIndex(ref stopSite, GlobalVar.AutoInline_StopBitsPMC[i].ToString());
                //}
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        public void ApplyAllValue()
        {
            try
            {
                for (int i = 1; i <= 3; i++)
                {
                    ComboBox portTest = (ComboBox)Controls.Find("cmbPortTest" + i, true)[0];
                    GlobalVar.AutoInline_PortTest[i - 1] = int.Parse(portTest.Text.Replace("COM", ""));

                    ComboBox baudTest = (ComboBox)Controls.Find("cmbBaudTest" + i, true)[0];
                    int.TryParse(baudTest.Text, out GlobalVar.AutoInline_BaudTest[i - 1]);

                    ComboBox parityTest = (ComboBox)Controls.Find("cmbParityTest" + i, true)[0];
                    GlobalVar.AutoInline_ParityTest[i - 1] = (Parity)parityTest.SelectedIndex;

                    ComboBox stopBitsTest = (ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0];
                    GlobalVar.AutoInline_StopBitsTest[i - 1] = (StopBits)stopBitsTest.SelectedIndex;
                }
                for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                {
                    ComboBox portSite = (ComboBox)Controls.Find("cmbPort" + ((enumPMCMotion)i).ToString(), true)[0];
                    GlobalVar.AutoInline_PortPMC[i] = int.Parse(portSite.Text.Replace("COM", ""));

                    ComboBox baudSite = (ComboBox)Controls.Find("cmbBaud" + ((enumPMCMotion)i).ToString(), true)[0];
                    int.TryParse(baudSite.Text, out GlobalVar.AutoInline_BaudPMC[i]);

                    ComboBox paritySite = (ComboBox)Controls.Find("cmbParity" + ((enumPMCMotion)i).ToString(), true)[0];
                    GlobalVar.AutoInline_ParityPMC[i] = (Parity)paritySite.SelectedIndex;

                    ComboBox stopBitsSite = (ComboBox)Controls.Find("cmbStopBits" + ((enumPMCMotion)i).ToString(), true)[0];
                    GlobalVar.AutoInline_StopBitsPMC[i] = (StopBits)stopBitsSite.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void SerialSet_Load(object sender, EventArgs e)
        {          
            try
            {
                #region Port
                string[] ports = SerialPort.GetPortNames();
                for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
                {
                    ((ComboBox)Controls.Find("cmbPortTest" + i, true)[0]).Items.Clear();
                    ((ComboBox)Controls.Find("cmbPortTest" + i, true)[0]).Items.AddRange(ports);
                    ((ComboBox)Controls.Find("cmbPortTest" + i, true)[0]).SelectedIndex = 0;
                }
                for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                {
                    ((ComboBox)Controls.Find("cmbPort" + ((enumPMCMotion)i).ToString(), true)[0]).Items.Clear();
                    ((ComboBox)Controls.Find("cmbPort" + ((enumPMCMotion)i).ToString(), true)[0]).Items.AddRange(ports);
                    ((ComboBox)Controls.Find("cmbPort" + ((enumPMCMotion)i).ToString(), true)[0]).SelectedIndex = 0;
                }
                #endregion

                #region baud rate
                for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
                {
                    ((ComboBox)Controls.Find("cmbBaudTest" + i, true)[0]).Items.Clear();
                }
                for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                {
                    ((ComboBox)Controls.Find("cmbBaud" + ((enumPMCMotion)i).ToString(), true)[0]).Items.Clear();
                }
                Array arrBaud = Enum.GetValues(typeof(EnumBaudRate));
                for (int j = 0; j < arrBaud.Length; j++)
                {
                    for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
                    {
                        ((ComboBox)Controls.Find("cmbBaudTest" + i, true)[0]).Items.Add(arrBaud.GetValue(j).ToString().Replace("Baud", ""));
                        ((ComboBox)Controls.Find("cmbBaudTest" + i, true)[0]).SelectedIndex = 0;
                    }
                    for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                    {
                        ((ComboBox)Controls.Find("cmbBaud" + ((enumPMCMotion)i).ToString(), true)[0]).Items.Add(arrBaud.GetValue(j).ToString().Replace("Baud", ""));
                        ((ComboBox)Controls.Find("cmbBaud" + ((enumPMCMotion)i).ToString(), true)[0]).SelectedIndex = 0;
                    }
                }
                #endregion
                
                #region parity
                for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
                {
                    ((ComboBox)Controls.Find("cmbParityTest" + i, true)[0]).Items.Clear();
                }
                for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                {
                    ((ComboBox)Controls.Find("cmbParity" + ((enumPMCMotion)i).ToString(), true)[0]).Items.Clear();
                }

                Array arrParity = Enum.GetValues(typeof(Parity));
                for (int j = 0; j < arrParity.Length; j++)
                {
                    for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
                    {
                        ((ComboBox)Controls.Find("cmbParityTest" + i, true)[0]).Items.Add(arrParity.GetValue(j));
                        ((ComboBox)Controls.Find("cmbParityTest" + i, true)[0]).SelectedIndex = 0;
                    }
                    for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                    {
                        ((ComboBox)Controls.Find("cmbParity" + ((enumPMCMotion)i).ToString(), true)[0]).Items.Add(arrParity.GetValue(j));
                        ((ComboBox)Controls.Find("cmbParity" + ((enumPMCMotion)i).ToString(), true)[0]).SelectedIndex = 0;
                    }
                }
                #endregion

                #region Stop bit
                for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
                {
                    ((ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0]).Items.Clear();
                }
                for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                {
                    ((ComboBox)Controls.Find("cmbStopBits" + ((enumPMCMotion)i).ToString(), true)[0]).Items.Clear();
                }
                Array arrStopBits = Enum.GetValues(typeof(StopBits));
                for (int j = 0; j < arrStopBits.Length; j++)
                {
                    for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
                    {
                        ((ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0]).Items.Add(arrStopBits.GetValue(j));
                        if (j > 0)
                        {
                            ((ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0]).SelectedIndex = 1;
                        }
                    }
                    for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                    {
                        ((ComboBox)Controls.Find("cmbStopBits" + ((enumPMCMotion)i).ToString(), true)[0]).Items.Add(arrStopBits.GetValue(j));
                        if (j > 0)
                        {
                            ((ComboBox)Controls.Find("cmbStopBits" + ((enumPMCMotion)i).ToString(), true)[0]).SelectedIndex = 1;
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }
    }
}
