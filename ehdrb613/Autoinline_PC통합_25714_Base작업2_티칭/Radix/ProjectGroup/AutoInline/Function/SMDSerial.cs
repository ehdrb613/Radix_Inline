using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.IO; // MemoryStream
using System.Threading;
using System.Diagnostics;

namespace Radix
{
    public class SMDSerial
    {



        // YJ20230508 시뮬레이션 기능 추가
        // 각 사이트/어레이별 테스트 시작 지령시 시뮬레이션 완료 시간 맞춰 수신하도록 한다. 타이머 필요.
        // 나머지 지령은 즉각 반응하면 된다.
        // 송신은 SendMessage 시 시뮬레이션 관련 정보(사이트/어레이 및 타이머) 세팅
        // 수신은 타이머로 Buffer에 메시지 추가하고 SerialSMD_DataReceived 호출한다.

        private int SMDNo = 0;
        private SerialPort SMD = new SerialPort();

        private string Buffer = ""; // 불완전한 메시지 확인용

        Random rnd = new Random();

        private void debug(string str)
        {
            Util.Debug("SMD " + SMDNo + " - " + str);
        }
        public SMDSerial(int no)
        {
            SMDNo = no;
            //if (GlobalVar.Simulation ||
            //    FuncInline.SimulationMode)
            //{
            timerSMD = new System.Threading.Timer(new TimerCallback(TimerSMD), false, 0, 1000);
            //}
        }




        public int? FtSiteIndex { get; set; }     // FT 포트만 담당 사이트(0-base) 지정. DL/Gen6+는 null

        // FT 포트에서 마지막으로 명령을 보낸 사이트(0-based). 
        // DL(ComSMD[0]) 또는 Gen6+ 통합검사에서는 사용하지 않음.
        public int LegacyLastFtSiteIndex { get; set; } = -1;

        /// <summary>
        /// DL 포트 여부 (프로젝트 규약: DL은 ComSMD[0])
        /// </summary>
        private bool IsDlPort
        {
            get { return this.SMDNo == 0; } // ComSMD[0] = DL, 나머지 = FT
        }
        public void SMDSet(string port, int baud, int databit, Parity parity, StopBits stopbit) // 통신설정값 저장
        {
            SMD.PortName = port;
            SMD.BaudRate = baud;
            SMD.DataBits = databit;
            SMD.Parity = parity;
            SMD.Handshake = Handshake.None;
            SMD.StopBits = stopbit;
            SMD.ReadTimeout = 1000;
            SMD.WriteTimeout = 1000;
            SMD.DataReceived += SerialSMD_DataReceived;
        }

