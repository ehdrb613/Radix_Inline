using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가

namespace Radix
{
    /*
     * Setting.cs : 각종 옵션 설정 관리
     */

    public partial class ComSet : Form
    {
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;

        public ComSet()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            Util.Debug(str);
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;
            GlobalVar.PwdPass = true;

            //tbModel.Text = GlobalVar.ModelName;
            LoadAllValue();


            #region 화면 제어용 쓰레드 타이머 시작
            //*
            TimerCallback CallBackUI = new TimerCallback(TimerUI);
            timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);
            //*/
            #endregion


            this.BringToFront();
        }

        private void LoadAllValue()
        {
            try
            {
                for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
                {
                    ComboBox portTest = (ComboBox)Controls.Find("cmbPortTest" + i, true)[0];
                    Util.SetComboIndex(ref portTest, "COM" + FuncInline.PortTest[i - 1]);

                    ComboBox baudTest = (ComboBox)Controls.Find("cmbBaudTest" + i, true)[0];
                    Util.SetComboIndex(ref baudTest, FuncInline.BaudTest[i - 1].ToString());

                    ComboBox parityTest = (ComboBox)Controls.Find("cmbParityTest" + i, true)[0];
                    Util.SetComboIndex(ref parityTest, FuncInline.ParityTest[i - 1].ToString());

                    ComboBox stopTest = (ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0];
                    Util.SetComboIndex(ref stopTest, FuncInline.StopBitsTest[i - 1].ToString());
                }
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                {
                    debug("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString());
                    ComboBox portSite = (ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                    Util.SetComboIndex(ref portSite, "COM" + FuncInline.PortPMC[i].ToString());

                    ComboBox baudSite = (ComboBox)Controls.Find("cmbBaud" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                    Util.SetComboIndex(ref baudSite, FuncInline.BaudPMC[i].ToString());

                    ComboBox paritySite = (ComboBox)Controls.Find("cmbParity" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                    Util.SetComboIndex(ref paritySite, FuncInline.ParityPMC[i].ToString());

                    ComboBox stopSite = (ComboBox)Controls.Find("cmbStopBits" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                    Util.SetComboIndex(ref stopSite, FuncInline.StopBitsPMC[i].ToString());
                }

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

        }

        private void ApplyAllValue()
        {
            try
            {
                for (int i = 1; i <= 3; i++)
                {
                    ComboBox portTest = (ComboBox)Controls.Find("cmbPortTest" + i, true)[0];
                    FuncInline.PortTest[i - 1] = int.Parse(portTest.Text.Replace("COM", ""));

                    ComboBox baudTest = (ComboBox)Controls.Find("cmbBaudTest" + i, true)[0];
                    int.TryParse(baudTest.Text, out FuncInline.BaudTest[i - 1]);

                    ComboBox parityTest = (ComboBox)Controls.Find("cmbParityTest" + i, true)[0];
                    FuncInline.ParityTest[i - 1] = (Parity)parityTest.SelectedIndex;

                    ComboBox stopBitsTest = (ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0];
                    FuncInline.StopBitsTest[i - 1] = (StopBits)stopBitsTest.SelectedIndex;
                }
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                {
                    ComboBox portSite = (ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                    FuncInline.PortPMC[i] = int.Parse(portSite.Text.Replace("COM", ""));

                    ComboBox baudSite = (ComboBox)Controls.Find("cmbBaud" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                    int.TryParse(baudSite.Text, out FuncInline.BaudPMC[i]);

                    ComboBox paritySite = (ComboBox)Controls.Find("cmbParity" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                    FuncInline.ParityPMC[i] = (Parity)paritySite.SelectedIndex;

                    ComboBox stopBitsSite = (ComboBox)Controls.Find("cmbStopBits" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                    FuncInline.StopBitsPMC[i] = (StopBits)stopBitsSite.SelectedIndex;
                }

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ///*
            this.BringToFront();
            if (FuncWin.MessageBoxOK(" Apply All Value?"))
            {
                //GlobalVar.ModelName = tbModel.Text; // YJ20210923 모델 적용시 모델명이 변경되지 않아서 추가함
                ApplyAllValue();
            }
            //*/
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //*
            this.BringToFront();
            if (FuncWin.MessageBoxOK(" Cancel All Value?"))
            {
                LoadAllValue();
            }
            //*/
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (FuncWin.MessageBoxOK(" Load All Values?"))
            {
                Func.LoadAllIni();

                LoadAllValue();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //*
            if (FuncWin.MessageBoxOK(" Save All Values?"))
            {
                //GlobalVar.ModelName = tbModel.Text;
                ApplyAllValue();

                Func.SaveAllIni();

                //if (!cmbModelList.Items.Contains(tbModel.Text))
                //{
                //    cmbModelList.Items.Add(tbModel.Text);
                //    cmbModelList.Text = tbModel.Text;
                //}
            }
            //*/
        }




        private void Setting_FormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalVar.SettingClose = true;
            timerUI.Dispose();
            GlobalVar.dlgOpened = false;
            if (this.Parent != null)
            {
                try
                {
                    this.Parent.BringToFront();
                }
                catch
                { }
            }
        }

        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Com) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                return;
            }
            try
            {
                if (timerDoing)
                {
                    return;
                }

                timerDoing = true;

                /* 화면 변경 timer */
                this.Invoke(new MethodInvoker(delegate ()
                {

                    #region 관리자 여부에 따라 컨트롤 보이기/숨기기
                    //tbAppName.Enabled = GlobalVar.PwdPass;
                    //numMachineNum.Enabled = GlobalVar.PwdPass;
                    //btnTower0_0.Enabled = GlobalVar.PwdPass;
                    #endregion


                }));

                timerDoing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }


        }


        private void Setting_Load(object sender, EventArgs e)
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
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
                {
                    ((ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.Clear();
                    ((ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).Items.AddRange(ports);
                    ((ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0]).SelectedIndex = 0;
                }
                #endregion

                #region baud rate
                for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
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
                    for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
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
                for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
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
                    for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
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
                for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
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
                    for (int i = 1; i <= GlobalVar.AutoInline_ComSMDCount; i++)
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
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

        }

        private void pbCancel_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }
            if (FuncWin.MessageBoxOK("Cancel port setting?"))
            {
                LoadAllValue();
            }
        }

        private bool CheckPortDupe() // 전체 포트 설정에 겹치는 부분이 있는가? 경고만 하고 적용은 한다.
        {
            for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
            {
                ComboBox portSite = (ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                for (int j = i + 1; j < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; j++)
                {
                    ComboBox portSite2 = (ComboBox)Controls.Find("cmbPort" + ((FuncInline.enumPMCMotion)j).ToString(), true)[0];
                    if (portSite.Text == portSite2.Text)
                    {
                        debug("dupe site " + i + "," + j);
                        return true;
                    }
                }
                for (int k = 1; k <= 3; k++)
                {
                    ComboBox cmb = (ComboBox)Controls.Find("cmbPortTest" + k, true)[0];
                    if (portSite.Text == cmb.Text)
                    {
                        debug("dupe site test " + i + "," + k);
                        return true;
                    }
                }
            }
            for (int i = 1; i <= 3; i++)
            {
                ComboBox cmb = (ComboBox)Controls.Find("cmbPortTest" + i, true)[0];
                for (int j = i + 1; j <= 3; j++)
                {
                    ComboBox cmb2 = (ComboBox)Controls.Find("cmbPortTest" + j, true)[0];
                    if (cmb.Text == cmb2.Text)
                    {
                        debug("dupe test " + i + "," + j);
                        return true;
                    }
                }
            }
            return false;
        }


        private void pbApply_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change setting while system is running!");
                return;
            }

            for (int i = 1; i <= 3; i++)
            {
                ComboBox cmb = (ComboBox)Controls.Find("cmbStopBitsTest" + i, true)[0];
                if ((cmb).Text == "None")
                {
                    cmb.Focus();
                    FuncWin.TopMessageBox("StopBits None not supported");
                    return;
                }
            }
            for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCMotion)).Length; i++)
            {
                ComboBox cmb = (ComboBox)Controls.Find("cmbStopBits" + ((FuncInline.enumPMCMotion)i).ToString(), true)[0];
                if ((cmb).Text == "None")
                {
                    cmb.Focus();
                    FuncWin.TopMessageBox("StopBits None not supported");
                    return;
                }
            }

            #region site에 PCB 들어 있는 경우 변경 금지
            #endregion

            if (FuncWin.MessageBoxOK("Apply port setting?"))
            {
                if (CheckPortDupe() &&
                    FuncWin.MessageBoxOK("Port settings are duplicated. Apply anyway?"))
                {
                    return;
                }
                ApplyAllValue();
                //PMCClass.ReconnectAllPort();
            }
        }

        private void pbSave_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }
            if (FuncWin.MessageBoxOK("Save port setting?"))
            {
                if (CheckPortDupe() &&
                    FuncWin.MessageBoxOK("Port settings are duplicated. Save anyway?"))
                {
                    return;
                }
                ApplyAllValue();
                Func.SavePortIni();
                //PMCClass.ReconnectAllPort();
            }
        }

        private void pbLoad_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }
            if (FuncWin.MessageBoxOK("Load port setting?"))
            {
                Func.LoadPortIni();
                LoadAllValue();
            }
        }
    }
}
