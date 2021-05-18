using OneQuick.WindowsEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace OneQuick.Config
{
    [Serializable]
    public class Configuration : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string Name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }
            propertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        public Configuration()
        {
            ReplacePhrases = new ObservableCollectionX<ReplacePhrase>();
            ScreenBorders = new ScreenBorderPackage();
            QuickSearch = new ObservableCollectionX<QuickSearchEntry>();
            CustomKeys = new ObservableCollectionX<HotkeyRemap>();
            KeyMapping = new ObservableCollectionX<KeyMappingItem>();
            Buildin = new BuildinFuncs();
            PropertyChanged += Configuration_PropertyChanged;
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTriggersAsNeeded();
        }

        [XmlIgnore]
        public bool IsInstalled
        {
            get => _isInstalled;
            set
            {
                _isInstalled = value;
                UpdateInstall();
            }
        }

        public void UpdateInstall()
        {
            foreach (ConfigEntry configEntry in IterAll())
            {
                configEntry.IsInstalled = IsInstalled;
            }
            UpdateKeyMappingServer();
        }

        private void EntryChanged(IEnumerable<ConfigEntry> newItems, IEnumerable<ConfigEntry> oldItems)
        {
            if (newItems != null)
            {
                foreach (ConfigEntry configEntry in newItems)
                {
                    configEntry.IsInstalled = IsInstalled;
                }
            }
            if (oldItems != null)
            {
                foreach (ConfigEntry configEntry2 in oldItems)
                {
                    configEntry2.IsInstalled = false;
                }
            }
        }

        public void UpdateTriggersAsNeeded()
        {
            foreach (ConfigEntry configEntry in IterAll())
            {
                configEntry.UpdateTriggersAsNeeded();
            }
        }

        private IEnumerable<ConfigEntry> IterAll()
        {
            if (ReplacePhrases != null)
            {
                foreach (ReplacePhrase replacePhrase in ReplacePhrases)
                {
                    yield return replacePhrase;
                }
            }
            if (ScreenBorders != null)
            {
                foreach (ScreenBorder screenBorder in ScreenBorders.IterAll())
                {
                    yield return screenBorder;
                }
            }
            if (QuickSearch != null)
            {
                foreach (QuickSearchEntry quickSearchEntry in QuickSearch)
                {
                    yield return quickSearchEntry;
                }
            }
            if (QuickSearchGroup != null)
            {
                yield return QuickSearchGroup;
            }
            if (CustomKeys != null)
            {
                foreach (HotkeyRemap hotkeyRemap in CustomKeys)
                {
                    yield return hotkeyRemap;
                }
            }
            if (Buildin != null)
            {
                yield return Buildin;
            }
            yield break;
        }

        public ObservableCollectionX<ReplacePhrase> ReplacePhrases
        {
            get => _replacePhrases;
            set
            {
                if (_replacePhrases != null)
                {
                    _replacePhrases.CollectionChanged -= _replacePhrases_CollectionChanged;
                    _replacePhrases.ItemsPropertyChanged -= _replacePhrases_ItemsPropertyChanged;
                    foreach (ReplacePhrase replacePhrase in _replacePhrases)
                    {
                        replacePhrase.IsInstalled = false;
                    }
                }
                _replacePhrases = value;
                if (_replacePhrases != null)
                {
                    _replacePhrases.CollectionChanged += _replacePhrases_CollectionChanged;
                    _replacePhrases.ItemsPropertyChanged += _replacePhrases_ItemsPropertyChanged;
                    foreach (ReplacePhrase replacePhrase2 in _replacePhrases)
                    {
                        replacePhrase2.IsInstalled = IsInstalled;
                    }
                }
                OnPropertyChanged("ReplacePhrases");
            }
        }

        private void _replacePhrases_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList newItems = e.NewItems;
            IEnumerable<ConfigEntry> newItems2 = (newItems != null) ? newItems.Cast<ReplacePhrase>() : null;
            IList oldItems = e.OldItems;
            EntryChanged(newItems2, (oldItems != null) ? oldItems.Cast<ReplacePhrase>() : null);
            OnPropertyChanged("ReplacePhrases");
        }

        private void _replacePhrases_ItemsPropertyChanged(object collection, object item, string PropertyName)
        {
            OnPropertyChanged("ReplacePhrases");
        }

        public ScreenBorderPackage ScreenBorders
        {
            get => _screenBorders;
            set
            {
                if (_screenBorders != null)
                {
                    _screenBorders.PropertyChanged -= _screenBorders_PropertyChanged;
                    foreach (ScreenBorder screenBorder in _screenBorders.IterAll())
                    {
                        screenBorder.IsInstalled = false;
                    }
                }
                _screenBorders = value;
                if (_screenBorders != null)
                {
                    _screenBorders.PropertyChanged += _screenBorders_PropertyChanged;
                    foreach (ScreenBorder screenBorder2 in _screenBorders.IterAll())
                    {
                        screenBorder2.IsInstalled = IsInstalled;
                    }
                }
                OnPropertyChanged("ScreenBorders");
            }
        }

        private void _screenBorders_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ScreenBorders");
        }

        public ObservableCollectionX<QuickSearchEntry> QuickSearch
        {
            get => _quickSearch;
            set
            {
                if (QuickSearchGroup != null)
                {
                    QuickSearchGroup.IsInstalled = false;
                    QuickSearchGroup = null;
                }
                if (_quickSearch != null)
                {
                    _quickSearch.CollectionChanged -= _quickSearch_CollectionChanged;
                    _quickSearch.ItemsPropertyChanged -= _quickSearch_ItemsPropertyChanged;
                    foreach (QuickSearchEntry quickSearchEntry in _quickSearch)
                    {
                        quickSearchEntry.IsInstalled = false;
                    }
                }
                _quickSearch = value;
                if (_quickSearch != null)
                {
                    QuickSearchGroup = new QuickSearchGroupCollection(value)
                    {
                        IsInstalled = IsInstalled
                    };
                    _quickSearch.CollectionChanged += _quickSearch_CollectionChanged;
                    _quickSearch.ItemsPropertyChanged += _quickSearch_ItemsPropertyChanged;
                    foreach (QuickSearchEntry quickSearchEntry2 in _quickSearch)
                    {
                        quickSearchEntry2.IsInstalled = IsInstalled;
                    }
                }
                OnPropertyChanged("QuickSearch");
                OnPropertyChanged("QuickSearchGroup");
            }
        }

        private void _quickSearch_ItemsPropertyChanged(object sender, object item, string PropertyName)
        {
            OnPropertyChanged("QuickSearch");
            QuickSearchGroup.IsInstalled = false;
            QuickSearchGroup.IsInstalled = IsInstalled;
            OnPropertyChanged("QuickSearchGroup");
        }

        private void _quickSearch_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList newItems = e.NewItems;
            IEnumerable<ConfigEntry> newItems2 = (newItems != null) ? newItems.Cast<QuickSearchEntry>() : null;
            IList oldItems = e.OldItems;
            EntryChanged(newItems2, (oldItems != null) ? oldItems.Cast<QuickSearchEntry>() : null);
            OnPropertyChanged("QuickSearch");
            QuickSearchGroup.IsInstalled = false;
            QuickSearchGroup.IsInstalled = IsInstalled;
            OnPropertyChanged("QuickSearchGroup");
        }

        [XmlIgnore]
        public QuickSearchGroupCollection QuickSearchGroup { get; private set; }

        public ObservableCollectionX<HotkeyRemap> CustomKeys
        {
            get => _customKeys;
            set
            {
                if (_customKeys != null)
                {
                    _customKeys.CollectionChanged -= _customKeys_CollectionChanged;
                    _customKeys.ItemsPropertyChanged -= _customKeys_ItemsPropertyChanged;
                    foreach (HotkeyRemap hotkeyRemap in _customKeys)
                    {
                        hotkeyRemap.IsInstalled = false;
                    }
                }
                _customKeys = value;
                if (_customKeys != null)
                {
                    _customKeys.CollectionChanged += _customKeys_CollectionChanged;
                    _customKeys.ItemsPropertyChanged += _customKeys_ItemsPropertyChanged;
                    foreach (HotkeyRemap hotkeyRemap2 in _customKeys)
                    {
                        hotkeyRemap2.IsInstalled = IsInstalled;
                    }
                }
                OnPropertyChanged("CustomKeys");
            }
        }

        private void _customKeys_ItemsPropertyChanged(object collection, object item, string PropertyName)
        {
            OnPropertyChanged("CustomKeys");
        }

        private void _customKeys_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IList newItems = e.NewItems;
            IEnumerable<ConfigEntry> newItems2 = (newItems != null) ? newItems.Cast<HotkeyRemap>() : null;
            IList oldItems = e.OldItems;
            EntryChanged(newItems2, (oldItems != null) ? oldItems.Cast<HotkeyRemap>() : null);
            OnPropertyChanged("CustomKeys");
        }

        public ObservableCollectionX<KeyMappingItem> KeyMapping
        {
            get => _keyMapping;
            set
            {
                if (_keyMapping != null)
                {
                    _keyMapping.CollectionChanged -= _keyMapping_CollectionChanged;
                    _keyMapping.ItemsPropertyChanged -= _keyMapping_ItemsPropertyChanged;
                }
                _keyMapping = value;
                if (_keyMapping != null)
                {
                    _keyMapping.CollectionChanged += _keyMapping_CollectionChanged;
                    _keyMapping.ItemsPropertyChanged += _keyMapping_ItemsPropertyChanged;
                }
                UpdateKeyMappingServer();
                OnPropertyChanged("KeyMapping");
            }
        }

        private void _keyMapping_ItemsPropertyChanged(object sender, object item, string PropertyName)
        {
            UpdateKeyMappingServer();
            OnPropertyChanged("KeyMapping");
        }

        private void _keyMapping_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateKeyMappingServer();
            OnPropertyChanged("KeyMapping");
        }

        public void UpdateKeyMappingServer()
        {
            if (IsInstalled)
            {
                EventsServer.KeyMaps = _keyMapping.ToDictionaryIntInt();
                return;
            }
            EventsServer.KeyMaps = new Dictionary<int, int>();
        }

        public BuildinFuncs Buildin
        {
            get => _buildin;
            set
            {
                if (_buildin != null)
                {
                    _buildin.PropertyChanged -= _buildin_PropertyChanged;
                    _buildin.IsInstalled = false;
                }
                _buildin = value;
                if (_buildin != null)
                {
                    _buildin.PropertyChanged += _buildin_PropertyChanged;
                    _buildin.IsInstalled = IsInstalled;
                }
                OnPropertyChanged("Buildin");
            }
        }

        private void _buildin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Buildin");
        }

        public event SavingDelegate Saving;

        public event SavedDelegate Saved;

        public static Configuration LoadFrom(string filepath)
        {
            return XmlSerialization.FromXmlFile<Configuration>(filepath, false);
        }

        public void Save(string filepath)
        {
            Saving?.Invoke();
            XmlSerialization.SaveToFile(this, filepath, true);
            SavedDelegate saved = Saved;
            if (saved == null)
            {
                return;
            }
            saved(filepath);
        }

        private bool _isInstalled;

        private ObservableCollectionX<ReplacePhrase> _replacePhrases;

        private ScreenBorderPackage _screenBorders;

        private ObservableCollectionX<QuickSearchEntry> _quickSearch;

        private ObservableCollectionX<HotkeyRemap> _customKeys;

        private ObservableCollectionX<KeyMappingItem> _keyMapping;

        private BuildinFuncs _buildin;

        // (Invoke) Token: 0x06000AE4 RID: 2788
        public delegate void SavingDelegate();

        // (Invoke) Token: 0x06000AE8 RID: 2792
        public delegate void SavedDelegate(string filepath);
    }
}
