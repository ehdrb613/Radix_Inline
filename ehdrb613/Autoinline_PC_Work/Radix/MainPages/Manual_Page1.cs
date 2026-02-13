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

namespace Radix
{
    /*
     * Manual.cs : 각 파트 및 장비의 수동 운전
     */

    public partial class Manual_Page1 : Form
    {
        enum enumManualAction
        {
            None,
            SiteOpen,
            SiteClose,
            RobotMove
        }

        private System.Threading.Timer timerCheck;
        //private System.Threading.Timer timerUI; // timerCheck랑 통합
        private System.Threading.Timer timerMotor; // 모터 상태모니터용
        private System.Threading.Timer timerJog; // 모터 조그 제어용
        private System.Threading.Timer timerManual; // 버튼 중 긴 동작이 필요한 것 수행. 현재 동작이 있으면 수행 거부할 것

        private bool timerDoing = false;

        //쿠카 수동동작
        private bool kuka_Manual_Move = false;

        public Manual_Page1()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            Util.Debug("Manual : " + str);
        }


        #region 초기화 관련

        private void Manual_Page1_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;

            //cmbLift.SelectedIndex = 0;
            //cmbPush.SelectedIndex = 0;
            //cmbRobotPosition.SelectedIndex = 0;

            //cmbRobotAction.Items.Clear();
            //for (int i = 0; i < Enum.GetValues(typeof(enumMoveAction)).Length; i++)
            //{
            //    cmbRobotAction.Items.Add(((enumMoveAction)i).ToString());
            //    cmbRobotAction.SelectedIndex = 0;
            //}

            // 타이머 시작
            TimerCallback CallBackCheck = new TimerCallback(TimerCheck);
            timerCheck = new System.Threading.Timer(CallBackCheck, false, 0, 100);

            //TimerCallback CallBackMotor = new TimerCallback(TimerMotor);
            //timerMotor = new System.Threading.Timer(CallBackMotor, false, 0, 100);

            //TimerCallback CallBackJog = new TimerCallback(TimerJog);
            //timerJog = new System.Threading.Timer(CallBackJog, false, 0, 100);

            //TimerCallback CallBackManual = new TimerCallback(TimerManual);
            //timerManual = new System.Threading.Timer(CallBackManual, false, 0, 100);

