using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class SetAuthenticationSuccess : IPacket
    {
        public SetAuthenticationSuccess()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
