using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace Radix.Popup.Machine
{
    public partial class GeneralTeach : UserControl
    {
        #region 로컬변수
        private bool timerDoing = false;

        #endregion
        public GeneralTeach()
        {
            InitializeComponent();
        }
        private void debug(string str)
        {
            Util.Debug(str);
        }
        #region 설정 관련
        public void LoadAllValue()
        {
            try
            {
                #region general
                tbAppName.Text = GlobalVar.AppName;
                cbUseDoor.Checked = GlobalVar.UseDoor;
                numNormalTimeout.Value = (decimal)GlobalVar.NormalTimeout;
                #endregion

                #region USE Check
              
                cb_Robot_Use.Checked = GlobalVar.RobotUse;
                #endregion

                #region #0 샌딩 전 리프트
                numLiftSpeed.Value = (decimal)FuncInline.ServoParamAll.sv0_BeforeLift.speed;
                numLift_Step.Value = (decimal)FuncInline.ServoParamAll.sv0_BeforeLift.step;
                numBeforeLift_1stPos.Value = (decimal)FuncInline.ServoParamAll.sv0_BeforeLift.init_1Stpos;
                numBeforeLift_InputPos.Value = (decimal)FuncInline.ServoParamAll.sv0_BeforeLift.Input_pos;

                #endregion


                #region 샌딩 트레이
                numTrayRowPitch.Value = (decimal)FuncInline.BeforeTrayRow_Pitch;
                numTrayColumnPitch.Value = (decimal)FuncInline.BeforeTrayColum_Pitch;
                numTrayRowPitch.Value = (decimal)FuncInline.AfterTrayRow_Pitch;
                numTrayColumnPitch.Value = (decimal)FuncInline.AfterTrayColum_Pitch;
                #endregion

                #region 샌딩 후 리프트
                numLiftSpeed.Value = (decimal)FuncInline.ServoParamAll.sv1_AfterLift.speed;
                numLift_Step.Value = (decimal)FuncInline.ServoParamAll.sv1_AfterLift.step;
                numAfterLift_1stPos.Value = (decimal)FuncInline.ServoParamAll.sv1_AfterLift.init_1Stpos;
                numAfterLift_OutPos.Value = (decimal)FuncInline.ServoParamAll.sv1_AfterLift.Output_pos;
                #endregion

                //로봇 스피드
                numRobotSpeed.Value = (decimal)FuncInline.RobotSpeed;

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
                #region general
                GlobalVar.AppName = tbAppName.Text;
                GlobalVar.UseDoor = cbUseDoor.Checked;
                GlobalVar.NormalTimeout = (ulong)numNormalTimeout.Value;
                #endregion

                #region USE Check
                GlobalVar.RobotUse = cb_Robot_Use.Checked;
                #endregion

                #region 샌딩 전 리프트
                FuncInline.ServoParamAll.sv0_BeforeLift.speed = (double)numLiftSpeed.Value;
                FuncInline.ServoParamAll.sv0_BeforeLift.step = (double)numLift_Step.Value;
                FuncInline.ServoParamAll.sv0_BeforeLift.init_1Stpos = (double)numBeforeLift_1stPos.Value;
                FuncInline.ServoParamAll.sv0_BeforeLift.Input_pos = (double)numBeforeLift_InputPos.Value;
                #endregion

                #region 샌딩 트레이
                FuncInline.BeforeTrayRow_Pitch = (double)numTrayRowPitch.Value;
                FuncInline.BeforeTrayColum_Pitch = (double)numTrayColumnPitch.Value;
                FuncInline.AfterTrayRow_Pitch = (double)numTrayRowPitch.Value;
                FuncInline.AfterTrayColum_Pitch = (double)numTrayColumnPitch.Value;
            
                #endregion

                #region 샌딩 후 리프트
                FuncInline.ServoParamAll.sv1_AfterLift.speed = (double)numLiftSpeed.Value;
                FuncInline.ServoParamAll.sv1_AfterLift.step = (double)numLift_Step.Value;
                FuncInline.ServoParamAll.sv1_AfterLift.init_1Stpos = (double)numAfterLift_1stPos.Value;
                FuncInline.ServoParamAll.sv1_AfterLift.Output_pos = (double)numAfterLift_OutPos.Value;
                
                #endregion


                //로봇 스피드
                FuncInline.RobotSpeed = (double)numRobotSpeed.Value;




            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }








        #endregion

        //서보 조그 실행
        private void btnServoPopUp_Click(object sender, EventArgs e)
        {
            ServoPopUp Servo_Move_dlg = new ServoPopUp();
            Servo_Move_dlg.Show();
        }

        private void GeneralTeach_Load(object sender, EventArgs e)
        {
          
        }
      

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            #region 위치값 표시
            //lbFixtureX_Pos.Text = GlobalVar.AxisStatus[(int)FuncAmplePacking.enumServoAxis.SV02_Fixture_Supply_X].Position.ToString("F3");
            //lbFixtureY_Pos.Text = GlobalVar.AxisStatus[(int)FuncAmplePacking.enumServoAxis.SV03_Fixture_Supply_Y].Position.ToString("F3");
            //lbFixtureZ_Pos.Text = GlobalVar.AxisStatus[(int)FuncAmplePacking.enumServoAxis.SV04_Fixture_Supply_Z].Position.ToString("F3");
            //lbOutputX_Pos.Text = GlobalVar.AxisStatus[(int)FuncAmplePacking.enumServoAxis.SV08_Output_Tray_X].Position.ToString("F3"); ;
            //lbOutputZ_Pos.Text = GlobalVar.AxisStatus[(int)FuncAmplePacking.enumServoAxis.SV09_Output_Tray_Z].Position.ToString("F3"); ;
            lbBeforeLift_Pos.Text = GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV00_In_Shuttle].Position.ToString("F3");
            lbAfterLift_Pos.Text = GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV01_Out_Shuttle].Position.ToString("F3");

            if (FuncInline.HiddenCount >= 5)
            {
                cbUseDoor.Visible = true;
                cb_Robot_Use.Visible = true;
                lbRobotUse.Visible = true;
            }
            else
            {
                cbUseDoor.Visible = false;
                cb_Robot_Use.Visible = false;
                lbRobotUse.Visible = false;

            }
          

            #endregion
        }
    }
}
