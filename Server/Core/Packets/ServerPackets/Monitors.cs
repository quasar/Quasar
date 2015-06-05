using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Monitors : IPacket
    {
        public Monitors()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}