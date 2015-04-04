using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Reconnect : IPacket
    {
        public void Execute(Client client)
        {
            client.Send<Reconnect>(this);
        }
    }
}