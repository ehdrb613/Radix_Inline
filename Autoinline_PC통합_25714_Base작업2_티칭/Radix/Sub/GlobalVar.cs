using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent; // ConcurrentQueue
using System.IO;
using System.IO.Ports;    // SerialPort 클래스 사용을 위해서 추가
using System.Net;
using System.Net.Sockets;
using System.Diagnostics; // Stopwatch
using System.Collections.Concurrent;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Dialogs; // MessageTask 관련
using System.Runtime.InteropServices;

namespace Radix
{
    #region 전역 Type 선언
    #region enum types

    #region 시스템 전역 Type 구분
    #region 프로젝트 구분
    public enum enumProject
    {
        AutoInline,
        Welding,
        AmplePacking
    }
    #endregion

    public enum enumRunMode
    {
        AutoRun,
        PassMode,
        SimulationMode,
        DryRun
    }

    /** @brief 이더켓 마스터 종류  */
    public enum enumMasterType // 이더켓 마스터 종류 
    {
        //MXP,//LS 산전, 안 깔린 PC에서 작동에러 나서 일단 지움
        AXL,//아젠 엑스텍
        //MXN,//LS 산전 라우터
        ADVANTECH,
        //RTEX_AXL    // RTEX = AXL과 통합
    }
    /** @brief 다관절로봇 메이커 종류  */
    public enum enumRobot // Robot
    {
        KUKA,//쿠카 로봇 사용
        Hyundai,//현대 로봇 사용
        None
    }

    /** @brief 시스템 및 타워 상태  */
    public enum enumSystemStatus // 시스템 및 타워 상태
    {
        BeforeInitialize,
        Initialize,
        Manual,
        EmgStop,
        ErrorStop,
        AutoRun,
        CycleStop,
        InputStop,
        OutputStop
    }

    /** @brief 다국어지원 문자셋  */
    public enum enumLanguage
    {
        English,
        Korean
    }

    /** @brief 법인코드(삼성)  */
    public enum enumCoCode
    {
        SEVT,
        SEV,
        SIEL,
        SEIN,
        SEDA_M,
        SEDA_C,
        SESC
    }

    /** @brief 거래처코드(삼성)  */
    public enum enumCustCode
    {
        M,  // - A015M
        G, // - A015G
        F, // - A015F
        V, // - A015V
        T, // - A015T1
        W, // - A015W
        A, // - A015A
        D, // - SM-S111DL
        Z, // - A015AZ
        R // - A015R4
    }

    /** @brief tower lamp 구분  */
    public enum enumTowerLamp // tower lamp 종류
    {
        Red,
        Yellow,
        Green,
        Buzzer
    }

    /** @brief tower lamp 동작  */
    public enum enumTowerLampAction // Tower Lamp 동작
    {
        Enable, // 작동여부
        Blink // 점멸 여부
    }


    #region 에러 관련
    /** @brief 에러 발생 파트  */
    public enum enumErrorPart// 에러 발생 파트
    {
        No_Error, // 에러 없음
        System, // 안전 관련
        InputJig,
        OutputJig,
        InputLift,
        OutputLift,
        BufferConveyor,
        InputConveyor,
        LoadingConveyor,
        UnloadingConveyor,
        NGConveyor,
        Site1,
        Site2,
        Site3,
        Site4,
        Site5,
        Site6,
        Site7,
        Site8,
        Site9,
        Site10,
        Site11,
        Site12,
        Site13,
        Site14,
        Site15,
        Site16,
        Site17,
        Site18,
        Site19,
        Site20,
        Site21,
        NextMachine
    }
    /** @brief 에러값 구분  */
    public enum enumErrorCode // 에러값 구분
    {
                #region 0~99 안전 시스템 관련
        No_Error,
        E_Stop,
        Door_Opened,
        Axis_Disabled,

        #endregion

        #region 100~199 시스템 정지해야 할 에러. 시스템 전반으로 중요도 높은 것.
        System_Init_Fail = 100,
        Vision_Connect_Fail,

        Router_API_Not_Connect,
        Router_Not_Ready,
        Router_Alarm,
        Router_Routing_TimeOut,
        Router_MPG_EMG,
        Router_Cuttion_Tool_Distance_Over,

        Robot_Disabled,
        Robot_Not_Ext, // 로봇 외부모드 아님
        Robot_Not_Inited, // 로봇 초기화 되지 않음
        Robot_Init_Fail, // 로봇 초기화 실패
        Robot_Error, // 로봇 에러
        Robot_Run_Error, // 로봇 동작 에러
        Robot_Vacuum_Error, // 로봇 진공 에러
        Robot_Scan_Error,   //로봇 바코드 스캔 에러

        Robot_InterLock, // 로봇 동작중 충돌 위험 발생
        Servo_Axis_InterLock, // 로봇 이외 서보모터 이동중 충돌 위험
        #endregion

        #region 장비별 에러는 200~799 사이에서 정의
        Sanding_Before_Tray_In_TimeOut = 200,//샌딩 전 트레이 공급 타임 아웃
        Sanding_Before_Tray_Search_NG, //샌딩 전 공급할 임플란트/배출할 빈공간 없음
        Sanding_Before_Tray_Search_WorkInMove_TimeOut, //샌딩 전 공급할 임플란트가 없을때 Working pos이동중 타임아웃
        Sanding_Before_Tray_Have,//샌딩 전 트레이를 공급하지 못하는 상황
        Sanding_Before_Tray_Clamp_Check,//샌딩 전 이송 트레이 클램프 실린더 타임
        Sanding_Before_Tray_PickUp_Check,//샌딩 전 Pickup Push_Up 실린더 문제
        Sanding_Before_Tray_Working_Check,//샌딩 전 Working 작업중 센서 체크 문제
        Sanding_Before_Tray_Dropoff_Check,//샌딩 전 Pickup Push_Up 실린더 문제
        Sanding_Before_Tray_Out_TimeOut,//샌딩 전 트레이 배출 타임 아웃
        Sanding_After_Tray_Search_NG, //샌딩 후 공급할 트레이/배출할 빈공간 없음
        Sanding_After_Tray_In_TimeOut,//샌딩후 트레이 공급 타임 아웃
        Sanding_After_Tray_Have,//샌딩 후 트레이를 공급하지 못하는 상황
        Sanding_After_Tray_Clamp_Check,//샌딩 후 이송 트레이 클램프 실린더 타임
        Sanding_After_Tray_PickUp_Check,//샌딩 후 Pickup Push_Up 실린더 문제
        Sanding_After_Tray_Working_Check,//샌딩 후 Working 작업중 센서 체크 문제
        Sanding_After_Tray_Dropoff_Check,//샌딩 후 Pickup Push_Up 실린더 문제
        Sanding_After_Tray_CycleStop,//샌딩 후 CycleStop 일때
        Sanding_After_Tray_Out_TimeOut,//샌딩 후 트레이 배출 타임 아웃
        Sanding_Work_Finish,//작업 할 제품이 없다.
        PCB_Info_Move_Fail,


        #region 300~ 테스트 통신 관련
        Test_Command_Timout = 300, // 테스트로 테스트 시작 지령 응답 없음
        Test_Response_Timout, // 테스트 결과 리턴 응답 없음
        Test_All_Fail, // 모든 테스트 결과 실패임
        #endregion

        #region 400~ 나머지 장치들 각 장치별로 세팅할 부분
        #endregion

        #region 600~ 운영 관련
        System_Not_Inited = 600, // 초기화 되지 않은 상태로 운영시도
        #endregion

        #region 700~ 세팅 관련
        Value_Range_Over = 700, // 세팅값이 유효범위 벗어남
        #endregion
        #endregion

        #region 800~ 사용자 호출 필요한 경우, 시스템은 정상 운영, 상황 해제시 자동 해제
        Run_Stopped = 800, // 확인되지 않은 상태로 정지된 경우
        Operator_Call, // 사용자 호출

        Sanding_Machine_Signal_Check,//샌딩기에서 신호를 받지 못함.
        Sanding_Machine_Jig_Check,  //샌딩기 지그센서에서 감지가 안됨

        Robot_Timeout,
        Robot_Gripper_Close_Check,

        Before_Implant_Output_Error,//쿠카가 공급트레이에서 임플란트 빼는 동작을 완료 하지 못함.
        Before_Implant_Input_Error,//쿠카가 샌딩전 임플란트를 샌딩기에 넣지 못함

        After_Implant_Output_Error,//쿠카가 샌딩기에서 임플란트 빼는 동작을 완료 하지 못함.
        After_Implant_Input_Error,//쿠카가 샌딩 완료된 임플란트를 트레이에 넣지 못함.
        After_Tray_Output_Timeout,//

        Only_1Line_implant_Work,
        #endregion

        #region 900~ 이외
        Digital_Input_Check = 900, // 센서 동작 확인
        Digital_Output_Check, // 디지털 출력 확인
        #endregion
        SECS_Not_Connect,
        SECS_TimeOut,
        SECS_Cancle_Receive,
        Robot_Loading_Error,
        Robot_UnLoading_Error,
        Robot_Working_Error,

        // SNC 박스 포장기용 에러 리스트
        Sys_Exception,              // 오토런 중 예외발생 (코딩에러발생)
        Sol_Error,
        Sensor_Error,
        Barcode_Error,
        TimeOut_Error,
        Manual_Empty_Error,         // 매뉴얼 수량 부족 오류
        Manual_Type_Error,          // 매뉴얼 타입이 MES와 다름
        SortOut_Full_Error,         // 분류기 Lot 처리 공간 부족시 라인정지에 사용
        MES_Recv_Error,             // MES 수신이 안됨
        MES_OutPos_Full,            // MES 가 지정한 분류 위치에 이미 다른 Lot가 존재할때
        Label_Print_Error,          // 라벨 프린터에 에러가 있음
        AutoRun_Skip_Error,         // 소재가 없어서 오토런이 너무 많이 스킵될때 발생 에러 
        Conveyor_Skip_Error,        // 너무 많이 연속 컨베이어 스킵이 발생 할때
        Conveyor_Interlock_Error,   // 컨베이어가 구동 시도 했으나 인터락 상태로 구동하지 못했을때
        Conveyor_Position_Error,    // 컨베어 위치오류 발생 (정지위치에서 1mm 이상 변화)
        LaserMark_Error,
        MES_Error,                  //MES 처리중 시간초과
        Fatal_System_Error,         // 복구 불가능한 시스템 오류 ( IO보드 통신 불능 등)


        // 다음 에러 추가는 2100 부터 설정 하라

    }

    //public enum enumError // 에러값 구분
    //{
    //    #region 0~99 안전 시스템 관련
    //    No_Error,
    //    E_Stop,
    //    Door_Opened,
    //    Axis_Disabled,

    //    #endregion

    //    #region 100~199 시스템 정지해야 할 에러. 시스템 전반으로 중요도 높은 것.
    //    System_Init_Fail = 100,
    //    Vision_Connect_Fail,

    //    Router_API_Not_Connect,
    //    Router_Not_Ready,
    //    Router_Alarm,
    //    Router_Routing_TimeOut,
    //    Router_MPG_EMG,
    //    Router_Cuttion_Tool_Distance_Over,

    //    Robot_Disabled,
    //    Robot_Not_Ext, // 로봇 외부모드 아님
    //    Robot_Not_Inited, // 로봇 초기화 되지 않음
    //    Robot_Init_Fail, // 로봇 초기화 실패
    //    Robot_Error, // 로봇 에러
    //    Robot_Run_Error, // 로봇 동작 에러
    //    Robot_Vacuum_Error, // 로봇 진공 에러
    //    Robot_Scan_Error,   //로봇 바코드 스캔 에러

    //    Robot_InterLock, // 로봇 동작중 충돌 위험 발생
    //    Servo_Axis_InterLock, // 로봇 이외 서보모터 이동중 충돌 위험
    //    #endregion

    //    #region 장비별 에러는 200~799 사이에서 정의
    //    #region 200~ 공통부
    //    Scrap_Full,//스크랩 가득 참
    //    #endregion

    //    #region 300~ 테스트 통신 관련
    //    Test_Command_Timout = 300, // 테스트로 테스트 시작 지령 응답 없음
    //    Test_Response_Timout, // 테스트 결과 리턴 응답 없음
    //    Test_All_Fail, // 모든 테스트 결과 실패임
    //    #endregion

    //    #region 400~ 나머지 장치들 각 장치별로 세팅할 부분
    //    #endregion

    //    #region 600~ 운영 관련
    //    System_Not_Inited = 600, // 초기화 되지 않은 상태로 운영시도
    //    #endregion

    //    #region 700~ 세팅 관련
    //    Value_Range_Over = 700, // 세팅값이 유효범위 벗어남
    //    #endregion
    //    #endregion

    //    #region 800~ 사용자 호출 필요한 경우, 시스템은 정상 운영, 상황 해제시 자동 해제
    //    Run_Stopped = 800, // 확인되지 않은 상태로 정지된 경우
    //    Operator_Call, // 사용자 호출

    //    Sanding_Machine_Signal_Check,//샌딩기에서 신호를 받지 못함.

    //    Robot_Timeout,
    //    Robot_Gripper_Close_Check,

    //    Before_Implant_Output_Error,//쿠카가 공급트레이에서 임플란트 빼는 동작을 완료 하지 못함.
    //    Before_Implant_Input_Error,//쿠카가 샌딩전 임플란트를 샌딩기에 넣지 못함

    //    After_Implant_Output_Error,//쿠카가 샌딩기에서 임플란트 빼는 동작을 완료 하지 못함.
    //    After_Implant_Input_Error,//쿠카가 샌딩 완료된 임플란트를 트레이에 넣지 못함.
    //    After_Tray_Output_Timeout,//

    //    Only_1Line_implant_Work,
    //    #endregion

    //    #region 900~ 이외
    //    Digital_Input_Check = 900, // 센서 동작 확인
    //    Digital_Output_Check, // 디지털 출력 확인
    //    #endregion
    //    SECS_Not_Connect,
    //    SECS_TimeOut,
    //    SECS_Cancle_Receive,
    //    Robot_Loading_Error,
    //    Robot_UnLoading_Error,
    //    Robot_Working_Error,
    //}
    #endregion


    

    #endregion

    #region 동작 관련
    /** @brief NG 컨베어 동작  */
    public enum enumNGAction // NG 컨베어 동작
    {
        Forward,
        Reward,
        Run,
        Stop
    }

    /** @brief 컨베어 동작 방향  */
    public enum enumConveyorAction // 컨베어 동작 방향
    {
        Forward,
        Reward,
        Stop
    }

    /** @brief 리프트 동작 구분  */
    public enum enumLiftAction // 리프트 동작 구분
    {
        None, // 해당 동작 없음
        Input, // 앞 장비(장치)에서 PCB 투입
        Output, // 뒷 장비(장치)로 PCB 배출
        Scan, // 스캔 실행
        Evade // 회피 위치 이동
    }


    /** @brief 로봇 동작 구분  */
    public enum enumRobotAction // 로봇 동작 구분
    {
        Loading_1_Get,//Loading 1 취출
        Loading_2_Get,//Loading 2 취출
        Loading_3_Get,//Loading 3 취출
        Work_0_Put,//Work 0 공급
        Work_1_Put,//Work 1 공급
        Work_2_Put,//Work 2 공급
        Work_3_Put,//Work 3 공급
        Work_4_Put,//Work 4 공급
        Work_5_Put,//Work 5 공급
        Work_6_Put,//Work 6 공급
        Work_7_Put,//Work 7 공급
        Work_8_Put,//Work 8 공급
        Work_9_Put,//Work 9 공급
        Work_0_Get,//Work 0 취출
        Work_1_Get,//Work 1 취출
        Work_2_Get,//Work 2 취출
        Work_3_Get,//Work 3 취출
        Work_4_Get,//Work 4 취출
        Work_5_Get,//Work 5 취출
        Work_6_Get,//Work 6 취출
        Work_7_Get,//Work 7 취출
        Work_8_Get,//Work 8 취출
        Work_9_Get,//Work 9 취출
        UnLoading,//UnLoading 공급

        OutBefore, // 샌딩 전 취출
        InBefore, // 샌딩 전 공급
        OutAfter, // 샌딩 후 취출
        InAfter, // 샌딩 후 공급
        OutBuffer, // 버퍼 취출
        InBuffer, // 버퍼 공급
        BeforeTarget, // 샌딩 전 취출 & 샌딩기 공급
        TargetAfter, // 샌딩기 취출 & 샌딩 후 배출
        None
    }

    /** @brief PCB 이송 동작 구분. * Queue로 관리. * 종속성(독립실행) 관련은 고민해 봐야함(아마도 쓰레드별로 따로 관리하면 될 듯). * 사이트번호를 따로 할지, 한번에 늘어 놓을지  */
    public enum enumMoveAction // PCB 이송 동작 구분. Queue로 관리. 종속성(독립실행) 관련은 고민해 봐야함(아마도 쓰레드별로 따로 관리하면 될 듯). 사이트번호를 따로 할지, 한번에 늘어 놓을지...
    {
        None, // 동작 없음
        BeforeToInputLift, // 전장비에서 PCB 받기
        InputLiftToScan, // 스캔위치로
        ScanToInputLift, // 스캔 후 
        InputLIftToBuffer, // Pass1으로
        BufferToInput, // Pass2로
        InputToLoading, // Pass3으로
        LoadingToUnloading, // Pass4로
        UnloadingToOutputLift, // OutputLIft로
        OutputLiftToAfter, // Output Lift에서 다음 장비로
        LoadingToRobot, // Pass3에서 PCB 취출
        RobotToUnloading, // Robot에서 Pass4로 배출
        RobotToNG, // Robot에서 NG 버퍼로
        ToSite1,
        ToSite2,
        ToSite3,
        ToSite4,
        ToSite5,
        ToSite6,
        ToSite7,
        ToSite8,
        ToSite9,
        ToSite10,
        ToSite11,
        ToSite12,
        ToSite13,
        ToSite14,
        ToSite15,
        ToSite16,
        ToSite17,
        ToSite18,
        ToSite19,
        ToSite20,
        ToSite21,
        FromSite1,
        FromSite2,
        FromSite3,
        FromSite4,
        FromSite5,
        FromSite6,
        FromSite7,
        FromSite8,
        FromSite9,
        FromSite10,
        FromSite11,
        FromSite12,
        FromSite13,
        FromSite14,
        FromSite15,
        FromSite16,
        FromSite17,
        FromSite18,
        FromSite19,
        FromSite20,
        FromSite21
    }
    /** @brief 사이트 자체 동작 구분  */
    public enum enumSiteAction // 사이트 자체 동작 구분
    {
        NotUse, // 사용 안 함
        PINChange, // 핀 교체중
        Waiting, // 비어 있음. 닫혀 있는 상태
        Opening, // 전면 오픈중
        Opened, // 열려 있음
        Closing, // 닫는 중
        Testing // 테스트중
    }

    /** @brief DryRun 동작 순서  */
    public enum enumDryRunMethod // DryRun 동작 순서
    {
        None,
        InputLiftScan,
        InputLiftOut,
        BufferConveyorOut,
        InputConveyorOut,
        RobotInput,
        RobotOutput,
        LoadingConveyorOut,
        UnloadingConveyorOut,
        OutputLiftOut
    }

    /** @brief 리프트 이름 */
    public enum enumLiftName // 리프트 이름
    {
        InputLift,
        OutputLift,
        Scan
    }

    /** @brief 리프트 위치 */
    public enum enumLiftPos // 리프트 위치
    {
        Down_Front,
        Up_Rear,
        Scan
    }

    /** @brief 장비 이송방향 구분. 좌우/우좌 */
    public enum enumMachineLeftRight // 좌우/우좌
    {
        LeftRight,
        RightLeft
    }

    /** @brief 장비  전/후면 기준면 */
    public enum enumMachineFrontRear // 전/후면 기준면
    {
        FrontBase,
        RearBase
    }

    /** @brief 장비 직렬 연결 여부 */
    public enum enumMachineLinear // 직렬 연결 여부
    {
        StandAlone,
        LinearFirst,
        LinearSecond,
        LinearAuto
    }

    /** @brief 다관절 로봇 툴 선택 */
    public enum enumRobotTool // 로봇 툴 선택
    {
        Left,
        Right,
        Both,
        None
    }

    /** @brief 로봇 이동시 기준 위치 (인라인) */
    public enum enumRobotJig // 로봇 이동시 기준 위치
    {
        InputJig,
        OutputJig,
        Vision
    }


    #endregion

    #region 통신관련
    /** @brief SECS GEM 시나리오 목록 */
    public enum enumSecsSenario // SECS GEM 시나리오 목록
    {
        StoreOutNormal, // 물류창고에서 AGV로 투입, 정상
        StoreInNormal, // AGV에서 물류창고로 반입, 정상
        ProcessNormal, // 장비에서 정상 처리
        CancelByHostRetry, // host에서 정지, 조치후 재시작
        CancelByHostAbort, // host에서 정지, 작업 취소
        RecipeFailByEqpRetry, //  eqp에서 레시피 실패로 정지 후 재시작
        RecipeFailByEqpAboard, // eqp에서 레시피 실패로 정지 후 취소
        ConversationTimeoutRetry, // 시작 지령 누락 후 재시작
        ConversationTimeoutAbrt, // 시작 지령 누락 후 취소
    }

    #endregion

