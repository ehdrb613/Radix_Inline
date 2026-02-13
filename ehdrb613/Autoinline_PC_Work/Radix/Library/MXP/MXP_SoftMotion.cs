using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;



namespace Radix
{
    class MXP
    {
        //***********************************************************************************************************************************************************/
        // System Function
        //***********************************************************************************************************************************************************/
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_InitKernel(ref UInt32 Status);
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_InitKernel_Developer(ref UInt32 Status);
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_Destroy();
       
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_SystemRun();
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_SystemReset();
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_SystemStop();

        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_GetKernelStatus(out Int32 Status);
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_GetOnlineMode(ref UInt32 Status);
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_IsSlaveOnline(UInt32 SlaveNo, out UInt32 Status);
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_GetSlaveCount(UInt32 IsAxis, out UInt32 Count);
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_QueryNodeType(UInt32 SlaveNo, out UInt32 NodeType);
        [DllImport("MXP_SoftMotion.dll")] extern public static int MXP_GetSystemInformation(out MXP_SYSINFO_OUT SysInfo);
        //***********************************************************************************************************************************************************/
        // Motion Function
        //***********************************************************************************************************************************************************/
        public class MXP_ret
        {
            public const Int32 RET_NO_ERROR = 0;
            public const Int32 RET_ERROR_FUNCTION = -1;
            public const Int32 RET_ERROR_FULL = -2;
            public const Int32 RET_ERROR_WRONG_INDEX = -3;
            public const Int32 RET_ERROR_WRONG_AXISNO = -4;
            public const Int32 RET_ERROR_MOTIONBUSY = -5;
            public const Int32 RET_ERROR_WRONG_SLAVENO = -6;
            public const Int32 RET_ERROR_WRONG_CAMTABLENO = -7;
            public const Int32 RET_ERROR_WRONG_ECMASTERNO = -8;
            public const Int32 RET_ERROR_WRONG_ECSLAVENO = -9;
            public const Int32 RET_ERROR_NOT_OPMODE = -10;
            public const Int32 RET_ERROR_NOTRUNNING = -11;
            public const Int32 RET_ERROR_WRONG_PARAMETER_NO = -12;
            public const Int32 RET_ERROR_WRONG_MXP_TYPE = -13;
            public const Int32 RET_ERROR_ALREADYOPEN = -14;
        }

        public enum MXP_KernelState
        {
            Idle = 0,
            Init,
            Initing,
            Inited,
            Run,
            Running,
            Runed,
            Reset,
            Close,
            Destory
        }
        public enum MXP_MotionBlockIndex
        {
            mcPower = 0,
            mcReset,
            mcStop,
            mcHalt,
            mcHome,
            mcMoveAbsolute,
            mcMoveRelative,
            mcMoveVelocity,
            mcMoveLinearAbsolute,
            mcMoveLinearRelative,
            mcMoveCircularAbsolute,
            mcMoveCircularRelative,
            mcGroupStop,
            mcWriteParameter,
            mcWriteBoolParameter,
            mcET_ReadParameter,
            mcET_WriteParameter,
            mcWriteOutputs,
            mcWriteDigitalOutputs,
            mcGearIn,
            mcGearOut,
            mcGearInPos,
            mcCamIn,
            mcCamOut,
            mcCamTableSelect,
            mcSetTouchProbe,
        }

        public struct HEARTBEAT
        {
            public Int32 AX;
            public Int32 Main;
            public Int32 Motion;
            public Int32 Scheduler;
            public Int32 Modbus;
            public Int32 EtherCAT;
        }
        public struct CREATION
        {
            public Byte Main;
            public Byte Motion;
            public Byte Scheduler;
            public Byte Modbus;
            public Byte EtherCAT;
        }

        public struct MODE
        {
            public Byte Start;
            public Byte Init;
            public Byte Ready;
            public Byte Run;
            public Byte Stop;
            public Byte Reserved1;
            public Byte Download;
            public Byte Clear;

            public Byte Reserved;
        }

        public struct SETTINGTIME
        {
            public Single Sheduler;
            public Single Motion;
            public Single EtherCATIO;
            public Single EtherCATMaster;
        }

        public struct CURRENTTIME
        {
            public Single Sheduler;
            public Single Motion;
            public Single EtherCATIO;
            public Single EtherCATMaster;
        }

        public struct MINTIME
        {
            public Single Sheduler;
            public Single Motion;
            public Single EtherCATIO;
            public Single EtherCATMaster;
        }

        public struct MAXTIME
        {
            public Single Sheduler;
            public Single Motion;
            public Single EtherCATIO;
            public Single EtherCATMaster;
        }

        public struct CURRENTOPTIME
        {
            public Single Sheduler;
            public Single Motion;
            public Single EtherCATIO;
            public Single EtherCATMaster;
        }

        public struct MAXOPTIME
        {
            public Single Sheduler;
            public Single Motion;
            public Single EtherCATIO;
            public Single EtherCATMaster;
        }

        public struct SCANTIME
        {
            public SETTINGTIME SetTime;
            public CURRENTTIME CurTime;
            public MINTIME MinTime;
            public MAXTIME MaxTime;
            public CURRENTOPTIME CurOp;
            public MAXOPTIME MaxOp;
        }

        public struct ETHERCAT_STATE
        {
            public Int32 DCfaultCnt;
            public Single DCSlotPos;
            public Single DCPrevInterval;
            public Int32 DcPlus;
            public Int32 DcMinus;
            public Int32 Master;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public Int32[] Slaves;     
        }

