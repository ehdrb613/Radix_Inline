using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Windows.Forms;

namespace Radix
{
    class AXT_Functioncheck
    {

        // 로딩
        int AXT_LoadPart_Product = 0; // 공급 쪽 제품 카운터
        bool AXT_LoadPart_Product_Check = false;    //공급쪽 제품 확인
        bool AXT_LoadPart_Servo_Move_1_Step = false;    //로드 서버 1스텝 무브
        bool AXT_LoadPart_Robot_Get = false; //로봇 제품 가지러 가는 단계

        bool AXT_WorkPart_Before_Align = false; //작업 전 정렬

        //작업      

        bool AXT_WorkPart_Robot_Put = false; //로봇 작업공간에 제품을 놓는 단계
        bool AXT_WorkPart_Robot_Get = false; //로봇 작업이 완료 된 제품을 가져가는 단계



        bool AXT_WorkPart_FinishCheck = false; //작업이 완료된 넘버 체크




        bool Work_0 = false; //작업공간 1 작업 확인
        bool Work_1 = false; //작업공간 1 작업 확인
        bool Work_2 = false; //작업공간 1 작업 확인
        bool Work_3 = false; //작업공간 1 작업 확인
        bool Work_4 = false; //작업공간 1 작업 확인
        bool Work_5 = false; //작업공간 1 작업 확인
        bool Work_6 = false; //작업공간 1 작업 확인
        bool Work_7 = false; //작업공간 1 작업 확인

        bool AXT_WorkPart_After_Align = false; //작업 전 정렬

        //언로딩
        bool AXT_UnloadPart_Robot_Get = false; //로봇 워크에서 제품을 가지러 가는 단계
        bool AXT_UnloadPart_Robot_Put = false; //로봇 워크에서 제품을 가지러 가는 단계

        bool AXT_UnloadPart_Product_Move = false; //언로딩 파트에서 제품이 이동하는 단계
        bool AXT_UnloadPart_Product_Fix = false; //언로딩 파트에서 제품이 고정하는 단계
        bool AXT_UnloadPart_Agv_wait = false; //언로딩 파트에서 제품이 고정하는 단계

        //SECS
        bool SECS_Wait = false; //언로딩 파트에서 제품이 이동하는 단계


        private void debug(string str)
        {
            Util.Debug(str);
        }



        public void Run()
        {
            #region stopper di change 감지 변수
            bool di_change_distribute = false;
            bool di_change_fluid1 = false;
            bool di_change_fluid2 = false;


            #endregion
            int can_align_time = Environment.TickCount;
            int cap_push_time = Environment.TickCount;
            int cover_push_time = Environment.TickCount;


            /* 공정 역순으로 로직 배치를 해야 선행 프로세스가 후행부분에 영향을 안 준다. 
             * 특히 시뮬레이션시 더함 
             * 컨베어는 순서 상관없이 가장 뒤쪽으로 가야 함 //*/

            while (!GlobalVar.GlobalStop)
            {
                #region 자동운전 중일때
                if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun &&
                    GlobalVar.SystemStatus != enumSystemStatus.ErrorStop) // 에러상태 아닐 때
                {
                    #region Door Check
                    if (GlobalVar.UseDoor)
                    {
                        if (DIO.GetDIData(enumDINames.X00_5_Left_Door_Sensor))
                        {
                            Func.AddError(enumError.Door_Left_Opened);
                        }
                        if (DIO.GetDIData(enumDINames.X00_6_Right_Door_Sensor))
                        {
                            Func.AddError(enumError.Door_Right_Opened);
                        }
                        if (DIO.GetDIData(enumDINames.X00_7_Back_side_Door_Sensor))
                        {
                            Func.AddError(enumError.Door_Back_Opened);
                        }
                    }
                    #endregion          

                    #region 작업공간 비어 있는지 확인
                    Work_Empty_Check();
                    Work_Work_Check();
                    Work_Finish_Check();
                    #endregion

                    #region AGV 연결

                    #region AVG 체크
                    AXT_AGVThread.AGVCheckStage(0);
                    #endregion

                    #region AGV - Docking
                    if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.Docking))
                    {
                        if (!DIO.GetDIData(enumDINames.X01_2_Product_Arrival_Check_Sensor))
                        {
                            GlobalVar.AGVJobCodeReceive_0 = true;
                            SECS_Wait = true;
                        }
                    }
                    #endregion

