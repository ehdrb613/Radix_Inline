/****************************************************************************
*****************************************************************************
**
** File Name
** ---------
**
** AXM.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Ajinextek Motion Library Header File
** 
**
*****************************************************************************
*****************************************************************************
**
** Source Change IndicesAxmStatusGetCmdPos
** ---------------------
**
** (None)
**
*****************************************************************************
*****************************************************************************
**
** Website
** ---------------------
**
** http://www.ajinextek.com
**
*****************************************************************************
*****************************************************************************
*/

using System.Runtime.InteropServices;

public class CAXM
{
    //========== 보드 및 모듈 확인함수(Info) - Infomation ===============================================================
    // 해당 축의 보드번호, 모듈 위치, 모듈 아이디를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmInfoGetAxis(int nAxisNo, ref int npBoardNo, ref int npModulePos, ref uint upModuleID);
    //지정한 모듈 번호로 해당 모듈의 Sub ID, 모듈 Name, 모듈 설명을 확인한다.
    //======================================================/
    // 지원 제품            : EtherCAT
    // upModuleSubID        : EtherCAT 모듈을 구분하기 위한 SubID
    // szModuleName            : 모듈의 모델명(50 Bytes)
    // szModuleDescription  : 모듈에 대한 설명(80 Bytes)
    //======================================================//
    [DllImport("AXL.dll")] public static extern uint AxmInfoGetAxisEx(int nAxisNo, ref uint upModuleSubID, System.Text.StringBuilder szModuleName, System.Text.StringBuilder szModuleDescription);
    // 모션 모듈이 존재하는지 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmInfoIsMotionModule(ref uint upStatus);
    // 해당 축이 유효한지 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmInfoIsInvalidAxisNo(int nAxisNo);
    // 해당 축이 제어가 가능한 상태인지 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmInfoGetAxisStatus(int nAxisNo);
    // 축 개수, 시스템에 장착된 유효한 모션 축수를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmInfoGetAxisCount(ref int npAxisCount);
    // 해당 보드/모듈의 첫번째 축번호를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmInfoGetFirstAxisNo(int nBoardNo, int nModulePos, ref int npAxisNo);
    // 해당 보드의 첫번째 축번호를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmInfoGetBoardFirstAxisNo(int nBoardNo, int nModulePos, ref int npAxisNo);

    //========= 가상 축 함수 ============================================================================================
    // 초기 상태에서 AXM 모든 함수의 축번호 설정은 0 ~ (실제 시스템에 장착된 축수 - 1) 범위에서 유효하지만
    // 이 함수를 사용하여 실제 장착된 축번호 대신 임의의 축번호로 바꿀 수 있다.
    // 이 함수는 제어 시스템의 H/W 변경사항 발생시 기존 프로그램에 할당된 축번호를 그대로 유지하고 실제 제어 축의 
    // 물리적인 위치를 변경하여 사용을 위해 만들어진 함수이다.
    // 주의사항 : 여러 개의 실제 축번호에 대하여 같은 번호로 가상 축을 중복해서 맵핑할 경우 
    //            실제 축번호가 낮은 축만 가상 축번호로 제어 할 수 있으며, 
    //            나머지 같은 가상축 번호로 맵핑된 축은 제어가 불가능한 경우가 발생 할 수 있다.

    // 가상축을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmVirtualSetAxisNoMap(int nReanAxisNo, int nVirtuanAxisNo);
    // 설정한 가상채널(축) 번호를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmVirtualGetAxisNoMap(int nReanAxisNo, ref int npVirtuanAxisNo);
    // 멀티 가상축을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmVirtualSetMultiAxisNoMap(int nSize, int[] npRealAxesNo, int[] npVirtualAxesNo);
    // 설정한 멀티 가상채널(축) 번호를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmVirtualGetMultiAxisNoMap(int nSize, int[] npRealAxesNo, int[] npVirtualAxesNo);
    // 가상축 설정을 해지한다.
    [DllImport("AXL.dll")] public static extern uint AxmVirtualResetAxisMap();

    // 축 번호를 재정의한다.
    [DllImport("AXL.dll")] public static extern uint AxmVirtualSetMultiReDefineAxisNo(int lSize, int[] lpReDefineAxesNo);
    // 재정의된 축 번호를 가져온다.
    [DllImport("AXL.dll")] public static extern uint AxmVirtualGetMultiReDefineAxisNo(int lSize, int[] lpReDefineAxesNo);

    //========= 인터럽트 관련 함수 ======================================================================================
    // 콜백 함수 방식은 이벤트 발생 시점에 즉시 콜백 함수가 호출 됨으로 가장 빠르게 이벤트를 통지받을 수 있는 장점이 있으나
    // 콜백 함수가 완전히 종료 될 때까지 메인 프로세스가 정체되어 있게 된다.
    // 즉, 콜백 함수 내에 부하가 걸리는 작업이 있을 경우에는 사용에 주의를 요한다. 
    // 이벤트 방식은 쓰레드등을 이용하여 인터럽트 발생여부를 지속적으로 감시하고 있다가 인터럽트가 발생하면 
    // 처리해주는 방법으로, 쓰레드 등으로 인해 시스템 자원을 점유하고 있는 단점이 있지만
    // 가장 빠르게 인터럽트를 검출하고 처리해줄 수 있는 장점이 있다.
    // 일반적으로는 많이 쓰이지 않지만, 인터럽트의 빠른처리가 주요 관심사인 경우에 사용된다. 
    // 이벤트 방식은 이벤트의 발생 여부를 감시하는 특정 쓰레드를 사용하여 메인 프로세스와 별개로 동작되므로
    // MultiProcessor 시스템등에서 자원을 가장 효율적으로 사용할 수 있게 되어 특히 권장하는 방식이다.
    // 인터럽트 메시지를 받아오기 위하여 윈도우 메시지 또는 콜백 함수를 사용한다.
    // (메시지 핸들, 메시지 ID, 콜백함수, 인터럽트 이벤트)
    //    hWnd    : 윈도우 핸들, 윈도우 메세지를 받을때 사용. 사용하지 않으면 NULL을 입력.
    //    wMsg    : 윈도우 핸들의 메세지, 사용하지 않거나 디폴트값을 사용하려면 0을 입력.
    //    proc    : 인터럽트 발생시 호출될 함수의 포인터, 사용하지 않으면 NULL을 입력.
    //    pEvent  : 이벤트 방법사용시 이벤트 핸들
    // Ex)
    // AxmInterruptSetAxis(0, Null, 0, AxtInterruptProc, NULL);
    // void __stdcall AxtInterruptProc(long lAxisNo, DWORD dwFlag){
    //     ... ;
    // }
    [DllImport("AXL.dll")] public static extern uint AxmInterruptSetAxis(int nAxisNo, uint hWnd, uint uMessage, CAXHS.AXT_INTERRUPT_PROC pProc, ref uint pEvent);

    // 설정 축의 인터럽트 사용 여부를 설정한다
    // 해당 축에 인터럽트 설정 / 확인
    // uUse : 사용 유무 => DISABLE(0), ENABLE(1)
    [DllImport("AXL.dll")] public static extern uint AxmInterruptSetAxisEnable(int nAxisNo, uint uUse);
    // 설정 축의 인터럽트 사용 여부를 반환한다
    [DllImport("AXL.dll")] public static extern uint AxmInterruptGetAxisEnable(int nAxisNo, ref uint upUse);

    //인터럽트를 이벤트 방식으로 사용할 경우 해당 인터럽트 정보 읽는다.
    [DllImport("AXL.dll")] public static extern uint AxmInterruptRead(ref int npAxisNo, ref uint upFlag);

    // 해당 축의 인터럽트 플래그 값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmInterruptReadAxisFlag(int nAxisNo, int nBank, ref uint upFlag);

    // 지정 축의 사용자가 설정한 인터럽트 발생 여부를 설정한다.
    // lBank         : 인터럽트 뱅크 번호 (0 - 1) 설정가능.
    // uInterruptNum : 인터럽트 번호 설정 비트번호로 설정 hex값 혹은 define된값을 설정
    // AXHS.h파일에 INTERRUPT_BANK1, 2 DEF를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxmInterruptSetUserEnable(int nAxisNo, int nBank, uint uInterruptNum);

    // 지정 축의 사용자가 설정한 인터럽트 발생 여부를 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxmInterruptGetUserEnable(int nAxisNo, int nBank, ref uint upInterruptNum);

    //======== 모션 파라메타 설정 =======================================================================================
    // AxmMotLoadParaAll로 파일을 Load 시키지 않으면 초기 파라메타 설정시 기본 파라메타 설정. 
    // 현재 PC에 사용되는 모든축에 똑같이 적용된다. 기본파라메타는 아래와 같다. 
    // 00:AXIS_NO.             =0          01:PULSE_OUT_METHOD.    =4         02:ENC_INPUT_METHOD.    =3     03:INPOSITION.          =2
    // 04:ALARM.               =1          05:NEG_END_LIMIT.       =1         06:POS_END_LIMIT.       =1     07:MIN_VELOCITY.        =1
    // 08:MAX_VELOCITY.        =700000     09:HOME_SIGNAL.         =4         10:HOME_LEVEL.          =1     11:HOME_DIR.            =0
    // 12:ZPHASE_LEVEL.        =1          13:ZPHASE_USE.          =0         14:STOP_SIGNAL_MODE.    =0     15:STOP_SIGNAL_LEVEL.   =1
    // 16:HOME_FIRST_VELOCITY. =100        17:HOME_SECOND_VELOCITY.=100       18:HOME_THIRD_VELOCITY. =20    19:HOME_LAST_VELOCITY.  =1
    // 20:HOME_FIRST_ACCEL.    =400        21:HOME_SECOND_ACCEL.   =400       22:HOME_END_CLEAR_TIME. =1000  23:HOME_END_OFFSET.     =0
    // 24:NEG_SOFT_LIMIT.      =-134217728 25:POS_SOFT_LIMIT.      =134217727 26:MOVE_PULSE.          =1     27:MOVE_UNIT.           =1
    // 28:INIT_POSITION.       =1000       29:INIT_VELOCITY.       =200       30:INIT_ACCEL.          =400   31:INIT_DECEL.          =400
    // 32:INIT_ABSRELMODE.     =0          33:INIT_PROFILEMODE.    =4         34:SVON_LEVEL.          =1     35:ALARM_RESET_LEVEL.   =1
    // 36:ENCODER_TYPE.        =1          37:SOFT_LIMIT_SEL.      =0         38:SOFT_LIMIT_STOP_MODE.=0     39:SOFT_LIMIT_ENABLE.   =0

    // 00=[AXIS_NO             ]: 축 (0축 부터 시작함)
    // 01=[PULSE_OUT_METHOD    ]: Pulse out method TwocwccwHigh = 6
    // 02=[ENC_INPUT_METHOD    ]: disable = 0, 1체배 = 1, 2체배 = 2, 4체배 = 3, 결선 관련방향 교체시(-).1체배 = 11  2체배 = 12  4체배 = 13
    // 03=[INPOSITION          ], 04=[ALARM     ], 05,06 =[END_LIMIT   ]  : 0 = B접점, 1= A접점, 2 = 사용안함, 3 = 기존상태 유지
    // 07=[MIN_VELOCITY        ]: 시작 속도(START VELOCITY)
    // 08=[MAX_VELOCITY        ]: 드라이버가 지령을 받아들일수 있는 지령 속도. 보통 일반 Servo는 700k
    // Ex> screw : 20mm pitch drive: 10000 pulse 모터: 400w
    // 09=[HOME_SIGNAL         ]: 4 - Home in0 , 0 :PosEndLimit , 1 : NegEndLimit // _HOME_SIGNAL참조.
    // 10=[HOME_LEVEL          ]: 0 = B접점, 1 = A접점, 2 = 사용안함, 3 = 기존상태 유지
    // 11=[HOME_DIR            ]: 홈 방향(HOME DIRECTION) 1:+방향, 0:-방향
    // 12=[ZPHASE_LEVEL        ]: 0 = B접점, 1 = B접점, 2 = 사용안함, 3 = 기존상태 유지
    // 13=[ZPHASE_USE          ]: Z상사용여부. 0: 사용안함 , 1: +방향, 2: -방향 
    // 14=[STOP_SIGNAL_MODE    ]: ESTOP, SSTOP 사용시 모드 0:감속정지, 1:급정지 
    // 15=[STOP_SIGNAL_LEVEL   ]: ESTOP, SSTOP 사용 레벨.  0 = B접점, 1 = A접점, 2 = 사용안함, 3 = 기존상태 유지 
    // 16=[HOME_FIRST_VELOCITY ]: 1차구동속도 
    // 17=[HOME_SECOND_VELOCITY]: 검출후속도 
    // 18=[HOME_THIRD_VELOCITY ]: 마지막 속도 
    // 19=[HOME_LAST_VELOCITY  ]: index검색및 정밀하게 검색하기위한 속도. 
    // 20=[HOME_FIRST_ACCEL    ]: 1차 가속도 , 21=[HOME_SECOND_ACCEL   ] : 2차 가속도 
    // 22=[HOME_END_CLEAR_TIME ]: 원점 검색 Enc 값 Set하기 위한 대기시간,  23=[HOME_END_OFFSET] : 원점검출후 Offset만큼 이동.
    // 24=[NEG_SOFT_LIMIT      ]: - SoftWare Limit 같게 설정하면 사용안함, 25=[POS_SOFT_LIMIT ]: + SoftWare Limit 같게 설정하면 사용안함.
    // 26=[MOVE_PULSE          ]: 드라이버의 1회전당 펄스량              , 27=[MOVE_UNIT  ]: 드라이버 1회전당 이동량 즉:스크류 Pitch
    // 28=[INIT_POSITION       ]: 에이젼트 사용시 초기위치  , 사용자가 임의로 사용가능
    // 29=[INIT_VELOCITY       ]: 에이젼트 사용시 초기속도  , 사용자가 임의로 사용가능
    // 30=[INIT_ACCEL          ]: 에이젼트 사용시 초기가속도, 사용자가 임의로 사용가능
    // 31=[INIT_DECEL          ]: 에이젼트 사용시 초기감속도, 사용자가 임의로 사용가능
    // 32=[INIT_ABSRELMODE     ]: 절대(0)/상대(1) 위치 설정
    // 33=[INIT_PROFILEMODE    ]: 프로파일모드(0 - 4) 까지 설정
    //                            '0': 대칭 Trapezode, '1': 비대칭 Trapezode, '2': 대칭 Quasi-S Curve, '3':대칭 S Curve, '4':비대칭 S Curve
    // 34=[SVON_LEVEL          ]: 0 = B접점, 1 = A접점
    // 35=[ALARM_RESET_LEVEL   ]: 0 = B접점, 1 = A접점
    // 36=[ENCODER_TYPE        ]: 0 = TYPE_INCREMENTAL, 1 = TYPE_ABSOLUTE
    // 37=[SOFT_LIMIT_SEL      ]: 0 = COMMAND, 1 = ACTUAL
    // 38=[SOFT_LIMIT_STOP_MODE]: 0 = EMERGENCY_STOP, 1 = SLOWDOWN_STOP
    // 39=[SOFT_LIMIT_ENABLE   ]: 0 = DISABLE, 1 = ENABLE

    // AxmMotSaveParaAll로 저장 되어진 .mot파일을 불러온다. 해당 파일은 사용자가 Edit 하여 사용 가능하다.
    [DllImport("AXL.dll")] public static extern uint AxmMotLoadParaAll(string szFilePath);           // string
    [DllImport("AXL.dll")] public static extern uint AxmMotLoadParaAll(char[] szFilePath);           // char[]
    // 모든축에 대한 모든 파라메타를 축별로 저장한다. .mot파일로 저장한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSaveParaAll(string szFilePath);

    // 파라메타 28 - 31번까지 사용자가 프로그램내에서  이 함수를 이용해 설정 한다
    [DllImport("AXL.dll")] public static extern uint AxmMotSetParaLoad(int nAxisNo, double InitPos, double InitVel, double InitAccel, double InitDecel);
    // 파라메타 28 - 31번까지 사용자가 프로그램내에서  이 함수를 이용해 확인 한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetParaLoad(int nAxisNo, ref double InitPos, ref double InitVel, ref double InitAccel, ref double InitDecel);

    // 지정 축의 펄스 출력 방식을 설정한다.
    // uMethod  0 :OneHighLowHigh, 1 :OneHighHighLow, 2 :OneLowLowHigh, 3 :OneLowHighLow, 4 :TwoCcwCwHigh
    //          5 :TwoCcwCwLow, 6 :TwoCwCcwHigh, 7 :TwoCwCcwLow, 8 :TwoPhase, 9 :TwoPhaseReverse
    //        OneHighLowHigh                = 0x0,                    // 1펄스 방식, PULSE(Active High), 정방향(DIR=Low)  / 역방향(DIR=High)
    //        OneHighHighLow                = 0x1,                    // 1펄스 방식, PULSE(Active High), 정방향(DIR=High) / 역방향(DIR=Low)
    //        OneLowLowHigh                 = 0x2,                    // 1펄스 방식, PULSE(Active Low),  정방향(DIR=Low)  / 역방향(DIR=High)
    //        OneLowHighLow                 = 0x3,                    // 1펄스 방식, PULSE(Active Low),  정방향(DIR=High) / 역방향(DIR=Low)
    //        TwoCcwCwHigh                  = 0x4,                    // 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active High
    //        TwoCcwCwLow                   = 0x5,                    // 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active Low
    //        TwoCwCcwHigh                  = 0x6,                    // 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active High
    //        TwoCwCcwLow                   = 0x7,                    // 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active Low
    //        TwoPhase                      = 0x8,                    // 2상(90' 위상차),  PULSE lead DIR(CW: 정방향), PULSE lag DIR(CCW:역방향)
    //        TwoPhaseReverse               = 0x9                     // 2상(90' 위상차),  PULSE lead DIR(CCW: 정방향), PULSE lag DIR(CW:역방향)
    [DllImport("AXL.dll")] public static extern uint AxmMotSetPulseOutMethod(int nAxisNo, uint uMethod);
    // 지정 축의 펄스 출력 방식 설정을 반환한다,
    [DllImport("AXL.dll")] public static extern uint AxmMotGetPulseOutMethod(int nAxisNo, ref uint upMethod);

    // 지정 축의 외부(Actual) 카운트의 증가 방향 설정을 포함하여 지정 축의 Encoder 입력 방식을 설정한다.
    // uMethod : 0 - 7 설정.
    //        ObverseUpDownMode             = 0x0,                     // 정방향 Up/Down
    //        ObverseSqr1Mode               = 0x1,                     // 정방향 1체배
    //        ObverseSqr2Mode               = 0x2,                     // 정방향 2체배
    //        ObverseSqr4Mode               = 0x3,                     // 정방향 4체배
    //        ReverseUpDownMode             = 0x4,                     // 역방향 Up/Down
    //        ReverseSqr1Mode               = 0x5,                     // 역방향 1체배
    //        ReverseSqr2Mode               = 0x6,                     // 역방향 2체배
    //        ReverseSqr4Mode               = 0x7                      // 역방향 4체배
    [DllImport("AXL.dll")] public static extern uint AxmMotSetEncInputMethod(int nAxisNo, uint uMethod);
    // 지정 축의 외부(Actual) 카운트의 증가 방향 설정을 포함하여 지정 축의 Encoder 입력 방식을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetEncInputMethod(int nAxisNo, ref uint upMethod);

    // 설정 속도 단위가 RPM(Revolution Per Minute)으로 맞추고 싶다면.
    // ex>        rpm 계산:
    // 4500 rpm ?
    // unit/ pulse = 1 : 1이면      pulse/ sec 초당 펄스수가 되는데
    // 4500 rpm에 맞추고 싶다면     4500 / 60 초 : 75회전/ 1초
    // 모터가 1회전에 몇 펄스인지 알아야 된다. 이것은 Encoder에 Z상을 검색해보면 알수있다.
    // 1회전:1800 펄스라면 75 x 1800 = 135000 펄스가 필요하게 된다.
    // AxmMotSetMoveUnitPerPulse에 Unit = 1, Pulse = 1800 넣어 동작시킨다.
    // 주의할점 : rpm으로 제어하게 된다면 속도와 가속도 도 rpm단위로 바뀌게 된다.

    // 지정 축의 펄스 당 움직이는 거리를 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetMoveUnitPerPulse(int nAxisNo, double dUnit, int nPulse);
    // 지정 축의 펄스 당 움직이는 거리를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetMoveUnitPerPulse(int nAxisNo, ref double dpUnit, ref int npPulse);

    // 지정 축에 감속 시작 포인트 검출 방식을 설정한다.
    // uMethod : 0 -1 설정
    // AutoDetect = 0x0 : 자동 가감속.
    // RestPulse  = 0x1 : 수동 가감속.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetDecelMode(int nAxisNo, uint uMethod);
    // 지정 축의 감속 시작 포인트 검출 방식을 반환한다        
    [DllImport("AXL.dll")] public static extern uint AxmMotGetDecelMode(int nAxisNo, ref uint upMethod);

    // 지정 축에 수동 감속 모드에서 잔량 펄스를 설정한다.
    // 사용방법: 만약 AxmMotSetRemainPulse를 500 펄스를 설정
    //           AxmMoveStartPos를 위치 10000을 보냈을경우에 9500펄스부터 
    //           남은 펄스 500은  AxmMotSetMinVel로 설정한 속도로 유지하면서 감속 된다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetRemainPulse(int nAxisNo, uint uData);
    // 지정 축의 수동 감속 모드에서 잔량 펄스를 반환한다.        
    [DllImport("AXL.dll")] public static extern uint AxmMotGetRemainPulse(int nAxisNo, ref uint upData);

    // 지정 축에 등속도 구동 함수에서의 최고 속도를 설정한다.
    // 주의사항 : 입력 최대 속도 값이 PPS가 아니라 UNIT 이다.
    // ex) 최대 출력 주파수(PCI-N804/404 : 10 MPPS)
    // ex) 최대 출력 Unit/Sec(PCI-N804/404 : 10MPPS * Unit/Pulse)
    [DllImport("AXL.dll")] public static extern uint AxmMotSetMaxVel(int nAxisNo, double dVel);
    // 지정 축의 등속도 구동 함수에서의 최고 속도를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetMaxVel(int nAxisNo, ref double dpVel);

    // 지정 축의 이동 거리 계산 모드를 설정한다.
    // uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
    //               POS_REL_MODE '1' - 상대 좌표계
    [DllImport("AXL.dll")] public static extern uint AxmMotSetAbsRelMode(int nAxisNo, uint uAbsRelMode);
    // 지정 축의 설정된 이동 거리 계산 모드를 반환한다
    [DllImport("AXL.dll")] public static extern uint AxmMotGetAbsRelMode(int nAxisNo, ref uint upAbsRelMode);

