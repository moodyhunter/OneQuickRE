using OneQuick.Notification;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace OneQuick.Config
{
    public class ConfigFileEntry : INotifyPropertyChanged
    {
        public ConfigFileEntry()
        {
        }

        public ConfigFileEntry(string buildinconf, string name, string path)
        {
            BuildinConfigName = buildinconf;
            _name = name;
            _path = path;
        }

        [XmlIgnore]
        public Configuration BuildinConfiguration
        {
            get
            {
                if (DefConfig.BuildinConfigObjects.ContainsKey(_buildinConfigName))
                {
                    return DefConfig.BuildinConfigObjects[_buildinConfigName];
                }
                return null;
            }
        }

        [XmlIgnore]
        public bool CanChange => BuildinConfigName == "";

        [XmlIgnore]
        public bool CanNotChange => BuildinConfigName != "";

        public string BuildinConfigName
        {
            get => _buildinConfigName;
            set
            {
                _buildinConfigName = value;
                OnPropertyChanged("BuildinConfigName");
            }
        }

        public Kwrapper HotkeyLoad
        {
            get => _hotkeyLoad;
            set
            {
                if (_hotkeyLoad != null)
                {
                    _hotkeyLoad.PropertyChanged -= _hotkeyLoad_PropertyChanged;
                }
                _hotkeyLoad = value;
                if (_hotkeyLoad != null)
                {
                    _hotkeyLoad.PropertyChanged += _hotkeyLoad_PropertyChanged;
                }
                OnPropertyChanged("HotkeyLoad");
            }
        }

        private void _hotkeyLoad_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("HotkeyLoad");
        }

        public Kwrapper HotkeyClear
        {
            get => _hotkeyClear;
            set
            {
                if (_hotkeyLoad != null)
                {
                    _hotkeyLoad.PropertyChanged -= _hotkeyLoad_PropertyChanged1;
                }
                _hotkeyClear = value;
                if (_hotkeyLoad != null)
                {
                    _hotkeyLoad.PropertyChanged += _hotkeyLoad_PropertyChanged1;
                }
                OnPropertyChanged("HotkeyClear");
            }
        }

        private void _hotkeyLoad_PropertyChanged1(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("HotkeyClear");
        }

        public string Name
        {
            get
            {
                if (BuildinConfigName != "")
                {
                    return "<" + BuildinConfigName + ">";
                }
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Path
        {
            get
            {
                if (BuildinConfigName != "")
                {
                    return "<Buildin>";
                }
                return _path;
            }
            set
            {
                _path = value;
                OnPropertyChanged("Path");
            }
        }

        public bool OverwriteScreenBorder
        {
            get => _owScreenBorder;
            set
            {
                _owScreenBorder = value;
                OnPropertyChanged("OverwriteScreenBorder");
            }
        }

        public bool OverwriteQuickSearch
        {
            get => _owQuickSearch;
            set
            {
                _owQuickSearch = value;
                OnPropertyChanged("OverwriteQuickSearch");
            }
        }

        public bool OverwriteReplacePhrase
        {
            get => _owReplacePhrase;
            set
            {
                _owReplacePhrase = value;
                OnPropertyChanged("OverwriteReplacePhrase");
            }
        }

        public bool OverwriteCustomHotkey
        {
            get => _owCustomHotkey;
            set
            {
                _owCustomHotkey = value;
                OnPropertyChanged("OverwriteCustomHotkey");
            }
        }

        public bool OverwriteKeyMapping
        {
            get => _owKeyMapping;
            set
            {
                _owKeyMapping = value;
                OnPropertyChanged("OverwriteKeyMapping");
            }
        }

        public bool OverwriteBuildinFuncs
        {
            get => _owBuildinFuncs;
            set
            {
                _owBuildinFuncs = value;
                OnPropertyChanged("OverwriteBuildinFuncs");
            }
        }

        public void LoadCategory()
        {
            Configuration configur = G.MainWindow.Configur;
            Configuration configuration;
            if (BuildinConfiguration != null)
            {
                configuration = BuildinConfiguration;
            }
            else
            {
                if (!File.Exists(Path))
                {
                    Notify.ShowMsg("FileNotExist", "");
                    return;
                }
                try
                {
                    configuration = Configuration.LoadFrom(Path);
                }
                catch
                {
                    Notify.ShowMsg("FileLoadError", "");
                    return;
                }
            }
            if (OverwriteScreenBorder)
            {
                configur.ScreenBorders = configuration.ScreenBorders;
            }
            if (OverwriteQuickSearch)
            {
                configur.QuickSearch = configuration.QuickSearch;
            }
            if (OverwriteReplacePhrase)
            {
                configur.ReplacePhrases = configuration.ReplacePhrases;
            }
            if (OverwriteCustomHotkey)
            {
                configur.CustomKeys = configuration.CustomKeys;
            }
            if (OverwriteKeyMapping)
            {
                configur.KeyMapping = configuration.KeyMapping;
            }
            if (OverwriteBuildinFuncs)
            {
                configur.Buildin = configuration.Buildin;
            }
        }

        public void ClearCategory()
        {
            Configuration configur = G.MainWindow.Configur;
            Configuration emptyConfig = DefConfig.EmptyConfig;
            if (OverwriteScreenBorder)
            {
                configur.ScreenBorders = emptyConfig.ScreenBorders;
            }
            if (OverwriteQuickSearch)
            {
                configur.QuickSearch = emptyConfig.QuickSearch;
            }
            if (OverwriteReplacePhrase)
            {
                configur.ReplacePhrases = emptyConfig.ReplacePhrases;
            }
            if (OverwriteCustomHotkey)
            {
                configur.CustomKeys = emptyConfig.CustomKeys;
            }
            if (OverwriteKeyMapping)
            {
                configur.KeyMapping = emptyConfig.KeyMapping;
            }
            if (OverwriteBuildinFuncs)
            {
                configur.Buildin = emptyConfig.Buildin;
            }
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

        private string _buildinConfigName = "";

        private Kwrapper _hotkeyLoad;

        private Kwrapper _hotkeyClear;

        private string _name;

        private string _path;

        private bool _owScreenBorder;

        private bool _owQuickSearch;

        private bool _owReplacePhrase;

        private bool _owCustomHotkey;

        private bool _owKeyMapping;

        private bool _owBuildinFuncs;
    }
}
