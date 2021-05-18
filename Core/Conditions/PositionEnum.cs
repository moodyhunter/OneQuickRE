using System;
using System.Linq;

namespace OneQuick.Core.Conditions
{
    public enum PositionEnum
    {
        None,
        TopLeft,
        TopCenter,
        TopRight = 0b100,
        CenterRight = 0b1000,
        BottomRight = 0b10000,
        BottomCenter = 0b100000,
        BottomLeft = 0b1000000,
        CenterLeft = 0b10000000,
        Center = 0b100000000,
        Corner = 85,
        Edge = 170,
        Anywhere = 511,
        TopRow = 7,
        CenterRow = 392,
        BottomRow = 112,
        LeftColumn = 193,
        CenterColumn = 290,
        RightColumn = 28
    }

    public static class PositionEnumExt
    {
        public static bool Include(this PositionEnum p, PositionEnum pos)
        {
            return (p & pos) == pos;
        }

        public static bool IncludedBy(this PositionEnum p, PositionEnum pos)
        {
            return pos.Include(p);
        }

        public static bool OverlapTo(this PositionEnum p, PositionEnum pos)
        {
            return (p & pos) > PositionEnum.None;
        }

        public static int PartsCount(this PositionEnum p)
        {
            return Convert.ToString((int)p, 2).ToCharArray().Count((char c) => c == '1');
        }

        public static string LocalString(this PositionEnum p)
        {
            return p.ToString();
        }
    }
}
