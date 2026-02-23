using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    class AutoInline
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


        public InShuttle InShuttle = new InShuttle(); //#1 Inshuttle 투입 배출부
        public FrontRack FrontRack = new FrontRack(); //#2 프론트 랙, 리프트 
        public RearRack RearRack = new RearRack(); //#1 샌딩전 트레이 투입 리프트
        public OutShuttle OutShuttle = new OutShuttle(); //#2 샌딩전 트레이 작업위치
        public Scan Scan = new Scan(); //#2 샌딩전 트레이 작업위치

        UInt16 ProcState = 0; // MXP 초기화 제어용
        UInt32 Status = 0; // MXP 초기화 제어용
        int startTime = 0; // MXP 초기화 Timeout 체크용
        #endregion

        #region 일반 변수
        public ulong runTime = GlobalVar.TickCount64; // 공정시간 적산용
        public ulong runTotal = 0; // 공정 카운트 적산용     
        public bool DoorLock_Check = false;

        public bool CheckOpenTurnCCW = false;   //처음 프로그램 켤땐 한번만 셔틀 턴쪽에 CCW 동작 걸어준다 NC동작과 동일 
        #endregion

        #endregion

        #region 클래스 초기화 관련
        //GlobalVar.Class를 내 타입(AutoInline)으로 변환해서 반환하는 static 프로퍼티
        public static AutoInline Class
        {
            get { return (AutoInline)GlobalVar.Class; }
        }
        /**
         * @brief 클래스 생성자
         *        필요한 모든 변수 및 클래스, 쓰레드 등을 초기화한다.
         */
        public AutoInline()
        {
            GlobalVar.Class = this;

            InitGlobals(); // GlobalVar에 선언된 전역변수 세팅

            FuncInline.InitStarted[(int)FuncInline.enumLoading.MasterKernel] = true;
            // 하부 클래스 초기화
            actionThread = new Thread(ActionThread);
            actionThread.Start();

            // 내부 쓰레드 초기화
            systemCheckThread = new Thread(SystemCheckThread);
            systemCheckThread.Start();


            #region 변수 초기화




            #endregion
            #region MsSQL 연결
            if (GlobalVar.UseMsSQL)
            {
                GlobalVar.Sql.Disconnect();
                GlobalVar.Sql = new MsSQL(GlobalVar.MsSQL_Server, GlobalVar.MsSQL_Port, GlobalVar.MsSQL_Id, GlobalVar.MsSQL_Pwd, GlobalVar.MsSQL_DB);
                GlobalVar.Sql.Connect();
                if (!GlobalVar.Sql.connected)
                {
                    FuncLog.WriteLog("DataBase connection Failed!");
                    FuncWin.TopMessageBox("DataBase connection Failed!");
                }
                FuncSql.UpdateDatabase();
            }
            #endregion

            #region Serial Port 연결
            #region Test PC 연결. 통신 테스트 위해 일단 막아둠
            //*
            for (int i = 0; i < FuncInline.ComSMD.Length; i++)
            {
                if (FuncInline.ComSMD[i] == null)
                {
                    FuncInline.ComSMD[i] = new SMDSerial(i);
                    //if (!GlobalVar.Simulation)
                    //{
                    FuncInline.ComSMD[i].SMDSet("COM" + FuncInline.PortTest[i],
                                                           FuncInline.BaudTest[i],
                                                           8,
                                                           FuncInline.ParityTest[i],
                                                           FuncInline.StopBitsTest[i]);

                    if(FuncInline.InlineType < FuncInline.enumInlineType.Gen5)
                    {
                        //펑션 4층은 연결 안함
                        if (i == 4 || i == 8)
                        {
                            continue;
                        }
                    }
                    else if(FuncInline.InlineType == FuncInline.enumInlineType.Gen6)
                    {
                        //통합 하나 빼곤 연결안함
                        if(i > 0)
                        {
                            continue;
                        }
                    }
                   
                    if (!FuncInline.ComSMD[i].Connect())
                    {
                        string label = "";
                        switch (i)
                        {
                            case 0:
                                label = "DownLoad";
                                break;
                            case 1:
                                label = "Front Func 1";
                                break;
                            case 2:
                                label = "Front Func 2";
                                break;
                            case 3:
                                label = "Front Func 3";
                                break;
                            case 4:
                                label = "Front Func 4";
                                break;
                            case 5:
                                label = "Rear Func 1";
                                break;
                            case 6:
                                label = "Rear Func 2";
                                break;
                            case 7:
                                label = "Rear Func 3";
                                break;
                            case 8:
                                label = "Rear Func 4";
                                break;
                            default:
                                break;
                        }
                        FuncWin.TopMessageBox($"{label} Connect fail. PORT is COM { FuncInline.PortTest[i]} ");
                        //FuncWin.AutoClosingMessageBox("TEST PC " + (i + 1) + " connect fail. PORT is COM" + FuncInline.PortTest[i], "notice", 10000);
                    }
                    //}
                }
            }

            FuncInline.Scanner.ComSet("COM" + FuncInline.PortScanner,
                                                           FuncInline.BaudScanner,
                                                           8,
                                                           FuncInline.ParityScanner,
                                                           FuncInline.StopBitsScanner);
            if (!FuncInline.Scanner.Connect())
            {
                FuncWin.TopMessageBox($"Scanner Connect fail. PORT is COM { FuncInline.PortScanner} ");
            }
            //*/
            #endregion


            //*/
            #endregion

            #region motion & io status thread start
            //*
            if (GlobalVar.Simulation == false)
            {
                (new Thread((new StatusThread()).Run)).Start();
                //StatusThread stat = new StatusThread();
                //Thread t1 = new Thread(stat.Run);
                //t1.Start();

                //2025.08.10 고동현 추가
                //LetsStatusThread LetsStat = new LetsStatusThread();
                //Thread t2 = new Thread(LetsStat.Run);
                //t2.Start();

                (new Thread((new LetsStatusThread()).Run)).Start();

                // 26개 사이트 개별 컨트롤
                for (int i = 0; i < FuncInline.MaxSiteCount; i++)
                {
                    (new Thread((new OneSiteAction(i)).Run)).Start();
                }

            }



            //*/
            #endregion



        }

        /**
         * @brief 소멸자
         */
        ~AutoInline()
        {
            ClassDisposing = true;
        }

        /**
         * @brief 프로젝트별로 type 다르게 쓰는 것에 따라 변수를 유동적으로 사용하기 위해 글로벌 변수를 초기화한다.
         */
        public void InitGlobals()
        {
            GlobalVar.ProjectType = enumProject.AutoInline; // 프로젝트 구분
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

            #region 전역변수 초기화
            // PCBInfo 배열을 struct에서 class로 바꿨기 때문에 초기화를 해 주지 않으면 도처에 Null exception이 발생한다.
            for (int i = 0; i < FuncInline.PCBInfo.Length; i++)
            {
                FuncInline.PCBInfo[i] = new PCBInfoClass();
            }
            for (int i = 0; i < FuncInline.OCROrder.Length; i++)
            {
                FuncInline.OCROrder[i] = (i + 1);
            }
         
            #endregion

            #region 전역변수 초기화. Null Pointer 피하기 위해
            for (int i = 0; i < FuncInline.PCBInfo.Length; i++)
            {
                FuncInline.ClearPCBInfo((FuncInline.enumTeachingPos)i);
            }
            //for (int i = 0; i < FuncInline.TeachingPos.Length; i++)
            //{
            //    FuncInline.TeachingPos[i] = new structPosition(0, 0, 0, 0);
            //}
            for (int i = 0; i < FuncInline.CheckPCB.Length; i++)
            {
                FuncInline.CheckPCB[i] = false;
            }
            for (int i = 0; i < FuncInline.SiteCheckTime.Length; i++)
            {
                FuncInline.SiteCheckTime[i] = new Stopwatch();
            }
            for (int i = 0; i < FuncInline.SiteAction.Length; i++)
            {
                FuncInline.SiteAction[i] = FuncInline.enumSiteAction.Waiting;
            }
            FuncInline.ComCommand.Command = FuncInline.enumletsCommand.None;
            for (int i = 0; i < FuncInline.PCBInfoMoveFlag.Length; i++)
            {
                FuncInline.PCBInfoMoveFlag[i] = FuncInline.enumTeachingPos.None;
            }
            #endregion

            #region 시뮬레이션 경우 DI 기본값이 없으므로 각 센서 기본값 강제할당
            if (GlobalVar.Simulation)
            {

                for (int i = 0; i < GlobalVar.AxisStatus.Length; i++)
                {
                    GlobalVar.AxisStatus[i].StandStill = true;
                }
                for (int i = 0; i < GlobalVar.LetsAxisStatus.Length; i++)
                {
                    GlobalVar.LetsAxisStatus[i].StandStill = true;
                }

                /*
                for (int i = 0; i < FuncInline.MaxSiteCount; i++)
                {
                    DIO.WriteDIData(FuncInline.enumDINames.X12_1_Site1_Clamp_Backward_Sensor + i * GlobalVar.DIModuleGap, true);
                }
                //*/
            }
            #endregion

            #region 변수 초기화
            // 배열 이외에는 생성시 기본값 지정되어 있어 초기화할 필요 없다
            #endregion


            #region 설정 저장값 읽기
            string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini";
            string Section = GlobalVar.IniSection;
            GlobalVar.ModelName = FuncFile.ReadIniFile(Section, "DefaultModel", IniPath, "");
            GlobalVar.Language = (enumLanguage)Convert.ToInt16(FuncFile.ReadIniFile("Default", "language", IniPath, "0"));

            // 설정 읽기
            Func.LoadAllIni(); // 모든 설정을 읽어서 전역변수에 저장
            #endregion

            FuncInline.InitDone[(int)FuncInline.enumLoading.GlobalSettings] = true;

            FuncInline.InitDone[(int)FuncInline.enumLoading.SerialPortConnection] = true;
            FuncInline.InitDone[(int)FuncInline.enumLoading.SubForms] = true;
            FuncInline.InitDone[(int)FuncInline.enumLoading.MotorInitialization] = true;
            FuncInline.InitDone[(int)FuncInline.enumLoading.DeviceConnections] = true;
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
                        else if (FuncInline.TabMain != FuncInline.enumTabMain.Auto)
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
                        //DIO.WriteDOData(FuncInline.enumDONames.Y02_2_TOWER_LAMP_GREEN, !DIO.GetDORead(FuncInline.enumDONames.Y02_2_TOWER_LAMP_GREEN));
                        DIO.Tower_Lamp_Green_Control(DIO.Tower_Lamp_Green_Check());
                    }
                    else
                    {
                        if (GlobalVar.TickCount64 - GlobalVar.TowerTick > 1000)
                        {
                            //DIO.WriteDOData(FuncInline.enumDONames.Y02_2_TOWER_LAMP_GREEN, GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, 2, 0]);
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
                        //DIO.WriteDOData(FuncInline.enumDONames.Y02_1_TOWER_LAMP_YELLOW, !DIO.GetDORead(FuncInline.enumDONames.Y02_1_TOWER_LAMP_YELLOW));
                        DIO.Tower_Lamp_Yellow_Control(DIO.Tower_Lamp_Yellow_Check());
                    }
                    else
                    {
                        if (GlobalVar.TickCount64 - GlobalVar.TowerTick > 1000)
                        {
                            //DIO.WriteDOData(FuncInline.enumDONames.Y02_1_TOWER_LAMP_YELLOW, GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, 1, 0]);
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
                        //DIO.WriteDOData(FuncInline.enumDONames.Y02_0_TOWER_LAMP_RED, !DIO.GetDORead(FuncInline.enumDONames.Y02_0_TOWER_LAMP_RED));
                        DIO.Tower_Lamp_Red_Control(DIO.Tower_Lamp_Red_Check());
                    }
                    else
                    {
                        if (GlobalVar.TickCount64 - GlobalVar.TowerTick > 1000)
                        {
                            //DIO.WriteDOData(FuncInline.enumDONames.Y02_0_TOWER_LAMP_RED, GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, 0, 0]);
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
                            FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                                  DateTime.Now.ToString("HH:mm:ss"),
                                                  FuncInline.enumErrorPart.System,
                                                  FuncInline.enumErrorCode.Door_Opened,
                                                  false,
                                                    $"{ FuncInline.OpenDoorInfo1 }"));


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
                        //                          FuncInline.enumErrorPart.System,
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
                        //                          FuncInline.enumErrorPart.System,
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
                                                  FuncInline.enumErrorPart.System,
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
                        if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
                        {
                            bool servoErrored = false;
                            FuncInline.enumServoAxis errorAxis = FuncInline.enumServoAxis.SV00_In_Shuttle;
                            for (FuncInline.enumServoAxis axis = FuncInline.enumServoAxis.SV00_In_Shuttle; axis <= FuncInline.enumServoAxis.SV07_Scan_X; axis++)
                            {
                                if (//axis != FuncInline.enumServoAxis.RobotZ1 &&
                                    Func.ServoErrored(axis))
                                {
                                    servoErrored = true;
                                    Func.ResetServoError(axis);
                                    FuncMotion.ServoOn((uint)axis, true);
                                    GlobalVar.ServoResetCount++;
                                    errorAxis = axis;

                                    Thread.Sleep(GlobalVar.ThreadSleep);
                                }
                            }
                            if (!servoErrored)
                            {
                                GlobalVar.ServoResetCount = 0;
                            }
                            if (GlobalVar.ServoResetCount > GlobalVar.ServoResetMax)
                            {
                                FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                                                DateTime.Now.ToString("HH:mm:ss"),
                                                                FuncInline.enumErrorPart.System,
                                                                FuncInline.enumErrorCode.Axis_Disabled,
                                                                false,
                                                                errorAxis.ToString() + " servo axis disabled. reset and start again."));
                            }
                        }
                    }
                        //호밍 중일때 센서 감지시 즉시 정지
                        //int SV00_BeforeLift = (int)FuncInline.enumServoAxis.SV00_In_Shuttle;
                        //int SV01_AfterLift = (int)FuncInline.enumServoAxis.SV01_Output_Tray_Lift;
                        //if (GlobalVar.AxisStatus[SV00_BeforeLift].Homing)
                        //{
                        //    if (DIO.GetDIData(FuncInline.enumDINames.X01_0_Before_Tray_Input_Lift_End_Sensor))
                        //    {
                        //        FuncMotion.MoveStop(SV00_BeforeLift); //정지상태 되면 서보 정지
                        //        //정지및 알람

                        //        FuncInline.AddError(FuncInline.enumErrorPart.BeforeLift,
                        //           FuncInline.enumErrorCode.Robot_InterLock,
                        //           "샌딩 전 리프트 초기화 중 트레이 센서가 감지(X01_0)되었습니다..\n" +
                        //                  $"센서에 감지되는 부분을 조치하고 다시 초기화해주세요.");

                        //    }
                        //}

                        //if (GlobalVar.AxisStatus[SV01_AfterLift].Homing)
                        //{
                        //    if (DIO.GetDIData(FuncInline.enumDINames.X03_1_After_Tray_Input_Lift_Start_Sensor) ||
                        //        DIO.GetDIData(FuncInline.enumDINames.X03_2_After_Tray_Input_Lift_End_Sensor))
                        //    {
                        //        FuncMotion.MoveStop(SV01_AfterLift); //정지상태 되면 서보 정지
                        //                                             //정지및 알람
                        //        FuncInline.AddError(FuncInline.enumErrorPart.BeforeLift,
                        //            FuncInline.enumErrorCode.Robot_InterLock,
                        //            "샌딩 후 리프트 초기화 중 트레이 센서가 감지(X03_1,X03_2)되었습니다..\n" +
                        //            $"센서에 감지되는 부분을 조치하고 다시 초기화해주세요.");
                        //    }
                        //}
                        #endregion
                        //debug("tmrCheck time 8 : " + (GlobalVar.TickCount64 - tick));
                        #region 조명 컨트롤 B접점
                        //DIO.WriteDOData(FuncInline.enumDONames.Y00_4_LED_Lamp1, GlobalVar.SystemStatus >= enumSystemStatus.AutoRun);
                        #endregion
                        //debug("tmrCheck time 9 : " + (GlobalVar.TickCount64 - tick));
                        #region 상태표시
                        #endregion
                        //debug("tmrCheck time 10 : " + (GlobalVar.TickCount64 - tick));




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

                    // #region [추가] 상태 변경 감지 및 컨베이어 정지 로직
                    // 현재 상태와 이전 상태가 다를 때 (상태가 변한 순간)
                    if (GlobalVar.SystemStatus != GlobalVar.PreSystemStatus)
                    {
                        // 1. 이전 상태가 '자동 운전' 관련 상태였고
                        bool wasRunning = (GlobalVar.PreSystemStatus == enumSystemStatus.AutoRun ||
                                           GlobalVar.PreSystemStatus == enumSystemStatus.InputStop ||
                                           GlobalVar.PreSystemStatus == enumSystemStatus.OutputStop);

                        // 2. 현재 상태가 '수동'이나 '에러/비상정지'로 바뀌었다면
                        bool isStopState = (GlobalVar.SystemStatus == enumSystemStatus.Manual ||
                                            GlobalVar.SystemStatus == enumSystemStatus.EmgStop ||
                                            GlobalVar.SystemStatus == enumSystemStatus.ErrorStop);

                        if (wasRunning && isStopState)
                        {
                            FuncLog.WriteLog("System Mode Changed (Auto -> Stop/Manual). Stop All Conveyors.");

                            // 모든 컨베이어 정지 함수 호출 (한 번만 실행됨)
                            StopAllConveyors();
                            // 2.모든 딜레이 타이머 초기화
                            // 이걸 해야 다시 AutoRun이 되었을 때, 딜레이들이 0초부터 다시 시작함.
                            FuncInline.ResetAllDelays();
                        }
                    }
                    // #endregion
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
                                FuncInline.InitDone[(int)FuncInline.enumLoading.MasterKernel] = true;
                                Controller.Status = 0;
                                Controller.MasterChecked = true;
                                Controller.MasterChecking = false;
                                FuncLetsMotion.initialized = true;
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

                            Thread.Sleep(1000);

                            if (!FuncInline.InitDone[(int)FuncInline.enumLoading.MasterKernel])
                            {
                                FuncLog.WriteLog("MasterKernel Init OK");
                                FuncInline.InitDone[(int)FuncInline.enumLoading.MasterKernel] = true;
                            }

                            if (!FuncLetsMotion.initialized)
                            {
                                //Lets보드 축 변경
                                if (FuncInline.InlineType < FuncInline.enumInlineType.Gen5)
                                {
                                    GlobalVar.LetsAxis_count = (uint)Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length - 1;    //NG버퍼 
                                }
                                else
                                {
                                    GlobalVar.LetsAxis_count = (uint)Enum.GetValues(typeof(FuncInline.enumLetsAxis)).Length;
                                }

                                if (FuncLetsMotion.CheckLetsMotion())
                                {
                                    FuncLog.WriteLog("LetsMotion Init OK");
                                }
                                else
                                {
                                    FuncLog.WriteLog("LetsMotion Init Fail");
                                    return;
                                }

                                Thread.Sleep(FuncInline.Connect_Sleep);

                                if (FuncLetsMotion.CheckLetsScanMotion())
                                {
                                    FuncLog.WriteLog("LetsMotion Scan Node OK");
                                }
                                else
                                {
                                    FuncLog.WriteLog("LetsMotion Scan Node Fail");
                                    return;
                                }
                            }
                          


                            #endregion
                            #endregion

                            //시작시 턴실린더(복동)위치를 잡아줘야함
                            if (CheckOpenTurnCCW == false)
                            {
                                DIO.WriteDOData(FuncInline.enumDONames.Y412_0_IN_Shuttle_Turn_CCW_Cylinder, true);
                                DIO.WriteDOData(FuncInline.enumDONames.Y4_0_IN_Shuttle_Turn_CW_Cylinder, false);
                                DIO.WriteDOData(FuncInline.enumDONames.Y304_4_Out_Shuttle_Turn_Ccw_Cylinder, true);
                                DIO.WriteDOData(FuncInline.enumDONames.Y304_3_Out_Shuttle_Turn_Cw_Cylinder, false);

                                CheckOpenTurnCCW = true;
                            }

                            if (!DoorLock_Check) OpenRobotDoor(true);
                            break;
                        case enumSystemStatus.Initialize:
                            #region 부위별 초기화 완료면 메뉴얼로
                            if (Init_Check() && GlobalVar.SystemStatus != enumSystemStatus.Manual)       //전체 Sub클래스 InitFinish 확인
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
                        case enumSystemStatus.InputStop:
                        case enumSystemStatus.OutputStop:

                            #region 자동일때 변경되어야 할 DIO



                            #endregion


                            //**************************************************************************************************
                            //
                            //**************************************************************************************************
                            #region 사이클스탑 

                            #endregion


                            //**************************************************************************************************
                            break;
                    }
                    //루프의 가장 마지막에 현재 상태를 이전 상태변수에 저장
                    GlobalVar.PreSystemStatus = GlobalVar.SystemStatus;
                    #endregion

                }
                catch (Exception ex)
                {
                    FuncLog.WriteLog("AutoInline_Class.ActionThread : " + ex.ToString());
                    FuncLog.WriteLog("AutoInline_Class.ActionThread : " + ex.StackTrace);
                }

                Thread.Sleep(GlobalVar.ThreadSleep);
            }
        }

        //서브클래스 InitFinish 확인
        private bool Init_Check()
        {
            return (InShuttle.Action == InShuttle.enumAction.InitFinish) &&
                (FrontRack.Action == FrontRack.enumAction.InitFinish) &&
                (RearRack.Action == RearRack.enumAction.InitFinish) &&
                (OutShuttle.OutShuttleAction == OutShuttle.OutShuttle_enumAction.InitFinish) &&
                (Scan.Action == Scan.enumAction.InitFinish);


        }

        //서브클래스 Init 동작 지령 확인 후 아니면 지령
        private void Init_Action()
        {
            if (InShuttle.Action != InShuttle.enumAction.Init &&
                InShuttle.Action != InShuttle.enumAction.InitFinish)
            {
                
                GlobalVar.LetsAxisStatus[(int)FuncInline.enumLetsAxis.ST00_InShuttle_Width].isHomed = false;
                GlobalVar.LetsAxisStatus[(int)FuncInline.enumLetsAxis.ST03_InConveyor_Width].isHomed = false;
                GlobalVar.LetsAxisStatus[(int)FuncInline.enumLetsAxis.ST00_InShuttle_Width].Homing = false;
                GlobalVar.LetsAxisStatus[(int)FuncInline.enumLetsAxis.ST03_InConveyor_Width].Homing = false;
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV00_In_Shuttle].isHomed = false;
               
                InShuttle.Action = InShuttle.enumAction.Init;
                
            }

            if (FrontRack.Action != FrontRack.enumAction.Init &&
               FrontRack.Action != FrontRack.enumAction.InitFinish)
            {
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV02_Lift1].isHomed = false;
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV03_Rack1_Width].isHomed = false;
               
                FrontRack.Action = FrontRack.enumAction.Init;
            }

            if (RearRack.Action != RearRack.enumAction.Init &&
             RearRack.Action != RearRack.enumAction.InitFinish)
            {
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV04_Lift2].isHomed = false;
                
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV05_Rack2_Width].isHomed = false;
               
                RearRack.Action = RearRack.enumAction.Init;
            }

            if (OutShuttle.OutShuttleAction != OutShuttle.OutShuttle_enumAction.Init &&
             OutShuttle.OutShuttleAction != OutShuttle.OutShuttle_enumAction.InitFinish)
            {
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV01_Out_Shuttle].isHomed = false;
                GlobalVar.LetsAxisStatus[(int)FuncInline.enumLetsAxis.ST01_OutShuttle_Width].isHomed = false;
                GlobalVar.LetsAxisStatus[(int)FuncInline.enumLetsAxis.ST02_OutConveyor_Width].isHomed = false;
                GlobalVar.LetsAxisStatus[(int)FuncInline.enumLetsAxis.ST01_OutShuttle_Width].Homing = false;
                GlobalVar.LetsAxisStatus[(int)FuncInline.enumLetsAxis.ST02_OutConveyor_Width].Homing = false;

                if (FuncInline.InlineType >= FuncInline.enumInlineType.Gen5 && FuncInline.InlineType <= FuncInline.enumInlineType.Gen6)
                {
                    GlobalVar.LetsAxisStatus[(int)FuncInline.enumLetsAxis.ST04_NGBuffer].isHomed = false;
                }
               
                OutShuttle.OutShuttleAction = OutShuttle.OutShuttle_enumAction.Init;
               
            }
            //if (OutShuttle.OutConveyorAction != OutShuttle.OutConveyor_enumAction.Init &&
            //OutShuttle.OutConveyorAction != OutShuttle.OutConveyor_enumAction.InitFinish)
            //{
            //    OutShuttle.OutConveyorAction = OutShuttle.OutConveyor_enumAction.Init;
            //}

            if (Scan.Action != Scan.enumAction.Init &&
             Scan.Action != Scan.enumAction.InitFinish)
            {
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV06_Scan_Y].isHomed = false;
                GlobalVar.AxisStatus[(int)FuncInline.enumServoAxis.SV07_Scan_X].isHomed = false;
                Scan.Action = Scan.enumAction.Init;
            }
            GlobalVar.LetsHoming = true;
        }

        public void OpenRobotDoor(bool Door)
        {
            // 도어락키 해제

            DoorLock_Check = Door;

        }
        private void StopAllConveyors()
        {
            try
            {
                // enumDONames에 정의된 모든 IO 이름을 가져와서 검사
                foreach (FuncInline.enumDONames doName in Enum.GetValues(typeof(FuncInline.enumDONames)))
                {
                    string sName = doName.ToString().ToUpper();

    
                    bool isDirectional = sName.Contains("MOTOR_CW") || sName.Contains("MOTOR_CCW");

                    // 위 조건을 모두 만족하면 정지(False)
                    if (isDirectional)
                    {
                        // 현재 켜져있는지 확인하고 끌 수도 있지만, 안전을 위해 무조건 Off
                        DIO.WriteDOData(doName, false);
                    }
                }

                //FuncLog.WriteLog("StopAllConveyors Executed: All Detected Motors Stopped.");
            }
            catch (Exception ex)
            {
                FuncLog.WriteLog($"StopAllConveyors Error: {ex.Message}");
            }
        }
        #region 제어기 초기화

        #region MXP
        private void CheckMXP() // MXP 초기화 함수
        {
            //debug("Environment.TickCount - startTime : " + (Environment.TickCount - startTime));
            if (Environment.TickCount - startTime > 60 * 1000)
            {
                GlobalVar.MasterChecked = false;
                GlobalVar.MasterChecking = false;
            }

            //debug("ProcState : " + ((MXP.MXP_KernelState)ProcState).ToString());
            switch (ProcState)
            {
                case (UInt16)MXP.MXP_KernelState.Idle:
                    {
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Init:
                    {
                        UInt32 status = 0;
                        Int32 InitError;

                        InitError = MXP.MXP_InitKernel_Developer(ref status);
                        Thread.Sleep(1000);

                        InitError = MXP.MXP_InitKernel_Developer(ref status);

                        if (InitError == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Initing;
                            //FuncWin.TopMessageBox("Succeed to initialize MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Idle;
                            //FuncWin.TopMessageBox("Fail to initialize MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Initing:
                    {

                        UInt32 usNumOfSlave = 0;
                        if (MXP.MXP_GetSlaveCount(0/*1: get number of axis, 0: get number of all slaves.*/, out usNumOfSlave) == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            /*
                            int nAxisCnt = 0;

                            // Set combobox items based on number of slave.
                            for (int i = 0; i < usNumOfSlave; i++)
                            {
                                UInt32 usNodeType = 0;
                                string strNum = "";

                                if (MXP.MXP.MXP_QueryNodeType((UInt32)i, out usNodeType) == MXP.MXP.MXP_ret.RET_NO_ERROR)
                                {
                                    if (usNodeType == (UInt32)MXP.MXP.MXP_SlaveType.CiA402_ST)
                                    {
                                        strNum = string.Format("{0}", i);
                                        nAxisCnt++;
                                    }
                                }
                            }
                            //*/
                            ProcState = (UInt16)MXP.MXP_KernelState.Inited;
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Inited:
                    {
                        ProcState = (UInt16)MXP.MXP_KernelState.Run;
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Run:
                    {
                        if (MXP.MXP_SystemRun() == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Running;
                            Status++;
                            if (Status > 3)
                            {
                                //FuncWin.TopMessageBox("Succeed to run MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            //FuncWin.TopMessageBox("Fail to run MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Running:
                    {
                        UInt32 usNumOfSlave = 0;
                        if (MXP.MXP_GetSlaveCount(0/*1: get number of axis, 0: get number of all slaves.*/, out usNumOfSlave) == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            /*
                            int nAxisCnt = 0;

                            // Set combobox items based on number of slave.
                            for (int i = 0; i < usNumOfSlave; i++)
                            {
                                UInt32 usNodeType = 0;
                                string strNum = "";

                                if (MXP.MXP.MXP_QueryNodeType((UInt32)i, out usNodeType) == MXP.MXP.MXP_ret.RET_NO_ERROR)
                                {
                                    if (usNodeType == (UInt32)MXP.MXP.MXP_SlaveType.CiA402_ST)
                                    {
                                        strNum = string.Format("{0}", i);
                                        nAxisCnt++;
                                    }
                                }
                            }
                            //*/
                            ProcState = (UInt16)MXP.MXP_KernelState.Inited;
                        }

                        UInt32 status = 0;
                        MXP.MXP_GetOnlineMode(ref status);
                        /*
                        if (status == (UInt32)MXP.MXP.MXP_ONLINESTATE_ENUM.NET_STATE_OP)
                        {
                            ProcState = (UInt16)MXP.MXP.MXP_KernelState.Runed;
                        }
                        //*/
                        //FuncMotion.ServoOnAll(true);
                        bool servoChecked = true;

                        for (ushort axis = 0; axis < GlobalVar.Axis_count; axis++)
                        {
                            MXP.MXP_READAXISINFO_IN inInfo = new MXP.MXP_READAXISINFO_IN { };
                            MXP.MXP_READAXISINFO_OUT outInfo = new MXP.MXP_READAXISINFO_OUT { };

                            MXP.MXP_READSTATUS_IN statIn = new MXP.MXP_READSTATUS_IN { };
                            MXP.MXP_READSTATUS_OUT statOut = new MXP.MXP_READSTATUS_OUT { };

                            ushort AxisNo = axis;

                            inInfo.Axis.AxisNo = axis;
                            inInfo.Enable = 1;

                            statIn.Axis.AxisNo = axis;
                            statIn.Enable = 1;

                            if (MXP.MXP_ReadAxisInfo(ref inInfo, out outInfo) != MXP.MXP_ret.RET_NO_ERROR ||
                                MXP.MXP_ReadStatus(ref statIn, out statOut) != MXP.MXP_ret.RET_NO_ERROR)
                            {
                                servoChecked = false;
                            }
                            if (!FuncMotion.ServoOn(axis, true))
                            {
                                servoChecked = false;
                            }
                        }
                        if (servoChecked)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Runed;
                            GlobalVar.MasterChecked = true;
                            GlobalVar.MasterChecking = false;
                        }
                        else
                        {
                            // 초기화 실패시 다시 초기화
                            ProcState = (UInt16)MXP.MXP_KernelState.Init;
                            GlobalVar.MasterChecked = false;
                            GlobalVar.MasterChecking = true;
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Runed:
                    {
                        //러닝중
                        Status = 0;
                        GlobalVar.MasterChecked = true;
                        GlobalVar.MasterChecking = false;
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Reset:
                    {
                        if (MXP.MXP_SystemReset() == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Running;
                            //FuncWin.TopMessageBox("Succeed to reset MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //FuncWin.TopMessageBox("Fail to reset MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Close:
                    {
                        Int32 Status = 0;
                        MXP.MXP_GetKernelStatus(out Status);
                        if (Status >= MXP.MXP_SysStatus.Initialized)
                        {
                            if (MXP.MXP_SystemStop() == MXP.MXP_ret.RET_NO_ERROR)
                            {
                                ProcState = (UInt16)MXP.MXP_KernelState.Destory;
                            }
                            else if (Status == 0)
                            {
                                ProcState = (UInt16)MXP.MXP_KernelState.Idle;
                                //FuncWin.TopMessageBox("Already destroy MXP", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else
                            {
                                //FuncWin.TopMessageBox("Fail to stop MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Destory:
                    {
                        if (MXP.MXP_Destroy() == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Idle;
                            //FuncWin.TopMessageBox("Succeed to close MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //FuncWin.TopMessageBox("Fail to close MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                        break;
                    }

            }
        }
        #endregion

        #region MXN
        private bool CheckMXN() // MXN 초기화 함수
        {
            if (GlobalVar.Simulation)
            {
                return true;
            }

            UInt16 usStatus;
            Int32 iRet;
            usStatus = 0;
            iRet = MXN.MXN_InitKernel(ref usStatus);
            if (iRet == MXN.KernelReturn.RET_NO_ERROR && usStatus >= MXN.KernelStatus.SYSTEM_INITED)
                //FuncWin.TopMessageBox("Success to load MXN API.", "SampleVC#", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            else
            {
                //FuncWin.TopMessageBox("Fail to load MXN API.", "SampleVC#", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        #endregion

        #region AXL  
        public static bool CheckAXL() // AXL 초기화 함수
        {
            if (GlobalVar.Simulation)
            {
                return true;
            }
            //++
            // Initialize library 
            uint uRetCode = (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS;
            uRetCode = CAXL.AxlOpen(7);
            if (uRetCode == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                uint uStatus = 0;

                if (CAXD.AxdInfoIsDIOModule(ref uStatus) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    if ((AXT_EXISTENCE)uStatus == AXT_EXISTENCE.STATUS_EXIST)
                    {
                        #region DIO 초기화
                        int nModuleCount = 0;

                        if (CAXD.AxdInfoGetModuleCount(ref nModuleCount) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                        {
                            short i = 0;
                            int nBoardNo = 0;
                            int nModulePos = 0;
                            uint uModuleID = 0;
                            string strData = "";

                            for (i = 0; i < nModuleCount; i++)
                            {
                                if (CAXD.AxdInfoGetModule(i, ref nBoardNo, ref nModulePos, ref uModuleID) == (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                                {
                                    switch ((AXT_MODULE)uModuleID)
                                    {
                                        case AXT_MODULE.AXT_SIO_DI32: strData = String.Format("[{0:D2}:{1:D2}] SIO-DI32", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DO32P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DO32P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DB32P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DB32P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DO32T: strData = String.Format("[{0:D2}:{1:D2}] SIO-DO32T", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DB32T: strData = String.Format("[{0:D2}:{1:D2}] SIO-DB32T", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDI32: strData = String.Format("[{0:D2}:{1:D2}] SIO_RDI32", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO32: strData = String.Format("[{0:D2}:{1:D2}] SIO_RDO32", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB128MLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB128MLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RSIMPLEIOMLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RSIMPLEIOMLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO16AMLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDO16AMLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO16BMLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDO16BMLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB96MLII: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB96MLII", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDO32RTEX: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDO32RTEX", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDI32RTEX: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDI32RTEX", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB32RTEX: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB32RTEX", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DI32_P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DI32_P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_DO32T_P: strData = String.Format("[{0:D2}:{1:D2}] SIO-DO32T_P", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_SIO_RDB32T: strData = String.Format("[{0:D2}:{1:D2}] SIO-RDB32T", nBoardNo, i); break;
                                        case AXT_MODULE.AXT_ECAT_DIO:
                                            uint uModuleSubID = 0;
                                            var szModuleName = new System.Text.StringBuilder(50);
                                            var szModuleDesc = new System.Text.StringBuilder(80);
                                            //CAXD.AxdInfoGetModuleEx(i, ref uModuleSubID, szModuleName, szModuleDesc);
                                            //strData = String.Format("[{0:D2}:{1:D2}] {2}", nBoardNo, i, szModuleName);
                                            break;
                                        default:
                                            strData = String.Format("[{0:D2}:{1:D2}] Unknown", nBoardNo, i);
                                            break;
                                    }
                                    //comboModule.Items.Add(strData);
                                }
                            }
                            //comboModule.SelectedIndex = 0;
                        }
                        #endregion

                        #region  Motion 초기화
                        int m_lAxisCounts = 0;                // 제어 가능한 축갯수 선언 및 초기화
                        int m_lAxisNo = 0;                    // 제어할 축 번호 선언 및 초기화   
                        uint m_uModuleID = 0;                // 제어할 축의 모듈 I/O 선언 및 초기화
                        int m_lBoardNo = 0, m_lModulePos = 0;

                        String strAxis = "";

                        //++ 유효한 전체 모션축수를 반환합니다.
                        uint a = CAXM.AxmInfoGetAxisCount(ref m_lAxisCounts);
                        m_lAxisNo = 0;
                        //++ 지정한 축의 정보를 반환합니다.
                        // [INFO] 여러개의 정보를 읽는 함수 사용시 불필요한 정보는 NULL(0)을 입력하면 됩니다.
                        CAXM.AxmInfoGetAxis(m_lAxisNo, ref m_lBoardNo, ref m_lModulePos, ref m_uModuleID);
                        for (int i = 0; i < m_lAxisCounts; i++)
                        {
                            switch (m_uModuleID)
                            {
                                //++ 지정한 축의 정보를 반환합니다.
                                // [INFO] 여러개의 정보를 읽는 함수 사용시 불필요한 정보는 NULL(0)을 입력하면 됩니다.
                                case (uint)AXT_MODULE.AXT_SMC_4V04: strAxis = String.Format("{0:0}-(AXT_SMC_4V04)", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_2V04: strAxis = String.Format("{0:0}-[AXT_SMC_2V04]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIPM: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIPM]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04PM2Q: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04PM2Q]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04PM2QE: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04PM2QE]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIIPM: strAxis = String.Format("{0:0}-(AXT_SMC_R1V04MLIIIPM)", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIISV: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIISV]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04A5: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04A4]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04A4: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIICL]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04SIIIHMIV: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04SIIIHMIV]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04SIIIHMIV_R: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04SIIIHMIV_R]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIISV: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIISV]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIISV_MD: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIISV_MD]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIIS7S: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIIS7S]", i); break;
                                case (uint)AXT_MODULE.AXT_SMC_R1V04MLIIIS7W: strAxis = String.Format("{0:0}-[AXT_SMC_R1V04MLIIIS7W]", i); break;
                                case (uint)AXT_MODULE.AXT_ECAT_MOTION:
                                    uint uModuleSubID = 0;
                                    var szModuleName = new System.Text.StringBuilder(50);
                                    var szModuleDesc = new System.Text.StringBuilder(80);
                                    //CAXM.AxmInfoGetAxisEx(i, ref uModuleSubID, szModuleName, szModuleDesc);
                                    //strAxis = String.Format("{0:0}-[ECAT-{1}]", i, szModuleName);
                                    break;
                                default: strAxis = String.Format("{0:00}-[Unknown]", i); break;
                            }
                            //cboSelAxis.Items.Add(strAxis);
                        }
                        #endregion
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Module not exist.");
                        //GlobalVar.GlobalStop = true;
                        return false;
                    }
                }
            }
            else
            {
                FuncWin.TopMessageBox("Open Error!");
                //GlobalVar.GlobalStop = true;
                return false;
            }
            return true;
        }
        #endregion

        #endregion
    }

}

