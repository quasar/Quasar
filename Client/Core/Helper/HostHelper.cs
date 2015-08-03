using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Utilities;

namespace xClient.Core.Helper
{
    public static class HostHelper
    {
        public static List<Host> GetHostsList(string rawHosts)
        {
            List<Host> hostsList = new List<Host>();

            var hosts = rawHosts.Split(';');

            foreach (var hostPart in from host in hosts where !string.IsNullOrEmpty(host) select host.Split(':'))
            {
                if (hostPart.Length != 2 || hostPart[0].Length < 1 || hostPart[1].Length < 1) throw new Exception("Invalid host");

                ushort port;
                if (!ushort.TryParse(hostPart[1], out port)) throw new Exception("Invalid host");

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
    }
}