    #region 디지털 입출력
    /** @brief 디지털 입력점 이름. 프로젝트별로 func프로젝트명.cs에서 별도 선언할 것. */
    public enum enumDINames // 디지털 입력점 이름
    {
        X00_0_MPG_RATE_SELET_1,
        X00_1_MPG_RATE_SELET_2,
        X00_2_MPG_AXIS_SELECT_1,
        X00_3_MPG_AXIS_SELECT_2,
        X00_4_MPG_AXIS_SELECT_4,
        X00_5_MPG_EMERGENCY_SW,
        X00_6_,
        X00_7_,
        X01_0_A_AXIS_LIMIT_plus,
        X01_1_COLLECTOR_OVERLAOD,
        X01_2_SPINDLE_1_ALARM,
        X01_3_SPINDLE_2_ALARM,
        X01_4_,
        X01_5_,
        X01_6_,
        X01_7_,
        X02_0_AXIS_0_NOT,
        X02_1_AXIS_0_POT,
        X02_2_AXIS_1_NOT,
        X02_3_AXIS_1_POT,
        X02_4_AXIS_2_NOT,
        X02_5_AXIS_2_POT,
        X02_6_AXIS_3_NOT,
        X02_7_AXIS_3_POT,
        X03_0_AXIS_4_NOT,
        X03_1_AXIS_4_POT,
        X03_2_AXIS_5_NOT,
        X03_3_AXIS_5_POT,
        X03_4_AXIS_6_NOT,
        X03_5_AXIS_6_POT,
        X03_6_AXIS_7_NOT,
        X03_7_AXIS_7_POT,
        X04_0_AXIS_8_NOT,
        X04_1_AXIS_8_POT,
        X04_2_AXIS_9_NOT,
        X04_3_AXIS_9_POT,
        X04_4_AXIS_10_NOT,
        X04_5_AXIS_10_POT,
        X04_6_AXIS_11_NOT,
        X04_7_AXIS_11_POT,
        X05_0_AXIS_12_NOT,
        X05_1_AXIS_12_POT,
        X05_2_AXIS_13_NOT,
        X05_3_AXIS_13_POT,
        X05_4_AXIS_14_NOT,
        X05_5_AXIS_14_POT,
        X05_6_AXIS_15_NOT,
        X05_7_AXIS_15_POT,
        X06_0_AXIS_0_Home,
        X06_1_AXIS_1_Home,
        X06_2_AXIS_2_Home,
        X06_3_AXIS_3_Home,
        X06_4_AXIS_4_Home,
        X06_5_AXIS_5_Home,
        X06_6_AXIS_6_Home,
        X06_7_AXIS_7_Home,
        X07_0_AXIS_8_Home,
        X07_1_AXIS_9_Home,
        X07_2_AXIS_10_Home,
        X07_3_AXIS_11_Home,
        X07_4_AXIS_12_Home,
        X07_5_AXIS_13_Home,
        X07_6_AXIS_14_Home,
        X07_7_AXIS_15_Home,
        #region 아래로 사용안함
        /*
        X08_0_EMGENCY_SWITCH,
        X08_1_START_SWITCH,
        X08_2_STOP_SWITCH,
        X08_3_RESET_SWITCH,
        X08_4_,
        X08_5_,
        X08_6_,
        X08_7_,
        X09_0_Door_Front_Sensor,
        X09_1_Door_Back_Sensor,
        X09_2_Door_Left_Sensor,
        X09_3_Door_Right_PCB_Sensor,
        X09_4_Door_Right_Tray_Sensor,
        X09_5_Door_Down_Sensor_1,
        X09_6_Door_Down_Sensor_2,
        X09_7_,
        X10_0_PCBInput_Magazine_Max_Clamp_TopForward,
        X10_1_PCBInput_Magazine_Max_Clamp_TopBackward,
        X10_2_PCBInput_Magazine_Min_Clamp_TopForward,
        X10_3_PCBInput_Magazine_Min_Clamp_TopBackward,
        X10_4_PCBInput_Magazine_Max_Clamp_BtmForward,
        X10_5_PCBInput_Magazine_Max_Clamp_BtmBackward,
        X10_6_PCBInput_Magazine_Min_Clamp_BtmForward,
        X10_7_PCBInput_Magazine_Min_Clamp_BtmBackward,
        X11_0_BoxIN_Magazine_Max_Clamp_TopForward,
        X11_1_BoxIN_Magazine_Max_Clamp_TopBackward,
        X11_2_BoxIN_Magazine_Min_Clamp_TopForward,
        X11_3_BoxIN_Magazine_Min_Clamp_TopBackward,
        X11_4_BoxIN_Magazine_Max_Clamp_BtmForward,
        X11_5_BoxIN_Magazine_Max_Clamp_BtmBackward,
        X11_6_BoxIN_Magazine_Min_Clamp_BtmForward,
        X11_7_BoxIN_Magazine_Min_Clamp_BtmBackward,
        X12_0_BoxOUT_Magazine_Max_Clamp_TopForward,
        X12_1_BoxOUT_Magazine_Max_Clamp_TopBackward,
        X12_2_BoxOUT_Magazine_Min_Clamp_TopForward,
        X12_3_BoxOUT_Magazine_Min_Clamp_TopBackward,
        X12_4_BoxOUT_Magazine_Max_Clamp_BtmForward,
        X12_5_BoxOUT_Magazine_Max_Clamp_BtmBackward,
        X12_6_BoxOUT_Magazine_Min_Clamp_BtmForward,
        X12_7_BoxOUT_Magazine_Min_Clamp_BtmBackward,
        X13_0_PCBInput_Magazine_Check_Sensor,
        X13_1_PCBInput_Push_Cylinder_Forward,
        X13_2_PCBInput_Push_Cylinder_Backward,
        X13_3_PCBInput_PCB_Check_Sensor,
        X13_4_InConver_PCB_Arrival_Sensor,
        X13_5_InConver_PCB_Detect_Sensor,
        X13_6_PCBInput_Push_1_Cylinder_Forward,
        X13_7_PCBInput_Push_1_Cylinder_Backward,
        X14_0_Array_Vacuum_1,
        X14_1_Array_Vacuum_2,
        X14_2_Array_Vacuum_3,
        X14_3_Array_Vacuum_4,
        X14_4_Array_Vacuum_5,
        X14_5_Scrap_Vacuum_1,
        X14_6_Scrap_Full,
        X14_7_,
        X15_0_Lift_BoxInPut_Magazine_Check_Sensor,
        X15_1_Lift_BoxInPut_Push_Cylinder_Forward,
        X15_2_Lift_BoxInPut_Push_Cylinder_Backward,
        X15_3_Lift_BoxInPut_Box_Check_Sensor,
        X15_4_Box_Arrival_Sensor,
        X15_5_Box_Detect_Sensor,
        X15_6_Box_Out_Push_Cylinder_Forward,
        X15_7_Box_Out_Push_Cylinder_Backward,
        X16_0_Box_Stopper_Cylinder_Forward_Left,
        X16_1_Box_Stopper_Cylinder_Backward_Left,
        X16_2_BoxWait_Stopper_Cylinder_Up,
        X16_3_BoxWait_Stopper_Cylinder_Down,
        X16_4_Output_Magazine_Out_Check_Sensor,
        X16_5_BoxWait_Detect_Sensor,
        X16_6_Lift_BoxInPut_Push_1_Cylinder_Forward,
        X16_7_Lift_BoxInPut_Push_1_Cylinder_Backward,        
        X17_0_Module2_Escape_Front_Right_Forward_Sensor,
        X17_1_Module2_Escape_Front_Right_Reward_Sensor,
        X17_2_Module2_Escape_Rear_Left_Forward_Sensor,
        X17_3_Module2_Escape_Rear_Left_Reward_Sensor,
        X17_4_Module2_Escape_Rear_Right_Forward_Sensor,
        X17_5_Module2_Escape_Rear_Right_Reward_Sensor,
        X17_6_Module2_Power_On,
        X17_7_Module2_Pressure_Check,
        X18_0_Module3_Clamp_Forward_Sensor,
        X18_1_Module3_Clamp_Reward_Sensor,
        X18_2_Module3_Module_Sensor,
        X18_3_Module3_PCB_Sensor,
        X18_4_Module3_Front_Left_Up_Sensor,
        X18_5_Module3_Front_Left_Down_Sensor,
        X18_6_Module3_Front_Right_Up_Sensor,
        X18_7_Module3_Front_Right_Down_Sensor,
        X19_0_Module3_Rear_Left_Up_Sensor,
        X19_1_Module3_Rear_Left_Down_Sensor,
        X19_2_Module3_Rear_Right_Up_Sensor,
        X19_3_Module3_Rear_Right_Down_Sensor,
        X19_4_Module3_Forward_Sensor,
        X19_5_Module3_Reward_Sensor,
        X19_6_Module3_Escape_Front_Left_Forward_Sensor,
        X19_7_Module3_Escape_Front_Left_Reward_Sensor,
        X20_0_Module3_Escape_Front_Right_Forward_Sensor,
        X20_1_Module3_Escape_Front_Right_Reward_Sensor,
        X20_2_Module3_Escape_Rear_Left_Forward_Sensor,
        X20_3_Module3_Escape_Rear_Left_Reward_Sensor,
        X20_4_Module3_Escape_Rear_Right_Forward_Sensor,
        X20_5_Module3_Escape_Rear_Right_Reward_Sensor,
        X20_6_Module3_Power_On,
        X20_7_Module3_Pressure_Check,
        X21_0_Module4_Clamp_Forward_Sensor,
        X21_1_Module4_Clamp_Reward_Sensor,
        X21_2_Module4_Module_Sensor,
        X21_3_Module4_PCB_Sensor,
        X21_4_Module4_Front_Left_Up_Sensor,
        X21_5_Module4_Front_Left_Down_Sensor,
        X21_6_Module4_Front_Right_Up_Sensor,
        X21_7_Module4_Front_Right_Down_Sensor,
        X22_0_Module4_Rear_Left_Up_Sensor,
        X22_1_Module4_Rear_Left_Down_Sensor,
        X22_2_Module4_Rear_Right_Up_Sensor,
        X22_3_Module4_Rear_Right_Down_Sensor,
        X22_4_Module4_Forward_Sensor,
        X22_5_Module4_Reward_Sensor,
        X22_6_Module4_Escape_Front_Left_Forward_Sensor,
        X22_7_Module4_Escape_Front_Left_Reward_Sensor,
        X23_0_Module4_Escape_Front_Right_Forward_Sensor,
        X23_1_Module4_Escape_Front_Right_Reward_Sensor,
        X23_2_Module4_Escape_Rear_Left_Forward_Sensor,
        X23_3_Module4_Escape_Rear_Left_Reward_Sensor,
        X23_4_Module4_Escape_Rear_Right_Forward_Sensor,
        X23_5_Module4_Escape_Rear_Right_Reward_Sensor,
        X23_6_Module4_Power_On,
        X23_7_Module4_Pressure_Check,
        X24_0_Module5_Clamp_Forward_Sensor,
        X24_1_Module5_Clamp_Reward_Sensor,
        X24_2_Module5_Module_Sensor,
        X24_3_Module5_PCB_Sensor,
        X24_4_Module5_Front_Left_Up_Sensor,
        X24_5_Module5_Front_Left_Down_Sensor,
        X24_6_Module5_Front_Right_Up_Sensor,
        X24_7_Module5_Front_Right_Down_Sensor,
        X25_0_Module5_Rear_Left_Up_Sensor,
        X25_1_Module5_Rear_Left_Down_Sensor,
        X25_2_Module5_Rear_Right_Up_Sensor,
        X25_3_Module5_Rear_Right_Down_Sensor,
        X25_4_Module5_Forward_Sensor,
        X25_5_Module5_Reward_Sensor,
        X25_6_Module5_Escape_Front_Left_Forward_Sensor,
        X25_7_Module5_Escape_Front_Left_Reward_Sensor,
        X26_0_Module5_Escape_Front_Right_Forward_Sensor,
        X26_1_Module5_Escape_Front_Right_Reward_Sensor,
        X26_2_Module5_Escape_Rear_Left_Forward_Sensor,
        X26_3_Module5_Escape_Rear_Left_Reward_Sensor,
        X26_4_Module5_Escape_Rear_Right_Forward_Sensor,
        X26_5_Module5_Escape_Rear_Right_Reward_Sensor,
        X26_6_Module5_Power_On,
        X26_7_Module5_Pressure_Check,
        X27_0_Module6_Clamp_Forward_Sensor,
        X27_1_Module6_Clamp_Reward_Sensor,
        X27_2_Module6_Module_Sensor,
        X27_3_Module6_PCB_Sensor,
        X27_4_Module6_Front_Left_Up_Sensor,
        X27_5_Module6_Front_Left_Down_Sensor,
        X27_6_Module6_Front_Right_Up_Sensor,
        X27_7_Module6_Front_Right_Down_Sensor,
        X28_0_Module6_Rear_Left_Up_Sensor,
        X28_1_Module6_Rear_Left_Down_Sensor,
        X28_2_Module6_Rear_Right_Up_Sensor,
        X28_3_Module6_Rear_Right_Down_Sensor,
        X28_4_Module6_Forward_Sensor,
        X28_5_Module6_Reward_Sensor,
        X28_6_Module6_Escape_Front_Left_Forward_Sensor,
        X28_7_Module6_Escape_Front_Left_Reward_Sensor,
        X29_0_Module6_Escape_Front_Right_Forward_Sensor,
        X29_1_Module6_Escape_Front_Right_Reward_Sensor,
        X29_2_Module6_Escape_Rear_Left_Forward_Sensor,
        X29_3_Module6_Escape_Rear_Left_Reward_Sensor,
        X29_4_Module6_Escape_Rear_Right_Forward_Sensor,
        X29_5_Module6_Escape_Rear_Right_Reward_Sensor,
        X29_6_Module6_Power_On,
        X29_7_Module6_Pressure_Check,
        X30_0_Module7_Clamp_Forward_Sensor,
        X30_1_Module7_Clamp_Reward_Sensor,
        X30_2_Module7_Module_Sensor,
        X30_3_Module7_PCB_Sensor,
        X30_4_Module7_Front_Left_Up_Sensor,
        X30_5_Module7_Front_Left_Down_Sensor,
        X30_6_Module7_Front_Right_Up_Sensor,
        X30_7_Module7_Front_Right_Down_Sensor,
        X30_8_Module7_Rear_Left_Up_Sensor,
        X31_1_Module7_Rear_Left_Down_Sensor,
        X31_2_Module7_Rear_Right_Up_Sensor,
        X31_3_Module7_Rear_Right_Down_Sensor,
        X31_4_Module7_Forward_Sensor,
        X31_5_Module7_Reward_Sensor,
        X31_6_Module7_Escape_Front_Left_Forward_Sensor,
        X31_7_Module7_Escape_Front_Left_Reward_Sensor,
        X32_0_Module7_Escape_Front_Right_Forward_Sensor,
        X32_1_Module7_Escape_Front_Right_Reward_Sensor,
        X32_2_Module7_Escape_Rear_Left_Forward_Sensor,
        X32_3_Module7_Escape_Rear_Left_Reward_Sensor,
        X32_4_Module7_Escape_Rear_Right_Forward_Sensor,
        X32_5_Module7_Escape_Rear_Right_Reward_Sensor,
        X32_6_Module7_Power_On,
        X32_7_Module7_Pressure_Check,
        X33_0_Module8_Clamp_Forward_Sensor,
        X33_1_Module8_Clamp_Reward_Sensor,
        X33_2_Module8_Module_Sensor,
        X33_3_Module8_PCB_Sensor,
        X33_4_Module8_Front_Left_Up_Sensor,
        X33_5_Module8_Front_Left_Down_Sensor,
        X33_6_Module8_Front_Right_Up_Sensor,
        X33_7_Module8_Front_Right_Down_Sensor,
        X34_0_Module8_Rear_Left_Up_Sensor,
        X34_1_Module8_Rear_Left_Down_Sensor,
        X34_2_Module8_Rear_Right_Up_Sensor,
        X34_3_Module8_Rear_Right_Down_Sensor,
        X34_4_Module8_Forward_Sensor,
        X34_5_Module8_Reward_Sensor,
        X34_6_Module8_Escape_Front_Left_Forward_Sensor,
        X34_7_Module8_Escape_Front_Left_Reward_Sensor,
        X35_0_Module8_Escape_Front_Right_Forward_Sensor,
        X35_1_Module8_Escape_Front_Right_Reward_Sensor,
        X35_2_Module8_Escape_Rear_Left_Forward_Sensor,
        X35_3_Module8_Escape_Rear_Left_Reward_Sensor,
        X35_4_Module8_Escape_Rear_Right_Forward_Sensor,
        X35_5_Module8_Escape_Rear_Right_Reward_Sensor,
        X35_6_Module8_Power_On,
        X35_7_Module8_Pressure_Check,
        X36_0_Module9_Clamp_Forward_Sensor,
        X36_1_Module9_Clamp_Reward_Sensor,
        X36_2_Module9_Module_Sensor,
        X36_3_Module9_PCB_Sensor,
        X36_4_Module9_Front_Left_Up_Sensor,
        X36_5_Module9_Front_Left_Down_Sensor,
        X36_6_Module9_Front_Right_Up_Sensor,
        X36_7_Module9_Front_Right_Down_Sensor,
        X37_0_Module9_Rear_Left_Up_Sensor,
        X37_1_Module9_Rear_Left_Down_Sensor,
        X37_2_Module9_Rear_Right_Up_Sensor,
        X37_3_Module9_Rear_Right_Down_Sensor,
        X37_4_Module9_Forward_Sensor,
        X37_5_Module9_Reward_Sensor,
        X37_6_Module9_Escape_Front_Left_Forward_Sensor,
        X37_7_Module9_Escape_Front_Left_Reward_Sensor,
        X38_0_Module9_Escape_Front_Right_Forward_Sensor,
        X38_1_Module9_Escape_Front_Right_Reward_Sensor,
        X38_2_Module9_Escape_Rear_Left_Forward_Sensor,
        X38_3_Module9_Escape_Rear_Left_Reward_Sensor,
        X38_4_Module9_Escape_Rear_Right_Forward_Sensor,
        X38_5_Module9_Escape_Rear_Right_Reward_Sensor,
        X38_6_Module9_Power_On,
        X38_7_Module9_Pressure_Check,
        X39_0_Module10_Clamp_Forward_Sensor,
        X39_1_Module10_Clamp_Reward_Sensor,
        X39_2_Module10_Module_Sensor,
        X39_3_Module10_PCB_Sensor,
        X39_4_Module10_Front_Left_Up_Sensor,
        X39_5_Module10_Front_Left_Down_Sensor,
        X39_6_Module10_Front_Right_Up_Sensor,
        X39_7_Module10_Front_Right_Down_Sensor,
        X40_0_Module10_Rear_Left_Up_Sensor,
        X40_1_Module10_Rear_Left_Down_Sensor,
        X40_2_Module10_Rear_Right_Up_Sensor,
        X40_3_Module10_Rear_Right_Down_Sensor,
        X40_4_Module10_Forward_Sensor,
        X40_5_Module10_Reward_Sensor,
        X40_6_Module10_Escape_Front_Left_Forward_Sensor,
        X40_7_Module10_Escape_Front_Left_Reward_Sensor,
        X41_0_Module10_Escape_Front_Right_Forward_Sensor,
        X41_1_Module10_Escape_Front_Right_Reward_Sensor,
        X41_2_Module10_Escape_Rear_Left_Forward_Sensor,
        X41_3_Module10_Escape_Rear_Left_Reward_Sensor,
        X41_4_Module10_Escape_Rear_Right_Forward_Sensor,
        X41_5_Module10_Escape_Rear_Right_Reward_Sensor,
        X41_6_Module10_Power_On,
        X41_7_Module10_Pressure_Check,
        X42_0_Module11_Clamp_Forward_Sensor,
        X42_1_Module11_Clamp_Reward_Sensor,
        X42_2_Module11_Module_Sensor,
        X42_3_Module11_PCB_Sensor,
        X42_4_Module11_Front_Left_Up_Sensor,
        X42_5_Module11_Front_Left_Down_Sensor,
        X42_6_Module11_Front_Right_Up_Sensor,
        X42_7_Module11_Front_Right_Down_Sensor,
        X43_0_Module11_Rear_Left_Up_Sensor,
        X43_1_Module11_Rear_Left_Down_Sensor,
        X43_2_Module11_Rear_Right_Up_Sensor,
        X43_3_Module11_Rear_Right_Down_Sensor,
        X43_4_Module11_Forward_Sensor,
        X43_5_Module11_Reward_Sensor,
        X43_6_Module11_Escape_Front_Left_Forward_Sensor,
        X43_7_Module11_Escape_Front_Left_Reward_Sensor,
        X44_0_Module11_Escape_Front_Right_Forward_Sensor,
        X44_1_Module11_Escape_Front_Right_Reward_Sensor,
        X44_2_Module11_Escape_Rear_Left_Forward_Sensor,
        X44_3_Module11_Escape_Rear_Left_Reward_Sensor,
        X44_4_Module11_Escape_Rear_Right_Forward_Sensor,
        X44_5_Module11_Escape_Rear_Right_Reward_Sensor,
        X44_6_Module11_Power_On,
        X44_7_Module11_Pressure_Check,
        X45_0_Module12_Clamp_Forward_Sensor,
        X45_1_Module12_Clamp_Reward_Sensor,
        X45_2_Module12_Module_Sensor,
        X45_3_Module12_PCB_Sensor,
        X45_4_Module12_Front_Left_Up_Sensor,
        X45_5_Module12_Front_Left_Down_Sensor,
        X45_6_Module12_Front_Right_Up_Sensor,
        X45_7_Module12_Front_Right_Down_Sensor,
        X46_0_Module12_Rear_Left_Up_Sensor,
        X46_1_Module12_Rear_Left_Down_Sensor,
        X46_2_Module12_Rear_Right_Up_Sensor,
        X46_3_Module12_Rear_Right_Down_Sensor,
        X46_4_Module12_Forward_Sensor,
        X46_5_Module12_Reward_Sensor,
        X46_6_Module12_Escape_Front_Left_Forward_Sensor,
        X46_7_Module12_Escape_Front_Left_Reward_Sensor,
        X47_0_Module12_Escape_Front_Right_Forward_Sensor,
        X47_1_Module12_Escape_Front_Right_Reward_Sensor,
        X47_2_Module12_Escape_Rear_Left_Forward_Sensor,
        X47_3_Module12_Escape_Rear_Left_Reward_Sensor,
        X47_4_Module12_Escape_Rear_Right_Forward_Sensor,
        X47_5_Module12_Escape_Rear_Right_Reward_Sensor,
        X47_6_Module12_Power_On,
        X47_7_Module12_Pressure_Check,
        X48_0_Module13_Clamp_Forward_Sensor,
        X48_1_Module13_Clamp_Reward_Sensor,
        X48_2_Module13_Module_Sensor,
        X48_3_Module13_PCB_Sensor,
        X48_4_Module13_Front_Left_Up_Sensor,
        X48_5_Module13_Front_Left_Down_Sensor,
        X48_6_Module13_Front_Right_Up_Sensor,
        X48_7_Module13_Front_Right_Down_Sensor,
        X49_0_Module13_Rear_Left_Up_Sensor,
        X49_1_Module13_Rear_Left_Down_Sensor,
        X49_2_Module13_Rear_Right_Up_Sensor,
        X49_3_Module13_Rear_Right_Down_Sensor,
        X49_4_Module13_Forward_Sensor,
        X49_5_Module13_Reward_Sensor,
        X49_6_Module13_Escape_Front_Left_Forward_Sensor,
        X49_7_Module13_Escape_Front_Left_Reward_Sensor,
        X50_0_Module13_Escape_Front_Right_Forward_Sensor,
        X50_1_Module13_Escape_Front_Right_Reward_Sensor,
        X50_2_Module13_Escape_Rear_Left_Forward_Sensor,
        X50_3_Module13_Escape_Rear_Left_Reward_Sensor,
        X50_4_Module13_Escape_Rear_Right_Forward_Sensor,
        X50_5_Module13_Escape_Rear_Right_Reward_Sensor,
        X50_6_Module13_Power_On,
        X50_7_Module13_Pressure_Check,
        X51_0_Module14_Clamp_Forward_Sensor,
        X51_1_Module14_Clamp_Reward_Sensor,
        X51_2_Module14_Module_Sensor,
        X51_3_Module14_PCB_Sensor,
        X51_4_Module14_Front_Left_Up_Sensor,
        X51_5_Module14_Front_Left_Down_Sensor,
        X51_6_Module14_Front_Right_Up_Sensor,
        X51_7_Module14_Front_Right_Down_Sensor,
        X52_0_Module14_Rear_Left_Up_Sensor,
        X52_1_Module14_Rear_Left_Down_Sensor,
        X52_2_Module14_Rear_Right_Up_Sensor,
        X52_3_Module14_Rear_Right_Down_Sensor,
        X52_4_Module14_Forward_Sensor,
        X52_5_Module14_Reward_Sensor,
        X52_6_Module14_Escape_Front_Left_Forward_Sensor,
        X52_7_Module14_Escape_Front_Left_Reward_Sensor,
        X53_0_Module14_Escape_Front_Right_Forward_Sensor,
        X53_1_Module14_Escape_Front_Right_Reward_Sensor,
        X53_2_Module14_Escape_Rear_Left_Forward_Sensor,
        X53_3_Module14_Escape_Rear_Left_Reward_Sensor,
        X53_4_Module14_Escape_Rear_Right_Forward_Sensor,
        X53_5_Module14_Escape_Rear_Right_Reward_Sensor,
        X53_6_Module14_Power_On,
        X53_7_Module14_Pressure_Check,
        X54_0_Module15_Clamp_Forward_Sensor,
        X54_1_Module15_Clamp_Reward_Sensor,
        X54_2_Module15_Module_Sensor,
        X54_3_Module15_PCB_Sensor,
        X54_4_Module15_Front_Left_Up_Sensor,
        X54_5_Module15_Front_Left_Down_Sensor,
        X54_6_Module15_Front_Right_Up_Sensor,
        X54_7_Module15_Front_Right_Down_Sensor,
        X55_0_Module15_Rear_Left_Up_Sensor,
        X55_1_Module15_Rear_Left_Down_Sensor,
        X55_2_Module15_Rear_Right_Up_Sensor,
        X55_3_Module15_Rear_Right_Down_Sensor,
        X55_4_Module15_Forward_Sensor,
        X55_5_Module15_Reward_Sensor,
        X55_6_Module15_Escape_Front_Left_Forward_Sensor,
        X55_7_Module15_Escape_Front_Left_Reward_Sensor,
        X56_0_Module15_Escape_Front_Right_Forward_Sensor,
        X56_1_Module15_Escape_Front_Right_Reward_Sensor,
        X56_2_Module15_Escape_Rear_Left_Forward_Sensor,
        X56_3_Module15_Escape_Rear_Left_Reward_Sensor,
        X56_4_Module15_Escape_Rear_Right_Forward_Sensor,
        X56_5_Module15_Escape_Rear_Right_Reward_Sensor,
        X56_6_Module15_Power_On,
        X56_7_Module15_Pressure_Check,
        X57_0_Module16_Clamp_Forward_Sensor,
        X57_1_Module16_Clamp_Reward_Sensor,
        X57_2_Module16_Module_Sensor,
        X57_3_Module16_PCB_Sensor,
        X57_4_Module16_Front_Left_Up_Sensor,
        X57_5_Module16_Front_Left_Down_Sensor,
        X57_6_Module16_Front_Right_Up_Sensor,
        X57_7_Module16_Front_Right_Down_Sensor,
        X58_0_Module16_Rear_Left_Up_Sensor,
        X58_1_Module16_Rear_Left_Down_Sensor,
        X58_2_Module16_Rear_Right_Up_Sensor,
        X58_3_Module16_Rear_Right_Down_Sensor,
        X58_4_Module16_Forward_Sensor,
        X58_5_Module16_Reward_Sensor,
        X58_6_Module16_Escape_Front_Left_Forward_Sensor,
        X58_7_Module16_Escape_Front_Left_Reward_Sensor,
        X59_0_Module16_Escape_Front_Right_Forward_Sensor,
        X59_1_Module16_Escape_Front_Right_Reward_Sensor,
        X59_2_Module16_Escape_Rear_Left_Forward_Sensor,
        X59_3_Module16_Escape_Rear_Left_Reward_Sensor,
        X59_4_Module16_Escape_Rear_Right_Forward_Sensor,
        X59_5_Module16_Escape_Rear_Right_Reward_Sensor,
        X59_6_Module16_Power_On,
        X59_7_Module16_Pressure_Check,
        X60_0_Module17_Clamp_Forward_Sensor,
        X60_1_Module17_Clamp_Reward_Sensor,
        X60_2_Module17_Module_Sensor,
        X60_3_Module17_PCB_Sensor,
        X60_4_Module17_Front_Left_Up_Sensor,
        X60_5_Module17_Front_Left_Down_Sensor,
        X60_6_Module17_Front_Right_Up_Sensor,
        X60_7_Module17_Front_Right_Down_Sensor,
        X61_0_Module17_Rear_Left_Up_Sensor,
        X61_1_Module17_Rear_Left_Down_Sensor,
        X61_2_Module17_Rear_Right_Up_Sensor,
        X61_3_Module17_Rear_Right_Down_Sensor,
        X61_4_Module17_Forward_Sensor,
        X61_5_Module17_Reward_Sensor,
        X61_6_Module17_Escape_Front_Left_Forward_Sensor,
        X61_7_Module17_Escape_Front_Left_Reward_Sensor,
        X62_0_Module17_Escape_Front_Right_Forward_Sensor,
        X62_1_Module17_Escape_Front_Right_Reward_Sensor,
        X62_2_Module17_Escape_Rear_Left_Forward_Sensor,
        X62_3_Module17_Escape_Rear_Left_Reward_Sensor,
        X62_4_Module17_Escape_Rear_Right_Forward_Sensor,
        X62_5_Module17_Escape_Rear_Right_Reward_Sensor,
        X62_6_Module17_Power_On,
        X62_7_Module17_Pressure_Check,
        X63_0_Module18_Clamp_Forward_Sensor,
        X63_1_Module18_Clamp_Reward_Sensor,
        X63_2_Module18_Module_Sensor,
        X63_3_Module18_PCB_Sensor,
        X63_4_Module18_Front_Left_Up_Sensor,
        X63_5_Module18_Front_Left_Down_Sensor,
        X63_6_Module18_Front_Right_Up_Sensor,
        X63_7_Module18_Front_Right_Down_Sensor,
        X64_0_Module18_Rear_Left_Up_Sensor,
        X64_1_Module18_Rear_Left_Down_Sensor,
        X64_2_Module18_Rear_Right_Up_Sensor,
        X64_3_Module18_Rear_Right_Down_Sensor,
        X64_4_Module18_Forward_Sensor,
        X64_5_Module18_Reward_Sensor,
        X64_6_Module18_Escape_Front_Left_Forward_Sensor,
        X64_7_Module18_Escape_Front_Left_Reward_Sensor,
        X65_0_Module18_Escape_Front_Right_Forward_Sensor,
        X65_1_Module18_Escape_Front_Right_Reward_Sensor,
        X65_2_Module18_Escape_Rear_Left_Forward_Sensor,
        X65_3_Module18_Escape_Rear_Left_Reward_Sensor,
        X65_4_Module18_Escape_Rear_Right_Forward_Sensor,
        X65_5_Module18_Escape_Rear_Right_Reward_Sensor,
        X65_6_Module18_Power_On,
        X65_7_Module18_Pressure_Check,
        X66_0_Module19_Clamp_Forward_Sensor,
        X66_1_Module19_Clamp_Reward_Sensor,
        X66_2_Module19_Module_Sensor,
        X66_3_Module19_PCB_Sensor,
        X66_4_Module19_Front_Left_Up_Sensor,
        X66_5_Module19_Front_Left_Down_Sensor,
        X66_6_Module19_Front_Right_Up_Sensor,
        X66_7_Module19_Front_Right_Down_Sensor,
        X67_0_Module19_Rear_Left_Up_Sensor,
        X67_1_Module19_Rear_Left_Down_Sensor,
        X67_2_Module19_Rear_Right_Up_Sensor,
        X67_3_Module19_Rear_Right_Down_Sensor,
        X67_4_Module19_Forward_Sensor,
        X67_5_Module19_Reward_Sensor,
        X67_6_Module19_Escape_Front_Left_Forward_Sensor,
        X67_7_Module19_Escape_Front_Left_Reward_Sensor,
        X68_0_Module19_Escape_Front_Right_Forward_Sensor,
        X68_1_Module19_Escape_Front_Right_Reward_Sensor,
        X68_2_Module19_Escape_Rear_Left_Forward_Sensor,
        X68_3_Module19_Escape_Rear_Left_Reward_Sensor,
        X68_4_Module19_Escape_Rear_Right_Forward_Sensor,
        X68_5_Module19_Escape_Rear_Right_Reward_Sensor,
        X68_6_Module19_Power_On,
        X68_7_Module19_Pressure_Check,
        X69_0_Module20_Clamp_Forward_Sensor,
        X69_1_Module20_Clamp_Reward_Sensor,
        X69_2_Module20_Module_Sensor,
        X69_3_Module20_PCB_Sensor,
        X69_4_Module20_Front_Left_Up_Sensor,
        X69_5_Module20_Front_Left_Down_Sensor,
        X69_6_Module20_Front_Right_Up_Sensor,
        X69_7_Module20_Front_Right_Down_Sensor,
        X70_0_Module20_Rear_Left_Up_Sensor,
        X70_1_Module20_Rear_Left_Down_Sensor,
        X70_2_Module20_Rear_Right_Up_Sensor,
        X70_3_Module20_Rear_Right_Down_Sensor,
        X70_4_Module20_Forward_Sensor,
        X70_5_Module20_Reward_Sensor,
        X70_6_Module20_Escape_Front_Left_Forward_Sensor,
        X70_7_Module20_Escape_Front_Left_Reward_Sensor,
        X71_0_Module20_Escape_Front_Right_Forward_Sensor,
        X71_1_Module20_Escape_Front_Right_Reward_Sensor,
        X71_2_Module20_Escape_Rear_Left_Forward_Sensor,
        X71_3_Module20_Escape_Rear_Left_Reward_Sensor,
        X71_4_Module20_Escape_Rear_Right_Forward_Sensor,
        X71_5_Module20_Escape_Rear_Right_Reward_Sensor,
        X71_6_Module20_Power_On,
        X71_7_Module20_Pressure_Check,
        X72_0_Module21_Clamp_Forward_Sensor,
        X72_1_Module21_Clamp_Reward_Sensor,
        X72_2_Module21_Module_Sensor,
        X72_3_Module21_PCB_Sensor,
        X72_4_Module21_Front_Left_Up_Sensor,
        X72_5_Module21_Front_Left_Down_Sensor,
        X72_6_Module21_Front_Right_Up_Sensor,
        X72_7_Module21_Front_Right_Down_Sensor,
        X73_0_Module21_Rear_Left_Up_Sensor,
        X73_1_Module21_Rear_Left_Down_Sensor,
        X73_2_Module21_Rear_Right_Up_Sensor,
        X73_3_Module21_Rear_Right_Down_Sensor,
        X73_4_Module21_Forward_Sensor,
        X73_5_Module21_Reward_Sensor,
        X73_6_Module21_Escape_Front_Left_Forward_Sensor,
        X73_7_Module21_Escape_Front_Left_Reward_Sensor,
        X74_0_Module21_Escape_Front_Right_Forward_Sensor,
        X74_1_Module21_Escape_Front_Right_Reward_Sensor,
        X74_2_Module21_Escape_Rear_Left_Forward_Sensor,
        X74_3_Module21_Escape_Rear_Left_Reward_Sensor,
        X74_4_Module21_Escape_Rear_Right_Forward_Sensor,
        X74_5_Module21_Escape_Rear_Right_Reward_Sensor,
        X74_6_Module21_Power_On,
        X74_7_Module21_Pressure_Check
        */
        #endregion
    }