            this.BringToFront();

        }

        private void Manual_Page1_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerCheck.Dispose();
            GlobalVar.dlgOpened = false;
            if (this.Parent != null)
            {
                try
                {
                    timerCheck.Dispose();
                    timerMotor.Dispose();
                    timerJog.Dispose();
                    timerManual.Dispose();
                    this.Parent.BringToFront();
                }
                catch
                { }
            }
        }

        #endregion

        #region 타이머 함수
        private void TimerCheck(Object state)
        {
            if (GlobalVar.TabMain != enumTabMain.Manual) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
            {
                timerDoing = false;
                return;
            }
            try
            {
                //timerDoing = false;
                if (timerDoing)
                {
                    return;
                }

                timerDoing = true;

                #region 창모드일때
                /* 창모드가 아니라 닫거나 리턴할 수 없다..
                if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중 자동 닫기
                {
                    this.BringToFront();
                    Util.TopMessageBox("Can't use manual move while system is running.");
                    //this.Close();
                    timerDoing = false;
                    return;
                }
                //*/

                /* 창모드가 아니라 닫거나 리턴할 수 없다..
                if (GlobalVar.UseDoor)
                {
                    if (!GlobalVar.Simulation &&
                    (DIO.GetDIData(enumDINames.X00_2_Front_Door1) ||
                    DIO.GetDIData(enumDINames.X00_3_Front_Door2) ||
                    DIO.GetDIData(enumDINames.X00_4_Front_Door3)))
                    {
                        this.BringToFront();
                        Util.TopMessageBox("Can't use manual move while doors are opened");
                        //this.Close();
                        timerDoing = false;
                        return;
                    }
                }
                //*/
                #endregion


                this.InvokeAndClose((MethodInvoker)delegate
                {
                    ulong startTime = GlobalVar.TickCount64;




                    #region #1 샌딩 전 리프트
                    //////서보
                    //int SV00_Input_Tray_Lift = (int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift;
                    //if (GlobalVar.AxisStatus[SV00_Input_Tray_Lift].Errored ||
                    //     GlobalVar.AxisStatus[SV00_Input_Tray_Lift].Disabled ||
                    //     GlobalVar.AxisStatus[SV00_Input_Tray_Lift].LimitSwitchNeg ||
                    //     GlobalVar.AxisStatus[SV00_Input_Tray_Lift].LimitSwitchPos)
                    //{
                    //    btnBefore_Lift_ServoOn.BackColor = Color.Tomato;
                    //}
                    //else if (GlobalVar.AxisStatus[SV00_Input_Tray_Lift].PowerOn)
                    //{
                    //    btnBefore_Lift_ServoOn.BackColor = Color.LightCyan;
                    //}
                    //else
                    //{
                    //    btnBefore_Lift_ServoOn.BackColor = Color.Gray;
                    //}

                    //btnBefore_Lift_Home.BackColor = GlobalVar.AxisStatus[(int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift].isHomed ? Color.LightCyan : Color.Gray;
                    //btnBefore_Lift_Home.Enabled = !GlobalVar.AxisStatus[(int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift].Homing && 
                    //                                GlobalVar.AxisStatus[(int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift].StandStill; //호밍 중이 아닐때만 활성화
                    //btnBefore_Lift_InputPos.Enabled = GlobalVar.AxisStatus[(int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift].StandStill; //정지 일때만 활성화
                    //btnBefore_Lift_StepUP.Enabled = GlobalVar.AxisStatus[(int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift].StandStill;
                    //btnBefore_Lift_StepDown.Enabled = GlobalVar.AxisStatus[(int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift].StandStill;
                    //btnBefore_Lift_1st_Position.Enabled = GlobalVar.AxisStatus[(int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift].StandStill;

                    //lbBeforeLiftPos.Text = $"{((SNUC_SandingClass)GlobalVar.ProjectClass).beforeLift01.currentFloor + 1}층";

                    ////#1 샌딩 전 리프트
                    //btnBefore_Tray_Lift_Conveyor.BackColor = DIO.GetDORead(FuncAutoInline.enumDONames.Y01_0_Before_Tray_Lift_Conveyor_CW) ? Color.Lime : Color.White;
                    //cbBefore_Tray_Input_Lift_End_Sensor.Checked = DIO.GetDIData(FuncAutoInline.enumDINames.X01_0_Before_Tray_Input_Lift_End_Sensor);



                    #endregion





                });

                timerDoing = false;

            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
            }
            timerDoing = false;
        }

        #endregion



        private void btnServoStopAll_Click(object sender, EventArgs e)
        {
            FuncMotion.MoveStopAll();
        }






        private void btnBefore_Tray_WorkArea_Stopper_Click(object sender, EventArgs e)
        {
            //FuncLog.WriteLog($"[Manual1]#1샌딩 전 리프트 - 컨베이어 동작 클릭 - {DIO.GetDORead(FuncAutoInline.enumDONames.Y01_2_Before_Tray_WorkArea_Stopper_Up)}");
            //DIO.WriteDOData(FuncAutoInline.enumDONames.Y01_2_Before_Tray_WorkArea_Stopper_Up, !DIO.GetDORead(FuncAutoInline.enumDONames.Y01_2_Before_Tray_WorkArea_Stopper_Up));
        }


        
        private void btnBefore_Lift_ServoOn_Click(object sender, EventArgs e)
        {
            //int SV00_BeforeLift = (int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift;
            //FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 서보On 클릭");

            //FuncMotion.ServoReset((uint)SV00_BeforeLift);
            //if (!GlobalVar.AxisStatus[SV00_BeforeLift].PowerOn)
            //{
            //    FuncMotion.ServoOn((uint)SV00_BeforeLift, true);
            //}
        }
        private void btnBefore_Lift_Home_Click(object sender, EventArgs e)
        {
            //int SV00_BeforeLift = (int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift;
            //if (GlobalVar.AxisStatus[SV00_BeforeLift].isHomed)
            //{
            //    FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 원점복귀 클릭 - isHOME");
            //    return;
            //}
            //// 인터락 체크
            //if (!FuncSandingMove.ServoInterlockCheck(SV00_BeforeLift, 0))
            //{
            //    FuncLog.WriteLog($"MoveAbsMM {SV00_BeforeLift} ServoInterlockCheck failed!\n{FuncAutoInline.Interlock_View}");
            //    return;
            //}

            //FuncMotion.MoveHome((uint)SV00_BeforeLift);
        }



        private void btnBefore_Lift_StepUP_Click(object sender, EventArgs e)
        {
            //FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 이전층으로 상승 시도");

            //int SV00_Input_Tray_Lift = (int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift;
            //if (RTEX.IsMoving(SV00_Input_Tray_Lift)) return;

            //if (GlobalVar.AxisStatus[SV00_Input_Tray_Lift].isHomed)
            //{
            //    int currentFloor = ((SNUC_SandingClass)GlobalVar.ProjectClass).beforeLift01.currentFloor;

            //    // 이전 층으로 상승
            //    if (currentFloor > 0 && currentFloor < FuncAutoInline.BeforeLiftPos.Length)
            //    {
            //        int prevFloor = currentFloor - 1;
            //        double prevPos = FuncAutoInline.BeforeLiftPos[prevFloor];

            //        FuncLog.WriteLog($"[Manual1]#1샌딩 전 리프트 : {prevFloor + 1}층으로 상승 지령");
            //        FuncSandingMove.MoveAbsMM(SV00_Input_Tray_Lift, prevPos);
            //    }
            //    else if (currentFloor == -1)
            //    {
            //        FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 현재 위치가 유효한 층이 아님");
            //    }
            //    else
            //    {
            //        FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 최하층이므로 더 이상 상승할 수 없음");
            //    }
            //}
            //else
            //{
            //    FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 원점복귀 필요");
            //}

        }

        private void btnBefore_Lift_StepDown_Click(object sender, EventArgs e)
        {
            //FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 다음층으로 하강 시도");

            //int SV00_Input_Tray_Lift = (int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift;
            //if (RTEX.IsMoving(SV00_Input_Tray_Lift)) return;

            //if (GlobalVar.AxisStatus[SV00_Input_Tray_Lift].isHomed)
            //{
            //    int currentFloor = ((SNUC_SandingClass)GlobalVar.ProjectClass).beforeLift01.currentFloor;

            //    // 다음 층으로 상승
            //    if (currentFloor >= 0 && currentFloor < FuncAutoInline.BeforeLiftPos.Length - 1)
            //    {
            //        int nextFloor = currentFloor + 1;
            //        double nextPos = FuncAutoInline.BeforeLiftPos[nextFloor];

            //        FuncLog.WriteLog($"[Manual1]#1샌딩 전 리프트 : {nextFloor + 1}층으로 하강 지령");
            //        FuncSandingMove.MoveAbsMM(SV00_Input_Tray_Lift, nextPos);
            //    }
            //    else if (currentFloor == -1)
            //    {
            //        FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 현재 위치가 유효한 층이 아님");
            //    }
            //    else
            //    {
            //        FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 최상층이므로 더 이상 하강할 수 없음");
            //    }
            //}
            //else
            //{
            //    FuncLog.WriteLog("[Manual1]#1샌딩 전 리프트 : 원점복귀 필요");
            //}

        }





    }
    

}