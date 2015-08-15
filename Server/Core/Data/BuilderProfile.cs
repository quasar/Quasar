using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace xServer.Core.Data
{
    public class BuilderProfile
    {
        private readonly string _profilePath;

        public BuilderProfile(string profilePath)
        {
            if (string.IsNullOrEmpty(profilePath)) throw new ArgumentException("Invalid Profile Path");
            _profilePath = Path.Combine(Application.StartupPath, "Profiles\\" + profilePath);
        }

        public string ReadValue(string pstrValueToRead)
        {
            try
            {
                XPathDocument doc = new XPathDocument(_profilePath);
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

                if (File.Exists(_profilePath))
                {
                    using (var reader = new XmlTextReader(_profilePath))
                    {
                        doc.Load(reader);
                    }
                }
                else
                {
                    var dir = Path.GetDirectoryName(_profilePath);
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
                    doc.Save(_profilePath);
                    return true;
                }
                oldNode.InnerText = pstrValueToWrite;
                doc.Save(_profilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}