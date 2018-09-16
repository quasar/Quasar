using ProtoBuf;
using Quasar.Common.Models;
using System.Collections.Generic;

namespace Quasar.Common.Messages
{
    [ProtoContract]
    public class GetStartupItemsResponse : IMessage
    {
        [ProtoMember(1)]
        public List<StartupItem> StartupItems { get; set; }
    }
}
