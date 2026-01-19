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
            InputTray,  //BeforeLiftClass_01.cs로 부터 Tray 공급 받음
            InputTrayCheck, //도착 확인
            WorkTrayClamp,   //샌딩 작업 준비
            WorkTray,   //샌딩 작업 중 대기
            OutputTray,   //WaitTrayClass_03.cs로 완료된 Tray 배출
            OutputTrayCheck   //배출 확인
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
            InputTray,  //BeforeLiftClass_01.cs로 부터 Tray 공급 받음
            InputTrayCheck, //도착 확인
            WorkTrayClamp,   //샌딩 작업 준비
            WorkTray,   //샌딩 작업 중 대기
            OutputTray,   //WaitTrayClass_03.cs로 완료된 Tray 배출
            OutputTrayCheck   //배출 확인
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

        /** @brief 시스템의 이전 상태 */
        private enumSystemStatus beforeSystemStatus = GlobalVar.SystemStatus;

        /** @brief 동작 수행시 타임아웃 체크 */
        private Stopwatch watch = new Stopwatch();
        /** @brief 한 공정 완료 여부. 각 하부 Part별로 완료여부 체크되면 컨베어 움직이고, 컨베어 움직이기 시작하면 완료여부 clear 하면 된다. */
        public bool StepFinish = false;

        /** @brief 현재 공정에서 작업중인 모델정보 */
        public string NowModel = "";
        public int SV01_Out_Shuttle = (int)FuncInline.enumServoAxis.SV01_Out_Shuttle;
      
        /** @brief 상승시 위치 */
        public double ready_pos = 0;
        /** @brief 버퍼 취출시 위치 */
        public double pickup_pos = 0;

        /** @brief 조립 최종 위치 */
        public double assemble_pos = 0;

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
        // ========================================================

        public string Name = "";
        #endregion

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
        public OutShuttle()
        {

            // 쓰레드를 시작한다
            actionThread = new Thread(ActionThread);
            actionThread.Start();
            Name = $"[OutShuttle]";
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
                    Y_IN_Stopper = DIO.GetDORead(FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_IN_SOL);
                    //OUT 스토퍼 동작
                    Y_OUT_Stopper = DIO.GetDORead(FuncInline.enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_Out_SOL);

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
                    X_NGbuffer_PCB_IN_Sensor = DIO.GetDIData(FuncInline.enumDINames.X402_2_Out_Conveyor_Ng_PCB_In_Sensor); ;
                    //PCB OK 라인 도착 감지 센서
                    X_NGbuffer_PCB_Stop_Sensor = DIO.GetDIData(FuncInline.enumDINames.X04_3_Out_Conveyor_NG_PCB_Stop_Sensor); ;

                    //4세대? 전용 =============================================
                    //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
                    X_Turn_Motor_Alarm = DIO.GetDIData(FuncInline.enumDINames.X304_4_Out_Shuttle_Turn_Motor_Alarm);
                    //인컨베이어에서 인셔틀로 지나가는 위치 PCB 인터락 감지센서(말굽)
                    X_Turn_Motor_HomeComplete = DIO.GetDIData(FuncInline.enumDINames.X304_6_Out_Shuttle_Turn_Motor_Home_Complete);
                    // ========================================================

                    #endregion

                    #region 시스템 상태 따라
                    switch (OutShuttleAction)
                    {
                        case OutShuttle_enumAction.Waiting:
                            #region Case Waiting
                            if (GlobalVar.LetsHoming &&
                               FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutConveyor] == false &&
                               FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutConveyor] == false)
                            {
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutConveyor] = true;
                                FuncLetsMotion.HomeRun((int)2);
                            }
                            if (GlobalVar.LetsHoming &&
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InConveyor] == false &&
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.InConveyor] == false &&
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutConveyor] == true)
                            {
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InConveyor] = true;
                                FuncLetsMotion.HomeRun((int)3);
                            }

                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                            {

                                if (FuncInline.CycleStop == true)
                                {
                                    Log = $"{Name} CycleStop 지령";
                                    FuncLog.WriteLog(Log);
                                    OutShuttleAction = OutShuttle_enumAction.CycleStop;

                                }
                                //동작해야 할때
                                if (!StepFinish)
                                {
                                    //공급 받아야할때
                                    if (((AutoInline)GlobalVar.Class).InShuttle.Action == InShuttle.enumAction.InPutCnyRun)  //투입 단계일때
                                    {
                                        Log = $"{Name} 트레이 공급 동작 지령";
                                        FuncLog.WriteLog(Log);
                                        OutShuttleAction = OutShuttle_enumAction.InputTray;
                                    }

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


                            if (true)
                            {

                                Log = $"{Name} 초기화 완료";
                                FuncLog.WriteLog(Log);
                                OutShuttleAction = OutShuttle_enumAction.InitFinish;
                            }
                            else
                            {

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
                            if (GlobalVar.LetsHoming &&
                               FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutConveyor] == false &&
                               FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutConveyor] == false)
                            {
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutConveyor] = true;
                                FuncLetsMotion.HomeRun((int)2);
                            }
                            if (GlobalVar.LetsHoming &&
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InConveyor] == false &&
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.InConveyor] == false &&
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutConveyor] == true)
                            {
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InConveyor] = true;
                                FuncLetsMotion.HomeRun((int)3);

                            }

                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                            {

                                if (FuncInline.CycleStop == true)
                                {
                                    Log = $"{Name} CycleStop 지령";
                                    FuncLog.WriteLog(Log);
                                    OutShuttleAction = OutShuttle_enumAction.CycleStop;

                                }
                                //동작해야 할때
                                if (!StepFinish)
                                {
                                    //공급 받아야할때
                                    if (((AutoInline)GlobalVar.Class).InShuttle.Action == InShuttle.enumAction.InPutCnyRun)  //투입 단계일때
                                    {
                                        Log = $"{Name} 트레이 공급 동작 지령";
                                        FuncLog.WriteLog(Log);
                                        OutShuttleAction = OutShuttle_enumAction.InputTray;
                                    }

                                }



                            }
                            Util.InitWatch(ref watch);
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
                                Log = $"{Name} {Enum.GetName(typeof(OutShuttle_enumAction), OutShuttleAction)} -> Waiting";
                                FuncLog.WriteLog(Log);
                                OutShuttleAction = OutShuttle_enumAction.Waiting;
                                break;
                            }

                            Util.InitWatch(ref watch);
                            break;
                        #endregion
                        case OutConveyor_enumAction.Init:
                            #region Case Init
                            // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행


                            if (true)
                            {

                                Log = $"{Name} 컨베이어 초기화 완료";
                                FuncLog.WriteLog(Log);
                                OutConveyorAction = OutConveyor_enumAction.InitFinish;
                            }
                            else
                            {

                            }

                            Util.ResetWatch(ref watch);
                            break;
                        #endregion
                        case OutConveyor_enumAction.InitFinish:
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

                        Util.InitWatch(ref watch);
                    }
                    #endregion

                    #region if AutoRun
                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                    {
                        if (watch.ElapsedMilliseconds > ActionTimeout)
                        {
                            FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                                                       DateTime.Now.ToString("HH:mm:ss"),
                                                                       FuncInline.enumErrorPart.OutShuttle_Up,
                                                                       FuncInline.enumErrorCode.PCB_Info_Move_Fail,
                                                                       false,
                                                                       ""));

                            //Action = enumAction.HomeMove;   //에러 발생시 준비위치로 이동 후 Wait
                            Util.InitWatch(ref watch);
                            continue;
                        }
                        switch (OutShuttleAction)
                        {

                            case OutShuttle_enumAction.HomeMove: //에러 발생 후 복귀 동작 후 -> Waiting으로
                                #region HomeMove
                                // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행

                                break;
                                #endregion


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

    }
}
