using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net;
using Secs4Net;
using SecsDevice;
using System.Threading;
using System.Text.RegularExpressions;

namespace Radix
{
    /* DataType
     * Format	SEMI
octal	Hexadecimal	SML
1-LB	2-LB	3-LB	
List	00	01	02	03	L
Binary	10	21	22	23	B
Boolean	11	25	26	27	BOOLEAN
ASCII	20	41	42	43	A
JIS-8	21	45	46	47	J
8-byte Signed Integer	30	61	62	63	I8
1-byte Signed Integer	31	65	66	67	I1
2-byte Signed Integer	32	69	6A	6B	I2
4-byte Signed Integer	34	71	72	73	I4
8-byte Floating-Point	40	81	82	83	F8
4-byte Floating-Point	44	91	92	93	F4
8-byte Unsigned Integer	50	A1	A2	A3	U8
1-byte Unsigned Integer	51	A5	A6	A7	U1
2-byte Unsigned Integer	52	A9	AA	AB	U2
4-byte Unsigned Integer	54	B1	B2	B3	U4
     */

    #region Type 정의


    public enum enumVID // 변수 목록
    {
        ControlState = 1001, // 장비와 호스트간의 통신 상태
        TRSMode = 1002, // AGV 통신 상태
        EQPID = 1003, // 장비의 ID
        OperatorID = 1004, // 운영자 ID
        Time = 1005, // 시간값
        LotID = 1006, // LOT ID
        ProductID = 1007, // 제품 아이디
        EquipmentState = 1008, // 장비의 운영 상태
        ProcessState = 1009, // 처리과정 상태
        TotalProductCount = 1010, // 총 생산 카운트
        PortStateList = 1011, // 포트 상태, 포트별/스테이지별 제품ID 목록
        RecipeID = 1012, // 레시피(모델) ID
        RecipeIDList = 1013, // 레시피(모델) 목록
        ProcessJudge = 1014, // 처리 결과
        ProcessDataList = 1015, // 처리시 세부 내역
        ScrapCode = 1016, // 불량처리 코드
        ProductIDList = 1017, // 출고 한정 제품 목록
        IdleState = 1018, // 설비 가동상태
        IdleReason = 1019, // 설비 비가동 사유
        AlarmID = 1020, // 알람 아이디
        AlarmCD = 1021, // 알람 코드
        AlarmText = 1022 // 알람 텍스트
    }

    public enum enumECID // 레포트 항목
    {
        ControlStateChange = 1, // 통신상태 변경
        ProcessStateChange = 2, // 제품 처리 상태 변경
        EquipmentStateChange = 3, // 장비 상태 변경
        TRSModeChange = 4, // AGV 통신상태 변경
        PortStateChange = 5, // 포트별,스테이지별 제품 정보 변경시
        RecipeChange = 6, // 현재 레시피 변경
        ProductProcessStart = 7, // 제품 작업시작 보고
        ProductID = 8, // 작업 가능한 Product ID 보고
        ProcessCancel = 9, // 제품 작업 취소 보고
        ProcessAbort = 10, // 제품 투입 취소 보고
        ScrapReport = 11, // 제품 폐기정보 보고
        RecipeListChange = 12, // 설비에 등록된 Recipe 변경 발생시
        ProductProcessData = 13, // 제품 처리 결과
        ProductIDList = 14, // 출고한정 제품 목록
        IdleReasonReport = 15, // 설비 비가동 보고
        None = 16
    }

    public enum enumAck
    {
        ACK = 0,
        ParameterMismatch = 1,
        ParameterNotExist = 7,
        ValueOutOfRange = 9,
        ConditionDismatch = 10,
        PPIDNotExist = 26,
        PPIDNotAvailable = 30,
        EQPIDNotExist = 31,
        UnknownCommand = 32,
        ProductIDMismatch = 54,
        EquipmentDown = 71,
        EquipmentOffline = 72,
        AlreadyReceived = 74
    }

    public enum enumAlarmEnable
    {
        Disable = 0x00,
        Enable = 0x80
    }

    public enum enumPortID // PORT ID
    {
        M = 0,
        L = 1,
        U = 2
    }

    public enum enumAlarmCode // Alarm Code 8bit로 Set/Unset
    {
        NotUsed = 0,
        PersonalSafety = 1,
        EquipmentSafety = 2,
        ParameterControlWarning = 3,
        ParameterControlError = 4,
        IrrecoverableError = 5,
        EquipmentStatusWarning = 6,
        AttentionFlag = 7,
        DataIntegrity = 8
    }

    public enum enumAlarmID // 실제 알람 코드를 부여
    {
        No_Error = 0,
        Unknown_Error = 1,
        E_Stop_Pressed = 2,
        Door_Opened = 3,
        Servo_Disabled = 4,
        System_Initial_Error = 5,
        System_Not_Inited = 6,
        AGV_Communication_Failed = 7,
        Process_Timeout = 8
    }

    public enum enumControlState // Host Online Control State
    {
        OnlineRemote = 0,
        Offline = 1,
        None
    }

    public enum enumTransferControlMode // AGV Control Mode
    {
        Online = 0,
        Offline = 1,
        None = 2 // 실제 보고에 넣지는 않지만 초기에 보고 위해
    }

    public enum enumEquipmentState // 현재 설비 동작 상태
    {
        Init = 0,   //초기화 상태
        Setup = 1,  //설정중 상태
        Ready = 2,  //Manual 상태
        Idle = 3,   //자동 운전 - Wait
        Run = 4,    //자동 운전 - 처리중
        Down = 5,   //자동 운전 - Error Stop
        PM = 6,      //복구 불능 에러(사용자가 전환)
        None = 7 // 실제 보고에 넣지는 않지만 초기에 보고 위해
    }

    public enum enumProcessState // 현재 설비 Process 운영 상태
    {
        Idle = 0,
        Excution = 1,
        Pause = 2,
        None = 3 // 실제 보고에 넣지는 않지만 초기에 보고 위해
    }

    public enum enumProcessJudge // 프로세스 결과. 장비별로 달라질 텐데?
    {
        OK = 0,
        NG = 1,
        Unknown = 2
    }

    public enum enumScrapCode // 세부 내역은?
    {
        Unknown = 0,
        None
    }

    public enum enumRemoteCommand // Host로부터 내려온 지령
    {
        None = 0, // 없음
        Start = 1, // 작업시작
        Cancel = 2 // 작업취소
    }

    public struct structRemoteCommand
    {
        public enumRemoteCommand Command;
        public String LotID;
        public String RecipeID;
        public String ProductID;
        public String CancelCode;
        public String CancelText;

        public structRemoteCommand(enumRemoteCommand cmd, string lot, string recipe, string product, string code, string text)
        {
            Command = cmd;
            LotID = lot;
            RecipeID = recipe;
            ProductID = product;
            CancelCode = code;
            CancelText = text;
        }
    }

    public struct structProductInfo
    {
        public string LotID;
        public string RecipeID;
        public string ProductID;

        public structProductInfo(string lot, string recipe, string product)
        {
            LotID = lot;
            RecipeID = recipe;
            ProductID = product;
        }
    }
    #endregion

    public class SECS
    {
        SecsGem _secsGem;
        readonly FormLog _logform = new FormLog();
        //readonly BindingList<RecvMessage> recvBuffer = new BindingList<RecvMessage>();

        #region private
        private string address = "127.0.0.1";
        private int port = 9999;
        private bool active = true;
        private int device = 0;
        public string state = "";
        private uint dataId = 1;

        private System.Threading.Timer timerSecs; // Thread Timer, 연결동작 구현용
        private bool closeThread = false;
        private int timerMethod = 0;
        private bool processing = false;

        private List<RecvMessage> lstUnreplyMsg = new List<RecvMessage>(); // 응답해야 할 메시지 리스트
        private string txtRecvSecondary = "";
        private string txtSendPrimary = "";
        private string txtReplySeconary = "";
        private string txtRecvPrimary = "";

        private bool[] ReportEnable = new bool[100]; // 각 CEID별 Report enable 여부
        private uint[,] ReportIDs = new uint[100, 100]; // 각 CEID별 report ID
        private uint[,] ReportDesc = new uint[100, 100]; // 각 Report ID별 vid 목록
        private bool[] AlarmEnabled = new bool[100]; // 각 알람 ID별 활성화 여부
        private uint[] RequestSVID = new uint[100]; // status variable request 시 요청한 SVID들, 1001 부터
                                                    //public bool[] RequestECID = new bool[4]; // S2F13 Equipment Constant 1~4
                                                    //public uint[] equipmentConstants = { 30, 1, 1, 2 }; // 1~4  EstablishCommunicationTimeout,CtrtlMode,WakeUpMode,WakeUpSubMode
                                                    // public enumAck equpmentAck = enumAck.ACK;

        private string[] vidType = new string[3000]; // 각 vid별 타입 문자열, List 제외
        private string[] vidValue = new string[3000]; // 각 vid별 값 문자열, List 제외


        private enumControlState controlState = enumControlState.Offline;
        private enumControlState controlStateOld = enumControlState.None;

        private string SECS_EQPID = ""; // Equipment ID
        private string SECS_OperatorID = ""; // Operator ID
        private string SECS_LotID = ""; // LOT ID
        private string SECS_ProductID = ""; // Product ID

        private enumTransferControlMode SECS_TRSMode = enumTransferControlMode.Offline; // AGV Transfer Mode
        private enumTransferControlMode Old_TRSMode = enumTransferControlMode.None; // AGV Transfer Mode Old
        private enumEquipmentState SECS_EquipmentState = enumEquipmentState.Init; // Equipment State
        private enumEquipmentState Old_EquipmentState = enumEquipmentState.None; // Equipment State
        private enumProcessState SECS_ProcessState = enumProcessState.Idle; // 처리부 동작상태
        private enumProcessState Old_ProcessState = enumProcessState.None; // 처리부 동작상태

        private UInt32[] SECS_TotalProductCount = new UInt32[100]; // Lot별 총 생산 Count. YJ20210929 배열로 변경
        private string[] SECS_TotalProductLot = new string[100]; // YJ20210929 총 생산 Count 인덱스별 Lot ID

        private string SECS_RecipeID = ""; // Recipe ID

        //public static int SECS_RecipeCount = 0; // SECS_RecipeList 저장된 유효 recipe 갯수
        private enumProcessJudge SECS_ProcessJudge = enumProcessJudge.OK; // 처리부 결과
        private enumScrapCode SECS_ScrapCode = enumScrapCode.Unknown;
        private enumScrapCode Old_ScrapCode = enumScrapCode.None;

        private string SECS_Message = ""; // 서버로부터 전송되어서 출력해야 할 메시지
        private bool SECS_Message_Show = false; // Terminal Message로 Host로부터 전송된 메시지 출력 여부, True일 때 출력할 메시지 있음. 출력 후 False로 바꿔야 함

        private enumAlarmCode SECS_AlarmCode = enumAlarmCode.NotUsed; // 알람 코드. 종류만 구분하고 설정/해제(첫바이트값)는 함수에서 조합
        private uint SECS_AlarmID = 0;  // 알람내용에 따른 아이디값, 초기에 목록 설정 필요. enumAlarmID가 아니라 배열로 관리해야 할 듯
        private string SECS_AlarmText = ""; // 알람 표시 내용, 초기에 목록 설정 필요. enumAlarmID가 아니라 배열로 관리해야 할 듯
        private uint[] AlarmID = new uint[100];
        private string[] AlarmText = new string[100];

        private string[] SECS_ProcessDataName = new string[100]; // 처리 데이터 이름
        private string[] SECS_ProcessDataValue = new string[100]; // 처리 데이터 값

        private int[] SECS_Port_Stage_Count = new int[Enum.GetValues(typeof(enumPortID)).Length]; // 각 포트별 stage 갯수
        private structProductInfo[,] SECS_Port_Stage_State = new structProductInfo[Enum.GetValues(typeof(enumPortID)).Length, 100]; // 각 포트의 stage별 상태(Product ID)
        private structProductInfo[,] Old_Port_Stage_State = new structProductInfo[Enum.GetValues(typeof(enumPortID)).Length, 100]; // 각 포트의 stage별 이전 상태
        private enumProcessState[] SECS_PortState = new enumProcessState[3]; // 각 포트의 운영 상태값
        private string[] SECS_RecipeList = new string[100]; // Recipe List
        private string[] Old_RecipeList = new string[100]; // Recipe List
        private string[] SECS_ProductList = new string[100]; // ProductID List
        private string[] Old_ProductList = new string[100]; // ProductID List
        public string SECS_IdleState = "R"; // Idle State 'R' or 'I'
        public string Old_IdleState = ""; // Idle State 'R' or 'I'
        public string SECS_IdleReason = ""; // Idle State Reason
        public string Old_IdleReason = ""; // Idle State Reason
        #endregion

