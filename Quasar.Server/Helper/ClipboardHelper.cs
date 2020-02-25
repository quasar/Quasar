using System;
using System.Windows.Forms;

namespace Quasar.Server.Helper
{
    public static class ClipboardHelper
    {
        public static void SetClipboardTextSafe(string text)
        {
            try
            {
                Clipboard.SetText(text);
            }
            catch (Exception)
            {
            }
        }
    }
}
