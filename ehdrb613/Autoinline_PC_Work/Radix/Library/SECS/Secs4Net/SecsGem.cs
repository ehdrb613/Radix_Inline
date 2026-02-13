using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Secs4Net.Properties;

namespace Secs4Net {
    public sealed class SecsGem : IDisposable {
        public event EventHandler ConnectionChanged;
        public ConnectionState State { get; private set; }
        public short DeviceId { get; set; }
        public int LinkTestInterval { get; set; }
        public int T3 { get; set; }
        public int T5 { get; set; }
        public int T6 { get; set; }
        public int T7 { get; set; }
        public int T8 { get; set; }

        public bool LinkTestEnable {
            get { return _timerLinkTest.Enabled; }
            set {
                _timerLinkTest.Interval = LinkTestInterval;
                _timerLinkTest.Enabled = value;
            }
        }

        readonly bool _isActive;
        readonly IPAddress _ip;
        readonly int _port;
        Socket _socket;

        readonly SecsDecoder _secsDecoder;
        readonly ConcurrentDictionary<int, SecsAsyncResult> _replyExpectedMsgs = new ConcurrentDictionary<int, SecsAsyncResult>();
        readonly Action<SecsMessage, Action<SecsMessage>> PrimaryMessageHandler;
        readonly SecsTracer _tracer;
        readonly System.Timers.Timer _timer7        = new System.Timers.Timer();	// between socket connected and received Select.req timer
        readonly System.Timers.Timer _timer8        = new System.Timers.Timer();
        readonly System.Timers.Timer _timerLinkTest = new System.Timers.Timer();

        readonly Action StartImpl;
        readonly Action StopImpl;

        readonly byte[] _recvBuffer;
        static readonly SecsMessage ControlMessage = new SecsMessage(0, 0, string.Empty);
        static readonly ArraySegment<byte> ControlMessageLengthBytes = new ArraySegment<byte>(new byte[] { 0, 0, 0, 10 });
        static readonly SecsTracer DefaultTracer = new SecsTracer();
        readonly Func<int> NewSystemByte;

        public SecsGem(IPAddress ip, int port, bool isActive, Action<SecsMessage, Action<SecsMessage>> primaryMsgHandler, SecsTracer tracer, int receiveBufferSize) {
            try
            {
                if (ip == null)
                {
                    //Console.WriteLine("ArgumentNullException : ip");
                    throw new ArgumentNullException("ip");
                }

                //Console.WriteLine("SecsGem.SecsGem");

                _ip = ip;
                _port = port;
                _isActive = isActive;
                _recvBuffer = new byte[receiveBufferSize < 0x4000 ? 0x4000 : receiveBufferSize];
                _secsDecoder = new SecsDecoder(HandleControlMessage, HandleDataMessage);
                _tracer = tracer ?? DefaultTracer;
                PrimaryMessageHandler = primaryMsgHandler ?? ((primary, reply) => reply(null));
                T3 = 15000; // Reply Timeout
                T5 = 10000; // Connection Separation Timeout
                T6 = 5000; // Control Transaction Timeout
                T7 = 10000; // NOT Selected Timeout
                T8 = 5000; // Network Intercharacter Timeout
                LinkTestInterval = 60000;

                int systemByte = new Random(Guid.NewGuid().GetHashCode()).Next();
                NewSystemByte = () => Interlocked.Increment(ref systemByte);

                #region Timer Action
                _timer7.Elapsed += delegate
                {
                    _tracer.TraceError("T7 Timeout");
                    this.CommunicationStateChanging(ConnectionState.Retry);
                };

                _timer8.Elapsed += delegate
                {
                    _tracer.TraceError("T8 Timeout");
                    this.CommunicationStateChanging(ConnectionState.Retry);
                };

                _timerLinkTest.Elapsed += delegate
                {
                    if (this.State == ConnectionState.Selected)
                        this.SendControlMessage(MessageType.Linktest_req, NewSystemByte());
                };
                #endregion
                if (_isActive)
                {
                    #region Active Impl
                    var timer5 = new System.Timers.Timer();
                    timer5.Elapsed += delegate
                    {
                        timer5.Enabled = false;
                        _tracer.TraceError("T5 Timeout");
                        this.CommunicationStateChanging(ConnectionState.Retry);
                    };

                    StartImpl = delegate
                    {
                        this.CommunicationStateChanging(ConnectionState.Connecting);
                        try
                        {
                            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            socket.Connect(_ip, _port);
                            this.CommunicationStateChanging(ConnectionState.Connected);
                            this._socket = socket;
                            this.SendControlMessage(MessageType.Select_req, NewSystemByte());
                            this._socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ReceiveComplete, null);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            Console.WriteLine(ex.StackTrace);
                            if (_isDisposed) return;
                            _tracer.TraceError(ex.Message);
                            _tracer.TraceInfo("Start T5 Timer");
                            timer5.Interval = T5;
                            timer5.Enabled = true;
                        }
                    };

                    StopImpl = delegate
                    {
                        timer5.Stop();
                        if (_isDisposed) timer5.Dispose();
                    };
                    #endregion
                    StartImpl.BeginInvoke(null, null);
                }
                else
                {
                    #region Passive Impl
                    var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.Bind(new IPEndPoint(_ip, _port));
                    server.Listen(0);

                    StartImpl = delegate
                    {
                        this.CommunicationStateChanging(ConnectionState.Connecting);
                        server.BeginAccept(iar =>
                        {
                            try
                            {
                                this._socket = server.EndAccept(iar);
                                this.CommunicationStateChanging(ConnectionState.Connected);
                                this._socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ReceiveComplete, null);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                Console.WriteLine(ex.StackTrace);
                                _tracer.TraceError("System Exception:" + ex.Message);
                                this.CommunicationStateChanging(ConnectionState.Retry);
                            }
                        }, null);
                    };

                    StopImpl = delegate
                    {
                        if (_isDisposed && server != null)
                            server.Close();
                    };
                    #endregion
                    StartImpl();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        #region Socket Receive Process
        void ReceiveComplete(IAsyncResult iar) {
            //Console.WriteLine("SecsGem.ReceiveComplete");

            try
            {
                int count = _socket.EndReceive(iar);

                _timer8.Enabled = false;

                if (count == 0) {
                    _tracer.TraceError("Received 0 byte data. Close the socket.");
                    this.CommunicationStateChanging(ConnectionState.Retry);
                    return;
                }

                if (_secsDecoder.Decode(_recvBuffer, 0, count)) {
                    _tracer.TraceInfo("Start T8 Timer");
                    _timer8.Interval = T8;
                    _timer8.Enabled = true;
                }

                _socket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, ReceiveComplete, null);
            } catch (NullReferenceException) {
            } catch (SocketException ex) {
                _tracer.TraceError("RecieveComplete socket error:" + ex.Message);
                this.CommunicationStateChanging(ConnectionState.Retry);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                _tracer.TraceError(ex.ToString());
                this.CommunicationStateChanging(ConnectionState.Retry);
            }
        }