        #region public
        public bool established = false; // S1F13 establish 수행 여부
        public bool beforeEstablished = false;
        public bool selected = false; // 연결 전송 여부
        public bool beforeSelected = false; // 이전 연결 여부
        public bool disconnnected = false; // 연결 끊어짐
        public int conversationTime = Environment.TickCount; // Product ID Report 후 remote command 가 와야 함. 경과 시간 체크용
        public bool conversationSent = false; // conversion check 해야 할 상황
        public bool conversationChecked = false; // conversion check됨. UI 연계 후 외부에서 클리어할 것
        public structRemoteCommand RemoteCommand = new structRemoteCommand(enumRemoteCommand.None, "", "", "", "", ""); // 서버로부터 날아온 지령
        public bool firstPort = true; // YJ20211006 두 포트 사용할 경우 false. main에서 초기화할 때 지정해야 함.
        #endregion

        #region 초기화 관련
        private bool timerSecsDoing = false;

        public SECS()
        {
            defineVid();
        }

        public SECS(string Address, int Port, bool Active, int Device)
        {
            address = Address;
            port = Port;
            active = Active;
            device = Device;
            defineVid();

            #region 배열 초기화
            for (int j = 0; j < SECS_Port_Stage_State.GetLength(0); j++)
            {
                for (int i = 0; i < SECS_Port_Stage_State.GetLength(1); i++)
                {
                    SECS_Port_Stage_State[j, i] = new structProductInfo(); // null pointer 피하기 위해 빈 구조체로 초기화
                }
            }
            for (int i = 0; i < SECS_TotalProductLot.Length; i++)
            {
                SECS_TotalProductLot[i] = "";
                SECS_TotalProductCount[i] = 0;
            }
            #endregion
        }

        public void defineVid()
        {
            vidType[(int)enumVID.AlarmID] = "B";
            vidType[(int)enumVID.AlarmCD] = "U4";
            vidType[(int)enumVID.AlarmText] = "A";
            vidType[(int)enumVID.ControlState] = "U4";
            vidType[(int)enumVID.EQPID] = "A";
            vidType[(int)enumVID.EquipmentState] = "U4";
            vidType[(int)enumVID.LotID] = "A";
            vidType[(int)enumVID.OperatorID] = "A";
            vidType[(int)enumVID.PortStateList] = "L";
            vidType[(int)enumVID.ProcessDataList] = "L";
            vidType[(int)enumVID.ProcessJudge] = "U4";
            vidType[(int)enumVID.ProcessState] = "U4";
            vidType[(int)enumVID.ProductID] = "A";
            vidType[(int)enumVID.RecipeID] = "A";
            vidType[(int)enumVID.RecipeIDList] = "L";
            vidType[(int)enumVID.ScrapCode] = "A";
            vidType[(int)enumVID.Time] = "A";
            vidType[(int)enumVID.TotalProductCount] = "U4";
            vidType[(int)enumVID.TRSMode] = "U4";
            vidType[(int)enumVID.ProductIDList] = "L";
            vidType[(int)enumVID.IdleState] = "A";
            vidType[(int)enumVID.IdleReason] = "A";

            vidValue[(int)enumVID.AlarmID] = "0";
            vidValue[(int)enumVID.AlarmCD] = "0";
            vidValue[(int)enumVID.AlarmText] = "";
            vidValue[(int)enumVID.ControlState] = "0";
            vidValue[(int)enumVID.EQPID] = "";
            vidValue[(int)enumVID.EquipmentState] = "0";
            vidValue[(int)enumVID.LotID] = "";
            vidValue[(int)enumVID.OperatorID] = "";
            vidValue[(int)enumVID.PortStateList] = "";
            vidValue[(int)enumVID.ProcessDataList] = "";
            vidValue[(int)enumVID.ProcessJudge] = "0";
            vidValue[(int)enumVID.ProcessState] = "0";
            vidValue[(int)enumVID.ProductID] = "";
            vidValue[(int)enumVID.RecipeID] = "";
            vidValue[(int)enumVID.RecipeIDList] = "";
            vidValue[(int)enumVID.ScrapCode] = "";
            vidValue[(int)enumVID.Time] = DateTime.Now.ToString("yyyyMMddHHmmss");
            vidValue[(int)enumVID.TotalProductCount] = "0";
            vidValue[(int)enumVID.TRSMode] = "0";
            vidValue[(int)enumVID.ProductIDList] = "";
            vidValue[(int)enumVID.IdleState] = "R";
            vidValue[(int)enumVID.IdleReason] = "";
        }
        #endregion