    // 지정 축의 구동 속도 프로파일 모드를 설정한다.
    // ProfileMode : SYM_TRAPEZOIDE_MODE    '0' - 대칭 Trapezode
    //               ASYM_TRAPEZOIDE_MODE   '1' - 비대칭 Trapezode
    //               QUASI_S_CURVE_MODE     '2' - 대칭 Quasi-S Curve
    //               SYM_S_CURVE_MODE       '3' - 대칭 S Curve
    //               ASYM_S_CURVE_MODE      '4' - 비대칭 S Curve
    //               SYM_TRAP_M3_SW_MODE    '5' - 대칭 Trapezode : MLIII 내부 S/W Profile
    //               ASYM_TRAP_M3_SW_MODE   '6' - 비대칭 Trapezode : MLIII 내부 S/W Profile
    //               SYM_S_M3_SW_MODE       '7' - 대칭 S Curve : MLIII 내부 S/W Profile
    //               ASYM_S_M3_SW_MODE      '8' - asymmetric S Curve : MLIII 내부 S/W Profile
    [DllImport("AXL.dll")] public static extern uint AxmMotSetProfileMode(int nAxisNo, uint uProfileMode);
    // 지정 축의 설정한 구동 속도 프로파일 모드를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetProfileMode(int nAxisNo, ref uint upProfileMode);

    // 지정 축의 가속도 단위를 설정한다.
    // AccelUnit : UNIT_SEC2   '0' - 가감속 단위를 unit/sec2 사용
    //             SEC         '1' - 가감속 단위를 sec 사용
    [DllImport("AXL.dll")] public static extern uint AxmMotSetAccelUnit(int nAxisNo, uint uAccelUnit);
    // 지정 축의 설정된 가속도단위를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetAccelUnit(int nAxisNo, ref uint upAccelUnit);

    // 주의사항: 최소속도를 UNIT/PULSE 보다 작게할 경우 최소단위가 UNIT/PULSE로 맞추어지기때문에 최소 속도가 UNIT/PULSE 가 된다.
    // 지정 축에 초기 속도를 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetMinVel(int nAxisNo, double dMinVelocity);
    // 지정 축의 초기 속도를 반환한다
    [DllImport("AXL.dll")] public static extern uint AxmMotGetMinVel(int nAxisNo, ref double dpMinVelocity);

    // 지정 축의 가속 저크값을 설정한다.[%].
    [DllImport("AXL.dll")] public static extern uint AxmMotSetAccelJerk(int nAxisNo, double dAccelJerk);
    // 지정 축의 설정된 가속 저크값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetAccelJerk(int nAxisNo, ref double dpAccelJerk);

    // 지정 축의 감속 저크값을 설정한다.[%].
    [DllImport("AXL.dll")] public static extern uint AxmMotSetDecelJerk(int nAxisNo, double dDecelJerk);
    // 지정 축의 설정된 감속 저크값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetDecelJerk(int nAxisNo, ref double dpDecelJerk);

    // 지정 축의 속도 Profile결정시 우선순위(속도 Or 가속도)를 설정한다.
    // Priority : PRIORITY_VELOCITY   '0' - 속도 Profile결정시 지정한 속도값에 가깝도록 계산함(일반장비 및 Spinner에 사용).
    //            PRIORITY_ACCELTIME  '1' - 속도 Profile결정시 지정한 가감속시간에 가깝도록 계산함(고속 장비에 사용).
    [DllImport("AXL.dll")] public static extern uint AxmMotSetProfilePriority(int nAxisNo, uint uPriority);
    // 지정 축의 속도 Profile결정시 우선순위(속도 Or 가속도)를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetProfilePriority(int nAxisNo, ref uint upPriority);

    //=========== 입출력 신호 관련 설정함수 =============================================================================
    // 지정 축의 Z 상 Level을 설정한다.
    // uLevel : LOW(0), HIGH(1)
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetZphaseLevel(int nAxisNo, uint uLevel);
    // 지정 축의 Z 상 Level을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetZphaseLevel(int nAxisNo, ref uint upLevel);

    // 지정 축의 Servo-On신호의 출력 레벨을 설정한다.
    // uLevel : LOW(0), HIGH(1)
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetServoOnLevel(int nAxisNo, uint uLevel);
    // 지정 축의 Servo-On신호의 출력 레벨 설정을 반환한다.        
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetServoOnLevel(int nAxisNo, ref uint upLevel);

    // 지정 축의 Servo-Alarm Reset 신호의 출력 레벨을 설정한다.
    // uLevel : LOW(0), HIGH(1)
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetServoAlarmResetLevel(int nAxisNo, uint uLevel);
    // 지정 축의 Servo-Alarm Reset 신호의 출력 레벨을 설정을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetServoAlarmResetLevel(int nAxisNo, ref uint upLevel);

    // 지정 축의 Inpositon 신호 사용 여부 및 신호 입력 레벨을 설정한다
    // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)        
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetInpos(int nAxisNo, uint uUse);
    // 지정 축의 Inpositon 신호 사용 여부 및 신호 입력 레벨을 반환한다.        
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetInpos(int nAxisNo, ref uint upUse);
    // 지정 축의 Inpositon 신호 입력 상태를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadInpos(int nAxisNo, ref uint upStatus);

    // 지정 축의 알람 신호 입력 시 비상 정지의 사용 여부 및 신호 입력 레벨을 설정한다.
    // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetServoAlarm(int nAxisNo, uint uUse);
    // 지정 축의 알람 신호 입력 시 비상 정지의 사용 여부 및 신호 입력 레벨을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetServoAlarm(int nAxisNo, ref uint upUse);
    // 지정 축의 알람 신호의 입력 레벨을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadServoAlarm(int nAxisNo, ref uint upStatus);

    // 지정 축의 end limit sensor의 사용 유무 및 신호의 입력 레벨을 설정한다. 
    // end limit sensor 신호 입력 시 감속정지 또는 급정지에 대한 설정도 가능하다.
    // uStopMode: EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
    // uPositiveLevel, uNegativeLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetLimit(int nAxisNo, uint uStopMode, uint uPositiveLevel, uint uNegativeLevel);
    // 지정 축의 end limit sensor의 사용 유무 및 신호의 입력 레벨, 신호 입력 시 정지모드를 반환한다
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetLimit(int nAxisNo, ref uint upStopMode, ref uint upPositiveLevel, ref uint upNegativeLevel);
    // 지정축의 end limit sensor의 입력 상태를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadLimit(int nAxisNo, ref uint upPositiveStatus, ref uint upNegativeStatus);

    // 지정 축의 Software limit의 사용 유무, 사용할 카운트, 그리고 정지 방법을 설정한다
    // uUse       : DISABLE(0), ENABLE(1)
    // uStopMode  : EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
    // uSelection : COMMAND(0), ACTUAL(1)
    // 주의사항: 원점검색시 위 함수를 이용하여 소프트웨어 리밋을 미리 설정해서 구동시 원점검색시 원점검색을 도중에 멈추어졌을경우에도  Enable된다. 
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetSoftLimit(int nAxisNo, uint uUse, uint uStopMode, uint uSelection, double dPositivePos, double dNegativePos);
    // 지정 축의 Software limit의 사용 유무, 사용할 카운트, 그리고 정지 방법을 반환한다
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetSoftLimit(int nAxisNo, ref uint upUse, ref uint upStopMode, ref uint upSelection, ref double dpPositivePos, ref double dpNegativePos);
    // 지정 축의 Software limit의 현재 상태를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadSoftLimit(int nAxisNo, ref uint upPositiveStatus, ref uint upNegativeStatus);

    // 비상 정지 신호의 정지 방법 (급정지/감속정지) 또는 사용 유무를 설정한다.
    // uStopMode  : EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
    // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetStop(int nAxisNo, uint uStopMode, uint uLevel);
    // 비상 정지 신호의 정지 방법 (급정지/감속정지) 또는 사용 유무를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetStop(int nAxisNo, ref uint upStopMode, ref uint upLevel);
    // 비상 정지 신호의 입력 상태를 반환한다.        
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadStop(int nAxisNo, ref uint upStatus);

    // 지정 축의 Servo-On 신호를 출력한다.
    // uOnOff : FALSE(0), TRUE(1) ( 범용 0출력에 해당됨)
    [DllImport("AXL.dll")] public static extern uint AxmSignalServoOn(int nAxisNo, uint uOnOff);
    // 지정 축의 Servo-On 신호의 출력 상태를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalIsServoOn(int nAxisNo, ref uint upOnOff);

    // 지정 축의 Servo-Alarm Reset 신호를 출력한다.
    // uOnOff : FALSE(0), TRUE(1) ( 범용 1출력에 해당됨)
    [DllImport("AXL.dll")] public static extern uint AxmSignalServoAlarmReset(int nAxisNo, uint uOnOff);

    // 범용 출력값을 설정한다.
    // uValue : Hex Value 0x00
    [DllImport("AXL.dll")] public static extern uint AxmSignalWriteOutput(int nAxisNo, uint uValue);
    // 범용 출력값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadOutput(int nAxisNo, ref uint upValue);
    
    // ML3 전용 합수
    // 지정축의 Brake sensor의 상태를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadBrakeOn(int nAxisNo, ref uint upOnOff);

    // lBitNo : Bit Number(0 - 4)
    // uOnOff : FALSE(0), TRUE(1)
    // 범용 출력값을 비트별로 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalWriteOutputBit(int nAxisNo, int nBitNo, uint uOn);
    // 범용 출력값을 비트별로 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadOutputBit(int nAxisNo, int nBitNo, ref uint upOn);

    // 범용 입력값을 Hex값으로 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadInput(int nAxisNo, ref uint upValue);

    // lBitNo : Bit Number(0 - 4)
    // 범용 입력값을 비트별로 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadInputBit(int nAxisNo, int nBitNo, ref uint upOn);

    // 입력신호들의 디지털 필터계수를 설정한다.
    // uSignal: END_LIMIT(0), INP_ALARM(1), UIN_00_01(2), UIN_02_04(3)
    // dBandwidthUsec: 0.2uSec~26666usec
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetFilterBandwidth(int nAxisNo, uint uSignal, double dBandwidthUsec);
    
    // 범용 입력의 비트 수를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetInputBitCount(int nAxisNo, ref uint upInputCount);
    
    // 범용 출력의 비트 수를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetOutputBitCount(int nAxisNo, ref uint upOutputCount);

    //========== 모션 구동중 및 구동후에 상태 확인하는 함수======================================================
    // (구동상태)모션 구동 중인가를 확인
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadInMotion(int nAxisNo, ref uint upStatus);

    //  (펄스 카운트 값)구동시작 이후 출력된 펄스 카운터 값을 확인
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadDrivePulseCount(int nAxisNo, ref int npPulse);

    // DriveStatus 레지스터를 확인
    // 주의사항 : 각 제품별로 하드웨어적인 신호가 다르기때문에 매뉴얼 및 AXHS.xxx 파일을 참고해야한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadMotion(int nAxisNo, ref uint upStatus);

    // EndStatus 레지스터를 확인
    // 주의사항 : 각 제품별로 하드웨어적인 신호가 다르기때문에 매뉴얼 및 AXHS.xxx 파일을 참고해야한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadStop(int nAxisNo, ref uint upStatus);

    // Mechanical 레지스터를 확인
    // 주의사항 : 각 제품별로 하드웨어적인 신호가 다르기때문에 매뉴얼 및 AXHS.xxx 파일을 참고해야한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadMechanical(int nAxisNo, ref uint upStatus);

    // 현재 속도를 읽어 온다
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadVel(int nAxisNo, ref double dpVelocity);

    // Command Pos과 Actual Pos의 차를 확인
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadPosError(int nAxisNo, ref double dpError);

    // 최후 드라이브의 이동 거리를 확인
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadDriveDistance(int nAxisNo, ref double dpUnit);

    // 지정 축의 위치 정보 사용 방법에 대하여 설정한다.
    // uPosType  : Actual position 과 Command position 의 표시 방법
    //    POSITION_LIMIT '0' - 기본 동작, 전체 범위 내에서 동작
    //    POSITION_BOUND '1' - 위치 범위 주기형, dNegativePos ~ dPositivePos 범위로 동작
    // 주의사항(PCI-Nx04해당)
    // - BOUNT설정시 카운트 값이 Max값을 초과 할 때 Min값이되며 반대로 Min값을 초과 할 때 Max값이 된다.
    // - 다시말해 현재 위치값이 설정한 값 밖에서 카운트 될 때는 위의 Min, Max값이 적용되지 않는다.
    // dPositivePos 설정 범위: 0 ~ 양수
    // dNegativePos 설정 범위: 음수 ~ 0
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetPosType(int nAxisNo, uint uPosType, double dPositivePos, double dNegativePos);
    // 지정 축의 위치 정보 사용 방법에 대하여 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetPosType(int nAxisNo, ref uint upPosType, ref double dpPositivePos, ref double dpNegativePos);
    // 지정 축의 절대치 엔코더 원점 Offset 위치를 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetAbsOrgOffset(int nAxisNo, double dOrgOffsetPos);
    // 지정 축의 절대치 엔코더 원점 Offset 위치를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetAbsOrgOffset(int nAxisNo, ref double dpOrgOffsetPos);

    // 지정 축의 Actual 위치를 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetActPos(int nAxisNo, double dPos);
    // 지정 축의 Actual 위치를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetActPos(int nAxisNo, ref double dpPos);
    // 서보측에서 올라오는 지정 축의 Actual 위치를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetAmpActPos(int lAxisNo, ref double dpPos);

    // 지정 축의 Command 위치를 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetCmdPos(int nAxisNo, double dPos);
    // 지정 축의 Command 위치를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetCmdPos(int nAxisNo, ref double dpPos);
    // 지정 축의 Command 위치와 Actual 위치를 dPos 값으로 일치 시킨다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetPosMatch(int nAxisNo, double dPos);

    // 지정 축의 모션 상태(Cmd, Act, Driver Status, Mechanical Signal, Universal Signal)를 한번에 확인 할 수 있다.
    // MOTION_INFO 구조체의 uMask 설정으로 모션 상태 정보를 지정한다.
    // uMask : 모션 상태 표시(6bit) - ex) uMask = 0x1F 설정 시 모든 상태를 표시함.
    // 사용자가 설정한 Level(In/Out)은 반영되지 않음.
    //    [0]        |    Command Position Read
    //    [1]        |    Actual Position Read
    //    [2]        |    Mechanical Signal Read
    //    [3]        |    Driver Status Read
    //    [4]        |    Universal Signal Input Read
    //               |    Universal Signal Output Read
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadMotionInfo(int nAxisNo, ref MOTION_INFO MI);

    // Network 제품 전용함수.
    // 지정한 축의 서보팩에 AlarmCode를 읽어오도록 명령하는 함수.
    [DllImport("AXL.dll")] public static extern uint AxmStatusRequestServoAlarm(int nAxisNo);
    // 지정한 축의 서보팩 AlarmCode를 읽어오는 함수.
    // upAlarmCode      : 해당 서보팩의 Alarm Code참조
    // MR_J4_xxB  : 상위 16Bit : 알람코드 2 digit의 10진수 값, 하위 16Bit : 알람 상세 코드 1 digit 10진수 값
    // uReturnMode      : 함수의 반환 동작조건을 설정[SIIIH(MR-J4-xxB)는 사용하지 않음]
    // [0-Immediate]    : 함수 실행 후 바로 반환
    // [1-Blocking]     : 서보팩으로 부터 알람 코드를 읽을 대 까지 반환하지않음
    // [2-Non Blocking] : 서보팩으로 부터 알람 코드를 읽을 대 까지 반환하지않으나 프로그램 Blocking되지않음
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadServoAlarm(int nAxisNo, uint uReturnMode, ref uint upAlarmCode);
    // 지정한 에러코드에 해당하는 Alarm String을 받아오는 함수
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetServoAlarmString(int nAxisNo, uint uAlarmCode, int nAlarmStringSize, byte[] szAlarmString);
    // 지정한 축에서 현재 표시 중인 Alarm string을 받아오는 함수
    //  lAlarmStringSize : Alarm string을 문자열의 크기. 표시될 문자열보다 작은 공간일 경우 초과하는 부분은 자동으로 생략됨
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadServoAlarmString(int lAxisNo, int lAlarmStringSize, char[] szAlarmString);

    // 지정한 축의 서보팩에 Alarm History를 읽어오도록 명령하는 함수
    [DllImport("AXL.dll")] public static extern uint AxmStatusRequestServoAlarmHistory(int nAxisNo);
    // 지정한 축의 서보팩 Alarm History를 읽어오는 함수.
    // lpCount          : 읽은 Alarm History 개수 
    // upAlarmCode      : Alarm History를 반환할 배열
    // uReturnMode      : 함수의 반환 동작조건을 설정
    // [0-Immediate]    : 함수 실행 후 바로 반환
    // [1-Blocking]     : 서보팩으로 부터 알람 코드를 읽을 때 까지 제어권 반환하지않음
    // [2-Non Blocking] : 서보팩으로 부터 알람 코드를 읽을 때 까지 제어권 반환하지않으나 프로그램 Blocking되지않음(윈도우 메세지 내부에서 처리함)
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadServoAlarmHistory(int nAxisNo, uint uReturnMode, ref int npCount, ref uint upAlarmCode);
    // 지정한 축의 서보팩 Alarm History를 Clear한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusClearServoAlarmHistory(int nAxisNo);

    //======== 홈관련 함수===============================================================================================
    // 지정 축의 Home 센서 Level 을 설정한다.
    // uLevel : LOW(0), HIGH(1)
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetSignalLevel(int nAxisNo, uint uLevel);
    // 지정 축의 Home 센서 Level 을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetSignalLevel(int nAxisNo, ref uint upLevel);
    // 현재 홈 신호 입력상태를 확인한다. 홈신호는 사용자가 임의로 AxmHomeSetMethod 함수를 이용하여 설정할수있다.
    // 일반적으로 홈신호는 범용 입력 0를 사용하고있지만 AxmHomeSetMethod 이용해서 바꾸면 + , - Limit를 사용할수도있다.
    // upStatus : OFF(0), ON(1)
    [DllImport("AXL.dll")] public static extern uint AxmHomeReadSignal(int nAxisNo, ref uint upStatus);

    // 해당 축의 원점검색을 수행하기 위해서는 반드시 원점 검색관련 파라메타들이 설정되어 있어야 됩니다. 
    // 만약 MotionPara설정 파일을 이용해 초기화가 정상적으로 수행됐다면 별도의 설정은 필요하지 않다. 
    // 원점검색 방법 설정에는 검색 진행방향, 원점으로 사용할 신호, 원점센서 Active Level, 엔코더 Z상 검출 여부 등을 설정 한다.
    // 주의사항 : 레벨을 잘못 설정시 -방향으로 설정해도  +방향으로 동작할수 있으며, 홈을 찾는데 있어 문제가 될수있다.
    // (자세한 내용은 AxmMotSaveParaAll 설명 부분 참조)
    // 홈레벨은 AxmSignalSetHomeLevel 사용한다.
    // HClrTim : HomeClear Time : 원점 검색 Encoder 값 Set하기 위한 대기시간 
    // HmDir(홈 방향): DIR_CCW (0) -방향 , DIR_CW(1) +방향
    // HOffset - 원점검출후 이동거리.
    // uZphas: 1차 원점검색 완료 후 엔코더 Z상 검출 유무 설정  0: 사용안함 , 1: Hmdir과 반대 방향, 2: Hmdir과 같은 방향
    // HmSig : PosEndLimit(0) -> +Limit
    //         NegEndLimit(1) -> -Limit
    //         HomeSensor (4) -> 원점센서(범용 입력 0)
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetMethod(int nAxisNo, int nHmDir, uint uHomeSignal, uint uZphas, double dHomeClrTime, double dHomeOffset);
    // 설정되어있는 홈 관련 파라메타들을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetMethod(int nAxisNo, ref int nHmDir, ref uint uHomeSignal, ref uint uZphas, ref double dHomeClrTime, ref double dHomeOffset);

    // 원점검색 방법의 미세조정을 하는 함수(기본적으로 설정하지 않아도됨).
    // dHomeDogDistance[500 pulse]: 첫번째 Step에서 HomeDog가 센서를 지나쳤는지 확인하기위한 Dog길이를 입력.(단위는 AxmMotSetMoveUnitPerPulse함수로 설정한 단위)
    // lLevelScanTime[100msec]: 2번째 Step(원점센서를 빠져나가는 동작)에서 Level상태를 확인할 Scan시간을 설정(단위는 msec[1~1000]).
    // uFineSearchUse[USE]: 기본 원점검색시 5 Step를 사용하는데 3 Step만 사용하도록 변경할때 0으로 설정.
    // uHomeClrUse[USE]: 원점검색 후 지령값과 Encoder값을 0으로 자동 설정여부를 설정.
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetFineAdjust(int nAxisNo, double dHomeDogLength, uint uLevelScanTime, uint uFineSearchUse, uint uHomeClrUse);
    // 설정되어있는 홈 관련 미세조정 파라메타들을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetFineAdjust(int nAxisNo, ref double dpHomeDogLength, ref uint upLevelScanTime, ref uint upFineSearchUse, ref uint upHomeClrUse);

    // 원점 검색시 Interlock을 설정하는 함수(기본적으로 설정하지 않아도됨).
    // uInterlockMode : Interlock 설정 모드
    //   (0) HOME_INTERLOCK_UNUSED          : Home Interlock 사용하지 않음
    //   [1] HOME_INTERLOCK_SENSOR_CHECK    : 원점검색 진행 방향에 설치된 리미트 센서가 감지 되었을 때 원점 센서가 같이 감지되지 않은 경우 INTERLOCK 에러 발생
    //   [2] HOME_INTERLOCK_DISTANCE        : 원점검색 진행 방향에 설치된 리미트 센서가 감지 된 후 원점 센서까지의 거리가 지정한 거리보다 클 경우 INTERLOCK 에러 발생
    // dInterlockData : Interlock Mode에 대한 설정값
    //   (0) HOME_INTERLOCK_UNUSED          : 사용안함
    //   [1] HOME_INTERLOCK_SENSOR_CHECK    : 사용안함
    //   [2] HOME_INTERLOCK_DISTANCE        : 원점검색 진행 방향에 설치된 리미트와 원점 센서까지의 거리(실제 거리보다 약간 크게 설정 함)
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetInterlock(int lAxisNo, uint uInterlockMode, double dInterlockData);
    // 원점 검색시 사용되는 Interlock 설정값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetInterlock(int lAxisNo, ref uint upInterlockMode, ref double dpInterlockData);