                    #region AGV - Load
                    else if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.Load))
                    {
                        if (SECS_Wait)
                        {
                            GlobalVar.Secs.SetSECS_EquipmentState(enumEquipmentState.Run);
                            GlobalVar.Secs.S6F11_EventReport(enumECID.EquipmentStateChange);
                            SECS_Wait = false;
                        }

                        #region 제품이 있다면 그냥 넘어감.
                        if (DIO.GetDIData(enumDINames.X01_2_Product_Arrival_Check_Sensor))
                        {

                        }
                        #endregion
                        #region 제품이 없다면 제품 공급 확인
                        else//제품이 없다면
                        {
                            if (GlobalVar.RobotWorking ||
                               !GlobalVar.AxisStatus[0].StandStill)
                            {
                                //그냥 넘어감
                            }
                            else
                            {
                                GlobalVar.AGV_0_Working = true;//////////확인 해야 됨.
                                if (DIO.GetDIData(enumDINames.X01_0_))
                                {
                                    AXT_Door_Open();
                                }
                                int checkTime = Environment.TickCount;
                                while (!GlobalVar.GlobalStop)
                                {
                                    if (DIO.GetDIData(enumDINames.X01_1_))
                                    {
                                        //조건 보고 해야 된다.
                                        GlobalVar.AGVReady_0 = true;
                                        break;
                                    }
                                    if (Environment.TickCount - checkTime > 10 * 1000)
                                    {
                                        Func.AddError(enumError.Door_Front_Open_Timeout);
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region AGV - UnLoad
                    else if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.UnLoad))
                    {
                        if (SECS_Wait)
                        {
                            GlobalVar.Secs.SetSECS_EquipmentState(enumEquipmentState.Run);
                            GlobalVar.Secs.S6F11_EventReport(enumECID.EquipmentStateChange);
                            SECS_Wait = false;
                        }
                        if (GlobalVar.RobotWorking ||
                              !GlobalVar.AxisStatus[1].StandStill)
                        {
                            //그냥 넘어감
                        }
                        else
                        {
                            GlobalVar.AGV_0_Working = true;//////////확인 해야 됨.
                            if (DIO.GetDIData(enumDINames.X02_1_Conveyor_Product_End) &&     //언로딩 파트 제품이 있고                            
                                !GlobalVar.RobotWorking)                            //로봇이 동작 하지 않을 때
                            {
                                if (DIO.GetDIData(enumDINames.X01_0_))
                                {
                                    AXT_Door_Open();
                                }
                                int checkTime = Environment.TickCount;
                                while (!GlobalVar.GlobalStop)
                                {
                                    if (DIO.GetDIData(enumDINames.X01_1_))
                                    {
                                        GlobalVar.AXT_UnLoadPart_Product--;

                                        //조건 보고 해야 된다.
                                        GlobalVar.AGVReady_0 = true;
                                        break;
                                    }
                                    if (Environment.TickCount - checkTime > 10 * 1000)
                                    {
                                        Func.AddError(enumError.Door_Front_Open_Timeout);
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                            }
                        }
                    }
                    #endregion

                    #region AGV - Complet
                    else if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.WorkFinish))
                    {
                        if (GlobalVar.AGVContinue_0)
                        {
                            GlobalVar.AGVFront_Door_Close = true;
                        }
                        else
                        {
                            if (DIO.GetDIData(enumDINames.X01_1_))
                            {
                                AXT_Door_Close();
                            }
                            int checkTime = Environment.TickCount;
                            while (!GlobalVar.GlobalStop)
                            {
                                if (DIO.GetDIData(enumDINames.X01_0_))
                                {
                                    GlobalVar.AGVFront_Door_Close = true;
                                    break;
                                }
                                if (Environment.TickCount - checkTime > 10 * 1000)
                                {
                                    Func.AddError(enumError.Door_Front_Open_Timeout);
                                    break;
                                }
                                Thread.Sleep(100);
                            }
                        }
                    }
                    #endregion

                    #endregion



                    #region Loading Part

                    #region 제품 확인
                    if (DIO.GetDIData(enumDINames.X01_2_Product_Arrival_Check_Sensor) ||
                        GlobalVar.Secs.GetStageLoaderCount() > 0 ||
                        //GlobalVar.AXT_LoadPart_Product > 0 ||
                        (GlobalVar.DryRun && !GlobalVar.AXT_Working))
                    {
                        if (DIO.GetDIData(enumDINames.X01_2_Product_Arrival_Check_Sensor))
                        {
                            //GlobalVar.AXT_Display_Load_0 = true;
                        }
                        AXT_LoadPart_Servo_Move_1_Step = true;
                    }
                    #endregion

                    #region 제품 1칸 이동
                    if (AXT_LoadPart_Servo_Move_1_Step)
                    {
                        #region 안전 확인
                        if (!DIO.GetDIData(enumDINames.X01_3_Product_Release_Check_Sensor) && //로봇이 가져갈 위치에 제품이 있는지 확인
                            !GlobalVar.AGV_0_Working &&                                        //AGV가 동작 중인지 확인
                            !GlobalVar.RobotWorking &&                                           //로봇이 동작 중인지 확인
                            GlobalVar.AxisStatus[0].StandStill &&
                            !GlobalVar.AXT_Working)
                        {//1스텝 전진
                            if (DIO.GetDIData(enumDINames.X01_2_Product_Arrival_Check_Sensor))
                            {
                                GlobalVar.AXT_LoadPart_Product++;

                                #region SECS 사용시
                                if (GlobalVar.UseSecs)
                                {
                                    GlobalVar.Secs.SetSECS_EquipmentState(enumEquipmentState.Run);
                                    GlobalVar.Secs.S6F11_EventReport(enumECID.EquipmentStateChange);

                                    GlobalVar.Secs.SetSECS_Port_Stage_Rotate(enumPortID.L, 0); // 로더(회전식) 포트를 한 칸 회전시킴

                                    #region 4.포트 정보 변경 보고
                                    GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 0, "UNKNOWN", "", ""); // 투입된 stage 제품 정보 저장. 제품ID는 remoteCommand로 받기 전이라 모른다.
                                    GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 현재 포트상태 레포트 전송
                                    #endregion

                                    #region 5.빈 아이디로 제품정보 보고. EqpID,OpID,ProductID
                                    GlobalVar.UseConversation = true; // 제품아이디 보고 후 원격지령을 대기한다.
                                    GlobalVar.Secs.SetSECS_ProductID("UNKNOWN"); // 투입 처리부 제품 없음
                                    GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID);
                                    #endregion

                                    SECS_Wait = true;
                                }
                                #endregion
                            }


                            #region SECS 사용시  YJ20210923 위치 변경
                            bool conversationChecked = GlobalVar.Secs.CheckConveration();

                            if (SECS_Wait)
                            {
                                #region 6.Host로부터 지령 대기
                                while (!GlobalVar.GlobalStop)
                                {
                                    structRemoteCommand remoteCmd = GlobalVar.Secs.GetRemoteCommand(); // 한번 조회하면 초기화되므로 변수로 받아서 써야 한다.
                                    #region Host로부터 Start 지령 수신한 경우
                                    if (remoteCmd.Command == enumRemoteCommand.Start)
                                    {
                                        // Remote command에 product ID 있으므로, 이를 로딩 0 스테이지에 적용                                        

                                        if (GlobalVar.RemoteLot != remoteCmd.LotID)
                                        {
                                            GlobalVar.AXT_totoalProductCount = 0;
                                        }
                                        GlobalVar.RemoteLot = remoteCmd.LotID;

                                        int stage = 0;
                                        GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, stage,  // 앞에서 한번 rotate 했으면 1, 안 했으면 0 stage에 받은 정보 저장
                                                                                remoteCmd.ProductID,
                                                                                remoteCmd.LotID,
                                                                                remoteCmd.RecipeID);

                                        #region 4.포트 정보 변경 보고
                                        GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 현재 포트상태 레포트 전송
                                        #endregion
                                        conversationChecked = false;
                                        break;
                                    }
                                    #endregion
                                    #region Host로부터 cancel 지령 수신한 경우
                                    else if (remoteCmd.Command == enumRemoteCommand.Cancel)
                                    {
                                        DialogResult resultDlg = MessageBox.Show("Host로부터 취소 지령이 수령되었습니다.현상 조치가 되었다면 OK, 취소하려면 Cancel을 클릭하세요.", "Cancel By Host", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                        #region retry 선택시 제품 아이디 보고
                                        if (resultDlg == DialogResult.OK)// 유저 OK NG 따라서 시퀀스 다시 구현 해야 될 것 같다.
                                        {
                                            #region 제품ID 전송
                                            GlobalVar.UseConversation = true; // 제품아이디 보고 후 원격지령을 대기하지 않는다. 원래는 대기해야 하나 시뮬레이션 종료 위해 안 함.
                                            GlobalVar.Secs.SetSECS_ProductID("UNKNOWN"); // 제품 정보 저장

                                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID); // 제품ID 레포트 전송
                                            #endregion

                                        }
                                        #endregion
                                        #region abort 선택시 취소 보고
                                        else
                                        {
                                            #region 4.포트 정보 변경 보고
                                            GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 0, "", "", ""); // 투입된 stage 제품 정보 저장. 제품ID는 remoteCommand로 받기 전이라 모른다.
                                            GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 현재 포트상태 레포트 전송
                                            #endregion

                                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessAbort);
                                            Func.AddError(enumError.SECS_Cancle_Receive);
                                        }
                                        #endregion
                                        conversationChecked = true;
                                        break;
                                    }
                                    #endregion

                                    conversationChecked = GlobalVar.Secs.CheckConveration();

                                    if (conversationChecked)
                                    {
                                        #region SECS TimeOut
                                        if (conversationChecked)
                                        {
                                            // Transaction Timeout 보고
                                            //GlobalVar.Secs.S9F13_ConversationTimeout(); //유저 OK NG 따라서 시퀀스 다시 구현 해야 될 것 같다.

                                            // 사용자에게 팝업 처리
                                            Func.AddError(enumError.SECS_TimeOut);//
                                            DialogResult result = MessageBox.Show("원격지령이 수령되지 않았습니다.  재시도를 원하면 OK, 취소하려면 Cancel을 클릭하세요.", "Conversation Timeout", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                                            // conversation check clear
                                            GlobalVar.Secs.ClearConverstaion();

                                            if (result == DialogResult.OK)
                                            {

                                            }
                                            else
                                            {
                                                #region 4.포트 정보 변경 보고
                                                GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 0, "", "", ""); // 투입된 stage 제품 정보 저장. 제품ID는 remoteCommand로 받기 전이라 모른다.
                                                GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 현재 포트상태 레포트 전송
                                                GlobalVar.SecsCancleReceive = true;
                                                #endregion                                                
                                            }
                                        }
                                        #endregion

                                        break;
                                    }
                                    Thread.Sleep(100);
                                }
                                GlobalVar.Secs.ClearConverstaion();
                                #endregion

                                SECS_Wait = false;
                            }
                            #endregion

                            if (!conversationChecked) // YJ20210923 위치/조건 재점검 필요
                            {
                                Ecat.MoveRelative(0, (40 * 10000), GlobalVar.Servo_Load * GlobalVar.ServoSpeed_AXT);
                                if (GlobalVar.UseSecs)
                                {
                                    #region 4.포트 정보 변경 보고
                                    GlobalVar.Secs.SetSECS_Port_Stage_Rotate(enumPortID.L, 1); // 로더 한 칸 회전
                                    //4번째 5번째는 제품이 있으면 안된다.
                                    GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 4, "", "", "");
                                    GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, 5, "", "", "");
                                    for (int i = 1; i <= 3; i++)
                                    {
                                        if (GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, i).ProductID == "UNKNOWN")
                                        {
                                            GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, i, "", "", "");
                                        }
                                    }
                                    ///////////////////////////////////////////////////////////////
                                    GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 현재 포트상태 레포트 전송
                                    #endregion
                                }
                            }

                            Thread.Sleep(100);

                            AXT_LoadPart_Servo_Move_1_Step = false;
                            GlobalVar.AXT_1_Step = true;
                        }
                        #endregion
                    }
                    #endregion

                    #region 로봇 동작 시작 확인
                    if ((DIO.GetDIData(enumDINames.X01_3_Product_Release_Check_Sensor) &&
                        !GlobalVar.AGV_0_Working &&                                        //AGV가 동작 중이 아니면
                        !GlobalVar.AXT_Working) ||           //작업 공간에서 작업중이 아니면                         
                        (GlobalVar.DryRun && !GlobalVar.AXT_Working))
                    {
                        //GlobalVar.Working_Number = Work_Empty_Check();향후 사용 예정
                        if (GlobalVar.Working_Number > 8) GlobalVar.Working_Number = 1;
                        if (GlobalVar.Working_Number > -1)
                        {
                            AXT_LoadPart_Robot_Get = true;
                            SECS_Wait = true;
                        }
                    }
                    #endregion

                    #region Robot Loading Get
                    if (AXT_LoadPart_Robot_Get &&
                        GlobalVar.AxisStatus[0].StandStill &&
                        !GlobalVar.AGV_0_Working)
                    {
                        GlobalVar.AXT_Working = true;// 작업이 시작 되었다.
                        GlobalVar.AXT_LoadPart_Product--;

                        #region SECS 사용시
                        if (GlobalVar.UseSecs && SECS_Wait)
                        {
                            #region Process 상태 변경 보고
                            GlobalVar.Secs.SetSECS_LotID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).LotID);
                            GlobalVar.Secs.SetSECS_ProductID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).ProductID);
                            GlobalVar.Secs.SetSECS_RecipeID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).RecipeID);
                            GlobalVar.Secs.SetSECS_ProcessState(enumProcessState.Excution);
                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessStateChange);
                            #endregion                                                     

                            #region 7.포트 변경 보고
                            GlobalVar.Secs.SetSECS_Port_Stage_Move(enumPortID.L, 3, enumPortID.M, 0);
                            GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 포트상태 레포트 전송
                            #endregion

                            // 8.처리 시작

                            #region 10.제품 처리 시작 보고
                            GlobalVar.Secs.SetSECS_ProductID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).ProductID); // 로딩부(stage3)의 제품 ID
                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductProcessStart);
                            #endregion                            
                            SECS_Wait = false;
                        }
                        #endregion

                        if (!GlobalVar.Loading_Part_Get1)
                        {
                            GlobalVar.Loading_Part_Get1 = true;//쿠카 쓰레드에서 해당 동작을 함.

                        }

                        if (GlobalVar.Loading_Part_Get1_Finish)//쿠카 쓰레드에서 동작이 완료 되면
                        {
                            GlobalVar.Loading_Part_Get1 = false;
                            GlobalVar.Loading_Part_Get1_Finish = false;

                            AXT_LoadPart_Robot_Get = false;

                            AXT_WorkPart_Before_Align = true;
                        }
                    }
                    #endregion  

                    #endregion


                    #region Work Part             
                    #region 작업전 정렬
                    if (AXT_WorkPart_Before_Align)
                    {
                        if (!GlobalVar.Working_Before_Align)
                        {
                            GlobalVar.Working_Before_Align = true;//쿠카 쓰레드에서 해당 동작을 함.
                        }
                        if (GlobalVar.Working_Before_Align_Finish)//쿠카 쓰레드에서 동작이 완료 되면
                        {
                            GlobalVar.Working_Before_Align = false;
                            if (GlobalVar.Working_Number > -1)
                            {
                                GlobalVar.Working_Before_Align_Finish = false;

                                AXT_WorkPart_Before_Align = false;//이번 조건

                                AXT_WorkPart_Robot_Put = true;//다음
                            }
                        }
                    }
                    #endregion

                    #region Robot Work Put
                    if (AXT_WorkPart_Robot_Put)
                    {
                        if (!GlobalVar.Working_Part_Put1)
                        {
                            GlobalVar.Working_Part_Put1 = true;//쿠카 쓰레드에서 해당 동작을 함.
                        }

                        if (GlobalVar.Working_Part_Put1_Finish)//쿠카 쓰레드에서 동작이 완료 되면
                        {
                            GlobalVar.Working_Part_Put1 = false;
                            GlobalVar.Working_Part_Put1_Finish = false;

                            GlobalVar.AXT_LoadPart_Product--;

                            AXT_WorkPart_Robot_Put = false;//이번 조건

                            //GlobalVar.AXT_WorkPart_Empty[GlobalVar.Working_Number] = true;//넣은 곳에 비어 있지 않다고 변수 변경
                            //GlobalVar.AXT_Function_Work_Start[GlobalVar.Working_Number] = true;// 넣은 곳에 작업 시작 변수 변경

                            AXT_WorkPart_FinishCheck = true;//다음
                        }
                    }
                    #endregion

                    #region 작업 완료된 제품 확인                   
                    if (AXT_WorkPart_FinishCheck)
                    {
                        if (!GlobalVar.DryRun &&
                            DIO.GetDIData(enumDINames.X02_0_Conveyor_Product_Start))
                        {
                            Func.AddError(enumError.UnLoad_Product_No_Have);
                        }
                        //GlobalVar.Working_Finish_Number = Work_Finish_Check(); 향후 사용 예정
                        GlobalVar.Working_Finish_Number = GlobalVar.Working_Number;
                        if (GlobalVar.Working_Finish_Number > -1)
                        {
                            AXT_WorkPart_FinishCheck = false;

                            AXT_WorkPart_Robot_Get = true;//다음
                        }

                        #region SECS 사용시
                        if (GlobalVar.UseSecs)
                        {
                            #region 10.제품 처리 결과 보고 YJ20210929 제품은 Machine에 있을 텐데 왜 Unloader에서 조회하지?         
                            GlobalVar.Secs.SetSECS_LotID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).LotID); // YJ20210929 U에서 M으로 변경
                            GlobalVar.UseConversation = false; // 제품아이디 보고 후 원격지령을 대기하지 않는다.
                            GlobalVar.Secs.SetSECS_ProductID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).ProductID); // YJ20210929 U에서 M으로 변경
                            GlobalVar.Secs.SetSECS_RecipeID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).RecipeID); // YJ20210929 U에서 M으로 변경
                                                                                                                                  //GlobalVar.Secs.SetSECS_TotalProductCount(GlobalVar.AXT_totoalProductCount); YJ20210929 총생산량 직접지정에서 증가로 변경
                            GlobalVar.Secs.SetSECS_TotalProductCountAdd(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).LotID); // YJ20210929 총생산량 직접지정에서 증가로 변경
                            if (GlobalVar.Working_Finish_Number != 5)
                            {
                                GlobalVar.Secs.SetSECS_ProcessJudge(enumProcessJudge.OK);
                            }
                            else
                            {
                                GlobalVar.Secs.SetSECS_ProcessJudge(enumProcessJudge.NG);
                            }

                            GlobalVar.Secs.ClearSECS_ProcessData();
                            GlobalVar.Secs.SetSECS_ProcessData("Result", "OK");
                            GlobalVar.Secs.SetSECS_ProcessData("FunctionCheck_Data", "  1234678은 OK    ");
                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProductProcessData);
                            Func.WriteLog("작업 결과 보냈다. ");
                            #endregion
                        }
                        #endregion
                    }
                    #endregion


                    #region Robot Work Get
                    if (AXT_WorkPart_Robot_Get)//기능 검사가 완료 된 것이 있으면 트루 시켜야 함.
                    {
                        if (GlobalVar.UseSecs)
                        {
                            #region 12.프로세스 상태 변경 보고
                            GlobalVar.Secs.SetSECS_LotID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).LotID);
                            GlobalVar.Secs.SetSECS_ProductID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).ProductID);
                            GlobalVar.Secs.SetSECS_RecipeID(GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.M, 0).RecipeID);
                            GlobalVar.Secs.SetSECS_ProcessState(enumProcessState.Idle);
                            GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessStateChange);
                            #endregion

                        }

                        if (!DIO.GetDIData(enumDINames.X02_0_Conveyor_Product_Start) ||
                           !GlobalVar.AGV_0_Working)
                        {
                            if (!GlobalVar.Working_Part_Get1)
                            {
                                GlobalVar.Working_Part_Get1 = true;//쿠카 쓰레드에서 해당 동작을 함.
                            }
                            if (GlobalVar.Working_Part_Get1_Finish)//쿠카 쓰레드에서 동작이 완료 되면
                            {
                                GlobalVar.Working_Part_Get1 = false;
                                GlobalVar.Working_Part_Get1_Finish = false;

                                AXT_WorkPart_Robot_Get = false;//이번 조건

                                AXT_WorkPart_After_Align = true;//다음
                            }
                        }
                    }
                    #endregion

                    #region 작업후 정렬
                    if (AXT_WorkPart_After_Align)
                    {
                        if (!GlobalVar.Working_After_Align)
                        {
                            GlobalVar.Working_After_Align = true;//쿠카 쓰레드에서 해당 동작을 함.
                        }

                        if (GlobalVar.Working_After_Align_Finish)//쿠카 쓰레드에서 동작이 완료 되면
                        {
                            GlobalVar.Working_After_Align = false;
                            GlobalVar.Working_After_Align_Finish = false;

                            AXT_WorkPart_After_Align = false;//이번 조건

                            AXT_UnloadPart_Robot_Put = true;//다음
                        }
                    }
                    #endregion


                    #endregion

                    #region Unloading Part

                    #region Robot Unloading Put
                    if (AXT_UnloadPart_Robot_Put)
                    {
                        if (!GlobalVar.Unloading_Part_Put1)
                        {
                            GlobalVar.Unloading_Part_Put1 = true;//쿠카 쓰레드에서 해당 동작을 함.
                        }

                        if (GlobalVar.Unloading_Part_Put1_Finish)//쿠카 쓰레드에서 동작이 완료 되면
                        {
                            GlobalVar.AXT_Working = false;

                            GlobalVar.Unloading_Part_Put1 = false;
                            GlobalVar.Unloading_Part_Put1_Finish = false;

                            AXT_UnloadPart_Robot_Put = false;

                            GlobalVar.Working_Number++;////////////////임시로 증가함

                            AXT_UnloadPart_Product_Move = true;
                        }
                    }
                    #endregion

                    #region Axis 1 Move
                    if (AXT_UnloadPart_Product_Move)
                    {
                        if (DIO.GetDIData(enumDINames.X02_0_Conveyor_Product_Start) ||
                            GlobalVar.DryRun)
                        {
                            GlobalVar.AXT_Display_UnLoad_5 = true;
                            #region SECS 사용시
                            if (GlobalVar.UseSecs)
                            {
                                #region 7.포트 변경 보고                            
                                GlobalVar.Secs.SetSECS_Port_Stage_Move(enumPortID.M, 0, enumPortID.U, GlobalVar.Secs.GetSECS_Port_Stage_Count(enumPortID.U) - 1);
                                GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 포트상태 레포트 전송
                                #endregion                                            
                            }
                            #endregion

                            GlobalVar.AXT_UnLoadPart_Product++;

                            Ecat.MoveRelative(1, 200 * 10000, GlobalVar.Servo_UnLoad * GlobalVar.ServoSpeed_AXT);
                            Thread.Sleep(500);
                            AXT_UnloadPart_Product_Move = false;
                            AXT_UnloadPart_Product_Fix = true;
                        }
                        else
                        {
                            Func.AddError(enumError.UnLoad_Product_No_Have);
                            AXT_UnloadPart_Product_Move = false;
                        }
                    }
                    #endregion

                    #region Product Fix
                    if (AXT_UnloadPart_Product_Fix)
                    {
                        if (DIO.GetDIData(enumDINames.X02_1_Conveyor_Product_End) ||
                            (GlobalVar.DryRun && GlobalVar.AxisStatus[1].StandStill))
                        {
                            DIO.WriteDOData(enumDONames.Y01_7_Conveyor_Product_Fixed, true);
                            Thread.Sleep(500);
                            DIO.WriteDOData(enumDONames.Y01_7_Conveyor_Product_Fixed, false);

                            AXT_UnloadPart_Product_Fix = false;

                            AXT_UnloadPart_Agv_wait = true;

                            GlobalVar.AXT_totoalProductCount++;

                            #region SECS 사용시
                            if (GlobalVar.UseSecs)
                            {
                                #region 7.포트 변경 보고                            
                                GlobalVar.Secs.SetSECS_Port_Stage_Rotate(enumPortID.U, 1);
                                GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 포트상태 레포트 전송                          
                                #endregion
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #endregion


                    #region SECS IDLE 만들기
                    if (!GlobalVar.AGV_0_Working && //AGV 안 붙어 있는 상태
                        !GlobalVar.RobotWorking && // 로봇이 안 움직이는 상태
                        !GlobalVar.AXT_Working &&// 작업 공간에서 작업을 하지 않을 때
                        !GlobalVar.Secs.GetStageTotalExist() &&
                        GlobalVar.Secs.GetSECS_EquipmentState() != enumEquipmentState.Idle)
                    {
                        GlobalVar.Secs.SetSECS_EquipmentState(enumEquipmentState.Idle);
                        GlobalVar.Secs.S6F11_EventReport(enumECID.EquipmentStateChange);
                        SECS_Wait = false;
                    }
                    #endregion

                    #region 워크 캔슬에 대한 변수 변경 향후 검사기 관련 추가 업데이트 해야 됨.
                    if (GlobalVar.AXT_Work_0_Cancle)
                    {
                        //AXT_Assembly_Press = false;
                        if (Cylinder_Ini())
                        {
                            GlobalVar.AXT_Working = false;
                            GlobalVar.AXT_Work_0_Cancle = false;
                        }
                    }
                    #endregion


                }
                #endregion


                #region 초기화
                else if (GlobalVar.SystemStatus == enumSystemStatus.Initialize) // 초기화 작업일 때
                {

                    #region Secs 초기화
                    if (GlobalVar.UseSecs &&
                        GlobalVar.Init_Secs)
                    {
                        GlobalVar.Init_Secs = false;//초기화 때 1번만 통신하게 하게 위해

                        #region init 상태 보고
                        GlobalVar.Secs.SetSECS_EquipmentState(enumEquipmentState.Init);
                        GlobalVar.Secs.S6F11_EventReport(enumECID.EquipmentStateChange);
                        #endregion

                        // 포트 목록 관리는 초기에 한번만 해도 된다.
                        GlobalVar.Secs.ClearSECS_Port_Stage_Count();
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.L, 6); // 6개 stage
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.M, 8); // 처리부 1개?
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.U, 6); // 배출 2개?
                        GlobalVar.Secs.SetSECS_EQPID(GlobalVar.SecsName); // OP ID도 EqID랑 같이 쓰기 때문에 EqID만 지정. 초기에 한번만 해도 된다.

                        // 포트 내 stage 제품 정보 초기화
                        GlobalVar.Secs.ClearSECS_Port_Stage_State(); // 개개 stage의 이동이 아닌 경우 클리어하고 하나씩 지정. 변화가 적은 경우 클리어 없이 변화분만 수정해도 됨
                        GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange);

                        GlobalVar.Secs.SetSECS_TRSMode(enumTransferControlMode.Online);
                        GlobalVar.Secs.S6F11_EventReport(enumECID.TRSModeChange);
                    }
                    #endregion

                    #region 동작 변수 초기화

                    #endregion

                    #region(실린더 종류 초기화)
                    if (GlobalVar.Init_CylinderCheck)//실린더 초기화 작업이 필요할때 해당 변수를 변경
                    {
                        if (Cylinder_Ini())
                        {
                            GlobalVar.Init_CylinderCheck = false;
                            GlobalVar.Init_Cylinder = true;
                            GlobalVar.Init_AxisCheck = true;
                        }
                    }
                    #endregion

                    #region(서보 종류 초기화)
                    if (GlobalVar.Init_AxisCheck)//라벨 발행 준비 작업이 필요할때 해당 변수를 변경
                    {
                        #region 서보 홈 잡는 동작을 할 때
                        if (GlobalVar.Axis_Homeing)
                        {
                            //서보 초기화             
                            #region 리미트 발생시 조금 이동
                            for (uint servoNum = 0; servoNum < GlobalVar.Axis_count; servoNum++)
                            {
                                #region if 서보 disabled
                                if (GlobalVar.AxisStatus[servoNum].Errored ||
                                    GlobalVar.AxisStatus[servoNum].Disabled ||
                                    GlobalVar.AxisStatus[servoNum].ErrorStop)
                                {
                                    Ecat.ServoReset(servoNum);
                                    Ecat.ServoOn(servoNum, true);
                                    Thread.Sleep(100);
                                }
                                #endregion
                                #region else if 하단 리미트시 상승
                                else if (GlobalVar.AxisStatus[servoNum].LimitSwitchNeg)
                                {
                                    Ecat.MoveAbsolute(servoNum,
                                                    GlobalVar.AxisStatus[servoNum].Position + 5,
                                                    100000);
                                }
                                #endregion
                                #region else if 상단 리미트시 하강
                                else if (GlobalVar.AxisStatus[servoNum].LimitSwitchPos)
                                {
                                    Ecat.MoveAbsolute(servoNum,
                                                    GlobalVar.AxisStatus[servoNum].Position - 5,
                                                    100000);
                                }
                                #endregion
                            }
                            #endregion
                            #region 서보 홈
                            for (int i = 0; i < GlobalVar.Axis_count; i++)
                            {
                                if (i != 1)
                                {
                                    Ecat.MoveHome((uint)i, true, 30000);
                                }

                                //if (!GlobalVar.AxisStatus[i].isHomed ||
                                //    !GlobalVar.AxisStatus[i].HomeAbsSwitch)
                                //{
                                //    Ecat.MoveHome((uint)i, true, 15000);
                                //}
                            }
                            //모든 축에 서버 홈을 다 잡으면 GlobalVar.Init_Axis 해줘야 되는데
                            GlobalVar.Init_AxisCheck = false;
                            GlobalVar.Init_Axis = true;
                            GlobalVar.Init_RobotCheck = true;
                            #endregion
                        }
                        #endregion
                        #region 서보 홈 잡는 동작을 하지 않는다.
                        else
                        {
                            GlobalVar.Init_AxisCheck = false;
                            GlobalVar.Init_Axis = true;
                            GlobalVar.Init_RobotCheck = true;
                        }
                        #endregion
                    }

                    #endregion

                    #region 로봇 종류 초기화
                    if (GlobalVar.Init_RobotCheck)
                    {
                        if (GlobalVar.HyundaeRobotUse)
                        {
                            if (FuncRobot.RobotInitDnet())
                            {
                                GlobalVar.Init_Robot = true;
                            }
                        }
                        else
                        {
                            GlobalVar.Init_Robot = true;
                        }
                    }
                    #endregion
                }
                #endregion

                #region 에러 발생시
                else if (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop) // 에러상태일 때
                {
                    DIO.WriteDOData(enumDONames.Y01_0_, false);
                    DIO.WriteDOData(enumDONames.Y01_1_, false);
                    DIO.WriteDOData(enumDONames.Y01_2_Label_Product_Guide_Cylinder, false);
                    DIO.WriteDOData(enumDONames.Y01_3_Label_Pickup_Cylinder, false);


                    Thread.Sleep(1000);
                }
                #endregion

                #region 비 가동중 초기화 해야 될 체크 변수
                else // 에러 아닌 정지 상태시 시간체크값들 초기화
                {
                    GlobalVar.Weight_Start_Time = Environment.TickCount;
                    GlobalVar.Weight_Check_Time = Environment.TickCount;


                    Thread.Sleep(1000);
                }
                #endregion

                Thread.Sleep(GlobalVar.ThreadSleep);
            }

            #region 자동운전 상태가 아닐때 
            DIO.WriteDOData(enumDONames.Y01_0_, false);
            DIO.WriteDOData(enumDONames.Y01_1_, false);
            DIO.WriteDOData(enumDONames.Y01_2_Label_Product_Guide_Cylinder, false);
            DIO.WriteDOData(enumDONames.Y01_3_Label_Pickup_Cylinder, false);
            #endregion
        }

        public int Work_Empty_Check()
        {
            for (int i = 0; i < 8; i++)
            {
                if (GlobalVar.AXT_WorkPart_Empty[i])
                {
                    return i + 1;
                }
            }
            return -1;
        }
        public int Work_Work_Check()
        {
            for (int i = 0; i < 8; i++)
            {
                if (GlobalVar.AXT_WorkPart_Work[i])
                {
                    return i + 1;
                }
            }
            return -1;
        }
        public int Work_Finish_Check()
        {
            for (int i = 0; i < 8; i++)
            {
                if (GlobalVar.AXT_WorkPart_WorkFinish[i])
                {
                    return i + 1;
                }
            }
            return -1;
        }

        #region 실린더 준비 작업
        public bool Cylinder_Ini()
        {
            AXT_Door_Close();
            if (DIO.GetDIData(enumDINames.X01_0_))
            {
                return true;
            }
            return false;
        }
        #endregion

        public void AXT_Door_Open()
        {
            DIO.WriteDOData(enumDONames.Y01_0_, false);// Close
            DIO.WriteDOData(enumDONames.Y01_1_, true);// Open
        }
        public void AXT_Door_Close()
        {
            DIO.WriteDOData(enumDONames.Y01_0_, true);// Close
            DIO.WriteDOData(enumDONames.Y01_1_, false);// Open
        }
        public void AXT_Robot_Clamp()
        {
            DIO.WriteDOData(enumDONames.Y02_6_, true);//클램프
            DIO.WriteDOData(enumDONames.Y02_7_, false);//언클램프
        }
        public void AXT_Robot_UnClamp()
        {
            DIO.WriteDOData(enumDONames.Y02_6_, false);//클램프
            DIO.WriteDOData(enumDONames.Y02_7_, true);//언클램프
        }

    }
}
