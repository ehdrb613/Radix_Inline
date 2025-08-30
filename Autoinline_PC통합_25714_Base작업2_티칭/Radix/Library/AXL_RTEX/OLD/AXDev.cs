/****************************************************************************
*****************************************************************************
**
** File Name
** ---------
**
** AXDev.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Ajinextek Develop Library Header File
** 
**
*****************************************************************************
*****************************************************************************
**
** Source Change Indices
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

public class CAXDev
{

//========== ���� �� ��� Ȯ���Լ�(Info) - Infomation =================================================================================

    // Board Number�� �̿��Ͽ� Board Address ã��
    [DllImport("AXL.dll")] public static extern uint AxlGetBoardAddress(int nBoardNo, ref uint upBoardAddress);
    // Board Number�� �̿��Ͽ� Board ID ã��
    [DllImport("AXL.dll")] public static extern uint AxlGetBoardID(int nBoardNo, ref uint upBoardID);
    // Board Number�� �̿��Ͽ� Board Version ã��
    [DllImport("AXL.dll")] public static extern uint AxlGetBoardVersion(int nBoardNo, ref uint upBoardVersion);
    // Board Number�� Module Position�� �̿��Ͽ� Module ID ã��
    [DllImport("AXL.dll")] public static extern uint AxlGetModuleID(int nBoardNo, int nModulePos, ref uint upModuleID);
    // Board Number�� Module Position�� �̿��Ͽ� Module Version ã��
    [DllImport("AXL.dll")] public static extern uint AxlGetModuleVersion(int nBoardNo, int nModulePos, ref uint upModuleVersion);
    // Board Number�� Module Position�� �̿��Ͽ� Network Node ���� Ȯ��
    [DllImport("AXL.dll")] public static extern uint AxlGetModuleNodeInfo(int nBoardNo, int nModulePos, ref uint upNetNo, ref uint upNodeAddr);

    // Board�� ����� ���� Data Flash Write (PCI-R1604[RTEX master board]����)
    // lPageAddr(0 ~ 199)
    // lByteNum(1 ~ 120)
    // ����) Flash�� ����Ÿ�� ������ ���� ���� �ð�(�ִ� 17mSec)�� �ҿ�Ǳ⶧���� ���� ����� ���� �ð��� �ʿ���.
    [DllImport("AXL.dll")] public static extern uint AxlSetDataFlash(int nBoardNo, int nPageAddr, int nBytesNum, ref byte upSetData);

    // Board�� ����� ���� Data Flash Read(PCI-R1604[RTEX master board]����)
    // lPageAddr(0 ~ 199)
    // lByteNum(1 ~ 120)
    [DllImport("AXL.dll")] public static extern uint AxlGetDataFlash(int nBoardNo, int nPageAddr, int nBytesNum, ref uint upGetData);
    // Board�� ����� ESTOP �ܺ� �Է� ��ȣ�� �̿��� InterLock ��� ��� ���� �� ������ ���� ����� ���� (PCI-Rxx00[MLIII master board]����)
    // 1. ��� ����
    //      ����: ��� ��� ������ �ܺο��� ESTOP ��ȣ �ΰ��� ���忡 ����� ��� ���� ��忡 ���ؼ� ESTOP ���� ��� ����    
    //    0: ��� ������� ����(�⺻ ������)
    //    1: ��� ���
    // 2. ������ ���� ��
    //      �Է� ���� ��� ���� ���� 1 ~ 40, ���� msec
    // Board �� dwInterLock, dwDigFilterVal�� �̿��Ͽ� EstopInterLock ��� ����
    [DllImport("AXL.dll")] public static extern uint AxlSetEStopInterLock(int nBoardNo, uint dwInterLock, uint dwDigFilterVal);
    // Board�� ������ dwInterLock, dwDigFilterVal ������ ��������
    [DllImport("AXL.dll")] public static extern uint AxlGetEStopInterLock(int nBoardNo, ref uint dwInterLock, ref uint dwDigFilterVal);
    // Board�� �Էµ� EstopInterLock ��ȣ�� �д´�.
    [DllImport("AXL.dll")] public static extern uint AxlReadEStopInterLock(int nBoardNo, ref uint dwInterLock);
        
 
    // Board Number�� Module Position�� �̿��Ͽ� AIO Module Number ã��
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetModuleNo(int nBoardNo, int nModulePos, ref int npModuleNo);
    // Board Number�� Module Position�� �̿��Ͽ� DIO Module Number ã��
    [DllImport("AXL.dll")] public static extern uint AxdInfoGetModuleNo(int nBoardNo, int nModulePos, ref int npModuleNo);

    // ���� �࿡ byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommand(int nAxisNo, byte sCommand);
    // ���� �࿡ 8bit byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommandData08(int nAxisNo, byte sCommand, uint uData);
    // ���� �࿡ 8bit byte ��������
    [DllImport("AXL.dll")] public static extern uint AxmGetCommandData08(int nAxisNo, byte sCommand, ref uint upData);
    // ���� �࿡ 16bit byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommandData16(int nAxisNo, byte sCommand, uint uData);
    // ���� �࿡ 16bit byte ��������
    [DllImport("AXL.dll")] public static extern uint AxmGetCommandData16(int nAxisNo, byte sCommand, ref uint upData);
    // ���� �࿡ 24bit byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommandData24(int nAxisNo, byte sCommand, uint uData);
    // ���� �࿡ 24bit byte ��������
    [DllImport("AXL.dll")] public static extern uint AxmGetCommandData24(int nAxisNo, byte sCommand, ref uint upData);
    // ���� �࿡ 32bit byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommandData32(int nAxisNo, byte sCommand, uint uData);
    // ���� �࿡ 32bit byte ��������
    [DllImport("AXL.dll")] public static extern uint AxmGetCommandData32(int nAxisNo, byte sCommand, ref uint upData);
    
    // ���� �࿡ byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommandQi(int nAxisNo, byte sCommand);
    // ���� �࿡ 8bit byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommandData08Qi(int nAxisNo, byte sCommand, uint uData);
    // ���� �࿡ 8bit byte ��������
    [DllImport("AXL.dll")] public static extern uint AxmGetCommandData08Qi(int nAxisNo, byte sCommand, ref uint upData);
    // ���� �࿡ 16bit byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommandData16Qi(int nAxisNo, byte sCommand, uint uData);
    // ���� �࿡ 16bit byte ��������
    [DllImport("AXL.dll")] public static extern uint AxmGetCommandData16Qi(int nAxisNo, byte sCommand, ref uint upData);
    // ���� �࿡ 24bit byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommandData24Qi(int nAxisNo, byte sCommand, uint uData);
    // ���� �࿡ 24bit byte ��������
    [DllImport("AXL.dll")] public static extern uint AxmGetCommandData24Qi(int nAxisNo, byte sCommand, ref uint upData);
    // ���� �࿡ 32bit byte Setting
    [DllImport("AXL.dll")] public static extern uint AxmSetCommandData32Qi(int nAxisNo, byte sCommand, uint uData);
    // ���� �࿡ 32bit byte ��������
    [DllImport("AXL.dll")] public static extern uint AxmGetCommandData32Qi(int nAxisNo, byte sCommand, ref uint upData);
    
    // ���� �࿡ Port Data �������� - IP
    [DllImport("AXL.dll")] public static extern uint AxmGetPortData(int nAxisNo,  uint wOffset, ref uint upData);
    // ���� �࿡ Port Data Setting - IP
    [DllImport("AXL.dll")] public static extern uint AxmSetPortData(int nAxisNo, uint wOffset, uint dwData);

    // ���� �࿡ Port Data �������� - QI
    [DllImport("AXL.dll")] public static extern uint AxmGetPortDataQi(int nAxisNo, uint byOffset, ref uint wData);
    // ���� �࿡ Port Data Setting - QI
    [DllImport("AXL.dll")] public static extern uint AxmSetPortDataQi(int nAxisNo, uint byOffset, uint wData);
        
    // ���� �࿡ ��ũ��Ʈ�� �����Ѵ�. - IP
    // sc    : ��ũ��Ʈ ��ȣ (1 - 4)
    // event : �߻��� �̺�Ʈ SCRCON �� �����Ѵ�.
    //         �̺�Ʈ ���� �హ������, �̺�Ʈ �߻��� ��, �̺�Ʈ ���� 1,2 �Ӽ� �����Ѵ�.
    // cmd   : � ������ �ٲܰ����� ���� SCRCMD�� �����Ѵ�.
    // data  : � Data�� �ٲܰ����� ����
    [DllImport("AXL.dll")] public static extern uint AxmSetScriptCaptionIp(int nAxisNo, int sc, uint uEvent, uint data);
    // ���� �࿡ ��ũ��Ʈ�� ��ȯ�Ѵ�. - IP
    [DllImport("AXL.dll")] public static extern uint AxmGetScriptCaptionIp(int nAxisNo, int sc, ref uint uEvent, ref uint data);

    // ���� �࿡ ��ũ��Ʈ�� �����Ѵ�. - QI
    // sc    : ��ũ��Ʈ ��ȣ (1 - 4)
    // event : �߻��� �̺�Ʈ SCRCON �� �����Ѵ�.
    //         �̺�Ʈ ���� �హ������, �̺�Ʈ �߻��� ��, �̺�Ʈ ���� 1,2 �Ӽ� �����Ѵ�.
    // cmd   : � ������ �ٲܰ����� ���� SCRCMD�� �����Ѵ�.
    // data  : � Data�� �ٲܰ����� ����
    [DllImport("AXL.dll")] public static extern uint AxmSetScriptCaptionQi(int nAxisNo, int sc, uint uEvent, uint cmd, uint data);
    // ���� �࿡ ��ũ��Ʈ�� ��ȯ�Ѵ�. - QI
    [DllImport("AXL.dll")] public static extern uint AxmGetScriptCaptionQi(int nAxisNo, int sc, ref uint uEvent, ref uint cmd, ref uint data);

    // ���� �࿡ ��ũ��Ʈ ���� Queue Index�� Clear ��Ų��.
    // uSelect IP. 
    // uSelect(0): ��ũ��Ʈ Queue Index �� Clear�Ѵ�.
    //        (1): ĸ�� Queue�� Index Clear�Ѵ�.

    // uSelect QI. 
    // uSelect(0): ��ũ��Ʈ Queue 1 Index �� Clear�Ѵ�.
    //        (1): ��ũ��Ʈ Queue 2 Index �� Clear�Ѵ�.

    [DllImport("AXL.dll")] public static extern uint AxmSetScriptCaptionQueueClear(int nAxisNo, uint uSelect);
    
    // ���� �࿡ ��ũ��Ʈ ���� Queue�� Index ��ȯ�Ѵ�. 
    // uSelect IP
    // uSelect(0): ��ũ��Ʈ Queue Index�� �о�´�.
    //        (1): ĸ�� Queue Index�� �о�´�.

    // uSelect QI. 
    // uSelect(0): ��ũ��Ʈ Queue 1 Index�� �о�´�.
    //        (1): ��ũ��Ʈ Queue 2 Index�� �о�´�.

    [DllImport("AXL.dll")] public static extern uint AxmGetScriptCaptionQueueCount(int nAxisNo, ref uint updata, uint uSelect);

    // ���� �࿡ ��ũ��Ʈ ���� Queue�� Data���� ��ȯ�Ѵ�. 
    // uSelect IP
    // uSelect(0): ��ũ��Ʈ Queue Data �� �о�´�.
    //        (1): ĸ�� Queue Data�� �о�´�.

    // uSelect QI.
    // uSelect(0): ��ũ��Ʈ Queue 1 Data �о�´�.
    //        (1): ��ũ��Ʈ Queue 2 Data �о�´�.

    [DllImport("AXL.dll")] public static extern uint AxmGetScriptCaptionQueueDataCount(int nAxisNo, ref uint updata, uint uSelect);

    // ���� ����Ÿ�� �о�´�.
    [DllImport("AXL.dll")] public static extern uint AxmGetOptimizeDriveData(int nAxisNo, double dMinVel, double dVel, double dAccel, double  dDecel, 
            ref uint wRangeData, ref uint wStartStopSpeedData, ref uint wObjectSpeedData, ref uint wAccelRate, ref uint wDecelRate);

    // ���峻�� �������͸� Byte������ ���� �� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmBoardWriteByte(int nBoardNo, uint wOffset, byte byData);
    [DllImport("AXL.dll")] public static extern uint AxmBoardReadByte(int nBoardNo, uint wOffset, ref byte byData);

    // ���峻�� �������͸� Word������ ���� �� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmBoardWriteWord(int nBoardNo, uint wOffset, uint wData);
    [DllImport("AXL.dll")] public static extern uint AxmBoardReadWord(int nBoardNo, uint wOffset, ref ushort wData);

    // ���峻�� �������͸� DWord������ ���� �� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmBoardWriteDWord(int nBoardNo, uint wOffset, uint dwData);
    [DllImport("AXL.dll")] public static extern uint AxmBoardReadDWord(int nBoardNo, uint wOffset, ref uint dwData);

    // ���峻�� ��⿡ �������͸� Byte���� �� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmModuleWriteByte(int nBoardNo, int nModulePos, uint wOffset, byte byData);
    [DllImport("AXL.dll")] public static extern uint AxmModuleReadByte(int nBoardNo, int nModulePos, uint wOffset, ref byte byData);

    // ���峻�� ��⿡ �������͸� Word���� �� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmModuleWriteWord(int nBoardNo, int nModulePos, uint wOffset, uint wData);
    [DllImport("AXL.dll")] public static extern uint AxmModuleReadWord(int nBoardNo, int nModulePos, uint wOffset, ref ushort wData);

    // ���峻�� ��⿡ �������͸� DWord���� �� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmModuleWriteDWord(int nBoardNo, int nModulePos, uint wOffset, uint dwData);
    [DllImport("AXL.dll")] public static extern uint AxmModuleReadDWord(int nBoardNo, int nModulePos, uint wOffset, ref uint dwData);
    

    // �ܺ� ��ġ �񱳱⿡ ���� �����Ѵ�.(Pos = Unit)
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetActComparatorPos(int nAxisNo, double dPos);
    // �ܺ� ��ġ �񱳱⿡ ���� ��ȯ�Ѵ�.(Positon = Unit)
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetActComparatorPos(int nAxisNo, ref double dpPos);

    // ���� ��ġ �񱳱⿡ ���� �����Ѵ�.(Pos = Unit)
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetCmdComparatorPos(int nAxisNo, double dPos);
    // ���� ��ġ �񱳱⿡ ���� ��ȯ�Ѵ�.(Pos = Unit)
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetCmdComparatorPos(int nAxisNo, ref double dpPos);
    
//========== �߰� �Լ� =========================================================================================================
    
    // ���� ���� �� �ӵ��� ������ ���Ѵ�� �����Ѵ�.
    // �ӵ� ������� �Ÿ��� �־��־�� �Ѵ�. 
    [DllImport("AXL.dll")] public static extern uint AxmLineMoveVel(int nCoord, double dVel, double dAccel, double dDecel);

//========= ���� ��ġ ���� �Լ�( �ʵ�: IP������ , QI���� ��ɾ���)==============================================================
    
    // ���� ���� Sensor ��ȣ�� ��� ���� �� ��ȣ �Է� ������ �����Ѵ�.
    // ��� ���� LOW(0), HIGH(1), UNUSED(2), USED(3)
    [DllImport("AXL.dll")] public static extern uint AxmSensorSetSignal(int nAxisNo, uint uLevel);
    // ���� ���� Sensor ��ȣ�� ��� ���� �� ��ȣ �Է� ������ ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmSensorGetSignal(int nAxisNo, ref uint upLevel);
    // ���� ���� Sensor ��ȣ�� �Է� ���¸� ��ȯ�Ѵ�
    [DllImport("AXL.dll")] public static extern uint AxmSensorReadSignal(int nAxisNo, ref uint upStatus);
    
    // ���� ���� ������ �ӵ��� �������� ���� ��ġ ����̹��� �����Ѵ�.
    // Sensor ��ȣ�� Active level�Է� ���� ��� ��ǥ�� ������ �Ÿ���ŭ ������ �����Ѵ�.
    // �޽��� ��µǴ� �������� �Լ��� �����.
    // lMethod :  0 - �Ϲ� ����, 1 - ���� ��ȣ ���� ���� ���� ����. ��ȣ ���� �� �Ϲ� ����
    //            2 - ���� ����
    [DllImport("AXL.dll")] public static extern uint AxmSensorMovePos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, int nMethod);

    // ���� ���� ������ �ӵ��� �������� ���� ��ġ ����̹��� �����Ѵ�.
    // Sensor ��ȣ�� Active level�Է� ���� ��� ��ǥ�� ������ �Ÿ���ŭ ������ �����Ѵ�.
    // �޽� ����� ����Ǵ� �������� �Լ��� �����.
    [DllImport("AXL.dll")] public static extern uint AxmSensorStartMovePos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, int nMethod);

    // �����˻� ���ེ�� ��ȭ�� ����� ��ȯ�Ѵ�.
    // *lpStepCount      : ��ϵ� Step�� ����
    // *upMainStepNumber : ��ϵ� MainStepNumber ������ �迭����Ʈ
    // *upStepNumber     : ��ϵ� StepNumber ������ �迭����Ʈ
    // *upStepBranch     : ��ϵ� Step�� Branch ������ �迭����Ʈ
    // ����: �迭������ 50���� ����
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetStepTrace(int nAxisNo, ref uint upStepCount, ref uint upMainStepNumber, ref uint upStepNumber, ref uint upStepBranch); 

//=======�߰� Ȩ ��ġ (PI-N804/404���� �ش��.)=================================================================================

    // ����ڰ� ������ ���� Ȩ���� �Ķ��Ÿ�� �����Ѵ�.(QIĨ ���� �������� �̿�).
    // uZphasCount : Ȩ �Ϸ��Ŀ� Z�� ī��Ʈ(0 - 15)
    // lHomeMode   : Ȩ ���� ���( 0 - 12)
    // lClearSet   : ��ġ Ŭ���� , �ܿ��޽� Ŭ���� ��� ���� (0 - 3)
    //               0: ��ġŬ���� ������, �ܿ��޽� Ŭ���� ��� ����
    //                 1: ��ġŬ���� �����, �ܿ��޽� Ŭ���� ��� ����
    //               2: ��ġŬ���� ������, �ܿ��޽� Ŭ���� �����
    //               3: ��ġŬ���� �����, �ܿ��޽� Ŭ���� �����.
    // dOrgVel : Ȩ���� Org  Speed ���� 
    // dLastVel: Ȩ���� Last Speed ���� 
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetConfig(int nAxisNo, uint uZphasCount, int nHomeMode, int nClearSet, double dOrgVel, double dLastVel, double dLeavePos);
    // ����ڰ� ������ ���� Ȩ���� �Ķ��Ÿ�� ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetConfig(int nAxisNo, ref uint upZphasCount, ref int npHomeMode, ref int npClearSet, ref double dpOrgVel, ref double dpLastVel, ref double dpLeavePos); //KKJ(070215)
    
    // ����ڰ� ������ ���� Ȩ ��ġ�� �����Ѵ�.
    // lHomeMode ���� ���� : 0 - 5 ���� (Move Return�Ŀ� Search��  �����Ѵ�.)
    // lHomeMode -1�� �״�� ���� HomeConfig���� ����Ѵ�� �״�� ������.
    // ��������      : Vel���� ����̸� CW, �����̸� CCW.
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetMoveSearch(int nAxisNo, double dVel, double dAccel, double dDecel);

    // ����ڰ� ������ ���� Ȩ ������ �����Ѵ�.
    // lHomeMode ���� ���� : 0 - 12 ���� 
    // lHomeMode -1�� �״�� ���� HomeConfig���� ����Ѵ�� �״�� ������.
    // ��������      : Vel���� ����̸� CW, �����̸� CCW.
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetMoveReturn(int nAxisNo, double dVel, double dAccel, double dDecel);
    
    // ����ڰ� ������ ���� Ȩ ��Ż�� �����Ѵ�.
    // ��������      : Vel���� ����̸� CW, �����̸� CCW.
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetMoveLeave(int nAxisNo, double dVel, double dAccel, double dDecel);

    // ����ڰ� ������ ������ Ȩ ��ġ�� �����Ѵ�.
    // lHomeMode ���� ���� : 0 - 5 ���� (Move Return�Ŀ� Search��  �����Ѵ�.)
    // lHomeMode -1�� �״�� ���� HomeConfig���� ����Ѵ�� �״�� ������.
    // ��������      : Vel���� ����̸� CW, �����̸� CCW.
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetMultiMoveSearch(int nArraySize, ref int npAxesNo, ref double dpVel, ref double dpAccel, ref double dpDecel);

    //������ ��ǥ���� ���� �ӵ� �������� ��带 �����Ѵ�.
    // (������ : �ݵ�� ����� �ϰ� ��밡��)
    // ProfileMode : '0' - ��Ī Trapezode
    //               '1' - ���Ī Trapezode
    //               '2' - ��Ī Quasi-S Curve
    //               '3' - ��Ī S Curve
    //               '4' - ���Ī S Curve
    [DllImport("AXL.dll")] public static extern uint AxmContiSetProfileMode(int nCoord, uint uProfileMode);
    // ������ ��ǥ���� ���� �ӵ� �������� ��带 ��ȯ�Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmContiGetProfileMode(int nCoord, ref uint upProfileMode);

    //========== DIO ���ͷ�Ʈ �÷��� ������Ʈ �б�
    // ������ �Է� ���� ���, Interrupt Flag Register�� Offset ��ġ���� bit ������ ���ͷ�Ʈ �߻� ���� ���� ����
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptFlagReadBit(int nModuleNo, int nOffset, ref uint upValue);
    // ������ �Է� ���� ���, Interrupt Flag Register�� Offset ��ġ���� byte ������ ���ͷ�Ʈ �߻� ���� ���� ����
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptFlagReadByte(int nModuleNo, int nOffset, ref uint upValue);
    // ������ �Է� ���� ���, Interrupt Flag Register�� Offset ��ġ���� word ������ ���ͷ�Ʈ �߻� ���� ���� ����
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptFlagReadWord(int nModuleNo, int nOffset, ref uint upValue);
    // ������ �Է� ���� ���, Interrupt Flag Register�� Offset ��ġ���� double word ������ ���ͷ�Ʈ �߻� ���� ���� ����
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptFlagReadDword(int nModuleNo, int nOffset, ref uint upValue);
    // ��ü �Է� ���� ���, Interrupt Flag Register�� Offset ��ġ���� bit ������ ���ͷ�Ʈ �߻� ���� ���� ����
    [DllImport("AXL.dll")] public static extern uint AxdiInterruptFlagRead(int nOffset, ref uint upValue);

    //========= �α� ���� �Լ� ==========================================================================================
    // ���� �ڵ����� ������.
    // ���� ���� �Լ� ���� ����� EzSpy���� ����͸� �� �� �ֵ��� ���� �Ǵ� �����ϴ� �Լ��̴�.
    // uUse : ��� ���� => DISABLE(0), ENABLE(1)
    [DllImport("AXL.dll")] public static extern uint AxmLogSetAxis(int nAxisNo, uint uUse);
    
    // EzSpy������ ���� �� �Լ� ���� ��� ����͸� ���θ� Ȯ���ϴ� �Լ��̴�.
    [DllImport("AXL.dll")] public static extern uint AxmLogGetAxis(int nAxisNo, ref uint upUse);

//=========== �α� ��� ���� �Լ�
    //������ �Է� ä���� EzSpy�� �α� ��� ���θ� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxaiLogSetChannel(int nChannelNo, uint uUse);
    //������ �Է� ä���� EzSpy�� �α� ��� ���θ� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxaiLogGetChannel(int nChannelNo, ref uint upUse);

//==������ ��� ä���� EzSpy �α� ��� 
    //������ ��� ä���� EzSpy�� �α� ��� ���θ� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxaoLogSetChannel(int nChannelNo, uint uUse);
    //������ ��� ä���� EzSpy�� �α� ��� ���θ� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxaoLogGetChannel(int nChannelNo, ref uint upUse);

//==Log
    // ������ ����� EzSpy�� �α� ��� ���� ����
    [DllImport("AXL.dll")] public static extern uint AxdLogSetModule(int nModuleNo, uint uUse);
    // ������ ����� EzSpy�� �α� ��� ���� Ȯ��
    [DllImport("AXL.dll")] public static extern uint AxdLogGetModule(int nModuleNo, ref uint upUse);

    // ������ ���尡 RTEX ����� �� �� ������ firmware ������ Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlGetFirmwareVersion(int nBoardNo, ref byte szVersion);
    // ������ ����� Firmware�� ���� �Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlSetFirmwareCopy(int nBoardNo, ref ushort wData, ref ushort wCmdData);
    // ������ ����� Firmware Update�� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlSetFirmwareUpdate(int nBoardNo);
    // ������ ������ ���� RTEX �ʱ�ȭ ���¸� Ȯ�� �Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlCheckStatus(int nBoardNo, ref uint dwStatus);
    // ������ �࿡ RTEX Master board�� ���� ����� ���� �մϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlRtexUniversalCmd(int nBoardNo, ushort wCmd, ushort wOffset, ref ushort wData);
    // ������ ���� RTEX ��� ����� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmRtexSlaveCmd(int nAxisNo, uint dwCmdCode, uint dwTypeCode, uint dwIndexCode, uint dwCmdConfigure, uint dwValue);
    // ������ �࿡ ������ RTEX ��� ����� ������� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmRtexGetSlaveCmdResult(int nAxisNo, ref uint dwIndex, ref uint dwValue);
    // ������ �࿡ ������ RTEX ��� ����� ������� Ȯ���Ѵ�. PCIE-Rxx04-RTEX ����
    [DllImport("AXL.dll")] public static extern uint AxmRtexGetSlaveCmdResultEx(int nAxisNo, ref uint dwpCommand, ref uint dwpType, ref uint dwpIndex, ref uint dwpValue);
    // ������ �࿡ RTEX ���� ������ Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmRtexGetAxisStatus(int nAxisNo, ref uint dwStatus);
    // ������ �࿡ RTEX ��� ���� ������ Ȯ���Ѵ�.(Actual position, Velocity, Torque)
    [DllImport("AXL.dll")] public static extern uint AxmRtexGetAxisReturnData(int nAxisNo,  ref uint dwReturn1, ref uint dwReturn2, ref uint dwReturn3);
    // ������ �࿡ RTEX Slave ���� ���� ���� ������ Ȯ���Ѵ�.(mechanical, Inposition and etc)
    [DllImport("AXL.dll")] public static extern uint AxmRtexGetAxisSlaveStatus(int nAxisNo,  ref uint dwStatus);
    // ������ �࿡ MLII Slave �࿡ ���� ��Ʈ�� ��ɾ �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmSetAxisCmd(int nAxisNo, ref uint tagCommand);
    // ������ �࿡ MLII Slave �࿡ ���� ��Ʈ�� ����� ����� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmGetAxisCmdResult(int nAxisNo, ref uint tagCommand);
    
    [DllImport("AXL.dll")] public static extern uint AxlGetDpRamData(int nBoardNo, ushort uAddress, ref uint upRdData);
    // DPRAM �����͸� Word������ �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlBoardWriteDpramWord(int nBoardNo, ushort uOffset, uint uWrData);
    [DllImport("AXL.dll")] public static extern uint AxlBoardReadDpramWord(int nBoardNo, ushort uOffset, ref uint upRdData);
    
    // �� ������ �� SLAVE���� ����� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlSetSendBoardEachCommand(int nBoardNo, uint uCommand, ref uint upSendData, uint uLength);

    [DllImport("AXL.dll")] public static extern uint AxlSetSendBoardCommand(int nBoardNo, uint uCommand, ref uint upSendData, uint uLength);
    [DllImport("AXL.dll")] public static extern uint AxlGetResponseBoardCommand(int nBoardNo, ref uint upReadData);

    // Network Type Master ���忡�� Slave ���� Firmware Version�� �о� ���� �Լ�.
    // ucaFirmwareVersion byte ���� Array�� �����ϰ� ũ�Ⱑ 4�̻��� �ǵ��� ���� �ؾ� �Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmInfoGetFirmwareVersion(int nAxisNo, ref byte ucaFirmwareVersion);
    [DllImport("AXL.dll")] public static extern uint AxaInfoGetFirmwareVersion(int nModuleNo, ref byte ucaFirmwareVersion);
    [DllImport("AXL.dll")] public static extern uint AxdInfoGetFirmwareVersion(int nModuleNo, ref byte ucaFirmwareVersion);
    [DllImport("AXL.dll")] public static extern uint AxcInfoGetFirmwareVersion(int nModuleNo, ref byte ucaFirmwareVersion);

//======== PCI-R1604-MLII ���� �Լ�=========================================================================== 
    // INTERPOLATE and LATCH Command�� Option Field�� Torq Feed Forward�� ���� ���� �ϵ��� �մϴ�.
    // �⺻���� MAX�� �����Ǿ� �ֽ��ϴ�.
    // �������� 0 ~ 4000H���� ���� �� �� �ֽ��ϴ�.
    // �������� 4000H�̻����� �����ϸ� ������ �� �̻����� �����ǳ� ������ 4000H���� ���� �˴ϴ�.
    [DllImport("AXL.dll")] public static extern uint AxmSetTorqFeedForward(int nAxisNo, uint uTorqFeedForward);
 
    // INTERPOLATE and LATCH Command�� Option Field�� Torq Feed Forward�� ���� �о���� �Լ� �Դϴ�.
    // �⺻���� MAX�� �����Ǿ� �ֽ��ϴ�.
    [DllImport("AXL.dll")] public static extern uint AxmGetTorqFeedForward(int nAxisNo, ref uint upTorqFeedForward);
 
    // INTERPOLATE and LATCH Command�� VFF Field�� Velocity Feed Forward�� ���� ���� �ϵ��� �մϴ�.
    // �⺻���� '0'�� �����Ǿ� �ֽ��ϴ�.
    // �������� 0 ~ FFFFH���� ���� �� �� �ֽ��ϴ�.
    [DllImport("AXL.dll")] public static extern uint AxmSetVelocityFeedForward(int nAxisNo, uint uVelocityFeedForward);
 
    // INTERPOLATE and LATCH Command�� VFF Field�� Velocity Feed Forward�� ���� �о���� �Լ� �Դϴ�.
    [DllImport("AXL.dll")] public static extern uint AxmGetVelocityFeedForward(int nAxisNo, ref uint upVelocityFeedForward);

    // Encoder type�� �����Ѵ�.
    // �⺻���� 0(TYPE_INCREMENTAL)�� �����Ǿ� �ֽ��ϴ�.
    // �������� 0 ~ 1���� ���� �� �� �ֽ��ϴ�.
    // ������ : 0(TYPE_INCREMENTAL), 1(TYPE_ABSOLUTE).
    [DllImport("AXL.dll")] public static extern uint AxmSignalSetEncoderType(int nAxisNo, uint uEncoderType);

    // Encoder type�� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmSignalGetEncoderType(int nAxisNo, ref uint upEncoderType);

    // Slave Firmware Update�� ���� �߰�
    //[DllImport("AXL.dll")] public static extern uint AxmSetSendAxisCommand(long lAxisNo, WORD wCommand, WORD* wpSendData, WORD wLength);

    //======== PCI-R1604-RTEX, RTEX-PM ���� �Լ�============================================================== 
    // ���� �Է� 2,3�� �Է½� JOG ���� �ӵ��� �����Ѵ�. 
    // ������ ���õ� ��� ����(Ex, PulseOutMethod, MoveUnitPerPulse ��)���� �Ϸ�� ���� �ѹ��� �����Ͽ��� �Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmMotSetUserMotion(int nAxisNo, double dVelocity, double dAccel, double dDecel);

    // ���� �Է� 2,3�� �Է½� JOG ���� ���� ��� ���θ� �����Ѵ�.
    // ������ :  0(DISABLE), 1(ENABLE)
    [DllImport("AXL.dll")] public static extern uint AxmMotSetUserMotionUsage(int nAxisNo, uint dwUsage);

    // MPGP �Է��� ����Ͽ� Load/UnLoad ��ġ�� �ڵ����� �̵��ϴ� ��� ����. 
    [DllImport("AXL.dll")] public static extern uint AxmMotSetUserPosMotion(int nAxisNo, double dVelocity, double dAccel, double dDecel, double dLoadPos, double dUnLoadPos, uint dwFilter, uint dwDelay);

    // MPGP �Է��� ����Ͽ� Load/UnLoad ��ġ�� �ڵ����� �̵��ϴ� ��� ����. 
    // ������ :  0(DISABLE), 1(Position ��� A ���), 2(Position ��� B ���)
    [DllImport("AXL.dll")] public static extern uint AxmMotSetUserPosMotionUsage(int nAxisNo, uint dwUsage);
    //======================================================================================================== 

    //======== SIO-CN2CH, ���� ��ġ Ʈ���� ��� ��� ���� �Լ�================================================ 
    // �޸� ������ ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxcKeWriteRamDataAddr(int nChannelNo, uint dwAddr, uint dwData);
    // �޸� ������ �б� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxcKeReadRamDataAddr(int nChannelNo, uint dwAddr, ref uint dwpData);
    // �޸� �ʱ�ȭ �Լ�
    [DllImport("AXL.dll")] public static extern uint AxcKeResetRamDataAll(int nModuleNo, uint dwData);
    // Ʈ���� Ÿ�� �ƿ� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxcTriggerSetTimeout(int nChannelNo, uint dwTimeout);
    // Ʈ���� Ÿ�� �ƿ� Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxcTriggerGetTimeout(int nChannelNo, ref uint dwpTimeout);
    // Ʈ���� ��� ���� Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxcStatusGetWaitState(int nChannelNo, ref uint dwpState);
    // Ʈ���� ��� ���� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxcStatusSetWaitState(int nChannelNo, uint dwState);
    
    //���� ä�ο� ��ɾ� ����
    [DllImport("AXL.dll")] public static extern uint AxcKeSetCommandData32(int nChannelNo, uint dwCommand, uint dwData);
    
    //���� ä�ο� ��ɾ� ����
    [DllImport("AXL.dll")] public static extern uint AxcKeSetCommandData16(int nChannelNo, uint dwCommand, uint wData); 
    
    //���� ä���� �������� Ȯ��
    [DllImport("AXL.dll")] public static extern uint AxcKeGetCommandData32(int nChannelNo, uint dwCommand, ref uint dwpData);
    
    //���� ä���� �������� Ȯ��
    [DllImport("AXL.dll")] public static extern uint AxcKeGetCommandData16(int nChannelNo, uint dwCommand, ref uint wpData); 
    
    //======================================================================================================== 
    
    //======== PCI-N804/N404 ����, Sequence Motion ===================================================================
    // Sequence Motion�� �� ������ ���� �մϴ�. (�ּ� 1��)
    // lSeqMapNo : �� ��ȣ ������ ��� Sequence Motion Index Point
    // lSeqMapSize : �� ��ȣ ����
    // long* LSeqAxesNo : �� ��ȣ �迭
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqSetAxisMap(int nSeqMapNo, int nSeqMapSize, ref int nSeqAxesNo);
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqGetAxisMap(int nSeqMapNo, ref int nSeqMapSize, ref int nSeqAxesNo);

    // Sequence Motion�� ����(Master) ���� ���� �մϴ�. 
    // �ݵ�� AxmSeqSetAxisMap(...) �� ������ �� ������ �����Ͽ��� �մϴ�.
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqSetMasterAxisNo(int nSeqMapNo, int nMasterAxisNo);

    // Sequence Motion�� Node ���� ������ ���̺귯���� �˸��ϴ�.
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqBeginNode(int nSeqMapNo);

    // Sequence Motion�� Node ���� ���Ḧ ���̺귯���� �˸��ϴ�.
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqEndNode(int nSeqMapNo);

    // Sequence Motion�� ������ ���� �մϴ�.
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqStart(int nSeqMapNo, ref uint dwStartOption);

    // Sequence Motion�� �� Profile Node ������ ���̺귯���� �Է� �մϴ�.
    // ���� 1�� Sequence Motion�� ����ϴ���, *dPosition�� 1���� Array�� �����Ͽ� �ֽñ� �ٶ��ϴ�.
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqAddNode(int nSeqMapNo, ref double dPosition, double dVelocity, double dAcceleration, double dDeceleration, double dNextVelocity);

    // Sequence Motion�� ���� �� ���� ���� ���� Node Index�� �˷� �ݴϴ�.
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqGetNodeNum(int nSeqMapNo, ref int nCurNodeNo);

    // Sequence Motion�� �� Node Count�� Ȯ�� �մϴ�.
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqGetTotalNodeNum(int nSeqMapNo, ref int nTotalNodeCnt);

    // Sequence Motion�� ���� ���� ������ Ȯ�� �մϴ�.
    // dwInMotion : 0(���� ����), 1(���� ��)
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqIsMotion(int nSeqMapNo, ref uint dwInMotion);

    // Sequence Motion�� Memory�� Clear �մϴ�.
    // AxmSeqSetAxisMap(...), AxmSeqSetMasterAxisNo(...) ���� ������ ���� �����˴ϴ�.
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqWriteClear(int nSeqMapNo);

    // Sequence Motion�� ������ ���� �մϴ�.
    // dwStopMode : 0(EMERGENCY_STOP), 1(SLOWDOWN_STOP) 
    
    [DllImport("AXL.dll")] public static extern uint AxmSeqStop(int nSeqMapNo, uint dwStopMode);
    //======================================================================================================== 
    
    [DllImport("AXL.dll")] public static extern uint AxmMoveStartPosWithAVC(int nAxisNo, double dPos, double dMaxVel, double dMaxAccel, double dMinJerk);
    //======== PCIe-Rxx04-SIIIH ���� �Լ�==========================================================================
    // (SIIIH, MR_J4_xxB, Para : 0 ~ 8) ==
    //     [0] : Command Position
    //     [1] : Actual Position
    //     [2] : Actual Velocity
    //     [3] : Mechanical Signal
    //     [4] : Regeneration load factor(%)
    //     [5] : Effective load factor(%)
    //     [6] : Peak load factor(%)
    //     [7] : Current Feedback
    //     [8] : Command Velocity
    [DllImport("AXL.dll")] public static extern uint AxmStatusSetMon(int nAxisNo, uint dwParaNo1, uint dwParaNo2, uint dwParaNo3, uint dwParaNo4, uint dwUse);
    [DllImport("AXL.dll")] public static extern uint AxmStatusGetMon(int nAxisNo, ref uint dwpParaNo1, ref uint dwpParaNo2, ref uint dwpParaNo3, ref uint dwpParaNo4, ref uint dwpUse);
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadMon(int nAxisNo, ref uint dwpParaNo1, ref uint dwpParaNo2, ref uint dwpParaNo3, ref uint dwpParaNo4, ref uint dwDataValid);
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadMonEx(int nAxisNo, ref int npDataCnt, ref uint dwpReadData);

    //======== PCI-R32IOEV-RTEX ���� �Լ�=========================================================================== 
    // I/O ��Ʈ�� �Ҵ�� HPI register �� �а� �������� API �Լ�.
    // I/O Registers for HOST interface.
    // I/O 00h Host status register (HSR)
    // I/O 04h Host-to-DSP control register (HDCR)
    // I/O 08h DSP page register (DSPP)
    // I/O 0Ch Reserved
    [DllImport("AXL.dll")] public static extern uint AxlSetIoPort(int nBoardNo, uint dwAddr, uint dwData);

    [DllImport("AXL.dll")] public static extern uint AxlGetIoPort(int nBoardNo, uint dwAddr, ref uint dwpData);                    

    //======== PCI-R3200-MLIII ���� �Լ�=========================================================================== 
    /*
        // M-III Master ���� �߿��� ������Ʈ �⺻ ���� ���� �Լ�
        DWORD   __stdcall AxlM3SetFWUpdateInit(long lBoardNo, DWORD dwTotalPacketSize);
        // M-III Master ���� �߿��� ������Ʈ �⺻ ���� ���� ��� Ȯ�� �Լ�
        DWORD   __stdcall AxlM3GetFWUpdateInit(long lBoardNo, DWORD *dwTotalPacketSize);
        // M-III Master ���� �߿��� ������Ʈ �ڷ� ���� �Լ�
        DWORD   __stdcall AxlM3SetFWUpdateCopy(long lBoardNo, DWORD *lFWUpdataData, DWORD dwLength);
        // M-III Master ���� �߿��� ������Ʈ �ڷ� ���� ��� Ȯ�� �Լ�
        DWORD   __stdcall AxlM3GetFWUpdateCopy(long lBoardNo, BYTE bCrcData, DWORD *lFWUpdataResult);
        // M-III Master ���� �߿��� ������Ʈ ����
        DWORD   __stdcall AxlM3SetFWUpdate(long lBoardNo, DWORD dwSectorNo);
        // M-III Master ���� �߿��� ������Ʈ ���� ��� Ȯ��
        DWORD   __stdcall AxlM3GetFWUpdate(long lBoardNo, DWORD *dwSectorNo, DWORD *dwIsDone);
    */

    // M-III Master ���� �߿��� ������Ʈ �⺻ ���� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetFWUpdateInit(int nBoardNo, uint dwTotalPacketSize, uint dwProcTotalStepNo);

    // M-III Master ���� �߿��� ������Ʈ �⺻ ���� ���� ��� Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetFWUpdateInit(int nBoardNo, ref uint dwTotalPacketSize, ref uint dwProcTotalStepNo);

    // M-III Master ���� �߿��� ������Ʈ �ڷ� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetFWUpdateCopy(int nBoardNo, ref uint pdwPacketData, uint dwPacketSize);

    // M-III Master ���� �߿��� ������Ʈ �ڷ� ���� ��� Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetFWUpdateCopy(int nBoardNo, ref uint dwPacketSize);

    // M-III Master ���� �߿��� ������Ʈ ����
    [DllImport("AXL.dll")] public static extern uint AxlM3SetFWUpdate(int nBoardNo, uint dwFlashBurnStepNo);

    // M-III Master ���� �߿��� ������Ʈ ���� ��� Ȯ��
    [DllImport("AXL.dll")] public static extern uint AxlM3GetFWUpdate(int nBoardNo, ref uint dwFlashBurnStepNo, ref uint dwIsFlashBurnDone);
    
    //M-III Master ���� EEPROM ������ ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetCFGData(int nBoardNo, ref uint pCmdData, uint CmdDataSize);
    
    // M-III Master ���� EEPROM ������ �������� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetCFGData(int nBoardNo, ref uint pCmdData, uint CmdDataSize);

    // M-III Master ���� CONNECT PARAMETER �⺻ ���� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetMCParaUpdateInit(int nBoardNo, ushort wCh0Slaves, ushort wCh1Slaves, uint dwCh0CycTime, uint dwCh1CycTime, uint dwChInfoMaxRetry);

    // M-III Master ���� CONNECT PARAMETER �⺻ ���� ���� ��� Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetMCParaUpdateInit(int nBoardNo, ref ushort wCh0Slaves, ref ushort wCh1Slaves, ref uint dwCh0CycTime, ref uint dwCh1CycTime, ref uint dwChInfoMaxRetry);

    // M-III Master ���� CONNECT PARAMETER �⺻ ���� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetMCParaUpdateCopy(int nBoardNo, ushort wIdx, ushort wChannel, ushort wSlaveAddr, uint dwProtoCalType, uint dwTransBytes, uint dwDeviceCode);

    // M-III Master ���� CONNECT PARAMETER �⺻ ���� ���� ��� Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetMCParaUpdateCopy(int nBoardNo, ushort wIdx, ref ushort wChannel, ref ushort wSlaveAddr, ref uint dwProtoCalType, ref uint dwTransBytes, ref uint dwDeviceCode);

    // M-III Master ���峻�� �������͸� DWord������ Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlBoardReadDWord(int nBoardNo, ushort wOffset, ref uint dwData);

    // M-III Master ���峻�� �������͸� DWord������ ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlBoardWriteDWord(int nBoardNo, ushort wOffset, uint dwData);
    
    // ���峻�� Ȯ�� �������͸� DWord������ ���� �� Ȯ���Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlBoardReadDWordEx(int nBoardNo, uint dwOffset, ref uint dwData);
    [DllImport("AXL.dll")] public static extern uint AxlBoardWriteDWordEx(int nBoardNo, uint dwOffset, uint dwData);

    // ������ ���� ���� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetCtrlStopMode(int nAxisNo, byte bStopMode);

    // ������ Lt ���� ���·� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetCtrlLtSel(int nAxisNo, byte bLtSel1, byte bLtSel2);

    // ������ IO �Է� ���¸� Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmStatusReadServoCmdIOInput(int nAxisNo, ref uint upStatus);

    // ������ ���� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoExInterpolate(int nAxisNo, uint dwTPOS, uint dwVFF, uint dwTFF, uint dwTLIM, uint dwExSig1, uint dwExSig2);

    // ���� �������� ���̾ ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetExpoAccBias(int nAxisNo, ushort wBias);

    // ���� �������� �ð� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetExpoAccTime(int nAxisNo, ushort wTime);

    // ������ �̵� �ð��� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetMoveAvrTime(int nAxisNo, ushort wTime);

    // ������ Acc ���� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetAccFilter(int nAxisNo, byte bAccFil);

    // ������ ���� �����1 ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetCprmMonitor1(int nAxisNo, byte bMonSel);

    // ������ ���� �����2 ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetCprmMonitor2(int nAxisNo, byte bMonSel);

    // ������ ���� �����1 Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoStatusReadCprmMonitor1(int nAxisNo, ref uint upStatus);

    // ������ ���� �����2 Ȯ�� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoStatusReadCprmMonitor2(int nAxisNo, ref uint upStatus);

    // ���� �������� Dec ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetAccDec(int nAxisNo, ushort wAcc1, ushort wAcc2, ushort wAccSW, ushort wDec1, ushort wDec2, ushort wDecSW);

    // ���� ���� ���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxmM3ServoSetStop(int nAxisNo, int nMaxDecel);

    //========== ǥ�� I/O ��� ���� Ŀ�ǵ� =========================================================================

    // Network��ǰ �� �����̺� ����� �Ķ���� ���� ���� ��ȯ�ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetStationParameter(int nBoardNo, int nModuleNo, ushort wNo, byte bSize, byte bModuleType, ref byte pbParam);

    // Network��ǰ �� �����̺� ����� �Ķ���� ���� �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationParameter(int nBoardNo, int nModuleNo, ushort wNo, byte bSize, byte bModuleType, ref byte pbParam);

    // Network��ǰ �� �����̺� ����� ID���� ��ȯ�ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetStationIdRd(int nBoardNo, int nModuleNo, byte bIdCode, byte bOffset, byte bSize, byte bModuleType, ref byte pbParam);

    // Network��ǰ �� �����̺� ����� ��ȿ Ŀ�ǵ�� ����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationNop(int nBoardNo, int nModuleNo, byte bModuleType);

    // Network��ǰ �� �����̺� ����� �¾��� �ǽ��ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationConfig(int nBoardNo, int nModuleNo, byte bConfigMode, byte bModuleType);

    // Network��ǰ �� �����̺� ����� �˶� �� ��� ���� ���� ��ȯ�ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetStationAlarm(int nBoardNo, int nModuleNo, ushort wAlarmRdMod, ushort wAlarmIndex, byte bModuleType, ref ushort pwAlarmData);

    // Network��ǰ �� �����̺� ����� �˶� �� ��� ���¸� �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationAlarmClear(int nBoardNo, int nModuleNo, ushort wAlarmClrMod, byte bModuleType);

    // Network��ǰ �� �����̺� ������ ��������� �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationSyncSet(int nBoardNo, int nModuleNo, byte bModuleType);

    // Network��ǰ �� �����̺� ������ ������ �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationConnect(int nBoardNo, int nModuleNo, byte bVer, byte bComMode, byte bComTime, byte bProfileType, byte bModuleType);

    // Network��ǰ �� �����̺� ������ ���� ������ �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationDisConnect(int nBoardNo, int nModuleNo, byte bModuleType);

    // Network��ǰ �� �����̺� ����� ���ֹ߼� �Ķ���� ���� ���� ��ȯ�ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetStationStoredParameter(int nBoardNo, int nModuleNo, ushort wNo, byte bSize, byte bModuleType, ref byte pbParam);

    // Network��ǰ �� �����̺� ����� ���ֹ߼� �Ķ���� ���� �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationStoredParameter(int nBoardNo, int nModuleNo, ushort wNo, byte bSize, byte bModuleType, ref byte pbParam);

    // Network��ǰ �� �����̺� ����� �޸� ���� ���� ��ȯ�ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetStationMemory(int nBoardNo, int nModuleNo, ushort wSize, uint dwAddress, byte bModuleType, byte bMode, byte bDataType, ref byte pbData);

    // Network��ǰ �� �����̺� ����� �޸� ���� �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationMemory(int nBoardNo, int nModuleNo, ushort wSize, uint dwAddress, byte bModuleType, byte bMode, byte bDataType, ref byte pbData);

    //========== ǥ�� I/O ��� Ŀ�ؼ� Ŀ�ǵ� =========================================================================

    // Network��ǰ �� �������� �����̺� ����� �ڵ� �＼�� ��� ���� �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationAccessMode(int nBoardNo, int nModuleNo, byte bModuleType, byte bRWSMode);

    // Network��ǰ �� �������� �����̺� ����� �ڵ� �＼�� ��� �������� ��ȯ�ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetStationAccessMode(int nBoardNo, int nModuleNo, byte bModuleType, ref byte bRWSMode);

    // Network��ǰ �� �����̺� ����� ���� �ڵ� ���� ��带 �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetAutoSyncConnectMode(int nBoardNo, int nModuleNo, byte bModuleType, uint dwAutoSyncConnectMode);

    // Network��ǰ �� �����̺� ����� ���� �ڵ� ���� ��� �������� ��ȯ�ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetAutoSyncConnectMode(int nBoardNo, int nModuleNo, byte bModuleType, ref uint dwpAutoSyncConnectMode);

    // Network��ǰ �� �����̺� ��⿡ ���� ���� ����ȭ ������ �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SyncConnectSingle(int nBoardNo, int nModuleNo, byte bModuleType);

    // Network��ǰ �� �����̺� ��⿡ ���� ���� ����ȭ ���� ������ �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SyncDisconnectSingle(int nBoardNo, int nModuleNo, byte bModuleType);

    // Network��ǰ �� �����̺� ������ ���� ���¸� Ȯ���ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3IsOnLine(int nBoardNo, int nModuleNo, ref uint dwData);

    //========== ǥ�� I/O �������� Ŀ�ǵ� =========================================================================

    // Network��ǰ �� ����ȭ ������ �����̺� I/O ��⿡ ���� ������ �������� ��ȯ�ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetStationRWS(int nBoardNo, int nModuleNo, byte bModuleType, ref uint pdwParam, byte bSize);

    // Network��ǰ �� ����ȭ ������ �����̺� I/O ��⿡ ���� �����Ͱ��� �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationRWS(int nBoardNo, int nModuleNo, byte bModuleType, ref uint pdwParam, byte bSize);

    // Network��ǰ �� �񵿱�ȭ ������ �����̺� I/O ��⿡ ���� ������ �������� ��ȯ�ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3GetStationRWA(int nBoardNo, int nModuleNo, byte bModuleType, ref uint pdwParam, byte bSize);

    // Network��ǰ �� �񵿱�ȭ ������ �����̺� I/O ��⿡ ���� �����Ͱ��� �����ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlM3SetStationRWA(int nBoardNo, int nModuleNo, byte bModuleType, ref uint pdwParam, byte bSize);

    // MLIII adjustment operation�� ���� �Ѵ�.
    // dwReqCode == 0x1005 : parameter initialization : 20sec
    // dwReqCode == 0x1008 : absolute encoder reset   : 5sec
    // dwReqCode == 0x100E : automatic offset adjustment of motor current detection signals  : 5sec
    // dwReqCode == 0x1013 : Multiturn limit setting  : 5sec
    [DllImport("AXL.dll")] public static extern uint AxmM3AdjustmentOperation(int nAxisNo, uint dwReqCode);

    // M3 ���� ���� �˻� ���� ���� ���ܿ� �Լ��̴�.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetM3FWRealRate(int nAxisNo, ref uint upHomeMainStepNumber, ref uint upHomeSubStepNumber, ref uint upHomeLastMainStepNumber, ref uint upHomeLastSubStepNumber);

    // M3 ���� ���� �˻��� ���������� Ż��� �����Ǵ� ��ġ ���� ��ȯ�ϴ� �Լ��̴�.
    [DllImport("AXL.dll")] public static extern uint AxmHomeGetM3OffsetAvoideSenArea(int nAxisNo, ref double dPos);

    // M3 ���� ���� �˻��� ���������� Ż��� �����Ǵ� ��ġ ���� �����ϴ� �Լ��̴�.
    // dPos ���� ���� 0�̸� �ڵ����� Ż��� �����Ǵ� ��ġ ���� �ڵ����� �����ȴ�.
    // dPos ���� ���� ����� ���� �Է��Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxmHomeSetM3OffsetAvoideSenArea(int nAxisNo, double dPos);
    
    // M3 ����, ����ġ ���ڴ� ��� ����, �����˻� �Ϸ� �� CMD/ACT POS �ʱ�ȭ ���� ����
    // dwSel: 0, ���� �˻��� CMD/ACTPOS 0���� ������.[�ʱⰪ]
    // dwSel: 1, ���� �˻��� CMD/ACTPOS ���� �������� ����.
    [DllImport("AXL.dll")] public static extern uint AxmM3SetAbsEncOrgResetDisable(int nAxisNo, uint dwSel);
    
    // M3 ����, ����ġ ���ڴ� ��� ����, �����˻� �Ϸ� �� CMD/ACT POS �ʱ�ȭ ���� ������ ��������
    // upSel: 0, ���� �˻��� CMD/ACTPOS 0���� ������.[�ʱⰪ]
    // upSel: 1, ���� �˻��� CMD/ACTPOS ���� �������� ����.
    [DllImport("AXL.dll")] public static extern uint AxmM3GetAbsEncOrgResetDisable(int nAxisNo, ref uint upSel);
    
    // M3 ����, �����̺� OFFLINE ��ȯ�� �˶� ���� ��� ��� ���� ����
    // dwSel: 0, ML3 �����̺� ONLINE->OFFLINE �˶� ó�� ������� ����.[�ʱⰪ]
    // dwSel: 1, ML3 �����̺� ONLINE->OFFLINE �˶� ó�� ���
    [DllImport("AXL.dll")] public static extern uint AxmM3SetOfflineAlarmEnable(int nAxisNo, uint dwSel);
    
    // M3 ����, �����̺� OFFLINE ��ȯ�� �˶� ���� ��� ��� ���� ���� �� ��������
    // upSel: 0, ML3 �����̺� ONLINE->OFFLINE �˶� ó�� ������� ����.[�ʱⰪ]
    // upSel: 1, ML3 �����̺� ONLINE->OFFLINE �˶� ó�� ���
    [DllImport("AXL.dll")] public static extern uint AxmM3GetOfflineAlarmEnable(int nAxisNo, ref uint upSel);
    
    // M3 ����, �����̺� OFFLINE ��ȯ ���� ���� �� ��������
    // upSel: 0, ML3 �����̺� ONLINE->OFFLINE ��ȯ���� ����
    // upSel: 1, ML3 �����̺� ONLINE->OFFLINE ��ȯ�Ǿ���.
    [DllImport("AXL.dll")] public static extern uint AxmM3ReadOnlineToOfflineStatus(int nAxisNo, ref uint upStatus);

    //======== EtherCAT ���� �Լ�=============================================================================
    // StationAddress�� �̿��Ͽ� EtherCAT Slave ��ǰ�� VendorID, ProductCode, RevisionNo�� �о���� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlECatGetProductInfo(uint dwStationAddress, ref uint upVendorID, ref uint upProductCode, ref uint upRevisionNo);

    // StationAddress�� �̿��Ͽ� EtherCAT Slave ��ǰ�� Network Status�� Ȯ���ϴ� �Լ�
    [DllImport("AXL.dll")] public static extern uint AxlECatGetModuleStatus(uint dwStationAddress);

    //Input PDO(Process Data Objects)�� �о�´�
    // dwBitOffset => ProcessImage inputs bit offset ��
    // dwDataBitLength => �о�� input pdo data�� bit ũ��
    // pbyData => ���� �����͸� ���� buffer
    [DllImport("AXL.dll")] public static extern uint AxlECatReadPdoInput(uint dwBitOffset, uint dwDataBitLength, ref byte bpData);
    
    //Output PDO(Process Data Objects)�� �о�´�
    // dwBitOffset => ProcessImage outputs bit offset ��
    // dwDataBitLength => �о�� input pdo data�� bit ũ��
    // pbyData => ���� �����͸� ���� buffer
    [DllImport("AXL.dll")] public static extern uint AxlECatReadPdoOutput(uint dwBitOffset, uint dwDataBitLength, ref byte bpData);
    
    //Output Process Data�� ���� ����.
    // dwBitOffset => ProcessImage outputs bit offset ��
    // dwDataBitLength => ������ �ϴ� output pdo data�� bit ũ��
    // pbyData => ������ �ϴ� �����͸� ���� buffer
    [DllImport("AXL.dll")] public static extern uint AxlECatWritePdoOutput(uint dwBitOffset, uint dwDataBitLength, ref byte pbyData);

    //COE�� �̿��� SDO(Service Data Objects)�� �о�´�
    [DllImport("AXL.dll")] public static extern uint AxlECatReadSdo(uint dwStationAddress, ushort wObjectIndex, byte byObjectSubIndex, ref byte pbyData, uint dwDataLength, ref uint pdwReadDataLength);

    //COE�� �̿��� SDO(Service Data Objects)�� ���� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlECatWriteSdo(uint dwStationAddress, ushort wObjectIndex, byte byObjectSubIndex, ref byte pbyData, uint dwDataLength);
    
    //�� ��ȣ�� ���� DWORD Type�� SDO�� �о�´�.
    [DllImport("AXL.dll")] public static extern uint AxlECatReadSdoFromAxisDword(int  lAxisNo, ushort wObjectIndex, byte byObjectSubIndex, ref uint pdwData);

    //�� ��ȣ�� ���� DWORD Type�� SDO�� ���� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlECatWriteSdoFromAxisDword(int  lAxisNo, ushort wObjectIndex, byte byObjectSubIndex, ref uint pdwData);

    //�� ��ȣ�� ���� WORD Type�� SDO�� �о�´�.
    [DllImport("AXL.dll")] public static extern uint AxlECatReadSdoFromAxisWord(int  lAxisNo, ushort wObjectIndex, byte byObjectSubIndex, ref ushort pwData);
    
    //�� ��ȣ�� ���� WORD Type�� SDO�� ���� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlECatWriteSdoFromAxisWord(int  lAxisNo, ushort wObjectIndex, byte byObjectSubIndex, ref ushort pwData);
    
    //�� ��ȣ�� ���� BYTE Type�� SDO�� �о�´�.
    [DllImport("AXL.dll")] public static extern uint AxlECatReadSdoFromAxisByte(int  lAxisNo, ushort wObjectIndex, byte byObjectSubIndex, ref byte pbyData);

    //�� ��ȣ�� ���� BYTE Type�� SDO�� ���� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlECatWriteSdoFromAxisByte(int  lAxisNo, ushort wObjectIndex, byte byObjectSubIndex, ref byte pbyData);

    //EEPRom�� ���� �о�´�
    [DllImport("AXL.dll")] public static extern uint AxlECatReadEEPRom(uint dwStationAddress, ushort wEEPRomStartOffset, ref ushort pwData, uint dwDataLength);

    //EEPRom�� ���� ����
    [DllImport("AXL.dll")] public static extern uint AxlECatWriteEEPRom(uint dwStationAddress, ushort wEEPRomStartOffset, ref ushort pwData, uint dwDataLength);
    
    //Register�� ���� �о�´�
    [DllImport("AXL.dll")] public static extern uint AxlECatReadRegister(uint dwStationAddress, ushort wRegisterOffset, object pvData, ushort wLen); // Intptr?
    
    //Register�� ���� ����
    [DllImport("AXL.dll")] public static extern uint AxlECatWriteRegister(uint dwStationAddress, ushort wRegisterOffset, object pvData, ushort wLen); // Intptr?
        
    //EtherCAT Slave�� Object Dictionary �� BackupData�� ���Ϸ� �����Ѵ�
    [DllImport("AXL.dll")] public static extern uint AxlECatSaveHotSwapData(uint dwStationAddress);

    //���Ϸ� ����� BackupData�� �ش� EtherCAT Slave�� �ε��Ѵ�
    [DllImport("AXL.dll")] public static extern uint AxlECatLoadHotSwapData(uint dwStationAddress);
    
    //HotSwapStart API(��ϵ� StationAddress�鿡 ���ؼ� HotSwap�� �����ϴ� �Լ�) ��� �� �ʿ��� HotSwapConfig�� StationAddress�� ����, ���� Ȯ��, ����
    [DllImport("AXL.dll")] public static extern uint AxlECatSetHotSwap(uint dwStationAddress);
    [DllImport("AXL.dll")] public static extern uint AxlECatIsSetHotSwap(uint dwStationAddress);
    [DllImport("AXL.dll")] public static extern uint AxlECatReSetHotSwap(uint dwStationAddress);
    
    //EtherCAT Master�� Mode�� Set(ConfigMode = 0, RunMode = 1)
    [DllImport("AXL.dll")] public static extern uint AxlECatSetMasterMode(uint dwMasterMode);
    
    //EtherCAT Master�� Mode ���¸� �����´�
    [DllImport("AXL.dll")] public static extern uint AxlECatGetMasterMode(ref uint pdMasterMode);
    
    //EtherCAT Master�� MasterOperationMode�� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlECatSetMasterOperationMode(uint dwOperationMode);
    
    //EtherCAT Master�� MasterOperationMode�� �����´�.
    [DllImport("AXL.dll")] public static extern uint AxlECatGetMasterOperationMode(ref uint pdwOperationMode);
    
    //EtherCAT Master�� Scan ����� ������, Scan�� Data�� SHM�� �����ϵ��� ���
    [DllImport("AXL.dll")] public static extern uint AxlECatRequestScanData();
    
    //Scan�� Slave�� ������ Index�� ���� �����´�
    //[DllImport("AXL.dll")] public static extern uint AxlECatGetSlaveScanDataByIndex(ref SlaveScanInfo pScanInfo, int nIndex); 
    
    //Scan�� Slave�� �� ������ �����´�
    [DllImport("AXL.dll")] public static extern uint AxlECatGetScanSlaveCount(ref uint pdwSlaveCount);
    
    //���� EtherCAT Master�� Status�� �����´�
    [DllImport("AXL.dll")] public static extern uint AxlECatGetStatus(ref int pnECMasterStatus, ref int pnECSlaveStatus, ref int pnECConnectedSlave, ref int pnECConfiguredSlave, ref int pnJobTaskCycleCnt, ref uint pdwECMasterNotification);
    
    //������ �߻��� Network�� �� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxlEcatReConnect();

    //Slave�� ������ Address ������ �����´�.
    [DllImport("AXL.dll")] public static extern uint AxaECatReadAddress(int nModuleNo, ref uint dwpStationAddress, ref int npAutoIncAddress, ref uint dwpAliasAddress);

    [DllImport("AXL.dll")] public static extern uint AxdECatReadAddress(int nModuleNo, ref uint dwpStationAddress, ref int npAutoIncAddress, ref uint dwpAliasAddress);

    [DllImport("AXL.dll")] public static extern uint AxmECatReadAddress(int nAxisNo, ref uint dwpStationAddress, ref int npAutoIncAddress, ref uint dwpAliasAddress);
    
    // Monitor
    // �����͸� ������ ������ �׸��� �߰��մϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorSetItem(int nItemIndex, uint dwSignalType, int nSignalNo, int nSubSignalNo);
    
    // ������ ������ ������ �׸�鿡 ���� ������ �����ɴϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorGetIndexInfo(ref int npItemSize, ref int npItemIndex);
    
    // ������ ������ ������ �� �׸��� ���� ������ �����ɴϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorGetItemInfo(int nItemIndex, ref uint dwpSignalType, ref int npSignalNo, ref int npSubSignalNo);
    
    // ��� ������ ���� �׸��� ������ �ʱ�ȭ �մϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorResetAllItem();
    
    // ���õ� ������ ���� �׸��� ������ �ʱ�ȭ �մϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorResetItem(int nItemIndex);
    
    // ������ ������ Ʈ���� ������ �����մϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorSetTriggerOption(uint dwSignalType, int nSignalNo, int nSubSignalNo, uint dwOperatorType, double dValue1, double dValue2);
    
    // ������ ������ Ʈ���� ������ �����ɴϴ�.
    //[DllImport("AXL.dll")] public static extern uint AxlMonitorGetTriggerOption(ref uint dwpSignalType, ref int npSignalNo, ref int npSubSignalNo, ref uint dwpOperatorType, ref double dpValue1, ref double dpValue2);
    
    // ������ ������ Ʈ���� ������ �ʱ�ȭ�մϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorResetTriggerOption();
    
    // ������ ������ �����մϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorStart(uint dwStartOption, uint dwOverflowOption);
    
    // ������ ������ �����մϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorStop();
    
    // ������ �����͸� �����ɴϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorReadData(ref int npItemSize, ref int npDataCount, ref double dpReadData);
    
    // ������ ������ �ֱ⸦ �����ɴϴ�.
    [DllImport("AXL.dll")] public static extern uint AxlMonitorReadPeriod(ref uint dwpPeriod);
    
    // X2, Y2 �࿡ ���� Offset ��ġ ������ ������ 2�� ���� ���� #01.
    [DllImport("AXL.dll")] public static extern uint AxmLineMoveDual01(int nCoordNo, ref double dpEndPosition, double dVelocity, double dAccel, double dDecel, double dOffsetLength, double dTotalLength, ref double dpStartOffsetPosition, ref double dpEndOffsetPosition);
    
    // X2, Y2 �࿡ ���� Offset ��ġ ������ ������ 2�� ��ȣ ���� #01
    [DllImport("AXL.dll")] public static extern uint AxmCircleCenterMoveDual01(int nCoordNo, ref int npAxes, ref double dpCenterPosition, ref double dpEndPosition, double dVelocity, double dAccel, double dDecel, uint dwCWDir, double dTotalLength, ref double dpStartOffsetPosition, ref double dpEndOffsetPosition); 
    
    // ECAT Foe ���� �Լ� �߰�
    [DllImport("AXL.dll")] public static extern uint AxdSetFirmwareUpdateInfo(int nModuleNo, uint dwTotalDataSize, uint dwTotalPacketSize);
    [DllImport("AXL.dll")] public static extern uint AxdSetFirmwareDataTrans(int nModuleNo, uint dwPacketIndex, ref uint dwaPacketData);
    [DllImport("AXL.dll")] public static extern uint AxdSetFirmwareUpdate(int nModuleNo, ref char szFileName, uint dwFileNameLen, uint dwPassWord);

    [DllImport("AXL.dll")] public static extern uint AxaSetFirmwareUpdateInfo(int nModuleNo, uint dwTotalDataSize, uint dwTotalPacketSize);
    [DllImport("AXL.dll")] public static extern uint AxaSetFirmwareDataTrans(int nModuleNo, uint dwPacketIndex, ref uint dwaPacketData);
    [DllImport("AXL.dll")] public static extern uint AxaSetFirmwareUpdate(int nModuleNo, ref char szFileName, uint dwFileNameLen, uint dwPassWord);

    [DllImport("AXL.dll")] public static extern uint AxmSetFirmwareUpdateInfo(int nAxisNo, uint dwTotalDataSize, uint dwTotalPacketSize);
    [DllImport("AXL.dll")] public static extern uint AxmSetFirmwareDataTrans(int nAxisNo, uint dwPacketIndex, ref uint dwaPacketData);
    [DllImport("AXL.dll")] public static extern uint AxmSetFirmwareUpdate(int nAxisNo, ref char szFileName, uint dwFileNameLen, uint dwPassWord);

    [DllImport("AXL.dll")] public static extern uint AxmMotSetOperationMode(int nAxisNo, uint dwOperationMode);
    [DllImport("AXL.dll")] public static extern uint AxmMotGetOperationMode(int nAxisNo, ref uint pdwOperationMode);

	[DllImport("AXL.dll")] public static extern uint AxmFilletMove(int lCoord, double[] dPos, double[] dFVector, double[] dSVector, double dVel, double dAccel, double dDecel, double dRadius);
}
