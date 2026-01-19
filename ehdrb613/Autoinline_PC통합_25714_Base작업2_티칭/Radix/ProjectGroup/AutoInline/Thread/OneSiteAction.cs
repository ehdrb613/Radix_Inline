using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Radix
{

    class OneSiteAction
    {
        /*
         * AutoRun.cs : 자동운전 조작
         */
        private int SiteIndex = 0;
        private int pcIndex = 0;

        private void debug(string str)
        {
            Util.Debug("SiteAction : " + str);
        }

        public OneSiteAction(int siteIndex)
        {
            SiteIndex = siteIndex;
            pcIndex = SiteIndex / 7;
        }


        public void Run()
        {
            // 전체 로직만 관장하고 구체적인 동작은 세부 쓰레드에서 수행

            enumSystemStatus beforeStatus = GlobalVar.SystemStatus;

            /* 공정 역순으로 로직 배치를 해야 선행 프로세스가 후행부분에 영향을 안 준다. 
             * 특히 시뮬레이션시 더함 
             * 컨베어는 순서 상관없이 가장 뒤쪽으로 가야 함 //*/

            Stopwatch watch = new Stopwatch();

            while (!GlobalVar.GlobalStop)
            {

                try
                {
                    Util.StartWatch(ref watch);

                    FuncInline.enumTeachingPos pos = FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex;
                    // ===== 자동운전 메인 =====
                    if ((int)GlobalVar.SystemStatus >= (int)enumSystemStatus.AutoRun &&
                        GlobalVar.SystemStatus != enumSystemStatus.ErrorStop) // 에러상태가 아닐 때만 동작
                    {
                        // 패스모드가 아니면 사이트 로직 수행
                        if (!FuncInline.PassMode)
                        {
                            // 현재 루프의 사이트 인덱스 (0~25 가정)
                            // 기존 코드의 SiteIndex를 그대로 사용한다고 가정
                        

                            // === 사이트별 자주 쓰는 IO 매핑을 한 번에 확보 ===
                            // (없을 수도 있는 IO는 TryGet 실패 시 false 리턴하므로, 사용 전 null 체크 느낌으로 bool 리턴값 한 번 더 확인)
                            bool hasDockDI = FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var diPcbDock);
                          
                            bool hasClampDO = FuncInline.SiteIoMaps.TryGetContactStopperDO(pos, out var doContactStopper);
                            bool hasUpDownDO = FuncInline.SiteIoMaps.TryGetContactUpDownDO(pos, out var doContactUpDown);
                            bool hasUpDI = FuncInline.SiteIoMaps.TryGetContactUpDI(pos, out var diContactUp);
                         

                            // === 1) 로딩 단계: Loading -> Testing 전이 ===
                            if (FuncInline.SiteAction[SiteIndex] == FuncInline.enumSiteAction.Loading)
                            {
                                // 로딩 시퀀스(핀업/언클램프/컨베이어/센서 체크)는 SitePCBLoad가 내부에서 처리
                                // (함수 위치: FuncInlineAction 으로 이동)
                                if (FuncInlineAction.SitePCBLoad(pos)) // 완료 시 true
                                {
                                    // 액션 상태를 Testing으로 전환하고, 배열상태/워치 초기화
                                    if (FuncInline.SiteAction[SiteIndex] != FuncInline.enumSiteAction.Testing)
                                    {
                                        FuncLog.WriteLog_Tester(
                                            $"Site{SiteIndex + 1} action change : {FuncInline.SiteAction[SiteIndex]} ==> Testing");
                                    }
                                    FuncInline.SiteAction[SiteIndex] = FuncInline.enumSiteAction.Testing;

                                    // 어레이 상태를 Testing 전 준비 상태로 리셋 (기존 로직 유지)
                                    for (int j = 0; j < FuncInline.PCBInfo[(int)pos].SMDStatus.Length; j++)
                                    {
                                        if (FuncInline.PCBInfo[(int)pos].Barcode[j].Length > 0 &&
                                            FuncInline.PCBInfo[(int)pos].SMDStatus[j] != FuncInline.enumSMDStatus.Test_Pass &&
                                            FuncInline.PCBInfo[(int)pos].SMDStatus[j] != FuncInline.enumSMDStatus.No_Test &&
                                            !FuncInline.PCBInfo[(int)pos].Xout[j] &&
                                            !FuncInline.PCBInfo[(int)pos].BadMark[j])
                                        {
                                            FuncInline.PCBInfo[(int)pos].SMDReady[j] = false;
                                            FuncInline.PCBInfo[(int)pos].SMDReadySent[j] = false;
                                            FuncInline.PCBInfo[(int)pos].SMDStatus[j] = FuncInline.enumSMDStatus.Before_Command;
                                        }
                                    }
                                    FuncInline.PCBInfo[(int)pos].PCBStatus = FuncInline.enumSMDStatus.Before_Command;
                                    FuncLog.WriteLog_Tester($"AutoRun : SiteAction Change Load Done - {pos} Loaded");
                                }
                            }

                            // === 2) 재테스트(열기) 단계: ReOpen -> ReClose ===
                            if (FuncInline.SiteAction[SiteIndex] == FuncInline.enumSiteAction.ReOpen)
                            {
                                if (FuncInline.SiteReOpen(pos)) // 핀 올리기(자체 재검사 준비) 완료 시
                                {
                                    if (FuncInline.SiteAction[SiteIndex] != FuncInline.enumSiteAction.ReClose)
                                    {
                                        FuncLog.WriteLog_Tester(
                                            $"Site{SiteIndex + 1} action change : {FuncInline.SiteAction[SiteIndex]} ==> ReClose");
                                    }
                                    FuncInline.SiteAction[SiteIndex] = FuncInline.enumSiteAction.ReClose;
                                    Util.ResetWatch(ref FuncInline.SiteCheckTime[(int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1]);
                                }
                            }

                            // === 3) 재테스트(닫기) 단계: ReClose -> Testing ===
                            if (FuncInline.SiteAction[SiteIndex] == FuncInline.enumSiteAction.ReClose)
                            {
                                // 재오픈엔 “열기/닫기”가 별도 없이 ‘로딩’과 동일 동작(핀다운/클램프)이라 SitePCBLoad 재사용
                                if (FuncInlineAction.SitePCBLoad(pos))
                                {
                                    if (FuncInline.SiteAction[SiteIndex] != FuncInline.enumSiteAction.Testing)
                                    {
                                        FuncLog.WriteLog_Tester(
                                            $"Site{SiteIndex + 1} action change : {FuncInline.SiteAction[SiteIndex]} ==> Testing");
                                    }
                                    FuncInline.SiteAction[SiteIndex] = FuncInline.enumSiteAction.Testing;
                                    Util.ResetWatch(ref FuncInline.SiteCheckTime[(int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1]);

                                    // 보드 테스트 타이머 기동
                                    Util.StartWatch(ref FuncInline.PCBInfo[(int)pos].StopWatch);
                                    for (int k = 0; k < FuncInline.PCBInfo[(int)pos].TestWatch.Length; k++)
                                    {
                                        Util.StartWatch(ref FuncInline.PCBInfo[(int)pos].TestWatch[k]);
                                    }
                                }
                            }

                            // === 4) 위치 복구(컨베이어를 한 번도 안 돌렸는데 상태가 꼬인 경우 정리) ===
                            if (!FuncInline.SiteConvRun[SiteIndex])
                            {
                                // 4-1) 클램프가 잡혀 있으면 해제(컨택트 스토퍼 OFF)
                                if (hasClampDO && DIO.GetDORead((int)doContactStopper))
                                {
                                    DIO.WriteDOData(doContactStopper, false);
                                }
                                // 4-2) 핀 다운이면 핀 업(컨택트 업/다운 OFF 쪽을 ‘업’으로 사용)
                                else if (hasUpDownDO && hasUpDI && !DIO.GetDIData(diContactUp))
                                {
                                    // 업/다운 실배선 규약: Up이면 DO=false 라는 전제(기존 코드와 일치) 
                                    DIO.WriteDOData(doContactUpDown, false);
                                }
                            }

                            // === 5) PCB 정보 없으면(=UnKnown) 사이트 타이머는 정지 ===
                            if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.UnKnown &&
                                FuncInline.PCBInfo[(int)pos].StopWatch != null &&
                                FuncInline.PCBInfo[(int)pos].StopWatch.IsRunning)
                            {
                                FuncInline.PCBInfo[(int)pos].StopWatch.Stop();
                            }

                            // === 6) 테스트 사이트 지정(Testing인데 아직 TestSite가 비어 있으면 등록) ===
                            if (FuncInline.PCBInfo[(int)pos].TestSite == null)
                                FuncInline.PCBInfo[(int)pos].TestSite = new int[10];

                            if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Testing &&
                                FuncInline.PCBInfo[(int)pos].TestSite[0] != (SiteIndex + 1))
                            {
                                FuncInline.SetTestSite(SiteIndex);
                            }

                            // === 7) 테스트 지령 송신 (Before_Command → 장비로 Start 전송) ===
                            //  - BuyerChange(블루/오렌지)나 재테스트는 여기서 제외(기존 로직 동일)
                            if (!FuncInline.PCBInfo[(int)pos].BuyerChange &&
                                !FuncInline.PCBInfo[(int)pos].SelfReTest &&
                                !FuncInline.PCBInfo[(int)pos].OtherReTest &&
                                FuncInline.SiteAction[SiteIndex] == FuncInline.enumSiteAction.Testing &&
                                FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Before_Command)
                            {
                                // 핀 다운이 아니면(=테스트 고정이 안 되어 있으면) 다시 로딩 단계로 되돌림
                                bool pinDown =
                                    (hasUpDownDO && hasUpDI && !DIO.GetDIData(diContactUp)&& !DIO.GetDOData(doContactUpDown));

                                if (!pinDown)
                                {
                                    Util.StartWatch(ref FuncInline.PCBInfo[(int)pos].StopWatch);
                                    Util.StartWatch(ref FuncInline.SiteCheckTime[SiteIndex]);
                                    FuncInline.SiteConvRun[SiteIndex] = false;
                                    FuncInline.SiteAction[SiteIndex] = FuncInline.enumSiteAction.Loading;
                                }
                                else
                                {
                                    // 세대/사이트에 따른 SMD COM 매핑 후 Start 전송
                                    int smdIndex = FuncInline.MapSmdIndex(SiteIndex);
                                    int siteIndexToSend = SiteIndex; // 장치 쪽은 0-based로 전송(기존 코드와 동일)
                                                                     // 필요 어레이 중 Before_Command 개수라도 있으면 전송(기존 판단 로직은 내부에 동일하게 존재)
                                    FuncInline.ComSMD[smdIndex].SendStart(siteIndexToSend); // 지령 전송
                                                                                            // (MapSmdIndex / SendStart 사용 근거) :contentReference[oaicite:3]{index=3}
                                }
                            }

                            // === 8) 테스팅 중 타이머 관리 ===
                            if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Testing)
                            {
                                if (FuncInline.PCBInfo[(int)pos].StopWatch == null ||
                                    !FuncInline.PCBInfo[(int)pos].StopWatch.IsRunning)
                                {
                                    Util.StartWatch(ref FuncInline.PCBInfo[(int)pos].StopWatch);
                                }
                            }

                           
                        }
                        // === 9) 테스트 종료된 사이트는 핀업/언클램프 처리 ===
                        if (FuncInline.PCBInfo[(int)pos].PCBStatus > FuncInline.enumSMDStatus.Testing)
                        {
                            FuncInline.SiteUnclamp(pos); // 핀 업 & 클램프 해제(사이트별 DO/DI 매핑 내부 사용)
                        }
                    }


                    if (GlobalVar.SystemStatus > enumSystemStatus.Initialize)
                    {
                        #region Site 관련
                        if (GlobalVar.SystemStatus > enumSystemStatus.Initialize)
                        {
                            //for (int i = 0; i < FuncInline.MaxSiteCount; i++)
                            //{
                            int smdIndex = SiteIndex / FuncInline.MaxSiteCount;
                            //FuncInline.enumTeachingPos pos = FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex;

                            #region 테스트 진행 관련

                            #region 사이트 및 보드 상태 강제 변경

                            #region PCB:test_pass/test_fail, Site:Testing ==> Site:Waiting으로
                            //*
                            //if (pos == FuncInline.enumTeachingPos.Site17)
                            //{
                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                                GlobalVar.SystemStatus != enumSystemStatus.ErrorStop &&
                                FuncInline.SiteAction[SiteIndex] == FuncInline.enumSiteAction.Testing &&
                                FuncInline.GetDISiteData(SiteIndex, FuncInline.enumDISite.X1_Site_PCB_Stop_Sensor) &&
                                (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Pass ||
                                        FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Test_Fail ||
                                        FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.ReTest))
                            {
                                //debug(pos.ToString() + " ===> waiting");
                                //FuncLog.WriteLog("AutoRun : SiteAction Change testing finish - " + pos.ToString() + " waiting");
                                if (FuncInline.SiteAction[SiteIndex] != FuncInline.enumSiteAction.Waiting)
                                {
                                    FuncLog.WriteLog_Tester("Site" + (SiteIndex + 1).ToString() + " action change : " + FuncInline.SiteAction[SiteIndex].ToString() + " ==> Waiting");
                                }
                                FuncInline.SiteAction[SiteIndex] = FuncInline.enumSiteAction.Waiting;
                                /*
                                if (FuncInline.PCBInfo[(int)pos].StopWatch != null &&
                                    FuncInline.PCBInfo[(int)pos].StopWatch.IsRunning)
                                {
                                    FuncInline.PCBInfo[(int)pos].StopWatch.Stop();
                                    //PCBInfo[(int)pos].StopWatch.Reset();
                                }
                                //*/
                            }
                            //}
                            //*/
                            #endregion

                            /* 하나씩 로딩하려니 쓰레드가 나눠져 있어 무조건 동시에 로딩하므로, 공통쓰레드로 옮김
                            #region PC orange 해제되고, Buyerchange white고, buyerchange 비트 살아 있으면 해제
                            bool loadingExist = false;
                            for (int i = 0; i < 20; i++)
                            {
                                if (FuncInline.SiteAction[pcIndex * 20 + i] == FuncInline.enumSiteAction.Loading)
                                {
                                    loadingExist = true;
                                    break;
                                }
                            }
                            if (!loadingExist &&
                                GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.White &&
                                !FuncInline.BuyerChangeOrange[pcIndex] &&
                                FuncInline.PCBInfo[(int)pos].BuyerChange)
                            {
                                FuncInline.PCBInfo[(int)pos].BuyerChange = false;
                            }
                            #endregion
                            //*/

                            #region pcbInfo:before_command인데 siteaction:waiting인 경우 buyerchange 해제되면 testing으로
                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                                GlobalVar.SystemStatus != enumSystemStatus.ErrorStop &&
                                ((FuncInline.BuyerChange != FuncInline.enumBuyerChange.Blue && FuncInline.BuyerChange != FuncInline.enumBuyerChange.Orange) || !FuncInline.PCBInfo[(int)pos].BuyerChange) &&
                                FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Before_Command &&
                                FuncInline.SiteAction[SiteIndex] != FuncInline.enumSiteAction.Testing &&
                                FuncInline.SiteAction[SiteIndex] != FuncInline.enumSiteAction.Loading &&
                                (!FuncInline.PCBInfo[(int)pos].SelfReTest || FuncInline.PCBInfo[(int)pos].SelfRetestCount == 0) &&
                                (!FuncInline.PCBInfo[(int)pos].OtherReTest || FuncInline.PCBInfo[(int)pos].OtherRetestCount == 0))
                            {
                                //debug(pos.ToString() + " !blue ===> testing");
                                //FuncLog.WriteLog("AutoRun : SiteAction Change not blue - " + pos.ToString() + " Testing");
                                if (FuncInline.SiteAction[SiteIndex] != FuncInline.enumSiteAction.Testing)
                                {
                                    FuncLog.WriteLog_Tester("Site" + (SiteIndex + 1).ToString() + " action change : " + FuncInline.SiteAction[SiteIndex].ToString() + " ==> Testing");
                                }
                                FuncInline.SiteAction[SiteIndex] = FuncInline.enumSiteAction.Testing;
                                Util.ResetWatch(ref FuncInline.SiteCheckTime[SiteIndex]);
                                Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex].StopWatch);
                                for (int j = 0; j < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex].TestWatch.Length; j++)
                                {
                                    Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex].TestWatch[j]);
                                }
                            }
                            #endregion

                            #region pcbInfo:testing인데 siteaction:waiting인 경우 testing으로
                            //*
                            if (GlobalVar.SystemStatus >= enumSystemStatus.AutoRun &&
                                GlobalVar.SystemStatus != enumSystemStatus.ErrorStop &&
                                FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Testing &&
                                FuncInline.SiteAction[SiteIndex] != FuncInline.enumSiteAction.Testing &&
                                !FuncInline.PCBInfo[(int)pos].SelfReTest &&
                                !FuncInline.PCBInfo[(int)pos].OtherReTest)
                            {
                                //debug(pos.ToString() + " waiting ===> testing");
                                //FuncLog.WriteLog("AutoRun : SiteAction Change PCB testing - " + pos.ToString() + " Testing");
                                if (FuncInline.SiteAction[SiteIndex] != FuncInline.enumSiteAction.Testing)
                                {
                                    FuncLog.WriteLog_Tester("Site" + (SiteIndex + 1).ToString() + " action change : " + FuncInline.SiteAction[SiteIndex].ToString() + " ==> Testing");
                                }
                                FuncInline.SiteAction[SiteIndex] = FuncInline.enumSiteAction.Testing;
                            }
                            //*/
                            #endregion

                            #region waiting 인데 타이머 돌고 있으면 정지
                            //*
                            if (FuncInline.SiteAction[SiteIndex] == FuncInline.enumSiteAction.Waiting &&
                                FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                                FuncInline.PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.Testing &&
                                FuncInline.PCBInfo[(int)pos].StopWatch != null &&
                                FuncInline.PCBInfo[(int)pos].StopWatch.IsRunning)
                            {
                                if (FuncInline.PCBInfo[(int)pos].TestTime == 0 &&
                                    FuncInline.PCBInfo[(int)pos].PCBStatus > FuncInline.enumSMDStatus.Testing)
                                {
                                    FuncInline.PCBInfo[(int)pos].TestTime = (int)(FuncInline.PCBInfo[(int)pos].StopWatch.ElapsedMilliseconds / 1000);
                                    FuncInline.TestTime[SiteIndex] = FuncInline.PCBInfo[(int)pos].TestTime;
                                    FuncInline.TestCycleTime = (double)FuncInline.PCBInfo[(int)pos].TestTime / (double)40;
                                }
                                Util.ResetWatch(ref FuncInline.PCBInfo[(int)pos].StopWatch);
                                //PCBInfo[(int)pos].StopWatch.Reset();
                                for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                                {
                                    Util.ResetWatch(ref FuncInline.PCBInfo[(int)pos].TestWatch[i]);
                                }
                            }
                            #endregion
                            #region 바이어체인지중에는 전체 타이머 정지
                            if (FuncInline.PCBInfo[(int)pos].BuyerChange)
                            {
                                Util.ResetWatch(ref FuncInline.PCBInfo[(int)pos].StopWatch);
                                //PCBInfo[(int)pos].StopWatch.Reset();
                                for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                                {
                                    Util.ResetWatch(ref FuncInline.PCBInfo[(int)pos].TestWatch[i]);
                                }
                            }
                            #endregion

                            #endregion

                            #region 테스트 현황 집계
                            // command_sent로 시간경과. 시작된 array 있으면 기다려야 하나? 재전송은 ?
                            // response_ng 경우 - 시작된 array 있으면 기다려야 하나? 재전송은?
                            // test_pass는 정상 배출로 연결
                            // test_fail도 정상 배출로 연결
                            #region 시간경과 판단 - 어레이별로 판단해야 함
                            #region 어레이별 타임아웃 판단
                            bool timeout = false;
                            for (int j = 0; j < FuncInline.MaxArrayCount; j++)
                            {

                                #region Testing인데 TestWatch 시작 안 되어있으면 시작
                                if (FuncInline.PCBInfo[(int)pos].TestWatch[j] == null ||
                                    !FuncInline.PCBInfo[(int)pos].TestWatch[j].IsRunning)
                                {
                                    Util.StartWatch(ref FuncInline.PCBInfo[(int)pos].TestWatch[j]);
                                }
                                #endregion

                                #region command retry & timeout
                                if (FuncInline.SiteAction[SiteIndex] == FuncInline.enumSiteAction.Testing &&
                                    !FuncInline.SimulationMode &&
                                    !GlobalVar.DryRun &&
                                    !FuncInline.PCBInfo[(int)pos].BuyerChange &&
                                    //!FuncInline.PCBInfo[(int)pos].SelfReTest &&
                                    //PCBInfo[(int)pos].PCBStatus != FuncInline.enumSMDStatus.ReTest &&
                                    FuncInline.PCBInfo[(int)pos].SMDStatus[j] >= FuncInline.enumSMDStatus.Before_Command &&
                                    FuncInline.PCBInfo[(int)pos].SMDStatus[j] <= FuncInline.enumSMDStatus.Command_Sent &&
                                    FuncInline.PCBInfo[(int)pos].TestWatch[j].IsRunning &&
                                    FuncInline.PCBInfo[(int)pos].TestWatch[j].ElapsedMilliseconds > FuncInline.TestCommandTimeout * 1000)
                                {
                                    if (FuncInline.PCBInfo[(int)pos].CommandRetry[j] >= FuncInline.TestCommandRetry)
                                    {
                                        timeout = true;
                                        FuncInline.PCBInfo[(int)pos].TestWatch[j].Stop();
                                        FuncError.AddError(new FuncInline.structError(DateTime.Now.ToString("yyyyMMdd"),
                                               DateTime.Now.ToString("HH:mm:ss"),
                                               FuncInline.enumErrorPart.Site1_F_DT1 + SiteIndex,
                                               FuncInline.enumErrorCode.Test_Command_Timeout,
                                               false,
                                               pos.ToString() + " " + (FuncInline.PCBInfo[(int)pos].SMDReadySent[j] ? "Ready" : "Test") + " Command Timed out. Check Test PC."));
                                        FuncInline.PCBInfo[(int)pos].PCBStatus = FuncInline.enumSMDStatus.Testing;
                                        //debug((pos).ToString() + "==> OK");
                                        FuncInline.PCBInfo[(int)pos].NgType = FuncInline.enumNGType.OK;

                                        FuncInline.ArrayNGCount[SiteIndex, j]++;
                                        FuncInline.ArrayNGCode[SiteIndex, j] = 999;
                                        #region 어레이 불량 누적 또는 연속 발생시 site block
                                        if (FuncInline.BlockDefectArray &&
                                            FuncInline.ArrayNGCount[SiteIndex, j] >= FuncInline.BlockNGCount)
                                        {
                                            FuncLog.WriteLog("Site #" + (SiteIndex + 1).ToString() + " Blocked. Array #" + (j + 1).ToString() + " NG count exceeded. ErrorCode : 999");
                                            FuncInline.BlockReason[SiteIndex] = "NG Count Block. ErrorCode : 999";
                                            FuncInline.InsertBlockHistory(SiteIndex + 1, false, "Array #" + (j + 1).ToString() + " NG Block. 999 : Command_Timeout");
                                            #region 바이어체인지시는 바로 안 막고 예정에 등록
                                            int orangeIndex = (SiteIndex) / 7; // 리프트 기준이 아니라 테스트PC별로 실행해야 하므로 일단 7로 하드코딩. 전역변수 선언 필요. 빈사이트 판단도 변경 필요.
                                            if (FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue ||
                                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ||
                                                FuncInline.BuyerChangeOrange[orangeIndex])
                                            {
                                                FuncInline.BuyerChangeBlock[SiteIndex] = true;
                                            }
                                            #endregion
                                            else
                                            {
                                                FuncInline.UseSite[SiteIndex] = false;
                                            }
                                        }
                                        #endregion
                                        #region 불량율 초과시 사이트 블럭
                                        if (FuncInline.BlockDefectArray &&
                                            FuncInline.CheckDefectRateBlock(SiteIndex + 1, j + 1))
                                        {
                                            FuncInline.BlockReason[SiteIndex] = "Defect Rate Block.";
                                            FuncLog.WriteLog("Site #" + (SiteIndex).ToString() + " Blocked. Array #" + (j + 1).ToString() + " Defect Rate Over");
                                            FuncInline.InsertBlockHistory(SiteIndex + 1, false, "Array #" + (j + 1).ToString() + " Defect Rate Over");
                                            #region 바이어체인지시는 바로 안 막고 예정에 등록
                                            int orangeIndex = (SiteIndex) / 7; // 전역변수로 변경 필요.
                                            if (FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue ||
                                                FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ||
                                                FuncInline.BuyerChangeOrange[orangeIndex])
                                            {
                                                FuncInline.BuyerChangeBlock[SiteIndex] = true;
                                            }
                                            #endregion
                                            else
                                            {
                                                FuncInline.UseSite[SiteIndex] = false;
                                            }
                                        }
                                        #endregion

                                        Thread.Sleep(GlobalVar.ThreadSleep);
                                        continue;
                                    }
                                    else
                                    {
                                        if (GlobalVar.SMDLog &&
                                            FuncInline.PCBInfo[(int)pos].SMDStatus[j] != FuncInline.enumSMDStatus.Before_Command)
                                        {
                                            FuncInline.PCBInfo[(int)pos].SMDReady[j] = false;
                                            FuncInline.PCBInfo[(int)pos].SMDReadySent[j] = false;
                                            FuncLog.WriteLog_Tester("PCBInfo " + pos.ToString() + " .SMDStatus " + (j + 1) + " = FuncInline.enumSMDStatus.Before_Command");
                                        }
                                        FuncInline.PCBInfo[(int)pos].SMDStatus[j] = FuncInline.enumSMDStatus.Before_Command;
                                        FuncInline.ComSMD[smdIndex].SendStart(SiteIndex);
                                        //PCBInfo[(int)pos].CommandRetry[j]++;
                                    }
                                }
                                #endregion
                                #region test timeout
                                if (FuncInline.SiteAction[SiteIndex] == FuncInline.enumSiteAction.Testing &&
                                    !FuncInline.PCBInfo[(int)pos].BuyerChange &&
                                    FuncInline.PCBInfo[(int)pos].SMDStatus[j] >= FuncInline.enumSMDStatus.Before_Command &&
                                    FuncInline.PCBInfo[(int)pos].SMDStatus[j] <= FuncInline.enumSMDStatus.Testing &&
                                    FuncInline.PCBInfo[(int)pos].TestWatch[j].IsRunning &&
                                    FuncInline.PCBInfo[(int)pos].TestWatch[j].ElapsedMilliseconds > FuncInline.TestTimeout * 1000)
                                {
                                    timeout = true;
                                    if (GlobalVar.SMDLog &&
                                        FuncInline.PCBInfo[(int)pos].SMDStatus[j] != FuncInline.enumSMDStatus.Test_Timeout)
                                    {
                                        FuncLog.WriteLog_Tester("PCBInfo " + pos.ToString() + " .SMDStatus " + (j + 1) + " = FuncInline.enumSMDStatus.Test_Timeout " + FuncInline.PCBInfo[(int)pos].TestWatch[j].ElapsedMilliseconds);
                                    }
                                    FuncInline.PCBInfo[(int)pos].ErrorCode[j] = 999; // Test Timeout
                                    FuncInline.PCBInfo[(int)pos].SMDStatus[j] = FuncInline.enumSMDStatus.Test_Timeout;
                                    FuncInline.PCBInfo[(int)pos].TestTime = FuncInline.TestTimeout;
                                    FuncInline.TestTime[SiteIndex] = FuncInline.PCBInfo[(int)pos].TestTime;
                                    FuncInline.InsertCommLog(SiteIndex + 1, j + 1, "NG", "Test Timeout", FuncInline.PCBInfo[(int)pos].TestWatch[j].ElapsedMilliseconds / 1000, 999, "TEST_TIMEOUT"); // TEST_TIMEOUT

                                    FuncInline.ArrayNGCount[SiteIndex, j]++;
                                    FuncInline.ArrayNGCode[SiteIndex, j] = 999;
                                    #region 어레이 불량 누적 또는 연속 발생시 site block
                                    if (FuncInline.BlockDefectArray &&
                                            FuncInline.ArrayNGCount[SiteIndex, j] >= FuncInline.BlockNGCount)
                                    {
                                        FuncLog.WriteLog("Site #" + (SiteIndex + 1).ToString() + " Blocked. Array #" + (j + 1).ToString() + " NG count exceeded. ErrorCode : 999");
                                        FuncInline.BlockReason[SiteIndex] = "NG Count Block. ErrorCode : 999";
                                        FuncInline.InsertBlockHistory(SiteIndex + 1, false, "Array #" + (j + 1).ToString() + " NG Block. 999 : Test_Timeout");
                                        #region 바이어체인지시는 바로 안 막고 예정에 등록
                                        int orangeIndex = (SiteIndex) / 7; // 전역 변수로 변경 필요
                                        if (FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue ||
                                            FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ||
                                            FuncInline.BuyerChangeOrange[orangeIndex])
                                        {
                                            FuncInline.BuyerChangeBlock[SiteIndex] = true;
                                        }
                                        #endregion
                                        else
                                        {
                                            FuncInline.UseSite[SiteIndex] = false;
                                        }
                                    }
                                    #endregion
                                    #region 불량율 초과시 사이트 블럭
                                    if (FuncInline.BlockDefectArray &&
                                            FuncInline.CheckDefectRateBlock(SiteIndex + 1, j + 1))
                                    {
                                        FuncInline.BlockReason[SiteIndex] = "Defect Rate Block.";
                                        FuncInline.InsertBlockHistory(SiteIndex + 1, false, "Array #" + (j + 1).ToString() + " Defect Rate Over");
                                        FuncLog.WriteLog("Site #" + (SiteIndex).ToString() + " Blocked. Array #" + (j + 1).ToString() + " Defect Rate Over");
                                        #region 바이어체인지시는 바로 안 막고 예정에 등록
                                        int orangeIndex = (SiteIndex) / 7; // 전역 변수로 변경 필요.
                                        if (FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue ||
                                            FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ||
                                            FuncInline.BuyerChangeOrange[orangeIndex])
                                        {
                                            FuncInline.BuyerChangeBlock[SiteIndex] = true;
                                        }
                                        #endregion
                                        else
                                        {
                                            FuncInline.UseSite[SiteIndex] = false;
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                            #region 사이트 검사시간 타임아웃
                            if (!timeout)
                            {
                                if (FuncInline.PCBInfo[(int)pos].StopWatch.ElapsedMilliseconds > FuncInline.TestTimeout * 1000)
                                {
                                    timeout = true;
                                }
                            }
                            #endregion
                            #region timeout 경우 STS 발송
                            if (timeout)
                            {
                                int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                                FuncInline.PCBInfo[(int)pos].PCBStatus = FuncInline.enumSMDStatus.Test_Timeout;
                                FuncInline.ComSMD[smdIndex].SendStop(siteIndex);
                            }
                            #endregion


                            #endregion

                            int site = SiteIndex;
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus >= FuncInline.enumSMDStatus.Command_Sent)
                            {
                                FuncInline.enumSMDStatus before_status = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus;

                                #region 전체 어레이에서 재테스트 여부 확인 - 하나라도 재테스트 지정되어 있으면 재테스트 수행
                                // SendStart에서도 재테스트면 이전 에러코드에 따라서 재테스트 수행
                                bool needRetest = false;
                                for (int j = 0; j < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode.Length; j++)
                                {
                                    try
                                    {
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode[j] > -1 &&
                                            //PCBInfo[(int)FuncInline.enumTeachingPos.Site7 + site].SMDStatus[j] == FuncInline.enumSMDStatus.Test_Fail &&
                                            FuncInline.TestErrorRetest[FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode[j]])
                                        {
                                            needRetest = true;
                                            break;
                                        }
                                    }
                                    catch (Exception xx)
                                    {
                                        //debug(xx.ToString());
                                        //debug(xx.StackTrace);
                                    }
                                }
                                #endregion

                                #region 전체 어레이 집계
                                try
                                {
                                    bool beforeCmd = false; // 지령 전송 안 한 거 있으면 지령전송 대기
                                    bool cmdSent = false; // 지령 회신 대기중 있으면 지령회신 대기
                                    bool testing = false; // 하나라도 testing이면 testing
                                    bool allPass = true; // 모두 pass라야 pass

                                    #region 테스트 진행 이전 상테 체크 - 하나라도 낮은 상태값 있는 것을 PCB 상태로 본다. Pass 처리 포함
                                    for (int array = 0; array < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode.Length; array++)
                                    {
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[array].Length > 0)
                                        {
                                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[array] == FuncInline.enumSMDStatus.Before_Command)
                                            {
                                                beforeCmd = true;
                                            }
                                            if (!beforeCmd &&
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[array] == FuncInline.enumSMDStatus.Command_Sent)
                                            {
                                                cmdSent = true;
                                            }
                                            if (!beforeCmd &&
                                                !cmdSent &&
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[array] == FuncInline.enumSMDStatus.Testing)
                                            {
                                                testing = true;
                                            }
                                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[array] != FuncInline.enumSMDStatus.Test_Pass &&
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[array] != FuncInline.enumSMDStatus.No_Test)
                                            {
                                                allPass = false;
                                            }
                                        }
                                    }
                                    #region before_command 라도 retest 대상이면 무시
                                    if (beforeCmd &&
                                        needRetest)
                                    {
                                        if (FuncInline.SelfRetest &&
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SelfRetestCount < FuncInline.SelfRetestCount)
                                        {
                                            beforeCmd = false;
                                        }
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SelfReTest)
                                        {
                                            beforeCmd = false;
                                        }
                                        if (FuncInline.OtherRetest &&
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].OtherRetestCount < FuncInline.OtherRetestCount)
                                        {
                                            beforeCmd = false;
                                        }
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].OtherReTest)
                                        {
                                            beforeCmd = false;
                                        }
                                    }
                                    #endregion
                                    if (beforeCmd)
                                    {
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.Before_Command;
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType = FuncInline.enumNGType.OK;
                                        //debug((FuncInline.enumTeachingPos.Site7 + site).ToString() + "==> OK");
                                    }
                                    else if (cmdSent)
                                    {
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.Command_Sent;
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType = FuncInline.enumNGType.OK;
                                        //debug((FuncInline.enumTeachingPos.Site7 + site).ToString() + "==> OK");
                                    }
                                    else if (testing &&
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus != FuncInline.enumSMDStatus.Testing)
                                    {
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.Testing;
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType = FuncInline.enumNGType.OK;
                                        //debug((FuncInline.enumTeachingPos.Site7 + site).ToString() + "==> OK");
                                        /*
                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch == null ||
                                            !FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch.IsRunning)
                                        {
                                            Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch);
                                        }
                                        //*/
                                    }

                                    #region 양품 처리
                                    else if (allPass)
                                    {


                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.Test_Pass;
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType = FuncInline.enumNGType.OK;
                                        //debug((FuncInline.enumTeachingPos.Site7 + site).ToString() + "==> OK");
                                        #region 바이어체인지 PC별 체크 해제
                                        if (FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange)
                                        {
                                            FuncInline.BuyerChange = FuncInline.enumBuyerChange.White;
                                        }
                                        if (site >= 0 &&
                                            site < 7 &&
                                            FuncInline.BuyerChangeOrange[0])
                                        {
                                            FuncInline.BuyerChangeOrange[0] = false;
                                            /*
                                            for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                                            {
                                                FuncInline.SiteInputCount[i] = 0;
                                            }
                                            //*/
                                        }
                                        if (site >= 7 &&
                                            site < 14 &&
                                            FuncInline.BuyerChangeOrange[1])
                                        {
                                            FuncInline.BuyerChangeOrange[1] = false;
                                            /*
                                            for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                                            {
                                                FuncInline.SiteInputCount[i] = 0;
                                            }
                                            //*/
                                        }
                                        if (site >= 14 &&
                                            site < 20 &&
                                            FuncInline.BuyerChangeOrange[2])
                                        {
                                            FuncInline.BuyerChangeOrange[2] = false;
                                            /*
                                            for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                                            {
                                                FuncInline.SiteInputCount[i] = 0;
                                            }
                                            //*/
                                        }
                                        #endregion
                                    }
                                    #endregion
                                    #endregion
                                    #region 테스트 이후 상태값 체크. Pass 이외 경우
                                    if (!beforeCmd &&
                                        !cmdSent &&
                                        !testing &&
                                        !allPass)
                                    {
                                        #region timeout
                                        bool chkTimeout = false;
                                        for (int j = 0; j < FuncInline.MaxArrayCount; j++)
                                        {
                                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex].SMDStatus[j] == FuncInline.enumSMDStatus.Test_Timeout)
                                            {
                                                if (GlobalVar.SMDLog &&
                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex].PCBStatus != FuncInline.enumSMDStatus.Test_Timeout)
                                                {
                                                    FuncLog.WriteLog_Tester("PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex).ToString() + " .PCBStatus = FuncInline.enumSMDStatus.Test_Timeout");
                                                }
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex].PCBStatus = FuncInline.enumSMDStatus.Test_Timeout;
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + SiteIndex].NgType = FuncInline.enumNGType.Timeout;
                                                //debug((FuncInline.enumTeachingPos.Site7 + i).ToString() + "==> Timeout");
                                                chkTimeout = true;
                                                break;
                                            }
                                        }
                                        #endregion

                                        #region FAIL CODE 없는 경우 무조건 FAIL
                                        bool no_code = false;
                                        for (int array = 0; array < FuncInline.MaxArrayCount; array++)
                                        {
                                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[array].Length > 0 &&
                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode[array] == 992)
                                            {
                                                no_code = true;
                                            }
                                        }
                                        if (no_code)
                                        {
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.Test_Fail;
                                        }
                                        #endregion
                                        #region 무조건 NG 경우
                                        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus == FuncInline.enumSMDStatus.Response_NG ||
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout ||
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus == FuncInline.enumSMDStatus.Test_Cancel ||
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus == FuncInline.enumSMDStatus.User_Cancel ||
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus == FuncInline.enumSMDStatus.Test_Timeout)
                                        {
                                            // 상태 변경을 하지 말것
                                        }
                                        #endregion
                                        #region timeout 이외
                                        else if (!chkTimeout)
                                        {

                                            // 전체상태가 같지 않고,테스트중인 것도 없고. NG 있고 나머지 완료된 경우
                                            for (int j = 0; j < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus.Length; j++)
                                            {
                                                if (FuncInline.ArrayUse[j] &&
                                                    (//PCBInfo[(int)FuncInline.enumTeachingPos.Site7 + site].SMDStatus[j] == FuncInline.enumSMDStatus.Response_NG ||
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[j] == FuncInline.enumSMDStatus.Test_Fail))
                                                {
                                                    // SS/STS 취소시 재테스트 안 하고 바로 불량 처리
                                                    // FL 경우 재테스트 고려해야 함. - 하나라도 에러코드 재테스트 여부 체크된 게 있으면
                                                    // 나머지 경우는 Timeout, 재테스트 한다.
                                                    bool retestChecked = false;
                                                    #region 제자리 Retest
                                                    if (FuncInline.SelfRetest &&
                                                        needRetest)
                                                    //!FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site7 + site].UserCancel)
                                                    {
                                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SelfRetestCount < FuncInline.SelfRetestCount)
                                                        {
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SelfRetestCount++;
                                                            for (int k = 0; k < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus.Length; k++)
                                                            {
                                                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Xout[k] ||
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].BadMark[k])
                                                                {
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] = FuncInline.enumSMDStatus.No_Test;
                                                                }
                                                                else if (FuncInline.ArrayUse[k] &&
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] != FuncInline.enumSMDStatus.Test_Pass &&
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] != FuncInline.enumSMDStatus.No_Test)
                                                                {
                                                                    if (GlobalVar.SMDLog &&
                                                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] != FuncInline.enumSMDStatus.Before_Command)
                                                                    {
                                                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDReady[k] = false;
                                                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDReadySent[k] = false;
                                                                        FuncLog.WriteLog_Tester("PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .SMDStatus " + (j + 1) + " = FuncInline.enumSMDStatus.Before_Command");
                                                                    }
                                                                    Util.ResetWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestWatch[k]);
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] = FuncInline.enumSMDStatus.Before_Command;
                                                                }
                                                            }
                                                            if (GlobalVar.SMDLog &&
                                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus != FuncInline.enumSMDStatus.Before_Command)
                                                            {
                                                                FuncLog.WriteLog_Tester("PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .PCBStatus = FuncInline.enumSMDStatus.Before_Command");
                                                            }
                                                            /*
                                                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus != FuncInline.enumSMDStatus.Before_Command)
                                                            {
                                                                Util.ResetWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch);
                                                            }
                                                            //*/
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.Before_Command;
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType = FuncInline.enumNGType.OK;
                                                            //debug((FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + "==> OK");
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SelfReTest = true;
                                                            //FuncLog.WriteLog("AutoRun : SiteAction Change self retest - " + pos.ToString() + " ReOpen");
                                                            if (FuncInline.SiteAction[site] != FuncInline.enumSiteAction.ReOpen)
                                                            {
                                                                FuncLog.WriteLog_Tester("Site" + (site + 1).ToString() + " action change : " + FuncInline.SiteAction[site].ToString() + " ==> ReOpen");
                                                            }
                                                            FuncInline.SiteAction[site] = FuncInline.enumSiteAction.ReOpen;
                                                            retestChecked = true;
                                                        }
                                                    }
                                                    #endregion

                                                    #region resite 처리
                                                    if (!retestChecked &&
                                                        FuncInline.OtherRetest &&
                                                        needRetest)
                                                    //!FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].UserCancel)
                                                    {
                                                        #region 리사이트자리 없으면 바로 불량 처리. 중복 처리
                                                        if (FuncInline.FailWhenNoEmpty &
                                                            FuncInline.CheckResitePosible(site))
                                                        {
                                                            /*
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.Test_Fail;
                                                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType == FuncInline.enumNGType.OK)
                                                            {
                                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType = FuncInline.enumNGType.NormalFail;
                                                                //debug((FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + "==> NormalFail");
                                                            }

                                                            #region 실패 사유 구분
                                                            //FuncInline.CheckFailType(FuncInline.enumTeachingPos.Site1_F_DT1 + site);
                                                            #endregion
                                                            //*/
                                                        }
                                                        #endregion

                                                        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].OtherRetestCount < FuncInline.OtherRetestCount)
                                                        {
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].OtherRetestCount++;
                                                            for (int k = 0; k < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus.Length; k++)
                                                            {
                                                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Xout[k] ||
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].BadMark[k])
                                                                {
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] = FuncInline.enumSMDStatus.No_Test;
                                                                }
                                                                else if (FuncInline.ArrayUse[k] &&
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] != FuncInline.enumSMDStatus.Test_Pass &&
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] != FuncInline.enumSMDStatus.No_Test)
                                                                {
                                                                    if (GlobalVar.SMDLog &&
                                                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] != FuncInline.enumSMDStatus.Before_Command)
                                                                    {
                                                                        FuncInline.PCBInfo[(int)pos].SMDReady[j] = false;
                                                                        FuncInline.PCBInfo[(int)pos].SMDReadySent[j] = false;
                                                                        FuncLog.WriteLog_Tester("PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .SMDStatus " + (k + 1) + " = FuncInline.enumSMDStatus.Before_Command");
                                                                    }
                                                                    Util.ResetWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestWatch[k]);
                                                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[k] = FuncInline.enumSMDStatus.Before_Command;
                                                                }
                                                            }
                                                            if (GlobalVar.SMDLog &&
                                                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus != FuncInline.enumSMDStatus.ReTest)
                                                            {
                                                                FuncLog.WriteLog_Tester("PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .PCBStatus = FuncInline.enumSMDStatus.ReTest");
                                                            }
                                                            /*
                                                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus != FuncInline.enumSMDStatus.ReTest)
                                                            {
                                                                Util.ResetWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch);
                                                            }
                                                            //*/
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].OtherReTest = true;
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.ReTest;
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType = FuncInline.enumNGType.OK;
                                                            //debug((FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + "==> OK");
                                                            retestChecked = true;
                                                        }
                                                    }
                                                    #endregion

                                                    #region 재테스트 아니면 실패 처리
                                                    if (!retestChecked) // 재테스트 아니면 실패 처리
                                                    {
                                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[j];
                                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType == FuncInline.enumNGType.OK)
                                                        {
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType = FuncInline.enumNGType.NormalFail;
                                                            //debug((FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + "==> NormalFail");
                                                        }

                                                        #region 실패 사유 구분
                                                        //FuncInline.CheckFailType(FuncInline.enumTeachingPos.Site1_F_DT1 + site);
                                                        #endregion

                                                        /*
                                                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus != FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[j])
                                                        {
                                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch.Stop();
                                                        }
                                                        //*/
                                                    }
                                                    #endregion
                                                    break;
                                                }
                                            }


                                        }
                                        #endregion

                                    }
                                    #endregion

                                    #region 검사 끝난 보드 타이머 정지
                                    //*
                                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus > FuncInline.enumSMDStatus.Testing &&
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch.IsRunning &&
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch.ElapsedMilliseconds > 0 &&
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestTime == 0)
                                    {
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestTime = (int)(FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch.ElapsedMilliseconds / 1000);
                                        FuncInline.TestTime[SiteIndex] = FuncInline.PCBInfo[(int)pos].TestTime;
                                        FuncInline.TestCycleTime = (double)FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestTime / (double)40;
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch.Stop();
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch.Reset();
                                    }
                                    //*/
                                    #endregion

                                    // 에러코드 없는 것 있으면 바로 NG로
                                    /*
                                    for (int k = 0; k < FuncInline.PCBInfo[(int)pos].ErrorCode.Length; k++)
                                    {
                                        if (FuncInline.PCBInfo[(int)pos].ErrorCode[k] >= 0 &&
                                            (FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)pos].ErrorCode[k]] == null ||
                                                    FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)pos].ErrorCode[k]] == ""))
                                        {
                                            FuncInline.PCBInfo[(int)pos].PCBStatus = FuncInline.enumSMDStatus.Test_Fail;
                                            FuncInline.PCBInfo[(int)pos].OtherReTest = false;
                                            FuncInline.PCBInfo[(int)pos].SelfReTest = false;
                                        }
                                    }
                                    //*/

                                    #region 실패 사유 구분
                                    FuncInline.CheckFailType(FuncInline.enumTeachingPos.Site1_F_DT1 + site);
                                    #endregion
                                }
                                catch (Exception xx)
                                {
                                    //debug(xx.ToString());
                                    //debug(xx.StackTrace);
                                }
                                //*/
                                #endregion

                                #region PCB 검사종료, 사이트 Testing이면 Waiting으로
                                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus > FuncInline.enumSMDStatus.Testing &&
                                    FuncInline.SiteAction[site] == FuncInline.enumSiteAction.Testing)
                                {
                                    if (FuncInline.SiteAction[site] != FuncInline.enumSiteAction.Waiting)
                                    {
                                        FuncLog.WriteLog_Tester("Site" + (site + 1).ToString() + " action change : " + FuncInline.SiteAction[site].ToString() + " ==> Waiting");
                                    }
                                    FuncInline.SiteAction[site] = FuncInline.enumSiteAction.Waiting;
                                }
                                #endregion

                                if (before_status != FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus)
                                {
                                    if (GlobalVar.SMDLog)
                                    {
                                        FuncLog.WriteLog_Tester("Site" + (site + 1).ToString() + " Status change : " + beforeStatus + " ==> " + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus);
                                    }
                                }
                            }
                            #endregion

                            #endregion


                            //}
                        }
                        #endregion
                    }

                    // 현재 사이트의 연결상태값 저장
                    if (FuncFastech.GetModuleID(enumFastechType.DI, SiteIndex) < 2 ||
                        FuncFastech.GetModuleID(enumFastechType.DO, SiteIndex) < 2 ||
                        FuncFastech.GetModuleID(enumFastechType.Motion, SiteIndex) < 2) // Fastech Slave 확인된 게 없으면 DisCon
                    {
                        FuncInline.SiteState[SiteIndex] = FuncInline.enumSiteState.DisCon;
                    }
                    else if (!FuncInline.GetDISiteData(SiteIndex, (int)FuncInline.enumDISite.X28_Module_Power)) // 전원  DI 안 들어 왔으면 PowerOff
                    {
                        FuncInline.SiteState[SiteIndex] = FuncInline.enumSiteState.PowerOff;
                    }
                    else if (!FuncInline.UseSite[SiteIndex]) // NotUse 지정
                    {
                        FuncInline.SiteState[SiteIndex] = FuncInline.enumSiteState.NotUse;
                    }
                    else // 기타 정상
                    {
                        FuncInline.SiteState[SiteIndex] = FuncInline.enumSiteState.Valid;
                    }
                }
                catch (Exception ex)
                {
                    //debug(ex.ToString());
                    //debug(ex.StackTrace);
                }
                beforeStatus = GlobalVar.SystemStatus;

                //debug("SiteAction time : " + watch.ElapsedMilliseconds);


                /* 임시 설정
                FuncInline.SiteAction[(int)FuncInline.enumTeachingPos.Site6 - (int)FuncInline.enumTeachingPos.Site7] = FuncInline.enumSiteAction.Testing;
                FuncInline.SiteAction[(int)FuncInline.enumTeachingPos.Site13 - (int)FuncInline.enumTeachingPos.Site7] = FuncInline.enumSiteAction.Testing;
                FuncInline.SiteAction[(int)FuncInline.enumTeachingPos.Site14 - (int)FuncInline.enumTeachingPos.Site7] = FuncInline.enumSiteAction.Testing;
                FuncInline.SiteAction[(int)FuncInline.enumTeachingPos.Site14_R_DT1 - (int)FuncInline.enumTeachingPos.Site7] = FuncInline.enumSiteAction.Testing;
                //*/

                Thread.Sleep(GlobalVar.ThreadSleep);
            }

            //DIO.WriteDOData(FuncInline.enumDONames.Y01_0_, false);
            //DIO.WriteDOData(FuncInline.enumDONames.Y01_1_, false);
            //DIO.WriteDOData(FuncInline.enumDONames.Y01_2_Label_Product_Guide_Cylinder, false);
            //DIO.WriteDOData(FuncInline.enumDONames.Y01_3_Label_Pickup_Cylinder, false);


        }

    }
}
