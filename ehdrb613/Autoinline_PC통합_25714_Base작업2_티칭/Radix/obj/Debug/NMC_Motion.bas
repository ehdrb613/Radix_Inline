'
' NMC_Motion
' 2015-03-26
' RSAutomation.co.,LTD.
' 
'----------------------------------------------------------------------------------
' Revision History.
' Initial Editing 2015.03.27
' Need to Check 로 표시 되어 있는 부분은 추가 검토가 필요한 내용입니다. - 2015-03-27
' Edited by MJLEE 2016.01.05
'
'
' Data Type Definition
Namespace NMC
    Class NMC_Motion
        Structure INT8T
            Public INT8 As SByte
        End Structure

        Structure UINT8T
            Public UINT8 As Byte
        End Structure

        Structure INT16T
            Public INT16 As Short
        End Structure

        Structure UINT16T
            Public UINT16 As UShort
        End Structure

        Structure INT32T
            Public INT32 As Integer
        End Structure

        Structure UINT32T
            Public UINT32 As UInteger
        End Structure

        Structure RealT
            Public REAL As Double
        End Structure

        Structure WORDT
            Public WORD As UShort
        End Structure

        Structure DWORDT
            Public DWORD As UInteger
        End Structure

        '16bit max (64k)
        'Public Enum MC_CONSTANT
        Public Const MAX_EEPROM_SIZE = &HFFFF&
        Public Const ECAT_MAX_NAME = &HFF&  '//255
        Public Const MAX_BOARD_CNT = 4
        Public Const MAX_AXIS_CNT = 64
        Public Const MAX_AXIS_ID = 65535 '//Ver_0c010012_6
        Public Const MAX_NODE_CNT = 256
        Public Const MAX_PHYSICAL_ADDR = 65535 '//Ver_0c010012_3
        Public Const MAX_MASTER_ID = 10
        Public Const MAX_ERR_LEN = 128
        Public Const MAX_PROFILE_ITEM_COUNT = 6
        Public Const MAX_AXES_IN_GROUP = 3
        Public Const MAX_AXES_GROUP_CNT = 32
        Public Const MAX_AXES_STATUS_SIZE = 8
        Public Const MAX_ALL_STATUS_SIZE = 1536 '//Ver_0c010019_1	: 계산식 = (MAX_AXIS_CNT + MAX_AXES_GROUP_CNT) 

        Public Const INVALID_BOARD = &HFFFF&
        Public Const INVALID_AXIS = &HFFFF&
        Public Const INVALID_MASTERID = &HFFFF&
        Public Const INVALID_FLASH = &HFFFFFFFF&

        ''// Axis Parameter Definition
        Public Const COMMANDED_POSITION = 1
        Public Const SW_LIMIT_POS = 2
        Public Const SW_LIMIT_NEG = 3
        Public Const ENABLE_LIMIT_POS = 4
        Public Const ENABLE_LIMIT_NEG = 5
        Public Const ENABLE_POS_LAG_MONITORING = 6
        Public Const MAX_POSITION_LAG = 7
        Public Const MAX_VELOCITY_SYSTEM = 8
        Public Const MAX_VELOCITY_APPL = 9
        Public Const ACTUAL_VELOCITY = 10
        Public Const COMMANDED_VELOCITY = 11
        Public Const MAX_ACCELERATION_SYSTEM = 12
        Public Const MAX_ACCELERATION_APPL = 13
        Public Const MAX_DECELERATION_SYSTEM = 14
        Public Const MAX_DECELERATION_APPL = 15
        Public Const MAX_JERK = 16
        Public Const ACTUAL_POSITION = 1001
        Public Const PROFILE_DACCEL = 1100
        ''//#define IOMAP_HEADER_SIZE 16
        Public Const IOMAP_HEADER_SIZE = 28 '//DSP 0.13 이상에서 변경됨 , 2014.08.26 (32 - 4)
        Public Const IOPAGE_SIZE = 4096

        'MC_XXXX Funtion Return Values.
        Public Enum MC_STATUS
            MC_OK = &H0
            MC_ERROR_HW_NOT_INSTALLED = &HDC
            MC_ERROR_DD_SEND_ERROR = &HDD
            MC_ERROR_DD_READ_ERROR = &HDE
            MC_DD_ERROR_SEND = &HDF
            MC_DD_ERROR_RECV = &HE0
            MC_DD_OPEN_FAIL = &HE6
            MC_DD_NOT_OPENED = &HE7
            MC_DD_CONN_FAIL = &HE8
            MC_DD_CLIENT_START_FAIL = &HE9
            MC_DD_OK = &H0
            MC_CN_NOT_CONNECTED = &HF0
            MC_CN_CONNECTED = &HF1
            MC_CN_CONNERROR = &HF2
            MC_INVALID_SYSTEM_STATE = &H10000 '// PLCOpen Motion Command Response
            MC_UNSUPPORT_CMD = &H20000
            MC_INVALID_PARAM = &H30000
            MC_INVALID_PARAM_1 = &H30001
            MC_INVALID_PARAM_2 = &H30002
            MC_INVALID_PARAM_3 = &H30003
            MC_INVALID_PARAM_4 = &H30004
            MC_INVALID_PARAM_5 = &H30005
            MC_INVALID_PARAM_6 = &H30006
            MC_INVALID_PARAM_7 = &H30007
            MC_INVALID_PARAM_8 = &H30008
            MC_INVALID_PARAM_9 = &H30009
            MC_INVALID_PARAM_10 = &H3000A
            MC_INVALID_SIZE = &H40000
            MC_INVALID_AXIS_NUM = &H50000
            MC_NOT_ENOUGH_RESOURCE = &H60000
            MC_LIMIT_ERROR_PARAM = &H70000
            MC_LIMIT_ERROR_PARAM_1 = &H70001
            MC_LIMIT_ERROR_PARAM_2 = &H70002
            MC_LIMIT_ERROR_PARAM_3 = &H70003
            MC_LIMIT_ERROR_PARAM_4 = &H70004
            MC_LIMIT_ERROR_PARAM_5 = &H70005
            MC_LIMIT_ERROR_PARAM_6 = &H70006
            MC_LIMIT_ERROR_PARAM_7 = &H70007
            MC_LIMIT_ERROR_PARAM_8 = &H70008
            MC_LIMIT_ERROR_PARAM_9 = &H70009
            MC_LIMIT_ERROR_PARAM_10 = &H7000A
            MC_INVALID_DEVICE_STATE = &H80000
            MC_INVALID_DEVICE_STATE_ERROR = &H80001
            MC_INVALID_AXIS_STATE_DISABLED = &H90000
            MC_INVALID_AXIS_STATE_SYNC_MOTION = &H90004
            MC_INVALID_AXIS_STATE_STOPPING = &H90006
            MC_INVALID_AXIS_STATE_ERRORSTOP = &H90007
            MC_INVALID_AXIS_CONFIG = &HA0000
            MC_INVALID_AXIS_CONFIG_POS_LIMIT_SWITCH = &HA000B
            MC_INVALID_AXIS_CONFIG_NEG_LIMIT_SWITCH = &HA000E
            MC_INVALID_AXIS_CONFIG_HOME_SENSOR = &HA0010
            MC_INVALID_AXIS_CONFIG_MARK_PULSE = &HA0012
            MC_INVALID_AXIS_CONFIG_HOMING_MODE = &HA0064
            MC_GEARING_RULE_VIOLATION = &HB0000
            MC_LIMIT_POSITION_OVER = &HC0000
            MC_INVALID_AXES_GROUP_NUM = &HD0000
            MC_AXIS_ALREADY_ASIGNED = &HE0000
            MC_AXES_GROUP_INVALID_STATE = &HF0000
            MC_AXIS_IN_SINGLE_MOTION_STATE = &H100000
            MC_GROUP_MEMBER_EMPTY = &H110000
            MC_GROUP_MEMBER_LIMIT_OVER = &H120000
            MC_GROUP_CMD_SIZE_ERROR = &H130000
            MC_GROUP_MEMBER_NOT_ALLOCED = &H140000
            MC_AXIS_IN_GROUP_MOTION = &H150000
            MC_FAIL = &HE00C0001 '// Libary 에서 발생하는 에러들
            MC_ERROR = &HE00C0002
            MC_IOMAPING_ERR = &HE00C0003
            MC_COMMINIT_ERR = &HE00C0004
            MC_COMM_EVENT_INIT_ERR = &HE00C0005
            MC_READ_ENI_NODE_ERR = &HE00C0006
            MC_INVALID_AXIS_ERR = &HE00C0007
            MC_INVALID_BOARD_ERR = &HE00C0008
            MC_XML_PARSING_ERR = &HE00C0009
            MC_XML_ITEM_COUNT_MISMATCH = &HE00C000A
            MC_NO_BOARD_INSTALLED = &HE00C000B
            MC_INVALID_DOWNLOAD_FILE_TYPE = &HE00C000C
            MC_OPEN_ENI_ERR = &HE00C000D
            MC_FILE_OPEN_FAIL = &HE00C000E
            MC_NO_MATCHING_DOWNLOADINFORMATION = &HE00C000F
            MC_NONE_OP = &HE00C0010
            MC_FAIL_GEN_DOWNLOAD_FILE = &HE00C0011
            MC_REG_KEY_READ_FAIL = &HE00C0012
            MC_NOT_ALLOWED_IN_THIS_MASTER_MODE = &HE00C0014
            MC_MASTERID_OUT_OF_RANGE = &HE00C0015
            MC_BOARDNO_OUT_OF_RANGE = &HE00C0016
            MC_AXISNO_OUT_OF_RANGE = &HE00C0017
            MC_BOARDCNT_OUT_OF_RANGE = &HE00C0018
            MC_RETURN_SIZE_NOT_EQUAL = &HE00C001A
            MC_MASTERID_DUPLICATION_ERR = &HE00C001B
            MC_PARAM_ERROR_FILE_IS_NULL = &HE00C001C
            MC_NO_MATCHING_BOARDID_FOUND = &HE00C001D
            MC_NOT_READY_NETWORK_CONFIGURATION = &HE00C001E
            MC_INVALID_MASTERID_ERR = &HE00C001F
            MC_MASTER_MODE_CHANGE_NOT_ALLOWED = &HE00C0020
            MC_MASTER_REQUEST_PARAM_ERROR = &HE00C0021
            MC_MASTER_INVALID_STATE = &HE00C0022
            MC_NOT_MOTION_LIBRAY_INITIALIZED = &HE00C0023 '//2014.08.22 Ver_0c01000D_2
            MC_IOMANAGER_NOT_RUNNING = &HE00C0024 '//2014.08.22 Ver_0c01000D_2
            MC_ANOTHER_PROGRAM_IS_USING_NMC_LIBRARY = &HE00C0025 '//2014.10.02 Ver_0c010010_2
            MC_SLAVE_ITEM_MISMATCH = &HE00C0026
            MC_SLAVE_ITEM_COUNT_MISMATCH = &HE00C0027

            MC_PCICIP_GEN_10 = &HCC100000
            COMM_CONNECTION_ESTABLISHED = &HED000001
            COMM_CONN_CONFIG_FAILED_INVALID_NETWORK_PATH = &HED000002
            COMM_CONN_CONFIG_FAILED_NO_RESPONSE = &HED000003
            COMM_CONN_CONFIG_FAILED_ERROR_RESPONSE = &HED000004
            COMM_CONNECTION_TIMED_OUT = &HED000005
            COMM_CONNECTION_CLOSED = &HED000006
            COMM_INCOMING_CONNECTION_RUN_IDLE_FLAG_CHANGED = &HED000007
            COMM_ASSEMBLY_NEW_INSTANCE_DATA = &HED000008
            COMM_ASSEMBLY_NEW_MEMBER_DATA = &HED000009
            COMM_CONNECTION_NEW_INPUT_SCANNER_DATA = &HED00000A
            COMM_CONNECTION_VERIFICATION = &HED00000B
            COMM_CONNECTION_RECONFIGURED = &HED00000C
            COMM_REQUEST_RESPONSE_RECEIVED = &HED000064
            COMM_REQUEST_FAILED_INVALID_NETWORK_PATH = &HED000065
            COMM_REQUEST_TIMED_OUT = &HED000066
            COMM_CLIENT_OBJECT_REQUEST_RECEIVED = &HED000067
            COMM_NEW_CLASS3_RESPONSE = &HED000068
            COMM_CLIENT_PCCC_REQUEST_RECEIVED = &HED000069
            COMM_NEW_LIST_IDENTITY_RESPONSE = &HED00006A
            COMM_ID_RESET = &HED00006B
            COMM_BACKPLANE_REQUEST_RECEIVED = &HED00006C
            COMM_OUT_OF_MEMORY = &HED0000C8
            COMM_UNABLE_INTIALIZE_WINSOCK = &HED0000C9
            COMM_UNABLE_START_THREAD = &HED0000CA
            COMM_ERROR_USING_WINSOCK = &HED0000CB
            COMM_ERROR_SETTING_SOCKET_TO_NONBLOCKING = &HED0000CC
            COMM_ERROR_SETTING_TIMER = &HED0000CD
            COMM_SESSION_COUNT_LIMIT_REACHED = &HED0000CE
            COMM_CONNECTION_COUNT_LIMIT_REACHED = &HED0000CF
            COMM_PENDING_REQUESTS_LIMIT_REACHED = &HED0000D0
            COMM_PENDING_REQUEST_GROUPS_LIMIT_REACHED = &HED0000D1
            COMM_ERROR_UNABLE_START_MODBUS = &HED0000D2
            COMM_ERROR_HW_NOT_INSTALLED = &HED0000DC
            COMM_ERROR_DD_SEND_ERROR = &HED0000DD
            COMM_ERROR_DD_READ_ERROR = &HED0000DE
            COMM_DD_ERROR_SEND = &HED0000DF
            COMM_DD_ERROR_RECV = &HED0000E0
            COMM_DD_OPEN_FAIL = &HED0000E6
            COMM_DD_NOT_OPENED = &HED0000E7
            COMM_DD_CONN_FAIL = &HED0000E8
            COMM_DD_CLIENT_START_FAIL = &HED0000E9
            COMM_DD_OK = &HED000000
            COMM_CN_NOT_CONNECTED = &HED0000F0
            COMM_CN_CONNECTED = &HED0000F1
            COMM_CN_CONNERROR = &HED0000F2
            COMM_ERROR_SUCCESS = &HEE000000
            COMM_ERROR_FAILURE = &HEE010000
            COMM_EXT_ERR_DUPLICATE_FWD_OPEN = &HEE010100
            COMM_EXT_ERR_CLASS_TRIGGER_INVALID = &HEE010103
            COMM_EXT_ERR_OWNERSHIP_CONFLICT = &HEE010106
            COMM_EXT_ERR_CONNECTION_NOT_FOUND = &HEE010107
            COMM_EXT_ERR_INVALID_CONN_TYPE = &HEE010108
            COMM_EXT_ERR_INVALID_CONN_SIZE = &HEE010109
            COMM_EXT_ERR_DEVICE_NOT_CONFIGURED = &HEE010110
            COMM_EXT_ERR_RPI_NOT_SUPPORTED = &HEE010111
            COMM_EXT_ERR_CONNECTION_LIMIT_REACHED = &HEE010113
            COMM_EXT_ERR_VENDOR_PRODUCT_CODE_MISMATCH = &HEE010114
            COMM_EXT_ERR_PRODUCT_TYPE_MISMATCH = &HEE010115
            COMM_EXT_ERR_REVISION_MISMATCH = &HEE010116
            COMM_EXT_ERR_INVALID_CONN_POINT = &HEE010117
            COMM_EXT_ERR_INVALID_CONFIG_FORMAT = &HEE010118
            COMM_EXT_ERR_NO_CONTROLLING_CONNECTION = &HEE010119
            COMM_EXT_ERR_TARGET_CONN_LIMIT_REACHED = &HEE01011A
            COMM_EXT_ERR_RPI_SMALLER_THAN_INHIBIT = &HEE01011B
            COMM_EXT_ERR_CONNECTION_TIMED_OUT = &HEE010203
            COMM_EXT_ERR_UNCONNECTED_SEND_TIMED_OUT = &HEE010204
            COMM_EXT_ERR_PARAMETER_ERROR = &HEE010205
            COMM_EXT_ERR_MESSAGE_TOO_LARGE = &HEE010206
            COMM_EXT_ERR_UNCONN_ACK_WITHOUT_REPLY = &HEE010207
            COMM_EXT_ERR_NO_BUFFER_MEMORY_AVAILABLE = &HEE010301
            COMM_EXT_ERR_BANDWIDTH_NOT_AVAILABLE = &HEE010302
            COMM_EXT_ERR_TAG_FILTERS_NOT_AVAILABLE = &HEE010303
            COMM_EXT_ERR_REAL_TIME_DATA_NOT_CONFIG = &HEE010304
            COMM_EXT_ERR_PORT_NOT_AVAILABLE = &HEE010311
            COMM_EXT_ERR_LINK_ADDR_NOT_AVAILABLE = &HEE010312
            COMM_EXT_ERR_INVALID_SEGMENT_TYPE_VALUE = &HEE010315
            COMM_EXT_ERR_PATH_CONNECTION_MISMATCH = &HEE010316
            COMM_EXT_ERR_INVALID_NETWORK_SEGMENT = &HEE010317
            COMM_EXT_ERR_INVALID_LINK_ADDRESS = &HEE010318
            COMM_EXT_ERR_SECOND_RESOURCES_NOT_AVAILABLE = &HEE010319
            COMM_EXT_ERR_CONNECTION_ALREADY_ESTABLISHED = &HEE01031A
            COMM_EXT_ERR_DIRECT_CONN_ALREADY_ESTABLISHED = &HEE01031B
            COMM_EXT_ERR_MISC = &HEE01031C
            COMM_EXT_ERR_REDUNDANT_CONNECTION_MISMATCH = &HEE01031D
            COMM_EXT_ERR_NO_MORE_CONSUMER_RESOURCES = &HEE01031E
            COMM_EXT_ERR_NO_TARGET_PATH_RESOURCES = &HEE01031F
            COMM_EXT_ERR_VENDOR_SPECIFIC = &HEE010320
            COMM_ERROR_NO_RESOURCE = &HEE020000
            COMM_ERROR_INVALID_PARAMETER_VALUE = &HEE030000
            COMM_ERROR_INVALID_SEG_TYPE = &HEE040000
            COMM_ERROR_INVALID_DESTINATION = &HEE050000
            COMM_ERROR_PARTIAL_DATA = &HEE060000
            COMM_ERROR_CONN_LOST = &HEE070000
            COMM_ERROR_BAD_SERVICE = &HEE080000
            COMM_ERROR_BAD_ATTR_DATA = &HEE090000
            COMM_ERROR_ATTR_LIST_ERROR = &HEE0A0000
            COMM_ERROR_ALREADY_IN_REQUESTED_MODE = &HEE0B0000
            COMM_ERROR_OBJECT_STATE_CONFLICT = &HEE0C0000
            COMM_ERROR_OBJ_ALREADY_EXISTS = &HEE0D0000
            COMM_ERROR_ATTR_NOT_SETTABLE = &HEE0E0000
            COMM_ERROR_PERMISSION_DENIED = &HEE0F0000
            COMM_ERROR_DEV_IN_WRONG_STATE = &HEE100000
            COMM_ERROR_REPLY_DATA_TOO_LARGE = &HEE110000
            COMM_ERROR_FRAGMENT_PRIMITIVE = &HEE120000
            COMM_ERROR_NOT_ENOUGH_DATA = &HEE130000
            COMM_ERROR_ATTR_NOT_SUPPORTED = &HEE140000
            COMM_ERROR_TOO_MUCH_DATA = &HEE150000
            COMM_ERROR_OBJ_DOES_NOT_EXIST = &HEE160000
            COMM_ERROR_NO_FRAGMENTATION = &HEE170000
            COMM_ERROR_DATA_NOT_SAVED = &HEE180000
            COMM_ERROR_DATA_WRITE_FAILURE = &HEE190000
            COMM_ERROR_REQUEST_TOO_LARGE = &HEE1A0000
            COMM_ERROR_RESPONSE_TOO_LARGE = &HEE1B0000
            COMM_ERROR_MISSING_LIST_DATA = &HEE1C0000
            COMM_ERROR_INVALID_LIST_STATUS = &HEE1D0000
            COMM_ERROR_SERVICE_ERROR = &HEE1E0000
            COMM_ERROR_VENDOR_SPECIFIC = &HEE1F0000
            COMM_ERROR_INVALID_PARAMETER = &HEE200000
            COMM_ERROR_WRITE_ONCE_FAILURE = &HEE210000
            COMM_ERROR_INVALID_REPLY = &HEE220000
            COMM_ERROR_BAD_KEY_IN_PATH = &HEE250000
            COMM_ERROR_BAD_PATH_SIZE = &HEE260000
            COMM_ERROR_UNEXPECTED_ATTR = &HEE270000
            COMM_ERROR_INVALID_MEMBER = &HEE280000
            COMM_ERROR_MEMBER_NOT_SETTABLE = &HEE290000
            COMM_ERROR_UNKNOWN_MODBUS_ERROR = &HEE2B0000
            COMM_ERROR_HW_NOT_INSTALLED1 = &HEE2C0000
            COMM_ERROR_ENCAP_PROTOCOL = &HEE6A0000
            COMM_ERROR_STILL_PROCESSING = &HEEFF0000
            MC_DOWNLOAD_FAIL_DUE_TO_ANOTHER_PROGRAM_IS_RUNNING = &HEE800000 '//Ver_0c010012_1
        End Enum

        ''//============================================================================
        ''//                              > ENUMERATED TYPES <
        ''//----------------------------------------------------------------------------
        ''// EtherCat Master
        Public Enum EcState
            eST_UNKNOWN = 0
            eST_INIT = &H1      '// Ver_0c010016_2
            eST_PREOP = &H2     '// Ver_0c010016_2
            eST_SAFEOP = &H4    '// Ver_0c010016_2
            eST_OP = &H8        '// Ver_0c010016_2
            eST_ACKERR = &H10   '// Ver_0c010016_2
        End Enum

        Public Enum EcMstMode
            eMM_IDLE = 0
            eMM_SCAN
            eMM_RUN
            eMM_INTRANSITION
            eMM_ERR
            eMM_LINKBROKEN
        End Enum

        Public Enum EcScanMode
            SCAN_ALL = 0
            SCAN_SINGLE
        End Enum

        Public Enum EcScanSts
            SCAN_NONE = 0
            SCAN_BUSY
            SCAN_DONE
        End Enum

        ''// IO
        Public Enum IOBufMode
            BUF_OUT = 0
            BUF_IN = 1
        End Enum

        ''// Motion
        ''//MC_ReadAxisStatus
        Public Enum MC_AXISSTATUS
            mcErrorStop = &H1&
            mcDisabled = &H2&
            mcStopping = &H4&
            mcStandStill = &H8&
            mcDiscreteMotion = &H10&
            mcContinuousMotion = &H20&
            mcSynchroMotion = &H40&
            mcHoming = &H80&
            mcReserved_as_8 = &H100&
            mcReserved_as_9 = &H200&
            mcConstantVelocity = &H400&
            mcAccelerating = &H800&
            mcDecelerating = &H1000&
            mcDirectionPositive = &H2000&
            mcDirectionNegative = &H4000&
            mcLimitSwitchNeg = &H8000&
            mcLimitSwitchPos = &H10000&
            mcHomeAbsSwitch = &H20000&
            mcLimitSwitchPosEvent = &H40000&
            mcLimitSwitchNegEvent = &H80000&
            mcDriveFault = &H100000&
            mcSensorStop = &H200000&
            mcReadyForPowerOn = &H400000&
            mcPowerOn = &H800000&
            mcIsHomed = &H1000000&
            mcAxisWarning = &H2000000&
            mcMotionComplete = &H4000000&
            mcGearing = &H8000000&
            mcGroupMotion = &H10000000&
            mcBufferFull = &H20000000&
            mcReserved_as_30 = &H40000000
            mcReserved_as_31 = &H80000000
        End Enum

        '//MC_ReadStatus
        Public Enum StatusTag
            mcASErrorStop = &H1&
            mcASDisabled = &H2&
            mcASStopping = &H4&
            mcASStandStill = &H8&
            mcASDiscreteMotion = &H10&
            mcASContinuousMotion = &H20&
            mcASSynchroMotion = &H40&
            mcASHoming = &H80&
        End Enum

        '//MC_ReadMotionState
        Public Enum MC_MOTIONSTATE
            mcMSConstantVelocity = &H1&
            mcMSAccelerating = &H2&
            mcMSDecelerating = &H4&
            mcMSDirectionPositive = &H8&
            mcMSDirectionNegative = &H10&
        End Enum

        Public Enum MC_AXISINFO
            mcAIHomeAbsSwitch = &H1&
            mcAILimitSwitchPos = &H2&
            mcAILimitSwitchNeg = &H4&
            mcAIReserved3 = &H8&
            mcAIReserved4 = &H10&
            mcAIReadyForPowerOn = &H20&
            mcAIPowerOn = &H40&
            mcAIIsHomed = &H80&
            mcAIAxisWarining = &H100&
            mcAIMotionComplete = &H200&
            mcAIGearing = &H400&
            mcAIGroupMotion = &H800&
            mcAIBufferFull = &H1000&
            mcAIReseved13 = &H2000&
        End Enum

        Public Enum MC_AXISERROR
            mcAxis_NO_ERROR = &H0&
            mcAxis_DEVICE_ERROR = &H1&
            mcAxis_INVALID_AXIS_STATE = &H2&
            mcAxis_PARAMETER_INVALID = &H3&
            mcAxis_UNSUPPORT_CMD_REQUEST = &H4&
            mcAxis_CMD_REQUEST_FORMAT_WRONG = &H5&
            mcAxis_RESOURCE_ERROR = &H6&
            mcAxis_CONFIG_INVALID = &H7&
            mcAxis_POSITION_FOLLOWING_ERROR = &H8&
            mcAxis_VELOCITY_FOLLOWING_ERROR = &H9&
            mcAxis_SYSTEM_MAX_VELOCITY_OVER_ERROR = &HA&
            mcAxis_SYSTEM_MAX_ACCEL_OVER_ERROR = &HB&
            mcAxis_SYSTEM_MAX_DECEL_OVER_ERROR = &HC&
            mcAxis_SYSYEM_MAX_JERK_OVER_ERROR = &HD&
            mcAxis_MALFUNCTION_ERROR = &HE&
            mcAxis_GEARING_RULE_VIOLATION = &HF&
            mcAxis_HW_LIMIT_REACHED_WARNING = &H8001&
            mcAxis_SW_LIMIT_REACHED_WARNING = &H8002&
        End Enum

        Structure MC_AxisErrorInfo
            Public ErrorId As UShort
            Public ErrorInfo0 As UShort
            Public ErrorInfo1 As UShort
        End Structure

        Public Enum MC_SOURCE
            mcSetValue = 0      'Synchronization on master set value
            mcActualValue       'Synchronization on master actual value
            mcSetValueFixedGear = &H10
            mcActualValueFixedGear = &H11
        End Enum

        Public Enum MC_EXECUTION_MODE
            mcImmediately = 0
            mcQueued
        End Enum

        Public Enum mcParamID
            mcpCommandedPosition = 1
            mcpSWLimitPos_ = 2
            mcpSWLimitNeg_ = 3
            mcpEnableLimitPos_ = 4
            mcpEnableLimitNeg_ = 5
            mcpEnablePosLagMonitoring = 6
            mcpMaxPositionLag_ = 7
            mcpMaxVelocitySystem_ = 8
            mcpMaxVelocityAppl_ = 9
            mcpActualVelocity = 10
            mcpCommandedVelocity = 11
            mcpMaxAccelerationSystem_ = 12
            mcpMaxAccelerationAppl = 13
            mcpMaxDecelerationSystem_ = 14
            mcpMaxDecelerationAppl = 15
            mcpMaxJerkSystem = 16
            mcpMaxJerkAppl_ = 17
            mcpActualPosition = 1000
            mcpCommandedAccel = 1001
            mcpActualAccel = 1002
            mcpCommandedJerk = 1003
            mcpActualJerk = 1004
            mcpTotalBufferCount = 1010
            mcpAvailableBufferCount = 1011
            mcpAxisType = 2002
            mcpModuloAxis = 2003
            mcpModuloValue = 2004
            mcpEnableHWLimitPos = 2010
            mcpHWLimitPosInputNum = 2011
            mcpHWLimitPosActLevel = 2012
            mcpEnableHWLimitNeg = 2013
            mcpHWLimitNegInputNum = 2014
            mcpHWLimitNegActLevel = 2015
            mcpHomeInputNum = 2016
            mcpHomeActLevel = 2017
            mcpMarkerInputNum = 2018
            mcpMarkerActLevel = 2019
            mcpInputActLevel = 2020
            mcpEnableLimitPos = 2030
            mcpSWLimitPos = 2031
            mcpEnableLimitNeg = 2032
            mcpSWLimitNeg = 2033
            mcpMaxVelocityAppl = 2034
            mcpMaxAccelAppl = 2035
            mcpMaxDecelAppl = 2036
            mcpMaxJerkAppl = 2037
            mcpMaxVelocitySystem = 2038
            mcpMaxAccelerationSystem = 2039
            mcpMaxDecelerationSystem = 2040
            mcpmcpMaxJerkSystem = 2041
            mcpmcpEStopType = 2060
            mcpEStopDecel = 2061
            mcpEStopJerk = 2062
            mcpInvertCmdDir = 2070
            mcpCmdScaleFactor = 2071
            mcpFeedbackMode = 2072
            mcpInvertFeedbackDir = 2073
            mcpFeedbackScaleFactor = 2074
            mcpPositionFeedbackFilter = 2075
            mcpVelocityFeedbackFilter = 2076
            mcpAccelerationFeedbackFilter = 2077
            mcpStartVelocityOffset = 2078
            mcpStopVelocityOffset = 2079
            mcpInPositionCheckType = 2080
            mcpInPositionWindowSize = 2081
            mcpInVelocityWindowSize = 2082
            mcpEnablePositionLagMonit = 2083
            mcpMaxPositionLag = 2084
            mcpPositionLagCalMethod = 2085
            mcpEnableVelocityLagMonit = 2086
            mcpMaxVelocityLag = 2087
            mcpVelocityLagCalMethod = 2088
            mcpHomingType = 2100
            mcpHomingDir = 2101
            mcpHomingVelocity = 2102
            mcpHomingAcceleration = 2103
            mcpHomingDeceleration = 2104
            mcpHomingJerk = 2105
            mcpHomingCreepVelocity = 2106
            mcpHomePositionOffset = 2107
            mcpHomeCompleteFlagHandle = 2108
            '//v12.1.0.33 syna
            mcpSensor0StopEnable = 2109
            mcpSensor0StopMode = 2110
            mcpSensor0StopIOOffset = 2111
            mcpSensor0StopIOSize = 2112
            mcpSensor0StopIOBit = 2113
            mcpSensor0StopPosOffset = 2114
            mcpSensor1StopEnable = 2115
            mcpSensor1StopMode = 2116
            mcpSensor1StopIOOffset = 2117
            mcpSensor1StopIOSize = 2118
            mcpSensor1StopIOBit = 2119
            mcpSensor1StopPosOffset = 2120
            mcpSensor2StopEnable = 2121
            mcpSensor2StopMode = 2122
            mcpSensor2StopIOOffset = 2123
            mcpSensor2StopIOSize = 2124
            mcpSensor2StopIOBit = 2125
            mcpSensor2StopPosOffset = 2126
            mcpSensor3StopEnable = 2127
            mcpSensor3StopMode = 2128
            mcpSensor3StopIOOffset = 2129
            mcpSensor3StopIOSize = 2130
            mcpSensor3StopIOBit = 2131
            mcpSensor3StopPosOffset = 2132
            mcpSensor4StopEnable = 2133
            mcpSensor4StopMode = 2134
            mcpSensor4StopIOOffset = 2135
            mcpSensor4StopIOSize = 2136
            mcpSensor4StopIOBit = 2137
            mcpSensor4StopPosOffset = 2138
        End Enum

        Public Enum MC_BUFFER_MODE
            '0:Aborting, 1:Buffered, 2:BlendingLow, 3:BlendingPrevious, 4:BlendingNext, 5:BlendingHigh
            mcAborting = 0
            mcBuffered
            mcBlendingLow
            mcBlendingPrevious
            mcBlendingNext
            mcBlendingHigh
            mcBufferedBlendingLow
            mcBufferedBlendingPrevious
            mcBufferedBlendingNext
            mcBufferedBlendingHigh
        End Enum

        Public Enum MC_DIRECTION
            ' 0:Positive Direction, 1:Shortest_way, 2:Negative_Direction, 3:Current_Direction
            mcPositiveDirection = 0
            mcShortestWay
            mcNegativeDirection
            mcCurrentDirection
        End Enum

        Public Enum MC_GearStatus
            mcGearActive = &H1&
            mcGearIn = &H2&
            mcGearReserved2 = &H4&
            mcGearReserved3 = &H8&
            mcGearReserved4 = &H10&
            mcGearReserved5 = &H20&
            mcGearReserved6 = &H40&
            mcGearReserved7 = &H80&
            mcGearReserved8 = &H100&
            mcGearReserved9 = &H200&
            mcGearReserved10 = &H400&
            mcGearReserved11 = &H800&
            mcGearReserved12 = &H1000&
            mcGearReserved13 = &H2000&
            mcGearReserved14 = &H4000&
            mcGearReserved15 = &H8000&
        End Enum

        '//MC_AXIS_OBJ_INST_ATTR_ERROR_LIMIT_STOP_TYPE					21
        Public Enum AxisStopType
            IMMEDIATE = 0
            DECEL
        End Enum

        '//MC_AXIS_OBJ_INST_ATTR_HOMING_DIRECTION							22
        Public Enum MC_AXIS_DIRECTION
            CW = 0
            CCW
        End Enum

        '//MC_AXIS_OBJ_INST_ATTR_CONTROL_TYPE								23
        Public Enum MC_AXIS_CONTROL
            OL_PULSE_DIRECTION = 0
            OL_TWO_PULSE
            OL_QUDARATURE_PULSE
            CL_VELOCITY_LEVEL_ANALOG
            CL_TORQUE_LEVEL_ANALOG
        End Enum

        '//MC_AXIS_OBJ_INST_ATTR_POSITION_I_MODE							34
        Public Enum MC_AXIS_I_MODE
            IN_STANDING = 0
            ALWAYS
        End Enum

        '// Flash Download Binary File Type
        Public Enum BinFileType
            BIN_BOOT = 1
            BIN_A8OS = 2
            BIN_TM = 3
            BIN_DSPOS = 5
            BIN_ENI = 6
        End Enum

        Public Enum SWVerType
            SW_VER_MOTION = 0
            SW_VER_PCICIP
            SW_VER_DDSDK
            SW_VER_DD
        End Enum

        Public Enum MC_COORDSYSTEM
            mcACS = 1
            mcMCS
            mcPCS
        End Enum

        Public Enum MC_TRANSITION_MODE
            mcTMNone = 0            'Insert no transition curve (default mode)
            mcTMStartVelocity       'Transition with given start velocity
            mcTMConstantVelocity    'Transition with given constant velocity
            mcTMCornerDistance      'Transition with given corner distance
            mcTMMaxCornerDeviation  'Transition with given maximum corner deviation
            '5 - 9 Reserved by PLCopen
            '10 -… Supplier specific modes
        End Enum

        Public Enum MC_CIRC_MODE
            mcBORDER = 0
            mcCENTER
            mcRADIUS
            mcCENTER_ANGLE = 10
            mcBORDER_ANGLE = 11
        End Enum

        Public Enum MC_CIRC_PATHCHOICE
            mcClockWise = 0
            mcCounterClockWise
        End Enum

        Public Enum MC_GroupStatus
            GroupMoving = &H1&
            GroupHoming = &H2&
            GroupErrorStop = &H4&
            GroupStandby = &H8&
            GroupStopping = &H10&
            GroupDisabled = &H20&
            ConstantVelocity = &H40&
            Accelerating = &H80&
            Decelerating = &H100&
            InPosition = &H200&
        End Enum

        '//============================================================================
        '//                              > Callback <
        '//----------------------------------------------------------------------------
        Public Const MAX_PATH = 260

        '// File download callback function prototype.
        Structure FlashStatus
            Public Status As UInteger
            Public StepNum As UInteger
            Public Progress As UInteger
            Public Message() As Byte
        End Structure

        'typedef void (*fnCallbackDownload) (FlashStatus *); 

        '//============================================================================
        '//                              > APIs <
        '//----------------------------------------------------------------------------
        '// Initialization
        '// General & Utiltiy Functions

        '//===============================================================================
        '// MC_STATUS MC_STATUS MC_Init(void)
        '// Desc :: 설치된 모든 보드를 초기화 한다.
        '// Param ::
        '//     IN : 
        '// return ::
        '//     No Error :
        '//         MC_OK
        '//     Error :
        '//         
        '//         
        '//===============================================================================
        Declare Function MC_Init Lib "NMC_Motion.dll" () As MC_STATUS

        '//===============================================================================
        '// MC_STATUS MC_MasterInit(UINT16 MasterID)
        '// Desc :: BoardID 를 입력받아서 Master 를 초기화 한다.
        '// Param ::
        '//     IN : UINT16 MasterID     : 초기화 하고자 하는 Master 의 ID 
        '// return ::
        '//     No Error :
        '//         MC_OK
        '//     Error :
        '//         
        '//         
        '//===============================================================================
        Declare Function MC_MasterInit Lib "NMC_Motion.dll" (ByVal MasterID As UInt16) As MC_STATUS

        '//===============================================================================
        '// MC_STATUS MC_MasterRUN(UINT16 MasterID)
        '// Desc :: BoardID 를 입력받아서 Master의 EtherCAT 동작 상태를 RUN 상태로 만든다.
        '// Param ::
        '//     IN : UINT16 MasterID     : Master 의 ID 
        '// return ::
        '//     No Error :
        '//         MC_OK
        '//     Error :
        '//         MC_NOT_READY_NETWORK_CONFIGURATION
        '//         
        '//===============================================================================
        Declare Function MC_MasterRUN Lib "NMC_Motion.dll" (ByVal MasterID As UInt16) As MC_STATUS

        '//===============================================================================
        '// MC_STATUS MC_MasterSTOP(UINT16 MasterID)
        '// Desc :: BoardID 를 입력받아서 Master의 EtherCAT 동작 상태를 STOP 상태로 만든다.
        '// Param ::
        '//     IN : UINT16 MasterID     : Master 의 ID 
        '// return ::
        '//     No Error :
        '//         MC_OK
        '//     Error :
        '//         
        '//         
        '//===============================================================================
        Declare Function MC_MasterSTOP Lib "NMC_Motion.dll" (ByVal MasterID As UInt16) As MC_STATUS

        '//===============================================================================
        '// MC_STATUS MC_GetSWVersion(UINT16 Type, UINT16 * Major, UINT16 *Minor)
        '// Desc :: Version 정보를 얻는다.
        '// Param ::
        '//     OUT : UINT16 * MasterCount : 설치된 보드 갯수
        '// return ::
        '//     No Error :
        '//         MC_OK
        '//     Error :
        '//         
        '//===============================================================================
        Declare Function MC_GetSWVersion Lib "NMC_Motion.dll" (
                                                              ByVal Type As UInt16,
                                                              ByRef Major As UInt16,
                                                              ByRef Minor As UInt16
                                                              ) As MC_STATUS

        '//===============================================================================
        '// MC_STATUS MC_GetErrorMessage(UINT32 ErrorCode, ByVal Size As UInteger, char * ErrorMessage)
        '// Desc :: ErrorCode 에 대한 문자열을 얻는다.
        '// Param ::
        '//     IN  : UINT32 ErrorCode; : Error Code
        '//     IN  : ByVal Size As UInteger ; Size should be MAX_ERR_LEN;
        '//     OUT : char * ErrorMessage; : 상응하는 문자열
        '// return ::
        '//     No Error :
        '//         MC_OK
        '//     Error :
        '//         
        '//===============================================================================
        'Need to Check
        Declare Function MC_GetErrorMessage Lib "NMC_Motion.dll" (
                                                                 ByVal ErrorCode As Int32,
                                                                 ByVal Size As UInt32,
                                                                 ByVal pErrorMessage As String
                                                                 ) As MC_STATUS

        '//---------------------------------------------------------------------------
        '// BoardID : BoardSwitchID
        '// nFileType : Binary File Type :
        '// sNvsFileName : FullPath File Name : c:\\test\\NMC.nvs
        '// sBinFileName : FullPath File Name : c:\\test\\xxx.bin or c:\\test\\xxx.xml ...
        '//
        '//---------------------------------------------------------------------------
        '	Declare Function  FileDownloadCB(
        '									ByVal BoardID As UInt16, 
        '									LPVOID lpCallbackFunc, '// Callback Function
        '									int nFileType, 
        '									char *sNvsFileName, 
        '									char *sBinFileName
        '									) As MC_STATUS	
        '	Declare Function  MC_STATUS FileMasterFWDownloadCB(
        '									ByVal BoardID As UInt16, 
        '									LPVOID lpCallbackFunc,
        '									int nFileType, 
        '									char *sNvsFileName, 
        '									char * sBinFileName
        '									);

        '//---------------------------------------------------------------------------
        '// BoardID : BoardSwitchID
        '// nFileType : Binary File Type :
        '// sNvsFileName : FullPath File Name : c:\\test\\NMC.nvs
        '// sBinFileName : FullPath File Name : c:\\test\\xxx.bin or c:\\test\\xxx.xml ...
        '//
        '//---------------------------------------------------------------------------
        Declare Function FileDownload Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal nFileType As Integer,
                ByVal sNvsFileName As String,
                ByVal sBinFileName As String,
                ByVal nMsgSize As Integer,
                ByVal szMsg As String
                ) As MC_STATUS
        '//===========================================================================
        '// Master Utility APIs
        '//---------------------------------------------------------------------------
        '//===============================================================================
        '// MC_STATUS MC_GetMasterMap(UINT16 MasterMap[MAX_BOARD_CNT], UINT16 * MasterCount)
        '// Desc :: 현재 설치된 모든 보드의 ID List 를 돌려준다.
        '// Param ::
        '//     OUT : UINT16 MasterMap[MAX_BOARD_CNT] : 설치된 보드 ID List
        '//     OUT : UINT16 * MasterCount : 설치된 보드 갯수
        '// return ::
        '//     No Error :
        '//         MC_OK
        '//     Error :
        '//         MC_NO_BOARD_INSTALLED
        '//===============================================================================
        Declare Function MC_GetMasterMap Lib "NMC_Motion.dll" (
                ByVal MasterMap() As UInt16,
                ByRef MasterCount As UInt16
                ) As MC_STATUS
        '//===============================================================================
        '// MC_STATUS MC_GetMasterCount(UINT16 * MasterCount)
        '// Desc :: 현재 설치된 보드의 갯수를 리턴한다.
        '// Param ::
        '//     OUT : UINT16 * MasterCount : 설치된 보드 갯수
        '// return ::
        '//     No Error :
        '//         MC_OK
        '//     Error :1
        '//         MC_NO_BOARD_INSTALLED
        '//===============================================================================
        Declare Function MC_GetMasterCount Lib "NMC_Motion.dll" (
                            ByRef MasterCount As UInt16
                            ) As MC_STATUS

        '//===============================================================================
        '// MC_STATUS  MC_GetBoardScanNo(ByVal BoardID As UInt16, UINT16 * BoardScanNo)
        '// Desc :: BoardID 에 해당하는 Master 의 Scan Number 를 돌려준다.
        '// Param ::
        '//     IN : ByVal BoardID As UInt16
        '//     OUT : UINT16 * BoardScanNo
        '// return ::
        '//     No Error :
        '//         MC_OK
        '//     Error :
        '//         MC_MASTERID_OUT_OF_RANGE
        '//===============================================================================
        Declare Function MC_GetBoardScanNo Lib "NMC_Motion.dll" (
                             ByVal BoardID As UInt16,
                             ByRef BoardScanNo As UInt16
                             ) As MC_STATUS

        Declare Function MasterGetSDOData Lib "NMC_Motion.dll" (
                            ByVal BoardID As UInt16,
                            ByVal SlaveNo As UInt16,
                            ByVal SDOIndex As UInt16,
                            ByVal SubIndex As Byte,
                            ByVal DataSize As UInteger,
                            ByRef respDataSize As UInteger,
                            ByRef bDataArray() As Byte
                            ) As MC_STATUS

        '// 40h-1 - MasterGetSDODataEcatAddr
        Declare Function MasterGetSDODataEcatAddr Lib "NMC_Motion.dll" (
                            ByVal BoardID As UInt16,
                            ByVal EcatAddr As UInt16,
                            ByVal SDOIndex As UInt16,
                            ByVal SubIndex As Byte,
                            ByVal DataSize As UInteger,
                            ByRef respDataSize As UInteger,
                            ByRef bDataArray() As Byte
                            ) As MC_STATUS

        Declare Function MasterSetSDOData Lib "NMC_Motion.dll" (
                            ByVal BoardID As UInt16,
                            ByVal SlaveNo As UInt16,
                            ByVal SDOIndex As UInt16,
                            ByVal SubIndex As Byte,
                            ByVal DataSize As UInteger,
                            ByRef respDataSize As UInteger,
                            ByRef bDataArray() As Byte
                            ) As MC_STATUS
        '// 41h-1 - DnloadSDOData
        Declare Function MasterSetSDODataEcatAddr Lib "NMC_Motion.dll" (
                            ByVal BoardID As UInt16,
                            ByVal EcatAddr As UInt16,
                            ByVal SDOIndex As UInt16,
                            ByVal SubIndex As Byte,
                            ByVal DataSize As UInteger,
                            ByRef respDataSize As UInteger,
                            ByRef bDataArray() As Byte
                            ) As MC_STATUS

        Declare Function MasterGetCurMode Lib "NMC_Motion.dll" (
                            ByVal BoardID As UInt16,
                            ByRef MasterMode As Byte
                            ) As MC_STATUS

        Declare Function MasterGetBoardID Lib "NMC_Motion.dll" (
                            ByVal MasterScanNo As UInt16,
                            ByRef MasterID As UInt16
                            ) As MC_STATUS

        Declare Function MasterGetOSRevision Lib "NMC_Motion.dll" (
                            ByVal BoardID As UInt16,
                            ByRef Major As Byte,
                            ByRef Minor As Byte
                            ) As MC_STATUS

        Declare Function MasterGetMCRevision Lib "NMC_Motion.dll" (
                            ByVal BoardID As UInt16,
                            ByRef Major As Byte,
                            ByRef Minor As Byte
                            ) As MC_STATUS


        '//===========================================================================
        '// Motion APIs
        '//---------------------------------------------------------------------------
        '//5.1.1.1
        Declare Function MC_Power Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal Enable As Boolean
                ) As MC_STATUS

        '//5.1.1.2
        Declare Function MC_MoveAbsolute Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal Position As Double,
                ByVal Velocity As Double,
                ByVal Accel As Double,
                ByVal Decel As Double,
                ByVal Jerk As Double,
                ByVal Dir As MC_DIRECTION,
                ByVal BufferMode As MC_BUFFER_MODE
                ) As MC_STATUS
        '//5.1.1.3
        Declare Function MC_MoveRelative Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal Distance As Double,
                ByVal Velocity As Double,
                ByVal Accel As Double,
                ByVal Decel As Double,
                ByVal Jerk As Double,
                ByVal BufferMode As MC_BUFFER_MODE
                ) As MC_STATUS
        '//5.1.1.6
        Declare Function MC_MoveVelocity Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal Velocity As Double,
                ByVal Accel As Double,
                ByVal Decel As Double,
                ByVal Jerk As Double,
                ByVal Dir As MC_DIRECTION,
                ByVal BufferMode As MC_BUFFER_MODE
                ) As MC_STATUS
        '//5.1.1.7
        Declare Function MC_Home Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal Position As Double,
                ByVal BufferMode As MC_BUFFER_MODE
                ) As MC_STATUS
        '//5.1.1.8
        Declare Function MC_Stop Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal Execute As Boolean,
                ByVal Decel As Double,
                ByVal Jerk As Double
                ) As MC_STATUS
        '//5.1.1.9 MCReadStatus
        Declare Function MC_ReadStatus Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByRef pStatus As UInteger
                ) As MC_STATUS
        '//5.1.1.10
        Declare Function MC_ReadAxisError Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByRef pErrorID As UInt16,
                ByRef pErrorInfo As UInt16,
                ByRef pErrorInfoExt As UInt16
                ) As MC_STATUS
        '//5.1.1.11
        Declare Function MC_Reset Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16
                ) As MC_STATUS

        '//5.1.1.12 MCReadParameter
        Declare Function MC_ReadParameter Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal ParameterNum As UInteger,
                ByRef pValue As Double
                ) As MC_STATUS

        '//5.1.1.12 MCReadBoolParameter
        Declare Function MC_ReadBoolParameter Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal ParameterNum As UInteger,
                ByRef pValue As Boolean
                ) As MC_STATUS

        '//5.1.1.12 MCReadIntParameter
        Declare Function MC_ReadIntParameter Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal ParameterNum As UInteger,
                ByRef pValue As UInteger
                ) As MC_STATUS

        '//5.1.1.13
        Declare Function MC_WriteParameter Lib "NMC_Motion.dll" (
                 ByVal BoardID As UInt16,
                 ByVal AxisID As UInt16,
                 ByVal ParameterNum As UInteger,
                 ByVal dValue As Double
                 ) As MC_STATUS

        '//5.1.1.13
        Declare Function MC_WriteBoolParameter Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal ParameterNum As UInteger,
                ByVal Value As Boolean
                ) As MC_STATUS

        '//5.1.1.13
        Declare Function MC_WriteIntParameter Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal ParameterNum As UInteger,
                ByVal dwValue As UInteger
                ) As MC_STATUS

        '//5.1.1.14 MCReadActualPosition
        Declare Function MC_ReadActualPosition Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByRef pPosition As Double
                ) As MC_STATUS
        '//5.1.1.21 MC_GearIn 
        Declare Function MC_GearIn Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal MasterAxis As UInt16,
                ByVal SlaveAxis As UInt16,
                ByVal RatioNumerator As UInteger,
                ByVal RatioDenominator As UInteger,
                ByVal MasterValueSource As MC_SOURCE,
                ByVal Acceleration As Double,
                ByVal Deceleration As Double,
                ByVal Jerk As Double,
                ByVal BufferMode As MC_BUFFER_MODE
                ) As MC_STATUS
        '//5.1.1.22 MC_GearOut
        Declare Function MC_GearOut Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal SlaveAxis As UInt16
                ) As MC_STATUS

        '//5.1.1.24
        Declare Function MC_TouchProbe Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal TriggerInput As UInteger,
                ByVal WindowOnly As Boolean,
                ByVal FirstPosition As Double,
                ByVal LastPosition As Double
                ) As MC_STATUS
        '//5.1.1.25
        Declare Function MC_AbortTrigger Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal TriggerInput As UInteger
                ) As MC_STATUS
        '//5.1.1.26
        Declare Function MC_ReadDigitalInput Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal InputNumber As UInteger,
                ByRef pValue As Boolean
                ) As MC_STATUS
        '//5.1.1.27
        Declare Function MC_ReadDigitalOutput Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal OutputNumber As UInteger,
                ByRef pValue As Boolean
                ) As MC_STATUS
        '//5.1.1.28
        Declare Function MC_WriteDigitalOutput Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal OutputNumber As UInteger,
                ByVal Value As Boolean
                ) As MC_STATUS

        '//5.1.1.29
        Declare Function MC_SetPosition Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal Position As Double,
                ByVal Relative As Boolean,
                ByVal Mode As MC_EXECUTION_MODE
                ) As MC_STATUS
        '//5.1.1.31 
        Declare Function MC_ReadActualVelocity Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByRef pVelocity As Double
                ) As MC_STATUS

        '//5.1.1.37
        Declare Function MC_Halt Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisID As UInt16,
                ByVal Decel As Double,
                ByVal Jerk As Double,
                ByVal BufferMode As MC_BUFFER_MODE
                ) As MC_STATUS

        '//5.1.1.38
        Declare Function MC_TriggerMonitor Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxisID As UInt16,
               ByVal TriggerInput As UInteger,
               ByRef pDone As Boolean,
               ByRef pRecordedPosition As Double,
               ByRef pProbeActive As Boolean
               ) As MC_STATUS
        '//5.1.1.39 
        Declare Function MC_ReadMotionState Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxisID As UInt16,
               ByRef pState As UInteger
               ) As MC_STATUS
        '//5.1.1.40 
        Declare Function MC_ReadAxisInfo Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxisID As UInt16,
               ByRef pAxisInfo As UInteger
               ) As MC_STATUS
        '//5.1.1.41 

        Declare Function MC_ReadAxisStatus Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxisID As UInt16,
               ByRef pAxisStatus As UInteger
               ) As MC_STATUS
        '//5.1.1.42 MC_GearMonitor

        Declare Function MC_GearMonitor Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxisID As UInt16,
               ByRef pStatus As UInt16
               ) As MC_STATUS
        '//5.1.1.43 MC_ReadProfileData

        Declare Function MC_ReadProfileData Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxisID As UInt16,
               ByRef pTickCount As UInteger,
               ByRef pPosition As Double,
               ByRef pVelocity As Double,
               ByRef pAccel As Double,
               ByRef pJerk As Double,
               ByRef pActPos As Double,
               ByRef pActVel As Double
               ) As MC_STATUS

        '//MC_ReadMultiAxisStatus
        Declare Function MC_ReadMultiAxisStatus Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxisCount As UInt16,
                ByVal AxisArray() As UInt16,
                ByRef Status As UInteger
                ) As MC_STATUS
        '//===========================================================================
        '//GROUP Motion
        '//---------------------------------------------------------------------------
        '//5.1.1.44 MC_AddAxisToGroup [32]

        Declare Function MC_AddAxisToGroup Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxisID As UInt16,
               ByVal AxesGroupNo As UInt16,
               ByVal IDInGroup As UInt16
               ) As MC_STATUS
        '//5.1.1.45 MC_RemoveAxisFromGroup [33]

        Declare Function MC_RemoveAxisFromGroup Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16,
                ByVal IDInGroup As UInt16
               ) As MC_STATUS
        '//5.1.1.46 MC_UngroupAllAxes [34]

        Declare Function MC_UngroupAllAxes Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16
               ) As MC_STATUS
        '//5.1.1.47 MC_GroupReadConfiguration [35]

        Declare Function MC_GroupReadConfiguration Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16,
               ByVal IDInGroup As UInt16,
               ByVal CoordSystem As MC_COORDSYSTEM,
               ByRef pAxisNo As UInt16
               ) As MC_STATUS
        '//5.1.1.48 MC_GroupEnable [36]

        Declare Function MC_GroupEnable Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16
               ) As MC_STATUS
        '//5.1.1.49 MC_GroupDisable [37]

        Declare Function MC_GroupDisable Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16
               ) As MC_STATUS
        '//5.1.1.50 MC_MoveLinearAbsolute [43]

        '//Coordination System 을 지정한다. '//ACS, MCS, PCS
        Declare Function MC_MoveLinearAbsolute Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16,
               ByVal PositionCount As UInt16,
               ByVal pPositionData As Double(),
               ByVal Velocity As Double,
               ByVal Accel As Double,
               ByVal Decel As Double,
               ByVal Jerk As Double,
               ByVal CoordSystem As MC_COORDSYSTEM,
               ByVal BufferMode As MC_BUFFER_MODE,
               ByVal TransitionMode As MC_TRANSITION_MODE,
               ByVal TransitionParameterCount As UInt16,
               ByVal pTransitionParameter As Double()
               ) As MC_STATUS
        '//5.1.1.51 MC_GroupHalt [42]

        Declare Function MC_GroupHalt Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16,
               ByVal Decel As Double,
               ByVal Jerk As Double,
               ByVal BufferMode As MC_BUFFER_MODE
               ) As MC_STATUS
        '//5.1.1.52 MC_GroupStop [41]

        Declare Function MC_GroupStop Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16,
               ByVal Execute As Boolean,
               ByVal Decel As Double,
               ByVal Jerk As Double
               ) As MC_STATUS

        '//5.1.1.53 MC_MoveCircularAbsolute2D [44]
        'Need to check
        Declare Function MC_MoveCircularAbsolute2D Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16,
               ByVal CircMode As MC_CIRC_MODE,
               ByVal PathChoice As MC_CIRC_PATHCHOICE,
               ByVal pAuxPoint As Double(),
               ByVal pEndPoint As Double(),
               ByVal Angle As Double,
               ByVal Velocity As Double,
               ByVal Accel As Double,
               ByVal Decel As Double,
               ByVal Jerk As Double,
               ByVal CordSystem As MC_COORDSYSTEM,
               ByVal BufferMode As MC_BUFFER_MODE,
               ByVal TransitionMode As MC_TRANSITION_MODE,
               ByVal TransitionParamCount As UInt16,
               ByVal pTransitionParameter As Double()
               ) As MC_STATUS

        '//5.1.1.54 MC_GroupStop [38]
        Declare Function MC_GroupReadStatus Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16,
               ByRef pGroupStatus As UInteger
               ) As MC_STATUS

        '//5.1.1.55 MC_GroupReadError [39]
        Declare Function MC_GroupReadError Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16,
               ByRef pErrorID As UInt16,
               ByRef pErrorInfo0 As UInt16,
               ByRef pErrorInfo1 As UInt16
               ) As MC_STATUS

        '//5.1.1.56 MC_GroupReset [40]
        Declare Function MC_GroupReset Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16
               ) As MC_STATUS

        '//5.1.1.57 MC_GroupReset []
        'Need to Check
        '[0]Pos,Vel,Acc,Jerk,ActPos,ActVel,[1]Pos,Vel,Acc,Jerk,ActPos,ActVel ...
        Declare Function MC_GroupReadProfileData Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal AxesGroupNo As UInt16,
                ByRef AxisCount As UInt16,
                ByRef TimeTick As UInteger,
                ByRef ProfileDataArray()() As Double
                ) As MC_STATUS

        '//5.1.1.58 MC_GroupReadInfo []
        'Need to Check
        Declare Function MC_GroupReadInfo Lib "NMC_Motion.dll" (
               ByVal BoardID As UInt16,
               ByVal AxesGroupNo As UInt16,
               ByRef pLastIdentNum As UInt16,
               ByRef pIdentAxisNumList() As UInt16
               ) As MC_STATUS

        '//5.1.1.59 MC_ReadAllStatus []
        Declare Function MC_ReadAllStatus Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16,
         ByRef StatusData() As UInt32
         ) As MC_STATUS

        '//5.1.1.64 MC_ReadRemainBuffer []
        Declare Function MC_ReadRemainBuffer Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16,
         ByVal AxesNo As UInt16,
         ByRef BufferSize As UInt32
         ) As MC_STATUS

        '//5.1.1.65 MC_GroupReadRemainBuffer []
        Declare Function MC_GroupReadRemainBuffer Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16,
         ByVal AxesGroupNo As UInt16,
         ByRef BufferSize As UInt32
         ) As MC_STATUS


        '//5.1.1.66 MC_MoveAbsoluteMultiAxis [] : //v12.1.0.40
        Declare Function MC_MoveAbsoluteMultiAxis Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16,
         ByVal AxisCount As UInt16,
         ByVal AxisArray() As UInt16,
         ByVal PositionArray() As Double,
         ByVal Velocity As Double,
         ByVal Acceleration As Double,
         ByVal Deceleration As Double,
         ByVal Jerk As Double,
         ByVal DirectionArray() As MC_DIRECTION,
         ByVal ErrorStopMode As Byte
         ) As MC_STATUS

        '//5.1.1.67 MC_MoveRelativeMultiAxis [] : //v12.1.0.40
        Declare Function MC_MoveRelativeMultiAxis Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16,
         ByVal AxisCount As UInt16,
         ByVal AxisArray() As UInt16,
         ByVal PositionArray() As Double,
         ByVal Velocity As Double,
         ByVal Acceleration As Double,
         ByVal Deceleration As Double,
         ByVal Jerk As Double,
         ByVal ErrorStopMode As Byte
         ) As MC_STATUS

        '//5.1.1.67 MC_HaltMultiAxis [] : //v12.1.0.42 
        Declare Function MC_HaltMultiAxis Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16,
         ByVal AxisCount As UInt16,
         ByVal AxisArray() As UInt16,
         ByVal ErrorStopMode As Byte
         ) As MC_STATUS
		
        '//5.1.1.68
        Declare Function MC_SetHomeFlag Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16,
         ByVal AxisID As UInt16,
         ByVal EcatAddr As UInt16
         ) As MC_STATUS

        '//5.1.1.69
        Declare Function MC_GetHomeFlag Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16,
         ByVal AxisID As UInt16,
         ByRef pHomeFlag As UInt32
         ) As MC_STATUS

        '//===========================================================================
        '//---------------------------------------------------------------------------
        '// Device IO APIs
        '//---------------------------------------------------------------------------
        '//MC_GetIOState
        Declare Function MC_GetIOState Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal type As UInteger,
                ByRef pState As UInteger
                ) As MC_STATUS

        '// MC_IO_WRITE 
        Declare Function MC_IO_RAW_WRITE Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal Offset As UInteger,
                ByVal Size As UInteger,
                ByVal pDataArray() As Byte
                ) As MC_STATUS

        '// MC_IO_WRITE 
        Declare Function MC_IO_WRITE Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal Offset As UInteger,
                ByVal Size As UInteger,
                ByVal pDataArray As Byte()
                ) As MC_STATUS

        '// MC_IO_WRITE_BIT
        Declare Function MC_IO_WRITE_BIT Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal Offset As UInteger,
                ByVal bitOffset As Byte,
                ByVal data As Boolean
                ) As MC_STATUS

        '// MC_IO_WRITE_BIT
        Declare Function MC_IO_RAW_WRITE_BIT Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal Offset As UInteger,
                ByVal bitOffset As Byte,
                ByVal data As Boolean
                ) As MC_STATUS

        '// MC_IO_WRITE_BYTE
        Declare Function MC_IO_WRITE_BYTE Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal Offset As UInteger,
                ByVal data As Byte
                ) As MC_STATUS

        '// MC_IO_WRITE_WORD
        Declare Function MC_IO_WRITE_WORD Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal Offset As UInteger,
                ByVal data As UInt16
                ) As MC_STATUS

        '// MC_IO_WRITE_DWORD
        Declare Function MC_IO_WRITE_DWORD Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal Offset As UInteger,
                ByVal data As Integer
                ) As MC_STATUS


        '// MC_IO_READ
        Declare Function MC_IO_READ Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal BufferInOut As UInt16,
                ByVal Offset As UInteger,
                ByVal Size As UInteger,
                ByVal pDataArray() As Byte
                ) As MC_STATUS

        '// MC_IO_READ
        Declare Function MC_IO_RAW_READ Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal BufferInOut As UInt16,
                ByVal Offset As UInteger,
                ByVal Size As UInteger,
                ByVal pDataArray() As Byte
                ) As MC_STATUS

        '// MC_IO_READ_BIT
        Declare Function MC_IO_READ_BIT Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16,
         ByVal EcatAddr As UInt16,
         ByVal BufferInOut As UInt16,
         ByVal Offset As UInteger,
         ByVal bitOffset As Byte,
         ByRef pdata As Boolean
         ) As MC_STATUS

        '// MC_IO_READ_BIT
        Declare Function MC_IO_RAW_READ_BIT Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal BufferInOut As UInt16,
                ByVal Offset As UInteger,
                ByVal bitOffset As Byte,
                ByRef pdata As Boolean
                ) As MC_STATUS

        '// MC_IO_READ_BYTE
        Declare Function MC_IO_READ_BYTE Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal BufferInOut As UInt16,
                ByVal Offset As UInteger,
                ByRef pdata As Byte
                ) As MC_STATUS

        '// MC_IO_READ_WORD
        Declare Function MC_IO_READ_WORD Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal BufferInOut As UInt16,
                ByVal Offset As UInteger,
                ByRef pdata As UInt16
                ) As MC_STATUS

        '// MC_IO_READ_DWORD
        Declare Function MC_IO_READ_DWORD Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal BufferInOut As UInt16,
                ByVal Offset As UInteger,
                ByRef pdata As UInteger
                ) As MC_STATUS

        '// Read Error Info '//Ver_0c01000D_4
        Declare Function NMCReadErrorInfo Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByRef pSequenceNo As UInteger,
                ByRef pErrorCode As UInteger,
                ByRef pExtErrorInfo As Byte,
                ByRef pRemainErrorCount As UInteger
               ) As MC_STATUS

        '// MasterGetErrorState '//Ver_0c010010_3
        Declare Function MasterGetLastError Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByRef pSequenceNo As UInteger,
                ByRef pErrorCode As UInteger,
                ByRef pExtErrorInfo As Byte
                ) As MC_STATUS

        '// MasterResetErrorState '//Ver_0c010010_4
        Declare Function MasterClearError Lib "NMC_Motion.dll" (
         ByVal BoardID As UInt16
         ) As MC_STATUS

        '//MasterGetAxesCount '//Ver_0c010010_1
        Declare Function MasterGetAxesCount Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByRef TotalAxisCount As UInteger
                ) As MC_STATUS

        '//ObjEcatSlave.h 에서 옮겨 옴. '//Ver_0c01000E_4
        Declare Function SlaveGetAliasNo Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByRef pAliasID As UInt16
                ) As MC_STATUS

        '//Ver_0c010012_8
        Declare Function SlaveSetAliasNo Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal AliasID As UInt16
                ) As MC_STATUS

        '//21
        Declare Function SlaveGetCurState Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByRef pdata As Byte
                ) As MC_STATUS

        '// 42h - Ethercat Direct Access
        Declare Function MasterECatDirectAccess Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatCmd As Byte,
                ByVal Adp As UInt16,
                ByVal Ado As UInt16,
                ByVal ReqDataSize As UInt16,
                ByRef pbReqDataArray As Byte,
                ByRef pRespDataSize As UInt16,
                ByRef pbRespDataArray As Byte,
                ByRef pWC As UInt16
                ) As MC_STATUS
        '//2015_01_30 Open EEPROM Read

        Declare Function ReadET1100EEPROMWORD Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal Addr As UInteger,
                ByRef pwData As UInt16,
                ByVal TIMEOUT As UInteger
                ) As MC_STATUS
        '//2015_01_30 Open EEPROM Write

        Declare Function WriteET1100EEPROMWORD Lib "NMC_Motion.dll" (
                ByVal BoardID As UInt16,
                ByVal EcatAddr As UInt16,
                ByVal Addr As UInteger,
                ByVal wData As UInt16,
                ByVal TIMEOUT As UInteger
                ) As MC_STATUS

        'Shared Function MC_GetErrorMessage(ByVal MS As MC_STATUS, ByVal p2 As Integer, ByVal msg As Char()) As MC_STATUS
        'Throw New NotImplementedException
        'End Function

    End Class
End Namespace
