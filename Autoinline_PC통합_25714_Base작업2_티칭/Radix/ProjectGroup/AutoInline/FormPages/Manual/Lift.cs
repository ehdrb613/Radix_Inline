using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent; // ConcurrentQueue
using System.Diagnostics;

namespace Radix.Popup.Manual
{
    /*
     * Manual.cs : 각 파트 및 장비의 수동 운전
     */

    public partial class Lift : Form
    {
        #region 로컬 변수
        private System.Threading.Timer timerUI = null; // Thread Timer
        private bool timerUIDoing = false;
        //private System.Threading.Timer timerMotor; // Thread Timer
        //private bool timerMotorDoing = false;
        private System.Threading.Timer timerJog = null; // Thread Timer
        private bool timerJogDoing = false;

        private bool jogLift = false;
        private bool jogLiftUp = false;
        private bool jogLiftDown = false;

        private string activePanelPos = "pnPosPassLine1"; // 현재 선택한 위치
        private FuncInline.enumLiftPos activePos = FuncInline.enumLiftPos.FrontPassLine; // 티칭배열 인덱스로 사용할 위치값
        private FuncInline.enumServoAxis actionAxis = FuncInline.enumServoAxis.SV02_Lift1; // 선택된 리프트 축
        private int activeLift = 0; // 선택된 리프트 순번
        private bool validPos = true; // 선택 조합이 맞는가?

        private int siteIndex = -1; //처음 선택은 PassLine1이기 때문에 사이트가 없다 //by DG

       FuncInline.enumTabMain beforeTabMain = FuncInline.TabMain;
      FuncInline.enumTabManual beforeTabManual = FuncInline.TabManual;
        bool valueChanged = false;

        private double[,] liftPos = new double[2, Enum.GetValues(typeof(enumLiftPos)).Length]; // 티칭값을 임시로 담아둘 변수. 저장 전에는 갖고만 있다가 저장시 이 값을 저장한다.
        #endregion


        #region 초기화 함수
        public Lift()
        {
            InitializeComponent();
        }

        private void Lift_Load(object sender, EventArgs e)
        {
            try
            {
                #region 화면 제어용 쓰레드 타이머 시작
                //*
                TimerCallback CallBackUI = new TimerCallback(TimerUI);
                timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);

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

        private void Lift_Leave(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun) // 운전중 아니면 모든 모터 정지
            {

                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
                {
                    FuncMotion.MoveStop(i);
                }
                //이부분은 Let's보드로 바꿔야함
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length; i++)
                {
                    //FuncInline.ComPMC[(int)((int)i / 2)].Stop((FuncInline.enumPMCAxis)i);
                }
            }
        }

        #endregion

