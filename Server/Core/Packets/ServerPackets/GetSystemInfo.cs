using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetSystemInfo : IPacket
    {
        public void Execute(Client client)
        {
            client.Send<GetSystemInfo>(this);
        }
    }
}