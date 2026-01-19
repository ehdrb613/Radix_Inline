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

    public partial class Manual : Form
    {
        Popup.Manual.Lift frmRobot = new Popup.Manual.Lift();
        Popup.Manual.Site frmSite = new Popup.Manual.Site();
        Popup.Manual.Conveyor frmConveyor = new Popup.Manual.Conveyor();
      
        #region 로컬변수
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerUIDoing = false;

        #endregion


        #region 초기화 관련
        public Manual()
        {
            InitializeComponent();

            frmRobot.FormBorderStyle = FormBorderStyle.None;
            frmRobot.TopMost = true;
            frmRobot.Dock = DockStyle.Fill;
            frmRobot.TopLevel = false;
            tpRobot.Controls.Clear();
            tpRobot.Controls.Add(frmRobot);
            frmRobot.Show();

            frmSite.FormBorderStyle = FormBorderStyle.None;
            frmSite.TopMost = true;
            frmSite.Dock = DockStyle.Fill;
            frmSite.TopLevel = false;
            tpSite.Controls.Clear();
            tpSite.Controls.Add(frmSite);
            frmSite.Show();

            frmConveyor.FormBorderStyle = FormBorderStyle.None;
            frmConveyor.TopMost = true;
            frmConveyor.Dock = DockStyle.Fill;
            frmConveyor.TopLevel = false;
            tpConveyor.Controls.Clear();
            tpConveyor.Controls.Add(frmConveyor);
            frmConveyor.Show();

            //frmVision.FormBorderStyle = FormBorderStyle.None;
            //frmVision.TopMost = true;
            //frmVision.Dock = DockStyle.Fill;
            //frmVision.TopLevel = false;
            //tpVision.Controls.Clear();
            //tpVision.Controls.Add(frmVision);
            //frmVision.Show();
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
            if (FuncInline.TabMain != FuncInline.enumTabMain.Manual) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
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
                    FuncForm.SetButtonColor2(btnLift_Manual, FuncInline.TabManual == FuncInline.enumTabManual.Lift);
                    FuncForm.SetButtonColor2(btnSite_Manual, FuncInline.TabManual == FuncInline.enumTabManual.Site);
                    FuncForm.SetButtonColor2(btnConveyor_Manual, FuncInline.TabManual == FuncInline.enumTabManual.Conveyor);
                    FuncForm.SetButtonColor2(btnVision_Manual, FuncInline.TabManual == FuncInline.enumTabManual.Vision);
                    #endregion

                    /*
                    lblSpeed.Text = "Motor Speed : " + GlobalVar.RobotJogSpeed.ToString() + " mm / sec";
                    //btnSlow.BackColor = GlobalVar.RobotJogSpeed != 1 && GlobalVar.RobotJogSpeed != 0.1 && GlobalVar.RobotJogSpeed == GlobalVar.RobotJogSpeedSlow ? Color.Lime : Color.White;
                    //btnMiddle.BackColor = GlobalVar.RobotJogSpeed != 1 && GlobalVar.RobotJogSpeed != 0.1 && GlobalVar.RobotJogSpeed == GlobalVar.RobotJogSpeedMiddle ? Color.Lime : Color.White;
                    btnFast.BackColor = GlobalVar.RobotJogSpeed != 1 && GlobalVar.RobotJogSpeed != 0.1 && GlobalVar.RobotJogSpeed == 50 ? Color.Lime : Color.White;
                    btn1mm.BackColor = GlobalVar.RobotJogSpeed == 1 ? Color.Lime : Color.White;
                    btn01mm.BackColor = GlobalVar.RobotJogSpeed == 0.1 ? Color.Lime : Color.White;

                    if (FuncInline.TabManual == FuncInline.enumTabManual.Site)
                    {
                        btnSlow.Visible = false;
                        btnMiddle.Visible = false;
                        btnFast.BackColor = GlobalVar.RobotJogSpeed != 1 && GlobalVar.RobotJogSpeed != 0.1 ? Color.Lime : Color.White;
                    }
                    else
                    {
                        btnSlow.Visible = false;// true;
                        btnMiddle.Visible = false; // true;
                    }
                    //*/
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
        private void btnRobot_Manual_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Manual - Robot Click");
            FuncInline.TabManual = FuncInline.enumTabManual.Lift;

            tcManual.SelectedIndex = (int)FuncInline.enumTabManual.Lift;
        }
        private void btnConveyor_Manual_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Manual - Conveyor Click");
            FuncInline.TabManual = FuncInline.enumTabManual.Conveyor;
            tcManual.SelectedIndex = (int)FuncInline.enumTabManual.Conveyor;
        }
        private void btnVision_Manual_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Manual - Vision Click");
            FuncInline.TabManual = FuncInline.enumTabManual.Vision;
            tcManual.SelectedIndex = (int)FuncInline.enumTabManual.Vision;
        }

        private void btnSite_Manual_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Manual - Site Click");
            FuncInline.TabManual = FuncInline.enumTabManual.Site;

            tcManual.SelectedIndex = (int)FuncInline.enumTabManual.Site;
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
            //GlobalVar.RobotJogSpeed = 50;
            //GlobalVar.WidthJogSpeed = 5;
        }

        private void btn1mm_Click(object sender, EventArgs e)
        {
            //GlobalVar.RobotJogSpeed = 1;
            //GlobalVar.WidthJogSpeed = 1;
        }

        private void btn01mm_Click(object sender, EventArgs e)
        {
            //GlobalVar.RobotJogSpeed = 0.1;
            //GlobalVar.WidthJogSpeed = 0.1;
        }
    }
}
