using OneQuick.Config;
using OneQuick.Notification;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace OneQuick
{
    internal static class G
    {
        static G()
        {
            NetOutputDetailResponse = DEBUG && false;
            GaMock = DEBUG && false;
            NetEnable = RELEASE || true;
            LogTraceEvents = DEBUG && false;
            GlobalTimerInterval = new TimeSpan(0, 10, 0);
            SavePreferenceInterval = new TimeSpan(0, 30, 0);
            StoreCheckLicenseInterval = new TimeSpan(0, 6, 0, 0);
            CheckUpdateInfoInterval = new TimeSpan(2, 0, 0, 0);
            CheckAppdataFolderInterval = new TimeSpan(3, 0, 0, 0);
            CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _isDesignTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            MyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            AppDataFolder = Helper.FolderReturn(Path.Combine(MyDocumentsFolder, "OneQuick"));
            LogFilePath = Path.Combine(AppDataFolder, "OneQuick" + (IsDesignTime ? ".DesignTime" : "") + ".log");
        }

        public static void SetVariables(MainWindow window, TrayIcon trayIcon)
        {
            MainWindow = window;
            _trayIcon = trayIcon;
        }

        public static MainWindow MainWindow { get; private set; }

        public static TrayIcon TrayIcon => _trayIcon;

        public static IntPtr ModuleHandle => NativeMethods.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

        public static void TriggerCounter()
        {
            Preference preference = MainWindow.Preference;
            long triggerCounter = preference.TriggerCounter;
            preference.TriggerCounter = triggerCounter + 1L;
        }

        public static bool IsDesignTime => _isDesignTime;
        public static bool DEBUG => false;
        public static bool RELEASE => true;
        public static bool STORE => true;
        public static bool DESKTOP => false;
        public static bool ReleaseCheck = false;
        public static bool RedirectToLocalUrl;
        public static bool NetOutputDetailResponse;
        public static bool GaMock;
        public static bool NetEnable;
        public static bool LogTraceEvents;
        public const int LVR_CustomHotkeyCount = 1;
        public const int LVR_KeyMappingCount = 1;
        public const int InputRememberSize = 10;
        public const string SearchText = "%s";
        public const int ScreenBorderNonMoveMs = 2000;
        public const int ChromeTabHeight = 50;
        public static readonly TimeSpan GlobalTimerInterval;
        public static readonly TimeSpan SavePreferenceInterval;
        public static readonly TimeSpan StoreCheckLicenseInterval;
        public static readonly TimeSpan CheckUpdateInfoInterval;
        public static readonly TimeSpan CheckAppdataFolderInterval;
        public const int ConfigBackupRemainDays = 7;
        public const long MaxLogFileSize = 1048576L;
        public const int LogFileTruncateLength = 524288;
        public const int DesktopPopBallonMilliseconds = 5000;
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static readonly string CurrentDirectory;
        public static readonly string MyDocumentsFolder;
        public static readonly string AppDataFolder;
        public static readonly string LogFilePath;
        public static TrayIcon _trayIcon;
        public const string AppName = "OneQuick";
        public const string MutexName = "XUJINKAI.ONEQUICK.";
        public static bool _isDesignTime;
        public const bool _debug = false;
        public const bool _store = true;
    }
}
