using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace xServer.Core.Data
{
    public static class XMLSettings
    {
        public const string VERSION = "RELEASE4.1";
        public static ushort ListenPort { get; set; }
        public static bool ShowToU { get; set; }
        public static bool AutoListen { get; set; }
        public static bool ShowPopup { get; set; }
        public static bool UseUPnP { get; set; }
        public static bool ShowToolTip { get; set; }
        public static string Password { get; set; }
        public static bool IntegrateNoIP { get; set; }
        public static string NoIPHost { get; set; }
        public static string NoIPUsername { get; set; }
        public static string NoIPPassword { get; set; }
        public static string SaveFormat { get; set; }

        private static readonly string _settingsPath = Path.Combine(Application.StartupPath, "settings.xml");

        public static void ReadSettings()
        {
            ListenPort = ushort.Parse(ReadValueSafe("ListenPort", "4782"));
            ShowToU = bool.Parse(ReadValueSafe("ShowToU", "True"));
            AutoListen = bool.Parse(ReadValueSafe("AutoListen", "False"));
            ShowPopup = bool.Parse(ReadValueSafe("ShowPopup", "False"));
            UseUPnP = bool.Parse(ReadValueSafe("UseUPnP", "False"));
            SaveFormat = ReadValueSafe("SaveFormat", "APP - URL - USER:PASS");
            ShowToolTip = bool.Parse(ReadValueSafe("ShowToolTip", "False"));
            IntegrateNoIP = bool.Parse(ReadValueSafe("EnableNoIPUpdater", "False"));
            NoIPHost = ReadValueSafe("NoIPHost");
            NoIPUsername = ReadValueSafe("NoIPUsername");
            NoIPPassword = ReadValueSafe("NoIPPassword");
            Password = ReadValueSafe("Password", "1234");
        }

        public static string ReadValue(string pstrValueToRead)
        {
            try
            {
                XPathDocument doc = new XPathDocument(_settingsPath);
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

        public static string ReadValueSafe(string pstrValueToRead, string defaultValue = "")
        {
            string value = ReadValue(pstrValueToRead);
            return (!string.IsNullOrEmpty(value)) ? value: defaultValue;
        }

        public static bool WriteValue(string pstrValueToRead, string pstrValueToWrite)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                if (File.Exists(_settingsPath))
                {
                    using (var reader = new XmlTextReader(_settingsPath))
                    {
                        doc.Load(reader);
                    }
                }
                else
                {
                    var dir = Path.GetDirectoryName(_settingsPath);
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
                    doc.Save(_settingsPath);
                    return true;
                }
                oldNode.InnerText = pstrValueToWrite;
                doc.Save(_settingsPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}