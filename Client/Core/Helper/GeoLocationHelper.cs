using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using xClient.Core.Data;

namespace xClient.Core.Helper
{
    public static class GeoLocationHelper
    {
        public static readonly string[] ImageList =
        {
            "ad", "ae", "af", "ag", "ai", "al",
            "am", "an", "ao", "ar", "as", "at", "au", "aw", "ax", "az", "ba",
            "bb", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bm", "bn", "bo",
            "br", "bs", "bt", "bv", "bw", "by", "bz", "ca", "catalonia", "cc",
            "cd", "cf", "cg", "ch", "ci", "ck", "cl", "cm", "cn", "co", "cr",
            "cs", "cu", "cv", "cx", "cy", "cz", "de", "dj", "dk", "dm", "do",
            "dz", "ec", "ee", "eg", "eh", "england", "er", "es", "et",
            "europeanunion", "fam", "fi", "fj", "fk", "fm", "fo", "fr", "ga",
            "gb", "gd", "ge", "gf", "gh", "gi", "gl", "gm", "gn", "gp", "gq",
            "gr", "gs", "gt", "gu", "gw", "gy", "hk", "hm", "hn", "hr", "ht",
            "hu", "id", "ie", "il", "in", "io", "iq", "ir", "is", "it", "jm",
            "jo", "jp", "ke", "kg", "kh", "ki", "km", "kn", "kp", "kr", "kw",
            "ky", "kz", "la", "lb", "lc", "li", "lk", "lr", "ls", "lt", "lu",
            "lv", "ly", "ma", "mc", "md", "me", "mg", "mh", "mk", "ml", "mm",
            "mn", "mo", "mp", "mq", "mr", "ms", "mt", "mu", "mv", "mw", "mx",
            "my", "mz", "na", "nc", "ne", "nf", "ng", "ni", "nl", "no", "np",
            "nr", "nu", "nz", "om", "pa", "pe", "pf", "pg", "ph", "pk", "pl",
            "pm", "pn", "pr", "ps", "pt", "pw", "py", "qa", "re", "ro", "rs",
            "ru", "rw", "sa", "sb", "sc", "scotland", "sd", "se", "sg", "sh",
            "si", "sj", "sk", "sl", "sm", "sn", "so", "sr", "st", "sv", "sy",
            "sz", "tc", "td", "tf", "tg", "th", "tj", "tk", "tl", "tm", "tn",
            "to", "tr", "tt", "tv", "tw", "tz", "ua", "ug", "um", "us", "uy",
            "uz", "va", "vc", "ve", "vg", "vi", "vn", "vu", "wales", "wf",
            "ws", "ye", "yt", "za", "zm", "zw"
        };

        public static int ImageIndex { get; set; }
        public static GeoInformation GeoInfo { get; private set; }
        public static DateTime LastLocated { get; private set; }
        public static bool LocationCompleted { get; private set; }

        static GeoLocationHelper()
        {
            LastLocated = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public static void Initialize()
        {
            TimeSpan lastLocateTry = new TimeSpan(DateTime.UtcNow.Ticks - LastLocated.Ticks);

            // last location was 30 minutes ago or last location has not completed
            if (lastLocateTry.TotalMinutes > 30 || !LocationCompleted)
            {
                TryLocate();

                if (string.IsNullOrEmpty(GeoInfo.CountryCode) || string.IsNullOrEmpty(GeoInfo.Country))
                {
                    ImageIndex = 247; // question icon
                    return;
                }

                for (int i = 0; i < ImageList.Length; i++)
                {
                    if (ImageList[i] == GeoInfo.CountryCode.ToLower())
                    {
                        ImageIndex = i;
                        break;
                    }
                }
            }
        }

        private static void TryLocate()
        {
            LocationCompleted = false;

            try
            {
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(GeoInformation));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://ip-api.com/json/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0";
                request.Proxy = null;
                request.Timeout = 10000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseString = reader.ReadToEnd();

                            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
                            {
                                GeoInfo = (GeoInformation)jsonSerializer.ReadObject(ms);
                            }
                        }
                    }
                }

                LastLocated = DateTime.UtcNow;
                LocationCompleted = true;
            }
            catch
            {
                TryLocateFallback();
            }
        }

        private static void TryLocateFallback()
        {
            GeoInfo = new GeoInformation();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://freegeoip.net/xml/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0";
                request.Proxy = null;
                request.Timeout = 10000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseString = reader.ReadToEnd();

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(responseString);

                            string xmlIp = doc.SelectSingleNode("Response//IP").InnerXml;
                            string xmlCountry = doc.SelectSingleNode("Response//CountryName").InnerXml;
                            string xmlCountryCode = doc.SelectSingleNode("Response//CountryCode").InnerXml;
                            string xmlRegion = doc.SelectSingleNode("Response//RegionName").InnerXml;
                            string xmlCity = doc.SelectSingleNode("Response//City").InnerXml;
                            string timeZone = doc.SelectSingleNode("Response//TimeZone").InnerXml;

                            GeoInfo.Ip = (!string.IsNullOrEmpty(xmlIp))
                                ? xmlIp
                                : "-";
                            GeoInfo.Country = (!string.IsNullOrEmpty(xmlCountry))
                                ? xmlCountry
                                : "Unknown";
                            GeoInfo.CountryCode = (!string.IsNullOrEmpty(xmlCountryCode))
                                ? xmlCountryCode
                                : "-";
                            GeoInfo.Region = (!string.IsNullOrEmpty(xmlRegion))
                                ? xmlRegion
                                : "Unknown";
                            GeoInfo.City = (!string.IsNullOrEmpty(xmlCity))
                                ? xmlCity
                                : "Unknown";
                            GeoInfo.Timezone = (!string.IsNullOrEmpty(timeZone))
                                ? timeZone
                                : "Unknown";

                            GeoInfo.Isp = "Unknown"; // freegeoip does not support ISP detection
                        }
                    }
                }

                LastLocated = DateTime.UtcNow;
                LocationCompleted = true;
            }
            catch
            {
                GeoInfo.Country = "Unknown";
                GeoInfo.CountryCode = "-";
                GeoInfo.Region = "Unknown";
                GeoInfo.City = "Unknown";
                GeoInfo.Timezone = "Unknown";
                GeoInfo.Isp = "Unknown";
                LocationCompleted = false;
            }

            if (string.IsNullOrEmpty(GeoInfo.Ip))
                TryGetWanIp();
        }

        private static void TryGetWanIp()
        {
            string wanIp = "-";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://api.ipify.org/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:48.0) Gecko/20100101 Firefox/48.0";
                request.Proxy = null;
                request.Timeout = 5000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            wanIp = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            GeoInfo.Ip = wanIp;
        }
    }
}