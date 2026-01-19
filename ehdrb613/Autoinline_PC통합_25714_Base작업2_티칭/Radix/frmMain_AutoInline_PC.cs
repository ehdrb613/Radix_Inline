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
    public partial class frmMain_AutoInline_PC : Form
    {
        /* frmMain.cs : 주 HMI 구현용 클래스
         *    1. 장비의 전반적인 상태 및 카운트 등 표시
         *    2. 세부적인 프로세스가 아닌 전반적인 장비 컨트롤
         *    3. 버튼 등을 이용한 기능 분기
         */

        #region 로컬 변수
        ulong buzzerTime = GlobalVar.TickCount64; // 부저 시간 제어용
        private bool beforeStart = false; // 자동 공정 시작 이전값, 변화시 동작 제어용

        private System.Threading.Timer timerUI; // Thread Timer
        private bool timerDoing = false;
        private bool led = true; // LED Light 제어용

        private int runTime = Environment.TickCount; // 공정시간 적산용
        private int runTotal = 0; // 공정 카운트 적산용

        private ulong checkTime = GlobalVar.TickCount64; // 로더,언로더 체크시간 주기 세팅용   

        private FuncInline.enumTabMain beforeTab = FuncInline.TabMain; // 다른 폼에서 탭을 강제로 이동시킬 경우 확인 위해

        #endregion


        #region 인라인화한 폼들
        ErrorDialog errorDialog = null;

        Manual frmManual = new Manual();
        Machine frmMachine = new Machine();
        Model frmModel = new Model();
        Teaching frmTeaching = new Teaching();
        Trace frmTrace = new Trace();
        LogViewer frmLogViewer = new LogViewer();
        PartClear frmPartClear = new PartClear();
        Origin frmOrigin = new Origin();
        //NGViewer dlgNG = new NGViewer();

        #endregion

        SiteDetail dlgDetail = null;

        private bool simulationPCBInput = false;
        private bool teachingMode = false;
        private enumSystemStatus beforeStatus = enumSystemStatus.BeforeInitialize;
        private FuncInline.enumBuyerChange beforeBuyer = FuncInline.enumBuyerChange.White;
        private enumRunMode beforeRunMode = enumRunMode.AutoRun;

        private void debug(string str) // 클래스 내부 콜용 Local Debug
        {
            Util.Debug("frmMain : " + str);
        }

        #region 초기화 관련 함수
        public frmMain_AutoInline_PC() // 클래스 초기화 함수
        {
            InitializeComponent();
        }

        #endregion


        private void frmMain_Shown(object sender, EventArgs e) // 화면 출력시
        {
            FuncInline.Mainform = this;

#if !DEBUG
            GlobalVar.Debug = false; // Debug로 컴파일시 Debug옵션 켜고, Release시 끈다
#endif

            #region 로그 Thread 시작
            (new Thread((new LogThread()).Run)).Start();
            #endregion

            FuncIni.LoadSimulationIni(); // simulation.ini 파일 있으면 시뮬레이션모드
            //* // 시뮬레이션시 UI변경 천천히 돌려 확인시 사용
            if (GlobalVar.Simulation)
            {
                GlobalVar.ThreadSleep = 1000;
            }
            //*/

            #region 화면 배치
            Screen[] sc = Screen.AllScreens; // 모니터 정보
            #endregion

            #region frmMain을 첫 모니터 좌상단에 배치
            this.Left = sc[0].Bounds.Left;
            this.Top = sc[0].Bounds.Top;
            lblVersion.Text = $"Ver - {Application.ProductVersion}";
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

            this.Activate(); // 다른 화면 띄우다가 포커스 넘어갈 수 있어서 강제로 포커스 가져온다.

            GlobalVar.ProjectClass = new AutoInline_Class();      //프로젝트 클레스

            // 1STEP 상태는 UI에 따른다.
            GlobalVar.StepStop = cbOneStep.Checked;


            //Controller.MasterInit();

            //if (!Controller.MasterChecked)
            //{
            //    try
            //    {
            //        dlgLoading.Close();
            //    }
            //    catch { }
            //    FuncLog.WriteLog("Kernel Init Failed!");
            //    this.BringToFront();
            //    FuncWin.TopMessageBox("Kernel Init Failed!");
            //    Controller.initFail = true;
            //    GlobalVar.GlobalStop = true;
            //    this.Close();
            //    return;
            //}

            #region motion & io status thread start
            //*
            DIO.InitDIO();   //State쓰레드 구동전 적용된 프로젝트로 적용 시켜준다.
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
                // 샘플 투입 버튼 감추기
                //btnSample.Visible = false;
            }
            #endregion

            FuncLog.WriteLog("Program Started");

            // IO 초기화

            #region 자동 동작 Thread 시작


            // Create 스레드
            //BoxPacking_AutoRun.Create_AutoRun_Threads();

            #endregion

            Thread.Sleep(1000); // 각 쓰레드 구동 확인 위해 잠시 대기

            #region 프로그램 시작시 출력이 나가야 하는 DIO
            if (GlobalVar.Simulation)
            {
                DIO.InitSimulation();
            }
            #endregion

            #region 시뮬레이션 경우 DI 기본값이 없으므로 각 센서 기본값 강제할당
            if (GlobalVar.Simulation)
            {
                DIO.EMG_Control(true);
            }
            #endregion

            //#region 변수 초기화
            //// 배열 이외에는 생성시 기본값 지정되어 있어 초기화할 필요 없다

            //for (int i = 0; i < FuncAmplePacking.ample.Length; i++)
            //{
            //    // 배열 요소가 초기화되지 않았을 경우 인스턴스화
            //    if (FuncAmplePacking.ample[i] == null)
            //    {
            //        FuncAmplePacking.ample[i] = new Ampule(); // AmpleClass를 실제 클래스명으로 대체하세요.
            //    }


            //}
            //#endregion

            #region 설정 저장값 읽기
            string IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini";
            string Section = GlobalVar.IniSection;
            GlobalVar.ModelName = FuncIni.ReadIniFile(Section, "DefaultModel", IniPath, "");
            GlobalVar.Language = (enumLanguage)Convert.ToInt16(FuncIni.ReadIniFile("Default", "language", IniPath, "0"));

            // 설정 읽기
            Func.LoadAllIni(); // 모든 설정을 읽어서 전역변수에 저장
            #endregion


            FuncLog.DeleteLogs();
            ////////////////////////////////////

            // Domino Laser marking thread
            //GlobalVar.LaserMark.InitThread();

            // Zebra Label Printer thread
            //GlobalVar.LabelPrint.InitThread();

            #region MsSQL 연결
            if (GlobalVar.UseMsSQL)
            {
                GlobalVar.Sql.Disconnect();
                //GlobalVar.Sql = new MsSQL(GlobalVar.MsSQL_Server, GlobalVar.MsSQL_Port, GlobalVar.MsSQL_Id, GlobalVar.MsSQL_Pwd, GlobalVar.MsSQL_DB);
                GlobalVar.Sql = new MsSQL();

                // 시작시 접속 테스트를 하지말자 JHRYU 2024.3
                //GlobalVar.Sql.Connect();
                //if (!GlobalVar.Sql.connected)
                //{
                //    FuncLog.WriteLog("DataBase connection Failed!");
                //    FuncWin.TopMessageBox("DataBase connection Failed!");
                //}
                //else
                //{
                //    //FuncSql.UpdateDatabase();
                //}
            }
            #endregion

            #region 로케일 적용
            setLanguage();
            #endregion

            FuncMotion.ServoReset_All();

            //for (ushort axis = 0; axis < GlobalVar.Axis_count; axis++)
            //{
            //    //GlobalVar.AxisStatus[axis].StandStill = false;
            //    RTEX.ServoReset(axis);
            //}

            FuncMotion.ServoOnAll(true); // 모든 서보 On

        

            //#region 화면 제어용 쓰레드 타이머 시작
            TimerCallback CallBackUI = new TimerCallback(TimerUI);
            timerUI = new System.Threading.Timer(CallBackUI, false, 0, 100);


            //#region 모델 목록
            //#endregion


            #region sub form 초기화
          
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

            //frmManual = new Manual();
            frmManual.FormBorderStyle = FormBorderStyle.None;
            frmManual.TopMost = true;
            frmManual.Dock = DockStyle.Fill;
            frmManual.TopLevel = false;
            tpManual.Controls.Clear();
            tpManual.Controls.Add(frmManual);
            frmManual.Show();

            //frmModel = new Model();
            frmModel.FormBorderStyle = FormBorderStyle.None;
            frmModel.TopMost = true;
            frmModel.Dock = DockStyle.Fill;
            frmModel.TopLevel = false;
            tpModel.Controls.Clear();
            tpModel.Controls.Add(frmModel);
            frmModel.Show();


            /*
            
                                    //frmMES = new MES();
                                    frmMES.FormBorderStyle = FormBorderStyle.None;
                                    frmMES.TopMost = true;
                                    frmMES.Dock = DockStyle.Fill;
                                    frmMES.TopLevel = false;
                                    tpModel.Controls.Clear();
                                    tpModel.Controls.Add(frmMES);
                                    frmMES.Show();

                                    //*/
            tcMain.SelectedIndex = 0;
            #endregion


            // 툴팁 기본 설정
            //ttAuto.SetToolTip(this.btnRePrint, "프린트 오류로 장비 정지시 사용\n다시 시작할 때 프린팅 과정을\n재시도하게 됩니다.");

            //ttAuto.SetToolTip(this.btnBoxConv_BarCylinderUP, "Box 컨베이어에서 종이박스를 수거할때 사용\nBox 눌림 실린더를 MANUAL 조작 합니다.");

            #region 콤보박스 등 로드
            LoadActionList();
            #endregion

            try
            {
                dlgLoading.Close();
            }
            catch { }
        }


        private void frmMain_FormClosed(object sender, FormClosedEventArgs e) // 프로그램 종료시
        {

            if (!Controller.initFail)
            {
                FuncLog.WriteLog("Program Ended");
                if (GlobalVar.Simulation == false)
                {
                    //GlobalVar.WorkWidth.Close();
                    //GlobalVar.WorkConv.Close();
                    FuncMotion.ServoOnAll(false);
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
                //timerTrayUI.Dispose();
                pb_AlarmGif.Visible = false;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }

            //if (GlobalVar.Simulation == true)
            //{
                Environment.Exit(Environment.ExitCode);//프로그램 남는거 때문에
                                                       //base.OnFormClosed(e);
                                                       //Dispose(); // 폼 해제
            //}



        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) // 사용자 확인 후 프로그램 종료 시도
        {
            if (!Controller.initFail)
            {
                if ((int)GlobalVar.SystemStatus < (int)enumSystemStatus.AutoRun)
                {
                    this.BringToFront();
                    if (!Controller.MasterChecked ||
                        FuncWin.MessageBoxOK("Terminate Program?"))
                    {
                        //ClearSubMenu();
                        //pbRouter.BackgroundImage = Radix.Properties.Resources.sub_Router_sel;

                        //FuncInline.TabMain = FuncInline.enumTabMain.Router;
                        //tcMain.SelectedIndex = (int)FuncInline.TabMain;
                        //DIO.WriteDOData(FuncAmplePacking.FuncInline.enumDONames.Y22_3_LED_Lamp_On, false);
                       
                         //닫을때 도어확인

                        GlobalVar.GlobalStop = true;

                        Thread.Sleep(1500);

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


        public void setLanguage() // 각 Form 클래스마다 각각 정의해서 화면 컨트롤 등에 대한 언어 적용. 이벤트성 메시지는 발생시 직접 읽어오도록
        {
            switch (GlobalVar.Language)
            {
                case enumLanguage.Korean:
                    break;
                case enumLanguage.English:
                default:
                    string path = "C:\\FA\\" + GlobalVar.SWName + "\\Language\\" + GlobalVar.Language.ToString() + ".ini";
                    //btnStart.Text = FuncFile.ReadIniFile("frmMain", "btnStart.Text", path, "Start");
                    //btnInit.Text = FuncFile.ReadIniFile("frmMain", "btnInit.Text", path, "Initialize");
                    break;
            }
        }

        #region 타이머 함수

        private void TimerUI(Object state) // 화면 제어 쓰레드 타이머 함수
        {

            if (FuncInline.Mainform == null) return;

            try
            {
                if (timerDoing)
                {
                    return;
                }
                timerDoing = true;
                if (!GlobalVar.GlobalStop &&
                             this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        Stopwatch watch = new Stopwatch();
                        watch.Start();

                        #region 시뮬레이션 리프트 위치 표시

                        #endregion

                        #region 시스템 운영상태 표시
                        string runMode = GlobalVar.SystemStatus.ToString();
                        if (FuncInline.PassMode)
                        {
                            runMode += " PassMode";
                        }
                        if (FuncInline.SimulationMode)
                        {
                            runMode += " SimulationMode";
                        }
                        if (GlobalVar.DryRun)
                        {
                            runMode += " DryRun";
                        }
                        if (FuncInline.CycleStop)
                        {
                            runMode += " CycleStop";
                        }
                        //lblRunMode.Text = runMode;
                        btnBuzzerStop.Visible = DIO.GetDORead((int)FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer);

                        lblTestCycleTime.Text = FuncInline.TestCycleTime.ToString("F1") + " sec";

                        btnRunPass.BackColor = GlobalVar.SystemStatus >= enumSystemStatus.AutoRun ?
                                            FuncInline.PassMode ? Color.Yellow : Color.Lime :
                                            Color.White;
                        btnRunPass.Text = FuncInline.PassMode ? "Pass Mode" :
                                          GlobalVar.DryRun ? "Dry Run" :
                                          FuncInline.SimulationMode ? "Simulation Mode" :
                                          FuncInline.CycleStop ? "Cycle Stop" : "Auto Run";
                        // jhryu : AutoRun/Pass Mode 색구분 요구
                        //if ((GlobalVar.SystemStatus >= enumSystemStatus.AutoRun) && FuncInline.PassMode) btnRunPass.BackColor = Color.Yellow;


                        if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
                        {
                            FuncInline.CycleStop = false;
                        }
                        #endregion

                        #region 교시모드 바뀌면 버튼 표시 변경
                        if (GlobalVar.PwdPass != teachingMode)
                        {
                            ClearSubMenu();
                            GlobalVar.Muting = GlobalVar.PwdPass;
                            FuncInline.SetMute(GlobalVar.Muting);
                          
                        }
                        teachingMode = GlobalVar.PwdPass;
                        lblTeachingMode.Visible = GlobalVar.PwdPass;
                        #endregion
                        #region 러닝 상태 변경시 버튼 표시 변경
                        if (GlobalVar.SystemStatus != beforeStatus)
                        {
                            ClearSubMenu();
                        }
                        beforeStatus = GlobalVar.SystemStatus;
                        #endregion

                        btnCNPass.BackColor = FuncInline.PassCNDuplication ? Color.Lime : Color.White;

                        #region 시스템 세팅 정보
                        // 쿨링
                        dataGridSystem.Rows[0].Cells[1].Value = (!FuncInline.CoolingByTime && !FuncInline.CoolingByTemperature ? "No Cooling" : "") +
                                                     (FuncInline.CoolingByTime ? "Time " + FuncInline.CoolingTime + "sec," : "") +
                                                     (FuncInline.CoolingByTemperature ? "Temp. " + FuncInline.CoolingTemperature + "deg. max " + FuncInline.CoolingMaxTime + "sec." : "");
                        // Site Block
                        dataGridSystem.Rows[1].Cells[1].Value = (!FuncInline.BlockNGArray ? "No Block" : FuncInline.BlockNGCount.ToString() + " times");
                        // Defect Block
                        dataGridSystem.Rows[2].Cells[1].Value = (FuncInline.DefectRate == 0 ? "No Block" : FuncInline.DefectRate.ToString() + " %");
                        // Self Retest
                        dataGridSystem.Rows[3].Cells[1].Value = (!FuncInline.SelfRetest ? "Not Use" : FuncInline.SelfRetestCount.ToString() + " times");
                        // Resite
                        dataGridSystem.Rows[4].Cells[1].Value = (!FuncInline.OtherRetest ? "Not Use" : FuncInline.OtherRetestCount.ToString() + " times");
                        // Ready Command
                        dataGridSystem.Rows[5].Cells[1].Value = (FuncInline.UseSMDReady ? "Use" : "Not Use");
                        // Scan Twice and Compare
                        dataGridSystem.Rows[6].Cells[1].Value = (FuncInline.ScanTwice ? "Use" : "Not Use");
                        // Scan Input Mis
                        dataGridSystem.Rows[7].Cells[1].Value = (FuncInline.ScanInsertCheck ? "Use" : "Not Use");
                        // CN duplication
                        dataGridSystem.Rows[8].Cells[1].Value = (FuncInline.CheckCNDuplication ? "Use" : "Not Use");
                        // CN cross
                        dataGridSystem.Rows[9].Cells[1].Value = (FuncInline.CheckCNCross ? "Use" : "Not Use");
                        // NG To Unloading
                        dataGridSystem.Rows[10].Cells[1].Value = (FuncInline.NGToUnloading ? "Use" : "Not Use");
                        // PASS To NG
                        dataGridSystem.Rows[11].Cells[1].Value = (FuncInline.PassToNG ? "Use" : "Not Use");
                        // Leave One Site Empty
                        dataGridSystem.Rows[12].Cells[1].Value = (FuncInline.LeaveOneSite ? "Use" : "Not Use");
                        // SystemLog
                        //lblSettingLog.Text = FuncInline.PinLogDirectory;
                        #endregion

                        #region 메인메뉴버튼 이미지
                        if (beforeTab != FuncInline.enumTabMain.PartClear &&
                            FuncInline.TabMain == FuncInline.enumTabMain.PartClear)
                        {
                            tcMain.SelectedIndex = (int)FuncInline.enumTabMain.PartClear;
                        }
                        bool tabChange = beforeTab != FuncInline.TabMain;
                        if (tabChange)
                        {
                            ClearSubMenu();
                            tcMain.SelectedIndex = (int)FuncInline.TabMain;
                            switch (FuncInline.TabMain)
                            {
                                case FuncInline.enumTabMain.Auto:
                                    pbAuto.BackgroundImage = Radix.Properties.Resources.sub_main_sel;
                                    break;
                                case FuncInline.enumTabMain.Manual:
                                    pbManual.BackgroundImage = Radix.Properties.Resources.sub_manual_sel;
                                    break;
                                case FuncInline.enumTabMain.IO:
                                    //pbIOMonitor.BackgroundImage = Radix.Properties.Resources.sub_io_sel;
                                    break;
                                case FuncInline.enumTabMain.Machine:
                                    pbTower.BackgroundImage = Radix.Properties.Resources.sub_system_sel;
                                    break;
                                case FuncInline.enumTabMain.Model:
                                    pbModel.BackgroundImage = Radix.Properties.Resources.sub_model_sel;
                                    break;
                                case FuncInline.enumTabMain.Teaching:
                                    pbTeaching.BackgroundImage = Radix.Properties.Resources.sub_teaching_sel;
                                    break;
                                case FuncInline.enumTabMain.Trace:
                                    pbTrace.BackgroundImage = Radix.Properties.Resources.sub_trace_sel;
                                    break;
                                case FuncInline.enumTabMain.PartClear:
                                    pbPartClear.BackgroundImage = Properties.Resources.part_clear_bright;
                                    //frmPartClear.RefreshAlarm();
                                    break;
                                case FuncInline.enumTabMain.Origin:
                                    pbInit.BackgroundImage = Properties.Resources.Origin_bright;
                                    //frmPartClear.RefreshAlarm();
                                    break;
                                case FuncInline.enumTabMain.Errors:
                                    //frmLogViewer.SetDate();
                                    //frmLogViewer.Refresh();
                                    break;
                            }
                        }
                        beforeTab = FuncInline.TabMain;
                        #endregion

                        #region 버튼 이미지 표시
                        //*
                        if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun &&
                           GlobalVar.SystemStatus != enumSystemStatus.ErrorStop)
                        {
                            pbStart.BackgroundImage = Radix.Properties.Resources.start_green;
                            if (FuncInline.CycleStop)
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
                                                    /* GlobalVar.SystemStatus >= enumSystemStatus.AutoRun ? Properties.Resources.errors_gray : //*/Radix.Properties.Resources.errors;

                        pbCycleStop.BackgroundImage = FuncInline.CycleStop ? Properties.Resources.cycle_stop_red : Properties.Resources.cycle_stop;
                        FuncForm.SetButtonColor2(btnDoor, GlobalVar.UseDoor);
                        FuncForm.SetButtonColor2(btnSimulationMode, FuncInline.TestPassMode == FuncInline.enumSMDStatus.Test_Pass);
                        btnSimulationMode.Text = FuncInline.TestPassMode == FuncInline.enumSMDStatus.Test_Pass ? "Pass" : "Fail";
                        FuncForm.SetButtonColor2(btnEStop, !DIO.GetDIData(FuncInline.enumDINames.X00_3_Emergency_Stop));
                        //FuncForm.SetButtonColor2(btnInput, FuncInline.InputPCB);
                        btnInput.BackColor = FuncInline.InputPCB ? Color.Lime :
                                            FuncInline.InShuttleAction != FuncInline.enumShuttleAction.Waiting || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.UnKnown ? Color.Silver :
                                            Color.White;
                        FuncForm.SetButtonColor2(btnInputStop1, FuncInline.InputStop[0]);
                        FuncForm.SetButtonColor2(btnInputStop2, FuncInline.InputStop[1]);
                    
                        FuncForm.SetButtonColor2(btnOutputStop, FuncInline.OutputStop);
                        bool doorOpened = !DIO.GetDIData(FuncInline.enumDINames.X00_0_Door_Open_Front_Left) ||
                                            !DIO.GetDIData(FuncInline.enumDINames.X00_1_Door_Open_Front_Right) ||
                                            !DIO.GetDIData(FuncInline.enumDINames.X00_2_Door_Open_Rear_Left) ||
                                            !DIO.GetDIData(FuncInline.enumDINames.X02_0_Door_Open_Rear_Right);
                        FuncForm.SetButtonColor2(btnDoorCheck, doorOpened);
                        //btnDoorCheck.Visible = !GlobalVar.UseDoor ||
                        //                        GlobalVar.SystemStatus == enumSystemStatus.ErrorStop ||
                        //                        !(GlobalVar.SystemStatus == enumSystemStatus.Initialize ||
                        //                            GlobalVar.SystemStatus >= enumSystemStatus.AutoRun);
                        if (doorOpened)
                        {
                            btnDoorCheck.Text = "Door Opened";
                        }
                        else
                        {
                            btnDoorCheck.Text = "Door Closed";
                        }
                        //*/
                        #endregion

                        #region 타워램프 테스트
#if DEBUG
                        lblRed.Visible = false; // GlobalVar.Simulation && DIO.GetDORead((int)FuncInline.enumDONames.Y4_4_Tower_Lamp_Red);
                        lblGreen.Visible = false; // GlobalVar.Simulation && DIO.GetDORead((int)FuncInline.enumDONames.Y4_3_Tower_Lamp_Green);
                        lblYellow.Visible = false; // GlobalVar.Simulation && DIO.GetDORead((int)FuncInline.enumDONames.Y412_3_Tower_Lamp_Yellow);
                        lblBuzzer.Visible = false; // GlobalVar.Simulation && DIO.GetDORead((int)FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer);
                        //lblRed.Visible = GlobalVar.Simulation && DIO.GetDORead((int)FuncInline.enumDONames.Y4_4_Tower_Lamp_Red);
                        //lblGreen.Visible = GlobalVar.Simulation && DIO.GetDORead((int)FuncInline.enumDONames.Y4_3_Tower_Lamp_Green);
                        //lblYellow.Visible = GlobalVar.Simulation && DIO.GetDORead((int)FuncInline.enumDONames.Y412_3_Tower_Lamp_Yellow);
                        //lblBuzzer.Visible = GlobalVar.Simulation && DIO.GetDORead((int)FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer);

                        btnMakeCN.Visible = true;
                        btnDeleteSQLLog.Visible = true;
                    

#endif
#if !DEBUG
                        lblRed.Visible = false;
                        lblGreen.Visible = false;
                        lblYellow.Visible = false;
                        lblBuzzer.Visible = false;

                        btnMakeCN.Visible = false;
                        btnDeleteSQLLog.Visible = false;

                        lblMuting.Visible = false;
#endif
                        #endregion

            
                        #region NG 리스트
                        /*
                        if (GlobalVar.NgQueue.Count != dataGridNG.Rows.Count - 1)
                        {
                            dataGridNG.Rows.Clear();
                            for (int i = GlobalVar.NgQueue.Count - 1; i >= 0; i--)
                            {
                                dataGridNG.Rows.Add(GlobalVar.NgQueue.ElementAt(i));
                            }
                        }
                        //*/
                        #endregion


                        if (FuncInline.TabMain != FuncInline.enumTabMain.Auto)
                        {
                            //if (!GlobalVar.GlobalStop)
                            //{
                            //Thread.Sleep(GlobalVar.ThreadSleep);
                            //timerUI = new System.Threading.Timer(new TimerCallback(TimerUI), false, 0, 100);
                            //}
                            //timerDoing = false;

                            return;
                        }


                        #region 모듈형 오토인라인 site 테스트 정보
                        #region In Shuttle

                        lblInShuttleAction.Text = FuncInline.InShuttleAction.ToString() + (FuncInline.InShuttleAction == FuncInline.enumShuttleAction.Cooling ? "(" + (FuncInline.CoolingWatch.ElapsedMilliseconds / 1000).ToString("F0") + ")" : "");
                        pnInShuttleTitle.BackColor = FuncError.CheckError(FuncInline.enumErrorPart.InShuttle) ? Color.Red :
                                                    FuncInline.InShuttleAction == FuncInline.enumShuttleAction.Waiting ? Color.Turquoise : Color.Lime;
                        pnInShuttle.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                                        !DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) &&
                                                        !DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) &&
                                                        !DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor)//인컨베이어-인셔틀 인터록센서
                                                        ? Color.Transparent 
                                                        : Color.Yellow;
                        //string arrayList = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InputLift].PCBStatus.ToString() + "\n";
                        string arrayList = "";
                        int arrayIndex = 1;
                        for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].Barcode.Length; i++)
                        {
                            if (FuncInline.ArrayUse[i] &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].Barcode[i] != null &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].Barcode[i].Length > 0)
                            {
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].Xout[i])
                                {
                                    arrayList += (i + 1) + ".XOut\n";
                                }
                                else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].BadMark[i])
                                {
                                    arrayList += (i + 1) + ".Bad\n";
                                }
                                else
                                {
                                    arrayList += (i + 1) + "." + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].Barcode[i] + "\n";
                                }
                                arrayIndex++;
                            }
                        }
                        lblInShuttleArray1.Text = arrayList;
                        #endregion
                
                        #region Out Shuttle UP
                        lblOutShuttleUpAction.Text = FuncInline.OutShuttleUpAction.ToString() +
                                                        (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown ?
                                                                "(" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].TestSite[0].ToString() + ")" : "");
                        pnOutShuttleUpTitle.BackColor = FuncError.CheckError(FuncInline.enumErrorPart.OutShuttle_Up) ? Color.Red :
                                                     FuncInline.OutShuttleUpAction == FuncInline.enumShuttleAction.Waiting ? Color.Turquoise : Color.Lime;
                        pnOutShuttleUp.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Fail || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].PCBStatus == FuncInline.enumSMDStatus.User_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ? Color.Tomato : // NG 경우 적색
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor) &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor)
                                                                ? Color.Transparent  // PCB 없으면 투명
                                                                : Color.Yellow; // 나머지 노랑

                        arrayList = ""; // FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutputLift].PCBStatus.ToString() + "\n";
                        arrayIndex = 1;
                        for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].Barcode.Length; i++)
                        {
                            if (FuncInline.ArrayUse[i] &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].Barcode[i].Length > 0)
                            {
                                string code = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].Barcode[i];
                                if (code.Length > 6)
                                {
                                    code = code.Substring(code.Length - 6, 6);
                                }
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].Xout[i])
                                {
                                    arrayList += (i + 1) + ".XOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].BadMark[i])
                                {
                                    arrayList += (i + 1) + ".Bad " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Pass)
                                {
                                    arrayList += (i + 1) + ".Pass " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    // NG 코드와 에러명 출력
                                    string errorName = "no ";
                                    try
                                    {
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].ErrorCode[i] > -1)
                                        {
                                            errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].ErrorCode[i]];
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //debug(xx.ToString());
                                        //debug(xx.StackTrace);
                                    }
                                    if (errorName.Length > 4)
                                    {
                                        errorName = errorName.Substring(0, 4);
                                    }
                                    arrayList += (i + 1) + "." + errorName + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Cancel ||
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].SMDStatus[i] == FuncInline.enumSMDStatus.User_Cancel)
                                {
                                    arrayList += (i + 1) + ".Cncl " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                                {
                                    arrayList += (i + 1) + ".TMOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else
                                {
                                    arrayList += (i + 1) + "." + code + " ";
                                }

                                arrayIndex++;
                            }
                        }
                        lblOutShuttleUp.Text = arrayList;
                        #endregion

                        #region Out Shuttle Down
                        lblOutShuttleDownAction.Text = FuncInline.OutShuttleDownAction.ToString() +
                                                        (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown ?
                                                                "(" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].TestSite[0].ToString() + ")" : "");
                        pnOutShuttleDownTitle.BackColor = FuncError.CheckError(FuncInline.enumErrorPart.OutShuttle_Down) ? Color.Red :
                                                     FuncInline.OutShuttleDownAction == FuncInline.enumShuttleAction.Waiting ? Color.Turquoise : Color.Lime;
                        pnOutShuttleDown.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Fail || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].PCBStatus == FuncInline.enumSMDStatus.User_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ? Color.Tomato : // NG 경우 적색
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor) &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor)
                                                                ? Color.Transparent // PCB 없으면 투명
                                                                : Color.Yellow; // 나머지 노랑

                        arrayList = ""; // FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutputLift].PCBStatus.ToString() + "\n";
                        arrayIndex = 1;
                        for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].Barcode.Length; i++)
                        {
                            if (FuncInline.ArrayUse[i] &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].Barcode[i].Length > 0)
                            {
                                string code = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].Barcode[i];
                                if (code.Length > 6)
                                {
                                    code = code.Substring(code.Length - 6, 6);
                                }
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].Xout[i])
                                {
                                    arrayList += (i + 1) + ".XOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].BadMark[i])
                                {
                                    arrayList += (i + 1) + ".Bad " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Pass)
                                {
                                    arrayList += (i + 1) + ".Pass " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    // NG 코드와 에러명 출력
                                    string errorName = "no ";
                                    try
                                    {
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].ErrorCode[i] > -1)
                                        {
                                            errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].ErrorCode[i]];
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //debug(xx.ToString());
                                        //debug(xx.StackTrace);
                                    }
                                    if (errorName.Length > 4)
                                    {
                                        errorName = errorName.Substring(0, 4);
                                    }
                                    arrayList += (i + 1) + "." + errorName + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Cancel ||
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].SMDStatus[i] == FuncInline.enumSMDStatus.User_Cancel)
                                {
                                    arrayList += (i + 1) + ".Cncl " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                                {
                                    arrayList += (i + 1) + ".TMOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else
                                {
                                    arrayList += (i + 1) + "." + code + " ";
                                }

                                arrayIndex++;
                            }
                        }
                        lblOutShuttleDown.Text = arrayList;
                        #endregion

                        #region Lift 1
                        lblLift1UpAction.Text = FuncInline.Lift1Action.ToString() +
                                                        (FuncInline.Lift1Action >= FuncInline.enumLiftAction.LoadingUp && FuncInline.Lift1Action <= FuncInline.enumLiftAction.UnloadingUp
                                                                ? $"({FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination.ToString().Replace("Site", "")})"
                                                                : "");
                        pnLift1Title.BackColor = FuncError.CheckError(FuncInline.enumErrorPart.Lift1_Up) ? Color.Red :
                                                            FuncInline.Lift1Action == FuncInline.enumLiftAction.Waiting ? Color.Turquoise : Color.Lime;
                        #region 상단
                        //배경 상태표시
                        pnLift1Up.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Fail || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.User_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ? Color.Tomato : // NG 경우 적색
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor) ? Color.Transparent : // PCB 없으면 투명
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].SelfRetestCount > 0 || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].OtherRetestCount > 0 ? Color.Pink :
                                                      Color.Yellow; // 나머지 노랑
                        arrayList = "";// FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.UnloadiNgShuttle].PCBStatus.ToString() + "\n";
                        arrayIndex = 1;
                        for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Barcode.Length; i++)
                        {
                            if (FuncInline.ArrayUse[i] &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Barcode[i].Length > 0)
                            {
                                string code = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Barcode[i];
                                if (code.Length > 6)
                                {
                                    code = code.Substring(code.Length - 6, 6);
                                }
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Xout[i])
                                {
                                    arrayList += (i + 1) + ".XOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].BadMark[i])
                                {
                                    arrayList += (i + 1) + ".Bad " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Pass)
                                {
                                    arrayList += (i + 1) + ".Pass " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    // NG 코드와 에러명 출력
                                    string errorName = "no";
                                    try
                                    {
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].ErrorCode[i] > -1)
                                        {
                                            errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].ErrorCode[i]];
                                        }
                                    }
                                    catch (Exception xx)
                                    {
                                        //debug(xx.ToString());
                                        //debug(xx.StackTrace);
                                    }
                                    if (errorName.Length > 4)
                                    {
                                        errorName = errorName.Substring(0, 4);
                                    }
                                    arrayList += (i + 1) + "." + errorName + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Cancel ||
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].SMDStatus[i] == FuncInline.enumSMDStatus.User_Cancel)
                                {
                                    arrayList += (i + 1) + ".Cncl " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                                {
                                    arrayList += (i + 1) + ".TMOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else
                                {
                                    arrayList += (i + 1) + "." + code + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }

                                arrayIndex++;
                            }
                        }
                        lblLift1UpArray1.Text = arrayList;
                        #endregion
                        #region 하단
                        //pnLift1Down.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Fail || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.User_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ? Color.Tomato : // NG 경우 적색
                        //                              FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                        //                              FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                        //                                        !DIO.GetDIData(FuncInline.enumDINames.X12_2_Lift1_Down_Start_Sensor) &&
                        //                                        !DIO.GetDIData(FuncInline.enumDINames.X12_3_Lift1_Down_Stop_Sensor) ? Color.Transparent : // PCB 없으면 투명
                        //                              FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].SelfRetestCount > 0 || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].OtherRetestCount > 0 ? Color.Pink :
                        //                              Color.Yellow; // 나머지 노랑
                        //arrayList = "";// FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.UnloadiNgShuttle].PCBStatus.ToString() + "\n";
                        //arrayIndex = 1;
                        //for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Barcode.Length; i++)
                        //{
                        //    if (FuncInline.ArrayUse[i] &&
                        //        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Barcode[i].Length > 0)
                        //    {
                        //        string code = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Barcode[i];
                        //        if (code.Length > 6)
                        //        {
                        //            code = code.Substring(code.Length - 6, 6);
                        //        }
                        //        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Xout[i])
                        //        {
                        //            arrayList += (i + 1) + ".XOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].BadMark[i])
                        //        {
                        //            arrayList += (i + 1) + ".Bad " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Pass)
                        //        {
                        //            arrayList += (i + 1) + ".Pass " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail)
                        //        {
                        //            // NG 코드와 에러명 출력
                        //            string errorName = "no";
                        //            try
                        //            {
                        //                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].ErrorCode[i] > -1)
                        //                {
                        //                    errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].ErrorCode[i]];
                        //                }
                        //            }
                        //            catch (Exception xx)
                        //            {
                        //                //debug(xx.ToString());
                        //                //debug(xx.StackTrace);
                        //            }
                        //            if (errorName.Length > 4)
                        //            {
                        //                errorName = errorName.Substring(0, 4);
                        //            }
                        //            arrayList += (i + 1) + errorName + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Cancel ||
                        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].SMDStatus[i] == FuncInline.enumSMDStatus.User_Cancel)
                        //        {
                        //            arrayList += (i + 1) + ".Cncl " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                        //        {
                        //            arrayList += (i + 1) + ".TMOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else
                        //        {
                        //            arrayList += (i + 1) + "." + code + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        arrayIndex++;
                        //    }
                        //}
                        //lblLift1DownArray1.Text = arrayList;
                        #endregion
                        #endregion

                        #region Lift 2
                        lblLift2UpAction.Text = FuncInline.Lift2Action.ToString() +
                                                        (FuncInline.Lift2Action >= FuncInline.enumLiftAction.LoadingUp && FuncInline.Lift2Action <= FuncInline.enumLiftAction.UnloadingUp 
                                                                  ? $"({FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination.ToString().Replace("Site", "")})"
                                                                : "");
                        pnLift2Title.BackColor = FuncError.CheckError(FuncInline.enumErrorPart.Lift2_Up) ? Color.Red :
                                                            FuncInline.Lift2Action == FuncInline.enumLiftAction.Waiting ? Color.Turquoise : Color.Lime;
                        #region 상단
                        pnLift2Up.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Fail || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.User_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ? Color.Tomato : // NG 경우 적색
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) &&
                                                                !DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor) ? Color.Transparent : // PCB 없으면 투명
                                                      FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].SelfRetestCount > 0 || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].OtherRetestCount > 0 ? Color.Pink :
                                                      Color.Yellow; // 나머지 노랑
                        arrayList = "";// FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.UnloadiNgShuttle].PCBStatus.ToString() + "\n";
                        arrayIndex = 1;
                        for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Barcode.Length; i++)
                        {
                            if (FuncInline.ArrayUse[i] &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Barcode[i].Length > 0)
                            {
                                string code = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Barcode[i];
                                if (code.Length > 6)
                                {
                                    code = code.Substring(code.Length - 6, 6);
                                }
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Xout[i])
                                {
                                    arrayList += (i + 1) + ".XOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].BadMark[i])
                                {
                                    arrayList += (i + 1) + ".Bad " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Pass)
                                {
                                    arrayList += (i + 1) + ".Pass " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    // NG 코드와 에러명 출력
                                    string errorName = "no";
                                    try
                                    {
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].ErrorCode[i] > -1)
                                        {
                                            errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].ErrorCode[i]];
                                        }
                                    }
                                    catch (Exception xx)
                                    {
                                        //debug(xx.ToString());
                                        //debug(xx.StackTrace);
                                    }
                                    if (errorName.Length > 4)
                                    {
                                        errorName = errorName.Substring(0, 4);
                                    }
                                    arrayList += (i + 1) + errorName + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Cancel ||
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].SMDStatus[i] == FuncInline.enumSMDStatus.User_Cancel)
                                {
                                    arrayList += (i + 1) + ".Cncl " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                                {
                                    arrayList += (i + 1) + ".TMOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else
                                {
                                    arrayList += (i + 1) + "." + code + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                arrayIndex++;
                            }
                        }
                        lblLift2UpArray1.Text = arrayList;
                        #endregion
                        #region 하단
                        //pnLift2Down.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Fail || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.User_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ? Color.Tomato : // NG 경우 적색
                        //                              FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                        //                              FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                        //                                        !DIO.GetDIData(FuncInline.enumDINames.X14_2_Lift2_Down_Start_Sensor) &&
                        //                                        !DIO.GetDIData(FuncInline.enumDINames.X14_3_Lift2_Down_Stop_Sensor) ? Color.Transparent : // PCB 없으면 투명
                        //                              FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].SelfRetestCount > 0 || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].OtherRetestCount > 0 ? Color.Pink :
                        //                              Color.Yellow; // 나머지 노랑
                        //arrayList = "";// FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.UnloadiNgShuttle].PCBStatus.ToString() + "\n";
                        //arrayIndex = 1;
                        //for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Barcode.Length; i++)
                        //{
                        //    if (FuncInline.ArrayUse[i] &&
                        //        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Barcode[i].Length > 0)
                        //    {
                        //        string code = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Barcode[i];
                        //        if (code.Length > 6)
                        //        {
                        //            code = code.Substring(code.Length - 6, 6);
                        //        }
                        //        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Xout[i])
                        //        {
                        //            arrayList += (i + 1) + ".XOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].BadMark[i])
                        //        {
                        //            arrayList += (i + 1) + ".Bad " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Pass)
                        //        {
                        //            arrayList += (i + 1) + ".Pass " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail)
                        //        {
                        //            // NG 코드와 에러명 출력
                        //            string errorName = "no";
                        //            try
                        //            {
                        //                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].ErrorCode[i] > -1)
                        //                {
                        //                    errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].ErrorCode[i]];
                        //                }
                        //            }
                        //            catch (Exception xx)
                        //            {
                        //                //debug(xx.ToString());
                        //                //debug(xx.StackTrace);
                        //            }
                        //            if (errorName.Length > 4)
                        //            {
                        //                errorName = errorName.Substring(0, 4);
                        //            }
                        //            arrayList += (i + 1) + errorName + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Cancel ||
                        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].SMDStatus[i] == FuncInline.enumSMDStatus.User_Cancel)
                        //        {
                        //            arrayList += (i + 1) + ".Cncl " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                        //        {
                        //            arrayList += (i + 1) + ".TMOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        else
                        //        {
                        //            arrayList += (i + 1) + "." + code + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                        //        }
                        //        arrayIndex++;
                        //    }
                        //}
                        //lblLift2DownArray1.Text = arrayList;
                        #endregion
                        #endregion

                        #region FrontPassLine
                        lblPassLine1Action.Text = FuncInline.FrontPassLineAction.ToString();
                        pnPassLine1Title.BackColor = FuncError.CheckError(FuncInline.enumErrorPart.FrontPassLine) ? Color.Red :
                                                        FuncInline.FrontPassLineAction == FuncInline.enumLiftAction.Waiting ? Color.Turquoise : Color.Lime;
                        pnPassLine1.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                                             !DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor)
                                                    ? Color.Transparent : Color.Yellow;
                        arrayList = "";// FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.BufferConveyor].PCBStatus.ToString() + "\n";
                        arrayIndex = 1;
                        for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].Barcode.Length; i++)
                        {
                            if (FuncInline.ArrayUse[i] &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].Barcode[i].Length > 0)
                            {
                                string code = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].Barcode[i];
                                if (code.Length > 6)
                                {
                                    code = code.Substring(code.Length - 6, 6);
                                }
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].Xout[i])
                                {
                                    arrayList += (i + 1) + ".XOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].BadMark[i])
                                {
                                    arrayList += (i + 1) + ".Bad " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else
                                {
                                    arrayList += (i + 1) + "." + code + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }

                                arrayIndex++;
                            }
                        }
                        lblFrontPassLineArray1.Text = arrayList;
                        #endregion

                        #region RearPassLine
                        lblRearPassLineAction.Text = FuncInline.RearPassLineAction.ToString() +
                                                        (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown ?
                                                                "(" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].TestSite[0].ToString() + ")" : "");
                        pnRearPassLineTitle.BackColor = FuncError.CheckError(FuncInline.enumErrorPart.RearPassLine) ? Color.Red :
                                                            FuncInline.RearPassLineAction == FuncInline.enumLiftAction.Waiting ? Color.Turquoise : Color.Lime;
                        pnRearPassLine.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].PCBStatus == FuncInline.enumSMDStatus.Test_Fail || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].PCBStatus == FuncInline.enumSMDStatus.User_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout 
                                                        ? Color.Tomato // NG 경우 적색
                                                        : FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                                        !DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor) ? Color.Transparent : // PCB 없으면 투명
                                                      Color.Yellow; // 나머지 노랑
                        arrayList = "";// FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.UnloadiNgShuttle].PCBStatus.ToString() + "\n";
                        arrayIndex = 1;
                        for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].Barcode.Length; i++)
                        {
                            if (FuncInline.ArrayUse[i] &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].Barcode[i].Length > 0)
                            {
                                string code = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].Barcode[i];
                                if (code.Length > 6)
                                {
                                    code = code.Substring(code.Length - 6, 6);
                                }
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].Xout[i])
                                {
                                    arrayList += (i + 1) + ".XOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].BadMark[i])
                                {
                                    arrayList += (i + 1) + ".Bad " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Pass)
                                {
                                    arrayList += (i + 1) + ".Pass " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    // NG 코드와 에러명 출력
                                    string errorName = "no";
                                    try
                                    {
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].ErrorCode[i] > -1)
                                        {
                                            errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].ErrorCode[i]];
                                        }
                                    }
                                    catch (Exception xx)
                                    {
                                        //debug(xx.ToString());
                                        //debug(xx.StackTrace);
                                    }
                                    if (errorName.Length > 6)
                                    {
                                        errorName = errorName.Substring(0, 6);
                                    }
                                    arrayList += (i + 1) + errorName + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Cancel ||
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].SMDStatus[i] == FuncInline.enumSMDStatus.User_Cancel)
                                {
                                    arrayList += (i + 1) + ".Cancel " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                                {
                                    arrayList += (i + 1) + ".TMOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else
                                {
                                    arrayList += (i + 1) + "." + code + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                arrayIndex++;
                            }
                        }
                        lblRearPassLine.Text = arrayList;
                        #endregion

                        #region NGPassLine
                        lblRearNGLineAction.Text = FuncInline.RearNGLineAction.ToString() +
                                                        (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown ?
                                                                "(" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].TestSite[0].ToString() + ")" : "");
                        pnRearNGLineTitle.BackColor = FuncError.CheckError(FuncInline.enumErrorPart.RearNGLine) ? Color.Red :
                                                            FuncInline.RearNGLineAction == FuncInline.enumLiftAction.Waiting ? Color.Turquoise : Color.Lime;
                        pnRearNGLine.BackColor = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].PCBStatus == FuncInline.enumSMDStatus.Test_Fail || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].PCBStatus == FuncInline.enumSMDStatus.User_Cancel || FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout
                                                        ? Color.Tomato // NG 경우 적색
                                                        : FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                                        !DIO.GetDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor) ? Color.Transparent : // PCB 없으면 투명
                                                      Color.Yellow; // 나머지 노랑
                        arrayList = "";// FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.UnloadiNgShuttle].PCBStatus.ToString() + "\n";
                        arrayIndex = 1;
                        for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].Barcode.Length; i++)
                        {
                            if (FuncInline.ArrayUse[i] &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].Barcode[i].Length > 0)
                            {
                                string code = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].Barcode[i];
                                if (code.Length > 6)
                                {
                                    code = code.Substring(code.Length - 6, 6);
                                }
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].Xout[i])
                                {
                                    arrayList += (i + 1) + ".XOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].BadMark[i])
                                {
                                    arrayList += (i + 1) + ".Bad " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Pass)
                                {
                                    arrayList += (i + 1) + ".Pass " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    // NG 코드와 에러명 출력
                                    string errorName = "no";
                                    try
                                    {
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].ErrorCode[i] > -1)
                                        {
                                            errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].ErrorCode[i]];
                                        }
                                    }
                                    catch (Exception xx)
                                    {
                                        //debug(xx.ToString());
                                        //debug(xx.StackTrace);
                                    }
                                    if (errorName.Length > 6)
                                    {
                                        errorName = errorName.Substring(0, 6);
                                    }
                                    arrayList += (i + 1) + errorName + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Cancel ||
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].SMDStatus[i] == FuncInline.enumSMDStatus.User_Cancel)
                                {
                                    arrayList += (i + 1) + ".Cancel " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                                {
                                    arrayList += (i + 1) + ".TMOut " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                //정상일때
                                else
                                {
                                    arrayList += (i + 1) + "." + code + " " + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                }
                                arrayIndex++;
                            }
                        }
                        lblRearNGLine.Text = arrayList;
                        #endregion




                        #region 모델 Array Layout 표시

                        //if (FuncInline.ArrayBitmap == null)
                        //{
                        //    pbArrayImage.BackgroundImage = null; // Properties.Resources.no_image;
                        //}
                        //else
                        //{
                        //    pbArrayImage.BackgroundImage = FuncInline.ArrayBitmap;
                        //    // 비전 사이즈 8192,5450
                        //    // PCB 91x185mm  4345,2849 ==> 7257,5225
                        //    // Array1 : 4498,4445
                        //    // Array2 : 6917,4322
                        //    // Array4 : 4500,3725
                        //    // Array5 : 6919,5029
                        //    // Array7 : 4505,3028
                        //    // Array10 : 6921,3597

                        //    #region 테스트용 좌표 임시할당
                        //    //FuncInline.Scan.ScanPos[0].xPos = 4498;
                        //    //FuncInline.Scan.ScanPos[0].yPos = 4445;
                        //    //FuncInline.Scan.ScanPos[1].xPos = 6917;
                        //    //FuncInline.Scan.ScanPos[1].yPos = 4322;
                        //    //FuncInline.Scan.ScanPos[3].xPos = 4500;
                        //    //FuncInline.Scan.ScanPos[3].yPos = 3725;
                        //    //FuncInline.Scan.ScanPos[4].xPos = 6919;
                        //    //FuncInline.Scan.ScanPos[4].yPos = 5029;
                        //    //FuncInline.Scan.ScanPos[6].xPos = 4505;
                        //    //FuncInline.Scan.ScanPos[6].yPos = 3028;
                        //    //FuncInline.Scan.ScanPos[9].xPos = 6921;
                        //    //FuncInline.Scan.ScanPos[9].yPos = 3597;
                        //    #endregion

                        //}
                        //#region 어레이인덱스 표시
                        ////DefaultPCBWidth, FuncInline.PCBWidth
                        ////DefaultPCBLength, FuncInline.PCBLength
                        //// 이미지는 부분 추출된 것이므로 영역을 다시 계산해야 한다.
                        //// 반대로 잘려나갔을 부분 고려해서 원본의 위치와 크기 기준으로 포인트를 계산하면 될듯
                        //Point arrayImageLocation = pbArrayImage.Location;
                        ////debug("image location : " + arrayImageLocation.X + "," + arrayImageLocation.Y);
                        //Size arrayImageSize = pbArrayImage.Size;
                        ////debug("image size : " + arrayImageSize.Width + "," + arrayImageSize.Height);
                        //Point originalImageLocation = pbArrayImage.Location;
                        //Size originalImageSize = pbArrayImage.Size;
                        //originalImageSize.Width = (int)(arrayImageSize.Width * FuncInline.DefaultPCBLength / FuncInline.PCBLength);
                        //originalImageSize.Height = (int)(arrayImageSize.Height * FuncInline.DefaultPCBWidth / FuncInline.PCBWidth);
                        //originalImageLocation.X -= originalImageSize.Width - arrayImageSize.Width;
                        //originalImageLocation.Y -= originalImageSize.Height - arrayImageSize.Height;
                        ////debug("original size : " + originalImageSize.Width + "," + originalImageSize.Height);
                        ////debug("original location : " + originalImageLocation.X + "," + originalImageLocation.Y);

                        //// 전체 출력 어레이 배치 범위를 산정하고
                        //// 범위에서 위치의 비율로 배치한다.
                        //// 우측/하단 범위는 30*30 만큼 빼고 생각한다.
                        //Point startArray = new Point(); // 전체 범위의 시작점
                        //Point endArray = new Point(); // 전체 범위의 끝점
                        //#region 출력 범위 산정
                        //for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                        //{
                        //    if (!FuncInline.PassMode &&
                        //        FuncInline.ArrayUse[i] &&
                        //        FuncInline.Scan.ScanPos[i].xPos > 0 &&
                        //        FuncInline.Scan.ScanPos[i].yPos > 0)
                        //    {
                        //        if (startArray.X == 0 ||
                        //            FuncInline.Scan.ScanPos[i].xPos < startArray.X)
                        //        {
                        //            startArray.X = (int)FuncInline.Scan.ScanPos[i].xPos;
                        //        }
                        //        if (startArray.Y == 0 ||
                        //            FuncInline.Scan.ScanPos[i].yPos < startArray.Y)
                        //        {
                        //            startArray.Y = (int)FuncInline.Scan.ScanPos[i].yPos;
                        //        }
                        //        if (endArray.X == 0 ||
                        //            FuncInline.Scan.ScanPos[i].xPos > endArray.X)
                        //        {
                        //            endArray.X = (int)FuncInline.Scan.ScanPos[i].xPos;
                        //        }
                        //        if (endArray.Y == 0 ||
                        //            FuncInline.Scan.ScanPos[i].yPos > endArray.Y)
                        //        {
                        //            endArray.Y = (int)FuncInline.Scan.ScanPos[i].yPos;
                        //        }
                        //    }
                        //}
                        ////debug("startArray : " + startArray.X + "," + startArray.Y);
                        ////debug("endArray : " + endArray.X + "," + endArray.Y);
                        //#endregion


                        //for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                        //{
                        //    PictureBox array = (PictureBox)Controls.Find("pbArray" + (i + 1), true)[0];
                        //    if (!FuncInline.PassMode &&
                        //        FuncInline.ArrayUse[i] &&
                        //        FuncInline.Scan.ScanArrayIndex[i] > -1 &&
                        //        FuncInline.Scan.ScanPos[FuncInline.Scan.ScanArrayIndex[i]].xPos > 0 &&
                        //        FuncInline.Scan.ScanPos[FuncInline.Scan.ScanArrayIndex[i]].yPos > 0)
                        //    {
                        //        Point arrayPoint = new Point();
                        //        arrayPoint.X = (int)(arrayImageLocation.X + 10 + (arrayImageSize.Width - 50) * (FuncInline.Scan.ScanPos[FuncInline.Scan.ScanArrayIndex[i]].xPos - startArray.X) / (endArray.X - startArray.X));
                        //        arrayPoint.Y = (int)(arrayImageLocation.Y + 10 + (arrayImageSize.Height - 50) * (FuncInline.Scan.ScanPos[FuncInline.Scan.ScanArrayIndex[i]].yPos - startArray.Y) / (endArray.Y - startArray.Y));
                        //        //arrayPoint.X = (int)(originalImageLocation.X + originalImageSize.Width * (FuncInline.Scan.ScanPos[i].xPos - FuncInline.ScanSize.x) / (FuncInline.ScanSize.z - FuncInline.ScanSize.x) * 0.7);
                        //        //arrayPoint.Y = (int)(originalImageLocation.Y + originalImageSize.Height * (FuncInline.Scan.ScanPos[i].yPos - FuncInline.ScanSize.y) / (FuncInline.ScanSize.a - FuncInline.ScanSize.y) + 10);
                        //        array.Location = arrayPoint;
                        //        array.Visible = true;
                        //        //debug((i + 1).ToString() + " : " + arrayPoint.X + "," + arrayPoint.Y);
                        //    }
                        //    else
                        //    {
                        //        array.Visible = false;
                        //    }
                        //}
                        //#endregion

                        #endregion


                   

                      


                        #region 시뮬레이션용 버튼 표시
                        FuncForm.SetButtonColor2(btnPCBInput, simulationPCBInput);
                        FuncForm.SetButtonColor2(btnSmemaAfter, DIO.GetDIData(FuncInline.enumDINames.X02_2_SMEMA_After_Ready));

                        FuncForm.SetButtonColor2(btnSmemaBeforeRun, DIO.GetDIData(FuncInline.enumDINames.X02_1_SMEMA_Before_Ready));
                        FuncForm.SetButtonColor2(btnSmemaBeforeTestPass, DIO.GetDIData(FuncInline.enumDINames.X00_6_SMEMA_Before_Pass));
                        FuncForm.SetButtonColor2(btnSmemaAfterAuto, DIO.GetDIData(FuncInline.enumDINames.X00_7_SMEMA_After_AutoInline));
                        FuncForm.SetButtonColor2(btnSmemaAfterReady, DIO.GetDIData(FuncInline.enumDINames.X02_2_SMEMA_After_Ready));
                        FuncForm.SetButtonColor2(btnSmemaBeforeReady, DIO.GetDORead((int)FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready));
                        FuncForm.SetButtonColor2(btnSmemaBeforeAuto, DIO.GetDORead((int)FuncInline.enumDONames.Y404_3_SMEMA_Before_AutoInline));
                        FuncForm.SetButtonColor2(btnSmemaAfterRun, DIO.GetDORead((int)FuncInline.enumDONames.Y412_1_SMEMA_After_Ready));
                        FuncForm.SetButtonColor2(btnSmemaAfterTestPass, DIO.GetDORead((int)FuncInline.enumDONames.Y404_5_SMEMA_After_Pass));
                        #endregion


                        #region 사이트 이외 PCB 유무 표시
                        #region 인 셔틀
                        if (DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ||
                                               DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
                                               DIO.GetDIData(FuncInline.enumDINames.X304_0_In_Shuttle_Interlock_Sensor))
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].BuyerChange &&
                                    FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBInShuttle.BackColor = Color.Blue;
                            }
                            else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].BuyerChange &&
                                    FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBInShuttle.BackColor = Color.Orange;
                            }
                            else
                            {
                                btnPCBInShuttle.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            btnPCBInShuttle.BackColor = Color.White;
                        }
                      
                        #endregion

                        #region NGbuffer
                        if (DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor))
                        {
                            btnPCBNgBuffer.BackColor = Color.Yellow;
                        }
                        else
                        {
                            btnPCBNgBuffer.BackColor = Color.White;
                        }
                        #endregion
                        #region 배출 셔틀
                        if (DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) ||
                            DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor))
                        {
                            btnPCBOutConveyor.BackColor = Color.Yellow;
                        }
                        else
                        {
                            btnPCBOutConveyor.BackColor = Color.White;
                        }
                     
                        #endregion

                        #region FrontLift Up
                        if (DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                            DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor))
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].BuyerChange &&
                                        FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBLift1Up.BackColor = Color.Blue;
                            }
                            else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].BuyerChange &&
                                        FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBLift1Up.BackColor = Color.Orange;
                            }
                            else
                            {
                                btnPCBLift1Up.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            btnPCBLift1Up.BackColor = Color.White;
                        }
                        #endregion
                        #region FrontLift Down
                        //if (DIO.GetDIData(FuncInline.enumDINames.X12_2_Lift1_Down_Start_Sensor) ||
                        //    DIO.GetDIData(FuncInline.enumDINames.X12_3_Lift1_Down_Stop_Sensor))
                        //{
                        //    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].BuyerChange &&
                        //                FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue)
                        //    {
                        //        btnPCBLift1Down.BackColor = Color.Blue;
                        //    }
                        //    else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].BuyerChange &&
                        //                FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue)
                        //    {
                        //        btnPCBLift1Down.BackColor = Color.Orange;
                        //    }
                        //    else
                        //    {
                        //        btnPCBLift1Down.BackColor = Color.Yellow;
                        //    }
                        //}
                        //else
                        //{
                        //    btnPCBLift1Down.BackColor = Color.White;
                        //}
                        #endregion
                        #region Lift2 Up
                        if (DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                            DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor))
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].BuyerChange &&
                                        FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBLift2Up.BackColor = Color.Blue;
                            }
                            else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].BuyerChange &&
                                        FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBLift2Up.BackColor = Color.Orange;
                            }
                            else
                            {
                                btnPCBLift2Up.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            btnPCBLift2Up.BackColor = Color.White;
                        }
                        #endregion
                        #region Lift2 Down
                        //if (DIO.GetDIData(FuncInline.enumDINames.X14_2_Lift2_Down_Start_Sensor) ||
                        //    DIO.GetDIData(FuncInline.enumDINames.X14_3_Lift2_Down_Stop_Sensor))
                        //{
                        //    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].BuyerChange &&
                        //                FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue)
                        //    {
                        //        btnPCBLift2Down.BackColor = Color.Blue;
                        //    }
                        //    else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].BuyerChange &&
                        //                FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue)
                        //    {
                        //        btnPCBLift2Down.BackColor = Color.Orange;
                        //    }
                        //    else
                        //    {
                        //        btnPCBLift2Down.BackColor = Color.Yellow;
                        //    }
                        //}
                        //else
                        //{
                        //    btnPCBLift2Down.BackColor = Color.White;
                        //}
                        #endregion

                        #region Front Passline
                        if (DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor))
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].BuyerChange &&
                                        FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBPassLine.BackColor = Color.Blue;
                            }
                            else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].BuyerChange &&
                                        FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBPassLine.BackColor = Color.Orange;
                            }
                            else
                            {
                                btnPCBPassLine.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            btnPCBPassLine.BackColor = Color.White;
                        }
                        #endregion

                        #region Rear Passline
                        if (DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor))
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].BuyerChange &&
                                        FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBRearPassLine.BackColor = Color.Blue;
                            }
                            else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].BuyerChange &&
                                        FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue)
                            {
                                btnPCBRearPassLine.BackColor = Color.Orange;
                            }
                            else
                            {
                                btnPCBRearPassLine.BackColor = Color.Yellow;
                            }
                        }
                        else
                        {
                            btnPCBRearPassLine.BackColor = Color.White;
                        }
                        #endregion

                        #endregion


                        #region Controls.Find 가 렉을 많이 일으켜 대안으로 Control.Item 목록에서 한번에 처리하는 방식으로 변경
                        foreach (Control conSite in pnSite.Controls) // 사이트 묶음 전체
                        {
                            if (conSite.GetType() == typeof(Panel))
                            {
                                int siteNo = -1;
                                int.TryParse(conSite.Name.Replace("pnSite", "").Replace("Title", ""), out siteNo);
                                //debug("Panel (Site Main) : " + conSite.Name + ", siteIndex : " + siteNo);
                                if (siteNo < 1 ||
                                    siteNo > FuncInline.MaxSiteCount)
                                {
                                    //debug("사이트 인덱스 오류 : " + siteNo);
                                    continue;
                                }
                                int siteIndex = siteNo - 1;
                                FuncInline.enumTeachingPos pos = FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex;
                                FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi);
                                if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid)
                                {
                                    conSite.BackColor = Color.Silver;
                                }
                                else
                                {
                                    conSite.BackColor = FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail ||
                                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel ? Color.Tomato : // NG 경우 적색
                                                                                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                                                                                            DIO.GetDIData(dockDi) &&    //사이트별 센서확인
                                                                                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.UnKnown ? Color.Transparent : // PCB 없으면 투명
                                                                                            FuncInline.PCBInfo[(int)pos].SelfRetestCount > 0 || FuncInline.PCBInfo[(int)pos].OtherRetestCount > 0 ? Color.Pink :
                                                                                            Color.Yellow; // 재테스트면 오렌지, 나머지 노랑
                                }
                                // ... (위 conSite.BackColor 까지는 이미 수정하신 코드 유지)

                                // 사이트 묶음 내부 컨트롤 순회
                                foreach (Control subSite in conSite.Controls)
                                {
                                    if (subSite.GetType() == typeof(Panel))
                                    {
                                        // pnSite** 만 처리
                                        if (!subSite.Name.Contains("pnSite"))
                                            continue;

                                        Panel pnTitle = (Panel)subSite;

                                        // pnSite**Title
                                        if (pnTitle.Name.Contains("Title"))
                                        {
                                            if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid)
                                            {
                                                pnTitle.BackColor = Color.Silver;
                                            }
                                            else
                                            {
                                                pnTitle.BackColor =
                                                    FuncError.CheckError(FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex) ? Color.Red :
                                                    (!FuncInline.PCBInfo[(int)pos].BuyerChange &&
                                                     FuncInline.SiteAction[siteIndex] != FuncInline.enumSiteAction.Waiting) ? Color.Lime :
                                                    Color.Turquoise;
                                            }
                                        }
                                        // pnSite**(타이틀 제외 실컨텐츠 패널)
                                        else
                                        {
                                            if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid)
                                            {
                                                // 원본 코드 유지 (컨테이너 패널에 Silver 지정)
                                                pnSite.BackColor = Color.Silver;
                                            }
                                            else
                                            {
                                                // 위 conSite와 동일한 팔레트 규칙 + PCB 유무는 dockDi로 판단
                                                pnSite.BackColor =
                                                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail ||
                                                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                                                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                                                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel ? Color.Tomato :
                                                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White :
                                                    DIO.GetDIData(dockDi) &&
                                                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.UnKnown ? Color.Transparent :
                                                    (FuncInline.PCBInfo[(int)pos].SelfRetestCount > 0 || FuncInline.PCBInfo[(int)pos].OtherRetestCount > 0) ? Color.Pink :
                                                    Color.Yellow;
                                            }
                                        }

                                        // 하위 컨트롤(Label 등) 처리
                                        foreach (Control subCon in subSite.Controls)
                                        {
                                            if (subCon.GetType() == typeof(Label))
                                            {
                                                Label subLabel = (Label)subCon;

                                                // lblSite**Action
                                                if (subLabel.Name.Contains("Action"))
                                                {
                                                    subLabel.Text =
                                                        FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid
                                                        ? FuncInline.SiteState[siteIndex].ToString()
                                                        : (FuncInline.SiteAction[siteIndex] == FuncInline.enumSiteAction.Testing && FuncInline.PCBInfo[(int)pos].BuyerChange) ? "Test_Wait"
                                                        : FuncInline.PCBInfo[(int)pos].UserCancel ? "Canceling"
                                                        : (FuncInline.PCBInfo[(int)pos].OtherReTest && FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.ReTest) ? "ReSite"
                                                        : FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? "Test_Pass"
                                                        : (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail ||
                                                           FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.ReTest) ? "Test_Fail"
                                                        : FuncInline.SiteAction[siteIndex].ToString();
                                                }

                                                // lblSite**Time
                                                if (subLabel.Name.Contains("Time"))
                                                {
                                                    if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid)
                                                    {
                                                        subLabel.Text =
                                                            FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.UnKnown ? "" :
                                                            Util.DoubleToString(FuncInline.PCBInfo[(int)pos].StopWatch.ElapsedMilliseconds / 1000, 1, 0);
                                                    }
                                                    else
                                                    {
                                                        subLabel.Text =
                                                            Util.DoubleToString(FuncInline.PCBInfo[(int)pos].StopWatch.ElapsedMilliseconds / 1000, 1, 0) +
                                                            (FuncInline.TestTime[siteIndex] > 0 ? "/" + FuncInline.TestTime[siteIndex] : "");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (subSite.GetType() == typeof(Button))
                                    {
                                        // btnPCBSite.. : PCB 유무 표시 (dockDi 사용)
                                        if (((Button)subSite).Name.Contains("btnPCBSite"))
                                        {
                                            if (DIO.GetDIData(dockDi))
                                            {
                                                if (FuncInline.PCBInfo[(int)pos].BuyerChange &&
                                                    FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue)
                                                {
                                                    ((Button)subSite).BackColor = Color.Blue;
                                                }
                                                else if (FuncInline.PCBInfo[(int)pos].BuyerChange &&
                                                         FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue)
                                                {
                                                    ((Button)subSite).BackColor = Color.Orange;
                                                }
                                                else
                                                {
                                                    ((Button)subSite).BackColor = Color.Yellow;
                                                }
                                            }
                                            else
                                            {
                                                ((Button)subSite).BackColor = Color.White;
                                            }
                                        }

                                        // btnSite..STS : STS 버튼(해당 포지션에 PCB정보 존재 시 노출)
                                        if (((Button)subSite).Name.Contains("STS"))
                                        {
                                            ((Button)subSite).Visible =
                                                FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                        }
                                    }
                                    else if (subSite.GetType() == typeof(Label))
                                    {
                                        // 어레이 목록(label) – 텍스트 생성부는 그대로, 배경색만 dockDi로 교체
                                        ((Label)subSite).Text = arrayList; // 위에서 구성한 arrayList 유지

                                        if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid)
                                        {
                                            ((Label)subSite).BackColor = Color.Silver;
                                        }
                                        else
                                        {
                                            ((Label)subSite).BackColor =
                                                FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail ||
                                                FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                                                FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                                                FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel ? Color.Tomato :
                                                FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White :
                                                DIO.GetDIData(dockDi) &&
                                                FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.UnKnown ? Color.Transparent :
                                                (FuncInline.PCBInfo[(int)pos].SelfRetestCount > 0 || FuncInline.PCBInfo[(int)pos].OtherRetestCount > 0) ? Color.Pink :
                                                Color.Yellow;
                                        }
                                    }
                                }


                                #region 이전코드
                                //foreach (Control subSite in conSite.Controls) // 사이트 묶음 전체
                                //{
                                //    if (subSite.GetType() == typeof(Panel))
                                //    {
                                //        //debug("     Panel (Site Sub) : " + subSite.Name);
                                //        #region pnSite** 표시
                                //        if (!subSite.Name.Contains("pnSite"))
                                //        {
                                //            continue;
                                //        }
                                //        //debug("         Panel (Title Sub) : " + subCon.Name);
                                //        #region pnSite**Title
                                //        Panel pnTitle = (Panel)subSite;
                                //        if (pnTitle.Name.Contains("Title"))
                                //        {
                                //            if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid)
                                //            {
                                //                pnTitle.BackColor = Color.Silver;
                                //            }
                                //            else
                                //            {
                                //                pnTitle.BackColor = FuncError.CheckError(FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex) ? Color.Red :
                                //                                                !FuncInline.PCBInfo[(int)pos].BuyerChange && FuncInline.SiteAction[(int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1] != FuncInline.enumSiteAction.Waiting ? Color.Lime :
                                //                                                Color.Turquoise;
                                //            }
                                //        }
                                //        #endregion
                                //        else
                                //        {
                                //            if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid)
                                //            {
                                //                pnSite.BackColor = Color.Silver;
                                //            }
                                //            else
                                //            {
                                //                pnSite.BackColor = FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail ||
                                //                        FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                                //                        FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                                //                        FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel ? Color.Tomato : // NG 경우 적색
                                //                                                                        FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                                //                                                                        !FuncInlineIO.GetDISiteData(siteIndex, FuncInline.enumDISite.X1_Site_PCB_Stop_Sensor) && FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.UnKnown ? Color.Transparent : // PCB 없으면 투명
                                //                                                                        FuncInline.PCBInfo[(int)pos].SelfRetestCount > 0 || FuncInline.PCBInfo[(int)pos].OtherRetestCount > 0 ? Color.Pink :
                                //                                                                        Color.Yellow; // 재테스트면 오렌지, 나머지 노랑
                                //            }
                                //        }
                                //        #endregion
                                //        foreach (Control subCon in subSite.Controls) // 사이트 묶음 전체
                                //        {
                                //            if (subCon.GetType() == typeof(Label))
                                //            {
                                //                //debug("         Label (Title Sub) : " + subCon.Name);
                                //                Label subLabel = (Label)subCon;
                                //                #region lblSite**Action
                                //                if (subLabel.Name.Contains("Action"))
                                //                {
                                //                    subLabel.Text = FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid ? FuncInline.SiteState[siteIndex].ToString() :
                                //                                    //!FuncInlineIO.GetDISiteData(siteIndex, FuncInline.enumDISite.X28_Module_Power) ? "PowerOff" :
                                //                                    //!FuncInline.UseSite[siteIndex] ? "NotUse" :
                                //                                    FuncInline.SiteAction[siteIndex] == FuncInline.enumSiteAction.Testing && FuncInline.PCBInfo[(int)pos].BuyerChange ? "Test_Wait" :
                                //                                    FuncInline.PCBInfo[(int)pos].UserCancel ? "Canceling" :
                                //                                    FuncInline.PCBInfo[(int)pos].OtherReTest && FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.ReTest ? "ReSite" :
                                //                                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? "Test_Pass" :
                                //                                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail || FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.ReTest ? "Test_Fail" :
                                //                                    FuncInline.SiteAction[siteIndex].ToString();
                                //                }
                                //                #endregion
                                //                #region lblSite**Time
                                //                if (subLabel.Name.Contains("Time"))
                                //                {
                                //                    if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid) //!FuncInline.UseSite[siteIndex] ||
                                //                                                                                           //!FuncInlineIO.GetDISiteData(siteIndex, FuncInline.enumDISite.X28_Module_Power))
                                //                    {
                                //                        subLabel.Text = FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.UnKnown ? "" :
                                //                                        Util.DoubleToString(FuncInline.PCBInfo[(int)pos].StopWatch.ElapsedMilliseconds / 1000, 1, 0);
                                //                    }
                                //                    else
                                //                    {
                                //                        subLabel.Text = Util.DoubleToString(FuncInline.PCBInfo[(int)pos].StopWatch.ElapsedMilliseconds / 1000, 1, 0) +
                                //                                        (FuncInline.TestTime[siteIndex] > 0 ? "/" + FuncInline.TestTime[siteIndex] : "");
                                //                    }
                                //                }
                                //                #endregion
                                //            }
                                //        }
                                //    }
                                //    else if (subSite.GetType() == typeof(Button))
                                //    {
                                //        //debug("     Button (Site Sub) : " + subSite.Name);
                                //        #region btnPCBSite..  : PCB 유무
                                //        if (((Button)subSite).Name.Contains("btnPCBSite"))
                                //        {
                                //            if (FuncInlineIO.GetDISiteData(siteIndex, FuncInline.enumDISite.X1_Site_PCB_Stop_Sensor))
                                //            {
                                //                if (FuncInline.PCBInfo[(int)pos].BuyerChange &&
                                //                            FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue)
                                //                {
                                //                    ((Button)subSite).BackColor = Color.Blue;
                                //                }
                                //                else if (FuncInline.PCBInfo[(int)pos].BuyerChange &&
                                //                            FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue)
                                //                {
                                //                    ((Button)subSite).BackColor = Color.Orange;
                                //                }
                                //                else
                                //                {
                                //                    ((Button)subSite).BackColor = Color.Yellow;
                                //                }
                                //            }
                                //            else
                                //            {
                                //                ((Button)subSite).BackColor = Color.White;
                                //            }
                                //            #endregion
                                //        }
                                //        #region btnSite..STS  : STS 버튼
                                //        if (((Button)subSite).Name.Contains("STS"))
                                //        {
                                //            ((Button)subSite).Visible = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                //        }
                                //        #endregion
                                //    }
                                //    else if (subSite.GetType() == typeof(Label))
                                //    {
                                //        //debug("     Label (Site Sub) : " + subSite.Name);
                                //        #region lblSite..  : 어레이 목록 표시
                                //        //arrayList = FuncInline.PCBInfo[(int)pos].UserCancel ? "Canceling\n" : // 취소중
                                //        //        FuncInline.PCBInfo[(int)pos].OtherReTest && FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.ReTest ? "ReSite\n" : // Resite 이동 전
                                //        //        FuncInline.PCBInfo[(int)pos].PCBStatus.ToString().Replace("UnKnown", "").Replace("Retest", "Test_Fail") + "\n";//"";// 
                                //        arrayList = "";
                                //        arrayIndex = 1;
                                //        for (int i = 0; i < FuncInline.PCBInfo[(int)pos].Barcode.Length; i++)
                                //        {
                                //            string code = FuncInline.PCBInfo[(int)pos].Barcode[i];
                                //            if (code.Length > 6)
                                //            {
                                //                code = code.Substring(code.Length - 6, 6);
                                //                //재검사고 이전 테스트 에러 없으면 표시 생략
                                //                if (FuncInline.PCBInfo[(int)pos].OtherRetestCount > 0 &&
                                //                    FuncInline.PCBInfo[(int)pos].BeforeCode[i] < 0)
                                //                {
                                //                }
                                //                else if (FuncInline.PCBInfo[(int)pos].Xout[i])
                                //                {
                                //                    arrayList += (i + 1) + ".XOut" + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                //                }
                                //                else if (FuncInline.UseBadMark && FuncInline.PCBInfo[(int)pos].BadMark[i])
                                //                {
                                //                    arrayList += (i + 1) + ".Bad" + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                //                }
                                //                else if (FuncInline.PCBInfo[(int)pos].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Pass)
                                //                {
                                //                    arrayList += (i + 1) + ".Pass" + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                //                }
                                //                else if (FuncInline.PCBInfo[(int)pos].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail)
                                //                {
                                //                    // NG 코드와 에러명 출력
                                //                    string errorName = "uknown";
                                //                    try
                                //                    {
                                //                        if (FuncInline.PCBInfo[(int)pos].ErrorCode[i] > -1)
                                //                        {
                                //                            errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)pos].ErrorCode[i]];
                                //                        }
                                //                    }
                                //                    catch (Exception xx)
                                //                    {
                                //                        //debug(xx.ToString());
                                //                        //debug(xx.StackTrace);
                                //                    }
                                //                    if (errorName.Length > 6)
                                //                    {
                                //                        errorName = errorName.Substring(0, 6);
                                //                    }
                                //                    arrayList += (i + 1) + "." + errorName + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                //                }
                                //                else if (FuncInline.PCBInfo[(int)pos].SMDStatus[i] == FuncInline.enumSMDStatus.Before_Command &&
                                //                        FuncInline.PCBInfo[(int)pos].BeforeCode[i] >= 0)
                                //                {
                                //                    // NG 코드와 에러명 출력
                                //                    string errorName = "uknown";
                                //                    try
                                //                    {
                                //                        if (FuncInline.PCBInfo[(int)pos].BeforeCode[i] > -1)
                                //                        {
                                //                            errorName = FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)pos].BeforeCode[i]];
                                //                        }
                                //                    }
                                //                    catch (Exception xx)
                                //                    {
                                //                        //debug(xx.ToString());
                                //                        //debug(xx.StackTrace);
                                //                    }
                                //                    if (errorName.Length > 6)
                                //                    {
                                //                        errorName = errorName.Substring(0, 6);
                                //                    }
                                //                    arrayList += (i + 1) + "." + errorName + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                //                }
                                //                else if (FuncInline.PCBInfo[(int)pos].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Cancel ||
                                //                    FuncInline.PCBInfo[(int)pos].SMDStatus[i] == FuncInline.enumSMDStatus.User_Cancel)
                                //                {
                                //                    arrayList += (i + 1) + ".Cancel" + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                //                }
                                //                else if (FuncInline.PCBInfo[(int)pos].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                                //                {
                                //                    arrayList += (i + 1) + ".TMOut" + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                //                }
                                //                else
                                //                {
                                //                    arrayList += (i + 1) + "." + code + (arrayIndex != FuncInline.MaxArrayCount && (arrayIndex - 1) % 2 == 1 ? "\n" : "");
                                //                }
                                //                arrayIndex++;
                                //            }
                                //        }
                                //        ((Label)subSite).Text = arrayList;
                                //        if (FuncInline.SiteState[siteIndex] != FuncInline.enumSiteState.Valid) ////!FuncInline.UseSite[siteIndex] ||
                                //                                                                               //!FuncInlineIO.GetDISiteData(siteIndex, FuncInline.enumDISite.X28_Module_Power))
                                //        {
                                //            ((Label)subSite).BackColor = Color.Silver;
                                //        }
                                //        else
                                //        {
                                //            ((Label)subSite).BackColor = FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail ||
                                //                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                                //                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                                //                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.User_Cancel ? Color.Tomato : // NG 경우 적색
                                //                                                                    FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ? Color.White : // 양품 경우 백색
                                //                                                                    !FuncInlineIO.GetDISiteData(siteIndex, FuncInline.enumDISite.X1_Site_PCB_Stop_Sensor) && FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.UnKnown ? Color.Transparent : // PCB 없으면 투명
                                //                                                                    FuncInline.PCBInfo[(int)pos].SelfRetestCount > 0 || FuncInline.PCBInfo[(int)pos].OtherRetestCount > 0 ? Color.Pink :
                                //                                                                    Color.Yellow; // 재테스트면 오렌지, 나머지 노랑
                                //        }
                                //        #endregion
                                //    }
                                //}
                                #endregion

                            }
                        }

                        #endregion


                        #endregion
                        //lblTackTime.Text = GlobalVar.TackTime.ToString("F1") + " sec";
                        //debug("runtime : " + watch.ElapsedMilliseconds);
                    }));
                }
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
            if (GlobalVar.GlobalStop)
            {
                try
                {
                    timerUI.Dispose();
                }
                catch { }
            }
        }

        #endregion
       
 
        ulong tick = GlobalVar.TickCount64;
    


        #region 버튼 등 공통 UI 컨트롤 이벤트

        #region Main Tap 관련
        private void pbAuto_Click(object sender, EventArgs e)//Main
        {
            FuncLog.WriteLog("Main - Main Click");
            if (!GlobalVar.Simulation &&
                FuncInline.TabMain == FuncInline.enumTabMain.PartClear &&
                GlobalVar.SystemErrored)
            {
                return;
            }
            //ClearSubMenu();
            //pbAuto.BackgroundImage = Radix.Properties.Resources.sub_main_sel;
            FuncInline.TabMain = FuncInline.enumTabMain.Auto;
            //tcMain.SelectedIndex = (int)FuncInline.TabMain;
            Close_Sub_Form();

        }
        private void pbManual_Click(object sender, EventArgs e)
        {
            if (FuncInline.TabMain == FuncInline.enumTabMain.PartClear &&
               GlobalVar.SystemErrored)
            {
                return;
            }
            #region 교시 모드 아니면 무시
            /*
            if (!GlobalVar.PwdPass)
            {
                return;
            }
            //*/
            #endregion
            #region Status 관련 확인
            if (!GlobalVar.Simulation &&
                GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                GlobalVar.SystemStatus != enumSystemStatus.ErrorStop) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            #endregion
            FuncLog.WriteLog("Main - Manual Click");

            if (GlobalVar.PwdPass || true)  //일단 패스워드 안보고 진행 by DG 2507
            {
                Close_Sub_Form();

                //ClearSubMenu();
                //pbManual.BackgroundImage = Radix.Properties.Resources.sub_manual_sel;
                FuncInline.TabMain = FuncInline.enumTabMain.Manual;
                //tcMain.SelectedIndex = (int)FuncInline.enumTabMain.Manual;
                //if (GlobalVar.dlgOpened)
                //{
                //    return;
                //}
                //Manual dlg = new Manual();
                //dlg.Owner = this;
                //dlg.Show();
            }
            else
            {
                Password dlg = new Password();
                dlg.ShowDialog();
            }

            //if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun &&     // 작동 중 자동 닫기
            //    (int)GlobalVar.SystemStatus != (int)enumSystemStatus.ErrorStop)    //ErrorStop일 경우는 메뉴얼창 들어갈수 있게 , 금수석님 인라인모듈쪽에 이렇게 하셨음 by DGKim
            //{
            //    FuncWin.TopMessageBox("Can't use while system is running.");
            //    return;
            //}
            //ClearSubMenu();
            //pbManual.BackgroundImage = Radix.Properties.Resources.sub_manual_sel;
            //FuncInline.TabMain = FuncInline.enumTabMain.Manual;
            //tcMain.SelectedIndex = (int)FuncInline.TabMain;
       
        }
        private void pbIOMonitor_Click(object sender, EventArgs e)
        {
            //ClearSubMenu();
            //pbIOMonitor.BackgroundImage = Radix.Properties.Resources.sub_io_sel;
            //FuncInline.TabMain = FuncInline.enumTabMain.IO;
            //tcMain.SelectedIndex = (int)FuncInline.TabMain;
        }
        private void pbTower_Click(object sender, EventArgs e)//System
        {
            if (FuncInline.TabMain == FuncInline.enumTabMain.PartClear &&
                GlobalVar.SystemErrored)
            {
                return;
            }
            if (!GlobalVar.Simulation &&
                GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                GlobalVar.SystemStatus != enumSystemStatus.ErrorStop) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            FuncLog.WriteLog("Main - System Click");

            if (GlobalVar.PwdPass)  //일단 무조건 통과
            {
                //ClearSubMenu();
                //pbTower.BackgroundImage = Radix.Properties.Resources.sub_system_sel;
                FuncInline.TabMain = FuncInline.enumTabMain.Machine;
                //tcMain.SelectedIndex = (int)FuncInline.TabMain;
                Close_Sub_Form();

                frmMachine.LoadAllValue();
            }
            else
            {
                Password dlg = new Password();
                dlg.ShowDialog();
            }

            //if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중 자동 닫기
            //{
            //    FuncWin.TopMessageBox("Can't use while system is running.");
            //    return;
            //}
            //GlobalVar.PwdPass = false;
            //Password dlg = new Password();
            //dlg.ShowDialog();
            //if (GlobalVar.PwdPass)
            //{

            //    ClearSubMenu();
            //    pbTower.BackgroundImage = Radix.Properties.Resources.sub_system_sel;
            //    FuncInline.TabMain = FuncInline.enumTabMain.Machine;
            //    tcMain.SelectedIndex = (int)FuncInline.TabMain;

            //    if (!GlobalVar.ByPassMode)
            //    {
            //        FuncIni.LoadMachinenIni();
            //    }
                
            //    //frmMachine.LoadAllValue();
            //}


        }
        private void pbModel_Click(object sender, EventArgs e)
        {
            if (FuncInline.TabMain == FuncInline.enumTabMain.PartClear &&
               GlobalVar.SystemErrored)
            {
                return;
            }
            #region 교시 모드 아니면 무시
            /*
            if (!GlobalVar.PwdPass)
            {
                return;
            }
            //*/
            #endregion
            if (!GlobalVar.Simulation &&
                GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                GlobalVar.SystemStatus != enumSystemStatus.ErrorStop) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            FuncLog.WriteLog("Main - Model Click");

            if (GlobalVar.PwdPass)
            {
                //ClearSubMenu();
                //pbModel.BackgroundImage = Radix.Properties.Resources.sub_model_sel;
                FuncInline.TabMain = FuncInline.enumTabMain.Model;
                //tcMain.SelectedIndex = (int)FuncInline.TabMain;
                Close_Sub_Form();

                frmModel.LoadAllValue();
                frmModel.PreviewModel(GlobalVar.ModelName);
            }
            else
            {
                Password dlg = new Password();
                dlg.ShowDialog();
            }

            //if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun) // 작동 중 자동 닫기
            //{
            //    FuncWin.TopMessageBox("Can't use while system is running.");
            //    return;
            //}
            //ClearSubMenu();
            //pbMES.BackgroundImage = Radix.Properties.Resources.sub_model_sel;

            //FuncInline.TabMain = FuncInline.enumTabMain.Model;
            //tcMain.SelectedIndex = (int)FuncInline.TabMain;

            //frmMES.UpdateMesTable();
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
            FuncInline.TabMain = FuncInline.enumTabMain.Teaching;
            tcMain.SelectedIndex = (int)FuncInline.TabMain;

            //frmTeaching.LoadAllValue();
        }
        private void pbTrace_Click(object sender, EventArgs e)
        {
            ClearSubMenu();
            pbTrace.BackgroundImage = Radix.Properties.Resources.sub_trace_sel;
            FuncInline.TabMain = FuncInline.enumTabMain.Trace;
            tcMain.SelectedIndex = (int)FuncInline.TabMain;
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
    
        #endregion
        private void ClearSubMenu()
        {
            pbAuto.BackgroundImage = Radix.Properties.Resources.sub_main;
            pbManual.BackgroundImage = Radix.Properties.Resources.sub_manual;
            //pbIOMonitor.BackgroundImage = Radix.Properties.Resources.sub_io;
            pbTower.BackgroundImage = Radix.Properties.Resources.sub_system;
            pbModel.BackgroundImage = Radix.Properties.Resources.sub_model;
            pbTeaching.BackgroundImage = Radix.Properties.Resources.sub_teaching;
            pbTrace.BackgroundImage = Radix.Properties.Resources.sub_trace;
            //pbRouter.BackgroundImage = Radix.Properties.Resources.sub_Router;
            pbPartClear.BackgroundImage = Properties.Resources.part_clear;

        }
        #endregion

        #region 우측 버튼 관련

        private void pbStart_Click(object sender, EventArgs e)
        {

            if (GlobalVar.SystemStatus == enumSystemStatus.BeforeInitialize)
            {
                FuncWin.MessageBoxOK("초기화를 먼저 수행하세요!");
                return;
            }

            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun)
            {
                int i = 0;
                return;
            }
            if (tcMain.SelectedIndex != (int)FuncInline.enumTabMain.Auto)
            {
                FuncWin.TopMessageBox("Main화면에서 선택 해주세요");
                return;
            }

            if (GlobalVar.SystemStatus != enumSystemStatus.Manual) return;

        
            // 여기서 부터는 매뉴얼 상태가 확실하다.

            if (FuncWin.MessageBoxOK("작업을 시작 하시겠습니까?"))
            {
                FuncLog.WriteLog("Main - Start Click ");
                //FuncAmplePacking.OpenRobotDoor(false); //도어락
                FuncInline.Start_Button(false);

                //// State SV07_Print
                //FuncBoxPacking.LogView2("===============START==============");
                //FuncBoxPacking.StepPrint();
            }

            //// 시작시 리프린트 설정 해제
            //btnRePrint.BackColor = Color.WhiteSmoke;

        }
        private void pbStop_Click(object sender, EventArgs e)
        {
            GlobalVar.WarningStatePre.Clear();

            FuncLog.WriteLog("Main - Stop Click ");
            //if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
            //{
            //    return;
            //}

            if (GlobalVar.SystemStatus == enumSystemStatus.Initialize &&
                FuncWin.MessageBoxOK("초기화를 취소하시겠습니까?"))
            {
                GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
            }
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
            {
                return;
            }
            //GlobalVar.StepStop = true;


            // 스톱 버튼을 누르면 강제로 Manual 모드가 되고 모든 서보가 정지한다.
            // 스누콘 박스포장기에서는 1STEP 의 종료 모드로 사용 한다.
            FuncInline.Stop_Button();

            //// State SV07_Print
            //FuncBoxPacking.LogView2("===============STOP==============");
            //FuncBoxPacking.StepPrint();


        }
        private void pbCycleStop_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Main - Cycle Stop Click ");
            //return 조건 변경 by DGkim
            if (!(GlobalVar.SystemStatus >= enumSystemStatus.AutoRun) &&
                GlobalVar.SystemStatus != enumSystemStatus.Manual)
            {
                return;
            }
            if (!FuncInline.CycleStop &&
                FuncWin.MessageBoxOK("Cycle Stop operation?"))
            {
                FuncLog.WriteLog("Main - Cycle Stop Click - ON ");
                FuncInline.CycleStop = true;
                if ((GlobalVar.SystemStatus >= enumSystemStatus.AutoRun))
                {
                    GlobalVar.SystemStatus = enumSystemStatus.CycleStop;
                }
                pbCycleStop.BackgroundImage = Properties.Resources.cycle_stop_red;
            }
            else
            {

                FuncLog.WriteLog("Main - Cycle Stop Click - OFF ");
                FuncInline.CycleStop = false;
                if ((GlobalVar.SystemStatus >= enumSystemStatus.AutoRun))
                {
                    GlobalVar.SystemStatus = enumSystemStatus.AutoRun;
                }
                pbCycleStop.BackgroundImage = Properties.Resources.cycle_stop;
            }
        }
        private void pbInit_Click(object sender, EventArgs e)
        {
            //return; //캡조립공정 추가로 일단 초기화 막음 메뉴얼 동작으로 초기화
            RunSystemInit(false);
        }

        private void RunSystemInit(bool op)
        {
            // 초기화 진행은 일회성 쓰레드를 만들어 루프를 태워야 메인화면 헹이 없어진다.
            try
            {
                FuncLog.WriteLog("Main - Init Click ");

                if (!op &&
                    FuncInline.TabMain != FuncInline.enumTabMain.Auto)
                {
                    FuncWin.TopMessageBox("Main화면에서 선택 해주세요");
                    return;
                }
                #region Door 관련 확인

               
                if (FuncInline.Door_Check())
                {
                    if (!op)
                    {
                        FuncWin.TopMessageBox("Can't Initialize while doors are opened.");
                    }
                    return;
                }
                
                #endregion

                #region Status 관련 확인
                if ((GlobalVar.SystemStatus >= enumSystemStatus.AutoRun) ||
                    (GlobalVar.SystemStatus == enumSystemStatus.Initialize))
                {
                    if (!op)
                    {
                        FuncWin.TopMessageBox("Can't Initialize while machine is running or initializing.");
                    }
                    return;
                }
                #endregion

                #region 제품 잔존 여부 확인
                /*
                if (DIO.Product_Residual_Check())
                {
                    return;
                }
                //*/
                #endregion

                #region 로봇 통신 확인

                #endregion

                #region 초기화 진행
                if (op ||
                    FuncWin.MessageBoxOK("Initialize facility?"))
                {
                    (new Thread(InitThread)).Start();
                    //GlobalVar.SystemStatus = enumSystemStatus.Initialize;

                }
                #endregion
            }
            catch (Exception ex)
            {
                FuncLog.WriteLog(ex.ToString());
                FuncLog.WriteLog(ex.StackTrace);
            }
        }

        /**
         * @brief 초기화 진행 상태 체크 쓰레드
        */

        private void InitThread()
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                //FuncLog.WriteLog("Init Click");
                pbInit.BackgroundImage = Properties.Resources.initialize_bright;

                Loading dlgLoading = new Loading();
                dlgLoading.TopMost = true;  //Init창 최상위로 유지 시킨다 by DG
                dlgLoading.Show();

                #region 전역 변수 초기화
                //SNUC_AmplePackingClass에서 바꿔준다 여기서 굳이 더 중복으로 할필요 없을것으로 보임 by DGKim
                //((SNUC_AmplePackingClass)GlobalVar.ProjectClass).mainConveyor.Action = MainConveyorClass.enumAction.Init;
                #endregion

                #region 지역 변수 초기화
                GlobalVar.Init_Finish = false;
                #endregion

                GlobalVar.SystemStatus = enumSystemStatus.Initialize;
                int startTime = Environment.TickCount;

                while (//!GlobalVar.GlobalStop &&
                    Environment.TickCount - startTime < 5 * 60 * 1000 &&
                    //GlobalVar.SystemStatus == enumSystemStatus.Initialize &&
                    !GlobalVar.GlobalStop)
                {
                    GlobalVar.Init_Finish = GlobalVar.SystemStatus == enumSystemStatus.Manual; //메인쓰레드에서 완료 되면 manual로 바꾸니까 이것만 봐도 됨

                    #region 초기화 성공
                    if (GlobalVar.Init_Finish)
                    {
                        dlgLoading.Close();

                        FuncWin.TopMessageBox("Initialize finished.");
                        GlobalVar.SystemStatus = enumSystemStatus.Manual;

                        pbInit.BackgroundImage = Properties.Resources.initialize;

                        #region 하부 클래스 상태 변경
                        FuncInline.InitAllSubAction();
                        #endregion

                        return;
                    }
                    #endregion

                    #region 초기화 취소
                    if (GlobalVar.SystemStatus != enumSystemStatus.Initialize)
                    {
                        FuncMotion.MoveStopAll();
                        #region 하부 클래스 상태 변경
                        FuncInline.InitAllSubAction();
                        #endregion
                        return;
                    }
                    #endregion

                    Application.DoEvents();
                    Thread.Sleep(100);
                }

                #region 초기화 실패 사유 조합
                string stat = "";
                stat += ((AutoInline_Class)GlobalVar.ProjectClass).beforeLift01.Log + "\n";
                stat += ((AutoInline_Class)GlobalVar.ProjectClass).beforeWork02.Log + "\n";
                
               

                // 다른 하부 클래스들도 stat += 로 연결해야 함
                #endregion

                #region 하부 클래스 상태 변경
                FuncInline.InitAllSubAction();
                #endregion

                GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;

                pbInit.BackgroundImage = Properties.Resources.initialize;
                dlgLoading.Close();
                FuncWin.TopMessageBox("initialize failed.\n" + stat);

                FuncLog.WriteLog("initialize failed.\n" + stat);

            }));
        }


        private void pbPartClear_Click(object sender, EventArgs e)
        {
            #region Status 관련 확인
            if (!GlobalVar.Simulation &&
                GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                GlobalVar.SystemStatus != enumSystemStatus.ErrorStop) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            #endregion

            FuncLog.WriteLog("Main - PartClear Click");

            //파트클리어 선택시 전체 초기화 할껀지와 파트별로 초기화 할껀지에 대해서 묻는 창이 나와야 하구요
            //전체 초기화 선택시 예 눌리면 초기화 진행 그리고 아니요 선택시에 지금 파트 클리어 화면과 동일하게 나오게 되면 개별 클리어 가능


            //*
            Stopwatch clearWatch = new Stopwatch();
            string msg = "Clear All Errored Parts?";
            if (FuncWin.MessageBoxOK(msg))
            {

                ClearAllAlarm();

            }
            else
            {
                //ClearSubMenu();
                FuncInline.TabMain = FuncInline.enumTabMain.PartClear;
                //tcMain.SelectedIndex = (int)FuncInline.TabMain;
                //pbPartClear.BackgroundImage = Properties.Resources.part_clear_bright;

                Close_Sub_Form();

            }
        }
        private void pbErrorLog_Click(object sender, EventArgs e)
        {
            ClearSubMenu();
            FuncInline.TabMain = FuncInline.enumTabMain.Errors;
            tcMain.SelectedIndex = (int)FuncInline.TabMain;

            //

            //frmLogViewer.ShowFromMain();

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
            pbBuzzerStop.BackgroundImage = GlobalVar.EnableBuzzer ? Properties.Resources.buzzer_stop : Properties.Resources.buzzer_stop_bright;
            //}
        }
        private void pbDailyCountReset_Click(object sender, EventArgs e)
        {
            this.BringToFront();
            if (FuncWin.MessageBoxOK("Reset Time Count?"))
            {
                //GlobalVar.ProductCanCount = 0;
                //GlobalVar.ProductBoxCount = 0;
                //((SNUC_AmplePackingClass)GlobalVar.ProjectClass).runTime = GlobalVar.TickCount64;
                //((SNUC_AmplePackingClass)GlobalVar.ProjectClass).runTotal = 0;
                //GlobalVar.TackStart = GlobalVar.TickCount64;

                lblRunningTime.Text = "-";
                lblTotalTime.Text = "-";
                lblCycleTime.Text = "-";

                // JHRYU
                GlobalVar.RunTimeCurrent.Reset();
                GlobalVar.RunTimeTotal.Reset();
                GlobalVar.RunTimeTact.Reset();
            }
        }
        private void pbExit_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("Main - Exit Click ");
            //ClearSubMenu();
            //pbRouter.BackgroundImage = Radix.Properties.Resources.sub_Router_sel;

            //FuncInline.TabMain = FuncInline.enumTabMain.Router;
            //tcMain.SelectedIndex = (int)FuncInline.TabMain;

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
                this.Close();
            }
            else
            {
                this.BringToFront();
                FuncWin.TopMessageBox("Can't stop program while system is running.");
            }
        }
        #endregion

       

        #endregion


        #region 수동조작 이벤트

        #region 수동 버튼
        private void pbDistributor_Click(object sender, EventArgs e)
        {
            if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun)
            {
                //DIO.WriteDOData(FuncInline.enumDONames.Y01_4_Label_Vacuum, DIO.GetDIData(FuncInline.enumDINames.X01_2_));
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
                //DIO.WriteDOData(FuncInline.enumDONames.Y02_5_, DIO.GetDIData(FuncInline.enumDINames.X02_6_));
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
                //DIO.WriteDOData(FuncInline.enumDONames.Y08_5_Cap_Stopper_Forward, DIO.GetDIData(FuncInline.enumDINames.X10_7_Cap_Stopper_Reward));
            }
        }

        private void lblCapStopper_Click(object sender, EventArgs e)
        {
            pbCapStopper_Click(sender, e);
        }


        #endregion

        #endregion


        //테스트 버튼 ////////////////////////////////////////////////////////////////////////////////////////////


    

        #region 컨트롤러 인스턴스
        /// <summary>
        /// PLCStatusThread에서 Main UI를 컨트롤 하기 위해 by DGKim 230327
        /// </summary>


        // 매인 UI 하단 에 한줄 추가
        public void logView(string msg)
        {
            try
            {
                if (listBoxStatus.InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        String timeString = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");
                        //StatusLabel1.Text = timeString + msg;

                        listBoxStatus.Items.Insert(0, timeString + msg);
                        if (listBoxStatus.Items.Count > 100)
                            listBoxStatus.Items.RemoveAt(listBoxStatus.Items.Count - 1);
                        listBoxStatus.SelectedIndex = 0;

                    }));
                }
                else
                {
                    String timeString = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");
                    //StatusLabel1.Text = timeString + msg;

                    listBoxStatus.Items.Insert(0, timeString + msg);
                    if (listBoxStatus.Items.Count > 100)
                        listBoxStatus.Items.RemoveAt(listBoxStatus.Items.Count - 1);
                    listBoxStatus.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {
            }
        }

        // 스레드 스탭 상태 보기용
        public void logView2(string msg)
        {
            try
            {
                if (listBoxStep.InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        if (msg.Length == 0)
                        {
                            listBoxStep.Items.Clear();
                        }
                        else
                        {

                            String timeString = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");
                            //StatusLabel1.Text = timeString + msg;

                            listBoxStep.Items.Insert(0, timeString + msg);
                            if (listBoxStep.Items.Count > 100)
                                listBoxStep.Items.RemoveAt(listBoxStatus.Items.Count - 1);
                            listBoxStep.SelectedIndex = 0;
                        }
                    }));
                }
                else
                {
                    if (msg.Length == 0)
                    {
                        listBoxStep.Items.Clear();
                    }
                    else
                    {
                        String timeString = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");
                        //StatusLabel1.Text = timeString + msg;

                        listBoxStep.Items.Insert(0, timeString + msg);
                        if (listBoxStep.Items.Count > 100)
                            listBoxStep.Items.RemoveAt(listBoxStep.Items.Count - 1);
                        listBoxStep.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        #endregion

        public void UpdateLogViewerDialog()
        {
            // ErrorDialog 에서 Log로 전환시 강제 조회 갱신 해줌
            frmLogViewer.ShowFromMain();

        }



        private void btnRtexTest_Click(object sender, EventArgs e)
        {

            GlobalVar.SystemStatus = enumSystemStatus.EmgStop;

            //uint nAxis = (uint)0;       // 0번축
            //if (!RTEX.IsStandStill(nAxis)) return;
            //RTEX.MoveRelative(nAxis, 10000, 50000);
        }


        private void cbOneStep_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVar.StepStop = cbOneStep.Checked;
        }

      









        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (FuncWin.MessageBoxOK("Clear ALL Lot Info?"))
            {
                //for (int i = 1; i <= 7; i++)
                //{
                //    var outInfo = GlobalVar.Tracking.OutInfo[i-1];
                //    string lotNo = outInfo.OutLot;
                //    int count = outInfo.OutCount;
                //    int mesSize = outInfo.MesLotSize;
                //    FuncLog.WriteLog($"작업자 LotClear {i} : Lot: {lotNo} {count}/{mesSize}");
                //    GlobalVar.Tracking.ClearOutLot(i);
                //}

                //GlobalVar.Tracking.TotalCount = 0;
            }
        }


       

    



        private void btn_StepReset_Click(object sender, EventArgs e)
        {
            //if ( GlobalVar.SystemStatus == enumSystemStatus.Manual )
            //{
            //    if (FuncWin.MessageBoxOK("STEP RESET ?"))
            //    {
            //        // 모든 스레드의 동작을 강제로 초기화 한다.
            //        // 에러가 발생한후 에러 처리를 하고 원점 초기화 없이 동작을 이어서 하고 싶을경우
            //        // 불가피하게 사용한다. 

            //        //GlobalVar.Thread_AutoRun.StepResetAll();
            //    }
            //}
        }

        int pos_idx = 0;

        private void pb_AlarmGif_Click(object sender, EventArgs e)
        {
            //Point[] poss = { FuncBoxPacking.GetErrPosByErrNum(922), FuncBoxPacking.GetErrPosByErrNum(1100),
            //    FuncBoxPacking.GetErrPosByErrNum(1201), FuncBoxPacking.GetErrPosByErrNum(1301),
            //    FuncBoxPacking.GetErrPosByErrNum(1401), FuncBoxPacking.GetErrPosByErrNum(1501),
            //    FuncBoxPacking.GetErrPosByErrNum(1601), FuncBoxPacking.GetErrPosByErrNum(1701),
            //    FuncBoxPacking.GetErrPosByErrNum(1801), FuncBoxPacking.GetErrPosByErrNum(1901),
            //    FuncBoxPacking.GetErrPosByErrNum(-100)
            //};

            // test
            //if (pos_idx >= poss.Length) pos_idx = 0;
            //var pos = poss[pos_idx++];


            //pb_AlarmGif.Parent = pictureBox2;
            //pb_AlarmGif.Location = pos;
            //pb_AlarmGif.BackColor = Color.Transparent;
            //pb_AlarmGif.BringToFront();
            //pb_AlarmGif.Visible = true;

        }

        // MES DB 접속 테스트를 한다.
        private void lbMes_Click(object sender, EventArgs e)
        {
            //// 장비 정지상태 에서만
            //if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun) return;

            //if (FuncWin.MessageBoxOK("MES 통신 접속테스트를 수행할까요?"))
            //{
            //    if (GlobalVar.Sql.Check_Connect())
            //    {
            //        FuncLog.WriteLog($"MES 접속테스트 성공입니다");
            //        GlobalVar.Sql.connected = true;
            //    }
            //    else
            //    {
            //        string msg = "ERROR - MES 접속테스트 실패입니다";
            //        GlobalVar.Sql.connected = false;

            //        FuncLog.WriteLog(msg);
            //        FuncWin.TopMessageBox(msg);
            //    }

            //}

        }

        /** 
         * @brief 메인 폼 구동시 각 파트의 Action값 리스트를 구해 ComboBox에 출력한다.
         */
        private void LoadActionList()
        {
            #region #0 메인 컨베이어
            //cbConveyorAction.Items.Clear();
            //string[] names = Enum.GetNames(typeof(MainConveyorClass.enumAction));
            //for (int i = 0; i < names.Length; i++)
            //{
            //    cbConveyorAction.Items.Add(names[i]);
            //}
            #endregion

         
        }


        private void tmrError_Tick(object sender, EventArgs e)
        {

        }

        private void pbMinimize_Click(object sender, EventArgs e)
        {
            // 현재 폼의 상태를 최소화로 변경
            this.WindowState = FormWindowState.Minimized;
        }

     
        private void button13_Click(object sender, EventArgs e)
        {
         

        }

       

        private void button14_Click(object sender, EventArgs e)
        {
          

        }
       


    

      

        private void Reset_Clear()
        {
            // 리셋버튼 누름시 메인페이지로 이동하고 메인탭이 선택되게 한다.
            ClearSubMenu();
            pbAuto.BackgroundImage = Radix.Properties.Resources.sub_main_sel;
            FuncInline.TabMain = FuncInline.enumTabMain.Auto;
            tcMain.SelectedIndex = (int)FuncInline.TabMain;
            Application.DoEvents();
        }

     


      

        private void btn_CountReset_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("[Main화면] 카운트 초기화 클릭");
            
        }

        private void lbHiddenClick_Click(object sender, EventArgs e)
        {
            FuncInline.HiddenCount++;
        }

        private void pbRunMode_Click(object sender, EventArgs e)
        {
            #region Status 관련 확인
            if (FuncInline.TabMain == FuncInline.enumTabMain.PartClear &&
               GlobalVar.SystemErrored)
            {
                return;
            }
            if (!GlobalVar.Simulation &&
                GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                GlobalVar.SystemStatus != enumSystemStatus.ErrorStop) // 작동 중 자동 닫기
            {
                FuncWin.TopMessageBox("Can't use while system is running.");
                return;
            }
            #endregion

            Close_Sub_Form();


            FuncLog.WriteLog("Main - RunMode Click");

            pbRunMode.BackgroundImage = Properties.Resources.run_mode_bright;
            (new ModeSelect()).ShowDialog();
            pbRunMode.BackgroundImage = Properties.Resources.run_mode;
        }
        private void ClearAllAlarm()
        {
            // 1) 시스템 안전 조건 점검: 아무것도 켜져 있지 않으면 종료
            if (!GlobalVar.SystemErrored &&
                !GlobalVar.E_Stop &&
                //!FuncInline.Loto_Switch &&
                //!GlobalVar.LightCurtain &&
                !GlobalVar.DoorOpen)
            {
                return;
            }

            // 2) 시스템 파트 에러 해제 및 안전플래그 초기화
            FuncError.RemoveError(FuncInline.enumErrorPart.System);
            FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.System] = false;
            //GlobalVar.Loto_Switch = false;
            GlobalVar.LightCurtain = false;
            GlobalVar.DoorOpen = false;

            // 3) E-STOP 해제 확인 (현 프로젝트 DIO 기준으로 수정)
            if (GlobalVar.E_Stop)
            {
                // 비상정지 해제 감지: X00_3_Emergency_Stop (필요시 X00_5_Emergency_Stop_Limit 추가 AND)
                if (DIO.GetDIData(FuncInline.enumDINames.X00_3_Emergency_Stop))
                {
                    FuncMotion.ServoAllInit();
                    GlobalVar.E_Stop = false;
                    GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
                }
                return;
            }

            // 4) 시스템 에러가 아니면 상태만 적정 단계로 맞추고 복귀
            if (!GlobalVar.SystemErrored)
            {
                GlobalVar.SystemStatus = (GlobalVar.SystemStatus < enumSystemStatus.Manual)
                    ? enumSystemStatus.BeforeInitialize
                    : enumSystemStatus.Manual;
                return;
            }

            Stopwatch clearWatch = new Stopwatch();

            try
            {
                FuncInline.WaitMessage = "Clearing Parts";
                Util.StartWatch(ref clearWatch);

                for (int i = 0; i < FuncInline.NeedPartClear.Length; i++)
                {
                    if (!FuncInline.NeedPartClear[i]) continue;

                    // 공통 컨텍스트
                    var part = (FuncInline.enumErrorPart)i;
                    bool diCheck = false;                  // 해당 파트에 PCB 감지되면 true
                    FuncInline.enumTeachingPos pos = FuncInline.enumTeachingPos.None;

                    // 해당 파트에서 구동할 수 있는 모터/스토퍼 (없는 경우 기본값)
                    bool hasMotor1 = false, hasMotor2 = false, hasStopper = false;
                    FuncInline.enumDONames motorCW = default, motorCCW = default, stopperDO = default;

                    // === 파트별 매핑 ===
                    switch (part)
                    {
                        // ───────── InConveyor ─────────
                        case FuncInline.enumErrorPart.InConveyor:
                            pos = FuncInline.enumTeachingPos.InConveyor;
                            diCheck =
                                DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                                DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor);

                            // 모터(인셔틀) & 스토퍼
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw;
                            hasMotor2 = true; motorCCW = FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw;
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL;
                            break;

                        // ───────── InShuttle ─────────
                        case FuncInline.enumErrorPart.InShuttle:
                            pos = FuncInline.enumTeachingPos.InShuttle;
                            diCheck =
                                DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ||
                                               DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
                                               DIO.GetDIData(FuncInline.enumDINames.X304_0_In_Shuttle_Interlock_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw;
                            hasMotor2 = true; motorCCW = FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw;
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL;
                            break;

                        // ───────── FrontPassline ─────────
                        case FuncInline.enumErrorPart.FrontPassLine:
                            pos = FuncInline.enumTeachingPos.FrontPassLine;
                            diCheck = DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw;
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y1_7_Front_PASSLINE_PCB_STOPPER_SOL;
                            break;

                        // ───────── RearPassline (OK Line) ─────────
                        case FuncInline.enumErrorPart.RearPassLine:
                            pos = FuncInline.enumTeachingPos.RearPassLine;
                            diCheck = DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y305_4_Rear_PassLine_Motor_Cw;
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y1_6_Rear_OK_PassLine_CONTACT_STOPPER_SOL;
                            break;

                        // ───────── RearNGline ─────────
                        case FuncInline.enumErrorPart.RearNGLine:
                            pos = FuncInline.enumTeachingPos.RearNGLine;
                            diCheck = DIO.GetDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y404_0_Rear_NgLine_Motor_Cw;
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y4_5_Rear_NG_PassLine_CONTACT_STOPPER_SOL;
                            break;

                        // ───────── Lift1_Up ─────────
                        case FuncInline.enumErrorPart.Lift1_Up:
                            pos = FuncInline.enumTeachingPos.Lift1_Up;
                            diCheck = DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor) ||
                                DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor);

                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw;
                            hasMotor2 = true; motorCCW = FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw;
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL;
                            break;

                        // ───────── Lift1_Down ─────────
                        case FuncInline.enumErrorPart.Lift1_Down:
                            pos = FuncInline.enumTeachingPos.Lift1_Down;
                            diCheck =
                                DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                                DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw;
                            hasMotor2 = true; motorCCW = FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw;
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL;
                            break;

                        // ───────── Lift2_Up ─────────
                        case FuncInline.enumErrorPart.Lift2_Up:
                            pos = FuncInline.enumTeachingPos.Lift2_Up;
                            diCheck =
                                DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                                DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw;
                            hasMotor2 = true; motorCCW = FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw;
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL;
                            break;

                        // ───────── Lift2_Down ─────────
                        case FuncInline.enumErrorPart.Lift2_Down:
                            pos = FuncInline.enumTeachingPos.Lift2_Down;
                            diCheck =
                                DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                                DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw;
                            hasMotor2 = true; motorCCW = FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw;
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL;
                            break;

                        // ───────── OutConveyor ─────────
                        case FuncInline.enumErrorPart.OutConveyor:
                            pos = FuncInline.enumTeachingPos.OutConveyor;
                            diCheck =
                                DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                                DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw;
                            // (OutConveyor 스토퍼 DO 맵이 명확치 않아 생략)
                            break;

                        // ───────── OutShuttle_Up (OK) ─────────
                        case FuncInline.enumErrorPart.OutShuttle_Up:
                            pos = FuncInline.enumTeachingPos.OutShuttle_Up;
                            // OK 라인은 전용 PCB In/Stop DI가 없어서, 인터락/스토퍼 IN 감지로 대체
                            diCheck =
                                DIO.GetDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor) ||
                                DIO.GetDIData(FuncInline.enumDINames.X303_5_Out_Shuttle_Stopper_Cyl_IN_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw;
                            hasMotor2 = true; motorCCW = FuncInline.enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw;
                            // OK 스토퍼 DO: Y300_2_Out_Shuttle_CONTACT_STOPPER_IN_SOL
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_IN_SOL;
                            break;

                        // ───────── OutShuttle_Down (NG) ─────────
                        case FuncInline.enumErrorPart.OutShuttle_Down:
                            pos = FuncInline.enumTeachingPos.OutShuttle_Down;
                            diCheck =
                                DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) ||
                                DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw;
                            hasMotor2 = true; motorCCW = FuncInline.enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw;
                            // NG 스토퍼 DO: Y302_1_Out_Shuttle_CONTACT_STOPPER_Out_SOL
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_Out_SOL;
                            break;

                        // ───────── NgBuffer ─────────
                        case FuncInline.enumErrorPart.NgBuffer:
                            pos = FuncInline.enumTeachingPos.NgBuffer;
                            diCheck = DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor) ||
                                DIO.GetDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw;
                            break;

                        // ───────── FrontScanSite ─────────
                        case FuncInline.enumErrorPart.FrontScanSite:
                            pos = FuncInline.enumTeachingPos.FrontScanSite;
                            diCheck = DIO.GetDIData(FuncInline.enumDINames.X03_4_Front_Scan_PCB_Dock_Sensor);
                            hasMotor1 = true; motorCW = FuncInline.enumDONames.Y406_1_Front_SCAN_Motor_Cw;
                            hasMotor2 = true; motorCCW = FuncInline.enumDONames.Y406_3_Front_SCAN_Motor_Ccw;
                            // 스토퍼: Y3_7_Front_SCAN_STOPPER_SOL
                            hasStopper = true; stopperDO = FuncInline.enumDONames.Y3_7_Front_SCAN_STOPPER_SOL;
                            break;

                        // ───────── 테스트 사이트(1~26) 공통처리 ─────────
                        default:
                            {
                                if (part >= FuncInline.enumErrorPart.Site1_F_DT1 &&
                                    part <= FuncInline.enumErrorPart.Site26_R_FT3)
                                {
                                    // enumErrorPart → enumTeachingPos 동형 매핑
                                    int offset = (int)part - (int)FuncInline.enumErrorPart.Site1_F_DT1;
                                    pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + offset);

                                    // PCB DOCK DI
                                    if (FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi))
                                    {
                                        diCheck = DIO.GetDIData(dockDi);
                                    }

                                    // 사이트 컨베이어 모터쌍(CW/CCW)
                                    if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(pos, out var cw, out var ccw))
                                    {
                                        hasMotor1 = true; motorCW = cw;
                                        hasMotor2 = true; motorCCW = ccw;
                                    }

                                    // 스토퍼(사이트 PCB 클램프)
                                    if (FuncInline.SiteIoMaps.TryGetContactStopperDO(pos, out var stopper))
                                    {
                                        hasStopper = true; stopperDO = stopper;
                                    }

                                    break;
                                }

                                // 그 외 파트는 처리 불필요
                                continue;
                            }
                    }

                    // PCB 감지되면 사용자 안내 후 중단 (시스템/버퍼 등 일부는 제외하려면 조건 분기)
                    if (part != FuncInline.enumErrorPart.System && diCheck)
                    {
                        FuncWin.TopMessageBox($"PCB Detected in {part}. Remove PCB and try again");
                        return;
                    }

                    // 로그/메시지
                    FuncInline.WaitMessage = $"Clearing Part {pos}";
                    Func.WriteLog($"PartClear - {pos}");

                    // 테스트 중이던 사이트면 SMD에 Stop 통지
                    if (pos >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                         pos <= FuncInline.enumTeachingPos.Site26_R_FT3 &&
                         FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Testing)
                    {
                        int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1; // 0~25
                        int smdIndex = FuncInline.MapSmdIndex(siteIndex);

                        // 0 - DL / 1,2,3,4 Front FT / 5,6,7,8 Rear FT
                        FuncInline.ComSMD[smdIndex].SendStop(siteIndex);
                    }

                    // PCB 정보 삭제 & 타이머 초기화
                    if (pos != FuncInline.enumTeachingPos.None)
                        FuncInline.ClearPCBInfo(pos);

                    FuncInline.PCBInfo[(int)pos].StopWatch.Stop();
                    FuncInline.PCBInfo[(int)pos].StopWatch.Reset();

                    // 동작 상태 리셋(파트별)
                    switch (part)
                    {
                        case FuncInline.enumErrorPart.InConveyor:
                            DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
                            FuncInline.InConveyorAction = FuncInline.enumLiftAction.Waiting;
                            break;
                        case FuncInline.enumErrorPart.InShuttle:
                            DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
                            FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                            break;

                        case FuncInline.enumErrorPart.FrontPassLine:
                            FuncInline.FrontPassLineAction = FuncInline.enumLiftAction.Waiting;
                            break;

                        case FuncInline.enumErrorPart.RearPassLine:
                            FuncInline.RearPassLineAction = FuncInline.enumLiftAction.Waiting;
                            break;
                        case FuncInline.enumErrorPart.RearNGLine:
                            FuncInline.RearNGLineAction = FuncInline.enumLiftAction.Waiting;
                            break;
                        case FuncInline.enumErrorPart.OutShuttle_Up:
                            FuncInline.OutShuttleUpAction = FuncInline.enumShuttleAction.Waiting;
                            break;
                        case FuncInline.enumErrorPart.OutShuttle_Down:
                            FuncInline.OutShuttleDownAction = FuncInline.enumShuttleAction.Waiting;
                            break;
                        case FuncInline.enumErrorPart.Lift1_Up:
                        case FuncInline.enumErrorPart.Lift1_Down:
                            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
                            break;

                        case FuncInline.enumErrorPart.Lift2_Up:
                        case FuncInline.enumErrorPart.Lift2_Down:
                            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
                            break;

                        case FuncInline.enumErrorPart.OutConveyor:
                            DIO.WriteDOData(FuncInline.enumDONames.Y412_1_SMEMA_After_Ready, false);
                            FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
                            break;

                        case FuncInline.enumErrorPart.NgBuffer:
                            FuncInline.NGBufferAction = FuncInline.enumLiftAction.Waiting;
                            FuncInline.NGOut = false;
                            FuncInline.NgAlarmWatch.Stop();
                            FuncInline.NgAlarmWatch.Reset();
                            break;

                        default:
                            {
                                // 사이트 1~26이면 사이트 액션 초기화 로그
                                if (part >= FuncInline.enumErrorPart.Site1_F_DT1 &&
                                    part <= FuncInline.enumErrorPart.Site26_R_FT3)
                                {
                                    int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                                    if (FuncInline.SiteAction[siteIndex] != FuncInline.enumSiteAction.Waiting)
                                    {
                                        Func.WriteLog_Tester($"{part} action change : {FuncInline.SiteAction[siteIndex]} ==> Waiting");
                                    }
                                    FuncInline.SiteAction[siteIndex] = FuncInline.enumSiteAction.Waiting;
                                }
                                break;
                            }
                    }

                    // 파트 에러 모두 제거 & 플래그 초기화
                    FuncError.RemoveError(part);
                    FuncError.RemoveAllError();
                    FuncInline.NeedPartClear[(int)part] = false;

                    // 스토퍼 먼저(있으면), 모터 CW 가동(시뮬 OFF 시)
                    if (hasStopper) DIO.WriteDOData(stopperDO, true);
                    if (hasMotor1 && !GlobalVar.Simulation) DIO.WriteDOData(motorCW, true);

                    FuncInline.WaitMessage = "";
                }

                // 서보 초기화 & 시스템 플래그 정리
                FuncMotion.ServoAllInit();
                FuncError.RemoveAllError();

                GlobalVar.E_Stop = false;
                GlobalVar.DoorOpen = false;
                GlobalVar.LightCurtain = false;
                GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0;
                GlobalVar.SystemStatus = (GlobalVar.SystemStatus > enumSystemStatus.Initialize)
                    ? enumSystemStatus.Manual
                    : enumSystemStatus.BeforeInitialize;

                // 클리어 플래그 전체 리셋
                for (int i = 0; i < FuncInline.NeedPartClear.Length; i++)
                    FuncInline.NeedPartClear[i] = false;

                FuncInline.DuplicatedPos[0] = FuncInline.enumTeachingPos.None;
                FuncInline.DuplicatedPos[1] = FuncInline.enumTeachingPos.None;

                // 약간 대기
                while (clearWatch.IsRunning && clearWatch.ElapsedMilliseconds < 2000)
                    Util.USleep(GlobalVar.ThreadSleep);

                FuncInline.WaitMessage = "";
                Util.ResetWatch(ref clearWatch);

                // 모든 컨베어 정지 및 NG 알람 타이머 리셋
                FuncInline.StopAllConveyor();
                Util.StartWatch(ref FuncInline.NgAlarmWatch);

                // 목적지 무효화(리프트/사이트 간 충돌 방지) — 기존 로직 유지, 명칭만 보정
                // Lift1_Up → Site1~13 목적지에 PCB 이미 있으면 삭제
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    var dst = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination;
                    if (dst >= FuncInline.enumTeachingPos.Site1_F_DT1 && dst <= FuncInline.enumTeachingPos.Site13_F_FT3 &&
                        FuncInline.PCBInfo[(int)dst].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination = FuncInline.enumTeachingPos.None;
                    }
                }
                // Lift1_Down → Site1~13 목적지 비었으면 삭제
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                {
                    var dst = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination;
                    if (dst >= FuncInline.enumTeachingPos.Site1_F_DT1 && dst <= FuncInline.enumTeachingPos.Site13_F_FT3 &&
                        FuncInline.PCBInfo[(int)dst].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination = FuncInline.enumTeachingPos.None;
                    }
                }
                // Lift2_Up → Site14~26 목적지에 PCB 이미 있으면 삭제
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    var dst = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination;
                    if (dst >= FuncInline.enumTeachingPos.Site14_R_DT1 && dst <= FuncInline.enumTeachingPos.Site26_R_FT3 &&
                        FuncInline.PCBInfo[(int)dst].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination = FuncInline.enumTeachingPos.None;
                    }
                }
                // Lift2_Down → Site14~26 목적지 비었으면 삭제
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                {
                    var dst = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination;
                    if (dst >= FuncInline.enumTeachingPos.Site14_R_DT1 && dst <= FuncInline.enumTeachingPos.Site26_R_FT3 &&
                        FuncInline.PCBInfo[(int)dst].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination = FuncInline.enumTeachingPos.None;
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        #region ClearAllAlarm Old
        //private void ClearAllAlarm()
        //{
        //    if (!GlobalVar.SystemErrored &&
        //        !GlobalVar.E_Stop &&
        //        !FuncInline.Loto_Switch &&
        //        !GlobalVar.LightCurtain &&
        //        !GlobalVar.DoorOpen)
        //    {
        //        return;
        //    }

        //    FuncError.RemoveError(FuncInline.enumErrorPart.System);

        //    FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.System] = false;
        //    GlobalVar.Loto_Switch = false;
        //    GlobalVar.LightCurtain = false;
        //    GlobalVar.DoorOpen = false;

        //    if (GlobalVar.E_Stop)
        //    {
        //        if (DIO.GetDIData(FuncInline.enumDINames.X00_7_Safety_EMO))
        //        {
        //            FuncMotion.ServoAllInit();
        //            GlobalVar.E_Stop = false;
        //            GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
        //        }
        //        return;
        //    }


        //    if (!GlobalVar.SystemErrored)
        //    {
        //        if (GlobalVar.SystemStatus < enumSystemStatus.Manual)
        //        {
        //            GlobalVar.SystemStatus = enumSystemStatus.BeforeInitialize;
        //        }
        //        else
        //        {
        //            GlobalVar.SystemStatus = enumSystemStatus.Manual;
        //        }
        //        return;
        //    }

        //    Stopwatch clearWatch = new Stopwatch();
        //    try
        //    {
        //        FuncInline.WaitMessage = "Clearing Parts";
        //        Util.StartWatch(ref clearWatch);

        //        for (int i = 0; i < FuncInline.NeedPartClear.Length; i++)
        //        {
        //            if (FuncInline.NeedPartClear[i])
        //            {
        //                FuncInline.enumErrorPart part = FuncInline.enumErrorPart.System;
        //                // 파트에러 있거나, 
        //                // 클리어 필요하거나, 
        //                // PCB 정보 있거나, 
        //                // PCB 감지되거나,
        //                // 액션이 있으면 클리어
        //                bool partError = false;
        //                bool needClear = false;
        //                bool pcbInfo = false;
        //                bool pcbCheck = false;
        //                bool actionCheck = false;
        //                bool diCheck = false;
        //                FuncInline.enumTeachingPos pos = FuncInline.enumTeachingPos.None;
        //                FuncInline.enumDONames convRun = FuncInline.enumDONames.Y01_7_;
        //                FuncInline.enumDONames convRun2 = FuncInline.enumDONames.Y01_7_;
        //                FuncInline.enumDONames stopperUp = FuncInline.enumDONames.Y01_7_;

        //                switch ((FuncInline.enumErrorPart)i)
        //                {
        //                    case FuncInline.enumErrorPart.InConveyor:
        //                        part = FuncInline.enumErrorPart.InConveyor;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.InConveyor);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.InConveyor];
        //                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor);
        //                        actionCheck = FuncInline.InConveyorAction != FuncInline.enumLiftAction.Waiting;
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor, false);
        //                        }
        //                        pos = FuncInline.enumTeachingPos.InConveyor;
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor);
        //                        convRun = FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw;
        //                        stopperUp = FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL;
        //                        break;
        //                    case FuncInline.enumErrorPart.FrontPassline:
        //                        part = FuncInline.enumErrorPart.FrontPassline;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.FrontPassline);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.FrontPassline];
        //                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassline].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor);
        //                        actionCheck = FuncInline.FrontPassLineAction != FuncInline.enumLiftAction.Waiting;
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor, false);
        //                        }
        //                        pos = FuncInline.enumTeachingPos.FrontPassline;
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor);
        //                        convRun = FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw;
        //                        stopperUp = FuncInline.enumDONames.Y03_3_Rack1_PassLine_Stopper_Forward;
        //                        break;
        //                    case FuncInline.enumErrorPart.Lift1_Up:
        //                        part = FuncInline.enumErrorPart.Lift1_Up;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.Lift1_Up);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift1_Up];
        //                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X04_1_Rack1_UpLift_Stop_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor);
        //                        actionCheck = FuncInline.Lift1Action != FuncInline.enumLiftAction.Waiting;
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X04_1_Rack1_UpLift_Stop_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor, false);
        //                        }
        //                        pos = FuncInline.enumTeachingPos.Lift1_Up;
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X04_1_Rack1_UpLift_Stop_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor);
        //                        convRun = FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw;
        //                        convRun2 = FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw;
        //                        stopperUp = FuncInline.enumDONames.Y03_3_Rack1_PassLine_Stopper_Forward;
        //                        break;
        //                    case FuncInline.enumErrorPart.Rack1_Lift_Down:
        //                        part = FuncInline.enumErrorPart.Rack1_Lift_Down;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.Rack1_Lift_Down);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Rack1_Lift_Down];
        //                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X05_2_Rack1_DownLift_End_Sensor);
        //                        actionCheck = FuncInline.Lift1Action != FuncInline.enumLiftAction.Waiting;
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X05_2_Rack1_DownLift_End_Sensor, false);
        //                        }
        //                        pos = FuncInline.enumTeachingPos.Lift1_Down;
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X05_2_Rack1_DownLift_End_Sensor);
        //                        convRun = FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw;
        //                        convRun2 = FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw;
        //                        stopperUp = FuncInline.enumDONames.Y04_6_Rack1_DownLift_Stopper_Up;
        //                        break;
        //                    case FuncInline.enumErrorPart.Rack2_PassLine:
        //                        part = FuncInline.enumErrorPart.Rack2_PassLine;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.Rack2_PassLine);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Rack2_PassLine];
        //                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassline].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X10_5_Rack2_PassLine_End_Sensor);
        //                        actionCheck = FuncInline.RearPassLineAction != FuncInline.enumLiftAction.Waiting;
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X10_5_Rack2_PassLine_End_Sensor, false);
        //                        }
        //                        pos = FuncInline.enumTeachingPos.RearPassline;
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X10_5_Rack2_PassLine_End_Sensor);
        //                        convRun = FuncInline.enumDONames.Y03_6_Rack2_PassLine_Conveyor_Run;
        //                        stopperUp = FuncInline.enumDONames.Y03_5_Rack2_PassLine_Stopper_Forward;
        //                        break;
        //                    case FuncInline.enumErrorPart.Lift2_Up:
        //                        part = FuncInline.enumErrorPart.Lift2_Up;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.Lift2_Up);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift2_Up];
        //                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X06_2_Rack2_UpLift_End_Sensor);
        //                        actionCheck = FuncInline.Lift2Action != FuncInline.enumLiftAction.Waiting;
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X06_2_Rack2_UpLift_End_Sensor, false);
        //                        }
        //                        pos = FuncInline.enumTeachingPos.Lift2_Up;
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X06_2_Rack2_UpLift_End_Sensor);
        //                        convRun = FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw;
        //                        convRun2 = FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw;
        //                        stopperUp = FuncInline.enumDONames.Y05_2_Rack2_UpLift_Stopper_Up;
        //                        break;
        //                    case FuncInline.enumErrorPart.Lift2_Down:
        //                        part = FuncInline.enumErrorPart.Lift2_Down;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.Lift2_Down);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift2_Down];
        //                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X07_2_Rack2_DownLift_End_Sensor);
        //                        actionCheck = FuncInline.Lift2Action != FuncInline.enumLiftAction.Waiting;
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X07_2_Rack2_DownLift_End_Sensor, false);
        //                        }
        //                        pos = FuncInline.enumTeachingPos.Lift2_Down;
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X07_2_Rack2_DownLift_End_Sensor);
        //                        convRun = FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw;
        //                        convRun2 = FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw;
        //                        stopperUp = FuncInline.enumDONames.Y05_6_Rack2_DownLift_Stopper_Up;
        //                        break;
        //                    case FuncInline.enumErrorPart.OutConveyor:
        //                        part = FuncInline.enumErrorPart.OutConveyor;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.OutConveyor);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.OutConveyor];
        //                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X08_2_Out_Conveyor_End_Sensor);
        //                        actionCheck = FuncInline.OutConveyorAction != FuncInline.enumLiftAction.Waiting;
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X08_2_Out_Conveyor_End_Sensor, false);
        //                        }
        //                        pos = FuncInline.enumTeachingPos.OutConveyor;
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X08_2_Out_Conveyor_End_Sensor);
        //                        convRun = FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw;
        //                        stopperUp = FuncInline.enumDONames.Y06_1_Out_Conveyor_Stopper_Up;
        //                        break;
        //                    case FuncInline.enumErrorPart.NgBuffer:
        //                        part = FuncInline.enumErrorPart.NgBuffer;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.NgBuffer);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.NgBuffer];
        //                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.NgBuffer].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X09_0_NG_Buffer_Start_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X09_1_NG_Buffer_Stop_Sensor) ||
        //                            DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor);
        //                        actionCheck = FuncInline.OutConveyorAction != FuncInline.enumLiftAction.Waiting;
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X09_0_NG_Buffer_Start_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X09_1_NG_Buffer_Stop_Sensor, false);
        //                            DIO.WriteDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor, false);
        //                        }
        //                        pos = FuncInline.enumTeachingPos.NgBuffer;
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X09_0_NG_Buffer_Start_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X09_1_NG_Buffer_Stop_Sensor) ||
        //                                    DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor);
        //                        break;
        //                    case FuncInline.enumErrorPart.Site1:
        //                    case FuncInline.enumErrorPart.Site2:
        //                    case FuncInline.enumErrorPart.Site3:
        //                    case FuncInline.enumErrorPart.Site4:
        //                    case FuncInline.enumErrorPart.Site5:
        //                    case FuncInline.enumErrorPart.Site6:
        //                    case FuncInline.enumErrorPart.Site7:
        //                    case FuncInline.enumErrorPart.Site8:
        //                    case FuncInline.enumErrorPart.Site9:
        //                    case FuncInline.enumErrorPart.Site10:
        //                    case FuncInline.enumErrorPart.Site11:
        //                    case FuncInline.enumErrorPart.Site12:
        //                    case FuncInline.enumErrorPart.Site13:
        //                    case FuncInline.enumErrorPart.Site14:
        //                    case FuncInline.enumErrorPart.Site15:
        //                    case FuncInline.enumErrorPart.Site16:
        //                    case FuncInline.enumErrorPart.Site17:
        //                    case FuncInline.enumErrorPart.Site18:
        //                    case FuncInline.enumErrorPart.Site19:
        //                    case FuncInline.enumErrorPart.Site20:
        //                    case FuncInline.enumErrorPart.Site21:
        //                    case FuncInline.enumErrorPart.Site22:
        //                    case FuncInline.enumErrorPart.Site23:
        //                    case FuncInline.enumErrorPart.Site24:
        //                    case FuncInline.enumErrorPart.Site25:
        //                    case FuncInline.enumErrorPart.Site26:
        //                    case FuncInline.enumErrorPart.Site27:
        //                    case FuncInline.enumErrorPart.Site28:
        //                    case FuncInline.enumErrorPart.Site29:
        //                    case FuncInline.enumErrorPart.Site30:
        //                    case FuncInline.enumErrorPart.Site31:
        //                    case FuncInline.enumErrorPart.Site32:
        //                    case FuncInline.enumErrorPart.Site33:
        //                    case FuncInline.enumErrorPart.Site34:
        //                    case FuncInline.enumErrorPart.Site35:
        //                    case FuncInline.enumErrorPart.Site36:
        //                    case FuncInline.enumErrorPart.Site37:
        //                    case FuncInline.enumErrorPart.Site38:
        //                    case FuncInline.enumErrorPart.Site39:
        //                    case FuncInline.enumErrorPart.Site40:
        //                        // 테스트사이트는 묶어서 처리
        //                        FuncInline.enumTeachingPos site = (enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + int.Parse(((FuncInline.enumErrorPart)i).ToString().Replace("Site", "")) - 1);
        //                        int siteIndex = (int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
        //                        part = FuncInline.enumErrorPart.Site1 + siteIndex;
        //                        partError = FuncError.CheckError(FuncInline.enumErrorPart.Site1 + siteIndex);
        //                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1 + siteIndex];
        //                        pcbInfo = FuncInline.PCBInfo[(int)site].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
        //                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + siteIndex * GlobalVar.DIModuleGap);
        //                        actionCheck = FuncInline.SiteAction[siteIndex] != FuncInline.enumSiteAction.Waiting;
        //                        #region 사이트 경우 오픈 여부 체크
        //                        /*
        //                        if (part >= FuncInline.enumErrorPart.Site1 &&
        //                            part <= FuncInline.enumErrorPart.Site40)
        //                        {
        //                            int siteNo = (int)part - (int)FuncInline.enumErrorPart.Site1;
        //                            if (GlobalVar.AutoInline_SiteClear[siteNo])
        //                            {
        //                                FuncWin.TopMessageBox("Open " + site.ToString() + " and remove PCB first.");
        //                                return;
        //                            }
        //                        }
        //                        //*/
        //                        #endregion
        //                        if (GlobalVar.Simulation)
        //                        {
        //                            DIO.WriteDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + GlobalVar.DIModuleGap * (pos - FuncInline.enumTeachingPos.Site1_F_DT1), false);
        //                        }
        //                        pos = (enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex);
        //                        diCheck = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + GlobalVar.DIModuleGap * (pos - FuncInline.enumTeachingPos.Site1_F_DT1));
        //                        convRun = FuncInline.enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap;
        //                        convRun2 = FuncInline.enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap;

        //                        break;
        //                    case FuncInline.enumErrorPart.System: // 시스템 경우 체크 없음
        //                    default: // 해당 사항 없음
        //                        continue;
        //                }

        //                //Site_Click(activeButton, e);

        //                if ((FuncInline.enumErrorPart)i != FuncInline.enumErrorPart.System &&
        //                    (FuncInline.enumErrorPart)i != FuncInline.enumErrorPart.NgBuffer &&
        //                    diCheck)
        //                {
        //                    FuncWin.TopMessageBox("PCB Detected in " + ((FuncInline.enumErrorPart)i).ToString() + ". Remove PCB and try again");
        //                    return;
        //                }
        //                /*
        //                if (partError ||
        //                    needClear ||
        //                    pcbInfo ||
        //                    pcbCheck ||
        //                    actionCheck)
        //                {
        //                    FuncInline.WaitMessage = "Clearing " + part.ToString();
        //                    if (!btnPartClear_Click())
        //                    {
        //                        FuncInline.WaitMessage = "";
        //                        return;
        //                    }
        //                }
        //                //*/
        //                //*/
        //                FuncInline.WaitMessage = "Clearing Part " + pos.ToString();
        //                Func.WriteLog("PartClear - " + pos.ToString());


        //                #region 사이트 경우 테스트중인 array에 STS전송
        //                if (pos >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
        //                    pos <= FuncInline.enumTeachingPos.Site26_R_FT3)
        //                {
        //                    if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Testing)
        //                    {
        //                        int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
        //                        int smdIndex = siteIndex / FuncInline.MaxSiteCount;
        //                        FuncInline.ComSMD[smdIndex].SendStop(siteIndex);
        //                    }
        //                }
        //                #endregion

        //                // PCB 정보 삭제
        //                if (pos != FuncInline.enumTeachingPos.None)// &&
        //                                                           //(pos != FuncInline.enumTeachingPos.NG1 || !FuncInline.NGOut))
        //                {
        //                    FuncInline.ClearPCBInfo(pos);
        //                }

        //                #region lift/conveyor action 삭제. 
        //                // 앞뒤가 같이 묶여 있는데 같이 처리? 
        //                // 앞쪽 먼저 처리하고 뒤쪽은 다시 판단하면 될까?
        //                // 버튼 상태는 어떤 기준으로 표시?
        //                FuncInline.PCBInfo[(int)pos].StopWatch.Stop();
        //                FuncInline.PCBInfo[(int)pos].StopWatch.Reset();
        //                switch ((FuncInline.enumErrorPart)i)
        //                {
        //                    case FuncInline.enumErrorPart.InConveyor:
        //                        DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
        //                        if (FuncInline.InConveyorAction == FuncInline.enumLiftAction.Output) // 배출시
        //                        {
        //                            FuncInline.FrontPassLineAction = FuncInline.enumLiftAction.Waiting;
        //                        }
        //                        else if (FuncInline.InConveyorAction == FuncInline.enumLiftAction.Input) // 공급시
        //                        {
        //                            // 공급시 제거는 스메마 끄고 제품 제거로. 
        //                        }
        //                        FuncInline.InConveyorAction = FuncInline.enumLiftAction.Waiting;
        //                        break;
        //                    case FuncInline.enumErrorPart.FrontPassline:
        //                        if (FuncInline.FrontPassLineAction == FuncInline.enumLiftAction.Input) // 공급
        //                        {
        //                            FuncInline.InConveyorAction = FuncInline.enumLiftAction.Waiting;
        //                        }
        //                        else if (FuncInline.FrontPassLineAction == FuncInline.enumLiftAction.Output) // 배출시
        //                        {
        //                            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //                        }
        //                        FuncInline.FrontPassLineAction = FuncInline.enumLiftAction.Waiting;
        //                        break;
        //                    case FuncInline.enumErrorPart.Lift1_Up:
        //                    case FuncInline.enumErrorPart.Rack1_Lift_Down:

        //                        FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //                        break;
        //                    case FuncInline.enumErrorPart.Rack2_PassLine:
        //                        if (FuncInline.RearPassLineAction == FuncInline.enumLiftAction.Input) // 공급시
        //                        {
        //                            FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
        //                        }
        //                        else if (FuncInline.RearPassLineAction == FuncInline.enumLiftAction.Output) // 배출시
        //                        {
        //                            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //                        }
        //                        FuncInline.RearPassLineAction = FuncInline.enumLiftAction.Waiting;
        //                        break;
        //                    case FuncInline.enumErrorPart.Lift2_Up:
        //                    case FuncInline.enumErrorPart.Lift2_Down:

        //                        FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //                        break;
        //                    case FuncInline.enumErrorPart.OutConveyor:
        //                        DIO.WriteDOData(FuncInline.enumDONames.Y412_1_SMEMA_After_Ready, false);
        //                        if (FuncInline.OutConveyorAction == FuncInline.enumLiftAction.Output) // 배출시
        //                        {
        //                        }
        //                        else if (FuncInline.OutConveyorAction == FuncInline.enumLiftAction.Input) // 공급시
        //                        {
        //                            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //                        }
        //                        FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
        //                        break;
        //                    case FuncInline.enumErrorPart.NgBuffer:
        //                        if (FuncInline.OutConveyorAction == FuncInline.enumLiftAction.Output) // 배출시
        //                        {
        //                        }
        //                        else if (FuncInline.OutConveyorAction == FuncInline.enumLiftAction.InputNG) // 공급시
        //                        {
        //                            FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
        //                        }
        //                        FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
        //                        FuncInline.NGOut = false;
        //                        FuncInline.NgAlarmWatch.Stop();
        //                        FuncInline.NgAlarmWatch.Reset();
        //                        break;
        //                    case FuncInline.enumErrorPart.Site1:
        //                    case FuncInline.enumErrorPart.Site2:
        //                    case FuncInline.enumErrorPart.Site3:
        //                    case FuncInline.enumErrorPart.Site4:
        //                    case FuncInline.enumErrorPart.Site5:
        //                    case FuncInline.enumErrorPart.Site6:
        //                    case FuncInline.enumErrorPart.Site7:
        //                    case FuncInline.enumErrorPart.Site8:
        //                    case FuncInline.enumErrorPart.Site9:
        //                    case FuncInline.enumErrorPart.Site10:
        //                    case FuncInline.enumErrorPart.Site11:
        //                    case FuncInline.enumErrorPart.Site12:
        //                    case FuncInline.enumErrorPart.Site13:
        //                    case FuncInline.enumErrorPart.Site14:
        //                    case FuncInline.enumErrorPart.Site15:
        //                    case FuncInline.enumErrorPart.Site16:
        //                    case FuncInline.enumErrorPart.Site17:
        //                    case FuncInline.enumErrorPart.Site18:
        //                    case FuncInline.enumErrorPart.Site19:
        //                    case FuncInline.enumErrorPart.Site20:
        //                    case FuncInline.enumErrorPart.Site21:
        //                    case FuncInline.enumErrorPart.Site22:
        //                    case FuncInline.enumErrorPart.Site23:
        //                    case FuncInline.enumErrorPart.Site24:
        //                    case FuncInline.enumErrorPart.Site25:
        //                    case FuncInline.enumErrorPart.Site26:
        //                    case FuncInline.enumErrorPart.Site27:
        //                    case FuncInline.enumErrorPart.Site28:
        //                    case FuncInline.enumErrorPart.Site29:
        //                    case FuncInline.enumErrorPart.Site30:
        //                    case FuncInline.enumErrorPart.Site31:
        //                    case FuncInline.enumErrorPart.Site32:
        //                    case FuncInline.enumErrorPart.Site33:
        //                    case FuncInline.enumErrorPart.Site34:
        //                    case FuncInline.enumErrorPart.Site35:
        //                    case FuncInline.enumErrorPart.Site36:
        //                    case FuncInline.enumErrorPart.Site37:
        //                    case FuncInline.enumErrorPart.Site38:
        //                    case FuncInline.enumErrorPart.Site39:
        //                    case FuncInline.enumErrorPart.Site40:
        //                        int siteIndex = int.Parse(((FuncInline.enumErrorPart)i).ToString().Replace("Site", "")) - 1;
        //                        if (FuncInline.SiteAction[siteIndex] != FuncInline.enumSiteAction.Waiting)
        //                        {
        //                            Func.WriteLog_Tester(((FuncInline.enumErrorPart)i).ToString() + " action change : " + FuncInline.SiteAction[siteIndex].ToString() + " ==> Waiting");
        //                        }
        //                        break;
        //                }
        //                // PCB 감지는 앞에서 거르고 있다. 그러므로 PCB 여부는 판단할 필요가 없지만. PCBInfo는 확인해야 함.
        //                // 배출시는 OutputLIft 제외하고 PCB 상태 초기화하고 뒷라인 감지여부에 따라 뒷라인 세팅
        //                // 투입시는 InputLIft 제외하고 앞쪽 상황 따라 다시 판단
        //                #endregion

        //                // 해당 파트 에러 삭제
        //                //if (part != FuncInline.enumErrorPart.Program)
        //                //{
        //                FuncError.RemoveError(part);
        //                FuncError.RemoveAllError();

        //                //}
        //                FuncInline.NeedPartClear[(int)part] = false;

        //                //if (convRun >= FuncInline.enumDONames.Y02_0_Input_Lift_Stopper_Up &&
        //                //    convRun <= FuncInline.enumDONames.Y03_0_Unloading_Conveyor_Run)
        //                if (convRun != FuncInline.enumDONames.Y01_7_)
        //                {
        //                    if (stopperUp != FuncInline.enumDONames.Y01_7_)
        //                    {
        //                        DIO.WriteDOData(stopperUp, true);
        //                    }
        //                    if (!GlobalVar.Simulation)
        //                    {
        //                        DIO.WriteDOData(convRun, true);
        //                    }
        //                }
        //                FuncInline.WaitMessage = "";
        //            }
        //        }

        //        FuncMotion.ServoAllInit();

        //        FuncError.RemoveAllError();

        //        GlobalVar.E_Stop = false;
        //        GlobalVar.DoorOpen = false;
        //        GlobalVar.LightCurtain = false;
        //        //GlobalVar.Loto_Switch = false;
        //        GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0 ? true : false;
        //        GlobalVar.SystemStatus = GlobalVar.SystemStatus > enumSystemStatus.Initialize ? enumSystemStatus.Manual : enumSystemStatus.BeforeInitialize;

        //        for (int i = 0; i < FuncInline.NeedPartClear.Length; i++)
        //        {
        //            FuncInline.NeedPartClear[i] = false;
        //        }

        //        FuncInline.DuplicatedPos[0] = FuncInline.enumTeachingPos.None;
        //        FuncInline.DuplicatedPos[1] = FuncInline.enumTeachingPos.None;

        //        while (clearWatch.IsRunning &&
        //            clearWatch.ElapsedMilliseconds < 2000)
        //        {
        //            Util.USleep(GlobalVar.ThreadSleep);
        //        }

        //        FuncInline.WaitMessage = "";

        //        Util.ResetWatch(ref clearWatch);
        //        FuncInline.StopAllConveyor();
        //        //*/
        //        Util.StartWatch(ref FuncInline.NgAlarmWatch); //  NG 버퍼 클리어 타이머 리셋

        //        #region 목적지 잃은 파트 목적지 삭제
        //        #region 리프트 1 상부. 목적지 사이트고 사이트에 이미 PCB 있으면 목적지 삭제
        //        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination <= FuncInline.enumTeachingPos.Site13_F_FT3 &&
        //            FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination = FuncInline.enumTeachingPos.None;
        //        }
        //        #endregion
        //        #region 리프트 1 하부. 목적지 사이트고 사이트 PCB 없으면 목적지 삭제
        //        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination <= FuncInline.enumTeachingPos.Site13_F_FT3 &&
        //            FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination = FuncInline.enumTeachingPos.None;
        //        }
        //        #endregion
        //        #region 리프트 2 상부. 목적지 사이트고 사이트에 이미 PCB 있으면 목적지 삭제
        //        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination >= FuncInline.enumTeachingPos.Site14_R_DT1 &&
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination <= FuncInline.enumTeachingPos.Site26_R_FT3 &&
        //            FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination = FuncInline.enumTeachingPos.None;
        //        }
        //        #endregion
        //        #region 리프트 2 하부. 목적지 사이트고 사이트 PCB 없으면 목적지 삭제
        //        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination >= FuncInline.enumTeachingPos.Site14_R_DT1 &&
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination <= FuncInline.enumTeachingPos.Site26_R_FT3 &&
        //            FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
        //        {
        //            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination = FuncInline.enumTeachingPos.None;
        //        }
        //        #endregion
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        debug(ex.ToString());
        //        debug(ex.StackTrace);
        //    }
        //}
        #endregion

        private void Close_Sub_Form()
        {
            try
            {
                if (dlgDetail != null)
                {
                    dlgDetail.Close();
                    dlgDetail.Dispose();
                    dlgDetail = null;
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            } // 에러나건 말건 기존 열린 거 닫고 다시 생성해서 연다
        }

        private void pbArray4_Click(object sender, EventArgs e)
        {

        }

        private void lblBuyerChange3_Click(object sender, EventArgs e)
        {

        }

        private void btnInputStop3_Click(object sender, EventArgs e)
        {

        }


        #region 상위 테스트 버튼

      
        private void btnSmemaAfter_Click_1(object sender, EventArgs e)
        {
            btnSmemaAfter.BackColor = btnSmemaAfter.BackColor == Color.Lime ? Color.White : Color.Lime;
            DIO.WriteDIData(FuncInline.enumDINames.X02_2_SMEMA_After_Ready, btnSmemaAfter.BackColor == Color.Lime);
            DIO.WriteDIData(FuncInline.enumDINames.X02_1_SMEMA_Before_Ready, btnSmemaAfter.BackColor == Color.Lime);
        }

      
        private void btnRTU_Click(object sender, EventArgs e)
        {
            RTUTest dlg = new RTUTest();
            dlg.Show();
        }

        private void btnSerial_Click(object sender, EventArgs e)
        {
            SerialTest dlg = new SerialTest();
            dlg.Show();
        }

        private void btnCNPass_Click(object sender, EventArgs e)
        {
            FuncInline.PassCNDuplication = !FuncInline.PassCNDuplication;
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            /*
            * 버튼 클릭시 PCB 수동투입 가능하도록 설정
            * 활성화시 황색으로 표시
            * 활성화시 로봇과 인터락 상황 아니면 InputLift 상단 대기함
            * 로봇 작업 없고 인터락 상황이면 로봇을 대기 위치로 보내야 함
            * Input 상태가 되면 원래로직으로 될 듯
            * 클릭하면 활성화, PCB 투입 후 비활성화 (일회성)
            */

            if (FuncInline.InShuttleAction != FuncInline.enumShuttleAction.Waiting ||
                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
            {
                return;
            }

            string enable = "Enable";
            if (FuncInline.InputPCB)
            {
                enable = "Disable";
            }
            if (FuncWin.MessageBoxOK(enable + " PCB Manual Input?"))
            {
                FuncLog.WriteLog("Main - InputPCB Click");
                FuncInline.InputPCB = !FuncInline.InputPCB;
            }
        }

        private void btnSMD_Click_1(object sender, EventArgs e)
        {
            SMDTest dlg = new SMDTest();
            dlg.Show();
        }
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnEStop_Click(object sender, EventArgs e)
        {
            DIO.WriteDIData(FuncInline.enumDINames.X00_3_Emergency_Stop, !DIO.GetDIData(FuncInline.enumDINames.X00_3_Emergency_Stop));
        }

        #region AutoInline UI 버튼
        private void pnSite_Click(object sender, EventArgs e)
        {
            try
            {
                dlgDetail.Close();
                dlgDetail.Dispose();
                dlgDetail = null;
            }
            catch { } // 에러나건 말건 기존 열린 거 닫고 다시 생성해서 연다
            try
            {
                if (sender.GetType() == typeof(Label))
                {
                    if (((Label)sender).Name.Contains("lblSite"))
                    {
                        int siteNo = int.Parse(((Label)sender).Name.Replace("lblSite", "")) - 1;
                        FuncInline.DetailSite = FuncInline.enumTeachingPos.Site1_F_DT1 + siteNo;
                    }
                    else
                    {
                        switch (((Label)sender).Name)
                        {
                            case "lblInConveyor":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.InConveyor;
                                break;
                            case "lblInShuttleArray1":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.InShuttle;
                                break;
                            case "lblFrontScan":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.FrontScanSite;
                                break;
                            case "lblFrontPassLineArray1":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.FrontPassLine;
                                break;
                            case "lblLift1UpArray1":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.Lift1_Up;
                                break;
                            case "lblRearPassLine":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.RearPassLine;
                                break;
                            case "lblRearNGLine":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.RearNGLine;
                                break;
                            case "lblLift2UpArray1":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.Lift2_Up;
                                break;
                            case "lblOutShuttleUp":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.OutShuttle_Up;
                                break;
                            case "lblOutShuttleDown":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.OutShuttle_Down;
                                break;
                            case "lblOutConveyor":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.OutConveyor;
                                break;
                            case "lblNGBuffer":
                                FuncInline.DetailSite = FuncInline.enumTeachingPos.NgBuffer;
                                break;
                        }
                    }
                }

                dlgDetail = new SiteDetail();
                dlgDetail.ShowDialog();
            }
            catch { }
        }

        private void btnSimulationMode_Click(object sender, EventArgs e)
        {
            if (FuncInline.TestPassMode == FuncInline.enumSMDStatus.Test_Pass)
            {
                FuncInline.TestPassMode = FuncInline.enumSMDStatus.Test_Fail;
            }
            else
            {
                FuncInline.TestPassMode = FuncInline.enumSMDStatus.Test_Pass;
            }
        }
        #endregion
    }

}