using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Tausi.RubiksCube
{
    public static class WindowsAPI
    {
        enum WS_EX : ulong
        {
            TRANSPARENT = 0x00000020L,
            TOOLWINDOW = 0x00000080L,
            LAYERED = 0x00080000L,
            NOACTIVATE = 0x08000000L,
        }

        enum GWL : int
        {
            STYLE = -16,
            EXSTYLE = -20
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        delegate bool EnumProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")] public static extern IntPtr GetActiveWindow();

        [DllImport("Dwmapi.dll")] static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
        [DllImport("Dwmapi.dll")] static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out bool pvAttribute, int cbAttribute);

        [DllImport("user32.dll")] static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr hWnd, int nIndex, ulong dwNewLong);
        [DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32.dll")] static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")] static extern bool EnumWindows(EnumProc enumProc, IntPtr lParam);
        [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")] static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")] static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)] static extern bool IsIconic(IntPtr hWnd);
        
        public struct WindowInfo
        {
            public int hWnd;
            public UnityEngine.Rect rect;
            public string title;
        }

        static List<IntPtr> FindWindows_Result;

        public static IEnumerable<WindowInfo> GetWindowInfos()
        {
            foreach (IntPtr hWnd in FindWindows())
            {
                var info = GetWindowInfo(hWnd);

                if (info.HasValue)
                    yield return info.Value;
            }
        }

        public static WindowInfo? GetWindowInfo(IntPtr hWnd)
        {
            var windowInfo = new WindowInfo
            {
                hWnd = (int) hWnd,
                rect = GetRect(hWnd)
            };

            if (windowInfo.rect.x > -32000)
            {
                windowInfo.title = GetWindowTitle(hWnd);
                return windowInfo;
            }

            return null;
        }

        public static UnityEngine.Rect GetRect(IntPtr hWnd)
        {
            var r = new RECT();
            GetWindowRect(hWnd, ref r);
            return new UnityEngine.Rect(r.left, r.top, r.right - r.left, r.bottom - r.top);
        }

        public static void ExtendFrameIntoClientArea(IntPtr hWnd)
        {
            var margins = new MARGINS { cxLeftWidth = -1 };
            DwmExtendFrameIntoClientArea(hWnd, ref margins);
        }

        public static void SetTopMost(IntPtr hWnd)
        {
            SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0, 0);
        }

        public static void SetTransparent(IntPtr hWnd, bool noActivate, bool clickThrough)
        {
            var flags = WS_EX.LAYERED | WS_EX.TOOLWINDOW;

            if (noActivate)
                flags |= WS_EX.NOACTIVATE;

            if (clickThrough)
                flags |= WS_EX.TRANSPARENT;

            SetWindowLong(hWnd, (int) GWL.EXSTYLE, (ulong) flags);
        }

        static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }

        static List<IntPtr> FindWindows()
        {
            FindWindows_Result = new List<IntPtr>();
            EnumWindows(EnumWindows_Callback, IntPtr.Zero);
            return FindWindows_Result;
        }

        [AOT.MonoPInvokeCallback(typeof(EnumProc))]
        static bool EnumWindows_Callback(IntPtr hWnd, IntPtr lParam)
        {
            if (!IsWindowVisible(hWnd))
                return true;

            if (IsIconic(hWnd))
                return true;

            if (GetWindowTextLength(hWnd) == 0)
                return true;

            DwmGetWindowAttribute(hWnd, 14, out bool isHiddenWindowStoreApp, Marshal.SizeOf(typeof(bool)));
            if (isHiddenWindowStoreApp)
                return true;

            bool hasSomeExtendedWindowsStyles = (GetWindowLong(hWnd, (int) GWL.EXSTYLE) & (ulong) WS_EX.TOOLWINDOW) != 0;
            if (hasSomeExtendedWindowsStyles)
                return true;

            FindWindows_Result.Add(hWnd);
            return true;
        }
    }
}