        public struct ETHERCATLINKSTATUS
        {
            public Int32 Master;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public Int32[] Slaves;     
        }

        public struct ALARM
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public Int32[] History;     
        }

        public struct PID
        {
            public UInt32 UserApp;
            public UInt32 Mpm;
            public UInt32 Main;
            public UInt32 Motion;
            public UInt32 Scheduler;
            public UInt32 Modbus;
            public UInt32 EtherCAT;
        } 


        public struct MXP_SYSINFO_OUT
        {

            public HEARTBEAT HB;
            public CREATION Crt;
            public MODE Mode;
            public SCANTIME ScanTime;
            public ETHERCAT_STATE Ether_State;
            public ETHERCATLINKSTATUS Ether_Link;
            public ALARM AlHist;
            public PID Pid;
            public Byte VerboseMode;
        }

        public enum MXP_BUFFERMODE_ENUM
        {
            MXP_ABORTING = 0,
            MXP_BUFFERED = 1,
            MXP_BLENDING_LOW = 2,
            MXP_BLENDING_PREVIOUS = 3,
            MXP_BLENDING_NEXT = 4,
            MXP_BLENDING_HIGH = 5,
            MXP_SINGLE_BLOCK = 6
        }
        public enum MXP_DIRECTION_ENUM
        {
            MXP_NONE_DIRECTION = 0,
            MXP_POSITIVE_DIRECTION = 1,
            MXP_SHORTEST_WAY = 2,
            MXP_NEGATIVE_DIRECTION = 3,
            MXP_CURRENT_DIRECTION = 4

        }
        public enum MXP_EXECUTIONMODE_ENUM
        {
            MXP_IMMEDIATELY = 1,
            MXP_QUEUED = 2
        }

        public enum MXP_SOURCE_ENUM
        {
            MXP_COMMANDEDVALUE = 1,
            MXP_SETVALUE = 2,
            MXP_ACTUALVALUE = 3
        }

        public enum MXP_STARTMODE_ENUM
        {
            MXP_ABSOLUTE = 0,
            MXP_RELATIVE = 1
            //MXP_RAMPIN = 3
        }

        public enum MXP_COORDSYSTEM_ENUM
        {
            MXP_ACS = 1,
            MXP_MCS = 2,
            MXP_PCS = 3
        }

        public enum MXP_TRANSITIONMODE_ENUM
        {
            MXP_TM_NONE = 0,
            MXP_TM_STARTVELOCITY = 1,
            MXP_TM_CONSTANTVELOCITY = 2,
            MXP_TM_CORNERDISTANCE = 3,
            MXP_TM_MAXCORNERDEVIATION = 4
        }

        public enum MXP_CIRCLEMODE_ENUM
        {
            MXP_BORDER = 1,
            MXP_CENTER = 2,
            MXP_RADIUS = 3
        }

        public enum MXP_PATHCHOICE_ENUM
        {
            MXP_CLOCKWISE = 0,
            MXP_COUNTERCLOCKWISE = 1
        }

        public enum MXP_SYNCMODE_ENUM
        {
            MXP_SHORTEST = 1,
            MXP_CATCHUP = 2,
            MXP_SLOWDOWN = 3
        }

        public enum MXP_SWITCHMODE_ENUM
        {
            MXP_ON = 0,
            MXP_OFF = 1,
            MXP_EDGE_ON = 2,
            MXP_EDGE_OFF = 3,
            MXP_EDGE_SWITCH_POSITIVE = 4,
            MXP_EDGE_SWITCH_NEGATIVE = 5
        }

        public enum MXP_HOMEDIRECTION_ENUM
        {
            MXP_POSITIVE = 1,
            MXP_NEGATIVE = 2,
            MXP_SWITCH_POSITIVE = 3,
            MXP_SWITCH_NEGATIVE = 4
        }

        public enum MXP_TOUCHPROBE_EDGE_ENUM
        {
            MXP_EDGE_POSITIVE = 0,
            MXP_EDGE_NEGATIVE = 1
        }

        public enum MXP_TOUCHPROBE_CHNL_ENUM
        {
            MXP_TOUCH_CH1 = 0,
            MXP_TOUCH_CH2 = 1
        }
        

        public enum MXP_CONTROLMODE_ENUM
        {
            MXP_PP = 0,
            MXP_CSP = 1,
            MXP_CSV = 2,
            MXP_CST = 3
        }

        public enum MXP_PDODIRECTION_ENUM
        {
            MXP_PDO_Tx = 0,	//Slave -> MXP : ex)ActualPosition
            MXP_PDO_Rx = 1		//MXP -> Slave : ex)TargetPosition
        }

        public class MXP_SysStatus
        {
            public const Int16 Unlicensed = -2;
            public const Int16 Idle = 1;
            public const Int16 Closing = 2;
            public const Int16 Closed = 3;
            public const Int16 Creating = 4;
            public const Int16 Created = 5;
            public const Int16 Initializing = 6;
            public const Int16 Initialized = 7;
            public const Int16 Ready = 8;
            public const Int16 Run = 9;
        }
        public enum MXP_ONLINESTATE_ENUM
        {
            NET_NOTCONNECTED        = 0x00,
            NET_STATE_INIT          = 0x01,
            NET_STATE_PREOP         = 0x02,
            NET_STATE_BOOT          = 0x03,
            NET_STATE_SAFEOP        = 0x04,
            NET_STATE_OP            = 0x08,

