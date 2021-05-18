using OneQuick.Core;
using OneQuick.Core.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OneQuick.Config
{
    [Serializable]
    public abstract class ConfigEntry : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        [XmlIgnore]
        public bool IsInstalled
        {
            get => _isInstalled;
            set
            {
                _isInstalled = value;
                if (value)
                {
                    ForceInstallTriggers();
                    return;
                }
                ForceUnInstallTriggers();
            }
        }

        protected abstract IEnumerable<Trigger> GenerateTriggers();

        private void ForceInstallTriggers()
        {
            ForceUnInstallTriggers();
            if (IsInstalled)
            {
                theTriggers = GenerateTriggers();
                EntryServer.Add(theTriggers);
                _isTriggerPropertyChanged = false;
            }
        }

        private void ForceUnInstallTriggers()
        {
            if (theTriggers != null)
            {
                EntryServer.Remove(theTriggers);
                theTriggers = null;
                _isTriggerPropertyChanged = false;
            }
        }

        protected void UpdateTriggersLater()
        {
            _isTriggerPropertyChanged = true;
        }

        public void UpdateTriggersAsNeeded()
        {
            if (_isTriggerPropertyChanged)
            {
                ForceInstallTriggers();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string Name, TriggerUpdate triggerUpdate)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
            if (triggerUpdate == TriggerUpdate.Later)
            {
                UpdateTriggersLater();
                return;
            }
            if (triggerUpdate != TriggerUpdate.Now)
            {
                return;
            }
            ForceInstallTriggers();
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => errors.Count > 0;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !errors.ContainsKey(propertyName))
            {
                return null;
            }
            return errors[propertyName];
        }

        public void AddError(string propertyName, string error, bool isWarning)
        {
            if (!errors.ContainsKey(propertyName))
            {
                errors[propertyName] = new List<string>();
            }
            if (!errors[propertyName].Contains(error))
            {
                if (isWarning)
                {
                    errors[propertyName].Add(error);
                }
                else
                {
                    errors[propertyName].Insert(0, error);
                }
                RaiseErrorsChanged(propertyName);
            }
        }

        public void RemoveError(string propertyName, string error)
        {
            if (errors.ContainsKey(propertyName) && errors[propertyName].Contains(error))
            {
                errors[propertyName].Remove(error);
                if (errors[propertyName].Count == 0)
                {
                    errors.Remove(propertyName);
                }
                RaiseErrorsChanged(propertyName);
            }
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            EventHandler<DataErrorsChangedEventArgs> errorsChanged = ErrorsChanged;
            if (errorsChanged == null)
            {
                return;
            }
            errorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private bool _isInstalled;

        private IEnumerable<Trigger> theTriggers;

        private bool _isTriggerPropertyChanged;

        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public enum TriggerUpdate
        {
            None,
            Later,
            Now
        }
    }
}
