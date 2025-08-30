using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;  // DLLImport
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

using System.Runtime.InteropServices;
namespace Radix
{
    /** @brief 가상키보드 키값 */
    public enum enumVirtualKey
    {
        VM_None = 0, //No virtual key value.
        VM_LeftButton = 1, //The left mouse button.
        VM_RightButton = 2, //The right mouse button.
        VM_Cancel = 3, //The cancel key or button
        VM_MiddleButton = 4, //The middle mouse button.
        VM_XButton1 = 5, //An additional "extended" device key or button (for example, an additional mouse button).
        VM_XButton2 = 6, //An additional "extended" device key or button (for example, an additional mouse button).
        VM_Back = 8, //The virtual back key or button.
        VM_Tab = 9, //The Tab key.
        VM_Clear = 12, //The Clear key or button.
        VM_Enter = 13, //The Enter key.
        VM_Shift = 16, //The Shift key. This is the general Shift case, applicable to key layouts with only one Shift key or that do not need to differentiate between left Shift and right Shift keystrokes.
        VM_Control = 17, //The Ctrl key. This is the general Ctrl case, applicable to key layouts with only one Ctrl key or that do not need to differentiate between left Ctrl and right Ctrl keystrokes.
        VM_Menu = 18, //The menu key or button.
        VM_Pause = 19, //The Pause key or button.
        VM_CapitalLock = 20, //The Caps Lock key or button.
        VM_Hangul = 21, //The Hangul symbol key-shift button.
        VM_Junja = 23, //The Junja symbol key-shift button.
        VM_Final = 24, //The Final symbol key-shift button.
        VM_Hanja = 25, //The Hanja symbol key shift button.
        VM_Escape = 27, //The Esc key.
        VM_Convert = 28, //The convert button or key.
        VM_NonConvert = 29, //The nonconvert button or key.
        VM_Accept = 30, //The accept button or key.
        VM_ModeChange = 31, //The mode change key.
        VM_Space = 32, //The Spacebar key or button.
        VM_PageUp = 33, //The Page Up key.
        VM_PageDown = 34, //The Page Down key.
        VM_End = 35, //The End key.
        VM_Home = 36, //The Home key.
        VM_Left = 37, //The Left Arrow key.
        VM_Up = 38, //The Up Arrow key.
        VM_Right = 39, //The Right Arrow key.
        VM_Down = 40, //The Down Arrow key.
        VM_Select = 41, //The Select key or button.
        VM_Print = 42, //The SV07_Print key or button.
        VM_Execute = 43, //The execute key or button.
        VM_Snapshot = 44, //The snapshot key or button.
        VM_Insert = 45, //The Insert key.
        VM_Delete = 46, //The Delete key.
        VM_Help = 47, //The Help key or button.
        VM_Number0 = 48, //The number "0" key.
        VM_Number1 = 49, //The number "1" key.
        VM_Number2 = 50, //The number "2" key.
        VM_Number3 = 51, //The number "3" key.
        VM_Number4 = 52, //The number "4" key.
        VM_Number5 = 53, //The number "5" key.
        VM_Number6 = 54, //The number "6" key.
        VM_Number7 = 55, //The number "7" key.
        VM_Number8 = 56, //The number "8" key.
        VM_Number9 = 57, //The number "9" key.
        VM_A = 65, //The letter "A" key.
        VM_B = 66, //The letter "B" key.
        VM_C = 67, //The letter "C" key.
        VM_D = 68, //The letter "D" key.
        VM_E = 69, //The letter "E" key.
        VM_F = 70, //The letter "F" key.
        VM_G = 71, //The letter "G" key.
        VM_H = 72, //The letter "H" key.
        VM_I = 73, //The letter "I" key.
        VM_J = 74, //The letter "J" key.
        VM_K = 75, //The letter "K" key.
        VM_L = 76, //The letter "L" key.
        VM_M = 77, //The letter "M" key.
        VM_N = 78, //The letter "N" key.
        VM_O = 79, //The letter "O" key.
        VM_P = 80, //The letter "P" key.
        VM_Q = 81, //The letter "Q" key.
        VM_R = 82, //The letter "R" key.
        VM_S = 83, //The letter "S" key.
        VM_T = 84, //The letter "T" key.
        VM_U = 85, //The letter "U" key.
        VM_V = 86, //The letter "V" key.
        VM_W = 87, //The letter "W" key.
        VM_X = 88, //The letter "X" key.
        VM_Y = 89, //The letter "Y" key.
        VM_Z = 90, //The letter "Z" key.
        VM_LeftWindows = 91, //The left Windows key.
        VM_RightWindows = 92, //The right Windows key.
        VM_Application = 93, //The application key or button.
        VM_Sleep = 95, //The sleep key or button.
        VM_NumberPad0 = 96, //The number "0" key as located on a numeric pad.
        VM_NumberPad1 = 97, //The number "1" key as located on a numeric pad.
        VM_NumberPad2 = 98, //The number "2" key as located on a numeric pad.
        VM_NumberPad3 = 99, //The number "3" key as located on a numeric pad.
        VM_NumberPad4 = 100, //The number "4" key as located on a numeric pad.
        VM_NumberPad5 = 101, //The number "5" key as located on a numeric pad.
        VM_NumberPad6 = 102, //The number "6" key as located on a numeric pad.
        VM_NumberPad7 = 103, //The number "7" key as located on a numeric pad.
        VM_NumberPad8 = 104, //The number "8" key as located on a numeric pad.
        VM_NumberPad9 = 105, //The number "9" key as located on a numeric pad.
        VM_Multiply = 106, //The multiply (*) operation key as located on a numeric pad.
        VM_Add = 107, //The add (+) operation key as located on a numeric pad.
        VM_Separator = 108, //The separator key as located on a numeric pad.
        VM_Subtract = 109, //The subtract (-) operation key as located on a numeric pad.
        VM_Decimal = 110, //The decimal (.) key as located on a numeric pad.
        VM_Divide = 111, //The divide (/) operation key as located on a numeric pad.
        VM_F1 = 112, //The F1 function key.
        VM_F2 = 113, //The F2 function key.
        VM_F3 = 114, //The F3 function key.
        VM_F4 = 115, //The F4 function key.
        VM_F5 = 116, //The F5 function key.
        VM_F6 = 117, //The F6 function key.
        VM_F7 = 118, //The F7 function key.
        VM_F8 = 119, //The F8 function key.
        VM_F9 = 120, //The F9 function key.
        VM_F10 = 121, //The F10 function key.
        VM_F11 = 122, //The F11 function key.
        VM_F12 = 123, //The F12 function key.
        VM_F13 = 124, //The F13 function key.
        VM_F14 = 125, //The F14 function key.
        VM_F15 = 126, //The F15 function key.
        VM_F16 = 127, //The F16 function key.
        VM_F17 = 128, //The F17 function key.
        VM_F18 = 129, //The F18 function key.
        VM_F19 = 130, //The F19 function key.
        VM_F20 = 131, //The F20 function key.
        VM_F21 = 132, //The F21 function key.
        VM_F22 = 133, //The F22 function key.
        VM_F23 = 134, //The F23 function key.
        VM_F24 = 135, //The F24 function key.
        VM_NavigationView = 136, //The navigation up button.
        VM_NavigationMenu = 137, //The navigation menu button.
        VM_NavigationUp = 138, //The navigation up button.
        VM_NavigationDown = 139, //The navigation down button.
        VM_NavigationLeft = 140, //The navigation left button.
        VM_NavigationRight = 141, //The navigation right button.
        VM_NavigationAccept = 142, //The navigation accept button.
        VM_NavigationCancel = 143, //The navigation cancel button.
        VM_NumberKeyLock = 144, //The Num Lock key.
        VM_Scroll = 145, //The Scroll Lock (ScrLk) key.
        VM_LeftShift = 160, //The left Shift key.
        VM_RightShift = 161, //The right Shift key.
        VM_LeftControl = 162, //The left Ctrl key.
        VM_RightControl = 163, //The right Ctrl key.
        VM_LeftMenu = 164, //The left menu key.
        VM_RightMenu = 165, //The right menu key.
        VM_GoBack = 166, //The go back key.
        VM_GoForward = 167, //The go forward key.
        VM_Refresh = 168, //The refresh key.
        VM_Stop = 169, //The stop key.
        VM_Search = 170, //The search key.
        VM_Favorites = 171, //The favorites key.
        VM_GoHome = 172, //The go home key.
        VM_GamepadA = 195, //The gamepad A button.
        VM_GamepadB = 196, //The gamepad B button.
        VM_GamepadX = 197, //The gamepad X button.
        VM_GamepadY = 198, //The gamepad Y button.
        VM_GamepadRightShoulder = 199, //The gamepad right shoulder.
        VM_GamepadLeftShoulder = 200, //The gamepad left shoulder.
        VM_GamepadLeftTrigger = 201, //The gamepad left trigger.
        VM_GamepadRightTrigger = 202, //The gamepad right trigger.
        VM_GamepadDPadUp = 203, //The gamepad d-pad up.
        VM_GamepadDPadDown = 204, //The gamepad d-pad down.
        VM_GamepadDPadLeft = 205, //The gamepad d-pad left.
        VM_GamepadDPadRight = 206, //The gamepad d-pad right.
        VM_GamepadMenu = 207, //The gamepad menu button.
        VM_GamepadView = 208, //The gamepad view button.
        VM_GamepadLeftThumbstickButton = 209, //The gamepad left thumbstick button.
        VM_GamepadRightThumbstickButton = 210, //The gamepad right thumbstick button.
        VM_GamepadLeftThumbstickUp = 211, //The gamepad left thumbstick up.
        VM_GamepadLeftThumbstickDown = 212, //The gamepad left thumbstick down.
        VM_GamepadLeftThumbstickRight = 213, //The gamepad left thumbstick right.
        VM_GamepadLeftThumbstickLeft = 214, //The gamepad left thumbstick left.
        VM_GamepadRightThumbstickUp = 215, //The gamepad right thumbstick up.
        VM_GamepadRightThumbstickDown = 216, //The gamepad right thumbstick down.
        VM_GamepadRightThumbstickRight = 217, //The gamepad right thumbstick right.
        VM_GamepadRightThumbstickLeft = 218 //The gamepad right thumbstick left.
    }



