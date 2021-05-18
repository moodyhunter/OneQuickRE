using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace OneQuick.SysX
{
    public static class Win
    {
        [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
        [DllImport("User32.dll")] private static extern int SetForegroundWindow(int hWnd);
        [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")] private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")] private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll")] private static extern bool GetLayeredWindowAttributes(IntPtr hwnd, out uint crKey, out byte bAlpha, out uint dwFlags);
        [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("User32.dll")] private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")] private static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("User32.dll")] private static extern int FindWindow(string ClassName, string WindowName);

        public static bool GetTopmostStatus(IntPtr? hWnd = null) => (GetWindowLong(hWnd ?? GetForegroundWindow(), -20) & 8) == 8;

        public static void ToggleTopmost(IntPtr? hWnd = null, bool? SetOrToggle = null)
        {
            IntPtr hWnd2 = hWnd ?? GetForegroundWindow();
            IntPtr hWndInsertAfter = (SetOrToggle ?? (!GetTopmostStatus(null))) ? HWND_TOPMOST : HWND_NOTOPMOST;
            _ = SetWindowPos(hWnd2, hWndInsertAfter, 0, 0, 0, 0, 67u);
        }

        public static double GetOpacity(IntPtr? hWnd = null)
        {
            IntPtr intPtr = hWnd ?? GetForegroundWindow();
            SetWindowLong(intPtr, -20, 524288U);
            GetLayeredWindowAttributes(intPtr, out uint num, out byte b, out uint num2);
            double num3 = b / 255.0;
            if (num3 == 0.0)
            {
                SetOpacity(1.0, new IntPtr?(intPtr));
                num3 = 1.0;
            }
            return num3;
        }

        public static void SetOpacity(double opacity, IntPtr? hWnd = null)
        {
            IntPtr intPtr = hWnd ?? GetForegroundWindow();
            SetWindowLong(intPtr, -20, 524288);
            SetLayeredWindowAttributes(intPtr, 0u, (byte)(opacity * 255.0), 2u);
        }

        public static string GetWindowTitle(IntPtr? hWnd = null)
        {
            IntPtr hWnd2 = hWnd ?? GetForegroundWindow();
            StringBuilder stringBuilder = new StringBuilder(256);
            if (GetWindowText(hWnd2, stringBuilder, 256) > 0)
            {
                return stringBuilder.ToString();
            }
            return null;
        }

        public static string GetWindowProcFileName(IntPtr? hWnd = null)
        {
            if (Environment.Is64BitOperatingSystem != Environment.Is64BitProcess) return "";
            GetWindowThreadProcessId(hWnd ?? GetForegroundWindow(), out uint num);
            if (num == 0u || num == 4u) return "";
            Process processById = Process.GetProcessById((int)num);
            string result;
            try
            {
                result = processById.MainModule.FileName;
            }
            catch (Exception ex)
            {
                Log.Error(ex, string.Format("get_MainModule: pid: {0}", num));
                result = "";
            }
            return result;
        }

        public static RECT GetWindowRectangle(IntPtr? hWnd = null)
        {
            GetWindowRect(hWnd ?? GetForegroundWindow(), out RECT result);
            return result;
        }
        public static void MonitorOff()
        {
            NativeMethods.SendMessage(65535, 274, 61808, 2);
        }

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
