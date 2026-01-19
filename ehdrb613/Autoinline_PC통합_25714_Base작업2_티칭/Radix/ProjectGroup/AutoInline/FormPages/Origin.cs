using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Radix
{
    /*
     * Origin.cs : 초기화(오리진) 폼 로직 (TryInit 제거 버전)
     * - enumInitialize 를 "직접" 사용 (FrontPassline, RearPassline 등 고정 이름 전제)
     * - Passline2 → RearPassline, Passline1 → FrontPassline 로 매핑
     * - InConveyor/OutConveyor/RearNGline/Scan 처리 추가
     * - 사이트 PCB 체크는 SiteIoMaps.TryGetPcbDockDI 사용
     * - 버튼 Name은 기존 것을 그대로 사용하고, 의미만 매핑(TODO 주석 참고)
     */

    public partial class Origin : Form
    {
        #region 로컬변수
        private System.Threading.Timer timerCheck;   // UI 주기 갱신 타이머
        private bool timerCheckDoing = false;

        private bool timerOriginDoing = false;       // 오리진 실행 중 플래그

        // enumInitialize 항목 수만큼 선택/완료 상태 배열
        private readonly bool[] posSelect = new bool[Enum.GetValues(typeof(FuncInline.enumInitialize)).Length];
        private readonly bool[] originDone = new bool[Enum.GetValues(typeof(FuncInline.enumInitialize)).Length];

        // 사이트 초기화 시작 인덱스( Site1_F_DT1 )을 “고정”으로 사용
        //  - 프로젝트에 반드시 존재해야 함
        private readonly int siteInitBase = (int)FuncInline.enumInitialize.Site1_F_DT1;
        #endregion

        #region 생성/공통
        public Origin()
        {
            InitializeComponent();
        }

        private void debug(string s) { Util.Debug(s); }
        #endregion

        #region 폼 라이프사이클
        private void Origin_Shown(object sender, EventArgs e)
        {
            GlobalVar.dlgOpened = true;
            // UI 갱신 타이머 시작 (100ms)
            timerCheck = new System.Threading.Timer(new TimerCallback(TimerCheck), null, 0, 100);
            this.BringToFront();
        }

        private void Origin_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { GlobalVar.dlgOpened = false; } catch { }
        }
        #endregion

        #region UI 갱신 타이머
        private void TimerCheck(object state)
        {
            if (FuncInline.TabMain != FuncInline.enumTabMain.Origin)
            {
                timerCheckDoing = false;
                return;
            }
            if (timerCheckDoing) return;
            timerCheckDoing = true;

            try
            {
                if (!GlobalVar.GlobalStop && this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        // ============================
                        // 1) 오리진 완료 상태 갱신
                        // ============================

                     
                        originDone[(int)FuncInline.enumInitialize.InConveyor] = FuncInlineAction.CheckOriginDone(FuncInline.enumInitialize.InConveyor);
                        // 셔틀
                        originDone[(int)FuncInline.enumInitialize.InShuttle] = FuncInlineAction.CheckOriginDone(FuncInline.enumInitialize.InShuttle);
                        originDone[(int)FuncInline.enumInitialize.OutShuttle] = FuncInlineAction.CheckOriginDone(FuncInline.enumInitialize.OutShuttle);
                        originDone[(int)FuncInline.enumInitialize.OutConveyor] = FuncInlineAction.CheckOriginDone(FuncInline.enumInitialize.OutConveyor);

                        // 리프트
                        originDone[(int)FuncInline.enumInitialize.FrontLift] = FuncInlineAction.CheckOriginDone(FuncInline.enumInitialize.FrontLift);
                        originDone[(int)FuncInline.enumInitialize.Lift2] = FuncInlineAction.CheckOriginDone(FuncInline.enumInitialize.Lift2);
                            
                        originDone[(int)FuncInline.enumInitialize.Scan] = FuncInlineAction.CheckOriginDone(FuncInline.enumInitialize.Scan);

                        // 사이트(0 ~ MaxSiteCount-1)
                        for (int i = 0; i < FuncInline.MaxSiteCount; i++)
                        {
                            int idx = siteInitBase + i;
                            originDone[idx] = FuncInlineAction.CheckOriginDone((FuncInline.enumInitialize)idx);
                        }

                        // ============================
                        // 2) 버튼 색상 표시
                        //    완료: Lime / 선택: Yellow / 대기: White
                        // ============================

                      
                        PaintButton(btnInConveyor, (int)FuncInline.enumInitialize.InConveyor);
                        PaintButton(btnInShuttle, (int)FuncInline.enumInitialize.InShuttle);
                        PaintButton(btnOutShuttle, (int)FuncInline.enumInitialize.OutShuttle);
                        PaintButton(btnOutConveyor, (int)FuncInline.enumInitialize.OutConveyor);
                        PaintButton(btnNGBuffer, (int)FuncInline.enumInitialize.NgBuffer);

                        PaintButton(btnLift1, (int)FuncInline.enumInitialize.FrontLift);
                        PaintButton(btnLift2, (int)FuncInline.enumInitialize.Lift2);
                        PaintButton(btnScan, (int)FuncInline.enumInitialize.Scan);

                        // 사이트 버튼: 현재 폼에 있는 수량만 칠함(예: 1~20)
                        PaintSite(btnSite1, 0);
                        PaintSite(btnSite2, 1);
                        PaintSite(btnSite3, 2);
                        PaintSite(btnSite4, 3);
                        PaintSite(btnSite5, 4);
                        PaintSite(btnSite6, 5);
                        PaintSite(btnSite7, 6);
                        PaintSite(btnSite8, 7);
                        PaintSite(btnSite9, 8);
                        PaintSite(btnSite10, 9);
                        PaintSite(btnSite11, 10);
                        PaintSite(btnSite12, 11);
                        PaintSite(btnSite13, 12);
                        PaintSite(btnSite14, 13);
                        PaintSite(btnSite15, 14);
                        PaintSite(btnSite16, 15);
                        PaintSite(btnSite17, 16);
                        PaintSite(btnSite18, 17);
                        PaintSite(btnSite19, 18);
                        PaintSite(btnSite20, 19);
                        PaintSite(btnSite21, 20);
                        PaintSite(btnSite22, 21);
                        PaintSite(btnSite23, 22);
                        PaintSite(btnSite24, 23);
                        PaintSite(btnSite25, 24);
                        PaintSite(btnSite26, 25);
                  

                        // 실행 중 메시지 표시
                        if (lblOriginMessage != null)
                            lblOriginMessage.Visible = timerOriginDoing;
                    }));
                }
            }
            catch { }
            finally
            {
                timerCheckDoing = false;
                if (GlobalVar.GlobalStop)
                {
                    try { timerCheck?.Dispose(); } catch { }
                }
            }
        }

        private void PaintButton(Button btn, int initIndex)
        {
            if (btn == null) return; // 폼에 버튼이 없으면 스킵
            btn.BackColor = originDone[initIndex] ? Color.Lime :
                            (posSelect[initIndex] ? Color.Yellow : Color.White);
        }

        private void PaintSite(Button btn, int siteOffset)
        {
            if (btn == null) return;
            int idx = siteInitBase + siteOffset;
            btn.BackColor = originDone[idx] ? Color.Lime :
                            (posSelect[idx] ? Color.Yellow : Color.White);
        }
        #endregion

        #region 오리진 실행(작업 스레드)
        private void TimerOrigin()
        {
            if (timerOriginDoing) return;
            timerOriginDoing = true;

            try
            {
                var watch = new Stopwatch();
                Util.StartWatch(ref watch);

                while (!GlobalVar.GlobalStop && watch.ElapsedMilliseconds < 60 *1000)
                {
                    if (FuncInline.TabMain != FuncInline.enumTabMain.Origin)
                    {
                        timerOriginDoing = false;
                        return;
                    }

                    bool allDone = true; // 하나라도 실행 대상이 있으면 false

                    // ===== 1) 개별 포지션 실행 =====
                    RunOriginIfSelected((int)FuncInline.enumInitialize.InShuttle, FuncInline.enumInitialize.InShuttle, ref allDone);
                    RunOriginIfSelected((int)FuncInline.enumInitialize.OutShuttle, FuncInline.enumInitialize.OutShuttle, ref allDone);
                    RunOriginIfSelected((int)FuncInline.enumInitialize.InConveyor, FuncInline.enumInitialize.InConveyor, ref allDone);
                    RunOriginIfSelected((int)FuncInline.enumInitialize.OutConveyor, FuncInline.enumInitialize.OutConveyor, ref allDone);

                    RunOriginIfSelected((int)FuncInline.enumInitialize.FrontLift, FuncInline.enumInitialize.FrontLift, ref allDone);
                    RunOriginIfSelected((int)FuncInline.enumInitialize.Lift2, FuncInline.enumInitialize.Lift2, ref allDone);
                    RunOriginIfSelected((int)FuncInline.enumInitialize.NgBuffer, FuncInline.enumInitialize.NgBuffer, ref allDone);
    
                    RunOriginIfSelected((int)FuncInline.enumInitialize.Scan, FuncInline.enumInitialize.Scan, ref allDone);

              

                    // ===== 2) 사이트 실행 =====
                    for (int i = 0; i < FuncInline.MaxSiteCount; i++)
                    {
                        int idx = siteInitBase + i;
                        if (posSelect[idx] && !originDone[idx])
                        {
                            allDone = false;
                            FuncInlineAction.RunOrigin((FuncInline.enumInitialize)idx); // i = siteIndex
                        }
                    }

                    // ===== 3) 모두 완료 → 종료 =====
                    if (allDone)
                    {
                        if (GlobalVar.SystemStatus < enumSystemStatus.Manual)
                            GlobalVar.SystemStatus = enumSystemStatus.Manual;

                        MessageBox.Show("Origin Finished");
                        timerOriginDoing = false;
                        return;
                    }

                    Thread.Sleep(GlobalVar.ThreadSleep);
                }

                // 타임아웃
                FuncWin.TopMessageBox("Origin Failed");
            }
            catch
            {
                FuncWin.TopMessageBox("Origin Failed");
            }
            finally
            {
                timerOriginDoing = false;
            }
        }

        private void RunOriginIfSelected(int idx, FuncInline.enumInitialize init, ref bool allDone)
        {
            if (posSelect[idx] && !originDone[idx])
            {
                allDone = false;
                FuncInlineAction.RunOrigin(init);
            }
        }
        #endregion

        #region UI 이벤트
        private void Site_Click(object sender, EventArgs e)
        {
            if (timerOriginDoing)
            {
                FuncWin.TopMessageBox("Wait for Origin finish");
                return;
            }

            try
            {
                var btn = sender as Button;
                if (btn == null) return;

                string name = btn.Name;

                // 1) 개별 포지션 버튼 매핑 (버튼 Name → enumInitialize 인덱스)
                //    - 디자이너 Name이 다르면 여기서 바꿔주면 됨
                if (name == "btnLift1") { Toggle((int)FuncInline.enumInitialize.FrontLift); return; }
                else if (name == "btnLift2") { Toggle((int)FuncInline.enumInitialize.Lift2); return; }
                else if (name == "btnInShuttle") { Toggle((int)FuncInline.enumInitialize.InShuttle); return; }
                else if (name == "btnOutShuttle") { Toggle((int)FuncInline.enumInitialize.OutShuttle); return; }
                else if (name == "btnOutConveyor") { Toggle((int)FuncInline.enumInitialize.OutConveyor); return; } 
                else if (name == "btnInConveyor") { Toggle((int)FuncInline.enumInitialize.InConveyor); return; }
                else if (name == "btnNGBuffer") { Toggle((int)FuncInline.enumInitialize.NgBuffer); return; }
                else if (name == "btnScan") { Toggle((int)FuncInline.enumInitialize.Scan); return; }

                // 2) 사이트 버튼 (예: btnSite1 ~ btnSite20)
                if (name.StartsWith("btnSite", StringComparison.OrdinalIgnoreCase))
                {
                    int n;
                    if (int.TryParse(name.Substring("btnSite".Length), out n))
                    {
                        if (n >= 1 && n <= FuncInline.MaxSiteCount)
                        {
                            int idx = siteInitBase + (n - 1);
                            Toggle(idx);
                            return;
                        }
                    }
                }
            }
            catch { }
        }

        private void Toggle(int idx)
        {
            posSelect[idx] = !posSelect[idx];
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (timerOriginDoing) { FuncWin.TopMessageBox("Wait for Origin finish"); return; }
            for (int i = 0; i < posSelect.Length; i++) posSelect[i] = true;
        }

        private void btnDeSelectAll_Click(object sender, EventArgs e)
        {
            if (timerOriginDoing) { FuncWin.TopMessageBox("Wait for Origin finish"); return; }
            for (int i = 0; i < posSelect.Length; i++) posSelect[i] = false;
        }

        private void btnOrigin_Click(object sender, EventArgs e)
        {
            if (timerOriginDoing)
            {
                FuncWin.TopMessageBox("Wait for Origin finish");
                return;
            }

            // ===== 대상 존재 여부 =====
            bool targetExist = false;
            for (int i = 0; i < posSelect.Length; i++)
            {
                if (posSelect[i] && !originDone[i]) { targetExist = true; break; }
            }
            if (!targetExist)
            {
                FuncWin.TopMessageBox("No part to run origin");
                return;
            }

            // ===== PCB 잔존 여부 체크 =====
            // - 사이트: SiteIoMaps.TryGetPcbDockDI 사용
            // - 나머지 포지션: 기존 DI 사용(설비 매핑에 맞게 필요 시 변경)
            // ----------------------------------------------------

            // InShuttle
            if (posSelect[(int)FuncInline.enumInitialize.InShuttle] && !originDone[(int)FuncInline.enumInitialize.InShuttle])
            {
                if (DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ||
                                               DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
                                               DIO.GetDIData(FuncInline.enumDINames.X304_0_In_Shuttle_Interlock_Sensor))
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in In Shuttle");
                    return;
                }
            }

            // OutShuttle
            if (posSelect[(int)FuncInline.enumInitialize.OutShuttle] && !originDone[(int)FuncInline.enumInitialize.OutShuttle])
            {
                if (DIO.GetDIData(FuncInline.enumDINames.X302_3_Out_Shuttle_OK_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X302_4_Out_Shuttle_OK_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in Out Shuttle");
                    return;
                }
            }



            // InConveyor
            if (posSelect[(int)FuncInline.enumInitialize.InConveyor] && !originDone[(int)FuncInline.enumInitialize.InConveyor])
            {
                if (DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor))
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in In Conveyor");
                    return;
                }
            }

            // OutConveyor
            if (posSelect[(int)FuncInline.enumInitialize.OutConveyor] && !originDone[(int)FuncInline.enumInitialize.OutConveyor])
            {
                if (DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in Out Conveyor");
                    return;
                }
            }

            // NGBuffer 
            if (posSelect[(int)FuncInline.enumInitialize.NgBuffer] && !originDone[(int)FuncInline.enumInitialize.NgBuffer])
            {
                if (DIO.GetDIData(FuncInline.enumDINames.X04_3_Out_Conveyor_NG_PCB_Stop_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X402_2_Out_Conveyor_Ng_PCB_In_Sensor))
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in NG Buffer(Out Conveyor Down)");
                    return;
                }
            }

            // Scan (스캔 포지션 DI가 있으면 여기에 매핑)
            if (posSelect[(int)FuncInline.enumInitialize.Scan] && !originDone[(int)FuncInline.enumInitialize.Scan])
            {
                // TODO: 스캔 포지션의 PCB 감지 DI가 있다면 아래에 추가
                // if (DIO.GetDIData(FuncInline.enumDINames.X??_Scan_PCB_Sensor)) { ... return; }
            }

            // FrontLift (상/하 모두 체크)
            if (posSelect[(int)FuncInline.enumInitialize.FrontLift] && !originDone[(int)FuncInline.enumInitialize.FrontLift])
            {
                if (DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in Front Lift Up");
                    return;
                }
                if (DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in Front PassLine");
                    return;
                }
            }

            // Lift2 (상/하 모두 체크)
            if (posSelect[(int)FuncInline.enumInitialize.Lift2] && !originDone[(int)FuncInline.enumInitialize.Lift2])
            {
                if (DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                    DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in Rear Lift Up");
                    return;
                }
                if (DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor) )
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in Rear PassLine");
                    return;
                }
                if (DIO.GetDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor))
                {
                    FuncWin.TopMessageBox("Can't run origin while PCB checked in Rear NGLine");
                    return;
                }
               
                   
            }

            // 사이트: SiteIoMaps.TryGetPcbDockDI로 공통 체크
            for (int i = 0; i < FuncInline.MaxSiteCount; i++)
            {
                int idx = siteInitBase + i;
                if (!posSelect[idx] || originDone[idx]) continue;

                FuncInline.enumTeachingPos pos = FuncInline.enumTeachingPos.Site1_F_DT1 + i;
                if (FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi))
                {
                    if (DIO.GetDIData(dockDi))
                    {
                        FuncWin.TopMessageBox("Can't run origin while PCB checked in Site #" + (i + 1));
                        return;
                    }
                }
            }

            // ===== 오리진 실행 시작 =====
            new Thread(TimerOrigin).Start();
        }
        #endregion
    }
}
