using OneQuick.Core.Operations;
using OneQuick.Core.Triggers;
using System.Collections.Generic;

namespace OneQuick.Config
{
    public class QuickSearchEntry : ConfigEntry
    {
        public QuickSearchEntry()
        {
            Enable = true;
            Name = "";
            LaunchKey = new Kwrapper(K.None);
            LaunchUrl = "";
            GroupKey = new Kwrapper(K.None);
        }

        public QuickSearchEntry(string name, K Key, string Url, K groupKey) : this()
        {
            Name = name;
            LaunchKey = new Kwrapper(Key);
            LaunchUrl = Url;
            GroupKey = new Kwrapper(groupKey);
        }

        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                OnPropertyChanged("Enable", TriggerUpdate.Now);
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name", TriggerUpdate.Now);
            }
        }

        public Kwrapper LaunchKey
        {
            get => _launchKey;
            set
            {
                _launchKey = value;
                OnPropertyChanged("LaunchKey", TriggerUpdate.Now);
            }
        }

        public string LaunchUrl
        {
            get => _launchUrl;
            set
            {
                _launchUrl = value;
                OnPropertyChanged("LaunchUrl", TriggerUpdate.Now);
            }
        }

        public Kwrapper GroupKey
        {
            get => _groupKey;
            set
            {
                _groupKey = value;
                OnPropertyChanged("GroupKey", TriggerUpdate.Now);
            }
        }

        protected override IEnumerable<Trigger> GenerateTriggers()
        {
            if (Enable && LaunchKey.KeyValue != 0 && LaunchUrl != "")
            {
                return new List<HotkeyTrigger>
                {
                    new HotkeyTrigger
                    {
                        TriggerType = TriggerType.CopySearch,
                        Sequence = new ObservableCollectionX<HotkeySingle>
                        {
                            new HotkeySingle((K)131139)
                            {
                                Handled = false
                            },
                            new HotkeySingle((K)131139)
                            {
                                Handled = true
                            },
                            new HotkeySingle(LaunchKey.KeyData)
                            {
                                Handled = true
                            }
                        },
                        Operation = new RunCmd(LaunchUrl)
                        {
                            ReplaceClipboardTo = "%s",
                            Mode = Operation.SyncMode.Sync
                        }
                    }
                };
            }
            return null;
        }

        private bool _enable;

        private string _name;

        private Kwrapper _launchKey;

        private string _launchUrl;

        private Kwrapper _groupKey;
    }
}
