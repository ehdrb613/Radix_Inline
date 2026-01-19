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
using System.Collections.Concurrent; // ConcurrentQueue
using System.Diagnostics;

namespace Radix.Popup.Manual
{
    /*
     * Manual.cs : 각 파트 및 장비의 수동 운전
     */

    public partial class Site : Form
    {
        #region 로컬 Type 정의
        enum enumManualAction
        {
            None,
            AllSiteOpen,
            AllSiteClose,
            SiteOpen,
            SiteClose,
            SiteReOpen,
            SiteReClose,
            RobotMove,
            ConvMove,
            LoadPCB
        }

        enum enumConvAction
        {
            InputLift_Input,
            InputLift_Scan,
            InputLift_ToBufferConveyor,
            Buffer_ToCoolingConveyor,
            Input_ToLoadingConveyor,
            Loading_ToUnloadingConveyor,
            Unloading_ToOutputLift,
            OutputLift_Out,
        }
        #endregion

        #region 로컬변수
        private System.Threading.Timer timerCheck = null;
        private bool timerCheckDoing = false;

        //private bool timerDoing = false;
        //private bool timerMotorDoing = false;
        //private bool timerJogDoing = false;
        //private bool timerManualDoing = false;
        private bool manualStop = false;

        /// 현재 폼의 siteIndex(0-base)에서 TeachingPos 열거형을 만들어준다.
        /// SiteIoMaps는 “Site1 + offset” 기반으로 맵핑하므로 이렇게 변환해서 쓴다.
        /// </summary>
        private FuncInline.enumTeachingPos GetSitePos(int idx)
            => FuncInline.enumTeachingPos.Site1_F_DT1 + idx;

        private FuncInline.enumTeachingPos manualPos = FuncInline.enumTeachingPos.Site1_F_DT1;
        private int siteIndex = 0;

        //private double siteJogSpeed = 2; // 사이트 폭조절 조그시 이동속도

        FuncInline.enumTabMain beforeTabMain = FuncInline.TabMain; // 조그 멈추기용
        FuncInline.enumTabMain beforeTabMain2 = FuncInline.TabMain; // 메뉴얼 동작 프로세스 정지용
        FuncInline.enumTabManual beforeTabManual = FuncInline.TabManual; // 조그 멈추기용
        FuncInline.enumTabManual beforeTabManual2 = FuncInline.TabManual; // 메뉴얼 동작 프로세스 정지용

        //private Stopwatch jogWatch = new Stopwatch(); // 조그 정지 위해
        int startIndex = 0;
        int endIndex = 1;
        #endregion

        #region 초기화 관련

        public Site()
        {
            InitializeComponent();
        }

        private void Manual_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;

            // 타이머 시작
            TimerCallback CallBackCheck = new TimerCallback(TimerCheck);
            timerCheck = new System.Threading.Timer(CallBackCheck, false, 0, 100);


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
                //timerMotor.Dispose(); // 사용 안함
                //timerJog.Dispose();
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
            if (FuncInline.TabMain != FuncInline.enumTabMain.Manual ||
                FuncInline.TabManual != FuncInline.enumTabManual.Site) // 메인 탭이 다른 곳에 있으면 실행 안 한다.
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

                        #region 버튼 UI 변경

                        // ── 1) PCB 도크 감지 표시 (pbPCBDetect)
                        if (FuncInline.SiteIoMaps.TryGetPcbDockDI(manualPos, out var dockDi))
                        {
                            FuncForm.SetStateColor(pbPCBDetect, dockDi, "gray");
                        }
                        else
                        {
                            // 맵핑이 없을 때 기본 표시 (필요 시 색만 유지)
                            FuncForm.SetStateColor(pbPCBDetect, null, "gray");
                        }

                        // ── 2) 핀 업 상태 표시 (pbPinUp)
                        //    기존: 우측 핀 다운 DI를 부정(!)하여 표시
                        //    변경: '업 센서' DI 를 그대로 사용 (업 = true → 표시)
                        if (FuncInline.SiteIoMaps.TryGetContactUpDI(manualPos, out var contactUpDi))
                        {
                            pbPinUp.Visible = DIO.GetDIData(contactUpDi);
                        }
                        else
                        {
                            pbPinUp.Visible = false;  // 맵핑 없으면 숨김
                        }

