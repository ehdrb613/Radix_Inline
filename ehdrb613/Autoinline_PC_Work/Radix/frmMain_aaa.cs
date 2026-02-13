using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent; // ConcurrentQueue

using System.Diagnostics;//외부 프로그램 컨트롤(Process)



using System.Runtime.InteropServices;
using Radix.Popup;

namespace Radix
{
    public partial class frmMain_Sanding_Megagen : Form
    {
        /* frmMain.cs : 주 HMI 구현용 클래스
         *    1. 장비의 전반적인 상태 및 카운트 등 표시
         *    2. 세부적인 프로세스가 아닌 전반적인 장비 컨트롤
         *    3. 버튼 등을 이용한 기능 분기
         */

        #region 로컬 변수
        UInt16 ProcState = 0; // MXP 초기화 제어용
        UInt32 Status = 0; // MXP 초기화 제어용
        int startTime = 0; // MXP 초기화 Timeout 체크용

        int buzzerTime = Environment.TickCount; // 부저 시간 제어용
        private bool beforeStart = false; // 자동 공정 시작 이전값, 변화시 동작 제어용

        private System.Threading.Timer timerUI; // Thread Timer
        private System.Threading.Timer timerTrayUI; // Thread Timer
        private bool timerDoing = false;
        private bool timerDoing_TrayUI = false;
        private bool led = true; // LED Light 제어용
        private int runTime = Environment.TickCount; // 공정시간 적산용
        private int runTotal = 0; // 공정 카운트 적산용
        private bool initFail = false; // 장치 초기화 체크용
        private int checkTime = Environment.TickCount; // 로더,언로더 체크시간 주기 세팅용   
        string beforeArrayImage = ""; // Array Layout 이미지 변경 체크용

        private bool PCB_Move_Array_PutDown_pre = false;  //Tray에 올려놓기전 true되는 값을 확인하기위해(UI용) by DG 220824

        private bool Warning = false;
        private bool MakeArrayImage = false;
        private int MakeArrayImageCount = 0;

        #region 인라인화한 폼들
        Manual frmManual = new Manual();
        IoMonitor frmIoMonitor = new IoMonitor();
        Machine frmMachine = new Machine();
        Model frmModel = new Model();
        Teaching frmTeaching = new Teaching();
        Trace frmTrace = new Trace();
        LogViewer frmLogViewer = new LogViewer();
        PartClear frmPartClear = new PartClear();
        ErrorDialog errorDialog = null;
        #endregion

        #endregion

        private void debug(string str) // 클래스 내부 콜용 Local Debug
        {
            Util.Debug("frmMain : " + str);
        }

        #region 초기화 관련 함수
        public frmMain_Sanding_Megagen() // 클래스 초기화 함수
        {
            InitializeComponent();
        }

        private void frmMain_Shown(object sender, EventArgs e) // 화면 출력시
        {
#if !DEBUG
            GlobalVar.Debug = false; // Debug로 컴파일시 Debug옵션 켜고, Release시 끈다
#endif

            #region 로그 Thread 시작
            (new Thread((new LogThread()).Run)).Start();
            #endregion

            FuncIni.LoadSimulationIni(); // simulation.ini 파일 있으면 시뮬레이션모드

            #region 화면 배치
            Screen[] sc = Screen.AllScreens; // 모니터 정보

            #region frmMain을 첫 모니터 좌상단에 배치
            this.Left = sc[0].Bounds.Left;
            this.Top = sc[0].Bounds.Top;
            lblVersion.Text = Application.ProductVersion;
            #endregion

            #region 로딩폼을 첫 모니터 가운데 띄우기
            Loading dlgLoading = new Loading();
            dlgLoading.TopMost = true;
            dlgLoading.Show();
            dlgLoading.Left = sc[0].Bounds.Right / 2 - dlgLoading.Width / 2;
            dlgLoading.Top = sc[0].Bounds.Bottom / 2 - dlgLoading.Height / 2;
            #endregion

            #region NG 모니터 화면을 두 번째 모니터 좌상단에 띄우기
            //NGViewer dlgNG = new NGViewer();
            //dlgNG.Show();
            //if (sc.Length > 0)
            //{
            //    dlgNG.Left = sc[1].Bounds.Left;
            //    dlgNG.Top = sc[1].Bounds.Top;
            //}
            #endregion

            #region PartClear 사용 유무에 따라 버튼 표시
            pbPartClear.Visible = GlobalVar.UsePartClear;
            if (!GlobalVar.UsePartClear)
            {
                Point loc = pbErrorLog.Location;
                loc.Y -= pbPartClear.Size.Height;
                pbErrorLog.Location = loc;

                loc = pbReset.Location;
                loc.Y -= pbPartClear.Size.Height;
                pbReset.Location = loc;

                loc = pbBuzzerStop.Location;
                loc.Y -= pbPartClear.Size.Height;
                pbBuzzerStop.Location = loc;
            }
            #endregion
            #region CycleStop 사용 유무에 따라 버튼 표시
            pbCycleStop.Visible = GlobalVar.UseCycleStop;
            if (!GlobalVar.CycleStop)
            {
                Point loc = pbErrorLog.Location;
                loc.Y -= pbCycleStop.Size.Height;
                pbErrorLog.Location = loc;

                loc = pbReset.Location;
                loc.Y -= pbCycleStop.Size.Height;
                pbReset.Location = loc;

                loc = pbInit.Location;
                loc.Y -= pbCycleStop.Size.Height;
                pbInit.Location = loc;

                loc = pbPartClear.Location;
                loc.Y -= pbCycleStop.Size.Height;
                pbPartClear.Location = loc;

                loc = pbBuzzerStop.Location;
                loc.Y -= pbCycleStop.Size.Height;
                pbBuzzerStop.Location = loc;
            }
            #endregion
            #endregion

            this.Activate(); // 다른 화면 띄우다가 포커스 넘어갈 수 있어서 강제로 포커스 가져온다.


            /* // 시뮬레이션시 UI변경 천천히 돌려 확인시 사용
            if (GlobalVar.Simulation)
            {
                GlobalVar.ThreadSleep = 1000;
            }
            //*/

            #region Ecat Master 등의 초기화 및 동작여부 체크

            #region 제어기
            GlobalVar.MasterChecked = false;
            GlobalVar.MasterChecking = true;

            if (GlobalVar.Simulation)
            {
                Status = 0;
                GlobalVar.MasterChecked = true;
                GlobalVar.MasterChecking = false;
            }
            else
            {
                if (GlobalVar.MasterType == enumMasterType.MXP)
                {
                    ProcState = (UInt16)MXP.MXP_KernelState.Init;
                }
                else if (GlobalVar.MasterType == enumMasterType.MXN)
                {
                    //MXN은 필요 없을 것 같다.
                }
                else if (GlobalVar.MasterType == enumMasterType.AXL)
                {
                    //아진은 필요 없을 것 같다.
                }
                else if (GlobalVar.MasterType == enumMasterType.ADVANTECH)
                {
                    if (!FuncAdvantech.InitializeComponent())
                    {
                        FuncLog.WriteLog("Get Device Numbers Failed!");
                        this.BringToFront();
                        MessageBox.Show("Get Device Numbers Failed!");
                        initFail = true;
                        GlobalVar.GlobalStop = true;
                        this.Close();
                        return;
                    }

                    FuncAdvantech.OpenBoard();
                    FuncAdvantechDIO.InitializeComponent();
                }
                GlobalVar.MasterChecked = false;
                GlobalVar.MasterChecking = true;
            }
            startTime = Environment.TickCount;
            Application.DoEvents();

            // 체크 완료까지 대기
            if (GlobalVar.MasterType == enumMasterType.MXP)
            {
                while (GlobalVar.MasterChecking &&
                    ProcState != (UInt16)MXP.MXP_KernelState.Runed)
                {
                    Thread.Sleep(1000);
                    CheckMXP();
                }
            }
            else if (GlobalVar.MasterType == enumMasterType.MXN)
            {
                GlobalVar.MasterChecked = CheckMXN();
                GlobalVar.MasterChecking = false;
            }
            else if (GlobalVar.MasterType == enumMasterType.AXL)
            {
                GlobalVar.MasterChecked = CheckAXL();
                GlobalVar.MasterChecking = false;

                #region 메가젠 인플란트 축동기화(갠트리) Setup by DGKim
                if (GlobalVar.MasterChecked)
                {

                    if (GantrySetup(0)) //갠트리 셋업 ON되어 있으면 Slave ServoON 안될때가 있어서 처음엔 OFF 해주고 Servo ON 하고 난뒤 다시 ON
                    {
                        Thread.Sleep(300);
                        if (GantrySetup(1)) //갠트리 셋업 ON되어 있으면 Slave ServoON 안될때가 있어서 처음엔 OFF 해주고 Servo ON 하고 난뒤 다시 ON
                        {
                            //next
                        }
                        else
                        {
                            FuncLog.WriteLog("Gantry Setup Failed!");
                            initFail = true;
                            GlobalVar.GlobalStop = true;
                            this.Close();
                            return;
                        }
                    }
                    else
                    {
                        FuncLog.WriteLog("Gantry Setup Failed!");
                        initFail = true;
                        GlobalVar.GlobalStop = true;
                        this.Close();
                        return;
                    }
                }
                #endregion 메가젠 인플란트 축동기화(갠트리) Setup by DGKim
            }

            if (!GlobalVar.MasterChecked)
            {
                try
                {
                    dlgLoading.Close();
                }
                catch { }
                FuncLog.WriteLog("Kernel Init Failed!");
                this.BringToFront();
                FuncWin.TopMessageBox("Kernel Init Failed!");
                initFail = true;
                GlobalVar.GlobalStop = true;
                this.Close();
                return;
            }



            #endregion

            #region motion & io status thread start
            //*
            if (GlobalVar.Simulation == false)
            {
                StatusThread stat = new StatusThread();
                Thread t1 = new Thread(stat.Run);
                t1.Start();
            }
            //*/
            #endregion

            #region 각 외부 통신 인터페이스 관련 추가
            if (!GlobalVar.Simulation)//각 외부 통신 인터페이스 관련 추가
            {

                /*
                GlobalVar.socketLeuzeTrigger.ReceiveTimeout = 500;
                GlobalVar.socketLeuzeData.ReceiveTimeout = 500;
                try
                {
                    GlobalVar.socketLeuzeTrigger.Connect(GlobalVar.Vision_IP, GlobalVar.Vision_Trigger_Port);
                    if (!GlobalVar.socketLeuzeTrigger.Connected)
                    {
                        try
                        {
                            dlgLoading.Close();
                        }
                        catch { }
                        Func.WriteLog("Vision Connection Failed!");
                        this.BringToFront();
                        Util.TopMessageBox("Vision Connection Failed!");
                        initFail = true;
                        this.Close();
                        return;
                    }
                    GlobalVar.streamLeuzeTrigger = GlobalVar.socketLeuzeTrigger.GetStream();
                }
                catch (Exception ex)
                {
                    try
                    {
                        dlgLoading.Close();
                    }
                    catch { }
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    Func.WriteLog("Vision Connection Failed!");
                    this.BringToFront();
                    Util.TopMessageBox("Vision Connection Failed!");
                    initFail = true;
                    this.Close();
                    return;
                }
                //*/

                /*
                try
                {
                    GlobalVar.socketLeuzeData.Connect(GlobalVar.Vision_IP, GlobalVar.Vision_Receive_Port);

                    if (!GlobalVar.socketLeuzeData.Connected)
                    {
                        try
                        {
                            dlgLoading.Close();
                        }
                        catch { }
                        Func.WriteLog("Vision Connection Failed!");
                        this.BringToFront();
                        this.BringToFront();
                        Util.TopMessageBox("Vision Connection Failed!");
                        initFail = true;
                        this.Close();
                        return;
                    }
                    GlobalVar.streamLeuzeData = GlobalVar.socketLeuzeData.GetStream();
                }
                catch (Exception ex)
                {
                    try
                    {
                        dlgLoading.Close();
                    }
                    catch { }
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    Func.WriteLog("Vision Connection Failed!");
                    Util.TopMessageBox("Vision Connection Failed!");
                    initFail = true;
                    this.Close();
                    return;
                }
                //*/

                //Autonics 스텝 모터 사용
                /*
                if (!GlobalVar.Simulation &&
                    (!GlobalVar.WorkConv.Open() ||
                        !GlobalVar.WorkWidth.Open()))
                {
                    try
                    {
                        dlgLoading.Close();
                    }
                    catch { }
                    Func.WriteLog("Motor Init Failed!");
                    this.BringToFront();
                    Util.TopMessageBox("Motor Init Failed!");
                    initFail = true;
                    this.Close();
                    return;
                }
                //*/

            }
            #endregion

            #region KUKA 초기화 체크
            ///*
            if (GlobalVar.RobotUse &&
                !GlobalVar.Simulation)
            {
                if (GlobalVar.Cifx.xDriverOpen() == 0)
                {
                    //Open the Sysdevice to get the handle
                    GlobalVar.Cifx.xSysdeviceOpen(GlobalVar.SzBoard);
                    //Open the channel to get the handle
                    if (GlobalVar.Cifx.xChannelOpen(GlobalVar.SzBoard, GlobalVar.PhChannel) == 0)
                    {
                        cifXUser.ActiveChannel = GlobalVar.PhChannel;
                        cifXUser.ActiveBoard = GlobalVar.SzBoard;

                    }
                    else
                    {
                        MessageBox.Show("Robot Communication Fail");
                        this.Close();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Robot Communication Fail");
                    this.Close();
                    return;
                }
            }
            //*/
            #endregion

            #endregion

            FuncLog.WriteLog("Program Started");


            // IO 초기화

            #region 자동 동작 Thread 시작

            /*
            AutoRun autoThread = new AutoRun();
            Thread auto = new Thread(autoThread.Run);
            (new Thread((new AutoRun()).Run)).Start();
            //*/

            //(new Thread((new AutoRun()).Run)).Start(); // 쓰레드 후관리할 거 아니면 굳이 로컬변수로 저장할 필요 없다.
            //(new Thread((new HighGain_PCBSupply()).Run)).Start();

            (new Thread((new Sanding_Before_Tray_MGG()).Run)).Start();//샌딩전 쓰레드
            (new Thread((new Sanding_After_Tray_MGG()).Run)).Start();//샌딩후 쓰레드
            (new Thread((new Sanding_Auto_Logic()).Run)).Start();//샌딩후 쓰레드

            (new Thread((new KUKAThread()).Run)).Start();//Kuka 쓰레드
            if (GlobalVar.RobotUse)
            {
                (new Thread((new cifxThread()).Run)).Start();//DeviceNet 쓰레드
                
            }

            #endregion

            Thread.Sleep(1000); // 각 쓰레드 구동 확인 위해 잠시 대기

            #region 프로그램 시작시 출력이 나가야 하는 DIO
            if (GlobalVar.Simulation)
            {
                DIO.InitSimulation();
            }
            DIO.InitDIO((int)DIO.DiType);
            //DIO.WriteDOData(enumDONames.Y15_0_Robot_Emg1, true);
            //DIO.WriteDOData(enumDONames.Y15_1_Robot_Emg2, true);
            //DIO.WriteDOData(enumDONames.Y18_0_Robot_Move_Enable, true);
            //DIO.WriteDOData(enumDONames.Y17_0_Boxer_Box_Check, true);
            #endregion

            #region 시뮬레이션 경우 DI 기본값이 없으므로 각 센서 기본값 강제할당
            if (GlobalVar.Simulation)
            {
                DIO.EMG_Control(true);
            }
            #endregion

            #region 변수 초기화
            // 배열 이외에는 생성시 기본값 지정되어 있어 초기화할 필요 없다
            #endregion

            #region 설정 저장값 읽기
            string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini";
            string Section = GlobalVar.IniSection;
            GlobalVar.ModelName = FuncIni.ReadIniFile(Section, "DefaultModel", IniPath, "");
            GlobalVar.Language = (enumLanguage)Convert.ToInt16(FuncIni.ReadIniFile("Default", "language", IniPath, "0"));

            // 설정 읽기
            FuncIni.LoadAllIni(); // 모든 설정을 읽어서 전역변수에 저장
            #endregion


            FuncLog.DeleteLogs();
            ////////////////////////////////////

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
                else
                {
                    FuncSql.UpdateDatabase();
                }
            }
            #endregion

            #region 로케일 적용
            setLanguage();
            #endregion

            Ecat.ServoReset_All();

            //for (ushort axis = 0; axis < GlobalVar.Axis_count; axis++)
            //{
            //    //GlobalVar.AxisStatus[axis].StandStill = false;
            //    Ecat.ServoReset(axis);
            //}

            Ecat.ServoOnAll(true); // 모든 서보 On

            #region 메가젠 인플란트 축동기화(갠트리) Setup by DGKim
            if (GlobalVar.MasterType == enumMasterType.AXL && GlobalVar.MasterChecked)
            {
                //GantrySetup(1); //갠트리 셋업 ON되어 있으면 Slave ServoON 안될때가 있어서 처음엔 OFF 해주고 Servo ON 하고 난뒤 다시 ON
            }
            #endregion 메가젠 인플란트 축동기화(갠트리) Setup by DGKim
            //버튼 만들기  
            //세로 갯수, 가로 갯수, 가로 크기, 세로 크기, 
            //Make_Burron(5, 5, 100, 100, 10, 100, 10, 100);
            //Make_PictureBox(ref Before_PictureBox, "Before", FuncSanding.Sanding_BeforeTraySize[0], FuncSanding.Sanding_BeforeTraySize[1], 20, 20, 230, -20, 200, 20);
            //Make_PictureBox(ref After_PictureBox, "After", FuncSanding.Sanding_AfterTraySize[0], FuncSanding.Sanding_AfterTraySize[1], 20, 20, 1010, -20, 200, 20);

            // Make_PictureBox(ref Before_PictureBox, "Before", FuncSanding.Sanding_BeforeTraySize[0], FuncSanding.Sanding_BeforeTraySize[1], 18, 18, 38, 17, 62, 18);

            //이미지 및 바코드 컨트롤 생성 by DGKim
            Make_Controls<Button>(ref Before_TrayBtn, "BeforeTray", FuncSanding.Sanding_BeforeTray_RackSize.GetLength(0), FuncSanding.Sanding_BeforeTray_RackSize.GetLength(1), 58, 25, 299, -72, 490, -47);

            //Make_PictureBox(ref After_PictureBox, "After", FuncSanding.Sanding_AfterTraySize[0], FuncSanding.Sanding_AfterTraySize[1], 18, 18, 532, 23, 62, 18);531, 312
            //Make_Controls<PictureBox>(ref After_PictureBox, "After", FuncSanding.Sanding_AfterTraySize[0], FuncSanding.Sanding_AfterTraySize[1], 18, 18, 532, 25, 62, 18);
            //Make_Controls<PictureBox>(ref After_PictureBox, "After", FuncSanding.Sanding_AfterTraySize[0], FuncSanding.Sanding_AfterTraySize[1], 18, 18, 807, -25, 62, 18); // YJ 시작좌표와 피치 방향 변경
            Make_Controls<PictureBox>(ref After_PictureBox, "After", FuncSanding.Sanding_AfterTraySize[0], FuncSanding.Sanding_AfterTraySize[1], 18, 18, 531, 25, 314, -18); // YJ 시작좌표와 피치 방향 변경
            Make_Controls<Button>(ref After_TrayBtn, "AfterTray", FuncSanding.Sanding_AfterTray_RackSize.GetLength(0), FuncSanding.Sanding_AfterTray_RackSize.GetLength(1), 58, 25, 11, 72, 490, -47);


            numBeforeTrayIndex.Maximum = FuncSanding.Sanding_BeforeTraySize[0] * FuncSanding.Sanding_BeforeTraySize[1] + 1;
            if (FuncSanding.Megagen)
            {
                numAfterTrayIndex.Maximum = FuncSanding.Sanding_AfterTraySize[0] * FuncSanding.Sanding_AfterTraySize[1] + 1 - 30;//-30 홈파기 때문
            }
            else
            {
                numAfterTrayIndex.Maximum = FuncSanding.Sanding_AfterTraySize[0] * FuncSanding.Sanding_AfterTraySize[1] + 1;
            }


            #region 일반 타이머 시작
            //Thread.Sleep(500);
            tmrCheck.Enabled = true;
            tmrError.Enabled = true;
            #endregion

            #region 화면 제어용 쓰레드 타이머 시작
            TimerCallback CallBackUI = new TimerCallback(TimerUI);
            timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);

