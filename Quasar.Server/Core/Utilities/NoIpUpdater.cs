using System;
using System.Net;
using System.Text;
using System.Threading;
using xServer.Core.Data;

namespace xServer.Core.Utilities
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
            while (Settings.EnableNoIPUpdater)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://dynupdate.no-ip.com/nic/update?hostname={0}", Settings.NoIPHost));
                    request.Proxy = null;
                    request.UserAgent = string.Format("Quasar No-Ip Updater/2.0 {0}", Settings.NoIPUsername);
                    request.Timeout = 10000;
                    request.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", Settings.NoIPUsername, Settings.NoIPPassword)))));
                    request.Method = "GET";

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
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
