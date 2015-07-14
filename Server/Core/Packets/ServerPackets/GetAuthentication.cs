using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
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