            ERR_NET_NOTCONNECTED    = 0x10,
            ERR_NET_STATE_INIT      = 0x11,
            ERR_NET_STATE_PREOP     = 0x12,
            ERR_NET_STATE_BOOT      = 0x13,
            ERR_NET_STATE_SAFEOP    = 0x14,
            ERR_NET_STATE_OP        = 0x18
        }
        public enum MXP_AXIS_STATE
        {
            PLCOPEN_AXIS_DISABLED		    = 0,
	        PLCOPEN_AXIS_STANDSTILL 	    = 1,
	        PLCOPEN_AXIS_HOMING 		    = 2,
	        PLCOPEN_AXIS_ERRORSTOP  	    = 3,
	        PLCOPEN_AXIS_STOPPING	    	= 4,
	        PLCOPEN_AXIS_DISCRETE_MOTION	= 5,
	        PLCOPEN_AXIS_SYNCHRO_MOTION 	= 6,
	        PLCOPEN_AXIS_CONTINUOUS_MOTION  = 7
        }
        public enum MXP_SlaveType
        {
            NULL_ST     = 0,
            DUMMY_ST    = 1,
            SIMPLE_ST   = 2,
            PORT_ST     = 3,
            ROUTER_ST   = 4,
            AX2000_ST   = 5,
            EL67XX_ST   = 6,
            MAILBOX_ST  = 7,
            CiA402_ST   = 8
        }

        public enum MXP_AXISSTATUS
        {
            ERROR_STOP = 0,
            DISABLED = 1,
            HOMING = 2,
            STOPPING = 3,
            STANDSTILL = 4,
            DISCRETE_MOTION = 5,
            CONTINUOUS_MOTION = 6,
            SYNCHRONIZED_MOTION = 7
        }

        public struct MXP_AXIS_REF
        {
            public UInt32 AxisNo;
        }

        public struct MXP_AXESGROUP_REF
        {
            public UInt32 AxesGroupNo;
        }

        public struct MXP_INPUT_REF
        {
            public UInt32 SourceNo;
        }

        public struct MXP_OUTPUT_REF
        {
            public UInt32 SourceNo;
        }

