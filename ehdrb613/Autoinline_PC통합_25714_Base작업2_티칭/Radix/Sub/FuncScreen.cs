using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Runtime.InteropServices;  // DLLImport

namespace Radix
{

    #region 구조체 선언
    /** 
     * @brief 색상값
     *      r : 적색
     *      g : 초록색
     *      b : 청색
     */
    public struct TColor
    {
        public int r;
        public int g;
        public int b;
    }

    //This structure shall be used to keep the size of the screen.
    /** 
     * @brief 2차원 사이즈
     *      cx : x 방향 크기
     *      cy : y 방향 크기
     */
    public struct SIZE
    {
        public int cx;
        public int cy;
    }
    #endregion


    ///
    /// This class shall keep the GDI32 APIs used in our program.
    ///
    /** 
     * @brief 스크린 처리 관련 함수 선언
     */
    public static class FuncScreen
    {
        /*
         * FuncScreen.cs : 스크린 처리 관련 함수 선언
         */
  
  
        #region 스캔 관련
        #region USER32 ASI 함수 Import
        [DllImport("USER32.DLL")]
        /** 
         * @brief The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen. 
         *      You can use the returned handle in subsequent GDI functions to draw in the DC. 
         *      The device context is an opaque data structure, whose values are used internally by GDI.
         * @param hWnd A handle to the window whose DC is to be retrieved. 
         *      If this value is NULL, GetDC retrieves the DC for the entire screen.
         * @return IntPtr If the function succeeds, the return value is a handle to the DC for the specified window's client area.
         *      If the function fails, the return value is NULL.
         */
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        /** 
         * @brief The ReleaseDC function releases a device context (DC), freeing it for use by other applications. 
         *      The effect of the ReleaseDC function depends on the type of DC. 
         *      It frees only common and window DCs. 
         *      It has no effect on class or private DCs.
         * @param hWnd A handle to the window whose DC is to be released.
         * @param hdc A handle to the DC to be released.
         * @return IntPtr The return value indicates whether the DC was released. 
         *      If the DC was released, the return value is 1.
         *      If the DC was not released, the return value is zero.
         */
        public static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("user32.dll")]
        /** 
         * @brief 바탕 화면 창에 대한 핸들을 검색합니다. 바탕 화면 창은 전체 화면을 덮습니다. 
         *      바탕 화면 창은 다른 창이 그려지는 위쪽 영역입니다.
         * @return 반환 값은 바탕 화면 창에 대한 핸들입니다.
         */
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        /** 
         * @brief The GetWindowDC function retrieves the device context (DC) for the entire window, including title bar, menus, and scroll bars. 
         *      A window device context permits painting anywhere in a window, because the origin of the device context is the upper-left corner of the window instead of the client area.
         *      GetWindowDC assigns default attributes to the window device context each time it retrieves the device context.
         *       Previous attributes are lost.
         * @param hWnd A handle to the window with a device context that is to be retrieved. 
         *      If this value is NULL, GetWindowDC retrieves the device context for the entire screen.
         *      If this parameter is NULL, GetWindowDC retrieves the device context for the primary display monitor. 
         *      To get the device context for other display monitors, use the EnumDisplayMonitors and CreateDC functions.
         * @return IntPtr If the function succeeds, the return value is a handle to a device context for the specified window.
         *      If the function fails, the return value is NULL, indicating an error or an invalid hWnd parameter.
         */
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        /** 
         * @brief The GetPixel function retrieves the red, green, blue (RGB) color value of the pixel at the specified coordinates.
         * @param hDC A handle to the device context.
         * @param x The x-coordinate, in logical units, of the pixel to be examined.
         * @param y The y-coordinate, in logical units, of the pixel to be examined.
         * @return int The return value is the COLORREF value that specifies the RGB of the pixel. 
         *      If the pixel is outside of the current clipping region, the return value is CLR_INVALID (0xFFFFFFFF defined in Wingdi.h).
         */
        public static extern int GetPixcel(IntPtr hDC, int x, int y);

