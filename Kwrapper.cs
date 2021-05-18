using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace OneQuick
{
    [Serializable]
    public class Kwrapper : INotifyPropertyChanged, IComparable
    {
        private void OnKeyDataPropertyChanged()
        {
            OnPropertyChanged("KeyValue");
            OnPropertyChanged("KeyData");
            OnPropertyChanged("KeyCode");
            OnPropertyChanged("Ctrl");
            OnPropertyChanged("Shift");
            OnPropertyChanged("Alt");
            OnPropertyChanged("Win");
            OnPropertyChanged("FormatString");
        }

        public int KeyValue
        {
            get => (int)KeyData;
            set
            {
                KeyData = (K)value;
                OnKeyDataPropertyChanged();
            }
        }

        [XmlIgnore]
        public K KeyData
        {
            get => _keyData;
            set
            {
                _keyData = value;
                OnKeyDataPropertyChanged();
            }
        }

        [XmlIgnore]
        public K KeyCode => KeyData.PrimeKey();

        public bool Ctrl => KeyData.Ctrl();

        public bool Shift => KeyData.Shift();

        public bool Alt => KeyData.Alt();

        public bool Win => KeyData.Win();

        public string FormatString => KeyData.FormatString();

        public Kwrapper()
        {
            KeyValue = 0;
        }

        public Kwrapper(K k) : this()
        {
            KeyData = k;
        }

        public Kwrapper(int x) : this()
        {
            KeyData = (K)x;
        }

        public override string ToString()
        {
            return KeyData.FormatString();
        }

        public override bool Equals(object obj)
        {
            Kwrapper kwrapper = obj as Kwrapper;
            K keyData = KeyData;
            K? k = (kwrapper != null) ? new K?(kwrapper.KeyData) : null;
            return keyData == k.GetValueOrDefault() & k != null;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static explicit operator Kwrapper(Keys v)
        {
            return new Kwrapper((int)v);
        }

        public static explicit operator Kwrapper(K v)
        {
            return new Kwrapper((int)v);
        }

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

        public int CompareTo(object obj)
        {
            Kwrapper kwrapper = obj as Kwrapper;
            return ToString().CompareTo((kwrapper != null) ? kwrapper.ToString() : null);
        }

        private K _keyData;
    }
}
