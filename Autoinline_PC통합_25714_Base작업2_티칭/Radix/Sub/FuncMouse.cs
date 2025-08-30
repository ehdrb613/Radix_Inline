using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;  // DLLImport
using Microsoft.Win32;

namespace Radix
{
    /** @brief 마우스 동작 및 체크 함수 선언 */
    class FuncMouse
    {
        /*
         * FuncMouse.cs : 마우스 동작 및 체크 함수 선언
         */

        #region 마우스 관련
        #region API 함수 Import
        [DllImport("user32.dll")]
        /** 
         * @brief Retrieves the position of the mouse cursor, in screen coordinates.
         * @param lpPoint A pointer to a POINT structure that receives the screen coordinates of the cursor.
         * @return bool Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.
         */
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("User32.Dll")]
        /** 
         * @brief Moves the cursor to the specified screen coordinates. 
         *      If the new coordinates are not within the screen rectangle set by the most recent ClipCursor function call, 
         *      the system automatically adjusts the coordinates so that the cursor stays within the rectangle.
         * @param x The new x-coordinate of the cursor, in screen coordinates.
         * @param y The new y-coordinate of the cursor, in screen coordinates.
         * @return long Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.
         */
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.Dll")]
        /** 
         * @brief The ClientToScreen function converts the client-area coordinates of a specified point to screen coordinates.
         * @param hWnd A handle to the window whose client area is used for the conversion.
         * @param point A pointer to a POINT structure that contains the client coordinates to be converted. 
         *      The new screen coordinates are copied into this structure if the function succeeds.
         * @return bool If the function succeeds, the return value is nonzero.
         *      If the function fails, the return value is zero.
         */
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        /** 
         * @brief The mouse_event function synthesizes mouse motion and button clicks.
         * @param dwFlags Controls various aspects of mouse motion and button clicking. 
         *      This parameter can be certain combinations of the following values.
         *      The values that specify mouse button status are set to indicate changes in status, not ongoing conditions. 
         *      For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, 
         *      but not for subsequent motions. 
         *      Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released.
         *      You cannot specify both MOUSEEVENTF_WHEEL and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP simultaneously in the dwFlags parameter,
         *      because they both require use of the dwData field.
         * @param dx The mouse's absolute position along the x-axis or its amount of motion since the last mouse event was generated, 
         *      depending on the setting of MOUSEEVENTF_ABSOLUTE. 
         *      Absolute data is specified as the mouse's actual x-coordinate; relative data is specified as the number of mickeys moved. 
         *      A mickey is the amount that a mouse has to move for it to report that it has moved.
         * @param dy The mouse's absolute position along the y-axis or its amount of motion since the last mouse event was generated, 
         *      depending on the setting of MOUSEEVENTF_ABSOLUTE. 
         *      Absolute data is specified as the mouse's actual y-coordinate; 
         *      relative data is specified as the number of mickeys moved.
         * @param cButtons f dwFlags contains MOUSEEVENTF_WHEEL, then dwData specifies the amount of wheel movement. 
         *      A positive value indicates that the wheel was rotated forward, away from the user; 
         *      a negative value indicates that the wheel was rotated backward, toward the user. 
         *      One wheel click is defined as WHEEL_DELTA, which is 120.
         *      If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement. 
         *      A positive value indicates that the wheel was tilted to the right; a negative value indicates that the wheel was tilted to the left.
         *      If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then dwData specifies which X buttons were pressed or released. 
         *      This value may be any combination of the following flags.
         *      If dwFlags is not MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then dwData should be zero.
         * @param dwExtraInfo An additional value associated with the mouse event. 
         *      An application calls GetMessageExtraInfo to obtain this extra information.
         * @return 없음
         */
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, int cButtons, int dwExtraInfo);
        #endregion

        #region 지역 상수
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; //The left button is down.
        private const int MOUSEEVENTF_LEFTUP = 0x0004; //The left button is up.
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //The middle button is down.
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040; //The middle button is up.
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; //The right button is down.
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; //The right button is up.
        private const int MOUSEEVENTF_MOVE = 0x0001; //Movement occurred.
        private const int MOUSEEVENTF_WHEEL = 0x0800; //The wheel has been moved, if the mouse has a wheel. The amount of movement is specified in dwData
        private const int MOUSEEVENTF_XDOWN = 0x0080; //An X button was pressed.
        private const int MOUSEEVENTF_XUP = 0x0100; //An X button was released.
        private const int MOUSEEVENTF_HWHEEL = 0x01000; //The wheel button is tilted.
        #endregion

        #region 구조체
        [StructLayout(LayoutKind.Sequential)]
        /** @brief 마우스 동작 등에 사용되는 좌표계 */
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        /** @brief 영역의 위치 및 크기 */
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        #endregion

        /** 
         * @brief 현재 마우스의 커서 좌표 가져오기 
         * @return POINT 마우스 커서의 좌표
         */
        public static POINT GetMousePos() // 현재 마우스의 좌표 가져오기
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        /** 
         * @brief 지정 좌표로 마우스 이동
         * @param x 이동할 x 좌표
         * @param y 이동할 y 좌표
         * @return 없음
         */
        public static void MouseMove(uint x, uint y) // 지정 좌표로 마우스 이동
        {
            Console.WriteLine("mouse move");
            POINT p = new POINT();
            p.x = Convert.ToInt16(x);
            p.y = Convert.ToInt16(y);

              SetCursorPos(p.x, p.y);
        }

        /** 
         * @brief 지정 창 지정 좌표로 마우스 이동
         * @param handle 이동할 창의 핸들
         * @param x 이동할 x 좌표
         * @param y 이동할 y 좌표
         * @return 없음
         */
        public static void MouseMove(IntPtr handle, uint x, uint y) // 지정 창 지정 좌표로 마우스 이동
        {
            Console.WriteLine("move mouse");
            POINT p = new POINT();
            p.x = Convert.ToInt16(x);
            p.y = Convert.ToInt16(y);

            ClientToScreen(handle, ref p);
            SetCursorPos(p.x, p.y);
        }

        /** 
         * @brief 현재 커서 위치에 마우스 좌클릭
         * @return 없음
         */
        public static void MouseLeftClick() // 마우스 좌클릭
        {
            //Call the imported function with the cursor's current position
            POINT p = GetMousePos();
            uint X = (uint)p.x;
            uint Y = (uint)p.y;
            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        /** 
         * @brief 지정 좌표에 마우스 우클릭
         * @param X 클릭할 x 좌표
         * @param Y 클릭할 y 좌표
         * @return 없음
         */
        public static void MouseLeftClick(uint X, uint Y) // 마우스 우클릭
        {
            MouseMove(X, Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        /** 
         * @brief 지정 스크린 해당 좌표 마우스 좌클릭
         * @param handle 스크린의 핸들
         * @param x 클릭할 x 좌표
         * @param y 클릭할 y 좌표
         * @return 없음
         */
        public static void MouseLeftClick(IntPtr handle, uint x, uint y) // 해당 좌표 마우스 좌클릭
        {
            FuncWin.SetForegroundWindow(handle);
            MouseMove(handle, x, y);
            /*
            //Call the imported function with the cursor's current position
            int lParam = ((int)y << 16) | (int)x;
            //FuncWin.PostMessage((int)handle, MOUSEEVENTF_LEFTDOWN, 0, lParam);
            //FuncWin.PostMessage((int)handle, MOUSEEVENTF_LEFTUP, 0, lParam);
            FuncWin.PostMessage(handle, MOUSEEVENTF_LEFTDOWN, 0, 0);
            Thread.Sleep(50);
            FuncWin.PostMessage(handle, MOUSEEVENTF_LEFTUP, 0, 0);
            //*/
            MouseLeftClick();
        }

        /** 
         * @brief 마우스 드레그
         * @param sx 드래그 시작 x 좌표
         * @param sy 드래그 시작 y 좌표
         * @param ex 드래그 종료 x 좌표
         * @param ey 드래그 종료 y 좌표
         * @return 없음
         */
        public static void MouseLeftDrag(uint sx, uint sy, uint ex, uint ey) // 마우스 드레그
        {
            MouseMove(sx, sy);
            mouse_event(MOUSEEVENTF_LEFTDOWN, sx, sy, 0, 0);
            MouseMove(ex, ey);
            mouse_event(MOUSEEVENTF_LEFTUP, ex, ey, 0, 0);
        }

        /** 
         * @brief 지정창 마우스 드레그
         * @param haldle 드래그할 창 핸들
         * @param sx 드래그 시작 x 좌표
         * @param sy 드래그 시작 y 좌표
         * @param ex 드래그 종료 x 좌표
         * @param ey 드래그 종료 y 좌표
         * @return 없음
         */
        public static void MouseLeftDrag(IntPtr handle, uint sx, uint sy, uint ex, uint ey) // 마우스 드레그
        {
            MouseMove(handle, sx, sy);
            POINT p = GetMousePos();
            mouse_event(MOUSEEVENTF_LEFTDOWN, (uint)p.x, (uint)p.y, 0, 0);
            MouseMove(handle, ex, ey);
            p = GetMousePos();
            mouse_event(MOUSEEVENTF_LEFTUP, (uint)p.x, (uint)p.y, 0, 0);
        }

        /*
        public static void MouseLeftClick(IntPtr handle)
        {
            //Call the imported function with the cursor's current position
            POINT p = GetMousePos();
            int lParam = (p.y << 16) | p.x;
            FuncWin.PostMessage(handle, MOUSEEVENTF_LEFTDOWN, 0, lParam);
            FuncWin.PostMessage(handle, MOUSEEVENTF_LEFTUP, 0, lParam);
        }
        //*/

        /*
        public static void MouseLeftClick(int x, int y)
        {
            MouseMove(y, y);
            MouseLeftClick();
        }
        //*/

        /** 
         * @brief 현재 커서 위치 마우스 중간 버튼 클릭
         * @return 없음
         */
        public static void MouseMiddleClick() // 마우스 중간 버튼 클릭
        {
            //Call the imported function with the cursor's current position
            POINT p = GetMousePos();
            uint X = (uint)p.x;
            uint Y = (uint)p.y;
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_MIDDLEUP, X, Y, 0, 0);
        }

        /** 
         * @brief 지정 좌표 마우스 중간 버튼 클릭
         * @param x 클릭할 x 좌표 
         * @param y 클릭할 y 좌표
         * @return 없음
         */
        public static void MouseMiddleClick(uint x, uint y) // 지정 좌표 마우스 중간버튼 클릭
        {
            MouseMove(y, y);
            MouseMiddleClick();
        }

        /** 
         * @brief 지정 화면 지정좌표 마우스 중간버튼 클릭
         * @param handle 클릭할 창 핸들
         * @param x 클릭할 x 좌표 
         * @param y 클릭할 y 좌표
         * @return 없음
         */
        public static void MouseMiddleClick(IntPtr handle, uint x, uint y) // 지정 화면 지정좌표 마우스 중간버튼 클릭
        {
            FuncWin.SetForegroundWindow(handle);
            MouseMove(handle, y, y);
            MouseMiddleClick();
        }

        /** 
         * @brief 마우스 중간버튼 드레그
         * @param sx 드래그 시작 x 좌표 
         * @param sy 드래그 시작 y 좌표
         * @param ex 드래그 종료 x 좌표 
         * @param ey 드래그 종료 y 좌표
         * @return 없음
         */
        public static void MouseMiddleDrag(uint sx, uint sy, uint ex, uint ey) // 마우스 중간버튼 드레그
        {
            MouseMove(sx, sy);
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, sx, sy, 0, 0);
            MouseMove(ex, ey);
            mouse_event(MOUSEEVENTF_MIDDLEUP, ex, ey, 0, 0);
        }

        /** 
         * @brief 지정창 마우스 중간버튼 드레그
         * @param handle 드래그할 창 핸들
         * @param sx 드래그 시작 x 좌표 
         * @param sy 드래그 시작 y 좌표
         * @param ex 드래그 종료 x 좌표 
         * @param ey 드래그 종료 y 좌표
         * @return 없음
         */
        public static void MouseMiddleDrag(IntPtr handle, uint sx, uint sy, uint ex, uint ey) // 지정창 마우스 중간버튼 드레그
        {
            MouseMove(handle, sx, sy);
            POINT p = GetMousePos();
            mouse_event(MOUSEEVENTF_MIDDLEDOWN, (uint)p.x, (uint)p.y, 0, 0);
            MouseMove(handle, ex, ey);
            p = GetMousePos();
            mouse_event(MOUSEEVENTF_MIDDLEUP, (uint)p.x, (uint)p.y, 0, 0);
        }

        /** 
         * @brief 현재 커서 위치 마우스 우클릭
         * @return 없음
         */
        public static void MouseRightClick() // 마우스 우클릭
        {
            //Call the imported function with the cursor's current position
            POINT p = GetMousePos();
            uint X = (uint)p.x;
            uint Y = (uint)p.y;
            mouse_event(MOUSEEVENTF_RIGHTDOWN, X, Y, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
        }

        /** 
         * @brief 지정좌표 마우스 우클릭
         * @param x 클릭할 x 좌표
         * @param y 클릭할 y 좌표
         * @return 없음
         */
        public static void MouseRightClick(uint x, uint y) // 지정좌표 마우스 우클릭
        {
            MouseMove(y, y);
            MouseRightClick();
        }

        /** 
         * @brief 지정창 지정좌표 마우스 우클릭
         * @param handle 클릭할 창 핸들
         * @param x 클릭할 x 좌표
         * @param y 클릭할 y 좌표
         * @return 없음
         */
        public static void MouseRightClick(IntPtr handle, uint x, uint y) // 지정창 지정좌표 마우스 우클릭
        {
            FuncWin.SetForegroundWindow(handle);
            MouseMove(handle, y, y);
            MouseRightClick();
        }

        /** 
         * @brief 마우스 우버튼 드레그
         * @param sx 드래그 시작 x 좌표
         * @param sy 드래그 시작 y 좌표
         * @param ex 드래그 종료 x 좌표
         * @param ey 드래그 종료 y 좌표
         * @return 없음
         */
        public static void MouseRightDrag(uint sx, uint sy, uint ex, uint ey) // 마우스 우버튼 드레그
        {
            MouseMove(sx, sy);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, sx, sy, 0, 0);
            MouseMove(ex, ey);
            mouse_event(MOUSEEVENTF_RIGHTUP, ex, ey, 0, 0);
        }

        /** 
         * @brief 지정창 마우스 우버튼 드레그
         * @param handle 드래그할 창 핸들
         * @param sx 드래그 시작 x 좌표
         * @param sy 드래그 시작 y 좌표
         * @param ex 드래그 종료 x 좌표
         * @param ey 드래그 종료 y 좌표
         * @return 없음
         */
        public static void MouseRightDrag(IntPtr handle, uint sx, uint sy, uint ex, uint ey) // 지정창 마우스 우버튼 드레그
        {
            MouseMove(handle, sx, sy);
            POINT p = GetMousePos();
            mouse_event(MOUSEEVENTF_RIGHTDOWN, (uint)p.x, (uint)p.y, 0, 0);
            MouseMove(handle, ex, ey);
            p = GetMousePos();
            mouse_event(MOUSEEVENTF_RIGHTUP, (uint)p.x, (uint)p.y, 0, 0);
        }

        /** 
         * @brief 마우스 휠 스크롤
         * @param s 스크롤할 거리
         * @return 없음
         */
        public static void MouseScroll(int s) // 마우스 휠 스크롤
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, s, 0);
        }
        #endregion
    }
}
