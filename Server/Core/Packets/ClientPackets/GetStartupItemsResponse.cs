using System.Collections.Generic;
using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetStartupItemsResponse : IPacket
    {
        public GetStartupItemsResponse()
        {
        }

        public GetStartupItemsResponse(Dictionary<string, int> startupitems)
        {
            StartupItems = startupitems;
        }

        [ProtoMember(1)]
        public Dictionary<string, int> StartupItems { get; set; }

        public void Execute(Client client)
        {
            client.Send<GetStartupItemsResponse>(this);
        }
    }
}