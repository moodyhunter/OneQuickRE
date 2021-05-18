using System.Collections.Generic;
using System.ComponentModel;
using OneQuick.Core.Conditions;
using OneQuick.Core.Operations;
using OneQuick.Core.Triggers;

namespace OneQuick.Config
{
    public class BuildinFuncs : ConfigEntry
    {
        public Kwrapper ShowHideWindow
        {
            get => _showHideWindow;
            set
            {
                if (_showHideWindow != null)
                {
                    _showHideWindow.PropertyChanged -= _showHideWindow_PropertyChanged;
                }
                _showHideWindow = value;
                if (_showHideWindow != null)
                {
                    _showHideWindow.PropertyChanged += _showHideWindow_PropertyChanged;
                }
                OnPropertyChanged("ShowHideWindow", TriggerUpdate.None);
            }
        }

        private void _showHideWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ShowHideWindow", TriggerUpdate.None);
        }

        public Kwrapper RunStopServer
        {
            get => _runStopServer;
            set
            {
                if (_runStopServer != null)
                {
                    _runStopServer.PropertyChanged -= _runStopServer_PropertyChanged;
                }
                _runStopServer = value;
                if (_runStopServer != null)
                {
                    _runStopServer.PropertyChanged += _runStopServer_PropertyChanged;
                }
                OnPropertyChanged("RunStopServer", TriggerUpdate.None);
            }
        }

        private void _runStopServer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("RunStopServer", TriggerUpdate.None);
        }

        public Kwrapper WindowInfo
        {
            get => _windowInfo;
            set
            {
                if (_windowInfo != null)
                {
                    _windowInfo.PropertyChanged -= _windowInfo_PropertyChanged;
                }
                _windowInfo = value;
                if (_windowInfo != null)
                {
                    _windowInfo.PropertyChanged += _windowInfo_PropertyChanged;
                }
                OnPropertyChanged("WindowInfo", TriggerUpdate.Now);
            }
        }

        private void _windowInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("WindowInfo", TriggerUpdate.Now);
        }

        public Kwrapper ExplorerAppPath
        {
            get => _explorerAppPath;
            set
            {
                if (_explorerAppPath != null)
                {
                    _explorerAppPath.PropertyChanged -= _explorerAppPath_PropertyChanged;
                }
                _explorerAppPath = value;
                if (_explorerAppPath != null)
                {
                    _explorerAppPath.PropertyChanged += _explorerAppPath_PropertyChanged;
                }
                OnPropertyChanged("WindowInfo", TriggerUpdate.Now);
            }
        }

        private void _explorerAppPath_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("WindowInfo", TriggerUpdate.Now);
        }

        public bool CloseNotepad
        {
            get => _closeNotepad;
            set
            {
                _closeNotepad = value;
                OnPropertyChanged("CloseNotepad", TriggerUpdate.Now);
            }
        }

        public bool ChromeScrollTab
        {
            get => _chromeScrollTab;
            set
            {
                _chromeScrollTab = value;
                OnPropertyChanged("ChromeScrollTab", TriggerUpdate.Now);
            }
        }

        public BuildinFuncs() : this(true)
        {
        }

        public BuildinFuncs(bool set_all)
        {
            SetAll(set_all);
        }

        public void SetAll(bool set)
        {
            if (ShowHideWindow == null)
            {
                ShowHideWindow = new Kwrapper(0);
            }
            if (RunStopServer == null)
            {
                RunStopServer = new Kwrapper(0);
            }
            if (WindowInfo == null)
            {
                WindowInfo = new Kwrapper(0);
            }
            if (ExplorerAppPath == null)
            {
                ExplorerAppPath = new Kwrapper(0);
            }
            CloseNotepad = set;
            ChromeScrollTab = set;
        }

        protected override IEnumerable<Trigger> GenerateTriggers()
        {
            List<Trigger> list = new List<Trigger>();
            if (WindowInfo.KeyValue > 0)
            {
                list.Add(new HotkeyTrigger(WindowInfo)
                {
                    TriggerType = TriggerType.AppInfoWindow,
                    Operation = new BuildinOperation(BuildinOperationEnum.ShowInfoWindow)
                });
            }
            if (ExplorerAppPath.KeyValue > 0)
            {
                list.Add(new HotkeyTrigger(ExplorerAppPath)
                {
                    TriggerType = TriggerType.AppPathExplorer,
                    Operation = new BuildinOperation(BuildinOperationEnum.ExplorerAppPath)
                });
            }
            if (CloseNotepad)
            {
                list.Add(new HotkeyTrigger((K)131159)
                {
                    TriggerType = TriggerType.CtrlWCloseNotepad,
                    Condition = new ProgramCodition("notepad.exe"),
                    Operation = new SendKey((K)262259)
                });
            }
            if (ChromeScrollTab)
            {
                list.Add(new HotkeyTrigger(K.WheelDown)
                {
                    TriggerType = TriggerType.ChromeScrollTab,
                    Condition = new MouseAppPosCodition("chrome.exe", PositionEnum.TopRow)
                    {
                        Padding = 50
                    },
                    Operation = new SendKey((K)131081)
                });
                list.Add(new HotkeyTrigger(K.WheelUp)
                {
                    TriggerType = TriggerType.ChromeScrollTab,
                    Condition = new MouseAppPosCodition("chrome.exe", PositionEnum.TopRow)
                    {
                        Padding = 50
                    },
                    Operation = new SendKey((K)196617)
                });
            }
            return list;
        }

        private Kwrapper _showHideWindow;

        private Kwrapper _runStopServer;

        private Kwrapper _windowInfo;

        private Kwrapper _explorerAppPath;

        private bool _closeNotepad;

        private bool _chromeScrollTab;
    }
}
