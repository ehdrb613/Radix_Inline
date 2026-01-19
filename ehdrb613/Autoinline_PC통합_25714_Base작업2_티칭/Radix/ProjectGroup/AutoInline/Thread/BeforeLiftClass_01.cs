using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Radix
{
    /**
     * @brief 작업 전 트레이 리프트 클래스
     *        작업 전 임플란트 트레이를 투입한다.
     *        제어 및 상태값, 선언, 쓰레드 등을 모두 포함
     */
    class BeforeLiftClass
    {
        #region type 선언
        #region enum
        /**
         * @brief 동작 구분
         */
        public enum enumAction
        {
            Waiting, // 아무 동작 없을 때, 다음 동작을 어떤걸 해야할지 체크하는 부분
            Init, // 초기화
            InitFinish, // 초기화 완료
            Skip,       // 제품없을때 스킵
            NotUse,     // 사용 안할때
            CycleStop,  // 사이클스탑일때

            HomeMove, //에러 발생 후 복귀 동작 후 -> Waiting으로

            EmptytrayReady, //빈트레이가 없을시 공급 준비 항상 1층은 빈트레이 투입하도록 지정함
            EmptytrayReadyCheck,    //빈트레이 공급되어있는지 확인
            InPutWait,   //Lift 하강 후 Tray 공급대기 상태(작업중인 트레이가 있거나, 배출중일때)
            InPutCnyRun, //트레이 투입동작
            InPutCnyRunCheck,   //트레이 투입동작 완료 확인
            LiftDown,       //리프트 하강 동작(제품 트레이 공급 동작)
            LiftDownCheck, //리프트 하강 체크
            LiftReadyPos,    //10층 마지막 투입완료 후에 다시 작업자 공급 위치로 올려준다.
            LiftUp,    //유효 층이 아닐경우 한층 업하고 다시 하강
        }
        #endregion
        #region struct
        #endregion
        #endregion

        #region 변수
        #region Thread 처리용
        /** @brief 동작 처리 쓰레드. */
        public Thread actionThread { get; set; }
        /** @brief 클래스 만료중 체크 */
        public bool ClassDisposing = false;
        #endregion
        #region 동작 설정용
        public double ThreadSleep = 100; // 쓰레드 동작 속도, 클래스 초기화 후 메인에서 설정값을 지정할 것
        public double ActionTimeout = 20 * 1000; // 타임아웃 처리 시간. 클래스 초기화 후 메인에서 설정값을 지정할 것
        #endregion
        /** @brief 쓰레드의 동작 단계 */
        public enumAction Action = enumAction.Waiting;
        /** @brief 쓰레드의 이전 동작 단계 */
        private enumAction beforeAction = enumAction.Waiting;
        /** @brief 시스템의 이전 상태 */
        private enumSystemStatus beforeSystemStatus = GlobalVar.SystemStatus;

        /** @brief 동작 수행시 타임아웃 체크 */
        private Stopwatch watch = new Stopwatch();
        /** @brief 한 공정 완료 여부. 각 하부 Part별로 완료여부 체크되면 컨베어 움직이고, 컨베어 움직이기 시작하면 완료여부 clear 하면 된다. */
        public bool StepFinish = false;

        /** @brief 현재 공정에서 작업중인 모델정보 */
        public string NowModel = "";
        public int SV00_Input_Tray_Lift = (int)FuncInline.enumServoAxis.SV00_In_Shuttle;

      
        /** @brief 1층 트레이 위치 */
        public double ready_pos = 0;
        /** @brief 작업자 트레이 투입 위치 */
        public double input_pos = 0;

        // 클래스 멤버나 외부에서 선언되어 있어야 함 (값 유지용)
        public int currentFloor = -1; // 초기값은 -1로, 아직 어떤 층에도 도달하지 않았음을 의미
        public int nearestFloor = -1;   //제대로된 위치가 아닐때 가장 가까운 위치를 찾음
        public int NextFloor = -1; // 다음 이동할 위치 지정변수

        const double tolerance = 1.0; // 허용 오차(mm)

        public bool trayEmptytrayOutCheck;
        //컨베이어 동작
        bool Y01_0_Before_Tray_Lift_Conveyor_CW = false;
        //리프트 트레이 감지 센서
        bool X01_0_Before_Tray_Input_Lift_End_Sensor = false;

        /** @brief 타임아웃 체크할때 어디서 문제 생겼는지 내용 저장용 */
        //에러 내용 저장용, 타임
        public string Log = "";

        //중복로그 방지용 플레그
        private bool isLogWritten = false; 

        //서보init 완료시 true 시작시 false
        private bool InitServo = false;
        #endregion

        /** @brief 생성자 */
        public BeforeLiftClass()
        {

            // 쓰레드를 시작한다
            actionThread = new Thread(ActionThread);
            actionThread.Start();
        }

        /** @brief 소멸자 */
        ~BeforeLiftClass()
        {
            ClassDisposing = true;
        }

        private void debug(string str)
        {
            Util.Debug("BeforeLiftClass : " + str);
        }

        /** @brief 동작 처리 쓰레드 */
        private void ActionThread()
        {
            while (!GlobalVar.GlobalStop &&
                !ClassDisposing)
            {
                try
                {
                    #region 상시 체크할 부분

              

                    #endregion

                    #region 시스템 상태 따라
                    switch (Action)
                    {
                        case enumAction.Waiting:
                            #region Case Waiting
                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                            {
                                if (FuncInline.CycleStop == true)
                                {
                                    Log = "[#1 샌딩 전 리프트] CycleStop 지령";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.CycleStop;

                                }

                                //동작해야 할때
                                else if (!StepFinish)
                                {
                               

                                }
                                //완료상태일때 대기
                                else
                                {
                                    //StepFinish = True일땐 대기 
                                }

                            }
                            Util.InitWatch(ref watch);
                            break;
                        #endregion

                        case enumAction.Skip:
                            #region Case Skip
                        

                            break;
                        #endregion

                        case enumAction.NotUse:
                        case enumAction.CycleStop:
                            #region Case NotUse/Case CycleStop
                            //NotUse 풀리면 다시 Waiting으로

                            if (FuncInline.CycleStop == false)
                            {
                                Log = $"[#0 샌딩 전 리프트] {Enum.GetName(typeof(enumAction), Action)} -> Waiting";
                                FuncLog.WriteLog(Log);
                                Action = enumAction.Waiting;
                                break;
                            }
                         
                            Util.InitWatch(ref watch);
                            break;
                        #endregion
                        case enumAction.Init:
                            #region Case Init

                            if (Y01_0_Before_Tray_Lift_Conveyor_CW)
                            {
                                Log = "[#1 샌딩 전 리프트] 초기화 - 컨베이어 정지";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(FuncInline.enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder, false);
                            }

                          


                            // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행
                            if (GlobalVar.AxisStatus[SV00_Input_Tray_Lift].isHomed &&
                                 FuncInlineMove.IsArrived(SV00_Input_Tray_Lift, input_pos))
                            {
                                if (InitServo == false)
                                {
                                    InitServo = true;
                                    Log = "[#1 샌딩 전 리프트] 초기화 - 서보 원점복귀 후 준비위치 완료";
                                    FuncLog.WriteLog(Log);
                                }

                             
                                Log = "[#1 샌딩 전 리프트] 초기화 완료";
                                FuncLog.WriteLog(Log);
                                // 모든 실린더 후진 확인 되면 완료
                                Action = enumAction.InitFinish;
                              
                              
                            }
                            else
                            {
                                if (GlobalVar.AxisStatus[SV00_Input_Tray_Lift].StandStill &&
                                    GlobalVar.AxisStatus[SV00_Input_Tray_Lift].isHomed)
                                {
                                    Log = "[#1 샌딩 전 리프트] 초기화 - SV01_LowBush_Z 상승 위치 동작";
                                    FuncLog.WriteLog(Log);
                                    InitServo = false; //준비위치 지령 날릴때 false 처리 완료 후 true
                                    FuncInlineMove.MoveAbsMM(SV00_Input_Tray_Lift, input_pos);
                                }
                                else
                                {
                                    if (GlobalVar.AxisStatus[SV00_Input_Tray_Lift].StandStill &&
                                       !GlobalVar.AxisStatus[SV00_Input_Tray_Lift].Homing)
                                    {
                                        Log = "[#1 샌딩 전 리프트] 초기화 - SV01_LowBush_Z 원점복귀 동작";
                                        FuncLog.WriteLog(Log);
                                        FuncMotion.MoveHome((uint)SV00_Input_Tray_Lift);
                                    }
                                    Log = "[#1 샌딩 전 리프트] 초기화 - SV01_LowBush_Z 원점복귀 /상승 위치 확인 안됨";
                                }

                            }

                            //호밍중에 센서 감지되면 바로 정지 지령(따로 알람있음 안전문제)
                            if (X01_0_Before_Tray_Input_Lift_End_Sensor)
                            {
                                FuncMotion.MoveStop(SV00_Input_Tray_Lift); //정지상태 되면 서보 정지
                            }

                            Util.ResetWatch(ref watch);
                            break;
                        #endregion
                        case enumAction.InitFinish:
                            #region Case InitFinish
                            // 원점 동작 완료
                            // Main Control Thread 에서 전체 초기화 체크 후 Waiting으로 변경한다.
                            StepFinish = false; //false 되야 동작 시작
                            Util.ResetWatch(ref watch);
                            break;
                            #endregion
                    }
                    #endregion


                    #region 동작 진행

                    #region if 시스템 변경되었을때
                    if (beforeSystemStatus != GlobalVar.SystemStatus &&
                     GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
                    {
                        FuncMotion.MoveStop(SV00_Input_Tray_Lift); //정지상태 되면 서보 정지
                        Util.InitWatch(ref watch);
                    }
                    #endregion

                    #region if AutoRun
                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                    {
                        if (watch.ElapsedMilliseconds > ActionTimeout)
                        {
                            FuncInline.AddError(FuncInline.enumErrorPart.InShuttle,
                                FuncInline.enumErrorCode.Conveyor_Timeout,
                                "샌딩 전 리프트 타임아웃 에러가 발생했습니다.\n" +
                                       $"{Log} \n 간섭 또는 동작 순서가 맞지 않는 부위를 확인 후 재시작하세요.");
                            //Action = enumAction.HomeMove;   //에러 발생시 준비위치로 이동 후 Wait
                            Util.InitWatch(ref watch);
                            continue;
                        }
                        switch (Action)
                        {

                            case enumAction.HomeMove: //에러 발생 후 복귀 동작 후 -> Waiting으로
                                #region HomeMove
                                // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행

                                #endregion
                                break;
                        

                        }
                    }
                    #endregion

                    #region else AutoRun 아닐때
                    //AutoRun이 아닐때 대기
                    else
                    {
                        Util.InitWatch(ref watch);
                    }
                    #endregion

                    #endregion

                    #region 상시 체크할 부분
                    #region 타임아웃 설정
                    if (watch == null ||
                         !watch.IsRunning)
                    {
                        if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                        {
                            Util.StartWatch(ref watch);
                        }
                        else
                        {
                            watch.Stop();
                        }

                    }
                    if (beforeAction != Action)
                    {
                        debug("action change " + beforeAction.ToString() + " ==> " + Action.ToString());
                        Util.ResetWatch(ref watch);
                    }
                    beforeAction = Action;
                    beforeSystemStatus = GlobalVar.SystemStatus;
                    #endregion
                    #endregion
                }
                catch (Exception ex)
                {
                    FuncLog.WriteLog("BeforeLiftClass.ActionThread : " + ex.ToString());
                    FuncLog.WriteLog("BeforeLiftClass.ActionThread : " + ex.StackTrace);
                }

                Thread.Sleep(GlobalVar.ThreadSleep);
            }
        }
        private void StepFinish_Send()
        {   
           
            StepFinish = true;  //완료했으면 True
        }

    }
}
