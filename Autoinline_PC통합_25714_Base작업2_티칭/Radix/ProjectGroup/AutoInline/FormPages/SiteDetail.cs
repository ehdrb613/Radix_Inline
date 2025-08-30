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
using System.Diagnostics;
using System.Collections.Concurrent; // ConcurrentQueue

namespace Radix
{

    public partial class SiteDetail : Form
    {
        enum enumSiteAction
        {
            None,
            Init,
            Unclamp,
            Open,
            Close,
            PartClear
        }

        bool siteOnly = true;
        int testPC = 0; // 0이상이면 특정 PC. -1이고 siteOnly false면 전체 사이트
        private enumSiteAction siteAction = enumSiteAction.None;
        FuncInline.enumTeachingPos startPos = FuncInline.DetailSite;
        FuncInline.enumTeachingPos endPos = FuncInline.DetailSite;

        private System.Threading.Timer timerSite; // 사이트 동작
        private bool timerSiteDoing = false; // 사이트 동작 Thread 중복실행 방지

        Stopwatch waitWatch = new Stopwatch();

        public SiteDetail()
        {
            InitializeComponent();
        }

        private void debug(string str)
        {
            //Util.Debug("SiteDetail : " + str);
        }


        private void SiteDetail_Shown(object sender, EventArgs e)
        {
            /*
            int siteIndex = FuncInline.ReverseSiteNo(((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site7));

            lblSiteName.Text = FuncInline.DetailSite.ToString();
            lblSiteAction.Text = FuncInline.SiteAction[(int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site7].ToString();
            lblPCBInfo.Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].PCBStatus.ToString();
            lblSelfRetest.Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].SelfReTest +
                                "(" + FuncInline.PCBInfo[(int)FuncInline.DetailSite].SelfRetestCount + ")";
            lblOtherRetest.Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].OtherReTest.ToString() +
                                "(" + FuncInline.PCBInfo[(int)FuncInline.DetailSite].OtherRetestCount + ")";
            btnUseSite.BackColor = FuncInline.UseSite[siteIndex] ? Color.Lime : Color.White;
            btnNotUseSite.BackColor = !FuncInline.UseSite[siteIndex] ? Color.Lime : Color.White;

            for (int i = 0; i < 12; i++)
            {
                if (FuncInline.PCBInfo[(int)FuncInline.DetailSite].Barcode[i].Length > 0)
                {
                    ((Label)(Controls.Find("lblArray" + (i + 1), true)[0])).Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].Barcode[i] +
                                " , " + FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[i].ToString() +
                                (FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[i] == FuncInline.enumSMDStatus.Before_Command ? 
                                        "(" + FuncInline.PCBInfo[(int)FuncInline.DetailSite].CommandRetry[i] + ")" : "") +
                                (FuncInline.PCBInfo[(int)FuncInline.DetailSite].ErrorCode[i] >= 0 ? 
                                        "(" + FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.DetailSite].ErrorCode[i]] + ")" : "");
                }
            }
            //*/
            //panelWait.Visible = false;
            tmrCheck.Enabled = true;

            TimerCallback CallBackCheck = new TimerCallback(TimerSite);
            timerSite = new System.Threading.Timer(CallBackCheck, false, 0, 100);
        }

