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
        public int SV04_Lift2 = (int)FuncInline.enumServoAxis.SV04_Lift2;
        public int SV05_Rack2_Width = (int)FuncInline.enumServoAxis.SV05_Rack2_Width;


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

        public static bool RLift_UpPCB_IN_Sensor = false;      // 리프트 UP 위치 PCB 진입
        public static bool RLift_UpPCB_Stop_Sensor = false;    // 리프트 UP 위치 PCB 정지

        public static bool RLift_DownPCB_IN_Sensor = false;    // 리프트 DOWN 위치 PCB 진입
        public static bool RLift_DownPCB_Stop_Sensor = false;  // 리프트 DOWN 위치 PCB 정지

        // Passline (OK/NG Line) 관련 센서
        public static bool ROKLine_Stopper = false;   //Rear OK PassLine 스토퍼
        public static bool RNGLine_Stopper = false;   //Rear NG PassLine 스토퍼
        public static bool Rear_Pass_OkLine_PCB_In_Sensor = false;
        public static bool Rear_Pass_NgLine_PCB_Stop_Sensor = false;

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

                    UpdateRearAllStatus();
                    UpdateRearETCStatus();

                    #endregion

                    #region 시스템 상태 따라
                    switch (Action)
                    {
                        case enumAction.Waiting:
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
                                    Action = enumAction.CycleStop;

                                }
                                //동작해야 할때
                                if (!StepFinish)
                                {
                                    //공급 받아야할때
                                    if (!trayWorkReady &&
                                        ((AutoInline)GlobalVar.Class).InShuttle.Action == InShuttle.enumAction.InPutCnyRun)  //투입 단계일때
                                    {
                                        Log = $"{Name} 트레이 공급 동작 지령";
                                        FuncLog.WriteLog(Log);
                                        Action = enumAction.InputTray;
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

                                // 클램프 해제
                                if (SiteIoMaps.TryGetContactStopperDO(currentSite, out FuncInline.enumDONames doClamp))
                                {
                                    DIO.WriteDOData(doClamp, false);
                                }
                                // 포고핀 상승 (Down Sol OFF)
                                if (SiteIoMaps.TryGetContactUpDownDO(currentSite, out FuncInline.enumDONames doDown))
                                {
                                    DIO.WriteDOData(doDown, false);
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

                            // 4. 완료 확인
                            // 두 축 모두 호밍 완료 && 지정 위치 도달 확인
                            if ((GlobalVar.AxisStatus[SV04_Lift2].isHomed &&
                                 FuncInlineMove.IsArrived(SV04_Lift2, 0)) &&
                                (GlobalVar.AxisStatus[SV05_Rack2_Width].isHomed &&
                                 FuncInlineMove.IsArrived(SV05_Rack2_Width, FuncInline.DefaultPCBWidth)))
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
                                                                       $"{Name}"));
                            //Action = enumAction.HomeMove;   //에러 발생시 준비위치로 이동 후 Wait
                            Util.InitWatch(ref watch);
                            continue;
                        }
                        switch (Action)
                        {

                            case enumAction.HomeMove: //에러 발생 후 복귀 동작 후 -> Waiting으로
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

    }
}