        void HandleControlMessage(Header header) {
            //Console.WriteLine("SecsGem.HandleControlMessage");
            try
            {
                int systembyte = header.SystemBytes;
                if ((byte)header.MessageType % 2 == 0)
                {
                    SecsAsyncResult ar = null;
                    if (_replyExpectedMsgs.TryGetValue(systembyte, out ar))
                    {
                        ar.EndProcess(ControlMessage, false);
                    }
                    else
                    {
                        _tracer.TraceWarning("Received Unexpected Control Message: " + header.MessageType);
                        return;
                    }
                }
                _tracer.TraceInfo("Receive Control message: " + header.MessageType);
                switch (header.MessageType)
                {
                    case MessageType.Select_req:
                        this.SendControlMessage(MessageType.Select_rsp, systembyte);
                        this.CommunicationStateChanging(ConnectionState.Selected);
                        break;
                    case MessageType.Select_rsp:
                        switch (header.F)
                        {
                            case 0:
                                this.CommunicationStateChanging(ConnectionState.Selected);
                                break;
                            case 1:
                                _tracer.TraceError("Communication Already Active.");
                                break;
                            case 2:
                                _tracer.TraceError("Connection Not Ready.");
                                break;
                            case 3:
                                _tracer.TraceError("Connection Exhaust.");
                                break;
                            default:
                                _tracer.TraceError("Connection Status Is Unknown.");
                                break;
                        }
                        break;
                    case MessageType.Linktest_req:
                        this.SendControlMessage(MessageType.Linktest_rsp, systembyte);
                        break;
                    case MessageType.Seperate_req:
                        this.CommunicationStateChanging(ConnectionState.Retry);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        void HandleDataMessage(Header header, SecsMessage msg) {
            //Console.WriteLine("SecsGem.HandleDataMessage");
            try
            {
                //Console.WriteLine("header.DeviceId : " + header.DeviceId);
                //Console.WriteLine("this.DeviceId : " + this.DeviceId);
                //Console.WriteLine("msg.S : " + msg.S);
                //Console.WriteLine("msg.F : " + msg.F);
                int systembyte = header.SystemBytes;

                if (header.DeviceId != this.DeviceId && msg.S != 9 && msg.F != 1)
                {
                    _tracer.TraceMessageIn(msg, systembyte);
                    _tracer.TraceWarning("Received Unrecognized Device Id Message");
                    try
                    {
                        this.SendDataMessage(new SecsMessage(9, 1, decodeSecsName(9, 1), false, Item.B(header.Bytes)), NewSystemByte(), null, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine(ex.StackTrace);
                        _tracer.TraceError("Send S9F1 Error:" + ex.Message);
                    }
                    return;
                }

                if (decodeSecsName(msg.S, msg.F).Contains("UnknownStream"))
                {
                    _tracer.TraceMessageIn(msg, systembyte);
                    _tracer.TraceWarning("Unknown Stream Message");
                    try
                    {
                        this.SendDataMessage(new SecsMessage(9, 3, decodeSecsName(9,3), false, Item.B(header.Bytes)), NewSystemByte(), null, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine(ex.StackTrace);
                        _tracer.TraceError("Send S9F3 Error:" + ex.Message);
                    }
                    return;
                }


                if (decodeSecsName(msg.S, msg.F).Contains("UnknownFunction"))
                {
                    _tracer.TraceMessageIn(msg, systembyte);
                    _tracer.TraceWarning("Unknown Function Message");
                    try
                    {
                        this.SendDataMessage(new SecsMessage(9, 5, decodeSecsName(9, 5), false, Item.B(header.Bytes)), NewSystemByte(), null, null);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine(ex.StackTrace);
                        _tracer.TraceError("Send S9F5 Error:" + ex.Message);
                    }
                    return;
                }



                if (msg.F % 2 != 0)
                {
                    if (msg.S != 9)
                    {
                        //Primary message
                        _tracer.TraceMessageIn(msg, systembyte);
                        PrimaryMessageHandler(msg, secondary =>
                        {
                            if (!header.ReplyExpected || State != ConnectionState.Selected)
                                return;

                            secondary = secondary ?? new SecsMessage(9, 7, decodeSecsName(9, 7), false, Item.B(header.Bytes));
                            secondary.ReplyExpected = false;
                            try
                            {
                                this.SendDataMessage(secondary, secondary.S == 9 ? NewSystemByte() : header.SystemBytes, null, null);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                                Console.WriteLine(ex.StackTrace);
                                _tracer.TraceError("Reply Secondary Message Error:" + ex.Message);
                            }
                        });
                        return;
                    }
                    // Error message
                    var headerBytes = (byte[])msg.SecsItem;
                    systembyte = BitConverter.ToInt32(new byte[] { headerBytes[9], headerBytes[8], headerBytes[7], headerBytes[6] }, 0);
                }

                // Secondary message
                SecsAsyncResult ar = null;
                if (_replyExpectedMsgs.TryGetValue(systembyte, out ar))
                    ar.EndProcess(msg, false);
                _tracer.TraceMessageIn(msg, systembyte);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }
        #endregion
        public static string decodeSecsName(byte s, byte f) // YJ추가. SECS GEM 통신시 stream과 function에 따른 이름을 리턴. 유효성 검사에도 이용.
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

        #region Socket Send Process
        void SendControlMessage(MessageType msgType, int systembyte) {
            //Console.WriteLine("SecsGem.SendControlMessage");
            try
            {
                if (this._socket == null || !this._socket.Connected)
                    return;

                if ((byte)msgType % 2 == 1 && msgType != MessageType.Seperate_req)
                {
                    var ar = new SecsAsyncResult(ControlMessage, 0, null, null);
                    _replyExpectedMsgs[systembyte] = ar;

                    ThreadPool.RegisterWaitForSingleObject(ar.AsyncWaitHandle,
                        (state, timeout) =>
                        {
                            SecsAsyncResult ars;
                            if (_replyExpectedMsgs.TryRemove((int)state, out ars) && timeout)
                            {
                                _tracer.TraceError("T6 Timeout");
                                this.CommunicationStateChanging(ConnectionState.Retry);
                            }
                        }, systembyte, T6, true);
                }

                var header = new Header(new byte[10])
                {
                    MessageType = msgType,
                    SystemBytes = systembyte
                };
                header.Bytes[0] = 0xFF;
                header.Bytes[1] = 0xFF;
                _socket.Send(new List<ArraySegment<byte>>(2){
                ControlMessageLengthBytes,
                new ArraySegment<byte>(header.Bytes)
            });
                _tracer.TraceInfo("Sent Control Message: " + header.MessageType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        SecsAsyncResult SendDataMessage(SecsMessage msg, int systembyte, AsyncCallback callback, object syncState) {
            //Console.WriteLine("SecsGem.SendDataMessage");
            SecsAsyncResult ar = null;
            try
            {
                if (this.State != ConnectionState.Selected)
                    throw new SecsException("Device is not selected");

                var header = new Header(new byte[10])
                {
                    S = msg.S,
                    F = msg.F,
                    ReplyExpected = msg.ReplyExpected,
                    DeviceId = this.DeviceId,
                    SystemBytes = systembyte
                };

                #region 데이터 타입이 강제로 바뀔 때 변환하기 U1 [1] 1 ==> U4 [1] 1, I1 [1] 0 ==> F4 [1] 0
                /* SecsItem type이 readonly라 일부만 바꿔 치환은 불가능
                try
                {
                    if ((uint)msg.SecsItem.Items[2].Items[1].Items[1].Items[0] == 1)
                    {
                        //msg.SecsItem.Items[2].Items[1].Items[1].Items[0] = Item.U4(1);
                    }
                }
                catch { }
                //*/
                #endregion

                var buffer = new EncodedBuffer(header.Bytes, msg.RawDatas);


                if (msg.ReplyExpected)
                {
                    ar = new SecsAsyncResult(msg, systembyte, callback, syncState);
                    _replyExpectedMsgs[systembyte] = ar;

                    ThreadPool.RegisterWaitForSingleObject(ar.AsyncWaitHandle,
                       (state, timeout) =>
                       {
                           SecsAsyncResult ars;
                           if (_replyExpectedMsgs.TryRemove((int)state, out ars) && timeout)
                           {
                               _tracer.TraceError(string.Format("T3 Timeout[id=0x{0:X8}]", ars.SystemByte));
                               ars.EndProcess(null, timeout);
                               _replyExpectedMsgs.Clear();
                               //Send(new SecsMessage(9, 9, decodeSecsName(9, 9), false, Item.L())); // T3 Timeout send
                           }
                       }, systembyte, T3, true);
                }

                SocketError error;
                _socket.Send(buffer, SocketFlags.None, out error);

                if (error != SocketError.Success)
                {
                    var errorMsg = "Socket send error :" + new SocketException((int)error).Message;
                    _tracer.TraceError(errorMsg);
                    this.CommunicationStateChanging(ConnectionState.Retry);
                    throw new SecsException(errorMsg);
                }

                _tracer.TraceMessageOut(msg, systembyte);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return ar;
        }
        #endregion
        #region Internal State Transition
        void CommunicationStateChanging(ConnectionState newState) {
            //Console.WriteLine("SecsGem.CommunicationStateChanging");
            State = newState;
            if (ConnectionChanged != null)
                ConnectionChanged(this, EventArgs.Empty);

            switch (State) {
                case ConnectionState.Selected:
                    _timer7.Enabled = false;
                    _tracer.TraceInfo("Stop T7 Timer");
                    break;
                case ConnectionState.Connected:
                    _tracer.TraceInfo("Start T7 Timer");
                    _timer7.Interval = T7;
                    _timer7.Enabled = true;
                    break;
                case ConnectionState.Retry:
                    if (_isDisposed)
                        return;
                    Reset();
                    Thread.Sleep(2000);
                    StartImpl();
                    break;
            }
        }

        void Reset() {
            //Console.WriteLine("SecsGem.Reset");
            this._timer7.Stop();
            this._timer8.Stop();
            this._timerLinkTest.Stop();
            this._secsDecoder.Reset();
            if (_socket != null) {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = null;
            }
            this._replyExpectedMsgs.Clear();
            StopImpl();
        }
        #endregion
        #region Public API
        /// <summary>
        /// Send SECS message to device.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>Device's reply msg if msg.ReplyExpected is true;otherwise, null.</returns>
        public SecsMessage Send(SecsMessage msg) {
            //Console.WriteLine("SecsGem.Send");
            return this.EndSend(this.BeginSend(msg, null, null));
        }

        /// <summary>
        /// Send SECS message asynchronously to device .
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="callback">Device's reply message handler callback.</param>
        /// <param name="state">synchronize state object</param>
        /// <returns>An IAsyncResult that references the asynchronous send if msg.ReplyExpected is true;otherwise, null.</returns>
        public IAsyncResult BeginSend(SecsMessage msg, AsyncCallback callback, object state) {
            //Console.WriteLine("SecsGem.BeginSend : " + msg.ToString());
            return this.SendDataMessage(msg, NewSystemByte(), callback, state);
        }

        /// <summary>
        /// Ends a asynchronous send.
        /// </summary>
        /// <param name="asyncResult">An IAsyncResult that references the asynchronous send</param>
        /// <returns>Device's reply message if <paramref name="asyncResult"/> is an IAsyncResult that references the asynchronous send, otherwise null.</returns>
        public SecsMessage EndSend(IAsyncResult asyncResult) {
            try
            {
                //Console.WriteLine("SecsGem.EndSend");
                if (asyncResult == null)
                {
                    //Console.WriteLine("EndSend : ArgumentNullException");
                    throw new ArgumentNullException("asyncResult");
                }
                var ar = asyncResult as SecsAsyncResult;
                if (ar == null)
                {
                    //Console.WriteLine("EndSend : BeginSend 에 의해 매개변수 asyncResult 얻지 못함");
                    throw new ArgumentException("BeginSend 에 의해 매개변수 asyncResult 얻지 못함", "asyncResult");
                }
                //Console.WriteLine("wait 전");
                ar.AsyncWaitHandle.WaitOne();
                //Console.WriteLine("wait 후");
                return ar.Secondary;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }

        volatile bool _isDisposed;
        public void Dispose() {
            //Console.WriteLine("SecsGem.Dispose");
            if (!_isDisposed) {
                _isDisposed = true;
                ConnectionChanged = null;
                if (State == ConnectionState.Selected)
                    this.SendControlMessage(MessageType.Seperate_req, NewSystemByte());
                Reset();
                _timer7.Dispose();
                _timer8.Dispose();
                _timerLinkTest.Dispose();
            }
        }

        public string DeviceAddress {
            get {
                return _isActive ? _ip.ToString() :
                    this._socket == null ? "N/A" : ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
            }
        }
        #endregion
        #region Async Impl
        sealed class SecsAsyncResult : IAsyncResult {
            readonly ManualResetEvent _ev = new ManualResetEvent(false);
            readonly SecsMessage Primary;
            readonly AsyncCallback _callback;
            public readonly int SystemByte;

            SecsMessage _secondary;
            bool _timeout;

            internal SecsAsyncResult(SecsMessage primaryMsg, int systembyte, AsyncCallback callback, object state) {
                //Console.WriteLine("SecsAsyncResult.SecsAsyncResult");
                Primary = primaryMsg;
                AsyncState = state;
                SystemByte = systembyte;
                _callback = callback;
            }

            internal void EndProcess(SecsMessage replyMsg, bool timeout) {
                //Console.WriteLine("SecsAsyncResult.EndProcess");
                if (replyMsg != null) {
                    _secondary = replyMsg;
                    //_secondary.Name = Primary.Name;
                }
                _timeout = timeout;
                IsCompleted = !timeout;
                _ev.Set();
                if (_callback != null)
                    _callback(this);
            }

            internal SecsMessage Secondary {
                get {
                    if (_timeout) throw new SecsException(Primary, Resources.T3Timeout);
                    if (_secondary == null) return null;
                    if (_secondary.F == 0) throw new SecsException(Primary, Resources.SxF0);
                    if (_secondary.S == 1)
                    {
                        switch (_secondary.F)
                        {
                            case 1: throw new SecsException(Primary, Resources.S1F1);
                            case 2: throw new SecsException(Primary, Resources.S1F2);
                            case 3: throw new SecsException(Primary, Resources.S1F3);
                            case 4: throw new SecsException(Primary, Resources.S1F4);
                            case 11: throw new SecsException(Primary, Resources.S1F11);
                            case 12: throw new SecsException(Primary, Resources.S1F12);
                            case 13: throw new SecsException(Primary, Resources.S1F13);
                            case 14: throw new SecsException(Primary, Resources.S1F14);
                            case 15: throw new SecsException(Primary, Resources.S1F15);
                            case 16: throw new SecsException(Primary, Resources.S1F16);
                            case 17: throw new SecsException(Primary, Resources.S1F17);
                            case 18: throw new SecsException(Primary, Resources.S1F18);
                            default: throw new SecsException(Primary, Resources.S1Fy);
                        }
                    }
                    if (_secondary.S == 2)
                    {
                        switch (_secondary.F)
                        {
                            case 13: throw new SecsException(Primary, Resources.S2F13);
                            case 14: throw new SecsException(Primary, Resources.S2F14);
                            case 15: throw new SecsException(Primary, Resources.S2F15);
                            case 16: throw new SecsException(Primary, Resources.S2F16);
                            case 17: throw new SecsException(Primary, Resources.S2F17);
                            case 18: throw new SecsException(Primary, Resources.S2F18);
                            case 23: throw new SecsException(Primary, Resources.S2F23);
                            case 24: throw new SecsException(Primary, Resources.S2F24);
                            case 29: throw new SecsException(Primary, Resources.S2F29);
                            case 30: throw new SecsException(Primary, Resources.S2F30);
                            case 31: throw new SecsException(Primary, Resources.S2F31);
                            case 32: throw new SecsException(Primary, Resources.S2F32);
                            case 33: throw new SecsException(Primary, Resources.S2F33);
                            case 34: throw new SecsException(Primary, Resources.S2F34);
                            case 35: throw new SecsException(Primary, Resources.S2F35);
                            case 36: throw new SecsException(Primary, Resources.S2F36);
                            case 37: throw new SecsException(Primary, Resources.S2F37);
                            case 38: throw new SecsException(Primary, Resources.S2F38);
                            case 39: throw new SecsException(Primary, Resources.S2F39);
                            case 40: throw new SecsException(Primary, Resources.S2F40);
                            case 41: throw new SecsException(Primary, Resources.S2F41);
                            case 42: throw new SecsException(Primary, Resources.S2F42);
                            case 43: throw new SecsException(Primary, Resources.S2F43);
                            case 44: throw new SecsException(Primary, Resources.S2F44);
                            default: throw new SecsException(Primary, Resources.S2Fy);
                        }
                    }
                    if (_secondary.S == 3)
                    {
                        switch (_secondary.F)
                        {
                            case 17: throw new SecsException(Primary, Resources.S3F17);
                            case 18: throw new SecsException(Primary, Resources.S3F18);
                            case 19: throw new SecsException(Primary, Resources.S3F19);
                            case 20: throw new SecsException(Primary, Resources.S3F20);
                            case 25: throw new SecsException(Primary, Resources.S3F25);
                            case 26: throw new SecsException(Primary, Resources.S3F26);
                            case 27: throw new SecsException(Primary, Resources.S3F27);
                            case 28: throw new SecsException(Primary, Resources.S3F28);
                            default: throw new SecsException(Primary, Resources.S3Fy);
                        }
                    }
                    if (_secondary.S == 5)
                    {
                        switch (_secondary.F)
                        {
                            case 1: throw new SecsException(Primary, Resources.S5F1);
                            case 2: throw new SecsException(Primary, Resources.S5F2);
                            case 3: throw new SecsException(Primary, Resources.S5F3);
                            case 4: throw new SecsException(Primary, Resources.S5F4);
                            case 5: throw new SecsException(Primary, Resources.S5F5);
                            case 6: throw new SecsException(Primary, Resources.S5F6);
                            case 7: throw new SecsException(Primary, Resources.S5F7);
                            case 8: throw new SecsException(Primary, Resources.S5F8);
                            default: throw new SecsException(Primary, Resources.S5Fy);
                        }
                    }
                    if (_secondary.S == 6)
                    {
                        switch (_secondary.F)
                        {
                            case 1: throw new SecsException(Primary, Resources.S6F1);
                            case 2: throw new SecsException(Primary, Resources.S6F2);
                            case 5: throw new SecsException(Primary, Resources.S6F5);
                            case 6: throw new SecsException(Primary, Resources.S6F6);
                            case 11: throw new SecsException(Primary, Resources.S6F11);
                            case 12: throw new SecsException(Primary, Resources.S6F12);
                            case 15: throw new SecsException(Primary, Resources.S6F15);
                            case 16: throw new SecsException(Primary, Resources.S6F16);
                            case 19: throw new SecsException(Primary, Resources.S6F19);
                            case 20: throw new SecsException(Primary, Resources.S6F20);
                            case 23: throw new SecsException(Primary, Resources.S6F23);
                            case 24: throw new SecsException(Primary, Resources.S6F24);
                            default: throw new SecsException(Primary, Resources.S6Fy);
                        }
                    }
                    if (_secondary.S == 7)
                    {
                        switch (_secondary.F)
                        {
                            case 1: throw new SecsException(Primary, Resources.S7F1);
                            case 2: throw new SecsException(Primary, Resources.S7F2);
                            case 3: throw new SecsException(Primary, Resources.S7F3);
                            case 4: throw new SecsException(Primary, Resources.S7F4);
                            case 5: throw new SecsException(Primary, Resources.S7F5);
                            case 6: throw new SecsException(Primary, Resources.S7F6);
                            case 17: throw new SecsException(Primary, Resources.S7F17);
                            case 18: throw new SecsException(Primary, Resources.S7F18);
                            case 19: throw new SecsException(Primary, Resources.S7F19);
                            case 20: throw new SecsException(Primary, Resources.S7F20);
                            case 23: throw new SecsException(Primary, Resources.S7F23);
                            case 24: throw new SecsException(Primary, Resources.S7F24);
                            case 25: throw new SecsException(Primary, Resources.S7F25);
                            case 26: throw new SecsException(Primary, Resources.S7F26);
                            case 27: throw new SecsException(Primary, Resources.S7F27);
                            case 28: throw new SecsException(Primary, Resources.S7F28);
                            case 29: throw new SecsException(Primary, Resources.S7F29);
                            case 30: throw new SecsException(Primary, Resources.S7F30);
                            default: throw new SecsException(Primary, Resources.S7Fy);
                        }
                    }
                    if (_secondary.S == 9) {
                        switch (_secondary.F) {
                            case 1: throw new SecsException(Primary, Resources.S9F1);
                            case 3: throw new SecsException(Primary, Resources.S9F3);
                            case 5: throw new SecsException(Primary, Resources.S9F5);
                            case 7: throw new SecsException(Primary, Resources.S9F7);
                            case 9: throw new SecsException(Primary, Resources.S9F9);
                            case 11: throw new SecsException(Primary, Resources.S9F11);
                            case 13: throw new SecsException(Primary, Resources.S9F13);
                            default: throw new SecsException(Primary, Resources.S9Fy);
                        }
                    }
                    if (_secondary.S == 10)
                    {
                        switch (_secondary.F)
                        {
                            case 1: throw new SecsException(Primary, Resources.S10F1);
                            case 2: throw new SecsException(Primary, Resources.S10F2);
                            case 3: throw new SecsException(Primary, Resources.S10F3);
                            case 4: throw new SecsException(Primary, Resources.S10F4);
                            case 5: throw new SecsException(Primary, Resources.S10F5);
                            case 6: throw new SecsException(Primary, Resources.S10F6);
                            case 7: throw new SecsException(Primary, Resources.S10F7);
                            default: throw new SecsException(Primary, Resources.S10Fy);
                        }
                    }
                    if (_secondary.S == 14)
                    {
                        switch (_secondary.F)
                        {
                            case 1: throw new SecsException(Primary, Resources.S14F1);
                            case 2: throw new SecsException(Primary, Resources.S14F2);
                            case 3: throw new SecsException(Primary, Resources.S14F3);
                            case 4: throw new SecsException(Primary, Resources.S14F4);
                            case 7: throw new SecsException(Primary, Resources.S14F7);
                            case 8: throw new SecsException(Primary, Resources.S14F8);
                            case 9: throw new SecsException(Primary, Resources.S14F9);
                            case 10: throw new SecsException(Primary, Resources.S14F10);
                            default: throw new SecsException(Primary, Resources.S14Fy);
                        }
                    }
                    if (_secondary.S == 16)
                    {
                        switch (_secondary.F)
                        {
                            case 5: throw new SecsException(Primary, Resources.S16F5);
                            case 6: throw new SecsException(Primary, Resources.S16F6);
                            case 11: throw new SecsException(Primary, Resources.S16F11);
                            case 12: throw new SecsException(Primary, Resources.S16F12);
                            case 15: throw new SecsException(Primary, Resources.S16F15);
                            case 16: throw new SecsException(Primary, Resources.S16F16);
                            case 17: throw new SecsException(Primary, Resources.S16F17);
                            case 18: throw new SecsException(Primary, Resources.S16F18);
                            case 19: throw new SecsException(Primary, Resources.S16F19);
                            case 20: throw new SecsException(Primary, Resources.S16F20);
                            case 21: throw new SecsException(Primary, Resources.S16F21);
                            case 22: throw new SecsException(Primary, Resources.S16F22);
                            case 25: throw new SecsException(Primary, Resources.S16F25);
                            case 26: throw new SecsException(Primary, Resources.S16F26);
                            case 27: throw new SecsException(Primary, Resources.S16F27);
                            case 28: throw new SecsException(Primary, Resources.S16F28);
                            default: throw new SecsException(Primary, Resources.S16Fy);
                        }
                    }
                    return _secondary;
                }
            }

            #region IAsyncResult Members

            public object AsyncState { get; private set; }

            public WaitHandle AsyncWaitHandle { get { return _ev; } }

            public bool CompletedSynchronously { get { return false; } }

            public bool IsCompleted { get; private set; }

            #endregion
        }
        #endregion
        #region SECS Decoder
        sealed class SecsDecoder {
            delegate int Decoder(byte[] data, int length, ref int index, out int need);
            #region share
            uint _messageLength;// total byte length
            Header _msgHeader; // message header
            SecsMessage _msg;
            readonly Stack<List<Item>> _stack = new Stack<List<Item>>(); // List Item stack
            SecsFormat _format;
            byte _lengthBits;
            readonly byte[] _itemLengthBytes = new byte[4];
            int _itemLength;
            #endregion

            /// <summary>
            /// decode pipeline
            /// </summary>
            readonly Decoder[] decoders;
            readonly Action<Header, SecsMessage> DataMsgHandler;
            readonly Action<Header> ControlMsgHandler;

            internal SecsDecoder(Action<Header> controlMsgHandler, Action<Header, SecsMessage> msgHandler) {
                //Console.WriteLine("SecsDecoder.SecsDecoder");
                DataMsgHandler = msgHandler;
                ControlMsgHandler = controlMsgHandler;

                decoders = new Decoder[5];

                #region decoders[0]: get total message length 4 bytes
                decoders[0] = (byte[] data, int length, ref int index, out int need) => {
                    if (!CheckAvailable(length, index, 4, out need)) return 0;

                    Array.Reverse(data, index, 4);
                    _messageLength = BitConverter.ToUInt32(data, index);
                    Trace.WriteLine("Get Message Length =" + _messageLength);
                    index += 4;

                    return 1;
                };
                #endregion
                #region decoders[1]: get message header 10 bytes
                decoders[1] = (byte[] data, int length, ref int index, out int need) => {
                    if (!CheckAvailable(length, index, 10, out need)) return 1;

                    _msgHeader = new Header(new byte[10]);
                    Array.Copy(data, index, _msgHeader.Bytes, 0, 10);
                    index += 10;
                    _messageLength -= 10;
                    if (_messageLength == 0) { // send ?
                        if (_msgHeader.MessageType == MessageType.Data_Message) {
                            _msg = new SecsMessage(_msgHeader.S, _msgHeader.F, string.Empty, _msgHeader.ReplyExpected, null);
                            //Console.WriteLine("1 : " + _msg.ReplyExpected.ToString());
                            _msg.Name = decodeName(_msg);
                            ProcessMessage();
                        } else {
                            ControlMsgHandler(_msgHeader);
                            _messageLength = 0;
                        }
                        return 0;
                    } else if (length - index >= _messageLength) { // reply?
                        
                        _msg = new SecsMessage(_msgHeader.S, _msgHeader.F, _msgHeader.ReplyExpected, data, ref index);
                        //Console.WriteLine("2 : " + _msg.ReplyExpected.ToString());
                        _msg.Name = decodeName(_msg);

                        ProcessMessage();
                        return 0; //completeWith message received
                    }
                    return 2;
                };
                #endregion
                #region decoders[2]: get _format + lengthBits(2bit) 1 byte
                decoders[2] = (byte[] data, int length, ref int index, out int need) => {
                    if (!CheckAvailable(length, index, 1, out need)) return 2;

                    _format = (SecsFormat)(data[index] & 0xFC);
                    _lengthBits = (byte)(data[index] & 3);
                    index++;
                    _messageLength--;
                    return 3;
                };
                #endregion
                #region decoders[3]: get _itemLength _lengthBits bytes
                decoders[3] = (byte[] data, int length, ref int index, out int need) => {
                    if (!CheckAvailable(length, index, _lengthBits, out need)) return 3;

                    Array.Copy(data, index, _itemLengthBytes, 0, _lengthBits);
                    Array.Reverse(_itemLengthBytes, 0, _lengthBits);

                    _itemLength = BitConverter.ToInt32(_itemLengthBytes, 0);
                    Array.Clear(_itemLengthBytes, 0, 4);

                    index += _lengthBits;
                    _messageLength -= _lengthBits;
                    return 4;
                };
                #endregion
                #region decoders[4]: get item value
                decoders[4] = (byte[] data, int length, ref int index, out int need) => {
                    need = 0;
                    Item item = null;
                    if (_format == SecsFormat.List) {
                        if (_itemLength == 0)
                            item = Item.L();
                        else {
                            _stack.Push(new List<Item>(_itemLength));
                            return 2;
                        }
                    } else {
                        if (!CheckAvailable(length, index, _itemLength, out need)) return 4;

                        item = _itemLength == 0 ? _format.BytesDecode() : _format.BytesDecode(data, index, _itemLength);
                        index += _itemLength;
                        _messageLength -= (uint)_itemLength;
                    }
                    if (_stack.Count > 0) {
                        var list = _stack.Peek();
                        list.Add(item);
                        while (list.Count == list.Capacity) {
                            item = Item.L(_stack.Pop());
                            if (_stack.Count > 0) {
                                list = _stack.Peek();
                                list.Add(item);
                            } else {
                                _msg = new SecsMessage(_msgHeader.S, _msgHeader.F, "Unknown", _msgHeader.ReplyExpected, item);
                                //Console.WriteLine("3 : " + _msg.ReplyExpected.ToString());
                                _msg.Name = decodeName(_msg);
                                ProcessMessage();
                                return 0;
                            }
                        }
                    }
                    return 2;
                };
                #endregion
            }

            string decodeName(SecsMessage msg)
            {
                //Console.WriteLine("SecsDecoder.decodeName");
                string name = "Unknown";

                if (msg == null) return name;
                if (msg.S == 1)
                {
                    switch (msg.F)
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
                if (msg.S == 2)
                {
                    switch (msg.F)
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
                if (msg.S == 3)
                {
                    switch (msg.F)
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
                if (msg.S == 5)
                {
                    switch (msg.F)
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
                if (msg.S == 6)
                {
                    switch (msg.F)
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
                if (msg.S == 7)
                {
                    switch (msg.F)
                    {
                        case 1:
                            name = "ProcessProgramLoadInquire";
                            break;
                        case 2:
                            name = "ProcessProgramLoadGrant";
                            break;
                        case 3:
                            name = "ProcessProgramSend";
                            break;
                        case 4:
                            name = "ProcessProgramAck";
                            break;
                        case 5:
                            name = "ProcessProgramRequest";
                            break;
                        case 6:
                            name = "ProcessProgramData";
                            break;
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
                if (msg.S == 9)
                {
                    switch (msg.F)
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
                    }
                }
                if (msg.S == 10)
                {
                    switch (msg.F)
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
                if (msg.S == 14)
                {
                    switch (msg.F)
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
                if (msg.S == 16)
                {
                    switch (msg.F)
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

            void ProcessMessage() {
                //Console.WriteLine("SecsDecoder.ProcessMessage");
                DataMsgHandler(_msgHeader, _msg);
                _msg = null;
                _messageLength = 0;
            }

            static bool CheckAvailable(int length, int index, int requireCount, out int need) {
                //Console.WriteLine("SecsDecoder.CheckAvailable");
                need = requireCount - (length - index);
                return need <= 0;
            }

            public void Reset() {
                //Console.WriteLine("SecsDecoder.Reset");
                _msg = null;
                _stack.Clear();
                _currentStep = 0;
                _remainBytes = new ArraySegment<byte>();
                _messageLength = 0;
            }

            /// <summary>
            /// Offset: next fill index
            /// Cout : next fill count
            /// </summary>
            ArraySegment<byte> _remainBytes;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="bytes">Byte</param>
            /// <param name="index">유효한 Bit의 시작 Index</param>
            /// <param name="length">유효 Bit 길이</param>
            /// <returns>디코딩 후에도 입력 Byte가 여전히 부족하면 true, 그렇지 않으면 false 반환</returns>
            public bool Decode(byte[] bytes, int index, int length) {
                //Console.WriteLine("SecsDecoder.Decode");
                if (_remainBytes.Count == 0) {
                    int need = Decode(bytes, length, ref index);
                    int remainLength = length - index;
                    if (remainLength > 0) {
                        var temp = new byte[remainLength + need];
                        Array.Copy(bytes, index, temp, 0, remainLength);
                        _remainBytes = new ArraySegment<byte>(temp, remainLength, need);
                        Trace.WriteLine("Remain Lenght: " + _remainBytes.Offset + ", Need:" + _remainBytes.Count);
                    } else {
                        _remainBytes = new ArraySegment<byte>();
                    }
                } else if (length - index >= _remainBytes.Count) {
                    Array.Copy(bytes, index, _remainBytes.Array, _remainBytes.Offset, _remainBytes.Count);
                    index = _remainBytes.Count;
                    byte[] temp = _remainBytes.Array;
                    _remainBytes = new ArraySegment<byte>();
                    if (Decode(temp, 0, temp.Length))
                        Decode(bytes, index, length);
                } else {
                    int remainLength = length - index;
                    Array.Copy(bytes, index, _remainBytes.Array, _remainBytes.Offset, remainLength);
                    _remainBytes = new ArraySegment<byte>(_remainBytes.Array, _remainBytes.Offset + remainLength, _remainBytes.Count - remainLength);
                    Trace.WriteLine("Remain Lenght: " + _remainBytes.Offset + ", Need:" + _remainBytes.Count);
                }
                return _messageLength > 0;
            }

            int _currentStep;
            /// <summary>
            // decode pipeline 을 통해 Byte 처리
            /// </summary>
            /// <param name="bytes">Byte</param>
            /// <param name="length">유효 Bit Index 시작</param>
            /// <param name="index">Byte Index 시작</param>
            /// <returns>currentStep 이 부족한 byte 수</returns>
            int Decode(byte[] bytes, int length, ref int index) {
                //Console.WriteLine("SecsDecoder.Decode1");
                int need;
                int nexStep = _currentStep;
                do {
                    _currentStep = nexStep;
                    nexStep = decoders[_currentStep](bytes, length, ref index, out need);
                } while (nexStep != _currentStep);
                return need;
            }
        }
        #endregion
        #region Message Header Struct
        struct Header {
            internal readonly byte[] Bytes;
            internal Header(byte[] headerbytes) {
                Bytes = headerbytes;
            }

            public short DeviceId {
                get {
                    return BitConverter.ToInt16(new[] { Bytes[1], Bytes[0] }, 0);
                }
                set {
                    byte[] values = BitConverter.GetBytes(value);
                    Bytes[0] = values[1];
                    Bytes[1] = values[0];
                }
            }
            public bool ReplyExpected {
                get { return (Bytes[2] & 0x80) == 0x80; }
                set { Bytes[2] = (byte)(S | (value ? 0x80 : 0)); }
            }
            public byte S {
                get { return (byte)(Bytes[2] & 0x7F); }
                set { Bytes[2] = (byte)(value | (ReplyExpected ? 0x80 : 0)); }
            }
            public byte F {
                get { return Bytes[3]; }
                set { Bytes[3] = value; }
            }
            public MessageType MessageType {
                get { return (MessageType)Bytes[5]; }
                set { Bytes[5] = (byte)value; }
            }
            public int SystemBytes {
                get {
                    return BitConverter.ToInt32(new[] { 
                        Bytes[9], 
                        Bytes[8], 
                        Bytes[7], 
                        Bytes[6] 
                    }, 0);
                }
                set {
                    byte[] values = BitConverter.GetBytes(value);
                    Bytes[6] = values[3];
                    Bytes[7] = values[2];
                    Bytes[8] = values[1];
                    Bytes[9] = values[0];
                }
            }
        }
        #endregion
        #region EncodedByteList Wrapper just need IList<T>.Count and Indexer
        sealed class EncodedBuffer : IList<ArraySegment<byte>> {
            readonly IList<RawData> _data;// raw data include first message length 4 byte
            readonly byte[] _header;

            internal EncodedBuffer(byte[] header, IList<RawData> msgRawDatas) {
                _header = header;
                _data = msgRawDatas;
            }

            #region IList<ArraySegment<byte>> Members
            int IList<ArraySegment<byte>>.IndexOf(ArraySegment<byte> item) { return -1; }
            void IList<ArraySegment<byte>>.Insert(int index, ArraySegment<byte> item) { }
            void IList<ArraySegment<byte>>.RemoveAt(int index) { }
            ArraySegment<byte> IList<ArraySegment<byte>>.this[int index] {
                get { return new ArraySegment<byte>(index == 1 ? _header : _data[index].Bytes); }
                set { }
            }
            #endregion
            #region ICollection<ArraySegment<byte>> Members
            void ICollection<ArraySegment<byte>>.Add(ArraySegment<byte> item) { }
            void ICollection<ArraySegment<byte>>.Clear() { }
            bool ICollection<ArraySegment<byte>>.Contains(ArraySegment<byte> item) { return false; }
            void ICollection<ArraySegment<byte>>.CopyTo(ArraySegment<byte>[] array, int arrayIndex) { }
            int ICollection<ArraySegment<byte>>.Count { get { return _data.Count; } }
            bool ICollection<ArraySegment<byte>>.IsReadOnly { get { return true; } }
            bool ICollection<ArraySegment<byte>>.Remove(ArraySegment<byte> item) { return false; }
            #endregion
            #region IEnumerable<ArraySegment<byte>> Members
            public IEnumerator<ArraySegment<byte>> GetEnumerator() {
                for (int i = 0, length = _data.Count; i < length; i++)
                    yield return new ArraySegment<byte>(i == 1 ? _header : _data[i].Bytes);
            }
            #endregion
            #region IEnumerable Members
            IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
            #endregion
        }
        #endregion
    }
}