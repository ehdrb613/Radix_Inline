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
     * @brief 작업 전 트레이 리프트 클래스
     *        작업 전 임플란트 트레이를 투입한다.
     *        제어 및 상태값, 선언, 쓰레드 등을 모두 포함
     */
    class InShuttle
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

            MoveLoadingPos,   //이동 지령에 따라, 위치 지정, 턴이 필요하면 턴 포지션으로(턴상태로 확인?)
            MoveTurnPos,
            MoveFrontPos,
            MoveRearPos,   //턴 포지션 확인 후 턴 진행, 추가 위치 이동 필요시 이동 후 컨베이어 동작

            Loading,   //앞설비로 부터 로딩
            LoadingCheck, //트레이 투입동작
            Cooling,
            Check_Destination, //목적지 판단
            FrontUnLoading,
            FrontUnLoadingCheck,
            RearUnLoading,
            RearUnLoadingCheck,
            Scan    // 비젼은 언젠가 추가하겠지?

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
        /** @brief 쓰레드의 이전 동작 단계 */
        private enumAction beforeAction = enumAction.Waiting;
        /** @brief 시스템의 이전 상태 */
        private enumSystemStatus beforeSystemStatus = GlobalVar.SystemStatus;

        /** @brief 동작 수행시 타임아웃 체크 */
        private Stopwatch watch = new Stopwatch();
        private Stopwatch delayWatch = new Stopwatch();
        /** @brief 한 공정 완료 여부. 각 하부 Part별로 완료여부 체크되면 컨베어 움직이고, 컨베어 움직이기 시작하면 완료여부 clear 하면 된다. */
        public bool StepFinish = false;

        private const string Key_InShuttle_Load = "InShuttle_Load";
        private const string Key_Front_Unload = "Front_Unload";
        private const string Key_Rear_Unload = "Rear_Unload";

        /** @brief 현재 공정에서 작업중인 모델정보 */
        public string NowModel = "";
        public int SV00_In_Shuttle = (int)FuncInline.enumServoAxis.SV00_In_Shuttle;
        
        //마지막 투입 위치 기억 변수 (true: Front, false: Rear)
        // 초기값을 false(Rear)로 두어야 첫 동작 때 Front로 먼저 감
        private bool LastInputWasFront = false;
        // 마지막으로 투입을 시도/확인한 사이트 인덱스 (Front/Rear 각각 관리)
        // -1로 초기화하여 처음 시작 시 0번(1층)부터 찾도록 함
        public int LastFrontIndex = -1;
        public int LastRearIndex = -1;
        
        public int PCBID = 0;

        /** @brief 전 설비 PCB 받는 위치 */
        public static double LoadingPos = 0;
        /** @brief Front Rack 투입 위치 */
        public static double FrontRackUnLoadingPos = 0;
        /** @brief Turn포지션 위치 */
        public static double TurnPos = 0;
        /** @brief Rear Rack 투입 위치 */
        public static double RearLiftUnLoadingPos = 0;

        const double tolerance = 1.0; // 허용 오차(mm)

        // 로딩 동작 내부 스텝 관리용 변수
        private int loadingStep = 0;

        #region InShuttle DIO 변수

        #region DO 출력부
        //턴 동작 복동(로딩 위치일때)
        public static bool Y_Turn_CCW_Cylinder = false;
        //턴 동작 복동(Rear 배출 위치일때)
        public static bool Y_Turn_CW_Cylinder = false;
        //스토퍼 동작
        public static bool Y_STOPPER_SOL = false;
        //컨베이어 정방향 동작
        public static bool Y_Conveyor_Cw = false;
        //컨베이어 역방향 동작
        public static bool Y_Motor_Ccw = false;

        //PCB받을 준비 완료 신호
        public static bool Y_SMEMA_Ready = false;
        //앞장비에게 뒷장비가 Inline이다 신호
        public static bool Y_SMEMA_Before_AutoInline = false;
        #endregion
        #region Di 출력부

        //PCB 진입부 감지 센서
        public bool X_Pcb_In_Sensor = false;
        //PCB 도착 감지 센서
        public bool X_Pcb_Stop_Sensor = false;
        //스토퍼 상승 센서
        public static bool X_Stopper_Cyl_Up_Sensor = false;

        //턴 실린더 정방향 센서(Rear 배출)
        public static bool X_Turn_Cw_Sensor = false;
        //턴 실린더 역방향 센서(로딩 위치일때)
        public static bool X_Turn_Ccw_Sensor = false;
        //턴 실린더 턴 가능 위치, 감지 안하면 동작 X
        public static bool X_Turn_Position_Interlock = false;
        //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(포토)
        public static bool X_PCB_Interlock_Sensor1 = false;

        //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
        public static bool X_PCB_Interlock_Sensor2 = false;

        //온도센서 감지 True면 OK
        public static bool X_Pcb_Temp_Sensor = false;

        //4세대? 전용 =============================================
        //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
        public static bool X_Turn_Motor_Alarm = false;
        //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
        public static bool X_Turn_Motor_HomeComplete = false;
        // ========================================================

        //PCB받을 준비 완료 신호
        public static bool X_SMEMA_Ready = false;
        //뒷설비에게 테스트 완료된 PCB를 패스해라 신호
        public static bool X_SMEMA_Before_Pass = false;
        public string Name = "";
        #endregion

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
        public InShuttle()
        {

            // 쓰레드를 시작한다
            actionThread = new Thread(ActionThread);
            actionThread.Start();

            Name = "[InShuttle]"; // 영문 로그 이름 설정
        }

        /** @brief 소멸자 */
        ~InShuttle()
        {
            ClassDisposing = true;
        }

        private void debug(string str)
        {
            Util.Debug("InShuttleClass : " + str);
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

                    //Loading Pos 위치가 아니면 신호 OFF해야함 
                    if (!FuncInlineMove.IsArrived(SV00_In_Shuttle, LoadingPos) ||
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                    {
                        //스메마 OFF
                        DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, false);
                    }
                    if(GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                        FuncInlineMove.IsArrived(SV00_In_Shuttle, LoadingPos) &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InConveyor].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                    {
                        //스메마 On
                        DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, true);
                    }

                    /** @brief 전 설비 PCB 받는 위치 */
                    LoadingPos = FuncInline.ShuttlePos[(int)enumShuttleName.InShuttle, (int)enumShuttlePos.InShuttle_InConveyorLoading];
                    /** @brief Front Rack 투입 위치 */
                    FrontRackUnLoadingPos = FuncInline.ShuttlePos[(int)enumShuttleName.InShuttle, (int)enumShuttlePos.InShuttle_FrontRackUnLoading];
                    /** @brief Turn포지션 위치 */
                    TurnPos = FuncInline.ShuttlePos[(int)enumShuttleName.InShuttle, (int)enumShuttlePos.InShuttle_TurnPosition];
                    /** @brief Rear Rack 투입 위치 */
                    RearLiftUnLoadingPos = FuncInline.ShuttlePos[(int)enumShuttleName.InShuttle, (int)enumShuttlePos.InShuttle_RearLiftUnLoading];

                    #region DO 출력부
                    //턴 동작 복동(로딩 위치일때)
                    Y_Turn_CCW_Cylinder = DIO.GetDORead(enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder);
                    //턴 동작 복동(Rear 배출 위치일때)
                    Y_Turn_CW_Cylinder = DIO.GetDORead(enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder);
                    //스토퍼 동작
                    Y_STOPPER_SOL = DIO.GetDORead(enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL);
                    //컨베이어 정방향 동작
                    Y_Conveyor_Cw = DIO.GetDORead(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw);
                    //컨베이어 역방향 동작
                    Y_Motor_Ccw = DIO.GetDORead(enumDONames.Y400_3_In_Shuttle_Motor_Ccw);

                    Y_SMEMA_Ready = DIO.GetDORead(enumDONames.Y4_2_SMEMA_Before_Ready);

                    if (!Y_SMEMA_Before_AutoInline)
                    {
                        //전장비 오토인라인 신호는 상시로 켜져 있어야함
                        DIO.WriteDOData(enumDONames.Y404_3_SMEMA_Before_AutoInline, true);
                    }

                    Y_SMEMA_Before_AutoInline = DIO.GetDORead(enumDONames.Y404_3_SMEMA_Before_AutoInline);

                  

                    #endregion
                    #region Di 출력부
                    //PCB 진입부 감지 센서
                    X_Pcb_In_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor);
                    //PCB 도착 감지 센서
                    X_Pcb_Stop_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor);
                    //스토퍼 상승 센서
                    X_Stopper_Cyl_Up_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_2_In_Shuttle_Stopper_Cyl_Up_Sensor);
                    //턴 실린더 정방향 센서(Rear 배출)
                    X_Turn_Cw_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_6_In_Shuttle_Turn_Cw_Cyl_Sensor);
                    //턴 실린더 역방향 센서(로딩 위치일때)
                    X_Turn_Ccw_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_7_In_Shuttle_Turn_Ccw_Cyl_Sensor);
                    //턴 실린더 턴 가능 위치, 감지 안하면 동작 X
                    X_Turn_Position_Interlock = DIO.GetDIData(FuncInline.enumDINames.X303_0_In_Shuttle_Turn_Position_Interlock);
                    //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(포토)
                    X_PCB_Interlock_Sensor1 = DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor);

                    //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
                    X_PCB_Interlock_Sensor2 = DIO.GetDIData(FuncInline.enumDINames.X304_0_In_Shuttle_Interlock_Sensor);

                    //온도센서 감지 True면 OK
                    X_Pcb_Temp_Sensor = DIO.GetDIData(FuncInline.enumDINames.X304_7_In_Shuttle_Pcb_Temp_Sensor);

                    //4세대? 전용 =============================================
                    //전동실린더 모터알람?
                    X_Turn_Motor_Alarm = DIO.GetDIData(FuncInline.enumDINames.X304_3_In_Shuttle_Turn_Motor_Alarm);
                    //전동실린더 홈완료 센서?
                    X_Turn_Motor_HomeComplete = DIO.GetDIData(FuncInline.enumDINames.X304_5_In_Shuttle_Turn_Motor_Home_Complete);
                    // ========================================================
                    X_SMEMA_Ready = DIO.GetDIData(FuncInline.enumDINames.X02_1_SMEMA_Before_Ready);
                    X_SMEMA_Before_Pass = DIO.GetDIData(FuncInline.enumDINames.X00_6_SMEMA_Before_Pass);

                    int PcbID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].Num;
                    String logPcbId = $"[PCB_ID:{PcbID}]";

                    //Move 중일때 인터락 및 진입 센서 감지될때
                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                        (Action >= enumAction.MoveLoadingPos && Action <= enumAction.MoveRearPos) &&
                        ((!FuncInline.LongPCBTest && X_Pcb_In_Sensor) || X_PCB_Interlock_Sensor1 || X_PCB_Interlock_Sensor2))
                    {
                        FuncInline.AddError(FuncInline.enumErrorPart.InShuttle,
                                            FuncInline.enumErrorCode.PCB_Interlock,
                                            $"[InConveyor]{Log} PCB Interlock Check.");
                        FuncInline.AddError(FuncInline.enumErrorPart.InConveyor,
                                           FuncInline.enumErrorCode.PCB_Interlock,
                                           $"{Log} PCB Interlock Check.");
                        Action = enumAction.Waiting;
                    }

                  


                    #endregion


                    #endregion

                    #region 시스템 상태 따라
                    switch (Action)
                    {
                        case enumAction.Waiting:
                            #region Case Waiting

                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                            {

                                //없을때 상시 준비위치에 대기 하기
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InConveyor].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                    !FuncInlineMove.IsArrived(SV00_In_Shuttle, LoadingPos))
                                {
                                    //턴해야 하면 턴포이션으로
                                    if (!Y_Turn_CCW_Cylinder &&
                                        Y_Turn_CW_Cylinder &&
                                        !X_Turn_Ccw_Sensor)
                                    {
                                        Log = $"{Name} MoveTurn Position Action";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.MoveTurnPos;
                                    }
                                    //턴안해도 되면 바로 LoadingPos션으로
                                    else
                                    {
                                        Log = $"{Name} MoveLoading Position Action";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.MoveLoadingPos;
                                    }

                                }
                                //동작하면 안되는 상태 먼저진행
                                else if (FuncInline.CycleStop == true)
                                {
                                    Log = $"{Name} CycleStop";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.CycleStop;

                                }
                                else if (FuncInline.InStop == true)
                                {
                                    //Util.InitWatch(ref watch);
                                    continue;
                                }
                                //받을준비 되었을때 로딩, 스메마 ON이거나 수동투입 ON 후 센서 감지일때
                                else if ((X_SMEMA_Ready || FuncInline.InputPCB && X_Pcb_In_Sensor)
                                    && Y_SMEMA_Ready &&
                                    FuncInline.PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus == enumSMDStatus.UnKnown)
                                {
                                    loadingStep = 0; //초기화
                                    Log = $"{Name} Loading Action";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.Loading;
                                }
                                //PASS모드 거나 직렬 PASS일때
                                else if (X_Pcb_Stop_Sensor &&
                                    PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus == enumSMDStatus.Bypass)
                                {
                                    PCBInfo[(int)enumTeachingPos.InShuttle].Destination = enumTeachingPos.OutShuttle_Up;
                                    if (PCBInfo[(int)enumTeachingPos.InShuttle].TestPass)
                                    {
                                        Log = $"{Name}[TestPass] MoveFrontPos Action";
                                    }
                                    else
                                    {
                                        Log = $"{Name}[PsssMode] MoveFrontPos Action";
                                    }

                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.MoveFrontPos;

                                }
                                //PCB 있을때
                                else if (X_Pcb_Stop_Sensor &&
                                    PCBInfo[(int)enumTeachingPos.InShuttle].PCBStatus != enumSMDStatus.UnKnown)
                                {
                                    Log = $"{Name} Check_Destination Action";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.Check_Destination;
                                }
                                else
                                {
                                    
                                    //if (!FuncInline.InputPCB && 
                                    //    (X_Pcb_Stop_Sensor || X_Pcb_In_Sensor || X_PCB_Interlock_Sensor1 || X_PCB_Interlock_Sensor2))
                                    //{
                                    //    // 기존 에러 처리
                                    //    FuncInline.AddError(FuncInline.enumErrorPart.InConveyor,
                                    //        FuncInline.enumErrorCode.PCB_Interlock,
                                    //        $"{Log} PCB detected.But no PCB Information.");
                                   
                                    //}
                                }

                            }
                            //Util.InitWatch(ref watch);
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

                            //Util.InitWatch(ref watch);
                            break;
                        #endregion
                        case enumAction.Init:
                            #region Case Init

                            PCBID = 0;
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InShuttle] = true;
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InConveyor] = true;

                            //스메마 OFF
                            DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, false);

                            if (Y_Conveyor_Cw)
                            {
                                Log = $"{Name} Init - Conveyor CW Stop";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);
                            }
                            if (Y_Motor_Ccw)
                            {
                                Log = $"{Name} Init - Conveyor CCW Stop";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw, false);
                            }
                            if (X_Stopper_Cyl_Up_Sensor)
                            {
                                Log = $"{Name} Init - Stopper_Cyl_Up";
                                FuncLog.WriteLog(Log);
                                Stopper_Open(false);
                                //DIO.WriteDOData(FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL, false);
                            }

                            if (!X_Turn_Ccw_Sensor)
                            {
                                if (GlobalVar.Simulation)
                                {
                                    DIO.WriteDIData(FuncInline.enumDINames.X302_7_In_Shuttle_Turn_Ccw_Cyl_Sensor, true);
                                }

                                Log = $"{Name} init - Turn Check";
                                DIO.WriteDOData(FuncInline.enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder, true);
                                DIO.WriteDOData(FuncInline.enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder, false);
                                continue;
                            }


                            if (GlobalVar.LetsHoming &&
                                  GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST00_InShuttle_Width].Homing == false &&
                                  GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST00_InShuttle_Width].isHomed == false)
                            {
                                Log = $"{Name} Init - InShuttle Width Homing Start";
                                FuncLog.WriteLog(Log);

                                FuncLetsMotion.HomeRun((int)0);//

                            }
                            if (GlobalVar.LetsHoming &&
                                GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST03_InConveyor_Width].Homing == false &&
                                  GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST03_InConveyor_Width].isHomed == false &&
                                  GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST00_InShuttle_Width].isHomed == true)
                            {
                                Log = $"{Name} Init - InConveyor Width Homing Start";
                                FuncLog.WriteLog(Log);

                                FuncLetsMotion.HomeRun((int)3); //InConveyor

                            }



                          
                            FuncInline.InitialDone[(int)FuncInline.enumInitialize.InConveyor] = FuncInlineAction.CheckOriginDone(enumInitialize.InConveyor);
                            FuncInline.InitialDone[(int)FuncInline.enumInitialize.InShuttle] = FuncInlineAction.CheckOriginDone(enumInitialize.InShuttle);

                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InConveyor] = !FuncInline.InitialDone[(int)enumInitialize.InConveyor];
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InShuttle] = !FuncInline.InitialDone[(int)enumInitialize.InShuttle];


                            if (FuncInline.InitialDone[(int)FuncInline.enumInitialize.InConveyor] &&
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.InShuttle])
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
                                if (GlobalVar.AxisStatus[SV00_In_Shuttle].StandStill &&
                                  !GlobalVar.AxisStatus[SV00_In_Shuttle].Homing)
                                {
                                    Log = $"{Name} Init - SV00_InShuttle Home Move Start";
                                    FuncLog.WriteLog(Log);
                                    FuncMotion.MoveHome((uint)SV00_In_Shuttle);
                                }
                            }

                            //호밍중에 센서 감지 안되면 바로 정지 지령( 안전문제)
                            if (!X_Turn_Ccw_Sensor)
                            {
                                Log = $"{Name} Init - InShuttle Turn Sensor Not Detected, Servo Stop";
                                FuncLog.WriteLog(Log);
                                FuncMotion.MoveStop(SV00_In_Shuttle); //정지상태 되면 서보 정지
                            }

                           // Util.ResetWatch(ref watch);
                            break;
                        #endregion
                        case enumAction.InitFinish:
                            #region Case InitFinish
                            // 원점 동작 완료
                            // Main Control Thread 에서 전체 초기화 체크 후 Waiting으로 변경한다.
                            StepFinish = false; //false 되야 동작 시작
                            //Util.ResetWatch(ref watch);
                            break;
                            #endregion
                    }
                    #endregion


                    #region 동작 진행

                    #region if 시스템 변경되었을때
                    if (beforeSystemStatus != GlobalVar.SystemStatus &&
                     GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
                    {
                        FuncMotion.MoveStop(SV00_In_Shuttle); //정지상태 되면 서보 정지
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
                        bool isCheckState = ((Action >= enumAction.MoveFrontPos && Action <= enumAction.MoveRearPos) ||
                                             Action == enumAction.FrontUnLoading || Action == enumAction.FrontUnLoadingCheck ||
                                             Action == enumAction.RearUnLoading || Action == enumAction.RearUnLoadingCheck ||
                                             Action == enumAction.Loading || Action == enumAction.LoadingCheck);

                        if (isCheckState && watch.ElapsedMilliseconds > ActionTimeout)
                        {
                            watch.Stop(); // 타임아웃 발생 시 타이머 정지
                            if((Action >= enumAction.MoveFrontPos && Action <= enumAction.MoveRearPos))
                            {
                                FuncInline.AddError(FuncInline.enumErrorPart.InShuttle, FuncInline.enumErrorCode.MoveFail,
                                    $"{Log}{Action.ToString()} Servo Move Timeout.");
                            }
                            // 에러 메시지 분기 처리
                            else if (Action == enumAction.FrontUnLoading || Action == enumAction.FrontUnLoadingCheck)
                            {
                                FuncInline.AddError(FuncInline.enumErrorPart.InShuttle, FuncInline.enumErrorCode.Conveyor_Timeout,
                                    $"{Log} FrontUnLoading Timeout.");
                                FuncInline.AddError(FuncInline.enumErrorPart.FrontPassLine, FuncInline.enumErrorCode.Conveyor_Timeout,
                                    "[PassLine] Loading Timeout.");
                                AutoInline.Class.FrontRack.PasslineAction = FrontRack.enumPassLineAction.Waiting;
                            }
                            else if (Action == enumAction.RearUnLoading || Action == enumAction.RearUnLoadingCheck)
                            {
                                FuncInline.AddError(FuncInline.enumErrorPart.InShuttle, FuncInline.enumErrorCode.Conveyor_Timeout,
                                   $"{Log} RearUnLoading Timeout.");
                                FuncInline.AddError(FuncInline.enumErrorPart.Lift2_Up, FuncInline.enumErrorCode.Conveyor_Timeout,
                                   "Loading Timeout.");
                                AutoInline.Class.RearRack.Action = RearRack.enumAction.Waiting;
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
                        switch (Action)
                        {

                            case enumAction.HomeMove: //에러 발생 후 복귀 동작 후 -> Waiting으로
                                #region HomeMove
                                // 에러 발생 후 복귀 동작. 안전하게 LoadingPos로 이동
                                Log = $"{Name} HomeMove Start";
                                FuncLog.WriteLog(Log);

                                // 1. 컨베이어 정지
                                DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);

                                // 2. 턴 실린더 원복 (CCW - 로딩 방향)
                                if (!X_Turn_Ccw_Sensor)
                                {
                                    DIO.WriteDOData(enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder, true);
                                    DIO.WriteDOData(enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder, false);
                                }

                                if (GlobalVar.AxisStatus[SV00_In_Shuttle].StandStill)
                                {
                                    // 3. 서보 이동 (변경된 함수 및 속도 적용)
                                    FuncInlineMove.MoveAbsolute((uint)SV00_In_Shuttle, LoadingPos);
                                }
                                Action = enumAction.Waiting;

                               
                                break;
                                #endregion
                            case enumAction.MoveLoadingPos:
                                #region MoveLoadingPos
                                // 로딩 위치로 이동 (턴 상태 확인 후)
                                if (X_Turn_Ccw_Sensor &&
                                    Y_Turn_CCW_Cylinder &&
                                        !Y_Turn_CW_Cylinder) // 턴이 로딩 방향(CCW)에 있어야 함
                                {
                                    if (X_Stopper_Cyl_Up_Sensor)
                                    {
                                        Stopper_Open(false);
                                        //DIO.WriteDOData(enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL, false);
                                    }
                                    if (GlobalVar.AxisStatus[SV00_In_Shuttle].StandStill)
                                    {
                                        Log = $"{Name} Move to Loading Position";
                                        FuncLog.WriteLog(Log);
                                        FuncInlineMove.MoveAbsolute((uint)SV00_In_Shuttle, LoadingPos);
                                    }
                                 
                                    if (FuncInlineMove.IsArrived(SV00_In_Shuttle, LoadingPos))
                                    {
                                        Log = $"{Name} Move LoadingPos Finish, Waiting";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.Waiting; // 도착 후 대기

                                        //스메마 ON 해야 할때 준비상태면
                                        DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, true);

                                    }
                                  
                                }
                                else
                                {
                                    Log = $"{Name} Move MoveTurnPos Action";
                                    FuncLog.WriteLog(Log);
                                    // 턴이 안되어 있으면 턴 포지션으로 먼저 이동 필요
                                    Action = enumAction.MoveTurnPos;
                                }
                                break;
                            #endregion

                            case enumAction.MoveTurnPos:
                                #region MoveTurnPos
                                // 턴 동작을 위해 턴 포지션으로 이동
                                if (GlobalVar.AxisStatus[SV00_In_Shuttle].StandStill)
                                {
                                    Log = $"{Name} Move to Turn Position";
                                    FuncLog.WriteLog(Log);
                                    FuncInlineMove.MoveAbsolute((uint)SV00_In_Shuttle, TurnPos);
                                }
                                if (FuncInlineMove.IsArrived(SV00_In_Shuttle, TurnPos) &&
                                    X_Turn_Position_Interlock)  //턴포지션이 맞으면
                                {
                                    //PCB 없으면 Loading 위치로
                                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                        !X_Pcb_Stop_Sensor)
                                    {
                                        // 도착 후 턴 동작 수행 
                                        DIO.WriteDOData(enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder, true);
                                        DIO.WriteDOData(enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder, false);

                                        if (Y_Turn_CCW_Cylinder && X_Turn_Ccw_Sensor)
                                        {
                                            if(FuncInline.IsDelayOver(Key_InShuttle_Load, 1000))
                                            {
                                                Log = $"{Name} Move to LoadingPos action";
                                                FuncLog.WriteLog(Log);
                                                Action = enumAction.MoveLoadingPos; // 턴 완료 후 로딩 위치로
                                            }
                                            
                                        }

                                    }
                                    //PCB 있으면 Rear UnLoading
                                    else
                                    {
                                        // 도착 후 턴 동작 수행 (예: CW로 Rear동작)
                                        DIO.WriteDOData(enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder, false);
                                        DIO.WriteDOData(enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder, true);

                                        if (Y_Turn_CW_Cylinder && X_Turn_Cw_Sensor)
                                        {
                                            if (FuncInline.IsDelayOver(Key_InShuttle_Load, 1000))
                                            {
                                                Log = $"{Name} Move to MoveRearPos action";
                                                FuncLog.WriteLog(Log);
                                                Action = enumAction.MoveRearPos; // 턴 완료 후 로딩 위치로
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (FuncInlineMove.IsArrived(SV00_In_Shuttle, TurnPos) &&
                                        !X_Turn_Position_Interlock)
                                    {
                                        FuncInline.AddError(FuncInline.enumErrorPart.InShuttle,
                                        FuncInline.enumErrorCode.TurnInterlock_Error,
                                        $"{Log} X303_0_In_Shuttle_Turn_Position_Interlock Sensor Check, Remove the PCB \n check for interference or sensor issues, and then restart.");
                                        Action = enumAction.Waiting;   //에러 발생시 준비위치로 이동 후 Wait

                                        Util.InitWatch(ref watch);
                                        break;
                                    }
                                }
                                break;
                            #endregion

                            case enumAction.MoveFrontPos:
                                #region MoveFrontPos
                                

                                if (GlobalVar.AxisStatus[SV00_In_Shuttle].StandStill)
                                {
                                    Log = $"{Name} Move to Front Position";
                                    FuncLog.WriteLog(Log);
                                    FuncInlineMove.MoveAbsolute((uint)SV00_In_Shuttle, FrontRackUnLoadingPos);
                                }
                                if (FuncInlineMove.IsArrived(SV00_In_Shuttle, FrontRackUnLoadingPos))
                                {
                                    Log = $"{Name} Start Front UnLoading Action";
                                    FuncLog.WriteLog(Log);

                                    //스토퍼 올려줘야 움직일수 있음
                                    Stopper_Open(true);
                                    Action = enumAction.FrontUnLoading;
                                }
                                break;
                            #endregion

                            case enumAction.MoveRearPos:
                                #region MoveRearPos
                                if (GlobalVar.AxisStatus[SV00_In_Shuttle].StandStill)
                                {
                                    Log = $"{Name} Move to Rear Position";
                                    FuncLog.WriteLog(Log);
                                    // Rear 위치로 이동
                                    FuncInlineMove.MoveAbsolute((uint)SV00_In_Shuttle, RearLiftUnLoadingPos);
                                }
                                if (FuncInlineMove.IsArrived(SV00_In_Shuttle, RearLiftUnLoadingPos))
                                {
                                    Log = $"{Name}Start Rear UnLoading Action";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.RearUnLoading;
                                }

                                break;
                            #endregion
                            case enumAction.Loading:
                                #region Loading
                                // 앞 설비로부터 로딩 시작
                                if (FuncInlineMove.IsArrived(SV00_In_Shuttle, LoadingPos))
                                {
                                    if (X_Stopper_Cyl_Up_Sensor)
                                    {
                                        Stopper_Open(false);
                                        //DIO.WriteDOData(enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL, false);
                                    }
                                   

                                    // [기존 로직 유지] PCB 정보 업데이트
                                    if (Y_Conveyor_Cw && (X_Pcb_In_Sensor || X_Pcb_Stop_Sensor))
                                    {
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InConveyor].TestPass =
                                            DIO.GetDIData(FuncInline.enumDINames.X00_6_SMEMA_Before_Pass) || FuncInline.PassMode;

                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InConveyor].PCBStatus =
                                            FuncInline.enumSMDStatus.InConveyor;
                                    }

                                    // =========================================================
                                    // [수정] 스텝 제어 (FuncInline.IsDelayOver 사용)
                                    // =========================================================

                                    // [Step 0] 컨베이어 구동 및 센서 감지 대기
                                    if (loadingStep == 0)
                                    {
                                       
                                        // 최초 진입 시 타이머 초기화 (안전장치)
                                        if (!Y_Conveyor_Cw)
                                        {
                                            Log = $"{Name} Loading Start";
                                            FuncLog.WriteLog(Log);
                                            FuncInline.ResetDelay("InShuttle_Loading"); // 타이머 리셋

                                            DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);
                                            DIO.WriteDOData(enumDONames.Y400_3_In_Shuttle_Motor_Ccw, false);
                                        }

                                        // 센서 감지 시 -> 1차 정지
                                        if (X_Pcb_Stop_Sensor)
                                        {
                                           
                                            if (!FuncInline.IsDelayOver(Key_InShuttle_Load, 300)) continue;

                                            Log = $"{Name} PCB Detected - Temporary Stop";
                                            FuncLog.WriteLog(Log);
                                            // 스메마 OFF
                                            DIO.WriteDOData(enumDONames.Y4_2_SMEMA_Before_Ready, false);

                                            // 1. 일시 정지
                                            DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);

                                            // 2. 다음 스텝으로 (타이머 시작은 IsDelayOver 호출 시 자동)
                                            loadingStep = 10;
                                        }
                                    }
                                    // [Step 10] 정지 상태 유지 (0.5초)
                                    else if (loadingStep == 10)
                                    {
                                        // 키: "InShuttle_Loading", 시간: 500ms
                                        if (FuncInline.IsDelayOver(Key_InShuttle_Load, 100))
                                        {
                                            Log = $"{Name} Restart Conveyor (Overdrive)";
                                            FuncLog.WriteLog(Log);

                                            // 3. 다시 구동 시작
                                            DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);

                                            loadingStep = 20;
                                        }
                                    }
                                    // [Step 20] N초 동안 추가 구동 (Overdrive)
                                    else if (loadingStep == 20)
                                    {
                                        // 키 재사용 (Step이 바뀌었으므로 상관없음), 시간: 2000ms
                                        if (FuncInline.IsDelayOver(Key_InShuttle_Load, 2000))
                                        {
                                            Log = $"{Name} Loading Finish (Overdrive Complete)";
                                            FuncLog.WriteLog(Log);

                                            // 4. 최종 정지
                                            DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);

                                            // 5. 완료 처리
                                            loadingStep = 0;
                                            Action = enumAction.LoadingCheck;
                                        }
                                    }
                                }
                                break;
                            #endregion
                            case enumAction.LoadingCheck:
                                #region LoadingCheck
                                if (X_Pcb_Stop_Sensor)
                                {
                                    //인터락 체크
                                    if (!FuncInline.LongPCBTest && X_Pcb_In_Sensor)
                                    {
                                        FuncInline.AddError(FuncInline.enumErrorPart.InConveyor,
                                        FuncInline.enumErrorCode.PCB_Interlock,
                                       $"{Log} Interlock Error: PCB Input and Stop sensors detected simultaneously.\n Remove the PCB \n check for interference or sensor issues, and then restart.");

                                        FuncInline.AddError(FuncInline.enumErrorPart.InShuttle,
                                        FuncInline.enumErrorCode.PCB_Interlock,
                                        $"{Log} Interlock Error: PCB Input and Stop sensors detected simultaneously.\n Remove the PCB \n check for interference or sensor issues, and then restart.");
                                        Action = enumAction.Waiting;   //에러 발생시 준비위치로 이동 후 Wait
                                        Util.InitWatch(ref watch);
                                        break;
                                    }

                                    //데이터 이동
                                    FuncInline.MovePCBInfo(FuncInline.enumTeachingPos.InConveyor, FuncInline.enumTeachingPos.InShuttle);
                                    //앞설비(마스터)에서 테스트완료된 PCB가 넘어올경우 ON, PASS시켜준다
                                    if (X_SMEMA_Before_Pass)
                                    {
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].TestPass = true;
                                    }

                                    //ID 부여
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].Num = ++PCBID;


                                    if (PCBID >= 999)
                                    {
                                        PCBID = 0; //초기화
                                        Log = $"{Name}PCB ID Reset ";
                                        FuncLog.WriteLog(Log);
                                    }
                                  
                                    Action = enumAction.Cooling;

                                }
                                else
                                {
                                    //PCB 감지 안되면 에러
                                    FuncInline.AddError(FuncInline.enumErrorPart.InShuttle,
                                    FuncInline.enumErrorCode.PCB_Detect_Fail,
                                    $"{Log} PCB_Detect_Fail, Remove the PCB \n check for interference or sensor issues, and then restart.");
                                    Action = enumAction.Waiting;   //에러 발생시 준비위치로 이동 후 Wait
                                    Util.InitWatch(ref watch);
                                    break;
                                }

                                #endregion
                                break;
                            case enumAction.Cooling:
                                #region Cooling
                                if (!FuncInline.CoolingFinish)
                                {
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus = FuncInline.enumSMDStatus.Cooling;
                                    // PassMode 통과, 테스트 완료 PCB일 경우
                                    if (FuncInline.PassMode || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].TestPass)
                                    {
                                        if (FuncInline.PassMode) //  패스모드일 경우 투입수량 증가. 패스 아니면 로봇이 투입시
                                        {
                                            FuncInline.PCBInputCount++;
                                        }

                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus = FuncInline.enumSMDStatus.Bypass;
                                        FuncInline.CoolingFinish = true;
                                    }
                                    // 쿨링 선택하지 않는 경우 통과
                                    else if (!FuncInline.CoolingByTime && !FuncInline.CoolingByTemperature)
                                    {
                                        FuncInline.CoolingFinish = true;
                                    }

                                    bool tempOK = true;
                                    if (FuncInline.CoolingByTemperature &&
                                        !X_Pcb_Temp_Sensor)
                                    {
                                        tempOK = false;
                                    }

                                    else
                                    {
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
                                            FuncInline.CoolingWatch.Reset();
                                            FuncInline.AddError(FuncInline.enumErrorPart.InShuttle,
                                               FuncInline.enumErrorCode.PCB_Temperature_Over, // 또는 PCB_Info_Move_Fail
                                               $"{Log} PCB Temperature is too high. \n");
                                          
                                            Action = enumAction.Waiting;
                                            continue;
                                        }
                                    }
                                }
                                if (FuncInline.CoolingFinish)
                                {
                                    Log = $"{Name}{logPcbId}CoolingFinish ";
                                    FuncLog.WriteLog(Log);

                                    if (FuncInline.CoolingWatch != null)
                                    {
                                        FuncInline.CoolingWatch.Stop();
                                        FuncInline.CoolingWatch.Reset();    //타이머 초기화
                                    }
                                    Action = enumAction.Waiting;
                                    FuncInline.CoolingFinish = false;   //초기화
                                }
                                break;
                            #endregion
                            case enumAction.Check_Destination:
                                #region Check Destination
                                {
                                    // 1. 각 Rack의 "가능한 빈 자리"를 미리 받아옵니다. (여기서 딱 한 번만 검사!)
                                    var frontSite = GetAvailableDLSite(true);
                                    var rearSite = GetAvailableDLSite(false);

                                    // None이 아니면 가능한 것으로 판단
                                    bool canFront = (frontSite != enumTeachingPos.None);
                                    bool canRear = (rearSite != enumTeachingPos.None);

                                    // 2. 둘 다 불가능하면 대기
                                    if (!canFront && !canRear)
                                    {
                                        Util.ResetWatch(ref watch);
                                        Log = $"{Name}{logPcbId} Dest Check - All Racks Full/Busy. Waiting...";
                                        Thread.Sleep(500);
                                        break;
                                    }

                                    // 3. 목적지 결정
                                    bool goToFront = false;

                                    if (canFront && canRear)
                                    {
                                        goToFront = !LastInputWasFront; // 교차 투입
                                    }
                                    else if (canFront)
                                    {
                                        goToFront = true;
                                    }
                                    else // canRear
                                    {
                                        goToFront = false;
                                    }

                                    // 4. 확정 및 할당 (재검사 하지 않음!)
                                    if (goToFront)
                                    {
                                        // 아까 찾아둔 frontSite를 바로 적용
                                        PCBInfo[(int)enumTeachingPos.InShuttle].Destination = frontSite;

                                        // LastIndex 업데이트 (순환 검색을 위해 필요)
                                        // frontSite 값에서 BaseIndex를 빼서 offset 계산
                                        int startIdx = (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                                        int currentIdx = (int)frontSite;
                                        LastFrontIndex = (currentIdx - startIdx); // 방금 찾은 인덱스 저장

                                        Log = $"{Name}{logPcbId} Destination: FRONT ({PCBInfo[(int)enumTeachingPos.InShuttle].Destination})";
                                        FuncLog.WriteLog(Log);

                                        LastInputWasFront = true;
                                        Action = enumAction.MoveFrontPos;
                                    }
                                    else
                                    {
                                        // 아까 찾아둔 rearSite를 바로 적용
                                        PCBInfo[(int)enumTeachingPos.InShuttle].Destination = rearSite;

                                        // LastIndex 업데이트
                                        int startIdx = (int)enumTeachingPos.Site14_R_DT1;
                                        int currentIdx = (int)rearSite;
                                        LastRearIndex = (currentIdx - startIdx); // 방금 찾은 인덱스 저장

                                        Log = $"{Name}{logPcbId} Destination: REAR ({PCBInfo[(int)enumTeachingPos.InShuttle].Destination})";
                                        FuncLog.WriteLog(Log);

                                        LastInputWasFront = false;
                                        Action = enumAction.MoveTurnPos;
                                    }

                                    Util.ResetWatch(ref watch);
                                }
                                #endregion
                                break;

                            case enumAction.FrontUnLoading:
                                #region FrontUnLoading
                                // FrontRack Passline이 받을 준비가 되면 구동
                                if (AutoInline.Class.FrontRack.PasslineAction == FrontRack.enumPassLineAction.Loading ||
                                    AutoInline.Class.FrontRack.PasslineAction == FrontRack.enumPassLineAction.LoadingCheck)
                                {
                                    // 1. 배출 컨베이어 가동
                                    DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);
    
                                    if (!X_Pcb_Stop_Sensor && AutoInline.Class.FrontRack.PasslineAction == FrontRack.enumPassLineAction.LoadingCheck)
                                    {
                                        // [오버드라이브] 확실히 넘겨주기 위해 1초 더 구동
                                        if (FuncInline.IsDelayOver(Key_Front_Unload, 500))
                                        {
                                            // 모터 정지
                                            DIO.WriteDOData(enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);

                                            Log = $"{Name}{logPcbId} Front UnLoading Physical Move Complete";
                                            FuncLog.WriteLog(Log);

                                            Action = enumAction.FrontUnLoadingCheck;
                                        }
                                    }
                                    else
                                    {
                                        // 아직 이동 중이면 타이머 리셋
                                        FuncInline.ResetDelay(Key_Front_Unload);
                                    }
                                }
                                break;
                            #endregion


                            case enumAction.FrontUnLoadingCheck:
                                #region FrontUnLoadingCheck
                                // 3. 데이터 이동 확인 (FrontRack이 데이터를 가져갔는지 확인)
                                bool isDataCleared =  FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus == FuncInline.enumSMDStatus.UnKnown;
                                bool frontInfoArrived = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                               
                                if (isDataCleared && frontInfoArrived)
                                {
                                    Log = $"{Name}Front UnLoading Data Check Complete";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.Waiting;
                                }
                                break;
                            #endregion


                            case enumAction.RearUnLoading:
                                #region RearUnLoading
                                // RearRack이 받을 준비가 되면 구동
                                if (AutoInline.Class.RearRack.Action == RearRack.enumAction.ShuttleLoading ||
                                    AutoInline.Class.RearRack.Action == RearRack.enumAction.ShuttleLoadingCheck)
                                {
                                    // 1. 배출 컨베이어 가동 (CCW)
                                    DIO.WriteDOData(enumDONames.Y400_3_In_Shuttle_Motor_Ccw, true);

                                    if (!X_Pcb_Stop_Sensor && AutoInline.Class.RearRack.RLift_UpPCB_Stop_Sensor)
                                    {
                                        // [오버드라이브] 0.5초 더 구동
                                        if (FuncInline.IsDelayOver(Key_Rear_Unload, 500))
                                        {
                                            // 모터 정지
                                            DIO.WriteDOData(enumDONames.Y400_3_In_Shuttle_Motor_Ccw, false);

                                            Log = $"{Name}{logPcbId} Rear UnLoading Physical Move Complete";
                                            FuncLog.WriteLog(Log);

                                            Action = enumAction.RearUnLoadingCheck;
                                        }
                                    }
                                    else
                                    {
                                        FuncInline.ResetDelay(Key_Rear_Unload);
                                    }
                                }
                                break;
                            #endregion

                           

                            case enumAction.RearUnLoadingCheck:
                                #region RearUnLoadingCheck
                                // 3. 데이터 이동 확인 (FrontRack이 데이터를 가져갔는지 확인)
                                isDataCleared = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus == FuncInline.enumSMDStatus.UnKnown;
                                bool RearInfoArrived = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                // 3. 데이터 이동 확인
                                if (isDataCleared && RearInfoArrived)
                                {
                                    Log = $"{Name} Rear UnLoading Data Check Complete";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.Waiting;
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
                    FuncLog.WriteLog($"{Log} : " + ex.ToString());
                    FuncLog.WriteLog($"{Log} : " + ex.StackTrace);
                }

                Thread.Sleep(GlobalVar.ThreadSleep);
            }
        }
        private void StepFinish_Send()
        {

            StepFinish = true;  //완료했으면 True
        }
        private void Stopper_Open(bool ON)
        {
            if (Y_STOPPER_SOL != ON)
            {
                //스토퍼 올려줘야 움직일수 있음
                DIO.WriteDOData(enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL, ON);
            }
        }
        /// <summary>
        /// 현재 셔틀이 Rear(CW) 위치인지 확인합니다.
        /// (센서 감지 또는 출력 신호로 판단)
        /// </summary>
        private bool IsRearPosition()
        {
            // CW(Rear) 센서가 감지되었거고, CW 출력이 나가고 있다면 Rear로 판단
            if ((X_Turn_Cw_Sensor && Y_Turn_CW_Cylinder) &&
                (!X_Turn_Ccw_Sensor || !Y_Turn_CCW_Cylinder))
            {
                return true;
            }
            return false;
        }

    }
}
