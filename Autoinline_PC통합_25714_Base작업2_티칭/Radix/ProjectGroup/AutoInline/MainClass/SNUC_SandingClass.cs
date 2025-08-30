using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Radix
{
    /**
     * @brief 프로젝트 메인 클래스
     *        폼을 제외한 프로젝트의 모든 선언과 함수, 클래스, 쓰레드 등을 선언
     */

    class SNUC_SandingClass
    {
        #region 변수 선언부

        #region 주 Control Thread 처리용
        /** @brief 동작 처리 쓰레드. */
        public Thread actionThread { get; set; }
        /** @brief 동작 처리 쓰레드. */
        public Thread systemCheckThread { get; set; }
        /** @brief 클래스 만료중 체크 */
        public bool ClassDisposing = false;
        #endregion

        #region 부위별 하부 클래스
        //public PanasonicVision sizeVision = new PanasonicVision("192.168.1.5", 8604, 8601); // 혼입감지 비전
        //public PanasonicVision printVision = new PanasonicVision("192.168.1.5", 8604, 8601); // 투입 및 인쇄 검사 비전


        public BeforeLiftClass beforeLift01 = new BeforeLiftClass(); //#1 샌딩전 트레이 투입 리프트
        public BeforeWorkClass beforeWork02 = new BeforeWorkClass(); //#2 샌딩전 트레이 작업위치
       

        #endregion

        #region 일반 변수
        public ulong runTime = GlobalVar.TickCount64; // 공정시간 적산용
        public ulong runTotal = 0; // 공정 카운트 적산용     
        public bool DoorLock_Check = false;
        #endregion

        #endregion

        #region 클래스 초기화 관련
        /**
         * @brief 클래스 생성자
         *        필요한 모든 변수 및 클래스, 쓰레드 등을 초기화한다.
         */
        public SNUC_SandingClass()
        {
            GlobalVar.ProjectClass = this;

            InitGlobals(); // GlobalVar에 선언된 전역변수 세팅

            // 하부 클래스 초기화
            actionThread = new Thread(ActionThread);
            actionThread.Start();

            // 내부 쓰레드 초기화
            systemCheckThread = new Thread(SystemCheckThread);
            systemCheckThread.Start();


            #region 변수 초기화


            

            #endregion

        }

        /**
         * @brief 소멸자
         */
        ~SNUC_SandingClass()
        {
            ClassDisposing = true;
        }

        /**
         * @brief 프로젝트별로 type 다르게 쓰는 것에 따라 변수를 유동적으로 사용하기 위해 글로벌 변수를 초기화한다.
         */
        public void InitGlobals()
        {
            GlobalVar.ProjectType = enumProject.Sanding; // 프로젝트 구분
            GlobalVar.MasterType = enumMasterType.AXL; // 이더켓 마스터 종류


            #region 각종 기능 사용 여부
            GlobalVar.UseNormalError = true; // 그냥 Error 사용. false일 경우 확장에러시스템 사용
            GlobalVar.UsePartClear = false; // 파트클리어 시스템 사용 여부, 사용시 에러창 클릭하면 파트클리어로, 미사용시 에러창으로
            GlobalVar.UseCycleStop = true; // Cycle Stop 기능 활성화 여부

            #endregion

            #region MsSql
            GlobalVar.UseMsSQL = true; //MSSql 사용 여부
            GlobalVar.MsSQL_Server = "127.0.0.1";//MSSql 연결할 서버 IP
            GlobalVar.MsSQL_Port = "1433"; //MSSql 연결할 서버 포트
            GlobalVar.MsSQL_Id = "sa"; //MSSql 계정
            GlobalVar.MsSQL_Pwd = "radix5243"; //MSSql 계정 비밀번호
            GlobalVar.MsSQL_DB = "AutoInline"; //MSSql DataBase명
            #endregion

            #region DeviceNet 이용한 로봇 제어 관련
           
            // 로봇 통신을 위해 지정 데이터 길이에 맞춰 통신 클래스를 초기화한다. 로봇 사용할 일 있을 때 다시 정리
            //GlobalVar.CifxClass = new CIFX(GlobalVar.RobotCount, GlobalVar.RobotDataLength);
            if (GlobalVar.RobotUse)
            {
                (new Thread((new cifxThread()).Run)).Start();//DeviceNet 쓰레드

            }
            // 로봇 관련 설정
            //GlobalVar.RobotUse = true; // 로봇 작업 전 임시
            //GlobalVar.RobotUse = true;
            //GlobalVar.RobotType = enumRobot.POSCO;
            #endregion



            #region 각 모듈의 크기를 지정한다. DIO 섞인 모듈은 별개로 하드코딩해야 할 듯.
            //GlobalVar.ModuleSize[0] = 32; // DI는 32점씩이다.
            //GlobalVar.ModuleSize[1] = 32; // DO는 32점씩이다.

            //GlobalVar.DiSize = new uint[Enum.GetValues(typeof(FuncAmplePacking.enumDINames)).Length / GlobalVar.ModuleSize[0]];
            //for (int i = 0; i < GlobalVar.DiSize.Length; i++)
            //{
            //    GlobalVar.DiSize[i] = GlobalVar.ModuleSize[0];
            //}
            //GlobalVar.DoSize = new uint[Enum.GetValues(typeof(FuncAmplePacking.enumDONames)).Length / GlobalVar.ModuleSize[1]];
            //for (int i = 0; i < GlobalVar.DoSize.Length; i++)
            //{
            //    GlobalVar.DoSize[i] = GlobalVar.ModuleSize[1];
            //}
            #endregion

            #region 각 모듈의 순서를 지정한다. 순차적으로 할당하고는 있지만 물리적으로 순서를 바꿀 필요가 있을 경우는 각 순서에 맞게 지정해야 한다.

            /** @brief 아진 엑스텍 인풋 모듈 갯수 */
            GlobalVar.Inputmodule = 5; //아진 엑스텍 인풋 모듈 갯수
                                       /** @brief 아진 엑스텍 아웃풋 모듈 갯수 */
            GlobalVar.Outputmodule = 5; //아진 엑스텍 아웃풋 모듈 갯수
                                        /** @brief 아진 엑스텍 인풋 시작 노드 ID */
            GlobalVar.InputStartNodeID = 0; //아진 엑스텍 인풋 시작 노드 ID
                                            /** @brief 아진 엑스텍 아웃풋 시작 노드 ID      */
            GlobalVar.OutputStartNodeID = GlobalVar.InputStartNodeID + GlobalVar.Inputmodule; //아진 엑스텍 아웃풋 시작 노드 ID     
            #endregion

            #region 서보모터 관련
            //GlobalVar.UseGantry = true; // 갤트리는 일단 보류
            //GlobalVar.Axis_num = (uint)GlobalVar.DiSize.Length + (uint)GlobalVar.DoSize.Length; // 모든 모듈 뒤에 서보 연결

            //GlobalVar.ServoGearRatio = new double[Enum.GetNames(typeof(FuncAmplePacking.enumServoAxis)).Length];
            //GlobalVar.Axis_count = (uint)Enum.GetValues(typeof(FuncAmplePacking.enumServoAxis)).Length; // 서보 모터 축수//해당 부분은 전체어떻게 가져갈지 생각해 보자
            //GlobalVar.Axis_count =1; // 서보 모터 축수//해당 부분은 전체어떻게 가져갈지 생각해 보자
            //DG 수정필요

            /** @brief 서보모터 기어비, 물리적으로 서보모터의 갯수만큼 배열을 지정한다. */


            GlobalVar.ServoRevPulse = new double[Enum.GetNames(typeof(FuncInline.enumServoAxis)).Length];
            for (int i = 0; i < GlobalVar.ServoRevPulse.Length; i++)
            {
                GlobalVar.ServoRevPulse[i] = 100000;
            }

            #endregion

            #region DIO 배열 관련 선언

            GlobalVar.DI_Array = new bool[Enum.GetValues(typeof(FuncInline.enumDINames)).Length];
            GlobalVar.DI_Before = new bool[Enum.GetValues(typeof(FuncInline.enumDINames)).Length];
            GlobalVar.DI_Change = new bool[Enum.GetValues(typeof(FuncInline.enumDINames)).Length];


            GlobalVar.DO_Array = new bool[Enum.GetValues(typeof(FuncInline.enumDONames)).Length];
            GlobalVar.DO_Read = new bool[Enum.GetValues(typeof(FuncInline.enumDONames)).Length];
            GlobalVar.DO_Before = new bool[Enum.GetValues(typeof(FuncInline.enumDONames)).Length];
            GlobalVar.DO_Change = new bool[Enum.GetValues(typeof(FuncInline.enumDONames)).Length];
            #endregion


        }

        /**
         * @brief 전역으로 기본 사용하는 쓰레드 구동
         */
        public void InitGlobalThread()
        {

        }

        /**
         * @brief 프로젝트 전용 쓰레드 구동.
         */
        public void InitLocalThread()
        {

        }

        /**
         * @brief 프로젝트 하부 클래스 구동
         */
        public void InitLocalClass()
        {

        }

        #endregion

        /**
         * @brief 시스템 상태 체크 쓰레드
         */
        private void SystemCheckThread()
        {
            while (!GlobalVar.GlobalStop &&
                !ClassDisposing)
            {
                try
                {

              
                    //debug("tmrCheck time 2 : " + (GlobalVar.TickCount64 - tick));
                    #region OP



                    DIO.WriteDOData((int)FuncInline.enumDONames.Y0_7_CNC_ON_Lamp, GlobalVar.SystemStatus >= enumSystemStatus.AutoRun && GlobalVar.SystemStatus != enumSystemStatus.ErrorStop);
                    DIO.WriteDOData((int)FuncInline.enumDONames.Y3_6_NgBuffer_ForwardSwitch_Lamp, GlobalVar.SystemStatus < enumSystemStatus.AutoRun || GlobalVar.SystemStatus == enumSystemStatus.ErrorStop);
                    DIO.WriteDOData((int)FuncInline.enumDONames.Y4_6_OP_Reset_Lamp, GlobalVar.SystemStatus == enumSystemStatus.ErrorStop ||
                                                                                            GlobalVar.Warning ||
                                                                                            GlobalVar.DoorOpen ||
                                                                                            GlobalVar.E_Stop);
                    //if (DIO.GetDIChange(DIO_BoxPacking_enumDINames.X00_2_OP_Reset) && DIO.GetDIData(DIO_BoxPacking_enumDINames.X00_2_OP_Reset) &&
                    //    (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop ||
                    //                                                                        GlobalVar.Warning ||
                    //                                                                        GlobalVar.DoorOpen ||
                    //                                                                        GlobalVar.E_Stop))
                    //{
                    //    GlobalVar.EnableBuzzer = false;
                    //}

                    if (DIO.GetDIChange(FuncInline.enumDINames.X01_1_OP_Start) && DIO.GetDIData(FuncInline.enumDINames.X01_1_OP_Start))
                    {
                        if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun ||
                            GlobalVar.SystemErrored ||
                            GlobalVar.E_Stop ||
                            GlobalVar.DoorOpen)
                        {

                        }
                        else if (GlobalVar.TabMain != enumTabMain.Auto)
                        {
                            FuncWin.TopMessageBox("Change to Auto Window first");

                        }
                        else
                        {
                            FuncLog.WriteLog("Main - OP Start Click ");
                            FuncInline.Start_Button(true);
                        }
                    }

                    if (DIO.GetDIChange(FuncInline.enumDINames.X01_3_OP_Stop) && DIO.GetDIData(FuncInline.enumDINames.X01_3_OP_Stop))
                    {
                        if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
                        {

                        }
                        else
                        {
                            FuncLog.WriteLog("Main - OP Stop Click ");
                            FuncInline.Stop_Button();
                        }
                    }

                    if (DIO.GetDIChange(FuncInline.enumDINames.X01_7_OP_Reset) && DIO.GetDIData(FuncInline.enumDINames.X01_7_OP_Reset))
                    {
                        //GlobalVar.SystemStatus > enumSystemStatus.AutoRun 변경, enum 순서 변경으로 인해 by DG 241119
                        if (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop || GlobalVar.SystemStatus == enumSystemStatus.EmgStop)
                        {
                            FuncLog.WriteLog("Main - OP Reset Click ");

                            FuncInline.Reset_Button();

                        }
                    }



                    #endregion
                    //debug("tmrCheck time 3: " + (GlobalVar.TickCount64 - tick));

                    //debug("tmrCheck time 4 : " + (GlobalVar.TickCount64 - tick));
                    #region Tower Lamp

                    #region Green
                    if (!GlobalVar.EnableTower)
                    {
                        DIO.Tower_Lamp_Green_Control(false);
                    }
                    else if (GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Green, (int)enumTowerLampAction.Enable] &&
                        GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Green, (int)enumTowerLampAction.Blink] &&
                        (GlobalVar.TickCount64 - GlobalVar.TowerTick) > 1000) // 점멸
                    {
                        //DIO.WriteDOData(enumDONames.Y02_2_TOWER_LAMP_GREEN, !DIO.GetDORead(enumDONames.Y02_2_TOWER_LAMP_GREEN));
                        DIO.Tower_Lamp_Green_Control(DIO.Tower_Lamp_Green_Check());
                    }
                    else
                    {
                        if (GlobalVar.TickCount64 - GlobalVar.TowerTick > 1000)
                        {
                            //DIO.WriteDOData(enumDONames.Y02_2_TOWER_LAMP_GREEN, GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, 2, 0]);
                            DIO.Tower_Lamp_Green_Control(GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Green, (int)enumTowerLampAction.Enable]);
                        }
                    }
                    #endregion
                    #region Yellow
                    if (!GlobalVar.EnableTower)
                    {
                        DIO.Tower_Lamp_Yellow_Control(false);
                    }
                    else if (GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Yellow, (int)enumTowerLampAction.Enable] &&
                        GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Yellow, (int)enumTowerLampAction.Blink] &&
                        GlobalVar.TickCount64 - GlobalVar.TowerTick > 1000) // 점멸
                    {
                        //DIO.WriteDOData(enumDONames.Y02_1_TOWER_LAMP_YELLOW, !DIO.GetDORead(enumDONames.Y02_1_TOWER_LAMP_YELLOW));
                        DIO.Tower_Lamp_Yellow_Control(DIO.Tower_Lamp_Yellow_Check());
                    }
                    else
                    {
                        if (GlobalVar.TickCount64 - GlobalVar.TowerTick > 1000)
                        {
                            //DIO.WriteDOData(enumDONames.Y02_1_TOWER_LAMP_YELLOW, GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, 1, 0]);
                            DIO.Tower_Lamp_Yellow_Control(GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Yellow, (int)enumTowerLampAction.Enable]);
                        }
                    }
                    #endregion
                    #region Red
                    if (!GlobalVar.EnableTower)
                    {
                        DIO.Tower_Lamp_Red_Control(false);
                    }
                    else if (GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Red, (int)enumTowerLampAction.Enable] &&
                        GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Red, (int)enumTowerLampAction.Blink] &&
                        GlobalVar.TickCount64 - GlobalVar.TowerTick > 1000) // 점멸
                    {
                        //DIO.WriteDOData(enumDONames.Y02_0_TOWER_LAMP_RED, !DIO.GetDORead(enumDONames.Y02_0_TOWER_LAMP_RED));
                        DIO.Tower_Lamp_Red_Control(DIO.Tower_Lamp_Red_Check());
                    }
                    else
                    {
                        if (GlobalVar.TickCount64 - GlobalVar.TowerTick > 1000)
                        {
                            //DIO.WriteDOData(enumDONames.Y02_0_TOWER_LAMP_RED, GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, 0, 0]);
                            DIO.Tower_Lamp_Red_Control(GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Red, (int)enumTowerLampAction.Enable]);
                        }
                    }
                    #endregion

                    #endregion
                    //debug("tmrCheck time 5 : " + (GlobalVar.TickCount64 - tick));
                    #region buzzer run/stop 등 운영상황에 관련된 것만 일괄 처리하고, 나머지 오퍼레이터 콜 등은 ErrorDialog에서 처리
                    //if (GlobalVar.E_Stop || GlobalVar.DoorOpen || Warning)


                    if (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop && GlobalVar.Warning)
                    {
                        // JHRYU : 경고창 상태에서 에러 발생시 경고를 끈다.
                        GlobalVar.Warning = false;
                    }
                    if (!GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Buzzer, (int)enumTowerLampAction.Enable])
                    {
                        GlobalVar.BuzzerTime = GlobalVar.TickCount64;   //buzzer 아닐때 시간 체크
                    }

                    if (!GlobalVar.EnableTower)
                    {
                        DIO.Tower_Lamp_Buzzer_Control(false);
                    }
                    // 경고창시 에러 알람 안나와서 일단 주석 처리함
                    else if (GlobalVar.Warning)
                    {
                        DIO.Tower_Lamp_Buzzer_Control(GlobalVar.EnableBuzzer &&
                            GlobalVar.TickCount64 - GlobalVar.WarningTime < 2000);   //202209 부져 상황별 정리 필요 by DG
                        DIO.Tower_Lamp_Yellow_Control(DIO.Tower_Lamp_Yellow_Check());
                    }
                    else
                    {
                        if (!GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Buzzer, (int)enumTowerLampAction.Enable])
                        {
                            GlobalVar.BuzzerTime = GlobalVar.TickCount64;   //buzzer 아닐때 시간 체크
                        }
                        DIO.Tower_Lamp_Buzzer_Control(GlobalVar.EnableBuzzer &&
                                                    GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Buzzer, (int)enumTowerLampAction.Enable] &&
                                                    (GlobalVar.TowerTime[(int)GlobalVar.SystemStatus] == 0 ||
                                                            GlobalVar.TickCount64 - GlobalVar.BuzzerTime < GlobalVar.TowerTime[(int)GlobalVar.SystemStatus] * 1000));
                    }
                    if (!GlobalVar.Warning)
                    {
                        GlobalVar.WarningTime = GlobalVar.TickCount64;   //Warning 아닐때 시간 체크
                    }



                    #endregion
                    //debug("tmrCheck time 6: " + (GlobalVar.TickCount64 - tick));
                    #region Door Check
                    //*

                    if (GlobalVar.UseDoor)
                    {

                        //if (DoorLock_Check)
                        //{
                        DoorLock_Check = false;
                        if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                            GlobalVar.SystemStatus != enumSystemStatus.ErrorStop &&
                             (DIO.Door_Check1()))
                        // DIO.Door_Check3()
                        {
                            GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                  DateTime.Now.ToString("HH:mm:ss"),
                                                  enumErrorPart.System,
                                                  enumErrorCode.Door_Opened,
                                                  false,
                                                    $"{ FuncInline.OpenDoorInfo1 }\n{ FuncInline.OpenDoorInfo2}\n{FuncInline.OpenDoorInfo3}"));


                        }
                        //}


                        // 도어락 ON
                        //if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                        //    GlobalVar.SystemStatus != enumSystemStatus.ErrorStop)
                        //{
                        //    OpenRobotDoor(false); //도어락
                        //    DoorLock_Check = true;

                        //}

                        //if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                        //GlobalVar.SystemStatus != enumSystemStatus.ErrorStop &&
                        //(DIO.Door_Check2()))
                        //{
                        //    GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                        //    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                        //                          DateTime.Now.ToString("HH:mm:ss"),
                        //                          enumErrorPart.System,
                        //                          enumErrorCode.Door_Opened,
                        //                          false,
                        //                          "Door area2 opened while system is running. Close door and try again."));


                        //}

                        //if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                        //GlobalVar.SystemStatus != enumSystemStatus.ErrorStop &&
                        //(DIO.Door_Check3()))
                        //{
                        //    GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                        //    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                        //                          DateTime.Now.ToString("HH:mm:ss"),
                        //                          enumErrorPart.System,
                        //                          enumErrorCode.Door_Opened,
                        //                          false,
                        //                          "Door area3 opened while system is running. Close door and try again."));


                        //}

                        /* 사용 미확정
                        if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                        GlobalVar.SystemStatus != enumSystemStatus.ErrorStop &&
                        (DIO.Door_Check4()))
                        {
                            GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                            FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                  DateTime.Now.ToString("HH:mm:ss"),
                                                  enumErrorPart.System,
                                                  enumErrorCode.Door_Opened,
                                                  false,
                                                  "Door area4(FEEDER) opened while system is running. Close door and try again."));
                        }
                        */
                    }

                    //*/
                    #endregion
                    //debug("tmrCheck time 7 : " + (GlobalVar.TickCount64 - tick));
                    #region 서보상태
                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                    {
                        /*
                        if (GlobalVar.AxisStatus[0].Disabled ||
                            !GlobalVar.AxisStatus[0].PowerOn)
                        {
                            Func.AddError(enumError.Work_X_Disabled);
                        }
                        if (GlobalVar.AxisStatus[0].LimitSwitchPos ||
                            GlobalVar.AxisStatus[0].LimitSwitchNeg)
                        {
                            Func.AddError(enumError.Work_X_Limit);
                        }
                        //*/
                    }

                    //호밍 중일때 센서 감지시 즉시 정지
                    //int SV00_BeforeLift = (int)FuncAutoInline.enumServoAxis.SV00_Input_Tray_Lift;
                    //int SV01_AfterLift = (int)FuncAutoInline.enumServoAxis.SV01_Output_Tray_Lift;
                    //if (GlobalVar.AxisStatus[SV00_BeforeLift].Homing)
                    //{
                    //    if (DIO.GetDIData(FuncAutoInline.enumDINames.X01_0_Before_Tray_Input_Lift_End_Sensor))
                    //    {
                    //        FuncMotion.MoveStop(SV00_BeforeLift); //정지상태 되면 서보 정지
                    //        //정지및 알람

                    //        FuncAutoInline.AddError(FuncAutoInline.enumErrorPart.BeforeLift,
                    //           FuncAutoInline.enumErrorCode.Robot_InterLock,
                    //           "샌딩 전 리프트 초기화 중 트레이 센서가 감지(X01_0)되었습니다..\n" +
                    //                  $"센서에 감지되는 부분을 조치하고 다시 초기화해주세요.");

                    //    }
                    //}

                    //if (GlobalVar.AxisStatus[SV01_AfterLift].Homing)
                    //{
                    //    if (DIO.GetDIData(FuncAutoInline.enumDINames.X03_1_After_Tray_Input_Lift_Start_Sensor) ||
                    //        DIO.GetDIData(FuncAutoInline.enumDINames.X03_2_After_Tray_Input_Lift_End_Sensor))
                    //    {
                    //        FuncMotion.MoveStop(SV01_AfterLift); //정지상태 되면 서보 정지
                    //                                             //정지및 알람
                    //        FuncAutoInline.AddError(FuncAutoInline.enumErrorPart.BeforeLift,
                    //            FuncAutoInline.enumErrorCode.Robot_InterLock,
                    //            "샌딩 후 리프트 초기화 중 트레이 센서가 감지(X03_1,X03_2)되었습니다..\n" +
                    //            $"센서에 감지되는 부분을 조치하고 다시 초기화해주세요.");
                    //    }
                    //}
                    #endregion
                    //debug("tmrCheck time 8 : " + (GlobalVar.TickCount64 - tick));
                    #region 조명 컨트롤 B접점
                    //DIO.WriteDOData(enumDONames.Y00_4_LED_Lamp1, GlobalVar.SystemStatus >= enumSystemStatus.AutoRun);
                    #endregion
                    //debug("tmrCheck time 9 : " + (GlobalVar.TickCount64 - tick));
                    #region 상태표시
                    #endregion
                    //debug("tmrCheck time 10 : " + (GlobalVar.TickCount64 - tick));
                    #region Timeout
                    //if (GlobalVar.SystemStatus == enumSystemStatus.AutoRun)
                    //{

                    //    if (GlobalVar.TickCount64 - GlobalVar.OutputTime > GlobalVar.OutStopTime * 60 * 1000)
                    //    {
                    //        GlobalVar.SystemStatus = enumSystemStatus.OutputStop;
                    //    }
                    //    else if (GlobalVar.TickCount64 - GlobalVar.InputTime > GlobalVar.InStopTime * 60 * 1000)
                    //    {
                    //        GlobalVar.SystemStatus = enumSystemStatus.InputStop;
                    //    }
                    //    //*
                    //    else if (GlobalVar.SystemStatus != enumSystemStatus.ErrorStop)
                    //    {
                    //        GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
                    //        //GlobalVar.OutputTime = GlobalVar.TickCount64;
                    //        //GlobalVar.InputTime = GlobalVar.TickCount64;
                    //    }
                    //    //*/
                    //    /*
                    //  else if (GlobalVar.SystemStatus != enumSystemStatus.InputStop &&
                    //      GlobalVar.SystemStatus != enumSystemStatus.OutputStop)
                    //  {
                    //      //GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
                    //      GlobalVar.OutputTime = GlobalVar.TickCount64;
                    //      GlobalVar.InputTime = GlobalVar.TickCount64;
                    //  }
                    //  //*/

                    //}
                    #endregion
                    //debug("tmrCheck time 11 : " + (GlobalVar.TickCount64 - tick));



                    ////////////////////
                    if (GlobalVar.TickCount64 - GlobalVar.TowerTick > 1000)
                    {
                        GlobalVar.TowerTick = GlobalVar.TickCount64;
                    }
                    //debug("tmrCheck time 12 : " + (GlobalVar.TickCount64 - tick));
                    //tick = GlobalVar.TickCount64;
                }
                catch (Exception ex)
                {
                    FuncLog.WriteLog("SNUC_AmplePacking.SystemCheckThread : " + ex.ToString());
                    FuncLog.WriteLog("SNUC_AmplePacking.SystemCheckThread : " + ex.StackTrace);
                }

                Thread.Sleep(GlobalVar.ThreadSleep);
            }
        }
        /** 
         * @brief 동작 처리 쓰레드 
         *          총괄해서 체크 및 하부 부위별 동작 지령을 컨트롤한다.
         */
        private void ActionThread()
        {
            while (!GlobalVar.GlobalStop &&
                !ClassDisposing)
            {
                try
                {
                    #region 상시 체크할 부분

                    //if(GlobalVar.SystemStatus <= enumSystemStatus.ErrorStop && FuncAmplePacking.ampule[0] != null)
                    //{

                    //    for (int i = FuncAmplePacking.ampule.Length - 1; i > 0; i--)
                    //    {                        
                    //        if (i != 9 || i != 11)
                    //        {
                    //            FuncAmplePacking.ampule[i].StopTimer();
                    //        }
                    //    }
                    //}
                    #endregion

                    #region 시스템 상태에 따라서 동작할 부분
                    switch (GlobalVar.SystemStatus)
                    {
                        case enumSystemStatus.BeforeInitialize:
                            #region 시스템 초기화
                            #region Ecat Master 등의 초기화 및 동작여부 체크

                            //Controller.MasterChecked = false;
                            Controller.MasterChecking = true;

                            if (GlobalVar.Simulation)
                            {
                                Controller.Status = 0;
                                Controller.MasterChecked = true;
                                Controller.MasterChecking = false;
                            }
                            //else
                            //{
                            //    if (GlobalVar.MasterType == enumMasterType.AXL)
                            //    {
                            //        //아진은 필요 없을 것 같다.
                            //    }
                            //    Controller.MasterChecked = false;
                            //    Controller.MasterChecking = true;
                            //}
                            Controller.startTime = GlobalVar.TickCount64;

                            // 체크 완료까지 대기
                            if (GlobalVar.MasterType == enumMasterType.AXL && Controller.MasterChecked == false)
                            {
                                Controller.MasterChecked = Controller.CheckAXL();
                                Controller.MasterChecking = false;

                                // 시뮬레이션용 서보 데이타 초기화
                                Controller.Init();
                            }

                            if (!Controller.MasterChecked)
                            {
                                try
                                {
                                    //dlgLoading.Close();
                                }
                                catch { }
                                FuncLog.WriteLog("Kernel Init Failed!");
                                //this.BringToFront();
                                FuncWin.TopMessageBox("Kernel Init Failed!");
                                Controller.initFail = true;
                                GlobalVar.GlobalStop = true;
                                //this.Close();
                                return;
                            }



                            #endregion
                            #endregion
                            if (!DoorLock_Check) OpenRobotDoor(true);
                            break;
                        case enumSystemStatus.Initialize:
                            #region 부위별 초기화 완료면 메뉴얼로
                            if (Init_Check())       //전체 Sub클래스 InitFinish 확인
                            {
                                GlobalVar.SystemStatus = enumSystemStatus.Manual;
                            }
                            #endregion
                            #region 초기화 진행 안 된 부위 있으면 초기화 지령
                            Init_Action();    //초기화진행 지령 안된 서브 클래스 지령
                            #endregion
                            if (DoorLock_Check) OpenRobotDoor(false);
                            break;
                        case enumSystemStatus.Manual:
                            if (!DoorLock_Check) OpenRobotDoor(true);
                            break;
                        case enumSystemStatus.EmgStop:
                            // 세부 컨트롤은 하부 클래스에서 각자 하면 된다.
                            break;
                        case enumSystemStatus.ErrorStop:
                            // 세부 컨트롤은 하부 클래스에서 각자 하면 된다.
                            break;
                        case enumSystemStatus.AutoRun:
                        //break; // 세 종류는 동작이 같으나 동작조건이나 이후 처리가 다르므로 통합한다.
                        case enumSystemStatus.CycleStop:
                        //break; // 세 종류는 동작이 같으나 동작조건이나 이후 처리가 다르므로 통합한다.
                        case enumSystemStatus.InOutStop: // InOutStop은 인라인 장비가 아니라 해당사항 없다.

                            #region 자동일때 변경되어야 할 DIO

                            if (DoorLock_Check) OpenRobotDoor(false); //도어락

                            #endregion


                            //**************************************************************************************************
                            //
                            //**************************************************************************************************
                            #region 사이클스탑 
                            if (false)
                            {
                                Thread.Sleep(200);  //기본적으로 일단 반박자 쉬고 가자

                                //텍타임 확인용 타이머 정지
                                if (GlobalVar.RunTimeTact.IsRunning)
                                {
                                    GlobalVar.RunTimeTact.Stop();
                                    FuncLog.WriteValueLog($"TactTime:{(int)GlobalVar.RunTimeTact.Elapsed.TotalSeconds}S");
                                }

                                //컨베이어 동작전 컨베이어 및 양품 배출쪽에 제품이 있는지 확인하고 없으면 정지
                                if (FuncInline.Check_CycleStop() &&   //&& 제품 체크 조건을 먼저 보도록 우선순위 변경 by DG 250212
                                    GlobalVar.SystemStatus == enumSystemStatus.CycleStop)
                                {
                                    GlobalVar.CycleStop_End = true;
                                    FuncLog.WriteLog("사이클스탑 상태 - 컨베이어 제품 없음 자동 정지 지령");
                                    Thread.Sleep(1000);
                                    // CycleStop 경우 컨베이어, 양품배출 쪽에 제품이 없을경우 정지시켜야됨
                                    GlobalVar.SystemStatus = enumSystemStatus.Manual;
                                }
                                else
                                {
                                    if (GlobalVar.StepStop)     // 1STEP 상태인가? 그러면 완료후 정지
                                    {
                                        Thread.Sleep(2500);
                                        FuncInline.Stop_Button();
                                        FuncLog.WriteLog("1StepStop End");

                                    }

                                    //컨베이어에 제품이 하나도 없을때
                                    if (FuncInline.Conveyor_Ampule_check &&
                                        GlobalVar.SystemStatus == enumSystemStatus.CycleStop)
                                    {
                                        FuncLog.WriteLog("사이클스탑 상태 - 컨베이어 제품 없음 대기");
                                        Thread.Sleep(1000);
                                        if (FuncInline.Print_OutAmpule_check)
                                        {
                                            FuncInline.send_WarningMsg("사이클 스탑모드 - 모든앰플 배출완료 \n 현재 작업중인 트레이가 있습니다. 정지하겠습니까?", 998);
                                            // CycleStop 경우 컨베이어 쪽에 제품이 없을경우 정지시켜야됨
                                            //GlobalVar.SystemStatus = enumSystemStatus.Manual;
                                        }

                                        continue;
                                    }

                                    // AutoRun 경우 컨베어를 이동시킨다.
                                    
                                    // mainConveyor00.Action = MainConveyorClass.enumAction.Unclamp;
                                }
                            }
                            #endregion


                            //**************************************************************************************************
                            break;
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    FuncLog.WriteLog("SNUC_AmplePackingClass.ActionThread : " + ex.ToString());
                    FuncLog.WriteLog("SNUC_AmplePackingClass.ActionThread : " + ex.StackTrace);
                }

                Thread.Sleep(GlobalVar.ThreadSleep);
            }
        }

        //서브클래스 InitFinish 확인
        private bool Init_Check()
        {
            return (beforeLift01.Action == BeforeLiftClass.enumAction.InitFinish) &&
                (beforeWork02.Action == BeforeWorkClass.enumAction.InitFinish);

       
        }


        
        // 컨베이어에 관련된 하부 부위별 클래스에서는 StepFinish가 true일 경우 이전 완료 후 재시작하지 못하는 상황으로 인지하므로 해당 변수를 false 시켜 속행시키도록 한다.
        private void StepFinish_Conveyor_SubClass_False()
        {
           
            beforeLift01.StepFinish = false;        // #1앰플바디 공급부 완료시



        }

        //서브클래스 Init 동작 지령 확인 후 아니면 지령
        private void Init_Action()
        {


            if (beforeLift01.Action != BeforeLiftClass.enumAction.Init &&
                beforeLift01.Action != BeforeLiftClass.enumAction.InitFinish)
            {
                beforeLift01.Action = BeforeLiftClass.enumAction.Init;
            }
            
            if(beforeWork02.Action != BeforeWorkClass.enumAction.Init &&
               beforeWork02.Action != BeforeWorkClass.enumAction.InitFinish)
            {
               beforeWork02.Action = BeforeWorkClass.enumAction.Init; 
            }
            
         

        }

        public void OpenRobotDoor(bool Door)
        {
            // 도어락키 해제

            DoorLock_Check = Door;

        }
    }

}
#region 파트 주석
//#0 메인 컨베이어

//#1앰플바디공급

//#2 받침대 공급

//#3하부시/받침대 조립

//#4바디/하부시 조립

//#5하 부시 검사

//#6상부시 공급

//#7상부시 검사

//#8픽스쳐 공급 검사

//#8픽스쳐 공급 매거진/트레이

//#9픽스쳐 정렬

//#10캡 조립

//#11 캡 높이 검사

//#12 프린팅 및 배출

//#13 양품 배출

//#13 현대로봇

#endregion
