using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace Radix.Library.LestExplorer
{
    class LetsExplorerDll
    {
        private const string DllName = "LetsDll.dll";

        // 장비 연결 초기화
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int InitConnectionManual(string ip);

        // 브로드캐스트 후 노드 검색 (발견된 노드 개수 리턴, 실패 시 -1)
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int ScanNodeList();

        // 검색된 노드 정보를 outBuffer에 기록 (int 배열), 반환값은 기록한 개수
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetNodeList([Out] int[] outBuffer, int maxCount);

        // nodeNo에 대응하는 IP 문자열을 outBuffer에 기록 (버퍼 크기 최소 16 이상)
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern void GetIpFromNode(int nodeNo, StringBuilder outBuffer);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern int GetGroupsForIp([MarshalAs(UnmanagedType.LPStr)] string ip, [Out] int[] outHandles, int maxCount);

        // + 방향 Jog
        [DllImport(DllName, EntryPoint = "_JogPlus@20", CallingConvention = CallingConvention.StdCall)]
        public static extern int JogPlus(int rawNodeInfo, double velocity, double acceleration);

        // – 방향 Jog
        [DllImport(DllName, EntryPoint = "_JogMinus@20", CallingConvention = CallingConvention.StdCall)]
        public static extern int JogMinus(int rawNodeInfo, double velocity, double acceleration);

        // 조그 정지 (Plus/Minus 공용)
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int StopJog(int rawNodeInfo);

        //ORIGIN,HOME
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int OrgHome(int rawHandle, int direction, double initVelocity, double endVelocity, double acceleration);

        //2축보드 ORGIN HOME
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int StartHome2Axis(int rawHandle, int direction, double initVelocity, double endVelocity, double acceleration, double originOffset);

        //MoveAbs
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int MoveAbs(int rawHandle, double position, double velocity, double acceleration, double jerk);

        //MoveRel
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int MoveRel(int rawHandle, double relDistance, double velocity, double acceleration, double jerk);

        //위치값
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern double GetPosition(int rawHandle, int mode);

        //원점0
        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetZero(int rawHandle, int mode);

        //상태체크
        [DllImport(DllName, EntryPoint = "_GetState@12", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetState(int rawHandle, int target, byte nodeType);

        //상태비트읽어오는 DLL GetAxisState_ALM 부터 GetAxisState_ORG_DONE까지

        [DllImport(DllName, EntryPoint = "_GetAxisState_SERVO@0", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAxisState_SERVO();

        [DllImport(DllName, EntryPoint = "_GetAxisState_MOVING@0", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAxisState_MOVING();

        [DllImport(DllName, EntryPoint = "_GetAxisState_ORG@0", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAxisState_ORG();

        [DllImport(DllName, EntryPoint = "_GetAxisState_All@0", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAxisState_All();

        [DllImport(DllName, EntryPoint = "_GetAxisState_ALM@0", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAxisState_ALM();

        [DllImport(DllName, EntryPoint = "_GetAxisState_ALM_RESET@0", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAxisState_ALM_RESET();

        [DllImport(DllName, EntryPoint = "_GetAxisState_EMG@0", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAxisState_EMG();

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, EntryPoint = "_GetNodeInfo@4")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern byte GetNodeType(int rawHandle);

        [DllImport(DllName, EntryPoint = "_AlmResetPulse@12", CallingConvention = CallingConvention.StdCall)]
        public static extern int AlmResetPulse(int rawHandle, int pulseMs, byte nodeType);


        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetEmgState(int rawHandle, byte nodeType);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetEmgOverlayEnabled(int enable);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetEmgBitIndex(int bitIndex);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void SetEmgActiveLow(int activeLow);

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int AutoDetectEmg2Axis(int rawHandle, int windowMs);
    }
}
