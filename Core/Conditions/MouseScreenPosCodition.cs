using OneQuick.SysX;
using System.Drawing;

namespace OneQuick.Core.Conditions
{
    public class MouseScreenPosCodition : PositionCodition
    {
        protected override bool IsFit()
        {
            Point point = Info.MousePos();
            foreach (Rectangle rectangle in Info.AllMonitorsXYWH())
            {
                if (PtrFitRect(point.X, point.Y, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height))
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return "Screen: " + Position.ToString();
        }
    }
}
