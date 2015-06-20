using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetSystemInfo : IPacket
    {
        public GetSystemInfo()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}