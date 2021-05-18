namespace OneQuick.WindowsEvents
{
    public class KeyPressEventArgs
    {
        public KeyPressEventArgs(char keyChar)
        {
            KeyChar = keyChar;
        }

        public char KeyChar { get; set; }

        public bool Handled { get; set; }
    }
}
