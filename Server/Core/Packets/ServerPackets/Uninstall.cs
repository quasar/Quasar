using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Uninstall : IPacket
    {
        public void Execute(Client client)
        {
            client.Send<Uninstall>(this);
        }
    }
}