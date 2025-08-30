/****************************************************************************
*****************************************************************************
**
** File Name
** ---------
**
** AXP.CS
**
** COPYRIGHT (c) AJINEXTEK Co., LTD
**
*****************************************************************************
*****************************************************************************
**
** Description
** -----------
** Ajinextek Macro Library Header File
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

public class CAXP
{
    // ������ ��ũ�ο� ��� ����� �����Ѵ�.
    // lMacroNo     : 0 ~ 7
    [DllImport("AXL.dll")] public static extern uint AxpMacroBeginNode(int nMacroNo);    
    // ������ ��ũ�ο� ��� ����� �����Ѵ�.
    // lMacroNo     : 0 ~ 7
    [DllImport("AXL.dll")] public static extern uint AxpMacroEndNode(int nMacroNo);    
    // ������ ��ũ�ο� ��ϵ� ������ ��� �����Ѵ�. 
    // lMacroNo     : ��ϵ� ������ ��� ���� �� ��ũ�� ��ȣ�� ����
    //  [-1]        : -1�� ��� ��� ��ũ�ο� ��ϵ� ������ ��� �����Ѵ�.
    //  [0 ~ 7]     : ������ ��ũ�ο� ��ϵ� ������ ��� �����Ѵ�.
    [DllImport("AXL.dll")] public static extern uint AxpMacroWriteClear(int nMacroNo);
    
    // ������ ��ũ�ο� ��带 ����Ѵ�.
    // lMacroNo     : 0 ~ 7
    // uFunction    : ��ũ�ο� ����� �Լ��� �����Ѵ�.{ MACRO_FUNCTION }
    //  [0] MACRO_FUNC_CALL         : �ٸ� ��ũ�θ� ȣ���Ѵ�.(ȣ��� ��ũ�ΰ� ����ǰų� RETURN�ϸ� �ٽ� ���ƿ�)
    //  [1] MACRO_FUNC_JUMP         : �ٸ� ��ũ�γ� ���� �����Ѵ�. (JUMP��ġ�� �ǵ��ƿ��� ����)
    //  [2] MACRO_FUNC_RETURN       : ȣ���� ��ũ�η� �ǵ��ư�
    //  [3] MACRO_FUNC_REPEAT       : ���� ��ġ���� ������ �����̸� ������ Ƚ����ŭ �ݺ�������
    //  [4] MACRO_FUNC_SET_OUTPUT   : Digital Output, Analog Output�� �����
    //  [5] MACRO_FUNC_WAIT         : ������ �ð���ŭ �����
    //  [6] MACRO_FUNC_STOP         : ��ũ�� ������ ������
    // dpArg[16] : ������ Function�� ���� ���� �迭
    // �� ������ : �迭�� ������ �ݵ�� 16���� �ؾ� ��
    //  [0] MACRO_FUNC_CALL
    //      dArg[0] Macro ��ȣ  
    //  [1] MACRO_FUNC_JUMP
    //      dArg[0] Jump Type       : [0] MACRO_JUMP_MACRO, [1] MACRO_JUMP_NODE
    //      dArg[1] ��ũ�ι�ȣ or ����ȣ 
    //  [2] MACRO_FUNC_RETURN       : ���� ����
    //  [3] MACRO_FUNC_REPEAT       : �ݺ����� ����
    //      dArg[0] �ݺ��� ȸ��
    //      dArg[1] ���� ��ġ���� �ݺ��� ��� ��ȣ(���� ����� ��ȣ���� ���� ��ȣ���� ��)
    //  [4] MACRO_FUNC_SET_OUTPUT
    //      dArg[0] Output Type     : [0] MACRO_DIGITAL_OUTPUT, [1] MACRO_ANALOG_OUTPUT
    //      dArg[1] DO ����ȣ or AO ä�ι�ȣ
    //      dArg[2] Data Type
    //        - Digital Output�ϰ��: [0] MACRO_DATA_BIT, [1] MACRO_DATA_BYTE, [2] MACRO_DATA_WORD, [3] MACRO_DATA_DWORD, [4] MACRO_DATA_BYTE12 
    //        - Analog Output�ϰ�� : [5] MACRO_DATA_VOLTAGE, [6] MACRO_DATA_DIGIT
    //      dArg[3] Offset or Byte Size
    //        - Digital Output�ϰ��: Offset / ByteSize(MACRO_DATA_BYTE12 Type�� ���)
    //        - Analog Output�ϰ�� : Reserved
    //      dArg[4] ��� ��
    //        - Digital Output�ϰ��: Data Type�� ������ ���� Digital Output, 
    //        - Analog Output�ϰ�� : ������� or ��� Digit
    //      dArg[4] ~ dArg[15] Byte ��°� (MACRO_DATA_BYTE12 Type�� ���)
    //        - ������ Byte(�ִ� 12Byte)������ �������� ��� ��⿡ ������� �ѹ��� �����
    //  [5] MACRO_FUNC_WAIT
    //      dArg[0] ��� �ð�(ms)   : ������ ms ���� �����
    //  [6] MACRO_FUNC_STOP
    //      dArg[0] �������        : [0] MACRO_QUICK_STOP, [1] MACRO_SLOW_STOP
    [DllImport("AXL.dll")] public static extern uint AxpMacroAddNode(int nMacroNo, uint uFunction, ref double dpArg);
    // ������ ��ũ���� ��忡 ������ ���� ��ȯ�Ѵ�. (��ȯ���� AxpMacroAddNode �Լ��� ������ ����)
    [DllImport("AXL.dll")] public static extern uint AxpMacroGetNode(int nMacroNo, int nNodeNo, ref uint upFunction, ref double dpArg);
    // ������ ��ũ�ο� ��ϵ� ������ �˻��� �� ������ �߻� ��� ��ġ�� ���� �ڵ带 ��ȯ�Ѵ�.
    // �� ������ : �ݵ�� �� �Լ��� ��ϵ� ���鿡 ���� ��ȿ�� �˻縦 �Ϸ��ؾ� ��ũ�� ������ �� �� ����.
    // lMacroNo         : 0 ~ 7
    // *lpErrorNodeNo   : ������ �߻��� ����ȣ
    // *upErrorCode     : ������ �߻��� ��忡 ���� �����ڵ�
    [DllImport("AXL.dll")] public static extern uint AxpMacroCheckNodeAll(int nMacroNo, ref int npErrorNodeNo, ref uint upErrorCode);
    // ������ ��ũ���� ������ �����Ѵ�.
    // �� ������  : �ݵ�� AxpMacroCheckNodeAll �Լ��� ��ϵ� ���鿡 ���� ��ȿ�� �˻縦 �Ϸ��ؾ� ��ũ�� ������ �� �� ����.
    // lMacroNo         : 0 ~ 7
    // uCondition       : ��ũ�� ���� ������ ������
    //  [0] MACRO_START_READY       : ��� ���·� ����
    //  [1] MACRO_START_IMMEDIATE   : ��� ����
    // bLockResource    : ��ũ�ο� ��ϵ� ������ �Է¸��, �Ƴ��α� ��� ä�ε鿡 ���� ������� ���θ� ����
    //  [FALSE]         : �������
    //  [TRUE]          : �������
    // lRepeatCount     : ��ũ�� ��ü ������ �ݺ� ȸ���� ����
    //  [-1]            : ���ѹݺ� ������
    //  [0]             : �ݺ��������� ����
    //  [1~ ]           : ������ ȸ����ŭ �ݺ� ������
    [DllImport("AXL.dll")] public static extern uint AxpMacroStart(int nMacroNo, uint uCondition, bool bLockResource, int nRepeatCount);
    // ������ ��ũ���� ���� ������ ��ȯ�Ѵ�.
    // lMacroNo         : 0 ~ 7
    [DllImport("AXL.dll")] public static extern uint AxpMacroGetStartInfo(int nMacroNo, ref uint upCondition, ref bool bpLockResource, ref int npRepeatCount);
    // ������ ��ũ���� ������ �����Ѵ�.
    // lMacroNo         : -1 ~ 7
    //  [-1]            : -1�� ��� ��� ��ũ���� ������ �����Ѵ�.
    // uStopMode        : �������� ��带 �����Ѵ�. (��ϵ� ��ũ�ο� ��� �������� ���� ��� �ǹ̰� ����)
    //  [0] MACRO_QUICK_STOP
    //  [1] MACRO_SLOW_STOP
    [DllImport("AXL.dll")] public static extern uint AxpMacroStop(int nMacroNo, uint uStopMode);
    // ������ Macro�� �������¸� ��ȯ�Ѵ�.
    // lMacroNo         : 0 ~ 7
    // *upStatus        : ��ũ���� ��������
    //  [0] MACRO_STATUS_STOP   : ��ũ�� ��������
    //  [1] MACRO_STATUS_READY  : ��ũ�� ���� ��� ����
    //  [2] MACRO_STATUS_RUN    : ��ũ�� ��������
    //  [3] MACRO_STATUS_ERROR  : ��ũ�� ��������(��������)
    // *lpRepeatCount   : ��ũ���� ��ü �ݺ����� ȸ���� ��ȯ
    [DllImport("AXL.dll")] public static extern uint AxpMacroReadRunStatus(int nMacroNo, ref uint upStatus, ref int npRepeatCount);
    // ������ ��ũ�ο� ��ϵ� ��尳���� ���� �������� ��������� ��ȯ�Ѵ�.
    // lMacroNo         : 0 ~ 7
    // *lpCurNodeNo     : ���� �������� ����� ��ȣ
    // *lpTotalNodeNum  : ������ ��ũ���� ��ü ��� ����
    [DllImport("AXL.dll")] public static extern uint AxpMacroGetNodeNum(int nMacroNo, ref int npCurNodeNo, ref int npTotalNodeNum);
    // ������ ��ũ�ο� �Լ��� ���� ���������� ��ȯ�Ѵ�.
    // lMacroNo         : 0 ~ 7
    // uFunction        : ������ Ȯ���� �Լ��� �����Ѵ�.{ MACRO_FUNCTION }
    //  [0] MACRO_FUNC_CALL
    //  [1] MACRO_FUNC_JUMP
    //  [2] MACRO_FUNC_RETURN
    //  [3] MACRO_FUNC_REPEAT
    //  [4] MACRO_FUNC_SET_OUTPUT
    //  [5] MACRO_FUNC_WAIT
    //  [6] MACRO_FUNC_STOP
    // dpReturnData[16]  : ������ �Լ��� ���� ��ȯ��
    // �� ������ : �迭�� ������ �ݵ�� 16���� �ؾ� ��
    //  [0] MACRO_FUNC_CALL
    //      dArg[0] ���� ȣ�� �� CALL����� ����ȣ
    //      dArg[1] ������ ��ũ�� ������ CALL��ɵ� ���� Index�� ��ȯ
    //  [1] MACRO_FUNC_JUMP
    //      dArg[0] ���� ȣ��� JUMP����� ����ȣ
    //      dArg[1] ������ ��ũ�� ������ JUMP��ɵ� ���� Index�� ��ȯ
    //  [2] MACRO_FUNC_RETURN       : ���� ����
    //      dArg[0] ���� ȣ��� RETURN����� ����ȣ
    //      dArg[1] ������ ��ũ�� ������ RETURN��ɵ� ���� Index�� ��ȯ
    //  [3] MACRO_FUNC_REPEAT       : �ݺ����� ����
    //      dArg[0] ���� ȣ��� REPEAT����� ����ȣ
    //      dArg[1] ������ ��ũ�� ������ REPEAT��ɵ� ���� Index�� ��ȯ
    //      dArg[2] ���� ȣ��� REPEAT����� �ݺ����� ����ȸ���� ��ȯ
    //      dArg[3] ���� ȣ��� REPEAT����� �ݺ������� ȸ���� ��ȯ
    //  [4] MACRO_FUNC_SET_OUTPUT
    //      dArg[0] ���� ȣ��� SET_OUTPUT����� ����ȣ
    //      dArg[1] ������ ��ũ�� ������ SET_OUTPUT��ɵ� ���� Index�� ��ȯ
    //      dArg[2] ���� ȣ��� SET_OUTPUT����� ��°��� ��ȯ
    //      dArg[2~13] Data Type�� "[4] MACRO_DATA_BYTE12"�� ����� Byte ��°�
    //  [5] MACRO_FUNC_WAIT
    //      dArg[0] ���� ȣ��� WAIT����� ����ȣ
    //      dArg[1] ������ ��ũ�� ������ WAIT��ɵ� ���� Index�� ��ȯ
    //      dArg[2] ���� ȣ��� WAIT����� ���� �ð��� ��ȯ
    //      dArg[3] ���� ȣ��� WAIT����� �ҿ� �ð��� ��ȯ
    //  [6] MACRO_FUNC_STOP
    //      dArg[0] ���� ȣ��� STOP����� ����ȣ
    //      dArg[1] ������ ��ũ�� ������ STOP��ɵ� ���� Index�� ��ȯ
    [DllImport("AXL.dll")] public static extern uint AxpMacroReadFunctionStatus(int nMacroNo, uint uFunction, ref double dpReturnData);
}