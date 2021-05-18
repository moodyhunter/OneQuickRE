using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OneQuick.SysX
{
    internal static class Info
    {
        static Info()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
            ProductName = (string)registryKey.GetValue("ProductName");
            ReleaseId = (string)registryKey.GetValue("ReleaseId");
            CurrentBuild = (string)registryKey.GetValue("CurrentBuild");
            CurrentVersion = (string)registryKey.GetValue("CurrentVersion");
            BuildBranch = (string)registryKey.GetValue("BuildBranch");
            ProductId = (string)registryKey.GetValue("ProductId");
            OsArch = Environment.Is64BitOperatingSystem ? "x64" : "x86";
            OsFullName = string.Concat(new string[]
            {
                ProductName,
                " ",
                OsArch,
                " ",
                ReleaseId,
                " (",
                CurrentBuild,
                ".",
                BuildBranch,
                ")"
            });
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        public static int TouchPointCount()
        {
            return GetSystemMetrics(SM_MAXIMUMTOUCHES);
        }

        public static Point MousePos()
        {
            return Control.MousePosition;
        }

        public static List<Rectangle> AllMonitorsXYWH()
        {
            List<Rectangle> list = new List<Rectangle>();
            foreach (Screen screen in Screen.AllScreens)
            {
                list.Add(screen.Bounds);
            }
            return list;
        }

        public static string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;

        public static string ComputerName => Environment.MachineName;

        public static readonly string ProductName;

        public static readonly string ReleaseId;

        public static readonly string CurrentBuild;

        public static readonly string CurrentVersion;

        public static readonly string BuildBranch;

        public static readonly string OsArch;

        public static readonly string ProductId;

        public static readonly string OsFullName;

        private static readonly int SM_MAXIMUMTOUCHES = 95;
    }
}
