using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetProcesses : IPacket
    {
        public void Execute(Client client)
        {
            client.Send<GetProcesses>(this);
        }
    }
}