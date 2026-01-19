using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Windows.Forms;

namespace Radix
{
    class AXT_Labeling
    {

        // 로딩

        bool AXT_LoadPart_Product_Check = false;    //공급쪽 제품 확인
        bool AXT_LoadPart_Servo_Move_1_Step = false;    //로드 서버 1스텝 무브
        bool AXT_LoadPart_Robot_Get = false; //로봇 로딩에서 제품을 가지러 가는 단계

        //작업      

        bool AXT_WorkPart_Empty = false; //작업공간 비어 있는지 확인

        bool AXT_WorkPart_Robot_Put = false; //로봇 작업공간에 제품을 놓는 단계        

        bool AXT_AGV_0_Working = false; //첫번째 AGV 동작중
        bool AXT_Working = false;   //워킹 작업 확인


        bool AXT_Labeling_LabelReady = false;    //라벨 발행 준비 작업
        bool AXT_Labeling_LabelPrint = false;    //라벨 발행 
        bool AXT_Labeling_LabelPrint_1 = false;    //라벨 발행 
        bool AXT_Labeling_LabelPickup = false;   //라벨 픽업
        bool AXT_Labeling_LabelVacuum = false;   //라벨 진공
        int AXT_Labeling_LabelVacuumCount = 0;   //라벨 진공 확인 카운터
        bool AXT_Labeling_LabelAttach = false;   //라벨 부착
        bool AXT_Work_Finish = false;   //라벨 완료 대기

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
                                if (GlobalVar.AxisStatus[2].StandStill)
                                {
                                    AXT_Door_Open();
                                }
                                int checkTime = Environment.TickCount;
                                while (!GlobalVar.GlobalStop)
                                {
                                    if (GlobalVar.AxisStatus[2].Position < (GlobalVar.Servo_DoorOpenPosition + 10) * 10000 &&
                                        GlobalVar.AxisStatus[2].Position > (GlobalVar.Servo_DoorOpenPosition - 10) * 10000)
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
                                if (GlobalVar.AxisStatus[2].StandStill)
                                {
                                    AXT_Door_Open();
                                }
                                int checkTime = Environment.TickCount;
                                while (!GlobalVar.GlobalStop)
                                {
                                    if (GlobalVar.AxisStatus[2].Position < (GlobalVar.Servo_DoorOpenPosition + 10) * 10000 &&
                                        GlobalVar.AxisStatus[2].Position > (GlobalVar.Servo_DoorOpenPosition - 10) * 10000)
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
                            if (GlobalVar.AxisStatus[2].StandStill)
                            {
                                AXT_Door_Close();
                            }
                            int checkTime = Environment.TickCount;
                            while (!GlobalVar.GlobalStop)
                            {
                                if (GlobalVar.AxisStatus[2].Position < (GlobalVar.Servo_DoorClosePosition + 10) * 10000 &&
                                    GlobalVar.AxisStatus[2].Position > (GlobalVar.Servo_DoorClosePosition - 10) * 10000)
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

                    //GlobalVar.AXT_LoadPart_Product = 1;
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
                    if (AXT_LoadPart_Servo_Move_1_Step &&
                        !GlobalVar.AXT_1_Step)//임시 사용 SECS 테스트 용으로 
                    {
                        #region 안전 확인
                        if (!DIO.GetDIData(enumDINames.X01_3_Product_Release_Check_Sensor) && //로봇이 가져갈 위치에 제품이 있는지 확인
                            !GlobalVar.AGV_0_Working &&                                       //AGV가 동작 중인지 확인
                            !GlobalVar.RobotWorking &&                                        //로봇이 동작 중인지 확인
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

                            #region UI  Display
                            //GlobalVar.AXT_Display_Load_3 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 3).ProductID.Length > 0;
                            //GlobalVar.AXT_Display_Load_2 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 2).ProductID.Length > 0;
                            //GlobalVar.AXT_Display_Load_1 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 1).ProductID.Length > 0;
                            //GlobalVar.AXT_Display_Load_0 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.L, 0).ProductID.Length > 0;
                            #endregion

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
                        AXT_LoadPart_Robot_Get = true;
                        SECS_Wait = true;
                    }
                    #endregion

                    #region 부품 공급

                    #region Robot Loading Get
                    if (AXT_LoadPart_Robot_Get &&
                        !GlobalVar.AGV_0_Working)
                    {
                        GlobalVar.AXT_Working = true;// 작업이 시작 되었다.
                        GlobalVar.AXT_Display_Load_3 = false;
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

                            AXT_WorkPart_Robot_Put = true;
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

                            AXT_WorkPart_Robot_Put = false;

                            AXT_Labeling_LabelPrint = true;
                        }
                    }
                    #endregion

                    #endregion

                    #endregion


                    #region Work 라벨 Part

                    if (!GlobalVar.RobotWorking) //로봇 워킹 작업 확인
                    {
                        #region 라벨 발행     ************************************************라벨 발행이 필요 할때 True 시켜 줘야 돌아간다.
                        if (AXT_Labeling_LabelPrint)//라벨 발행이 필요 할때 해당 변수를 변경
                        {
                            if (DIO.GetDIData(enumDINames.X02_4_Label_Move_Left_Sensor))
                            {
                                DIO.WriteDOData(enumDONames.Y01_5_Label_Move_Left, false);//좌
                                DIO.WriteDOData(enumDONames.Y01_6_Label_Move_Right, true);//우
                            }
                            else if (DIO.GetDIData(enumDINames.X02_5_Label_Move_Right_Sensor))
                            {
                                if (!GlobalVar.DryRun)
                                {
                                    GlobalVar.Printer.DataMix();
                                }
                                Thread.Sleep(1000);
                                //DIO.WriteDOData(enumDONames.Y02_0_Label_Guide_Cylinder, true);
                                AXT_Labeling_LabelPrint = false;
                                //AXT_Labeling_LabelPickup = true;
                                AXT_Labeling_LabelPrint_1 = true;


                                DIO.WriteDOData(enumDONames.Y01_5_Label_Move_Left, true);//좌
                                DIO.WriteDOData(enumDONames.Y01_6_Label_Move_Right, false);//우
                            }

                        }
                        #endregion

                        #region 라벨 발행    
                        if (AXT_Labeling_LabelPrint_1)//라벨 발행이 필요 할때 해당 변수를 변경
                        {
                            if (DIO.GetDIData(enumDINames.X02_4_Label_Move_Left_Sensor))
                            {
                                AXT_Labeling_LabelPrint_1 = false;
                                AXT_Labeling_LabelPickup = true;
                            }
                            else if (DIO.GetDIData(enumDINames.X02_5_Label_Move_Right_Sensor))
                            {
                                DIO.WriteDOData(enumDONames.Y01_5_Label_Move_Left, true);//좌
                                DIO.WriteDOData(enumDONames.Y01_6_Label_Move_Right, false);//우
                            }

                        }
                        #endregion

                        #region 라벨 픽업
                        if (AXT_Labeling_LabelPickup)//발행된 라벨을 픽업할때 해당 변수를 변경
                        {
                            #region 라벨이 있으면
                            if (DIO.GetDIData(enumDINames.X02_7_Label_Check_Sensor) ||
                                GlobalVar.DryRun)
                            {
                                #region 픽업 실린더가 하강이면(안전 체크)
                                if (DIO.GetDIData(enumDINames.X02_3_Label_Pickup__Down_Sensor))
                                {
                                    DIO.WriteDOData(enumDONames.Y01_3_Label_Pickup_Cylinder, false);
                                }
                                #endregion
                                #region 픽업 실린더가 상승이면
                                else if (DIO.GetDIData(enumDINames.X02_2_Label_Pickup_Up_Sensor))
                                {
                                    #region 무브 실린더가 우측이면
                                    if (DIO.GetDIData(enumDINames.X02_5_Label_Move_Right_Sensor))
                                    {
                                        DIO.WriteDOData(enumDONames.Y01_5_Label_Move_Left, true);
                                        DIO.WriteDOData(enumDONames.Y01_6_Label_Move_Right, false);
                                    }
                                    #endregion
                                    #region 무브 실린더가 좌측이면
                                    else if (DIO.GetDIData(enumDINames.X02_4_Label_Move_Left_Sensor))
                                    {
                                        //DIO.WriteDOData(enumDONames.Y01_3_Label_Pickup_Cylinder, true);
                                        DIO.WriteDOData(enumDONames.Y01_4_Label_Vacuum, true);

                                        AXT_Labeling_LabelPickup = false;
                                        AXT_Labeling_LabelVacuum = true;
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                            #region 라벨이 없으면
                            else
                            {
                                Func.AddError(enumError.Label_Checking_Error);//라벨 감지 에러
                                AXT_Labeling_LabelPrint = true;// 다시 라벨을 뽑으라
                            }
                            #endregion
                        }
                        #endregion

                        #region 진공 확인
                        if (AXT_Labeling_LabelVacuum)
                        {
                            Thread.Sleep(1000);
                            #region 진공 체크
                            if (DIO.GetDIData(enumDINames.X02_6_Label_Vacuum_Sensor) ||
                                GlobalVar.DryRun)
                            {
                                #region 무브 실린더가 좌측이면
                                if (DIO.GetDIData(enumDINames.X02_4_Label_Move_Left_Sensor))
                                {
                                    DIO.WriteDOData(enumDONames.Y01_5_Label_Move_Left, false);//무브 좌측
                                    DIO.WriteDOData(enumDONames.Y01_6_Label_Move_Right, true);//무브 우측
                                }
                                #endregion
                                #region 무브 실린더가 우측이면
                                else if (DIO.GetDIData(enumDINames.X02_5_Label_Move_Right_Sensor))
                                {
                                    DIO.WriteDOData(enumDONames.Y01_3_Label_Pickup_Cylinder, true);//라벨 부착

                                    AXT_Labeling_LabelVacuum = false;
                                    AXT_Labeling_LabelAttach = true;
                                }
                                #endregion
                            }
                            #endregion
                            #region 라벨 진공이 되지 않으면 다시 뽑아라 사용하지 않을 것 같음
                            else if (AXT_Labeling_LabelVacuumCount > 5)
                            {
                                AXT_Labeling_LabelVacuumCount = 0;
                                Func.AddError(enumError.Label_Vacuum_Error);//라벨 진공 에러
                                DIO.WriteDOData(enumDONames.Y01_4_Label_Vacuum, false);//라벨 진공 오프
                                AXT_Labeling_LabelVacuum = false;//진공 확인 변수 끄고
                                AXT_Labeling_LabelPrint = true;// 다시 라벨을 뽑으라
                            }
                            #endregion                                  
                        }
                        #endregion

                        #region 라벨 부착
                        if (AXT_Labeling_LabelAttach)
                        {
                            //모델별로 제품 가이드 상승 하강 체크 해야 겠다.///////////////////////////////////////////////////////////////////
                            #region 픽업 실린더가 하강이면
                            if (DIO.GetDIData(enumDINames.X02_3_Label_Pickup__Down_Sensor))
                            {
                                DIO.WriteDOData(enumDONames.Y01_4_Label_Vacuum, false);//진공
                                Thread.Sleep(1000);
                                DIO.WriteDOData(enumDONames.Y01_3_Label_Pickup_Cylinder, false);//픽업
                                AXT_Labeling_LabelAttach = false;
                                AXT_Labeling_LabelReady = true;

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
                                    GlobalVar.Secs.SetSECS_ProcessJudge(enumProcessJudge.OK); // 검사기 이외에는 의미 없어서 OK로

                                    GlobalVar.Secs.ClearSECS_ProcessData();
                                    GlobalVar.Secs.SetSECS_ProcessData("Result", "OK");
                                    GlobalVar.Secs.SetSECS_ProcessData("Label_Data", GlobalVar.Label_Barcode);
                                    GlobalVar.Secs.S6F11_EventReport(enumECID.ProductProcessData);
                                    Func.WriteLog("작업 결과 보냈다. ");
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion

                        #region 실린더 안전위치 작업
                        if (AXT_Labeling_LabelReady)//// 로봇 부르기전 실린더 초기화
                        {
                            #region 라벨 가이드 실린더가 상승이면
                            if (DIO.GetDIData(enumDINames.X01_4_Label_Guide_Up_Sensor))
                            {
                                //DIO.WriteDOData(enumDONames.Y02_0_Label_Guide_Cylinder, false);
                            }
                            #endregion
                            #region 라벨 가이드 실린더가 하강이면
                            else if (DIO.GetDIData(enumDINames.X01_5_Label_Guide_Down_Sensor))
                            {
                                #region 픽업 실린더가 하강이면(안전 체크)
                                if (DIO.GetDIData(enumDINames.X02_3_Label_Pickup__Down_Sensor))
                                {
                                    DIO.WriteDOData(enumDONames.Y01_3_Label_Pickup_Cylinder, false);
                                }
                                #endregion
                                #region 픽업 실린더가 상승이면
                                else if (DIO.GetDIData(enumDINames.X02_2_Label_Pickup_Up_Sensor))
                                {
                                    #region 무브 실린더가 우측이면
                                    if (DIO.GetDIData(enumDINames.X02_5_Label_Move_Right_Sensor))
                                    {
                                        DIO.WriteDOData(enumDONames.Y01_5_Label_Move_Left, true);
                                        DIO.WriteDOData(enumDONames.Y01_6_Label_Move_Right, false);
                                    }
                                    #endregion
                                    #region 무브 실린더가 좌측이면
                                    else if (DIO.GetDIData(enumDINames.X02_4_Label_Move_Left_Sensor))
                                    {
                                        AXT_Labeling_LabelReady = false;

                                        AXT_UnloadPart_Robot_Get = true;
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion


                    #region Unloading Part

                    #region Robot Unloading Get
                    if (AXT_UnloadPart_Robot_Get)
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

                                AXT_UnloadPart_Robot_Get = false;

                                AXT_UnloadPart_Robot_Put = true;
                            }
                        }
                    }
                    #endregion

                    #region Robot Unloading Put
                    if (AXT_UnloadPart_Robot_Put &&
                        !GlobalVar.AGV_0_Working)
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
                        if ((DIO.GetDIData(enumDINames.X02_1_Conveyor_Product_End) && GlobalVar.AxisStatus[1].StandStill) ||
                            (GlobalVar.DryRun && GlobalVar.AxisStatus[1].StandStill))
                        {
                            DIO.WriteDOData(enumDONames.Y01_7_Conveyor_Product_Fixed, true);
                            Thread.Sleep(1000);
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

                            //GlobalVar.AXT_Display_UnLoad_0 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.U, 0).ProductID.Length > 0;
                            //GlobalVar.AXT_Display_UnLoad_1 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.U, 1).ProductID.Length > 0;
                            //GlobalVar.AXT_Display_UnLoad_2 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.U, 2).ProductID.Length > 0;
                            //GlobalVar.AXT_Display_UnLoad_3 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.U, 3).ProductID.Length > 0;
                            //GlobalVar.AXT_Display_UnLoad_4 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.U, 4).ProductID.Length > 0;
                            //GlobalVar.AXT_Display_UnLoad_5 = GlobalVar.Secs.GetSECS_Port_Stage_Product(enumPortID.U, 5).ProductID.Length > 0;
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


                    #region 모델별 제품 가이드 실린더 제어
                    if (GlobalVar.ModelName == "DefaultModel")
                    {
                        if (DIO.GetDIData(enumDINames.X01_7_Label_Product_Guide_Down_Sensor))
                        {
                            DIO.WriteDOData(enumDONames.Y01_2_Label_Product_Guide_Cylinder, true); //제품 가이드 실린더 업
                        }
                    }
                    else //모델을 등록하고 모델을 이름을 확인해야 함.
                    {
                        if (DIO.GetDIData(enumDINames.X01_6_Label_Product_Guide_Up_Sensor))
                        {
                            DIO.WriteDOData(enumDONames.Y01_2_Label_Product_Guide_Cylinder, false); //제품 가이드 실린더 다운
                        }
                    }
                    #endregion

                    #region 워크 캔슬에 대한 변수 변경
                    if (GlobalVar.AXT_Work_0_Cancle)
                    {
                        AXT_Labeling_LabelVacuum = false;
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
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.M, 1); // 처리부 1개?
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



        #region 실린더 준비 작업
        public bool Cylinder_Ini()
        {
            DIO.WriteDOData(enumDONames.Y01_4_Label_Vacuum, false);
            #region 라벨 가이드 실린더가 상승이면
            if (DIO.GetDIData(enumDINames.X01_4_Label_Guide_Up_Sensor))
            {
                //DIO.WriteDOData(enumDONames.Y02_0_Label_Guide_Cylinder, false);
                return false;
            }
            #endregion
            #region 라벨 가이드 실린더가 하강이면
            else if (DIO.GetDIData(enumDINames.X01_5_Label_Guide_Down_Sensor))
            {
                #region 픽업 실린더가 하강이면(안전 체크)
                if (DIO.GetDIData(enumDINames.X02_3_Label_Pickup__Down_Sensor))
                {
                    DIO.WriteDOData(enumDONames.Y01_3_Label_Pickup_Cylinder, false);
                }
                #endregion
                #region 픽업 실린더가 상승이면
                else if (DIO.GetDIData(enumDINames.X02_2_Label_Pickup_Up_Sensor))
                {
                    #region 무브 실린더가 우측이면
                    if (DIO.GetDIData(enumDINames.X02_5_Label_Move_Right_Sensor))
                    {
                        DIO.WriteDOData(enumDONames.Y01_5_Label_Move_Left, true);
                        DIO.WriteDOData(enumDONames.Y01_6_Label_Move_Right, false);
                    }
                    #endregion
                    #region 무브 실린더가 좌측이면
                    else if (DIO.GetDIData(enumDINames.X02_4_Label_Move_Left_Sensor))
                    {
                        return true;
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
            return false;
        }
        #endregion



        public void AXT_Door_Open()
        {
            Ecat.MoveAbsolute(2, GlobalVar.Servo_DoorOpenPosition * 10000, 1 * GlobalVar.Servo_Door * GlobalVar.ServoSpeed_AXT);
        }
        public void AXT_Door_Close()
        {
            Ecat.MoveAbsolute(2, GlobalVar.Servo_DoorClosePosition * 10000, GlobalVar.Servo_Door * GlobalVar.ServoSpeed_AXT);
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

        #region 6.Host로부터 지령 대기
        public bool Wait_Command()
        {
            while (!GlobalVar.GlobalStop)
            {
                structRemoteCommand remoteCmd = GlobalVar.Secs.GetRemoteCommand(); // 한번 조회하면 초기화되므로 변수로 받아서 써야 한다.
                if (remoteCmd.Command == enumRemoteCommand.Start)
                {
                    // Remote command에 product ID 있으므로, 이를 로딩 0 스테이지에 적용
                    int stage = 0;
                    GlobalVar.Secs.SetSECS_Port_Stage_State(enumPortID.L, stage,  // 앞에서 한번 rotate 했으면 1, 안 했으면 0 stage에 받은 정보 저장
                                                                                            remoteCmd.ProductID,
                                                                                            remoteCmd.LotID,
                                                                                            remoteCmd.RecipeID);

                    #region 4.포트 정보 변경 보고
                    GlobalVar.Secs.S6F11_EventReport(enumECID.PortStateChange); // 현재 포트상태 레포트 전송
                    #endregion

                    return true;
                }
                #region Host로부터 cancel 지령 수신한 경우
                else if (remoteCmd.Command == enumRemoteCommand.Cancel)
                {
                    DialogResult resultDlg = MessageBox.Show("Host로부터 취소 지령이 수령되었습니다.현상 조치가 되었다면 OK, 취소하려면 Cancel을 클릭하세요.", "Cancel By Host", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    #region retry 선택시 제품 아이디 보고
                    if (resultDlg == DialogResult.OK)// 유저 OK NG 따라서 시퀀스 다시 구현 해야 될 것 같다.
                    {
                        #region 제품ID 전송
                        GlobalVar.UseConversation = false; // 제품아이디 보고 후 원격지령을 대기하지 않는다. 원래는 대기해야 하나 시뮬레이션 종료 위해 안 함.
                        GlobalVar.Secs.SetSECS_ProductID("Product1"); // 제품 정보 저장

                        GlobalVar.Secs.S6F11_EventReport(enumECID.ProductID); // 제품ID 레포트 전송
                        #endregion
                        return false;
                    }
                    #endregion
                    #region abort 선택시 취소 보고
                    else
                    {
                        GlobalVar.Secs.S6F11_EventReport(enumECID.ProcessAbort);
                        return true;
                    }
                    #endregion
                }
                #endregion

                if (GlobalVar.Secs.CheckConveration())
                {
                    return true;
                }
                Thread.Sleep(100);
            }
            return true;
        }
        #endregion


    }
}
