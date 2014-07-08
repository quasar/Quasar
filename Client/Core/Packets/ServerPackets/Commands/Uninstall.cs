using ProtoBuf;

namespace Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Uninstall : IPacket
    {
        public Uninstall() { }

        public void Execute(Client client)
        {
            client.Send<Uninstall>(this);
        }
    }
}
