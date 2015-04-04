using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Drives : IPacket
    {
        public void Execute(Client client)
        {
            client.Send<Drives>(this);
        }
    }
}