    /** @brief 디지털 출력점 이름. 프로젝트별로 func프로젝트명.cs에서 별도 선언할 것. */
    public enum enumDONames // 디지털 출력점 이름
    {
        Y00_0_SPINDLE_1_ON,
        Y00_1_SPINDLE_2_ON,
        Y00_2_DUST_COLLECTOR_ON,
        Y00_3_,
        Y00_4_,
        Y00_5_,
        Y00_6_,
        Y00_7_,
        Y01_0_EMGENCY_LAMP,
        Y01_1_START_LAMP,
        Y01_2_STOP_LAMP,
        Y01_3_RESET_LAMP,
        Y01_4_,
        Y01_5_,
        Y01_6_,
        Y01_7_Ionizer_On,
        Y02_0_TOWER_LAMP_RED,
        Y02_1_TOWER_LAMP_YELLOW,
        Y02_2_TOWER_LAMP_GREEN,
        Y412_2_Tower_Lamp_Buzzer,
        Y02_4_,
        Y02_5_PCBInput_Magazine_Clamp,
        Y02_6_BoxIN_Magazine_Clamp,
        Y02_7_BoxOUT_Magazine_Clamp,
        Y03_0_PCBInput_Push_Cylinder,
        Y03_1_InConver_Motor,
        Y03_2_InConver_PCB_Clamp,
        Y03_3_Lift_BoxInPut_Push_Cylinder,
        Y03_4_Box_Conveyor_Motor,
        Y03_5_Box_Clamp,
        Y03_6_Box_Out_Push_Cylinder,
        Y03_7_Box_Stopper_Cylinder,
        Y04_0_Array_Vacuum_1,
        Y04_1_Array_Vacuum_2,
        Y04_2_Array_Vacuum_3,
        Y04_3_Array_Vacuum_4,
        Y04_4_Array_Vacuum_5,
        Y04_5_Scrap_Vacuum_1,
        Y04_6_PCBInput_Push_1_Cylinder,
        Y04_7_Lift_BoxInPut_Push_1_Cylinder,
        Y05_0_BoxWait_Stopper_Cylinder,
        Y05_1_PCBInput_Magazine_Max_Clamp,
        Y05_2_BoxIN_Magazine_Max_Clamp,
        Y05_3_BoxOUT_Magazine_Max_Clamp,
        Y05_4_,
        Y05_5_,
        Y05_6_,
        Y05_7_,
        Y06_0_,
        Y06_1_,
        Y06_2_,
        Y06_3_,
        Y06_4_,
        Y06_5_,
        Y06_6_,
        Y06_7_,
        Y07_0_,
        Y07_1_,
        Y07_2_,
        Y07_3_,
        Y07_4_,
        Y07_5_,
        Y07_6_,
        Y07_7_,
        #region 아래로 사용안함
        /*
        Y08_0_,
        Y08_1_,
        Y08_2_,
        Y08_3_,
        Y08_4_,
        Y08_5_,
        Y08_6_,
        Y08_7_,
        Y09_0_Module1_Front_Up,
        Y09_1_Module1_Rear_Up,
        Y09_2_Module1_Forward,
        Y09_3_Module1_Reward,
        Y09_4_Module1_Escape_Forward,
        Y09_5_Module1_Conveyor_Run,
        Y09_6_Module1_Fan_Run,
        Y09_7_Module1_Clamp,
        Y10_0_Module2_Front_Up,
        Y10_1_Module2_Rear_Up,
        Y10_2_Module2_Forward,
        Y10_3_Module2_Reward,
        Y10_4_Module2_Escape_Forward,
        Y10_5_Module2_Conveyor_Run,
        Y10_6_Module2_Fan_Run,
        Y10_7_Module2_Clamp,
        Y11_0_Module3_Front_Up,
        Y11_1_Module3_Rear_Up,
        Y11_2_Module3_Forward,
        Y11_3_Module3_Reward,
        Y11_4_Module3_Escape_Forward,
        Y11_5_Module3_Conveyor_Run,
        Y11_6_Module3_Fan_Run,
        Y11_7_Module3_Clamp,
        Y12_0_Module4_Front_Up,
        Y12_1_Module4_Rear_Up,
        Y12_2_Module4_Forward,
        Y12_3_Module4_Reward,
        Y12_4_Module4_Escape_Forward,
        Y12_5_Module4_Conveyor_Run,
        Y12_6_Module4_Fan_Run,
        Y12_7_Module4_Clamp,
        Y13_0_Module5_Front_Up,
        Y13_1_Module5_Rear_Up,
        Y13_2_Module5_Forward,
        Y13_3_Module5_Reward,
        Y13_4_Module5_Escape_Forward,
        Y13_5_Module5_Conveyor_Run,
        Y13_6_Module5_Fan_Run,
        Y13_7_Module5_Clamp,
        Y14_0_Module6_Front_Up,
        Y14_1_Module6_Rear_Up,
        Y14_2_Module6_Forward,
        Y14_3_Module6_Reward,
        Y14_4_Module6_Escape_Forward,
        Y14_5_Module6_Conveyor_Run,
        Y14_6_Module6_Fan_Run,
        Y14_7_Module6_Clamp,
        Y15_0_Module7_Front_Up,
        Y15_1_Module7_Rear_Up,
        Y15_2_Module7_Forward,
        Y15_3_Module7_Reward,
        Y15_4_Module7_Escape_Forward,
        Y15_5_Module7_Conveyor_Run,
        Y15_6_Module7_Fan_Run,
        Y15_7_Module7_Clamp,
        Y16_0_Module8_Front_Up,
        Y16_1_Module8_Rear_Up,
        Y16_2_Module8_Forward,
        Y16_3_Module8_Reward,
        Y16_4_Module8_Escape_Forward,
        Y16_5_Module8_Conveyor_Run,
        Y16_6_Module8_Fan_Run,
        Y16_7_Module8_Clamp,
        Y17_0_Module9_Front_Up,
        Y17_1_Module9_Rear_Up,
        Y17_2_Module9_Forward,
        Y17_3_Module9_Reward,
        Y17_4_Module9_Escape_Forward,
        Y17_5_Module9_Conveyor_Run,
        Y17_6_Module9_Fan_Run,
        Y17_7_Module9_Clamp,
        Y18_0_Module10_Front_Up,
        Y18_1_Module10_Rear_Up,
        Y18_2_Module10_Forward,
        Y18_3_Module10_Reward,
        Y18_4_Module10_Escape_Forward,
        Y18_5_Module10_Conveyor_Run,
        Y18_6_Module10_Fan_Run,
        Y18_7_Module10_Clamp,
        Y19_0_Module11_Front_Up,
        Y19_1_Module11_Rear_Up,
        Y19_2_Module11_Forward,
        Y19_3_Module11_Reward,
        Y19_4_Module11_Escape_Forward,
        Y19_5_Module11_Conveyor_Run,
        Y19_6_Module11_Fan_Run,
        Y19_7_Module11_Clamp,
        Y20_0_Module12_Front_Up,
        Y20_1_Module12_Rear_Up,
        Y20_2_Module12_Forward,
        Y20_3_Module12_Reward,
        Y20_4_Module12_Escape_Forward,
        Y20_5_Module12_Conveyor_Run,
        Y20_6_Module12_Fan_Run,
        Y20_7_Module12_Clamp,
        Y21_0_Module13_Front_Up,
        Y21_1_Module13_Rear_Up,
        Y21_2_Module13_Forward,
        Y21_3_Module13_Reward,
        Y21_4_Module13_Escape_Forward,
        Y21_5_Module13_Conveyor_Run,
        Y21_6_Module13_Fan_Run,
        Y21_7_Module13_Clamp,
        Y22_0_Module14_Front_Up,
        Y22_1_Module14_Rear_Up,
        Y22_2_Module14_Forward,
        Y22_3_Module14_Reward,
        Y22_4_Module14_Escape_Forward,
        Y22_5_Module14_Conveyor_Run,
        Y22_6_Module14_Fan_Run,
        Y22_7_Module14_Clamp,
        Y23_0_Module15_Front_Up,
        Y23_1_Module15_Rear_Up,
        Y23_2_Module15_Forward,
        Y23_3_Module15_Reward,
        Y23_4_Module15_Escape_Forward,
        Y23_5_Module15_Conveyor_Run,
        Y23_6_Module15_Fan_Run,
        Y23_7_Module15_Clamp,
        Y24_0_Module16_Front_Up,
        Y24_1_Module16_Rear_Up,
        Y24_2_Module16_Forward,
        Y24_3_Module16_Reward,
        Y24_4_Module16_Escape_Forward,
        Y24_5_Module16_Conveyor_Run,
        Y24_6_Module16_Fan_Run,
        Y24_7_Module16_Clamp,
        Y25_0_Module17_Front_Up,
        Y25_1_Module17_Rear_Up,
        Y25_2_Module17_Forward,
        Y25_3_Module17_Reward,
        Y25_4_Module17_Escape_Forward,
        Y25_5_Module17_Conveyor_Run,
        Y25_6_Module17_Fan_Run,
        Y25_7_Module17_Clamp,
        Y26_0_Module18_Front_Up,
        Y26_1_Module18_Rear_Up,
        Y26_2_Module18_Forward,
        Y26_3_Module18_Reward,
        Y26_4_Module18_Escape_Forward,
        Y26_5_Module18_Conveyor_Run,
        Y26_6_Module18_Fan_Run,
        Y26_7_Module18_Clamp,
        Y27_0_Module19_Front_Up,
        Y27_1_Module19_Rear_Up,
        Y27_2_Module19_Forward,
        Y27_3_Module19_Reward,
        Y27_4_Module19_Escape_Forward,
        Y27_5_Module19_Conveyor_Run,
        Y27_6_Module19_Fan_Run,
        Y27_7_Module19_Clamp,
        Y28_0_Module20_Front_Up,
        Y28_1_Module20_Rear_Up,
        Y28_2_Module20_Forward,
        Y28_3_Module20_Reward,
        Y28_4_Module20_Escape_Forward,
        Y28_5_Module20_Conveyor_Run,
        Y28_6_Module20_Fan_Run,
        Y28_7_Module20_Clamp,
        Y29_0_Module21_Front_Up,
        Y29_1_Module21_Rear_Up,
        Y29_2_Module21_Forward,
        Y29_3_Module21_Reward,
        Y29_4_Module21_Escape_Forward,
        Y29_5_Module21_Conveyor_Run,
        Y29_6_Module21_Fan_Run,
        Y29_7_Module21_Clamp
        */
        #endregion
    }

    #endregion