    /**
     * @brief 키보드 관련 함수 선언
     */
    class FuncKeyboard
    {
        /*
         * FuncKeyboard.cs : 키보드 관련 함수 선언
         */

        private const string OnScreenKeyboadApplication = "osk.exe";
        private const UInt32 WM_SYSCOMMAND = 0x112;
        private const UInt32 SC_RESTORE = 0xf120;
        private const UInt32 SC_CLOSE = 0xf060;
        private const Int32 WM_USER = 1024;
        private const Int32 WM_CSKEYBOARD = WM_USER + 192;
        private const Int32 WM_CSKEYBOARDMOVE = WM_USER + 193;
        private const Int32 WM_CSKEYBOARDRESIZE = WM_USER + 197;

        #region Vertual Keyboard 관련
        [DllImport("kernel32.dll", SetLastError = true)]
        /** 
         * @brief Disables file system redirection for the calling thread. File system redirection is enabled by default.
         * @param ptr The WOW64 file system redirection value. 
         *      The system uses this parameter to store information necessary to revert (re-enable) file system redirection.
         * @return IntPtr If the function succeeds, the return value is a nonzero value.
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
         * @return IntPtr If the function succeeds, the return value is a nonzero value.
         *      If the function fails, the return value is FALSE (zero). 
         *      To get extended error information, call GetLastError.
         */
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
        #endregion

