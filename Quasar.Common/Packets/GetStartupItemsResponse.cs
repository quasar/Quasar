using System.Collections.Generic;
using ProtoBuf;

namespace Quasar.Common.Packets
{
    [ProtoContract]
    public class GetStartupItemsResponse : IPacket
    {
        [ProtoMember(1)]
        public List<string> StartupItems { get; set; }
    }
}
