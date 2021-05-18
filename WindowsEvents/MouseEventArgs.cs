using System.Drawing;

namespace OneQuick.WindowsEvents
{
    [Flags]
    public enum MouseButtons
    {
        None = 0,
        Left = 1048576,
        Right = 2097152,
        Middle = 4194304,
        XButton1 = 8388608,
        XButton2 = 16777216
    }
    public class MouseEventArgs
    {
        public MouseEventArgs(MouseButtons buttons, int clicks, int x, int y, int delta)
        {
            Button = buttons;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
        }

        public bool Handled { get; set; }

        public MouseButtons Button { get; private set; }

        public int Clicks { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public int Delta { get; private set; }

        public Point Location => new Point(X, Y);
    }
}
