using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetLogs : IPacket
    {
        public GetLogs() { }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}