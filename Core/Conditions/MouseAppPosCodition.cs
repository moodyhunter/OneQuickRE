using OneQuick.SysX;
using System;
using System.Drawing;

namespace OneQuick.Core.Conditions
{
    public class MouseAppPosCodition : PositionCodition
    {
        public ProgramCodition App { get; set; }

        public MouseAppPosCodition()
        {
        }

        public MouseAppPosCodition(string AppFileName, PositionEnum pos)
        {
            App = new ProgramCodition(AppFileName);
            Position = new PositionWrapper(pos);
        }

        protected override bool IsFit()
        {
            IntPtr foregroundWindow = Win.GetForegroundWindow();
            if (App.FitbyHwnd(foregroundWindow))
            {
                Win.RECT windowRectangle = Win.GetWindowRectangle(new IntPtr?(foregroundWindow));
                Point point = Info.MousePos();
                return PtrFitRect(point.X, point.Y, windowRectangle.Left, windowRectangle.Top, windowRectangle.Right - windowRectangle.Left, windowRectangle.Bottom - windowRectangle.Top);
            }
            return false;
        }

        public override string ToString()
        {
            return App.ToString() + ": " + Position.ToString();
        }
    }
}