    // 원점을 빠르고 정밀하게 검색하기 위해 여러 단계의 스탭으로 검출한다. 이때 각 스탭에 사용 될 속도를 설정한다. 
    // 이 속도들의 설정값에 따라 원점검색 시간과, 원점검색 정밀도가 결정된다. 
    // 각 스탭별 속도들을 적절히 바꿔가면서 각 축의 원점검색 속도를 설정하면 된다. 
    // (자세한 내용은 AxmMotSaveParaAll 설명 부분 참조)
    // 원점검색시 사용될 속도를 설정하는 함수
    // [dVelFirst]- 1차구동속도   [dVelSecond]-검출후속도   [dVelThird]- 마지막 속도  [dvelLast]- index검색및 정밀하게 검색하기위해. 
    // [dAccFirst]- 1차구동가속도 [dAccSecond]-검출후가속도 
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetVel(int nAxisNo, double dVelFirst, double dVelSecond, double dVelThird, double dvelLast, double dAccFirst, double dAccSecond);
    // 설정되어있는 원점검색시 사용될 속도를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetVel(int nAxisNo, ref double dVelFirst, ref double dVelSecond, ref double dVelThird, ref double dvelLast, ref double dAccFirst, ref double dAccSecond);

    // 원점검색을 시작한다.
    // 원점검색 시작함수를 실행하면 라이브러리 내부에서 해당축의 원점검색을 수행 할 쓰레드가 자동 생성되어 원점검색을 순차적으로 수행한 후 자동 종료된다.
    // 주의사항 : 진행방향과 반대방향의 리미트 센서가 들어와도 진행방향의 센서가 ACTIVE되지않으면 동작한다.
    //            원점 검색이 시작되어 진행방향이 리밋트 센서가 들어오면 리밋트 센서가 감지되었다고 생각하고 다음단계로 진행된다.
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetStart(int nAxisNo);
    // 원점검색 결과를 사용자가 임의로 설정한다.
    // 원점검색 함수를 이용해 성공적으로 원점검색이 수행되고나면 검색 결과가 HOME_SUCCESS로 설정됩니다.
    // 이 함수는 사용자가 원점검색을 수행하지않고 결과를 임의로 설정할 수 있다. 
    // uHomeResult 설정
    // HOME_SUCCESS                             = 0x01        // 홈 완료
    // HOME_SEARCHING                           = 0x02        // 홈검색중
    // HOME_ERR_GNT_RANGE                       = 0x10        // 홈 검색 범위를 벗어났을경우
    // HOME_ERR_USER_BREAK                      = 0x11        // 속도 유저가 임의로 정지명령을 내렸을경우
    // HOME_ERR_VELOCITY                        = 0x12        // 속도 설정 잘못했을경우
    // HOME_ERR_AMP_FAULT                       = 0x13        // 서보팩 알람 발생 에러
    // HOME_ERR_NEG_LIMIT                       = 0x14        // (-)방향 구동중 (+)리미트 센서 감지 에러
    // HOME_ERR_POS_LIMIT                       = 0x15        // (+)방향 구동중 (-)리미트 센서 감지 에러
    // HOME_ERR_NOT_DETECT                      = 0x16        // 지정한 신호 검출하지 못 할 경우 에러
    // HOME_ERR_UNKNOWN                         = 0xFF        
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetResult(int nAxisNo, uint uHomeResult);
    // 원점검색 결과를 반환한다.
    // 원점검색 함수의 검색 결과를 확인한다. 원점검색이 시작되면 HOME_SEARCHING으로 설정되며 원점검색에 실패하면 실패원인이 설정된다. 실패 원인을 제거한 후 다시 원점검색을 진행하면 된다.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetResult(int nAxisNo, ref uint upHomeResult);

    // 원점검색 진행률을 반환한다.
    // 원점검색 시작되면 진행율을 확인할 수 있다. 원점검색이 완료되면 성공여부와 관계없이 100을 반환하게 된다. 원점검색 성공여부는 GetHome Result함수를 이용해 확인할 수 있다.
    // upHomeMainStepNumber : Main Step 진행율이다. 
    // 겐트리 FALSE일 경우upHomeMainStepNumber : 0 일때면 선택한 축만 진행사항이고 홈 진행율은 upHomeStepNumber 표시한다.
    // 겐트리 TRUE일 경우 upHomeMainStepNumber : 0 일때면 마스터 홈을 진행사항이고 마스터 홈 진행율은 upHomeStepNumber 표시한다.
    // 겐트리 TRUE일 경우 upHomeMainStepNumber : 10 일때면 슬레이브 홈을 진행사항이고 마스터 홈 진행율은 upHomeStepNumber 표시한다.
    // upHomeStepNumber     : 선택한 축에대한 진행율을 표시한다. 
    // 겐트리 FALSE일 경우  : 선택한 축만 진행율을 표시한다.
    // 겐트리 TRUE일 경우 마스터축, 슬레이브축 순서로 진행율을 표시된다.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetRate(int nAxisNo, ref uint upHomeMainStepNumber, ref uint upHomeStepNumber);

    //========= 위치 구동함수 ===========================================================================================
    // 주의사항: 위치를 설정할경우 반드시 UNIT/PULSE의 맞추어서 설정한다.
    //           위치를 UNIT/PULSE 보다 작게할 경우 최소단위가 UNIT/PULSE로 맞추어지기때문에 그위치까지 구동이 될수없다.

    // 설정 속도 단위가 RPM(Revolution Per Minute)으로 맞추고 싶다면.
    // ex>        rpm 계산:
    // 4500 rpm ?
    // unit/ pulse = 1 : 1이면      pulse/ sec 초당 펄스수가 되는데
    // 4500 rpm에 맞추고 싶다면     4500 / 60 초 : 75회전/ 1초
    // 모터가 1회전에 몇 펄스인지 알아야 된다. 이것은 Encoder에 Z상을 검색해보면 알수있다.
    // 1회전:1800 펄스라면 75 x 1800 = 135000 펄스가 필요하게 된다.
    // AxmMotSetMoveUnitPerPulse에 Unit = 1, Pulse = 1800 넣어 동작시킨다. 

    // 설정한 거리만큼 또는 위치까지 이동한다.
    // 지정 축의 절대 좌표/ 상대좌표 로 설정된 위치까지 설정된 속도와 가속율로 구동을 한다.
    // 속도 프로파일은 AxmMotSetProfileMode 함수에서 설정한다.
    // 펄스가 출력되는 시점에서 함수를 벗어난다.
    // AxmMotSetAccelUnit(lAxisNo, 1) 일경우 dAccel -> dAccelTime , dDecel -> dDecelTime 으로 바뀐다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel);

