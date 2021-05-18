using Microsoft.Win32;
using OneQuick.Config;
using OneQuick.Core;
using OneQuick.Notification;
using OneQuick.SysX;
using OneQuick.WindowsEvents;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Threading;

namespace OneQuick
{
    public partial class MainWindow : Window, INotifyPropertyChanged, IStyleConnector
    {
        public bool WindowsHookEnable
        {
            get => EventsServer.Enable;
            set
            {
                EventsServer.Enable = value;
                string actionLabel = "HOOK_" + (value ? "Enable" : "Disable");
                OnPropertyChanged("WindowsHookEnable");
            }
        }

        public bool IsEntryServerRunning
        {
            get => _isEntryServerRunning;
            set
            {
                if (_isEntryServerRunning != value)
                {
                    _isEntryServerRunning = value;
                    EntryServer.EnableCounter = value;
                    string actionLabel = "SERVER_" + (value ? "Run" : "STOP");
                    OnPropertyChanged("IsEntryServerRunning");
                }
            }
        }

        public Visibility ReleaseCheckVisibility => G.ReleaseCheck.ToVisibility();

        public Visibility Debug_Visibility => DEBUG_MODE.ToVisibility();

        public bool DEBUG_MODE
        {
            get => _debug_mode;
            set
            {
                if (_debug_mode != value)
                {
                    string actionLabel = "DEBUG_MODE_" + (value ? "True" : "False");
                    _debug_mode = value;
                    if (!_debug_mode)
                    {
                        LogListenerEnable = false;
                        MainTabControl.SelectedItem = About_TabItem;
                    }
                    OnPropertyChanged("DEBUG_MODE");
                    OnPropertyChanged("Debug_Visibility");
                }
            }
        }

        public string LiteOrProLocal => "PRO!";

        public string AppNameLocal => AppName + " (" + LiteOrProLocal + ")";

        public string TitleOnWindow => (G.ReleaseCheck ? "<Release Test>" : AppNameLocal) + (G.DEBUG ? " //DEBUG: F11 Exit." : "");

        public string AboutUsingInfo
        {
            get
            {
                int config_entrys = EntryServer.Entrys.Count();
                int key_mappings = EventsServer.KeyMaps.Count();
                if (Preference != null)
                {
                    int days = Math.Max((DateTime.Now - Preference.FirstRunDT).Days, 0);
                    long triggers = Preference.TriggerCounter;
                    return "Running: " + days + " days, triggers: " + triggers + ", configs: " + config_entrys + ", maps: " + key_mappings;
                }
                return "Configs: " + config_entrys + ", maps: " + key_mappings;
            }
        }

        public bool SubscribeInfoUnderLine => LitePrivilege;

        public string SubscribeInfoText => "Haha";


        public MainWindow()
        {
            DateTime now = DateTime.Now;
            InitializeComponent();
            Log.Info(string.Format("{0} {1} {2}, {3}", new object[]
                {
                    AppCodeName,
                    ArchStr,
                    VersionObj,
                    Info.OsFullName
                }));
            Log.Info(Environment.CommandLine);
            InitializeTrayIcon();
            G.SetVariables(this, TrayIcon);
            if (G.IsDesignTime)
            {
                DEBUG_MODE = true;
                Log.Debug(new string[]
                {
                    "[MainWindow] IsDesignTime: return;"
                });
                return;
            }
            if (!IsNewInstance())
            {
                ShowExistInstance();
                Exit(false);
                return;
            }
            CheckPrivilege(true);
            UpgradeBefore();
            bool flag2 = !File.Exists(PreferenceFilePath);
            if (!InitPreference())
            {
                Exit(false);
                return;
            }
            InitConfig();
            if (flag2)
            {
                Log.Info(new string[]
                {
                    "No preferece file before, it's new install!"
                });
            }
            else if (Preference.LastRunVersion != VersionP3)
            {
                Log.Info(new string[]
                {
                    "Last run version != current version, it's update!"
                });
                Preference.NewPushNumber();
            }
            UpgradeAfter();
            Preference.RunTimes++;
            TrayIcon.Visible = true;
            Command_Init();
            SBC_LT = true;
            MainTabControl.SelectedItem = About_TabItem;
            EntryServer.EnableChanged += EntryServer_EnableChanged;
            EventsServer.HotkeyDown += EventsServer_HotkeyDown_Buildin;
            EventsServer.HotkeyDown += EventsServer_HotkeyDown_Configs;
            if (Environment.Is64BitOperatingSystem != Environment.Is64BitProcess)
            {
                Notify.ShowMsg("Tip_ProcessSystemBitNotMatch", "");
            }
            DispatcherTimer dispatcherTimer = new DispatcherTimer
            {
                Interval = G.GlobalTimerInterval
            };
            dispatcherTimer.Tick += MainWindow_Timer_Tick;
            dispatcherTimer.Start();
            IsEntryServerRunning = true;
            WindowsHookEnable = true;
            //ReloadPurchaseInfo(false); 
            Log.Verbose(new string[]
            {
                "End of MainWindow"
            });
        }

        private void IntervalWork()
        {
            Log.Verbose(new string[]
            {
                "IntervalWork"
            });
            CheckPrivilege(false);
            if (DateTime.Now - LastSavePreferenceDT > G.SavePreferenceInterval)
            {
                try
                {
                    SavePreference();
                    LastSavePreferenceDT = DateTime.Now;
                }
                catch
                {
                }
            }
            if (DateTime.Now - LastCheckAppdataFolderDT > G.CheckAppdataFolderInterval)
            {
                LastCheckAppdataFolderDT = DateTime.Now;
                Task.Run(delegate ()
                {
                    Log.CheckLogFileSize();
                    CheckBackupConfigCount();
                });
            }
        }

        private void MainWindow_Timer_Tick(object sender, EventArgs e)
        {
            IntervalWork();
        }

        private void UpgradeBefore()
        {
            string text = Path.Combine(G.MyDocumentsFolder, "OneQuick Lite");
            if (Directory.Exists(text) && !File.Exists(PreferenceFilePath))
            {
                string text2 = Path.Combine(G.MyDocumentsFolder, "OneQuick");
                if (Directory.Exists(text2))
                {
                    Directory.Delete(text2, true);
                }
                Directory.Move(text, text2);
                Notify.ShowMsg("AppDataFolder Change From 'OneQuick Lite' to 'OneQuick'\n我的文档中，OneQuick Lite目录改为OneQuick。", "OneQuick AppData Folder Changed");
            }
            string path = Path.Combine(G.AppDataFolder, "Lite.log");
            if (File.Exists(path))
            {
                File.Delete(path);
                Notify.ShowMsg("Lite.log change to OneQuick.log", "Delete Lite.log");
            }
            string text3 = Path.Combine(G.AppDataFolder, "preference.config");
            if (File.Exists(text3) && !File.Exists(PreferenceFilePath))
            {
                File.Move(text3, PreferenceFilePath);
                Notify.ShowMsg("Preference file name Change From 'preference.config' to 'preference.cfg'", "OneQuick Preference File Name Changed");
            }
        }

