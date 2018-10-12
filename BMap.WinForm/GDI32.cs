#region 文件说明 Info 
/* ========================================================================
* 【本类功能概述】 
* 
* 作者：zzl 时间：2018/9/26 17:17:09 
* 文件名：GDI32 
* 版本：V1.0.1 
* 
* 修改者：   时间： 
* 修改说明： 
* ========================================================================
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BMap.WinForm
{
    public class GDI32
    {
        public const string TOOLBARCLASSNAME = "ToolbarWindow32";
        public const string REBARCLASSNAME = "ReBarWindow32";
        public const string PROGRESSBARCLASSNAME = "msctls_progress32";
        public const string SCROLLBAR = "SCROLLBAR";

        #region Gdi32.dll functions

        [DllImport("gdi32.dll")]
        static public extern bool StretchBlt(IntPtr hDCDest, int XOriginDest, int YOriginDest, int WidthDest, int HeightDest,
        IntPtr hDCSrc, int XOriginScr, int YOriginSrc, int WidthScr, int HeightScr, uint Rop);
        [DllImport("gdi32.dll")]
        static public extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        static public extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int Width, int Heigth);
        [DllImport("gdi32.dll")]
        static public extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        [DllImport("gdi32.dll")]
        static public extern bool BitBlt(IntPtr hDCDest, int XOriginDest, int YOriginDest, int WidthDest, int HeightDest,
        IntPtr hDCSrc, int XOriginScr, int YOriginSrc, uint Rop);
        [DllImport("gdi32.dll")]
        static public extern IntPtr DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        static public extern bool PatBlt(IntPtr hDC, int XLeft, int YLeft, int Width, int Height, uint Rop);
        [DllImport("gdi32.dll")]
        static public extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        static public extern uint GetPixel(IntPtr hDC, int XPos, int YPos);
        [DllImport("gdi32.dll")]
        static public extern int SetMapMode(IntPtr hDC, int fnMapMode);
        [DllImport("gdi32.dll")]
        static public extern int GetObjectType(IntPtr handle);
        [DllImport("gdi32")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO_FLAT bmi,
        int iUsage, ref int ppvBits, IntPtr hSection, int dwOffset);
        [DllImport("gdi32")]
        public static extern int GetDIBits(IntPtr hDC, IntPtr hbm, int StartScan, int ScanLines, int lpBits, BITMAPINFOHEADER bmi, int usage);
        [DllImport("gdi32")]
        public static extern int GetDIBits(IntPtr hdc, IntPtr hbm, int StartScan, int ScanLines, int lpBits, ref BITMAPINFO_FLAT bmi, int usage);
        [DllImport("gdi32")]
        public static extern IntPtr GetPaletteEntries(IntPtr hpal, int iStartIndex, int nEntries, byte[] lppe);
        [DllImport("gdi32")]
        public static extern IntPtr GetSystemPaletteEntries(IntPtr hdc, int iStartIndex, int nEntries, byte[] lppe);
        [DllImport("gdi32")]
        public static extern uint SetDCBrushColor(IntPtr hdc, uint crColor);
        [DllImport("gdi32")]
        public static extern IntPtr CreateSolidBrush(uint crColor);
        [DllImport("gdi32")]
        public static extern int SetBkMode(IntPtr hDC, BackgroundMode mode);
        [DllImport("gdi32")]
        public static extern int SetViewportOrgEx(IntPtr hdc, int x, int y, int param);
        [DllImport("gdi32")]
        public static extern uint SetTextColor(IntPtr hDC, uint colorRef);
        [DllImport("gdi32")]
        public static extern int SetStretchBltMode(IntPtr hDC, int StrechMode);
        #endregion


        #region BITMAPINFO_FLAT

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO_FLAT
        {
            public int bmiHeader_biSize;
            public int bmiHeader_biWidth;
            public int bmiHeader_biHeight;
            public short bmiHeader_biPlanes;
            public short bmiHeader_biBitCount;
            public int bmiHeader_biCompression;
            public int bmiHeader_biSizeImage;
            public int bmiHeader_biXPelsPerMeter;
            public int bmiHeader_biYPelsPerMeter;
            public int bmiHeader_biClrUsed;
            public int bmiHeader_biClrImportant;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] bmiColors;
        }
        #endregion

        #region BITMAPINFOHEADER

        [StructLayout(LayoutKind.Sequential)]
        public class BITMAPINFOHEADER
        {
            public int biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }
        #endregion

        #region Background Mode

        public enum BackgroundMode
        {
            TRANSPARENT = 1,
            OPAQUE = 2
        }
        #endregion


        #region 封装
        public const int SRCCOPY = 0xCC0020;
        public static void DrawBitmap(Graphics g,Bitmap bmp)
        {
            Graphics clientDC = g;
            IntPtr hdcPtr = clientDC.GetHdc();
            IntPtr memdcPtr = CreateCompatibleDC(hdcPtr);   // 创建兼容DC
            IntPtr bmpPtr = bmp.GetHbitmap();
            SelectObject(memdcPtr, bmpPtr);
            BitBlt(hdcPtr, 0, 0, bmp.Width, bmp.Height, memdcPtr, 0, 0, SRCCOPY);
            DeleteDC(memdcPtr);             // 释放内存
            DeleteObject(bmpPtr);           // 释放内存
            clientDC.ReleaseHdc(hdcPtr);    // 释放内存
            clientDC.Dispose();
        }
        #endregion
    }
}
