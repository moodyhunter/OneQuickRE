using OneQuick.Core;
using System.Collections.Generic;
using WindowsInput;
using WindowsInput.Native;

namespace OneQuick.WindowsSimulator
{
    internal static class SimulatorServer
    {
        public static void KeyDown(K Key)
        {
            EntryServer.EnableCounter = false;
            sim.Keyboard.KeyDown((VirtualKeyCode)Key.PrimeKey());
            EntryServer.EnableCounter = true;
        }

        public static void KeyUp(K Key)
        {
            EntryServer.EnableCounter = false;
            sim.Keyboard.KeyUp((VirtualKeyCode)Key.PrimeKey());
            EntryServer.EnableCounter = true;
        }

        public static void VSendKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, VirtualKeyCode keyCode)
        {
            EntryServer.EnableCounter = false;
            sim.Keyboard.ModifiedKeyStroke(modifierKeyCodes, keyCode);
            EntryServer.EnableCounter = true;
        }

        public static void VSendKeyStroke(VirtualKeyCode modifierKeyCode, VirtualKeyCode keyCode)
        {
            EntryServer.EnableCounter = false;
            sim.Keyboard.ModifiedKeyStroke(modifierKeyCode, keyCode);
            EntryServer.EnableCounter = true;
        }

        public static void VSendKeyPress(VirtualKeyCode keyCode)
        {
            EntryServer.EnableCounter = false;
            sim.Keyboard.KeyPress(keyCode);
            EntryServer.EnableCounter = true;
        }

        private static List<VirtualKeyCode> VKmodList(K k, bool LeftModifiers = true)
        {
            List<VirtualKeyCode> list = new List<VirtualKeyCode>();
            int num = LeftModifiers ? 0 : 1;
            if (k.Ctrl())
            {
                list.Add(VirtualKeyCode.LCONTROL + num);
            }
            if (k.Shift())
            {
                list.Add(VirtualKeyCode.LSHIFT + num);
            }
            if (k.Alt())
            {
                list.Add(VirtualKeyCode.LMENU + num);
            }
            if (k.Win())
            {
                list.Add(VirtualKeyCode.LWIN + num);
            }
            return list;
        }

        public static void SendKey(K Key, bool LeftModifiers = true)
        {
            VirtualKeyCode keyCode = (VirtualKeyCode)(Key & K.KeyCode);
            if (Key.ModifiersKeyPressed())
            {
                VSendKeyStroke(VKmodList(Key, LeftModifiers), keyCode);
                return;
            }
            VSendKeyPress(keyCode);
        }

        public static void SendText(string s, int backSpaceCount = 0)
        {
            VirtualKeyCode keyCode = VirtualKeyCode.BACK;
            EntryServer.EnableCounter = false;
            for (int i = 0; i < backSpaceCount; i++)
            {
                sim.Keyboard.KeyPress(keyCode);
            }
            sim.Keyboard.TextEntry(s);
            EntryServer.EnableCounter = true;
        }

        private static readonly InputSimulator sim = new InputSimulator();
    }
}