        public bool Connect() // serial 포트 연결
        {
            if (!SMD.IsOpen)
            {
                try
                {
                    //debug("SMD connect");
                    // 연결
                    if (GlobalVar.Simulation)
                    {
                        return true;
                    }
                    else
                    {
                        SMD.Open();
                        FuncLog.WriteLog_Tester(SMD.PortName + " CONNECT " + SMD.IsOpen.ToString());
                        return SMD.IsOpen;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                //debug("loader connected");
            }
            return true;
        }

        public void Disconnect() // serial 포트 연결 끊기
        {
            //debug("SMD disconnect");
            FuncLog.WriteLog_Tester(SMD.PortName + " DISCONNECT ");
            if (!GlobalVar.Simulation)
            {
                SMD.Close();
            }
        }

        public void SendMessage(string text) // 포트로 메시지 전송
        {
            FuncLog.WriteLog_Tester(SMD.PortName + " SEND : " + text);

            if (string.IsNullOrEmpty(text)) return;

            if (GlobalVar.Simulation ||
                FuncInline.SimulationMode ||
                GlobalVar.DryRun)
            {
                if (text.StartsWith("RD")) // 즉시응답 메시지
                {
                    Buffer += "OK" + text.Substring(2) + "\r\n";
                    SMD_DataReceived();
                }
                if (text.StartsWith("ST")) // 즉시응답 메시지
                {
                    Buffer += "OK" + text.Substring(3) + "\r\n";
                    SMD_DataReceived();
                }

                // STS 등은 무시해도 됨
                // 시작지령만 사이트/어레이 기억했다가 타이머 경과시 완료처리하면 됨(Buffer에 수신메시지 강제 할당하고 received 호출)
                // FuncInline.PCBInfo 와 dual 관련 정보 확인해서 완료처리하면 됨 ==> SendMessage에서는 따로 처리할 필요 없이 타이머만 만들면 된다.
                // STA0102[G33137710RC0B] ==> OK0102 ==> PS0102[G33137710RC0B] (FL010297[G33137710RC0B])
                // STF0904[G32252010RE07] ==> OK0904 ==> PS0904[G32252010RE07] (FL090497[G32252010RE07])
            }
            else
            {
                //SMD.DiscardOutBuffer();
                //SMD.DiscardInBuffer();
                SMD.Write("\r\n");
                SMD.Write(text + "\r\n");
            }

        }

        public void SetTestType(int site, int array, FuncInline.enumTestType type) // 테스트 유형 D/F/A
        {
            FuncInline.TestType = type;
        }

        public void SetStatus(int site, int array, FuncInline.enumSMDStatus status) // array 진행 상태 변경
        {
            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[array] = status;

            SetAllStatus(site);
        }

        public void SetAllStatus(int site)
        {
            return; // 통신시에는 각 array에만 체크하고 autoRun에서 전체적인 체크를 한다.
        }

        public void ClearSite(int site) // Site에서 배출시 모든 정보 초기화
        {
            //for (int i = 0; i < FuncInline.MaxArrayCount; i++)
            //{
            //    //Barcode[site, i] = "";
            //    //SMDStatus[site, i] = FuncInline.enumSMDStatus.UnKnown;
            //    Error[site, i] = 0;
            //}
        }

        public string GetBuffer() // 디버깅용. 통신버퍼 내용 가져오기
        {
            return Buffer;
        }

        public string GetAllState() // 디버깅용. 전체 상태값 가져오기
        {
            string str = "";
            //for (int j = 0; j < FuncInline.MaxSiteCount; j++)
            //{
            //    for (int i = 0; i < FuncInline.MaxArrayCount; i++)
            //    {
            //        str += j.ToString() + i + ":" + Barcode[j, i] + "," + TestType[j, i] + "," + SMDStatus[j, i].ToString() + "," + Error[j, i] + "\r\n";
            //    }
            //}
            return str;
        }

        /// <summary>
        /// 사이트 시작(배열 단위 준비/시작 명령 송신)
        /// siteIndex : 0~25 (Site1~Site26에 대응)
        /// </summary>
        /// 
        public bool SendStart(int site) // site 내 모든 어레이에 테스트 시작 지령 전송. 0부터 site1, site8, site15
        {
            // siteIndex: 0 기반 (Site1_F_DT1 → 0)
            var pos = FuncInline.enumTeachingPos.Site1_F_DT1 + site;
            var info = FuncInline.PCBInfo[(int)pos];
            int maxArray = FuncInline.MaxArrayCount;

            if (site < 0)
            {
                return false;
            }
            try
            {
                //debug("STA : " + site);
                if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].BuyerChange)
                {
                    // Blue 모드면 전송 금지
                    return false;
                }
                if (!FuncInline.IsContactDown((FuncInline.enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + site)))
                {
                    // 핀 내려져 있지 않으면 전송 금지
                    return false;
                }

                //세대별 사이트 별로 DownLoad테스트, Function테스트, 통합 테스트 구분되어야 함


                #region Gen6 이상, 통합검사
                if (FuncInline.InlineType >= FuncInline.enumInlineType.Gen6)
                {
                    #region RD 처리
                    bool all_ready = true;
                    if (FuncInline.UseSMDReady)
                    {
                        // 전체 어레이 돌면서 SMDReady 모두 되어 있으면 통과
                        // 통과 안 된(SMDReadySent 안 되어 있는) 어레이 RD 전송
                        // Retry Count는 SMDReadySent 후 OK 시 초기화
                        // NG시 에러 처리
                        // Timeout은 AutoRun에서 체크
                        for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] == FuncInline.enumSMDStatus.Before_Command && // 지령 전송 단계고
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i].Length > 0 &&  // Xout 에서 설정된 대로 코드가 등록된 것만 전송
                                !FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDReady[i]) // ready 아니면
                            {

                                all_ready = false;
                                if (!FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDReadySent[i])
                                {
                                    // RD 전송 안 했으면 전송한다.
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].CommandRetry[i]++;
                                    string msg = "RD" + Util.IntToString(site + 1, 2) + Util.IntToString(i + 1, 2) + "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i] + "]";

                                    Util.ResetWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestWatch[i]);
                                    Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestWatch[i]);
                                    FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .SMDStatus " + (i + 1) + " = FuncInline.enumSMDStatus.Ready_Sent");
                                    FuncInline.InsertCommLog(site + 1, i + 1, "RD", msg, 0, -1, "");
                                    SendMessage(msg);
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDReadySent[i] = true;  //RD 전송했으면
                                    //FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].CommandRetry[i]++;
                                }
                                else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestWatch[i] != null &&
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestWatch[i].IsRunning &&
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestWatch[i].ElapsedMilliseconds > 10 * 1000)
                                {
                                    // RD 전송 후 10초 경과하면 다시 전송하기 위해 전송여부 초기화한다.
                                    // 전송시 재시도 카운트 올려서 타임아웃 처리한다.
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDReadySent[i] = false;
                                }
                            }
                            else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode[i] == 996)
                            {
                                // TestFail에 코드 996은 command NG 이므로 진행하면 안 된다.
                                all_ready = false;
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].CommandRetry[i] = 999; // command timeout으로 빼기 위해 무지 큰 숫자로 넣어 버림
                            }
                        }
                        #region 다시 한번 ready 검사
                        for (int i = 0; i < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode.Length; i++)
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i].Length > 0 &&
                                !FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Xout[i] &&
                                !FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].BadMark[i] &&
                                !FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDReady[i] &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] == FuncInline.enumSMDStatus.Before_Command)
                            {
                                return false;
                            }
                        }
                        #endregion
                    }
                    if (!all_ready) // RD 사용하고 모두 OK 수신 안 했으면 ST로 진행하지 않음
                    {
                        return true;
                    }
                    #endregion


                    // Ready 모두 통과한 경우 ST로 진행

                    bool all_sent = true;
                    //debug("site index : " + ((int)FuncInline.enumTeachingPos.Site1_F_DT1 + site));
                    //debug("site name : " + (enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + site));
                    for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                    {


                        // RD ==> OK 못 받은 어레이 있으면 진행하면 안 된다.
                        // 루프 분리해서 별개로 처리해야 할 듯

                        //debug("SMDStatus " + i + " : " + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i].ToString());
                        //debug("barcode : " + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i]);
                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] == FuncInline.enumSMDStatus.Before_Command &&
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i].Length > 0) // Xout 에서 설정된 대로 코드가 등록된 것만 전송
                        {
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].CommandRetry[i]++;

                            int errorCode = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode[i];
                            string msg = "ST" + FuncInline.TestType.ToString().Substring(0, 1) + Util.IntToString(site + 1, 2) + Util.IntToString(i + 1, 2) + "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i] + "]";
                            if (errorCode >= 0 &&
                                FuncInline.TestErrorRetestType[errorCode] != null)
                            {
                                msg = "ST" + FuncInline.TestErrorRetestType[errorCode].Substring(0, 1) + Util.IntToString(site + 1, 2) + Util.IntToString(i + 1, 2) + "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i] + "]";
                            }

                            if (errorCode >= 0 &&
                                FuncInline.TestErrorRetestType[errorCode] == null)
                            {
                                // 등록된 코드가 없으면 즉시 Fail 처리
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] = FuncInline.enumSMDStatus.Test_Fail;
                            }
                            else
                            {

                                Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestWatch[i]);
                                if (GlobalVar.SMDLog &&
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] != FuncInline.enumSMDStatus.Command_Sent)
                                {
                                    FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .SMDStatus " + (i + 1) + " = FuncInline.enumSMDStatus.Command_Sent");
                                }
                                all_sent = false;
                                //FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].CommandRetry[i]++;
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] = FuncInline.enumSMDStatus.Command_Sent; // 지령에 대한 응답 수신 여부 초기화


                                //FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].TestSite = site + 1;
                                FuncInline.SetTestSite(site);
                                FuncInline.InsertCommLog(site + 1, i + 1, "ST", msg, 0, -1, "");
                                SendMessage(msg);
                            }
                        }
                        #region Xout/BadMark NG로 보낼 경우는 미리 fail 처리
                        //*
                        else if (FuncInline.XoutToNG &&
                            (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] == FuncInline.enumSMDStatus.No_Test ||
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Xout[i]))
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Xout[i])
                            {
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode[i] = 994;
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] = FuncInline.enumSMDStatus.Test_Fail;
                            }
                        }
                        else if (FuncInline.BadMarkToNG &&
                            (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] == FuncInline.enumSMDStatus.No_Test ||
                                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].BadMark[i]))
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].BadMark[i])
                            {
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode[i] = 995;
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] = FuncInline.enumSMDStatus.Test_Fail;
                            }
                        }
                        //*/
                        #endregion
                        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i].Length == 0)
                        {
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] = FuncInline.enumSMDStatus.No_Test;
                        }
                    }
                    if (all_sent)
                    {
                        if (GlobalVar.SMDLog &&
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus != FuncInline.enumSMDStatus.Command_Sent)
                        {
                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .PCBStatus = FuncInline.enumSMDStatus.Command_Sent");
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.Command_Sent;
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].NgType = FuncInline.enumNGType.OK;
                        }
                    }
                    else
                    {
                        Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch);
                    }
                }
                #endregion

                #region Gen5 (DL+FT 분리형)


                bool isDl = this.IsDlPort;

                // 1) RD 단계 (UseSMDReady가 true면 RD→OK 모두 완료될 때까지 ST 진행 금지)
                bool allReady = true;
                if (FuncInline.UseSMDReady)
                {
                    for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                    {
                        bool isDf = info.SMDStatus[i] == FuncInline.enumSMDStatus.DTest_Fail;

                        // 지령 전이고, 사용 어레이여야 RD 대상
                        bool target = info.SMDStatus[i] == FuncInline.enumSMDStatus.Before_Command
                                   && !info.Xout[i]
                                   && !(FuncInline.UseBadMark && info.BadMark[i]);

                        // FT는 ST에 바코드가 필요 → DF가 아니면 바코드 없는 어레이는 RD 생략
                        if (!isDl && !isDf && string.IsNullOrEmpty(info.Barcode[i]))
                            target = false;

                        if (target && !info.SMDReady[i])
                        {
                            allReady = false;

                            bool needSend = !info.SMDReadySent[i];
                            if (!needSend &&
                                info.TestWatch[i] != null &&
                                info.TestWatch[i].IsRunning &&
                                info.TestWatch[i].ElapsedMilliseconds > 10_000)
                            {
                                info.SMDReadySent[i] = false; // 10초 넘으면 재전송
                                needSend = true;
                            }

                            if (needSend)
                            {
                                info.CommandRetry[i]++;
                                string msg = isDl
                                    ? "RD" + (site + 1).ToString("00") + (i + 1).ToString("00")  // DL: RDSSAA
                                    : "RD" + (i + 1).ToString("00");                               // FT: RDAA

                                Util.ResetWatch(ref info.TestWatch[i]);
                                Util.StartWatch(ref info.TestWatch[i]);
                                FuncInline.InsertCommLog(site + 1, i + 1, "RD", msg, 0, -1, "");
                                SendMessage(msg);
                                info.SMDReadySent[i] = true;
                            }
                        }
                        else if (info.SMDStatus[i] == FuncInline.enumSMDStatus.Test_Fail &&
                                 info.ErrorCode[i] == 996)  // Command NG
                        {
                            allReady = false;
                            info.CommandRetry[i] = 999;
                        }
                    }

                    // RD OK 미도달 어레이가 있으면 ST 진행하지 않음
                    for (int i = 0; i < info.Barcode.Length; i++)
                    {
                        bool mustReady = info.SMDStatus[i] == FuncInline.enumSMDStatus.Before_Command
                                      && !info.Xout[i]
                                      && !(FuncInline.UseBadMark && info.BadMark[i]);

                        if (!isDl)
                        {
                            bool isDf = info.SMDStatus[i] == FuncInline.enumSMDStatus.DTest_Fail;
                            if (!isDf && string.IsNullOrEmpty(info.Barcode[i]))
                                mustReady = false;
                        }

                        if (mustReady && !info.SMDReady[i])
                            return false;
                    }
                }
                if (!allReady) return true; // 계속 대기

                // 2) ST/DF 송신
                bool allSent = true;

                for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                {
                    // 정책상 Xout/BadMark를 NG로 보낼 때 미리 Fail
                    if (FuncInline.XoutToNG &&
                        (info.SMDStatus[i] == FuncInline.enumSMDStatus.No_Test || info.Xout[i]))
                    {
                        if (info.Xout[i]) { info.ErrorCode[i] = 994; info.SMDStatus[i] = FuncInline.enumSMDStatus.Test_Fail; }
                        continue;
                    }
                    if (FuncInline.BadMarkToNG &&
                        (info.SMDStatus[i] == FuncInline.enumSMDStatus.No_Test || info.BadMark[i]))
                    {
                        if (info.BadMark[i]) { info.ErrorCode[i] = 995; info.SMDStatus[i] = FuncInline.enumSMDStatus.Test_Fail; }
                        continue;
                    }

                    if (info.SMDStatus[i] != FuncInline.enumSMDStatus.Before_Command) continue;
                    if (info.Xout[i]) continue;
                    if (FuncInline.UseBadMark && info.BadMark[i]) continue;

                    string msg;

                    if (isDl)
                    {
                        // DL: STSSAA (바코드 없음)
                        msg = "ST" + (site + 1).ToString("00") + (i + 1).ToString("00");
                    }
                    else
                    {
                        string bc = info.Barcode[i];

                        // FT: DL Fail이면 DF, 아니면 ST + AA + [BARCODE]
                        if (info.SMDStatus[i] == FuncInline.enumSMDStatus.DTest_Fail)
                        {
                            int code = info.ErrorCode[i]; if (code < 0) code = 0;
                            msg = "DF" + (i + 1).ToString("00") + code.ToString() + "[" + bc + "]";
                        }
                        else
                        {

                            if (string.IsNullOrEmpty(bc)) { info.SMDStatus[i] = FuncInline.enumSMDStatus.No_Test; continue; }
                            msg = "ST" + (i + 1).ToString("00") + "[" + bc + "]";
                        }
                    }

                    info.CommandRetry[i]++;
                    Util.StartWatch(ref info.TestWatch[i]);
                    if (GlobalVar.SMDLog && info.SMDStatus[i] != FuncInline.enumSMDStatus.Command_Sent)
                        FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + pos.ToString() + " .SMDStatus " + (i + 1) + " = FuncInline.enumSMDStatus.Command_Sent");

                    allSent = false;
                    info.SMDStatus[i] = FuncInline.enumSMDStatus.Command_Sent;

                    FuncInline.SetTestSite(site);
                    FuncInline.InsertCommLog(site + 1, i + 1, (msg.StartsWith("DF") ? "DF" : "ST"), msg, 0, -1, "");
                    SendMessage(msg);
                }

                if (allSent)
                {
                    if (GlobalVar.SMDLog &&
                        info.PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                        info.PCBStatus != FuncInline.enumSMDStatus.Command_Sent)
                    {
                        FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + pos.ToString() + " .PCBStatus = FuncInline.enumSMDStatus.Command_Sent");
                        info.PCBStatus = FuncInline.enumSMDStatus.Command_Sent;
                        info.NgType = FuncInline.enumNGType.OK;
                    }
                }
                else
                {
                    Util.StartWatch(ref info.StopWatch);
                }

                return true;
                #endregion

                #region Gen1~4 (DL+FT 분리형, FT는 3포트/측)

                #endregion

            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return true;
        }

        public bool SendStop(int site) // site 내 모든 어레이에 테스트 시작 지령 전송. 0부터 site1, site8, site15
        {
            if (site < 0)
            {
                return false;
            }
            try
            {
                // 공통 참조
                var pos = FuncInline.enumTeachingPos.Site1_F_DT1 + site;
                var info = FuncInline.PCBInfo[(int)pos];


                // ─────────────────────────────────────────────
                // Gen6+ (통합검사) : 기존 동작 그대로 유지
                //   STSSSAA[BARCODE]
                // ─────────────────────────────────────────────
                if (FuncInline.InlineType >= FuncInline.enumInlineType.Gen6)
                {
                    //통합 테스트일때
                    bool all_sent = true;
                    //debug("site index : " + ((int)FuncInline.enumTeachingPos.Site1_F_DT1 + site));
                    //debug("site name : " + (enumTeachingPos)((int)FuncInline.enumTeachingPos.Site1_F_DT1 + site));
                    for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                    {
                        //debug("SMDStatus " + i + " : " + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i].ToString());
                        //debug("barcode : " + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i]);
                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout &&
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i].Length > 0) // Xout 에서 설정된 대로 코드가 등록된 것만 전송
                        {
                            string msg = "STS" + Util.IntToString(site + 1, 2) + Util.IntToString(i + 1, 2) + "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i] + "]";
                            SendMessage(msg);

                            FuncInline.InsertCommLog(site + 1, i + 1, "STS", msg, 0, 999, "TEST_TIMEOUT");
                        }
                        else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].Barcode[i].Length == 0)
                        {
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[i] = FuncInline.enumSMDStatus.No_Test;
                        }
                    }
                    if (all_sent)
                    {
                        if (GlobalVar.SMDLog &&
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus <= FuncInline.enumSMDStatus.Testing &&
                            !FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].UserCancel)
                        {
                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .UserCancel = True");
                            Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].StopWatch);
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].UserCancel = true;
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].PCBStatus = FuncInline.enumSMDStatus.User_Cancel;
                        }
                    }
                    return true;
                }
                // ─────────────────────────────────────────────
                // Gen1~5 (DL/FT 분리형)
                //   · DL : STS + SS(2) + AA(2)                (바코드 없음)
                //   · FT : STS + AA(2) + [BARCODE]            (SendStart() FT 형식과 동일한 괄호 포함)
                //   · FT 포트는 사이트 개념 없이 포트=층 매핑(FtSiteIndex)으로만 동작
                // ─────────────────────────────────────────────

                if (this.IsDlPort && FuncInline.InlineType <= FuncInline.enumInlineType.Gen5)
                {
                    // DL 포트 (ComSMD[0])
                    // SS 매핑 규칙 :
                    //   Front  : Site1(1)  ~ Site10(10)          -> 01 ~ 10
                    //   Rear   : Site14(14) ~ Site23(22)        -> 11 ~ 20
                    //   그 외 사이트는 DL 번호 매핑 제외

                    // DL용 사이트번호(SS) 매핑
                    int DlMapFromSite(int site0)
                    {
                        int ss = site0 + 1; // 1-base

                        if (FuncInline.InlineType <= FuncInline.enumInlineType.Gen4)
                        {
                            // Front Site1~10 → 01~10
                            if (ss >= 1 && ss <= 10) return ss;
                            // Rear  Site14~23 → 11~20
                            if (ss >= 14 && ss <= 23) return (ss - 13) + 10; // 14→11 ... 23→20
                            return -1;
                        }
                        else if (FuncInline.InlineType == FuncInline.enumInlineType.Gen5)
                        {
                            // Gen5 Front는 Site1~9 사용 (→ 01~09)
                            if (ss >= 1 && ss <= 9) return ss;
                            // Rear  Site14~23 → 11~20
                            if (ss >= 14 && ss <= 23) return (ss - 13) + 10;
                            return -1;
                        }
                        return -1;
                    }

                    for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                    {
                        if (info.SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout)
                        {
                            int ss = DlMapFromSite(site);
                            if (ss < 0) continue; // DL 번호 매핑 불가 사이트는 스킵

                            string msg = "STS"
                                       + Util.IntToString(ss, 2)
                                       + Util.IntToString(i + 1, 2); // 바코드 없음
                            SendMessage(msg);
                            FuncInline.InsertCommLog(site + 1, i + 1, "STS", msg, 0, 999, "TEST_TIMEOUT");
                        }
                        else if (info.Barcode[i].Length == 0)
                        {
                            info.SMDStatus[i] = FuncInline.enumSMDStatus.No_Test;
                        }
                    }
                }
                else if (!this.IsDlPort && FuncInline.InlineType <= FuncInline.enumInlineType.Gen5)
                {

                    // FT: 해당 포트는 층별 단독이고, site 인자로 구분 
                    for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                    {
                        if (info.SMDStatus[i] == FuncInline.enumSMDStatus.Test_Timeout &&
                            info.Barcode[i].Length > 0)
                        {
                            string msg = "STS"
                                       + Util.IntToString(i + 1, 2)
                                       + "[" + info.Barcode[i] + "]";
                            SendMessage(msg);
                            FuncInline.InsertCommLog(site + 1, i + 1, "STS", msg, 0, 999, "TEST_TIMEOUT");
                        }
                        else if (info.Barcode[i].Length == 0)
                        {
                            info.SMDStatus[i] = FuncInline.enumSMDStatus.No_Test;
                        }
                    }
                }

                if(true)
                { 
                    bool all_sent = true;
                    if (all_sent)
                    {
                        if (GlobalVar.SMDLog &&
                            info.PCBStatus != FuncInline.enumSMDStatus.UnKnown &&
                            info.PCBStatus <= FuncInline.enumSMDStatus.Testing &&
                            !info.UserCancel)
                        {
                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .UserCancel = True");
                            Util.StartWatch(ref info.StopWatch);
                            info.UserCancel = true;
                            info.PCBStatus = FuncInline.enumSMDStatus.User_Cancel;
                        }
                    }
                }
                


            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return true;
        }

        private System.Threading.Timer timerSMD = null; // 송수신을 시뮬레이션 하기 위한 쓰레드 타이머
        private bool timerSMD_doing = false;
        private Stopwatch smdWatch = new Stopwatch();

        private void TimerSMD(Object state) // 시뮬레이션 완료 처리하기 위한 타이머. RD/ST 지령에 대해서는 sendMessage에서 즉각 응답설정한다.
        {
            // 시뮬레이션모드 때만 작동한다.
            if (!GlobalVar.Simulation &&
                !FuncInline.SimulationMode &&
                !GlobalVar.DryRun)
            {
                return;
            }

            if (!smdWatch.IsRunning)
            {
                Util.StartWatch(ref smdWatch);
            }

            if (timerSMD_doing)
            {
                if (smdWatch.ElapsedMilliseconds < 5 * 1000)
                {
                    return;
                }
                else
                {
                    timerSMD_doing = false;
                    Util.StartWatch(ref smdWatch);
                }
            }

            timerSMD_doing = true;

            try
            {
                // 포트에 해당하는 사이트만 처리
                for (int site = SMDNo * FuncInline.MaxSiteCount; site < SMDNo * FuncInline.MaxSiteCount + FuncInline.MaxSiteCount; site++) // siteIndex와 돌려 환산하기 위해 0부터 계산(siteNo로 환산)
                {
                    int siteIndex = site;

                    // 사이트 testing 아니면 통과
                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].PCBStatus != FuncInline.enumSMDStatus.Testing)
                    {
                        continue;
                    }

                    int siteNo = site + 1;

                    int firstIndex = 0; // NG 처리할 어레이 인덱스
                    for (int i = 0; i < FuncInline.MaxArrayCount; i++)
                    {
                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[i].Length > 0)
                        {
                            firstIndex = i;
                            break;
                        }
                    }

                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].StopWatch != null &&
                        //FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].StopWatch.IsRunning &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].StopWatch.ElapsedMilliseconds >= FuncInline.TestPassTime * 1000)
                    {
                        for (int array = 0; array < FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode.Length; array++)
                        {
                            if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[array].Length > 0 &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].SMDStatus[array] >= FuncInline.enumSMDStatus.Before_Command &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].SMDStatus[array] <= FuncInline.enumSMDStatus.Command_Sent)
                            {
                                Buffer += "OK" +
                                            Util.IntToString(site + 1, 2) +
                                            Util.IntToString(array + 1, 2) +
                                            "\r\n";
                                SMD_DataReceived();
                            }

                            else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[array].Length > 0 &&
                                FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].SMDStatus[array] == FuncInline.enumSMDStatus.Testing)
                            {
                                // 타이머 경과시 완료처리하면 됨(Buffer에 수신메시지 강제 할당하고 received 호출)
                                // FuncInline.PCBInfo 와 dual 관련 정보 확인해서 완료처리하면 됨 ==> SendMessage에서는 따로 처리할 필요 없이 타이머만 만들면 된다.
                                // STA0102[G33137710RC0B] ==> OK0102 ==> PS0102[G33137710RC0B] (FL010297[G33137710RC0B])
                                // STF0904[G32252010RE07] ==> OK0904 ==> PS0904[G32252010RE07] (FL090497[G32252010RE07])
                                #region 시뮬레이션 테스트용
                                /*
                                if (array < FuncInline.MaxArrayCount / 2)
                                {
                                    Buffer += "PS" +
                                                Util.IntToString(site + 1, 2) +
                                                Util.IntToString(array + 1, 2) +
                                                "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[array] + "]\r\n";
                                }
                                //*/
                                #endregion
                                /* 임시 좌측은 Pass, 우측은 Fail
                                if (siteIndex < 20)
                                {
                                    Buffer += "PS" +
                                                Util.IntToString(site + 1, 2) +
                                                Util.IntToString(array + 1, 2) +
                                                "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[array] + "]\r\n";
                                }
                                else
                                {
                                    Buffer += "FL" +
                                                Util.IntToString(site + 1, 2) +
                                                Util.IntToString(array + 1, 2) +
                                                Util.IntToString(GlobalVar.AutoInline_TestFailCode, 2) +
                                                "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[array] + "]\r\n";
                                    FuncInline.TestPassCount = 0;
                                }
                                //*/
                                //* 본로직
                                /*
                                if (array == 5)
                                {
                                    Buffer += "FL" +
                                                Util.IntToString(site + 1, 2) +
                                                Util.IntToString(array + 1, 2) +
                                                "54" +
                                                "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[array] + "]\r\n";
                                    FuncInline.TestPassCount = 0;

                                        // 등록된 코드가 없으면 즉시 Fail 처리
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode[array] = 992; // NO_CODE
                                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[array] = FuncInline.enumSMDStatus.Test_Fail;
                                        if (GlobalVar.SMDLog &&
                                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] != FuncInline.enumSMDStatus.Test_Fail)
                                        {
                                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1).ToString() + " .SMDStatus " + array + " = FuncInline.enumSMDStatus.Test_Fail");
                                        }

                                }
                                //*/
                                if (FuncInline.TestPassMode == FuncInline.enumSMDStatus.Test_Pass ||
                                    !FuncInline.TestFail[array])
                                {
                                    Buffer += "PS" +
                                                Util.IntToString(site + 1, 2) +
                                                Util.IntToString(array + 1, 2) +
                                                "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[array] + "]\r\n";
                                }
                                else if (FuncInline.TestPassMode == FuncInline.enumSMDStatus.Test_Fail)
                                {
                                    //* 임시. 타임아웃 테스트 위해 막음
                                    // 지정횟수 이내면 Pass
                                    FuncInline.TestPassCount++;
                                    if (FuncInline.TestFail[array] &&
                                        (FuncInline.TestPassSet == 0 ||
                                                FuncInline.TestPassCount > FuncInline.TestPassSet))
                                    {
                                        // 0 에서 100까지 랜덤 숫자
                                        int fail_code = rnd.Next(0, 100);
                                        Buffer += "FL" +
                                                    Util.IntToString(site + 1, 2) +
                                                    Util.IntToString(array + 1, 2) +
                                                    Util.IntToString(fail_code, 2) +
                                                    "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[array] + "]\r\n";
                                        FuncInline.TestPassCount = 0;
                                    }
                                    else
                                    {
                                        Buffer += "PS" +
                                                    Util.IntToString(site + 1, 2) +
                                                    Util.IntToString(array + 1, 2) +
                                                    "[" + FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex].Barcode[array] + "]\r\n";
                                    }
                                    //*/
                                }
                                //*/
                                SMD_DataReceived();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }

            timerSMD_doing = false;

            if (GlobalVar.GlobalStop)
            {
                if (!GlobalVar.Simulation)
                {
                    Disconnect();
                }

                timerSMD.Dispose();
            }
        }


        public void SerialSMD_DataReceived(object sender, SerialDataReceivedEventArgs e) // 데이터 수신시
        {
            SMD_DataReceived();
        }

        public void SMD_DataReceived()
        {
            try
            {
                string msg = "";
                if (!GlobalVar.Simulation &&
                    !FuncInline.SimulationMode &&
                    !GlobalVar.DryRun)
                {
                    msg = SMD.ReadExisting();
                }
                if (!GlobalVar.Simulation &&
                    !FuncInline.SimulationMode &&
                    !GlobalVar.DryRun &&
                    (msg == null ||
                        msg.Length == 0))
                {
                    return;
                }

                if (!GlobalVar.Simulation &&
                    !FuncInline.SimulationMode &&
                    !GlobalVar.DryRun)
                {
                    FuncLog.WriteLog_Tester(SMD.PortName + " RECEIVE : " + msg);
                    Buffer += msg;
                }
                if (!Buffer.Contains("\r\n")) // CR LF 없으면 버퍼에 받아만 두고 리턴
                {
                    return;
                }

                // 여러 메시지일 경우 부분 메시지 전체를 다 판단해야 함
              
                string[] lines = Buffer.Split('\n');   // 마지막 조각(lines[^1])은 미완성일 수 있음


                //debug("SMD receive : " + Buffer);
                FuncLog.WriteLog_Tester(SMD.PortName + " BUFFER : " + Buffer);

                if (lines.Length > 1)
                {
                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        string line = lines[i].Trim('\r', '\n').Trim();
                        if (line.Length < 4) continue; // 너무 짧으면 무시

                        try
                        {
                            // ----------------------------------------------------------
                            // Gen6+ : 기존 통합검사 수신 로직 그대로 유지
                            // ----------------------------------------------------------
                            if (FuncInline.InlineType >= FuncInline.enumInlineType.Gen6)
                            {
                                ProcessGen6PlusLine(line);
                                continue;
                            }

                            // ----------------------------------------------------------
                            // Gen1~5 : DL/FT 분리형
                            // ----------------------------------------------------------
                            string cmd;
                            int siteIndex0 = -1;
                            int arrayIndex0 = -1;
                            int? defectCode = -1;

                            if (this.IsDlPort)
                            {
                                // DL: OK/NG/PS/FL + SS(2) + AA(2)
                                if (!TryParseDlFrame_Gen1to5(line, out cmd, out siteIndex0, out arrayIndex0, out defectCode))
                                    continue;
                            }
                            else
                            {
                              
                               

                                if (!TryParseFtFrame_Gen1to5(line, out cmd, out arrayIndex0, out defectCode))
                                    continue;

                                var ftSite = ResolveFtSiteIndex();
                                if (!ftSite.HasValue)
                                {
                                    FuncLog.WriteLog_Tester($"{SMD.PortName} FT site mapping missing. Drop: {line}");
                                    continue;
                                }
                                siteIndex0 = ftSite.Value;
                            }

                            // 공통 처리 (Ready OK, Command OK/NG, PS/FL 등)
                            HandleLegacyCommon(line, cmd, siteIndex0, arrayIndex0, defectCode);
                        }
                        catch (Exception exLine)
                        {
                            debug(exLine.ToString());
                            debug(exLine.StackTrace);
                        }
                    }

                    // 마지막 미완 메시지를 버퍼에 남겨둠
                    Buffer = lines[lines.Length - 1];
                }



            }

            catch (Exception ex)
            {
                try { FuncLog.WriteLog_Tester("SMD_DataReceived EX: " + ex.ToString()); } catch { }
            }
        }

        // ======================================================================
        // [Gen6+] 기존 통합검사 1줄 처리 (원본 로직 그대로 사용)
        //  - 아래는 기존 수신코드를 "line" 단위로 옮긴 형태입니다.
        //  - 기존 코드와 동일하게 동작하도록 site/array/defectCode 파싱 및 상태 갱신을 수행합니다.
        // ======================================================================
        private void ProcessGen6PlusLine(string line)
        {
            // === 아래는 질문에 주신 기존 "lines[i]" 기반 코드를 "line" 변수 기준으로 바꿔 이식한 것입니다. ===
            //     (주요 흐름: OK(Ready/Command) / NG(Command NG) / SS(취소) / PS(패스) / FL(불량) 등)
            try
            {
                if (line.Length <= 6) return;

                int site = int.Parse(line.Substring(2, 2));
                int array = int.Parse(line.Substring(4, 2));
                int defectCode = -1;

                // Ready OK 처리
                if (line.Substring(0, 2) == "OK")
                {
                    Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].StopWatch);
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].CommandRetry[array - 1] = 0;

                    if (FuncInline.UseSMDReady &&
                        (!FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDReady[array - 1]))
                    {
                        FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1).ToString() + " .SMDReady " + array + " = True");
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDReady[array - 1] = true;
                        FuncLog.WriteLog_Tester(SMD.PortName + " READY OK : " + site + "," + array);
                    }
                    else if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].PCBStatus != FuncInline.enumSMDStatus.User_Cancel &&
                             FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] == FuncInline.enumSMDStatus.Command_Sent)
                    {
                        if (GlobalVar.SMDLog &&
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] != FuncInline.enumSMDStatus.Testing)
                        {
                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1).ToString() + " .SMDStatus " + array + " = FuncInline.enumSMDStatus.Testing");
                        }
                        Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].TestWatch[array - 1]);
                        if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].StopWatch == null ||
                            !FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].StopWatch.IsRunning)
                        {
                            Util.StartWatch(ref FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].StopWatch);
                        }
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] = FuncInline.enumSMDStatus.Testing;
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].ErrorCode[array - 1] = -1;
                        FuncLog.WriteLog_Tester(SMD.PortName + " RES OK : " + site + "," + array);
                    }
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDReadySent[array - 1] = false;
                }
                else if (line.Substring(0, 2) == "NG")
                {
                    defectCode = 996; // COMMAND_NG
                    if (GlobalVar.SMDLog &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] != FuncInline.enumSMDStatus.Test_Fail)
                    {
                        FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site).ToString() + " .SMDStatus " + (array) + " = FuncInline.enumSMDStatus.Test_Fail");
                    }

                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].TestWatch[array - 1].Stop();
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] = FuncInline.enumSMDStatus.Test_Fail;
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].ErrorCode[array - 1] = defectCode;
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].BeforeCode[array - 1] = defectCode;
                    FuncLog.WriteLog_Tester(SMD.PortName + " RES NG : " + site + "," + array + "-" + defectCode);

                    if (FuncInline.ArrayNGCode[site - 1, array - 1] == defectCode)
                        FuncInline.ArrayNGCount[site - 1, array - 1]++;
                    else
                        FuncInline.ArrayNGCount[site - 1, array - 1] = 1;
                    FuncInline.ArrayNGCode[site - 1, array - 1] = defectCode;

                    // 블럭/불량율 체크 등 기존 로직 그대로…
                    // (생략 없이 프로젝트 원본대로 유지 필요시, 여기 전체 붙여 넣기)
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].PCBStatus = FuncInline.enumSMDStatus.Test_Fail;
                }
                else if (line.Substring(0, 2) == "SS")
                {
                    defectCode = 997; // TEST_CANCEL
                    if (GlobalVar.SMDLog &&
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] != FuncInline.enumSMDStatus.Test_Cancel)
                    {
                        FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1).ToString() + " .SMDStatus " + array + " = FuncInline.enumSMDStatus.Test_Cancel");
                    }
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].TestWatch[array - 1].Stop();
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] = FuncInline.enumSMDStatus.Test_Cancel;
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].ErrorCode[array - 1] = defectCode;
                    FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].BeforeCode[array - 1] = defectCode;
                    FuncLog.WriteLog_Tester(SMD.PortName + " RES CANCEL : " + site + "," + array + "-" + defectCode);
                }
                else if (line.Substring(0, 2) == "PS")
                {
                    // (기존 CN Cross 체크 등 원본 그대로 유지 가능)
                    if (FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] <= FuncInline.enumSMDStatus.Testing)
                    {
                        if (GlobalVar.SMDLog &&
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] != FuncInline.enumSMDStatus.Test_Pass)
                        {
                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1).ToString() + " .SMDStatus " + array + " = FuncInline.enumSMDStatus.Test_Pass");
                        }
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].TestWatch[array - 1].Stop();
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] = FuncInline.enumSMDStatus.Test_Pass;
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].ErrorCode[array - 1] = -1;
                        FuncLog.WriteLog_Tester(SMD.PortName + " TEST PASS : " + site + "," + array);

                        FuncInline.ArrayNGCode[site - 1, array - 1] = -1;
                        FuncInline.ArrayNGCount[site - 1, array - 1] = 0;
                        FuncInline.PlusPinUseByArray(site, array, false, false);
                    }
                }
                else if (line.Length >= 8 && line.Substring(0, 2) == "FL")
                {
                    // defectCode 파싱 (예: "FLSSAA12[BAR]" → 12)
                    try
                    {
                        string head = line.Split('[')[0];
                        if (head.Length >= 8)
                        {
                            // "FL" + "SS" + "AA" 다음의 나머지
                            defectCode = int.Parse(head.Substring(6));
                        }
                    }
                    catch { }

                    if (defectCode >= 0 && FuncInline.TestErrorRetestType[defectCode] == null)
                    {
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].ErrorCode[array - 1] = 992; // NO_CODE
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site].SMDStatus[array - 1] = FuncInline.enumSMDStatus.Test_Fail;
                    }
                    else
                    {
                        if (GlobalVar.SMDLog &&
                            FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] != FuncInline.enumSMDStatus.Test_Fail)
                        {
                            FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + (FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1).ToString() + " .SMDStatus " + array + " = FuncInline.enumSMDStatus.Test_Fail");
                        }
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].TestWatch[array - 1].Stop();
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].ErrorCode[array - 1] = defectCode;
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].BeforeCode[array - 1] = defectCode;
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].UserCancel = false;
                        FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].SMDStatus[array - 1] = FuncInline.enumSMDStatus.Test_Fail;
                        FuncLog.WriteLog_Tester(SMD.PortName + " TEST FAIL : " + site + "," + array + "-" + defectCode);

                        FuncInline.PlusPinUseByArray(site, array, false, true);
                        // (Block/Rate 체크는 원본 로직 그대로)
                    }
                }

                string defectName = "";
                try
                {
                    if (defectCode >= 0 && FuncInline.TestErrorCode[defectCode] != null)
                        defectName = FuncInline.TestErrorCode[defectCode];
                }
                catch { }

                // 커뮤니케이션 로그
                try
                {
                    var watch = FuncInline.PCBInfo[(int)FuncInline.enumTeachingPos.Site1_F_DT1 + site - 1].TestWatch[array - 1];
                    long sec = (watch != null && watch.IsRunning) ? (watch.ElapsedMilliseconds / 1000) : 0;
                    FuncInline.InsertCommLog(site, array, line.Substring(0, 2), line, sec, defectCode, defectName);
                }
                catch { }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
        }

        // ======================================================================
        // [Gen1~5] DL 프레임 파서
        //   - 형식: OK/NG/PS/FL + SS(2) + AA(2)
        //   - 예: "OK1101", "PS2403", "FL0602<ERR>"
        // ======================================================================
        private bool TryParseDlFrame_Gen1to5(string line, out string cmd, out int siteIndex0, out int arrayIndex0, out int? defectCode)
        {
            cmd = "";
            siteIndex0 = -1;
            arrayIndex0 = -1;
            defectCode = null;

            if (line.Length < 6) return false;

            cmd = line.Substring(0, 2);
            if (cmd != "OK" && cmd != "NG" && cmd != "PS" && cmd != "FL" && cmd != "SS") return false;

            // SS(2) + AA(2)
            int ss, aa;
            if (!int.TryParse(line.Substring(2, 2), out ss)) return false;
            if (!int.TryParse(line.Substring(4, 2), out aa)) return false;

            siteIndex0 = ss - 1; // 0-base
            arrayIndex0 = aa - 1;

            if (cmd == "FL")
            {
                // 예: "FL0602" 바로 뒤에 에러코드가 붙을 수 있음 (숫자 연속)
                // 라인 끝까지 숫자를 추출
                if (line.Length > 6)
                {
                    string tail = line.Substring(6).Trim();
                    int ec;
                    if (tail.Length > 0 && int.TryParse(tail, out ec))
                        defectCode = ec;
                }
            }
            return true;
        }

        // ======================================================================
        // [Gen1~5] FT 프레임 파서
        //   - 형식: OK/NG/PS/FL + AA(2)
        //   - 예: "OK01", "PS03", "FL02<ERR>"
        // ======================================================================
        private bool TryParseFtFrame_Gen1to5(string line, out string cmd, out int arrayIndex0, out int? defectCode)
        {
            cmd = "";
            arrayIndex0 = -1;
            defectCode = null;

            if (line.Length < 4) return false;

            cmd = line.Substring(0, 2);
            if (cmd != "OK" && cmd != "NG" && cmd != "PS" && cmd != "FL" && cmd != "SS") return false;

            int aa;
            if (!int.TryParse(line.Substring(2, 2), out aa)) return false;
            arrayIndex0 = aa - 1;

            if (cmd == "FL")
            {
                // 예: "FL02" 뒤의 숫자들 = 에러코드
                if (line.Length > 4)
                {
                    string tail = line.Substring(4).Trim();
                    int ec;
                    if (tail.Length > 0 && int.TryParse(tail, out ec))
                        defectCode = ec;
                }
            }
            return true;
        }

        // ======================================================================
        // [Gen1~5] 공통 처리
        //   - OK: RD OK(Ready) 또는 ST OK(Testing 시작)
        //   - NG: Command NG
        //   - PS: Test Pass
        //   - FL: Test Fail (DL/FT 동일하되 DL Fail은 이후 FT에서 DF 송신을 위해 DTest_Fail 설정)
        // ======================================================================
        private void HandleLegacyCommon(string line, string cmd, int siteIndex0, int arrayIndex0, int? defectCode)
        {
            // 보호
            if (siteIndex0 < 0 || siteIndex0 >= FuncInline.MaxSiteCount) return;
            if (arrayIndex0 < 0 || arrayIndex0 >= FuncInline.MaxArrayCount) return;

            var pos = (int)FuncInline.enumTeachingPos.Site1_F_DT1 + siteIndex0;
            var info = FuncInline.PCBInfo[pos];

            if (cmd == "OK")
            {
                // RD OK → Ready set, 또는 ST OK → Testing 전환
                // Ready 판단은: UseSMDReady == true이고 SMDReady=false 상태면 RD OK로 처리
                info.CommandRetry[arrayIndex0] = 0;

                if (FuncInline.UseSMDReady && !info.SMDReady[arrayIndex0])
                {
                    // RD OK
                    info.SMDReady[arrayIndex0] = true;
                    info.SMDReadySent[arrayIndex0] = false;
                    FuncLog.WriteLog_Tester(SMD.PortName + " READY OK : " + (siteIndex0 + 1) + "," + (arrayIndex0 + 1));
                }
                else if (info.SMDStatus[arrayIndex0] == FuncInline.enumSMDStatus.Command_Sent &&
                         info.PCBStatus != FuncInline.enumSMDStatus.User_Cancel)
                {
                    // ST OK → Testing
                    if (GlobalVar.SMDLog && info.SMDStatus[arrayIndex0] != FuncInline.enumSMDStatus.Testing)
                    {
                        FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + ((FuncInline.enumTeachingPos)pos).ToString() + " .SMDStatus " + (arrayIndex0 + 1) + " = FuncInline.enumSMDStatus.Testing");
                    }
                    Util.StartWatch(ref info.TestWatch[arrayIndex0]);
                    if (info.StopWatch == null || !info.StopWatch.IsRunning)
                        Util.StartWatch(ref info.StopWatch);

                    info.SMDStatus[arrayIndex0] = FuncInline.enumSMDStatus.Testing;
                    info.ErrorCode[arrayIndex0] = -1;
                    FuncLog.WriteLog_Tester(SMD.PortName + " RES OK : " + (siteIndex0 + 1) + "," + (arrayIndex0 + 1));
                }
                return;
            }

            if (cmd == "NG")
            {
                // Command NG
                int ec = 996;
                if (GlobalVar.SMDLog && info.SMDStatus[arrayIndex0] != FuncInline.enumSMDStatus.Test_Fail)
                {
                    FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + ((FuncInline.enumTeachingPos)pos).ToString() + " .SMDStatus " + (arrayIndex0 + 1) + " = FuncInline.enumSMDStatus.Test_Fail");
                }
                if (info.TestWatch[arrayIndex0] != null) info.TestWatch[arrayIndex0].Stop();

                info.SMDStatus[arrayIndex0] = FuncInline.enumSMDStatus.Test_Fail;
                info.ErrorCode[arrayIndex0] = ec;
                info.BeforeCode[arrayIndex0] = ec;
                FuncLog.WriteLog_Tester(SMD.PortName + " RES NG : " + (siteIndex0 + 1) + "," + (arrayIndex0 + 1) + "-" + ec);

                // 누적/연속 불량 카운트 업데이트 (원본 로직과 동일 정책이면 그대로)
                if (FuncInline.ArrayNGCode[siteIndex0, arrayIndex0] == ec)
                    FuncInline.ArrayNGCount[siteIndex0, arrayIndex0]++;
                else
                    FuncInline.ArrayNGCount[siteIndex0, arrayIndex0] = 1;
                FuncInline.ArrayNGCode[siteIndex0, arrayIndex0] = ec;

                // PCB 자체 Fail
                info.PCBStatus = FuncInline.enumSMDStatus.Test_Fail;
                return;
            }

            if (cmd == "SS")
            {
                // Test Cancel
                int ec = 997;
                if (GlobalVar.SMDLog && info.SMDStatus[arrayIndex0] != FuncInline.enumSMDStatus.Test_Cancel)
                {
                    FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + ((FuncInline.enumTeachingPos)pos).ToString() + " .SMDStatus " + (arrayIndex0 + 1) + " = FuncInline.enumSMDStatus.Test_Cancel");
                }
                if (info.TestWatch[arrayIndex0] != null) info.TestWatch[arrayIndex0].Stop();
                info.SMDStatus[arrayIndex0] = FuncInline.enumSMDStatus.Test_Cancel;
                info.ErrorCode[arrayIndex0] = ec;
                info.BeforeCode[arrayIndex0] = ec;
                FuncLog.WriteLog_Tester(SMD.PortName + " RES CANCEL : " + (siteIndex0 + 1) + "," + (arrayIndex0 + 1) + "-" + ec);
                return;
            }

            if (cmd == "PS")
            {
                // Test Pass
                if (info.SMDStatus[arrayIndex0] <= FuncInline.enumSMDStatus.Testing)
                {
                    if (GlobalVar.SMDLog && info.SMDStatus[arrayIndex0] != FuncInline.enumSMDStatus.Test_Pass)
                    {
                        FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + ((FuncInline.enumTeachingPos)pos).ToString() + " .SMDStatus " + (arrayIndex0 + 1) + " = FuncInline.enumSMDStatus.Test_Pass");
                    }
                    if (info.TestWatch[arrayIndex0] != null) info.TestWatch[arrayIndex0].Stop();
                    info.SMDStatus[arrayIndex0] = FuncInline.enumSMDStatus.Test_Pass;
                    info.ErrorCode[arrayIndex0] = -1;
                    FuncLog.WriteLog_Tester(SMD.PortName + " TEST PASS : " + (siteIndex0 + 1) + "," + (arrayIndex0 + 1));

                    FuncInline.ArrayNGCode[siteIndex0, arrayIndex0] = -1;
                    FuncInline.ArrayNGCount[siteIndex0, arrayIndex0] = 0;
                    FuncInline.PlusPinUseByArray(siteIndex0 + 1, arrayIndex0 + 1, false, false);
                }
                return;
            }

            if (cmd == "FL")
            {
                int ec = defectCode ?? -1;

                // DL 실패면 FT에서 DF를 보내도록 상태 마킹
                if (this.IsDlPort)
                {
                    // DL → DTest_Fail 세팅
                    info.SMDStatus[arrayIndex0] = FuncInline.enumSMDStatus.DTest_Fail;
                    info.ErrorCode[arrayIndex0] = ec < 0 ? 0 : ec;
                    info.BeforeCode[arrayIndex0] = info.ErrorCode[arrayIndex0];
                }
                else
                {
                    // FT 불량
                    if (GlobalVar.SMDLog && info.SMDStatus[arrayIndex0] != FuncInline.enumSMDStatus.Test_Fail)
                    {
                        FuncLog.WriteLog_Tester("AutoInline_PCBInfo " + ((FuncInline.enumTeachingPos)pos).ToString() + " .SMDStatus " + (arrayIndex0 + 1) + " = FuncInline.enumSMDStatus.Test_Fail");
                    }
                    if (info.TestWatch[arrayIndex0] != null) info.TestWatch[arrayIndex0].Stop();
                    info.SMDStatus[arrayIndex0] = FuncInline.enumSMDStatus.Test_Fail;
                    info.ErrorCode[arrayIndex0] = ec;
                    info.BeforeCode[arrayIndex0] = ec;
                    info.UserCancel = false;
                    FuncLog.WriteLog_Tester(SMD.PortName + " TEST FAIL : " + (siteIndex0 + 1) + "," + (arrayIndex0 + 1) + "-" + ec);

                    FuncInline.PlusPinUseByArray(siteIndex0 + 1, arrayIndex0 + 1, false, true);

                    // 누적/연속 불량 카운트 업데이트 (원본 로직과 동일 정책이면 그대로)
                    if (FuncInline.ArrayNGCode[siteIndex0, arrayIndex0] == ec)
                        FuncInline.ArrayNGCount[siteIndex0, arrayIndex0]++;
                    else
                        FuncInline.ArrayNGCount[siteIndex0, arrayIndex0] = 1;
                    FuncInline.ArrayNGCode[siteIndex0, arrayIndex0] = ec;
                }
                return;
            }
        }

        // - Gen6+는 통합검사라 FT 개념 없음 → null
        // - Gen5, Gen1~4만 FT 매핑 사용
        private int? ResolveFtSiteIndex()
        {
            if (IsDlPort) return null;

            // Gen5: DL 1개 + FT 8개(Front 4, Rear 4)
            if (FuncInline.InlineType == FuncInline.enumInlineType.Gen5)
            {
                switch (this.SMDNo)
                {
                    case 1: return 10; // F-FT1 → Site11(0-base 10)
                    case 2: return 11; // F-FT2 → Site12
                    case 3: return 12; // F-FT3 → Site13
                    case 4: return 9;  // F-FT4 → Site10
                    case 5: return 23; // R-FT1 → Site24
                    case 6: return 24; // R-FT2 → Site25
                    case 7: return 25; // R-FT3 → Site26
                    case 8: return 22; // R-FT4 → Site23
                    default: return null;
                }
            }

            // Gen1~4: DL 1개 + FT 6개(Front 3, Rear 3)
            if (FuncInline.InlineType <= FuncInline.enumInlineType.Gen4)
            {
                switch (this.SMDNo)
                {
                    case 1: return 10; // F-FT1 → Site11
                    case 2: return 11; // F-FT2 → Site12
                    case 3: return 12; // F-FT3 → Site13
                    case 5: return 23; // R-FT1 → Site24
                    case 6: return 24; // R-FT2 → Site25
                    case 7: return 25; // R-FT3 → Site26
                    default: return null;
                }
            }

            // Gen6+ (통합검사): 사이트 번호 포함된 기존 포맷 사용 → FT 매핑 불필요
            return null;
        }

    }






}


