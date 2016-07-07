using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Data;

namespace xClient.Core.Helper
{
    public static class HostHelper
    {
        public static List<Host> GetHostsList(string rawHosts)
        {
            List<Host> hostsList = new List<Host>();

            if (string.IsNullOrEmpty(rawHosts)) return hostsList;

            var hosts = rawHosts.Split(';');

            foreach (var host in hosts)
            {
                // invalid host, ignore
                if ((string.IsNullOrEmpty(host) || !host.Contains(':'))) continue;

                ushort port;
                if (!ushort.TryParse(host.Substring(host.LastIndexOf(':') + 1), out port)) continue; // invalid, ignore host

                hostsList.Add(new Host {Hostname = host.Substring(0, host.LastIndexOf(':')), Port = port});
            }

            return hostsList;
        }

        public static string GetRawHosts(List<Host> hosts)
        {
            StringBuilder rawHosts = new StringBuilder();

            foreach (var host in hosts)
                rawHosts.Append(host + ";");

            return rawHosts.ToString();
        }
    }
}