            TimerCallback CallBackTrayUI = new TimerCallback(TimerTrayUI);
            timerTrayUI = new System.Threading.Timer(CallBackTrayUI, false, 0, 500);
            #endregion


            #region 장비 초기화 창을 열어서 초기화 진행
            /*
            Init dlg = new Init();
            dlg.Show();
            //*/
            #endregion

            #region 모델 목록
            //Func.GetModelList();
            //cmbModel.Items.Clear();
            //for (int i = 0; i < GlobalVar.ModelNames.Length; i++)
            //{
            //    if (GlobalVar.ModelNames[i] == null ||
            //        GlobalVar.ModelNames[i].Length == 0)
            //    {
            //        break;
            //    }
            //    cmbModel.Items.Add(GlobalVar.ModelNames[i]);
            //    if (GlobalVar.ModelNames[i] == GlobalVar.ModelName)
            //    {
            //        cmbModel.SelectedIndex = cmbModel.Items.Count - 1;
            //    }
            //}

            ////모델 변경후 Main누를때 체크하고 모델명 변경하기 위해 by DG 220913
            //if (GlobalVar.MasterType == enumMasterType.MXN)
            //{
            //    MXN.MXN_GetModelInfo(GlobalVar.ModelPath_MXN, GlobalVar.ModelName_MXN);
            //    LabelModelName.Text = GlobalVar.ModelName_MXN.ToString();

            //    Double ReadData = 0;
            //    MXN.MXN_Read_MGN(165, ref ReadData);
            //    life1.Text = ReadData.ToString("F2") + " M";


            //    MXN.MXN_Read_MGN(166, ref ReadData);
            //    life2.Text = ReadData.ToString("F2") + " M";

            //    MXN.MXN_Read_MGN(167, ref ReadData);
            //    life3.Text = ReadData.ToString("F2") + " M";

            //    MXN.MXN_Read_MGN(40, ref ReadData);
            //    lifeSet.Text = ReadData.ToString("F2") + " M";
            //}

            #endregion

            #region sub form 초기화
            //frmManual = new Manual();
            frmManual.FormBorderStyle = FormBorderStyle.None;
            frmManual.TopMost = true;
            frmManual.Dock = DockStyle.Fill;
            frmManual.TopLevel = false;
            tpManual.Controls.Clear();
            tpManual.Controls.Add(frmManual);
            frmManual.Show();

            //frmIoMonitor = new IoMonitor();
            frmIoMonitor.FormBorderStyle = FormBorderStyle.None;
            frmIoMonitor.TopMost = true;
            frmIoMonitor.Dock = DockStyle.Fill;
            frmIoMonitor.TopLevel = false;
            tpIO.Controls.Clear();
            tpIO.Controls.Add(frmIoMonitor);
            frmIoMonitor.Show();

            //frmModel = new Model();
            frmModel.FormBorderStyle = FormBorderStyle.None;
            frmModel.TopMost = true;
            frmModel.Dock = DockStyle.Fill;
            frmModel.TopLevel = false;
            tpModel.Controls.Clear();
            tpModel.Controls.Add(frmModel);
            frmModel.Show();

            //frmLogViewer = new LogViewer();
            frmLogViewer.FormBorderStyle = FormBorderStyle.None;
            frmLogViewer.TopMost = true;
            frmLogViewer.Dock = DockStyle.Fill;
            frmLogViewer.TopLevel = false;
            tpErrors.Controls.Clear();
            tpErrors.Controls.Add(frmLogViewer);
            frmLogViewer.Show();

            //frmMachine = new Machine();
            frmMachine.FormBorderStyle = FormBorderStyle.None;
            frmMachine.TopMost = true;
            frmMachine.Dock = DockStyle.Fill;
            frmMachine.TopLevel = false;
            tpMachine.Controls.Clear();
            tpMachine.Controls.Add(frmMachine);
            frmMachine.Show();

            //frmTeaching = new Teaching();
            //frmTeaching.FormBorderStyle = FormBorderStyle.None;
            //frmTeaching.TopMost = true;
            //frmTeaching.Dock = DockStyle.Fill;
            //frmTeaching.TopLevel = false;
            //tpTeaching.Controls.Clear();
            //tpTeaching.Controls.Add(frmTeaching);
            //frmTeaching.Show();

            //frmPartClear = new PartClear();
            //frmPartClear.FormBorderStyle = FormBorderStyle.None;
            //frmPartClear.TopMost = true;
            //frmPartClear.Dock = DockStyle.Fill;
            //frmPartClear.TopLevel = false;
            //tpPartClear.Controls.Clear();
            //tpPartClear.Controls.Add(frmPartClear);
            //frmPartClear.Show();

            //frmTrace = new Trace();
            //frmTrace.FormBorderStyle = FormBorderStyle.None;
            //frmTrace.TopMost = true;
            //frmTrace.Dock = DockStyle.Fill;
            //frmTrace.TopLevel = false;
            //tpTrace.Controls.Clear();
            //tpTrace.Controls.Add(frmTrace);
            //frmTrace.Show();

            //for (int i = 0; i < tcMain.TabPages.Count; i++)
            //{
            //    tcMain.SelectedIndex = i;
            //    Thread.Sleep(100);
            //}
            tcMain.SelectedIndex = 0;
            #endregion



            try
            {
                dlgLoading.Close();
            }
            catch { }
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e) // 프로그램 종료시
        {

            if (!initFail)
            {
                FuncLog.WriteLog("Program Ended");
                if (GlobalVar.Simulation == false)
                {
                    //GlobalVar.WorkWidth.Close();
                    //GlobalVar.WorkConv.Close();
                    //Ecat.ServoOnAll(false);
                }


                if (GlobalVar.UseMsSQL &&
                    GlobalVar.Sql.connected)
                {
                    GlobalVar.Sql.Disconnect();
                }
            }

            try
            {
                timerUI.Dispose();
                timerTrayUI.Dispose();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }

            if (GlobalVar.Simulation == true)
            {
                Environment.Exit(Environment.ExitCode);//프로그램 남는거 때문에
                                                       //base.OnFormClosed(e);
                                                       //Dispose(); // 폼 해제
            }




        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) // 사용자 확인 후 프로그램 종료 시도
        {
            if (!initFail)
            {
                if ((int)GlobalVar.SystemStatus < (int)enumSystemStatus.AutoRun)
                {
                    this.BringToFront();
                    if (!GlobalVar.MasterChecked ||
                        FuncWin.MessageBoxOK("Terminate Program?"))
                    {
                        ClearSubMenu();
                        pbRouter.BackgroundImage = Radix.Properties.Resources.sub_Router_sel;

                        GlobalVar.TabMain = enumTabMain.Router;
                        tcMain.SelectedIndex = (int)GlobalVar.TabMain;

                        GlobalVar.GlobalStop = true;

                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    e.Cancel = true;
                    FuncWin.TopMessageBox("Can't close program while system is running.");
                }
                //timerKernel.Dispose();
            }
        }

        #region 제어기 초기화

        #region MXP
        private void CheckMXP() // MXP 초기화 함수
        {
            debug("Environment.TickCount - startTime : " + (Environment.TickCount - startTime));
            if (Environment.TickCount - startTime > 60 * 1000)
            {
                GlobalVar.MasterChecked = false;
                GlobalVar.MasterChecking = false;
            }

            debug("ProcState : " + ((MXP.MXP_KernelState)ProcState).ToString());
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
                            //Util.TopMessageBox("Succeed to initialize MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Idle;
                            //Util.TopMessageBox("Fail to initialize MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                                //Util.TopMessageBox("Succeed to run MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            //Util.TopMessageBox("Fail to run MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                        //Ecat.ServoOnAll(true);
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
                            if (!Ecat.ServoOn(axis, true))
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
                            //Util.TopMessageBox("Succeed to reset MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //Util.TopMessageBox("Fail to reset MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                                //Util.TopMessageBox("Already destroy MXP", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                            else
                            {
                                //Util.TopMessageBox("Fail to stop MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            }
                        }
                        break;
                    }
                case (UInt16)MXP.MXP_KernelState.Destory:
                    {
                        if (MXP.MXP_Destroy() == MXP.MXP_ret.RET_NO_ERROR)
                        {
                            ProcState = (UInt16)MXP.MXP_KernelState.Idle;
                            //Util.TopMessageBox("Succeed to close MXP.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            //Util.TopMessageBox("Fail to close MXP!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                //Util.TopMessageBox("Success to load MXN API.", "SampleVC#", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            else
            {
                //Util.TopMessageBox("Fail to load MXN API.", "SampleVC#", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }
        #endregion

        #region AXL  

        private bool CheckAXL() // AXL 초기화 함수
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
                                            CAXD.AxdInfoGetModuleEx(i, ref uModuleSubID, szModuleName, szModuleDesc);
                                            strData = String.Format("[{0:D2}:{1:D2}] {2}", nBoardNo, i, szModuleName);
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
                                    CAXM.AxmInfoGetAxisEx(i, ref uModuleSubID, szModuleName, szModuleDesc);
                                    strAxis = String.Format("{0:0}-[ECAT-{1}]", i, szModuleName);
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
                        return false;
                    }
                }
            }
            else
            {
                FuncWin.TopMessageBox("Open Error!");
                return false;
            }
            return true;
        }

