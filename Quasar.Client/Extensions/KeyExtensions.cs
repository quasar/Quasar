using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Quasar.Client.Extensions
{
    public static class KeyExtensions
    {
        public static bool ContainsModifierKeys(this List<Keys> pressedKeys)
        {
            return pressedKeys.Any(x => x.IsModifierKey());
        }

        public static bool IsModifierKey(this Keys key)
        {
            return (key == Keys.LControlKey
                    || key == Keys.RControlKey
                    || key == Keys.LMenu
                    || key == Keys.RMenu
                    || key == Keys.LWin
                    || key == Keys.RWin
                    || key == Keys.Control
                    || key == Keys.Alt);
        }

        public static bool ContainsKeyChar(this List<Keys> pressedKeys, char c)
        {
            return pressedKeys.Contains((Keys)char.ToUpper(c));
        }

        public static bool IsExcludedKey(this Keys k)
        {
            // The keys below are excluded. If it is one of the keys below,
            // the KeyPress event will handle these characters. If the keys
            // are not any of those specified below, we can continue.
            return (k >= Keys.A && k <= Keys.Z
                    || k >= Keys.NumPad0 && k <= Keys.Divide
                    || k >= Keys.D0 && k <= Keys.D9
                    || k >= Keys.Oem1 && k <= Keys.OemClear
                    || k >= Keys.LShiftKey && k <= Keys.RShiftKey
                    || k == Keys.CapsLock
                    || k == Keys.Space);
        }

        public static string GetDisplayName(this Keys key)
        {
            string name = key.ToString();
            if (name.Contains("ControlKey"))
                return "Control";
            else if (name.Contains("Menu"))
                return "Alt";
            else if (name.Contains("Win"))
                return "Win";
            else if (name.Contains("Shift"))
                return "Shift";
            return name;
        }
    }
}
