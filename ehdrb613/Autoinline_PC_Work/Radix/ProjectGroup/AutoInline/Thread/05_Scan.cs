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
    class Scan
    {
        #region type 선언
        #region enum
        /**
         * @brief 동작 구분
         */
        public enum enumAction
        {
            Waiting, // 아무 동작 없을 때, 정지위치로
            Init, // 초기화
            InitFinish, // 초기화 완료
            Skip,       // 제품없을때 스킵
            NotUse,     // 사용 안할때
            CycleStop,  // 사이클스탑일때

            HomeMove, //에러 발생 후 복귀 동작 후 -> Waiting으로
            MoveFrontArray,
            MoveRearArray,
            Scan,
            ScanCheck
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
        public int SV07_Scan_X = (int)FuncInline.enumServoAxis.SV07_Scan_X;
        public int SV06_Scan_Y = (int)FuncInline.enumServoAxis.SV06_Scan_Y;

        public string Name = "";

        /** @brief 상승시 위치 */
        public double ready_pos = 0;
        /** @brief 버퍼 취출시 위치 */
        public double pickup_pos = 0;

        /** @brief 조립 최종 위치 */
        public double assemble_pos = 0;

        

        /** @brief 타임아웃 체크할때 어디서 문제 생겼는지 내용 저장용 */
        //에러 내용 저장용, 타임
        public string Log = "";

        //중복로그 방지용 플레그
        private bool isLogWritten = false;

        //서보init 완료시 true 시작시 false
        private bool InitServo = false;
        #endregion

        /** @brief 생성자 */
        public Scan()
        {
            // 쓰레드를 시작한다
            actionThread = new Thread(ActionThread);
            actionThread.Start();

            Name = $"[Scan]";

        }

        /** @brief 소멸자 */
        ~Scan()
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
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.Scan] = true;
                            // (1) Y축
                           

                            FuncInline.InitialDone[(int)enumInitialize.Scan] = FuncInlineAction.CheckOriginDone(enumInitialize.Scan);
                            FuncInline.InitialStarted[(int)FuncInline.enumInitialize.Scan] = !FuncInline.InitialDone[(int)enumInitialize.Scan];

                            if (FuncInline.InitialDone[(int)enumInitialize.Scan])
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

                                InitServo = false;
                            }
                            else
                            {
                                if (GlobalVar.AxisStatus[SV06_Scan_Y].StandStill &&
                                    !GlobalVar.AxisStatus[SV06_Scan_Y].isHomed &&
                                 !GlobalVar.AxisStatus[SV06_Scan_Y].Homing)
                                {
                                    FuncLog.WriteLog($"{Name} Y-Axis Homing Start");
                                    FuncMotion.MoveHome((uint)SV06_Scan_Y);
                                }
                                // (2) X축
                                if (GlobalVar.AxisStatus[SV07_Scan_X].StandStill &&
                                    !GlobalVar.AxisStatus[SV07_Scan_X].isHomed &&
                                    !GlobalVar.AxisStatus[SV07_Scan_X].Homing)
                                {
                                    FuncLog.WriteLog($"{Name} X-Axis Homing Start");
                                    FuncMotion.MoveHome((uint)SV07_Scan_X);
                                }
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
                        // 1. 상태 변경 감지 및 타이머 리셋
                        if (Action != beforeAction)
                        {
                            watch.Restart();
                            beforeAction = Action;
                        }

                        // 2. 타임아웃 체크 (Loading/UnLoading 관련 상태일 때만)
                        bool isCheckState = ((Action >= enumAction.MoveFrontArray && Action <= enumAction.MoveRearArray));

                        if (isCheckState && watch.ElapsedMilliseconds > ActionTimeout)
                        {
                            watch.Stop(); // 타임아웃 발생 시 타이머 정지
                            
                                FuncInline.AddError(FuncInline.enumErrorPart.System, FuncInline.enumErrorCode.MoveFail,
                                    $"{Log}{Action.ToString()} Servo Move Timeout.");
                           
                            // 초기화 및 대기
                            Action = enumAction.Waiting;
                            continue; // switch문 실행 안 하고 다음 루프로
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