    // 설정한 거리만큼 또는 위치까지 이동한다.
    // 지정 축의 절대 좌표/상대좌표로 설정된 위치까지 설정된 속도와 가속율로 구동을 한다.
    // 속도 프로파일은 AxmMotSetProfileMode 함수에서 설정한다. 
    // 펄스 출력이 종료되는 시점에서 함수를 벗어난다
    [DllImport("AXL.dll")] public static extern uint AxmMovePos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel);

    // 설정한 속도로 구동한다.
    // 지정 축에 대하여 설정된 속도와 가속율로 지속적으로 속도 모드 구동을 한다. 
    // 펄스 출력이 시작되는 시점에서 함수를 벗어난다.
    // Vel값이 양수이면 CW, 음수이면 CCW 방향으로 구동.
    [DllImport("AXL.dll")] public static extern uint AxmMoveVel(int nAxisNo, double dVel, double dAccel, double dDecel);

    // 지정된 다축에 대하여 설정된 속도와 가속율로 지속적으로 속도 모드 구동을 한다.
    // 펄스 출력이 시작되는 시점에서 함수를 벗어난다.
    // Vel값이 양수이면 CW, 음수이면 CCW 방향으로 구동.
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartMultiVel(int nArraySize, int[] npAxesNo, double[] dVel, double[] dAccel, double[] dDecel);

    // 지정된 다축에 대하여 설정된 속도와 가속율, SyncMode에 따라 지속적으로 속도 모드 구동을 한다.
    // 펄스 출력이 시작되는 시점에서 함수를 벗어난다.
    // Vel값이 양수이면 CW, 음수이면 CCW 방향으로 구동.
    // uSyncMode    : 동기정지기능 사용안함(0), 동기정지 기능만 사용(1), 알람에 대해서도 동기 정기기능 사용(2)
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartMultiVelEx(int nArraySize, int[] npAxesNo, double[] dpVel, double[] dpAccel, double[] dpDecel, uint uSyncMode);

    // 지정된 다축에 대하여 설정된 속도와 가속율로 지속적으로 속도 모드 구동을 한다.
    // 펄스 출력이 시작되는 시점에서 함수를 벗어나며 Master축은(Distance가 가장 큰) dVel속도로 움직이며, 나머지 축들의 Distance비율로 움직인다. 
    // 속도는 해당 Chip중 축 번호가 가장 낮은 축의 속도만 읽힘
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartLineVel(int nArraySize, int[] npAxesNo, double[] dpDis, double dVel, double dAccel, double dDecel);

    // 특정 Input 신호의 Edge를 검출하여 즉정지 또는 감속정지하는 함수.
    // lDetect Signal : edge 검출할 입력 신호 선택.
    // lDetectSignal  : PosEndLimit(0), NegEndLimit(1), HomeSensor(4), EncodZPhase(5), UniInput02(6), UniInput03(7)
    // Signal Edge    : 선택한 입력 신호의 edge 방향 선택 (rising or falling edge).
    //                  SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
    // 구동방향      : Vel값이 양수이면 CW, 음수이면 CCW.
    // SignalMethod  : 급정지 EMERGENCY_STOP(0), 감속정지 SLOWDOWN_STOP(1)
    // 주의사항: SignalMethod를 EMERGENCY_STOP(0)로 사용할경우 가감속이 무시되며 지정된 속도로 가속 급정지하게된다.
    //          PCI-Nx04를 사용할 경우 lDetectSignal이 PosEndLimit , NegEndLimit(0,1) 을 찾을경우 신호의레벨 Active 상태를 검출하게된다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveSignalSearch(int nAxisNo, double dVel, double dAccel, int nDetectSignal, int nSignalEdge, int nSignalMethod);

    // 특정 Input 신호의 Edge를 검출하여 사용자가 지정함 위치 값만큼 이동하는 함수.(MLIII : Sigma-5/7 전용)
    // dVel           : 구동 속도 설정, 양수이면 CW, 음수이면 CCW.
    // dAccel         : 구동 가속도 설정
    // dDecel         : 구동 감속도 설정, 일반적으로 dAccel의 50배로 설정함.
    // lDetectSignal  : HomeSensor(4)
    // dDis           : 입력 신호의 검출 위치를 기준으로 사용자가 지정한 위치만큼 상대 구동됨.
    // 주의사항:        
    //          - 구동방향과 반대 방향으로 dDis 값 입력시 역방향으로 구동 될 수 있음.
    //          - 속도가 빠르고, dDis 값이 작은 경우 모터가 신호 감지해서 정지한 이후에 최종 위치로 가기 위해서 역방향으로 구동될 수 있음
    //          - 해당 함수를 사용하기 전에 원점 센서는 반드시 LOW 또는 HIGH로 설정되어 있어야함.
    [DllImport("AXL.dll")] public static extern uint AxmMoveSignalSearchAtDis(int nAxisNo, double dVel, double dAccel, double dDecel, int nDetectSignal, double dDis);

    // 지정 축에서 설정된 신호를 검출하고 그 위치를 저장하기 위해 이동하는 함수이다.
    // 원하는 신호를 골라 찾아 움직이는 함수 찾을 경우 그 위치를 저장시켜놓고 AxmGetCapturePos사용하여 그값을 읽는다.
    // Signal Edge   : 선택한 입력 신호의 edge 방향 선택 (rising or falling edge).
    //                 SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
    // 구동방향      : Vel값이 양수이면 CW, 음수이면 CCW.
    // SignalMethod  : 급정지 EMERGENCY_STOP(0), 감속정지 SLOWDOWN_STOP(1)
    // lDetect Signal: edge 검출할 입력 신호 선택.SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
    //                 상위 8bit에 대하여 기본 구동(0), Software 구동(1) 을 선택할 수 있다. SMP Board(PCIe-Rxx05-MLIII) 전용
    // lDetectSignal : PosEndLimit(0), NegEndLimit(1), HomeSensor(4), EncodZPhase(5), UniInput02(6), UniInput03(7)
    // lTarget       : COMMAND(0), ACTUAL(1)
    // 주의사항: SignalMethod를 EMERGENCY_STOP(0)로 사용할경우 가감속이 무시되며 지정된 속도로 가속 급정지하게된다.
    //           PCI-Nx04를 사용할 경우 lDetectSignal이 PosEndLimit , NegEndLimit(0,1) 을 찾을경우 신호의레벨 Active 상태를 검출하게된다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveSignalCapture(int nAxisNo, double dVel, double dAccel, int nDetectSignal, int nSignalEdge, int nTarget, int nSignalMethod);
    // 'AxmMoveSignalCapture' 함수에서 저장된 위치값을 확인하는 함수이다.
    // 주의사항: 함수 실행 결과가 "AXT_RT_SUCCESS"일때 저장된 위치가 유효하며, 이 함수를 한번 실행하면 저장 위치값이 초기화된다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveGetCapturePos(int nAxisNo, ref double dpCapPos);

    // "설정한 거리만큼 또는 위치까지 이동하는 함수.
    // 함수를 실행하면 해당 Motion 동작을 시작한 후 Motion 이 완료될때까지 기다리지 않고 바로 함수를 빠져나간다."
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartMultiPos(int nArraySize, int[] nAxisNo, double[] dPos, double[] dVel, double[] dAccel, double[] dDecel);

    // 다축을 설정한 거리만큼 또는 위치까지 이동한다.
    // 지정 축들의 절대 좌표로 설정된 위치까지 설정된 속도와 가속율로 구동을 한다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveMultiPos(int nArraySize, int[] nAxisNo, double[] dPos, double[] dVel, double[] dAccel, double[] dDecel);

    // 설정한 토크 및 속도 값으로 모터를 구동한다.(PCIR-1604-MLII/SIIIH, PCIe-Rxx04-SIIIH  전용 함수)
    // dTroque        : 최대 출력 토크에 대한 %값.     
    // 구동방향       : dTroque값이 양수이면 CW, 음수이면 CCW.
    // dVel           : 최대 모터 구동 속도에 대한 %값.
    // uAccFilterSel  : LINEAR_ACCDCEL(0), EXPO_ACCELDCEL(1), SCURVE_ACCELDECEL(2)
    // uGainSel       : GAIN_1ST(0), GAIN_2ND(1)
    // uSpdLoopSel    : PI_LOOP(0), P_LOOP(1)

    // PCIe-Rxx05-MLIII
    // dTorque        : 최대 출력 토크에 대한 %값 (단위: %)
    //                  dTorque 값이 양수지면 CW, 음수이면 CCW 방향으로 구동
    // dVel           : 구동 속도 (단위: pps)
    // dwAccFilterSel : 사용하지 않음
    // dwGainSel      : 사용하지 않음
    // dwSpdLoopSel   : 사용하지 않음
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartTorque(int nAxisNo, double dTorque, double dVel, uint uAccFilterSel, uint uGainSel, uint uSpdLoopSel);

    // 지정 축의 토크 구동을 정지 한다.
    // AxmMoveStartTorque후 반드시 AxmMoveTorqueStop를 실행하여야 한다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveTorqueStop(int nAxisNo, uint uMethod);

    // 설정한 거리만큼 또는 위치까지 이동한다.
    // 지정 축의 절대 좌표/상대좌표로 설정된 위치까지 설정된 속도/가속율로 구동을 한다.
    // 속도 프로파일은 비대칭 사다리꼴로 고정됩니다.
    // 가감속도 설정 단위는 기울기로 고정됩니다.
    // dAccel != 0.0 이고 dDecel == 0.0 일 경우 이전 속도에서 감속 없이 지정 속도까지 가속.
    // dAccel != 0.0 이고 dDecel != 0.0 일 경우 이전 속도에서 지정 속도까지 가속후 등속 이후 감속.
    // dAccel == 0.0 이고 dDecel != 0.0 일 경우 이전 속도에서 다음 속도까지 감속.

    // 다음의 조건을 만족하여야 합니다.
    // dVel[1] == dVel[3]을 반드시 만족하여야 한다.
    // dVel[2]로 정속 구동 구간이 발생할 수 있도록 dPosition이 충분히 큰값이어야 한다.
    // Ex) dPosition = 10000;
    // dVel[0] = 300., dAccel[0] = 200., dDecel[0] = 0.;    <== 가속
    // dVel[1] = 500., dAccel[1] = 100., dDecel[1] = 0.;    <== 가속
    // dVel[2] = 700., dAccel[2] = 200., dDecel[2] = 250.;  <== 가속, 등속, 감속
    // dVel[3] = 500., dAccel[3] = 0.,   dDecel[3] = 150.;  <== 감속
    // dVel[4] = 200., dAccel[4] = 0.,   dDecel[4] = 350.;  <== 감속
    // 펄스 출력이 종료되는 시점에서 함수를 벗어난다
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartPosWithList(int nAxisNo, double dPosition, double[] dpVel, double[] dpAccel, double[] dpDecel, int nListNum);

    // 설정한 거리만큼 또는 위치까지 대상 축의 위치가 증감할 때 이동을 시작한다.
    // lEvnetAxisNo    : 시작 조건 발생 축
    // dComparePosition: 시작 조건 발생 축의 조건 발생 위치.
    // uPositionSource : 시작 조건 발생 축의 조건 발생 위치 기준 선택 => COMMAND(0), ACTUAL(1)
    // 예약 후 취소는 AxmMoveStop, AxmMoveEStop, AxmMoveSStop를 사용
    // 이동 축과 시작 조건 발생 축은 4축 단위 하나의 그룹(2V04의 경우 같은 모듈)에 존재하여야 합니다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartPosWithPosEvent(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, int nEventAxisNo, double dComparePosition, uint uPositionSource);

    // 지정 축을 설정한 감속도로 감속 정지 한다.
    // dDecel : 정지 시 감속율값
    [DllImport("AXL.dll")] public static extern uint AxmMoveStop(int nAxisNo, double dDecel);
        // 지정 축을 설정한 감속도로 감속 정지 한다.(PCI-Nx04 전용)
    // 현재 가감속 상태와 관계없이 즉시 감속 가능 함수이며 제한된 구동에 대하여 사용 가능하다.
    // -- 사용 가능 구동 : AxmMoveStartPos, AxmMoveVel, AxmLineMoveEx2.
    // dDecel : 정지 시 감속율값
    // 주의 : 감속율값은 최초 설정 감속율보다 크거나 같아야 한다.
    // 주의 : 감속 설정을 시간으로 하였을 경우 최초 설정 감속 시간보다 작거나 같아야 한다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveStopEx(int nAxisNo, double dDecel);
    // 지정 축을 급 정지 한다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveEStop(int nAxisNo);
    // 지정 축을 감속 정지한다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveSStop(int nAxisNo);

    //========= 오버라이드 함수 =========================================================================================
    // 위치 오버라이드 한다.
    // 지정 축의 구동이 종료되기 전 지정된 출력 펄스 수를 조정한다.
    // 주의사항: 오버라이드할 위치를 넣을때는 구동 시점의 위치를 기준으로한 Relative 형태의 위치값으로 넣어준다.
    //           구동시작후 같은방향의 경우 오버라이드를 계속할수있지만 반대방향으로 오버라이드할경우에는 오버라이드를 계속할수없다.
    [DllImport("AXL.dll")] public static extern uint AxmOverridePos(int nAxisNo, double dOverridePos);

    // 지정 축의 속도오버라이드 하기전에 오버라이드할 최고속도를 설정한다.
    // 주의점 : 속도오버라이드를 5번한다면 그중에 최고 속도를 설정해야된다. 
    [DllImport("AXL.dll")] public static extern uint AxmOverrideSetMaxVel(int nAxisNo, double dOverrideMaxVel);
    // 속도 오버라이드 한다.
    // 지정 축의 구동 중에 속도를 가변 설정한다. (반드시 모션 중에 가변 설정한다.)
    // 주의점: AxmOverrideVel 함수를 사용하기전에. AxmOverrideMaxVel 최고로 설정할수있는 속도를 설정해놓는다.
    // EX> 속도오버라이드를 두번한다면 
    // 1. 두개중에 높은 속도를 AxmOverrideMaxVel 설정 최고 속도값 설정.
    // 2. AxmMoveStartPos 실행 지정 축의 구동 중(Move함수 모두 포함)에 속도를 첫번째 속도로 AxmOverrideVel 가변 설정한다.
    // 3. 지정 축의 구동 중(Move함수 모두 포함)에 속도를 두번째 속도로 AxmOverrideVel 가변 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmOverrideVel(int nAxisNo, double dOverrideVelocity);
    // 가속도, 속도, 감속도를  오버라이드 한다.
    // 지정 축의 구동 중에 가속도, 속도, 감속도를 가변 설정한다. (반드시 모션 중에 가변 설정한다.)
    // 주의점: AxmOverrideAccelVelDecel 함수를 사용하기전에. AxmOverrideMaxVel 최고로 설정할수있는 속도를 설정해놓는다.
    // EX> 속도오버라이드를 두번한다면 
    // 1. 두개중에 높은 속도를 AxmOverrideMaxVel 설정 최고 속도값 설정.
    // 2. AxmMoveStartPos 실행 지정 축의 구동 중(Move함수 모두 포함)에 가속도, 속도, 감속도를 첫번째 속도로 AxmOverrideAccelVelDecel 가변 설정한다.
    // 3. 지정 축의 구동 중(Move함수 모두 포함)에 가속도, 속도, 감속도를 두번째 속도로 AxmOverrideAccelVelDecel 가변 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmOverrideAccelVelDecel(int nAxisNo, double dOverrideVelocity, double dMaxAccel, double dMaxDecel);
    // 어느 시점에서 속도 오버라이드 한다.
    // 어느 위치 지점과 오버라이드할 속도를 입력시켜 그위치에서 속도오버라이드 되는 함수
    // lTarget : COMMAND(0), ACTUAL(1)
    // 주의점: AxmOverrideVelAtPos 함수를 사용하기전에. AxmOverrideMaxVel 최고로 설정할수있는 속도를 설정해놓는다.
    [DllImport("AXL.dll")] public static extern uint AxmOverrideVelAtPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, double dOverridePos, double dOverrideVelocity, int nTarget);
    // 지정한 시점들에서 지정한 속도로 오버라이드 한다.
    // lArraySize     : 오버라이드 할 위치의 개수를 설정.
    // *dpOverridePos : 오버라이드 할 위치의 배열(lArraySize에서 설정한 개수보다 같거나 크게 선언해야됨)
    // *dpOverrideVel : 오버라이드 할 위치에서 변경 될 속도 배열(lArraySize에서 설정한 개수보다 같거나 크게 선언해야됨)
    // lTarget        : COMMAND(0), ACTUAL(1) 
    // uOverrideMode : 오버라이드 시작 방법을 지정함.
    //                : OVERRIDE_POS_START(0) 지정한 위치에서 지정한 속도로 오버라이드 시작함        
    //                : OVERRIDE_POS_END(1) 지정한 위치에서 지정한 속도가 되도록 미리 오버라이드 시작함
    [DllImport("AXL.dll")] public static extern uint AxmOverrideVelAtMultiPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, int nArraySize, double[] dpOverridePos, double[] dpOverrideVel, int nTarget, uint uOverrideMode);

    // 지정한 시점들에서 지정한 속도/가감속도로 오버라이드 한다.(MLII 전용)
    // lArraySize     : 오버라이드 할 위치의 개수를 설정(최대 5).
    // *dpOverridePos : 오버라이드 할 위치의 배열(lArraySize에서 설정한 개수보다 같거나 크게 선언해야됨)
    // *dpOverrideVel : 오버라이드 할 위치에서 변경 될 속도 배열(lArraySize에서 설정한 개수보다 같거나 크게 선언해야됨)
    // *dpOverrideAccelDecel : 오버라이드 할 위치에서 변경 될 가감속도 배열(lArraySize에서 설정한 개수보다 같거나 크게 선언해야됨)
    // lTarget        : COMMAND(0), ACTUAL(1) 
    // dwOverrideMode : 오버라이드 시작 방법을 지정함.
    //                : OVERRIDE_POS_START(0) 지정한 위치에서 지정한 속도로 오버라이드 시작함  
    //                : OVERRIDE_POS_END(1) 지정한 위치에서 지정한 속도가 되도록 미리 오버라이드 시작함
    [DllImport("AXL.dll")] public static extern uint AxmOverrideVelAtMultiPos2(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, long lArraySize, double[] dpOverridePos, double[] dpOverrideVel, double[] dpOverrideAccelDecel, int nTarget, uint dwOverrideMode);
    
    //========= 마스터, 슬레이브  기어비로 구동 함수 ====================================================================
    // Electric Gear 모드에서 Master 축과 Slave 축과의 기어비를 설정한다.
    // dSlaveRatio : 마스터축에 대한 슬레이브의 기어비( 0 : 0% , 0.5 : 50%, 1 : 100%)
    [DllImport("AXL.dll")] public static extern uint AxmLinkSetMode(int nMasterAxisNo, int nSlaveAxisNo, double dSlaveRatio);
    // Electric Gear 모드에서 설정된 Master 축과 Slave 축과의 기어비를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmLinkGetMode(int nMasterAxisNo, ref uint uSlaveAxisNo, ref double dpGearRatio);
    // Master 축과 Slave축간의 전자기어비를 설정 해제 한다.
    [DllImport("AXL.dll")] public static extern uint AxmLinkResetMode(int nMasterAxisNo);

    //======== 겐트리 관련 함수==========================================================================================
    // 모션모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어를 지원한다. 
    // 이 함수를 이용해 Master축을 겐트리 제어로 설정하면 해당 Slave축은 Master축과 동기되어 구동됩니다. 
    // 만약 겐트리 설정 이후 Slave축에 구동명령이나 정지 명령등을 내려도 모두 무시됩니다.
    // uSlHomeUse     : 슬레이축 홈사용 우뮤 ( 0 - 2)
    //             (0 : 슬레이브축 홈을 사용안하고 마스터축을 홈을 찾는다.)
    //             (1 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정함.)
    //             (2 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정안함.)
    // dSlOffset      : 슬레이브축 옵셋값
    // dSlOffsetRange : 슬레이브축 옵셋값 레인지 설정
    // 주의사항       : 갠트리 ENABLE시 슬레이브축은 모션중 AxmStatusReadMotion 함수로 확인하면 True(Motion 구동 중)로 확인되야 정상동작이다. 
    //                  슬레이브축에 AxmStatusReadMotion로 확인했을때 InMotion 이 False이면 Gantry Enable이 안된것이므로 알람 혹은 리밋트 센서 등을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxmGantrySetEnable(int nMasterAxisNo, int nSlaveAxisNo, uint uSlHomeUse, double dSlOffset, double dSlOffsetRange);

    // Slave축의 Offset값을 알아내는방법.
    // A. 마스터, 슬레이브를 모두 서보온을 시킨다.                 
    // B. AxmGantrySetEnable함수에서 uSlHomeUse = 2로 설정후 AxmHomeSetStart함수를 이용해서 홈을 찾는다. 
    // C. 홈을 찾고 나면 마스터축의 Command값을 읽어보면 마스터축과 슬레이브축의 틀어진 Offset값을 볼수있다.
    // D. Offset값을 읽어서 AxmGantrySetEnable함수의 dSlOffset인자에 넣어준다. 
    // E. dSlOffset값을 넣어줄때 마스터축에 대한 슬레이브 축 값이기때문에 부호를 반대로 -dSlOffset 넣어준다.
    // F. dSIOffsetRange 는 Slave Offset의 Range 범위를 말하는데 Range의 한계를 지정하여 한계를 벗어나면 에러를 발생시킬때 사용한다.
    // G. AxmGantrySetEnable함수에 Offset값을 넣어줬으면  AxmGantrySetEnable함수에서 uSlHomeUse = 1로 설정후 AxmHomeSetStart함수를 이용해서 홈을 찾는다.

    // 겐트리 구동에 있어 사용자가 설정한 파라메타를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmGantryGetEnable(int nMasterAxisNo, ref uint upSlHomeUse, ref double dpSlOffset, ref double dSlORange, ref uint uGatryOn);
    // 모션 모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어를 해제한다.
    [DllImport("AXL.dll")] public static extern uint AxmGantrySetDisable(int nMasterAxisNo, int nSlaveAxisNo);

    // PCI-Rxx04-MLII 전용.
    // 모션 모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어 중 동기 보상 기능을 설정한다.
    // lMasterGain, lSlaveGain : 두 축간 위치 편차에 대한 보상 값 반영 비율을 % 값으로 입력한다.
    // lMasterGain, lSlaveGain : 0을 입력하면 두 축간 위치 편차 보상 기능을 사용하지 않음. 기본값 : 0%
    [DllImport("AXL.dll")] public static extern uint AxmGantrySetCompensationGain(int nMasterAxisNo, int nMasterGain, int nSlaveGain);
    // 모션 모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어 중 동기 보상 기능을 설정을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxmGantryGetCompensationGain(int nMasterAxisNo, ref int nMasterGain, ref int nSlaveGain);

    // Master 와 Slave 간 오차 범위를 설정 하고 오차범위 이상이면 Read 함수의 Status에 TRUE를 반환 한다.
    // PCI-R1604 / PCI-R3200-MLIII 전용 함수
    // lMasterAxisNo : Gantry Master Axis No
    // dErrorRange : 오차 범위 설정 값 0~2147483647 (양수만 입력 음수 입력시 1170 Error Code Return)
    // uUse : 모드 설정
    //      ( 0 : Disable)
    //      ( 1 : User 감시 모드)
    //      ( 2 : Flag Latch 모드)
    //      ( 3 : Flag Latch 모드 + Error 발생시 SSTOP)
    //      ( 4 : Flag Latch 모드 + Error 발생시 ESTOP)
    [DllImport("AXL.dll")] public static extern uint AxmGantrySetErrorRange(int nMasterAxisNo, double dErrorRange, uint uUse);
    // Master 와 Slave 간의 오차 범위 설정값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmGantryGetErrorRange(int nMasterAxisNo, ref double dpErrorRange, ref uint upUse);
    // Master 와 Slave 간의 오차값 비교 결과를 반환 한다.
    // dwpStatus : FALSE(0) -> Master 와 Slave 사이의 오차범위가 설정한 오차범위 보다 작다. (정상상태)
    //             TRUE(1) -> Master 와 Slave 사이의 오차범위가 설정한 오차범위 보다 크다. (비정상상태)
    // AxmGantryReadErrorRangeStatus 함수의 경우 InMotion && Gantry Enable && Master/Slave Servo On 상태를 만족 할 때만
    // AXT_RT_SUCCESS를 Return 하며 위의 상태를 만족하지 않으면 Error Code를 Return 한다.
    [DllImport("AXL.dll")] public static extern uint AxmGantryReadErrorRangeStatus(int nMasterAxisNo, ref uint dwpStatus);
    // Master 와 Slave 간의 오차값을 반환 한다.
    // Flag Latch 모드 일때 Latch 된 값을 사용자가 읽어 간 후 다음 Error가 발생 되기 전까지 Error가 발생 했을 때의 값을 유지 하게 됩니다.
    // dwpStatus 가 1일 때만 Read 해야 한다. 계속 ComparePos 를 Read 하면 부하가 많이 걸려 함수 응답속도가 느려지게 된다.
    [DllImport("AXL.dll")] public static extern uint AxmGantryReadErrorRangeComparePos(int nMasterAxisNo, ref double dpComparePos);

    //====일반 보간함수 =================================================================================================
    // 주의사항1: AxmContiSetAxisMap함수를 이용하여 축맵핑후에 낮은순서축부터 맵핑을 하면서 사용해야된다.
    //           원호보간의 경우에는 반드시 낮은순서축부터 축배열에 넣어야 동작 가능하다.

    // 주의사항2: 위치를 설정할경우 반드시 마스터축과 슬레이브 축의 UNIT/PULSE의 맞추어서 설정한다.
    //           위치를 UNIT/PULSE 보다 작게 설정할 경우 최소단위가 UNIT/PULSE로 맞추어지기때문에 그위치까지 구동이 될수없다.

    // 주의사항3: 원호 보간을 할경우 반드시 한칩내에서 구동이 될수있으므로 
    //            4축내에서만 선택해서 사용해야된다.

    // 주의사항4: 보간 구동 시작/중에 비정상 정지 조건(+- Limit신호, 서보 알람, 비상정지 등)이 발생하면 
    //            구동 방향에 상관없이 구동을 시작하지 않거나 정지 된다.

    // 직선 보간 한다.
    // 시작점과 종료점을 지정하여 다축 직선 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점과 종료점을 지정하여 직선 보간 구동하는 Queue에 저장함수가된다. 
    // 직선 프로파일 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    [DllImport("AXL.dll")] public static extern uint AxmLineMove(int nCoord, double[] dPos, double dVel, double dAccel, double dDecel);

    // 2축 단위 직선 보간 한다.(Software 방식)
    // 시작점과 종료점을 지정하여 다축 직선 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    [DllImport("AXL.dll")] public static extern uint AxmLineMoveEx2(int nCoord, double[] dpEndPos, double dVel, double dAccel, double dDecel);

    // 2축 원호보간 한다.
    // 시작점, 종료점과 중심점을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode, 와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dCenterPos = 중심점 X,Y 배열 , dEndPos = 종료점 X,Y 배열.
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmCircleCenterMove(int nCoord, int[] nAxisNo, double[] dCenterPos, double[] dEndPos, double dVel, double dAccel, double dDecel, uint uCWDir);

    // 중간점, 종료점을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 중간점, 종료점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dMidPos = 중간점 X,Y 배열 , dEndPos = 종료점 X,Y 배열, lArcCircle = 아크(0), 원(1)
    [DllImport("AXL.dll")] public static extern uint AxmCirclePointMove(int nCoord, int[] nAxisNo, double[] dMidPos, double[] dEndPos, double dVel, double dAccel, double dDecel, int nArcCircle);

    // 중간점, 종료점을 지정하여 3차원 원/원호 보간을 지원한다. 구동 시작 후 함수를 벗어난다.
    // 축 매핑은 3축이상 가능하며 3축 이상의 축들은 Linear Interpolation된다.
    [DllImport("AXL.dll")] public static extern uint AxmCirclePointMoveEx(int nCoordNo,  ref int nAxisNo,  ref double dpMidPos, ref double dpEndPos, double dVel, double dAccel, double dDecel, int nArcCircle, int nArraySize);
    // 시작점, 종료점과 반지름을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dRadius = 반지름, dEndPos = 종료점 X,Y 배열 , uShortDistance = 작은원(0), 큰원(1)
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmCircleRadiusMove(int nCoord, int[] nAxisNo, double dRadius, double[] dEndPos, double dVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance);

    // 시작점, 회전각도와 반지름을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dCenterPos = 중심점 X,Y 배열 , dAngle = 각도.
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmCircleAngleMove(int nCoord, int[] nAxisNo, double[] dCenterPos, double dAngle, double dVel, double dAccel, double dDecel, uint uCWDir);

    //====연속 보간 함수 ================================================================================================
    // 지정된 좌표계에 연속보간 축 맵핑을 설정한다.
    // (축맵핑 번호는 0 부터 시작))
    // 주의점: 축맵핑할때는 반드시 실제 축번호가 작은 숫자부터 큰숫자를 넣는다.
    //         가상축 맵핑 함수를 사용하였을 때 가상축번호를 실제 축번호가 작은 값 부터 lpAxesNo의 낮은 인텍스에 입력하여야 한다.
    //         가상축 맵핑 함수를 사용하였을 때 가상축번호에 해당하는 실제 축번호가 다른 값이라야 한다.
    //         같은 축을 다른 Coordinate에 중복 맵핑하지 말아야 한다.
    [DllImport("AXL.dll")] public static extern uint AxmContiSetAxisMap(int nCoord, uint uSize, int[] npRealAxesNo);
    //지정된 좌표계에 연속보간 축 맵핑을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmContiGetAxisMap(int nCoord, ref uint uSize, int[] npRealAxesNo);
    //지정된 좌표계에 연속보간 축 맵핑을 초기화한다.
    [DllImport("AXL.dll")] public static extern uint AxmContiResetAxisMap(int lCoordinate);

    // 지정된 좌표계에 연속보간 축 절대/상대 모드를 설정한다.
    // (주의점 : 반드시 축맵핑 하고 사용가능)
    // 지정 축의 이동 거리 계산 모드를 설정한다.
    // uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
    //               POS_REL_MODE '1' - 상대 좌표계
    [DllImport("AXL.dll")] public static extern uint AxmContiSetAbsRelMode(int nCoord, uint uAbsRelMode);
    // 지정된 좌표계에 연속보간 축 절대/상대 모드를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmContiGetAbsRelMode(int nCoord, ref uint upAbsRelMode);

    // 지정된 좌표계에 보간 구동을 위한 내부 Queue가 비어 있는지 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmContiReadFree(int nCoord, ref uint upQueueFree);
    // 지정된 좌표계에 보간 구동을 위한 내부 Queue에 저장되어 있는 보간 구동 개수를 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmContiReadIndex(int nCoord, ref int npQueueIndex);

    // 지정된 좌표계에 연속 보간 구동을 위해 저장된 내부 Queue를 모두 삭제하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmContiWriteClear(int nCoord);

    // 지정된 좌표계에 연속보간에서 수행할 작업들의 등록을 시작한다. 이함수를 호출한후,
    // AxmContiEndNode함수가 호출되기 전까지 수행되는 모든 모션작업은 실제 모션을 수행하는 것이 아니라 연속보간 모션으로 등록 되는 것이며,
    // AxmContiStart 함수가 호출될 때 비로소 등록된모션이 실제로 수행된다.
    [DllImport("AXL.dll")] public static extern uint AxmContiBeginNode(int nCoord);
    // 지정된 좌표계에서 연속보간을 수행할 작업들의 등록을 종료한다.
    [DllImport("AXL.dll")] public static extern uint AxmContiEndNode(int nCoord);

    // 연속 보간 시작 한다.
    // uProfileset(CONTI_NODE_VELOCITY(0) : 연속 보간 사용, CONTI_NODE_MANUAL(1) : 프로파일 보간 사용, CONTI_NODE_AUTO(2) : 자동 프로파일 보간, 3 : 속도보상 모드 사용) 
    [DllImport("AXL.dll")] public static extern uint AxmContiStart(int nCoord, uint uProfileset, int nAngle);
    // 지정된 좌표계에 연속 보간 구동 중인지 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmContiIsMotion(int nCoord, ref uint upInMotion);

    // 지정된 좌표계에 연속 보간 구동 중 현재 구동중인 연속 보간 인덱스 번호를 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmContiGetNodeNum(int nCoord, ref int npNodeNum);
    // 지정된 좌표계에 설정한 연속 보간 구동 총 인덱스 갯수를 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmContiGetTotalNodeNum(int nCoord, ref int npNodeNum);


    // 특정 모션 세그먼트에서 I/O출력
    // AxmContiBeginNode 함수와 AxmContiEndNode 함수 사이에서 사용하여야 된다.
    // 다음 연속 보간 구동 함수(ex: AxmLineMove, AxmCircleCenterMove, etc...)에 대해서만 유효하다.
    // Digital I/O 출력 시점은 다음 연속 보간 구동함수의 종점을 기준으로 조건(dpDistTime, lpDistTimeMode)만큼 이전에 출력한다.
    //
    // lSize :  동시에 출력할 IO 접점 수 (1 ~ 8)
    // lModuleNo : dwModuleType=0 일때 는 축번호, dwModuleType=1일 경우는 Digital I/O Module No.
    // dwModuleType : 0=Motion I/O Output(Slave 자체의 출력), 1=Digital I/O Output
    // 
    // % 아래 4개의 매개 변수는 lSize 만큼의 배열로 입력해서 여러 출력 접점을 동시에 제어할 수 있다.
    // lpBit : 출력 접점에 대한 Offset 위치
    // lpOffOn : 해당 출력 접점의 출력값 [LOW(0), HIGH(1)]
    // dpDistTime : 거리 값(pulse), 시간 값(msec) => 모션 프로파일 종점을 기준으로 한다.
    // lpDistTimeMode : 0=거리 모드, 1=시간모드 => 모션 프로파일 종점을 기준으로 한다.
    [DllImport("AXL.dll")] public static extern uint AxmContiDigitalOutputBit(int nCoord, int nSize, int nModuleType, int[] npModuleNo, int[] npBit, int[] npOffOn, double[] dpDistTime, int[] npDistTimeMode);

    // 지정된 좌표계 EndStatus(정지 상태) 레지스터를 반환한다.
    //    축의 모션 종료 상태값:
    //     - [00000000h] 구동하지 않음 또는 초기 상태
    //     - [00000001h]Bit 0, 정방향 리미트 신호(PELM)에 의한 종료
    //     - [00000002h]Bit 1, 역방향 리미트 신호(NELM)에 의한 종료
    //     - [00000010h]Bit 4, 정방향 소프트 리미트 급정지 기능에 의한 구동 종료
    //     - [00000020h]Bit 5, 역방향 소프트 리미트 급정지 기능에 의한 구동 종료
    //     - [00000040h]Bit 6, 정방향 소프트 리미트 감속정지 기능에 의한 구동 종료
    //     - [00000080h]Bit 7, 역방향 소프트 리미트 감속정지 기능에 의한 구동 종료
    //     - [00000100h]Bit 8, 서보 알람 기능에 의한 구동 종료
    //     - [00000200h]Bit 9, 비상 정지 신호 입력에 의한 구동 종료
    //     - [00000400h]Bit 10, 급 정지 명령에 의한 구동 종료
    //     - [00000800h]Bit 11, 감속 정지 명령에 의한 구동 종료
    //     - [00001000h]Bit 12, 전축 급정지 명령에 의한 구동 종료
    //     - [00040000h]Bit 18, 신호 검색 성공 종료
    //     - [00080000h]Bit 19, 보간 데이터 이상으로 구동 종료
    //     - [00100000h]Bit 20, 비정상 구동 정지발생(EMG, Alarm, Limit, Software Limit)
    //     - [10000000h]Bit 28, 현재/마지막 구동 드라이브 방향
    //     - 비어있는 Bit는 N/A
    [DllImport("AXL.dll")] public static extern uint AxmContiReadStop(int lCoord, ref uint upStatus);

    // LineMove 함수들에 대해 속도지정 방식을 지정한다.
    // > uVelMode: 속도 모드를 지정
    //   - [0] -> 벡터 속도
    //   - [1] -> 지정축의 속도
    //   - [2] -> 장축의 속도(Pulse 기준)
    // > lMasterAxisNo: uVelMode가 [1]일때 지정축
    // { RETURN VALUE }
    //   - [0] -> LineMove 함수의 속도 모드 설정이 성공했을 때
    [DllImport("AXL.dll")] public static extern uint AxmContiSetLineMoveVelMode(int lCoord, uint uVelMode, int lMasterAxisNo);
    [DllImport("AXL.dll")] public static extern uint AxmContiGetLineMoveVelMode(int lCoord, ref uint upVelMode, ref int lpMasterAxisNo);

    // LineMove 함수로 구동 및 예약할 때 소프트 리밋 체크기능 사용여부를 지정한다.
    // > uEnable: 속도 모드를 지정
    //   - [0] -> 체크기능 사용하지 않음
    //   - [1] -> 체크기능 사용함
    [DllImport("AXL.dll")] public static extern uint AxmContiSetCheckSoftLimit(int lCoord, uint uUse);
    [DllImport("AXL.dll")] public static extern uint AxmContiGetCheckSoftLimit(int lCoord, ref uint upUse);

    [DllImport("AXL.dll")] public static extern uint AxmContiSetConnectionRadius(int lCoord, double dRadius);
    [DllImport("AXL.dll")] public static extern uint AxmContiSetConnectionMaxCentripetalAccel(int lCoord, double dMaxAccel);
    //====================트리거 함수 ===================================================================================
    // 주의사항: 트리거 위치를 설정할경우 반드시 UNIT/PULSE의 맞추어서 설정한다.
    //           위치를 UNIT/PULSE 보다 작게할 경우 최소단위가 UNIT/PULSE로 맞추어지기때문에 그위치에 출력할수없다.

    // 지정 축에 트리거 기능의 사용 여부, 출력 레벨, 위치 비교기, 트리거 신호 지속 시간 및 트리거 출력 모드를 설정한다.
    // 트리거 기능 사용을 위해서는 먼저  AxmTriggerSetTimeLevel 를 사용하여 관련 기능 설정을 먼저 하여야 한다.
    // dTrigTime : 트리거 출력 시간, 1usec - 최대 50msec ( 1 - 50000 까지 설정)
    // upTriggerLevel  : 트리거 출력 레벨 유무 => LOW(0), HIGH(1)
    // uSelect         : 사용할 기준 위치      => COMMAND(0), ACTUAL(1)
    // uInterrupt      : 인터럽트 설정         => DISABLE(0), ENABLE(1)

    // 지정 축에 트리거 신호 지속 시간 및 트리거 출력 레벨, 트리거 출력방법을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmTriggerSetTimeLevel(int nAxisNo, double dTrigTime, uint uTriggerLevel, uint uSelect, uint uInterrupt);
    // 지정 축에 트리거 신호 지속 시간 및 트리거 출력 레벨, 트리거 출력방법을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmTriggerGetTimeLevel(int nAxisNo, ref double dTrigTime, ref uint uTriggerLevel, ref uint uSelect, ref uint uInterrupt);

    // 지정 축의 트리거 출력 기능을 설정한다.
    // uMethod : PERIOD_MODE  0x0 : 현재 위치를 기준으로 dPos를 위치 주기로 사용한 주기 트리거 방식
    //           ABS_POS_MODE 0x1 : 트리거 절대 위치에서 트리거 발생, 절대 위치 방식
    // dPos : 주기 선택시 : 위치마다위치마다 출력하기때문에 그 위치
    //        절대 선택시 : 출력할 그 위치, 이 위치와같으면 무조건 출력이 나간다. 
    // 주의사항: AxmTriggerSetAbsPeriod의 주기모드로 설정할경우 처음 그위치가 범위 안에 있으므로 트리거 출력이 한번 발생한다.
    [DllImport("AXL.dll")] public static extern uint AxmTriggerSetAbsPeriod(int nAxisNo, uint uMethod, double dPos);
    // 지정 축에 트리거 기능의 사용 여부, 출력 레벨, 위치 비교기, 트리거 신호 지속 시간 및 트리거 출력 모드를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmTriggerGetAbsPeriod(int nAxisNo, ref uint upMethod, ref double dpPos);

    // 사용자가 지정한 시작위치부터 종료위치까지 일정구간마다 트리거를 출력 한다.
    [DllImport("AXL.dll")] public static extern uint AxmTriggerSetBlock(int nAxisNo, double dStartPos, double dEndPos, double dPeriodPos);
    // 'AxmTriggerSetBlock' 함수의 트리거 설정한 값을 읽는다..
    [DllImport("AXL.dll")] public static extern uint AxmTriggerGetBlock(int nAxisNo, ref double dpStartPos, ref double dpEndPos, ref double dpPeriodPos);

    // 사용자가 한 개의 트리거 펄스를 출력한다.
    [DllImport("AXL.dll")] public static extern uint AxmTriggerOneShot(int nAxisNo);
    // 사용자가 한 개의 트리거 펄스를 몇초후에 출력한다.
    [DllImport("AXL.dll")] public static extern uint AxmTriggerSetTimerOneshot(int nAxisNo, int mSec);
    // 절대위치 트리거 무한대 절대위치 출력한다.
    [DllImport("AXL.dll")] public static extern uint AxmTriggerOnlyAbs(int nAxisNo, int nTrigNum, double[] dTrigPos);
    // 트리거 설정을 리셋한다.
    [DllImport("AXL.dll")] public static extern uint AxmTriggerSetReset(int nAxisNo);

    //======== CRC( 잔여 펄스 클리어 함수)===============================================================================
    // Level   : LOW(0), HIGH(1), UNUSED(2), USED(3)
    // uMethod : 잔여펄스 제거 출력 신호 펄스 폭 0 - 7까지 설정가능.
    //           0 : 30 uSec , 1 : 100 uSec, 2: 500 uSec, 3:1 mSec, 4:10 mSec, 5:50 mSec, 6:100 mSec
    //지정 축에 CRC 신호 사용 여부 및 출력 레벨을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmCrcSetMaskLevel(int nAxisNo, uint uLevel, uint uMethod);
    // 지정 축의 CRC 신호 사용 여부 및 출력 레벨을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCrcGetMaskLevel(int nAxisNo, ref uint upLevel, ref uint upMethod);

    //uOnOff  : CRC 신호를 Program으로 발생 여부  (FALSE(0),TRUE(1))
    // 지정 축에 CRC 신호를 강제로 발생 시킨다.
    [DllImport("AXL.dll")] public static extern uint AxmCrcSetOutput(int nAxisNo, uint uOnOff);
    // 지정 축의 CRC 신호를 강제로 발생 여부를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCrcGetOutput(int nAxisNo, ref uint upOnOff);

    //======MPG(Manual Pulse Generation) 함수============================================================================
    // lInputMethod  : 0-3 까지 설정가능. 0:OnePhase, 1:사용 안함, 2:TwoPhase2, 3:TwoPhase4
    // lDriveMode    : 0만 설정가능 (0 :MPG 연속모드)
    // MPGPos        : MPG 입력신호마다 이동하는 거리
    // MPGdenominator: MPG(수동 펄스 발생 장치 입력)구동 시 나누기 값
    // dMPGnumerator : MPG(수동 펄스 발생 장치 입력)구동 시 곱하기 값
    // uNumerator    : 최대(1 에서    64) 까지 설정 가능
    // uDenominator  : 최대(1 에서  4096) 까지 설정 가능
    // dMPGdenominator = 4096, MPGnumerator=1 가 의미하는 것은 
    // MPG 한바퀴에 200펄스면 그대로 1:1로 1펄스씩 출력을 의미한다. 
    // 만약 dMPGdenominator = 4096, MPGnumerator=2 로 했을경우는 1:2로 2펄스씩 출력을 내보낸다는의미이다. 
    // 여기에 MPG PULSE = ((Numerator) * (Denominator)/ 4096 ) 칩내부에 출력나가는 계산식이다.
    // 주의사항     : AxmStatusReadInMotion 함수 실행 결과에 유의한다.  (AxmMPGReset 하기 전까지 정상 상태에서는 모션 구동 중 상태.)

    // 지정 축에 MPG 입력방식, 드라이브 구동 모드, 이동 거리, MPG 속도 등을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmMPGSetEnable(int nAxisNo, int nInputMethod, int nDriveMode, double dMPGPos, double dVel, double dAccel);
    // 지정 축에 MPG 입력방식, 드라이브 구동 모드, 이동 거리, MPG 속도 등을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMPGGetEnable(int nAxisNo, ref int npInputMethod, ref int npDriveMode, ref double dpMPGPos, ref double dpVel);

    // PCI-Nx04 함수 전용.
    // 지정 축에 MPG 드라이브 구동 모드에서 한펄스당 이동할 펄스 비율을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmMPGSetRatio(int nAxisNo, double dMPGnumerator, double dMPGdenominator);
    // 지정 축에 MPG 드라이브 구동 모드에서 한펄스당 이동할 펄스 비율을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMPGGetRatio(int nAxisNo, ref double dMPGnumerator, ref double dMPGdenominator);

    // 지정 축에 MPG 드라이브 설정을 해지한다.
    [DllImport("AXL.dll")] public static extern uint AxmMPGReset(int nAxisNo);

    //======= 헬리컬 이동 ===============================================================================================
    // 주의사항 : Helix를 연속보간 사용시 Spline, 직선보간과 원호보간을 같이 사용할수없다.

    // 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 헬리컬 보간 구동하는 함수이다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 헬리컬 연속보간 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다)
    // dCenterPos = 중심점 X,Y  , dEndPos = 종료점 X,Y
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmHelixCenterMove(int nCoord, double dCenterXPos, double dCenterYPos, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dAccel, double dDecel, uint uCWDir);

    // 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬 보간 구동하는 함수이다. 
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 중간점, 종료점을 지정하여 헬리컬연속 보간 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    // dMidPos = 중간점 X,Y  , dEndPos = 종료점 X,Y
    [DllImport("AXL.dll")] public static extern uint AxmHelixPointMove(int nCoord, double dMidXPos, double dMidYPos, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dAccel, double dDecel);

    // 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬 보간 구동하는 함수이다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬연속 보간 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    // dRadius = 반지름, dEndPos = 종료점 X,Y  , uShortDistance = 작은원(0), 큰원(1)
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmHelixRadiusMove(int nCoord, double dRadius, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance);

    // 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 헬리컬 보간 구동하는 함수이다
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 헬리컬연속 보간 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    //dCenterPos = 중심점 X,Y  , dAngle = 각도.
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmHelixAngleMove(int nCoord, double dCenterXPos, double dCenterYPos, double dAngle, double dZPos, double dVel, double dAccel, double dDecel, uint uCWDir);

    //======== 스플라인 이동 ============================================================================================
    // 주의사항 : Spline를 연속보간 사용시 Helix , 직선보간과 원호보간을 같이 사용할수없다.

    // AxmContiBeginNode, AxmContiEndNode와 같이사용안함. 
    // 스플라인 연속 보간 구동하는 함수이다. 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다.
    // AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    // lPosSize : 최소 3개 이상.
    // 2축으로 사용시 dPoZ값을 0으로 넣어주면 됨.
    // 3축으로 사용시 축맵핑을 3개및 dPosZ 값을 넣어준다.
    [DllImport("AXL.dll")] public static extern uint AxmSplineWrite(int nCoord, int nPosSize, double[] dPosX, double[] dPosY, double dVel, double dAccel, double dDecel, double dPosZ, int nPointFactor);

    //======== PCI-R1604-MLII/SIIIH, PCIe-Rxx04-SIIIH 전용 함수 ================================================================================== 
    // 위치 보정 테이블 기능에 필요한 내용을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationSet(int nAxisNo, int nNumEntry, double dStartPos, ref double dpPosition, ref double dpCorrection, uint uRollOver);
    // 위치 보정 테이블 기능 설정 내용을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationGet(int nAxisNo, ref int npNumEntry, ref double dpStartPos, ref double dpPosition, ref double dpCorrection, ref uint upRollOver);

    // 위치 보정 테이블 기능의 사용유부를 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationEnable(int nAxisNo, uint uEnable);
    // 위치 보정 테이블 기능의 사용유무에 대한 설정 상태를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationIsEnable(int nAxisNo, ref uint upEnable);
    // 현재 지령 위치에서의 보정값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationGetCorrection(int nAxisNo, ref double dpCorrection);


    	// Backlash에 관련된 설정을하는 함수
	// > lBacklashDir: Backlash 보상을 적용할 구동 방향을 설정 (원점검색 방향과 동일하게 설정함)  
	//   - [0] -> Command Position값이 (+)방향으로 구동할 때 지정한 backlash를 적용함 
	//   - [1] -> Command Position값이 (-)방향으로 구동할 때 지정한 backlash를 적용함
	//   - Ex1) lBacklashDir이 0, backlash가 0.01일 때 0.0 -> 100.0으로 위치이동 할 때 실제 이동하는 위치는 100.01이됨
	//   - Ex2) lBacklashDir이 0, backlash가 0.01일 때 0.0 -> -100.0으로 위치이동 할 때 실제 이동하는 위치는 -100.0이됨
	//   ※ NOTANDUM 
	//   - 정확한 Backlash보상을 위해서는 원점검색시 마지막에 Backlash양 만큼 (+)Or(-)방향으로 이동 한 후 원점을 완료하고
	//     Backlash보정을 사용한다. 이 때 Backlash양 만큼 (+)구동을 했다면 backlash_dir을 [1](-)로, (-)구동을 했다면
	//     backlash_dir을 [0](+)로 설정하면 된다.
	// > dBacklash: 기구부에서 진행 방향과 반대반향으로 방향전환시 발생되는 Backlash양을 설정함
	// { RETURN VALUE } 
	//   - [0] -> Backlash 설정이 성공했을 때
    [DllImport("AXL.dll")] public static extern uint AxmCompensationSetBacklash(int nAxisNo, int lBacklashDir, double dBacklash);
	// Backlash에 관련된 설정 내용을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationGetBacklash(int nAxisNo, ref int lpBacklashDir, ref double dpBacklash);
	// Backlash사용유무를 설정/확인하는 함수
	// > dwEnable: Backlash보정 사용유무를 지정
	//   - [0]DISABLE -> Backlash보정을 사용안함    
	//   - [1]ENABLE  -> Backlash보정을 사용함
	// { RETURN VALUE } 
	//   - [0] -> Backlash 설정반환이 성공했을 때
    //   - [4303] -> Backlash 보정기능이 설정되어있지않을 때
    [DllImport("AXL.dll")] public static extern uint AxmCompensationEnableBacklash(int nAxisNo, uint dwEnable);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationIsEnableBacklash(int nAxisNo, ref uint dwpEnable);
   	// Backlash보정기능을 사용할 때 Backlash양 만큼 좌우로 이동하여 기구물의 위치를 자동 정렬함(서보 온 동작 이후 한번 사용함)
	// > dVel: 이동 속도[unit / sec]
	// > dAccel: 이동가속도[unit / sec^2]
	// > dAccel: 이동감속도[unit / sec^2]
	// > dWaitTime: Backlash 양만큼 구동 후 원래의 위치로 되돌아올기 까지의 대기시간[msec]
	// { RETURN VALUE } 
	//   - [0]    -> Backlash 보정을 위한 위치설정이 성공했을 때
	//   - [4303] -> Backlash 보정기능이 설정되어있지않을 때
    [DllImport("AXL.dll")] public static extern uint AxmCompensationSetLocating(int lAxisNo, double dVel, double dAccel, double dDecel, double dWaitTime);	

    // ECAM 기능에 필요한 내용을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmEcamSet(int nAxisNo, int nMasterAxisNo, int nNumEntry, double dMasterStartPos, ref double dpMasterPos, ref double dpSlavePos);
    // ECAM 기능에 필요한 내용을 CMD/ACT Source와 함께 설정한다. (PCIe-Rxx04-SIIIH 전용 함수)
    [DllImport("AXL.dll")] public static extern uint AxmEcamSetWithSource(int nAxisNo, int nMasterAxis, int nNumEntry, double dMasterStartPos, ref double dpMasterPos, ref double dpSlavePos, uint dwSource);
    // ECAM 기능 설정 내용을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmEcamGet(int nAxisNo, ref int npMasterAxisNo, ref int npNumEntry, ref double dpMasterStartPos, ref double dpMasterPos, ref double dpSlavePos);
    // ECAM 기능 설정 내용을 CMD/ACT Source와 함께 반환한다. (PCIe-Rxx04-SIIIH 전용 함수)
    [DllImport("AXL.dll")] public static extern uint AxmEcamGetWithSource(int nAxisNo, ref int npMasterAxis, ref int npNumEntry, ref double dpMasterStartPos, ref double dpMasterPos, ref double dpSlavePos, ref uint dwpSource);

    // ECAM 기능의 사용 유무를 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmEcamEnableBySlave(int nAxisNo, uint uEnable);
    // ECAM 기능의 사용 유무를 지정한 Master 축에 연결된 모든 Slave 축에 대하여 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmEcamEnableByMaster(int nAxisNo, uint uEnable);
    // ECAM 기능의 사용 유무에 대한 설정 상태를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmEcamIsSlaveEnable(int nAxisNo, ref uint upEnable);

    //======== Servo Status Monitor =====================================================================================
    // 지정 축의 예외 처리 기능에 대해 설정한다.(MLII : Sigma-5, SIIIH : MR_J4_xxB 전용)
    // uSelMon(0~3) : 감시 정보 선택.
    //          [0] : Torque 
    //          [1] : Velocity of motor
    //          [2] : Accel. of motor
    //          [3] : Decel. of motor
    //          [4] : Position error between Cmd. position and Act. position.
    // uActionValue : 이상 동작 판정 기준 값 설정. 각 정보에 따라 설정 값의 의미가 다음.
    //          [0] : uSelMon에서 선택한 감시 정보에 대하여 예외 처리 하지 않음.
    //         [>0] : uSelMon에서 선택한 감시 정보에 대하여 예외 처리 기능 적용.
    // uAction(0~3) : uActionValue 이상으로 감시 정보가 확인되었을때 예외처리 방법 설정.
    //          [0] : Warning(setting flag only) 
    //          [1] : Warning(setting flag) + Slow-down stop
    //          [2] : Warning(setting flag) + Emergency stop
    //          [3] : Warning(setting flag) + Emergency stop + Servo-Off
    // ※ 주의: 5개의 SelMon 정보에 대해 각각 예외처리 설정이 가능하며, 사용중 예외처리를 원하지않을 경우
    //          반드시 해당 SelMon 정보의 ActionValue값을 0으로 설정해 감시기능을 Disable 해됨.
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetServoMonitor(int nAxisNo, uint uSelMon, double dActionValue, uint uAction);
    // 지정 축의 예외 처리 기능에 대한 설정 상태를 반환한다.(MLII : Sigma-5, SIIIH : MR_J4_xxB 전용)
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetServoMonitor(int nAxisNo, uint uSelMon, ref double dpActionValue, ref uint upAction);
    // 지정 축의 예외 처리 기능에 대한 사용 유무를 설정한다.(MLII : Sigma-5, SIIIH : MR_J4_xxB 전용)
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetServoMonitorEnable(int nAxisNo, uint uEnable);
    // 지정 축의 예외 처리 기능에 대한 사용 유무를 반환한다.(MLII : Sigma-5, SIIIH : MR_J4_xxB 전용)
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetServoMonitorEnable(int nAxisNo, ref uint upEnable);

    // 지정 축의 예외 처리 기능 실행 결과 플래그 값을 반환한다. 함수 실행 후 자동 초기화.(MLII : Sigma-5, SIIIH : MR_J4_xxB 전용)
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadServoMonitorFlag(int nAxisNo, uint uSelMon, ref uint upMonitorFlag, ref double dpMonitorValue);
    // 지정 축의 예외 처리 기능을 위한 감시 정보를 반환한다.(MLII : Sigma-5, SIIIH : MR_J4_xxB 전용)
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadServoMonitorValue(int nAxisNo, uint uSelMon, ref double dpMonitorValue);
    // 지정 축의 부하율을 읽을 수 있도록 설정 합니다.(MLII : Sigma-5, SIIIH : MR_J4_xxB 전용)
    // (MLII, Sigma-5, dwSelMon : 0 ~ 2) ==
    //     [0] : Accumulated load ratio(%)
    //     [1] : Regenerative load ratio(%)
    //     [2] : Reference Torque load ratio
    //     [3] : Motor rotation speed (rpm)
    // (SIIIH, MR_J4_xxB, dwSelMon : 0 ~ 4) ==
    //     [0] : Assumed load inertia ratio(0.1times)
    //     [1] : Regeneration load factor(%)
    //     [2] : Effective load factor(%)
    //     [3] : Peak load factor(%)
    //     [4] : Current feedback(0.1%)
    //     [5] : Speed feedback(rpm)
    // (RTEX, A5Nx, A6Nx, dwSelMon : 0 ~ 4) ==
    //     [0] : Command Torque(0.1%)
    //     [1] : Regenerative load ratio(0.1%)
    //     [2] : Overload ratio(0.1%)
    //     [3] : Inertia ratio(%)
    //     [4] : Actual speed(rpm)
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetReadServoLoadRatio(int nAxisNo, uint dwSelMon);
     // 지정 축의 부하율을 반환한다.(MLII : Sigma-5, SIIIH : MR_J4_xxB 전용)
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadServoLoadRatio(int nAxisNo, ref double dpMonitorValue);

