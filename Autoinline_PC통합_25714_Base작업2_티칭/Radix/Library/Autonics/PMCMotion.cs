using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace Radix
{
    public enum enumAxis
    {
        X,
        Y,
        XY
    }

    #region 구조체
    [StructLayout(LayoutKind.Sequential)] // 구조체 사이즈 고정
    public struct PMC_PARADATA
    {
        public int iErrorState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] // 배열 사이즈 고정
        public bool[] bLmtStopMod;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bLmtActLev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bSCurve;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bEndPEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bDecValue;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bSofLmtEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bPowHomStart;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bPowPgmStart;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bInput0Lev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bInput1Lev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int iPulseType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iSpdMul;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iJrkSpd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iAccSpdRate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iDecSpdRate;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iStrSpd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iDrvSpd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iDrvSpd1Pgm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iDrvSpd2Pgm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iDrvSpd3Pgm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iDrvSpd4Pgm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iTim1Pgm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iTim2Pgm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iTim3Pgm;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public long[] lSofLmtP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public long[] lSofLmtM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iEndPWidth;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iPulSclNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iPulSclDen;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomMod1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomMod1Dir;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomMod2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomMod2Dir;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomMod3;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomMod3Dir;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomMod4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomMod4Dir;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomEndPosClr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomSig0Lev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomSig1Lev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomSig2Lev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iHomLowSpd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] iHomHighSpd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public long[] lHomOffset;

    }// PMC_PARADATA;

    [StructLayout(LayoutKind.Sequential)]
    public struct PMC_SOFTVERSION
    {
        public int iErrorState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public char[] cModName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
        public char[] cSofVer;

    }// PMC_SOFTVERSION;

    [StructLayout(LayoutKind.Sequential)]
    public struct PMC_ERRORSTATE
    {
        public int iErrorState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bSofLmtErrP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bSofLmtErrM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHardLmtErrP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHardLmtErrM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bEmgErr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bPgmErr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomErr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bInxErr;
    }// PMC_ERRORSTATE;

    [StructLayout(LayoutKind.Sequential)]
    public struct PMC_RUNSTATE
    {
        public int iErrorState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomIsRun;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bJogIsRun;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bPgmIsRun;

    }//  PMC_RUNSTATE;

    [StructLayout(LayoutKind.Sequential)]
    public struct PMC_HOMMOD
    {
        public int iErrorState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bEnable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bDirection;
    }// HOMMOD;

    public struct PMC_PARALLELSTATE
    {
        public int iErrorState;
        public bool HOME;
        public bool STROBE;
        public bool X;
        public bool Y;
        public bool MODE0;
        public bool MODE1;
        public bool STEPSL0;
        public bool STEPSL1;
        public bool STEPSL2;
        public bool STEPSL3;
        public bool STEPSL4;
        public bool STEPSL5;
    }// PARALLELSTATE;

    [StructLayout(LayoutKind.Sequential)]
    public struct PMC_AXISSTATE
    {
        public int iErrorState;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomSig0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomSig1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bHomSig2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] LmtP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] LmtM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] EMG;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bInput0Lev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public bool[] bInput1Lev;
    }// AXISSTATE;

    /* 이전 구현
public struct PMC_AXISSTATE
{
    public int iErrorState;
    public byte[] bHomSig0;
    public byte[] bHomSig1;
    public byte[] bHomSig2;
    public byte[] LmtP;
    public byte[] LmtM;
    public byte[] EMG;
    public byte[] bInput0Lev;
    public byte[] bInput1Lev;
}

public struct PMC_ERRORSTATE
{
    public int iErrorState;
    public byte[] bSofLmtErrP;
    public byte[] bSofLmtErrM;
    public byte[] bHardLmtErrP;
    public byte[] bHardLmtErrM;
    public byte[] bEmgErr;
    public byte[] bPgmErr;
    public byte[] HomErr;
    public byte[] InxErr;
}

public struct PMC_RUNSTATE
{
    public int iErrorState;
    public byte[] bHomIsRun;
    public byte[] bJogIsRun;
    public byte[] bPgmIsRun;
}

public struct PMC_PARADATA
{
    public int iErrorState;
    public byte[] bLmtStopMod;
    public byte[] bLmtAcLevel;
    public byte[] bSCurve;
    public byte bEndPEnable;
}
//*/
    #endregion

    public class PMCMotion
    {
        #region DLL Import
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Open(int PortNum, int BaudRate);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetBaudrate(int PortNum, char nNodeId, int BaudRate);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Timeout(int PortNum, int RITimeout, int RTTimeoutMultiplier, int RTTimeoutConstant, int WTTimeoutMultiplier, int WTTimeoutConstant);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Reset(int PortNum, char nNodeId);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Read(int PortNum);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Send(int PortNum, ref char nData, int data_length);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Close(int PortNum);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_ClrINCPos(int PortNum, char nNodeId, char axis);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_ClrABSPos(int PortNum, char nNodeId, char axis);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetDrvSpd(int PortNum, char nNodeId, char axis, int nDrvIndex);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetStrSpd(int PortNum, char nNodeId, char axis, int iStrSpd);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetStrSpd(int PortNum, char nNodeId, char axis, ref int iStrSpd);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_HomRun(int PortNum, char nNodeId, char axis);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_HomStop(int PortNum, char nNodeId, char axis);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_ABSMove(int PortNum, char nNodeId, char axis, int lPos_X, int IPos_Y);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_INCMove(int PortNum, char nNodeId, char axis, long lPos_X, long IPos_Y);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_LIDMove(int PortNum, char nNodeId, bool bFLS, long lXEndPos, long lYEndPos);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_CIDMove(int PortNum, char nNodeId, bool bFLS, long lRadius, long lMDP);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_FIDMove(int PortNum, char nNodeId, bool bFLS, long lXCenPos, long lYCenPos, long lXEndPos, long lYEndPos, long lMDP);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_RIDMove(int PortNum, char nNodeId, bool bFLS, long lXCenPos, long lYCenPos, long lXEndPos, long lYEndPos, long lMDP);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SlowStop(int PortNum, char nNodeId, char axis);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_EmgStop(int PortNum, char nNodeId);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetSpdMul(int PortNum, char nNodeId, char axis, int iSpdMul);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetSpdMul(int PortNum, char nNodeId, char axis, ref int iSpdMul);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetJrkSpd(int PortNum, char nNodeId, char axis, int iJrkSpd);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetJrkSpd(int PortNum, char nNodeId, char axis, ref int iJrkSpd);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetAccSpdRate(int PortNum, char nNodeId, char axis, int iAccSpdRate);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetAccSpdRate(int PortNum, char nNodeId, char axis, ref int iAccSpdRate);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetDecSpdRate(int PortNum, char nNodeId, char axis, int iDecSpdRate);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetDecSpdRate(int PortNum, char nNodeId, char axis, ref int iDecSpdRate);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetCurDrvSpd(int PortNum, char nNodeId, char axis, ref int IDrvSpd_X, ref int IDrvSpd_Y);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetDrvSpdPgm(int PortNum, char nNodeId, char axis, int nDrvIndex, int iDrvSpd);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetDrvSpdPgm(int PortNum, char nNodeId, char axis, int nDrvIndex, ref int iDrvSpd);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetSofLmt(int PortNum, char nNodeId, char axis, int iDirection, long lSoftLmt);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetSofLmt(int PortNum, char nNodeId, char axis, int iDirection, ref long lSoftLmt);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetEndPWidth(int PortNum, char nNodeId, char axis, int iEndPWidth);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetEndPWidth(int PortNum, char nNodeId, char axis, ref int iEndPWidth);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetPulSclNum(int PortNum, char nNodeId, char axis, int dPulScl);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetPulSclNum(int PortNum, char nNodeId, char axis, ref int dPulScl);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetPulSclDen(int PortNum, char nNodeId, char axis, int dPulScl);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetPulSclDen(int PortNum, char nNodeId, char axis, ref int dPulScl);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetHomSpd(int PortNum, char nNodeId, char axis, bool bSpd, int iSpd);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetHomSpd(int PortNum, char nNodeId, char axis, bool bSpd, ref int iSpd);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetHomOffset(int PortNum, char nNodeId, char axis, long lOffset);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetHomOffset(int PortNum, char nNodeId, char axis, ref long lOffset);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetTimPgm(int PortNum, char nNodeId, char axis, int nIndex, int iPostTim);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetTimPgm(int PortNum, char nNodeId, char axis, int nIndex, ref int iPostTim);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetLmtStopMod(int PortNum, char nNodeId, char axis, bool bInstant);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetLmtStopMod(int PortNum, char nNodeId, char axis, ref bool bStopType);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetLmtActLev(int PortNum, char nNodeId, char axis, bool bLmtActLev);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetLmtActLev(int PortNum, char nNodeId, char axis, ref bool bLevel);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetSCurve(int PortNum, char nNodeId, char axis, bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetSCurve(int PortNum, char nNodeId, char axis, ref bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetEndPEnable(int PortNum, char nNodeId, char axis, bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetEndPEnable(int PortNum, char nNodeId, char axis, ref bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetDecValue(int PortNum, char nNodeId, char axis, bool bDec);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetDecValue(int PortNum, char nNodeId, char axis, ref bool bDec);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetSofLmtEnable(int PortNum, char nNodeId, char axis, bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetSofLmtEnable(int PortNum, char nNodeId, char axis, ref bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetPowHomStart(int PortNum, char nNodeId, char axis, bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetPowHomStart(int PortNum, char nNodeId, char axis, ref bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetPowPgmStart(int PortNum, char nNodeId, char axis, bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetPowPgmStart(int PortNum, char nNodeId, char axis, ref bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetInputLev(int PortNum, char nNodeId, char axis, bool bInPort, bool bActLev);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetInputLev(int PortNum, char nNodeId, char axis, bool bInPort, ref bool bActLev);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetPulseType(int PortNum, char nNodeId, int iPulseType);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetPulseType(int PortNum, char nNodeId, ref int iPulseType);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Step1Enable(int PortNum, char nNodeId, char axis, bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Step2Enable(int PortNum, char nNodeId, char axis, bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Step3Enable(int PortNum, char nNodeId, char axis, bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Step4Enable(int PortNum, char nNodeId, char axis, bool bEnable);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Step1Direction(int PortNum, char nNodeId, char axis, bool bDirection);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Step2Direction(int PortNum, char nNodeId, char axis, bool bDirection);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Step3Direction(int PortNum, char nNodeId, char axis, bool bDirection);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Step4Direction(int PortNum, char nNodeId, char axis, bool bDirection);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetHomEndPosClr(int PortNum, char nNodeId, char axis, ref bool bClear);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetHomSigLev(int PortNum, char nNodeId, char axis, int nHomSigNo, bool bLevel);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetHomSigLev(int PortNum, char nNodeId, char axis, int nHomSigNo, ref bool bLevel);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetHomMod(int PortNum, char nNodeId, char axis, int nStepNo, bool bEnable, bool bDirection);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetHomMod(int PortNum, char nNodeId, char axis, int nStepNo, ref PMC_HOMMOD pMode);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmABS(int PortNum, char nNodeId, char axis, int nStepNo, long lPos, int nSpeed, int nTimer, bool bEndP, bool bBoth);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmINC(int PortNum, char nNodeId, char axis, int nStepNo, long lPos, int nSpeed, int nTimer, bool bEndP, bool bBoth);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmHOM(int PortNum, char nNodeId, char axis, int nStepNo, bool bEndP, bool bBoth);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmLID(int PortNum, char nNodeId, int nStepNo, long lXEndPos, long lYEndPos, bool bFLS, int nSpeed, int nTimer, bool bEndP);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmCID(int PortNum, char nNodeId, int nStepNo, long lRadius, bool bFLS, int nSpeed, int nTimer, bool bEndP);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmFID(int PortNum, char nNodeId, int nStepNo, long XCenPos, long XEndPos, long YCenPos, long YEndPos, bool bFLS, int nSpeed, int nTimer, bool bEndP);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmRID(int PortNum, char nNodeId, int nStepNo, long XCenPos, long XEndPos, long YCenPos, long YEndPos, bool bFLS, int nSpeed, int nTimer, bool bEndP);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmICJ(int PortNum, char nNodeId, char axis, int nStepNo, int nJumpStep, int nInputPtNo);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmIRD(int PortNum, char nNodeId, char axis, int nStepNo, int nInputPtNo);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmOPC(int PortNum, char nNodeId, char axis, int nStepNo, int nInputPtNo, bool bON);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmOPT(int PortNum, char nNodeId, char axis, int nStepNo, int nOutPtNo, int iOnTim, bool bNextStep);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmJMP(int PortNum, char nNodeId, char axis, int nStepNo, int nJumpStep);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmREP(int PortNum, char nNodeId, char axis, int nStepNo, int nRepCnt);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmRPE(int PortNum, char nNodeId, char axis, int nStepNo);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmEND(int PortNum, char nNodeId, char axis, int nStepNo);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmTIM(int PortNum, char nNodeId, char axis, int nStepNo, int nOnTim);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmNOP(int PortNum, char nNodeId, char axis, int nStepNo);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmRun(int PortNum, char nNodeId, char axis, int iStepNo);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmStepRun(int PortNum, char nNodeId, char axis, int iStepNo);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmPause(int PortNum, char nNodeId, char axis);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmReRun(int PortNum, char nNodeId, char axis);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_PgmStop(int PortNum, char nNodeId, char axis);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_DelPgmDataAll(int PortNum, char nNodeId, char axis);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_DelPgmData(int PortNum, char nNodeId, char axis, int nStepNo);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_Init(int PortNum, char nNodeId);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_IsCon(int PortNum, char nNodeId, ref bool bOn);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetCurPos(int PortNum, char nNodeId, char axis, ref long lCurPos);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetCurPgmNo(int PortNum, char nNodeId, char axis, ref int iCurPgmNo);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_ContMove(int PortNum, char nNodeId, char axis, bool bDirection);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetUserOut(int PortNum, char nNodeId, char axis, bool bPort, bool bOn);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetParaAll(int PortNum, char nNodeId, char axis, ref PMC_PARADATA pData);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetParaOPMAll(int PortNum, char nNodeId, char axis, ref PMC_PARADATA pData);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetParaPMAll(int PortNum, char nNodeId, char axis, ref PMC_PARADATA pData);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetParaHSMAll(int PortNum, char nNodeId, char axis, ref PMC_PARADATA pData);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetErrorSt(int PortNum, char nNodeId, char axis, ref PMC_ERRORSTATE pError);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_IsRun(int PortNum, char nNodeId, char axis, ref PMC_RUNSTATE pRun);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetModName(int PortNum, char nNodeId, ref PMC_SOFTVERSION pVersion);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetSofVer(int PortNum, char nNodeId, ref PMC_SOFTVERSION pVersion);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetParallelIO(int PortNum, char nNodeId, ref PMC_PARALLELSTATE pState);

        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr autpmc_GetAxisIO(int PortNum, char nNodeId, char axis, ref PMC_AXISSTATE pState);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetCurDrvSpd(int PortNum, char nNodeId, char axis, int iDrvSpd_X, int iDrvSpd_Y);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_GetCurPos(int PortNum, char nNodeId, char axis, ref int ICurPos_X, ref int ICurPos_Y);
        [DllImport("PMCLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int autpmc_SetHomEndPosClr(int PortNum, char nNodeId, char axis, bool bEnable);


        #endregion

        private Stopwatch watch = new Stopwatch();

        private int portNum;
        private char nNodeID;
        private int baudRate;

        public structAxisStatus[] axisStatus = new structAxisStatus[2];

        private System.Threading.Timer timerStatus = null; // Thread Timer
        private bool timerStart = false;
        private bool timerDoing = false;


        public void debug(string str)
        {
            //Util.Debug("PMCMotion : " + str);
        }

        public PMCMotion()
        {
        }

        public void SetPMCMotion(int PortNum, int NodeID, int BaudRate)
        {
            portNum = PortNum;
            nNodeID = (char)NodeID;
            baudRate = BaudRate;
        }

        public PMCMotion(int PortNum, int NodeID, int BaudRate)
        {
            portNum = PortNum;
            nNodeID = (char)NodeID;
            baudRate = BaudRate;
        }

        private void TimerStatus(Object state) // 쓰레드 타이머 함수
        {
            if (GlobalVar.GlobalStop)
            {
                timerStatus.Dispose();
                return;
            }
            try
            {
                if (timerDoing)
                {
                    return;
                }
                //if (!IsCon())
                //{
                //    return;
                //}
                timerDoing = true;

                int[] pos = GetCurPos(enumAxis.XY);
                int[] vel = GetDrvSpd(enumAxis.XY);

                PMC_AXISSTATE axisState = GetAxisIO(enumAxis.XY);
                PMC_ERRORSTATE errorState = GetErrorSt(enumAxis.XY);
                PMC_RUNSTATE runState = IsRun(enumAxis.XY);

                try
                {
                    if (IsCon())
                    {
                        axisStatus[0].Position = pos[0];
                        axisStatus[1].Position = pos[1];
                        axisStatus[0].Velocity = vel[0];
                        axisStatus[1].Velocity = vel[1];
                        axisStatus[0].HomeAbsSwitch = axisState.bHomSig1[0];
                        axisStatus[1].HomeAbsSwitch = axisState.bHomSig1[1];
                        axisStatus[0].LimitSwitchPos = axisState.LmtP[0];
                        axisStatus[1].LimitSwitchPos = axisState.LmtP[1];
                        axisStatus[0].LimitSwitchNeg = axisState.LmtP[0];
                        axisStatus[1].LimitSwitchNeg = axisState.LmtP[1];
                        axisStatus[0].Homing = runState.bHomIsRun[0];
                        axisStatus[1].Homing = runState.bHomIsRun[1];
                        axisStatus[0].StandStill = !runState.bHomIsRun[0] && !runState.bJogIsRun[0] && Math.Abs(axisStatus[0].Velocity) < 3;
                        axisStatus[1].StandStill = !runState.bHomIsRun[1] && !runState.bJogIsRun[1] && Math.Abs(axisStatus[1].Velocity) < 3;
                        axisStatus[0].Errored = errorState.bEmgErr[0] ||
                                        errorState.bHardLmtErrM[0] ||
                                        errorState.bHardLmtErrP[0] ||
                                        errorState.bHomErr[0];
                        axisStatus[1].Errored = errorState.bEmgErr[1] ||
                                        errorState.bHardLmtErrM[1] ||
                                        errorState.bHardLmtErrP[1] ||
                                        errorState.bHomErr[1];
                    }
                }
                catch (Exception ex)
                {
                    debug(ex.ToString());
                    debug(ex.StackTrace);
                }


                timerDoing = false;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            finally
            {
                timerDoing = false;
            }

        }


        /*
              * PMCMotion 통신시 한 지령 수행 중에 다른 지령 호출시 access denied 가 나온다.
              * semaphore를 이용해서 각 함수간에 락을 걸어야 함.
              * semaphore 무한 대기를 피하기 위해 1초 이상 걸려 있으면 락을 해제하도록 해야 함
              * 혹시 락 해제 후 중복 시행에 따라 에러 대비 try
              */

        #region 통신 관련

        public bool Open() // 모션 IC 초기화
        {
            try
            {
                //if (IsCon())
                //{
                //    return true;
                //}

                if (!timerStart)
                {
                    //TimerCallback CallBackStatus = new TimerCallback(TimerStatus);
                    //timerStatus = new System.Threading.Timer(CallBackStatus, false, 0, 100);
                    timerStart = true;
                }

                int res = autpmc_Open(portNum, baudRate);
                //debug("open result : " + portNum + "," + baudRate + " ==> " + res.ToString());
                /*
                if (res == 0)
                {
                    //debug("iscon : " + IsCon());
                }
                return res == 0;
                //*/
                return res == 0 && IsCon();
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool Reset() // 모션 IC 초기화
        {
            try
            {
                int res = autpmc_Reset(portNum, nNodeID);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool IsCon() // 통신 접속을 해서 데이터를 정상적으로 주고 받는지 확인
        {
            //debug("isCon");
            try
            {
                bool on = false;
                int res = autpmc_IsCon(portNum, nNodeID, ref on);
                return res == 0 && on;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetBaudrate(int BaudRate) // 통신 속도 변경
        {
            try
            {
                int res = autpmc_SetBaudrate(portNum, nNodeID, BaudRate);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool ClrABSPos(enumAxis axis) // 절대 위치를 초기화
        {
            try
            {
                int res = autpmc_ClrABSPos(portNum, nNodeID, (char)axis);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool ClrINCPos(enumAxis axis) // 상대 위치를 초기화
        {
            try
            {
                int res = autpmc_ClrINCPos(portNum, nNodeID, (char)axis);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool Close() // 통신 연결 해제
        {
            try
            {
                int res = autpmc_Close(portNum);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        #endregion


        #region 속도 설정
        public bool SetSpdMul(enumAxis axis, int mul) // 속도 배율 설정
        {
            try
            {
                int res = autpmc_SetSpdMul(portNum, nNodeID, (char)axis, mul);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetJrkSpd(enumAxis axis, int speed) // Jerk 속도 설정
        {
            try
            {
                int res = autpmc_SetJrkSpd(portNum, nNodeID, (char)axis, speed);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetAccSpdRate(enumAxis axis, int speed) // 가속 배율 설정
        {
            try
            {
                int res = autpmc_SetAccSpdRate(portNum, nNodeID, (char)axis, speed);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetDecSpdRate(enumAxis axis, int speed) // 감속 배율 설정
        {
            try
            {
                int res = autpmc_SetDecSpdRate(portNum, nNodeID, (char)axis, speed);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetStrSpd(enumAxis axis, int speed) // 초기 속도 설정
        {
            try
            {
                int res = autpmc_SetStrSpd(portNum, nNodeID, (char)axis, speed);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetCurDrvSpd(enumAxis axis, int speedX, int speedY) // 현재 속도 설정
        {
            try
            {
                int res = autpmc_SetCurDrvSpd(portNum, nNodeID, (char)axis, speedX, speedY);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetDrvSpd(enumAxis axis, int index) // 파라미터에 설정된 구동 속도 선택
        {
            try
            {
                int res = autpmc_SetDrvSpd(portNum, nNodeID, (char)axis, index);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }
        #endregion


        #region 상태 조회
        public bool GetLmtStopMod(enumAxis axis)
        {
            try
            {
                bool stopType = false;
                int res = autpmc_GetLmtStopMod(portNum, nNodeID, (char)axis, ref stopType);
                return res == 0 && stopType;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public PMC_AXISSTATE GetAxisIO(enumAxis axis) // X/Y축 입출력 커넥터 신호 읽기
        {
            //debug("GetAxisIO : " + axis.ToString());
            IntPtr ptrStat;
            PMC_AXISSTATE stat = new PMC_AXISSTATE();
            try
            {
                if (true) //IsCon())
                {
                    ptrStat = autpmc_GetAxisIO(portNum, nNodeID, (char)axis, ref stat);
                    //if (stat.iErrorState == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    //{
                    //    stat.iErrorState = 0;
                    //}
                    if (stat.bHomSig0 == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bHomSig0 = new bool[2];
                    }
                    if (stat.bHomSig1 == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bHomSig1 = new bool[2];
                    }
                    if (stat.bHomSig2 == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bHomSig2 = new bool[2];
                    }
                    if (stat.LmtP == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.LmtP = new bool[2];
                    }
                    if (stat.LmtM == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.LmtM = new bool[2];
                    }
                    if (stat.EMG == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.EMG = new bool[2];
                    }
                    if (stat.bInput0Lev == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bInput0Lev = new bool[2];
                    }
                    if (stat.bInput1Lev == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bInput1Lev = new bool[2];
                    }
                }
                //return (PMC_AXISSTATE)Marshal.PtrToStructure(ptrStat, typeof(PMC_AXISSTATE));
            }
            catch (AccessViolationException ax)
            {
                debug(ax.ToString());
                debug(ax.StackTrace);
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return stat;
        }

        public int[] GetCurPos(enumAxis axis) // 현재 좌표
        {
            int[] pos = new int[2];
            try
            {
                if (true) //IsCon())
                {
                    ulong tick = GlobalVar.TickCount64;
                    autpmc_GetCurPos(portNum, nNodeID, (char)axis, ref pos[0], ref pos[1]);
                    if (portNum == 11)
                    {
                        //debug("time : " + (GlobalVar.TickCount64 - tick));
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return pos;
        }

        public int[] GetDrvSpd(enumAxis axis) // 현재 속도
        {
            int[] speed = new int[2];
            try
            {
                if (true) //IsCon())
                {
                    autpmc_GetCurDrvSpd(portNum, nNodeID, (char)axis, ref speed[0], ref speed[1]);
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return speed;
        }

        public int PgmHom(enumAxis axis, int nStepNo, bool bEndP, bool bBoth)
        {
            try
            {
                int res = autpmc_PgmHOM(portNum, nNodeID, (char)axis, nStepNo, bEndP, bBoth);
                return res;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return 0;
        }

        public PMC_ERRORSTATE GetErrorSt(enumAxis axis) // 에러 상태
        {
            IntPtr ptrStat;
            PMC_ERRORSTATE stat = new PMC_ERRORSTATE();
            try
            {
                if (true) //IsCon())
                {
                    ptrStat = autpmc_GetErrorSt(portNum, nNodeID, (char)axis, ref stat);
                    //if (stat.iErrorState == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    //{
                    //    stat.iErrorState = 0;
                    //}
                    if (stat.bSofLmtErrP == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bSofLmtErrP = new bool[2];
                    }
                    if (stat.bSofLmtErrM == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bSofLmtErrM = new bool[2];
                    }
                    if (stat.bHardLmtErrP == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bHardLmtErrP = new bool[2];
                    }
                    if (stat.bHardLmtErrM == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bHardLmtErrM = new bool[2];
                    }
                    if (stat.bEmgErr == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bEmgErr = new bool[2];
                    }
                    if (stat.bPgmErr == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bPgmErr = new bool[2];
                    }
                    if (stat.bHomErr == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bHomErr = new bool[2];
                    }
                    if (stat.bInxErr == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bInxErr = new bool[2];
                    }
                }
                //return (PMC_ERRORSTATE)Marshal.PtrToStructure(ptrStat, typeof(PMC_ERRORSTATE));
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return stat;
        }

        public PMC_RUNSTATE IsRun(enumAxis axis) // 구동 상태
        {
            //debug("IsRun : " + axis.ToString());
            IntPtr ptrStat;
            PMC_RUNSTATE stat = new PMC_RUNSTATE();
            try
            {
                if (true) //IsCon())
                {
                    ptrStat = autpmc_IsRun(portNum, nNodeID, (char)axis, ref stat);
                    //if (stat.iErrorState == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    //{
                    //    stat.iErrorState = 0;
                    //}
                    if (stat.bHomIsRun == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bHomIsRun = new bool[2];
                    }
                    if (stat.bJogIsRun == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bJogIsRun = new bool[2];
                    }
                    if (stat.bPgmIsRun == null) // 지령 실패시 null이라서 null pointer 피하기 위해 기본값 할당
                    {
                        stat.bPgmIsRun = new bool[2];
                    }
                }
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            //return (PMC_RUNSTATE)Marshal.PtrToStructure(ptrStat, typeof(PMC_RUNSTATE));
            return stat;
        }

        #endregion


        #region Homing
        public bool SetHomEndPosClr(enumAxis axis, bool enable) // 원점 복귀 종료시 위치 카운터 초기화
        {
            try
            {
                int res = autpmc_SetHomEndPosClr(portNum, nNodeID, (char)axis, enable);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool HomRun(enumAxis axis) // 홈 이동
        {
            try
            {
                //debug("motion : " + portNum + " axis : " + ((int)axis).ToString());
                //SetHomEndPosClr(axis, true);
                int res = autpmc_HomRun(portNum, nNodeID, (char)axis);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool HomStop(enumAxis axis) // 원점 복귀 모드 종료
        {
            try
            {
                // 원점 초기화 해제
                //SetHomEndPosClr(axis, false);
                int res = autpmc_HomStop(portNum, nNodeID, (char)axis);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetHomSpd(enumAxis axis, int speed) // 원점구동 속도
        {
            try
            {
                if (autpmc_SetHomSpd(portNum, nNodeID, (char)axis, false, speed / 5) != 0)
                {
                    return false;
                }
                int res = autpmc_SetHomSpd(portNum, nNodeID, (char)axis, true, speed);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetHomMod(enumAxis axis, int stepNo, bool enable, bool dir) // 원점구동 모드
        {
            try
            {
                int res = autpmc_SetHomMod(portNum, nNodeID, (char)axis, stepNo, enable, dir);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }



        public bool Step1Enable(enumAxis axis, bool enable) // Step1 사용
        {
            try
            {
                int res = autpmc_Step1Enable(portNum, nNodeID, (char)axis, enable);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool Step2Enable(enumAxis axis, bool enable) // Step2 사용
        {
            try
            {
                int res = autpmc_Step2Enable(portNum, nNodeID, (char)axis, enable);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool Step3Enable(enumAxis axis, bool enable) // Step3 사용
        {
            try
            {
                int res = autpmc_Step3Enable(portNum, nNodeID, (char)axis, enable);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool Step4Enable(enumAxis axis, bool enable) // Step4 사용
        {
            try
            {
                int res = autpmc_Step4Enable(portNum, nNodeID, (char)axis, enable);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool Step1Direction(enumAxis axis, bool direction) // Step1 방향
        {
            try
            {
                int res = autpmc_Step1Direction(portNum, nNodeID, (char)axis, direction);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool Step2Direction(enumAxis axis, bool direction) // Step2 방향
        {
            try
            {
                int res = autpmc_Step2Direction(portNum, nNodeID, (char)axis, direction);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool Step3Direction(enumAxis axis, bool direction) // Step3 방향
        {
            try
            {
                int res = autpmc_Step3Direction(portNum, nNodeID, (char)axis, direction);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool Step4Direction(enumAxis axis, bool direction) // Step4 방향
        {
            try
            {
                int res = autpmc_Step4Direction(portNum, nNodeID, (char)axis, direction);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetHomSigLev(enumAxis axis, int sig, bool level) // 원점 신호의 논리 레벨 설정
        {
            try
            {
                int res = autpmc_SetHomSigLev(portNum, nNodeID, (char)axis, sig, level);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool SetHomOffset(enumAxis axis, long offset) // 원점 신호의 논리 레벨 설정
        {
            try
            {
                int res = autpmc_SetHomOffset(portNum, nNodeID, (char)axis, offset);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        #endregion

        #region 구동

        public bool SlowStop(enumAxis axis) // 감속 정지
        {
            try
            {
                int res = autpmc_SlowStop(portNum, nNodeID, (char)axis);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool EmgStop() // 긴급정지
        {
            try
            {
                int res = autpmc_EmgStop(portNum, nNodeID);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool ABSMove(enumAxis axis, int pos) // 절대 위치 이동
        {
            try
            {
                //debug("ABSMove " + portNum.ToString() + "," + ((int)nNodeID).ToString() + "," + axis.ToString() + "," + pos.ToString());
                int xPos = pos;
                int yPos = pos;
                if (axis == enumAxis.Y)
                {
                    xPos = 0;
                }
                if (axis == enumAxis.X)
                {
                    yPos = 0;
                }
                int res = autpmc_ABSMove(portNum, nNodeID, (char)axis, xPos, yPos);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool INCMove(enumAxis axis, long pos) // 상대 위치 이동
        {
            try
            {
                //debug("INCMove : " + pos.ToString());
                long xPos = pos;
                long yPos = pos;
                //if (axis == enumAxis.Y)
                //{
                //    xPos = 0;
                //}
                //if (axis == enumAxis.X)
                //{
                //    yPos = 0;
                //}
                int res = autpmc_INCMove(portNum, nNodeID, (char)axis, xPos, yPos);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public bool ContMove(enumAxis axis, bool plus) // 연속 구동
        {
            try
            {
                /*
                int convSpeed = 2000; // (int)FuncMotion.MMToPulse(GetSpeed(), GlobalVar.GearRatio[6], GlobalVar.RevMM[6], GlobalVar.RevPulse[6]) / 3;
                GlobalVar.WorkConv.SetDrvSpd(enumAxis.XY, 1);
                GlobalVar.WorkConv.SetStrSpd(enumAxis.XY, 30);
                GlobalVar.WorkConv.SetCurDrvSpd(enumAxis.XY, convSpeed, convSpeed);
                GlobalVar.WorkConv.SetAccSpdRate(enumAxis.XY, convSpeed * 10);
                GlobalVar.WorkConv.SetDecSpdRate(enumAxis.XY, convSpeed * 10);
                GlobalVar.WorkConv.SetJrkSpd(enumAxis.XY, convSpeed * 100);
                //*/
                int res = autpmc_ContMove(portNum, nNodeID, (char)axis, plus);
                return res == 0;
            }
            catch (Exception ex)
            {
                debug(ex.ToString());
                debug(ex.StackTrace);
            }
            return false;
        }

        public void SetDriveSpeedPgm(enumAxis axis, int index, int speed)
        {
            autpmc_SetDrvSpdPgm(portNum, nNodeID, (char)axis, index, speed);
        }

        #endregion

    }
}
