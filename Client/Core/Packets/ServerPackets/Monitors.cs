using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Monitors : IPacket
    {
        public void Execute(Client client)
        {
            client.Send<Monitors>(this);
        }
    }
}