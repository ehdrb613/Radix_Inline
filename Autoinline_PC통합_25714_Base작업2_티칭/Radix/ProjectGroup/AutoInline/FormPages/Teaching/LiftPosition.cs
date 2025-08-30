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
using System.Diagnostics;

namespace Radix.Popup.Teaching
{
    /*
     * Setting.cs : 각종 옵션 설정 관리
     */

    public partial class LiftPosition : Form
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
        private FuncInline.enumLiftPos activePos = FuncInline.enumLiftPos.UnKnown; // 티칭배열 인덱스로 사용할 위치값
        private FuncInline.enumShuttlePos activeShuttlePos = FuncInline.enumShuttlePos.UnKnown; // 티칭배열 인덱스로 사용할 위치값
        private FuncInline.enumServoAxis actionAxis = FuncInline.enumServoAxis.SV02_Lift1; // 선택된 리프트 축
        private int activeNum = 0; // 선택된 축 순번
        private bool validPos = true; // 선택 조합이 맞는가?

        private int siteIndex = -1; //처음 선택은 PassLine1이기 때문에 사이트가 없다 //by DG

        FuncInline.enumTabMain beforeTabMain = FuncInline.TabMain;
        FuncInline.enumTabTeaching beforeTabTeaching = FuncInline.TabTeaching;
        bool valueChanged = false;

        private double[,] liftPos = new double[2, Enum.GetValues(typeof(FuncInline.enumLiftPos)).Length]; // 티칭값을 임시로 담아둘 변수. 저장 전에는 갖고만 있다가 저장시 이 값을 저장한다.

        private double[,] ShuttlePos = new double[2, Enum.GetValues(typeof(FuncInline.enumShuttlePos)).Length]; // 티칭값을 임시로 담아둘 변수,셔틀 위치값
        private int debugCount = 0;
        #endregion


        #region 초기화 함수
        public LiftPosition()
        {
            InitializeComponent();
        }

        private void Position_Load(object sender, EventArgs e)
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

        private void Position_Leave(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun) // 운전중 아니면 모든 모터 정지
            {

                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
                {
                    FuncMotion.MoveStop(i);
                }
                for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length; i++)
                {
                    //FuncPMC.Stop((enumPMCAxis)i);
                }
            }
        }

        #endregion

        #region 타이머 함수
        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Teaching ||
                FuncInline.TabTeaching != FuncInline.enumTabTeaching.LiftPosition) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                debugCount = 0;
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
                    //pnHide.Visible = hiddenCount > 3;

                    #region 관리자 여부에 따라 컨트롤 보이기/숨기기
                    //tbAppName.Enabled = GlobalVar.PwdPass;
                    //numMachineNum.Enabled = GlobalVar.PwdPass;
                    //pbSave.Visible = GlobalVar.PwdPass;
                    pnHide.Visible = debugCount >= 5;
                    lblHide.Visible = debugCount >= 5;
                    #endregion



                    FuncForm.SetServoStateColor(pbAxisStatus, actionAxis);
#if DEBUG
                    pnDebug.Visible = true;
                    lblServoPos.Text = (GlobalVar.AxisStatus[(int)actionAxis].Position).ToString("F0");
#endif
#if !DEBUG
                        pnDebug.Visible = false;
