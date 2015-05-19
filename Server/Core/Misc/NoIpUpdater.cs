using System;
using System.Net;
using System.Text;
using System.Threading;
using xServer.Settings;

namespace xServer.Core.Misc
{
    public static class NoIpUpdater
    {
        private static bool _running;

        public static void Start()
        {
            if (_running) return;
            Thread updateThread = new Thread(BackgroundUpdater) {IsBackground = true};
            updateThread.Start();
        }

        private static void BackgroundUpdater()
        {
            _running = true;
            while (XMLSettings.IntegrateNoIP)
            {
                try
                {
                    string wanIp = string.Empty;
                    using (WebClient wc = new WebClient())
                    {
                        wanIp = wc.DownloadString("http://icanhazip.com/");
                    }

                    if (!string.IsNullOrEmpty(wanIp))
                    {
                        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://dynupdate.no-ip.com/nic/update?hostname={0}&myip={1}", XMLSettings.NoIPHost, wanIp));
                        request.UserAgent = string.Format("X IP Automation Tool/3 {0}", XMLSettings.NoIPUsername);
                        request.Timeout = 20000;
                        request.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", XMLSettings.NoIPUsername, XMLSettings.NoIPPassword)))));
                        request.Method = "GET";

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                        }
                    }

                }
                catch
                {
                }

                Thread.Sleep(TimeSpan.FromMinutes(10));
            }
            _running = false;
        }
    }
}
