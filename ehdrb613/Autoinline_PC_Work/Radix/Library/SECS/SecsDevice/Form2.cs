using System;
using System.ComponentModel;
using System.Windows.Forms;
using Secs4Net;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SecsDevice {
    public partial class Form2 : Form {
        SecsGem _secsGem;
        readonly FormLog _logform = new FormLog();
        readonly BindingList<RecvMessage> recvBuffer = new BindingList<RecvMessage>();
        private uint dataId = 1;

        private System.Threading.Timer timerSecs; // Thread Timer
        private bool closeThread = false;
        private int timerMethod = 0;
        private bool established = false;
        private bool processing = false;



        public Form2() {
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

        private void btnEstablishCommunication_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }
            Item item = Item.L(
                    //Item.A("WorkPlateInspection"),
                    //Item.A("1.01")
                );


            var msg = new SecsMessage(1, 13, "EstablishCommunication", item);
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

            var msg = new SecsMessage(1, 1, "AreYouThere");
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

        private void btnChangeLocal_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(dataId),
                Item.U4(12),
                Item.L()
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "SendReport", item);
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

        private void btnReportOffline_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(dataId),
                Item.U4(11),
                Item.L()
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "SendReport", item);
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

        private void btnDefineReportAck_Click(object sender, EventArgs e)
        {
            Item item = Item.B(0);
            var msg = new SecsMessage(2, 34, "DefineReport", item);
            _secsGem.Send(msg);
        }

        private void btnLinkReportAck_Click(object sender, EventArgs e)
        {
            Item item = Item.B(0);
            var msg = new SecsMessage(2, 36, "LinkReport", item);
            _secsGem.Send(msg);
        }

        private void btnEnableReportAck_Click(object sender, EventArgs e)
        {
            Item item = Item.B(0);
            var msg = new SecsMessage(2, 38, "EnableReport", item);
            _secsGem.Send(msg);
        }

        private void btnTerminalReportAck_Click(object sender, EventArgs e)
        {
            Item item = Item.B(0);
            var msg = new SecsMessage(10, 4, "TerminalMessage", item);
            _secsGem.Send(msg);
        }

        private void btnPlateStart_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(dataId),
                Item.U4(22),
                Item.L(
                    Item.U4((byte)numSlotNo.Value), // SlotNo
                    Item.A("BS000" + ((byte)numSlotNo.Value).ToString()) // PlateID
                )
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "SendReport", item);
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

        string decodeSecsName(byte s, byte f)
        {
            string name = "Unknown";

            if (s == 1)
            {
                switch (f)
                {
                    case 1:
                        name = "AreYouThrereRequest";
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
                    case 11:
                        name = "StatusVariableNameListRequest";
                        break;
                    case 12:
                        name = "StatusVariableNameListReply";
                        break;
                    case 13:
                        name = "EstablishCommunicationRequest";
                        break;
                    case 14:
                        name = "EstablishCommunicationAcknowledge";
                        break;
                    case 15:
                        name = "RequestOffline";
                        break;
                    case 16:
                        name = "Request";
                        break;
                    case 17:
                        name = "RequestOnline";
                        break;
                    case 18:
                        name = "RequestOnlineAcknowledge";
                        break;
                }
            }
            if (s == 2)
            {
                switch (f)
                {
                    case 13:
                        name = "EquipmentConstantRequest";
                        break;
                    case 14:
                        name = "EquipmentConstantData";
                        break;
                    case 15:
                        name = "NewEquipmentConstantSend";
                        break;
                    case 16:
                        name = "NewEquipmentConstantAcknowledge";
                        break;
                    case 17:
                        name = "DateTimeRequest";
                        break;
                    case 18:
                        name = "DateTimeData";
                        break;
                    case 23:
                        name = "TraceInitializeSend";
                        break;
                    case 24:
                        name = "TraceInitializeAcknowledge";
                        break;
                    case 29:
                        name = "EquipmentConstantNameListRequest";
                        break;
                    case 30:
                        name = "EquipmentConstantNameList";
                        break;
                    case 31:
                        name = "DateTimeSetRequest";
                        break;
                    case 32:
                        name = "DateTimeSetAcknowledge";
                        break;
                    case 33:
                        name = "DefineReport";
                        break;
                    case 34:
                        name = "DefineReportAcknowledge";
                        break;
                    case 35:
                        name = "LinkReportEvent";
                        break;
                    case 36:
                        name = "LinkReportAcknowledge";
                        break;
                    case 37:
                        name = "EnableDisableEventReport";
                        break;
                    case 38:
                        name = "EnableDisableEventReportAcknowledge";
                        break;
                    case 39:
                        name = "MultiBlockInquire";
                        break;
                    case 40:
                        name = "MultiBlockGrant";
                        break;
                    case 41:
                        name = "HostCommandSend";
                        break;
                    case 42:
                        name = "HostCommandAcknowledge";
                        break;
                    case 43:
                        name = "ResetSpoolingStreamsFunctions";
                        break;
                    case 44:
                        name = "ResetSpoolingAcknowledge";
                        break;
                }
            }
            if (s == 3)
            {
                switch (f)
                {
                    case 17:
                        name = "CarrierActionRequest";
                        break;
                    case 18:
                        name = "CarrierActionAcknowledge";
                        break;
                    //case 19:
                    //    name = "";
                    //    break;
                    //case 20:
                    //    name = "";
                    //    break;
                    case 25:
                        name = "PortAction";
                        break;
                    case 26:
                        name = "PortActionAcknowledge";
                        break;
                    case 27:
                        name = "ChangeAccess";
                        break;
                    case 28:
                        name = "ChangeAccessAcknowledge";
                        break;
                }
            }
            if (s == 5)
            {
                switch (f)
                {
                    case 1:
                        name = "AlarmReportSend";
                        break;
                    case 2:
                        name = "AlarmReportAcknowledge";
                        break;
                    case 3:
                        name = "EnableDisableAlarmSend";
                        break;
                    case 4:
                        name = "EnableDisableAlarmAcknowledge";
                        break;
                    case 5:
                        name = "ListAlarmRequest";
                        break;
                    case 6:
                        name = "ListAlarmData";
                        break;
                    case 7:
                        name = "ListEnabledAlarmRequest";
                        break;
                    case 8:
                        name = "ListEnabledAlarmData";
                        break;
                }
            }
            if (s == 6)
            {
                switch (f)
                {
                    case 1:
                        name = "TraceDataSend";
                        break;
                    case 2:
                        name = "EventReportSend";
                        break;
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
                        name = "EventReportAcknowledge";
                        break;
                    case 15:
                        name = "EventReportRequest";
                        break;
                    case 16:
                        name = "EventReportData";
                        break;
                    case 19:
                        name = "IndividualEventReportRequest";
                        break;
                    case 20:
                        name = "IndividualEventReportData";
                        break;
                    case 23:
                        name = "RequestSpoolData";
                        break;
                    case 24:
                        name = "RequestSpoolDataAcknowledge";
                        break;
                }
            }
            if (s == 7)
            {
                switch (f)
                {
                    case 1:
                        name = "ProcessProgramLoadInquire";
                        break;
                    case 2:
                        name = "ProcessProgramLoadGrant";
                        break;
                    //case 3:
                    //    name = "";
                    //    break;
                    //case 4:
                    //    name = "";
                    //    break;
                    //case 5:
                    //    name = "";
                    //    break;
                    //case 6:
                    //    name = "";
                    //    break;
                    case 17:
                        name = "DeleteProcessProgramSend";
                        break;
                    case 18:
                        name = "DeleteProcessProgramAcknowledge";
                        break;
                    case 19:
                        name = "CurrentEPPDRequest";
                        break;
                    case 20:
                        name = "CurrentEPPDData";
                        break;
                    case 23:
                        name = "FormattedProcessProgramSend";
                        break;
                    case 24:
                        name = "FormattedProcessProgramAcknowledge";
                        break;
                    case 25:
                        name = "FormattedProcessProgramRequest";
                        break;
                    case 26:
                        name = "FormattedProcessProgramData";
                        break;
                    case 27:
                        name = "ProcessProgramVerificationSend";
                        break;
                    case 28:
                        name = "ProcessProgramVerificationAcknowledge";
                        break;
                    case 29:
                        name = "ProcessProgramVerificationInquire";
                        break;
                    case 30:
                        name = "ProcessProgramVerificationGrant";
                        break;
                }
            }
            if (s == 9)
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
                        //case 13:
                        //    name = "";
                        //    break;
                }
            }
            if (s == 10)
            {
                switch (f)
                {
                    case 1:
                        name = "TerminalRequest";
                        break;
                    case 2:
                        name = "TerminalRequestAcknowledge";
                        break;
                    case 3:
                        name = "TerminalDisplaySingleBlock";
                        break;
                    case 4:
                        name = "TerminalDisplaySingleBlockAcknowledge";
                        break;
                    case 5:
                        name = "TerminalDisplayMultiBlock";
                        break;
                    case 6:
                        name = "TerminalDisplayMultiBlockAcknowledge";
                        break;
                    case 7:
                        name = "MultiBlockNotAllowed";
                        break;
                }
            }
            if (s == 14)
            {
                switch (f)
                {
                    case 1:
                        name = "GetAttributeRequest";
                        break;
                    case 2:
                        name = "GetAttributeData";
                        break;
                    case 3:
                        name = "SetAttributeRequest";
                        break;
                    case 4:
                        name = "SetAttributeData";
                        break;
                    case 7:
                        name = "GetAttributeName";
                        break;
                    case 8:
                        name = "GetAttributeNameData";
                        break;
                    case 9:
                        name = "ControlJobCreate";
                        break;
                    case 10:
                        name = "ControlJobCreateAcknowledge";
                        break;
                    case 11:
                        name = "ControlJobDeleteRequest";
                        break;
                    case 12:
                        name = "ControlJobDeleteAcknowledge";
                        break;
                }
            }
            if (s == 16)
            {
                switch (f)
                {
                    case 5:
                        name = "ProcessJobCommand";
                        break;
                    case 6:
                        name = "ProcessJobCommandAcknowledge";
                        break;
                    case 11:
                        name = "ProcessJobCreateEnh";
                        break;
                    case 12:
                        name = "ProcessJobCreateEnhAcknowledge";
                        break;
                    case 15:
                        name = "ProcessJobMultiCreate";
                        break;
                    case 16:
                        name = "ProcessJobMultiCreateAcknowledge";
                        break;
                    case 17:
                        name = "ProcessJobDequeue";
                        break;
                    case 18:
                        name = "ProcessJobDequeueCreate";
                        break;
                    case 19:
                        name = "PRGetAllJobs";
                        break;
                    case 20:
                        name = "PRGetAllJobsSend";
                        break;
                    case 21:
                        name = "PRGetSpace";
                        break;
                    case 22:
                        name = "PRGetAllJobsSend";
                        break;
                    //case 25:
                    //    name = "";
                    //    break;
                    //case 26:
                    //    name = "";
                    //    break;
                    case 27:
                        name = "ControlJobCommandRequest";
                        break;
                    case 28:
                        name = "ControlJobCommandAcknowledge";
                        break;
                }
            }

            return name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timerMethod = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timerMethod = 2;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            timerMethod = 3;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            timerMethod = 4;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            timerMethod = 5;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            timerMethod = 6;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            timerMethod = 7;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            timerMethod = 8;

        }

        private void button9_Click(object sender, EventArgs e)
        {
            timerMethod = 9;

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
                Item.Boolean(false),
                Item.L()
            );

            var msg = new SecsMessage(2, 37, "DisableEvent", item);
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
                Item.Boolean(false),
                Item.L()
            );

            var msg = new SecsMessage(2, 33, "DeleteAllReport", item);
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
                Item.Boolean(true),
                Item.L()
            );

            var msg = new SecsMessage(2, 37, "DisableEvent", item);
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

            /*
            Item item = Item.L(
                Item.U4((byte)0), // dataID, 0 fixed
                Item.L(
                    Item.L( // SlotMap Data
                        Item.U4(10), // Report ID
                        Item.L(
                            Item.U4(2102), // SlotState
                            Item.U4(2101), //PlateID
                            Item.U4(2104), // SlotState
                            Item.U4(2103), //PlateID
                            Item.U4(2106), // SlotState
                            Item.U4(2105), //PlateID
                            Item.U4(2108), // SlotState
                            Item.U4(2107) //PlateID
                        )
                    ),
                    Item.L( // Plate Inspection Information
                        Item.U4(11), // Report ID
                        Item.L(
                            Item.U4(2110), //SlotNo
                            Item.U4(2111) //PlateID
                        )
                    ),
                    Item.L( // Inspection Result
                        Item.U4(12), // Report ID
                        Item.L(
                            Item.U4(2111), //PlateID
                            Item.U4(2112), //PlateJudge
                            Item.U4(2113), //InspectPeriod
                            Item.U4(2114), //UpFlatnessValue
                            Item.U4(2115), //UpFlatnessJudge
                            Item.U4(2116), //UpTiltValue
                            Item.U4(2117), //UpTiltJudge
                            Item.U4(2118), //UpTorsionValue
                            Item.U4(2119), //UpTorsionJudge
                            Item.U4(2120), //UpRoundValue
                            Item.U4(2121), //UpRoundJudge
                            Item.U4(2122), //RightFlatnessValue
                            Item.U4(2123), //RightFlatnessJudge
                            Item.U4(2124), //RightTiltValue
                            Item.U4(2125), //RightTiltJudge
                            Item.U4(2126), //RightTorsionValue
                            Item.U4(2127), //RightTorsionJudge
                            Item.U4(2128), //RightRoundValue
                            Item.U4(2129), //RightRoundJudge
                            Item.U4(2130), //LeftFlatnessValue
                            Item.U4(2131), //LeftFlatnessJudge
                            Item.U4(2132), //LeftTiltValue
                            Item.U4(2133), //LeftTiltJudge
                            Item.U4(2134), //LeftTorsionValue
                            Item.U4(2135), //LeftTorsionJudge
                            Item.U4(2136), //LeftRoundValue
                            Item.U4(2137), //LeftRoundJudge
                            Item.U4(2138), //BottomParallelValue
                            Item.U4(2139), //BottomParallelJudge
                            Item.U4(2140), //BottomRoundValue
                            Item.U4(2141) //BottomRoundJudge
                        )
                    ),
                    Item.L( // Alarm Information
                        Item.U4(99), // Report ID
                        Item.L(
                            Item.U4(2200), //AlarmID
                            Item.U4(2201) //AlarmText
                        )
                    )

                )
            );
            //*/
            Item item = Item.L(
                Item.U4(1),
                Item.L(
                    Item.L(
                        Item.U4(1),
                        Item.L(
                            Item.U4(1004),
                            Item.U4(1003)
                            )
                    ),
                    Item.L(
                        Item.U4(30),
                        Item.L(
                            Item.U4(2101),
                            Item.U4(2102)
                            )
                    ),
                    Item.L(
                        Item.U4(31),
                        Item.L(
                            Item.U4(2103),
                            Item.U4(2104)
                            )
                    ),
                    Item.L(
                        Item.U4(32),
                        Item.L(
                            Item.U4(2105),
                            Item.U4(2106)
                            )
                    ),
                    Item.L(
                        Item.U4(33),
                        Item.L(
                            Item.U4(2107),
                            Item.U4(2108)
                            )
                    ),
                    Item.L(
                        Item.U4(11),
                        Item.L(
                            Item.U4(2110),
                            Item.U4(2111)
                            )
                    ),
                    Item.L(
                        Item.U4(12),
                        Item.L(
                            Item.U4(2110),
                            Item.U4(2111),
                            Item.U4(2112),
                            Item.U4(2113),
                            Item.U4(2114),
                            Item.U4(2115),
                            Item.U4(2116),
                            Item.U4(2117),
                            Item.U4(2118),
                            Item.U4(2119),
                            Item.U4(2120),
                            Item.U4(2121),
                            Item.U4(2122),
                            Item.U4(2123),
                            Item.U4(2124),
                            Item.U4(2125),
                            Item.U4(2126),
                            Item.U4(2127),
                            Item.U4(2128),
                            Item.U4(2129),
                            Item.U4(2130),
                            Item.U4(2131),
                            Item.U4(2132),
                            Item.U4(2133),
                            Item.U4(2134),
                            Item.U4(2135),
                            Item.U4(2136),
                            Item.U4(2137),
                            Item.U4(2138),
                            Item.U4(2139),
                            Item.U4(2140),
                            Item.U4(2141)
                            )
                    )
                )
            );

            var msg = new SecsMessage(2, 33, "DefineReport", item);
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

            /*
            Item item = Item.L(
                Item.U4((byte)0), // dataID, 0 fixed
                Item.L(
                    Item.L( // Change to Offline
                        Item.U4(11), // CEID
                        Item.L(
                        )
                    ),
                    Item.L( // Change to Local
                        Item.U4(12), // CEID
                        Item.L(
                        )
                    ),
                    Item.L( // SlotMap Data
                        Item.U4(20), // CEID
                        Item.L(
                            Item.U4(10) //ReportID
                        )
                    ),
                    Item.L( // NG'd Plate Loaded
                        Item.U4(21), // CEID
                        Item.L(
                        )
                    ),
                    Item.L( // Inspection Started
                        Item.U4(22), // CEID
                        Item.L(
                            Item.U4(12) //Report ID
                        )
                    ),
                    Item.L( // Inspection Complete
                        Item.U4(22), // CEID
                        Item.L(
                            Item.U4(12) //Report ID
                        )
                    ),
                    Item.L( // Reinspection Started
                        Item.U4(23), // CEID
                        Item.L(
                            Item.U4(12) //Report ID
                        )
                    ),
                    Item.L( // Reinspection Complete
                        Item.U4(24), // CEID
                        Item.L(
                            Item.U4(13) //Report ID
                        )
                    ),
                    Item.L( // Plate Inspection Information
                        Item.U4(30), // CEID
                        Item.L(
                            Item.U4(13) //Report ID
                        )
                    ),
                    Item.L( // Aborted
                        Item.U4(40), // CEID
                        Item.L(
                        )
                    ),
                    Item.L( // Alarm Information
                        Item.U4(99), // CEID
                        Item.L(
                            Item.U4(99) //Report
                        )
                    )

                )
            );
            //*/

            Item item = Item.L(
                Item.U4((byte)1),
                Item.L(
                    Item.L(
                        Item.U4(11),
                        Item.L(
                            Item.U4(1)
                            )
                        ),
                    Item.L(
                        Item.U4(12),
                        Item.L(
                            Item.U4(1)
                            )
                        ),
                    Item.L(
                        Item.U4(20),
                        Item.L(
                            Item.U4(1),
                            Item.U4(30),
                            Item.U4(31),
                            Item.U4(32),
                            Item.U4(33)
                            )
                        ),
                    Item.L(
                        Item.U4(22),
                        Item.L(
                            Item.U4(1),
                            Item.U4(11)
                            )
                        ),
                    Item.L(
                        Item.U4(23),
                        Item.L(
                            Item.U4(1),
                            Item.U4(11)
                            )
                        ),
                    Item.L(
                        Item.U4(24),
                        Item.L(
                            Item.U4(1),
                            Item.U4(11)
                            )
                        ),
                    Item.L(
                        Item.U4(25),
                        Item.L(
                            Item.U4(1),
                            Item.U4(11)
                            )
                        ),
                    Item.L(
                        Item.U4(30),
                        Item.L(
                            Item.U4(1),
                            Item.U4(12)
                            )
                        )
                    )
                );

            var msg = new SecsMessage(2, 35, "LinkReport", item);
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

        private void button10_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(0),
                Item.A("Terminal Message")
            );

            var msg = new SecsMessage(10, 3, "TerminalMessage", item);
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

        private void btnPlateIDReport_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L( // Plate ID Report (20)
                Item.U4(dataId), // Data ID
                Item.U4(20), // CEID
                Item.L(
                    Item.L(
                        Item.U4(10), // Report ID
                        Item.L(
                            Item.L(
                                Item.U4(2), // Slot State
                                Item.A("BS0001") // Plate ID
                            )
                        ),
                        Item.L(
                            Item.L(
                                Item.U4(2), // Slot State
                                Item.A("BS0002") // Plate ID
                            )
                        ),
                        Item.L(
                            Item.L(
                                Item.U4(2), // Slot State
                                Item.A("BS0003") // Plate ID
                            )
                        ),
                        Item.L(
                            Item.L(
                                Item.U4(2), // Slot State
                                Item.A("BS0004") // Plate ID
                            )
                        )
                    )
                )
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "EventReport", item);
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

        private void button13_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L( // NG'd Plate Loaded (21)
                Item.U4(dataId), // Data ID
                Item.U4(21), // CEID
                Item.L(
                )
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "EventReport", item);
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

        private void btnPlateEnd_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(dataId),
                Item.U4(23),
                Item.L(
                    Item.U4((byte)numSlotNo.Value), // SlotNo
                    Item.A("BS000" + ((byte)numSlotNo.Value).ToString()) // PlateID
                )
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "SendReport", item);
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

        private void button11_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(dataId),
                Item.U4(24),
                Item.L(
                    Item.U4((byte)numSlotNo.Value), // SlotNo
                    Item.A("BS000" + ((byte)numSlotNo.Value).ToString()) // PlateID
                )
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "SendReport", item);
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

        private void button12_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(dataId),
                Item.U4(25),
                Item.L(
                    Item.U4((byte)numSlotNo.Value), // SlotNo
                    Item.A("BS000" + ((byte)numSlotNo.Value).ToString()) // PlateID
                )
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "SendReport", item);
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

        private void btnInspectReport_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            string judge = cmbJudge.SelectedIndex == 1 ? "OK" : "NG";
            string period = cmbJudge.SelectedIndex == 2 ? "NG" : "OK";

            Item item = Item.L( // Plate Inspection Result
                Item.U4(dataId),
                Item.U4(30),
                Item.L(
                    Item.A("BS000" + ((byte)numSlotNo.Value).ToString()), // PlateID
                    Item.A(judge), // PlateJudge
                    Item.A(period), // Inspect Period
                    Item.F4((float)0.001), // Up Flatness Value
                    Item.A(judge), // Up Flatness Judge
                    Item.F4((float)0.001), // Up Tilt Value
                    Item.A(judge), // Up Tilt Judge
                    Item.F4((float)0.001), // Up Torsion Value
                    Item.A(judge), // Up Torsion Judge
                    Item.F4((float)0.001), // Up Round Value
                    Item.A(judge), // Up Round Judge
                    Item.F4((float)0.001), // Right Flatness Value
                    Item.A(judge), // Right Flatness Judge
                    Item.F4((float)0.001), // Right Tilt Value
                    Item.A(judge), // Right Tilt Judge
                    Item.F4((float)0.001), // Right Torsion Value
                    Item.A(judge), // Right Torsion Judge
                    Item.F4((float)0.001), // Right Round Value
                    Item.A(judge), // Right Round Judge
                    Item.F4((float)0.001), // Left Flatness Value
                    Item.A(judge), // Left Flatness Judge
                    Item.F4((float)0.001), // Left Tilt Value
                    Item.A(judge), // Left Tilt Judge
                    Item.F4((float)0.001), // Left Torsion Value
                    Item.A(judge), // Left Torsion Judge
                    Item.F4((float)0.001), // Left Round Value
                    Item.A(judge), // Left Round Judge
                    Item.F4((float)0.001), // Bottom Parallel Value
                    Item.A(judge), // Bottom Parallel Judge
                    Item.F4((float)0.001), // Bottom Round Value
                    Item.A(judge) // Bottom Round Judge
                )
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "SendReport", item);
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

        private void button14_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            string judge = cmbJudge.SelectedIndex == 1 ? "OK" : "NG";
            string period = cmbJudge.SelectedIndex == 2 ? "NG" : "OK";

            Item item = Item.L( // Alarm Report
                Item.U4(dataId),
                Item.U4(99),
                Item.L(
                    Item.U4((uint)numAlarmID.Value), // Alarm ID
                    Item.A(getAlarmText((int)numAlarmID.Value)) // Alarm Text
                )
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "SendReport", item);
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

        string getAlarmText(int index)
        {
            switch(index)
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

        private void button15_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L( // Aborted (40)
                Item.U4(dataId), // Data ID
                Item.U4(40), // CEID
                Item.L(
                )
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(6, 11, "EventReport", item);
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

        private void btnSelectedEqStatusRequest_Click(object sender, EventArgs e)
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

            var msg = new SecsMessage(1, 3, "SelectedEquipmentStatusRequest", item);
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

        private void btnStatusVariableNameListRequest_Click(object sender, EventArgs e)
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

            var msg = new SecsMessage(1, 11, "StatusVariableNameListRequest", item);
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

        private void btnEquipmentConstantRequest_Click(object sender, EventArgs e)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(1),
                Item.U4(2),
                Item.U4(3),
                Item.U4(4)
            );
            //dataId++; // fixed to 1

            var msg = new SecsMessage(2, 13, "EquipmentConstantRequest", item);
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


    }


}
