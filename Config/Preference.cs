using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;

namespace OneQuick.Config
{
    public class Preference : INotifyPropertyChanged
    {
        public Preference()
        {
            GUID = Guid.NewGuid().ToString();
            RunTimes = 0;
            TriggerCounter = 0L;
            CheckPrerelease = false;
            NewPushNumber();
            FirstRunDT = DateTime.Now;
            _configPath = "";
            ConfigEntrys = new ObservableCollectionX<ConfigFileEntry>();
        }

        public string GUID
        {
            get => _guid;
            set
            {
                if (!Guid.TryParse(value, out Guid guid))
                {
                    guid = Guid.NewGuid();
                    _guid = guid.ToString();
                }
                else
                {
                    _guid = value;
                }
                OnPropertyChanged("GUID");
            }
        }

        public int RunTimes
        {
            get => _runTimes;
            set
            {
                _runTimes = value;
                OnPropertyChanged("RunTimes");
            }
        }

        public long TriggerCounter
        {
            get => _triggerCounter;
            set
            {
                _triggerCounter = value;
                OnPropertyChanged("TriggerCounter");
            }
        }

        public bool CheckPrerelease
        {
            get => _checkPrerelease;
            set
            {
                _checkPrerelease = value;
                OnPropertyChanged("CheckPrerelease");
            }
        }

        public int PushNumber
        {
            get => _pushNumber;
            set
            {
                _pushNumber = value;
                OnPropertyChanged("PushNumber");
            }
        }

        public void NewPushNumber()
        {
            PushNumber = new Random().Next(0, 100);
        }

        public DateTime FirstRunDT
        {
            get => _firstRunDT;
            set
            {
                _firstRunDT = value;
                OnPropertyChanged("FirstRunDT");
            }
        }

        public string ConfigPath
        {
            get => _configPath;
            set
            {
                _configPath = value;
                OnPropertyChanged("ConfigPath");
            }
        }

        public ObservableCollectionX<ConfigFileEntry> ConfigEntrys
        {
            get => _configEntrys;
            set
            {
                if (_configEntrys != null)
                {
                    _configEntrys.ItemsPropertyChanged -= _configEntrys_ItemsPropertyChanged;
                    _configEntrys.CollectionChanged -= _configEntrys_CollectionChanged;
                }
                _configEntrys = value;
                _configEntrys.ItemsPropertyChanged += _configEntrys_ItemsPropertyChanged;
                _configEntrys.CollectionChanged += _configEntrys_CollectionChanged;
                OnPropertyChanged("ConfigEntrys");
            }
        }

        private void _configEntrys_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("ConfigEntrys");
        }

        private void _configEntrys_ItemsPropertyChanged(object sender, object item, string PropertyName)
        {
            OnPropertyChanged("ConfigEntrys");
        }

        public void AddConfigEntry(string path, bool allset = false)
        {
            if (path == "")
            {
                return;
            }
            bool flag = false;
            using (IEnumerator<ConfigFileEntry> enumerator = ConfigEntrys.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Path == path)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
            {
                ConfigEntrys.Add(new ConfigFileEntry
                {
                    Name = Path.GetFileNameWithoutExtension(path),
                    Path = path,
                    OverwriteScreenBorder = allset,
                    OverwriteReplacePhrase = allset,
                    OverwriteQuickSearch = allset,
                    OverwriteCustomHotkey = allset,
                    OverwriteKeyMapping = allset,
                    OverwriteBuildinFuncs = allset
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string Name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }
            propertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        private string _guid;

        private int _runTimes;

        private long _triggerCounter;

        private bool _checkPrerelease;

        private int _pushNumber;

        private DateTime _firstRunDT;

        private string _configPath = "";

        private ObservableCollectionX<ConfigFileEntry> _configEntrys;
    }
}
