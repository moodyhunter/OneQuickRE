using OneQuick.WindowsEvents;
using OneQuick.WindowsSimulator;
using System.Xml.Serialization;

namespace OneQuick.Core.Operations
{
    public class SendKey : Operation
    {
        [XmlIgnore]
        public K Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged("Key");
            }
        }

        public int KeyValue
        {
            get => (int)Key;
            set
            {
                Key = (K)value;
                OnPropertyChanged("Key");
            }
        }

        public bool LeftModifiers
        {
            get => _leftModifiers;
            set
            {
                _leftModifiers = value;
                OnPropertyChanged("LeftModifiers");
            }
        }

        public SendKey()
        {
            Mode = SyncMode.Task;
        }

        public SendKey(K k) : this()
        {
            Key = k;
        }

        protected override bool EmptyParameter()
        {
            return Key == K.None;
        }

        protected override void _invoke()
        {
            if (Log.VerboseSwitch)
            {
                Log.Verbose(new string[]
                {
                    ">>" + Key.FormatString() + ", " + EventsServer.GetModKeyState().FormatString()
                });
            }
            bool flag = TryKeyUp(Key, K.Alt, EventsServer.LAlt, K.LMenu);
            bool flag2 = TryKeyUp(Key, K.Alt, EventsServer.RAlt, K.RMenu);
            bool flag3 = TryKeyUp(Key, K.Ctrl, EventsServer.LCtrl, K.LControlKey);
            bool flag4 = TryKeyUp(Key, K.Ctrl, EventsServer.RCtrl, K.RControlKey);
            bool flag5 = TryKeyUp(Key, K.Shift, EventsServer.LShift, K.LShiftKey);
            bool flag6 = TryKeyUp(Key, K.Shift, EventsServer.RShift, K.RShiftKey);
            bool flag7 = TryKeyUp(Key, K.Win, EventsServer.LWin, K.LWin);
            bool flag8 = TryKeyUp(Key, K.Win, EventsServer.RWin, K.RWin);
            if (Log.VerboseSwitch)
            {
                Log.Verbose(new string[]
                {
                    ">>>>" + Key.FormatString() + ", " + EventsServer.GetModKeyState().FormatString()
                });
            }
            SimulatorServer.SendKey(Key, LeftModifiers);
            if (Log.VerboseSwitch)
            {
                Log.Verbose(new string[]
                {
                    "<<<<" + Key.FormatString() + ", " + EventsServer.GetModKeyState().FormatString()
                });
            }
            if (flag8)
            {
                SimulatorServer.KeyDown(K.RWin);
            }
            if (flag7)
            {
                SimulatorServer.KeyDown(K.LWin);
            }
            if (flag6)
            {
                SimulatorServer.KeyDown(K.RShiftKey);
            }
            if (flag5)
            {
                SimulatorServer.KeyDown(K.LShiftKey);
            }
            if (flag4)
            {
                SimulatorServer.KeyDown(K.RControlKey);
            }
            if (flag3)
            {
                SimulatorServer.KeyDown(K.LControlKey);
            }
            if (flag2)
            {
                SimulatorServer.KeyDown(K.RMenu);
            }
            if (flag)
            {
                SimulatorServer.KeyDown(K.LMenu);
            }
            if (Log.VerboseSwitch)
            {
                Log.Verbose(new string[]
                {
                    "<<" + Key.FormatString() + ", " + EventsServer.GetModKeyState().FormatString()
                });
            }
        }

        private bool TryKeyUp(K SendKey, K ModMask, bool ModKeyDown, K SendUpModCode)
        {
            if ((SendKey & ModMask) != ModMask && ModKeyDown)
            {
                SimulatorServer.KeyUp(SendUpModCode);
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return Key.FormatString();
        }

        private K _key;

        private bool _leftModifiers = true;
    }
}
