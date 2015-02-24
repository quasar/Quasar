using System.IO;
using System.Net;
using System.Xml;

namespace xClient.Core
{
    class GeoIP
    {
        public string WANIP { get; private set; }
        public string Country { get; private set; }
        public string CountryCode { get; private set; }
        public string Region { get; private set; }
        public string City { get; private set; }

        public GeoIP()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://freegeoip.net/xml/");
                request.Proxy = null;
                request.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseString = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(responseString);

                WANIP = doc.SelectSingleNode("Response//IP").InnerXml;
                Country = (!string.IsNullOrEmpty(doc.SelectSingleNode("Response//CountryName").InnerXml)) ? doc.SelectSingleNode("Response//CountryName").InnerXml : "Unknown";
                CountryCode = (!string.IsNullOrEmpty(doc.SelectSingleNode("Response//CountryCode").InnerXml)) ? doc.SelectSingleNode("Response//CountryCode").InnerXml : "-";
                Region = (!string.IsNullOrEmpty(doc.SelectSingleNode("Response//RegionName").InnerXml)) ? doc.SelectSingleNode("Response//RegionName").InnerXml : "Unknown";
                City = (!string.IsNullOrEmpty(doc.SelectSingleNode("Response//City").InnerXml)) ? doc.SelectSingleNode("Response//City").InnerXml : "Unknown";
            }
            catch
            {
                WANIP = "-";
                Country = "Unknown";
                CountryCode = "-";
                Region = "Unknown";
                City = "Unknown";
            }
        }
    }
}