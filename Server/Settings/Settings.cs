using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace xRAT_2.Settings
{
    public static class XMLSettings
    {
        public const string VERSION = "RELEASE1";
        public static ushort ListenPort { get; set; }
        public static bool AutoListen { get; set; }
        public static bool ShowPopup { get; set; }
        public static string Password { get; set; }

        private static string settingsFilePath = Path.Combine(Application.StartupPath, "settings.xml");

        public static bool WriteDefaultSettings()
        {
            try
            {
                if (!File.Exists(settingsFilePath))
                {
                    XmlDocument doc = new XmlDocument();
                    XmlNode root = doc.CreateElement("settings");
                    doc.AppendChild(root);

                    root.AppendChild(doc.CreateElement("ListenPort")).InnerText = "4782";
                    root.AppendChild(doc.CreateElement("AutoListen")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("ShowPopup")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("Password")).InnerText = "1234";
                    root.AppendChild(doc.CreateElement("ShowToU")).InnerText = "True";

                    doc.Save(settingsFilePath);
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
                XPathDocument doc = new XPathDocument(settingsFilePath);
                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr;
                expr = nav.Compile(@"/settings/" + pstrValueToRead);
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
                XmlNode oldNode;
                XmlDocument doc = new XmlDocument();
                using (var reader = new XmlTextReader(settingsFilePath))
                {
                    doc.Load(reader);
                }

                XmlElement root = doc.DocumentElement;
                oldNode = root.SelectSingleNode(@"/settings/" + pstrValueToRead);
                if (oldNode == null) // create if not exist
                {
                    oldNode = doc.SelectSingleNode("settings");
                    oldNode.AppendChild(doc.CreateElement(pstrValueToRead)).InnerText = pstrValueToWrite;
                    doc.Save(settingsFilePath);
                    return true;
                }
                oldNode.InnerText = pstrValueToWrite;
                doc.Save(settingsFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}