//======== PCI-R1604-RTEX 전용 함수==================================================================================
    // RTEX A4Nx 관련 Scale Coefficient를 설정한다.(RTEX, A4Nx 전용)
    [DllImport("AXL.dll")] public static extern uint AxmMotSetScaleCoeff(int nAxisNo, int nScaleCoeff);
    // RTEX A4Nx 관련 Scale Coefficient 를 확인한다.(RTEX, A4Nx 전용)
    [DllImport("AXL.dll")] public static extern uint AxmMotGetScaleCoeff(int nAxisNo, ref int npScaleCoeff);
    // 특정 Input 신호의 Edge를 검출하여 즉정지 또는 감속정지하는 함수.
    // lDetect Signal : edge 검출할 입력 신호 선택.
    // lDetectSignal  : PosEndLimit(0), NegEndLimit(1), HomeSensor(4), EncodZPhase(5), UniInput02(6), UniInput03(7)
    // Signal Edge    : 선택한 입력 신호의 edge 방향 선택 (rising or falling edge).
    //                  SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
    // 구동방향      : Vel값이 양수이면 CW, 음수이면 CCW.
    // SignalMethod  : 급정지 EMERGENCY_STOP(0), 감속정지 SLOWDOWN_STOP(1)
    // 주의사항: SignalMethod를 EMERGENCY_STOP(0)로 사용할경우 가감속이 무시되며 지정된 속도로 가속 급정지하게된다.
    //          PCI-Nx04를 사용할 경우 lDetectSignal이 PosEndLimit , NegEndLimit(0,1) 을 찾을경우 신호의레벨 Active 상태를 검출하게된다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveSignalSearchEx(int nAxisNo, double dVel, double dAccel, int nDetectSignal, int nSignalEdge, int nSignalMethod);
//-------------------------------------------------------------------------------------------------------------------

//======== PCI-R1604-MLII/SIIIH, PCIe-Rxx04-SIIIH 전용 함수 ==================================================================================
    // 설정한 절대 위치로 이동한다.
    // 속도 프로파일은 사라디꼴 전용으로 구동한다.
    // 펄스가 출력되는 시점에서 함수를 벗어난다.
    // 항상 위치 및 속도, 가감속도를 변경 가능하며, 반대방향 위치 변경 기능을 포함한다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveToAbsPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel);
    // 지정 축의 현재 구동 속도를 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadVelEx(int nAxisNo, ref double dpVel);
//-------------------------------------------------------------------------------------------------------------------

