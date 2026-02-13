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

namespace Radix.Popup.Teaching
{
    /*
     * Setting.cs : 각종 옵션 설정 관리
     */

    public partial class Width : Form
    {
        #region 로컬 변수
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;
        private System.Threading.Timer timerMotor; // Thread Timer
        private bool timerMotorDoing = false;
        private System.Threading.Timer timerJog; // Thread Timer
        private bool timerJogDoing = false;

        private bool jogWidthSite = false;
        private bool jogWidthSiteNarrow = false;
        private bool jogWidthSiteWide = false;

        private string activePanelWidth = "pnWidthInputLift";

        private bool widthAdvanced = false; // 폭조절 상세 모드

        enumTabMain beforeTabMain = GlobalVar.TabMain;
        enumTabTeaching beforeTabTeaching = GlobalVar.TabTeaching;
        #endregion


        #region 초기화 함수
        public Width()
        {
            InitializeComponent();
        }


        private void Width_Load(object sender, EventArgs e)
        {
            debug("load");
            try
            {
                #region 화면 제어용 쓰레드 타이머 시작
                //*
                TimerCallback CallBackUI = new TimerCallback(TimerUI);
                timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);

                TimerCallback CallBackMotor = new TimerCallback(TimerMotor);
                timerMotor = new System.Threading.Timer(CallBackMotor, false, 0, 100);

                TimerCallback CallBackJog = new TimerCallback(TimerJog);
                timerJog = new System.Threading.Timer(CallBackJog, false, 0, 100);
                //*/
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

        }

        private void Width_Leave(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun) // 운전중 아니면 모든 모터 정지
            {

                for (int i = 0; i < Enum.GetValues(typeof(enumServoAxis)).Length; i++)
                {
                    FuncMotion.MoveStop(i);
                }
                for (int i = 0; i < Enum.GetValues(typeof(enumPMCMotion)).Length; i++)
                {
                    GlobalVar.AutoInline_ComPMC[i].SlowStop(enumAxis.XY);
                }
            }
            timerJog.Dispose();
            timerMotor.Dispose();
            timerUI.Dispose();

        }

        #endregion

        #region 타이머 함수
        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            if (GlobalVar.TabMain != enumTabMain.Teaching) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
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
                if (this == null)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {

                        #region 관리자 여부에 따라 컨트롤 보이기/숨기기
                        //tbAppName.Enabled = GlobalVar.PwdPass;
                        //numMachineNum.Enabled = GlobalVar.PwdPass;
                        //pbSave.Visible = GlobalVar.PwdPass;
                        #endregion

                        //Util.SetButtonDoColor(btnRobotGripLeft, enumDONames.Y03_7_Robot_Vac_Input);
                        //Util.SetButtonDoColor(btnRobotGripRight, enumDONames.Y04_0_Robot_Vac_Output);
                        //Util.SetButtonDoColor(btnVisionGripLeft, enumDONames.Y03_7_Robot_Vac_Input);
                        //Util.SetButtonDoColor(btnVisionGripRight, enumDONames.Y04_0_Robot_Vac_Output);

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
            timerDoing = false;
        }

