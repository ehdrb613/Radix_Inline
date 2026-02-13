using System;
using System.Drawing;
using System.Windows.Forms;
using Secs4Net;
using System.IO;
using System.Threading;

namespace SecsDevice {
    public partial class FormLog : Form {
        class SecsLogger : SecsTracer {
            readonly FormLog _form;
            internal SecsLogger(FormLog form) {
                _form = form;
                //WriteLog("");
            }
            public override void TraceMessageIn(SecsMessage msg, int systembyte) {
                try
                {
                    if (decodeSecsName(msg.S, msg.F).Contains("Unknown"))
                    {
                        WriteLog(string.Format("<-- [0x{0:X8}] {1}\n", systembyte, decodeSecsName(msg.S, msg.F)));
                    }
                    else
                    {
                        WriteLog(string.Format("<-- [0x{0:X8}] {1}\n", systembyte, msg.ToSML()));
                    }
                    ///*
                    _form.Invoke((MethodInvoker)delegate
                    {
                        _form.richTextBox1.SelectionColor = Color.Black;
                        _form.richTextBox1.AppendText(string.Format("<-- [0x{0:X8}] {1}\n", systembyte, msg.ToSML()));
                        //WriteLog(string.Format("<-- [0x{0:X8}] {1}\n", systembyte, msg.ToSML()));
                    });
                    //*/
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
            }

            public override void TraceMessageOut(SecsMessage msg, int systembyte) {
                try
                {
                    if (decodeSecsName(msg.S, msg.F).Contains("Unknown"))
                    {
                        WriteLog(string.Format("--> [0x{0:X8}] {1}\n", systembyte, decodeSecsName(msg.S, msg.F)));
                    }
                    else
                    {
                        WriteLog(string.Format("--> [0x{0:X8}] {1}\n", systembyte, msg.ToSML()));
                    }
                    //*
                    _form.Invoke((MethodInvoker)delegate
                    {
                        _form.richTextBox1.SelectionColor = Color.Black;
                        _form.richTextBox1.AppendText(string.Format("--> [0x{0:X8}] {1}\n", systembyte, msg.ToSML()));
                        //WriteLog(string.Format("--> [0x{0:X8}] {1}\n", systembyte, msg.ToSML()));
                    });
                    //*/
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
            }

            public override void TraceInfo(string msg) {
                try
                {
                    _form.Invoke((MethodInvoker)delegate
                    {
                        _form.richTextBox1.SelectionColor = Color.Blue;
                        _form.richTextBox1.AppendText(msg + Environment.NewLine);
                        WriteLog(msg);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
            }

            public override void TraceWarning(string msg) {
                try
                {
                    _form.Invoke((MethodInvoker)delegate
                    {
                        _form.richTextBox1.SelectionColor = Color.Green;
                        _form.richTextBox1.AppendText(msg + Environment.NewLine);
                        WriteLog(msg);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
            }

            public override void TraceError(string msg) {
                try
                {
                    _form.Invoke((MethodInvoker)delegate
                    {
                        _form.richTextBox1.SelectionColor = Color.Red;
                        _form.richTextBox1.AppendText(msg + Environment.NewLine);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
            }

            private void WriteLog(string text) // WriteLog(문자열) 로그 저장
            {
                try
                {

                    // 로그는 날짜별로 텍스트 파일로 저장함
                    string logPath = "C:\\FA";
                    if (!Directory.Exists(logPath))
                    {
                        Directory.CreateDirectory(logPath);
                    }
                    logPath += "\\AXT_Label";
                    if (!Directory.Exists(logPath))
                    {
                        Directory.CreateDirectory(logPath);
                    }
                    logPath += "\\SecsLog";
                    if (!Directory.Exists(logPath))
                    {
                        Directory.CreateDirectory(logPath);
                    }
                    logPath += "\\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    if (!File.Exists(logPath))
                    {
                        File.Create(logPath);
                    }
                    string logText = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text;

                    WriteFile(logPath, logText);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
            }

            public string decodeSecsName(byte s, byte f) // YJ추가. SECS GEM 통신시 stream과 function에 따른 이름을 리턴. 유효성 검사에도 이용.
            {
                string name = "Unknown"; // 모든 stream + function 0

                if (s == 1)
                {
                    switch (f)
                    {
                        case 1:
                            name = "AreYouThrere";
                            break;
                        case 2:
                            name = "OnLineData";
                            break;
                        case 3:
                            name = "SelectedEquipmentStatusRequest";
                            break;
                        case 4:
                            name = "SelectedEquipmentStatusData";
                            break;
                        //case 11:
                        //    name = "StatusVariableNameListRequest";
                        //    break;
                        //case 12:
                        //    name = "StatusVariableNameListReply";
                        //    break;
                        case 13:
                            name = "EstablishCommunicationRequest";
                            break;
                        case 14:
                            name = "EstablishCommunicationRequestAck";
                            break;
                        //case 15:
                        //    name = "ReqeustOffline";
                        //    break;
                        //case 16:
                        //    name = "OfflineAck";
                        //    break;
                        //case 17:
                        //    name = "RequestOnline";
                        //    break;
                        //case 18:
                        //    name = "OnlineAck";
                        //    break;
                        case 0:
                            name = "AbortTransaction";
                            break;
                        default:
                            name = "UnknownFunction";
                            break;
                    }
                }
                else if (s == 2)
                {
                    switch (f)
                    {
                        //case 13:
                        //    name = "EquipmentConstantRequest";
                        //    break;
                        //case 14:
                        //    name = "EquipmentConstantData";
                        //    break;
                        //case 15:
                        //    name = "NewEquipmentConstantSend";
                        //    break;
                        //case 16:
                        //    name = "NewEquipmentConstantAck";
                        //    break;
                        //case 17:
                        //    name = "DateTimeRequest";
                        //    break;
                        //case 18:
                        //    name = "DateTimeData";
                        //    break;
                        //case 23:
                        //    name = "TraceInitializeSend";
                        //    break;
                        //case 24:
                        //    name = "TraceInitializeAck";
                        //    break;
                        //case 29:
                        //    name = "EquipmentConstantNameListRequest";
                        //    break;
                        //case 30:
                        //    name = "EquipmentConstantNameList";
                        //    break;
                        case 31:
                            name = "DateTimeRequest";
                            break;
                        case 32:
                            name = "DateTimeAck";
                            break;
                        case 33:
                            name = "DefineReport";
                            break;
                        case 34:
                            name = "DefineReportAck";
                            break;
                        case 35:
                            name = "LinkEventReport";
                            break;
                        case 36:
                            name = "LinkEventReportAck";
                            break;
                        case 37:
                            name = "EnableDisableEventReport";
                            break;
                        case 38:
                            name = "EnableDisableEventReportAck";
                            break;
                        //case 39:
                        //    name = "MultiBlockInquire";
                        //    break;
                        //case 40:
                        //    name = "MultiBlockGrant";
                        //    break;
                        case 41:
                            name = "HostCommandSend";
                            break;
                        case 42:
                            name = "HostCommandAck";
                            break;
                        //case 43:
                        //    name = "ResetSpoolingStreamsFunctions";
                        //    break;
                        //case 44:
                        //    name = "ResetSpoolingAcknowledge";
                        //    break;
                        //case 49:
                        //    name = "EnhancedRemoteCommand";
                        //    break;
                        //case 50:
                        //    name = "EnhancedRemoteCommandAck";
                        //    break;
                        case 0:
                            name = "AbortTransaction";
                            break;
                        default:
                            name = "UnknownFunction";
                            break;
                    }
                }
                //else if (s == 3)
                //{
                //    switch (f)
                //    {
                //        case 17:
                //            name = "CarrierActionRequest";
                //            break;
                //        case 18:
                //            name = "CarrierActionAcknowledge";
                //            break;
                //        //case 19:
                //        //    name = "";
                //        //    break;
                //        //case 20:
                //        //    name = "";
                //        //    break;
                //        case 25:
                //            name = "PortAction";
                //            break;
                //        case 26:
                //            name = "PortActionAcknowledge";
                //            break;
                //        case 27:
                //            name = "ChangeAccess";
                //            break;
                //        case 28:
                //            name = "ChangeAccessAcknowledge";
                //            break;
                //        case 0:
                //            name = "AbortTransaction";
                //            break;
                //    }
                //}
                else if (s == 5)
                {
                    switch (f)
                    {
                        case 1:
                            name = "AlarmReportSend";
                            break;
                        case 2:
                            name = "AlarmReportAck";
                            break;
                        case 3:
                            name = "EnableDisableAlarmSend";
                            break;
                        case 4:
                            name = "EnableDisableAlarmAck";
                            break;
                        //case 5:
                        //    name = "ListAlarmRequest";
                        //    break;
                        //case 6:
                        //    name = "ListAlarmData";
                        //    break;
                        //case 7:
                        //    name = "ListEnabledAlarmRequest";
                        //    break;
                        //case 8:
                        //    name = "ListEnabledAlarmData";
                        //    break;
                        case 0:
                            name = "AbortTransaction";
                            break;
                        default:
                            name = "UnknownFunction";
                            break;
                    }
                }
                else if (s == 6)
                {
                    switch (f)
                    {
                        //case 1:
                        //    name = "TraceDataSend";
                        //    break;
                        //case 2:
                        //    name = "TraceDataAck";
                        //    break;
                        //case 5:
                        //    name = "";
                        //    break;
                        //case 6:
                        //    name = "";
                        //    break;
                        case 11:
                            name = "EventReportSend";
                            break;
                        case 12:
                            name = "EventReportSendAck";
                            break;
                        //case 15:
                        //    name = "EventReportRequest";
                        //    break;
                        //case 16:
                        //    name = "EventReportData";
                        //    break;
                        //case 19:
                        //    name = "IndividualEventReportRequest";
                        //    break;
                        //case 20:
                        //    name = "IndividualEventReportData";
                        //    break;
                        //case 23:
                        //    name = "RequestSpoolData";
                        //    break;
                        //case 24:
                        //    name = "RequestSpoolDataAck";
                        //    break;
                        case 0:
                            name = "AbortTransaction";
                            break;
                        default:
                            name = "UnknownFunction";
                            break;
                    }
                }
                else if (s == 7)
                {
                    switch (f)
                    {
                        //case 1:
                        //    name = "ProcessProgramLoadInquire";
                        //    break;
                        //case 2:
                        //    name = "ProcessProgramLoadGrant";
                        //    break;
                        case 3:
                            name = "ProcessProgramSend";
                            break;
                        case 4:
                            name = "ProcessProgramAck";
                            break;
                        //case 5:
                        //    name = "ProcessProgramRequest";
                        //    break;
                        //case 6:
                        //    name = "ProcessProgramData";
                        //    break;
                        //case 17:
                        //    name = "DeleteProcessProgramSend";
                        //    break;
                        //case 18:
                        //    name = "DeleteProcessProgramSendAck";
                        //    break;
                        case 19:
                            name = "CurrentEPPDRequestReport";
                            break;
                        case 20:
                            name = "CurrentEPPDData";
                            break;
                        //case 23:
                        //    name = "FormattedProcessProgramSend";
                        //    break;
                        //case 24:
                        //    name = "FormattedProcessProgramAck";
                        //    break;
                        //case 25:
                        //    name = "FormattedProcessProgramRequest";
                        //    break;
                        //case 26:
                        //    name = "FormattedProcessProgramData";
                        //    break;
                        //case 27:
                        //    name = "ProcessProgramVerificationSend";
                        //    break;
                        //case 28:
                        //    name = "ProcessProgramVerificationAck";
                        //    break;
                        //case 29:
                        //    name = "ProcessProgramVerificationInquire";
                        //    break;
                        //case 30:
                        //    name = "ProcessProgramVerificationGrant";
                        //    break;
                        case 0:
                            name = "AbortTransaction";
                            break;
                        default:
                            name = "UnknownFunction";
                            break;
                    }
                }
                else if (s == 9)
                {
                    switch (f)
                    {
                        case 1:
                            name = "UnrecognizedDeviceID";
                            break;
                        case 3:
                            name = "UnrecognizedStreamType";
                            break;
                        case 5:
                            name = "UnrecognizedFunctionType";
                            break;
                        case 7:
                            name = "IllegalData";
                            break;
                        case 9:
                            name = "TransactionTimerTimeout";
                            break;
                        case 11:
                            name = "DataTooLong";
                            break;
                        case 13:
                            name = "ConversationTimeout";
                            break;
                        case 0:
                            name = "AbortTransaction";
                            break;
                        default:
                            name = "UnknownFunction";
                            break;
                    }
                }
                else if (s == 10)
                {
                    switch (f)
                    {
                        //case 1:
                        //    name = "TerminalRequest";
                        //    break;
                        //case 2:
                        //    name = "TerminalRequestAck";
                        //    break;
                        case 3:
                            name = "TerminalDisplaySingle";
                            break;
                        case 4:
                            name = "TerminalDisplaySingleAck";
                            break;
                        //case 5:
                        //    name = "TerminalDisplayMulti";
                        //    break;
                        //case 6:
                        //    name = "TerminalDisplayMultiAcknowledge";
                        //    break;
                        //case 7:
                        //    name = "MultiBlockNotAllowed";
                        //    break;
                        case 0:
                            name = "AbortTransaction";
                            break;
                        default:
                            name = "UnknownFunction";
                            break;
                    }
                }
                //if (s == 14)
                //{
                //    switch (f)
                //    {
                //        case 1:
                //            name = "GetAttributeRequest";
                //            break;
                //        case 2:
                //            name = "GetAttributeData";
                //            break;
                //        case 3:
                //            name = "SetAttributeRequest";
                //            break;
                //        case 4:
                //            name = "SetAttributeData";
                //            break;
                //        case 7:
                //            name = "GetAttributeName";
                //            break;
                //        case 8:
                //            name = "GetAttributeNameData";
                //            break;
                //        case 9:
                //            name = "ControlJobCreate";
                //            break;
                //        case 10:
                //            name = "ControlJobCreateAcknowledge";
                //            break;
                //        case 11:
                //            name = "ControlJobDeleteRequest";
                //            break;
                //        case 12:
                //            name = "ControlJobDeleteAcknowledge";
                //            break;
                //        case 0:
                //            name = "AbortTransaction";
                //            break;
                //    }
                //}
                //if (s == 16)
                //{
                //    switch (f)
                //    {
                //        case 5:
                //            name = "ProcessJobCommand";
                //            break;
                //        case 6:
                //            name = "ProcessJobCommandAcknowledge";
                //            break;
                //        case 11:
                //            name = "ProcessJobCreateEnh";
                //            break;
                //        case 12:
                //            name = "ProcessJobCreateEnhAcknowledge";
                //            break;
                //        case 15:
                //            name = "ProcessJobMultiCreate";
                //            break;
                //        case 16:
                //            name = "ProcessJobMultiCreateAcknowledge";
                //            break;
                //        case 17:
                //            name = "ProcessJobDequeue";
                //            break;
                //        case 18:
                //            name = "ProcessJobDequeueCreate";
                //            break;
                //        case 19:
                //            name = "PRGetAllJobs";
                //            break;
                //        case 20:
                //            name = "PRGetAllJobsSend";
                //            break;
                //        case 21:
                //            name = "PRGetSpace";
                //            break;
                //        case 22:
                //            name = "PRGetAllJobsSend";
                //            break;
                //        //case 25:
                //        //    name = "";
                //        //    break;
                //        //case 26:
                //        //    name = "";
                //        //    break;
                //        case 27:
                //            name = "ControlJobCommandRequest";
                //            break;
                //        case 28:
                //            name = "ControlJobCommandAcknowledge";
                //            break;
                //        case 0:
                //            name = "AbortTransaction";
                //            break;
                //    }
                //}
                else
                {
                    name = "UnknownStream";
                }

                //Console.WriteLine("S" + s + "F" + f + " : " + name);

                return name;
            }

            private void WriteFile(string path, string text) // WriteFile(경로,문자열) 테스트파일에 문자열 추가 저장 
            {
                //string path = @"c:\temp\MyTest.txt";
                // This text is added only once to the file.
                StreamWriter sw;
                try
                {
                    if (!File.Exists(path))
                    {
                        // 파일이 없으면 새로 생성해서 빈 라인을 저장
                        using (sw = File.CreateText(path))
                        {
                            sw.WriteLine("");
                        }
                        sw.Close();
                        Thread.Sleep(100);
                    }
                    using (sw = File.AppendText(path))
                    {
                        sw.WriteLine(text);
                    }
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public readonly SecsTracer Logger;

        public FormLog() {
            InitializeComponent();

            Logger = new SecsLogger(this);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }


    }
}
