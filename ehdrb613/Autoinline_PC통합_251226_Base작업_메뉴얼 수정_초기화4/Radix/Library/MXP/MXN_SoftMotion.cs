using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Radix
{
    class MXN
    {
        //***********************************************************************************************************************************************************/
        // System Function
        //***********************************************************************************************************************************************************/
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_InitKernel(ref UInt16 usStatus);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Destroy();

        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_GetOnlineMode(ref UInt16 usStatus);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_GetKernelStatus();
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_GetExitProc(UInt32 uiProcNum, ref Byte ucStatus);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_SetExitProc(UInt32 uiProcNum);

        //***********************************************************************************************************************************************************/
        // Command Function
        //***********************************************************************************************************************************************************/
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Power(UInt16 usServoOn);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Stop(UInt32 uiAxisNo);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_MoveAbsolute(ref MXN_MOVEABSOLUTE_IN InParam);
        public struct MXN_MOVEABSOLUTE_IN
        {
            public UInt32 uiAxisNo;
            public Int32 iVelocity;
            public Int32 iPosition;

        }

        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_MoveRelative(ref MXN_MOVERELATIVE_IN InParam);
        public struct MXN_MOVERELATIVE_IN
        {
            public UInt32 uiAxisNo;
            public Int32 iVelocity;
            public Int32 iDistance;
        }

        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_ReadActualPosition(UInt32 uiAxisNo, ref Int32 iPosition);

        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_ReadAxisInfo(UInt32 uiAxisNo, ref MXN_READAXISINFO_OUT OutAxisParam);
        public struct MXN_READAXISINFO_OUT
        {
            public Byte ucPowerOn;
            public Byte ucIsHomed;
            public Int32 iVelocity;
        }

        //***********************************************************************************************************************************************************/ 
        // Register
        //***********************************************************************************************************************************************************/
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Read_X(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, ref UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Write_X(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Read_Y(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, ref UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Write_Y(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Read_T(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, ref UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Write_T(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Read_C(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, ref UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Write_C(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Read_R(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, ref UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Write_R(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Read_D(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, ref UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Write_D(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Read_G(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, ref UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Write_G(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Read_F(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, ref UInt32 data);
        [DllImport("MXN_SoftMotion.dll")]
        extern public static int MXN_Write_F(Int32 type, Int32 idx, Int32 startBit, Int32 endBit, UInt32 data);

        //***********************************************************************************************************************************************************/ 
        // Define
        //***********************************************************************************************************************************************************/
        public const Int16 STATE_NONE = 0x00;
        public const Int16 STATE_INIT = 0x01;
        public const Int16 STATE_PREOP = 0x02;
        public const Int16 STATE_BOOT = 0x03;
        public const Int16 STATE_SAFEOP = 0x04;
        public const Int16 STATE_OP = 0x08;

        public const Int16 REG_DATA = 0;
        public const Int16 REG_BIT = 1;

        public class KernelReturn
        {
            public const Int16 RET_NO_ERROR = 0;
            public const Int16 RET_ERROR_FUNCTION = -1;
            public const Int16 RET_ERROR_FULL = -2;
            public const Int16 RET_ERROR_WRONG_INDEX = -3;
            public const Int16 RET_ERROR_WRONG_AXISNO = -4;
            public const Int16 RET_ERROR_MOTIONBUSY = -5;
            public const Int16 RET_ERROR_WRONG_SLAVENO = -6;
            public const Int16 RET_ERROR_WRONG_CAMTABLENO = -7;
            public const Int16 RET_ERROR_WRONG_ECMASTERNO = -8;
            public const Int16 RET_ERROR_WRONG_ECSLAVENO = -9;
            public const Int16 RET_ERROR_NOT_OPMODE = -10;
            public const Int16 RET_ERROR_NOTRUNNING = -11;

        }

        public class KernelStatus
        {
            public const Int16 SYSTEM_UNLICENSED = -2;
            public const Int16 SYSTEM_IDLE = 1;
            public const Int16 SYSTEM_KILLING = 2;
            public const Int16 SYSTEM_KILLED = 3;
            public const Int16 SYSTEM_CREATING = 4;
            public const Int16 SYSTEM_CREATED = 5;
            public const Int16 SYSTEM_INITING = 6;
            public const Int16 SYSTEM_INITED = 7;
            public const Int16 SYSTEM_READY = 8;
            public const Int16 SYSTEM_RUN = 9;
        }
    }
}
