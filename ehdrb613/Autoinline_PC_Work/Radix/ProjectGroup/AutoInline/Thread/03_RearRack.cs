using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using static Radix.FuncInline;

namespace Radix
{
    /**
     * @brief Before 작업위치 클래스
     *        작업전 임플란트를 샌딩기에 투입한다.
     *        제어 및 상태값, 선언, 쓰레드 등을 모두 포함
     */
    class RearRack
    {
        #region type 선언
        #region enum
        /**
         * @brief 동작 구분
         */
        public enum enumAction
        {
            Waiting, // 아무 동작 없을 때
            Init, // 초기화
            InitFinish, // 초기화 완료
            Skip,       // 제품없을때 스킵
            NotUse,     // 사용 안할때
            CycleStop,  // 사이클스탑일때

            HomeMove, //에러 발생 후 복귀 동작 후 -> Waiting으로

            MoveLift,   //각 사이트,OKline,NGLine,InShuttle위치 이동
            MoveSuttlePos,
            MoveScanPos,
            MoveOKline,
            MoveNGline,

            ShuttleLoading,
            ShuttleLoadingCheck,
            Loading,   //테스트 완료된 사이트, 스캔 완료된 Scan사이트로 부터 로딩
            LoadingCheck,
            UnLoading,  //사이트,Scan,Passline 사이트로 언로딩,OutShuttle로 언로딩
            UnLoadingCheck,
            ScanWait,    //Rear는 Lift에서 Scan한다
            ScanOK
        }
        public enum enumOKLineAction
        {
            Waiting, // 아무 동작 없을 때
            Loading,   //Inshuttle로 부터 로딩
            LoadingCheck,
            UnLoading,  //FrontLift로 언로딩
            UnLoadingCheck
        }
        public enum enumNGLineAction
        {
            Waiting, // 아무 동작 없을 때
            Loading,   //Inshuttle로 부터 로딩
            LoadingCheck,
            UnLoading,  //FrontLift로 언로딩
            UnLoadingCheck
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
        public double ActionTimeout = FuncInline.ConveyorTimeout * 1000; // 타임아웃 처리 시간. 클래스 초기화 후 메인에서 설정값을 지정할 것
        #endregion
        /** @brief 쓰레드의 동작 단계 */
        public enumAction Action = enumAction.Waiting;
        /** @brief 쓰레드의 동작 단계 */
        public enumOKLineAction OKLineAction = enumOKLineAction.Waiting;
        /** @brief 쓰레드의 동작 단계 */
        public enumNGLineAction NGLineAction = enumNGLineAction.Waiting;
        /** @brief 쓰레드의 이전 동작 단계 */
        private enumAction beforeAction = enumAction.Waiting;
        /** @brief 시스템의 이전 상태 */
        private enumSystemStatus beforeSystemStatus = GlobalVar.SystemStatus;

        public bool ScanReady = false;  //Rear 스캔 준비

        /** @brief 동작 수행시 타임아웃 체크 */
        private Stopwatch watch = new Stopwatch();
        //로딩 스텝 및 딜레이 타이머
        private int loadingStep = 0;
        public int LastRearFTIndex = -1; // FT 사이트 순차 투입용 인덱스

        private Stopwatch delayWatch = new Stopwatch();
        /** @brief 한 공정 완료 여부. 각 하부 Part별로 완료여부 체크되면 컨베어 움직이고, 컨베어 움직이기 시작하면 완료여부 clear 하면 된다. */
        public bool StepFinish = false;

        /** @brief 현재 공정에서 작업중인 모델정보 */
        public string NowModel = "";
        public int SV04_Lift2 = (int)FuncInline.enumServoAxis.SV04_Lift2;
        public int SV05_Rack2_Width = (int)FuncInline.enumServoAxis.SV05_Rack2_Width;
        public int Rear = (int)enumLiftName.RearLift;

        private const string Key_OKLine_Load = "OKLine_Load";
        private const string Key_OKLine_Unload = "OkLine_Unload";
        private const string Key_Lift_Load = "RearLift_Load";
        private const string Key_Lift_Unload = "RearLift_Unload";

        private const string Key_NGLine_Load = "NGLine_Load";
        private const string Key_NGLine_Unload = "NGLine_Unload";
        private const string Key_Scan_delay = "Scan_delay";

        static enumTeachingPos dest = enumTeachingPos.None;

        //SiteAction 인덱스
        int siteIdxLocal = (int)dest - (int)enumTeachingPos.Site1_F_DT1;
        //SiteLiftPos 인덱스
        int sitePosindex = (int)dest - (int)enumTeachingPos.Site14_R_DT1 + (int)enumLiftPos.Site14_R_DT1_Up;

        #region 변수 선언부

        // =============================================================
        // [상태 변수] Rear Rack (Site 14~26)
        // =============================================================
        public static bool[] Rear_PCB_Sensor = new bool[13];
        public static bool[] Rear_ClampSol = new bool[13];
        public static bool[] Rear_DownSol = new bool[13];
        public static bool[] Rear_Up_Sensor = new bool[13];
        public static bool[,] Rear_Motor = new bool[13, 2];

        // =============================================================
        // [ETC] Rear Lift 및 Passline 상태
        // =============================================================
        // 리프트 관련 센서
        public static bool RLift_IN_Stopper = false;     // 스토퍼 실린더 상승(OUT) 
        public static bool RLift_OUT_Stopper = false;     // 스토퍼 실린더 상승(OUT)
        public static bool RLift_IN_UpStopper_Sensor = false;     // 스토퍼 실린더 상승(IN) 센서
        public static bool RLift_Out_UpStopper_Sensor = false;     // 스토퍼 실린더 상승(OUT) 센서

        public bool RLift_UpPCB_IN_Sensor = false;      // 리프트 UP 위치 PCB 진입
        public bool RLift_UpPCB_Stop_Sensor = false;    // 리프트 UP 위치 PCB 정지

        public static bool RLift_DownPCB_IN_Sensor = false;    // 리프트 DOWN 위치 PCB 진입
        public static bool RLift_DownPCB_Stop_Sensor = false;  // 리프트 DOWN 위치 PCB 정지

        // Passline (OK/NG Line) 관련 센서
        public static bool ROKLine_Stopper = false;   //Rear OK PassLine 스토퍼
        public static bool RNGLine_Stopper = false;   //Rear NG PassLine 스토퍼
        public bool Rear_Pass_OkLine_PCB_In_Sensor = false;
        public bool Rear_Pass_NgLine_PCB_Stop_Sensor = false;

        public static bool Rear_Rack_PCB_Interlock_Sensor = false;

        // 리프트 및 패스라인 모터 상태 [1, 2] (CW, CCW)
        public static bool[,] RLift_UPMotor = new bool[1, 2];   // 리프트 상승 모터
        public static bool[,] RLift_DownMotor = new bool[1, 2]; // 리프트 하강 모터
        public static bool ROKLine_Motor = false; // 패스라인 모터
        public static bool RNgLine_Motor = false;   // NG 라인 모터



        public string Name = "";



        #endregion

