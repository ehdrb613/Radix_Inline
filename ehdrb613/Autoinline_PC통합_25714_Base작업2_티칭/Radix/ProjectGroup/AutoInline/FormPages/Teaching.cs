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

    public partial class Teaching : Form
    {
        Popup.Teaching.LiftPosition frmLiftPosition = new Popup.Teaching.LiftPosition();
        //Popup.Teaching.OtherPosition frmOtherPosition = new Popup.Teaching.OtherPosition();
        Popup.Teaching.Width frmWidth = new Popup.Teaching.Width();
        //Popup.Teaching.ETCPosition frmETC = new Popup.Teaching.ETCPosition();

        #region 로컬변수
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerUIDoing = false;
        #endregion


        #region 초기화 관련
        public Teaching()
        {
            InitializeComponent();

            frmLiftPosition.FormBorderStyle = FormBorderStyle.None;
            frmLiftPosition.TopMost = true;
            frmLiftPosition.Dock = DockStyle.Fill;
            frmLiftPosition.TopLevel = false;
            frmLiftPosition.LoadAllValue();
            tpLift.Controls.Clear();
            tpLift.Controls.Add(frmLiftPosition);
            frmLiftPosition.Show();

            /*
            frmOtherPosition.FormBorderStyle = FormBorderStyle.None;
            frmOtherPosition.TopMost = true;
            frmOtherPosition.Dock = DockStyle.Fill;
            frmOtherPosition.TopLevel = false;
            frmOtherPosition.LoadAllValue();
            tpOtherPosition.Controls.Clear();
            tpOtherPosition.Controls.Add(frmOtherPosition);
            frmOtherPosition.Show();
            //*/

            frmWidth.FormBorderStyle = FormBorderStyle.None;
            frmWidth.TopMost = true;
            frmWidth.Dock = DockStyle.Fill;
            frmWidth.TopLevel = false;
            frmWidth.LoadAllValue();
            //width_Panel.Controls.Clear();
            //width_Panel.Controls.Add(frmWidth);
            tpWidth.Controls.Clear();
            tpWidth.Controls.Add(frmWidth);
            frmWidth.Show();
            //width_Panel.BringToFront();

            //frmETC.FormBorderStyle = FormBorderStyle.None;
            //frmETC.TopMost = true;
            //frmETC.Dock = DockStyle.Fill;
            //frmETC.TopLevel = false;
            //frmETC.LoadAllValue();
            ////width_Panel.Controls.Clear();
            ////width_Panel.Controls.Add(frmWidth);
            //tpETC.Controls.Clear();
            //tpETC.Controls.Add(frmETC);
            //frmETC.Show();
            //width_Panel.BringToFront();

            //FuncWin.OpenExeInPanel(tpScan, "cmd.exe", "/?", System.Diagnostics.ProcessWindowStyle.Normal);

            //position_Panel.BringToFront();

        }

        private void debug(string str)
        {
            Util.Debug(str);
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
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
            FuncInline.TabTeaching = FuncInline.enumTabTeaching.LiftPosition;
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
            if (FuncInline.TabMain != FuncInline.enumTabMain.Teaching) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerUIDoing = false;
                return;
            }
            try
            {
                if (timerUIDoing)
                {
                    return;
                }
                timerUIDoing = true;
                //timerUI.Dispose();

                Util.StartWatch(ref GlobalVar.PwdWatch); // 교시모드 타이머 연장
                /* 화면 변경 timer */
                if (!GlobalVar.GlobalStop &&
                    this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                {

                    #region sub menu
                    FuncForm.SetButtonColor2(btnLiftPosition_Setting, FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition);
                    //FuncForm.SetButtonColor2(btnOtherPosition_Setting, FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition);
                    FuncForm.SetButtonColor2(btnWidth_Setting, FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width);
                    FuncForm.SetButtonColor2(btnScan_Setting, FuncInline.TabTeaching == FuncInline.enumTabTeaching.Scan);
                    #endregion

                    /*
                    lblSpeed.Text = GlobalVar.RobotJogSpeed.ToString() + " mm / sec";
                    //btnSlow.BackColor = GlobalVar.RobotJogSpeed == GlobalVar.RobotJogSpeedSlow ? Color.Lime : Color.White;
                    //btnMiddle.BackColor = GlobalVar.RobotJogSpeed == GlobalVar.RobotJogSpeedMiddle ? Color.Lime : Color.White;
                    btnFast.BackColor = GlobalVar.RobotJogSpeed == 50 ? Color.Lime : Color.White;
                    btn1mm.BackColor = GlobalVar.RobotJogSpeed == 1 ? Color.Lime : Color.White;
                    if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Scan)
                    {
                        label1.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btn01mm.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btn1mm.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btnSlow.Visible = false; // FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btnMiddle.Visible = false; // FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btnFast.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbLoad.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbSave.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbApply.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbCancel.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;

                    }
                    else if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width)
                    {
                        btnSlow.Visible = false;
                        btnMiddle.Visible = false;
                        label1.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btn01mm.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btn1mm.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btnFast.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbLoad.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbSave.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbApply.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbCancel.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btnFast.BackColor = GlobalVar.RobotJogSpeed != 1 && GlobalVar.RobotJogSpeed != 0.1 ? Color.Lime : Color.White;
                    }
                    else
                    {
                        btnSlow.Visible = false; // true;
                        btnMiddle.Visible = false; // true;
                        label1.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btn01mm.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btn1mm.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        btnFast.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbLoad.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbSave.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbApply.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                        pbCancel.Visible = FuncInline.TabTeaching != FuncInline.enumTabTeaching.Scan;
                    }
                    btn01mm.BackColor = GlobalVar.RobotJogSpeed == 0.1 ? Color.Lime : Color.White;
                    //*/



                    tcTeaching.SelectedIndex = (int)FuncInline.TabTeaching;
                }));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }

            timerUIDoing = false;
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

            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Apply Robot Position setting?"))
                {
                    frmLiftPosition.ApplyAllValue();
                }
            }
            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Apply Width setting?"))
                {
                    frmWidth.ApplyAllValue();
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

            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Cancel Lift Position setting?"))
                {
                    frmLiftPosition.LoadAllValue();
                }
            }
            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Cancel Width setting?"))
                {
                    frmWidth.LoadAllValue();
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

            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Load Robot Position setting?"))
                {
                    Func.LoadTeachingPositionIni();
                    frmLiftPosition.LoadAllValue();
                }
            }
            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Load Width setting?"))
                {
                    Func.LoadTeachingIni();
                    frmWidth.LoadAllValue();
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

            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Save Lift Position setting?"))
                {
                    frmLiftPosition.ApplyAllValue();
                    Func.SaveTeachingPositionIni();
                }
            }
            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Save Width setting?"))
                {
                    frmWidth.ApplyAllValue();
                    Func.SaveTeachingWidthIni();
                }
            }
        }
        private void btnPosition_Setting_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Teaching - Robot Position Click");
            FuncInline.TabTeaching = FuncInline.enumTabTeaching.LiftPosition;
            //GlobalVar.Machine_Machine = true;
            //GlobalVar.Machine_TowerLamp = false;
            //GlobalVar.Machine_SerialSet = false;

            //position_Panel.Controls.Clear();
            //position_Panel.Controls.Add(ucSc1);
            //position_Panel.BringToFront();

            tcTeaching.SelectedIndex = (int)FuncInline.enumTabTeaching.LiftPosition;
            frmLiftPosition.LoadAllValue();
        }
        private void btnWidth_Setting_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Teaching - Width Click");
            FuncInline.TabTeaching = FuncInline.enumTabTeaching.Width;

            tcTeaching.SelectedIndex = (int)FuncInline.enumTabTeaching.Width;
            frmWidth.LoadAllValue();
        }
        private void btnScan_Setting_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Teaching - Vision Click");
            FuncInline.TabTeaching = FuncInline.enumTabTeaching.Scan;
            //FuncWin.OpenExeInPanel(position_Panel, "cmd.exe", "/?", System.Diagnostics.ProcessWindowStyle.Normal);
            //scan_Panel.BringToFront();
            tcTeaching.SelectedIndex = (int)FuncInline.enumTabTeaching.Scan;
        }

        private void pbLoad_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Load Lift Position setting?"))
                {
                    FuncLog.WriteLog("Teaching - Load Lift Position");
                    Func.LoadTeachingPositionIni();
                    frmLiftPosition.LoadAllValue();
                }
            }
            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Load Width setting?"))
                {
                    FuncLog.WriteLog("Teaching - Load Width");
                    Func.LoadTeachingWidthIni();
                    frmWidth.LoadAllValue();
                }
            }
            //if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.ETC) //GlobalVar.Machine_TowerLamp)
            //{
            //    if (FuncWin.MessageBoxOK("Load ETC Position setting?"))
            //    {
            //        FuncLog.WriteLog("Teaching - Load ETC");
            //        Func.LoadTeachingETCIni();
            //        frmETC.LoadAllValue();
            //    }
            //}

        }

        private void pbSave_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Save Lift Position setting?"))
                {
                    FuncLog.WriteLog("Teaching - Save Lift Position");
                    frmLiftPosition.ApplyAllValue();
                    Func.SaveTeachingPositionIni();
                }
            }
            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Save Width setting?"))
                {
                    FuncLog.WriteLog("Teaching - Save Width");
                    frmWidth.ApplyAllValue();
                    Func.SaveTeachingWidthIni();
                    //Func.SaveWidthOffsetIni();
                }
            }
            //if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.ETC) //GlobalVar.Machine_TowerLamp)
            //{
            //    if (FuncWin.MessageBoxOK("Save ETC Position setting?"))
            //    {
            //        FuncLog.WriteLog("Teaching - Save ETC");
            //        frmETC.ApplyAllValue();
            //        Func.SaveTeachingETCIni();
            //        //Func.SaveWidthOffsetIni();
            //    }
            //}

        }

        private void pbApply_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change setting while system is running!");
                return;
            }

            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Apply Lift Position setting?"))
                {
                    FuncLog.WriteLog("Teaching - Apply Lift Position");
                    frmLiftPosition.ApplyAllValue();
                }
            }
            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Apply Width setting?"))
                {
                    FuncLog.WriteLog("Teaching - Apply Width");
                    frmWidth.ApplyAllValue();
                }
            }

        }

        public void LoadAllValue()
        {
            frmLiftPosition.LoadAllValue();
            frmWidth.LoadAllValue();
        }


        private void pbCancel_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.LiftPosition) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Cancel Lift Position setting?"))
                {
                    FuncLog.WriteLog("Teaching - Cancel Lift Position");
                    frmLiftPosition.LoadAllValue();
                }
            }
            if (FuncInline.TabTeaching == FuncInline.enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Cancel Width setting?"))
                {
                    FuncLog.WriteLog("Teaching - Cancel Width");
                    frmWidth.LoadAllValue();
                }
            }
        }
        #endregion

        private void btnSlow_Click(object sender, EventArgs e)
        {
            //GlobalVar.RobotJogSpeed = GlobalVar.RobotJogSpeedSlow;
            //GlobalVar.WidthJogSpeed = GlobalVar.WidthJogSpeedSlow;
        }

        private void btnMiddle_Click(object sender, EventArgs e)
        {
            //GlobalVar.RobotJogSpeed = GlobalVar.RobotJogSpeedMiddle;
            //GlobalVar.WidthJogSpeed = GlobalVar.WidthJogSpeedMiddle;
        }

        private void btnFast_Click(object sender, EventArgs e)
        {
            FuncInline.RobotJogSpeed = 50;
            FuncInline.WidthJogSpeed = 5;
        }

        private void btn1mm_Click(object sender, EventArgs e)
        {
            FuncInline.RobotJogSpeed = 1;
            FuncInline.WidthJogSpeed = 1;
        }

        private void btn01mm_Click(object sender, EventArgs e)
        {
            FuncInline.RobotJogSpeed = 0.1;
            FuncInline.WidthJogSpeed = 0.1;
        }

        private void btnLiftPosition_Setting_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Teaching - Lift Click");
            FuncInline.TabTeaching = FuncInline.enumTabTeaching.LiftPosition;
            tcTeaching.SelectedIndex = (int)FuncInline.enumTabTeaching.LiftPosition;
            frmWidth.LoadAllValue();
        }

        private void btnEtc_Setting_Click(object sender, EventArgs e)
        {
            //FuncLog.WriteLog("Teaching - ETC Click");
            //FuncInline.TabTeaching = FuncInline.enumTabTeaching.ETC;

            //tcTeaching.SelectedIndex = (int)FuncInline.enumTabTeaching.ETC;
            //frmETC.LoadAllValue();
        }
    }
}