        #region 타이머 함수
        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Manual ||
                FuncInline.TabManual != FuncInline.enumTabManual.Lift) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerUIDoing = false;
                return;
            }
            try
            {
                /* 화면 변경 timer */
                if (this == null)
                {
                    return;
                }
                if (timerUIDoing)
                {
                    return;
                }
                timerUIDoing = true;
                //timerUI.Dispose();

                if (!GlobalVar.GlobalStop &&
                    this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {

                        #region 관리자 여부에 따라 컨트롤 보이기/숨기기
                        //tbAppName.Enabled = GlobalVar.PwdPass;
                        //numMachineNum.Enabled = GlobalVar.PwdPass;
                        //pbSave.Visible = GlobalVar.PwdPass;
                        #endregion



                        FuncForm.SetServoStateColor(pbAxisStatus, actionAxis);

                        #region 모터 현재 위치 표시
                        lblLiftPos.Text = FuncMotion.GetRealPosition((int)actionAxis).ToString("F2");
                        //lblTeaching.Text = liftPos[activeLift, (int)activePos].ToString("F2");

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


        private void TimerJog(Object state) // 조그제어 쓰레드 타이머 함수
        {
            #region 창 떠나면 조그 멈추기
            if (beforeTabMain == FuncInline.enumTabMain.Manual && beforeTabManual != FuncInline.enumTabManual.Lift &&
                (FuncInline.TabMain != FuncInline.enumTabMain.Manual || FuncInline.TabManual != FuncInline.enumTabManual.Lift))
            {
                FuncInlineMove.StopAllJog();
                //FuncInline.ClearAllSiteAction();
                #region 값 수정 상태로 창 떠나면 알림
                //if (valueChanged)
                //{
                //    FuncInline.TabMain = FuncInline.enumTabMain.Teaching;
                //    FuncInline.TabTeaching = FuncInline.enumTabTeaching.LiftPosition;

                //    valueChanged = false;
                //    if (FuncWin.MessageBoxOK("Robot Teaching changed. Save?"))
                //    {
                //        ApplyAllValue();
                //        Func.SaveTeachingPositionIni();
                //    }
                //}
                #endregion
            }
            beforeTabMain = FuncInline.TabMain;
            beforeTabManual = FuncInline.TabManual;
            #endregion

            if (FuncInline.TabMain != FuncInline.enumTabMain.Manual ||
                FuncInline.TabManual != FuncInline.enumTabManual.Lift) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
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
                //timerJog.Dispose();

                #region JOG 멈추기
                #region 지정 리프트

                FuncMotion.MoveStop((int)actionAxis);
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }
            timerJogDoing = false;
            if (GlobalVar.GlobalStop)
            {
                try
                {
                    timerJog.Dispose();
                }
                catch { }
            }
        }
        #endregion

        #region 설정 관련
     

        public void ApplyAllValue()
        {
            try
            {
                for (int j = 0; j < FuncInline.LiftPos.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.LiftPos.GetLength(1); i++)
                    {
                        // apply건 load건 다시 계산된 건 teaching 좌표에 적용
                        FuncInline.LiftPos[j, i] = liftPos[j, i];
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            valueChanged = false;
        }

        #endregion

        #region 로컬함수
        private void debug(string str)
        {
            Util.Debug("frmPosition : " + str);
        }


        #endregion

        #region 조그버튼 처리

        private void pbJogUpRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();

            #region 마우스 이외의 방법으로 클릭시 동작 방지
            /*
            if (e.Button == MouseButtons.None)
            {
                FuncWin.TopMessageBox("Use mouse at jog action.");
                return;
            }
            //*/

            #endregion


            //axis = (int)FuncInline.enumServoAxis.RobotZ2;
            if (btnPitch.BackColor == Color.Lime)
            {
                //Ecat.MoveRelative((uint)actionAxis,
                //      Util.GetRealSpeed((int)actionAxis, 0 - GlobalVar.RobotJogSpeed), // 좌표 환산 안 한 값이니 좌표값이지만 GetRealPulse 함수를 그대로 쓴다.
                //      Util.GetRealSpeed((int)actionAxis, GlobalVar.RobotJogSpeedMiddle));
                FuncMotion.MoveAbsolute((uint)actionAxis,
                  double.Parse(lblLiftPos.Text) + (double)numPitch.Value,
                  (double)numSpeed.Value);
            }
            else
            {
                jogLift = true;
                jogLiftUp = true;

                /*
                uint duRetCode = 0;

                double dVelocity = Util.GetRealSpeed((int)actionAxis, 0 - GlobalVar.RobotJogSpeed);
                double dAccel = dVelocity * 10;
                double dDecel = dVelocity * 10;

                //++ 지정한 축을 (+)방향으로 지정한 속도/가속도/감속도로 모션구동합니다.
                duRetCode = CAXM.AxmMoveVel((int)actionAxis, dVelocity, dAccel, dDecel);
                if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    MessageBox.Show(String.Format("AxmMoveVel return error[Code:{0:d}]", duRetCode));
                    //*/
                //*
                FuncMotion.MoveAbsolute((uint)actionAxis,10000,(double)numSpeed.Value);

            }
        }

        private void pbJogUpRobot_MouseUp(object sender, MouseEventArgs e)
        {

            if (btnSpeed.BackColor == Color.Lime)
            {
                FuncInlineMove.StopAllJog(true);
                Thread.Sleep(200);
                FuncInlineMove.StopAllJog(true);
                //FuncInlineMove.StopAllJog();
                jogLiftUp = false;
                //Util.StartWatch(jogWatch);
            }
        }

        private void pbJogDownRobot_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();

            #region 마우스 이외의 방법으로 클릭시 동작 방지
            /*
            if (e.Button == MouseButtons.None)
            {
                FuncWin.TopMessageBox("Use mouse at jog action.");
                return;
            }
            //*/

            #endregion


            //axis = (int)FuncInline.enumServoAxis.RobotZ2;
            if (btnPitch.BackColor == Color.Lime)
            {
                //Ecat.MoveRelative((uint)actionAxis,
                //      Util.GetRealSpeed((int)actionAxis, GlobalVar.RobotJogSpeed), // 좌표 환산 안 한 값이니 좌표값이지만 GetRealPulse 함수를 그대로 쓴다.
                //      Util.GetRealSpeed((int)actionAxis, GlobalVar.RobotJogSpeedMiddle));
                FuncMotion.MoveAbsolute((uint)actionAxis,
                  double.Parse(lblLiftPos.Text) - (double)numPitch.Value,
                 (double)numSpeed.Value);
            }
            else
            {
                jogLift = true;
                jogLiftDown = true;
                FuncMotion.MoveAbsolute((uint)actionAxis,
                   0,
                 (double)numSpeed.Value);
            }
        }

        private void pbJogDownRobot_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                FuncInlineMove.StopAllJog(true);
                Thread.Sleep(200);
                FuncInlineMove.StopAllJog(true);
                //FuncInlineMove.StopAllJog();
                jogLiftDown = false;
                //Util.StartWatch(jogWatch);
            }
        }
        #endregion

        #region 버튼 클릭 이벤트

    
        private void pbRobotMove_Click(object sender, EventArgs e)
        {
            if (!validPos)
            {
                FuncWin.TopMessageBox("Check Lift and Position Selection.");
                return;
            }

            #region 초기화 전 실행 금지
            if (GlobalVar.SystemStatus <= enumSystemStatus.Initialize)
            {
                FuncWin.TopMessageBox("Origin first.");
                return;
            }
            #endregion

            #region 도어 열림시 동작 금지
            if (GlobalVar.UseDoor &&
                    (!DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                            !DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                            !DIO.GetDIData(FuncInline.enumDINames.X00_2_Door_Open_Rear_Left) ||
                            !DIO.GetDIData(FuncInline.enumDINames.X02_0_Door_Open_Rear_Right)))
            {
                FuncWin.TopMessageBox("Can not move while doors are opened.");
                return;
            }
            #endregion

            FuncMotion.MoveAbsolute((uint)actionAxis,
                                (double)numPos.Value,
                               (double)numSpeed.Value);
        }

        private void pbRobotHome_Click(object sender, EventArgs e)
        {
            if (!validPos)
            {
                FuncWin.TopMessageBox("Check Lift and Position Selection.");
                return;
            }
            #region 도어 열림시 동작 금지
            if (GlobalVar.UseDoor &&
                    (!DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                            !DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                            !DIO.GetDIData(FuncInline.enumDINames.X00_2_Door_Open_Rear_Left) ||
                            !DIO.GetDIData(FuncInline.enumDINames.X02_0_Door_Open_Rear_Right)))
            {
                FuncWin.TopMessageBox("Can not move while doors are opened.");
                return;
            }
            #endregion

            FuncLog.WriteLog("Lift Home Click : " + (activeLift + 1) + " - " +
                            liftPos[activeLift, (int)activePos]);

           
            FuncInlineMove.MoveHome((uint)actionAxis);

        }

        private void pbRobotStop_Click(object sender, EventArgs e)
        {
            if (!validPos)
            {
                FuncWin.TopMessageBox("Check Lift and Position Selection.");
                return;
            }
            Func.StopRobot();
        }

        private void pbAxisStatusZRobot_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoOn((uint)actionAxis, false);
            FuncMotion.ServoReset((uint)actionAxis);
            //if (GlobalVar.AxisStatus[(int)actionAxis].Disabled)
            //{
            FuncMotion.ServoOn((uint)actionAxis, true);
            //}
        }

        #endregion

        #region 값 변환 이벤트




        #endregion

        private void Position_Shown(object sender, EventArgs e)
        {
            numPos.Value = (decimal)liftPos[0, (int)activePos];
            //debug("shown");


        }



        private void btnPos_Click(object sender, EventArgs e) // 버튼 눌렀을 때
        {
            try
            {
                string name = ((Button)sender).Name;
                if (name.Contains("Site"))
                {
                    int.TryParse(name.Replace("btnPosSite", ""), out siteIndex);
                    siteIndex--;
                }
                else
                {
                    siteIndex = -1;
                }
                activePanelPos = name;
                SetLiftPosition();

                #region 티칭 좌표 선택
                if (validPos)
                {
                    lblTeaching.Text = FuncInline.LiftPos[activeLift, (int)activePos].ToString("F2");
                    numPos.Value = (decimal)FuncInline.LiftPos[activeLift, (int)activePos];
                }
                #endregion
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void SetLiftPosition() // 위치, 리프트 선택 따라서 축 선택
        {
            #region 전체 버튼 색상 지정
            foreach (Control conButton in pnPos.Controls)
            {
                if (conButton.Name.StartsWith("btnPos"))
                {
                    if (conButton.Name == activePanelPos)
                    {
                        ((Button)conButton).BackColor = Color.Lime;
                    }
                    else
                    {
                        ((Button)conButton).BackColor = Color.White;
                    }
                }
            }
            #endregion

            if (activeLift == 0 &&
                btnPosPassLine1.BackColor == Color.Lime &&
                btnLiftUp.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.FrontPassLine; // 패스라인1 에서 FrontLift 상단으로 이송
                validPos = true;
            }
            else if (activeLift == 1 &&
                btnPosPassLine2.BackColor == Color.Lime &&
                btnLiftUp.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.RearPassLine; // FrontLift 상단에서 PassLine 로 이송
                validPos = true;
            }
            else if (activeLift == 1 &&
                btnPosPassLine2.BackColor == Color.Lime &&
                btnLiftDown.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.RearNGLine; // FrontLift 하단에서 NGLine 로 이송
                validPos = true;
            }
          
            else if (siteIndex >= 0 &&
                siteIndex < 20 && // 좌측 렉 사이트
                activeLift == 0) // 좌측 리프트 사용하는 경우만
            {
                if (btnLiftUp.BackColor == Color.Lime) // 리프트1 상단 이용시
                {
                    activePos = FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex;
                }
                else // 리프트 1 하단 이용시
                {
                    activePos = FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex;
                }
                validPos = true;
            }
            else if (siteIndex >= 20 &&
                siteIndex < 40 && // 우측 렉 사이트
                activeLift == 1) // 우측 리프트 사용하는 경우만
            {
                if (btnLiftUp.BackColor == Color.Lime) // 리프트2 상단 이용시
                {
                    activePos = FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex;
                }
                else // 리프트 1 하단 이용시
                {
                    activePos = FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex;
                }
                validPos = true;
            }
            else
            {
                validPos = false;
            }

            #region 티칭 좌표 선택
            if (validPos)
            {
                lblTeaching.Text = FuncInline.LiftPos[activeLift, (int)activePos].ToString("F2");
                numPos.Value = (decimal)FuncInline.LiftPos[activeLift, (int)activePos];
            }
            #endregion

        }

        private void RobotPosition_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                //timerUI.Dispose();
                //timerJog.Dispose();
                //timerMotor.Dispose(); // 사용안함
            }
            catch { }
        }

        private void btnLift1_Click(object sender, EventArgs e)
        {
            btnLift1.BackColor = Color.Lime;
            btnLift2.BackColor = Color.White;
            activeLift = 0;
            actionAxis = FuncInline.enumServoAxis.SV02_Lift1;
            SetLiftPosition();
        }

        private void btnLift2_Click(object sender, EventArgs e)
        {
            btnLift1.BackColor = Color.White;
            btnLift2.BackColor = Color.Lime;
            activeLift = 1;
            actionAxis = FuncInline.enumServoAxis.SV04_Lift2;
            SetLiftPosition();
        }

        private void btnLiftUp_Click(object sender, EventArgs e)
        {
            btnLiftUp.BackColor = Color.Lime;
            btnLiftDown.BackColor = Color.White;
            SetLiftPosition();
        }

        private void btnLiftDown_Click(object sender, EventArgs e)
        {
            btnLiftUp.BackColor = Color.White;
            btnLiftDown.BackColor = Color.Lime;
            SetLiftPosition();
        }

        private int hiddenCount = 0;
        private void pnHidden_Leave(object sender, EventArgs e)
        {
            hiddenCount = 0;
        }

        private void pnHidden_Paint(object sender, PaintEventArgs e)
        {
            hiddenCount++;
            if (hiddenCount > 3)
            {

            }
        }

        private void btnSpeed_Click(object sender, EventArgs e)
        {
            btnPitch.BackColor = Color.White;
            btnSpeed.BackColor = Color.Lime;
        }

        private void btnPitch_Click(object sender, EventArgs e)
        {
            btnPitch.BackColor = Color.Lime;
            btnSpeed.BackColor = Color.White;
        }

        private void numPitch_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                trackBarPitch.Value = (int)((double)numPitch.Value * 100);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void trackBarPitch_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                numPitch.Value = (decimal)Math.Max(0.01, (double)trackBarPitch.Value / 100);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void trackBarSpeed_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                numSpeed.Value = trackBarSpeed.Value;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void numSpeed_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                trackBarSpeed.Value = (int)numSpeed.Value;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }
    }
}