        /** @brief 타임아웃 체크할때 어디서 문제 생겼는지 내용 저장용 */
        //에러 내용 저장용, 타임
        public string Log = "";

        //중복로그 방지용 플레그
        private bool isLogWritten = false;

        //서보init 완료시 true 시작시 false
        private bool InitServo = false;
        #endregion

        /** @brief 생성자 */
        public RearRack()
        {

            // 쓰레드를 시작한다
            actionThread = new Thread(ActionThread);
            actionThread.Start();

            Name = "[Rear Rack]"; // 영문 로그 이름 설정

        }

        /** @brief 소멸자 */
        ~RearRack()
        {
            ClassDisposing = true;
        }

        private void debug(string str)
        {
            Util.Debug($"{Name} : " + str);
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
                    ActionTimeout = FuncInline.ConveyorTimeout * 1000;

                    UpdateRearAllStatus();
                    UpdateRearETCStatus();

                    enumSMDStatus LiftStatus = FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus;
                    enumSMDStatus PassLineStatus = FuncInline.PCBInfo[(int)enumTeachingPos.RearPassLine].PCBStatus;
                    enumSMDStatus NGLineStatus = FuncInline.PCBInfo[(int)enumTeachingPos.RearNGLine].PCBStatus;

                    if (PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination != enumTeachingPos.None)
                    {
                        dest = PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination;
                        //SiteAction 인덱스
                        siteIdxLocal = (int)dest - (int)enumTeachingPos.Site1_F_DT1;
                        //SiteLiftPos 인덱스
                        sitePosindex = (int)dest - (int)enumTeachingPos.Site14_R_DT1 + (int)enumLiftPos.Site14_R_DT1_Up;
                    }
                

                    double targetPos = -9999;

                    int PcbLiftID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Num;
                    String logPcbLiftID = $"[PCB_ID:{PcbLiftID}]";
                    int PcbOKLineID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].Num;
                    String logOKLineID = $"[PCB_ID:{PcbOKLineID}]";
                    int PcbNGLineID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].Num;
                    String logNGLineID = $"[PCB_ID:{PcbNGLineID}]";
                    #endregion

