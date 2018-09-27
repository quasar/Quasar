using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using xClient.Core.Data;

namespace xClient.Core.Utilities
{
    public class HostsManager
    {
        public bool IsEmpty { get { return _hosts.Count == 0; } }

        private readonly Queue<Host> _hosts = new Queue<Host>();

        public HostsManager(List<Host> hosts)
        {
            foreach(var host in hosts)
                _hosts.Enqueue(host);
        }

        public Host GetNextHost()
        {
            var temp = _hosts.Dequeue();
            _hosts.Enqueue(temp); // add to the end of the queue

            temp.IpAddress = GetIp(temp);
            return temp;
        }

        private static IPAddress GetIp(Host host)
        {
            if (string.IsNullOrEmpty(host.Hostname)) return null;

            IPAddress ip;
            if (IPAddress.TryParse(host.Hostname, out ip))
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    if (!Socket.OSSupportsIPv6) return null;
                }
                return ip;
            }

            var ipAddresses = Dns.GetHostEntry(host.Hostname).AddressList;
            foreach (IPAddress ipAddress in ipAddresses)
            {
                switch (ipAddress.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        return ipAddress;
                    case AddressFamily.InterNetworkV6:
                        /* Only use resolved IPv6 if no IPv4 address available,
                         * otherwise it could be possible that the router the client
                         * is using to connect to the internet doesn't support IPv6.
                         */
                        if (ipAddresses.Length == 1)
                            return ipAddress;
                        break;
                }
            }

            return ip;
        }
    }
}
