using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

/**
 * @brief 가상키보드 관련 함수 선언
 */

class VirtualKB
{
    [DllImport("kernel32.dll", SetLastError = true)]
    /**
     * @brief Disables file system redirection for the calling thread. File system redirection is enabled by default.
     * @param ptr The WOW64 file system redirection value. 
     *      The system uses this parameter to store information necessary to revert (re-enable) file system redirection.
     * @return bool If the function succeeds, the return value is a nonzero value.
     *      If the function fails, the return value is zero. 
     *      To get extended error information, call GetLastError.
     */
    private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
    [DllImport("kernel32.dll", SetLastError = true)]
    /**
     * @brief Restores file system redirection for the calling thread.
     *      This function should not be called without a previous call to the Wow64DisableWow64FsRedirection function.
     *      Any data allocation on behalf of the Wow64DisableWow64FsRedirection function is cleaned up by this function.
     * @param ptr The WOW64 file system redirection value. 
     *      This value is obtained from the Wow64DisableWow64FsRedirection function.
     * @return bool If the function succeeds, the return value is a nonzero value.
     *      If the function fails, the return value is FALSE (zero). 
     *      To get extended error information, call GetLastError.
     */
    public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

    private const UInt32 WM_SYSCOMMAND = 0x112;
    private const UInt32 SC_RESTORE = 0xf120;
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
     * @return IntPtr 반환 값은 메시지 처리의 결과를 지정합니다. 전송된 메시지에 따라 달라집니다.
     */
    static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    private const string OnScreenKeyboadApplication = "osk.exe";

    /**
     * @brief 윈도우 내장 가상 키보드인 osk.exe를 실행합니다.
     * @return void
     */
    static public void Open()
    {
        // Get the name of the On screen keyboard
        string processName = System.IO.Path.GetFileNameWithoutExtension(OnScreenKeyboadApplication);

        // Check whether the application is not running 
        var query = from process in Process.GetProcesses()
                    where process.ProcessName == processName
                    select process;

        var keyboardProcess = query.FirstOrDefault();

        // launch it if it doesn't exist
        if (keyboardProcess == null)
        {
            IntPtr ptr = new IntPtr(); ;
            bool sucessfullyDisabledWow64Redirect = false;

            // Disable x64 directory virtualization if we're on x64,
            // otherwise keyboard launch will fail.
            if (System.Environment.Is64BitOperatingSystem)
            {
                sucessfullyDisabledWow64Redirect = Wow64DisableWow64FsRedirection(ref ptr);
            }

            // osk.exe is in windows/system folder. So we can directky call it without path
            using (Process osk = new Process())
            {
                osk.StartInfo.FileName = OnScreenKeyboadApplication;
                osk.Start();
            }

            // Re-enable directory virtualisation if it was disabled.
            if (System.Environment.Is64BitOperatingSystem)
                if (sucessfullyDisabledWow64Redirect)
                    Wow64RevertWow64FsRedirection(ptr);
        }
        else
        {
            // Bring keyboard to the front if it's already running
            var windowHandle = keyboardProcess.MainWindowHandle;
            SendMessage(windowHandle, WM_SYSCOMMAND, new IntPtr(SC_RESTORE), new IntPtr(0));
        }
    }
}