                    #region 시스템 상태 따라
                    switch (Action)
                    {
                        case enumAction.Waiting:
                            if(GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                            {
                                #region Waiting (Decision)
                                // =============================================================
                                // [CASE 1] 리프트가 비어있을 때 (제품을 가져오는 동작 우선)
                                // =============================================================
                                if (LiftStatus == enumSMDStatus.UnKnown)
                                {
                                    // 1. InShuttle 입고 상태 확인 (Rear 쪽으로 오는지 확인)
                                    bool isIncomingFromShuttle = FuncInline.PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus != enumSMDStatus.UnKnown &&
                                                                 (AutoInline.Class.InShuttle.Action == InShuttle.enumAction.MoveRearPos ||
                                                                  AutoInline.Class.InShuttle.Action == InShuttle.enumAction.RearUnLoading ||
                                                                  AutoInline.Class.InShuttle.Action == InShuttle.enumAction.RearUnLoadingCheck);

                                    if (!isIncomingFromShuttle)
                                    {
                                        // 2. 테스트 완료된 사이트 탐색 (Site 14 ~ 26) - Front와 동일한 우선순위 적용
                                        enumTeachingPos targetSite = enumTeachingPos.None;
                                        int currentPriority = 99;

                                        int startSite = (int)enumTeachingPos.Site14_R_DT1;
                                        int endSite = (int)enumTeachingPos.Site26_R_FT3;

                                        for (int i = startSite; i <= endSite; i++)
                                        {
                                            enumTeachingPos sitePos = (enumTeachingPos)i;
                                            int siteIdx = i - (int)enumTeachingPos.Site1_F_DT1; // 전역 SiteAction 인덱스 계산

                                            if (FuncInline.SiteAction[siteIdx] == FuncInline.enumSiteAction.Unloading &&
                                                FuncInline.PCBInfo[i].PCBStatus != enumSMDStatus.UnKnown)
                                            {
                                                int thisPriority = 99;
                                                var status = FuncInline.PCBInfo[i].PCBStatus;

                                                if (status == enumSMDStatus.Test_Fail) thisPriority = 1;
                                                else if (status == enumSMDStatus.Test_Pass) thisPriority = 2;
                                                else if (status == enumSMDStatus.ReTest) thisPriority = 3;
                                                else if (status == enumSMDStatus.DTest_AllFail) thisPriority = 4;
                                                else if (status == enumSMDStatus.DTest_Fail) thisPriority = 5;
                                                else if (status == enumSMDStatus.DTest_Pass) thisPriority = 6;

                                                if (thisPriority < currentPriority)
                                                {
                                                    currentPriority = thisPriority;
                                                    targetSite = sitePos;
                                                }
                                            }
                                        }

                                        // 결정된 사이트가 있으면 이동
                                        if (targetSite != enumTeachingPos.None)
                                        {
                                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination = targetSite;
                                            Log = $"{Name} Move to Site[{targetSite}] for Loading (Priority:{currentPriority})";
                                            FuncLog.WriteLog(Log);
                                            Action = enumAction.MoveLift;
                                        }
                                        else
                                        {
                                            // 아무 작업이 없으면 기본적으로 InShuttle 대기 위치로 이동하여 대기
                                            if (!FuncInlineMove.IsArrived((int)SV04_Lift2, LiftPos[Rear, (int)enumLiftPos.RearInShuttlePos]))
                                            {
                                                Action = enumAction.MoveSuttlePos;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // InShuttle이 오고 있으면 마중 나가거나 대기
                                        if (FuncInlineMove.IsArrived((int)SV04_Lift2, LiftPos[Rear, (int)enumLiftPos.RearInShuttlePos]))
                                        {
                                            // 이미 도착해 있다면 InShuttle이 배출 시작할 때 Loading으로 전환
                                            if (AutoInline.Class.InShuttle.Action == InShuttle.enumAction.RearUnLoading)
                                            {
                                                Action = enumAction.ShuttleLoading;
                                            }
                                        }
                                        else
                                        {
                                            // 입고 위치로 이동
                                            Log = $"{Name} Incoming PCB from InShuttle -> Move to InShuttle Position";
                                            FuncLog.WriteLog(Log);
                                            Action = enumAction.MoveSuttlePos;
                                        }

                                    }
                                }
                                // =============================================================
                                // [CASE 2] 리프트에 제품이 있을 때 (스캔 또는 배출 동작)
                                // =============================================================
                                else if (LiftStatus != enumSMDStatus.UnKnown)
                                {

                                    // 1. 스캔이 필요한 상태 (Destination이 자기 자신이거나 None일 때)
                                    if (dest == enumTeachingPos.Lift2_Up || dest == enumTeachingPos.None)
                                    {
                                        Log = $"{Name}{logPcbLiftID} Move to Scan Position";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.MoveScanPos;
                                    }
                                    // 2. OK 라인 배출
                                    else if (dest == enumTeachingPos.OutShuttle_Up)
                                    {
                                        Log = $"{Name}{logPcbLiftID} Move to OK Line Position";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.MoveOKline;
                                    }
                                    // 3. NG 라인 배출
                                    else if (dest == enumTeachingPos.OutShuttle_Down)
                                    {
                                        Log = $"{Name}{logPcbLiftID} Move to NG Line Position";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.MoveNGline;
                                    }
                                    // 4. 사이트 투입 (Site 14~26)
                                    else if (dest >= enumTeachingPos.Site14_R_DT1 && dest <= enumTeachingPos.Site26_R_FT3)
                                    {
                                        Log = $"{Name}{logPcbLiftID} Move to Site[{dest}] for UnLoading";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.MoveLift;
                                    }
                                }

                                #endregion
                            }
                            Util.InitWatch(ref watch);
                            break;
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
                                Log = $"[#2 샌딩 전 작업위치] {Enum.GetName(typeof(enumAction), Action)} -> Waiting";
                                FuncLog.WriteLog(Log);
                                Action = enumAction.Waiting;
                                break;
                            }

                            Util.InitWatch(ref watch);
                            break;
                        #endregion
                        case enumAction.Init:
                            #region Case Init
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.RearLift] = true;

                            // 1. 동작 중인 모터/컨베이어 정지
                            // PassLine 모터 정지
                            if (ROKLine_Motor)
                            {
                                Log = $"{Name} Init - PassLine Motor Stop";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(FuncInline.enumDONames.Y305_4_Rear_PassLine_Motor_Cw, false);
                            }
                            // NG Line 모터 정지
                            if (RNgLine_Motor)
                            {
                                Log = $"{Name} Init - NG Line Motor Stop";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(FuncInline.enumDONames.Y404_0_Rear_NgLine_Motor_Cw, false);
                            }
                            // Lift 모터 정지
                            if (RLift_UPMotor[0, 0] || RLift_DownMotor[0, 0])
                            {
                                DIO.WriteDOData(FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
                                DIO.WriteDOData(FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, false);
                            }

                            // 2. 실린더 초기화 (클램프 해제, 포고핀 상승)
                            // Rear Rack 전체 사이트 (14~26) 순회
                            int startPos = (int)FuncInline.enumTeachingPos.Site14_R_DT1;
                            int endPos = (int)FuncInline.enumTeachingPos.Site26_R_FT3;

                            for (int i = startPos; i <= endPos; i++)
                            {
                                enumTeachingPos currentSite = (enumTeachingPos)i;


                                // 1) 컨택트 UP/DOWN 솔 OFF
                                if (FuncInline.SiteIoMaps.TryGetContactUpDownDO(currentSite, out var updownDo))
                                {
                                    DIO.WriteDOData(updownDo, false);
                                }

                                // 2) 컨택트 스토퍼 솔 OFF
                                if (FuncInline.SiteIoMaps.TryGetContactStopperDO(currentSite, out var stopperDo))
                                {
                                    DIO.WriteDOData(stopperDo, false);
                                }

                                // 3) 사이트 이송 모터(정/역) OFF
                                if (FuncInline.SiteIoMaps.TryGetSiteMotor(currentSite, out var cwDo, out var ccwDo))
                                {
                                    DIO.WriteDOData(cwDo, false);
                                    DIO.WriteDOData(ccwDo, false);
                                }
                            }


                            //초기화 index
                            startPos = (int)enumInitialize.Site14_R_DT1;
                            endPos = (int)enumInitialize.Site26_R_FT3;
                            for (int i = startPos; i <= endPos; i++)
                            {
                                if (FuncInline.InitialDone[i] == false)
                                {
                                    FuncInline.InitialStarted[i] = true;
                                }
                            }
                            for (int i = startPos; i <= endPos; i++)
                            {
                                FuncInline.InitialDone[i] = FuncInlineAction.CheckOriginDone((enumInitialize)i);
                                if (FuncInline.InitialStarted[i] && FuncInline.InitialDone[i])
                                {
                                    Log = $"{Name} Init - InitialDone - {(enumInitialize)i}";
                                    FuncLog.WriteLog(Log);
                                    FuncInline.InitialStarted[i] = false;
                                }
                            }



                            // 3. 서보 모터 호밍 (Rear Lift & Rear Width)
                            // (1) 리프트 호밍
                            if (!GlobalVar.AxisStatus[SV04_Lift2].isHomed &&
                                GlobalVar.AxisStatus[SV04_Lift2].StandStill &&
                                !GlobalVar.AxisStatus[SV04_Lift2].Homing)
                            {
                                Log = $"{Name} Init - SV04_Lift2 Home Move Start";
                                FuncLog.WriteLog(Log);
                                FuncMotion.MoveHome((uint)SV04_Lift2);
                            }

                            // (2) 폭 조절 호밍
                            if (!GlobalVar.AxisStatus[SV05_Rack2_Width].isHomed &&
                                GlobalVar.AxisStatus[SV05_Rack2_Width].StandStill &&
                                !GlobalVar.AxisStatus[SV05_Rack2_Width].Homing)
                            {
                                Log = $"{Name} Init - SV05_Rack2_Width Home Move Start";
                                FuncLog.WriteLog(Log);
                                FuncMotion.MoveHome((uint)SV05_Rack2_Width);
                            }

                            FuncInline.InitialDone[(int)enumInitialize.RearLift] = FuncInlineAction.CheckOriginDone(enumInitialize.RearLift);
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.RearLift] = !FuncInline.InitialDone[(int)enumInitialize.RearLift];

                            // 4. 완료 확인
                            // 두 축 모두 호밍 완료 && 지정 위치 도달 확인
                            if (FuncInline.InitialDone[(int)enumInitialize.RearLift])
                            {
                                if (InitServo == false)
                                {
                                    InitServo = true;
                                    Log = $"{Name} Init - Servo Home Finish";
                                    FuncLog.WriteLog(Log);
                                }

                                Log = $"{Name} Init Finish";
                                FuncLog.WriteLog(Log);
                                Action = enumAction.InitFinish;
                            }

                            // 호밍 중 인터락 센서 감지 시 정지 (안전)
                            if (Rear_Rack_PCB_Interlock_Sensor)
                            {
                                Log = $"{Name} Init - Interlock Sensor Detected, Servo Stop";
                                FuncMotion.MoveStop(SV04_Lift2);
                                FuncMotion.MoveStop(SV05_Rack2_Width);
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
                        FuncMotion.MoveStop(SV04_Lift2);
                        FuncMotion.MoveStop(SV05_Rack2_Width);
                        Util.InitWatch(ref watch);
                    }
                    #endregion
                    //시스템 상태에 따른 타이머 제어 (Pause / Resume)
                    if (GlobalVar.SystemStatus != enumSystemStatus.AutoRun)
                    {
                        // 자동 운전이 아니면 타이머 일시정지 (시간이 흐르지 않음)
                        if (watch.IsRunning) watch.Stop();
                    }
                    else
                    {
                        // 자동 운전이고, 타이머가 멈춰있다면(일시정지 상태였다면) 다시 시작 (Resume)
                        // 단, Waiting 상태에서는 타이머가 돌 필요 없음
                        if (!watch.IsRunning && Action != enumAction.Waiting)
                        {
                            watch.Start();
                        }
                    }
                    #region if AutoRun
                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                    {

                        // 1. 상태 변경 감지 및 타이머 리셋
                        if (Action != beforeAction)
                        {
                            watch.Restart();
                            beforeAction = Action;
                        }

                        // 2. 타임아웃 체크 (Loading/UnLoading 관련 상태일 때만)
                        bool isCheckState = ((Action >= enumAction.MoveLift && Action <= enumAction.MoveNGline) ||
                                             Action == enumAction.UnLoading || Action == enumAction.UnLoadingCheck ||
                                             Action == enumAction.Loading || Action == enumAction.LoadingCheck);

                        if (isCheckState && watch.ElapsedMilliseconds > ActionTimeout)
                        {
                            watch.Stop(); // 타임아웃 발생 시 타이머 정지
                            if ((Action >= enumAction.MoveLift && Action <= enumAction.MoveNGline))
                            {
                                FuncInline.AddError(FuncInline.enumErrorPart.Lift2_Up, FuncInline.enumErrorCode.MoveFail,
                                    $"{Log}{Action.ToString()} Servo Move Timeout.");
                            }
                            //Scan에러는 NG로 배출해야함
                            //else if (Action == enumAction.MoveScanPos)
                            //{
                            //    FuncInline.AddError(FuncInline.enumErrorPart.Lift2_Up, FuncInline.enumErrorCode.Scan_Timeout,
                            //   $"{Log} UnLoading Timeout.");
                            //}
                            else if (Action == enumAction.UnLoading || Action == enumAction.UnLoadingCheck)
                            {
                                if (dest == enumTeachingPos.RearPassLine || dest == enumTeachingPos.OutShuttle_Up)
                                {
                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift2_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                                    $"{Log} UnLoading Timeout.");
                                    FuncInline.AddError(FuncInline.enumErrorPart.RearPassLine, FuncInline.enumErrorCode.Conveyor_Timeout,
                                                    $"{Log} Loading Timeout.");

                                    OKLineAction = enumOKLineAction.Waiting;
                                }
                                else if (dest == enumTeachingPos.RearNGLine || dest == enumTeachingPos.OutShuttle_Down)
                                {
                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift2_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                                 $"{Log} UnLoading Timeout.");
                                    FuncInline.AddError(FuncInline.enumErrorPart.RearNGLine, FuncInline.enumErrorCode.Conveyor_Timeout,
                                                $"{Log} Loading Timeout.");

                                    NGLineAction = enumNGLineAction.Waiting;
                                }
                                else
                                {
                                    FuncInline.enumErrorPart errorPart = siteIdxLocal + FuncInline.enumErrorPart.Site1_F_DT1;

                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift2_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                            $"{Log} UnLoading Timeout.");
                                    FuncInline.AddError(errorPart, FuncInline.enumErrorCode.Conveyor_Timeout,
                                             $"{Log} Loading Timeout.");

                                    FuncInline.SiteAction[siteIdxLocal] = enumSiteAction.Waiting;
                                }

                            }

                            else if (Action == enumAction.Loading || Action == enumAction.LoadingCheck)
                            {

                                FuncInline.enumErrorPart errorPart = siteIdxLocal + FuncInline.enumErrorPart.Site1_F_DT1;

                                FuncInline.AddError(FuncInline.enumErrorPart.Lift2_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                                $"{Log} Loading Timeout.");
                                FuncInline.AddError(errorPart, FuncInline.enumErrorCode.Conveyor_Timeout,
                                                $"{Log} UnLoading Timeout.");

                                FuncInline.SiteAction[siteIdxLocal] = enumSiteAction.Waiting;

                            }
                            else // Loading 등 기타
                            {
                                FuncInline.AddError(FuncInline.enumErrorPart.Lift2_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                                $"{Log} Action Timeout.");
                            }

                            // 초기화 및 대기
                            Action = enumAction.Waiting;
                            continue; // switch문 실행 안 하고 다음 루프로
                        }

