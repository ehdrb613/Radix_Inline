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

    public partial class Machine : Form
    {
        Popup.Machine.GeneralTeach ucSc1 = new Popup.Machine.GeneralTeach();
        Popup.Machine.TowerLamp ucSc2 = new Popup.Machine.TowerLamp();
        Popup.Machine.SerialSet ucSc3 = new Popup.Machine.SerialSet();

        #region 로컬변수
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;
        #endregion


        #region 초기화 관련
        public Machine()
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

            ucSc1.LoadAllValue();
            ucSc2.LoadAllValue();
            ucSc3.LoadAllValue();

            #region 화면 제어용 쓰레드 타이머 시작            
            TimerCallback CallBackUI = new TimerCallback(TimerUI);
            timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);
            #endregion

            this.BringToFront();
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

        private void Setting_Load(object sender, EventArgs e)
        {
            GlobalVar.Machine_Machine = true;
            GlobalVar.Machine_TowerLamp = false;
            GlobalVar.Machine_SerialSet = false;

            machine_Panel.Controls.Clear();
            machine_Panel.Controls.Add(ucSc1);
            try
            {
                //cmbLoadcellPort.Items.AddRange(SerialPort.GetPortNames());
                //cmbLoadcellBaud.SelectedIndex = 3;

                //if (cmbLoadcellPort.Items.Count > 1)
                //{
                //    cmbLoadcellPort.SelectedIndex = 0;  // 컴포트 정보가 없을 경우 컴포트의 1번째를 사용
                //}
            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
            }
        }

        public void LoadAllValue()
        {
            //frmGeneral.LoadAllValue();
            //frmTowerLamp.LoadAllValue();
            //frmSerialSet.LoadAllValue();
            //frmPinBlock.LoadAllValue();
            FuncIni.LoadMachinenIni();
            ucSc1.LoadAllValue();
            ucSc2.LoadAllValue();
            ucSc3.LoadAllValue();
            
        }
        #endregion

        #region 타이머
        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            if (GlobalVar.TabMain != enumTabMain.Machine) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
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
                this.InvokeAndClose((MethodInvoker)delegate
                {
                    #region 관리자 여부에 따라 컨트롤 보이기/숨기기
                    /*
                    //tbAppName.Enabled = GlobalVar.PwdPass;
                    //numMachineNum.Enabled = GlobalVar.PwdPass;
                    pbSave.Visible = GlobalVar.PwdPass;
                    //btnTower0_0.Enabled = GlobalVar.PwdPass;
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            Controls.Find("btnTower" + i.ToString() + "_" + j.ToString(), true)[0].Enabled = GlobalVar.PwdPass;
                            Controls.Find("btnBlink" + i.ToString() + "_" + j.ToString(), true)[0].Enabled = GlobalVar.PwdPass;
                        }
                    }
                    //*/

                
                    #endregion
                });

                timerDoing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }
        }


        #endregion

        #region 버튼 이벤트
        private void btnApply_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change setting while system is running!");
                return;
            }        

            if (GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Apply Machine setting?"))
                {
                    ucSc1.ApplyAllValue();
                }
            }
            if (GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Apply Tower Lamp setting?"))
                {
                    ucSc2.ApplyAllValue();
                }
            }
            if (GlobalVar.Machine_SerialSet)
            {
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
                for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                {
                    ComboBox cmb = (ComboBox)Controls.Find("cmbStopBits" + ((enumPMCMotion)i).ToString(), true)[0];
                    if ((cmb).Text == "None")
                    {
                        cmb.Focus();
                        FuncWin.TopMessageBox("StopBits None not supported");
                        return;
                    }
                }
                if (FuncWin.MessageBoxOK("Apply port setting?"))
                {
                    ucSc3.ApplyAllValue();
                }
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }
       
            if (GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Cancel Machine setting?"))
                {
                    ucSc1.LoadAllValue();
                }
            }
            if (GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Cancel Tower Lamp setting?"))
                {
                    ucSc2.LoadAllValue();
                }
            }
            if (GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Cancel port setting?"))
                {
                    ucSc3.LoadAllValue();
                }
            }
        }
        private void btnLoad_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Load Machine setting?"))
                {
                    FuncIni.LoadMachinenIni();
                    ucSc1.LoadAllValue();
                }
            }
            if (GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Load Tower Lamp setting?"))
                {
                    FuncIni.LoadMachinenIni();
                    ucSc2.LoadAllValue();
                }
            }
            if (GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Load port setting?"))
                {
                    FuncIni.LoadPortIni();
                    ucSc3.LoadAllValue();
                }
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Machine setting?"))
                {
                    ucSc1.ApplyAllValue();
                    FuncIni.SaveMachineIni();
                }
            }
            if (GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Tower Lamp setting?"))
                {
                    ucSc2.ApplyAllValue();
                    FuncIni.SaveMachineIni();
                }
            }
            if (GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Save port setting?"))
                {
                    ucSc3.ApplyAllValue();
                    FuncIni.SavePortIni();
                }
            }
        }
        private void btnMachine_Setting_Click(object sender, EventArgs e)
        {
            GlobalVar.Machine_Machine = true;
            GlobalVar.Machine_TowerLamp = false;
            GlobalVar.Machine_SerialSet = false;

            machine_Panel.Controls.Clear();
            machine_Panel.Controls.Add(ucSc1);
        }
        private void btnTower_Lamp_Setting_Click(object sender, EventArgs e)
        {
            GlobalVar.Machine_Machine = false;
            GlobalVar.Machine_TowerLamp = true;
            GlobalVar.Machine_SerialSet = false;

            machine_Panel.Controls.Clear();
            machine_Panel.Controls.Add(ucSc2);
        }
        private void btnSerial_Setting_Click(object sender, EventArgs e)
        {
            GlobalVar.Machine_Machine = false;
            GlobalVar.Machine_TowerLamp = false;
            GlobalVar.Machine_SerialSet = true;

            machine_Panel.Controls.Clear();
            machine_Panel.Controls.Add(ucSc3);
        }
        

        private void pbLoad_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Load Machine setting?"))
                {
                    FuncIni.LoadMachinenIni();
                    ucSc1.LoadAllValue();
                   
                }
            }
            if (GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Load Tower Lamp setting?"))
                {
                    FuncIni.LoadMachinenIni();
                    ucSc2.LoadAllValue();
                }
            }
            if (GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Load port setting?"))
                {
                    FuncIni.LoadPortIni();
                    ucSc3.LoadAllValue();
                }
            }
        }

        private void pbSave_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Machine setting?"))
                {
                    ucSc1.ApplyAllValue();
                    FuncIni.SaveMachineIni();
                    ucSc1.LoadAllValue();
                }
            }
            if (GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Tower Lamp setting?"))
                {
                    ucSc2.ApplyAllValue();
                    FuncIni.SaveMachineIni();
                }
            }
            if (GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Save port setting?"))
                {
                    ucSc3.ApplyAllValue();
                    FuncIni.SavePortIni();
                }
            }
        }

        private void pbApply_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change setting while system is running!");
                return;
            }

            if (GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Apply Machine setting?"))
                {
                    ucSc1.ApplyAllValue();
                    //ucSc1.LoadAllValue();
                }
            }
            if (GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Apply Tower Lamp setting?"))
                {
                    ucSc2.ApplyAllValue();
                }
            }
            if (GlobalVar.Machine_SerialSet)
            {
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
                for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                {
                    ComboBox cmb = (ComboBox)Controls.Find("cmbStopBits" + ((enumPMCMotion)i).ToString(), true)[0];
                    if ((cmb).Text == "None")
                    {
                        cmb.Focus();
                        FuncWin.TopMessageBox("StopBits None not supported");
                        return;
                    }
                }
                if (FuncWin.MessageBoxOK("Apply port setting?"))
                {
                    ucSc3.ApplyAllValue();
                }
            }
        }

        private void pbCancel_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Cancel Machine setting?"))
                {
                    ucSc1.LoadAllValue();
                }
            }
            if (GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Cancel Tower Lamp setting?"))
                {
                    ucSc2.LoadAllValue();
                }
            }
            if (GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Cancel port setting?"))
                {
                    ucSc3.LoadAllValue();
                }
            }
        }
        #endregion

        private void Machine_Move(object sender, EventArgs e)
        {
            FuncLog.WriteLog("");
        }
    }
}
