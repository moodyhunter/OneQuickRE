using OneQuick.Core.Operations;
using OneQuick.Core.Triggers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace OneQuick.Config
{
    public class HotkeyRemap : ConfigEntry
    {
        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                OnPropertyChanged("Enable", TriggerUpdate.Now);
            }
        }

        public HotkeyTrigger Hotkey
        {
            get => _hotkey;
            set
            {
                if (_hotkey != null)
                {
                    _hotkey.PropertyChanged -= _hotkey_PropertyChanged;
                }
                _hotkey = value;
                if (_hotkey != null)
                {
                    _hotkey.PropertyChanged += _hotkey_PropertyChanged;
                }
                OnPropertyChanged("Hotkey", TriggerUpdate.Now);
            }
        }

        private void _hotkey_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Hotkey", TriggerUpdate.Now);
        }

        public Operation Operation
        {
            get => _operation;
            set
            {
                if (_operation != null)
                {
                    _operation.PropertyChanged -= _operation_PropertyChanged;
                }
                _operation = value;
                if (_operation != null)
                {
                    _operation.PropertyChanged += _operation_PropertyChanged;
                }
                OnPropertyChanged("Operation", TriggerUpdate.Now);
            }
        }

        private void _operation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Operation", TriggerUpdate.Now);
        }

        public HotkeyRemap()
        {
            Enable = true;
        }

        public HotkeyRemap(K key) : this()
        {
            Hotkey = new HotkeyTrigger(key);
        }

        public HotkeyRemap(K key, Operation op) : this(key)
        {
            Operation = op;
        }

        protected override IEnumerable<Trigger> GenerateTriggers()
        {
            if (G.MainWindow.LitePrivilege)
            {
                MainWindow mainWindow = G.MainWindow;
                ObservableCollectionX<HotkeyRemap> observableCollectionX;
                if (mainWindow == null)
                {
                    observableCollectionX = null;
                }
                else
                {
                    Configuration configur = mainWindow.Configur;
                    observableCollectionX = (configur != null) ? configur.CustomKeys : null;
                }
                ObservableCollectionX<HotkeyRemap> observableCollectionX2 = observableCollectionX;
                if (observableCollectionX2 != null)
                {
                    if ((from o in observableCollectionX2
                         where o.Enable
                         select o).Count() > 1)
                    {
                        return null;
                    }
                }
            }
            if (Enable && Hotkey != null && Operation != null)
            {
                List<Trigger> list = new List<Trigger>();
                Hotkey.Operation = Operation;
                Hotkey.TriggerType = TriggerType.CustomHotkey;
                list.Add(Hotkey);
                return list;
            }
            return null;
        }

        private bool _enable;

        private HotkeyTrigger _hotkey;

        private Operation _operation;
    }
}