    #region Device Net 입출력
    /** @brief DeviceNet 입력점 이름. 프로젝트별로 func프로젝트명.cs에서 별도 선언할 것. */
    public enum enumDnetINames // DeviceNet 입력점 이름
    {
        DI_000_Alwats_On,
        DI_001_,
        DI_002_,
        DI_003_,
        DI_004_,
        DI_005_,
        DI_006_,
        DI_007_,
        DI_008_Job_Select_Completed,
        DI_009_Job_Select_Ack_1,
        DI_010_Job_Select_Ack_2,
        DI_011_,
        DI_012_,
        DI_013_Step_Set_Alarm,
        DI_014_Mode_Select_Remote,
        DI_015_Mode_Select_Manual,
        DI_016_Low_Battery_Voltage,
        DI_017_Robot_Operation_Error,
        DI_018_Robot_Warning,
        DI_019_Motor_On,
        DI_020_Robot_Emergency_Stop,
        DI_021_External_Reset_ACK,
        DI_022_Robot_Moving,
        DI_023_Holding,
        DI_024_Robot_Error_Code_Stobe,
        DI_025_Program_End,
        DI_026_Robot_Alarm_Error,
        DI_027_Interlock_Alarm,
        DI_028_Robot_Running_Program,
        DI_029_Mode_Select_Auto,
        DI_030_Robot_Ready_OK,
        DI_031_Home_Position,
        DI_032_,
        DI_033_,
        DI_034_,
        DI_035_,
        DI_036_,
        DI_037_,
        DI_038_,
        DI_039_,
        DI_040_Tool_Get_Request_Sol_On,
        DI_041_Tool_Put_Request_Sol_off,
        DI_042_,
        DI_043_,
        DI_044_,
        DI_045_,
        DI_046_Process_Start_Ready,
        DI_047_Process_Error,
        DI_048_Process_1_Running,
        DI_049_Process_2_Running,
        DI_050_Process_3_Running,
        DI_051_Process_4_Running,
        DI_052_Process_5_Running,
        DI_053_Process_6_Running,
        DI_054_Process_7_Running,
        DI_055_Process_8_Running,
        DI_056_Process_9_Running,
        DI_057_Process_10_Running,
        DI_058_,
        DI_059_,
        DI_060_,
        DI_061_,
        DI_062_,
        DI_063_,
        DI_064_Process_1_Completed,
        DI_065_Process_2_Completed,
        DI_066_Process_3_Completed,
        DI_067_Process_4_Completed,
        DI_068_Process_5_Completed,
        DI_069_Process_6_Completed,
        DI_070_Process_7_Completed,
        DI_071_Process_8_Completed,
        DI_072_Process_9_Completed,
        DI_073_Process_10_Completed,
        DI_074_,
        DI_075_,
        DI_076_,
        DI_077_,
        DI_078_,
        DI_079_,
        DI_080_,
        DI_081_,
        DI_082_,
        DI_083_,
        DI_084_,
        DI_085_,
        DI_086_,
        DI_087_,
        DI_088_,
        DI_089_,
        DI_090_,
        DI_091_,
        DI_092_,
        DI_093_,
        DI_094_,
        DI_095_,
        DI_096_Process_Error_Code,
        DI_097_Process_Error_Code,
        DI_098_Process_Error_Code,
        DI_099_Process_Error_Code,
        DI_100_Process_Error_Code,
        DI_101_,
        DI_102_,
        DI_103_,
        DI_104_Robot_System_Error_Code,
        DI_105_Robot_System_Error_Code,
        DI_106_Robot_System_Error_Code,
        DI_107_Robot_System_Error_Code,
        DI_108_Robot_System_Error_Code,
        DI_109_Robot_System_Error_Code,
        DI_110_Robot_System_Error_Code,
        DI_111_Robot_System_Error_Code,
        DI_112_Robot_System_Error_Code,
        DI_113_Robot_System_Error_Code,
        DI_114_Robot_System_Error_Code,
        DI_115_Robot_System_Error_Code,
        DI_116_Robot_System_Error_Code,
        DI_117_Robot_System_Error_Code,
        DI_118_Robot_System_Error_Code,
        DI_119_Robot_System_Error_Code,
        DI_120_,
        DI_121_,
        DI_122_,
        DI_123_,
        DI_124_,
        DI_125_,
        DI_126_,
        DI_127_,
        DI_128_,
        DI_129_,
        DI_130_,
        DI_131_,
        DI_132_,
        DI_133_,
        DI_134_,
        DI_135_,
        DI_136_,
        DI_137_,
        DI_138_,
        DI_139_,
        DI_140_,
        DI_141_,
        DI_142_,
        DI_143_,
        DI_144_,
        DI_145_,
        DI_146_,
        DI_147_,
        DI_148_,
        DI_149_,
        DI_150_,
        DI_151_,
        DI_152_,
        DI_153_,
        DI_154_,
        DI_155_,
        DI_156_,
        DI_157_,
        DI_158_,
        DI_159_,
        DI_160_,
        DI_161_,
        DI_162_,
        DI_163_,
        DI_164_,
        DI_165_,
        DI_166_,
        DI_167_,
        DI_168_,
        DI_169_,
        DI_170_,
        DI_171_,
        DI_172_,
        DI_173_,
        DI_174_,
        DI_175_,
        DI_176_,
        DI_177_,
        DI_178_,
        DI_179_,
        DI_180_,
        DI_181_,
        DI_182_,
        DI_183_,
        DI_184_,
        DI_185_,
        DI_186_,
        DI_187_,
        DI_188_,
        DI_189_,
        DI_190_,
        DI_191_,
        DI_192_,
        DI_193_,
        DI_194_,
        DI_195_,
        DI_196_,
        DI_197_,
        DI_198_,
        DI_199_,
        DI_200_,
        DI_201_,
        DI_202_,
        DI_203_,
        DI_204_,
        DI_205_,
        DI_206_,
        DI_207_,
        DI_208_,
        DI_209_,
        DI_210_,
        DI_211_,
        DI_212_,
        DI_213_,
        DI_214_,
        DI_215_,
        DI_216_,
        DI_217_,
        DI_218_,
        DI_219_,
        DI_220_,
        DI_221_,
        DI_222_,
        DI_223_,
        DI_224_,
        DI_225_,
        DI_226_,
        DI_227_,
        DI_228_,
        DI_229_,
        DI_230_,
        DI_231_,
        DI_232_,
        DI_233_,
        DI_234_,
        DI_235_,
        DI_236_,
        DI_237_,
        DI_238_,
        DI_239_,
        DI_240_,
        DI_241_,
        DI_242_,
        DI_243_,
        DI_244_,
        DI_245_,
        DI_246_,
        DI_247_,
        DI_248_,
        DI_249_,
        DI_250_,
        DI_251_,
        DI_252_,
        DI_253_,
        DI_254_,
        DI_255_,
        DI_256_,
        DI_257_,
        DI_258_,
        DI_259_,
        DI_260_,
        DI_261_,
        DI_262_,
        DI_263_,
        DI_264_,
        DI_265_,
        DI_266_,
        DI_267_,
        DI_268_,
        DI_269_,
        DI_270_,
        DI_271_,
        DI_272_,
        DI_273_,
        DI_274_,
        DI_275_,
        DI_276_,
        DI_277_,
        DI_278_,
        DI_279_,
        DI_280_,
        DI_281_,
        DI_282_,
        DI_283_,
        DI_284_,
        DI_285_,
        DI_286_,
        DI_287_,
        DI_288_,
        DI_289_,
        DI_290_,
        DI_291_,
        DI_292_,
        DI_293_,
        DI_294_,
        DI_295_,
        DI_296_,
        DI_297_,
        DI_298_,
        DI_299_,
        DI_300_,
        DI_301_,
        DI_302_,
        DI_303_,
        DI_304_,
        DI_305_,
        DI_306_,
        DI_307_,
        DI_308_,
        DI_309_,
        DI_310_,
        DI_311_,
        DI_312_,
        DI_313_,
        DI_314_,
        DI_315_,
        DI_316_,
        DI_317_,
        DI_318_,
        DI_319_,
        DI_320_,
        DI_321_,
        DI_322_,
        DI_323_,
        DI_324_,
        DI_325_,
        DI_326_,
        DI_327_,
        DI_328_,
        DI_329_,
        DI_330_,
        DI_331_,
        DI_332_,
        DI_333_,
        DI_334_,
        DI_335_,
        DI_336_,
        DI_337_,
        DI_338_,
        DI_339_,
        DI_340_,
        DI_341_,
        DI_342_,
        DI_343_,
        DI_344_,
        DI_345_,
        DI_346_,
        DI_347_,
        DI_348_,
        DI_349_,
        DI_350_,
        DI_351_,
        DI_352_,
        DI_353_,
        DI_354_,
        DI_355_,
        DI_356_,
        DI_357_,
        DI_358_,
        DI_359_,
        DI_360_,
        DI_361_,
        DI_362_,
        DI_363_,
        DI_364_,
        DI_365_,
        DI_366_,
        DI_367_,
        DI_368_,
        DI_369_,
        DI_370_,
        DI_371_,
        DI_372_,
        DI_373_,
        DI_374_,
        DI_375_,
        DI_376_,
        DI_377_,
        DI_378_,
        DI_379_,
        DI_380_,
        DI_381_,
        DI_382_,
        DI_383_,
        DI_384_,
        DI_385_,
        DI_386_,
        DI_387_,
        DI_388_,
        DI_389_,
        DI_390_,
        DI_391_,
        DI_392_,
        DI_393_,
        DI_394_,
        DI_395_,
        DI_396_,
        DI_397_,
        DI_398_,
        DI_399_,
        DI_400_,
        DI_401_,
        DI_402_,
        DI_403_,
        DI_404_,
        DI_405_,
        DI_406_,
        DI_407_,
        DI_408_,
        DI_409_,
        DI_410_,
        DI_411_,
        DI_412_,
        DI_413_,
        DI_414_,
        DI_415_,
        DI_416_,
        DI_417_,
        DI_418_,
        DI_419_,
        DI_420_,
        DI_421_,
        DI_422_,
        DI_423_,
        DI_424_,
        DI_425_,
        DI_426_,
        DI_427_,
        DI_428_,
        DI_429_,
        DI_430_,
        DI_431_,
        DI_432_,
        DI_433_,
        DI_434_,
        DI_435_,
        DI_436_,
        DI_437_,
        DI_438_,
        DI_439_,
        DI_440_,
        DI_441_,
        DI_442_,
        DI_443_,
        DI_444_,
        DI_445_,
        DI_446_,
        DI_447_,
        DI_448_,
        DI_449_,
        DI_450_,
        DI_451_,
        DI_452_,
        DI_453_,
        DI_454_,
        DI_455_,
        DI_456_,
        DI_457_,
        DI_458_,
        DI_459_,
        DI_460_,
        DI_461_,
        DI_462_,
        DI_463_,
        DI_464_,
        DI_465_,
        DI_466_,
        DI_467_,
        DI_468_,
        DI_469_,
        DI_470_,
        DI_471_,
        DI_472_,
        DI_473_,
        DI_474_,
        DI_475_,
        DI_476_,
        DI_477_,
        DI_478_,
        DI_479_,
        DI_480_,
        DI_481_,
        DI_482_,
        DI_483_,
        DI_484_,
        DI_485_,
        DI_486_,
        DI_487_,
        DI_488_,
        DI_489_,
        DI_490_,
        DI_491_,
        DI_492_,
        DI_493_,
        DI_494_,
        DI_495_,
        DI_496_,
        DI_497_,
        DI_498_,
        DI_499_,
        DI_500_,
        DI_501_,
        DI_502_,
        DI_503_,
        DI_504_,
        DI_505_,
        DI_506_,
        DI_507_,
        DI_508_,
        DI_509_,
        DI_510_,
        DI_511_,
        DI_512_,
        DI_513_,
        DI_514_,
        DI_515_,
        DI_516_,
        DI_517_,
        DI_518_,
        DI_519_,
        DI_520_,
        DI_521_,
        DI_522_,
        DI_523_,
        DI_524_,
        DI_525_,
        DI_526_,
        DI_527_,
        DI_528_,
        DI_529_,
        DI_530_,
        DI_531_,
        DI_532_,
        DI_533_,
        DI_534_,
        DI_535_,
        DI_536_,
        DI_537_,
        DI_538_,
        DI_539_,
        DI_540_,
        DI_541_,
        DI_542_,
        DI_543_,
        DI_544_,
        DI_545_,
        DI_546_,
        DI_547_,
        DI_548_,
        DI_549_,
        DI_550_,
        DI_551_,
        DI_552_,
        DI_553_,
        DI_554_,
        DI_555_,
        DI_556_,
        DI_557_,
        DI_558_,
        DI_559_,
        DI_560_,
        DI_561_,
        DI_562_,
        DI_563_,
        DI_564_,
        DI_565_,
        DI_566_,
        DI_567_,
        DI_568_,
        DI_569_,
        DI_570_,
        DI_571_,
        DI_572_,
        DI_573_,
        DI_574_,
        DI_575_,
        DI_576_,
        DI_577_,
        DI_578_,
        DI_579_,
        DI_580_,
        DI_581_,
        DI_582_,
        DI_583_,
        DI_584_,
        DI_585_,
        DI_586_,
        DI_587_,
        DI_588_,
        DI_589_,
        DI_590_,
        DI_591_,
        DI_592_,
        DI_593_,
        DI_594_,
        DI_595_,
        DI_596_,
        DI_597_,
        DI_598_,
        DI_599_,
        DI_600_,
        DI_601_,
        DI_602_,
        DI_603_,
        DI_604_,
        DI_605_,
        DI_606_,
        DI_607_,
        DI_608_,
        DI_609_,
        DI_610_,
        DI_611_,
        DI_612_,
        DI_613_,
        DI_614_,
        DI_615_,
        DI_616_,
        DI_617_,
        DI_618_,
        DI_619_,
        DI_620_,
        DI_621_,
        DI_622_,
        DI_623_,
        DI_624_,
        DI_625_,
        DI_626_,
        DI_627_,
        DI_628_,
        DI_629_,
        DI_630_,
        DI_631_,
        DI_632_,
        DI_633_,
        DI_634_,
        DI_635_,
        DI_636_,
        DI_637_,
        DI_638_,
        DI_639_
    }

    /** @brief DeviceNet 출력점 이름. 프로젝트별로 func프로젝트명.cs에서 별도 선언할 것. */
    public enum enumDnetONames // DeviceNet 출력점 이름
    {
        DO_000_Always_On,
        DO_001_,
        DO_002_,
        DO_003_,
        DO_004_,
        DO_005_,
        DO_006_,
        DO_007_,
        DO_008_Job_Select_Order,
        DO_009_Job_Select_1,
        DO_010_Job_Select_2,
        DO_011_,
        DO_012_,
        DO_013_,
        DO_014_,
        DO_015_Home_Position,
        DO_016_,
        DO_017_Cycle_End,
        DO_018_Alarm_Reset,
        DO_019_Motor_On_External,
        DO_020_Motor_Off_External,
        DO_021_External_Reset,
        DO_022_External_Start,
        DO_023_Hold_External,
        DO_024_Main_Program_Select,
        DO_025_Program_Select_Bit,
        DO_026_Program_Select_Bit,
        DO_027_Program_Select_Bit,
        DO_028_Program_Select_Bit,
        DO_029_Program_Select_Bit,
        DO_030_Program_Select_Bit,
        DO_031_Program_Select_Bit,
        DO_032_Robot_Speed_1,
        DO_033_Robot_Speed_2,
        DO_034_Robot_Speed_4,
        DO_035_Robot_Speed_8,
        DO_036_Robot_Speed_16,
        DO_037_Robot_Speed_32,
        DO_038_Robot_Speed_64,
        DO_039_Robot_Speed_128,
        DO_040_Tool_Get_Sol_On,
        DO_041_Tool_Put_Sol_Off,
        DO_042_,
        DO_043_,
        DO_044_,
        DO_045_,
        DO_046_,
        DO_047_Process_Error_Check,
        DO_048_Process_1_Start,
        DO_049_Process_2_Start,
        DO_050_Process_3_Start,
        DO_051_Process_4_Start,
        DO_052_Process_5_Start,
        DO_053_Process_6_Start,
        DO_054_Process_7_Start,
        DO_055_Process_8_Start,
        DO_056_Process_9_Start,
        DO_057_Process_10_Start,
        DO_058_,
        DO_059_,
        DO_060_,
        DO_061_,
        DO_062_,
        DO_063_,
        DO_064_Process_1_Completed_Check,
        DO_065_Process_2_Completed_Check,
        DO_066_Process_3_Completed_Check,
        DO_067_Process_4_Completed_Check,
        DO_068_Process_5_Completed_Check,
        DO_069_Process_6_Completed_Check,
        DO_070_Process_7_Completed_Check,
        DO_071_Process_8_Completed_Check,
        DO_072_Process_9_Completed_Check,
        DO_073_Process_10_Completed_Check,
        DO_074_,
        DO_075_,
        DO_076_,
        DO_077_,
        DO_078_,
        DO_079_,
        DO_080_X_Offset_1,
        DO_081_X_Offset_2,
        DO_082_X_Offset_4,
        DO_083_X_Offset_8,
        DO_084_X_Offset_16,
        DO_085_X_Offset_32,
        DO_086_X_Offset_64,
        DO_087_X_Offset_128,
        DO_088_X_Offset_256,
        DO_089_X_Offset_512,
        DO_090_X_Offset_1024,
        DO_091_X_Offset_2048,
        DO_092_X_Offset_4096,
        DO_093_X_Offset_8192,
        DO_094_X_Offset_16384,
        DO_095_X_Offset_symbol,
        DO_096_Y_Offset_1,
        DO_097_Y_Offset_2,
        DO_098_Y_Offset_4,
        DO_099_Y_Offset_8,
        DO_100_Y_Offset_16,
        DO_101_Y_Offset_32,
        DO_102_Y_Offset_64,
        DO_103_Y_Offset_128,
        DO_104_Y_Offset_256,
        DO_105_Y_Offset_512,
        DO_106_Y_Offset_1024,
        DO_107_Y_Offset_2048,
        DO_108_Y_Offset_4096,
        DO_109_Y_Offset_8192,
        DO_110_Y_Offset_16384,
        DO_111_Y_Offset_symbol,
        DO_112_Z_Offset_1,
        DO_113_Z_Offset_2,
        DO_114_Z_Offset_4,
        DO_115_Z_Offset_8,
        DO_116_Z_Offset_16,
        DO_117_Z_Offset_32,
        DO_118_Z_Offset_64,
        DO_119_Z_Offset_128,
        DO_120_Z_Offset_256,
        DO_121_Z_Offset_512,
        DO_122_Z_Offset_1024,
        DO_123_Z_Offset_2048,
        DO_124_Z_Offset_4096,
        DO_125_Z_Offset_8192,
        DO_126_Z_Offset_16384,
        DO_127_Z_Offset_symbol,
        DO_128_FuncCheck_01,
        DO_129_FuncCheck_02,
        DO_130_FuncCheck_03,
        DO_131_FuncCheck_04,
        DO_132_FuncCheck_05,
        DO_133_FuncCheck_06,
        DO_134_FuncCheck_07,
        DO_135_FuncCheck_08,
        DO_136_FuncCheck_09,
        DO_137_FuncCheck_11,
        DO_138_FuncCheck_11,
        DO_139_FuncCheck_12,
        DO_140_FuncCheck_13,
        DO_141_FuncCheck_14,
        DO_142_FuncCheck_15,
        DO_143_FuncCheck_16,
        DO_144_,
        DO_145_,
        DO_146_,
        DO_147_,
        DO_148_,
        DO_149_,
        DO_150_,
        DO_151_,
        DO_152_,
        DO_153_,
        DO_154_,
        DO_155_,
        DO_156_,
        DO_157_,
        DO_158_,
        DO_159_,
        DO_160_,
        DO_161_,
        DO_162_,
        DO_163_,
        DO_164_,
        DO_165_,
        DO_166_,
        DO_167_,
        DO_168_,
        DO_169_,
        DO_170_,
        DO_171_,
        DO_172_,
        DO_173_,
        DO_174_,
        DO_175_,
        DO_176_,
        DO_177_,
        DO_178_,
        DO_179_,
        DO_180_,
        DO_181_,
        DO_182_,
        DO_183_,
        DO_184_,
        DO_185_,
        DO_186_,
        DO_187_,
        DO_188_,
        DO_189_,
        DO_190_,
        DO_191_,
        DO_192_,
        DO_193_,
        DO_194_,
        DO_195_,
        DO_196_,
        DO_197_,
        DO_198_,
        DO_199_,
        DO_200_,
        DO_201_,
        DO_202_,
        DO_203_,
        DO_204_,
        DO_205_,
        DO_206_,
        DO_207_,
        DO_208_,
        DO_209_,
        DO_210_,
        DO_211_,
        DO_212_,
        DO_213_,
        DO_214_,
        DO_215_,
        DO_216_,
        DO_217_,
        DO_218_,
        DO_219_,
        DO_220_,
        DO_221_,
        DO_222_,
        DO_223_,
        DO_224_,
        DO_225_,
        DO_226_,
        DO_227_,
        DO_228_,
        DO_229_,
        DO_230_,
        DO_231_,
        DO_232_,
        DO_233_,
        DO_234_,
        DO_235_,
        DO_236_,
        DO_237_,
        DO_238_,
        DO_239_,
        DO_240_,
        DO_241_,
        DO_242_,
        DO_243_,
        DO_244_,
        DO_245_,
        DO_246_,
        DO_247_,
        DO_248_,
        DO_249_,
        DO_250_,
        DO_251_,
        DO_252_,
        DO_253_,
        DO_254_,
        DO_255_,
        DO_256_,
        DO_257_,
        DO_258_,
        DO_259_,
        DO_260_,
        DO_261_,
        DO_262_,
        DO_263_,
        DO_264_,
        DO_265_,
        DO_266_,
        DO_267_,
        DO_268_,
        DO_269_,
        DO_270_,
        DO_271_,
        DO_272_,
        DO_273_,
        DO_274_,
        DO_275_,
        DO_276_,
        DO_277_,
        DO_278_,
        DO_279_,
        DO_280_,
        DO_281_,
        DO_282_,
        DO_283_,
        DO_284_,
        DO_285_,
        DO_286_,
        DO_287_,
        DO_288_,
        DO_289_,
        DO_290_,
        DO_291_,
        DO_292_,
        DO_293_,
        DO_294_,
        DO_295_,
        DO_296_,
        DO_297_,
        DO_298_,
        DO_299_,
        DO_300_,
        DO_301_,
        DO_302_,
        DO_303_,
        DO_304_,
        DO_305_,
        DO_306_,
        DO_307_,
        DO_308_,
        DO_309_,
        DO_310_,
        DO_311_,
        DO_312_,
        DO_313_,
        DO_314_,
        DO_315_,
        DO_316_,
        DO_317_,
        DO_318_,
        DO_319_,
        DO_320_,
        DO_321_,
        DO_322_,
        DO_323_,
        DO_324_,
        DO_325_,
        DO_326_,
        DO_327_,
        DO_328_,
        DO_329_,
        DO_330_,
        DO_331_,
        DO_332_,
        DO_333_,
        DO_334_,
        DO_335_,
        DO_336_,
        DO_337_,
        DO_338_,
        DO_339_,
        DO_340_,
        DO_341_,
        DO_342_,
        DO_343_,
        DO_344_,
        DO_345_,
        DO_346_,
        DO_347_,
        DO_348_,
        DO_349_,
        DO_350_,
        DO_351_,
        DO_352_,
        DO_353_,
        DO_354_,
        DO_355_,
        DO_356_,
        DO_357_,
        DO_358_,
        DO_359_,
        DO_360_,
        DO_361_,
        DO_362_,
        DO_363_,
        DO_364_,
        DO_365_,
        DO_366_,
        DO_367_,
        DO_368_,
        DO_369_,
        DO_370_,
        DO_371_,
        DO_372_,
        DO_373_,
        DO_374_,
        DO_375_,
        DO_376_,
        DO_377_,
        DO_378_,
        DO_379_,
        DO_380_,
        DO_381_,
        DO_382_,
        DO_383_,
        DO_384_,
        DO_385_,
        DO_386_,
        DO_387_,
        DO_388_,
        DO_389_,
        DO_390_,
        DO_391_,
        DO_392_,
        DO_393_,
        DO_394_,
        DO_395_,
        DO_396_,
        DO_397_,
        DO_398_,
        DO_399_,
        DO_400_,
        DO_401_,
        DO_402_,
        DO_403_,
        DO_404_,
        DO_405_,
        DO_406_,
        DO_407_,
        DO_408_,
        DO_409_,
        DO_410_,
        DO_411_,
        DO_412_,
        DO_413_,
        DO_414_,
        DO_415_,
        DO_416_,
        DO_417_,
        DO_418_,
        DO_419_,
        DO_420_,
        DO_421_,
        DO_422_,
        DO_423_,
        DO_424_,
        DO_425_,
        DO_426_,
        DO_427_,
        DO_428_,
        DO_429_,
        DO_430_,
        DO_431_,
        DO_432_,
        DO_433_,
        DO_434_,
        DO_435_,
        DO_436_,
        DO_437_,
        DO_438_,
        DO_439_,
        DO_440_,
        DO_441_,
        DO_442_,
        DO_443_,
        DO_444_,
        DO_445_,
        DO_446_,
        DO_447_,
        DO_448_,
        DO_449_,
        DO_450_,
        DO_451_,
        DO_452_,
        DO_453_,
        DO_454_,
        DO_455_,
        DO_456_,
        DO_457_,
        DO_458_,
        DO_459_,
        DO_460_,
        DO_461_,
        DO_462_,
        DO_463_,
        DO_464_,
        DO_465_,
        DO_466_,
        DO_467_,
        DO_468_,
        DO_469_,
        DO_470_,
        DO_471_,
        DO_472_,
        DO_473_,
        DO_474_,
        DO_475_,
        DO_476_,
        DO_477_,
        DO_478_,
        DO_479_,
        DO_480_,
        DO_481_,
        DO_482_,
        DO_483_,
        DO_484_,
        DO_485_,
        DO_486_,
        DO_487_,
        DO_488_,
        DO_489_,
        DO_490_,
        DO_491_,
        DO_492_,
        DO_493_,
        DO_494_,
        DO_495_,
        DO_496_,
        DO_497_,
        DO_498_,
        DO_499_,
        DO_500_,
        DO_501_,
        DO_502_,
        DO_503_,
        DO_504_,
        DO_505_,
        DO_506_,
        DO_507_,
        DO_508_,
        DO_509_,
        DO_510_,
        DO_511_,
        DO_512_,
        DO_513_,
        DO_514_,
        DO_515_,
        DO_516_,
        DO_517_,
        DO_518_,
        DO_519_,
        DO_520_,
        DO_521_,
        DO_522_,
        DO_523_,
        DO_524_,
        DO_525_,
        DO_526_,
        DO_527_,
        DO_528_,
        DO_529_,
        DO_530_,
        DO_531_,
        DO_532_,
        DO_533_,
        DO_534_,
        DO_535_,
        DO_536_,
        DO_537_,
        DO_538_,
        DO_539_,
        DO_540_,
        DO_541_,
        DO_542_,
        DO_543_,
        DO_544_,
        DO_545_,
        DO_546_,
        DO_547_,
        DO_548_,
        DO_549_,
        DO_550_,
        DO_551_,
        DO_552_,
        DO_553_,
        DO_554_,
        DO_555_,
        DO_556_,
        DO_557_,
        DO_558_,
        DO_559_,
        DO_560_,
        DO_561_,
        DO_562_,
        DO_563_,
        DO_564_,
        DO_565_,
        DO_566_,
        DO_567_,
        DO_568_,
        DO_569_,
        DO_570_,
        DO_571_,
        DO_572_,
        DO_573_,
        DO_574_,
        DO_575_,
        DO_576_,
        DO_577_,
        DO_578_,
        DO_579_,
        DO_580_,
        DO_581_,
        DO_582_,
        DO_583_,
        DO_584_,
        DO_585_,
        DO_586_,
        DO_587_,
        DO_588_,
        DO_589_,
        DO_590_,
        DO_591_,
        DO_592_,
        DO_593_,
        DO_594_,
        DO_595_,
        DO_596_,
        DO_597_,
        DO_598_,
        DO_599_,
        DO_600_,
        DO_601_,
        DO_602_,
        DO_603_,
        DO_604_,
        DO_605_,
        DO_606_,
        DO_607_,
        DO_608_,
        DO_609_,
        DO_610_,
        DO_611_,
        DO_612_,
        DO_613_,
        DO_614_,
        DO_615_,
        DO_616_,
        DO_617_,
        DO_618_,
        DO_619_,
        DO_620_,
        DO_621_,
        DO_622_,
        DO_623_,
        DO_624_,
        DO_625_,
        DO_626_,
        DO_627_,
        DO_628_,
        DO_629_,
        DO_630_,
        DO_631_,
        DO_632_,
        DO_633_,
        DO_634_,
        DO_635_,
        DO_636_,
        DO_637_,
        DO_638_,
        DO_639_
    }
    #endregion

