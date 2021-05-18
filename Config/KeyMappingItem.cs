using System.ComponentModel;

namespace OneQuick.Config
{
    public class KeyMappingItem : INotifyPropertyChanged
    {
        public KeyMappingItem()
        {
            Key = new Kwrapper(0);
            Value = new Kwrapper(0);
        }

        public KeyMappingItem(K k, K v)
        {
            Enable = true;
            Key = new Kwrapper(k);
            Value = new Kwrapper(v);
        }

        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                OnPropertyChanged("Enable");
            }
        }

        public Kwrapper Key
        {
            get => _key;
            set
            {
                if (_key != null)
                {
                    _key.PropertyChanged -= _key_PropertyChanged;
                }
                _key = value;
                _key.PropertyChanged += _key_PropertyChanged;
                OnPropertyChanged("Key");
            }
        }

        private void _key_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Key");
        }

        public Kwrapper Value
        {
            get => _value;
            set
            {
                if (_value != null)
                {
                    _value.PropertyChanged -= _value_PropertyChanged;
                }
                _value = value;
                _value.PropertyChanged += _value_PropertyChanged;
                OnPropertyChanged("Value");
            }
        }

        private void _value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Value");
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

        private bool _enable;

        private Kwrapper _key;

        private Kwrapper _value;
    }
}
