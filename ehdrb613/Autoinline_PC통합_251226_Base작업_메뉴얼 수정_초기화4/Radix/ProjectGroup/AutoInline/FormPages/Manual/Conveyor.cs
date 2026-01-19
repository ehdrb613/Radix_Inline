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

    public partial class Conveyor : Form
    {
        #region 로컬 Type 정의

        #endregion

        #region 로컬변수
        private System.Threading.Timer timerCheck = null;
        private bool timerCheckDoing = false;
        private System.Threading.Timer timerJog = null; // 모터 조그 제어용
        private bool timerJogDoing = false;

        private FuncInline.enumTeachingPos manualPos = FuncInline.enumTeachingPos.InConveyor;
        int teachingIndex = 0; // 선택된 모터 순번. 스탭모터부터
        FuncInline.enumServoAxis activeServoAxis = FuncInline.enumServoAxis.SV00_In_Shuttle;
        bool activeServo = false; // 축이 서보로 지정되어 있는가?
        FuncInline.enumLetsAxis activeStepAxis = FuncInline.enumLetsAxis.ST00_InShuttle_Width;

        private bool jogWidth = false;
        private bool jogWidthUp = false;
        private bool jogWidthDown = false;

       FuncInline.enumTabMain beforeTabMain = FuncInline.TabMain; // 조그 멈추기용
       FuncInline.enumTabMain beforeTabMain2 = FuncInline.TabMain; // 메뉴얼 동작 프로세스 정지용
       FuncInline.enumTabManual beforeTabManual = FuncInline.TabManual; // 조그 멈추기용
       FuncInline.enumTabManual beforeTabManual2 = FuncInline.TabManual; // 메뉴얼 동작 프로세스 정지용

        //private Stopwatch jogWatch = new Stopwatch(); // 조그 정지 위해
        #endregion

        #region 초기화 관련

        public Conveyor()
        {
            InitializeComponent();
        }

        private void Manual_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;

            // 타이머 시작
            TimerCallback CallBackCheck = new TimerCallback(TimerCheck);
            timerCheck = new System.Threading.Timer(CallBackCheck, false, 0, 100);

            TimerCallback CallBackJog = new TimerCallback(TimerJog);
            timerJog = new System.Threading.Timer(CallBackJog, false, 0, 100);

            this.BringToFront();

            #region Autonics Step Motor 상태조회 Flag 세팅
            //debug("manual ui time : " + (Environment.TickCount - startTime).ToString());
            #endregion

        }

        private void Manual_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (this.Parent != null)
            //{
            try
            {
                GlobalVar.dlgOpened = false;
                //timerCheck.Dispose();
                //timerJog.Dispose();
                //timerMotor.Dispose(); // 사용 안 함
                //this.Parent.BringToFront();
            }
            catch
            { }
            //}
        }

        #endregion

        #region 타이머 함수
        private void TimerCheck(Object state)
        {
            //timerCheckDoing = false;
            if (FuncInline.TabMain != FuncInline.enumTabMain.Manual ||
                FuncInline.TabManual != FuncInline.enumTabManual.Conveyor) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                //pnHide.Visible = false;
                timerCheckDoing = false;
                return;
            }
            try
            {
                if (timerCheckDoing)
                {
                    return;
                }

                timerCheckDoing = true;

                /* 창모드가 아니라 닫거나 리턴할 수 없다..
                if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중 자동 닫기
                {
                    this.BringToFront();
                    FuncWin.TopMessageBox("Can't use manual move while system is running.");
                    //this.Close();
                    timerDoing = false;
                    return;
                }
                //*/

                /* 창모드가 아니라 닫거나 리턴할 수 없다..
                if (GlobalVar.UseDoor)
                {
                    if (!GlobalVar.Simulation &&
                    (DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                    DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                    DIO.GetDIData(FuncInline.enumDINames.X00_4_Front_Door3)))
                    {
                        this.BringToFront();
                        FuncWin.TopMessageBox("Can't use manual move while doors are opened");
                        //this.Close();
                        timerDoing = false;
                        return;
                    }
                }
                //*/

                if (!GlobalVar.GlobalStop &&
                    this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {

                        int startTime = Environment.TickCount;

                        #region 버튼 UI 변경


                        switch (manualPos)
                        {
                            // ───────── InConveyor ─────────
                            case FuncInline.enumTeachingPos.InConveyor:

                                // 컨베이어 모터: CW/CCW DO 기준
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw);
                                FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw);
                                btnStopConveyor.BackColor =
                                    !DIO.GetDORead(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw) &&
                                    !DIO.GetDORead(FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw)
                                    ? Color.Lime : Color.White;
                                break;

                            
                                break;
                            case FuncInline.enumTeachingPos.FrontPassLine:

                                
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw) ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.Lift1_Up:

                                FuncForm.SetButtonDoColor(btnInStopperOpen, FuncInline.enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL);
                              
                       
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw);
                                FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw) && !DIO.GetDORead(FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw)
                                                            ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.Lift1_Down:


                                FuncForm.SetButtonDoColor(btnInStopperOpen, FuncInline.enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL);

                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw);
                                FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw) && !DIO.GetDORead(FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw)
                                                            ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.RearPassLine:


                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y305_4_Rear_PassLine_Motor_Cw);
                                btnStopConveyor.BackColor =
                                    !DIO.GetDORead(FuncInline.enumDONames.Y305_4_Rear_PassLine_Motor_Cw)
                                    ? Color.Lime : Color.White;

                                break;
                            case FuncInline.enumTeachingPos.Lift2_Up:

                                // 스토퍼 : Rear Lift IN(상부)용 DO
                                FuncForm.SetButtonDoColor(btnInStopperOpen, FuncInline.enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL);
                                FuncForm.SetButtonColor2(
                                    btnOutStopperOpen,
                                    !DIO.GetDORead(FuncInline.enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL));

                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw);
                                FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) && !DIO.GetDORead(FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw)
                                                            ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.Lift2_Down:

                                // 스토퍼 : Rear Lift IN(상부)용 DO
                                FuncForm.SetButtonDoColor(btnInStopperOpen, FuncInline.enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL);
                                FuncForm.SetButtonColor2(
                                    btnOutStopperOpen,
                                    !DIO.GetDORead(FuncInline.enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL));

                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw);
                                FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw) && !DIO.GetDORead(FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw)
                                                            ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.OutConveyor:

                          
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw);
                            
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw) ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.NgBuffer:

                         
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw);
                                //FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw) ? Color.Lime : Color.White;
                                FuncForm.SetButtonColor3(btnNGForward,
                                                        (DIO.GetDIData(FuncInline.enumDINames.X03_5_NgBuffer_LowerForwardSensor)) && DIO.GetDIData(FuncInline.enumDINames.X03_1_NgBuffer_UpperForwardSensor),
                                                        DIO.GetDIData(FuncInline.enumDINames.X03_6_NgBuffer_LowerBackwardSensor) && DIO.GetDIData(FuncInline.enumDINames.X03_2_NgBuffer_UpperBackwardSensor));
                               FuncForm.SetButtonColor3(btnNGReward,
                                                        DIO.GetDIData(FuncInline.enumDINames.X03_6_NgBuffer_LowerBackwardSensor) && DIO.GetDIData(FuncInline.enumDINames.X03_2_NgBuffer_UpperBackwardSensor),
                                                        (DIO.GetDIData(FuncInline.enumDINames.X03_5_NgBuffer_LowerForwardSensor)) && DIO.GetDIData(FuncInline.enumDINames.X03_1_NgBuffer_UpperForwardSensor));
                                btnNGStop.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y412_7_NgBuffer_Lower_cylinder_forward) &&
                                                                    !DIO.GetDORead(FuncInline.enumDONames.Y4_7_Ngbuffer_Lower_cylinder_backward) &&
                                                                    !DIO.GetDORead(FuncInline.enumDONames.Y412_6_Ngbuffer_Upper_cylinder_forward) &&
                                                                    !DIO.GetDORead(FuncInline.enumDONames.Y412_5_Ngbuffer_Upper_cylinder_backward)
                                                        ? Color.Lime : Color.White;
                                break;
                        }

                        #endregion

                        if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length) // 스텝모터
                        {
                            pbAxisStatus.Visible = false;
                            lblCurrPos.Text = GlobalVar.LetsAxisStatus[(int)activeStepAxis].Position.ToString("F2");
                        }
                        else // 서보 모터
                        {
                            pbAxisStatus.Visible = true;
                            FuncForm.SetServoStateColor(pbAxisStatus, activeServoAxis);
                            lblCurrPos.Text = GlobalVar.AxisStatus[(int)activeStepAxis].Position.ToString("F2");
                        }

                        #region 버튼 UI 변경

                        #endregion
                    }));
                }

                //timerDoing = false;

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            timerCheckDoing = false;
            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    timerCheck = new System.Threading.Timer(new TimerCallback(TimerCheck), false, 0, 100);
            //}
            if (GlobalVar.GlobalStop)
            {
                try
                {
                    timerCheck.Dispose();
                }
                catch { }
            }
        }

        /*
        private void SetStepFlag()
        {
            if (GlobalVar.SystemStatus != enumSystemStatus.Initialize &&
                GlobalVar.SystemStatus < enumSystemStatus.AutoRun) // 초기화나 정지시만 조회 flag를 조정한다.
            {
                for (int i = 0; i < GlobalVar.LetsAxisStatus.Length; i += 2)
                {
                    bool flag = false;

                    int motion = (int)manualPos / 2;
                    if (manualPos != FuncInline.enumTeachingPos.NgBuffer &&
                        i == motion * 2)
                    {
                        flag = true;
                    }

                    GlobalVar.LetsAxisStatus[i].ReadFlag = flag;
                    if (i + 1 < GlobalVar.LetsAxisStatus.Length)
                    {
                        GlobalVar.LetsAxisStatus[i + 1].ReadFlag = flag;
                    }
                }
            }
        }
        //*/

        private void TimerJog(Object state)
        {
            #region 창 떠나면 조그 멈추기
            if (beforeTabMain == FuncInline.enumTabMain.Teaching &&
                FuncInline.TabMain != FuncInline.enumTabMain.Teaching)
            {
                FuncInlineMove.StopAllJog(true);
            }
            beforeTabMain = FuncInline.TabMain;
            #endregion

            if (FuncInline.TabMain != FuncInline.enumTabMain.Manual ||
                FuncInline.TabManual != FuncInline.enumTabManual.Conveyor) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerJogDoing = false;
                return;
            }
            try
            {
                //timerJog.Dispose();
                if (timerJogDoing)
                {
                    return;
                }
                timerJogDoing = true;


                #region JOG 멈추기
                #region 로봇Z축
                if (jogWidth) //
                {
                    if (!jogWidthDown &&
                        !jogWidthUp)
                    {
                        if (GlobalVar.E_Stop ||
                            ((teachingIndex >= Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length && GlobalVar.AxisStatus[(int)activeServoAxis].Errored)) ||
                            (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length && GlobalVar.LetsAxisStatus[(int)activeStepAxis].Errored))
                        {
                            if (teachingIndex >= Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length)
                            {
                           
                                FuncMotion.MoveStop((int)activeServoAxis);
                            }
                            else
                            {
                                
                                FuncLetsMotion.Stop((int)activeStepAxis);
                            }
                            jogWidth = false;
                        }
                       
                        else
                        {
                            jogWidth = false;
                        }
                    }
                }
                #endregion


                #endregion

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
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

        #region 컨베어 관련 이벤트

        private void btnInStopperOpen_Click(object sender, EventArgs e)
        {
            // CONTACT_STOPPER_SOL 만 사용 — 위치별 매핑
            FuncInline.enumDONames stopperDO;

            switch (manualPos)
            {
                // 인컨/인셔틀 → 인셔틀 컨택트 스토퍼

                case FuncInline.enumTeachingPos.InShuttle:
                    stopperDO = FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL;
                    break;

                // Lift1 = Front Lift (Up/Down 공통 컨택트 스토퍼)
                case FuncInline.enumTeachingPos.Lift1_Up:
                case FuncInline.enumTeachingPos.Lift1_Down:
                    stopperDO = FuncInline.enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL;
                    break;

                // Lift2 = Rear Lift
                case FuncInline.enumTeachingPos.Lift2_Up:
                case FuncInline.enumTeachingPos.Lift2_Down:
                    stopperDO = FuncInline.enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL;
                    break;

                // 아웃 셔틀 (OK/NG)
                case FuncInline.enumTeachingPos.OutShuttle_Up:
                case FuncInline.enumTeachingPos.OutShuttle_Down:
                    stopperDO = FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_IN_SOL;
                    break;

                // CONTACT_STOPPER_SOL 없는 구간(OutConveyor, NgBuffer 등)은 스킵
                default:
                    return;
            }
            
            // Down 동작: SOL OFF
            DIO.WriteDOData(stopperDO, !DIO.GetDORead(stopperDO));
        }
       

        private void btnRunConveyor_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;
            bool cw = name == "btnRunConveyorCW";
            FuncInline.enumDONames o = FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw;
           FuncInline.enumDONames s = FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw;
            switch (manualPos)
            {
                case FuncInline.enumTeachingPos.FrontPassLine:
                    o = FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw;
                    break;
                case FuncInline.enumTeachingPos.Lift1_Up:
                    o = FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw;
                    s = FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw;
                    break;
                case FuncInline.enumTeachingPos.Lift1_Down:
                    o = FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw;
                    s = FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw;
                    break;
                case FuncInline.enumTeachingPos.RearPassLine:
                    o = FuncInline.enumDONames.Y305_4_Rear_PassLine_Motor_Cw;
                    break;
                case FuncInline.enumTeachingPos.Lift2_Up:
                    o = FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw;
                    s = FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw;
                    break;
                case FuncInline.enumTeachingPos.Lift2_Down:
                    o = FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw;
                    s = FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw;
                    break;
                case FuncInline.enumTeachingPos.OutConveyor:
                    o = FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw;
                    break;
                case FuncInline.enumTeachingPos.NgBuffer:
                    o = FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw;
                    break;
            }
            if (s != FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw)
            {
                DIO.WriteDOData(s, !cw);
            }
            DIO.WriteDOData(o, cw);
        }

       


        private void pbMoveConveyor_Click(object sender, EventArgs e)
        {
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

        


            if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length) // 스텝모터
            {
                FuncLetsMotion.ABSMove((int)activeStepAxis, (double)numMovePos.Value, (double)numSpeed.Value);
            }
            else // 서보모터
            {
                FuncMotion.MoveAbsolute((uint)activeServoAxis, (double)numMovePos.Value, (double)numSpeed.Value);
            }
        }

        private void pbHomeConveyor_Click(object sender, EventArgs e)
        {
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

  



            if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length) // 스텝모터
            {
                FuncLetsMotion.HomeRun((int)activeStepAxis);
                //PMCClass.HomeRun(activeStepAxis, GlobalVar.WidthHomSpeed);
            }
            else // 서보모터
            {
                FuncInlineMove.MoveHome((uint)activeServoAxis);
            }

        }

        private void pbStopConveyor_Click(object sender, EventArgs e)
        {
            if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length) // 스텝모터
            {
                FuncLetsMotion.Stop((int)activeStepAxis);
                //PMCClass.Stop(activeStepAxis);
            }
            else // 서보모터
            {
                FuncMotion.MoveStop((int)activeServoAxis);
            }
        }

        private void pbNarrowPosConveyor_Click(object sender, EventArgs e)
        {
            numMovePos.Value = (decimal)FuncInline.TeachingWidth[(int)teachingIndex];
        }

        #endregion



        #region 기타 함수
        private void debug(string str)
        {
            Util.Debug("Manual : " + str);
        }

        #endregion

        #region 조그 이벤트

        private void pbJogNarrowConveyor_MouseDown(object sender, MouseEventArgs e)
        {
            #region 마우스 이외의 방법으로 클릭시 동작 방지
            /*
            if (e.Button == MouseButtons.None)
            {
                FuncWin.TopMessageBox("Use mouse at jog action.");
                return;
            }
            //*/

            #endregion

            if (btnSpeed.BackColor == Color.Lime &&
                 CheckPCBExist())
            {
                //FuncWin.TopMessageBox("Can't move while PCB exists.");
                return;
            }

            FuncInlineMove.StopAllJog(true);



            if (btnPitch.BackColor == Color.Lime)
            {
                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length)
                {
                    FuncLetsMotion.ABSMove((int)activeStepAxis, GlobalVar.LetsAxisStatus[(int)activeStepAxis].Position - (double)numPitch.Value, (double)numSpeed.Value);
                   
                }
                else
                {
                    FuncMotion.MoveAbsolute((uint)activeServoAxis,
                                        double.Parse(lblTeachingWidth.Text) - (double)numPitch.Value,
                                        (double)numSpeed.Value);
                }
            }
            else
            {
                jogWidth = true;
                jogWidthDown = true;

                //스텝모터일때
                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length)
                {
                    FuncLetsMotion.INCMove((int)activeStepAxis, -500, (double)numSpeed.Value);
                }
                //서보일때
                else
                {
                    FuncMotion.MoveAbsolute((uint)activeServoAxis,
                                  -500,
                                    (double)numSpeed.Value);
                }
            }
        }

        private void pbJogNarrowConveyor_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length)
                {
                    FuncLetsMotion.Stop((int)activeStepAxis);
                }
                else
                {
                    FuncMotion.MoveStop((int)activeServoAxis);
                }
                //FuncInlineMove.StopAllJog(false);
                jogWidthDown = false;
                //Util.StartWatch(jogWatch);

            }
        }

        private void pbJogWideConveyor_MouseDown(object sender, MouseEventArgs e)
        {
            #region 마우스 이외의 방법으로 클릭시 동작 방지
            /*
            if (e.Button == MouseButtons.None)
            {
                FuncWin.TopMessageBox("Use mouse at jog action.");
                return;
            }
            //*/

            #endregion

            if (btnSpeed.BackColor == Color.Lime &&
                CheckPCBExist())
            {
                //FuncWin.TopMessageBox("Can't move while PCB exists.");
                return;
            }

            FuncInlineMove.StopAllJog(true);



            double speed = (double)numSpeed.Value;

            //피치 이동
            if (btnPitch.BackColor == Color.Lime)
            {
                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length)
                {
                    FuncLetsMotion.ABSMove((int)activeStepAxis, GlobalVar.LetsAxisStatus[(int)activeStepAxis].Position + (double)numPitch.Value, (double)numSpeed.Value);
                    //PMCClass.ABSMove(activeStepAxis, GlobalVar.LetsAxisStatus[(int)activeStepAxis].Position + (double)numPitch.Value, (double)numSpeed.Value);
                }
                else
                {

                    FuncMotion.MoveAbsolute((uint)activeServoAxis,
                                    double.Parse(lblCurrPos.Text) + (double)numPitch.Value,
                                    (double)numSpeed.Value);

                }
            }
            //스피드 이동
            else
            {

                jogWidth = true;
                jogWidthUp = true;

                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length)
                {
                    FuncLetsMotion.INCMove((int)activeStepAxis, 500, (double)numSpeed.Value);
                }
                else
                {
                    FuncMotion.MoveAbsolute((uint)activeServoAxis,
                                  500,
                                    (double)numSpeed.Value);
                }
            }
        }

        private void pbJogWideConveyor_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                if (teachingIndex < Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length)
                {
                    FuncLetsMotion.Stop((int)activeStepAxis);
                }
                else
                {
                    FuncMotion.MoveStop((int)activeServoAxis);
                }
                //FuncInlineMove.StopAllJog(false);
                jogWidthUp = false;
                //Util.StartWatch(jogWatch);

            }
        }




        #endregion



        private void btnStopperDownConveyor_Click(object sender, EventArgs e)
        {
            // CONTACT_STOPPER_SOL 만 사용 — 위치별 매핑
            FuncInline.enumDONames stopperDO;

            switch (manualPos)
            {
                // 인컨/인셔틀 → 인셔틀 컨택트 스토퍼
                case FuncInline.enumTeachingPos.InConveyor:
                case FuncInline.enumTeachingPos.InShuttle:
                    stopperDO = FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL;
                    break;

                // 프론트 패스라인
                case FuncInline.enumTeachingPos.FrontPassLine:
                    stopperDO = FuncInline.enumDONames.Y1_7_Front_PASSLINE_PCB_STOPPER_SOL;
                    break;

                // 리어 OK 패스라인
                case FuncInline.enumTeachingPos.RearPassLine:
                    stopperDO = FuncInline.enumDONames.Y1_6_Rear_OK_PassLine_CONTACT_STOPPER_SOL;
                    break;

                // Lift1 = Front Lift (Up/Down 공통 컨택트 스토퍼)
                case FuncInline.enumTeachingPos.Lift1_Up:
                case FuncInline.enumTeachingPos.Lift1_Down:
                    stopperDO = FuncInline.enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL;
                    break;

                // Lift2 = Rear Lift (Up=IN, Down=OUT 컨택트 스토퍼)
                case FuncInline.enumTeachingPos.Lift2_Up:
                    stopperDO = FuncInline.enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL;
                    break;
                case FuncInline.enumTeachingPos.Lift2_Down:
                    stopperDO = FuncInline.enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL;
                    break;

                // 아웃 셔틀 (OK/NG)
                case FuncInline.enumTeachingPos.OutShuttle_Up:
                    stopperDO = FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_IN_SOL;
                    break;
                case FuncInline.enumTeachingPos.OutShuttle_Down:
                    stopperDO = FuncInline.enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_Out_SOL;
                    break;

                // 프론트 스캔 사이트
                case FuncInline.enumTeachingPos.FrontScanSite:
                    stopperDO = FuncInline.enumDONames.Y3_7_Front_SCAN_STOPPER_SOL;
                    break;

                // CONTACT_STOPPER_SOL 없는 구간(OutConveyor, NgBuffer 등)은 스킵
                default:
                    return;
            }

            // Down 동작: SOL OFF
            DIO.WriteDOData(stopperDO, false);
        }

        private void btnStopConveyor_Click(object sender, EventArgs e)
        {
            FuncInline.enumDONames o = FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw;
            FuncInline.enumDONames s = FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw;
            switch (manualPos)
            {
                case FuncInline.enumTeachingPos.FrontPassLine:
                    o = FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw;
                    break;
                case FuncInline.enumTeachingPos.Lift1_Up:
                    o = FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw;
                    s = FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw;
                    break;
                case FuncInline.enumTeachingPos.Lift1_Down:
                    o = FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw;
                    s = FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw;
                    break;
                case FuncInline.enumTeachingPos.RearPassLine:
                    o = FuncInline.enumDONames.Y305_4_Rear_PassLine_Motor_Cw;
                    break;
                case FuncInline.enumTeachingPos.Lift2_Up:
                    o = FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw;
                    s = FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw;
                    break;
                case FuncInline.enumTeachingPos.Lift2_Down:
                    o = FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw;
                    s = FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw;
                    break;
                case FuncInline.enumTeachingPos.OutConveyor:
                    o = FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw;
                    break;
                case FuncInline.enumTeachingPos.NgBuffer: // NG
                    o = FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw;
                    break;
            }
            DIO.WriteDOData(o, false);
            if (s != FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw)
            {
                DIO.WriteDOData(s, false);
            }
        }

        private void btnNGReward_Click(object sender, EventArgs e)
        {
            if (manualPos == FuncInline.enumTeachingPos.NgBuffer)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (!GlobalVar.GlobalStop &&
                    watch.ElapsedMilliseconds < 10 * 1000 &&
                    !GlobalVar.SystemErrored)
                {
                    if (FuncInline.MoveNGSylinder(FuncInline.enumNGAction.Backward))
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                //DIO.DoubleSol(FuncInline.enumDONames.Y412_7_NgBuffer_Lower_cylinder_forward, false);
                //DIO.DoubleSol(FuncInline.enumDONames.Y412_6_Ngbuffer_Upper_cylinder_forward, false);
            }
        }

        private void btnNGForward_Click(object sender, EventArgs e)
        {
            if (manualPos == FuncInline.enumTeachingPos.NgBuffer)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (!GlobalVar.GlobalStop &&
                    watch.ElapsedMilliseconds < 10 * 1000 &&
                    !GlobalVar.SystemErrored)
                {
                    if (FuncInline.MoveNGSylinder(FuncInline.enumNGAction.Forward))
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
              
            }
        }

        private void btnNGStop_Click(object sender, EventArgs e)
        {
            DIO.WriteDOData(FuncInline.enumDONames.Y412_7_NgBuffer_Lower_cylinder_forward, false);
            DIO.WriteDOData(FuncInline.enumDONames.Y4_7_Ngbuffer_Lower_cylinder_backward, false);
            DIO.WriteDOData(FuncInline.enumDONames.Y412_6_Ngbuffer_Upper_cylinder_forward, false);
            DIO.WriteDOData(FuncInline.enumDONames.Y412_5_Ngbuffer_Upper_cylinder_backward, false);
        }
        // pnPos 안의 "btnPos..." 버튼만 대상으로 하이라이트
        private void HighlightPnPos(Button clicked)
        {
            if (clicked == null) return;

            foreach (Control conPos in pnPos.Controls)
            {
                if (conPos is Button b && b.Name.StartsWith("btnPos", StringComparison.OrdinalIgnoreCase))
                {
                    b.BackColor = (b == clicked) ? Color.Lime : Color.White;
                }
            }
        }
        private void btnPosClick(object sender, EventArgs e)
        {
            string sender_name = ((Button)sender).Name;
            //debug("sender_name : " + sender_name);
            ((Button)sender).BackColor = Color.Lime;

            switch (sender_name)
            {
                case "btnPosInConveyor":
                    manualPos = FuncInline.enumTeachingPos.InShuttle;

                    HighlightPnPos((Button)sender); //선택 버튼 색상변경

                    btnRunConveyorCW.Enabled = true;

                    btnInStopperOpen.Enabled = false;
                    btnOutStopperOpen.Enabled = false;
                    btnClampOn.Enabled = false;
                    btnRunConveyorCCW.Enabled = false;
                    btnNGForward.Enabled = false;
                    btnNGReward.Enabled = false;
                    btnNGStop.Enabled = false;

                    btnInStopperOpen.Visible = true;
                    btnOutStopperOpen.Visible = true;
                    btnClampOn.Visible = true;
                    btnRunConveyorCW.Visible = true;
                    btnRunConveyorCCW.Visible = true;


                    btnNGForward.Visible = false;
                    btnNGReward.Visible = false;
                    btnNGStop.Visible = false;

                    teachingIndex = (int)FuncInline.enumLetsAxis.ST00_InShuttle_Width;
                    activeStepAxis = FuncInline.enumLetsAxis.ST00_InShuttle_Width;
                    activeServo = false; // 축이 서보로 지정되어 있는가?
                    lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
                    break;

                case "btnPosInShuttle":
                    manualPos = FuncInline.enumTeachingPos.InShuttle;

                    HighlightPnPos((Button)sender); //선택 버튼 색상변경

                    btnInStopperOpen.Enabled = true;
                    btnOutStopperOpen.Enabled = true;
                    btnClampOn.Enabled = true;
                    btnRunConveyorCW.Enabled = true;
                    btnRunConveyorCCW.Enabled = true;

                    btnInStopperOpen.Visible = true;
                    btnOutStopperOpen.Visible = true;
                    btnClampOn.Visible = true;
                    btnRunConveyorCW.Visible = true;
                    btnRunConveyorCCW.Visible = true;

                    btnNGForward.Enabled = false;
                    btnNGReward.Enabled = false;
                    btnNGStop.Enabled = false;

                    btnNGForward.Visible = false;
                    btnNGReward.Visible = false;
                    btnNGStop.Visible = false;
                    teachingIndex = (int)FuncInline.enumLetsAxis.ST00_InShuttle_Width;
                    activeStepAxis = FuncInline.enumLetsAxis.ST00_InShuttle_Width;
                    activeServo = false; // 축이 서보로 지정되어 있는가?
                    lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
                    break;

            }
        }
        private void btnPosInputConveyor_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.InShuttle;

            HighlightPnPos((Button)sender); //선택 버튼 색상변경

            btnInStopperOpen.Enabled = true;
            btnOutStopperOpen.Enabled = true;
            btnClampOn.Enabled = true;
            btnRunConveyorCW.Enabled = true;
            btnRunConveyorCCW.Enabled = true;

            btnInStopperOpen.Visible = true;
            btnOutStopperOpen.Visible = true;
            btnClampOn.Visible = true;
            btnRunConveyorCW.Visible = true;
            btnRunConveyorCCW.Visible = true;
      
            btnNGForward.Enabled = false;
            btnNGReward.Enabled = false;
            btnNGStop.Enabled = false;

            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = (int)FuncInline.enumLetsAxis.ST00_InShuttle_Width;
            activeStepAxis = FuncInline.enumLetsAxis.ST00_InShuttle_Width;

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosPassLine1_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.FrontPassLine;
         
            HighlightPnPos((Button)sender); //선택 버튼 색상변경

            
            btnClampOn.Enabled = true;
            btnRunConveyorCW.Enabled = true;
            

            btnInStopperOpen.Visible = true;
            btnOutStopperOpen.Visible = true;
            btnClampOn.Visible = true;
            btnRunConveyorCW.Visible = true;
            btnRunConveyorCCW.Visible = true;

            btnInStopperOpen.Enabled = false;
            btnOutStopperOpen.Enabled = false;
            btnNGForward.Enabled = false;
            btnNGReward.Enabled = false;
            btnNGStop.Enabled = false;
            btnRunConveyorCCW.Enabled = false;

            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
         
            teachingIndex = 3;
            activeServoAxis = FuncInline.enumServoAxis.SV03_Rack1_Width;
          

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosLift1Up_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.Lift1_Up;
            HighlightPnPos((Button)sender); //선택 버튼 색상변경

       

            btnInStopperOpen.Visible = true;
            btnOutStopperOpen.Visible = true;
            btnRunConveyorCCW.Visible = true;
            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = (int)FuncInline.enumLetsAxis.ST04_NGBuffer + 1;
            
            activeServoAxis = FuncInline.enumServoAxis.SV03_Rack1_Width;


            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosLift1Down_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.Lift1_Down;

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.White;

            btnInStopperOpen.Visible = true;
            btnOutStopperOpen.Visible = true;
            btnRunConveyorCCW.Visible = true;
            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = 3;
            activeServoAxis = FuncInline.enumServoAxis.SV03_Rack1_Width;
            

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosPassLine2_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.RearPassLine;
     
            HighlightPnPos((Button)sender); //선택 버튼 색상변경

            btnInStopperOpen.Visible = true;
            btnOutStopperOpen.Visible = true;
            btnRunConveyorCCW.Visible = false;
            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = 4;

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosLift2Up_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.Lift2_Up;
           
            HighlightPnPos((Button)sender); //선택 버튼 색상변경
        

            btnInStopperOpen.Visible = true;
            btnOutStopperOpen.Visible = true;
            btnRunConveyorCCW.Visible = true;
            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = 4;
            activeServoAxis = FuncInline.enumServoAxis.SV05_Rack2_Width;
         

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosLift2Down_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.Lift2_Down;

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.White;

            btnInStopperOpen.Visible = true;
            btnOutStopperOpen.Visible = true;
            btnRunConveyorCCW.Visible = true;
            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = 4;
            activeServoAxis = FuncInline.enumServoAxis.SV05_Rack2_Width;
          

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosOutputConveyor_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.OutConveyor;
    
            HighlightPnPos((Button)sender); //선택 버튼 색상변경

            btnInStopperOpen.Visible = true;
            btnOutStopperOpen.Visible = true;
            btnRunConveyorCCW.Visible = false;
            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = 1;
            activeStepAxis = FuncInline.enumLetsAxis.ST02_OutConveyor_Width;
            activeServoAxis = FuncInline.enumServoAxis.SV05_Rack2_Width;
          

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosNG_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.NgBuffer;

            HighlightPnPos((Button)sender); //선택 버튼 색상변경

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.Lime;

            btnInStopperOpen.Visible = false;
            btnOutStopperOpen.Visible = false;
            btnRunConveyorCCW.Visible = false;
            btnNGForward.Visible = true;
            btnNGReward.Visible = true;
            btnNGStop.Visible = true;
            teachingIndex = 2;
            activeStepAxis = FuncInline.enumLetsAxis.ST04_NGBuffer;

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
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

        private void btnPosInShuttle_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.InShuttle;

            HighlightPnPos((Button)sender); //선택 버튼 색상변경
        }

        private void btnPosFrontScanSite_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.FrontScanSite;

            HighlightPnPos((Button)sender); //선택 버튼 색상변경

            btnInStopperOpen.Enabled = true;
            btnOutStopperOpen.Enabled = true;
            btnClampOn.Enabled = true;
            btnRunConveyorCW.Enabled = true;
            btnRunConveyorCCW.Enabled = true;

            btnInStopperOpen.Visible = true;
            btnOutStopperOpen.Visible = true;
            btnClampOn.Visible = true;
            btnRunConveyorCW.Visible = true;
            btnRunConveyorCCW.Visible = true;

            btnNGForward.Enabled = false;
            btnNGReward.Enabled = false;
            btnNGStop.Enabled = false;

            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = (int)FuncInline.enumLetsAxis.ST00_InShuttle_Width;
            activeStepAxis = FuncInline.enumLetsAxis.ST00_InShuttle_Width;

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosRearNGLine_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.RearNGLine;
            HighlightPnPos((Button)sender); //선택 버튼 색상변경
        }

        private void btnPosOutShuttleUp_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.OutShuttle_Up;
            HighlightPnPos((Button)sender); //선택 버튼 색상변경
        }

        private void btnPosOutShuttleDown_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.OutShuttle_Down;
            HighlightPnPos((Button)sender); //선택 버튼 색상변경
        }

        private void btnClampOn_Click(object sender, EventArgs e)
        {

        }

        private bool CheckPCBExist()
        {
            //IN Shuttle
            if (teachingIndex == 0 &&
                            (DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                                    DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
                                    DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in In Conveyor. Manual move disabled while PCB exist");
                return true;
            }
            //Out ShuttleUP
            if (teachingIndex == 1 &&
                (DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in Out Shuttle Up. Manual move disabled while PCB exist");
                return true;
            }
            //Out ShuttleDown
            if (teachingIndex == 1 &&
                (DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in Out Shuttle Down. Manual move disabled while PCB exist");
                return true;
            }

            //Out ConveyorUP
            if (teachingIndex == 2 &&
                (DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in Out Conveyor Up. Manual move disabled while PCB exist");
                return true;
            }


            //In Conveyor
            if (teachingIndex == 3 &&
                (DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected In Conveyor. Manual move disabled while PCB exist");
                return true;
            }
            //NG buffer(OutConveyorDown)
            if (teachingIndex == 4 &&
                (DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor)))
            {
                FuncWin.TopMessageBox("PCB detected in NG buffer. Manual move disabled while PCB exist");
                return true;
            }

            // Front Rack,Lift
            if (teachingIndex == 5)
            {
                // 1) Front 측 사이트(1~13) PCB 도크 센서로 체크
                var frontSites = new FuncInline.enumTeachingPos[]
                {
                    FuncInline.enumTeachingPos.Site1_F_DT1,
                    FuncInline.enumTeachingPos.Site2_F_DT2,
                    FuncInline.enumTeachingPos.Site3_F_DT3,
                    FuncInline.enumTeachingPos.Site4_F_DT4,
                    FuncInline.enumTeachingPos.Site5_F_DT5,
                    FuncInline.enumTeachingPos.Site6_F_DT6,
                    FuncInline.enumTeachingPos.Site7_F_DT7,
                    FuncInline.enumTeachingPos.Site8_F_DT8,
                    FuncInline.enumTeachingPos.Site9_F_DT9,
                    FuncInline.enumTeachingPos.Site10_F_DT10_FT4,
                    FuncInline.enumTeachingPos.Site11_F_FT1,
                    FuncInline.enumTeachingPos.Site12_F_FT2,
                    FuncInline.enumTeachingPos.Site13_F_FT3,
                };

                for (int i = 0; i < frontSites.Length; i++)
                {
                    var site = frontSites[i];
                    if (FuncInline.SiteIoMaps.TryGetPcbDockDI(site, out var di) && DIO.GetDIData(di))
                    {
                        //int siteNo = (int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1; // 1-based
                        string label = FuncInline.SiteDisplay.GetSiteDisplayName(site);

                        FuncWin.TopMessageBox($"PCB detected in Site #{label}. Manual move disabled while PCB exist");
                        return true;
                    }
                }

                // 2) Front Lift 컨베이어(상/하) PCB 감지
                if (DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("PCB detected in Front Lift Conveyor. Manual move disabled while PCB exist");
                    return true;
                }
            }

            // Rear Rack, Lift
            if (teachingIndex == 6)
            {
                // 1) Rear 측 사이트(14~26) PCB 도크 센서로 체크
                var rearSites = new FuncInline.enumTeachingPos[]
                {
                    FuncInline.enumTeachingPos.Site14_R_DT1,
                    FuncInline.enumTeachingPos.Site15_R_DT2,
                    FuncInline.enumTeachingPos.Site16_R_DT3,
                    FuncInline.enumTeachingPos.Site17_R_DT4,
                    FuncInline.enumTeachingPos.Site18_R_DT5,
                    FuncInline.enumTeachingPos.Site19_R_DT6,
                    FuncInline.enumTeachingPos.Site20_R_DT7,
                    FuncInline.enumTeachingPos.Site21_R_DT8,
                    FuncInline.enumTeachingPos.Site22_R_DT9,
                    FuncInline.enumTeachingPos.Site23_R_DT10_FT4,
                    FuncInline.enumTeachingPos.Site24_R_FT1,
                    FuncInline.enumTeachingPos.Site25_R_FT2,
                    FuncInline.enumTeachingPos.Site26_R_FT3,
                };

                for (int i = 0; i < rearSites.Length; i++)
                {
                    var site = rearSites[i];
                    if (FuncInline.SiteIoMaps.TryGetPcbDockDI(site, out var di) && DIO.GetDIData(di))
                    {
                        //int siteNo = (int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1; // 1-based
                        string label = FuncInline.SiteDisplay.GetSiteDisplayName(site);

                        FuncWin.TopMessageBox($"PCB detected in Site #{label}. Manual move disabled while PCB exist");
                        return true;
                    }
                }

                // 2) Rear Lift 컨베이어(상/하) PCB 감지
                if (DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("PCB detected in Lift #2 Conveyor. Manual move disabled while PCB exist");
                    return true;
                }
            }

            return false;
        }

    }
}
