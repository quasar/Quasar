using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace xServer.Settings
{
    public class ProfileManager
    {
        private readonly string _settingsFilePath;

        public ProfileManager(string settingsFile)
        {
            if (string.IsNullOrEmpty(settingsFile)) throw new ArgumentException();
            _settingsFilePath = Path.Combine(Application.StartupPath, "Profiles\\" + settingsFile);
        }

        public string ReadValue(string pstrValueToRead)
        {
            try
            {
                XPathDocument doc = new XPathDocument(_settingsFilePath);
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

        public string ReadValueSafe(string pstrValueToRead, string defaultValue = "")
        {
            string value = ReadValue(pstrValueToRead);
            return (!string.IsNullOrEmpty(value)) ? value : defaultValue;
        }

        public bool WriteValue(string pstrValueToRead, string pstrValueToWrite)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                using (var reader = new XmlTextReader(_settingsFilePath))
                {
                    doc.Load(reader);
                }

                XmlElement root = doc.DocumentElement;
                XmlNode oldNode = root.SelectSingleNode(@"/settings/" + pstrValueToRead);
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