        private void UpgradeAfter()
        {
            if (Preference.LastRunVersion.StartsWith("0"))
            {
                Log.Info(new string[]
                {
                    "Upgrade 0.* -> 1.0, QuickSearch, CustomKeys, KeyMapping will be set."
                });
                Configuration prodVersion = DefConfig.ProdVersion;
                if (Configur.QuickSearch.Count == 0)
                {
                    Configur.QuickSearch = prodVersion.QuickSearch;
                }
                if (Configur.CustomKeys.Count == 0)
                {
                    Configur.CustomKeys = prodVersion.CustomKeys;
                }
                if (Configur.KeyMapping.Count == 0)
                {
                    Configur.KeyMapping = prodVersion.KeyMapping;
                }
                string[] files = Directory.GetFiles(G.AppDataFolder, "OneQuick.config.*.bkp", SearchOption.TopDirectoryOnly);
                if (files.Length != 0)
                {
                    foreach (string text in files)
                    {
                        string destFileName = Path.Combine(ConfigBackupFolder, Path.GetFileName(text));
                        File.Move(text, destFileName);
                    }
                }
                Preference.LastRunVersion = "1.0.0";
            }
            if (Preference.LastRunVersion.StartsWith("1.0"))
            {
                Preference.LastRunVersion = "1.1.0";
            }
        }

        private void EntryServer_EnableChanged()
        {
            OnPropertyChanged("IsRunning");
        }

        private void RegisterRestart()
        {
            NativeMethods.RegisterApplicationRestart(IsOpen ? "--start" : "--hide", 0);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_SHOW_ALL)
            {
                ShowWindow(null, null);
            }
            if (msg == NativeMethods.WM_EXIT_ALL)
            {
                Log.Info(new string[]
                {
                    "Exit with message WM_EXIT_ALL"
                });
                Exit(true);
            }
            if (msg == 17L)
            {
                RegisterRestart();
            }
            if (msg == 22L)
            {
                Log.Info(new string[]
                {
                    "Exit with message WM_ENDSESSION"
                });
                Exit(true);
            }
            return IntPtr.Zero;
        }

        public static void ShowExistInstance()
        {
            NativeMethods.PostMessage((IntPtr)65535, NativeMethods.WM_SHOW_ALL, IntPtr.Zero, IntPtr.Zero);
        }

        public static void ExitExistInstance()
        {
            NativeMethods.PostMessage((IntPtr)65535, NativeMethods.WM_EXIT_ALL, IntPtr.Zero, IntPtr.Zero);
        }

        private static bool IsNewInstance()
        {
            if (mutex == null)
            {
                mutex = new Mutex(true, "XUJINKAI.ONEQUICK.", out isNewInstance);
                return isNewInstance;
            }
            return isNewInstance;
        }

        private void Exit(bool SaveConfig)
        {
            if (SaveConfig)
            {
                SaveConfigToFile(null, null);
                SavePreference();
            }
            Application.Current.Shutdown();
        }

        private void ToggleShowHide(object sender = null, RoutedEventArgs e = null)
        {
            if (Visibility == Visibility.Hidden)
            {
                ShowWindow(null, null);
                return;
            }
            CloseWindow(null, null);
        }

        public void ShowWindow(object sender = null, RoutedEventArgs e = null)
        {
            Show();
            Activate();
            if (LitePrivilege)
            {
                MainTabControl.SelectedItem = About_TabItem;
            }
            IntervalWork();
        }

        public void CloseWindow(object sender = null, RoutedEventArgs e = null)
        {
            Close();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Exit(true);
        }

        private void AskExit(object sender = null, RoutedEventArgs e = null)
        {
            if (Notify.AskYesNo("Tip_Exit", AppName))
            {
                Exit(true);
            }
        }

        private void ToggleRunStop(object sender = null, RoutedEventArgs e = null)
        {
            IsEntryServerRunning = !IsEntryServerRunning;
        }

        protected override void OnActivated(EventArgs e)
        {
            Log.Debug(new string[]
            {
                "Activated"
            });
            if (ConfigFileChanged)
            {
                ReloadConfigFile(false);
            }
            base.OnActivated(e);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            Log.Debug(new string[]
            {
                "Deactivated"
            });
            if (ConfigObjectChanged)
            {
                if (ConfigFileChanged)
                {
                    Backup_ConfigFileOnPath(null, null);
                }
                SaveConfigToFile(null, null);
            }
            base.OnDeactivated(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            Log.Verbose(new string[]
            {
                "OnSourceInitialized"
            });
            base.OnSourceInitialized(e);
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
            {
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            Log.Verbose(new string[]
            {
                "------Window Close------"
            });
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void EventsServer_HotkeyDown_Configs(HotKeyEventArgs e)
        {
            if (ProPrivilege && ConfigHotkeyDict.ContainsKey(e.KeyData))
            {
                ConfigHotkeyDict[e.KeyData]();
                e.Handled = true;
            }
        }

        private void EventsServer_HotkeyDown_Buildin(HotKeyEventArgs e)
        {
            K keyData = e.KeyData;
            Configuration configur = Configur;
            K? k;
            if (configur == null)
            {
                k = null;
            }
            else
            {
                BuildinFuncs buildin = configur.Buildin;
                if (buildin == null)
                {
                    k = null;
                }
                else
                {
                    Kwrapper showHideWindow = buildin.ShowHideWindow;
                    k = (showHideWindow != null) ? new K?(showHideWindow.KeyData) : null;
                }
            }
            K? k2 = k;
            if (keyData == k2.GetValueOrDefault() & k2 != null)
            {
                ToggleShowHide(null, null);
                e.Handled = true;
                return;
            }
            K keyData2 = e.KeyData;
            Configuration configur2 = Configur;
            K? k3;
            if (configur2 == null)
            {
                k3 = null;
            }
            else
            {
                BuildinFuncs buildin2 = configur2.Buildin;
                if (buildin2 == null)
                {
                    k3 = null;
                }
                else
                {
                    Kwrapper runStopServer = buildin2.RunStopServer;
                    k3 = (runStopServer != null) ? new K?(runStopServer.KeyData) : null;
                }
            }
            k2 = k3;
            if (keyData2 == k2.GetValueOrDefault() & k2 != null)
            {
                ToggleRunStop(null, null);
                e.Handled = true;
            }
        }

        private void Command_Init()
        {
            RoutedCommand routedCommand = new RoutedCommand();
            routedCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBindings.Add(new CommandBinding(routedCommand, new ExecutedRoutedEventHandler(CloseWindow)));
            RoutedCommand routedCommand2 = new RoutedCommand();
            routedCommand2.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Alt | ModifierKeys.Shift));
            CommandBindings.Add(new CommandBinding(routedCommand2, delegate (object o, ExecutedRoutedEventArgs e)
            {
                DEBUG_MODE = !DEBUG_MODE;
            }));
        }


        private void MainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            mainwindow_preview_keypress += e.Key.ToString();
            if (mainwindow_preview_keypress.Length > 8)
            {
                mainwindow_preview_keypress = mainwindow_preview_keypress.Substring(mainwindow_preview_keypress.Length - 8);
                if (mainwindow_preview_keypress == "XUJINKAI")
                {
                    Notify.ShowMsg("作者从2013年接触AHK，几经修改，15年OneQuick成型，\n16年发布并开始准备WPF版，18年1.0版正式发布。\n是您的支持在鼓励着OneQuick继续前进。", "感谢使用OneQuick");
                }
            }
        }

