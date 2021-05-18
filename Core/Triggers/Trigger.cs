using OneQuick.Core.Conditions;
using OneQuick.Core.Operations;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OneQuick.Core.Triggers
{
    [XmlInclude(typeof(HotkeyTrigger))]
    [XmlInclude(typeof(InputTrigger))]
    [XmlInclude(typeof(NonMoveTrigger))]
    public abstract class Trigger : INotifyPropertyChanged
    {
        private event Action _triggerFired;

        public event Action TriggerFired
        {
            add
            {
                _triggerFired += value;
            }
            remove
            {
                _triggerFired -= value;
            }
        }

        [XmlIgnore]
        public Condition Condition { get; set; }

        [XmlIgnore]
        public Operation Operation { get; set; }

        [XmlIgnore]
        public int Priority => this.Priority();

        public abstract void ResetState();

        public void TriggerFire()
        {
            if (Log.VerboseSwitch)
            {
                Log.Verbose(new string[]
                {
                    "[TriggerFire] " + DebugFormatString()
                });
            }
            Operation operation = Operation;
            if (operation != null)
            {
                operation.BeginInvoke();
            }
            _triggerFired?.Invoke();
            // TriggerTypeCounter();
        }

        public string DebugFormatString()
        {
            string[] array = new string[8];
            array[0] = ToString();
            array[1] = " -> ";
            int num = 2;
            Operation operation = Operation;
            array[num] = (operation != null) ? operation.ToString() : null;
            array[3] = " (";
            int num2 = 4;
            Condition condition = Condition;
            array[num2] = (condition != null) ? condition.ToString() : null;
            array[5] = ", ";
            array[6] = Priority.ToString();
            array[7] = ")";
            return string.Concat(array);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }
            propertyChanged(this, new PropertyChangedEventArgs(Name));
        }

        [XmlIgnore]
        public TriggerType TriggerType { get; set; }
    }
}
