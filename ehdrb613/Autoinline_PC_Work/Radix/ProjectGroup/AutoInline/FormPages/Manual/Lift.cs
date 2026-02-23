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
        private FuncInline.enumLiftPos activeLiftPos = FuncInline.enumLiftPos.UnKnown; // 티칭배열 인덱스로 사용할 위치값
       
        private FuncInline.enumShuttlePos activeShuttlePos = FuncInline.enumShuttlePos.UnKnown; // 티칭배열 인덱스로 사용할 위치값

        private FuncInline.enumServoAxis actionAxis = FuncInline.enumServoAxis.SV02_Lift1; // 선택된 리프트 축

        private const FuncInline.enumServoAxis actionAxisX = FuncInline.enumServoAxis.SV07_Scan_X; // 선택된 리프트 축
        private const FuncInline.enumServoAxis actionAxisY = FuncInline.enumServoAxis.SV06_Scan_Y; // 선택된 리프트 축
        private int activeNum = 0; // 선택된 리프트 순번
        private bool validPos = true; // 선택 조합이 맞는가?

        private int siteIndex = -1; //처음 선택은 PassLine1이기 때문에 사이트가 없다 //by DG

        FuncInline.enumTabMain beforeTabMain = FuncInline.TabMain;
        FuncInline.enumTabManual beforeTabManual = FuncInline.TabManual;
        bool valueChanged = false;

        private double[,] liftPos = new double[2, Enum.GetValues(typeof(FuncInline.enumLiftPos)).Length]; // 티칭값을 임시로 담아둘 변수. 저장 전에는 갖고만 있다가 저장시 이 값을 저장한다.
        private double[,] ShuttlePos = new double[2, Enum.GetValues(typeof(FuncInline.enumShuttlePos)).Length]; // 티칭값을 임시로 담아둘 변수,셔틀 위치값

        bool isLift = false;
        bool isShuttle = false;
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
            //if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun) // 운전중 아니면 모든 모터 정지
            //{

            //    for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length; i++)
            //    {
            //        FuncMotion.MoveStop(i);
            //    }
            //    //이부분은 Let's보드로 바꿔야함
            //    for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length; i++)
            //    {
            //        //FuncInline.ComPMC[(int)((int)i / 2)].Stop((FuncInline.enumPMCAxis)i);
            //    }
            //}
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
                        lblPos.Text = GlobalVar.AxisStatus[(int)actionAxis].Position.ToString("F2");
                        //lblTeaching.Text = liftPos[activeLift, (int)activePos].ToString("F2");

                        //동작중이면 MOVE버튼 비활성화
                        pbRobotMove.Enabled = GlobalVar.AxisStatus[(int)actionAxis].StandStill;
                        btnScanMove.Enabled = GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV07_Scan_X].StandStill &&
                                                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV06_Scan_Y].StandStill;
                        #endregion

                        #region 센서등의 상태에 따라 Trun 동작 활성화/비활성화
                        //interlock센서 감지되면 턴 가능
                        btnInShuttleTurnStart.Enabled = DIO.GetDIData(FuncInline.enumDINames.X303_0_In_Shuttle_Turn_Position_Interlock);
                        btnOutShuttleTurnStart.Enabled = DIO.GetDIData(FuncInline.enumDINames.X303_3_Out_Shuttle_Turn_Position_Interlock);
                        btnInShuttleTurnStart.BackColor = DIO.GetDOData(FuncInline.enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder)? Color.Lime : Color.White;
                        btnOutShuttleTurnStart.BackColor = DIO.GetDOData(FuncInline.enumDONames.Y304_3_Out_Shuttle_Turn_Cw_Cylinder) ? Color.Lime : Color.White;
                        #endregion


                        #region 세대별 텍스트명 변경
                        // PCB 감지 표시 버튼들을 배열로 모아서 루프 처리
                        Button[] pcbBtns = new Button[]
                        {
                            btnPosSite1,  btnPosSite2,  btnPosSite3,  btnPosSite4,  btnPosSite5,  btnPosSite6,
                            btnPosSite7,  btnPosSite8,  btnPosSite9,  btnPosSite10, btnPosSite11, btnPosSite12,
                            btnPosSite13, btnPosSite14, btnPosSite15, btnPosSite16, btnPosSite17, btnPosSite18,
                            btnPosSite19, btnPosSite20, btnPosSite21, btnPosSite22, btnPosSite23, btnPosSite24,
                            btnPosSite25, btnPosSite26
                        };
                        // 세대별 사이트 수(MaxSiteCount)에 맞춰 안전하게 처리
                        int count = Math.Min(pcbBtns.Length, FuncInline.MaxSiteCount);
                        for (int i = 0; i < count; i++)
                        {
                            int.TryParse(pcbBtns[i].Name.Replace("btnPosSite", ""), out siteIndex);
                            siteIndex--;
                            FuncInline.enumTeachingPos pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex);
                            //세대별 사이트 명칭 변경
                            string label = FuncInline.SiteDisplay.GetSiteDisplayName(pos);

                            pcbBtns[i].Text = label;
                        }

                        #endregion

                        #region Scanner
                        lbScanText.Text = FuncInline.Load_Scanner;
                        #endregion
                    }));
                }

            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
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

                //FuncMotion.MoveStop((int)actionAxis);
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
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


            if (btnPitch.BackColor == Color.Lime)
            {

                if (actionAxis == FuncInline.enumServoAxis.SV02_Lift1 ||
                        actionAxis == FuncInline.enumServoAxis.SV04_Lift2)
                {
                    FuncInlineMove.MoveAbsolute((uint)actionAxis,
                    double.Parse(lblPos.Text) + (double)numPitch.Value,
                   (double)numSpeed.Value);
                }
                else
                {
                    FuncInlineMove.MoveAbsolute((uint)actionAxis,
                    double.Parse(lblPos.Text) - (double)numPitch.Value,
                    (double)numSpeed.Value);
                }
            }
            else
            {
                jogLift = true;
                jogLiftUp = true;


                if (actionAxis == FuncInline.enumServoAxis.SV02_Lift1 ||
                  actionAxis == FuncInline.enumServoAxis.SV04_Lift2)
                {
                    FuncInlineMove.MoveAbsolute((uint)actionAxis, 10000, (double)numSpeed.Value);
                }
                else
                {
                    FuncInlineMove.MoveAbsolute((uint)actionAxis, -200, (double)numSpeed.Value);
                }

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

                if (actionAxis == FuncInline.enumServoAxis.SV02_Lift1 ||
                  actionAxis == FuncInline.enumServoAxis.SV04_Lift2)
                {
                    FuncInlineMove.MoveAbsolute((uint)actionAxis,
                    double.Parse(lblPos.Text) - (double)numPitch.Value,
                    (double)numSpeed.Value);
                }
                else
                {
                    FuncInlineMove.MoveAbsolute((uint)actionAxis,
                    double.Parse(lblPos.Text) + (double)numPitch.Value,
                    (double)numSpeed.Value);
                }
            }
            else
            {
                jogLift = true;
                jogLiftDown = true;
                if (actionAxis == FuncInline.enumServoAxis.SV02_Lift1 ||
                    actionAxis == FuncInline.enumServoAxis.SV04_Lift2)
                {
                    FuncInlineMove.MoveAbsolute((uint)actionAxis, -200, (double)numSpeed.Value);
                }
                else
                {
                    FuncInlineMove.MoveAbsolute((uint)actionAxis, 10000, (double)numSpeed.Value);
                }
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
                FuncWin.TopMessageBox("Check Position Selection.");
                return;
            }

            #region 초기화 전 실행 금지
            if (!GlobalVar.AxisStatus[(uint)actionAxis].isHomed)
            {
                FuncWin.TopMessageBox("Homing first");
                return;
            }
            #endregion

            #region 도어 열림시 동작 금지
            if (GlobalVar.UseDoor &&
                    (DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_2_Door_Open_Rear_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X02_0_Door_Open_Rear_Right)))
            {
                FuncWin.TopMessageBox("Can not move while doors are opened.");
                return;
            }
            #endregion

            FuncLog.WriteLog($"Move Click : {actionAxis.ToString()}, Pos:{numPos.Value}, Speed:{numSpeed.Value} ");

            FuncInlineMove.MoveAbsolute((uint)actionAxis, (double)numPos.Value, (double)numSpeed.Value);
        
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
                    (DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_2_Door_Open_Rear_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X02_0_Door_Open_Rear_Right)))
            {
                FuncWin.TopMessageBox("Can not move while doors are opened.");
                return;
            }
            #endregion
            if (activeNum < 2)
            {
                FuncInline.enumLiftName LiftName = (FuncInline.enumLiftName)(activeNum);
                FuncLog.WriteLog("Lift Home Click : " + LiftName.ToString() + " - " +
                            liftPos[(int)LiftName, (int)activeLiftPos]);
            }
            else
            {
                FuncInline.enumShuttleName ShuttleName = (FuncInline.enumShuttleName)(activeNum - 2);
                FuncLog.WriteLog("Shuttle Home Click : " + ShuttleName.ToString() + " - " +
                           ShuttlePos[(int)ShuttleName, (int)activeShuttlePos]);
            }

            FuncMotion.MoveHome((uint)actionAxis);

        }

        private void pbRobotStop_Click(object sender, EventArgs e)
        {
            if (!validPos)
            {
                FuncWin.TopMessageBox("Check Position Selection.");
                return;
            }
            if (activeNum < 2)
            {
                FuncInline.enumLiftName LiftName = (FuncInline.enumLiftName)(activeNum);
                FuncLog.WriteLog("Lift Stop Click : " + LiftName.ToString());

                Func.StopLift();
            }
            else
            {
                FuncInline.enumShuttleName ShuttleName = (FuncInline.enumShuttleName)(activeNum - 2);
                FuncLog.WriteLog("Shuttle Stop Click : " + ShuttleName.ToString());

                Func.StopShuttle();
            }

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

       



        private void btnPos_Click(object sender, EventArgs e) // 버튼 눌렀을 때
        {
            try
            {
                isLift = false;
                isShuttle = false;
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
                if ((siteIndex >= 0 && siteIndex < 13) ||
                    name.Contains("btnPosFrontScan") ||
                    name.Contains("btnPosFrontPassLine") ||
                    name.Contains("btnPosOutShuttleUp") ||
                    name.Contains("btnPosOutShuttleDown"))
                {
                    isLift = true;
                    //서보 FonrtLift 자동 선택
                    btnLift1_Click(btnLift1, EventArgs.Empty);
                   
                }
                else if ((siteIndex >= 13 && siteIndex < 26) ||
                    name.Contains("btnPosRearScan") ||
                    name.Contains("btnPosRearPassLine") ||
                    name.Contains("btnPosRearNGLine") ||
                    name.Contains("btnPosRearInshuttle") )
                {
                    isLift = true;
                    //서보 RearLift 자동 선택
                    btnLift2_Click(btnLift2, EventArgs.Empty);
                    
                }
                else if (name.Contains("btnPosInShuttle_InConLoading") ||
                    name.Contains("btnPosInShuttle_FrontRackUnLoading") ||
                    name.Contains("btnPosInShuttle_TurnPosition") ||
                    name.Contains("btnPosInShuttle_RearLiftUnLoading"))
                {
                    isLift = false;
                    //서보 InShuttle 자동 선택
                    btnInShuttle_Click(btnInShuttle, EventArgs.Empty);
                    
                }
                else if (name.Contains("btnPosOutShuttle_FrontLiftLoading") ||
                   name.Contains("btnPosOutShuttle_OutConvUnLoading") ||
                   name.Contains("btnPosOutShuttle_TurnPosition") ||
                   name.Contains("btnPosOutShuttle_RearRackLoading"))
                {
                    isLift = false;
                    //서보 OutShuttle 자동 선택
                    btnOutShuttle_Click(btnOutShuttle, EventArgs.Empty);
                   
                }

                activePanelPos = name;
                SetLiftPosition();

                #region 티칭 좌표 선택
                if (validPos)
                {
                    if (activeNum < 2)
                    {
                        lblTeaching.Text = FuncInline.LiftPos[activeNum, (int)activeLiftPos].ToString("F2");
                        numPos.Value = (decimal)FuncInline.LiftPos[activeNum, (int)activeLiftPos];

                    }
                    else
                    {
                        lblTeaching.Text = FuncInline.ShuttlePos[activeNum - 2, (int)activeShuttlePos].ToString("F2");
                        numPos.Value = (decimal)FuncInline.ShuttlePos[activeNum - 2, (int)activeShuttlePos];

                    }

                }
                #endregion

                //string name = ((Button)sender).Name;
                //if (name.Contains("Site"))
                //{
                //    int.TryParse(name.Replace("btnPosSite", ""), out siteIndex);
                //    siteIndex--;
                //}
                //else
                //{
                //    siteIndex = -1;
                //}
                //activePanelPos = name;
                //SetLiftPosition();

                //#region 티칭 좌표 선택
                //if (validPos)
                //{
                //    lblTeaching.Text = FuncInline.LiftPos[activeNum, (int)activeLiftPos].ToString("F2");
                //    numPos.Value = (decimal)FuncInline.LiftPos[activeNum, (int)activeLiftPos];
                //}
                //#endregion
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
                activeLiftPos = FuncInline.enumLiftPos.FrontPassLine; // PassLine에서 FrontLift 상단 이송
                validPos = true;
            }
            else if (activeNum == 0 &&
                btnPosFrontScan.BackColor == Color.Lime &&
                btnLiftUp.BackColor == Color.Lime)
            {
                activeLiftPos = FuncInline.enumLiftPos.FrontScanPos; //  FrontScanSite 위치
                validPos = true;
            }
            else if (activeNum == 0 &&
              btnPosOutShuttleUp.BackColor == Color.Lime &&
              btnLiftUp.BackColor == Color.Lime)
            {
                activeLiftPos = FuncInline.enumLiftPos.OutShuttleUp; // FrontLift - Outshuttle UP
                validPos = true;
            }
            else if (activeNum == 0 &&
               btnPosOutShuttleDown.BackColor == Color.Lime &&
               btnLiftUp.BackColor == Color.Lime)
            {
                activeLiftPos = FuncInline.enumLiftPos.OutShuttleDown; // FrontLift - Outshuttle Down
                validPos = true;
            }

            else if (activeNum == 1 &&
                btnPosRearPassLine.BackColor == Color.Lime &&
                btnLiftUp.BackColor == Color.Lime)
            {
                activeLiftPos = FuncInline.enumLiftPos.RearPassLine; // RearLift - RearPassLine
                validPos = true;
            }
            else if (activeNum == 1 &&
              btnPosRearNGLine.BackColor == Color.Lime &&
              btnLiftUp.BackColor == Color.Lime)
            {
                activeLiftPos = FuncInline.enumLiftPos.RearNGLine; // RearLift - RearNGLine 
                validPos = true;
            }
            else if (activeNum == 1 &&
              btnPosRearScan.BackColor == Color.Lime &&
              btnLiftUp.BackColor == Color.Lime)
            {
                activeLiftPos = FuncInline.enumLiftPos.RearScanPos; 
                validPos = true;
            }               
            else if (activeNum == 1 &&
              btnPosRearInshuttle.BackColor == Color.Lime &&
              btnLiftUp.BackColor == Color.Lime)
            {
                activeLiftPos = FuncInline.enumLiftPos.RearInShuttlePos; 
                validPos = true;
            }
            else if (siteIndex >= 0 &&
                siteIndex < 13 && // 좌측 렉 사이트
                activeNum == 0) // 좌측 리프트 사용하는 경우만
            {
                if (btnLiftUp.BackColor == Color.Lime) // 리프트1 상단 이용시
                {
                    activeLiftPos = FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex;
                }
                else // 리프트 1 하단 이용시
                {
                    activeLiftPos = FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex;
                }
                validPos = true;
            }
            else if (siteIndex >= 13 &&
                siteIndex < 26 && // 우측 렉 사이트
                activeNum == 1) // 우측 리프트 사용하는 경우만
            {
                if (btnLiftUp.BackColor == Color.Lime) // 리프트2 상단 이용시
                {
                    activeLiftPos = FuncInline.enumLiftPos.Site1_F_DT1_Up + siteIndex;
                }
                else // 리프트 1 하단 이용시
                {
                    activeLiftPos = FuncInline.enumLiftPos.Site1_F_DT1_Down + siteIndex;
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
                if (activeNum == 0)
                {
                    activeLiftPos = FuncInline.enumLiftPos.FrontPassLine; // PassLine에서 FrontLift 상단 이송
                    PaintPosButtons("btnPosFrontPassLine");    //활성화 버튼 색상 변경
                    validPos = true;
                }
                else if (activeNum == 1)
                {
                    activeLiftPos = FuncInline.enumLiftPos.RearPassLine; // PassLine에서 FrontLift 상단 이송
                    PaintPosButtons("btnPosRearPassLine");    //활성화 버튼 색상 변경
                    validPos = true;
                }
                else if (activeNum == 2)
                {
                    activeShuttlePos = FuncInline.enumShuttlePos.InShuttle_InConveyorLoading; // PassLine에서 FrontLift 상단 이송
                    PaintPosButtons("btnPosInShuttle_InConLoading");    //활성화 버튼 색상 변경
                    validPos = true;
                }
                else if (activeNum == 3)
                {
                    activeShuttlePos = FuncInline.enumShuttlePos.OutShuttle_FrontLiftLoading; // PassLine에서 FrontLift 상단 이송
                    PaintPosButtons("btnPosOutShuttle_FrontLiftLoading");    //활성화 버튼 색상 변경
                    validPos = true;
                }
                else
                {
                    validPos = false;
                }
                
            }

            #region 티칭 좌표 선택
            //if (validPos)
            //{
            //    lblTeaching.Text = FuncInline.LiftPos[activeNum, (int)activeLiftPos].ToString("F2");
            //    numPos.Value = (decimal)FuncInline.LiftPos[activeNum, (int)activeLiftPos];
            //}
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
            btnInShuttle.BackColor = Color.White;
            btnOutShuttle.BackColor = Color.White;
            activeNum = 0;
            actionAxis = FuncInline.enumServoAxis.SV02_Lift1;
            //티칭위치 선택이 아닌 서보모터 선택시 지정된 티칭위치로 이동
            if (!isLift)
            {
                siteIndex = -1;
            }
            
            //activePanelPos = "btnPosFrontPassLine";
            SetLiftPosition();
            lblTeaching.Text = FuncInline.LiftPos[activeNum, (int)activeLiftPos].ToString("F2");
        }

        private void btnLift2_Click(object sender, EventArgs e)
        {
            btnLift1.BackColor = Color.White;
            btnLift2.BackColor = Color.Lime;
            btnInShuttle.BackColor = Color.White;
            btnOutShuttle.BackColor = Color.White;
            activeNum = 1;
            actionAxis = FuncInline.enumServoAxis.SV04_Lift2;

            //티칭위치 선택이 아닌 서보모터 선택시 지정된 티칭위치로 이동
            if (!isLift)
            {
                siteIndex = -1;
            }
            //activePanelPos = "btnPosRearPassLine";
            SetLiftPosition();
            lblTeaching.Text = FuncInline.LiftPos[activeNum, (int)activeLiftPos].ToString("F2");
        }

        private void btnLiftUp_Click(object sender, EventArgs e)
        {
            btnLiftUp.BackColor = Color.Lime;
            btnLiftDown.BackColor = Color.White;

            //티칭위치 선택이 아닌 서보모터 선택시 지정된 티칭위치로 이동
            if (!isLift)
            {
                siteIndex = -1;
            }
            activePanelPos = "btnPosInShuttle_InConLoading";
            SetLiftPosition();
           
        }

        private void btnLiftDown_Click(object sender, EventArgs e)
        {
            btnLiftUp.BackColor = Color.White;
            btnLiftDown.BackColor = Color.Lime;
            //티칭위치 선택이 아닌 서보모터 선택시 지정된 티칭위치로 이동
            if (!isLift)
            {
                siteIndex = -1;
            }
            activePanelPos = "btnPosInShuttle_InConLoading";
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

        private void btnScanT_Click(object sender, EventArgs e)
        {
            FuncInline.Scanner.SendTrigger();
        }

        private void btnScanMove_Click(object sender, EventArgs e)
        {
            #region 초기화 전 실행 금지
            if (!GlobalVar.AxisStatus[(int)actionAxisX].isHomed ||
                !GlobalVar.AxisStatus[(int)actionAxisY].isHomed)
            {
                FuncWin.TopMessageBox("Homing first.");
                return;
            }
            #endregion
            #region 도어 열림시 동작 금지
            if (GlobalVar.UseDoor &&
                    (DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                            DIO.GetDIData(FuncInline.enumDINames.X00_2_Door_Open_Rear_Left) ||
                            DIO.GetDIData(FuncInline.enumDINames.X02_0_Door_Open_Rear_Right)))
            {
                FuncWin.TopMessageBox("Can not move while doors are opened.");
                return;
            }
            #endregion
            int Pos = cobScanArray_Sel.SelectedIndex; // 없으면 -1

            if(Pos == -1)
            {
                FuncWin.TopMessageBox("Check Scan Position Selection.");
                return;
            }
            else
            {
                FuncLog.WriteLog($"Move Scan Click Pos X - {FuncInline.ScanTeachingPos[Pos + 1].x} Y - {FuncInline.ScanTeachingPos[Pos + 1].y}, Speed:{numSpeed.Value} ");

                FuncInlineMove.MoveAbsolute((uint)actionAxisX, FuncInline.ScanTeachingPos[Pos + 1].x, (double)numSpeed.Value);
                FuncInlineMove.MoveAbsolute((uint)actionAxisY, FuncInline.ScanTeachingPos[Pos + 1].y, (double)numSpeed.Value);
            }
          
            
         


        }

        private void btnScanStop_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Scan Move Stop Click");

            Func.StopScan();
        }

        private void btnScanHome_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Scan X,Y Home Click");

            FuncMotion.MoveHome((uint)actionAxis);
        }

        private void btnInShuttle_Click(object sender, EventArgs e)
        {
            btnLift1.BackColor = Color.White;
            btnLift2.BackColor = Color.White;
            btnInShuttle.BackColor = Color.Lime;
            btnOutShuttle.BackColor = Color.White;
            activeNum = 2;
            actionAxis = FuncInline.enumServoAxis.SV00_In_Shuttle;

            //티칭위치 선택이 아닌 서보모터 선택시 지정된 티칭위치로 이동
            if (isLift)
            {
                siteIndex = -1;
            }
            //activePanelPos = "btnPosInShuttle_InConLoading";

            SetLiftPosition();
            lblTeaching.Text = FuncInline.ShuttlePos[activeNum - 2, (int)activeShuttlePos].ToString("F2");
        }

        private void btnOutShuttle_Click(object sender, EventArgs e)
        {
            btnLift1.BackColor = Color.White;
            btnLift2.BackColor = Color.White;
            btnInShuttle.BackColor = Color.White;
            btnOutShuttle.BackColor = Color.Lime;
            activeNum = 3;
            actionAxis = FuncInline.enumServoAxis.SV01_Out_Shuttle;
            //티칭위치 선택이 아닌 서보모터 선택시 지정된 티칭위치로 이동
            if (isLift)
            {
                siteIndex = -1;
            }
            SetLiftPosition();
            lblTeaching.Text = FuncInline.ShuttlePos[activeNum - 2, (int)activeShuttlePos].ToString("F2");
        }

        void PaintPosButtons(string activeName) //패널 그룹화 해서 컨트롤 찾기 인셔틀 추가
        {
            Control[] panels = { pnPos, pnshuttlePos }; // ← 둘을 묶어서

            foreach (var panel in panels)
            {

                foreach (Control conButton in groupBox1.Controls)
                {
                    if (conButton.Name.StartsWith("btnPos"))
                    {
                        if (conButton.Name == activeName)
                        {
                            ((Button)conButton).BackColor = Color.Lime;
                        }
                        else
                        {
                            ((Button)conButton).BackColor = Color.White;
                        }
                    }
                }
                foreach (Control conButton in groupBox2.Controls)
                {
                    if (conButton.Name.StartsWith("btnPos"))
                    {
                        if (conButton.Name == activeName)
                        {
                            ((Button)conButton).BackColor = Color.Lime;
                        }
                        else
                        {
                            ((Button)conButton).BackColor = Color.White;
                        }
                    }
                }
                foreach (Control conButton in groupBox3.Controls)
                {
                    if (conButton.Name.StartsWith("btnPos"))
                    {
                        if (conButton.Name == activeName)
                        {
                            ((Button)conButton).BackColor = Color.Lime;
                        }
                        else
                        {
                            ((Button)conButton).BackColor = Color.White;
                        }
                    }
                }
                foreach (Control conButton in groupBox4.Controls)
                {
                    if (conButton.Name.StartsWith("btnPos"))
                    {
                        if (conButton.Name == activeName)
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

        private void btnInShuttleTurnStart_Click(object sender, EventArgs e)
        {
            DIO.DoubleSol(FuncInline.enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder, !DIO.GetDOData(FuncInline.enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder));
        }

        private void btnOutShuttleTurnStart_Click(object sender, EventArgs e)
        {
            DIO.DoubleSol(FuncInline.enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder, !DIO.GetDOData(FuncInline.enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder));
        }
    }
}
