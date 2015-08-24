using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetWebcamsResponse : IPacket
    {
        public int Number { get; set; }

        public GetWebcamsResponse()
        {
        }

        public GetWebcamsResponse(int number)
        {
            this.Number = number;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}