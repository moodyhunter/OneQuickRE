using System;
using System.Runtime.InteropServices;

namespace OneQuick
{
    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        [DllImport("Powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int RegisterApplicationRestart([MarshalAs(UnmanagedType.LPWStr)] string commandLineArgs, int Flags);

        public const int HWND_BROADCAST = 65535;

        public static readonly int WM_SHOW_ALL = RegisterWindowMessage("WM_SHOW_XUJINKAI.ONEQUICK.");

        public static readonly int WM_EXIT_ALL = RegisterWindowMessage("WM_EXIT_XUJINKAI.ONEQUICK.");

        public const uint WM_QUERYENDSESSION = 17u;

        public const uint WM_ENDSESSION = 22u;
    }
}