#endif
                    #region 모터 현재 위치 표시
                    lblPos.Text = FuncMotion.GetRealPosition((int)actionAxis).ToString("F2");
                    //lblTeaching.Text = liftPos[activeLift, (int)activePos].ToString("F2");

                    #endregion

                    #region 티칭값 표시
                    foreach (Control conPos in pnPos.Controls) // 사이트 묶음 전체
                    {
                        if (conPos.GetType() == typeof(Label))
                        {
                            Label label = (Label)conPos;
                            if (label.Name.Contains("lblLiftTeaching"))
                            {
                                string str = label.Name.Replace("lblLiftTeaching", "");
                                string[] idxs = str.Split('_');
                                if (idxs.Length == 2)
                                {
                                    int liftIndex = -1;
                                    int posIndex = -1;
                                    int.TryParse(idxs[0], out liftIndex);
                                    int.TryParse(idxs[1], out posIndex);
                                    if (liftIndex >= 0 &&
                                        posIndex >= 0)
                                    {
                                        label.Text = liftPos[liftIndex, posIndex].ToString("F2");
                                    }
                                }
                            }
                        }
                    }
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
            if (beforeTabMain == FuncInline.enumTabMain.Teaching && beforeTabTeaching == FuncInline.enumTabTeaching.LiftPosition &&
                (FuncInline.TabMain != FuncInline.enumTabMain.Teaching || FuncInline.TabTeaching != FuncInline.enumTabTeaching.LiftPosition))
            {
                FuncInlineMove.StopAllJog();
                //FuncInline.ClearAllSiteAction();
                #region 값 수정 상태로 창 떠나면 알림
                if (valueChanged)
                {
                    FuncInline.TabMain = FuncInline.enumTabMain.Teaching;
                    FuncInline.TabTeaching = FuncInline.enumTabTeaching.LiftPosition;

                    valueChanged = false;
                    if (FuncWin.MessageBoxOK("Robot Teaching changed. Save?"))
                    {
                        ApplyAllValue();
                        Func.SaveTeachingPositionIni();
                    }
                }
                #endregion
            }
            beforeTabMain = FuncInline.TabMain;
            beforeTabTeaching = FuncInline.TabTeaching;
            #endregion

            if (FuncInline.TabMain != FuncInline.enumTabMain.Teaching ||
                FuncInline.TabTeaching != FuncInline.enumTabTeaching.LiftPosition) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
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
                if (jogLift) // 축 두개
                {
                    if (!jogLiftDown &&
                        !jogLiftUp)
                    {
                        if (GlobalVar.E_Stop ||
                            GlobalVar.AxisStatus[(int)actionAxis].Errored)
                        {
                            FuncMotion.MoveStop((int)actionAxis);
                            jogLift = false;
                        }
                        else if (Math.Abs(GlobalVar.AxisStatus[(int)actionAxis].Velocity) > 0.1)
                        {
                            FuncMotion.MoveStop((int)actionAxis);
                        }
                        else
                        {
                            jogLift = false;
                        }
                    }
                }
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
        public void LoadAllValue()
        {
            try
            {
                // 시스템 옵셋에서 불러온다. Loading 경우에는 옵셋에서 PCB 길이를 빼 줘야 한다.(그립버전)
                // ==> 진공버전에서는 모두 고정티칭
                for (int j = 0; j < FuncInline.LiftPos.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.LiftPos.GetLength(1); i++)
                    {
                        // apply건 load건 다시 계산된 건 teaching 좌표에 적용
                        liftPos[j, i] = FuncInline.LiftPos[j, i];
                        //}
                    }
                }
                for (int j = 0; j < FuncInline.ShuttlePos.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.ShuttlePos.GetLength(1); i++)
                    {
                        // apply건 load건 다시 계산된 건 teaching 좌표에 적용
                        ShuttlePos[j, i] = FuncInline.ShuttlePos[j, i];
                       
                    }
                }

                if (activeNum < 2)
                {
                    lblTeaching.Text = liftPos[activeNum, (int)activePos].ToString("F2");
                }
                else
                {
                    lblTeaching.Text = ShuttlePos[activeNum - 2, (int)activeShuttlePos].ToString("F2");
                }

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            valueChanged = false;
        }

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
                for (int j = 0; j < FuncInline.ShuttlePos.GetLength(0); j++)
                {
                    for (int i = 0; i < FuncInline.ShuttlePos.GetLength(1); i++)
                    {
                        // apply건 load건 다시 계산된 건 teaching 좌표에 적용
                        FuncInline.ShuttlePos[j, i] = ShuttlePos[j, i];

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
                //FuncMotion.MoveRelative((uint)actionAxis,
                //      Util.GetRealSpeed((int)actionAxis, 0 - GlobalVar.RobotJogSpeed), // 좌표 환산 안 한 값이니 좌표값이지만 GetRealPulse 함수를 그대로 쓴다.
                //      Util.GetRealSpeed((int)actionAxis, GlobalVar.RobotJogSpeedMiddle));
                FuncInlineMove.MoveAbsolute((uint)actionAxis,
                   double.Parse(lblPos.Text) + (double)numPitch.Value,
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
                FuncInlineMove.MoveAbsolute((uint)actionAxis,10000,(double)numSpeed.Value);

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
                //FuncMotion.MoveRelative((uint)actionAxis,
                //      Util.GetRealSpeed((int)actionAxis, GlobalVar.RobotJogSpeed), // 좌표 환산 안 한 값이니 좌표값이지만 GetRealPulse 함수를 그대로 쓴다.
                //      Util.GetRealSpeed((int)actionAxis, GlobalVar.RobotJogSpeedMiddle));
                FuncInlineMove.MoveAbsolute((uint)actionAxis,
                  double.Parse(lblPos.Text) - (double)numPitch.Value,
                  (double)numSpeed.Value);
            }
            else
            {
                jogLift = true;
                jogLiftDown = true;
                FuncInlineMove.MoveAbsolute((uint)actionAxis,0,(double)numSpeed.Value);
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

        private void pbRobotApply_Click(object sender, EventArgs e)
        {
            #region 상하 구분 확인
            if (btnUpAndDown.BackColor == Color.Lime &&
                btnLiftDown.BackColor == Color.Lime)
            {
                FuncWin.TopMessageBox("Check Up Posion. or Check One Point Only.");
                return;
            }
            #endregion

            try
            {
                
                if (activeNum < 2)
                {
                    liftPos[activeNum, (int)activePos] = double.Parse(lblPos.Text);
                    lblTeaching.Text = liftPos[activeNum, (int)activePos].ToString("F2");

                    Func.WriteLog("Lift Pos apply : " + activePos.ToString() + " - " + liftPos[activeNum, (int)activePos]);
                }
                else
                {
                    ShuttlePos[activeNum - 2, (int)activeShuttlePos] = double.Parse(lblPos.Text);
                    lblTeaching.Text = ShuttlePos[activeNum - 2, (int)activeShuttlePos].ToString("F2");

                    Func.WriteLog("Shuttle Pos apply : " + activeShuttlePos.ToString() + " - " + ShuttlePos[activeNum - 2, (int)activeShuttlePos]);
                }
             

                #region 하단 티칭 자동 변경
                //if (btnUpAndDown.BackColor == Color.Lime)
                //{
                //    if (activePos != FuncInline.enumLiftPos.FrontPassLine &&
                //        activePos != FuncInline.enumLiftPos.NGDown &&
                //        activePos.ToString().Contains("Up"))
                //    {
                //        FuncInline.enumLiftPos pos = activePos;
                //        if (activePos >= FuncInline.enumLiftPos.Site1_F_DT1_Up &&
                //            activePos <= FuncInline.enumLiftPos.Site40Up)
                //        {
                //            pos += 40;
                //        }
                //        else
                //        {
                //            pos += 1;
                //        }
                //        liftPos[activeLift, (int)pos] = double.Parse(lblLiftPos.Text) + 60;
                //        Func.WriteLog("Lift Pos apply : " + pos.ToString() + " - " + liftPos[activeLift, (int)pos]);
                //    }
                //}
                #endregion

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            valueChanged = true;



        }

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

            FuncInlineMove.MoveAbsolute((uint)actionAxis,(double)numPos.Value,(double)numSpeed.Value);
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
            if (activeNum < 2)
            {
                FuncInline.enumLiftName LiftName = (FuncInline.enumLiftName)(activeNum);
                Func.WriteLog("Lift Home Click : " + LiftName.ToString() + " - " +
                            liftPos[(int)LiftName, (int)activePos]);
            }
            else
            {
                FuncInline.enumShuttleName ShuttleName = (FuncInline.enumShuttleName)(activeNum - 2);
                Func.WriteLog("Shuttle Home Click : " + ShuttleName.ToString() + " - " +
                           ShuttlePos[(int)ShuttleName, (int)activePos]);
            }
          
            FuncMotion.MoveHome((uint)actionAxis);
            //Func.HomeXYZRobot();
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

            //서보 FonrtLift 자동 선택
            btnLift1_Click(btnLift1, EventArgs.Empty);
        }



        private void btnPos_Click(object sender, EventArgs e) // 버튼 눌렀을 때
        {
            try
            {
                bool isLift = false;
                bool isShuttle = false;
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
                if((siteIndex >= 0 && siteIndex < 13) ||
                    name.Contains("btnPosFrontScan") ||
                    name.Contains("btnPosFrontPassLine") ||
                    name.Contains("btnPosOutShuttleUp") ||
                    name.Contains("btnPosOutShuttleDown"))
                {
                    //서보 FonrtLift 자동 선택
                    btnLift1_Click(btnLift1, EventArgs.Empty);
                    isLift = true;
                }
                else if((siteIndex >= 13 && siteIndex < 26) ||
                    name.Contains("btnPosRearScan") ||
                    name.Contains("btnPosRearPassLine") ||
                    name.Contains("btnPosRearNGLine") ||
                    name.Contains("btnPosOutShuttleDown"))
                {
                    //서보 RearLift 자동 선택
                    btnLift2_Click(btnLift2, EventArgs.Empty);
                    isLift = true;
                }
                else if(name.Contains("btnPosInShuttle_InConLoading") ||
                    name.Contains("btnPosInShuttle_FrontRackUnLoading") ||
                    name.Contains("btnPosInShuttle_TurnPosition") ||
                    name.Contains("btnPosInShuttle_RearLiftUnLoading"))
                {
                    //서보 InShuttle 자동 선택
                    btnInShuttle_Click(btnInShuttle, EventArgs.Empty);
                    isLift = false;
                }
                else if (name.Contains("btnPosOutShuttle_FrontLiftLoading") ||
                   name.Contains("btnPosOutShuttle_OutConvUnLoading") ||
                   name.Contains("btnPosOutShuttle_TurnPosition") ||
                   name.Contains("btnPosOutShuttle_RearRackLoading"))
                {
                    //서보 OutShuttle 자동 선택
                    btnOutShuttle_Click(btnOutShuttle, EventArgs.Empty);
                    isLift = false;
                }

                activePanelPos = name;
                SetLiftPosition();

                #region 티칭 좌표 선택
                if (validPos)
                {
                    if (activeNum < 2)
                    {
                        lblTeaching.Text = liftPos[activeNum, (int)activePos].ToString("F2");
                        numPos.Value = (decimal)liftPos[activeNum, (int)activePos];
                     
                    }
                    else
                    {
                        lblTeaching.Text = ShuttlePos[activeNum - 2, (int)activeShuttlePos].ToString("F2");
                        numPos.Value = (decimal)ShuttlePos[activeNum - 2, (int)activeShuttlePos];
                      
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
        void PaintPosButtons(string activeName) //패널 그룹화 해서 컨트롤 찾기 인셔틀 추가
        {
            Control[] panels = { pnPos, pnshuttlePos }; // ← 둘을 묶어서

            foreach (var panel in panels)
            {
                foreach (Control conButton in panel.Controls)
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
            }
        }

        private void SetLiftPosition() // 위치, 리프트 선택 따라서 축 선택
        {
            #region 전체 버튼 색상 지정
            //foreach (Control conButton in pnPos.Controls)
            //{
            //    if (conButton.Name.StartsWith("btnPos"))
            //    {
            //        if (conButton.Name == activePanelPos)
            //        {
            //            ((Button)conButton).BackColor = Color.Lime;
            //        }
            //        else
            //        {
            //            ((Button)conButton).BackColor = Color.White;
            //        }
            //    }
            //}

            PaintPosButtons(activePanelPos);    //활성화 버튼 색상 변경

            #endregion
            #region Lift 부분
            if (activeNum == 0 &&
               btnPosFrontPassLine.BackColor == Color.Lime &&
               btnLiftUp.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.FrontPassLine; // PassLine에서 FrontLift 상단 이송
                validPos = true;
            }
            else if (activeNum == 0 &&
                btnPosFrontScan.BackColor == Color.Lime &&
                btnLiftUp.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.FrontScanPos; //  FrontScanSite 위치
                validPos = true;
            }
            else if (activeNum == 0 &&
              btnPosOutShuttleUp.BackColor == Color.Lime &&
              btnLiftUp.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.OutShuttleUp; // FrontLift - Outshuttle UP
                validPos = true;
            }
            else if (activeNum == 0 &&
               btnPosOutShuttleDown.BackColor == Color.Lime &&
               btnLiftUp.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.OutShuttleDown; // FrontLift - Outshuttle Down
                validPos = true;
            }

            else if (activeNum == 1 &&
                btnPosRearPassLine.BackColor == Color.Lime &&
                btnLiftUp.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.RearPassLine; // RearLift - RearPassLine
                validPos = true;
            }
            else if (activeNum == 1 &&
              btnPosRearNGLine.BackColor == Color.Lime &&
              btnLiftUp.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.RearNGLine; // RearLift - RearNGLine 
                validPos = true;
            }
            else if (activeNum == 1 &&
              btnPosRearScan.BackColor == Color.Lime &&
              btnLiftUp.BackColor == Color.Lime)
            {
                activePos = FuncInline.enumLiftPos.RearScanPos; // FrontLift 상단에서 패스라인2로 이송
                validPos = true;
            }
            else if (siteIndex >= 0 &&
                siteIndex < 13 && // 좌측 렉 사이트
                activeNum == 0) // 좌측 리프트 사용하는 경우만
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
            else if (siteIndex >= 13 &&
                siteIndex < 26 && // 우측 렉 사이트
                activeNum == 1) // 우측 리프트 사용하는 경우만
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
            #endregion
            #region 셔틀 위치
            else if (activeNum == 2 &&
            btnPosInShuttle_InConLoading.BackColor == Color.Lime)
            {
                activeShuttlePos = FuncInline.enumShuttlePos.InShuttle_InConveyorLoading; // InShuttle_InConveyorLoading
                validPos = true;
            }
            else if (activeNum == 2 &&
            btnPosInShuttle_FrontRackUnLoading.BackColor == Color.Lime)
            {
                activeShuttlePos = FuncInline.enumShuttlePos.InShuttle_FrontRackUnLoading; // nShuttle_FrontRackUnLoading
                validPos = true;
            }
            else if (activeNum == 2 &&
            btnPosInShuttle_TurnPosition.BackColor == Color.Lime)
            {
                activeShuttlePos = FuncInline.enumShuttlePos.InShuttle_TurnPosition; // InShuttle_TurnPosition
                validPos = true;
            }
            else if (activeNum == 2 &&
            btnPosInShuttle_RearLiftUnLoading.BackColor == Color.Lime)
            {
                activeShuttlePos = FuncInline.enumShuttlePos.InShuttle_RearLiftUnLoading; // InShuttle RearLiftUnLoading
                validPos = true;
            }
            else if (activeNum == 3 &&
            btnPosOutShuttle_FrontLiftLoading.BackColor == Color.Lime)
            {
                activeShuttlePos = FuncInline.enumShuttlePos.OutShuttle_FrontLiftLoading; // OutShuttle_FrontLiftLoading
                validPos = true;
            }
            else if (activeNum == 3 &&
            btnPosOutShuttle_OutConvUnLoading.BackColor == Color.Lime)
            {
                activeShuttlePos = FuncInline.enumShuttlePos.OutShuttle_OutCovyUnLoading; // RearLift - RearNGLine 
                validPos = true;
            }
            else if (activeNum == 3 &&
            btnPosOutShuttle_TurnPosition.BackColor == Color.Lime)
            {
                activeShuttlePos = FuncInline.enumShuttlePos.OutShuttle_TurnPosition; // RearLift - RearNGLine 
                validPos = true;
            }
            else if (activeNum == 3 &&
            btnPosOutShuttle_RearRackLoading.BackColor == Color.Lime)
            {
                activeShuttlePos = FuncInline.enumShuttlePos.OutShuttle_RearRackLoading; // RearLift - RearNGLine 
                validPos = true;
            }
            #endregion

            else
            {
                validPos = false;
            }
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

      

        private void btnLiftUp_Click(object sender, EventArgs e)
        {
            btnLiftUp.BackColor = Color.Lime;
            btnLiftDown.BackColor = Color.White;
            SetLiftPosition();
            lblTeaching.Text = FuncInline.LiftPos[activeNum, (int)activePos].ToString("F2");
        }

        private void btnLiftDown_Click(object sender, EventArgs e)
        {
            btnLiftUp.BackColor = Color.White;
            btnLiftDown.BackColor = Color.Lime;
            SetLiftPosition();
            lblTeaching.Text = FuncInline.LiftPos[activeNum, (int)activePos].ToString("F2");
        }

        private int hiddenCount = 0;

        private void pnHidden_Click(object sender, EventArgs e)
        {
            hiddenCount++;
            if (hiddenCount > 3)
            {
                pnHide.Visible = true;
            }
        }

        private void pnHidden_Leave(object sender, EventArgs e)
        {
            hiddenCount = 0;
        }

        private void btnOverAllChange_Click(object sender, EventArgs e)
        {
            try
            {
                if(activeNum < 2)
                {
                    for (int i = 0; i < liftPos.GetLength(1); i++)
                    {
                        liftPos[activeNum, i] += (double)numOverAll.Value;
                    }
                }
               
                
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        private void btnOneOnly_Click(object sender, EventArgs e)
        {
            btnOneOnly.BackColor = Color.Lime;
            btnUpAndDown.BackColor = Color.White;
        }

        private void btnUpAndDown_Click(object sender, EventArgs e)
        {
            btnOneOnly.BackColor = Color.White;
            btnUpAndDown.BackColor = Color.Lime;
        }

        // Passline 1 위치값을 전체 왼쪽 티칭에 적용
        private void btnDefaultLeft_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    // Passline2쪽에 적용
            //    liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.Pass2Up] = liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.Pass1Up];
            //    liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.Pass2Down] = liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.Pass1Up] + 60;
            //    // Site1 ~ Site6 아래쪽 역순으로
            //    for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site6Up; pos >= FuncInline.enumLiftPos.Site1_F_DT1_Up; pos--)
            //    {
            //        liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)pos] = liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.Pass1Up] - 60 + ((int)pos - (int)FuncInline.enumLiftPos.Site6Up) * 60;
            //        liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)pos + 40] = liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.Pass1Up] + ((int)pos - (int)FuncInline.enumLiftPos.Site6Up) * 60;
            //    }
            //    // Site7 ~ Site40 위쪽 정순으로
            //    for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site7Up; pos <= FuncInline.enumLiftPos.Site20Up; pos++)
            //    {
            //        liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)pos] = liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.Pass1Up] + 60 + ((int)pos - (int)FuncInline.enumLiftPos.Site7Up) * 60;
            //        liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)pos + 40] = liftPos[(int)FuncInline.enumLiftName.FrontLift, (int)FuncInline.enumLiftPos.Pass1Up] + 120 + ((int)pos - (int)FuncInline.enumLiftPos.Site7Up) * 60;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    debug(ex.ToString());
            //    debug(ex.StackTrace);
            //}
        }

        // Passline 2 위치값을 전체 오른쪽 티칭에 적용
        private void btnDefaultRight_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    // Passline2 하단
            //    liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Down] = liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Up] + 60;
            //    // Output Conveyor 쪽에 적용
            //    liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.OutputUp] = liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Up];
            //    liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.OutputDown] = liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Up] + 60;
            //    // NG
            //    liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Down] = liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Up];
            //    // Site21 ~ Site26 아래쪽 역순으로
            //    for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site26Up; pos >= FuncInline.enumLiftPos.Site21Up; pos--)
            //    {
            //        liftPos[(int)FuncInline.enumLiftName.Lift2, (int)pos] = liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Up] - 60 + ((int)pos - (int)FuncInline.enumLiftPos.Site26Up) * 60;
            //        liftPos[(int)FuncInline.enumLiftName.Lift2, (int)pos + 40] = liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Up] + ((int)pos - (int)FuncInline.enumLiftPos.Site26Up) * 60;
            //    }
            //    // Site7 ~ Site40 위쪽 정순으로
            //    for (FuncInline.enumLiftPos pos = FuncInline.enumLiftPos.Site27Up; pos <= FuncInline.enumLiftPos.Site40Up; pos++)
            //    {
            //        liftPos[(int)FuncInline.enumLiftName.Lift2, (int)pos] = liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Up] + 60 + ((int)pos - (int)FuncInline.enumLiftPos.Site27Up) * 60;
            //        liftPos[(int)FuncInline.enumLiftName.Lift2, (int)pos + 40] = liftPos[(int)FuncInline.enumLiftName.Lift2, (int)FuncInline.enumLiftPos.Pass2Up] + 120 + ((int)pos - (int)FuncInline.enumLiftPos.Site27Up) * 60;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    debug(ex.ToString());
            //    debug(ex.StackTrace);
            //}
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

        private void panel3_Click(object sender, EventArgs e)
        {
            debugCount++;
        }
        private void btnLift1_Click(object sender, EventArgs e)
        {
            btnLift1.BackColor = Color.Lime;
            btnLift2.BackColor = Color.White;
            btnInShuttle.BackColor = Color.White;
            btnOutShuttle.BackColor = Color.White;
            activeNum = 0;
            actionAxis = FuncInline.enumServoAxis.SV02_Lift1;
            SetLiftPosition();
            lblTeaching.Text = FuncInline.LiftPos[activeNum, (int)activePos].ToString("F2");
        }

        private void btnLift2_Click(object sender, EventArgs e)
        {
            btnLift1.BackColor = Color.White;
            btnLift2.BackColor = Color.Lime;
            btnInShuttle.BackColor = Color.White;
            btnOutShuttle.BackColor = Color.White;
            activeNum = 1;
            actionAxis = FuncInline.enumServoAxis.SV04_Lift2;
            SetLiftPosition();
            lblTeaching.Text = FuncInline.LiftPos[activeNum, (int)activePos].ToString("F2");
        }
        private void btnInShuttle_Click(object sender, EventArgs e)
        {
            btnLift1.BackColor = Color.White;
            btnLift2.BackColor = Color.White;
            btnInShuttle.BackColor = Color.Lime;
            btnOutShuttle.BackColor = Color.White;
            activeNum = 2;
            actionAxis = FuncInline.enumServoAxis.SV00_In_Shuttle;
            SetLiftPosition();
            lblTeaching.Text = FuncInline.ShuttlePos[activeNum, (int)activePos].ToString("F2");
        }

        private void btnOutShuttle_Click(object sender, EventArgs e)
        {
            btnLift1.BackColor = Color.White;
            btnLift2.BackColor = Color.White;
            btnInShuttle.BackColor = Color.White;
            btnOutShuttle.BackColor = Color.Lime;
            activeNum = 3;
            actionAxis = FuncInline.enumServoAxis.SV01_Out_Shuttle;
            SetLiftPosition();
            lblTeaching.Text = FuncInline.ShuttlePos[activeNum, (int)activePos].ToString("F2");
        }
    }
}
