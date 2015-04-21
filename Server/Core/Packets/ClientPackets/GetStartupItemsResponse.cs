using System.Collections.Generic;
using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetStartupItemsResponse : IPacket
    {
        [ProtoMember(1)]
        public Dictionary<string, int> StartupItems { get; set; }

        public GetStartupItemsResponse()
        {
        }

        public GetStartupItemsResponse(Dictionary<string, int> startupitems)
        {
            this.StartupItems = startupitems;
        }

        public void Execute(Client client)
        {
            client.Send<GetStartupItemsResponse>(this);
        }
    }
}