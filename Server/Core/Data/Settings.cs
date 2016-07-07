using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace xServer.Core.Data
{
    public static class Settings
    {
        private static readonly string SettingsPath = Path.Combine(Application.StartupPath, "settings.xml");

        public static string RepositoryURL = @"https://github.com/quasar/QuasarRAT";

        public static ushort ListenPort
        {
            get
            {
                return ushort.Parse(ReadValueSafe("ListenPort", "4782"));
            }
            set
            {
                WriteValue("ListenPort", value.ToString());
            }
        }

        public static bool IPv6Support
        {
            get
            {
                return bool.Parse(ReadValueSafe("IPv6Support", "False"));
            }
            set
            {
                WriteValue("IPv6Support", value.ToString());
            }
        }

        public static bool ShowToU
        {
            get
            {
                return bool.Parse(ReadValueSafe("ShowToU", "True"));
            }
            set
            {
                WriteValue("ShowToU", value.ToString());
            }
        }

        public static bool AutoListen
        {
            get
            {
                return bool.Parse(ReadValueSafe("AutoListen", "False"));
            }
            set
            {
                WriteValue("AutoListen", value.ToString());
            }
        }

        public static bool ShowPopup
        {
            get
            {
                return bool.Parse(ReadValueSafe("ShowPopup", "False"));
            }
            set
            {
                WriteValue("ShowPopup", value.ToString());
            }
        }

        public static bool UseUPnP
        {
            get
            {
                return bool.Parse(ReadValueSafe("UseUPnP", "False"));
            }
            set
            {
                WriteValue("UseUPnP", value.ToString());
            }
        }

        public static bool ShowToolTip
        {
            get
            {
                return bool.Parse(ReadValueSafe("ShowToolTip", "False"));
            }
            set
            {
                WriteValue("ShowToolTip", value.ToString());
            }
        }

        public static string Password
        {
            get
            {
                return ReadValueSafe("Password", "1234");
            }
            set
            {
                WriteValue("Password", value);
            }
        }

        public static bool EnableNoIPUpdater
        {
            get
            {
                return bool.Parse(ReadValueSafe("EnableNoIPUpdater", "False"));
            }
            set
            {
                WriteValue("EnableNoIPUpdater", value.ToString());
            }
        }

        public static string NoIPHost
        {
            get
            {
                return ReadValueSafe("NoIPHost");
            }
            set
            {
                WriteValue("NoIPHost", value);
            }
        }

        public static string NoIPUsername
        {
            get
            {
                return ReadValueSafe("NoIPUsername");
            }
            set
            {
                WriteValue("NoIPUsername", value);
            }
        }

        public static string NoIPPassword
        {
            get
            {
                return ReadValueSafe("NoIPPassword");
            }
            set
            {
                WriteValue("NoIPPassword", value);
            }
        }

        public static string SaveFormat
        {
            get
            {
                return ReadValueSafe("SaveFormat", "APP - URL - USER:PASS");
            }
            set
            {
                WriteValue("SaveFormat", value);
            }
        }

        public static ushort ReverseProxyPort
        {
            get
            {
                return ushort.Parse(ReadValueSafe("ReverseProxyPort", "3128"));
            }
            set
            {
                WriteValue("ReverseProxyPort", value.ToString());
            }
        }

        private static string ReadValue(string pstrValueToRead)
        {
            try
            {
                XPathDocument doc = new XPathDocument(SettingsPath);
                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr = nav.Compile(@"/settings/" + pstrValueToRead);
                XPathNodeIterator iterator = nav.Select(expr);
                while (iterator.MoveNext())
                {
                    return iterator.Current.Value;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ReadValueSafe(string pstrValueToRead, string defaultValue = "")
        {
            string value = ReadValue(pstrValueToRead);
            return (!string.IsNullOrEmpty(value)) ? value: defaultValue;
        }

        private static void WriteValue(string pstrValueToRead, string pstrValueToWrite)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                if (File.Exists(SettingsPath))
                {
                    using (var reader = new XmlTextReader(SettingsPath))
                    {
                        doc.Load(reader);
                    }
                }
                else
                {
                    var dir = Path.GetDirectoryName(SettingsPath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    doc.AppendChild(doc.CreateElement("settings"));
                }

                XmlElement root = doc.DocumentElement;
                XmlNode oldNode = root.SelectSingleNode(@"/settings/" + pstrValueToRead);
                if (oldNode == null) // create if not exist
                {
                    oldNode = doc.SelectSingleNode("settings");
                    oldNode.AppendChild(doc.CreateElement(pstrValueToRead)).InnerText = pstrValueToWrite;
                    doc.Save(SettingsPath);
                    return;
                }
                oldNode.InnerText = pstrValueToWrite;
                doc.Save(SettingsPath);
            }
            catch
            {
            }
        }
    }
}