                        // -----------------------------------------------------
                        // [메인 시퀀스]
                        // -----------------------------------------------------
                        switch (Action)
                        {
                            // =============================================================
                            // [Move Actions] FrontRack 스타일 적용
                            // =============================================================
                            case enumAction.MoveSuttlePos:
                                #region MoveSuttlePos
                                targetPos = LiftPos[Rear, (int)enumLiftPos.RearInShuttlePos]; // Rear용 인덱스 사용
                                if (FuncInlineMove.IsArrived(SV04_Lift2, targetPos))
                                {
                                    Log = $"{Name}[Lift] InShuttle Pos Arrived -> Loading";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.Waiting;
                                }
                                else if (GlobalVar.AxisStatus[SV04_Lift2].StandStill)
                                {
                                    FuncInlineMove.MoveAbsolute((uint)SV04_Lift2, targetPos);
                                }
                                break;
                            #endregion

                            case enumAction.MoveScanPos:
                                #region MoveScanPos
                                targetPos = LiftPos[Rear, (int)enumLiftPos.RearScanPos];
                                if (FuncInlineMove.IsArrived(SV04_Lift2, targetPos))
                                {
                                    Log = $"{Name}[Lift] Scan Pos Arrived -> ScanWait";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.ScanWait;
                                }
                                else if (GlobalVar.AxisStatus[SV04_Lift2].StandStill)
                                {
                                    FuncInlineMove.MoveAbsolute((uint)SV04_Lift2, targetPos);
                                }
                                break;
                            #endregion

                            case enumAction.MoveOKline:
                                #region MoveOKline
                                targetPos = LiftPos[Rear, (int)enumLiftPos.RearPassLine];
                                if (FuncInlineMove.IsArrived(SV04_Lift2, targetPos))
                                {
                                    Log = $"{Name}[Lift] OK Line Pos Arrived -> UnLoading";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.UnLoading;
                                }
                                else if (GlobalVar.AxisStatus[SV04_Lift2].StandStill)
                                {
                                    FuncInlineMove.MoveAbsolute((uint)SV04_Lift2, targetPos);
                                }
                                break;
                            #endregion

                            case enumAction.MoveNGline:
                                #region MoveNGline
                                targetPos = LiftPos[Rear, (int)enumLiftPos.RearNGLine];
                                if (FuncInlineMove.IsArrived(SV04_Lift2, targetPos))
                                {
                                    Log = $"{Name}[Lift] NG Line Pos Arrived -> UnLoading";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.UnLoading;
                                }
                                else if (GlobalVar.AxisStatus[SV04_Lift2].StandStill)
                                {
                                    FuncInlineMove.MoveAbsolute((uint)SV04_Lift2, targetPos);
                                }
                                break;
                            #endregion

                            case enumAction.MoveLift:
                                #region MoveLift (Test Site)
                                dest = PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination;
                                // Site 인덱스 계산 (14~26 범위)
                                sitePosindex = ((int)dest - (int)enumTeachingPos.Site14_R_DT1) + (int)enumLiftPos.Site14_R_DT1_Up;
                                targetPos = LiftPos[Rear, sitePosindex];

                                if (FuncInlineMove.IsArrived(SV04_Lift2, targetPos))
                                {
                                    Log = $"{Name}[Lift] {dest} Arrived -> Action";
                                    FuncLog.WriteLog(Log);
                                    Action = (PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus == enumSMDStatus.UnKnown) ? enumAction.Loading : enumAction.UnLoading;
                                }
                                else if (GlobalVar.AxisStatus[SV04_Lift2].StandStill)
                                {
                                    FuncInlineMove.MoveAbsolute((uint)SV04_Lift2, targetPos);
                                }
                                break;
                            #endregion

                            // =============================================================
                            // [Process Actions]
                            // =============================================================
                            case enumAction.ShuttleLoading:
                                #region ShuttleLoading
                                // InShuttle 배출 상태 확인 로직 포함
                                Stopper_IN_Open(true);
                                Stopper_Out_Open(false);
                                if (AutoInline.Class.InShuttle.Action == InShuttle.enumAction.RearUnLoading ||
                                        AutoInline.Class.InShuttle.Action == InShuttle.enumAction.RearUnLoadingCheck)
                                {
                                    if (LiftLoadingShuttleAction())
                                    {
                                        Log = $"{Name}[Lift]InShuttle->LiftUp Loading Finish, LoadingCheck Action ";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.ShuttleLoadingCheck;
                                    }
                                }

                                break;
                            #endregion

                            case enumAction.ShuttleLoadingCheck:
                                if (AutoInline.Class.InShuttle.Action != InShuttle.enumAction.RearUnLoadingCheck) continue;

                                FuncInline.MovePCBInfo(enumTeachingPos.InShuttle, enumTeachingPos.Lift2_Up);

                                // 3. [Logical Check] 데이터 이동 완료 확인
                                // Source는 비워졌는지(UnKnown), Lift는 채워졌는지(!UnKnown) 확인
                                bool isInshuttleEmpty = PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus == enumSMDStatus.UnKnown;
                                bool isLiftOccupied = PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus != enumSMDStatus.UnKnown;

                                if (isInshuttleEmpty && isLiftOccupied)
                                {
                                    Log = $"{Name}[Lift][PCB_ID:{PCBInfo[(int)enumTeachingPos.Lift2_Up].Num}]LiftUp Loading Complete from InShuttle";
                                    FuncLog.WriteLog(Log);

                                    // 4. 목적지 초기화 (이제 Lift가 가지고 있으므로 Source 정보는 불필요)
                                    // 단, 다음 갈 곳(OutShuttle 등)을 정해야 한다면 유지하거나 Waiting에서 다시 판단

                                    Action = enumAction.Waiting;
                                }

                                break;
                            // =============================================================
                            // [로딩 동작] - Loading (From Site 14~26)
                            // =============================================================
                            case enumAction.Loading:

                                Stopper_IN_Open(false);
                                Stopper_Out_Open(true);
                                targetPos = LiftPos[Rear, sitePosindex];
                                // 3. Test Site에서 로딩
                                if (FuncInline.SiteAction[siteIdxLocal] == enumSiteAction.Unloading &&
                                     FuncInlineMove.IsArrived((int)SV04_Lift2, targetPos))
                                {
                                    // Site에서 Lift로 당겨오는 방향의 함수 호출 (Shuttle과 반대 방향)
                                    if (LiftLoadingSiteAction(dest))
                                    {
                                        Log = $"{Name}[Lift][{(enumTeachingPos)dest}] Site->LiftUp Loading Finish, LoadingCheck Action ";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.LoadingCheck;
                                    }
                                }
                                else
                                {
                                    watch.Restart();
                                }
                                break;

                            case enumAction.LoadingCheck:

                                // Waiting 상태에서 Destination에 '가져올 위치'를 저장해두었음
                                var sourcePos = PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination;

                                if (FuncInline.IsDelayOver(Key_Lift_Load, 500))
                                {
                                    // Destination에 저장된 사이트로부터 데이터 수취
                                    FuncInline.MovePCBInfo(PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination, enumTeachingPos.Lift2_Up);
                                }
                                // 3. [Logical Check] 데이터 이동 완료 확인
                                // Source는 비워졌는지(UnKnown), Lift는 채워졌는지(!UnKnown) 확인
                                bool isSourceEmpty = PCBInfo[(int)sourcePos].PCBStatus == enumSMDStatus.UnKnown;
                                isLiftOccupied = PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus != enumSMDStatus.UnKnown;

                                if (isSourceEmpty && isLiftOccupied)
                                {

                                    Log = $"{Name} Site Loading Finish";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.Waiting;

                                    Stopper_IN_Open(false);
                                    Stopper_Out_Open(false);
                                }


                                break;

                            // =============================================================
                            // [스캔 및 판정 동작] - FrontRack 구조 추종
                            // =============================================================
                            case enumAction.ScanWait:
                                #region ScanWait
                                // 1. 05_Scan 쓰레드에 스캔 시작 요청
                                if (!AutoInline.Class.Scan.RearScanComplete)
                                {
                                    ScanReady = true;
                                }
                                // 2. 스캔 완료 대기
                                else
                                {
                                    ScanReady = false;
                                    AutoInline.Class.Scan.RearScanComplete = false;

                                    Log = $"{Name} Scan Finished. Move to ScanOK.";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.ScanOK;
                                }
                                break;
                            #endregion

                            case enumAction.ScanOK:
                                #region ScanOK (판정 및 차기 목적지 결정)
                                {
                                    // 1. 결과 판정 (ErrorCode가 -1이 아니면 불량)
                                    bool isNG = false;
                                    for (int i = 0; i < MaxArrayCount; i++)
                                    {
                                        if (PCBInfo[(int)enumTeachingPos.Lift2_Up].ErrorCode[i] != -1)
                                        {
                                            isNG = true;
                                            break;
                                        }
                                    }

                                    // 2. 목적지 결정
                                    if (isNG)
                                    {
                                        // NG인 경우 즉시 NG Line(Down)으로 지정
                                        PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination = enumTeachingPos.OutShuttle_Down;
                                        Log = $"{Name}{logPcbLiftID} Result: NG -> Dest: OutShuttle_Down";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.Waiting; // Waiting에서 MoveNGline 판단
                                    }
                                    else
                                    {
                                        // OK인 경우 순차 FT 사이트 검색 (Rear 랙 검색: false)
                                        enumTeachingPos nextFT = GetAvailableFTSite(false);

                                        if (nextFT != enumTeachingPos.None)
                                        {
                                            PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination = nextFT;

                                            // 순차 인덱스 갱신 (시작점: Site23 또는 Site24 세대별 기준)
                                            int startFT = (InlineType == enumInlineType.Gen5) ? (int)enumTeachingPos.Site23_R_DT10_FT4 : (int)enumTeachingPos.Site24_R_FT1;
                                            AutoInline.Class.RearRack.LastRearFTIndex = (int)nextFT - startFT;

                                            Log = $"{Name}{logPcbLiftID} Result: OK -> Dest: {nextFT}";
                                            FuncLog.WriteLog(Log);
                                            Action = enumAction.Waiting; // Waiting에서 MoveLift 판단
                                        }
                                        else
                                        {
                                            // 투입 가능한 FT 사이트가 없으면 대기 (인터락)
                                            if (watch.ElapsedMilliseconds > 2000)
                                            {
                                                Log = $"{Name} Scan OK, but all FT Sites are Busy. Waiting...";
                                                FuncLog.WriteLog(Log);
                                                watch.Restart();
                                            }
                                        }
                                    }
                                }
                                break;
                            #endregion
                            // =============================================================
                            // [언로딩 동작] - Site 배출 인터락 및 딜레이 포함
                            // =============================================================
                            case enumAction.UnLoading:
                                #region UnLoading
                                {
                                    //dest = PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination;
                                    siteIdxLocal = (int)dest - (int)enumTeachingPos.Site1_F_DT1; // 전역 인덱스 (0~25)

                                    Stopper_IN_Open(false);
                                    Stopper_Out_Open(true);

                                    // 1. 배출 인터락 확인 (사이트 또는 라인 상태)
                                    bool isTargetReady = false;

                                    // A. OK/NG Line 배출 시
                                    if (dest == enumTeachingPos.OutShuttle_Up || dest == enumTeachingPos.OutShuttle_Down)
                                    {
                                        if (dest == enumTeachingPos.OutShuttle_Up && OKLineAction == enumOKLineAction.Waiting)
                                        {
                                            OKLineAction = enumOKLineAction.Loading;
                                        }
                                        else if (dest == enumTeachingPos.OutShuttle_Down && NGLineAction == enumNGLineAction.Waiting)
                                        {
                                            NGLineAction = enumNGLineAction.Loading;
                                        }
                                        isTargetReady = true; // 라인은 위에서 상태 전이를 했으므로 진행
                                    }
                                    // B. Test Site 배출 시 (14~26)
                                    else if (dest >= enumTeachingPos.Site14_R_DT1 && dest <= enumTeachingPos.Site26_R_FT3)
                                    {
                                        // 사이트가 Waiting 상태이고, 비어있어야 배출 가능
                                        if (FuncInline.SiteAction[siteIdxLocal] == enumSiteAction.Waiting &&
                                            PCBInfo[(int)dest].PCBStatus == enumSMDStatus.UnKnown)
                                        {
                                            // 상대 사이트를 Loading 상태로 변경
                                            FuncInline.SiteAction[siteIdxLocal] = enumSiteAction.Loading;
                                            
                                        }
                                        if(FuncInline.SiteAction[siteIdxLocal] == enumSiteAction.Loading)
                                        {
                                            isTargetReady = true;
                                        }
                                    }

                                    // 2. 배출 수행
                                    if (isTargetReady)
                                    {
                                        Stopper_IN_Open(false);
                                        Stopper_Out_Open(true); // 배출용 스토퍼 열기

                                        if (LiftUnLoadingAction(dest))
                                        {
                                            Log = $"{Name}[Lift]{logPcbLiftID} UnLoading Finish -> Check Action";
                                            FuncLog.WriteLog(Log);
                                            Action = enumAction.UnLoadingCheck;
                                        }
                                    }
                                    else
                                    {
                                        // 목적지가 준비 안 됨 (병목 현상)
                                        if (watch.ElapsedMilliseconds > 2000)
                                        {
                                            Log = $"{Name} Waiting for Destination [{dest}] Ready...";
                                            FuncLog.WriteLog(Log);
                                            watch.Restart();
                                        }
                                    }
                                }
                                break;
                            #endregion

                            case enumAction.UnLoadingCheck:
                                #region UnLoadingCheck
                                {
                                    //dest = PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination;

                                    // 1. 배출 완료 후 딜레이 (데이터 이동 전 안착 시간)
                                    if (FuncInline.IsDelayOver(Key_Lift_Unload, 200))
                                    {
                                        // 2. 데이터 이동 (Lift -> Dest)
                                        FuncInline.MovePCBInfo(enumTeachingPos.Lift2_Up, dest);

                                        // 3. 상대 공정 상태 전이
                                        if (dest == enumTeachingPos.OutShuttle_Up)
                                        {
                                            // 리프트는 비워졌고, 스캔사이트에 데이터가 찼는지 확인
                                            if (PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus == enumSMDStatus.UnKnown &&
                                                PCBInfo[(int)dest].PCBStatus != enumSMDStatus.UnKnown)
                                            {
                                                Log = $"{Name}[Lift][OKLine] UnLoading Complete. Lift Empty.";
                                                FuncLog.WriteLog(Log);
                                                Action = enumAction.Waiting;

                                                Stopper_IN_Open(false);
                                                Stopper_Out_Open(false);
                                            }
                                        }

                                        else if (dest == enumTeachingPos.OutShuttle_Down)
                                        {
                                            // 리프트는 비워졌고, 스캔사이트에 데이터가 찼는지 확인
                                            if (PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus == enumSMDStatus.UnKnown &&
                                                PCBInfo[(int)dest].PCBStatus != enumSMDStatus.UnKnown)
                                            {
                                                Log = $"{Name}[Lift][NGLine] UnLoading Complete. Lift Empty.";
                                                FuncLog.WriteLog(Log);
                                                Action = enumAction.Waiting;

                                                Stopper_IN_Open(false);
                                                Stopper_Out_Open(false);
                                            }
                                        }
                                        else if (dest >= enumTeachingPos.Site14_R_DT1 && dest <= enumTeachingPos.Site26_R_FT3)
                                        {
                                            // 완료 확인
                                            if (PCBInfo[(int)enumTeachingPos.Lift2_Up].PCBStatus == enumSMDStatus.UnKnown &&
                                                PCBInfo[(int)dest].PCBStatus != enumSMDStatus.UnKnown)
                                            //FuncInline.SiteAction[siteIdxLocal] == enumSiteAction.Testing)
                                            {
                                                Log = $"{Name}[Lift][{dest}] UnLoading Complete. Lift Empty.";
                                                FuncLog.WriteLog(Log);
                                                Action = enumAction.Waiting;

                                                Stopper_IN_Open(false);
                                                Stopper_Out_Open(false);
                                            }
                                        }





                                    }
                                }
                                break;
                                #endregion

                        }