    #region 서보모터 enum 정리
    /** @brief 서보모터 축이름 */
    public enum enumServoAxis // 서보모터 축이름
    {
        Router_X,
        Router_Y,
        Router_Z,
        Router_A,
        BaseRobot_X,
        BaseRobot_X_1,
        BaseRobot_Y,
        BaseRobot_Z,
        BaseRobot_T,
        Lift_PCBInPut,
        Lift_BoxInPut,
        Lift_BoxOutPut,
        PCB_Width,
        PCBInPut_Push,
        BoxInPut_Push,
        BoxOutPut_Push
    }


    #endregion

   

   



    #region 구조체
    /** @brief 좌표계 데이터 */
    public struct structPosition // 좌표계
    {
        public double x;
        public double y;
        public double z;
        public double a;

        public structPosition(double xPos, double yPos, double zPos, double angle)
        {
            x = xPos;
            y = yPos;
            z = zPos;
            a = angle;
        }
    }

    /** @brief 오프셋 데이터 */
    public struct structOffset // 오프셋
    {
        public double x;
        public double y;
        public double z;
        public double a;

        public structOffset(double xPos, double yPos, double zPos, double aPos)
        {
            x = xPos;
            y = yPos;
            z = zPos;
            a = aPos;
        }
    }

    /** @brief 서보모터 축 정보 */
    public struct structAxisStatus // 서보 축 정보
    {
        public ushort AxisNo;
        public bool Errored;
        public ushort ErrorID;
        public bool HomeAbsSwitch;
        public bool LimitSwitchPos;
        public bool LimitSwitchNeg;
        public bool PowerOn;
        public bool isHomed;
        public bool ErrorStop;
        public bool Disabled;
        public bool Stopping;
        public bool Homing;
        public bool StandStill;
        public double Position;     // mm 단위 위치값
        public double Velocity;
        public double Torque;
        public ulong HomedTime;

        public structAxisStatus(ushort no, bool e, ushort eid, bool home, bool p, bool n, bool on, bool homed, bool estop, bool d, bool st, bool h, bool ss, double Pos, double Vel, double Toq)
        {
            AxisNo = no;
            Errored = e;
            ErrorID = eid;
            HomeAbsSwitch = home;
            LimitSwitchPos = p;
            LimitSwitchNeg = n;
            PowerOn = on;
            isHomed = h;
            ErrorStop = estop;
            Disabled = d;
            Stopping = st;
            Homing = h;
            StandStill = ss;
            Position = Pos;
            Velocity = Vel;
            Torque = Toq;
            HomedTime = 0;
        }
    }

    /** @brief 시리얼포트 정보 */
    public struct structSerialPort // 시리얼포트 정보
    {
        public int rsPort;
        public EnumBaudRate rsBaudRate;
        public int rsDataLength;
        public Parity rsParity;
        public StopBits rsStopBits;
        public SerialPort serialPort;    // 시리얼포트 선언

        public structSerialPort(int RsPort, EnumBaudRate RsBaudRate, int RsDataLength, Parity RsParity, StopBits RsStopBits, SerialPort SerialPort)
        {
            rsPort = RsPort;
            rsBaudRate = RsBaudRate;
            rsDataLength = RsDataLength;
            rsParity = RsParity;
            rsStopBits = RsStopBits;
            serialPort = SerialPort;
        }
    }

    /** @brief 레이저마킹기 마킹 정보 */
    public struct structMarkPoint // 레이저마킹기 마킹 정보
    {
        public double xPos;
        public double yPos;
        public bool mark2D;
        public string text;

        public structMarkPoint(double XPos, double YPos, bool Mark2D, string Text)
        {
            xPos = XPos;
            yPos = YPos;
            mark2D = Mark2D;
            text = Text;
        }
    }



    #endregion
    #endregion

    #endregion
    /**
     * @brief  시스템 전역 변수 선언
     */
    public static class GlobalVar
    {

        #region 기본설정 바꿔야 하는 부분
        /** @brief 프로젝트 구분 */
        public static enumProject ProjectType = enumProject.AutoInline; // 이더켓 마스터 종류
        #region Master, DIO, Servo, StepMotor
        /** @brief 이더켓 마스터 종류 */
        public static enumMasterType MasterType = enumMasterType.AXL; // 이더켓 마스터 종류
        /** @brief 첫 서보 모터의 SlaveID, 서보가 먼저 연결되면 0, crevis 하나 후에 연결되면 1 */
        public static uint Axis_num = 0; // 첫 서보 모터의 SlaveID, 서보가 먼저 연결되면 0, crevis 하나 후에 연결되면 1
        /** @brief DIO 모듈 순서, 물리적으로 IO 모듈의 갯수만큼 배열을 지정한다. 서보 상관 없이 크레비스 모듈간의 순서 */
        public static uint[] Slave_order = { 0, 1 }; // crevis 모듈 순서, 서보 상관 없이 크레비스 모듈간의 순서
        /** @brief DIO 모듈 중 입력용의 SlaveID, 물리적으로 IO 모듈의 갯수만큼 배열을 지정한다. 먼저 연결되면 0, 서보 모터 후에 연결되면 서보모터 갯수 */
        public static uint[] Slave_di_num = { 0, 1 }; // crevis 모듈 중 입력용의 SlaveID, 먼저 연결되면 0, 서보 모터 후에 연결되면 서보모터 갯수
        /** @brief 디지털 입력의 점수, 물리적으로 IO 모듈의 갯수만큼 배열을 지정한다 */
        public static uint[] DiSize = { 32 * 7, (uint)((Enum.GetValues(typeof(FuncInline.enumDINames))).Length) - 32 * 7 }; // 디지털 입력의 점수, 모듈이 두개
        /** @brief 디지털 출력의 점수, 물리적으로 IO 모듈의 갯수만큼 배열을 지정한다 */
        public static uint[] DoSize = { 0, (uint)(Enum.GetValues(typeof(FuncInline.enumDONames)).Length) }; // 디지털 출력의 점수, 모듈이 두개
        /** @brief 입출력 모듈의 bit수, 물리적으로 IO 모듈의 갯수만큼 배열을 지정한다 */
        public static uint[] ModuleSize = { 8, 8 }; // 입출력 모듈의 bit수
        /** @brief crevis 모듈 중 출력용의 SlaveID, 물리적으로 IO 모듈의 갯수만큼 배열을 지정한다. 먼저 연결되면 0, 서보 모터 후에 연결되면 서보모터 갯수 */
        public static uint[] Slave_do_num = Slave_di_num; // crevis 모듈 중 출력용의 SlaveID, 먼저 연결되면 0, 서보 모터 후에 연결되면 서보모터 갯수

        /** @brief 서보모터 기어비, 물리적으로 서보모터의 갯수만큼 배열을 지정한다. */
        public static double[] ServoGearRatio = { 1, 1, 1, 1, 1,
                                                  1, 1, 1, 1, 1}; // 서보모터 기어비
        /** @brief 서보모터 한바퀴 펄스, 물리적으로 서보모터의 갯수만큼 배열을 지정한다. */
        //public static double[] ServoRevPulse = { 8388608, 8388608, 8388608, 8388608, 8388608,
        //                                        8388608, 8388608, 8388608, 8388608, 8388608,
        //                                        8388608, 8388608, 8388608 }; // 서보모터 한바퀴 펄스

        // JHRYU 2023.12.08 서보설정
        // 펄스값이 너무 높아서 아진 컨트롤러가 속도 제한이 걸려버려서
        // 파나텀에서 서보드라이브의 1회전당 펄스수를 8388608 -> 524288 로 일괄 변경 하였다
        //public static double[] ServoRevPulse = { 524288, 524288, 524288, 524288, 524288,
        //                                        524288, 524288, 524288, 524288, 524288,
        //                                        131072/*524288*/, 524288, 524288 }; // 서보모터 한바퀴 펄스


        // 2023.12.19 서보 드라이브 앤코더 펄스 조정
        // 컨베이어도 느리고 캔추리도 느려서 모든 서보의 앤코더 펄스를 131072로 변경
        public static double[] ServoRevPulse = { 100000, 100000, 100000, 100000, 100000,
                                                100000, 100000, 100000, 100000, 100000,
                                                100000, 100000, 100000 }; // 서보모터 한바퀴 펄스


        /** @brief 서보모터 한바퀴 회전당 이동 거리, 물리적으로 서보모터의 갯수만큼 배열을 지정한다. */
        // 3,4 번 서보는 각도값으로 조작하는 타입이므로 1회전은 360도 이므로 
        // 각도를 입력했을때 원하는 펄스만큼 이동 가능하다.
        //public static double[] ServoRevMM = { (81.18 * Math.PI), (81.18 * Math.PI), (360), (360), 4,
        //                                    20, 10, 10, (70.03 * Math.PI), 10,
        //                                    10, 20, 5 }; // 서보모터 한바퀴 회전당 이동 거리

        public static double[] ServoRevMM = { 20, 20, 20, 5, 20,
                                            5, 20, 20}; // 서보모터 한바퀴 회전당 이동 거리


        //public static bool ThreadInputPCB = false;
        //public static int[] ConvSpeed = { 0, 0 };
        /** @brief 스텝모터 기어비, 물리적으로 스탭모터의 갯수만큼 배열을 지정한다. */
        public static double[] WidthGearRatio = { 1, 1, 1, 1, 1, 1, 1, 1,
                                                  1, 1, 1, 1, 1,
                                                  1, 1, 1, 1, 1,
                                                  1, 1, 1, 1, 1,
                                                  1, 1, 1, 1, 1,
                                                  1}; // 스텝모터 기어비
        /** @brief 스텝모터 한바퀴 펄스, 물리적으로 스탭모터의 갯수만큼 배열을 지정한다. */
        public static double[] WidthRevPulse = { 1000, 1000, 1000, 1000, 1000, 1000, 1000, 1000,
                                                 1000, 1000, 1000, 1000, 1000,
                                                 1000, 1000, 1000, 1000, 1000,
                                                 1000, 1000, 1000, 1000, 1000,
                                                 1000, 1000, 1000, 1000, 1000,
                                                 1000}; // 스텝모터 한바퀴 펄스
        /** @brief 스텝모터 한바퀴 회전당 이동 거리, 물리적으로 스탭모터의 갯수만큼 배열을 지정한다. */
        public static double[] WidthRevMM = { 10, 10, 10, 10, 10, 10, 10, 10,
                                              10, 10, 10, 10, 10,
                                              10, 10, 10, 10, 10,
                                              10, 10, 10, 10, 10,
                                              10, 10, 10, 10, 10,
                                              10 }; // 스텝모터 한바퀴 회전당 이동 거리
        #endregion

        #region 설비 이름 관련
        //public static string SwVersion = "0.001"; // 소프트웨어 버전
        /** @brief 장비의 메인 타이틀. */
        public static string AppName = "SNUC_Sanding_System"; // 장비의 메인 타이틀
                                                          //public static string DepartName = ""; // 사용처 또는 부서명
        /** @brief 설정 및 데이터용 Master Directory. */
        public static string FaPath = "C:\\FA"; // 설정 및 데이터용 Master Directory
        /** @brief 소프트웨어 이름, 실행파일명과 일치시킬 것 */
        public static string SWName = "SNUC_Sanding"; // 소프트웨어 이름, 실행파일명과 일치
        /** @brief 모델세팅 저장 폴더 */
        public static string ModelPath = "Models"; // 모델세팅 저장 폴더
        /** @brief 세팅폴더 이름 */
        public static string IniPath = "Setting"; // 세팅폴더 이름
        /** @brief 세팅Ini의 세션명 */
        public static string IniSection = "setting"; // 세팅Init의 세션명
        /** @brief 로그파일 폴더 이름 */
        public static string LogPath = "Log"; // 로그파일 폴더 이름
        /** @brief 검사로그파일 폴더 이름 */
        public static string ValueLogPath = "ValueLog"; // 검사로그파일 폴더 이름
        /** @brief 서보동작로그파일 폴더 이름 */
        public static string ServoLogPath = "MoveLog"; // 서보동작 로그파일 폴더 이름
        /** @brief SMD통신 로그파일 폴더 이름 */
        public static string TesterLogPath = "TesterLog"; // SMD통신 로그파일 폴더 이름
        /** @brief 스캐너 로그파일 폴더 이름 */
        public static string ScanLogPath = "ScanLog"; // 스캐너 로그파일 폴더 이름
        /** @brief 데이터베이스 로그파일 폴더 이름 */
        public static string SqlLogPath = "SqlLog"; // 데이터베이스 로그파일 폴더 이름
        /** @brief 디버그 로그파일 폴더 이름 */
        public static string DebugLogPath = "Debug"; // 디버그 로그파일 폴더 이름
        /** @brief 카운트 로그파일 폴더 이름 */
        public static string CountPath = "Count"; // 카운트 로그파일 폴더 이름
        /** @brief 카운트 로그파일 검색용 폴더 이름 */
        public static string SearchPath = "Search"; // 카운트 로그파일 검색용 폴더 이름
        /** @brief SECS/GEM 통신 로그파일 검색용 폴더 이름 */
        public static string SECSPath = "SECS"; // SECS/GEM 통신 로그파일 검색용 폴더 이름
        /** @brief Array Layout Image파일 폴더 이름 */
        public static string ArrayImagePath = "ArrayImage"; // Array Layout Image파일 폴더 이름
        /** @brief SQL로그 저장 여부. 따로 설정연계는 안 한다. */
        public static bool SqlLog = false; // SQL로그 저장 여부. 따로 설정연계는 안 한다.
        /** @brief 디버그 로그 저장 여부. 따로 설정연계는 안 한다.  */
        public static bool DebugLog = false; // 디버그 로그 저장 여부. 따로 설정연계는 안 한다. 

        #endregion

        #region 각종 기능 사용여부
        /** @brief 그냥 Error 사용. false일 경우 확장에러시스템 사용  */
        public static bool UseNormalError = true; // 그냥 Error 사용. false일 경우 확장에러시스템 사용
        /** @brief 파트클리어 시스템 사용 여부, 사용시 에러창 클릭하면 파트클리어로, 미사용시 에러창으로  */
        public static bool UsePartClear = false; // 파트클리어 시스템 사용 여부, 사용시 에러창 클릭하면 파트클리어로, 미사용시 에러창으로
        /** @brief 시스템 구분 InOut Stop 사용 여부  */
        public static bool UseInOutStop = false;
        /** @brief 도어 사용 여부를 읽어 오는 변수 */
        public static string set_useDoor = ""; //GSCHOI-230616  도어 사용 여부를 읽어 오는 변수

        /** @brief 장비 기본 세팅 저장된 파일 경로 */
        public static string seting_IniPath = GlobalVar.FaPath + "\\" + GlobalVar.SWName + "\\" + GlobalVar.IniPath + "\\setting.ini";
        /** @brief 장비 기본 세팅 ini의 섹션명 */
        public static string seting_Section = GlobalVar.IniSection;

        #endregion

        #region DB
        #region MsSQL
        /** @brief MSSql 사용 여부 */
        public static bool UseMsSQL = false;
        /** @brief MSSql 연결할 서버 IP */
        public static string MsSQL_Server = "127.0.0.1";//"192.168.0.31"; //"127.0.0.1";
        /** @brief MSSql 연결할 서버 포트 */
        public static string MsSQL_Port = "1433";
        /** @brief MSSql 계정 */
        public static string MsSQL_Id = "sa"; //"sa";
        /** @brief MSSql 계정 비밀번호 */
        public static string MsSQL_Pwd = "radix5243";
        /** @brief MSSql DataBase명 */
        public static string MsSQL_DB = "AutoInline";
        /** @brief MSSql 연결 개체 */
        //public static MsSQL Sql = new MsSQL(MsSQL_Server, MsSQL_Port, MsSQL_Id, MsSQL_Pwd, MsSQL_DB);
        public static MsSQL Sql = new MsSQL();

        /** @brief MSSql 데이터 버전. * 구조변경 등 작업시 버전별 구조변경문 FuncSQL에 선언 */
        public static int MsSql_Version = 0;
        #endregion
        #endregion

        #endregion

        #region Key Messages
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_CHAR = 0x105;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;
        #endregion

        #region Class Variables
        public const int SRCCOPY = 13369376;
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        public static object ProjectClass = null; // 메인 클래스를 다른 클래스 등에서 참조할 경우 링크를 위해. 프로젝트별로 다를 것이므로 object로 선언하고, 사용시 해당 클래스로 캐스팅해서 사용해야 함.
        #endregion

        #region DIO & Motion & Robot             



        #region Init Check 변수

        public static bool Init_AutoRun_Need = false;            // 메인 오토런
        public static bool Init_AutoRun_Finish = false;          // 메인 오토런

        //Init Variable
        public static bool Init_Variable_Need = false;           // 변수 초기화 시작
        //Init Part
        public static bool Init_Feeder_Need = false;            // 피더
        public static bool Init_Feeder_Finish = false;          // 피더

        public static bool Init_InBlster_Need = false;          // Blister 안착기
        public static bool Init_InBlster_Finish = false;        // Blister 안착기

        public static bool Init_SupplyManual_Need = false;      // Manual 공급기
        public static bool Init_SupplyManual_Finish = false;    // Manual 공급기

        public static bool Init_SupplyBox_Need = false;         // Box 공급기
        public static bool Init_SupplyBox_Finish = false;       // Box 공급기

        public static bool Init_BoxIn_Need = false;             // 투입기
        public static bool Init_BoxIn_Finish = false;           // 투입기

