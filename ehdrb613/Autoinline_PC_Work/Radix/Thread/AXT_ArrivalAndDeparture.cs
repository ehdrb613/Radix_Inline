using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Windows.Forms;

namespace Radix
{
    class AXT_ArrivalAndDeparture
    {
        int AXT_ArrivalAndDeparture_Part_Count = 0;         //부품 세트 카운트
        int AXT_ArrivalAndDeparture_Product_Count = 0;      //완료된 제품 카운트
        bool AXT_ArrivalAndDeparture_MoveSwitch = false;    //부품 공급 스위치 
        bool AXT_ArrivalAndDeparture_SupplyScan = false;    //부품 공급 스캔 해야 됨.
        int AXT_ArrivalAndDeparture_SupplyScanCount = 0;    //부품 공급 스캔 실패 카운트.
        bool AXT_ArrivalAndDeparture_DisposeScan = false;   //제품 배출 스캔 해야 됨.
        int AXT_ArrivalAndDeparture_DisposeScanCount = 0;    //제품 배출 스캔 실패 카운트.
        bool AXT_ArrivalAndDeparture_SelectAuto = false;    //셀렉터 스위치 False : Manual, True : Auto
        

        private void debug(string str)
        {
            Util.Debug(str);
        }

        public void Run()
        {
            #region stopper di change 감지 변수

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

                    #region AGV 연결

                    #region AVG 체크
                    AXT_AGVThread.AGVCheckStage(0);
                    #endregion

                    #region AGV - Docking
                    if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.Docking))
                    {
                        GlobalVar.AGVJobCodeReceive_0 = true;
                    }
                    #endregion

