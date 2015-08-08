using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
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