        #region 키보드 관련
        [DllImport("user32.dll")]
        /** 
         * @brief Synthesizes a keystroke. 
         *      The system can use such a synthesized keystroke to generate a WM_KEYUP or WM_KEYDOWN message. 
         *      The keyboard driver's interrupt handler calls the keybd_event function.
         * @param vk A virtual-key code. 
         *      The code must be a value in the range 1 to 254. 
         *      For a complete list, see Virtual Key Codes.
         * @param scan A hardware scan code for the key.
         * @param flags Controls various aspects of function operation. 
         *      This parameter can be one or more of the following values.
         * @param extraInfo An additional value associated with the key stroke.
         * @return 없음
         */
        public static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo);

        [DllImport("user32.dll")]
        /** 
         * @brief Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code.
         * @param wCode The virtual key code or scan code for a key. 
         *      How this value is interpreted depends on the value of the uMapType parameter.
         *      Starting with Windows Vista, the high byte of the uCode value can contain either 0xe0 or 0xe1 to specify the extended scan code.
         * @param wMapType The translation to be performed. The value of this parameter depends on the value of the uCode parameter.
         * @return uint The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode and uMapType.
         *      If there is no translation, the return value is zero.
         */
        public static extern uint MapVirtualKey(int wCode, int wMapType);

