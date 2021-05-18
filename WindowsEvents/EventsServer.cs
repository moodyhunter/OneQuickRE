using OneQuick.WindowsSimulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OneQuick.WindowsEvents
{
    public static class EventsServer
    {
        private static void InstallKeyboardHook()
        {
            if (_keyboardHookHandle == 0)
            {
                _keyboardProcDelegate = new HookProc(KeyboardLLHookProc);
                _keyboardHookHandle = SetWindowsHookEx(13, _keyboardProcDelegate, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                if (_keyboardHookHandle == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private static void UnInstallKeyboardHook()
        {
            if (_keyboardHookHandle != 0)
            {
                bool flag = UnhookWindowsHookEx(_keyboardHookHandle) != 0;
                _keyboardHookHandle = 0;
                _keyboardProcDelegate = null;
                if (!flag)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private static int KeyboardLLHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0 && Log.TraceEventsSwitch)
            {
                /*ref*/
                KeyboardHookStruct ptr = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                bool flag = wParam == 256 || wParam == 260;
                K virtualKeyCode = (K)ptr.VirtualKeyCode;
                K modKeyState = GetModKeyState();
                Log.Trace(new string[]
                {
                    string.Concat(new string[]
                    {
                        "                ",
                        flag ? "↓ " : " ↑",
                        virtualKeyCode.FormatString(),
                        "(",
                        modKeyState.FormatString(),
                        ")"
                    })
                });
            }
            if (Enable && nCode >= 0)
            {
                KeyboardHookStruct keyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                if (keyboardHookStruct.VirtualKeyCode == 0)
                {
                    return CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
                }
                bool flag2 = wParam == 256 || wParam == 260;
                bool flag3 = wParam == 257 || wParam == 261;
                _rawKeyboard(keyboardHookStruct);
                K modKeyState2 = GetModKeyState();
                if (flag2)
                {
                    K virtualKeyCode2 = (K)keyboardHookStruct.VirtualKeyCode;
                    if (_keyDown.Count > 0)
                    {
                        KeyEventArgs keyEventArgs = new KeyEventArgs(virtualKeyCode2 | modKeyState2);
                        foreach (KeyEvent keyEvent in _keyDown)
                        {
                            keyEvent(keyEventArgs);
                            if (keyEventArgs.Handled)
                            {
                                return -1;
                            }
                        }
                    }
                    char? keyPress = null;
                    if (wParam == 256)
                    {
                        bool shiftKeyDown = ShiftKeyDown;
                        bool keyState = GetKeyState(20) != 0;
                        byte[] array = new byte[256];
                        GetKeyboardState(array);
                        byte[] array2 = new byte[2];
                        if (ToAscii(keyboardHookStruct.VirtualKeyCode, keyboardHookStruct.ScanCode, array, array2, keyboardHookStruct.Flags) == 1)
                        {
                            char c = (char)array2[0];
                            if (!char.IsControl(c))
                            {
                                keyPress = new char?(c);
                                if ((keyState ^ shiftKeyDown) && char.IsLetter(c))
                                {
                                    keyPress = new char?(char.ToUpper(c));
                                }
                            }
                        }
                    }
                    if (_hotkeyDown.Count > 0)
                    {
                        HotKeyEventArgs hotKeyEventArgs = new HotKeyEventArgs(GetHotkeyArgKey(virtualKeyCode2, modKeyState2), virtualKeyCode2)
                        {
                            KeyPress = keyPress
                        };
                        foreach (HotkeyEvent hotkeyEvent in _hotkeyDown)
                        {
                            hotkeyEvent(hotKeyEventArgs);
                            if (hotKeyEventArgs.Handled)
                            {
                                return -1;
                            }
                        }
                    }
                    if (keyPress != null && _keyPress.Count > 0)
                    {
                        KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs(keyPress.Value);
                        foreach (KeyPressEvent keyPressEvent in _keyPress)
                        {
                            keyPressEvent(keyPressEventArgs);
                            if (keyPressEventArgs.Handled)
                            {
                                return -1;
                            }
                        }
                    }
                }
                if (flag3)
                {
                    K virtualKeyCode3 = (K)keyboardHookStruct.VirtualKeyCode;
                    if (_hotkeyUp.Count > 0)
                    {
                        HotKeyEventArgs hotKeyEventArgs2 = new HotKeyEventArgs(GetHotkeyArgKey(virtualKeyCode3, modKeyState2), virtualKeyCode3);
                        foreach (HotkeyEvent hotkeyEvent2 in _hotkeyUp)
                        {
                            hotkeyEvent2(hotKeyEventArgs2);
                            if (hotKeyEventArgs2.Handled)
                            {
                                return -1;
                            }
                        }
                    }
                    if (_keyUp.Count > 0)
                    {
                        KeyEventArgs keyEventArgs2 = new KeyEventArgs(virtualKeyCode3 | modKeyState2);
                        foreach (KeyEvent keyEvent2 in _keyUp)
                        {
                            keyEvent2(keyEventArgs2);
                            if (keyEventArgs2.Handled)
                            {
                                return -1;
                            }
                        }
                    }
                }
                if (!KeyMaps.ContainsKey(keyboardHookStruct.VirtualKeyCode))
                {
                    goto IL_49E;
                }
                G.TriggerCounter();
                int virtualKeyCode4 = keyboardHookStruct.VirtualKeyCode;
                int num = KeyMaps[virtualKeyCode4];
                if (flag2)
                {
                    Enable = false;
                    SimulatorServer.KeyDown((K)num);
                    Enable = true;
                    if (Log.VerboseSwitch)
                    {
                        Log.Verbose(new string[]
                        {
                            "KeyboardLLHookProc, KeyRemapSimu, ↓ " + ((K)virtualKeyCode4).FormatString() + " >> " + ((K)num).FormatString()
                        });
                    }
                    return -1;
                }
                if (flag3)
                {
                    Enable = false;
                    SimulatorServer.KeyUp((K)num);
                    if (Log.VerboseSwitch)
                    {
                        Log.Verbose(new string[]
                        {
                            "KeyboardLLHookProc, KeyRemapSimu,  ↑" + ((K)virtualKeyCode4).FormatString() + " >> " + ((K)num).FormatString()
                        });
                    }
                    Enable = true;
                    return -1;
                }
            }
        IL_49E:
            return CallNextHookEx(_keyboardHookHandle, nCode, wParam, lParam);
        }

        private static K GetHotkeyArgKey(K keyCode, K Modkey)
        {
            K result;
            switch (keyCode)
            {
                case K.LShiftKey:
                case K.RShiftKey:
                    result = Modkey | K.Shift;
                    break;
                case K.LControlKey:
                case K.RControlKey:
                    result = Modkey | K.Ctrl;
                    break;
                case K.LMenu:
                case K.RMenu:
                    result = Modkey | K.Alt;
                    break;
                case K.LWin:
                case K.RWin:
                    result = Modkey | K.Win;
                    break;
                default:
                    result = Modkey | keyCode;
                    break;
            }
            return result;
        }

        public static K GetModKeyState()
        {
            K k = K.None;
            if (ShiftKeyDown)
            {
                k |= K.Shift;
            }
            if (AltKeyDown)
            {
                k |= K.Alt;
            }
            if (CtrlKeyDown)
            {
                k |= K.Ctrl;
            }
            if (WinKeyDown)
            {
                k |= K.Win;
            }
            return k;
        }

        public static bool LCtrl => IsKeyDownState(162);

        public static bool RCtrl => IsKeyDownState(163);

        public static bool LShift => IsKeyDownState(160);

        public static bool RShift => IsKeyDownState(161);

        public static bool LAlt => IsKeyDownState(164);

        public static bool RAlt => IsKeyDownState(165);

        public static bool LWin => IsKeyDownState(91);

        public static bool RWin => IsKeyDownState(92);

        public static bool ShiftKeyDown => IsKeyDownState(16);

        public static bool CtrlKeyDown => IsKeyDownState(17);

        public static bool AltKeyDown => IsKeyDownState(18);

        public static bool WinKeyDown => LWin || RWin;

        public static bool IsKeyDownState(byte VK)
        {
            return (GetKeyState(VK) & 128) == 128;
        }

        private static void InstallMouseHook()
        {
            if (_mouseHookHandle == 0)
            {
                _mouseProcDelegate = new HookProc(MouseLLHookProc);
                _mouseHookHandle = SetWindowsHookEx(14, _mouseProcDelegate, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                if (_mouseHookHandle == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private static void UnInstallMouseHook()
        {
            if (_mouseHookHandle != 0)
            {
                bool flag = UnhookWindowsHookEx(_mouseHookHandle) != 0;
                _mouseHookHandle = 0;
                _mouseProcDelegate = null;
                if (!flag)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private static int MouseLLHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (Enable && nCode >= 0)
            {
                MouseLLHookStruct mouseLLHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));
                MouseButtons buttons = MouseButtons.None;
                short num = 0;
                int clicks = 0;
                bool flag = false;
                bool flag2 = false;
                K k = K.None;
                switch (wParam)
                {
                    case 513:
                        flag = true;
                        buttons = MouseButtons.Left;
                        k = K.LButton;
                        clicks = 1;
                        break;
                    case 514:
                        flag2 = true;
                        buttons = MouseButtons.Left;
                        k = K.LButton;
                        clicks = 1;
                        break;
                    case 515:
                        buttons = MouseButtons.Left;
                        k = K.LButton;
                        clicks = 2;
                        break;
                    case 516:
                        flag = true;
                        buttons = MouseButtons.Right;
                        k = K.RButton;
                        clicks = 1;
                        break;
                    case 517:
                        flag2 = true;
                        buttons = MouseButtons.Right;
                        k = K.RButton;
                        clicks = 1;
                        break;
                    case 518:
                        buttons = MouseButtons.Right;
                        k = K.RButton;
                        clicks = 2;
                        break;
                    case 519:
                        flag = true;
                        buttons = MouseButtons.Middle;
                        k = K.MButton;
                        clicks = 1;
                        break;
                    case 520:
                        flag2 = true;
                        buttons = MouseButtons.Middle;
                        k = K.MButton;
                        clicks = 1;
                        break;
                    case 521:
                        buttons = MouseButtons.Middle;
                        k = K.MButton;
                        clicks = 2;
                        break;
                    case 522:
                        num = (short)(mouseLLHookStruct.MouseData >> 16 & 65535);
                        if (num > 0)
                        {
                            k = K.WheelUp;
                        }
                        else
                        {
                            k = K.WheelDown;
                        }
                        break;
                }
                MouseEventArgs mouseEventArgs = new MouseEventArgs(buttons, clicks, mouseLLHookStruct.Point.X, mouseLLHookStruct.Point.Y, num);
                _rawMouse?.Invoke(mouseLLHookStruct, mouseEventArgs);
                if (flag)
                {
                    foreach (MouseEvent mouseEvent in _mouseDown)
                    {
                        mouseEvent(mouseEventArgs);
                        if (mouseEventArgs.Handled)
                        {
                            return -1;
                        }
                    }
                }
                if (flag2)
                {
                    foreach (MouseEvent mouseEvent2 in _mouseUp)
                    {
                        mouseEvent2(mouseEventArgs);
                        if (mouseEventArgs.Handled)
                        {
                            return -1;
                        }
                    }
                }
                if (num != 0)
                {
                    foreach (MouseEvent mouseEvent3 in _mouseWheel)
                    {
                        mouseEvent3(mouseEventArgs);
                        if (mouseEventArgs.Handled)
                        {
                            return -1;
                        }
                    }
                }
                if (MousePosX != mouseLLHookStruct.Point.X || MousePosY != mouseLLHookStruct.Point.Y)
                {
                    MousePosX = mouseLLHookStruct.Point.X;
                    MousePosY = mouseLLHookStruct.Point.Y;
                    foreach (MouseEvent mouseEvent4 in _mouseMove)
                    {
                        mouseEvent4(mouseEventArgs);
                        if (mouseEventArgs.Handled)
                        {
                            return -1;
                        }
                    }
                }
                if (k != K.None)
                {
                    K modKeyState = GetModKeyState();
                    HotKeyEventArgs hotKeyEventArgs = new HotKeyEventArgs(k | modKeyState, k);
                    if (flag2)
                    {
                        using (List<HotkeyEvent>.Enumerator enumerator2 = _hotkeyUp.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                HotkeyEvent hotkeyEvent = enumerator2.Current;
                                hotkeyEvent(hotKeyEventArgs);
                                if (hotKeyEventArgs.Handled)
                                {
                                    return -1;
                                }
                            }
                            goto IL_367;
                        }
                    }
                    foreach (HotkeyEvent hotkeyEvent2 in _hotkeyDown)
                    {
                        hotkeyEvent2(hotKeyEventArgs);
                        if (hotKeyEventArgs.Handled)
                        {
                            return -1;
                        }
                    }
                }
            IL_367:
                if (k == K.None || !KeyMaps.ContainsKey((int)k))
                {
                    goto IL_517;
                }
                G.TriggerCounter();
                int num2 = KeyMaps[(int)k];
                if (flag)
                {
                    Enable = false;
                    SimulatorServer.KeyDown((K)num2);
                    Enable = true;
                    if (Log.TraceEventsSwitch)
                    {
                        Log.Trace(new string[]
                        {
                            string.Format("MouseLLHookProc, KeyRemapSimu, ↓ {0} >> {1} ({2}, {3})", new object[]
                            {
                                mouseEventArgs.Button,
                                (K)num2,
                                mouseEventArgs.X,
                                mouseEventArgs.Y
                            })
                        });
                    }
                    return -1;
                }
                if (flag2)
                {
                    Enable = false;
                    SimulatorServer.KeyUp((K)num2);
                    if (Log.TraceEventsSwitch)
                    {
                        Log.Trace(new string[]
                        {
                            string.Format("MouseLLHookProc, KeyRemapSimu,  ↑{0} >> {1} ({2}, {3})", new object[]
                            {
                                mouseEventArgs.Button,
                                (K)num2,
                                mouseEventArgs.X,
                                mouseEventArgs.Y
                            })
                        });
                    }
                    Enable = true;
                    return -1;
                }
                if (num != 0)
                {
                    Enable = false;
                    SimulatorServer.KeyDown((K)num2);
                    SimulatorServer.KeyUp((K)num2);
                    if (Log.TraceEventsSwitch)
                    {
                        Log.Trace(new string[]
                        {
                            string.Format("MouseLLHookProc, KeyRemapSimu, {0}, {1} ({2}, {3})", new object[]
                            {
                                (mouseEventArgs.Delta > 0) ? "↑↑" : "↓↓",
                                (K)num2,
                                mouseEventArgs.X,
                                mouseEventArgs.Y
                            })
                        });
                    }
                    Enable = true;
                    return -1;
                }
            }
        IL_517:
            return CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32")]
        private static extern int GetDoubleClickTime();

        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern short GetKeyState(int vKey);

        public static event Action<bool> EnableChanged;

        public static bool Enable
        {
            get => _enable;
            set
            {
                if (_enable != value)
                {
                    _enable = value;
                    if (_enable)
                    {
                        InstallHooks();
                    }
                    else
                    {
                        UnInstallHooks();
                    }
                    Action<bool> enableChanged = EnableChanged;
                    if (enableChanged == null)
                    {
                        return;
                    }
                    enableChanged(value);
                }
            }
        }

        static EventsServer()
        {
            _enable = false;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            UnInstallHooks();
        }

        private static void InstallHooks()
        {
            InstallKeyboardHook();
            InstallMouseHook();
        }

        private static void UnInstallHooks()
        {
            UnInstallKeyboardHook();
            UnInstallMouseHook();
        }

        public static int MousePosX { get; private set; }

        public static int MousePosY { get; private set; }

        private static event Action<KeyboardHookStruct> _rawKeyboard;

        public static event Action<KeyboardHookStruct> RawKeyboard
        {
            add
            {
                _rawKeyboard += value;
            }
            remove
            {
                _rawKeyboard -= value;
            }
        }

        public static event KeyPressEvent KeyPress
        {
            add
            {
                if (!_keyPress.Contains(value))
                {
                    _keyPress.Insert(0, value);
                }
            }
            remove
            {
                _keyPress.Remove(value);
            }
        }

        public static event KeyEvent KeyDown
        {
            add
            {
                if (!_keyDown.Contains(value))
                {
                    _keyDown.Insert(0, value);
                }
            }
            remove
            {
                _keyDown.Remove(value);
            }
        }

        public static event KeyEvent KeyUp
        {
            add
            {
                if (!_keyUp.Contains(value))
                {
                    _keyUp.Insert(0, value);
                }
            }
            remove
            {
                _keyUp.Remove(value);
            }
        }

        private static event Action<MouseLLHookStruct, MouseEventArgs> _rawMouse;

        public static event Action<MouseLLHookStruct, MouseEventArgs> RawMouse
        {
            add
            {
                _rawMouse += value;
            }
            remove
            {
                _rawMouse -= value;
            }
        }

        public static event MouseEvent MouseDown
        {
            add
            {
                if (!_mouseDown.Contains(value))
                {
                    _mouseDown.Insert(0, value);
                }
            }
            remove
            {
                _mouseDown.Remove(value);
            }
        }

        public static event MouseEvent MouseUp
        {
            add
            {
                if (!_mouseUp.Contains(value))
                {
                    _mouseUp.Insert(0, value);
                }
            }
            remove
            {
                _mouseUp.Remove(value);
            }
        }

        public static event MouseEvent MouseWheel
        {
            add
            {
                if (!_mouseWheel.Contains(value))
                {
                    _mouseWheel.Insert(0, value);
                }
            }
            remove
            {
                _mouseWheel.Remove(value);
            }
        }

        public static event MouseEvent MouseMove
        {
            add
            {
                if (!_mouseMove.Contains(value))
                {
                    _mouseMove.Insert(0, value);
                }
            }
            remove
            {
                _mouseMove.Remove(value);
            }
        }

        public static event HotkeyEvent HotkeyDown
        {
            add
            {
                if (!_hotkeyDown.Contains(value))
                {
                    _hotkeyDown.Insert(0, value);
                }
            }
            remove
            {
                _hotkeyDown.Remove(value);
            }
        }

        public static event HotkeyEvent HotkeyUp
        {
            add
            {
                if (!_hotkeyUp.Contains(value))
                {
                    _hotkeyUp.Insert(0, value);
                }
            }
            remove
            {
                _hotkeyUp.Remove(value);
            }
        }

        private static HookProc _keyboardProcDelegate;
        private static int _keyboardHookHandle = 0;
        private static HookProc _mouseProcDelegate;
        private static int _mouseHookHandle = 0;
        private static bool _enable;
        public static Dictionary<int, int> KeyMaps = new Dictionary<int, int>();
        private static readonly List<KeyPressEvent> _keyPress = new List<KeyPressEvent>();
        private static readonly List<KeyEvent> _keyDown = new List<KeyEvent>();
        private static readonly List<KeyEvent> _keyUp = new List<KeyEvent>();
        private static readonly List<MouseEvent> _mouseDown = new List<MouseEvent>();
        private static readonly List<MouseEvent> _mouseUp = new List<MouseEvent>();
        private static readonly List<MouseEvent> _mouseWheel = new List<MouseEvent>();
        private static readonly List<MouseEvent> _mouseMove = new List<MouseEvent>();
        private static readonly List<HotkeyEvent> _hotkeyDown = new List<HotkeyEvent>();
        private static readonly List<HotkeyEvent> _hotkeyUp = new List<HotkeyEvent>();

        public struct KeyboardHookStruct
        {
            public int VirtualKeyCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public int ExtraInfo;
        }

        public struct Point
        {
            public int X;
            public int Y;
        }

        public struct MouseLLHookStruct
        {
            public Point Point;
            public int MouseData;
            public int Flags;
            public int Time;
            public int ExtraInfo;
        }

        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);
        public delegate void KeyPressEvent(KeyPressEventArgs e);
        public delegate void KeyEvent(KeyEventArgs e);
        public delegate void HotkeyEvent(HotKeyEventArgs e);
        public delegate void MouseEvent(MouseEventArgs e);
    }
}
