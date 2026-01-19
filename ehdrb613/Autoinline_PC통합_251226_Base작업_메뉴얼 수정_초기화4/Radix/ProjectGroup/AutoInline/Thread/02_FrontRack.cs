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

        public int SV02_Lift1 = (int)FuncInline.enumServoAxis.SV02_Lift1;
        public int SV03_Rack1_Width = (int)FuncInline.enumServoAxis.SV03_Rack1_Width;


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

        public static bool FLift_UpPCB_IN_Sensor = false;   //Lift Up PCB 진입센서
        public static bool FLift_UpPCB_Stop_Sensor = false;   //Lift Up PCB 정지센서
        public static bool FLift_DownPCB_IN_Sensor = false;   //Lift Up PCB 진입센서
        public static bool FLift_DownPCB_Stop_Sensor = false;   //Lift Up PCB 정지센서

        public static bool Front_PassLine_PCB_Sensor = false;  //Front Rack 인터락 센서

        public static bool Front_Interlock_Sensor = false;  //Front Rack 인터락 센서
        #endregion
       
        public string Name = "";
        #endregion

        /** @brief 타임아웃 체크할때 어디서 문제 생겼는지 내용 저장용 */
        //에러 내용 저장용, 타임
        public string Log = "";
        FuncInline.enumErrorPart ErrorPart = FuncInline.enumErrorPart.No_Error;

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

            Name = "[Front Rack]"; // 영문 로그 이름 설정
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
                                    if (((AutoInline)GlobalVar.Class).FrontRack.Action == FrontRack.enumAction.Waiting)  //투입 단계일때
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
                                enumDONames doPcbName;
                                if (SiteIoMaps.TryGetContactUpDownDO(currentSite, out doPcbName))
                                {
                                    //
                                    //if (Front_PCB_Sensor[index] == false)
                                    //{
                                    //나중엔 Notuse인건 무시하는거 생각해봐야할거같다
                                    //솔 무조건 업
                                        DIO.WriteDOData(doPcbName, false);
                                    //}
                                }

                                if (SiteIoMaps.TryGetContactStopperDO(currentSite, out doPcbName))
                                {
                                    //무조건 클램프 해제
                                    DIO.WriteDOData(doPcbName, false);
                                }


                            }
                            //Lift 호밍
                            if ((GlobalVar.AxisStatus[SV02_Lift1].isHomed &&
                                 FuncInlineMove.IsArrived(SV02_Lift1, 0)) &&
                                 (GlobalVar.AxisStatus[SV03_Rack1_Width].isHomed &&
                                 FuncInlineMove.IsArrived(SV03_Rack1_Width, FuncInline.DefaultPCBWidth)))
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
                                                                       ErrorPart,
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
    }
}



