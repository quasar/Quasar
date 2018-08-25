using System.Collections.Generic;
using ProtoBuf;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetStartupItemsResponse : IMessage
    {
        [ProtoMember(1)]
        public List<string> StartupItems { get; set; }
    }
}
