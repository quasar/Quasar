using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetSystemInfo : IPacket
    {
        public GetSystemInfo()
        {
        }

        public void Execute(Client client)
        {
            client.Send<GetSystemInfo>(this);
        }
    }
}