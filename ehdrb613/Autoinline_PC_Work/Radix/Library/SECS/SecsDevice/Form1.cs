using System;
using System.ComponentModel;
using System.Windows.Forms;
using Secs4Net;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SecsDevice {
    public partial class Form1 : Form {
        SecsGem _secsGem;
        readonly FormLog _logform = new FormLog();
        readonly BindingList<RecvMessage> recvBuffer = new BindingList<RecvMessage>();
        private uint dataId = 1;

        private System.Threading.Timer timerSecs; // Thread Timer
        private bool closeThread = false;
        private int timerMethod = 0;
        private bool established = false;
        private bool processing = false;

        private int product_id = 1;

        public Form1() {
            InitializeComponent();

            radioActiveMode.DataBindings.Add("Enabled", btnEnable, "Enabled");
            radioPassiveMode.DataBindings.Add("Enabled", btnEnable, "Enabled");
            txtAddress.DataBindings.Add("Enabled", btnEnable, "Enabled");
            numPort.DataBindings.Add("Enabled", btnEnable, "Enabled");
            numDeviceId.DataBindings.Add("Enabled", btnEnable, "Enabled");
            recvMessageBindingSource.DataSource = recvBuffer;
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            #region 화면 제어용 쓰레드 타이머 시작
            TimerCallback CallBackUI = new TimerCallback(TimerSecs);
            timerSecs = new System.Threading.Timer(CallBackUI, false, 0, 500);
            #endregion
        }

        private void TimerSecs(Object state) // 화면 제어 쓰레드 타이머 함수
        {
            while (!closeThread) {

                if (_secsGem != null &&
                    _secsGem.State == ConnectionState.Selected &&
                    timerMethod > 0)
                {
                    try
                    {

                        /* 화면 변경 timer */
                        //this.Invoke(new MethodInvoker(delegate ()
                        //{
                            switch (timerMethod)
                            {
                                case 1:
                                    #region Mode Change to Online Local
                                    #region Establish Communication Request 대기
                                    while (!established)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    #endregion
                                    #region Are You There
                                    btnRUThere_Click(new object(), new EventArgs());
                                    #endregion
                                    #endregion
                                    break;
                                case 2:
                                    #region Mode Change to Offline
                                    #region Establish Communication Request 대기
                                    #endregion
                                    #endregion
                                    break;
                                case 3:
                                    #region Error in Mode Changes
                                    #endregion
                                    break;
                                case 4:
                                    #region Normal Operation
                                    #endregion
                                    break;
                                case 5:
                                    #region Normal Operation when spec. out at first and spec. In at ReInspection
                                    #endregion
                                    break;
                                case 6:
                                    #region Plate Inspection Period NG
                                    #endregion
                                    break;
                                case 7:
                                    #region NG’d Plate inserted
                                    #endregion
                                    break;
                                case 8:
                                    #region Spec. Out at ReInspection in Normal Operation
                                    #endregion
                                    break;
                                case 9:
                                    #region In case that it is impossible to recover
                                    #endregion
                                    break;
                            }

                        //}));

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine(ex.StackTrace);
                    }
                    timerMethod = 0;
                }

                Thread.Sleep(100);
            }

        }


        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            MessageBox.Show(e.Exception.ToString());
        }

        private void btnEnable_Click(object sender, EventArgs e) {
            if (_secsGem != null)
                _secsGem.Dispose();


            _secsGem = new SecsGem(IPAddress.Parse(txtAddress.Text), (int)numPort.Value,
                radioActiveMode.Checked,
                (primaryMsg, reply) => {
                    this.Invoke((MethodInvoker)delegate {
                        recvBuffer.Add(new RecvMessage {
                            Msg = primaryMsg,
                            ReplyAction = reply
                        });
                        #region primary message 에 따른 동작
                        Thread.Sleep(100);
                        switch (primaryMsg.S)
                        {
                            case 1:
                                switch (primaryMsg.F)
                                {
                                    case 1:// Are You There
                                        //break; // sencondary 응답 테스트로 막음
                                    case 5:// Request Offline
                                    case 13:// Establish Communication Request
                                        established = true;

                                        lstUnreplyMsg.SelectedIndex = 0;

                                        lstUnreplyMsg_SelectedIndexChanged(new object(), new EventArgs());
                                        btnReplySecondary_Click(new object(), new EventArgs());
                                        break;
                                }
                                break;
                            case 2:
                                switch (primaryMsg.F)
                                {
                                    case 33:// Define Report
                                    case 35:// Link Event Report
                                    case 37:// Enable/Disable Event
                                        established = true;

                                        lstUnreplyMsg.SelectedIndex = 0;

                                        lstUnreplyMsg_SelectedIndexChanged(new object(), new EventArgs());
                                        btnReplySecondary_Click(new object(), new EventArgs());
                                        break;
                                }
                                break;
                            case 5:
                                switch (primaryMsg.F)
                                {
                                    case 1:// Alarm Report
                                        established = true;

                                        lstUnreplyMsg.SelectedIndex = 0;

                                        lstUnreplyMsg_SelectedIndexChanged(new object(), new EventArgs());
                                        btnReplySecondary_Click(new object(), new EventArgs());
                                        break;
                                }
                                break;
                            case 6:
                                switch (primaryMsg.F)
                                {
                                    case 11:// Event Report
                                        established = true;

                                        lstUnreplyMsg.SelectedIndex = 0;

                                        lstUnreplyMsg_SelectedIndexChanged(new object(), new EventArgs());
                                        btnReplySecondary_Click(new object(), new EventArgs());
                                        break;
                                }
                                break;
                            case 7:
                                switch (primaryMsg.F)
                                {
                                    case 3:// Process Program Send
                                        established = true;
                                        try
                                        {
                                            /*
                                            for (int i = 0; i < primaryMsg.SecsItem.Items.Count; i++)
                                            {
                                                Console.WriteLine("process program : " + primaryMsg.SecsItem.Items[i]);
                                            }
                                            //*/
                                            lstUnreplyMsg.SelectedIndex = 0;

                                            lstUnreplyMsg_SelectedIndexChanged(new object(), new EventArgs());
                                            btnReplySecondary_Click(new object(), new EventArgs());
                                        }
                                        catch (Exception ex)
                                        {
                                            // 파싱 에러 서버로 통보
                                            //S9F7_IllegalData();
                                            Console.WriteLine(ex.ToString());
                                            Console.WriteLine(ex.StackTrace);
                                            break;
                                        }
                                        break;
                                    case 4:
                                        break;

                                    case 19:// Current EPPD Request (PER)
                                        established = true;

                                        lstUnreplyMsg.SelectedIndex = 0;

                                        lstUnreplyMsg_SelectedIndexChanged(new object(), new EventArgs());
                                        btnReplySecondary_Click(new object(), new EventArgs());
                                        break;
                                    case 20:
                                        break;

                                    case 0:// Abort Transaction
                                        break;

                                    default: // 해당 stream 없음
                                        //S9F3_UnrecognizedStreamType();
                                        break;
                                }
                                break;

                            case 10:
                                switch (primaryMsg.F)
                                {
                                    case 3:// Terminal Message
                                        established = true;

                                        lstUnreplyMsg.SelectedIndex = 0;

                                        lstUnreplyMsg_SelectedIndexChanged(new object(), new EventArgs());
                                        btnReplySecondary_Click(new object(), new EventArgs());
                                        break;
                                }
                                break;
                        }
                        #endregion
                    });
                },
                _logform.Logger, 0);

            _secsGem.ConnectionChanged += delegate {
                this.Invoke((MethodInvoker)delegate {
                    lbStatus.Text = _secsGem.State.ToString();
                });
            };

            btnEnable.Enabled = false;
            btnDisable.Enabled = true;
        }

        private void btnDisable_Click(object sender, EventArgs e) {
            if (_secsGem != null) {
                _secsGem.Dispose();
                _secsGem = null;
            }
            btnEnable.Enabled = true;
            btnDisable.Enabled = false;
            lbStatus.Text = "Disable";
            recvBuffer.Clear();
        }

        private void Form1_Load(object sender, EventArgs e) {
            _logform.Show(this);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            closeThread = true;
            _logform.Close();
            timerSecs.Dispose();
        }

        private void btnSendPrimary_Click(object sender, EventArgs e) {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            var msg = txtSendPrimary.Text.ToSecsMessage();
            _secsGem.BeginSend(msg, ar => {
                try {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                } catch (SecsException ex) {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void lstUnreplyMsg_SelectedIndexChanged(object sender, EventArgs e) {
            RecvMessage recv = lstUnreplyMsg.SelectedItem as RecvMessage;
            if (recv == null)
                return;
            txtRecvPrimary.Text = recv.Msg.ToSML();
            processing = false;
        }

        private void btnReplySecondary_Click(object sender, EventArgs e) {
            RecvMessage recv = lstUnreplyMsg.SelectedItem as RecvMessage;
            if (recv == null)
                return;

            txtReplySeconary.Text = "";

            switch (recv.Msg.S)
            {
                case 1:
                    switch (recv.Msg.F)
                    {
                        case 1: // Are You There
                        case 13: // Establish Communication Request

                        case 15: // Offline
                            txtReplySeconary.Text = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B(0)).ToSML();
                            break;
                    }
                    break;
                case 2:
                    switch (recv.Msg.F)
                    {
                        case 33: // Define Report
                        case 35: // Link Event Report
                        case 37: // Enable/Disable Event
                            txtReplySeconary.Text = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B(0)).ToSML();
                            break;
                    }
                    break;
                case 5:
                    switch (recv.Msg.F)
                    {
                        case 1: // Alarm Report
                            txtReplySeconary.Text = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B(0)).ToSML();
                            break;
                    }
                    break;
                case 6:
                    switch (recv.Msg.F)
                    {
                        case 11: // EventReport
                            txtReplySeconary.Text = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B(0)).ToSML();
                            break;
                    }
                    break;
                case 7:
                    switch (recv.Msg.F)
                    {
                        case 3: // Process Program Send
                            txtReplySeconary.Text = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B(0)).ToSML();
                            break;
                        case 19: // Current EEPD
                            txtReplySeconary.Text = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B(0)).ToSML();
                            break;
                    }
                    break;
                case 10:
                    switch (recv.Msg.F)
                    {
                        case 3: // Send Terminal Message
                            txtReplySeconary.Text = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B(0)).ToSML();
                            break;
                    }
                    break;
            }

            if (string.IsNullOrEmpty(txtReplySeconary.Text))
                return;

            recv.ReplyAction(txtReplySeconary.Text.ToSecsMessage());
            //_secsGem.Send(txtReplySeconary.Text.ToSecsMessage());
            recvBuffer.Remove(recv);
            txtRecvPrimary.Clear();
            processing = false;
        }

        string decodeSecsName(byte s, byte f)
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

        string getAlarmText(int index)
        {
            switch (index)
            {
                case 0: return "No Error";
                case 1: return "Unknown Error";
                case 2: return "E_Stop Pressed";
                case 3: return "Door Opened";
                case 4: return "Servo Disabled";
                case 5: return "PLC Communication Failed";
                case 6: return "Inspection Sensor Communication Failed";
                case 7: return "System Initial Error";
                case 8: return "System Not Inited";
                case 9: return "No Plate Table";
                case 10: return "No Plate In Slot";
                case 11: return "Inspection Timed out";
                case 12: return "No Inspection Data";
                case 13: return "Inspect Period Exceeded";
                case 14: return "Inspection NG’d Plate";
            }

            return "No Error";
        }

        private void btnEstablishCommunication_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }
            Item item = Item.L(
                );


            var msg = new SecsMessage(1, 13, decodeSecsName(1,13), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnRUThere_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            var msg = new SecsMessage(1, 1, decodeSecsName(1,1));
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnDisableEvent_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(1),
                Item.L()
            );

            var msg = new SecsMessage(2, 37, decodeSecsName(2,37), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnDeleteAllReport_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(1),
                Item.L()
            );

            var msg = new SecsMessage(2, 33, decodeSecsName(2,33), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnEnableAllEvent_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(1),
                Item.L(
                    Item.U4(1),
                    Item.U4(2),
                    Item.U4(3),
                    Item.U4(4),
                    Item.U4(5),
                    Item.U4(6),
                    Item.U4(7),
                    Item.U4(8),
                    Item.U4(9),
                    Item.U4(10),
                    Item.U4(11),
                    Item.U4(12),
                    Item.U4(13),
                    Item.U4(14)
                )
            );

            var msg = new SecsMessage(2, 37, decodeSecsName(2,37), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnDefineReport_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(1), // dataID, 0 fixed
                Item.L(
                    Item.L( // Control State Change
                        Item.U4(1), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1001) // Control State
                        )
                    ),
                    Item.L( // Process State Change Report
                        Item.U4(2), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1009), // Process State
                            Item.U4(1006), // LOT ID
                            Item.U4(1007) // Product ID
                        )
                    ),
                    Item.L( // Equipment State Change
                        Item.U4(3), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1008) // Equipment State
                        )
                    ),
                    Item.L( // TRS Mode Change
                        Item.U4(4), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1002) // TRS Mode
                        )
                    ),
                    Item.L( // Recipe Change
                        Item.U4(5), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1011) // Recipe ID
                        )
                    ),
                    Item.L( // Port State Change
                        Item.U4(6), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1012) // Port State List
                        )
                    ),
                    Item.L( // Recipe List Change Report
                        Item.U4(7), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1007) // Recipe ID List
                        )
                    ),
                    Item.L( // Product Process Data Report
                        Item.U4(8), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1007)
                        )
                    ),
                    Item.L( // Scrap Report
                        Item.U4(9), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1007)
                        )
                    ),
                    Item.L( // Product Abort Report
                        Item.U4(10), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1007) // Product ID
                        )
                    ),
                    Item.L( // Product Process Start
                        Item.U4(11), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1007), // Product ID
                            Item.U4(1016) // Product ID
                        )
                    ),
                    Item.L( // Product Cancel Report
                        Item.U4(12), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1013) // Product ID
                        )
                    ),
                    Item.L( // Product ID Report
                        Item.U4(13), // Report ID
                        Item.L(
                            Item.U4(1003), // EQP ID
                            Item.U4(1004), // Operator ID
                            Item.U4(1005), // Time
                            Item.U4(1006), // Time
                            Item.U4(1007), // Time
                            Item.U4(1012), // Time
                            Item.U4(1010), // Time
                            Item.U4(1014), // Time
                            Item.U4(1015) // Product ID
                        )
                    ),
                    Item.L( // Product List Report
                        Item.U4(14), // Report ID
                        Item.L(
                            Item.U4(1017) // Product List
                        )
                    )

                )
            );

            var msg = new SecsMessage(2, 33, decodeSecsName(2,33), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnLinkEventReport_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(1), // dataID, 0 fixed
                Item.L(
                    Item.L( // Change to Offline
                        Item.U4(1), // CEID
                        Item.L(
                            Item.U4(1) //ReportID
                        )
                    ),
                    Item.L( // Change to Local
                        Item.U4(2), // CEID
                        Item.L(
                            Item.U4(2) //ReportID
                        )
                    ),
                    Item.L( // SlotMap Data
                        Item.U4(3), // CEID
                        Item.L(
                            Item.U4(3) //ReportID
                        )
                    ),
                    Item.L( // NG'd Plate Loaded
                        Item.U4(4), // CEID
                        Item.L(
                            Item.U4(4) //ReportID
                        )
                    ),
                    Item.L( // Inspection Started
                        Item.U4(5), // CEID
                        Item.L(
                            Item.U4(5) //Report ID
                        )
                    ),
                    Item.L( // Inspection Complete
                        Item.U4(6), // CEID
                        Item.L(
                            Item.U4(6) //Report ID
                        )
                    ),
                    Item.L( // Reinspection Started
                        Item.U4(7), // CEID
                        Item.L(
                            Item.U4(7) //Report ID
                        )
                    ),
                    Item.L( // Reinspection Complete
                        Item.U4(8), // CEID
                        Item.L(
                            Item.U4(8) //Report ID
                        )
                    ),
                    Item.L( // Plate Inspection Information
                        Item.U4(9), // CEID
                        Item.L(
                            Item.U4(9) //Report ID
                        )
                    ),
                    Item.L( // Aborted
                        Item.U4(10), // CEID
                        Item.L(
                            Item.U4(10) //Report ID
                        )
                    ),
                    Item.L( // Alarm Information
                        Item.U4(11), // CEID
                        Item.L(
                            Item.U4(11) //Report
                        )
                    ),
                    Item.L( // Alarm Information
                        Item.U4(12), // CEID
                        Item.L(
                            Item.U4(12) //Report
                        )
                    ),
                    Item.L( // Alarm Information
                        Item.U4(13), // CEID
                        Item.L(
                            Item.U4(13) //Report
                        )
                    ),
                    Item.L( // Product List
                        Item.U4(14), // CEID
                        Item.L(
                            Item.U4(14) //Report
                        )
                    )

                )
            );

            var msg = new SecsMessage(2, 35, decodeSecsName(2,35), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnTerminalMessage_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.B(0),
                Item.A("Terminal\n Message")
            );

            var msg = new SecsMessage(10, 3, decodeSecsName(10,3), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);

        }

        private void btnEquipmentStatusRequest_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                    Item.U4(1001),
                    Item.U4(1008),
                    Item.U4(1009),
                    Item.U4(1002)
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(1, 3, decodeSecsName(1,3), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnDateTime_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            //Item item = Item.A(DateTime.Now.ToString("yyyyMMddHHmmss"));            
            Item item = Item.A(DateTime.Now.ToString("20210901010101")); // YJ20210914 파싱에러 테스트용 복원

            var msg = new SecsMessage(2, 31, decodeSecsName(2, 31), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnProcessProgramSend_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                    Item.A("Recipe1"),
                    Item.A("Recipe2")
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(7, 3, decodeSecsName(7, 3), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnCurrentEPPDRequest_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(7, 19, decodeSecsName(7, 19), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnEnableAlarmReport_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                    Item.B((byte)0x80),
                    Item.L()
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(5, 3, decodeSecsName(5, 3), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnDisableAlarmReport_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                    Item.B((byte)0x00),
                    Item.L()
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(5, 3, decodeSecsName(5, 3), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnRemoteCommandStart_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                    Item.A("START"),
                    Item.L(
                        Item.L(
                            Item.A("LOTID"),
                            Item.A(tbProductID.Text)
                        ),
                        Item.L(
                            Item.A("PPID"),
                            Item.A(tbRecipeID.Text)
                        ),
                        Item.L(
                            Item.A("PRODUCTID"),
                            Item.A(tbProductID.Text)
                        )
                    )
            );

            var msg = new SecsMessage(2, 41, decodeSecsName(2, 41), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void btnRemoteCommandCancel_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                    Item.A("CANCEL"),
                    Item.L(
                        Item.L(
                            Item.A("PRODUCTID"),
                            Item.A(tbProductID.Text)
                        ),
                        Item.L(
                            Item.A("CODE"),
                            Item.A(tbCancelCode.Text)
                        ),
                        Item.L(
                            Item.A("TEXT"),
                            Item.A(tbCancelText.Text)
                        )
                    )
            );

            var msg = new SecsMessage(2, 41, decodeSecsName(2, 41), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(1), // dataID, 1 fixed
                Item.L(
                )
            );

            var msg = new SecsMessage(2, 35, decodeSecsName(2, 35), item);
            _secsGem.BeginSend(msg, ar => {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = reply.ToSML();
                    });
                }
                catch (SecsException ex)
                {
                    this.Invoke((MethodInvoker)delegate {
                        txtRecvSecondary.Text = ex.Message;
                    });
                }
            }, null);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _secsGem.Send(new SecsMessage(18, 1, decodeSecsName(18, 1), false, Item.L()));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _secsGem.Send(new SecsMessage(1, 97, decodeSecsName(1, 97), false, Item.L()));
        }

    }

    public sealed class RecvMessage {
        public SecsMessage Msg { get; set; }
        public Action<SecsMessage> ReplyAction { get; set; }
    }
}
