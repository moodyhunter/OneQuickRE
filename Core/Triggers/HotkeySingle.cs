namespace OneQuick.Core.Triggers
{
    public class HotkeySingle : Kwrapper
    {
        public ModSide ModSide
        {
            get => _modSide;
            set
            {
                _modSide = value;
                OnPropertyChanged("ModSide");
            }
        }

        public bool Handled
        {
            get => _handled;
            set
            {
                _handled = value;
                OnPropertyChanged("Handled");
            }
        }

        public HotkeySingle()
        {
            ModSide = ModSide.Both;
            Handled = true;
        }

        public HotkeySingle(K k) : this()
        {
            KeyData = k;
        }

        public override bool Equals(object obj)
        {
            HotkeySingle hotkeySingle = obj as HotkeySingle;
            K keyData = KeyData;
            K? k = (hotkeySingle != null) ? new K?(hotkeySingle.KeyData) : null;
            return keyData == k.GetValueOrDefault() & k != null;
        }

        public override string ToString()
        {
            return KeyData.FormatString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        private ModSide _modSide;

        private bool _handled;
    }
}
