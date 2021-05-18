using OneQuick.Core.Triggers;
using OneQuick.WindowsEvents;
using OneQuick.WindowsSimulator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace OneQuick.Core
{
    internal static class EntryServer
    {
        public static int TimerIntervalMs { get; } = 100;

        public static ReadOnlyCollection<Trigger> Entrys => TriggerList.AsReadOnly();

        public static event Action EntrysChanged;

        private static void TriggerListChanged()
        {
            HotkeyTriggers = (from o in TriggerList.OfType<HotkeyTrigger>()
                              orderby o.Priority() descending
                              select o).ToList();
            InputTriggers = (from o in TriggerList.OfType<InputTrigger>()
                             orderby o.Priority() descending
                             select o).ToList();
            NonMoveTriggers = (from o in TriggerList.OfType<NonMoveTrigger>()
                               orderby o.Priority() descending
                               select o).ToList();
            Action entrysChanged = EntrysChanged;
            if (entrysChanged == null)
            {
                return;
            }
            entrysChanged();
        }

        public static event Action EnableChanged;

        public static bool EnableCounter
        {
            get => _enableCounter == 1;
            set
            {
                if (value)
                {
                    _enableCounter++;
                }
                else
                {
                    _enableCounter--;
                }
                Action enableChanged = EnableChanged;
                if (enableChanged == null)
                {
                    return;
                }
                enableChanged();
            }
        }

        public static string DebugStatus()
        {
            string text = "";
            return string.Concat(new object[]
            {
                text,
                _enableCounter,
                "(",
                TriggerList.Count,
                "): ",
                HotkeyTriggers.Count(),
                ", ",
                InputTriggers.Count(),
                ", ",
                NonMoveTriggers.Count()
            });
        }

        public static bool Contains(Trigger trigger)
        {
            return TriggerList.Contains(trigger);
        }

        public static bool ContainsLeastOne(IEnumerable<Trigger> triggers)
        {
            if (triggers == null)
            {
                return false;
            }
            using (IEnumerator<Trigger> enumerator = triggers.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (Contains(enumerator.Current))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ContainsAll(IEnumerable<Trigger> triggers)
        {
            if (triggers == null)
            {
                return false;
            }
            using (IEnumerator<Trigger> enumerator = triggers.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!Contains(enumerator.Current))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool Add(Trigger trigger)
        {
            if (trigger == null)
            {
                return false;
            }
            if (!Contains(trigger))
            {
                TriggerList.Add(trigger);
                TriggerListChanged();
                return true;
            }
            return false;
        }

        public static void Add(IEnumerable<Trigger> triggers)
        {
            if (triggers == null)
            {
                return;
            }
            foreach (Trigger trigger in triggers)
            {
                Add(trigger);
            }
        }

        public static bool Remove(Trigger trigger)
        {
            if (Contains(trigger))
            {
                TriggerList.Remove(trigger);
                TriggerListChanged();
                return true;
            }
            return false;
        }

        public static void Remove(IEnumerable<Trigger> triggers)
        {
            if (triggers == null)
            {
                return;
            }
            foreach (Trigger trigger in triggers)
            {
                Remove(trigger);
            }
        }

        public static void ResetTriggers()
        {
            foreach (Trigger trigger in TriggerList)
            {
                trigger.ResetState();
            }
        }

        private static void ResetNonMoveTriggers()
        {
            foreach (NonMoveTrigger nonMoveTrigger in NonMoveTriggers)
            {
                nonMoveTrigger.ResetState();
            }
        }

        static EntryServer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += Timer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, TimerIntervalMs);
            dispatcherTimer.Start();
            EventsServer.RawKeyboard += EventsServer_RawKeyboard;
            EventsServer.RawMouse += EventsServer_RawMouse;
            EventsServer.HotkeyDown += EventsServer_HotkeyDown;
            EventsServer.HotkeyUp += EventsServer_HotkeyUp;
            EventsServer.KeyUp += EventsServer_KeyUp;
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            foreach (NonMoveTrigger nonMoveTrigger in NonMoveTriggers)
            {
                nonMoveTrigger.TimePassed(TimerIntervalMs);
            }
        }

        private static void EventsServer_RawKeyboard(EventsServer.KeyboardHookStruct obj)
        {
            if (!EnableCounter)
            {
                return;
            }
            ResetNonMoveTriggers();
        }

        private static void EventsServer_RawMouse(EventsServer.MouseLLHookStruct arg1, MouseEventArgs arg2)
        {
            if (!EnableCounter)
            {
                return;
            }
            ResetNonMoveTriggers();
        }

        public static void HandleNextWinUp()
        {
            _handleNextWinUp = true;
        }

        private static void EventsServer_KeyUp(KeyEventArgs e)
        {
            if (_handleNextWinUp && (e.KeyCode == K.LWin || e.KeyCode == K.RWin))
            {
                SimulatorServer.KeyDown(K.LControlKey);
                SimulatorServer.KeyUp(K.LControlKey);
                _handleNextWinUp = false;
            }
        }

        private static void EventsServer_HotkeyDown(HotKeyEventArgs e)
        {
            if (!EnableCounter)
            {
                return;
            }
            HotkeyProc(e, true);
        }

        private static void EventsServer_HotkeyUp(HotKeyEventArgs e)
        {
            if (!EnableCounter)
            {
                return;
            }
            HotkeyProc(e, false);
        }

        private static void HotkeyProc(HotKeyEventArgs e, bool isKeyDown)
        {
            if (e.KeyPress != null)
            {
                KeyPressSequence += e.KeyPress.Value.ToString();
            }
            else if (isKeyDown && e.KeyData == K.Back && KeyPressSequence.Length > 0)
            {
                KeyPressSequence = KeyPressSequence.Substring(0, KeyPressSequence.Length - 1);
            }
            if (KeyPressSequence.Length > 10)
            {
                KeyPressSequence = KeyPressSequence.Substring(1);
            }
            HotkeySingle hotkeySingle = null;
            K rawKeyCode = e.RawKeyCode;
            if (HotkeySequence.Count == 0)
            {
                hotkeySingle = new HotkeySingle(e.KeyData);
            }
            else if (isKeyDown)
            {
                if (SealedLastHotkey)
                {
                    SealedLastHotkey = false;
                    hotkeySingle = new HotkeySingle(e.KeyData);
                }
                else
                {
                    HotkeySingle hotkeySingle2 = HotkeySequence.Last();
                    if ((e.KeyData | hotkeySingle2.KeyData) == e.KeyData)
                    {
                        HotkeySequence.Last().KeyData = e.KeyData;
                    }
                    else
                    {
                        hotkeySingle = new HotkeySingle(e.KeyData);
                    }
                }
            }
            else
            {
                K keyData = HotkeySequence.Last().KeyData;
                if (rawKeyCode == keyData || rawKeyCode.IsPrimeKey() || ((rawKeyCode == K.LControlKey || rawKeyCode == K.RControlKey) && keyData == K.Ctrl) || ((rawKeyCode == K.LWin || rawKeyCode == K.RWin) && keyData == K.Win) || ((rawKeyCode == K.LShiftKey || rawKeyCode == K.RShiftKey) && keyData == K.Shift) || ((rawKeyCode == K.LMenu || rawKeyCode == K.RMenu) && keyData == K.Alt))
                {
                    SealedLastHotkey = true;
                }
            }
            if (hotkeySingle != null)
            {
                if (rawKeyCode == K.LControlKey || rawKeyCode == K.LWin || rawKeyCode == K.LShiftKey || rawKeyCode == K.LMenu)
                {
                    hotkeySingle.ModSide = ModSide.Left;
                }
                else if (rawKeyCode == K.RControlKey || rawKeyCode == K.RWin || rawKeyCode == K.RShiftKey || rawKeyCode == K.RMenu)
                {
                    hotkeySingle.ModSide = ModSide.Right;
                }
                HotkeySequence.Add(hotkeySingle);
            }
            if (HotkeySequence.Count > 10)
            {
                HotkeySequence.RemoveAt(0);
            }
            if (Log.TraceEventsSwitch)
            {
                string text = "";
                text = string.Concat(new string[]
                {
                    text,
                    isKeyDown ? "↓ " : " ↑",
                    e.RawKeyCode.FormatString(),
                    ": ",
                    e.KeyData.FormatString()
                });
                text = string.Concat(new object[]
                {
                    text,
                    "       ",
                    KeyPressSequence.Length,
                    ":",
                    KeyPressSequence
                });
                text = string.Concat(new object[]
                {
                    text,
                    "       ",
                    HotkeySequence.Count,
                    ":",
                    string.Join(",", HotkeySequence)
                });
                Log.Trace(new string[]
                {
                    text
                });
            }
            bool flag = false;
            bool flag2 = false;
            foreach (HotkeyTrigger hotkeyTrigger in HotkeyTriggers)
            {
                flag = hotkeyTrigger.EventForward(HotkeySequence, isKeyDown, out bool flag3);
                flag2 = flag2 || flag3;
                if (flag)
                {
                    break;
                }
            }
            if (!flag)
            {
                if (isKeyDown && e.KeyData == K.Back)
                {
                    using (List<InputTrigger>.Enumerator enumerator2 = InputTriggers.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            InputTrigger inputTrigger = enumerator2.Current;
                            inputTrigger.Backspace();
                        }
                        goto IL_3DE;
                    }
                }
                if (e.KeyPress != null)
                {
                    foreach (InputTrigger inputTrigger2 in InputTriggers)
                    {
                        flag = inputTrigger2.EventForward(KeyPressSequence);
                        if (flag)
                        {
                            break;
                        }
                    }
                }
            }
        IL_3DE:
            if (flag)
            {
                ResetTriggers();
            }
            e.Handled = flag2;
        }

        private static string KeyPressSequence = "";

        private static readonly ObservableCollectionX<HotkeySingle> HotkeySequence = new ObservableCollectionX<HotkeySingle>();

        private static readonly List<Trigger> TriggerList = new List<Trigger>();

        private static List<HotkeyTrigger> HotkeyTriggers = new List<HotkeyTrigger>();

        private static List<InputTrigger> InputTriggers = new List<InputTrigger>();

        private static List<NonMoveTrigger> NonMoveTriggers = new List<NonMoveTrigger>();

        private static int _enableCounter = 0;

        private static bool _handleNextWinUp = false;

        private static bool SealedLastHotkey = false;
    }
}