        [DllImport("USER32.DLL")]
        /** 
         * @brief The SetPixel function sets the pixel at the specified coordinates to the specified color.
         * @param hDC A handle to the device context.
         * @param x The x-coordinate, in logical units, of the pixel to be examined.
         * @param y The y-coordinate, in logical units, of the pixel to be examined.
         * @param color The color to be used to paint the point. To create a COLORREF color value, use the RGB macro.
         * @return int If the function succeeds, the return value is the RGB value that the function sets the pixel to. 
         *      This value may differ from the color specified by crColor; that occurs when an exact match for the specified color cannot be found.
         *      If the function fails, the return value is -1.
         */
        public static extern int SetPixel(IntPtr hDC, int x, int y, int color);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        /** 
         * @brief 지정된 시스템 메트릭 또는 시스템 구성 설정을 검색합니다.
         *      GetSystemMetrics에서 검색된 모든 차원은 픽셀 단위입니다.
         * @param abc 검색할 시스템 메트릭 또는 구성 설정입니다. 
         *      이 매개 변수는 다음 값 중 하나일 수 있습니다. 
         *      모든 SM_CX* 값은 너비이고 모든 SM_CY* 값은 높이입니다. 
         *      또한 부울 데이터를 반환하도록 설계된 모든 설정은 TRUE 를 0이 아닌 값으로, FALSE 를 0 값으로 나타냅니다.
         * @return int 함수가 성공하면 반환 값은 요청된 시스템 메트릭 또는 구성 설정입니다.
         *      함수가 실패하면 반환 값은 0입니다. 
         *      GetLastError 는 확장된 오류 정보를 제공하지 않습니다.
         */
        public static extern int GetSystemMetrics(int abc);
        #endregion

        #region GDI32 API 함수 Import
        [DllImport("gdi32.dll")]
        /** 
         * @brief The BitBlt function performs a bit-block transfer of the color data corresponding to a rectangle of pixels 
         *      from the specified source device context into a destination device context.
         * @param hObject A handle to the destination device context.
         * @param nXDest The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.
         * @param nYDest The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.
         * @param nWidth The width, in logical units, of the source and destination rectangles.
         * @param nHeight The height, in logical units, of the source and the destination rectangles.
         * @param hObjectSource A handle to the source device context.
         * @param nXSrc The x-coordinate, in logical units, of the upper-left corner of the source rectangle.
         * @param nYSrc The y-coordinate, in logical units, of the upper-left corner of the source rectangle.
         * @param dwRop A raster-operation code. 
         *      These codes define how the color data for the source rectangle is to be combined with the color data for the destination rectangle to achieve the final color.
         * @return bool If the function succeeds, the return value is nonzero.
         *      If the function fails, the return value is zero. 
         *      To get extended error information, call GetLastError.
         */
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.DLL")]
        /** 
         * @brief The StretchDIBits function copies the color data for a rectangle of pixels in a DIB, JPEG, or PNG image to the specified destination rectangle. 
         *      If the destination rectangle is larger than the source rectangle, this function stretches the rows and columns of color data to fit the destination rectangle. 
         *      If the destination rectangle is smaller than the source rectangle, this function compresses the rows and columns by using the specified raster operation.
         * @param hdc A handle to the destination device context.
         * @param x The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.
         * @param y The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.
         * @param nWidth The width, in logical units, of the destination rectangle.
         * @param nHeight The height, in logical units, of the destination rectangle.
         * @param hSrcDC A handle to the source device context.
         * @param nXSrc The x-coordinate, in logical units, of the upper-left corner of the source rectangle.
         * @param nYSrc The y-coordinate, in logical units, of the upper-left corner of the source rectangle.
         * @param nSrcWidth The width, in logical units, of the source rectangle.
         * @param nSrcHeight The height, in logical units, of the source rectangle.
         * @param dwRop The raster operation to be performed. Raster operation codes define how the system combines colors in output operations that involve a brush, a source bitmap, and a destination bitmap.
         *      See BitBlt for a list of common raster operation codes (ROPs). 
         *      Note that the CAPTUREBLT ROP generally cannot be used for printing device contexts.
         * @return int If the function succeeds, the return value is nonzero.
         *      If the function fails, the return value is zero.
         */
        public static extern int StretchBlt(IntPtr hdc, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int nSrcWidth, int nSrcHeight, int dwRop);

