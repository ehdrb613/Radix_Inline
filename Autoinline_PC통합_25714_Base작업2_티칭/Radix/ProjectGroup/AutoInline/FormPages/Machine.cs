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

        Popup.Machine.General frmGeneral = new Popup.Machine.General();
        Popup.Machine.TowerLamp frmTowerLamp = new Popup.Machine.TowerLamp();
        Popup.Machine.SerialSet frmSerialSet = new Popup.Machine.SerialSet();
        Popup.Machine.PinBlock frmPinBlock = new Popup.Machine.PinBlock();
        Popup.Machine.TestSite frmTest = new Popup.Machine.TestSite();
        Popup.Machine.IoMonitor frmIOMonitor = new Popup.Machine.IoMonitor();
      
        #region 로컬변수
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;
        private int hiddenTime = 0; // 관리자 버튼 클릭 횟수
        #endregion


        #region 초기화 관련
        public Machine()
        {
            //debug("init");
            InitializeComponent();

            frmGeneral.FormBorderStyle = FormBorderStyle.None;
            frmGeneral.TopMost = true;
            frmGeneral.Dock = DockStyle.Fill;
            frmGeneral.TopLevel = false;
            frmGeneral.LoadAllValue();
            //machine_Panel.Controls.Clear();
            //machine_Panel.Controls.Add(frmGeneral);
            tpMachine.Controls.Clear();
            tpMachine.Controls.Add(frmGeneral);
            frmGeneral.Show();

            frmTowerLamp.FormBorderStyle = FormBorderStyle.None;
            frmTowerLamp.TopMost = true;
            frmTowerLamp.Dock = DockStyle.Fill;
            frmTowerLamp.TopLevel = false;
            frmTowerLamp.LoadAllValue();
            //tower_Panel.Controls.Clear();
            //tower_Panel.Controls.Add(frmTowerLamp);
            tpTowerLamp.Controls.Clear();
            tpTowerLamp.Controls.Add(frmTowerLamp);
            frmTowerLamp.Show();

            frmSerialSet.FormBorderStyle = FormBorderStyle.None;
            frmSerialSet.TopMost = true;
            frmSerialSet.Dock = DockStyle.Fill;
            frmSerialSet.TopLevel = false;
            frmSerialSet.LoadAllValue();
            //serial_Panel.Controls.Clear();
            //serial_Panel.Controls.Add(frmSerialSet);
            tpSerial.Controls.Clear();
            tpSerial.Controls.Add(frmSerialSet);
            frmSerialSet.Show();

            frmPinBlock.FormBorderStyle = FormBorderStyle.None;
            frmPinBlock.TopMost = true;
            frmPinBlock.Dock = DockStyle.Fill;
            frmPinBlock.TopLevel = false;
            frmPinBlock.LoadAllValue();
            //pin_Panel.Controls.Clear();
            //pin_Panel.Controls.Add(frmPinBlock);
            tpPinBlock.Controls.Clear();
            tpPinBlock.Controls.Add(frmPinBlock);
            frmPinBlock.Show();

            frmTest.FormBorderStyle = FormBorderStyle.None;
            frmTest.TopMost = true;
            frmTest.Dock = DockStyle.Fill;
            frmTest.TopLevel = false;
            frmTest.LoadAllValue();
            tpTest.Controls.Clear();
            tpTest.Controls.Add(frmTest);
            frmTest.Show();

            frmIOMonitor.FormBorderStyle = FormBorderStyle.None;
            frmIOMonitor.TopMost = true;
            frmIOMonitor.Dock = DockStyle.Fill;
            frmIOMonitor.TopLevel = false;
            tpIOMonitor.Controls.Clear();
            tpIOMonitor.Controls.Add(frmIOMonitor);
            frmIOMonitor.Show();
            //machine_Panel.BringToFront();


            //frmModuleIO.FormBorderStyle = FormBorderStyle.None;
            //frmModuleIO.TopMost = true;
            //frmModuleIO.Dock = DockStyle.Fill;
            //frmModuleIO.TopLevel = false;
            //tpModuleIO.Controls.Clear();
            //tpModuleIO.Controls.Add(frmModuleIO);
            //frmModuleIO.Show();
            //machine_Panel.BringToFront();

        }

        private void debug(string str)
        {
            Util.Debug("frmMachine : " + str);
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
            //debug("shown");
            GlobalVar.dlgOpened = true;
            //GlobalVar.PwdPass = true;

            #region 화면 제어용 쓰레드 타이머 시작            
            TimerCallback CallBackUI = new TimerCallback(TimerUI);
            timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);
            #endregion

            this.BringToFront();
        }

        private void Setting_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (this.Parent != null)
            //{
            try
            {
                GlobalVar.SettingClose = true;
                GlobalVar.dlgOpened = false;
                //timerUI.Dispose();
                //this.Parent.BringToFront();
            }
            catch
            { }
            //}
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            //debug("load");
            FuncInline.TabMachine = FuncInline.enumTabMachine.Machine;
            //GlobalVar.Machine_Machine = true;
            //GlobalVar.Machine_TowerLamp = false;
            //GlobalVar.Machine_SerialSet = false;

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
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        #endregion

        #region 타이머
        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Machine) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerDoing = false;
                return;
            }
            if (timerDoing)
            {
                return;
            }
            timerDoing = false;
            try
            {
                Util.StartWatch(ref GlobalVar.PwdWatch); // 교시모드 타이머 연장
                //timerUI.Dispose();

                /* 화면 변경 timer */
                if (!GlobalVar.GlobalStop &&
                    this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
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

                    #region Sub Menu
                    FuncForm.SetButtonColor2(btnMachine_Setting, FuncInline.TabMachine == FuncInline.enumTabMachine.Machine);
                    FuncForm.SetButtonColor2(btnPin_Setting, FuncInline.TabMachine == FuncInline.enumTabMachine.PinBlock);
                    FuncForm.SetButtonColor2(btnTest_Setting, FuncInline.TabMachine == FuncInline.enumTabMachine.TestSite);
                    FuncForm.SetButtonColor2(btnSerial_Setting, FuncInline.TabMachine == FuncInline.enumTabMachine.SerialSet);
                    FuncForm.SetButtonColor2(btnTower_Lamp_Setting, FuncInline.TabMachine == FuncInline.enumTabMachine.TowerLamp);
                    FuncForm.SetButtonColor2(btnIOMonitor, FuncInline.TabMachine == FuncInline.enumTabMachine.IoMonitor);

                    tcMachine.SelectedIndex = (int)FuncInline.TabMachine;
                    #endregion
                }));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }

            timerDoing = false;
            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    timerUI = new System.Threading.Timer(new TimerCallback(TimerUI), false, 0, 100);
            //}
            if (GlobalVar.GlobalStop)
            {
                try
                {
                    timerUI.Dispose();
                }
                catch { }
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

            if (FuncInline.TabMachine == FuncInline.enumTabMachine.Machine) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Apply Machine setting?"))
                {
                    frmGeneral.ApplyAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TowerLamp) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Apply Tower Lamp setting?"))
                {
                    frmTowerLamp.ApplyAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.SerialSet) //GlobalVar.Machine_SerialSet)
            {
              
                if (FuncWin.MessageBoxOK("Apply port setting?"))
                {
                    frmSerialSet.ApplyAllValue();
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

            if (FuncInline.TabMachine == FuncInline.enumTabMachine.Machine) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Cancel Machine setting?"))
                {
                    frmGeneral.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TowerLamp) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Cancel Tower Lamp setting?"))
                {
                    frmTowerLamp.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.SerialSet) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Cancel port setting?"))
                {
                    frmSerialSet.LoadAllValue();
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

            if (FuncInline.TabMachine == FuncInline.enumTabMachine.Machine) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Load Machine setting?"))
                {
                    Func.LoadMachinenIni();
                    frmGeneral.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TowerLamp) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Load Tower Lamp setting?"))
                {
                    Func.LoadMachinenIni();
                    frmTowerLamp.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.SerialSet) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Load port setting?"))
                {
                    Func.LoadPortIni();
                    frmSerialSet.LoadAllValue();
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

            if (FuncInline.TabMachine == FuncInline.enumTabMachine.Machine) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Save Machine setting?"))
                {
                    frmGeneral.ApplyAllValue();
                    Func.SaveMachineIni();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TowerLamp) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Save Tower Lamp setting?"))
                {
                    frmTowerLamp.ApplyAllValue();
                    Func.SaveMachineIni();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.SerialSet) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Save port setting?"))
                {
                    frmSerialSet.ApplyAllValue();
                    Func.SavePortIni();
                }
            }
        }
        private void btnMachine_Setting_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("System - Machine Click");

            FuncInline.TabMachine = FuncInline.enumTabMachine.Machine;
            //GlobalVar.Machine_Machine = true;
            //GlobalVar.Machine_TowerLamp = false;
            //GlobalVar.Machine_SerialSet = false;

            //machine_Panel.Controls.Clear();
            //machine_Panel.Controls.Add(ucSc1);
            tcMachine.SelectedIndex = (int)FuncInline.enumTabMachine.Machine;
            frmGeneral.LoadAllValue();
            //machine_Panel.BringToFront();
        }
        private void btnTower_Lamp_Setting_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("System - TowerLamp Click");

            FuncInline.TabMachine = FuncInline.enumTabMachine.TowerLamp;
            //GlobalVar.Machine_Machine = false;
            //GlobalVar.Machine_TowerLamp = true;
            //GlobalVar.Machine_SerialSet = false;

            //machine_Panel.Controls.Clear();
            //machine_Panel.Controls.Add(ucSc2);
            tcMachine.SelectedIndex = (int)FuncInline.enumTabMachine.TowerLamp;
            frmTowerLamp.LoadAllValue();
            //tower_Panel.BringToFront();
        }
        private void btnSerial_Setting_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("System - Serial Click");

            FuncInline.TabMachine = FuncInline.enumTabMachine.SerialSet;
            //GlobalVar.Machine_Machine = false;
            //GlobalVar.Machine_TowerLamp = false;
            //GlobalVar.Machine_SerialSet = true;

            //machine_Panel.Controls.Clear();
            //machine_Panel.Controls.Add(ucSc3);
            tcMachine.SelectedIndex = (int)FuncInline.enumTabMachine.SerialSet;
            frmSerialSet.LoadAllValue();
            //serial_Panel.BringToFront();
        }

        private void btnPin_Setting_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("System - PinBlock Click");

            FuncInline.TabMachine = FuncInline.enumTabMachine.PinBlock;

            //machine_Panel.Controls.Clear();
            //machine_Panel.Controls.Add(ucSc4);
            tcMachine.SelectedIndex = (int)FuncInline.enumTabMachine.PinBlock;
            frmPinBlock.LoadAllValue();
            //pin_Panel.BringToFront();
        }

        private void btnTest_Setting_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("System - TestSite Click");

            FuncInline.TabMachine = FuncInline.enumTabMachine.TestSite;
            tcMachine.SelectedIndex = (int)FuncInline.enumTabMachine.TestSite;
            frmTest.LoadAllValue();
            frmTest.ResetTestErrorCode();
        }

        private void btnIOMonitor_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("System - IO Data Click");

            FuncInline.TabMachine = FuncInline.enumTabMachine.IoMonitor;
            tcMachine.SelectedIndex = (int)FuncInline.enumTabMachine.IoMonitor;

        }

        private void pbLoad_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (FuncInline.TabMachine == FuncInline.enumTabMachine.Machine) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Load Machine setting?"))
                {
                    FuncLog.WriteLog("System - Load Machine setting");

                    Func.LoadMachinenIni();
                    frmGeneral.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TowerLamp) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Load Tower Lamp setting?"))
                {
                    FuncLog.WriteLog("System - Load TowerLamp setting");

                    Func.LoadTowerIni();
                    frmTowerLamp.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.SerialSet) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Load port setting?"))
                {
                    FuncLog.WriteLog("System - Load Port setting");
                    Func.LoadPortIni();
                    frmSerialSet.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.PinBlock) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Load Pin Block setting?"))
                {
                    FuncLog.WriteLog("System - Load PinBlock setting");
                    Func.LoadPinIni();
                    frmPinBlock.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TestSite) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Load Test Site setting?"))
                {
                    FuncLog.WriteLog("System - Load TestSite setting");
                    Func.LoadTestSiteIni();
                    Func.LoadSiteOrder();
                    frmTest.LoadAllValue();
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

            if (FuncInline.TabMachine == FuncInline.enumTabMachine.Machine) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Save Machine setting?"))
                {
                    FuncLog.WriteLog("System - Save Machine setting");
                    frmGeneral.ApplyAllValue();
                    Func.SaveMachineIni();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TowerLamp) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Save Tower Lamp setting?"))
                {
                    FuncLog.WriteLog("System - Save TowerLamp setting");
                    frmTowerLamp.ApplyAllValue();
                    Func.SaveTowerIni();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.SerialSet) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Save port setting?"))
                {
                    FuncLog.WriteLog("System - Save Port setting");
                    frmSerialSet.ApplyAllValue();
                    Func.SavePortIni();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.PinBlock) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Save Pin Block setting?"))
                {
                    FuncLog.WriteLog("System - Save PinBlock setting");
                    frmPinBlock.ApplyAllValue();
                    Func.SavePinIni();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TestSite) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Save Test Site setting?"))
                {
                    FuncLog.WriteLog("System - Save TestSite setting");
                    #region 사이트 사용여부 달라지면 로그 남긴다
                    for (int i = 0; i < FuncInline.MaxSiteCount; i++)
                    {
                        Button useButton = (Button)Controls.Find("btnUseSite" + (i + 1), true)[0];
                        // 블럭되는 경우
                        if (FuncInline.UseSite[i] &&
                            useButton.BackColor != Color.Lime)
                        {
                            FuncInline.BlockReason[i] = "User Block - System";
                            FuncInline.InsertBlockHistory(i + 1, false, "User Block - System");
                        }
                        // 블럭 해제되는 경우
                        else if (FuncInline.UseSite[i] &&
                            useButton.BackColor != Color.Lime)
                        {
                            FuncInline.BlockReason[i] = "";
                            FuncInline.InsertBlockHistory(i + 1, true, "User Enable - System");
                        }
                    }
                    #endregion
                    frmTest.ApplyAllValue();
                    Func.SaveTestSiteIni();
                    Func.SaveSiteOrder();
                    //FuncInline.ResetInputCount();
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

            if (FuncInline.TabMachine == FuncInline.enumTabMachine.Machine) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Apply Machine setting?"))
                {
                    FuncLog.WriteLog("System - Apply Machine setting");
                    frmGeneral.ApplyAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TowerLamp) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Apply Tower Lamp setting?"))
                {
                    FuncLog.WriteLog("System - Apply PowerLamp setting");
                    frmTowerLamp.ApplyAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.SerialSet) //GlobalVar.Machine_SerialSet)
            {
             
                if (FuncWin.MessageBoxOK("Apply port setting?"))
                {
                    FuncLog.WriteLog("System - Apply Port setting");
                    frmSerialSet.ApplyAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.PinBlock) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Apply Pin Block setting?"))
                {
                    FuncLog.WriteLog("System - Apply PinBlock setting");
                    frmPinBlock.ApplyAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TestSite) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Apply Test Site setting?"))
                {
                    FuncLog.WriteLog("System - Apply TestSite setting");
                    frmTest.ApplyAllValue();
                }
            }
        }

        public void LoadAllValue()
        {
            frmGeneral.LoadAllValue();
            frmTowerLamp.LoadAllValue();
            frmTest.LoadAllValue();
            frmSerialSet.LoadAllValue();
            frmPinBlock.LoadAllValue();
        }

        private void pbCancel_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (FuncInline.TabMachine == FuncInline.enumTabMachine.Machine) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Cancel Machine setting?"))
                {
                    FuncLog.WriteLog("System - Cancel Machine setting");
                    frmGeneral.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TowerLamp) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Cancel Tower Lamp setting?"))
                {
                    FuncLog.WriteLog("System - Cancel TowerLamp setting");
                    frmTowerLamp.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.SerialSet) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Cancel port setting?"))
                {
                    FuncLog.WriteLog("System - Cancel Port setting");
                    frmSerialSet.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.PinBlock) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Cancel Pin Block setting?"))
                {
                    FuncLog.WriteLog("System - Cancel PinBlock setting");
                    frmPinBlock.LoadAllValue();
                }
            }
            if (FuncInline.TabMachine == FuncInline.enumTabMachine.TestSite) //GlobalVar.Machine_SerialSet)
            {
                if (FuncWin.MessageBoxOK("Cancel Test Site setting?"))
                {
                    FuncLog.WriteLog("System - Cancel TestSite setting");
                    frmTest.LoadAllValue();
                }
            }
        }

        #endregion

        private void pnHidden_Click(object sender, EventArgs e)
        {
            /*
            if (++hiddenTime > 2)
            {
                if (GlobalVar.PwdPass)
                {
                    hiddenTime = 0;
                    return;
                }
                Password dlg = new Password();
                dlg.ShowDialog();
                hiddenTime = 0;
            }
            //*/
        }

        private void pnHidden_Leave(object sender, EventArgs e)
        {
            //hiddenTime = 0;
        }

        private void pnHidden_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnModuleIO_Click(object sender, EventArgs e)
        {
          
        }
    }
}
