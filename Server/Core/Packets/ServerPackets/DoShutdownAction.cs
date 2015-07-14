using ProtoBuf;
using xServer.Core.Networking;
using ShutdownAction = xServer.Core.Commands.CommandHandler.ShutdownAction;

namespace xServer.Core.Packets.ServerPackets
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