        #region 메가젠임플란트 Z축 1,2 동기화(갠트리) setup
        private bool GantrySetup(int On)
        {
            if (GlobalVar.Simulation)
            {
                return true;
            }
            //======== 겐트리 관련 함수==========================================================================================
            // 모션모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어를 지원한다. 
            // 이 함수를 이용해 Master축을 겐트리 제어로 설정하면 해당 Slave축은 Master축과 동기되어 구동됩니다. 
            // 만약 겐트리 설정 이후 Slave축에 구동명령이나 정지 명령등을 내려도 모두 무시됩니다.
            // uSlHomeUse     : 슬레이축 홈사용 우뮤 ( 0 - 2)
            //             (0 : 슬레이브축 홈을 사용안하고 마스터축을 홈을 찾는다.)
            //             (1 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정함.)
            //             (2 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정안함.)
            // dSlOffset      : 슬레이브축 옵셋값
            // dSlOffsetRange : 슬레이브축 옵셋값 레인지 설정
            // 주의사항       : 갠트리 ENABLE시 슬레이브축은 모션중 AxmStatusReadMotion 함수로 확인하면 True(Motion 구동 중)로 확인되야 정상동작이다. 
            //                  슬레이브축에 AxmStatusReadMotion로 확인했을때 InMotion 이 False이면 Gantry Enable이 안된것이므로 알람 혹은 리밋트 센서 등을 확인한다.
            uint duRetCodeBefore;
            uint duRetCodeAfter;
            uint duSlaveHmUse = 0, duGantryOnBefore = 0, duGantryOnAfter = 0;
            double dSlaveHmOffset = 0.0, dSlaveHmRange = 1.0;

            //++ 지정한 축의 겐트리제어 관련 설정값을 확인합니다.
            CAXM.AxmGantryGetEnable((int)enum_Sanding_ServoAxis.Sanding_Before_Z1, ref duSlaveHmUse, ref dSlaveHmOffset, ref dSlaveHmRange, ref duGantryOnBefore);

            lbBeforeGantryHmOffset.Text = Convert.ToString(dSlaveHmOffset);
            lbBeforeGantryHmRange.Text = Convert.ToString(dSlaveHmRange);
            lbBeforeGantryEnable.Text = Convert.ToString(duGantryOnBefore);

            //++ 지정한 축의 겐트리제어 관련 설정값을 확인합니다.
            CAXM.AxmGantryGetEnable((int)enum_Sanding_ServoAxis.Sanding_After_Z1, ref duSlaveHmUse, ref dSlaveHmOffset, ref dSlaveHmRange, ref duGantryOnAfter);

            lbAfterGantryHmOffset.Text = Convert.ToString(dSlaveHmOffset);
            lbAfterGantryHmRange.Text = Convert.ToString(dSlaveHmRange);
            lbAfterGantryEnable.Text = Convert.ToString(duGantryOnAfter);


            duSlaveHmUse = 0;
            dSlaveHmOffset = Convert.ToDouble(lbBeforeGantryHmOffset.Text);
            dSlaveHmRange = Convert.ToDouble(lbBeforeGantryHmRange.Text);
            //++ 지정한 Master축과 Slave축으로 겐트리 기능을 활성화 시킵니다.
            //[INFO] 겐트리 제어 기능을 활성화 시키고 이후 Slave축에 구동 명령이나 정지 명령등을 내려도 모두 무시 됩니다.
            if (duGantryOnBefore == 1 && On != 1)
            {
                duRetCodeBefore = CAXM.AxmGantrySetDisable((int)enum_Sanding_ServoAxis.Sanding_Before_Z1, (int)enum_Sanding_ServoAxis.Sanding_Before_Z2);
            }
            else if (duGantryOnBefore == 0 && On == 1)
            {
                duRetCodeBefore = CAXM.AxmGantrySetEnable((int)enum_Sanding_ServoAxis.Sanding_Before_Z1, (int)enum_Sanding_ServoAxis.Sanding_Before_Z2, duSlaveHmUse, dSlaveHmOffset, dSlaveHmRange);

                if (duRetCodeBefore != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    lbBeforeGantryEnable.Text = "Fail";
                    MessageBox.Show(String.Format("AxmGantrySetEnable return error[Code:{0:d}]", duRetCodeBefore));
                    logView($"AxmGantrySetEnable return error[Code:{duRetCodeBefore}]", "BeforeLog");
                    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                               DateTime.Now.ToString("HH:mm:ss"),
                                               enumErrorPart.System,
                                               enumErrorCode.E_Stop,
                                               false,
                                               "Before Gantry Setup Error."));
                    return false;
                }
            }




            duSlaveHmUse = 0;
            dSlaveHmOffset = Convert.ToDouble(lbAfterGantryHmOffset.Text);
            dSlaveHmRange = Convert.ToDouble(lbAfterGantryHmRange.Text);
            //++ 지정한 Master축과 Slave축으로 겐트리 기능을 활성화 시킵니다.
            //[INFO] 겐트리 제어 기능을 활성화 시키고 이후 Slave축에 구동 명령이나 정지 명령등을 내려도 모두 무시 됩니다.
            if (duGantryOnAfter == 1 && On != 1)
            {
                duRetCodeAfter = CAXM.AxmGantrySetDisable((int)enum_Sanding_ServoAxis.Sanding_After_Z1, (int)enum_Sanding_ServoAxis.Sanding_After_Z2);
            }
            else if (duGantryOnAfter == 0 && On == 1)
            {
                duRetCodeAfter = CAXM.AxmGantrySetEnable((int)enum_Sanding_ServoAxis.Sanding_After_Z1, (int)enum_Sanding_ServoAxis.Sanding_After_Z2, duSlaveHmUse, dSlaveHmOffset, dSlaveHmRange);
                if (duRetCodeAfter != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    lbAfterGantryEnable.Text = "Fail";
                    MessageBox.Show(String.Format("AxmGantrySetEnable return error[Code:{0:d}]", duRetCodeAfter));
                    logView($"AxmGantrySetEnable return error[Code:{duRetCodeAfter}]", "AfterLog");
                    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                              DateTime.Now.ToString("HH:mm:ss"),
                                              enumErrorPart.System,
                                              enumErrorCode.E_Stop,
                                              false,
                                              "After Gantry Setup Error."));
                    return false;
                }
            }


            //++ 지정한 축의 겐트리제어 관련 설정값을 확인합니다.
            CAXM.AxmGantryGetEnable((int)enum_Sanding_ServoAxis.Sanding_Before_Z1, ref duSlaveHmUse, ref dSlaveHmOffset, ref dSlaveHmRange, ref duGantryOnBefore);

            lbBeforeGantryHmOffset.Text = Convert.ToString(dSlaveHmOffset);
            lbBeforeGantryHmRange.Text = Convert.ToString(dSlaveHmRange);
            lbBeforeGantryEnable.Text = Convert.ToString(duGantryOnBefore);

            //++ 지정한 축의 겐트리제어 관련 설정값을 확인합니다.
            CAXM.AxmGantryGetEnable((int)enum_Sanding_ServoAxis.Sanding_After_Z1, ref duSlaveHmUse, ref dSlaveHmOffset, ref dSlaveHmRange, ref duGantryOnAfter);

            lbAfterGantryHmOffset.Text = Convert.ToString(dSlaveHmOffset);
            lbAfterGantryHmRange.Text = Convert.ToString(dSlaveHmRange);
            lbAfterGantryEnable.Text = Convert.ToString(duGantryOnAfter);

            return true;
        }
        #endregion

        #endregion

        #endregion

        public void setLanguage() // 각 Form 클래스마다 각각 정의해서 화면 컨트롤 등에 대한 언어 적용. 이벤트성 메시지는 발생시 직접 읽어오도록
        {
            switch (GlobalVar.Language)
            {
                case enumLanguage.Korean:
                    break;
                case enumLanguage.English:
                default:
                    string path = "C:\\FA\\" + GlobalVar.SWName + "\\Language\\" + GlobalVar.Language.ToString() + ".ini";
                    //btnStart.Text = Util.ReadIniFile("frmMain", "btnStart.Text", path, "Start");
                    //btnInit.Text = Util.ReadIniFile("frmMain", "btnInit.Text", path, "Initialize");
                    break;
            }
        }

        #endregion

        #region 타이머 함수

        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            try
            {
                if (timerDoing)
                {
                    return;
                }
                timerDoing = true;

                /* 화면 변경 timer */
                this.Invoke(new MethodInvoker(delegate ()
                {

                    //tbError.Text = GlobalVar.SystemMsg;

                    #region 버튼 이미지 표시
                    //*


                    if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun &&
                    (int)GlobalVar.SystemStatus != (int)enumSystemStatus.ErrorStop)
                    {
                        pbStart.BackgroundImage = Radix.Properties.Resources.start_green;
                        if (GlobalVar.CycleStop)
                        {
                            pbStop.BackgroundImage = Radix.Properties.Resources.stop;
                            pbCycleStop.BackgroundImage = Radix.Properties.Resources.cycle_stop_red;
                        }
                        else
                        {
                            pbStop.BackgroundImage = Radix.Properties.Resources.stop;
                            pbCycleStop.BackgroundImage = Radix.Properties.Resources.cycle_stop;
                        }
                    }
                    else
                    {
                        pbStart.BackgroundImage = Radix.Properties.Resources.start;
                        pbStop.BackgroundImage = Radix.Properties.Resources.stop_red;
                        pbCycleStop.BackgroundImage = Radix.Properties.Resources.cycle_stop;
                    }
                    pbErrorLog.BackgroundImage = GlobalVar.SystemErrored || GlobalVar.E_Stop || GlobalVar.DoorOpen ?
                                                Radix.Properties.Resources.errors_red :
                                                Radix.Properties.Resources.errors;
                    pbReset.BackgroundImage = GlobalVar.SystemErrored || GlobalVar.E_Stop || GlobalVar.DoorOpen ?
                                                Radix.Properties.Resources.errors_reset_bright :
                                                Radix.Properties.Resources.errors_reset;
                    //GSCHOI-230613      샌딩기 지그에 임플란트 유무를 표시하는 이미지 구현==================================
                    pBsanding_jig_L.BackgroundImage = DIO.GetDIData(DIO_Sanding_enumDINames.X04_3_Sanding_Implant_0) ?
                                                     Radix.Properties.Resources.Lamp_Green :
                                                     Radix.Properties.Resources.Lamp_Gray;

                    pBsanding_jig_R.BackgroundImage = DIO.GetDIData(DIO_Sanding_enumDINames.X04_4_Sanding_Implant_1) ?
                                 Radix.Properties.Resources.Lamp_Green :
                                 Radix.Properties.Resources.Lamp_Gray;
                    //=======================================================================================================

                    pbBuzzerStop.BackgroundImage = !GlobalVar.EnableBuzzer || !GlobalVar.EnableBuzzer || !(GlobalVar.SystemErrored || GlobalVar.E_Stop || GlobalVar.DoorOpen) ?
                                                    Properties.Resources.buzzer_stop : Properties.Resources.buzzer_stop_bright;
                    //*/
                    #endregion


                    #region 출력에 따른 UI 변경 공간


                    //FuncForm.SetButtonColor2(btnBoxOutClamp, GlobalVar.HighGain_Magazine_Change_Tray_Out);


                    //FuncForm.SetLabelColor2(lbboxOutPush_st, (DIO.GetDOData(enumDONames.Y03_6_Box_Out_Push_Cylinder)));

                    //lbScanReceive.Text = GlobalVar.Load_Scanner;

                    if (DIO.Tower_Lamp_Buzzer_Check())
                    {
                        //lblDOCapFeederPower.Text = "ON";
                        //lblDOCapFeederPower.BackColor = Color.Green;
                    }
                    else
                    {
                        //lblDOCapFeederPower.Text = "OFF";
                        //lblDOCapFeederPower.BackColor = Color.Transparent;
                    }

                    #endregion

                    #region Tray Rack UI 표시
                    if (MakeArrayImage)//이미지가 다 만들어지고                        
                    {
                        if (MakeArrayImageCount > 2)//1초 이후 돌아라   (0.5초 타이머)
                        {

                            #region Tray Rack UI 변경 공간
                            #region Before Rack
                            int Count = 0;
                            for (int x = 0; x < FuncSanding.Sanding_BeforeTray_RackSize.GetLength(1); x++)
                            {
                                for (int z = 0; z < FuncSanding.Sanding_BeforeTray_RackSize.GetLength(0); z++)
                                {


                                    Before_TrayBtn[z, x].Name = "BeforeTray_Button" + z.ToString() + "_" + x.ToString();
                                   
                                    Before_TrayBtn[z, x].Text = $"{z+1}층{x+1}열";
                                    if (FuncSanding.Sanding_BeforeTray_RackNotUse[z,x] == false)
                                    {
                                        if (DIO.GetDIData((int)DIO_Sanding_enumDINames.X05_0_Before_Tray_Input_Check_Sensor_1 + Count))
                                        {
                                            FuncSanding.Sanding_BeforeTray_RackSize[z, x] = (int)enum_Sanding_Rack.In_Tray;
                                            //Before_TrayBtn[z, x].Visible = true;
                                            Before_TrayBtn[z, x].BackColor = Color.SkyBlue;
                                            if (DIO.GetDIData((int)DIO_Sanding_enumDINames.X05_0_Before_Tray_Input_Check_Sensor_1 + (Count + 1)))
                                            {
                                                FuncSanding.Sanding_BeforeTray_RackSize[z, x] = (int)enum_Sanding_Rack.In_Implant;
                                                //Before_TrayBtn[z, x].Visible = true;
                                                Before_TrayBtn[z, x].BackColor = Color.Lime;

                                            }
                                            //Before_TrayBtn[z, x].Text = $"{z}/{x}";  // Pos값 주석 처리 하고 나중에 표시 해야할 부분
                                        }
                                        else
                                        {
                                            FuncSanding.Sanding_BeforeTray_RackSize[z, x] = (int)enum_Sanding_Rack.No_Tray;
                                            //Before_TrayBtn[z, x].Visible = true;
                                            Before_TrayBtn[z, x].BackColor = Color.Gray;

                                            if (DIO.GetDIData((int)DIO_Sanding_enumDINames.X05_0_Before_Tray_Input_Check_Sensor_1 + (Count + 1)))
                                            {
                                                FuncSanding.Sanding_BeforeTray_RackSize[z, x] = (int)enum_Sanding_Rack.Error;
                                                //Before_TrayBtn[z, x].Visible = true;
                                                Before_TrayBtn[z, x].BackColor = Color.Tomato;
                                                //Before_TrayBtn[z, x].Text = $"SensorE";  // Pos값 주석 처리 하고 나중에 표시 해야할 부분
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //After_TrayBtn[z, x].Visible = true;
                                        Before_TrayBtn[z, x].BackColor = Color.White;
                                        Before_TrayBtn[z, x].Text = $"NotUse";
                                    }
                              
                                    Count += 2;

                                    #region 각 층별 POS값을 확인 하기위해 버튼에 표시(주석예정)
                                    //Before_TrayBtn[z, x].Font = new System.Drawing.Font("Calibri", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                                    //Before_TrayBtn[z, x].Text = $" Z{FuncSanding.Sanding_BeforeTrayRack_ZPOS[z, x]}Y{FuncSanding.Sanding_BeforeTrayRack_XPOS[z, x]}";
                                    #endregion 각 층별 POS값을 확인 하기위해 버튼에 표시(주석예정)

                                    //FuncLog.WriteLog($"{Before_TrayBtn[z, y].Name } : {DIO.GetDIData((int)DIO_Sanding_enumDINames.X05_0_Before_Tray_Input_Check_Sensor_1 + Count).ToString()}");

                                }
                            }

                            #endregion Before Rack

                            #region After Rack

                            Count = 0;
                            for (int x = 0; x < FuncSanding.Sanding_AfterTray_RackSize.GetLength(1); x++)
                            {
                                for (int z = 0; z < FuncSanding.Sanding_AfterTray_RackSize.GetLength(0); z++)
                                {

                                    After_TrayBtn[z, x].Name = "AfterTray_Button" + z.ToString() + "_" + x.ToString();

                                    After_TrayBtn[z, x].Text = $"{z + 1}층{x + 1}열";
                                    if (FuncSanding.Sanding_AfterTray_RackNotUse[z, x] == false)
                                    {
                                        #region After 트레이가 있으면 상태표시
                                        if (DIO.GetDIData((int)DIO_Sanding_enumDINames.X18_0_After_Tray_Input_Check_Sensor_1 + Count))
                                        {
                                            FuncSanding.Sanding_AfterTray_RackSize[z, x] = (int)enum_Sanding_Rack.In_Tray;
                                            //After_TrayBtn[z, x].Visible = true;
                                            After_TrayBtn[z, x].BackColor = Color.SkyBlue;

                                            if (DIO.GetDIData((int)DIO_Sanding_enumDINames.X18_0_After_Tray_Input_Check_Sensor_1 + (Count + 1)))
                                            {
                                                FuncSanding.Sanding_AfterTray_RackSize[z, x] = (int)enum_Sanding_Rack.In_Implant;
                                                //After_TrayBtn[z, x].Visible = true;
                                                After_TrayBtn[z, x].BackColor = Color.Lime;
                                            }
                                            //After_TrayBtn[z, y].Text = $"{z}/{y}";  // Pos값 주석 처리 하고 나중에 표시 해야할 부분
                                        }
                                        #endregion After 트레이가 있으면 상태표시
                                        #region After 트레이가 없으면 상태표시
                                        else
                                        {
                                            FuncSanding.Sanding_AfterTray_RackSize[z, x] = (int)enum_Sanding_Rack.No_Tray;
                                            //After_TrayBtn[z, x].Visible = true;
                                            After_TrayBtn[z, x].BackColor = Color.Gray;
                                            if (DIO.GetDIData((int)DIO_Sanding_enumDINames.X18_0_After_Tray_Input_Check_Sensor_1 + (Count + 1)))
                                            {
                                                FuncSanding.Sanding_AfterTray_RackSize[z, x] = (int)enum_Sanding_Rack.Error;
                                                //After_TrayBtn[z, x].Visible = true;
                                                After_TrayBtn[z, x].BackColor = Color.Tomato;
                                                // After_TrayBtn[z, x].Text = $"SensorE";  // Pos값 주석 처리 하고 나중에 표시 해야할 부분
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //After_TrayBtn[z, x].Visible = true;
                                        After_TrayBtn[z, x].BackColor = Color.White;
                                        After_TrayBtn[z, x].Text = $"NotUse";
                                    }
                                    Count += 2;

                                    #endregion After 트레이가 없으면 상태표시
                                    #region 각 층별 POS값을 확인 하기위해 버튼에 표시(주석예정)
                                    //After_TrayBtn[z, x].Font = new System.Drawing.Font("Calibri", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                                    //After_TrayBtn[z, x].Text = $" Z{FuncSanding.Sanding_AfterTrayRack_ZPOS[z, x]}Y{FuncSanding.Sanding_AfterTrayRack_XPOS[z, x]}";
                                    #endregion 각 층별 POS값을 확인 하기위해 버튼에 표시(주석예정)


                                    //FuncLog.WriteLog($"{After_TrayBtn[z, y].Name } : {DIO.GetDIData((int)DIO_Sanding_enumDINames.X18_0_After_Tray_Input_Check_Sensor_1 + Count).ToString()}");



                                }
                            }
                            #endregion After Rack

                            #endregion Tray Rack UI 변경 공간


                        }
                        else//설정 시간(?*10) 이후에는 추가 증가 하지 마라
                        {
                            MakeArrayImageCount++;
                        }

                    }
                    #endregion


                    #region Robot에 따른 UI 변경 공간

                    //와이파이 이미지에 대한 용도는  더 생각해 보자
                    if (FuncRobot.RobotInited())// &&
                                                //FuncRobot.RobotReady())                        
                    {
                        pB_Wifi.Visible = true;
                    }
                    else
                    {
                        pB_Wifi.Visible = false;
                    }

                    //DNet.GetDNetData((int)Kuka_enumDnetINames.IN_HOME))
                    if (FuncRobot.RobotReady_Kuka())
                    {

                        FuncSanding.KUKA_Center = true;
                    }
                    else
                    {
                        FuncSanding.KUKA_Center = false;
                    }

                    if (GlobalVar.RobotAction == enumRobotAction.OutBefore ||//샌딩 전 취출 후 샌딩기 공급
                       GlobalVar.RobotAction == enumRobotAction.InBefore)
                    {
                        //pBkukamove_Center.BackgroundImage = Radix.Properties.Resources.kuka_move_Left;
                        //pBkukamove_Center.Image = 
                    }
                    else if (GlobalVar.RobotAction == enumRobotAction.OutAfter)//샌딩기 배출 후 배출 트레이 공급
                    {
                        //pBkukamove_Center.BackgroundImage = Radix.Properties.Resources.kuka_move_Right;
                        //pBkukamove_Center.Image = ;
                    }
                    else//그외
                    {
                        //pBkukamove_Center.BackgroundImage = Radix.Properties.Resources.kuka_center;
                        pBkukamove_Center.Image = Radix.Properties.Resources.kuka_center;
                    }

                    //lbBOXOutposition.Text = (GlobalVar.AxisStatus[(int)enumServoAxis.Lift_BoxOutPut].Position / 1000).ToString();



                    #endregion

                    #region 시스템 운영상태 표시
                    //if (GlobalVar.DryRun)
                    //{
                    //    pbDryRun.Visible = true;
                    //    lblDryRun.Text = "Dry Run";
                    //}
                    //else
                    //{
                    //    pbDryRun.Visible = false;
                    //    lblDryRun.Text = "Auto Run";
                    //}
                    string runMode = GlobalVar.SystemStatus.ToString();
                    if (GlobalVar.AutoInline_PassMode)
                    {
                        runMode += " PassMode";
                    }
                    if (GlobalVar.AutoInline_SimulationMode)
                    {
                        runMode += " SimulationMode";
                    }
                    if (GlobalVar.DryRun)
                    {
                        runMode += " DryRun";
                    }
                    if (GlobalVar.CycleStop)
                    {
                        runMode += " CycleStop";
                    }
                    lblRunMode.Text = runMode;


                    lbError_code.Text = FuncSanding.MGG_SandingErrorCode;   //MGG - 에러코드 확인을 위해 추가 by DGKim
                    #endregion

                    #region 모델 확인
                    lbModelName.Text = GlobalVar.ModelName;
                    #endregion


                    #region 갠트리 서보 Slave축 관련 표시

                    lbBeforeMotionState.Text = GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_Before_Z2].StandStill.ToString();
                    lbBeforeZ1pos.Text = GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_Before_Z1].Position.ToString("F2");
                    lbBeforeZ2pos.Text = GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_Before_Z2].Position.ToString("F2");

                    lbAfterMotionState.Text = GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_After_Z2].StandStill.ToString();
                    lbAfterZ1pos.Text = GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_After_Z1].Position.ToString("F2");
                    lbAfterZ2pos.Text = GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_After_Z2].Position.ToString("F2");

                    #endregion
                    //GlobalVar.AxisStatus[0].StandStill = false;

                    //lblTackTime.Text = GlobalVar.TackTime.ToString("F1") + " sec";

                    FuncForm.SetButtonColor2(btnSmemaAfter, GlobalVar.TestPass);

                    lblRobotSpeed.Text = FuncSanding.RobotSpeed.ToString();

                    //lb_Lift_BF_In_Count.Text = FuncSanding.Before_Tray_In_Floor.ToString();
                    //lb_Lift_BF_Out_Count.Text = FuncSanding.Before_Tray_Out_Floor.ToString();
                    //lb_Lift_AF_In_Count.Text = FuncSanding.After_Tray_In_Floor.ToString();
                    //lb_Lift_AF_Out_Count.Text = FuncSanding.After_Tray_Out_Floor.ToString();

                    //GSCHOI-230614   Tray 공급 및 배출 수량을 표시해 준다.===========================
                    lb_Lift_BF_In_Count.Text = FuncSanding.Before_Tray_In_Count.ToString();
                    lb_Lift_BF_Out_Count.Text = FuncSanding.Before_Tray_Out_Count.ToString();
                    lb_Lift_AF_In_Count.Text = FuncSanding.After_Tray_In_Count.ToString();
                    lb_Lift_AF_Out_Count.Text = FuncSanding.After_Tray_Out_Count.ToString();
                    //================================================================================
                    if (FuncSanding.LineJump)
                    {
                        if (FuncSanding.Sanding_BeforeTrayColumnIndex % 2 == 1)
                        {
                            // numBeforeTrayIndex.Value
                            if (FuncSanding.Sanding_BeforeTrayCoulmnIndex_before < FuncSanding.Sanding_BeforeTrayColumnIndex)
                            {
                                FuncSanding.Sanding_BeforeTrayColumnIndex++;
                            }
                            else if(FuncSanding.Sanding_BeforeTrayCoulmnIndex_before > FuncSanding.Sanding_BeforeTrayColumnIndex)
                            {
                                FuncSanding.Sanding_BeforeTrayColumnIndex--;
                            }
                            else
                            {

                            }
                            FuncSanding.Sanding_BeforeTrayCoulmnIndex_before = FuncSanding.Sanding_BeforeTrayColumnIndex;
                        }
                        else
                        {

                        }
                    }

                    numBeforeTrayIndex.Value = (decimal)((FuncSanding.Sanding_BeforeTrayRowIndex * FuncSanding.Sanding_BeforeTraySize[1]) + FuncSanding.Sanding_BeforeTrayColumnIndex + 1);
                    //numAfterTrayIndex.Value = (decimal)((FuncSanding.Sanding_AfterTrayRowIndex * FuncSanding.Sanding_AfterTraySize[1]) + FuncSanding.Sanding_AfterTrayColumnIndex + 1);

                    int afterIndex = 0;
                    #region LineJump속성 True 일때 한줄간격으로 띄우고 값을 증가 시키기 위해
                    if (FuncSanding.LineJump && FuncSanding.Sanding_AfterTrayRowIndex % 2 == 1)
                    {
                            if (FuncSanding.Sanding_AfterTrayRowIndex <= 9 &&
                            FuncSanding.Sanding_AfterTrayRowIndex >= 3)
                            {
                                if (FuncSanding.Sanding_AfterTrayColumnIndex < 9)
                                {
                                    FuncSanding.Sanding_AfterTrayRowIndex++;
                                }
                                else if (FuncSanding.Sanding_AfterTrayColumnIndex >= 9)
                                {
                                    FuncSanding.Sanding_AfterTrayRowIndex--;
                                    if(FuncSanding.Sanding_AfterTrayRowIndex != 2)
                                    {
                                        FuncSanding.Sanding_AfterTrayColumnIndex = 9;
                                    }
                                    else
                                    {
                                        FuncSanding.Sanding_AfterTrayColumnIndex = 14;
                                    }
                                }
                            }
                            else
                            {
                                if(FuncSanding.Sanding_AfterTrayColumnIndex < 14 &&
                                    FuncSanding.Sanding_AfterTrayRowIndex < 12) 
                                {
                                    FuncSanding.Sanding_AfterTrayRowIndex++;
                                }
                                else if(FuncSanding.Sanding_AfterTrayColumnIndex >= 14)
                                {
                                    FuncSanding.Sanding_AfterTrayRowIndex--;
                                    FuncSanding.Sanding_AfterTrayColumnIndex = 14;
                                }
                            } 

                        if (FuncSanding.Sanding_AfterTrayRowIndex >= 12)
                        {
                            FuncSanding.Sanding_AfterTrayRowIndex = 11;
                            FuncSanding.Sanding_AfterTrayColumnIndex = 14;
                        }

                    }
                    #endregion

                    if (FuncSanding.Sanding_AfterTrayRowIndex > 8)
                    {
                        afterIndex = FuncSanding.Sanding_AfterTrayRowIndex * FuncSanding.Sanding_AfterTraySize[1] - 30;
                    }
                    else if (FuncSanding.Sanding_AfterTrayRowIndex > 2)
                    {
                        afterIndex = 3 * FuncSanding.Sanding_AfterTraySize[1] + (FuncSanding.Sanding_AfterTrayRowIndex - 3) * 10;
                    }
                    else
                    {
                        afterIndex = FuncSanding.Sanding_AfterTrayRowIndex * FuncSanding.Sanding_AfterTraySize[1];
                    }
                    numAfterTrayIndex.Value = afterIndex + FuncSanding.Sanding_AfterTrayColumnIndex + 1;

                    lbCheckServo.Text = GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_Before_Z2].Position.ToString();


                }));

                timerDoing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }
            finally
            {
                timerDoing = false;
            }

        }

        int tick = Environment.TickCount;
        private void tmrCheck_Tick(object sender, EventArgs e)
        {
            tmrCheck.Enabled = false;
            //debug("tmrCheck time : " + (Environment.TickCount - tick));
            int runningHour = (Environment.TickCount - runTime) / 1000 / 60 / 60;
            int runningMin = (Environment.TickCount - runTime - runningHour * 1000 * 60 * 60) / 1000 / 60;
            int runningSec = (Environment.TickCount - runTime - runningHour * 1000 * 60 * 60 - runningMin * 1000 * 60) / 1000;
            string runningStr = runningHour + " : " + runningMin + " : " + runningSec;

            int runSumSec = runTotal + (Environment.TickCount - runTime) / 1000;
            int runTotalHour = runSumSec / 60 / 60;
            int runTotalMin = (runSumSec - runTotalHour * 60 * 60) / 60;
            int runTotalSec = runSumSec - runTotalHour * 60 * 60 - runTotalMin * 60;
            string runTotalStr = runTotalHour + " : " + runTotalMin + " : " + runTotalSec;

            try
            {
                //debug("tmrCheck time 1 : " + (Environment.TickCount - tick));
                if (GlobalVar.CycleWatch.IsRunning ||
                    GlobalVar.CycleWatch.ElapsedMilliseconds == 0)
                {
                    lblCycleTime.Text = "-";
                }
                else
                {
                    lblCycleTime.Text = (GlobalVar.CycleWatch.ElapsedMilliseconds / 1000).ToString("F1") + " sec";
                }

                //Running Time
                if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun)
                {
                    lblRunningTime.Text = runningStr;
                    lblTotalTime.Text = runTotalStr;
                }
                else
                {
                    lblRunningTime.Text = "-";
                }

                //debug("tmrCheck time 2 : " + (Environment.TickCount - tick));
                #region OP
                if (!GlobalVar.E_Stop &&
                    DIO.EMG_Check())
                {
                    GlobalVar.E_Stop = true;



                    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                DateTime.Now.ToString("HH:mm:ss"),
                                                enumErrorPart.System,
                                                enumErrorCode.E_Stop,
                                                false,
                                                "Emergency Stop Button Pressed. Release Button and Initialize system."));
                    //#region Normal Error
                    //if (GlobalVar.UseNormalError)
                    //{
                    //    FuncError.AddError(enumError.E_Stop);
                    //}
                    //#endregion
                    //#region Part Error
                    //if (GlobalVar.PartError)
                    //{
                    //    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                    //                                DateTime.Now.ToString("HH:mm:ss"),
                    //                                enumErrorPart.System,
                    //                                enumErrorCode.E_Stop,
                    //                                false,
                    //                                ""));
                    //}
                    //#endregion                    
                }


                DIO.WriteDOData((int)DIO_Sanding_enumDONames.Y00_0_OP_Start_Lamp, GlobalVar.SystemStatus >= enumSystemStatus.AutoRun && GlobalVar.SystemStatus != enumSystemStatus.ErrorStop);
                DIO.WriteDOData((int)DIO_Sanding_enumDONames.Y00_1_OP_Stop_Lamp, GlobalVar.SystemStatus < enumSystemStatus.AutoRun || GlobalVar.SystemStatus == enumSystemStatus.ErrorStop);
                DIO.WriteDOData((int)DIO_Sanding_enumDONames.Y00_2_OP_Reset_Lamp, GlobalVar.SystemStatus == enumSystemStatus.ErrorStop ||
                                                                                        Warning ||
                                                                                        GlobalVar.DoorOpen ||
                                                                                        GlobalVar.E_Stop);
                if (DIO.GetDIChange(DIO_Sanding_enumDINames.X00_2_OP_Reset) && DIO.GetDIData(DIO_Sanding_enumDINames.X00_2_OP_Reset) &&
                    (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop ||
                                                                                        Warning ||
                                                                                        GlobalVar.DoorOpen ||
                                                                                        GlobalVar.E_Stop))
                {
                    GlobalVar.EnableBuzzer = false;
                }

                if (DIO.GetDIChange(DIO_Sanding_enumDINames.X00_0_OP_Start) && DIO.GetDIData(DIO_Sanding_enumDINames.X00_0_OP_Start))
                {
                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun ||
                        GlobalVar.SystemErrored ||
                        GlobalVar.E_Stop ||
                        GlobalVar.DoorOpen)
                    {

                    }
                    else if (tcMain.SelectedIndex != (int)enumTabMain.Auto)
                    {
                        FuncWin.TopMessageBox("Change to Auto Window first");

                    }
                    else
                    {
                        Start_Button(true);
                    }
                }

                if (DIO.GetDIChange(DIO_Sanding_enumDINames.X00_1_OP_Stop) && DIO.GetDIData(DIO_Sanding_enumDINames.X00_1_OP_Stop))
                {
                    if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
                    {

                    }
                    else
                    {
                        Stop_Button();
                    }
                }


                #endregion
                //debug("tmrCheck time 3: " + (Environment.TickCount - tick));
                #region CycleStop_End
                if (GlobalVar.CycleStop_End)
                {
                    GlobalVar.CycleStop_End = false;
                    FuncLog.WriteLog("사이클 스탑을 누르고 작업이 종료 되었다.");
                    Stop_Button();
                }
                #endregion
                //debug("tmrCheck time 4 : " + (Environment.TickCount - tick));
                #region Tower Lamp

                #region Green
                if (!GlobalVar.EnableTower)
                {
                    DIO.Tower_Lamp_Green_Control(false);
                }
                else if (GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Green, (int)enumTowerLampAction.Enable] &&
                    GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Green, (int)enumTowerLampAction.Blink] &&
                    Environment.TickCount - GlobalVar.TowerTick > 1000) // 점멸
                {
                    //DIO.WriteDOData(enumDONames.Y02_2_TOWER_LAMP_GREEN, !DIO.GetDORead(enumDONames.Y02_2_TOWER_LAMP_GREEN));
                    DIO.Tower_Lamp_Green_Control(DIO.Tower_Lamp_Green_Check());
                }
                else
                {
                    //DIO.WriteDOData(enumDONames.Y02_2_TOWER_LAMP_GREEN, GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, 2, 0]);
                    DIO.Tower_Lamp_Green_Control(GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Green, (int)enumTowerLampAction.Enable]);
                }
                #endregion
                #region Yellow
                if (!GlobalVar.EnableTower)
                {
                    DIO.Tower_Lamp_Green_Control(false);
                }
                else if (GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Yellow, (int)enumTowerLampAction.Enable] &&
                    GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Yellow, (int)enumTowerLampAction.Blink] &&
                    Environment.TickCount - GlobalVar.TowerTick > 1000) // 점멸
                {
                    //DIO.WriteDOData(enumDONames.Y02_1_TOWER_LAMP_YELLOW, !DIO.GetDORead(enumDONames.Y02_1_TOWER_LAMP_YELLOW));
                    DIO.Tower_Lamp_Yellow_Control(DIO.Tower_Lamp_Yellow_Check());
                }
                else
                {
                    //DIO.WriteDOData(enumDONames.Y02_1_TOWER_LAMP_YELLOW, GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, 1, 0]);
                    DIO.Tower_Lamp_Yellow_Control(GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Yellow, (int)enumTowerLampAction.Enable]);
                }
                #endregion
                #region Red
                if (!GlobalVar.EnableTower)
                {
                    DIO.Tower_Lamp_Green_Control(false);
                }
                else if (GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Red, (int)enumTowerLampAction.Enable] &&
                    GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Red, (int)enumTowerLampAction.Blink] &&
                    Environment.TickCount - GlobalVar.TowerTick > 1000) // 점멸
                {
                    //DIO.WriteDOData(enumDONames.Y02_0_TOWER_LAMP_RED, !DIO.GetDORead(enumDONames.Y02_0_TOWER_LAMP_RED));
                    DIO.Tower_Lamp_Red_Control(DIO.Tower_Lamp_Red_Check());
                }
                else
                {
                    //DIO.WriteDOData(enumDONames.Y02_0_TOWER_LAMP_RED, GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, 0, 0]);
                    DIO.Tower_Lamp_Red_Control(GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Red, (int)enumTowerLampAction.Enable]);
                }
                #endregion
                if (Environment.TickCount - GlobalVar.TowerTick > 1000)
                {
                    GlobalVar.TowerTick = Environment.TickCount;
                }
                #endregion
                //debug("tmrCheck time 5 : " + (Environment.TickCount - tick));
                #region buzzer run/stop 등 운영상황에 관련된 것만 일괄 처리하고, 나머지 오퍼레이터 콜 등은 ErrorDialog에서 처리
                //if (GlobalVar.E_Stop || GlobalVar.DoorOpen || Warning)
                if (!GlobalVar.EnableTower)
                {
                    DIO.Tower_Lamp_Buzzer_Control(false);
                }
                else if (Warning)
                {
                    DIO.Tower_Lamp_Buzzer_Control(GlobalVar.EnableBuzzer);   //202209 부져 상황별 정리 필요 by DG
                }
                else
                {
                    DIO.Tower_Lamp_Buzzer_Control(GlobalVar.EnableBuzzer && GlobalVar.TowerAction[(int)GlobalVar.SystemStatus, (int)enumTowerLamp.Buzzer, (int)enumTowerLampAction.Enable]);
                }


                #endregion
                //debug("tmrCheck time 6: " + (Environment.TickCount - tick));
                #region Door Check
                //*
                if (GlobalVar.UseDoor)
                {
                    if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                    GlobalVar.SystemStatus != enumSystemStatus.ErrorStop &&
                    (DIO.Door_Check()))
                    {
                        GlobalVar.SystemStatus = enumSystemStatus.ErrorStop;
                        FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                              DateTime.Now.ToString("HH:mm:ss"),
                                              enumErrorPart.System,
                                              enumErrorCode.Door_Opened,
                                              false,
                                              "Door opened while system is running. Close door and try again."));
                        //#region Normal Error
                        //if (GlobalVar.UseNormalError)
                        //{
                        //    FuncError.AddError(enumError.Door_Opened);
                        //}
                        //#endregion
                        //#region Part Error
                        //if (GlobalVar.PartError)
                        //{
                        //    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                        //                         DateTime.Now.ToString("HH:mm:ss"),
                        //                         enumErrorPart.System,
                        //                         enumErrorCode.Door_Opened,
                        //                         false,
                        //                         ""));
                        //}
                        //#endregion                     
                    }
                }
                //*/
                #endregion
                //debug("tmrCheck time 7 : " + (Environment.TickCount - tick));
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
                #endregion
                //debug("tmrCheck time 8 : " + (Environment.TickCount - tick));
                #region 조명 컨트롤 B접점
                //DIO.WriteDOData(enumDONames.Y00_4_LED_Lamp1, GlobalVar.SystemStatus >= enumSystemStatus.AutoRun);
                #endregion
                //debug("tmrCheck time 9 : " + (Environment.TickCount - tick));
                #region 상태표시
                #endregion
                //debug("tmrCheck time 10 : " + (Environment.TickCount - tick));
                #region Timeout
                //if (GlobalVar.SystemStatus == enumSystemStatus.AutoRun)
                //{

                //    if (Environment.TickCount - GlobalVar.OutputTime > GlobalVar.OutStopTime * 60 * 1000)
                //    {
                //        GlobalVar.SystemStatus = enumSystemStatus.OutputStop;
                //    }
                //    else if (Environment.TickCount - GlobalVar.InputTime > GlobalVar.InStopTime * 60 * 1000)
                //    {
                //        GlobalVar.SystemStatus = enumSystemStatus.InputStop;
                //    }
                //    //*
                //    else if (GlobalVar.SystemStatus != enumSystemStatus.ErrorStop)
                //    {
                //        GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
                //        //GlobalVar.OutputTime = Environment.TickCount;
                //        //GlobalVar.InputTime = Environment.TickCount;
                //    }
                //    //*/
                //    /*
                //  else if (GlobalVar.SystemStatus != enumSystemStatus.InputStop &&
                //      GlobalVar.SystemStatus != enumSystemStatus.OutputStop)
                //  {
                //      //GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
                //      GlobalVar.OutputTime = Environment.TickCount;
                //      GlobalVar.InputTime = Environment.TickCount;
                //  }
                //  //*/

                //}
                #endregion
                //debug("tmrCheck time 11 : " + (Environment.TickCount - tick));
                #region 에러창
                if (GlobalVar.SystemErrorQueue.Count > 0)
                {
                    //if (GlobalVar.NormalError)
                    //{
                    //    structError er = new structError(DateTime.Now.ToString("yyyyMMdd"),
                    //                                    DateTime.Now.ToString("HH:mm:ss"),
                    //                                    enumErrorPart.No_Error,
                    //                                    enumErrorCode.No_Error,
                    //                                    false,
                    //                                    "");
                    //    GlobalVar.SystemErrorQueue.TryDequeue(out er);
                    //    ErrorDialog dlg = new ErrorDialog(er);
                    //    dlg.Owner = this;
                    //    dlg.Show();
                    //}
                    //if (GlobalVar.PartError)
                    //{
                    structError er = new structError(DateTime.Now.ToString("yyyyMMdd"),
                                                    DateTime.Now.ToString("HH:mm:ss"),
                                                    enumErrorPart.No_Error,
                                                    enumErrorCode.No_Error,
                                                    false,
                                                    "");
                    GlobalVar.SystemErrorQueue.TryDequeue(out er);
                    if (er.ErrorCode != enumErrorCode.No_Error)
                    {
                        #region 에러창 떠 있으면 닫고 다른 창을 연다
                        try
                        {
                            Form fc = Application.OpenForms["ErrorDialog"];
                            if (fc != null)
                            {
                                fc.Close();
                            }
                        }
                        catch (Exception xx)
                        {
                            debug(xx.ToString());
                            debug(xx.StackTrace);
                        }
                        #endregion

                        errorDialog = new ErrorDialog(er);
                        errorDialog.Owner = this;
                        errorDialog.Show();
                    }
                    //}
                }
                #endregion

                #region 메인 페이지 변경
                if ((int)GlobalVar.TabMain != tcMain.SelectedIndex)
                {
                    tcMain.SelectedIndex = (int)GlobalVar.TabMain;
                }
                #endregion
                if (!GlobalVar.dlgOpened)
                {
                    if (Environment.TickCount - checkTime > 5000)
                    {
                        checkTime = Environment.TickCount;
                    }
                }

                //debug("tmrCheck time 12 : " + (Environment.TickCount - tick));
                tick = Environment.TickCount;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            //DIO.WriteDOData(enumDONames.Y04_1_Cleaner_Run, GlobalVar.SystemStatus == enumSystemStatus.AutoRun);
            tmrCheck.Enabled = true;
        }

        private int beforeRowIndex = 0;
        private int beforeColumnIndex = 0;
        private int afterRowIndex = 0;
        private int afterColumnIndex = 0;
        private void TimerTrayUI(Object state) // 화면 제어 쓰레드 Tray UI 타이머 함수
        {
            try
            {
                if (timerDoing_TrayUI)
                {
                    return;
                }
                timerDoing_TrayUI = true;

                /* 화면 변경 timer */
                this.Invoke(new MethodInvoker(delegate ()
                {
                    //로드할때 적용 전값과 적용 후 값이 다를때 한번만 컨트롤 다시 생성
                    if (FuncSanding.SettingChange)
                    {
                        //MakeArrayImage = false;

                        if (FuncSanding.Sanding_Before_Tray_LR_Pickup == true)
                        {
                            //BeforeTray L->R    
                            Make_Controls<PictureBox>(ref Before_PictureBox, "Before", FuncSanding.Sanding_BeforeTraySize[0], FuncSanding.Sanding_BeforeTraySize[1], 18, 18, 38, 17, 62, 18);
                            lbLRcheck.Text = "L  -- > R";
                        }
                        else
                        {
                            //BeforeTray R->L
                            Make_Controls<PictureBox>(ref Before_PictureBox, "Before", FuncSanding.Sanding_BeforeTraySize[0], FuncSanding.Sanding_BeforeTraySize[1], 18, 18, 191, -17, 62, 18);
                            lbLRcheck.Text = "L  < --  R";
                        }
                        FuncSanding.SettingChange = false;
                    }

                    if (MakeArrayImage)//이미지가 다 만들어지고                        
                    {
                        if (MakeArrayImageCount > 2)//1초 이후 돌아라   (0.5초 타이머)
                        {
                            if (beforeRowIndex != FuncSanding.Sanding_BeforeTrayRowIndex ||
                                beforeColumnIndex != FuncSanding.Sanding_BeforeTrayColumnIndex)
                            {
                                #region Before Tray UI
                                for (int i = 0; i < FuncSanding.Sanding_BeforeTraySize[0]; i++)//15
                                {
                                    for (int j = 0; j < FuncSanding.Sanding_BeforeTraySize[1]; j++)//10
                                    {
                                        try
                                        {
                                            if ((FuncSanding.Sanding_BeforeTrayRowIndex < i ||
                                                    (FuncSanding.Sanding_BeforeTrayRowIndex == i &&
                                                        FuncSanding.Sanding_BeforeTrayColumnIndex <= j)) &&
                                                 i * 10 + j < FuncSanding.Sanding_BeforeTrayMax)
                                            {
                                                Before_PictureBox[i, j].Visible = true;

                                                if (FuncSanding.LineJump)
                                                {
                                                    if(j % 2 == 1 && FuncSanding.Megagen)
                                                    {
                                                        Before_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Gray_1;
                                                    }
                                                    else
                                                    {
                                                        if(i== 14 && j == 8 && FuncSanding.Megagen)
                                                        {
                                                            Before_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Gray_1;
                                                        }
                                                        else
                                                        {
                                                            Before_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Green_1;
                                                        } 
                                                    } 
                                                }
                                                else
                                                {
                                                    Before_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Green_1;
                                                }
                                               
                                            }
                                            else
                                            {
                                                //Before_PictureBox[i, j].Visible = false;
                                                Before_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Gray_1;
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            debug(ex.ToString());
                                        }
                                    }
                                }
                                #endregion
                            }

                            if (afterRowIndex != FuncSanding.Sanding_BeforeTrayRowIndex ||
                                afterColumnIndex != FuncSanding.Sanding_BeforeTrayColumnIndex)
                            {
                                #region After Tray UI
                                for (int i = 0; i < FuncSanding.Sanding_AfterTraySize[0]; i++)
                                {
                                    for (int j = 0; j < FuncSanding.Sanding_AfterTraySize[1]; j++)
                                    {
                                        try
                                        {
                                            int cnt = i * 15 + j;

                                            if (FuncSanding.Megagen)
                                            {
                                                if (i > 8)
                                                {
                                                    cnt = 15 * 3 + 10 * 6 + (i - 9) * 15 + j;
                                                }
                                                else if (i >= 3)
                                                {
                                                    cnt = 15 * 3 + (i - 3) * 10 + j;
                                                }
                                            }

                                            if ((FuncSanding.Sanding_AfterTrayRowIndex < i ||
                                                (FuncSanding.Sanding_AfterTrayRowIndex == i &&
                                                    FuncSanding.Sanding_AfterTrayColumnIndex <= j)) &&
                                                cnt < FuncSanding.Sanding_AfterTrayMax)
                                            {
                                                if (i >= 3 && i <= 8 &&
                                                   j >= 5 && j <= 9)
                                                {
                                                    After_PictureBox[i, j].Visible = false;
                                                    After_PictureBox[i, j + 5].Image = Radix.Properties.Resources.Lamp_Gray_1;
                                                    continue;
                                                }
                                                else
                                                {
                                                    if (i >= 3 && i <= 8 &&
                                                   j >= 5 + 5 && j <= 9 + 5)
                                                    {
                                                        //pass                                                
                                                    }
                                                    else
                                                    {
                                                            After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Gray_1;     
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (i >= 3 && i <= 8 &&
                                              j >= 5 && j <= 9)
                                                {
                                                    j = j + 5;
                                                    After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Green_1;
                                                    j = j - 5;
                                                }
                                                else
                                                {
                                                    if (FuncSanding.LineJump)
                                                    {
                                                        if (i % 2 == 1 && FuncSanding.Megagen)
                                                        {

                                                           
                                                              After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Gray_1;
                                                        }
                                                        else
                                                        {
                                                            if (j == 14 && i == 10 && FuncSanding.Megagen)
                                                            {
                                                                After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Gray_1;
                                                            }
                                                            else
                                                            {
                                                                After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Green_1;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Green_1;
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            debug(ex.ToString());
                                        }




                                        /* // 기본로직
                                            if ((FuncSanding.Sanding_AfterTrayRowIndex < i ||
                                                (FuncSanding.Sanding_AfterTrayRowIndex == i &&
                                                    FuncSanding.Sanding_AfterTrayColumnIndex <= j)) &&
                                                i * 10 + j < FuncSanding.Sanding_AfterTrayMax)
                                            {

                                                After_PictureBox[i, j].Visible = true;
                                                After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Green_1;
                                            }


                                            else
                                            {
                                                After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Gray_1;
                                            }
                                        //*/

                                        /* // 메가젠 속 빈 모양 환산 로직
                                        int cnt = i * 15 + j;

                                        if (!FuncSanding.Neo_Biotech)
                                        {
                                            if (i >= 3 && i <= 8 &&
                                            j >= 10)
                                            {
                                                continue;
                                            }
                                            if (i > 8)
                                            {
                                                cnt = 15 * 3 + 10 * 6 + (i - 9) * 15 + j;
                                            }
                                            else if (i >= 3)
                                            {
                                                cnt = 15 * 3 + (i - 3) * 10 + j;
                                            }
                                        }

                                        if ((FuncSanding.Sanding_AfterTrayRowIndex < i ||
                                                (FuncSanding.Sanding_AfterTrayRowIndex == i &&
                                                    FuncSanding.Sanding_AfterTrayColumnIndex <= j)) ||
                                            cnt >= FuncSanding.Sanding_AfterTrayMax)
                                        {
                                            //Controls.Find("pbAfterTray_" + i + "_" + j, true)[0].Visible = false;
                                            After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Gray;
                                        }
                                        else
                                        {
                                            //Controls.Find("pbAfterTray_" + i + "_" + j, true)[0].Visible = true;
                                            After_PictureBox[i, j].Image = Radix.Properties.Resources.Lamp_Green;
                                        }
                                        //*/
                                    }
                                }
                                #endregion
                            }

                            beforeRowIndex = FuncSanding.Sanding_BeforeTrayRowIndex;
                            beforeColumnIndex = FuncSanding.Sanding_BeforeTrayColumnIndex;
                            afterRowIndex = FuncSanding.Sanding_AfterTrayRowIndex;
                            afterColumnIndex = FuncSanding.Sanding_AfterTrayColumnIndex;




                        }
                        else//설정 시간(?*10) 이후에는 추가 증가 하지 마라
                        {
                            MakeArrayImageCount++;
                        }

                    }
                }));

                timerDoing_TrayUI = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                //debug(ex.StackTrace);
            }
            finally
            {
                timerDoing_TrayUI = false;
            }

        }
        #endregion

        #region 버튼 등 공통 UI 컨트롤 이벤트

        #region Main Tap 관련
        private void pbAuto_Click(object sender, EventArgs e)//Main
        {
            ClearSubMenu();
            pbAuto.BackgroundImage = Radix.Properties.Resources.sub_main_sel;
            GlobalVar.TabMain = enumTabMain.Auto;
            tcMain.SelectedIndex = (int)GlobalVar.TabMain;


            //모델 변경후 Main누를때 체크하고 모델명 변경하기 위해 by DG 220913
            //MXN.MXN_GetModelInfo(GlobalVar.ModelPath_MXN, GlobalVar.ModelName_MXN);
            //LabelModelName.Text = GlobalVar.ModelName_MXN.ToString();
        }
        private void pbManual_Click(object sender, EventArgs e)
        {
            if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            ClearSubMenu();
            pbManual.BackgroundImage = Radix.Properties.Resources.sub_manual_sel;
            GlobalVar.TabMain = enumTabMain.Manual;
            tcMain.SelectedIndex = (int)GlobalVar.TabMain;
        }
        private void pbIOMonitor_Click(object sender, EventArgs e)
        {
            ClearSubMenu();
            pbIOMonitor.BackgroundImage = Radix.Properties.Resources.sub_io_sel;
            GlobalVar.TabMain = enumTabMain.IO;
            tcMain.SelectedIndex = (int)GlobalVar.TabMain;
        }
        private void pbTower_Click(object sender, EventArgs e)//System
        {

            if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            GlobalVar.PwdPass = false;
            Password dlg = new Password();
            dlg.ShowDialog();
            if (GlobalVar.PwdPass)
            {

                ClearSubMenu();
                pbTower.BackgroundImage = Radix.Properties.Resources.sub_system_sel;
                GlobalVar.TabMain = enumTabMain.Machine;
                tcMain.SelectedIndex = (int)GlobalVar.TabMain;

                frmMachine.LoadAllValue();
            }


        }
        private void pbModel_Click(object sender, EventArgs e)
        {
            if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            ClearSubMenu();
            pbModel.BackgroundImage = Radix.Properties.Resources.sub_model_sel;

            GlobalVar.TabMain = enumTabMain.Model;
            tcMain.SelectedIndex = (int)GlobalVar.TabMain;

            frmModel.LoadAllValue();
        }
        private void pbTeaching_Click(object sender, EventArgs e)
        {
            if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            ClearSubMenu();
            pbTeaching.BackgroundImage = Radix.Properties.Resources.sub_teaching_sel;
            GlobalVar.TabMain = enumTabMain.Teaching;
            tcMain.SelectedIndex = (int)GlobalVar.TabMain;

            frmTeaching.LoadAllValue();
        }
        private void pbTrace_Click(object sender, EventArgs e)
        {
            ClearSubMenu();
            pbTrace.BackgroundImage = Radix.Properties.Resources.sub_trace_sel;
            GlobalVar.TabMain = enumTabMain.Trace;
            tcMain.SelectedIndex = (int)GlobalVar.TabMain;
        }
        #region Router Program 창 활성화
        //이미 실행중이면, 포커스
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        //이미 실행중이면 보이게
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        //이미 실행중이면, 맨 앞으로
        [DllImport("user32.dll")]
        private static extern void BringWindowToTop(IntPtr hwnd);
        private void pbRouter_Click(object sender, EventArgs e)
        {
            ClearSubMenu();
            pbRouter.BackgroundImage = Radix.Properties.Resources.sub_Router_sel;

            GlobalVar.TabMain = enumTabMain.Router;
            tcMain.SelectedIndex = (int)GlobalVar.TabMain;

            String RouterProgram = "MXN_HMI.exe";

            if (FuncWin.FindWindowByExname(RouterProgram) == IntPtr.Zero)
            {
                foreach (Process process in Process.GetProcesses())
                {

                    if (process.ProcessName == RouterProgram.Replace(".exe", ""))
                    {
                        ShowWindow(process.MainWindowHandle, 5);
                        BringWindowToTop(process.MainWindowHandle);
                        SetForegroundWindow(process.MainWindowHandle);
                    }
                }
            }
        }
        #endregion
        private void ClearSubMenu()
        {
            pbAuto.BackgroundImage = Radix.Properties.Resources.sub_main;
            pbManual.BackgroundImage = Radix.Properties.Resources.sub_manual;
            pbIOMonitor.BackgroundImage = Radix.Properties.Resources.sub_io;
            pbTower.BackgroundImage = Radix.Properties.Resources.sub_system;
            pbModel.BackgroundImage = Radix.Properties.Resources.sub_model;
            pbTeaching.BackgroundImage = Radix.Properties.Resources.sub_teaching;
            pbTrace.BackgroundImage = Radix.Properties.Resources.sub_trace;
            pbRouter.BackgroundImage = Radix.Properties.Resources.sub_Router;
            pbPartClear.BackgroundImage = Properties.Resources.part_clear;

        }
        #endregion

        #region 우측 버튼 관련

        private void pbStart_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Main - Start Click ");
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                return;
            }
            if (tcMain.SelectedIndex != (int)enumTabMain.Auto)
            {
                FuncWin.TopMessageBox("Change to Auto Window first");
                return;
            }

            if (FuncWin.MessageBoxOK("Start operation?"))
            {
                Start_Button(false);
            }
        }
        private void pbStop_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Main - Stop Click ");
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
            {
                return;
            }

            Stop_Button();
        }
        private void pbCycleStop_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Main - Cycle Stop Click ");
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
            {
                return;
            }
            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                !GlobalVar.CycleStop &&
                FuncWin.MessageBoxOK("Cycle Stop operation?"))
            {
                GlobalVar.CycleStop = true;
                pbCycleStop.BackgroundImage = Properties.Resources.cycle_stop_red;
            }
        }
        private void pbInit_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Main - Init Click ");

            if (GlobalVar.TabMain != enumTabMain.Auto)
            {
                FuncWin.TopMessageBox("Main화면에서 선택 해주세요");
                return;
            }
            #region Door 관련 확인
            //*/             

            //GSCHOI-230616   Use Door 사용을 위해 추가 함===============================================================
            if (FuncIni.ReadIniFile(GlobalVar.seting_Section, "UseDoor", GlobalVar.seting_IniPath, "0") == "true")
            {
                GlobalVar.UseDoor = true;
            }
            else if (FuncIni.ReadIniFile(GlobalVar.seting_Section, "UseDoor", GlobalVar.seting_IniPath, "0") == "false")
            {
                GlobalVar.UseDoor = false;
            }
            //===========================================================================================================

            if (GlobalVar.UseDoor)
            {
                if (DIO.Door_Check())
                {
                    FuncWin.TopMessageBox("Can't Initialize while doors are opened.");
                    return;
                }
            }
            //*/
            #endregion

            #region Status 관련 확인
            if ((GlobalVar.SystemStatus >= enumSystemStatus.AutoRun) ||
                (GlobalVar.SystemStatus == enumSystemStatus.Initialize))
            {
                FuncWin.TopMessageBox("Can't Initialize while machine is running or initializing.");
                return;
            }
            #endregion

            #region 제품 잔존 여부 확인
            if (DIO.Product_Residual_Check())
            {
                return;
            }
            #endregion

            #region 초기화 진행
            if (FuncWin.MessageBoxOK("Initialize facility?"))
            {
                //Func.WriteLog("Init Click");
                pbInit.BackgroundImage = Properties.Resources.initialize_bright;

                Loading dlgLoading = new Loading();
                dlgLoading.TopMost = true;  //Init창 최상위로 유지 시킨다 by DG
                dlgLoading.Show();

                #region 전역 변수 초기화
                FuncSanding.Init_Variable_Before_Tray_Finish = false;
                FuncSanding.Init_Variable_After_Tray_Finish = false;



                #endregion

                #region 지역 변수 초기화
                GlobalVar.Init_Finish = false;
                GlobalVar.Init_Variable = true;//해당 변수(전역,지역)를 통해 각 쓰레드 초기화 시작                
                #endregion

                GlobalVar.SystemStatus = enumSystemStatus.Initialize;
                int startTime = Environment.TickCount;

                while (Environment.TickCount - startTime < 2 * 60 * 1000 &&
                    GlobalVar.SystemStatus == enumSystemStatus.Initialize) // 2분
                {
                    if (GlobalVar.Simulation)
                    {
                        GlobalVar.Init_Finish = true;
                    }

                    //로봇 부터 해야 될지 변수실린더서보로봇 순으로 해야할지 잘 모르겠음.
                    if (GlobalVar.RobotUse)
                    {
                        if (FuncRobot.RobotInited())
                        {
                            //초기화가 완료 된 상태면 그냥 넘어감.
                            //GlobalVar.Init_Robot = true; // YJ20230428 로봇체크 안 되어 진행 안 되어 추가함.
                        }
                        else if (FuncRobot.RobotInit())
                        {
                            Thread.Sleep(100);
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    //쓰레드에서 초기화 경우를 만들어서 진행
                    #region 초기화 성공  // 실린더, 서보, 로봇 각가의 글로벌 변수 트루 확인
                    #region 각 쓰레드 변수 초기화 완료
                    if (GlobalVar.Init_Variable &&
                        DIO.Check_Init_Variable())
                    {
                        GlobalVar.Init_Variable = false;
                        GlobalVar.Init_Cylinder = true;
                    }
                    #endregion
                    #region 각 쓰레드 실린더 초기화 완료
                    if (GlobalVar.Init_Cylinder &&
                        DIO.Check_Init_Cylinder())
                    {
                        GlobalVar.Init_Cylinder = false;
                        GlobalVar.Init_Servo = true;
                    }
                    #endregion
                    #region 각 쓰레드 서보 초기화 완료
                    if (GlobalVar.Init_Servo &&
                        DIO.Check_Init_Servo())
                    {
                        GlobalVar.Init_Servo = false;
                        GlobalVar.Init_Robot = true;
                    }
                    #endregion
                    #region 각 쓰레드 로봇 초기화 완료
                    if (GlobalVar.Init_Robot &&
                        DIO.Check_Init_Robot())
                    {
                        GlobalVar.Init_Robot = false;
                        GlobalVar.Init_Finish = true;
                    }
                    #endregion     
                    if (GlobalVar.Init_Finish)
                    {
                        if (GlobalVar.Simulation)
                        {
                            dlgLoading.Close();

                            FuncWin.TopMessageBox("Initialize finished.");
                            GlobalVar.SystemStatus = enumSystemStatus.Manual;

                            pbInit.BackgroundImage = Properties.Resources.initialize;

                            return;
                        }
                        //서보들 추가 이동이 필요할 경우 이동 완료 확인
                        if (GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_Before_Z1].StandStill &&
                            GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_Before_X].StandStill &&
                            GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_After_Z1].StandStill &&
                            GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_After_X].StandStill)
                        {
                            dlgLoading.Close();

                            FuncWin.TopMessageBox("Initialize finished.");
                            GlobalVar.SystemStatus = enumSystemStatus.Manual;

                            pbInit.BackgroundImage = Properties.Resources.initialize;

                            return;
                        }
                    }
                    #endregion

                    Application.DoEvents();
                    Thread.Sleep(100);
                }

                #region 초기화 실패 사유 조합
                string stat = "";

                if (!GlobalVar.Init_Robot)
                {
                    stat += "\n Robot Check";
                }
                if (!GlobalVar.Init_Servo)
                {
                    stat += "\n Servo Check";
                }
                if (!GlobalVar.Init_Cylinder)
                {
                    stat += "\n Cylinder Check";
                }
                #endregion

                GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;

                pbInit.BackgroundImage = Properties.Resources.initialize;
                dlgLoading.Close();
                FuncWin.TopMessageBox("initialize failed." + stat);

            }
            #endregion
            /*
            if (GlobalVar.dlgOpened)
            {
                return;
            }
            Init dlg = new Init();
            dlg.Show();
            //*/
        }
        private void pbPartClear_Click(object sender, EventArgs e)
        {
            if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            ClearSubMenu();
            GlobalVar.TabMain = enumTabMain.PartClear;
            tcMain.SelectedIndex = (int)GlobalVar.TabMain;
            pbPartClear.BackgroundImage = Properties.Resources.part_clear_bright;
        }
        private void pbErrorLog_Click(object sender, EventArgs e)
        {
            ClearSubMenu();
            GlobalVar.TabMain = enumTabMain.Errors;
            tcMain.SelectedIndex = (int)GlobalVar.TabMain;
        }
        private void pbBuzzerStop_Click(object sender, EventArgs e)
        {
            if (!GlobalVar.EnableTower ||
                !GlobalVar.EnableBuzzer ||
                !(GlobalVar.SystemErrored || GlobalVar.E_Stop || GlobalVar.DoorOpen))
            {
                return;
            }
            //string msg = (GlobalVar.EnableBuzzer ? "Disable" : "Enable") + " Buzzer?";
            //if (FuncWin.MessageBoxOK(msg))
            //{
            GlobalVar.EnableBuzzer = !GlobalVar.EnableBuzzer;
            //pbBuzzerStop.BackgroundImage = GlobalVar.EnableBuzzer ? Properties.Resources.buzzer_stop : Properties.Resources.buzzer_stop_bright;
            //}
        }
        private void pbDailyCountReset_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (FuncWin.MessageBoxOK("Reset production count?"))
            {
                //GlobalVar.ProductCanCount = 0;
                //GlobalVar.ProductBoxCount = 0;
                runTime = Environment.TickCount;
                runTotal = 0;
                GlobalVar.TackStart = Environment.TickCount;

                lblRunningTime.Text = "-";
                lblTotalTime.Text = "-";
            }
        }
        private void pbExit_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Main - Exit Click ");
            //ClearSubMenu();
            //pbRouter.BackgroundImage = Radix.Properties.Resources.sub_Router_sel;

            //GlobalVar.TabMain = enumTabMain.Router;
            //tcMain.SelectedIndex = (int)GlobalVar.TabMain;

            //String RouterProgram = "MXN_HMI.exe";

            //if (FuncWin.FindWindowByExname(RouterProgram) == IntPtr.Zero)
            //{
            //    foreach (Process process in Process.GetProcesses())
            //    {

            //        if (process.ProcessName == RouterProgram.Replace(".exe", ""))
            //        {
            //            ShowWindow(process.MainWindowHandle, 5);
            //            BringWindowToTop(process.MainWindowHandle);
            //            SetForegroundWindow(process.MainWindowHandle);
            //        }
            //    }
            //}

            if ((int)GlobalVar.SystemStatus < (int)enumSystemStatus.AutoRun ||
                GlobalVar.SystemStatus == enumSystemStatus.ErrorStop)
            {
                //FuncWin.CloseWindowByExname("MXN_HMI.exe"); //확인필요
                //GlobalVar.GlobalStop = true;
                this.Close();
            }
            else
            {
                this.BringToFront();
                FuncWin.TopMessageBox("Can't stop program while system is running.");
            }
        }
        #endregion

        #region 상위 테스트 버튼

        private void btnSMD_Click(object sender, EventArgs e)
        {
            SMDTest dlg = new SMDTest();
            dlg.Show();
        }
        private void btnPCBInput_Click(object sender, EventArgs e)
        {
            //DIO.WriteDIData(enumDINames.X01_0_Input_Lift_Start_Sensor, true);
        }
        private void btnSmemaAfter_Click(object sender, EventArgs e)
        {
            //DIO.WriteDIData(enumDINames.X06_2_SMEMA_After_Ready, !DIO.GetDIData(enumDINames.X06_2_SMEMA_After_Ready));
            GlobalVar.TestPass = !GlobalVar.TestPass;

        }
        private void btnStepTest_Click_1(object sender, EventArgs e)
        {
            StepTest dlg = new StepTest();
            dlg.Show();
        }
        #endregion

        #endregion

        #region Start / STop 함수화
        public void Start_Button(bool op)
        {
            if (GlobalVar.SystemStatus == enumSystemStatus.Manual)
            {
                if (!GlobalVar.Simulation &&
                    DIO.EMG_Check())
                {
                    if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                    {
                        return;
                    }
                    else
                    {
                        FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                          DateTime.Now.ToString("HH:mm:ss"),
                                          enumErrorPart.System,
                                          enumErrorCode.E_Stop,
                                          false,
                                          "Emergency Stop Button Pressed. Release Button and Initialize system."));

                    }
                    FuncLog.WriteLog("HJ 확인 - Start 에러 추가     E_Stop");
                }
                else if (lbBeforeGantryEnable.Text != "1" ||
                    lbAfterGantryEnable.Text != "1" ||
                    GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_Before_Z2].PowerOn == false ||
                    GlobalVar.AxisStatus[(int)enum_Sanding_ServoAxis.Sanding_After_Z2].PowerOn == false)
                {
                    FuncLog.WriteLog($"Start - 갠트리 에러 Before : {lbBeforeGantryEnable.Text} / After : {lbAfterGantryEnable.Text} ");
                    GantrySetup(0);
                    Thread.Sleep(500);
                    GantrySetup(1);

                    Ecat.ServoReset_All();
                    Ecat.ServoOnAll(true); // 모든 서보 On

                    //알람 처리
                    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                          DateTime.Now.ToString("HH:mm:ss"),
                                          enumErrorPart.System,
                                          enumErrorCode.E_Stop,
                                          false,
                                          "GantrySetup Fail. Release Button and Initialize system."));
                }
                //*
                else if (//!GlobalVar.ServoInited ||
                    GlobalVar.SystemStatus <= enumSystemStatus.Initialize)
                {
                    if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                    {
                        return;
                    }
                    else
                    {
                        FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                                          DateTime.Now.ToString("HH:mm:ss"),
                                          enumErrorPart.System,
                                          enumErrorCode.System_Not_Inited,
                                          false,
                                          "System is not initialized. Initialize first."));

                    }
                    //#region Normal Error
                    //if (GlobalVar.UseNormalError)
                    //{
                    //    FuncError.AddError(enumError.System_Not_Inited);
                    //}
                    //#endregion
                    //#region Part Error
                    //if (GlobalVar.PartError)
                    //{
                    //    FuncError.AddError(new structError(DateTime.Now.ToString("yyyyMMdd"),
                    //                          DateTime.Now.ToString("HH:mm:ss"),
                    //                          enumErrorPart.System,
                    //                          enumErrorCode.System_Not_Inited,
                    //                          false,
                    //                          ""));
                    //}
                    //#endregion                  
                    FuncLog.WriteLog("HJ 확인 - Start 에러 추가     enumSystemStatus.BeforeInitialize");
                }

                //*/
                #region Door Check
                //*
                else if (GlobalVar.UseDoor &&
                        (DIO.Door_Check()))
                {
                    if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                    {
                        return;
                    }
                    else
                    {
                        this.BringToFront();
                        FuncWin.TopMessageBox("Can't start while doors are opened!");
                    }
                }
                //*/
                #endregion
                else if (GlobalVar.SystemErrored)
                {
                    if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                    {
                        return;
                    }
                    else
                    {
                        FuncWin.TopMessageBox("See error log first.");
                    }
                }
                else if (GlobalVar.RobotUse &&
                    !FuncRobot.RobotInited())
                {
                    if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                    {
                        return;
                    }
                    else
                    {
                        FuncWin.TopMessageBox("Init Robot first.");
                    }

                }
                else
                {
                    GlobalVar.CycleStop = false;
                    GlobalVar.EnableTower = true;
                    GlobalVar.EnableBuzzer = true;
                    GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
                    GlobalVar.SystemMsg = "Run started";
                    runTime = Environment.TickCount;

                }
            }
            else if (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop)
            {
                if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                {
                    return;
                }
                else
                {
                    FuncWin.TopMessageBox("See error log first.");
                }
            }
            else if (GlobalVar.SystemErrored)
            {
                if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                {
                    return;
                }
                else
                {
                    FuncWin.TopMessageBox("Error Check.");
                }
            }
            else // BeforeInit
            {
                if (op) // op 버튼으로 시작시는 에러차 있으면 안 되므로 그냥 리턴
                {
                    return;
                }
                else
                {
                    FuncWin.TopMessageBox("Run system initialization first.");
                }
            }
            if (GlobalVar.dlgOpened)
            {
                return;
            }
        }
        public void Stop_Button()
        {
            GlobalVar.SystemStatus = enumSystemStatus.Manual;
            GlobalVar.SystemMsg = "Run stoped";
            runTotal += (Environment.TickCount - runTime) / 1000;

            //Manual 상에서 계속 쓰레드가 돌기 때문에
            //FuncSanding.Before_Tray_In_Start = false;
            //FuncSanding.Before_Tray_Out_Start = false;

           // Ecat.MoveStopAll(); //전체 서보 정지 신호 by DGKim 230802
            //FuncRobot.RobotHold(true);  //쿠카 정지 신호

        }
        #endregion


        #region 수동조작 이벤트

        #region 수동 버튼
        private void pbDistributor_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
            {
                //DIO.WriteDOData(enumDONames.Y01_4_Label_Vacuum, DIO.GetDIData(enumDINames.X01_2_));
            }
        }

        private void lblDistributor_Click(object sender, EventArgs e)
        {
            pbDistributor_Click(sender, e);
        }

        private void pbSuppy2Stopper_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
            {
                //DIO.WriteDOData(enumDONames.Y02_5_, DIO.GetDIData(enumDINames.X02_6_));
            }
        }

        private void lblSuppy2Stopper_Click(object sender, EventArgs e)
        {
            pbSuppy2Stopper_Click(sender, e);
        }


        private void pbCapStopper_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
            {
                //DIO.WriteDOData(enumDONames.Y08_5_Cap_Stopper_Forward, DIO.GetDIData(enumDINames.X10_7_Cap_Stopper_Reward));
            }
        }

        private void lblCapStopper_Click(object sender, EventArgs e)
        {
            pbCapStopper_Click(sender, e);
        }


        #endregion

        #endregion

        private void numBeforeTrayIndex_ValueChanged(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus == enumSystemStatus.AutoRun)
            {
                return;
            }
            
            FuncSanding.Sanding_BeforeTrayRowIndex = int.Parse((((int)numBeforeTrayIndex.Value - 1) / FuncSanding.Sanding_BeforeTraySize[1]).ToString());
            FuncSanding.Sanding_BeforeTrayColumnIndex = ((int)numBeforeTrayIndex.Value - 1) % FuncSanding.Sanding_BeforeTraySize[1];
        }

        private void numAfterTrayIndex_ValueChanged(object sender, EventArgs e)
        {
            if (!((NumericUpDown)sender).Name.Equals("numAfterTrayIndex"))
            {
                return;
            }

            if (GlobalVar.SystemStatus == enumSystemStatus.AutoRun)
            {
                return;
            }


            //FuncSanding.Sanding_AfterTrayRowIndex = int.Parse((((int)numAfterTrayIndex.Value - 1) / FuncSanding.Sanding_AfterTraySize[1]).ToString());
            //FuncSanding.Sanding_AfterTrayColumnIndex = ((int)numAfterTrayIndex.Value - 1) % FuncSanding.Sanding_AfterTraySize[1];


            if (numAfterTrayIndex.Value > FuncSanding.Sanding_AfterTrayMax - 1)
            {
                return;
            }

            int column = (int)numAfterTrayIndex.Value - 1;
            int row = column / FuncSanding.Sanding_AfterTraySize[1];    //  row = col(11)/12
            if (FuncSanding.Megagen)
            {
                if (column >= FuncSanding.Sanding_AfterTraySize[1] * 3 + 10 * 6)
                {
                    row = 3 + 6 + (column - (FuncSanding.Sanding_AfterTraySize[1] * 3 + 10 * 6)) / FuncSanding.Sanding_AfterTraySize[1];
                    column = (column - (FuncSanding.Sanding_AfterTraySize[1] * 3 + 10 * 6)) % FuncSanding.Sanding_AfterTraySize[1];
                }
                else if (column >= FuncSanding.Sanding_AfterTraySize[1] * 3)
                {
                    row = 3 + (column - (FuncSanding.Sanding_AfterTraySize[1] * 3)) / 10;
                    column = (column - (FuncSanding.Sanding_AfterTraySize[1] * 3)) % 10;
                }
                else
                {
                    row = column / FuncSanding.Sanding_AfterTraySize[1];
                    column = column % FuncSanding.Sanding_AfterTraySize[1];
                }
            }
            else
            {
                row = column / FuncSanding.Sanding_AfterTraySize[1];
                column = column % FuncSanding.Sanding_AfterTraySize[1];
            }

            //column = column % GlobalVar.AfterTraySize[1];
            FuncSanding.Sanding_AfterTrayRowIndex = row;
            FuncSanding.Sanding_AfterTrayColumnIndex = column;
        }
        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!((ComboBox)sender).Focused)
            //{
            //    return;    // 포커스가 없는 상태에서의 변경은 처리안함
            //}

            //if (FuncWin.MessageBoxOK("Change model to " + cmbModel.Text + "?"))
            //{
            //    GlobalVar.ModelName = cmbModel.Text;
            //    FuncIni.LoadModelIni();
            //}
            //else
            //{
            //    for (int i = 0; i < cmbModel.Items.Count; i++)
            //    {
            //        if (cmbModel.Items[i].ToString() == GlobalVar.ModelName)
            //        {
            //            ActiveControl = null; // focus 해제하여 changed 이벤트 재발생 방지
            //            cmbModel.SelectedIndex = i;
            //            return;
            //        }
            //    }
            //}
        }

        #region 필요 함수 

        #region Input Box 입력을 받아야 되는 경우가 있을 때
        public static DialogResult InputBox(string title, string content, ref string value)
        {
            Form form = new Form();
            PictureBox picture = new PictureBox();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.ClientSize = new Size(300, 300);
            form.Controls.AddRange(new Control[] { label, picture, textBox, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            form.Text = title;
            picture.SizeMode = PictureBoxSizeMode.StretchImage;
            label.Text = content;
            textBox.Text = value;
            buttonOk.Text = "확인";
            buttonCancel.Text = "취소";

            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            picture.SetBounds(10, 10, 50, 50);
            label.SetBounds(25, 17, 100, 120);
            textBox.SetBounds(25, 220, 220, 20);
            buttonOk.SetBounds(135, 270, 70, 20);
            buttonCancel.SetBounds(215, 270, 70, 20);

            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;
            return dialogResult;
        }
        #endregion

        #region 배열로 버튼 만듬
        private Button[,] buttons = null;

        private void RemoveAllButton()
        {
            if (buttons != null)
            {
                for (int j = 0; j < buttons.GetLength(0); j++)
                {
                    for (int i = 0; i < buttons.GetLength(1); i++)
                    {
                        if (buttons[j, i] != null)
                        {
                            try
                            {
                                this.Controls.Remove((Button)buttons[j, i]);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                }
            }
        }

        private void btnButton_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;
            string[] rowCol = name.Replace("Button", "").Split('_');
            int rowIndex = -1;
            int colIndex = -1;
            int.TryParse(rowCol[0], out rowIndex);
            int.TryParse(rowCol[1], out colIndex);
            MessageBox.Show("Button Click \n row : " + rowIndex + "\n col : " + colIndex);
        }

        private void Make_Burron(int RowCount, int ColCount, int Width, int Height, int XStar, int XPitch, int YStart, int YPitch)
        {
            RemoveAllButton();

            int numRowCount = RowCount;//세로 갯수
            int numColCount = ColCount;//가로 갯수
            int numWidth = Width;//가로 크기
            int numHeight = Height;//세로 크기

            int numXStart = XStar;//X 시작 좌표
            int numXPitch = XPitch;//X Pitch
            int numYStart = YStart;//Y 시작 좌표
            int numYPitch = YPitch;//Y Pitch

            buttons = new Button[numRowCount, numColCount];

            for (int j = 0; j < buttons.GetLength(0); j++)
            {
                for (int i = 0; i < buttons.GetLength(1); i++)
                {
                    buttons[j, i] = new Button();
                    buttons[j, i].Location = new Point(numXStart + i * numXPitch, numYStart + j * numYPitch);
                    buttons[j, i].Size = new Size(numWidth, numHeight);
                    buttons[j, i].Text = j.ToString() + "_" + i.ToString();
                    buttons[j, i].Name = "Button" + j.ToString() + "_" + i.ToString();
                    buttons[j, i].Click += new System.EventHandler(this.btnButton_Click);
                    buttons[j, i].Image = Radix.Properties.Resources.LED_round_green;
                    buttons[j, i].BackgroundImageLayout = ImageLayout.Stretch;
                    tpMain.Controls.Add(buttons[j, i]);
                }
            }
        }
        #endregion

        #region 배열로 픽처박스/버튼 만듬
        private PictureBox[,] Before_PictureBox = null;
        private PictureBox[,] After_PictureBox = null;
        private Button[,] Before_TrayBtn = null;
        private Button[,] After_TrayBtn = null;

        private void RemoveAllPictureBox()
        {
            if (Before_PictureBox != null)
            {
                for (int j = 0; j < Before_PictureBox.GetLength(0); j++)
                {
                    for (int i = 0; i < Before_PictureBox.GetLength(1); i++)
                    {
                        if (Before_PictureBox[j, i] != null)
                        {
                            try
                            {
                                this.Controls.Remove((PictureBox)Before_PictureBox[j, i]);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                Console.WriteLine(ex.StackTrace);
                            }
                        }
                    }
                }
            }
        }
        private void Traybtn_Click(object sender, EventArgs e)
        {
            string name = ((Button)sender).Name;
            Button[,] pb = Before_TrayBtn;

            #region Name Part 비교
            if (name.Contains("BeforeTray"))
            {
                name = name.Replace("BeforeTray", "");
            }
            else if (name.Contains("AfterTray"))
            {
                name = name.Replace("AfterTray", "");
                pb = After_TrayBtn;
            }
            else
            {
                MessageBox.Show("PictureBox Make Name Check");
                return;
            }
            #endregion
            string[] rowCol = name.Replace("_Button", "").Split('_');
            int rowIndex = -1;
            int colIndex = -1;
            int.TryParse(rowCol[0], out rowIndex);
            int.TryParse(rowCol[1], out colIndex);
            MessageBox.Show("Button Click \n row : " + (rowIndex + 1) + "\n col : " + (colIndex + 1) + $"\n 컨트롤러 이름:{((Button)sender).Name}");

            MakeArrayImage = true;
        }

        //Megagen Tray,임플란트 센서쪽 버튼 이벤트  by DGKim
        private void btnPictureBox_Click(object sender, EventArgs e)
        {
            string name = ((PictureBox)sender).Name;
            PictureBox[,] pb = Before_PictureBox;

            #region Name Part 비교
            bool after = false;
            if (name.Contains("Before"))
            {
                name = name.Replace("Before", "");
            }
            else if (name.Contains("After"))
            {
                after = true;
                name = name.Replace("After", "");
                pb = After_PictureBox;
            }
            else
            {
                MessageBox.Show("PictureBox Make Name Check");
                return;
            }
            #endregion

            string[] rowCol = name.Replace("_PictureBox", "").Split('_');
            int rowIndex = -1;
            int colIndex = -1;
            int.TryParse(rowCol[0], out rowIndex);
            int.TryParse(rowCol[1], out colIndex);

            if (after &&
                rowIndex >= 3 &&
                rowIndex <= 8 &&
                colIndex >= 5)
            {
                colIndex -= 5;
            }
                MessageBox.Show("PictureBox Click \n row : " + (rowIndex + 1) + "\n col : " + (colIndex + 1) + $"\n 컨트롤러 이름:{((PictureBox)sender).Name}");

            MakeArrayImage = true;

            //테스트용 확인
            //((PictureBox)pb[rowIndex, colIndex]).Size = new Size(50, 50);
        }

        //Megagen 이미지 생성만 아니라 버튼생성도 가능하게 함수 만듬 by DGKim
        private void Make_Controls<T>(ref T[,] controls, string Partname, int RowCount, int ColCount, int Width, int Height, int XStar, int XPitch, int YStart, int YPitch) where T : Control, new()
        {
            RemoveAllPictureBox();

            int numRowCount = RowCount; // 세로 갯수
            int numColCount = ColCount; // 가로 갯수
            int numWidth = Width; // 가로 크기
            int numHeight = Height; // 세로 크기

            int numXStart = XStar; // X 시작 좌표
            int numXPitch = XPitch; // X Pitch
            int numYStart = YStart; // Y 시작 좌표
            int numYPitch = YPitch; // Y Pitch

            controls = new T[numRowCount, numColCount];

            for (int j = 0; j < controls.GetLength(0); j++)
            {
                for (int i = 0; i < controls.GetLength(1); i++)
                {
                    controls[j, i] = new T();
                    controls[j, i].Location = new Point(numXStart + i * numXPitch, numYStart + j * numYPitch);
                    controls[j, i].Size = new Size(numWidth, numHeight);
                    controls[j, i].TabStop = false;


                    // 컨트롤러 이름에 따른 구분을 위한 코드
                    if (controls[j, i] is PictureBox)
                    {
                        controls[j, i].Name = Partname + "_PictureBox" + j.ToString() + "_" + i.ToString();
                    }
                    else if (controls[j, i] is Button)
                    {
                        controls[j, i].Name = Partname + "_Button" + j.ToString() + "_" + i.ToString();
                    }


                    if (Partname == "Before")
                    {
                        if (controls[j, i] is PictureBox)
                        {
                            PictureBox pictureBox = controls[j, i] as PictureBox;
                            pictureBox.Image = Radix.Properties.Resources.Lamp_Green_1;
                            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                            pictureBox.Click += new EventHandler(btnPictureBox_Click); // 이벤트 핸들러 추가
                        }

                        pnTrayLeft.Controls.Add(controls[j, i]);
                    }
                    else if (Partname == "After")
                    {
                        if (controls[j, i] is PictureBox)
                        {

                            PictureBox pictureBox = controls[j, i] as PictureBox;
                            if (i >= 5 && i <= 9 && j >= 3 && j <= 8) // YJ 90도 바꿔서 i/j 서로 바뀜
                            {
                                pictureBox.Image = null;
                            }
                            else
                            {
                                pictureBox.Image = Radix.Properties.Resources.Lamp_Gray_1;
                                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                pictureBox.Click += new EventHandler(btnPictureBox_Click); // 이벤트 핸들러 추가
                            }
                        }

                        #region 샌딩후 트레이 배열 표시는 90도 돌려야 한다.
                        //controls[j, i].Location = new Point(numXStart + i * numXPitch, numYStart + j * numYPitch);
                        controls[j, i].Location = new Point(numXStart + j * numXPitch, numYStart + i * numYPitch); // YJ i/j만 서로 바꾸면 위치가 90도 돌아간다.
                        #endregion

                        pnTrayLeft.Controls.Add(controls[j, i]);
                    }
                    else if (Partname == "BeforeTray")
                    {
                        if (controls[j, i] is Button)
                        {
                            Button button = controls[j, i] as Button;
                            button.Text = $"{i + 1}열/{j + 1}층";
                            button.BackColor = Color.Lime;
                            button.FlatStyle = FlatStyle.Flat;

                            button.FlatAppearance.BorderSize = 1;
                            //button.Font = new System.Drawing.Font("Calibri", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            button.Padding = Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
                            button.Click += new EventHandler(Traybtn_Click); // 이벤트 핸들러 추가
                        }
                        pnBeforeTrayRack.Controls.Add(controls[j, i]);
                    }
                    else if (Partname == "AfterTray")
                    {
                        if (controls[j, i] is Button)
                        {
                            Button button = controls[j, i] as Button;
                            button.Text = $"{i + 1}열/{j + 1}층";
                            button.BackColor = Color.Gray;
                            button.FlatStyle = FlatStyle.Flat;

                            button.FlatAppearance.BorderSize = 1;
                            //button.Font = new System.Drawing.Font("Calibri", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            button.Padding = Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);

                            button.Click += new EventHandler(Traybtn_Click); // 이벤트 핸들러 추가
                        }
                        pnAfterTrayRack.Controls.Add(controls[j, i]);
                    }


                    //pnTrayLeft.Controls.Add(controls[j, i]);

                    controls[j, i].BringToFront();
                }
            }

            if (Partname == "After")
            {
                MakeArrayImage = true;
            }
        }


        #region 주석처리 90도 회전 X
        //private void Make_Controls<T>(ref T[,] controls, string Partname, int RowCount, int ColCount, int Width, int Height, int XStar, int XPitch, int YStart, int YPitch) where T : Control, new()
        //{


        //    int numRowCount = RowCount; // 세로 갯수
        //    int numColCount = ColCount; // 가로 갯수
        //    int numWidth = Width; // 가로 크기
        //    int numHeight = Height; // 세로 크기

        //    int numXStart = XStar; // X 시작 좌표
        //    int numXPitch = XPitch; // X Pitch
        //    int numYStart = YStart; // Y 시작 좌표
        //    int numYPitch = YPitch; // Y Pitch

        //    controls = new T[numRowCount, numColCount];

        //    for (int j = 0; j < controls.GetLength(0); j++)
        //    {
        //        for (int i = 0; i < controls.GetLength(1); i++)
        //        {
        //            controls[j, i] = new T();
        //            controls[j, i].Location = new Point(numXStart + i * numXPitch, numYStart + j * numYPitch);
        //            controls[j, i].Size = new Size(numWidth, numHeight);
        //            controls[j, i].TabStop = false;


        //            // 컨트롤러 이름에 따른 구분을 위한 코드
        //            if (controls[j, i] is PictureBox)
        //            {
        //                controls[j, i].Name = Partname + "_PictureBox" + j.ToString() + "_" + i.ToString();
        //            }
        //            else if (controls[j, i] is Button)
        //            {
        //                controls[j, i].Name = Partname + "_Button" + j.ToString() + "_" + i.ToString();
        //            }


        //            if (Partname == "Before")
        //            {
        //                if (controls[j, i] is PictureBox)
        //                {
        //                    PictureBox pictureBox = controls[j, i] as PictureBox;
        //                    pictureBox.Image = Radix.Properties.Resources.Lamp_Green_1;
        //                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        //                    pictureBox.Click += new EventHandler(btnPictureBox_Click); // 이벤트 핸들러 추가
        //                }

        //                pnTrayLeft.Controls.Add(controls[j, i]);
        //            }
        //            else if (Partname == "After")
        //            {
        //                if (controls[j, i] is PictureBox)
        //                {

        //                    PictureBox pictureBox = controls[j, i] as PictureBox;
        //                    if (j >= 5 && j <= 9 && i >= 3 && i <= 8)
        //                    {
        //                        pictureBox.Image = null;
        //                    }
        //                    else
        //                    {
        //                        pictureBox.Image = Radix.Properties.Resources.Lamp_Gray_1;
        //                        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        //                        pictureBox.Click += new EventHandler(btnPictureBox_Click); // 이벤트 핸들러 추가
        //                    }
        //                }

        //                pnTrayLeft.Controls.Add(controls[j, i]);
        //            }
        //            else if (Partname == "BeforeTray")
        //            {
        //                if (controls[j, i] is Button)
        //                {
        //                    Button button = controls[j, i] as Button;
        //                    button.Text = $"{j + 1}층/{i + 1}번";
        //                    button.BackColor = Color.Lime;
        //                    button.FlatStyle = FlatStyle.Flat;

        //                    button.FlatAppearance.BorderSize = 1;
        //                    //button.Font = new System.Drawing.Font("Calibri", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        //                    button.Padding = Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
        //                    button.Click += new EventHandler(Traybtn_Click); // 이벤트 핸들러 추가
        //                }
        //                pnBeforeTrayRack.Controls.Add(controls[j, i]);
        //            }
        //            else if (Partname == "AfterTray")
        //            {
        //                if (controls[j, i] is Button)
        //                {
        //                    Button button = controls[j, i] as Button;
        //                    button.Text = $"{j + 1}층/{i + 1}번";
        //                    button.BackColor = Color.Gray;
        //                    button.FlatStyle = FlatStyle.Flat;

        //                    button.FlatAppearance.BorderSize = 1;
        //                    //button.Font = new System.Drawing.Font("Calibri", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        //                    button.Padding = Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);

        //                    button.Click += new EventHandler(Traybtn_Click); // 이벤트 핸들러 추가
        //                }
        //                pnAfterTrayRack.Controls.Add(controls[j, i]);
        //            }


        //            //pnTrayLeft.Controls.Add(controls[j, i]);

        //            controls[j, i].BringToFront();
        //        }
        //    }

        //    if (Partname == "After")
        //    {
        //        MakeArrayImage = true;
        //    }
        //}
        #endregion


        private void Make_PictureBox(ref PictureBox[,] pb, string Partname, int RowCount, int ColCount, int Width, int Height, int XStar, int XPitch, int YStart, int YPitch)
        {
            //RemoveAllPictureBox();

            int numRowCount = RowCount;//세로 갯수
            int numColCount = ColCount;//가로 갯수
            int numWidth = Width;//가로 크기
            int numHeight = Height;//세로 크기

            int numXStart = XStar;//X 시작 좌표
            int numXPitch = XPitch;//X Pitch
            int numYStart = YStart;//Y 시작 좌표
            int numYPitch = YPitch;//Y Pitch

            pb = new PictureBox[numRowCount, numColCount];

            for (int j = 0; j < pb.GetLength(0); j++)
            {
                for (int i = 0; i < pb.GetLength(1); i++)
                {
                    pb[j, i] = new PictureBox();
                    //pb[j, i].Location = new Point(numXStart + i * numXPitch + (j % 2 == 1 ? numXPitch / 2 : 0), numYStart + j * numYPitch);   //엇갈리게 표시
                    pb[j, i].Location = new Point(numXStart + i * numXPitch, numYStart + j * numYPitch);
                    pb[j, i].Size = new Size(numWidth, numHeight);
                    pb[j, i].Name = Partname + "_PictureBox" + j.ToString() + "_" + i.ToString();
                    pb[j, i].Click += new System.EventHandler(this.btnPictureBox_Click);

                    if (Partname == "Before")
                    {
                        pb[j, i].Image = Radix.Properties.Resources.Lamp_Green_1;
                    }
                    else if (Partname == "After")
                    {
                        if (j >= 5 && j <= 9 && i >= 3 && i <= 8)
                        {
                            pb[j, i].Image = null; // 이미지를 비어있게 설정합니다.
                        }
                        else
                        {
                            pb[j, i].Image = Radix.Properties.Resources.Lamp_Gray_1;
                        }
                    }

                    //pb[j, i].BackgroundImageLayout = ImageLayout.Stretch; //이건 백그라운드 이미지 적용시
                    pb[j, i].SizeMode = PictureBoxSizeMode.StretchImage;    //SizeMode는 일반이미지 적용시

                    //tpMain.Controls.Add(pb[j, i]);
                    pnTrayLeft.Controls.Add(pb[j, i]);
                    pb[j, i].BringToFront();
                    //tpMain.Controls.Remove(pb[j, i]);
                }
            }

            if (Partname == "After")
            {
                MakeArrayImage = true;
            }

        }
        #endregion

        #endregion


















        //테스트 버튼 ////////////////////////////////////////////////////////////////////////////////////////////







        

        private void pbReset_Click(object sender, EventArgs e)
        {
            //if (GlobalVar.SystemStatus <= enumSystemStatus.Initialize ||
            // GlobalVar.SystemStatus == enumSystemStatus.EmgStop)
            //{
            //    GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
            //}
            //else if (GlobalVar.SystemStatus == enumSystemStatus.ErrorStop)
            //{
            //    GlobalVar.SystemStatus = enumSystemStatus.Manual;
            //}
            if (FuncSanding.Before_TrayState == (int)enum_Sanding_Action.Ready)
            {
                if (DIO.GetDIData((int)DIO_Sanding_enumDINames.X01_0_Before_Tray_MoveArea_Check_Sensor) ||
                    DIO.GetDIData((int)DIO_Sanding_enumDINames.X01_1_Before_Tray_WorkArea_Check_Sensor))
                {
                    FuncWin.TopMessageBox("[Before] 셔틀센서에 Tray 감지중 - 제거해 주세요");
                    return;
                }
                else
                {
                    FuncWin.TopMessageBox("[Before] 셔틀에 Tray가 있을경우 제거해 주세요");
                }
                FuncSanding.Before_Working_Tray_Ready = false;
                FuncSanding.Before_Tray_Out_Start = false;  //샌딩 후 트레이 배출시작 KuKa에서 True  직교로봇은 신호 확인 후 Move
                FuncSanding.ErrorBeforeTray = false;        //에러 확인 리셋
            }
            if (FuncSanding.After_TrayState == (int)enum_Sanding_Action.Ready)
            {
                if (DIO.GetDIData((int)DIO_Sanding_enumDINames.X02_0_After_Tray_MoveArea_Check_Sensor) ||
                    DIO.GetDIData((int)DIO_Sanding_enumDINames.X02_1_After_Tray_WorkArea_Check_Sensor))
                {
                    FuncWin.TopMessageBox("[After] 셔틀센서에 Tray 감지중 - 제거해 주세요");
                    return;
                }
                else
                {
                    FuncWin.TopMessageBox("[After] 셔틀에 Tray가 있을경우 제거해 주세요");
                }
                FuncSanding.After_Working_Tray_Ready = false;
                FuncSanding.After_Tray_Out_Start = false;  //샌딩 후 트레이 배출시작 KuKa에서 True  직교로봇은 신호 확인 후 Move
                FuncSanding.ErrorAfterTray = false;        //에러 확인 리셋
            }

            if (!FuncRobot.RobotInited_Kuka())
            {
                FuncRobot.RobotInit();
            }
            

            FuncError.RemoveAllError();

            GlobalVar.E_Stop = false;
            GlobalVar.EnableTower = true;
            GlobalVar.EnableBuzzer = true;
            GlobalVar.SystemErrored = false;

            FuncRobot.RobotHold(false);
            FuncRobot.RobotReset();

            Ecat.ServoReset_All();

          

            //for (uint axis = 0; axis < GlobalVar.Axis_count; axis++)
            //{
            //    Ecat.ServoReset(axis);
            //}
            Ecat.ServoOnAll(true);
            FuncSanding.MGG_SandingErrorCode = "";   //MGG - 에러코드 확인을 위해 추가 by DGKim
        }

        private void pBB_count_reset_Click(object sender, EventArgs e)
        {
            FuncSanding.Before_Tray_In_Count = 0;
            FuncSanding.Before_Tray_Out_Count = 0;
        }

        private void pBA_count_reset_Click(object sender, EventArgs e)
        {
            FuncSanding.After_Tray_In_Count = 0;
            FuncSanding.After_Tray_Out_Count = 0;
        }

        #region 컨트롤러 인스턴스
        /// <summary>
        /// PLCStatusThread에서 Main UI를 컨트롤 하기 위해 by DGKim 230327
        /// </summary>


        public void logView(string text, string name)
        {
            if (name == "BeforeLog")
            {

                if (lbBeforeLog.InvokeRequired)
                {
                    lbBeforeLog.Invoke(new Action<string, string>(logView), new object[] { text, name });
                }
                else
                {
                    lbBeforeLog.Text = text;
                }
            }
            else if (name == "RobotLog")
            {
                if (lbRobotLog.InvokeRequired)
                {
                    lbRobotLog.Invoke(new Action<string, string>(logView), new object[] { text, name });
                }
                else
                {
                    lbRobotLog.Text = text;
                }
            }
            else if (name == "AfterLog")
            {
                if (lbAfterLog.InvokeRequired)
                {
                    lbAfterLog.Invoke(new Action<string, string>(logView), new object[] { text, name });
                }
                else
                {
                    lbAfterLog.Text = text;
                }
            }

        }



        #endregion

        private void inTray_Click(object sender, EventArgs e)
        {
            if (GlobalVar.Simulation)
            {
                DIO.WriteDIData(((int)DIO_Sanding_enumDINames.X05_0_Before_Tray_Input_Check_Sensor_1 + ((((int)numbcol.Value - 1) * FuncSanding.Sanding_BeforeTray_RackSize.GetLength(0)) + (int)(numbrow.Value - 1)) * 2),
              !DIO.GetDIData((int)DIO_Sanding_enumDINames.X05_0_Before_Tray_Input_Check_Sensor_1 + ((((int)numbcol.Value - 1) * FuncSanding.Sanding_BeforeTray_RackSize.GetLength(0)) + (int)(numbrow.Value - 1)) * 2));
                DIO.WriteDIData((int)DIO_Sanding_enumDINames.X05_1_Before_Implant_Input_Check_Sensor_1 + ((((int)numbcol.Value - 1) * FuncSanding.Sanding_BeforeTray_RackSize.GetLength(0)) + (int)(numbrow.Value - 1)) * 2,
                !DIO.GetDIData((int)DIO_Sanding_enumDINames.X05_1_Before_Implant_Input_Check_Sensor_1 + ((((int)numbcol.Value - 1) * FuncSanding.Sanding_BeforeTray_RackSize.GetLength(0)) + (int)(numbrow.Value - 1)) * 2));
            }
        }

        private void inATray_Click(object sender, EventArgs e)
        {
            if (GlobalVar.Simulation)
            {
                DIO.WriteDIData(((int)DIO_Sanding_enumDINames.X18_0_After_Tray_Input_Check_Sensor_1 + ((((int)numAcol.Value - 1) * FuncSanding.Sanding_AfterTray_RackSize.GetLength(0)) + (int)(numArow.Value - 1)) * 2),
              !DIO.GetDIData((int)DIO_Sanding_enumDINames.X18_0_After_Tray_Input_Check_Sensor_1 + ((((int)numAcol.Value - 1) * FuncSanding.Sanding_AfterTray_RackSize.GetLength(0)) + (int)(numArow.Value - 1)) * 2));
                DIO.WriteDIData((int)DIO_Sanding_enumDINames.X18_1_After_Implant_Input_Check_Sensor_1 + ((((int)numAcol.Value - 1) * FuncSanding.Sanding_AfterTray_RackSize.GetLength(0)) + (int)(numArow.Value - 1)) * 2,
                !DIO.GetDIData((int)DIO_Sanding_enumDINames.X18_1_After_Implant_Input_Check_Sensor_1 + ((((int)numAcol.Value - 1) * FuncSanding.Sanding_AfterTray_RackSize.GetLength(0)) + (int)(numArow.Value - 1)) * 2));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int a = 0;
            GantrySetup(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            uint upOnOff = 0;
            CAXM.AxmSignalIsServoOn(1, ref upOnOff);

            CAXM.AxmSignalServoOn(1, upOnOff == (uint)1 ? (uint)0 : (uint)1);



        }

        private void button4_Click(object sender, EventArgs e)
        {
            GantrySetup(0);
        }
    }
}

