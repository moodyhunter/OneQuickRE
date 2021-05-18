namespace OneQuick.WindowsEvents
{
    public class KeyEventArgs : Kwrapper
    {
        public KeyEventArgs(K k)
        {
            KeyData = k;
        }

        public bool Handled { get; set; }
    }
}