        public static bool Init_BoxReverse_Need = false;        // Box Reverse (박스 반전기)
        public static bool Init_BoxReverse_Finish = false;      // Box Reverse (박스 반전기)

        public static bool Init_BoxLabel_Need = false;          // 라벨기
        public static bool Init_BoxLabel_Finish = false;        // 라벨기

        public static bool Init_SortOut_Need = false;           // 최종 선별기
        public static bool Init_SortOut_Finish = false;         // 최종 선별기

        public static bool Init_MES_Need = false;               // MES 처리용
        public static bool Init_MES_Finish = false;             // MES 처리용

        public static bool Init_LaserMark_Need = false;         // LaserMark 처리용
        public static bool Init_LaserMark_Finish = false;       // LaserMark 처리용

        public static bool Init_BoxCheck_Need = false;         // LaserMark 처리용
        public static bool Init_BoxCheck_Finish = false;       // LaserMark 처리용

        public static bool Init_OutBarcodes_Need = false;       // 배출 컨베이어의 바코드 처리
        public static bool Init_OutBarcodes_Finish = false;     // 배출 컨베이어의 바코드 처리



        //#region  변수
        ///** @brief 변수 초기화 시작 */
        //public static bool Init_Variable = false; // 변수 초기화 시작
        ///** @brief 해당 쓰레드 변수 초기화 완료(샘플) */
        //public static bool Init_Variable_ABase_Finish = false; // 해당 쓰레드 변수 초기화 완료(샘플)
        ///** @brief 해당 쓰레드 변수 초기화 완료 (HighGain PCBSupply) */
        //public static bool Init_Variable_HighGain_PCBSupply_Finish = false; // 해당 쓰레드 변수 초기화 완료
        ///** @brief 해당 쓰레드 변수 초기화 완료 (HighGain PCBMove) */
        //public static bool Init_Variable_HighGain_PCBMove_Finish = false; // 해당 쓰레드 변수 초기화 완료
        ///** @brief 해당 쓰레드 변수 초기화 완료 (HighGain Router) */
        //public static bool Init_Variable_HighGain_Router_Finish = false; // 해당 쓰레드 변수 초기화 완료
        ///** @brief 해당 쓰레드 변수 초기화 완료 (HighGain BoxMove) */
        //public static bool Init_Variable_HighGain_BoxMove_Finish = false; // 해당 쓰레드 변수 초기화 완료

        //#endregion
        //#region 실린더
        ///** @brief 실린더 초기화 시작 */
        //public static bool Init_Cylinder = false; // 실린더 초기화 시작
        ///** @brief 해당 쓰레드 실린더 초기화 완료(샘플) */
        //public static bool Init_Cylinder_ABase_Finish = false; // 해당 쓰레드 변수 초기화 완료(샘플)
        ///** @brief 해당 쓰레드 실린더 초기화 완료(HighGain PCBSupply) */
        //public static bool Init_Cylinder_HighGain_PCBSupply_Finish = false; // 해당 쓰레드 실린더 초기화 완료
        ///** @brief 해당 쓰레드 실린더 초기화 완료(HighGain PCBMove) */
        //public static bool Init_Cylinder_HighGain_PCBMove_Finish = false; // 해당 쓰레드 실린더 초기화 완료
        ///** @brief 해당 쓰레드 실린더 초기화 완료(HighGain Router) */
        //public static bool Init_Cylinder_HighGain_Router_Finish = false; // 해당 쓰레드 실린더 초기화 완료
        ///** @brief 해당 쓰레드 실린더 초기화 완료(HighGain BoxMove) */
        //public static bool Init_Cylinder_HighGain_BoxMove_Finish = false; // 해당 쓰레드 실린더 초기화 완료
        //#endregion
        //#region 서보
        ///** @brief 서보 초기화 시작 */
        //public static bool Init_Servo = false; // 서보 초기화 시작
        ///** @brief 해당 쓰레드 서보 초기화 완료(샘플) */
        //public static bool Init_Servo_ABase_Finish = false; // 해당 쓰레드 변수 초기화 완료(샘플)
        ///** @brief 해당 쓰레드 서보 초기화 완료(HighGain PCBSupply) */
        //public static bool Init_Servo_HighGain_PCBSupply_Finish = false; // 해당 쓰레드 서보 초기화 완료
        ///** @brief 해당 쓰레드 서보 초기화 완료(HighGain PCBMove) */
        //public static bool Init_Servo_HighGain_PCBMove_Finish = false; // 해당 쓰레드 서보 초기화 완료
        ///** @brief 해당 쓰레드 서보 초기화 완료(HighGain Router) */
        //public static bool Init_Servo_HighGain_Router_Finish = false; // 해당 쓰레드 서보 초기화 완료
        ///** @brief 해당 쓰레드 서보 초기화 완료(HighGain BoxMove) */
        //public static bool Init_Servo_HighGain_BoxMove_Finish = false; // 해당 쓰레드 서보 초기화 완료
        //#endregion
        //#region 로봇
        ///** @brief 로봇 초기화 시작 */
        //public static bool Init_Robot = false; // 로봇 초기화 시작
        ///** @brief 해당 쓰레드 로봇 초기화 완료(샘플) */
        //public static bool Init_Rpbot_ABase_Finish = false; // 해당 쓰레드 변수 초기화 완료(샘플)
        ///** @brief 해당 쓰레드 로봇 초기화 완료(HighGain PCBSupply) */
        //public static bool Init_Robot_HighGain_PCBSupply_Finish = false; // 해당 쓰레드 로봇 초기화 완료
        ///** @brief 해당 쓰레드 로봇 초기화 완료(HighGain PCBMove) */
        //public static bool Init_Robot_HighGain_PCBMove_Finish = false; //  해당 쓰레드 로봇 초기화 완료
        ///** @brief 해당 쓰레드 로봇 초기화 완료(HighGain Router) */
        //public static bool Init_Robot_HighGain_Router_Finish = false; //  해당 쓰레드 로봇 초기화 완료
        ///** @brief 해당 쓰레드 로봇 초기화 완료(HighGain BoxMove) */
        //public static bool Init_Robot_HighGain_BoxMove_Finish = false; //  해당 쓰레드 로봇 초기화 완료        
        //#endregion



        /** @brief 초기화 전체 완료 */
        public static bool Init_Finish = false; // 초기화 완료
        #endregion

        //public static bool[] Init_Servo = new bool[Enum.GetValues(typeof(enumServoAxis)).Length]; // 각 서보모터 초기화 완료
        /** @brief 각 서보모터 초기화 여부 */
        public static bool[] Homing_Servo = new bool[Enum.GetValues(typeof(enumServoAxis)).Length]; // 각 서보모터 초기화 여부        

        /** @brief 서보 홈 잡을 것이냐  */
        public static bool Axis_Homeing = true; // 서보 홈 잡을 것이냐 
        //public static uint Axis_count = (uint)Enum.GetValues(typeof(enumServoAxis)).Length; // 서보 모터 축수
        /** @brief 서보 모터 축수 * 해당 부분은 전체어떻게 가져갈지 생각해 보자 */
        public static uint Axis_count = (uint)Enum.GetValues(typeof(FuncInline.enumServoAxis)).Length;// 서보 모터 축수//해당 부분은 전체어떻게 가져갈지 생각해 보자
        /** @brief 홈 잡은 후 옵셋까지 가지 못한 경우 */
        public static bool HomeOffsetCanNotMove = false;//홈 잡은 후 옵셋까지 가지 못한 경우
        /** @brief 홈 지령 후 슬립 */
        public static int Home_Command_After_Sleep = 1000; // 홈 지령 후 슬립
        /** @brief 서보 정지 지령 후 슬립 */
        public static int Servo_Stop_After_Sleep = 50; // 서보 정지 지령 후 슬립
        /** @brief 무빙 지령 후 슬립 */
        public static int Servo_Move_After_Sleep = 250; // 무빙 지령 후 슬립

        //MXP
        //public static uint[,] DIO_Size = { { (uint)DiSize[0] / 8, (uint)DoSize[0] / 8 }, { (uint)DiSize[1] / 8, (uint)DoSize[1] / 8 } };  // 입출력 모듈의 갯수, 순서대로 DI, DO 슬롯의 수
        //AXL
        //public static uint[,] DIO_Size = { { (uint)DiSize[0] / ModuleSize[0], (uint)DoSize[0] / ModuleSize[0] }, 
        //                                    { (uint)DiSize[1] / ModuleSize[1], (uint)DoSize[1] / ModuleSize[1] } };  // 입출력 모듈의 갯수, 순서대로 DI, DO 슬롯의 수

        /** @brief 어드벤텍 디지털 입력의 점수 */
        public static byte AD_DiSize = 64; // 어드벤텍 디지털 입력의 점수
        /** @brief 어드벤텍 디지털 출력의 점수 */
        public static byte AD_DoSize = 64; // 어드벤텍 디지털 출력의 점수

        /** @brief 아진 엑스텍 인풋 모듈 갯수 */
        public static int Inputmodule = 1; //아진 엑스텍 인풋 모듈 갯수
        /** @brief 아진 엑스텍 아웃풋 모듈 갯수 */
        public static int Outputmodule = 1; //아진 엑스텍 아웃풋 모듈 갯수
        /** @brief 아진 엑스텍 인풋 시작 노드 ID */
        public static int InputStartNodeID = 0; //아진 엑스텍 인풋 시작 노드 ID
        /** @brief 아진 엑스텍 아웃풋 시작 노드 ID      */
        public static int OutputStartNodeID = InputStartNodeID+ Inputmodule; //아진 엑스텍 아웃풋 시작 노드 ID     

        /** @brief 같은 패턴으로 모듈마다 반복될 경우 다음 모듈간의 DI 차이 */
        public static int DIModuleGap = 24; // 같은 패턴으로 모듈마다 반복될 경우 다음 모듈간의 DI 차이
        /** @brief 같은 패턴으로 모듈마다 반복될 경우 다음 모듈간의 DO 차이 */
        public static int DOModuleGap = 8;   // 같은 패턴으로 모듈마다 반복될 경우 다음 모듈간의 DO 차이

        //
        #region Servo Set

        //public static double SiteGearRatio = 1;
        //public static double SiteRevPulse = 400;
        //public static double SiteRevMM = 3;
        #endregion
        #endregion

        #region 시스템 전역변수
        /** @brief 다국어지원용 언어. Locale 적용은 되어 있지 않음. */
        public static enumLanguage Language = enumLanguage.English;

        /** @brief 시뮬레이션 실행 여부, 개발시 장비 없이 가능(시뮬레이션 코드 추가 코딩 필요) */
        public static bool Simulation = false; // 시뮬레이션 실행 여부, 개발시 장비 없이 가능(시뮬레이션 코드 추가 코딩)        
        /** @brief 디버그 모드 구동. 일단 켜 두고 Release로 컴파일시 false로 바꾼다. */
        public static bool Debug = false; // 디버그 모드 구동. 일단 켜 두고 Release로 컴파일시 false로 바꾼다.

        public static bool SMDLog = true; // PCB 상태 추적 로그 저장 여부

        /** @brief 프로그램 종료 여부 */
        public static bool GlobalStop = false; // 프로그램 종료 여부
        /** @brief 각 쓰레드 동작 속도(sleep값) */
        public static int ThreadSleep = 50; // 각 쓰레드 동작 속도(sleep값)        

        public static int Simulation_Timeout = 100; // 시뮬레이션시 타임아웃 처리 곱하기 값. 개발중 타임아웃 때문에 디버깅이 어렵기 때문에 크게 해서 작업하고, 완료 또는 타임아웃 체크시 1로 바꾼다.
        /** @brief 이더켓 마스터의 초기화 구동중 */
        //public static bool MasterChecking = false; // 이더켓 마스터의 초기화 구동중
        /** @brief 이더켓 마스터의 초기화 완료됨 */
        //public static bool MasterChecked = false; // 이더켓 마스터의 초기화 완료됨        
        /** @brief 각 서보 축 상태 저장 */
        public static structAxisStatus[] AxisStatus = new structAxisStatus[Axis_count]; // 각 서보 축 상태 저장        
        /** @brief 현재 디지털 입력 */
        //DIO_BoxPacking_enumDINames
        //DIO_BoxPacking_enumDONames
        public static bool[] DI_Array = new bool[Enum.GetValues(typeof(FuncInline.enumDINames)).Length]; // 현재 디지털 입력 
        /** @brief 이전 디지털 입력 */
        public static bool[] DI_Before = new bool[Enum.GetValues(typeof(FuncInline.enumDINames)).Length]; // 이전 디지털 입력
        /** @brief 디지털 입력 변화여부. 변화여부 체크 후 해당 DI 변화여부는 초기화됨 */
        public static bool[] DI_Change = new bool[Enum.GetValues(typeof(FuncInline.enumDINames)).Length]; // 디지털 입력 변화여부
        /** @brief 현재 시도중인 디지털 출력 */
        public static bool[] DO_Array = new bool[Enum.GetValues(typeof(FuncInline.enumDONames)).Length]; // 현재 시도중인 디지털 출력 
        /** @brief 출력 확인된 디지털 출력 */
        public static bool[] DO_Read = new bool[Enum.GetValues(typeof(FuncInline.enumDONames)).Length]; // 출력 확인된 디지털 출력
        /** @brief 이전 디지털 출력 */
        public static bool[] DO_Before = new bool[Enum.GetValues(typeof(FuncInline.enumDONames)).Length]; // 이전 디지털 출력
        /** @brief 디지털 출력 변화여부. 변화여부 체크 후 해당 DO 변화여부는 초기화됨 */
        public static bool[] DO_Change = new bool[Enum.GetValues(typeof(FuncInline.enumDONames)).Length]; // 디지털 출력 변화여부

        /** @brief cycle time 산정위한 타이머 */
        public static Stopwatch CycleWatch = new Stopwatch();
        #endregion

     

        #region 전역 설정
        /** @brief 모델 이름 배열. 최대 1000개 */
        public static string[] ModelNames = new string[1000]; // 모델 이름들
        public static StringBuilder ModelPath_MXN = new StringBuilder(100);
        public static StringBuilder ModelName_MXN = new StringBuilder(100);
        /** @brief 등록된 모델 갯수 */
        public static int ModelCount = 0; // 등록된 모델 갯수
        /** @brief 세팅창 닫았음. 이후 메인에 모델 정보 갱신용 */
        public static bool SettingClose = false; // 세팅창 닫았음. 이후 메인에 모델 정보 갱신용

        public static int ServoResetCount = 0; // 러닝 중 서보알람 발생시 리셋 카운트
        public static int ServoResetMax = 10; // 러닝 중 서보알람 발생시 리셋 카운트 연속시 알람 기준값
                                              // 
        /** @brief 택타임 체크 시작 시간 */
        public static ulong TackStart = GlobalVar.TickCount64; // 택타임 체크 시작 시간                                                             

        /** @brief 도어 체크 사용여부 */
        public static bool UseDoor = true; // 도어 체크 사용여부

        
        /** @brief DryRun 사용여부 */
        public static bool DryRun = false;
        /** @brief DryRun처럼 소재 없이 로봇 연속 구동 여부 */
        public static bool TestPass = false; // DryRun처럼 소재 없이 로봇 연속 구동
        /** @brief Robot이 소재 처리시 에러 발생해도 계속 진행 여부 */
        public static bool AutoPass = true; // Robot이 소재 처리시 에러 발생해도 계속 진행
        /** @brief Cycle Stop 기능 활성화 여부 */
        public static bool UseCycleStop = true; // Cycle Stop 기능 활성화 여부
        /** @brief 사이클 스탑을 눌렀다 */
        public static bool CycleStop = false;//사이클 스탑을 눌렀다.
        /** @brief 사이클 스탑을 누르고 동작이 끝났다 */
        public static bool CycleStop_End = false;//사이클 스탑을 누르고 동작이 끝났다.
        /** @brief ByPass 기능 활성화 여부 */
        public static bool ByPassMode = false;//Bypass기능 활성화 여부

        /** @brief 에러코드별로 에러 발생 여부 저장 */
        public static bool[] SystemError = new bool[1000]; // 에러코드별로 에러 발생 여부 저장 YJ20220119 큐 하나에서 체크하도록 변경
        /** @brief 현재 발생된 에러가 있음 */
        public static bool SystemErrored = false; // 현재 발생된 에러가 있음        
        //public static bool[] SystemError = new bool[1000]; // 에러코드별로 에러 발생 여부 저장 YJ20220119 큐 하나에서 체크하도록 변경
       
      
        /** @brief Part Error 사용 여부 */
        public static bool PartError = false; // Part Error 사용

        public static bool TeachingMode = false; // 교시 모드     
        public static bool Muting = false; // 뮤팅 모드. NG버퍼 전진시 안전PLC Mute를 true 시킨다.
        public static bool OriginDoing = false; // 초기화 관련 실행중   
        public static bool LightCurtain = false; // 라이트 커튼 감지 여부  
        public static Stopwatch PwdWatch = new Stopwatch(); // 관리자 통과 시간. 시간경과거나 런 들어갈 때 

        


        /** @brief True 일때 Warning 폼실행 진입하기 위해 */
        public static bool dlgWarning_Check = false;      //True 일때 Warning 폼실행 진입하기 위해 by DG
        /** @brief Warning MSG를 추가하기위해 */
        public static String dlgWarning_Msg = "";      //Warning MSG를 추가하기위해 by DG
		 /** @brief Warning 경고창이 떠있는지 확인하기 위해 */
        public static bool Warning = false; //경고창이 떠있는지
		 /** @brief Warning 이 발생한 쓰레드(위치)를 확인 하기 위해 */
        public static int WarningState = 0; //경고창 현위치 확인
		 /** @brief Warning 경고창을 추가로 발생될때 그전 쓰레드(위치)를 확인 하기 위해 */
        //public static int WarningStatePre = 0; //이전 경고창위치 확인
        public static List<int> WarningStatePre = new List<int>(); //이전 경고창위치 확인

        /** @brief 전역 에러값 */
        public static string GlobalError = ""; // 전역 에러값                
        /** @brief 상태 메시지 */
        public static string SystemMsg = ""; // 상태 메시지
        /** @brief Emergency Stop */
        public static bool E_Stop = false; // Emergency Stop
        /** @brief 시스템 또는 타워 상태값 */
        public static enumSystemStatus SystemStatus = enumSystemStatus.BeforeInitialize; // 시스템 또는 타워 상태값
        /** @brief 시스템 상태별 타워램프 설정 배열 */
        public static bool[,,] TowerAction = new bool[Enum.GetValues(typeof(enumSystemStatus)).Length, 4, 2];
        /** @brief 시스템 상태별 부저 시간 배열 */
        public static ulong[] TowerTime = new ulong[8];
        /** @brief 부저 작동 시간 */
        public static ulong TowerTick = GlobalVar.TickCount64;
        /** @brief 타워램프 출력 여부 */
        public static bool EnableTower = true;//부저 스탑 눌렀을 때 부저가 울리지 않음
        /** @brief 부저 스탑 눌렀을 때 부저가 울리지 않음 */
        public static bool EnableBuzzer = true;
        public static bool OperationLock = false;
        /** @brief 도어가 열려 시스템이 정지됨 */
        public static bool DoorOpen = false;
		 /** @brief 부저 타이머 */
        public static ulong BuzzerTime = 0;
		 /** @brief Warning 타이머 */
        public static ulong WarningTime = 0;
		 /** @brief 관리자 비밀번호 통과 여부 */
        public static bool PwdPass = false; // 관리자 비밀번호 통과 여부
        /** @brief 관리자 비밀번호 */
        public static string ManagePasswd = "1234"; // 관리자 비밀번호
        /** @brief 팝업창 하나만 유지하기 위해, 여러 개 열 경우 UI 쓰레드들간에 충돌이 있는지 버벅임 */
        public static bool dlgOpened = false; // 팝업창 하나만 유지하기 위해, 여러 개 열 경우 UI 쓰레드들간에 충돌이 있는지 버벅임.     

        /** @brief 서보모터 자동 이동속도 mm/sec */
        public static double ServoSpeed = 100; // 서보모터 이동속도 mm/sec, BoxPacking 에서는 전체 서보의 배율 퍼센트로 작동
        /** @brief 서보모터 메뉴얼 이동속도 mm/sec */
        public static double ServoManualSpeed = 20; // 서보모터 메뉴얼 이동속도 mm/sec
        /** @brief 서보모터 homing속도, pulse */
        public static int ServoHomSpeed = 100; // 서보모터 homing속도, pulse
        /** @brief 서보모터 가감속 */
        public static double ServoAccDec = 5; // 서보모터 가감속
        /** @brief 서보모터 Jerk */
        public static double ServoJerk = ServoAccDec * ServoAccDec; // 서보모터 Jerk
        /** @brief 폭조절 스텝모터 속도, pulse */
        public static int WidthSpeed = 100; // 폭조절 스텝모터 속도, pulse
        /** @brief 폭조절 스텝모터 homing속도, pulse */
        public static int WidthHomSpeed = 100; // 폭조절 스텝모터 homing속도, pulse
        /** @brief 폭조절 스텝모터 가감속 */
        public static int WidthAccDec = 5; // 폭조절 가감속   
        /** @brief 폭조절 스텝모터 Jerk */
        public static int WidthJerk = WidthAccDec * WidthAccDec; // 폭조절 Jerk

        /** @brief Lift 속도조절 */
        public static double Magazine_LiftSpeed_check = 2; //Lift 속도조절 
        /** @brief 컨베이어, Push, T턴 속도조절 */
        public static double Magazine_Speed_check = 3; //컨베이어, Push, T턴 속도조절
        #endregion

        #region StopWatch
     


        
       

        /** @brief 직교로봇 이동 타임아웃 처리 초 */
        public static int Timeout_Servo = 30; // 직교로봇 이동 타임아웃 처리 초
        /** @brief 컨베어 이동 타임아웃 처리 초 */
        public static int Timeout_Conveyor = 30; // 컨베어 이동 타임아웃 처리 초
        /** @brief 2D 스캔 이동 타임아웃 처리 초 */
        public static int Timeout_Scan = 30; // 2D 스캔 이동 타임아웃 처리 초
        /** @brief 라우팅 타임아웃 처리 초 */
        public static int Timeout_Routing = 60; // 라우팅 타임아웃 처리 초
        #endregion


