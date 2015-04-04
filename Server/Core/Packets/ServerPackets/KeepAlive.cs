using System;
using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    internal class KeepAlive : IPacket
    {
        public Client Client;

        [ProtoMember(1)]
        public DateTime TimeSent { get; private set; }

        public void Execute(Client client)
        {
            TimeSent = DateTime.Now;
            Client = client;
            client.Send<KeepAlive>(this);
        }
    }
}