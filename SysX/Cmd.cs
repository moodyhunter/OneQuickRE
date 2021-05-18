using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace OneQuick.SysX
{
    internal static class Cmd
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            IntPtr intPtr = CommandLineToArgvW(commandLine, out int num);
            if (intPtr == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            string[] result;
            try
            {
                string[] array = new string[num];
                for (int i = 0; i < array.Length; i++)
                {
                    IntPtr ptr = Marshal.ReadIntPtr(intPtr, i * IntPtr.Size);
                    array[i] = Marshal.PtrToStringUni(ptr);
                }
                result = array;
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
            }
            return result;
        }

        public static void RunSmart(string CommandWithArgs, bool ShowWindow = true)
        {
            if (IsUrl(CommandWithArgs))
            {
                OpenLink(CommandWithArgs);
                return;
            }
            string text = CommandLineToArgs(CommandWithArgs)[0];
            string args = CommandWithArgs.Substring(Math.Min(text.Length + 1, CommandWithArgs.Length));
            Run(text, args, ShowWindow);
        }

        public static void Run(string Command, string Args, bool ShowWindow = true)
        {
            if (ShowWindow)
            {
                Process.Start(Command, Args);
                return;
            }
            new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = Command,
                    Arguments = Args
                }
            }.Start();
        }

        public static bool IsUrl(string str)
        {
            string text = str.ToLower();
            return text.StartsWith("http://") || text.StartsWith("https://");
        }

        public static void OpenLink(string url)
        {
            if (IsUrl(url))
            {
                Run(url, "", true);
                return;
            }
            throw new Exception("Not URL.");
        }

        public static void Explorer(string path)
        {
            if (File.Exists(path))
            {
                Run("explorer", "/select, " + path, true);
                return;
            }
            if (Directory.Exists(path))
            {
                Run("explorer", path, true);
                return;
            }
            RunSmart("explorer", true);
        }
    }
}
