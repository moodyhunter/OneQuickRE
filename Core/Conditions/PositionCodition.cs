namespace OneQuick.Core.Conditions
{
    public abstract class PositionCodition : Condition
    {
        public int Padding { get; set; } = 8;

        public PositionWrapper Position { get; set; }

        public virtual bool PtrFitRect(int X, int Y, int RectX, int RectY, int RectWidth, int RectHeight)
        {
            string text = "";
            int num = RectX + Padding;
            int num2 = RectX + RectWidth - Padding;
            if (X >= RectX && X < num)
            {
                text += "L";
            }
            else if (X >= num && X < num2)
            {
                text += "C";
            }
            else if (X >= num2 && X < RectX + RectWidth)
            {
                text += "R";
            }
            else
            {
                text += "N";
            }
            int num3 = RectY + Padding;
            int num4 = RectY + RectHeight - Padding;
            if (Y >= RectY && Y < num3)
            {
                text += "T";
            }
            else if (Y >= num3 && Y < num4)
            {
                text += "C";
            }
            else if (Y >= num4 && Y < RectY + RectHeight)
            {
                text += "B";
            }
            else
            {
                text += "N";
            }
            uint num5 = Internal.ComputeStringHash(text);
            PositionEnum positionEnum;
            if (num5 <= 1792671826u)
            {
                if (num5 <= 1726105803u)
                {
                    if (num5 != 1709328184u)
                    {
                        if (num5 == 1726105803u)
                        {
                            if (text == "LB")
                            {
                                positionEnum = PositionEnum.BottomLeft;
                                goto IL_241;
                            }
                        }
                    }
                    else if (text == "LC")
                    {
                        positionEnum = PositionEnum.CenterLeft;
                        goto IL_241;
                    }
                }
                else if (num5 != 1761926707u)
                {
                    if (num5 == 1792671826u)
                    {
                        if (text == "CT")
                        {
                            positionEnum = PositionEnum.TopCenter;
                            goto IL_241;
                        }
                    }
                }
                else if (text == "RT")
                {
                    positionEnum = PositionEnum.TopRight;
                    goto IL_241;
                }
            }
            else if (num5 <= 1977225635u)
            {
                if (num5 != 1960448016u)
                {
                    if (num5 == 1977225635u)
                    {
                        if (text == "CC")
                        {
                            positionEnum = PositionEnum.Center;
                            goto IL_241;
                        }
                    }
                }
                else if (text == "CB")
                {
                    positionEnum = PositionEnum.BottomCenter;
                    goto IL_241;
                }
            }
            else if (num5 != 2095213421u)
            {
                if (num5 != 2114256706u)
                {
                    if (num5 == 2131034325u)
                    {
                        if (text == "RB")
                        {
                            positionEnum = PositionEnum.BottomRight;
                            goto IL_241;
                        }
                    }
                }
                else if (text == "RC")
                {
                    positionEnum = PositionEnum.CenterRight;
                    goto IL_241;
                }
            }
            else if (text == "LT")
            {
                positionEnum = PositionEnum.TopLeft;
                goto IL_241;
            }
            positionEnum = PositionEnum.None;
        IL_241:
            return positionEnum != PositionEnum.None && Position.Position.Include(positionEnum);
        }
    }
}
