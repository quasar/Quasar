using Core.Packets;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Packets.ClientPackets
{
    [ProtoContract]
    public class UnknownPacket : IPacket
    {
        [ProtoMember(1)]
        public IPacket Packet { get; set; }

        public UnknownPacket() { }
        public UnknownPacket(IPacket packet) { Packet = packet; }

        public void Execute(Client client)
        {
            client.Send<UnknownPacket>(this);
        }
    }
}