        /** 
         * @brief 키보드 누른 이벤트를 발생시킨다.
         * @param key 가상키 코드
         * @return 없음
         */
        public static void Key_Down(enumVirtualKey key)
        {
            try
            {
                keybd_event((uint)key, GlobalVar.WM_KEYDOWN, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        /** 
         * @brief 지정 창핸들로 키보드 누른 이벤트를 발생시킨다.
         * @param handle 키보드 이벤트를 발생시킬 창 핸들
         * @param key 가상키 코드
         * @return 없음
         */
        public static void Key_Down(IntPtr handle, enumVirtualKey key)
        {
            FuncWin.SetForegroundWindow(handle);
            Key_Down(key);
        }

        /** 
         * @brief 키보드 떼는 이벤트를 발생시킨다.
         * @param handle 키보드 이벤트를 발생시킬 창 핸들
         * @return 없음
         */
        public static void Key_Up(enumVirtualKey key)
        {
            try
            {
                keybd_event((uint)key, GlobalVar.WM_KEYUP, 0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        /** 
         * @brief 지정 윈도우에 키보드 떼는 이벤트를 발생시킨다.
         * @param handle 키보드 이벤트를 발생시킬 창 핸들
         * @param key 가상키 코드
         * @return 없음
         */
        public static void Key_Up(IntPtr handle, enumVirtualKey key)
        {
            FuncWin.SetForegroundWindow(handle);
            Key_Up(key);
        }

        /** 
         * @brief 키보드 특정 키를 누르고 떼는 이벤트를 발생시킨다.
         * @param key 가상키 코드
         * @return 없음
         */
        public static void PressKey(enumVirtualKey key)
        {
            Key_Down(key);
            Thread.Sleep(50);
            Key_Up(key);
        }

        /** 
         * @brief 지정 윈도우에 키보드 특정 키를 누르고 떼는 이벤트를 발생시킨다.
         * @param handle 키보드 이벤트를 발생시킬 창 핸들
         * @param key 가상키 코드
         * @return 없음
         */
        public static void PressKey(IntPtr handle, enumVirtualKey key)
        {
            FuncWin.SetForegroundWindow(handle);
            PressKey(key);
        }

        public static void CloseKeyboard() // 가상 키보드 닫기
        {
            try
            {
                string processName = System.IO.Path.GetFileNameWithoutExtension(OnScreenKeyboadApplication);

                // Check whether the application is not running 
                var query = from process in Process.GetProcesses()
                            where process.ProcessName == processName
                            select process;

                var keyboardProcess = query.FirstOrDefault();
                if (keyboardProcess != null &&
                    keyboardProcess.MainWindowHandle != IntPtr.Zero)
                {
                    FuncWin.PostMessage(keyboardProcess.MainWindowHandle, WM_SYSCOMMAND, (int)(new IntPtr(SC_CLOSE)), (int)(new IntPtr(0)));
                }
                else if (keyboardProcess != null &&
                    keyboardProcess.Handle != IntPtr.Zero)
                {
                    FuncWin.PostMessage(keyboardProcess.Handle, WM_SYSCOMMAND, (int)(new IntPtr(SC_CLOSE)), (int)(new IntPtr(0)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }

        /** 
         * @brief 윈도우의 가상키보드 화면을 띄운다.
         * @return 없음
         */
        public static void OpenKeyboard() // 가상 키보드 열기
        {
            try
            {
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
                    FuncWin.SendMessage(windowHandle, WM_SYSCOMMAND, (int)(new IntPtr(SC_RESTORE)), (int)(new IntPtr(0)));
                }

                //if (keyboardProcess.MainWindowHandle != null)
                //{
                //    PostMessage(keyboardProcess.MainWindowHandle.ToInt32(), WM_CSKEYBOARDMOVE, 0, 700); // Move to 0, 0
                //    PostMessage(keyboardProcess.MainWindowHandle.ToInt32(), WM_CSKEYBOARDRESIZE, 600, 300); // Resize to 600, 300
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        }


        #endregion

    }
}