        [DllImport("gdi32.dll")]
        /** 
         * @brief The CreateCompatibleBitmap function creates a bitmap compatible with the device that is associated with the specified device context.
         * @param hdc A handle to a device context.
         * @param nWidth The bitmap width, in pixels.
         * @param nHeight The bitmap height, in pixels.
         * @return IntPtr If the function succeeds, the return value is a handle to the compatible bitmap (DDB).
         *      If the function fails, the return value is NULL.
         */
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        /** 
         * @brief The CreateCompatibleDC function creates a memory device context (DC) compatible with the specified device.
         * @param hdc A handle to an existing DC. 
         *      If this handle is NULL, the function creates a memory DC compatible with the application's current screen.
         * @return IntPtr If the function succeeds, the return value is the handle to a memory DC.
         *      If the function fails, the return value is NULL.
         */
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        /** 
         * @brief The DeleteDC function deletes the specified device context (DC).
         * @param hdc A handle to an existing DC. 
         *      If this handle is NULL, the function creates a memory DC compatible with the application's current screen.
         * @return bool If the function succeeds, the return value is nonzero.
         *      If the function fails, the return value is zero.
         */
        public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        /** 
         * @brief The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object. 
         *      After the object is deleted, the specified handle is no longer valid.
         * @param hObject A handle to a logical pen, brush, font, bitmap, region, or palette.
         * @return bool If the function succeeds, the return value is nonzero.
         *      If the specified handle is not valid or is currently selected into a DC, the return value is zero.
         */
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        /** 
         * @brief The SelectObject function selects an object into the specified device context (DC). 
         *      The new object replaces the previous object of the same type.
         * @param hDC A handle to the DC.
         * @param hObject A handle to the object to be selected. 
         *      The specified object must have been created by using one of the following functions.
         * @return IntPtr If the selected object is not a region and the function succeeds, the return value is a handle to the object being replaced.
         *      If the selected object is a region and the function succeeds, the return value is one of the following values.
         */
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        #endregion

        /** 
         * @brief The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen. 
         *      You can use the returned handle in subsequent GDI functions to draw in the DC. 
         *      The device context is an opaque data structure, whose values are used internally by GDI.
         * @param handle A handle to the window whose DC is to be retrieved. 
         *      If this value is NULL, GetDC retrieves the DC for the entire screen.
         * @return IntPtr If the function succeeds, the return value is a handle to the DC for the specified window's client area.
         *      If the function fails, the return value is NULL.
         */
        public static IntPtr InitDC(IntPtr handle)
        {
            return GetDC(handle);
        }

        /** 
         * @brief The ReleaseDC function releases a device context (DC), freeing it for use by other applications. 
         *      The effect of the ReleaseDC function depends on the type of DC. 
         *      It frees only common and window DCs. 
         *      It has no effect on class or private DCs.
         * @param handle A handle to the window whose DC is to be released.
         * @param hdc A handle to the DC to be released.
         * @return IntPtr The return value indicates whether the DC was released. 
         *      If the DC was released, the return value is 1.
         *      If the DC was not released, the return value is zero.
         */
        public static void EndDC(IntPtr handle, IntPtr dc)
        {
            ReleaseDC(handle, dc);
        }

        /** 
         * @brief The GetWindowDC function retrieves the device context (DC) for the entire window, including title bar, menus, and scroll bars. 
         *      A window device context permits painting anywhere in a window, because the origin of the device context is the upper-left corner of the window instead of the client area.
         *      GetWindowDC assigns default attributes to the window device context each time it retrieves the device context.
         *       Previous attributes are lost.
         * @param handle A handle to the window with a device context that is to be retrieved. 
         *      If this value is NULL, GetWindowDC retrieves the device context for the entire screen.
         *      If this parameter is NULL, GetWindowDC retrieves the device context for the primary display monitor. 
         *      To get the device context for other display monitors, use the EnumDisplayMonitors and CreateDC functions.
         * @return IntPtr If the function succeeds, the return value is a handle to a device context for the specified window.
         *      If the function fails, the return value is NULL, indicating an error or an invalid hWnd parameter.
         */
        public static IntPtr InitWinDC(IntPtr handle)
        {
            return GetWindowDC(handle);
        }


