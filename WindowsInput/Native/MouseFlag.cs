using System;

namespace WindowsInput.Native
{
    [Flags]
    internal enum MouseFlag : uint
    {
        Move = 1u,
        LeftDown = 2u,
        LeftUp = 4u,
        RightDown = 8u,
        RightUp = 16u,
        MiddleDown = 32u,
        MiddleUp = 64u,
        XDown = 128u,
        XUp = 256u,
        VerticalWheel = 2048u,
        HorizontalWheel = 4096u,
        VirtualDesk = 16384u,
        Absolute = 32768u
    }
}