//========  PCI-R1604-SIIIH, PCIe-Rxx04-SIIIH 전용 함수 ==================================================================================
    // 지정 축의 전자 기어비를 설정한다. 설정 후 비 휘발성 메모리에 기억됩니다.
    // 초기 값(lNumerator : 4194304(2^22), lDenominator : 10000)
    // MR-J4-B는 전자 기어비를 설정할 수 없으며, 상위 제어기에서 아래의 함수를 사용하여 설정하여야 합니다.
    // 기존 펄스 입력 방식 서보 드라이버(MR-J4-A)의 파라미터 No.PA06, No.PA07에 해당.
    // ex1) 1 um를 제어 단위로 가정. 감속기 비율 : 1/1. Rotary motor를 장착한 Linear stage.
    // Encoder resulotion = 2^22
    // Ball screw pitch : 6 mm
    // ==> lNumerator = 2^22, lDenominator = 6000(6/0.001)
    // AxmMotSetMoveUnitPerPulse에서 Unit/Pulse = 1/1로 설정하였다면, 모든 함수의 위치 단위 : um, 속도 단위 : um/sec, 가감속도 단뒤 : um/sec^2이 된다.
    // AxmMotSetMoveUnitPerPulse에서 Unit/Pulse = 1/1000로 설정하였다면, 모든 함수의 위치 단위 : mm, 속도 단위 : mm/sec, 가감속도 단뒤 : mm/sec^2이 된다.
    // ex2) 0.01도 회전을 제어 단위로 가정. 감속기 비율 : 1/1. Rotary motor를 장착한 회전체 구조물.
    // Encoder resulotion = 2^22
    // 1 회전 : 360
    // ==> lNumerator = 2^22, lDenominator = 36000(360 / 0.01)
    // AxmMotSetMoveUnitPerPulse에서 Unit/Pulse = 1/1로 설정하였다면, 모든 함수의 위치 단위 : 0.01도, 속도 단위 : 0.01도/sec, 가감속도 단뒤 : 0.01도/sec^2이 된다.
    // AxmMotSetMoveUnitPerPulse에서 Unit/Pulse = 1/100로 설정하였다면, 모든 함수의 위치 단위 : 1도, 속도 단위 : 1도/sec, 가감속도 단뒤 : 1도/sec^2이 된다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetElectricGearRatio(int nAxisNo, int nNumerator, int nDenominator);
    // 지정 축의 전자 기어비 설정을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetElectricGearRatio(int nAxisNo, ref int npNumerator, ref int npDenominator);

    // 지정 축의 토크 리미트 값을 설정 합니다.
        // 정방향, 역방향 구동시의 토크 값을 제한하는 함수.
    // SSCNET : 설정 값은 1 ~ 3000(0.1% ~ 300.0%)까지 설정
    //          최대 토크의 0.1% 단위로 제어 함.
    // RTEX   : 설정 값은 1 ~ 500 (1% ~ 500 %)까지 설정
    //          최대 토크의 1% 단위로 제어 함.
    //          * Torque Limit 기능을 사용할 축의 Servo Drive Parameter Pr5.21(토크 한계 선택 설정)을 4로 변경 후 사용해야 한다.
    // ML-III : 설정 값은 0 ~ 800 (0% ~ 800%)까지 설정
    //          Rotary Servo 앰프 모드만 지원
    //          PCI-Rxx00-MLIII 제품만 지원
    //          단위는 1%로 제어 함.
    //          * PlusDirTorqueLimit(Forwared Torque Limit)는 Servo Drive Parameter Pn402 를 설정합니다.
    //          * MinusDirTorqueLimit(Reverse Torque Limit)는 Servo Drive Parameter Pn403 을 설정합니다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetTorqueLimit(int nAxisNo, double dbPluseDirTorqueLimit, double dbMinusDirTorqueLimit);

    // 지정 축의 토크 리미트 값을 확인 합니다.
    // 정방향, 역방향 구동시의 토크 값을 읽어 오는 함수.
    // 설정 값은 1 ~ 3000(0.1% ~ 300.0%)까지 설정
    // 최대 토크의 0.1% 단위로 제어 함.
    // ML-III : 설정 범위는 0 ~ 800 (0% ~ 800%)임.
    //          Rotary Servo 앰프 모드만 지원
    //          최대 토크의 단위는 1% 임.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetTorqueLimit(int nAxisNo, ref double dbpPluseDirTorqueLimit, ref double dbpMinusDirTorqueLimit);

    // 지정 축의 토크 리미트 값을 설정 합니다.(아래 표시된 제품만 기능 지원함)
    // ML-III : 설정 값은 0 ~ 800 (0% ~ 800%)까지 설정
    //          Liner Servo 앰프 모드만 지원(Only SGD7S, SGD7W)
    //          PCI-Rxx00-MLIII 제품만 지원
    //          단위는 1%로 제어 함.
    //          * PlusDirTorqueLimit(Forwared Torque Limit)는 Servo Drive Parameter Pn483 를 설정합니다.
    //          * MinusDirTorqueLimit(Reverse Torque Limit)는 Servo Drive Parameter Pn484 을 설정합니다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetTorqueLimitEx(int lAxisNo, double dbPlusDirTorqueLimit, double dbMinusDirTorqueLimit);

    // 지정 축의 토크 리미트 값을 확인 합니다.(아래 표시된 제품만 기능 지원함)
    // 정방향, 역방향 구동시의 토크 값을 읽어 오는 함수.
    // ML-III : 설정 범위는 0 ~ 800 (0% ~ 800%)임.
    //          Liner Servo 앰프 모드만 지원(Only SGD7S, SGD7W)
    //          최대 토크의 단위는 1% 임.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetTorqueLimitEx(int lAxisNo, ref double dbpPlusDirTorqueLimit, ref double dbpMinusDirTorqueLimit);

    // 지정 축의 토크 리미트 값을 설정 합니다.
    // 정방향, 역방향 구동시의 토크 값을 제한하는 함수.
    // 설정 값은 1 ~ 3000(0.1% ~ 300.0%)까지 설정
    // 최대 토크의 0.1% 단위로 제어 함.
    // dPosition : 토크 리미트 값을 변경할 위치 정보(해당 위치 정보와 대상 위치가 같은 이벤트 발생시 적용됨).
    // lTarget   : COMMAND(0), ACTUAL(1)
    [DllImport("AXL.dll")] public static extern uint AxmMotSetTorqueLimitAtPos(int lAxisNo, double dbPlusDirTorqueLimit, double dbMinusDirTorqueLimit, double dPosition, int lTarget);

    // 지정 축의 토크 리미트 값을 확인 합니다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetTorqueLimitAtPos(int lAxisNo, ref double dbpPlusDirTorqueLimit, ref double dbpMinusDirTorqueLimit, ref double dpPosition, ref int lpTarget);

    // 토크 리미트 기능을 Enable/Disable 한다. (PCI-R1604 RTEX 전용 함수)
    // PCI-R1604의 경우 토크 리미트 값을 설정하고 기능을 Enable 해야 토크 리미트 기능이 동작합니다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetTorqueLimitEnable(int nAxisNo, uint uUse);

    // 토크 리미트 기능의 사용유무를 확인 한다. (PCI-R1604 RTEX 전용 함수)
    [DllImport("AXL.dll")] public static extern uint AxmMotGetTorqueLimitEnable(int nAxisNo, ref uint upUse);

    // 지정 축의 AxmOverridePos에 대한 특수 기능 사용 유무를 설정한다.
    // dwUsage        : AxmOverridPos 적용 가능 위치 판단 기능 사용 유무.
    //                  DISABLE(0) : 특수 기능 사용하지 않음.
    //                  ENABLE(1) : AxmMoveStartPos 설정한 구동 중 위치 변경 가능 위치를 감속 거리의 lDecelPosRatio(%)을 기준으로 판단한다.
    // lDecelPosRatio : 감속 거리에 대한 %값.
    // dReserved      : 사용하지 않음.
    [DllImport("AXL.dll")] public static extern uint AxmOverridePosSetFunction(int nAxisNo, uint dwUsage, int nDecelPosRatio, double dReserved);
    // 지정 축의 AxmOverridePos에 대한 특수 기능 사용 유무를 확인 한다.
    [DllImport("AXL.dll")] public static extern uint AxmOverridePosGetFunction(int nAxisNo, ref uint dwpUsage, ref int npDecelPosRatio, ref double dpReserved);

    // 지정축의 특정 위치에서 설정한 디지털 출력 값을 제어한다.
    // lModuleNo   : 모듈 번호
    // lOffset     : 출력 접점에 대한 Offset 위치
    // uValue      : OFF(0)
    //             : OM(1)
    //             : Function Clear(0xFF)
    // dPosition   : DO 출력 값을 변경할 위치 정보(해당 위치 정보와 대상 위치가 같아지는 이벤트 발생시 실행됨).
    // lTarget     : COMMAND(0), ACTUAL(1)
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetWriteOutputBitAtPos(int lAxisNo, int lModuleNo, int lOffset, uint uValue, double dPosition, int lTarget);
    // 지정축의 특정 위치에서 설정한 디지털 출력 값 제어 설정을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetWriteOutputBitAtPos(int lAxisNo, ref int lpModuleNo, ref int lpOffset, ref uint upValue, ref double dpPosition, ref int lpTarget);

//-------------------------------------------------------------------------------------------------------------------

//======== PCI-R3200-MLIII 전용 함수==================================================================================
    // 잔류 진동 억제(VST) 특수 함수    
    // 사용전에 반드시 코디에 대해서 축을 할당을 해야하며, 코디 한개에 1개의 축만 맵핑을 해야한다.
    // 아래 함수 실행전에 반드시 Servo ON 상태에서 사용한다.
    // lCoordnate        : 입력 성형 적용 코디 번호를 입력한다. 각 보드별 첫번째 부터 10번째의 코디에 축을 할당해서 사용해야 한다.
    //                     MLIII 마스터 보드는 보드 번호를 기준으로 16 ~ 31까지 보드 별로 순차적으로 16씩 증가된다.
    //                     MLIII B/D 0 : 16 ~ 31
    //                     MLIII B/D 1 : 31 ~ 47
    // cISTSize          : 입력 성형 사용 주파수 개수에 대해서 입력한다. 1로 값을 고정해서 사용한다.
    // dbpFrequency,     : 10H ~ 500Hz
    //                     1차 주파수 부터 순서데로 입력한다.(저주파부터 고주파).
    // dbpDampingRatio   : 0.001 ~ 0.9
    // dwpImpulseCount   : 2 ~ 5
    [DllImport("AXL.dll")] public static extern uint AxmAdvVSTSetParameter(int nCoordinate, uint dwISTSize, double[] dbpFrequency, double[] dbpDampingRatio, ref uint dwpImpulseCount);
    [DllImport("AXL.dll")] public static extern uint AxmAdvVSTGetParameter(int nCoordinate, ref uint dwpISTSize, double[] dbpFrequency, double[] dbpDampingRatio, ref uint dwpImpulseCount);
    // lCoordnate        : 입력 성형 코디 번호를 입력한다.
    // dwISTEnable       : 입력 성형 사용 유무를 결정한다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvVSTSetEnabele(int nCoordinate, uint dwISTEnable);
    [DllImport("AXL.dll")] public static extern uint AxmAdvVSTGetEnabele(int nCoordinate, ref uint dwISTEnable);

//====일반 보간함수 =================================================================================================    
    // 직선 보간 한다.
    // 시작점과 종료점을 지정하여 다축 직선 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점과 종료점을 지정하여 직선 보간 구동하는 Queue에 저장함수가된다. 
    // 직선 프로파일 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvLineMove(int nCoordinate, double[] dPosition, double dMaxVelocity, double dStartVel, double dStopVel, double dMaxAccel, double dMaxDecel);
    // 지정된 좌표계에 시작점과 종료점을 지정하는 다축 직선 보간 오버라이드하는 함수이다.
    // 현재 진행중인 보간구동을 지정된 속도 및 위치로 직선 보간 오버라이드 하며, 다음 노드에 대한 직선 보간 구동 예약도 가능한다.
    // IOverrideMode = 0일 경우, 구동중인 보간이 직선, 원호 보간에 관계없이 현재 구동 노드에서 직선 보간으로 즉시 오버라이드 되고, 
    // IOverrideMode = 1이면 현재 구동 노드 다음의 노드부터 직선보간이 차례로 예약된다.
    // IOverrideMode = 1로 본 함수를 호출할때마다 최소 1개에서 최대 8개까지 오버라이드 큐에 증가되면서 자동적으로 예약이 되며, 예약 후 마지막에 IOverrideMode = 0으로 본 함수가 호출되면
    // 내부적으로 오버라이드 큐에 있는 예약 보간들이 연속보간 큐로 저장되고, 직선 오버라이드 구동과 이후의 예약된 연속보간이 순차적으로 실행된다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvOvrLineMove(int nCoordinate, double[] dPosition, double dMaxVelocity, double dStartVel, double dStopVel, double dMaxAccel, double dMaxDecel, int nOverrideMode);
    // 2축 원호보간 한다.
    // 시작점, 종료점과 중심점을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode, 와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dCenterPos = 중심점 X,Y 배열 , dEndPos = 종료점 X,Y 배열.
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmAdvCircleCenterMove(int nCoord, int[] nAxisNo, double[] dCenterPos, double[] dEndPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir);
    // 중간점, 종료점을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 중간점, 종료점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dMidPos = 중간점 X,Y 배열 , dEndPos = 종료점 X,Y 배열, lArcCircle = 아크(0), 원(1)
    [DllImport("AXL.dll")] public static extern uint AxmAdvCirclePointMove(int nCoord, int[] nAxisNo, double[] dMidPos, double[] dEndPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, int nArcCircle);
    // 시작점, 회전각도와 반지름을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dCenterPos = 중심점 X,Y 배열 , dAngle = 각도.
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmAdvCircleAngleMove(int nCoord, int[] nAxisNo, double[] dCenterPos, double dAngle, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir);
    // 시작점, 종료점과 반지름을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dRadius = 반지름, dEndPos = 종료점 X,Y 배열 , uShortDistance = 작은원(0), 큰원(1)
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmAdvCircleRadiusMove(int nCoord, int[] nAxisNo, double dRadius, double[] dEndPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance);
    // 지정된 좌표계에 시작점과 종료점을 지정하여 2축 원호 보간 오버라이드 구동한다.
    // 현재 진행중인 보간구동을 지정된 속도 및 위치로 원호 보간 오버라이드 하며, 다음 노드에 대한 원호 보간 구동 예약도 가능한다.
    // IOverrideMode = 0일 경우, 구동중인 보간이 직선, 원호 보간에 관계없이 현재 구동 노드에서 원호 보간으로 즉시 오버라이드 되고, 
    // IOverrideMode = 1이면 현재 구동 노드 다음의 노드부터 원호보간이 차례로 예약된다.
    // IOverrideMode = 1로 본 함수를 호출할때마다 최소 1개에서 최대 8개까지 오버라이드 큐에 증가되면서 자동적으로 예약이 되며, 예약 후 마지막에 IOverrideMode = 0으로 본 함수가 호출되면
    // 내부적으로 오버라이드 큐에 있는 예약 보간들이 연속보간 큐로 저장되고, 원호 오버라이드 구동과 이후의 예약된 연속보간이 순차적으로 실행된다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvOvrCircleRadiusMove(int nCoord, int[] nAxisNo, double dRadius, double[] dEndPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance, int nOverrideMode);

//======= 헬리컬 이동 ===============================================================================================
    // 주의사항 : Helix를 연속보간 사용시 Spline, 직선보간과 원호보간을 같이 사용할수없다.
    
    // 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 헬리컬 보간 구동하는 함수이다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 헬리컬 연속보간 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmAdvContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다)
    // dCenterPos = 중심점 X,Y  , dEndPos = 종료점 X,Y .
    // uCWDir DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향        
    [DllImport("AXL.dll")] public static extern uint AxmAdvHelixCenterMove(int nCoord, double dCenterXPos, double dCenterYPos, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir);
    // 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬 보간 구동하는 함수이다. 
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 중간점, 종료점을 지정하여 헬리컬연속 보간 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmAdvContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    // dMidPos = 중간점 X,Y  , dEndPos = 종료점 X,Y 
    [DllImport("AXL.dll")] public static extern uint AxmAdvHelixPointMove(int nCoord, double dMidXPos, double dMidYPos, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel);
    // 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 헬리컬 보간 구동하는 함수이다
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 헬리컬연속 보간 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmAdvContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    //dCenterPos = 중심점 X,Y  , dAngle = 각도.
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmAdvHelixAngleMove(int nCoord, double dCenterXPos, double dCenterYPos, double dAngle, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir);
    // 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬 보간 구동하는 함수이다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬연속 보간 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmAdvContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    // dRadius = 반지름, dEndPos = 종료점 X,Y  , uShortDistance = 작은원(0), 큰원(1)
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향   
    [DllImport("AXL.dll")] public static extern uint AxmAdvHelixRadiusMove(int nCoord, double dRadius, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance);
    // 지정된 좌표계에 시작점과 종료점을 지정하여 3축 헬리컬 보간 오버라이드 구동한다.
    // 현재 진행중인 보간구동을 지정된 속도 및 위치로 헬리컬 보간 오버라이드 하며, 다음 노드에 대한 헬리컬 보간 구동 예약도 가능한다.
    // IOverrideMode = 0일 경우, 구동중인 보간이 직선, 원호 보간에 관계없이 현재 구동 노드에서 헬리컬 보간으로 즉시 오버라이드 되고, 
    // IOverrideMode = 1이면 현재 구동 노드 다음의 노드부터 헬리컬 보간이 차례로 예약된다.
    // IOverrideMode = 1로 본 함수를 호출할때마다 최소 1개에서 최대 8개까지 오버라이드 큐에 증가되면서 자동적으로 예약이 되며, 예약 후 마지막에 IOverrideMode = 0으로 본 함수가 호출되면
    // 내부적으로 오버라이드 큐에 있는 예약 보간들이 연속보간 큐로 저장되고, 헬리컬 오버라이드 구동과 이후의 예약된 연속보간이 순차적으로 실행된다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvOvrHelixRadiusMove(int nCoord, double dRadius, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance, int nOverrideMode);

