using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace xRAT_2.Settings
{
    public class ProfileManager
    {
        private string settingsFilePath { get; set; }

        public ProfileManager(string settingsFile)
        {
            settingsFilePath = Path.Combine(Application.StartupPath, "Profiles\\" + settingsFile);

            try
            {
                if (!File.Exists(settingsFilePath))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(settingsFilePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath));

                    XmlDocument doc = new XmlDocument();
                    XmlNode root = doc.CreateElement("settings");
                    doc.AppendChild(root);

                    root.AppendChild(doc.CreateElement("Hostname"));
                    root.AppendChild(doc.CreateElement("ListenPort")).InnerText = "4782";
                    root.AppendChild(doc.CreateElement("Password")).InnerText = "1234";
                    root.AppendChild(doc.CreateElement("Delay")).InnerText = "5000";
                    root.AppendChild(doc.CreateElement("Mutex"));
                    root.AppendChild(doc.CreateElement("InstallClient")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("InstallName"));
                    root.AppendChild(doc.CreateElement("InstallPath")).InnerText = "1";
                    root.AppendChild(doc.CreateElement("InstallSub"));
                    root.AppendChild(doc.CreateElement("HideFile")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("AddStartup")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("RegistryName"));
                    root.AppendChild(doc.CreateElement("AdminElevation")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("ChangeIcon")).InnerText = "False";

                    doc.Save(settingsFilePath);
                }
            }
            catch
            { }
        }

        public string ReadValue(string pstrValueToRead)
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

        public bool WriteValue(string pstrValueToRead, string pstrValueToWrite)
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(settingsFilePath);
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                reader.Close();
                XmlNode oldNode;
                XmlElement root = doc.DocumentElement;
                oldNode = root.SelectSingleNode("/settings/" + pstrValueToRead);
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