        #region 장비별 수정할 부분
        #region 화면 처리 관련
        /** @brief 물리적인 화면번호가 프로그램상 번호와 다를 수 있어 별도로 체크해서 저장한다. */
        public static int[] ScreenNumbers = { 0, 0 }; // 물리적인 화면번호가 프로그램상 번호와 다를 수 있어 별도로 체크해서 저장한다.
        /** @brief OKCANCEL 메시지박스가 있는지 여부, 하나가 떠 있으면 더 진행 안 되게 한다. */
        public static bool MessageOKShown = false; // OKCANCEL 메시지박스가 있는지 여부, 하나가 떠 있으면 더 진행 안 되게 한다.
        /** @brief OKCANCEL 메시지박스가 있는지 여부, 하나가 떠 있으면 더 진행 안 되게 한다. */
        public static bool MessageOK = false; // OKCANCEL 메시지박스가 있는지 여부, 하나가 떠 있으면 더 진행 안 되게 한다.
        /** @brief OKCANCEL 메시지박스에서 OK를 클릭했는지 여부. */
        public static bool MessageOKClick = false; // OKCANCEL 메시지박스에서 OK를 클릭했는지 여부.
        /** @brief 메시지창 띄울 창 번호 */
        public static int MessageScreen = 0; // 메시지창 띄울 창 번호
        /** @brief 메시지창 폼 개체 */
        public static Form MessageForm = null;
        /** @brief 메시지창 클래스 개체 */
        public static MessageBox MessageOKBox;
        /** @brief YES/NO 메시지창 개체 */
        public static TaskDialog dialogTask; // YES/NO 메시지창
        /** @brief OK 메시지창 개체 */
        public static TaskDialog messageTask; // OK 메시지창
        #endregion

        /** @brief 기본 모델명 */
        public static string ModelName = "DefaultModel"; // 기본 모델명

        public static string ModelName_2 = "DefaultModel"; // YJ20211006 두번째 포트 사용시 기본 모델명
     

        /** @brief 로그 저장할 큐 */
        public static ConcurrentQueue<string> LogQueue = new ConcurrentQueue<string>(); // 로그 저장할 
        ///** @brief 로그 저장할 큐 */
        public static ConcurrentQueue<string> ValueLogQueue = new ConcurrentQueue<string>(); // 로그 저장할 
        ///** @brief 로그 저장할 큐 */
        public static ConcurrentQueue<string> ServoLogQueue = new ConcurrentQueue<string>(); // 로그 저장할 
        /** @brief 디버그 로그 저장할 큐 */
        public static ConcurrentQueue<string> DebugLogQueue = new ConcurrentQueue<string>(); // 디버그 로그 저장할 큐

        //// ConcurrentQueue<LogData> 선언
        //public static ConcurrentQueue<GlobalVar.LogData> CntQueue = new ConcurrentQueue<GlobalVar.LogData>();

        public static ConcurrentQueue<string> TesterLogQueue = new ConcurrentQueue<string>(); // 테스트 로그 저장할 큐
        public static ConcurrentQueue<string> ScanLogQueue = new ConcurrentQueue<string>(); // 스캔 로그 저장할 큐
        public static ConcurrentQueue<string> SQLExecuteQueue = new ConcurrentQueue<string>(); // 데이터베이스 insert/update 큐

        public static ConcurrentQueue<FuncInline.structError> SystemErrorQueue = new ConcurrentQueue<FuncInline.structError>(); // 현재 발생된 에러 저장. ErrorDialog 띄우기용. 프로젝트마다 타입이 다를 것이므로 object로 선언
        public static ConcurrentQueue<FuncInline.structError> SystemErrorListQueue = new ConcurrentQueue<FuncInline.structError>(); // 현재 발생된 에러 저장. 카운트 조회용. 프로젝트마다 타입이 다를 것이므로 object로 선언
        /** @brief 에러 히스토리 리스트 */
        public static List<FuncInline.structError> SystemErrorList = new List<FuncInline.structError>();//YJ20230629 에러 히스토리 리스트.

        public static Stopwatch SystemLogWatch = new Stopwatch(); // System 로그 주기 체크용

        // 로그 데이터를 저장할 구조체 정의
        public struct LogData
        {
            public string LogText;
            public string Model;

            public LogData(string logText, string model)
            {
                LogText = logText;
                Model = model;
            }
        }




        #endregion

   

        



        #region 장비별 설정
        /** @brief 법인코드 */
        public static string CoCode = "7"; // 법인코드
        /** @brief 장비 호기 */
        public static int MachineNumber = 1; // 장비 호기
        /** @brief 장비 전/후면 기준면 */
        public static enumMachineFrontRear MachineFrontRear = enumMachineFrontRear.FrontBase; // 전/후면 기준면
        /** @brief 장비 진행방향. 좌우/우좌 */
        public static enumMachineLeftRight MachineLeftRight = enumMachineLeftRight.LeftRight; // 좌우/우좌
        /** @brief 장비 리니어 연결 여부 */
        public static enumMachineLinear MachineLinear = enumMachineLinear.StandAlone; // 리니어 연결 여부

        /** @brief 로그파일 삭제 일수 */
        public static int LogFileDeleteDay = 30; // 로그파일 삭제 일수
        /** @brief InStop 처리 시간 */
        public static int InStopTime = 5; // InStop 처리 시간
        /** @brief OutStop 처리 시간 */
        public static int OutStopTime = 5; // OutStop 처리 시간

        /** @brief AutoInline 핀수명 관리 일수 */
        public static int PinLifeDate = 1000;
        /** @brief AutoInline 핀수명 관리 작동횟수 */
        public static int PinLifeCount = 1000;
        /** @brief AutoInline 핀수명 관리 사용여부 */
        public static bool CheckPinLife = false;
        //public static int CustCodeCount = 0;
        //public static string[] CustCodes = new string[1000];
        //public static string[] CustNames = new string[1000];

      
        #endregion



        #region 동작 관련 등 전역 설정, Popup Dialog 연결 변수 등

        /** @brief Instop 처리 시간체크값 */
        public static ulong InputTime = GlobalVar.TickCount64; // Instop 처리 시간값
        /** @brief Outstop 처리 시간체크값 */
        public static ulong OutputTime = GlobalVar.TickCount64; // Outstop 처리 시간값

        /** @brief 컨베어 정/역 변환시 대기 시간 */
        public static int ConveyorReverseTime = 1000; // 컨베어 정/역 변환시 대기 시간
        /** @brief 로봇 동작시 완료 체크 에러 처리 시간 */
        public static int RobotErrorTime = 30000; // 로봇 동작시 완료 체크 에러 처리 시간

        /** @brief 일반적인 타임아웃 처리 시간 */
        public static long NormalTimeout = 30000; // 일반적인 타임아웃 처리 시간
                                                 //public static double[] WorkAngle = { 2.5, 182 };
                                                 //public static long WorkWidthHome = 4294967218; // 오토닉스 모터 홈 찾기 후 위치값 부호가 이상한지 아주 크게 나타날 경우 이 만큼 감산한다.

        public static long BoxPackingTimeout = 12000;            // BoxPacking 일반 타임아웃 처리 시간
        public static long Timeout_Step = 12;                    // BoxPackingTimeout 의 초

        /** @brief 프로그램 중간에 프로세싱 타임 체크용. */
        public static ulong tick = GlobalVar.TickCount64; // 프로그램 중간에 프로세싱 타임 체크용.
        #endregion


        #region 설비별 개별 변수

        #region 레이저 마킹기
        /*
        M,  // - A015M
        G, // - A015G
        F, // - A015F
        V, // - A015V
        T, // - A015T1
        W, // - A015W
        A, // - A015A
        D, // - SM-S111DL
        Z, // - A015AZ
        R // - A015R4
        //*/

        //public static string CustomerCode = "M"; // 고객사 코드(거래선)
        //public static string CustomerName = ""; // 고객사 이름(거래선)
        //public static double InputPCBWidth = 100; // 투입컨베어 PCB 폭
        //public static double WorkPCBWidth = 0; // 작업PCB 폭
        //public static double FrontMarkHeight = 111; // Front z축 거리
        //public static double RearMarkHeight = 114; // Rear z축 거리
        //public static structPosition WorkInputPos = new structPosition(0, 13, FrontMarkHeight, 2.34); // 워크지그로 투입시 지그 좌표값
        //public static structPosition WorkOutputPos = new structPosition(330, 13, FrontMarkHeight, 182.34); // 워크지그에서 언로더로 배출시 지그 좌표값

        //public static structPosition FrontFirstAlignPos = new structPosition(0, 0, FrontMarkHeight, 0); // 전면 첫번째 얼라인 마크 체크 좌표값
        //public static structPosition FrontFirstAlignOffset = new structPosition(0, 0, FrontMarkHeight, 0); // 전면 첫번째 얼라인 마크 체크 후 오프셋값
        //public static structPosition FrontSecondAlignPos = new structPosition(0, 0, FrontMarkHeight, 0); // 전면 두번째 얼라인 마크 체크 좌표값
        //public static structPosition FrontSecondAlignOffset = new structPosition(0, 0, FrontMarkHeight, 0); // 전면 두번째 얼라인 마크 체크 후 오프셋값
        //public static structPosition RearFirstAlignPos = new structPosition(0, 0, RearMarkHeight, 0); // 후면 첫번째 얼라인 마크 체크 좌표값
        //public static structPosition RearFirstAlignOffset = new structPosition(0, 0, RearMarkHeight, 0); // 후면 첫번째 얼라인 마크 체크 후 오프셋값
        //public static structPosition RearSecondAlignPos = new structPosition(0, 0, RearMarkHeight, 0); // 후면 두번째 얼라인 마크 체크 좌표값
        //public static structPosition RearSecondAlignOffset = new structPosition(0, 0, RearMarkHeight, 0); // 후면 두번째 얼라인 마크 체크 후 오프셋값
        //public static double MarkingSize = 2.5; // 2D 마크의 크기, 가로,세로 공용
        //public static int ModuleCount = 16; // 2D 마크의 내부 점수, 가로,세로 공용
        //public static double LaserPower = 100; // 레이저 마킹기 출력, 내정값 대비 비율
        //public static double ScanSpeed = 100; // 레이저 마킹기 스캔 스피드(그리는 속도), 내정값 대비 비율
        //public static int FrontMarkCount = 0; // 전면 마킹할 좌표 수
        //public static int RearMarkCount = 0; // 후면 마킹할 좌표 수
        //public static structMarkPoint[] FrontMarkPos = new structMarkPoint[100]; // 전면 마킹할 좌표 목록
        //public static structMarkPoint[] RearMarkPos = new structMarkPoint[100]; // 후면 마킹할 좌표 목록
        //public static bool VisionCheck = true; // 비전 체크 여부
        //public static bool VisionCheckRemark = false; // 2D 비전 체크 실패시 재마킹
        //public static bool VisionCheckStop = true; // 비전 체크 실패시 정지 여부
        //public static bool AlignCheck = true; // Align 체크 유무
        //public static int VisionCheckTime = 1; // 비전 체크 몇 번에 한 번
        //public static int VisionCheckCount = 0; // 비전 체크 실행 횟수 - 몇 번에 한 번에 이용. 몇 번째 찍고 있으니 패쓰~, 첫번째만 찍기
        #endregion

        #region airosol 자동제어용
        //public static int DistributeTime1_1 = GlobalVar.TickCount64; // 분배기 최종 작동 시간, 일정 시간 지나고 작동해서 캔 서로 분리하기 위해
        //public static int Supply_Count = 2; // 주입기에서 다음으로 넘어갈 때 캔 갯수 카운트용, 스토퍼 미리 내밀기 위해 2로 세팅
        //public static bool Supply_Input = true; // 주입기 부분으로 캔 공급상태
        ////public static int Supply_Output_Time = GlobalVar.TickCount64; // 공급기로부터 다음으로 배출 시간
        //public static int Supply_Time = 500; // 액주입 신호 시간
        //public static int Supply1_Signal_Time = GlobalVar.TickCount64; // 주입기 1 주입시간
        //public static int Supply2_Signal_Time = GlobalVar.TickCount64; // 주입기 2 주입시간
        //public static int NG_Conv_Time = 3000; // NG 컨베어 작동 시간
        //public static int Weight_Time = 1000; // 중량NG 로드셀 안정화 시간

        //public static int Weight_Start_Time = GlobalVar.TickCount64; // 중량NG 배출 컨베어 작동 시간
        //public static int Weight_Check_Time = GlobalVar.TickCount64; // 중량검사시 대기 시간
        //public static double Weight_Min_Value = 0.0; // 중량 검사 최소값
        //public static double Weight_Max_Value = 0.5; // 중량 검사 최대값

        ////public static int Stopper_Off_Time = 500; // 스토퍼 감지 해제시 전진 대기 시간
        //public static int Stopper_OnTime_Fluid1 = GlobalVar.TickCount64; // 주입1 스토퍼 감지 해제 시간    

        //public static int Stopper_OffTime_Fluid1 = GlobalVar.TickCount64; // 주입1 스토퍼 감지 시작 시간     

        //public static int EndTime_Fluid1 = GlobalVar.TickCount64; // 주입1 완료 시간        

        //public static int Feeder_Time = 30000; // 피더 가동 시간
        //public static int Cap_Feeder_Time = GlobalVar.TickCount64;
        //public static int Cover_Feeder_Time = GlobalVar.TickCount64;

        //public static bool Arrive_Fluid1 = false; // 주입기1에 캔 도착
        //public static bool Arrive_Fluid2 = false; // 주입기2에 캔 도착
        //public static bool Arrive_Weight = false; // 중량체크에 캔 도착
        //public static bool Arrive_Cap = false; // 캡에 캔 도착
        //public static bool Arrive_Cover = false; // 커버에 캔 도착
        //public static bool Arrive_Labeler = false; // 라벨부착에 캔 도착
        //public static bool Arrive_Label_Check = false; // 라벨검사에 캔 도착
        //public static bool Arrive_Label_NG = false; // 라벨불량에 캔 도착

        //public static bool Supply1_Running = false; // 주입기 1에 주입중임
        //public static int Supply1_Running_Time = GlobalVar.TickCount64; // 주입기 1에 주입 경과 시간
        //public static bool Supply2_Running = false; // 주입기 2에 주입중임
        //public static int Supply2_Running_Time = GlobalVar.TickCount64; // 주입기 2에 주입 경과 시간
        //public static bool Supply1_Finish = false; // 주입기 1에 주입완료
        //public static bool Supply2_Finish = false; // 주입기 2에 주입완료
        //public static bool Weight_Running = false; // 중량검사 과정 수행 여부(캔 취출부터 배출까지)
        //public static int Weight_Running_Time = GlobalVar.TickCount64; // 중량검사 과정 수행 경과 시간
        //public static bool Weight_Finish = false; // 중량검사 과정 완료 여부(중량검사부터 배출까지)
        //public static bool Weight_Check = false; // 중량검사 체크 여부(중량검사 시작부터 종료까지)
        //public static bool Weight_OK = true; // 중량검사 양품 여부
        //public static bool Cap_Running = false; // 캡조립 과정 시작
        //public static int Cap_Running_Time = GlobalVar.TickCount64; // 캡조립 과정 경과 시간
        //public static bool Cap_Finish = false; // 캡조립 중 체결과정까지
        //public static bool Cover_Running = false; // 커버조립 과정 시작
        //public static int Cover_Running_Time = GlobalVar.TickCount64; // 커버조립 과정 경과 시간
        //public static bool Cover_Finish = false; // 커버조립 중 체결과정까지
        //public static bool Labeler_Running = false; // 라벨부착중
        //public static int Labeler_Running_Time = GlobalVar.TickCount64; // 라벨부착 경과 시간
        //public static bool Labeler_Push = false; // 라벨스펀지 작동중
        //public static bool Label_Check_Running = false; // 라벨 체크중
        //public static int Label_Check_Running_Time = GlobalVar.TickCount64; // 라벨 체크 경과 시간
        //public static bool Label_NG_Running = false; // NG 배출중
        //public static int Label_NG_Running_Time = GlobalVar.TickCount64; // 라벨부착불량 시간

        //public static int Weight_NG_Conveyor_Time = GlobalVar.TickCount64; // 중량불량배출 컨베어 시간
        //public static int Label_NG_Conveyor_Time = GlobalVar.TickCount64; // 라벨부착불량배출 컨베어 시간

        ////public static bool Robot_Out_Running = false; // 25개 박스에 투입중
        ////public static bool Robot_Out_Finish = false; // 25개 박스에 투입완료
        ////public static bool Robot_In = false; // 5개 캔 이동중
        ////public static bool Robot_In_Finish = false; // 5개 캔 이동 완료
        //public static bool Box_Out_Running = false; // 박스 배출중
        //public static int Box_Out_Running_Time = GlobalVar.TickCount64; // 박스 배출 경과 시간
        //public static bool Box_Out_Finish = false; // 박스 배출완료
        //public static bool Box_In_Running = false; // 박스 공급중
        //public static int Box_In_Running_Time = GlobalVar.TickCount64; // 박스 공급 경과 시간
        //public static bool Box_In_Finish = false; // 박스 공급완료
        //public static bool Box_Align_Running = false; // 박스 정렬중
        //public static int Box_Align_Running_Time = GlobalVar.TickCount64; // 박스 정렬 경과 시간
        //public static bool Box_Align_Finish = false; // 박스 정렬완료
        //public static bool Box_Open_Running = false; // 박스 벌림중
        //public static int Box_Open_Running_Time = GlobalVar.TickCount64; // 박스 벌림 경과 시간
        //public static bool Box_Open_Finish = false; // 박스 벌림완료
        //public static int Can_Count = 0; // 25개 캔 누적 갯수

        //public static int Box_Input_Delay = 5000; // 박스 공급 전 딜레이
        //public static int Box_Input_Delay_Time = 0; // 박스 공급 전 딜레이 실제값
        //public static int Box_Out_Delay = 10000; // 박스 배출 후 딜레이
        //public static int Box_Out_Delay_Time = GlobalVar.TickCount64; // 박스 배출 딜레이 실제값

        //public static bool Transfer_Timeout_Error = false;

        //public static int Label_Time = 500; // 라벨부착 신호 설정 시간
        ////public static int Labeler_Signal_Time = GlobalVar.TickCount64; // 라벨부착 신호 전송 시간

        //public static int Label_Complete_Time = 1000; // 라벨부착 완료 대기 시간
        //public static int Label_Check_Time = 500; // 라벨부착감지 신호 설정 시간
        ////public static int Label_Check_Signal_Time = GlobalVar.TickCount64; // 라벨부착감지 신호 전송 시간
        //public static bool Label_NG = false; // 라벨 부착 여부
        //public static bool Can_Align_Start = false;
        //public static bool Can_Align_Finish = false;

        //public static bool Use_Fluid = true; // 주입기 사용 여부
        ////public static bool Use_Fluid1 = true; // 주입시 사용 여부
        ////public static bool Use_Fluid2 = true; // 주입시 사용 여부
        //public static bool Use_Weight = true; // 중량체크 사용 여부
        //public static bool Use_Cap = true; // 캡 피더 사용 여부
        //public static bool Use_Cover = true; // 커버 피더 사용 여부
        //public static bool Use_Labeler = true; // 라벨부착 사용 여부
        //public static bool Use_Label_Check = true; // 라벨체크 사용 여부

        ////public static int Conveyor_Move_Time = 30000; // 스토퍼간 이동 체크 시간        
        //public static int ToRobotTime = GlobalVar.TickCount64;

        //public static int Transfer_Time = 10000; // 각 파트 이송 체크 시간

        //public static int Distribute_Count = 0; // 분배기 동작 카운트
        //public static int ToWeight_Count = 0; // 주입기에서 중량체크로 보낼 카운트

        //public static int Stopper_Delay = 1000; // 스토퍼 전진시 딜레이
        //public static int Cap_Push_Time = 1000; // 캡 체결 시간
        //public static int Cover_Push_Time = 1000; // 커버 체결 시간

        //public static bool RobotRun = false;
        //public static int RobotCount = 1; // 캔 이동 순서
        //public static bool BoxLock = false; // 박스 이동 방지
        #endregion

        #region 아진엑스텍 공통 변수

        #region AGV 통신 관련 변수
        //public static enumAGVStage_0 AGVStatus_0 = enumAGVStage_0.None;
        //public static enumAGVStage_1 AGVStatus_1 = enumAGVStage_1.None;
        //public static enumAGVStage_2 AGVStatus_2 = enumAGVStage_2.None;
        //public enum enumAGVStage_0
        //{
        //    None,
        //    Docking,
        //    Load,
        //    UnLoad,
        //    INSP,
        //    WorkReady,
        //    Working,
        //    WorkFinish,
        //    WorkContinue
        //}
        //public enum enumAGVStage_1
        //{
        //    None,
        //    Docking,
        //    Load,
        //    UnLoad,
        //    INSP,
        //    WorkReady,
        //    Working,
        //    WorkFinish,
        //    WorkContinue
        //}

        //public enum enumAGVStage_2
        //{
        //    None,
        //    Docking,
        //    Load,
        //    UnLoad,
        //    INSP,
        //    WorkReady,
        //    Working,
        //    WorkFinish,
        //    WorkContinue
        //}

        //public static bool AGV_0_Working = false;

        //public static bool AGVJobCodeReceive_0 = false;
        //public static bool AGVReady_0 = false;
        //public static bool AGVFront_Door_Close = false;
        //public static bool AGVContinue_0 = false;

        //public static bool AGVJobCodeReceive_1 = false;
        //public static bool AGVReady_1 = false;
        //public static bool AGVJobCodeReceive_2 = false;
        //public static bool AGVReady_2 = false;
        #endregion

        #region 설비 구분용 변수
        //public static bool ScannerUse = false;
        //public static bool LoadcellUse = false;
        //public static bool FuncTestUse = false;
        //public static bool LabelPrintUse = true;
        //public static bool VisualinspectionUse = false;
        #endregion

        #region 변수
        //public static int AXT_LoadPart_Product = 0;//공급 작업중인 수량
        //public static int AXT_UnLoadPart_Product = 0; //배출 작업중인 수량
        //public static uint AXT_totoalProductCount = 0; // 총 생산 수량
        //public static bool AXT_Working = false; //작업 공간에서 작업 중

        //public static int ServoSpeed_AXT = 10000; // ServoVel
        //public static int Servo_Load = 10; // Servo_Load
        //public static int Servo_UnLoad = 10; // Servo_UnLoad
        //public static int Servo_Door = 10; // Servo_Door        
        //public static double Servo_LoadServoHomeOffset = 0; // Servo_Load_HomeOffset
        //public static double Servo_UnLoadServoHomeOffset = 0; // Servo_UnLoad_HomeOffset
        //public static double Servo_DoorOpenPosition = 0; // Servo_Door_OpenPosition
        //public static double Servo_DoorClosePosition = 0; // Servo_Door_ClosePosition

        //public static bool AXT_Display_Load_0 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Load_1 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Load_2 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Load_3 = false; //UI 버튼 이미지

        //public static bool AXT_Display_Work_0 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Work_1 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Work_2 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Work_3 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Work_4 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Work_5 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Work_6 = false; //UI 버튼 이미지
        //public static bool AXT_Display_Work_7 = false; //UI 버튼 이미지

