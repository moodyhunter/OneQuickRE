namespace OneQuick
{
    public enum K
    {
        Modifiers = -65536,
        KeyCode = 65535,
        Shift,
        Ctrl = 131072,
        Alt = 262144,
        Win = 524288,
        WheelDown = 4096,
        WheelUp,
        WheelLeft,
        WheelRight,
        None = 0,
        LButton,
        RButton,
        Cancel,
        MButton,
        XButton1,
        XButton2,
        Back = 8,
        Tab,
        LineFeed,
        Clear = 12,
        Return,
        Enter = 13,
        ShiftKey = 16,
        ControlKey,
        Menu,
        Pause,
        Capital,
        CapsLock = 20,
        KanaMode,
        HanguelMode = 21,
        HangulMode = 21,
        JunjaMode = 23,
        FinalMode,
        HanjaMode,
        KanjiMode = 25,
        Escape = 27,
        IMEConvert,
        IMENonconvert,
        IMEAccept,
        IMEAceept = 30,
        IMEModeChange,
        Space,
        PageUp,
        PageDown,
        End,
        Home,
        Left,
        Up,
        Right,
        Down,
        Select,
        Print,
        Execute,
        Snapshot,
        PrintScreen = 44,
        Insert,
        Delete,
        Help,
        D0,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        D8,
        D9,
        A = 65,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        LWin,
        RWin,
        Apps,
        Sleep = 95,
        NumPad0,
        NumPad1,
        NumPad2,
        NumPad3,
        NumPad4,
        NumPad5,
        NumPad6,
        NumPad7,
        NumPad8,
        NumPad9,
        Multiply,
        Add,
        Separator,
        Subtract,
        Decimal,
        Divide,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,
        F13,
        F14,
        F15,
        F16,
        F17,
        F18,
        F19,
        F20,
        F21,
        F22,
        F23,
        F24,
        NumLock = 144,
        Scroll,
        LShiftKey = 160,
        RShiftKey,
        LControlKey,
        RControlKey,
        LMenu,
        RMenu,
        BrowserBack,
        BrowserForward,
        BrowserRefresh,
        BrowserStop,
        BrowserSearch,
        BrowserFavorites,
        BrowserHome,
        VolumeMute,
        VolumeDown,
        VolumeUp,
        MediaNextTrack,
        MediaPreviousTrack,
        MediaStop,
        MediaPlayPause,
        LaunchMail,
        SelectMedia,
        LaunchApplication1,
        LaunchApplication2,
        OemSemicolon = 186,
        Oem1 = 186,
        Oemplus,
        Oemcomma,
        OemMinus,
        OemPeriod,
        OemQuestion,
        Oem2 = 191,
        Oemtilde,
        Oem3 = 192,
        OemOpenBrackets = 219,
        Oem4 = 219,
        OemPipe,
        Oem5 = 220,
        OemCloseBrackets,
        Oem6 = 221,
        OemQuotes,
        Oem7 = 222,
        Oem8,
        OemBackslash = 226,
        Oem102 = 226,
        ProcessKey = 229,
        Packet = 231,
        Attn = 246,
        Crsel,
        Exsel,
        EraseEof,
        Play,
        Zoom,
        NoName,
        Pa1,
        OemClear
    }
    public static class KExt
    {
        public static bool Contains(this K k, K x)
        {
            K k2 = k & K.KeyCode;
            K k3 = x & K.KeyCode;
            K k4 = x & K.Modifiers;
            return (k3 == K.None || k3 == k2) && (k & k4) == k4;
        }

        public static K PrimeKey(this K k)
        {
            return k & K.KeyCode;
        }

        public static K ModifiersKey(this K k)
        {
            return k & K.Modifiers;
        }

        public static bool PrimeIsModifiers(this K k)
        {
            k &= K.KeyCode;
            return k == K.LControlKey || k == K.RControlKey || k == K.LShiftKey || k == K.RShiftKey || k == K.LMenu || k == K.RMenu || k == K.LWin || k == K.RWin;
        }

        public static bool Ctrl(this K k)
        {
            return (k & K.Ctrl) > K.None;
        }

        public static bool Shift(this K k)
        {
            return (k & K.Shift) > K.None;
        }

        public static bool Alt(this K k)
        {
            return (k & K.Alt) > K.None;
        }

        public static bool Win(this K k)
        {
            return (k & K.Win) > K.None;
        }

        public static bool IsPrimeKey(this K k)
        {
            return k.PrimeKeyPressed() && !k.ModifiersKeyPressed();
        }

        public static K ToModifiersKey(this K k)
        {
            K k2 = k.PrimeKey();
            if (k2 - K.LWin <= 1)
            {
                return k.ModifiersKey() | K.Win;
            }
            switch (k2)
            {
                case K.LShiftKey:
                case K.RShiftKey:
                    return k.ModifiersKey() | K.Shift;
                case K.LControlKey:
                case K.RControlKey:
                    return k.ModifiersKey() | K.Ctrl;
                case K.LMenu:
                case K.RMenu:
                    return k.ModifiersKey() | K.Alt;
                default:
                    return k.ModifiersKey();
            }
        }

        public static bool PrimeKeyPressed(this K k)
        {
            return (k & K.KeyCode) > K.None;
        }

        public static bool ModifiersKeyPressed(this K k)
        {
            return (k & K.Modifiers) > K.None;
        }

        public static int ModifiersKeyCount(this K k)
        {
            return (k.Ctrl() ? 1 : 0) + (k.Shift() ? 1 : 0) + (k.Alt() ? 1 : 0) + (k.Win() ? 1 : 0);
        }

        public static string FormatString(this K k)
        {
            string text = "";
            if (k.Ctrl())
            {
                text += "Ctrl_";
            }
            if (k.Shift())
            {
                text += "Shift_";
            }
            if (k.Alt())
            {
                text += "Alt_";
            }
            if (k.Win())
            {
                text += "Win_";
            }
            if (k.PrimeKeyPressed())
            {
                text += (k & K.KeyCode).ToString();
            }
            else if (text.Length >= 1)
            {
                text = text.Substring(0, text.Length - 1);
            }
            return text;
        }
    }
}
