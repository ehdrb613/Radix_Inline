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
        Popup.Teaching.Position frmPosition = new Popup.Teaching.Position();
        Popup.Teaching.Width frmWidth = new Popup.Teaching.Width();

        #region 로컬변수
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;
        #endregion


        #region 초기화 관련
        public Teaching()
        {
            InitializeComponent();

            frmPosition.FormBorderStyle = FormBorderStyle.None;
            frmPosition.TopMost = true;
            frmPosition.Dock = DockStyle.Fill;
            frmPosition.TopLevel = false;
            frmPosition.LoadAllValue();
            //position_Panel.Controls.Clear();
            //position_Panel.Controls.Add(frmPosition);
            tpPosition.Controls.Clear();
            tpPosition.Controls.Add(frmPosition);
            frmPosition.Show();
            //position_Panel.BringToFront();

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

            FuncWin.OpenExeInPanel(tpScan, "cmd.exe", "/?", System.Diagnostics.ProcessWindowStyle.Normal);

            //position_Panel.BringToFront();

        }

        private void debug(string str)
        {
            Util.Debug(str);
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;
            GlobalVar.PwdPass = true;

            #region 화면 제어용 쓰레드 타이머 시작            
            //TimerCallback CallBackUI = new TimerCallback(TimerUI);
            //timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);
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
            GlobalVar.TabTeaching = enumTabTeaching.Position;
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
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
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
                if (this.InvokeRequired)
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
                }));
                }

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
        public void LoadAllValue()
        {
            frmPosition.LoadAllValue();
            frmWidth.LoadAllValue();
        }
        private void btnPosition_Setting_Click(object sender, EventArgs e)
        {
            GlobalVar.TabTeaching = enumTabTeaching.Position;
            //GlobalVar.Machine_Machine = true;
            //GlobalVar.Machine_TowerLamp = false;
            //GlobalVar.Machine_SerialSet = false;

            //position_Panel.Controls.Clear();
            //position_Panel.Controls.Add(ucSc1);
            //position_Panel.BringToFront();

            tcTeaching.SelectedIndex = (int)enumTabTeaching.Position;
            frmPosition.LoadAllValue();
        }
        private void btnWidth_Setting_Click(object sender, EventArgs e)
        {
            GlobalVar.TabTeaching = enumTabTeaching.Width;
            //GlobalVar.Machine_Machine = false;
            //GlobalVar.Machine_TowerLamp = true;
            //GlobalVar.Machine_SerialSet = false;

            //position_Panel.Controls.Clear();
            //position_Panel.Controls.Add(ucSc2);
            //width_Panel.BringToFront();
            tcTeaching.SelectedIndex = (int)enumTabTeaching.Width;
            frmWidth.LoadAllValue();
        }
        private void btnScan_Setting_Click(object sender, EventArgs e)
        {
            GlobalVar.TabTeaching = enumTabTeaching.Scan;
            //FuncWin.OpenExeInPanel(position_Panel, "cmd.exe", "/?", System.Diagnostics.ProcessWindowStyle.Normal);
            //scan_Panel.BringToFront();
            tcTeaching.SelectedIndex = (int)enumTabTeaching.Scan;
        }

        private void pbLoad_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                FuncWin.TopMessageBox("Can not change while system is running!");
                return;
            }

            if (GlobalVar.TabTeaching == enumTabTeaching.Position) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Load Position setting?"))
                {
                    FuncIni.LoadTeachingPositionIni();
                    frmPosition.LoadAllValue();
                }
            }
            if (GlobalVar.TabTeaching == enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Load Width setting?"))
                {
                    FuncIni.LoadTeachingWidthIni();
                    frmWidth.LoadAllValue();
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

            if (GlobalVar.TabTeaching == enumTabTeaching.Position) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Save Position setting?"))
                {
                    frmPosition.ApplyAllValue();
                    FuncIni.SaveTeachingPositionIni();
                }
            }
            if (GlobalVar.TabTeaching == enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Save Width setting?"))
                {
                    frmWidth.ApplyAllValue();
                    FuncIni.SaveTeachingWidthIni();
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

            if (GlobalVar.TabTeaching == enumTabTeaching.Position) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Apply Position setting?"))
                {
                    frmPosition.ApplyAllValue();
                }
            }
            if (GlobalVar.TabTeaching == enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Apply Width setting?"))
                {
                    frmWidth.ApplyAllValue();
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

            if (GlobalVar.TabTeaching == enumTabTeaching.Position) //GlobalVar.Machine_Machine)
            {
                if (FuncWin.MessageBoxOK("Cancel Position setting?"))
                {
                    frmPosition.LoadAllValue();
                }
            }
            if (GlobalVar.TabTeaching == enumTabTeaching.Width) //GlobalVar.Machine_TowerLamp)
            {
                if (FuncWin.MessageBoxOK("Cancel Width setting?"))
                {
                    frmWidth.LoadAllValue();
                }
            }
        }
        #endregion

    }
}
