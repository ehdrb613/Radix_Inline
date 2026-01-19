using Radix.Library.LestExplorer;
using System;
using System.Threading;



namespace Radix
{
    /* 2025.08.10 고동현 추가
     * Read StepMotor Status Value
     * LetsExplororer1220 참조
     **/
    public struct LetsStatusUI
    {
        public bool ServoOn;
        public bool Moving;
        public bool Org;
        public double Position;
    }

    class LetsStatusThread
    {
        public static volatile int SelectedRawHandle = 0;
        public static volatile byte SelectedNodeType = 0;
        public static event Action<LetsStatusUI> SnapshotUpdated;
        public static LetsStatusUI LastSnapshot;
        public static volatile int ExtraSleepMs = 0; //2축홈 시 충돌때문에 Sleep시간을 늘리기 위한 변수임

        private void debug(string str) // 클래스 내부에서 Call 하는 로컬 Debug
        {
            Util.Debug(str);
        }


        public void Run()
        {
            int nStatusSleep = GlobalVar.ThreadSleep * 10;
            ulong chkTime = GlobalVar.TickCount64; // 상태 쓰레드 수행 시간 체크용

            while (GlobalVar.GlobalStop == false) // 프로그램 종료 전까지
            {
                try
                {
                    if (FuncLetsMotion.initialized && FuncLetsMotion.Scan_initialized )//연결완료 되었으면 진행
                    {
                        for (int axis = 0; axis < GlobalVar.LetsAxis_count; axis++) // 전체 스텝 모터 갯수대로
                        {
                            if (!GlobalVar.LetsHoming)
                            {
                                //SelectedNodeType = LetsExplorerDll.GetNodeType(axis);
                                //byte nodeType = SelectedNodeType;
                                //LetsStatusThread.SelectedRawHandle = raw;
                                //LetsStatusThread.SelectedNodeType = LetsExplorerDll.GetNodeType(axis);
                                int mask = 0;
                                int mAll = 0;
                                int mServo = 0;
                                int mMove = 0;
                                int mOrg = 0;
                                double position = 0;
                                // 상태 마스크
                                mAll = LetsExplorerDll.GetAxisState_All();
                                mServo = LetsExplorerDll.GetAxisState_SERVO();
                                mMove = LetsExplorerDll.GetAxisState_MOVING();
                                mOrg = LetsExplorerDll.GetAxisState_ORG();

                                mask = LetsExplorerDll.GetState(GlobalVar.LetsAxis[axis], mAll, LetsExplorerDll.GetNodeType(GlobalVar.LetsAxis[axis]));


                                #region 서보 온 확인
                                GlobalVar.LetsAxisStatus[axis].PowerOn = (mask & mServo) != 0;  // 온 여부
                                #endregion

                                #region 정지상태 확인
                                GlobalVar.LetsAxisStatus[axis].StandStill = (mask & mMove) == 0;  // 정지상태 여부

                                #endregion
                                #region Position
                                position = LetsExplorerDll.GetPosition(GlobalVar.LetsAxis[axis], 0);

                                GlobalVar.LetsAxisStatus[axis].Position = FuncLetsMotion.GetRealPos((FuncInline.enumLetsAxis)axis, position) - FuncInline.OffsetWidth[(int)axis];  //위치값 옵셋값 적용표시

                                #endregion
                                #region Home 상태 확인
                                //GlobalVar.LetsAxisStatus[axis].isHomed = (mask & mOrg) != 0 && GlobalVar.LetsAxisStatus[axis].StandStill;  // 홈센서 감지 여부
                                                                                                                                           //GlobalVar.LetsAxisStatus[axis].Homing = !GlobalVar.LetsAxisStatus[axis].isHomed;  // 호밍 여부
                                #endregion
                            }



                     

                        //호밍 중이면 시작
                        if (GlobalVar.LetsAxisStatus[axis].Homing && GlobalVar.LetsAxisStatus[axis].HomedTime == 0)
                            {
                                GlobalVar.LetsAxisStatus[axis].isHomed = false;
                                GlobalVar.LetsAxisStatus[axis].HomedTime = GlobalVar.TickCount64;
                            }

                            // 홈완료 1초 지나서 홈 펄스값 초기화 설정
                            if (GlobalVar.LetsAxisStatus[axis].isHomed &&
                                //GlobalVar.LetsAxisStatus[axis].Homing &&
                                GlobalVar.LetsAxisStatus[axis].HomedTime != 0)
                            {
                                if (GlobalVar.LetsAxisStatus[axis].HomedTime + 1000 < GlobalVar.TickCount64)
                                {
                                    LetsExplorerDll.SetZero(GlobalVar.LetsAxis[axis], 0);
                                    GlobalVar.LetsAxisStatus[axis].Homing = false;
                                    GlobalVar.LetsAxisStatus[axis].HomedTime = 0;
                                }
                            }

                        }
                        //LastSnapshot = snap;
                        //SnapshotUpdated?.Invoke(snap); // UI로 브로드캐스트
             
                        
                        
                        


                        //모든축 홈완료 되었으면 쓰래드 주기 100 + 0으로 변경
                        if ((GlobalVar.LetsAxisStatus[0].StandStill && FuncInline.InitialDone[(int)FuncInline.enumInitialize.InShuttle] == true) &&
                            (GlobalVar.LetsAxisStatus[1].StandStill && FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutShuttle] == true) &&
                            (GlobalVar.LetsAxisStatus[2].StandStill && FuncInline.InitialDone[(int)FuncInline.enumInitialize.OutConveyor] == true) &&
                            (GlobalVar.LetsAxisStatus[3].StandStill && FuncInline.InitialDone[(int)FuncInline.enumInitialize.InConveyor] == true) &&
                            (FuncInline.InlineType >= FuncInline.enumInlineType.Gen5 ?
                            (GlobalVar.LetsAxisStatus[4].StandStill && FuncInline.InitialDone[(int)FuncInline.enumInitialize.NgBuffer] == true) :
                            true))
                        {
                            LetsStatusThread.ExtraSleepMs = 0;
                            GlobalVar.LetsHoming = false;
                        }

                    }

                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                }

                Thread.Sleep(nStatusSleep + ExtraSleepMs);
                //Util.Debug("status check time : " + (GlobalVar.TickCount64 - chkTime).ToString());
                chkTime = GlobalVar.TickCount64;
                //Application.DoEvents();                
            }

            Environment.Exit(Environment.ExitCode);//프로그램 남는거 때문에
        }

        public static long UpdateStatus() //상태변화 있을 시 호출하면될듯 
        {
            return 1;
        }


    }
}