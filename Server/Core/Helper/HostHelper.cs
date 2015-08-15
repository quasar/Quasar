using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using xServer.Core.Data;
using xServer.Core.Utilities;

namespace xServer.Core.Helper
{
    public static class HostHelper
    {
        public static List<Host> GetHostsList(string rawHosts)
        {
            List<Host> hostsList = new List<Host>();

            if (string.IsNullOrEmpty(rawHosts)) return hostsList;

            var hosts = rawHosts.Split(';');

            foreach (var hostPart in from host in hosts where (!string.IsNullOrEmpty(host) && host.Contains(':')) select host.Split(':'))
            {
                if (hostPart.Length != 2 || hostPart[0].Length < 1 || hostPart[1].Length < 1) continue; // invalid, ignore host

                ushort port;
                if (!ushort.TryParse(hostPart[1], out port)) continue; // invalid, ignore host

                hostsList.Add(new Host { Hostname = hostPart[0], Port = port });
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

        public static string GetRawHosts(BindingList<Host> hosts)
        {
            StringBuilder rawHosts = new StringBuilder();

            foreach (var host in hosts)
                rawHosts.Append(host + ";");

            return rawHosts.ToString();
        }
    }
}
