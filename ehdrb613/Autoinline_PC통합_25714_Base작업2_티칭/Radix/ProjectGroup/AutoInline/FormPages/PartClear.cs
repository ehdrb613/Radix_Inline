using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Radix
{
    /*
     * Manual.cs : 각 파트 및 장비의 수동 운전
     */

    public partial class PartClear : Form
    {



        #region 로컬변수
        private System.Threading.Timer timerCheck;
        private bool timerCheckDoing = false;

        private Button activeButton = null;
        private bool allClear = false; // 전체 초기화 여부
        private bool allPart = false; // 에러 상관 없이 모든 사이트 여부
        private Stopwatch clearWatch = new Stopwatch();

        private int siteIndex = -1;
        private FuncInline.enumTeachingPos sitePos = FuncInline.enumTeachingPos.InConveyor;

        #endregion

        #region 초기화 관련
        public PartClear()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            Util.Debug(str);
        }




        private void Manual_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;


            // 타이머 시작
            timerCheck = new System.Threading.Timer(new TimerCallback(TimerCheck), false, 0, 100);

            this.BringToFront();

        }

        private void Manual_FormClosed(object sender, FormClosedEventArgs e)
        {
            //if (this.Parent != null)
            //{
            try
            {
                GlobalVar.dlgOpened = false;
                //timerCheck.Dispose();
                //timerManual.Dispose();
                //this.Parent.BringToFront();
            }
            catch
            { }
            //}
        }

        #endregion


        #region 타이머 함수

        private void TimerCheck(Object state)
        {
            bool partClearNeeded = false;
            for (int i = 0; i < FuncInline.NeedPartClear.Length; i++)
            {
                if (FuncInline.NeedPartClear[i])
                {
                    partClearNeeded = true;
                    break;
                }
            }

            // 클리어할 파트 있는데 다른 페이지로 이동시 강제 이동
            if (partClearNeeded &&
                FuncInline.TabMain != FuncInline.enumTabMain.PartClear)
            {
                FuncInline.TabMain = FuncInline.enumTabMain.PartClear;
            }

            if (FuncInline.TabMain != FuncInline.enumTabMain.PartClear)
            {
                timerCheckDoing = false;
                return;
            }
            try
            {
                if (timerCheckDoing)
                {
                    return;
                }
                timerCheckDoing = true;
                //timerCheck.Dispose();

                if (!GlobalVar.GlobalStop &&
                    this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                {
                    int startTime = Environment.TickCount;

                    #region 버튼 색상 표시
                    FuncForm.SetButtonColor3_2(btnIn_Conveyor,
                                        FuncError.CheckError(FuncInline.enumErrorPart.InConveyor) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.InConveyor],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InConveyor].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnIn_Shuttle,
                                        FuncError.CheckError(FuncInline.enumErrorPart.InShuttle) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.InShuttle],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnFrontPassLine,
                                        FuncError.CheckError(FuncInline.enumErrorPart.FrontPassLine) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.FrontPassLine],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnFrontScan,
                                        FuncError.CheckError(FuncInline.enumErrorPart.FrontScanSite) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.FrontScanSite],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontScanSite].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnRack1_Lift_Up,
                                        FuncError.CheckError(FuncInline.enumErrorPart.Lift1_Up) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift1_Up],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnRack1_Lift_Down,
                                        FuncError.CheckError(FuncInline.enumErrorPart.Lift1_Down) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift1_Down],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnRearPassLine,
                                        FuncError.CheckError(FuncInline.enumErrorPart.RearPassLine) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.RearPassLine],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnRearNGLine,
                                FuncError.CheckError(FuncInline.enumErrorPart.RearNGLine) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.RearNGLine],
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnRack2_Lift_Up,
                                        FuncError.CheckError(FuncInline.enumErrorPart.Lift2_Up) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift2_Up],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnRack2_Lift_Down,
                                        FuncError.CheckError(FuncInline.enumErrorPart.Lift2_Down) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift2_Down],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnOut_Shuttle_Up,
                                      FuncError.CheckError(FuncInline.enumErrorPart.OutShuttle_Up) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.OutShuttle_Up],
                                              FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnOut_Shuttle_Down,
                                    FuncError.CheckError(FuncInline.enumErrorPart.OutShuttle_Down) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.OutShuttle_Down],
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnOut_Conveyor,
                                        FuncError.CheckError(FuncInline.enumErrorPart.OutConveyor) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.OutConveyor],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutConveyor].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnNG_Buffer,
                                        FuncError.CheckError(FuncInline.enumErrorPart.NgBuffer) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.NgBuffer],
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.NgBuffer].PCBStatus > FuncInline.enumSMDStatus.UnKnown);

                    FuncForm.SetButtonColor3_2(btnSystem,
                                           FuncError.CheckError(FuncInline.enumErrorPart.System) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.System],
                                           false);
                    FuncForm.SetButtonColor3_2(btnAllClear,
                                           GlobalVar.SystemErrored || partClearNeeded,
                                           false);
                    FuncForm.SetButtonColor3_2(btnBuzzerStop,
                                           DIO.GetDORead(FuncInline.enumDONames.Y412_2_Tower_Lamp_Buzzer),
                                           false);

                    FuncForm.SetButtonColor3_2(btnSite1,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 0)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 0],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 0 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 0].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite2,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 1)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 1],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 1 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 1].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite3,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 2)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 2],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 2 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 2].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite4,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 3)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 3],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 3 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 3].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite5,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 4)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 4],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 4 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 4].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite6,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 5)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 5],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 5 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 5].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite7,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 6)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 6],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 6 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 6].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite8,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 7)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 7],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 7 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 7].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite9,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 8)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 8],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 8 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 8].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite10,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 9)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 9],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 9 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 9].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite11,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 10)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 10],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 10 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 10].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite12,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 11)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 11],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 11 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 11].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite13,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 12)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 12],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 12 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 12].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite14,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 13)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 13],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 13 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 13].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite15,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 14)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 14],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 14 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 14].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite16,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 15)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 15],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 15 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 15].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite17,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 16)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 16],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 16 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 16].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite18,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 17)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 17],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 17 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 17].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite19,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 18)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 18],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 18 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 18].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite20,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 19)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 19],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 19 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 19].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite21,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 20)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 20],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 20 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 20].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite22,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 21)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 21],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 21 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 21].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite23,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 22)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 22],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 22 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 22].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite24,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 23)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 23],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 23 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 23].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite25,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 24)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 24],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 24 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 24].PCBStatus > FuncInline.enumSMDStatus.UnKnown);
                    FuncForm.SetButtonColor3_2(btnSite26,
                                        FuncError.CheckError((FuncInline.enumErrorPart.Site1_F_DT1 + 25)) || FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + 25],
                                                //DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 25 * GlobalVar.DIModuleGap) ||
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + 25].PCBStatus > FuncInline.enumSMDStatus.UnKnown);


                    #region PCB 여부
                    FuncForm.SetButtonColor2(btnPCBInConveyor,
                                           DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                                                DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor),
                                           Color.Yellow);
                    FuncForm.SetButtonColor2(btnPCBInShuttle,
                                          DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ||
                                               DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
                                               DIO.GetDIData(FuncInline.enumDINames.X304_0_In_Shuttle_Interlock_Sensor),
                                          Color.Yellow);
                    FuncForm.SetButtonColor2(btnPCBFrontPassLine,
                                                DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor),
                                           Color.Yellow);
                    FuncForm.SetButtonColor2(btnPCBFrontScan,
                                                DIO.GetDIData(FuncInline.enumDINames.X03_4_Front_Scan_PCB_Dock_Sensor),
                                           Color.Yellow);
                    FuncForm.SetButtonColor2(btnPCBLift1Up,
                                           DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                                                DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor),
                                           Color.Yellow);
                    FuncForm.SetButtonColor2(btnPCBLift1Down,
                                           DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                                                DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor),
                                           Color.Yellow);


                    FuncForm.SetButtonColor2(btnPCBRearPassLine,
                                           DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor),
                                           Color.Yellow);
                    FuncForm.SetButtonColor2(btnPCBRearNGLine,
                                           DIO.GetDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor),
                                           Color.Yellow);

                    FuncForm.SetButtonColor2(btnPCBLift2Up,
                                           DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                                                DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor),
                                           Color.Yellow);
                    FuncForm.SetButtonColor2(btnPCBLift2Down,
                                           DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                                                DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor),
                                           Color.Yellow);

                    FuncForm.SetButtonColor2(btnPCBOutShuttleUp,
                                       DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) ||
                                            DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor) ||
                                            DIO.GetDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor),
                                       Color.Yellow);

                    FuncForm.SetButtonColor2(btnPCBOutShuttleDown,
                                      DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) ||
                                           DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor) ||
                                           DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor),
                                      Color.Yellow);

                    FuncForm.SetButtonColor2(btnPCBOutConveyor,
                                           DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                                                DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor),
                                           Color.Yellow);

                    FuncForm.SetButtonColor2(btnPCBNGBuffer,
                                           DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor) ||
                                                DIO.GetDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor),
                                           Color.Yellow);

                    // PCB 감지 전용 버튼(1~26): ★여기서 기존 DIModuleGap 더하던 DI 접근을 SiteIoMaps로 변경
                    // btnPCBSite1 ~ btnPCBSite26를 배열로 묶어서 루프 처리
                    var pcbSiteButtons = new[]
                    {
                        btnPCBSite1,  btnPCBSite2,  btnPCBSite3,  btnPCBSite4,  btnPCBSite5,  btnPCBSite6,  btnPCBSite7,  btnPCBSite8,  btnPCBSite9,  btnPCBSite10,
                        btnPCBSite11, btnPCBSite12, btnPCBSite13, btnPCBSite14, btnPCBSite15, btnPCBSite16, btnPCBSite17, btnPCBSite18, btnPCBSite19, btnPCBSite20,
                        btnPCBSite21, btnPCBSite22, btnPCBSite23, btnPCBSite24, btnPCBSite25, btnPCBSite26
                    };

                    for (int i = 0; i < pcbSiteButtons.Length; i++)
                    {
                        FuncForm.SetButtonColor2(pcbSiteButtons[i], FuncInline.GetDockState(i), Color.Yellow);
                    }

                    //FuncForm.SetButtonColor2(btnPCBSite1, DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor),
                    //                       Color.Yellow);


                    #endregion

                    if (activeButton != null &&
                        activeButton.BackColor == Color.Red)
                    {
                        activeButton.BackColor = Color.DarkRed;
                    }
                    else if (activeButton != null &&
                        activeButton.BackColor == Color.Lime)
                    {
                        activeButton.BackColor = Color.LimeGreen;
                    }
                    else if (activeButton != null &&
                        activeButton.BackColor == Color.WhiteSmoke)
                    {
                        activeButton.BackColor = Color.DarkGray;
                    }
                    #endregion

                    #region PCB 언클램프 버튼 제어
                    // ───────────────────────────────────────────────────────────────
                    // PCB 언클램프 버튼 활성 표시(참고: 기존 Down/ClampBack DI 조합을 SiteIoMaps로 시도, 없으면 비활성)
                    // ───────────────────────────────────────────────────────────────
                    if (activeButton != null && activeButton.Name.Contains("btnSite"))
                    {
                        bool buttonActive = false;
                        if (int.TryParse(activeButton.Name.Replace("btnSite", ""), out int siteNo) && siteNo >= 1 && siteNo <= 26)
                        {
                            var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + (siteNo - 1));

                            // 핀 DOWN 센서 / 클램프 후진 센서를 맵에서 가져와서 판단 (둘 중 하나라도 “언클램프 필요” 상황이면 true)
                            bool pinDown = FuncInline.SiteIoMaps.TryGetContactUpDownDO(pos, out var pinDownDo) && DIO.GetDIData(pinDownDo);
                            bool clampON = FuncInline.SiteIoMaps.TryGetContactStopperDO(pos, out var clampDo) && DIO.GetDIData(clampDo);
                            if (pinDown || clampON) buttonActive = true;
                        }
                        // 여긴 화면 표시 용도로만 사용 (원래 주석 처리되어 있던 영역)
                        btnUnclampPCB.Enabled = buttonActive;
                        btnUnclampPCB.BackColor = Color.White;
                    }
                    else
                    {
                        btnUnclampPCB.Enabled = false;
                    }



                    #endregion

                    //Console.WriteLine("manual ui time : " + (Environment.TickCount - startTime).ToString());
                }));
                }

            }
            catch
            { }

            timerCheckDoing = false;
            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    timerCheck = new System.Threading.Timer(new TimerCallback(TimerCheck), false, 0, 100);
            //}
            if (GlobalVar.GlobalStop)
            {
                try
                {
                    timerCheck.Dispose();
                }
                catch { }
            }
        }
        #endregion

        private void refreshInfo()
        {
            FuncInline.enumTeachingPos pos = FuncInline.enumTeachingPos.None;
            FuncInline.enumErrorPart part = FuncInline.enumErrorPart.No_Error;
            string pcbDetect = "Empty";
            switch (activeButton.Name)
            {
                case "btnIn_Conveyor":
                    pos = FuncInline.enumTeachingPos.InConveyor;
                    part = FuncInline.enumErrorPart.InConveyor;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnIn_Shuttle":
                    pos = FuncInline.enumTeachingPos.InShuttle;
                    part = FuncInline.enumErrorPart.InShuttle;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnFrontPassLine":
                    pos = FuncInline.enumTeachingPos.FrontPassLine;
                    part = FuncInline.enumErrorPart.FrontPassLine;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnFrontScan":
                    pos = FuncInline.enumTeachingPos.FrontScanSite;
                    part = FuncInline.enumErrorPart.FrontScanSite;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X03_4_Front_Scan_PCB_Dock_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnRack1_Lift_Up":
                    pos = FuncInline.enumTeachingPos.Lift1_Up;
                    part = FuncInline.enumErrorPart.Lift1_Up;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnRack1_Lift_Down":
                    pos = FuncInline.enumTeachingPos.Lift1_Down;
                    part = FuncInline.enumErrorPart.Lift1_Down;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;

                //Rear
                case "btnRearPassLine":
                    pos = FuncInline.enumTeachingPos.RearPassLine;
                    part = FuncInline.enumErrorPart.RearPassLine;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnRearNGLine":
                    pos = FuncInline.enumTeachingPos.RearNGLine;
                    part = FuncInline.enumErrorPart.RearNGLine;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;

                case "btnRack2_Lift_Up":
                    pos = FuncInline.enumTeachingPos.Lift2_Up;
                    part = FuncInline.enumErrorPart.Lift2_Up;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnRack2_Lift_Down":
                    pos = FuncInline.enumTeachingPos.Lift2_Down;
                    part = FuncInline.enumErrorPart.Lift2_Down;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;

                case "btnOut_Shuttle_Up":
                    pos = FuncInline.enumTeachingPos.OutShuttle_Up;
                    part = FuncInline.enumErrorPart.OutShuttle_Up;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnOut_Shuttle_Down":
                    pos = FuncInline.enumTeachingPos.OutShuttle_Down;
                    part = FuncInline.enumErrorPart.OutShuttle_Down;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnOut_Conveyor":
                    pos = FuncInline.enumTeachingPos.OutConveyor;
                    part = FuncInline.enumErrorPart.OutConveyor;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnNG_Buffer":
                    pos = FuncInline.enumTeachingPos.NgBuffer;
                    part = FuncInline.enumErrorPart.NgBuffer;
                    pcbDetect = DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor) ||
                                                DIO.GetDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor) ?
                                "Detected" : "Not Detected";
                    break;
                case "btnSite1":
                case "btnSite2":
                case "btnSite3":
                case "btnSite4":
                case "btnSite5":
                case "btnSite6":
                case "btnSite7":
                case "btnSite8":
                case "btnSite9":
                case "btnSite10":
                case "btnSite11":
                case "btnSite12":
                case "btnSite13":
                case "btnSite14":
                case "btnSite15":
                case "btnSite16":
                case "btnSite17":
                case "btnSite18":
                case "btnSite19":
                case "btnSite20":
                case "btnSite21":
                case "btnSite22":
                case "btnSite23":
                case "btnSite24":
                case "btnSite25":
                case "btnSite26":
                case "btnSite27":
                case "btnSite28":
                case "btnSite29":
                case "btnSite30":
                case "btnSite31":
                case "btnSite32":
                case "btnSite33":
                case "btnSite34":
                case "btnSite35":
                case "btnSite36":
                case "btnSite37":
                case "btnSite38":
                case "btnSite39":
                case "btnSite40":
                    // 테스트사이트는 묶어서 처리
                    pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + (int.Parse(activeButton.Name.Replace("btnSite", ""))) - 1);
                    part = FuncInline.enumErrorPart.Site1_F_DT1 + (int.Parse(activeButton.Name.Replace("btnSite", ""))) - 1;

                    if (FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi))
                    {
                        pcbDetect = DIO.GetDIData(dockDi) ?
                                "Detected" : "Not Detected";
                    }

                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(dockDi, false);
                    }



                    break;
                case "btnSystem":
                    break;
                default: // 해당 사항 없음
                    return;
            }

            string text = activeButton.Name.Replace("btn", "") + " Info : \r\n\r\n"; // 사이트 이름
            if (pcbDetect.Length > 0) // 센서 감지 유무
            {
                text += "PCB " + pcbDetect + "\r\n\r\n";
            }
            text += "PCB Status - " + FuncInline.PCBInfo[(int)pos].PCBStatus.ToString() + "\r\n\r\n"; // PCB 처리 상태
            // 에러 상태
            if (!FuncError.CheckError(part))
            {
                text += "No Error\r\n\r\n";
            }
            else
            {
                text += "Alarm : ";
                for (int i = GlobalVar.SystemErrorQueue.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        FuncInline.structError err = GlobalVar.SystemErrorQueue.ElementAt(i);
                        if (err.ErrorPart == part)
                        {
                            text += ((FuncInline.enumErrorCode)err.ErrorCode).ToString() + "(" + err.ErrorCode + ")";
                        }
                    }
                    catch (Exception ex)
                    {
                        debug(ex.ToString());
                        debug(ex.StackTrace);
                    }
                }
                text += "\r\n\r\n";
            }
            if (FuncInline.PCBInfo[(int)pos].PCBStatus >= FuncInline.enumSMDStatus.Before_Command &&
                FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.No_Test) // Array 정보
            {
                for (int i = 0; i < FuncInline.PCBInfo[(int)pos].SMDStatus.Length; i++)
                {
                    if (FuncInline.ArrayUse[i] &&
                        FuncInline.PCBInfo[(int)pos].SMDStatus[i] >= FuncInline.enumSMDStatus.Before_Command &&
                        FuncInline.PCBInfo[(int)pos].SMDStatus[i] != FuncInline.enumSMDStatus.No_Test)
                    {
                        text += "Array " + (i + 1).ToString() + " - " +
                                FuncInline.PCBInfo[(int)pos].Barcode[i] + " " +
                                FuncInline.PCBInfo[(int)pos].SMDStatus[i].ToString() + "\r\n";
                    }
                }
            }
            tbSiteInfo.Text = text;
        }

        private void Site_Click(object sender, EventArgs e)
        {
            activeButton = (Button)sender;
            FuncInline.enumTeachingPos pos = FuncInline.enumTeachingPos.None;
            FuncInline.enumErrorPart part = FuncInline.enumErrorPart.No_Error;
            switch (activeButton.Name)
            {
                case "btnIn_Conveyor":
                    pos = FuncInline.enumTeachingPos.InConveyor;
                    part = FuncInline.enumErrorPart.InConveyor;
                    break;
                case "btnIn_Shuttle":
                    pos = FuncInline.enumTeachingPos.InShuttle;
                    part = FuncInline.enumErrorPart.InShuttle;
                    break;
                case "btnFrontPassLine":
                    pos = FuncInline.enumTeachingPos.FrontPassLine;
                    part = FuncInline.enumErrorPart.FrontPassLine;
                    break;
                case "btnFrontScan":
                    pos = FuncInline.enumTeachingPos.FrontScanSite;
                    part = FuncInline.enumErrorPart.FrontScanSite;
                    break;
                case "btnRack1_Lift_Up":
                    pos = FuncInline.enumTeachingPos.Lift1_Up;
                    part = FuncInline.enumErrorPart.Lift1_Up;
                    break;
                case "btnRack1_Lift_Down":
                    pos = FuncInline.enumTeachingPos.Lift1_Down;
                    part = FuncInline.enumErrorPart.Lift1_Down;
                    break;
                case "btnRearPassLine":
                    pos = FuncInline.enumTeachingPos.RearPassLine;
                    part = FuncInline.enumErrorPart.RearPassLine;
                    break;
                case "btnRearNGLine":
                    pos = FuncInline.enumTeachingPos.RearNGLine;
                    part = FuncInline.enumErrorPart.RearNGLine;
                    break;
                case "btnRack2_Lift_Up":
                    pos = FuncInline.enumTeachingPos.Lift2_Up;
                    part = FuncInline.enumErrorPart.Lift2_Up;
                    break;
                case "btnRack2_Lift_Down":
                    pos = FuncInline.enumTeachingPos.Lift2_Down;
                    part = FuncInline.enumErrorPart.Lift2_Down;
                    break;
                case "btnOut_Shuttle_Up":
                    pos = FuncInline.enumTeachingPos.OutShuttle_Up;
                    part = FuncInline.enumErrorPart.OutShuttle_Up;
                    break;
                case "btnOut_Shuttle_Down":
                    pos = FuncInline.enumTeachingPos.OutShuttle_Down;
                    part = FuncInline.enumErrorPart.OutShuttle_Down;
                    break;
                case "btnOut_Conveyor":
                    pos = FuncInline.enumTeachingPos.OutConveyor;
                    part = FuncInline.enumErrorPart.OutConveyor;
                    break;
                case "btnNG_Buffer":
                    pos = FuncInline.enumTeachingPos.NgBuffer;
                    part = FuncInline.enumErrorPart.NgBuffer;
                    break;
                case "btnSite1":
                case "btnSite2":
                case "btnSite3":
                case "btnSite4":
                case "btnSite5":
                case "btnSite6":
                case "btnSite7":
                case "btnSite8":
                case "btnSite9":
                case "btnSite10":
                case "btnSite11":
                case "btnSite12":
                case "btnSite13":
                case "btnSite14":
                case "btnSite15":
                case "btnSite16":
                case "btnSite17":
                case "btnSite18":
                case "btnSite19":
                case "btnSite20":
                case "btnSite21":
                case "btnSite22":
                case "btnSite23":
                case "btnSite24":
                case "btnSite25":
                case "btnSite26":

                    int n = int.Parse(activeButton.Name.Replace("btnSite", "")); // 1~26
                    // 테스트사이트는 묶어서 처리
                    pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + (n - 1));
                    part = FuncInline.enumErrorPart.Site1_F_DT1 + (n - 1);

                    #region 사이트 경우 오픈 여부 체크
                    if (//!GlobalVar.Simulation &&
                        pos >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                        pos <= FuncInline.enumTeachingPos.Site26_R_FT3)
                    {
                        siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                        sitePos = pos;
                        // 도크 DI로 PCB 존재 시 언클램프 안내 후 즉시 언클램프/핀업
                        if (!GlobalVar.Simulation &&
                            FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi) &&
                            DIO.GetDIData(dockDi))
                        {
                            //세대별 사이트 명칭 변경
                            string label = FuncInline.SiteDisplay.GetSiteDisplayName(pos);

                            if (FuncWin.MessageBoxOK("PCB Detected in " + label + ". Unclamp site?"))
                            {
                                // 클램프 해제: Stopper DO(클램프 전진 DO) OFF
                                if (FuncInline.SiteIoMaps.TryGetContactStopperDO(pos, out var stopperDo))
                                    DIO.WriteDOData(stopperDo, false);

                                // 핀 UP: 단동 솔레노이드(Down일 때 TRUE) → FALSE로 써서 상승
                                if (FuncInline.SiteIoMaps.TryGetContactUpDownDO(pos, out var contactDownDo))
                                    DIO.WriteDOData(contactDownDo, false);
                            }
                            return;
                        }

                    }
                    #endregion

                    if (GlobalVar.Simulation)
                    {
                        if (FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi))
                        {
                            DIO.WriteDIData(dockDi, false);
                        }
                    }

                    break;
                case "btnSystem":
                    if (!allClear &&
                        FuncWin.MessageBoxOK("Clear System Part"))
                    {
                        FuncError.RemoveError(FuncInline.enumErrorPart.System);
                        FuncError.RemoveAllError();
                        return;
                    }
                    break;
                default: // 해당 사항 없음
                    return;
            }

            if (pos >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                pos <= FuncInline.enumTeachingPos.Site26_R_FT3)
            {
                siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                sitePos = pos;
            }
            else
            {
                siteIndex = -1;
                sitePos = FuncInline.enumTeachingPos.None;
            }

            if (allClear ||
                FuncWin.MessageBoxOK("Clear Part " + pos.ToString()))
            {
                btnPartClear_Click();
            }
            refreshInfo();
        }

        private bool btnPartClear_Click()
        {
            if (activeButton == null)
            {
                if (!allClear)
                {
                    FuncWin.TopMessageBox("Select part to clear");
                }
                return false;
            }
            bool diCheck = false;
            FuncInline.enumTeachingPos pos = FuncInline.enumTeachingPos.None;
            FuncInline.enumErrorPart part = FuncInline.enumErrorPart.System;
            FuncInline.enumDONames convRun = FuncInline.enumDONames.Y300_3; //안쓰는 IO지정
            FuncInline.enumDONames convRun2 = FuncInline.enumDONames.Y300_3;
            FuncInline.enumDONames stopperDown = FuncInline.enumDONames.Y300_3;
            FuncInline.enumDONames stopperDown2 = FuncInline.enumDONames.Y300_3;
            switch (activeButton.Name)
            {
                case "btnIn_Conveyor":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor, false);

                    }
                    pos = FuncInline.enumTeachingPos.InConveyor;
                    part = FuncInline.enumErrorPart.InConveyor;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor);
                    convRun = FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw;
                    //stopperUp = FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL;
                    break;
                case "btnIn_Shuttle":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.InShuttle;
                    part = FuncInline.enumErrorPart.InShuttle;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw;
                    stopperDown = FuncInline.enumDONames.Y302_2_IN_Shuttle_CONTACT_STOPPER_SOL;
                    break;
                case "btnFrontPassLine":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.FrontPassLine;
                    part = FuncInline.enumErrorPart.FrontPassLine;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw;
                    stopperDown = FuncInline.enumDONames.Y1_7_Front_PASSLINE_PCB_STOPPER_SOL;
                    break;
                case "btnFrontScan":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X03_4_Front_Scan_PCB_Dock_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.FrontScanSite;
                    part = FuncInline.enumErrorPart.FrontScanSite;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X03_4_Front_Scan_PCB_Dock_Sensor);
                    convRun = FuncInline.enumDONames.Y406_1_Front_SCAN_Motor_Cw;
                    convRun2 = FuncInline.enumDONames.Y406_3_Front_SCAN_Motor_Ccw;
                    stopperDown = FuncInline.enumDONames.Y3_7_Front_SCAN_STOPPER_SOL;

                    break;
                case "btnRack1_Lift_Up":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.Lift1_Up;
                    part = FuncInline.enumErrorPart.Lift1_Up;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw;
                    convRun2 = FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw;
                    stopperDown = FuncInline.enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL;

                    break;
                case "btnRack1_Lift_Down":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.Lift1_Down;
                    part = FuncInline.enumErrorPart.Lift1_Down;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw;
                    convRun2 = FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw;
                    stopperDown = FuncInline.enumDONames.Y300_1_Front_Lift_CONTACT_STOPPER_SOL;
                    break;

                //Rear
                case "btnRearPassLine":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.RearPassLine;
                    part = FuncInline.enumErrorPart.RearPassLine;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor);
                    convRun = FuncInline.enumDONames.Y305_4_Rear_PassLine_Motor_Cw;
                    stopperDown = FuncInline.enumDONames.Y1_6_Rear_OK_PassLine_CONTACT_STOPPER_SOL;
                    break;
                case "btnRearNGLine":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.RearNGLine;
                    part = FuncInline.enumErrorPart.RearNGLine;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y404_0_Rear_NgLine_Motor_Cw;
                    stopperDown = FuncInline.enumDONames.Y4_5_Rear_NG_PassLine_CONTACT_STOPPER_SOL;
                    break;

                case "btnRack2_Lift_Up":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.Lift2_Up;
                    part = FuncInline.enumErrorPart.Lift2_Up;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw;
                    convRun2 = FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw;
                    stopperDown = FuncInline.enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL;
                    stopperDown2 = FuncInline.enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL;
                    break;
                case "btnRack2_Lift_Down":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.Lift2_Down;
                    part = FuncInline.enumErrorPart.Lift2_Down;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw;
                    convRun2 = FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw;
                    stopperDown = FuncInline.enumDONames.Y300_0_Rear_Lift_CONTACT_STOPPER_Out_SOL;
                    stopperDown2 = FuncInline.enumDONames.Y302_0_Rear_Lift_CONTACT_STOPPER_IN_SOL;
                    break;

                case "btnOut_Shuttle_Up":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.OutShuttle_Up;
                    part = FuncInline.enumErrorPart.OutShuttle_Up;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw;
                    convRun2 = FuncInline.enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw;
                    stopperDown = FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_IN_SOL;
                    stopperDown2 = FuncInline.enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_Out_SOL;
                    break;
                case "btnOut_Shuttle_Down":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.OutShuttle_Down;
                    part = FuncInline.enumErrorPart.OutShuttle_Down;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw;
                    convRun2 = FuncInline.enumDONames.Y400_7_Out_Shuttle_Ng_Motor_Ccw;
                    stopperDown = FuncInline.enumDONames.Y300_2_Out_Shuttle_CONTACT_STOPPER_IN_SOL;
                    stopperDown2 = FuncInline.enumDONames.Y302_1_Out_Shuttle_CONTACT_STOPPER_Out_SOL;

                    break;
                case "btnOut_Conveyor":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.OutConveyor;
                    part = FuncInline.enumErrorPart.OutConveyor;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor);
               
                    convRun = FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw;
                    break;
                case "btnNG_Buffer":
                    if (GlobalVar.Simulation)
                    {
                        DIO.WriteDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor, false);
                        DIO.WriteDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor, false);
                    }
                    pos = FuncInline.enumTeachingPos.NgBuffer;
                    part = FuncInline.enumErrorPart.NgBuffer;
                    diCheck = DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor) ||
                                                DIO.GetDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor);
                    convRun = FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw;
                    break;
                case "btnSite1":
                case "btnSite2":
                case "btnSite3":
                case "btnSite4":
                case "btnSite5":
                case "btnSite6":
                case "btnSite7":
                case "btnSite8":
                case "btnSite9":
                case "btnSite10":
                case "btnSite11":
                case "btnSite12":
                case "btnSite13":
                case "btnSite14":
                case "btnSite15":
                case "btnSite16":
                case "btnSite17":
                case "btnSite18":
                case "btnSite19":
                case "btnSite20":
                case "btnSite21":
                case "btnSite22":
                case "btnSite23":
                case "btnSite24":
                case "btnSite25":
                case "btnSite26":

                  
                    // 테스트사이트는 묶어서 처리
                    pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + (int.Parse(activeButton.Name.Replace("btnSite", ""))) - 1);
                    part = FuncInline.enumErrorPart.Site1_F_DT1 + (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;

                    if (FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(dockDi, false);
                        }
                        diCheck = DIO.GetDIData(dockDi);
                    }
                    if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(pos, out var docw,out var doccw))
                    {

                        convRun = docw;
                        convRun2 = doccw;
                    }

                    break;
                case "btnSystem":
                    FuncMotion.ServoAllInit();
                    break;
              
                default: // 해당 사항 없음
                    return false;
            }

            //*
            if (diCheck)
            {
                if (!allPart)
                {
                    FuncWin.TopMessageBox("Can't clear part while PCB detected. Remove PCB and clear again.");
                }
                return false;
            }
            //*/
            FuncInline.WaitMessage = "Clearing Part " + pos.ToString();
            FuncLog.WriteLog("PartClear - " + pos.ToString());


            #region 사이트 경우 테스트중인 array에 STS전송
            if (pos >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                pos <= FuncInline.enumTeachingPos.Site26_R_FT3)
            {
                FuncInline.SiteAction[(int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1] = FuncInline.enumSiteAction.Waiting;
                if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Testing)
                {
                    int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                    int smdIndex = FuncInline.MapSmdIndex(siteIndex);    //세대별 변환
                    FuncInline.ComSMD[smdIndex].SendStop(siteIndex);
                }
            }
            #endregion

            // PCB 정보 삭제
            if (pos != FuncInline.enumTeachingPos.None)// &&
                                                       //(pos != FuncInline.enumTeachingPos.NG1 || !FuncInline.NGOut))
            {
                FuncInline.ClearPCBInfo(pos);
            }



            #region lift/conveyor action 삭제. 
            // 앞뒤가 같이 묶여 있는데 같이 처리? 
            // 앞쪽 먼저 처리하고 뒤쪽은 다시 판단하면 될까?
            // 버튼 상태는 어떤 기준으로 표시?
            FuncInline.PCBInfo[(int)pos].StopWatch.Stop();
            FuncInline.PCBInfo[(int)pos].StopWatch.Reset();
            switch (activeButton.Name)
            {
                case "btnIn_Conveyor":
                    DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
                  
                    if (FuncInline.InConveyorAction == FuncInline.enumLiftAction.Input) // 공급시
                    {
                        // 공급시 제거는 스메마 끄고 제품 제거로. 
                    }
                    FuncInline.InConveyorAction = FuncInline.enumLiftAction.Waiting;
                    FuncInline.InputPCB = false;
                    break;
                case "btnIn_Shuttle":
                    DIO.WriteDOData(FuncInline.enumDONames.Y4_2_SMEMA_Before_Ready, false);
                    if (FuncInline.InShuttleAction == FuncInline.enumShuttleAction.Input) // 공급시
                    {
                        FuncInline.InConveyorAction = FuncInline.enumLiftAction.Waiting;
                    }
                    else if (FuncInline.InConveyorAction == FuncInline.enumLiftAction.Input) // 공급시
                    {
                        // 공급시 제거는 스메마 끄고 제품 제거로. 
                    }
                    FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                    FuncInline.InputPCB = false;
                    break;

                case "btnFrontPassLine":
                    if (FuncInline.FrontPassLineAction == FuncInline.enumLiftAction.Input) // 공급
                    {
                        FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                    }
                    else if (FuncInline.FrontPassLineAction == FuncInline.enumLiftAction.Output) // 배출시
                    {
                        FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
                    }
                    FuncInline.FrontPassLineAction = FuncInline.enumLiftAction.Waiting;
                    break;
                case "btnRack1_Lift_Up":
                case "btnRack1_Lift_Down":
                    if (FuncInline.Lift1Action == FuncInline.enumLiftAction.InputUp &&
                        FuncInline.FrontPassLineAction == FuncInline.enumLiftAction.Output) // 배출시
                    {
                        GlobalVar.RightMovinPassLine1ToLift1 = false;
                    }
                    else if (FuncInline.Lift1Action == FuncInline.enumLiftAction.OutShuttleUP &&
                        FuncInline.RearPassLineAction == FuncInline.enumLiftAction.Input) // 배출시
                    {
                        GlobalVar.RightMovingLift1ToPassLine2 = false;
                    }
                    FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
                    break;
                case "btnRearPassLine":
                    if (FuncInline.RearPassLineAction == FuncInline.enumLiftAction.Input) // 공급시
                    {
                        if (FuncInline.Lift1Action == FuncInline.enumLiftAction.OutShuttleUP) // 배출시
                        {
                            GlobalVar.RightMovingLift1ToPassLine2 = false;
                        }
                        FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
                    }
                    else if (FuncInline.RearPassLineAction == FuncInline.enumLiftAction.Output) // 배출시
                    {
                        FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
                    }
                    FuncInline.RearPassLineAction = FuncInline.enumLiftAction.Waiting;
                    break;
                case "btnRack2_Lift_Up":
                case "btnRack2_Lift_Down":

                    FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
                    break;
                case "btnOut_Conveyor":
                    DIO.WriteDOData(FuncInline.enumDONames.Y412_1_SMEMA_After_Ready, false);
                    if (FuncInline.OutConveyorAction == FuncInline.enumLiftAction.Input) // 공급시
                    {
                        FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
                    }
                    FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
                    break;
                case "btnNG_Buffer":
                    if (FuncInline.OutConveyorAction == FuncInline.enumLiftAction.InputNG) // 공급시
                    {
                        FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
                    }
                    FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
                    FuncInline.NGOut = false;
                    FuncInline.NgAlarmWatch.Stop();
                    FuncInline.NgAlarmWatch.Reset();
                    break;
                case "btnSite1":
                case "btnSite2":
                case "btnSite3":
                case "btnSite4":
                case "btnSite5":
                case "btnSite6":
                case "btnSite7":
                case "btnSite8":
                case "btnSite9":
                case "btnSite10":
                case "btnSite11":
                case "btnSite12":
                case "btnSite13":
                case "btnSite14":
                case "btnSite15":
                case "btnSite16":
                case "btnSite17":
                case "btnSite18":
                case "btnSite19":
                case "btnSite20":
                case "btnSite21":
                case "btnSite22":
                case "btnSite23":
                case "btnSite24":
                case "btnSite25":
                case "btnSite26":
                case "btnSite27":
                case "btnSite28":
                case "btnSite29":
                case "btnSite30":
                case "btnSite31":
                case "btnSite32":
                case "btnSite33":
                case "btnSite34":
                case "btnSite35":
                case "btnSite36":
                case "btnSite37":
                case "btnSite38":
                case "btnSite39":
                case "btnSite40":
                    int siteIndex = int.Parse(activeButton.Name.Replace("btnSite", "")) - 1;
                    if (FuncInline.SiteAction[siteIndex] != FuncInline.enumSiteAction.Waiting)
                    {
                        FuncLog.WriteLog_Tester(activeButton.Name.Replace("btn", "") + " action change : " + FuncInline.SiteAction[siteIndex].ToString() + " ==> Waiting");
                    }

                    DIO.WriteDOData(FuncInline.enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
                    break;
            }
            // PCB 감지는 앞에서 거르고 있다. 그러므로 PCB 여부는 판단할 필요가 없지만. PCBInfo는 확인해야 함.
            // 배출시는 OutputLIft 제외하고 PCB 상태 초기화하고 뒷라인 감지여부에 따라 뒷라인 세팅
            // 투입시는 InputLIft 제외하고 앞쪽 상황 따라 다시 판단
            #endregion

            // 해당 파트 에러 삭제
            //if (part != FuncInline.enumErrorPart.Program)
            //{
            FuncError.RemoveError(part);
            FuncError.RemoveAllError();

            //}
            FuncInline.NeedPartClear[(int)part] = false;
            RefreshAlarm();

            //if (convRun >= FuncInline.enumDONames.Y02_0_Input_Lift_Stopper_Up &&
            //    convRun <= FuncInline.enumDONames.Y03_0_Unloading_Conveyor_Run)
            if (convRun != FuncInline.enumDONames.Y300_3)
            {
                if (stopperDown != FuncInline.enumDONames.Y300_3)
                {
                    DIO.WriteDOData(stopperDown, false);
                }
                if (!GlobalVar.Simulation)
                {
                    DIO.WriteDOData(convRun, true);
                }
                //Util.USleep(2000);
                //DIO.WriteDOData(convRun, false);
                if (!allClear)
                {
                    Util.StartWatch(ref clearWatch);
                    while (clearWatch.IsRunning &&
                        clearWatch.ElapsedMilliseconds < 2000)
                    {
                        Util.USleep(GlobalVar.ThreadSleep);
                    }
                    Util.ResetWatch(ref clearWatch);
                    DIO.WriteDOData(convRun, false);
                    FuncInline.WaitMessage = "";
                }
            }
            FuncInline.WaitMessage = "";
            //refreshInfo();
            return true;
        }

        private void btnAlarmClear_Click(object sender, EventArgs e)
        {
            GlobalVar.SystemStatus = GlobalVar.SystemStatus <= enumSystemStatus.Initialize ? enumSystemStatus.BeforeInitialize : enumSystemStatus.Manual;

            FuncError.RemoveAllError();
            btnRefresh_Click(sender, e);
            // 타워램프와 부저 상태를 유지하기 위해 알람 클리어시 상태를 변경한다.

            //* // 리셋하면 위치값까지 초기화되어서 초기화하면 안 된다.
            //for (int i = 0; i < Enum.GetValues(typeof(FuncInline.enumPMCAxis)).Length; i++)
            //{
            //    //PMCClass.Reset((FuncInline.enumPMCAxis)i); // 에러 있는  경우만 리셋한다.
            //}
            //*/

            GlobalVar.E_Stop = false;
            GlobalVar.DoorOpen = false;
            GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0 ? true : false;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //String sql = "select [Date],[Time],[Part],[ErrorCode],[ErrorName],[Description] " +
            //                "from [SystemError] " +
            //                "where [Clear] = '0' " +
            //                "order by [Date] desc, [Time] desc";
            //SqlDataReader rs = GlobalVar.Sql.Read(sql);
            //SqlDataReader rs = FuncInline.GetUnClearedSystemError();
            //if (rs != null)
            //{
            //    dataGridError.Rows.Clear();
            //    int rowNum = 0;
            //    while (rs.Read())
            //    {
            //        dataGridError.Rows.Add(rs[0].ToString(), rs[1].ToString(), rs[2].ToString(), rs[3].ToString(), rs[4].ToString(), rs[5].ToString());
            //        dataGridError.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

            //        rowNum++;
            //    }
            //    rs.Close();
            //}
            RefreshAlarm();
        }

        public void RefreshAlarm()
        {
            string[,] rs = FuncInline.GetUnClearedSystemError();
            if (rs != null)
            {
                dataGridError.Rows.Clear();
                int rowNum = 0;
                //while (rs.Read())
                for (int j = 0; j < rs.GetLength(0); j++)
                {
                    dataGridError.Rows.Add(rs[j, 0].ToString(), rs[j, 1].ToString(), rs[j, 2].ToString(), rs[j, 3].ToString(), rs[j, 4].ToString(), rs[j, 5].ToString());
                    dataGridError.Rows[rowNum].DefaultCellStyle.BackColor = rowNum % 2 > 0 ? Color.Cyan : Color.WhiteSmoke;

                    rowNum++;
                }
                //rs.Close();
            }
        }

        private void btnAllClear_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Name == "btnAllClear")
            {
                //*
                string msg = "Clear All Errored Part?";
                allPart = ((Button)sender).Name == "btnAllPartClear";
                if (((Button)sender).Name == "btnAllPartClear")
                {
                    msg = "Clear All Part?";
                }
                if (!FuncWin.MessageBoxOK(msg))
                {
                    allClear = false;
                    return;
                }

                FuncInline.WaitMessage = "Clearing Parts";
                allClear = true;
                Util.StartWatch(ref clearWatch);

                for (int i = 0; i < FuncInline.NeedPartClear.Length; i++)
                {
                    if (FuncInline.NeedPartClear[i] ||
                        ((Button)sender).Name == "btnAllPartClear")
                    {
                        Control[] controls = Controls.Find("btn" + ((FuncInline.enumErrorPart)i).ToString(), true);
                        if (controls.Length > 0)
                        {
                            activeButton = (Button)controls[0];

                            FuncInline.enumErrorPart part = FuncInline.enumErrorPart.System;
                            // 파트에러 있거나, 
                            // 클리어 필요하거나, 
                            // PCB 정보 있거나, 
                            // PCB 감지되거나,
                            // 액션이 있으면 클리어
                            bool partError = false;
                            bool needClear = false;
                            bool pcbInfo = false;
                            bool pcbCheck = false;
                            bool actionCheck = false;
                            switch (activeButton.Name)
                            {
                                case "btnIn_Conveyor":
                                    part = FuncInline.enumErrorPart.InConveyor;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.InConveyor);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.InConveyor];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor);
                                    actionCheck = FuncInline.InConveyorAction != FuncInline.enumLiftAction.Waiting;
                                    break;
                                case "btnIn_Shuttle":
                                    part = FuncInline.enumErrorPart.InShuttle;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.InShuttle);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.InShuttle];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.InShuttle].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor);
                                    actionCheck = FuncInline.InShuttleAction != FuncInline.enumShuttleAction.Waiting;
                                    break;
                                case "btnFrontPassLine":
                                    part = FuncInline.enumErrorPart.FrontPassLine;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.FrontPassLine);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.FrontPassLine];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontPassLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor);
                                    actionCheck = FuncInline.FrontPassLineAction != FuncInline.enumLiftAction.Waiting;
                                    break;
                                case "btnFrontScan":
                                    part = FuncInline.enumErrorPart.FrontScanSite;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.FrontScanSite);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.FrontScanSite];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.FrontScanSite].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X03_4_Front_Scan_PCB_Dock_Sensor);
                                    actionCheck = FuncInline.FrontScanSiteAction != FuncInline.enumLiftAction.Waiting;
                                    break;
                                case "btnRack1_Lift_Up":
                                    part = FuncInline.enumErrorPart.Lift1_Up;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.Lift1_Up);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift1_Up];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor);
                                    actionCheck = FuncInline.Lift1Action != FuncInline.enumLiftAction.Waiting;
                                    break;
                                case "btnRack1_Lift_Down":
                                    part = FuncInline.enumErrorPart.Lift1_Down;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.Lift1_Down);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift1_Down];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor);
                                    actionCheck = FuncInline.Lift1Action != FuncInline.enumLiftAction.Waiting;
                                    break;

                                case "btnRack2_Lift_Up":
                                    part = FuncInline.enumErrorPart.Lift2_Up;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.Lift2_Up);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift2_Up];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor);
                                    actionCheck = FuncInline.Lift2Action != FuncInline.enumLiftAction.Waiting;
                                    break;
                                case "btnRack2_Lift_Down":
                                    {
                                        part = FuncInline.enumErrorPart.Lift2_Down;
                                        partError = FuncError.CheckError(FuncInline.enumErrorPart.Lift2_Down);
                                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Lift2_Down];
                                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown;


                                        pcbCheck =
                                            DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                                            DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor);
                                        actionCheck = FuncInline.Lift2Action != FuncInline.enumLiftAction.Waiting;
                                        break;
                                    }
                                case "btnRearPassLine":
                                    {
                                        part = FuncInline.enumErrorPart.RearPassLine;
                                        partError = FuncError.CheckError(FuncInline.enumErrorPart.RearPassLine);
                                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.RearPassLine];
                                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearPassLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown;

                                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor);

                                        actionCheck = FuncInline.RearPassLineAction != FuncInline.enumLiftAction.Waiting;
                                        break;
                                    }
                                case "btnRearNGLine":
                                    {
                                        part = FuncInline.enumErrorPart.RearNGLine;
                                        partError = FuncError.CheckError(FuncInline.enumErrorPart.RearNGLine);
                                        needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.RearNGLine];
                                        pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.RearNGLine].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                        pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor);
                                        actionCheck = FuncInline.RearNGLineAction != FuncInline.enumLiftAction.Waiting;
                                        break;
                                    }
                                case "btnOut_Shuttle_Up":
                                    part = FuncInline.enumErrorPart.OutShuttle_Up;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.OutShuttle_Up);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.OutShuttle_Up];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) ||
                                                 DIO.GetDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor);
                                    actionCheck = FuncInline.OutShuttleUpAction != FuncInline.enumShuttleAction.Waiting;
                                    break;
                                case "btnOut_Shuttle_Down":
                                    part = FuncInline.enumErrorPart.OutShuttle_Down;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.OutShuttle_Down);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.OutShuttle_Down];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutShuttle_Down].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) ||
                                         DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor);
                                    actionCheck = FuncInline.OutShuttleDownAction != FuncInline.enumShuttleAction.Waiting;
                                    break;

                                case "btnOut_Conveyor":
                                    part = FuncInline.enumErrorPart.OutConveyor;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.OutConveyor);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.OutConveyor];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.OutConveyor].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor);
                                    actionCheck = FuncInline.OutConveyorAction != FuncInline.enumLiftAction.Waiting;
                                    break;
                                case "btnNG_Buffer":
                                    part = FuncInline.enumErrorPart.NgBuffer;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.NgBuffer);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.NgBuffer];
                                    pcbInfo = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.NgBuffer].PCBStatus != FuncInline.enumSMDStatus.UnKnown;
                                    pcbCheck = DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor);

                                    actionCheck = FuncInline.NGBufferAction != FuncInline.enumLiftAction.Waiting;
                                    break;
                                case "btnSite1":
                                case "btnSite2":
                                case "btnSite3":
                                case "btnSite4":
                                case "btnSite5":
                                case "btnSite6":
                                case "btnSite7":
                                case "btnSite8":
                                case "btnSite9":
                                case "btnSite10":
                                case "btnSite11":
                                case "btnSite12":
                                case "btnSite13":
                                case "btnSite14":
                                case "btnSite15":
                                case "btnSite16":
                                case "btnSite17":
                                case "btnSite18":
                                case "btnSite19":
                                case "btnSite20":
                                case "btnSite21":
                                case "btnSite22":
                                case "btnSite23":
                                case "btnSite24":
                                case "btnSite25":
                                case "btnSite26":

                                    FuncInline.enumTeachingPos pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + int.Parse(activeButton.Name.Replace("btnSite", "")) - 1);
                                    int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;

                                    part = FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex;
                                    partError = FuncError.CheckError(FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex);
                                    needClear = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex];
                                    pcbInfo = FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.UnKnown;


                                    pcbCheck = FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi) && DIO.GetDIData(dockDi);

                                    actionCheck = FuncInline.SiteAction[siteIndex] != FuncInline.enumSiteAction.Waiting;

                                    // (기존 오픈 체크 로직 그대로 유지)
                                    if (part >= FuncInline.enumErrorPart.Site1_F_DT1 && part <= FuncInline.enumErrorPart.Site26_R_FT3)
                                    {
                                        int siteNo = (int)part - (int)FuncInline.enumErrorPart.Site1_F_DT1;
                                        if (FuncInline.SiteClear[siteNo])
                                        {
                                            FuncWin.TopMessageBox("Open " + pos.ToString() + " and remove PCB first.");
                                            return;
                                        }
                                    }
                                    break;
                                default: // 해당 사항 없음
                                    continue;
                            }

                            Site_Click(activeButton, e);

                            if (partError ||
                                needClear ||
                                pcbInfo ||
                                pcbCheck ||
                                actionCheck)
                            {
                                FuncInline.WaitMessage = "Clearing " + part.ToString();
                                if (!btnPartClear_Click())
                                {
                                    FuncInline.WaitMessage = "";
                                    return;
                                }
                            }
                        }
                        else
                        {
                            FuncInline.NeedPartClear[i] = false; // system 경우 여기 통해 클리어된다.
                        }
                    }
                }

                FuncMotion.ServoAllInit();

                btnAlarmClear_Click(sender, e);

                FuncError.RemoveAllError();
                btnRefresh_Click(sender, e);

                GlobalVar.E_Stop = false;
                GlobalVar.DoorOpen = false;
                GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0 ? true : false;
                GlobalVar.SystemStatus = GlobalVar.SystemStatus > enumSystemStatus.Initialize ? enumSystemStatus.Manual : enumSystemStatus.BeforeInitialize;

                for (int i = 0; i < FuncInline.NeedPartClear.Length; i++)
                {
                    FuncInline.NeedPartClear[i] = false;
                }

                FuncInline.DuplicatedPos[0] = FuncInline.enumTeachingPos.None;
                FuncInline.DuplicatedPos[1] = FuncInline.enumTeachingPos.None;

                while (clearWatch.IsRunning &&
                    clearWatch.ElapsedMilliseconds < 2000)
                {
                    Util.USleep(GlobalVar.ThreadSleep);
                }

                FuncInline.WaitMessage = "";

                allClear = false;

                Util.ResetWatch(ref clearWatch);
                FuncInline.StopAllConveyor();
                //*/
                Util.StartWatch(ref FuncInline.NgAlarmWatch); //  NG 버퍼 클리어 타이머 리셋

                #region 목적지 잃은 파트 목적지 삭제
                #region 리프트 1 상부. 목적지 사이트고 사이트에 이미 PCB 있으면 목적지 삭제
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination <= FuncInline.enumTeachingPos.Site13_F_FT3 &&
                    FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination = FuncInline.enumTeachingPos.None;
                }
                #endregion
                #region 리프트 1 하부. 목적지 사이트고 사이트 PCB 없으면 목적지 삭제
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination <= FuncInline.enumTeachingPos.Site13_F_FT3 &&
                    FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                {
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination = FuncInline.enumTeachingPos.None;
                }
                #endregion
                #region 리프트 2 상부. 목적지 사이트고 사이트에 이미 PCB 있으면 목적지 삭제
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination >= FuncInline.enumTeachingPos.Site14_R_DT1 &&
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination <= FuncInline.enumTeachingPos.Site26_R_FT3 &&
                    FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                {
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination = FuncInline.enumTeachingPos.None;
                }
                #endregion
                #region 리프트 2 하부. 목적지 사이트고 사이트 PCB 없으면 목적지 삭제
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination >= FuncInline.enumTeachingPos.Site14_R_DT1 &&
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination <= FuncInline.enumTeachingPos.Site26_R_FT3 &&
                    FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                {
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination = FuncInline.enumTeachingPos.None;
                }
                #endregion
                #endregion

            }
            else
            {
                // ===== 여기부터 "Clear All Parts?" 분기 =====
                if (FuncWin.MessageBoxOK("Clear All Parts?"))
                {
                    #region PCB 존재 유무 확인 (명칭 최신화 + 사이트는 SiteIoMaps 사용)
                    // In Conveyor
                    if (DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in In Conveyor. Remove PCB and try again."); return; }
                    }
                    // In Shuttle
                    if (DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in In Shuttle. Remove PCB and try again."); return; }
                    }

                    // Front PassLine
                    if (DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Front PassLine Conveyor. Remove PCB and try again."); return; }
                    }
                    // Front ScanSite
                    if (DIO.GetDIData(FuncInline.enumDINames.X03_4_Front_Scan_PCB_Dock_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X03_4_Front_Scan_PCB_Dock_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Front ScanSite Conveyor. Remove PCB and try again."); return; }
                    }
                    // Lift1 Up
                    if (DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Front Lift UP Conveyor. Remove PCB and try again."); return; }
                    }

                    // Lift1 Down
                    if (DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Front Lift DOWN Conveyor. Remove PCB and try again."); return; }
                    }


                    // Rear PassLine
                    if (DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Rear PassLine Conveyor. Remove PCB and try again."); return; }
                    }
                    // Rear NGLine
                    if (DIO.GetDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Rear NGLine Conveyor. Remove PCB and try again."); return; }
                    }

                    // Lift2 Up
                    if (DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Rear Lift UP Conveyor. Remove PCB and try again."); return; }
                    }

                    // Lift2 Down
                    if (DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Rear Lift DOWN Conveyor. Remove PCB and try again."); return; }
                    }
                    // Out Shuttle up
                    if (DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) ||
                                            DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor) ||
                                            DIO.GetDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Out Shuttle Up Conveyor. Remove PCB and try again."); return; }
                    }

                    // Out Shuttle Down
                    if (DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Out Shuttle Down Conveyor. Remove PCB and try again."); return; }
                    }

                    // Out Conveyor
                    if (DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor, false);
                        }
                        else { FuncWin.TopMessageBox("PCB detected in Output Conveyor. Remove PCB and try again."); return; }
                    }

                    // NG Buffer
                    if (DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor) ||
                        DIO.GetDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor))
                    {
                        if (GlobalVar.Simulation)
                        {
                            DIO.WriteDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor, false);
                            DIO.WriteDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor, false);

                        }
                        else { FuncWin.TopMessageBox("PCB detected in NG Buffer Conveyor. Remove PCB and try again."); return; }
                    }

                    // 모든 사이트 도크 감지(***SiteIoMaps 사용***)
                    for (int i = 0; i < FuncInline.MaxTestPCCount * FuncInline.MaxSiteCount; i++)
                    {
                        var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);
                        if (FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi) && DIO.GetDIData(dockDi))
                        {
                            //세대별 사이트 명칭 변경
                            string label = FuncInline.SiteDisplay.GetSiteDisplayName(pos);
                            if (GlobalVar.Simulation)
                            {
                                DIO.WriteDIData(dockDi, false);
                            }

                            else
                            {
                                FuncWin.TopMessageBox($"PCB detected in Site #{label}. Remove PCB and try again.");
                                return;
                            }
                        }
                    }
                    #endregion

                    FuncInline.WaitMessage = "Clearing All Parts.";

                    // PCB 정보 전체 삭제
                    for (int i = 0; i < FuncInline.PCBInfo.Length; i++)
                        FuncInline.ClearPCBInfo((FuncInline.enumTeachingPos)i);

                    // 액션 전체 삭제
                    FuncInline.InConveyorAction = FuncInline.enumLiftAction.Waiting;
                    FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                    FuncInline.FrontPassLineAction = FuncInline.enumLiftAction.Waiting;
                    FuncInline.FrontScanSiteAction = FuncInline.enumLiftAction.Waiting;
                    FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
                    FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
                    FuncInline.RearPassLineAction = FuncInline.enumLiftAction.Waiting;
                    FuncInline.RearNGLineAction = FuncInline.enumLiftAction.Waiting;
                    FuncInline.OutShuttleUpAction = FuncInline.enumShuttleAction.Waiting;
                    FuncInline.OutShuttleDownAction = FuncInline.enumShuttleAction.Waiting;
                    FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
                    FuncInline.NGBufferAction = FuncInline.enumLiftAction.Waiting;
                    FuncInline.ScanAction = FuncInline.enumLiftAction.Waiting;


                    for (int i = 0; i < FuncInline.SiteAction.Length; i++)
                    {
                        FuncInline.SiteAction[i] = FuncInline.enumSiteAction.Waiting;
                    }
                    FuncInline.NGOut = false;

                    // 모든 컨베어 “구동 후 정지”(원코드 2초 유지)
                    if (!GlobalVar.Simulation)
                    {
                        DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);    //인컨베이어,인셔틀 동작 동시
                        DIO.WriteDOData(FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw, true);
                        DIO.WriteDOData(FuncInline.enumDONames.Y406_3_Front_SCAN_Motor_Ccw, true);
                        DIO.WriteDOData(FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, true);
                        //DIO.WriteDOData(FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, true);
                        DIO.WriteDOData(FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, true);
                        //DIO.WriteDOData(FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, true);
                        DIO.WriteDOData(FuncInline.enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, true);
                        DIO.WriteDOData(FuncInline.enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, true);
                        DIO.WriteDOData(FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw, true);
                        DIO.WriteDOData(FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw, true);

                        // 사이트 컨베이어: SiteIoMaps로 CCW만 구동(원 코드 동작과 동일)
                        for (int i = 0; i < FuncInline.MaxSiteCount; i++)
                        {
                            var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);
                            if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(pos, out var cw, out var ccw))
                                DIO.WriteDOData(ccw, true);
                        }
                    }

                    Stopwatch watch = new Stopwatch();
                    Util.StartWatch(ref watch);
                    while (watch.ElapsedMilliseconds < 2000) Util.USleep(GlobalVar.ThreadSleep);

                    DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, false);    //인컨베이어,인셔틀 동작 동시
                    DIO.WriteDOData(FuncInline.enumDONames.Y404_1_Front_Passline_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y406_3_Front_SCAN_Motor_Ccw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, false);
                    //DIO.WriteDOData(FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, false);
                    //DIO.WriteDOData(FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y400_5_Out_Shuttle_Ng_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y400_1_Out_Conveyor_Motor_Cw, false);
                    DIO.WriteDOData(FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw, false);

                    for (int i = 0; i < FuncInline.MaxTestPCCount * FuncInline.MaxSiteCount; i++)
                    {
                        var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);
                        if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(pos, out var cw, out var ccw))
                            DIO.WriteDOData(ccw, false);
                    }

                    // 전체 플래그 초기화
                    for (int i = 0; i < FuncInline.NeedPartClear.Length; i++) FuncInline.NeedPartClear[i] = false;
                    FuncMotion.ServoAllInit();
                    FuncError.RemoveAllError();

                    GlobalVar.E_Stop = false;
                    GlobalVar.DoorOpen = false;
                    GlobalVar.SystemErrored = GlobalVar.SystemErrorListQueue.Count > 0;
                    GlobalVar.SystemStatus = enumSystemStatus.Manual;

                    FuncInline.WaitMessage = "";

                    #region 목적지 잃은 파트 목적지 삭제(기존 유지, 위와 동일)
                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination <= FuncInline.enumTeachingPos.Site13_F_FT3 &&
                        FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination = FuncInline.enumTeachingPos.None;
                    }
                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination <= FuncInline.enumTeachingPos.Site13_F_FT3 &&
                        FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Down].Destination = FuncInline.enumTeachingPos.None;
                    }
                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination >= FuncInline.enumTeachingPos.Site14_R_DT1 &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination <= FuncInline.enumTeachingPos.Site26_R_FT3 &&
                        FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Up].Destination].PCBStatus != FuncInline.enumSMDStatus.UnKnown)
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift1_Up].Destination = FuncInline.enumTeachingPos.None;
                    }
                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination >= FuncInline.enumTeachingPos.Site14_R_DT1 &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination <= FuncInline.enumTeachingPos.Site26_R_FT3 &&
                        FuncInline.PCBInfo[(int)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination].PCBStatus == FuncInline.enumSMDStatus.UnKnown)
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Lift2_Down].Destination = FuncInline.enumTeachingPos.None;
                    }
                    #endregion
                }
            }

            FuncInline.SystemLogSave = true;

        }

        private void btnBuzzerStop_Click(object sender, EventArgs e)
        {
            GlobalVar.EnableBuzzer = false;

        }

        private void btnSiteTest_Click(object sender, EventArgs e)
        {
            FuncInline.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                        DateTime.Now.ToString("HH:mm:ss"),
                                        FuncInline.enumErrorPart.Site7_F_DT7,
                                        FuncInline.enumErrorCode.Site_Power_Off,
                                        false,
                                        "Module powered off while PCB is in Module. Check PCB status or signal cable and power switch."));
        }

        private void btnUnclampPCB_Click(object sender, EventArgs e)
        {
            int siteNo = -1;
            int.TryParse(activeButton.Name.Replace("btnSite", ""), out siteNo);

            FuncInline.enumTeachingPos pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + (siteNo - 1));
            if (siteNo > 0)
            {
                    // 클램프 해제: Stopper DO(클램프 전진 DO) OFF
                    if (FuncInline.SiteIoMaps.TryGetContactStopperDO(pos, out var stopperDo))
                        DIO.WriteDOData(stopperDo, false);

                    // 핀 UP: 단동 솔레노이드(Down일 때 TRUE) → FALSE로 써서 상승
                    if (FuncInline.SiteIoMaps.TryGetContactUpDownDO(pos, out var contactDownDo))
                        DIO.WriteDOData(contactDownDo, false);
            }
        }

    }
}