        public string FeedbackInfo => string.Concat(new string[]
                {
                    AppCodeName,
                    " ",
                    ArchStr,
                    " ",
                    VersionObj.ToString(),
                    "\r\n",
                    Info.OsFullName
                });

        private void CopyFeedbackInfo(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(FeedbackInfo);
            Notify.PopNewToast(FeedbackInfo, "Copied");
        }

        public Preference Preference
        {
            get => _preference;
            set
            {
                if (_preference != null)
                {
                    _preference.PropertyChanged -= _preference_PropertyChanged;
                }
                _preference = value;
                if (_preference != null)
                {
                    _preference.PropertyChanged += _preference_PropertyChanged;
                    OnConfigFilePathChanged(_preference.ConfigPath);
                }
                OnPropertyChanged("Preference");
            }
        }

        private void _preference_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Preference preference = _preference;
            if (preference?.ConfigEntrys == null)
            {
                return;
            }
            ConfigHotkeyDict = new Dictionary<K, Action>();
            using (IEnumerator<ConfigFileEntry> enumerator = _preference.ConfigEntrys.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ConfigFileEntry x = enumerator.Current;
                    if (x.HotkeyLoad != null && x.HotkeyLoad.KeyValue != 0)
                    {
                        ConfigHotkeyDict.Add(x.HotkeyLoad.KeyData, delegate
                        {
                            x.LoadCategory();
                        });
                    }
                    if (x.HotkeyLoad != null)
                    {
                        Kwrapper hotkeyClear = x.HotkeyClear;
                        if (hotkeyClear == null || hotkeyClear.KeyValue != 0)
                        {
                            ConfigHotkeyDict.Add(x.HotkeyClear.KeyData, delegate
                            {
                                x.ClearCategory();
                            });
                        }
                    }
                }
            }
        }

        private bool InitPreference()
        {
            bool result;
            try
            {
                if (File.Exists(PreferenceFilePath))
                {
                    Preference = XmlSerialization.FromXmlFile<Preference>(PreferenceFilePath, false);
                }
                else
                {
                    Preference = new Preference();
                    SavePreference();
                }
                if (Preference.FirstRunDT == DateTime.MinValue)
                {
                    Preference.FirstRunDT = DateTime.Now;
                }
                using (List<ConfigFileEntry>.Enumerator enumerator = DefConfig.BuildinConfigEntrys.Values.ToList().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ConfigFileEntry x = enumerator.Current;
                        if ((from o in Preference.ConfigEntrys
                             where o.BuildinConfigName == x.BuildinConfigName
                             select o).Count() == 0)
                        {
                            Preference.ConfigEntrys.Add(x);
                        }
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, new string[]
                {
                    "InitPreference"
                });
                Notify.ShowMsg("Tip_InitPreferenceError", "");
                Cmd.Explorer(PreferenceFilePath);
                result = false;
            }
            return result;
        }

        private void SavePreference()
        {
            Preference.LastRunVersion = VersionP3;
            XmlSerialization.SaveToFile(Preference, PreferenceFilePath, true);
        }

        private void ResetPreference()
        {
            Preference = new Preference();
            SavePreference();
        }
        public string ConfigFilePath
        {
            get
            {
                if (!LitePrivilege)
                {
                    Preference preference = Preference;
                    if (!string.IsNullOrEmpty(preference?.ConfigPath))
                    {
                        return Preference.ConfigPath;
                    }
                }
                return ConfigFileDefaultPath;
            }
            set
            {
                Preference.ConfigPath = value;
                OnConfigFilePathChanged(value);
            }
        }

        public string ConfigFileDir => Path.GetDirectoryName(ConfigFilePath);

        public event ConfigFilePathChangedDelegate ConfigFilePathChanged;

        public void OnConfigFilePathChanged(string path)
        {
            ConfigFilePathChanged?.Invoke(path);
            OnPropertyChanged("ConfigFileDir");
            OnPropertyChanged("ConfigFilePath");
        }

        public bool ConfigFileChanged
        {
            get => _configFileChanged;
            private set
            {
                if (_configFileChanged != value)
                {
                    _configFileChanged = value;
                    OnPropertyChanged("ConfigFileChanged");
                }
            }
        }

        public bool ConfigObjectChanged
        {
            get => _configObjectChanged;
            private set
            {
                if (_configObjectChanged != value)
                {
                    _configObjectChanged = value;
                    OnPropertyChanged("ConfigObjectChanged");
                }
            }
        }

        public Configuration ConfigDefault => DefConfig.ProdVersion;

        public Configuration Configur
        {
            get => _config;
            set
            {
                if (_config != null)
                {
                    _config.Saving -= Config_Saving;
                    _config.Saved -= Config_Saved;
                    _config.PropertyChanged -= Config_PropertyChanged;
                    _config.IsInstalled = false;
                }
                _config = value;
                if (_config != null)
                {
                    _config.IsInstalled = true;
                    _config.PropertyChanged += Config_PropertyChanged;
                    _config.Saving += Config_Saving;
                    _config.Saved += Config_Saved;
                }
                ConfigObjectChanged = true;
                OnPropertyChanged("Configur");
            }
        }

        private void Config_Saving()
        {
            StopConfigWathcer();
        }

        private void Config_Saved(string filepath)
        {
            StartConfigWatcher();
            ConfigFileChanged = false;
            ConfigObjectChanged = false;
        }

        private void Config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ConfigObjectChanged = true;
            OnPropertyChanged("Configur");
        }

        private void InitConfig()
        {
            Log.Verbose(new string[]
            {
                "InitConfig"
            });
            if (File.Exists(ConfigFilePath))
            {
                ReloadConfigFile(true);
            }
            else
            {
                Configur = ConfigDefault;
                Configur.Save(ConfigFilePath);
            }
            SetupConfigWatcher();
        }

        private void ReloadConfigFile(bool onStartup = false)
        {
            try
            {
                Configur = Configuration.LoadFrom(ConfigFilePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, new string[]
                {
                    "ReloadConfigFile"
                });
                if (onStartup)
                {
                    Notify.ShowMsg("ConfigError_OnStartup_OverwriteDefault", "");
                    Backup_ConfigFileOnPath(null, null);
                    Configur = ConfigDefault;
                    Configur.Save(ConfigFilePath);
                }
                else
                {
                    Notify.ShowMsg("ConfigChangedReloadFail", "");
                    Backup_ConfigFileOnPath(null, null);
                    Configur.Save(ConfigFilePath);
                }
            }
            finally
            {
                ConfigObjectChanged = false;
                ConfigFileChanged = false;
            }
        }

        private void SaveConfigToFile(object sender = null, RoutedEventArgs e = null)
        {
            Configur.Save(ConfigFilePath);
        }

        private object Configs_ListView_OnAddingItem()
        {
            ConfigFileEntry configFileEntry = new ConfigFileEntry();
            Preference.ConfigEntrys.Add(configFileEntry);
            ConfigEntry_SelectFile(configFileEntry);
            return configFileEntry;
        }

        private void Configs_ListView_OnRemovingItem(object obj)
        {
            ConfigFileEntry configFileEntry = obj as ConfigFileEntry;
            if (configFileEntry.CanNotChange)
            {
                return;
            }
            Preference.ConfigEntrys.Remove(configFileEntry);
        }

        private void ConfigEntry_LoadKind(object sender, RoutedEventArgs e)
        {
            if (!Notify.AskYesNo("All configs will be cleared, Y/n?", ""))
            {
                return;
            }
            (((Button)sender).DataContext as ConfigFileEntry).LoadCategory();
        }

        private void ConfigEntry_ClearKind(object sender, RoutedEventArgs e)
        {
            if (!Notify.AskYesNo("All configs will be cleared, Y/n?", ""))
            {
                return;
            }
            (((Button)sender).DataContext as ConfigFileEntry).ClearCategory();
        }

        private void ConfigEntry_Select(object sender, RoutedEventArgs e)
        {
            ConfigFileEntry item = ((Button)sender).DataContext as ConfigFileEntry;
            ConfigEntry_SelectFile(item);
        }

        private void ConfigEntry_SelectFile(ConfigFileEntry item)
        {
            string initialDirectory = G.AppDataFolder;
            string fileName = "";
            if (File.Exists(item.Path))
            {
                initialDirectory = Directory.GetParent(item.Path).FullName;
                fileName = Path.GetFileName(item.Path);
            }
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = initialDirectory,
                FileName = fileName,
                DefaultExt = "",
                Filter = "*.*|*.*"
            };
            bool? flag = openFileDialog.ShowDialog();
            bool flag2 = true;
            if (flag.GetValueOrDefault() == flag2 & flag != null)
            {
                item.Name = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                item.Path = openFileDialog.FileName;
            }
        }

        private void ConfigEntry_Edit(object sender, RoutedEventArgs e)
        {
            ConfigFileEntry configFileEntry = ((Button)sender).DataContext as ConfigFileEntry;
            if (File.Exists(configFileEntry.Path))
            {
                Cmd.Run(configFileEntry.Path, "", true);
                return;
            }
            Notify.ShowMsg("FileNotExist", "");
        }

        private void ConfigFile_ChangePath(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Directory.GetParent(ConfigFilePath).FullName,
                FileName = ConfigFileName,
                DefaultExt = ".config",
                Filter = "*.config|*.config|*.*|*.*",
                //Title = LangHelper.CurrentFields.ConfigDlgChangePath,
                OverwritePrompt = false
            };
            bool? flag = saveFileDialog.ShowDialog();
            bool flag2 = true;
            if (flag.GetValueOrDefault() == flag2 & flag != null)
            {
                string fileName = saveFileDialog.FileName;
                if (File.Exists(fileName))
                {
                    try
                    {
                        Configur = Configuration.LoadFrom(fileName);
                        ConfigFilePath = fileName;
                        Notify.ShowMsg("ConfigDlgChangePathLoaded", "");
                        return;
                    }
                    catch
                    {
                        if (Notify.AskOverwrite(""))
                        {
                            Configur.Save(fileName);
                            ConfigFilePath = fileName;
                        }
                        return;
                    }
                }
                ConfigFilePath = fileName;
                SaveConfigToFile(null, null);
            }
        }

        private void ConfigFile_ImportFrom(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetParent(ConfigFilePath).FullName,
                FileName = "",
                DefaultExt = ".config",
                Filter = "*.config|*.config|*.*|*.*"
            };
            bool? flag = openFileDialog.ShowDialog();
            bool flag2 = true;
            if (flag.GetValueOrDefault() == flag2 & flag != null)
            {
                try
                {
                    Configuration configur = Configuration.LoadFrom(openFileDialog.FileName);
                    Configur = configur;
                    Preference preference = Preference;
                    if (preference != null)
                    {
                        preference.AddConfigEntry(openFileDialog.FileName, true);
                    }
                }
                catch
                {
                    Notify.ShowMsg("ConfigDlgImportError", "");
                }
            }
        }

        private void ConfigFile_ExportTo(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Directory.GetParent(ConfigFilePath).FullName,
                FileName = "",
                DefaultExt = ".config",
                Filter = "*.config|*.config|*.*|*.*"
            };
            bool? flag = saveFileDialog.ShowDialog();
            bool flag2 = true;
            if (flag.GetValueOrDefault() == flag2 & flag != null)
            {
                string fileName = saveFileDialog.FileName;
                Configur.Save(fileName);
                Preference preference = Preference;
                if (preference == null)
                {
                    return;
                }
                preference.AddConfigEntry(fileName, true);
            }
        }

        private void ConfigFile_Open(object sender, RoutedEventArgs e)
        {
            Cmd.Run(ConfigFilePath, "", true);
        }

        private void ConfigFile_ResetPath(object sender, RoutedEventArgs e)
        {
            if (File.Exists(ConfigFileDefaultPath) && !Notify.AskOverwrite(""))
            {
                return;
            }
            Configur.Save(ConfigFileDefaultPath);
            ConfigFilePath = "";
        }

        private void Config_ResetToDefault()
        {
            Configur = ConfigDefault;
            Configur.Save(ConfigFilePath);
        }

        private void Config_ResetToDefault(object sender, RoutedEventArgs e)
        {
            if (Notify.AskYesNo("ConfigResetTip", ""))
            {
                Config_ResetToDefault();
            }
        }

        private void Config_Clear(object sender, RoutedEventArgs e)
        {
            if (Notify.AskYesNo("ConfigClearTip", ""))
            {
                Configur = DefConfig.EmptyConfig;
                Configur.Save(ConfigFilePath);
            }
        }

        private void StartConfigWatcher()
        {
            if (_configFileWatcher != null)
            {
                _configFileWatcher.EnableRaisingEvents = true;
            }
        }

        private void StopConfigWathcer()
        {
            if (_configFileWatcher != null)
            {
                _configFileWatcher.EnableRaisingEvents = false;
            }
        }

        private void SetupConfigWatcher()
        {
            if (_configFileWatcher != null)
            {
                Log.Info(new string[]
                {
                    "SetupConfigWatcher return: _configFileWatcher != null"
                });
                return;
            }
            _configFileWatcher = new FileSystemWatcher
            {
                Path = ConfigFileDir,
                NotifyFilter = NotifyFilters.LastWrite
            };
            ConfigFilePathChanged += ConfigFileWatcher_ConfigFilePathChanged;
            _configFileWatcher.Changed += ConfigFileWatcher_Changed;
            _configFileWatcher.EnableRaisingEvents = true;
        }

        private void ConfigFileWatcher_ConfigFilePathChanged(string path)
        {
            _configFileWatcher.Path = ConfigFileDir;
        }

        private void ConfigFileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed && Helper.PathEquals(e.FullPath, ConfigFilePath))
            {
                Log.Info(new string[]
                {
                    "ConfigFileWatcher: Config File Changed"
                });
                ConfigFileChanged = true;
            }
        }

        private void CheckBackupConfigCount()
        {
            Log.Verbose(new string[]
            {
                "CheckBackupConfigCount"
            });
            string[] files = Directory.GetFiles(ConfigBackupFolder, GetConfigBackupFilePattern(), SearchOption.TopDirectoryOnly);
            if (files.Length != 0)
            {
                foreach (string text in (from c in files
                                         where (DateTime.Now - File.GetLastAccessTime(c)).Days > 7
                                         select c).ToArray())
                {
                    Log.Info(new string[]
                    {
                        "Delete Backup File " + text
                    });
                    File.Delete(text);
                }
            }
        }

        private string GetConfigBackupFilePattern()
        {
            return "Backup.*";
        }

        private string GetConfigBackupPath(string filename = "")
        {
            if (filename == "")
            {
                filename = ConfigFileName;
            }
            string str = DateTime.Now.ToString("yyyyMMddHHmmss");
            return Path.Combine(ConfigBackupFolder, "Backup." + str + "." + filename);
        }

        private void Backup_ConfigFileOnPath(object sender = null, RoutedEventArgs e = null)
        {
            if (File.Exists(ConfigFilePath))
            {
                File.Copy(ConfigFilePath, GetConfigBackupPath(""), true);
            }
        }

        private void Backup_ConfigObject(object sender = null, RoutedEventArgs e = null)
        {
            Configuration configur = Configur;
            if (configur == null)
            {
                return;
            }
            configur.Save(GetConfigBackupPath(""));
        }

        private void Config_SetDebugVersion(object sender, RoutedEventArgs e)
        {
            if (Notify.AskYesNo("", ""))
            {
                Configur = DefConfig.DebugVersion;
            }
        }

        private void Config_SetProdVersion(object sender, RoutedEventArgs e)
        {
            if (Notify.AskYesNo("", ""))
            {
                Configur = DefConfig.ProdVersion;
            }
        }
        public bool IsOpen => Visibility != Visibility.Hidden;
        public TrayIcon TrayIcon { get; private set; }
        public string TrayiconText => string.Concat(new string[]
                {
                    AppName,
                    " v",
                    VersionP3,
                    "\n",
                    (IsEntryServerRunning && WindowsHookEnable) ? "Running": "Stopped"
                });

        private Icon TrayIconIcon => !WindowsHookEnable
                    ? Properties.Resources.tray_red
                    : !IsEntryServerRunning ? Properties.Resources.tray_stop_border : Properties.Resources.white_black_border_16;

        private void InitializeTrayIcon()
        {
            TrayIcon = new TrayIcon();
            TrayIcon.MouseDown += TrayIcon_MouseDown;
            TrayIcon.MouseClick += TrayIcon_MouseClick;
            TrayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
            EntryServer.EnableChanged += TrayIcon_EntryServer_EnableChanged;
            EventsServer.EnableChanged += TrayIcon_EventsServer_EnableChanged;
            TrayIcon_EntryServer_EnableChanged();
            Closed += TrayIcon_MainWindow_Closed;
        }

        private void TrayIcon_EventsServer_EnableChanged(bool obj)
        {
            TrayIcon.Text = TrayiconText;
            TrayIcon.Icon = TrayIconIcon;
        }

        private void TrayIcon_EntryServer_EnableChanged()
        {
            TrayIcon.Text = TrayiconText;
            TrayIcon.Icon = TrayIconIcon;
        }

        private void TrayIcon_MainWindow_Closed(object sender, EventArgs e)
        {
            TrayIcon.Hide();
        }

        private void TrayIcon_BalloonTipClicked(object arg1, EventArgs arg2)
        {
            ShowWindow(null, null);
        }

        private void TrayIcon_MouseClick(object arg1, Notification.MouseButtons arg2)
        {
            if (arg2 == Notification.MouseButtons.Left)
            {
                ToggleShowHide(null, null);
            }
        }

        private void TrayIcon_MouseDown(object arg1, Notification.MouseButtons arg2)
        {
            if (arg2 == Notification.MouseButtons.Right)
            {
                Trayicon_UpdateContextMenu();
            }
        }

        private void Trayicon_UpdateContextMenu()
        {
            TrayIcon.SetContextMenu(new List<TrayMenuItemBase>
            {
                new TrayMenuItem("WindowShow", delegate()
                {
                    ShowWindow(null, null);
                })
                {
                    Default = true
                },
                new TrayMenuSeperator(),
                new TrayMenuItem("HomePage", delegate()
                {
                    OpenLink_HomePage(null, null);
                }),
                new TrayMenuItem("StorePage", delegate()
                {
                    OpenLink_StoreVersionUrlOrProtocol(null, null);
                }),
                new TrayMenuSeperator(),
                new TrayMenuItem("ServerRun", delegate()
                {
                    IsEntryServerRunning = !IsEntryServerRunning;
                })
                {
                    Checked = IsEntryServerRunning
                },
                new TrayMenuItem("Advanced")
                {
                    SubItems = new List<TrayMenuItemBase>
                    {
                        new TrayMenuItem("Window Hook", delegate() {WindowsHookEnable = !WindowsHookEnable;})
                        {
                            Checked = WindowsHookEnable
                        }
                    }
                },
                new TrayMenuItem("Exit", delegate()
                {
                    AskExit(null, null);
                })
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string Name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }
            propertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        public string TitleOnUpdate => AppName + " v" + VersionP3 + " Updater";
        public string Author => "xujinkai";
        public string AuthorUrl => "http://xujinkai.net/?onequick";
        public string AuthorZhiUrl => "https://www.zhihu.com/people/xxxjin/activities";
        public string WeiboUrl => "http://weibo.com/onequick";
        public string Weibo_ShowText => "微博(OneQuick工具)";
        public string StoreVersionUrl => "https://www.microsoft.com/store/apps/9pfn5k6qxt46";
        public string StoreVersionProtocolUri => "ms-windows-store://pdp/?productid=9pfn5k6qxt46";
        public void OpenLink_StoreVersionUrlOrProtocol(object sender = null, RoutedEventArgs e = null)
        {
            if (int.Parse(Info.CurrentBuild) >= 14393)
            {
                Cmd.Run(StoreVersionProtocolUri, "", true);
                return;
            }
            Cmd.OpenLink(StoreVersionUrl);
        }
        public string AppUrl => "http://onequick.org/?f=app";
        public string AppUrl_Show => "OneQuick.org";

        public string PrivacyPolicyUrl => "http://onequick.org/privacy-policy";

        public string FeedbackUrl => "http://onequick.org/go?feedback";

        public string ChangeLogUrl => "http://onequick.org/go?change-log";

        public string DocsOnlineUrl => "http://onequick.org/go?docs";

        private void OpenLink_HomePage(object sender = null, RoutedEventArgs e = null)
        {
            Cmd.OpenLink(AppUrl);
        }

        private void OpenLink_Docs(object sender = null, RoutedEventArgs e = null)
        {
            Cmd.OpenLink(DocsOnlineUrl);
        }

        private void OpenLink_ChangeLog(object sender = null, RoutedEventArgs e = null)
        {
            Cmd.OpenLink(ChangeLogUrl);
        }

        public string AppName => "OneQuick";

        public bool StoreVersion => G.STORE;

        public bool DesktopVersion => G.DESKTOP;

        public string DesktopOrStore => !G.STORE ? "Desktop" : "Store";

        public string VersionP3 => VersionObj.ToString(3);

        public Version VersionObj => VersionAssembly;

        public Version VersionAssembly => Assembly.GetExecutingAssembly().GetName().Version;

        public Version VersionFile => new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);

        public Visibility ENV_DEBUG_Visibility => G.DEBUG.ToVisibility();

        public Visibility ENV_STORE_Visibility => G.STORE.ToVisibility();

        public Visibility ENV_DESKTOP_Visibility => G.DESKTOP.ToVisibility();

        public bool X64Version => Environment.Is64BitProcess;

        public string ArchStr => !X64Version ? "x86" : "x64";

        public string StoreStartupTaskId => "OneQuickStartupTask";

        public string CurrentExePath => Assembly.GetEntryAssembly().Location;

        public string PreferenceFileName => "preference.cfg";

        public string PreferenceFilePath => Path.Combine(G.AppDataFolder, PreferenceFileName);

        public string ConfigFileName => "OneQuick.config";

        public string ConfigFileDefaultPath => Path.Combine(G.AppDataFolder, ConfigFileName);

        public string ConfigBackupFolder => Helper.FolderReturn(Path.Combine(G.AppDataFolder, "ConfigBackup"));

        public string UpdateInformationFilePath => Path.Combine(G.AppDataFolder, "information.xml");

        public string UpdateStepInfoFilePath => Path.Combine(G.AppDataFolder, "OneQuick.update.step.info.xml");

        public bool LitePrivilege => !ProPrivilege;

        public bool ProPrivilege => true;

        public Visibility LiteVersionVisibility => LitePrivilege.ToVisibility();

        public Visibility ProVersionVisibility => ProPrivilege.ToVisibility();

        public Visibility LiteVersionRestrictCountVisibility => (LitePrivilege && (IsHotkeyRemapSelected || IsKeyMappingSelected)).ToVisibility();

        public Visibility NeedProVersionTipVisibility => (LitePrivilege && IsConfigTabSelected).ToVisibility();

        public string AppCodeName => AppName + DesktopOrStore;
        public void OnPrivilegeUpdated()
        {
            Log.Debug(new string[]
            {
                "OnPrivilegeUpdated in"
            });
            OnPropertyChanged("LitePrivilege");
            OnPropertyChanged("ProPrivilege");
            OnPropertyChanged("LiteVersionVisibility");
            OnPropertyChanged("ProVersionVisibility");
            OnPropertyChanged("LiteVersionRestrictCountVisibility");
            OnPropertyChanged("NeedProVersionTipVisibility");
            OnPropertyChanged("ProLicenseExpirationDate");
            Log.Debug(new string[]
            {
                "OnPrivilegeUpdated::Configur?.UpdateInstall();"
            });
            Configuration configur = Configur;
            if (configur != null)
            {
                configur.UpdateInstall();
            }
            OnConfigFilePathChanged(ConfigFilePath);
            Log.Debug(new string[]
            {
                "OnPrivilegeUpdated::Update GUI"
            });
            Log.Debug(new string[]
            {
                "OnPrivilegeUpdated out"
            });
        }

        public void CheckPrivilege(bool force = false)
        {
            OnPrivilegeUpdated();
        }

        private void Null_Event(object sender, RoutedEventArgs e)
        {
            Notify.ShowMsg("Null_Event", "");
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
            EntryServer.EnableCounter = false;
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
            EntryServer.EnableCounter = true;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged("IsReplacePhrasesSelected");
            OnPropertyChanged("IsScreenCornerSelected");
            OnPropertyChanged("IsQuickSearchSelected");
            OnPropertyChanged("IsHotkeyRemapSelected");
            OnPropertyChanged("IsKeyMappingSelected");
            OnPropertyChanged("IsBuildinFuncSelected");
            OnPropertyChanged("Items_ToolBar_Enable");
            OnPropertyChanged("LiteVersionRestrictCountVisibility");
            OnPropertyChanged("NeedProVersionTipVisibility");
            OnPropertyChanged("AboutUsingInfo");
        }

        public bool IsReplacePhrasesSelected
        {
            get
            {
                TabControl mainTabControl = MainTabControl;
                return mainTabControl?.SelectedItem == ReplacePhrase_TabItem;
            }
        }

        public bool IsScreenCornerSelected
        {
            get
            {
                TabControl mainTabControl = MainTabControl;
                return mainTabControl?.SelectedItem == ScreenBorder_TabItem;
            }
        }

        public bool IsQuickSearchSelected
        {
            get
            {
                TabControl mainTabControl = MainTabControl;
                return mainTabControl?.SelectedItem == QuickSearch_TabItem;
            }
        }

        public bool IsHotkeyRemapSelected
        {
            get
            {
                TabControl mainTabControl = MainTabControl;
                return mainTabControl?.SelectedItem == HotkeyRemap_TabItem;
            }
        }

        public bool IsKeyMappingSelected
        {
            get
            {
                TabControl mainTabControl = MainTabControl;
                return mainTabControl?.SelectedItem == KeyMapping_TabItem;
            }
        }

        public bool IsBuildinFuncSelected
        {
            get
            {
                TabControl mainTabControl = MainTabControl;
                return mainTabControl?.SelectedItem == BuildinFuncs_TabItem;
            }
        }

        public bool IsConfigTabSelected
        {
            get
            {
                TabControl mainTabControl = MainTabControl;
                return mainTabControl?.SelectedItem == Config_TabItem;
            }
        }

        public bool Items_ToolBar_Enable => IsReplacePhrasesSelected || IsQuickSearchSelected || IsHotkeyRemapSelected || IsKeyMappingSelected || IsConfigTabSelected;

        private void ToolBar_NewItem(object sender, RoutedEventArgs e)
        {
            if (IsReplacePhrasesSelected)
            {
                ReplacePhrase_ListView.AddItem();
                return;
            }
            if (IsQuickSearchSelected)
            {
                QuickSearch_ListView.AddItem();
                return;
            }
            if (IsHotkeyRemapSelected)
            {
                CustomKeys_ListView.AddItem();
                return;
            }
            if (IsKeyMappingSelected)
            {
                KeyMapping_ListView.AddItem();
                return;
            }
            if (IsConfigTabSelected)
            {
                Configs_ListView.AddItem();
            }
        }

        private void ToolBar_DelItem(object sender, RoutedEventArgs e)
        {
            if (IsReplacePhrasesSelected)
            {
                ReplacePhrase_ListView.DeleteSelected();
                return;
            }
            if (IsQuickSearchSelected)
            {
                QuickSearch_ListView.DeleteSelected();
                return;
            }
            if (IsHotkeyRemapSelected)
            {
                CustomKeys_ListView.DeleteSelected();
                return;
            }
            if (IsKeyMappingSelected)
            {
                KeyMapping_ListView.DeleteSelected();
                return;
            }
            if (IsConfigTabSelected)
            {
                Configs_ListView.DeleteSelected();
            }
        }

        private object ReplacePhrase_ListView_OnAddingItem()
        {
            ReplacePhrase replacePhrase = DefConfig.NewReplacePhrase();
            Configur.ReplacePhrases.Add(replacePhrase);
            return replacePhrase;
        }

        private void ReplacePhrase_ListView_OnRemovingItem(object obj)
        {
            Configur.ReplacePhrases.Remove(obj as ReplacePhrase);
        }

        private void ReplacePhrase_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReplacePhrase_ListView.SelectedItems.Count == 1)
            {
                ReplacePhrase_Item.DataContext = ReplacePhrase_ListView.SelectedItem;
                return;
            }
            ReplacePhrase_Item.DataContext = null;
        }

        private void ReplacePhrase_Input_Edit_TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key key = e.Key;
            if (key != Key.Up)
            {
                if (key == Key.Down)
                {
                    ReplacePhrase_ListView.NextItem();
                    ReplacePhrase_Input_Edit_TextBox.SelectAll();
                    e.Handled = true;
                    return;
                }
            }
            else
            {
                ReplacePhrase_ListView.PrevItem();
                ReplacePhrase_Input_Edit_TextBox.SelectAll();
                e.Handled = true;
            }
        }

        private void ReplacePhrase_Output_Edit_TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Key key = e.Key;
            if (key == Key.Tab)
            {
                ReplacePhrase_Input_Edit_TextBox.Focus();
                e.Handled = true;
                return;
            }
            if (key == Key.Up)
            {
                ReplacePhrase_ListView.PrevItem();
                ReplacePhrase_Output_Edit_TextBox.SelectAll();
                e.Handled = true;
                return;
            }
            if (key != Key.Down)
            {
                return;
            }
            ReplacePhrase_ListView.NextItem();
            ReplacePhrase_Output_Edit_TextBox.SelectAll();
            e.Handled = true;
        }

        private object CustomKeys_ListView_OnAddingItem()
        {
            HotkeyRemap hotkeyRemap = DefConfig.NewHotkeyRemap();
            Configur.CustomKeys.Add(hotkeyRemap);
            return hotkeyRemap;
        }

        private void CustomKeys_ListView_OnRemovingItem(object obj)
        {
            Configur.CustomKeys.Remove(obj as HotkeyRemap);
        }

        private void CustomKeys_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomKeys_ListView.SelectedItems.Count == 1)
            {
                CustomHotkey_Item.DataContext = CustomKeys_ListView.SelectedItem;
                return;
            }
            CustomHotkey_Item.DataContext = null;
        }

        private object KeyMapping_ListView_OnAddingItem()
        {
            KeyMappingItem keyMappingItem = DefConfig.NewKeyMappingItem();
            Configur.KeyMapping.Add(keyMappingItem);
            return keyMappingItem;
        }

        private void KeyMapping_ListView_OnRemovingItem(object obj)
        {
            Configur.KeyMapping.Remove(obj as KeyMappingItem);
        }

        private void KeyMapping_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private object QuickSearch_ListView_OnAddingItem()
        {
            QuickSearchEntry quickSearchEntry = DefConfig.NewQuickSearchEntry();
            Configur.QuickSearch.Add(quickSearchEntry);
            return quickSearchEntry;
        }

        private void QuickSearch_ListView_OnRemovingItem(object obj)
        {
            Configur.QuickSearch.Remove((QuickSearchEntry)obj);
        }

        private void QuickSearch_ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuickSearch_ListView.SelectedItems.Count == 1)
            {
                QuickSearch_Item.DataContext = QuickSearch_ListView.SelectedItem;
                return;
            }
            QuickSearch_Item.DataContext = null;
        }

        private void UpdateSBC()
        {
            OnPropertyChanged("SBC_LT");
            OnPropertyChanged("SBC_T");
            OnPropertyChanged("SBC_RT");
            OnPropertyChanged("SBC_R");
            OnPropertyChanged("SBC_RB");
            OnPropertyChanged("SBC_B");
            OnPropertyChanged("SBC_LB");
            OnPropertyChanged("SBC_L");
        }

        public bool SBC_LT
        {
            get => ScreenBorder_Detail != null && ScreenBorder_Detail.ScreenBorder == Configur.ScreenBorders.LT;
            set
            {
                if (value)
                {
                    ScreenBorder_Detail.ScreenBorder = Configur.ScreenBorders.LT;
                }
                UpdateSBC();
            }
        }

        public bool SBC_T
        {
            get => ScreenBorder_Detail != null && ScreenBorder_Detail.ScreenBorder == Configur.ScreenBorders.T;
            set
            {
                if (value)
                {
                    ScreenBorder_Detail.ScreenBorder = Configur.ScreenBorders.T;
                }
                UpdateSBC();
            }
        }

        public bool SBC_RT
        {
            get => ScreenBorder_Detail != null && ScreenBorder_Detail.ScreenBorder == Configur.ScreenBorders.RT;
            set
            {
                if (value)
                {
                    ScreenBorder_Detail.ScreenBorder = Configur.ScreenBorders.RT;
                }
                UpdateSBC();
            }
        }

        public bool SBC_R
        {
            get => ScreenBorder_Detail != null && ScreenBorder_Detail.ScreenBorder == Configur.ScreenBorders.R;
            set
            {
                if (value)
                {
                    ScreenBorder_Detail.ScreenBorder = Configur.ScreenBorders.R;
                }
                UpdateSBC();
            }
        }

        public bool SBC_RB
        {
            get => ScreenBorder_Detail != null && ScreenBorder_Detail.ScreenBorder == Configur.ScreenBorders.RB;
            set
            {
                if (value)
                {
                    ScreenBorder_Detail.ScreenBorder = Configur.ScreenBorders.RB;
                }
                UpdateSBC();
            }
        }

        public bool SBC_B
        {
            get => ScreenBorder_Detail != null && ScreenBorder_Detail.ScreenBorder == Configur.ScreenBorders.B;
            set
            {
                if (value)
                {
                    ScreenBorder_Detail.ScreenBorder = Configur.ScreenBorders.B;
                }
                UpdateSBC();
            }
        }

        public bool SBC_LB
        {
            get => ScreenBorder_Detail != null && ScreenBorder_Detail.ScreenBorder == Configur.ScreenBorders.LB;
            set
            {
                if (value)
                {
                    ScreenBorder_Detail.ScreenBorder = Configur.ScreenBorders.LB;
                }
                UpdateSBC();
            }
        }

        public bool SBC_L
        {
            get => ScreenBorder_Detail != null && ScreenBorder_Detail.ScreenBorder == Configur.ScreenBorders.L;
            set
            {
                if (value)
                {
                    ScreenBorder_Detail.ScreenBorder = Configur.ScreenBorders.L;
                }
                UpdateSBC();
            }
        }

        private void EntryServerStatus_ToolBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    EntryServerShortStatus_Refresh(null, null);
                    EntryServer.EnableChanged += EntryServer_EnableChanged_Debug;
                    EntryServer.EntrysChanged += EntryServer_EntrysChanged_Debug;
                    return;
                }
                EntryServer.EnableChanged -= EntryServer_EnableChanged_Debug;
                EntryServer.EntrysChanged -= EntryServer_EntrysChanged_Debug;
            }
        }

        private void EntryServer_EntrysChanged_Debug()
        {
            EntryServerShortStatus_Refresh(null, null);
        }

        private void EntryServer_EnableChanged_Debug()
        {
            EntryServerShortStatus_Refresh(null, null);
        }

        private void EntryServerShortStatus_Refresh(object sender = null, RoutedEventArgs e = null)
        {
            Dispatcher.Invoke(delegate ()
            {
                EntryServerShortStatus.Content = EntryServer.DebugStatus();
            });
        }

        private void PopTestToast(object sender, RoutedEventArgs e)
        {
            Notify.PopNewToast("test message", null);
        }

        private void ClearAllToast(object sender, RoutedEventArgs e)
        {
            Notify.ClearToast();
        }

        private void SetWindowTitleDefault(object sender, RoutedEventArgs e)
        {
            Title = TitleOnWindow;
        }

        private void ThrowNewException(object sender, RoutedEventArgs e)
        {
            throw new Exception("ThrowNewException()");
        }

        private void ThrowNewExceptionThread(object sender, RoutedEventArgs e)
        {
            new Thread(delegate ()
            {
                throw new Exception("ThrowNewExceptionThread()");
            }).Start();
        }

        private void ThrowNewExceptionTask(object sender, RoutedEventArgs e)
        {
            Task.Run(delegate ()
            {
                throw new Exception("ThrowNewExceptionTask()");
            });
        }

        private void Run_Notepad(object sender, RoutedEventArgs e)
        {
            Cmd.RunSmart("notepad", true);
        }

        private IOrderedEnumerable<Core.Triggers.Trigger> EntryServer_Entrys_byOrder => from o in EntryServer.Entrys
                                                                                        orderby o.ToString()
                                                                                        select o;

        private void TabItem_Entrys_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                {
                    EntryServer_EntrysChanged();
                    EntryServer.EntrysChanged += EntryServer_EntrysChanged;
                    return;
                }
                EntryServer.EntrysChanged -= EntryServer_EntrysChanged;
            }
        }

        private void EntryServer_EntrysChanged()
        {
            EntrysListView.ItemsSource = EntryServer_Entrys_byOrder;
        }

        public bool LogListenerEnable
        {
            get => _logListenerEnable;
            set
            {
                if (_logListenerEnable != value)
                {
                    if (value)
                    {
                        Log.Listener += Log_Listener;
                    }
                    else
                    {
                        Log.Listener -= Log_Listener;
                    }
                    _logListenerEnable = value;
                    OnPropertyChanged("LogListenerEnable");
                }
            }
        }

        private void LogAddLine(string s)
        {
            if ((DateTime.Now - LastLogTime).TotalSeconds > 1.0)
            {
                EventsLogBox.AppendText("------\r\n");
                LastLogTime = DateTime.Now;
            }
            EventsLogBox.AppendText(s + "\r\n");
            ScrollViewer_Log.ScrollToEnd();
        }

        private void Log_Listener(string log)
        {
            Application.Current.Dispatcher.Invoke(delegate ()
            {
                LogAddLine(log);
            });
        }

        private void Button_Events_Clear_Click(object sender, RoutedEventArgs e)
        {
            EventsLogBox.Clear();
        }

        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 38:
                    ((Button)target).Click += this.ConfigEntry_LoadKind;
                    return;
                case 39:
                    ((Button)target).Click += this.ConfigEntry_ClearKind;
                    return;
                case 40:
                    ((Button)target).Click += this.ConfigEntry_Select;
                    return;
                case 41:
                    ((Button)target).Click += this.ConfigEntry_Edit;
                    return;
                default:
                    return;
            }
        }

        private bool _isEntryServerRunning;

        private bool _debug_mode;

        private DateTime LastCheckAppdataFolderDT;

        private DateTime LastSavePreferenceDT;

        private static Mutex mutex;

        private static bool isNewInstance;

        private Dictionary<K, Action> ConfigHotkeyDict = new Dictionary<K, Action>();
        private string mainwindow_preview_keypress = "";
        private Preference _preference;
        private bool _configFileChanged;
        private bool _configObjectChanged;
        private Configuration _config;
        private FileSystemWatcher _configFileWatcher;
        public static readonly string AddonsInfoUrl = (!G.RedirectToLocalUrl) ? "https://onequick.org/addons-information.xml" : "http://127.0.0.1/addons-information.xml";
        public static readonly string UpdateInfoUrl = (!G.RedirectToLocalUrl) ? "https://onequick.org/update-information.xml" : "http://127.0.0.1/update-information.xml";
        private bool _logListenerEnable;
        private DateTime LastLogTime = DateTime.Now;
        public delegate void ConfigFilePathChangedDelegate(string path);
    }
}
