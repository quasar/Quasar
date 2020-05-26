using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quasar.Common.DNS
{
    public class HostsConverter
    {
        public List<Host> RawHostsToList(string rawHosts)
        {
            List<Host> hostsList = new List<Host>();

            if (string.IsNullOrEmpty(rawHosts)) return hostsList;

            var hosts = rawHosts.Split(';');

            foreach (var host in hosts)
            {
                if ((string.IsNullOrEmpty(host) || !host.Contains(':'))) continue; // invalid host, ignore

                ushort port;
                if (!ushort.TryParse(host.Substring(host.LastIndexOf(':') + 1), out port)) continue; // invalid, ignore host

                hostsList.Add(new Host { Hostname = host.Substring(0, host.LastIndexOf(':')), Port = port });
            }

            return hostsList;
        }

        public  string ListToRawHosts(IList<Host> hosts)
        {
            StringBuilder rawHosts = new StringBuilder();

            foreach (var host in hosts)
                rawHosts.Append(host + ";");

            return rawHosts.ToString();
        }
    }
}
