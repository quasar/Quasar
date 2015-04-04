using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetStartupItems : IPacket
    {
        public void Execute(Client client)
        {
            client.Send<GetStartupItems>(this);
        }
    }
}