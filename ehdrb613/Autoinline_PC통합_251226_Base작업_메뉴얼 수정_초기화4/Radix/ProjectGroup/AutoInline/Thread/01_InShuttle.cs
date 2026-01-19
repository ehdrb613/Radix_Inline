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
      
            Move,   //이동 지령에 따라, 위치 지정, 턴이 필요하면 턴 포지션으로(턴상태로 확인?)
            MoveCheck,
            Turn,   //턴 포지션 확인 후 턴 진행, 추가 위치 이동 필요시 이동 후 컨베이어 동작
            TurnCheck,
            Loading,   //앞설비로 부터 로딩
            LoadingCheck, //트레이 투입동작
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
        public int SV00_In_Shuttle = (int)FuncInline.enumServoAxis.SV00_In_Shuttle;


        /** @brief 1층 트레이 위치 */
        public double ready_pos = 0;
        /** @brief 작업자 트레이 투입 위치 */
        public double input_pos = 0;


        const double tolerance = 1.0; // 허용 오차(mm)

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
        #endregion
        #region Di 출력부

        //PCB 진입부 감지 센서
        public static bool X_Pcb_In_Sensor = false;
        //PCB 도착 감지 센서
        public static bool X_Pcb_Stop_Sensor = false;
        //스토퍼 상승 센서
        public static bool X_Stopper_Cyl_Up_Sensor = false;

        //턴 실린더 정방향 센서(Rear 배출)
        public static bool X_Trun_Cw_Sensor = false;
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
                    #endregion
                    #region Di 출력부
                    
                    //PCB 진입부 감지 센서
                    X_Pcb_In_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor);
                    //PCB 도착 감지 센서
                    X_Pcb_Stop_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor);
                    //스토퍼 상승 센서
                    X_Stopper_Cyl_Up_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_2_In_Shuttle_Stopper_Cyl_Up_Sensor);
                    //턴 실린더 정방향 센서(Rear 배출)
                    X_Trun_Cw_Sensor = DIO.GetDIData(FuncInline.enumDINames.X302_6_In_Shuttle_Turn_Cw_Cyl_Sensor);
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

                    #endregion


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
                                    Log = "[InShuttle] CycleStop 지령";
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

                            if (!X_Turn_Ccw_Sensor)
                            {
                                Log = $"{Name} init - Turn Check";
                                DIO.WriteDOData(FuncInline.enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder, true);
                                DIO.WriteDOData(FuncInline.enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder, false);
                                continue;  
                            }
                           

                            if (GlobalVar.LetsHoming &&
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InShuttle] == false &&
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.InShuttle] == false)
                            {
                                Log = $"{Name} Init - InShuttle Width Homing Start";
                                FuncLog.WriteLog(Log);
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.InShuttle] = true;
                                FuncLetsMotion.HomeRun((int)0);

                            }
                            if (GlobalVar.LetsHoming &&
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutShuttle] == false &&
                                FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutShuttle] == false &&
                                 FuncInline.InitialDone[(int)FuncInline.enumInitialize.InShuttle] == true)
                            {
                                Log = $"{Name} Init - OutShuttle Width Homing Start";
                                FuncLog.WriteLog(Log);
                                FuncInline.InitialStarted[(int)FuncInline.enumInitialize.OutShuttle] = true;
                                FuncLetsMotion.HomeRun((int)1);
                            }

                            // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행
                            if (GlobalVar.AxisStatus[SV00_In_Shuttle].isHomed &&
                                 FuncInlineMove.IsArrived(SV00_In_Shuttle, 0))
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
                                FuncMotion.MoveStop(SV00_In_Shuttle); //정지상태 되면 서보 정지
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
                        FuncMotion.MoveStop(SV00_In_Shuttle); //정지상태 되면 서보 정지
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
                                       $"{Log} Remove the PCB, check for interference or sensor issues, and then restart.");
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
