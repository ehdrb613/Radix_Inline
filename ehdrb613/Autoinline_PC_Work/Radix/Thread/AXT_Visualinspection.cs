using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Windows.Forms;

namespace Radix
{
    class AXT_Visualinspection
    {


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


            int acsRecive = 0;
            int turnCount = 0;
            bool turn = false;
            bool Half_Move = false;

            /* 공정 역순으로 로직 배치를 해야 선행 프로세스가 후행부분에 영향을 안 준다. 
             * 특히 시뮬레이션시 더함 
             * 컨베어는 순서 상관없이 가장 뒤쪽으로 가야 함 //*/



            while (!GlobalVar.GlobalStop)
            {
                #region 자동운전 중일때
                if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun &&
                    GlobalVar.SystemStatus != enumSystemStatus.ErrorStop) // 에러상태 아닐 때
                {
                    #region AGV 연결

                    #region AVG 체크
                    AXT_AGVThread.AGVCheckStage(0);
                    #endregion

                    #region AGV - Docking
                    if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.Docking))
                    {
                        GlobalVar.AGVJobCodeReceive_0 = true;
                        SECS_Wait = true;
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

                        if (!GlobalVar.AxisStatus[0].StandStill)
                        {
                            //그냥 넘어감
                        }
                        else
                        {
                            GlobalVar.AGVReady_0 = true;
                            GlobalVar.AGV_0_Working = true;//////////확인 해야 됨.
                        }
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

                        if (!GlobalVar.AxisStatus[0].StandStill)
                        {
                            //그냥 넘어감
                        }
                        else
                        {
                            GlobalVar.AGVReady_0 = true;
                            GlobalVar.AGV_0_Working = true;//////////확인 해야 됨.
                        }
                    }
                    #endregion

                    #region AGV - Complet
                    else if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.WorkFinish))
                    {
                        GlobalVar.AGVFront_Door_Close = true;
                        turn = true;                                   
                    }
                    #endregion


                    #endregion

                    #region 1 스텝 무브
                    if (turn)
                    {
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
                            Ecat.MoveRelative(0, 23 * 1000 * 10000, GlobalVar.Servo_Load * GlobalVar.ServoSpeed_AXT);
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

                        turnCount++;
                        
                        if (turnCount > 2)
                        {                                                     
                            turnCount = 0;
                        }
                        turn = false;
                    }
                    #endregion

                    #region 반바퀴 무브
                    if (Half_Move)
                    {

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
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.L, 3); // 6개 stage
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.M, 3); // 처리부 1개?
                        GlobalVar.Secs.SetSECS_Port_Stage_Count(enumPortID.U, 3); // 배출 2개?
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
            // 동작 할 실린더가 없다.
            return true;
        }
        #endregion


    }
}
