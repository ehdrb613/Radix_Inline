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
    class FrontRack
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

            MoveLift,   //각 사이트,Passline,Scansite,Outshuttle위치로 이동
            MovePassline,
            MoveScanSite,
            MoveOutUp,
            MoveOutDown,

            Loading,   //테스트 완료된 사이트, 스캔 완료된 Scan사이트로 부터 로딩
            LoadingCheck,

            UnLoading,  //사이트,Scan,Passline 사이트로 언로딩,OutShuttle로 언로딩
            UnLoadingCheck
        }
        public enum enumPassLineAction
        {
            Waiting, // 아무 동작 없을 때
            Loading,   //Inshuttle로 부터 로딩
            LoadingCheck,
            UnLoading,  //FrontLift로 언로딩
            UnLoadingCheck
        }
        public enum enumScanSiteAction
        {
            Waiting, // 아무 동작 없을 때
            Loading,   //FrontLift로 부터 로딩
            LoadingCheck,
            ScanWait,
            ScanOK,
            UnLoading,  //FrontLift로 언로딩
            UnLoadingCheck
        }
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
        /** @brief 쓰레드의 동작 단계 */
        public enumPassLineAction PasslineAction = enumPassLineAction.Waiting;
        /** @brief 쓰레드의 동작 단계 */
        public enumScanSiteAction ScanSiteAction = enumScanSiteAction.Waiting;
        /** @brief 쓰레드의 이전 동작 단계 */
        private enumAction beforeAction = enumAction.Waiting;
        /** @brief 시스템의 이전 상태 */
        private enumSystemStatus beforeSystemStatus = GlobalVar.SystemStatus;

        /** @brief 동작 수행시 타임아웃 체크 */
        private Stopwatch watch = new Stopwatch();
        // 로딩 스텝 및 딜레이 타이머
        private int loadingStep = 0;

        public int LastFrontFTIndex = 0; // FT 사이트 순차 투입용 인덱스
        private Stopwatch delayWatch = new Stopwatch();

        /** @brief 한 공정 완료 여부. 각 하부 Part별로 완료여부 체크되면 컨베어 움직이고, 컨베어 움직이기 시작하면 완료여부 clear 하면 된다. */
        public bool StepFinish = false;

        /** @brief 현재 공정에서 작업중인 모델정보 */
        public string NowModel = "";

        public int SV02_Lift1 = (int)FuncInline.enumServoAxis.SV02_Lift1;
        public int SV03_Rack1_Width = (int)FuncInline.enumServoAxis.SV03_Rack1_Width;
        public int Front = (int)enumLiftName.FrontLift;
        private enumTeachingPos NextDestination = enumTeachingPos.None;

        private const string Key_PassLine_Load = "PassLine_Load";
        private const string Key_PassLine_Unload = "PassLine_Unload";
        private const string Key_Lift_Load = "Lift_Load";
        private const string Key_Lift_Unload = "Lift_Unload";
        private const string Key_Conveyor_Load = "Conveyor_Load";
        private const string Key_Conveyor_Unload = "Conveyor_Unload";
        private const string Key_NG_Load = "NG_Load";
        private const string Key_NG_Unload = "NG_Unload";

        #region InShuttle DIO 변수

        #region DO 출력부
        //PCB 클램프 솔
        public static bool[] Front_ClampSol = new bool[13];
        //포고핀 다운 솔
        public static bool[] Front_DownSol = new bool[13];

        public static bool FLift_Stopper = false;   //Front Lift 스토퍼

        public static bool FScan_ClampSol = false;   //스캔층 클램프
        public static bool FPassLine_Stopper = false;   //Front PassLine 스토퍼(3세대엔 없음)

        //모터CW,CCW 상태
        public static bool[,] Front_Motor = new bool[13, 2];   //모터CW,CCW 상태

        public static bool[,] FLift_UPMotor = new bool[1, 2];   //Front 리프트 cw,ccw

        public static bool[,] FScan_Motor = new bool[1, 2];   //스캔층 cw,ccw

        public static bool FPassLine_Motor = false;   //패스라인 cw,ccw


        #endregion
        #region Di 출력부
        int startNum = (int)enumTeachingPos.Site1_F_DT1;
        //PCB층별 감지 센서
        public static bool[] Front_PCB_Sensor = new bool[13];

        //포고핀 업 센서
        public static bool[] Front_Up_Sensor = new bool[13];

        public static bool FLift_UpStopper_Sensor = false;   //Lift 스토퍼 업센서(Down은 고정)

        public bool FLift_UpPCB_IN_Sensor = false;   //Lift Up PCB 진입센서
        public bool FLift_UpPCB_Stop_Sensor = false;   //Lift Up PCB 정지센서
        public bool FLift_DownPCB_IN_Sensor = false;   //Lift Up PCB 진입센서
        public bool FLift_DownPCB_Stop_Sensor = false;   //Lift Up PCB 정지센서

        public bool Front_PassLine_PCB_Sensor = false;  //Front Rack 인터락 센서

        public static bool Front_Interlock_Sensor = false;  //Front Rack 인터락 센서
        #endregion

        public string Name = "";

        #endregion

        /** @brief 타임아웃 체크할때 어디서 문제 생겼는지 내용 저장용 */
        //에러 내용 저장용, 타임
        public string Log = "";

        private FuncInline.enumErrorCode errorCode = FuncInline.enumErrorCode.No_Error;
        private FuncInline.enumErrorPart errorPart = FuncInline.enumErrorPart.No_Error;
        //중복로그 방지용 플레그
        private bool isLogWritten = false;

        //서보init 완료시 true 시작시 false
        private bool InitServo = false;


        #endregion



        /** @brief 생성자 */
        public FrontRack()
        {

            // 쓰레드를 시작한다
            actionThread = new Thread(ActionThread);
            actionThread.Start();

            Name = $"[Front Rack]"; // 영문 로그 이름 설정
        }

        /** @brief 소멸자 */
        ~FrontRack()
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

                    UpdateFrontAllStatus();    //층별센서,출력상태 확인
                    UpdateFrontETCStatus();

                    enumSMDStatus LiftStatus = FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus;
                    enumSMDStatus PassLineStatus = FuncInline.PCBInfo[(int)enumTeachingPos.FrontPassLine].PCBStatus;
                    enumSMDStatus ScanSiteStatus = FuncInline.PCBInfo[(int)enumTeachingPos.FrontScanSite].PCBStatus;

                    int PcbLiftID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Num;
                    String logPcbLiftID = $"[PCB_ID:{PcbLiftID}]";
                    int PcbPassLineID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].Num;
                    String logPassLineID = $"[PCB_ID:{PcbPassLineID}]";
                    int PcbScanID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontScanSite].Num;
                    String logPcbScanID = $"[PCB_ID:{PcbScanID}]";

                    #endregion

                    #region 시스템 상태 따라
                    Name = "[Front Rack]";
                    switch (Action)
                    {
                        case enumAction.Waiting:
                            #region Case Waiting
                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                            {
                                //테스트 Site에서 테스트 완료된 보드가 있을경우 Lift Move하고 Loading 받아야함
                                //동작 우선순위 뒷설비로 배출 가능할경우 1.Test_Fail 2.Test_Pass 3.ReTest 
                                //inShuttle,PassLine에서 먼저 Front로 가야하는 Destination이 지정됐을경우 해당 조건 PASS 되도록

                                // 0. 리프트가 비어있어야 사이트에서 꺼내올 수 있음
                                if (LiftStatus == enumSMDStatus.UnKnown &&
                                    PassLineStatus == enumSMDStatus.UnKnown)
                                {
                                    // InShuttle에 제품이 있고 && 그 목적지가 내쪽(FrontPassLine)인 경우
                                    //    (곧 들어올 예정이니 리프트를 움직이지 말고 대기하여 입고 우선순위 확보)
                                    bool isIncomingFromShuttle = FuncInline.PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus != enumSMDStatus.UnKnown &&
                                                                 (AutoInline.Class.InShuttle.Action == InShuttle.enumAction.MoveFrontPos ||
                                                                 AutoInline.Class.InShuttle.Action == InShuttle.enumAction.FrontUnLoading ||
                                                                 AutoInline.Class.InShuttle.Action == InShuttle.enumAction.FrontUnLoadingCheck);

                                    if (!isIncomingFromShuttle)
                                    {
                                        // === 테스트 완료된 사이트 탐색 로직 ===
                                        enumTeachingPos targetSite = enumTeachingPos.None;
                                        int currentPriority = 99; // 낮을수록 높은 우선순위 (1:Fail, 2:Pass, 3:ReTest)

                                        // 1. Site 1 ~ 13 순회
                                        int startSite = (int)enumTeachingPos.Site1_F_DT1;
                                        int endSite = (int)enumTeachingPos.Site13_F_FT3;

                                        for (int i = startSite; i <= endSite; i++)
                                        {
                                            enumTeachingPos sitePos = (enumTeachingPos)i;
                                            int siteIdx = i - startSite;

                                            // 사이트가 Waiting 상태이고(테스트 끝남), PCB 정보가 있을 때
                                            if (FuncInline.SiteAction[siteIdx] == FuncInline.enumSiteAction.Unloading &&
                                                FuncInline.PCBInfo[i].PCBStatus != enumSMDStatus.UnKnown)
                                            {
                                                int thisPriority = 0;
                                                var status = FuncInline.PCBInfo[i].PCBStatus;

                                                // 우선순위 결정
                                                if (status == enumSMDStatus.Test_Fail) thisPriority = 1;      // 1순위: Fail
                                                else if (status == enumSMDStatus.Test_Pass) thisPriority = 2; // 2순위: Pass
                                                else if (status == enumSMDStatus.ReTest) thisPriority = 3;    // 3순위: ReTest
                                                else if (status == enumSMDStatus.DTest_AllFail) thisPriority = 4;    // 4순위: DTest_AllFail
                                                else if (status == enumSMDStatus.DTest_Fail &&
                                                    PCBInfo[(int)enumTeachingPos.FrontScanSite].PCBStatus == enumSMDStatus.UnKnown) thisPriority = 5;    // 5순위: DTest_Fail, ScanSite 비어있으면
                                                else if (status == enumSMDStatus.DTest_Pass &&
                                                    PCBInfo[(int)enumTeachingPos.FrontScanSite].PCBStatus == enumSMDStatus.UnKnown) thisPriority = 6;    // 6순위: DTest_Pass, ScanSite 비어있으면
                                                else continue; // 그 외 상태(Testing 등)는 패스

                                                // 더 높은 우선순위(낮은 값)를 발견하면 타겟 갱신
                                                if (thisPriority < currentPriority)
                                                {
                                                    currentPriority = thisPriority;
                                                    targetSite = sitePos;
                                                }
                                            }
                                        }

                                        // 2. Scan Site 확인 (스캔 완료된 보드 배출)
                                        // ScanSite도 Waiting이고 데이터가 있다면 우선순위 비교 (보통 Fail급으로 높게 처리하거나 별도 처리)
                                        if (ScanSiteAction == enumScanSiteAction.UnLoading &&
                                            ScanSiteStatus != enumSMDStatus.UnKnown &&
                                            IsFrontFTSiteAvailable())   //펑션층 비어있는지 확인
                                        {
                                            int scanPriority = 4; // 스캔 완료는 4순위와 동급으로 처리 (병목 방지)
                                            if (scanPriority < currentPriority)
                                            {
                                                currentPriority = scanPriority;
                                                targetSite = enumTeachingPos.FrontScanSite;
                                            }
                                        }

                                        // 3. 결정된 곳이 있으면 이동 지령
                                        if (targetSite != enumTeachingPos.None)
                                        {
                                            //NextDestination = targetSite; // 가야할 곳 저장
                                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination = targetSite;

                                            Log = $"{Name}MoveLift Action -> {targetSite}";
                                            FuncLog.WriteLog(Log);

                                            Action = enumAction.MoveLift; // MoveLift 상태로 전환
                                        }
                                        else
                                        {
                                            if (FuncInlineMove.IsArrived(SV02_Lift1, LiftPos[Front, (int)enumLiftPos.FrontPassLine]))
                                            {
                                                //여기선 대기중 아무것도 안함
                                            }
                                            else
                                            {
                                                Log = $"{Name}MoveLift Action -> Passline Wait";
                                                FuncLog.WriteLog(Log);

                                                Action = enumAction.MovePassline; // MoveLift 상태로 전환
                                            }

                                        }
                                    }
                                }
                                // 1. [PassLine -> Lift] 로딩
                                // PassLine에 제품이 있고(UnLoading상태), Lift가 비어있을 때, 테스트Site가 비어 있을때 
                                else if (PassLineStatus != enumSMDStatus.UnKnown &&
                                    //PasslineAction == enumPassLineAction.UnLoading &&
                                    LiftStatus == enumSMDStatus.UnKnown)
                                {
                                    if (FuncInlineMove.IsArrived((int)SV02_Lift1, LiftPos[Front, (int)enumLiftPos.FrontPassLine]))
                                    {
                                        Log = $"{Name}FrontPassLine Move OK -> Loading Action";
                                        FuncLog.WriteLog(Log);
                                        FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination = enumTeachingPos.FrontPassLine;
                                        Action = enumAction.Loading; //PassLine->Lift 투입 시작
                                    }
                                    else
                                    {
                                        Log = $"{Name}PCB IN -> Move MovePassline Position";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.MovePassline;
                                    }

                                }

                                // 2. [Lift/Passline -> Site/Scan/Outshuttle] 투입 (Lift에 제품이 있고 목적지가 있을 때)
                                else if (LiftStatus != enumSMDStatus.UnKnown &&
                                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination != enumTeachingPos.None)
                                {
                                    var destination = FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination;

                                    //1. ScanSite로 가야 할 경우
                                    if (destination == enumTeachingPos.FrontScanSite)
                                    {
                                        Log = $"{Name}{logPcbLiftID}PCB IN -> Move ScanSite Position";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.MoveScanSite;
                                    }
                                    // 2.Front Site (테스트 사이트)로 가야 할 경우
                                    // Site1_F_DT1 부터 Site13_F_FT3 사이의 값이면 MoveLift로 이동
                                    else if (destination >= enumTeachingPos.Site1_F_DT1 && destination <= enumTeachingPos.Site13_F_FT3)
                                    {
                                        Log = $"{Name}{logPcbLiftID}PCB IN -> Move Site Position ({destination})";
                                        FuncLog.WriteLog(Log);

                                        // 이동할 목적지(Site) 정보를 멤버변수 등에 저장해두면 MoveLift 단계에서 사용 가능
                                        // 예: NextDestination = destination; (이미 PCBInfo에 있으므로 거기서 가져다 써도 됨)

                                        Action = enumAction.MoveLift;
                                    }
                                    // 3. 패스모드(Bypass) 이거나 PASS 판정인 경우 -> 상단 배출
                                    else if (LiftStatus == enumSMDStatus.Bypass)
                                    {
                                        if (PCBInfo[(int)enumTeachingPos.Lift1_Up].TestPass)
                                        {
                                            Log = $"{Name}{logPcbLiftID}[TestPass] Move OutShuttleUp Action";
                                        }
                                        else
                                        {
                                            Log = $"{Name}{logPcbLiftID}[PsssMode] Move OutShuttleUp Action";
                                        }
                                        PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination = enumTeachingPos.OutShuttle_Up;
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.MoveOutUp; // 패스 PCB 배출위치로 이동
                                    }
                                    // 4. 테스트 PASS (또는 NG->PASS 옵션) -> 상단 배출
                                    else if (LiftStatus == enumSMDStatus.Test_Pass ||
                                            (FuncInline.NGToUnloading && LiftStatus == enumSMDStatus.Test_Fail))
                                    {
                                        Log = $"{Name}{logPcbLiftID}[{LiftStatus}]PCB IN -> Move OutSuttleUp Position(NG to PassLine)";
                                        FuncLog.WriteLog(Log);
                                        PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination = enumTeachingPos.OutShuttle_Up;
                                        Action = enumAction.MoveOutUp;
                                    }
                                    // 5. Fail PCB -> 하단 배출
                                    else if (LiftStatus == enumSMDStatus.Test_Fail)
                                    {
                                        Log = $"{Name}{logPcbLiftID}[{LiftStatus}]PCB IN -> Move OutSuttleDown Position";
                                        FuncLog.WriteLog(Log);
                                        PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination = enumTeachingPos.OutShuttle_Down;
                                        Action = enumAction.MoveOutDown;
                                    }
                                    // 6. 그 외 (알 수 없는 상태 등) -> 하단 배출 (안전 처리)
                                    else
                                    {
                                        Log = $"{Name}{logPcbLiftID}[{LiftStatus}]PCB IN(unknown state) -> Move OutSuttleDown Position";
                                        FuncLog.WriteLog(Log);
                                        PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination = enumTeachingPos.OutShuttle_Down;
                                        Action = enumAction.MoveOutDown;
                                    }

                                }
                                else
                                {
                                    if (PassLineStatus == FuncInline.enumSMDStatus.UnKnown &&
                                        LiftStatus == FuncInline.enumSMDStatus.UnKnown &&
                                    !FuncInlineMove.IsArrived(SV02_Lift1, LiftPos[Front, (int)enumLiftPos.FrontPassLine]))
                                    {
                                        Action = enumAction.MovePassline;
                                    }
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
                                Log = $"{Name} {Enum.GetName(typeof(enumAction), Action)} -> Waiting";
                                FuncLog.WriteLog(Log);
                                Action = enumAction.Waiting;
                                break;
                            }

                            Util.InitWatch(ref watch);
                            break;
                        #endregion
                        case enumAction.Init:
                            #region Case Init
                            // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.FrontLift] = true;

                            // Front 시작(Site1) ~ 끝(Site13) 범위 설정
                            int startPos = (int)enumTeachingPos.Site1_F_DT1;
                            int endPos = (int)enumTeachingPos.Site13_F_FT3;

                            // Site1_F_DT1(15) 부터 Site13_F_FT3(27) 까지 순차 반복
                            for (int i = startPos; i <= endPos; i++)
                            {
                                // 1. 현재 Site Enum 변환
                                enumTeachingPos currentSite = (enumTeachingPos)i;

                                // 2. 배열 인덱스 계산 (0 ~ 12)
                                int index = i - startPos;

                                // (안전장치) 배열 범위 넘어가면 중단
                                if (index >= Front_PCB_Sensor.Length) break;


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
                            startPos = (int)enumInitialize.Site1_F_DT1;
                            endPos = (int)enumInitialize.Site13_F_FT3;

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
                            FuncInline.InitialDone[(int)enumInitialize.FrontLift] = FuncInlineAction.CheckOriginDone(enumInitialize.FrontLift);
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.FrontLift] = !FuncInline.InitialDone[(int)enumInitialize.FrontLift];
                            //Lift 호밍
                            if (FuncInline.InitialDone[(int)enumInitialize.FrontLift])
                            {
                                if (InitServo == false)
                                {
                                    InitServo = true;
                                    Log = $"{Name} Init - Servo Home Finish";
                                    FuncLog.WriteLog(Log);
                                }

                                Log = $"{Name} Init Finish";
                                FuncLog.WriteLog(Log);
                                // 모든 실린더 후진 확인 되면 완료
                                Action = enumAction.InitFinish;
                            }
                            else
                            {
                                if (!GlobalVar.AxisStatus[SV02_Lift1].isHomed &&
                                  GlobalVar.AxisStatus[SV02_Lift1].StandStill &&
                                  !GlobalVar.AxisStatus[SV02_Lift1].Homing)
                                {
                                    Log = $"{Name} Init - SV02_Lift1 Home Move Start";
                                    FuncLog.WriteLog(Log);
                                    FuncMotion.MoveHome((uint)SV02_Lift1);
                                }

                                if (!GlobalVar.AxisStatus[SV03_Rack1_Width].isHomed &&
                                    GlobalVar.AxisStatus[SV03_Rack1_Width].StandStill &&
                                  !GlobalVar.AxisStatus[SV03_Rack1_Width].Homing)
                                {
                                    Log = $"{Name} Init - SV03_Rack1_Width Home Move Start";
                                    FuncLog.WriteLog(Log);
                                    FuncMotion.MoveHome((uint)SV03_Rack1_Width);
                                }
                            }






                            //호밍중에 센서 감지 안되면 바로 정지 지령( 안전문제)
                            if (Front_Interlock_Sensor)
                            {
                                Log = $"{Name} Init -  Interlock_ Sensor PCB Detected, Servo Stop";
                                FuncMotion.MoveStop(SV02_Lift1); //정지상태 되면 서보 정지
                                FuncMotion.MoveStop(SV03_Rack1_Width); //정지상태 되면 서보 정지
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
                        FuncMotion.MoveStop(SV02_Lift1); //정지상태 되면 서보 정지
                        FuncMotion.MoveStop(SV03_Rack1_Width); //정지상태 되면 서보 정지
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
                        var dest = PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination;

                        //SiteAction 인덱스
                        int siteIdxLocal = (int)dest - (int)enumTeachingPos.Site1_F_DT1;
                        //SiteLiftPos 인덱스
                        int sitePosindex = (int)dest - (int)enumTeachingPos.Site1_F_DT1 + (int)enumLiftPos.Site1_F_DT1_Up;

                        double targetPos = -9999;
                        // 1. 상태 변경 감지 및 타이머 리셋
                        if (Action != beforeAction)
                        {
                            watch.Restart();
                            beforeAction = Action;
                        }

                        // 2. 타임아웃 체크 (Loading/UnLoading 관련 상태일 때만)
                        bool isCheckState = ((Action >= enumAction.MoveLift && Action <= enumAction.MoveOutDown) ||
                                             Action == enumAction.UnLoading || Action == enumAction.UnLoadingCheck ||
                                             Action == enumAction.Loading || Action == enumAction.LoadingCheck);

                        if (isCheckState && watch.ElapsedMilliseconds > ActionTimeout)
                        {
                            watch.Stop(); // 타임아웃 발생 시 타이머 정지
                            if ((Action >= enumAction.MoveLift && Action <= enumAction.MoveOutDown))
                            {
                                FuncInline.AddError(FuncInline.enumErrorPart.Lift1_Up, FuncInline.enumErrorCode.MoveFail,
                                    $"{Log}{Action.ToString()} Servo Move Timeout.");
                            }

                            else if (Action == enumAction.UnLoading || Action == enumAction.UnLoadingCheck)
                            {
                                if (dest == enumTeachingPos.FrontScanSite)
                                {
                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift1_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} UnLoading Timeout.");
                                    FuncInline.AddError(FuncInline.enumErrorPart.FrontScanSite, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} Loading Timeout.");
                                }
                                else if (dest == enumTeachingPos.OutShuttle_Up)
                                {
                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift1_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} UnLoading Timeout.");
                                    FuncInline.AddError(FuncInline.enumErrorPart.OutShuttle_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} Loading Timeout.");
                                }
                                else if (dest == enumTeachingPos.OutShuttle_Down)
                                {
                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift1_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} UnLoading Timeout.");
                                    FuncInline.AddError(FuncInline.enumErrorPart.OutShuttle_Down, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} Loading Timeout.");
                                }
                                else
                                {
                                    FuncInline.enumErrorPart errorPart = siteIdxLocal + FuncInline.enumErrorPart.Site1_F_DT1;

                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift1_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                    $"{Log} UnLoading Timeout.");
                                    FuncInline.AddError(errorPart, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} Loading Timeout.");

                                    FuncInline.SiteAction[siteIdxLocal] = enumSiteAction.Waiting;
                                }

                            }
                            else if (Action == enumAction.Loading || Action == enumAction.LoadingCheck)
                            {
                                if (dest == enumTeachingPos.FrontScanSite)
                                {
                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift1_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} Loading Timeout.");
                                    FuncInline.AddError(FuncInline.enumErrorPart.FrontScanSite, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} UnLoading Timeout.");

                                    ScanSiteAction = enumScanSiteAction.Waiting;
                                }
                                else if (dest == enumTeachingPos.FrontPassLine)
                                {
                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift1_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} Loading Timeout.");
                                    FuncInline.AddError(FuncInline.enumErrorPart.FrontPassLine, FuncInline.enumErrorCode.Conveyor_Timeout,
                                $"{Log} UnLoading Timeout.");

                                    PasslineAction = enumPassLineAction.Waiting;
                                }

                                else
                                {
                                    FuncInline.enumErrorPart errorPart = siteIdxLocal + FuncInline.enumErrorPart.Site1_F_DT1;

                                    FuncInline.AddError(FuncInline.enumErrorPart.Lift1_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                    $"{Log} Loading Timeout.");
                                    FuncInline.AddError(errorPart, FuncInline.enumErrorCode.Conveyor_Timeout,
                                    $"{Log} UnLoading Timeout.");

                                    FuncInline.SiteAction[siteIdxLocal] = enumSiteAction.Waiting;
                                }

                            }
                            else // Loading 등 기타
                            {
                                FuncInline.AddError(FuncInline.enumErrorPart.InShuttle, FuncInline.enumErrorCode.Conveyor_Timeout,
                                   $"{Log} Action Timeout.");
                            }

                            // 초기화 및 대기
                            Action = enumAction.Waiting;
                            continue; // switch문 실행 안 하고 다음 루프로
                        }


                        //PassLine
                        switch (PasslineAction)
                        {

                            case enumPassLineAction.Waiting:
                                #region Waiting
                                // InShuttle이 보낼 준비가 되면 Loading 시작
                                if (PassLineStatus == FuncInline.enumSMDStatus.UnKnown &&
                                    !Front_PassLine_PCB_Sensor &&
                                    AutoInline.Class.InShuttle.Action == InShuttle.enumAction.FrontUnLoading)
                                {
                                    Log = $"{Name}[PassLine]PCB Loding Action";
                                    FuncLog.WriteLog(Log);

                                    DIO.WriteDOData(enumDONames.Y1_7_Front_PASSLINE_PCB_STOPPER_SOL, true); //5세대? 이상만 핀 스토퍼 있는걸로 알고있음, 미리 막는다

                                    PasslineAction = enumPassLineAction.Loading;
                                }

                                else if (PassLineStatus != FuncInline.enumSMDStatus.UnKnown &&
                                    Front_PassLine_PCB_Sensor &&
                                    FuncInlineMove.IsArrived(SV02_Lift1, FuncInline.LiftPos[Front, (int)enumLiftPos.FrontPassLine]) &&
                                    Action == enumAction.Loading)   //리프트 액션이 로딩일때
                                {
                                    Log = $"{Name}[PassLine]{logPassLineID}PassLine PCB UnLoding Action";
                                    FuncLog.WriteLog(Log);

                                    DIO.WriteDOData(enumDONames.Y1_7_Front_PASSLINE_PCB_STOPPER_SOL, false); //5세대? 이상만 핀 스토퍼 있는걸로 알고있음, 클램프 해제 
                                    PasslineAction = enumPassLineAction.UnLoading;
                                }
                                else
                                {

                                    //PCB를 감지하는데 데이터가 없거나, PCB감지 안하고 데이터가 있거나 하면 타임아웃? 바로 에러
                                    if ((PassLineStatus == FuncInline.enumSMDStatus.UnKnown &&
                                        Front_PassLine_PCB_Sensor) ||
                                        (PassLineStatus != FuncInline.enumSMDStatus.UnKnown &&
                                        !Front_PassLine_PCB_Sensor))
                                    {
                                        FuncInline.AddError(FuncInline.enumErrorPart.FrontPassLine,
                                        FuncInline.enumErrorCode.PCB_Detect_Fail, // 또는 PCB_Info_Move_Fail
                                        $"{Name}[PassLine]{logPassLineID} FrontUnLoading Timeout. Check PCB/Sensors.");
                                    }
                                    else
                                    {
                                        Util.ResetWatch(ref watch);
                                    }

                                }
                                break;
                            #endregion

                            case enumPassLineAction.Loading:
                                #region Loading (InShuttle -> PassLine)
                                // 모터 구동
                                DIO.WriteDOData(enumDONames.Y404_1_Front_Passline_Motor_Cw, true);

                                // 센서 감지 시 일시 정지
                                if (Front_PassLine_PCB_Sensor)
                                {
 
                                    Log = $"{Name}[PassLine] LoadingCheck Action";
                                    FuncLog.WriteLog(Log);
                                    PasslineAction = enumPassLineAction.LoadingCheck;
                                    DIO.WriteDOData(enumDONames.Y404_1_Front_Passline_Motor_Cw, false);

                                  
                                }

                                break;
                            #endregion

                            case enumPassLineAction.LoadingCheck:
                                #region LoadingCheck
                                //잠시 대기후 데이터 전달
                                if (FuncInline.IsDelayOver(Key_PassLine_Load, 500))
                                {
                                    // 데이터 이동 (InShuttle -> FrontPassLine)
                                    // InShuttle에서 데이터를 가져와서 내 자리에 채움
                                    if (AutoInline.Class.InShuttle.Action == InShuttle.enumAction.FrontUnLoadingCheck)
                                    {
                                        FuncInline.MovePCBInfo(enumTeachingPos.InShuttle, enumTeachingPos.FrontPassLine);
                                    }
                                    // 데이터가 잘 들어왔는지 확인
                                    if (PassLineStatus != enumSMDStatus.UnKnown &&
                                        PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus == enumSMDStatus.UnKnown)
                                    {
                                        Log = $"{Name}[PassLine][PCB_ID:{PCBInfo[(int)enumTeachingPos.FrontPassLine].Num}] PassLine Loading Complete";
                                        FuncLog.WriteLog(Log);
                                        PasslineAction = enumPassLineAction.Waiting;
                                    }
                                }
                            
                                break;
                            #endregion
                            case enumPassLineAction.UnLoading:
                                #region UnLoading (PassLine -> Lift)
                                // 리프트가 가져가는 중 (리프트 모터와 함께 돔)
                                // 리프트 쪽에서 제어하므로 여기선 모터만 켜주거나 상태만 유지
                                if (!FPassLine_Motor)
                                {
                                    DIO.WriteDOData(enumDONames.Y404_1_Front_Passline_Motor_Cw, true);
                                }


                                // PassLine 센서가 꺼지면 (리프트로 넘어감), Lift LoadingCheck 상태면  check상태로
                                if (!Front_PassLine_PCB_Sensor && (Action == enumAction.LoadingCheck || FLift_UpPCB_Stop_Sensor))
                                {
                                    //// 조금 더 돌려주고 끔 (리프트 진입 확실히)
                                    //if (FuncInline.IsDelayOver(Key_PassLine_Unload, 1000))
                                    //{
                                    Log = $"{Name}[PassLine][PCB_ID:{PCBInfo[(int)enumTeachingPos.FrontPassLine].Num}] PassLine UnLoading Action";
                                    FuncLog.WriteLog(Log);

                                    DIO.WriteDOData(enumDONames.Y404_1_Front_Passline_Motor_Cw, false);
                                    PasslineAction = enumPassLineAction.UnLoadingCheck;
                                    //}
                                }
                                else
                                {
                                    FuncInline.ResetDelay(Key_PassLine_Unload);
                                }
                                break;
                            #endregion

                            case enumPassLineAction.UnLoadingCheck:
                                #region UnLoadingCheck
                                // 데이터 이동 확인 (PassLine -> Lift)
                                // Lift 로직에서 데이터를 가져가면 내 데이터는 UnKnown이 됨

                                if (PassLineStatus == enumSMDStatus.UnKnown && LiftStatus != enumSMDStatus.UnKnown)
                                {
                                    Log = $"{Name}[PassLine] PassLine UnLoading Complete";
                                    FuncLog.WriteLog(Log);
                                    PasslineAction = enumPassLineAction.Waiting;
                                }
                                break;
                                #endregion


                        }
                        //ScanSite
                        switch (ScanSiteAction)
                        {

                            case enumScanSiteAction.Waiting:
                                #region Waiting

                                break;
                            #endregion
                            case enumScanSiteAction.Loading:
                                #region Loading


                                break;
                            #endregion
                            case enumScanSiteAction.LoadingCheck:
                                #region WLoadingCheckaiting


                                break;
                            #endregion
                            case enumScanSiteAction.ScanWait:
                                #region ScanWait


                                break;
                            #endregion
                            case enumScanSiteAction.ScanOK:
                                #region ScanOK

                                // 3. 빈 FT 사이트 탐색 및 목적지 설정
                                var targetFT = GetAvailableFrontFTSite();

                                if (targetFT != FuncInline.enumTeachingPos.None)
                                {
                                    // 목적지 설정
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontScanSite].Destination = targetFT;

                                    Log = $"{Name} Scan Complete. Next Destination: {targetFT}";
                                    FuncLog.WriteLog(Log);

                                    // 언로딩으로 전환
                                    ScanSiteAction = enumScanSiteAction.UnLoading;
                                }
                                else
                                {
                                    // 빈 FT가 없으면 대기 (Waiting) 혹은 알람?
                                    // 보통은 여기서 계속 대기하며 빈 자리가 날 때까지 돔
                                    // Log = $"{Name} Scan Complete but No Empty FT Site. Waiting...";
                                }
                                break;
                            #endregion
                            case enumScanSiteAction.UnLoading:
                                #region UnLoading


                                break;
                            #endregion
                            case enumScanSiteAction.UnLoadingCheck:
                                #region UnLoadingCheck

                                break;
                                #endregion


                        }

                        //LiftAction
                        switch (Action)
                        {
                            case enumAction.HomeMove: //에러 발생 후 복귀 동작 후 -> Waiting으로
                                #region HomeMove
                                // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행

                                break;
                            #endregion

                            case enumAction.MovePassline:
                                #region MovePassline
                                //PassLine으로 이동
                                double Pos = LiftPos[Front, (int)enumLiftPos.FrontPassLine];
                                if (GlobalVar.AxisStatus[SV02_Lift1].StandStill)
                                {
                                    Log = $"{Name}[Lift]Move to FrontPassLine Position";
                                    FuncLog.WriteLog(Log);
                                    FuncInlineMove.MoveAbsolute((uint)SV02_Lift1, Pos);
                                }
                                if (FuncInlineMove.IsArrived((int)SV02_Lift1, Pos))
                                {
                                    Log = $"{Name}[Lift]FrontPassLine Move OK ->Loading Action";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.Waiting;
                                }
                                Util.InitWatch(ref watch);
                                break;
                            #endregion
                            case enumAction.MoveOutUp:
                                #region MovePassline

                                Pos = LiftPos[Front, (int)enumLiftPos.OutShuttleUp];
                                if (GlobalVar.AxisStatus[SV02_Lift1].StandStill)
                                {
                                    Log = $"{Name}[Lift]{Action} Position";
                                    FuncLog.WriteLog(Log);
                                    FuncInlineMove.MoveAbsolute((uint)SV02_Lift1, Pos);
                                }
                                if (FuncInlineMove.IsArrived((int)SV02_Lift1, Pos))
                                {
                                    Log = $"{Name}[Lift]{logPcbLiftID}{Action} Lift Move OK ->UnLoading Action";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.UnLoading;
                                }
                                Util.InitWatch(ref watch);
                                break;
                            #endregion
                            case enumAction.MoveOutDown:
                                #region MovePassline
                                //PassLine으로 이동
                                Pos = LiftPos[Front, (int)enumLiftPos.OutShuttleDown];
                                if (GlobalVar.AxisStatus[SV02_Lift1].StandStill)
                                {
                                    Log = $"{Name}[Lift]{Action} Position";
                                    FuncLog.WriteLog(Log);
                                    FuncInlineMove.MoveAbsolute((uint)SV02_Lift1, Pos);
                                }
                                if (FuncInlineMove.IsArrived((int)SV02_Lift1, Pos))
                                {
                                    Log = $"{Name}[Lift]{logPcbLiftID}{Action}Lift Move OK ->UnLoading Action";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.UnLoading;
                                }
                                Util.InitWatch(ref watch);
                                break;
                            #endregion
                            case enumAction.MoveScanSite:
                                #region MovePassline
                                //PassLine으로 이동
                                Pos = LiftPos[Front, (int)enumLiftPos.FrontScanPos];
                                if (GlobalVar.AxisStatus[SV02_Lift1].StandStill)
                                {
                                    Log = $"{Name}[Lift]{Action} Position";
                                    FuncLog.WriteLog(Log);
                                    FuncInlineMove.MoveAbsolute((uint)SV02_Lift1, Pos);
                                }
                                if (FuncInlineMove.IsArrived((int)SV02_Lift1, Pos))
                                {
                                    if (PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus == enumSMDStatus.UnKnown)
                                    {
                                        Log = $"{Name}[Lift]{logPcbLiftID}{Action}Lift Move OK ->UnLoading Action";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.UnLoading; //ScanSite UnLoading 받기 시작
                                    }
                                    {
                                        Log = $"{Name}[Lift]{Action}Lift Move OK ->Loading Action";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.Loading; //ScanSite Loading 투입 시작
                                    }

                                }
                                Util.InitWatch(ref watch);
                                break;
                            #endregion
                            case enumAction.MoveLift:
                                #region MoveLift
                                {
                                    dest = PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination;

                                    siteIdxLocal = (int)dest - (int)enumTeachingPos.Site1_F_DT1;
                                    // 목적지 좌표 가져오기
                                    targetPos = -9999;

                                    // A. Lift가 비어있으면 -> Loading 위치로 이동 (PassLine or Site/Scan)
                                    if (PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus == enumSMDStatus.UnKnown)
                                    {

                                        // 1. NextDestination이 설정되어 있으면 Site으로 이동 (Waiting에서 설정한 값)
                                        if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination != enumTeachingPos.None &&
                                            FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination != enumTeachingPos.FrontScanSite)
                                        {
                                            sitePosindex = ((int)dest - (int)enumTeachingPos.Site1_F_DT1) + (int)enumLiftPos.Site1_F_DT1_Up;
                                            targetPos = LiftPos[Front, sitePosindex];
                                        }
                                        else if (FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination == enumTeachingPos.FrontScanSite)
                                        {
                                            targetPos = LiftPos[Front, (int)enumTeachingPos.FrontScanSite];
                                        }

                                        // 이동 지령
                                        if (targetPos != -9999)
                                        {
                                            if (FuncInlineMove.IsArrived((int)SV02_Lift1, targetPos))
                                            {
                                                Log = $"{Name}[Lift]{FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination.ToString()}Move OK -> Loading Action";
                                                FuncLog.WriteLog(Log);

                                                Action = enumAction.Loading;    //Test사이트로 부터 받음

                                            }
                                            if (GlobalVar.AxisStatus[SV02_Lift1].StandStill)
                                            {
                                                Log = $"{Name}[Lift] Move to {FuncInline.PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination.ToString()} Position";
                                                FuncLog.WriteLog(Log);
                                                FuncInlineMove.MoveAbsolute((uint)SV02_Lift1, targetPos);
                                            }

                                        }
                                    }

                                    //Lift에 테스트해야할 제품이 있으면 -> UnLoading 위치로 이동 (Site or Scan)
                                    else
                                    {

                                        if (dest == enumTeachingPos.FrontScanSite)
                                        {
                                            sitePosindex = (int)enumLiftPos.FrontScanPos;
                                            targetPos = LiftPos[Front, sitePosindex];
                                        }
                                        else if (dest == enumTeachingPos.OutShuttle_Up)
                                        {
                                            sitePosindex = (int)enumLiftPos.OutShuttleUp;
                                            targetPos = LiftPos[Front, sitePosindex];
                                        }
                                        else if (dest == enumTeachingPos.OutShuttle_Down)
                                        {
                                            sitePosindex = (int)enumLiftPos.OutShuttleDown;
                                            targetPos = LiftPos[Front, sitePosindex];
                                        }
                                        else
                                        {
                                            sitePosindex = ((int)dest - (int)enumTeachingPos.Site1_F_DT1) + (int)enumLiftPos.Site1_F_DT1_Up;
                                            targetPos = LiftPos[Front, sitePosindex];
                                        }
                                        // 이동 지령
                                        if (targetPos != -9999)
                                        {
                                            if (GlobalVar.AxisStatus[SV02_Lift1].StandStill)
                                            {
                                                Log = $"{Name}[Lift]{Action} Move to {(enumTeachingPos)sitePosindex} Position";
                                                FuncLog.WriteLog(Log);
                                                FuncInlineMove.MoveAbsolute((uint)SV02_Lift1, targetPos);
                                            }

                                            if (FuncInlineMove.IsArrived((int)SV02_Lift1, targetPos))
                                            {
                                                Log = $"{Name}[Lift]{logPcbLiftID}{Action}{(enumTeachingPos)sitePosindex} Move OK -> UnLoading Action";
                                                FuncLog.WriteLog(Log);
                                                Action = enumAction.UnLoading; // 투입 시작
                                            }
                                        }
                                    }



                                }
                                Util.InitWatch(ref watch);
                                break;
                            #endregion

                            case enumAction.Loading:
                                #region Loading (PassLine/Site/Scan -> Lift)
                                Util.InitWatch(ref watch);
                                Stopper_Open(false);
                                dest = PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination;

                                siteIdxLocal = (int)dest - (int)enumTeachingPos.Site1_F_DT1;
                                sitePosindex = ((int)dest - (int)enumTeachingPos.Site1_F_DT1) + (int)enumLiftPos.Site1_F_DT1_Up;

                                // 1. PassLine에서 로딩
                                if (dest == enumTeachingPos.FrontPassLine)
                                {
                                    if (PasslineAction == enumPassLineAction.UnLoading ||
                                        PasslineAction == enumPassLineAction.UnLoadingCheck)
                                    {
                                        if (LiftupLoadingAction())
                                        {
                                            Log = $"{Name}[Lift]PassLine->LiftUp Loading Finish, LoadingCheck Action ";
                                            FuncLog.WriteLog(Log);
                                            Action = enumAction.LoadingCheck;
                                            break;
                                        }
                                    }

                                }
                                // 2. ScanStie에서 로딩
                                if (dest == enumTeachingPos.FrontScanSite)
                                {
                                    //한번더 체크
                                    if (ScanSiteAction == enumScanSiteAction.UnLoading ||
                                        ScanSiteAction == enumScanSiteAction.UnLoadingCheck)
                                    {
                                        if (LiftupLoadingAction())
                                        {
                                            Log = $"{Name}[Lift]ScanSite->LiftUp Loading Finish, LoadingCheck Action ";
                                            FuncLog.WriteLog(Log);
                                            Action = enumAction.LoadingCheck;
                                            break;
                                        }
                                    }
                                }
                                if (dest == enumTeachingPos.FrontPassLine || dest == enumTeachingPos.FrontScanSite) break;

                                targetPos = LiftPos[Front, sitePosindex];
                                // 3. Test Site에서 로딩
                                if (FuncInline.SiteAction[siteIdxLocal] == enumSiteAction.Unloading &&
                                     FuncInlineMove.IsArrived((int)SV02_Lift1, targetPos))
                                {
                                    if (LiftupLoadingAction())
                                    {
                                        Log = $"{Name}[Lift][{(enumTeachingPos)dest}] Site->LiftUp Loading Finish, LoadingCheck Action ";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.LoadingCheck;
                                    }

                                }
                                //Util.InitWatch(ref watch);
                                break;
                            #endregion

                            case enumAction.LoadingCheck:
                                #region Loading (PassLine/Site -> Lift)

                                // 0. 소스 위치 확인 (Lift가 어디서 가져오기로 했는지)
                                // Waiting 상태에서 Destination에 '가져올 위치'를 저장해두었음
                                var sourcePos = PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination;

                                //잠시 대기하고 데이터 이동
                                if (FuncInline.IsDelayOver(Key_PassLine_Load, 500))
                                {
                                    // 2. [Data Move] 데이터 이동 실행 (Source -> Lift)
                                    // MovePCBInfo 내부에서 Source가 비어있지 않을 때만 이동하도록 되어 있다고 가정하거나,
                                    FuncInline.MovePCBInfo(sourcePos, enumTeachingPos.Lift1_Up);
                                }
                                // 3. [Logical Check] 데이터 이동 완료 확인
                                // Source는 비워졌는지(UnKnown), Lift는 채워졌는지(!UnKnown) 확인
                                bool isSourceEmpty = PCBInfo[(int)sourcePos].PCBStatus == enumSMDStatus.UnKnown;
                                bool isLiftOccupied = PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus != enumSMDStatus.UnKnown;

                                if (isSourceEmpty && isLiftOccupied)
                                {
                                    Log = $"{Name}[Lift][PCB_ID:{PCBInfo[(int)enumTeachingPos.Lift1_Up].Num}]PassLine Loading Complete from [{sourcePos}]";
                                    FuncLog.WriteLog(Log);

                                    // 4. 목적지 초기화 (이제 Lift가 가지고 있으므로 Source 정보는 불필요)
                                    // 단, 다음 갈 곳(OutShuttle 등)을 정해야 한다면 유지하거나 Waiting에서 다시 판단

                                    Action = enumAction.Waiting;
                                }

                                //Util.InitWatch(ref watch);
                                break;
                            #endregion
                            case enumAction.UnLoading:
                                #region UnLoading (Lift -> Site/Scan/OutUp/OutDown)
                                {

                                    // 목적지 확인
                                    dest = PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination;

                                    // Site 인덱스 계산 (Test Site용)
                                    siteIdxLocal = (int)dest - (int)enumTeachingPos.Site1_F_DT1;

                                    int outshuttle = (int)enumShuttleName.OutShuttle;
                                    // 타겟 위치 계산 (도착 확인용)
                                    // ScanSite, OutShuttle 등은 별도 위치이므로 TestSite 범위일 때만 유효할 수 있음
                                    // 여기서는 도착 여부는 이미 Move 단계에서 확인했다고 가정하고,
                                    // 상대방의 상태(Waiting/Loading)를 확인하여 배출함.

                                    // 1. ScanSite로 투입
                                    if (dest == enumTeachingPos.FrontScanSite)
                                    {
                                        // ScanSite가 받을 준비(Waiting) 되었는지 확인
                                        if (ScanSiteAction == enumScanSiteAction.Waiting ||
                                             ScanSiteAction == enumScanSiteAction.Loading ||
                                             ScanSiteAction == enumScanSiteAction.LoadingCheck)
                                        {
                                            if (ScanSiteAction != enumScanSiteAction.Loading)
                                            {
                                                // 상대방 로딩 시작 신호 (필요시)
                                                ScanSiteAction = enumScanSiteAction.Loading;
                                            }
                                            
                                            // 배출 동작 수행
                                            if (LiftUnLoadingSiteAction())
                                            {
                                                Log = $"{Name}[Lift]{logPcbLiftID} Lift->ScanSite UnLoading Finish";
                                                FuncLog.WriteLog(Log);
                                                Action = enumAction.UnLoadingCheck;
                                            }
                                        }
                                        else
                                        {
                                            //배출 대기중일때 정지
                                            watch.Restart();
                                        }
                                    }
                                    // 2. OutShuttle (Pass/Fail) 투입, 투입 위치에 있으면
                                    else if ((dest == enumTeachingPos.OutShuttle_Up || dest == enumTeachingPos.OutShuttle_Down) &&
                                       FuncInlineMove.IsArrived(outshuttle, FuncInline.ShuttlePos[outshuttle, (int)enumShuttlePos.OutShuttle_FrontLiftLoading]))
                                    {
                                        if (AutoInline.Class.OutShuttle.OutShuttleAction == OutShuttle.OutShuttle_enumAction.FrontLoading ||
                                            AutoInline.Class.OutShuttle.OutShuttleAction == OutShuttle.OutShuttle_enumAction.FrontLoadingCheck)
                                        {
                                            // (추가: OutShuttle이 올바른 층에 있는지 확인 필요)
                                            // if (isOutShuttleReady && IsOutShuttleAtLiftPos())
                                            Stopper_Open(true);
                                            //if (PCBInfo[(int)enumTeachingPos.OutShuttle_Up].Destination == enumTeachingPos.Lift1_Up)
                                            if (LiftUnLoadingOutShuttleAction())
                                            {
                                                string destName = (dest == enumTeachingPos.OutShuttle_Up) ? "OutShuttleUp" : "OutShuttleDown";
                                                Log = $"{Name}[Lift]{logPcbLiftID} Lift->{destName} UnLoading Finish";
                                                FuncLog.WriteLog(Log);
                                                Action = enumAction.UnLoadingCheck;
                                            }
                                        }
                                        else
                                        {
                                            //배출 대기중일때 정지
                                            watch.Restart();
                                        }
                                        // OutShuttle 상태 확인 (예: Loading 상태이고 내 층에 있는지)
                                        //bool isOutShuttleReady = AutoInline.Class.OutShuttle.OutShuttleAction == OutShuttle.OutShuttle_enumAction.Loading;

                                       

                                    }

                                    // 3. Test Site로 투입
                                    else if (dest >= enumTeachingPos.Site1_F_DT1 && dest <= enumTeachingPos.Site13_F_FT3)
                                    {
                                        // 해당 사이트가 받을 준비(Waiting) 되었는지 확인
                                        if (FuncInline.SiteAction[siteIdxLocal] == enumSiteAction.Waiting &&
                                            PCBInfo[(int)dest].PCBStatus == enumSMDStatus.UnKnown)
                                        {
                                            Log = $"{Name}[Lift]{logPcbLiftID} Lift->Site[{dest}] UnLoading Start";
                                            FuncLog.WriteLog(Log);
                                            // 사이트 로딩 시작 신호
                                            FuncInline.SiteAction[siteIdxLocal] = enumSiteAction.Loading;

                                        }
                                        if (FuncInline.SiteAction[siteIdxLocal] == enumSiteAction.Loading)
                                        {
                                            if (LiftUnLoadingSiteAction())
                                            {
                                                Log = $"{Name}[Lift]{logPcbLiftID} Lift->Site[{dest}] UnLoading Check";
                                                FuncLog.WriteLog(Log);
                                                Action = enumAction.UnLoadingCheck;
                                            }
                                        }
                                        else
                                        {
                                            //배출 대기중일때 정지
                                            watch.Restart();
                                        }
                                    }
                                }
                                //Util.InitWatch(ref watch);
                                break;
                            #endregion

                            case enumAction.UnLoadingCheck:
                                #region UnLoadingCheck (Data Move & Finish)
                                {
                                    // 목적지 재확인
                                    dest = PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination;
                                    // Site 인덱스 계산 (Test Site용)
                                    siteIdxLocal = (int)dest - (int)enumTeachingPos.Site1_F_DT1;

                                    // 1. 데이터 이동 (Lift -> Dest)
                                    FuncInline.MovePCBInfo(enumTeachingPos.Lift1_Up, dest);

                                    // 2. 완료 확인
                                    // 리프트가 비워졌는지 확인
                                    if (PCBInfo[(int)enumTeachingPos.Lift1_Up].PCBStatus == enumSMDStatus.UnKnown &&
                                        PCBInfo[(int)dest].PCBStatus != enumSMDStatus.UnKnown &&
                                        FuncInline.SiteAction[siteIdxLocal] == enumSiteAction.Testing)
                                    {
                                        // 다음 동작 결정 (UnLoading 후에는 보통 빈 리프트이므로 새로운 로딩을 위해 이동하거나 대기)

                                        //혹시 나중에 따로 분기할수도 있으니 나눠놓음
                                        //ScanSite로 보낸 후 -> 리프트는 빈 상태 -> 다음 로딩(PassLine/Scan/Site) 판단을 위해 Waiting으로
                                        if (dest == enumTeachingPos.FrontScanSite)
                                        {
                                            Action = enumAction.Waiting;
                                        }
                                        //OutShuttle로 보낸 후 -> 빈 상태 -> Waiting
                                        else if (dest == enumTeachingPos.OutShuttle_Up || dest == enumTeachingPos.OutShuttle_Down)
                                        {
                                            Action = enumAction.Waiting;
                                            Stopper_Open(false);
                                        }
                                        //Test Site로 보낸 후 -> 빈 상태 -> Waiting
                                        else
                                        {
                                            Action = enumAction.Waiting;
                                        }
                                        Log = $"{Name}[Lift][{dest}] UnLoading Complete. Lift Empty.";
                                        FuncLog.WriteLog(Log);

                                    }
                                }
                                break;
                                #endregion
                        }
                    }
                    #endregion
                    #endregion

                    #region else AutoRun 아닐때
                    //AutoRun이 아닐때 대기
                    else
                    {
                        Util.InitWatch(ref watch);
                    }
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
                    FuncLog.WriteLog($"{Name}.ActionThread : " + ex.ToString());
                    FuncLog.WriteLog($"{Name}.ActionThread : " + ex.StackTrace);
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
        // 센서 상태 업데이트 함수
        // Front 구역의 모든 센서/솔레노이드/모터 상태를 업데이트하는 통합 함수
        public void UpdateFrontAllStatus()
        {
            // Front 시작(Site1) ~ 끝(Site13) 범위 설정
            int startPos = (int)enumTeachingPos.Site1_F_DT1;
            int endPos = (int)enumTeachingPos.Site13_F_FT3;

            // Site1_F_DT1(15) 부터 Site13_F_FT3(27) 까지 순차 반복
            for (int i = startPos; i <= endPos; i++)
            {
                // 1. 현재 Site Enum 변환
                enumTeachingPos currentSite = (enumTeachingPos)i;

                // 2. 배열 인덱스 계산 (0 ~ 12)
                int index = i - startPos;

                // (안전장치) 배열 범위 넘어가면 중단
                if (index >= Front_PCB_Sensor.Length) break;


                // =========================================================
                // 1) PCB Dock 센서 (DI) -> Front_PCB_Sensor
                // =========================================================
                enumDINames diPcbName;
                if (SiteIoMaps.TryGetPcbDockDI(currentSite, out diPcbName))
                {
                    Front_PCB_Sensor[index] = DIO.GetDIData(diPcbName);
                }
                else
                {
                    Front_PCB_Sensor[index] = false;
                }

                // =========================================================
                // 2) 컨택트 스토퍼 솔 (DO) -> Front_ClampSol (클램프)
                // =========================================================
                enumDONames doStopperName;
                if (SiteIoMaps.TryGetContactStopperDO(currentSite, out doStopperName))
                {
                    Front_ClampSol[index] = DIO.GetDOData(doStopperName);
                }
                else
                {
                    Front_ClampSol[index] = false;
                }

                // =========================================================
                // 3) 컨택트 UP/DOWN 솔 (DO) -> Front_DownSol (다운/업 솔)
                // =========================================================
                enumDONames doUpDownName;
                if (SiteIoMaps.TryGetContactUpDownDO(currentSite, out doUpDownName))
                {
                    Front_DownSol[index] = DIO.GetDOData(doUpDownName);
                }
                else
                {
                    Front_DownSol[index] = false;
                }

                // =========================================================
                // 4) 컨택트 UP 센서 (DI) -> Front_Up_Sensor
                // =========================================================
                enumDINames diUpName;
                if (SiteIoMaps.TryGetContactUpDI(currentSite, out diUpName))
                {
                    Front_Up_Sensor[index] = DIO.GetDIData(diUpName);
                }
                else
                {
                    Front_Up_Sensor[index] = false;
                }

                // =========================================================
                // 5) 이송 모터 CW/CCW (DO) -> Front_Motor[index, 0/1]
                //    [index, 0] = CW, [index, 1] = CCW
                // =========================================================
                enumDONames doCW, doCCW;
                if (SiteIoMaps.TryGetSiteMotor(currentSite, out doCW, out doCCW))
                {
                    Front_Motor[index, 0] = DIO.GetDOData(doCW);  // CW
                    Front_Motor[index, 1] = DIO.GetDOData(doCCW); // CCW
                }
                else
                {
                    Front_Motor[index, 0] = false;
                    Front_Motor[index, 1] = false;
                }
            }
        }

        //나머지 리프트, 스캔 쪽 상태 READ
        public void UpdateFrontETCStatus()
        {
            FScan_ClampSol = DIO.GetDOData(enumDONames.Y3_7_Front_SCAN_STOPPER_SOL);   //스캔층 클램프
            FPassLine_Stopper = DIO.GetDOData(enumDONames.Y1_7_Front_PASSLINE_PCB_STOPPER_SOL);   //Front PassLine 스토퍼(3세대엔 없음)
            FPassLine_Motor = DIO.GetDOData(enumDONames.Y404_1_Front_Passline_Motor_Cw);   //패스라인 cw,ccw

            FLift_Stopper = DIO.GetDOData(enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL);   //Front Lift 스토퍼
            //Front 리프트 cw,ccw
            FLift_UPMotor[0, 0] = DIO.GetDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw);
            FLift_UPMotor[0, 1] = DIO.GetDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw);
            //스캔층 cw,ccw
            FScan_Motor[0, 0] = DIO.GetDOData(enumDONames.Y406_1_Front_SCAN_Motor_Cw);
            FScan_Motor[0, 1] = DIO.GetDOData(enumDONames.Y406_3_Front_SCAN_Motor_Ccw);


            // -------------------------------------------------
            FLift_UpStopper_Sensor = DIO.GetDIData(enumDINames.X403_7_Front_Lift_Stopper_Cyl_Sensor);   //Lift 스토퍼 업센서(Down은 고정)
            FLift_UpPCB_IN_Sensor = DIO.GetDIData(enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor);   //Lift Up PCB 진입센서
            FLift_UpPCB_Stop_Sensor = DIO.GetDIData(enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor);   //Lift Up PCB 정지센서
            FLift_DownPCB_IN_Sensor = DIO.GetDIData(enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor);   //Lift Up PCB 진입센서
            FLift_DownPCB_Stop_Sensor = DIO.GetDIData(enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor);   //Lift Up PCB 정지센서

            Front_PassLine_PCB_Sensor = DIO.GetDIData(enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor);  //Front Rack 인터락 센서

            Front_Interlock_Sensor = DIO.GetDIData(enumDINames.X114_7_Front_Rack_PCB_Interlock_Sensor);  //Front Rack 인터락 센서
        }
        private void Stopper_Open(bool ON)
        {
            if (FLift_Stopper != ON)
            {
                //스토퍼 올려줘야 움직일수 있음
                DIO.WriteDOData(enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL, ON);
            }
        }


        /// <summary>
        /// Front Lift Up Loading 동작 (구동 -> 센서감지 -> 정지 -> 오버드라이브 -> 완료)
        /// </summary>
        /// <returns>동작 완료시 true</returns>
        private bool LiftupLoadingAction()
        {
            switch (loadingStep)
            {
                case 0:
                    #region Step 0: 구동 및 도착 확인
                    // 모터 정방향 구동
                    DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, true);
                    DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);

                    // 도착 센서 감지
                    if (FLift_UpPCB_Stop_Sensor)
                    {
                        // 감지 후 300ms 지연 (안정화)
                        if (FuncInline.IsDelayOver(Key_Lift_Load, 300))
                        {
                            // 1차 정지
                            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
                            loadingStep = 10;
                        }
                    }
                    else
                    {
                        // 센서 미감지시 타이머 리셋
                        FuncInline.ResetDelay(Key_Lift_Load);
                    }
                    #endregion
                    break;

                case 10:
                    #region Step 10: 잠시 대기 후 재구동 준비
                    // 정지 상태에서 100ms 대기
                    if (FuncInline.IsDelayOver(Key_Lift_Load, 100))
                    {
                        Log = $"{Name} Restart Conveyor (Overdrive)";
                        FuncLog.WriteLog(Log);

                        // 재구동 시작
                        DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, true);
                        loadingStep = 20;
                    }
                    #endregion
                    break;

                case 20:
                    #region Step 20: 오버드라이브 (2000ms)
                    // 2초간 추가 구동
                    if (FuncInline.IsDelayOver(Key_Lift_Load, 2000))
                    {
                        Log = $"{Name} Loading Finish (Overdrive Complete)";
                        FuncLog.WriteLog(Log);

                        // 최종 정지
                        DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);

                        // 스텝 초기화 및 완료 리턴
                        loadingStep = 0;
                        return true;
                    }
                    #endregion
                    break;
            }

            return false; // 아직 진행 중
        }

        /// <summary>
        /// Front FT 사이트(Site10 ~ Site13) 중 진입 가능한(사용중 + 빈공간) 곳이 있는지 확인
        /// 4세대: FT4(Site10) 사용 안 함 (FT1~3만 체크)
        /// 5세대: FT4(Site10) 포함 전체 체크
        /// </summary>
        public static bool IsFrontFTSiteAvailable()
        {
            // FT 사이트 전체 범위 (FT4 ~ FT1/FT3 등 전체 포함)
            int startSite = (int)enumTeachingPos.Site10_F_DT10_FT4;
            int endSite = (int)enumTeachingPos.Site13_F_FT3;

            for (int i = startSite; i <= endSite; i++)
            {
                // [세대별 예외 처리]
                // 5세대 미만(4세대 이하)이고, 현재 검사하는 곳이 FT4(Site10)라면 건너뜀
                if (FuncInline.InlineType < FuncInline.enumInlineType.Gen5 &&
                    i == (int)enumTeachingPos.Site10_F_DT10_FT4)
                {
                    continue;
                }

                // 1. 사이트 사용 여부 (설정에서 Use 체크 되어있는지)
                // SiteUse 배열이 없다면 GlobalVar.SiteUse[i] 등 실제 변수로 교체 필요
                if (!FuncInline.UseSite[i]) continue;

                // 2. 사이트가 비어있는지 확인 (UnKnown 상태)
                bool isEmpty = FuncInline.PCBInfo[i].PCBStatus == enumSMDStatus.UnKnown;

                // 사용 중이고(Enabled) + 비어있다면(Empty) -> 진입 가능
                if (isEmpty)
                {
                    return true; // 빈 자리가 하나라도 있으면 True
                }
            }

            // 모든 유효한 FT 사이트가 꽉 찼음
            return false;
        }

        /// <summary>
        /// Front FT 사이트(Site10 ~ Site13) 중 사용 가능하고 빈 곳을 순차적으로 검색하여 반환
        /// </summary>
        /// <returns>빈 FT 사이트 위치 (없으면 None)</returns>
        public FuncInline.enumTeachingPos GetAvailableFrontFTSite()
        {
            // FT 사이트 시작/끝 인덱스 정의
            int startFT = (int)FuncInline.enumTeachingPos.Site10_F_DT10_FT4;
            int endFT = (int)FuncInline.enumTeachingPos.Site13_F_FT3;

            // FT 사이트 총 개수 (보통 4개)
            int ftCount = endFT - startFT + 1;

            // 검색 시작 위치: 마지막에 넣은 FT 위치 다음부터
            // LastFrontFTIndex는 전역변수나 클래스 멤버변수로 관리 필요 (초기값 0)
            int startIndex = (LastFrontFTIndex + 1) % ftCount;

            for (int k = 0; k < ftCount; k++)
            {
                // 원형 검색 인덱스 계산
                int currentOffset = (startIndex + k) % ftCount;
                int currentSiteIdx = startFT + currentOffset;

                // [세대별 예외 처리]
                // 5세대 미만(Gen4 이하)은 FT4(Site10) 사용 안 함 -> 건너뜀
                if (FuncInline.InlineType < FuncInline.enumInlineType.Gen5 &&
                    currentSiteIdx == (int)FuncInline.enumTeachingPos.Site10_F_DT10_FT4)
                {
                    continue;
                }

                // 1. 사이트 사용 여부 (UseSite 배열 등)
                if (!FuncInline.UseSite[currentSiteIdx]) continue;

                // 2. 빈 사이트인지 확인 (UnKnown)
                if (FuncInline.PCBInfo[currentSiteIdx].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                {
                    // 사용 가능한 사이트 찾음!

                    // 마지막 인덱스 업데이트 (다음엔 여기 다음부터 찾음)
                    LastFrontFTIndex = currentOffset;

                    return (FuncInline.enumTeachingPos)currentSiteIdx;
                }
            }

            return FuncInline.enumTeachingPos.None; // 빈 곳 없음
        }
        /// <summary>
        /// Front Lift UnLoading 동작 (구동 -> 센서 OFF 확인 -> 정지 -> 완료)
        /// </summary>
        private bool LiftUnLoadingSiteAction()
        {
            // 1. 배출 모터 구동
            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, true);

            // 2. 센서 OFF 확인 (제품이 리프트를 떠남)
            // 보통 Lift Stop 센서가 꺼지면 나간 것으로 간주
            if (!FLift_UpPCB_Stop_Sensor && !FLift_DownPCB_IN_Sensor)
            {
                // 3. 완전히 나가도록 약간의 딜레이
                if (FuncInline.IsDelayOver(Key_Lift_Unload, 500))
                {
                    // 4. 모터 정지
                    DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
                    DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);

                    // 완료 리턴
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
        /// Front Lift UnLoading 동작 (구동 -> 센서 OFF 확인 -> 정지 -> 완료)
        /// </summary>
        private bool LiftUnLoadingOutShuttleAction()
        {
            // 1. 배출 모터 구동
            DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, true);
            DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);

            // 2. 센서 OFF 확인 (제품이 리프트를 떠남)
            // 보통 Lift Stop 센서가 꺼지면 나간 것으로 간주
            if (!FLift_UpPCB_Stop_Sensor)
            {
                // 3. 완전히 나가도록 약간의 딜레이
                if (FuncInline.IsDelayOver(Key_Lift_Unload, 500))
                {
                    // 4. 모터 정지
                    DIO.WriteDOData(enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
                    DIO.WriteDOData(enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);

                    // 완료 리턴
                    return true;
                }
            }
            else
            {
                FuncInline.ResetDelay(Key_Lift_Unload);
            }

            return false;
        }
    }
}



