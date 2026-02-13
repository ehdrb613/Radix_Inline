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
    class OutShuttle
    {
        #region type 선언
        #region enum
        /**
         * @brief 동작 구분
         */
        public enum OutShuttle_enumAction
        {
            Waiting, // 아무 동작 없을 때
            Init, // 초기화
            InitFinish, // 초기화 완료
            Skip,       // 제품없을때 스킵
            NotUse,     // 사용 안할때
            CycleStop,  // 사이클스탑일때

            HomeMove, //에러 발생 후 복귀 동작 후 -> Waiting으로
                      // 이동 동작
            MoveFrontLoadingPos,     // Front 또는 Rear 위치로 이동
            MoveTurnPos,
            MoveRearLoadingPos,
            MoveUnLoadingPos,   // Pass(OutConveyor) 또는 NG(Ngbuffer) 위치로 이동

            // 로딩 동작 (InShuttle 참고)
            FrontLoading,            //FrontRack Loading, OutShuttleUp위치면 Up Conveyor 확인, Down위치면 Down Conveyor확인
            FrontLoadingCheck,

            RearLoading,            //RearRack Loading OutShuttleUp위치면 Up Conveyor 확인, Down위치면 Down Conveyor확인, 둘다 있으면  UP,Down동시 진행
            RearLoadingCheck,
            // 언로딩 동작
            UnLoading,          //OutShuttleUp에 PCB있으면 Conveyor 배출, Down에 PCB있으면 NGbuffer로 배출, 둘다 있으면  UP,Down동시 진행
            UnLoadingCheck          //OutShuttleUp에 PCB있으면 Conveyor 배출, Down에 PCB있으면 NGbuffer로 배출, 둘다 있으면  UP,Down동시 진행
        }
        public enum OutConveyor_enumAction
        {
            Waiting, // 아무 동작 없을 때
            Init, // 초기화
            InitFinish, // 초기화 완료
            Skip,       // 제품없을때 스킵
            NotUse,     // 사용 안할때
            CycleStop,  // 사이클스탑일때

            HomeMove, //에러 발생 후 복귀 동작 후 -> Waiting으로
            Loading,
            LoadingCheck,
            UnLoading,
            UnLoadingCheck
        }
        public enum Ngbuffer_enumAction
        {
            Waiting, // 아무 동작 없을 때
            Init, // 초기화
            InitFinish, // 초기화 완료
            Skip,       // 제품없을때 스킵
            NotUse,     // 사용 안할때
            CycleStop,  // 사이클스탑일때

            HomeMove, //에러 발생 후 복귀 동작 후 -> Waiting으로
            Loading,
            LoadingCheck,
            UnLoading,
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
        public double ActionTimeout = 20 * 1000; // 타임아웃 처리 시간. 클래스 초기화 후 메인에서 설정값을 지정할 것
        #endregion
        /** @brief 쓰레드의 동작 단계 */
        public OutShuttle_enumAction OutShuttleAction = OutShuttle_enumAction.Waiting;
        /** @brief 쓰레드의 이전 동작 단계 */
        private OutShuttle_enumAction OutShuttlebeforeAction = OutShuttle_enumAction.Waiting;
        /** @brief 쓰레드의 동작 단계 */
        public OutConveyor_enumAction OutConveyorAction = OutConveyor_enumAction.Waiting;
        /** @brief 쓰레드의 이전 동작 단계 */
        private OutConveyor_enumAction OutConveyorbeforeAction = OutConveyor_enumAction.Waiting;
        /** @brief 쓰레드의 동작 단계 */
        public Ngbuffer_enumAction NgbufferAction = Ngbuffer_enumAction.Waiting;
        /** @brief 쓰레드의 이전 동작 단계 */
        private Ngbuffer_enumAction NgbufferbeforeAction = Ngbuffer_enumAction.Waiting;

        /** @brief 시스템의 이전 상태 */
        private enumSystemStatus beforeSystemStatus = GlobalVar.SystemStatus;

        /** @brief 동작 수행시 타임아웃 체크 */
        private Stopwatch watch = new Stopwatch();
        /** @brief 한 공정 완료 여부. 각 하부 Part별로 완료여부 체크되면 컨베어 움직이고, 컨베어 움직이기 시작하면 완료여부 clear 하면 된다. */
        public bool StepFinish = false;

        /** @brief 현재 공정에서 작업중인 모델정보 */
        public string NowModel = "";
        public int SV01_Out_Shuttle = (int)FuncInline.enumServoAxis.SV01_Out_Shuttle;

        // 로딩 동작 내부 스텝 관리용 변수
        private int loadingStep = 0;

        // 이동 목표 위치 및 소스/목적지 저장용
        private double TargetPos = 0.0;
        private enumTeachingPos CurrentSource = enumTeachingPos.None; // 어디서 로딩했는지
        private bool IsTurnRequired = false; // 턴 필요 여부

        // 딜레이 키
        private const string Key_OutShuttle_Load = "OutShuttle_Load";
        private const string Key_OutShuttle_Unload = "OutShuttle_Unload";
        private const string Key_OutConv_Load = "OutConv_Load";
        private const string Key_OutConv_Unload = "OutConv_Unload";
        private const string Key_NG_Load = "NG_Load";
        private const string Key_Turn = "OutShuttle_Turn";
        private enumShuttleName outshuttle = enumShuttleName.OutShuttle;
        #region InShuttle DIO 변수

        #region DO 출력부
        //턴 동작 복동(로딩 위치일때)
        public static bool Y_Turn_CCW_Cylinder = false;
        //턴 동작 복동(Rear 배출 위치일때)
        public static bool Y_Turn_CW_Cylinder = false;
        //IN 스토퍼 동작
        public static bool Y_IN_Stopper = false;
        //OUT 스토퍼 동작
        public static bool Y_OUT_Stopper = false;
        //컨베이어 OK 정방향 동작
        public static bool Y_OK_Motor_CW = false;
        //컨베이어 OK 역방향 동작
        public static bool Y_OK_Motor_Ccw = false;
        //컨베이어 NG 정방향 동작
        public static bool Y_NG_Motor_CW = false;
        //컨베이어 NG 역방향 동작
        public static bool Y_NG_Motor_Ccw = false;
        //아웃컨베이어 OK 정방향 동작
        public static bool Y_OutConveyor_Motor_CW = false;
        //컨베이어 OK 정방향 동작
        public static bool Y_Ngbuffer_Motor_CW = false;

        //PCB보낼 준비 완료 신호
        public static bool Y_SMEMA_Ready = false;
        //뒷장비에게 PCB 패스 시켜라
        public static bool Y_SMEMA_After_Pass = false;

        //5세대 전용===============================
        //NG버퍼 하단 전진(복동)
        public static bool Y_Ngbuffer_Lower_cylinder_forward = false;
        //NG버퍼 하단 후진(복동)
        public static bool Y_Ngbuffer_Lower_cylinder_backward = false;

        //NG버퍼 상단 전진(복동)
        public static bool Y_Ngbuffer_Upper_cylinder_forward = false;
        //NG버퍼 상단 후진(복동)
        public static bool Y_Ngbuffer_Upper_cylinder_backward = false;


        //==========================================

        #endregion
        #region Di 출력부
        //PCB OK 라인 진입부 감지 센서
        public static bool X_OK_PCB_IN_Sensor = false;
        //PCB OK 라인 도착 감지 센서
        public static bool X_OK_PCB_Stop_Sensor = false;
        //PCB OK 라인 진입부 감지 센서
        public static bool X_NG_PCB_IN_Sensor = false;
        //PCB OK 라인 도착 감지 센서
        public static bool X_NG_PCB_Stop_Sensor = false;

        //PCB OK 라인 진입부 감지 센서
        public static bool X_OutConveyor_PCB_IN_Sensor = false;
        //PCB OK 라인 도착 감지 센서
        public static bool X_OutConveyor_PCB_Stop_Sensor = false;
        //PCB OK 라인 진입부 감지 센서
        public static bool X_NGbuffer_PCB_IN_Sensor = false;
        //PCB OK 라인 도착 감지 센서
        public static bool X_NGbuffer_PCB_Stop_Sensor = false;

        //IN 스토퍼 상승 센서
        public static bool X_IN_Stopper_Up_Sensor = false;
        //OUT 스토퍼 상승 센서
        public static bool X_Out_Stopper_Up_Sensor = false;

        //턴 실린더 정방향 센서(Rear 배출)
        public static bool X_Turn_CW_Sensor = false;
        //턴 실린더 역방향 센서(로딩 위치일때)
        public static bool X_Turn_CCW_Sensor = false;
        //턴 실린더 턴 가능 위치, 감지 안하면 동작 X
        public static bool X_Turn_Interlock_Sensor = false;
        //아웃셔틀에서 아웃컨베이어로 지나가는 위치 PCB 인터락 감지센서(포토)
        public static bool X_OK_Interlock_Sensor = false;
        //아웃셔틀에서 NG버퍼로 지나가는 위치 PCB 인터락 감지센서(포토)
        public static bool X_NG_Interlock_Sensor = false;


        //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
        public static bool X_Turn_Motor_Alarm = false;
        //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
        public static bool X_Turn_Motor_HomeComplete = false;

        public static bool X_SMEMA_After_Ready = false;
        public static bool X_SMEMA_After_AutoInline = false;
        // ========================================================

        public string Name = $"[OutShuttle]";
        public string OutCvyName = $"[OutConveyor]";
        public string NGName = $"[NGBuffer]";
        #endregion

        #endregion

        /** @brief 타임아웃 체크할때 어디서 문제 생겼는지 내용 저장용 */
        //에러 내용 저장용, 타임
        public string Log = "";
        private int PcbOutCvyID = 0;
        private String logOutCvyID = $"";
        private int PcbNgBufferID = 0;
        private String logNgBufferID = $"";
        private int PcbSuttleUpID = 0;
        private String logSuttleUpID = "";
        private int PcbSuttleDownID = 0;
        private String logSuttleDownID = $"";
        //중복로그 방지용 플레그
        private bool isLogWritten = false;

        //서보init 완료시 true 시작시 false
        private bool InitServo = false;
        #endregion

        /** @brief 생성자 */
        public OutShuttle()
        {

            // 쓰레드를 시작한다
            actionThread = new Thread(ActionThread);
            actionThread.Start();
        }

        /** @brief 소멸자 */
        ~OutShuttle()
        {
            ClassDisposing = true;
        }

        private void debug(string str)
        {
            Util.Debug($"{ Name} : " + str);
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


                    //턴 동작 복동(로딩 위치일때)
                    Y_Turn_CCW_Cylinder = DIO.GetDORead(FuncInline.enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder);
                    //턴 동작 복동(Rear 배출 위치일때)
                    Y_Turn_CW_Cylinder = DIO.GetDORead(FuncInline.enumDONames.Y304_3_Out_Shuttle_Turn_Cw_Cylinder);
                    //IN 스토퍼 동작
                    Y_IN_Stopper = DIO.GetDORead(FuncInline.enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_In_SOL);
                    //OUT 스토퍼 동작
                    Y_OUT_Stopper = DIO.GetDORead(FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_Out_SOL);

                    //컨베이어 OK 정방향 동작
                    Y_OK_Motor_CW = DIO.GetDORead(FuncInline.enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw);
                    //컨베이어 OK 역방향 동작
                    Y_OK_Motor_Ccw = DIO.GetDORead(FuncInline.enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw);
                    //컨베이어 NG 정방향 동작
                    Y_NG_Motor_CW = DIO.GetDORead(FuncInline.enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw);
                    //컨베이어 역방향 동작
                    Y_NG_Motor_Ccw = DIO.GetDORead(FuncInline.enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw);

                    Y_OutConveyor_Motor_CW = DIO.GetDORead(FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw);
                    Y_Ngbuffer_Motor_CW = DIO.GetDORead(FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw);

                    Y_Ngbuffer_Lower_cylinder_forward = DIO.GetDORead(FuncInline.enumDONames.Y412_7_NgBuffer_Lower_cylinder_forward);
                    Y_Ngbuffer_Lower_cylinder_backward = DIO.GetDORead(FuncInline.enumDONames.Y4_7_Ngbuffer_Lower_cylinder_backward);
                    Y_Ngbuffer_Upper_cylinder_forward = DIO.GetDORead(FuncInline.enumDONames.Y412_6_Ngbuffer_Upper_cylinder_forward);
                    Y_Ngbuffer_Upper_cylinder_backward = DIO.GetDORead(FuncInline.enumDONames.Y412_5_Ngbuffer_Upper_cylinder_backward);

                    Y_SMEMA_After_Pass = DIO.GetDORead(FuncInline.enumDONames.Y404_5_SMEMA_After_Pass);
                    Y_SMEMA_Ready = DIO.GetDORead(FuncInline.enumDONames.Y412_1_SMEMA_After_Ready);

                    #endregion
                    #region Di 출력부
                    //PCB OK 진입부 감지 센서
                    X_OK_PCB_IN_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor);
                    //PCB OK 도착 감지 센서
                    X_OK_PCB_Stop_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor);
                    //PCB NG 진입부 감지 센서
                    X_NG_PCB_IN_Sensor = DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor);
                    //PCB NG 도착 감지 센서
                    X_NG_PCB_Stop_Sensor = DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor);
                    //IN 스토퍼 상승 센서
                    X_IN_Stopper_Up_Sensor = DIO.GetDIData(FuncInline.enumDINames.X303_5_Out_Shuttle_Stopper_Cyl_IN_Sensor);
                    //Out 스토퍼 상승 센서
                    X_Out_Stopper_Up_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_5_Out_Shuttle_Stopper_Cyl_Out_Sensor);

                    //턴 실린더 정방향 센서(Rear 배출)
                    X_Turn_CW_Sensor = DIO.GetDIData(FuncInline.enumDINames.X303_1_Out_Shuttle_Turn_Cw_Cyl_Sensor);
                    //턴 실린더 역방향 센서(로딩 위치일때)
                    X_Turn_CCW_Sensor = DIO.GetDIData(FuncInline.enumDINames.X303_2_Out_Shuttle_Turn_Ccw_Cyl_Sensor);

                    //턴 실린더 턴 가능 위치, 감지 안하면 동작 X
                    X_Turn_Interlock_Sensor = DIO.GetDIData(FuncInline.enumDINames.X303_3_Out_Shuttle_Turn_Position_Interlock);
                    //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(포토)
                    X_OK_Interlock_Sensor = DIO.GetDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor);


                    //PCB OK 라인 진입부 감지 센서
                    X_OutConveyor_PCB_IN_Sensor = DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor);
                    //PCB OK 라인 도착 감지 센서
                    X_OutConveyor_PCB_Stop_Sensor = DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor);
                    //PCB OK 라인 진입부 감지 센서
                    X_NGbuffer_PCB_IN_Sensor = DIO.GetDIData(FuncInline.enumDINames.X402_2_Out_Conveyor_NG_PCB_In_Sensor); ;
                    //PCB OK 라인 도착 감지 센서
                    X_NGbuffer_PCB_Stop_Sensor = DIO.GetDIData(FuncInline.enumDINames.X04_3_Out_Conveyor_NG_PCB_Stop_Sensor); ;

                    //4세대? 전용 =============================================
                    //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
                    X_Turn_Motor_Alarm = DIO.GetDIData(FuncInline.enumDINames.X304_4_Out_Shuttle_Turn_Motor_Alarm);
                    //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
                    X_Turn_Motor_HomeComplete = DIO.GetDIData(FuncInline.enumDINames.X304_6_Out_Shuttle_Turn_Motor_Home_Complete);
                    // ========================================================
                    X_SMEMA_After_AutoInline = DIO.GetDIData(FuncInline.enumDINames.X00_7_SMEMA_After_AutoInline);
                    X_SMEMA_After_Ready = DIO.GetDIData(FuncInline.enumDINames.X02_2_SMEMA_After_Ready);
                    if (FuncInline.PCBInfo != null)
                    {
                        PcbOutCvyID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutConveyor].Num;
                        logOutCvyID = $"[PCB_ID:{PcbOutCvyID}]";
                        PcbNgBufferID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.NgBuffer].Num;
                        logNgBufferID = $"[PCB_ID:{PcbNgBufferID}]";
                        PcbSuttleUpID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].Num;
                        logSuttleUpID = $"[PCB_ID:{PcbSuttleUpID}]";
                        PcbSuttleDownID = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].Num;
                        logSuttleDownID = $"[PCB_ID:{PcbSuttleDownID}]";
                    }


                    #endregion

                    #region 시스템 상태 따라
                    switch (OutShuttleAction)
                    {

                        case OutShuttle_enumAction.Waiting:
                            #region Case Waiting


                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                            {

                                if (FuncInline.CycleStop == true)
                                {
                                    Log = $"{Name} CycleStop 지령";
                                    FuncLog.WriteLog(Log);
                                    OutShuttleAction = OutShuttle_enumAction.CycleStop;

                                }



                            }
                            Util.InitWatch(ref watch);
                            break;
                        #endregion

                        case OutShuttle_enumAction.Skip:
                            #region Case Skip

                            break;
                        #endregion

                        case OutShuttle_enumAction.NotUse:
                        case OutShuttle_enumAction.CycleStop:
                            #region Case NotUse/Case CycleStop
                            //NotUse 풀리면 다시 Waiting으로


                            if (FuncInline.CycleStop == false)
                            {
                                Log = $"{Name} {Enum.GetName(typeof(OutShuttle_enumAction), OutShuttleAction)} -> Waiting";
                                FuncLog.WriteLog(Log);
                                OutShuttleAction = OutShuttle_enumAction.Waiting;
                                break;
                            }

                            Util.InitWatch(ref watch);
                            break;
                        #endregion
                        case OutShuttle_enumAction.Init:
                            #region Case Init
                            // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행

                            // 2. 잔류 PCB 감지
                            //if (X_OK_PCB_IN_Sensor || X_OK_PCB_Stop_Sensor || X_NG_PCB_IN_Sensor || X_NG_PCB_Stop_Sensor)
                            //{
                            //    FuncInline.enumErrorPart errorPart = FuncInline.enumErrorPart.OutShuttle_Up;

                            //    FuncError.AddError(new FuncInline.structError(
                            //        DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HH:mm:ss"),
                            //        errorPart, FuncInline.enumErrorCode.PCB_Detect_Fail,
                            //        false, $"{Name} PCB Detected. Remove PCB first."));

                            //    return;
                            //}

                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutConveyor] = true;
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutShuttle] = true;
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.NgBuffer] = true;
                            if (Y_OK_Motor_CW)
                            {
                                Log = $"{Name} Init - Out_Shuttle OK Conveyor CW Stop";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, false);
                            }
                            if (Y_OK_Motor_Ccw)
                            {
                                Log = $"{Name} Init - Out_Shuttle OK Conveyor CCW Stop";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw, false);
                            }
                            if (Y_NG_Motor_CW)
                            {
                                Log = $"{Name} Init - Out_Shuttle NG Conveyor CW Stop";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, false);
                            }
                            if (Y_NG_Motor_Ccw)
                            {
                                Log = $"{Name} Init - Out_Shuttle NG  Conveyor CW Stop";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw, false);
                            }

                            if (Y_IN_Stopper)
                            {
                                Log = $"{Name} Init - Out_Shuttle IN Stopper False";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_In_SOL, false);
                            }
                            if (Y_OUT_Stopper)
                            {
                                Log = $"{Name} Init -  Out_Shuttle Out Stopper False";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_Out_SOL, false);
                            }


                            if (!X_Turn_CCW_Sensor)
                            {
                                Log = $"{Name} init - Turn Check";
                                DIO.WriteDOData(enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder, true);
                                DIO.WriteDOData(enumDONames.Y304_3_Out_Shuttle_Turn_Cw_Cylinder, false);
                                continue;
                            }


                            if (GlobalVar.LetsHoming &&
                               GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST02_OutConveyor_Width].Homing == false &&
                                  GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST02_OutConveyor_Width].isHomed == false)
                            {
                                Log = $"{Name} Init - OutConveyor Width Homing Start";
                                FuncLog.WriteLog(Log);

                                FuncLetsMotion.HomeRun((int)2);
                            }
                            if (GlobalVar.LetsHoming &&
                              GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST01_OutShuttle_Width].Homing == false &&
                                  GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST01_OutShuttle_Width].isHomed == false &&
                                  GlobalVar.LetsAxisStatus[(int)enumLetsAxis.ST02_OutConveyor_Width].isHomed == true)
                            {
                                Log = $"{Name} Init - OutShuttle Width Homing Start";
                                FuncLog.WriteLog(Log);

                                FuncLetsMotion.HomeRun((int)1);
                            }



                            FuncInline.InitialDone[(int)enumInitialize.OutConveyor] = FuncInlineAction.CheckOriginDone(enumInitialize.OutConveyor);
                            FuncInline.InitialDone[(int)enumInitialize.OutShuttle] = FuncInlineAction.CheckOriginDone(enumInitialize.OutShuttle);
                            FuncInline.InitialDone[(int)enumInitialize.NgBuffer] = FuncInlineAction.CheckOriginDone(enumInitialize.NgBuffer);
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutConveyor] = !FuncInline.InitialDone[(int)enumInitialize.OutConveyor];
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutShuttle] = !FuncInline.InitialDone[(int)enumInitialize.OutShuttle];
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.NgBuffer] = !FuncInline.InitialDone[(int)enumInitialize.NgBuffer];

                            // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행
                            if (FuncInline.InitialDone[(int)enumInitialize.OutConveyor] &&
                                 FuncInline.InitialDone[(int)enumInitialize.OutShuttle] &&
                                 FuncInline.InitialDone[(int)enumInitialize.NgBuffer])
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
                                OutShuttleAction = OutShuttle_enumAction.InitFinish;
                            }
                            else
                            {

                                if (GlobalVar.AxisStatus[SV01_Out_Shuttle].StandStill &&
                                    !GlobalVar.AxisStatus[SV01_Out_Shuttle].Homing)
                                {
                                    Log = $"{Name} Init - SV01_Out_Shuttle Home Move Start";
                                    FuncLog.WriteLog(Log);
                                    FuncMotion.MoveHome((uint)SV01_Out_Shuttle);
                                }

                            }

                            //호밍중에 센서 감지 안되면 바로 정지 지령( 안전문제)
                            if (!X_Turn_CCW_Sensor)
                            {
                                Log = $"{Name} Init -  Turn Sensor Not Detected, Servo Stop";
                                FuncMotion.MoveStop(SV01_Out_Shuttle); //정지상태 되면 서보 정지
                            }

                            Util.ResetWatch(ref watch);
                            break;
                        #endregion
                        case OutShuttle_enumAction.InitFinish:
                            #region Case InitFinish
                            // 원점 동작 완료
                            // Main Control Thread 에서 전체 초기화 체크 후 Waiting으로 변경한다.
                            StepFinish = false; //false 되야 동작 시작
                            Util.ResetWatch(ref watch);
                            break;
                            #endregion
                    }

                    switch (OutConveyorAction)
                    {
                        case OutConveyor_enumAction.Waiting:
                            #region Case Waiting          
                            break;
                        #endregion

                        case OutConveyor_enumAction.Skip:
                            #region Case Skip

                            break;
                        #endregion

                        case OutConveyor_enumAction.NotUse:
                        case OutConveyor_enumAction.CycleStop:
                            #region Case NotUse/Case CycleStop
                            //NotUse 풀리면 다시 Waiting으로


                            if (FuncInline.CycleStop == false)
                            {
                                Log = $"{OutCvyName} {Enum.GetName(typeof(OutConveyor_enumAction), OutConveyorAction)} -> Waiting";
                                FuncLog.WriteLog(Log);
                                OutConveyorAction = OutConveyor_enumAction.Waiting;
                                break;
                            }

                            Util.InitWatch(ref watch);
                            break;
                            #endregion

                    }


                    switch (NgbufferAction)
                    {
                        case Ngbuffer_enumAction.Waiting:
                            #region Case Waiting

                            break;
                            #endregion

                    }
                    #endregion


                    #region 동작 진행

                    #region if 시스템 변경되었을때
                    if (beforeSystemStatus != GlobalVar.SystemStatus &&
                     GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
                    {
                        FuncMotion.MoveStop(SV01_Out_Shuttle); //정지상태 되면 서보 정지
                        Util.InitWatch(ref watch);
                    }
                    #endregion
                    // 1. [TimeOut 관리] 시스템 상태에 따른 타이머 제어 (Pause / Resume)
                    if (GlobalVar.SystemStatus != enumSystemStatus.AutoRun)
                    {
                        if (watch.IsRunning) watch.Stop();
                    }
                    else
                    {
                        // AutoRun이고, 타이머가 멈춰있으며, Waiting이 아닐 때 재개
                        if (!watch.IsRunning && OutShuttleAction != OutShuttle_enumAction.Waiting)
                        {
                            watch.Start();
                        }
                    }
                    #region if AutoRun
                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                    {
                        // 2-1. Action 상태 변경 감지 -> 타이머 리셋
                        if (OutShuttleAction != OutShuttlebeforeAction)
                        {
                            watch.Restart();
                            OutShuttlebeforeAction = OutShuttleAction;
                        }

                        // 2-2. TimeOut 체크 (Waiting이 아닐 때)
                        if (OutShuttleAction != OutShuttle_enumAction.Waiting &&
                            OutConveyorAction != OutConveyor_enumAction.Waiting &&
                            NgbufferAction != Ngbuffer_enumAction.Waiting &&
                            watch.ElapsedMilliseconds > ActionTimeout)
                        {
                            watch.Stop();

                            // 에러 발생 및 로그
                            string errReason = $"{OutShuttleAction} Timeout. (Elapsed: {watch.ElapsedMilliseconds}ms)";
                            FuncLog.WriteLog($"[ERROR] {Name} : {errReason}");

                            FuncInline.AddError(FuncInline.enumErrorPart.OutShuttle_Up, FuncInline.enumErrorCode.Conveyor_Timeout, errReason);

                            // 안전 조치: 모터 정지 및 상태 초기화                         
                            OutShuttleAction = OutShuttle_enumAction.Waiting;
                            loadingStep = 0;

                            continue; // 에러 처리 후 루프 재시작
                        }

                        Logic_OutShuttle();
                        Logic_OutConveyor();
                        Logic_Ngbuffer();
                    }
                    #endregion

                    #region else AutoRun 아닐때
                    //AutoRun이 아닐때 대기
                    else
                    {
                        // AutoRun이 아닐 때 필요한 초기화가 있다면 수행
                        if (watch.IsRunning) watch.Reset();
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
                    if (OutShuttlebeforeAction != OutShuttleAction)
                    {
                        debug("action change " + OutShuttlebeforeAction.ToString() + " ==> " + OutShuttleAction.ToString());
                        Util.ResetWatch(ref watch);
                    }
                    if (OutConveyorbeforeAction != OutConveyorAction)
                    {
                        debug("action change " + OutConveyorbeforeAction.ToString() + " ==> " + OutConveyorAction.ToString());
                        Util.ResetWatch(ref watch);
                    }
                    OutShuttlebeforeAction = OutShuttleAction;
                    OutConveyorbeforeAction = OutConveyorAction;
                    NgbufferbeforeAction = NgbufferAction;
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
        #region 1. OutShuttle Logic
        private void Logic_OutShuttle()
        {
            // 상/하단 PCB 정보 및 목적지 확인용 변수
            var shuttleUp = PCBInfo[(int)enumTeachingPos.OutShuttle_Up];
            var shuttleDown = PCBInfo[(int)enumTeachingPos.OutShuttle_Down];

            switch (OutShuttleAction)
            {
                case OutShuttle_enumAction.Waiting:
                    #region Waiting
                    // 1. [배출 우선] 셔틀에 제품이 있는 경우 -> 배출 위치로 이동
                    if (shuttleUp.PCBStatus != enumSMDStatus.UnKnown || shuttleDown.PCBStatus != enumSMDStatus.UnKnown)
                    {
                        Log = $"{Name} PCB Detected on Shuttle (UP:{shuttleUp.PCBStatus}/DOWN:{shuttleDown.PCBStatus}). Start UnLoading.";
                        FuncLog.WriteLog(Log);
                        OutShuttleAction = OutShuttle_enumAction.MoveUnLoadingPos;
                    }
                    // 2. [로딩 대기] 제품이 없는 경우 -> Front/Rear Rack 배출 요청 확인
                    else
                    {
                        // 2-1. Front Rack 확인
                        // FrontRack이 UnLoading 중이고, 그 목적지가 OutShuttle인 경우
                        if (AutoInline.Class.FrontRack.Action == FrontRack.enumAction.UnLoading)
                        {
                            var frontDest = PCBInfo[(int)enumTeachingPos.Lift1_Up].Destination;
                            if (frontDest == enumTeachingPos.OutShuttle_Up || frontDest == enumTeachingPos.OutShuttle_Down)
                            {
                                CurrentSource = enumTeachingPos.Lift1_Up; // 소스 저장
                                Log = $"{Name} FrontRack UnLoading Request Detected. Destination: {frontDest}";
                                FuncLog.WriteLog(Log);

                                //Rear 턴상태면 MOVETurnPos로 
                                if (IsRearPosition())
                                {
                                    Log = $"{Name} MoveTurn Position Action";
                                    FuncLog.WriteLog(Log);
                                    OutShuttleAction = OutShuttle_enumAction.MoveTurnPos;
                                }
                                else
                                {
                                    Log = $"{Name} MoveFrontLoadingPos Position Action";
                                    OutShuttleAction = OutShuttle_enumAction.MoveFrontLoadingPos;
                                }

                            }
                        }
                        // 2-2. Rear Rack 확인
                        // RearRack 로직과 연동 (RearRack 구현에 따라 조건문 수정 필요)
                        else if (AutoInline.Class.RearRack.Action == RearRack.enumAction.UnLoading)
                        {
                            var rearDest = PCBInfo[(int)enumTeachingPos.Lift2_Up].Destination;
                            if (rearDest == enumTeachingPos.OutShuttle_Up || rearDest == enumTeachingPos.OutShuttle_Down)
                            {
                                CurrentSource = enumTeachingPos.Lift2_Up; // 소스 저장
                                Log = $"{Name} RearRack UnLoading Request Detected. Destination: {rearDest}";
                                FuncLog.WriteLog(Log);

                                //Front 턴상태면 MOVETurnPos로 
                                if (!IsRearPosition())
                                {
                                    Log = $"{Name} MoveTurn Position Action";
                                    FuncLog.WriteLog(Log);
                                    OutShuttleAction = OutShuttle_enumAction.MoveTurnPos;
                                }
                                else
                                {
                                    Log = $"{Name} MoveRearLoadingPos Position Action";
                                    OutShuttleAction = OutShuttle_enumAction.MoveRearLoadingPos;
                                }
                            }
                        }
                        else
                        {
                            //Rear 턴상태면 MOVETurnPos로 
                            if (IsRearPosition())
                            {
                                Log = $"{Name} MoveTurn Position Action";
                                FuncLog.WriteLog(Log);
                                OutShuttleAction = OutShuttle_enumAction.MoveTurnPos;
                            }
                            else
                            {
                                int LoadingPos = (int)FuncInline.enumShuttlePos.OutShuttle_FrontLiftLoading;
                                if (FuncInlineMove.IsArrived(SV01_Out_Shuttle, ShuttlePos[(int)outshuttle, LoadingPos]))
                                {
                                    //로딩위치면 대기
                                }
                                else
                                {
                                    //Rear 턴상태면 MOVETurnPos로 
                                    if (IsRearPosition())
                                    {
                                        Log = $"{Name} MoveTurn Position Action";
                                        FuncLog.WriteLog(Log);
                                        OutShuttleAction = OutShuttle_enumAction.MoveTurnPos;
                                    }
                                    else
                                    {
                                        Log = $"{Name} MoveFrontLoadingPos Position Action";
                                        OutShuttleAction = OutShuttle_enumAction.MoveFrontLoadingPos;
                                    }

                                }

                            }
                        }
                    }

                    #endregion
                    break;

                case OutShuttle_enumAction.MoveTurnPos:
                    #region Move to Loading Pos
                    TargetPos = ShuttlePos[(int)outshuttle, (int)enumShuttlePos.OutShuttle_TurnPosition];

                    // 서보 이동 및 도착 확인
                    if (FuncInlineMove.IsArrived(SV01_Out_Shuttle, TargetPos))
                    {
                        // 도착 후 턴 동작
                        // CurrentSource에 따라 방향 결정 (Lift1=Front, Lift2=Rear)
                        if (CurrentSource == enumTeachingPos.Lift1_Up)
                        {
                            // Front를 봐야 하므로 CCW (가정)
                            DIO.WriteDOData(enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder, true);
                            DIO.WriteDOData(enumDONames.Y304_3_Out_Shuttle_Turn_Cw_Cylinder, false);

                            if (X_Turn_CCW_Sensor)
                            {
                                Log = $"{Name} Turn CCW Complete. Move to FrontLoadingPos.";
                                FuncLog.WriteLog(Log);
                                OutShuttleAction = OutShuttle_enumAction.MoveFrontLoadingPos;
                            }
                        }
                        else if (CurrentSource == enumTeachingPos.Lift2_Up)// Lift2_Up (Rear)
                        {
                            // Rear를 봐야 하므로 CW (가정)
                            DIO.WriteDOData(enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder, false);
                            DIO.WriteDOData(enumDONames.Y304_3_Out_Shuttle_Turn_Cw_Cylinder, true);

                            if (X_Turn_CW_Sensor)
                            {
                                Log = $"{Name} Turn CW Complete. Move to RearLoadingPos.";
                                FuncLog.WriteLog(Log);
                                OutShuttleAction = OutShuttle_enumAction.MoveRearLoadingPos;
                            }
                        }
                        else
                        {
                            // Front를 봐야 하므로 CCW (가정)
                            DIO.WriteDOData(enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder, true);
                            DIO.WriteDOData(enumDONames.Y304_3_Out_Shuttle_Turn_Cw_Cylinder, false);

                            if (X_Turn_CCW_Sensor)
                            {
                                Log = $"{Name}Turn CCW Complete. Waiting Move to FrontLoadingPos.";
                                FuncLog.WriteLog(Log);
                                OutShuttleAction = OutShuttle_enumAction.MoveFrontLoadingPos;
                            }
                        }
                    }
                    else
                    {
                        if (GlobalVar.AxisStatus[SV01_Out_Shuttle].StandStill)
                        {
                            Log = $"{Name} Move to MoveTurnPos Position";
                            FuncLog.WriteLog(Log);
                            FuncInlineMove.MoveAbsolute((uint)SV01_Out_Shuttle, TargetPos);
                        }
                    }
                    #endregion
                    break;
                case OutShuttle_enumAction.MoveFrontLoadingPos:
                    #region MoveFrontLoadingPos
                    TargetPos = ShuttlePos[(int)outshuttle, (int)enumShuttlePos.OutShuttle_FrontLiftLoading];
                    if (FuncInlineMove.IsArrived(SV01_Out_Shuttle, TargetPos))
                    {
                        Log = $"{Name} Start FrontLoading action";
                        FuncLog.WriteLog(Log);
                        loadingStep = 0; // 로딩 스텝 초기화
                        OutShuttleAction = OutShuttle_enumAction.FrontLoading;
                    }
                    else
                    {
                        if (GlobalVar.AxisStatus[SV01_Out_Shuttle].StandStill)
                        {
                            Log = $"{Name} Move to FrontLoadingPos Position";
                            FuncLog.WriteLog(Log);
                            FuncInlineMove.MoveAbsolute((uint)SV01_Out_Shuttle, TargetPos);
                        }
                    }
                    #endregion
                    break;
                case OutShuttle_enumAction.MoveRearLoadingPos:
                    #region Move to Loading Pos
                    TargetPos = ShuttlePos[(int)outshuttle, (int)enumShuttlePos.OutShuttle_RearRackLoading];
                    if (FuncInlineMove.IsArrived(SV01_Out_Shuttle, TargetPos))
                    {
                        Log = $"{Name} Start RearLoading action";
                        FuncLog.WriteLog(Log);
                        loadingStep = 0; // 로딩 스텝 초기화
                        //작성해야함
                        OutShuttleAction = OutShuttle_enumAction.RearLoading;
                    }
                    else
                    {
                        if (GlobalVar.AxisStatus[SV01_Out_Shuttle].StandStill)
                        {
                            Log = $"{Name} Move to RearLoadingPos Position";
                            FuncLog.WriteLog(Log);
                            FuncInlineMove.MoveAbsolute((uint)SV01_Out_Shuttle, TargetPos);
                        }
                    }
                    #endregion
                    break;
                case OutShuttle_enumAction.MoveUnLoadingPos:
                    #region Move Unloading Pos
                    TargetPos = ShuttlePos[(int)outshuttle, (int)enumShuttlePos.OutShuttle_OutCovyUnLoading];
                    if (FuncInlineMove.IsArrived(SV01_Out_Shuttle, TargetPos))
                    {
                        Log = $"{Name} Start UnLoading action";
                        FuncLog.WriteLog(Log);
                        OutShuttleAction = OutShuttle_enumAction.UnLoading;
                    }
                    else
                    {
                        if (GlobalVar.AxisStatus[SV01_Out_Shuttle].StandStill)
                        {
                            Log = $"{Name} Move to UnLoadingPos Position";
                            FuncLog.WriteLog(Log);
                            FuncInlineMove.MoveAbsolute((uint)SV01_Out_Shuttle, TargetPos);
                        }
                    }
                    #endregion
                    break;
                case OutShuttle_enumAction.FrontLoading:
                case OutShuttle_enumAction.RearLoading:
                    #region Loading (Common Logic with Step Control & Motor Dir)
                    // 1. 스토퍼 Open (진입 허용 - 함수 내부에서 방향 처리됨)
                    Stopper_IN_Open(true);
                    Stopper_Out_Open(false);

                    enumTeachingPos dest = PCBInfo[(int)CurrentSource].Destination;
                    bool isUpLoad = dest == enumTeachingPos.OutShuttle_Up;
                    bool isDownLoad = dest == enumTeachingPos.OutShuttle_Down;

                    // Rear Loading 여부 확인 (모터 방향 반전용)
                    bool isRear = OutShuttleAction == OutShuttle_enumAction.RearLoading;

                    // [Step 0] 컨베이어 구동 및 도착 센서 대기
                    if (loadingStep == 0)
                    {
                        if (isUpLoad)
                        {
                            if (isRear) DIO.WriteDOData(enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw, true); // Rear: CCW
                            else DIO.WriteDOData(enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, true);  // Front: CW
                        }
                        if (isDownLoad)
                        {
                            if (isRear) DIO.WriteDOData(enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw, true); // Rear: CCW
                            else DIO.WriteDOData(enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, true);  // Front: CW
                        }

                        // 도착 센서 감지
                        bool arrived = (isUpLoad && X_OK_PCB_Stop_Sensor) || (isDownLoad && X_NG_PCB_Stop_Sensor);

                        if (arrived)
                        {
                            if (FuncInline.IsDelayOver(Key_OutShuttle_Load, 300))
                            {
                                // 1차 정지 (안전을 위해 CW/CCW 모두 Off)
                                DIO.WriteDOData(enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, false);
                                DIO.WriteDOData(enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw, false);
                                DIO.WriteDOData(enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, false);
                                DIO.WriteDOData(enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw, false);
                                loadingStep = 10;
                            }
                        }
                        else
                        {
                            FuncInline.ResetDelay(Key_OutShuttle_Load);
                        }
                    }
                    // [Step 10] 잠시 대기 후 오버드라이브 준비
                    else if (loadingStep == 10)
                    {
                        if (FuncInline.IsDelayOver(Key_OutShuttle_Load, 100))
                        {
                            Log = $"{Name} Loading Overdrive Start.";
                            FuncLog.WriteLog(Log);

                            // 재구동 (방향 맞춰서)
                            if (isUpLoad)
                            {
                                if (isRear) DIO.WriteDOData(enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw, true);
                                else DIO.WriteDOData(enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, true);
                            }
                            if (isDownLoad)
                            {
                                if (isRear) DIO.WriteDOData(enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw, true);
                                else DIO.WriteDOData(enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, true);
                            }
                            loadingStep = 20;
                        }
                    }
                    // [Step 20] 오버드라이브 (확실한 밀착)
                    else if (loadingStep == 20)
                    {
                        if (FuncInline.IsDelayOver(Key_OutShuttle_Load, 1500))
                        {
                            // 최종 정지 (CW/CCW 모두 Off)
                            DIO.WriteDOData(enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, false);
                            DIO.WriteDOData(enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw, false);
                            DIO.WriteDOData(enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, false);
                            DIO.WriteDOData(enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw, false);

                            Stopper_IN_Open(false); // 스토퍼 닫음

                            Log = $"{Name} Loading Physical Complete.";
                            FuncLog.WriteLog(Log);

                            loadingStep = 0;
                            if (OutShuttleAction == OutShuttle_enumAction.FrontLoading)
                                OutShuttleAction = OutShuttle_enumAction.FrontLoadingCheck;
                            else
                                OutShuttleAction = OutShuttle_enumAction.RearLoadingCheck;
                        }
                    }
                    #endregion
                    break;


                case OutShuttle_enumAction.FrontLoadingCheck:
                case OutShuttle_enumAction.RearLoadingCheck:
                    #region LoadingCheck (Data Move)
                    enumTeachingPos targetPos = (PCBInfo[(int)CurrentSource].Destination == enumTeachingPos.OutShuttle_Up) ?
                        enumTeachingPos.OutShuttle_Up : enumTeachingPos.OutShuttle_Down;

                    FuncInline.MovePCBInfo(CurrentSource, targetPos);

                    if (PCBInfo[(int)targetPos].PCBStatus != enumSMDStatus.UnKnown)
                    {
                        Log = $"{Name} Loading Data Transfer Complete. ID: {PCBInfo[(int)targetPos].Num}";
                        FuncLog.WriteLog(Log);
                        OutShuttleAction = OutShuttle_enumAction.Waiting;
                    }
                    #endregion
                    break;

                case OutShuttle_enumAction.UnLoading:
                    #region UnLoading (Conveyor Run for Discharge)
                    // 1. 배출 스토퍼 Open (공통 동작)
                    Stopper_IN_Open(false);
                    Stopper_Out_Open(true);

                    // 2. 데이터 존재 여부 확인
                    bool hasUpData = shuttleUp.PCBStatus != enumSMDStatus.UnKnown;
                    bool hasDownData = shuttleDown.PCBStatus != enumSMDStatus.UnKnown;

                    // 3. 뒷 설비 준비 상태 확인
                    bool canDischargeUp = AutoInline.Class.OutShuttle.OutConveyorAction == OutConveyor_enumAction.Loading ||
                                             AutoInline.Class.OutShuttle.OutConveyorAction == OutConveyor_enumAction.LoadingCheck;

                    bool canDischargeDown = AutoInline.Class.OutShuttle.NgbufferAction == Ngbuffer_enumAction.Loading ||
                                                AutoInline.Class.OutShuttle.NgbufferAction == Ngbuffer_enumAction.LoadingCheck; ;

                    // =============================================================
                    // Case 1: Up & Down 둘 다 제품이 있는 경우
                    // =============================================================
                    if (hasUpData && hasDownData)
                    {
                        // A. 타임아웃 예외 처리 (둘 다 나갈 곳이 없으면 대기)
                        if (!canDischargeUp && !canDischargeDown)
                        {
                            watch.Restart(); // 대기 중 (에러 아님)
                        }

                        // B. 모터 구동 (준비된 쪽만 구동)
                        if (canDischargeUp) DIO.WriteDOData(enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, true);
                        if (canDischargeDown) DIO.WriteDOData(enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, true);

                        // C. 센서 확인 (구동 중인 라인의 센서가 켜져 있으면 리셋)
                        bool upMoving = canDischargeUp && X_OK_PCB_Stop_Sensor;
                        bool downMoving = canDischargeDown && X_NG_PCB_Stop_Sensor;

                        if (upMoving || downMoving)
                        {
                            FuncInline.ResetDelay(Key_OutShuttle_Unload);
                        }
                        else
                        {
                            // 움직일 수 있는 애들은 다 빠져나감 -> 1초 오버드라이브 후 완료
                            if (FuncInline.IsDelayOver(Key_OutShuttle_Unload, 2000))
                            {
                                // 하나라도 배출 시도를 했다면 Check로 이동
                                if (canDischargeUp || canDischargeDown)
                                    OutShuttleAction = OutShuttle_enumAction.UnLoadingCheck;
                            }
                        }
                    }
                    // =============================================================
                    // Case 2: Up에만 제품이 있는 경우
                    // =============================================================
                    else if (hasUpData)
                    {
                        if (!canDischargeUp)
                        {
                            watch.Restart(); // 대기
                        }
                        else
                        {
                            DIO.WriteDOData(enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, true);

                            if (X_OK_PCB_Stop_Sensor)
                            {
                                FuncInline.ResetDelay(Key_OutShuttle_Unload);
                            }
                            else
                            {
                                if (FuncInline.IsDelayOver(Key_OutShuttle_Unload, 1000))
                                {
                                    OutShuttleAction = OutShuttle_enumAction.UnLoadingCheck;
                                }
                            }
                        }
                    }
                    // =============================================================
                    // Case 3: Down에만 제품이 있는 경우
                    // =============================================================
                    else if (hasDownData)
                    {
                        if (!canDischargeDown)
                        {
                            watch.Restart(); // 대기
                        }
                        else
                        {
                            DIO.WriteDOData(enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, true);

                            if (X_NG_PCB_Stop_Sensor)
                            {
                                FuncInline.ResetDelay(Key_OutShuttle_Unload);
                            }
                            else
                            {
                                if (FuncInline.IsDelayOver(Key_OutShuttle_Unload, 1000))
                                {
                                    OutShuttleAction = OutShuttle_enumAction.UnLoadingCheck;
                                }
                            }
                        }
                    }
                    // 예외: 데이터가 없는데 여기 들어온 경우 (바로 탈출)
                    else
                    {
                        OutShuttleAction = OutShuttle_enumAction.UnLoadingCheck;
                    }
                    #endregion
                    break;
                case OutShuttle_enumAction.UnLoadingCheck:
                    #region UnLoadingCheck (Data Move & Finish)

                    // 1. 모터 안전 정지 (공통)
                    DIO.WriteDOData(enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, false);
                    DIO.WriteDOData(enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, false);
                    Stopper_Out_Open(false);

                    // 데이터 상태 다시 확인
                    bool chkUpData = shuttleUp.PCBStatus != enumSMDStatus.UnKnown;
                    bool chkDownData = shuttleDown.PCBStatus != enumSMDStatus.UnKnown;
              
                    // 뒷 설비 상태 다시 확인 (데이터 이동 조건)
                    bool chkUpReady = AutoInline.Class.OutShuttle.OutConveyorAction == OutConveyor_enumAction.Loading ||
                                        AutoInline.Class.OutShuttle.OutConveyorAction == OutConveyor_enumAction.LoadingCheck;
                    bool chkDownReady = AutoInline.Class.OutShuttle.NgbufferAction == Ngbuffer_enumAction.Loading ||
                                            AutoInline.Class.OutShuttle.NgbufferAction == Ngbuffer_enumAction.LoadingCheck;

                    // =============================================================
                    // Case 1 Check: Up & Down 둘 다 있었던 경우 (혹은 현재 둘 다 있는 경우)
                    // =============================================================
                    if (chkUpData && chkDownData)
                    {
                        // 준비된 쪽 데이터 이동
                        if (chkUpReady) FuncInline.MovePCBInfo(enumTeachingPos.OutShuttle_Up, enumTeachingPos.OutConveyor);
                        if (chkDownReady) FuncInline.MovePCBInfo(enumTeachingPos.OutShuttle_Down, enumTeachingPos.NgBuffer);
                    }
                    // =============================================================
                    // Case 2 Check: Up만 있는 경우
                    // =============================================================
                    else if (chkUpData)
                    {
                        if (chkUpReady) FuncInline.MovePCBInfo(enumTeachingPos.OutShuttle_Up, enumTeachingPos.OutConveyor);
                    }
                    // =============================================================
                    // Case 3 Check: Down만 있는 경우
                    // =============================================================
                    else if (chkDownData)
                    {
                        if (chkDownReady) FuncInline.MovePCBInfo(enumTeachingPos.OutShuttle_Down, enumTeachingPos.NgBuffer);
                    }

                    // 2. 최종 완료 확인
                    // 셔틀이 완전히 비워졌으면 Waiting
                    if (shuttleUp.PCBStatus == enumSMDStatus.UnKnown && shuttleDown.PCBStatus == enumSMDStatus.UnKnown)
                    {
                        Log = $"{Name} UnLoading Complete. Shuttle Empty.";
                        FuncLog.WriteLog(Log);
                        OutShuttleAction = OutShuttle_enumAction.Waiting;
                    }
                    else
                    {
                        // 하나라도 남아있으면(뒷설비 Full 등으로 인해) Waiting으로 갔다가 다시 시도
                        // (Waiting 로직에서 데이터가 있으면 다시 UnLoading으로 보냄 -> 무한루프로 재시도 효과)
                        Log = $"{Name} UnLoading Partial Complete. Retry Remaining.";
                        FuncLog.WriteLog(Log);
                        OutShuttleAction = OutShuttle_enumAction.Waiting;
                    }
                    #endregion
                    break;
               

            }
        }
        #endregion

        #region 2. OutConveyor Logic
        private void Logic_OutConveyor()
        {
            var outConvInfo = PCBInfo[(int)enumTeachingPos.OutConveyor];

            switch (OutConveyorAction)
            {
                case OutConveyor_enumAction.Waiting:
                    // 1. Loading 조건: 내 자리가 비어있고, 입구 센서가 감지되거나 OutShuttle이 Unloading 중일 때
                    if (outConvInfo.PCBStatus == enumSMDStatus.UnKnown)
                    {
                        if (X_OutConveyor_PCB_IN_Sensor ||  //수동 투입할때
                            (AutoInline.Class.OutShuttle.OutShuttleAction == OutShuttle_enumAction.UnLoading &&
                            PCBInfo[(int)enumTeachingPos.OutShuttle_Up].PCBStatus != enumSMDStatus.UnKnown))
                        {
                            Log = $"{OutCvyName} PCB Entrance Detected. Start Loading.";
                            FuncLog.WriteLog(Log);
                            OutConveyorAction = OutConveyor_enumAction.Loading;
                        }
                    }
                    // 2. UnLoading 조건: 내 자리에 제품이 있고, 뒷 설비가 준비되었을 때 (SMEMA 등)
                    else if (outConvInfo.PCBStatus != enumSMDStatus.UnKnown && X_SMEMA_After_Ready)
                    {
                        // TODO: 뒷설비 Ready 신호 확인 (현재는 바로 Unloading으로 넘김)
                        // if (NextMachineReady)
                        Log = $"{OutCvyName} Y412_1_SMEMA_After_Ready -> ON";
                        FuncLog.WriteLog(Log);
                        DIO.WriteDOData(FuncInline.enumDONames.Y412_1_SMEMA_After_Ready, true);

                        if (X_SMEMA_After_AutoInline && outConvInfo.PCBStatus != enumSMDStatus.Bypass)
                        {
                            Log = $"{OutCvyName}[PCB_ID:{outConvInfo.Num}]PCB Test Ok. Y404_5_SMEMA_After_Pass -> ON";
                            FuncLog.WriteLog(Log);
                            DIO.WriteDOData(FuncInline.enumDONames.Y404_5_SMEMA_After_Pass, true);
                        }
                        else
                        {
                            Log = $"{OutCvyName}[PCB_ID:{outConvInfo.Num} PCB Test Ok. UnLoading Action";
                            FuncLog.WriteLog(Log);
                        }
                        OutConveyorAction = OutConveyor_enumAction.UnLoading;
                    }

                    break;

                case OutConveyor_enumAction.Loading:
                    #region Loading
                    if (Y_OutConveyor_Motor_CW)
                    {
                        DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, true);
                    }
                    // 도착 센서 감지
                    if (X_OutConveyor_PCB_Stop_Sensor)
                    {
                        Log = $"{OutCvyName}Loading OK, LoadingCheck Action";
                        FuncLog.WriteLog(Log);
                        DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);
                        OutConveyorAction = OutConveyor_enumAction.LoadingCheck;
                    }
                    else
                    {
                        FuncInline.ResetDelay(Key_OutConv_Load);
                    }
                    #endregion
                    break;

                case OutConveyor_enumAction.LoadingCheck:
                    #region LoadingCheck (Data Move)
                    // 1. 데이터 이동 (OutShuttle -> OutConveyor)
                    if (FuncInline.IsDelayOver(Key_OutConv_Load, 500))
                    {
                        FuncInline.MovePCBInfo(enumTeachingPos.OutShuttle_Up, enumTeachingPos.OutConveyor);
                    }
                    // 2. 데이터 확인
                    if (PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus != enumSMDStatus.UnKnown &&
                        PCBInfo[(int)enumTeachingPos.OutShuttle_Up].PCBStatus == enumSMDStatus.UnKnown)
                    {
                        Log = $"{OutCvyName}[PCB_ID:{outConvInfo.Num}]Loading Complete.";
                        FuncLog.WriteLog(Log);
                        OutConveyorAction = OutConveyor_enumAction.Waiting;

                    }
                    else
                    {
                        // 센서는 감지됐는데 데이터가 없는 경우 (수동 투입 or 에러)
                        // 일단 Waiting으로 가서 다시 판단하거나 에러 처리
                        if (X_OutConveyor_PCB_Stop_Sensor)
                        {
                            // 임시: 데이터 생성 혹은 에러
                            // outConvInfo.PCBStatus = enumSMDStatus.Exist; 
                            OutConveyorAction = OutConveyor_enumAction.Waiting;
                        }
                    }
                    #endregion
                    break;

                case OutConveyor_enumAction.UnLoading:
                    #region UnLoading (Hardware Only)
                    if (!Y_OutConveyor_Motor_CW)
                    {
                        DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, true);
                    }

                    // 배출 완료 확인 (센서 OFF)
                    if (!X_OutConveyor_PCB_Stop_Sensor)
                    {
                        //받았으면 꺼준다
                        if (!X_SMEMA_After_Ready)
                        {
                            Log = $"{OutCvyName} Y412_1_SMEMA_After_Ready -> Off.";
                            FuncLog.WriteLog(Log);
                            DIO.WriteDOData(enumDONames.Y412_1_SMEMA_After_Ready, false);
                        }
                        else
                        {
                            break;
                        }
                        // 완전히 나가도록 2초 더 구동
                        if (FuncInline.IsDelayOver(Key_OutConv_Unload, 2000))
                        {
                            DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);
                            Log = $"{OutCvyName}[PCB_ID:{outConvInfo.Num}] UnLoadingCheck Ok.";
                            FuncLog.WriteLog(Log);

                            OutConveyorAction = OutConveyor_enumAction.UnLoadingCheck;

                            //2초지나고 Pass신호는 꺼준다
                            if (!Y_SMEMA_After_Pass)
                            {
                                Log = $"{OutCvyName}Y404_5_SMEMA_After_Pass -> OFF.";
                                FuncLog.WriteLog(Log);
                                DIO.WriteDOData(enumDONames.Y404_5_SMEMA_After_Pass, false);
                            }
                        }
                    }
                    else
                    {
                        FuncInline.ResetDelay(Key_OutConv_Unload);
                    }
                    #endregion
                    break;

                case OutConveyor_enumAction.UnLoadingCheck:
                    #region UnLoadingCheck (Data Move)
                    //0.카운트? 실적 처리 생각

                    // 1. 데이터 소멸 (다음 설비로 이동 처리)                        
                    ClearPCBInfo(enumTeachingPos.OutConveyor);
                    // 2. 데이터 확인 (없어진 것 확인)
                    if (PCBInfo[(int)enumTeachingPos.OutConveyor].PCBStatus == enumSMDStatus.UnKnown)
                    {
                        Log = $"{OutCvyName} UnLoadingCheck Complete";
                        FuncLog.WriteLog(Log);
                        DIO.WriteDOData(enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);
                        OutConveyorAction = OutConveyor_enumAction.Waiting;

                    }
                    #endregion
                    break;
            }
        }
        #endregion

        #region 3. NG Buffer Logic
        private void Logic_Ngbuffer()
        {
            var ngInfo = PCBInfo[(int)enumTeachingPos.NgBuffer];

            switch (NgbufferAction)
            {
                case Ngbuffer_enumAction.Waiting:
                    // 1. Loading 조건: 비어있고, 센서 감지 or 셔틀 NG 배출 중
                    if (ngInfo.PCBStatus == enumSMDStatus.UnKnown)
                    {
                        if (X_NGbuffer_PCB_IN_Sensor ||
                           (AutoInline.Class.OutShuttle.OutShuttleAction == OutShuttle_enumAction.UnLoading &&
                            PCBInfo[(int)enumTeachingPos.OutShuttle_Down].PCBStatus != enumSMDStatus.UnKnown))
                        {
                            Log = $"{NGName} NG PCB Entrance Detected. Start Loading.";
                            FuncLog.WriteLog(Log);
                            NgbufferAction = Ngbuffer_enumAction.Loading;
                        }
                    }
                    break;

                case Ngbuffer_enumAction.Loading:
                    #region Loading
                    DIO.WriteDOData(enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw, true);

                    if (X_NGbuffer_PCB_Stop_Sensor)
                    {
                        Log = $"{OutCvyName}Loading OK, LoadingCheck Action";
                        FuncLog.WriteLog(Log);
                        DIO.WriteDOData(enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw, false);
                        NgbufferAction = Ngbuffer_enumAction.LoadingCheck;

                    }
                  
                    #endregion
                    break;

                case Ngbuffer_enumAction.LoadingCheck:
                    #region LoadingCheck
                    if (ngInfo.PCBStatus != enumSMDStatus.UnKnown)
                    {
                        Log = $"{NGName}[PCB_ID:{ngInfo.Num}] Loading Complete.";
                        FuncLog.WriteLog(Log);
                        // 2. 상세 NG 정보 수집 (Array별 바코드 및 에러 내역)
                        string ngDetails = $"PCB ID: {ngInfo.Num} Removal Request.\n"; // 알람 메시지 헤더

                        for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                        {
                            // 바코드가 존재하는(=유효한) 어레이만 확인
                            if (ngInfo.Barcode[i].Length > 0)
                            {
                                int errCode = ngInfo.ErrorCode[i];
                                string errName = "Unknown Error";

                                // 에러 코드명을 가져옴 (배열 범위 체크 및 null 체크 포함)
                                if (errCode >= 0 && errCode < FuncInline.TestErrorCode.Length &&
                                    FuncInline.TestErrorCode[errCode] != null)
                                {
                                    errName = FuncInline.TestErrorCode[errCode];
                                }

                                // 로그 포맷: [Array번호] 바코드 : 에러번호(에러명)
                                ngDetails += $"[Arr{i + 1}] {ngInfo.Barcode[i]} : {errCode}({errName})\n";
                            }
                        }
                        // 3. 에러 발생 (수집된 상세 정보를 메시지로 출력)
                        // FuncInline.enumErrorCode.NG_Buffer_Full 은 예시이며, 실제 알람 코드에 맞게 사용
                        FuncInline.AddError(FuncInline.enumErrorPart.NgBuffer,
                                            FuncInline.enumErrorCode.NG_Buffer_Full,
                                            ngDetails);
                        watch.Reset();
                        NgbufferAction = Ngbuffer_enumAction.UnLoading; // 수동 제거 대기
                        
                    }
                  
                    #endregion
                    break;

                case Ngbuffer_enumAction.UnLoading:
                    #region UnLoading 알람 발생 시키자

                    if (!X_NGbuffer_PCB_Stop_Sensor)
                    {
                        Log = $"{NGName}[PCB_ID:{ngInfo.Num}] UnLoadingCheck Action";
                        FuncLog.WriteLog(Log);
                        NgbufferAction = Ngbuffer_enumAction.UnLoadingCheck;
                    }
                    
                    #endregion
                    break;

                case Ngbuffer_enumAction.UnLoadingCheck:
                    #region UnLoadingCheck (Data Move)
                    // 1. 데이터 소멸 (작업자가 가져감)
                    //PCBInfo[(int)enumTeachingPos.NgBuffer].PCBStatus = enumSMDStatus.UnKnown;
                    ClearPCBInfo(enumTeachingPos.NgBuffer);
                    // 2. 완료 확인
                    if (PCBInfo[(int)enumTeachingPos.NgBuffer].PCBStatus == enumSMDStatus.UnKnown)
                    {
                        Log = $"{NGName}[PCB_ID:{ngInfo.Num}]NG PCB Remove Complete";
                        FuncLog.WriteLog(Log);
                        NgbufferAction = Ngbuffer_enumAction.Waiting;
                    }
                    #endregion
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 현재 셔틀이 Rear(CW) 위치인지 확인합니다.
        /// (센서 감지 또는 출력 신호로 판단)
        /// </summary>
        private bool IsRearPosition()
        {
            // CW(Rear) 센서가 감지되었거고, CW 출력이 나가고 있다면 Rear로 판단
            if ((X_Turn_CW_Sensor && Y_Turn_CW_Cylinder) &&
                (!X_Turn_CCW_Sensor || !Y_Turn_CCW_Cylinder))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 진입(IN) 스토퍼 제어 (Turn 위치에 따라 가변)
        /// </summary>
        /// <param name="isUp">true: 스토퍼 상승(On), false: 하강(Off)</param>
        public void Stopper_IN_Open(bool isUp)
        {
            FuncInline.enumDONames targetSol;

            if (IsRearPosition())
            {
                // [Rear/CW 상태] 진입 방향이 반대이므로 물리적 OUT 솔레노이드를 동작
                targetSol = FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_Out_SOL;
            }
            else
            {
                // [Front/CCW 상태] 정상 방향이므로 물리적 IN 솔레노이드를 동작
                targetSol = FuncInline.enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_In_SOL;
            }

            DIO.WriteDOData(targetSol, isUp);
        }

        /// <summary>
        /// 배출(OUT) 스토퍼 제어 (Turn 위치에 따라 가변)
        /// </summary>
        /// <param name="isUp">true: 스토퍼 상승(On), false: 하강(Off)</param>
        public void Stopper_Out_Open(bool isUp)
        {
            FuncInline.enumDONames targetSol;

            if (IsRearPosition())
            {
                // [Rear/CW 상태] 배출 방향이 반대이므로 물리적 IN 솔레노이드를 동작
                targetSol = FuncInline.enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_In_SOL;
            }
            else
            {
                // [Front/CCW 상태] 정상 방향이므로 물리적 OUT 솔레노이드를 동작
                targetSol = FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_Out_SOL;
            }

            DIO.WriteDOData(targetSol, isUp);
        }
    }
}