                        // -----------------------------------------------------
                        // [서브 시퀀스] OK Line & NG Line Logic
                        // -----------------------------------------------------
                        Logic_OKLine();
                        Logic_NGLine();
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
                        //debug("action change " + beforeAction.ToString() + " ==> " + Action.ToString());
                        Util.ResetWatch(ref watch);
                    }
                    beforeAction = Action;
                    beforeSystemStatus = GlobalVar.SystemStatus;
                    #endregion
                    #endregion
                }
                catch (Exception ex)
                {
                    FuncLog.WriteLog("BeforeWorkClass.ActionThread : " + ex.ToString());
                    FuncLog.WriteLog("BeforeWorkClass.ActionThread : " + ex.StackTrace);
                }

                Thread.Sleep(GlobalVar.ThreadSleep);
            }
        }
        private void StepFinish_Send()
        {   //완료직전에 Ampule데이터 입력
            //FuncAmplePacking.ampule[1].Model = GlobalVar.ModelName;
            //FuncAmplePacking.ampule[1].Lot_Model = FuncAmplePacking.LOT_Model;
            //FuncAmplePacking.ampule[1].Lot_Num = FuncAmplePacking.LOT_Num;
            //FuncAmplePacking.ampule[1].Left_Ampule = 1;  //공급했으니 좌측 앰플 유무표시
            //FuncAmplePacking.ampule[1].Right_Ampule = 1;

            StepFinish = true;  //완료했으면 True
        }

        /// <summary>
        /// Rear Rack (Site 14~26) 전체 상태 업데이트
        /// </summary>
        public void UpdateRearAllStatus()
        {
            int startPos = (int)enumTeachingPos.Site14_R_DT1;
            int endPos = (int)enumTeachingPos.Site26_R_FT3;

            for (int i = startPos; i <= endPos; i++)
            {
                enumTeachingPos currentSite = (enumTeachingPos)i;
                int index = i - startPos; // 0 ~ 12
                if (index >= Rear_PCB_Sensor.Length) break;

                // 1. PCB 감지 (SiteIoMaps 활용)
                if (SiteIoMaps.TryGetPcbDockDI(currentSite, out enumDINames diPcb))
                    Rear_PCB_Sensor[index] = DIO.GetDIData(diPcb);
                else
                    Rear_PCB_Sensor[index] = false;

                // 2. 클램프 솔 (SiteIoMaps 활용)
                if (SiteIoMaps.TryGetContactStopperDO(currentSite, out enumDONames doClamp))
                    Rear_ClampSol[index] = DIO.GetDOData(doClamp);
                else
                    Rear_ClampSol[index] = false;

                // 3. 포고핀 다운 솔 (SiteIoMaps 활용)
                if (SiteIoMaps.TryGetContactUpDownDO(currentSite, out enumDONames doDown))
                    Rear_DownSol[index] = DIO.GetDOData(doDown);
                else
                    Rear_DownSol[index] = false;

                // 4. 포고핀 업 센서 (SiteIoMaps 활용)
                if (SiteIoMaps.TryGetContactUpDI(currentSite, out enumDINames diUp))
                    Rear_Up_Sensor[index] = DIO.GetDIData(diUp);
                else
                    Rear_Up_Sensor[index] = false;

                // 5. 모터 상태 (SiteIoMaps 활용)
                if (SiteIoMaps.TryGetSiteMotor(currentSite, out enumDONames cw, out enumDONames ccw))
                {
                    Rear_Motor[index, 0] = DIO.GetDOData(cw);
                    Rear_Motor[index, 1] = DIO.GetDOData(ccw);
                }
                else
                {
                    Rear_Motor[index, 0] = false;
                    Rear_Motor[index, 1] = false;
                }
            }
        }

        /// <summary>
        /// Rear Lift 및 기타 Passline 센서/모터 업데이트
        /// (제공된 Enum 목록 기반 매핑)
        /// </summary>
        public void UpdateRearETCStatus()
        {
            // [DI] 리프트 및 패스라인 센서
            RLift_UpPCB_IN_Sensor = DIO.GetDIData(enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor);
            RLift_UpPCB_Stop_Sensor = DIO.GetDIData(enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor);

            RLift_DownPCB_IN_Sensor = DIO.GetDIData(enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor);
            RLift_DownPCB_Stop_Sensor = DIO.GetDIData(enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor);

            // 스토퍼 실린더 센서 (IN_UP / Out_UP) 상황에 따라 선택
            RLift_IN_Stopper = DIO.GetDOData(enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL); ;     // 스토퍼 실린더 상승(IN) 
            RLift_OUT_Stopper = DIO.GetDOData(enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL); ;     // 스토퍼 실린더 상승(OUT)

            RLift_IN_UpStopper_Sensor = DIO.GetDIData(enumDINames.X405_3_Rear_Lift_Stopper_Cyl_IN_UP_Sensor);
            RLift_Out_UpStopper_Sensor = DIO.GetDIData(enumDINames.X406_2_Rear_Lift_Stopper_Cyl_Out_UP_Sensor);

            // 패스라인 관련
            ROKLine_Stopper = DIO.GetDOData(enumDONames.Y1_6_Rear_OK_PassLine_CONTACT_STOPPER_SOL);    //Rear OK PassLine 스토퍼
            RNGLine_Stopper = DIO.GetDOData(enumDONames.Y4_5_Rear_NG_PassLine_CONTACT_STOPPER_SOL);    //Rear NG PassLine 스토퍼

            Rear_Pass_OkLine_PCB_In_Sensor = DIO.GetDIData(enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor);
            Rear_Pass_NgLine_PCB_Stop_Sensor = DIO.GetDIData(enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor);
            Rear_Rack_PCB_Interlock_Sensor = DIO.GetDIData(enumDINames.X405_2_Rear_Rack_PCB_Interlock_Sensor);


            // [DO] 모터 상태 확인
            // Rear PassLine Motor
            ROKLine_Motor = DIO.GetDOData(enumDONames.Y305_4_Rear_PassLine_Motor_Cw);

            // Rear NG Line Motor (Y404_0)
            RNgLine_Motor = DIO.GetDOData(enumDONames.Y404_0_Rear_NgLine_Motor_Cw);

            // Rear Lift Up Motor
            RLift_UPMotor[0, 0] = DIO.GetDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw);
            RLift_UPMotor[0, 1] = DIO.GetDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw);

            // Rear Lift Down Motor
            RLift_DownMotor[0, 0] = DIO.GetDOData(enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw);
            RLift_DownMotor[0, 1] = DIO.GetDOData(enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw);
        }
        private void Logic_RearRack()
        {

        }
        private void Logic_OKLine()
        {
            switch (OKLineAction)
            {
                case enumOKLineAction.Loading:
                    DIO.WriteDOData(enumDONames.Y305_4_Rear_PassLine_Motor_Cw, true);
                    if (Rear_Pass_OkLine_PCB_In_Sensor) OKLineAction = enumOKLineAction.LoadingCheck;
                    break;
                case enumOKLineAction.LoadingCheck:
                    DIO.WriteDOData(enumDONames.Y305_4_Rear_PassLine_Motor_Cw, false);
                    OKLineAction = enumOKLineAction.UnLoading;
                    break;
                case enumOKLineAction.UnLoading:
                    // 04_OutShuttle이 비어있고 받을 준비가 되면 배출
                    if (PCBInfo[(int)enumTeachingPos.OutShuttle_Up].PCBStatus == enumSMDStatus.UnKnown)
                    {
                        DIO.WriteDOData(enumDONames.Y305_4_Rear_PassLine_Motor_Cw, true);
                        // OutShuttle 입구 센서 확인 로직 필요
                        if (!Rear_Pass_OkLine_PCB_In_Sensor) OKLineAction = enumOKLineAction.UnLoadingCheck;
                    }
                    break;
                case enumOKLineAction.UnLoadingCheck:
                    DIO.WriteDOData(enumDONames.Y305_4_Rear_PassLine_Motor_Cw, false);
                    FuncInline.MovePCBInfo(enumTeachingPos.OutShuttle_Up, enumTeachingPos.OutShuttle_Up); // 데이터는 이미 이동되었을 것
                    OKLineAction = enumOKLineAction.Waiting;
                    break;
            }
        }

        private void Logic_NGLine()
        {
            switch (NGLineAction)
            {
                case enumNGLineAction.Loading:
                    DIO.WriteDOData(enumDONames.Y404_0_Rear_NgLine_Motor_Cw, true);
                    if (Rear_Pass_NgLine_PCB_Stop_Sensor) NGLineAction = enumNGLineAction.LoadingCheck;
                    break;
                case enumNGLineAction.LoadingCheck:
                    DIO.WriteDOData(enumDONames.Y404_0_Rear_NgLine_Motor_Cw, false);
                    NGLineAction = enumNGLineAction.UnLoading;
                    break;
                case enumNGLineAction.UnLoading:
                    if (PCBInfo[(int)enumTeachingPos.OutShuttle_Down].PCBStatus == enumSMDStatus.UnKnown)
                    {
                        DIO.WriteDOData(enumDONames.Y404_0_Rear_NgLine_Motor_Cw, true);
                        if (!Rear_Pass_NgLine_PCB_Stop_Sensor) NGLineAction = enumNGLineAction.UnLoadingCheck;
                    }
                    break;
                case enumNGLineAction.UnLoadingCheck:
                    DIO.WriteDOData(enumDONames.Y404_0_Rear_NgLine_Motor_Cw, false);
                    NGLineAction = enumNGLineAction.Waiting;
                    break;
            }
        }

        private bool LiftUnLoadingAction(enumTeachingPos dest)
        {
            // 배출 모터 구동 (Lift -> Line)
            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, true);

            bool arrivalSensor = false;
            if (dest == enumTeachingPos.OutShuttle_Up)
            {
                arrivalSensor = Rear_Pass_OkLine_PCB_In_Sensor;
            }
            else if (dest == enumTeachingPos.OutShuttle_Down)
            {
                arrivalSensor = Rear_Pass_NgLine_PCB_Stop_Sensor;
            }
            else
            {
                // 사이트 인덱스 계산 (0 ~ 12)
                int siteIdx = (int)dest - (int)enumTeachingPos.Site14_R_DT1;

                // FSite_Stop_Sensor 배열이 있다고 가정 (UpdateFrontAllStatus에서 갱신됨)
                // 만약 배열이 없다면: DIO.GetDIData(BaseAddress + siteIdx) 방식으로 읽어야 함
                if (siteIdx >= 0 && siteIdx < 13)
                {
                    arrivalSensor = Rear_PCB_Sensor[siteIdx];
                }
            }



            if (!RLift_UpPCB_Stop_Sensor && !RLift_UpPCB_IN_Sensor && arrivalSensor)
            {
                if (FuncInline.IsDelayOver(Key_Lift_Unload, 500))
                {
                    DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
                    return true;
                }
            }
            else
            {
                FuncInline.ResetDelay(Key_Lift_Unload);
            }


            return false;
        }
        /// <summary>
        /// InShuttle로부터 제품을 로딩하는 동작 (방향: CW)
        /// </summary>
        private bool LiftLoadingShuttleAction()
        {
            switch (loadingStep)
            {
                case 0: // 1단계: CW 구동 및 도착 센서 대기
                    DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, true);
                    DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);

                    if (!AutoInline.Class.InShuttle.X_Pcb_Stop_Sensor && RLift_UpPCB_Stop_Sensor)
                    {
                        if (FuncInline.IsDelayOver(Key_Lift_Load, 300))
                        {
                            DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
                            loadingStep = 10;
                        }
                    }
                    else FuncInline.ResetDelay(Key_Lift_Load);
                    break;

                case 10: // 2단계: 잠시 정지 후 오버드라이브 준비
                    if (FuncInline.IsDelayOver(Key_Lift_Load, 100))
                    {
                        DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, true);
                        loadingStep = 20;
                    }
                    break;

                case 20: // 3단계: 확실한 안착을 위해 2초간 추가 구동
                    if (FuncInline.IsDelayOver(Key_Lift_Load, 2000))
                    {
                        DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
                        loadingStep = 0; // 스텝 초기화
                        return true;     // 동작 완료
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Rear Site(14~26)로부터 제품을 로딩하는 동작 (방향: CCW)
        /// </summary>
        private bool LiftLoadingSiteAction(enumTeachingPos dest)
        {
            switch (loadingStep)
            {
                case 0: // 1단계: CCW 구동 및 도착 센서 대기
                    DIO.WriteDOData(enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
                    DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, true); // 반대 방향

                    // 사이트 인덱스 계산 (0 ~ 12)
                    int siteIdx = (int)dest - (int)enumTeachingPos.Site14_R_DT1;
                    bool UnLoadingSensor = false;
                    // FSite_Stop_Sensor 배열이 있다고 가정 (UpdateFrontAllStatus에서 갱신됨)
                    // 만약 배열이 없다면: DIO.GetDIData(BaseAddress + siteIdx) 방식으로 읽어야 함
                    if (siteIdx >= 0 && siteIdx < 13)
                    {
                        UnLoadingSensor = Rear_PCB_Sensor[siteIdx];
                    }

                    if (RLift_UpPCB_IN_Sensor && !UnLoadingSensor)
                    {
                        if (FuncInline.IsDelayOver(Key_Lift_Load, 300))
                        {
                            DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
                            loadingStep = 10;
                        }
                    }
                    else FuncInline.ResetDelay(Key_Lift_Load);
                    break;

                case 10: // 2단계: 정지 후 오버드라이브
                    if (FuncInline.IsDelayOver(Key_Lift_Load, 100))
                    {
                        DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, true);
                        loadingStep = 20;
                    }
                    break;

                case 20: // 3단계: 최종 안착 구동
                    if (FuncInline.IsDelayOver(Key_Lift_Load, 2000))
                    {
                        DIO.WriteDOData(enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
                        loadingStep = 0;
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 진입(IN) 스토퍼 제어
        /// </summary>
        /// <param name="isUp">true: 스토퍼 상승(On), false: 하강(Off)</param>
        public void Stopper_IN_Open(bool isUp)
        {
            FuncInline.enumDONames targetSol;
            // [Front/CCW 상태] 정상 방향이므로 물리적 IN 솔레노이드를 동작
            targetSol = FuncInline.enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL;

            DIO.WriteDOData(targetSol, isUp);
        }
        /// <summary>
        /// 진입(IN) 스토퍼 제어
        /// </summary>
        /// <param name="isUp">true: 스토퍼 상승(On), false: 하강(Off)</param>
        public void Stopper_Out_Open(bool isUp)
        {
            FuncInline.enumDONames targetSol;
            // [Front/CCW 상태] 정상 방향이므로 물리적 IN 솔레노이드를 동작
            targetSol = FuncInline.enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL;

            DIO.WriteDOData(targetSol, isUp);
        }
    }
}
