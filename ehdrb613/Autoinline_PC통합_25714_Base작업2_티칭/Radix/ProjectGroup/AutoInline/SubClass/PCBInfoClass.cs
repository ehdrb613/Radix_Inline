using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Collections.Concurrent;

namespace Radix
{
    public class PCBInfoClass
    {
        public string[] Barcode; // 스캔된 2d코드. array 갯수만큼
        public bool[] Xout; // 각 array xout 체크 여부. array 갯수만큼, false 정상
        public bool[] Skip; // 각 array Skip 체크 여부. array 갯수만큼, false 정상
        public bool[] BadMark; // 각 array BadMark 체크 여부. array 갯수만큼, false 정상
        public bool TestPass; // 전라인에서 Pass로 넘어왔는지 여부.
        public bool DTestPass;  //다운로드 테스트 여부
        public bool FTestPass;
        public bool TempCheck; // 온도체크 통과 여부
        public FuncInline.enumSMDStatus[] SMDStatus; // 각 Array의 테스트 상황
        public FuncInline.enumSMDStatus PCBStatus; // PCB 전체 테스트 상황 //***************************************
        public Stopwatch StopWatch; // 테스트 시간 측정용
        public int TestTime; // 테스트 완료 시간
        public Stopwatch[] TestWatch; // 어레이별 테스트 시간
        public int[] ErrorCode; // 테스트 에러코드
        public int[] BeforeCode; // 이전 테스트 에러코드
        public FuncInline.enumNGType NgType; // fail 구분
        //public string ImagePath; // 안 쓰는 거 같은데?
        public int[] CommandRetry;
        public int SelfRetestCount;
        public int OtherRetestCount;
        public bool SelfReTest;
        public bool OtherReTest;
        public bool UserCancel;
        public int[] TestSite;
        public bool BuyerChange;
        public bool[] SMDReady;
        public bool[] SMDReadySent;
        public FuncInline.enumTeachingPos Destination;
        //public ConcurrentQueue<FuncInline.enumMoveAction> MoveActionQueue; // PCB 이송라인,투입부,배출부,지그처리부,NG 셔틀부 따로 지정해야 함
        public bool OutputNG;
        //public int CycleStartTime;
        public string SiteInputDate;
        public string SiteInputTime;
        public bool Carrier; // 캐리어 존재 유무

        public PCBInfoClass()
        {
            Barcode = new string[FuncInline.MaxArrayCount];
            for (int i = 0; i < Barcode.Length; i++)
            {
                Barcode[i] = "";
            }
            Xout = new bool[FuncInline.MaxArrayCount];
            Skip = new bool[FuncInline.MaxArrayCount];
            BadMark = new bool[FuncInline.MaxArrayCount];
            TestPass = false;
            DTestPass = false;  //다운로드 테스트 여부
            FTestPass = false;
            TempCheck = false;
            SMDStatus = new FuncInline.enumSMDStatus[FuncInline.MaxArrayCount];
            for (int i = 0; i < SMDStatus.Length; i++)
            {
                SMDStatus[i] = FuncInline.enumSMDStatus.UnKnown;
            }
            PCBStatus = FuncInline.enumSMDStatus.UnKnown;
            StopWatch = new Stopwatch();
            TestTime = 0;
            TestWatch = new Stopwatch[FuncInline.MaxArrayCount];
            for (int i = 0; i < TestWatch.Length; i++)
            {
                TestWatch[i] = new Stopwatch();
            }
            ErrorCode = new int[FuncInline.MaxArrayCount];
            for (int i = 0; i < ErrorCode.Length; i++)
            {
                ErrorCode[i] = -1;
            }
            BeforeCode = new int[FuncInline.MaxArrayCount];
            for (int i = 0; i < BeforeCode.Length; i++)
            {
                BeforeCode[i] = -1;
            }
            NgType = FuncInline.enumNGType.Unknown;
            CommandRetry = new int[FuncInline.MaxArrayCount];
            SelfRetestCount = 0;
            OtherRetestCount = 0;
            SelfReTest = false;
            OtherReTest = false;
            UserCancel = false;
            TestSite = new int[10];
            BuyerChange = false;
            SMDReady = new bool[FuncInline.MaxArrayCount];
            SMDReadySent = new bool[FuncInline.MaxArrayCount];
            Destination = FuncInline.enumTeachingPos.None;
            OutputNG = false;
            SiteInputDate = "";
            SiteInputTime = "";
            Carrier = false;
        }
    }
}
