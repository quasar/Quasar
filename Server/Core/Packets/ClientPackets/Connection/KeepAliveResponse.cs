using Core.Packets;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Packets.ClientPackets
{
    [ProtoContract]
    internal class KeepAliveResponse : IPacket
    {
        [ProtoMember(1)]
        public DateTime TimeSent { get; set; }

        public void Execute(Client client)
        {
            client.Send<KeepAliveResponse>(this);
        }
    }
}