        private void TimerMotor(Object state) // 모터상태 쓰레드 타이머 함수
        {
            if (GlobalVar.TabMain != enumTabMain.Teaching ||
                GlobalVar.TabTeaching != enumTabTeaching.Width) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerJogDoing = false;
                return;
            }
            try
            {
                if (timerMotorDoing)
                {
                    return;
                }

                timerMotorDoing = true;

                /* 화면 변경 timer */
                if (this == null)
                {
                    return;
                }
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {

                        #region 모터 현재 위치 표시

                        // 이하 PMC 관련은 추후 문제될 경우 두개씩 합치도록 수정 필요
                        if (true)//!GlobalVar.Simulation)
                        {
                            lblCurrPosWidthInputLift.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.InputLift].Position.ToString("F2");
                            lblCurrPosWidthOutputLift.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.OutputLift].Position.ToString("F2");
                            lblCurrPosWidthBufferConveyor.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.BufferConveyor].Position.ToString("F2");
                            lblCurrPosWidthInputConveyor.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.CoolingConveyor].Position.ToString("F2");
                            lblCurrPosWidthLoadingConveyor.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.LoadingConveyor].Position.ToString("F2");
                            lblCurrPosWidthUnloadingConveyor.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.UnloadingConveyor].Position.ToString("F2");
                            lblCurrPosWidthInputJig.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.InputJig].Position.ToString("F2");
                            lblCurrPosWidthOutputJig.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.OutputJig].Position.ToString("F2");
                            lblCurrPosWidthSite1.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site1].Position.ToString("F2");
                            lblCurrPosWidthSite2.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site2].Position.ToString("F2");
                            lblCurrPosWidthSite3.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site3].Position.ToString("F2");
                            lblCurrPosWidthSite4.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site4].Position.ToString("F2");
                            lblCurrPosWidthSite5.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site5].Position.ToString("F2");
                            lblCurrPosWidthSite6.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site6].Position.ToString("F2");
                            lblCurrPosWidthSite7.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site7].Position.ToString("F2");
                            lblCurrPosWidthSite8.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site8].Position.ToString("F2");
                            lblCurrPosWidthSite9.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site9].Position.ToString("F2");
                            lblCurrPosWidthSite10.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site10].Position.ToString("F2");
                            lblCurrPosWidthSite11.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site11].Position.ToString("F2");
                            lblCurrPosWidthSite12.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site12].Position.ToString("F2");
                            lblCurrPosWidthSite13.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site13].Position.ToString("F2");
                            lblCurrPosWidthSite14.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site14].Position.ToString("F2");
                            lblCurrPosWidthSite15.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site15].Position.ToString("F2");
                            lblCurrPosWidthSite16.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site16].Position.ToString("F2");
                            lblCurrPosWidthSite17.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site17].Position.ToString("F2");
                            lblCurrPosWidthSite18.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site18].Position.ToString("F2");
                            lblCurrPosWidthSite19.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site19].Position.ToString("F2");
                            lblCurrPosWidthSite20.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site20].Position.ToString("F2");
                            lblCurrPosWidthSite21.Text = GlobalVar.PMCStatus[(int)enumPMCAxis.Site21].Position.ToString("F2");
                        }
                        #endregion

                    }));
                }
                timerMotorDoing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }
            timerMotorDoing = false;
        }

        private void TimerJog(Object state) // 조그제어 쓰레드 타이머 함수
        {
            #region 창 떠나면 조그 멈추기
            if (beforeTabMain == enumTabMain.Teaching && beforeTabTeaching == enumTabTeaching.Width &&
                (GlobalVar.TabMain != enumTabMain.Teaching || GlobalVar.TabTeaching != enumTabTeaching.Width))
            {
                FuncMotion.StopAllJog();
            }
            beforeTabMain = GlobalVar.TabMain;
            beforeTabTeaching = GlobalVar.TabTeaching;
            #endregion

            if (GlobalVar.TabMain != enumTabMain.Teaching ||
                GlobalVar.TabTeaching != enumTabTeaching.Width) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerJogDoing = false;
                return;
            }
            if (this == null)
            {
                return;
            }

            try
            {
                if (timerJogDoing)
                {
                    return;
                }

                timerJogDoing = true;

                #region JOG 멈추기
                #region 사이트 폭조절축
                if (jogWidthSite)
                {
                    if (!jogWidthSiteNarrow &&
                        !jogWidthSiteWide)
                    {
                        int index = -1;
                        Array array = Enum.GetValues(typeof(enumPMCAxis));
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (activePanelWidth.Replace("pnWidth", "") == ((enumPMCAxis)i).ToString())
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index >= 0)
                        {
                            enumPMCAxis pmcAxis = (enumPMCAxis)index;
                            if (Math.Abs(FuncPMC.GetCurrSpeed(pmcAxis)) > 0)
                            {
                                FuncPMC.Stop(pmcAxis);
                            }
                            else
                            {
                                jogWidthSite = false;
                            }
                        }
                        else
                        {
                            jogWidthSite = false;
                        }
                    }
                }
                #endregion

                #endregion



                timerJogDoing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }
            timerJogDoing = false;
        }
        #endregion

        #region 설정 관련
        public void LoadAllValue()
        {
            try
            {


                for (int i = 0; i < Enum.GetValues(typeof(enumPMCAxis)).Length; i++)
                {
                    Control[] controls = Controls.Find("lblSetPosWidth" + ((enumPMCAxis)i).ToString(), true);
                    if (controls != null &&
                        controls.Length > 0)
                    {
                        ((Label)(controls[0])).Text = GlobalVar.AutoInline_TeachingWidth[i].ToString("F2");
                    }
                }

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


                for (int i = 0; i < Enum.GetValues(typeof(enumPMCAxis)).Length; i++)
                {
                    Control[] controls = Controls.Find("lblSetPosWidth" + ((enumPMCAxis)i).ToString(), true);
                    if (controls != null &&
                        controls.Length > 0)
                    {
                        GlobalVar.AutoInline_TeachingWidth[i] = double.Parse(((Label)(controls[0])).Text);
                    }
                }

                // 전역설정
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        #endregion

        #region 로컬함수
        private void debug(string str)
        {
            Util.Debug("frmWidth : " + str);
        }

        private void ClearPanelColor(bool width)
        {
            foreach (Panel pn in pnWidth.Controls.OfType<Panel>())
            {
                if (pn.Name.Contains("pnWidth"))
                {
                    pn.BackColor = Color.Transparent;
                }
            }
        }

        private void setPanelColor(object sender)
        {
            ((Panel)sender).BackColor = Color.Lime;
            string name = ((Panel)sender).Name;
            activePanelWidth = name;
            Console.WriteLine(((Panel)sender).Name);
        }

        #endregion

        #region 버튼 클릭 이벤트

        private void pnWidth_Click(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(Panel))
            {
                if (!cbWidthAdvanced.Checked &&
                    ((Panel)sender).Name.Replace("pnWidth", "") != "InputLift")
                {
                    FuncWin.TopMessageBox("In Basic Mode, Only InputLift Width Teaching is needed. And other teaching will be calculated automactically.");
                    return;
                }
                ClearPanelColor(true);
                setPanelColor(sender);
                // pnWidthInput == > lblSetPosWidthInput
                ;
                numWidth.Value = (decimal)double.Parse((((Label)(Controls.Find(((Panel)sender).Name.Replace("pnWidth", "lblSetPosWidth"), true)[0])).Text));
            }
            else
            if (sender.GetType() == typeof(Label))
            {
                if (!cbWidthAdvanced.Checked &&
                    ((Label)sender).Name.Replace("pnWidth", "") != "InputLift")
                {
                    FuncWin.TopMessageBox("In Basic Mode, Only InputLift Width Teaching is needed. And other teaching will be calculated automactically.");
                    return;
                }
                ClearPanelColor(true);
                setPanelColor(((Label)sender).Parent);
                numWidth.Value = (decimal)double.Parse((((Label)(Controls.Find(((Panel)(((Label)sender).Parent)).Name.Replace("pnWidth", "lblSetPosWidth"), true)[0])).Text));
            }
        }

        private void pbWidthApply_Click(object sender, EventArgs e)
        {
            ((Label)(Controls.Find(activePanelWidth.Replace("pnWidth", "lblSetPosWidth"), true)[0])).Text = ((Label)(Controls.Find(activePanelWidth.Replace("pnWidth", "lblCurrPosWidth"), true)[0])).Text;
            #region 기본모드에서는 system offset으로 나머지 모두에 적용
            if (!cbWidthAdvanced.Checked)
            {
                for (int i = 0; i < GlobalVar.WidthOffset.Length; i++)
                {
                    string widthName = ((enumPMCAxis)i).ToString();
                    if (widthName != "InputLift")
                    {
                        Control[] controls = Controls.Find("pnWidth" + widthName, true);
                        if (controls != null &&
                            controls.Length > 0)
                        {
                            ((Label)Controls.Find("lblSetPosWidth" + widthName, true)[0]).Text = (double.Parse(lblSetPosWidthInputLift.Text) + GlobalVar.WidthOffset[i]).ToString("F2");
                        }
                    }
                }

            }
            #endregion
        }

        private void pbWIdthMove_Click(object sender, EventArgs e)
        {
            int index = -1;
            Array array = Enum.GetValues(typeof(enumPMCAxis));
            for (int i = 0; i < array.Length; i++)
            {
                if (activePanelWidth.Replace("pnWidth", "") == ((enumPMCAxis)i).ToString())
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                enumPMCAxis axis = (enumPMCAxis)index;
                if (FuncPMC.GetCurrSpeed(axis) < 1)
                {
                    FuncPMC.ABSMove(axis, (long)numWidth.Value, (int)numWidthSpeedSite.Value);
                }
            }
        }

        private void pbWIdthHome_Click(object sender, EventArgs e)
        {
            int index = -1;
            Array array = Enum.GetValues(typeof(enumPMCAxis));
            for (int i = 0; i < array.Length; i++)
            {
                if (activePanelWidth.Replace("pnWidth", "") == ((enumPMCAxis)i).ToString())
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                enumPMCAxis axis = (enumPMCAxis)index;
                if (FuncPMC.GetCurrSpeed(axis) < 1)
                {
                    FuncPMC.HomeRun(axis, GlobalVar.WidthHomSpeed);
                }
            }
        }

        private void pbWIdthStop_Click(object sender, EventArgs e)
        {
            int index = -1;
            Array array = Enum.GetValues(typeof(enumPMCAxis));
            for (int i = 0; i < array.Length; i++)
            {
                if (activePanelWidth.Replace("pnWidth", "") == ((enumPMCAxis)i).ToString())
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                enumPMCAxis axis = (enumPMCAxis)index;
                FuncPMC.Stop(axis);
            }
        }

        private void pbWidthNarrow_MouseDown(object sender, MouseEventArgs e)
        {
            int index = -1;
            Array array = Enum.GetValues(typeof(enumPMCAxis));
            for (int i = 0; i < array.Length; i++)
            {
                if (activePanelWidth.Replace("pnWidth", "") == ((enumPMCAxis)i).ToString())
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                enumPMCAxis axis = (enumPMCAxis)index;
                if (FuncPMC.GetCurrSpeed(axis) < 1)
                {
                    jogWidthSite = true;
                    jogWidthSiteNarrow = true;

                    FuncPMC.ContMove(axis, (int)numWidthSpeedSite.Value);
                }
            }
        }

        private void pbWidthNarrow_MouseUp(object sender, MouseEventArgs e)
        {
            int index = -1;
            Array array = Enum.GetValues(typeof(enumPMCAxis));
            for (int i = 0; i < array.Length; i++)
            {
                if (activePanelWidth.Replace("pnWidth", "") == ((enumPMCAxis)i).ToString())
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                FuncMotion.StopAllJog();
                //enumPMCAxis axis = (enumPMCAxis)index;
                //if (FuncPMC.GetCurrSpeed(axis) < 1)
                //{
                //    FuncPMC.SlowStop(axis);
                //}
                jogWidthSite = false;
            }
        }

        private void pbWidthWide_MouseDown(object sender, MouseEventArgs e)
        {
            int index = -1;
            Array array = Enum.GetValues(typeof(enumPMCAxis));
            for (int i = 0; i < array.Length; i++)
            {
                if (activePanelWidth.Replace("pnWidth", "") == ((enumPMCAxis)i).ToString())
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                enumPMCAxis axis = (enumPMCAxis)index;
                if (FuncPMC.GetCurrSpeed(axis) < 1)
                {
                    jogWidthSite = true;
                    jogWidthSiteWide = true;

                    FuncPMC.ContMove(axis, 0 - (int)numWidthSpeedSite.Value);
                }
            }
        }

        private void pbWidthWide_MouseUp(object sender, MouseEventArgs e)
        {
            int index = -1;
            Array array = Enum.GetValues(typeof(enumPMCAxis));
            for (int i = 0; i < array.Length; i++)
            {
                if (activePanelWidth.Replace("pnWidth", "") == ((enumPMCAxis)i).ToString())
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                FuncMotion.StopAllJog();
                //enumPMCAxis axis = (enumPMCAxis)index;
                //if (FuncPMC.GetCurrSpeed(axis) < 1)
                //{
                //    FuncPMC.SlowStop(axis);
                //}
                jogWidthSite = false;
            }
        }

        private void btnWidthCalcOffset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < GlobalVar.WidthOffset.Length; i++)
            {
                Control[] controls = Controls.Find("lblSetPosWidth" + ((enumPMCAxis)i).ToString(), true);
                if (controls.Length > 0 &&
                    ((Label)controls[0]).Name != "lblSetPosWidthInputLift")
                {
                    ((Label)controls[0]).Text = (double.Parse(lblSetPosWidthInputLift.Text) + GlobalVar.WidthOffset[i]).ToString("F2");
                }
            }
        }

        private void btnWidthSaveOffset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < GlobalVar.WidthOffset.Length; i++)
            {
                Control[] controls = Controls.Find("lblSetPosWidth" + ((enumPMCAxis)i).ToString(), true);
                if (controls.Length > 0)
                {
                    GlobalVar.WidthOffset[i] = double.Parse(lblSetPosWidthInputLift.Text) - double.Parse(((Label)controls[0]).Text);
                }
            }
        }


        private void btnMoveWidth_Click(object sender, EventArgs e)
        {

        }

        private void btnClampWidth_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 값 변환 이벤트

        private void cbWidthAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWidthAdvanced.Checked)
            {
                cbWidthAdvanced.Text = "Advanced Mode";
                //lblInputLiftWidth.Text = "InputLift Width";
            }
            else
            {
                cbWidthAdvanced.Text = "Basic Mode";
                //lblInputLiftWidth.Text = "OverAll Width";
                pnWidth_Click(pnWidthInputLift, e);
            }
            bool visible = cbWidthAdvanced.Checked;

            //pnWidthBufferConveyor.Visible = visible;
            //pnWidthInputConveyor.Visible = visible;
            //pnWidthLoadingConveyor.Visible = visible;
            //pnWidthUnloadingConveyor.Visible = visible;
            //pnWidthOutputLift.Visible = visible;
            //pnWidthInputJig.Visible = visible;
            //pnWidthOutputJig.Visible = visible;
            //for (int i = 1; i <= 21; i++)
            //{
            //    Controls.Find("pnWidthSite" + i,true)[0].Visible = visible;
            //}

            //lblPCBWidth.Visible = visible;
            //lblPCBWidth2.Visible = visible;
            //numPCBWidth.Visible = visible;
            btnWidthCalcOffset.Visible = visible;
            btnWidthSaveOffset.Visible = visible;
        }


        #endregion

        private void Width_Shown(object sender, EventArgs e)
        {
            debug("shown");
        }
    }
}