        public struct MXP_CAM_REF
        {
            public UInt32 CamTable;
        }


	

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_PowerCmd(UInt32 MotionBlockIndex, ref MXP_POWER_IN In);
        public struct MXP_POWER_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
            public Byte Positive;
            public Byte Negative;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetPowerOutParam(UInt32 MotionBlockIndex, out MXP_POWER_OUT Out);
        public struct MXP_POWER_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Status;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ResetCmd(UInt32 MotionBlockIndex, ref MXP_RESET_IN In);
        public struct MXP_RESET_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetResetOutParam(UInt32 MotionBlockIndex, out MXP_RESET_OUT Out);
        public struct MXP_RESET_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_HaltCmd(UInt32 MotionBlockIndex, ref MXP_HALT_IN In);
        public struct MXP_HALT_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Single Deceleration;
            public Single Jerk;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetHaltOutParam(UInt32 MotionBlockIndex, out MXP_HALT_OUT Out);
        public struct MXP_HALT_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_StopCmd(UInt32 MotionBlockIndex, ref MXP_STOP_IN In);
        public struct MXP_STOP_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Single Deceleration;
            public Single Jerk;
        }
                
        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetStopOutParam(UInt32 MotionBlockIndex, out MXP_STOP_OUT Out);
        public struct MXP_STOP_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_HomeCmd(UInt32 MotionBlockIndex, ref MXP_HOME_IN In);
        public struct MXP_HOME_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Single Position;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetHomeOutParam(UInt32 MotionBlockIndex, out MXP_HOME_OUT Out);
        public struct MXP_HOME_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_MoveAbsoluteCmd(UInt32 MotionBlockIndex, ref MXP_MOVEABSOLUTE_IN In);
        public struct MXP_MOVEABSOLUTE_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Byte ContinuousUpdate;
            public Single Position;
            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_DIRECTION_ENUM Direction;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetMoveAbsoluteOutParam(UInt32 MotionBlockIndex, out MXP_MOVEABSOLUTE_OUT Out);
        public struct MXP_MOVEABSOLUTE_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_MoveRelativeCmd(UInt32 MotionBlockIndex, ref MXP_MOVERELATIVE_IN In);
        public struct MXP_MOVERELATIVE_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Byte ContinuousUpdate;
            public Single Distance;
            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetMoveRelativeOutParam(UInt32 nMotionBlockIndex, out MXP_MOVERELATIVE_OUT Out);
        public struct MXP_MOVERELATIVE_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_MoveVelocityCmd(UInt32 MotionBlockIndex, ref MXP_MOVEVELOCITY_IN In);
        public struct MXP_MOVEVELOCITY_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Byte ContinuousUpdate;
            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_DIRECTION_ENUM Direction;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetMoveVelocityOutParam(UInt32 nMotionBlockIndex, out MXP_MOVEVELOCITY_OUT Out);
        public struct MXP_MOVEVELOCITY_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadDigitalInput(ref MXP_READDIGITALINPUT_IN In, out MXP_READDIGITALINPUT_OUT Out);
        public struct MXP_READDIGITALINPUT_IN
        {
            public MXP_INPUT_REF Input;
            public Byte Enable;
            public Int32 InputNumber;
        }
        public struct MXP_READDIGITALINPUT_OUT
        {
            public MXP_INPUT_REF Input;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Byte Value;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadDigitalOutput(ref MXP_READDIGITALOUTPUT_IN In, out MXP_READDIGITALOUTPUT_OUT Out);
        public struct MXP_READDIGITALOUTPUT_IN
        {
            public MXP_OUTPUT_REF Output;
            public Byte Enable;
            public Int32 OutputNumber;
        }
        public struct MXP_READDIGITALOUTPUT_OUT
        {
            public MXP_OUTPUT_REF Output;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Byte Value;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_WriteDigitalOutputCmd(UInt32 MotionBlockIndex, ref MXP_WRITEDIGITALOUTPUT_IN In);
        
        public struct MXP_WRITEDIGITALOUTPUT_IN
        {
            public MXP_OUTPUT_REF Output;
            public Byte Execute;
            public Int32 OutputNumber;
            public Byte Value;
            public MXP_EXECUTIONMODE_ENUM ExecutionMode;
        }
        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetWriteDigitalOutputOutParam(UInt32 MotionBlockIndex, out MXP_WRITEDIGITALOUTPUT_OUT Out);
        public struct MXP_WRITEDIGITALOUTPUT_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadActualPosition(ref MXP_READACTUALPOSITION_IN In, out MXP_READACTUALPOSITION_OUT Out);
        public struct MXP_READACTUALPOSITION_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READACTUALPOSITION_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single Position;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadActualVelocity(ref MXP_READACTUALVELOCITY_IN In, out MXP_READACTUALVELOCITY_OUT Out);
        public struct MXP_READACTUALVELOCITY_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READACTUALVELOCITY_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single Velocity;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadActualTorque(ref MXP_READACTUALTORQUE_IN In, out MXP_READACTUALTORQUE_OUT Out);
        public struct MXP_READACTUALTORQUE_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READACTUALTORQUE_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single Torque;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadActualForce(ref MXP_READACTUALFORCE_IN In, out MXP_READACTUALFORCE_OUT Out);
        public struct MXP_READACTUALFORCE_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READACTUALFORCE_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single Force;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadStatus(ref MXP_READSTATUS_IN In, out MXP_READSTATUS_OUT Out);
        public struct MXP_READSTATUS_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READSTATUS_OUT
        {
            public MXP_AXIS_REF Axis;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Byte ErrorStop;
            public Byte Disabled;
            public Byte Stopping;
            public Byte Homing;
            public Byte Standstill;
            public Byte DiscreteMotion;
            public Byte ContinuousMotion;
            public Byte SynchronizedMotion;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadAxisInfo(ref MXP_READAXISINFO_IN In, out MXP_READAXISINFO_OUT Out);
        public struct MXP_READAXISINFO_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READAXISINFO_OUT
        {
            public MXP_AXIS_REF Axis;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Byte HomeAbsSwitch;
            public Byte LimitSwitchPos;
            public Byte LimitSwitchNeg;
            public Byte Simulation;
            public Byte CommunicationReady;
            public Byte ReadyForPowerOn;
            public Byte PowerOn;
            public Byte IsHomed;
            public Byte AxisWarning;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadAxisError(ref MXP_READAXISERROR_IN In, out MXP_READAXISERROR_OUT Out);
        public struct MXP_READAXISERROR_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READAXISERROR_OUT
        {
            public MXP_AXIS_REF Axis;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public UInt16 AxisErrorID;
            public UInt16 AuxErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadMotionState(ref MXP_READMOTIONSTATE_IN In, out MXP_READMOTIONSTATE_OUT Out);
        public struct MXP_READMOTIONSTATE_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
            public MXP_SOURCE_ENUM Source;
        }
        public struct MXP_READMOTIONSTATE_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Byte ConstantVelocity;
            public Byte Accelerating;
            public Byte Decelerating;
            public Byte DirectionPositive;
            public Byte DirectionNegative;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadParameter(ref MXP_READPARAMETER_IN In, out MXP_READPARAMETER_OUT Out);
        public struct MXP_READPARAMETER_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
            public Int32 ParameterNumber;
        }
        public struct MXP_READPARAMETER_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single Value;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadBoolParameter(ref MXP_READBOOLPARAMETER_IN In, out MXP_READBOOLPARAMETER_OUT Out);
        public struct MXP_READBOOLPARAMETER_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
            public Int32 ParameterNumber;
        }
        public struct MXP_READBOOLPARAMETER_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Byte Value;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_WriteParameterCmd(UInt32 MotionBlockIndex, ref MXP_WRITEPARAMETER_IN In);
        public struct MXP_WRITEPARAMETER_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Int32 ParameterNumber;
            public Single Value;
            public MXP_EXECUTIONMODE_ENUM ExecutionMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetWriteParameterOutParam(UInt32 MotionBlockIndex, out MXP_WRITEPARAMETER_OUT Out);
        public struct MXP_WRITEPARAMETER_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_WriteBoolParameterCmd(UInt32 MotionBlockIndex, ref MXP_WRITEBOOLPARAMETER_IN In);
        public struct MXP_WRITEBOOLPARAMETER_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Int32 ParameterNumber;
            public Byte Value;
            public MXP_EXECUTIONMODE_ENUM ExecutionMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetWriteBoolParameterOutParam(UInt32 MotionBlockIndex, out MXP_WRITEBOOLPARAMETER_OUT Out);
        public struct MXP_WRITEBOOLPARAMETER_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_SetPositionCmd(UInt32 MotionBlockIndex, ref MXP_SETPOSITION_IN In);
        public struct MXP_SETPOSITION_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Single Position;
            public Byte Relative;
            public MXP_EXECUTIONMODE_ENUM ExecutionMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetSetPositionOutParam(UInt32 MotionBlockIndex, out MXP_SETPOSITION_OUT Out);
        public struct MXP_SETPOSITION_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_SetOverrideCmd(UInt32 MotionBlockIndex, ref MXP_SETOVERRIDE_IN In);
        public struct MXP_SETOVERRIDE_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
            public Single VelFactor;
            public Single AccFactor;
            public Single JerkFactor;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetSetOverrideOutParam(UInt32 MotionBlockIndex, out MXP_SETOVERRIDE_OUT Out);
        public struct MXP_SETOVERRIDE_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_TorqueControlCmd(UInt32 MotionBlockIndex, ref MXP_TORQUECONTROL_IN In);
        public struct MXP_TORQUECONTROL_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public Byte ContinuousUpdate;
            public Single Torque;
            public Single TorqueRamp;
            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_DIRECTION_ENUM Direction;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetTorqueControlOutParam(UInt32 MotionBlockIndex, out MXP_TORQUECONTROL_OUT Out);
        public struct MXP_TORQUECONTROL_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte InTorque;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_TorqueFeedbacjControlCmd(UInt32 MotionBlockIndex, ref MXP_TORQUECONTROL_IN In, ref MXP_FEEDBACK_IN In2);
        public struct MXP_FEEDBACK_IN
        {
            public MXP_INPUT_REF Input;
            public UInt16 Offset;
            public UInt16 Size;
            public UInt16 DataRange;
            public Single PGain;
            public Single IGain;
            public Single DGain;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetTorqueFeedbackControlOutParam(UInt32 MotionBlockIndex, out MXP_TORQUECONTROL_OUT Out);


        [DllImport("MXP_SoftMotion.dll")] 
        extern public static int MXP_MoveLinearAbsoluteCmd(UInt32 MotionBlockIndex, ref MXP_MOVELINEARABSOLUTE_IN In);
        public struct MXP_MOVELINEARABSOLUTE_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Execute;

            public Single PositionX;
            public Single PositionY;
            public Single PositionZ;
            public Single PositionU;
            public Single PositionV;
            public Single PositionW;
            public Single PositionA;
            public Single PositionB;
            public Single PositionC;

            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;

            public MXP_COORDSYSTEM_ENUM CoordSystem;
            public MXP_BUFFERMODE_ENUM BufferMode;
            public MXP_TRANSITIONMODE_ENUM TransitionMode;
            public Single TransitionParameter;     
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetMoveLinearAbsoluteOutParam(UInt32 MotionBlockIndex, out MXP_MOVELINEARABSOLUTE_OUT Out);
        public struct MXP_MOVELINEARABSOLUTE_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_MoveLinearRelativeCmd(UInt32 MotionBlockIndex, ref MXP_MOVELINEARRELATIVE_IN In);
        public struct MXP_MOVELINEARRELATIVE_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Execute;
            public Single DistanceX;
            public Single DistanceY;
            public Single DistanceZ;
            public Single DistanceU;
            public Single DistanceV;
            public Single DistanceW;
            public Single DistanceA;
            public Single DistanceB;
            public Single DistanceC;

            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_COORDSYSTEM_ENUM CoordSystem;
            public MXP_BUFFERMODE_ENUM BufferMode;
            public MXP_TRANSITIONMODE_ENUM TransitionMode;
            public Single TransitionParameter;            
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetMoveLinearRelativeOutParam(UInt32 MotionBlockIndex, out MXP_MOVELINEARRELATIVE_OUT Out);
        public struct MXP_MOVELINEARRELATIVE_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_MoveCircularAbsoluteCmd(UInt32 MotionBlockIndex, ref MXP_MOVECIRCULARABSOLUTE_IN In);
        public struct MXP_MOVECIRCULARABSOLUTE_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Execute;
            public MXP_CIRCLEMODE_ENUM CircMode;
            public Single AuxPoint1;
            public Single AuxPoint2;
            public Single EndPoint1;
            public Single EndPoint2;
            public MXP_PATHCHOICE_ENUM PathChoice;
            public Int32 Plane1;
            public Int32 Plane2;
            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_COORDSYSTEM_ENUM CoordSystem;
            public MXP_BUFFERMODE_ENUM BufferMode;
            public MXP_TRANSITIONMODE_ENUM TransitionMode;
            public Single TransitionParameter;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetMoveCircularAbsoluteOutParam(UInt32 MotionBlockIndex, out MXP_MOVECIRCULARABSOLUTE_OUT Out);
        public struct MXP_MOVECIRCULARABSOLUTE_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_MoveCircularRelativeCmd(UInt32 MotionBlockIndex, ref MXP_MOVECIRCULARRELATIVE_IN In);
        public struct MXP_MOVECIRCULARRELATIVE_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Execute;
            public MXP_CIRCLEMODE_ENUM CircMode;
            public Single AuxPoint1;
            public Single AuxPoint2;
            public Single EndPoint1;
            public Single EndPoint2;
            public MXP_PATHCHOICE_ENUM PathChoice;
            public Int32 Plane1;
            public Int32 Plane2;
            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_COORDSYSTEM_ENUM CoordSystem;
            public MXP_BUFFERMODE_ENUM BufferMode;
            public MXP_TRANSITIONMODE_ENUM TransitionMode;
            public Single TransitionParameter;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetMoveCircularRelativeOutParam(UInt32 MotionBlockIndex, out MXP_MOVECIRCULARRELATIVE_OUT Out);
        public struct MXP_MOVECIRCULARRELATIVE_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GroupStopCmd(UInt32 MotionBlockIndex, ref MXP_GROUPSTOP_IN In);
        public struct MXP_GROUPSTOP_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Execute;
            public Single Deceleration;
            public Single Jerk;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetGroupStopOutParam(UInt32 MotionBlockIndex, out MXP_GROUPSTOP_OUT Out);
        public struct MXP_GROUPSTOP_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadCommandPosition(ref MXP_READCOMMANDPOSITION_IN In, out MXP_READCOMMANDPOSITION_OUT Out);
        public struct MXP_READCOMMANDPOSITION_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READCOMMANDPOSITION_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single CommandPosition;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadCommandVelocity(ref MXP_READCOMMANDVELOCITY_IN In, out MXP_READCOMMANDVELOCITY_OUT Out);
        public struct MXP_READCOMMANDVELOCITY_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READCOMMANDVELOCITY_OUT
        {
            public MXP_AXIS_REF Axis;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single CommandVelocity;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GroupReadActualVelocity(ref MXP_GROUPREADACTUALVELOCITY_IN In, out MXP_GROUPREADACTUALVELOCITY_OUT Out);
        public struct MXP_GROUPREADACTUALVELOCITY_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Enable;
            public MXP_COORDSYSTEM_ENUM CoordSystem;
        }
        public struct MXP_GROUPREADACTUALVELOCITY_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single Velocity;
            public Single PathVelocity;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GroupReadCommandVelocity(ref MXP_GROUPREADCOMMANDVELOCITY_IN In, out MXP_GROUPREADCOMMANDVELOCITY_OUT Out);
        public struct MXP_GROUPREADCOMMANDVELOCITY_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Enable;
        }
        public struct MXP_GROUPREADCOMMANDVELOCITY_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single CommandVelocity;
            public Single PathCommandVelocity;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GroupSetPositionCmd(UInt32 MotionBlockIndex, ref MXP_GROUPSETPOSITION_IN In);
        public struct MXP_GROUPSETPOSITION_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Execute;
            public Single PositionX;
            public Single PositionY;
            public Single PositionZ;
            public Single PositionU;
            public Single PositionV;
            public Single PositionW;
            public Single PositionA;
            public Single PositionB;
            public Single PositionC;
            public Byte Relative;
            public MXP_COORDSYSTEM_ENUM CoordSystem;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetGroupSetPositionOutParam(UInt32 MotionBlockIndex, out MXP_GROUPSETPOSITION_OUT Out);
        public struct MXP_GROUPSETPOSITION_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GroupReadActualPosition(ref MXP_GROUPREADACTUALPOSITION_IN In, out MXP_GROUPREADACTUALPOSITION_OUT Out);
        public struct MXP_GROUPREADACTUALPOSITION_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Enable;
            public MXP_COORDSYSTEM_ENUM CoordSystem;
        }
        public struct MXP_GROUPREADACTUALPOSITION_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single PositionX;
            public Single PositionY;
            public Single PositionZ;
            public Single PositionU;
            public Single PositionV;
            public Single PositionW;
            public Single PositionA;
            public Single PositionB;
            public Single PositionC;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GroupHomeCmd(UInt32 MotionBlockIndex, ref MXP_GROUPHOME_IN In);
        public struct MXP_GROUPHOME_IN
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Execute;
            public Single PositionX;
            public Single PositionY;
            public Single PositionZ;
            public Single PositionU;
            public Single PositionV;
            public Single PositionW;
            public Single PositionA;
            public Single PositionB;
            public Single PositionC;
            public MXP_COORDSYSTEM_ENUM CoordSystem;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetGroupHomeOutParam(UInt32 nMotionBlockIndex, out MXP_GROUPHOME_OUT Out);
        public struct MXP_GROUPHOME_OUT
        {
            public MXP_AXESGROUP_REF AxesGroup;
            public Byte Done;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

         //***********************************************************************************************************************************************************/ 
        // Motion Function - E-CAM, E-Gear
        //***********************************************************************************************************************************************************/
        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_CamTableSelectCmd(UInt32 MotionBlockIndex, ref MXP_CAMTABLESELECT_IN In);
        public struct MXP_CAMTABLESELECT_IN
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public MXP_CAM_REF CamTable;
            public Byte Execute;
            public Byte Periodic;
            public Byte MasterAbsolute;
            public Byte SlaveAbsolute;
            public MXP_EXECUTIONMODE_ENUM ExecutionMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetCamTableSelectOutParam(UInt32 MotionBlockIndex, out MXP_CAMTABLESELECT_OUT Out);
        public struct MXP_CAMTABLESELECT_OUT
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public MXP_CAM_REF CamTable;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public UInt16 CamTableID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_CamInCmd(UInt32 MotionBlockIndex, ref MXP_CAMIN_IN In);
        public struct MXP_CAMIN_IN
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte Execute;
            public Byte ContinuousUpdate;
            public Single MasterOffset;
            public Single SlaveOffset;
            public Single MasterScaling;
            public Single SlaveScaling;
            public Single MasterStartDistance;
            public Single MasterSyncPosition;
            public MXP_STARTMODE_ENUM StartMode;
            public MXP_SOURCE_ENUM MasterValueSource;
            public UInt16 CamTableID;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetCamInOutParam(UInt32 MotionBlockIndex, out MXP_CAMIN_OUT Out);
        public struct MXP_CAMIN_OUT
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte InSync;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
            public Byte EndOfProfile;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_CamOutCmd(UInt32 MotionBlockIndex, ref MXP_CAMOUT_IN In);
        public struct MXP_CAMOUT_IN
        {
            public MXP_AXIS_REF Slave;
            public Byte Execute;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetCamOutOutParam(UInt32 MotionBlockIndex, out MXP_CAMOUT_OUT Out);
        public struct MXP_CAMOUT_OUT
        {
            public MXP_AXIS_REF Slave;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GearInCmd(UInt32 MotionBlockIndex, ref MXP_GEARIN_IN In);
        public struct MXP_GEARIN_IN
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte Execute;
            public Byte ContinuousUpdate;
            public Int32 RatioNumerator;
            public UInt32 RatioDenominator;
            public MXP_SOURCE_ENUM MasterValueSource;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetGearInOutParam(UInt32 MotionBlockIndex, out MXP_GEARIN_OUT Out);
        public struct MXP_GEARIN_OUT
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte InGear;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GearOutCmd(UInt32 MotionBlockIndex, ref MXP_GEAROUT_IN In);
        public struct MXP_GEAROUT_IN
        {
            public MXP_AXIS_REF Slave;
            public Byte Execute;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetGearOutOutParam(UInt32 MotionBlockIndex, out MXP_GEAROUT_OUT Out);
        public struct MXP_GEAROUT_OUT
        {
            public MXP_AXIS_REF Slave;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GearInPosCmd(UInt32 nMotionBlockIndex, ref MXP_GEARINPOS_IN In);
        public struct MXP_GEARINPOS_IN
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte Execute;
            public UInt32 RatioDenominator;
            public Int32 RatioNumerator;
            public MXP_SOURCE_ENUM MasterValueSource;
            public Single MasterSyncPosition;
            public Single SlaveSyncPosition;
            public MXP_SYNCMODE_ENUM SyncMode;
            public Single MasterStartDistance;
            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetGearInPosOutParam(UInt32 MotionBlockIndex, out MXP_GEARINPOS_OUT Out);
        public struct MXP_GEARINPOS_OUT
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte StartSync;
            public Byte InSync;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GearInPosExCmd(UInt32 nMotionBlockIndex, ref MXP_GEARINPOSEX_IN In);
        public struct MXP_GEARINPOSEX_IN
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte Execute;
            public UInt32 RatioDenominator;
            public Int32 RatioNumerator;
            public MXP_SOURCE_ENUM MasterValueSource;
            public Single MasterSyncPosition;
            public Single SlaveSyncPosition;
            public Single SlaveSyncPosDistance;
            public MXP_SYNCMODE_ENUM SyncMode;
            public Single MasterStartDistance;
            public Single Velocity;
            public Single Acceleration;
            public Single Deceleration;
            public Single Jerk;
            public MXP_BUFFERMODE_ENUM BufferMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetGearInPosExOutParam(UInt32 MotionBlockIndex, out MXP_GEARINPOSEX_OUT Out);
        public struct MXP_GEARINPOSEX_OUT
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte StartSync;
            public Byte InSync;
            public Byte Busy;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ShiftSyncPosCmd(UInt32 MotionBlockIndex, ref MXP_SHIFTSYNCPOS_IN In);
        public struct MXP_SHIFTSYNCPOS_IN
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte Execute;
            public Single ShiftDistance;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetShiftSyncPosOutParam(UInt32 MotionBlockIndex, out MXP_SHIFTSYNCPOS_OUT Out);
        public struct MXP_SHIFTSYNCPOS_OUT
        {
            public MXP_AXIS_REF Master;
            public MXP_AXIS_REF Slave;
            public Byte Active;
            public Byte CommandAborted;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            
        }

        //***********************************************************************************************************************************************************/ 
        // Motion Function Extension
        //***********************************************************************************************************************************************************/
        [DllImport("MXP_SoftMotion.dll")] 
        extern public static int MXP_ReadInputs(ref MXP_READINPUTS_IN In, out MXP_READINPUTS_OUT Out, out Byte Data);
        
        public struct MXP_READINPUTS_IN
        {
            public MXP_INPUT_REF Input;
            public Byte Enable;
        }
        public struct MXP_READINPUTS_OUT
        {
            public MXP_INPUT_REF Input;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public UInt16 Size;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadOutputs(ref MXP_READOUTPUTS_IN In, out MXP_READOUTPUTS_OUT Out, out Byte Data);  
        public struct MXP_READOUTPUTS_IN
        {
            public MXP_OUTPUT_REF Output;
            public Byte Enable;
        }
        public struct MXP_READOUTPUTS_OUT
        {
            public MXP_OUTPUT_REF Output;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public UInt16 Size;
        }
        
        
        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_WriteOutputsCmd(UInt32 MotionBlockIndex, ref MXP_WRITEOUTPUTS_IN In, ref Byte Data);
        public struct MXP_WRITEOUTPUTS_IN
        {
            public MXP_OUTPUT_REF Output;
            public Byte Execute;
            public UInt16 Size;
        }
        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetWriteOutputsOutParam(UInt32 MotionBlockIndex, out MXP_WRITEOUTPUTS_OUT Out);
        public struct MXP_WRITEOUTPUTS_OUT
        {
            public MXP_OUTPUT_REF Output;

            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }



        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_SetTouchProbeFunctionCmd(UInt32 MotionBlockIndex, ref MXP_SETTOUCHPROBEFUNC_IN In);
        public struct MXP_SETTOUCHPROBEFUNC_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
            public UInt16 FuncData;
        }
        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetSetTouchProbeFunctionOutParam(UInt32 MotionBlockIndex, out MXP_SETTOUCHPROBEFUNC_OUT Out);
        public struct MXP_SETTOUCHPROBEFUNC_OUT
        {
            public MXP_AXIS_REF Axis;

            public Byte Enabled;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadActualTouchProbeStatus(ref MXP_READACTUALTOUCHPROBESTATUS_IN In, out MXP_READACTUALTOUCHPROBESTATUS_OUT Out);
        public struct MXP_READACTUALTOUCHPROBESTATUS_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READACTUALTOUCHPROBESTATUS_OUT
        {
            public MXP_AXIS_REF Axis;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public UInt16 Status;
        }


        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadActualTouchProbePosition(ref MXP_READACTUALTOUCHPROBEPOSITION_IN In, out MXP_READACTUALTOUCHPROBEPOSITION_OUT Out);
        public struct MXP_READACTUALTOUCHPROBEPOSITION_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
            public MXP_TOUCHPROBE_CHNL_ENUM Channel;
            public MXP_TOUCHPROBE_EDGE_ENUM Edge;
        }
        public struct MXP_READACTUALTOUCHPROBEPOSITION_OUT
        {
            public MXP_AXIS_REF Axis;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single Position;
        }


        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadControlMode(ref MXP_READCONTROLMODE_IN In, out MXP_READCONTROLMODE_OUT Out);
        public struct MXP_READCONTROLMODE_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Enable;
        }
        public struct MXP_READCONTROLMODE_OUT
        {
            public MXP_AXIS_REF Axis;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public UInt32 IsControlMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_WriteControlModeCmd(UInt32 MotionBlockIndex, ref MXP_WRITECONTROLMODE_IN In);
        public struct MXP_WRITECONTROLMODE_IN
        {
            public MXP_AXIS_REF Axis;
            public Byte Execute;
            public MXP_CONTROLMODE_ENUM ControlMode;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_GetWriteControlModeOutParam(UInt32 MotionBlockIndex, out MXP_WRITECONTROLMODE_OUT Out);
        public struct MXP_WRITECONTROLMODE_OUT
        {
            public MXP_AXIS_REF Axis;

            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadPDOData(ref MXP_READPDODATA_IN In, out MXP_READPDODATA_OUT Out, Byte[] Data);
        public struct MXP_READPDODATA_IN
        {
            public MXP_INPUT_REF Input;

            public Byte Enable;
            public Byte Direction;
            public UInt16 Offset;
            public UInt16 Size;
        }

        public struct MXP_READPDODATA_OUT
        {
            public MXP_INPUT_REF Input;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ReadFollowingErrorValue(ref MXP_READFOLLOWINGERRORVALUE_IN In, out MXP_READFOLLOWINGERRORVALUE_OUT Out);
        public struct MXP_READFOLLOWINGERRORVALUE_IN
        {
            public MXP_AXIS_REF Axis;

            public Byte Enable;
        }

        public struct MXP_READFOLLOWINGERRORVALUE_OUT
        {
            public MXP_AXIS_REF Axis;

            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            public Single FollowingErrorValue;
        }


        //***********************************************************************************************************************************************************/
        // SDO Read/Write
        //***********************************************************************************************************************************************************/
        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ET_ReadParameterCmd(UInt32 MotionBlockIndex, ref MXP_ET_READPARAMETER_IN In);
        public struct MXP_ET_READPARAMETER_IN
        {
            public UInt32 SlaveNo;
            public UInt32 Index;
            public UInt32 SubIndex;
            public UInt32 BufLen;
        }

        [DllImport("MXP_SoftMotion.dll", CharSet = CharSet.Ansi)]
        extern public static int MXP_ET_GetReadParameterOutParam(UInt32 MotionBlockIndex, out MXP_ET_READPARAMETER_OUT Out);
        public struct MXP_ET_READPARAMETER_OUT
        {
            public UInt32 SlaveNo;
            public Byte Valid;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Byte[] Data;
        }

        [DllImport("MXP_SoftMotion.dll")]
        extern public static int MXP_ET_WriteParameterCmd(UInt32 MotionBlockIndex, ref MXP_ET_WRITEPARAMETER_IN In);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        [Serializable]
        public struct MXP_ET_WRITEPARAMETER_IN
        {
            public UInt32 SlaveNo;
            public UInt32 Index;
            public UInt32 SubIndex;
            public UInt32 BufLen;
            public UInt32 Data;
        }

        [DllImport("MXP_SoftMotion.dll", CharSet = CharSet.Ansi)]
        extern public static int MXP_ET_GetWriteParameterOutParam(UInt32 MotionBlockIndex, out MXP_ET_WRITEPARAMETER_OUT Out);
        public struct MXP_ET_WRITEPARAMETER_OUT
        {
            public UInt32 SlaveNo;
            public Byte Done;
            public Byte Busy;
            public Byte Error;
            public UInt16 ErrorID;
        }        
    }
} // EOF