//====일반 보간함수 =================================================================================================
    // 직선 보간을 예약 구동한다.
    // 시작점과 종료점을 지정하여 다축 직선 보간을 예약 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점과 종료점을 지정하여 직선 보간 구동하는 Queue에 저장함수가된다. 
    // 직선 프로파일 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptLineMove(int nCoordinate, double[] dPosition, double dMaxVelocity, double dStartVel, double dStopVel, double dMaxAccel, double dMaxDecel, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 지정된 좌표계에 시작점과 종료점을 지정하는 다축 직선 보간 오버라이드를 예약 구동하는 함수이다.
    // 현재 진행중인 보간구동을 지정된 속도 및 위치로 직선 보간 오버라이드를 예약 구동 하며, 다음 노드에 대한 직선 보간 구동 예약이 가능한다.
    // IOverrideMode = 0일 경우, 구동중인 보간이 직선, 원호 보간에 관계없이 현재 구동 노드에서 직선 보간으로 즉시 오버라이드 되고, 
    // IOverrideMode = 1이면 현재 구동 노드 다음의 노드부터 직선보간이 차례로 예약된다.
    // IOverrideMode = 1로 본 함수를 호출할때마다 최소 1개에서 최대 8개까지 오버라이드 큐에 증가되면서 자동적으로 예약이 되며, 예약 후 마지막에 IOverrideMode = 0으로 본 함수가 호출되면
    // 내부적으로 오버라이드 큐에 있는 예약 보간들이 연속보간 큐로 저장되고, 직선 오버라이드 구동과 이후의 예약된 연속보간이 순차적으로 실행된다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptOvrLineMove(int nCoordinate, double[] dPosition, double dMaxVelocity, double dStartVel, double dStopVel, double dMaxAccel, double dMaxDecel, int nOverrideMode, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 2축 원호보간을 예약 구동한다.
    // 시작점, 종료점과 중심점을 지정하여 원호 보간을 예약 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode, 와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dCenterPos = 중심점 X,Y 배열 , dEndPos = 종료점 X,Y 배열.
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptCircleCenterMove(int nCoord, int[] nAxisNo, double[] dCenterPos, double[] dEndPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 중간점, 종료점을 지정하여 원호 보간을 예약 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 중간점, 종료점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dMidPos = 중간점 X,Y 배열 , dEndPos = 종료점 X,Y 배열, lArcCircle = 아크(0), 원(1)
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptCirclePointMove(int nCoord, int[] nAxisNo, double[] dMidPos, double[] dEndPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, int nArcCircle, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 시작점, 회전각도와 반지름을 지정하여 원호 보간을 예약 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dCenterPos = 중심점 X,Y 배열 , dAngle = 각도.
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptCircleAngleMove(int nCoord, int[] nAxisNo, double[] dCenterPos, double dAngle, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 시작점, 종료점과 반지름을 지정하여 원호 보간을 예약 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmAdvContiStart함수를 사용해서 시작한다.
    // lAxisNo = 두축 배열 , dRadius = 반지름, dEndPos = 종료점 X,Y 배열 , uShortDistance = 작은원(0), 큰원(1)
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptCircleRadiusMove(int nCoord, int[] nAxisNo, double dRadius, double[] dEndPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 지정된 좌표계에 시작점과 종료점을 지정하여 2축 원호 보간 오버라이드를 예약 구동한다.
    // 현재 진행중인 보간구동을 지정된 속도 및 위치로 원호 보간 오버라이드을 예약 구동하며, 다음 노드에 대한 원호 보간 구동 예약이 가능한다.
    // IOverrideMode = 0일 경우, 구동중인 보간이 직선, 원호 보간에 관계없이 현재 구동 노드에서 원호 보간으로 즉시 오버라이드 되고, 
    // IOverrideMode = 1이면 현재 구동 노드 다음의 노드부터 원호보간이 차례로 예약된다.
    // IOverrideMode = 1로 본 함수를 호출할때마다 최소 1개에서 최대 8개까지 오버라이드 큐에 증가되면서 자동적으로 예약이 되며, 예약 후 마지막에 IOverrideMode = 0으로 본 함수가 호출되면
    // 내부적으로 오버라이드 큐에 있는 예약 보간들이 연속보간 큐로 저장되고, 원호 오버라이드 구동과 이후의 예약된 연속보간이 순차적으로 실행된다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptOvrCircleRadiusMove(int nCoord, int[] nAxisNo, double dRadius, double[] dEndPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance, int nOverrideMode, uint dwScript, int nScirptAxisNo, double dScriptPos);

//======= 헬리컬 이동 ===============================================================================================
    // 주의사항 : Helix를 연속보간 사용시 Spline, 직선보간과 원호보간을 같이 사용할수없다.

    // 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 헬리컬 보간을 예약 구동하는 함수이다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 헬리컬 연속보간을 예약 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmAdvContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다)
    // dCenterPos = 중심점 X,Y  , dEndPos = 종료점 X,Y .
    // uCWDir DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향    
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptHelixCenterMove(int nCoord, double dCenterXPos, double dCenterYPos, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬 보간을 예약 구동하는 함수이다. 
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 중간점, 종료점을 지정하여 헬리컬연속 보간을 예약 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmAdvContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    // dMidPos = 중간점 X,Y  , dEndPos = 종료점 X,Y
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptHelixPointMove(int nCoord, double dMidXPos, double dMidYPos, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 헬리컬 보간을 예약 구동하는 함수이다
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 헬리컬연속 보간을 예약 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmAdvContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    //dCenterPos = 중심점 X,Y  , dAngle = 각도.
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptHelixAngleMove(int nCoord, double dCenterXPos, double dCenterYPos, double dAngle, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬 보간을 예약 구동하는 함수이다.
    // AxmAdvContiBeginNode, AxmAdvContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬연속 보간을 예약 구동하는 함수이다. 
    // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmAdvContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
    // dRadius = 반지름, dEndPos = 종료점 X,Y  , uShortDistance = 작은원(0), 큰원(1)
    // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향  
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptHelixRadiusMove(int nCoord, double dRadius, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance, uint dwScript, int nScirptAxisNo, double dScriptPos);
    // 지정된 좌표계에 시작점과 종료점을 지정하여 3축 헬리컬 보간 오버라이드를 예약 구동한다.
    // 현재 진행중인 보간구동을 지정된 속도 및 위치로 헬리컬 보간 오버라이드를 예약 구동한다.
    // IOverrideMode = 0일 경우, 구동중인 보간이 직선, 원호 보간에 관계없이 현재 구동 노드에서 헬리컬 보간으로 즉시 오버라이드 되고, 
    // IOverrideMode = 1이면 현재 구동 노드 다음의 노드부터 헬리컬 보간이 차례로 예약된다.
    // IOverrideMode = 1로 본 함수를 호출할때마다 최소 1개에서 최대 8개까지 오버라이드 큐에 증가되면서 자동적으로 예약이 되며, 예약 후 마지막에 IOverrideMode = 0으로 본 함수가 호출되면
    // 내부적으로 오버라이드 큐에 있는 예약 보간들이 연속보간 큐로 저장되고, 헬리컬 오버라이드 구동과 이후의 예약된 연속보간이 순차적으로 실행된다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvScriptOvrHelixRadiusMove(int nCoord, double dRadius, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dStartVel, double dStopVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance, int nOverrideMode, uint dwScript, int nScirptAxisNo, double dScriptPos);

//====연속 보간 함수 ================================================================================================
    // 지정된 좌표계에 연속 보간 구동 중 현재 구동중인 연속 보간 인덱스 번호를 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiGetNodeNum(int nCoordinate, ref int npNodeNum);
    // 지정된 좌표계에 설정한 연속 보간 구동 총 인덱스 갯수를 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiGetTotalNodeNum(int nCoordinate, ref int npNodeNum);
    // 지정된 좌표계에 보간 구동을 위한 내부 Queue에 저장되어 있는 보간 구동 개수를 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiReadIndex(int nCoordinate, ref int npQueueIndex);
    // 지정된 좌표계에 보간 구동을 위한 내부 Queue가 비어 있는지 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiReadFree(int nCoordinate, ref uint upQueueFree);
    // 지정된 좌표계에 연속 보간 구동을 위해 저장된 내부 Queue를 모두 삭제하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiWriteClear(int nCoordinate);
    // 지정된 좌표계에 연속 보간 오버라이드 구동을 위해 예약된 오버라이드용 큐를 모두 삭제하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvOvrContiWriteClear(int nCoordinate);
    // 연속 보간 시작 한다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiStart(int nCoord, uint dwProfileset, int nAngle); 
    // 연속 보간 정지 한다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiStop(int nCoordinate, double dDecel);
    //지정된 좌표계에 연속보간 축 맵핑을 설정한다.
    //(축맵핑 번호는 0 부터 시작))
    // 주의점:  축맵핑할때는 반드시 실제 축번호가 작은 숫자부터 큰숫자를 넣는다.
    //          가상축 맵핑 함수를 사용하였을 때 가상축번호를 실제 축번호가 작은 값 부터 lpAxesNo의 낮은 인텍스에 입력하여야 한다.
    //          가상축 맵핑 함수를 사용하였을 때 가상축번호에 해당하는 실제 축번호가 다른 값이라야 한다.
    //          같은 축을 다른 Coordinate에 중복 맵핑하지 말아야 한다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiSetAxisMap(int nCoord, int nSize, int[] npAxesNo);
    //지정된 좌표계에 연속보간 축 맵핑을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiGetAxisMap(int nCoord, ref int npSize, ref int npAxesNo);
    // 지정된 좌표계에 연속보간 축 절대/상대 모드를 설정한다.
    // (주의점 : 반드시 축맵핑 하고 사용가능)
    // 지정 축의 이동 거리 계산 모드를 설정한다.
    //uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
    //              POS_REL_MODE '1' - 상대 좌표계
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiSetAbsRelMode(int nCoord, uint uAbsRelMode);
    // 지정된 좌표계에 연속보간 축 절대/상대 모드를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiGetAbsRelMode(int nCoord, ref uint uAbsRelMode);
    // 지정된 좌표계에 연속 보간 구동 중인지 확인하는 함수이다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiIsMotion(int nCoordinate, ref uint upInMotion);
    // 지정된 좌표계에 연속보간에서 수행할 작업들의 등록을 시작한다. 이함수를 호출한후,
    // AxmAdvContiEndNode함수가 호출되기 전까지 수행되는 모든 모션작업은 실제 모션을 수행하는 것이 아니라 연속보간 모션으로 등록 되는 것이며,
    // AxmAdvContiStart 함수가 호출될 때 비로소 등록된모션이 실제로 수행된다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiBeginNode(int nCoord);
    // 지정된 좌표계에서 연속보간을 수행할 작업들의 등록을 종료한다.
    [DllImport("AXL.dll")] public static extern uint AxmAdvContiEndNode(int nCoord);
    
    // 지정한 다축을 설정한 감속도로 동기 감속 정지한다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveMultiStop(int nArraySize, int[] npAxesNo, double[] dMaxDecel);
    // 지정한 다축을 동기 급 정지한다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveMultiEStop(int nArraySize, int[] npAxesNo);
    // 지정한 다축을 동기 감속 정지한다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveMultiSStop(int nArraySize, int[] npAxesNo);

    // 지정한 축의 실제 구동 속도를 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadActVel(int nAxisNo, ref double dpVel);          //2010.10.11
    // 서보 타입 슬레이브 기기의 SVCMD_STAT 커맨드 값을 읽는다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadServoCmdStat(int nAxisNo, ref uint upStatus);
    // 서보 타입 슬레이브 기기의 SVCMD_CTRL 커맨드 값을 읽는다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadServoCmdCtrl(int nAxisNo, ref uint upStatus);
    
    // 겐트리 구동시 마스터 축과 슬레이브 축 간의 위치 차에 대한 설정된 오차 한계값을 반환한다.    
    [DllImport("AXL.dll")] public static extern uint AxmGantryGetMstToSlvOverDist(int nAxisNo, ref double dpPosition);
    // 겐트리 구동시 마스터 축과 슬레이브 축 간의 위치 차에 대한 오차 한계값을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmGantrySetMstToSlvOverDist(int nAxisNo, double dPosition);

    // 지정 축의 알람 신호의 코드 상태를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalReadServoAlarmCode(int nAxisNo, ref ushort upCodeStatus);
    
    // 서보 타입 슬레이브 기기의 좌표계 설정을 실시한다. (MLIII 전용)
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoCoordinatesSet(int nAxisNo, uint dwPosData, uint dwPos_sel, uint dwRefe);
    // 서보 타입 슬레이브 기기의 브레이크 작동 신호를 출력한다. (MLIII 전용)
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoBreakOn(int nAxisNo);
    // 서보 타입 슬레이브 기기의 브레이크 작동 신호를 해제한다. (MLIII 전용)
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoBreakOff(int nAxisNo);
    // 서보 타입 슬레이브 기기의 
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoConfig(int nAxisNo, uint dwCfMode);
    // 서보 타입 슬레이브 기기의 센서 종보 초기화를 요구한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSensOn(int nAxisNo);
    // 서보 타입 슬레이브 기기의 센서전원 OFF를 요구한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSensOff(int nAxisNo);
    // 서보 타입 슬레이브 기기의 SMON 커맨드를 실행한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSmon(int nAxisNo);
    // 서보 타입 슬레이브 기기의 모니터 정보나 입출력 신호의 상태를 읽는다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoGetSmon(int nAxisNo, ref uint pbParam);
    // 서보 타입 슬레이브 기기에 서보 ON을 요구한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSvOn(int nAxisNo);
    // 서보 타입 슬레이브 기기에 서보 OFF를 요구한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSvOff(int nAxisNo);
    // 서보 타입 슬레이브 기기가 지정된 보간 위치로 보간이동을 실시한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoInterpolate(int nAxisNo, uint dwTPOS, uint dwVFF, uint dwTFF, uint dwTLIM);
    // 서보 타입 슬레이브 기기가 지정한 위치로 위치이송을 실시한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoPosing(int nAxisNo, uint dwTPOS, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM);
    // 서보 타입 슬레이브 기기가 지정된 이동속도로 정속이송을 실시한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoFeed(int nAxisNo, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM);
    // 서보 타입 슬레이브 기기가 이송중 외부 위치결정 신호의 입력에 의해 위치이송을 실시한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoExFeed(int nAxisNo, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM, uint dwExSig1, uint dwExSig2);
    // 서보 타입 슬레이브 기기가 외부 위치결정 신호 입력에 의해 위치이송을 실시한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoExPosing(int nAxisNo, uint dwTPOS, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM, uint dwExSig1, uint dwExSig2);
    // 서보 타입 슬레이브 기기가 원점 복귀를 실시한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoZret(int nAxisNo, uint dwSPD, uint dwACCR, uint dwDECR, uint dwTLIM, uint dwExSig1, uint dwExSig2, uint bHomeDir, uint bHomeType);
    // 서보 타입 슬레이브 기기가 속도제어를 실시한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoVelctrl(int nAxisNo, uint dwTFF, uint dwVREF, uint dwACCR, uint dwDECR, uint dwTLIM);
    // 서보 타입 슬레이브 기기가 토크제어를 실시한다.    
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoTrqctrl(int nAxisNo, uint dwVLIM, int nTQREF);
    // bmode 0x00 : common parameters ram
    // bmode 0x01 : common parameters flash
    // bmode 0x10 : device parameters ram
    // bmode 0x11 : device parameters flash
    // 서보 타입 슬레이브 기기의 서보팩 특정 파라미터 설정값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoGetParameter(int nAxisNo, uint wNo, uint bSize, uint bMode, ref uint pbParam);
    // 서보 타입 슬레이브 기기의 서보팩 특정 파라미터 값을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetParameter(int nAxisNo, uint wNo, uint bSize, uint bMode, ref uint pbParam);
    // M3 서보팩에 Command Execution을 실행한다
    // dwSize는 사용할 변수 개수(예: 1)M3StatNop : AxmServoCmdExecution(m_lAxis, 0, NULL), 2)M3GetStationIdRd : AxmServoCmdExecution(m_lAxis, 3, *pbIdRd))
    // M3StationNop(int lNodeNum)                                                                                                   : bwSize = 0
    // M3GetStationIdRd(int lNodeNum, BYTE bIdCode, BYTE bOffset, BYTE bSize, BYTE *pbIdRd)                                         : bwSize = 3
    // M3ServoSetConfig(int lNodeNum, BYTE bMode)                                                                                   : bwSize = 1
    // M3SetStationAlarmClear(int lNodeNum, WORD wAlarmClrMod)                                                                      : bwSize = 1
    // M3ServoSyncSet(int lNodeNum)                                                                                                 : bwSize = 0
    // M3SetStationConnect(int lNodeNum, BYTE bVer, uByteComMod ubComMode, BYTE bComTime, BYTE bProfileType)                        : bwSize = 4
    // M3SetStationDisconnect(int lNodeNum)                                                                                         : bwSize = 0
    // M3ServoSmon(int lNodeNum)                                                                                                    : bwSize = 0
    // M3ServoSvOn(int lNodeNum)                                                                                                    : bwSize = 0
    // M3ServoSvOff(int lNodeNum)                                                                                                   : bwSize = 0
    // M3ServoInterpolate(int lNodeNum, LONG lTPOS, LONG lVFF, LONG lTFF)                                                           : bwSize = 3
    // M3ServoPosing(int lNodeNum, LONG lTPOS, LONG lSPD, LONG lACCR, LONG lDECR, LONG lTLIM)                                       : bwSize = 5
    // M3ServoFeed(int lNodeNum, LONG lSPD, LONG lACCR, LONG lDECR, LONG lTLIM)                                                     : bwSize = 4
    // M3ServoExFeed(int lNodeNum, LONG lSPD, LONG lACCR, LONG lDECR, LONG lTLIM, DWORD dwExSig1, DWORD dwExSig2)                   : bwSize = 6
    // M3ServoExPosing(int lNodeNum, LONG lTPOS, LONG lSPD, LONG lACCR, LONG lDECR, LONG lTLIM, DWORD dwExSig1, DWORD dwExSig2)     : bwSize = 7
    // M3ServoTrqctrl(int lNodeNum, LONG lVLIM, LONG lTQREF)                                                                        : bwSize = 2
    // M3ServoGetParameter(int lNodeNum, WORD wNo, BYTE bSize, BYTE bMode, BYTE *pbParam)                                           : bwSize = 3
    // M3ServoSetParameter(int lNodeNum, WORD wNo, BYTE bSize, BYTE bMode, BYTE *pbParam)                                           : bwSize = 7
    [DllImport("AXL.dll")] public static extern uint AxmServoCmdExecution(int lAxisNo, uint dwCommand, uint dwSize, ref uint pdwExcData);
    // 서보 타입 슬레이브 기기의 설정된 토크 제한 값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoGetTorqLimit(int nAxisNo, ref uint dwpTorqLimit);
    // 서보 타입 슬레이브 기기에 토크 제한 값을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetTorqLimit(int nAxisNo, uint dwTorqLimit);

    // 서보 타입 슬레이브 기기에 설정된 SVCMD_IO 커맨드 값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoGetSendSvCmdIOOutput(int nAxisNo, ref uint dwData);
    // 서보 타입 슬레이브 기기에 SVCMD_IO 커맨드 값을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetSendSvCmdIOOutput(int nAxisNo, uint dwData);
  
    // 서보 타입 슬레이브 기기에 설정된 SVCMD_CTRL 커맨드 값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoGetSvCmdCtrl(int nAxisNo, ref uint dwData);
    // 서보 타입 슬레이브 기기에 SVCMD_CTRL 커맨드 값을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetSvCmdCtrl(int nAxisNo, uint dwData);

    // MLIII adjustment operation을 설정 한다.
    // dwReqCode == 0x1005 : parameter initialization : 20sec
    // dwReqCode == 0x1008 : absolute encoder reset   : 5sec
    // dwReqCode == 0x100E : automatic offset adjustment of motor current detection signals  : 5sec
    // dwReqCode == 0x1013 : Multiturn limit setting  : 5sec
    [DllImport("AXL.dll")] public static extern uint AxmM3AdjustmentOperation(int nAxisNo, uint dwReqCode);
    // 서보 축 추가 모니터링 채널별 선택 값을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetMonSel(int nAxisNo, uint dwMon0, uint dwMon1, uint dwMon2);
    // 서보 축 추가 모니터링 채널별 설정 값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoGetMonSel(int nAxisNo, ref uint upMon0, ref uint upMon1, ref uint upMon2);
    // 서보 축 추가 모니터링 채널별 설정 값을 기준으로 현재 상태를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoReadMonData(int nAxisNo, uint dwMonSel, ref uint dwpMonData);
    // 제어할 토크 축 설정
    [DllImport("AXL.dll")] public static extern uint AxmAdvTorqueContiSetAxisMap(int nCoord, int nSize, int[] npAxesNo, uint dwTLIM, uint dwConMode);
    // 2014.04.28
    // 토크 프로파일 설정 파라미터
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetTorqProfile(int nCoord, int nAxisNo, int TorqueSign, uint dwVLIM, uint dwProfileMode, uint dwStdTorq, uint dwStopTorq);
    // 토크 프로파일 확인 파라미터
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoGetTorqProfile(int nCoord, int nAxisNo, ref int npTorqueSign, ref uint updwVLIM, ref uint upProfileMode, ref uint upStdTorq, ref uint upStopTorq);
//-------------------------------------------------------------------------------------------------------------------

    //======== EtherCAT 전용 함수 =======================================================================================
    // Inposition 신호의 Range를 설정한다. (dInposRange > 0)
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetInposRange(int nAxisNo, double dInposRange);
    // Inposition 신호의 Range를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetInposRange(int nAxisNo, ref double dpInposRange);    
	
    // 절대 엔코더 위치를 dPositiond로 지정한다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetEncClear(int nAxisNo, double dPosition);
    
    // 지정 축에 해당하는 서보팩의 지령 위치와 실체 위치를 dPos 값으로 일치 시킨다.
    // Reserved
    //[DllImport("AXL.dll")] public static extern uint AxmStatusSetMotorPosMatch(int nAxisNo);
    
    // Work 좌표계를 설정한다. 디폴트값은 World Coordinate와 일치한다.
    // lCoordNo : 좌표계번호(0~7)
    // dpOrigin : World Coordinate를 기준으로하는 Work 좌표계 원점위치
    // dpXPos   : World Coordinate를 기준으로하는 Work 좌표계 X축상의 임의의 위치
    // dpYPos   : World Coordinate를 기준으로하는 Work 좌표계 Y축상의 임의의 위치
    [DllImport("AXL.dll")] public static extern uint AxmMotSetWorkCoordinate(int nCoordNo, ref double dpOrigin, ref double dpXPos, ref double dpYPos);
    // 설정된 Work 좌표계를 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetWorkCoordinate(int nCoordNo, ref double dpOrigin, ref double dpXPos, ref double dpYPos);
    // 오버라이드 할 때 위치속성(절대/상대)을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetOverridePosMode(int nAxisNo, uint dwAbsRelMode);
    // 오버라이드 할 때 위치속성(절대/상대)을 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetOverridePosMode(int nAxisNo, ref uint dwpAbsRelMode);
    // LineMove 오버라이드할때 위치속성(절대/상대)을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetOverrideLinePosMode(int nCoordNo, uint dwAbsRelMode);
    // LineMove 오버라이드할때 위치속성(절대/상대)을 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetOverrideLinePosMode(int nCoordNo, ref uint dwpAbsRelMode);
    
    // 지정된 축의 Vibration Control 설정값(주파수, 댐핑계수, 임펄스 개수)을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetVibrationControl(int nAxisNo, int nSize, ref double pdFrequency, ref double pdDampingRatio, ref int npImpulseCount);
    
    // 지정된 축의 Vibration Control을 Enable or Disable 한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotVibrationControlEnable(int nAxisNo, uint dwEnable);
    
    // 지정된 축의 Vibration Control이 Enable 되어있는지 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmMotVibrationControlIsEnable(int nAxisNo, ref uint pdwEnable);
    
    // 지정된 좌표계의 Vibration Control을 Enable or Disable 한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotVibrationContronCoordEnable(int nCoordNo, uint dwEnable);
    
    // 지정된 좌표계에 Vibration Control이 Enable 되어있는지 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmMotVibrationContronCoordIsEnable(int nCoordNo, ref uint pdwEnable);
    
    // 지정된 축의 Position 범위 마다 TorqueLimit을 설정한다.
    // Position 개수는 lSize, TorqueLimit 개수는 lSize - 1
    // Position은 오름차순으로 입력되어야만 한다
    // EtherCAT은 PlusTorqueLimit만으로 양쪽 다 같은 TorqueLimit을 건다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetMultiTorqueLimit(int nAxisNo, int nSize, double[] dpPosition, double[] dpPlusTorqueLimit, double[] dpMinusTorqueLimit, int nTarget);

    // 지정된 축의 Multi TorqueLimit을 Enable or Disable 한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotMultiTorqueLimitEnable(int nAxisNo, uint dwEnable);

    // 지정된 축의 Multi TorqueLimit이 Enable 되어있는지 읽어온다. 
    [DllImport("AXL.dll")] public static extern uint AxmMotMultiTorqueLimitIsEnable(int nAxisNo, ref uint dwpEnable);

    // AxmMoveStartPos와 동일하며 종료속도(EndVel)가 추가되었다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartPosEx(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, double dEndVel);
    // AxmMovePos와 동일하며 종료속도(EndVel)가 추가되었다.
    [DllImport("AXL.dll")] public static extern uint AxmMovePosEx(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, double dEndVel);
    // AxmMoveStartMultiPos와 동일하며 종료속도(EndVel)가 추가되었다.
    // EndVel 관련 Ex함수는 사용하지 않음.
    //[DllImport("AXL.dll")] public static extern uint AxmMoveStartMultiPosEx(int nArraySize, ref int npAxisNo, ref double dpPos, ref double dpVel, ref double dpAccel, ref double dpDecel, ref double dpEndVel);
    // AxmMoveMultiPos와 동일하며 종료속도(EndVel)가 추가되었다.
    // Reserved
    //[DllImport("AXL.dll")] public static extern uint AxmMoveMultiPosEx(int nArraySize, ref int npAxisNo, ref double dpPos, ref double dpVel, ref double dpAccel, ref double dpDecel, ref double dpEndVel);
    
    // Coordinate Motion을 경로상에서 감속정지(dDecel) 시킨다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveCoordStop(int nCoordNo, double dDecel);
    // Coordinate Motion을 급정지 시킨다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveCoordEStop(int nCoordNo);
    // Coordinate Motion을 경로상에서 감속정지 시킨다.
    [DllImport("AXL.dll")] public static extern uint AxmMoveCoordSStop(int nCoordNo);
    
    // AxmMoveStartPosWithPosEvent와 동일하며 종료속도(EndVel)가 추가되었다.
    // EndVel 관련 Ex함수는 사용하지 않음.
    //[DllImport("AXL.dll")] public static extern uint AxmMoveStartPosWithPosEventEx(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, int nEventAxisNo, double dComparePos, uint uPosSource, double dEndVel);
    
    // AxmLineMove모션의 위치를 오버라이드 시킨다.
    [DllImport("AXL.dll")] public static extern uint AxmOverrideLinePos(int nCoordNo, ref double dpOverridePos);
    // AxmLineMove모션의 속도를 오버라이드 시킨다. 각축들의 속도비율은 dpDistance로 결정된다.
    [DllImport("AXL.dll")] public static extern uint AxmOverrideLineVel(int nCoordNo, double dOverrideVel, ref double dpDistance);
    
    // AxmLineMove모션의 속도를 오버라이드 시킨다.
    // dMaxAccel  : 오버라이드 가속도
    // dMaxDecel  : 오버라이드 감속도
    // dpDistance : 각축의 속도 비율
    [DllImport("AXL.dll")] public static extern uint AxmOverrideLineAccelVelDecel(int nCoordNo, double dOverrideVelocity, double dMaxAccel, double dMaxDecel, ref double dpDistance);
    // AxmOverrideVelAtPos에 추가적으로 AccelDecel을 오버라이드 시킨다.
    [DllImport("AXL.dll")] public static extern uint AxmOverrideAccelVelDecelAtPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, double dOverridePos, double dOverrideVel, double dOverrideAccel, double dOverrideDecel, int nTarget);
    
    // 하나의 마스터축에 다수의 슬레이브축들을 연결한다(Electronic Gearing).
    // lMasterAxisNo : 마스터축
    // lSize : 슬레이브축 개수(최대 8)
    // lpSlaveAxisNo : 슬레이브축 번호
    // dpGearRatio : 마스터축을 기준으로하는 슬레이브축 비율(0%만 제외)
    [DllImport("AXL.dll")] public static extern uint AxmEGearSet(int nMasterAxisNo, int nSize, int[] npSlaveAxisNo, double[] dpGearRatio);
    // Electronic Gearing정보를 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmEGearGet(int nMasterAxisNo, ref int npSize, int[] npSlaveAxisNo, double[] dpGearRatio);
    // Electronic Gearing을 해제한다.
    [DllImport("AXL.dll")] public static extern uint AxmEGearReset(int nMasterAxisNo);
    // Electronic Gearing을 활성/비활성한다.
    [DllImport("AXL.dll")] public static extern uint AxmEGearEnable(int nMasterAxisNo, uint dwEnable);
    // Electronic Gearing을 활성/비활성상태를 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmEGearIsEnable(int nMasterAxisNo, uint dwEnable);
    
    // AxmLineMove와 동일하며 종료속도(EndVel)가 추가되었다.
    // EndVel관련 Ex함수는 사용하지 않음
    //[DllImport("AXL.dll")] public static extern uint AxmLineMoveEx(int nCoordNo, ref double dpEndPos, double dVel, double dAccel, double dDecel, double dEndVel);
    
    // AxmCirclePointMove와 사용법은 동일하며 3차원 원/원호 보간을 지원한다.
    // 축 매핑은 3축이상 가능하며 3축 이상의 축들은 Linear Interpolation된다.
    // dEndVel : 종료속도
    //[DllImport("AXL.dll")] public static extern uint AxmCirclePointMoveEx(int nCoordNo, ref int nAxisNo, ref double dpMidPos, ref double dpEndPos, double dVel, double dAccel, double dDecel, int nArcCircle, int nArraySize);
    
    // 임의의 축을 중심으로 회전하는 헬리컬 보간 구동을 지원한다.    
    // dpFirstCenterPos=중심위치, dpSecondCenterPos=반대편 중심위치, dPicth=이동량(mm)/1Revolution, dTraverseDistance=이동량(mm)
    // dpFirstCenterPos에서 dpSecondCenterPos를 잇는 직선이 회전축이 된다. 중심직선(dpFirstCenterPos-->dpSecondCenterPos)와 시작위치까지의 직선(dpFirstCenterPos-->시작위치)는 서로 수직이다.
    // dTraverseDistance은 시작위치에서 회전축과 평행한 직선의 거리이다.
    // 축 매핑은 3축이상 가능하며 3축이상의 축들은 Linear Interpolation된다.
    [DllImport("AXL.dll")] public static extern uint AxmHelixPitchMove(int nCoordNo, ref double dpFirstCenterPos, ref double dpSecondCenterPos, double dPitch, double dTraverseDistance, double dVel, double dAccel, double dDecel, uint uCWDir);
    // AxmHelixPitchMove와 동일하며 종료속도(EndVel)가 추가되었다.
    // EndVel 관련 Ex함수는 사용하지 않음.
    //[DllImport("AXL.dll")] public static extern uint AxmHelixPitchMoveEx(int nCoordNo, ref double dpFirstCenterPos, ref double dpSecondCenterPos, double dPitch, double dTraverseDistance, double dVel, double dAccel, double dDecel, uint uCWDir, double dEndVel);
    
    // 1차원 보정함수 
    // 보정 테이블 범위를 넘어서 구동할경우 : 보상 테이블의 맨처음과 맨마지막의 보정값은 항상 "0"으로 설정. 즉 pdMotorPosition과 pdLoadPosition이 동일하여야 한다.        
    // 보정값 측정축(lSourceAxis)과 보정값 적용축(lTargetAxis)에 같은 축, 혹은 다른 축을 설정할수있다.    
    // 보정 테이블의 위치값은 절대위치로 설정
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimSet(int nTableNo, int nSourceAxis, int nTargetAxis, int nSize, double[] dpMotorPosition, double[] dpLoadPosition);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimSet(int nTableNo, int nSourceAxis, int nTargetAxis, int nSize, double[] dpMotorPosition, double[] dpLoadPosition, uint dwInterpolationMethod);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimSet(int nTableNo, int nSourceAxis, int nTargetAxis, int nSize, double[] dpMotorPosition, double[] dpLoadPosition, uint dwInterpolationMethod, uint dwUseReverse);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimSet(int nTableNo, int nSourceAxis, int nTargetAxis, int nSize, double[] dpMotorPosition, double[] dpLoadPosition, uint dwInterpolationMethod, uint dwUseReverse, double[] dpLoadReversePosition);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimGet(int nTableNo, ref int npSourceAxis, ref int npTargetAxis, ref int npSize, double[] dpMotorPosition, double[] dpLoadPosition);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimReset(int nTableNo);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimIsSet(int lTableNo, ref uint dwpSet);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimEnable(int nTableNo, uint dwEnable);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimIsEnable(int nTableNo, ref uint dwpEnable);
    
    // 현재 지령 위치에서의 보정값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimGetCorrection(int nTableNo, ref double pdCorrection);

    // 역방향 구동 보정 테이블을 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimSetReverse(int lTableNo, ref double dpLoadPosition);
    // 역방향 구동 보정 테이블을 확인한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimGetReverse(int lTableNo, ref double dpLoadPosition);
    // 현재 지령 위치에서의 역방향 구동 보정값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimGetCorrectionReverse(int lTableNo, ref double pdCorrection);

    // 1차원 보정 테이블을 File을 참조하여 설정한다. 파일 확장자는 cmp1이다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimFileLoad(int nTableNo, int nSourceAxis, int nTargetAxis, string szFilePath);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimFileLoad(int nTableNo, int nSourceAxis, int nTargetAxis, string szFilePath, uint dwInterpolationMethod);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimFileLoad(int nTableNo, int nSourceAxis, int nTargetAxis, char[] szFilePath);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimFileLoad(int nTableNo, int nSourceAxis, int nTargetAxis, char[] szFilePath, uint dwInterpolationMethod);
    // 1차원 보정 테이블을 File로 저장한다. 파일 확장자는 cmp1이다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimFileSave(int nTableNo, string szFilePath);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimFileSave(int nTableNo, char[] szFilePath);

    // 1차원 보정 테이블의 보간 방법을 설정한다.
    //      dwMethod : 설정할 보간 방법
    //          INTERPOLATION_LINEAR(0) - Linear interpolation
    //          INTERPOLATION_CUBIC_SPLINE(1) - Cubic spline interpolation
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimSetInterpolationMethod(int lTableNo, uint dwMethod);
    // 설정된 1차원 보정 테이블의 보간 방법을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationOneDimGetInterpolationMethod(int lTableNo, ref uint dwpMethod);

    // 2차원 보정함수 
    // 보정 테이블 범위를 넘어서 구동할경우 : 보상 테이블의 테두리 보정값은 항상 "0"으로 설정. 즉 pdMotorPosition과 pdLoadPosition이 동일하다.
    // pdMotorPosition1는 lAxis[0]의 모터위치,  pdMotorPosition2는 lAxis[1]의 모터위치이다.
    // pdMotorPosition1의 사이즈는 lPositionSize1, pdMotorPosition2의 사이즈는 lPositionSize2
    // 보정값 측정축(lSourceAxis)과 보정값 적용축(lTargetAxis)에 같은 축, 혹은 다른 축을 설정할수있다.    
    // 보정 테이블의 위치값은 절대위치로 설정
    // Reserved
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimSet(int lTableNo, int lSourceAxis1, int lSourceAxis2, int lTargetAxis1, int lTargetAxis2, int lSize1, int lSize2, double[] dpMotorPosition1, double[] dpMotorPosition2, double[] dpLoadPosition1, double[] dpLoadPosition2);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimGet(int lTableNo, ref int lpSourceAxis1, ref int lpSourceAxis2, ref int lpTargetAxis1, ref int lpTargetAxis2, ref int lpSize1, ref int lpSize2, double[] dpMotorPosition1, double[] dpMotorPosition2, double[] dpLoadPosition1, double[] dpLoadPosition2);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimReset(int lTableNo);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimIsSet(int lTableNo, ref uint dwpSet);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimEnable(int lTableNo, uint dwEnable);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimIsEnable(int lTableNo, ref uint dwpEnable);

    // 현재 지령 위치에서의 보정값을 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimGetCorrection(int lTableNo, ref double pdCorrection);

    // 2차원 보정 테이블을 File을 참조하여 설정한다. 파일 확장자는 cmp2이다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimFileLoad(int nTableNo, int nSourceAxis1, int nSourceAxis2, int nTargetAxis1, int nTargetAxis2, string szFilePath);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimFileLoad(int nTableNo, int nSourceAxis1, int nSourceAxis2, int nTargetAxis1, int nTargetAxis2, char[] szFilePath);
    // 2차원 보정 테이블을 File로 저장한다. 파일 확장자는 cmp2이다.
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimFileSave(int nTableNo, string szFilePath);
    [DllImport("AXL.dll")] public static extern uint AxmCompensationTwoDimFileSave(int nTableNo, char[] szFilePath);
    
    //// Reserved
    //[DllImport("AXL.dll")] public static extern uint AxmBlendingSet(int nAxisNo, double dValue, uint dwMethod);
    //[DllImport("AXL.dll")] public static extern uint AxmBlendingGet(int nAxisNo, ref double dpValue, ref uint dwpMethod);
    //[DllImport("AXL.dll")] public static extern uint AxmBlendingReset(int nAxisNo);
    //[DllImport("AXL.dll")] public static extern uint AxmBlendingEnable(int nAxisNo, uint dwEnable);
    //[DllImport("AXL.dll")] public static extern uint AxmBlendingIsEnable(int nAxisNo, ref uint dwpEnable);
    
    // Blending 모션을 지원한다.
    // dValue   : 첫번째 모션 세그먼트의 종료시점을 기준으로하는 블렌딩이 시작되는 시점
    // dwMethod : 0=거리(mm), 1=시간(msec), 2=거리비율(%)
    // 자동가감속, 자동원호삽입 모드에서는Blending 모드가 해제된다.
    [DllImport("AXL.dll")] public static extern uint AxmBlendingCoordSet(int nCoordNo, double dValue, uint dwMethod);
    [DllImport("AXL.dll")] public static extern uint AxmBlendingCoordGet(int nCoordNo, ref double dpValue, ref uint dwpMethod);
    [DllImport("AXL.dll")] public static extern uint AxmBlendingCoordReset(int nCoordNo);
    [DllImport("AXL.dll")] public static extern uint AxmBlendingCoordEnable(int nCoordNo, uint dwEnable);
    [DllImport("AXL.dll")] public static extern uint AxmBlendingCoordIsEnable(int nCoordNo, ref uint dwpEnable);
    
    // 지정 축에서 설정된 신호를 검출하고 그 위치를 저장하기 위해 이동하는 함수이다.
    // 원하는 신호를 골라 찾아 움직이는 함수 찾을 경우 그 위치를 저장시켜놓고 AxmGetCapturePos사용하여 그값을 읽는다.
    // Signal Edge   : 선택한 입력 신호의 edge 방향 선택 (rising or falling edge).
    //                 SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
    // 구동방향      : Vel값이 양수이면 CW, 음수이면 CCW.
    // dPos             : 지정 위치까지 구동하면서 Signal Pos를 저장한다. (Max 256)
    // lDetect Signal: edge 검출할 입력 신호 선택.SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
    // lDetectSignal : PosEndLimit(0), NegEndLimit(1), HomeSensor(4), EncodZPhase(5), UniInput02(6), UniInput03(7)
    // lTarget       : COMMAND(0), ACTUAL(1)
    // 주의사항: SignalMethod를 EMERGENCY_STOP(0)로 사용할경우 가감속이 무시되며 지정된 속도로 가속 급정지하게된다.
    //           PCI-Nx04를 사용할 경우 lDetectSignal이 PosEndLimit , NegEndLimit(0,1) 을 찾을경우 신호의레벨 Active 상태를 검출하게된다.
    // Reserved
    //[DllImport("AXL.dll")] public static extern uint AxmMoveMultiSignalCapture(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, int nDetectSignal, int nSignalEdge, int nTarget);
    // 'AxmMoveSignalCapture' 함수에서 저장된 위치값을 확인하는 함수이다.
    // 주의사항: 함수 실행 결과가 "AXT_RT_SUCCESS"일때 저장된 위치가 유효하며, 이 함수를 한번 실행하면 저장 위치값이 초기화된다.
    // Reserved
    //[DllImport("AXL.dll")] public static extern uint AxmMoveGetMultiCapturePos(int nAxisNo, ref double dpCapPosition, ref int npSize);
    
    // 지정한 축의 서보팩 AlarmCode를 읽어오는 함수.
    // upAlarmCode      : 해당 서보팩의 Alarm Code참조
    // uReturnMode      : 함수의 반환 동작조건을 설정
    // EtherCAT 경우에 uReturnMode는 사용하지 않음.    
    //DWORD    __stdcall AxmStatusReadServoAlarm(long lAxisNo, DWORD uReturnMode, DWORD *upAlarmCode);

    // 지정한 에러코드에 해당하는 Alarm String을 받아오는 함수
    //DWORD    __stdcall AxmStatusGetServoAlarmString(long lAxisNo, DWORD uAlarmCode, long lAlarmStringSize, char *szAlarmString);

    // 지정한 축의 서보팩 AlarmCode를 읽어오는 함수.
    // upAlarmCode      : 해당 서보팩의 Alarm Code참조
    // uReturnMode      : 함수의 반환 동작조건을 설정
    // EtherCAT 경우에 uReturnMode는 사용하지 않음.    
    //DWORD    __stdcall AxmStatusReadServoAlarm(long lAxisNo, DWORD uReturnMode, DWORD *upAlarmCode);

    //해당 축의 Torque값을 읽어온다.
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadTorque(int nAxisNo, ref double pdTorque);

    // 지정한 에러코드에 해당하는 Alarm String을 받아오는 함수
    //DWORD    __stdcall AxmStatusGetServoAlarmString(long lAxisNo, DWORD uAlarmCode, long lAlarmStringSize, char *szAlarmString);
    
    // 주의사항: 입력한 종료속도가 '0'미만이면 '0'으로, 'AxmMotSetMaxVel'로 설정한 최대속도를 초과하면 'MaxVel'로 재설정된다. 
    // 지정 축에 종료속도를 설정한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetEndVel(int nAxisNo, double dEndVelocity);
    
    // 지정 축의 종료속도를 반환한다.
    [DllImport("AXL.dll")] public static extern uint AxmMotGetEndVel(int nAxisNo, ref double dpEndVelocity);


    // 직선 보간 한다.
    // 시작점과 종료점을 지정하여 다축 직선 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점과 종료점을 지정하여 직선 보간 구동하는 Queue에 저장함수가된다. 
    // 직선 프로파일 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // lpAxisNo의 축 배열에 해당하는 축들은 직선 보간을 하며, 나머지 Coordinate의 축들은 직선 보간 비율에 맞게 단축 구동을 수행한다.
    [DllImport("AXL.dll")] public static extern uint AxmLineMoveWithAxes(int nCoord, int nArraySize, ref int npAxisNo, ref double dpEndPos, double dVel, double dAccel, double dDecel);

    // 2차원/3차원 원호보간 및 그 이상의 축에 대해서 직선보간을 한다.
    // 시작점, 종료점과 중심점을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode, 와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // 2차원/3차원 이상의 축에 대해서는 직선보간을 할 때 dEndPosition의 값을 Targetposition으로 사용한다.
    // lArraySize       : 원호보간을 할 축의 갯수(2 or 3)
    // lpAxisNo         : 원호 보간을 할 축 배열
    // dCenterPosition  : 원호보간에 사용할 중심점 배열, AxmContiSetAxisMap에서 맵핑한 축 번호 순서에 맞는 배열의 위치에 값을 입력한다.
    // dEndPos          : 원호보간에 사용할 종료점 배열, AxmContiSetAxisMap에서 맵핑한 축 번호 순서에 맞는 배열의 위치에 값을 입력한다.
    //   Ex) AxmContiSetAxisMap(4, [0, 1, 2, 3]맵핑, 2축 3축 원호보간, 시작위치 (50.0, 100), 중심점을 (50.0, 100.0), 종료점 (150.0, 100.0), 0축, 1축 (300.0, 400.0)
    //       dCenterPosition[4] = { 0.0, 0.0, 50.0, 100.0 }, dEndPos[4] = { 300.0, 400.0, 150.0, 100.0 }
    // uCWDir           : DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    // dw3DCircle(0)    : 2차원 원호보간 및 그 이상의 축에 대해서 직선보간
    // dw3DCircle(1)    : 3차원 원호보간 및 그 이상의 축에 대해서 직선보간
    [DllImport("AXL.dll")] public static extern uint AxmCircleCenterMoveWithAxes(int nCoord, int nArraySize, ref int npAxisNo, ref double dCenterPosition, ref double dEndPosition, double dMaxVelocity, double dMaxAccel, double dMaxDecel, uint uCWDir, bool b3DCircle);
    
    // 시작점, 종료점과 반지름을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // lArraySize       : 원호보간을 할 축의 갯수(2 or 3)
    // lpAxisNo         : 원호 보간을 할 축 배열
    // dRadius          : 원의 반지름
    // dEndPos          : 원호보간시 종료위치 배열, AxmContiSetAxisMap에서 맵핑한 축 번호 순서에 맞는 배열의 위치에 값을 입력한다.
    // uCWDir           : DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    // uShortDistance   : 작은원(0), 큰원(1)
    [DllImport("AXL.dll")] public static extern uint AxmCircleRadiusMoveWithAxes(int nCoord, int nArraySize, ref int npAxisNo, double dRadius, ref double dEndPos, double dVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance);
    
    // 시작점, 회전각도와 반지름을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // lArraySize       : 원호보간을 할 축의 갯수(2 or 3)
    // npAxisNo         : 원호 보간을 할 축 배열
    // dpCenterPos       : 원호보간에 사용할 중심점 배열, AxmContiSetAxisMap에서 맵핑한 축 번호 순서에 맞는 배열의 위치에 값을 입력한다
    // dAngle           : 각도.
    // uCWDir           : DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    [DllImport("AXL.dll")] public static extern uint AxmCircleAngleMoveWithAxes(int nCoord, int nArraySize, ref int npAxisNo, ref double dpCenterPos, double dAngle, double dVel, double dAccel, double dDecel, uint uCWDir);
    
    // 2차원/3차원 원호보간 및 그 이상의 축에 대해서 직선보간을 한다.
    // 시작위치에서 중간점, 종료점을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode, 와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // 2차원/3차원 이상의 축에 대해서는 직선보간을 할 때 dEndPosition의 값을 Targetposition으로 사용한다.
    // lArraySize       : 원호보간을 할 축의 갯수(2 or 3)
    // npAxisNo         : 원호 보간을 할 축 배열
    // dpMidPos          : 원호보간에 사용할 중간점 배열, AxmContiSetAxisMap에서 맵핑한 축 번호 순서에 맞는 배열의 위치에 입력한다.
    // dpEndPos          : 원호보간에 사용할 종료점 배열, AxmContiSetAxisMap에서 맵핑한 축 번호 순서에 맞는 배열의 위치에 입력한다.
    //   Ex) AxmContiSetAxisMap(4, [0, 1, 2, 3]맵핑, 2축 3축 원호보간, 시작위치 (0.0, 100), 중간점을 (70.70, 70.7), 종료점 (0.0, 100.0), 0축, 1축 (300.0, 400.0)
    //       dpMidPos[4] = { 0.0, 0.0, 50.0, 100.0 }, dpEndPos[4] = { 300.0, 400.0, 0.0, 100.0 } 
    // nArcCircle       : 아크(0), 원(1)
    [DllImport("AXL.dll")] public static extern uint AxmCirclePointMoveWithAxes(int nCoord, int nArraySize, ref int npAxisNo, ref double dpMidPos, ref double dpEndPos, double dVel, double dAccel, double dDecel, int nArcCircle);
    
    // 직선 보간 한다.
    // 시작점과 종료점을 지정하여 다축 직선 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점과 종료점을 지정하여 직선 보간 구동하는 Queue에 저장함수가된다. 
    // 직선 프로파일 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // npAxisNo의 축 배열에 해당하는 축들은 직선 보간을 하며, 나머지 Coordinate의 축들은 직선 보간 비율에 맞게 단축 구동을 수행한다.
    [DllImport("AXL.dll")] public static extern uint AxmLineMoveWithAxesWithEvent(int nCoord, int nArraySize, ref int npAxisNo, ref double dpEndPos, double dVel, double dAccel, double dDecel, uint uEventFlag);
    
    // 2차원/3차원 원호보간 및 그 이상의 축에 대해서 직선보간을 한다.
    // 시작점, 종료점과 중심점을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
    // AxmContiBeginNode, AxmContiEndNode, 와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
    // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
    // npAxisNo = 축 배열 , dpCenterPosition = 중심점 X,Y 배열/X, Y, Z 배열, dpEndPosition = 종료점 X,Y 배열/X, Y, Z배열
    // 2차원/3차원 이상의 축에 대해서는 직선보간을 할 때 dEndPosition의 값을 Targetposition으로 사용한다.
    // uCWDir = DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
    // u3DCircle(0) = 2차원 원호보간 및 그 이상의 축에 대해서 직선보간
    // u3DCircle(1) = 3차원 원호보간 및 그 이상의 축에 대해서 직선보간
    [DllImport("AXL.dll")] public static extern uint AxmCircleCenterMoveWithAxesWithEvent(int nCoord, int nArraySize, ref int npAxisNo, ref double dpCenterPosition, ref double dpEndPosition, double dMaxVelocity, double dMaxAccel, double dMaxDecel, uint uCWDir, uint u3DCircle, uint uEventFlag);
    
    // 단축 PVT 구동을 한다.
	// 사용자가 Position, Velocity, Time Table을 이용하여 생성한 프로파일로 구동한다.
	// AxmSyncBegin, AxmSyncEnd API와 함께 사용시 여러 축의 PVT 구동을 예약한다.
	// 예약된 PVT 구동 프로파일은 AxmSyncStart 명령을 받게되면 동시에 시작한다.
	// nAxisNo : 구동 축
	// uArraySize : PVT Table size
	// dpPos : Position 배열
	// dpVel : Velocity 배열
	// upUsec : Time 배열(Usec 단위, 단 Cycle의 배수여야만 한다. ex 1sec = 1,000,000)
	[DllImport("AXL.dll")] public static extern uint AxmMovePVT(int nAxisNo, uint uArraySize, ref double dpPos, ref double dpVel, ref uint upUsec);
	
	//====Sync 함수 ================================================================================================
	//지정된 Sync No.에서 사용할 축을 맵핑한다.
	//(맵핑 번호는 0 부터 시작))
	// SyncSetAxisMap의 경우 Sync 구동에서 사용되는 유효축을 설정하는 함수이다.
	// SyncBegin과 SyncEnd 사이에서 사용되는 PVT Motion의 지정 축이 맵핑되지 않은 축일 경우
	// 예약되지 않고 즉시 구동한다. 즉 맵핑된 축만이 Begin과 End사이에서 구동 예약이 되며
	// SyncStart API를 호출하면 지정된 Sync Index에서 예약된 구동이 동시에 시작한다.

	// Sync 구동에서 사용될 유효 축을 지정한다.
	// nSyncNo : Sync Index
	// nSize : 맵핑할 축 갯수
	// npAxesNo : 맵핑 축 배열
	[DllImport("AXL.dll")] public static extern uint AxmSyncSetAxisMap(int nSyncNo, int nSize, ref int npAxesNo);

	// 지정된 Sync Index에 할당된 축 맵핑과 예약 프로파일을 리셋한다.
	[DllImport("AXL.dll")] public static extern uint AxmSyncClear(int nSyncNo);

	// 지정된 Sync Index에 수행할 구동 예약을 시작한다.
	// 이 함수를 호출한 후, AxmSyncEnd 함수가 호출되기 전까지 실행되는
	// 유효 축의 PVT 구동은 실제 구동을 즉시 수행하는 것이 아니라 구동 예약이 되며
	// AxmSyncStart 함수가 호출될 때 비로소 예약된 구동이 수행된다.
	[DllImport("AXL.dll")] public static extern uint AxmSyncBegin(int nSyncNo);

	// 지정된 Sync Index에서 수행할 구동 예약을 종료한다.
	[DllImport("AXL.dll")] public static extern uint AxmSyncEnd(int nSyncNo);

	// 지정된 Sync Index에서 예약된 구동을 시작한다.
	[DllImport("AXL.dll")] public static extern uint AxmSyncStart(int nSyncNo);
    
}