                    #region AGV - Load
                    else if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.Load))
                    {
                        #region 제품이 있다면 그냥 넘어감.
                        if (DIO.GetDIData(enumDINames.X01_4_Label_Guide_Up_Sensor))
                        {
                            //그냥 넘어감
                        }
                        #endregion
                        #region 제품이 없다면 제품 공급 확인
                        else//제품이 없다면
                        {
                            if (!GlobalVar.AxisStatus[1].StandStill)
                            {
                                //그냥 넘어감
                            }
                            else
                            {
                                //조건 보고 해야 된다.
                                GlobalVar.AGVReady_0 = true;
                                GlobalVar.AGV_0_Working = true;//////////확인 해야 됨.                         
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region AGV - UnLoad
                    else if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.UnLoad))
                    {
                        if (!GlobalVar.AxisStatus[0].StandStill)
                        {
                            //그냥 넘어감
                        }
                        else
                        {
                            if (DIO.GetDIData(enumDINames.X01_1_))     //언로딩 파트 제품이 있고             
                            {
                                //조건 보고 해야 된다.
                                GlobalVar.AGVReady_0 = true;
                                GlobalVar.AGV_0_Working = true;//////////확인 해야 됨.                           
                            }
                        }
                    }
                    #endregion

                    #region AGV - Complet
                    else if (GlobalVar.AGVStatus_0 == (GlobalVar.enumAGVStage_0.WorkFinish))
                    {
                        GlobalVar.AGVFront_Door_Close = true;
                    }
                    #endregion

                    #endregion                   


                    #region InConveyor

                    #region 셀렉트 스위치가 Auto 이면
                    if (DIO.GetDIData(enumDINames.X01_3_Product_Release_Check_Sensor))
                    {
                        AXT_ArrivalAndDeparture_SelectAuto = true;
                    }
                    else
                    {
                        AXT_ArrivalAndDeparture_SelectAuto = false;
                    }
                    #endregion
              
                    #region 배출부에 제품이 없다면
                    if (!DIO.GetDIData(enumDINames.X01_1_) &&
                        !GlobalVar.AGV_0_Working)
                    {
                        #region 공급부에 제품이 있다면
                        if (DIO.GetDIData(enumDINames.X01_0_) ||
                            GlobalVar.DryRun)
                        {
                            #region 제품 이송 스위치 트리거  
                            if (!AXT_ArrivalAndDeparture_MoveSwitch &&
                                DIO.GetDIChange(enumDINames.X01_2_Product_Arrival_Check_Sensor) &&
                                DIO.GetDIData(enumDINames.X01_2_Product_Arrival_Check_Sensor))
                            {
                                AXT_ArrivalAndDeparture_MoveSwitch = true;
                            }
                            #endregion

                            #region 스위치가 눌러진 상태
                            if (AXT_ArrivalAndDeparture_MoveSwitch)
                            {
                                Load_Servo_Move();
                            }
                            #endregion
                        }
                        #endregion

                        #region 공급부에 제품이 없다면
                        else
                        {
                            //향후 AGV 통신 확인 해야 함.
                            #region 셀렉트 스위치가 Auto 이면
                            if (AXT_ArrivalAndDeparture_SelectAuto)
                            {
                                if (!AXT_ArrivalAndDeparture_SupplyScan)//스캔을 하지 않아도 되면
                                {
                                    Load_Servo_Move();
                                }
                            }
                            #endregion
                            #region 셀렉트 스위치가 Manual 이면
                            else
                            {
                                //wait
                            }
                            #endregion
                        }
                        #endregion                      
                    }
                    #endregion

                    #region Load 제품스캔
                    if (AXT_ArrivalAndDeparture_SupplyScan &&
                    GlobalVar.AxisStatus[0].StandStill)
                    {
                        if (GlobalVar.DryRun)
                        {
                            AXT_ArrivalAndDeparture_SupplyScan = false;
                            AXT_ArrivalAndDeparture_Part_Count++;
                        }
                        else if (GlobalVar.Scanner.InConveyorScanner_Connect())
                        {
                            GlobalVar.Scanner.SendTrigger(true);
                            Thread.Sleep(1000);

                            if (!String.IsNullOrEmpty(GlobalVar.Load_Scanner) &&
                                GlobalVar.Load_Data_Received)
                            {
                                AXT_ArrivalAndDeparture_SupplyScanCount = 0;
                                GlobalVar.Load_Data_Received = false;
                                //MCS로 바코드 전달
                                AXT_ArrivalAndDeparture_SupplyScan = false;
                                AXT_ArrivalAndDeparture_Part_Count++;
                            }
                            else if (AXT_ArrivalAndDeparture_SupplyScanCount > 10)
                            {
                                AXT_ArrivalAndDeparture_SupplyScanCount = 0;
                                Func.AddError(enumError.LoadScanner_Scan_Error);
                                //세트로 제거 하는 것이 맞지 않나 싶다.
                                //알람 발생 후 AXT_ArrivalAndDeparture_SupplyScan 빠져 나갈지 확인 해야 됨.
                            }
                            else
                            {
                                AXT_ArrivalAndDeparture_SupplyScanCount++;
                            }
                        }
                        else
                        {
                            //알람 발생
                            Func.AddError(enumError.Box_Input_Error);
                            MessageBox.Show("스캐너 연결 되지 않음");
                        }
                    }
                    #endregion

                    #endregion

                    #region OutConveyor

                    #region 배출부에 제품이 있다면
                    if (DIO.GetDIData(enumDINames.X01_5_Label_Guide_Down_Sensor))
                    {
                        //사람이 제거 할때 까지 wait
                        //향후 AGV가 가져가는지 확인 해야 됨.
                    }
                    #endregion

                    #region 배출부에 제품이 없다면
                    else
                    {
                        #region 공급부에 제품이 있다면
                        if (DIO.GetDIData(enumDINames.X01_4_Label_Guide_Up_Sensor) &&
                            !GlobalVar.AGV_0_Working)
                        {
                            Thread.Sleep(3000);
                            UnLoad_Servo_Move();
                        }
                        #endregion                 
                    }
                    #endregion

                    #region UnLoad 제품스캔
                    if (AXT_ArrivalAndDeparture_DisposeScan &&
                        GlobalVar.AxisStatus[1].StandStill)
                    {
                        if (GlobalVar.DryRun)
                        {
                            AXT_ArrivalAndDeparture_DisposeScan = false;
                            AXT_ArrivalAndDeparture_Product_Count++;
                        }
                        else if (GlobalVar.Scanner.OutConveyorScanner_Connect())
                        {
                            GlobalVar.Scanner.SendTrigger(false);
                            Thread.Sleep(1000);
                            if (!String.IsNullOrEmpty(GlobalVar.UnLoad_Scanner))
                            {
                                //MCS로 바코드 전달
                                AXT_ArrivalAndDeparture_DisposeScan = false;
                                AXT_ArrivalAndDeparture_Product_Count++;
                            }
                            //몇번 스캔을 할지


                            if (!String.IsNullOrEmpty(GlobalVar.UnLoad_Scanner) &&
                                  GlobalVar.UnLoad_Data_Received)
                            {
                                AXT_ArrivalAndDeparture_DisposeScanCount = 0;
                                GlobalVar.UnLoad_Data_Received = false;
                                //MCS로 바코드 전달
                                AXT_ArrivalAndDeparture_DisposeScan = false;
                                AXT_ArrivalAndDeparture_Product_Count++;
                            }
                            else if (AXT_ArrivalAndDeparture_DisposeScanCount > 10)
                            {
                                AXT_ArrivalAndDeparture_DisposeScanCount = 0;
                                Func.AddError(enumError.UnLoadScanner_Scan_Error);
                            }
                            else
                            {
                                AXT_ArrivalAndDeparture_DisposeScanCount++;
                            }
                        }
                        else
                        {
                            //알람 발생
                            Func.AddError(enumError.Box_Input_Error);
                            MessageBox.Show("연결 되지 않음");
                        }
                    }
                    #endregion

                    #endregion

                }
                #endregion

                #region 초기화
                else if (GlobalVar.SystemStatus == enumSystemStatus.Initialize) // 초기화 작업일 때
                {
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


                    Thread.Sleep(1000);
                }
                #endregion

                #region 비 가동중 초기화 해야 될 체크 변수
                else // 에러 아닌 정지 상태시 시간체크값들 초기화
                {


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



        public void Load_Servo_Move()
        {
            #region 배출단에 제품이 있으면 기다림.
            if (DIO.GetDIData(enumDINames.X01_1_))
            {
                //wait
            }
            #endregion
     
            #region Servo 1 Step 전진
            else
            {
                if (!GlobalVar.AGV_0_Working)
                {
                    Ecat.MoveRelative(0, (-769774 * 100 / 39 * 3), GlobalVar.Servo_Load * GlobalVar.ServoSpeed_AXT);
                    AXT_ArrivalAndDeparture_MoveSwitch = false;
                    if (!AXT_ArrivalAndDeparture_SelectAuto)
                    {
                        AXT_ArrivalAndDeparture_SupplyScan = true;
                    }
                }
            }
            #endregion
        }

        public void UnLoad_Servo_Move()
        {
            #region 배출단에 제품이 있으면 기다림.
            if (DIO.GetDIData(enumDINames.X01_5_Label_Guide_Down_Sensor))
            {
                //wait
            }
            #endregion      
            #region Servo 1 Step 전진
            else
            {
                if (!GlobalVar.AGV_0_Working)
                {
                    Ecat.MoveRelative(1, (-78952 * 1000 / 40), GlobalVar.Servo_UnLoad * GlobalVar.ServoSpeed_AXT);

                    AXT_ArrivalAndDeparture_DisposeScan = true;
                }
            }
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