        //public static bool AXT_Display_UnLoad_0 = false; //UI 버튼 이미지
        //public static bool AXT_Display_UnLoad_1 = false; //UI 버튼 이미지
        //public static bool AXT_Display_UnLoad_2 = false; //UI 버튼 이미지
        //public static bool AXT_Display_UnLoad_3 = false; //UI 버튼 이미지
        //public static bool AXT_Display_UnLoad_4 = false; //UI 버튼 이미지
        //public static bool AXT_Display_UnLoad_5 = false; //UI 버튼 이미지

        //public static bool AXT_1_Step = false; //UI 버튼 이미지

        //public static bool AXT_Work_0_Cancle = false; //워크 캔슬
        //public static bool AXT_Work_1_Cancle = false; //워크 캔슬
        //public static bool AXT_Work_2_Cancle = false; //워크 캔슬
        //public static bool AXT_Work_3_Cancle = false; //워크 캔슬
        //public static bool AXT_Work_4_Cancle = false; //워크 캔슬
        //public static bool AXT_Work_5_Cancle = false; //워크 캔슬
        //public static bool AXT_Work_6_Cancle = false; //워크 캔슬
        //public static bool AXT_Work_7_Cancle = false; //워크 캔슬
        #endregion

        #region 검사기
        //public static bool[] AXT_WorkPart_Empty = new bool[8]; //작업공간 비어 있는지 확인
        //public static bool[] AXT_WorkPart_Work = new bool[8]; //작업공간 비어 있는지 확인
        //public static bool[] AXT_WorkPart_WorkFinish = new bool[8]; //작업공간 비어 있는지 확인
        //public static bool[] AXT_Function_Work_Start = new bool[8];
        #endregion

        #region 로봇 무빙 단계별 지령 변수

        #region Loading
        //public static bool Loading_Part_Get1 = false; // Loading Part_Get1
        //public static bool Loading_Part_Get2 = false; // Loading Part_Get2
        //public static bool Loading_Part_Get3 = false; // Loading Part_Get3
        //public static bool Loading_Part_Get1_Finish = false; // Loading Part_Get1
        //public static bool Loading_Part_Get2_Finish = false; // Loading Part_Get2
        //public static bool Loading_Part_Get3_Finish = false; // Loading Part_Get3
        #endregion

        #region Working
        //public static int Working_Number = 1; // Working 검사기 번호
        //public static int Working_Finish_Number = 1; // Working 검사기 번호
        //public static bool Working_Before_Align = false; // Working Before Align
        //public static bool Working_Before_Align_Finish = false; // Working Before Align_Finish
        //public static bool Working_Part_Put1 = false; // Working Part_Put1
        //public static bool Working_Part_Put2 = false; // Working Part_Put2
        //public static bool Working_Part_Put3 = false; // Working Part_Put3
        //public static bool Working_Part_Put1_Finish = false; // Working Part_Put1
        //public static bool Working_Part_Put2_Finish = false; // Working Part_Put2
        //public static bool Working_Part_Put3_Finish = false; // Working Part_Put3
        //public static bool Working_Part_Get1 = false; // Working Part_Get1
        //public static bool Working_Part_Get1_Finish = false; // Working Part_Get1
        //public static bool Working_After_Align = false; // Working After Align_Finish
        //public static bool Working_After_Align_Finish = false; // Working After Align_Finish
        #endregion

        #region Unloading
        //public static bool Unloading_Part_Put1 = false; // Working Part_Put1
        //public static bool Unloading_Part_Put1_Finish = false; // Working Part_Put1
        #endregion

        #endregion

        #region Offset 정보
        //public static structOffset Offset_Loding_1 = new structOffset(0, 0, 0); //로딩 1
        //public static structOffset Offset_Loding_2 = new structOffset(0, 0, 0); //로딩 2
        //public static structOffset Offset_Loding_3 = new structOffset(0, 0, 0); //로딩 3

        //public static structOffset Offset_Work_Before_Align = new structOffset(0, 0, 0); //검사기 Put 전 정렬

        //public static structOffset Offset_Work_FuncCheck_1_Put = new structOffset(0, 0, 0); //검사기 1 Put
        //public static structOffset Offset_Work_FuncCheck_2_Put = new structOffset(0, 0, 0); //검사기 2 Put
        //public static structOffset Offset_Work_FuncCheck_3_Put = new structOffset(0, 0, 0); //검사기 3 Put
        //public static structOffset Offset_Work_FuncCheck_4_Put = new structOffset(0, 0, 0); //검사기 4 Put
        //public static structOffset Offset_Work_FuncCheck_5_Put = new structOffset(0, 0, 0); //검사기 5 Put
        //public static structOffset Offset_Work_FuncCheck_6_Put = new structOffset(0, 0, 0); //검사기 6 Put
        //public static structOffset Offset_Work_FuncCheck_7_Put = new structOffset(0, 0, 0); //검사기 7 Put
        //public static structOffset Offset_Work_FuncCheck_8_Put = new structOffset(0, 0, 0); //검사기 8 Put

        //public static structOffset Offset_Work_FuncCheck_1_Get = new structOffset(0, 0, 0); //검사기 1 Get
        //public static structOffset Offset_Work_FuncCheck_2_Get = new structOffset(0, 0, 0); //검사기 2 Get
        //public static structOffset Offset_Work_FuncCheck_3_Get = new structOffset(0, 0, 0); //검사기 3 Get
        //public static structOffset Offset_Work_FuncCheck_4_Get = new structOffset(0, 0, 0); //검사기 4 Get
        //public static structOffset Offset_Work_FuncCheck_5_Get = new structOffset(0, 0, 0); //검사기 5 Get
        //public static structOffset Offset_Work_FuncCheck_6_Get = new structOffset(0, 0, 0); //검사기 6 Get
        //public static structOffset Offset_Work_FuncCheck_7_Get = new structOffset(0, 0, 0); //검사기 7 Get
        //public static structOffset Offset_Work_FuncCheck_8_Get = new structOffset(0, 0, 0); //검사기 8 Get

        //public static structOffset Offset_Work_after_Align = new structOffset(0, 0, 0); //검사기 Get 후 정렬

        //public static structOffset Offset_UnLoding = new structOffset(0, 0, 0); //언로딩
        #endregion

        #endregion

        #region SECS/GEM YJ 추가/변경
        //public static string SecsIP = "100.100.100.104"; //"127.0.0.1"; //"150.0.104.144"; // Host의 IP. 의미는 없다.
        //public static int SecsPort = 5106; // SECS 통신 포트
        //public static string SecsName = "AXT_Label"; // 장비명, 장비에 맞춰 수정
        //public static int SecsDevice = 0; // 장비 아이디, 장비마다 Host에서 정해준 대로
        //public static bool UseConversation = true; // Conversation Timeout 체크 여부. 제품ID 보고 후 원격지령 대기 여부.

        //public static string SecsVersion = "1.04"; // 통신 프로토콜 버전

        //public static bool SecsActive = false;
        //public static SECS Secs = new SECS(SecsIP, SecsPort, SecsActive, SecsDevice);
        //public static bool SecsOnlineMode = true;
        //public static bool UseSecs = false;
        //public static bool Init_Secs = false;

        //public static int ConversationTimeout = 30 * 1000; // Converation Timeout 세팅값

        //public static bool SecsTimeReceived = false; // S2F31 DateTime 수신 여부 YJ20210917
        //public static bool SecsRecipeListSent = false; // TimeReceived 따라 RecipeList 전송 여부 YJ20210917
        //public static bool SecsCancleReceive = false; // Cancle 커멘드 수령 함.

        //public static string RemoteLot = ""; // 최종 수신 Lot ID

        #region YJ20211006 두번째 포트 SECS 설정 관련 
        //public static int SecsPort_2 = 5106; // SECS 통신 포트
        //public static string SecsName_2 = ""; // 장비명, 장비에 맞춰 수정. 빈 값으로 두면 두번째 class를 만들지 않는다.
        //public static int SecsDevice_2 = 0; // 장비 아이디, 장비마다 Host에서 정해준 대로
        //public static bool UseConversation_2 = true; // Conversation Timeout 체크 여부. 제품ID 보고 후 원격지령 대기 여부.

        //public static bool SecsActive_2 = false;
        //public static SECS Secs_2 = new SECS(SecsIP, SecsPort_2, SecsActive_2, SecsDevice_2);

        //public static bool SecsTimeReceived_2 = false; // S2F31 DateTime 수신 여부
        //public static bool SecsRecipeListSent_2 = false; // TimeReceived 따라 RecipeList 전송 여부
        //public static bool SecsCancleReceive_2 = false; // Cancle 커멘드 수령 함.

        //public static string RemoteLot_2 = ""; // 최종 수신 Lot ID
        #endregion


        #region 시나리오 테스트용
        //public static enumSecsSenario Senario = enumSecsSenario.ProcessNormal; // 시뮬레이션 수행할 시나리오
        //public static bool SenarioStart = false; // 시뮬레이션 실행 여부, 중도에 false로 바뀌면 시나리오 종료
        #endregion

        #endregion

        #region 모듈형 AutoInline 전역변수
      

     

        #region Serial 통신 관련
        /** @brief AutoInline TestPC의 수 */
        public static int AutoInline_ComSMDCount = 3;
        /** @brief AutoInline TestPC의 통신 클래스 배열 */
        public static SMDSerial[] AutoInline_ComSMD = new SMDSerial[AutoInline_ComSMDCount]; // TestPC의 통신 클래스
     
        #endregion

        #region Serial DeviceServer 관련 - Telnet/TCP 통신
        /** @brief AutoInline DeviceServer 아이피 */
        public static string SerialDeviceServer_IP = "192.168.0.1";
        /** @brief AutoInline DeviceServer 퐅 */
        public static int SerialDeviceServer_Port = 23;
        /** @brief AutoInline DeviceServer TCP 연결 개체 */
        public static TcpClient SocketSerialDeviceServer = new TcpClient();
        /** @brief AutoInline DeviceServer stream 개체. */
        public static NetworkStream StreamSerialDeviceServer = default(NetworkStream);
        #endregion





        #endregion

        #region cifx DeviceNet(로봇 관련)        

        #region 로봇 전역
        /** @brief 다관절로봇 로봇 사용유무 */
        public static bool RobotUse = true; // 로봇 사용유무
        /** @brief 다관절로봇 로봇 종류    */
        public static enumRobot RobotType = enumRobot.KUKA; // 로봇 종류         

        /** @brief 다관절로봇 cifx class */
        public static cifXUser Cifx = new cifXUser(); // cifx class
        /** @brief 다관절로봇 cifx 에러 여부 */
        public static bool CifxError = false; // cifx 에러 여부
        /** @brief 다관절로봇 cifx 에러 횟수 */
        public static int CifxErrorCount = 0; // cifx 에러 횟수
        /** @brief 다관절로봇 cifx 출력 적용 여부 */
        public static bool CifxWrite = false; // cifx 출력 적용 여부
        /** @brief 다관절로봇 cifx 타임아웃 */
        public static uint DnetTimeout = 60000; // cifx 타임아웃
        /** @brief 다관절로봇 driver */
        public static uint PhDriver = 0; // driver 
        /** @brief 다관절로봇 보드명, 하나면 cifx0 */
        public static string SzBoard = "cifx0"; // 보드명, 하나면 cifx0
        /** @brief 다관절로봇 물리디바이스 */
        public static uint PhSysDevice = 0; // 물리디바이스
        /** @brief 다관절로봇 채널 포인터 */
        public static int PhChannel = 0; // 채널 포인터
        /** @brief 다관절로봇 채널값 */
        public static uint UlChannel = 0; // 채널값
        /** @brief 다관절로봇 상태값 */
        public static uint UlState = 0; // 상태값

        /** @brief 다관절로봇 인풋 점수 */
        public static int DNetISize = 645;
        /** @brief 다관절로봇 아웃풋 점수 */
        public static int DNetOSize = 645;

        /** @brief 다관절로봇 현재 디바이스넷 입력 */
        public static bool[] DNetI_Array = new bool[DNetISize]; // 현재 디바이스넷 입력
        /** @brief 다관절로봇 이전 디바이스넷 입력 */
        public static bool[] DNetI_Before = new bool[DNetISize]; // 이전 디바이스넷 입력
        /** @brief 다관절로봇 디바이스넷 입력 변화여부 */
        public static bool[] DNetI_Change = new bool[DNetISize]; // 디바이스넷 입력 변화여부
        /** @brief 다관절로봇 현재 출력할 디바이스넷 출력 */
        public static bool[] DNetO_Array = new bool[DNetOSize]; // 현재 출력할 디바이스넷 출력
        /** @brief 다관절로봇 출력 확인된 디바이스넷 출력 */
        public static bool[] DNetO_Read = new bool[DNetOSize]; // 출력 확인된 디바이스넷 출력
        /** @brief 다관절로봇 이전 디바이스넷 출력 */
        public static bool[] DNetO_Before = new bool[DNetOSize]; // 이전 디바이스넷 출력
        /** @brief 다관절로봇 디바이스넷 출력 변화 여부 */
        public static bool[] DNetO_Change = new bool[DNetOSize]; // 디바이스넷 출력 변화 여부

        /** @brief 다관절로봇 로봇동작 속도 % */
        public static double RobotSpeed = 50; // 로봇동작 속도 %
        /** @brief 다관절로봇 로봇 동작 지령 */
        public static enumRobotAction RobotAction = enumRobotAction.None; // 로봇 동작 지령
        /** @brief 다관절로봇 로봇 동작 툴 선택 */
        public static enumRobotTool RobotTool = enumRobotTool.Both; // 로봇 동작 툴 선택              

        /** @brief 다관절로봇 초기화 중 */
        public static bool RobotIning = false; // 초기화 중
        /** @brief 다관절로봇 로봇 동작 중 */
        public static bool RobotWorking = false; // 로봇 동작 중
        /** @brief 다관절로봇 로봇 알람 */
        public static bool RobotAlarm = false; // 로봇 알람
        #endregion





        #endregion

        #endregion






        #region RS-232를 통한 외부 제어기기(스캐너, 프린터, 로드셀)

        #region Scanner        
        /** @brief 2D스캐너 통신을 위한 Class Object */
        public static Scanner Scanner = new Scanner(); // 스캐너 통신을 위한 Class Object
        /** @brief 2D스캐너 데이터 수신 여부 */
        public static bool Load_Data_Received = false;
        /** @brief 2D스캐너 수신 데이터 */
        public static string Load_Scanner = " ";
        #endregion

        #region Label 출력 문장                
        //public static Print_Zebra_ZT610 Printer = new Print_Zebra_ZT610(); // 프린터 통신을 위한 Class Object        
        //public static string Label_Data = "";
        //public static string Label_Barcode = "";
        //public static string Label_String01 = "";
        //public static string Label_String02 = "";
        //public static string Label_String03 = "";
        //public static string Label_String04 = "";
        //public static string Label_String05 = "";
        //public static string Label_String06 = "";
        //public static string Label_String07 = "";
        //public static string Label_String08 = "";
        //public static string Label_String09 = "";
        //public static string Label_String10 = "";
        //public static string Label_String11 = "";
        //public static string Label_String12 = "";
        #endregion

        #region Loadcell
        //public static LoadCell Loadcell = new LoadCell(); // 프린터 통신을 위한 Class Object       
        //public static string LoadcellPort = "COM2";
        //public static int LoadcellBaud = 9600;
        //public static double LoadcellData = 0;
        //public static bool LoadcellDataOK = false;
        //public static bool LoadcellOpened = false;
        //public static double LoadcellCount = 0;
        //public static int LoadcellDataLength = 0;
        #endregion

        #endregion

        #region Ethernet를 통한 외부 제어 기기(레이저 마커, 로이체 비젼)

        #region 레이저마커
        /*
    public static string LaserIP = "192.168.100.105";
    public static int LaserPort = 9094;
    public static LaserMarker Laser = new LaserMarker(LaserIP, LaserPort);
    public static enumLaserStatus LaserStatus = enumLaserStatus.BeforeInitialize;
    public static string MarkingText = "";
    //*/
        #endregion

        #region 로이체 Vision 관련       
        /*
    public static string Vision_IP = "192.168.100.100"; // 비전 카메라 아이피, 복수일 경우 배열로 만들어야 함
    public static int Vision_Trigger_Port = 2006; // 비전 체크 트리거 송신용 포트
    public static int Vision_Receive_Port = 2005; // 비전 체크 데이터 수신용 포트
    public static structPosition VisionPosition = new structPosition(); // 비전 기준값 반영된 비전 체크 좌표 
    public static string VisionResult = ""; // 비전 체크값 디버깅용
    public static string Vision_App_Path = FaPath + "\\" + SWName + "\\ACR300i_Autostart.bat";
    //public static string Vision_App_Name = "DinoCapture.exe";
    public static string Vision_App_Title = "ACR 300i View (192.168.100.100)";
    public static structPosition VisionResolution = new structPosition(640, 480, GlobalVar.FrontMarkHeight, 0); // 비전 해상도
    public static structPosition VisionArea = new structPosition(34.7, 27.4, GlobalVar.FrontMarkHeight, 0); // 비전 범위
    public static structPosition VisionCenter = new structPosition(783, 238, GlobalVar.FrontMarkHeight, 0); // 비전 체크시 기준 좌표값
    //public static structPosition VisionLaserGap = new structPosition(19.25, 129.96, GlobalVar.MarkerHeight, 0); // 비전과 레이저 헤드 중심의 간격
    //public static structPosition VisionLaserGap = new structPosition(20.50, 131.16, GlobalVar.FrontMarkHeight, 0); // 비전과 레이저 헤드 중심의 간격
    public static structPosition VisionLaserGap = new structPosition(19.8, 130.45, GlobalVar.FrontMarkHeight, 0); // 비전과 레이저 헤드 중심의 간격
    public static structPosition RearVisionLaserGap = new structPosition(19.8, 130.45, GlobalVar.RearMarkHeight, 0); // 비전과 레이저 헤드 중심의 간격

    public static TcpClient socketLeuzeTrigger = new TcpClient();
    public static NetworkStream streamLeuzeTrigger = default(NetworkStream);

    public static TcpClient socketLeuzeData = new TcpClient();
    public static NetworkStream streamLeuzeData = default(NetworkStream);

    public static string VisionResult2D = "";
    public static structPosition VisionResultAlign = new structPosition();

    public static string MarkedCode = "";
    //*/
        #endregion



        #endregion






        #region PMC Motion
        //public static structSerialPort SerialWorkWidth;
        //public static structSerialPort SerialWorkConv;

        //public static PMCMotion WorkWidth = new PMCMotion(4, 1, 115200);
        //public static PMCMotion WorkConv = new PMCMotion(3, 1, 115200);
        //EnumBaudRate.
        #endregion


        #region 아래 부터는 필요시 위에 정리하면서 사용 바람 =============================================================================================


        #endregion

        // JHRYU SNUC_BoxPacking
        //public static string STR_1ST_SLOT_LANG = "CHN"; // "KOR"        // 1번 매뉴얼 슬롯의 MES 언어 타입 설정

        public static bool StepStop = false;        //1스탭 스탑을 눌렀다.

        public static bool BarcodePass = false;     // 오토런시 바코드 에러 검사를 하지않고 정상 수신 한것으로 처리한다.
        public static bool MesPass = false;         // 오토런시 MES 와의 통신을 시뮬레이션하고 정상 송수신 한것으로 처리한다.
        public static bool LabelPass = false;       // 라벨 프린트 스킵
        public static bool G_FeederON = false;      // UI Feeder 강제 런 모드
        public static bool G_LaserPass = false;     // UI 레이저 마킹기 패스모드
        public static bool G_BoxCheckPass = false;  // UI 박스외형 체크 패스모드 ( 사용 안함 )



        public static ulong StepTactTime = 0;         // 1Step 걸린시간 정보

        //public static MainScanQR QR = new MainScanQR();
        //public static DominoMark LaserMark = new DominoMark();
        //public static ZebraLabel LabelPrint = new ZebraLabel();

        public static int G_ManualType = 0;         // 현재 UI 에서 설정된 매뉴얼 슬롯 번호, 0 = 미설정 1 = (중국,영어), 2 = 다국어, 3 = 매뉴얼 없는 박스
        public static int G_LaserMarkModel = 1;     // 레이저 마킹기 기본 선택 모델 1 = 종이박스, 2 = 색박스
        public static bool LotFullLineStop = true;  // 분류중인 6개 Lot 이외에 새Lot 감지되면 라인정지여부, 정지안하면 7번으로 다 버림
        public static bool G_RePrint = true;        // 프린트 오류로 정지후 재시작시 프린트 다시 시도 여부
        public static bool G_ProgrammerUI = false;  // 개발자용 UI 보이기


        public static bool G_UseMesOutPos = true;   // MES 가 지정한 위치로 분류함, 미사용시 빈자리를 찾아서 투척
        public static int G_ErrNo = 0;              // 에러일때 에러코드 int 값



        //public static BoxPacking_AutoRun Thread_AutoRun = new BoxPacking_AutoRun();
        //public static BoxPacking_Feeder Thread_Feeder = new BoxPacking_Feeder();
        //public static BoxPacking_InBlister Thread_InBlister = new BoxPacking_InBlister();
        //public static BoxPacking_SupplyBox Thread_SupplyBox = new BoxPacking_SupplyBox();
        //public static BoxPacking_SupplyManual Thread_SupplyManual = new BoxPacking_SupplyManual();
        //public static BoxPacking_BoxIn Thread_BoxIn = new BoxPacking_BoxIn();
        //public static BoxPacking_BoxReverse Thread_BoxReverse = new BoxPacking_BoxReverse();
        //public static BoxPacking_BoxLabel Thread_BoxLabel = new BoxPacking_BoxLabel();
        //public static BoxPacking_SortOut Thread_SortOut = new BoxPacking_SortOut();

        //public static BoxPacking_LaserMark Thread_LaserMark = new BoxPacking_LaserMark();
        //public static BoxPacking_BoxCheck Thread_BoxCheck = new BoxPacking_BoxCheck();
        //public static BoxPacking_MES Thread_MES = new BoxPacking_MES();
        //public static BoxPacking_OutBarcodes Thread_OutBarcodes = new BoxPacking_OutBarcodes();

        //public static BoxPacking_Tracking Tracking = new BoxPacking_Tracking();

        // 메인 UI 에서 작업시간 계산용
        public static Stopwatch RunTimeCurrent = new Stopwatch();
        public static Stopwatch RunTimeTotal = new Stopwatch();
        public static Stopwatch RunTimeStop = new Stopwatch();  // (레이저마킹 정지용)오토런이 아닌상태(장비 정지후 경과시간)을 측정하는 용도
        public static Stopwatch RunTimeTact = new Stopwatch();

        //삼성 인라인 Trace.cs용
        public static System.Windows.Forms.DataGridViewCellStyle dataGridStyle = new System.Windows.Forms.DataGridViewCellStyle();

        // Windows API GetTickCount64 함수를 가져옴
        [DllImport("kernel32.dll")]
        private static extern ulong GetTickCount64();
        public static ulong TickCount64
        {
            get { return GetTickCount64(); }
        }
    }

}
