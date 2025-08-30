using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;


namespace Radix
{
    public partial class ServoPopUp : Form
    {
        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;

        #region Servo Speed Set 관련
        private double ServoSpeed = 0;
        private enum enumServoSpeedSet
        {
            Slow,
            Middle,
            High
        }
        private static enumServoSpeedSet ServoSpeedSet = enumServoSpeedSet.Slow;
        #endregion

        private static FuncInline.enumServoAxis ServoSelect = FuncInline.enumServoAxis.SV00_In_Shuttle;


        public ServoPopUp()
        {
            InitializeComponent();


        }
        private void debug(string str)
        {
            Util.Debug(str);
        }
        private void ServoPopUp_Shown(object sender, EventArgs e)
        {
            #region 화면 제어용 쓰레드 타이머 시작            
            TimerCallback CallBackUI = new TimerCallback(TimerUI);
            timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);
            #endregion

            #region Robot Speed
            ServoSpeedSet = enumServoSpeedSet.Slow;
            #endregion
            ServoSelect = FuncInline.enumServoAxis.SV00_In_Shuttle;
            cbAxisSelect.Text = ServoSelect.ToString();

            this.BringToFront();
        }
        private void ServoPopUp_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerUI.Dispose();
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

        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            try
            {
                if ((int)GlobalVar.SystemStatus == (int)enumSystemStatus.AutoRun)
                {
                    timerUI.Dispose();
                    this.Close();
                    return;
                }

                if (GlobalVar.GlobalStop)
                {
                    timerUI.Dispose();
                    this.Close();
                    return;
                }

                if (timerDoing)
                {
                    return;
                }

                timerDoing = true;

                /* 화면 변경 timer */
                this.Invoke(new MethodInvoker(delegate ()
                {
                    #region Servo Speed 선택
                    if (ServoSpeedSet == enumServoSpeedSet.High)
                    {
                        FuncForm.SetButtonColor2(btnHighSpeed, true);
                        FuncForm.SetButtonColor2(btnMiddleSpeed, false);
                        FuncForm.SetButtonColor2(btnSlowSpeed, false);
                        //ServoSpeed = GlobalVar.ServoSpeed * (0.5);
                        GlobalVar.ServoManualSpeed = 20 * (1);
                    }
                    else if (ServoSpeedSet == enumServoSpeedSet.Middle)
                    {
                        FuncForm.SetButtonColor2(btnHighSpeed, false);
                        FuncForm.SetButtonColor2(btnMiddleSpeed, true);
                        FuncForm.SetButtonColor2(btnSlowSpeed, false);
                        //ServoSpeed = GlobalVar.ServoSpeed * (0.5);
                        GlobalVar.ServoManualSpeed = 20 * (0.5);
                    }
                    else if (ServoSpeedSet == enumServoSpeedSet.Slow)
                    {
                        FuncForm.SetButtonColor2(btnHighSpeed, false);
                        FuncForm.SetButtonColor2(btnMiddleSpeed, false);
                        FuncForm.SetButtonColor2(btnSlowSpeed, true);
                        //ServoSpeed = GlobalVar.ServoSpeed * (0.2);
                        GlobalVar.ServoManualSpeed = 20 * (0.2);
                    }
                    #endregion

                    #region Servo Current 
                    lbAxis.Text = ServoSelect.ToString();

                    lblServoPos.Text = GlobalVar.AxisStatus[(int)ServoSelect].Position.ToString("F3");//FuncMotion.GetRealPosition((int)ServoSelect).ToString(); //(GlobalVar.AxisStatus[(int)ServoSelect].Position / 1000).ToString();
                    FuncForm.SetServoStateColor(pbAxisStatus, (int)ServoSelect);

                    #region Z 축을 선택 할 경우 Up Down 표시, Jof 표시
                    if (ServoSelect == FuncInline.enumServoAxis.SV02_Lift1 ||
                    ServoSelect == FuncInline.enumServoAxis.SV04_Lift2)
                    {
                        pbJogUpServo.Visible = true;
                        lbJogUp.Visible = true;
                      
                        lbJogUp.Text = "UP(+)";
                        lbJogDown.Text = "DOWN(-)";
                      
                      
                        pbJogDownServo.Visible = true;
                        lbJogDown.Visible = true;

                        pbJogFrontServo.Visible = false;
                        lbJogFront.Visible = false;
                        pbJogBackServo.Visible = false;
                        lbJogBack.Visible = false;
                    }
                    else
                    {
                        pbJogUpServo.Visible = false;
                        lbJogUp.Visible = false;
                        pbJogDownServo.Visible = false;
                        lbJogDown.Visible = false;
                        pbJogFrontServo.Visible = true;
                        lbJogFront.Visible = true;
                        pbJogBackServo.Visible = true;
                        lbJogBack.Visible = true;
                    }
                    #endregion

                    #endregion


                    // Limit 보이기

                    bool IsLimitPlus = GlobalVar.AxisStatus[(int)ServoSelect].LimitSwitchPos;
                    bool IsLimitMinus = GlobalVar.AxisStatus[(int)ServoSelect].LimitSwitchNeg;
                    bool IsHomeOn = GlobalVar.AxisStatus[(int)ServoSelect].HomeAbsSwitch ;
                    lbLimitPlus.BackColor = (IsLimitPlus) ? Color.Lime : Color.WhiteSmoke;
                    lbLimitMinus.BackColor = (IsLimitMinus) ? Color.Lime : Color.WhiteSmoke;
                    lbHome.BackColor = (IsHomeOn) ? Color.Lime : Color.WhiteSmoke;

                }));

                timerDoing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }
        }

        private void cbAxisSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAxisSelect.SelectedIndex >= (int)FuncInline.enumServoAxis.SV00_In_Shuttle && cbAxisSelect.SelectedIndex <= (int)FuncInline.enumServoAxis.SV07_Scan_Y)
            {
                ServoSelect = (FuncInline.enumServoAxis)cbAxisSelect.SelectedIndex;
            }
        }

        #region Click 이벤트
        private void btnSlowSpeed_Click(object sender, EventArgs e)
        {
            ServoSpeedSet = enumServoSpeedSet.Slow;
        }

        private void btnMiddleSpeed_Click(object sender, EventArgs e)
        {
            ServoSpeedSet = enumServoSpeedSet.Middle;
        }

        private void btnHighSpeed_Click(object sender, EventArgs e)
        {
            ServoSpeedSet = enumServoSpeedSet.High;
        }
        private void pbAxisStatus_Click(object sender, EventArgs e)
        {
            FuncMotion.ServoReset((uint)ServoSelect);
            if (!GlobalVar.AxisStatus[(int)ServoSelect].PowerOn)
            {
                FuncMotion.ServoOn((uint)ServoSelect, true);
            }
        }

        bool IsConveyorAxis( int axis )
        {
            if (axis == (int)FuncInline.enumServoAxis.SV00_In_Shuttle)
                return true;
            return false;
        }


        private void pbRobotMove_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.AxisStatus[(int)ServoSelect].StandStill)
            {
                FuncWin.TopMessageBox("Servo Moving!");
                return;
            }

            //FuncBoxPackingMove.MoveServo((int)ServoSelect, (double)numServo.Value, (double)GlobalVar.ServoManualSpeed, false);

            int axis = (int)ServoSelect;

            FuncInlineMove.MoveAbsMM((int)ServoSelect, (double)numServo.Value);
          
        }

        private void pbRobotStop_Click(object sender, EventArgs e)
        {
            FuncMotion.MoveStop((int)ServoSelect);
        }

        private void pbRobotHome_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.AxisStatus[(int)ServoSelect].StandStill)
            {
                FuncWin.TopMessageBox("Servo Moving!");
                return;
            }
            // 인터락 체크
            if (!FuncInlineMove.ServoInterlockCheck((int)ServoSelect, 0))
            {
                FuncWin.TopMessageBox($"MoveAbsMM {(int)ServoSelect} ServoInterlockCheck failed!\n{FuncInline.Interlock_View}");
                return;
            }
            FuncMotion.MoveHome((uint)ServoSelect);
          
        }

      
        #endregion

        #region Jog 속도 및 거리 체크가 필요함.
        #region Up/Down
        private void pbJogUpServo_MouseDown(object sender, MouseEventArgs e)
        {
            //FuncInlineMotion.StopAllJog();

            int axis = (int)ServoSelect;
            double speed = GlobalVar.ServoManualSpeed;

            if (ServoSelect == FuncInline.enumServoAxis.SV02_Lift1 ||
                         ServoSelect == FuncInline.enumServoAxis.SV04_Lift2)
            {
                FuncMotion.MoveRelative((uint)axis,
                 1000,
                 speed);
            }
            else
            {
                FuncMotion.MoveRelative((uint)axis,
                  -1000,
                  speed);
            }
                










            //FuncInlineMotion.StopAllJog();

            //int axis = (int)ServoSelect;

            //RTEX.MoveRelative((uint)axis,
            //      -10000,
            //      GlobalVar.ServoManualSpeed);

            ////if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_Before_X)
            ////{
            ////    FuncBoxPackingMove.JogMoveCheck_StateChange(0);
            ////}
            ////else if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_After_X)
            ////{
            ////    FuncBoxPackingMove.JogMoveCheck_StateChange(1);
            ////}

        }
        private void pbJogUpServo_MouseUp(object sender, MouseEventArgs e)
        {
            //FuncInlineMotion.StopAllJog();
            FuncMotion.JogMoveStopAll();
        }
        private void pbJogDownServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();

            int axis = (int)ServoSelect;
            double speed = GlobalVar.ServoManualSpeed;

            if (ServoSelect == FuncInline.enumServoAxis.SV02_Lift1 ||
                        ServoSelect == FuncInline.enumServoAxis.SV04_Lift2)
            {
                FuncMotion.MoveRelative((uint)axis,
                   -1000,
                   speed);
            }
            else
            {
                FuncMotion.MoveRelative((uint)axis,
                   1000,
                   speed);
            }
        





            //FuncInlineMotion.StopAllJog();

            //int axis = (int)ServoSelect;

            //RTEX.MoveRelative((uint)axis,
            //      10000,
            //      GlobalVar.ServoManualSpeed);


            ////if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_Before_X)
            ////{
            ////    FuncBoxPackingMove.JogMoveCheck_StateChange(0);
            ////}
            ////else if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_After_X)
            ////{
            ////    FuncBoxPackingMove.JogMoveCheck_StateChange(1);
            ////}

        }
        private void pbJogDownServo_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.JogMoveStopAll();
        }
        #endregion
        #region Left/Right
        private void pbJogLeftServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();

            int axis = (int)ServoSelect;

            FuncMotion.MoveRelative((uint)axis,
                  -1000,
                  GlobalVar.ServoManualSpeed);
        }
        private void pbJogLeftServo_MouseUp(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();
        }
        private void pbJogRightServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();

            int axis = (int)ServoSelect;

            FuncMotion.MoveRelative((uint)axis,
                  1000,
                  GlobalVar.ServoManualSpeed);
        }
        private void pbJogRightServo_MouseUp(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();
        }
        #endregion
        #region Front/Back
        private void pbJogFrontServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();

            int axis = (int)ServoSelect;
            double speed = GlobalVar.ServoManualSpeed;  // mm/s



            //


            FuncMotion.MoveRelative((uint)axis,
                  1000,
                  speed);
                 //GlobalVar.ServoManualSpeed);

            //if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_Before_X)
            //{
            //    FuncBoxPackingMove.JogMoveCheck_StateChange(0);
            //}
            //else if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_After_X)
            //{
            //    FuncBoxPackingMove.JogMoveCheck_StateChange(1);
            //}

        }
        private void pbJogFrontServo_MouseUp(object sender, MouseEventArgs e)
        {
            FuncMotion.JogMoveStopAll();
            //RTEX.MoveStop((int)ServoSelect);
        }

        private void pbJogBackServo_MouseDown(object sender, MouseEventArgs e)
        {
            FuncInlineMove.StopAllJog();

            int axis = (int)ServoSelect;
            double speed = GlobalVar.ServoManualSpeed;  // mm/s


            FuncMotion.MoveRelative((uint)axis,
                  -1000,
                  speed );

            //if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_Before_X)
            //{
            //    FuncBoxPackingMove.JogMoveCheck_StateChange(0);
            //}
            //else if ((int)ServoSelect <= (int)enum_BoxPacking_ServoAxis.Sanding_After_X)
            //{
            //    FuncBoxPackingMove.JogMoveCheck_StateChange(1);
            //}

        }
        private void pbJogBackServo_MouseUp(object sender, MouseEventArgs e)
        {

            FuncMotion.JogMoveStopAll();
        }



        #endregion

        #endregion

        private void test111_Click(object sender, EventArgs e)
        {
            double zpos = 911;
            double xpos = 19;

            //Bogan_Move((int)enum_BoxPacking_ServoAxis.Sanding_Before_Z1, zpos,
            //    (int)enum_BoxPacking_ServoAxis.Sanding_Before_X, xpos);
        }
        public void Bogan_Move(int axis_1, double ZPOS, int axis_2, double XPOS)
        {
            uint lSize = 2;
            int[] lAxesNo = { axis_1, axis_2 };
            ZPOS = FuncMotion.GetRealPulse((int)axis_1, ZPOS);
            XPOS = FuncMotion.GetRealPulse((int)axis_2, XPOS);
            double[] dPosition = { ZPOS, XPOS };
            double dMaxVelocity = FuncMotion.GetRealPulse((int)axis_1, GlobalVar.ServoSpeed ); //100;
            double dMaxAccel = dMaxVelocity * 5;//200
            double dMaxDecel = dMaxVelocity * 5;//200
            int lCoordinate = 0;

            uint uAbsRelMode = 0;
            // 지정 축의 이동 거리 계산 모드를 설정한다.
            // uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
            //               POS_REL_MODE '1' - 상대 좌표계

            uint uProfileMode = 3;
            // 지정 축의 구동 속도 프로파일 모드를 설정한다.
            // ProfileMode : SYM_TRAPEZOIDE_MODE    '0' - 대칭 Trapezode
            //               ASYM_TRAPEZOIDE_MODE   '1' - 비대칭 Trapezode
            //               QUASI_S_CURVE_MODE     '2' - 대칭 Quasi-S Curve
            //               SYM_S_CURVE_MODE       '3' - 대칭 S Curve
            //               ASYM_S_CURVE_MODE      '4' - 비대칭 S Curve
            //               SYM_TRAP_M3_SW_MODE    '5' - 대칭 Trapezode : MLIII 내부 S/W Profile
            //               ASYM_TRAP_M3_SW_MODE   '6' - 비대칭 Trapezode : MLIII 내부 S/W Profile
            //               SYM_S_M3_SW_MODE       '7' - 대칭 S Curve : MLIII 내부 S/W Profile
            //               ASYM_S_M3_SW_MODE      '8' - asymmetric S Curve : MLIII 내부 S/W Profile

            // 지정된 좌표계에 연속 보간 구동을 위해 저장된 내부 Queue를 모두 삭제하는 함수이다.
            CAXM.AxmContiWriteClear(lCoordinate);

            // 지정된 좌표계에 연속보간 축 맵핑을 설정한다.
            // (축맵핑 번호는 0 부터 시작))
            // 주의점: 축맵핑할때는 반드시 실제 축번호가 작은 숫자부터 큰숫자를 넣는다.
            //         가상축 맵핑 함수를 사용하였을 때 가상축번호를 실제 축번호가 작은 값 부터 lpAxesNo의 낮은 인텍스에 입력하여야 한다.
            //         가상축 맵핑 함수를 사용하였을 때 가상축번호에 해당하는 실제 축번호가 다른 값이라야 한다.
            //         같은 축을 다른 Coordinate에 중복 맵핑하지 말아야 한다.
            CAXM.AxmContiSetAxisMap(lCoordinate, lSize, lAxesNo);

            // 지정된 좌표계에 연속보간 축 절대/상대 모드를 설정한다.
            // (주의점 : 반드시 축맵핑 하고 사용가능)
            // 지정 축의 이동 거리 계산 모드를 설정한다.
            // uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
            //               POS_REL_MODE '1' - 상대 좌표계
            CAXM.AxmContiSetAbsRelMode(lCoordinate, uAbsRelMode);// 상대위치구동으로설정

            // 시작점과 종료점을 지정하여 다축 직선 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
            // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점과 종료점을 지정하여 직선 보간 구동하는 Queue에 저장함수가된다. 
            // 직선 프로파일 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
            uint duRetCode;
            duRetCode = CAXM.AxmLineMove(lCoordinate, dPosition, dMaxVelocity, dMaxAccel, dMaxDecel);
            if (duRetCode != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                MessageBox.Show(String.Format("AxmLineMove return error[Code:{0:d}]", duRetCode));
        }

        private void pbJogDownServo_Click(object sender, EventArgs e)
        {

        }
    }
}
