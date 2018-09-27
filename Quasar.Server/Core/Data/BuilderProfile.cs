using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using xServer.Core.Helper;

namespace xServer.Core.Data
{
    public class BuilderProfile
    {
        private readonly string _profilePath;

        public string Hosts
        {
            get
            {
                return ReadValueSafe("Hosts");
            }
            set
            {
                WriteValue("Hosts", value);
            }
        }

        public string Tag
        {
            get
            {
                return ReadValueSafe("Tag", "Office04");
            }
            set
            {
                WriteValue("Tag", value);
            }
        }

        public string Password
        {
            get
            {
                return ReadValueSafe("Password", Settings.Password);
            }
            set
            {
                WriteValue("Password", value);
            }
        }

        public int Delay
        {
            get
            {
                return int.Parse(ReadValueSafe("Delay", "3000"));
            }
            set
            {
                WriteValue("Delay", value.ToString());
            }
        }

        public string Mutex
        {
            get
            {
                return ReadValueSafe("Mutex", FormatHelper.GenerateMutex());
            }
            set
            {
                WriteValue("Mutex", value);
            }
        }

        public bool InstallClient
        {
            get
            {
                return bool.Parse(ReadValueSafe("InstallClient", "False"));
            }
            set
            {
                WriteValue("InstallClient", value.ToString());
            }
        }

        public string InstallName
        {
            get
            {
                return ReadValueSafe("InstallName", "Client");
            }
            set
            {
                WriteValue("InstallName", value);
            }
        }

        public short InstallPath
        {
            get
            {
                return short.Parse(ReadValueSafe("InstallPath", "1"));
            }
            set
            {
                WriteValue("InstallPath", value.ToString());
            }
        }

        public string InstallSub
        {
            get
            {
                return ReadValueSafe("InstallSub", "SubDir");
            }
            set
            {
                WriteValue("InstallSub", value);
            }
        }

        public bool HideFile
        {
            get
            {
                return bool.Parse(ReadValueSafe("HideFile", "False"));
            }
            set
            {
                WriteValue("HideFile", value.ToString());
            }
        }

        public bool HideSubDirectory
        {
            get
            {
                return bool.Parse(ReadValueSafe("HideSubDirectory", "False"));
            }
            set
            {
                WriteValue("HideSubDirectory", value.ToString());
            }
        }

        public bool AddStartup
        {
            get
            {
                return bool.Parse(ReadValueSafe("AddStartup", "False"));
            }
            set
            {
                WriteValue("AddStartup", value.ToString());
            }
        }

        public string RegistryName
        {
            get
            {
                return ReadValueSafe("RegistryName", "Quasar Client Startup");
            }
            set
            {
                WriteValue("RegistryName", value);
            }
        }

        public bool ChangeIcon
        {
            get
            {
                return bool.Parse(ReadValueSafe("ChangeIcon", "False"));
            }
            set
            {
                WriteValue("ChangeIcon", value.ToString());
            }
        }

        public string IconPath
        {
            get
            {
                return ReadValueSafe("IconPath");
            }
            set
            {
                WriteValue("IconPath", value);
            }
        }

        public bool ChangeAsmInfo
        {
            get
            {
                return bool.Parse(ReadValueSafe("ChangeAsmInfo", "False"));
            }
            set
            {
                WriteValue("ChangeAsmInfo", value.ToString());
            }
        }

        public bool Keylogger
        {
            get
            {
                return bool.Parse(ReadValueSafe("Keylogger", "False"));
            }
            set
            {
                WriteValue("Keylogger", value.ToString());
            }
        }

        public string LogDirectoryName
        {
            get
            {
                return ReadValueSafe("LogDirectoryName", "Logs");
            }
            set
            {
                WriteValue("LogDirectoryName", value);
            }
        }

        public bool HideLogDirectory
        {
            get
            {
                return bool.Parse(ReadValueSafe("HideLogDirectory", "False"));
            }
            set
            {
                WriteValue("HideLogDirectory", value.ToString());
            }
        }

        public string ProductName
        {
            get
            {
                return ReadValueSafe("ProductName");
            }
            set
            {
                WriteValue("ProductName", value);
            }
        }

        public string Description
        {
            get
            {
                return ReadValueSafe("Description");
            }
            set
            {
                WriteValue("Description", value);
            }
        }

        public string CompanyName
        {
            get
            {
                return ReadValueSafe("CompanyName");
            }
            set
            {
                WriteValue("CompanyName", value);
            }
        }

        public string Copyright
        {
            get
            {
                return ReadValueSafe("Copyright");
            }
            set
            {
                WriteValue("Copyright", value);
            }
        }

        public string Trademarks
        {
            get
            {
                return ReadValueSafe("Trademarks");
            }
            set
            {
                WriteValue("Trademarks", value);
            }
        }

        public string OriginalFilename
        {
            get
            {
                return ReadValueSafe("OriginalFilename");
            }
            set
            {
                WriteValue("OriginalFilename", value);
            }
        }

        public string ProductVersion
        {
            get
            {
                return ReadValueSafe("ProductVersion");
            }
            set
            {
                WriteValue("ProductVersion", value);
            }
        }

        public string FileVersion
        {
            get
            {
                return ReadValueSafe("FileVersion");
            }
            set
            {
                WriteValue("FileVersion", value);
            }
        }

        public BuilderProfile(string profileName)
        {
            if (string.IsNullOrEmpty(profileName)) throw new ArgumentException("Invalid Profile Path");
            _profilePath = Path.Combine(Application.StartupPath, "Profiles\\" + profileName + ".xml");
        }

        private string ReadValue(string pstrValueToRead)
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

        private string ReadValueSafe(string pstrValueToRead, string defaultValue = "")
        {
            string value = ReadValue(pstrValueToRead);
            return (!string.IsNullOrEmpty(value)) ? value : defaultValue;
        }

        private void WriteValue(string pstrValueToRead, string pstrValueToWrite)
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
                    return;
                }
                oldNode.InnerText = pstrValueToWrite;
                doc.Save(_profilePath);
            }
            catch
            {
            }
        }
    }
}