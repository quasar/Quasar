using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace xServer.Settings
{
    public static class XMLSettings
    {
        public const string VERSION = "RELEASE3";
        public static ushort ListenPort { get; set; }
        public static bool ShowToU { get; set; }
        public static bool AutoListen { get; set; }
        public static bool ShowPopup { get; set; }
        public static bool UseUPnP { get; set; }
        public static bool ShowToolTip { get; set; }
        public static string Password { get; set; }

        private static string _settingsFilePath = Path.Combine(Application.StartupPath, "settings.xml");

        public static bool WriteDefaultSettings()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    XmlDocument doc = new XmlDocument();
                    XmlNode root = doc.CreateElement("settings");
                    doc.AppendChild(root);

                    root.AppendChild(doc.CreateElement("ListenPort")).InnerText = "4782";
                    root.AppendChild(doc.CreateElement("AutoListen")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("ShowPopup")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("Password")).InnerText = "1234";
                    root.AppendChild(doc.CreateElement("ShowToU")).InnerText = "True";
                    root.AppendChild(doc.CreateElement("UseUPnP")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("ShowToolTip")).InnerText = "False";

                    doc.Save(_settingsFilePath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ReadValue(string pstrValueToRead)
        {
            try
            {
                XPathDocument doc = new XPathDocument(_settingsFilePath);
                XPathNavigator nav = doc.CreateNavigator();
                var expr = nav.Compile(@"/settings/" + pstrValueToRead);
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

        public static bool WriteValue(string pstrValueToRead, string pstrValueToWrite)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                using (var reader = new XmlTextReader(_settingsFilePath))
                {
                    doc.Load(reader);
                }

                XmlElement root = doc.DocumentElement;
                var oldNode = root.SelectSingleNode(@"/settings/" + pstrValueToRead);
                if (oldNode == null) // create if not exist
                {
                    oldNode = doc.SelectSingleNode("settings");
                    oldNode.AppendChild(doc.CreateElement(pstrValueToRead)).InnerText = pstrValueToWrite;
                    doc.Save(_settingsFilePath);
                    return true;
                }
                oldNode.InnerText = pstrValueToWrite;
                doc.Save(_settingsFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}