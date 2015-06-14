using ProtoBuf;
using xClient.Core.Networking;

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
            client.Send(this);
        }
    }
}