        #region 시스템 함수
        private void TimerSecs(Object state) // 연결 초기 자동 송수신해야 할 경우. main으로 빼서 사용 안 함
        {
            while (!closeThread)
            {
                if (timerSecsDoing)
                {
                    return;
                }
                timerSecsDoing = true;
                //Console.WriteLine("selected : " + selected);
                //Console.WriteLine("beforeSelected : " + beforeSelected);
                //Console.WriteLine("established : " + established);
                //Console.WriteLine("beforeEstablished : " + beforeEstablished);
                if (!beforeSelected &&
                    !selected &&
                    _secsGem != null &&
                    _secsGem.State == ConnectionState.Selected) // 연결 초기에 연결요청 전송
                {
                    S1F13_EstablishCommunication();
                }
                beforeSelected = selected;
                if (_secsGem == null ||
                    _secsGem.State != ConnectionState.Selected) // 연결상태 아니면 연결 정보 초기화
                {
                    selected = false;
                    beforeSelected = false;
                    established = false;
                    beforeEstablished = false;

                    if (firstPort)
                    {
                        GlobalVar.SecsTimeReceived = false; // YJ20210917 연결 초기에 다시 시간값 수신 이후 레시피 전송 위해
                        GlobalVar.SecsRecipeListSent = false; // YJ20210917 연결 초기에 다시 시간값 수신 이후 레시피 전송 위해
                    }
                    else
                    {
                        GlobalVar.SecsTimeReceived_2 = false; // YJ20210917 연결 초기에 다시 시간값 수신 이후 레시피 전송 위해
                        GlobalVar.SecsRecipeListSent_2 = false; // YJ20210917 연결 초기에 다시 시간값 수신 이후 레시피 전송 위해
                    }
                }

                // YJ20210917 연결 후 시간값 수신 후 레시피 정보 안 보냈으면 보낸다.
                if (firstPort &&
                    GlobalVar.SecsTimeReceived && // GlobalVar.SecsTimeReceived 를 true 로 만드는 부분은 enable의 s2f31 파트에서 한다.
                    !GlobalVar.SecsRecipeListSent)
                {
                    // 모델 목록 조회
                    Func.GetModelList();

                    // 모델 목록 Secs에 세팅
                    ClearSECS_RecipeList();
                    for (int i = 0; i < GlobalVar.ModelCount; i++)
                    {
                        SetSECS_RecipeList(GlobalVar.ModelNames[i]);
                    }

                    // 모델 목록 전송
                    S6F11_EventReport(enumECID.RecipeListChange);

                    // 현재 모델 전송
                    SetSECS_RecipeID(GlobalVar.ModelName);
                    S6F11_EventReport(enumECID.RecipeChange);

                    // 다시 실행되지 않게 전송 완료 변수 세팅
                    GlobalVar.SecsRecipeListSent = true;
                }
                // YJ20211006 두번째 포트 연결 후 시간값 수신 후 레시피 정보 안 보냈으면 보낸다.
                if (!firstPort &&
                    GlobalVar.SecsTimeReceived_2 && // GlobalVar.SecsTimeReceived 를 true 로 만드는 부분은 enable의 s2f31 파트에서 한다.
                    !GlobalVar.SecsRecipeListSent_2)
                {
                    // 모델 목록 조회
                    Func.GetModelList();

                    // 모델 목록 Secs에 세팅
                    ClearSECS_RecipeList();
                    for (int i = 0; i < GlobalVar.ModelCount; i++)
                    {
                        SetSECS_RecipeList(GlobalVar.ModelNames[i]);
                    }

                    // 모델 목록 전송
                    S6F11_EventReport(enumECID.RecipeListChange);

                    // 현재 모델 전송
                    SetSECS_RecipeID(GlobalVar.ModelName_2);
                    S6F11_EventReport(enumECID.RecipeChange);

                    // 다시 실행되지 않게 전송 완료 변수 세팅
                    GlobalVar.SecsRecipeListSent_2 = true;
                }


                if (_secsGem != null &&
                    _secsGem.State == ConnectionState.Selected &&
                    timerMethod > 0)
                {
                    try
                    {
                        switch (timerMethod)
                        {
                            case 1:
                                #region Mode Change to Online Local
                                #region Establish Communication Request 대기
                                while (_secsGem.State != ConnectionState.Selected ||
                                        !established)
                                {
                                    Thread.Sleep(100);
                                }
                                #endregion
                                #region Are You There
                                //btnRUThere_Click(new object(), new EventArgs());
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


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine(ex.StackTrace);
                    }
                    timerMethod = 0;
                }

                timerSecsDoing = false;
                Thread.Sleep(100);
            }

        }

        public void Enable()
        {
            #region 쓰레드 타이머 시작
            TimerCallback CallBackSecs = new TimerCallback(TimerSecs);
            timerSecs = new System.Threading.Timer(CallBackSecs, false, 0, 1000);
            #endregion

            try
            {
                _logform.Show();

                if (_secsGem != null)
                    _secsGem.Dispose();

                _secsGem = new SecsGem(IPAddress.Parse(address), port, active, (primaryMsg, reply) =>
                {
                    lstUnreplyMsg.Add(new RecvMessage
                    {
                        Msg = primaryMsg,
                        ReplyAction = reply
                    });

                    //if (!beforeSelected &&
                    //    _secsGem.State == ConnectionState.Selected) // 연결 초기에 연결요청 전송
                    //{
                    //    S1F13_EstablishCommunication();
                    //    beforeSelected = true;
                    //}
                    //if (_secsGem.State != ConnectionState.Selected) // 연결상태 아니면 연결 정보 초기화
                    //{
                    //    beforeSelected = false;
                    //    established = false;
                    //    beforeEstablished = false;
                    //}

                    if (firstPort && // YJ20211006 첫포트일 경우
                        GlobalVar.UseConversation &&
                        conversationSent &&
                        Environment.TickCount - conversationTime > GlobalVar.ConversationTimeout) // 원격지령 대기시간 초과 체크
                    {
                        S9F13_ConversationTimeout();
                        conversationChecked = true;
                        conversationSent = false;
                    }
                    if (!firstPort && // YJ20211006 첫포트일 경우
                        GlobalVar.UseConversation_2 &&
                        conversationSent &&
                        Environment.TickCount - conversationTime > GlobalVar.ConversationTimeout) // 원격지령 대기시간 초과 체크
                    {
                        S9F13_ConversationTimeout();
                        conversationChecked = true;
                        conversationSent = false;
                    }

                    #region 수신한 primary message 에 따른 동작, 수신 데이터에 따라 값 설정해야 할 경우
                    Thread.Sleep(500);
                    switch (primaryMsg.S)
                    {
                        case 1:
                            switch (primaryMsg.F)
                            {
                                case 1:// Are You There
                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                                case 2:
                                    break;
                                case 3:// Selected Equipment Status Request
                                    RequestSVID = new uint[Enum.GetValues(typeof(enumVID)).Length];
                                    try
                                    {
                                        if (primaryMsg.SecsItem.Items.Count == 0) // 리스트 비어 있으면 모든 SVID를 응답에 포함
                                        {
                                            //for (int i = 0; i < RequestSVID.Length; i++)
                                            //{
                                            //    RequestSVID[i] = true;
                                            //}
                                        }
                                        else // 리스트 비어 있지 않으면 응답에 포함할 SVID 확인
                                        {
                                            for (int i = 0; i < primaryMsg.SecsItem.Items.Count; i++)
                                            {
                                                RequestSVID[i] = (uint)primaryMsg.SecsItem.Items[i];
                                            }
                                        }
                                        lstUnreplyMsg_select();
                                        ReplySecondary();
                                    }
                                    catch
                                    {
                                        // YJ20210914 sencondary 정보 지워 줘야 송수신 순서가 얽히지 않는다.
                                        lstUnreplyMsg_select();
                                        CancelSecondary();

                                        // 파싱 에러 서버로 통보
                                        S9F7_IllegalData();
                                        break;
                                    }
                                    break;
                                case 4:
                                    break;

                                case 13:// Establish Communication Request
                                    established = true;
                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                                case 14:
                                    established = true;
                                    break;

                                case 0:// Abort Transaction
                                    break;

                                default: // 해당 stream 없음
                                    //S9F5_UnrecognizedFunctionType();
                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                            }
                            break;
                        case 2:
                            switch (primaryMsg.F)
                            {

                                case 31: // Date and Time Send (DTS)
                                    try
                                    {
                                        string time = (string)primaryMsg.SecsItem;
                                        FuncWin.SetTime("YYYYMMDDhhmmss", time);

                                        if (firstPort)
                                        {
                                            GlobalVar.SecsTimeReceived = true; // YJ20210917 연결초기 시간 수신 후 레시피리스트 자동 전송하기 위해
                                        }
                                        else
                                        {
                                            GlobalVar.SecsTimeReceived_2 = true; // YJ20210917 연결초기 시간 수신 후 레시피리스트 자동 전송하기 위해
                                        }

                                        lstUnreplyMsg_select();
                                        ReplySecondary();
                                    }
                                    catch
                                    {
                                        // YJ20210914 sencondary 정보 지워 줘야 송수신 순서가 얽히지 않는다.
                                        lstUnreplyMsg_select();
                                        CancelSecondary();

                                        // 파싱 에러 서버로 통보
                                        S9F7_IllegalData();
                                        break;
                                    }

                                    break;
                                case 32:
                                    break;

                                case 33:// Define Report
                                    try
                                    {
                                        if (primaryMsg.SecsItem.Items[1].Count == 0) // 리스트 비어 있으면 전체 레포트 초기화
                                        {
                                            for (int j = 0; j < ReportDesc.GetLength(0); j++)
                                            {
                                                for (int i = 0; i < ReportDesc.GetLength(1); i++)
                                                {
                                                    ReportDesc[j, i] = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int i = 0; i < primaryMsg.SecsItem.Items[1].Items.Count; i++)
                                            {
                                                for (int j = 0; j < primaryMsg.SecsItem.Items[1].Items[i].Items[1].Items.Count; j++)
                                                {
                                                    ReportDesc[(int)primaryMsg.SecsItem.Items[1].Items[i].Items[0], j] = (uint)primaryMsg.SecsItem.Items[1].Items[i].Items[1].Items[j];
                                                    //Console.WriteLine("report desc " + primaryMsg.SecsItem.Items[1].Items[i].Items[0].ToString() + "," + j + " ==> " + primaryMsg.SecsItem.Items[1].Items[i].Items[1].Items[j].ToString());
                                                }
                                            }
                                        }
                                        lstUnreplyMsg_select();
                                        ReplySecondary();
                                    }
                                    catch (Exception ex)
                                    {
                                        // YJ20210914 sencondary 정보 지워 줘야 송수신 순서가 얽히지 않는다.
                                        lstUnreplyMsg_select();
                                        CancelSecondary();

                                        Console.WriteLine(ex.ToString());
                                        Console.WriteLine(ex.StackTrace);
                                        // 파싱 에러 서버로 통보
                                        S9F7_IllegalData();
                                        break;
                                    }
                                    break;
                                case 34:
                                    break;

                                case 35:// Link Event Report
                                    try
                                    {
                                        if (primaryMsg.SecsItem.Items[1].Items.Count == 0)
                                        {
                                            ReportIDs = new uint[100, 100]; // 리스트 비어 있으면 전체 초기화
                                        }
                                        else
                                        {
                                            for (int i = 0; i < primaryMsg.SecsItem.Items[1].Items.Count; i++) // CEID별로
                                            {
                                                uint ceid = (uint)primaryMsg.SecsItem.Items[1].Items[i].Items[0];
                                                for (int j = 0; j < primaryMsg.SecsItem.Items[1].Items[i].Items[1].Items.Count; j++)
                                                {
                                                    ReportIDs[(int)primaryMsg.SecsItem.Items[1].Items[i].Items[0], j] = (uint)primaryMsg.SecsItem.Items[1].Items[i].Items[1].Items[j];
                                                    //Console.WriteLine("report link " + (uint)primaryMsg.SecsItem.Items[1].Items[i].Items[0] + "," + j + " ==> " + primaryMsg.SecsItem.Items[1].Items[i].Items[1].Items[j].ToString());
                                                }
                                            }
                                        }
                                        lstUnreplyMsg_select();
                                        ReplySecondary();
                                    }
                                    catch
                                    {
                                        // YJ20210914 sencondary 정보 지워 줘야 송수신 순서가 얽히지 않는다.
                                        lstUnreplyMsg_select();
                                        CancelSecondary();

                                        // 파싱 에러 서버로 통보
                                        S9F7_IllegalData();
                                        break;
                                    }
                                    break;
                                case 36:
                                    break;

                                case 37:// Enable/Disable Event
                                    try
                                    {
                                        //bool enabled = primaryMsg.SecsItem.Items[0].ToString() == "1"; 활성여부가 아니라 dataId임
                                        if (primaryMsg.SecsItem.Items[1].Count == 0) // 리스트 비어 있으면 전체 레포트에 적용
                                        {
                                            for (int i = 0; i < ReportEnable.Length; i++)
                                            {
                                                ReportEnable[i] = false;
                                            }
                                        }
                                        else // 리스트 있으면 각 이벤트 지정된 것만 적용
                                        {
                                            for (int i = 0; i < primaryMsg.SecsItem.Items[1].Items.Count; i++)
                                            {
                                                ReportEnable[(int)primaryMsg.SecsItem.Items[1].Items[i]] = true;
                                            }
                                        }
                                        lstUnreplyMsg_select();
                                        ReplySecondary();
                                    }
                                    catch
                                    {
                                        // YJ20210914 sencondary 정보 지워 줘야 송수신 순서가 얽히지 않는다.
                                        lstUnreplyMsg_select();
                                        CancelSecondary();

                                        // 파싱 에러 서버로 통보
                                        S9F7_IllegalData();
                                        break;
                                    }
                                    break;
                                case 38:
                                    break;

                                case 41: // Host Command Send (HCS)
                                    try
                                    {
                                        enumRemoteCommand cmd = enumRemoteCommand.None;
                                        switch (primaryMsg.SecsItem.Items[0].ToString().ToUpper())
                                        {
                                            case "START":
                                                cmd = enumRemoteCommand.Start;
                                                break;
                                            case "CANCEL":
                                                cmd = enumRemoteCommand.Cancel;
                                                break;
                                            default:
                                                break;
                                        }

                                        structRemoteCommand rcmd = new structRemoteCommand(cmd, "", "", "", "", "");

                                        for (int i = 0; i < primaryMsg.SecsItem.Items[1].Items.Count; i++) // command는 이미 했으므로
                                        {
                                            switch (primaryMsg.SecsItem.Items[1].Items[i].Items[0].ToString().ToUpper()) // name
                                            {
                                                case "LOTID":
                                                    rcmd.LotID = primaryMsg.SecsItem.Items[1].Items[i].Items[1].ToString(); // value
                                                    break;
                                                case "PPID":
                                                    rcmd.RecipeID = primaryMsg.SecsItem.Items[1].Items[i].Items[1].ToString(); // value
                                                    break;
                                                case "PRODUCTID":
                                                    rcmd.ProductID = primaryMsg.SecsItem.Items[1].Items[i].Items[1].ToString(); // value
                                                    break;
                                                case "CODE":
                                                    rcmd.CancelCode = primaryMsg.SecsItem.Items[1].Items[i].Items[1].ToString(); // value
                                                    break;
                                                case "TEXT":
                                                    rcmd.CancelText = primaryMsg.SecsItem.Items[1].Items[i].Items[1].ToString(); // value
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        RemoteCommand = rcmd; // 각 패러미터 세팅 모두 끝나고 command를 바꿔준다.

                                        lstUnreplyMsg_select();
                                        ReplySecondary();
                                    }
                                    catch
                                    {
                                        // YJ20210914 sencondary 정보 지워 줘야 송수신 순서가 얽히지 않는다.
                                        lstUnreplyMsg_select();
                                        CancelSecondary();

                                        // 파싱 에러 서버로 통보
                                        S9F7_IllegalData();
                                        break;
                                    }

                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                                case 42:
                                    break;

                                case 0:// Abort Transaction
                                    break;

                                default: // 해당 stream 없음
                                    //S9F5_UnrecognizedFunctionType();
                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                            }
                            break;
                        case 5:
                            switch (primaryMsg.F)
                            {
                                case 3:// Enable/Disable Alarm Send (EAS)
                                    try
                                    {
                                        bool enabled = primaryMsg.SecsItem.Items[0].ToString() == "80";
                                        if (primaryMsg.SecsItem.Items[1].Count == 0) // 리스트 비어 있으면 전체 알람에 적용
                                        {
                                            for (int i = 0; i < AlarmEnabled.Length; i++)
                                            {
                                                AlarmEnabled[i] = enabled;
                                            }
                                        }
                                        else // 리스트 있으면 각 이벤트 지정된 것만 적용
                                        {
                                            for (int i = 0; i < primaryMsg.SecsItem.Items[1].Items.Count; i++)
                                            {
                                                AlarmEnabled[(int)primaryMsg.SecsItem.Items[1].Items[i]] = enabled;
                                            }
                                        }
                                        lstUnreplyMsg_select();
                                        ReplySecondary();
                                    }
                                    catch
                                    {
                                        // YJ20210914 sencondary 정보 지워 줘야 송수신 순서가 얽히지 않는다.
                                        lstUnreplyMsg_select();
                                        CancelSecondary();

                                        // 파싱 에러 서버로 통보
                                        S9F7_IllegalData();
                                        break;
                                    }
                                    break;
                                case 4:
                                    break;

                                case 0:// Abort Transaction
                                    break;

                                default: // 해당 stream 없음
                                    //S9F5_UnrecognizedFunctionType();
                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                            }
                            break;

                        case 7:
                            switch (primaryMsg.F)
                            {
                                case 3:// Process Program Send
                                    try
                                    {
                                        /*
                                        for (int i = 0; i < primaryMsg.SecsItem.Items.Count; i++)
                                        {
                                            Console.WriteLine("process program : " + primaryMsg.SecsItem.Items[i]);
                                        }
                                        //*/
                                        lstUnreplyMsg_select();
                                        ReplySecondary();
                                    }
                                    catch
                                    {
                                        // YJ20210914 sencondary 정보 지워 줘야 송수신 순서가 얽히지 않는다.
                                        lstUnreplyMsg_select();
                                        CancelSecondary();

                                        // 파싱 에러 서버로 통보
                                        S9F7_IllegalData();
                                        break;
                                    }
                                    break;
                                case 4:
                                    break;

                                case 19:// Current EPPD Request (PER)
                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                                case 20:
                                    break;

                                case 0:// Abort Transaction
                                    break;

                                default: // 해당 stream 없음
                                    //S9F5_UnrecognizedFunctionType();
                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                            }
                            break;

                        case 10:
                            switch (primaryMsg.F)
                            {
                                case 3:// Terminal Display, Single (VTN)
                                    SECS_Message = primaryMsg.SecsItem.Items[1].ToString();
                                    SECS_Message_Show = true;

                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                                case 4:
                                    break;

                                case 0:// Abort Transaction
                                    break;

                                default: // 해당 stream 없음
                                    //S9F5_UnrecognizedFunctionType();
                                    lstUnreplyMsg_select();
                                    ReplySecondary();
                                    break;
                            }
                            break;
                        default:
                            //S9F3_UnrecognizedStreamType();
                            lstUnreplyMsg_select();
                            ReplySecondary();
                            break;
                    }
                    #endregion
                },
                    _logform.Logger, 0);

                _secsGem.ConnectionChanged += delegate
                {
                    state = _secsGem.State.ToString();
                    if (_secsGem.State == ConnectionState.Retry)
                    {
                        disconnnected = true;
                    }
                    else
                    if (_secsGem.State == ConnectionState.Selected)
                    {
                        disconnnected = false;
                    }
                    Console.WriteLine("ConnectionChanged : " + _secsGem.State.ToString());
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
                timerSecs.Dispose();
            }
        }

        public bool Disconnected() // 비정상 접속 종료 여부. 수시로 콜해서 접종 직후는 메시지처리 등을 해야 함.
        {
            bool con = disconnnected;
            disconnnected = false;
            return con;
        }

        public void lstUnreplyMsg_select()
        {
            try
            {
                if (lstUnreplyMsg.Count == 0)
                {
                    return;
                }
                RecvMessage recv = lstUnreplyMsg[0] as RecvMessage;
                if (recv == null)
                    return;
                txtRecvPrimary = recv.Msg.ToSML();
                processing = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        public Item JoinItem(Item item1, Item item2)
        {
            try
            {
                // item1은 List이어야 함
                string str1 = Util.EraseLastLine(Util.EraseLastLine(new SecsMessage(1, 1, "join", item1).ToSML())); // List 이므로 마지막 > 와 . 을 삭제
                string str2 = Util.EraseLastLine(Util.EraseFirstLine(new SecsMessage(1, 1, "join", item2).ToSML())); // 헤더와 마지막 . 을 삭제
                string join = str1 + str2 + "\n>\n.";
                //Console.WriteLine("str1 : " + str1);
                //Console.WriteLine("str2 : " + str2);
                join = join.ToSecsMessage().ToSML().Replace("\n      <U1 [1] 1>", "\n      <U4 [1] 1>").Replace("\n    <U1 [1] 1>", "\n      <U4 [1] 1>");
                //Console.WriteLine("join : " + join);
                //Console.WriteLine("msg : " + join.ToSecsMessage().ToSML());
                return join.ToSecsMessage().SecsItem; // 강제로 u4 ==> u1 바뀌어서
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return item1;
        }

        public Item MakeItem(string type, string value)
        {
            if (type == "A")
            {
                return Item.A(value);
            }
            if (type == "U1")
            {
                try
                {
                    return Item.U1(byte.Parse(value));
                }
                catch
                {
                    return Item.U1(0);
                }
            }
            if (type == "U2")
            {
                try
                {
                    return Item.U2(byte.Parse(value));
                }
                catch
                {
                    return Item.U2(0);
                }
            }
            if (type == "U4")
            {
                try
                {
                    return Item.U4(uint.Parse(value));
                }
                catch
                {
                    return Item.U4(0);
                }
            }
            if (type == "U8")
            {
                try
                {
                    return Item.U8(uint.Parse(value));
                }
                catch
                {
                    return Item.U8(0);
                }
            }
            if (type == "B")
            {
                try
                {
                    return Item.B(byte.Parse(value));
                }
                catch
                {
                    return Item.B(0);
                }
            }
            if (type == "F4")
            {
                try
                {
                    return Item.F4(float.Parse(value));
                }
                catch
                {
                    return Item.F4(0);
                }
            }
            if (type == "F8")
            {
                try
                {
                    return Item.F8(float.Parse(value));
                }
                catch
                {
                    return Item.F8(0);
                }
            }
            if (type == "Boolean")
            {
                try
                {
                    return Item.Boolean(bool.Parse(value));
                }
                catch
                {
                    return Item.Boolean(false);
                }
            }
            if (type == "I1")
            {
                try
                {
                    return Item.I1(sbyte.Parse(value));
                }
                catch
                {
                    return Item.I1(0);
                }
            }
            if (type == "I2")
            {
                try
                {
                    return Item.I2(sbyte.Parse(value));
                }
                catch
                {
                    return Item.I2(0);
                }
            }
            if (type == "I4")
            {
                try
                {
                    return Item.I4(sbyte.Parse(value));
                }
                catch
                {
                    return Item.I4(0);
                }
            }
            if (type == "I8")
            {
                try
                {
                    return Item.I8(sbyte.Parse(value));
                }
                catch
                {
                    return Item.I8(0);
                }
            }
            return Item.L();
        }

        public void ReplySecondary()
        {
            if (lstUnreplyMsg.Count == 0)
            {
                return;
            }
            RecvMessage recv = lstUnreplyMsg[0] as RecvMessage;
            if (recv == null)
            {
                return;
            }

            txtReplySeconary = "";

            switch (recv.Msg.S) // 각 호스트 메시지에 대해서 응답 데이터를 보내야 할 경우
            {
                case 1:
                    switch (recv.Msg.F)
                    {
                        case 1: // Are You There
                            Item item = Item.L(
                                Item.A(GlobalVar.SecsName), 
                                Item.A(GlobalVar.SecsVersion)
                            ); // YJ20211006 첫번째 포트인 경우
                            if (!firstPort)
                            {
                                item = Item.L(
                                    Item.A(GlobalVar.SecsName_2),
                                    Item.A(GlobalVar.SecsVersion)
                                ); // YJ20211006 두 번째 포트인 경우
                            }
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML(); // YJ20211006 두 포트 사용경우 장비명을 다르게
                            break;
                        case 3: // Selected Equipment Status Request
                            item = Item.L(); // YJ20211006 선언은 위에서

                            //VID 순서대로 되어 있으면 안 됨.
                            // 받은 순서대로 할 것
                            for (int i = 0; i < RequestSVID.Length; i++)
                            {
                                if (RequestSVID[i] == (uint)enumVID.ControlState) // svid 1001 ControlState
                                {
                                    Item varControlState = Item.U4((uint)controlState);
                                    item = JoinItem(item, varControlState);
                                }
                                if (RequestSVID[i] == (uint)enumVID.TRSMode) // svid 1002 TRSMode
                                {
                                    Item varTRSMode = Item.U4((UInt32)SECS_TRSMode);
                                    item = JoinItem(item, varTRSMode);
                                }
                                if (RequestSVID[i] == (uint)enumVID.EQPID) // svid 1003 Equpment ID
                                {
                                    Item varEQPID = Item.A(SECS_EQPID);
                                    item = JoinItem(item, varEQPID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.OperatorID) // svid 1004 Operator ID
                                {
                                    Item varOPID = Item.A(SECS_OperatorID);
                                    item = JoinItem(item, varOPID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.Time) // svid 1005 Time
                                {
                                    Item clock = Item.A(DateTime.Now.ToString("yyyyMMddHHmmss"));
                                    item = JoinItem(item, clock);
                                }
                                if (RequestSVID[i] == (uint)enumVID.LotID) // svid 1006 LOT ID
                                {
                                    Item varLotID = Item.A(SECS_LotID);
                                    item = JoinItem(item, varLotID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ProductID) // svid 1007 Product ID
                                {
                                    Item varProductID = Item.A(SECS_ProductID);
                                    item = JoinItem(item, varProductID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.EquipmentState) // svid 1008 Equpment State
                                {
                                    Item varEQState = Item.U4((UInt32)SECS_EquipmentState);
                                    item = JoinItem(item, varEQState);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ProcessState) // svid 1009 Process State  ?????
                                {
                                    Item varProcessState = Item.U4((UInt32)SECS_ProcessState);
                                    item = JoinItem(item, varProcessState);
                                }
                                if (RequestSVID[i] == (uint)enumVID.TotalProductCount) // svid 1010 Total Product Count
                                {
                                    Item varCount = Item.U4((UInt32)GetSECS_TotalProductCount(SECS_LotID)); // YJ20210929 배열 조회로 변경
                                    item = JoinItem(item, varCount);
                                }
                                if (RequestSVID[i] == (uint)enumVID.PortStateList) // svid 1011 Port State List. 형식은 고정? 구성은?
                                {
                                    /*
                                    Item portList = Item.L();
                                    for (int i = 0; i < SECS_PortState.Length; i++)
                                    {
                                        JoinItem(portList, Item.U4((UInt32)SECS_PortState[i]));
                                    }
                                    item = JoinItem(item, portList);
                                    //*/
                                    Item list = Item.L();
                                    foreach (enumPortID port in Enum.GetValues(typeof(enumPortID)))
                                    {
                                        Item sub_list = Item.L();

                                        Item id_list = Item.L();
                                        id_list = JoinItem(id_list, Item.A("PortID"));
                                        id_list = JoinItem(id_list, Item.A(((int)port).ToString()));
                                        sub_list = JoinItem(sub_list, id_list);

                                        Item type_list = Item.L();
                                        type_list = JoinItem(type_list, Item.A("PortType"));
                                        type_list = JoinItem(type_list, Item.A(port.ToString()));
                                        sub_list = JoinItem(sub_list, type_list);

                                        for (int j = 0; j < SECS_Port_Stage_Count[(int)port]; j++)
                                        {
                                            Item stage_list = Item.L();
                                            stage_list = JoinItem(stage_list, Item.A(Util.FillZero(j + 1, 2)));
                                            stage_list = JoinItem(stage_list, Item.A(SECS_Port_Stage_State[(int)port, j].ProductID));
                                            sub_list = JoinItem(sub_list, stage_list);
                                        }
                                        list = JoinItem(list, sub_list);
                                    }
                                    item = JoinItem(item, list);
                                }
                                if (RequestSVID[i] == (uint)enumVID.RecipeID) // svid 1012 Recipe ID
                                {
                                    Item varRecipeID = Item.A(SECS_RecipeID);
                                    item = JoinItem(item, varRecipeID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.RecipeIDList) // svid 1013 Recipe List. 형식은 고정? 구성은?
                                {
                                    Item recipeList = Item.L();
                                    for (int j = 0; j < SECS_RecipeList.Length; j++)
                                    {
                                        if (SECS_RecipeList[j].Length > 0)
                                        {
                                            JoinItem(recipeList, Item.A(SECS_RecipeList[j]));
                                        }
                                    }
                                    item = JoinItem(item, recipeList);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ProcessJudge) // svid 1014 Process Judge
                                {
                                    Item varJudge = Item.U4((UInt32)SECS_ProcessJudge);
                                    item = JoinItem(item, varJudge);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ProcessDataList) // svid 1015 Process Data List. 형식은? 구성은? 장비별로 달라지겠지?
                                {
                                    Item dataList = Item.L();
                                    /*
                                    for (int i = 0; i < GlobalVar.SECS_RecipeCount; i++)
                                    {
                                        JoinItem(recipeList, Item.A(GlobalVar.SECS_RecipeList[i]));
                                    }
                                    //*/
                                    item = JoinItem(item, dataList);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ScrapCode) // svid 1016 Scrap Code, 구분은 어떻게?
                                {
                                    Item varCode = Item.U4((UInt32)SECS_ScrapCode);
                                    item = JoinItem(item, varCode);
                                }
                                if (RequestSVID[i] == (uint)enumVID.AlarmID) // svid 1017 Alarm ID
                                {
                                    Item varAlarm = Item.U4(SECS_AlarmID);
                                    item = JoinItem(item, varAlarm);
                                }
                                if (RequestSVID[i] == (uint)enumVID.AlarmCD) // svid 1018 Alarm Code
                                {
                                    Item varAlarm = Item.U4((byte)SECS_AlarmCode);
                                    item = JoinItem(item, varAlarm);
                                }
                                if (RequestSVID[i] == (uint)enumVID.AlarmText) // svid 1019 Alarm Text
                                {
                                    Item varAlarm = Item.A(SECS_AlarmText);
                                    item = JoinItem(item, varAlarm);
                                }
                            }

                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML();
                            break;

                        case 13: // Establish Communication Request

                            item = Item.L(
                                Item.B((byte)enumAck.ACK),
                                Item.L(
                                    Item.A(GlobalVar.SecsName),
                                    Item.A(GlobalVar.SecsVersion)
                                )
                            ); // YJ20211006 첫번째 포트인 경우
                            if (!firstPort)
                            {
                                item = Item.L(
                                    Item.B((byte)enumAck.ACK),
                                    Item.L(
                                        Item.A(GlobalVar.SecsName_2),
                                        Item.A(GlobalVar.SecsVersion)
                                    )
                                ); // YJ20211006 두 번째 포트인 경우
                            }
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML();
                            break;

                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 2:
                    switch (recv.Msg.F)
                    {
                        case 31: // Date and Time Set Request
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 33: // Define Report
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 35: // Link Event Report
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 37: // Enable/Disable Event
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 41: // Host Command Send
                            Item item = Item.L(
                                Item.B((byte)enumAck.ACK)
                            );
                            Item sub = Item.L();
                            if (RemoteCommand.LotID.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("LOTID"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            if (RemoteCommand.RecipeID.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("PPID"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            if (RemoteCommand.ProductID.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("PRODUCTID"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            if (RemoteCommand.CancelCode.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("CODE"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            if (RemoteCommand.CancelText.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("TEXT"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            item = JoinItem(item, sub);


                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML();
                            break;
                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 5:
                    switch (recv.Msg.F)
                    {
                        case 3: // Enable/Disable Alarm Send
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 7:
                    switch (recv.Msg.F)
                    {
                        case 3: // Process Program Send
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 19: // Current EPPD Request
                            Item item = Item.L(
                                Item.B((byte)enumAck.ACK),
                                Item.A(SECS_RecipeID)
                            );
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML();
                            break;
                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 10:
                    switch (recv.Msg.F)
                    {
                        case 3: // Send Terminal Message
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 0: // Abort Transaction
                default:
                    txtReplySeconary = new SecsMessage(9, 3, decodeSecsName(9, 3)).ToSML();
                    break;
            }

            if (string.IsNullOrEmpty(txtReplySeconary))
            {
                txtRecvPrimary = "";
                lstUnreplyMsg.Remove(recv);
                processing = false;
                return;
            }

            recv.ReplyAction(txtReplySeconary.ToSecsMessage());
            lstUnreplyMsg.Remove(recv);
            txtRecvPrimary = "";
            processing = false;
        }

        public void CancelSecondary() // YJ 도착한 Primary에 대한 응답을 제거하지 않으면 다음 정상 응답에 이전 Primary의 정보를 가져가므로 삭제한다.
        {
            if (lstUnreplyMsg.Count == 0)
            {
                return;
            }
            RecvMessage recv = lstUnreplyMsg[0] as RecvMessage;
            if (recv == null)
            {
                return;
            }

            txtReplySeconary = "";

            switch (recv.Msg.S) // 각 호스트 메시지에 대해서 응답 데이터를 보내야 할 경우
            {
                case 1:
                    switch (recv.Msg.F)
                    {
                        case 1: // Are You There
                            Item item = Item.L(
                                Item.A(GlobalVar.SecsName), 
                                Item.A(GlobalVar.SecsVersion)
                            ); // YJ20211006 첫번째 포트인 경우
                            if (!firstPort)
                            {
                                item = Item.L(
                                    Item.A(GlobalVar.SecsName_2),
                                    Item.A(GlobalVar.SecsVersion)
                                ); // YJ20211006 두 번째 포트인 경우
                            }
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML();
                            break;
                        case 3: // Selected Equipment Status Request
                            item = Item.L(); 

                            //VID 순서대로 되어 있으면 안 됨.
                            // 받은 순서대로 할 것
                            for (int i = 0; i < RequestSVID.Length; i++)
                            {
                                if (RequestSVID[i] == (uint)enumVID.ControlState) // svid 1001 ControlState
                                {
                                    Item varControlState = Item.U4((uint)controlState);
                                    item = JoinItem(item, varControlState);
                                }
                                if (RequestSVID[i] == (uint)enumVID.TRSMode) // svid 1002 TRSMode
                                {
                                    Item varTRSMode = Item.U4((UInt32)SECS_TRSMode);
                                    item = JoinItem(item, varTRSMode);
                                }
                                if (RequestSVID[i] == (uint)enumVID.EQPID) // svid 1003 Equpment ID
                                {
                                    Item varEQPID = Item.A(SECS_EQPID);
                                    item = JoinItem(item, varEQPID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.OperatorID) // svid 1004 Operator ID
                                {
                                    Item varOPID = Item.A(SECS_OperatorID);
                                    item = JoinItem(item, varOPID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.Time) // svid 1005 Time
                                {
                                    Item clock = Item.A(DateTime.Now.ToString("yyyyMMddHHmmss"));
                                    item = JoinItem(item, clock);
                                }
                                if (RequestSVID[i] == (uint)enumVID.LotID) // svid 1006 LOT ID
                                {
                                    Item varLotID = Item.A(SECS_LotID);
                                    item = JoinItem(item, varLotID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ProductID) // svid 1007 Product ID
                                {
                                    Item varProductID = Item.A(SECS_ProductID);
                                    item = JoinItem(item, varProductID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.EquipmentState) // svid 1008 Equpment State
                                {
                                    Item varEQState = Item.U4((UInt32)SECS_EquipmentState);
                                    item = JoinItem(item, varEQState);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ProcessState) // svid 1009 Process State  ?????
                                {
                                    Item varProcessState = Item.U4((UInt32)SECS_ProcessState);
                                    item = JoinItem(item, varProcessState);
                                }
                                if (RequestSVID[i] == (uint)enumVID.TotalProductCount) // svid 1010 Total Product Count
                                {
                                    Item varCount = Item.U4((UInt32)GetSECS_TotalProductCount(SECS_LotID)); // YJ20210929 배열 조회로 변경
                                    item = JoinItem(item, varCount);
                                }
                                if (RequestSVID[i] == (uint)enumVID.PortStateList) // svid 1011 Port State List. 형식은 고정? 구성은?
                                {
                                    /*
                                    Item portList = Item.L();
                                    for (int i = 0; i < SECS_PortState.Length; i++)
                                    {
                                        JoinItem(portList, Item.U4((UInt32)SECS_PortState[i]));
                                    }
                                    item = JoinItem(item, portList);
                                    //*/
                                    Item list = Item.L();
                                    foreach (enumPortID port in Enum.GetValues(typeof(enumPortID)))
                                    {
                                        Item sub_list = Item.L();

                                        Item id_list = Item.L();
                                        id_list = JoinItem(id_list, Item.A("PortID"));
                                        id_list = JoinItem(id_list, Item.A(((int)port).ToString()));
                                        sub_list = JoinItem(sub_list, id_list);

                                        Item type_list = Item.L();
                                        type_list = JoinItem(type_list, Item.A("PortType"));
                                        type_list = JoinItem(type_list, Item.A(port.ToString()));
                                        sub_list = JoinItem(sub_list, type_list);

                                        for (int j = 0; j < SECS_Port_Stage_Count[(int)port]; j++)
                                        {
                                            Item stage_list = Item.L();
                                            stage_list = JoinItem(stage_list, Item.A(Util.FillZero(j + 1, 2)));
                                            stage_list = JoinItem(stage_list, Item.A(SECS_Port_Stage_State[(int)port, j].ProductID));
                                            sub_list = JoinItem(sub_list, stage_list);
                                        }
                                        list = JoinItem(list, sub_list);
                                    }
                                    item = JoinItem(item, list);
                                }
                                if (RequestSVID[i] == (uint)enumVID.RecipeID) // svid 1012 Recipe ID
                                {
                                    Item varRecipeID = Item.A(SECS_RecipeID);
                                    item = JoinItem(item, varRecipeID);
                                }
                                if (RequestSVID[i] == (uint)enumVID.RecipeIDList) // svid 1013 Recipe List. 형식은 고정? 구성은?
                                {
                                    Item recipeList = Item.L();
                                    for (int j = 0; j < SECS_RecipeList.Length; j++)
                                    {
                                        if (SECS_RecipeList[j].Length > 0)
                                        {
                                            JoinItem(recipeList, Item.A(SECS_RecipeList[j]));
                                        }
                                    }
                                    item = JoinItem(item, recipeList);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ProcessJudge) // svid 1014 Process Judge
                                {
                                    Item varJudge = Item.U4((UInt32)SECS_ProcessJudge);
                                    item = JoinItem(item, varJudge);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ProcessDataList) // svid 1015 Process Data List. 형식은? 구성은? 장비별로 달라지겠지?
                                {
                                    Item dataList = Item.L();
                                    /*
                                    for (int i = 0; i < GlobalVar.SECS_RecipeCount; i++)
                                    {
                                        JoinItem(recipeList, Item.A(GlobalVar.SECS_RecipeList[i]));
                                    }
                                    //*/
                                    item = JoinItem(item, dataList);
                                }
                                if (RequestSVID[i] == (uint)enumVID.ScrapCode) // svid 1016 Scrap Code, 구분은 어떻게?
                                {
                                    Item varCode = Item.U4((UInt32)SECS_ScrapCode);
                                    item = JoinItem(item, varCode);
                                }
                                if (RequestSVID[i] == (uint)enumVID.AlarmID) // svid 1017 Alarm ID
                                {
                                    Item varAlarm = Item.U4(SECS_AlarmID);
                                    item = JoinItem(item, varAlarm);
                                }
                                if (RequestSVID[i] == (uint)enumVID.AlarmCD) // svid 1018 Alarm Code
                                {
                                    Item varAlarm = Item.U4((byte)SECS_AlarmCode);
                                    item = JoinItem(item, varAlarm);
                                }
                                if (RequestSVID[i] == (uint)enumVID.AlarmText) // svid 1019 Alarm Text
                                {
                                    Item varAlarm = Item.A(SECS_AlarmText);
                                    item = JoinItem(item, varAlarm);
                                }
                            }

                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML();                          
                            break;

                        case 13: // Establish Communication Request
                            item = Item.L(
                                Item.B((byte)enumAck.ACK),
                                Item.L(
                                    Item.A(GlobalVar.SecsName),
                                    Item.A(GlobalVar.SecsVersion)
                                )
                            ); // YJ20211006 첫번째 포트인 경우
                            if (!firstPort)
                            {
                                item = Item.L(
                                    Item.B((byte)enumAck.ACK),
                                    Item.L(
                                        Item.A(GlobalVar.SecsName_2),
                                        Item.A(GlobalVar.SecsVersion)
                                    )
                                ); // YJ20211006 두 번째 포트인 경우
                            }
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML();
                            break;

                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 2:
                    switch (recv.Msg.F)
                    {
                        case 31: // Date and Time Set Request
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 33: // Define Report
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 35: // Link Event Report
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 37: // Enable/Disable Event
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 41: // Host Command Send
                            Item item = Item.L(
                                Item.B((byte)enumAck.ACK)
                            );
                            Item sub = Item.L();
                            if (RemoteCommand.LotID.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("LOTID"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            if (RemoteCommand.RecipeID.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("PPID"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            if (RemoteCommand.ProductID.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("PRODUCTID"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            if (RemoteCommand.CancelCode.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("CODE"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            if (RemoteCommand.CancelText.Length > 0)
                            {
                                Item list = Item.L(
                                    Item.A("TEXT"),
                                    Item.B((byte)enumAck.ACK)
                                );
                                sub = JoinItem(sub, list);
                            }
                            item = JoinItem(item, sub);


                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML();
                            break;
                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 5:
                    switch (recv.Msg.F)
                    {
                        case 3: // Enable/Disable Alarm Send
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 7:
                    switch (recv.Msg.F)
                    {
                        case 3: // Process Program Send
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 19: // Current EPPD Request
                            Item item = Item.L(
                                Item.B((byte)enumAck.ACK),
                                Item.A(SECS_RecipeID)
                            );
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), item).ToSML();
                            break;
                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 10:
                    switch (recv.Msg.F)
                    {
                        case 3: // Send Terminal Message
                            txtReplySeconary = new SecsMessage(recv.Msg.S, (byte)(recv.Msg.F + 1), decodeSecsName(recv.Msg.S, (byte)(recv.Msg.F + 1)), Item.B((byte)enumAck.ACK)).ToSML();
                            break;
                        case 0: // Abort Transaction
                            break;
                        default:
                            txtReplySeconary = new SecsMessage(9, 5, decodeSecsName(9, 5)).ToSML();
                            break;
                    }
                    break;
                case 0: // Abort Transaction
                default:
                    txtReplySeconary = new SecsMessage(9, 3, decodeSecsName(9, 3)).ToSML();
                    break;
            }

            lstUnreplyMsg.Remove(recv);
            txtRecvPrimary = "";
            processing = false;
        }


        public void Disable()
        {
            if (_secsGem != null)
            {
                _secsGem.Dispose();
                _secsGem = null;
            }
            state = "Disable";
            lstUnreplyMsg.Clear();

        }

        public void SendPrimary(string PrimaryMesssage)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                return;
            }

            try
            {

                var msg = PrimaryMesssage.ToSecsMessage();
                _secsGem.BeginSend(msg, ar =>
                {
                    try
                    {
                        var reply = _secsGem.EndSend(ar);
                        txtRecvSecondary = reply.ToSML();
                    }
                    catch (SecsException ex)
                    {
                        txtRecvSecondary = ex.Message;
                    }
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void SendPrimary(SecsMessage PrimaryMesssage)
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
                return;

            //var msg = PrimaryMesssage.ToSecsMessage();
            _secsGem.BeginSend(PrimaryMesssage, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                    #region 응답 따라서 처리
                    #endregion
                }
                catch (SecsException ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void SendSecondary(RecvMessage ReceivedMesssage)
        {
            if (ReceivedMesssage == null)
                return;

            try
            {
                SecsMessage replySecondary = null;

                #region 수신 메시지 따라서 응답 설정
                if (ReceivedMesssage.Msg.S == 1 && ReceivedMesssage.Msg.F == 1)
                {
                    //_secsGem.EndSend()
                }
                #endregion

                if (replySecondary == null)
                    return;

                _secsGem.Send(replySecondary);
                lstUnreplyMsg.Remove(ReceivedMesssage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }


        public static string decodeSecsName(byte s, byte f)
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
        #endregion

        #region 변수 세팅 - MCS로 보고해야 할 항목 중에서 변경이 될 경우 바로 콜해서 변수 변경해 주고, 해당 레포트 등 액션 실행시 바로 전송.
        public void ClearAlarmList() // 설비상 알람 목록 연동 클리어
        {
            AlarmID = new uint[100];
            AlarmText = new string[100];
        }
        public void AddAlarmList(uint id, string text) // 설비상 알람 목록 하나씩 추가
        {
            for (int i = 0; i < AlarmID.Length; i++)
            {
                if (AlarmID[i] == 0)
                {
                    AlarmID[i] = id;
                    AlarmText[i] = text;
                    return;
                }
            }
        }

        public void SetControlState(enumControlState state) // MCS통신 상태 변경시 통신상태값 변경 
        {
            controlState = state;
        }
        public enumControlState GetControlState() // 현재 MCS통신 상태값 받기 
        {
            return controlState;
        }

        public void SetSECS_EQPID(string id) // 장비ID 변경 
        {
            SECS_EQPID = id;
            SECS_OperatorID = id;
        }
        public string GetSECS_EQPID() // 현재 장비ID 받기 
        {
            return SECS_EQPID;
        }

        public void SetSECS_OperatorID(string id) // 운영자ID 변경 
        {
            //SECS_OperatorID = id;
            SECS_EQPID = id;
        }
        public string GetSECS_OperatorID() // 현재 운영자ID 
        {
            //return SECS_OperatorID;
            return SECS_EQPID;
        }

        public void SetSECS_LotID(string id) // Lot ID 변경 
        {
            SECS_LotID = id;
        }
        public string GetSECS_LotID() // 현재 Lot ID 
        {
            return SECS_LotID;
        }

        public void SetSECS_ProductID(string id) // 제품ID 변경 
        {
            SECS_ProductID = id;
        }
        public string GetSECS_ProductID() // 현재 제품ID 
        {
            return SECS_ProductID;
        }

        public void SetSECS_TRSMode(enumTransferControlMode mode) // AGV통신상태 변경 
        {
            SECS_TRSMode = mode;
        }
        public enumTransferControlMode GetSECS_TRSMode() // 현재 AGV통신상태 
        {
            return SECS_TRSMode;
        }

        public void SetSECS_EquipmentState(enumEquipmentState mode) // 장비운영상태 변경 
        {
            SECS_EquipmentState = mode;
        }
        public enumEquipmentState GetSECS_EquipmentState() // 현재 장비운영상태 
        {
            return SECS_EquipmentState;
        }

        public void SetSECS_ProcessState(enumProcessState mode) // 장비의 생산처리 상태 변경 
        {
            SECS_ProcessState = mode;
        }
        public enumProcessState GetSECS_ProcessState() // 현재 장비의 생산처리 상태 
        {
            return SECS_ProcessState;
        }

        public int GetTotalProductIndex(string lot) // YJ20210929 로트별 총 생산량 검색시 사용할 인덱스 찾기
        {
            #region 이미 저장된 Lot가 있으면 해당 인덱스 리턴
            if (!String.IsNullOrEmpty(lot))
            {
                for (int i = 0; i < SECS_TotalProductLot.Length; i++)
                {
                    if (SECS_TotalProductLot[i] == lot)
                    {
                        return i;
                    }
                }
            }
            #endregion

            #region 이미 저장된 Lot가 없으면 새로 빈 자리 찾아서 그 자리에 lot 세팅하고 해당 인덱스 리턴
            int idx = -1;
            for (int i = 0; i < SECS_TotalProductLot.Length; i++)
            {
                if (String.IsNullOrEmpty(SECS_TotalProductLot[i]))
                {
                    SECS_TotalProductLot[i] = lot;
                    SECS_TotalProductCount[i] = 0;
                    idx = i;
                    break;
                }
            }
            return idx;
            #endregion
        }
        public void SetSECS_TotalProductCount(string lot, uint count) // YJ20210929 총 생산총계값 변경 
        {
            int idx = GetTotalProductIndex(lot);
            if (idx >= 0)
            {
                SECS_TotalProductCount[idx] = count;
            }
        }
        public void SetSECS_TotalProductCountAdd(string lot) // YJ20210929 총 생산총계값 증가
        {
            int idx = GetTotalProductIndex(lot);
            if (idx >= 0)
            {
                SECS_TotalProductCount[idx]++;
            }
        }
        public uint GetSECS_TotalProductCount(string lot) // YJ20210929 현재 총 생산총계값 
        {
            int idx = GetTotalProductIndex(lot);
            if (idx >= 0)
            {
                return SECS_TotalProductCount[idx];
            }
            return 0;
        }
        public void ClearSECS_TotalProductLot() // YJ20210929 총 생산량 저장할 LOT 정보 초기화
        {
            for (int i = 0; i < SECS_TotalProductLot.Length; i++)
            {
                SECS_TotalProductLot[i] = "";
            }
        }

        public void ClearSECS_TotalProductCount() // YJ20210929 총 생산량 값 초기화
        {
            for (int i = 0; i < SECS_TotalProductCount.Length; i++)
            {
                SECS_TotalProductCount[i] = 0;
            }
        }

        public void SetSECS_RecipeID(string id) // Recipe ID (모델) 변경 
        {
            SECS_RecipeID = id;
        }
        public string GetSECS_RecipeID() // 현재 Recipe ID(모델) 
        {
            return SECS_RecipeID;
        }

        public void SetSECS_ProcessJudge(enumProcessJudge judge) // 생산처리결과 변경 
        {
            SECS_ProcessJudge = judge;
        }
        public enumProcessJudge GetSECS_ProcessJudge() // 현재 생산처리결과 
        {
            return SECS_ProcessJudge;
        }

        public void SetSECS_ScrapCode(enumScrapCode code) // 폐기처리 코드 변경 
        {
            SECS_ScrapCode = code;
        }
        public enumScrapCode GetSECS_ScrapCode() // 현재 폐기처리 코드 
        {
            return SECS_ScrapCode;
        }

        public bool GetSECS_MessageExist() // 서버로부터 메시지 수신 여부 
        {
            return SECS_Message_Show;
        }
        public string GetSECS_Message() // 서버로부터 수신된 메시지. 함수 콜되면 수신여부는 초기화됨 
        {
            string msg = SECS_Message;
            SECS_Message_Show = false;
            return msg;
        }

        public void SetSECS_AlarmCode(enumAlarmCode code, bool set) // 알람코드값 변경 
        {
            SECS_AlarmCode = code;
            if (set)
            {
                SECS_AlarmCode += 80;
            }
        }

        public void SetSECS_AlarmID(uint id) // 알람 ID값 변경 
        {
            SECS_AlarmID = id;
        }

        public void SetSECS_AlarmText(string text) // 알람 텍스트 변경 
        {
            SECS_AlarmText = text;
        }

        public void ClearSECS_ProcessData() // 생산처리 데이터 초기화 
        {
            SECS_ProcessDataName = new string[100];
            SECS_ProcessDataValue = new string[100];
        }
        public void SetSECS_ProcessData(string name, string val) // 생산처리 데이터 추가 
        {
            if (name.Length == 0 ||
                val.Length == 0)
            {
                return;
            }
            for (int i = 0; i < SECS_ProcessDataName.Length; i++)
            {
                if (SECS_ProcessDataName[i] == null ||
                    SECS_ProcessDataName[i].Length == 0)
                {
                    SECS_ProcessDataName[i] = name;
                    SECS_ProcessDataValue[i] = val;
                    return;
                }
            }
        }

        public void ClearSECS_Port_Stage_Count() // 포트별 스테이지 데이터 초기화 
        {
            for (int i = 0; i < Enum.GetValues(typeof(enumPortID)).Length; i++)
            {
                SECS_Port_Stage_Count[i] = 0;
            }
        }
        public void SetSECS_Port_Stage_Count(enumPortID port, int count) // 포트별,스테이지별 제품 정보 변경 
        {
            SECS_Port_Stage_Count[(int)port] = count;
        }
        public int GetSECS_Port_Stage_Count(enumPortID port) // 포트별 스테이지 갯수 
        {
            return SECS_Port_Stage_Count[(int)port];
        }
        public void SetSECS_Port_Stage_Move(enumPortID fromPort, int fromStage, enumPortID toPort, int toStage) // 스테이지 하나를 다른 스테이지로 이동한 경우
        {
            try
            {
                SECS_Port_Stage_State[(int)toPort, toStage] = SECS_Port_Stage_State[(int)fromPort, fromStage];
                SECS_Port_Stage_State[(int)fromPort, fromStage] = new structProductInfo("", "", ""); // 출발 stage 정보를 지운다.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        // 특정 포트 회전식 스테이지간에 n회 이동 경우
        // YJ20210923 언로더 데이터는 직선 역방향 큐로 적용
        public void SetSECS_Port_Stage_Rotate(enumPortID port, int count)
        {
            if (port == enumPortID.L) // 로더는 정방향 회전식. 마지막 제품정보를 다시 첫 스테이지로 보낼 수도 있다.
            {
                for (int j = 0; j < count; j++) // 이동 횟수만큼
                {
                    structProductInfo temp = SECS_Port_Stage_State[(int)port, SECS_Port_Stage_Count[(int)port] - 1];
                    for (int i = SECS_Port_Stage_Count[(int)port]; i > 0; i--) // 범위 내 한 칸씩 이동
                    {
                        SECS_Port_Stage_State[(int)port, i] = SECS_Port_Stage_State[(int)port, i - 1]; // 제품 정보 이동
                        SECS_Port_Stage_State[(int)port, i - 1] = new structProductInfo("", "", ""); // 앞칸의 정보는 클리어
                    }
                    SECS_Port_Stage_State[(int)port, 0] = temp; // 마지막 스테이지 정보를 첫 스테이지로 넘긴다.
                }
            }
            else if (port == enumPortID.U) // 언로더는 역방향 큐 방식. 0번쪽으로 가능 갯수만큼 몰아 놓는다.
            {
                for (int i = GetSECS_Port_Stage_Count(enumPortID.U) - 1; i >= 0; i--) // 뒤에서부터
                {
                    int toIndex = -1;
                    for (int j = 0; j < i; j++) // 정방향(배출쪽)부터 빈 자리 찾는다.
                    {
                        structProductInfo info = GetSECS_Port_Stage_Product(enumPortID.U, j);
                        if (info.ProductID == "")
                        {
                            toIndex = j;
                            break;
                        }
                    }
                    if (toIndex >= 0)
                    {
                        SetSECS_Port_Stage_Move(enumPortID.U, i, enumPortID.U, toIndex);
                    }
                }
            }
        }
        public structProductInfo GetSECS_Port_Stage_Product(enumPortID port, int stage) // 특정 포트/스테이지의 제품ID 조회
        {
            try
            {
                return SECS_Port_Stage_State[(int)port, stage];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
            return new structProductInfo("", "", "");
        }

        public void ClearSECS_Port_Stage_State() // 전체 포트상태 데이터 초기화
        {
            for (int j = 0; j < SECS_Port_Stage_State.GetLength(0); j++)
            {
                for (int i = 0; i < SECS_Port_Stage_State.GetLength(1); i++)
                {
                    SECS_Port_Stage_State[j, i] = new structProductInfo("", "", "");
                }
            }
        }
        public void ClearSECS_Port_Stage_State(enumPortID port) // 특정 포트상태 데이터 초기화
        {
            for (int i = 0; i < SECS_Port_Stage_State.GetLength(1); i++)
            {
                SECS_Port_Stage_State[(int)port, i] = new structProductInfo("", "", "");
            }
        }
        public void SetSECS_Port_Stage_State(enumPortID port, int stage, string id, string lot, string recipe) // 포트별,스테이지별 제품ID 변경 
        {
            SECS_Port_Stage_State[(int)port, stage] = new structProductInfo(lot, recipe, id);
        }

        public void ClearSECS_PortState() // 포트의 처리상태값 초기화 
        {
            for (int i = 0; i < SECS_PortState.Length; i++)
            {
                SECS_PortState[i] = enumProcessState.Idle;
            }
        }
        public void SetSECS_PortState(enumPortID id, enumProcessState val) // 특정 포트의 처리상태값 변경 
        {
            SECS_PortState[(int)id] = val;
        }

        public void ClearSECS_RecipeList() // 설비상 처리할 recipe(모델) 목록 초기화 
        {
            for (int i = 0; i < SECS_RecipeList.Length; i++)
            {
                SECS_RecipeList[i] = "";
            }
        }
        public void SetSECS_RecipeList(string recipe) // 설비상 처리할 recipe(모델) 추가 
        {
            for (int i = 0; i < SECS_RecipeList.Length; i++)
            {
                if (SECS_RecipeList[i].Length == 0)
                {
                    SECS_RecipeList[i] = recipe;
                    return;
                }
            }
        }

        public void ClearSECS_ProductList() // 출고설비상 처리할 ProductID 목록 초기화 
        {
            for (int i = 0; i < SECS_ProductList.Length; i++)
            {
                SECS_ProductList[i] = "";
            }
        }
        public void SetSECS_ProductList(string product) // 출고설비상 처리할 ProductID 추가 
        {
            for (int i = 0; i < SECS_ProductList.Length; i++)
            {
                if (SECS_ProductList[i].Length == 0)
                {
                    SECS_ProductList[i] = product;
                    return;
                }
            }
        }

        public void SetSECS_IdleState(bool idle, string reason)
        {
            SECS_IdleState = idle ? "I" : "R";
            SECS_IdleReason = reason;
        }
        public bool GetSECS_IdleState()
        {
            return SECS_IdleState == "I";
        }
        #endregion


        #region Eq에서 Host로 엑션 발생시 전송할 함수들
        #region 통신 관련
        public void S1F1_RUThere() // 통신초기 및 수시 통신상태 확인용 
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.A(GlobalVar.SecsName),
                Item.A(GlobalVar.SecsVersion)
            ); // YJ20211006 첫번째 포트인 경우
            if (!firstPort)
            {
                item = Item.L(
                    Item.A(GlobalVar.SecsName_2),
                    Item.A(GlobalVar.SecsVersion)
                ); // YJ20211006 두 번째 포트인 경우
            }

            var msg = new SecsMessage(1, 1, decodeSecsName(1, 1), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void S1F13_EstablishCommunication() // 통신초기 실행 
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }
            selected = true;

            Item item = Item.L(
                Item.A(GlobalVar.SecsName),
                Item.A(GlobalVar.SecsVersion)
            ); // YJ20211006 첫번째 포트인 경우
            if (!firstPort)
            {
                item = Item.L(
                    Item.A(GlobalVar.SecsName_2),
                    Item.A(GlobalVar.SecsVersion)
                ); // YJ20211006 두 번째 포트인 경우
            }

            var msg = new SecsMessage(1, 13, decodeSecsName(1, 13), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);

            selected = true;
        }

        public void S9F1_UnrecognizedDeviceID()
        {
            _secsGem.Send(new SecsMessage(9, 1, decodeSecsName(9, 1), false, Item.L()));
        }

        public void S9F3_UnrecognizedStreamType()
        {
            _secsGem.Send(new SecsMessage(9, 3, decodeSecsName(9, 3), false, Item.L()));
        }

        public void S9F5_UnrecognizedFunctionType()
        {
            _secsGem.Send(new SecsMessage(9, 5, decodeSecsName(9, 5), false, Item.L()));
        }

        public void S9F7_IllegalData()
        {
            _secsGem.Send(new SecsMessage(9, 7, decodeSecsName(9, 7), false, Item.L()));
        }

        public void S9F9_TransactionTimerTimeout()
        {
            _secsGem.Send(new SecsMessage(9, 9, decodeSecsName(9, 9), false, Item.L()));
        }

        public void S9F13_ConversationTimeout()
        {
            Item item = Item.L(
                Item.A("S02F41"),
                Item.A("START")
                );
            _secsGem.Send(new SecsMessage(9, 13, decodeSecsName(9, 13), false, item));
        }

        #endregion

        #region 호스트 테스트용
        public void S1F3_StateValueRequest() // 호스트 테스트용 
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }
            Item item = Item.L(
                    Item.U4((uint)enumVID.ControlState),
                    Item.U4((uint)enumVID.EquipmentState),
                    Item.U4((uint)enumVID.TRSMode),
                    Item.U4((uint)enumVID.ProcessState),
                    Item.U4((uint)enumVID.PortStateList)
                );


            var msg = new SecsMessage(1, 3, decodeSecsName(1, 3), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void S2F31_DateTimeSend() // 호스트 테스트용 
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }
            Item item = Item.L(
                    Item.A(DateTime.Now.ToString("yyyyMMddHHmmss"))
                );


            var msg = new SecsMessage(2, 31, decodeSecsName(2, 31), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void S2F33_DefineReport() // 호스트 테스트용 
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.U4(dataId), // dataID, 1 fixed
                Item.L(
                    Item.L( // SlotMap Data
                        Item.U4(10), // Report ID
                        Item.L(
                            Item.U4(2101), // SlotState
                            Item.U4(2102), //PlateID
                            Item.U4(2103), // SlotState
                            Item.U4(2104), //PlateID
                            Item.U4(2105), // SlotState
                            Item.U4(2106), //PlateID
                            Item.U4(2107), // SlotState
                            Item.U4(2108) //PlateID
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

            var msg = new SecsMessage(2, 33, decodeSecsName(2, 33), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void S2F33_DeleteAllReport() // 호스트 테스트용 
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

            var msg = new SecsMessage(2, 33, decodeSecsName(2, 33), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void S2F34_DefineReportAck() // 호스트 테스트용 
        {
            Item item = Item.B(0);
            var msg = new SecsMessage(2, 34, decodeSecsName(2, 34), item);
            _secsGem.Send(msg);
        }

        public void S2F35_LinkEventReport() // 호스트 테스트용 
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }


            Item item = Item.L(
                Item.U4(dataId), // dataID, 1 fixed
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

            var msg = new SecsMessage(2, 35, decodeSecsName(2, 35), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void S2F36_LinkReportAck() // 호스트 테스트용 
        {
            Item item = Item.B(0);
            var msg = new SecsMessage(2, 36, decodeSecsName(2, 36), item);
            _secsGem.Send(msg);
        }

        public void S2F37_EnableAllEvent() // 호스트 테스트용 
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

            var msg = new SecsMessage(2, 37, decodeSecsName(2, 37), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void S2F37_DisableEvent() // 호스트 테스트용 
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

            var msg = new SecsMessage(2, 37, decodeSecsName(2, 37), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void S2F38_EnableReportAck() // 호스트 테스트용 
        {
            Item item = Item.B(0);
            var msg = new SecsMessage(2, 38, decodeSecsName(2, 38), item);
            _secsGem.Send(msg);
        }

        public void S5F3_EnableAlarmReportSend(bool enable) // 호스트 테스트용 
        {
            Item item = Item.L(
                Item.B(enable ? (byte)enumAlarmEnable.Enable : (byte)enumAlarmEnable.Disable),
                Item.L()
            );
            var msg = new SecsMessage(5, 3, decodeSecsName(5, 3), item);
            _secsGem.Send(msg);
        }


        public void S7F19_CurrentEPPDRequestFromHost() // 호스트 테스트용 
        {
            if (_secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            //*
            Item item = Item.L(
                Item.A("recipe1") // Recipe 목록 따라
            );
            _secsGem.Send(new SecsMessage(7, 19, decodeSecsName(7, 19), true, item));
        }

        public void S10F3_TerminalMessage() // 호스트 테스트용 
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            Item item = Item.L(
                Item.B((byte)enumAck.ACK),
                Item.A("Terminal Message")
            );

            var msg = new SecsMessage(10, 3, decodeSecsName(10, 3), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);

        }

        public void S10F4_TerminalReportAck() // 호스트 테스트용 
        {
            Item item = Item.B(0);
            var msg = new SecsMessage(10, 4, decodeSecsName(10, 4), item);
            _secsGem.Send(msg);
        }

        #endregion

        #region Equipment에서 상황에 따라 보낼 때
        public void S5F1_AlarmReportSend(bool set, enumAlarmCode AlarmCode, uint AlarmID, string AlarmText) // 알람 보고
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            byte code = (byte)AlarmCode;
            if (set)
            {
                code += (byte)enumAlarmEnable.Enable;
            }

            Item item = Item.L(
                    Item.B(code),
                    Item.U4(AlarmID),
                    Item.A(AlarmText)
                );
            var msg = new SecsMessage(5, 1, decodeSecsName(5, 1), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
        }

        public void S6F11_EventReport(enumECID ecid)
        {
            if ((uint)ecid != 1 &&
                (uint)ecid != 15 && // YJ20210928 추가
                !ReportEnable[(int)ecid])
            {
                Console.WriteLine("Report not enabled");
                return;
            }

            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                Console.WriteLine("Not Selected");
                return;
            }

            try
            {
                #region YJ20210928 Port List Report시 이전 보고내용과 같다면 무시. 첫 전송시는 모두 빈 데이터로 시작하므로 빈 데이터는 전송
                if (ecid == enumECID.PortStateChange)
                {
                    bool diff = false;
                    bool empty = true;
                    for (int i = 0; i < Enum.GetValues(typeof(enumPortID)).Length; i++)
                    {
                        for (int j = 0; j < SECS_Port_Stage_Count[i]; j++)
                        {
                            if (SECS_Port_Stage_State[i, j].ProductID != Old_Port_Stage_State[i, j].ProductID)
                            {
                                diff = true;
                            }
                            if (SECS_Port_Stage_State[i, j].ProductID.Length > 0)
                            {
                                empty = false;
                            }
                            Old_Port_Stage_State[i, j].ProductID = SECS_Port_Stage_State[i, j].ProductID;
                            Old_Port_Stage_State[i, j].RecipeID = SECS_Port_Stage_State[i, j].RecipeID;
                            Old_Port_Stage_State[i, j].LotID = SECS_Port_Stage_State[i, j].LotID;
                        }
                        if (diff)
                        {
                            break;
                        }
                    }
                    if (!diff && !empty)
                    {
                        Console.WriteLine("같은 포트 레포트 이미 전송했음");
                        return;
                    }
                }
                #endregion

                #region YJ20210928 TRSModeChange Report시 이전 보고내용과 같다면 무시.
                if (ecid == enumECID.TRSModeChange)
                {
                    if (SECS_TRSMode == enumTransferControlMode.None)
                    {
                        Console.WriteLine("TRS 레포트 해당사항 아님");
                        return;
                    }
                    if (SECS_TRSMode == Old_TRSMode)
                    {
                        Console.WriteLine("같은 TRS 레포트 이미 전송했음");
                        return;
                    }
                    Old_TRSMode = SECS_TRSMode;
                }
                #endregion

                #region YJ20210928 EquipmentStateChange Report시 이전 보고내용과 같다면 무시.
                if (ecid == enumECID.EquipmentStateChange)
                {
                    if (SECS_EquipmentState == enumEquipmentState.None)
                    {
                        Console.WriteLine("enumEquipmentState 레포트 해당사항 아님");
                        return;
                    }
                    if (SECS_EquipmentState == Old_EquipmentState)
                    {
                        Console.WriteLine("같은 enumEquipmentState 레포트 이미 전송했음");
                        return;
                    }
                    Old_EquipmentState = SECS_EquipmentState;
                }
                #endregion

                #region YJ20210928 ProcessStateChange Report시 이전 보고내용과 같다면 무시.
                if (ecid == enumECID.ProcessStateChange)
                {
                    if (SECS_ProcessState == enumProcessState.None)
                    {
                        Console.WriteLine("ProcessStateChange 레포트 해당사항 아님");
                        return;
                    }
                    if (SECS_ProcessState == Old_ProcessState)
                    {
                        Console.WriteLine("같은 ProcessStateChange 레포트 이미 전송했음");
                        return;
                    }
                    Old_ProcessState = SECS_ProcessState;
                }
                #endregion

                #region YJ20210929 ControlStateChange Report시 이전 보고내용과 같다면 무시.
                if (ecid == enumECID.ControlStateChange)
                {
                    if (controlState == enumControlState.None)
                    {
                        Console.WriteLine("ControlStateChange 레포트 해당사항 아님");
                        return;
                    }
                    if (controlState == controlStateOld)
                    {
                        Console.WriteLine("같은 ControlStateChange 레포트 이미 전송했음");
                        return;
                    }
                    controlStateOld = controlState;
                }
                #endregion

                #region YJ20210929 IdleReasonReport Report시 이전 보고내용과 같다면 무시.
                if (ecid == enumECID.IdleReasonReport)
                {
                    if (SECS_IdleState == "")
                    {
                        Console.WriteLine("SECS_IdleState 레포트 해당사항 아님");
                        return;
                    }
                    if (SECS_IdleState == Old_IdleState &&
                        SECS_IdleReason == Old_IdleReason)
                    {
                        Console.WriteLine("같은 SECS_IdleState 레포트 이미 전송했음");
                        return;
                    }
                    Old_IdleState = SECS_IdleState;
                    Old_IdleReason = SECS_IdleReason;
                }
                #endregion

                #region YJ20210929 RecipeListChange Report시 이전 보고내용과 같다면 무시. 최초 빈 리스트는 보고하지 않으므로 레시피 관리에서 하나 이상을 유지해야 한다.
                if (ecid == enumECID.RecipeListChange)
                {
                    bool same = true;
                    for (int i = 0; i < SECS_RecipeList.Length; i++)
                    {
                        if (SECS_RecipeList[i] != Old_RecipeList[i])
                        {
                            same = false;
                        }
                        Old_RecipeList[i] = SECS_RecipeList[i];
                    }
                    if (same)
                    {
                        Console.WriteLine("같은 RecipeListChange 레포트 이미 전송했음");
                        return;
                    }
                }
                #endregion

                #region YJ20210929 ProductIDList Report시 이전 보고내용과 같다면 무시.
                if (ecid == enumECID.ProductIDList)
                {
                    bool same = true;
                    for (int i = 0; i < SECS_ProductList.Length; i++)
                    {
                        if (SECS_ProductList[i] != Old_ProductList[i])
                        {
                            same = false;
                        }
                        Old_ProductList[i] = SECS_ProductList[i];
                    }
                    if (same)
                    {
                        Console.WriteLine("같은 SECS_ProductList 레포트 이미 전송했음");
                        return;
                    }
                }
                #endregion

                #region YJ20210929 ScrapReport Report시 이전 보고내용과 같다면 무시.
                if (ecid == enumECID.ScrapReport)
                {
                    if (SECS_ScrapCode == enumScrapCode.None)
                    {
                        Console.WriteLine("SECS_ScrapCode 레포트 해당사항 아님");
                        return;
                    }
                    if (SECS_ScrapCode == Old_ScrapCode)
                    {
                        Console.WriteLine("같은 SECS_ScrapCode 레포트 이미 전송했음");
                        return;
                    }
                    Old_ScrapCode = SECS_ScrapCode;
                }
                #endregion

                #region YJ20210929 ProcessAbort  ProcessCancel시 SECS_ProductID에 비어 있으면 무시.
                if ((ecid == enumECID.ProcessAbort || ecid == enumECID.ProcessCancel) &&
                    (SECS_ProductID == "" || SECS_ProductID == "UNKNOWN"))
                {
                    Console.WriteLine("Product ID가 없어서 보고 하지 않음");
                    return;                 
                }
                #endregion




                #region 전역변수에서 vid 별로 할당된 값을 복사
                vidValue[(int)enumVID.AlarmCD] = ((int)SECS_AlarmCode).ToString();
                vidValue[(int)enumVID.AlarmID] = ((int)SECS_AlarmID).ToString();
                vidValue[(int)enumVID.AlarmText] = SECS_AlarmText;
                vidValue[(int)enumVID.EQPID] = SECS_EQPID;
                vidValue[(int)enumVID.OperatorID] = SECS_OperatorID;
                vidValue[(int)enumVID.ProcessState] = ((int)SECS_ProcessState).ToString();
                vidValue[(int)enumVID.Time] = DateTime.Now.ToString("yyyyMMddHHmmss");
                vidValue[(int)enumVID.ControlState] = ((int)controlState).ToString();
                vidValue[(int)enumVID.EquipmentState] = ((int)SECS_EquipmentState).ToString();
                vidValue[(int)enumVID.LotID] = SECS_LotID;
                vidValue[(int)enumVID.ProcessJudge] = ((int)SECS_ProcessJudge).ToString();
                vidValue[(int)enumVID.ProductID] = SECS_ProductID;
                vidValue[(int)enumVID.RecipeID] = SECS_RecipeID;
                vidValue[(int)enumVID.ScrapCode] = ((int)SECS_ScrapCode).ToString();
                vidValue[(int)enumVID.TotalProductCount] = SECS_TotalProductCount[GetTotalProductIndex(SECS_LotID)].ToString();
                vidValue[(int)enumVID.TRSMode] = ((int)SECS_TRSMode).ToString();
                vidValue[(int)enumVID.IdleState] = SECS_IdleState;
                vidValue[(int)enumVID.IdleReason] = SECS_IdleReason;
                #endregion

                Item item = Item.L(
                );

                if (ecid == enumECID.ControlStateChange &&
                    !ReportEnable[(int)ecid]) // 레포트 정의 전에 컨트롤 상태 변경이면 내정값으로 전송
                {
                    item = JoinItem(item, Item.A(vidValue[(int)enumVID.EQPID]));
                    item = JoinItem(item, Item.A(vidValue[(int)enumVID.OperatorID]));
                    item = JoinItem(item, Item.A(vidValue[(int)enumVID.Time]));
                    item = JoinItem(item, Item.U4(uint.Parse(vidValue[(int)enumVID.ControlState])));
                }
                else if (ecid == enumECID.IdleReasonReport &&
                    !ReportEnable[(int)ecid]) // 레포트 정의 전에 Idle 상태 변경이면 내정값으로 전송
                {
                    item = JoinItem(item, Item.A(vidValue[(int)enumVID.EQPID]));
                    item = JoinItem(item, Item.A(vidValue[(int)enumVID.OperatorID]));
                    item = JoinItem(item, Item.A(vidValue[(int)enumVID.Time]));
                    item = JoinItem(item, Item.A(vidValue[(int)enumVID.IdleState]));
                    item = JoinItem(item, Item.A(vidValue[(int)enumVID.IdleReason]));
                }
                else // 레포트 정의 이후면
                {
                    for (int i = 0; i < ReportDesc.GetLength(1); i++)
                    {

                        if (ReportDesc[(int)ecid, i] > 0)
                        {

                            if (vidType[ReportDesc[(int)ecid, i]] == "B")
                            {
                                Item it = Item.B(byte.Parse(vidValue[ReportDesc[(int)ecid, i]]));
                                item = JoinItem(item, it);
                            }
                            else if (vidType[ReportDesc[(int)ecid, i]] == "A")
                            {
                                Item it = Item.A(vidValue[ReportDesc[(int)ecid, i]]);
                                item = JoinItem(item, it);
                            }
                            else if (vidType[ReportDesc[(int)ecid, i]] == "U4")
                            {
                                Item it = Item.U4(uint.Parse(vidValue[ReportDesc[(int)ecid, i]]));
                                item = JoinItem(item, it);
                            }
                            else if (vidType[ReportDesc[(int)ecid, i]] == "L")
                            {
                                Item list = Item.L();

                                // 리스트 종류 따라
                                switch (ReportDesc[(int)ecid, i])
                                {
                                    #region PortStateList
                                    case (uint)enumVID.PortStateList:
                                        foreach (enumPortID port in Enum.GetValues(typeof(enumPortID)))
                                        {
                                            Item sub_list = Item.L();

                                            Item id_list = Item.L();
                                            id_list = JoinItem(id_list, Item.A("PortID"));
                                            id_list = JoinItem(id_list, Item.A(((int)port).ToString()));
                                            sub_list = JoinItem(sub_list, id_list);

                                            Item type_list = Item.L();
                                            type_list = JoinItem(type_list, Item.A("PortType"));
                                            type_list = JoinItem(type_list, Item.A(port.ToString()));
                                            sub_list = JoinItem(sub_list, type_list);

                                            for (int j = 0; j < SECS_Port_Stage_Count[(int)port]; j++)
                                            {
                                                Item stage_list = Item.L();
                                                stage_list = JoinItem(stage_list, Item.A(Util.FillZero(j + 1, 2)));
                                                stage_list = JoinItem(stage_list, Item.A(SECS_Port_Stage_State[(int)port, j].ProductID));
                                                sub_list = JoinItem(sub_list, stage_list);
                                            }
                                            list = JoinItem(list, sub_list);
                                        }
                                        break;
                                    #endregion
                                    #region ProcessDataList
                                    case (uint)enumVID.ProcessDataList:
                                        Item sub = Item.L();
                                        for (int j = 0; j < SECS_ProcessDataName.Length; j++)
                                        {
                                            if (SECS_ProcessDataName[j] != null &&
                                                SECS_ProcessDataName[j].Length > 0)
                                            {
                                                Item process_list = Item.L();
                                                process_list = JoinItem(process_list, Item.A(SECS_ProcessDataName[j]));
                                                process_list = JoinItem(process_list, Item.A(SECS_ProcessDataValue[j]));
                                                sub = JoinItem(sub, process_list);
                                            }
                                        }
                                        list = JoinItem(list, sub);
                                        break;
                                    #endregion
                                    #region RecipeIDList
                                    case (uint)enumVID.RecipeIDList:
                                        for (int j = 0; j < SECS_RecipeList.Length; j++)
                                        {
                                            //Console.WriteLine("j : " + j);
                                            if (SECS_RecipeList[j] != null &&
                                                SECS_RecipeList[j].Length > 0)
                                            {
                                                //Console.WriteLine("recipe : " + SECS_RecipeList[j]);
                                                list = JoinItem(list, Item.A(SECS_RecipeList[j]));
                                                //Console.WriteLine("recipe added");
                                            }
                                        }
                                        break;
                                    #endregion
                                    #region ProductIDList
                                    case (uint)enumVID.ProductIDList:
                                        for (int j = 0; j < SECS_ProductList.Length; j++)
                                        {
                                            //Console.WriteLine("j : " + j);
                                            if (SECS_ProductList[j] != null &&
                                                SECS_ProductList[j].Length > 0)
                                            {
                                                //Console.WriteLine("recipe : " + SECS_RecipeList[j]);
                                                list = JoinItem(list, Item.A(SECS_ProductList[j]));
                                                //Console.WriteLine("recipe added");
                                            }
                                        }
                                        break;
                                        #endregion
                                }

                                item = JoinItem(item, list);
                            }
                        }
                    }
                }

                //Console.WriteLine("6");

                Item event_item = Item.L
                (
                    Item.U4(1),
                    Item.U4((uint)ecid),
                    Item.L(
                        Item.L(
                            Item.U4((uint)ecid),
                            item
                        )
                    )
                );

                var msg = new SecsMessage(6, 11, decodeSecsName(6, 11), event_item);
                _secsGem.BeginSend(msg, ar =>
                {
                    try
                    {
                        var reply = _secsGem.EndSend(ar);
                        txtRecvSecondary = reply.ToSML();
                    }
                    catch (SecsException ex)
                    {
                        txtRecvSecondary = ex.Message;
                    }
                }, null);

                if (firstPort && // 첫 포트인 경우
                    GlobalVar.UseConversation && 
                    ecid == enumECID.ProductID) // 제품 ID report 후 conversion timeout 체크 위해 변수 세팅. 장비따라 체크해야 할 경우는?
                {
                    conversationSent = true;
                    conversationTime = Environment.TickCount;
                }
                if (!firstPort && // 두번째 포트인 경우
                    GlobalVar.UseConversation_2 &&
                    ecid == enumECID.ProductID) // 제품 ID report 후 conversion timeout 체크 위해 변수 세팅. 장비따라 체크해야 할 경우는?
                {
                    conversationSent = true;
                    conversationTime = Environment.TickCount;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        // YJ20210923 레시피 목록이 변경되었을 때
        // 모델 추가 또는 삭제시
        public void SendRecipeList()
        {
            ClearSECS_RecipeList();
            Func.GetModelList();
            for (int i = 0; i < GlobalVar.ModelCount; i++)
            {
                SetSECS_RecipeList(GlobalVar.ModelNames[i]);
            }

            S7F3_ProcessProgramSend();
        }

        public void S7F3_ProcessProgramSend() // 유저action으로 레시피 목록이 변경되었을 경우. 자동운전중에는 S6F11로 
        {
            if (_secsGem == null ||
                _secsGem.State != ConnectionState.Selected)
            {
                MessageBox.Show("Not Selected");
                return;
            }

            //*
            Item item = Item.L(
            );

            for (int i = 0; i < SECS_RecipeList.Length; i++)
            {
                if (SECS_RecipeList[i] != null &&
                    SECS_RecipeList[i].Trim().Length > 0)
                {
                    item = JoinItem(item, Item.A(SECS_RecipeList[i].Trim()));
                }
            }
            //Console.WriteLine("S7F3 전");
            //Console.WriteLine("item " + item.ToString());
            //Console.WriteLine("message " + (new SecsMessage(7, 3, decodeSecsName(7, 3), true, item)).ToSML());


            var msg = new SecsMessage(7, 3, decodeSecsName(7, 3), item);
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
            //_secsGem.Send(new SecsMessage(7, 3, decodeSecsName(7, 3), true, item));
            //Console.WriteLine("S7F3 완");
        }

        // YJ20210923 모델 변경시 MCS로 전송
        public void SendCurrentRecipe()
        {
            if (firstPort)
            {
                SetSECS_EQPID(GlobalVar.SecsName); // OP ID도 EqID랑 같이 쓰기 때문에 EqID만 지정. 초기에 한번만 해도 된다.
            }
            else
            {
                SetSECS_EQPID(GlobalVar.SecsName_2); // OP ID도 EqID랑 같이 쓰기 때문에 EqID만 지정. 초기에 한번만 해도 된다.
            }
            SetSECS_RecipeID(GlobalVar.ModelName); // YJ20210923 모델 설정 변경
            S7F19_CurrentEPPDRequestToHost();
        }

        public void S7F19_CurrentEPPDRequestToHost() // 유저action으로 레시피가 변경되었을 경우. 자동운전중에는 S6F11로 
        {
            var msg = new SecsMessage(7, 19, decodeSecsName(7, 19), Item.L(Item.A(SECS_RecipeID)));
            _secsGem.BeginSend(msg, ar =>
            {
                try
                {
                    var reply = _secsGem.EndSend(ar);
                    txtRecvSecondary = reply.ToSML();
                }
                catch (SecsException ex)
                {
                    txtRecvSecondary = ex.Message;
                }
            }, null);
            //_secsGem.Send(new SecsMessage(7, 19, decodeSecsName(7, 19), true, Item.L(Item.A(SECS_RecipeID))));
        }

        public structRemoteCommand GetRemoteCommand() // 원격 지령 검색
        {
            structRemoteCommand cmd = RemoteCommand;
            if (cmd.Command != enumRemoteCommand.None)
            {
                RemoteCommand = new structRemoteCommand(enumRemoteCommand.None, "", "", "", "", ""); // 한번 조회하면 저장된 값은 클리어한다.
            }
            return cmd;
        }

        public bool CheckConveration() // product id report 후 
        {
            return conversationChecked;
        }
        public void ClearConverstaion() // conversation timeout 체크 이후 프로그램에서 정보 클리어
        {
            conversationChecked = false;
            conversationSent = false;
        }

        public int GetStageLoaderCount() // 로더(라벨기)에 처리할 제품이 몇개?
        {
            int count = 0;
            for (int i = 0; i < Math.Min(4, GetSECS_Port_Stage_Count(enumPortID.L)); i++)
            {
                if (GetSECS_Port_Stage_Product(enumPortID.L, i).ProductID.Length > 0)
                {
                    count++;
                }
            }
            return count;
        }
        public bool GetStageTotalExist() // 전체 스테이지에 하나라도 제품이 있는가?
        {
            int count = GetStageLoaderCount();

            //for (int i = 0; i < GetSECS_Port_Stage_Count(enumPortID.L); i++)
            //{
            //    if (GetSECS_Port_Stage_Product(enumPortID.L, i).ProductID.Length > 0)
            //    {
            //        count++;
            //    }
            //}

            for (int i = 0; i < GetSECS_Port_Stage_Count(enumPortID.M); i++)
            {
                if (GetSECS_Port_Stage_Product(enumPortID.M, i).ProductID.Length > 0)
                {
                    count++;
                }
            }

            //for (int i = 0; i < GetSECS_Port_Stage_Count(enumPortID.U); i++)
            //{
            //    if (GetSECS_Port_Stage_Product(enumPortID.U, i).ProductID.Length > 0)
            //    {
            //        count++;
            //    }
            //}

            return count > 0;
        }
        #endregion


        #endregion


    }

    public sealed class RecvMessage
    {
        public SecsMessage Msg { get; set; }
        public Action<SecsMessage> ReplyAction { get; set; }
    }
}