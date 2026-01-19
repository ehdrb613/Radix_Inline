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

    public partial class Position : Form
    {
        #region 로컬 변수
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;
        private System.Threading.Timer timerMotor; // Thread Timer
        private bool timerMotorDoing = false;
        private System.Threading.Timer timerJog; // Thread Timer
        private bool timerJogDoing = false;

        private bool jogRobotX = false;
        private bool jogRobotXLeft = false;
        private bool jogRobotXRight = false;
        private bool jogRobotY = false;
        private bool jogRobotYRear = false;
        private bool jogRobotYFront = false;
        private bool jogRobotZ = false;
        private bool jogRobotZUp = false;
        private bool jogRobotZDown = false;

        private bool jogInputLift = false;
        private bool jogInputLiftUp = false;
        private bool jogInputLiftDown = false;

        private bool jogOutputLift = false;
        private bool jogOutputLiftUp = false;
        private bool jogOutputLiftDown = false;

        private bool jogVisionMove = false;
        private bool jogVisionMoveRear = false;
        private bool jogVisionMoveFront = false;

        private string activePanelPos = "pnPosLoadingConveyor";

        private bool siteAdvanced = false; // 사이트 상세 모드

        enumTabMain beforeTabMain = GlobalVar.TabMain;
        enumTabTeaching beforeTabTeaching = GlobalVar.TabTeaching;

        #endregion


        #region 초기화 함수
        public Position()
        {
            InitializeComponent();
        }

        private void Position_Load(object sender, EventArgs e)
        {
            try
            {
                #region 화면 제어용 쓰레드 타이머 시작
                //*
                //TimerCallback CallBackUI = new TimerCallback(TimerUI);
                //timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);

                TimerCallback CallBackMotor = new TimerCallback(TimerMotor);
                timerMotor = new System.Threading.Timer(CallBackMotor, false, 0, 100);

                TimerCallback CallBackJog = new TimerCallback(TimerJog);
                timerJog = new System.Threading.Timer(CallBackJog, false, 0, 100);
                //*/
                #endregion
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

        private void Position_Leave(object sender, EventArgs e)
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
            if (GlobalVar.TabMain != enumTabMain.Teaching ||
                GlobalVar.TabTeaching != enumTabTeaching.Position) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerJogDoing = false;
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
                GlobalVar.TabTeaching != enumTabTeaching.Position) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
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
                    FuncForm.SetServoStateColor(pbAxisStatusXRobot, enumServoAxis.BaseRobot_T);
                    FuncForm.SetServoStateColor(pbAxisStatusYRobot, enumServoAxis.BaseRobot_T);
                    FuncForm.SetServoStateColor(pbAxisStatusZRobot, enumServoAxis.BaseRobot_T, enumServoAxis.BaseRobot_T);
                    FuncForm.SetServoStateColor(pbAxisStatusInputLift, enumServoAxis.BaseRobot_T);
                    FuncForm.SetServoStateColor(pbAxisStatusVision, enumServoAxis.BaseRobot_T);
                    FuncForm.SetServoStateColor(pbAxisStatusOutputLift, enumServoAxis.BaseRobot_T);
                    
                    #region 모터 현재 위치 표시
                    lblRobotXPos.Text = FuncMotion.GetRealPosition((int)enumServoAxis.BaseRobot_T).ToString("F2");
                    lblRobotYPos.Text = FuncMotion.GetRealPosition((int)enumServoAxis.BaseRobot_T).ToString("F2");
                    lblRobotZPos.Text = FuncMotion.GetRealPosition((int)enumServoAxis.BaseRobot_T).ToString("F2");

                    lblCurrPosInputLift.Text = FuncMotion.GetRealPosition((int)enumServoAxis.BaseRobot_T).ToString("F2");
                    lblCurrPosVision.Text = FuncMotion.GetRealPosition((int)enumServoAxis.BaseRobot_T).ToString("F2");
                    lblCurrPosOutputLift.Text = FuncMotion.GetRealPosition((int)enumServoAxis.BaseRobot_T).ToString("F2");
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
            if (beforeTabMain == enumTabMain.Teaching && beforeTabTeaching == enumTabTeaching.Position &&
                (GlobalVar.TabMain != enumTabMain.Teaching || GlobalVar.TabTeaching != enumTabTeaching.Position))
            {
                FuncMotion.StopAllJog();
            }
            beforeTabMain = GlobalVar.TabMain;
            beforeTabTeaching = GlobalVar.TabTeaching;
            #endregion

            if (GlobalVar.TabMain != enumTabMain.Teaching ||
                GlobalVar.TabTeaching != enumTabTeaching.Position) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
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
                #region 로봇x축
                if (jogRobotX)
                {
                    if (!jogRobotXLeft &&
                        !jogRobotXRight)
                    {
                        if (GlobalVar.E_Stop ||
                            GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Errored)
                        {
                            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
                            jogRobotX = false;
                        }
                        else if (Math.Abs(GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Velocity) > 0)
                        {
                            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
                        }
                        else
                        {
                            jogRobotX = false;
                        }
                    }
                }
                #endregion
                #region 로봇Y축
                if (jogRobotY)
                {
                    if (!jogRobotYRear &&
                        !jogRobotYFront)
                    {
                        if (GlobalVar.E_Stop ||
                            GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Errored)
                        {
                            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
                            jogRobotY = false;
                        }
                        else if (Math.Abs(GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Velocity) > 0)
                        {
                            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
                        }
                        else
                        {
                            jogRobotY = false;
                        }
                    }
                }
                #endregion
                #region 로봇Z축
                if (jogRobotZ) // 축 두개
                {
                    if (!jogRobotZDown &&
                        !jogRobotZUp)
                    {
                        if (GlobalVar.E_Stop ||
                            GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Errored ||
                            GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Errored)
                        {
                            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
                            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
                            jogRobotY = false;
                        }
                        else if (Math.Abs(GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Velocity) > 0)
                        {
                            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
                        }
                        else if (Math.Abs(GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Velocity) > 0)
                        {
                            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
                        }
                        else
                        {
                            jogRobotY = false;
                        }
                    }
                }
                #endregion

                #region InputLift
                if (jogInputLift)
                {
                    if (!jogInputLiftUp &&
                        !jogInputLiftDown)
                    {
                        enumServoAxis axis = enumServoAxis.BaseRobot_T;
                        if (GlobalVar.E_Stop ||
                            GlobalVar.AxisStatus[(int)axis].Errored)
                        {
                            FuncMotion.MoveStop((int)axis);
                            jogInputLift = false;
                        }
                        else if (!GlobalVar.AxisStatus[(int)axis].StandStill)
                        {
                            FuncMotion.MoveStop((int)axis);
                        }
                        else
                        {
                            jogInputLift = false;
                        }
                    }
                }
                #endregion
                #region VisionMove
                if (jogVisionMove)
                {
                    if (!jogVisionMoveFront &&
                        !jogVisionMoveRear)
                    {
                        enumServoAxis axis = enumServoAxis.BaseRobot_T;
                        if (GlobalVar.E_Stop ||
                            GlobalVar.AxisStatus[(int)axis].Errored)
                        {
                            FuncMotion.MoveStop((int)axis);
                            jogVisionMove = false;
                        }
                        else if (!GlobalVar.AxisStatus[(int)axis].StandStill)
                        {
                            FuncMotion.MoveStop((int)axis);
                        }
                        else
                        {
                            jogVisionMove = false;
                        }
                    }
                }
                #endregion
                #region OutputLift
                if (jogOutputLift)
                {
                    if (!jogOutputLiftUp &&
                        !jogOutputLiftDown)
                    {
                        enumServoAxis axis = enumServoAxis.BaseRobot_T;
                        if (GlobalVar.E_Stop ||
                            GlobalVar.AxisStatus[(int)axis].Errored)
                        {
                            FuncMotion.MoveStop((int)axis);
                            jogOutputLift = false;
                        }
                        else if (!GlobalVar.AxisStatus[(int)axis].StandStill)
                        {
                            FuncMotion.MoveStop((int)axis);
                        }
                        else
                        {
                            jogOutputLift = false;
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
                for (int i = 0; i < Enum.GetValues(typeof(enumTeachingPos)).Length; i++)
                {
                    Control[] controls = Controls.Find("lblRobotX" + ((enumTeachingPos)i).ToString(), true);
                    if (controls != null &&
                        controls.Length > 0)
                    {
                        ((Label)(Controls.Find("lblRobotX" + ((enumTeachingPos)i).ToString(), true)[0])).Text = GlobalVar.AutoInline_TeachingPos[i].x.ToString("F2");
                        ((Label)(Controls.Find("lblRobotY" + ((enumTeachingPos)i).ToString(), true)[0])).Text = GlobalVar.AutoInline_TeachingPos[i].y.ToString("F2");
                        ((Label)(Controls.Find("lblRobotZ" + ((enumTeachingPos)i).ToString(), true)[0])).Text = GlobalVar.AutoInline_TeachingPos[i].z.ToString("F2");
                    }
                }

                // 전역설정
                //numRobotSpeed.Value = (decimal)GlobalVar.AutoInline_RobotSpeed;
                //numRobotSpeedSlow.Value = (decimal)GlobalVar.AutoInline_RobotSpeedSlow;
                //numRobotAccDec.Value = (decimal)GlobalVar.AutoInline_RobotAccDec;

                //numSiteOffsetX.Value = (decimal)GlobalVar.SiteOffset.x;
                //numSiteOffsetZ.Value = (decimal)GlobalVar.SiteOffset.z;
                //numRobotInputOutputOffset.Value = (decimal)GlobalVar.AutoInline_RobotInputOutputOffsetX;
                //numRobotGripShift.Value = (decimal)GlobalVar.AutoInline_RobotGripShift;
                //numRobotWidthOffset.Value = (decimal)GlobalVar.AutoInline_RobotWidthOffset;

                lblInputLiftDown.Text = GlobalVar.AutoInline_LiftPos[(int)enumLiftName.InputLift, (int)enumLiftPos.Down_Front].ToString("F2");
                lblInputLiftScan.Text = GlobalVar.AutoInline_LiftPos[(int)enumLiftName.InputLift, (int)enumLiftPos.Scan].ToString("F2");
                lblInputLiftUp.Text = GlobalVar.AutoInline_LiftPos[(int)enumLiftName.InputLift, (int)enumLiftPos.Up_Rear].ToString("F2");
                lblVisionFront.Text = GlobalVar.AutoInline_LiftPos[(int)enumLiftName.Scan, (int)enumLiftPos.Down_Front].ToString("F2");
                lblVisionRear.Text = GlobalVar.AutoInline_LiftPos[(int)enumLiftName.Scan, (int)enumLiftPos.Up_Rear].ToString("F2");
                lblOutputLiftDown.Text = GlobalVar.AutoInline_LiftPos[(int)enumLiftName.OutputLift, (int)enumLiftPos.Down_Front].ToString("F2");
                lblOutputLiftUp.Text = GlobalVar.AutoInline_LiftPos[(int)enumLiftName.OutputLift, (int)enumLiftPos.Up_Rear].ToString("F2");

                //numServoSpeedInputLift.Value = (decimal)GlobalVar.AutoInline_LiftSpeed;
                //numServoSpeedOutputLift.Value = (decimal)GlobalVar.AutoInline_LiftSpeed;
                //numServoSpeedVision.Value = (decimal)GlobalVar.AutoInline_LiftSpeed;

                numPosInputLift.Value = (decimal)GlobalVar.AutoInline_LiftPos[(int)enumLiftName.InputLift, (int)enumLiftPos.Up_Rear];
                numPosOutputLift.Value = (decimal)GlobalVar.AutoInline_LiftPos[(int)enumLiftName.OutputLift, (int)enumLiftPos.Up_Rear];
                numPosVision.Value = (decimal)GlobalVar.AutoInline_LiftPos[(int)enumLiftName.Scan, (int)enumLiftPos.Up_Rear];
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


                for (int i = 0; i < Enum.GetValues(typeof(enumTeachingPos)).Length; i++)
                {
                    Control[] controls = Controls.Find("lblRobotX" + ((enumTeachingPos)i).ToString(), true);
                    if (controls != null &&
                        controls.Length > 0)
                    {
                        GlobalVar.AutoInline_TeachingPos[i].x = double.Parse(((Label)(Controls.Find("lblRobotX" + ((enumTeachingPos)i).ToString(), true)[0])).Text);
                        GlobalVar.AutoInline_TeachingPos[i].y = double.Parse(((Label)(Controls.Find("lblRobotY" + ((enumTeachingPos)i).ToString(), true)[0])).Text);
                        GlobalVar.AutoInline_TeachingPos[i].z = double.Parse(((Label)(Controls.Find("lblRobotZ" + ((enumTeachingPos)i).ToString(), true)[0])).Text);
                    }
                }

                // 전역설정
                //GlobalVar.AutoInline_RobotSpeed = (double)numRobotSpeed.Value;
                //GlobalVar.AutoInline_RobotSpeedSlow = (double)numRobotSpeedSlow.Value;
                //GlobalVar.AutoInline_RobotAccDec = (double)numRobotAccDec.Value;

                //GlobalVar.SiteOffset.x = (double)numSiteOffsetX.Value;
                //GlobalVar.SiteOffset.z = (double)numSiteOffsetZ.Value;
                //GlobalVar.AutoInline_RobotInputOutputOffsetX = (double)numRobotInputOutputOffset.Value;
                //GlobalVar.AutoInline_RobotGripShift = (double)numRobotGripShift.Value;
                //GlobalVar.AutoInline_RobotWidthOffset = (double)numRobotWidthOffset.Value;

                GlobalVar.AutoInline_LiftPos[(int)enumLiftName.InputLift, (int)enumLiftPos.Down_Front] = double.Parse(lblInputLiftDown.Text);
                GlobalVar.AutoInline_LiftPos[(int)enumLiftName.InputLift, (int)enumLiftPos.Scan] = double.Parse(lblInputLiftScan.Text);
                GlobalVar.AutoInline_LiftPos[(int)enumLiftName.InputLift, (int)enumLiftPos.Up_Rear] = double.Parse(lblInputLiftUp.Text);
                GlobalVar.AutoInline_LiftPos[(int)enumLiftName.Scan, (int)enumLiftPos.Down_Front] = double.Parse(lblVisionFront.Text);
                GlobalVar.AutoInline_LiftPos[(int)enumLiftName.Scan, (int)enumLiftPos.Up_Rear] = double.Parse(lblVisionRear.Text);
                GlobalVar.AutoInline_LiftPos[(int)enumLiftName.OutputLift, (int)enumLiftPos.Down_Front] = double.Parse(lblOutputLiftDown.Text);
                GlobalVar.AutoInline_LiftPos[(int)enumLiftName.OutputLift, (int)enumLiftPos.Up_Rear] = double.Parse(lblOutputLiftUp.Text);
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
            Util.Debug("frmPosition : " + str);
        }

        private void ClearPanelColor(bool width)
        {
            foreach (Panel pn in pnPos.Controls.OfType<Panel>())
            {
                if (pn.Name.Contains("pnPos"))
                {
                    pn.BackColor = Color.Transparent;
                }
            }
        }

        private void setPanelColor(object sender)
        {
            ((Panel)sender).BackColor = Color.Lime;
            string name = ((Panel)sender).Name;

            activePanelPos = name;
            Console.WriteLine(((Panel)sender).Name);
        }

        #endregion

        #region 조그버튼 처리
        private void pbJogLeftRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogRobotX = true;
            jogRobotXLeft = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  -99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numRobotSpeed.Value));
        }

        private void pbJogLeftRobot_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            //RTEX.MoveStop((int)enumServoAxis.RobotX);
            jogRobotXLeft = false;
        }

        private void pbJogRightRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogRobotX = true;
            jogRobotXRight = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numRobotSpeed.Value));
        }

        private void pbJogRightRobot_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            //RTEX.MoveStop((int)enumServoAxis.RobotX);
            jogRobotXRight = false;
        }

        private void pbJogBackRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogRobotY = true;
            jogRobotYRear = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numRobotSpeed.Value));
        }

        private void pbJogBackRobot_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            //RTEX.MoveStop((int)enumServoAxis.RobotY);
            jogRobotYRear = false;
        }

        private void pbJogFrontRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogRobotY = true;
            jogRobotYFront = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  -99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numRobotSpeed.Value));
        }

        private void pbJogFrontRobot_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            //RTEX.MoveStop((int)enumServoAxis.RobotY);
            jogRobotYFront = false;
        }

        private void pbJogUpRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogRobotZ = true;
            jogRobotZUp = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numRobotSpeed.Value));
            axis = (int)enumServoAxis.BaseRobot_T;
            FuncMotion.MoveAbsolute((uint)axis,
                  99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numRobotSpeed.Value));
        }

        private void pbJogUpRobot_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            //RTEX.MoveStop((int)enumServoAxis.RobotZ1);
            //RTEX.MoveStop((int)enumServoAxis.RobotZ2);
            jogRobotZUp = false;
        }

        private void pbJogDownRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogRobotZ = true;
            jogRobotZDown = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  -99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numRobotSpeed.Value));
            axis = (int)enumServoAxis.BaseRobot_T;
            FuncMotion.MoveAbsolute((uint)axis,
                  -99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numRobotSpeed.Value));
        }

        private void pbJogDownRobot_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            //RTEX.MoveStop((int)enumServoAxis.RobotZ1);
            //RTEX.MoveStop((int)enumServoAxis.RobotZ2);
            jogRobotZDown = false;
        }

        private void pbJogUpInputLift_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogInputLift = true;
            jogInputLiftUp = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numServoSpeedInputLift.Value));
        }

        private void pbJogUpInputLift_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            jogInputLiftUp = false;
        }

        private void pbJogDownInputLift_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogInputLift = true;
            jogInputLiftDown = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  -99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numServoSpeedInputLift.Value));

        }

        private void pbJogDownInputLift_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            jogInputLiftDown = false;

        }

        private void pbJogRearVision_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogVisionMove = true;
            jogVisionMoveRear = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  -99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numServoSpeedVision.Value));
        }

        private void pbJogRearVision_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            jogVisionMoveRear = false;
        }

        private void pbJogFrontVision_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogVisionMove = true;
            jogVisionMoveFront = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numServoSpeedVision.Value));
        }

        private void pbJogFrontVision_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            jogVisionMoveFront = false;
        }

        private void pbJogUpOutputLift_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogOutputLift = true;
            jogOutputLiftUp = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numServoSpeedOutputLift.Value));
        }

        private void pbJogUpOutputLift_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            jogOutputLiftUp = false;
        }

        private void pbJogDownOutputLift_MouseDown(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();

            int axis = (int)enumServoAxis.BaseRobot_T;
            jogOutputLift = true;
            jogOutputLiftDown = true;
            FuncMotion.MoveAbsolute((uint)axis,
                  -99999999999,
                  FuncMotion.GetRealSpeed((int)axis, (double)numServoSpeedOutputLift.Value));
        }

        private void pbJogDownOutputLift_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.StopAllJog();
            jogOutputLiftDown = false;
        }
        #endregion

        #region 버튼 클릭 이벤트
        private void pnPos_Click(object sender, EventArgs e)
        {
            if (sender.GetType() == typeof(Panel))
            {
                if (!cbRobotAdvanced.Checked &&
                    ((Panel)sender).Name.Replace("pnPos", "") != "LoadingConveyor")
                {
                    FuncWin.TopMessageBox("In Basic Mode, Only LoadingConveyor position Teaching is needed. And other teaching will be calculated automactically.");
                    return;
                }
                ClearPanelColor(false);
                setPanelColor(sender);
                // pnPosSite1 ==> lblRobotXSite1
                numRobotX.Value = (decimal)double.Parse((((Label)(Controls.Find(((Panel)sender).Name.Replace("pnPos", "lblRobotX"), true)[0])).Text));
                numRobotY.Value = (decimal)double.Parse((((Label)(Controls.Find(((Panel)sender).Name.Replace("pnPos", "lblRobotY"), true)[0])).Text));
                numRobotZ.Value = (decimal)double.Parse((((Label)(Controls.Find(((Panel)sender).Name.Replace("pnPos", "lblRobotZ"), true)[0])).Text));
                activePanelPos = ((Panel)sender).Name;
            }
            else
            if (sender.GetType() == typeof(Label))
            {
                if (!cbRobotAdvanced.Checked &&
                    ((Label)sender).Name.Replace("lblRobotX", "").Replace("lblRobotY", "").Replace("lblRobotZ", "").Replace("lbl", "") != "LoadingConveyor")
                {
                    FuncWin.TopMessageBox("In Basic Mode, Only LoadingConveyor position Teaching is needed. And other teaching will be calculated automactically.");
                    return;
                }
                ClearPanelColor(false);
                setPanelColor(((Label)sender).Parent);
                numRobotX.Value = (decimal)double.Parse((((Label)(Controls.Find(((Panel)(((Label)sender).Parent)).Name.Replace("pnPos", "lblRobotX"), true)[0])).Text));
                numRobotY.Value = (decimal)double.Parse((((Label)(Controls.Find(((Panel)(((Label)sender).Parent)).Name.Replace("pnPos", "lblRobotY"), true)[0])).Text));
                numRobotZ.Value = (decimal)double.Parse((((Label)(Controls.Find(((Panel)(((Label)sender).Parent)).Name.Replace("pnPos", "lblRobotZ"), true)[0])).Text));
                activePanelPos = (((Label)sender).Parent).Name;
            }
        }

        private void pbRobotApply_Click(object sender, EventArgs e)
        {
            ((Label)(Controls.Find(activePanelPos.Replace("pnPos", "lblRobotX"), true)[0])).Text = lblRobotXPos.Text; //((double)numRobotX.Value).ToString("F2");
            ((Label)(Controls.Find(activePanelPos.Replace("pnPos", "lblRobotY"), true)[0])).Text = lblRobotYPos.Text; //((double)numRobotY.Value).ToString("F2");
            ((Label)(Controls.Find(activePanelPos.Replace("pnPos", "lblRobotZ"), true)[0])).Text = lblRobotZPos.Text; //((double)numRobotZ.Value).ToString("F2");

            #region 기본모드에서는 system offset으로 나머지 모두에 적용
            if (!cbRobotAdvanced.Checked)
            {
                for (int i = 0; i < GlobalVar.PositionOffset.Length; i++)
                {
                    string posName = ((enumTeachingPos)i).ToString();
                    if (posName != "LoadingConveyor")
                    {
                        Control[] controls = Controls.Find("pnPos" + posName, true);
                        if (controls != null &&
                            controls.Length > 0)
                        {
                            ((Label)Controls.Find("lblRobotX" + posName, true)[0]).Text = (double.Parse(lblRobotXLoadingConveyor.Text) + GlobalVar.PositionOffset[i].x).ToString("F2");
                            ((Label)Controls.Find("lblRobotY" + posName, true)[0]).Text = (double.Parse(lblRobotYLoadingConveyor.Text) + GlobalVar.PositionOffset[i].y).ToString("F2");
                            ((Label)Controls.Find("lblRobotZ" + posName, true)[0]).Text = (double.Parse(lblRobotZLoadingConveyor.Text) + GlobalVar.PositionOffset[i].z).ToString("F2");
                        }
                    }
                }

            }
            #endregion

        }

        private void pbRobotMove_Click(object sender, EventArgs e)
        {
            //if (numRobotX.Value == 0 ||
            //    numRobotY.Value == 0 ||
            //    numRobotZ.Value == 0)
            //{
            //    Util.TopMessageBox("input position first");
            //    return;
            //}
            //if (FuncRobot.MoveXYZRobot((double)numRobotSpeed.Value, (double)numRobotX.Value, (double)numRobotY.Value, (double)numRobotZ.Value))
            //{
            //}
        }

        private void pbRobotHome_Click(object sender, EventArgs e)
        {
            //FuncRobot.HomeXYZRobot();
        }

        private void pbRobotStop_Click(object sender, EventArgs e)
        {
            //FuncRobot.StopRobot();
        }

        private void pbAxisStatusXRobot_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoReset((uint)enumServoAxis.BaseRobot_T);
            if (GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Disabled)
            {
                FuncMotion.ServoOn((int)enumServoAxis.BaseRobot_T, true);
            }
        }

        private void pbAxisStatusYRobot_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoReset((uint)enumServoAxis.BaseRobot_T);
            if (GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Disabled)
            {
                FuncMotion.ServoOn((int)enumServoAxis.BaseRobot_T, true);
            }
        }

        private void pbAxisStatusZRobot_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoReset((uint)enumServoAxis.BaseRobot_T);
            if (GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Disabled)
            {
                FuncMotion.ServoOn((int)enumServoAxis.BaseRobot_T, true);
            }
            FuncMotion.ServoReset((uint)enumServoAxis.BaseRobot_T);
            if (GlobalVar.AxisStatus[(int)enumServoAxis.BaseRobot_T].Disabled)
            {
                FuncMotion.ServoOn((int)enumServoAxis.BaseRobot_T, true);
            }
        }

        private void btnSaveSiteOffset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < GlobalVar.PositionOffset.Length; i++)
            {
                Control[] controls = Controls.Find("pnPos" + ((enumPMCAxis)i).ToString(), true);
                if (controls.Length > 0)
                {
                    GlobalVar.PositionOffset[i].x = double.Parse(lblRobotXLoadingConveyor.Text) - double.Parse(((Label)Controls.Find("lblRobotX" + ((enumPMCAxis)i).ToString(), true)[0]).Text);
                    GlobalVar.PositionOffset[i].y = double.Parse(lblRobotYLoadingConveyor.Text) - double.Parse(((Label)Controls.Find("lblRobotY" + ((enumPMCAxis)i).ToString(), true)[0]).Text);
                    GlobalVar.PositionOffset[i].z = double.Parse(lblRobotZLoadingConveyor.Text) - double.Parse(((Label)Controls.Find("lblRobotZ" + ((enumPMCAxis)i).ToString(), true)[0]).Text);
                }
            }
        }

        private void btnCalcSiteOffset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < GlobalVar.PositionOffset.Length; i++)
            {
                string posName = ((enumTeachingPos)i).ToString();
                Control[] controls = Controls.Find("pnPos" + posName, true);
                if (controls != null &&
                    controls.Length > 0 &&
                    ((Panel)controls[0]).Name != "pnPosLoadingConveyor")
                {
                    ((Label)Controls.Find("lblRobotX" + posName, true)[0]).Text = (double.Parse(lblRobotXLoadingConveyor.Text) + GlobalVar.PositionOffset[i].x).ToString("F2");
                    ((Label)Controls.Find("lblRobotY" + posName, true)[0]).Text = (double.Parse(lblRobotYLoadingConveyor.Text) + GlobalVar.PositionOffset[i].y).ToString("F2");
                    ((Label)Controls.Find("lblRobotZ" + posName, true)[0]).Text = (double.Parse(lblRobotZLoadingConveyor.Text) + GlobalVar.PositionOffset[i].z).ToString("F2");
                }
            }
        }

        private void cbInputLiftUp_CheckedChanged(object sender, EventArgs e)
        {
            cbInputLiftUp.Checked = true;
            cbInputLiftDown.Checked = false;
            cbInputLiftScan.Checked = false;
        }

        private void cbInputLiftDown_CheckedChanged(object sender, EventArgs e)
        {
            cbInputLiftUp.Checked = false;
            cbInputLiftDown.Checked = true;
            cbInputLiftScan.Checked = false;
        }

        private void cbInputLiftScan_CheckedChanged(object sender, EventArgs e)
        {
            cbInputLiftUp.Checked = false;
            cbInputLiftDown.Checked = false;
            cbInputLiftScan.Checked = true;
        }

        private void cbVisionRear_CheckedChanged(object sender, EventArgs e)
        {
            cbVisionRear.Checked = true;
            cbVisionFront.Checked = false;
        }

        private void cbVisionFront_CheckedChanged(object sender, EventArgs e)
        {
            cbVisionRear.Checked = false;
            cbVisionFront.Checked = true;
        }

        private void cbOutputLiftUp_CheckedChanged(object sender, EventArgs e)
        {
            cbOutputLiftUp.Checked = true;
            cbOutputLiftDown.Checked = false;
        }

        private void cbOutputLiftDown_CheckedChanged(object sender, EventArgs e)
        {
            cbOutputLiftUp.Checked = false;
            cbOutputLiftDown.Checked = true;
        }

        private void pbMoveInputLift_Click(object sender, EventArgs e)
        {
            enumServoAxis axis = enumServoAxis.BaseRobot_T;
            if (GlobalVar.AxisStatus[(int)axis].StandStill)
            {
                FuncMotion.MoveAbsolute((uint)axis,
                    FuncMotion.GetRealPulse((int)axis, (double)numPosInputLift.Value),
                    FuncMotion.GetRealPulse((int)axis, (double)numServoSpeedInputLift.Value));
            }
        }

        private void pbHomeInputLift_Click(object sender, EventArgs e)
        {
            enumServoAxis axis = enumServoAxis.BaseRobot_T;
            if (GlobalVar.AxisStatus[(int)axis].StandStill)
            {
                FuncMotion.MoveHome((uint)axis);
            }
        }

        private void pbStopInputLift_Click(object sender, EventArgs e)
        {
            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
        }

        private void pbMoveVision_Click(object sender, EventArgs e)
        {
            enumServoAxis axis = enumServoAxis.BaseRobot_T;
            if (GlobalVar.AxisStatus[(int)axis].StandStill)
            {
                FuncMotion.MoveAbsolute((uint)axis,
                    FuncMotion.GetRealPulse((int)axis, (double)numPosVision.Value),
                    FuncMotion.GetRealPulse((int)axis, (double)numServoSpeedVision.Value));
            }
        }

        private void pbHomeVision_Click(object sender, EventArgs e)
        {
            enumServoAxis axis = enumServoAxis.BaseRobot_T;
            if (GlobalVar.AxisStatus[(int)axis].StandStill)
            {
                FuncMotion.MoveHome((uint)axis);
            }
        }

        private void pbStopVision_Click(object sender, EventArgs e)
        {
            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
        }

        private void pbMoveOutputLift_Click(object sender, EventArgs e)
        {
            enumServoAxis axis = enumServoAxis.BaseRobot_T;
            if (GlobalVar.AxisStatus[(int)axis].StandStill)
            {
                FuncMotion.MoveAbsolute((uint)axis,
                    FuncMotion.GetRealPulse((int)axis, (double)numPosOutputLift.Value),
                    FuncMotion.GetRealPulse((int)axis, (double)numServoSpeedOutputLift.Value));
            }
        }

        private void pbHomeOutputLift_Click(object sender, EventArgs e)
        {
            enumServoAxis axis = enumServoAxis.BaseRobot_T;
            if (GlobalVar.AxisStatus[(int)axis].StandStill)
            {
                FuncMotion.MoveHome((uint)axis);
            }
        }

        private void pbStopOutputLift_Click(object sender, EventArgs e)
        {
            FuncMotion.MoveStop((int)enumServoAxis.BaseRobot_T);
        }

        private void pbInputLiftApply_Click(object sender, EventArgs e)
        {
            if (cbInputLiftDown.Checked)
            {
                lblInputLiftDown.Text = lblCurrPosInputLift.Text;
            }
            else if (cbInputLiftUp.Checked)
            {
                lblInputLiftUp.Text = lblCurrPosInputLift.Text;
            }
            else
            {
                lblInputLiftScan.Text = lblCurrPosInputLift.Text;
            }
        }

        private void pbVisionApply_Click(object sender, EventArgs e)
        {
            if (cbVisionRear.Checked)
            {
                lblVisionRear.Text = lblCurrPosVision.Text;
            }
            else
            {
                lblVisionFront.Text = lblCurrPosVision.Text;
            }
        }

        private void pbOutputLiftApply_Click(object sender, EventArgs e)
        {
            if (cbOutputLiftDown.Checked)
            {
                lblOutputLiftDown.Text = lblCurrPosOutputLift.Text;
            }
            else
            {
                lblOutputLiftUp.Text = lblCurrPosOutputLift.Text;
            }
        }
        #endregion

        #region 값 변환 이벤트
        private void cbRobotAdvanced_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRobotAdvanced.Checked)
            {
                cbRobotAdvanced.Text = "Advanced Mode";
            }
            else
            {
                cbRobotAdvanced.Text = "Basic Mode";
                pnPos_Click(pnPosLoadingConveyor, e);
            }
            bool visible = cbRobotAdvanced.Checked;

            btnCalcSiteOffset.Visible = visible;
            btnSaveSiteOffset.Visible = visible;
            //for (int i = 2; i <= 21; i++)
            //{
            //    Controls.Find("pnPosSite" + i, true)[0].Visible = visible;
            //}
        }



        #endregion

        private void Position_Shown(object sender, EventArgs e)
        {
            debug("shown");
        }
    }
}
