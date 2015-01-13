using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Core
{
    class GeoIP
    {
        public string WANIP { get; private set; }
        public string Country { get; private set; }
        public string CountryCode { get; private set; }
        public string Region { get; private set; }
        public string City { get; private set; }

        private string GetWANIP()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://checkip.dyndns.org");
            request.Proxy = null;
            request.Timeout = 5000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseString = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            responseString = (new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b")).Match(responseString).Value;
            return responseString;
        }

        public GeoIP()
        {
            try
            {
                WANIP = GetWANIP();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("http://api.hackertarget.com/geoip/?q={0}", WANIP));
                request.Proxy = null;
                request.Timeout = 5000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseString = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                string[] resp = responseString.Split('\n');

                foreach (string line in resp)
                {
                    if (line.StartsWith("Country: "))
                    {
                        Country = line.Replace("Country: ", string.Empty);
                        CountryCode = Country;
                    }
                    else if (line.StartsWith("State: "))
                    {
                        Region = line.Replace("State: ", string.Empty);
                    }
                    else if (line.StartsWith("City: "))
                    {
                        City = line.Replace("City: ", string.Empty);
                    }
                }
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