        private void btnUseSite_Click(object sender, EventArgs e)
        {
            try
            {
                //FuncInline.enumTeachingPos startPos = FuncInline.DetailSite;
                //FuncInline.enumTeachingPos endPos = FuncInline.DetailSite;
                string msg = FuncInline.DetailSite.ToString();
                if (!siteOnly)
                {
                    msg = "all sites";
                    if (testPC == 0) // 전체 사이트
                    {
                        startPos = FuncInline.enumTeachingPos.Site1_F_DT1;
                        endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.MaxSiteCount - 1;
                    }
                    else
                    {
                        int pcIndex = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / 7;
                        startPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * FuncInline.MaxSiteCount;
                        endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + Math.Min(FuncInline.MaxSiteCount - 1, pcIndex * 7 + (7 - 1));
                        msg = "PC " + (pcIndex + 1);
                    }
                }
                else
                {
                    startPos = FuncInline.DetailSite;
                    endPos = FuncInline.DetailSite;
                }

                //OpenMessage("Use " + msg + " ?");

                //while (!GlobalVar.GlobalStop &&
                //    messageOpen)
                //{
                //    if (messageClose)

                //    Thread.Sleep(GlobalVar.ThreadSleep);
                //}


                if (!FuncWin.MessageBoxOK("Use " + msg + " ?", GlobalVar.ScreenNumbers[0]))
                {
                    return;
                }

                FuncLog.WriteLog("SiteDetail - Use Site " + msg);

                #region 최소 투입카운트 구한다
                /*
                int minInputLeft = 99999;
                int minInputRight = 99999;
                for (int i = 0; i < FuncInline.SiteInputCount.Length; i++)
                {
                    if (FuncInline.UseSite[i] &&
                        i < 20 &&
                        FuncInline.SiteInputCount[i] < minInputLeft)
                    {
                        minInputLeft = FuncInline.SiteInputCount[i];
                    }
                    else if (FuncInline.UseSite[i] &&
                        i >= 20 &&
                        FuncInline.SiteInputCount[i] < minInputRight)
                    {
                        minInputRight = FuncInline.SiteInputCount[i];
                    }
                }
                if (minInputLeft == 99999)
                {
                    minInputLeft = 0;
                }
                if (minInputRight == 99999)
                {
                    minInputRight = 0;
                }
                //*/
                #endregion

                //FuncInline.ResetInputCount();
                for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                {
                    int siteIndex = ((int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1);

                    //msg = "Use " + pos.ToString() + " ?";
                    if (!FuncInline.UseSite[siteIndex])// &&
                                                                 //(!siteOnly || FuncWin.MessageBoxOK(msg)))
                    {
                        // 투입카운트를 사용중인 사이트중 최소값으로 맞춘다.
                        //FuncInline.SiteInputCount[siteIndex] = siteIndex < 20 ? minInputLeft: minInputRight;

                        #region 어레이 NG 카운트 초기화
                        for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                        {
                            FuncInline.ArrayNGCount[siteIndex, i] = 0;
                            FuncInline.ArrayNGCode[siteIndex, i] = -1;
                        }
                        #endregion
                        //UseSite[siteIndex] = true;
                        FuncInline.EnableSite(siteIndex, true);
                        //btnUseSite.BackColor = Color.Lime;
                        //btnNotUseSite.BackColor = Color.White;

                        Func.SaveTestSiteIni();


                    }
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void btnNotUseSite_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = "all sites";
                //FuncInline.enumTeachingPos startPos = FuncInline.DetailSite;
                //FuncInline.enumTeachingPos endPos = FuncInline.DetailSite;
                #region 모듈쪽 소스코드 주석처리
                //string msg = "all sites";
                //if (!siteOnly)
                //{
                //    if (testPC == 0) // 전체 사이트
                //    {
                //        if (FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue ||
                //            FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ||
                //            FuncInline.BuyerChangeOrange[0] ||
                //            FuncInline.BuyerChangeOrange[1] ||
                //            FuncInline.BuyerChangeOrange[2])
                //        {
                //            FuncWin.TopMessageBox("Can't set NotUse at all site while BuyerChange.");
                //            return;
                //        }
                //        startPos = FuncInline.enumTeachingPos.Site1_F_DT1;
                //        endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.MaxSiteCount - 1;
                //    }
                //    else // 지정 PC
                //    {
                //        int pcIndex = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / 7;
                //        if (FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue ||
                //            FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ||
                //            FuncInline.BuyerChangeOrange[pcIndex])
                //        {
                //            FuncWin.TopMessageBox("Can't set NotUse at all site while BuyerChange.");
                //            return;
                //        }
                //        startPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * 7;
                //        endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + Math.Min(FuncInline.MaxSiteCount - 1, pcIndex * 7 + (7 - 1));
                //        msg = "PC " + (pcIndex + 1);
                //    }
                //    if (!FuncWin.MessageBoxOK("NotUse " + msg + " ?"))
                //    {
                //        return;
                //    }
                //}
                //else
                //{
                //    int pcIndex = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / FuncInline.MaxSiteCount;
                //    if (FuncInline.BuyerChangeSite[pcIndex] == FuncInline.DetailSite &&
                //        (FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue ||
                //            FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ||
                //            FuncInline.BuyerChangeOrange[pcIndex]))
                //    {
                //        FuncWin.TopMessageBox("Can't set NotUse at BuyerChange first input site.");
                //        return;
                //    }
                //    msg = FuncInline.DetailSite.ToString();
                //    startPos = FuncInline.DetailSite;
                //    endPos = FuncInline.DetailSite;
                //}

                #region 한칸 이상 비워야 하는데 다 막히는 경우
                /*
                for (FuncInline.enumTeachingPos site = startPos; site <= endPos; site++)
                {
                    int orangeIndex = ((int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / 7;
                    int rackEmpty = FuncInline.GetEmptySiteCount((int)site - (int)FuncInline.enumTeachingPos.Site1_F_DT1);
                    if (FuncInline.BuyerChange == FuncInline.enumBuyerChange.Blue ||
                        FuncInline.BuyerChange == FuncInline.enumBuyerChange.Orange ||
                        FuncInline.BuyerChangeOrange[orangeIndex])
                    {
                        if (rackEmpty <= 2)
                        {
                            FuncWin.TopMessageBox("Can't set " + site.ToString() + " NotUse. Two or more site must be empty while BuyerChange.");
                            return;
                        }
                    }
                    if (FuncInline.BuyerChange == FuncInline.enumBuyerChange.White && FuncInline.LeaveOneSite)
                    {
                        if (rackEmpty <= 1)
                        {
                            FuncWin.TopMessageBox("Can't set " + site.ToString() + " NotUse. One or more site must be empty.");
                            return;
                        }
                    }
                }*/
                #endregion
                #endregion


                msg = FuncInline.SiteDisplay.GetSiteDisplayName(FuncInline.DetailSite);
                FuncLog.WriteLog("SiteDetail - NotUse Site " + msg);

                for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                {
                    int siteIndex = ((int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1);
                    msg = "NotUse " + pos.ToString() + " ?";
                    if (FuncInline.UseSite[siteIndex] &&
                        (!siteOnly || FuncWin.MessageBoxOK(msg)))
                    {
                        FuncInline.UseSite[siteIndex] = false;
                        FuncInline.BlockReason[siteIndex] = "User Block - Site Detail";
                        btnUseSite.BackColor = Color.White;
                        btnNotUseSite.BackColor = Color.Lime;
                        FuncInline.InsertBlockHistory(siteIndex + 1, false, "User Block - Site Detail");
                        Func.SaveTestSiteIni();
                    }
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void btnPartClear_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun ||
                    GlobalVar.SystemStatus == enumSystemStatus.ErrorStop)
                {
                    if (FuncInline.DetailSite >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                        FuncInline.DetailSite < FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.MaxSiteCount - 1 &&
                        siteAction != enumSiteAction.None)
                    {
                        FuncWin.TopMessageBox(FuncInline.SiteAction.ToString() + " is already excuting. Wait a seconds!");
                        return;
                    }

                    //FuncInline.enumTeachingPos startPos = FuncInline.DetailSite;
                    //FuncInline.enumTeachingPos endPos = FuncInline.DetailSite;
                    string msg = FuncInline.DetailSite.ToString();
                    if (FuncInline.DetailSite >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                        FuncInline.DetailSite < FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.MaxSiteCount - 1 &&
                        !siteOnly)
                    {
                        if (testPC == 0) // 전체 사이트
                        {
                            startPos = FuncInline.enumTeachingPos.Site1_F_DT1;
                            endPos = FuncInline.enumTeachingPos.Site13_F_FT3;
                            msg = "all sites";
                        }
                        else
                        {
                            int pcIndex = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / 7;
                            startPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * 7;
                            endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * 7 + (7 - 1);
                            msg = "PC " + (pcIndex + 1);
                        }
                    }
                    else
                    {
                        startPos = FuncInline.DetailSite;
                        endPos = FuncInline.DetailSite;
                        msg = startPos.ToString();
                    }
                    for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                    {
                        if (pos >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                            pos < FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.MaxSiteCount)
                        {
                            // ── 테스트 사이트: SiteIoMaps로 ‘PCB Dock’ DI만 확인 (필요 센서만 사용)
                            if (!GlobalVar.Simulation &&
                                FuncInline.SiteIoMaps.TryGetPcbDockDI(pos, out var dockDi) &&
                                DIO.GetDIData(dockDi))
                            {
                                // 표시명은 세대별 라벨 변환 사용(있으면)
                                string label = FuncInline.SiteDisplay.GetSiteDisplayName(pos);
                                             
                                FuncWin.TopMessageBox(label + " can not be cleared. Remove PCB in " + label);
                                return;
                            }
                        }
                        else
                        {
                            switch (FuncInline.DetailSite)
                            {
                                // ───────── InConveyor ─────────
                                case FuncInline.enumTeachingPos.InConveyor:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X302_0_In_Shuttle_Pcb_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── InShuttle ─────────
                                case FuncInline.enumTeachingPos.InShuttle:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X303_4_In_Shuttle_Pcb_Interlock_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X302_1_In_Shuttle_Pcb_Stop_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X304_0_In_Shuttle_Interlock_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── FrontPassLine ─────────
                                case FuncInline.enumTeachingPos.FrontPassLine:
                                    // 대표 점유 DI 하나로 단순화(Stop 센서)
                                    if (DIO.GetDIData(FuncInline.enumDINames.X114_6_Front_PASSLINE_PCB_Stop_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── RearPassLine (OK Line) ─────────
                                case FuncInline.enumTeachingPos.RearPassLine:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X405_0_Rear_Pass_OkLine_PCB_In_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── RearNGLine ─────────
                                case FuncInline.enumTeachingPos.RearNGLine:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X406_4_Rear_Pass_NgLine_PCB_Stop_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── Lift1_Up ─────────
                                case FuncInline.enumTeachingPos.Lift1_Up:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X403_2_Front_Lift_Up_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X403_5_Front_Lift_Up_PCB_Stop_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── Lift1_Down ─────────
                                case FuncInline.enumTeachingPos.Lift1_Down:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X400_0_Front_Lift_Down_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X400_2_Front_Lift_Down_PCB_Stop_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── Lift2_Up ─────────
                                case FuncInline.enumTeachingPos.Lift2_Up:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X404_6_Rear_Lift_Up_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X405_1_Rear_Lift_Up_PCB_Stop_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── Lift2_Down ─────────
                                case FuncInline.enumTeachingPos.Lift2_Down:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X405_5_Rear_Lift_Down_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X405_7_Rear_Lift_Down_PCB_Stop_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── OutConveyor ─────────
                                case FuncInline.enumTeachingPos.OutConveyor:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X02_3_Out_Conveyor_PASSLIne_PCB_Start_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X02_4_Out_Conveyor_PASSLine_PCB_Stop_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── OutShuttle_Up (OK) ─────────
                                case FuncInline.enumTeachingPos.OutShuttle_Up:
                                    // OK 라인은 전용 In/Stop DI가 부족 → 인터락/스토퍼 IN 센서로 점유 대체 판단
                                    if (DIO.GetDIData(FuncInline.enumDINames.X304_1_Out_Shuttle_Ok_Interlock_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X303_5_Out_Shuttle_Stopper_Cyl_IN_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── OutShuttle_Down (NG) ─────────
                                case FuncInline.enumTeachingPos.OutShuttle_Down:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X402_0_Out_Shuttle_Ng_PCB_In_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X04_2_Out_Shuttle_NG_PCB_Stop_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── NgBuffer ─────────
                                case FuncInline.enumTeachingPos.NgBuffer:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X304_2_Out_Shuttle_Ng_Interlock_Sensor) ||
                                        DIO.GetDIData(FuncInline.enumDINames.X03_7_NgBuffer_PCB_Stop_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                                // ───────── FrontScanSite ─────────
                                case FuncInline.enumTeachingPos.FrontScanSite:
                                    if (DIO.GetDIData(FuncInline.enumDINames.X03_4_Front_Scan_PCB_Dock_Sensor))
                                    {
                                        FuncWin.TopMessageBox(FuncInline.DetailSite + " can not be cleared. Remove PCB in " + FuncInline.DetailSite);
                                        return;
                                    }
                                    break;

                            }
                        }
                    }

                    if (!FuncWin.MessageBoxOK("Clear Part at " + msg + " ?"))
                    {
                        return;
                    }

                    FuncLog.WriteLog("SiteDetail - PartClear " + msg);

                    ConcurrentQueue<int> clearQueue = new ConcurrentQueue<int>(); // 오토닉스 모션 지령 저장
                    for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                    {
                        FuncInline.SiteMessage = "Clearing Part " + pos.ToString();
                        int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                        clearQueue.Enqueue(siteIndex);
                        FuncInline.PartClear(pos);
                        if (pos >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                            pos <= FuncInline.enumTeachingPos.Site26_R_FT3)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y08_1_Site1_CCW_Conveyor_Run + siteIndex * GlobalVar.DOModuleGap, true);
                            FuncInline.SiteAction[siteIndex] = FuncInline.enumSiteAction.Waiting;
                        }
                        else if (pos == FuncInline.enumTeachingPos.InConveyor)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y03_0_In_Conveyor_Conveyor_Run, true);
                            FuncInline.InConveyorAction = FuncInline.enumLiftAction.Waiting;
                        }
                        else if (pos == FuncInline.enumTeachingPos.InShuttle)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y03_0_In_Conveyor_Conveyor_Run, true);
                            FuncInline.InShuttleAction = FuncInline.enumShuttleAction.Waiting;
                        }
                        else if (pos == FuncInline.enumTeachingPos.OutShuttle_Up)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y03_0_In_Conveyor_Conveyor_Run, true);
                            FuncInline.OutShuttleUpAction = FuncInline.enumShuttleAction.Waiting;
                        }
                        else if (pos == FuncInline.enumTeachingPos.OutShuttle_Down)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y03_0_In_Conveyor_Conveyor_Run, true);
                            FuncInline.OutShuttleDownAction = FuncInline.enumShuttleAction.Waiting;
                        }
                        else if (pos == FuncInline.enumTeachingPos.FrontPassLine)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y03_4_Rack1_PassLine_Conveyor_Run, true);
                            FuncInline.FrontPassLineAction = FuncInline.enumLiftAction.Waiting;
                        }
                        else if (pos == FuncInline.enumTeachingPos.Lift1_Up)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y04_0_Rack1_UpLift_CW_Conveyor_Run, true);
                            DIO.WriteDOData(FuncInline.enumDONames.Y04_1_Rack1_UpLift_CCW_Conveyor_Run, false);
                            if (FuncInline.Lift1Action.ToString().Contains("Up"))
                            {
                                FuncInline.Lift1Action = FuncInline.enumLiftAction.Waiting;
                            }
                        }
                      
                        else if (pos == FuncInline.enumTeachingPos.RearPassLine)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y03_6_Rack2_PassLine_Conveyor_Run, true);
                            FuncInline.RearPassLineAction = FuncInline.enumLiftAction.Waiting;
                        }
                        else if (pos == FuncInline.enumTeachingPos.RearNGLine)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y03_6_Rack2_PassLine_Conveyor_Run, true);
                            FuncInline.RearNGLineAction = FuncInline.enumLiftAction.Waiting;
                        }
                        else if (pos == FuncInline.enumTeachingPos.Lift2_Up)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y05_0_Rack2_UpLift_CW_Conveyor_Run, true);
                            DIO.WriteDOData(FuncInline.enumDONames.Y05_1_Rack2_UpLift_CCW_Conveyor_Run, false);
                            if (FuncInline.Lift2Action.ToString().Contains("Up"))
                            {
                                FuncInline.Lift2Action = FuncInline.enumLiftAction.Waiting;
                            }
                        }
                     
                        else if (pos == FuncInline.enumTeachingPos.OutConveyor)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y06_0_Out_Conveyor_Conveyor_Run, true);
                            if (!FuncInline.OutConveyorAction.ToString().Contains("NG"))
                            {
                                FuncInline.OutConveyorAction = FuncInline.enumLiftAction.Waiting;
                            }
                        }
                        else if (pos == FuncInline.enumTeachingPos.NgBuffer)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y06_3_NG_Buffer_Conveyor_Run, true);
                            if (FuncInline.NGBufferAction.ToString().Contains("NG"))
                            {
                                FuncInline.NGBufferAction = FuncInline.enumLiftAction.Waiting;
                            }
                            //FuncInline.NGOut = false;
                        }
                    }
                    Stopwatch watch = new Stopwatch();
                    Util.StartWatch(ref watch);
                    while (watch.ElapsedMilliseconds < 2000)
                    {
                        Util.USleep(GlobalVar.ThreadSleep);
                    }
                    while (clearQueue.Count > 0)
                    {
                        int siteIndex = -1;
                        if (clearQueue.TryDequeue(out siteIndex) &&
                            siteIndex >= 0 &&
                            siteIndex < FuncInline.MaxSiteCount)
                        {
                            //by DG 수정필요
                            DIO.WriteDOData(FuncInline.enumDONames.Y402_1_Front_FT_1_Motor_Cw , false);
                            DIO.WriteDOData(FuncInline.enumDONames.Y402_3_Front_FT_1_Motor_Ccw, false);
                        }

                    }
                    for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                    {
                        FuncInline.SiteMessage = "Clearing Part " + pos.ToString();
                        if (pos == FuncInline.enumTeachingPos.InShuttle)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y305_0_In_Conveyor_In_Shuttle_Conveyor_Cw, true);
                            DIO.WriteDOData(FuncInline.enumDONames.Y400_3_In_Shuttle_Motor_Ccw, true);
                        }
                        else if (pos == FuncInline.enumTeachingPos.NgBuffer)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y402_5_Out_Conveyor_Ng_Motor_Cw, true);
                        }
                        else if (pos == FuncInline.enumTeachingPos.OutShuttle_Up)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y304_0_Out_Shuttle_Ok_Motor_Cw, true);
                            DIO.WriteDOData(FuncInline.enumDONames.Y402_7_Out_Shuttle_Ok_Motor_Ccw, false);
                        }
                        else if (pos == FuncInline.enumTeachingPos.FrontPassLine)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y05_5_Passline1_Conveyor_Forward, true);
                        }
                        else if (pos == FuncInline.enumTeachingPos.RearPassLine)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y06_1_Passline2_Conveyor_Forward, true);
                        }
                        else if (pos == FuncInline.enumTeachingPos.Passline3)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y06_5_Passline3_Conveyor_Forward, true);
                        }
                        else if (pos == FuncInline.enumTeachingPos.Lift1_Up)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y405_0_Front_Lift_Up_Motor_Cw, true);
                            DIO.WriteDOData(FuncInline.enumDONames.Y405_2_Front_Lift_Up_Motor_Ccw, false);
                        }
                        else if (pos == FuncInline.enumTeachingPos.Lift1_Down)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y405_4_Front_Lift_Down_Motor_Cw, true);
                            DIO.WriteDOData(FuncInline.enumDONames.Y405_6_Front_Lift_Down_Motor_Ccw, false);
                        }
                        else if (pos == FuncInline.enumTeachingPos.Lift2_Up)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y305_2_Rear_Lift_Up_Motor_Cw, true);
                            DIO.WriteDOData(FuncInline.enumDONames.Y304_2_Rear_Lift_Up_Motor_Ccw, false);
                        }
                        else if (pos == FuncInline.enumTeachingPos.Lift2_Down)
                        {
                            DIO.WriteDOData(FuncInline.enumDONames.Y305_1_Rear_Lift_Down_Motor_Cw, true);
                            DIO.WriteDOData(FuncInline.enumDONames.Y304_1_Rear_Lift_Down_Motor_Ccw, false);
                        }
                   
                    }
                    FuncInline.SiteMessage = "";
                }
                else
                {
                    FuncWin.TopMessageBox("Can't clear while system is running.");
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void btnStartTest_Click(object sender, EventArgs e)
        {
            try
            {
                //FuncInline.enumTeachingPos startPos = FuncInline.DetailSite;
                //FuncInline.enumTeachingPos endPos = FuncInline.DetailSite;
                string msg = FuncInline.DetailSite.ToString();
                if (!siteOnly)
                {
                    if (testPC == 0) // 전체 사이트
                    {
                        startPos = FuncInline.enumTeachingPos.Site1_F_DT1;
                        endPos = FuncInline.enumTeachingPos.Site13_F_FT3;
                        msg = "all sites";
                    }
                    else
                    {
                        int pcIndex = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / FuncInline.MaxSiteCount;
                        startPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * FuncInline.MaxSiteCount;
                        endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * FuncInline.MaxSiteCount + FuncInline.MaxSiteCount - 1;
                        msg = "PC " + (pcIndex + 1);
                    }
                }
                else
                {
                    startPos = FuncInline.DetailSite;
                    endPos = FuncInline.DetailSite;
                    msg = startPos.ToString();
                }

                #region PCB 감지시 동작 금지
                for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                {
                    int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                    if (FuncInline.PCBInfo[(int)pos].PCBStatus >= FuncInline.enumSMDStatus.Command_Sent)
                    {
                        FuncWin.TopMessageBox(pos.ToString() + " already testing. Can't start test.");
                        return;
                    }
                }
                #endregion

                if (!FuncWin.MessageBoxOK("Start Test at " + msg + " ?"))
                {
                    return;
                }

                FuncLog.WriteLog("SiteDetail - Start Test " + msg);

                bool exist = false;
                for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                {
                    int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                    int siteNo = siteIndex + 1;
                    // PCB 감지되고 테스팅중 아니면 지령 보낸다.
                    if (FuncInline.GetDISiteData(siteIndex, FuncInline.enumDISite.X1_Site_PCB_Stop_Sensor) &&
                        FuncInline.PCBInfo[(int)pos].PCBStatus >= FuncInline.enumSMDStatus.Before_Command &&
                        FuncInline.PCBInfo[(int)pos].PCBStatus <= FuncInline.enumSMDStatus.Before_Command)
                    {
                        exist = true;
                        FuncInline.StartTest(siteNo);
                    }
                }
                if (!exist)
                {
                    FuncWin.TopMessageBox("No PCB to start test.");
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void btnStopTest_Click(object sender, EventArgs e)
        {
            try
            {
                //FuncInline.enumTeachingPos startPos = FuncInline.DetailSite;
                //FuncInline.enumTeachingPos endPos = FuncInline.DetailSite;
                string msg = FuncInline.DetailSite.ToString();
                if (!siteOnly)
                {
                    if (testPC == 0) // 전체 사이트
                    {
                        startPos = FuncInline.enumTeachingPos.Site1_F_DT1;
                        endPos = FuncInline.enumTeachingPos.Site13_F_FT3;
                        msg = "all sites";
                    }
                    else
                    {
                        int pcIndex = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / FuncInline.MaxSiteCount;
                        startPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * FuncInline.MaxSiteCount;
                        endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * FuncInline.MaxSiteCount + FuncInline.MaxSiteCount - 1;
                        msg = "PC " + (pcIndex + 1);
                    }
                }
                else
                {
                    startPos = FuncInline.DetailSite;
                    endPos = FuncInline.DetailSite;
                    msg = startPos.ToString();
                }
                if (!FuncWin.MessageBoxOK("Stop Test at " + msg + " ?"))
                {
                    return;
                }

                FuncLog.WriteLog("SiteDetail - Stop Test " + msg);

                bool exist = false;
                for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                {
                    int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                    int siteNo = siteIndex + 1;
                    if (FuncInline.GetDISiteData(siteIndex, FuncInline.enumDISite.X1_Site_PCB_Stop_Sensor))
                    {
                        exist = true;
                        FuncInline.StopTest(siteNo);
                    }
                }
                if (!exist)
                {
                    FuncWin.TopMessageBox("No PCB to stop test.");
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SiteDetail_FormClosed(object sender, FormClosedEventArgs e)
        {
            tmrCheck.Enabled = false;
            try
            {
                //timerSite.Dispose();
            }
            catch { }
        }

        private void tmrCheck_Tick(object sender, EventArgs e)
        {
            //tmrCheck.Enabled = false;
            try
            {
                /*
                Util.ResetWatch(ref waitWatch);
                Util.StartWatch(ref waitWatch);

                lblSiteName.Text = FuncInline.DetailSite.ToString();
                lblSiteAction.Text = FuncInline.SiteAction[(int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1].ToString();
                lblPCBInfo.Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].PCBStatus.ToString();
                lblSelfRetest.Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].SelfReTest +
                                    "(" + FuncInline.PCBInfo[(int)FuncInline.DetailSite].SelfRetestCount + ")";
                lblOtherRetest.Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].OtherReTest.ToString() +
                                    "(" + FuncInline.PCBInfo[(int)FuncInline.DetailSite].OtherRetestCount + ")";

                int siteIndex = (int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                int pcNUm = (siteIndex / FuncInline.MaxSiteCount) + 1;

                btnSiteOnly.Text = FuncInline.DetailSite.ToString() + " Only";
                btnTestPC.Text = "Test PC " + pcNUm;
                FuncForm.SetButtonColor2(btnSiteOnly, siteOnly);
                FuncForm.SetButtonColor2(btnTestPC, !siteOnly && testPC > 0);
                FuncForm.SetButtonColor2(btnAllSites, !siteOnly && testPC == 0);

                #region 사이트 하나 선택시
                if (siteOnly)
                {
                    FuncForm.SetButtonColor2(btnUseSite, FuncInline.UseSite[siteIndex]);
                    FuncForm.SetButtonColor2(btnNotUseSite, !FuncInline.UseSite[siteIndex]);
                    btnPartClear.BackColor = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex] ? Color.Tomato : Color.White;

                }
                #endregion
                #region 사이트 하나 아닌 경우
                else
                {
                    #region 검색 범위
                    //FuncInline.enumTeachingPos startPos = FuncInline.DetailSite;
                    //FuncInline.enumTeachingPos endPos = FuncInline.DetailSite;
                    if (!siteOnly)
                    {
                        if (testPC == 0) // 전체 사이트
                        {
                            startPos = FuncInline.enumTeachingPos.Site1_F_DT1;
                            endPos = FuncInline.enumTeachingPos.Site13_F_FT3;
                        }
                        else
                        {
                            int pcIndex = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / FuncInline.MaxSiteCount;
                            startPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * FuncInline.MaxSiteCount;
                            endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * FuncInline.MaxSiteCount + FuncInline.MaxSiteCount - 1;
                        }
                    }
                    #endregion

                    bool useTrue = true;
                    bool useFalse = true;
                    bool homeTrue = true;
                    bool homeFalse = true;
                    bool unclampTrue = true;
                    bool unclampFalse = true;
                    bool openTrue = true;
                    bool openFalse = true;
                    bool closeTrue = true;
                    bool closeFalse = true;
                    bool partClearTrue = true;
                    bool partClearFalse = true;

                    for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                    {
                        int subIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;

                            #region 사용/미사용
                            if (FuncInline.UseSite[subIndex])
                            {
                                useFalse = false;
                            }
                            else
                            {
                                useTrue = false;
                            }
                            #endregion
                            #region partClear
                            if (FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + subIndex])
                            {
                                partClearFalse = false;
                            }
                            else
                            {
                                partClearTrue = false;
                            }
                            #endregion
                    }


                    FuncForm.SetButtonColor3(btnUseSite, useTrue, useFalse);
                    FuncForm.SetButtonColor3(btnNotUseSite, useFalse, useTrue);
                    btnPartClear.BackColor = partClearTrue ? Color.Tomato :
                                             partClearFalse ? Color.White : Color.Yellow;
                }
                #endregion


                int checkCount = 0;
                bool started = true; // 모두 testing이면
                bool stoped = true; // 모두 testing 초과면
                for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                {
                    try
                    {
                        if (FuncInline.PCBInfo[(int)FuncInline.DetailSite].Barcode[i].Length > 0)
                        {
                            ((Label)(Controls.Find("lblArray" + (i + 1), true)[0])).Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].Barcode[i] +
                                        " , " + FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[i].ToString() +
                                        (FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[i] == FuncInline.enumSMDStatus.Before_Command ?
                                                "(" + FuncInline.PCBInfo[(int)FuncInline.DetailSite].CommandRetry[i] + ")" : "") +
                                        (FuncInline.PCBInfo[(int)FuncInline.DetailSite].ErrorCode[i] >= 0 ?
                                                "(" + FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.DetailSite].ErrorCode[i]] + ")" : "");
                            if (FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[i] != FuncInline.enumSMDStatus.Testing)
                            {
                                started = false;
                            }
                            if (FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[i] <= FuncInline.enumSMDStatus.Testing)
                            {
                                stoped = false;
                            }
                            checkCount++;
                        }
                    }
                    catch { }
                }
                FuncForm.SetButtonColor2(btnStartTest, checkCount > 0 && started);
                FuncForm.SetButtonColor2(btnStopTest, checkCount > 0 && stoped);


                #region 대기 팝업 컨트롤

                if (FuncInline.SiteMessage.Length > 0)
                {
                }
                else
                {
                    panelWait.Visible = false;
                }
                //debug("timer : " + waitWatch.ElapsedMilliseconds);
                #endregion

                //*/

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

            //if (!GlobalVar.GlobalStop)
            //{
            //    Thread.Sleep(GlobalVar.ThreadSleep);
            //    tmrCheck.Enabled = true;
            //}
        }

        private void btnSiteOnly_Click(object sender, EventArgs e)
        {
            siteOnly = true;
            testPC = 0;
        }

        private void btnTestPC_Click(object sender, EventArgs e)
        {
            try
            {
                siteOnly = false;
                int siteNo = int.Parse(FuncInline.DetailSite.ToString().Replace("Site", ""));
                int siteIndex = siteNo - 1;
                testPC = (siteIndex / FuncInline.MaxSiteCount) + 1;
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void btnAllSites_Click(object sender, EventArgs e)
        {
            siteOnly = false;
            testPC = 0;
        }

        private void TimerSite(Object state)
        {
            try
            {
                if (timerSiteDoing)
                {
                    return;
                }
                timerSiteDoing = true;


                if (!GlobalVar.GlobalStop &&
                    this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        #region 공통
                        /*
                        if (FuncInline.DetailSite >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                            FuncInline.DetailSite <= FuncInline.enumTeachingPos.Site13_F_FT3)
                        {
                            if (FuncInline.UseSite[(int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1])
                            {
                                lblBlockReason.Visible = false;
                            }
                            else
                            {
                                lblBlockReason.Visible = true;
                                lblBlockReason.Text = "Block Reason : " + FuncInline.BlockReason[(int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1];
                            }
                        }
                        else
                        {
                            lblBlockReason.Visible = false;
                        }
                        //*/

                        btnBlockHistory.Visible = FuncInline.DetailSite >= FuncInline.enumTeachingPos.Site1_F_DT1 && 
                                                    FuncInline.DetailSite < FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.MaxSiteCount;

                        if (GlobalVar.Simulation)
                        {
                            lblDestination.Visible = true;
                            lblPassed.Visible = true;
                            lblNG.Visible = true;
                            lblDestination.Text = "Destination : " + FuncInline.PCBInfo[(int)FuncInline.DetailSite].Destination;
                            lblPassed.Text = FuncInline.CheckPassedPCB(FuncInline.DetailSite).ToString();
                            lblNG.Text = FuncInline.CheckNGPCB(FuncInline.DetailSite).ToString();
                        }
                        else
                        {
                            lblDestination.Visible = false;
                            lblPassed.Visible = false;
                            lblNG.Visible = false;
                        }

                        lblSiteName.Text = FuncInline.DetailSite.ToString();
                        lblPCBInfo.Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].PCBStatus.ToString();
                        string testSites = "";
                        for (int i = FuncInline.PCBInfo[(int)FuncInline.DetailSite].TestSite.Length - 1; i >= 0; i--)
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.DetailSite].TestSite[i] > 0)
                            {
                                testSites += (testSites.Length > 0 ? "," : "") + FuncInline.PCBInfo[(int)FuncInline.DetailSite].TestSite[i];
                            }
                        }
                        lblTestSite.Text = testSites.Length > 0 ? "(" + (testSites.Length > 0 ? "Site" + testSites : "") + ")" : "-";
                        if (FuncInline.SelfRetest)
                        {
                            lblSelfRetest.Visible = true;
                            lblSelfRetestTitle.Visible = true;
                            lblSelfRetest.Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].SelfReTest +
                                                "(" + FuncInline.PCBInfo[(int)FuncInline.DetailSite].SelfRetestCount + ")";
                        }
                        else
                        {
                            lblSelfRetest.Visible = false;
                            lblSelfRetestTitle.Visible = false;
                        }
                        if (FuncInline.OtherRetest)
                        {
                            lblOtherRetest.Visible = true;
                            lblOtherRetestTitle.Visible = true;
                            lblOtherRetest.Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].OtherReTest.ToString();
                        }
                        else
                        {
                            lblOtherRetest.Visible = false;
                            lblOtherRetestTitle.Visible = false;
                        }
                        int checkCount = 0;
                        bool started = true; // 모두 testing이면
                        bool stoped = true; // 모두 testing 초과면

                        foreach (Control conSite in this.Controls) // 사이트 묶음 전체
                        {
                            if (conSite.Name.Contains("lblArray"))
                            {
                                //debug("Main : " + conSite.Name);
                                int array = 0;
                                int.TryParse(conSite.Name.Replace("lblArray", ""), out array);
                                if (array < 1 ||
                                    array > FuncInline.MaxArrayCount)
                                {
                                    continue;
                                }
                                int arrayIndex = array - 1;

                                ((Label)conSite).Text = FuncInline.PCBInfo[(int)FuncInline.DetailSite].Barcode[arrayIndex] +
                                            " , " + 
                                            (FuncInline.PCBInfo[(int)FuncInline.DetailSite].BadMark[arrayIndex] ? "BadMark" :
                                                    FuncInline.PCBInfo[(int)FuncInline.DetailSite].Xout[arrayIndex] ? "XOut" :   
                                                    FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[arrayIndex].ToString()) +
                                            (FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[arrayIndex] == FuncInline.enumSMDStatus.Before_Command ?
                                                    "(" + FuncInline.PCBInfo[(int)FuncInline.DetailSite].CommandRetry[arrayIndex] + ")" : "") +
                                            (FuncInline.PCBInfo[(int)FuncInline.DetailSite].ErrorCode[arrayIndex] >= 0 ?
                                                    "(" + FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.DetailSite].ErrorCode[arrayIndex]] + ")" : "") +
                                            (FuncInline.PCBInfo[(int)FuncInline.DetailSite].BeforeCode[arrayIndex] >= 0 ?
                                                    "(" + FuncInline.TestErrorCode[FuncInline.PCBInfo[(int)FuncInline.DetailSite].BeforeCode[arrayIndex]] + ")" : "");
                                if (FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[arrayIndex] != FuncInline.enumSMDStatus.Testing)
                                {
                                    started = false;
                                }
                                if (FuncInline.PCBInfo[(int)FuncInline.DetailSite].SMDStatus[arrayIndex] <= FuncInline.enumSMDStatus.Testing)
                                {
                                    stoped = false;
                                }
                                checkCount++;
                            }
                        }


                        FuncForm.SetButtonColor2(btnStartTest, checkCount > 0 && started);
                        FuncForm.SetButtonColor2(btnStopTest, checkCount > 0 && stoped);
                        #endregion

                        #region 사이트 경우
                        if (FuncInline.DetailSite >= FuncInline.enumTeachingPos.Site1_F_DT1 &&
                            FuncInline.DetailSite < FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.MaxSiteCount)
                        {
                            btnUseSite.Visible = true;
                            btnNotUseSite.Visible = true;
                            btnStartTest.Visible = true;
                            btnStopTest.Visible = true;
                            btnSiteOnly.Visible = true;
                            btnTestPC.Visible = true;
                            btnAllSites.Visible = true;
                            btnUnclamp.Visible = true;

                            lblSiteAction.Text = FuncInline.SiteAction[(int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1].ToString();

                            int siteIndex = (int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                            int pcNUm = (siteIndex / FuncInline.SitePerRack) + 1;

                            btnSiteOnly.Text = FuncInline.DetailSite.ToString() + " Only";
                            btnTestPC.Text = "Test PC " + pcNUm;
                            FuncForm.SetButtonColor2(btnSiteOnly, siteOnly);
                            FuncForm.SetButtonColor2(btnTestPC, !siteOnly && testPC > 0);
                            FuncForm.SetButtonColor2(btnAllSites, !siteOnly && testPC == 0);

                            #region 사이트 하나 선택시
                            if (siteOnly)
                            {
                                FuncForm.SetButtonColor2(btnUseSite, FuncInline.UseSite[siteIndex]);
                                FuncForm.SetButtonColor2(btnNotUseSite, !FuncInline.UseSite[siteIndex]);
                                btnPartClear.BackColor = FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + siteIndex] ? Color.Tomato : Color.White;
                                FuncForm.SetButtonColor2(btnUnclamp, FuncInline.CheckSiteClamp((FuncInline.enumTeachingPos)siteIndex));
                            }
                            #endregion
                            #region 사이트 하나 아닌 경우
                            else
                            {
                                #region 검색 범위
                                //FuncInline.enumTeachingPos startPos = FuncInline.DetailSite;
                                //FuncInline.enumTeachingPos endPos = FuncInline.DetailSite;
                                if (!siteOnly)
                                {
                                    if (testPC == 0) // 전체 사이트
                                    {
                                        startPos = FuncInline.enumTeachingPos.Site1_F_DT1;
                                        endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.MaxSiteCount - 1;
                                    }
                                    else
                                    {
                                        int pcIndex = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / FuncInline.MaxSiteCount;
                                        startPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * FuncInline.MaxSiteCount;
                                        endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * FuncInline.MaxSiteCount + FuncInline.MaxSiteCount - 1;
                                    }
                                }
                                #endregion

                                bool useTrue = true;
                                bool useFalse = true;
                                bool partClearTrue = true;
                                bool partClearFalse = true;
                                bool unclampTrue = true;
                                bool unclampFalse = true;

                                for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                                {
                                    int subIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;

                                    #region 사용/미사용
                                    if (FuncInline.UseSite[subIndex])
                                    {
                                        useFalse = false;
                                    }
                                    else
                                    {
                                        useTrue = false;
                                    }
                                    #endregion
                                    #region partClear
                                    if (FuncInline.NeedPartClear[(int)FuncInline.enumErrorPart.Site1_F_DT1 + subIndex])
                                    {
                                        partClearFalse = false;
                                    }
                                    else
                                    {
                                        partClearTrue = false;
                                    }
                                    #endregion
                                    #region unclamp
                                    if (FuncInline.CheckSiteClamp((FuncInline.enumTeachingPos)subIndex))
                                    {
                                        unclampFalse = false;
                                    }
                                    else
                                    {
                                        unclampTrue = false;
                                    }
                                    #endregion
                                }


                                FuncForm.SetButtonColor3(btnUseSite, useTrue, useFalse);
                                FuncForm.SetButtonColor3(btnNotUseSite, useFalse, useTrue);
                                btnPartClear.BackColor = partClearTrue ? Color.Tomato :
                                                         partClearFalse ? Color.White : Color.Yellow;
                                FuncForm.SetButtonColor3(btnUnclamp, unclampTrue, unclampFalse);
                            }
                            #endregion

                        }
                        #endregion

                        #region 사이트 이외
                        else
                        {
                            btnUseSite.Visible = false;
                            btnNotUseSite.Visible = false;
                            btnStartTest.Visible = false;
                            btnStopTest.Visible = false;
                            btnSiteOnly.Visible = false;
                            btnTestPC.Visible = false;
                            btnAllSites.Visible = false;
                            btnUnclamp.Visible = false;

                            switch (FuncInline.DetailSite)
                            {
                                // 액션 정의 필요
                                /*
                                case FuncInline.enumTeachingPos.InShuttle:
                                    lblSiteAction.Text = FuncInline.InShuttleAction.ToString();
                                    break;
                                case FuncInline.enumTeachingPos.FrontPassline:
                                    lblSiteAction.Text = FuncInline.Passline1Action.ToString();
                                    break;
                                case FuncInline.enumTeachingPos.Lift1_Up:
                                    lblSiteAction.Text = FuncInline.Lift1Action.ToString().Contains("Up") ? FuncInline.Lift1Action.ToString() : "None";
                                    break;
                                case FuncInline.enumTeachingPos.Lift1_Down:
                                    lblSiteAction.Text = FuncInline.Lift1Action.ToString().Contains("Down") ? FuncInline.Lift1Action.ToString() : "None";
                                    break;
                                case FuncInline.enumTeachingPos.RearPassline:
                                    lblSiteAction.Text = FuncInline.Passline2Action.ToString();
                                    break;
                                case FuncInline.enumTeachingPos.Lift2_Up:
                                    lblSiteAction.Text = FuncInline.Lift2Action.ToString().Contains("Up") ? FuncInline.Lift2Action.ToString() : "None";
                                    break;
                                case FuncInline.enumTeachingPos.Lift2_Down:
                                    lblSiteAction.Text = FuncInline.Lift2Action.ToString().Contains("Down") ? FuncInline.Lift2Action.ToString() : "None";
                                    break;
                                case FuncInline.enumTeachingPos.OutShuttle:
                                    lblSiteAction.Text = !FuncInline.OutShuttleAction.ToString().Contains("NG") ? FuncInline.OutShuttleAction.ToString() : "None";
                                    break;
                                case FuncInline.enumTeachingPos.NgBuffer:
                                    lblSiteAction.Text = FuncInline.OutShuttleAction.ToString().Contains("NG") ? FuncInline.OutShuttleAction.ToString() : "None";
                                    break;
                                //*/
                            }
                        }
                        #endregion

                    }));
                }

                if (siteAction != enumSiteAction.None)
                {
                    Stopwatch siteWatch = new Stopwatch();
                    Util.StartWatch(ref siteWatch);
                    switch (siteAction)
                    {
                        case enumSiteAction.PartClear:
                            ConcurrentQueue<int> clearQueue = new ConcurrentQueue<int>(); // 오토닉스 모션 지령 저장
                            for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                            {
                                FuncInline.SiteMessage = "Clearing Part " + pos.ToString();
                                int siteIndex = (int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1;
                                clearQueue.Enqueue(siteIndex);
                                FuncInline.PartClear(pos);
                                FuncInline.WriteDOSiteData(siteIndex, FuncInline.enumDOSite.Y1_Conveyor_Backward, true);
                            }
                            Stopwatch watch = new Stopwatch();
                            Util.StartWatch(ref watch);
                            while (watch.ElapsedMilliseconds < 2000)
                            {
                                Util.USleep(GlobalVar.ThreadSleep);
                            }
                            while (clearQueue.Count > 0)
                            {
                                int siteIndex = -1;
                                if (clearQueue.TryDequeue(out siteIndex) &&
                                    siteIndex > -1 &&
                                    siteIndex < FuncInline.MaxSiteCount)
                                {
                                    FuncInline.WriteDOSiteData(siteIndex, FuncInline.enumDOSite.Y1_Conveyor_Backward, false);
                                }
                            }
                            FuncInline.SiteMessage = "";
                            break;

                        default:
                            break;
                    }
                    for (int i = 0; i < FuncInline.SiteCheckTime.Length; i++)
                    {
                        Util.ResetWatch(ref FuncInline.SiteCheckTime[i]);
                    }

                }

            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }

            siteAction = enumSiteAction.None;
            timerSiteDoing = false;
            if (GlobalVar.GlobalStop)
            {
                try
                {
                    timerSite.Dispose();
                }
                catch { }
            }
        }

        private void btnUnclamp_Click(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVar.SystemStatus < enumSystemStatus.AutoRun ||
                    GlobalVar.SystemStatus == enumSystemStatus.ErrorStop)
                {
                    //FuncInline.enumTeachingPos startPos = FuncInline.DetailSite;
                    //FuncInline.enumTeachingPos endPos = FuncInline.DetailSite;
                    string msg = FuncInline.DetailSite.ToString();
                    if (!siteOnly)
                    {
                        msg = "all sites";
                        if (testPC == 0) // 전체 사이트
                        {
                            startPos = FuncInline.enumTeachingPos.Site1_F_DT1;
                            endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + FuncInline.MaxSiteCount - 1;
                        }
                        else
                        {
                            int pcIndex = ((int)FuncInline.DetailSite - (int)FuncInline.enumTeachingPos.Site1_F_DT1) / 7;
                            startPos = FuncInline.enumTeachingPos.Site1_F_DT1 + pcIndex * 7;
                            endPos = FuncInline.enumTeachingPos.Site1_F_DT1 + Math.Min(FuncInline.MaxSiteCount - 1, pcIndex * 7 + (7 - 1));
                            msg = "PC " + (pcIndex + 1);
                        }
                    }
                    else
                    {
                        startPos = FuncInline.DetailSite;
                        endPos = FuncInline.DetailSite;
                    }

                    if (!FuncWin.MessageBoxOK("Unclamp " + msg + " ?", GlobalVar.ScreenNumbers[0]))
                    {
                        return;
                    }

                    FuncLog.WriteLog("SiteDetail - Unclamp Site " + msg);

                    for (FuncInline.enumTeachingPos pos = startPos; pos <= endPos; pos++)
                    {
                        int siteIndex = ((int)pos - (int)FuncInline.enumTeachingPos.Site1_F_DT1);

                        if (FuncInline.PCBInfo[(int)pos].PCBStatus == FuncInline.enumSMDStatus.Testing &&
                            !FuncWin.MessageBoxOK(pos.ToString() + " is testing. Do you want unclamp realy?"))
                        {
                            return;
                        }
                        FuncInline.WriteDOSiteData(siteIndex, FuncInline.enumDOSite.Y0_Conveyor_Forward, false);
                        FuncInline.WriteDOSiteData(siteIndex, FuncInline.enumDOSite.Y1_Conveyor_Backward, false);
                        FuncInline.WriteDOSiteData(siteIndex, FuncInline.enumDOSite.Y2_Up_Jig_Down, false);
                        FuncInline.WriteDOSiteData(siteIndex, FuncInline.enumDOSite.Y3_Jig_Stop_Up, false);
                        FuncInline.WriteDOSiteData(siteIndex, FuncInline.enumDOSite.Y4_Down_Jig_Up, false);
                        FuncInline.WriteDOSiteData(siteIndex, FuncInline.enumDOSite.Y5_Up_Blower_On, false);
                        FuncInline.WriteDOSiteData(siteIndex, FuncInline.enumDOSite.Y6_Down_Blower_On, false);
                        //DIO.WriteDOData(FuncInline.enumDONames.Y08_2_Site1_Clamp_Forward + siteIndex * GlobalVar.DOModuleGap, false);
                        // 스탭모터로 대체 필요
                    }
                }
                else
                {
                    FuncWin.TopMessageBox("Can't clear while system is running.");
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void lblArray_Click(object sender, EventArgs e)
        {
            try
            {
                string name = ((Label)sender).Name;
                int array_no = int.Parse(name.Replace("lbl", "").Replace("Title", "").Replace("Array", ""));
                string cn = FuncInline.PCBInfo[(int)FuncInline.DetailSite].Barcode[array_no - 1];
                if (FuncWin.InputBox("Change CN", "Change CN of " + FuncInline.DetailSite.ToString() + " Array #" + array_no + "?", ref cn) == DialogResult.OK)
                {
                    FuncInline.PCBInfo[(int)FuncInline.DetailSite].Barcode[array_no - 1] = cn;
                }
            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
        }

        private void btnBlockHistory_Click(object sender, EventArgs e)
        {
            FuncLog.WriteLog("SiteDetail - Block History Click ");

            BlockHistory dlg = new BlockHistory();
            dlg.ShowDialog();
        }
    }

}