                        // ── 3) 핀 다운/업 버튼 색상
                        //    다운 DO가 OFF면(= 업) 초록으로 표시
                        if (FuncInline.SiteIoMaps.TryGetContactUpDownDO(manualPos, out var upDo))
                        {
                            bool downOn = DIO.GetDORead(upDo);
                            FuncForm.SetButtonColor2(btnPinDown, downOn);
                            FuncForm.SetButtonColor2(btnPinUp, !downOn);
                        }
                        else
                        {
                            FuncForm.SetButtonColor2(btnPinDown, false);
                            FuncForm.SetButtonColor2(btnPinUp, false);
                        }
                      

                        // 4) 클램프 전진
                        if (FuncInline.SiteIoMaps.TryGetContactStopperDO(manualPos, out var StopperDO))
                        {
                            FuncForm.SetButtonColor2(btnClampForward, DIO.GetDORead(StopperDO));

                            FuncForm.SetButtonColor2(btnClampReward, !DIO.GetDORead(StopperDO));
                        }
                        else
                        {
                            FuncForm.SetButtonColor2(btnClampForward, false);
                            FuncForm.SetButtonColor2(btnClampReward, false);
                        }

                        // 5) 컨베이어 CW/CCW/STOP 버튼 색상: DO 페어 매핑 사용
                        if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(manualPos, out var motorCwDo, out var motorCcwDo))
                        {
                            bool cw = DIO.GetDORead(motorCwDo);
                            bool ccw = DIO.GetDORead(motorCcwDo);

                            FuncForm.SetButtonColor2(btnConveyorCW, cw);
                            FuncForm.SetButtonColor2(btnConveyorCCW, ccw);
                            FuncForm.SetButtonColor2(btnConveyorStop, !cw && !ccw);
                        }
                        else
                        {
                            FuncForm.SetButtonColor2(btnConveyorCW, false);
                            FuncForm.SetButtonColor2(btnConveyorCCW, false);
                            FuncForm.SetButtonColor2(btnConveyorStop, true);
                        }
                    

