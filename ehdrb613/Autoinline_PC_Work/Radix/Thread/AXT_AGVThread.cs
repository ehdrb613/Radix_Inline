using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace Radix
{
    class AXT_AGVThread
    {
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
                    #region 첫 번째 AGV 통신 확인
                    AXT_AGVThread.AGVCheckStage(0);
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


        #region AGV 관련 PIO 통신 함수
        //PIO L_REQ Sequnce : AGV에서 설비로 물건 전달 (Load)
        //PIO R_REQ Sequnce : 설비에서 AGV로 물건 전달 (Unload)
        #region AGV Stage Check
        public static void AGVCheckStage(int AGVnumber)
        {
            #region 첫번째 AGV
            if (AGVnumber == 0)
            {
                DIO.WriteDOData(enumDONames.Y03_7_PIO_0_E_STOP + AGVnumber, true);

                #region AGV -> 설비 (load)
                if (DIO.GetDIData(enumDINames.X03_0_PIO_0_valid + AGVnumber) &&
                    !DIO.GetDIData(enumDINames.X03_1_PIO_0_CS_0 + AGVnumber) &&
                    DIO.GetDIData(enumDINames.X03_2_PIO_0_CS_1 + AGVnumber) &&
                    !DIO.GetDIData(enumDINames.X03_3_PIO_0_CS_2 + AGVnumber))
                {
                    GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Docking;                    

                    #region Job Code Receive
                    //AGV가 도킹 되고 Job 코드를 받았을 때 비트를 변수 변경이 필요하다.
                    if (GlobalVar.AGVJobCodeReceive_0 &&//메인(다른 쓰레드)에서 리시브 변수 변경
                        !GlobalVar.AGVReady_0)
                    {
                        GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Load;
                        AGVJobCodeReceive_L(AGVnumber);
                    }
                    else
                    {
                        AGVJobCodeReceive_S(AGVnumber);
                    }
                    #endregion                
                }
                #endregion
                #region 설비 -> AGV (Unload)
                else if (DIO.GetDIData(enumDINames.X03_0_PIO_0_valid + AGVnumber) &&
                    DIO.GetDIData(enumDINames.X03_1_PIO_0_CS_0 + AGVnumber) &&
                    DIO.GetDIData(enumDINames.X03_2_PIO_0_CS_1 + AGVnumber) &&
                    !DIO.GetDIData(enumDINames.X03_3_PIO_0_CS_2 + AGVnumber))
                {
                    GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Docking;

                    #region Job Code Receive
                    //AGV가 도킹 되고 Job 코드를 받았을 때 비트를 변수 변경이 필요하다.
                    if (GlobalVar.AGVJobCodeReceive_0 &&//메인(다른 쓰레드)에서 리시브 변수 변경
                        !GlobalVar.AGVReady_0)
                    {
                        GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.UnLoad;
                        AGVJobCodeReceive_U(AGVnumber);
                    }
                    else
                    {
                        AGVJobCodeReceive_S(AGVnumber);
                    }
                    #endregion                             
                }
                #endregion

                #region Ready Receive
                //제품을 받을 준비가 되었을 때 레디 비트를 살려줘야 한다.
                if (GlobalVar.AGVReady_0)//메인(다른 쓰레드)에서 레디 변수 변경
                {
                    //TR 받았을 때
                    if (DIO.GetDIData(enumDINames.X03_4_PIO_0_tr_req + AGVnumber) &&
                    !DIO.GetDIData(enumDINames.X03_6_PIO_0_compt + AGVnumber))
                    {
                        GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkReady;
                        AGVChechWorkReady_On(AGVnumber);
                        if (DIO.GetDIData(enumDINames.X03_5_PIO_0_busy + AGVnumber) &&
                            DIO.GetDIData(enumDINames.X03_7_PIO_0_cont + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkContinue;                            
                            GlobalVar.AGVJobCodeReceive_0 = false;
                            GlobalVar.AGVContinue_0 = true;//////////////컨티뉴 변수 추가
                        }
                        else if (DIO.GetDIData(enumDINames.X03_5_PIO_0_busy + AGVnumber))
                        {
                            GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.Working;
                            GlobalVar.AGVJobCodeReceive_0 = false;
                            GlobalVar.AGVContinue_0 = false;//////////////컨티뉴 변수 추가
                        }
                    }
                    //컴플리트 받았을 때
                    else if (DIO.GetDIData(enumDINames.X03_6_PIO_0_compt + AGVnumber))
                    {
                        GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkFinish;
                        if (GlobalVar.AGVFront_Door_Close)
                        {
                            GlobalVar.AGVFront_Door_Close = false;

                            AGVChechWorkReady_Off(AGVnumber);

                            GlobalVar.AGVReady_0 = false;
                            GlobalVar.AGV_0_Working = false;
                        }
                    }
                }
                #endregion

                #region  충전 시작
                #endregion
                #region  충전 종료
                #endregion
                #region 글로벌 변수 초기화
                else
                {
                    //GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.None;
                    //GlobalVar.AGVJobCodeReceive_0 = false;
                    //GlobalVar.AGVReady_0 = false;
                }
                #endregion
            }
            #endregion

            #region 두번째 AGV              확인 해야 됨.
            if (AGVnumber == -8)
            {
                DIO.WriteDOData(enumDONames.Y03_7_PIO_0_E_STOP + AGVnumber, true);


            }
            #endregion

            #region 세번째 AGV              확인 해야 됨.
            if (AGVnumber == -16)
            {
            }
            #endregion
        }
        public static void AGVJobCodeReceive_U(int AGVnumber)
        {
            DIO.WriteDOData(enumDONames.Y03_0_PIO_0_L_REQ + AGVnumber, false);
            DIO.WriteDOData(enumDONames.Y03_1_PIO_0_U_REQ + AGVnumber, true);
        }
        public static void AGVJobCodeReceive_L(int AGVnumber)
        {
            DIO.WriteDOData(enumDONames.Y03_0_PIO_0_L_REQ + AGVnumber, true);
            DIO.WriteDOData(enumDONames.Y03_1_PIO_0_U_REQ + AGVnumber, false);
        }
        public static void AGVJobCodeReceive_S(int AGVnumber)
        {
            DIO.WriteDOData(enumDONames.Y03_0_PIO_0_L_REQ + AGVnumber, false);
            DIO.WriteDOData(enumDONames.Y03_1_PIO_0_U_REQ + AGVnumber, false);
        }
        #endregion

        #region AGV 작업 개시 요청    
        public static void AGVChechWorkReady_On(int AGVnumber)
        {
            DIO.WriteDOData(enumDONames.Y03_3_PIO_0_ready + AGVnumber, true);
        }
        public static void AGVChechWorkReady_Off(int AGVnumber)
        {
            DIO.WriteDOData(enumDONames.Y03_3_PIO_0_ready + AGVnumber, false);
        }
        #endregion

        #region AGV 작업 완료 확인
        public static void AGVChechWorkfinish(int AGVnumber)
        {
            if (DIO.GetDIData(enumDINames.X03_5_PIO_0_busy + AGVnumber) &&
                !DIO.GetDIData(enumDINames.X03_6_PIO_0_compt + AGVnumber) &&
                DIO.GetDIData(enumDINames.X03_7_PIO_0_cont + AGVnumber))
            {
                GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkFinish;
            }
            else if (DIO.GetDIData(enumDINames.X03_5_PIO_0_busy + AGVnumber) &&
                DIO.GetDIData(enumDINames.X03_6_PIO_0_compt + AGVnumber) &&
                DIO.GetDIData(enumDINames.X03_7_PIO_0_cont + AGVnumber))
            {
                GlobalVar.AGVStatus_0 = GlobalVar.enumAGVStage_0.WorkContinue;
            }
        }
        #endregion

        #endregion








    }
}
