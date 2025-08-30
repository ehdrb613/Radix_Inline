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
        //private enumError error;
        private FuncInline.structError Part_error;
        
        private ulong startTime = GlobalVar.TickCount64;

        //public ErrorDialog(enumError er)
        //{
        //    InitializeComponent();
        //    error = er;
        //}
        public ErrorDialog(FuncInline.structError er)
        {
            InitializeComponent();
            Part_error = er;
        }

        private void ErrorDialog_Shown(object sender, EventArgs e)
        {
            pnErrorPart.Visible = !GlobalVar.UseNormalError;

            #region Normal_Error
            //if (GlobalVar.UseNormalError)
            //{
            //    lblErrorCode.Text = ((int)error).ToString();
            //    lblErrorName.Text = error.ToString();
            //    lblErrorDecription.Text = FuncIni.ReadIniFile("error_desc", error.ToString(), GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\ErrorDesc.ini").Replace("\\", "\n");

            //    #region 에러코드에 따라서 버튼 표시 및 기본 처리
            //    switch (error)
            //    {
            //        case enumError.E_Stop:
            //            GlobalVar.E_Stop = true;
            //            GlobalVar.SystemStatus = enumSystemStatus.EmgStop;
            //            //btnBypass.Visible = false;
            //            //btnRetry.Visible = true;
            //            pbClose.Visible = true;
            //            break;
            //        case enumError.System_Not_Inited:
            //            GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
            //            FuncLog.WriteLog("HJ 확인 - Error Dialog Show(System_Not_Inited)     enumSystemStatus.BeforeInitialize");
            //            /*
            //            if (GlobalVar.SystemStatus == enumSystemStatus.Run)
            //            {
            //                GlobalVar.SystemStatus = enumSystemStatus.Manual;
            //            }
            //            */
            //            //btnBypass.Visible = false;
            //            //btnRetry.Visible = false;
            //            pbClose.Visible = true;
            //            break;
            //        case enumError.Door_Opened:

            //            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            //            {
            //                GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
            //            }
            //            //btnBypass.Visible = false;
            //            //btnRetry.Visible = false;
            //            pbClose.Visible = true;
            //            break;
            //        case enumError.Operator_Call:
            //            //btnBypass.Visible = false;
            //            //btnRetry.Visible = false;
            //            pbClose.Visible = true;
            //            break;
            //        case enumError.Digital_Input_Check:
            //            //btnBypass.Visible = false;
            //            //btnRetry.Visible = false;
            //            pbClose.Visible = true;
            //            break;
            //        case enumError.Digital_Output_Check:
            //            //btnBypass.Visible = false;
            //            //btnRetry.Visible = false;
            //            pbClose.Visible = true;
            //            break;
            //        default:
            //            //btnBypass.Visible = false;
            //            //btnRetry.Visible = false;
            //            pbClose.Visible = true;
            //            break;
            //    }
            //    #endregion
            //}
            #endregion

            #region Part_Error
            //if (GlobalVar.PartError)
            //{
            lblErrorCode.Text = ((int)Part_error.ErrorCode).ToString();
            lblErrorName.Text = Part_error.ErrorCode.ToString();
            lblErrorPart.Text = Part_error.ErrorPart.ToString();
            lblErrorDecription.Text = Part_error.Description;
            //lblErrorDecription.Text = FuncIni.ReadIniFile("error_desc", error.ToString(), GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\ErrorDesc.ini").Replace("\\", "\n");

            #region 에러코드에 따라서 버튼 표시 및 기본 처리            
            switch ((int)Part_error.ErrorCode)
            {
                case (int)enumErrorCode.E_Stop:
                    GlobalVar.E_Stop = true;
                    GlobalVar.SystemStatus = enumSystemStatus.EmgStop;
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = true;
                    pbClose.Visible = true;
                    break;
                case (int)enumErrorCode.System_Not_Inited:
                    GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                    FuncLog.WriteLog("HJ 확인 - Error Dialog Show(System_Not_Inited)     enumSystemStatus.BeforeInitialize");
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
                case (int)enumErrorCode.Door_Opened:

                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                    {
                        GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                    }
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = false;
                    pbClose.Visible = true;
                    break;
                case (int)enumErrorCode.Operator_Call:
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = false;
                    pbClose.Visible = true;
                    break;
                case (int)enumErrorCode.Digital_Input_Check:
                    //btnBypass.Visible = false;
                    //btnRetry.Visible = false;
                    pbClose.Visible = true;
                    break;
                case (int)enumErrorCode.Digital_Output_Check:
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
            //}
            #endregion

            startTime = GlobalVar.TickCount64;
            this.BringToFront();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            #region Normal_Error
            //if (GlobalVar.UseNormalError)
            //{
            //    // 에러 상태 다시 확인하고 시작
            //    switch (error)
            //    {
            //        /*
            //        case enumError.E_Stop:
            //            if (!DIO.GetDIData(FuncInline.enumDINames.X00_3_Emergency_Stop))
            //            {
            //                GlobalVar.E_Stop = false;
            //                FuncError.RemoveError(error);
            //                DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Buzzer, false);
            //                this.Close();
            //            }
            //            break;
            //        case enumError.System_Not_Inited:
            //            if (GlobalVar.ServoInited &&
            //                GlobalVar.SystemStatus != enumSystemStatus.BeforeInitialize &&
            //                Func.RobotInited())
            //            {
            //                FuncError.RemoveError(error);
            //                DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Buzzer, false);
            //                this.Close();
            //            }
            //            break;
            //        case enumError.Door_Opened:
            //            if (!DIO.GetDIData(FuncInline.enumDINames.X00_4_Position_SensorDoor_Open_0))
            //            {
            //                FuncError.RemoveError(error);
            //                DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Buzzer, false);
            //                this.Close();
            //            }
            //            break;
            //            //*/
            //        case enumError.Operator_Call:
            //            break;
            //        case enumError.Digital_Input_Check:
            //            break;
            //        case enumError.Digital_Output_Check:
            //            break;
            //        default:
            //            break;
            //    }
            //}
            #endregion

            #region Part_Error
            //if (GlobalVar.PartError)
            //{
            // 에러 상태 다시 확인하고 시작
            switch ((int)Part_error.ErrorCode)
            {
                /*
                case enumError.E_Stop:
                    if (!DIO.GetDIData(FuncInline.enumDINames.X00_3_Emergency_Stop))
                    {
                        GlobalVar.E_Stop = false;
                        FuncError.RemoveError(error);
                        DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Buzzer, false);
                        this.Close();
                    }
                    break;
                case enumError.System_Not_Inited:
                    if (GlobalVar.ServoInited &&
                        GlobalVar.SystemStatus != enumSystemStatus.BeforeInitialize &&
                        Func.RobotInited())
                    {
                        FuncError.RemoveError(error);
                        DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Buzzer, false);
                        this.Close();
                    }
                    break;
                case enumError.Door_Opened:
                    if (!DIO.GetDIData(FuncInline.enumDINames.X00_4_Position_SensorDoor_Open_0))
                    {
                        FuncError.RemoveError(error);
                        DIO.WriteDOData(FuncInline.enumDONames.Y00_6_Buzzer, false);
                        this.Close();
                    }
                    break;
                    //*/
                case (int)enumErrorCode.Operator_Call:
                    break;
                case (int)enumErrorCode.Digital_Input_Check:
                    break;
                case (int)enumErrorCode.Digital_Output_Check:
                    break;
                default:
                    break;
            }
            //}
            #endregion      
        }

        private void btnBypass_Click(object sender, EventArgs e)
        {
            //// 에러 삭제하고 계속 진행
            //#region Normal_Error
            ////if (GlobalVar.UseNormalError)
            ////{
            ////    FuncError.RemoveError(error);
            ////}
            //#endregion

            //#region Part_Error
            ////if (GlobalVar.PartError)
            ////{       
            //FuncError.RemoveError(Part_error.ErrorCode);
            ////}
            //#endregion

            //DIO.WriteDOData(DIO_BoxPacking_enumDONames.Y01_3_Main_Buzzer, false);

            //GlobalVar.SystemStatus = enumSystemStatus.Manual;
            //this.Close();
        }

        private void tmrCheck_Tick(object sender, EventArgs e)
        {
            #region Normal_Error
            //if (GlobalVar.UseNormalError)
            //{
            //    // 에러 원인 해제되면 자동 창닫기
            //    switch (error)
            //    {
            //        case enumError.E_Stop:
            //            break;
            //        case enumError.System_Not_Inited:
            //            break;
            //        case enumError.Door_Opened:
            //            break;
            //        case enumError.Operator_Call:
            //            break;
            //        case enumError.Digital_Input_Check:
            //            break;
            //        case enumError.Digital_Output_Check:
            //            break;
            //        default:
            //            break;
            //    }
            //}
            #endregion

            #region Part_Error
            //if (GlobalVar.PartError)
            //{
            switch ((int)Part_error.ErrorCode)
            {
                case (int)enumErrorCode.E_Stop:
                    break;
                case (int)enumErrorCode.System_Not_Inited:
                    break;
                case (int)enumErrorCode.Door_Opened:
                    break;
                case (int)enumErrorCode.Operator_Call:
                    break;
                case (int)enumErrorCode.Digital_Input_Check:
                    break;
                case (int)enumErrorCode.Digital_Output_Check:
                    break;
                default:
                    break;
            }
            //}
            #endregion
            // 에러 원인 해제되면 자동 창닫기


            //if (!FuncError.CheckError(error.ErrorCode))
            //{
            //    DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);

            //    this.Close();
            //    return;
            //}

            // 부저는 정해진 시간만큼
            // 0이면 사용안함, 나머지는 세팅 시간만큼
            //*
            //DIO.WriteDOData(DIO_BoxPacking_enumDONames.Y00_7_Buzzer,
            //                  GlobalVar.EnableTower &&
            //                  GlobalVar.EnableBuzzer &&
            //                  (GlobalVar.TowerTime[(int)GlobalVar.SystemStatus] == 0 ||
            //                        GlobalVar.TickCount64 - GlobalVar.BuzzerTime < GlobalVar.TowerTime[(int)GlobalVar.SystemStatus] * 1000));
            //*/

            // tower led는 main에서 제어
        }


        private void pbClose_Click(object sender, EventArgs e)
        {
            #region Normal_Error
            //if (GlobalVar.UseNormalError)
            //{
            //    // 에러 상태 무시하고 정지
            //    switch (error)
            //    {
            //        case enumError.E_Stop:
            //            //GlobalVar.E_Stop = false;
            //            break;
            //        case enumError.System_Not_Inited:
            //            //GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
            //            break;
            //        case enumError.Door_Opened:
            //            break;
            //        case enumError.Operator_Call:
            //            break;
            //        case enumError.Digital_Input_Check:
            //            break;
            //        case enumError.Digital_Output_Check:
            //            break;
            //        default:
            //            break;
            //    }
            //    FuncError.RemoveError(error);
            //}
            #endregion

            #region Part_Error
            //if (GlobalVar.PartError)
            //{
            // 에러 상태 무시하고 정지
            switch ((int)Part_error.ErrorCode)
            {
                case (int)enumErrorCode.E_Stop:
                    //GlobalVar.E_Stop = false;
                    break;
                case (int)enumErrorCode.System_Not_Inited:
                    //GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                    break;
                case (int)enumErrorCode.Door_Opened:
                    break;
                case (int)enumErrorCode.Operator_Call:
                    break;
                case (int)enumErrorCode.Digital_Input_Check:
                    break;
                case (int)enumErrorCode.Digital_Output_Check:
                    break;
                default:
                    break;
            }
            //FuncError.RemoveError(Part_error.ErrorCode);
            //}
            #endregion

            // 에러 클리어 및 부저 컨트롤은 에러창이나 파트클리어에서 확인 후 클리어하는 걸로
            if (GlobalVar.UsePartClear)
            {
                FuncInline.TabMain = FuncInline.enumTabMain.PartClear;
            }
            else
            {
                //DIO.WriteDOData(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer, false);
                //DIO.Tower_Lamp_Buzzer_Control(false);

                //if (!GlobalVar.SystemErrored &&
                //    GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                //{
                //    GlobalVar.SystemStatus = enumSystemStatus.Manual;
                //}

                FuncInline.TabMain = FuncInline.enumTabMain.Errors;

                FuncInline.Mainform.UpdateLogViewerDialog();

            }
            this.Close();


            // DIO 오류가 발생하면 DIO모듈 전원을 확인하고 프로그램을 다시 시작하는것이 좋다.
            if (GlobalVar.G_ErrNo == (int)enumErrorCode.Fatal_System_Error)
            {
                // 시스템이 종료되도록 유도한다. 어차피 DIO 통신불가로 제어를 못하는 상태이다.
                //GlobalVar.GlobalStop = true;
            }
        }
    }
}