                        FuncForm.SetButtonColor2(btnTestStart,
                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].PCBStatus >= FuncInline.enumSMDStatus.Command_Sent);
                        FuncForm.SetButtonColor2(btnTestStop,
                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].PCBStatus < FuncInline.enumSMDStatus.Command_Sent);

                        #endregion

                        // 공통 유틸: 사이트 인덱스(i) → Dock DI 상태 읽기
                        bool GetDockState(int i)
                        {
                            var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);
                            return FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var di) && DIO.GetDIData(di);
                        }

                        btnSite1.BackColor = siteIndex == 0 ? Color.Lime :
                                        !FuncInline.UseSite[0] ? Color.LightGray :
                                        GetDockState(0) ?
                                        Color.Yellow : Color.White;
                        btnSite2.BackColor = siteIndex == 1 ? Color.Lime :
                                        !FuncInline.UseSite[1] ? Color.LightGray :
                                        GetDockState(1) ?
                                        Color.Yellow : Color.White;
                        btnSite3.BackColor = siteIndex == 2 ? Color.Lime :
                                        !FuncInline.UseSite[2] ? Color.LightGray :
                                        GetDockState(2) ?
                                        Color.Yellow : Color.White;
                        btnSite4.BackColor = siteIndex == 3 ? Color.Lime :
                                        !FuncInline.UseSite[3] ? Color.LightGray :
                                        GetDockState(3) ?
                                        Color.Yellow : Color.White;
                        btnSite5.BackColor = siteIndex == 4 ? Color.Lime :
                                        !FuncInline.UseSite[4] ? Color.LightGray :
                                        GetDockState(4) ?
                                        Color.Yellow : Color.White;
                        btnSite6.BackColor = siteIndex == 5 ? Color.Lime :
                                        !FuncInline.UseSite[5] ? Color.LightGray :
                                        GetDockState(5) ?
                                        Color.Yellow : Color.White;
                        btnSite7.BackColor = siteIndex == 6 ? Color.Lime :
                                        !FuncInline.UseSite[6] ? Color.LightGray :
                                        GetDockState(6) ?
                                        Color.Yellow : Color.White;
                        btnSite8.BackColor = siteIndex == 7 ? Color.Lime :
                                        !FuncInline.UseSite[7] ? Color.LightGray :
                                        GetDockState(7) ?
                                        Color.Yellow : Color.White;
                        btnSite9.BackColor = siteIndex == 8 ? Color.Lime :
                                        !FuncInline.UseSite[8] ? Color.LightGray :
                                        GetDockState(8) ?
                                        Color.Yellow : Color.White;
                        btnSite10.BackColor = siteIndex == 9 ? Color.Lime :
                                        !FuncInline.UseSite[9] ? Color.LightGray :
                                        GetDockState(9) ?
                                        Color.Yellow : Color.White;
                        btnSite11.BackColor = siteIndex == 10 ? Color.Lime :
                                        !FuncInline.UseSite[10] ? Color.LightGray :
                                        GetDockState(10) ?
                                        Color.Yellow : Color.White;
                        btnSite12.BackColor = siteIndex == 11 ? Color.Lime :
                                        !FuncInline.UseSite[11] ? Color.LightGray :
                                        GetDockState(11) ?
                                        Color.Yellow : Color.White;
                        btnSite13.BackColor = siteIndex == 12 ? Color.Lime :
                                        !FuncInline.UseSite[12] ? Color.LightGray :
                                        GetDockState(12) ?
                                        Color.Yellow : Color.White;
                        btnSite14.BackColor = siteIndex == 13 ? Color.Lime :
                                        !FuncInline.UseSite[13] ? Color.LightGray :
                                        GetDockState(13) ?
                                        Color.Yellow : Color.White;
                        btnSite15.BackColor = siteIndex == 14 ? Color.Lime :
                                        !FuncInline.UseSite[14] ? Color.LightGray :
                                        GetDockState(14) ?
                                        Color.Yellow : Color.White;
                        btnSite16.BackColor = siteIndex == 15 ? Color.Lime :
                                        !FuncInline.UseSite[15] ? Color.LightGray :
                                        GetDockState(15) ?
                                        Color.Yellow : Color.White;
                        btnSite17.BackColor = siteIndex == 16 ? Color.Lime :
                                        !FuncInline.UseSite[16] ? Color.LightGray :
                                        GetDockState(16) ?
                                        Color.Yellow : Color.White;
                        btnSite18.BackColor = siteIndex == 17 ? Color.Lime :
                                        !FuncInline.UseSite[17] ? Color.LightGray :
                                        GetDockState(17) ?
                                        Color.Yellow : Color.White;
                        btnSite19.BackColor = siteIndex == 18 ? Color.Lime :
                                        !FuncInline.UseSite[18] ? Color.LightGray :
                                        GetDockState(18) ?
                                        Color.Yellow : Color.White;
                        btnSite20.BackColor = siteIndex == 19 ? Color.Lime :
                                        !FuncInline.UseSite[19] ? Color.LightGray :
                                        GetDockState(19) ?
                                        Color.Yellow : Color.White;
                        btnSite21.BackColor = siteIndex == 20 ? Color.Lime :
                                        !FuncInline.UseSite[20] ? Color.LightGray :
                                        GetDockState(20) ?
                                        Color.Yellow : Color.White;
                        btnSite22.BackColor = siteIndex == 21 ? Color.Lime :
                                        !FuncInline.UseSite[21] ? Color.LightGray :
                                        GetDockState(21) ?
                                        Color.Yellow : Color.White;
                        btnSite23.BackColor = siteIndex == 22 ? Color.Lime :
                                        !FuncInline.UseSite[22] ? Color.LightGray :
                                        GetDockState(22) ?
                                        Color.Yellow : Color.White;
                        btnSite24.BackColor = siteIndex == 23 ? Color.Lime :
                                        !FuncInline.UseSite[23] ? Color.LightGray :
                                        GetDockState(23) ?
                                        Color.Yellow : Color.White;
                        btnSite25.BackColor = siteIndex == 24 ? Color.Lime :
                                        !FuncInline.UseSite[24] ? Color.LightGray :
                                        GetDockState(24) ?
                                        Color.Yellow : Color.White;
                        btnSite26.BackColor = siteIndex == 25 ? Color.Lime :
                                        !FuncInline.UseSite[25] ? Color.LightGray :
                                        GetDockState(25) ?
                                        Color.Yellow : Color.White;


                      
                        // PCB 감지 표시 버튼들을 배열로 모아서 루프 처리
                        Button[] pcbBtns = new Button[]
                        {
                            btnSite1PCB,  btnSite2PCB,  btnSite3PCB,  btnSite4PCB,  btnSite5PCB,  btnSite6PCB,
                            btnSite7PCB,  btnSite8PCB,  btnSite9PCB,  btnSite10PCB, btnSite11PCB, btnSite12PCB,
                            btnSite13PCB, btnSite14PCB, btnSite15PCB, btnSite16PCB, btnSite17PCB, btnSite18PCB,
                            btnSite19PCB, btnSite20PCB, btnSite21PCB, btnSite22PCB, btnSite23PCB, btnSite24PCB,
                            btnSite25PCB, btnSite26PCB
                        };

                        // 세대별 사이트 수(MaxSiteCount)에 맞춰 안전하게 처리
                        int count = Math.Min(pcbBtns.Length, FuncInline.MaxSiteCount);
                        for (int i = 0; i < count; i++)
                        {
                            pcbBtns[i].BackColor = GetDockState(i) ? Color.Yellow : Color.White;
                        }

                        //btnSite1PCB.BackColor = GetDockState(0) ? Color.Yellow : Color.White;
                        //btnSite2PCB.BackColor = GetDockState(1) ? Color.Yellow : Color.White;
                        //btnSite3PCB.BackColor = GetDockState(2) ? Color.Yellow : Color.White;
                        //btnSite4PCB.BackColor = GetDockState(3) ? Color.Yellow : Color.White;
                        //btnSite5PCB.BackColor = GetDockState(4) ? Color.Yellow : Color.White;
                        //btnSite6PCB.BackColor = GetDockState(5) ? Color.Yellow : Color.White;
                        //btnSite7PCB.BackColor = GetDockState(6) ? Color.Yellow : Color.White;
                        //btnSite8PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 7 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite9PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 8 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite10PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 9 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite11PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 10 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite12PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 11 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite13PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 12 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite14PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 13 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite15PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 14 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite16PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 15 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite17PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 16 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite18PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 17 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite19PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 18 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite20PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 19 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite21PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 20 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite22PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 21 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite23PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 22 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite24PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 23 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite25PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 24 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                        //btnSite26PCB.BackColor = DIO.GetDIData(FuncInline.enumDINames.X114_5_Front_DT_1_PCB_Dock_Sensor + 25 * GlobalVar.DIModuleGap) ? Color.Yellow : Color.White;
                       

                    }));
                }


            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

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


        #region Site 관련 이벤트
        #endregion


        #region 기타 함수
        private void debug(string str)
        {
            Util.Debug("Manual : " + str);
        }

        #endregion




        private void btnSite_Click(object sender, EventArgs e)
        {
            try
            {
                string name = ((Button)sender).Name;
                siteIndex = int.Parse(name.Replace("btnSite", "")) - 1;
                if (btnAllSite.BackColor == Color.Lime)
                {
                    startIndex = siteIndex;
                    endIndex = siteIndex + 1;
                }
                else if (btnRack1.BackColor == Color.Lime)
                {
                    startIndex = 0;
                    endIndex = 13;
                }
                else if (btnRack2.BackColor == Color.Lime)
                {
                    startIndex = 13;
                    endIndex = 26;
                }
                else
                {
                    startIndex = siteIndex;
                    endIndex = siteIndex + 1;
                }
                manualPos = FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex;
                /*
                for (int i = 1; i <= FuncInline.MaxSiteCount * FuncInline.MaxTestPCCount; i++)
                {
                    ((Button)Controls.Find("btnSite" + i, true)[0]).BackColor = siteIndex == i - 1 ? Color.Lime : Color.White;
                }
                //*/
                btnSite1.BackColor = siteIndex == 0 ? Color.Lime : Color.White;
                btnSite2.BackColor = siteIndex == 1 ? Color.Lime : Color.White;
                btnSite3.BackColor = siteIndex == 2 ? Color.Lime : Color.White;
                btnSite4.BackColor = siteIndex == 3 ? Color.Lime : Color.White;
                btnSite5.BackColor = siteIndex == 4 ? Color.Lime : Color.White;
                btnSite6.BackColor = siteIndex == 5 ? Color.Lime : Color.White;
                btnSite7.BackColor = siteIndex == 6 ? Color.Lime : Color.White;
                btnSite8.BackColor = siteIndex == 7 ? Color.Lime : Color.White;
                btnSite9.BackColor = siteIndex == 8 ? Color.Lime : Color.White;
                btnSite10.BackColor = siteIndex == 9 ? Color.Lime : Color.White;
                btnSite11.BackColor = siteIndex == 10 ? Color.Lime : Color.White;
                btnSite12.BackColor = siteIndex == 11 ? Color.Lime : Color.White;
                btnSite13.BackColor = siteIndex == 12 ? Color.Lime : Color.White;
                btnSite14.BackColor = siteIndex == 13 ? Color.Lime : Color.White;
                btnSite15.BackColor = siteIndex == 14 ? Color.Lime : Color.White;
                btnSite16.BackColor = siteIndex == 15 ? Color.Lime : Color.White;
                btnSite17.BackColor = siteIndex == 16 ? Color.Lime : Color.White;
                btnSite18.BackColor = siteIndex == 17 ? Color.Lime : Color.White;
                btnSite19.BackColor = siteIndex == 18 ? Color.Lime : Color.White;
                btnSite20.BackColor = siteIndex == 19 ? Color.Lime : Color.White;
                btnSite21.BackColor = siteIndex == 20 ? Color.Lime : Color.White;
                btnSite22.BackColor = siteIndex == 21 ? Color.Lime : Color.White;
                btnSite23.BackColor = siteIndex == 22 ? Color.Lime : Color.White;
                btnSite24.BackColor = siteIndex == 23 ? Color.Lime : Color.White;
                btnSite25.BackColor = siteIndex == 24 ? Color.Lime : Color.White;
                btnSite26.BackColor = siteIndex == 25 ? Color.Lime : Color.White;
              

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }



        private void btnModuleTestStartSite_Click(object sender, EventArgs e)
        {

        }

        private void btnModuleTestStopSite_Click(object sender, EventArgs e)
        {

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            /*
            // PCB 투입 상태로 기동시 상태
            //FuncInline.PMCStatus[(int)FuncInline.enumPMCAxis.Site1].Position =
            DIO.WriteDOData(FuncInline.enumDONames.Y15_0_Module1_Front_Down, true);
            DIO.WriteDOData(FuncInline.enumDONames.Y15_1_Module1_Rear_Down, false);
            DIO.WriteDOData(FuncInline.enumDONames.Y15_2_Module1_Forward, true);
            DIO.WriteDOData(FuncInline.enumDONames.Y15_3_Module1_Reward, false);
            DIO.WriteDOData(FuncInline.enumDONames.Y15_4_Module1_PinClamp_Forward, true);
            DIO.WriteDOData(FuncInline.enumDONames.Y15_5_Module1_Conveyor_Run, true);
            DIO.WriteDOData(FuncInline.enumDONames.Y15_6_Module1_Fan_Run, true);
            DIO.WriteDOData(FuncInline.enumDONames.Y15_7_Module1_Clamp, true);

            DIO.WriteDIData(FuncInline.enumDINames.X31_4_Module1_Forward_Sensor, false);
            DIO.WriteDIData(FuncInline.enumDINames.X31_5_Module1_Reward_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X30_0_Module1_Clamp_Forward_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X30_1_Module1_Clamp_Reward_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X30_3_Module1_PCB_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X30_4_Module1_Front_Left_Up_Sensor, false);
            DIO.WriteDIData(FuncInline.enumDINames.X30_5_Module1_Front_Left_Down_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X30_6_Module1_Front_Right_Up_Sensor, false);
            DIO.WriteDIData(FuncInline.enumDINames.X30_7_Module1_Front_Right_Down_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X30_8_Module1_Rear_Left_Up_Sensor, false);
            DIO.WriteDIData(FuncInline.enumDINames.X31_1_Module1_Rear_Left_Down_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X31_2_Module1_Rear_Right_Up_Sensor, false);
            DIO.WriteDIData(FuncInline.enumDINames.X31_3_Module1_Rear_Right_Down_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X31_6_Module1_PinClamp_Front_Left_Forward_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X31_7_Module1_PinClamp_Front_Left_Reward_Sensor, false);
            DIO.WriteDIData(FuncInline.enumDINames.X32_0_Module1_PinClamp_Front_Right_Forward_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X32_1_Module1_PinClamp_Front_Right_Reward_Sensor, false);
            DIO.WriteDIData(FuncInline.enumDINames.X32_2_Module1_PinClamp_Rear_Left_Forward_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X32_3_Module1_PinClamp_Rear_Left_Reward_Sensor, false);
            DIO.WriteDIData(FuncInline.enumDINames.X32_4_Module1_PinClamp_Rear_Right_Forward_Sensor, true);
            DIO.WriteDIData(FuncInline.enumDINames.X32_5_Module1_PinClamp_Rear_Right_Reward_Sensor, false);
            //*/
        }
        private void btnPinUp_Click(object sender, EventArgs e)
        {
            try
            {
                // 다중 사이트 동작 여부 
                if (checkAllSite())
                {
                    int missing = 0;

                    // 주의: 기존 코드 스타일을 그대로 따라 i < endIndex (endIndex는 배타 범위)
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        // i번째 사이트의 TeachingPos 계산 (Site1_F_DT1 + i)
                        var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);

                        // 단동 솔 DO 매핑 조회: true=Down / false=Up
                        if (FuncInline.SiteIoMaps.TryGetContactUpDownDO(pos, out var contactDownDo))
                        {
                            // 핀 업 = false
                            DIO.WriteDOData(contactDownDo, false);
                        }
                        else
                        {
                            // 매핑 누락 사이트 카운트 (루프 중 매핑 실패 팝업 난사 방지)
                            missing++;
                        }
                    }

                    if (missing > 0)
                    {
                        FuncWin.TopMessageBox(
                            $"ContactUpDown DO 매핑 누락 사이트 {missing}개가 있습니다.\r\nSiteIoMaps 설정을 확인해 주세요.");
                    }
                }
               
                
            }
            catch (Exception ex)
            {
                Util.Debug(ex.ToString());
                Util.Debug(ex.StackTrace);
            }

        }


        private void btnPinDown_Click(object sender, EventArgs e)
        {
            try
            {
                // 다중 사이트 동작 여부 (기존 checkAllSite / startIndex, endIndex 그대로 활용)
                if (checkAllSite())
                {
                    int missing = 0;

                    // 주의: 기존 코드 스타일을 그대로 따라 i < endIndex (endIndex는 배타 범위)
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        // i번째 사이트의 TeachingPos 계산 (Site1_F_DT1 + i)
                        var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);

                        // 단동 솔 DO 매핑 조회: true=Down / false=Up
                        if (FuncInline.SiteIoMaps.TryGetContactUpDownDO(pos, out var contactDownDo))
                        {
                            // 핀 다운 = ON
                            DIO.WriteDOData(contactDownDo, true);
                        }
                        else
                        {
                            // 매핑 누락 사이트 카운트 (루프 중 매핑 실패 팝업 난사 방지)
                            missing++;
                        }
                    }

                    if (missing > 0)
                    {
                        FuncWin.TopMessageBox(
                            $"ContactUpDown DO 매핑 누락 사이트 {missing}개가 있습니다.\r\nSiteIoMaps 설정을 확인해 주세요.");
                    }
                }
               
            }
            catch (Exception ex)
            {
                Util.Debug(ex.ToString());
                Util.Debug(ex.StackTrace);
            }

            //if (checkAllSite())
            //{
            //    for (int i = startIndex; i < endIndex; i++)
            //    {

            //        DIO.WriteDOData(FuncInline.enumDONames.Y08_3_Site1_PinClamp_Down + i * GlobalVar.DOModuleGap, false);

            //    }
            //}
            //DIO.WriteDOData(FuncInline.enumDONames.Y08_3_Site1_PinClamp_Down + siteIndex * GlobalVar.DOModuleGap, false);
        }

        private void btnClampForward_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkAllSite())
                {

               

                int missing = 0;
                for (int i = startIndex; i < endIndex; i++)
                {
                    var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);

                    // 매핑: 사이트 클램프(스톱퍼) 단동 솔 DO (전진 = true)
                    if (FuncInline.SiteIoMaps.TryGetContactStopperDO(pos, out var clampForwardDo))
                    {
                        DIO.WriteDOData(clampForwardDo, true);
                    }
                    else
                    {
                        missing++;
                    }
                }

                if (missing > 0)
                    FuncWin.TopMessageBox($"ContactStopper DO 매핑 누락 사이트 {missing}개가 있습니다.\r\nSiteIoMaps 설정을 확인해 주세요.");

                }

            }
            catch (Exception ex)
            {
                Util.Debug(ex.ToString());
                Util.Debug(ex.StackTrace);
            }

            //if (checkAllSite())
            //{
            //    if (FuncInline.SiteIoMaps.TryGetContactStopperDO(manualPos, out var stopperDo))
            //    {
            //        bool cur = DIO.GetDORead(stopperDo);
            //        DIO.WriteDOData(stopperDo, !cur); // 토글 (필요시 true/false 고정으로 변경)
            //    }
            //    for (int i = startIndex; i < endIndex; i++)
            //    {

            //        DIO.WriteDOData(FuncInline.enumDONames.Y08_2_Site1_Clamp_Forward + i * GlobalVar.DOModuleGap, true);

            //    }
            //}
            //DIO.WriteDOData(FuncInline.enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap, true);
        }

        private void btnClampReward_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkAllSite())
                {

                    int missing = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);

                        // 매핑: 사이트 클램프(스톱퍼) 단동 솔 DO (전진 = true)
                        if (FuncInline.SiteIoMaps.TryGetContactStopperDO(pos, out var clampForwardDo))
                        {
                            DIO.WriteDOData(clampForwardDo, false);
                        }
                        else
                        {
                            missing++;
                        }
                    }

                    if (missing > 0)
                        FuncWin.TopMessageBox($"ContactStopper DO 매핑 누락 사이트 {missing}개가 있습니다.\r\nSiteIoMaps 설정을 확인해 주세요.");
                }
            }
            catch (Exception ex)
            {
                Util.Debug(ex.ToString());
                Util.Debug(ex.StackTrace);
            }
        }


        private void btnConveyorCCW_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkAllSite())
                {

                    int missing = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);

                        bool hasAny = false;

                        // CCW DO (필수)
                        if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(manualPos, out var motorCwDo, out var motorCcwDo))
                        {
                            DIO.WriteDOData(motorCcwDo, true);
                            DIO.WriteDOData(motorCwDo, false);
                            hasAny = true;
                        }

                        if (!hasAny) missing++;
                    }

                    if (missing > 0)
                        FuncWin.TopMessageBox($"컨베이어 DO(CCW/CW) 매핑 누락 사이트 {missing}개가 있습니다.\r\nSiteIoMaps 설정을 확인해 주세요.");
                }
            }
            catch (Exception ex)
            {
                Util.Debug(ex.ToString());
                Util.Debug(ex.StackTrace);
            }

            //if (checkAllSite())
            //{
            //    for (int i = startIndex; i < endIndex; i++)
            //    {
            //        if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(manualPos, out var motorCwDo, out var motorCcwDo))
            //        {
            //            DIO.WriteDOData(motorCwDo, false);
            //            DIO.WriteDOData(motorCcwDo, true);
            //        }
            //        DIO.DoubleSol(FuncInline.enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap, false);

            //    }
            //}
            //DIO.DoubleSol(FuncInline.enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        }

        private void btnConveyorCW_Click(object sender, EventArgs e)
        {

            try
            {
                if (checkAllSite())
                {

                    int missing = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);

                        bool hasAny = false;

                        // CCW DO (필수)
                        if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(manualPos, out var motorCwDo, out var motorCcwDo))
                        {
                            DIO.WriteDOData(motorCcwDo, false);
                            DIO.WriteDOData(motorCwDo, true);
                            hasAny = true;
                        }

                        if (!hasAny) missing++;
                    }

                    if (missing > 0)
                        FuncWin.TopMessageBox($"컨베이어 DO(CCW/CW) 매핑 누락 사이트 {missing}개가 있습니다.\r\nSiteIoMaps 설정을 확인해 주세요.");
                }
            }
            catch (Exception ex)
            {
                Util.Debug(ex.ToString());
                Util.Debug(ex.StackTrace);
            }

            //if (checkAllSite())
            //{
            //    for (int i = startIndex; i < endIndex; i++)
            //    {

            //        DIO.DoubleSol(FuncInline.enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap, true);

            //    }
            //}
            //DIO.DoubleSol(FuncInline.enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, true);
        }

        private void btnConveyorStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkAllSite())
                {
                    int missing = 0;
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        var pos = (FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + i);

                        bool hasAny = false;

                        // CCW DO (필수)
                        if (FuncInline.SiteIoMaps.TryGetSiteTransportMotorPair(manualPos, out var motorCwDo, out var motorCcwDo))
                        {
                            DIO.WriteDOData(motorCcwDo, false);
                            DIO.WriteDOData(motorCwDo, false);
                            hasAny = true;
                        }

                        if (!hasAny) missing++;
                    }

                    if (missing > 0)
                        FuncWin.TopMessageBox($"컨베이어 DO(CCW/CW) 매핑 누락 사이트 {missing}개가 있습니다.\r\nSiteIoMaps 설정을 확인해 주세요.");
                }
            }
            catch (Exception ex)
            {
                Util.Debug(ex.ToString());
                Util.Debug(ex.StackTrace);
            }

            //if (checkAllSite())
            //{
            //    for (int i = startIndex; i < endIndex; i++)
            //    {

            //        DIO.WriteDOData(FuncInline.enumDONames.Y08_0_Site1_CW_Conveyor_Run + i * GlobalVar.DOModuleGap, false);
            //        DIO.WriteDOData(FuncInline.enumDONames.Y08_1_Site1_CCW_Conveyor_Run + i * GlobalVar.DOModuleGap, false);

            //    }
            //}
            //DIO.WriteDOData(FuncInline.enumDONames.Y08_0_Site1_CW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
            //DIO.WriteDOData(FuncInline.enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, false);
        }

        private bool checkAllSite()
        {
            if (btnSelectedSite.BackColor == Color.Lime)
            {
                return true;
            }
            string target = "All Site";
            if (btnRack1.BackColor == Color.Lime)
            {
                target = "Rack #1";
                startIndex = 0;
                endIndex = 13;
            }
            else if (btnRack2.BackColor == Color.Lime)
            {
                target = "Rack #2";
                startIndex = 13;
                endIndex = 26;
            }
            else
            {
                startIndex = 0;
                endIndex = 26;
            }
            return FuncWin.MessageBoxOK(target + " move selected. Move Anyway?");
        }

        private void btnAllSite_Click(object sender, EventArgs e)
        {
            btnRack1.BackColor = Color.White;
            btnRack2.BackColor = Color.White;
            btnAllSite.BackColor = Color.Lime;
            btnSelectedSite.BackColor = Color.White;
            startIndex = 0;
            endIndex = 25;
        }

        private void btnSelectedSite_Click(object sender, EventArgs e)
        {
            btnRack1.BackColor = Color.White;
            btnRack2.BackColor = Color.White;
            btnAllSite.BackColor = Color.White;
            btnSelectedSite.BackColor = Color.Lime;
            startIndex = siteIndex;
            endIndex = siteIndex + 1;
        }

        private void btnRack2_Click(object sender, EventArgs e)
        {
            btnRack1.BackColor = Color.White;
            btnRack2.BackColor = Color.Lime;
            btnAllSite.BackColor = Color.White;
            btnSelectedSite.BackColor = Color.White;
            startIndex = 13;
            endIndex = 26;
        }

        private void btnRack1_Click(object sender, EventArgs e)
        {
            btnRack1.BackColor = Color.Lime;
            btnRack2.BackColor = Color.White;
            btnAllSite.BackColor = Color.White;
            btnSelectedSite.BackColor = Color.White;
            startIndex = 0;
            endIndex = 13;
        }

       
    }
}
