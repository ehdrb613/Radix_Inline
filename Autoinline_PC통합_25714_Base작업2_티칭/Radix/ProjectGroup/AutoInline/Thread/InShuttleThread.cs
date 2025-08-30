using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace Radix
{

    class InShuttleThread
    {
        /*
         * InShuttleThread.cs : 투입컨베어는 스캔 동작으로 인해 다른 컨베어에 성능문제를 야기할 수 있기 때문에 독립시킴
         *                      투입 -> (스캔) -> 캐리어분리 -> 반전 -> (스캔) -> 쿨링 까지 처리
         */

        int rescanCount = 0;
        #region 파트별 동작 기억용
        FuncInline.enumShuttleAction InShuttleAction = FuncInline.enumShuttleAction.Waiting;
        #endregion

        private void debug(string str)
        {
            Util.Debug("InShuttleThread : " + str);
        }


        public void Run()
        {
            enumSystemStatus beforeStatus = GlobalVar.SystemStatus; // 이전 상태값 기억

            while (!GlobalVar.GlobalStop)
            {
                try
                {
                    #region 사이트 action 변화 로깅
                    #region In Shuttle
                    if (InShuttleAction != InShuttleAction)
                    {
                        FuncLog.WriteLog_Tester("InShuttle Action Change ==> " + FuncInline.InShuttleAction.ToString());
                    }
                    InShuttleAction = FuncInline.InShuttleAction;
                    #endregion
                    #endregion

                    #region 컨베어 동작들. 

                    if (!GlobalVar.E_Stop &&
                        !GlobalVar.DoorOpen &&
                        GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                    {
                        #region PCB 보유 정보와 센서 정보 불일치시 에러 처리
                        //if (!FuncInline.PassMode)
                        //{
                        if (FuncInline.InShuttleAction == FuncInline.enumShuttleAction.Waiting &&
                            !DIO.GetDORead((int)FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw) &&
                            !DIO.GetDORead((int)FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw))
                        {
                            #region 정보 없는데 센서 감지되면 Input으로 연결
                            if (//!FuncInline.LiftPCBCheck &&
                                FuncInline.InShuttleAction == FuncInline.enumShuttleAction.Waiting &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                (DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor)))
                            {
                                if (//FuncInline.LiftPCBCheck &&
                                    !FuncInline.InputPCB) // &&
                                                          //!DIO.GetDIData(FuncInline.enumDINames.X02_1_SMEMA_Before_Ready) ||
                                                          //        !DIO.GetDORead((int)FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready)))
                                {
                                    if (!FuncError.CheckError(FuncInline.enumErrorPart.InShuttle, FuncInline.enumErrorCode.PCB_Info_Not_Exist))
                                    {
                                        FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                                                        DateTime.Now.ToString("HH:mm:ss"),
                                                                        FuncInline.enumErrorPart.InShuttle,
                                                                        FuncInline.enumErrorCode.PCB_Info_Not_Exist,
                                                                        false,
                                                                        "PCB detected. But no PCB Information."));
                                        Thread.Sleep(GlobalVar.ThreadSleep);
                                        continue;
                                    }
                                    FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                                    Thread.Sleep(GlobalVar.ThreadSleep);
                                    continue;
                                }
                                else
                                {
                                    // InputPCB 일회성 수동투입
                                    FuncInline.InputPCB = false;
                                    Util.StartWatch(ref FuncInline.InShuttleActionWatch);
                                    FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Input;
                                }
                            }
                            #endregion
                            #region 정보 있는데 센서 감지 안 되면 에러 처리
                            if (FuncInline.InShuttleAction == FuncInline.enumShuttleAction.Waiting &&
                                FuncInline.FrontPassLineAction != FuncInline.enumLiftAction.Input &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                                !DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) &&
                                !DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                                !DIO.GetDORead((int)FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw) &&
                                !DIO.GetDORead((int)FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw) &&
                                !FuncError.CheckError(FuncInline.enumErrorPart.InShuttle, FuncInline.enumErrorCode.PCB_Detect_Fail))
                            {
                                FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                                                DateTime.Now.ToString("HH:mm:ss"),
                                                                FuncInline.enumErrorPart.InShuttle,
                                                                FuncInline.enumErrorCode.PCB_Detect_Fail,
                                                                false,
                                                                "PCB detect failed. Check PCB Information."));
                                Thread.Sleep(GlobalVar.ThreadSleep);
                                continue;
                            }
                            #endregion
                        }
                        #endregion

                        #region In Shuttle
                        switch (InShuttleAction)
                        {
                            #region Input 전 장비에서 투입
                            case FuncInline.enumShuttleAction.Input:
                                if (InShuttleInput())
                                {
                                    //GlobalVar.InputLiftInputChecked = false; // 투입센서 감지되면 true, 종단에 도착하면 false.  True 동안 smema 보내면 안 된다.
                                    Util.ResetWatch(ref FuncInline.InShuttleActionWatch);
                                    DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
                                    DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);

                                    // 부가파트의 액션은 일단 모두 삭제

                                    Util.StartWatch(ref FuncInline.InShuttleActionWatch);
                                    FuncInline.CoolingFinish = false;

                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].TestPass = DIO.GetDIData(FuncInline.enumDINames.X00_6_SMEMA_Before_Pass) ||
                                                                                                            FuncInline.PassMode; // pass 여부 수신

                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus = FuncInline.enumSMDStatus.Before_Scan;
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].Carrier = FuncInline.CarrierSeparation; // 캐리어 분리 사용시 캐리어 정보를 남겨놔야 캐리어만 이송할 수 있다.
                                    rescanCount = 0;

                                    FuncInline.ResetInOutTime();

                                    #region if Pass할 PCB면 종료
                                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].TestPass)
                                    {
                                        //debug("Input if Pass할 PCB면 종료");
                                        //GlobalVar.InputLiftAction = FuncInline.enumLiftAction.Waiting;
                                        if (GlobalVar.SMDLog &&
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.No_Test)
                                        {
                                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo InputLift .PCBStatus = FuncInline.enumSMDStatus.No_Test");
                                        }
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus = FuncInline.enumSMDStatus.No_Test;
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].NgType = FuncInline.enumNGType.OK;
                                        Util.ResetWatch(ref FuncInline.InShuttleActionWatch);
                                        DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
                                        DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                                    }
                                    #endregion
                                    #region else Pass할 PCB 아니면 스캔 동작으로 연결
                                    else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                                    {
                                  
                                        if (GlobalVar.SMDLog &&
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.Before_Scan)
                                        {
                                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo InputLift .PCBStatus = FuncInline.enumSMDStatus.Before_Scan");
                                        }
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus = FuncInline.enumSMDStatus.Before_Scan;
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].NgType = FuncInline.enumNGType.OK;
                                    }
                                    #endregion


                                    #region PassMode DataBase 투입 수량 증가
                                    if (FuncInline.PassMode)
                                    {
                                        FuncInline.IncreasePCBCount(DateTime.Now.ToString("yyyyMMdd"),
                                                                    DateTime.Now.ToString("HH:mm:ss"),
                                                                    true, false, false, false);
                                    }
                                    #endregion

                                    //PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].CycleStartTime = Environment.TickCount;

                                    #region buyer change. White더라도 PC별로 Orange 유지되는 경우는 사이트 투입시 지정해야 한다.
                                    //if ((FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue ||
                                    //    FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange))
                                    //{
                                    //    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].BuyerChange = true;
                                    //}
                                    #endregion

                                    // PassMode는 Waiting으로 해서 바로 이송 준비하도록 한다.
                                    if (FuncInline.PassMode)
                                    {
                                        FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                                    }
                                    else
                                    {
                                        if (FuncInline.USECooling)
                                        {
                                            Util.StartWatch(ref FuncInline.InShuttleActionWatch);
                                            FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Cooling;
                                        }
                                        else
                                        {
                                            Util.StartWatch(ref FuncInline.InShuttleActionWatch);
                                            FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                                        }
                                      
                                     
                                    }
                                    //Thread.Sleep(300);
                                }
                                break;
                            #endregion


                            #region 쿨링
                            case FuncInline.enumShuttleAction.Cooling:
                                if (!FuncInline.CoolingFinish)
                                {
                                    // PassMode 통과
                                    if (FuncInline.PassMode)
                                    {
                                        FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                                        FuncInline.CoolingFinish = true;
                                    }
                                    // 쿨링 선택하지 않는 경우 통과
                                    if (!FuncInline.CoolingByTime &&
                                        !FuncInline.CoolingByTemperature)
                                    {
                                        FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                                        FuncInline.CoolingFinish = true;
                                    }
                                    bool tempOK = true;
                                    if (FuncInline.CoolingByTemperature &&
                                        (int)FuncInline.Temperature_NS10LT_PV > FuncInline.CoolingTemperature)
                                    {
                                        tempOK = false;
                                    }
                                    bool timeOK = true;
                                    if (FuncInline.CoolingByTime &&
                                        (FuncInline.CoolingWatch == null ||
                                                FuncInline.CoolingWatch.ElapsedMilliseconds < FuncInline.CoolingTime * 1000))
                                    {
                                        if (FuncInline.CoolingWatch == null ||
                                            !FuncInline.CoolingWatch.IsRunning)
                                        {
                                            Util.StartWatch(ref FuncInline.CoolingWatch);
                                        }
                                        timeOK = false;
                                    }
                                    if (tempOK &&
                                        timeOK)
                                    {
                                        FuncInline.CoolingFinish = true;
                                    }
                                    if (FuncInline.CoolingByTemperature &&
                                        FuncInline.CoolingWatch.IsRunning &&
                                        FuncInline.CoolingWatch.ElapsedMilliseconds >= FuncInline.CoolingMaxTime * 1000)
                                    {
                                        FuncInline.CoolingWatch.Stop();
                                        FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                                                        DateTime.Now.ToString("HH:mm:ss"),
                                                                        FuncInline.enumErrorPart.InShuttle,
                                                                        FuncInline.enumErrorCode.PCB_Temperature_Over,
                                                                        false,
                                                                        "PCB Temperature is too high"));
                                        Thread.Sleep(GlobalVar.ThreadSleep);
                                        continue;
                                    }
                                }
                                if (FuncInline.CoolingFinish)
                                {
                                    //debug("cooling end");

                                    Util.StartWatch(ref FuncInline.InShuttleActionWatch);
                                    FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                                }
                                break;
                            #endregion
                            #region 동작 없을 때
                            case FuncInline.enumShuttleAction.Waiting:                             

                                // 쿨링 미완료면 쿨링으로
                                if (FuncInline.USECooling &&
                                    !FuncInline.PassMode &&
                                    (DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor)) &&
                                    !FuncInline.CoolingFinish)
                                {
                                    Util.StartWatch(ref FuncInline.InShuttleActionWatch);
                                    Util.StartWatch(ref FuncInline.CoolingWatch);
                                    FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Cooling;
                                }
                                break;
                                #endregion
                        }
                        #endregion

                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    //debug(ex.ToString());
                    //debug(ex.StackTrace);
                }
                beforeStatus = GlobalVar.SystemStatus;

                Thread.Sleep(GlobalVar.ThreadSleep);

            }


        }

        // 전 장비에서 투입컨베어로 수령
        public bool InShuttleInput()
        {
            try
            {
                FuncInline.ResetInOutTime();

                #region PCB 정보 있으면 액션 삭제 초기화?
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    //debug("InputConveyorInput PCB 정보 있으면 액션 삭제");
                    Util.ResetWatch(ref FuncInline.InShuttleActionWatch);
                    FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                    DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                    return false;
                }
                #endregion
                FuncInline.ResetInOutTime();
                DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, !FuncInline.CycleStop &&
                                                                        FuncInline.BuyerChange != FuncInline.enumBuyerChange.Yellow &&
                                                                        //!GlobalVar.InputLiftInputChecked &&
                                                                        FuncInlineMove.CheckNearPos(FuncInline.enumPMCAxis.ST00_InShuttle_Width, false) &&
                                                                        !DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                                                                        DIO.GetDIData(FuncInline.enumDINames.X02_1_SMEMA_Before_Ready)); // 스메마 출력 가동

                string fail_str = "";
                #region else 수령 위치로 이동 되면

                #region if 우측 스토퍼 안 올라가 있으면 스토퍼 상승
                if (!DIO.GetDIData(FuncInline.enumDINames.X302_2_In_Shuttle_Stopper_Cyl_Up_Sensor))  // 스토퍼 안 올라가 있으면 스토퍼 상승
                {
                    //debug("Input if 우측 스토퍼 안 올라가 있으면 스토퍼 상승");
                    fail_str = "Right Stopper Up";
                    DIO.WriteDOData(FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL, false);
                }
                #endregion if 스토퍼 안 올라가 있으면 스토퍼 상승

                #region else if 스메마 입력되면 컨베어 가동
                else if (//(FuncInline.InputPCB || DIO.GetDIData(FuncInline.enumDINames.X02_1_SMEMA_Before_Ready)) &&
                    !DIO.GetDORead((int)FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw))  // 스메마 입력되면 컨베어 가동
                {
                    //debug("Input if 스메마 입력되면 컨베어 가동");
                    fail_str = "Conveyor Run";
                    DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);
                }
                #endregion else if 스메마 입력되면 컨베어 가동

                #region else if 시작/중간 센서 감지되고 최종 센서 확인 안 되면 컨베어 가동
                /*
                else if (DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) &&
                    !DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                    !DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                    !DIO.GetDORead((int)FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw)) // 시작/중간 센서 감지되고 최종 센서 확인 안 되면 컨베어 가동
                {
                    //debug("Input else if 시작/중간 센서 감지되고 최종 센서 확인 안 되면 컨베어 가동");
                    DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);
                }
                //*/
                #endregion else if 시작/중간 센서 감지되고 최종 센서 확인 안 되면 컨베어 가동

                #region else if 최종 센서 확인되면 투입 종료
                else if (!DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) &&
                    (DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor)))
                {
                    //debug("Input if 최종 센서 확인되면 투입 종료");

                    if (FuncInline.PassMode) //  패스모드일 경우 투입수량 증가. 패스 아니면 로봇이 투입시
                    {
                        FuncInline.PCBInputCount++;
                    }

                    return true;
                }
              
                #endregion else if 최종 센서 확인되면 투입 종료 - Scan으로 변경

                #endregion else 수령 위치면

                // 타임아웃 처리
                if ((FuncInline.InShuttleActionWatch.IsRunning &&
                            FuncInline.InShuttleActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000 * (GlobalVar.Simulation ? GlobalVar.Simulation_Timeout : 1)))
                {
                    //debug("time : " + FuncInline.Lift2ActionWatch.ElapsedMilliseconds);
                    FuncInline.InShuttleActionWatch.Stop();
                    FuncInline.InShuttleActionWatch.Reset();
                    DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw, false);

                    FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                                DateTime.Now.ToString("HH:mm:ss"),
                                                FuncInline.enumErrorPart.InShuttle,
                                                FuncInline.enumErrorCode.Conveyor_Timeout,
                                                false,
                                                "InShuttle input action timed out. " + fail_str));

                    return false;
                }


            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

            return false;
        }


        // 투입셔틀에 PCB 위치 다르면 정지센서로 이동
        private bool MoveInShuttlePCBPos()
        {
            // 반전기 반전 위치면 완료
            if (DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                !DIO.GetDORead(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw) &&
                !DIO.GetDORead(FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw))
            {
                //debug("CheckInShuttlePCBPos 정지센서 감지되면 완료");
                return true;
            }

            string fail_str = "";
            // 셔틀 스토퍼 올려져 있지 않으면 올리기
            if (!DIO.GetDIData(FuncInline.enumDINames.X302_2_In_Shuttle_Stopper_Cyl_Up_Sensor))
            {
                //debug("CheckInShuttlePCBPos 셔틀 스토퍼 올려져 있지 않으면 올리기");
                DIO.WriteDOData((int)FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL, false);
                fail_str = "Stopper Up";
            }
            // 정지센서 감지 안 되고 컨베어 가동 아니면 가동
            else if (!DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                (!DIO.GetDORead(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw) ||
                        DIO.GetDORead(FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw)))
            {
                //debug("CheckInShuttlePCBPos 정지센서 감지 안 되고 컨베어 가동 아니면 가동");
                DIO.WriteDOData((int)FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);
                DIO.WriteDOData((int)FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw, false);
                fail_str = "Conveyor Run";
            }
            // 정지센서 감지 되고 컨베어 가동중이면 정지
            else if (DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                (DIO.GetDORead(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw) ||
                        DIO.GetDORead(FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw)))
            {
                //debug("CheckInShuttlePCBPos 정지센서 감지 되고 컨베어 가동중이면 정지");
                DIO.WriteDOData((int)FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                DIO.WriteDOData((int)FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw, false);
                fail_str = "PCB Detect";
            }

            // 타임아웃 처리
            if ((FuncInline.InShuttleActionWatch.IsRunning &&
                        FuncInline.InShuttleActionWatch.ElapsedMilliseconds > FuncInline.ConveyorTimeout * 1000 * (GlobalVar.Simulation ? GlobalVar.Simulation_Timeout : 1)))
            {
                //debug("time : " + FuncInline.Lift2ActionWatch.ElapsedMilliseconds);
                FuncInline.InShuttleActionWatch.Stop();
                FuncInline.InShuttleActionWatch.Reset();
                DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                DIO.WriteDOData(FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw, false);

                FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                            DateTime.Now.ToString("HH:mm:ss"),
                                            FuncInline.enumErrorPart.InShuttle,
                                            FuncInline.enumErrorCode.Conveyor_Timeout,
                                            false,
                                            "InShuttle action timed out. " + fail_str));
                return false;
            }

            return false;
        }



    }
}
