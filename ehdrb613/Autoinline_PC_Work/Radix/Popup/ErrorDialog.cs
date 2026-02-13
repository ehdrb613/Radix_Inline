using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Radix
{
    /*
     * ErrorDialog.cs : 현재 발생된 에러의 상세 내역을 표시 및 조치 실행
     */

    public partial class ErrorDialog : Form
    {
        private FuncInline.structError error;
        private int startTime = Environment.TickCount;

        public ErrorDialog(FuncInline.structError er)
        {
            InitializeComponent();
            error = er;
        }

        private void ErrorDialog_Shown(object sender, EventArgs e)
        {
            lblErrorCode.Text = ((int)error.ErrorCode).ToString();
            lblErrorName.Text = error.ErrorCode.ToString();
            lblErrorPart.Text = error.ErrorPart.ToString();
            lblErrorDecription.Text = error.Description; //Util.ReadIniFile("error_desc", error.ToString(), GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\ErrorDesc.ini").Replace("\\", "\n");

            #region 에러코드에 따라서 버튼 표시 및 기본 처리
            switch (error.ErrorCode)
            {
                case FuncInline.enumErrorCode.E_Stop:
                    GlobalVar.E_Stop = true;
                    GlobalVar.SystemStatus = enumSystemStatus.EmgStop;
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = true;
                    pbClose.Visible = true;
                    break;
                case FuncInline.enumErrorCode.System_Not_Inited:
                    GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                   
                    /*
                    if (GlobalVar.SystemStatus == enumSystemStatus.Run)
                    {
                        GlobalVar.SystemStatus = enumSystemStatus.Manual;
                    }
                    */
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = false;
                    pbClose.Visible = true;
                    break;
                case FuncInline.enumErrorCode.Door_Opened:

                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                    {
                        GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                    }
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = false;
                    pbClose.Visible = true;
                    break;
                case FuncInline.enumErrorCode.Operator_Call:
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = false;
                    pbClose.Visible = true;
                    break;
                case FuncInline.enumErrorCode.Digital_Input_Check:
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = false;
                    pbClose.Visible = true;
                    break;
                case FuncInline.enumErrorCode.Digital_Output_Check:
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = false;
                    pbClose.Visible = true;
                    break;
                default:
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = false;
                    pbClose.Visible = true;
                    break;
            }
            #endregion

            FuncInline.SystemLogSave = true;

            startTime = Environment.TickCount;
            this.BringToFront();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // 에러 상태 무시하고 정지
            switch (error.ErrorCode)
            {
                case FuncInline.enumErrorCode.E_Stop:
                    //GlobalVar.E_Stop = false;
                    break;
                case FuncInline.enumErrorCode.System_Not_Inited:
                    //GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                    break;
                case FuncInline.enumErrorCode.Door_Opened:
                    break;
                case FuncInline.enumErrorCode.Operator_Call:
                    break;
                case FuncInline.enumErrorCode.Digital_Input_Check:
                    break;
                case FuncInline.enumErrorCode.Digital_Output_Check:
                    break;
                default:
                    break;
            }
            FuncError.RemoveError(error.ErrorCode);
            DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);
            this.Close();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            // 에러 상태 다시 확인하고 시작
            switch (error.ErrorCode)
            {
                /*
                case FuncInline.enumError.E_Stop:
                    if (!DIO.GetDIData(FuncInline.enumDINames.X00_3_Emergency_Stop))
                    {
                        GlobalVar.E_Stop = false;
                        FuncError.RemoveError(error);
                        DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Buzzer, false);
                        this.Close();
                    }
                    break;
                case FuncInline.enumError.System_Not_Inited:
                    if (GlobalVar.ServoInited &&
                        GlobalVar.SystemStatus != enumSystemStatus.BeforeInitialize &&
                        Func.RobotInited())
                    {
                        FuncError.RemoveError(error);
                        DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Buzzer, false);
                        this.Close();
                    }
                    break;
                case FuncInline.enumError.Door_Opened:
                    if (!DIO.GetDIData(FuncInline.enumDINames.X00_4_Position_SensorDoor_Open_0))
                    {
                        FuncError.RemoveError(error);
                        DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Buzzer, false);
                        this.Close();
                    }
                    break;
                    //*/
                case FuncInline.enumErrorCode.Operator_Call:
                    break;
                case FuncInline.enumErrorCode.Digital_Input_Check:
                    break;
                case FuncInline.enumErrorCode.Digital_Output_Check:
                    break;
                default:
                    break;
            }
        }

        private void btnBypass_Click(object sender, EventArgs e)
        {
            // 에러 삭제하고 계속 진행
            FuncError.RemoveError(error.ErrorCode);
            DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);

            Util.Debug("에러창 바이패스 클릭시 Manual 변경");

            GlobalVar.SystemStatus = enumSystemStatus.Manual;
            this.Close();
        }

        private void tmrCheck_Tick(object sender, EventArgs e)
        {
            pbBuzzerStop.Visible = GlobalVar.EnableBuzzer;

            //tmrCheck.Enabled = false;
            // 에러 원인 해제되면 자동 창닫기
            switch (error.ErrorCode)
            {
                case FuncInline.enumErrorCode.E_Stop:
                    break;
                case FuncInline.enumErrorCode.System_Not_Inited:
                    break;
                case FuncInline.enumErrorCode.Door_Opened:
                    break;
                case FuncInline.enumErrorCode.Operator_Call:
                    break;
                case FuncInline.enumErrorCode.Digital_Input_Check:
                    break;
                case FuncInline.enumErrorCode.Digital_Output_Check:
                    break;
                default:
                    break;
            }

            //if (!FuncError.CheckError(error.ErrorCode))
            //{
            //    DIO.WriteDOData(FuncInline.enumDONames.Y00_3_Buzzer, false);

            //    this.Close();
            //    return;
            //}

            // 부저는 정해진 시간만큼
            // 0이면 사용안함, 나머지는 세팅 시간만큼
            /*
            DIO.WriteDOData(FuncInline.enumDONames.Y00_3_Buzzer,
                              GlobalVar.EnableTower &&
                              GlobalVar.EnableBuzzer &&
                              (GlobalVar.TowerTime[(int)GlobalVar.SystemStatus] == 0 ||
                                    Environment.TickCount - GlobalVar.BuzzerTime < GlobalVar.TowerTime[(int)GlobalVar.SystemStatus] * 1000));
                              //*/

            // tower led는 main에서 제어

            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    tmrCheck.Enabled = true;
            //}
        }


        private void pbClose_Click(object sender, EventArgs e)
        {
            /*
            // 에러 상태 무시하고 정지
            switch (error.ErrorCode)
            {
                case FuncInline.enumErrorCode.E_Stop:
                    //GlobalVar.E_Stop = false;
                    break;
                case FuncInline.enumErrorCode.System_Not_Inited:
                    //GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                    break;
                case FuncInline.enumErrorCode.Door_Opened:
                    break;
                case FuncInline.enumErrorCode.Operator_Call:
                    break;
                case FuncInline.enumErrorCode.Digital_Input_Check:
                    break;
                case FuncInline.enumErrorCode.Digital_Output_Check:
                    break;
                default:
                    break;
            }
            FuncError.RemoveError(error.ErrorCode);
            DIO.WriteDOData(FuncInline.enumDONames.Y00_3_Buzzer, false);

            if(!GlobalVar.SystemErrored &&
                GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                GlobalVar.SystemStatus = enumSystemStatus.Manual;
            }            
            //*/

            if (error.ErrorCode == FuncInline.enumErrorCode.Operator_Call) // Operator Call 시에는 아무 것도 안 한다.
            {
                if (GlobalVar.SystemErrored)
                {
                    FuncError.RemoveError(error.ErrorCode);
                    GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0 ? true : false;
                    FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.System] = false;
                    if (!GlobalVar.SystemErrored)
                    {
                        GlobalVar.SystemStatus = GlobalVar.SystemStatus <= enumSystemStatus.Initialize ? enumSystemStatus.BeforeInitialize : enumSystemStatus.Manual;
                    }
                }
                else // 정지 없이 알림으로만 발생한 경우
                {
                    FuncInline.BuyerChangeBeforeCheck = false;
                    FuncInline.BuyerChangeBeforeEnd = false;
                }
            }
            else if (error.ErrorCode >= FuncInline.enumErrorCode.Run_Stopped)
            {
                GlobalVar.SystemStatus = GlobalVar.SystemStatus <= enumSystemStatus.Initialize ? enumSystemStatus.BeforeInitialize : enumSystemStatus.Manual;
                FuncError.RemoveError(error.ErrorCode);
                GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0 ? true : false;
                FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.System] = false;
            }
            else if (GlobalVar.SystemErrored)
            {
                // 창만 닫고 PartClear 창으로 이동한다.
                FuncInline.TabMain = FuncInline.enumTabMain.PartClear;
            }
            else
            {
                // 창만 닫고 Errors 창으로 이동한다.
                FuncInline.TabMain = FuncInline.enumTabMain.Errors;
            }

            this.Close();
        }

        private void pbBuzzerStop_Click(object sender, EventArgs e)
        {
            GlobalVar.EnableBuzzer = false;
        }
    }
}
