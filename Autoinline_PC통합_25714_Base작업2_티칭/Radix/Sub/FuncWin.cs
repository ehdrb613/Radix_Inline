using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;  // DLLImport
using System.Diagnostics; // Process
using System.Windows.Forms;
using System.Drawing;

namespace Radix
{
    #region 구조체 선언
    /** @brief 시스템 시간 체크용 구조체 */
    struct SYSTEMTIME //  YJ 추가
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }
    #endregion

    /** @brief 윈도우 처리 함수 선언 */
    class FuncWin
    {

        #region API용 상수
        private const UInt32 WM_SYSCOMMAND = 0x112;
        private const UInt32 SC_RESTORE = 0xf120;
        public const int WM_CLOSE = 0x0010;

        private static System.Threading.Timer _messageTimer; //쓰레드 타이머
        private static System.Threading.Timer _timeoutTimer; //쓰레드 타이머

        private static string _caption;
        private static string _text;
        #endregion

        /** @brief OpenExeInTabPage(외부 실행파일을 패널 안에 삽입) 함수 실행 후 윈도 포인터 */
        static IntPtr ptr = IntPtr.Zero;


        #region 윈도우 관련
        #region API 함수 Import
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        /** 
         * @brief 지정된 창 제목 표시줄의 텍스트를 변경합니다(있는 경우). 
         *      지정한 창이 컨트롤이면 컨트롤의 텍스트가 변경됩니다. 
         *      그러나 SetWindowText 는 다른 애플리케이션에서 컨트롤의 텍스트를 변경할 수 없습니다.
         * @param hWnd 텍스트를 변경할 창 또는 컨트롤에 대한 핸들입니다.
         * @param lpString 새 제목 또는 컨트롤 텍스트입니다.
         * @return 함수가 성공하면 반환 값이 0이 아닙니다.
         *      함수가 실패하면 반환 값은 0입니다. 확장 오류 정보를 가져오려면 GetLastError를 호출합니다.
         */
        public static extern bool SetWindowText(IntPtr hWnd, String lpString);

        [DllImport("user32.dll", SetLastError = true)]
        /** 
         * @brief 클래스 이름 및 창 이름이 지정된 문자열과 일치하는 최상위 창에 대한 핸들을 검색합니다. 
         *      이 함수는 자식 창을 검색하지 않습니다. 
         *      이 함수는 대/소문자를 구분하는 검색을 수행하지 않습니다.
         * @param lpClassName RegisterClass 또는 RegisterClassEx 함수에 대한 이전 호출에서 만든 클래스 이름 또는 클래스 원 자입니다. 
         *      원자는 lpClassName의 하위 단어여야 합니다. 
         *      상위 단어는 0이어야 합니다.
         *      lpClassName이 문자열을 가리키는 경우 창 클래스 이름을 지정합니다. 
         *      클래스 이름은 RegisterClass 또는 RegisterClassEx에 등록된 이름 또는 미리 정의된 컨트롤 클래스 이름일 수 있습니다.
         *      lpClassName이 NULL이면 제목이 lpWindowName 매개 변수와 일치하는 창을 찾습니다.
         * @param lpWindowName 창 이름(창의 제목)입니다. 이 매개 변수가 NULL이면 모든 창 이름이 일치합니다.
         * @return 함수가 성공하면 반환 값은 지정된 클래스 이름과 창 이름을 가진 창에 대한 핸들입니다.
         *      함수가 실패하면 반환 값은 NULL입니다. 확장 오류 정보를 가져오려면 GetLastError를 호출합니다.
         */
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /** 
         * @brief 클래스 이름과 창 이름이 지정된 문자열과 일치하는 창에 대한 핸들을 검색합니다. 
         *      이 함수는 지정된 자식 창 다음에 있는 자식 창부터 자식 창을 검색합니다. 
         *      이 함수는 대/소문자를 구분하는 검색을 수행하지 않습니다.
         * @param hwndParent 자식 창을 검색할 부모 창에 대한 핸들입니다.
         *      hwndParent가 NULL이면 함수는 데스크톱 창을 부모 창으로 사용합니다. 
         *      이 함수는 바탕 화면의 자식 창인 창 간에 검색합니다.
         *      hwndParent가 HWND_MESSAGE 경우 함수는 모든 메시지 전용 창을 검색합니다.
         * @param hwndChildAfter 자식 창에 대한 핸들입니다. 
         *      검색은 Z 순서의 다음 자식 창으로 시작됩니다. 
         *      자식 창은 하위 창이 아니라 hwndParent의 직접 자식 창이어야 합니다.
         *      hwndChildAfter가 NULL이면 hwndParent의 첫 번째 자식 창으로 검색이 시작됩니다.
         *      hwndParent와 hwndChildAfter가 모두 NULL인 경우 함수는 모든 최상위 및 메시지 전용 창을 검색합니다.
         * @param lpszClass RegisterClass 또는 RegisterClassEx 함수에 대한 이전 호출에서 만든 클래스 이름 또는 클래스 원 자입니다. 
         *      원자는 lpszClass의 낮은 순서 단어에 배치되어야 합니다. 
         *      상위 단어는 0이어야 합니다.
         *      lpszClass가 문자열이면 창 클래스 이름을 지정합니다. 
         *      클래스 이름은 RegisterClass 또는 RegisterClassEx에 등록된 이름 또는 미리 정의된 컨트롤 클래스 이름일 수 있습니다
         *      MAKEINTATOM(0x8000). 이 후자의 경우 0x8000 메뉴 클래스의 원자입니다. 자세한 내용은 이 항목의 설명 섹션을 참조하세요.
         * @param lpszWindow 창 이름(창의 제목)입니다. 이 매개 변수가 NULL이면 모든 창 이름이 일치합니다.
         * @return 함수가 성공하면 반환 값은 지정된 클래스 및 창 이름이 있는 창에 대한 핸들입니다.
         *      함수가 실패하면 반환 값은 NULL입니다. 확장 오류 정보를 가져오려면 GetLastError를 호출합니다.
         */
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        /** 
         * @brief 지정된 점을 포함하는 창에 대한 핸들을 검색합니다.
         * @param x 포인트의 x 좌표
         * @param y 포인트의 y 좌표
         * @return 반환 값은 점을 포함하는 창에 대한 핸들입니다. 
         *      지정된 지점에 창이 없으면 반환 값은 NULL입니다. 
         *      점이 정적 텍스트 컨트롤 위에 있는 경우 반환 값은 정적 텍스트 컨트롤 아래의 창에 대한 핸들입니다.
         */
        public static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        /** 
         * @brief Changes the position and dimensions of the specified window. 
         *      For a top-level window, the position and dimensions are relative to the upper-left corner of the screen. 
         *      For a child window, they are relative to the upper-left corner of the parent window's client area.
         * @param hWnd A handle to the window.
         * @param x The new position of the left side of the window.
         * @param y The new position of the top of the window.
         * @param nWidth The new width of the window.
         * @param nHeight The new height of the window.
         * @return Indicates whether the window is to be repainted. 
         *      If this parameter is TRUE, the window receives a message. 
         *      If the parameter is FALSE, no repainting of any kind occurs. 
         *      This applies to the client area, the nonclient area (including the title bar and scroll bars), 
         *      and any part of the parent window uncovered as a result of moving a child window.
         */
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        /** 
         * @brief 지정 윈도우를 활성/비활성
         * @param hWnd A handle to the window.
         * @param bEnable 활성 여부
         * @return 
         */
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("User32.dll")]
        /** 
         * @brief 지정된 창을 만든 스레드를 포그라운드로 가져오고 창을 활성화합니다. 
         *      키보드 입력은 창으로 전달되며 사용자에 대한 다양한 시각 신호가 변경됩니다. 
         *      시스템은 다른 스레드보다 포그라운드 창을 만든 스레드에 약간 더 높은 우선 순위를 할당합니다.
         * @param hWnd A handle to the window.
         * @return 창이 포그라운드로 가져온 경우 반환 값은 0이 아닌 값입니다.
         *      창을 포그라운드로 가져오지 않은 경우 반환 값은 0입니다.
         */
        public static extern Int32 SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        /** 
         * @brief 지정된 메시지를 창 또는 창으로 보냅니다. 
         *      SendMessage 함수는 지정된 창에 대한 창 프로시저를 호출하며 창 프로시저가 메시지를 처리할 때까지 반환되지 않습니다.
         *      메시지를 보내고 즉시 반환하려면 SendMessageCallback 또는 SendNotifyMessage 함수를 사용합니다. 
         *      스레드의 메시지 큐에 메시지를 게시하고 즉시 반환하려면 PostMessage 또는 PostThreadMessage 함수를 사용합니다
         * @param hWnd 창 프로시저에서 메시지를 받을 창에 대한 핸들입니다. 
         *      이 매개 변수가 HWND_BROADCAST ((HWND)0xffff 경우 비활성화되거나 보이지 않는 소유되지 않은 창, 겹치는 창 및 팝업 창을 포함하여 시스템의 모든 최상위 창으로 메시지가 전송됩니다. 
         *      하지만 메시지는 자식 창으로 전송되지 않습니다.
         *      메시지 전송에는 UIPI가 적용됩니다. 
         *      프로세스의 스레드는 무결성 수준이 낮거나 같은 프로세스에서 스레드의 메시지 큐에만 메시지를 보낼 수 있습니다.
         * @param Msg 보낼 메시지입니다.
         *      시스템 제공 메시지 목록은 시스템 정의 메시지를 참조하세요.
         * @param WPARAM 추가 메시지 관련 정보입니다.
         * @param 추가 메시지 관련 정보입니다.
         * @return 반환 값은 메시지 처리의 결과를 지정합니다. 
         *      전송된 메시지에 따라 달라집니다.
         */
        public static extern Int32 SendMessage(IntPtr hWnd, UInt32 msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        /** 
         * @brief 지정된 창을 만든 스레드와 연결된 메시지 큐에 메시지를 배치(게시)하고 스레드가 메시지를 처리할 때까지 기다리지 않고 반환합니다.
         *      스레드와 연결된 메시지 큐에 메시지를 게시하려면 PostThreadMessage 함수를 사용합니다.
         * @param hWnd 창 프로시저가 메시지를 수신하는 창에 대한 핸들입니다.  
         *      Windows Vista부터 메시지 게시에는 UIPI가 적용됩니다. 
         *      프로세스의 스레드는 무결성 수준이 낮거나 같은 프로세스에서 스레드의 메시지 큐에만 메시지를 게시할 수 있습니다.
         * @param Msg 보낼 메시지입니다.
         *      시스템 제공 메시지 목록은 시스템 정의 메시지를 참조하세요.
         * @param WPARAM 추가 메시지 관련 정보입니다.
         * @param 추가 메시지 관련 정보입니다.
         * @return 함수가 성공하면 반환 값이 0이 아닙니다.
         *      함수가 실패하면 반환 값은 0입니다. 
         *      확장 오류 정보를 가져오려면 GetLastError를 호출합니다.
         */
        public static extern Int32 PostMessage(IntPtr hWnd, UInt32 msg, int wParam, int lParam);

        [DllImport("User32")]
        /** 
         * @brief 지정된 창의 표시 상태를 설정합니다.
         * @param hWnd 창에 대한 핸들입니다.
         * @param nCmdShow 창을 표시하는 방법을 제어합니다. 
         *      애플리케이션을 시작한 프로그램이 STARTUPINFO 구조를 제공하는 경우 이 매개 변수는 애플리케이션이 ShowWindow를 처음 호출할 때 무시됩니다. 
         *      그렇지 않으면 ShowWindow가 처음 호출될 때 값은 해당 nCmdShow 매개 변수에서 WinMain 함수에서 가져온 값이어야 합니다. 
         *      후속 호출에서 이 매개 변수는 다음 값 중 하나일 수 있습니다.
         * @return 창이 이전에 표시된 경우 반환 값은 0이 아닌 값입니다.
         *      창이 이전에 숨겨져 있으면 반환 값은 0입니다.
         */
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        /** 
         * @brief Sets the current system time and date. The system time is expressed in Coordinated Universal Time (UTC).
         * @param lpSystemTime A pointer to a SYSTEMTIME structure that contains the new system date and time.
         *      The wDayOfWeek member of the SYSTEMTIME structure is ignored.
         * @return If the function succeeds, the return value is nonzero.
         *      If the function fails, the return value is zero. 
         *      To get extended error information, call GetLastError.
         */
        public extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime); // YJ 추가

        [DllImport("user32.dll")]
        /** 
         * @brief 클래스 이름과 창 이름이 지정된 문자열과 일치하는 창에 대한 핸들을 검색합니다. 
         *      이 함수는 지정된 자식 창 다음에 있는 자식 창부터 자식 창을 검색합니다. 
         *      이 함수는 대/소문자를 구분하는 검색을 수행하지 않습니다.
         * @param hWnd1 자식 창을 검색할 부모 창에 대한 핸들입니다.
         *      hwndParent가 NULL이면 함수는 데스크톱 창을 부모 창으로 사용합니다. 
         *      이 함수는 바탕 화면의 자식 창인 창 간에 검색합니다.
         *      hwndParent가 HWND_MESSAGE 경우 함수는 모든 메시지 전용 창을 검색합니다.
         * @param hWnd2 자식 창에 대한 핸들입니다. 
         *      검색은 Z 순서의 다음 자식 창으로 시작됩니다. 
         *      자식 창은 하위 창이 아니라 hwndParent의 직접 자식 창이어야 합니다.
         *      hwndChildAfter가 NULL이면 hwndParent의 첫 번째 자식 창으로 검색이 시작됩니다.
         *      hwndParent와 hwndChildAfter가 모두 NULL인 경우 함수는 모든 최상위 및 메시지 전용 창을 검색합니다.
         * @param Ipsz1 RegisterClass 또는 RegisterClassEx 함수에 대한 이전 호출에서 만든 클래스 이름 또는 클래스 원 자입니다. 
         *      원자는 lpszClass의 낮은 순서 단어에 배치되어야 합니다. 
         *      상위 단어는 0이어야 합니다.
         *      lpszClass가 문자열이면 창 클래스 이름을 지정합니다. 
         *      클래스 이름은 RegisterClass 또는 RegisterClassEx에 등록된 이름 또는 미리 정의된 컨트롤 클래스 이름일 수 있습니다MAKEINTATOM(0x8000). 
         *      이 후자의 경우 0x8000 메뉴 클래스의 원자입니다. 
         *      자세한 내용은 이 항목의 설명 섹션을 참조하세요.
         * @param Ipsz2 창 이름(창의 제목)입니다. 
         *      이 매개 변수가 NULL이면 모든 창 이름이 일치합니다.
         * @return 함수가 성공하면 반환 값은 지정된 클래스 및 창 이름이 있는 창에 대한 핸들입니다.
         *      함수가 실패하면 반환 값은 NULL입니다. 
         *      확장 오류 정보를 가져오려면 GetLastError를 호출합니다.
         */
        public static extern int FindWindowEx(int hWnd1, int hWnd2, string Ipsz1, string Ipsz2);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        /** 
         * @brief 지정된 메시지를 창 또는 창으로 보냅니다. 
         *      SendMessage 함수는 지정된 창에 대한 창 프로시저를 호출하며 창 프로시저가 메시지를 처리할 때까지 반환되지 않습니다.
         *      메시지를 보내고 즉시 반환하려면 SendMessageCallback 또는 SendNotifyMessage 함수를 사용합니다. 
         *      스레드의 메시지 큐에 메시지를 게시하고 즉시 반환하려면 PostMessage 또는 PostThreadMessage 함수를 사용합니다.
         * @param hWnd 창 프로시저에서 메시지를 받을 창에 대한 핸들입니다. 
         *      이 매개 변수가 HWND_BROADCAST ((HWND)0xffff 경우 비활성화되거나 보이지 않는 소유되지 않은 창, 겹치는 창 및 팝업 창을 포함하여 시스템의 모든 최상위 창으로 메시지가 전송됩니다. 
         *      하지만 메시지는 자식 창으로 전송되지 않습니다.
         *      메시지 전송에는 UIPI가 적용됩니다. 
         *      프로세스의 스레드는 무결성 수준이 낮거나 같은 프로세스에서 스레드의 메시지 큐에만 메시지를 보낼 수 있습니다.
         * @param Msg 보낼 메시지입니다.
         *      시스템 제공 메시지 목록은 시스템 정의 메시지를 참조하세요.
         * @param wParam 추가 메시지 관련 정보입니다.
         * @param lParam 추가 메시지 관련 정보입니다.
         * @return 반환 값은 메시지 처리의 결과를 지정합니다. 
         *      전송된 메시지에 따라 달라집니다.
         */
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        /** 
         * @brief 지정된 자식 창의 부모 창을 변경합니다.
         * @param hWndChild 자식 창에 대한 핸들입니다.
         * @param hWndNewParent 새 부모 창에 대한 핸들입니다. 
         *      이 매개 변수가 NULL이면 바탕 화면 창이 새 부모 창이 됩니다. 
         *      이 매개 변수가 HWND_MESSAGE 자식 창은 메시지 전용 창이 됩니다.
         * @return 함수가 성공하면 반환 값은 이전 부모 창에 대한 핸들입니다.
         *      함수가 실패하면 반환 값은 NULL입니다. 
         *      확장 오류 정보를 가져오려면 GetLastError를 호출합니다.
         */
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        #endregion


        /** 
         * @brief 자식 윈도우의 리스트를 검색합니다.
         * @param hParent 부모 창에 대한 핸들입니다. 
         * @param maxCount 검색할 최대 목록 수입니다.
         * @return 검색된 자식 윈도우의 포인터리스트
         */
        public static List<IntPtr> GetAllChildrenWindowHandles(IntPtr hParent, int maxCount) // 자식 윈도우 리스트 검색
        {
            List<IntPtr> result = new List<IntPtr>();
            int ct = 0;
            IntPtr prevChild = IntPtr.Zero;
            IntPtr currChild = IntPtr.Zero;
            while (true && ct < maxCount)
            {
                currChild = FindWindowEx(hParent, prevChild, null, null);
                if (currChild == IntPtr.Zero) break;
                result.Add(currChild);
                prevChild = currChild;
                ++ct;
            }
            return result;
        }

        /** 
         * @brief 타이틀바에 표시된 타이틀과 일치하는 윈도우를 검색합니다.
         * @param title 검색할 타이틀 텍스트
         * @return 검색된 윈도우의 핸들포인터
         */
        public static IntPtr FindWindowByName(string title) // 타이틀로 윈도우 핸들 검색
        {
            return FindWindow(null, title);
        }

        /** 
         * @brief 실행파일명으로 실행중인 윈도우 핸들 검색
         * @param exName 검색할 실행파일명
         * @return 검색된 윈도우의 핸들포인터
         */
        public static IntPtr FindWindowByExname(string exName) // 실행파일명으로 윈도우 핸들 검색
        {
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.ProcessName.Contains(exName))
                {
                    return pList.MainWindowHandle;
                }
            }
            return IntPtr.Zero;
        }

        /** 
         * @brief 타이해당 타이틀의 실행중인 윈도 닫기 
         * @param WinName 검색할 타이틀 텍스트
         * @return 없음
         */
        public static void CloseWindow(string WinName) // CloseWindow(창타이틀) 해당 타이틀의 윈도 닫기 
        {
            IntPtr handle = FindWindow(null, WinName);
            if ((int)handle > 0)
            {
                PostMessage(handle, WM_CLOSE, 0, 0);
            }
        }

        /** 
         * @brief 타이해당 타이틀의 실행중인 윈도 닫기 
         * @param title 검색할 타이틀 텍스트
         * @return 없음
         */
        public static void CloseWindowByName(string title) // 이름으로 윈도 찾아 닫기
        {
            IntPtr mbWnd = FindWindowByName(title);
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, 0, 0);
        }

        /** 
         * @brief 실행파일명으로 실행중인 윈도 찾아 닫기
         * @param exName 검색할 실행파일명
         * @return 없음
         */
        public static void CloseWindowByExname(string exName) // 실행파일명으로 윈도 찾아 닫기
        {
            IntPtr mbWnd = FindWindowByExname(exName);
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, 0, 0);
        }

        /** 
         * @brief 지정경로 프로그램 실행
         * @param path 실행할 프로그램 경로
         * @return 없음
         */
        public static void RunProgram(string path) // 지정경로 프로그램 실행
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " : " + path);
                Console.WriteLine(e.StackTrace);
            }
        }

        #endregion

        /** 
         * @brief 시스템 시간을 설정. 
         *      관리자 권한으로 실행해야 작동한다.
         * @param format 시간값 표시 형식 문자열
         * @param time 변경할 시간값.
         *      format에서 지정한 형식과 일치해야 한다.
         * @return 없음
         */
        public static void SetTime(string format, string time) // 시스템 시간을 설정. 관리자 권한으로 실행해야 작동한다.
        {
            ushort wYear = 0;
            ushort wMonth = 0;
            ushort wDay = 0;
            ushort wHour = 0;
            ushort wMinute = 0;
            ushort wSecond = 0;
            ushort wMSecond = 0;

            try
            {
                if (format == "YYYYMMDDhhmmss" ||
                    format == "YYYYMMDDhhmmssff") // YYYYMMDDhhmmss(ff)
                {
                    wYear = ushort.Parse(time.Substring(0, 4));
                    wMonth = ushort.Parse(time.Substring(4, 2));
                    wDay = ushort.Parse(time.Substring(6, 2));
                    wHour = ushort.Parse(time.Substring(8, 2));
                    wMinute = ushort.Parse(time.Substring(10, 2));
                    wSecond = ushort.Parse(time.Substring(12, 2));
                    if (format == "YYYYMMDDhhmmssff")
                    {
                        wMSecond = ushort.Parse(time.Substring(14, 2));
                    }
                }

                DateTime tmpdate = new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond);
                tmpdate = tmpdate.AddHours(-9); //KOR timezone adjust 

                SYSTEMTIME st = new SYSTEMTIME();
                st.wDayOfWeek = (ushort)tmpdate.DayOfWeek;
                st.wMonth = (ushort)tmpdate.Month;
                st.wDay = (ushort)tmpdate.Day;
                st.wHour = (ushort)tmpdate.Hour;
                st.wMinute = (ushort)tmpdate.Minute;
                st.wSecond = (ushort)tmpdate.Second;
                st.wMilliseconds = (ushort)tmpdate.Millisecond;
                SetSystemTime(ref st);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }


        }

        /** 
         * @brief 외부 실행파일을 패널 안에 삽입한다.
         * @param panel 프로그램을 삽입할 부모 패널
         * @param exeName 실행할 프로그램 경로.
         *      Full Path로 입력해야 한다.
         * @param args 프로그램 실행시 argument
         * @param style 프로그램창 실행시 윈도우 형식
         * @return 없음
         */
        public static void OpenExeInPanel(Panel panel, string exeName, string args, ProcessWindowStyle style) // 외부 실행파일을 패널 안에 삽입한다.
        {
            var process = new Process();
            process.StartInfo.FileName = exeName;
            process.StartInfo.Arguments = args;
            process.StartInfo.WindowStyle = style;
            process.Start();

            IntPtr ptr = IntPtr.Zero;
            while ((ptr = process.MainWindowHandle) == IntPtr.Zero) ;
            FuncWin.SetParent(process.MainWindowHandle, panel.Handle);
            FuncWin.MoveWindow(process.MainWindowHandle, 0, 0, panel.Width, panel.Height, true);
        }

        /** 
         * @brief 외부 실행파일을 탭페이지 안에 삽입한다.
         * @param page 프로그램을 삽입할 부모 탭 페이지
         * @param exeName 실행할 프로그램 경로.
         *      Full Path로 입력해야 한다.
         * @param args 프로그램 실행시 argument
         * @param style 프로그램창 실행시 윈도우 형식
         * @return 없음
         */
        public static void OpenExeInTabPage(TabPage page, string exeName, string args, ProcessWindowStyle style) // 외부 실행파일을 패널 안에 삽입한다.
        {
            if (FindWindowByExname(exeName) != IntPtr.Zero)
            {
                return;
            }

            var process = new Process();
            process.StartInfo.FileName = exeName;
            process.StartInfo.Arguments = args;
            process.StartInfo.WindowStyle = style;
            process.Start();

            //IntPtr ptr = IntPtr.Zero;

            while ((ptr = process.MainWindowHandle) == IntPtr.Zero) ;
            FuncWin.SetParent(process.MainWindowHandle, page.Handle);
            FuncWin.MoveWindow(process.MainWindowHandle, 0, 0, page.Width, page.Height, true);
        }

        /** 
         * @brief 메시지 창을 출력하고 확인창이 닫힌 후 OK를 클릭했는지 확인한다.
         *      확인창이 표시되고 있는 동안은 해당 쓰레드는 정지 상태가 된다.
         * @param msg 확인창에 표시할 안내 메시지
         * @return OK 를 누르면 true
         *      Cancel이나 창을 강제로 닫았으면 false
         */
        public static bool MessageBoxOK(string msg) // 확인창에서 OK 눌렀나?
        {
            //return MessageBox.Show(msg, "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK;
            DialogForm dialog = new DialogForm(msg, "Notice", true);
            dialog.ShowYesNo();
            return dialog.YesClick;
        }

        public static bool MessageBoxOK(string msg, int screen) // 메시지박스 띄워서 OK/CANCEL 클릭 기다려서 OK클릭했는지 리턴
        {
            try
            {


                DialogMessage.DialogTask(msg, "Notice");
                return GlobalVar.MessageOK;


            }
            catch (Exception ex)
            {
                //debug(ex.ToString());
                //debug(ex.StackTrace);
            }
            return false;
        }

        /*
        public static bool MessageBoxOK(string msg, Form form) // 메시지박스 띄워서 OK/CANCEL 클릭 기다려서 OK클릭했는지 리턴
        {
            GlobalVar.MessageForm = form;
            return MessageBoxOK(msg, GlobalVar.ScreenNumbers[0]);
        }
        //*/

        /** 
         * @brief 뒤로 숨지 않는 메시지박스 
         *      검색된 메인스크린으로 출력한다.
         * @param msg 확인창에 표시할 안내 메시지
         * @return 없음
         */
        public static void TopMessageBox(string msg) // 뒤로 숨지 않는 메시지박스, 스크린 번호 지정하지 않으면 메인스크린으로
        {
            //return MessageBox.Show(new Form { TopMost = true }, msg);
            //(new MessageForm(msg, GlobalVar.ScreenNumbers[0])).Show();
            TopMessageBox(msg, GlobalVar.ScreenNumbers[0]);
        }

        /** 
         * @brief 지정한 스크린에 뒤로 숨지 않는 메시지박스를 출력한다.
         * @param msg 확인창에 표시할 안내 메시지
         * @param screen 검색된 스크린 순번. 0부터
         * @return 없음
         */
        public static void TopMessageBox(string msg, int screen) // 새 폼으로 일반 alert 메시지를 띄운다.
        {
            //DialogMessage.MessageTask(msg, "Notice");
            (new DialogForm(msg, "Notice", false)).ShowOk();



            //FuncWin.AutoClosingMessageBox(msg, "Notice", 15000);
            //MessageForm form = new MessageForm(msg, screen);
            //form.Show();
            //form.Focus();
            //Application.DoEvents();
        }

        public static DialogResult InputBox(string title, string content, ref string value)
        {
            Form form = new Form();
            PictureBox picture = new PictureBox();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.ClientSize = new Size(300, 250);
            form.Controls.AddRange(new Control[] { label, picture, textBox, buttonOk, buttonCancel });
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Font = new System.Drawing.Font("Calibri", 18F, System.Drawing.FontStyle.Bold);
            form.MaximizeBox = false;
            form.MinimizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            form.Text = title;
            picture.SizeMode = PictureBoxSizeMode.StretchImage;
            label.Text = content;
            textBox.Text = value;
            buttonOk.Text = "OK";
            buttonCancel.Text = "CANCEL";

            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            picture.SetBounds(10, 10, 50, 50);
            label.SetBounds(25, 17, 200, 120);
            textBox.SetBounds(25, 150, 220, 90);
            buttonOk.SetBounds(10, 200, 70, 40);
            buttonCancel.SetBounds(165, 200, 120, 40);

            DialogResult dialogResult = form.ShowDialog();

            value = textBox.Text;
            return dialogResult;
        }

    }
}