        #endregion




        #region Public Class Functions
        /** 
         * @brief DC 개체로부터 스크린 전체 캡쳐 수행
         * @param hDC 윈도우로부터 가져온 DC 핸들
         * @return Bitmap 캡쳐 결과물 비트맵
         */
        public static Bitmap ScanScreen(IntPtr hDC) // 스크린 캡쳐
        {
            return ScanScreen(hDC, 0, 0, GetSystemMetrics(GlobalVar.SM_CXSCREEN), GetSystemMetrics(GlobalVar.SM_CYSCREEN), 0, 0);
        }

        /** 
         * @brief DC 개체로부터 스크린 범위 지정 캡쳐 수행
         * @param hDC 윈도우로부터 가져온 DC 핸들
         * @param srcX 화면상의 캡쳐 시작 X 좌표
         * @param srcY 화면상의 캡쳐 시작 Y 좌표
         * @param width 화면상의 캡쳐할 폭
         * @param height 화면상의 캡쳐할 높이
         * @param destWidth 캡쳐할 비트맵의 폭
         * @param destHeight 캡쳐할 비트맵의 높이
         * @return Bitmap 캡쳐 결과물 비트맵
         */
        public static Bitmap ScanScreen(IntPtr hDC, int srcX, int srcY, int width, int height, int destWidth, int destHeight) // 스크린 범위 캡쳐
        {
            //In size variable we shall keep the size of the screen.
            SIZE size;

            //Variable to keep the handle to bitmap.
            IntPtr hBitmap;

            //Here we make a compatible device context in memory for screen
            //device context.
            IntPtr hMemDC;// = CreateCompatibleDC(hDC);

         
            try
            {
                hMemDC = CreateCompatibleDC(hDC);

                //We pass SM_CXSCREEN constant to GetSystemMetrics to get the
                //X coordinates of the screen.
                //size.cx = GetSystemMetrics(Globals.SM_CXSCREEN);
                size.cx = width;

                //We pass SM_CYSCREEN constant to GetSystemMetrics to get the
                //Y coordinates of the screen.
                //size.cy = GetSystemMetrics(Globals.SM_CYSCREEN);
                size.cy = height;

                //We create a compatible bitmap of the screen size and using
                //the screen device context.
                hBitmap = CreateCompatibleBitmap(hDC, size.cx, size.cy);

                //As hBitmap is IntPtr, we cannot check it against null.
                //For this purpose, IntPtr.Zero is used.

                if (hBitmap != IntPtr.Zero)
                {
                    //Here we select the compatible bitmap in the memeory device
                    //context and keep the refrence to the old bitmap.
                    IntPtr hOld = (IntPtr)SelectObject(hMemDC, hBitmap);
                    //We copy the Bitmap to the memory device context.
                    if (destWidth == 0 ||
                        destHeight == 0)
                    {
                        BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, srcX, srcY, GlobalVar.SRCCOPY);
                    }
                    else
                    {
                        StretchBlt(hMemDC, 0, 0, destWidth, destHeight, hDC, srcX, srcY, size.cx, size.cy, GlobalVar.SRCCOPY);
                    }

                    //We select the old bitmap back to the memory device context.
                    SelectObject(hMemDC, hOld);
                    //Image is created by Image bitmap handle and stored in
                    //local variable.
                    Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);
                    //Release the memory to avoid memory leaks.
                    DeleteObject(hBitmap);
                    //This statement runs the garbage collector manually.
                    GC.Collect();
                    //Return the bitmap 
                    return bmp;
                }
                //We delete the memory device context.
                DeleteDC(hMemDC);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.StackTrace);
            }
        
            //If hBitmap is null, retun null.
            return null;
        }
        #endregion


        /** 
         * @brief 비트맵에서 Color 배열 취출
         * @param bmp 추출할 비트맵
         * @return TColor[,] 색상 배열
         */
        public static TColor[,] getRGB(Bitmap bmp) // 비트맵에서 Color 배열 취출
        {
            if (bmp == null)
            {
                return null;
            }
            TColor[,] arrRGB = new TColor[bmp.Height, bmp.Width];
            Color clr;
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    try
                    {
                        clr = bmp.GetPixel(j, i); // Get the color of pixel at position 5,5
                        arrRGB[i, j].r = clr.R;
                        arrRGB[i, j].g = clr.G;
                        arrRGB[i, j].b = clr.B;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Console.WriteLine(ex.StackTrace);
                        break;
                    }
                }
            }
            return arrRGB;
        }

        //*
        /** 
         * @brief 비트맵에서 특정 라인 색상 읽기
         * @param bmp 비트맵
         * @param line 추출할 라인
         * @return int[] 추출된 라인의 색상 배열
         */
        public static int[] getRGB(Bitmap bmp, int line) // 비트맵에서 특정 라인 색상 읽기
        {
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            try
            {
                var ptr = (IntPtr)((long)data.Scan0 + data.Stride * (bmp.Height - line - 1));
                var ret = new int[bmp.Width];
                System.Runtime.InteropServices.Marshal.Copy(ptr, ret, 0, ret.Length * 4);
                return ret;
            }
            finally
            {
                bmp.UnlockBits(data);
            }
        }

        /** 
         * @brief 비트맵 특정 범위 색상 읽기
         * @param image 비트맵 이미지
         * @param startX 추출 시작 x 좌표
         * @param startY 추출 시작 y 좌표
         * @param w 추출할 폭
         * @param h 추출할 높이
         * @param rgbArray 추출할 색상 배열을 참조전달로 지정
         * @param offset 추출할 색상 배열에 적용할 offset
         * @param scansize 스캔 범위
         */
        public static void getRGB(this Bitmap image, int startX, int startY, int w, int h, ref TColor[] rgbArray, int offset, int scansize) // 비트맵 특정 범위 색상 읽기
        {
            const int PixelWidth = 4;
            const PixelFormat PixelFormat = PixelFormat.Format32bppRgb;
 
            // En garde!
            if (image == null) throw new ArgumentNullException("image");
            if (rgbArray == null) throw new ArgumentNullException("rgbArray");
            if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
            if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
            if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
            if (h < 0 || (rgbArray.Length < offset + h * scansize) || h > image.Height) throw new ArgumentOutOfRangeException("h");

            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);
            try
            {
                byte[] pixelData = new Byte[data.Stride];
                for (int scanline = 0; scanline < data.Height; scanline++)
                {
                    Marshal.Copy(data.Scan0 + (scanline * data.Stride), pixelData, 0, data.Stride);
                    for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        // PixelFormat.Format32bppRgb means the data is stored
                        // in memory as BGR. We want RGB, so we must do some 
                        // bit-shuffling.
                        TColor col = new TColor();
                        /*
                        col.r = (pixelData[pixeloffset * PixelWidth + 2] << 16);
                        col.g = (pixelData[pixeloffset * PixelWidth + 1] << 8);
                        col.b = pixelData[pixeloffset * PixelWidth];
                        //*/
                        rgbArray[offset + (scanline * scansize) + pixeloffset] = col;
                    }
                }
            }
            finally
            {
                image.UnlockBits(data);
            }
        }
        //*/

        /** 
         * @brief 지정된 파일을 비트맵으로 읽기
         * @param path 읽을 비트맵 파일
         * @return Bitmap 읽은 비트맵
         */
        public static Bitmap LoadBitmap(string path) // 파일을 비트맵으로 읽기
        {
            if (File.Exists(path))
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                    return new Bitmap(memoryStream);
                }
            }
            else
            {
                return null;
            }
        }

    }

 
}
