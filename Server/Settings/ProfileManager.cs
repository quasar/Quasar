using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace xServer.Settings
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
                    root.AppendChild(doc.CreateElement("ChangeAsmInfo")).InnerText = "False";
                    root.AppendChild(doc.CreateElement("ProductName"));
                    root.AppendChild(doc.CreateElement("Description"));
                    root.AppendChild(doc.CreateElement("CompanyName"));
                    root.AppendChild(doc.CreateElement("Copyright"));
                    root.AppendChild(doc.CreateElement("Trademarks"));
                    root.AppendChild(doc.CreateElement("OriginalFilename"));
                    root.AppendChild(doc.CreateElement("ProductVersion"));
                    root.AppendChild(doc.CreateElement("FileVersion"));

                    doc.Save(settingsFilePath);
                }
            }
            catch
            {
            }
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