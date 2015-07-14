using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetDrives : IPacket
    {
        public GetDrives()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}