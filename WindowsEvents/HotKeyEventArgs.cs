namespace OneQuick.WindowsEvents
{
    public class HotKeyEventArgs : Kwrapper
    {
        public HotKeyEventArgs(K k, K rawCode)
        {
            KeyData = k;
            RawKeyCode = rawCode;
        }
        public char? KeyPress { get; set; }
        public bool Handled { get; set; }
        public K RawKeyCode;
    }
}
