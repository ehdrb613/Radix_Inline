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
    class BeforeWorkClass
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

        /** @brief 상승시 위치 */
        public double ready_pos = 0;
        /** @brief 버퍼 취출시 위치 */
        public double pickup_pos = 0;

        /** @brief 조립 최종 위치 */
        public double assemble_pos = 0;

        //클램프
        bool Y01_3_Before_Tray_WorkArea_Clamp_Forward = false;
        bool X01_4_Before_Tray_Clamp_Forward_Sensor = false;
        bool X01_5_Before_Tray_Clamp_Backward_Sensor = false;
        //스토퍼 상승
        bool Y01_2_Before_Tray_WorkArea_Stopper_Up = false;
        bool X01_2_Before_Tray_WorkArea_Stopper_Up_Sensor = false;
        bool X01_3_Before_Tray_WorkArea_Stopper_Down_Sensor = false;
        //컨베이어 동작
        bool Y01_1_Before_Tray_WorkArea_Conveyor_CW = false;

        //리프트 트레이 감지 센서
        bool X01_1_Before_Tray_WorkArea_Check_Sensor = false;

        // 플래그 변수
        public bool trayRequest = false;        // 트레이 요청
        public bool trayWorkReady = false;      // 트레이 작업준비완료
        public bool trayEjectReady = false;     // 트레이 배출준비

        /** @brief 타임아웃 체크할때 어디서 문제 생겼는지 내용 저장용 */
        //에러 내용 저장용, 타임
        public string Log = "";

        //중복로그 방지용 플레그
        private bool isLogWritten = false;

        //서보init 완료시 true 시작시 false
        private bool InitServo = false;
        #endregion

        /** @brief 생성자 */
        public BeforeWorkClass()
        {

            // 쓰레드를 시작한다
            actionThread = new Thread(ActionThread);
            actionThread.Start();
        }

        /** @brief 소멸자 */
        ~BeforeWorkClass()
        {
            ClassDisposing = true;
        }

        private void debug(string str)
        {
            Util.Debug("LowBushAssembleClass : " + str);
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
                                    Log = "[#2 샌딩 전 작업위치] CycleStop 지령";
                                    FuncLog.WriteLog(Log);
                                    Action = enumAction.CycleStop;

                                }
                                //동작해야 할때
                                if (!StepFinish)
                                {
                                    //공급 받아야할때
                                    if (!trayWorkReady &&
                                        !X01_1_Before_Tray_WorkArea_Check_Sensor &&
                                        ((AutoInline_Class)GlobalVar.ProjectClass).beforeLift01.Action == BeforeLiftClass.enumAction.InPutCnyRun)  //투입 단계일때
                                    {
                                        Log = "[#2 샌딩 전 작업위치] 트레이 공급 동작 지령";
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
                            // Main Control Thread 에서 초기화 지령 들어오면 초기화 수행
                            trayRequest = true; //초기화 하면 공급요청해야함

                            if (!X01_1_Before_Tray_WorkArea_Check_Sensor &&     // 트레이 없어야하고 스토퍼Down센서 ON, 스토퍼출력 OFF, 클램프 OFF, 클램프센서 BackWard On, 컨베이어 동작안할때 초기화 완료
                                X01_3_Before_Tray_WorkArea_Stopper_Down_Sensor &&
                                !Y01_2_Before_Tray_WorkArea_Stopper_Up &&
                                X01_5_Before_Tray_Clamp_Backward_Sensor &&
                                !Y01_3_Before_Tray_WorkArea_Clamp_Forward &&
                                !Y01_1_Before_Tray_WorkArea_Conveyor_CW)
                            {

                                Log = "[#2 샌딩 전 작업위치] 초기화 완료";
                                FuncLog.WriteLog(Log);
                                Action = enumAction.InitFinish;
                            }
                            else
                            {
                               
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
                                                                       ""));
                        
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

    }
}
