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
        
        FuncInline.enumPMCAxis activeStepAxis = FuncInline.enumPMCAxis.ST00_InShuttle_Width;

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
                            case FuncInline.enumTeachingPos.InConveyor:
                                FuncForm.SetButtonColor3(btnStopperUp,
                                                        FuncInline.enumDINames.X03_3_In_Conveyor_Stopper_Up_Sensor,
                                                        FuncInline.enumDINames.X03_4_In_Conveyor_Stopper_Down_Sensor);
                                FuncForm.SetButtonColor3(btnStopperDown,
                                                        FuncInline.enumDINames.X03_4_In_Conveyor_Stopper_Down_Sensor,
                                                        FuncInline.enumDINames.X03_3_In_Conveyor_Stopper_Up_Sensor);
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw) ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.FrontPassLine:
                                FuncForm.SetButtonColor2(btnStopperUp, DIO.GetDORead(FuncInline.enumDONames.Y03_3_Rack1_PassLine_Stopper_Forward));
                                FuncForm.SetButtonColor2(btnStopperDown, !DIO.GetDORead(FuncInline.enumDONames.Y03_3_Rack1_PassLine_Stopper_Forward));
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw) ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.Lift1_Up:
                                FuncForm.SetButtonColor3(btnStopperUp,
                                                        FuncInline.enumDINames.X04_3_Rack1_UpLift_Stopper_Up_Sensor,
                                                        FuncInline.enumDINames.X04_4_Rack1_UpLift_Stopper_Down_Sensor);
                                FuncForm.SetButtonColor3(btnStopperDown,
                                                        FuncInline.enumDINames.X04_4_Rack1_UpLift_Stopper_Down_Sensor,
                                                        FuncInline.enumDINames.X04_3_Rack1_UpLift_Stopper_Up_Sensor);
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw);
                                FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw) && !DIO.GetDORead(FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw)
                                                            ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.Lift1_Down:
                                FuncForm.SetButtonColor3(btnStopperUp,
                                                        FuncInline.enumDINames.X05_3_Rack1_DownLift_Stopper_Up_Sensor,
                                                        FuncInline.enumDINames.X05_4_Rack1_DownLift_Stopper_Down_Sensor);
                                FuncForm.SetButtonColor3(btnStopperDown,
                                                        FuncInline.enumDINames.X05_4_Rack1_DownLift_Stopper_Down_Sensor,
                                                        FuncInline.enumDINames.X05_3_Rack1_DownLift_Stopper_Up_Sensor);
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw);
                                FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw) && !DIO.GetDORead(FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw)
                                                            ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.RearPassLine:
                                FuncForm.SetButtonColor2(btnStopperUp, DIO.GetDORead(FuncInline.enumDONames.Y03_5_Rack2_PassLine_Stopper_Forward));
                                FuncForm.SetButtonColor2(btnStopperDown, !DIO.GetDORead(FuncInline.enumDONames.Y03_5_Rack2_PassLine_Stopper_Forward));
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y03_6_Rack2_PassLine_Conveyor_Run);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y03_6_Rack2_PassLine_Conveyor_Run) ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.Lift2_Up:
                                FuncForm.SetButtonColor3(btnStopperUp,
                                                        FuncInline.enumDINames.X06_3_Rack2_UpLift_Stopper_Up_Sensor,
                                                        FuncInline.enumDINames.X06_4_Rack2_UpLift_Stopper_Down_Sensor);
                                FuncForm.SetButtonColor3(btnStopperDown,
                                                        FuncInline.enumDINames.X06_4_Rack2_UpLift_Stopper_Down_Sensor,
                                                        FuncInline.enumDINames.X06_3_Rack2_UpLift_Stopper_Up_Sensor);
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw);
                                FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw) && !DIO.GetDORead(FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw)
                                                            ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.Lift2_Down:
                                FuncForm.SetButtonColor3(btnStopperUp,
                                                        FuncInline.enumDINames.X07_3_Rack2_DownLift_Stopper_Up_Sensor,
                                                        FuncInline.enumDINames.X07_4_Rack2_DownLift_Stopper_Down_Sensor);
                                FuncForm.SetButtonColor3(btnStopperDown,
                                                        FuncInline.enumDINames.X07_4_Rack2_DownLift_Stopper_Down_Sensor,
                                                        FuncInline.enumDINames.X07_3_Rack2_DownLift_Stopper_Up_Sensor);
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw);
                                FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw);
                                btnStopConveyor.BackColor = !DIO.GetDORead(FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw) && !DIO.GetDORead(FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw)
                                                            ? Color.Lime : Color.White;
                                break;
                            case FuncInline.enumTeachingPos.OutConveyor:
                            
                                FuncForm.SetButtonDoColor(btnRunConveyorCW, FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw);
                                //FuncForm.SetButtonDoColor(btnRunConveyorCCW, FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw);
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

                        if (teachingIndex < 5) // 스텝모터
                        {
                            pbAxisStatus.Visible = false;
                            lblCurrPos.Text = FuncInline.PMCStatus[(int)activeStepAxis].Position.ToString("F2");
                        }
                        else // 서보 모터
                        {
                            pbAxisStatus.Visible = true;
                            FuncForm.SetServoStateColor(pbAxisStatus, activeServoAxis);
                            lblCurrPos.Text = FuncMotion.GetRealPosition((int)activeServoAxis).ToString("F2");
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
                for (int i = 0; i < FuncInline.PMCStatus.Length; i += 2)
                {
                    bool flag = false;

                    int motion = (int)manualPos / 2;
                    if (manualPos != FuncInline.enumTeachingPos.NgBuffer &&
                        i == motion * 2)
                    {
                        flag = true;
                    }

                    FuncInline.PMCStatus[i].ReadFlag = flag;
                    if (i + 1 < FuncInline.PMCStatus.Length)
                    {
                        FuncInline.PMCStatus[i + 1].ReadFlag = flag;
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
                            (teachingIndex > 2 && GlobalVar.AxisStatus[(int)activeServoAxis].Errored) ||
                            (teachingIndex <= 2 && FuncInline.PMCStatus[(int)activeStepAxis].Errored))
                        {
                            if (teachingIndex > 2)
                            {
                                if (!GlobalVar.Axis_Sync)
                                {
                                    FuncMotion.MoveStop((int)activeSubAxis);
                                }
                                FuncMotion.MoveStop((int)activeServoAxis);
                            }
                            else
                            {
                                //PMCClass.Stop(activeStepAxis);
                            }
                            jogWidth = false;
                        }
                        else if (teachingIndex > 2 && Math.Abs(GlobalVar.AxisStatus[(int)activeServoAxis].Velocity) > 0.1)
                        {
                            if (!GlobalVar.Axis_Sync)
                            {
                                FuncMotion.MoveStop((int)activeSubAxis);
                            }
                            FuncMotion.MoveStop((int)activeServoAxis);
                        }
                        else if (teachingIndex <= 2 && Math.Abs(FuncInline.PMCStatus[(int)activeStepAxis].Velocity) > 0.1)
                        {
                            //PMCClass.Stop(activeStepAxis);
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

        private void btnStopperUpConveyor_Click(object sender, EventArgs e)
        {
           FuncInline.enumDONames o = FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL;
            switch (manualPos)
            {
                case FuncInline.enumTeachingPos.FrontPassLine:
                    o = FuncInline.enumDONames.Y03_3_Rack1_PassLine_Stopper_Forward;
                    break;
                case FuncInline.enumTeachingPos.Lift1_Up:
                    o = FuncInline.enumDONames.Y04_2_Rack1_UpLift_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.Lift1_Down:
                    o = FuncInline.enumDONames.Y04_6_Rack1_DownLift_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.RearPassLine:
                    o = FuncInline.enumDONames.Y03_5_Rack2_PassLine_Stopper_Forward;
                    break;
                case FuncInline.enumTeachingPos.Lift2_Up:
                    o = FuncInline.enumDONames.Y05_2_Rack2_UpLift_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.Lift2_Down:
                    o = FuncInline.enumDONames.Y05_6_Rack2_DownLift_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.OutConveyor:
                    o = FuncInline.enumDONames.Y06_1_Out_Conveyor_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.NgBuffer: // NG
                    //debug("NG conveyor no stopper");
                    return;
            }
            DIO.WriteDOData(o, true);
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
                    o = FuncInline.enumDONames.Y03_6_Rack2_PassLine_Conveyor_Run;
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
                    (!DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                            !DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                            !DIO.GetDIData(FuncInline.enumDINames.X00_2_Door_Open_Rear_Left) ||
                            !DIO.GetDIData(FuncInline.enumDINames.X02_0_Door_Open_Rear_Right)))
            {
                FuncWin.TopMessageBox("Can not move while doors are opened.");
                return;
            }
            #endregion

        


            if (teachingIndex <= 2) // 스텝모터
            {
                PMCMotion.ABSMove(activeStepAxis, (double)numMovePos.Value + FuncInline.OffsetWidth[teachingIndex], GlobalVar.WidthSpeed);
            }
            else // 서보모터
            {
                FuncMotion.MoveAbsolute((uint)activeServoAxis, (double)numMovePos.Value, GlobalVar.WidthSpeed);
            }
        }

        private void pbHomeConveyor_Click(object sender, EventArgs e)
        {
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

  



            if (teachingIndex <= 2) // 스텝모터
            {
                //PMCClass.HomeRun(activeStepAxis, GlobalVar.WidthHomSpeed);
            }
            else // 서보모터
            {
                FuncInlineMove.MoveHome((uint)activeServoAxis);
            }

        }

        private void pbStopConveyor_Click(object sender, EventArgs e)
        {
            if (teachingIndex <= 2) // 스텝모터
            {
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

            FuncInlineMove.StopAllJog(true);

          

            if (btnPitch.BackColor == Color.Lime)
            {
                if (teachingIndex <= 2) // 스텝 모터
                {
                    //PMCClass.ABSMove(activeStepAxis, 
                                    //FuncInline.PMCStatus[(int)activeStepAxis].Position - (double)numPitch.Value, 
                                    //(double)numSpeed.Value);
                }
                else // 서보모터
                {
                    FuncMotion.MoveAbsolute((uint)activeServoAxis, 
                                    GlobalVar.AxisStatus[(int)activeServoAxis].Position - (double)numPitch.Value, 
                                    (double)numSpeed.Value);
                }
            }
            else
            {
                jogWidth = true;
                jogWidthDown = true;

                if (teachingIndex <= 2)
                {
                    //PMCClass.ContMove(activeStepAxis, 0 - (double)numSpeed.Value);
                }
                else
                {
                    FuncMotion.MoveAbsolute((uint)activeServoAxis, 0, (double)numSpeed.Value);
                }
                ////PMCClass.ABSMove(axis, 70, (int)numWidthSpeedConveyor.Value);
            }
        }

        private void pbJogNarrowConveyor_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                if (teachingIndex <= 2)
                {
                    //PMCClass.Stop(activeStepAxis);
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


            FuncInlineMove.StopAllJog(true);

           


            if (btnPitch.BackColor == Color.Lime)
            {
                if (teachingIndex <= 2) // 스텝 모터
                {
                    //PMCClass.ABSMove(activeStepAxis, FuncInline.PMCStatus[(int)activeStepAxis].Position + (double)numPitch.Value, (double)numSpeed.Value);
                }
                else // 서보모터
                {
                    FuncMotion.MoveAbsolute((uint)activeServoAxis, 
                                        GlobalVar.AxisStatus[(int)activeServoAxis].Position + (double)numPitch.Value, 
                                        (double)numSpeed.Value);
                }
            }
            else
            {
                jogWidth = true;
                jogWidthDown = true;

                if (teachingIndex <= 2)
                {
                    //PMCClass.ContMove(activeStepAxis, (double)numSpeed.Value);
                }
                else
                {
                    FuncMotion.MoveAbsolute((uint)activeServoAxis, 9999, (double)numSpeed.Value);
                }
                ////PMCClass.ABSMove(axis, 70, (int)numWidthSpeedConveyor.Value);
            }
        }

        private void pbJogWideConveyor_MouseUp(object sender, MouseEventArgs e)
        {
            if (btnSpeed.BackColor == Color.Lime)
            {
                if (teachingIndex <= 2)
                {
                    //PMCClass.Stop(activeStepAxis);
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
           FuncInline.enumDONames o = FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL;
            switch (manualPos)
            {
                case FuncInline.enumTeachingPos.FrontPassLine:
                    o = FuncInline.enumDONames.Y03_3_Rack1_PassLine_Stopper_Forward;
                    break;
                case FuncInline.enumTeachingPos.Lift1_Up:
                    o = FuncInline.enumDONames.Y04_2_Rack1_UpLift_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.Lift1_Down:
                    o = FuncInline.enumDONames.Y04_6_Rack1_DownLift_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.RearPassLine:
                    o = FuncInline.enumDONames.Y03_5_Rack2_PassLine_Stopper_Forward;
                    break;
                case FuncInline.enumTeachingPos.Lift2_Up:
                    o = FuncInline.enumDONames.Y05_2_Rack2_UpLift_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.Lift2_Down:
                    o = FuncInline.enumDONames.Y05_6_Rack2_DownLift_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.OutConveyor:
                    o = FuncInline.enumDONames.Y06_1_Out_Conveyor_Stopper_Up;
                    break;
                case FuncInline.enumTeachingPos.NgBuffer: // NG
                    //debug("NG conveyor no stopper");
                    return;
            }
            DIO.WriteDOData(o, false);
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
                    o = FuncInline.enumDONames.Y03_6_Rack2_PassLine_Conveyor_Run;
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

        private void btnPosInputConveyor_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.InConveyor;

            btnPosInConveyor.BackColor = Color.Lime;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.White;

            btnStopperUp.Visible = true;
            btnStopperDown.Visible = true;
            btnRunConveyorCCW.Visible = false;
            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = 0;
            activeStepAxis = FuncInline.enumPMCAxis.ST00_InShuttle_Width;

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosPassLine1_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.FrontPassLine;

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.Lime;
            btnPosLift1Up.BackColor = Color.White;
            btnPosLift1Down.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosLift2Down.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.White;

            btnStopperUp.Visible = true;
            btnStopperDown.Visible = true;
            btnRunConveyorCCW.Visible = false;
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

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.Lime;
            btnPosLift1Down.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosLift2Down.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.White;

            btnStopperUp.Visible = true;
            btnStopperDown.Visible = true;
            btnRunConveyorCCW.Visible = true;
            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = 3;
            activeServoAxis = FuncInline.enumServoAxis.SV03_Rack1_Width;


            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosLift1Down_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.Lift1_Down;

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.White;
            btnPosLift1Down.BackColor = Color.Lime;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosLift2Down.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.White;

            btnStopperUp.Visible = true;
            btnStopperDown.Visible = true;
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

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.White;
            btnPosLift1Down.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.Lime;
            btnPosLift2Up.BackColor = Color.White;
            btnPosLift2Down.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.White;

            btnStopperUp.Visible = true;
            btnStopperDown.Visible = true;
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

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.White;
            btnPosLift1Down.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.Lime;
            btnPosLift2Down.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.White;

            btnStopperUp.Visible = true;
            btnStopperDown.Visible = true;
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
            btnPosLift1Down.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosLift2Down.BackColor = Color.Lime;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.White;

            btnStopperUp.Visible = true;
            btnStopperDown.Visible = true;
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

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.White;
            btnPosLift1Down.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosLift2Down.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.Lime;
            btnPosNG.BackColor = Color.White;

            btnStopperUp.Visible = true;
            btnStopperDown.Visible = true;
            btnRunConveyorCCW.Visible = false;
            btnNGForward.Visible = false;
            btnNGReward.Visible = false;
            btnNGStop.Visible = false;
            teachingIndex = 1;
            activeStepAxis = FuncInline.enumPMCAxis.ST02_OutConveyor_Width;
            activeServoAxis = FuncInline.enumServoAxis.SV05_Rack2_Width;
          

            lblTeachingWidth.Text = FuncInline.TeachingWidth[teachingIndex].ToString("F2");
        }

        private void btnPosNG_Click(object sender, EventArgs e)
        {
            manualPos = FuncInline.enumTeachingPos.NgBuffer;

            btnPosInConveyor.BackColor = Color.White;
            btnPosFrontPassLine.BackColor = Color.White;
            btnPosLift1Up.BackColor = Color.White;
            btnPosLift1Down.BackColor = Color.White;
            btnPosRearPassLine.BackColor = Color.White;
            btnPosLift2Up.BackColor = Color.White;
            btnPosLift2Down.BackColor = Color.White;
            btnPosOutConveyor.BackColor = Color.White;
            btnPosNG.BackColor = Color.Lime;

            btnStopperUp.Visible = false;
            btnStopperDown.Visible = false;
            btnRunConveyorCCW.Visible = false;
            btnNGForward.Visible = true;
            btnNGReward.Visible = true;
            btnNGStop.Visible = true;
            teachingIndex = 2;
            activeStepAxis = FuncInline.enumPMCAxis.ST04_NGBuffer;

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
    }
}
