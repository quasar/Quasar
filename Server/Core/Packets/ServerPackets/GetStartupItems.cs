using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetStartupItems : IPacket
    {
        public GetStartupItems()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}